using Azati.Shx.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Azati.Shx.LoadShxDef {
	class Program {
		static void Main( string[] args ) {
			if( args.Length > 0 ) {
				var path = args[0];
				if( Directory.Exists( path ) ) {
					ProcessShxFiles( path );
				}
			} else {
				Console.WriteLine( "Usage: prg.exe  directory_of_shx_fonts_files" );
			}
		}

		private static void ProcessShxFiles( string path ) {

			var files = Directory.GetFiles( path, "*.shx", SearchOption.TopDirectoryOnly );
			if( files.Length > 0 ) {
				using( var ctx = new FontsDbContext( Properties.Settings.Default.ShxConnStrings ) ) {
					ctx.Configuration.AutoDetectChangesEnabled = true;
					var machineName = Environment.MachineName;
					var lds = ctx.LoadDirectories.Where( d => d.FullPath.Equals( path, StringComparison.InvariantCultureIgnoreCase )
															&& d.ComputerName.Equals( machineName, StringComparison.CurrentCultureIgnoreCase ) ).ToList();
					LoadDirectory loadDir;
					if( lds.Count == 0 ) {
						loadDir = new LoadDirectory { FullPath = path, ComputerName = machineName };
						ctx.LoadDirectories.Add( loadDir );
						ctx.SaveChanges();
					} else {
						loadDir = lds[0];
					}
					var tmpDir = CreateUniqueTempDirectory();
					foreach( var file in files ) {
						var fi = new FileInfo( file );
						var shpFile = Path.ChangeExtension( Path.Combine( tmpDir, fi.Name ), ".SHP" );
						try {
							if( File.Exists( shpFile ) ) {
								File.Delete( shpFile );
							}
							var shxFile = new ShxFontFile {
								ShxFileName = fi.Name,
								ShxFileSize = (int) fi.Length,
								ShxFileDate = fi.CreationTime,
								Crc32 = Crc32OfFile(file),
								Remarks = "Before Read"
							};
							loadDir.ShxFontFiles.Add( shxFile );
							ctx.SaveChanges();
							var exitCode = -1;
							using( var proc = new Process() ) {
								proc.StartInfo.FileName = Properties.Settings.Default.DumpShxProgram;
								proc.StartInfo.Arguments = "-o \"" + shpFile + "\" \"" + fi + "\"";
								proc.StartInfo.UseShellExecute = false;
								proc.StartInfo.CreateNoWindow = true;
								proc.Start();
								proc.WaitForExit( 1000 );
								exitCode = proc.ExitCode;
							}
							if( exitCode==0 && File.Exists( shpFile ) ) {
								Console.WriteLine( $"\t{shpFile}" );
								var shpFontFile = new ShpFontFile( ctx, shxFile );
								shpFontFile.Parse( shpFile );
							}
						} catch( Exception ex ) {
							Console.WriteLine( ex.Message );
						}
					}
				}
			}
		}


		/// <summary>
		/// Calculate CRC32 of a file
		/// </summary>
		/// <param name="path">Full path of the file</param>
		/// <see cref="https://damieng.com/blog/2006/08/08/calculating_crc32_in_c_and_net"/>
		/// <returns></returns>
		public static string Crc32OfFile( string path ) {
			var crc32 = new Crc32();
			var hash = string.Empty;
			using ( var fs = File.Open( path, FileMode.Open ) ) {
				foreach (byte b in crc32.ComputeHash( fs ))
					hash += b.ToString( "X2" );
			}
			return hash;
		}


		/// <summary>
		/// Creates the unique temporary directory.
		/// </summary>
		/// <returns>
		/// Directory path.
		/// </returns>
		public static string CreateUniqueTempDirectory() {
			var uniqueTempDir = Path.GetFullPath(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
			Directory.CreateDirectory( uniqueTempDir );
			return uniqueTempDir;
		}
	}
}

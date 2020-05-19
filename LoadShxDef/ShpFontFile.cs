using Azati.Shx.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azati.Shx.LoadShxDef {
	/*
		http://help.autodesk.com/view/ACD/2020/ENU/?guid=GUID-DE941DB5-7044-433C-AA68-2A9AE98A5713
		Each line in a shape definition file can contain up to 128 characters.
		Longer lines cannot be compiled.
		Because the program ignores blank lines and text to the right of a semicolon, you can embed comments in shape definition files.
		Each shape description has a header line of the following form and is followed by one or more lines containing specification bytes,
		separated by commas and terminated by a 0.

		*shapenumber,defbytes,shapename
	     specbyte1,specbyte2,specbyte3,...,0

		shapenumber
			A number, unique to the file, between 1 and 258 (and up to 32768 for Unicode fonts), and preceded by an asterisk (*).
			Non-Unicode font files use the shape numbers 256, 257, and 258 for the symbolic identifiers:
				Degree_Sign, Plus_Or_Minus_Sign, and Diameter_Symbol.
			For Unicode fonts these glyphs appear at the U+00B0, U+00B1, and U+2205 shape numbers and are part of the “Latin Extended-A” subset.

			Text fonts (files containing shape definitions for each character) require specific numbers corresponding to the value
			of each character in the ASCII code; other shapes can be assigned any numbers.

		defbytes
			The number of data bytes ( specbytes ) required to describe the shape, including the terminating 0.
			The limit is 2,000 bytes per shape.

		shapename
			The shape name. Shape names must be uppercase to be recognized.
			Names with lowercase characters are ignored and are usually used to label font shape definitions.

		specbyte
			A shape specification byte.
			Each specification byte is a code that defines either a vector length and direction or one of a number of special codes.
			A specification byte can be expressed in the shape definition file as either a decimal or hexadecimal value.
			If the first character of a specification byte is a 0 (zero), the two characters that follow are interpreted as hexadecimal values.

		==========
		Text fonts are files of shape definitions with shape numbers corresponding to an ASCII code for each character.
		Text fonts must include a special shape number 0 that conveys information about the font itself.
		Codes 1 through 31 are for control characters, only one of which is used in a text font:
			10 (LF)
			The line feed (LF) must drop down one line without drawing.
			This is used for repeated TEXT commands, to place succeeding lines below the first one.
			*10,5,lf
			2,8,(0,-10),0

		You can modify the spacing of lines by adjusting the downward movement specified by the LF shape definition.

		Text fonts must include a special shape number 0 that conveys information about the font itself.
		The format has the following syntax:

		*0,4,font-name
		above,below,modes,0

		The above value specifies the number of vector lengths above the baseline that the uppercase letters extend,
		and below indicates how far the lowercase letters descend below the baseline.
		The baseline is similar in concept to the lines on writing paper.
		These values define the basic character size and are used as scale factors for the height specified for the text object.

		The modes byte should be 0 for a horizontally oriented font and 2 for a dual-orientation (horizontal or vertical) font.
		The special 00E (14) command code is honored only when modes is set to 2.

		The standard fonts supplied with the program include a few additional characters required for dimensioning.
		%%d Degree symbol (°)
		%%p Plus/minus tolerance symbol (±)
		%%c Circle diameter dimensioning symbol

		You can use these and other %%nnn control sequences to specify a character.
		
		===========
		A single Unicode font, due to its large character set, is capable of supporting all languages and platforms.
		Unicode shape definition files are virtually identical in format and syntax to regular shape definition files.
		The main difference is in the syntax of the font header as shown in the following code:
		*UNIFONT,6,font-name
		above,below,modes,encoding,type,0

		The font-name , above , below , and modes parameters are the same as in regular fonts.
		The remaining two parameters are defined as follows:

		encoding
			Font encoding. Uses one of the following integer values.
			0 Unicode
			1 Packed multibyte 1
			2 Shape file

		type
			Font embedding information. Specifies whether the font is licensed. Licensed fonts must not be modified or exchanged.
			Bitcoded values can be added.
			0 Font can be embedded
			1 Font cannot be embedded
			2 Embedding is read-only

		Another important difference is the handling of the code 7 subshape reference.
		If a shape description includes a code 7 subshape reference, the data following the code 7 is interpreted as a two-byte value.
		This affects the total number of data bytes ( defbytes ) in the shape description header.
		For example, the following shape description is found in the romans.shp file:

		*00080,4,keuroRef
		7,020AC,0

		The second field in the header represents the total number of bytes in the shape description.
		If you are not used to working with Unicode font descriptions, you may be inclined to use three bytes rather than four,
		but this would cause an error during the compiling of the SHP file.
		This is true even if the shape number you are referencing is not in the two-byte range (below 255);
		the compiler always uses two bytes for this value, so you must account for that in the header.

		The only other difference between Unifont shape definitions and regular shape definitions is the shape numbers.
		The Unifont shape definitions that the program provides use hexadecimal shape numbers as opposed to decimal values.
		Although hexadecimal numbers are not required, their use makes it easier to cross-reference the shape numbers with the \U+ control character values.

		=========
		The first line of a Big Font shape definition file must be as follows:
		*BIGFONT nchars,nranges,b1,e1,b2,e2,...

		nchars represents the approximate number of character definitions in the set;
		if it is off by more than about 10 percent, either speed or file size suffers.
		You can use the rest of the line to name special character codes (escape codes) that signify the start of a two-byte code.
		For example, on Japanese computers, Kanji characters start with hexadecimal codes in the range 90-AF or E0-FF.
		When the operating system sees one of these codes, it reads the next byte and combines the two bytes into a code for one Kanji character.
		In the *BIGFONT line, nranges tells how many contiguous ranges of numbers are used as escape codes;
		b1 , e1 , b2 , e2 , and so on, define the beginning and ending codes in each range.
	*/
	public class ShpFontFile {
		private FontsDbContext _ctx;
		private ShxFontFile _shxFile;

		public ShpFontFile( FontsDbContext ctx, ShxFontFile shxFile ) {
			this._ctx = ctx;
			this._shxFile = shxFile;
		}

		internal void Parse( string shpFile ) {
			using( var sr = new StreamReader( shpFile ) ) {
				var remarks = string.Empty;
				var myType = string.Empty;
				while( sr.Peek() >= 0 ) {
					var line = sr.ReadLine();
					if ( line != null ) {
						if ( line.Trim().Length == 0 ) {
							continue;
						}
						if ( line.StartsWith( ";" ) ) {
							remarks += line + "\n";
							continue;
						}
						//	*shapenumber,defbytes,shapename
						//	*0,4,font-name
						//	*UNIFONT,6,font-name
						//	*BIGFONT nchars,nranges,b1,e1,b2,e2,...
						if( line.StartsWith( "*" ) ) {
							if ( myType.Length == 0 ) {
								if ( line.StartsWith("*0,4") ) {
									myType = "FONT";
									_shxFile.ShxType = myType;
									SetFontParam( sr, line );
								} else if ( line.StartsWith( "*UNIFONT" ) ) {
									myType = "UNIFONT";
									_shxFile.ShxType = myType;
									SetFontParam( sr, line );
								} else if( line.StartsWith( "*BIGFONT" ) ) {
									myType = "BIGFONT";
									_shxFile.ShxType = myType;
									_shxFile.HeaderBigFont = line;
									line = sr.ReadLine();
									SetFontParam( sr, line );
								} else {
									myType = "SYMBOL";
									_shxFile.ShxType = myType;
								}
								_ctx.ChangeTracker.DetectChanges();
								_ctx.SaveChanges();
							}
						}
					}
				}
			}

		}

		private void SetFontParam( StreamReader sr, string line ) {
			_shxFile.HeaderLine1 = line;
			var arr = line.Split( ',' );
			if ( arr.Length > 2 ) {
				_shxFile.FontName = arr[2];
			}
			var nextLine = sr.ReadLine();
			if ( nextLine != null ) {
				_shxFile.HeaderLine2 = nextLine;
				arr = nextLine.Split( ',' );
				/*
					*0,4,font-name
					above,below,modes,0
	
					*UNIFONT,6,font-name
					above,below,modes,encoding,type,0
				 */
				if ( arr.Length > 3 ) {
					_shxFile.Above = short.Parse( arr[0] );
					_shxFile.Below = short.Parse( arr[1] );
					_shxFile.Modes = byte.Parse( arr[2] );
				}
				if ( arr.Length > 5 ) {
					_shxFile.Encoding = byte.Parse( arr[3] );
					_shxFile.Type = byte.Parse( arr[4] );
				}
			}
		}
	}
}

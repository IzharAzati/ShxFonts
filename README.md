# ShxFonts
AutoCAD SHX fonts file

Since Autodesk did not support AutoCAD in languages other than English and written from right to left (RTL), (Arabic, Hebrew,..) many "solutions" have been created over the years to write in ACAD software. Among the solutions were created countless SHX fonts and many people that duplicate files and just rename the file.
To try to mess things up, I created this project.
The master program expects a parameter which is a directory path where SHX files are located, creates a temporary directory and tries to "translate" the SHX file to the SHP source format by running the service software that comes with ACAD, dumpshx.exe After a SHP file is created, the program tries to decode The SHP file and decomposes it into tables that are stored in an MSSQL database.
Another DLL tries to "draw" all (most) column-shaped characters to try to see similarities / differences between the various files.
Notably, not all valid SHX files and some of the created SHP files, too, are not "valid".
I'm a novice in using GIT, excuse me.

מאחר ו-AUTODESK לא תמכה ב-AUTOCAD בשפות שאינן אנגלית ואשר נכתבות מימין לשמאל, (ערבית, עברית) נוצרו במשך השנים הרבה "פתרונות" כדי לכתוב בתוכנת ACAD. בין הפתרונות נוצרו אין ספור קבצי פונטים SHX וגם הרבה אנשים שיכפלו קבצים ורק שינו את שם הקובץ.
כדי לנסות לעשות סדר בבלגן, יצרתי את הפרויקט הזה.
התוכנית הראשית מצפה לפרמטר שהוא מסלול של ספריה שבה קבצי SHX נמצאים, יוצרת ספריה זמנית ומנסה ל"תרגם" את קובץ SHX לפורמט המקור SHP על ידי הרצת תוכנת השרות שמגיעה עם ACAD, dumpshx.exe לאחר שנוצר קובץ SHP, אם נוצר, התוכנית מנסה לפענח את קובץ SHP ומפרקת אותו לטבלאות אשר נשמרות בבסיס נתונים MSSQL.
DLL נוסף מנסה ל"צייר" את כל (רוב) התווים בצורת עמודות, כדי לנסות לראות דימיון/שוני בין הקבצים השונים.
יש לציין, לא כל קבצי SHX תקינים וגם חלק מקבצי SHP שנוצרים, גם הם, לא "תקינים".
אני טירון בשימוש ב-GIT, סילחו לי.

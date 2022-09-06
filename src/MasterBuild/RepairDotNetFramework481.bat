@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0
IF NOT EXIST "%MASTERBUILDDIR%ndp481-devpack-enu.exe" bitsadmin /transfer DotNet48 /dynamic /download /priority HIGH https://go.microsoft.com/fwlink/?linkid=2203306 "%MASTERBUILDDIR%ndp481-devpack-enu.exe" 

IF ERRORLEVEL 1 GOTO END
ECHO Installing .NET Framework 4.8.1...
start /wait "%MASTERBUILDDIR%ndp481-devpack-enu.exe" ndp481-devpack-enu.exe /repair /quiet /norestart
:END


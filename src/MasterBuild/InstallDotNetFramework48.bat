@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0
IF NOT EXIST "%MASTERBUILDDIR%ndp48-devpack-enu.exe" bitsadmin /transfer DotNet48 /dynamic /download /priority HIGH https://go.microsoft.com/fwlink/?linkid=2088517 "%MASTERBUILDDIR%ndp48-devpack-enu.exe" 

IF ERRORLEVEL 1 GOTO END
ECHO Installing .NET Framework 4.8...
start /wait "%MASTERBUILDDIR%ndp48-devpack-enu.exe" ndp48-devpack-enu.exe /install /quiet /norestart
:END


@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0
IF NOT EXIST "%MASTERBUILDDIR%nuget.exe" bitsadmin /transfer Nuget /dynamic /download /priority HIGH https://dist.nuget.org/win-x86-commandline/latest/nuget.exe "%MASTERBUILDDIR%nuget.exe" 
SET EnableNuGetPackageRestore=true

ECHO Ensuring Nuget.org package source is in list of Nuget package sources...
"%MASTERBUILDDIR%nuget.exe" source Add -Name "nuget.org" -Source "https://api.nuget.org/v3/index.json" -Verbosity quiet -NonInteractive >NUL 2>&1

:END




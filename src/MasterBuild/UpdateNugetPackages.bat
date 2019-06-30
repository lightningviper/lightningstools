@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0
IF NOT EXIST "%MASTERBUILDDIR%nuget.exe" bitsadmin /transfer Nuget /dynamic /download /priority HIGH https://dist.nuget.org/win-x86-commandline/v4.1.0/nuget.exe "%MASTERBUILDDIR%nuget.exe" 
SET EnableNuGetPackageRestore=true

CALL %MASTERBUILDDIR%GetVsWhere.bat
for /f "usebackq tokens=*" %%i in (`%MASTERBUILDDIR%vswhere.exe -all -latest -products * -requires Microsoft.Component.MSBuild -property installationPath`) do (
  set InstallDir=%%i
)

SET SOLUTION=""
SET SOLUTION=%1
IF "%SOLUTION%"=="" SET SOLUTION=%MASTERBUILDDIR%BuildAll.sln

ECHO Restoring packages for solution: %SOLUTION%
"%MASTERBUILDDIR%nuget.exe" restore "%SOLUTION%" -NoCache -NonInteractive -MsbuildPath "%InstallDir%\MSBuild\Current\Bin"

ECHO Updating packages for solution: %SOLUTION%
"%MASTERBUILDDIR%nuget.exe" update "%SOLUTION%" -NonInteractive -MsbuildPath "%InstallDir%\MSBuild\Current\Bin"

:END




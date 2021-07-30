@ECHO OFF
SET MASTERBUILDDIR=%~dp0
CALL %MASTERBUILDDIR%GetVsWhere.bat

for /f "usebackq tokens=*" %%i in (`%MASTERBUILDDIR%vswhere.exe -all -latest -products * -requires Microsoft.Component.MSBuild -property installationPath`) do (
  set InstallDir=%%i
)

FOR /R "%MASTERBUILDDIR%".. %%S IN (*.sln) DO (
	"%InstallDir%\Common7\IDE\devenv.com"  /Clean "Release" "%%S"
	IF ERRORLEVEL 1 GOTO END
	"%InstallDir%\Common7\IDE\devenv.com"  /Clean "Debug" "%%S"
	IF ERRORLEVEL 1 GOTO END
)
:END
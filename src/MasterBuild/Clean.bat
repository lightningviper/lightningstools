@ECHO OFF

SET MASTERBUILDDIR=%~dp0
CALL %MASTERBUILDDIR%GetVsWhere.bat

for /f "usebackq tokens=*" %%i in (`%MASTERBUILDDIR%vswhere.exe -all -latest -products * -requires Microsoft.Component.MSBuild -property installationPath`) do (
  set InstallDir=%%i
)


SET SOLUTION=%1
IF "%SOLUTION%"=="" SET SOLUTION=%MASTERBUILDDIR%BuildAll.sln

"%InstallDir%\Common7\IDE\devenv.com"  /Clean "Debug" "%SOLUTION%"
IF ERRORLEVEL 1 GOTO END
"%InstallDir%\Common7\IDE\devenv.com"  /Clean "Release" "%SOLUTION%"
IF ERRORLEVEL 1 GOTO END

:END



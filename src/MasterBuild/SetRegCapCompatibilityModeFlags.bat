@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0
CALL %MASTERBUILDDIR%GetVsWhere.bat

for /f "usebackq tokens=*" %%i in (`%MASTERBUILDDIR%vswhere.exe -latest -products * -requires Microsoft.Component.MSBuild -property installationPath`) do (
  set InstallDir=%%i
)

CALL %MASTERBUILDDIR%Replace.bat "[VSInstallDir]" "%InstallDir:\=\\%" "%MASTERBUILDDIR%SetRegCapCompatibilityModeFlags.reg.template" > "%MASTERBUILDDIR%SetRegCapCompatibilityModeFlags.reg" 2>NUL
REG IMPORT "%MASTERBUILDDIR%SetRegCapCompatibilityModeFlags.reg" >NUL 2>NUL
DEL "%MASTERBUILDDIR%SetRegCapCompatibilityModeFlags.reg" >NUL 2>NUL

:END
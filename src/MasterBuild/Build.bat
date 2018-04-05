@ECHO OFF

:start
SET MASTERBUILDDIR=%~dp0
CALL %MASTERBUILDDIR%GetVsWhere.bat

for /f "usebackq tokens=*" %%i in (`%MASTERBUILDDIR%vswhere.exe -latest -products * -requires Microsoft.Component.MSBuild -property installationPath`) do (
  set InstallDir=%%i
)
for /f "usebackq tokens=*" %%i in (`%MASTERBUILDDIR%vswhere.exe -latest -products * -requires Microsoft.Component.MSBuild -property instanceId`) do (
  set InstanceId=%%i
)

CALL %MASTERBUILDDIR%Replace.bat "[InstanceId]" "%InstanceId%" "%MASTERBUILDDIR%EnableCommandLineInstallerBuilds.reg.template" > "%MASTERBUILDDIR%EnableCommandLineInstallerBuilds.reg" 2>NUL
REG IMPORT "%MASTERBUILDDIR%EnableCommandLineInstallerBuilds.reg" >NUL 2>NUL
DEL "%MASTERBUILDDIR%EnableCommandLineInstallerBuilds.reg" >NUL 2>NUL

SET SOLUTION=%1
IF "%SOLUTION%"=="" SET SOLUTION=%MASTERBUILDDIR%BuildAll.sln
CALL %MASTERBUILDDIR%RestorePackages.bat %SOLUTION%

"%InstallDir%\Common7\IDE\devenv.com"  /Build "Release" "%SOLUTION%"
IF ERRORLEVEL 1 GOTO END
"%InstallDir%\Common7\IDE\devenv.com"  /Build "Debug" "%SOLUTION%"
IF ERRORLEVEL 1 GOTO END

:END



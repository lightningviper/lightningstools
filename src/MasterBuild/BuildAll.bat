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

FOR /R "%MASTERBUILDDIR%".. %%S IN (*.sln) DO (
	SET LAST_BUILT_SOLUTION="%%S"
	CALL %MASTERBUILDDIR%RestorePackages.bat "%%S"
	IF ERRORLEVEL 1 GOTO END
	"%InstallDir%\Common7\IDE\devenv.com"  /Build "Release" "%%S"
	IF ERRORLEVEL 1 GOTO END
	"%InstallDir%\Common7\IDE\devenv.com"  /Build "Debug" "%%S"
	IF ERRORLEVEL 1 GOTO END
)
:END
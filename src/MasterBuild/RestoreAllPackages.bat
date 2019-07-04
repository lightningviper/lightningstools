@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0
CALL %MASTERBUILDDIR%GetVsWhere.bat

for /f "usebackq tokens=*" %%i in (`%MASTERBUILDDIR%vswhere.exe -all -latest -products * -requires Microsoft.Component.MSBuild -property installationPath`) do (
  set InstallDir=%%i
)
for /f "usebackq tokens=*" %%i in (`%MASTERBUILDDIR%vswhere.exe -all -latest -products * -requires Microsoft.Component.MSBuild -property instanceId`) do (
  set InstanceId=%%i
)


FOR /R "%MASTERBUILDDIR%".. %%S IN (*.sln) DO (
	SET LAST_BUILT_SOLUTION="%%S"
	CALL %MASTERBUILDDIR%RestorePackages.bat "%%S"
	IF ERRORLEVEL 1 GOTO END
)
:END
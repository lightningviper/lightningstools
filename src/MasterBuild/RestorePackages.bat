@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0
IF NOT EXIST "%MASTERBUILDDIR%nuget.exe" powershell Invoke-WebRequest -OutFile "%MASTERBUILDDIR%nuget.exe" "https://dist.nuget.org/win-x86-commandline/v4.1.0/nuget.exe"
  
IF ERRORLEVEL 1 GOTO END
SET EnableNuGetPackageRestore=true

CALL GetVsWhere.bat
for /f "usebackq tokens=*" %%i in (`%MASTERBUILDDIR%vswhere.exe -latest -products * -requires Microsoft.Component.MSBuild -property installationPath`) do (
  set InstallDir=%%i
)
for /f "usebackq tokens=*" %%i in (`%MASTERBUILDDIR%vswhere.exe -latest -products * -requires Microsoft.Component.MSBuild -property instanceId`) do (
  set InstanceId=%%i
)

FOR /R "%MASTERBUILDDIR%".. %%S IN (*.sln) DO (
	Pushd "%%~dpS"
	ECHO Restoring packages for solution: %%S
	"%MASTERBUILDDIR%nuget.exe" restore "%%S" -NoCache -NonInteractive -MsbuildPath "%InstallDir%\MSBuild\15.0\Bin"
	IF ERRORLEVEL 1 GOTO END
	Popd
)
:END



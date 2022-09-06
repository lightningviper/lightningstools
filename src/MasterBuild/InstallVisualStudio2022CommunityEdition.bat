@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0

IF EXIST "%MASTERBUILDDIR%VisualStudioSetup.exe" DEL "%MASTERBUILDDIR%VisualStudioSetup.exe"
IF ERRORLEVEL 1 GOTO END

bitsadmin /transfer VisualStudio2022CommunityEdition /dynamic /download /priority HIGH "https://c2rsetup.officeapps.live.com/c2r/downloadVS.aspx?sku=community&channel=Release&version=VS2022" "%MASTERBUILDDIR%VisualStudioSetup.exe" 

IF ERRORLEVEL 1 GOTO END

ECHO Installing Visual Studio 2022 Community Edition...
"%MASTERBUILDDIR%VisualStudioSetup.exe" --in "%MASTERBUILDDIR%VisualStudio2022CommunityEdition-ResponseFile.json"

CALL %MASTERBUILDDIR%GetVSWhere.bat
for /f "usebackq tokens=*" %%i in (`%MASTERBUILDDIR%vswhere.exe -all -latest -products * -requires Microsoft.Component.MSBuild -property instanceId`) do (
  set InstanceId=%%i
)

REM Delete Visual Studio reboot-required marker so we can proceed with installation of add-ins
IF EXIST "C:\ProgramData\Microsoft\VisualStudio\Packages\_Instances\.\%InstanceId%\reboot.sem" DEL "C:\ProgramData\Microsoft\VisualStudio\Packages\_Instances\%InstanceId%\reboot.sem"

:END
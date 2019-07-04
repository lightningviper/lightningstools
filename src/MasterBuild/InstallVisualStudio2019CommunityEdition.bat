@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0

IF EXIST "%MASTERBUILDDIR%vs_community.exe" DEL "%MASTERBUILDDIR%vs_community.exe"
IF ERRORLEVEL 1 GOTO END

bitsadmin /transfer VisualStudio2019CommunityEdition /dynamic /download /priority HIGH "https://download.visualstudio.microsoft.com/download/pr/08ad1141-4528-456b-8319-2d1eb127bd85/f6577aed743a858a9a523a2c5c6bc53d/vs_community.exe" "%MASTERBUILDDIR%vs_community.exe" 

IF ERRORLEVEL 1 GOTO END

ECHO Installing Visual Studio 2019 Community Edition...
"%MASTERBUILDDIR%vs_community.exe" --add Microsoft.VisualStudio.Workload.CoreEditor --add Microsoft.VisualStudio.Workload.ManagedDesktop --add Microsoft.VisualStudio.Workload.NativeDesktop --add Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -add Microsoft.Component.ClickOnce --passive --wait --norestart

CALL %MASTERBUILDDIR%GetVSWhere.bat
for /f "usebackq tokens=*" %%i in (`%MASTERBUILDDIR%vswhere.exe -all -latest -products * -requires Microsoft.Component.MSBuild -property instanceId`) do (
  set InstanceId=%%i
)

REM Delete Visual Studio reboot-required marker so we can proceed with installation of add-ins
IF EXIST "C:\ProgramData\Microsoft\VisualStudio\Packages\_Instances\.\%InstanceId%\reboot.sem" DEL "C:\ProgramData\Microsoft\VisualStudio\Packages\_Instances\%InstanceId%\reboot.sem"

:END







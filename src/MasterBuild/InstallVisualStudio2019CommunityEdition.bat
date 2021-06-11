@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0

IF EXIST "%MASTERBUILDDIR%vs_community.exe" DEL "%MASTERBUILDDIR%vs_community.exe"
IF ERRORLEVEL 1 GOTO END

bitsadmin /transfer VisualStudio2019CommunityEdition /dynamic /download /priority HIGH "https://download.visualstudio.microsoft.com/download/pr/5c44c598-f77e-4815-89ca-e7a1f87c579a/9e77a440580e677baea233033f38f9cf10a5084c915b684714bbbb19c67ee361/vs_Community.exe" "%MASTERBUILDDIR%vs_community.exe" 

IF ERRORLEVEL 1 GOTO END

ECHO Installing Visual Studio 2019 Community Edition...
"%MASTERBUILDDIR%vs_community.exe" --add Microsoft.VisualStudio.Workload.CoreEditor --add Microsoft.VisualStudio.Workload.ManagedDesktop --add Microsoft.VisualStudio.Workload.NativeDesktop --add Microsoft.VisualStudio.Component.VC.Tools.x86.x64 --add Microsoft.Component.ClickOnce --passive --wait --norestart

CALL %MASTERBUILDDIR%GetVSWhere.bat
for /f "usebackq tokens=*" %%i in (`%MASTERBUILDDIR%vswhere.exe -all -latest -products * -requires Microsoft.Component.MSBuild -property instanceId`) do (
  set InstanceId=%%i
)

REM Delete Visual Studio reboot-required marker so we can proceed with installation of add-ins
IF EXIST "C:\ProgramData\Microsoft\VisualStudio\Packages\_Instances\.\%InstanceId%\reboot.sem" DEL "C:\ProgramData\Microsoft\VisualStudio\Packages\_Instances\%InstanceId%\reboot.sem"

:END
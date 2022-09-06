@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0

IF EXIST "%MASTERBUILDDIR%VisualStudioSetup.exe" DEL "%MASTERBUILDDIR%VisualStudioSetup.exe"
IF ERRORLEVEL 1 GOTO END

bitsadmin /transfer VisualStudio2022CommunityEdition /dynamic /download /priority HIGH "https://c2rsetup.officeapps.live.com/c2r/downloadVS.aspx?sku=community&channel=Release&version=VS2022" "%MASTERBUILDDIR%VisualStudioSetup.exe" 

IF ERRORLEVEL 1 GOTO END

ECHO Installing Visual Studio 2022Community Edition...
"%MASTERBUILDDIR%VisualStudioSetup.exe" --add Microsoft.VisualStudio.Workload.CoreEditor --add Microsoft.VisualStudio.Workload.ManagedDesktop --add Microsoft.VisualStudio.Workload.NativeDesktop;includeRecommended --add Microsoft.VisualStudio.Workload.NativeGame;includeRecommended --add Microsoft.VisualStudio.Component.VC.Tools.x86.x64 --add Microsoft.Component.ClickOnce --passive --wait --norestart

ECHO Installing Visual Studio 2022Community Edition additional components...
"%MASTERBUILDDIR%VisualStudioSetup.exe" --add Microsoft.Net.Component.4.8.TargetingPack --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe" --add Microsoft.Net.ComponentGroup.4.8.DeveloperTools --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe" --add Microsoft.VisualStudio.Component.Debugger.JustInTime --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe" --add Microsoft.VisualStudio.Component.DiagnosticTools --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe" --add Microsoft.VisualStudio.Component.IntelliCode --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe" --add Microsoft.VisualStudio.Component.NuGet --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe" --add Microsoft.Net.Component.4.8.1.SDK --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe" --add Microsoft.Net.Component.4.8.1.TargetingPack --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe" --add Microsoft.Net.ComponentGroup.4.8.1.DeveloperTools --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe" --add Microsoft.VisualStudio.Component.Wcf.Tooling --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe" --add Microsoft.VisualStudio.ComponentGroup.MSIX.Packaging --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe" --add Microsoft.VisualStudio.ComponentGroup.WindowsAppSDK.Cs --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe" --add Microsoft.VisualStudio.Component.NuGet.BuildTools --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe"Microsoft.VisualStudio.Component.VC.14.29.16.11.MFC --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe"Microsoft.VisualStudio.Component.VC.14.29.16.11.x86.x64 --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe"Microsoft.VisualStudio.ComponentGroup.VC.Tools.142.x86.x64 --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe"Microsoft.VisualStudio.ComponentGroup.WindowsAppSDK.Cpp --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe"Microsoft.VisualStudio.ComponentGroup.NativeDesktop.Llvm.Clang --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe"Microsoft.Component.VC.Runtime.UCRTSDK --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe"Microsoft.VisualStudio.Component.VC.140 --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe"Microsoft.VisualStudio.Component.VC.ATLMFC --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe"Microsoft.VisualStudio.Component.VC.CLI.Support --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe"Microsoft.VisualStudio.Component.VC.Llvm.Clang --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe"Microsoft.VisualStudio.Component.VC.Llvm.ClangToolset --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe"Microsoft.VisualStudio.Component.VC.Modules.x86.x64 --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe"Microsoft.VisualStudio.Component.VC.v141.x86.x64 --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe"Microsoft.VisualStudio.ComponentGroup.VC.Tools.142.x86.x64 --passive --wait --norestart
"%MASTERBUILDDIR%VisualStudioSetup.exe"Microsoft.VisualStudio.Component.Windows10SDK --passive --wait --norestart

CALL %MASTERBUILDDIR%GetVSWhere.bat
for /f "usebackq tokens=*" %%i in (`%MASTERBUILDDIR%vswhere.exe -all -latest -products * -requires Microsoft.Component.MSBuild -property instanceId`) do (
  set InstanceId=%%i
)

REM Delete Visual Studio reboot-required marker so we can proceed with installation of add-ins
IF EXIST "C:\ProgramData\Microsoft\VisualStudio\Packages\_Instances\.\%InstanceId%\reboot.sem" DEL "C:\ProgramData\Microsoft\VisualStudio\Packages\_Instances\%InstanceId%\reboot.sem"

:END
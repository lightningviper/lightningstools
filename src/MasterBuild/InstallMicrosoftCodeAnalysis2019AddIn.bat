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

IF NOT EXIST "%MASTERBUILDDIR%Microsoft.CodeAnalysis.FxCopAnalyzers.Setup.vsix" bitsadmin /transfer VisualStudio2017InstallerProjectsAddIn /dynamic /download /priority HIGH "https://visualstudioplatformteam.gallerycdn.vsassets.io/extensions/visualstudioplatformteam/microsoftcodeanalysis2019/3.0.0.1925803/1557326995287/Microsoft.CodeAnalysis.FxCopAnalyzers.Setup.vsix" "%MASTERBUILDDIR%Microsoft.CodeAnalysis.FxCopAnalyzers.Setup.vsix" 


IF ERRORLEVEL 1 GOTO END
ECHO Installing Microsoft Code Analysis 2019 add-in...
"%InstallDir%\Common7\IDE\VsixInstaller.exe" /a /q "%MASTERBUILDDIR%Microsoft.CodeAnalysis.FxCopAnalyzers.Setup.vsix"
IF ERRORLEVEL 1 GOTO END

"%InstallDir%\Common7\IDE\CommonExtensions\Microsoft\VSI\bin\regcap.exe"
IF NOT EXIST "%WINDIR%\Microsoft.NET\Framework\URTInstallPath_GAC\" MD "%WINDIR%\Microsoft.NET\Framework\URTInstallPath_GAC\" 

:END


@ECHO OFF
SET MASTERBUILDDIR=%~dp0

REM Download and Install TortoiseGit
CALL %MASTERBUILDDIR%InstallTortoiseGit.bat

REM Clone Git repository
CALL %MASTERBUILDDIR%Clone.bat

REM Install Visual Studio 2017 Community Edition
CALL %MASTERBUILDDIR%InstallVisualStudio2017CommunityEdition.bat

REM Install .NET Framework 3.5 SP1
CALL %MASTERBUILDDIR%InstallDotNetFramework35SP1.bat

REM Install DirectX SDK
CALL %MASTERBUILDDIR%InstallDirectXSDK.bat

REM Install Bootstrappers
CALL %MASTERBUILDDIR%InstallBootstrappers.bat

REM Install Visual Studio Installer Projects Add-In for Visual Studio
CALL %MASTERBUILDDIR%InstallVisualStudio2017InstallerProjectAddIn.bat
CALL %MASTERBUILDDIR%SetRegCapCompatibilityModeFlags.bat

REM Restore NuGet Packages
CALL %MASTERBUILDDIR%RestorePackages.bat


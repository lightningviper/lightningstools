@ECHO OFF
SET MASTERBUILDDIR=%~dp0

REM Download and Install Git for Windows
CALL %MASTERBUILDDIR%InstallGitForWindows.bat

REM Clone Git repository
CALL %MASTERBUILDDIR%Clone.bat

REM Install Visual Studio 2019 Community Edition
CALL %MASTERBUILDDIR%InstallVisualStudio2019CommunityEdition.bat

REM Install Bootstrappers
CALL %MASTERBUILDDIR%InstallBootstrappers.bat

REM Install Visual Studio Installer Projects Add-In for Visual Studio
CALL %MASTERBUILDDIR%SetRegCapCompatibilityModeFlags.bat
CALL %MASTERBUILDDIR%InstallVisualStudioInstallerProjectAddIn.bat
CALL %MASTERBUILDDIR%SetRegCapCompatibilityModeFlags.bat

REM Install Microsoft Code Analysis 2019 add-in
CALL %MASTERBUILDDIR%SetRegCapCompatibilityModeFlags.bat
CALL %MASTERBUILDDIR%InstallMicrosoftCodeAnalysis2019AddIn.bat
CALL %MASTERBUILDDIR%SetRegCapCompatibilityModeFlags.bat

REM Install TortoiseGit
CALL %MASTERBUILDDIR%InstallTortoiseGit.bat

REM Install Windows 10 SDK
CALL %MASTERBUILDDIR%InstallWindowsSDK.bat

REM Restore NuGet packages
CALL %MASTERBUILDDIR%RestoreAllPackages.bat

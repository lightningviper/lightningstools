@ECHO OFF

IF EXIST "%TEMP%\LightningsToolsInstall" RD /S /Q "%TEMP%\LightningsToolsInstall"
MKDIR "%TEMP%\LightningsToolsInstall"

IF NOT EXIST "%TEMP%\LightningsToolsInstall\InstallTortoiseGit.bat" powershell Invoke-WebRequest -OutFile "%TEMP%\LightningsToolsInstall\InstallTortoiseGit.bat" "https://raw.githubusercontent.com/lightningviper/lightningstools/master/src/MasterBuild/InstallTortoiseGit.bat"
IF ERRORLEVEL 1 GOTO END

CD "%TEMP%\LightningsToolsInstall"
CALL InstallTortoiseGit.bat

CD "%CD:~0,2%\lightningstools\src\MasterBuild"
CALL PrepareBuildMachine.bat

:END

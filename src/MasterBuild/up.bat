@ECHO OFF

IF EXIST "%TEMP%\LightningsToolsInstall" RD /S /Q "%TEMP%\LightningsToolsInstall"
MKDIR "%TEMP%\LightningsToolsInstall"

IF NOT EXIST "%TEMP%\LightningsToolsInstall\InstallGitForWindows.bat" powershell Invoke-WebRequest -OutFile "%TEMP%\LightningsToolsInstall\InstallGitForWindows.bat" "https://raw.githubusercontent.com/lightningviper/lightningstools/master/src/MasterBuild/InstallGitForWindows.bat"
IF ERRORLEVEL 1 GOTO END

CD "%TEMP%\LightningsToolsInstall"
CALL InstallGitForWindows.bat

IF NOT EXIST "%TEMP%\LightningsToolsInstall\Clone.bat" powershell Invoke-WebRequest -OutFile "%TEMP%\LightningsToolsInstall\Clone.bat" "https://raw.githubusercontent.com/lightningviper/lightningstools/master/src/MasterBuild/Clone.bat"
IF ERRORLEVEL 1 GOTO END

CD "%TEMP%\LightningsToolsInstall"
CALL Clone.bat

CD "%CD:~0,2%\lightningstools\src\MasterBuild"
CALL PrepareBuildMachine.bat

:END

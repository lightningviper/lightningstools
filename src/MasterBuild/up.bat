@ECHO OFF

IF EXIST "%TEMP%\LightningsToolsInstall" RD /S /Q "%TEMP%\LightningsToolsInstall"
MKDIR "%TEMP%\LightningsToolsInstall"

IF NOT EXIST "%TEMP%\LightningsToolsInstall\InstallGitForWindows.bat" bitsadmin /transfer InstallGitForWindows.bat /dynamic /download /priority HIGH https://raw.githubusercontent.com/lightningviper/lightningstools/master/src/MasterBuild/InstallGitForWindows.bat "%TEMP%\LightningsToolsInstall\InstallGitForWindows.bat" 
IF ERRORLEVEL 1 GOTO END

CD "%TEMP%\LightningsToolsInstall"
CALL InstallGitForWindows.bat

IF NOT EXIST "%TEMP%\LightningsToolsInstall\Clone.bat" bitsadmin /transfer Clone.bat /dynamic /download /priority HIGH https://raw.githubusercontent.com/lightningviper/lightningstools/master/src/MasterBuild/Clone.bat "%TEMP%\LightningsToolsInstall\Clone.bat"
IF ERRORLEVEL 1 GOTO END

CD "%TEMP%\LightningsToolsInstall"
CALL Clone.bat

CD "%CD:~0,2%\lightningstools\src\MasterBuild"
CALL PrepareBuildMachine.bat

:END

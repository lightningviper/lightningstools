@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0

IF NOT EXIST "%MASTERBUILDDIR%TeensyduinoInstall.exe" bitsadmin /transfer TeensyDuino /dynamic /download /priority HIGH https://www.pjrc.com/teensy/td_146/TeensyduinoInstall.exe "%MASTERBUILDDIR%TeensyduinoInstall.exe"
IF ERRORLEVEL 1 GOTO END

ECHO Running TeensyduinoInstall.exe...
start /wait "" "%MASTERBUILDDIR%TeensyduinoInstall.exe" 

:END







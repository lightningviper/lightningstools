@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0

IF NOT EXIST "%MASTERBUILDDIR%arduino-1.8.9-windows.exe" bitsadmin /transfer arduino-1.8.9-windows.exe /dynamic /download /priority HIGH https://downloads.arduino.cc/arduino-1.8.13-windows.exe "%MASTERBUILDDIR%arduino-1.8.13-windows.exe"
IF ERRORLEVEL 1 GOTO END

ECHO Running arduino-1.8.13-windows.exe...
start /wait "" "%MASTERBUILDDIR%arduino-1.8.13-windows.exe" 

:END










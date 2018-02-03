@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0
IF NOT EXIST "%MASTERBUILDDIR%vswhere.exe" powershell Invoke-WebRequest -OutFile "%MASTERBUILDDIR%vswhere.exe" "https://github.com/Microsoft/vswhere/releases/download/1.0.62/vswhere.exe" 
IF ERRORLEVEL 1 GOTO END
:END

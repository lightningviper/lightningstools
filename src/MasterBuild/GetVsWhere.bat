@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0
IF NOT EXIST "%MASTERBUILDDIR%vswhere.exe" bitsadmin /transfer vswhere /dynamic /download /priority HIGH https://github.com/Microsoft/vswhere/releases/download/1.0.62/vswhere.exe "%MASTERBUILDDIR%vswhere.exe" 

IF ERRORLEVEL 1 GOTO END
:END

@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0
IF NOT EXIST "%MASTERBUILDDIR%vswhere.exe" bitsadmin /transfer vswhere /dynamic /download /priority HIGH https://github.com/microsoft/vswhere/releases/download/2.8.4/vswhere.exe "%MASTERBUILDDIR%vswhere.exe" 

IF ERRORLEVEL 1 GOTO END
:END

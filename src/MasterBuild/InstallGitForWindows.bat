@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0

SET ProgFiles86Root=%ProgramFiles(x86)%
IF NOT "%ProgFiles86Root%"=="" SET ProcArch=x64
IF "%ProgFiles86Root%"=="" SET ProcArch=win32

IF "%ProcArch%"=="x64" GOTO GET64
IF "%ProcArch%"=="win32" GOTO GET32
GOTO END

:GET64
IF NOT EXIST "%MASTERBUILDDIR%Git-2.16.2-64-bit.exe" bitsadmin /transfer GitForWindows /dynamic /download /priority HIGH "https://github.com/git-for-windows/git/releases/download/v2.16.2.windows.1/Git-2.16.2-64-bit.exe" "%MASTERBUILDDIR%Git-2.16.2-64-bit.exe"
IF ERRORLEVEL 1 GOTO END
ECHO Installing Git-2.16.2-64-bit.exe...
"%MASTERBUILDDIR%Git-2.16.2-64-bit.exe" /VERYSILENT /NORESTART /PathOption=CmdTools ADDLOCAL=ALL 
GOTO END

:GET32
IF NOT EXIST "%MASTERBUILDDIR%Git-2.16.2-32-bit.exe" bitsadmin /transfer GitForWindows /dynamic /download /priority HIGH "https://github.com/git-for-windows/git/releases/download/v2.16.2.windows.1/Git-2.16.2-32-bit.exe" "%MASTERBUILDDIR%Git-2.16.2-32-bit.exe"
IF ERRORLEVEL 1 GOTO END
ECHO Installing Git-2.16.2-32-bit.exe...
"%MASTERBUILDDIR%Git-2.16.2-32-bit.exe" /VERYSILENT /NORESTART /PathOption=CmdTools ADDLOCAL=ALL 
GOTO END

:END

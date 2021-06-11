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
IF NOT EXIST "%MASTERBUILDDIR%TortoiseGit-2.12.0.0-64bit.msi" bitsadmin /transfer TortoiseGit-2.12.0.0-64bit.msi /dynamic /download /priority HIGH "https://download.tortoisegit.org/tgit/2.12.0.0/TortoiseGit-2.12.0.0-64bit.msi" "%MASTERBUILDDIR%TortoiseGit-2.12.0.0-64bit.msi"
IF ERRORLEVEL 1 GOTO END
ECHO Installing TortoiseGit-2.12.0.0-64bit.msi...
"%MASTERBUILDDIR%TortoiseGit-2.12.0.0-64bit.msi" /quiet /norestart
GOTO END

:GET32
IF NOT EXIST "%MASTERBUILDDIR%TortoiseGit-2.12.0.0-32bit.msi" bitsadmin /transfer TortoiseGit-2.12.0.0-32bit.msi /dynamic /download /priority HIGH "https://download.tortoisegit.org/tgit/2.12.0.0/TortoiseGit-2.12.0.0-32bit.msi" "%MASTERBUILDDIR%TortoiseGit-2.12.0.0-32bit.msi"
IF ERRORLEVEL 1 GOTO END
ECHO Installing TortoiseGit-2.12.0.0-32bit.msi...
"%MASTERBUILDDIR%TortoiseGit-2.12.0.0-32bit.msi" /quiet /norestart
GOTO END

:END

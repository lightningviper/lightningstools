@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0
IF NOT EXIST "%MASTERBUILDDIR%vs_community.exe" bitsadmin /transfer VisualStudio2017CommunityEdition /dynamic /download /priority HIGH https://aka.ms/vs/15/release/vs_community.exe "%MASTERBUILDDIR%vs_community.exe" 

IF ERRORLEVEL 1 GOTO END
ECHO Installing Visual Studio 2017 Community Edition...
"%MASTERBUILDDIR%vs_community.exe" --add Microsoft.VisualStudio.Workload.CoreEditor --add Microsoft.VisualStudio.Workload.ManagedDesktop --add Microsoft.VisualStudio.Workload.NativeDesktop --passive --wait --norestart

:END







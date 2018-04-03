@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0
Dism.exe /online /add-capability /Quiet /NoRestart /capabilityname:NetFx3~~~~ 

IF NOT EXIST "%MASTERBUILDDIR%dotnetfx35setup.exe" bitsadmin /transfer DotNetFx35Setup /dynamic /download /priority HIGH https://download.microsoft.com/download/0/6/1/061F001C-8752-4600-A198-53214C69B51F/dotnetfx35setup.exe "%MASTERBUILDDIR%dotnetfx35setup.exe" 
IF ERRORLEVEL 1 GOTO END

ECHO Installing .NET Framework 3.5 SP1...
"%MASTERBUILDDIR%dotnetfx35setup.exe" /q /norollback /norestart
:END







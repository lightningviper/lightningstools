@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0
IF NOT EXIST "%MASTERBUILDDIR%sdksetup.exe" bitsadmin /transfer Win10SDK /dynamic /download /priority HIGH http://download.microsoft.com/download/2/1/2/2122BA8F-7EA6-4784-9195-A8CFB7E7388E/StandaloneSDK/sdksetup.exe "%MASTERBUILDDIR%sdksetup.exe" 

IF ERRORLEVEL 1 GOTO END
ECHO Installing Windows SDK version 10.0.10586.0 ...
start /wait "%MASTERBUILDDIR%sdksetup.exe" sdksetup.exe /features + /q /norestart /ceip off
:END


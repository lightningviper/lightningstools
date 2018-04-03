@ECHO OFF
:start
SET MASTERBUILDDIR=%~dp0
IF NOT EXIST "%MASTERBUILDDIR%DXSDK_Jun10.exe" bitsadmin /transfer DXSDK_Jun10 /dynamic /download /priority HIGH https://download.microsoft.com/download/A/E/7/AE743F1F-632B-4809-87A9-AA1BB3458E31/DXSDK_Jun10.exe "%MASTERBUILDDIR%DXSDK_Jun10.exe" 

IF ERRORLEVEL 1 GOTO END

"%MASTERBUILDDIR%DXSDK_Jun10.exe" /F /S /O /U

:END







@ECHO OFF
SET ProgFiles86Root=%ProgramFiles(x86)%
IF NOT "%ProgFiles86Root%"=="" GOTO start
SET ProgFiles86Root=%ProgramFiles%

:start
SET MASTERBUILDDIR=%~dp0
xcopy /S /E /N "%MASTERBUILDDIR%\Bootstrappers" "%ProgFiles86Root%\Microsoft SDKs\ClickOnce Bootstrapper\Packages\"  
:END



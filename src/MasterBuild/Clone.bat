@ECHO OFF
IF EXIST "%CD:~0,2%\lightningstools" GOTO END
ECHO Cloning lightningviper/lightningstools repository...
"%ProgramFiles%\Git\cmd\git.exe" clone "https://github.com/lightningviper/lightningstools" "%CD:~0,2%\lightningstools"
:END

@ECHO OFF
call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\Tools\VsDevCmd.bat">nul
@ECHO OFF

SET outputDirectory=%~dp0\Publish

del %outputDirectory%\RePKG.zip
msbuild /p:OutputPath="%outputDirectory%" /p:Configuration=Release RePKG

move %outputDirectory%\RePKG.exe %outputDirectory%\input.exe
ilrepack /out:"%outputDirectory%\RePKG.exe" /wildcards /parallel "%outputDirectory%\input.exe" "%outputDirectory%\*.dll"
del %outputDirectory%\input.exe
del %outputDirectory%\*.dll
del %outputDirectory%\*.pdb
del %outputDirectory%\*.config
del %outputDirectory%\*.json
cd %outputDirectory%
7z a -tzip RePKG.zip *
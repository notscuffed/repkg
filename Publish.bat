@ECHO OFF
call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\Tools\VsDevCmd.bat">nul
@ECHO OFF

SET outputDirectory=%~dp0\Publish

del %outputDirectory%\RePKG.zip
msbuild /p:OutputPath="%outputDirectory%" /p:Configuration=Release RePKG

ilrepack /out:"%outputDirectory%\output.exe" /wildcards /parallel "%outputDirectory%\RePKG.exe" "%outputDirectory%\*.dll"
del %outputDirectory%\RePKG.exe
move %outputDirectory%\output.exe %outputDirectory%\RePKG.exe
del %outputDirectory%\*.dll
del %outputDirectory%\*.pdb
del %outputDirectory%\*.config
cd %outputDirectory%
7z a -tzip RePKG.zip *
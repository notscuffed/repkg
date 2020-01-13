cd RePKG.Native
dotnet publish /p:NativeLib=Shared -r win-x64 -c release

SET NATIVE_DIR=./bin/Release/netstandard2.0/win-x64/native
SET BINDINGS_DIR=../Bindings/Python

copy "%NATIVE_DIR%\RePKG.Native.dll" "%BINDINGS_DIR%"
copy "%NATIVE_DIR%\RePKG.Native.pdb" "%BINDINGS_DIR%"
copy "%NATIVE_DIR%\RePKG.Native.exp" "%BINDINGS_DIR%"
copy "%NATIVE_DIR%\RePKG.Native.lib" "%BINDINGS_DIR%"

cd "%BINDINGS_DIR%"
repkg_ffi_generate.py

pause
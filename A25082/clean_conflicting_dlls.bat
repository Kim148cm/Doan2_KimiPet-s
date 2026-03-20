@echo off
echo Cleaning conflicting DLL files...
if exist "bin\BanHangThoiTrang.dll" del "bin\BanHangThoiTrang.dll"
if exist "bin\BanHangThoiTrang.pdb" del "bin\BanHangThoiTrang.pdb"
if exist "bin\BanHangThoiTrang.dll.config" del "bin\BanHangThoiTrang.dll.config"
echo Cleanup completed.


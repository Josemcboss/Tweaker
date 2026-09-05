@echo off
title Ghost Memory Cleaner
cd /d "%~dp0\GhostMemoryCleaner"

:: Comprobar permisos de administrador y auto-elevar si es necesario
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo Solicitando permisos de administrador para purga de memoria...
    powershell -Command "Start-Process '%~0' -Verb RunAs"
    exit /b
)

if exist "bin\Debug\net10.0-windows\GhostMemoryCleaner.exe" (
    start "" "bin\Debug\net10.0-windows\GhostMemoryCleaner.exe"
) else (
    echo Compilando Ghost Memory Cleaner...
    dotnet build GhostMemoryCleaner.csproj -c Debug
    start "" "bin\Debug\net10.0-windows\GhostMemoryCleaner.exe"
)
exit /b

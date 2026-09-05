@echo off
setlocal EnableDelayedExpansion
title Compilar Ghost Memory Cleaner Portable

echo ========================================================
echo     COMPILANDO GHOST MEMORY CLEANER (PORTABLE .EXE)
echo ========================================================
echo.

cd /d "%~dp0"

echo Publicando ejecutable portable standalone...
dotnet publish "GhostMemoryCleaner\GhostMemoryCleaner.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o "MemoryCleaner_Portable"

if %ERRORLEVEL% equ 0 (
    echo.
    echo ========================================================
    echo   COMPILACION EXITOSA!
    echo   Ejecutable disponible en: MemoryCleaner_Portable\GhostMemoryCleaner.exe
    echo ========================================================
) else (
    echo.
    echo [ERROR] La compilacion fallo.
)

pause

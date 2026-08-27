@echo off
title Tweaker - Ejecutar como Administrador
cd /d "%~dp0"

echo ================================================================
echo    INICIANDO TWEAKER (MODO ADMINISTRADOR)
echo ================================================================
echo.

:: Verificar si el .exe existe
if not exist "Tweaker\bin\Debug\net10.0-windows\Tweaker.exe" (
    echo Compilando aplicacion...
    dotnet build Tweaker\Tweaker.csproj --configuration Debug
)

echo Lanzando Tweaker...
start "" "Tweaker\bin\Debug\net10.0-windows\Tweaker.exe"
exit

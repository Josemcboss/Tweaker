@echo off
title Tweaker - Ejecutar como Administrador
cd /d "%~dp0"

:: Verificar privilegios de administrador y auto-elevar si es necesario
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo Elevando a Administrador...
    powershell -Command "Start-Process cmd -ArgumentList '/c \"\"%~f0\"\"' -Verb RunAs"
    exit /b
)

echo ================================================================
echo    INICIANDO TWEAKER (MODO ADMINISTRADOR)
echo ================================================================
echo.

:: Cerrar cualquier instancia previa de Tweaker para desbloquear el archivo .exe
echo Cerrando instancias previas de Tweaker...
taskkill /F /IM Tweaker.exe >nul 2>&1
timeout /t 1 /nobreak >nul

echo Compilando aplicacion para asegurar ultimos cambios...
dotnet build Tweaker\Tweaker.csproj --configuration Debug
if %errorLevel% neq 0 (
    echo.
    echo ERROR al compilar. Presiona cualquier tecla para salir...
    pause >nul
    exit /b
)

echo.
echo Lanzando Tweaker...
start "" "Tweaker\bin\Debug\net10.0-windows\Tweaker.exe"
exit

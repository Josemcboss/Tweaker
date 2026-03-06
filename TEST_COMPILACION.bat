@echo off
:: Script de prueba rápida
title Ghost Optimizer - Test de Compilacion

echo.
echo ========================================
echo  GHOST OPTIMIZER - TEST RAPIDO
echo ========================================
echo.
echo Este script verifica que todo este
echo listo para compilar.
echo.

:: Verificar .NET SDK
echo [1/4] Verificando .NET SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
    color 0C
    echo [ERROR] .NET SDK no encontrado
    echo.
    echo Descargalo desde:
    echo https://dotnet.microsoft.com/download/dotnet/10.0
    echo.
    pause
    exit /b 1
) else (
    for /f "tokens=*" %%v in ('dotnet --version') do (
        echo [OK] .NET SDK Version: %%v
    )
)

:: Verificar proyecto
echo [2/4] Verificando proyecto...
if not exist "Tweaker\Tweaker.csproj" (
    color 0C
    echo [ERROR] No se encuentra Tweaker\Tweaker.csproj
    echo.
    pause
    exit /b 1
) else (
    echo [OK] Proyecto encontrado
)

:: Verificar scripts de compilacion
echo [3/4] Verificando scripts...
if not exist "BUILD_AND_ZIP.bat" (
    color 0E
    echo [ADVERTENCIA] BUILD_AND_ZIP.bat no encontrado
) else (
    echo [OK] BUILD_AND_ZIP.bat encontrado
)

if not exist "BUILD_RELEASE.bat" (
    color 0E
    echo [ADVERTENCIA] BUILD_RELEASE.bat no encontrado
) else (
    echo [OK] BUILD_RELEASE.bat encontrado
)

:: Test build rapido
echo [4/4] Probando compilacion rapida...
echo.
echo (Esto puede tardar unos segundos...)
dotnet build Tweaker\Tweaker.csproj -c Release -v quiet

if errorlevel 1 (
    color 0C
    echo.
    echo [ERROR] La compilacion de prueba fallo
    echo Revisa que no haya errores en el codigo.
    echo.
    pause
    exit /b 1
) else (
    echo.
    color 0A
    echo [OK] Compilacion de prueba exitosa!
)

echo.
echo ========================================
echo  TODO LISTO PARA COMPILAR
echo ========================================
echo.
color 07
echo Puedes ejecutar:
echo   ^> BUILD_AND_ZIP.bat   (Recomendado)
echo   ^> BUILD_RELEASE.bat   (Solo compilar)
echo.
echo ========================================
echo.
pause

@echo off
:: ========================================
:: SCRIPT DE PUBLICACIÓN - GHOST OPTIMIZER
:: ========================================

echo ========================================
echo PUBLICANDO GHOST OPTIMIZER v2.0.0
echo ========================================
echo.

:: Limpiar publicaciones anteriores
if exist "publish" (
    echo Limpiando publicaciones anteriores...
    rmdir /s /q publish
)

echo.
echo Compilando y publicando...
echo.

:: Publicar versión Release optimizada
dotnet publish Tweaker\Tweaker.csproj ^
    -c Release ^
    -r win-x64 ^
    --self-contained true ^
    -p:PublishSingleFile=true ^
    -p:PublishTrimmed=false ^
    -p:EnableCompressionInSingleFile=true ^
    -p:DebugType=none ^
    -p:DebugSymbols=false ^
    -p:PublishReadyToRun=true ^
    -o publish\GhostOptimizer

if errorlevel 1 (
    echo.
    echo ========================================
    echo ERROR EN LA PUBLICACIÓN
    echo ========================================
    pause
    exit /b 1
)

echo.
echo ========================================
echo PUBLICACIÓN COMPLETADA EXITOSAMENTE
echo ========================================
echo.
echo Archivos generados en: publish\GhostOptimizer\
echo.
echo Ejecutable: publish\GhostOptimizer\Tweaker.exe
echo.
echo ========================================
echo CREANDO PAQUETE DE DISTRIBUCIÓN
echo ========================================
echo.

:: Copiar archivos adicionales
if exist "Release\DIAGNOSTICO_SERVICIOS.bat" (
    copy "Release\DIAGNOSTICO_SERVICIOS.bat" "publish\GhostOptimizer\"
    echo Copiado: DIAGNOSTICO_SERVICIOS.bat
)

if exist "Release\*.md" (
    xcopy "Release\*.md" "publish\GhostOptimizer\Docs\" /Y /I
    echo Copiada: Documentación
)

:: Crear archivo README para la distribución
echo Creando README...
(
echo # Ghost Optimizer v2.0.0
echo.
echo ## Instalación
echo 1. Extrae todos los archivos en una carpeta
echo 2. Ejecuta Tweaker.exe como ADMINISTRADOR
echo.
echo ## Requisitos
echo - Windows 10/11 x64
echo - Permisos de Administrador
echo.
echo ## Características
echo - Optimización de Servicios con Validación de Seguridad
echo - Tweaks Avanzados de Sistema
echo - Optimización de Latencia
echo - Sistema de Backup y Restauración
echo - Indicadores de Seguridad en Tiempo Real
echo.
echo ## Documentación
echo Revisa la carpeta Docs\ para documentación adicional
echo.
echo ## Soporte
echo https://github.com/Josemcboss/Tweaker
) > "publish\GhostOptimizer\README.txt"

echo.
echo ========================================
echo PAQUETE LISTO PARA DISTRIBUCIÓN
echo ========================================
echo.
echo Contenido de la distribución:
dir /b "publish\GhostOptimizer"
echo.
echo Tamaño del ejecutable:
dir "publish\GhostOptimizer\Tweaker.exe" | findstr "Tweaker.exe"
echo.

:: Opcional: Crear archivo ZIP
where 7z >nul 2>nul
if %errorlevel% == 0 (
    echo ========================================
    echo CREANDO ARCHIVO ZIP
    echo ========================================
    echo.
    7z a -tzip "publish\GhostOptimizer_v2.0.0_win-x64.zip" ".\publish\GhostOptimizer\*"
    echo.
    echo Archivo creado: publish\GhostOptimizer_v2.0.0_win-x64.zip
) else (
    echo.
    echo NOTA: 7-Zip no está instalado. Comprime manualmente la carpeta
    echo       publish\GhostOptimizer\ para distribuir.
)

echo.
echo ========================================
echo PROCESO COMPLETADO
echo ========================================
pause

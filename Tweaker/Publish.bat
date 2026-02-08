@echo off
echo.
echo ============================================
echo  GHOST OPTIMIZER - PUBLICACION RAPIDA
echo ============================================
echo.

echo Publicando aplicacion...
echo.

dotnet publish Tweaker\Tweaker.csproj --configuration Release --runtime win-x64 --self-contained true --output Release /p:PublishSingleFile=true /p:PublishTrimmed=false /p:EnableCompressionInSingleFile=true /p:PublishReadyToRun=true

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ? PUBLICACION EXITOSA
    echo.
    echo Archivo generado: Release\GhostOptimizer.exe
    echo.
    echo Abriendo directorio...
    start explorer.exe Release
    echo.
    echo ¡Listo para compartir!
    echo Solo necesitas el archivo GhostOptimizer.exe
    echo.
) else (
    echo.
    echo X ERROR EN PUBLICACION
    echo Verifica que tengas .NET 10 SDK instalado
    echo.
)

pause
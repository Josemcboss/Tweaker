@echo off
echo ============================================
echo   GHOST OPTIMIZER - RELEASE HELPER
echo ============================================
echo.

REM Obtener ruta del instalador
set /p INSTALLER_PATH="Ruta del instalador (ej: publish\Setup.exe): "

if not exist "%INSTALLER_PATH%" (
    echo ERROR: Archivo no encontrado: %INSTALLER_PATH%
    pause
    exit /b 1
)

echo.
echo Calculando SHA256...
powershell -Command "& {$hash = Get-FileHash '%INSTALLER_PATH%' -Algorithm SHA256; Write-Host ''; Write-Host 'SHA256 Hash:' -ForegroundColor Green; Write-Host $hash.Hash -ForegroundColor Yellow; Write-Host ''; Write-Host 'Tamano de archivo:' -ForegroundColor Green; $size = (Get-Item '%INSTALLER_PATH%').Length; Write-Host ('{0:N0} bytes ({1:N2} MB)' -f $size, ($size/1MB)) -ForegroundColor Yellow; Write-Host ''; Write-Host 'Copia este hash a update.json' -ForegroundColor Cyan}"

echo.
echo ============================================
echo SIGUIENTE PASO:
echo 1. Copia el hash mostrado arriba
echo 2. Actualiza Release/update.json
echo 3. Crea Release en GitHub
echo 4. Sube el instalador al Release
echo 5. Commit y push update.json
echo ============================================
echo.

pause

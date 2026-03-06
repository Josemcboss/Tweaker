@echo off
:: ====================================================
:: GHOST OPTIMIZER - COMPILADOR + EMPAQUETADOR
:: Compila y crea ZIP listo para compartir
:: ====================================================

title Ghost Optimizer - Build and Package

color 0B
echo.
echo ====================================================
echo    GHOST OPTIMIZER - BUILD ^& ZIP v2.5.0
echo ====================================================
echo.
echo Este script hara TODO automaticamente:
echo  - Compilar ejecutable optimizado
echo  - Crear estructura de carpetas
echo  - Copiar documentacion
echo  - Generar README
echo  - Crear archivo ZIP
echo.
echo Presiona cualquier tecla para comenzar...
pause >nul
cls

echo.
echo ====================================================
echo [PASO 1/6] VERIFICANDO ENTORNO
echo ====================================================
echo.

:: Verificar que estamos en el directorio correcto
if not exist "Tweaker\Tweaker.csproj" (
    color 0C
    echo ERROR: No se encuentra Tweaker\Tweaker.csproj
    echo Asegurate de ejecutar este script desde la raiz del proyecto.
    echo.
    pause
    exit /b 1
)

:: Verificar .NET SDK
dotnet --version >nul 2>&1
if errorlevel 1 (
    color 0C
    echo ERROR: .NET SDK no esta instalado o no esta en PATH
    echo.
    echo Descarga .NET 10 SDK desde:
    echo https://dotnet.microsoft.com/download/dotnet/10.0
    echo.
    pause
    exit /b 1
)

echo [OK] Proyecto encontrado: Tweaker\Tweaker.csproj
for /f "tokens=*" %%v in ('dotnet --version') do echo [OK] .NET SDK Version: %%v
echo.

echo ====================================================
echo [PASO 2/6] LIMPIANDO COMPILACIONES ANTERIORES
echo ====================================================
echo.

if exist "publish\GhostOptimizer_v2.5.0" (
    echo Eliminando carpeta anterior...
    rmdir /s /q "publish\GhostOptimizer_v2.5.0"
)
if exist "publish\GhostOptimizer_v2.5.0_Windows_x64.zip" (
    echo Eliminando ZIP anterior...
    del /f /q "publish\GhostOptimizer_v2.5.0_Windows_x64.zip"
)
if not exist "publish" mkdir publish

echo [OK] Limpieza completada
echo.

echo ====================================================
echo [PASO 3/6] COMPILANDO GHOST OPTIMIZER
echo ====================================================
echo.
echo Target: .NET 10.0 (Windows x64)
echo Modo: Release - Ejecutable unico autocontenido
echo.
echo Esto puede tardar 1-2 minutos...
echo.

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
    -o publish\GhostOptimizer_v2.5.0

if errorlevel 1 (
    color 0C
    echo.
    echo ====================================================
    echo ERROR EN LA COMPILACION
    echo ====================================================
    echo.
    echo Verifica que:
    echo - Tienes .NET 10 SDK instalado correctamente
    echo - No hay errores de codigo en el proyecto
    echo - Tienes permisos de escritura en la carpeta
    echo.
    pause
    exit /b 1
)

echo.
echo [OK] Compilacion exitosa
echo.

:: Verificar que se creo el ejecutable
if not exist "publish\GhostOptimizer_v2.5.0\Tweaker.exe" (
    color 0C
    echo.
    echo ERROR: No se encontro el ejecutable compilado.
    echo Ubicacion esperada: publish\GhostOptimizer_v2.5.0\Tweaker.exe
    echo.
    pause
    exit /b 1
)

echo ====================================================
echo [PASO 4/6] COPIANDO ARCHIVOS ADICIONALES
echo ====================================================
echo.

:: Crear estructura de carpetas
if not exist "publish\GhostOptimizer_v2.5.0\Docs" mkdir "publish\GhostOptimizer_v2.5.0\Docs"
if not exist "publish\GhostOptimizer_v2.5.0\Scripts" mkdir "publish\GhostOptimizer_v2.5.0\Scripts"

:: Copiar documentacion
set DOC_COUNT=0
if exist "Release\*.md" (
    for %%f in (Release\*.md) do (
        copy "%%f" "publish\GhostOptimizer_v2.5.0\Docs\" >nul 2>&1
        set /a DOC_COUNT+=1
    )
    echo [OK] %DOC_COUNT% archivos de documentacion copiados
)

:: Copiar scripts de diagnostico
set SCRIPT_COUNT=0
if exist "Release\*.bat" (
    for %%f in (Release\*.bat) do (
        copy "%%f" "publish\GhostOptimizer_v2.5.0\Scripts\" >nul 2>&1
        set /a SCRIPT_COUNT+=1
    )
    echo [OK] %SCRIPT_COUNT% scripts de diagnostico copiados
)

echo.

echo ====================================================
echo [PASO 5/6] GENERANDO README.TXT
echo ====================================================
echo.

:: Crear README detallado
(
echo ====================================================
echo    GHOST OPTIMIZER v2.5.0
echo ====================================================
echo.
echo ## INSTALACION
echo.
echo 1. Extrae todos los archivos en una carpeta
echo 2. Ejecuta Tweaker.exe como ADMINISTRADOR
echo    ^(Click derecho -^> Ejecutar como administrador^)
echo.
echo ## REQUISITOS DEL SISTEMA
echo.
echo - Windows 10/11 ^(64 bits^)
echo - Permisos de Administrador
echo - 100 MB de espacio en disco
echo.
echo ## CARACTERISTICAS PRINCIPALES
echo.
echo ### Optimizaciones de Sistema
echo - Optimizacion de CPU y GPU
echo - Ajustes de latencia y red
echo - Mejoras de energia y rendimiento
echo - Optimizacion de mouse y perifericos
echo.
echo ### Debloat de Windows
echo - Elimina aplicaciones innecesarias
echo - Desactiva servicios no criticos
echo - Limpia telemetria y rastreadores
echo.
echo ### Game Booster Inteligente
echo - Deteccion automatica de juegos
echo - Optimizacion dinamica de recursos
echo - Limpieza de memoria en tiempo real
echo.
echo ### Sistema de Seguridad
echo - Indicadores de nivel de riesgo
echo - Backup automatico de cambios
echo - Validacion de servicios criticos
echo - Sistema de restauracion integrado
echo.
echo ## DOCUMENTACION ADICIONAL
echo.
echo Revisa la carpeta Docs\ para mas informacion:
echo - Guias de uso
echo - Notas de la version
echo - Scripts de diagnostico en Scripts\
echo.
echo ## SOPORTE Y ACTUALIZACIONES
echo.
echo - GitHub: https://github.com/Josemcboss/Tweaker
echo - Actualizaciones automaticas incluidas
echo.
echo ## LICENCIA
echo.
echo Ghost Optimizer v2.5.0
echo Copyright ^(c^) 2024 DaddyGhost
echo Todos los derechos reservados.
echo.
echo ====================================================
echo    IMPORTANTE - LEER ANTES DE USAR
echo ====================================================
echo.
echo - SIEMPRE crea un punto de restauracion de Windows
echo   antes de aplicar optimizaciones
echo - Lee los indicadores de nivel de riesgo de cada tweak
echo - El programa crea backups automaticos de cambios
echo - Si experimentas problemas, usa las herramientas
echo   de restauracion incluidas
echo.
echo ====================================================
) > "publish\GhostOptimizer_v2.5.0\README.txt"

echo [OK] README.txt generado
echo.

echo ====================================================
echo [PASO 6/6] CREANDO ARCHIVO ZIP
echo ====================================================
echo.

:: Crear ZIP usando PowerShell
echo Comprimiendo archivos...
echo (Esto puede tardar unos segundos)
echo.

:: Crear script temporal de PowerShell
echo $ErrorActionPreference = 'Stop' > "%TEMP%\create_zip.ps1"
echo $source = 'publish\GhostOptimizer_v2.5.0' >> "%TEMP%\create_zip.ps1"
echo $destination = 'publish\GhostOptimizer_v2.5.0_Windows_x64.zip' >> "%TEMP%\create_zip.ps1"
echo try { >> "%TEMP%\create_zip.ps1"
echo     if (Test-Path $destination) { Remove-Item $destination -Force } >> "%TEMP%\create_zip.ps1"
echo     Compress-Archive -Path "$source\*" -DestinationPath $destination -CompressionLevel Optimal >> "%TEMP%\create_zip.ps1"
echo     if (Test-Path $destination) { >> "%TEMP%\create_zip.ps1"
echo         $zipSize = (Get-Item $destination).Length >> "%TEMP%\create_zip.ps1"
echo         $folderSize = (Get-ChildItem $source -Recurse ^| Measure-Object -Property Length -Sum).Sum >> "%TEMP%\create_zip.ps1"
echo         Write-Host '' >> "%TEMP%\create_zip.ps1"
echo         Write-Host '[OK] ZIP CREADO EXITOSAMENTE' -ForegroundColor Green >> "%TEMP%\create_zip.ps1"
echo         Write-Host '' >> "%TEMP%\create_zip.ps1"
echo         Write-Host "  Archivo: GhostOptimizer_v2.5.0_Windows_x64.zip" -ForegroundColor Cyan >> "%TEMP%\create_zip.ps1"
echo         Write-Host "  Tamano original: $([math]::Round($folderSize/1MB, 2)) MB" -ForegroundColor Cyan >> "%TEMP%\create_zip.ps1"
echo         Write-Host "  Tamano comprimido: $([math]::Round($zipSize/1MB, 2)) MB" -ForegroundColor Cyan >> "%TEMP%\create_zip.ps1"
echo         Write-Host "  Ratio compresion: $([math]::Round(($zipSize/$folderSize)*100, 0))%%" -ForegroundColor Cyan >> "%TEMP%\create_zip.ps1"
echo         Write-Host '  Ubicacion: publish\' -ForegroundColor Cyan >> "%TEMP%\create_zip.ps1"
echo         Write-Host '' >> "%TEMP%\create_zip.ps1"
echo     } else { >> "%TEMP%\create_zip.ps1"
echo         Write-Host '[ERROR] No se pudo crear el ZIP' -ForegroundColor Red >> "%TEMP%\create_zip.ps1"
echo         exit 1 >> "%TEMP%\create_zip.ps1"
echo     } >> "%TEMP%\create_zip.ps1"
echo } catch { >> "%TEMP%\create_zip.ps1"
echo     Write-Host "[ERROR] $($_.Exception.Message)" -ForegroundColor Red >> "%TEMP%\create_zip.ps1"
echo     exit 1 >> "%TEMP%\create_zip.ps1"
echo } >> "%TEMP%\create_zip.ps1"

:: Ejecutar script de PowerShell
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\create_zip.ps1"
set ZIP_ERROR=%ERRORLEVEL%

:: Limpiar script temporal
del "%TEMP%\create_zip.ps1" 2>nul

:: Limpiar script temporal
del "%TEMP%\create_zip.ps1" 2>nul

if %ZIP_ERROR% neq 0 (
    color 0C
    echo.
    echo ERROR: Fallo la creacion del ZIP
    echo.
    pause
    exit /b 1
)

echo.
echo ====================================================
echo    COMPILACION Y EMPAQUETADO COMPLETADO
echo ====================================================
echo.
color 0A
echo [EXITO] Paquete listo para compartir
echo.
color 07
echo UBICACION:
echo   publish\GhostOptimizer_v2.5.0_Windows_x64.zip
echo.
echo CONTENIDO DEL PAQUETE:
echo   - Tweaker.exe         (ejecutable principal)
echo   - README.txt          (instrucciones)
echo   - Docs\               (documentacion completa)
echo   - Scripts\            (herramientas diagnostico)
echo.
echo ====================================================
echo    COMO COMPARTIR
echo ====================================================
echo.
echo OPCIONES RECOMENDADAS:
echo.
echo 1. Google Drive / OneDrive / Dropbox
echo    ^> Sube el ZIP y genera enlace compartido
echo.
echo 2. WeTransfer ^(https://wetransfer.com^)
echo    ^> Subida rapida, sin necesidad de cuenta
echo.
echo 3. GitHub Releases
echo    ^> Crea nuevo Release en tu repositorio
echo    ^> Adjunta el ZIP como asset
echo.
echo 4. Compartir directamente
echo    ^> Discord / Telegram / Email
echo    ^> Simplemente arrastra el ZIP
echo.
echo ====================================================
echo.
echo NOTA IMPORTANTE:
echo El ejecutable es PORTABLE (no requiere instalacion)
echo El usuario debe ejecutarlo como Administrador
echo.
echo ====================================================
echo.

:: Calcular hash SHA256 (opcional pero recomendado)
set /p CALC_HASH="Deseas calcular el hash SHA256 del ZIP? (S/N): "
if /i "%CALC_HASH%"=="S" (
    echo.
    echo Calculando hash SHA256...
    powershell -Command "& {$hash = Get-FileHash 'publish\GhostOptimizer_v2.5.0_Windows_x64.zip' -Algorithm SHA256; Write-Host ''; Write-Host 'SHA256:' $hash.Hash -ForegroundColor Yellow; Write-Host ''}"
    echo ^(Guarda este hash para verificacion de integridad^)
    echo.
)

:: Abrir carpeta
set /p OPEN="Deseas abrir la carpeta publish? (S/N): "
if /i "%OPEN%"=="S" (
    start explorer "publish"
)

echo.
echo Presiona cualquier tecla para salir...
pause >nul

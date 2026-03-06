@echo off
:: ====================================================
:: GHOST OPTIMIZER - COMPILADOR DE RELEASE
:: Genera ejecutable listo para compartir
:: ====================================================

title Ghost Optimizer - Build Release

color 0B
echo.
echo ====================================================
echo    GHOST OPTIMIZER - COMPILADOR DE RELEASE v2.5.0
echo ====================================================
echo.
echo Este script compilara el ejecutable optimizado
echo listo para compartir con otras personas.
echo.
echo Presiona cualquier tecla para comenzar...
pause >nul
echo.

:: Verificar que estamos en el directorio correcto
if not exist "Tweaker\Tweaker.csproj" (
    echo ERROR: No se encuentra Tweaker\Tweaker.csproj
    echo Asegurate de ejecutar este script desde la raiz del proyecto.
    pause
    exit /b 1
)

:: Limpiar compilaciones anteriores
echo [1/5] Limpiando compilaciones anteriores...
if exist "publish\GhostOptimizer_v2.5.0" (
    rmdir /s /q "publish\GhostOptimizer_v2.5.0"
)
if not exist "publish" mkdir publish
echo       Limpieza completada.
echo.

:: Compilar proyecto
echo [2/5] Compilando Ghost Optimizer...
echo       Target: .NET 10 (Windows x64)
echo       Modo: Release - Ejecutable unico
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
    echo.
    echo ====================================================
    echo    ERROR EN LA COMPILACION
    echo ====================================================
    echo.
    echo Verifica que:
    echo - Tienes .NET 10 SDK instalado
    echo - No hay errores de codigo
    echo - Tienes permisos de escritura
    echo.
    pause
    exit /b 1
)

echo       Compilacion completada exitosamente.
echo.

:: Copiar archivos adicionales
echo [3/5] Copiando archivos adicionales...

:: Crear estructura de carpetas
if not exist "publish\GhostOptimizer_v2.5.0\Docs" mkdir "publish\GhostOptimizer_v2.5.0\Docs"
if not exist "publish\GhostOptimizer_v2.5.0\Scripts" mkdir "publish\GhostOptimizer_v2.5.0\Scripts"

:: Copiar documentacion si existe
if exist "Release\*.md" (
    xcopy "Release\*.md" "publish\GhostOptimizer_v2.5.0\Docs\" /Y /I /Q >nul 2>&1
    echo       - Documentacion copiada
)

:: Copiar scripts de diagnostico si existen
if exist "Release\*.bat" (
    xcopy "Release\*.bat" "publish\GhostOptimizer_v2.5.0\Scripts\" /Y /I /Q >nul 2>&1
    echo       - Scripts de diagnostico copiados
)

echo.

:: Crear README
echo [4/5] Generando README...
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

echo       README.txt creado
echo.

:: Mostrar resumen
echo [5/5] Generando resumen de la compilacion...
echo.

:: Calcular tamano del ejecutable
for %%A in ("publish\GhostOptimizer_v2.5.0\Tweaker.exe") do set size=%%~zA

echo ====================================================
echo    COMPILACION COMPLETADA EXITOSAMENTE
echo ====================================================
echo.
echo UBICACION: publish\GhostOptimizer_v2.5.0\
echo.
echo EJECUTABLE: Tweaker.exe
echo TAMANO: %size% bytes
echo.
echo ARCHIVOS INCLUIDOS:
echo - Tweaker.exe ^(ejecutable principal^)
echo - README.txt ^(instrucciones^)
echo - Docs\ ^(documentacion^)
echo - Scripts\ ^(herramientas de diagnostico^)
echo.
echo ====================================================
echo    SIGUIENTE PASO: COMPARTIR
echo ====================================================
echo.
echo Para compartir la aplicacion:
echo.
echo 1. Comprime la carpeta completa:
echo    publish\GhostOptimizer_v2.5.0\
echo.
echo 2. Nombra el archivo ZIP:
echo    GhostOptimizer_v2.5.0_Windows_x64.zip
echo.
echo 3. Comparte el ZIP con:
echo    - Google Drive / OneDrive / Dropbox
echo    - WeTransfer
echo    - GitHub Releases
echo.
echo NOTA: El ejecutable es portable, no requiere instalacion.
echo       Toda la configuracion se guarda en la carpeta del programa.
echo.
echo ====================================================

:: Opcional: Abrir la carpeta de salida
echo.
set /p OPEN="Deseas abrir la carpeta de salida? (S/N): "
if /i "%OPEN%"=="S" start explorer "publish\GhostOptimizer_v2.5.0"

echo.
echo Presiona cualquier tecla para salir...
pause >nul

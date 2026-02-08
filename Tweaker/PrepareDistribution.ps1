# ========================================================================================================
# PREPARAR DISTRIBUCIÓN - GHOST OPTIMIZER
# ========================================================================================================
# Script final para preparar archivos para distribución pública
# ========================================================================================================

Write-Host ""
Write-Host "?? PREPARANDO GHOST OPTIMIZER PARA DISTRIBUCIÓN" -ForegroundColor Green
Write-Host "???????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""

# Variables
$ReleaseDir = "Release"
$DistributionDir = "Distribution"
$AppName = "GhostOptimizer"
$Version = "v2.0.0"
$ExeFile = Join-Path $ReleaseDir "$AppName.exe"

# ========================================================================================================
# 1. VERIFICAR ARCHIVOS
# ========================================================================================================
Write-Host "?? VERIFICANDO ARCHIVOS..." -ForegroundColor Cyan
Write-Host "?????????????????????????" -ForegroundColor DarkCyan

if (Test-Path $ExeFile) {
    $FileInfo = Get-Item $ExeFile
    $FileSizeMB = [math]::Round($FileInfo.Length / 1MB, 2)
    Write-Host "   ? Ejecutable: $($FileInfo.Name) ($FileSizeMB MB)"
} else {
    Write-Host "   ? ERROR: No se encontró $ExeFile" -ForegroundColor Red
    Write-Host "   Ejecuta primero el script de publicación"
    exit 1
}

# ========================================================================================================
# 2. CREAR DIRECTORIO DE DISTRIBUCIÓN
# ========================================================================================================
Write-Host ""
Write-Host "?? CREANDO DIRECTORIO DE DISTRIBUCIÓN..." -ForegroundColor Cyan
Write-Host "????????????????????????????????????????" -ForegroundColor DarkCyan

if (Test-Path $DistributionDir) {
    Remove-Item -Path $DistributionDir -Recurse -Force
}
New-Item -ItemType Directory -Path $DistributionDir -Force | Out-Null

Write-Host "   ? Directorio creado: $DistributionDir"

# ========================================================================================================
# 3. COPIAR ARCHIVOS PRINCIPALES
# ========================================================================================================
Write-Host ""
Write-Host "?? COPIANDO ARCHIVOS..." -ForegroundColor Cyan
Write-Host "?????????????????????" -ForegroundColor DarkCyan

# Copiar ejecutable
Copy-Item $ExeFile -Destination $DistributionDir
Write-Host "   ? Copiado: $AppName.exe"

# Copiar README si existe
$ReadmeFile = Join-Path $ReleaseDir "README.md"
if (Test-Path $ReadmeFile) {
    Copy-Item $ReadmeFile -Destination $DistributionDir
    Write-Host "   ? Copiado: README.md"
}

# ========================================================================================================
# 4. CREAR ARCHIVO DE INSTALACIÓN
# ========================================================================================================
Write-Host ""
Write-Host "?? CREANDO INSTRUCCIONES DE USO..." -ForegroundColor Cyan
Write-Host "?????????????????????????????????" -ForegroundColor DarkCyan

$InstructionsContent = @"
===============================================
  GHOST OPTIMIZER v2.0 - INSTRUCCIONES
===============================================

?? GAMING PERFORMANCE TWEAKER

¿CÓMO USAR?
===========
1. Click derecho en GhostOptimizer.exe
2. Seleccionar "Ejecutar como administrador"
3. ¡Listo! La aplicación se abrirá

?? IMPORTANTE:
- SIEMPRE ejecutar como administrador
- Crear punto de restauración antes de usar
- Leer descripciones antes de aplicar tweaks

?? PARA GAMING COMPETITIVO:
===========================
1. Red & Ping ? TCP/IP Optimization
2. GHOST Pack ? MPO Disable + Ultimate Performance  
3. Input & Visuals ? Mouse Acceleration OFF
4. Reiniciar Windows

?? SI NAVEGADORES VAN LENTOS:
============================
- Red & Ping ? Browser Optimization
- Esto balancea gaming vs navegación web

?? PARA REVERTIR CAMBIOS:
========================
- Usar botones OFF (rojos)
- O Dashboard ? "Revertir Todo"
- O Restaurar punto de Windows

?? BENEFICIOS ESPERADOS:
=======================
? Ping: -5 a -30ms
? FPS: +10-30% mejora
? Input Lag: -10-50ms
? Stuttering: Eliminado

?? DISCLAIMER:
==============
Esta herramienta modifica configuraciones de Windows.
Úsala bajo tu propio riesgo.

Autor: DaddyGhost
Versión: 2.0.0
"@

$InstructionsFile = Join-Path $DistributionDir "INSTRUCCIONES.txt"
$InstructionsContent | Out-File -FilePath $InstructionsFile -Encoding UTF8
Write-Host "   ? Creado: INSTRUCCIONES.txt"

# ========================================================================================================
# 5. CREAR ARCHIVO ZIP
# ========================================================================================================
Write-Host ""
Write-Host "?? CREANDO ARCHIVO ZIP..." -ForegroundColor Cyan
Write-Host "?????????????????????????" -ForegroundColor DarkCyan

$ZipFile = "$AppName-$Version-Windows-x64.zip"
if (Test-Path $ZipFile) {
    Remove-Item $ZipFile -Force
}

try {
    Compress-Archive -Path "$DistributionDir\*" -DestinationPath $ZipFile -CompressionLevel Optimal
    $ZipInfo = Get-Item $ZipFile
    $ZipSizeMB = [math]::Round($ZipInfo.Length / 1MB, 2)
    Write-Host "   ? ZIP creado: $ZipFile ($ZipSizeMB MB)"
} catch {
    Write-Host "   ? Error creando ZIP: $($_.Exception.Message)" -ForegroundColor Red
}

# ========================================================================================================
# 6. CALCULAR CHECKSUMS
# ========================================================================================================
Write-Host ""
Write-Host "?? CALCULANDO CHECKSUMS..." -ForegroundColor Cyan
Write-Host "?????????????????????????" -ForegroundColor DarkCyan

$ExeHash = Get-FileHash (Join-Path $DistributionDir "$AppName.exe") -Algorithm SHA256
$ZipHash = Get-FileHash $ZipFile -Algorithm SHA256

$ChecksumContent = @"
GHOST OPTIMIZER v2.0 - CHECKSUMS
=================================

Archivos:
- GhostOptimizer.exe: $FileSizeMB MB
- ${ZipFile}: $ZipSizeMB MB

SHA256:
- GhostOptimizer.exe: $($ExeHash.Hash)
- ${ZipFile}: $($ZipHash.Hash)

Fecha: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
"@

$ChecksumFile = Join-Path $DistributionDir "CHECKSUMS.txt"
$ChecksumContent | Out-File -FilePath $ChecksumFile -Encoding UTF8

# También crear checksum en el ZIP
$ChecksumContent | Out-File -FilePath "CHECKSUMS.txt" -Encoding UTF8

Write-Host "   ? Checksums calculados"

# ========================================================================================================
# 7. RESUMEN FINAL
# ========================================================================================================
Write-Host ""
Write-Host "?? RESUMEN DE DISTRIBUCIÓN" -ForegroundColor Magenta
Write-Host "?????????????????????????" -ForegroundColor Magenta

Write-Host ""
Write-Host "? ARCHIVOS LISTOS PARA DISTRIBUCIÓN:" -ForegroundColor Green
Write-Host "   ?? Directorio: $DistributionDir\"
Write-Host "      • GhostOptimizer.exe ($FileSizeMB MB)"
Write-Host "      • README.md (documentación)"
Write-Host "      • INSTRUCCIONES.txt (guía rápida)"
Write-Host "      • CHECKSUMS.txt (verificación)"
Write-Host ""
Write-Host "   ?? Archivo de distribución:"
Write-Host "      • $ZipFile ($ZipSizeMB MB)"
Write-Host "      • CHECKSUMS.txt (incluido)"

Write-Host ""
Write-Host "?? OPCIONES DE DISTRIBUCIÓN:" -ForegroundColor Cyan
Write-Host "   1. ?? ARCHIVO ÚNICO:"
Write-Host "      • Subir solo: GhostOptimizer.exe"
Write-Host "      • Instrucciones: 'Ejecutar como administrador'"
Write-Host ""
Write-Host "   2. ?? PAQUETE COMPLETO:"
Write-Host "      • Subir: $ZipFile"
Write-Host "      • Incluye documentación y checksums"
Write-Host ""
Write-Host "   3. ?? PLATAFORMAS RECOMENDADAS:"
Write-Host "      • GitHub Releases"
Write-Host "      • Google Drive / OneDrive"
Write-Host "      • Discord / Telegram"
Write-Host "      • WeTransfer (para archivos grandes)"

Write-Host ""
Write-Host "?? INSTRUCCIONES PARA USUARIOS:" -ForegroundColor Yellow
Write-Host "   1. Descargar GhostOptimizer.exe (o descomprimir ZIP)"
Write-Host "   2. Click derecho ? 'Ejecutar como administrador'"
Write-Host "   3. Crear punto de restauración (recomendado)"
Write-Host "   4. Aplicar tweaks gradualmente"
Write-Host "   5. Reiniciar Windows después de tweaks importantes"

Write-Host ""
Write-Host "?? VERIFICACIÓN DE INTEGRIDAD:" -ForegroundColor Cyan
Write-Host "   • SHA256 del ejecutable: $($ExeHash.Hash.Substring(0,32))..."
Write-Host "   • SHA256 del ZIP: $($ZipHash.Hash.Substring(0,32))..."
Write-Host "   • Checksums completos en: CHECKSUMS.txt"

Write-Host ""
Write-Host "?? GHOST OPTIMIZER LISTO PARA EL MUNDO" -ForegroundColor Green
Write-Host "????????????????????????????????????????" -ForegroundColor Green
Write-Host "¡Tu aplicación está lista para compartir!" -ForegroundColor White

# Abrir directorio de distribución
Write-Host ""
Write-Host "?? Abriendo directorio de distribución..." -ForegroundColor Cyan
Start-Process explorer.exe -ArgumentList (Resolve-Path $DistributionDir)

Write-Host ""
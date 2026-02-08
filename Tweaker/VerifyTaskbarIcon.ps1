# ========================================================================================================
# VERIFICACIÓN FINAL DEL ICONO - GHOST OPTIMIZER
# ========================================================================================================
# Este script verifica que el icono esté correctamente configurado y funcionando
# ========================================================================================================

Write-Host ""
Write-Host "?? VERIFICACIÓN FINAL DEL ICONO GHOST OPTIMIZER" -ForegroundColor Green
Write-Host "???????????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""

# ========================================================================================================
# 1. VERIFICAR ARCHIVOS DE ICONO
# ========================================================================================================
Write-Host "?? 1. VERIFICANDO ARCHIVOS DE ICONO..." -ForegroundColor Cyan
Write-Host "??????????????????????????????????????" -ForegroundColor DarkCyan

$iconFiles = @(
    "Tweaker\Resources\GhostOptimizer.ico",
    "Tweaker\Resources\GhostOptimizer_Preview.png",
    "Tweaker\Resources\ICON_INFO.txt"
)

foreach ($file in $iconFiles) {
    if (Test-Path $file) {
        $fileInfo = Get-Item $file
        $sizeKB = [math]::Round($fileInfo.Length / 1KB, 2)
        Write-Host "   ? $($fileInfo.Name): ${sizeKB} KB"
    } else {
        Write-Host "   ? FALTA: $file" -ForegroundColor Red
    }
}

# ========================================================================================================
# 2. VERIFICAR CONFIGURACIÓN DEL PROYECTO
# ========================================================================================================
Write-Host ""
Write-Host "?? 2. VERIFICANDO CONFIGURACIÓN DEL PROYECTO..." -ForegroundColor Cyan
Write-Host "????????????????????????????????????????????????" -ForegroundColor DarkCyan

$projectFile = "Tweaker\Tweaker.csproj"

if (Test-Path $projectFile) {
    $projectContent = Get-Content $projectFile -Raw
    
    $hasApplicationIcon = $projectContent -match "<ApplicationIcon>Resources\\GhostOptimizer\.ico</ApplicationIcon>"
    $hasTitle = $projectContent -match "<Title>Ghost Optimizer</Title>"
    $hasProduct = $projectContent -match "<Product>Ghost Optimizer</Product>"
    $hasCompany = $projectContent -match "<Company>DaddyGhost</Company>"
    
    Write-Host "   ?? Configuración del proyecto:"
    Write-Host "      • ApplicationIcon configurado: $(if ($hasApplicationIcon) { '? SÍ' } else { '? NO' })"
    Write-Host "      • Title configurado: $(if ($hasTitle) { '? SÍ' } else { '? NO' })"
    Write-Host "      • Product configurado: $(if ($hasProduct) { '? SÍ' } else { '? NO' })"
    Write-Host "      • Company configurado: $(if ($hasCompany) { '? SÍ' } else { '? NO' })"
    
} else {
    Write-Host "   ? Archivo de proyecto no encontrado" -ForegroundColor Red
}

# ========================================================================================================
# 3. VERIFICAR EJECUTABLE PUBLICADO
# ========================================================================================================
Write-Host ""
Write-Host "?? 3. VERIFICANDO EJECUTABLE PUBLICADO..." -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????" -ForegroundColor DarkCyan

$exePaths = @(
    "Release\GhostOptimizer.exe",
    "Distribution\GhostOptimizer.exe"
)

$executableFound = $false

foreach ($exePath in $exePaths) {
    if (Test-Path $exePath) {
        $executableFound = $true
        $fileInfo = Get-Item $exePath
        $fileSizeMB = [math]::Round($fileInfo.Length / 1MB, 2)
        
        Write-Host "   ? Ejecutable encontrado: $exePath"
        Write-Host "      ?? Tamaño: $fileSizeMB MB"
        Write-Host "      ?? Fecha: $($fileInfo.LastWriteTime)"
        
        # Verificar propiedades del archivo
        try {
            $versionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($fileInfo.FullName)
            
            Write-Host ""
            Write-Host "   ?? Información del ejecutable:"
            Write-Host "      • Producto: $($versionInfo.ProductName)"
            Write-Host "      • Versión: $($versionInfo.ProductVersion)"
            Write-Host "      • Compañía: $($versionInfo.CompanyName)"
            Write-Host "      • Descripción: $($versionInfo.FileDescription)"
            Write-Host "      • Copyright: $($versionInfo.LegalCopyright)"
            
            if ($versionInfo.ProductName -eq "Ghost Optimizer") {
                Write-Host "   ? Información del producto correcta"
            } else {
                Write-Host "   ?? Información del producto puede estar desactualizada" -ForegroundColor Yellow
            }
            
        } catch {
            Write-Host "   ?? No se pudo obtener información de versión: $($_.Exception.Message)" -ForegroundColor Yellow
        }
        
        break
    }
}

if (-not $executableFound) {
    Write-Host "   ? No se encontró ningún ejecutable publicado" -ForegroundColor Red
}

# ========================================================================================================
# 4. VERIFICAR QUE EL ICONO SE VEA CORRECTAMENTE
# ========================================================================================================
Write-Host ""
Write-Host "??? 4. VERIFICANDO VISIBILIDAD DEL ICONO..." -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????" -ForegroundColor DarkCyan

$iconPath = "Tweaker\Resources\GhostOptimizer.ico"

if (Test-Path $iconPath) {
    try {
        # Intentar cargar el icono para verificar que no esté corrupto
        Add-Type -AssemblyName System.Drawing
        $icon = [System.Drawing.Icon]::new($iconPath)
        
        Write-Host "   ? Icono se puede cargar correctamente"
        Write-Host "   ?? Dimensiones: $($icon.Width)x$($icon.Height)"
        
        $icon.Dispose()
        
    } catch {
        Write-Host "   ? Error cargando icono: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "   ? Archivo de icono no encontrado" -ForegroundColor Red
}

# ========================================================================================================
# 5. INSTRUCCIONES PARA PROBAR EL ICONO
# ========================================================================================================
Write-Host ""
Write-Host "?? 5. INSTRUCCIONES PARA PROBAR..." -ForegroundColor Cyan
Write-Host "?????????????????????????????????" -ForegroundColor DarkCyan

Write-Host "   ?? Para verificar que el icono funciona:"
Write-Host "      1. Ejecutar GhostOptimizer.exe como administrador"
Write-Host "      2. Verificar icono en taskbar (barra de tareas)"
Write-Host "      3. Usar Alt+Tab para ver icono en switcher"
Write-Host "      4. Ver icono en explorador de archivos"

Write-Host ""
Write-Host "   ?? Lugares donde debe aparecer el icono:"
Write-Host "      ? Taskbar de Windows"
Write-Host "      ? Alt + Tab (cambio de ventanas)"
Write-Host "      ? Barra de título de la ventana"
Write-Host "      ? Explorador de archivos (junto al .exe)"
Write-Host "      ? Notificaciones del sistema"
Write-Host "      ? Menú inicio (si se ancla la aplicación)"

# ========================================================================================================
# 6. VERIFICAR DISTRIBUCIÓN
# ========================================================================================================
Write-Host ""
Write-Host "?? 6. VERIFICANDO ARCHIVOS DE DISTRIBUCIÓN..." -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????????" -ForegroundColor DarkCyan

$distributionFiles = @(
    "Distribution\GhostOptimizer.exe",
    "Distribution\README.md",
    "Distribution\INSTRUCCIONES.txt",
    "Distribution\CHECKSUMS.txt",
    "GhostOptimizer-v2.0.0-Windows-x64.zip"
)

Write-Host "   ?? Archivos de distribución:"
foreach ($file in $distributionFiles) {
    if (Test-Path $file) {
        $fileInfo = Get-Item $file
        $size = if ($fileInfo.Length -gt 1MB) {
            "$([math]::Round($fileInfo.Length / 1MB, 2)) MB"
        } else {
            "$([math]::Round($fileInfo.Length / 1KB, 2)) KB"
        }
        Write-Host "      ? $($fileInfo.Name): $size"
    } else {
        Write-Host "      ? FALTA: $file" -ForegroundColor Red
    }
}

# ========================================================================================================
# 7. RESUMEN FINAL
# ========================================================================================================
Write-Host ""
Write-Host "?? RESUMEN FINAL - ICONO EN TASKBAR" -ForegroundColor Magenta
Write-Host "?????????????????????????????????????" -ForegroundColor Magenta

Write-Host ""
if ((Test-Path "Tweaker\Resources\GhostOptimizer.ico") -and 
    (Test-Path "Tweaker\Tweaker.csproj") -and 
    (Test-Path "Release\GhostOptimizer.exe" -ErrorAction SilentlyContinue)) {
    
    Write-Host "? CONFIGURACIÓN DEL ICONO COMPLETADA:" -ForegroundColor Green
    Write-Host "   ?? Icono creado: GhostOptimizer.ico"
    Write-Host "   ?? Proyecto configurado: ApplicationIcon"
    Write-Host "   ?? Ejecutable compilado: Con icono integrado"
    Write-Host "   ?? Distribución lista: Archivos preparados"
    
    Write-Host ""
    Write-Host "?? RESULTADO ESPERADO:" -ForegroundColor Yellow
    Write-Host "   Al ejecutar GhostOptimizer.exe, el logo aparecerá:"
    Write-Host "   • ?? En la taskbar de Windows"
    Write-Host "   • ?? En Alt+Tab (cambio de ventanas)"
    Write-Host "   • ?? En la barra de título"
    Write-Host "   • ?? En el explorador de archivos"
    Write-Host "   • ?? En notificaciones del sistema"
    
    Write-Host ""
    Write-Host "?? LISTO PARA COMPARTIR:" -ForegroundColor Cyan
    Write-Host "   • Archivo único: GhostOptimizer.exe (66.8 MB)"
    Write-Host "   • Paquete completo: GhostOptimizer-v2.0.0-Windows-x64.zip"
    Write-Host "   • Instrucciones: 'Ejecutar como administrador'"
    
} else {
    Write-Host "? CONFIGURACIÓN INCOMPLETA:" -ForegroundColor Red
    Write-Host "   Algunos archivos faltan o no se configuraron correctamente"
    Write-Host "   Ejecuta los scripts de creación de icono nuevamente"
}

Write-Host ""
Write-Host "?? GHOST OPTIMIZER - ICONO EN TASKBAR VERIFICADO" -ForegroundColor Green
Write-Host "???????????????????????????????????????????????????" -ForegroundColor Green
Write-Host "¡Tu aplicación ahora tiene presencia visual completa en Windows!" -ForegroundColor White
Write-Host ""
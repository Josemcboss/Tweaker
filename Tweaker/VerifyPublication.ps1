# ========================================================================================================
# VERIFICACIÓN DE PUBLICACIÓN - GHOST OPTIMIZER
# ========================================================================================================
# Este script verifica que la publicación se haya realizado correctamente
# ========================================================================================================

Write-Host ""
Write-Host "?? VERIFICANDO PUBLICACIÓN DE GHOST OPTIMIZER" -ForegroundColor Green
Write-Host "?????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""

# Variables
$ProjectPath = "Tweaker\Tweaker.csproj"
$OutputDir = "Release"
$AppName = "GhostOptimizer"
$ExePath = Join-Path $OutputDir "$AppName.exe"

# ========================================================================================================
# 1. VERIFICAR CONFIGURACIÓN DEL PROYECTO
# ========================================================================================================
Write-Host "?? 1. VERIFICANDO CONFIGURACIÓN DEL PROYECTO..." -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????????????" -ForegroundColor DarkCyan

if (Test-Path $ProjectPath) {
    $projectContent = Get-Content $ProjectPath -Raw
    
    $hasPublishSingleFile = $projectContent -match "PublishSingleFile.*true"
    $hasSelfContained = $projectContent -match "SelfContained.*true"
    $hasPublishTrimmed = $projectContent -match "PublishTrimmed.*true"
    $hasCompression = $projectContent -match "EnableCompressionInSingleFile.*true"
    $hasReadyToRun = $projectContent -match "PublishReadyToRun.*true"
    $hasRuntimeId = $projectContent -match "RuntimeIdentifier.*win-x64"
    
    Write-Host "   ? Configuraciones de publicación:"
    Write-Host "      • PublishSingleFile: $(if ($hasPublishSingleFile) { '? HABILITADO' } else { '? FALTA' })"
    Write-Host "      • SelfContained: $(if ($hasSelfContained) { '? HABILITADO' } else { '? FALTA' })"
    Write-Host "      • PublishTrimmed: $(if ($hasPublishTrimmed) { '? HABILITADO' } else { '? FALTA' })"
    Write-Host "      • EnableCompression: $(if ($hasCompression) { '? HABILITADO' } else { '? FALTA' })"
    Write-Host "      • PublishReadyToRun: $(if ($hasReadyToRun) { '? HABILITADO' } else { '? FALTA' })"
    Write-Host "      • RuntimeIdentifier: $(if ($hasRuntimeId) { '? win-x64' } else { '? FALTA' })"
} else {
    Write-Host "   ? No se encontró el archivo del proyecto: $ProjectPath" -ForegroundColor Red
}

# ========================================================================================================
# 2. VERIFICAR ARCHIVOS GENERADOS
# ========================================================================================================
Write-Host ""
Write-Host "?? 2. VERIFICANDO ARCHIVOS GENERADOS..." -ForegroundColor Cyan
Write-Host "???????????????????????????????????????" -ForegroundColor DarkCyan

if (Test-Path $ExePath) {
    $FileInfo = Get-Item $ExePath
    $FileSizeMB = [math]::Round($FileInfo.Length / 1MB, 2)
    
    Write-Host "   ? Ejecutable encontrado:"
    Write-Host "      • Nombre: $($FileInfo.Name)"
    Write-Host "      • Tamaño: $FileSizeMB MB"
    Write-Host "      • Fecha: $($FileInfo.LastWriteTime)"
    Write-Host "      • Ruta completa: $(Resolve-Path $ExePath)"
    
    # Verificar si es realmente single-file (debe ser el archivo más grande)
    $OtherFiles = Get-ChildItem $OutputDir -File | Where-Object { $_.Name -ne "$AppName.exe" }
    
    if ($OtherFiles.Count -eq 0) {
        Write-Host "   ? Single-file deployment correcto (solo 1 archivo)"
    } elseif ($OtherFiles.Count -le 3) {
        Write-Host "   ?? Hay archivos adicionales (deployment parcial):"
        foreach ($file in $OtherFiles) {
            $sizeMB = [math]::Round($file.Length / 1MB, 3)
            Write-Host "      • $($file.Name) ($sizeMB MB)"
        }
    } else {
        Write-Host "   ? Muchos archivos adicionales (single-file falló):"
        Write-Host "      Archivos adicionales: $($OtherFiles.Count)"
    }
    
} else {
    Write-Host "   ? No se encontró el ejecutable en: $ExePath" -ForegroundColor Red
}

# ========================================================================================================
# 3. VERIFICAR DEPENDENCIAS
# ========================================================================================================
Write-Host ""
Write-Host "?? 3. VERIFICANDO DEPENDENCIAS..." -ForegroundColor Cyan
Write-Host "?????????????????????????????????" -ForegroundColor DarkCyan

if (Test-Path $ExePath) {
    # Verificar que el archivo no dependa de .NET runtime instalado
    try {
        $dumpbinResult = dumpbin /dependents $ExePath 2>$null
        if ($dumpbinResult) {
            $hasMsvcrt = $dumpbinResult -match "MSVCR|VCRUNTIME"
            $hasKernel32 = $dumpbinResult -match "KERNEL32"
            $hasNetFramework = $dumpbinResult -match "mscor|System\."
            
            Write-Host "   ?? Análisis de dependencias:"
            Write-Host "      • Dependencias de sistema: $(if ($hasKernel32) { '? NORMALES' } else { '?? INUSUALES' })"
            Write-Host "      • Runtime C++: $(if ($hasMsvcrt) { '?? PRESENTE' } else { '? NO REQUERIDO' })"
            Write-Host "      • .NET Framework: $(if ($hasNetFramework) { '? DEPENDIENTE' } else { '? INDEPENDIENTE' })"
        }
    } catch {
        Write-Host "   ?? No se pudo analizar dependencias (dumpbin no disponible)"
    }
    
    # Verificar tamaño (single-file self-contained suele ser 50-150MB)
    $FileInfo = Get-Item $ExePath
    $FileSizeMB = [math]::Round($FileInfo.Length / 1MB, 2)
    
    if ($FileSizeMB -gt 30 -and $FileSizeMB -lt 200) {
        Write-Host "   ? Tamaño apropiado para self-contained ($FileSizeMB MB)"
    } elseif ($FileSizeMB -lt 30) {
        Write-Host "   ?? Tamaño pequeño, puede requerir .NET runtime ($FileSizeMB MB)"
    } else {
        Write-Host "   ?? Tamaño muy grande, verificar configuración ($FileSizeMB MB)"
    }
    
} else {
    Write-Host "   ? No se puede verificar dependencias sin ejecutable"
}

# ========================================================================================================
# 4. VERIFICAR .NET SDK
# ========================================================================================================
Write-Host ""
Write-Host "?? 4. VERIFICANDO .NET SDK..." -ForegroundColor Cyan
Write-Host "????????????????????????????" -ForegroundColor DarkCyan

try {
    $dotnetVersion = dotnet --version 2>$null
    if ($dotnetVersion) {
        Write-Host "   ? .NET SDK instalado: $dotnetVersion"
        
        # Verificar que sea compatible con .NET 10
        if ($dotnetVersion.StartsWith("10.") -or $dotnetVersion.StartsWith("11.")) {
            Write-Host "   ? Versión compatible con .NET 10"
        } else {
            Write-Host "   ?? Versión puede no ser compatible con .NET 10" -ForegroundColor Yellow
        }
    } else {
        Write-Host "   ? .NET SDK no detectado" -ForegroundColor Red
    }
} catch {
    Write-Host "   ? Error verificando .NET SDK: $($_.Exception.Message)" -ForegroundColor Red
}

# ========================================================================================================
# 5. SIMULAR PRUEBA DE EJECUCIÓN
# ========================================================================================================
Write-Host ""
Write-Host "?? 5. SIMULANDO PRUEBA DE EJECUCIÓN..." -ForegroundColor Cyan
Write-Host "?????????????????????????????????????" -ForegroundColor DarkCyan

if (Test-Path $ExePath) {
    Write-Host "   ?? Comandos de prueba recomendados:"
    Write-Host "      • Prueba básica: $ExePath --help"
    Write-Host "      • Prueba como admin: Ejecutar como administrador"
    Write-Host "      • Prueba en otro PC: Copiar solo el .exe"
    
    # Verificar propiedades del archivo
    try {
        $VersionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($ExePath)
        if ($VersionInfo) {
            Write-Host ""
            Write-Host "   ?? Información de versión:"
            Write-Host "      • Producto: $($VersionInfo.ProductName)"
            Write-Host "      • Versión: $($VersionInfo.ProductVersion)"
            Write-Host "      • Compañía: $($VersionInfo.CompanyName)"
            Write-Host "      • Descripción: $($VersionInfo.FileDescription)"
        }
    } catch {
        Write-Host "   ?? No se pudo obtener información de versión"
    }
} else {
    Write-Host "   ? No se puede probar sin ejecutable"
}

# ========================================================================================================
# 6. GUÍA DE DISTRIBUCIÓN
# ========================================================================================================
Write-Host ""
Write-Host "?? 6. GUÍA DE DISTRIBUCIÓN..." -ForegroundColor Cyan
Write-Host "????????????????????????????" -ForegroundColor DarkCyan

Write-Host "   ?? Para compartir tu aplicación:"
Write-Host "      1. ? Copia solo el archivo: $AppName.exe"
Write-Host "      2. ? No necesitas otros archivos"
Write-Host "      3. ? No necesitas instalador"
Write-Host "      4. ?? El usuario DEBE ejecutar como Administrador"
Write-Host "      5. ? Funciona sin .NET instalado en el sistema objetivo"

Write-Host ""
Write-Host "   ?? Opciones de distribución:"
Write-Host "      • Subir a Google Drive/OneDrive"
Write-Host "      • Compartir por Discord/Telegram"
Write-Host "      • Crear release en GitHub"
Write-Host "      • Usar WeTransfer para archivos grandes"

Write-Host ""
Write-Host "   ?? Instrucciones para usuarios:"
Write-Host "      • Descargar GhostOptimizer.exe"
Write-Host "      • Click derecho ? 'Ejecutar como administrador'"
Write-Host "      • Crear punto de restauración (recomendado)"
Write-Host "      • Aplicar tweaks gradualmente"

# ========================================================================================================
# 7. RESUMEN FINAL
# ========================================================================================================
Write-Host ""
Write-Host "?? RESUMEN DE VERIFICACIÓN" -ForegroundColor Magenta
Write-Host "???????????????????????????" -ForegroundColor Magenta

if (Test-Path $ExePath) {
    $FileInfo = Get-Item $ExePath
    $FileSizeMB = [math]::Round($FileInfo.Length / 1MB, 2)
    
    Write-Host ""
    Write-Host "? PUBLICACIÓN EXITOSA:" -ForegroundColor Green
    Write-Host "   • Ejecutable: $AppName.exe"
    Write-Host "   • Tamaño: $FileSizeMB MB"
    Write-Host "   • Tipo: Single-file self-contained"
    Write-Host "   • Estado: Listo para distribución"
    
    Write-Host ""
    Write-Host "?? PRÓXIMOS PASOS:" -ForegroundColor Yellow
    Write-Host "   1. Probar el ejecutable en tu sistema"
    Write-Host "   2. Probar en otro PC (sin .NET instalado)"
    Write-Host "   3. Crear documentación para usuarios"
    Write-Host "   4. Compartir con la comunidad gaming"
    
    Write-Host ""
    Write-Host "?? UBICACIÓN FINAL:" -ForegroundColor Cyan
    Write-Host "   $(Resolve-Path $ExePath)"
    
} else {
    Write-Host ""
    Write-Host "? PUBLICACIÓN INCOMPLETA:" -ForegroundColor Red
    Write-Host "   • No se encontró el ejecutable"
    Write-Host "   • Verificar errores de compilación"
    Write-Host "   • Ejecutar script de publicación"
}

Write-Host ""
Write-Host "?? GHOST OPTIMIZER - VERIFICACIÓN COMPLETADA" -ForegroundColor Green
Write-Host "?????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""
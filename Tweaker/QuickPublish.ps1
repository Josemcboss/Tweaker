# ========================================================================================================
# PUBLICACIÓN RÁPIDA - GHOST OPTIMIZER
# ========================================================================================================
# Script simple para generar EXE ejecutable independiente
# ========================================================================================================

Write-Host ""
Write-Host "?? GHOST OPTIMIZER - PUBLICACIÓN RÁPIDA" -ForegroundColor Green
Write-Host "??????????????????????????????????????" -ForegroundColor Green
Write-Host ""

# Variables
$ProjectPath = "Tweaker\Tweaker.csproj"
$OutputDir = "Release"
$AppName = "Tweaker"  # El nombre real del ejecutable generado

# Crear directorio si no existe
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

Write-Host "?? Publicando aplicación..." -ForegroundColor Cyan
Write-Host "??????????????????????????" -ForegroundColor DarkCyan

# Comando de publicación optimizado
try {
    dotnet publish $ProjectPath `
        --configuration Release `
        --runtime win-x64 `
        --self-contained true `
        --output $OutputDir `
        /p:PublishSingleFile=true `
        /p:PublishTrimmed=false `
        /p:EnableCompressionInSingleFile=true `
        /p:PublishReadyToRun=true `
        --verbosity minimal
        
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "? PUBLICACIÓN EXITOSA" -ForegroundColor Green
        
        $ExePath = Join-Path $OutputDir "$AppName.exe"
        if (Test-Path $ExePath) {
            $FileInfo = Get-Item $ExePath
            $FileSizeMB = [math]::Round($FileInfo.Length / 1MB, 2)
            
            Write-Host ""
            Write-Host "?? ARCHIVO GENERADO:" -ForegroundColor Cyan
            Write-Host "   Ejecutable: $AppName.exe"
            Write-Host "   Tamaño: $FileSizeMB MB"
            Write-Host "   Ubicación: $(Resolve-Path $ExePath)"
            
            Write-Host ""
            Write-Host "?? PARA COMPARTIR:" -ForegroundColor Yellow
            Write-Host "   1. Copia el archivo $AppName.exe"
            Write-Host "   2. Comparte solo ese archivo"
            Write-Host "   3. Instrucciones: 'Ejecutar como Administrador'"
            
            Write-Host ""
            Write-Host "?? Abriendo directorio..." -ForegroundColor Cyan
            Start-Process explorer.exe -ArgumentList (Resolve-Path $OutputDir)
            
        } else {
            Write-Host "? ERROR: No se encontró el ejecutable" -ForegroundColor Red
        }
    } else {
        throw "Error en el comando dotnet publish"
    }
    
} catch {
    Write-Host "? ERROR EN PUBLICACIÓN:" -ForegroundColor Red
    Write-Host "   $($_.Exception.Message)" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "?? SOLUCIONES:" -ForegroundColor Cyan
    Write-Host "   • Verificar que .NET 10 SDK esté instalado"
    Write-Host "   • Ejecutar desde el directorio raíz del proyecto"
    Write-Host "   • Verificar que el proyecto compile (dotnet build)"
}

Write-Host ""
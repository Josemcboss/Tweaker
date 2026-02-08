# ?? Script de Publicación - GHOST OPTIMIZER
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  ?? GHOST OPTIMIZER - PUBLICACIÓN" -ForegroundColor Green
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Variables
$projectName = "Tweaker"
$outputDir = "Publish"
$version = "2.0.0"

Write-Host "?? Configuración:" -ForegroundColor Yellow
Write-Host "  Proyecto: $projectName" -ForegroundColor Gray
Write-Host "  Versión: $version" -ForegroundColor Gray
Write-Host "  Directorio: $outputDir" -ForegroundColor Gray
Write-Host ""

# Limpiar directorio anterior
if (Test-Path $outputDir) {
    Write-Host "??? Limpiando publicación anterior..." -ForegroundColor Yellow
    Remove-Item -Recurse -Force $outputDir
}

# Crear directorio de publicación
New-Item -ItemType Directory -Path $outputDir | Out-Null

Write-Host ""
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  ?? OPCIÓN 1: Framework-Dependent" -ForegroundColor Yellow
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""
Write-Host "Características:" -ForegroundColor White
Write-Host "  ? Tamaño pequeño (~5-10 MB)" -ForegroundColor Green
Write-Host "  ? Rápida de crear" -ForegroundColor Green
Write-Host "  ? Requiere .NET 10 instalado" -ForegroundColor Red
Write-Host ""

$option1 = Read-Host "¿Crear versión Framework-Dependent? (S/N)"

if ($option1 -eq "S" -or $option1 -eq "s") {
    Write-Host ""
    Write-Host "?? Compilando Framework-Dependent..." -ForegroundColor Cyan
    
    dotnet publish -c Release -o "$outputDir\Framework-Dependent" --self-contained false
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "? Publicación Framework-Dependent completada" -ForegroundColor Green
        Write-Host "?? Ubicación: $outputDir\Framework-Dependent" -ForegroundColor Gray
        
        # Calcular tamaño
        $size = (Get-ChildItem "$outputDir\Framework-Dependent" -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
        Write-Host "?? Tamaño: $([math]::Round($size, 2)) MB" -ForegroundColor Gray
    } else {
        Write-Host "? Error en compilación" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  ?? OPCIÓN 2: Self-Contained (Windows)" -ForegroundColor Yellow
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""
Write-Host "Características:" -ForegroundColor White
Write-Host "  ? No requiere .NET instalado" -ForegroundColor Green
Write-Host "  ? Funciona en cualquier Windows 10+" -ForegroundColor Green
Write-Host "  ? Tamaño grande (~150-200 MB)" -ForegroundColor Red
Write-Host ""

$option2 = Read-Host "¿Crear versión Self-Contained? (S/N)"

if ($option2 -eq "S" -or $option2 -eq "s") {
    Write-Host ""
    Write-Host "?? Compilando Self-Contained para Windows x64..." -ForegroundColor Cyan
    
    dotnet publish -c Release -r win-x64 -o "$outputDir\Self-Contained" --self-contained true -p:PublishSingleFile=true
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "? Publicación Self-Contained completada" -ForegroundColor Green
        Write-Host "?? Ubicación: $outputDir\Self-Contained" -ForegroundColor Gray
        
        # Calcular tamaño
        $size = (Get-ChildItem "$outputDir\Self-Contained" -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
        Write-Host "?? Tamaño: $([math]::Round($size, 2)) MB" -ForegroundColor Gray
    } else {
        Write-Host "? Error en compilación" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  ?? OPCIÓN 3: Single-File Executable" -ForegroundColor Yellow
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""
Write-Host "Características:" -ForegroundColor White
Write-Host "  ? Un solo archivo .exe" -ForegroundColor Green
Write-Host "  ? No requiere .NET instalado" -ForegroundColor Green
Write-Host "  ? Fácil de distribuir" -ForegroundColor Green
Write-Host "  ?? Tamaño ~150 MB" -ForegroundColor Yellow
Write-Host ""

$option3 = Read-Host "¿Crear Single-File Executable? (S/N)"

if ($option3 -eq "S" -or $option3 -eq "s") {
    Write-Host ""
    Write-Host "?? Compilando Single-File Executable..." -ForegroundColor Cyan
    
    dotnet publish -c Release -r win-x64 -o "$outputDir\Single-File" --self-contained true `
        -p:PublishSingleFile=true `
        -p:IncludeNativeLibrariesForSelfExtract=true `
        -p:EnableCompressionInSingleFile=true
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "? Single-File Executable completado" -ForegroundColor Green
        Write-Host "?? Ubicación: $outputDir\Single-File" -ForegroundColor Gray
        
        # Buscar el .exe
        $exeFile = Get-ChildItem "$outputDir\Single-File\*.exe" | Select-Object -First 1
        if ($exeFile) {
            $sizeMB = $exeFile.Length / 1MB
            Write-Host "?? Tamaño del .exe: $([math]::Round($sizeMB, 2)) MB" -ForegroundColor Gray
            Write-Host "?? Archivo: $($exeFile.Name)" -ForegroundColor Gray
        }
    } else {
        Write-Host "? Error en compilación" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  ?? CREAR ARCHIVO ZIP" -ForegroundColor Yellow
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

$createZip = Read-Host "¿Crear archivo ZIP para distribución? (S/N)"

if ($createZip -eq "S" -or $createZip -eq "s") {
    Write-Host ""
    Write-Host "?? Creando archivos ZIP..." -ForegroundColor Cyan
    
    if (Test-Path "$outputDir\Framework-Dependent") {
        Compress-Archive -Path "$outputDir\Framework-Dependent\*" -DestinationPath "$outputDir\GHOST-Optimizer-v$version-Framework.zip" -Force
        Write-Host "? Framework-Dependent.zip creado" -ForegroundColor Green
    }
    
    if (Test-Path "$outputDir\Self-Contained") {
        Compress-Archive -Path "$outputDir\Self-Contained\*" -DestinationPath "$outputDir\GHOST-Optimizer-v$version-Full.zip" -Force
        Write-Host "? Self-Contained.zip creado" -ForegroundColor Green
    }
    
    if (Test-Path "$outputDir\Single-File") {
        Compress-Archive -Path "$outputDir\Single-File\*" -DestinationPath "$outputDir\GHOST-Optimizer-v$version-Portable.zip" -Force
        Write-Host "? Single-File.zip creado" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  ? PUBLICACIÓN COMPLETADA" -ForegroundColor Green
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

Write-Host "?? RESUMEN:" -ForegroundColor White
Write-Host ""

if (Test-Path "$outputDir\Framework-Dependent") {
    $size = (Get-ChildItem "$outputDir\Framework-Dependent" -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
    Write-Host "  ?? Framework-Dependent: $([math]::Round($size, 2)) MB" -ForegroundColor Gray
}

if (Test-Path "$outputDir\Self-Contained") {
    $size = (Get-ChildItem "$outputDir\Self-Contained" -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
    Write-Host "  ?? Self-Contained: $([math]::Round($size, 2)) MB" -ForegroundColor Gray
}

if (Test-Path "$outputDir\Single-File") {
    $exe = Get-ChildItem "$outputDir\Single-File\*.exe" | Select-Object -First 1
    if ($exe) {
        $sizeMB = $exe.Length / 1MB
        Write-Host "  ?? Single-File: $([math]::Round($sizeMB, 2)) MB" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "?? Todos los archivos en: $outputDir" -ForegroundColor Cyan
Write-Host ""

Write-Host "?? PRÓXIMOS PASOS:" -ForegroundColor Yellow
Write-Host "  1. Probar el ejecutable" -ForegroundColor Gray
Write-Host "  2. Crear README.md de usuario" -ForegroundColor Gray
Write-Host "  3. Subir a GitHub Release" -ForegroundColor Gray
Write-Host "  4. Compartir link de descarga" -ForegroundColor Gray
Write-Host ""

# Abrir carpeta de publicación
$openFolder = Read-Host "¿Abrir carpeta de publicación? (S/N)"
if ($openFolder -eq "S" -or $openFolder -eq "s") {
    Invoke-Item $outputDir
}

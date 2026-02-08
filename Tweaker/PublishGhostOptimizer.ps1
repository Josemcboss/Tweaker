# ========================================================================================================
# SCRIPT DE PUBLICACIÓN - GHOST OPTIMIZER
# ========================================================================================================
# Este script publica la aplicación Ghost Optimizer como un EXE independiente listo para distribución
# ========================================================================================================

param(
    [Parameter(HelpMessage="Configuración de build (Release/Debug)")]
    [ValidateSet("Release", "Debug")]
    [string]$Configuration = "Release",
    
    [Parameter(HelpMessage="Arquitectura objetivo")]
    [ValidateSet("win-x64", "win-x86", "win-arm64")]
    [string]$Runtime = "win-x64",
    
    [Parameter(HelpMessage="Crear también versión portable")]
    [switch]$IncludePortable,
    
    [Parameter(HelpMessage="Abrir carpeta después de publicar")]
    [switch]$OpenFolder
)

Write-Host ""
Write-Host "?? GHOST OPTIMIZER - PUBLICACIÓN AUTOMATIZADA" -ForegroundColor Green
Write-Host "???????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""

# Variables de configuración
$ProjectPath = "Tweaker\Tweaker.csproj"
$SolutionPath = "Tweaker.sln"
$OutputDir = "Publish"
$AppName = "GhostOptimizer"
$Version = "v2.0.0"

# Crear directorio de salida
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

Write-Host "?? CONFIGURACIÓN DE PUBLICACIÓN:" -ForegroundColor Cyan
Write-Host "????????????????????????????????" -ForegroundColor DarkCyan
Write-Host "   Configuración: $Configuration"
Write-Host "   Plataforma: $Runtime"
Write-Host "   Proyecto: $ProjectPath"
Write-Host "   Directorio de salida: $OutputDir"
Write-Host ""

# ========================================================================================================
# 1. VERIFICAR PRERREQUISITOS
# ========================================================================================================
Write-Host "?? VERIFICANDO PRERREQUISITOS..." -ForegroundColor Cyan
Write-Host "?????????????????????????????????" -ForegroundColor DarkCyan

# Verificar que existe el proyecto
if (-not (Test-Path $ProjectPath)) {
    Write-Host "   ? ERROR: No se encontró el proyecto en $ProjectPath" -ForegroundColor Red
    exit 1
}

# Verificar .NET SDK
try {
    $dotnetVersion = dotnet --version 2>$null
    if ($dotnetVersion) {
        Write-Host "   ? .NET SDK: $dotnetVersion"
    } else {
        throw "No se pudo obtener la versión de .NET"
    }
} catch {
    Write-Host "   ? ERROR: .NET SDK no está instalado o no está en PATH" -ForegroundColor Red
    exit 1
}

# Verificar que el proyecto compila
Write-Host "   ?? Verificando que el proyecto compila..."
$buildResult = dotnet build $ProjectPath --configuration $Configuration --verbosity minimal 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "   ? ERROR: El proyecto no compila correctamente" -ForegroundColor Red
    Write-Host "   Detalles del error:"
    $buildResult | Write-Host -ForegroundColor Yellow
    exit 1
}
Write-Host "   ? Proyecto compila correctamente"

Write-Host ""

# ========================================================================================================
# 2. LIMPIAR Y PREPARAR
# ========================================================================================================
Write-Host "?? PREPARANDO PUBLICACIÓN..." -ForegroundColor Cyan
Write-Host "????????????????????????????" -ForegroundColor DarkCyan

# Limpiar builds anteriores
Write-Host "   ?? Limpiando builds anteriores..."
dotnet clean $ProjectPath --configuration $Configuration --verbosity minimal | Out-Null

# Restaurar dependencias
Write-Host "   ?? Restaurando paquetes NuGet..."
dotnet restore $ProjectPath --verbosity minimal | Out-Null

# Limpiar directorio de salida anterior
$PublishPath = Join-Path $OutputDir "$AppName-$Version-$Runtime"
if (Test-Path $PublishPath) {
    Write-Host "   ??? Limpiando directorio de publicación anterior..."
    Remove-Item -Path $PublishPath -Recurse -Force
}

Write-Host "   ? Preparación completada"
Write-Host ""

# ========================================================================================================
# 3. PUBLICACIÓN PRINCIPAL (SINGLE FILE)
# ========================================================================================================
Write-Host "?? PUBLICANDO APLICACIÓN (SINGLE FILE)..." -ForegroundColor Green
Write-Host "??????????????????????????????????????????" -ForegroundColor DarkGreen

$PublishArgs = @(
    "publish"
    $ProjectPath
    "--configuration", $Configuration
    "--runtime", $Runtime
    "--self-contained", "true"
    "--output", $PublishPath
    "/p:PublishSingleFile=true"
    "/p:PublishTrimmed=true"
    "/p:TrimMode=link"
    "/p:EnableCompressionInSingleFile=true"
    "/p:IncludeNativeLibrariesForSelfExtract=true"
    "/p:PublishReadyToRun=true"
    "/p:DebugType=embedded"
    "/p:DebugSymbols=false"
    "--verbosity", "normal"
)

Write-Host "   ?? Ejecutando comando de publicación..."
Write-Host "   dotnet $($PublishArgs -join ' ')" -ForegroundColor DarkGray

$publishResult = dotnet @PublishArgs 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "   ? Publicación exitosa" -ForegroundColor Green
} else {
    Write-Host "   ? ERROR en publicación:" -ForegroundColor Red
    $publishResult | Write-Host -ForegroundColor Yellow
    exit 1
}

# ========================================================================================================
# 4. VERIFICAR ARCHIVOS PUBLICADOS
# ========================================================================================================
Write-Host ""
Write-Host "?? VERIFICANDO ARCHIVOS PUBLICADOS..." -ForegroundColor Cyan
Write-Host "????????????????????????????????????????" -ForegroundColor DarkCyan

$ExePath = Join-Path $PublishPath "$AppName.exe"
if (Test-Path $ExePath) {
    $FileInfo = Get-Item $ExePath
    $FileSizeMB = [math]::Round($FileInfo.Length / 1MB, 2)
    
    Write-Host "   ? Ejecutable creado: $($FileInfo.Name)"
    Write-Host "   ?? Tamaño: $FileSizeMB MB"
    Write-Host "   ?? Fecha: $($FileInfo.LastWriteTime)"
    Write-Host "   ?? Ruta: $ExePath"
} else {
    Write-Host "   ? ERROR: No se encontró el ejecutable en $ExePath" -ForegroundColor Red
    exit 1
}

# Verificar otros archivos importantes
$OtherFiles = Get-ChildItem $PublishPath -File | Where-Object { $_.Name -ne "$AppName.exe" }
Write-Host ""
Write-Host "   ?? Otros archivos en el directorio:"
foreach ($file in $OtherFiles) {
    $sizeMB = [math]::Round($file.Length / 1MB, 3)
    Write-Host "      • $($file.Name) ($sizeMB MB)"
}

# ========================================================================================================
# 5. CREAR VERSIÓN PORTABLE (OPCIONAL)
# ========================================================================================================
if ($IncludePortable) {
    Write-Host ""
    Write-Host "?? CREANDO VERSIÓN PORTABLE..." -ForegroundColor Cyan
    Write-Host "?????????????????????????????????" -ForegroundColor DarkCyan
    
    $PortablePath = Join-Path $OutputDir "$AppName-$Version-$Runtime-Portable"
    
    $PortableArgs = @(
        "publish"
        $ProjectPath
        "--configuration", $Configuration
        "--runtime", $Runtime
        "--self-contained", "true"
        "--output", $PortablePath
        "/p:PublishSingleFile=false"
        "/p:PublishTrimmed=true"
        "/p:PublishReadyToRun=true"
        "--verbosity", "minimal"
    )
    
    dotnet @PortableArgs | Out-Null
    
    if (Test-Path (Join-Path $PortablePath "$AppName.exe")) {
        Write-Host "   ? Versión portable creada en: $PortablePath"
        
        # Contar archivos en versión portable
        $PortableFiles = Get-ChildItem $PortablePath -File
        Write-Host "   ?? Archivos en versión portable: $($PortableFiles.Count)"
    } else {
        Write-Host "   ? ERROR: No se pudo crear la versión portable" -ForegroundColor Red
    }
}

# ========================================================================================================
# 6. CREAR DOCUMENTACIÓN DE DISTRIBUCIÓN
# ========================================================================================================
Write-Host ""
Write-Host "?? CREANDO DOCUMENTACIÓN..." -ForegroundColor Cyan
Write-Host "???????????????????????????????" -ForegroundColor DarkCyan

$ReadmeContent = @"
# ?? GHOST OPTIMIZER v$Version

## ?? Gaming Performance Tweaker

**Ghost Optimizer** es una herramienta avanzada de optimización para gamers que aplica tweaks técnicos para mejorar el rendimiento en juegos competitivos.

## ?? INSTALACIÓN Y USO

### ? Requisitos del Sistema:
- Windows 10/11 (x64)
- Permisos de Administrador
- Procesador de 4+ cores (recomendado)

### ?? Instalación:
1. Descarga el archivo \`$AppName.exe\`
2. **Ejecuta como Administrador** (click derecho ? "Ejecutar como administrador")
3. ¡Listo! No requiere instalación adicional

### ?? IMPORTANTE:
- **SIEMPRE** ejecutar como Administrador
- Crear punto de restauración antes de aplicar tweaks
- Leer las descripciones de cada tweak antes de aplicar

## ?? CARACTERÍSTICAS PRINCIPALES:

### ?? **Red & Ping:**
- TCP/IP Optimization (Método AdamX)
- DNS Optimization (Cloudflare, Google)
- Advanced Network Tweaks (MTU, QoS, Auto-Tuning)
- Browser Optimization

### ?? **Sistema & GPU:**
- Game Mode Optimization
- GPU Hardware Scheduling
- System Responsiveness
- Power Management

### ?? **GHOST Pack:**
- MPO (Multiplane Overlay) Disable
- Ultimate Performance Power Plan
- HPET/Hyper-V Optimization
- Core Isolation (VBS) Control

### ?? **Input & Visuals:**
- Mouse Acceleration Disable
- Keyboard Optimization
- Visual Effects Optimization
- Memory Tweaks

### ?? **Limpieza:**
- Windows Bloatware Removal
- Service Optimization
- Disk Cleanup
- DNS Cache Management

### ?? **Advanced:**
- GPU IRQ Optimization
- Spectre/Meltdown Mitigations
- Network Adapter Settings
- Congestion Control

## ?? GUÍA DE USO:

### 1. **Primera vez:**
   - Crear punto de restauración
   - Aplicar tweaks básicos (Game Mode, DNS)
   - Reiniciar Windows

### 2. **Gaming Competitivo:**
   - Red & Ping: TCP/IP Optimization
   - GHOST Pack: MPO Disable, Ultimate Performance
   - Input: Mouse Acceleration Off
   - Advanced: GPU IRQ (si tienes 4+ cores)

### 3. **Navegadores lentos:**
   - Red & Ping: Browser Optimization
   - Esto balancea gaming vs navegación web

### 4. **Revertir cambios:**
   - Usar botones "OFF" rojos
   - O usar "Revertir Todo" en Dashboard
   - O restaurar punto de Windows

## ?? BENEFICIOS ESPERADOS:

- ? **Ping:** -5 a -30ms reducción
- ? **FPS:** +10-30% en juegos CPU/GPU bound
- ? **Input Lag:** -10-50ms reducción
- ? **Frame Times:** Más consistentes
- ? **Micro-stuttering:** Eliminado/reducido

## ?? PRECAUCIONES:

- ?? **Algunos tweaks son EXPERIMENTALES**
- ?? **Advanced tweaks pueden afectar seguridad**
- ?? **Hyper-V disable rompe Docker/WSL2**
- ?? **Spectre/Meltdown disable expone vulnerabilidades**

## ?? SOLUCIÓN DE PROBLEMAS:

### Problema: "App no inicia"
**Solución:** Ejecutar como Administrador

### Problema: "Navegadores lentos"
**Solución:** Red & Ping ? Browser Optimization

### Problema: "Juegos van peor"
**Solución:** Dashboard ? Revertir Todo, reiniciar

### Problema: "Sistema inestable"
**Solución:** Usar Punto de Restauración de Windows

## ?? SOPORTE:

- **Versión:** $Version
- **Autor:** DaddyGhost
- **GitHub:** https://github.com/Josemcboss/Tweaker

## ?? DISCLAIMER:

Esta herramienta modifica configuraciones avanzadas de Windows. Úsala bajo tu propio riesgo. Siempre crea backups antes de aplicar tweaks. El autor no se hace responsable por daños en el sistema.

---

## ?? INFORMACIÓN TÉCNICA:

- **Tipo:** Single-file self-contained executable
- **Runtime:** .NET $($dotnetVersion.Split('.')[0]).0
- **Arquitectura:** $Runtime
- **Compilado:** $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
- **Tamaño:** $FileSizeMB MB

¡Disfruta de tu experiencia gaming optimizada! ????
"@

$ReadmePath = Join-Path $PublishPath "README.md"
$ReadmeContent | Out-File -FilePath $ReadmePath -Encoding UTF8

Write-Host "   ? README.md creado"

# Crear archivo de información técnica
$InfoContent = @"
GHOST OPTIMIZER - Información Técnica
=====================================

Versión: $Version
Fecha de compilación: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
Configuración: $Configuration
Runtime: $Runtime
Tamaño del ejecutable: $FileSizeMB MB

.NET Runtime: $dotnetVersion
Tipo de deployment: Single-file self-contained
Trimming: Habilitado
Ready-to-Run: Habilitado
Compresión: Habilitada

Archivos incluidos:
$(Get-ChildItem $PublishPath -File | ForEach-Object { "  - $($_.Name) ($([math]::Round($_.Length / 1KB, 1)) KB)" } | Out-String)

Checksums:
$(Get-ChildItem $PublishPath -File | ForEach-Object { 
    $hash = Get-FileHash $_.FullName -Algorithm SHA256
    "  $($_.Name): $($hash.Hash.Substring(0,16))..."
} | Out-String)
"@

$InfoPath = Join-Path $PublishPath "BUILD_INFO.txt"
$InfoContent | Out-File -FilePath $InfoPath -Encoding UTF8

Write-Host "   ? BUILD_INFO.txt creado"

# ========================================================================================================
# 7. CREAR ARCHIVO ZIP PARA DISTRIBUCIÓN
# ========================================================================================================
Write-Host ""
Write-Host "?? CREANDO ARCHIVO ZIP..." -ForegroundColor Cyan
Write-Host "?????????????????????????" -ForegroundColor DarkCyan

$ZipPath = Join-Path $OutputDir "$AppName-$Version-$Runtime.zip"
if (Test-Path $ZipPath) {
    Remove-Item $ZipPath -Force
}

try {
    Compress-Archive -Path "$PublishPath\*" -DestinationPath $ZipPath -CompressionLevel Optimal
    $ZipInfo = Get-Item $ZipPath
    $ZipSizeMB = [math]::Round($ZipInfo.Length / 1MB, 2)
    Write-Host "   ? Archivo ZIP creado: $($ZipInfo.Name) ($ZipSizeMB MB)"
} catch {
    Write-Host "   ?? No se pudo crear el archivo ZIP: $($_.Exception.Message)" -ForegroundColor Yellow
}

# ========================================================================================================
# 8. RESUMEN FINAL
# ========================================================================================================
Write-Host ""
Write-Host "?? RESUMEN DE PUBLICACIÓN" -ForegroundColor Magenta
Write-Host "???????????????????????????" -ForegroundColor Magenta

Write-Host ""
Write-Host "? PUBLICACIÓN COMPLETADA EXITOSAMENTE:" -ForegroundColor Green
Write-Host "   • Ejecutable principal: $AppName.exe ($FileSizeMB MB)"
Write-Host "   • Tipo: Single-file self-contained"
Write-Host "   • Arquitectura: $Runtime"
Write-Host "   • Configuración: $Configuration"
Write-Host "   • Documentación: README.md incluida"
Write-Host "   • Información técnica: BUILD_INFO.txt incluida"
if (Test-Path $ZipPath) {
    Write-Host "   • Archivo de distribución: $(Split-Path $ZipPath -Leaf)"
}

Write-Host ""
Write-Host "?? ARCHIVOS GENERADOS:" -ForegroundColor Cyan
Write-Host "   ?? Directorio: $PublishPath"
Write-Host "   ?? Ejecutable: $AppName.exe"
Write-Host "   ?? Documentación: README.md"
Write-Host "   ?? Info técnica: BUILD_INFO.txt"
if (Test-Path $ZipPath) {
    Write-Host "   ?? Distribución: $(Split-Path $ZipPath -Leaf)"
}

Write-Host ""
Write-Host "?? PRÓXIMOS PASOS:" -ForegroundColor Yellow
Write-Host "   1. Probar el ejecutable en un sistema limpio"
Write-Host "   2. Verificar que funciona sin .NET instalado"
Write-Host "   3. Compartir el archivo ZIP o directamente el EXE"
Write-Host "   4. Instruir a los usuarios: 'Ejecutar como Administrador'"

Write-Host ""
Write-Host "?? GHOST OPTIMIZER LISTO PARA DISTRIBUCIÓN" -ForegroundColor Green
Write-Host "????????????????????????????????????????????" -ForegroundColor Green
Write-Host "Tu aplicación está compilada y lista para compartir!" -ForegroundColor White

# Abrir carpeta si se solicita
if ($OpenFolder) {
    Write-Host ""
    Write-Host "?? Abriendo directorio de publicación..." -ForegroundColor Cyan
    Start-Process explorer.exe -ArgumentList $PublishPath
}

Write-Host ""
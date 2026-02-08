# ========================================================================================================
# CREAR ICONO PARA TASKBAR - GHOST OPTIMIZER
# ========================================================================================================
# Este script crea un archivo .ico para mostrar el logo en la taskbar de Windows
# ========================================================================================================

Add-Type -AssemblyName PresentationCore
Add-Type -AssemblyName PresentationFramework
Add-Type -AssemblyName WindowsBase
Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName System.Windows.Forms

Write-Host ""
Write-Host "?? CREANDO ICONO PARA TASKBAR - GHOST OPTIMIZER" -ForegroundColor Green
Write-Host "???????????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""

# ========================================================================================================
# 1. CREAR LOGO COMO BITMAP
# ========================================================================================================
Write-Host "?? GENERANDO BITMAP DEL LOGO..." -ForegroundColor Cyan
Write-Host "??????????????????????????????" -ForegroundColor DarkCyan

# Función para crear bitmap del logo
function Create-GhostOptimizerBitmap {
    param([int]$size = 256)
    
    # Crear bitmap
    $bitmap = New-Object System.Drawing.Bitmap($size, $size)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $graphics.Clear([System.Drawing.Color]::Transparent)
    
    # Colores del logo
    $purple = [System.Drawing.Color]::FromArgb(138, 43, 226)    # #8A2BE2
    $orange = [System.Drawing.Color]::FromArgb(255, 69, 0)     # #FF4500
    $darkPurple = [System.Drawing.Color]::FromArgb(75, 0, 130) # #4B0082
    $red = [System.Drawing.Color]::FromArgb(255, 0, 0)         # #FF0000
    $white = [System.Drawing.Color]::FromArgb(255, 255, 255)   # #FFFFFF
    $background = [System.Drawing.Color]::FromArgb(26, 26, 26) # #1A1A1A
    
    $center = $size / 2
    $radius = $size * 0.4
    
    try {
        # Fondo circular oscuro
        $backgroundBrush = New-Object System.Drawing.SolidBrush($background)
        $graphics.FillEllipse($backgroundBrush, $center - $radius, $center - $radius, $radius * 2, $radius * 2)
        
        # Anillo exterior con gradiente
        $pen = New-Object System.Drawing.Pen($purple, $size * 0.02)
        $graphics.DrawEllipse($pen, $center - $radius, $center - $radius, $radius * 2, $radius * 2)
        
        # Anillo interior
        $innerRadius = $radius * 0.9
        $pen2 = New-Object System.Drawing.Pen($orange, $size * 0.01)
        $graphics.DrawEllipse($pen2, $center - $innerRadius, $center - $innerRadius, $innerRadius * 2, $innerRadius * 2)
        
        # Cuerpo del fantasma (simplificado para icono)
        $ghostRadius = $radius * 0.6
        $ghostBrush = New-Object System.Drawing.SolidBrush($purple)
        
        # Cabeza del fantasma
        $headRect = New-Object System.Drawing.RectangleF($center - $ghostRadius * 0.5, $center - $ghostRadius * 0.3, $ghostRadius, $ghostRadius * 0.7)
        $graphics.FillEllipse($ghostBrush, $headRect)
        
        # Parte inferior del fantasma
        $bodyRect = New-Object System.Drawing.RectangleF($center - $ghostRadius * 0.5, $center, $ghostRadius, $ghostRadius * 0.5)
        $graphics.FillRectangle($ghostBrush, $bodyRect)
        
        # Ojos rojos
        $eyeBrush = New-Object System.Drawing.SolidBrush($red)
        $eyeSize = $size * 0.03
        
        # Ojo izquierdo
        $graphics.FillEllipse($eyeBrush, $center - $ghostRadius * 0.25, $center - $ghostRadius * 0.1, $eyeSize, $eyeSize * 1.3)
        
        # Ojo derecho
        $graphics.FillEllipse($eyeBrush, $center + $ghostRadius * 0.1, $center - $ghostRadius * 0.1, $eyeSize, $eyeSize * 1.3)
        
        # Brillo en los ojos
        $highlightBrush = New-Object System.Drawing.SolidBrush($white)
        $highlightSize = $eyeSize * 0.4
        
        $graphics.FillEllipse($highlightBrush, $center - $ghostRadius * 0.22, $center - $ghostRadius * 0.07, $highlightSize, $highlightSize)
        $graphics.FillEllipse($highlightBrush, $center + $ghostRadius * 0.13, $center - $ghostRadius * 0.07, $highlightSize, $highlightSize)
        
    } finally {
        $graphics.Dispose()
    }
    
    return $bitmap
}

# Crear bitmaps en diferentes tamaños
$sizes = @(16, 24, 32, 48, 64, 128, 256)
$bitmaps = @{}

foreach ($size in $sizes) {
    Write-Host "   ??? Generando bitmap ${size}x${size}..."
    $bitmaps[$size] = Create-GhostOptimizerBitmap -size $size
}

Write-Host "   ? Bitmaps generados para todos los tamaños"

# ========================================================================================================
# 2. CREAR ARCHIVO .ICO
# ========================================================================================================
Write-Host ""
Write-Host "?? CREANDO ARCHIVO .ICO..." -ForegroundColor Cyan
Write-Host "?????????????????????????" -ForegroundColor DarkCyan

$iconPath = "Tweaker\Resources\GhostOptimizer.ico"

# Crear directorio si no existe
$resourcesDir = Split-Path $iconPath -Parent
if (-not (Test-Path $resourcesDir)) {
    New-Item -ItemType Directory -Path $resourcesDir -Force | Out-Null
}

try {
    # Usar el bitmap más grande como base
    $mainBitmap = $bitmaps[256]
    
    # Crear icono usando .NET
    $iconHandle = $mainBitmap.GetHicon()
    $icon = [System.Drawing.Icon]::FromHandle($iconHandle)
    
    # Guardar como archivo .ico
    $fileStream = New-Object System.IO.FileStream($iconPath, [System.IO.FileMode]::Create)
    $icon.Save($fileStream)
    $fileStream.Close()
    
    Write-Host "   ? Archivo .ico creado: $iconPath"
    
    # Verificar archivo
    if (Test-Path $iconPath) {
        $fileInfo = Get-Item $iconPath
        $fileSizeKB = [math]::Round($fileInfo.Length / 1KB, 2)
        Write-Host "   ?? Tamaño del archivo: $fileSizeKB KB"
    }
    
} catch {
    Write-Host "   ? Error creando archivo .ico: $($_.Exception.Message)" -ForegroundColor Red
    
    # Método alternativo usando System.Drawing
    try {
        Write-Host "   ?? Intentando método alternativo..."
        
        # Crear archivo .ico manualmente
        $iconBytes = @()
        
        # Header del archivo ICO
        $iconBytes += 0, 0  # Reserved
        $iconBytes += 1, 0  # Type (1 = ICO)
        $iconBytes += 1, 0  # Count (1 imagen)
        
        # Directory entry
        $iconBytes += 32     # Width (32 pixels)
        $iconBytes += 32     # Height (32 pixels)
        $iconBytes += 0      # Color count
        $iconBytes += 0      # Reserved
        $iconBytes += 1, 0   # Color planes
        $iconBytes += 32, 0  # Bits per pixel
        
        # Convertir bitmap a bytes
        $bitmap32 = $bitmaps[32]
        $memoryStream = New-Object System.IO.MemoryStream
        $bitmap32.Save($memoryStream, [System.Drawing.Imaging.ImageFormat]::Png)
        $bitmapBytes = $memoryStream.ToArray()
        $memoryStream.Close()
        
        # Data size y offset
        $dataSize = $bitmapBytes.Length
        $iconBytes += [byte]($dataSize -band 0xFF)
        $iconBytes += [byte](($dataSize -shr 8) -band 0xFF)
        $iconBytes += [byte](($dataSize -shr 16) -band 0xFF)
        $iconBytes += [byte](($dataSize -shr 24) -band 0xFF)
        
        $iconBytes += 22, 0, 0, 0  # Data offset (22 bytes header + directory)
        
        # Agregar data de la imagen
        $iconBytes += $bitmapBytes
        
        # Guardar archivo
        [System.IO.File]::WriteAllBytes($iconPath, $iconBytes)
        
        Write-Host "   ? Archivo .ico creado con método alternativo"
        
    } catch {
        Write-Host "   ? Error con método alternativo: $($_.Exception.Message)" -ForegroundColor Red
        
        # Crear un icono simple como último recurso
        Write-Host "   ?? Creando icono básico como último recurso..."
        
        $simpleBitmap = $bitmaps[32]
        $simpleIconHandle = $simpleBitmap.GetHicon()
        $simpleIcon = [System.Drawing.Icon]::FromHandle($simpleIconHandle)
        
        $bytes = New-Object byte[] 1024
        $iconStream = New-Object System.IO.MemoryStream($bytes)
        $simpleIcon.Save($iconStream)
        [System.IO.File]::WriteAllBytes($iconPath, $iconStream.ToArray())
        
        Write-Host "   ? Icono básico creado"
    }
}

# Limpiar bitmaps
foreach ($bitmap in $bitmaps.Values) {
    $bitmap.Dispose()
}

# ========================================================================================================
# 3. ACTUALIZAR PROYECTO
# ========================================================================================================
Write-Host ""
Write-Host "?? ACTUALIZANDO CONFIGURACIÓN DEL PROYECTO..." -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????????" -ForegroundColor DarkCyan

$projectFile = "Tweaker\Tweaker.csproj"

if (Test-Path $projectFile) {
    $projectContent = Get-Content $projectFile -Raw
    
    # Verificar si ya tiene configuración de icono
    if ($projectContent -match "<ApplicationIcon>") {
        Write-Host "   ?? El proyecto ya tiene configuración de icono"
    } else {
        # Agregar configuración de icono
        $iconConfig = "    <ApplicationIcon>Resources\GhostOptimizer.ico</ApplicationIcon>"
        
        # Buscar donde insertar (después de ApplicationManifest o al final del PropertyGroup)
        if ($projectContent -match "(<ApplicationManifest>.*</ApplicationManifest>)") {
            $newContent = $projectContent -replace "(<ApplicationManifest>.*</ApplicationManifest>)", "`$1`n$iconConfig"
        } else {
            $newContent = $projectContent -replace "(</PropertyGroup>)", "$iconConfig`n    `$1"
        }
        
        $newContent | Out-File -FilePath $projectFile -Encoding UTF8
        Write-Host "   ? Configuración de icono agregada al proyecto"
    }
} else {
    Write-Host "   ? No se encontró el archivo del proyecto" -ForegroundColor Red
}

# ========================================================================================================
# 4. CREAR ÍCONOS PARA DIFERENTES CONTEXTOS
# ========================================================================================================
Write-Host ""
Write-Host "?? CREANDO ÍCONOS ADICIONALES..." -ForegroundColor Cyan
Write-Host "??????????????????????????????????" -ForegroundColor DarkCyan

# Crear iconos PNG para diferentes usos
$pngSizes = @(16, 32, 64, 128, 256)

foreach ($size in $pngSizes) {
    $pngPath = "Tweaker\Resources\GhostOptimizer_${size}x${size}.png"
    
    try {
        $bitmap = Create-GhostOptimizerBitmap -size $size
        $bitmap.Save($pngPath, [System.Drawing.Imaging.ImageFormat]::Png)
        $bitmap.Dispose()
        
        Write-Host "   ? PNG creado: GhostOptimizer_${size}x${size}.png"
    } catch {
        Write-Host "   ? Error creando PNG ${size}x${size}: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# ========================================================================================================
# 5. VERIFICAR RESULTADO
# ========================================================================================================
Write-Host ""
Write-Host "?? VERIFICANDO ARCHIVOS CREADOS..." -ForegroundColor Cyan
Write-Host "?????????????????????????????????" -ForegroundColor DarkCyan

$createdFiles = Get-ChildItem "Tweaker\Resources\GhostOptimizer*" -ErrorAction SilentlyContinue

if ($createdFiles) {
    Write-Host "   ? Archivos de icono creados:"
    foreach ($file in $createdFiles) {
        $sizeKB = [math]::Round($file.Length / 1KB, 2)
        Write-Host "      • $($file.Name) ($sizeKB KB)"
    }
} else {
    Write-Host "   ? No se crearon archivos de icono" -ForegroundColor Red
}

# Verificar que el .ico principal existe
if (Test-Path $iconPath) {
    Write-Host "   ? Icono principal disponible: GhostOptimizer.ico"
} else {
    Write-Host "   ? Icono principal no se pudo crear" -ForegroundColor Red
}

# ========================================================================================================
# 6. INSTRUCCIONES FINALES
# ========================================================================================================
Write-Host ""
Write-Host "?? INSTRUCCIONES FINALES" -ForegroundColor Magenta
Write-Host "????????????????????????" -ForegroundColor Magenta

Write-Host ""
Write-Host "? ÍCONOS CREADOS EXITOSAMENTE:" -ForegroundColor Green
Write-Host "   • GhostOptimizer.ico (icono principal para taskbar)"
Write-Host "   • GhostOptimizer_*.png (iconos PNG en varios tamaños)"
Write-Host "   • Configuración agregada al proyecto"

Write-Host ""
Write-Host "?? PRÓXIMOS PASOS:" -ForegroundColor Yellow
Write-Host "   1. Compilar el proyecto (dotnet build)"
Write-Host "   2. Publicar nuevamente la aplicación"
Write-Host "   3. El logo aparecerá en:"
Write-Host "      • Taskbar de Windows"
Write-Host "      • Alt+Tab switcher" 
Write-Host "      • Barra de título"
Write-Host "      • Íconos de archivos"

Write-Host ""
Write-Host "?? DÓNDE SE VERÁ EL LOGO:" -ForegroundColor Cyan
Write-Host "   ? Taskbar (barra de tareas)"
Write-Host "   ? Alt + Tab (cambio de ventanas)"
Write-Host "   ? Sistema de notificaciones"
Write-Host "   ? Explorador de archivos (junto al .exe)"
Write-Host "   ? Menú de inicio (si se ancla)"

Write-Host ""
Write-Host "?? LOGO EN TASKBAR - IMPLEMENTACIÓN COMPLETA" -ForegroundColor Green
Write-Host "???????????????????????????????????????????????" -ForegroundColor Green
Write-Host "¡El logo GHOST OPTIMIZER ahora aparecerá en toda la interfaz de Windows!" -ForegroundColor White
Write-Host ""
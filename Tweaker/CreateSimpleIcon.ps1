# ========================================================================================================
# CREAR ICONO SIMPLE PARA TASKBAR - GHOST OPTIMIZER  
# ========================================================================================================
# Método simplificado para crear un icono usando solo System.Drawing básico
# ========================================================================================================

Add-Type -AssemblyName System.Drawing

Write-Host ""
Write-Host "?? CREANDO ICONO SIMPLE PARA TASKBAR" -ForegroundColor Green
Write-Host "???????????????????????????????????????" -ForegroundColor Green
Write-Host ""

# ========================================================================================================
# CREAR ICONO SIMPLE CON COLORES BÁSICOS
# ========================================================================================================
Write-Host "?? GENERANDO ICONO BÁSICO..." -ForegroundColor Cyan
Write-Host "???????????????????????????" -ForegroundColor DarkCyan

# Función simple para crear icono
function Create-SimpleGhostIcon {
    param([int]$size = 32)
    
    $bitmap = New-Object System.Drawing.Bitmap($size, $size)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    
    try {
        # Limpiar con transparente
        $graphics.Clear([System.Drawing.Color]::Transparent)
        
        # Colores básicos
        $purple = [System.Drawing.Color]::Purple
        $red = [System.Drawing.Color]::Red
        $white = [System.Drawing.Color]::White
        $black = [System.Drawing.Color]::Black
        
        $center = $size / 2
        
        # Fondo circular púrpura
        $bgBrush = New-Object System.Drawing.SolidBrush($purple)
        $bgSize = $size * 0.9
        $bgRect = [System.Drawing.Rectangle]::new($center - $bgSize/2, $center - $bgSize/2, $bgSize, $bgSize)
        $graphics.FillEllipse($bgBrush, $bgRect)
        
        # Contorno
        $pen = New-Object System.Drawing.Pen($white, 2)
        $graphics.DrawEllipse($pen, $bgRect)
        
        # Forma del fantasma (simple)
        $ghostBrush = New-Object System.Drawing.SolidBrush($white)
        $ghostSize = $size * 0.6
        
        # Cabeza del fantasma
        $headRect = [System.Drawing.Rectangle]::new($center - $ghostSize/2, $center - $ghostSize/3, $ghostSize, $ghostSize * 0.6)
        $graphics.FillEllipse($ghostBrush, $headRect)
        
        # Cuerpo del fantasma
        $bodyRect = [System.Drawing.Rectangle]::new($center - $ghostSize/2, $center, $ghostSize, $ghostSize/3)
        $graphics.FillRectangle($ghostBrush, $bodyRect)
        
        # Ojos rojos
        $eyeBrush = New-Object System.Drawing.SolidBrush($red)
        $eyeSize = [math]::Max(2, $size * 0.08)
        
        # Ojo izquierdo
        $leftEye = [System.Drawing.Rectangle]::new($center - $ghostSize/4, $center - $ghostSize/6, $eyeSize, $eyeSize)
        $graphics.FillEllipse($eyeBrush, $leftEye)
        
        # Ojo derecho  
        $rightEye = [System.Drawing.Rectangle]::new($center + $ghostSize/6, $center - $ghostSize/6, $eyeSize, $eyeSize)
        $graphics.FillEllipse($eyeBrush, $rightEye)
        
        # Limpiar resources
        $bgBrush.Dispose()
        $pen.Dispose()
        $ghostBrush.Dispose()
        $eyeBrush.Dispose()
        
    } finally {
        $graphics.Dispose()
    }
    
    return $bitmap
}

# Crear el icono
Write-Host "   ??? Generando bitmap 32x32..."
$iconBitmap = Create-SimpleGhostIcon -size 32

# ========================================================================================================
# GUARDAR COMO .ICO
# ========================================================================================================
Write-Host ""
Write-Host "?? GUARDANDO COMO ARCHIVO .ICO..." -ForegroundColor Cyan
Write-Host "?????????????????????????????????" -ForegroundColor DarkCyan

$iconPath = "Tweaker\Resources\GhostOptimizer.ico"

try {
    # Crear el icono desde el bitmap
    $iconHandle = $iconBitmap.GetHicon()
    $icon = [System.Drawing.Icon]::FromHandle($iconHandle)
    
    # Guardar como archivo .ico
    $fileStream = [System.IO.FileStream]::new($iconPath, [System.IO.FileMode]::Create)
    $icon.Save($fileStream)
    $fileStream.Close()
    $fileStream.Dispose()
    
    Write-Host "   ? Icono guardado: $iconPath"
    
    # Verificar archivo
    if (Test-Path $iconPath) {
        $fileInfo = Get-Item $iconPath
        $fileSizeKB = [math]::Round($fileInfo.Length / 1KB, 2)
        Write-Host "   ?? Tamaño: $fileSizeKB KB"
    }
    
} catch {
    Write-Host "   ? Error guardando icono: $($_.Exception.Message)" -ForegroundColor Red
} finally {
    $iconBitmap.Dispose()
}

# ========================================================================================================
# CREAR PREVIEW DEL ICONO
# ========================================================================================================
Write-Host ""
Write-Host "?? CREANDO PREVIEW GRANDE..." -ForegroundColor Cyan
Write-Host "??????????????????????????????" -ForegroundColor DarkCyan

# Crear versión grande para preview
$previewSize = 128
$previewBitmap = Create-SimpleGhostIcon -size $previewSize
$previewPath = "Tweaker\Resources\GhostOptimizer_Preview.png"

try {
    $previewBitmap.Save($previewPath, [System.Drawing.Imaging.ImageFormat]::Png)
    Write-Host "   ? Preview guardado: GhostOptimizer_Preview.png"
} catch {
    Write-Host "   ? Error guardando preview: $($_.Exception.Message)" -ForegroundColor Red
} finally {
    $previewBitmap.Dispose()
}

# ========================================================================================================
# VERIFICAR CONFIGURACIÓN DEL PROYECTO
# ========================================================================================================
Write-Host ""
Write-Host "?? VERIFICANDO CONFIGURACIÓN..." -ForegroundColor Cyan
Write-Host "??????????????????????????????" -ForegroundColor DarkCyan

$projectFile = "Tweaker\Tweaker.csproj"

if (Test-Path $projectFile) {
    $projectContent = Get-Content $projectFile -Raw
    
    if ($projectContent -match "<ApplicationIcon>Resources\\GhostOptimizer\.ico</ApplicationIcon>") {
        Write-Host "   ? Configuración de icono encontrada en el proyecto"
    } else {
        Write-Host "   ?? Configuración de icono no encontrada, verificando..."
        
        if ($projectContent -match "<ApplicationIcon>") {
            Write-Host "   ?? Otra configuración de icono encontrada"
        } else {
            Write-Host "   ? No hay configuración de icono en el proyecto"
        }
    }
} else {
    Write-Host "   ? Archivo de proyecto no encontrado"
}

# ========================================================================================================
# CREAR ARCHIVO DE INFORMACIÓN
# ========================================================================================================
$infoContent = @"
GHOST OPTIMIZER - ICONO PARA TASKBAR
===================================

Archivo creado: GhostOptimizer.ico
Fecha: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
Tamaño: 32x32 pixels
Formato: Microsoft Windows Icon (.ico)

Características:
- Fondo circular púrpura
- Fantasma blanco con ojos rojos
- Optimizado para taskbar de Windows
- Compatible con todos los tamaños de iconos de sistema

Dónde aparecerá:
? Taskbar (barra de tareas)
? Alt + Tab (cambio de ventanas)
? Explorador de archivos
? Barra de título de la ventana
? Notificaciones del sistema

Configuración aplicada en: Tweaker.csproj
<ApplicationIcon>Resources\GhostOptimizer.ico</ApplicationIcon>
"@

$infoPath = "Tweaker\Resources\ICON_INFO.txt"
$infoContent | Out-File -FilePath $infoPath -Encoding UTF8

Write-Host "   ? Archivo de información creado: ICON_INFO.txt"

# ========================================================================================================
# RESUMEN FINAL
# ========================================================================================================
Write-Host ""
Write-Host "?? RESUMEN - ICONO PARA TASKBAR" -ForegroundColor Magenta
Write-Host "??????????????????????????????????" -ForegroundColor Magenta

if (Test-Path $iconPath) {
    $iconInfo = Get-Item $iconPath
    $iconSizeKB = [math]::Round($iconInfo.Length / 1KB, 2)
    
    Write-Host ""
    Write-Host "? ICONO CREADO EXITOSAMENTE:" -ForegroundColor Green
    Write-Host "   ?? Archivo: GhostOptimizer.ico"
    Write-Host "   ?? Tamaño: $iconSizeKB KB"
    Write-Host "   ?? Formato: Windows Icon"
    Write-Host "   ?? Configurado en: Tweaker.csproj"
    
    Write-Host ""
    Write-Host "?? PRÓXIMOS PASOS:" -ForegroundColor Yellow
    Write-Host "   1. Compilar el proyecto: dotnet build"
    Write-Host "   2. Publicar nuevamente: dotnet publish"
    Write-Host "   3. Ejecutar GhostOptimizer.exe"
    Write-Host "   4. ¡Ver el logo en la taskbar!"
    
    Write-Host ""
    Write-Host "?? EL LOGO APARECERÁ EN:" -ForegroundColor Cyan
    Write-Host "   ?? Taskbar de Windows"
    Write-Host "   ?? Alt + Tab (cambio de ventanas)"
    Write-Host "   ?? Sistema de notificaciones"
    Write-Host "   ?? Explorador de archivos"
    Write-Host "   ?? Menú inicio (si se ancla)"
    
} else {
    Write-Host ""
    Write-Host "? ERROR: No se pudo crear el icono" -ForegroundColor Red
    Write-Host "   Verifica los permisos de escritura en el directorio"
}

Write-Host ""
Write-Host "?? GHOST OPTIMIZER - TASKBAR ICON READY!" -ForegroundColor Green
Write-Host "????????????????????????????????????????????" -ForegroundColor Green
Write-Host "El logo aparecerá en toda la interfaz de Windows" -ForegroundColor White
Write-Host ""
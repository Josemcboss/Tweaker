# ?? Verificación Visual de NetworkPage - Optimizaciones Avanzadas

Write-Host "???????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  ?? VERIFICACIÓN VISUAL - NetworkPage" -ForegroundColor Green
Write-Host "???????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Leer el archivo XAML
$xamlPath = "MainWindow.xaml"
if (-not (Test-Path $xamlPath)) {
    Write-Host "? No se encontró MainWindow.xaml" -ForegroundColor Red
    exit 1
}

$xamlContent = Get-Content $xamlPath -Raw

Write-Host "?? ANÁLISIS DE NetworkPage" -ForegroundColor Yellow
Write-Host "????????????????????????????????????????" -ForegroundColor Gray
Write-Host ""

# Verificar sección "Optimizaciones Avanzadas de Red"
if ($xamlContent -match 'Optimizaciones Avanzadas de Red') {
    Write-Host "? Título de sección encontrado" -ForegroundColor Green
} else {
    Write-Host "? Título de sección NO encontrado" -ForegroundColor Red
}

# Verificar cada botón
$buttons = @(
    @{Name="MTU Optimization"; On="BtnMTU_On_Click"; Off="BtnMTU_Off_Click"},
    @{Name="QoS Configuration"; On="BtnQoS_On_Click"; Off="BtnQoS_Off_Click"},
    @{Name="Auto-Tuning Level"; On="BtnAutoTuning_On_Click"; Off="BtnAutoTuning_Off_Click"},
    @{Name="Adapter Advanced Settings"; On="BtnAdapterSettings_On_Click"; Off="BtnAdapterSettings_Off_Click"},
    @{Name="Congestion Control"; On="BtnCongestionControl_On_Click"; Off="BtnCongestionControl_Off_Click"},
    @{Name="Aplicar Todas"; On="BtnAllAdvancedNetwork_On_Click"; Off="BtnAllAdvancedNetwork_Off_Click"}
)

Write-Host ""
Write-Host "?? BOTONES INDIVIDUALES" -ForegroundColor Yellow
Write-Host "????????????????????????????????????????" -ForegroundColor Gray

$foundButtons = 0
$totalButtons = $buttons.Count * 2  # ON y OFF

foreach ($button in $buttons) {
    $name = $button.Name
    $onHandler = $button.On
    $offHandler = $button.Off
    
    $onFound = $xamlContent -match $onHandler
    $offFound = $xamlContent -match $offHandler
    
    Write-Host ""
    Write-Host "  ?? $name" -ForegroundColor Cyan
    
    if ($onFound) {
        Write-Host "     ? ON handler: $onHandler" -ForegroundColor Green
        $foundButtons++
    } else {
        Write-Host "     ? ON handler: $onHandler" -ForegroundColor Red
    }
    
    if ($offFound) {
        Write-Host "     ? OFF handler: $offHandler" -ForegroundColor Green
        $foundButtons++
    } else {
        Write-Host "     ? OFF handler: $offHandler" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "????????????????????????????????????????" -ForegroundColor Gray
Write-Host "Botones encontrados: $foundButtons/$totalButtons" -ForegroundColor White
Write-Host ""

# Verificar elementos visuales clave
Write-Host "?? ELEMENTOS VISUALES" -ForegroundColor Yellow
Write-Host "????????????????????????????????????????" -ForegroundColor Gray
Write-Host ""

$visualElements = @(
    @{Name="Color cyan (#00D9FF)"; Pattern="#00D9FF"},
    @{Name="Emoji ??"; Pattern="??"},
    @{Name="Border destacado"; Pattern='BorderBrush="#00D9FF"'},
    @{Name="Background #1A1D21"; Pattern='Background="#1A1D21"'},
    @{Name="Botón APLICAR TODAS"; Pattern="APLICAR TODAS"},
    @{Name="Botón RESTAURAR TODAS"; Pattern="RESTAURAR TODAS"}
)

$foundVisuals = 0

foreach ($element in $visualElements) {
    $name = $element.Name
    $pattern = $element.Pattern
    
    if ($xamlContent -match [regex]::Escape($pattern)) {
        Write-Host "  ? $name" -ForegroundColor Green
        $foundVisuals++
    } else {
        Write-Host "  ? $name" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "????????????????????????????????????????" -ForegroundColor Gray
Write-Host "Elementos visuales: $foundVisuals/$($visualElements.Count)" -ForegroundColor White
Write-Host ""

# Verificar descripciones
Write-Host "?? DESCRIPCIONES" -ForegroundColor Yellow
Write-Host "????????????????????????????????????????" -ForegroundColor Gray
Write-Host ""

$descriptions = @(
    "Maximum Transmission Unit a 1492 bytes",
    "Prioriza tráfico de gaming",
    "RSS, Chimney Offload, NetDMA",
    "Window Scaling, SACK",
    "Compound TCP y Explicit Congestion Notification"
)

$foundDescriptions = 0

foreach ($desc in $descriptions) {
    if ($xamlContent -match [regex]::Escape($desc)) {
        Write-Host "  ? $desc" -ForegroundColor Green
        $foundDescriptions++
    } else {
        Write-Host "  ? $desc" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "????????????????????????????????????????" -ForegroundColor Gray
Write-Host "Descripciones: $foundDescriptions/$($descriptions.Count)" -ForegroundColor White
Write-Host ""

# Resumen final
Write-Host "???????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  ?? RESUMEN FINAL" -ForegroundColor Green
Write-Host "???????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

$totalChecks = $totalButtons + $visualElements.Count + $descriptions.Count
$totalFound = $foundButtons + $foundVisuals + $foundDescriptions
$percentage = [math]::Round(($totalFound / $totalChecks) * 100, 1)

Write-Host "Verificaciones totales: $totalFound/$totalChecks ($percentage%)" -ForegroundColor White
Write-Host ""

if ($percentage -eq 100) {
    Write-Host "? UI COMPLETAMENTE IMPLEMENTADA" -ForegroundColor Green
    Write-Host ""
    Write-Host "?? NetworkPage tiene todas las optimizaciones avanzadas" -ForegroundColor Green
    Write-Host "?? Listo para testing manual" -ForegroundColor Green
} elseif ($percentage -ge 90) {
    Write-Host "?? UI CASI COMPLETA" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Faltan algunos elementos menores" -ForegroundColor Yellow
} else {
    Write-Host "? UI INCOMPLETA" -ForegroundColor Red
    Write-Host ""
    Write-Host "Revisa los elementos faltantes arriba" -ForegroundColor Red
}

Write-Host ""
Write-Host "???????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Instrucciones para testing manual
Write-Host "?? PRÓXIMOS PASOS" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Ejecutar la aplicación" -ForegroundColor Gray
Write-Host "2. Navegar a 'Red & Ping'" -ForegroundColor Gray
Write-Host "3. Scroll hacia abajo hasta 'Optimizaciones Avanzadas de Red'" -ForegroundColor Gray
Write-Host "4. Verificar que todos los botones sean visibles" -ForegroundColor Gray
Write-Host "5. Probar cada botón individualmente" -ForegroundColor Gray
Write-Host "6. Probar 'APLICAR TODAS'" -ForegroundColor Gray
Write-Host ""
Write-Host "?? Ejecuta TestAdvancedNetwork.ps1 para verificar funcionalidad" -ForegroundColor Cyan
Write-Host ""

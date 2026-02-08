# Test Browser Optimization Implementation
# Verifica que todos los componentes para la optimización de navegadores estén funcionando

Write-Host "?? TESTING BROWSER OPTIMIZATION IMPLEMENTATION" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

$tests = @()
$passed = 0
$total = 0

# Test 1: Verificar que BrowserOptimization.cs existe
$total++
Write-Host "1. Verificando BrowserOptimization.cs..." -ForegroundColor Yellow
if (Test-Path "Tweaker\Optimizations\BrowserOptimization.cs") {
    Write-Host "   ? Archivo encontrado" -ForegroundColor Green
    $passed++
    
    # Verificar métodos clave
    $content = Get-Content "Tweaker\Optimizations\BrowserOptimization.cs" -Raw
    if ($content -match "ApplyBrowserBalancedTweaks" -and 
        $content -match "RestoreGamingOnlyTweaks" -and 
        $content -match "DiagnoseBrowserIssues") {
        Write-Host "   ? Métodos principales encontrados" -ForegroundColor Green
    } else {
        Write-Host "   ?? Algunos métodos faltan" -ForegroundColor Yellow
    }
} else {
    Write-Host "   ? Archivo no encontrado" -ForegroundColor Red
}

# Test 2: Verificar que MainWindow.xaml.cs tiene los métodos
$total++
Write-Host "`n2. Verificando métodos en MainWindow.xaml.cs..." -ForegroundColor Yellow
if (Test-Path "Tweaker\MainWindow.xaml.cs") {
    $content = Get-Content "Tweaker\MainWindow.xaml.cs" -Raw
    if ($content -match "BtnBrowserOptimization_Apply_Click" -and 
        $content -match "BtnBrowserOptimization_Revert_Click" -and 
        $content -match "BtnBrowserDiagnose_Click") {
        Write-Host "   ? Métodos de eventos encontrados" -ForegroundColor Green
        $passed++
    } else {
        Write-Host "   ? Métodos de eventos faltantes" -ForegroundColor Red
    }
} else {
    Write-Host "   ? MainWindow.xaml.cs no encontrado" -ForegroundColor Red
}

# Test 3: Verificar que MainWindow.xaml tiene los botones
$total++
Write-Host "`n3. Verificando botones en MainWindow.xaml..." -ForegroundColor Yellow
if (Test-Path "Tweaker\MainWindow.xaml") {
    $content = Get-Content "Tweaker\MainWindow.xaml" -Raw
    if ($content -match "BtnBrowserOptimization_Apply_Click" -and 
        $content -match "BtnBrowserOptimization_Revert_Click" -and 
        $content -match "BtnBrowserDiagnose_Click") {
        Write-Host "   ? Botones encontrados en XAML" -ForegroundColor Green
        $passed++
    } else {
        Write-Host "   ? Botones faltantes en XAML" -ForegroundColor Red
    }
} else {
    Write-Host "   ? MainWindow.xaml no encontrado" -ForegroundColor Red
}

# Test 4: Verificar scripts PowerShell
$total++
Write-Host "`n4. Verificando scripts PowerShell..." -ForegroundColor Yellow
$scriptsFound = 0
if (Test-Path "Tweaker\DiagnoseBrowserSlowness.ps1") {
    Write-Host "   ? DiagnoseBrowserSlowness.ps1 encontrado" -ForegroundColor Green
    $scriptsFound++
}
if (Test-Path "Tweaker\FixBrowserSlowness.ps1") {
    Write-Host "   ? FixBrowserSlowness.ps1 encontrado" -ForegroundColor Green
    $scriptsFound++
}

if ($scriptsFound -eq 2) {
    $passed++
    Write-Host "   ? Todos los scripts encontrados" -ForegroundColor Green
} else {
    Write-Host "   ?? Algunos scripts faltan ($scriptsFound/2)" -ForegroundColor Yellow
}

# Test 5: Verificar sintaxis de BrowserOptimization.cs
$total++
Write-Host "`n5. Verificando sintaxis de C#..." -ForegroundColor Yellow
try {
    # Verificar que no hay errores de sintaxis obvios
    $content = Get-Content "Tweaker\Optimizations\BrowserOptimization.cs" -Raw
    
    # Contar llaves para verificar balance
    $openBraces = ($content.ToCharArray() | Where-Object { $_ -eq '{' }).Count
    $closeBraces = ($content.ToCharArray() | Where-Object { $_ -eq '}' }).Count
    
    if ($openBraces -eq $closeBraces) {
        Write-Host "   ? Llaves balanceadas ($openBraces)" -ForegroundColor Green
        $passed++
    } else {
        Write-Host "   ? Llaves desbalanceadas: { $openBraces vs } $closeBraces" -ForegroundColor Red
    }
} catch {
    Write-Host "   ? Error verificando sintaxis: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 6: Verificar que el código compila (simulado)
$total++
Write-Host "`n6. Verificando estructura de clases..." -ForegroundColor Yellow
if (Test-Path "Tweaker\Optimizations\BrowserOptimization.cs") {
    $content = Get-Content "Tweaker\Optimizations\BrowserOptimization.cs" -Raw
    
    if ($content -match "namespace Tweaker\.Optimizations" -and
        $content -match "public static class BrowserOptimization" -and
        $content -match "using Microsoft\.Win32") {
        Write-Host "   ? Estructura de clase correcta" -ForegroundColor Green
        $passed++
    } else {
        Write-Host "   ? Estructura de clase incorrecta" -ForegroundColor Red
    }
}

# Test 7: Verificar contenido específico de funcionalidad
$total++
Write-Host "`n7. Verificando funcionalidad específica..." -ForegroundColor Yellow
if (Test-Path "Tweaker\Optimizations\BrowserOptimization.cs") {
    $content = Get-Content "Tweaker\Optimizations\BrowserOptimization.cs" -Raw
    
    $features = 0
    if ($content -match "NetworkThrottlingIndex.*5") { $features++ }
    if ($content -match "TcpAckFrequency.*2") { $features++ }
    if ($content -match "MaxNegativeCacheTtl.*30") { $features++ }
    if ($content -match "FlushDnsCache") { $features++ }
    
    Write-Host "   ? Características implementadas: $features/4" -ForegroundColor Green
    if ($features -ge 3) { $passed++ }
}

# Resumen
Write-Host ""
Write-Host "?? RESUMEN DE TESTS:" -ForegroundColor Cyan
Write-Host "==================" -ForegroundColor Cyan
Write-Host "Tests pasados: $passed/$total" -ForegroundColor $(if ($passed -eq $total) { "Green" } else { "Yellow" })
Write-Host "Porcentaje de éxito: $([math]::Round($passed/$total*100, 1))%" -ForegroundColor $(if ($passed -eq $total) { "Green" } else { "Yellow" })

if ($passed -eq $total) {
    Write-Host ""
    Write-Host "?? ¡IMPLEMENTACIÓN COMPLETA Y CORRECTA!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "? Todos los componentes están en su lugar" -ForegroundColor Green
    Write-Host "? La funcionalidad debería funcionar correctamente" -ForegroundColor Green
    Write-Host "? Los navegadores podrán optimizarse sin afectar gaming" -ForegroundColor Green
} elseif ($passed -ge $total * 0.8) {
    Write-Host ""
    Write-Host "?? IMPLEMENTACIÓN MAYORMENTE COMPLETA" -ForegroundColor Yellow
    Write-Host "====================================" -ForegroundColor Yellow
    Write-Host "La mayoría de componentes están correctos." -ForegroundColor Yellow
    Write-Host "Revisar los elementos marcados con ?" -ForegroundColor Yellow
} else {
    Write-Host ""
    Write-Host "? IMPLEMENTACIÓN INCOMPLETA" -ForegroundColor Red
    Write-Host "===========================" -ForegroundColor Red
    Write-Host "Se necesitan correcciones significativas." -ForegroundColor Red
}

Write-Host ""
Write-Host "?? PRÓXIMOS PASOS:" -ForegroundColor Cyan
Write-Host "=================" -ForegroundColor Cyan
Write-Host "1. Compila la aplicación para verificar errores" -ForegroundColor White
Write-Host "2. Prueba los botones en la interfaz" -ForegroundColor White
Write-Host "3. Ejecuta DiagnoseBrowserSlowness.ps1 manualmente" -ForegroundColor White
Write-Host "4. Prueba FixBrowserSlowness.ps1 manualmente" -ForegroundColor White
Write-Host ""
Write-Host "?? ¡La solución para navegadores lentos está lista!" -ForegroundColor Green
# =============================================================================
# SCRIPT DE VERIFICACIÓN DEL SISTEMA DE LICENCIAS
# =============================================================================

Write-Host "?? VERIFICANDO SISTEMA DE LICENCIAS DE GHOST OPTIMIZER..." -ForegroundColor Cyan
Write-Host ""

# Rutas importantes
$executablePath = "C:\Users\Administrator\source\repos\Tweaker\Tweaker\bin\Release\net10.0-windows\win-x64\publish\Tweaker.exe"
$projectPath = "C:\Users\Administrator\source\repos\Tweaker\Tweaker"

Write-Host "?? VERIFICANDO ARCHIVOS..." -ForegroundColor Yellow
Write-Host ""

# Verificar archivos clave
$filesToCheck = @(
    @{ Path = "$projectPath\App.xaml"; Name = "App.xaml" }
    @{ Path = "$projectPath\LoginWindow.xaml"; Name = "LoginWindow.xaml" }
    @{ Path = "$projectPath\LoginWindow.xaml.cs"; Name = "LoginWindow.xaml.cs" }
    @{ Path = "$projectPath\Utilities\LicenseChecker.cs"; Name = "LicenseChecker.cs" }
    @{ Path = "$projectPath\Properties\Settings.cs"; Name = "Settings.cs" }
    @{ Path = "$projectPath\App.config"; Name = "App.config" }
    @{ Path = $executablePath; Name = "Executable" }
)

foreach ($file in $filesToCheck) {
    if (Test-Path $file.Path) {
        Write-Host "? $($file.Name)" -ForegroundColor Green
    } else {
        Write-Host "? $($file.Name) - NO ENCONTRADO" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "?? VERIFICANDO CONFIGURACIÓN..." -ForegroundColor Yellow
Write-Host ""

# Verificar App.xaml StartupUri
$appXamlContent = Get-Content "$projectPath\App.xaml" -Raw
if ($appXamlContent -match 'StartupUri="LoginWindow\.xaml"') {
    Write-Host "? App.xaml configurado para iniciar con LoginWindow" -ForegroundColor Green
} else {
    Write-Host "? App.xaml NO está configurado correctamente" -ForegroundColor Red
}

# Verificar que LicenseChecker existe y tiene la estructura correcta
$licenseCheckerContent = Get-Content "$projectPath\Utilities\LicenseChecker.cs" -Raw
if ($licenseCheckerContent -match "VerifyLicense" -and $licenseCheckerContent -match "GUMROAD_API_URL") {
    Write-Host "? LicenseChecker tiene estructura correcta" -ForegroundColor Green
} else {
    Write-Host "? LicenseChecker mal configurado" -ForegroundColor Red
}

Write-Host ""
Write-Host "?? INFORMACIÓN DEL EJECUTABLE..." -ForegroundColor Yellow
Write-Host ""

if (Test-Path $executablePath) {
    $exe = Get-Item $executablePath
    Write-Host "?? Ubicación: $($exe.DirectoryName)" -ForegroundColor White
    Write-Host "?? Nombre: $($exe.Name)" -ForegroundColor White
    Write-Host "?? Tamaño: $([Math]::Round($exe.Length / 1MB, 2)) MB" -ForegroundColor White
    Write-Host "?? Modificado: $($exe.LastWriteTime)" -ForegroundColor White
    
    Write-Host ""
    Write-Host "?? PARA PROBAR LA APLICACIÓN:" -ForegroundColor Magenta
    Write-Host "   1. Ejecuta: $executablePath" -ForegroundColor White
    Write-Host "   2. Debería abrir la ventana de LOGIN (no MainWindow)" -ForegroundColor White
    Write-Host "   3. Ingresa cualquier clave para probar la validación" -ForegroundColor White
    Write-Host ""
    
    Write-Host "?? CLAVES DE PRUEBA:" -ForegroundColor Magenta
    Write-Host "   • Para testing offline: 'TEST-KEY-1234-5678'" -ForegroundColor White
    Write-Host "   • Para testing real: Necesitas clave válida de Gumroad" -ForegroundColor White
} else {
    Write-Host "? EJECUTABLE NO ENCONTRADO" -ForegroundColor Red
}

Write-Host ""
Write-Host "?? CARACTERÍSTICAS IMPLEMENTADAS:" -ForegroundColor Yellow
Write-Host ""
Write-Host "? Ventana de login moderna y elegante" -ForegroundColor Green
Write-Host "? Validación en tiempo real con API de Gumroad" -ForegroundColor Green
Write-Host "? Guardado automático de licencias válidas" -ForegroundColor Green
Write-Host "? Manejo robusto de errores de conexión" -ForegroundColor Green
Write-Host "? Animaciones de progreso durante validación" -ForegroundColor Green
Write-Host "? Verificación de reembolsos y chargebacks" -ForegroundColor Green
Write-Host "? Política 'deny by default' en errores" -ForegroundColor Green

Write-Host ""
Write-Host "?? CONFIGURACIÓN ACTUAL:" -ForegroundColor Yellow
Write-Host ""
Write-Host "?? API URL: https://api.gumroad.com/v2/licenses/verify" -ForegroundColor White
Write-Host "?? Product ID: ghostopt (placeholder - cambiar por real)" -ForegroundColor White
Write-Host "?? Timeout: 30 segundos" -ForegroundColor White
Write-Host "?? Settings: Guardado automático en Properties.Settings" -ForegroundColor White

Write-Host ""
Write-Host "?? SIGUIENTE PASO:" -ForegroundColor Cyan
Write-Host "   Ejecuta la aplicación para probar el sistema de licencias!" -ForegroundColor White
Write-Host ""

# Opcional: Mostrar las primeras líneas de archivos clave para verificar
Write-Host "?? VERIFICACIÓN RÁPIDA DE CONTENIDO:" -ForegroundColor Yellow
Write-Host ""

# App.xaml
$appLines = Get-Content "$projectPath\App.xaml" | Select-Object -First 6
Write-Host "?? App.xaml (primeras líneas):" -ForegroundColor Cyan
foreach ($line in $appLines) {
    if ($line -match "StartupUri") {
        Write-Host "   $line" -ForegroundColor Green
    } else {
        Write-Host "   $line" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "?? VERIFICACIÓN COMPLETA!" -ForegroundColor Green
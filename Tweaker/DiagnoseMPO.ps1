# ?? MPO Diagnostic Script
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  ?? DIAGNÓSTICO DE MPO" -ForegroundColor Green
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Verificar permisos de administrador
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "?? ADVERTENCIA: Este script necesita permisos de Administrador" -ForegroundColor Yellow
    Write-Host "   Ejecuta PowerShell como Administrador" -ForegroundColor Yellow
    Write-Host ""
}

# Verificar estado de MPO
Write-Host "?? 1. VERIFICANDO ESTADO DE MPO" -ForegroundColor Yellow
Write-Host "??????????????????????????????????????" -ForegroundColor Gray

$dwmPath = "HKLM:\SOFTWARE\Microsoft\Windows\Dwm"

try {
    $value = Get-ItemProperty -Path $dwmPath -Name "OverlayTestMode" -ErrorAction SilentlyContinue
    
    if ($value) {
        $mpoValue = $value.OverlayTestMode
        Write-Host "? Clave OverlayTestMode encontrada" -ForegroundColor Green
        Write-Host "   Ruta: $dwmPath" -ForegroundColor Gray
        Write-Host "   Valor: $mpoValue" -ForegroundColor Cyan
        
        if ($mpoValue -eq 5) {
            Write-Host ""
            Write-Host "? MPO ESTÁ DESHABILITADO CORRECTAMENTE" -ForegroundColor Green
            Write-Host "   Stuttering debería estar resuelto" -ForegroundColor Green
        } else {
            Write-Host ""
            Write-Host "?? MPO ESTÁ HABILITADO" -ForegroundColor Yellow
            Write-Host "   Valor actual: $mpoValue (debería ser 5)" -ForegroundColor Yellow
            Write-Host "   Stuttering puede ocurrir" -ForegroundColor Yellow
        }
    } else {
        Write-Host "? OverlayTestMode NO EXISTE" -ForegroundColor Red
        Write-Host "   MPO está habilitado por defecto" -ForegroundColor Red
        Write-Host ""
        Write-Host "?? Solución: Deshabilitar MPO desde la app" -ForegroundColor Cyan
    }
} catch {
    Write-Host "? Error al leer registro: $_" -ForegroundColor Red
}

Write-Host ""

# Verificar si se requiere reinicio
Write-Host "?? 2. VERIFICANDO NECESIDAD DE REINICIO" -ForegroundColor Yellow
Write-Host "??????????????????????????????????????" -ForegroundColor Gray

$lastReboot = (Get-CimInstance -ClassName Win32_OperatingSystem).LastBootUpTime
$timeSinceReboot = (Get-Date) - $lastReboot

Write-Host "Último reinicio: $lastReboot" -ForegroundColor Gray
Write-Host "Tiempo desde reinicio: $($timeSinceReboot.Days) días, $($timeSinceReboot.Hours) horas" -ForegroundColor Cyan

if ($timeSinceReboot.TotalHours -lt 1) {
    Write-Host "? Sistema reiniciado recientemente" -ForegroundColor Green
} else {
    Write-Host "?? Considera reiniciar si aplicaste cambios de MPO" -ForegroundColor Yellow
}

Write-Host ""

# Verificar procesos que pueden causar problemas con MPO
Write-Host "?? 3. VERIFICANDO PROCESOS PROBLEMÁTICOS" -ForegroundColor Yellow
Write-Host "??????????????????????????????????????" -ForegroundColor Gray

$problematicProcesses = @(
    "Discord",
    "obs64",
    "obs32",
    "MSIAfterburner",
    "RTSS",
    "RivaTuner",
    "GeForceExperience",
    "nvcontainer",
    "ShadowPlay"
)

$foundProblematic = @()
foreach ($proc in $problematicProcesses) {
    $running = Get-Process -Name $proc -ErrorAction SilentlyContinue
    if ($running) {
        $foundProblematic += $proc
        Write-Host "?? $proc está ejecutándose" -ForegroundColor Yellow
    }
}

if ($foundProblematic.Count -eq 0) {
    Write-Host "? No se encontraron procesos problemáticos" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "?? NOTA: Estos procesos pueden causar stuttering con MPO:" -ForegroundColor Cyan
    foreach ($proc in $foundProblematic) {
        Write-Host "   • $proc" -ForegroundColor Gray
    }
}

Write-Host ""

# Información de GPU
Write-Host "?? 4. INFORMACIÓN DE GPU" -ForegroundColor Yellow
Write-Host "??????????????????????????????????????" -ForegroundColor Gray

try {
    $gpu = Get-WmiObject Win32_VideoController | Select-Object -First 1
    Write-Host "GPU: $($gpu.Name)" -ForegroundColor Cyan
    Write-Host "Driver: $($gpu.DriverVersion)" -ForegroundColor Gray
    Write-Host "Resolución: $($gpu.CurrentHorizontalResolution)x$($gpu.CurrentVerticalResolution)" -ForegroundColor Gray
} catch {
    Write-Host "? No se pudo obtener información de GPU" -ForegroundColor Red
}

Write-Host ""

# Opciones de solución
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  ?? OPCIONES DE SOLUCIÓN" -ForegroundColor Green
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

if ($value -and $value.OverlayTestMode -eq 5) {
    Write-Host "? MPO ya está deshabilitado" -ForegroundColor Green
    Write-Host ""
    Write-Host "Si sigues teniendo problemas:" -ForegroundColor Yellow
    Write-Host "  1. Reinicia Windows" -ForegroundColor White
    Write-Host "  2. Cierra overlays (Discord, OBS, etc.)" -ForegroundColor White
    Write-Host "  3. Actualiza drivers de GPU" -ForegroundColor White
} else {
    Write-Host "Para deshabilitar MPO:" -ForegroundColor Yellow
    Write-Host "  1. Ejecutar aplicación Tweaker como Admin" -ForegroundColor White
    Write-Host "  2. Ir a 'GHOST Pack'" -ForegroundColor White
    Write-Host "  3. Click en botón 'ON' de MPO" -ForegroundColor White
    Write-Host "  4. REINICIAR Windows" -ForegroundColor White
    Write-Host ""
    Write-Host "O ejecutar manualmente:" -ForegroundColor Cyan
    Write-Host '  reg add "HKLM\SOFTWARE\Microsoft\Windows\Dwm" /v OverlayTestMode /t REG_DWORD /d 5 /f' -ForegroundColor Gray
}

Write-Host ""

# Opción de aplicar el fix manualmente
if ($isAdmin -and (-not $value -or $value.OverlayTestMode -ne 5)) {
    Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host "  ??? APLICAR FIX AHORA" -ForegroundColor Green
    Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host ""
    
    $apply = Read-Host "¿Aplicar fix de MPO ahora? (S/N)"
    
    if ($apply -eq "S" -or $apply -eq "s") {
        try {
            Write-Host ""
            Write-Host "Aplicando fix..." -ForegroundColor Yellow
            
            # Crear clave si no existe
            if (-not (Test-Path $dwmPath)) {
                New-Item -Path $dwmPath -Force | Out-Null
            }
            
            # Establecer valor
            Set-ItemProperty -Path $dwmPath -Name "OverlayTestMode" -Value 5 -Type DWord
            
            # Verificar
            $newValue = Get-ItemProperty -Path $dwmPath -Name "OverlayTestMode"
            
            if ($newValue.OverlayTestMode -eq 5) {
                Write-Host "? MPO DESHABILITADO EXITOSAMENTE" -ForegroundColor Green
                Write-Host ""
                Write-Host "?????? REINICIA WINDOWS AHORA ??????" -ForegroundColor Yellow
                Write-Host ""
                
                $reboot = Read-Host "¿Reiniciar ahora? (S/N)"
                if ($reboot -eq "S" -or $reboot -eq "s") {
                    Write-Host ""
                    Write-Host "Reiniciando en 10 segundos..." -ForegroundColor Yellow
                    Write-Host "Presiona Ctrl+C para cancelar" -ForegroundColor Gray
                    Start-Sleep -Seconds 10
                    Restart-Computer -Force
                }
            } else {
                Write-Host "? Error: El valor no se aplicó correctamente" -ForegroundColor Red
            }
        } catch {
            Write-Host "? Error al aplicar fix: $_" -ForegroundColor Red
        }
    }
}

Write-Host ""
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  ? DIAGNÓSTICO COMPLETADO" -ForegroundColor Green
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

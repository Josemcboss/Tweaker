# ???????????????????????????????????????????????????????????????????????????????
# GHOST OPTIMIZER - TESTING RÁPIDO (ANTES/DESPUÉS)
# ???????????????????????????????????????????????????????????????????????????????

Write-Host @"
?????????????????????????????????????????????????????????????????
?                 GHOST OPTIMIZER - TESTING RÁPIDO              ?
?????????????????????????????????????????????????????????????????
"@ -ForegroundColor Cyan

Write-Host "`n¿Qué deseas hacer?`n" -ForegroundColor Yellow
Write-Host "1. Guardar estado ANTES de aplicar tweaks" -ForegroundColor White
Write-Host "2. Guardar estado DESPUÉS de aplicar tweaks" -ForegroundColor White
Write-Host "3. Comparar ANTES vs DESPUÉS" -ForegroundColor White
Write-Host "4. Salir`n" -ForegroundColor White

$choice = Read-Host "Selecciona una opción (1-4)"

function Get-SystemState {
    $state = @{}
    
    Write-Host "`nRecopilando información del sistema..." -ForegroundColor Cyan
    
    # Red
    try {
        $state.NetworkThrottling = (Get-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile" -Name NetworkThrottlingIndex -ErrorAction Stop).NetworkThrottlingIndex
    } catch { $state.NetworkThrottling = "N/A" }
    
    # GPU
    try {
        $state.GpuPriority = (Get-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games" -Name "GPU Priority" -ErrorAction Stop).'GPU Priority'
    } catch { $state.GpuPriority = "N/A" }
    
    # GameDVR
    try {
        $state.GameDVR = (Get-ItemProperty -Path "HKCU:\System\GameConfigStore" -Name GameDVR_Enabled -ErrorAction Stop).GameDVR_Enabled
    } catch { $state.GameDVR = "N/A" }
    
    # Plan de Energía
    try {
        $powerPlan = powercfg /getactivescheme
        if ($powerPlan -match "High performance") {
            $state.PowerPlan = "High Performance"
        } elseif ($powerPlan -match "Ultimate") {
            $state.PowerPlan = "Ultimate Performance"
        } elseif ($powerPlan -match "Balanced") {
            $state.PowerPlan = "Balanced"
        } else {
            $state.PowerPlan = "Other"
        }
    } catch { $state.PowerPlan = "N/A" }
    
    # Servicios
    try {
        $state.SysMain = (Get-Service -Name "SysMain" -ErrorAction Stop).Status
    } catch { $state.SysMain = "N/A" }
    
    try {
        $state.DiagTrack = (Get-Service -Name "DiagTrack" -ErrorAction Stop).Status
    } catch { $state.DiagTrack = "N/A" }
    
    try {
        $state.WSearch = (Get-Service -Name "WSearch" -ErrorAction Stop).Status
    } catch { $state.WSearch = "N/A" }
    
    # Mouse
    try {
        $state.MouseSpeed = (Get-ItemProperty -Path "HKCU:\Control Panel\Mouse" -Name MouseSpeed -ErrorAction Stop).MouseSpeed
    } catch { $state.MouseSpeed = "N/A" }
    
    # Teclado
    try {
        $state.KeyboardDelay = (Get-ItemProperty -Path "HKCU:\Control Panel\Keyboard" -Name KeyboardDelay -ErrorAction Stop).KeyboardDelay
    } catch { $state.KeyboardDelay = "N/A" }
    
    # MPO
    try {
        $state.MPO = (Get-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows\Dwm" -Name OverlayTestMode -ErrorAction Stop).OverlayTestMode
    } catch { $state.MPO = "N/A" }
    
    # Core Isolation
    try {
        $state.CoreIsolation = (Get-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Control\DeviceGuard" -Name EnableVirtualizationBasedSecurity -ErrorAction Stop).EnableVirtualizationBasedSecurity
    } catch { $state.CoreIsolation = "N/A" }
    
    # RAM libre
    try {
        $os = Get-CimInstance -ClassName Win32_OperatingSystem
        $state.FreeRAM_GB = [math]::Round($os.FreePhysicalMemory / 1MB, 2)
    } catch { $state.FreeRAM_GB = "N/A" }
    
    # Timestamp
    $state.Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    
    Write-Host "? Información recopilada." -ForegroundColor Green
    
    return $state
}

function Save-State {
    param([string]$Filename)
    
    $state = Get-SystemState
    $state | ConvertTo-Json | Out-File $Filename
    
    Write-Host "`n? Estado guardado en: $Filename" -ForegroundColor Green
    Write-Host "`nResumen:" -ForegroundColor Cyan
    Write-Host "  NetworkThrottling: $($state.NetworkThrottling)" -ForegroundColor White
    Write-Host "  GPU Priority: $($state.GpuPriority)" -ForegroundColor White
    Write-Host "  GameDVR: $($state.GameDVR)" -ForegroundColor White
    Write-Host "  Power Plan: $($state.PowerPlan)" -ForegroundColor White
    Write-Host "  SysMain: $($state.SysMain)" -ForegroundColor White
    Write-Host "  Mouse Speed: $($state.MouseSpeed)" -ForegroundColor White
    Write-Host "  Free RAM: $($state.FreeRAM_GB) GB" -ForegroundColor White
}

function Compare-States {
    if (-not (Test-Path "state_before.json")) {
        Write-Host "`n? ERROR: No se encontró state_before.json" -ForegroundColor Red
        Write-Host "Ejecuta primero la opción 1 para guardar el estado ANTES." -ForegroundColor Yellow
        return
    }
    
    if (-not (Test-Path "state_after.json")) {
        Write-Host "`n? ERROR: No se encontró state_after.json" -ForegroundColor Red
        Write-Host "Ejecuta primero la opción 2 para guardar el estado DESPUÉS." -ForegroundColor Yellow
        return
    }
    
    $before = Get-Content "state_before.json" | ConvertFrom-Json
    $after = Get-Content "state_after.json" | ConvertFrom-Json
    
    Write-Host "`n???????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host " COMPARACIÓN: ANTES vs DESPUÉS" -ForegroundColor Cyan
    Write-Host "???????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    
    Write-Host "`nFecha ANTES: $($before.Timestamp)" -ForegroundColor White
    Write-Host "Fecha DESPUÉS: $($after.Timestamp)" -ForegroundColor White
    
    function Compare-Value {
        param($Name, $Before, $After, $OptimalValue = $null)
        
        Write-Host "`n$Name" -ForegroundColor Yellow
        Write-Host "  ANTES  : $Before" -ForegroundColor White
        Write-Host "  DESPUÉS: $After" -ForegroundColor White
        
        if ($Before -ne $After) {
            Write-Host "  CAMBIÓ: ?" -ForegroundColor Green
            
            if ($null -ne $OptimalValue) {
                if ($After -eq $OptimalValue) {
                    Write-Host "  VALOR ÓPTIMO ALCANZADO ?" -ForegroundColor Green
                } else {
                    Write-Host "  Valor óptimo esperado: $OptimalValue" -ForegroundColor Yellow
                }
            }
        } else {
            Write-Host "  SIN CAMBIOS" -ForegroundColor Gray
        }
    }
    
    Compare-Value "NetworkThrottlingIndex" $before.NetworkThrottling $after.NetworkThrottling -OptimalValue 4294967295
    Compare-Value "GPU Priority" $before.GpuPriority $after.GpuPriority -OptimalValue 8
    Compare-Value "GameDVR_Enabled" $before.GameDVR $after.GameDVR -OptimalValue 0
    Compare-Value "Power Plan" $before.PowerPlan $after.PowerPlan -OptimalValue "Ultimate Performance"
    Compare-Value "SysMain Service" $before.SysMain $after.SysMain -OptimalValue "Stopped"
    Compare-Value "DiagTrack Service" $before.DiagTrack $after.DiagTrack -OptimalValue "Stopped"
    Compare-Value "Windows Search" $before.WSearch $after.WSearch -OptimalValue "Stopped"
    Compare-Value "Mouse Speed" $before.MouseSpeed $after.MouseSpeed -OptimalValue "0"
    Compare-Value "Keyboard Delay" $before.KeyboardDelay $after.KeyboardDelay -OptimalValue "0"
    Compare-Value "MPO (Overlay)" $before.MPO $after.MPO -OptimalValue 5
    Compare-Value "Core Isolation" $before.CoreIsolation $after.CoreIsolation -OptimalValue 0
    Compare-Value "Free RAM (GB)" $before.FreeRAM_GB $after.FreeRAM_GB
    
    Write-Host "`n???????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host " FIN DE COMPARACIÓN" -ForegroundColor Cyan
    Write-Host "???????????????????????????????????????????????????????????????`n" -ForegroundColor Cyan
}

switch ($choice) {
    "1" {
        Write-Host "`n?? Guardando estado ANTES de aplicar tweaks..." -ForegroundColor Cyan
        Save-State "state_before.json"
        Write-Host "`n?? Ahora aplica los tweaks en GHOST Optimizer y luego ejecuta la opción 2." -ForegroundColor Yellow
    }
    "2" {
        Write-Host "`n?? Guardando estado DESPUÉS de aplicar tweaks..." -ForegroundColor Cyan
        Save-State "state_after.json"
        Write-Host "`n?? Ahora ejecuta la opción 3 para ver la comparación." -ForegroundColor Yellow
    }
    "3" {
        Compare-States
    }
    "4" {
        Write-Host "`n?? Hasta pronto!" -ForegroundColor Cyan
        exit
    }
    default {
        Write-Host "`n? Opción inválida." -ForegroundColor Red
    }
}

Write-Host "`nPresiona cualquier tecla para salir..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

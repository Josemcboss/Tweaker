# ???????????????????????????????????????????????????????????????????
# SCRIPT: REEMPLAZAR BOTONES ON/OFF POR MODERNTOGGLESWITCH
# Ghost Optimizer v2.4.0
# ???????????????????????????????????????????????????????????????????

$xamlFile = "Tweaker\MainWindow.xaml"
$backupFile = "Tweaker\MainWindow.xaml.backup"

Write-Host "???????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  MODERNTOGGLESWITCH - REEMPLAZO GLOBAL" -ForegroundColor Yellow
Write-Host "???????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Crear backup
Write-Host "[1/3] Creando backup..." -ForegroundColor Green
Copy-Item $xamlFile $backupFile -Force
Write-Host "      ? Backup creado: $backupFile" -ForegroundColor Gray
Write-Host ""

# Leer archivo
$content = Get-Content $xamlFile -Raw

# Contar botones ON/OFF actuales
$onButtons = ([regex]::Matches($content, 'Content="ON".*?Style="\{StaticResource OnButton\}"')).Count
$offButtons = ([regex]::Matches($content, 'Content="OFF".*?Style="\{StaticResource OffButton\}"')).Count

Write-Host "[2/3] Análisis del archivo:" -ForegroundColor Green
Write-Host "      • Botones ON encontrados: $onButtons" -ForegroundColor Gray
Write-Host "      • Botones OFF encontrados: $offButtons" -ForegroundColor Gray
Write-Host "      • Pares de botones: $($onButtons)" -ForegroundColor Yellow
Write-Host ""

# Lista de tweaks a actualizar con sus eventos
$tweaksToUpdate = @(
    # RED & PING
    @{
        Name = "NetworkOptimization"
        OnEvent = "BtnNetworkOptimization_On_Click"
        OffEvent = "BtnNetworkOptimization_Off_Click"
        Section = "Red & Ping"
    },
    @{
        Name = "DnsCache"
        OnEvent = "BtnDnsCache_On_Click"
        OffEvent = "BtnDnsCache_Off_Click"
        Section = "Red & Ping"
    },
    @{
        Name = "NetworkPower"
        OnEvent = "BtnNetworkPower_On_Click"
        OffEvent = "BtnNetworkPower_Off_Click"
        Section = "Red & Ping"
    },
    @{
        Name = "NetBios"
        OnEvent = "BtnNetBios_On_Click"
        OffEvent = "BtnNetBios_Off_Click"
        Section = "Red & Ping"
    },
    @{
        Name = "MTU"
        OnEvent = "BtnMTU_On_Click"
        OffEvent = "BtnMTU_Off_Click"
        Section = "Red & Ping"
    },
    @{
        Name = "QoS"
        OnEvent = "BtnQoS_On_Click"
        OffEvent = "BtnQoS_Off_Click"
        Section = "Red & Ping"
    },
    @{
        Name = "AutoTuning"
        OnEvent = "BtnAutoTuning_On_Click"
        OffEvent = "BtnAutoTuning_Off_Click"
        Section = "Red & Ping"
    },
    @{
        Name = "AdapterSettings"
        OnEvent = "BtnAdapterSettings_On_Click"
        OffEvent = "BtnAdapterSettings_Off_Click"
        Section = "Red & Ping"
    },
    @{
        Name = "CongestionControl"
        OnEvent = "BtnCongestionControl_On_Click"
        OffEvent = "BtnCongestionControl_Off_Click"
        Section = "Red & Ping"
    },
    
    # SISTEMA & GPU
    @{
        Name = "SystemProfile"
        OnEvent = "BtnSystemProfile_On_Click"
        OffEvent = "BtnSystemProfile_Off_Click"
        Section = "Sistema & GPU"
    },
    @{
        Name = "GameDVR"
        OnEvent = "BtnGameDVR_On_Click"
        OffEvent = "BtnGameDVR_Off_Click"
        Section = "Sistema & GPU"
    },
    @{
        Name = "GpuScheduling"
        OnEvent = "BtnGpuScheduling_On_Click"
        OffEvent = "BtnGpuScheduling_Off_Click"
        Section = "Sistema & GPU"
    },
    @{
        Name = "SystemResponsiveness"
        OnEvent = "BtnSystemResponsiveness_On_Click"
        OffEvent = "BtnSystemResponsiveness_Off_Click"
        Section = "Sistema & GPU"
    },
    @{
        Name = "HighPerformance"
        OnEvent = "BtnHighPerformance_On_Click"
        OffEvent = "BtnHighPerformance_Off_Click"
        Section = "Sistema & GPU"
    },
    @{
        Name = "PowerThrottling"
        OnEvent = "BtnPowerThrottling_On_Click"
        OffEvent = "BtnPowerThrottling_Off_Click"
        Section = "Sistema & GPU"
    },
    @{
        Name = "CoreParking"
        OnEvent = "BtnCoreParking_On_Click"
        OffEvent = "BtnCoreParking_Off_Click"
        Section = "Sistema & GPU"
    },
    @{
        Name = "GameMode"
        OnEvent = "BtnGameMode_On_Click"
        OffEvent = "BtnGameMode_Off_Click"
        Section = "Sistema & GPU"
    },
    @{
        Name = "NTFSLastAccess"
        OnEvent = "BtnNTFSLastAccess_On_Click"
        OffEvent = "BtnNTFSLastAccess_Off_Click"
        Section = "Sistema & GPU"
    },
    @{
        Name = "GamePriority"
        OnEvent = "BtnGamePriority_On_Click"
        OffEvent = "BtnGamePriority_Off_Click"
        Section = "Sistema & GPU"
    },
    
    # LIMPIEZA
    @{
        Name = "Hibernation"
        OnEvent = "BtnHibernation_On_Click"
        OffEvent = "BtnHibernation_Off_Click"
        Section = "Limpieza"
    },
    @{
        Name = "WindowsSearch"
        OnEvent = "BtnWindowsSearch_On_Click"
        OffEvent = "BtnWindowsSearch_Off_Click"
        Section = "Limpieza"
    },
    @{
        Name = "SysMainService"
        OnEvent = "BtnSysMainService_On_Click"
        OffEvent = "BtnSysMainService_Off_Click"
        Section = "Limpieza"
    },
    
    # GHOST PACK
    @{
        Name = "UltimatePower"
        OnEvent = "BtnUltimatePower_On_Click"
        OffEvent = "BtnUltimatePower_Off_Click"
        Section = "GHOST Pack"
    },
    @{
        Name = "GameBar"
        OnEvent = "BtnGameBar_On_Click"
        OffEvent = "BtnGameBar_Off_Click"
        Section = "GHOST Pack"
    },
    @{
        Name = "CoreIsolation"
        OnEvent = "BtnCoreIsolation_On_Click"
        OffEvent = "BtnCoreIsolation_Off_Click"
        Section = "GHOST Pack"
    },
    @{
        Name = "HPET"
        OnEvent = "BtnHPET_On_Click"
        OffEvent = "BtnHPET_Off_Click"
        Section = "GHOST Pack"
    },
    @{
        Name = "MPOFix"
        OnEvent = "BtnMPOFix_On_Click"
        OffEvent = "BtnMPOFix_Off_Click"
        Section = "GHOST Pack"
    },
    @{
        Name = "HyperV"
        OnEvent = "BtnHyperV_On_Click"
        OffEvent = "BtnHyperV_Off_Click"
        Section = "GHOST Pack"
    },
    
    # ADVANCED
    @{
        Name = "SpectreMeltdown"
        OnEvent = "BtnSpectreMeltdown_On_Click"
        OffEvent = "BtnSpectreMeltdown_Off_Click"
        Section = "Advanced"
    },
    @{
        Name = "GpuIRQ"
        OnEvent = "BtnGpuIRQ_On_Click"
        OffEvent = "BtnGpuIRQ_Off_Click"
        Section = "Advanced"
    },
    @{
        Name = "OptimizeUSB"
        OnEvent = "BtnOptimizeUSB_On_Click"
        OffEvent = "BtnOptimizeUSB_Off_Click"
        Section = "Advanced"
    },
    @{
        Name = "OptimizeInputQueues"
        OnEvent = "BtnOptimizeInputQueues_On_Click"
        OffEvent = "BtnOptimizeInputQueues_Off_Click"
        Section = "Advanced"
    },
    @{
        Name = "DisableFTH"
        OnEvent = "BtnDisableFTH_On_Click"
        OffEvent = "BtnDisableFTH_Off_Click"
        Section = "Advanced"
    },
    
    # INPUT & VISUALS (faltantes)
    @{
        Name = "Memory"
        OnEvent = "BtnMemory_On_Click"
        OffEvent = "BtnMemory_Off_Click"
        Section = "Input & Visuals"
    },
    @{
        Name = "Transparency"
        OnEvent = "BtnTransparency_On_Click"
        OffEvent = "BtnTransparency_Off_Click"
        Section = "Input & Visuals"
    },
    @{
        Name = "StickyKeys"
        OnEvent = "BtnStickyKeys_On_Click"
        OffEvent = "BtnStickyKeys_Off_Click"
        Section = "Input & Visuals"
    }
)

Write-Host "[3/3] Generando reporte de tweaks..." -ForegroundColor Green
Write-Host ""
Write-Host "???????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  TWEAKS A ACTUALIZAR: $($tweaksToUpdate.Count)" -ForegroundColor Yellow
Write-Host "???????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Agrupar por sección
$sections = $tweaksToUpdate | Group-Object Section

foreach ($section in $sections) {
    Write-Host "  ?? $($section.Name): $($section.Count) tweaks" -ForegroundColor Cyan
    foreach ($tweak in $section.Group) {
        Write-Host "     • $($tweak.Name)" -ForegroundColor Gray
    }
    Write-Host ""
}

Write-Host "???????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""
Write-Host "?? IMPORTANTE:" -ForegroundColor Yellow
Write-Host "   Este script solo genera el análisis." -ForegroundColor Gray
Write-Host "   El reemplazo se hará manualmente por seguridad." -ForegroundColor Gray
Write-Host ""
Write-Host "? Backup creado en: $backupFile" -ForegroundColor Green
Write-Host ""

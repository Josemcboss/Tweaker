# Script para agregar indicadores visuales a TODOS los botones faltantes
# Este script genera los reemplazos necesarios para MainWindow.xaml.cs

Write-Host "?? Generando lista de botones para actualizar..." -ForegroundColor Cyan

# Lista de todos los botones que faltan por actualizar
$buttonsToUpdate = @(
    # Windows Bloatware (faltantes)
    @{Name="BtnSysMain"; Text="SYSMAIN"; Tweak="SysMain"},
    @{Name="BtnTelemetry"; Text="TELEMETRÍA"; Tweak="Telemetry"},
    
    # Input & Visuals (faltantes)
    @{Name="BtnMemory"; Text="RAM"; Tweak="MemoryOptimization"},
    @{Name="BtnTransparency"; Text="TRANSPARENCIA"; Tweak="TransparencyEffects"},
    
    # GHOST Pack
    @{Name="BtnUltimatePower"; Text="ULTIMATE"; Tweak="UltimatePower"},
    @{Name="BtnGameBar"; Text="GAME BAR"; Tweak="GameBar"},
    @{Name="BtnCoreIsolation"; Text="CORE ISOLATION"; Tweak="CoreIsolation"},
    @{Name="BtnHPET"; Text="HPET"; Tweak="HPET"},
    @{Name="BtnHyperV"; Text="HYPER-V"; Tweak="HyperV"},
    @{Name="BtnMPOFix"; Text="MPO"; Tweak="MPOFix"},
    
    # Advanced
    @{Name="BtnSpectreMeltdown"; Text="MITIGACIONES"; Tweak="SpectreMeltdown"},
    @{Name="BtnGpuIRQ"; Text="GPU IRQ"; Tweak="GpuIRQ"},
    @{Name="BtnOptimizeUSB"; Text="USB"; Tweak="USBOptimization"},
    @{Name="BtnOptimizeInputQueues"; Text="COLAS"; Tweak="InputQueues"},
    @{Name="BtnDisableFTH"; Text="FTH"; Tweak="FaultTolerantHeap"},
    
    # Advanced Network
    @{Name="BtnMTU"; Text="MTU"; Tweak="MTUOptimization"},
    @{Name="BtnQoS"; Text="QOS"; Tweak="QoSConfiguration"},
    @{Name="BtnAutoTuning"; Text="AUTO-TUNING"; Tweak="AutoTuningLevel"},
    @{Name="BtnAdapterSettings"; Text="ADAPTADOR"; Tweak="AdapterSettings"},
    @{Name="BtnCongestionControl"; Text="CONGESTION"; Tweak="CongestionControl"},
    
    # Game Mode
    @{Name="BtnGameMode"; Text="GAME MODE"; Tweak="WindowsGameMode"},
    @{Name="BtnNTFSLastAccess"; Text="LAST ACCESS"; Tweak="NTFSLastAccessTime"},
    @{Name="BtnGamePriority"; Text="PRIORIDAD"; Tweak="GameProcessPriority"}
)

Write-Host "? Total de botones a actualizar: $($buttonsToUpdate.Count)" -ForegroundColor Green
Write-Host "`nGenerando código para cada botón...`n" -ForegroundColor Yellow

foreach ($button in $buttonsToUpdate) {
    Write-Host "?? $($button.Name)_On_Click ? UpdateButtonState((Button)sender, true);" -ForegroundColor White
    Write-Host "?? $($button.Name)_Off_Click ? UpdateRelatedOnButton(`"$($button.Text)`", false);" -ForegroundColor Gray
    Write-Host ""
}

Write-Host "`n?? PATRÓN A SEGUIR:" -ForegroundColor Cyan
Write-Host @"

// BOTÓN ON:
private void Btn{NAME}_On_Click(object sender, RoutedEventArgs e)
{
    _tweakHelper.ExecuteTweak(...);
    UpdateButtonState((Button)sender, true); // ? AGREGAR ESTA LÍNEA
}

// BOTÓN OFF:
private void Btn{NAME}_Off_Click(object sender, RoutedEventArgs e)
{
    _tweakHelper.ExecuteTweakRevert(...);
    UpdateRelatedOnButton("{TEXT}", false); // ? AGREGAR ESTA LÍNEA
}

"@ -ForegroundColor White

Write-Host "? Script completado!" -ForegroundColor Green

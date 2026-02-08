# Fix Browser Slowness - DaddyGhost Tweaker
# Este script ajusta los tweaks que causan lentitud en navegadores
# Mantiene beneficios para gaming pero optimiza para navegación web

param(
    [switch]$Force = $false
)

Write-Host "?? REPARAR LENTITUD EN NAVEGADOR" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

if (-not $Force) {
    Write-Host "?? Este script ajustará tweaks de red para mejorar navegadores" -ForegroundColor Yellow
    Write-Host "Se mantendrán beneficios para gaming pero con mejor balance" -ForegroundColor Yellow
    Write-Host ""
    $confirm = Read-Host "¿Continuar? (y/n)"
    if ($confirm -ne "y" -and $confirm -ne "Y") {
        Write-Host "? Operación cancelada" -ForegroundColor Red
        exit
    }
}

$fixes = @()

Write-Host "??? Aplicando correcciones..." -ForegroundColor Green
Write-Host ""

# 1. Ajustar NetworkThrottlingIndex a valor balanceado (no completamente deshabilitado)
try {
    $regPath = "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile"
    $currentValue = Get-ItemProperty -Path $regPath -Name "NetworkThrottlingIndex" -ErrorAction SilentlyContinue
    
    if ($currentValue.NetworkThrottlingIndex -eq -1 -or $currentValue.NetworkThrottlingIndex -eq [uint32]0xFFFFFFFF) {
        # Cambiar a valor balanceado (5 = prioridad alta pero no extrema)
        Set-ItemProperty -Path $regPath -Name "NetworkThrottlingIndex" -Value 5
        Write-Host "? NetworkThrottlingIndex ajustado de FFFFFFFF a 5 (balanceado)" -ForegroundColor Green
        $fixes += "NetworkThrottlingIndex balanceado para navegadores"
    } else {
        Write-Host "? NetworkThrottlingIndex ya está en valor aceptable ($($currentValue.NetworkThrottlingIndex))" -ForegroundColor Green
    }
} catch {
    Write-Host "? Error ajustando NetworkThrottlingIndex: $($_.Exception.Message)" -ForegroundColor Red
}

# 2. Ajustar TCP settings en interfaces activas
$interfacesFixed = 0
$tcpInterfaces = Get-ChildItem "HKLM:\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces"

foreach ($interface in $tcpInterfaces) {
    try {
        $props = Get-ItemProperty -Path $interface.PSPath -ErrorAction SilentlyContinue
        
        # Solo interfaces activas
        if ($props.DhcpIPAddress -or ($props.IPAddress -and $props.IPAddress -ne "0.0.0.0")) {
            $interfaceId = $interface.PSChildName.Substring(0, 8) + "..."
            $needsFix = $false
            
            # Ajustar TcpAckFrequency de 1 a 2 (menos agresivo)
            if ($props.TcpAckFrequency -eq 1) {
                Set-ItemProperty -Path $interface.PSPath -Name "TcpAckFrequency" -Value 2
                Write-Host "? Interface $interfaceId - TcpAckFrequency: 1 ? 2 (menos agresivo)" -ForegroundColor Green
                $needsFix = $true
            }
            
            # Restaurar TcpDelAckTicks a valor más conservador
            if ($props.TcpDelAckTicks -eq 0) {
                Set-ItemProperty -Path $interface.PSPath -Name "TcpDelAckTicks" -Value 1
                Write-Host "? Interface $interfaceId - TcpDelAckTicks: 0 ? 1 (reduce overhead)" -ForegroundColor Green
                $needsFix = $true
            }
            
            # Mantener TCPNoDelay=1 (Nagle deshabilitado) para gaming
            # pero agregar configuración de buffer más grande para navegadores
            if ($props.TCPNoDelay -eq 1) {
                # Agregar TcpWindowSize para mejor throughput en navegadores
                if (-not $props.TcpWindowSize) {
                    Set-ItemProperty -Path $interface.PSPath -Name "TcpWindowSize" -Value 65536
                    Write-Host "? Interface $interfaceId - TcpWindowSize agregado (65536 bytes)" -ForegroundColor Green
                    $needsFix = $true
                }
            }
            
            if ($needsFix) {
                $interfacesFixed++
            }
        }
    } catch {
        Write-Host "?? Error procesando interface: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

if ($interfacesFixed -gt 0) {
    $fixes += "$interfacesFixed interfaces TCP optimizadas para navegadores"
    Write-Host "? $interfacesFixed interfaces de red ajustadas" -ForegroundColor Green
} else {
    Write-Host "?? Las interfaces TCP ya estaban bien configuradas" -ForegroundColor Blue
}

# 3. Restaurar DNS Cache settings para navegadores
try {
    $dnsRegPath = "HKLM:\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters"
    $dnsFixed = $false
    
    # Habilitar cache negativo (pero corto para navegadores)
    $negativeCache = Get-ItemProperty -Path $dnsRegPath -Name "MaxNegativeCacheTtl" -ErrorAction SilentlyContinue
    if ($negativeCache.MaxNegativeCacheTtl -eq 0) {
        Set-ItemProperty -Path $dnsRegPath -Name "MaxNegativeCacheTtl" -Value 30  # 30 segundos
        Write-Host "? DNS MaxNegativeCacheTtl: 0 ? 30 segundos" -ForegroundColor Green
        $dnsFixed = $true
    }
    
    # Restaurar NetFailureCacheTime
    $failureCache = Get-ItemProperty -Path $dnsRegPath -Name "NetFailureCacheTime" -ErrorAction SilentlyContinue
    if ($failureCache.NetFailureCacheTime -eq 0) {
        Set-ItemProperty -Path $dnsRegPath -Name "NetFailureCacheTime" -Value 10  # 10 segundos
        Write-Host "? DNS NetFailureCacheTime: 0 ? 10 segundos" -ForegroundColor Green
        $dnsFixed = $true
    }
    
    # Mantener cache positivo alto (bueno para navegadores)
    $positiveCache = Get-ItemProperty -Path $dnsRegPath -Name "MaxCacheTtl" -ErrorAction SilentlyContinue
    if (-not $positiveCache.MaxCacheTtl -or $positiveCache.MaxCacheTtl -lt 3600) {
        Set-ItemProperty -Path $dnsRegPath -Name "MaxCacheTtl" -Value 86400  # 1 día
        Write-Host "? DNS MaxCacheTtl configurado a 1 día" -ForegroundColor Green
        $dnsFixed = $true
    }
    
    if ($dnsFixed) {
        $fixes += "DNS Cache optimizado para navegadores"
    }
} catch {
    Write-Host "? Error ajustando DNS Cache: $($_.Exception.Message)" -ForegroundColor Red
}

# 4. Flush DNS y reiniciar servicios relevantes
Write-Host ""
Write-Host "?? Aplicando cambios..." -ForegroundColor Yellow

try {
    # Flush DNS
    ipconfig /flushdns | Out-Null
    Write-Host "? DNS Cache limpiado" -ForegroundColor Green
    
    # Reiniciar DNS Client service
    Restart-Service -Name "Dnscache" -Force -ErrorAction SilentlyContinue
    Write-Host "? Servicio DNS Client reiniciado" -ForegroundColor Green
    
    $fixes += "DNS Cache limpiado y servicio reiniciado"
} catch {
    Write-Host "?? No se pudo reiniciar servicios automáticamente" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "?? RESUMEN DE CORRECCIONES:" -ForegroundColor Cyan
Write-Host "=========================" -ForegroundColor Cyan

if ($fixes.Count -eq 0) {
    Write-Host "?? No se necesitaron correcciones. La configuración ya es balanceada." -ForegroundColor Blue
} else {
    Write-Host "? Se aplicaron $($fixes.Count) correcciones:" -ForegroundColor Green
    foreach ($fix in $fixes) {
        Write-Host "• $fix" -ForegroundColor White
    }
}

Write-Host ""
Write-Host "?? BENEFICIOS ESPERADOS:" -ForegroundColor Green
Write-Host "======================" -ForegroundColor Green
Write-Host "• Navegadores cargan páginas más rápido" -ForegroundColor White
Write-Host "• Menos consultas DNS innecesarias" -ForegroundColor White
Write-Host "• Mejor balance gaming vs navegación web" -ForegroundColor White
Write-Host "• TCP más eficiente para múltiples pestañas" -ForegroundColor White

Write-Host ""
Write-Host "? GAMING PERFORMANCE:" -ForegroundColor Yellow
Write-Host "===================" -ForegroundColor Yellow
Write-Host "• Se mantienen la mayoría de optimizaciones para gaming" -ForegroundColor White
Write-Host "• TcpAckFrequency: 2 (sigue siendo muy bueno)" -ForegroundColor White
Write-Host "• TCPNoDelay: 1 (Nagle sigue deshabilitado)" -ForegroundColor White
Write-Host "• NetworkThrottlingIndex: 5 (prioridad alta)" -ForegroundColor White

Write-Host ""
Write-Host "?? PRÓXIMOS PASOS:" -ForegroundColor Cyan
Write-Host "================" -ForegroundColor Cyan
Write-Host "1. ? Reinicia todos los navegadores abiertos" -ForegroundColor Green
Write-Host "2. ? Prueba cargar algunas páginas web" -ForegroundColor Green
Write-Host "3. ?? Si aún hay lentitud, considera:" -ForegroundColor Yellow
Write-Host "   • Cambiar DNS servers (ejecuta: Set-DnsClientServerAddress)" -ForegroundColor Gray
Write-Host "   • Deshabilitar extensions problemáticas del navegador" -ForegroundColor Gray
Write-Host "   • Limpiar cache del navegador" -ForegroundColor Gray
Write-Host ""
Write-Host "4. ?? Prueba juegos para confirmar que el rendimiento gaming se mantiene" -ForegroundColor Blue

Write-Host ""
Write-Host "?? Corrección completada. ¡Reinicia el navegador y prueba!" -ForegroundColor Green
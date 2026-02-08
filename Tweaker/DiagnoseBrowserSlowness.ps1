# Diagn?stico de Lentitud en Navegador - DaddyGhost Tweaker
# Este script identifica qu? tweaks est?n causando lentitud en el navegador

Write-Host "?? DIAGNÓSTICO: LENTITUD EN NAVEGADOR" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar tweaks que pueden afectar navegadores
Write-Host "?? Verificando tweaks aplicados..." -ForegroundColor Yellow

$issues = @()

# 1. Verificar Network Throttling Index
try {
    $throttling = Get-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile" -Name "NetworkThrottlingIndex" -ErrorAction SilentlyContinue
    if ($throttling.NetworkThrottlingIndex -eq -1 -or $throttling.NetworkThrottlingIndex -eq [uint32]0xFFFFFFFF) {
        $issues += "? NetworkThrottlingIndex deshabilitado completamente - puede causar lentitud en navegadores"
        Write-Host "? NetworkThrottlingIndex: DESHABILITADO (FFFFFFFF)" -ForegroundColor Red
    } else {
        Write-Host "? NetworkThrottlingIndex: $($throttling.NetworkThrottlingIndex)" -ForegroundColor Green
    }
} catch {
    Write-Host "?? No se pudo verificar NetworkThrottlingIndex" -ForegroundColor Yellow
}

# 2. Verificar TcpAckFrequency extremo en interfaces
$tcpInterfaces = Get-ChildItem "HKLM:\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces"
$problematicInterfaces = 0

foreach ($interface in $tcpInterfaces) {
    try {
        $props = Get-ItemProperty -Path $interface.PSPath -ErrorAction SilentlyContinue
        
        if ($props.DhcpIPAddress -or ($props.IPAddress -and $props.IPAddress -ne "0.0.0.0")) {
            # Interface activa
            $interfaceId = $interface.PSChildName
            
            if ($props.TcpAckFrequency -eq 1) {
                Write-Host "?? Interface $interfaceId - TcpAckFrequency=1 (muy agresivo para navegadores)" -ForegroundColor Yellow
                $problematicInterfaces++
            }
            
            if ($props.TcpDelAckTicks -eq 0) {
                Write-Host "?? Interface $interfaceId - TcpDelAckTicks=0 (puede causar overhead)" -ForegroundColor Yellow
                $problematicInterfaces++
            }
        }
    } catch {
        # Continuar con siguiente interface
    }
}

if ($problematicInterfaces -gt 0) {
    $issues += "? Configuración TCP muy agresiva en $problematicInterfaces interfaces - optimizada para gaming, no navegación"
}

# 3. Verificar DNS Cache settings
try {
    $dnsCache = Get-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters" -ErrorAction SilentlyContinue
    
    if ($dnsCache.MaxNegativeCacheTtl -eq 0) {
        $issues += "? DNS Negative Cache deshabilitado - puede causar consultas DNS excesivas"
        Write-Host "? DNS MaxNegativeCacheTtl: DESHABILITADO" -ForegroundColor Red
    }
    
    if ($dnsCache.NetFailureCacheTime -eq 0) {
        Write-Host "? DNS NetFailureCacheTime: DESHABILITADO - reintenta conexiones fallidas constantemente" -ForegroundColor Red
    }
} catch {
    Write-Host "?? No se pudo verificar configuración DNS Cache" -ForegroundColor Yellow
}

# 4. Verificar si Nagle's Algorithm está deshabilitado globalmente
$nagleDisabled = 0
foreach ($interface in $tcpInterfaces) {
    try {
        $props = Get-ItemProperty -Path $interface.PSPath -ErrorAction SilentlyContinue
        if ($props.TCPNoDelay -eq 1) {
            $nagleDisabled++
        }
    } catch {
        # Continuar
    }
}

if ($nagleDisabled -gt 0) {
    $issues += "? Nagle's Algorithm deshabilitado en $nagleDisabled interfaces - causa fragmentación para navegadores"
}

Write-Host ""
Write-Host "?? RESUMEN DEL DIAGNÓSTICO:" -ForegroundColor Cyan
Write-Host "=========================" -ForegroundColor Cyan

if ($issues.Count -eq 0) {
    Write-Host "? No se encontraron tweaks problemáticos para navegadores" -ForegroundColor Green
    Write-Host ""
    Write-Host "Posibles causas externas:" -ForegroundColor Yellow
    Write-Host "• DNS servers lentos (prueba cambiar a 1.1.1.1 o 8.8.8.8)"
    Write-Host "• Extensions del navegador problemáticas"
    Write-Host "• Cache del navegador corrupto"
    Write-Host "• Antivirus/Firewall interfiriendo"
} else {
    Write-Host "? Se encontraron $($issues.Count) problemas:" -ForegroundColor Red
    Write-Host ""
    foreach ($issue in $issues) {
        Write-Host $issue -ForegroundColor Red
    }
    
    Write-Host ""
    Write-Host "?? SOLUCIONES RECOMENDADAS:" -ForegroundColor Green
    Write-Host "=========================" -ForegroundColor Green
    Write-Host "1. Ejecuta FixBrowserSlowness.ps1 para ajustes automáticos"
    Write-Host "2. O aplica el perfil 'Navegación Balanceada' en el tweaker"
    Write-Host "3. Reinicia el navegador después de los cambios"
    Write-Host "4. Si persiste, ejecuta: ipconfig /flushdns"
}

Write-Host ""
Write-Host "?? TESTS ADICIONALES:" -ForegroundColor Cyan
Write-Host "====================" -ForegroundColor Cyan

# Test de DNS
Write-Host "?? Probando velocidad de DNS..." -ForegroundColor Yellow
$dnsTests = @(
    @{Server="1.1.1.1"; Name="Cloudflare"},
    @{Server="8.8.8.8"; Name="Google"},
    @{Server="208.67.222.222"; Name="OpenDNS"}
)

foreach ($dns in $dnsTests) {
    try {
        $result = Measure-Command { nslookup google.com $dns.Server | Out-Null }
        Write-Host "• $($dns.Name) ($($dns.Server)): $([math]::Round($result.TotalMilliseconds))ms" -ForegroundColor White
    } catch {
        Write-Host "• $($dns.Name) ($($dns.Server)): ERROR" -ForegroundColor Red
    }
}

# Test de conectividad
Write-Host ""
Write-Host "?? Probando latencia..." -ForegroundColor Yellow
try {
    $pingResult = Test-NetConnection -ComputerName "google.com" -CommonTCPPort HTTP
    if ($pingResult.TcpTestSucceeded) {
        Write-Host "• Conectividad HTTP: ? OK" -ForegroundColor Green
    } else {
        Write-Host "• Conectividad HTTP: ? FALLA" -ForegroundColor Red
    }
} catch {
    Write-Host "• Test de conectividad: ? ERROR" -ForegroundColor Red
}

Write-Host ""
Write-Host "?? Diagnóstico completado. ¿Ejecutar solución automática? (FixBrowserSlowness.ps1)" -ForegroundColor Cyan
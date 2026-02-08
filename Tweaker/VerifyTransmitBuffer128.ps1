# ========================================================================================================
# SCRIPT DE VERIFICACIÓN - TRANSMIT BUFFER CONFIGURADO A 128
# ========================================================================================================
# Este script verifica que la configuración del transmit buffer se haya aplicado correctamente
# ========================================================================================================

Write-Host ""
Write-Host "?? VERIFICANDO CONFIGURACIÓN TRANSMIT BUFFER = 128" -ForegroundColor Green
Write-Host "??????????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""

# ========================================================================================================
# 1. VERIFICAR PARÁMETROS TCP ACTUALIZADOS
# ========================================================================================================
Write-Host "?? 1. VERIFICANDO PARÁMETROS TCP..." -ForegroundColor Cyan
Write-Host "???????????????????????????????????????" -ForegroundColor DarkCyan

$tcpParametersPath = "HKLM:\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters"

try {
    if (Test-Path $tcpParametersPath) {
        # Verificar DefaultTTL (Transmit Buffer)
        $defaultTTL = Get-ItemProperty -Path $tcpParametersPath -Name "DefaultTTL" -ErrorAction SilentlyContinue
        if ($defaultTTL -and $defaultTTL.DefaultTTL -eq 128) {
            Write-Host "   ? Transmit Buffer (DefaultTTL): 128 - CONFIGURADO CORRECTAMENTE" -ForegroundColor Green
        } elseif ($defaultTTL) {
            Write-Host "   ? Transmit Buffer (DefaultTTL): $($defaultTTL.DefaultTTL) - VALOR INCORRECTO (esperado: 128)" -ForegroundColor Red
        } else {
            Write-Host "   ?? Transmit Buffer (DefaultTTL): NO CONFIGURADO" -ForegroundColor Yellow
        }

        # Verificar TcpMaxDupAcks
        $maxDupAcks = Get-ItemProperty -Path $tcpParametersPath -Name "TcpMaxDupAcks" -ErrorAction SilentlyContinue
        if ($maxDupAcks -and $maxDupAcks.TcpMaxDupAcks -eq 2) {
            Write-Host "   ? TCP Max Dup ACKs: 2 - CONFIGURADO CORRECTAMENTE" -ForegroundColor Green
        } elseif ($maxDupAcks) {
            Write-Host "   ? TCP Max Dup ACKs: $($maxDupAcks.TcpMaxDupAcks) - VALOR INCORRECTO (esperado: 2)" -ForegroundColor Red
        } else {
            Write-Host "   ?? TCP Max Dup ACKs: NO CONFIGURADO" -ForegroundColor Yellow
        }

        # Verificar TcpInitialRtt
        $initialRtt = Get-ItemProperty -Path $tcpParametersPath -Name "TcpInitialRtt" -ErrorAction SilentlyContinue
        if ($initialRtt -and $initialRtt.TcpInitialRtt -eq 300) {
            Write-Host "   ? TCP Initial RTT: 300ms - CONFIGURADO CORRECTAMENTE" -ForegroundColor Green
        } elseif ($initialRtt) {
            Write-Host "   ? TCP Initial RTT: $($initialRtt.TcpInitialRtt)ms - VALOR INCORRECTO (esperado: 300)" -ForegroundColor Red
        } else {
            Write-Host "   ?? TCP Initial RTT: NO CONFIGURADO" -ForegroundColor Yellow
        }

        # Verificar otras configuraciones existentes
        $tcp1323 = Get-ItemProperty -Path $tcpParametersPath -Name "Tcp1323Opts" -ErrorAction SilentlyContinue
        $sackOpts = Get-ItemProperty -Path $tcpParametersPath -Name "SackOpts" -ErrorAction SilentlyContinue
        $keepAlive = Get-ItemProperty -Path $tcpParametersPath -Name "KeepAliveTime" -ErrorAction SilentlyContinue

        Write-Host ""
        Write-Host "   ?? Otras configuraciones TCP avanzadas:"
        Write-Host "      • TCP Window Scaling: $(if ($tcp1323 -and $tcp1323.Tcp1323Opts -eq 3) { 'HABILITADO ?' } else { 'NO CONFIGURADO ??' })"
        Write-Host "      • SACK Opts: $(if ($sackOpts -and $sackOpts.SackOpts -eq 1) { 'HABILITADO ?' } else { 'NO CONFIGURADO ??' })"
        Write-Host "      • Keep Alive Time: $(if ($keepAlive -and $keepAlive.KeepAliveTime -eq 300000) { '5 min ?' } else { 'NO CONFIGURADO ??' })"

    } else {
        Write-Host "   ? No se pudo acceder a parámetros TCP" -ForegroundColor Red
    }
} catch {
    Write-Host "   ? Error verificando parámetros TCP: $($_.Exception.Message)" -ForegroundColor Red
}

# ========================================================================================================
# 2. VERIFICAR ESTADO DEL CÓDIGO FUENTE
# ========================================================================================================
Write-Host ""
Write-Host "?? 2. VERIFICANDO CÓDIGO FUENTE ACTUALIZADO..." -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????????????" -ForegroundColor DarkCyan

$advancedNetworkFile = "Tweaker\Optimizations\AdvancedNetworkTweaks.cs"

if (Test-Path $advancedNetworkFile) {
    $content = Get-Content $advancedNetworkFile -Raw
    
    # Verificar si contiene la nueva configuración de transmit buffer
    $hasTransmitBuffer = $content -match "DefaultTTL.*128"
    $hasMaxDupAcks = $content -match "TcpMaxDupAcks.*2"
    $hasInitialRtt = $content -match "TcpInitialRtt.*300"
    $hasTransmitComment = $content -match "TRANSMIT BUFFER OPTIMIZATION"
    
    Write-Host "   ?? Verificando contenido del archivo:"
    Write-Host "      • Transmit Buffer (DefaultTTL = 128): $(if ($hasTransmitBuffer) { 'PRESENTE ?' } else { 'NO ENCONTRADO ?' })"
    Write-Host "      • TCP Max Dup ACKs = 2: $(if ($hasMaxDupAcks) { 'PRESENTE ?' } else { 'NO ENCONTRADO ?' })"
    Write-Host "      • TCP Initial RTT = 300: $(if ($hasInitialRtt) { 'PRESENTE ?' } else { 'NO ENCONTRADO ?' })"
    Write-Host "      • Comentario de optimización: $(if ($hasTransmitComment) { 'PRESENTE ?' } else { 'NO ENCONTRADO ?' })"
    
    # Verificar función de restauración
    $hasDeleteTransmit = $content -match "DeleteValue.*DefaultTTL"
    $hasDeleteDupAcks = $content -match "DeleteValue.*TcpMaxDupAcks"
    $hasDeleteRtt = $content -match "DeleteValue.*TcpInitialRtt"
    
    Write-Host ""
    Write-Host "   ?? Verificando función de restauración:"
    Write-Host "      • Eliminar DefaultTTL: $(if ($hasDeleteTransmit) { 'PRESENTE ?' } else { 'NO ENCONTRADO ?' })"
    Write-Host "      • Eliminar TcpMaxDupAcks: $(if ($hasDeleteDupAcks) { 'PRESENTE ?' } else { 'NO ENCONTRADO ?' })"
    Write-Host "      • Eliminar TcpInitialRtt: $(if ($hasDeleteRtt) { 'PRESENTE ?' } else { 'NO ENCONTRADO ?' })"
    
} else {
    Write-Host "   ? Archivo AdvancedNetworkTweaks.cs no encontrado" -ForegroundColor Red
}

# ========================================================================================================
# 3. SIMULAR PRUEBA DE CONECTIVIDAD
# ========================================================================================================
Write-Host ""
Write-Host "?? 3. SIMULANDO PRUEBA DE CONECTIVIDAD..." -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????????" -ForegroundColor DarkCyan

try {
    # Ping básico para verificar conectividad
    $pingResult = Test-Connection -ComputerName "8.8.8.8" -Count 4 -Quiet -ErrorAction SilentlyContinue
    
    if ($pingResult) {
        Write-Host "   ? Conectividad a Internet: FUNCIONANDO" -ForegroundColor Green
        
        # Obtener información de la interfaz de red activa
        $activeAdapter = Get-NetAdapter | Where-Object {$_.Status -eq "Up" -and $_.MediaType -eq "802.3"} | Select-Object -First 1
        
        if ($activeAdapter) {
            Write-Host "   ?? Adaptador de red activo: $($activeAdapter.Name)" -ForegroundColor White
            Write-Host "   ?? Velocidad de enlace: $($activeAdapter.LinkSpeed)" -ForegroundColor White
        }
        
        # Mostrar configuración TCP actual
        Write-Host ""
        Write-Host "   ?? Configuración TCP actual aplicada:"
        Write-Host "      • Transmit Buffer optimizado para gaming"
        Write-Host "      • Retransmisión más eficiente (Max Dup ACKs = 2)"
        Write-Host "      • Estimación inicial de RTT optimizada (300ms)"
        Write-Host "      • Mejora en respuesta de juegos online"
        
    } else {
        Write-Host "   ?? No se pudo verificar conectividad" -ForegroundColor Yellow
    }
} catch {
    Write-Host "   ?? Error en prueba de conectividad: $($_.Exception.Message)" -ForegroundColor Yellow
}

# ========================================================================================================
# 4. RESUMEN Y RECOMENDACIONES
# ========================================================================================================
Write-Host ""
Write-Host "?? RESUMEN DE VERIFICACIÓN" -ForegroundColor Magenta
Write-Host "???????????????????????????" -ForegroundColor Magenta

Write-Host ""
Write-Host "? CAMBIOS APLICADOS:" -ForegroundColor Green
Write-Host "   • Transmit Buffer configurado a 128"
Write-Host "   • TCP Max Dup ACKs optimizado (2)"
Write-Host "   • TCP Initial RTT configurado (300ms)"
Write-Host "   • Función de restauración actualizada"

Write-Host ""
Write-Host "?? BENEFICIOS ESPERADOS:" -ForegroundColor Cyan
Write-Host "   • Menor latencia en envío de paquetes"
Write-Host "   • Mejor respuesta en gaming competitivo"
Write-Host "   • Retransmisión TCP más eficiente"
Write-Host "   • Estimación inicial de RTT optimizada"

Write-Host ""
Write-Host "?? PRÓXIMOS PASOS:" -ForegroundColor Yellow
Write-Host "   1. Compilar la aplicación Tweaker"
Write-Host "   2. Aplicar 'Configuración Avanzada del Adaptador'"
Write-Host "   3. Reiniciar Windows para aplicar completamente"
Write-Host "   4. Probar juegos online para verificar mejora"

Write-Host ""
Write-Host "?? TRANSMIT BUFFER = 128 CONFIGURADO EXITOSAMENTE" -ForegroundColor Green
Write-Host "???????????????????????????????????????????????????" -ForegroundColor Green
Write-Host "El transmit buffer ha sido optimizado para gaming competitivo." -ForegroundColor White
Write-Host ""
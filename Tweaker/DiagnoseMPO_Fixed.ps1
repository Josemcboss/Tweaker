# ???????????????????????????????????????????????????????????????????
# SCRIPT DIAGNÓSTICO MPO (MULTIPLANE OVERLAY) - SOLUCIÓN PARPADEO
# ???????????????????????????????????????????????????????????????????
# 
# Este script diagnostica y soluciona problemas del MPO en Ghost Optimizer
# Específicamente diseñado para resolver el parpadeo al restaurar MPO
#
# AUTOR: Ghost Optimizer Team
# FECHA: 2024
# VERSIÓN: 2.0
# ???????????????????????????????????????????????????????????????????

Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?? DIAGNÓSTICO MPO (MULTIPLANE OVERLAY) - GHOST OPTIMIZER" -ForegroundColor Yellow
Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Verificar permisos de administrador
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")

if (-not $isAdmin) {
    Write-Host "? ERROR: Este script requiere permisos de Administrador" -ForegroundColor Red
    Write-Host "   Haz clic derecho en PowerShell -> 'Ejecutar como administrador'" -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Presiona Enter para salir..."
    exit
}

Write-Host "? Ejecutando como Administrador - OK" -ForegroundColor Green
Write-Host ""

# Función para verificar estado actual del MPO
function Get-MPOStatus {
    try {
        $regPath = "HKLM:\SOFTWARE\Microsoft\Windows\Dwm"
        $regName = "OverlayTestMode"
        
        if (Test-Path $regPath) {
            $value = Get-ItemProperty -Path $regPath -Name $regName -ErrorAction SilentlyContinue
            
            if ($value) {
                $mpoValue = $value.OverlayTestMode
                switch ($mpoValue) {
                    5 { 
                        return @{
                            Status = "DISABLED"
                            Value = 5
                            Description = "MPO Deshabilitado (Gaming optimized)"
                            Color = "Green"
                        }
                    }
                    0 { 
                        return @{
                            Status = "ENABLED"
                            Value = 0
                            Description = "MPO Explícitamente habilitado"
                            Color = "Yellow"
                        }
                    }
                    default { 
                        return @{
                            Status = "CUSTOM"
                            Value = $mpoValue
                            Description = "Configuración personalizada (inusual)"
                            Color = "Magenta"
                        }
                    }
                }
            } else {
                return @{
                    Status = "DEFAULT"
                    Value = $null
                    Description = "Windows Default (MPO habilitado automáticamente)"
                    Color = "Cyan"
                }
            }
        } else {
            return @{
                Status = "ERROR"
                Value = $null
                Description = "Clave DWM no encontrada (problema de Windows)"
                Color = "Red"
            }
        }
    } catch {
        return @{
            Status = "ERROR"
            Value = $null
            Description = "Error al leer registro: $($_.Exception.Message)"
            Color = "Red"
        }
    }
}

# Función para mostrar problemas comunes
function Show-CommonIssues {
    Write-Host "?? PROBLEMAS COMUNES CON MPO:" -ForegroundColor Red
    Write-Host "?????????????????????????????????????????" -ForegroundColor DarkGray
    
    $issues = @(
        @{ Symbol = "??"; Issue = "STUTTERING"; Description = "Micro-stutters cada pocos segundos en juegos" },
        @{ Symbol = "???"; Issue = "PANTALLAZOS NEGROS"; Description = "Pantalla negra al hacer Alt+Tab o cambiar ventanas" },
        @{ Symbol = "??"; Issue = "PROBLEMAS OVERLAYS"; Description = "Discord, Steam overlay con lag o no funcionan" },
        @{ Symbol = "??"; Issue = "G-SYNC ISSUES"; Description = "G-Sync/FreeSync inconsistente o con flickering" },
        @{ Symbol = "??"; Issue = "STREAMING PROBLEMS"; Description = "OBS captura pantalla negra o con artifacts" },
        @{ Symbol = "?"; Issue = "PARPADEO AL RESTAURAR"; Description = "Pantalla parpadea al volver a habilitar MPO" }
    )
    
    foreach ($issue in $issues) {
        Write-Host "   $($issue.Symbol) $($issue.Issue):" -ForegroundColor White -NoNewline
        Write-Host " $($issue.Description)" -ForegroundColor Gray
    }
    Write-Host ""
}

# Función para aplicar soluciones
function Apply-MPOFix {
    param(
        [string]$Action
    )
    
    $regPath = "HKLM:\SOFTWARE\Microsoft\Windows\Dwm"
    $regName = "OverlayTestMode"
    
    try {
        switch ($Action.ToLower()) {
            "disable" {
                Write-Host "?? DESHABILITANDO MPO (Modo Gaming)..." -ForegroundColor Yellow
                
                # Crear clave si no existe
                if (-not (Test-Path $regPath)) {
                    New-Item -Path $regPath -Force | Out-Null
                }
                
                # Establecer valor 5 (deshabilita MPO)
                Set-ItemProperty -Path $regPath -Name $regName -Value 5 -Type DWord
                
                Write-Host "? MPO deshabilitado correctamente" -ForegroundColor Green
                Write-Host "   Registro: $regPath\$regName = 5" -ForegroundColor Gray
            }
            
            "enable" {
                Write-Host "?? RESTAURANDO MPO (Configuración Windows)..." -ForegroundColor Yellow
                
                if (Test-Path $regPath) {
                    # MÉTODO MEJORADO: Eliminar clave en vez de establecer 0
                    # Esto previene conflictos y parpadeos
                    Remove-ItemProperty -Path $regPath -Name $regName -ErrorAction SilentlyContinue
                    
                    Write-Host "? MPO restaurado correctamente" -ForegroundColor Green
                    Write-Host "   Registro: $regPath\$regName eliminado (Windows default)" -ForegroundColor Gray
                } else {
                    Write-Host "? MPO ya está en configuración Windows default" -ForegroundColor Green
                }
            }
            
            "reset" {
                Write-Host "?? RESET COMPLETO DE MPO..." -ForegroundColor Yellow
                
                if (Test-Path $regPath) {
                    # Limpiar completamente
                    Remove-ItemProperty -Path $regPath -Name $regName -ErrorAction SilentlyContinue
                    
                    Write-Host "? MPO reseteado completamente" -ForegroundColor Green
                    Write-Host "   Todas las configuraciones MPO eliminadas" -ForegroundColor Gray
                }
            }
        }
        
        return $true
    } catch {
        Write-Host "? Error aplicando solución: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# DIAGNÓSTICO PRINCIPAL
Write-Host "?? ESTADO ACTUAL DEL SISTEMA:" -ForegroundColor White
Write-Host "?????????????????????????????????????????" -ForegroundColor DarkGray

# 1. Estado del MPO
$mpoStatus = Get-MPOStatus
Write-Host "   ?? Estado MPO: " -NoNewline
Write-Host "$($mpoStatus.Status)" -ForegroundColor $mpoStatus.Color
Write-Host "   ?? Descripción: $($mpoStatus.Description)" -ForegroundColor Gray
if ($mpoStatus.Value -ne $null) {
    Write-Host "   ?? Valor registro: $($mpoStatus.Value)" -ForegroundColor Gray
}
Write-Host ""

# 2. Versión Windows
$windowsVersion = (Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion").DisplayVersion
Write-Host "   ?? Windows Version: $windowsVersion" -ForegroundColor Cyan

# 3. Información GPU
try {
    $gpu = Get-CimInstance -ClassName Win32_VideoController | Where-Object { $_.Name -notlike "*Microsoft*" } | Select-Object -First 1
    Write-Host "   ?? GPU Principal: $($gpu.Name)" -ForegroundColor Cyan
} catch {
    Write-Host "   ?? No se pudo obtener información de GPU" -ForegroundColor Yellow
}
Write-Host ""

# Mostrar problemas comunes
Show-CommonIssues

# RECOMENDACIONES BASADAS EN ESTADO ACTUAL
Write-Host "?? RECOMENDACIONES PARA TU SISTEMA:" -ForegroundColor White
Write-Host "?????????????????????????????????????????" -ForegroundColor DarkGray

switch ($mpoStatus.Status) {
    "DEFAULT" {
        Write-Host "   ?? Tu MPO está en configuración Windows predeterminada" -ForegroundColor Cyan
        Write-Host "   ?? Recomendación:" -ForegroundColor White
        Write-Host "      • SI tienes stuttering ? Deshabilitar MPO" -ForegroundColor Yellow
        Write-Host "      • SI funciona bien ? Mantener como está" -ForegroundColor Green
    }
    
    "DISABLED" {
        Write-Host "   ?? Tu MPO está DESHABILITADO (Gaming optimized)" -ForegroundColor Green
        Write-Host "   ?? Recomendación:" -ForegroundColor White
        Write-Host "      • SI tienes buen rendimiento ? Perfecto, mantener" -ForegroundColor Green
        Write-Host "      • SI tienes parpadeo ? Usar método de restauración segura" -ForegroundColor Yellow
    }
    
    "ENABLED" {
        Write-Host "   ?? Tu MPO está EXPLÍCITAMENTE HABILITADO" -ForegroundColor Yellow
        Write-Host "   ?? Recomendación:" -ForegroundColor White
        Write-Host "      • Configuración inusual, considerar reset" -ForegroundColor Yellow
    }
    
    "ERROR" {
        Write-Host "   ?? ERROR en configuración MPO" -ForegroundColor Red
        Write-Host "   ?? Recomendación:" -ForegroundColor White
        Write-Host "      • Usar opción de reset completo" -ForegroundColor Red
    }
}
Write-Host ""

# MENÚ DE OPCIONES
Write-Host "?? OPCIONES DISPONIBLES:" -ForegroundColor White
Write-Host "?????????????????????????????????????????" -ForegroundColor DarkGray
Write-Host "   1??  Deshabilitar MPO (Solucionar stuttering)" -ForegroundColor Green
Write-Host "   2??  Restaurar MPO (Método seguro anti-parpadeo)" -ForegroundColor Yellow
Write-Host "   3??  Reset completo MPO (Limpiar configuración)" -ForegroundColor Magenta
Write-Host "   4??  Solo mostrar diagnóstico (No cambiar nada)" -ForegroundColor Cyan
Write-Host "   5??  Salir" -ForegroundColor White
Write-Host ""

do {
    $choice = Read-Host "Selecciona una opción (1-5)"
    
    switch ($choice) {
        "1" {
            Write-Host ""
            Write-Host "?? DESHABILITANDO MPO..." -ForegroundColor Yellow
            Write-Host "Esto solucionará stuttering pero requiere REINICIO" -ForegroundColor Gray
            
            if (Apply-MPOFix -Action "disable") {
                Write-Host ""
                Write-Host "? MPO DESHABILITADO EXITOSAMENTE" -ForegroundColor Green
                Write-Host "?? REINICIA Windows para aplicar cambios" -ForegroundColor Red
                Write-Host ""
                Write-Host "?? BENEFICIOS ESPERADOS:" -ForegroundColor White
                Write-Host "   • Stuttering eliminado" -ForegroundColor Green
                Write-Host "   • Frame times más consistentes" -ForegroundColor Green
                Write-Host "   • Overlays funcionando mejor" -ForegroundColor Green
            }
            break
        }
        
        "2" {
            Write-Host ""
            Write-Host "?? RESTAURANDO MPO (MÉTODO SEGURO)..." -ForegroundColor Yellow
            Write-Host "Esto restaura MPO minimizando parpadeos" -ForegroundColor Gray
            
            if (Apply-MPOFix -Action "enable") {
                Write-Host ""
                Write-Host "? MPO RESTAURADO EXITOSAMENTE" -ForegroundColor Green
                Write-Host "?? REINICIA Windows para aplicar cambios" -ForegroundColor Red
                Write-Host ""
                Write-Host "?? ADVERTENCIAS:" -ForegroundColor White
                Write-Host "   • Stuttering puede volver si tenías problemas" -ForegroundColor Yellow
                Write-Host "   • Si tienes issues, vuelve a deshabilitar MPO" -ForegroundColor Yellow
            }
            break
        }
        
        "3" {
            Write-Host ""
            Write-Host "?? RESET COMPLETO DE MPO..." -ForegroundColor Magenta
            Write-Host "Esto limpia toda configuración MPO" -ForegroundColor Gray
            
            if (Apply-MPOFix -Action "reset") {
                Write-Host ""
                Write-Host "? MPO RESETEADO COMPLETAMENTE" -ForegroundColor Green
                Write-Host "?? REINICIA Windows para aplicar cambios" -ForegroundColor Red
                Write-Host ""
                Write-Host "?? ESTADO FINAL:" -ForegroundColor White
                Write-Host "   • MPO volverá a configuración Windows predeterminada" -ForegroundColor Cyan
                Write-Host "   • Puedes aplicar tweaks nuevamente si necesario" -ForegroundColor Cyan
            }
            break
        }
        
        "4" {
            Write-Host ""
            Write-Host "?? DIAGNÓSTICO COMPLETO MOSTRADO ARRIBA" -ForegroundColor Cyan
            Write-Host "No se realizaron cambios en el sistema" -ForegroundColor Gray
            break
        }
        
        "5" {
            Write-Host ""
            Write-Host "?? Saliendo del diagnóstico MPO..." -ForegroundColor White
            Write-Host "Recuerda: Cualquier cambio MPO requiere REINICIO" -ForegroundColor Yellow
            break
        }
        
        default {
            Write-Host "? Opción inválida. Selecciona 1-5." -ForegroundColor Red
            continue
        }
    }
    
    break
} while ($true)

Write-Host ""
Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?? GHOST OPTIMIZER - MPO DIAGNOSTIC COMPLETE" -ForegroundColor Yellow
Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

if ($choice -in @("1", "2", "3")) {
    Write-Host "?? SIGUIENTE PASO OBLIGATORIO:" -ForegroundColor White
    Write-Host "   ?? REINICIA tu PC para aplicar los cambios" -ForegroundColor Red
    Write-Host "   ??  Los cambios MPO NO funcionan sin reinicio" -ForegroundColor Red
    Write-Host ""
    
    $restart = Read-Host "¿Quieres reiniciar ahora? (S/N)"
    if ($restart -eq "S" -or $restart -eq "s") {
        Write-Host "?? Reiniciando en 10 segundos..." -ForegroundColor Yellow
        Start-Sleep -Seconds 3
        Restart-Computer -Force
    }
}

Read-Host "Presiona Enter para salir..."
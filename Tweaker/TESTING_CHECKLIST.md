# ? CHECKLIST DE TESTING - GHOST OPTIMIZER

## ?? OBJETIVO
Verificar que todos los tweaks funcionen correctamente y que la reversión sea segura.

---

## ?? PREPARACIÓN (15 minutos)

- [ ] **Crear Punto de Restauración Manual**
  ```powershell
  # PowerShell (Admin)
  Checkpoint-Computer -Description "GHOST Testing" -RestorePointType "MODIFY_SETTINGS"
  ```

- [ ] **Hacer Backup del Registry**
  - Abrir `regedit`
  - File > Export > Guardar como `registry_backup_YYYYMMDD.reg`

- [ ] **Guardar estado ANTES**
  - Ejecutar `QuickTest.ps1` (Opción 1)

- [ ] **Instalar Herramientas de Medición**
  - [ ] LatencyMon: https://www.resplendence.com/latencymon
  - [ ] 3DMark (opcional): Steam
  - [ ] CrystalDiskMark: https://crystalmark.info/

---

## ?? TESTING POR CATEGORÍA

### 1?? RED & PING

- [ ] **Optimizar Red (ON)**
  - Click en botón ON
  - Verificar MessageBox de éxito
  - Ejecutar: `VerifyTweaks.ps1` y verificar sección "Red y Ping"
  - Medir ping: `ping google.com -n 20`

- [ ] **Restaurar Red (OFF)**
  - Click en botón OFF
  - Verificar que revierte correctamente
  - Ping debería volver a valores normales

**Estado**: ? PASS / ? FAIL  
**Notas**: _____________________________________

---

### 2?? GPU & SISTEMA

- [ ] **System Profile Games Priority (ON)**
  - Aplicar
  - Verificar en Registry: GPU Priority = 8
  - **Reiniciar Windows**

- [ ] **GameDVR / Xbox Game Bar (ON)**
  - Deshabilitar
  - Verificar en Registry: GameDVR_Enabled = 0
  - **Efecto inmediato** (no requiere reinicio)

- [ ] **Hardware GPU Scheduling (ON/OFF)**
  - Probar ambos estados
  - **Reiniciar Windows** después de cada cambio
  - Medir input lag con LatencyMon

**Estado**: ? PASS / ? FAIL  
**Notas**: _____________________________________

---

### 3?? CPU OPTIMIZATIONS

- [ ] **System Responsiveness (ON)**
  - Aplicar
  - Verificar: SystemResponsiveness = 0
  - **Reiniciar Windows**

- [ ] **Plan de Energía: Alto Rendimiento (ON)**
  - Activar
  - Verificar: `powercfg /getactivescheme` muestra "High performance"
  - **Efecto inmediato**

- [ ] **Power Throttling (ON)**
  - Deshabilitar
  - Verificar en Registry
  - **Reiniciar Windows**

- [ ] **Core Parking (ON)**
  - Deshabilitar
  - Verificar en Resource Monitor (todos los cores activos)
  - **Reiniciar Windows**

**Estado**: ? PASS / ? FAIL  
**Notas**: _____________________________________

---

### 4?? WINDOWS BLOATWARE

- [ ] **Hibernación (ON)**
  - Deshabilitar
  - Verificar: `C:\hiberfil.sys` NO existe después de reiniciar
  - **Reiniciar Windows**

- [ ] **Windows Search (ON)**
  - Deshabilitar
  - Verificar servicio: `Get-Service WSearch` = Stopped
  - **Efecto inmediato**

- [ ] **SysMain/SuperFetch (ON)**
  - Deshabilitar
  - Verificar servicio: `Get-Service SysMain` = Stopped
  - Verificar RAM libre (debería aumentar)
  - **Efecto inmediato**

- [ ] **Telemetry/DiagTrack (ON)**
  - Deshabilitar
  - Verificar servicio: `Get-Service DiagTrack` = Stopped
  - **Efecto inmediato**

**Estado**: ? PASS / ? FAIL  
**Notas**: _____________________________________

---

### 5?? GHOST PACK ??

- [ ] **MPO (ON)**
  - Deshabilitar
  - Verificar en Registry: OverlayTestMode = 5
  - **Reiniciar Windows**
  - Probar juego para verificar si elimina stuttering

- [ ] **Ultimate Performance Plan (ON)**
  - Activar
  - Verificar: `powercfg /list` muestra "Ultimate Performance" activo
  - **Efecto inmediato**
  - Medir latencia con LatencyMon

- [ ] **Xbox Game Bar (ON)**
  - Deshabilitar
  - Verificar en Registry
  - **Efecto inmediato**
  - Probar juego para verificar reducción de input lag

- [ ] **Core Isolation/VBS (ON)**
  - Deshabilitar
  - Verificar en Registry: EnableVirtualizationBasedSecurity = 0
  - **Reiniciar Windows OBLIGATORIO**
  - Ejecutar 3DMark ANTES y DESPUÉS para medir diferencia

- [ ] **HPET (ON)**
  - Deshabilitar
  - Verificar con bcdedit: `useplatformclock No`
  - **Reiniciar Windows OBLIGATORIO**

- [ ] **Hyper-V (ON)**
  - ?? **ADVERTENCIA**: Docker y WSL2 dejarán de funcionar
  - Deshabilitar solo si NO usas virtualización
  - Verificar con bcdedit: `hypervisorlaunchtype Off`
  - **Reiniciar Windows OBLIGATORIO**

**Estado**: ? PASS / ? FAIL  
**Notas**: _____________________________________

---

### 6?? INPUT & VISUALS

- [ ] **Mouse Acceleration (ON)**
  - Deshabilitar
  - Verificar en Registry: MouseSpeed = 0
  - **Efecto inmediato**
  - Probar en juego: aim debe ser 1:1 pixel perfect

- [ ] **Teclado (ON)**
  - Optimizar
  - Verificar en Registry: KeyboardDelay = 0
  - **Efecto inmediato**

- [ ] **Efectos Visuales (ON)**
  - Optimizar
  - Verificar en Registry: VisualFXSetting = 2
  - **Efecto inmediato**
  - Windows debe verse más "flat" pero rápido

- [ ] **Memoria/RAM (ON)**
  - ?? **ADVERTENCIA**: Requiere 16GB+ RAM
  - Optimizar
  - Verificar en Registry: DisablePagingExecutive = 1
  - **Reiniciar Windows**

**Estado**: ? PASS / ? FAIL  
**Notas**: _____________________________________

---

### 7?? CLEANER ???

- [ ] **Analizar Espacio**
  - Click en "ANALIZAR"
  - Verificar que muestra cantidad de archivos y MB

- [ ] **Limpiar Archivos Temporales**
  - Click en "LIMPIAR"
  - Confirmar
  - Verificar que libera espacio
  - Verificar en `C:\Windows\Temp` (debe estar vacío o casi vacío)

- [ ] **Flush DNS**
  - Click en "FLUSH DNS"
  - Verificar: `ipconfig /displaydns` debe estar vacío

**Estado**: ? PASS / ? FAIL  
**Notas**: _____________________________________

---

### 8?? ADVANCED TWEAKS ??

- [ ] **Spectre & Meltdown (ON)**
  - ?? **PELIGROSO**: Solo para testing, NO para producción
  - Deshabilitar
  - Verificar en Registry
  - **Reiniciar Windows**
  - Ejecutar 3DMark ANTES y DESPUÉS para medir ganancia de FPS

- [ ] **GPU IRQ (EN DESARROLLO)**
  - Verificar que muestra mensaje "Función en Desarrollo"
  - NO debe hacer cambios reales

**Estado**: ? PASS / ? FAIL  
**Notas**: _____________________________________

---

## ?? TESTING DEL BOTÓN "REVERTIR TODO" (CRÍTICO)

### Pre-requisitos
- [ ] Haber aplicado al menos 10 tweaks diferentes
- [ ] Documentar qué tweaks se aplicaron

### Pasos
1. [ ] Navegar al Dashboard
2. [ ] Click en botón "?? REVERTIR TODO (Emergencia)"
3. [ ] Leer advertencia completa
4. [ ] Click en "Sí" para confirmar
5. [ ] Esperar a que complete (5-10 segundos)
6. [ ] Leer MessageBox con resumen
7. [ ] Verificar log en Visual Studio Output
8. [ ] **REINICIAR Windows**

### Verificación POST-REINICIO
```powershell
# Ejecutar después de reiniciar
.\VerifyTweaks.ps1
```

**Resultado Esperado**:
- [ ] Todos los tweaks revertidos a valores predeterminados
- [ ] Tasa de éxito > 90% en el log
- [ ] Sistema funciona normalmente
- [ ] Sin errores en el log

**Estado**: ? PASS / ? FAIL  
**Notas**: _____________________________________

---

## ?? MEDICIONES (OPCIONAL)

### LatencyMon (ANTES vs DESPUÉS)
- [ ] DPC Latency:
  - ANTES: _______ µs
  - DESPUÉS: _______ µs
  - Mejora: _______ %

### 3DMark (ANTES vs DESPUÉS)
- [ ] Time Spy Score:
  - ANTES: _______
  - DESPUÉS: _______
  - Mejora: _______ %

### Ping (ANTES vs DESPUÉS)
- [ ] Ping a google.com:
  - ANTES: _______ ms
  - DESPUÉS: _______ ms
  - Mejora: _______ ms

### RAM Libre (ANTES vs DESPUÉS)
- [ ] RAM Disponible:
  - ANTES: _______ GB
  - DESPUÉS: _______ GB
  - Ganancia: _______ GB

---

## ?? BUGS ENCONTRADOS

### Bug #1
- **Descripción**: _____________________________________
- **Severidad**: ? Crítica / ? Alta / ? Media / ? Baja
- **Pasos para Reproducir**: _____________________________________
- **Screenshot**: _____________________________________

### Bug #2
- **Descripción**: _____________________________________
- **Severidad**: ? Crítica / ? Alta / ? Media / ? Baja
- **Pasos para Reproducir**: _____________________________________

---

## ? VERIFICACIÓN FINAL

- [ ] **Ejecutar `VerifyTweaks.ps1`** (debe mostrar > 90% éxito)
- [ ] **Ejecutar `QuickTest.ps1` Opción 2** (guardar estado DESPUÉS)
- [ ] **Ejecutar `QuickTest.ps1` Opción 3** (comparar ANTES vs DESPUÉS)
- [ ] **Sin crashes durante testing**
- [ ] **Todos los MessageBoxes funcionan correctamente**
- [ ] **Botón "REVERTIR TODO" funciona correctamente**
- [ ] **Sin errores en Visual Studio Output**

---

## ?? RESUMEN DE TESTING

**Fecha**: _______________

**Tester**: _____________________________________

**Sistema**:
- OS: Windows ___ (Build _____)
- CPU: _____________________________________
- RAM: _____ GB
- GPU: _____________________________________

**Resultados**:
- Total de Tests: _____
- Pasados: _____
- Fallados: _____
- Tasa de Éxito: _____ %

**Bugs Críticos Encontrados**: _____

**Recomendación**:
? ? LISTO PARA RELEASE  
? ?? REQUIERE MÁS TRABAJO  
? ? NO LISTO

**Comentarios Adicionales**:
_____________________________________
_____________________________________
_____________________________________

---

**Firma del Tester**: _____________________________________

**Fecha**: _______________

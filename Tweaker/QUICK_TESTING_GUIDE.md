# ?? GUÍA RÁPIDA DE TESTING - GHOST OPTIMIZER

## ?? ANTES DE EMPEZAR

### 1. **Crear Punto de Restauración** (CRÍTICO)
```powershell
# PowerShell (como Administrador)
Checkpoint-Computer -Description "GHOST Testing" -RestorePointType "MODIFY_SETTINGS"
```

### 2. **Guardar Estado Inicial**
```powershell
# Ejecutar en PowerShell
cd Tweaker
.\QuickTest.ps1
# Seleccionar opción 1
```

---

## ?? TESTING RÁPIDO (30 MINUTOS)

### Método 1: Verificación Automática

```powershell
# 1. Ejecutar GHOST Optimizer
# 2. Aplicar algunos tweaks (ej: Red, GPU, Mouse)
# 3. REINICIAR si es necesario
# 4. Ejecutar script de verificación:

cd Tweaker
.\VerifyTweaks.ps1
```

**Resultado Esperado**: Tasa de éxito > 90%

---

### Método 2: Comparación ANTES/DESPUÉS

```powershell
# PASO 1: Antes de aplicar tweaks
cd Tweaker
.\QuickTest.ps1
# Opción 1: Guardar estado ANTES

# PASO 2: Aplicar tweaks en GHOST Optimizer

# PASO 3: Después de aplicar tweaks
.\QuickTest.ps1
# Opción 2: Guardar estado DESPUÉS

# PASO 4: Ver comparación
.\QuickTest.ps1
# Opción 3: Comparar
```

---

## ?? TESTING BÁSICO (5 TWEAKS ESENCIALES)

### 1. Mouse Acceleration OFF ??
- Página: Input & Visuals
- Botón: ON (para deshabilitar)
- Verificar: `MouseSpeed = 0`
- Reinicio: NO requerido
- **Test**: Mueve el mouse en juego, debe ser 1:1

### 2. Network Optimization ??
- Página: Red & Ping
- Botón: ON
- Verificar: `NetworkThrottlingIndex = 0xFFFFFFFF`
- Reinicio: Recomendado
- **Test**: `ping google.com -n 20` (debe reducir ping)

### 3. Core Isolation OFF ??
- Página: GHOST Pack
- Botón: ON (para deshabilitar)
- Verificar: `EnableVirtualizationBasedSecurity = 0`
- Reinicio: **OBLIGATORIO**
- **Test**: 3DMark ANTES y DESPUÉS (+10-30% FPS esperado)

### 4. SysMain OFF ???
- Página: Limpieza
- Botón: ON (para deshabilitar)
- Verificar: `Get-Service SysMain` = Stopped
- Reinicio: NO requerido
- **Test**: Verificar RAM libre (debe aumentar 1-3GB)

### 5. Ultimate Performance ?
- Página: GHOST Pack
- Botón: ON
- Verificar: `powercfg /getactivescheme` = Ultimate Performance
- Reinicio: NO requerido
- **Test**: LatencyMon (latencia debe bajar)

---

## ? VERIFICACIONES RÁPIDAS

### Registry Quick Check (PowerShell)
```powershell
# Red
Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile" -Name NetworkThrottlingIndex

# GPU
Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games" -Name "GPU Priority"

# Mouse
Get-ItemProperty "HKCU:\Control Panel\Mouse" -Name MouseSpeed

# Servicios
Get-Service SysMain, DiagTrack, WSearch | Select Name, Status, StartType
```

---

## ?? TESTING DEL BOTÓN "REVERTIR TODO"

### Pasos Rápidos
1. Aplicar 5-10 tweaks
2. Ir al Dashboard
3. Click en "?? REVERTIR TODO"
4. Confirmar
5. **REINICIAR**
6. Ejecutar: `.\VerifyTweaks.ps1`

**Resultado Esperado**: Todos los tweaks revertidos

---

## ?? SI ALGO SALE MAL

### Opción 1: Botón "REVERTIR TODO"
```
Dashboard > ?? REVERTIR TODO > Confirmar > REINICIAR
```

### Opción 2: Punto de Restauración
```
Windows + R > rstrui.exe > Seleccionar punto de restauración
```

### Opción 3: Registry Backup
```
regedit > File > Import > Seleccionar .reg backup
```

---

## ?? HERRAMIENTAS RECOMENDADAS

### 1. LatencyMon (DPC Latency)
- **Descarga**: https://www.resplendence.com/latencymon
- **Uso**: Ejecutar 5 min ANTES y DESPUÉS
- **Métrica**: "Highest measured interrupt" < 1ms es bueno

### 2. 3DMark (FPS Gaming)
- **Descarga**: Steam
- **Uso**: Time Spy ANTES y DESPUÉS
- **Métrica**: +10-30% FPS con Core Isolation OFF

### 3. PingPlotter (Red)
- **Descarga**: https://www.pingplotter.com/
- **Uso**: Medir ping ANTES y DESPUÉS
- **Métrica**: -5 a -30ms con Network Optimization

---

## ?? CHECKLIST ULTRA-RÁPIDO

- [ ] Punto de Restauración creado
- [ ] Estado ANTES guardado (`QuickTest.ps1`)
- [ ] 5 tweaks esenciales probados
- [ ] Estado DESPUÉS guardado
- [ ] Comparación ejecutada
- [ ] Botón "REVERTIR TODO" probado
- [ ] Sin crashes
- [ ] Sistema funciona normal

---

## ?? REPORTAR BUG RÁPIDO

Si encuentras un bug, anótalo así:

```markdown
## Bug: [Título]
- **Tweak**: [Nombre del tweak]
- **Qué pasó**: [Descripción]
- **Qué esperaba**: [Resultado esperado]
- **Registry Path**: [Si aplica]
- **Screenshot**: [Si es necesario]
```

Guárdalo en un archivo `BUGS_FOUND.txt`

---

## ? TESTING COMPLETADO

Si todo funciona:
1. ? Todos los tweaks se aplican correctamente
2. ? Botón "REVERTIR TODO" funciona
3. ? Sin crashes
4. ? Sistema funciona normal después de revertir

**¡Listo para release!** ??

---

## ?? AYUDA RÁPIDA

### "No se aplica el tweak"
- Verifica permisos de Administrador
- Revisa Output de Visual Studio
- Verifica path del Registry manualmente

### "Botón REVERTIR TODO no funciona"
- Usa Punto de Restauración de Windows
- Importa backup del Registry
- Revierte manualmente cada tweak (OFF)

### "Sistema inestable después de tweaks"
- REINICIA Windows (muchos tweaks lo requieren)
- Usa Punto de Restauración
- Revierte tweaks uno por uno para identificar el problema

---

**¡IMPORTANTE!**: SIEMPRE haz backup antes de testing ???

**Autor**: GitHub Copilot  
**Versión**: 1.0  
**Fecha**: 2024

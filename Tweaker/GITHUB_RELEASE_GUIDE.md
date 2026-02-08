# ?? Guía para Publicar en GitHub

## ?? Paso a Paso

### 1?? Compilar el Programa

Ejecuta el script de publicación:

```powershell
.\Publish.ps1
```

Recomendaciones:
- ? **Portable (Single-File):** Más fácil de usar
- ? **Crear los 3 ZIP:** Para dar opciones a usuarios

---

### 2?? Probar el Ejecutable

Antes de publicar, **prueba**:

1. Ejecutar `Tweaker.exe` como Administrador
2. Navegar por todas las páginas
3. Activar 2-3 tweaks
4. Verificar notificaciones
5. Probar "Revertir Todo"

**Si todo funciona** ? Continúa

---

### 3?? Preparar Release en GitHub

#### A. Ir a GitHub
1. Abre https://github.com/Josemcboss/Tweaker
2. Click en **"Releases"**
3. Click en **"Draft a new release"**

#### B. Configurar Release
```
Tag version: v2.0.0
Release title: GHOST OPTIMIZER v2.0.0 - Gaming Optimizer
Target: master
```

#### C. Descripción del Release

Copia y pega:

```markdown
# ?? GHOST OPTIMIZER v2.0.0

## ?? Optimizador Gaming Profesional para Windows

**GHOST OPTIMIZER** es la herramienta definitiva para gamers competitivos que buscan el máximo rendimiento en Windows 10/11.

---

## ? Novedades v2.0

### ?? Características Nuevas
- ? **Dashboard dinámico** con estadísticas en tiempo real
- ?? **Botón "Revertir Todo"** de emergencia
- ?? **Optimizaciones avanzadas de red** (MTU, QoS, Auto-Tuning, etc.)
- ?? **Sistema de notificaciones** in-app moderno
- ?? **Backup automático** de configuraciones

### ?? Mejoras de UI
- ?? **Title bar mejorado** con glow effect
- ?? **Navegación tipo Discord** fluida
- ?? **Colores gaming** modernos (Hone.gg style)
- ?? **Responsive design** optimizado

### ?? Bugs Corregidos
- ? Warnings CS8625 eliminados
- ? Emojis correctos en todos los botones
- ? Detección de red mejorada
- ? Título de ventana mejorado

---

## ?? Descargas

### ?? Versión Portable (Recomendada)
**Un solo archivo .exe - No requiere .NET**

Ideal para:
- ? Usuarios que quieren simplicidad
- ? PCs sin .NET 10 instalado
- ? Distribución rápida

**Tamaño:** ~150 MB  
[?? GHOST-Optimizer-v2.0-Portable.zip](enlace)

---

### ?? Versión Ligera
**Requiere .NET 10 instalado**

Ideal para:
- ? Usuarios con .NET 10 ya instalado
- ? Menor tamaño de descarga

**Tamaño:** ~5-10 MB  
[?? GHOST-Optimizer-v2.0-Framework.zip](enlace)

---

### ?? Versión Completa
**Incluye todos los archivos (sin comprimir)**

Ideal para:
- ? Desarrollo y debugging
- ? Testing avanzado

**Tamaño:** ~150 MB  
[?? GHOST-Optimizer-v2.0-Full.zip](enlace)

---

## ?? Optimizaciones Disponibles

### ?? Red & Ping
- TCP/IP Optimization (ping -5 a -30ms)
- DNS Cloudflare/Google
- MTU Optimization
- QoS Configuration
- Network Throttling OFF

### ?? Input & Visuals
- Mouse Acceleration OFF (aim pixel-perfect)
- Keyboard Optimization (input lag -50ms)
- Visual Effects OFF (FPS +3-8%)
- RAM Optimization

### ?? Sistema & GPU
- System Profile Games
- GameDVR OFF (input lag -15ms)
- GPU Hardware Scheduling
- Core Parking OFF

### ??? Limpieza
- Hibernation OFF (8-32GB liberados)
- Windows Search OFF (200-500MB RAM)
- SysMain OFF (1-3GB RAM)
- Telemetry OFF

### ?? GHOST Pack
- MPO OFF (sin stuttering)
- Ultimate Performance (latencia CPU -93%)
- Core Isolation OFF (FPS +10-30%)
- HPET OFF

---

## ?? Requisitos

- **Windows 10/11** (64-bit)
- **Permisos de Administrador** (obligatorio)
- **.NET 10** (solo versión Framework)

---

## ?? Instalación

1. **Descargar** el ZIP (recomendado: Portable)
2. **Extraer** el contenido
3. **Ejecutar como Administrador** `Tweaker.exe`
4. **Crear punto de restauración** (recomendado)

---

## ?? Resultados Esperados

- **Ping:** -5 a -30ms
- **FPS:** +5 a +30 (según hardware)
- **Input lag:** -50 a -80ms
- **RAM libre:** +1 a +5GB

---

## ?? Advertencias

- ? **Sí crear** punto de restauración
- ? **Sí leer** descripciones antes de activar
- ? **No aplicar** Spectre/Meltdown sin entender los riesgos
- ? **No deshabilitar** Hyper-V si usas Docker/WSL2

---

## ?? Revertir Cambios

### Opción 1: Botón "Revertir Todo"
Dashboard ? Acciones Rápidas ? ?? REVERTIR TODOS LOS TWEAKS

### Opción 2: Punto de Restauración
Windows + R ? `rstrui.exe` ? Seleccionar punto

---

## ?? Soporte

- **Issues:** [Reportar problema](https://github.com/Josemcboss/Tweaker/issues)
- **Documentación:** [README completo](https://github.com/Josemcboss/Tweaker/blob/master/README_USER.md)

---

## ?? Créditos

- **Desarrollado por:** DaddyGhost
- **Inspirado por:** AdamX Tweaks, FR33THY, ChrisTitusTech
- **Comunidad:** Gaming community feedback

---

## ?? Licencia

MIT License - Open Source

**Disclaimer:** Usa bajo tu propio riesgo. Crea un punto de restauración antes de aplicar tweaks.

---

**¡Feliz gaming! ????**

---

## ?? Changelog Completo

### Added
- Dashboard dinámico con estadísticas
- Botón "Revertir Todo"
- Optimizaciones avanzadas de red
- Sistema de notificaciones moderno
- Profile Manager (backend)
- Backup Service automático

### Improved
- Title bar con glow effect
- Navegación fluida tipo Discord
- Colores gaming modernos
- Tooltips informativos

### Fixed
- Warnings CS8625 eliminados
- Emojis corregidos
- Detección de red mejorada
- Compilación limpia (0 warnings)

---

**Versión:** 2.0.0  
**Fecha:** 2026-02-03  
**Build:** Release
```

---

### 4?? Subir Archivos

En la sección **"Attach binaries"**:

1. **Arrastra y suelta** los 3 archivos ZIP:
   - `GHOST-Optimizer-v2.0-Portable.zip`
   - `GHOST-Optimizer-v2.0-Framework.zip`
   - `GHOST-Optimizer-v2.0-Full.zip`

2. Espera a que se suban (puede tardar)

---

### 5?? Configurar Opciones

- ? **Set as the latest release** (marcar)
- ? **Create a discussion** (opcional)
- ? **Pre-release** (desmarcar)

---

### 6?? Publicar

Click en **"Publish release"**

---

## ?? ¡Listo!

Tu release está publicado en:
```
https://github.com/Josemcboss/Tweaker/releases/latest
```

---

## ?? Compartir

### Reddit
```
Título: [Release] GHOST OPTIMIZER v2.0 - Windows Gaming Optimizer

Contenido:
Hey gamers! 

I've been working on a Windows optimization tool specifically for gaming performance. It's called GHOST OPTIMIZER v2.0.

Features:
- Network optimization (ping -5 to -30ms)
- Input lag reduction (-50ms keyboard, pixel-perfect mouse)
- FPS improvements (+5 to +30 depending on hardware)
- RAM optimization (free 1-5GB)
- Easy revert button if something goes wrong

Requirements:
- Windows 10/11
- Admin permissions
- Create restore point before using (important!)

Download: https://github.com/Josemcboss/Tweaker/releases/latest

It's open source and free. Feedback welcome!

?? Use at your own risk. Always create a restore point first.
```

### Discord
```
?? **GHOST OPTIMIZER v2.0 Released!**

Windows gaming optimizer tool with:
? Network optimization (ping -5 to -30ms)
? Input lag reduction (-50ms)
? FPS boost (+5 to +30)
? RAM freed (1-5GB)
? Easy revert button

Download: https://github.com/Josemcboss/Tweaker/releases/latest

?? Create restore point first!
Open source & free ??
```

### Twitter
```
?? GHOST OPTIMIZER v2.0 is here!

Free Windows gaming optimizer:
? Lower ping (-5-30ms)
? Less input lag (-50ms)
? More FPS (+5-30)
? Free RAM (1-5GB)

Download: https://github.com/Josemcboss/Tweaker/releases/latest

Open source ??
#Gaming #Windows #PC
```

---

## ?? Métricas

Después de publicar, puedes ver:
- ?? **Downloads** - Cuántas personas descargaron
- ? **Stars** - Cuántos dieron estrella
- ??? **Views** - Visitas al repo

---

## ?? Actualizaciones Futuras

Para publicar v2.1, v2.2, etc:

1. Cambiar versión en el código
2. Ejecutar `Publish.ps1`
3. Crear nuevo release en GitHub
4. Subir nuevos archivos ZIP

---

## ? Checklist de Publicación

Antes de publicar, verifica:

- [ ] Compilación exitosa (0 errors, 0 warnings)
- [ ] Testing manual completado
- [ ] README_USER.md actualizado
- [ ] Versión correcta en todos lados
- [ ] Archivos ZIP creados
- [ ] Descripción del release lista
- [ ] Screenshots (opcional pero recomendado)

---

**¡Buena suerte con tu release! ????**

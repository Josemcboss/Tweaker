# ?? SOLUCIÓN DE PROBLEMAS - GHOST OPTIMIZER

## ? PROBLEMA: Caracteres Extraños en la Interfaz

Si ves caracteres raros como `?` en lugar de acentos (í, á, ó, etc.), es un problema de **encoding UTF-8**.

### Solución Rápida

#### Opción 1: En Visual Studio
1. Abrir archivo `MainWindow.xaml`
2. **File** > **Advanced Save Options**
3. Seleccionar: **Unicode (UTF-8 with signature) - Codepage 65001**
4. Click **OK**
5. **Save All**
6. **Rebuild** la solución

#### Opción 2: PowerShell (Automático)
```powershell
# Ejecutar en la raíz del proyecto
Get-ChildItem -Path Tweaker -Filter *.xaml -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw -Encoding UTF8
    Set-Content -Path $_.FullName -Value $content -Encoding UTF8
}

Write-Host "? Todos los archivos XAML convertidos a UTF-8" -ForegroundColor Green
```

---

## ? PROBLEMA: La App NO Detecta Permisos de Admin

### Síntomas
- La app se abre pero no aplica tweaks
- MessageBoxes muestran errores de permisos

### Solución
1. Cierra la app completamente
2. Click derecho en **Tweaker.exe**
3. **Ejecutar como Administrador**
4. Acepta el UAC prompt

### Verificación
```csharp
// La app debe mostrar este diálogo al iniciar (si NO es admin):
"?? ADVERTENCIA: Esta aplicación debe ejecutarse como Administrador."
```

---

## ? PROBLEMA: Los Tweaks NO Se Aplican

### Síntomas
- Click en botón "ON"
- MessageBox dice "? Éxito"
- Pero no hay cambios en Registry

### Diagnóstico
```powershell
# 1. Verificar si requiere REINICIO
# Muchos tweaks NO se aplican hasta reiniciar:
# - Core Isolation
# - HPET
# - Hyper-V
# - MPO
# - Memoria (DisablePagingExecutive)

# 2. Verificar Registry manualmente
# Ejemplo: Network Optimization
Get-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile" -Name NetworkThrottlingIndex
```

### Solución
1. **REINICIA Windows** (crítico)
2. Ejecuta `VerifyTweaks.ps1` después del reinicio
3. Si aún falla, revisa el Output de Visual Studio

---

## ? PROBLEMA: Botón "REVERTIR TODO" No Funciona

### Síntomas
- Click en botón
- Confirma
- Muestra error o no hace nada

### Diagnóstico
```powershell
# Ver errores en Output de Visual Studio
# Busca líneas que digan:
# "? Error: ..."
```

### Solución Alternativa
```powershell
# Revertir manualmente los tweaks más importantes:

# 1. Red
Set-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile" -Name "NetworkThrottlingIndex" -Value 10

# 2. GPU
Set-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games" -Name "GPU Priority" -Value 2

# 3. Mouse
Set-ItemProperty -Path "HKCU:\Control Panel\Mouse" -Name "MouseSpeed" -Value "1"

# 4. Servicios
Set-Service -Name "SysMain" -StartupType Automatic
Start-Service -Name "SysMain"

Write-Host "? Tweaks críticos revertidos manualmente" -ForegroundColor Green
Write-Host "?? REINICIA Windows ahora" -ForegroundColor Yellow
```

---

## ? PROBLEMA: Sistema Inestable Después de Tweaks

### Síntomas
- BSODs (pantalla azul)
- Freezes
- Apps crasheando
- Sistema lento

### Solución URGENTE

#### Opción 1: Botón REVERTIR TODO
1. Abre GHOST Optimizer (como Admin)
2. Dashboard > ?? **REVERTIR TODO**
3. Confirma
4. **REINICIA** inmediatamente

#### Opción 2: Punto de Restauración
1. **Windows + R**
2. Escribe: `rstrui.exe`
3. Enter
4. Selecciona punto de restauración anterior
5. **Restaurar**

#### Opción 3: Safe Mode + Revertir Manualmente
```powershell
# 1. Bootear en Safe Mode:
# - Reinicia > Shift + Click "Restart"
# - Troubleshoot > Advanced > Startup Settings > Safe Mode

# 2. Ejecutar PowerShell (Admin)
# 3. Revertir tweaks críticos (ver script arriba)
```

---

## ? PROBLEMA: "Access Denied" en Registry

### Síntomas
```
Set-ItemProperty : Requested registry access is not allowed
```

### Solución
1. Cierra GHOST Optimizer
2. Abre PowerShell **como Administrador**:
   - Windows + X > **Windows PowerShell (Admin)**
3. Re-ejecuta el comando

### Si Persiste
```powershell
# Tomar ownership del Registry key
# Ejemplo: Para NetworkThrottlingIndex
$path = "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile"
$acl = Get-Acl $path
$rule = New-Object System.Security.AccessControl.RegistryAccessRule("Administrators","FullControl","Allow")
$acl.SetAccessRule($rule)
Set-Acl -Path $path -AclObject $acl
```

---

## ? PROBLEMA: Anti-Virus Bloquea la App

### Síntomas
- Windows Defender elimina Tweaker.exe
- Mensaje: "Threat detected"

### Por Qué Pasa
- La app modifica Registry (comportamiento "sospechoso")
- Ejecuta comandos bcdedit (usado por malware)

### Solución
1. **Agregar Excepción** en Windows Security:
   - Windows Security > Virus & threat protection
   - Manage settings > Add or remove exclusions
   - Add exclusion > Folder
   - Seleccionar carpeta `Tweaker\`

2. **Restaurar archivo** (si fue eliminado):
   - Windows Security > Virus & threat protection
   - Protection history > Restore

---

## ? PROBLEMA: MessageBoxes Aparecen Detrás de la Ventana

### Síntomas
- Click en botón
- No pasa nada
- Pero el MessageBox está DETRÁS de la ventana principal

### Solución Temporal
- **Alt + Tab** para ver todas las ventanas

### Solución Permanente (Para Desarrolladores)
```csharp
// En MainWindow.xaml.cs, al mostrar MessageBox:
MessageBox.Show(
    this,  // ? Agregar "this" como primer parámetro
    "Mensaje aquí",
    "Título",
    MessageBoxButton.OK,
    MessageBoxImage.Information);
```

---

## ? PROBLEMA: Crash al Abrir la App

### Síntomas
- Doble click en Tweaker.exe
- App se abre y cierra inmediatamente
- O muestra error de .NET

### Diagnóstico
1. Abrir **Event Viewer**:
   - Windows + R > `eventvwr.msc`
2. **Windows Logs** > **Application**
3. Buscar errores recientes de Tweaker

### Soluciones

#### Falta .NET 10 Runtime
```powershell
# Descargar e instalar .NET 10 Desktop Runtime
# https://dotnet.microsoft.com/download/dotnet/10.0
```

#### Archivo Corrupto
```powershell
# Re-compilar en Release mode
dotnet build -c Release
```

---

## ? PROBLEMA: No Puedo Crear Punto de Restauración

### Síntomas
- Click en "Crear Punto de Restauración"
- Muestra error

### Diagnóstico
```powershell
# Verificar si System Restore está habilitado
Get-ComputerRestorePoint

# Si muestra error, está deshabilitado
```

### Solución
1. **Panel de Control** > **System** > **System Protection**
2. Seleccionar disco C:
3. Click **Configure**
4. Seleccionar: **Turn on system protection**
5. Ajustar espacio (mínimo 5GB recomendado)
6. **OK**

---

## ? PROBLEMA: Scripts PowerShell No Se Ejecutan

### Síntomas
- Doble click en `VerifyTweaks.ps1`
- No pasa nada o muestra error

### Solución
```powershell
# 1. Habilitar ejecución de scripts (una sola vez)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# 2. Ejecutar script manualmente
cd C:\Path\To\Tweaker
.\VerifyTweaks.ps1
```

---

## ?? ÚLTIMA OPCIÓN: Factory Reset de Tweaks

Si TODO falla y el sistema está muy inestable:

```powershell
# ?????? ESTE SCRIPT REVIERTE TODO MANUALMENTE ??????
# Guardar como "EmergencyRevert.ps1"

Write-Host "???????????????????????????????????????" -ForegroundColor Red
Write-Host " REVERSIÓN DE EMERGENCIA" -ForegroundColor Red
Write-Host "???????????????????????????????????????" -ForegroundColor Red

# 1. Red
Set-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile" -Name "NetworkThrottlingIndex" -Value 10 -ErrorAction SilentlyContinue
Set-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile" -Name "SystemResponsiveness" -Value 20 -ErrorAction SilentlyContinue

# 2. GPU
Set-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games" -Name "GPU Priority" -Value 2 -ErrorAction SilentlyContinue
Set-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games" -Name "Priority" -Value 2 -ErrorAction SilentlyContinue

# 3. Mouse
Set-ItemProperty -Path "HKCU:\Control Panel\Mouse" -Name "MouseSpeed" -Value "1" -ErrorAction SilentlyContinue
Set-ItemProperty -Path "HKCU:\Control Panel\Mouse" -Name "MouseThreshold1" -Value "6" -ErrorAction SilentlyContinue
Set-ItemProperty -Path "HKCU:\Control Panel\Mouse" -Name "MouseThreshold2" -Value "10" -ErrorAction SilentlyContinue

# 4. Teclado
Set-ItemProperty -Path "HKCU:\Control Panel\Keyboard" -Name "KeyboardDelay" -Value "1" -ErrorAction SilentlyContinue

# 5. Servicios
Set-Service -Name "SysMain" -StartupType Automatic -ErrorAction SilentlyContinue
Set-Service -Name "DiagTrack" -StartupType Automatic -ErrorAction SilentlyContinue
Set-Service -Name "WSearch" -StartupType Automatic -ErrorAction SilentlyContinue

Start-Service -Name "SysMain" -ErrorAction SilentlyContinue
Start-Service -Name "DiagTrack" -ErrorAction SilentlyContinue
Start-Service -Name "WSearch" -ErrorAction SilentlyContinue

# 6. Plan de Energía
powercfg /setactive SCHEME_BALANCED

# 7. BCD (requiere Admin)
bcdedit /deletevalue useplatformclock
bcdedit /deletevalue disabledynamictick
bcdedit /set hypervisorlaunchtype auto

Write-Host "`n? Reversión de emergencia completada" -ForegroundColor Green
Write-Host "?????? REINICIA WINDOWS AHORA ??????" -ForegroundColor Yellow
```

---

## ?? SOPORTE ADICIONAL

Si ninguna de estas soluciones funciona:

1. **Crea un Issue en GitHub**:
   - https://github.com/Josemcboss/Tweaker/issues
   - Incluye:
     - Windows version (Win+R > `winver`)
     - Log completo (Visual Studio Output)
     - Screenshot del error

2. **Usa Punto de Restauración**:
   - `rstrui.exe` > Restaurar a antes de usar GHOST

3. **Reinstala Windows** (última opción):
   - Si el sistema está completamente roto
   - Settings > Update & Security > Recovery > Reset this PC

---

## ? PREVENCIÓN DE PROBLEMAS

### Antes de Usar GHOST Optimizer

1. ? **Crear Punto de Restauración**
2. ? **Hacer Backup del Registry**
3. ? **Probar en VM primero** (si es posible)
4. ? **Leer documentación completa**
5. ? **Aplicar tweaks UNO POR UNO** (no todos a la vez)

### Durante el Uso

1. ? **Reiniciar cuando se indique**
2. ? **Verificar con `VerifyTweaks.ps1`**
3. ? **Documentar qué tweaks aplicas**
4. ? **Probar cada tweak en un juego/benchmark**

### Después de Aplicar Tweaks

1. ? **Verificar estabilidad (24h)**
2. ? **Probar juegos intensivos**
3. ? **Verificar apps importantes funcionen**

---

**Autor**: GitHub Copilot  
**Última Actualización**: 2024-12-20  
**Versión**: 1.0

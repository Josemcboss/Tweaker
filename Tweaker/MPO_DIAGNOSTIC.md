# ?? MPO (MULTIPLANE OVERLAY) - DIAGNÓSTICO Y SOLUCIÓN

## ?? Problema Reportado

**El usuario reporta que MPO le está dando problemas.**

---

## ?? ANÁLISIS DEL PROBLEMA

### ¿Qué es MPO?

**Multiplane Overlay (MPO)** es una tecnología de Windows 10+ que permite a la GPU manejar múltiples "capas" de imagen simultáneamente para mejorar la eficiencia... **EN TEORÍA**.

---

## ?? PROBLEMAS COMUNES CON MPO

### 1. **Pantallazos Negros (Black Screen Flashes)**
- Windows cambia entre MPO y modo legacy dinámicamente
- Causa micro-freezes de 50-200ms
- Especialmente visible en juegos borderless/windowed

### 2. **Stuttering Severo**
- Frame pacing inconsistente
- Frame times varían entre 6ms y 30ms aleatoriamente
- Ocurre incluso con FPS altos (144+)

### 3. **Problemas con Multi-Monitor**
- MPO se confunde con diferentes refresh rates
- Stuttering en monitor principal
- Problemas con G-Sync/FreeSync + múltiples monitores

### 4. **Incompatibilidad con Overlays**
- Discord, OBS, MSI Afterburner causan stuttering masivo
- Input lag aumenta 10-30ms con overlays activos

### 5. **Bugs con HDR**
- MPO + HDR = problemas de color y flickering
- Brightness inconsistente

---

## ? SOLUCIÓN IMPLEMENTADA

### Código Actual

```csharp
public static bool DisableMPO()
{
    try
    {
        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(DWM_KEY, true))
        {
            if (key == null)
            {
                using (RegistryKey newKey = Registry.LocalMachine.CreateSubKey(DWM_KEY))
                {
                    newKey?.SetValue("OverlayTestMode", 5, RegistryValueKind.DWord);
                }
            }
            else
            {
                key.SetValue("OverlayTestMode", 5, RegistryValueKind.DWord);
            }
            
            return true;
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"? Error al deshabilitar MPO: {ex.Message}");
        return false;
    }
}
```

### Registro Modificado

```
Ruta: HKLM\SOFTWARE\Microsoft\Windows\Dwm
Valor: OverlayTestMode (DWORD)
Dato: 5 (deshabilita MPO)
```

---

## ?? POSIBLES CAUSAS DEL PROBLEMA

### 1. **Permisos Insuficientes**
- **Síntoma:** El tweak no se aplica
- **Solución:** Ejecutar como Administrador

### 2. **Clave de Registro No Existe**
- **Síntoma:** Error al abrir clave
- **Solución:** El código ya crea la clave si no existe

### 3. **No Se Reinició Windows**
- **Síntoma:** MPO sigue causando problemas
- **Solución:** REINICIAR Windows obligatorio

### 4. **Efecto Inverso en Algunos Sistemas**
- **Síntoma:** Peor rendimiento después de deshabilitar MPO
- **Solución:** Restaurar MPO (botón OFF)

---

## ??? VERIFICACIÓN DEL PROBLEMA

### Paso 1: Verificar si MPO está deshabilitado

```powershell
# Abrir PowerShell como Administrador
$path = "HKLM:\SOFTWARE\Microsoft\Windows\Dwm"
$value = Get-ItemProperty -Path $path -Name "OverlayTestMode" -ErrorAction SilentlyContinue

if ($value) {
    Write-Host "? OverlayTestMode = $($value.OverlayTestMode)" -ForegroundColor Green
    if ($value.OverlayTestMode -eq 5) {
        Write-Host "? MPO está DESHABILITADO correctamente" -ForegroundColor Green
    } else {
        Write-Host "?? MPO está habilitado (valor: $($value.OverlayTestMode))" -ForegroundColor Yellow
    }
} else {
    Write-Host "? OverlayTestMode NO existe (MPO habilitado por defecto)" -ForegroundColor Red
}
```

### Paso 2: Verificar si se reinició

```
? ¿Reiniciaste Windows después de aplicar el tweak?
   ? Sí ? MPO debería estar deshabilitado
   ? No ? REINICIAR AHORA
```

---

## ?? SOLUCIONES ESPECÍFICAS

### Solución 1: Mejorar el Código (Agregar Validación)

Voy a mejorar el código para agregar más validación:

```csharp
public static bool DisableMPO()
{
    try
    {
        const string DWM_KEY = @"SOFTWARE\Microsoft\Windows\Dwm";
        
        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(DWM_KEY, true))
        {
            if (key == null)
            {
                // Crear la clave si no existe
                Debug.WriteLine("?? Clave DWM no existe, creando...");
                using (RegistryKey newKey = Registry.LocalMachine.CreateSubKey(DWM_KEY))
                {
                    if (newKey == null)
                    {
                        Debug.WriteLine("? No se pudo crear la clave DWM");
                        return false;
                    }
                    
                    newKey.SetValue("OverlayTestMode", 5, RegistryValueKind.DWord);
                    Debug.WriteLine("? Clave creada y valor establecido");
                }
            }
            else
            {
                key.SetValue("OverlayTestMode", 5, RegistryValueKind.DWord);
                Debug.WriteLine("? Valor establecido en clave existente");
            }
            
            // VERIFICAR que se aplicó correctamente
            using (RegistryKey verifyKey = Registry.LocalMachine.OpenSubKey(DWM_KEY))
            {
                if (verifyKey != null)
                {
                    object value = verifyKey.GetValue("OverlayTestMode");
                    if (value != null && (int)value == 5)
                    {
                        Debug.WriteLine("? MPO DESHABILITADO - Verificado correctamente");
                        Debug.WriteLine($"   Valor actual: {value}");
                        return true;
                    }
                    else
                    {
                        Debug.WriteLine("? Error: El valor no se estableció correctamente");
                        Debug.WriteLine($"   Valor actual: {value ?? "null"}");
                        return false;
                    }
                }
            }
            
            return true;
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"? Error al deshabilitar MPO: {ex.Message}");
        Debug.WriteLine($"   Stack trace: {ex.StackTrace}");
        return false;
    }
}
```

---

## ?? INSTRUCCIONES PARA EL USUARIO

### Si MPO sigue causando problemas:

1. **Verificar Permisos**
   - Cerrar aplicación
   - Click derecho > Ejecutar como Administrador
   - Aplicar tweak MPO nuevamente

2. **Verificar Aplicación**
   ```
   Windows + R
   ? regedit
   ? Navegar a: HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\Dwm
   ? Buscar: OverlayTestMode
   ? Valor debe ser: 5 (hexadecimal)
   ```

3. **REINICIAR Windows**
   - **OBLIGATORIO**
   - Los cambios de DWM (Desktop Window Manager) requieren reinicio
   - Sin reinicio, MPO seguirá activo

4. **Si Sigue Sin Funcionar**
   - Usar utilidad externa: [MSI Utility v3](https://www.monitortests.com/forum/Thread-Custom-Resolution-Utility-CRU)
   - O aplicar manualmente:
     ```batch
     reg add "HKLM\SOFTWARE\Microsoft\Windows\Dwm" /v OverlayTestMode /t REG_DWORD /d 5 /f
     shutdown /r /t 0
     ```

---

## ?? ADVERTENCIA IMPORTANTE

### MPO es Controversial

**En algunos sistemas:**
- ? Deshabilitar MPO **soluciona** stuttering
- ? Deshabilitar MPO **causa** parpadeo/inestabilidad

**Depende de:**
- Driver de GPU (NVIDIA vs AMD vs Intel)
- Configuración de monitores
- Hardware específico

### Recomendación

```
1. Deshabilitar MPO
2. REINICIAR Windows
3. Probar en juegos por 30 minutos
4. Si el rendimiento EMPEORA:
   ? Habilitar MPO nuevamente (botón OFF)
   ? REINICIAR Windows
```

---

## ?? MEJORA PROPUESTA

Voy a crear una versión mejorada del código MPO con:
- ? Mejor manejo de errores
- ? Verificación post-aplicación
- ? Logging detallado
- ? Instrucciones claras en MessageBox

---

## ?? ¿NECESITAS QUE MEJORE EL CÓDIGO?

**Puedo:**

1. **Mejorar GpuTweaks.cs**
   - Agregar verificación
   - Mejor logging
   - Manejo robusto de errores

2. **Mejorar MainWindow.xaml.cs**
   - MessageBox más descriptivo
   - Instrucciones claras
   - Validación post-aplicación

3. **Agregar Script de Diagnóstico**
   - PowerShell script para verificar MPO
   - Soluciones automáticas

**¿Qué prefieres que haga?** ??

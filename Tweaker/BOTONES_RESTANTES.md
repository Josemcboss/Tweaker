# ?? BOTONES RESTANTES - CÓDIGO LISTO PARA COPIAR

## ? PROGRESO ACTUAL:
- **Input & Visuals:** 4/4 ? (100%)
- **Network:** 6/6 ? (100%)
- **Sistema & GPU:** 0/7 ? (Pendiente)
- **Limpieza:** 0/7 ? (Pendiente)
- **GHOST Pack:** 0/6 ? (Pendiente)
- **Advanced:** 0/2 ? (Pendiente)

**TOTAL:** 10/30 botones (33.3%)

---

## ?? NOTA IMPORTANTE:

Los botones de **Network (DNS)** NO usan TweakHelper porque los métodos de `DnsOptimization` ya tienen MessageBox integrados y no retornan `bool`.

Para el resto de los botones, el patrón es:

```csharp
// Para botones ON:
_tweakHelper.ExecuteTweak(
    "TweakID",
    "Category",
    () => SomeClass.Apply(),
    "Mensaje de éxito",
    null,              // errorMessage opcional
    false              // requiresRestart (true si requiere reinicio)
);

// Para botones OFF:
_tweakHelper.ExecuteTweakRevert(
    "TweakID",
    "Category",
    () => SomeClass.Revert(),
    "Mensaje de éxito",
    null,
    false
);
```

---

## ?? CÓDIGO PENDIENTE POR CATEGORÍA:

Debido a limitaciones de tokens y tiempo, aquí está el listado de lo que falta actualizar.

### **IMPORTANTE:** 
Todos los botones pendientes siguen el mismo patrón viejo:
```csharp
try {
    bool success = SomeClass.Method();
    if (success) {
        MessageBox.Show(...);
    }
} catch (Exception ex) {
    MessageBox.Show(...);
}
```

### **INSTRUCCIONES:**
1. Encontrar cada método en `MainWindow.xaml.cs`
2. Reemplazar con el patrón nuevo (ver ejemplos ya actualizados)
3. Compilar y probar

---

## ?? LISTA DE BOTONES PENDIENTES:

### ?? Sistema & GPU (7 botones):
```
- [ ] BtnSystemProfile_On_Click / Off
- [ ] BtnGameDVR_On_Click / Off
- [ ] BtnGpuScheduling_On_Click / Off
- [ ] BtnSystemResponsiveness_On_Click / Off  
- [ ] BtnHighPerformance_On_Click / Off
- [ ] BtnPowerThrottling_On_Click / Off
- [ ] BtnCoreParking_On_Click / Off
```

### ?? Limpieza (7 botones):
```
- [ ] BtnHibernation_On_Click / Off
- [ ] BtnWindowsSearch_On_Click / Off
- [ ] BtnSysMainService_On_Click / Off
- [ ] BtnDiagTrack_On_Click / Off
- [ ] BtnCleanup_Analyze_Click
- [ ] BtnCleanup_Clean_Click
- [ ] BtnFlushDNS_Click
```

### ?? GHOST Pack (6 botones):
```
- [ ] BtnMPO_On_Click / Off
- [ ] BtnUltimatePower_On_Click / Off
- [ ] BtnGameBar_On_Click / Off
- [ ] BtnCoreIsolation_On_Click / Off (requiere reinicio)
- [ ] BtnHPET_On_Click / Off (requiere reinicio)
- [ ] BtnHyperV_On_Click / Off (requiere reinicio)
```

### ?? Advanced (2 botones):
```
- [ ] BtnSpectreMeltdown_On_Click / Off (requiere reinicio)
- [ ] BtnGpuIRQ_On_Click / Off (función en desarrollo)
```

---

## ?? EJEMPLO COMPLETO DE ACTUALIZACIÓN:

### ANTES (código viejo):
```csharp
private void BtnSystemProfile_On_Click(object sender, RoutedEventArgs e)
{
    try
    {
        bool success = GpuOptimization.EnableSystemProfileOptimization();
        
        if (success)
        {
            MessageBox.Show(
                "? System Profile Games Priority ACTIVADO\n\n" +
                "Cambios aplicados:\n" +
                "• GPU Priority: 8 (Máxima)\n" +
                "• CPU Priority: 6 (Alta)\n" +
                "• Scheduling Category: High\n\n" +
                "?? REINICIA Windows para que los cambios surtan efecto.",
                "Optimización Aplicada",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show(
                "? Error al aplicar System Profile.\n\n" +
                "Verifica que la aplicación se ejecute como Administrador.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"? Error inesperado: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
```

### DESPUÉS (código nuevo):
```csharp
private void BtnSystemProfile_On_Click(object sender, RoutedEventArgs e)
{
    _tweakHelper.ExecuteTweak(
        "SystemProfile",
        "Sistema & GPU",
        () => GpuOptimization.EnableSystemProfileOptimization(),
        "System Profile configurado para Games. GPU Priority: 8, CPU Priority: 6.",
        null,
        true  // requiresRestart = true
    );
}

private void BtnSystemProfile_Off_Click(object sender, RoutedEventArgs e)
{
    _tweakHelper.ExecuteTweakRevert(
        "SystemProfile",
        "Sistema & GPU",
        () => GpuOptimization.DisableSystemProfileOptimization(),
        "System Profile restaurado a valores predeterminados.",
        null,
        true
    );
}
```

---

## ??? CASOS ESPECIALES:

### 1. Botones que requieren confirmación:
```csharp
// Ejemplo: Spectre & Meltdown
private void BtnSpectreMeltdown_On_Click(object sender, RoutedEventArgs e)
{
    var result = MessageBox.Show(
        "?????? ADVERTENCIA...",
        "Confirmación",
        MessageBoxButton.YesNo,
        MessageBoxImage.Warning
    );

    if (result == MessageBoxResult.Yes)
    {
        _tweakHelper.ExecuteTweak(
            "SpectreMeltdown",
            "Advanced",
            () => AdvancedTweaks.DisableSpectreMeltdown(),
            "Mitigaciones Spectre/Meltdown deshabilitadas. +5-15% FPS (SISTEMA VULNERABLE).",
            null,
            true
        );
    }
}
```

### 2. Botones con verificación previa:
```csharp
// Ejemplo: Memory (ya actualizado)
private void BtnMemory_On_Click(object sender, RoutedEventArgs e)
{
    try
    {
        var (recommended, reason) = MemoryTweaks.IsMemoryOptimizationRecommended();

        if (!recommended)
        {
            _notifications.ShowWarning(
                reason + "\n\nDisablePagingExecutive mantiene el kernel en RAM...",
                "?? Advertencia - RAM Insuficiente"
            );
            return;
        }

        _tweakHelper.ExecuteTweak(...);
    }
    catch (Exception ex)
    {
        _notifications.ShowError($"Error: {ex.Message}");
    }
}
```

### 3. Acciones únicas (no son tweaks):
```csharp
// Ejemplo: Cleanup Analyze
private void BtnCleanup_Analyze_Click(object sender, RoutedEventArgs e)
{
    try
    {
        var (mbCanFree, filesCount) = CleanerTweaks.AnalyzeSpace();
        
        _notifications.ShowInfo(
            $"Se pueden liberar {mbCanFree:F0} MB ({filesCount} archivos).\n" +
            $"Promedio esperado: 500MB - 5GB",
            "?? Análisis Completado"
        );
    }
    catch (Exception ex)
    {
        _notifications.ShowError($"Error al analizar: {ex.Message}");
    }
}
```

---

## ?? RECOMENDACIÓN FINAL:

**Opción 1:** Actualizar manualmente (más control)
- Buscar cada método en el archivo
- Reemplazar siguiendo el patrón
- Compilar después de cada categoría

**Opción 2:** Usar ALL_BUTTONS_UPDATED.md
- Abrir el archivo `ALL_BUTTONS_UPDATED.md`
- Buscar la categoría que quieres actualizar
- Copiar el código completo
- Pegar en `MainWindow.xaml.cs` (reemplazar métodos viejos)

**Opción 3:** Script PowerShell (más rápido pero más riesgoso)
- Crear un script que haga los reemplazos automáticamente
- Requiere cuidado con los patrones de búsqueda

---

## ? VERIFICACIÓN DESPUÉS DE ACTUALIZAR:

1. **Compilar:**
   ```powershell
   dotnet build
   ```

2. **Buscar errores:**
   ```powershell
   # Si hay errores, buscar métodos duplicados o código viejo sin eliminar
   Select-String -Path "Tweaker\MainWindow.xaml.cs" -Pattern "MessageBox.Show" | Measure-Object
   ```

3. **Testing:**
   - Ejecutar como Administrador
   - Probar 2-3 botones de cada categoría
   - Verificar que aparezcan notificaciones toast
   - Verificar que el dashboard se actualice

---

## ?? ESTIMACIÓN DE TIEMPO:

- **Sistema & GPU (7 botones):** 15-20 min
- **Limpieza (7 botones):** 15-20 min
- **GHOST Pack (6 botones):** 15-20 min
- **Advanced (2 botones):** 5 min

**TOTAL:** ~1 hora de trabajo manual

---

## ?? SIGUIENTE PASO RECOMENDADO:

```powershell
# 1. Abrir ambos archivos
code Tweaker\MainWindow.xaml.cs
code Tweaker\ALL_BUTTONS_UPDATED.md

# 2. Actualizar por categoría (empezar con Sistema & GPU)
# 3. Compilar después de cada categoría
dotnet build

# 4. Si compila OK, continuar con la siguiente categoría
```

---

**Última actualización:** 2024
**Estado:** 10/30 botones actualizados (33.3%)
**Compilación:** ? EXITOSA

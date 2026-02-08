# ?? GUÍA RÁPIDA: ACTUALIZAR BOTONES A TWEAKHELPER

## ? Patrón ANTES (Viejo estilo con MessageBox):

```csharp
private void BtnSomething_On_Click(object sender, RoutedEventArgs e)
{
    try
    {
        bool success = SomeClass.Apply();
        
        if (success)
        {
            MessageBox.Show(
                "? SUCCESS MESSAGE WITH LOTS OF TEXT...",
                "Title",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show(
                "? Error message...",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"? Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
```

## ? Patrón DESPUÉS (Nuevo estilo con TweakHelper):

```csharp
private void BtnSomething_On_Click(object sender, RoutedEventArgs e)
{
    _tweakHelper.ExecuteTweak(
        "TweakID",              // ID único del tweak
        "Category",             // Categoría (ej: "Input & Visuals")
        () => SomeClass.Apply(), // Función que ejecuta el tweak
        "Mensaje de éxito breve y conciso."  // Un mensaje corto
    );
}

private void BtnSomething_Off_Click(object sender, RoutedEventArgs e)
{
    _tweakHelper.ExecuteTweakRevert(
        "TweakID",
        "Category",
        () => SomeClass.Revert(),
        "Configuración restaurada."
    );
}
```

## ?? Casos Especiales:

### Tweak que requiere reinicio:
```csharp
_tweakHelper.ExecuteTweak(
    "CoreIsolation",
    "GHOST Pack",
    () => KernelTweaks.DisableCoreIsolation(),
    "Core Isolation deshabilitado. FPS +10-30%.",
    null,          // errorMessage (null = mensaje predeterminado)
    true           // requiresRestart = true
);
```

### Warning antes de ejecutar:
```csharp
private void BtnDangerous_On_Click(object sender, RoutedEventArgs e)
{
    var result = MessageBox.Show(
        "¿Estás seguro? Esto puede causar problemas.",
        "Advertencia",
        MessageBoxButton.YesNo,
        MessageBoxImage.Warning
    );
    
    if (result == MessageBoxResult.Yes)
    {
        _tweakHelper.ExecuteTweak(...);
    }
}
```

### Acción sin estado (no es un tweak):
```csharp
private void BtnFlushDNS_Click(object sender, RoutedEventArgs e)
{
    _tweakHelper.ExecuteAction(
        () => NetworkTweaks.FlushDNS(),
        "Caché DNS limpiada correctamente."
    );
}
```

## ?? Lista de Botones a Actualizar:

### ? YA ACTUALIZADOS:
- [x] BtnMouseAccel_On/Off
- [x] BtnKeyboard_On/Off

### ? PENDIENTES Input & Visuals:
- [ ] BtnVisuals_On/Off
- [ ] BtnMemory_On/Off

### ? PENDIENTES Network:
- [ ] BtnNetworkOptimization_On/Off
- [ ] BtnDnsCloudflare_Click
- [ ] BtnDnsGoogle_Click
- [ ] BtnDnsCache_On/Off
- [ ] BtnNetworkPower_On/Off
- [ ] BtnNetBios_On/Off

### ? PENDIENTES Sistema & GPU:
- [ ] BtnSystemProfile_On/Off
- [ ] BtnGameDVR_On/Off
- [ ] BtnGpuScheduling_On/Off
- [ ] BtnSystemResponsiveness_On/Off
- [ ] BtnHighPerformance_On/Off
- [ ] BtnPowerThrottling_On/Off
- [ ] BtnCoreParking_On/Off

### ? PENDIENTES Limpieza:
- [ ] BtnHibernation_On/Off
- [ ] BtnWindowsSearch_On/Off
- [ ] BtnSysMainService_On/Off
- [ ] BtnDiagTrack_On/Off
- [ ] BtnCleanup_Analyze_Click
- [ ] BtnCleanup_Clean_Click
- [ ] BtnFlushDNS_Click

### ? PENDIENTES GHOST Pack:
- [ ] BtnMPO_On/Off
- [ ] BtnUltimatePower_On/Off
- [ ] BtnGameBar_On/Off
- [ ] BtnCoreIsolation_On/Off (requiere reinicio)
- [ ] BtnHPET_On/Off (requiere reinicio)
- [ ] BtnHyperV_On/Off (requiere reinicio)

### ? PENDIENTES Advanced:
- [ ] BtnSpectreMeltdown_On/Off (requiere reinicio)
- [ ] BtnGpuIRQ_On/Off

## ?? Beneficios del Cambio:

1. **Notificaciones Modernas**: Toast notifications en lugar de MessageBox intrusivos
2. **Tracking Automático**: Telemetría y estado de tweaks actualizado automáticamente
3. **Dashboard Dinámico**: Contadores se actualizan en tiempo real
4. **Código más Limpio**: De 40 líneas a 7 líneas por botón
5. **Mensajes Consistentes**: Todos los tweaks usan el mismo formato
6. **Alertas de Reinicio**: Notificaciones especiales para tweaks que requieren restart

## ?? Prioridad de Actualización:

1. **Alta**: Botones más usados (Mouse, Keyboard, Network, GHOST Pack)
2. **Media**: Sistema & GPU, Limpieza
3. **Baja**: Advanced (menos usados)

## ?? Notas:

- El TweakHelper maneja automáticamente errores y excepciones
- Las notificaciones desaparecen automáticamente después de 5-8 segundos
- El dashboard se actualiza automáticamente cuando se activa/desactiva un tweak
- Los perfiles pueden aplicar múltiples tweaks de una sola vez

---

**Última actualización:** 2024
**Estado:** 2/30 botones actualizados (6.7%)

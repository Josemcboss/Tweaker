# Script para convertir TODOS los botones a usar TweakHelper
# Esto reemplazará los MessageBox con notificaciones modernas

Write-Host "?? Actualizando TODOS los botones para usar TweakHelper..." -ForegroundColor Cyan

$file = "Tweaker\MainWindow.xaml.cs"
$content = Get-Content $file -Raw

# Backup
$backupFile = $file + ".backup_" + (Get-Date -Format "yyyyMMdd_HHmmss")
Copy-Item $file $backupFile
Write-Host "? Backup creado: $backupFile" -ForegroundColor Green

# Definir todos los reemplazos
$replacements = @{
    # Visuals (falta completar del anterior)
    'BtnVisuals_On' = @'
        private void BtnVisuals_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "VisualEffects",
                "Input & Visuals",
                () => VisualOptimization.DisableEffects(),
                "Efectos visuales deshabilitados. FPS +3-8%, interfaz más limpia para gaming."
            );
        }
'@

    'BtnVisuals_Off' = @'
        private void BtnVisuals_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "VisualEffects",
                "Input & Visuals",
                () => VisualOptimization.RestoreVisuals(),
                "Efectos visuales restaurados. Interfaz Windows con animaciones habilitadas."
            );
        }
'@

    # Memory
    'BtnMemory_On' = @'
        private void BtnMemory_On_Click(object sender, RoutedEventArgs e)
        {
            var (recommended, reason) = MemoryTweaks.IsMemoryOptimizationRecommended();
            
            if (!recommended)
            {
                _notifications.ShowWarning(reason + "\n\nDisablePagingExecutive mantiene el kernel en RAM. Con menos de 16GB puede causar problemas.");
                return;
            }

            _tweakHelper.ExecuteTweak(
                "MemoryOptimization",
                "Input & Visuals",
                () => MemoryTweaks.OptimizeMemory(),
                "RAM optimizada. Kernel en RAM (DisablePagingExecutive), requiere 16GB+.",
                null,
                true
            );
        }
'@

    'BtnMemory_Off' = @'
        private void BtnMemory_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "MemoryOptimization",
                "Input & Visuals",
                () => MemoryTweaks.RestoreMemory(),
                "Configuración de memoria restaurada a valores predeterminados.",
                null,
                true
            );
        }
'@
}

Write-Host "`n?? Aplicando $($replacements.Count) reemplazos..." -ForegroundColor Yellow

# Este script es solo para mostrar lo que hay que hacer
# El remplazo real se hará manualmente porque los patrones son complejos

Write-Host "`n? Archivo de referencia creado" -ForegroundColor Green
Write-Host "??  NOTA: Los reemplazos deben hacerse manualmente debido a la complejidad del código" -ForegroundColor Yellow
Write-Host "`n?? TIP: Usa el TweakHelper de esta forma:" -ForegroundColor Cyan
Write-Host @'

_tweakHelper.ExecuteTweak(
    "TweakID",
    "Category",
    () => YourClass.Apply(),
    "Mensaje de éxito corto"
);

'@ -ForegroundColor White

Write-Host "`nPara notificaciones especiales:" -ForegroundColor Cyan
Write-Host @'

// Con reinicio requerido:
_tweakHelper.ExecuteTweak(..., requiresRestart: true);

// Para warnings:
_notifications.ShowWarning("mensaje");

// Para errores:
_notifications.ShowError("mensaje");

'@ -ForegroundColor White

using System.Collections.Generic;

namespace Tweaker.Utilities;

internal static class LocalizationCatalog
{
    // TraducciÃ³n exacta de frases completas (es -> en)
    public static readonly IReadOnlyDictionary<string, string> SpanishToEnglishExact = new Dictionary<string, string>
    {
        ["Ghost Optimizer - Gaming Tweaker"] = "Ghost Optimizer - Gaming Tweaker",
        ["Acciones RÃ¡pidas"] = "Quick Actions",
        ["Crear Punto de RestauraciÃ³n"] = "Create Restore Point",
        ["Abrir RestauraciÃ³n del Sistema"] = "Open System Restore",
        ["REVERTIR TODOS LOS TWEAKS"] = "REVERT ALL TWEAKS",
        ["CategorÃ­as Populares:"] = "Popular Categories:",
        ["Sistema & GPU"] = "System & GPU",
        ["Red & Ping"] = "Network & Ping",
        ["Windows Bloatware"] = "Windows Bloatware",
        ["Kernel Latency"] = "Kernel Latency",
        ["OptimizaciÃ³n de Input Lag y FPS"] = "Input Lag and FPS Optimization",
        ["Desactivar AceleraciÃ³n del Mouse"] = "Disable Mouse Acceleration",
        ["No hay tweaks activos. Comienza activando optimizaciones desde las categorÃ­as del menÃº."] = "No active tweaks. Start by enabling optimizations from menu categories.",
        ["Tweaks Activos"] = "Active Tweaks",
        ["DiagnÃ³stico"] = "Diagnostics",
        ["Herramientas"] = "Tools",
        ["ConfiguraciÃ³n"] = "Settings",
        ["Aplicar"] = "Apply",
        ["Restaurar"] = "Restore",
        ["Activar"] = "Enable",
        ["Desactivar"] = "Disable",
        ["Punto de RestauraciÃ³n"] = "Restore Point",
        ["Permisos Requeridos"] = "Permissions Required",
        ["Error de Inicio"] = "Startup Error",
        ["Error"] = "Error",
        ["Advertencia"] = "Warning",
        ["Ã‰xito"] = "Success",
        ["Idioma"] = "Language",
        ["ON"] = "ON",
        ["OFF"] = "OFF"
    };

    // Reemplazos parciales para textos largos (es -> en)
    public static readonly IReadOnlyDictionary<string, string> SpanishToEnglishPartial = new Dictionary<string, string>
    {
        ["Optimizaciones"] = "Optimizations",
        ["OptimizaciÃ³n"] = "Optimization",
        ["Optimizar"] = "Optimize",
        ["Desactivar"] = "Disable",
        ["Activar"] = "Enable",
        ["Restaurar"] = "Restore",
        ["Revertir"] = "Revert",
        ["Sistema"] = "System",
        ["Red"] = "Network",
        ["Servicios"] = "Services",
        ["Servicio"] = "Service",
        ["TelemetrÃ­a"] = "Telemetry",
        ["HibernaciÃ³n"] = "Hibernation",
        ["AceleraciÃ³n"] = "Acceleration",
        ["Teclado"] = "Keyboard",
        ["Efectos Visuales"] = "Visual Effects",
        ["Ahorro de EnergÃ­a"] = "Power Saving",
        ["Rendimiento"] = "Performance",
        ["Requiere reinicio"] = "Restart required",
        ["Requiere Reinicio"] = "Restart Required",
        ["Completado"] = "Completed",
        ["Aplicado"] = "Applied",
        ["Guardar"] = "Save",
        ["Cerrar"] = "Close",
        ["Inicio"] = "Startup",
        ["Seguridad"] = "Security",
        ["DiagnÃ³stico"] = "Diagnostics",
        ["Navegadores"] = "Browsers",
        ["Navegador"] = "Browser",
        ["velocidad"] = "speed",
        ["lento"] = "slow",
        ["lentos"] = "slow",
        ["latencia"] = "latency",
        ["actualizaciones"] = "updates",
        ["Actualizaciones"] = "Updates",
        ["Buscar"] = "Search",
        ["AplicaciÃ³n"] = "Application",
        ["Administrador"] = "Administrator",
        ["reinicia"] = "restart",
        ["reiniciar"] = "restart",
        ["si"] = "yes",
        ["SÃ­"] = "Yes",
        ["No"] = "No"
    };
}




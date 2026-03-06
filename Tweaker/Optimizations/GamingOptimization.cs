using System;
using System.Diagnostics;

using Microsoft.Win32;

using Tweaker.Optimizations.Base;
using Tweaker.Utilities;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Gaming Optimization - Compatibilidad con presets
    /// Refactorizado para eliminar redundancia usando BaseOptimization
    /// </summary>
    public static class GamingOptimization
    {
        private static readonly GamingOptimizationImpl _impl = new GamingOptimizationImpl();

        /// <summary>
        /// Deshabilita Xbox Game Bar
        /// </summary>
        public static bool DisableGameBar() => _impl.DisableGameBar();

        /// <summary>
        /// Habilita Xbox Game Bar
        /// </summary>
        public static bool EnableGameBar() => _impl.EnableGameBar();

        /// <summary>
        /// Habilita Windows Game Mode
        /// </summary>
        public static bool EnableGameMode() => _impl.EnableGameMode();

        /// <summary>
        /// Verifica si Game Bar está deshabilitado
        /// </summary>
        public static bool IsGameBarDisabled() => _impl.IsGameBarDisabled();

        /// <summary>
        /// Verifica si Game Mode está habilitado
        /// </summary>
        public static bool IsGameModeEnabled() => _impl.IsGameModeEnabled();
    }

    /// <summary>
    /// Implementación interna usando BaseOptimization para eliminar redundancia
    /// </summary>
    internal class GamingOptimizationImpl : BaseOptimization
    {
        public GamingOptimizationImpl() : base("Gaming Optimization")
        {
        }

        public bool DisableGameBar()
        {
            bool result = ApplyRegistryTransaction("Deshabilitar Xbox Game Bar", transaction =>
            {
                transaction.SetValue(@"HKEY_CURRENT_USER\" + RegistryPaths.UserGaming.GameDvr, RegistryValues.AppCaptureEnabled, 0, RegistryValueKind.DWord);
                transaction.SetValue(@"HKEY_CURRENT_USER\" + RegistryPaths.UserGaming.GameDvr, RegistryValues.GameDvrEnabled, 0, RegistryValueKind.DWord);
            });

            if (result)
            {
                ShowBenefits(
                    "Input lag reducido en 5-15ms",
                    "CPU liberado del overlay",
                    "Sin interrupciones durante el gaming",
                    "Mejor rendimiento general"
                );
            }

            return result;
        }

        public bool EnableGameBar()
        {
            return ApplyRegistryTransaction("Habilitar Xbox Game Bar", transaction =>
            {
                transaction.SetValue(@"HKEY_CURRENT_USER\" + RegistryPaths.UserGaming.GameDvr, RegistryValues.AppCaptureEnabled, 1, RegistryValueKind.DWord);
                transaction.SetValue(@"HKEY_CURRENT_USER\" + RegistryPaths.UserGaming.GameDvr, RegistryValues.GameDvrEnabled, 1, RegistryValueKind.DWord);
            });
        }

        public bool EnableGameMode()
        {
            return SetRegistryDWord(Registry.CurrentUser, RegistryPaths.UserGaming.GameBar,
                "UseNexusForGameBarEnabled", 0, "Optimizar Windows Game Mode");
        }

        public bool IsGameBarDisabled()
        {
            return IsRegistryValueEqual(Registry.CurrentUser, RegistryPaths.UserGaming.GameDvr,
                RegistryValues.GameDvrEnabled, "0");
        }

        public bool IsGameModeEnabled()
        {
            // Por defecto está habilitado si no existe el valor o es 0
            int value = GetRegistryValue<int>(Registry.CurrentUser, RegistryPaths.UserGaming.GameBar,
                "UseNexusForGameBarEnabled", 0);
            return value == 0;
        }
    }
}

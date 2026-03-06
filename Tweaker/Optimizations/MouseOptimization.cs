using System;
using System.Diagnostics;

using Microsoft.Win32;

using Tweaker.Optimizations.Base;
using Tweaker.Utilities;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Mouse Optimization - Compatibilidad con presets
    /// Refactorizado para eliminar redundancia usando BaseOptimization
    /// </summary>
    public static class MouseOptimization
    {
        private static readonly MouseOptimizationImpl _impl = new MouseOptimizationImpl();

        /// <summary>
        /// Deshabilita aceleración del mouse
        /// </summary>
        public static bool DisableAcceleration() => _impl.DisableAcceleration();

        /// <summary>
        /// Habilita aceleración del mouse
        /// </summary>
        public static bool EnableAcceleration() => _impl.EnableAcceleration();

        /// <summary>
        /// Verifica si la aceleración está deshabilitada
        /// </summary>
        public static bool IsAccelerationDisabled() => _impl.IsAccelerationDisabled();

        /// <summary>
        /// Obtiene información sobre la configuración actual
        /// </summary>
        public static string GetInfo() => _impl.GetInfo();
    }

    /// <summary>
    /// Implementación interna usando BaseOptimization para eliminar redundancia
    /// </summary>
    internal class MouseOptimizationImpl : BaseOptimization
    {
        public MouseOptimizationImpl() : base("Mouse Optimization")
        {
        }

        public bool DisableAcceleration()
        {
            bool result = ApplyRegistryTransaction("Deshabilitar aceleración del mouse", transaction =>
            {
                transaction.SetValue(@"HKEY_CURRENT_USER\" + RegistryPaths.UserInput.Mouse, RegistryValues.MouseSpeed, "0", RegistryValueKind.String);
                transaction.SetValue(@"HKEY_CURRENT_USER\" + RegistryPaths.UserInput.Mouse, RegistryValues.MouseThreshold1, "0", RegistryValueKind.String);
                transaction.SetValue(@"HKEY_CURRENT_USER\" + RegistryPaths.UserInput.Mouse, RegistryValues.MouseThreshold2, "0", RegistryValueKind.String);
            });

            if (result)
            {
                ShowBenefits(
                    "Aim 1:1 pixel perfect tracking",
                    "Movimientos predecibles y consistentes",
                    "Mejor muscle memory",
                    "Precisión mejorada en gaming competitivo"
                );
            }

            return result;
        }

        public bool EnableAcceleration()
        {
            return ApplyRegistryTransaction("Habilitar aceleración del mouse", transaction =>
            {
                transaction.SetValue(@"HKEY_CURRENT_USER\" + RegistryPaths.UserInput.Mouse, RegistryValues.MouseSpeed, "1", RegistryValueKind.String);
                transaction.SetValue(@"HKEY_CURRENT_USER\" + RegistryPaths.UserInput.Mouse, RegistryValues.MouseThreshold1, "6", RegistryValueKind.String);
                transaction.SetValue(@"HKEY_CURRENT_USER\" + RegistryPaths.UserInput.Mouse, RegistryValues.MouseThreshold2, "10", RegistryValueKind.String);
            });
        }

        public bool IsAccelerationDisabled()
        {
            try
            {
                string speed = GetRegistryValue<string>(Registry.CurrentUser, RegistryPaths.UserInput.Mouse, RegistryValues.MouseSpeed, "1");
                string t1 = GetRegistryValue<string>(Registry.CurrentUser, RegistryPaths.UserInput.Mouse, RegistryValues.MouseThreshold1, "6");
                string t2 = GetRegistryValue<string>(Registry.CurrentUser, RegistryPaths.UserInput.Mouse, RegistryValues.MouseThreshold2, "10");

                return speed == "0" && t1 == "0" && t2 == "0";
            }
            catch
            {
                return false;
            }
        }

        public string GetInfo()
        {
            if (IsAccelerationDisabled())
            {
                return "? Aceleración DESACTIVADA (Óptimo para gaming)";
            }
            else
            {
                return "?? Aceleración ACTIVA (Afecta precisión del aim)";
            }
        }
    }
}

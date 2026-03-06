using System;
using System.Diagnostics;
using Microsoft.Win32;
using Tweaker.Optimizations.Base;
using Tweaker.Utilities;

namespace Tweaker.Optimizations
{
    public static class GpuTweaks
    {
        private static readonly GpuTweaksImpl _impl = new GpuTweaksImpl();

        public static bool DisableMPO() => _impl.DisableMPO();
        public static bool EnableMPO() => _impl.EnableMPO();
        public static string DiagnoseMPOState() => _impl.DiagnoseMPOState();
        public static bool IsMPODisabled() => _impl.IsMPODisabled();
        public static bool DisableGameMode() => _impl.DisableGameMode();
        public static bool EnableGameMode() => _impl.EnableGameMode();
        public static bool DisableMPOSafely() => _impl.DisableMPO();
        public static bool EnableMPOSafely() => _impl.EnableMPO();
        public static string GetHDCPInfo() => _impl.GetHDCPInfo();
    }

    internal class GpuTweaksImpl : BaseOptimization
    {
        private const string DwmKeyPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\Dwm";
        private const string GraphicsKeyPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\GraphicsDrivers";
        private const string GameBarKeyPath = @"HKEY_CURRENT_USER\Software\Microsoft\GameBar";

        public GpuTweaksImpl() : base("GPU Tweaks")
        {
        }

        public bool DisableMPO()
        {
            return ApplyRegistryTransaction("Deshabilitar MPO", transaction =>
            {
                transaction.SetValue(DwmKeyPath, "OverlayTestMode", 5, RegistryValueKind.DWord);
                transaction.SetValue(GraphicsKeyPath, "DisableDomainMPO", 1, RegistryValueKind.DWord);
                transaction.SetValue(GraphicsKeyPath, "DisablePreemption", 1, RegistryValueKind.DWord);
            });
        }

        public bool EnableMPO()
        {
            return ApplyRegistryTransaction("Habilitar MPO", transaction =>
            {
                transaction.DeleteValue(DwmKeyPath, "OverlayTestMode");
                transaction.DeleteValue(GraphicsKeyPath, "DisableDomainMPO");
                transaction.DeleteValue(GraphicsKeyPath, "DisablePreemption");
            });
        }

        public bool DisableGameMode()
        {
            return ApplyRegistryTransaction("Deshabilitar Game Mode", transaction =>
            {
                transaction.SetValue(GameBarKeyPath, "AutoGameModeEnabled", 0, RegistryValueKind.DWord);
            });
        }

        public bool EnableGameMode()
        {
            return ApplyRegistryTransaction("Habilitar Game Mode", transaction =>
            {
                transaction.SetValue(GameBarKeyPath, "AutoGameModeEnabled", 1, RegistryValueKind.DWord);
            });
        }

        public string DiagnoseMPOState()
        {
            try
            {
                var overlayValue = Registry.GetValue(DwmKeyPath, "OverlayTestMode", null);
                bool isDisabled = overlayValue != null && Convert.ToInt32(overlayValue) == 5;

                var sb = new System.Text.StringBuilder();
                sb.AppendLine(isDisabled
                    ? "✅ MPO DESHABILITADO (OverlayTestMode = 5)"
                    : "⚠️ MPO HABILITADO (valor predeterminado de Windows)");
                sb.AppendLine($"   Valor actual OverlayTestMode: {overlayValue ?? "no existe (default = 0)"}");
                sb.AppendLine(isDisabled
                    ? "   → Stuttering y pantallazos negros reducidos"
                    : "   → Considera deshabilitar MPO si tienes stuttering");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"❌ Error leyendo estado MPO: {ex.Message}";
            }
        }

        public bool IsMPODisabled()
        {
            try
            {
                var value = Registry.GetValue(DwmKeyPath, "OverlayTestMode", null);
                return value != null && Convert.ToInt32(value) == 5;
            }
            catch
            {
                return false;
            }
        }
        
        public string GetHDCPInfo()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("?? HDCP NO SE PUEDE DESHABILITAR VA REGISTRO");
            sb.AppendLine();
            sb.AppendLine("HDCP (High-bandwidth Digital Content Protection):");
            sb.AppendLine("- Proteccin DRM en HDMI/DisplayPort");
            sb.AppendLine("- Aade 2-5ms de latencia");
            sb.AppendLine("- Causa pantallazos negros en algunos monitores");
            sb.AppendLine();
            sb.AppendLine("SOLUCIONES ALTERNATIVAS:");
            sb.AppendLine("1. Monitor OSD: Busca Settings > HDMI/DP > HDCP > Off");
            sb.AppendLine("2. Usa cable HDMI/DP pasivo (sin HDCP 2.2+ support)");
            sb.AppendLine("3. Actualiza firmware del monitor");
            sb.AppendLine();
            sb.AppendLine("NOTA: En gaming, HDCP rara vez es el problema.");
            sb.AppendLine("MPO (Multiplane Overlay) causa ms stuttering.");
            sb.AppendLine();
            sb.AppendLine("Si tienes pantallazos negros:");
            sb.AppendLine("1. Deshabilita MPO (botn arriba)");
            sb.AppendLine("2. Actualiza drivers GPU");
            sb.AppendLine("3. Prueba cable DisplayPort en vez de HDMI");
            return sb.ToString();
        }
    }
}

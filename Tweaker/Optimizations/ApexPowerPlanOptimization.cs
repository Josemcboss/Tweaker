using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// ApexPowerPlanOptimization - Inspirado en Paragon Tweaking Utility (PTU)
    /// Plan de energía dedicado Ultra-Gaming / Apex:
    /// - Desactiva Core Parking agresivo (100% núcleos activos)
    /// - Desactiva Power Throttling a nivel global de Windows
    /// - Frecuencia de CPU y estados C-State de respuesta inmediata
    /// </summary>
    public static class ApexPowerPlanOptimization
    {
        private const string SUB_PROCESSOR = "54533251-82be-4824-96c1-47b60b740d00";
        private const string PROCTHROTTLEMIN = "89ba256d-0b19-4417-90f4-b38ba2f2e0ff";
        private const string PROCTHROTTLEMAX = "bc5038f7-23e0-4960-96da-33abaf5935ec";
        private const string CPMINCORES = "0cc06647-7777-4a9c-9c74-b0f20e4603e0";
        private const string SUB_PCIEXPRESS = "501a4d13-42af-4429-9dec-e4e9f777647c";
        private const string ASPM = "ee12f906-d277-404b-b6da-f5fae829d4e1";
        private const string SUB_DISK = "0012ee47-9041-4b5d-9b77-535fba8b1442";
        private const string DISKIDLE = "6738e2c4-3291-4942-8924-ccde32a4f30e";

        /// <summary>
        /// Aplica la configuración de Plan de Energía Apex Gaming
        /// </summary>
        public static bool ApplyApexPowerPlan()
        {
            try
            {
                Debug.WriteLine("→ Configurando Plan de Energía Apex / Ultra Gaming...");

                // 1. Activar esquema Ultimate Performance o High Performance como base
                RunPowerCfg("-duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61"); // Ultimate Performance
                RunPowerCfg("-setactive e9a42b02-d5df-448d-aa00-03f14749eb61");

                // Si no existe Ultimate Performance, activar High Performance
                RunPowerCfg("-setactive 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c");

                // 2. Desactivar Core Parking (Min Cores = 100%)
                RunPowerCfg($"-setacvalueindex SCHEME_CURRENT {SUB_PROCESSOR} {CPMINCORES} 100");
                RunPowerCfg($"-setdcvalueindex SCHEME_CURRENT {SUB_PROCESSOR} {CPMINCORES} 100");

                // 3. CPU Estado Mínimo y Máximo al 100%
                RunPowerCfg($"-setacvalueindex SCHEME_CURRENT {SUB_PROCESSOR} {PROCTHROTTLEMIN} 100");
                RunPowerCfg($"-setdcvalueindex SCHEME_CURRENT {SUB_PROCESSOR} {PROCTHROTTLEMIN} 100");
                RunPowerCfg($"-setacvalueindex SCHEME_CURRENT {SUB_PROCESSOR} {PROCTHROTTLEMAX} 100");
                RunPowerCfg($"-setdcvalueindex SCHEME_CURRENT {SUB_PROCESSOR} {PROCTHROTTLEMAX} 100");

                // 4. Desactivar PCIe ASPM (Link State Power Management OFF)
                RunPowerCfg($"-setacvalueindex SCHEME_CURRENT {SUB_PCIEXPRESS} {ASPM} 0");
                RunPowerCfg($"-setdcvalueindex SCHEME_CURRENT {SUB_PCIEXPRESS} {ASPM} 0");

                // 5. Disco nunca se apaga (0 segundos)
                RunPowerCfg($"-setacvalueindex SCHEME_CURRENT {SUB_DISK} {DISKIDLE} 0");
                RunPowerCfg($"-setdcvalueindex SCHEME_CURRENT {SUB_DISK} {DISKIDLE} 0");

                // 6. Aplicar cambios al esquema actual
                RunPowerCfg("-setactive SCHEME_CURRENT");

                // 7. Desactivar Power Throttling en registro
                using (RegistryKey? key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Power\PowerThrottling"))
                {
                    if (key != null)
                    {
                        key.SetValue("PowerThrottlingOff", 1, RegistryValueKind.DWord);
                    }
                }

                // 8. Desactivar Power Throttling mediante comando powercfg
                RunPowerCfg("/powerthrottling disable /flags:all");

                Debug.WriteLine("✓ Plan de Energía Apex / Ultra Gaming activado con éxito.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error en ApplyApexPowerPlan: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura el plan de energía estándar Equilibrado (Balanced)
        /// </summary>
        public static bool RestoreDefaultPowerPlan()
        {
            try
            {
                Debug.WriteLine("→ Restaurando Plan de Energía Estándar (Equilibrado)...");

                // Esquema Equilibrado de Windows
                RunPowerCfg("-setactive 381b4222-f694-41f0-9685-ff5bb260df2e");

                using (RegistryKey? key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Power\PowerThrottling"))
                {
                    if (key != null)
                    {
                        key.DeleteValue("PowerThrottlingOff", false);
                    }
                }

                RunPowerCfg("/powerthrottling enable /flags:all");

                Debug.WriteLine("✓ Plan de Energía Equilibrado restaurado.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error en RestoreDefaultPowerPlan: {ex.Message}");
                return false;
            }
        }

        private static void RunPowerCfg(string arguments)
        {
            try
            {
                using var p = new Process();
                p.StartInfo = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                p.Start();
                p.WaitForExit(3000);
            }
            catch
            {
                // Ignorar excepciones menores de comandos individuales
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    public static class CyberSecurityOptimization
    {
        // Reglas ASR de Windows Defender recomendadas para Gaming/Workstation
        // 9e6c4e1f-7d60-472f-ba1a-a39ef669e4b2: Block credential stealing from LSASS
        // d4f940ab-401b-4efc-aadc-ad5f3c50688a: Block executable content from email client and webmail
        // 75668c1f-73b5-4cf0-bb93-3ecf5cb760c6: Block process creations originating from PSExec/WMI
        // 56a8634f-1184-415a-a8a3-9fba12b4e10b: Block only untrusted scripts (PowerShell, VBScript, JavaScript)
        private static readonly string[] AsrRuleIds = new[]
        {
            "9e6c4e1f-7d60-472f-ba1a-a39ef669e4b2",
            "d4f940ab-401b-4efc-aadc-ad5f3c50688a",
            "75668c1f-73b5-4cf0-bb93-3ecf5cb760c6",
            "56a8634f-1184-415a-a8a3-9fba12b4e10b"
        };

        public static bool ApplySecureDns(string provider)
        {
            try
            {
                string primaryDns = "1.1.1.1";
                string secondaryDns = "1.0.0.1";
                string dohTemplate = "https://cloudflare-dns.com/dns-query";

                switch (provider.ToLowerInvariant())
                {
                    case "quad9":
                        primaryDns = "9.9.9.9";
                        secondaryDns = "149.112.112.112";
                        dohTemplate = "https://dns.quad9.net/dns-query";
                        break;
                    case "adguard":
                        primaryDns = "94.140.14.14";
                        secondaryDns = "94.140.15.15";
                        dohTemplate = "https://dns.adguard-dns.com/dns-query";
                        break;
                    case "cloudflare":
                    default:
                        primaryDns = "1.1.1.1";
                        secondaryDns = "1.0.0.1";
                        dohTemplate = "https://cloudflare-dns.com/dns-query";
                        break;
                }

                // Configurar mediante netsh / PowerShell para todas las interfaces de red activas
                string script = $@"
                    Get-NetAdapter | Where-Object {{ .Status -eq 'Up' }} | ForEach-Object {{
                        Set-DnsClientServerAddress -InterfaceIndex .ifIndex -ServerAddresses ('{primaryDns}', '{secondaryDns}')
                    }}
                    # Si soporta DoH (Windows 11)
                    if (Get-Command Add-DnsClientDohServerAddress -ErrorAction SilentlyContinue) {{
                        Add-DnsClientDohServerAddress -ServerAddress '{primaryDns}' -DohTemplate '{dohTemplate}' -AllowFallbackToUdp False -AutoUpgrade True -ErrorAction SilentlyContinue
                    }}
                ";

                RunPowerShell(script);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error aplicando DoH: {ex.Message}");
                return false;
            }
        }

        public static bool ResetDnsToAutomatic()
        {
            try
            {
                string script = @"
                    Get-NetAdapter | Where-Object { .Status -eq 'Up' } | ForEach-Object {
                        Set-DnsClientServerAddress -InterfaceIndex .ifIndex -ResetServerAddresses
                    }
                ";
                RunPowerShell(script);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error restableciendo DNS: {ex.Message}");
                return false;
            }
        }

        public static bool EnableDefenderAsrRules()
        {
            try
            {
                foreach (var ruleId in AsrRuleIds)
                {
                    string script = $@"Add-MpPreference -AttackSurfaceReductionRules_Ids {ruleId} -AttackSurfaceReductionRules_Actions Enabled";
                    RunPowerShell(script);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error aplicando ASR Rules: {ex.Message}");
                return false;
            }
        }

        public static bool DisableDefenderAsrRules()
        {
            try
            {
                foreach (var ruleId in AsrRuleIds)
                {
                    string script = $@"Add-MpPreference -AttackSurfaceReductionRules_Ids {ruleId} -AttackSurfaceReductionRules_Actions Disabled";
                    RunPowerShell(script);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deshabilitando ASR Rules: {ex.Message}");
                return false;
            }
        }

        public static bool ApplyTelemetryHardening()
        {
            try
            {
                // Telemetria Windows & Experiencias de Consumidor
                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DataCollection"))
                {
                    key?.SetValue("AllowTelemetry", 0, RegistryValueKind.DWord);
                }
                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\CloudContent"))
                {
                    key?.SetValue("DisableWindowsConsumerFeatures", 1, RegistryValueKind.DWord);
                }
                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\AdvertisingInfo"))
                {
                    key?.SetValue("DisabledByGroupPolicy", 1, RegistryValueKind.DWord);
                }

                // Deshabilitar servicios DiagTrack sin romper Windows Update
                string script = @"
                    Stop-Service -Name DiagTrack -Force -ErrorAction SilentlyContinue
                    Set-Service -Name DiagTrack -StartupType Disabled -ErrorAction SilentlyContinue
                    Stop-Service -Name dmwappushservice -Force -ErrorAction SilentlyContinue
                    Set-Service -Name dmwappushservice -StartupType Disabled -ErrorAction SilentlyContinue
                ";
                RunPowerShell(script);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error aplicando Telemetry Hardening: {ex.Message}");
                return false;
            }
        }

        public static bool RevertTelemetryHardening()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DataCollection"))
                {
                    key?.DeleteValue("AllowTelemetry", false);
                }
                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\CloudContent"))
                {
                    key?.DeleteValue("DisableWindowsConsumerFeatures", false);
                }
                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\AdvertisingInfo"))
                {
                    key?.DeleteValue("DisabledByGroupPolicy", false);
                }

                string script = @"
                    Set-Service -Name DiagTrack -StartupType Automatic -ErrorAction SilentlyContinue
                    Start-Service -Name DiagTrack -ErrorAction SilentlyContinue
                ";
                RunPowerShell(script);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error revirtiendo Telemetry Hardening: {ex.Message}");
                return false;
            }
        }

        private static void RunPowerShell(string script)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script}\"",
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            using var proc = Process.Start(psi);
            proc?.WaitForExit(5000);
        }
    }
}


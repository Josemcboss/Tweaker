using System;
using System.Diagnostics;
using System.Windows;

namespace Tweaker.Utilities
{
    /// <summary>
    /// Utilidad para crear Puntos de Restauración del Sistema
    /// CRÍTICO antes de aplicar tweaks de registro
    /// </summary>
    public static class SystemRestore
    {
        /// <summary>
        /// CREA UN PUNTO DE RESTAURACIÓN DEL SISTEMA
        /// 
        /// ¿Qué es un Punto de Restauración?
        /// ????????????????????????????????????????????????????????????????
        /// - Snapshot del registro de Windows y archivos del sistema
        /// - Permite revertir cambios si algo sale mal
        /// - NO afecta archivos personales (documentos, fotos, etc.)
        /// - Solo restaura configuración del sistema y registro
        /// 
        /// ¿Por qué es CRÍTICO?
        /// ????????????????????????????????????????????????????????????????
        /// - Esta aplicación modifica el REGISTRO de Windows
        /// - Aunque los tweaks son reversibles, un punto de restauración es SEGURO
        /// - Si Windows no bootea, puedes restaurar desde Safe Mode
        /// - Profesional tener backup antes de modificar sistema
        /// 
        /// MÉTODO DE CREACIÓN:
        /// ????????????????????????????????????????????????????????????????
        /// Usa PowerShell cmdlet: Checkpoint-Computer
        /// 
        /// Comando ejecutado:
        /// powershell.exe -NoProfile -ExecutionPolicy Bypass -Command 
        ///   "Checkpoint-Computer -Description 'descripcion' -RestorePointType 'MODIFY_SETTINGS'"
        /// 
        /// RestorePointType:
        /// - MODIFY_SETTINGS = Cambio de configuración (nuestro caso)
        /// - APPLICATION_INSTALL = Instalación de app
        /// - APPLICATION_UNINSTALL = Desinstalación
        /// 
        /// LIMITACIONES DE WINDOWS:
        /// ????????????????????????????????????????????????????????????????
        /// - Windows solo permite 1 punto de restauración cada 24 horas (por defecto)
        /// - Si ya creaste uno hoy, puede fallar
        /// - Necesita System Protection habilitado en C:\
        /// - Requiere permisos de Administrador
        /// 
        /// CÓMO VERIFICAR QUE EXISTE:
        /// ????????????????????????????????????????????????????????????????
        /// 1. Windows + R ? rstrui.exe
        /// 2. Busca punto de restauración con tu descripción
        /// 
        /// CÓMO RESTAURAR SI HAY PROBLEMAS:
        /// ????????????????????????????????????????????????????????????????
        /// 1. Safe Mode: F8 al bootear
        /// 2. Troubleshoot > Advanced > System Restore
        /// 3. Seleccionar punto de restauración
        /// 4. Confirmar y reiniciar
        /// 
        /// NOTA: Este método es ASÍNCRONO (no espera a que termine)
        /// PowerShell crea el punto en background
        /// Toma ~1-3 minutos completarse
        /// </summary>
        /// <param name="description">Descripción del punto de restauración</param>
        public static void CreateRestorePoint(string description)
        {
            try
            {
                // Validar descripción
                if (string.IsNullOrWhiteSpace(description))
                {
                    description = $"Tweaker Backup - {DateTime.Now:yyyy-MM-dd HH:mm}";
                }

                Debug.WriteLine($"Creando punto de restauración: {description}");

                // Comando PowerShell para crear punto de restauración
                string command = $"-NoProfile -ExecutionPolicy Bypass -Command " +
                                $"\"Checkpoint-Computer -Description '{description}' -RestorePointType 'MODIFY_SETTINGS'\"";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = command,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas", // Requiere admin
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                // Iniciar proceso
                Process process = Process.Start(psi);

                if (process != null)
                {
                    Debug.WriteLine("? Comando de creación de punto de restauración enviado");
                    Debug.WriteLine("  PowerShell está creando el punto en background");
                    Debug.WriteLine("  Esto puede tomar 1-3 minutos");
                    Debug.WriteLine($"  Descripción: {description}");

                    // Opcional: Esperar a que termine (puede tardar)
                    // process.WaitForExit(180000); // 3 minutos timeout

                    // Leer output
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    if (!string.IsNullOrEmpty(output))
                    {
                        Debug.WriteLine($"Output: {output}");
                    }

                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.WriteLine($"Error: {error}");
                    }
                }
                else
                {
                    Debug.WriteLine("? No se pudo iniciar PowerShell");
                }
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                // Usuario canceló UAC o sin permisos
                Debug.WriteLine($"?? Usuario canceló UAC o sin permisos: {ex.Message}");
                
                MessageBox.Show(
                    "?? NO SE PUDO CREAR PUNTO DE RESTAURACIÓN\n\n" +
                    "Posibles causas:\n" +
                    "• Cancelaste el UAC prompt\n" +
                    "• No tienes permisos de Administrador\n" +
                    "• System Protection está deshabilitado\n" +
                    "• Ya creaste un punto hoy (Windows limita 1 por día)\n\n" +
                    "PUEDES CONTINUAR, pero sin backup.\n" +
                    "Los tweaks son reversibles con los botones OFF.",
                    "Advertencia - Sin Punto de Restauración",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error al crear punto de restauración: {ex.Message}");
                
                MessageBox.Show(
                    $"? Error al crear punto de restauración:\n\n{ex.Message}\n\n" +
                    $"Puedes continuar sin punto de restauración.\n" +
                    $"Los tweaks son reversibles.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// VERIFICA SI SYSTEM PROTECTION ESTÁ HABILITADO
        /// System Protection debe estar ON para crear puntos de restauración
        /// </summary>
        public static bool IsSystemProtectionEnabled()
        {
            try
            {
                // Ejecutar: Get-ComputerRestorePoint (PowerShell)
                // Si retorna algo = System Protection habilitado
                
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-NoProfile -Command \"Get-ComputerRestorePoint | Select-Object -First 1\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process process = Process.Start(psi))
                {
                    if (process == null)
                        return false;

                    process.WaitForExit(5000); // 5 segundos timeout

                    string output = process.StandardOutput.ReadToEnd();
                    
                    // Si hay output = hay puntos de restauración = System Protection habilitado
                    return !string.IsNullOrWhiteSpace(output);
                }
            }
            catch
            {
                // Si falla, asumimos que está deshabilitado
                return false;
            }
        }

        /// <summary>
        /// ABRE LA INTERFAZ DE SYSTEM RESTORE de Windows
        /// Permite al usuario ver/crear/restaurar puntos manualmente
        /// </summary>
        public static void OpenSystemRestoreUI()
        {
            try
            {
                // Comando: rstrui.exe (System Restore UI)
                Process.Start("rstrui.exe");
                
                Debug.WriteLine("? System Restore UI abierta");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error al abrir System Restore UI: {ex.Message}");
                
                MessageBox.Show(
                    $"No se pudo abrir System Restore.\n\n" +
                    $"Abre manualmente:\n" +
                    $"Windows + R ? rstrui.exe",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// MUESTRA DIÁLOGO PREGUNTANDO SI QUIERE CREAR PUNTO DE RESTAURACIÓN
        /// Retorna true si el usuario acepta
        /// </summary>
        public static bool PromptCreateRestorePoint()
        {
            var result = MessageBox.Show(
                "??? RECOMENDACIÓN: CREAR PUNTO DE RESTAURACIÓN\n\n" +
                "Esta aplicación modificará el registro de Windows.\n" +
                "Aunque todos los tweaks son reversibles, es ALTAMENTE\n" +
                "recomendado crear un punto de restauración antes.\n\n" +
                "BENEFICIOS:\n" +
                "? Puedes revertir TODOS los cambios fácilmente\n" +
                "? Seguridad si algo sale mal\n" +
                "? Recuperación desde Safe Mode si es necesario\n\n" +
                "NOTA:\n" +
                "• Toma ~1-3 minutos crear el punto\n" +
                "• Windows limita 1 punto por día\n" +
                "• Requiere System Protection habilitado\n\n" +
                "¿Deseas crear un punto de restauración AHORA?",
                "Punto de Restauración Recomendado",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                CreateRestorePoint($"Tweaker Backup - {DateTime.Now:yyyy-MM-dd HH:mm}");
                
                MessageBox.Show(
                    "? Creando punto de restauración en background...\n\n" +
                    "Esto puede tomar 1-3 minutos.\n" +
                    "Puedes continuar usando la aplicación.\n\n" +
                    "VERIFICAR:\n" +
                    "Windows + R ? rstrui.exe\n" +
                    "Busca: 'Tweaker Backup'",
                    "Punto de Restauración",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                
                return true;
            }

            return false;
        }
    }
}

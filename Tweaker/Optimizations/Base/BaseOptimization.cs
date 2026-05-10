using System;
using System.Diagnostics;

using Microsoft.Win32;

using Tweaker.Utilities;

namespace Tweaker.Optimizations.Base
{
    /// <summary>
    /// BaseOptimization - Clase base para todas las optimizaciones
    /// Elimina redundancia en manejo de errores, logging y patrones comunes
    /// </summary>
    public abstract class BaseOptimization
    {
        protected string OptimizationName { get; }

        protected BaseOptimization(string optimizationName)
        {
            OptimizationName = optimizationName;
        }

        /// <summary>
        /// Ejecuta una operación de optimización con manejo de errores estándar
        /// </summary>
        protected bool ExecuteOptimization(string operationDescription, Func<bool> operation)
        {
            try
            {
                Debug.WriteLine($"?? {OptimizationName}: {operationDescription}");

                bool result = operation();

                if (result)
                {
                    Debug.WriteLine($"? {OptimizationName}: {operationDescription} - EXITOSO");
                }
                else
                {
                    Debug.WriteLine($"? {OptimizationName}: {operationDescription} - FALLIDO");
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? {OptimizationName}: Error en {operationDescription}");
                Debug.WriteLine($"   Excepción: {ex.Message}");
                return false;
            }
        }



        protected bool ApplyRegistryTransaction(string operationName, Action<RegistryTransaction> transactionActions)
        {
            return ExecuteOptimization(operationName, () => {
                using (var transaction = new RegistryTransaction())
                {
                    transactionActions(transaction);
                    transaction.Commit();
                }
                return true;
            });
        }

        /// <summary>
        /// Helper para establecer un valor DWORD con logging automático
        /// </summary>
        protected bool SetRegistryDWord(RegistryKey hive, string keyPath, string valueName, int value, string description = "")
        {
            string fullDescription = string.IsNullOrEmpty(description) ?
                $"Configurar {valueName} = {value}" : description;

            return RegistryHelper.SetRegistryValue(hive, keyPath, valueName, value,
                $"{OptimizationName} - {fullDescription}");
        }

        /// <summary>
        /// Helper para establecer un valor String con logging automático
        /// </summary>
        protected bool SetRegistryString(RegistryKey hive, string keyPath, string valueName, string value, string description = "")
        {
            string fullDescription = string.IsNullOrEmpty(description) ?
                $"Configurar {valueName} = {value}" : description;

            return RegistryHelper.SetRegistryValue(hive, keyPath, valueName, value,
                $"{OptimizationName} - {fullDescription}");
        }

        /// <summary>
        /// Helper para leer valores del registro de forma segura
        /// </summary>
        protected T? GetRegistryValue<T>(RegistryKey hive, string keyPath, string valueName, T? defaultValue = default(T))
        {
            return RegistryHelper.GetRegistryValue(hive, keyPath, valueName, defaultValue);
        }

        /// <summary>
        /// Helper para verificar si un valor del registro tiene un valor específico
        /// </summary>
        protected bool IsRegistryValueEqual(RegistryKey hive, string keyPath, string valueName, object expectedValue)
        {
            return RegistryHelper.IsRegistryValueEqual(hive, keyPath, valueName, expectedValue);
        }

        /// <summary>
        /// Ejecuta un comando del sistema con manejo de errores
        /// </summary>
        protected bool ExecuteSystemCommand(string fileName, string arguments, string operationDescription, bool requiresAdmin = true)
        {
            return ExecuteOptimization(operationDescription, () =>
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = fileName,
                        Arguments = arguments,
                        UseShellExecute = requiresAdmin,
                        CreateNoWindow = true
                    };

                    if (requiresAdmin)
                    {
                        psi.Verb = "runas";
                    }
                    else
                    {
                        psi.RedirectStandardOutput = true;
                        psi.RedirectStandardError = true;
                    }

                    using (Process? process = Process.Start(psi))
                    {
                        if (process != null)
                        {
                            process.WaitForExit();
                            return process.ExitCode == 0;
                        }
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error ejecutando comando: {fileName} {arguments}");
                    Debug.WriteLine($"Error: {ex.Message}");
                    return false;
                }
            });
        }

        /// <summary>
        /// Muestra un resumen de la operación
        /// </summary>
        protected void ShowOperationSummary(bool success, string successMessage, string failureMessage)
        {
            Debug.WriteLine("──────────────────────────?");
            if (success)
            {
                Debug.WriteLine($"? {OptimizationName} - {successMessage}");
            }
            else
            {
                Debug.WriteLine($"? {OptimizationName} - {failureMessage}");
            }
            Debug.WriteLine("──────────────────────────?");
        }

        /// <summary>
        /// Muestra información sobre beneficios esperados
        /// </summary>
        protected void ShowBenefits(params string[] benefits)
        {
            Debug.WriteLine($"?? {OptimizationName} - BENEFICIOS ESPERADOS:");
            foreach (string benefit in benefits)
            {
                Debug.WriteLine($"   • {benefit}");
            }
        }

        /// <summary>
        /// Muestra advertencias importantes
        /// </summary>
        protected void ShowWarnings(params string[] warnings)
        {
            Debug.WriteLine($"?? {OptimizationName} - ADVERTENCIAS:");
            foreach (string warning in warnings)
            {
                Debug.WriteLine($"   ?? {warning}");
            }
        }
    }

    /// <summary>
    /// Clase base específica para optimizaciones que requieren reinicio
    /// </summary>
    public abstract class RestartRequiredOptimization : BaseOptimization
    {
        protected RestartRequiredOptimization(string optimizationName) : base(optimizationName)
        {
        }

        protected void ShowRestartWarning()
        {
            Debug.WriteLine("?? REINICIO REQUERIDO para que los cambios tengan efecto completo");
        }

        protected bool ExecuteWithRestartWarning(string operationDescription, Func<bool> operation)
        {
            bool result = ExecuteOptimization(operationDescription, operation);
            if (result)
            {
                ShowRestartWarning();
            }
            return result;
        }
    }
}

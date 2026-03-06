using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// MouseTweaks - Desactivar Aceleración del Mouse
    /// CRÍTICO para FPS gaming (CS2, Valorant, Apex)
    /// </summary>
    public static class MouseTweaks
    {
        // P/Invoke para aplicar cambios sin reiniciar
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        private const uint SPI_SETMOUSE = 0x0004;
        private const uint SPIF_UPDATEINIFILE = 0x01;
        private const uint SPIF_SENDCHANGE = 0x02;

        private const string MOUSE_KEY = @"Control Panel\Mouse";

        /// <summary>
        /// DESACTIVAR ACELERACIÓN DEL MOUSE
        /// 
        /// ¿Qué es la Aceleración del Mouse?
        /// ????????????????????????????????????????????????????????????????
        /// Windows añade aceleración artificial que hace que el cursor
        /// se mueva más rápido cuanto más rápido muevas el mouse.
        /// 
        /// PROBLEMA EN GAMING:
        /// - Inconsistencia en el aim (imposible crear muscle memory)
        /// - Un movimiento rápido = distancia impredecible
        /// - Los PRO PLAYERS SIEMPRE la deshabilitan
        /// 
        /// SOLUCIÓN:
        /// - MouseSpeed = 0 (Deshabilitar aceleración)
        /// - MouseThreshold1 = 0 (Sin umbral de velocidad)
        /// - MouseThreshold2 = 0 (Sin segundo umbral)
        /// 
        /// EFECTO:
        /// ? 1:1 pixel perfect tracking
        /// ? Movimientos predecibles
        /// ? Muscle memory consistente
        /// ? Aim más preciso
        /// 
        /// USADO POR:
        /// - 100% de PRO PLAYERS en shooters
        /// - TenZ, s1mple, Shroud, todos
        /// </summary>
        public static bool Apply()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(MOUSE_KEY, true))
                {
                    if (key == null)
                    {
                        Debug.WriteLine("? No se pudo abrir clave Mouse");
                        return false;
                    }

                    // Desactivar aceleración
                    key.SetValue("MouseSpeed", "0", RegistryValueKind.String);
                    key.SetValue("MouseThreshold1", "0", RegistryValueKind.String);
                    key.SetValue("MouseThreshold2", "0", RegistryValueKind.String);

                    Debug.WriteLine("? Mouse Acceleration OFF (Registro)");
                }

                // Aplicar cambios SIN reiniciar usando SystemParametersInfo
                int[] mouseParams = new int[3] { 0, 0, 0 }; // Speed, Threshold1, Threshold2
                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(mouseParams));
                Marshal.Copy(mouseParams, 0, ptr, 3);

                bool result = SystemParametersInfo(
                    SPI_SETMOUSE,
                    0,
                    ptr,
                    SPIF_UPDATEINIFILE | SPIF_SENDCHANGE
                );

                Marshal.FreeHGlobal(ptr);

                if (result)
                {
                    Debug.WriteLine("? Mouse Acceleration OFF (Aplicado en tiempo real)");
                    Debug.WriteLine("? EFECTO INMEDIATO - Sin reinicio");
                    return true;
                }
                else
                {
                    Debug.WriteLine("?? Registro modificado pero SystemParametersInfo falló");
                    Debug.WriteLine("Reinicia Windows para aplicar cambios");
                    return true; // Registro sí se modificó
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// RESTAURAR ACELERACIÓN DEL MOUSE
        /// (Valores predeterminados de Windows)
        /// </summary>
        public static bool Revert()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(MOUSE_KEY, true))
                {
                    if (key == null) return false;

                    // Valores predeterminados de Windows
                    key.SetValue("MouseSpeed", "1", RegistryValueKind.String);
                    key.SetValue("MouseThreshold1", "6", RegistryValueKind.String);
                    key.SetValue("MouseThreshold2", "10", RegistryValueKind.String);
                }

                // Aplicar en tiempo real
                int[] mouseParams = new int[3] { 1, 6, 10 };
                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(mouseParams));
                Marshal.Copy(mouseParams, 0, ptr, 3);

                SystemParametersInfo(SPI_SETMOUSE, 0, ptr, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
                Marshal.FreeHGlobal(ptr);

                Debug.WriteLine("? Mouse Acceleration restaurada a default");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verificar estado actual
        /// </summary>
        public static string GetStatus()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(MOUSE_KEY, false))
                {
                    if (key == null) return "Error: No se pudo leer";

                    string speed = key.GetValue("MouseSpeed")?.ToString() ?? "?";
                    string t1 = key.GetValue("MouseThreshold1")?.ToString() ?? "?";
                    string t2 = key.GetValue("MouseThreshold2")?.ToString() ?? "?";

                    if (speed == "0" && t1 == "0" && t2 == "0")
                        return "? Aceleración DESACTIVADA (Óptimo)";
                    else
                        return "?? Aceleración ACTIVA (Afecta aim)";
                }
            }
            catch
            {
                return "Error al verificar";
            }
        }
    }
}

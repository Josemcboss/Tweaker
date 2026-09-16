using System;
using System.Diagnostics;
using System.Windows;
using Tweaker.Controls;

namespace Tweaker.Services
{
    /// <summary>
    /// Servicio administrador de Overlays y Toasts sutiles In-Game para Ghost Optimizer.
    /// </summary>
    public sealed class InGameOverlayService
    {
        private static readonly Lazy<InGameOverlayService> _instance = new(() => new InGameOverlayService());
        public static InGameOverlayService Instance => _instance.Value;

        private InGameToastOverlay? _activeToast;
        private readonly object _lock = new object();

        private InGameOverlayService()
        {
        }

        /// <summary>
        /// Muestra la notificación de píldora flotante en pantalla
        /// </summary>
        public void ShowGameToast(string gameName, string details, bool isActive = true)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                try
                {
                    lock (_lock)
                    {
                        if (_activeToast != null)
                        {
                            try { _activeToast.Close(); } catch { }
                            _activeToast = null;
                        }

                        _activeToast = new InGameToastOverlay();
                        _activeToast.Closed += (s, e) =>
                        {
                            lock (_lock)
                            {
                                if (_activeToast == s) _activeToast = null;
                            }
                        };
                        _activeToast.ShowToast(gameName, details, isActive);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"⚠️ Error al mostrar InGameToast: {ex.Message}");
                }
            });
        }
    }
}

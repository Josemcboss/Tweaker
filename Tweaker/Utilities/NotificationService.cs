using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Tweaker.Utilities
{
    /// <summary>
    /// Servicio de notificaciones tipo toast (modernas y no intrusivas) y gestor de prompts (MessageBox)
    /// </summary>
    public class NotificationService
    {
        private static NotificationService? _instance;
        private Grid? _notificationContainer;
        private DispatcherTimer? _autoHideTimer;

        public static NotificationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NotificationService();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Inicializa el contenedor de notificaciones en la ventana principal
        /// </summary>
        public void Initialize(Grid mainGrid)
        {
            if (_notificationContainer == null)
            {
                _notificationContainer = new Grid
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 50, 20, 0),
                    IsHitTestVisible = false // Permite clicks a travs del Grid
                };

                // Agregar al Grid principal (ltimo elemento para que est encima)
                mainGrid.Children.Add(_notificationContainer);
                Grid.SetRowSpan(_notificationContainer, 2);
                Grid.SetColumnSpan(_notificationContainer, 2);

                _autoHideTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(5)
                };
                _autoHideTimer.Tick += AutoHideTimer_Tick;
            }
        }

        /// <summary>
        /// Muestra una notificacin de xito
        /// </summary>
        public void ShowSuccess(string message, string title = "Tweak Activado", int durationSeconds = 5)
        {
            ShowNotification(message, "✅ " + title, "#0E7A0D", durationSeconds);
        }

        /// <summary>
        /// Muestra una notificacin de error
        /// </summary>
        public void ShowError(string message, string title = "Error", int durationSeconds = 7)
        {
            ShowNotification(message, "❌ " + title, "#C42B1C", durationSeconds);
        }

        /// <summary>
        /// Muestra una notificacin de advertencia
        /// </summary>
        public void ShowWarning(string message, string title = "Advertencia", int durationSeconds = 6)
        {
            ShowNotification(message, "⚠️ " + title, "#FFC107", durationSeconds);
        }

        /// <summary>
        /// Muestra una notificacin informativa
        /// </summary>
        public void ShowInfo(string message, string title = "Información", int durationSeconds = 5)
        {
            ShowNotification(message, "ℹ️ " + title, "#5865F2", durationSeconds);
        }

        /// <summary>
        /// Muestra una alerta de reinicio necesario
        /// </summary>
        public void ShowRestartRequired(string tweakName)
        {
            ShowNotification(
                $"El tweak '{tweakName}' requiere reiniciar Windows para aplicarse completamente.",
                "⚠️ Reinicio Necesario",
                "#9B59B6",
                8
            );
        }

        /// <summary>
        /// Muestra un dilogo de pregunta al usuario (equivalente a MessageBox.Show con Yes/No, etc.)
        /// </summary>
        /// <returns>Resultado del dilogo</returns>
        public MessageBoxResult ShowPrompt(string message, string title, MessageBoxButton buttons, MessageBoxImage icon)
        {
            // Detener timer de notificaciones para evitar que se oculte mientras el prompt est abierto
            _autoHideTimer?.Stop();

            // Si hay una notificacin visible, ocultarla antes de mostrar el prompt
            if (_notificationContainer != null && _notificationContainer.Children.Count > 0)
            {
                AnimateNotificationOut(true); // Ocultar rpidamente sin esperar
            }

            MessageBoxResult result = MessageBox.Show(message, title, buttons, icon);

            // Reiniciar timer si es necesario
            if (_notificationContainer != null && _notificationContainer.Children.Count > 0 && _autoHideTimer != null)
            {
                _autoHideTimer.Start();
            }

            return result;
        }

        private void ShowNotification(string message, string title, string accentColor, int durationSeconds)
        {
            if (_notificationContainer == null)
                return;

            // Crear el border de la notificacin
            var notification = CreateNotificationBorder(title, message, accentColor);

            // Agregar al contenedor
            _notificationContainer.Children.Clear(); // Solo una notificacin a la vez
            _notificationContainer.Children.Add(notification);

            // Animacin de entrada
            AnimateNotificationIn(notification);

            // Timer para ocultar automticamente
            _autoHideTimer.Interval = TimeSpan.FromSeconds(durationSeconds);
            _autoHideTimer.Stop();
            _autoHideTimer.Start();
        }

        private Border CreateNotificationBorder(string title, string message, string accentColor)
        {
            var titleBlock = new TextBlock
            {
                Text = title,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var messageBlock = new TextBlock
            {
                Text = message,
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B9BBBE")),
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 300
            };

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(titleBlock);
            stackPanel.Children.Add(messageBlock);

            var border = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(accentColor)),
                BorderThickness = new Thickness(0, 0, 4, 0),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(20, 15, 20, 15),
                MaxWidth = 350,
                Margin = new Thickness(0, 0, 0, 10),
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Black,
                    Direction = 270,
                    BlurRadius = 15,
                    ShadowDepth = 5,
                    Opacity = 0.5
                },
                Child = stackPanel,
                Opacity = 0,
                RenderTransform = new TranslateTransform(50, 0)
            };

            return border;
        }

        private void AnimateNotificationIn(Border notification)
        {
            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            var slideIn = new DoubleAnimation
            {
                From = 50,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(400),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            notification.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            ((TranslateTransform)notification.RenderTransform).BeginAnimation(TranslateTransform.XProperty, slideIn);
        }

        private void AnimateNotificationOut(bool instant = false)
        {
            if (_notificationContainer == null || _notificationContainer.Children.Count == 0)
                return;

            var notification = _notificationContainer.Children[0] as Border;
            if (notification == null)
                return;

            if (instant)
            {
                _notificationContainer.Children.Clear();
                return;
            }

            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };

            var slideOut = new DoubleAnimation
            {
                From = 0,
                To = 50,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };

            fadeOut.Completed += (s, e) =>
            {
                _notificationContainer.Children.Clear();
            };

            notification.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            ((TranslateTransform)notification.RenderTransform).BeginAnimation(TranslateTransform.XProperty, slideOut);
        }

        private void AutoHideTimer_Tick(object sender, EventArgs e)
        {
            _autoHideTimer?.Stop();
            AnimateNotificationOut();
        }
    }
}

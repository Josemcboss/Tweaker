using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using Tweaker.Services;

namespace Tweaker.Controls
{
    public partial class GameBoosterWidget : UserControl
    {
        private bool _isExpanded = true;
        private Point _dragStartPoint;
        private bool _isDragging = false;
        private readonly GameBoosterService _gameBooster;

        public GameBoosterWidget()
        {
            InitializeComponent();

            _gameBooster = GameBoosterService.Instance;
            _gameBooster.GameModeChanged += GameBooster_GameModeChanged;

            // Animación de entrada
            Loaded += (s, e) =>
            {
                var slideIn = (Storyboard)FindResource("SlideIn");
                slideIn.Begin();
            };

            // Estado inicial
            UpdateStatus("Escritorio", false);
        }

        private void GameBooster_GameModeChanged(object? sender, GameModeChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.IsActive)
                {
                    UpdateStatus($"JUGANDO: {e.GameName?.ToUpper()}", true);
                }
                else
                {
                    UpdateStatus(e.Status == "Monitoreando..." ? "Monitoreando..." : "Escritorio", false);
                }
            });
        }

        private void UpdateStatus(string status, bool isGameActive)
        {
            TxtStatus.Text = status;

            // Cambiar color del LED
            var color = isGameActive ? Color.FromRgb(0, 255, 0) : // Verde
                       status.Contains("Monitoreando") ? Color.FromRgb(255, 165, 0) : // Naranja
                       Color.FromRgb(128, 128, 128); // Gris

            StatusLed.Fill = new SolidColorBrush(color);
            
            // Actualizar efecto de glow
            var effect = StatusLed.Effect as DropShadowEffect;
            if (effect != null)
            {
                effect.Color = color;
                effect.BlurRadius = isGameActive ? 15 : 10;
            }

            // Pulsar LED si está activo
            if (isGameActive)
            {
                StartLedPulse();
            }
            else
            {
                StopLedPulse();
            }
        }

        private void StartLedPulse()
        {
            var animation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.3,
                Duration = TimeSpan.FromSeconds(0.8),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            StatusLed.BeginAnimation(OpacityProperty, animation);
        }

        private void StopLedPulse()
        {
            StatusLed.BeginAnimation(OpacityProperty, null);
            StatusLed.Opacity = 1.0;
        }

        private void BtnToggle_Click(object sender, RoutedEventArgs e)
        {
            _isExpanded = !_isExpanded;

            if (_isExpanded)
            {
                var expand = (Storyboard)FindResource("Expand");
                expand.Begin();
                BtnToggle.Content = "?";
            }
            else
            {
                var collapse = (Storyboard)FindResource("Collapse");
                collapse.Begin();
                BtnToggle.Content = "+";
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            // Animación de salida
            var fadeOut = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(0.2)
            };

            fadeOut.Completed += (s, args) =>
            {
                Visibility = Visibility.Collapsed;
            };

            BeginAnimation(OpacityProperty, fadeOut);
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _dragStartPoint = e.GetPosition(null);
            Mouse.Capture((UIElement)sender);
        }

        private void ToggleAutoBooster_Checked(object sender, RoutedEventArgs e)
        {
            _gameBooster.StartMonitoring();
        }

        private void ToggleAutoBooster_Unchecked(object sender, RoutedEventArgs e)
        {
            _gameBooster.StopMonitoring();
        }

        // Permitir arrastrar el widget
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                var currentPosition = e.GetPosition(null);
                var offset = currentPosition - _dragStartPoint;

                var transform = RenderTransform as TranslateTransform ?? new TranslateTransform();
                transform.X += offset.X;
                transform.Y += offset.Y;

                RenderTransform = transform;
                _dragStartPoint = currentPosition;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (_isDragging)
            {
                _isDragging = false;
                Mouse.Capture(null);
            }
        }
    }
}

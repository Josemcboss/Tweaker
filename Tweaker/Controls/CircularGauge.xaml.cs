using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Tweaker.Controls
{
    public partial class CircularGauge : UserControl
    {
        public CircularGauge()
        {
            InitializeComponent();
            this.SizeChanged += (s, e) => UpdateGauge();
        }

        #region Dependency Properties

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(CircularGauge),
                new PropertyMetadata(0.0, OnGaugePropertyChanged));

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(CircularGauge),
                new PropertyMetadata(100.0, OnGaugePropertyChanged));

        public double MaxValue
        {
            get => (double)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(CircularGauge),
                new PropertyMetadata("Usage"));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty StrokeBrushProperty =
            DependencyProperty.Register(nameof(StrokeBrush), typeof(Brush), typeof(CircularGauge),
                new PropertyMetadata(Brushes.Red, OnGaugePropertyChanged));

        public Brush StrokeBrush
        {
            get => (Brush)GetValue(StrokeBrushProperty);
            set => SetValue(StrokeBrushProperty, value);
        }

        public static readonly DependencyProperty IconDataProperty =
            DependencyProperty.Register(nameof(IconData), typeof(Geometry), typeof(CircularGauge),
                new PropertyMetadata(null));

        public Geometry IconData
        {
            get => (Geometry)GetValue(IconDataProperty);
            set => SetValue(IconDataProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(CircularGauge),
                new PropertyMetadata(null));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(CircularGauge),
                new PropertyMetadata(null));

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty TemperatureProperty =
            DependencyProperty.Register(nameof(Temperature), typeof(double), typeof(CircularGauge),
                new PropertyMetadata(-1.0));

        public double Temperature
        {
            get => (double)GetValue(TemperatureProperty);
            set => SetValue(TemperatureProperty, value);
        }

        private static void OnGaugePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CircularGauge gauge)
            {
                gauge.UpdateGauge();
            }
        }

        #endregion

        private void UpdateGauge()
        {
            if (GaugePath == null) return;

            double value = Value;
            double max = MaxValue;
            if (max <= 0) max = 100.0;

            // Clamp value
            if (value < 0) value = 0;
            if (value > max) value = max;

            double percent = value / max;

            // Gauge Center, Radius
            double cx = 35;
            double cy = 35;
            double r = 25;

            if (percent <= 0.0001)
            {
                GaugePath.Data = null;
                return;
            }

            if (percent >= 0.9999)
            {
                // Full circle
                GaugePath.Data = new EllipseGeometry(new Point(cx, cy), r, r);
                return;
            }

            // Draw circular arc starting at top (-90 degrees)
            double startAngle = -Math.PI / 2.0; 
            double sweepAngle = 2.0 * Math.PI * percent;
            double endAngle = startAngle + sweepAngle;

            double sx = cx + r * Math.Cos(startAngle);
            double sy = cy + r * Math.Sin(startAngle);
            double ex = cx + r * Math.Cos(endAngle);
            double ey = cy + r * Math.Sin(endAngle);

            var figure = new PathFigure
            {
                StartPoint = new Point(sx, sy),
                IsClosed = false
            };

            var arc = new ArcSegment
            {
                Point = new Point(ex, ey),
                Size = new Size(r, r),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = sweepAngle > Math.PI,
                RotationAngle = 0
            };

            figure.Segments.Add(arc);

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            GaugePath.Data = geometry;
        }
    }
}

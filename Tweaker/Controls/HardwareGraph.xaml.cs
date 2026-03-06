using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using Tweaker.ViewModels;

namespace Tweaker.Controls
{
    public partial class HardwareGraph : UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty LineColorProperty =
            DependencyProperty.Register(nameof(LineColor), typeof(Color), typeof(HardwareGraph),
                new PropertyMetadata(Colors.DodgerBlue, OnVisualPropertyChanged));

        public static readonly DependencyProperty FillColorProperty =
            DependencyProperty.Register(nameof(FillColor), typeof(Color), typeof(HardwareGraph),
                new PropertyMetadata(Color.FromArgb(0x33, 0x1E, 0x90, 0xFF), OnVisualPropertyChanged));

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(HardwareGraph),
                new PropertyMetadata(100.0, OnVisualPropertyChanged));

        public static readonly DependencyProperty ValueUnitProperty =
            DependencyProperty.Register(nameof(ValueUnit), typeof(string), typeof(HardwareGraph),
                new PropertyMetadata("%"));

        public Color LineColor
        {
            get => (Color)GetValue(LineColorProperty);
            set => SetValue(LineColorProperty, value);
        }

        public Color FillColor
        {
            get => (Color)GetValue(FillColorProperty);
            set => SetValue(FillColorProperty, value);
        }

        public double MaxValue
        {
            get => (double)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public string ValueUnit
        {
            get => (string)GetValue(ValueUnitProperty);
            set => SetValue(ValueUnitProperty, value);
        }

        #endregion

        private readonly HardwareGraphViewModel _vm;
        private readonly Polyline _line;
        private readonly Polygon _fillArea;

        public HardwareGraph()
        {
            InitializeComponent();

            _vm = new HardwareGraphViewModel();
            DataContext = _vm;

            _line = new Polyline
            {
                StrokeThickness = 2,
                StrokeLineJoin = PenLineJoin.Round
            };

            _fillArea = new Polygon
            {
                Stroke = Brushes.Transparent
            };

            RootGrid.Children.Add(_fillArea);
            RootGrid.Children.Add(_line);

            UpdateBrushes();

            // initialize with zeros
            UpdateVisuals(new List<double>(new double[60]));

            this.SizeChanged += (s, e) => UpdateVisuals(_vm.GetValuesSnapshot());
        }

        private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HardwareGraph graph)
            {
                graph.UpdateBrushes();
                graph.UpdateVisuals(graph._vm.GetValuesSnapshot());
            }
        }

        private void UpdateBrushes()
        {
            _line.Stroke = new SolidColorBrush(LineColor);

            var gradient = new LinearGradientBrush();
            gradient.StartPoint = new Point(0, 0);
            gradient.EndPoint = new Point(0, 1);
            gradient.GradientStops.Add(new GradientStop(FillColor, 0.0));

            // Fade to transparent version of FillColor or just transparent
            var transparentColor = FillColor;
            transparentColor.A = 0;
            gradient.GradientStops.Add(new GradientStop(transparentColor, 1.0));

            _fillArea.Fill = gradient;
        }

        // Add a generic value and update visuals
        public void AddValue(double value)
        {
            _vm.AddValue(value);
            UpdateVisuals(_vm.GetValuesSnapshot());
        }

        private void UpdateVisuals(IList<double> values)
        {
            if (values == null || values.Count == 0 || RootGrid == null) return;

            double width = ActualWidth > 0 ? ActualWidth : 600;
            double height = ActualHeight > 0 ? ActualHeight : 200;

            if (width < 10 || height < 10) return;

            double xStep = width / (values.Count - 1);
            double maxVal = MaxValue > 0 ? MaxValue : 100.0;

            var points = new PointCollection();
            for (int i = 0; i < values.Count; i++)
            {
                double x = i * xStep;
                // Clamp value to 0-MaxValue range for visualization
                double val = Math.Min(Math.Max(values[i], 0), maxVal);
                double y = height - (val / maxVal) * height;
                points.Add(new Point(x, y));
            }

            _line.Points = points;

            // For fill polygon: add baseline points
            var polyPoints = new PointCollection(points.Count + 2);
            foreach (var p in points) polyPoints.Add(p);
            polyPoints.Add(new Point(width, height));
            polyPoints.Add(new Point(0, height));
            _fillArea.Points = polyPoints;
        }
    }
}

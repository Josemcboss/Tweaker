using System;
using System.Windows;
using System.Windows.Input;

namespace Tweaker.Windows
{
    public partial class Win32PriorityWindow : Window
    {
        public int SelectedValue { get; private set; }

        public Win32PriorityWindow(int currentValue)
        {
            InitializeComponent();
            SelectedValue = currentValue;
            SelectRadioButtonForValue(currentValue);
        }

        private void SelectRadioButtonForValue(int value)
        {
            switch (value)
            {
                case 42:
                    Radio2A.IsChecked = true;
                    break;
                case 41:
                    Radio29.IsChecked = true;
                    break;
                case 40:
                    Radio28.IsChecked = true;
                    break;
                case 38:
                    Radio26.IsChecked = true;
                    break;
                case 37:
                    Radio25.IsChecked = true;
                    break;
                case 36:
                    Radio24.IsChecked = true;
                    break;
                case 26:
                    Radio1A.IsChecked = true;
                    break;
                case 25:
                    Radio19.IsChecked = true;
                    break;
                case 24:
                    Radio18.IsChecked = true;
                    break;
                case 2:
                    Radio02.IsChecked = true;
                    break;
                default:
                    // If custom value, fallback to default or select nearest
                    Radio26.IsChecked = true;
                    break;
            }
        }

        private int GetSelectedValueFromUI()
        {
            if (Radio2A.IsChecked == true) return 42;
            if (Radio29.IsChecked == true) return 41;
            if (Radio28.IsChecked == true) return 40;
            if (Radio26.IsChecked == true) return 38;
            if (Radio25.IsChecked == true) return 37;
            if (Radio24.IsChecked == true) return 36;
            if (Radio1A.IsChecked == true) return 26;
            if (Radio19.IsChecked == true) return 25;
            if (Radio18.IsChecked == true) return 24;
            if (Radio02.IsChecked == true) return 2;
            return 38; // Default fallback
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedValue = GetSelectedValueFromUI();
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

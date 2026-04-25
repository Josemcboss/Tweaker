using System.Windows;

namespace Tweaker.Views;

public partial class InputDialog : Window
{
    public InputDialog()
    {
        InitializeComponent();
        ProfileNameTextBox.TextChanged += (_, __) => SaveButton.IsEnabled = !string.IsNullOrWhiteSpace(ProfileNameTextBox.Text);
        SaveButton.IsEnabled = false;
    }

    public string ProfileName => ProfileNameTextBox.Text.Trim();
    public string Description => DescriptionTextBox.Text.Trim();

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ProfileNameTextBox.Text))
        {
            return;
        }

        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}

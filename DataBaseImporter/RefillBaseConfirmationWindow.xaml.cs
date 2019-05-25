using System.Windows;
using System.Windows.Controls;

namespace DataBaseImporter
{
    public partial class RefillBaseConfirmationWindow
    {
        public bool Success { get; private set; }
        public RefillBaseConfirmationWindow()
        {
            InitializeComponent();
        }

        private void OkOnClick(object sender, RoutedEventArgs e)
        {
            if (ConfirmationTextBox.Text != "очистить базу данных") return;
            Success = true;
            Close();
        }

        private void CancelOnClick(object sender, RoutedEventArgs e)
        {
            Success = false;
            Close();
        }

        private void ConfirmationOnTextChanged(object sender, TextChangedEventArgs e)
        {
            OkButton.IsEnabled = ConfirmationTextBox.Text == "очистить базу данных";
        }
    }
}
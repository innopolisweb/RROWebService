using System;
using System.Windows;
using System.Windows.Controls;

namespace DataBaseImporter
{
    public partial class SheetIdWindow
    {
        public SheetIdWindow()
        {
            InitializeComponent();
        }

        public string SheetId { get; set; }

        private void OkOnClick(object sender, RoutedEventArgs e)
        {
            SheetId = IdTextBox.Text;
            Close();
        }

        private void IdTextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            OkButton.IsEnabled = !String.IsNullOrWhiteSpace(IdTextBox.Text);
        }
    }
}

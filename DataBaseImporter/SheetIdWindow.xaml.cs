using System.Windows;

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
    }
}

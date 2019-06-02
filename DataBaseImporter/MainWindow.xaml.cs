using System.Windows;
using DataBaseImporter.ViewModels;

namespace DataBaseImporter
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindowOnLoaded(object sender, RoutedEventArgs e)
        { 
            ((MainViewModel)DataContext).InitializeServiceCommand.Execute(null);
        }
    }
}

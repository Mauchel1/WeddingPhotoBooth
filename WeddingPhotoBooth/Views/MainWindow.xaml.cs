using System.Drawing.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WeddingPhotoBooth.Models;
using WeddingPhotoBooth.Services;
using WeddingPhotoBooth.ViewModels;
using static System.Windows.Forms.AxHost;


namespace WeddingPhotoBooth
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;


        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainViewModel();
            DataContext = _vm;

            Closing += MainWindow_Closing;

        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.Dispose();
            }
        }


        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            await _vm.StartPhotoSessionAsync();
        }

        private async void NeustartButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.Restart();
        }

        private async void DebugButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.State = PhotoBoothState.Preview;
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                System.Diagnostics.Debug.WriteLine(printer);
            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.PrintCurrentSession();
            _vm.Restart();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}

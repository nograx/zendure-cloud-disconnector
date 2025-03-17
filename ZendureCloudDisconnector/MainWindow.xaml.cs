using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZendureCloudDisconnector.ViewModel;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;

namespace ZendureCloudDisconnector
{
    /// <summary>
    /// interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainViewModel();

            this.DataContext = viewModel;
        }
        private void StartWatcherClick(object sender, RoutedEventArgs e)
        {
            viewModel.StartWatcher();
        }

        private void DisconnectClick(object sender, RoutedEventArgs e)
        {
            viewModel.DisconnectFromZendureCloud();
        }

        private void ReconnectClick(object sender, RoutedEventArgs e)
        {
            viewModel.ReconnectToZendureCloud();
        }                

        private void StartNotifyClick(object sender, RoutedEventArgs e)
        {
            viewModel.SubscribeToNotifyGattCharacteristic();
        }

        private void Donate_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://www.paypal.com/paypalme/PeterFrommert";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }

        private void Readme_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://github.com/nograx/zendure-cloud-disconnector";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }        
    }
}

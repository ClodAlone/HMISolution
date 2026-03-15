using DevExpress.Xpf.Core;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace DXApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        HubConnection connection;

        public MainWindow()
        {
            InitializeComponent();

            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5100/DataHub")
                .AddMessagePackProtocol()
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            Connect();
        }

        private async void Connect()
        {
            try
            {
                await connection.StartAsync();
                FetchData();
            }
            catch (Exception ex)
            {
            }
        }

        private async void FetchData()
        {
            try
            {
                await connection.InvokeAsync("GetData");
                Dispatcher.BeginInvoke(new Action(FetchData), System.Windows.Threading.DispatcherPriority.Normal);
            }
            catch (Exception ex)
            {
            }
        }

    }
}

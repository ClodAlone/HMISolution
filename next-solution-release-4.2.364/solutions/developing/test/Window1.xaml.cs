using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using OPCUAViewModel;
using Utilities;

namespace test
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            Tracing.SimpleLogging.WriteToLog("System", "Starting");

            InitializeComponent();
            DiscoveryViewModel = new OPCUADiscoveryViewModel(Dispatcher.CurrentDispatcher, null);
            DataContext = DiscoveryViewModel;
        }

        private OPCUADiscoveryViewModel DiscoveryViewModel;
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            String str = e.ToXml();
            MessageBox.Show(str);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (DiscoveryViewModel != null)
                DiscoveryViewModel.RefreshAll();
        }

        protected void Cmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

        }

        protected void Cmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void listBox2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox listWorkstation = sender as ListBox;
            String workstation = listWorkstation.SelectedItem as String;
            if (workstation != null)
                DiscoveryViewModel.DiscoverHostNameNow(workstation);
        }
    }
}

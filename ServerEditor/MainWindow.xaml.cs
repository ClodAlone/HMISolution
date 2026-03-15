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
using ServerEditor.ViewModels;

namespace ServerEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var vm = new MainViewModel();
            this.DataContext = vm;
            vm.ServerStarted += () => 
            {
                 // Switch to Client tab
                 MainTabs.SelectedIndex = 1;

                 // Trigger connection with configured endpoint
                 string endpoint = vm.ServerEndpointUrl;
                 if (string.IsNullOrWhiteSpace(endpoint))
                 {
                     var hostname = System.Net.Dns.GetHostName();
                     endpoint = $"opc.tcp://{hostname}:14840/SimpleOpcFileServer";
                 }
                 OpcBrowser.Connect(endpoint);
            };
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (DataContext is MainViewModel vm)
            {
                vm.StopServer();
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SelectedItem = e.NewValue as NodeItemViewModel;
                // Force command manager to re-evaluate CanExecute
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
}
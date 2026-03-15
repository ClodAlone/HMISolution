using DeployClientLib;
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
using System.Windows.Threading;
using Utilities;
using Utilities.WPF;

namespace DeployClientTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DeployServerClient deployServerClient = new DeployServerClient();
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
        }

        private async void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (!deployServerClient.Connected)
                return;

            dataGrid.ItemsSource = await deployServerClient.GetProcessesData();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            btnConnect.IsEnabled = false;
            var ret = await deployServerClient.InitiateConnection(textboxHost.Text, textboxUser.Text, textboxPassword.Text, 1, true);
            if (!ret)
            {
                MessageBox.Show("Failed to connect");
                btnConnect.IsEnabled = true;
            }
            else
            {
                dispatcherTimer.Stop();
                dispatcherTimer.Start();
                btnRemoveProcess.IsEnabled = btnAddProcess.IsEnabled = 
                    btnDisconnect.IsEnabled = 
                    btnProcessFileVersion.IsEnabled = btnPlatformVersion.IsEnabled = 
                    btnInstallLicense.IsEnabled = btnCheckLicense.IsEnabled = true;
                //ret = await deployServerClient.UploadFile("d:\\My Movie.mp4", "subfolder\\subfolder\\My Movie.mp4", 10240);
            }
        }

        private async void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            btnRemoveProcess.IsEnabled = btnAddProcess.IsEnabled = btnDisconnect.IsEnabled = false;
            var ret = await deployServerClient.CloseConnection();
            btnConnect.IsEnabled = true;
            dispatcherTimer.Stop();
        }

        private async void Button_AddProcess_Click(object sender, RoutedEventArgs e)
        {
            var ret = await deployServerClient.AddProcess("Notepad", "notepad.exe", "", true, "test");
        }

        private async void Button_RemoveProcess_Click(object sender, RoutedEventArgs e)
        {
            var ret = await deployServerClient.RemoveProces("Notepad", "test");
        }

        private async void btnProcessFileVersion_Click(object sender, RoutedEventArgs e)
        {
            var ret = await deployServerClient.FileVersion("Notepad.exe");
            if (String.IsNullOrEmpty(ret))
                MessageBox.Show("File version not available");
            else
                MessageBox.Show(String.Format("File Version : {0}", ret));
        }

        private async void btnPlatformVersion_Click(object sender, RoutedEventArgs e)
        {
            var ret = await deployServerClient.GetPlatformDescription();
            MessageBox.Show(String.Format("OS Architecture : {0}, Framework Description : {1}, Process Architecture : {2}, OS Description : {3}, IsLinux : {4}, IsWindows : {5}, is OSX : {6}", 
                ret.OSArchitecture, ret.FrameworkDescription, ret.ProcessArchitecture, ret.OSDescription, ret.IsLinux, ret.IsWindows, ret.IsOSX));
        }

        private async void btnInstallLicense_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".mlztrack";
            dlg.Filter = "MLZTrack Files|*.mlztrack";
            var result = dlg.ShowDialog();

            if (result == true)
            {
                try
                {
                    Tuple<string, string> folders = null;
                    if (!String.IsNullOrWhiteSpace(textboxCompanyName.Text) && !String.IsNullOrWhiteSpace(textboxAppName.Text))
                        folders = new Tuple<string, string>(textboxCompanyName.Text, textboxAppName.Text);

                    if (await deployServerClient.FileExists(dlg.SafeFileName, "License.key", folders))
                        if (MessageBox.Show("File exist, overwerite ?", "Install License", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                            return;

                    await deployServerClient.UploadLicense(dlg.FileName, "License.key", 1024000, folders);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                MessageBox.Show("License transfered");
            }
        }

        private async void btnCheckLicense_Click(object sender, RoutedEventArgs e)
        {
            var layoutcontrol = new MSZui.UserControl3(false, true);
            var dialog = new GeneralDialogContent(layoutcontrol, GeneralDialogButtons.CloseHelpButtons)
            {
                Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                Title = "License",
                HelpLink = "DongleOption"
            };
            try
            {
                Tuple<string, string> folders = null;
                if (!String.IsNullOrWhiteSpace(textboxCompanyName.Text) && !String.IsNullOrWhiteSpace(textboxAppName.Text))
                    folders = new Tuple<string, string>(textboxCompanyName.Text, textboxAppName.Text);
                await deployServerClient.LoadLicenseData(layoutcontrol.LicenseModel.OptionsTags, folders).ContinueWith(model =>
                {
                    layoutcontrol.DataContext = model.Result;
                    layoutcontrol.UpdateUI();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            dialog.ShowDialog();
        }
    }
}

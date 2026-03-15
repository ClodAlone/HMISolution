using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using Utilities.WPF;
using WPFUtilities;

namespace InstallDongle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            SetPropertyHelper();
            Loaded += (o, e) =>
            {
                ThemeHelper.SetTheme(this, Properties.Settings.Default.Skin);
                //ThemeHelper.SetDefaultUserLookAndFeel();
                FontSize = 12;
                Topmost = false;
                ApplicationPropertiesHelper.SetProperty("CurrentSkinForeColor", Brushes.White);
            };
        }

        private static void SetPropertyHelper()
        {
            var commonFolder = String.Format("{0}\\{1}\\{2}",
                             Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                             Properties.Settings.Default.CompanyName,
                             Properties.Settings.Default.CommonApplicationFolder);
            ApplicationPropertiesHelper.SetProperty("CommonFolder", commonFolder);

            var userFolder = String.Format("{0}\\{1}\\{2}",
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            Properties.Settings.Default.CompanyName,
                            Properties.Settings.Default.CommonApplicationFolder);
            ApplicationPropertiesHelper.SetProperty("UserFolder", userFolder);

            var projectFolder = String.Format("{0}\\{1}\\{2}",
                Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                Properties.Settings.Default.CompanyName,
                Properties.Settings.Default.CommonApplicationFolder);
            ApplicationPropertiesHelper.SetProperty("ProjectFolder", projectFolder);
        }

        private void Button_Close(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            DongleOptions();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            GetKey();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            DongleService();
        }

        void DongleOptions()
        {
            var layoutcontrol = new MSZui.UserControl3(false);
            var dialog = new Utilities.GeneralDialog(layoutcontrol)
            //var wnd = new Window
            {
                Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                Title = Properties.Resources.DongleRibbonLabelDongleOptions,
                HelpLink = "DongleOption",
                bShowOk = false
            };

            var foreg = ApplicationPropertiesHelper.GetProperty("CurrentSkinForeColor") as Brush;
            ThemeHelper.SetTheme(dialog, Properties.Settings.Default.Skin);
            dialog.Loaded += (o, e) =>
            {
                ThemeHelper.SetTheme(layoutcontrol, Properties.Settings.Default.Skin);
                FontSize = 12;
                var labelList = layoutcontrol.GetVisualChildrenOfType<Label>();
                var textblockList = layoutcontrol.GetVisualChildrenOfType<TextBlock>();
                var checkboxList = layoutcontrol.GetVisualChildrenOfType<CheckBox>();
                foreach (var l in labelList)
                    l.Foreground = foreg;
                foreach (var t in textblockList)
                    t.Foreground = foreg;
                foreach (var c in checkboxList)
                    c.Foreground = foreg;
            };
            try
            {
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                dialog.Close();
            }
        }
        private void DongleService()
        {
            try
            {
                string entryPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                string path = System.IO.Path.Combine(entryPath, $"{Properties.Settings.Default.ServiceInstallProcess}");
                string prjpath = System.IO.Path.Combine(entryPath, $"{Properties.Settings.Default.Service}");
                var arguments = string.Format("/F\"{1}\" /N\"{0}\" /Y\"{2}\" /Z\"{3}\" /W\"{4}\"", Properties.Settings.Default.DongleNetServiceName, prjpath, Properties.Resources.DongleNetServiceDisplayName, Properties.Resources.DongleNetServiceTitle, Properties.Settings.Default.Skin);
                RunElevated.Run(path, arguments);
            }
            catch (Exception ex)
            {
            }
        }
        IUIMsgBoxAlertService uIMsgBoxAlertServiceComponent;
        void GetKey()
        {
            try
            {
                if (uIMsgBoxAlertServiceComponent == null)
                {
                    uIMsgBoxAlertServiceComponent = new UIMsgBoxAlertServiceComponent() as IUIMsgBoxAlertService;
                    uIMsgBoxAlertServiceComponent.Initialize();
                }

                var layoutcontrol = new MSZui.UserControl2(uIMsgBoxAlertServiceComponent, true);
                var dialog = new GeneralDialog(layoutcontrol)
                {
                    Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                    Title = Properties.Resources.DongleRibbonLabelGetKey,
                    HelpLink = "LicenceManager",
                };

                var foreg = ApplicationPropertiesHelper.GetProperty("CurrentSkinForeColor") as Brush;
                ThemeHelper.SetTheme(dialog, Properties.Settings.Default.Skin);
                dialog.Loaded += (o, e) =>
                {
                    ThemeHelper.SetTheme(layoutcontrol, Properties.Settings.Default.Skin);
                    FontSize = 12;
                    var labelList = layoutcontrol.GetVisualChildrenOfType<Label>();
                    var textblockList = layoutcontrol.GetVisualChildrenOfType<TextBlock>();
                    var checkboxList = layoutcontrol.GetVisualChildrenOfType<CheckBox>();
                    foreach (var l in labelList)
                        l.Foreground = foreg;
                    foreach (var t in textblockList)
                        t.Foreground = foreg;
                    foreach (var c in checkboxList)
                        c.Foreground = foreg;
                };
                
                try
                {
                    dialog.ShowDialog();
                }
                catch (Exception ex)
                {
                    dialog.Close();
                }
            }
            catch
            { }
        }
    }
}

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
using Mindscape.WpfElements.Themes;
using DevExpress.Xpf.Core;
using WPFUtilities;
using Utilities;

namespace UFUACertificateChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DXWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                //DataContext = App.cl;
                var t = DataContext as CommandLineOptions;
                ApplicationPropertiesHelper.SetProperty("CurrentSkin", t.CurrentSkin);
                ThemeHelper.SetTheme(this);

                //propertyGrid.Resources.MergedDictionaries.Add(new Alloy());

                if (t != null)
                {
                    string s = t.ExePath;
                }
                UpdateCurrentCertificate();
            };
        }

        void UpdateCurrentCertificate()
        {
            var t = DataContext as CommandLineOptions;
            if (t != null)
            {
                CertificateMng cert = new CertificateMng();
                propertyGrid.SelectedObject = cert.FindCertificate(t.SectionName, t.ExePath, t.ApplicationType, t.ApplicationName);
            }
        }

        private void CheckAndCreate_Click(object sender, RoutedEventArgs e)
        {
             var t = DataContext as CommandLineOptions;
             if (t != null)
             {
                 CertificateMng cert = new CertificateMng();
                 cert.CheckCertificate(t.SectionName, t.ExePath, t.ApplicationType, t.ApplicationName);
                 UpdateCurrentCertificate();
             }
        }

        private void CreateNew_Click(object sender, RoutedEventArgs e)
        {
            var t = DataContext as CommandLineOptions;
            if (t != null)
            {
                try
                {
                    CertificateMng cert = new CertificateMng();
                    cert.CreateNewCertificate(t.SectionName, t.ExePath, appname: t.ApplicationName);
                    UpdateCurrentCertificate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format(Properties.Resources.ErrorCreatingCertificate, ex.Message));
                }
            }
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            var u = new UIMsgBoxAlertService.ComponentService.UIMsgBoxAlertServiceComponent();
            string s = u.ShowOpenFileDialog("(*.cer)|*.cer|(*.pfx)|*.pfx|All files (*.*)|*.*");

            var t = DataContext as CommandLineOptions;
            if (t != null && s.Length > 0)
            {
                CertificateMng cert = new CertificateMng();
                cert.installCertificate(s, t.SectionName, t.ExePath, t.ApplicationType, t.ApplicationName);
                UpdateCurrentCertificate();
            }
        }
    }
}

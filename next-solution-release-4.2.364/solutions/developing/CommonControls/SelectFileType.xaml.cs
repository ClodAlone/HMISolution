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
using Ookii.Dialogs.Wpf;
using Utilities;
using Utilities.WPF;
using UIMsgBoxAlertService.ComponentService;
using HelpProvider.ComponentService;

namespace CommonControls
{
    /// <summary>
    /// Interaction logic for SelectFileType.xaml
    /// </summary>
    public partial class SelectFileType : UserControl
    {
        readonly bool bOpen;
        readonly String Filter;
        readonly String DefExtension;
        IUIMsgBoxAlertService UIMsgBoxAlertService;
        IHelpProvider HelpProvider;

        public String currentUri;

        public SelectFileType(bool bopen, String filter, String defextesion = null, IUIMsgBoxAlertService uIMsgBoxAlertService = null, IHelpProvider helpProvider = null)
        {
            InitializeComponent();
            UIMsgBoxAlertService = uIMsgBoxAlertService;
            HelpProvider = helpProvider;
            bOpen = bopen;
            Filter = filter;
            DefExtension = defextesion;
        }

        private void Button_Click_DataSource(object sender, RoutedEventArgs e)
        {
            var connectionstring = string.Empty;
            if (CommonControls.ConnectionWizardDialog.GetConnectionString(ref connectionstring, "ConnectionWizard", this.FindParent<Window>(), UIMsgBoxAlertService, HelpProvider))
            {
                currentUri = connectionstring;
                this.FindParent<Window>().DialogResult = true;
            }
        }

        private void Button_Click_FileSource(object sender, RoutedEventArgs e)
        {
            if (bOpen)
            {
                var dialog = new VistaOpenFileDialog();
                dialog.Filter = Filter;
                if (!String.IsNullOrEmpty(DefExtension))
                {
                    dialog.DefaultExt = DefExtension;
                    dialog.AddExtension = true;
                }
                if (dialog.ShowDialog() == true)
                {
                    currentUri = dialog.FileName;
                    this.FindParent<Window>().DialogResult = true;
                }
            }
            else
            {
                var dialog = new VistaSaveFileDialog();
                dialog.Filter = Filter;
                if (!String.IsNullOrEmpty(DefExtension))
                {
                    dialog.DefaultExt = DefExtension;
                    dialog.AddExtension = true;
                }
                if (dialog.ShowDialog() == true)
                {
                    currentUri = dialog.FileName;
                    this.FindParent<Window>().DialogResult = true;
                }
            }
        }
    }
}

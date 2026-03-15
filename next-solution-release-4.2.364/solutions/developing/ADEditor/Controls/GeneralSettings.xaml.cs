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
using Utilities;
using Utilities.WPF;
using CommonControls;
using DocumentManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using HelpProvider.ComponentService;

namespace ADEditor.Controls
{
    /// <summary>
    /// Interaction logic for GeneralSettings.xaml
    /// </summary>
    public partial class GeneralSettings : UserControl
    {
        IDocument _Document;
        public IDocument Document
        {
            get
            {
                return _Document;
            }
            set
            {
                _Document = value;
            }
        }

        public GeneralSettings()
        {
            InitializeComponent();

            Foreground = ApplicationPropertiesHelper.GetProperty("CurrentSkinForeColor") as Brush;
        }

        private void textLogConnection_DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            IUIMsgBoxAlertService uIMsgBoxAlertService = null;
            IHelpProvider helpProvider = null;
            if (Document != null)
            {
                uIMsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                helpProvider = Document.GetService(typeof(IHelpProvider)) as IHelpProvider;
            }
            var connectionstring = XpoHelpers.XpoHelper.NormalizeConnectionString(textLogConnection.Text, Document?.rootBase);
            if (CommonControls.ConnectionWizardDialog.GetConnectionString(ref connectionstring, "ConnectionSourcePropertyEditor", this.FindParent<Window>(), uIMsgBoxAlertService, helpProvider))
            {
                var settings = DataContext as ADModel.ADGeneralSettings;
                settings.EventDefaultConnection = XpoHelpers.XpoHelper.AddPlaceholderToConnectionString(connectionstring, Document?.rootBase);
            }
        }

        private void textLogConnection_DlgButton_Clear(object sender, RoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                textLogConnection.Text = string.Empty;
                var settings = DataContext as ADModel.ADGeneralSettings;
                settings.EventDefaultConnection = string.Empty;
            }
        }

        bool bLoaded = false;
        private void OnVerifyCustomMessageCheck(object sender, RoutedEventArgs e)
        {
            if(bLoaded)
                VerifyCustomMessageCheck();
        }
        private void VerifyCustomMessageCheck()
        {
            var settings = DataContext as ADModel.ADGeneralSettings;
            if (settings.CustomMessage.Value)
            {
                if (!settings.AddAlarmState.Value && !settings.AddDateTime.Value &&
                    !settings.AddNotificationName.Value && !settings.AddNotificationText.Value &&
                    !settings.AddServerText.Value)
                {
                    MessageBox.Show(string.Format(Properties.Resources.CustomMessageCheckInvalid));
                }
            }
        }


        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if(!bLoaded)
            {
                bLoaded = true;
                VerifyCustomMessageCheck();
            }
                
        }
    }
}

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
using CommonControls;
using Utilities;
using Utilities.WPF;
using DevExpress.Xpo;
using System.Speech.Synthesis;
using UFUAEditor.ComponentService;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for NewConfigurationSettings.xaml
    /// </summary>
    public partial class NewConfigurationSettings : UserControl
    {
        public NewConfigurationSettings()
        {
            InitializeComponent();

            Foreground = ApplicationPropertiesHelper.GetProperty("CurrentSkinForeColor") as Brush;
            textEditMaxAge.Mask = String.Format("d '({0})' hh:mm:ss", Properties.Resources.TimeSpanFormatDaysPart);

            //DataContextChanged += (o, e) =>
            //    {
            //        layoutItems.CurrentItem = DataContext;
            //    };

            var userName = UFUAServerInfo.UFUAServerInfo.GetCFR21UserName();
            if (String.IsNullOrEmpty(userName))
            {
                labelDataProtectionEnabled.Visibility = Visibility.Collapsed;
                chkDataProtectionEnabled.Visibility = Visibility.Collapsed;
                labelAuditTraceDefConnection.Visibility = Visibility.Collapsed;
                editAuditTraceDefConnection.Visibility = Visibility.Collapsed;
            }
        }

        private void textEditHistorianConnection_DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            var doc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.Workspace.ContextDocument;
            string connection = XpoHelpers.XpoHelper.NormalizeConnectionString(textEditHistorianConnection.Text, doc?.rootBase);
            if (CommonControls.ConnectionWizardDialog.GetConnectionString(ref connection, "ConnectionWizard", this.FindParent<Window>(), UFUAEditorManagerComponent.ufuaEditorManagerComponent.UIInterface, UFUAEditorManagerComponent.ufuaEditorManagerComponent.HelpProvider))
            {
                using (var cursor = new WaitCursor())
                {
                    var settings = DataContext as UFUAModel.UFUAConfiguration;
                    settings.HistorianDefaultConnection = XpoHelpers.XpoHelper.AddPlaceholderToConnectionString(connection, doc?.rootBase);

                    if (!UFUAHistorianModel.Helpers.HistorianHelper.TryUpdateSchema<UFUAHistorianModel.UFUAAuditDataLog>(connection))
                    {
                        if (UFUAEditorManagerComponent.ufuaEditorManagerComponent.UIInterface != null)
                            UFUAEditorManagerComponent.ufuaEditorManagerComponent.UIInterface.ShowInformation(Properties.Resources.UnableToOpenDatabaseConnection);
                        else
                            MessageBox.Show(Properties.Resources.UnableToOpenDatabaseConnection, Properties.Resources.CSHeader, MessageBoxButton.OK);
                    }
                }
            }
        }

        private void textEditHistorianConnection_DlgButton_Clear(object sender, RoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                textEditHistorianConnection.Text = string.Empty;
                var settings = DataContext as UFUAModel.UFUAConfiguration;
                settings.HistorianDefaultConnection = string.Empty;
            }
        }

        private void textEditEventConnection_DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            var doc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.Workspace.ContextDocument;
            string connection = XpoHelpers.XpoHelper.NormalizeConnectionString(textEditEventConnection.Text, doc?.rootBase);
            if (CommonControls.ConnectionWizardDialog.GetConnectionString(ref connection, "ConnectionWizard", this.FindParent<Window>(), UFUAEditorManagerComponent.ufuaEditorManagerComponent.UIInterface, UFUAEditorManagerComponent.ufuaEditorManagerComponent.HelpProvider))
            {
                using (var cursor = new WaitCursor())
                {
                    var settings = DataContext as UFUAModel.UFUAConfiguration;
                    settings.EventDefaultConnection = XpoHelpers.XpoHelper.AddPlaceholderToConnectionString(connection, doc?.rootBase);

                    if (!UFUAHistorianModel.Helpers.HistorianHelper.TryUpdateSchema<UFUAHistorianModel.UFUAAuditLogItem>(connection))
                    {
                        if (UFUAEditorManagerComponent.ufuaEditorManagerComponent.UIInterface != null)
                            UFUAEditorManagerComponent.ufuaEditorManagerComponent.UIInterface.ShowInformation(Properties.Resources.UnableToOpenDatabaseConnection);
                        else
                            MessageBox.Show(Properties.Resources.UnableToOpenDatabaseConnection, Properties.Resources.CSHeader, MessageBoxButton.OK);
                    }
                }
            }
        }

        private void textEditEventConnection_DlgButton_Clear(object sender, RoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                textEditEventConnection.Text = string.Empty;
                var settings = DataContext as UFUAModel.UFUAConfiguration;
                settings.EventDefaultConnection = string.Empty;
            }
        }
        private void textEditAuditConnection_DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            var doc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.Workspace.ContextDocument;
            string connection = XpoHelpers.XpoHelper.NormalizeConnectionString(textEditAuditConnection.Text, doc?.rootBase);
            if (CommonControls.ConnectionWizardDialog.GetConnectionString(ref connection, "ConnectionWizard", this.FindParent<Window>(), UFUAEditorManagerComponent.ufuaEditorManagerComponent.UIInterface, UFUAEditorManagerComponent.ufuaEditorManagerComponent.HelpProvider))
            {
                using (var cursor = new WaitCursor())
                {
                    var settings = DataContext as UFUAModel.UFUAConfiguration;
                    settings.AuditTraceDefaultConnection = XpoHelpers.XpoHelper.AddPlaceholderToConnectionString(connection, doc?.rootBase);

                    if (!UFUAHistorianModel.Helpers.HistorianHelper.TryUpdateSchema<UFUAHistorianModel.UFUAAuditDataLog>(connection))
                    {
                        if (UFUAEditorManagerComponent.ufuaEditorManagerComponent.UIInterface != null)
                            UFUAEditorManagerComponent.ufuaEditorManagerComponent.UIInterface.ShowInformation(Properties.Resources.UnableToOpenDatabaseConnection);
                        else
                            MessageBox.Show(Properties.Resources.UnableToOpenDatabaseConnection, Properties.Resources.CSHeader, MessageBoxButton.OK);
                    }
                }
            }
        }

        private void textEditAuditConnection_DlgButton_Clear(object sender, RoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                textEditAuditConnection.Text = string.Empty;
                var settings = DataContext as UFUAModel.UFUAConfiguration;
                settings.AuditTraceDefaultConnection = string.Empty;
            }
        }


        //bool bComboVoicesFilled;
        //private void comboSpeechVoices_DropDownOpened(object sender, EventArgs e)
        //{
        //    if (bComboVoicesFilled)
        //        return;
        //    bComboVoicesFilled = true;
        //    using(var cursor = new WaitCursor())
        //    {
        //        try
        //        {
        //            var synthesizer = new SpeechSynthesizer();
        //            comboSpeechVoices.ItemsSource = (from c in synthesizer.GetInstalledVoices() select c.VoiceInfo.Name).ToList();

        //        }
        //        catch (Exception)
        //        {
        //        }
        //    }
        //}
    }
}

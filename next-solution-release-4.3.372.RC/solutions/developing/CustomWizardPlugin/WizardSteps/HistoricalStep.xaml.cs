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
using UriResolver.ComponentService;
using System.IO;
using Utilities;
using Utilities.WPF;
using CustomWizardPlugin.ComponentService;
using DevExpress.Xpo.DB;
using DevExpress.Xpo;
using DocumentManager.ComponentService;

namespace CustomWizardPlugin
{
    /// <summary>
    /// Interaction logic for HistoricalStep.xaml
    /// </summary>
    public partial class HistoricalStep : UserControl, IWizardElement
    {
        public HistoricalStep()
        {
            InitializeComponent();
            InitLabels();
        }

        void InitLabels()
        {
            var cultInfo = new System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.Name, false).TextInfo;
            labelHisTitle.Text = cultInfo.ToTitleCase(Properties.Resources.Step4Comment);
            buttonHisConnectoinString.Content = cultInfo.ToTitleCase(Properties.Resources.GetConnectionString);
            buttonDLRConnectionString.Content = cultInfo.ToTitleCase(Properties.Resources.GetConnectionString);
            labelDlrTitle.Text = cultInfo.ToTitleCase(Properties.Resources.AddDLRConnectionString);
        }

        public bool Execute()
        {
            if ((bool)HisGroup.IsEnabled || (bool)DlrGroup.IsEnabled)
            {
                AddHisConnectionString();
            } 
            return false;
        }

        private void buttonHisConnectoinString_Click(object sender, RoutedEventArgs e)
        {
            var connectionWizard = new CommonControls.ConnectionWizard(CustomWizardPluginComponent.wizard.ProjectView?.UIInterface, CustomWizardPluginComponent.wizard.ProjectView?.helpProvider) { ConnectionString = historyconnectionString.Text };
            var wnd = new GeneralDialogContent(connectionWizard) { Owner = this.FindParent<Window>(),
                HelpLink = "CustomWizard_HisConnectoinString"
            };
            if (wnd.ShowDialog() == true)
            {
                historyconnectionString.Text = connectionWizard.ConnectionString;
            }
        }

        private void buttonDLRConnectionString_Click(object sender, RoutedEventArgs e)
        {
            var connectionWizard = new CommonControls.ConnectionWizard(CustomWizardPluginComponent.wizard.ProjectView?.UIInterface, CustomWizardPluginComponent.wizard.ProjectView?.helpProvider) { ConnectionString = dlrconnectionString.Text };
            var wnd = new GeneralDialogContent(connectionWizard) { Owner = this.FindParent<Window>(),
                HelpLink = "CustomWizard_DLRConnectoinString"
            };
            if (wnd.ShowDialog() == true)
            {
                dlrconnectionString.Text = connectionWizard.ConnectionString;
            }
        }

        private void AddHisConnectionString()
        {
            UpdateDocument();
        }

        void UpdateDocument()
        {
            var connString = CustomWizardPluginComponent.ProjectView.GetServerIOConnectionStringFromUri(CustomWizardPluginComponent.ProjectUri);
            if (connString != null)
            {
                using (var dl = XpoDefault.GetDataLayer(connString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                {
                    using (var uow = new UnitOfWork(dl))
                    {
                        if ((bool)HisGroup.IsEnabled)
                        {
                            //Add Historical connection string
                            var Configuration = GetConfiguration(uow);
                            Configuration.HistorianDefaultConnection = historyconnectionString.Text;
                        }

                        if ((bool)DlrGroup.IsEnabled)
                        {
                            //Add Datalogger connection string
                            var Configuration = GetConfiguration(uow);
                            Configuration.EventDefaultConnection = dlrconnectionString.Text;
                        }

                        uow.CommitChanges();
                    }
                }
            }
        }

        UFUAModel.UFUAConfiguration GetConfiguration(UnitOfWork uow)
        {
            var list = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(uow, true)/*.AsParallel()*/ select tag).ToList();
            if (list.Count == 0)
                return new UFUAModel.UFUAConfiguration(uow);
            return list[0];
        }
    }
}

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
using System.IO;
using CustomWizardPlugin.ComponentService;
using DevExpress.Xpo.DB;
using DevExpress.Xpo;
using DocumentManager.ComponentService;

namespace CustomWizardPlugin
{
    /// <summary>
    /// Interaction logic for DataloggerStep.xaml
    /// </summary>
    public partial class DataloggerStep : UserControl, IWizardElement
    {
        public DataloggerStep()
        {
            InitializeComponent();
        }

        public bool Execute()
        {
            // throw new NotImplementedException();
            if ((bool)checkboxEnable.IsChecked &&
                ((bool)checkboxDLROnChange.IsChecked ||
                (bool)checkboxDLR5Sec.IsChecked ||
                (bool)checkboxDLR30Sec.IsChecked ||
                (bool)checkboxDLR1Min.IsChecked))
            {
                AddHistaricals();
            }
            return false;
        }

        void AddHistaricals()
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
                        if ((bool)checkboxDLROnChange.IsChecked)
                        {
                            //checkboxDLROnChange
                            var datalogger = new UFUAModel.UFUAHistorianSettings(uow) { Name = Properties.Resources.DLROnChange };
                        }
                        if ((bool)checkboxDLR5Sec.IsChecked)
                        {
                            //checkboxDLR5Sec
                            var datalogger = new UFUAModel.UFUAHistorianSettings(uow) { Name = Properties.Resources.DLR5Sec };
                            datalogger.MinTimeInterval = new TimeSpan(0, 0, 5);
                            datalogger.MaxTimeInterval = new TimeSpan(0, 0, 5);
                        }
                        if ((bool)checkboxDLR30Sec.IsChecked)
                        {
                            //checkboxDLR30Sec
                            var datalogger = new UFUAModel.UFUAHistorianSettings(uow) { Name = Properties.Resources.DLR30Sec };
                            datalogger.MinTimeInterval = new TimeSpan(0, 0, 30);
                            datalogger.MaxTimeInterval = new TimeSpan(0, 0, 30);
                        }
                        if ((bool)checkboxDLR1Min.IsChecked)
                        {
                            //checkboxDLR1Min
                            var datalogger = new UFUAModel.UFUAHistorianSettings(uow) { Name = Properties.Resources.DLR1Min };
                            datalogger.MinTimeInterval = new TimeSpan(0, 1, 0);
                            datalogger.MaxTimeInterval = new TimeSpan(0, 1, 0);
                        }

                        uow.CommitChanges();
                    }
                }
            }
        }

        private void checkboxEnable_Click(object sender, RoutedEventArgs e)
        {
            gridControlContainer.IsEnabled = (bool)checkboxEnable.IsChecked;
        }
    }
}

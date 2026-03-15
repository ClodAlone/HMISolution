using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UriResolver.ComponentService;
using System.IO;
using System.Xml.Linq;
using DevExpress.Xpo.DB;
using DevExpress.Xpo;
using System.Windows.Documents;
using CustomWizardPlugin.ComponentService;
using Utilities.WPF;
using Utilities;
using UFInterfaces.Editors;
using VFS;
using UFUAEditor.Document;
using UFUAEditor.ComponentService;
using UFUserEditor.Document;
using DocumentManager.ComponentService;

namespace CustomWizardPlugin
{
    /// <summary>
    /// Interaction logic for DriverStep.xaml
    /// </summary>
    public partial class DriverStep : UserControl, IWizardElement
    {
        private List<DriverXmlInfo> selectedDrivers = new List<DriverXmlInfo>();

        public DriverStep()
        {
            InitializeComponent();
            SetBusy(false);
            //WithoutRadio.IsChecked = true;
        }
        public bool Execute()
        {
            //if ((bool)WithRadio.IsChecked && driverList.Items.Count != 0)
            if (driverList.Items.Count != 0)
            {
                AddDrivers();
            }
            return false;
        }

        void AddDrivers()
        {
            foreach (var item in driverList.Items)
            {
                var result = selectedDrivers.Find(
                delegate(DriverXmlInfo ds)
                {
                    return ds.FriendlyName == item;
                }
                );

                if (result != null)
                {
                    UpdateDocument(result);
                }
            }
        }

        void UpdateDocument(DriverXmlInfo driverInfo)
        {
            var connString = CustomWizardPluginComponent.ProjectView.GetServerIOConnectionStringFromUri(CustomWizardPluginComponent.ProjectUri);
            if (connString != null)
            {
                using (var dl = XpoDefault.GetDataLayer(connString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                {
                    using (var uow = new UnitOfWork(dl))
                    {
                        var driver = new UFUAModel.UFUACommunicationDriver(uow);
                        driver.FriendlyName = driverInfo.FriendlyName;
                        driver.Name = driverInfo.AssemblyName.Substring(0, driverInfo.AssemblyName.Length - 4);
                        driver.AssemblyName = driverInfo.AssemblyName;

                        var Configuration = GetConfiguration(uow);
                        Configuration.ComunicationDrivers.Add(driver);

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
        
        private void buttonAddDriver_Click(object sender, RoutedEventArgs e)
        {
            var usercontrol = CustomWizardPluginComponent.ProjectView.UFUAEditorManager.GetDriverListControl();
            SetBusy(true);

            usercontrol.Loaded += (o, re) =>
            {
                SetBusy(false);
            };

            GeneralDialogContent dlg = new GeneralDialogContent(usercontrol)
            {
                Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                Title = Properties.Resources.ScreenTemplateTitle,
                HelpLink = "CustomWizard_Driver"
            };
            
            if (dlg.ShowDialog() != true)
            {
                return;
            }

            var selectedDriver = usercontrol.DataContext;
            if (selectedDrivers != null && selectedDriver != null)
            {
                if (!selectedDrivers.Contains(selectedDriver))
                    selectedDrivers.Add((DriverXmlInfo)selectedDriver);
            }
            else
            {
                return;
            }

            driverList.Items.Clear();

            for (int x = 0; x < selectedDrivers.Count(); x++)
            {
                if (selectedDrivers.ElementAt(x) != null)
                    driverList.Items.Add(selectedDrivers.ElementAt(x).FriendlyName);
            }
        }

        private void buttonRemoveDriver_Click(object sender, RoutedEventArgs e)
        {
            if (driverList.SelectedItem == null)
                return;

            DriverXmlInfo result = selectedDrivers.Find(
            delegate(DriverXmlInfo ds)
            {
                return ds.FriendlyName == driverList.SelectedItem;
            }
            );

            if (result != null)
            {
                selectedDrivers.Remove(result);
                driverList.Items.Remove(driverList.SelectedItem);
                driverList.Refresh();
            }
        }

        void SetBusy(bool bSet)
        {
            busyControl.Visibility = bSet ? Visibility.Visible : Visibility.Collapsed;
            busyContent.Visibility = busyControl.Visibility;
            busyControl.Refresh();
            busyContent.Refresh();
        }

    }
}

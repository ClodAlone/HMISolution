using CustomWizardPlugin.ComponentService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UIMsgBoxAlertService.ComponentService;

namespace CustomWizardPlugin
{   
    /// <summary>
    /// Helper class object used in the startup button's link.
    /// </summary>
    public class ButtonHelper
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public BitmapImage Image { get; set; }
        public Action Action { get; set; }
        public ArchType Architecture { get; set; }
    }
    /// <summary>
    /// Interaction logic for ProjectWizardPathAndType.xaml
    /// </summary>
    public partial class ProjectArchitecture : UserControl, IWizardElement
    {
        readonly ObservableCollection<String> listServers = new ObservableCollection<string>();
        System.Globalization.TextInfo textInfo;
        bool bEnableLocal;
        bool bEnableRemote;
        public ProjectArchitecture()
        {
            InitializeComponent();
            textInfo = new System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.Name, false).TextInfo;
            InitLabels();
            InitTransporCombo();
            InitListBox();
            textEditFullSynchronizationTimeSpan.Mask = String.Format("d '({0})' hh:mm", Properties.Resources.TimeSpanFormatDaysPart);
        }

        void InitLabels()
        {
            redundancyFullSynchronizationStartTime.Text = textInfo.ToTitleCase(Properties.Resources.RedundancyFullSynchronizationStartTime);
            serversLabel.Text = textInfo.ToTitleCase(Properties.Resources.ServerList);
            transportLabel.Content = textInfo.ToTitleCase(Properties.Resources.TransportType);
            serverNameLabel.Content = textInfo.ToTitleCase(Properties.Resources.ServerName);
            portNumberLabel.Content = textInfo.ToTitleCase(Properties.Resources.PortNumber);
        }

        void InitTransporCombo()
        {
            CustomWizardPluginComponent.NewProject.RedundancyFullSynchronizationStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            CustomWizardPluginComponent.NewProject.RedundancyFullSynchronizationTimeSpan = new TimeSpan();
            CustomWizardPluginComponent.NewProject.Transport = Opc.Ua.Utils.UriSchemeNetTcp;
            CustomWizardPluginComponent.NewProject.Server = Properties.Settings.Default.DefaultServerName;
            textEditFullSynchronizationStartTime.DataContext = CustomWizardPluginComponent.NewProject;
            textEditFullSynchronizationTimeSpan.DataContext = CustomWizardPluginComponent.NewProject;

            List<Uri> list = new List<Uri>();
            var baseAddresses = UFUAServerInfo.UFUAServerInfo.GetCurrentApplicationBaseAddresses();
            if (baseAddresses.Count > 0)
            {
                foreach (var address in baseAddresses)
                {
                    var uri = Opc.Ua.Utils.ParseUri(address);
                    if (uri != null)
                    {
                        if (uri.Scheme == Opc.Ua.Utils.UriSchemeNetPipe)
                            bEnableLocal = true;
                        else
                        {
                            list.Add(uri);
                            if (!bEnableRemote)
                            {
                                bEnableRemote = true;
                                CustomWizardPluginComponent.NewProject.Port = uri.Port;
                            }
                        }
                    }
                }
            }

            transport.ItemsSource = list;
            transport.SelectedIndex = 0;
        }

        void InitListBox()
        {
            List<ButtonHelper> list = new List<ButtonHelper>();
            if (bEnableLocal)
                list.Add(new ButtonHelper() 
                { 
                    Name = "localBtn", 
                    Description = textInfo.ToTitleCase(Properties.Resources.LocalProject), 
                    Image = CustomWizardPluginComponent.GetControlImage("WPLocal"), 
                    Architecture = ArchType.local,
                    Action = () => { InitLocal(); } 
                });
            if (bEnableRemote)
            {
                list.Add(new ButtonHelper() 
                { 
                    Name = "distributedBtn", 
                    Description = textInfo.ToTitleCase(Properties.Resources.DistributedPeoject), 
                    Image = CustomWizardPluginComponent.GetControlImage("WPDistributed"),
                    Architecture = ArchType.distributed,
                    Action = () => { InitDistributed(); } 
                });
                list.Add(new ButtonHelper() 
                { 
                    Name = "redundancyBtn", 
                    Description = textInfo.ToTitleCase(Properties.Resources.RedundancedProject), 
                    Image = CustomWizardPluginComponent.GetControlImage("WPRedundant"),
                    Architecture = ArchType.redundancy,
                    Action = () => { InitRedundancy(); } 
                });
            }
            listBoxLink.ItemsSource = list;
            listBoxServer.ItemsSource = listServers;

            if (list.Count > 0)
                list[0].Action();
        }

        private void redundancyServersAdd_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtName.Text))
            {
                txtName.Focus();
                return;
            }
            if (listServers.Contains(txtName.Text))
            {
                listBoxServer.SelectedItem = txtName.Text;
                return;
            }

            listServers.Add(txtName.Text);
            txtName.Text = null;
            txtName.Focus();
        }

        private void redundancyServersRemove_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxServer.SelectedItem != null)
            {
                listServers.Remove(listBoxServer.SelectedItem as String);
            }
        }

        private void redundancyServersMoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxServer.SelectedItem != null)
            {
                var server = listBoxServer.SelectedItem as String;
                var index = listServers.IndexOf(server);
                if (index > 0)
                {
                    listServers.Move(index, index - 1);
                }
            }
        }

        private void redundancyServersMoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxServer.SelectedItem != null)
            {
                var server = listBoxServer.SelectedItem as String;
                var index = listServers.IndexOf(server);
                if (index < listServers.Count - 1)
                {
                    listServers.Move(index, index + 1);
                }
            }
        }

        private void redundancyServersClear_Click(object sender, RoutedEventArgs e)
        {
            if (CustomWizardPluginComponent.wizard.ProjectView?.UIInterface == null ||
                CustomWizardPluginComponent.wizard.ProjectView?.UIInterface.ShowYesNo(Properties.Resources.RedundancyClearServersConfirmation,
                CustomDialogIcons.Question) == CustomDialogResults.Yes)
            {
                listServers.Clear();
            }
        }

        void InitRedundancy()
        {
            serverList.Visibility = transportList.Visibility = System.Windows.Visibility.Visible;
            serverNameLabel.Visibility = txtServerName.Visibility = System.Windows.Visibility.Collapsed;
        }

        void InitDistributed()
        {
            transportList.Visibility = System.Windows.Visibility.Visible;
            serverList.Visibility = System.Windows.Visibility.Collapsed;
            serverNameLabel.Visibility = txtServerName.Visibility = System.Windows.Visibility.Visible;
        }

        void InitLocal()
        {
            serverList.Visibility = transportList.Visibility = System.Windows.Visibility.Collapsed;
            serverNameLabel.Visibility = txtServerName.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void listBoxLink_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ButtonHelper selectedItem = (sender as ListBox)?.SelectedItem as ButtonHelper;
            if (selectedItem == null)
                return;
            selectedItem.Action();
        }

        public bool Execute()
        {
            int port;
            if(int.TryParse(txtPortNumber.Text,out port))
                CustomWizardPluginComponent.NewProject.Port = port;
            if (!string.IsNullOrWhiteSpace(txtServerName.Text))
                CustomWizardPluginComponent.NewProject.Server = txtServerName.Text;
            var uri = transport.SelectedValue as Uri;
            if (uri != null)
                CustomWizardPluginComponent.NewProject.Transport = uri.Scheme;
            CustomWizardPluginComponent.NewProject.ServerList = listServers.ToList();
            var helper = listBoxLink.SelectedItem as ButtonHelper;
            if (helper != null)
                CustomWizardPluginComponent.NewProject.Architecture = helper.Architecture;
            return false;
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utilities;
using Utilities.WPF;
using System.Reflection;
using DriverCodeBaseEx;

namespace SNMP.UI
{
    /// <summary>
    /// Interaction logic for GeneralSettingsEditor.xaml
    /// </summary>
    public partial class GeneralSettingsEditor : UserControl, IDisposable
    {
        DriverSettingsInterfaces.ComunicationSettingsContext2 comSetContext;
        ConfigurationEditor configurationEditor = new ConfigurationEditor();
        DriverCodeBaseEx.UI.Controls.BaseChannelTree cht = null;
        bool alreadyLoaded = false;
        String DriverName;

        public GeneralSettingsEditor()
        {
            InitializeComponent();

            DataContextChanged += (o, e) =>
            {
                ReadSettings();
            };

            Loaded += (o, e) =>
            {
                if (alreadyLoaded)
                    return;

                alreadyLoaded = true;
                SetCurrentData();
            };
        }

        private void ReadSettings()
        {
            if (!(DataContext is DriverSettingsInterfaces.ComunicationSettingsContext))
                return;

            comSetContext = DataContext as DriverSettingsInterfaces.ComunicationSettingsContext2;
            Assembly a = Assembly.GetAssembly(this.GetType());
            DriverName = a.GetName().Name;
            DriverName = DriverName.Replace(".UI", "");

            if (!configurationEditor.IsValid)
                configurationEditor.Init<SNMPDriverSettings>(comSetContext.ConnectionString, DriverName, comSetContext.bProtected, comSetContext.protectionCode);

            if (!configurationEditor.IsValid)
            {
                if (!configurationEditor.IsBelongFromParent())
                {
                    configurationEditor.BackupAndDeleteDriverSettings(comSetContext.ConnectionString, DriverName, comSetContext.bProtected, comSetContext.protectionCode, out string backupFile);
                    MessageBox.Show(string.Format(DriverCodeBaseEx.Properties.Resources.ErrorDynsettingsFileInvalidBackup, backupFile), DriverName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show(string.Format(DriverCodeBaseEx.Properties.Resources.ErrorOpeningDocument, comSetContext.ConnectionString, DriverName));
                return;
            }
        }


        private void SetCurrentData()
        {
            ReadSettings();

            DataContext = null;
            DataContext = configurationEditor.Configuration as SNMPDriverSettings;
            if (configurationEditor.Configuration is SNMPDriverSettings && (configurationEditor.Configuration as SNMPDriverSettings).ChannelSettings != null)
            {
                cht = new DriverCodeBaseEx.UI.Controls.BaseChannelTree() { DataContext = configurationEditor.Configuration as SNMPDriverSettings };
                cht.AddStationEv += StationAdd_Click;
                cht.AddChannelEv += ChannelAdd_Click;
                cht.EditEv += ChannelEdit_Click;
                cht.TreeDblClickEv += ChannelEdit_DblClick;
                cht.SetConfigurationEditor(configurationEditor);
                ChannelSettings.Content = cht;
            }

            DriverCodeBaseEx.UI.Controls.BaseGeneralSettings bgs = new DriverCodeBaseEx.UI.Controls.BaseGeneralSettings() { DataContext = DataContext };
            bgs.AggregationLimit.Visibility = System.Windows.Visibility.Collapsed;
            bgs.AggregationLimitText.Visibility = System.Windows.Visibility.Collapsed;
            bgs.AggregationLimitErr.Visibility = System.Windows.Visibility.Collapsed;
            bgs.AggregationThreshold.Visibility = System.Windows.Visibility.Collapsed;
            bgs.AggregationThresholdText.Visibility = System.Windows.Visibility.Collapsed;
            bgs.AggregationThresholdErr.Visibility = System.Windows.Visibility.Collapsed;
            
            MainStack.Children.Add(bgs);
        }
        
        private void ChannelAdd_Click(object sender, RoutedEventArgs e)
        {
            ChannelDetails d = new ChannelDetails();
            var ch = configurationEditor.NewChannel();

            d.DataContext = ch;
 
            GeneralDialogContent newChDetDialog = new GeneralDialogContent(d)
            {
                Owner = this.FindParent<Window>(),
                Title = DriverCodeBaseEx.UI.Properties.Resources.ChannelsSettings
            };
            if (newChDetDialog.ShowDialog() != true)
            {
                //Cancel, do not add
                configurationEditor.RemoveChannelSettings(ch.Name, true);
                return;
            }

            cht.AddChannel(ch);
        }
        private void Edit()
        {
            if (cht.GetSelecteItem() as SNMPChannelSettings != null)
            {
                var ch = cht.GetSelecteItem() as SNMPChannelSettings;

                using (var uow = configurationEditor.UfW.BeginNestedUnitOfWork())
                {
                    ChannelDetails d = new ChannelDetails();
                    d.DataContext = uow.GetNestedObject(ch);
                    GeneralDialogContent newChDetDialog = new GeneralDialogContent(d)
                    {
                        Owner = this.FindParent<Window>(),
                        Title = DriverCodeBaseEx.UI.Properties.Resources.ChannelsSettings
                    };
                    if (newChDetDialog.ShowDialog() == true)
                        uow.CommitChanges();
                }

            }
            else if (cht.GetSelecteItem() as SNMPStationSettings != null)
            {
                var st = cht.GetSelecteItem() as SNMPStationSettings;
                using (var uow = configurationEditor.UfW.BeginNestedUnitOfWork())
                {                    
                    string oldname = st.Channel;
                    StationDetails d = new StationDetails();
                    d.DataContext = uow.GetNestedObject(st);
                    GeneralDialogContent newChDetDialog = new GeneralDialogContent(d)
                    {
                        Owner = this.FindParent<Window>(),
                        Title = DriverCodeBaseEx.UI.Properties.Resources.StationsSettings
                    };
                    if (newChDetDialog.ShowDialog() == true)
                        uow.CommitChanges();
                    if (st.Channel != oldname)
                    {
                        var temp = ChannelSettings.Content as DriverCodeBaseEx.UI.Controls.BaseChannelTree;
                        if (temp != null)
                            temp.InitChannelTree();
                    }
                }
            }
        }
        private void ChannelEdit_Click(object sender, RoutedEventArgs e)
        {
            Edit();
        }
        private void ChannelEdit_DblClick(object sender, MouseButtonEventArgs e)
        {
            Edit();
        }
        

        private void StationAdd_Click(object sender, RoutedEventArgs e)
        {
            var channel = cht.GetSelectedChannel() as SNMPChannelSettings;
            if (channel == null)
                return;

            StationDetails d = new StationDetails();
            var st = configurationEditor.AddStation(channel.Name) as StationSettings;
            ((SNMPStationSettings)st).snmpVersion = ((SNMPChannelSettings)channel).snmpVersion;

            d.DataContext = st;

            GeneralDialogContent newChDetDialog = new GeneralDialogContent(d)
            {
                Owner = this.FindParent<Window>(),
                Title = DriverCodeBaseEx.UI.Properties.Resources.StationsSettings
            };
            if (newChDetDialog.ShowDialog() != true)
            {
                //Cancel, do not add
                configurationEditor.RemoveStationSettings(channel.Name, st.Name);
                return;
            }

            cht.AddStation(st);
        }
        #region ServiceMethods
        public void SaveDriverSettings()
        {
            string message;
            if (!configurationEditor.Save(out message, comSetContext.bProtected, comSetContext.protectionCode))
            {
                Assembly a = Assembly.GetAssembly(this.GetType());
                System.Windows.MessageBox.Show(message, a.GetName().Name, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
        #endregion

        #region IDisposable Members
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            if (cht != null)
            {
                cht.Dispose();
                cht = null;
            }
            if (configurationEditor != null)
            {
                configurationEditor.Dispose();
                configurationEditor = null;
            }
        }

        #endregion
    }
}

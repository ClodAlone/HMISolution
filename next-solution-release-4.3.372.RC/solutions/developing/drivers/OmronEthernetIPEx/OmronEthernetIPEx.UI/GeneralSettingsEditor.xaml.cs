using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utilities;
using Utilities.WPF;
using DevExpress.Xpo;
using System.Reflection;
using System.Xml;
using System.IO;
using DevExpress.Xpo.DB;
using DriverCodeBaseEx;
using DriverCodeBaseEx.UI.Controls;

namespace OmronEthernetIP.UI
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
        string DriverName;

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
                configurationEditor.Init<OmronEthernetIPDriverSettings>(comSetContext.ConnectionString, DriverName, comSetContext.bProtected, comSetContext.protectionCode);

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
            DataContext = configurationEditor.Configuration as OmronEthernetIPDriverSettings;
            if (configurationEditor.Configuration is OmronEthernetIPDriverSettings && (configurationEditor.Configuration as OmronEthernetIPDriverSettings).ChannelSettings != null)
            {
                cht = new DriverCodeBaseEx.UI.Controls.BaseChannelTree() { DataContext = configurationEditor.Configuration as OmronEthernetIPDriverSettings };
                cht.AddStationEv += StationAdd_Click;
                cht.AddChannelEv += ChannelAdd_Click;
                cht.EditEv += ChannelEdit_Click;
                cht.TreeDblClickEv += ChannelEdit_DblClick;
                cht.TestCommSupported = true;
                cht.TestCommEv += TestComm_Click;
                cht.SetConfigurationEditor(configurationEditor);
                ChannelSettings.Content = cht;
            }

            // symbolic driver don't manage aggregation's
            var genset = new DriverCodeBaseEx.UI.Controls.BaseGeneralSettings();
            genset.AggregationThresholdText.Visibility = System.Windows.Visibility.Collapsed;
            genset.AggregationThreshold.Visibility = System.Windows.Visibility.Collapsed;
            genset.AggregationLimitText.Visibility = System.Windows.Visibility.Collapsed;
            genset.AggregationLimit.Visibility = System.Windows.Visibility.Collapsed;
            genset.DataContext = DataContext;
            MainStack.Children.Add(genset);
        }

        private void ChannelAdd_Click(object sender, RoutedEventArgs e)
        {
            ChannelDetails d = new ChannelDetails();
            var ch = configurationEditor.AddChannel() as ChannelSettings;
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
            if (cht.GetSelecteItem() as OmronEthernetIPChannelSettings != null)
            {
                var ch = cht.GetSelecteItem() as OmronEthernetIPChannelSettings;

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
            else if (cht.GetSelecteItem() as OmronEthernetIPStationSettings != null)
            {
                var st = cht.GetSelecteItem() as OmronEthernetIPStationSettings;
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
            var channel = cht.GetSelectedChannel() as OmronEthernetIPChannelSettings;
            if (channel == null)
            {

                return;
            }
            StationDetails d = new StationDetails();
            var st = configurationEditor.AddStation(channel.Name) as StationSettings;

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
        /// <summary>
        /// Event Botton test connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestComm_Click(object sender, RoutedEventArgs e)
        {
            if (!(cht.GetSelecteItem() is OmronEthernetIPStationSettings))
                return;

            var stationSettings = cht.GetSelecteItem() as OmronEthernetIPStationSettings;
            var channelSettings = cht.GetSelectedChannel() as OmronEthernetIPChannelSettings;

            using (OmronEthernetIPDriver driver = new OmronEthernetIPDriver(comSetContext.ConnectionString))
            {
                bool result = driver.TestComm(channelSettings, stationSettings, out string error);
                
                BaseTestConnectionInfo useControlInfo = new BaseTestConnectionInfo();
                useControlInfo.DataContext = new TestCommInfo(result, error);
                GeneralDialogContent newChDetDialog = new GeneralDialogContent(useControlInfo)
                {
                    Owner = this.FindParent<Window>(),
                    Title = DriverCodeBaseEx.UI.Properties.Resources.CaptionDeviceInformation
                };
                newChDetDialog.ShowDialog();
            }
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

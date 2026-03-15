using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utilities;
using Utilities.WPF;
using DevExpress.Xpo;
using System.Reflection;
using DevExpress.Xpo.DB;
using System.Xml;
using System.IO;
using DriverCodeBaseEx;
using DataWriter;

namespace SQLDriver.UI
{
    /// <summary>
    /// Interaction logic for GeneralSettingsEditor.xaml
    /// </summary>
    public partial class GeneralSettingsEditor : UserControl, IDisposable
    {
        #region Declarations
        DriverSettingsInterfaces.ComunicationSettingsContext2 comSetContext;
        ConfigurationEditor configurationEditor = new ConfigurationEditor();
        DriverCodeBaseEx.UI.Controls.BaseChannelTree cht = null;
        bool alreadyLoaded = false;       
        string DriverName;        
        #endregion

        #region CTors
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
        #endregion

        #region Driver Specific
        private void ReadSettings()
        {
            if (!(DataContext is DriverSettingsInterfaces.ComunicationSettingsContext))
                return;
            
            comSetContext = DataContext as DriverSettingsInterfaces.ComunicationSettingsContext2;

            Assembly a = Assembly.GetAssembly(this.GetType());
            DriverName = a.GetName().Name;
            DriverName = DriverName.Replace(".UI", "");

            if (!configurationEditor.IsValid)
                configurationEditor.Init<SQLDriverDriverSettings>(comSetContext.ConnectionString, DriverName, comSetContext.bProtected, comSetContext.protectionCode);

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
            DataContext = configurationEditor.Configuration as SQLDriverDriverSettings;
            if (configurationEditor.Configuration is SQLDriverDriverSettings && (configurationEditor.Configuration as SQLDriverDriverSettings).ChannelSettings != null)
            {
                cht = new DriverCodeBaseEx.UI.Controls.BaseChannelTree() { DataContext = configurationEditor.Configuration as SQLDriverDriverSettings };
                cht.AddStationEv += StationAdd_Click;
                cht.AddChannelEv += ChannelAdd_Click;
                cht.EditEv += ChannelEdit_Click;
                cht.TreeDblClickEv += ChannelEdit_DblClick;                
                cht.TestCommSupported = true;
                cht.SetConfigurationEditor(configurationEditor);
                cht.TestCommEv += TestComm_Click;
                ChannelSettings.Content = cht;
            }
            DriverCodeBaseEx.UI.Controls.BaseGeneralSettings baseGeneralSetting = new DriverCodeBaseEx.UI.Controls.BaseGeneralSettings() { DataContext = DataContext };
            baseGeneralSetting.AggregationLimitText.Visibility = System.Windows.Visibility.Collapsed;
            baseGeneralSetting.AggregationLimit.Visibility = System.Windows.Visibility.Collapsed;
            baseGeneralSetting.AggregationThresholdText.Visibility = System.Windows.Visibility.Collapsed;
            baseGeneralSetting.AggregationThreshold.Visibility = System.Windows.Visibility.Collapsed;
            //baseGeneralSetting.textBlockSynchAtStartup.Visibility = System.Windows.Visibility.Collapsed;
            //baseGeneralSetting.SynchAtStartup.Visibility = System.Windows.Visibility.Collapsed;
            MainStack.Children.Add(baseGeneralSetting);
            //gridGeneral.Children.Add(new DriverCodeBase.Controls.BaseGeneralSettings() { DataContext = DataContext });
        }
                
        private void ChannelAdd_Click(object sender, RoutedEventArgs e)
        {
            ChannelDetails d = new ChannelDetails();
            var ch = configurationEditor.AddChannel() as ChannelSettings;

            d.DataContext = ch;
            d.ConnectionString = comSetContext.ConnectionString;

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
            if (cht.GetSelecteItem() as SQLDriverChannelSettings != null)
            {
                var ch = cht.GetSelecteItem() as SQLDriverChannelSettings;

                using (var uow = configurationEditor.UfW.BeginNestedUnitOfWork())
                {
                    ChannelDetails d = new ChannelDetails();
                    d.DataContext = uow.GetNestedObject(ch);
                    d.ConnectionString = comSetContext.ConnectionString;
                    GeneralDialogContent newChDetDialog = new GeneralDialogContent(d)
                    {
                        Owner = this.FindParent<Window>(),
                        Title = DriverCodeBaseEx.UI.Properties.Resources.ChannelsSettings
                    };
                    if (newChDetDialog.ShowDialog() == true)
                        uow.CommitChanges();
                }
                
            }
            else if (cht.GetSelecteItem() as SQLDriverStationSettings != null)
            {
                var st = cht.GetSelecteItem() as SQLDriverStationSettings;

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
            var channel = cht.GetSelectedChannel() as SQLDriverChannelSettings;
            if (channel == null)
            {

                return;
            }

            StationDetails d = new StationDetails();
            var st = configurationEditor.AddStation(channel.Name) as StationSettings; ;

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

        private void TestComm_Click(object sender, RoutedEventArgs e)
        {
            if (!(cht.GetSelecteItem() is SQLDriverStationSettings))
                return;

            var stationSettings = cht.GetSelecteItem() as SQLDriverStationSettings;
            var channelSettings = cht.GetSelectedChannel() as SQLDriverChannelSettings;

            if(string.IsNullOrWhiteSpace(channelSettings.ConnectionString))
            {
                MessageBox.Show(Properties.Resources.SQLConnectionStringNotAvailabe, DriverName, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                DataSetWriter writer = new DataWriter.DataSetWriter(channelSettings.Provider, channelSettings.ConnectionString);
                try
                {
                    writer.OpenConnection();
                    MessageBox.Show(DriverCodeBaseEx.Properties.Resources.TestCommSucceded, DriverName, MessageBoxButton.OK, MessageBoxImage.Information);
                    writer.CloseConnection();
                }
                catch (Exception ex)
                {
                    StringBuilder errorMessages = new StringBuilder();
                    errorMessages.Append("Connection string: " + channelSettings.ConnectionString + "\n");
                    errorMessages.Append("Message: " + ex.Message);
                    MessageBox.Show(string.Format(DriverCodeBaseEx.Properties.Resources.ErrorTestCommFailed, errorMessages,ToString()), DriverName, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

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

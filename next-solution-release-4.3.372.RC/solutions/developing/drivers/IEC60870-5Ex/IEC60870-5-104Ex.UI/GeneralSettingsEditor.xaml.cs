////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	GeneralSettingsEditor.xaml.cs
//
// summary:	Implements the general settings editor.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
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

namespace IEC60870_5_104.UI
{
    /// <summary>   Interaction logic for GeneralSettingsEditor.xaml. </summary>
    public partial class GeneralSettingsEditor : UserControl, IDisposable
    {
        #region Declarations
        DriverSettingsInterfaces.ComunicationSettingsContext2 comSetContext;
        ConfigurationEditor configurationEditor = new ConfigurationEditor();
        /// <summary>   The cht. </summary>
        DriverCodeBaseEx.UI.Controls.BaseChannelTree cht = null;
        /// <summary>   true if already loaded. </summary>
        bool alreadyLoaded = false;       
        string DriverName;
        #endregion

        #region CTors
        /// <summary>   Default constructor. </summary>
        public GeneralSettingsEditor()
        {
            InitializeComponent();

            DataContextChanged += (o, e) =>
            {
                ReadSettings();
            };

            //executed at loaded
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
                configurationEditor.Init<IEC60870_5_104DriverSettings>(comSetContext.ConnectionString, DriverName, comSetContext.bProtected, comSetContext.protectionCode);

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

        /// <summary>   Sets current data. </summary>
        private void SetCurrentData()
        {
            ReadSettings();

            DataContext = null;
            DataContext = configurationEditor.Configuration as IEC60870_5_104DriverSettings;
            if (configurationEditor.Configuration is IEC60870_5_104DriverSettings && (configurationEditor.Configuration as IEC60870_5_104DriverSettings).ChannelSettings != null)
            {
                cht = new DriverCodeBaseEx.UI.Controls.BaseChannelTree() { DataContext = configurationEditor.Configuration as IEC60870_5_104DriverSettings };
                cht.AddStationEv += StationAdd_Click;
                cht.AddChannelEv += ChannelAdd_Click;
                cht.EditEv += ChannelEdit_Click;
                cht.TreeDblClickEv += ChannelEdit_DblClick;
                cht.TestCommSupported = true;
                cht.TestCommEv += TestComm_Click;
                cht.SetConfigurationEditor(configurationEditor);
                cht.SingleStationForChannel = true;
                ChannelSettings.Content = cht;
            }

            DriverCodeBaseEx.UI.Controls.BaseGeneralSettings baseGeneralSetting = new DriverCodeBaseEx.UI.Controls.BaseGeneralSettings() { DataContext = DataContext };
            baseGeneralSetting.AggregationThresholdText.Visibility = System.Windows.Visibility.Collapsed;
            baseGeneralSetting.AggregationThreshold.Visibility = System.Windows.Visibility.Collapsed;
            baseGeneralSetting.AggregationLimitText.Visibility = System.Windows.Visibility.Collapsed;
            baseGeneralSetting.AggregationLimit.Visibility = System.Windows.Visibility.Collapsed;
            baseGeneralSetting.textBlockSynchAtStartup.Visibility = System.Windows.Visibility.Collapsed;
            baseGeneralSetting.SynchAtStartup.Visibility = System.Windows.Visibility.Collapsed;
            MainStack.Children.Add(baseGeneralSetting);
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by ChannelAdd for click events. </summary>
        ///
        /// <param name="sender" type="object">     Source of the event. </param>
        /// <param name="e" type="RoutedEventArgs"> Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ChannelAdd_Click(object sender, RoutedEventArgs e)
        {
            ChannelDetails d = new ChannelDetails();
            var ch = configurationEditor.AddChannel() as ChannelSettings;
            IEC60870_5_104DriverSettings mds = DataContext as IEC60870_5_104DriverSettings;
            ch.DriverSettings = mds;
            ch.DefaultSettings();
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
        /// <summary>   Edits this object. </summary>
        private void Edit()
        {
            if (cht.GetSelecteItem() as IEC60870_5_104ChannelSettings != null)
            {
                var ch = cht.GetSelecteItem() as IEC60870_5_104ChannelSettings;

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
            else if (cht.GetSelecteItem() as IEC60870_5_104StationSettings != null)
            {
                var st = cht.GetSelecteItem() as IEC60870_5_104StationSettings;
                using (var uow = configurationEditor.UfW.BeginNestedUnitOfWork())
                {
                    string oldname = st.Channel;
                    StationDetails d = new StationDetails();
                    d.DataContext = uow.GetNestedObject(st);
                    GeneralDialogContent newChDetDialog = new GeneralDialogContent(d)
                    {
                        Title = DriverCodeBaseEx.UI.Properties.Resources.StationsSettings,
                        Owner = this.FindParent<Window>()
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
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by ChannelEdit for click events. </summary>
        ///
        /// <param name="sender" type="object">     Source of the event. </param>
        /// <param name="e" type="RoutedEventArgs"> Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ChannelEdit_Click(object sender, RoutedEventArgs e)
        {
            Edit();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by ChannelEdit for double click events. </summary>
        ///
        /// <param name="sender" type="object">             Source of the event. </param>
        /// <param name="e" type="MouseButtonEventArgs">    Mouse button event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ChannelEdit_DblClick(object sender, MouseButtonEventArgs e)
        {
            Edit();
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by StationAdd for click events. </summary>
        ///
        /// <param name="sender" type="object">     Source of the event. </param>
        /// <param name="e" type="RoutedEventArgs"> Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void StationAdd_Click(object sender, RoutedEventArgs e)
        {
            var channel = cht.GetSelectedChannel() as IEC60870_5_104ChannelSettings;
            if (channel == null)
            {

                return;
            }
            var listStations = (from c in channel.DriverSettings.StationSettings where c.Channel == channel.Name select c).ToList();
            if (listStations.Count > 0)
                return;

            StationDetails d = new StationDetails();
            var st = configurationEditor.AddStation(channel.Name) as StationSettings;

            IEC60870_5_104DriverSettings mds = DataContext as IEC60870_5_104DriverSettings;
            st.DriverSettings = mds;
            st.DefaultSettings();
            st.Channel = channel.Name;
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
            if (!(cht.GetSelecteItem() is IEC60870_5_104StationSettings))
                return;

            var stationSettings = cht.GetSelecteItem() as IEC60870_5_104StationSettings;
            var channelSettings = cht.GetSelectedChannel() as IEC60870_5_104ChannelSettings;

            using (IEC60870_5_104Driver driver = new IEC60870_5_104Driver(comSetContext.ConnectionString))
            {
                if (driver.TestComm(channelSettings, stationSettings, out string error))
                    MessageBox.Show(DriverCodeBaseEx.Properties.Resources.TestCommSucceded, DriverName, MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show(string.Format(DriverCodeBaseEx.Properties.Resources.ErrorTestCommFailed, error), DriverName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region IDisposable Members
        /// <summary>   true if disposed. </summary>
        bool bDisposed;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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

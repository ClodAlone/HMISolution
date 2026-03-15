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
using Simotion;

namespace Simotion.UI
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
                configurationEditor.Init<SimotionDriverSettings>(comSetContext.ConnectionString, DriverName, comSetContext.bProtected, comSetContext.protectionCode);

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
            DataContext = configurationEditor.Configuration as SimotionDriverSettings;
            if (configurationEditor.Configuration is SimotionDriverSettings && (configurationEditor.Configuration as SimotionDriverSettings).ChannelSettings != null)
            {
                cht = new DriverCodeBaseEx.UI.Controls.BaseChannelTree() { DataContext = configurationEditor.Configuration as SimotionDriverSettings };
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

            var genset = new DriverCodeBaseEx.UI.Controls.BaseGeneralSettings();
            genset.AggregationLimitText.Visibility = Visibility.Collapsed;
            genset.AggregationLimit.Visibility = Visibility.Collapsed;
            genset.AggregationLimitErr.Visibility = Visibility.Collapsed;
            genset.AggregationThresholdText.Visibility = Visibility.Collapsed;
            genset.AggregationThreshold.Visibility = Visibility.Collapsed;
            genset.AggregationThresholdErr.Visibility = Visibility.Collapsed;
            genset.DataContext = DataContext;
            //gridGeneral.Children.Add(genset);
            MainStack.Children.Add(genset);
            //gridGeneral.Children.Add(new DriverCodeBaseEx.Controls.BaseGeneralSettings() { DataContext = DataContext });
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
            if (cht.GetSelecteItem() as SimotionChannelSettings != null)
            {
                var ch = cht.GetSelecteItem() as SimotionChannelSettings;

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
            else if (cht.GetSelecteItem() as SimotionStationSettings != null)
            {
                var st = cht.GetSelecteItem() as SimotionStationSettings;
                using (var uow = configurationEditor.UfW.BeginNestedUnitOfWork())
                {
                    var c = cht.GetSelecteParentItem() as SimotionChannelSettings;

                    StationDetails d = new StationDetails(configurationEditor.ConnectionString, configurationEditor.UfW);
                    d.DataContext = uow.GetNestedObject(st);
                    GeneralDialogContent newChDetDialog = new GeneralDialogContent(d)
                    {
                        Owner = this.FindParent<Window>(),
                        Title = DriverCodeBaseEx.UI.Properties.Resources.StationsSettings
                    };
                    if (newChDetDialog.ShowDialog() == true)
                        uow.CommitChanges();
                    if (st.Channel != c.Name)
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


        private void StationAdd_Click(object sender, RoutedEventArgs e)
        {
            var channel = cht.GetSelectedChannel() as SimotionChannelSettings;
            if (channel == null)
            {

                return;
            }
            var listStations = (from c in channel.DriverSettings.StationSettings where c.Channel == channel.Name select c).ToList();
            if (listStations.Count > 0)
                return;

            StationDetails d = new StationDetails(configurationEditor.ConnectionString, configurationEditor.UfW);
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

        private void TestComm_Click(object sender, RoutedEventArgs e)
        {
            if (!(cht.GetSelecteItem() is SimotionStationSettings))
                return;

            var stationSettings = cht.GetSelecteItem() as SimotionStationSettings;
            var channelSettings = cht.GetSelectedChannel() as SimotionChannelSettings;

            using (SimotionDriver driver = new SimotionDriver(comSetContext.ConnectionString))
            {
                if (driver.TestComm(channelSettings, stationSettings, out string error))
                    MessageBox.Show(DriverCodeBaseEx.Properties.Resources.TestCommSucceded, DriverName, MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show(string.Format(DriverCodeBaseEx.Properties.Resources.ErrorTestCommFailed, error), DriverName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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

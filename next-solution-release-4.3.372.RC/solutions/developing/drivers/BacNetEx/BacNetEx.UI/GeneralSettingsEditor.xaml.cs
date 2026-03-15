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
using BACnet;
using DriverCodeBaseEx.UI.Controls;

namespace BACnet.UI
{
    /// <summary>   Interaction logic for GeneralSettingsEditor.xaml. </summary>
    public partial class GeneralSettingsEditor : UserControl, IDisposable
    {
        #region Declarations
        DriverSettingsInterfaces.ComunicationSettingsContext2 comSetContext;
        /// <summary>   The idl. </summary>
        //IDataLayer idl = null;
        ///// <summary>   The ufw. </summary>
        //UnitOfWork ufw = null;
        ///// <summary>   The configuration. </summary>
        //BACnetDriverSettings configuration = null;
        ///// <summary>   The file base. </summary>
        //string fileBase;
        ///// <summary>   The in memory. </summary>
        //InMemoryDataStore InMemory = null;
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

            //idl = DriverCodeBaseEx.CommunicationDriver.GetDriverDataLayer(comSetContext.ConnectionString, DriverName, out fileBase, out InMemory);//, false);
            //if(idl == null)
            //{
            //    MessageBox.Show(string.Format(DriverCodeBaseEx.Properties.Resources.ErrorOpeningDocument, comSetContext.ConnectionString, DriverName));
            //    return;
            //}

            //ufw = new UnitOfWork(idl);
            //DriverCodeBaseEx.CommunicationDriver.UpdateDriverSchema(ufw);
            //if (!IsBelongFromParent())
            //{
            //    throw new DriverBaseInterfaces.ValidatingDocumentException(fileBase);
            //}

            if (!configurationEditor.IsValid)
                configurationEditor.Init<BACnetDriverSettings>(comSetContext.ConnectionString, DriverName, comSetContext.bProtected, comSetContext.protectionCode);

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

        //bool IsBelongFromParent()
        //{
        //    if (String.IsNullOrEmpty(fileBase) || !File.Exists(fileBase))
        //        return true;

        //    if (comSetContext.bProtected && Utilities.IO.FileSystem.IsXmlFile(fileBase))
        //        return false;

        //    var id = XpoHelpers.XpoHelper.GetProtectionCode(ufw);
        //    if ((id == Guid.Empty) || (id == comSetContext.protectionCode))
        //    {
        //        return (true);
        //    }
        //    else if ((comSetContext.protectionCode == Guid.Empty) && !comSetContext.bProtected)
        //    {
        //        comSetContext.bProtected = true;
        //        comSetContext.protectionCode = id;
        //        return (true);
        //    }
        //    else
        //    {
        //        return (false);
        //    }
        //}

        /// <summary>   Sets current data. </summary>
        private void SetCurrentData()
        {
            ReadSettings();

            DataContext = null;
            DataContext = configurationEditor.Configuration as BACnetDriverSettings;
            if (configurationEditor.Configuration is BACnetDriverSettings && (configurationEditor.Configuration as BACnetDriverSettings).ChannelSettings != null)
            {
                cht = new DriverCodeBaseEx.UI.Controls.BaseChannelTree() { DataContext = configurationEditor.Configuration as BACnetDriverSettings };
                cht.AddStationEv += StationAdd_Click;
                cht.AddChannelEv += ChannelAdd_Click;
                cht.EditEv += ChannelEdit_Click;
                cht.TreeDblClickEv += ChannelEdit_DblClick;
                cht.TestCommSupported = true;
                cht.TestCommEv += TestComm_Click;                
                cht.SetConfigurationEditor(configurationEditor);
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
            MainStack.Children.Insert(0, genset);
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
            //var ch = new BACnetChannelSettings(ufw);
            //BACnetDriverSettings mds = DataContext as BACnetDriverSettings;
            //ch.DriverSettings = mds;
            //ch.DefaultSettings();
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
                //mds.ChannelSettings.Remove(ch);
                //ch.Delete();
                configurationEditor.RemoveChannelSettings(ch.Name, true);
                return;
            }
            cht.AddChannel(ch);
        }
        /// <summary>   Edits this object. </summary>
        private void Edit()
        {
            if (cht.GetSelecteItem() as BACnetChannelSettings != null)
            {
                var ch = cht.GetSelecteItem() as BACnetChannelSettings;

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
            else if (cht.GetSelecteItem() as BACnetStationSettings != null)
            {
                var st = cht.GetSelecteItem() as BACnetStationSettings;
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
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by StationAdd for click events. </summary>
        ///
        /// <param name="sender" type="object">     Source of the event. </param>
        /// <param name="e" type="RoutedEventArgs"> Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void StationAdd_Click(object sender, RoutedEventArgs e)
        {
            var channel = cht.GetSelectedChannel() as BACnetChannelSettings;
            if (channel == null)
            {

                return;
            }

            StationDetails d = new StationDetails();
            //var st = new BACnetStationSettings(ufw);

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
            if (!(cht.GetSelecteItem() is BACnetStationSettings))
                return;

            var stationSettings = cht.GetSelecteItem() as BACnetStationSettings;
            var channelSettings = cht.GetSelectedChannel() as BACnetChannelSettings;
                        
            using (BACnetDriver driver = new BACnetDriver(comSetContext.ConnectionString))
            {
                driver.Init(comSetContext.ConnectionString);
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

        #endregion

        #region ServiceMethods
        /// <summary>   Saves the driver settings. </summary>
        //public void SaveDriverSettings()
        //{
        //    if (comSetContext == null)
        //        return;

        //    if (comSetContext.bProtected)
        //        XpoHelpers.XpoHelper.AddProtectionCode(ufw, comSetContext.protectionCode);
        //    else
        //        XpoHelpers.XpoHelper.RemoveProtectionCode(ufw);

        //    try
        //    {
        //        ufw.CommitChanges();
        //        ufw.ReloadChangedObjects();
        //        ufw.PurgeDeletedObjects();

        //        ufw.CommitChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (Environment.UserInteractive)
        //            MessageBox.Show(ex.Message);
        //        if (File.Exists(fileBase))
        //        {
        //            try
        //            {
        //                File.Delete(fileBase);
        //            }
        //            catch (Exception ex1)
        //            {
        //                if (Environment.UserInteractive)
        //                    MessageBox.Show(ex1.Message);
        //            }
        //        }
        //        return;
        //    }

        //    if (!String.IsNullOrEmpty(fileBase))
        //    {
        //        if (File.Exists(fileBase))
        //        {
        //            try
        //            {
        //                File.Delete(fileBase);
        //            }
        //            catch (Exception ex)
        //            {
        //                if (Environment.UserInteractive)
        //                    MessageBox.Show(ex.Message);
        //                return;
        //            }
        //        }
        //        try
        //        {
        //            if (comSetContext.bProtected)
        //            {
        //                using (var memoryStream = new MemoryStream())
        //                {
        //                    var writer = XmlWriter.Create(memoryStream);
        //                    InMemory.WriteXml(writer);
        //                    writer.Flush();//14577
        //                    writer.Close();
        //                    var str = Convert.ToBase64String(memoryStream.ToArray());
        //                    var toWrite = DriverCodeBaseEx.CommunicationDriver.EncryptString(str);
        //                    File.WriteAllText(fileBase, toWrite);
        //                }
        //            }
        //            else
        //                InMemory.WriteXml(fileBase);
        //        }
        //        catch (Exception ex)
        //        {
        //            Assembly a = Assembly.GetAssembly(this.GetType());
        //            System.Windows.MessageBox.Show(ex.Message, a.GetName().Name, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        //        }
        //    }
        //}
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

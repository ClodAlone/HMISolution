////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	GeneralSettingsEditor.xaml.cs
//
// summary:	Implements the general settings editor.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

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
using Utilities;
using Utilities.WPF;
using DevExpress.Xpo;
using System.Reflection;
using IEC60870_5_104;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using Ookii.Dialogs.Wpf;
using System.Xml;
using System.IO;

namespace IEC60870_5_104.UI
{
    /// <summary>   Interaction logic for GeneralSettingsEditor.xaml. </summary>
    public partial class GeneralSettingsEditor : UserControl, IDisposable
    {
        #region Declarations
        DriverSettingsInterfaces.ComunicationSettingsContext2 comSetContext;
        /// <summary>   The idl. </summary>
        IDataLayer idl = null;
        /// <summary>   The ufw. </summary>
        UnitOfWork ufw = null;
        /// <summary>   The configuration. </summary>
        IEC60870_5_104DriverSettings configuration = null;
        /// <summary>   The file base. </summary>
        string fileBase;
        /// <summary>   The in memory. </summary>
        InMemoryDataStore InMemory = null;
        /// <summary>   The cht. </summary>
        DriverCodeBase.UI.Controls.BaseChannelTree cht = null;
        /// <summary>   true if already loaded. </summary>
        bool alreadyLoaded = false;
        IDataLayer dlClipboard;
        UnitOfWork uowClipboard;
        InMemoryDataStore InMemoryClipboard;
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
            string DriverName = a.GetName().Name;
            DriverName = DriverName.Replace(".UI", "");
            idl = DriverCodeBase.CommunicationDriver.GetDriverDataLayer(comSetContext.ConnectionString, DriverName, out fileBase, out InMemory); //, false);
            if (idl == null)
            {
                MessageBox.Show(string.Format(DriverCodeBase.Properties.Resources.ErrorOpeningDocument, comSetContext.ConnectionString, DriverName));
                return;
            }

            ufw = new UnitOfWork(idl);
            DriverCodeBase.CommunicationDriver.UpdateDriverSchema(ufw);
            if (!IsBelongFromParent())
            {
                throw new DriverBaseInterfaces.ValidatingDocumentException(fileBase);
            }

            InMemoryClipboard = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
            dlClipboard = new SimpleDataLayer(InMemoryClipboard);
            uowClipboard = new UnitOfWork(dlClipboard);

            try
            {
                //driver specific class
                configuration = (from tag in new XPQuery<IEC60870_5_104DriverSettings>(ufw).AsParallel() select tag).Single();

            }
            catch (InvalidOperationException ex)
            {
                configuration = new IEC60870_5_104DriverSettings(ufw);//driver specific class
                configuration.DefaultSettings();
            }
        }

        bool IsBelongFromParent()
        {
            if (String.IsNullOrEmpty(fileBase) || !File.Exists(fileBase))
                return true;

            if (comSetContext.bProtected && Utilities.IO.FileSystem.IsXmlFile(fileBase))
                return false;

            var id = XpoHelpers.XpoHelper.GetProtectionCode(ufw);
            if ((id == Guid.Empty) || (id == comSetContext.protectionCode))
            {
                return (true);
            }
            else if ((comSetContext.protectionCode == Guid.Empty) && !comSetContext.bProtected)
            {
                comSetContext.bProtected = true;
                comSetContext.protectionCode = id;
                return (true);
            }
            else
            {
                return (false);
            }
        }

        /// <summary>   Sets current data. </summary>
        private void SetCurrentData()
        {
            ReadSettings();

            DataContext = null;
            DataContext = configuration;

            if (configuration != null && configuration.ChannelSettings != null)
            {
                cht = new DriverCodeBase.UI.Controls.BaseChannelTree() { DataContext = configuration };
                cht.AddStationEv += StationAdd_Click;
                cht.AddChannelEv += ChannelAdd_Click;
                cht.EditEv += ChannelEdit_Click;
                cht.TreeDblClickEv += ChannelEdit_DblClick;
                cht.CopyEv += OnCopy;
                cht.CutEv += OnCut;
                cht.PasteEv += OnPaste;
                cht.SingleStationForChannel = true;
                ChannelSettings.Content = cht;
            }
            DriverCodeBase.UI.Controls.BaseGeneralSettings baseGeneralSetting = new DriverCodeBase.UI.Controls.BaseGeneralSettings() { DataContext = DataContext };
            baseGeneralSetting.AggregationThresholdText.Visibility = System.Windows.Visibility.Collapsed;
            baseGeneralSetting.AggregationThreshold.Visibility = System.Windows.Visibility.Collapsed;
            baseGeneralSetting.AggregationLimitText.Visibility = System.Windows.Visibility.Collapsed;
            baseGeneralSetting.AggregationLimit.Visibility = System.Windows.Visibility.Collapsed;
            baseGeneralSetting.textBlockSynchAtStartup.Visibility = System.Windows.Visibility.Collapsed;
            baseGeneralSetting.SynchAtStartup.Visibility = System.Windows.Visibility.Collapsed;
            baseGeneralSetting.textBlockWriteAsync.Visibility = Visibility.Collapsed;
            baseGeneralSetting.WriteAsync.Visibility = Visibility.Collapsed;
            MainStack.Children.Add(baseGeneralSetting);
        }

        #region WinClipboard
        internal void CopyInMemoryDataToWinClipboard()
        {
            Clipboard.Clear();
            try
            {
                using (var xml = new StringWriter())
                {
                    using (var xmlTextWriter = new XmlTextWriter(xml) { Formatting = System.Xml.Formatting.Indented })
                    {
                        InMemoryClipboard.WriteXml(xmlTextWriter);
                    }

                    Clipboard.SetText(xml.ToString());
                }
            }
            catch (Exception ex)
            {
                Assembly a = Assembly.GetAssembly(this.GetType());
                System.Windows.MessageBox.Show(ex.Message, a.GetName().Name, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        string LastClipboardUnicodeText = String.Empty;
        internal void CopyWinClipboardToInMemoryData(bool force = false)
        {
            if (force || Clipboard.ContainsText(TextDataFormat.UnicodeText) &&
                Clipboard.GetText(TextDataFormat.UnicodeText) != LastClipboardUnicodeText)
            {
                CleanClipbaord();
                LastClipboardUnicodeText = Clipboard.GetText(TextDataFormat.UnicodeText);
                using (var xml = new StringReader(Clipboard.GetText(TextDataFormat.UnicodeText)))
                {
                    using (var xmlTextReader = new XmlTextReader(xml))
                    {
                        var tempds = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
                        //using (var tempdl = new SimpleDataLayer(tempds))
                        {
                            bool bValid = true;
                            try
                            {
                                tempds.ReadXml(xmlTextReader);
                            }
                            catch
                            {
                                bValid = false;
                            }

                            if (bValid)
                            {
                                InMemoryClipboard.ReadFromInMemoryDataStore(tempds);
                            }
                        }
                    }
                }
            }
        }
        internal void CleanClipbaord()
        {
            if (uowClipboard == null)
                return;

            uowClipboard.ClearDatabase();

        }
        #endregion

        #region clipboard
        internal XPObject CloneToClipboard(XPObject obj, bool checkattributes)
        {

            XpoHelpers.CloneIXPSimpleObjectHelper cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(ufw, uowClipboard, checkattributes, true, true);
            return cloneHelper.Clone(obj, false);
        }

        internal XPObject CloneFromClipboard(XPObject obj, bool checkattributes)
        {

            XpoHelpers.CloneIXPSimpleObjectHelper cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(uowClipboard, ufw, checkattributes);
            return cloneHelper.Clone(obj, false);
        }


        internal void CopySelectedToClipboard()
        {
            var slctd = cht.GetSelecteItem();
            if (slctd == null)
                return;

            CleanClipbaord();

            if (slctd is IEC60870_5_104ChannelSettings)
                CloneToClipboard(slctd as IEC60870_5_104ChannelSettings, false);
            else if (slctd is IEC60870_5_104StationSettings)
                CloneToClipboard(slctd as IEC60870_5_104StationSettings, false);

            uowClipboard.CommitChanges();

        }

        internal void PasteFromClipboard()
        {
            var channelstocopy = (from c in new XPQuery<IEC60870_5_104ChannelSettings>(uowClipboard).AsParallel() select c).ToList();
            if (channelstocopy.Count > 0)
            {
                var ch = new IEC60870_5_104ChannelSettings(ufw);
                ch.CopyProperties(channelstocopy[0]);
                IEC60870_5_104DriverSettings mds = DataContext as IEC60870_5_104DriverSettings;
                ch.DriverSettings = mds;
                if (ch.DriverSettings != null)
                    ch.Name = ch.DriverSettings.GetNewChannelName();
                cht.AddChannel(ch);
                return;
            }
            var stationstocopy = (from c in new XPQuery<IEC60870_5_104StationSettings>(uowClipboard).AsParallel() select c).ToList();
            if (stationstocopy.Count > 0)
            {
                var channel = cht.GetSelecteItem() as IEC60870_5_104ChannelSettings;
                if (channel == null)
                    return;
                var st = new IEC60870_5_104StationSettings(ufw);
                st.CopyProperties(stationstocopy[0]);
                IEC60870_5_104DriverSettings mds = DataContext as IEC60870_5_104DriverSettings;
                st.DriverSettings = mds;
                if (st.DriverSettings != null)
                    st.Name = st.DriverSettings.GetNewStationName();
                st.Channel = channel.Name;
                cht.AddStation(st);
            }
        }
        #endregion

        private void OnCopy(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            CopySelectedToClipboard();
            CopyInMemoryDataToWinClipboard();
        }
        private void OnCut(object sender, ExecutedRoutedEventArgs e)
        { }
        private void OnPaste(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            CopyWinClipboardToInMemoryData(true);
            PasteFromClipboard();
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
            var ch = new IEC60870_5_104ChannelSettings(ufw);
            IEC60870_5_104DriverSettings mds = DataContext as IEC60870_5_104DriverSettings;
            ch.DriverSettings = mds;
            ch.DefaultSettings();
            d.DataContext = ch;

            GeneralDialogContent newChDetDialog = new GeneralDialogContent(d)
            {
                Owner = this.FindParent<Window>(),
                Title = DriverCodeBase.UI.Properties.Resources.ChannelsSettings
            };
            if (newChDetDialog.ShowDialog() != true)
            {
                //Cancel, do not add
                mds.ChannelSettings.Remove(ch);
                ch.Delete();
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

                using (var uow = ufw.BeginNestedUnitOfWork())
                {
                    ChannelDetails d = new ChannelDetails();
                    d.DataContext = uow.GetNestedObject(ch);
                    GeneralDialogContent newChDetDialog = new GeneralDialogContent(d)
                    {
                        Owner = this.FindParent<Window>(),
                        Title = DriverCodeBase.UI.Properties.Resources.ChannelsSettings
                    };
                    if (newChDetDialog.ShowDialog() == true)
                        uow.CommitChanges();
                }
                
            }
            else if (cht.GetSelecteItem() as IEC60870_5_104StationSettings != null)
            {
                var st = cht.GetSelecteItem() as IEC60870_5_104StationSettings;
                using (var uow = ufw.BeginNestedUnitOfWork())
                {
                    string oldname = st.Channel;
                    StationDetails d = new StationDetails();
                    d.DataContext = uow.GetNestedObject(st);
                    GeneralDialogContent newChDetDialog = new GeneralDialogContent(d)
                    {
                        Title = DriverCodeBase.UI.Properties.Resources.StationsSettings,
                        Owner = this.FindParent<Window>()
                    };
                    if (newChDetDialog.ShowDialog() == true)
                        uow.CommitChanges();
                    if (st.Channel != oldname)
                    {
                        var temp = ChannelSettings.Content as DriverCodeBase.UI.Controls.BaseChannelTree;
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
            var channel = cht.GetSelectedChannel() as IEC60870_5_104ChannelSettings;
            if (channel == null)
            {

                return;
            }
            var listStations = (from c in channel.DriverSettings.StationSettings where c.Channel == channel.Name select c).ToList();
            if (listStations.Count > 0)
                return;

            StationDetails d = new StationDetails();
            var st = new IEC60870_5_104StationSettings(ufw);

            IEC60870_5_104DriverSettings mds = DataContext as IEC60870_5_104DriverSettings;
            st.DriverSettings = mds;
            st.DefaultSettings();
            st.Channel = channel.Name;
            d.DataContext = st;

            GeneralDialogContent newChDetDialog = new GeneralDialogContent(d)
            {
                Owner = this.FindParent<Window>(),
                Title = DriverCodeBase.UI.Properties.Resources.StationsSettings
            };
            if (newChDetDialog.ShowDialog() != true)
            {
                //Cancel, do not add
                mds.StationSettings.Remove(st);
                st.Delete();
                return;
            }
            cht.AddStation(st);
        }

        #endregion

        #region ServiceMethods
        /// <summary>   Saves the driver settings. </summary>
        public void SaveDriverSettings()
        {
            if (comSetContext == null)
                return;

            if (comSetContext.bProtected)
                XpoHelpers.XpoHelper.AddProtectionCode(ufw, comSetContext.protectionCode);
            else
                XpoHelpers.XpoHelper.RemoveProtectionCode(ufw);

            try
            {
                ufw.CommitChanges();
                ufw.ReloadChangedObjects();
                ufw.PurgeDeletedObjects();

                ufw.CommitChanges();
            }
            catch (Exception ex)
            {
                if (Environment.UserInteractive)
                    MessageBox.Show(ex.Message);
                if (File.Exists(fileBase))
                {
                    try
                    {
                        File.Delete(fileBase);
                    }
                    catch (Exception ex1)
                    {
                        if (Environment.UserInteractive)
                            MessageBox.Show(ex1.Message);
                    }
                }
                return;
            }

            if (!String.IsNullOrEmpty(fileBase))
            {
                if (File.Exists(fileBase))
                {
                    try
                    {
                        File.Delete(fileBase);
                    }
                    catch (Exception ex)
                    {
                        if (Environment.UserInteractive)
                            MessageBox.Show(ex.Message);
                        return;
                    }
                }
                try
                {
                    if (comSetContext.bProtected)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            var writer = XmlWriter.Create(memoryStream);
                            InMemory.WriteXml(writer);
                            writer.Flush();//14577
                            writer.Close();
                            var str = Convert.ToBase64String(memoryStream.ToArray());
                            var toWrite = DriverCodeBase.CommunicationDriver.EncryptString(str);
                            File.WriteAllText(fileBase, toWrite);
                        }
                    }
                    else
                        InMemory.WriteXml(fileBase);
                }
                catch (Exception ex)
                {
                    Assembly a = Assembly.GetAssembly(this.GetType());
                    System.Windows.MessageBox.Show(ex.Message, a.GetName().Name, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
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

            if (ufw != null)
            {
                ufw.Disconnect();
                ufw.Dispose();
                ufw = null;
            }

            if (uowClipboard != null)
            {
                uowClipboard.Disconnect();
                uowClipboard.Dispose();
                uowClipboard = null;
            }
            if (dlClipboard != null)
                dlClipboard.Dispose();

            if (idl != null)
                idl.Dispose();
        }

        #endregion
    }
}

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
using EIB;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using System.Xml;
using System.IO;
using DriverCodeBaseEx;


namespace EIB.UI
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class GeneralSettingsEditor : UserControl, IDisposable
    {
        #region Declarations
        DriverSettingsInterfaces.ComunicationSettingsContext2 comSetContext;
        //IDataLayer idl = null;
        //UnitOfWork ufw = null;
        //EIBDriverSettings configuration = null;
        //string fileBase;
        //InMemoryDataStore InMemory = null;
        ConfigurationEditor configurationEditor = new ConfigurationEditor();
        DriverCodeBaseEx.UI.Controls.BaseChannelTree cht = null;
        bool alreadyLoaded = false;
        IDataLayer dlClipboard;
        UnitOfWork uowClipboard;
        InMemoryDataStore InMemoryClipboard;
        string DriverName;
        #endregion

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
                {
                    return;
                }

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
                configurationEditor.Init<EIBDriverSettings>(comSetContext.ConnectionString, DriverName, comSetContext.bProtected, comSetContext.protectionCode);

            if (!configurationEditor.IsValid)
            {
                if (!configurationEditor.IsBelongFromParent())
                    throw new DriverBaseInterfaces.ValidatingDocumentException(configurationEditor.FileBase);

                MessageBox.Show(string.Format(DriverCodeBaseEx.Properties.Resources.ErrorOpeningDocument, comSetContext.ConnectionString, DriverName));
                return;
            }

            InMemoryClipboard = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
            dlClipboard = new SimpleDataLayer(InMemoryClipboard);
            uowClipboard = new UnitOfWork(dlClipboard);

        }

        private void SetCurrentData()
        {
            ReadSettings();

            DataContext = null;
            DataContext = configurationEditor.Configuration as EIBDriverSettings;
            if (configurationEditor.Configuration is EIBDriverSettings && (configurationEditor.Configuration as EIBDriverSettings).ChannelSettings != null)
            {
                cht = new DriverCodeBaseEx.UI.Controls.BaseChannelTree() { DataContext = configurationEditor.Configuration as EIBDriverSettings };
                cht.AddStationEv += StationAdd_Click;
                cht.AddChannelEv += ChannelAdd_Click;
                cht.EditEv += ChannelEdit_Click;
                //cht.DeleteEv += ChannelDelete_Click;
                cht.TreeDblClickEv += ChannelEdit_DblClick;
                cht.TestCommSupported = true;
                cht.TestCommEv += TestComm_Click;
                cht.CopyEv += OnCopy;
                cht.CutEv += OnCut;
                cht.PasteEv += OnPaste;
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
            MainStack.Children.Add(genset);
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

            XpoHelpers.CloneIXPSimpleObjectHelper cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(configurationEditor.UfW, uowClipboard, checkattributes, true, true);
            return cloneHelper.Clone(obj, false);
        }

        internal XPObject CloneFromClipboard(XPObject obj, bool checkattributes)
        {

            XpoHelpers.CloneIXPSimpleObjectHelper cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(uowClipboard, configurationEditor.UfW, checkattributes);
            return cloneHelper.Clone(obj, false);
        }

        internal void CopySelectedToClipboard()
        {
            var slctd = cht.GetSelecteItem();
            if (slctd == null)
                return;

            CleanClipbaord();

            if (slctd is EIBChannelSettings)
                CloneToClipboard(slctd as EIBChannelSettings, false);
            else if (slctd is EIBStationSettings)
                CloneToClipboard(slctd as EIBStationSettings, false);

            uowClipboard.CommitChanges();

        }

        internal void PasteFromClipboard()
        {
            var channelstocopy = (from c in new XPQuery<EIBChannelSettings>(uowClipboard).AsParallel() select c).ToList();
            if (channelstocopy.Count > 0)
            {
                var ch = configurationEditor.NewChannel();
                ch.CopyProperties(channelstocopy[0]);
                EIBDriverSettings mds = DataContext as EIBDriverSettings;
                ch.DriverSettings = mds;
                if (ch.DriverSettings != null)
                    ch.Name = ch.DriverSettings.GetNewChannelName();
                cht.AddChannel(ch);
                return;
            }
            var stationstocopy = (from c in new XPQuery<EIBStationSettings>(uowClipboard).AsParallel() select c).ToList();
            if (stationstocopy.Count > 0)
            {
                var channel = cht.GetSelecteItem() as EIBChannelSettings;
                if (channel == null)
                    return;
                var st = configurationEditor.NewStation();
                st.CopyProperties(stationstocopy[0]);
                EIBDriverSettings mds = DataContext as EIBDriverSettings;
                st.DriverSettings = mds;
                if (st.DriverSettings != null)
                    st.Name = st.DriverSettings.GetNewStationName();
                st.Channel = channel.Name;
                cht.AddStation(st);
            }
        }
        #endregion

        #region ServiceMethods
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
        //                    var toWrite = DriverCodeBaseEx.CommunicationDriver.EncryptString(str);//WPFUtilities.CryptString.CryptString.EncryptString(str);
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

        private void OnCopy(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            CopySelectedToClipboard();
            CopyInMemoryDataToWinClipboard();
        }

        private void OnCut(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void OnPaste(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            CopyWinClipboardToInMemoryData(true);
            PasteFromClipboard();
        }

        private void ChannelEdit_DblClick(object sender, MouseButtonEventArgs e)
        {
            Edit();
        }

        private void ChannelEdit_Click(object sender, RoutedEventArgs e)
        {
            Edit();
        }

        private void Edit()
        {
            if (cht.GetSelecteItem() as EIBChannelSettings != null)
            {
                var ch = cht.GetSelecteItem() as EIBChannelSettings;

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
            else if (cht.GetSelecteItem() as EIBStationSettings != null)
            {
                var st = cht.GetSelecteItem() as EIBStationSettings;
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
                configurationEditor.RemoveChannelSettings(ch.Name, true);
                return;
            }
            cht.AddChannel(ch);
        }

        private void StationAdd_Click(object sender, RoutedEventArgs e)
        {
            var channel = cht.GetSelectedChannel() as EIBChannelSettings;
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
                configurationEditor.RemoveStationSettings(channel.Name, st.Name);
                return;
            }
            cht.AddStation(st);
        }

        private void TestComm_Click(object sender, RoutedEventArgs e)
        {
            if (!(cht.GetSelecteItem() is EIBStationSettings))
                return;

            var stationSettings = cht.GetSelecteItem() as EIBStationSettings;
            var channelSettings = cht.GetSelectedChannel() as EIBChannelSettings;

            using (EIBDriver driver = new EIBDriver(comSetContext.ConnectionString))
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

            configurationEditor.Dispose();

            if (uowClipboard != null)
            {
                uowClipboard.Disconnect();
                uowClipboard.Dispose();
                uowClipboard = null;
            }
            if (dlClipboard != null)
                dlClipboard.Dispose();
        }

        #endregion
    }
}

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
using BrPvi;

namespace BrPvi.UI
{
    /// <summary>
    /// Interaction logic for GeneralSettingsEditor.xaml
    /// </summary>
    public partial class GeneralSettingsEditor : UserControl, IDisposable
    {
        #region Declarations
        DriverSettingsInterfaces.ComunicationSettingsContext2 comSetContext;
        bool alreadyLoaded = false;
        //IDataLayer idl = null;
        //UnitOfWork ufw = null;
        //BrPviDriverSettings configuration = null;
        //string fileBase;
        ////IDataLayer ProjectIdl = null;
        //InMemoryDataStore InMemory = null;

        ConfigurationEditor configurationEditor = new ConfigurationEditor();
        DriverCodeBaseEx.UI.Controls.BaseChannelTree cht = null;
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
            //idl = DriverCodeBaseEx.CommunicationDriver.GetDriverDataLayer(comSetContext.ConnectionString, DriverName, out fileBase, out InMemory); //, false);            
            //if (idl == null)
            //{
            //    MessageBox.Show(string.Format(DriverCodeBaseEx.Properties.Resources.ErrorOpeningDocument, comSetContext.ConnectionString, DriverName));
            //    return;
            //}

            //ufw = new UnitOfWork(idl);

            //if (!IsBelongFromParent())
            //{
            //    throw new DriverBaseInterfaces.ValidatingDocumentException(fileBase);
            //}

            if (!configurationEditor.IsValid)
                configurationEditor.Init< BrPviDriverSettings>(comSetContext.ConnectionString, DriverName, comSetContext.bProtected, comSetContext.protectionCode);

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

            //try
            //{
            //    //driver specific class
            //    configuration = (from tag in new XPQuery<BrPviDriverSettings>(ufw).AsParallel() select tag).Single();

            //}
            //catch (InvalidOperationException ex)
            //{
            //    configuration = new BrPviDriverSettings(ufw);//driver specific class
            //    configuration.DefaultSettings();
            //}

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

        private void SetCurrentData()
        {
            ReadSettings();

            DataContext = null;
            DataContext = configurationEditor.Configuration as BrPviDriverSettings;
            if (configurationEditor.Configuration is BrPviDriverSettings && (configurationEditor.Configuration as BrPviDriverSettings).ChannelSettings != null)
            {
                cht = new DriverCodeBaseEx.UI.Controls.BaseChannelTree() { DataContext = configurationEditor.Configuration as BrPviDriverSettings };
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
            MainStack.Children.Add(genset);
        }

        #region WinClipboard
        internal void CopyInMemoryDataToWinClipboard()
        {
            Clipboard.Clear();
            using (var xml = new StringWriter())
            {
                using (var xmlTextWriter = new XmlTextWriter(xml) { Formatting = System.Xml.Formatting.Indented })
                {
                    InMemoryClipboard.WriteXml(xmlTextWriter);
                }

                Clipboard.SetText(xml.ToString());
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

            //XpoHelpers.CloneIXPSimpleObjectHelper cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(ufw, uowClipboard, checkattributes, true, true);
            XpoHelpers.CloneIXPSimpleObjectHelper cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(configurationEditor.UfW, uowClipboard, checkattributes, true, true);
            return cloneHelper.Clone(obj, false);
        }

        internal XPObject CloneFromClipboard(XPObject obj, bool checkattributes)
        {

            //XpoHelpers.CloneIXPSimpleObjectHelper cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(uowClipboard, ufw, checkattributes);
            XpoHelpers.CloneIXPSimpleObjectHelper cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(uowClipboard, configurationEditor.UfW, checkattributes);
            return cloneHelper.Clone(obj, false);
        }


        internal void CopySelectedToClipboard()
        {
            var slctd = cht.GetSelecteItem();
            if (slctd == null)
                return;

            CleanClipbaord();

            if (slctd is BrPviChannelSettings)
                CloneToClipboard(slctd as BrPviChannelSettings, false);
            else if (slctd is BrPviStationSettings)
                CloneToClipboard(slctd as BrPviStationSettings, false);

            uowClipboard.CommitChanges();

        }

        internal void PasteFromClipboard()
        {
            var channelstocopy = (from c in new XPQuery<BrPviChannelSettings>(uowClipboard).AsParallel() select c).ToList();
            if (channelstocopy.Count > 0)
            {
                var ch = configurationEditor.NewChannel();
                ch.CopyProperties(channelstocopy[0]);
                BrPviDriverSettings mds = DataContext as BrPviDriverSettings;
                ch.DriverSettings = mds;
                if (ch.DriverSettings != null)
                    ch.Name = ch.DriverSettings.GetNewChannelName();
                cht.AddChannel(ch);
                return;
            }
            var stationstocopy = (from c in new XPQuery<BrPviStationSettings>(uowClipboard).AsParallel() select c).ToList();
            if (stationstocopy.Count > 0)
            {
                var channel = cht.GetSelecteItem() as BrPviChannelSettings;
                if (channel == null)
                    return;
                var st = configurationEditor.NewStation();
                st.CopyProperties(stationstocopy[0]);
                BrPviDriverSettings mds = DataContext as BrPviDriverSettings;
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

        //private void ChannelDelete_Click(object sender, RoutedEventArgs e)
        //{
        //    if (ChannelsGrid.SelectedItem != null)
        //    {
        //        var ch = ChannelsGrid.SelectedItem as BrPviChannelSettings;
        //        if (MessageBox.Show(string.Format(Properties.Resources.CaptionAskDeleteChannel, ch.Name),
        //            Properties.Resources.CaptionDeleteChannel, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        //        {
        //            BrPviDriverSettings mds = DataContext as BrPviDriverSettings;

        //            mds.ChannelSettings.Remove(ch);
        //            ch.Delete();
        //        }
        //    }
        //}

        private void ChannelAdd_Click(object sender, RoutedEventArgs e)
        {
            ChannelDetails d = new ChannelDetails();
            //var ch = new BrPviChannelSettings(ufw);
            //BrPviDriverSettings mds = DataContext as BrPviDriverSettings;
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

        //private void ChannelEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    if (ChannelsGrid.SelectedItem != null)
        //    {
        //        var ch = ChannelsGrid.SelectedItem as BrPviChannelSettings;

        //        BrPviChannelSettings newch = new BrPviChannelSettings(ufw);
        //        newch.CopyProperties(ch);

        //        ChannelDetails d = new ChannelDetails();
        //        d.DataContext = ch;

        //        GeneralDialogContent newChDetDialog = new GeneralDialogContent(d)
        //        {
        //            Owner = this.FindParent<Window>()
        //        };
        //        if (newChDetDialog.ShowDialog() != true)
        //        {
        //            //cancel, back to older values...
        //            ch.CopyProperties(newch);
        //        }
        //    }
        //}
        private void ChannelEdit_Click(object sender, RoutedEventArgs e)
        {
            Edit();
        }

        private void ChannelEdit_DblClick(object sender, MouseButtonEventArgs e)
        {
            Edit();
        }

        private void Edit()
        {
            if (cht.GetSelecteItem() as BrPviChannelSettings != null)
            {
                var ch = cht.GetSelecteItem() as BrPviChannelSettings;

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
            else if (cht.GetSelecteItem() as BrPviStationSettings != null)
            {
                var st = cht.GetSelecteItem() as BrPviStationSettings;
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

        //private void StationDelete_Click(object sender, RoutedEventArgs e)
        //{
        //    if (StationsGrid.SelectedItem != null)
        //    {
        //        var st = StationsGrid.SelectedItem as BrPviStationSettings;
        //        if (MessageBox.Show(string.Format(Properties.Resources.CaptionAskDeleteStation, st.Name),
        //            Properties.Resources.CaptionDeleteStation, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        //        {
        //            BrPviDriverSettings mds = DataContext as BrPviDriverSettings;

        //            mds.StationSettings.Remove(st);
        //            st.Delete();
        //        }
        //    }
        //}
 
        private void StationAdd_Click(object sender, RoutedEventArgs e)
        {
            var channel = cht.GetSelectedChannel() as BrPviChannelSettings;
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
                Title = DriverCodeBaseEx.UI.Properties.Resources.ChannelsSettings
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
            if (!(cht.GetSelecteItem() is BrPviStationSettings))
                return;

            var stationSettings = cht.GetSelecteItem() as BrPviStationSettings;
            var channelSettings = cht.GetSelectedChannel() as BrPviChannelSettings;

            using (BrPviDriver driver = new BrPviDriver(comSetContext.ConnectionString))
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

            //if (ufw != null)
            //{
            //    ufw.Disconnect();
            //    ufw.Dispose();
            //    ufw = null;
            //}
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

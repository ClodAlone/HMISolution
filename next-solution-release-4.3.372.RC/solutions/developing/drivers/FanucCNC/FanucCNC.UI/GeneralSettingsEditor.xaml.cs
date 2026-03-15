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

namespace FanucCNC.UI
{
    /// <summary>
    /// Interaction logic for GeneralSettingsEditor.xaml
    /// </summary>
    public partial class GeneralSettingsEditor : UserControl, IDisposable
    {
        DriverSettingsInterfaces.ComunicationSettingsContext2 comSetContext;
        IDataLayer idl = null;
        UnitOfWork ufw = null;
        FanucCNCDriverSettings configuration = null;
        bool alreadyLoaded = false;
        string fileBase;
        InMemoryDataStore InMemory = null;
        DriverCodeBase.UI.Controls.BaseChannelTree cht = null;
        IDataLayer dlClipboard;
        UnitOfWork uowClipboard;
        InMemoryDataStore InMemoryClipboard;

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
                configuration = (from tag in new XPQuery<FanucCNCDriverSettings>(ufw).AsParallel() select tag).Single();

            }
            catch (InvalidOperationException ex)
            {
                configuration = new FanucCNCDriverSettings(ufw);//driver specific class
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
                //cht.DeleteEv += ChannelDelete_Click;
                cht.TreeDblClickEv += ChannelEdit_DblClick;
                cht.CopyEv += OnCopy;
                cht.CutEv += OnCut;
                cht.PasteEv += OnPaste;
                cht.SingleStationForChannel = true;
                ChannelSettings.Content = cht;
            }

            var genset = new DriverCodeBase.UI.Controls.BaseGeneralSettings();
            //genset.AggregationThresholdText.Visibility = System.Windows.Visibility.Collapsed;
            //genset.AggregationThreshold.Visibility = System.Windows.Visibility.Collapsed;
            //genset.AggregationLimitText.Visibility = System.Windows.Visibility.Collapsed;
            //genset.AggregationLimit.Visibility = System.Windows.Visibility.Collapsed;
            genset.DataContext = DataContext;
            //gridGeneral.Children.Add(genset);
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

            if (slctd is FanucCNCChannelSettings)
                CloneToClipboard(slctd as FanucCNCChannelSettings, false);
            else if (slctd is FanucCNCStationSettings)
                CloneToClipboard(slctd as FanucCNCStationSettings, false);

            uowClipboard.CommitChanges();

        }

        internal void PasteFromClipboard()
        {
            var channelstocopy = (from c in new XPQuery<FanucCNCChannelSettings>(uowClipboard).AsParallel() select c).ToList();
            if (channelstocopy.Count > 0)
            {
                var ch = new FanucCNCChannelSettings(ufw);
                ch.CopyProperties(channelstocopy[0]);
                FanucCNCDriverSettings mds = DataContext as FanucCNCDriverSettings;
                ch.DriverSettings = mds;
                if (ch.DriverSettings != null)
                    ch.Name = ch.DriverSettings.GetNewChannelName();
                cht.AddChannel(ch);
                return;
            }
            var stationstocopy = (from c in new XPQuery<FanucCNCStationSettings>(uowClipboard).AsParallel() select c).ToList();
            if (stationstocopy.Count > 0)
            {
                var channel = cht.GetSelecteItem() as FanucCNCChannelSettings;
                if (channel == null)
                    return;
                var st = new FanucCNCStationSettings(ufw);
                st.CopyProperties(stationstocopy[0]);
                FanucCNCDriverSettings mds = DataContext as FanucCNCDriverSettings;
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

        private void ChannelAdd_Click(object sender, RoutedEventArgs e)
        {
            ChannelDetails d = new ChannelDetails();
            var ch = new FanucCNCChannelSettings(ufw);
            FanucCNCDriverSettings mds = DataContext as FanucCNCDriverSettings;
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
        private void Edit()
        {
            if (cht.GetSelecteItem() as FanucCNCChannelSettings != null)
            {
                var ch = cht.GetSelecteItem() as FanucCNCChannelSettings;

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
            else if (cht.GetSelecteItem() as FanucCNCStationSettings != null)
            {
                var st = cht.GetSelecteItem() as FanucCNCStationSettings;
                using (var uow = ufw.BeginNestedUnitOfWork())
                {
                    string oldname = st.Channel;
                    StationDetails d = new StationDetails();
                    d.DataContext = uow.GetNestedObject(st);
                    GeneralDialogContent newChDetDialog = new GeneralDialogContent(d)
                    {
                        Owner = this.FindParent<Window>(),
                        Title = DriverCodeBase.UI.Properties.Resources.StationsSettings
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
            var channel = cht.GetSelecteItem() as FanucCNCChannelSettings;
            if (channel == null)
            {
                return;
            }

            // don't allow user to add more than one station for channel (no communications parameters are inside station)
            var listStations = (from c in channel.DriverSettings.StationSettings where c.Channel == channel.Name select c).ToList();
            if (listStations.Count > 0)
                return;

            StationDetails d = new StationDetails();
            var st = new FanucCNCStationSettings(ufw);

            FanucCNCDriverSettings mds = DataContext as FanucCNCDriverSettings;
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

        #region ServiceMethods
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
                            var toWrite = DriverCodeBase.CommunicationDriver.EncryptString(str);//WPFUtilities.CryptString.CryptString.EncryptString(str);
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
        bool bDisposed;
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

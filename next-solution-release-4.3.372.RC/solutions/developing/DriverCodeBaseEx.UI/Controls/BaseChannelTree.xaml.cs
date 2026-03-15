using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using DevExpress.Xpo.DB;
using DevExpress.Xpo;
using Utilities;
using Utilities.WPF;
using WPFUtilities;
using System.Xml;
using System.IO;
using System.Reflection;

namespace DriverCodeBaseEx.UI.Controls
{
    /// <summary>
    /// Interaction logic for Base Channel
    /// </summary>
    public partial class BaseChannelTree : UserControl, IDisposable
    {
        #region Declarations
        BitmapImage openFolderImg;
        BitmapImage closedFolderImg;
        TreeListNode itemRoot;
        readonly static String rootName = "Root";
        DriverSettings currSettings = null;
        public event EventHandler<RoutedEventArgs> AddStationEv;
        public event EventHandler<RoutedEventArgs> AddChannelEv;
        public event EventHandler<RoutedEventArgs> EditEv;
        public event EventHandler<RoutedEventArgs> TestCommEv;
        public event EventHandler<MouseButtonEventArgs> TreeDblClickEv;

        // block to add more station to channel
        public bool SingleStationForChannel { get; set; } = false;
        // enable Test Communication button only if driver support this function
        public bool TestCommSupported { get; set; } = false;

        bool alreadyLoaded = false;
        private bool? _CanAddNewStationEvaluated = null;

        IDataLayer dlClipboard;
        UnitOfWork uowClipboard;
        InMemoryDataStore InMemoryClipboard;
        BaseConfigurationEditor configurationEditor = null;

        #endregion

        #region CTors

        /// <summary>
        /// basic channel settings 
        /// </summary>
        public BaseChannelTree()
        {
            InitializeComponent();


            Loaded += (o, e) =>
            {
                if (alreadyLoaded || bDisposed)
                    return;

                alreadyLoaded = true;
                currSettings = DataContext as DriverSettings;
                if (currSettings != null && currSettings.ChannelSettings != null)
                    InitChannelTree();

                if (TestCommSupported)
                    btnTestComm.Visibility = Visibility.Visible;

                InMemoryClipboard = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
                dlClipboard = new SimpleDataLayer(InMemoryClipboard);
                uowClipboard = new UnitOfWork(dlClipboard);
            };
        }
        #endregion

        #region Public Methods

        public object GetSelecteItem()
        {
            return (treeListControl?.GetSelectedNodes().FirstOrDefault() as TreeListNode != null ? (treeListControl?.GetSelectedNodes().FirstOrDefault() as TreeListNode).Tag : null);
        }

        public List<ChannelSettings> GetSelecteChannels()
        {
            List<object> chList = new List<object>();   
            chList.AddRange(treeListControl?.SelectedItems.Count != 0 ? treeListControl.GetTreeSelectedItems() : null);
            
            return (from p in chList where p is ChannelSettings select ((ChannelSettings)p)).ToList();
        }
        public List<StationSettings> GetSelecteStations()
        {
            List<object> chList = new List<object>();
            chList = (treeListControl?.SelectedItems.Count != 0 ? treeListControl.GetTreeSelectedItems() : null);

            return (from p in chList where p is StationSettings select ((StationSettings)p)).ToList();
        }

        public void AddStation(StationSettings station)
        {
            var list = (from p in itemRoot.Nodes where (p.Tag as ChannelSettings).Name == station.Channel select p).ToList();
            if (list.Count > 0)
            {
                treeListControl.ClearSelection();
                var item = AddTreeItem(station, list[0]);
                treeListControl.SelectNode(item);
            }
        }
        public void AddChannel(ChannelSettings channel)
        {
            treeListControl.ClearSelection();
            var item = AddTreeItem(channel, itemRoot);
            treeListControl.SelectNode(item);
        }
        public void DeleteSelectedItem()
        {
            var items = new List<TreeListNode>();
            foreach (var item in treeListControl.GetSelectedNodes())
            {
                if (item is TreeListNode)
                    items.Add(item as TreeListNode);
            }

            try
            {
                treeListControl.BeginDataUpdate();
                foreach (var item in items)
                    DeleteTreeItem(item);
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }

            items.Clear();
        }

        public object GetSelecteParentItem()
        {
            return (treeListControl?.GetSelectedNodes().FirstOrDefault() as TreeListNode != null ? (treeListControl?.GetSelectedNodes().FirstOrDefault() as TreeListNode).ParentNode.Tag : null);
        }

        public object GetSelectedChannel()
        {
            object selectedElement = GetSelecteItem();
            if (selectedElement is ChannelSettings)
            {
                return selectedElement;
            }
            else
            { // is selected element is a station, get channel (item) from previous/parent node
                if (selectedElement is StationSettings)
                    return GetSelecteParentItem();
                else
                    return selectedElement;
            }            
        }

        public void SetConfigurationEditor(BaseConfigurationEditor ce)
        {
            configurationEditor= ce;
        }

        #endregion

        #region Methods
        internal void DeleteTreeItem(TreeListNode item)
        {
            var parent = item.ParentNode ?? itemRoot;
            parent.Nodes.Remove(item);
        }

        static BitmapImage GetBitmapImage(String image)
        {
            var bm = new BitmapImage();
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

            String str = String.Format("pack://application:,,,/{0};component/Images/{1}.png",
                System.IO.Path.GetFileNameWithoutExtension(assembly.Location), image);
            bm.BeginInit();
            bm.UriSource = new Uri(str);
            bm.EndInit();
            return bm;
        }

        public void InitChannelTree()
        {
            openFolderImg = DockingHelper.GetBitmapImageSource("OpenFolderSmall", true);
            closedFolderImg = DockingHelper.GetBitmapImageSource("CloseFolderSmall", true);
            treeListView.Nodes.Clear();
            itemRoot = treeListControl.AddNode(new TreeItemControl(rootName) { ResourceIcon = closedFolderImg });
            treeListControl.AddNode(null, itemRoot, TreeListControlHelper.DummyNode);

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                if (bDisposed)
                    return;

                FillItems(itemRoot);
                if (itemRoot.Nodes.Count == 0)
                    treeListControl.AddNode(null, itemRoot, TreeListControlHelper.DummyNode);
                itemRoot.IsExpanded = true;
                UpdateNrChannelsAndStations();
            });
        }

        

        void ClearNodes(TreeListNode node)
        {
            if (node == null)
                return;

            //var mappedChildNodes = (from TreeItemControl data in contentToNodeMap.Keys where node.Nodes.Contains(contentToNodeMap[data]) select data).ToList();
            //foreach (var k in mappedChildNodes)
            //    contentToNodeMap.Remove(k);
            node.Nodes.Clear();
        }

        private void FillItems(TreeListNode itemRoot)
        {
            treeListControl.BeginDataUpdate();
            try
            {
                ClearNodes(itemRoot);
                using (new WaitCursor())
                {
                    var listFolders = currSettings.ChannelSettings;
                    if (listFolders != null)
                    {
                        foreach (var folder in listFolders)
                            AddTreeItem(folder, itemRoot);
                    }
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }
        }
        internal TreeListNode AddTreeItem(Object tag, TreeListNode parent)
        {
            if (bDisposed)
                return null;

            if (!parent.IsExpanded)
            {
                parent.IsExpanded = true;
                var list = (from p in parent.Nodes where p.Tag == tag select p).ToList();
                if (list.Count > 0)
                    return list[0];
            }

            bool bKnownType = false;

            var ic = new TreeItemControl(tag);
            var newitem = treeListControl.AddNode(ic, parent, tag);

            //contentToNodeMap.Add(ic, newitem);

            //if (mapObjectToOldParent.ContainsKey(tag))
            //{
            //    var oldparent = mapObjectToOldParent[tag];
            //    mapObjectToOldParent.Remove(tag);

            //    var keys = mapObjectToParent.Keys.ToList();
            //    foreach (var key in keys)
            //    {
            //        if (mapObjectToParent[key] == oldparent)
            //            mapObjectToParent[key] = newitem;
            //    }
            //}

            if (tag is ChannelSettings)
            {
                bKnownType = true;

                var folder = tag as ChannelSettings;

                SetBindingOnProp(newitem, folder, "Name");
                (newitem.Content as TreeItemControl).ResourceIcon = closedFolderImg;

                treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);
            }
            else if (tag is StationSettings)
            {
                bKnownType = true;

                var folder = tag as StationSettings;

                SetBindingOnProp(newitem, folder, "Name");
                (newitem.Content as TreeItemControl).ResourceIcon = closedFolderImg;
            }

            treeListControl.RefreshRow(newitem.RowHandle); //Otherwise node's header (ItemHeader) can disappear in certain conditions
            return bKnownType ? newitem : null;
        }

        void UpdateFolderIcon(TreeListNode node, bool isOpen)
        {
            if (node != null && (node == itemRoot || node.Tag is ChannelSettings || node.Tag is StationSettings))
                (node.Content as TreeItemControl).ResourceIcon = isOpen ? openFolderImg : closedFolderImg;
        }

        void SetBindingOnProp(TreeListNode node, object bindingSource, string propName, DependencyProperty targetDP = null)
        {
            if (targetDP == null)
                targetDP = TreeItemControl.ItemHeaderProperty;
            var myBinding = new Binding(propName);
            myBinding.Source = bindingSource;
            myBinding.Mode = BindingMode.OneWay;
            myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //myBinding.Converter = new TreeItemHeaderConverter(treeListControl, node);
            BindingOperations.SetBinding(node.Content as TreeItemControl, targetDP, myBinding);
        }

        private static bool CanBeExpanded(TreeListNode parent)
        {
            return parent.Nodes.Count == 1 && parent.Nodes[0].Tag == TreeListControlHelper.DummyNode;
        }

        void OnTreeNodeCollapsing(object sender, TreeListNodeAllowEventArgs e)
        {
            if (!(e.Node.Content as TreeItemControl).IsNodeExpanding)
                UpdateFolderIcon(e.Node, false);
        }

        void OnTreeNodeExpanding(object sender, TreeListNodeAllowEventArgs e)
        {
            TreeListNode item = e.Node;
            try
            {
                UpdateFolderIcon(item, true);
                if (item == null || TreeListControlHelper.WasExpanded(item) ||
                    e != null && !CanBeExpanded(item))
                    return;

                (item.Content as TreeItemControl).IsNodeExpanding = true;
                treeListControl.BeginDataUpdate();

                ClearNodes(item);
                if (item.Tag is ChannelSettings)
                    FillItems(item, item.Tag as ChannelSettings);

                treeListControl.EndDataUpdate();
                e.Handled = true;
            }
            finally
            {
                (item.Content as TreeItemControl).IsNodeExpanding = false;
            }
        }

        private void FillItems(TreeListNode itemRoot, ChannelSettings root = null)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                ClearNodes(itemRoot);
                using (new WaitCursor())
                {
                    var listFolders = (from c in currSettings.StationSettings where c.Channel == root.Name select c).ToList();
                    if (listFolders != null)
                    {
                        foreach (var folder in listFolders)
                            AddTreeItem(folder, itemRoot);
                    }
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }
        }

        private void ChannelAdd_Click(object sender, RoutedEventArgs e)
        {
            EventHandler<RoutedEventArgs> temp = AddChannelEv;
            if (temp != null)
                temp(sender, e);
            UpdateNrChannelsAndStations();
        }

        private void StationAdd_Click(object sender, RoutedEventArgs e)
        {
            EventHandler<RoutedEventArgs> temp = AddStationEv;
            if (temp != null)
                temp(sender, e);
            UpdateNrChannelsAndStations();
        }

        private void ChannelDelete_Click(object sender, RoutedEventArgs e)
        {
            var chList = GetSelecteChannels();
            var stlist = GetSelecteStations();
            if (chList.Count > 0)
            {                                       
                var listStations = (from c in currSettings.StationSettings where  chList.Any(s => s.Name == c.Channel) select c).ToList();
                //I also add stations that have been selected but do not belong to have selected channels.
                var listStationsWithoutChannelSelect = stlist.Except(listStations).ToList();
                listStations.AddRange(listStationsWithoutChannelSelect);

                MessageBoxResult res = MessageBoxResult.No;
                if ((listStations.Count > 0) && (chList.Count > 1))
                    res = MessageBox.Show(string.Format(Properties.Resources.AskDeleteChannelsAndStations, 
                        chList.Count, listStations.Count), Properties.Resources.CaptionDeleteChannel, MessageBoxButton.YesNo);
                else if ((listStations.Count > 0) && (chList.Count == 1))
                    res = MessageBox.Show(string.Format(Properties.Resources.AskDeleteChannelAndStations, 
                        chList[0].Name, listStations.Count), Properties.Resources.CaptionDeleteChannel, MessageBoxButton.YesNo);
                else if (chList.Count > 1)
                    res = MessageBox.Show(string.Format(Properties.Resources.AskDeleteChannels, chList.Count),
                        Properties.Resources.CaptionDeleteChannel, MessageBoxButton.YesNo);
                else
                    res = MessageBox.Show(string.Format(Properties.Resources.AskDeleteChannel, chList[0].Name),
                        Properties.Resources.CaptionDeleteChannel, MessageBoxButton.YesNo);

                if (res == MessageBoxResult.Yes)
                {
                    using (new WaitCursor())
                    {
                        while (listStations.Count > 0)
                        {
                            currSettings.StationSettings.Remove(listStations[0]);
                            listStations[0].Delete();
                            listStations.RemoveAt(0);
                        }
                        while (chList.Count > 0)
                        {
                            currSettings.ChannelSettings.Remove(chList[0]);
                            chList[0].Delete();
                            chList.RemoveAt(0);
                        }
                        DeleteSelectedItem();
                    }
                }
            }
            else if (stlist.Count > 0 )
            {
                MessageBoxResult res = MessageBoxResult.No;
                
                if (stlist.Count > 1)
                    res = MessageBox.Show(string.Format(Properties.Resources.AskDeleteStations, stlist.Count), 
                        Properties.Resources.CaptionDeleteChannel, MessageBoxButton.YesNo);
                else
                    res = MessageBox.Show(string.Format(Properties.Resources.AskDeleteStation, stlist[0].Name),
                    Properties.Resources.CaptionDeleteStation, MessageBoxButton.YesNo);

                if (res == MessageBoxResult.Yes)
                {
                    using (new WaitCursor())
                    {
                        while (stlist.Count > 0)
                        {
                            currSettings.StationSettings.Remove(stlist[0]);
                            stlist[0].Delete();
                            stlist.RemoveAt(0);
                        }
                        DeleteSelectedItem();
                    }
                }
            }
            UpdateNrChannelsAndStations();            
        }

        private void ChannelEdit_Click(object sender, RoutedEventArgs e)
        {
            String name = null;
            var channel = GetSelecteItem() as ChannelSettings;
            if (channel != null)
                name = channel.Name;

            EventHandler<RoutedEventArgs> temp = EditEv;
            if (temp != null)
                temp(sender, e);

            if (channel != null && name != null)
                UpdateStationChannel(name, channel.Name);

        }

        private void TestComm_Click(object sender, RoutedEventArgs e)
        {
            EventHandler<RoutedEventArgs> temp = TestCommEv;
            if (temp != null)
                temp(sender, e);
        }

        private void UpdateStationChannel(string oldname, string newname)
        {
            if (oldname != newname)
            {
                MessageBox.Show(string.Format(Properties.Resources.ChannelNameChanged, oldname, newname));
                var listStations = (from c in currSettings.StationSettings where c.Channel == oldname select c).ToList();
                if (listStations != null)
                {
                    foreach (var station in listStations)
                    {
                        station.Channel = newname;
                    }
                }
            }
        }

        private void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            e.Handled = true;
            String name = null;
            var channel = GetSelecteItem() as ChannelSettings;
            if (channel != null)
                name = channel.Name;

            EventHandler<MouseButtonEventArgs> temp = TreeDblClickEv;
            if (temp != null)
                temp(sender, null);

            if (channel != null && name != null)
                UpdateStationChannel(name, channel.Name);
        }

        private void TreeListControl_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            // enabled/disable interface elements for driver that manage 1 station for channel
            if (SingleStationForChannel)
            {                
                if (GetSelecteItem() is ChannelSettings)
                {
                    // disable add station if at least 1 station was added
                    var stations = (from c in currSettings.StationSettings where c.Channel == ((ChannelSettings)GetSelecteItem()).Name select c).ToList();
                    if (stations != null)
                        _CanAddNewStationEvaluated = (stations.Count == 0);
                }
                else
                    _CanAddNewStationEvaluated = false;
            }
            else
                _CanAddNewStationEvaluated = (GetSelecteItem() is ChannelSettings || GetSelecteItem() is StationSettings);

            UpdateNrChannelsAndStations();
        }

        /// <summary>
        /// Update the number of channels and stations within the UI according to the selected object
        /// </summary>
        private void UpdateNrChannelsAndStations()
        {
            object selItem = GetSelecteItem();
            if (selItem == null)    // channels + stations
            {
                lblChannels.Visibility = Visibility.Visible;
                lblNrChannels.Visibility = Visibility.Visible;
                lblStations.Visibility = Visibility.Visible;
                lblNrStations.Visibility = Visibility.Visible;
                lblNrChannels.Text = currSettings.ChannelSettings.Count.ToString();
                lblNrStations.Text = currSettings.StationSettings.Count.ToString();
            } 
            else if (selItem is ChannelSettings)    // stations of channel
            { 
                lblChannels.Visibility = Visibility.Collapsed;
                lblNrChannels.Visibility = Visibility.Collapsed;
                lblStations.Visibility = Visibility.Visible;
                lblNrStations.Visibility = Visibility.Visible;
                var stations = (from s in currSettings.StationSettings where s.Channel == ((ChannelSettings)GetSelecteItem()).Name select s).ToList();
                lblNrStations.Text = (stations != null ? stations.Count : 0).ToString();
            }
            else if (selItem is StationSettings)    // none
            {
                lblChannels.Visibility = Visibility.Collapsed;
                lblNrChannels.Visibility = Visibility.Collapsed;
                lblStations.Visibility = Visibility.Hidden;
                lblNrStations.Visibility = Visibility.Hidden;
            }
        }

        #endregion

        #region Commands
        private void CanCommandCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = ((GetSelecteItem() is ChannelSettings) || (GetSelecteItem() is StationSettings));
        }

        private void OnCommandCopy(object sender, ExecutedRoutedEventArgs e)
        {
            CopySelectedToClipboard();
            CopyInMemoryDataToWinClipboard();
            CleanClipbaord();
        }

        private void CanCommandCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = (GetSelecteItem() is StationSettings);
        }

        private void OnCommandCut(object sender, ExecutedRoutedEventArgs e)
        {
            OnCommandCopy(sender, e);                
            var stlist = (from s in new XPQuery<StationSettings>(uowClipboard).AsParallel() select s).ToList();
            while (stlist.Count > 0)
            {
                currSettings.StationSettings.Remove(stlist[0]);
                stlist[0].Delete();
                stlist.RemoveAt(0);
            }
            DeleteSelectedItem();
            UpdateNrChannelsAndStations();
            CleanClipbaord();
        }               

        private void CanCommandPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            if (!alreadyLoaded)
            {
                e.CanExecute = false;
                return;
            }

            CopyWinClipboardToInMemoryData(true);

            var channelstocount = (from c in new XPQuery<ChannelSettings>(uowClipboard).AsParallel() select c).Count();
            var stationstocount = (from s in new XPQuery<StationSettings>(uowClipboard).AsParallel() select s).Count();

            e.CanExecute = ((channelstocount + stationstocount) > 0);

            if (!e.CanExecute)
                return;

            var currentItem = GetSelecteItem();
            // root
            if (currentItem == null)
            {
                e.CanExecute = channelstocount > 0;
            }
            else if (currentItem is ChannelSettings || currentItem is StationSettings)
            {
                if (channelstocount == 0 && (stationstocount > 0)) // copy stations only
                {
                    if (SingleStationForChannel)
                    {
                        string channelName = (currentItem is ChannelSettings ? (currentItem as ChannelSettings).Name : (currentItem as StationSettings).Channel);
                        e.CanExecute = (configurationEditor.GetStationList(channelName).Count() == 0);
                    }
                }
                else
                {       
                    // copy channel
                    e.CanExecute = (channelstocount > 0);
                }
            }            
        }

        private void OnCommandPaste(object sender, ExecutedRoutedEventArgs e)
        {
            if (!alreadyLoaded)
                return;

            e.Handled = true;
            CopyWinClipboardToInMemoryData(true);
            PasteFromClipboard();
        }


        private void OnRemoveItem(object sender, ExecutedRoutedEventArgs e)
        {
            ChannelDelete_Click(sender, e);
        }

        private void CanRemoveItem(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = ((GetSelecteItem() is ChannelSettings) || (GetSelecteItem() is StationSettings));
        }

        private void OnAddNewStation(object sender, ExecutedRoutedEventArgs e)
        {
            StationAdd_Click(sender, e);
        }

        private void CanAddNewStation(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;

            if (_CanAddNewStationEvaluated.HasValue)
            {
                e.CanExecute = _CanAddNewStationEvaluated.Value;
                if (e.CanExecute)
                    btnAddStation.ToolTip = null;
                else
                    btnAddStation.ToolTip = Properties.Resources.NoMoreStationsCanBeAddedToChannel;
            }
        }

        private void OnAddNewChannel(object sender, ExecutedRoutedEventArgs e)
        {
            ChannelAdd_Click(sender, e);
        }

        private void CanAddNewChannel(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = true;
        }

        private void OnCommandProperties(object sender, ExecutedRoutedEventArgs e)
        {
            ChannelEdit_Click(sender, e);
        }

        private void CanCommandProperties(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = (GetSelecteItem() is ChannelSettings || GetSelecteItem() is StationSettings);
        }

        private void OnTestComm(object sender, ExecutedRoutedEventArgs e)
        {
            TestComm_Click(sender, e);
        }

        private void CanTestComm(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = (TestCommSupported && (GetSelecteItem() is StationSettings));
        }

        #endregion

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
            var chList = GetSelecteChannels();
            var stlist = GetSelecteStations();
            if (chList.Count == 0 && stlist.Count == 0)
                return;

            CleanClipbaord();

            if (chList.Count > 0)
            {
                foreach (var ch in chList)
                {
                    if (ch is ChannelSettings)
                        CloneToClipboard(ch as ChannelSettings, false);
                }
            }
            else if (stlist.Count > 0)
            {
                foreach (var st in stlist)
                {
                    if (st is StationSettings)
                        CloneToClipboard(st as StationSettings, false);
                }
            }

            uowClipboard.CommitChanges();
        }

        internal void PasteFromClipboard()
        {            
            var channelstocopy = (from c in new XPQuery<ChannelSettings>(uowClipboard).AsParallel() select c).ToList();
            if (channelstocopy.Count > 0)
            {
                foreach (var sourceCh in channelstocopy)
                {
                    var ch = configurationEditor.NewChannel();
                    ch.CopyProperties(sourceCh);
                    ch.DriverSettings = DataContext as DriverSettings;
                    if (ch.DriverSettings != null)
                        ch.Name = ch.DriverSettings.GetNewChannelName();
                    AddChannel(ch);

                    var stationstocopy = (from s in currSettings.StationSettings where s.Channel == sourceCh.Name select s).ToList();
                    foreach (var sourceSt in stationstocopy)
                    {
                        var st = configurationEditor.NewStation();
                        st.CopyProperties(sourceSt);
                        st.DriverSettings = DataContext as DriverSettings;
                        if (st.DriverSettings != null)
                            st.Name = st.DriverSettings.GetNewStationName();
                        st.Channel = ch.Name;
                        AddStation(st);
                    }
                }
            }
            else
            {
                var stationstocopy = (from c in new XPQuery<StationSettings>(uowClipboard).AsParallel() select c).ToList();
                foreach (var sourceSt in stationstocopy)
                {
                    object item = GetSelecteItem() as ChannelSettings;
                    if (item == null)
                    {
                        item = GetSelecteParentItem();
                        if (!(item is ChannelSettings))
                            return;
                    }
                    var channel = item as ChannelSettings;
                    var st = configurationEditor.NewStation();
                    st.CopyProperties(sourceSt);
                    st.DriverSettings = DataContext as DriverSettings;
                    if (st.DriverSettings != null)
                        st.Name = st.DriverSettings.GetNewStationName();
                    st.Channel = channel.Name;
                    AddStation(st);
                }
            }
            UpdateNrChannelsAndStations();
        }
        #endregion


        #region IDisposable Members
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            CleanClipbaord();
            Clipboard.Clear();
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

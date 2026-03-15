using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;
using Utilities.WPF;
using Ookii.Dialogs.Wpf;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpf.Grid;
using WPFUtilities;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Utilities;
using System.Collections.ObjectModel;

namespace DriverCodeBaseEx.UI.Controls
{
    /// <summary>
    /// Interaction logic for BaseImportTree.xaml
    /// </summary>
    public partial class BaseImportTree : UserControl, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly static ImportData DummySubItem = new ImportData();

        public struct GridColData
        {
            public string colName;
            public string bindingName;
            public int colWidth;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="enableMessage"></param>
        public delegate ImportDataModel loadImportfile(string file);

        /// <summary>
        /// 
        /// </summary>
        public loadImportfile LoadImportFile;

        //public event EventHandler<RoutedEventArgs> GetPlcTags;
        public delegate ImportDataModel getPlcTags();

        /// <summary>
        /// 
        /// </summary>
        public getPlcTags GetPlcTags;

        public delegate void addTrueChildren(ObservableCollection<ImportData> children, ImportData root);
        public addTrueChildren AddTrueChildren;

        public event EventHandler<RoutedEventArgs> StationChanged;

        public event EventHandler<RoutedEventArgs> ImpUpdate;

        public delegate ImportDataTreeItemControl createItemControl(object tag);
        public createItemControl CreateItemControl;
        private bool importButtonPlcState;
        private bool importButtonFileState;
        private uint importSortId = 0;

        public string ReadFolderName()
        {
            return DKPFolderCommandVariable.uriLabel.Text.Length != 0 ? DKPFolderCommandVariable.uriLabel.Text : CmbStation.Text;
        }

        public void SetButtonEnable(eImportButtons btn, bool bEnable)
        {
            switch (btn)
            {
                case eImportButtons.eiBtnLoadFromFile:
                    btnOpen.IsEnabled = bEnable;
                    break;
                case eImportButtons.eiBtnLoadFromDevice:
                    btnGetPLCTags.IsEnabled = bEnable;
                    break;
                case eImportButtons.eiBtnSelAll:
                    btnSelAll.IsEnabled = bEnable;
                    break;
                case eImportButtons.eiBtnClear:
                    btnClear.IsEnabled = bEnable;
                    break;
                case eImportButtons.eiBtnImport:
                    btnImport.IsEnabled = bEnable;
                    break;
                case eImportButtons.eiBtnUpdateSymbol:
                    btnUpdateSymbol.IsEnabled = bEnable;
                    break;
            }
        }

        public void ReloadInputFile()
        {
            if (CmbStation.Text.Length == 0 || String.IsNullOrEmpty(file))
            {
                return;
            }

            LoadFile();
        }

        public List<ImportData> GetSelectedTags()
        {
            List<ImportData> list = new List<ImportData>();
            foreach (object tvi in ImportTreeListControl.SelectedItems)
            {
                if (tvi as TreeItemControl != null)
                {
                    ImportData elem = ((TreeItemControl)tvi).TreeItemInnerObject as ImportData;
                    if (elem != null)
                        list.Add(elem);
                }
            }
            return list;
        }

        public String ReadStationName()
        {
            //return CmbStation.Text --> will not be updated immediatly when OnChangedEvent being raised
            if (CmbStation.SelectedValue == null)
                return string.Empty;
            else
                return CmbStation.SelectedValue.ToString();
        }

        public bool HasStation(string stationName)
        {
            return stationList.ContainsKey(stationName);
        }

        public StationSettings GetStationSettings(string stationName)
        {
            if (stationList.ContainsKey(stationName))
                return stationList[stationName];
            else
                return null;
        }

        public Dictionary<string, StationSettings> GetStationSettingsList()
        {
            return stationList;
        }

        public bool HasChannel(string channelName)
        {
            return channelList.ContainsKey(channelName);
        }

        public ChannelSettings GetChannelSettings(string channelName)
        {
            if (channelList.ContainsKey(channelName))
                return channelList[channelName];
            else
                return null;
        }

        public ChannelSettings GetChannelSettingsFromStation(string stationName)
        {            
            StationSettings station = GetStationSettings(stationName);
            if (station != null)
                return GetChannelSettings(station.Channel);
            else
                return null;
        }

        public Dictionary<string, ChannelSettings> GeChannelSettingsList()
        {
            return channelList;
        }

        GetStationName inGetStationName;
        public GetStationName funcGetStationName()
        {
            inGetStationName = () => AddStationName.IsChecked == true ? CmbStation.Text + "_" : "";
            return inGetStationName;
        }

        public int GetBehaviorForExistingTags()
        {
            return (cmbBehaviorExistingTag.SelectedIndex);
        }

        public int GetBehaviorForDynamicLink()
        {
            return (cmbBehaviorDynamicLink.SelectedIndex);
        }

        string fileFilter = "csv U files|*.csv";
        string file = String.Empty;
        bool alreadyLoaded = false;
        /// <summary>   List of stations. </summary>
        Dictionary<string, StationSettings> stationList;
        /// <summary>   List of stations. </summary>
        Dictionary<string, ChannelSettings> channelList;
        /// <summary>   The connection. </summary>
        string conn;
        /// <summary>   true to protect. </summary>
        bool protect;
        /// <summary>   The idl. </summary>
        IDataLayer idl = null;
        /// <summary>   The file base. </summary>
        string fileBase;
        /// <summary>   The in memory. </summary>
        InMemoryDataStore InMemory = null;
        string DriverName;

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseImportTree(string driverName, string connection, bool bProtect, List<GridColData> cols)
        {
            InitializeComponent();

            conn = connection;
            protect = bProtect;
            DriverName = driverName;
            // list of stations/channel must be load immeadiatly; many drivers use these during load event
            FillChannelStationList(ref channelList, ref stationList);
            Loaded += (o, e) =>
            {
                if (alreadyLoaded)
                    return;
                alreadyLoaded = true;

                InitializeImportTree();
                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(CheckBox.IsCheckedProperty, typeof(CheckBox));
                descriptor.AddValueChanged(AddStationName, ImportTree_Changed);
                descriptor =
                  DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(CmbStation, ImportTree_Changed);

                //FillChannelStationList(ref channelList, ref stationList);
                CmbStation.ItemsSource = (from s in stationList orderby s.Value.Name select s.Value.Name).ToList();
                CmbStation.SelectedIndex = 0;

                cmbBehaviorExistingTag.ItemsSource = new Dictionary<int, string>()
                {
                    {(int)UFUAModel.BehaviorExistingTagsValues.Ignore, Properties.Resources.ImportIgnore},
                    {(int)UFUAModel.BehaviorExistingTagsValues.Overwrite, Properties.Resources.ImportOverwrite},
                    {(int)UFUAModel.BehaviorExistingTagsValues.CreateNew, Properties.Resources.ImportCreateNew},
                };
                cmbBehaviorExistingTag.SelectedIndex = 1;

                cmbBehaviorDynamicLink.ItemsSource = new Dictionary<int, string>()
                {
                    {(int)UFUAModel.BehaviorDynamicLinkValues.AddToExisting, Properties.Resources.ImportAddToExisting},
                    {(int)UFUAModel.BehaviorDynamicLinkValues.ReplaceExisting, Properties.Resources.ImportReplaceExisting},
                };
                cmbBehaviorDynamicLink.SelectedIndex = 1;

                DependencyPropertyDescriptor descriptorBehavior =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(cmbBehaviorExistingTag, cmbBehaviorExistingTag_TextChanged);

                cmbBehaviorExistingTag_TextChanged();
            };

            if (cols.Count > 0)
            {
                // add a column allow to restore import original order
                cols.Insert(0, new GridColData() { colName = Properties.Resources.ImportTreeCoumnId, bindingName = "ImportSortId", colWidth = 50 });
                for (int i = 0; i < cols.Count; i++)
                {
                    TreeListColumn newColumn = new TreeListColumn()
                    {
                        Header = cols[i].colName,
                        FilterPopupMode = FilterPopupMode.Excel,
                        FieldName = cols[i].bindingName,
                        FixedWidth = true,
                        Width = cols[i].colWidth,
                    };
                    ImportTreeListControl.Columns.Add(newColumn);
                }
            }
        }

        #region Methods
        private void InitializeImportTree()
        {
            treeListView.Nodes.Clear();

            var rt = new ImportDataTreeItemControl(rootName, GetBitmapImageSourceRoot());
            rt.TagName = rootName;
            itemRoot = ImportTreeListControl.AddNode(rt);
            ImportTreeListControl.AddNode(null, itemRoot, TreeListControlHelper.DummyNode);
        }

        private void FillChannelStationList(ref Dictionary<string, ChannelSettings> listChannel, ref Dictionary<string, StationSettings> listStation)
        {

            listChannel = new Dictionary<string, ChannelSettings>();
            listStation = new Dictionary<string, StationSettings>();

            idl = DriverCodeBaseEx.CommunicationDriver.GetDriverDataLayer(conn, DriverName);

            if (idl != null) {
                using (var ufw = new UnitOfWork(idl))
                {
                    DriverCodeBaseEx.CommunicationDriver.UpdateDriverSchema(ufw);
                    DriverSettings configuration = null;
                    try
                    {
                        configuration = (from tag in new XPQuery<DriverSettings>(ufw).AsParallel() where tag.ClassInfo.AssemblyName.ToLower() == DriverName.ToLower() select tag).Single();
                    }
                    catch (Exception ex)
                    {
                    }
                    if ((configuration != null) && (configuration.StationSettings.Count > 0))
                    {
                        listStation = (from s in configuration.StationSettings.AsParallel()
                                        orderby s.Name
                                        select s).ToDictionary(k => k.Name, v => v);
                    }

                    if ((configuration != null) && (configuration.ChannelSettings.Count > 0))
                    {
                        listChannel = (from c in configuration.ChannelSettings.AsParallel()
                                orderby c.Name
                                select c).ToDictionary(k => k.Name, v => v);
                    }
                }
            }
        }
        public void SetFileFilter(string filter)
        {
            fileFilter = filter;
        }
        public void SetVisibleButtons(bool bLoad = true, bool bGetPLCTags = false, bool bSelectAll = true, bool bSelectNone = true,
            bool bImportTags = true, bool bUpdateSymbol = false)
        {
            if (!bLoad)
                ButtonGrid.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Pixel);
            if (!bGetPLCTags)
                ButtonGrid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Pixel);
            if (!bSelectAll)
                ButtonGrid.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Pixel);
            if (!bSelectNone)
                ButtonGrid.ColumnDefinitions[3].Width = new GridLength(0, GridUnitType.Pixel);
            if (!bImportTags)
                ButtonGrid.ColumnDefinitions[4].Width = new GridLength(0, GridUnitType.Pixel);
            if (!bUpdateSymbol)
                ButtonGrid.ColumnDefinitions[5].Width = new GridLength(0, GridUnitType.Pixel);
        }

        public void SetVisibleTitle(bool bVisblie = true)
        {
            if (bVisblie)
                txtImportTreeTitle.Visibility = Visibility.Visible;
            else
                txtImportTreeTitle.Visibility = Visibility.Collapsed;
        }

        private void ResetFilter()
        {
            ImportTreeListControl.FilterString = string.Empty;
        }

        #endregion


        private void ImportTree_Changed(object sender, EventArgs e)
        {
            //ImportTreeListControl.Refresh();
            UpdateDataWithInterfaceParameters();
        }

        private void UpdateAllColumnsWithInterfaceParameters(TreeListNodeCollection nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                TreeListNode n = nodes[i];

                // if node is a tag, update all visibile property
                if (n.Content as ImportDataTreeItemControl != null)
                    ((ImportDataTreeItemControl)n.Content).UpdateAllColumns();

                // traverse sub node
                if (n.HasChildren)
                    UpdateAllColumnsWithInterfaceParameters(n.Nodes);
            }
        }

        private void cmbBehaviorExistingTag_TextChanged(object sender, EventArgs e)
        {
            cmbBehaviorExistingTag_TextChanged();
        }

        private void cmbBehaviorExistingTag_TextChanged()
        {
            if(cmbBehaviorExistingTag.SelectedIndex == (int)UFUAModel.BehaviorExistingTagsValues.Overwrite)
            {
                txtBehaviorDynamicLink.IsEnabled = true;
                txtBehaviorDynamicLink.Opacity = 1;
                cmbBehaviorDynamicLink.IsEnabled = true;
            }
            else
            {
                txtBehaviorDynamicLink.IsEnabled = false;
                txtBehaviorDynamicLink.Opacity = 0.5;
                cmbBehaviorDynamicLink.IsEnabled = false;
                cmbBehaviorDynamicLink.SelectedIndex = (int)UFUAModel.BehaviorDynamicLinkValues.ReplaceExisting;
            }
        }

        /// <summary>
        /// Refresh imported tag with user's interface parameters (Add station Name, ecc)
        /// </summary>
        public void UpdateDataWithInterfaceParameters()
        {
            UpdateAllColumnsWithInterfaceParameters(ImportTreeListControl.View.Nodes);

            ImportTreeListControl.Refresh();
        }

        private void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            if (CmbStation.Text.Length == 0)
            {
                MessageBox.Show(Properties.Resources.ImportTreeEnterStationName,
                                Properties.Resources.ImportTreeMsgBoxTitle);
                return;
            }
            if (Environment.UserInteractive)
            {
                VistaOpenFileDialog dialog = new VistaOpenFileDialog();
                dialog.Filter = fileFilter;
                if (dialog.ShowDialog() != true)
                    file = String.Empty;
                file = dialog.FileName;
            }
            if (String.IsNullOrEmpty(file))
                return;

            // remove filter from grid
            ResetFilter();

            LoadFile();
        }

        TreeListNode itemRoot;
        readonly static String rootName = "Root";

        public void SetImportDataModel(ImportDataModel idm)
        {
            importSortId = 0;
            if (idm != null && idm.Children.Count != 0)
            {
                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    if (bDisposed)
                        return;

                    FillItems(idm.Children);
                    itemRoot.IsExpanded = true;
                    UnLockImportButtons();
                });
            }
            else
            {
                // if no data (or import failed), clear all nodes and reinit tree control
                InitializeImportTree();
                UnLockImportButtons();
            }
        }

        void ClearNodes(TreeListNode node)
        {
            node.Nodes.Clear();
        }

        public static bool IsDummyChild(ObservableCollection<ImportData> children)
        {
            bool returnValue = false;
            if ((children != null) && (children.Count == 1) && (children[0] == DummySubItem))
            {
                returnValue = true;
            }
            return returnValue;
        }

        private void FillItems(ObservableCollection<ImportData> children, TreeListNode root = null)
        {
            if (root == null)
                root = itemRoot;
            ImportTreeListControl.BeginDataUpdate();
            try
            {
                importSortId++;
                ClearNodes(root);
                using (new WaitCursor())
                {
                    if(IsDummyChild(children) && (AddTrueChildren != null))
                    {
                        AddTrueChildren(children, root.Tag as ImportData);
                    }
                    foreach (var ch in children)
                    {
                        AddTreeItem(ch, root);
                    }
                }
            }
            finally
            {
                ImportTreeListControl.EndDataUpdate();
            }
            
        }

        private BitmapImage GetBitmapImageSourceRoot()
        {
            return SharedResources.Helpers.ResourceManager.GetCommonImage("SharedImages", "CloseFolderSmall", true);
        }

        private BitmapImage GetBitmapImageSourceImportData(ImportData tagImp)
        {
            if (tagImp.ArrayDimension == 0 && tagImp.Children.Count > 0)
            {
                return SharedResources.Helpers.ResourceManager.GetCommonImage("UFUAServer", "UFUASVariableType");
            }
            else
            {
                switch (tagImp.IconTagType)
                {
                    case UFUAModel.DataType.Boolean:
                        return SharedResources.Helpers.ResourceManager.GetCommonImage("UFUAServer", "UFUASVariableBooleanSmall");
                    case UFUAModel.DataType.SByte:
                        return SharedResources.Helpers.ResourceManager.GetCommonImage("UFUAServer", "UFUASVariableSByteSmall");
                    case UFUAModel.DataType.Byte:
                        return SharedResources.Helpers.ResourceManager.GetCommonImage("UFUAServer", "UFUASVariableByteSmall");
                    case UFUAModel.DataType.Int16:
                        return SharedResources.Helpers.ResourceManager.GetCommonImage("UFUAServer", "UFUASVariableInt16Small");
                    case UFUAModel.DataType.UInt16:
                        return SharedResources.Helpers.ResourceManager.GetCommonImage("UFUAServer", "UFUASVariableUInt16Small");
                    case UFUAModel.DataType.Int32:
                        return SharedResources.Helpers.ResourceManager.GetCommonImage("UFUAServer", "UFUASVariableInt32Small");
                    case UFUAModel.DataType.UInt32:
                        return SharedResources.Helpers.ResourceManager.GetCommonImage("UFUAServer", "UFUASVariableUInt32Small");
                    case UFUAModel.DataType.Int64:
                        return SharedResources.Helpers.ResourceManager.GetCommonImage("UFUAServer", "UFUASVariableInt64Small");
                    case UFUAModel.DataType.UInt64:
                        return SharedResources.Helpers.ResourceManager.GetCommonImage("UFUAServer", "UFUASVariableUInt64Small");
                    case UFUAModel.DataType.Float:
                        return SharedResources.Helpers.ResourceManager.GetCommonImage("UFUAServer", "UFUASVariableFloatSmall");
                    case UFUAModel.DataType.Double:
                        return SharedResources.Helpers.ResourceManager.GetCommonImage("UFUAServer", "UFUASVariableDoubleSmall");
                    case UFUAModel.DataType.String:
                        return SharedResources.Helpers.ResourceManager.GetCommonImage("UFUAServer", "UFUASVariableStringSmall");
                    default:
                        return null;
                }
            }            
        }
        private ImportDataTreeItemControl CreateImportDataTreeItemControl(object tag)
        {
            // if customized by driver
            if (CreateItemControl != null)
                return CreateItemControl(tag);
            else // default
                return new ImportDataTreeItemControl(tag);
        }

        internal TreeListNode AddTreeItem(Object tag, TreeListNode parent)
        {
            TreeListNode newItem = null;

            if (bDisposed)
                return newItem;

            //if (parent == null)
            //    parent = itemRoot;

            if (!parent.IsExpanded)
            {
                parent.IsExpanded = true;
                var list = (from p in parent.Nodes where p.Tag == tag select p).ToList();
                if (list.Count > 0)
                    return list[0];
            }

            importSortId++;
            ImportData idTag = tag as ImportData;
            idTag.ImportSortId = importSortId;
            if (idTag != null)
            {
                var ic = CreateImportDataTreeItemControl(tag);
                if (ic != null)
                {
                    if (ic.ResourceIcon == null)
                        ic.ResourceIcon = GetBitmapImageSourceImportData(idTag);
                    newItem = ImportTreeListControl.AddNode(ic, parent, idTag);
                    if (idTag.Children.Count > 0)
                        ImportTreeListControl.AddNode(null, newItem, TreeListControlHelper.DummyNode);

                    //    //SetBindingOnProp(newitem, tag, nameof(n.Name));
                    //    //SetBindingOnProp(newitem, tag, nameof(n.DynAddress));
                    //    //SetBindingOnProp(newitem, tag, nameof(n.szType));

                    //ImportTreeListControl.RefreshRow(newItem.RowHandle); //Otherwise node's header (ItemHeader) can disappear in certain conditions
                }
            }

            return newItem;
        }

        //void SetBindingOnProp(TreeListNode node, object bindingSource, string propName, DependencyProperty targetDP = null)
        //{
        //    if (targetDP == null)
        //        targetDP = TreeItemControl.ItemHeaderProperty;
        //    var myBinding = new Binding(propName);
        //    myBinding.Source = bindingSource;
        //    myBinding.Mode = BindingMode.OneWay;
        //    myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        //    BindingOperations.SetBinding(node.Content as TreeItemControl, targetDP, myBinding);
        //}
        private void LoadFile()
        {
            if (LoadImportFile == null)
                return;

            LockImportButtons();
            var importDataModel = LoadImportFile(file);
            SetImportDataModel(importDataModel);
        }
        private void SelAll_Click(object sender, RoutedEventArgs e)
        {
            ImportTreeListControl.SelectAll(); 
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ImportTreeListControl.UnselectAll();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            if (CmbStation.Text.Length == 0)
            {
                MessageBox.Show(Properties.Resources.ImportTreeEnterStationName,
                                Properties.Resources.ImportTreeMsgBoxTitle);
                return;
            }

            var wnd = this.FindParent<Window>();
            if (wnd != null)
            {
                wnd.DialogResult = true;
                wnd.Close();
            }
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            EventHandler<RoutedEventArgs> temp = ImpUpdate;
            if (temp != null)
                temp(sender, e);
        }

        bool cmbStationSelectionHandled = true;
        private void CmbStation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {           
            if (cmbStationSelectionHandled)
            {
                // if any tags were imported before
                if (ImportTreeListControl.View.Nodes.Count > 0 && ImportTreeListControl.View.Nodes[0].Nodes.Count > 1)
                {
                    // request to user if want to change station and lose all data or remain 
                    if (MessageBox.Show(Properties.Resources.AskToChangeStationImporter, Properties.Resources.ImportTreeMsgBoxTitle, MessageBoxButton.YesNo) == MessageBoxResult.No)
                    {
                        ComboBox combo = (ComboBox)sender;
                        cmbStationSelectionHandled = false;
                        if (e.RemovedItems.Count > 0)
                            combo.SelectedItem = e.RemovedItems[0];
                        return;
                    }
                    ResetFilter();
                    SetImportDataModel(null);
                }

                EventHandler<RoutedEventArgs> temp = StationChanged;
                if (temp != null)
                    temp(sender, e);
            }
            cmbStationSelectionHandled = true;
        }

        private void GetPlc_Click(object sender, RoutedEventArgs e)
        {
            if (CmbStation.Text.Length == 0)
            {
                MessageBox.Show(Properties.Resources.ImportTreeEnterStationName,
                                Properties.Resources.ImportTreeMsgBoxTitle);
                return;
            }

            if (GetPlcTags == null)
                return;

            ResetFilter();
            //EventHandler<RoutedEventArgs> temp = GetPlcTags;
            //if (temp != null)
            //    temp(sender, e);

            LockImportButtons();
            var importDataModel = GetPlcTags();
            SetImportDataModel(importDataModel);
        }

        /// <summary>
        /// Disable import's buttons (from File, PLC) during import
        /// </summary>
        private void LockImportButtons()
        {
            importButtonPlcState = btnGetPLCTags.IsEnabled;
            importButtonFileState = btnOpen.IsEnabled;

            btnGetPLCTags.IsEnabled = false;
            btnOpen.IsEnabled = false;
        }

        /// <summary>
        /// Set import's buttons (from File, PLC) to import's previous state
        /// </summary>
        private void UnLockImportButtons()
        {            
            btnGetPLCTags.IsEnabled = importButtonPlcState;
            btnOpen.IsEnabled = importButtonFileState;
        }
        #region IDisposable Members

        bool bDisposed;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            if(bDisposed)
                return;
            bDisposed = true;

            if (idl != null)
                idl.Dispose();
        }


        #endregion


        private void OnTreeNodeExpanding(object sender, DevExpress.Xpf.Grid.TreeList.TreeListNodeAllowEventArgs e)
        {
            TreeListNode item = e.Node;
            try
            {
                if (item == null || TreeListControlHelper.WasExpanded(item)) //Node's subtree already populated
                    return;

                if (e != null && !TreeListControlHelper.CanBeExpanded(item))
                {
                    return;
                }

                (item.Content as TreeItemControl).IsNodeExpanding = true;
                ImportTreeListControl.BeginDataUpdate();

                if (item.Tag is ImportData)
                {
                    var id = (item.Tag as ImportData);
                    if(id != null && id.Children.Count > 0)
                        FillItems(id.Children, item);
                }
                ImportTreeListControl.EndDataUpdate();
            }
            finally
            {
                (item.Content as TreeItemControl).IsNodeExpanding = false;
            }
        }
    }
}
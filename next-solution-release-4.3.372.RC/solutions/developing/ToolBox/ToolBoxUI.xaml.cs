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
using UFInterfaces;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Windows.Markup;
using Utilities;
using Utilities.WPF;
using UIMsgBoxAlertService.ComponentService;
using Toolbox.ComponentService;
using System.Xml.Linq;
using System.Dynamic;
using WPFUtilities;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid.TreeList;
using System.ComponentModel;
using DevExpress.Data.TreeList;

namespace Toolbox
{
    /// <summary>
    /// Helper class object used in the TreeViewAdvItem.Tag property.
    /// </summary>
    class CategoryTagHelper: INotifyPropertyChanged
    {
        public string FileName { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }

/// <summary>
/// Helper class object used in the TreeViewAdvItem.Tag property.
/// </summary>
internal class ToolControlTagHelper : INotifyPropertyChanged
    {
        public TreeListNode ItemPointer { get; set; }
        public ToolBoxData ToolData { get; set; }
        public string Name { get; set; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }

    /// <summary>
    /// Interaction logic for ToolBoxUI.xaml
    /// </summary>
    public partial class ToolBoxUI : UserControl, IDisposable
    {
        String lastSelectedType;
        BitmapImage openFolderImg;
        BitmapImage closedFolderImg;
        readonly String rootName = "ToolboxObjects";
        readonly Dictionary<TreeListNode, ToolBoxData> mapTreeViewItemsCode = new Dictionary<TreeListNode, ToolBoxData>();
        Dictionary<TreeItemControl, TreeListNode> contentToNodeMap = new Dictionary<TreeItemControl, TreeListNode>();
        readonly Dictionary<String, DXTabItem> mapTabs = new Dictionary<String, DXTabItem>();
        readonly Dictionary<String, String> mapTabSubType = new Dictionary<String, String>();
        readonly ToolboxComponent toolBoxComponent;
        readonly Dictionary<String, String> mapItems = new Dictionary<String, String>();
        readonly IEnumerable<string> tables = new string[1] {"Toolbox" };
        bool bLoaded;

        public ToolBoxUI(ToolboxComponent c)
        {
            InitializeComponent();
            openFolderImg = ToolboxComponent.GetControlImage("OpenFolderSmall", true);
            closedFolderImg = ToolboxComponent.GetControlImage("CloseFolderSmall", true);
            if (!bLoaded)
            {
                bLoaded = true;

                mapItems.Clear();
                contentToNodeMap.Clear();
                string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
                string startingPath = String.Format("{0}.{1}\\Cultures\\", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"),mainversion);
                string fileToOpen = string.Empty;

                foreach (string key in tables)
                {
                    fileToOpen = string.Format("{0}{1}\\StringTable_{2}.xml", startingPath, System.Threading.Thread.CurrentThread.CurrentUICulture.Name, key);

                    if (!File.Exists(fileToOpen))
                        fileToOpen = string.Format("{0}StringTable_{1}.xml", startingPath,key);
                    if (File.Exists(fileToOpen))
                    {
                        LoadFromXml(fileToOpen);
                        rootName = mapItems.ContainsKey(rootName) ? mapItems[rootName] : rootName;
                    }
                }
            }
            toolBoxComponent = c;
        }

        void UpdateFolderIcon(TreeListNode node, bool isOpen)
        {
            if (node != null && node.Tag is CategoryTagHelper)
                (node.Content as TreeItemControl).ResourceIcon = isOpen ? openFolderImg : closedFolderImg;
        }

        ToolBoxData lastSet;
        public ToolBoxData ActiveToolCode
        {
            get
            {
                //var groupBar = GetActiveGroupBar();
                var treeview = GetActiveTreeItem();
                if (treeview == null)
                    return lastSet;
                if (treeview.SelectedItem == null)
                    return lastSet;
                if (treeview.SelectedItem is TreeListNode)
                {
                    var toolControl = (treeview.SelectedItem as TreeListNode).Tag as ToolControlTagHelper;
                    if (toolControl != null)
                        return toolControl.ToolData;
                }
                return lastSet;
            }

            set
            {
                lastSet = value;
                //var groupBar = GetActiveGroupBar();
                var treeview = GetActiveTreeItem();
                if (treeview == null)
                    return;

                if (value == null)
                {
                    //var selected = treeview.SelectedItem;
                    //if (selected != null)
                    //{
                    //    var toolControl = (treeview.SelectedItem as TreeListNode).Tag as ToolControlTagHelper;
                    //    if (toolControl != null)
                    //        treeview.SelectedItem = toolControl.ItemPointer;
                    //}
                    //else
                    treeview.ClearSelection();
                    treeview.SelectedItem = null;
                }
                else
                {
                    foreach (KeyValuePair<TreeListNode, ToolBoxData> item in mapTreeViewItemsCode)
                    {
                        if (item.Value == value)
                        {
                            treeview.SelectedItem = item.Key;
                            break;
                        }
                    }
                }

                //if (toolBoxComponent.WorkSpace != null)
                //    toolBoxComponent.WorkSpace.ActivatePreviousActiveElement();
            }
        }

        internal void LoadToolboxItems(String type, FrameworkElement control)
        {
            if (lastSelectedType == type)
                return;
            lastSelectedType = type;
            if (mapTabs.ContainsKey(lastSelectedType))
            {
                var tabItem = mapTabs[lastSelectedType];
                tabItem.IsSelected = true;
                tabControl.SelectedItem = tabItem;
                return;
            }
            var newItem = new DXTabItem() { Header = lastSelectedType, Content = control, IsSelected = true, VerticalContentAlignment = VerticalAlignment.Stretch };
            newItem.InitItemTemplate();
            mapTabs.Add(lastSelectedType, newItem);
            tabControl.Items.Add(newItem);
            tabControl.SelectedItem = newItem;

            txtNoLibrary.Visibility = Visibility.Collapsed;
        }

        internal void LoadToolboxItems(String type, String subType, bool bForce = false)
        {
            if (lastSelectedType == type && !bForce)
                return;
            lastSelectedType = type;
            if (mapTabSubType.ContainsKey(type))
                mapTabSubType.Remove(type);
            mapTabSubType.Add(type, subType);
            using (new WaitCursor())
            {
                if (String.IsNullOrEmpty(type))
                    return;

                LoadSymbols();
            }
        }

        TreeListControl GetActiveTreeItem()
        {
            var tabItem = tabControl.SelectedItem as DXTabItem;
            if (tabItem == null)
                return null;
            var treeview = tabItem.Content as TreeListControl;
            return treeview;
        }

        void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            TreeListControl treeListControl = GetActiveTreeItem();
            TreeListNode node = treeListControl.View.GetNodeByRowHandle(e.HitInfo.RowHandle);
            if (node == null)
                return;

            node.IsExpanded = !node.IsExpanded;
        }

        void treeListControl_SelectionChanged(object sender, TreeListSelectionChangedEventArgs e)
        {
            OnActivate(sender as TreeListControl);
            e.Handled = true;
        }

        internal void OnActivate(TreeListControl treeListControl)
        {
            var listSelected = treeListControl.GetSelectedNodes();
            //treeListControl.SelectedItems.Clear();
            //foreach (TreeListNode newitem in listSelected)
            //{
            //    if (newitem.Tag is ToolControlTagHelper)
            //    {
            //        treeListControl.SelectedItems.Add(newitem);
            //    }
            //}
            //if (listSelected.Count() > 0)
            //{
                treeListControl.SelectedItem = listSelected.FirstOrDefault();
            //}

        }

        void LoadSymbols()
        {
            if (mapTabs.ContainsKey(lastSelectedType))
            {
                var tabItem = mapTabs[lastSelectedType];
                tabItem.IsSelected = true;
                tabControl.SelectedItem = tabItem;
                return;
            }

            LoadToolboxCategories();
        }

        void LoadToolboxCategories()
        {
            var categories = toolBoxComponent.GetListCategories(lastSelectedType, mapTabSubType[lastSelectedType]).ToList();
            if (categories.Count == 0)
                return;

            txtNoLibrary.Visibility = Visibility.Collapsed;

            var treeListControl = TryFindResource("treeListControl") as TreeListControl;
            var currentSkin = Utilities.ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");
            WPFUtilities.ThemeHelper.SetTheme(treeListControl, currentSkin);
            var treeListView = treeListControl.View as TreeListView;
            treeListView.Nodes.Clear();
            contentToNodeMap.Clear();
            //var itemRoot = treeListControl.AddNode(new TreeItemControl(rootName) { ResourceIcon = openFolderImg },null, rootName);
            LoadTreeViewItems(treeListControl, null, lastSelectedType, mapTabSubType[lastSelectedType]);
            var newItem = new DXTabItem() { Header = lastSelectedType, Content = treeListControl, IsSelected = true };
            newItem.InitItemTemplate();
            mapTabs.Add(lastSelectedType, newItem);
            tabControl.Items.Add(newItem);
            tabControl.SelectedItem = newItem;
            //itemRoot.IsExpanded = true;
        }

        private void LoadTreeViewItems(TreeListControl treeListControl, TreeListNode itemRoot, string lastSelectedType, string lastSelectedSubType)
        {
            treeListControl.BeginDataUpdate();
            try
            {
                var categoriesd = toolBoxComponent.GetListCategories(lastSelectedType, lastSelectedSubType, false).ToList();
                foreach (var category in categoriesd)
                {
                    var filename = System.IO.Path.GetFileName(category);
                    var name = mapItems.ContainsKey(filename) ? mapItems[filename] : filename;
                    var tag = new CategoryTagHelper() { FileName = filename, Category = category, Name = name };
                    var itemNode = AddTreeItem(treeListControl, tag, name, itemRoot);
                    UpdateFolderIcon(itemNode, false);
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }
        }

        internal TreeListNode AddTreeItem(TreeListControl treeListControl, Object tag, string header, TreeListNode parent = null)
        {
            if (parent != null && !parent.IsExpanded && !(parent.Content as TreeItemControl).IsNodeExpanding)
            {
                parent.IsExpanded = true;
                var list = (from p in parent.Nodes where p.Tag == tag select p).ToList();
                if (list.Count > 0)
                    return list[0];
            }

            var ic = new TreeItemControl(tag, header);
            var newitem = treeListControl.AddNode(ic, parent, tag);
            contentToNodeMap.Add(ic, newitem);

            if (NeedToBeExpanded(newitem))
                treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);

            if (tag is CategoryTagHelper)
            {
                SetBindingOnProp(newitem, tag, "Name");
            }

            return newitem;
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
            BindingOperations.SetBinding(node.Content as HeaderedItemsControl, targetDP, myBinding);
        }

        bool NeedToBeExpanded(TreeListNode item)
        {
            if (item.Tag is CategoryTagHelper)
            {
               return true;
            }
            return false;
        }

        void OnTreeNodeCollapsing(object sender, TreeListNodeAllowEventArgs e)
        {
            if (!(e.Node.Content as TreeItemControl).IsNodeExpanding)
                UpdateFolderIcon(e.Node, false);
        }

        void OnTreeNodeExpanding(object sender, TreeListNodeAllowEventArgs e)
        {
            if (e == null)
                return;

            var treeListView = sender as TreeListView;
            var treeListControl = treeListView.FindParent<TreeListControl>() as TreeListControl;
            TreeListNode item = e.Node;
            
            if (item == null || TreeListControlHelper.WasExpanded(item)) //Node's subtree already populated
                return;

            //if (!TreeListControlHelper.CanBeExpanded(item))
            //{
            //    if (item.Tag is CategoryTagHelper)
            //        UpdateFolderIcon(item, true);
            //    return;
            //}

            (item.Content as TreeItemControl).IsNodeExpanding = true;
            treeListControl.BeginDataUpdate();

            try
            {

                ClearNodes(item);

                var activeTab = tabControl.SelectedItem as DXTabItem;
                if (activeTab == null)
                    return;
                var type = activeTab.Header as String;
                if (String.IsNullOrEmpty(type))
                    return;
                if (!mapTabSubType.ContainsKey(type))
                    return;
                var lastSelectedSubType = mapTabSubType[type];
                using (var cursor = new WaitCursor())
                {
                    var tagInfo = item.Tag as CategoryTagHelper;
                    var startingPath = ApplicationPropertiesHelper.GetProperty<String>("CommonFolder");
                    string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
                    var folder = String.Format("{0}.{2}\\{1}\\", startingPath, Properties.Settings.Default.ToolBoxFolder, mainversion);
                    var filename = tagInfo.Category.Remove(0, folder.Length);
                    LoadTreeViewItems(treeListControl, item, filename, lastSelectedSubType);
                    var toolDatas = toolBoxComponent.GetListTools(lastSelectedType, lastSelectedSubType, tagInfo.Category).ToList();

                    if (toolDatas.Count > 0)
                    {
                        TreeListNode itempointer = null;// treeListControl.AddNode(new TreeItemControl(null, "Pointer", ToolboxComponent.GetControlImage("TBPointer")), item, null);
                        
                        foreach (var toolData in toolDatas)
                        {
                            // Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                            {
                                var name = mapItems.ContainsKey(toolData.Title) ? mapItems[toolData.Title] : toolData.Title;
                                var tag = new ToolControlTagHelper(){ToolData = toolData,ItemPointer = itempointer,Name = name};
                                var ic = new TreeItemControl(tag, name) { ResourceIcon = toolData.image };
                                var itemTool = treeListControl.AddNode(ic, item, tag);
                                contentToNodeMap.Add(ic, itemTool);

                                mapTreeViewItemsCode.Add(itemTool, toolData);
                            }//);
                        }
                    }
                }

                e.Handled = true;
            }
            finally
            {
                treeListControl.EndDataUpdate();
                (item.Content as TreeItemControl).IsNodeExpanding = false;
            }
        }

        void OnTreeNodeExpanded(object sender, TreeListNodeEventArgs e)
        {
            var item = e.Node;
            if (item != null)
                UpdateFolderIcon(item, item.IsExpanded);
        }

        void ClearNodes(TreeListNode node)
        {
            node.Nodes.Clear();
        }

        void ClearObjectMapNode(TreeListNode node)
        {
            if (node.Content is TreeItemControl && contentToNodeMap.ContainsKey(node.Content as TreeItemControl))
            {
                var content = node.Content as TreeItemControl;
                contentToNodeMap.Remove(content);
            }

            foreach (var child in node.Nodes)
                ClearObjectMapNode(child);
        }

        void OnTreeNodeChanged(object sender, TreeListNodeChangedEventArgs e)
        {
            if (e.ChangeType == NodeChangeType.Remove && e.Node.Tag != null && e.Node.Tag != TreeListControlHelper.DummyNode)
                ClearObjectMapNode(e.Node);
        }

        void MenuItemReset_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var activeTab = tabControl.SelectedItem as DXTabItem;
            if (activeTab == null)
                return;
            var type = activeTab.Header as String;
            if (String.IsNullOrEmpty(type))
                return;
            if (!mapTabSubType.ContainsKey(type))
                return;
            var lastSelectedSubType = mapTabSubType[type];

            LoadToolboxItems(type, lastSelectedSubType);
        }

        internal void ResetToolbox()
        {
            mapTreeViewItemsCode.Clear();
            mapTabs.Clear();
            tabControl.Items.Clear();
            foreach (var key in mapTabSubType.Keys.ToList())
                LoadToolboxItems(key, mapTabSubType[key], true);
        }

        public void Dispose()
        {
            mapTreeViewItemsCode.Clear();
            mapTabs.Clear();
        }
        public void LoadFromXml(string filepath)
        {
            //mapItems.Clear();

            try
            {
                var keyexpandolist = Utilities.XmlHelper.GetExpandoAttributeFromXml(File.ReadAllText(filepath), "resources",true);
                if (keyexpandolist.Count() != 0)
                {
                    keyexpandolist.ToList().ForEach(e =>
                    {
                        var regkeydictionary = e as IDictionary<string, object>;
                        regkeydictionary.ToList().ForEach(r =>
                        {
                            try
                            {
                                mapItems.Add(r.Key.ToString(), r.Value.ToString());
                            }
                            catch
                            {
                            }
                        });
                    });
                }
            }
            catch (Exception e)
            {

            }

        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape)
                return;
            e.Handled = true;
            ActiveToolCode = null;
        }

        #region Drag & Drop

        private void TreeListControl_DragRecordOver(object sender, DragRecordOverEventArgs e)
        {
            if (e == null)
                return;

            var treeListView = sender as TreeListView;
            var treeListControl = treeListView.FindParent<TreeListControl>() as TreeListControl;
            var data = (RecordDragDropData)e.Data.GetData(typeof(RecordDragDropData));
            if (data.Records != null && data.Records.Count() > 0)
            {
                var node = treeListControl.View.GetNodeByContent(data.Records[0]);
                var parentNode = node.ParentNode;

                if (parentNode != null && parentNode.Content == e.TargetRecord || (e.TargetRecord as TreeItemControl)?.InnerControl == null || (e.TargetRecord as TreeItemControl)?.InnerControl.GetType() == (node.Content as TreeItemControl)?.InnerControl.GetType())
                {
                    e.Effects = DragDropEffects.None;
                    return;
                }
                //var targetNode = ((TreeListView)e.OriginalSource).GetNodeByContent(e.TargetRecord);
                //var parentTargetNode = targetNode.ParentNode;
                //var parentRowItem = parentNode.Content;
                //var parentTargetRowItem = parentTargetNode.Content;
            }
        }

        List<TreeListNode> draggedNodes = new List<TreeListNode>();
        private void TreeListControl_DragStart(object sender, StartRecordDragEventArgs e)
        {
            draggedNodes.Clear();

            if (e.Records == null || e.Records.Count() == 0)
            {
                e.AllowDrag = false;
                e.Handled = true;
                return;
            }

            foreach (var c in e.Records)
            {
                if (c as TreeItemControl != null && contentToNodeMap.ContainsKey(c as TreeItemControl))
                    draggedNodes.Add(contentToNodeMap[c as TreeItemControl]);
            }

            bool bAllow = true;
            foreach (var dragItem in draggedNodes)
            {
                if(dragItem.Tag == null || dragItem.Tag is string || dragItem.Tag is CategoryTagHelper)
                {
                    bAllow = false;
                    break;
                }
            }

            if (!bAllow)
            {
                e.AllowDrag = false;
                e.Handled = true;
                draggedNodes.Clear();
                return;
            }
        }

        private void TreeListControl_DragEnd(object sender, DropRecordEventArgs e)
        {
            //e.Handled = true;
        }

        void TreeListControl_CompletedDragDrop(object sender, CompleteRecordDragDropEventArgs e)
        {
            //e.Handled = true;
        }

        #endregion
    }
}

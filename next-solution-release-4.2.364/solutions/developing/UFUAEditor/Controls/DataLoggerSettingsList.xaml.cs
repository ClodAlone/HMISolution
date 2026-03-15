using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UFUAEditor.Document;
using Utilities;
using Utilities.WPF;
using UFUAEditor.ComponentService;
using DevExpress.Xpo;
using System.Windows.Threading;
using DocumentManager.ComponentService;
using DataLoggerModel.Helpers;
using WPFUtilities;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using System.Windows.Data;
using UFUAEditor.Helpers;
using System.ComponentModel;
using UFUAEditor.Extensions;
using DevExpress.Data.TreeList;
using Utilities.Xpo.UndoRedo;
using UIMsgBoxAlertService.ComponentService;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for DataLoggerSettingsList.xaml
    /// </summary>
    public partial class DataLoggerSettingsList : TreeViewEditorHelper, IEditableObject
    {
        #region Declarations

        TreeListNode itemRoot;
        BitmapImage openFolderImg;
        BitmapImage closedFolderImg;

        List<int> expandedMasterRowHandles = new List<int>();

        bool isPopup;
        #endregion

        public DataLoggerSettingsList(UFUAServerDocument doc, bool bPopup = false)
        {
            InitializeComponent();
            SetTabsContent(treeListControl, gridDataControl);

            Document = doc;
            isPopup = bPopup;

            Document.CreateUndoRedoHelper();
            Document.ChangedDocument += Document_ChangedDocument;

            gridDataControl.ItemsSource = Document.GetDataLoggerSettings();
            tabDLSettingsTabControl.SelectionChanging += (s, e) =>
            {
                if (e.NewSelectedItem == gridControlTab &&
                    dpFlatGridRefresh != null && dpFlatGridRefresh.Status == DispatcherOperationStatus.Aborted)
                    FlatGridRefresh();
            };

            InitializeAddressSpace();
        }

        void Document_ChangedDocument(object sender, ChangedDocumentEvent e)
        {
            if (e.Source == this)
                return;

            var changedObjects = (from object c in e.ChangedObjects.AsParallel()
                                  where c is DataLoggerModel.DataLoggerSettings ||
                                  c is DataLoggerModel.DataLoggerColumn
                                  select c).ToList();

            if (changedObjects.Count > 0)
            {
                try
                {
                    UnSubscribeSelectionChangedEvent = true;
                    treeListControl.BeginDataUpdate();

                    foreach (var changedObject in changedObjects)
                    {
                        if (e.ChangedType == ChangedType.removed)
                        {
                            if (mapObjectToNode.ContainsKey(changedObject))
                            {
                                var parent = mapObjectToNode[changedObject].ParentNode;
                                parent.Nodes.Remove(mapObjectToNode[changedObject]);

                                FlatGridRefresh();
                            }
                        }
                        else if (e.ChangedType == ChangedType.added)
                        {
                            TreeListNode parent = null;
                            if (changedObject is DataLoggerModel.DataLoggerColumn && (changedObject as DataLoggerModel.DataLoggerColumn).DataLoggerReference != null)
                            {
                                var datalogger = (changedObject as DataLoggerModel.DataLoggerColumn).DataLoggerReference;
                                if (mapObjectToNode.ContainsKey(datalogger))
                                    parent = mapObjectToNode[datalogger];
                            }
                            else if (changedObject is DataLoggerModel.DataLoggerSettings)
                                parent = itemRoot;

                            if (parent != null)
                            {
                                if (parent.WasExpanded())
                                    AddTreeItem(changedObject, parent);

                                FlatGridRefresh();
                            }
                        }
                    }
                }
                finally
                {
                    treeListControl.EndDataUpdate();
                    UnSubscribeSelectionChangedEvent = false;
                }
            }
        }

        #region Commands
        private void OnCommandCut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                CopySelectedToClipboard();
                DeleteSelectedItems();
                Document.CopyInMemoryDataToWinClipboard();
            }
        }

        private void CanCommandCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabDLSettingsTabControl.SelectedItem == treeListTab && IsAnyItemSelected(typeof(DataLoggerModel.DataLoggerSettings));
            e.Handled = !e.CanExecute;
        }

        private void OnCommandCopy(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                CopySelectedToClipboard();
                Document.CopyInMemoryDataToWinClipboard();
            }
        }

        private void CanCommandCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabDLSettingsTabControl.SelectedItem == treeListTab && IsAnyItemSelected(typeof(DataLoggerModel.DataLoggerSettings));
            e.Handled = !e.CanExecute;
        }

        private void OnCommandPaste(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                PasteFromClipboard();
            }
        }

        private void CanCommandPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabDLSettingsTabControl.SelectedItem == treeListTab && Document.ClipboardContainsDataLoggerSettings() || Document.ClipboardContainsDataLoggerColumn();
            e.Handled = !e.CanExecute;
        }
        #endregion

        #region Address Space

        class rootHeader
        {
            public String Name { get; set; }
        }

        void InitializeAddressSpace()
        {
            openFolderImg = UFUAEditorManagerComponent.GetBitmapImage("OpenFolderSmall", true);
            closedFolderImg = UFUAEditorManagerComponent.GetBitmapImage("CloseFolderSmall", true);

            treeListView.Nodes.Clear();
            itemRoot = treeListControl.AddNode(new TreeItemControl(Properties.Resources.DataLoggerSettings) { ResourceIcon = closedFolderImg }, Tag as TreeListNode, Document);
            treeListControl.AddNode(null, itemRoot, TreeListControlHelper.DummyNode);

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                if (bDisposed)
                    return;

                FillItems(itemRoot);
                if (itemRoot.Nodes.Count == 0)
                    treeListControl.AddNode(null, itemRoot, TreeListControlHelper.DummyNode);
                itemRoot.IsExpanded = true;
            });
        }

        void OnGridSelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            e.Handled = true;
            if (!isPopup && !UnSubscribeSelectionChangedEvent)
            {
                EndEdit();

                UpdateContextObjects(sender);
            }
        }

        void treeListControl_SelectionChanged(object sender, TreeListSelectionChangedEventArgs e)
        {
            if (!isPopup && !UnSubscribeSelectionChangedEvent)
            {
                EndEdit();

                UpdateContextObjects(sender);
            }
        }

        private void FillItems(TreeListNode iRoot)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                ClearNodes(iRoot);
                using (new WaitCursor())
                {
                    var listDataLoggers = Document.GetDataLoggerSettings();

                    if (listDataLoggers != null)
                    {
                        foreach (var column in listDataLoggers)
                            AddTreeItem(column, iRoot);
                    }
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }
        }

        private void FillItems(TreeListNode iRoot, DataLoggerModel.DataLoggerSettings root)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                ClearNodes(iRoot);
                using (new WaitCursor())
                {
                    var columns = (from c in root.Columns orderby c.Oid ascending select c).ToList();
                    foreach (var column in columns)
                        AddTreeItem(column, iRoot);
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }
        }

        void OnTreeNodeChanged(object sender, TreeListNodeChangedEventArgs e)
        {
            if (e.ChangeType == NodeChangeType.Add && e.Node.Tag != null && e.Node.Tag != TreeListControlHelper.DummyNode && !mapObjectToNode.ContainsKey(e.Node.Tag))
                mapObjectToNode.Add(e.Node.Tag, e.Node);
            else if (e.ChangeType == NodeChangeType.Remove && e.Node.Tag != null && e.Node.Tag != TreeListControlHelper.DummyNode)
                ClearObjectMapNode(e.Node);
        }

        void ClearObjectMapNode(TreeListNode node)
        {
            if (node.Tag != null && node.Tag != TreeListControlHelper.DummyNode && mapObjectToNode.ContainsKey(node.Tag))
            {
                mapObjectToNode.Remove(node.Tag);
                foreach (var child in node.Nodes)
                    ClearObjectMapNode(child);
            }
        }

        void ClearNodes(TreeListNode node)
        {
            node.Nodes.Clear();
        }

        void OnTreeNodeCollapsing(object sender, TreeListNodeAllowEventArgs e)
        {
            if (!(e.Node.Content as TreeItemControl).IsNodeExpanding)
                UpdateFolderIcon(e.Node, false);
        }

        void UpdateFolderIcon(TreeListNode node, bool isOpen)
        {
            if (node != null && (node == itemRoot || node.Tag is UFUAModel.UFUAFolder))
                (node.Content as TreeItemControl).ResourceIcon = isOpen ? openFolderImg : closedFolderImg;
        }

        void OnTreeNodeExpanding(object sender, TreeListNodeAllowEventArgs e)
        {
            TreeListNode item = e.Node;
            try
            {
                if (item == null || TreeListControlHelper.WasExpanded(item)) //Node's subtree already populated
                    return;

                if (e != null && !TreeListControlHelper.CanBeExpanded(item))
                {
                    if (item.Tag is UFUAModel.UFUAFolder || item == itemRoot)
                        UpdateFolderIcon(item, true);
                    return;
                }

                (item.Content as TreeItemControl).IsNodeExpanding = true;
                treeListControl.BeginDataUpdate();

                ClearNodes(item);
                if (item.Tag is DataLoggerModel.DataLoggerSettings)
                    FillItems(item, item.Tag as DataLoggerModel.DataLoggerSettings);

                treeListControl.EndDataUpdate();
            }
            finally
            {
                (item.Content as TreeItemControl).IsNodeExpanding = false;
            }
        }

        internal TreeListNode AddTreeItem(Object tag, TreeListNode parent)
        {
            if (bDisposed)
                return null;

            if (!parent.IsExpanded && !(parent.Content as TreeItemControl).IsNodeExpanding)
            {
                parent.IsExpanded = true;
                var list = (from p in parent.Nodes where p.Tag == tag select p).ToList();
                if (list.Count > 0)
                    return list[0];
            }

            var ic = new TreeItemControl(tag);
            var newitem = treeListControl.AddNode(ic, parent, tag);

            if (tag is DataLoggerModel.DataLoggerSettings)
            {
                //var datalogger = tag as DataLoggerModel.DataLoggerSettings;
                //(newitem.Content as TreeItemControl).ItemHeader = datalogger.Name;
                SetBindingOnProp(newitem, tag, "Name");
                (newitem.Content as TreeItemControl).ResourceIcon = UFUAEditorManagerComponent.GetBitmapImage("UFUASDataLoggerSettingsSmall");
                if (NeedToBeExpanded(newitem))
                    treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);
            }
            else if (tag is DataLoggerModel.DataLoggerColumn)
            {
                //var column = tag as DataLoggerModel.DataLoggerColumn;
                //(newitem.Content as TreeItemControl).ItemHeader = column.Name;
                SetBindingOnProp(newitem, tag, "ColumnFullName");
                (newitem.Content as TreeItemControl).ResourceIcon = UFUAEditorManagerComponent.GetBitmapImage("UFUASDataLoggerColumnSmall");
            }

            treeListControl.RefreshRow(newitem.RowHandle); //Otherwise node's header (ItemHeader) can disappear in certain conditions
            return newitem;
        }

        public TreeListNode GetSelectedParent(bool bSkipSelected = false)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (!bSkipSelected && selected != null)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null)
                    return selected;
            }

            return itemRoot;
        }

        DispatcherOperation dpFlatGridRefresh;
        List<DataLoggerModel.DataLoggerSettings> itemsToSelect = null;
        internal void FlatGridRefresh(List<DataLoggerModel.DataLoggerSettings> selectItems = null)
        {
            itemsToSelect = selectItems;
            if (dpFlatGridRefresh == null || dpFlatGridRefresh.Status == DispatcherOperationStatus.Aborted)
            {
                var selected = gridDataControl.SelectedItem;
                UnSubscribeSelectionChangedEvent = true;
                gridDataControl.ItemsSource = null;
                UnSubscribeSelectionChangedEvent = false;
                var action = new Action(() =>
                {
                    using (new WaitCursor())
                    {
                        try
                        {
                            UnSubscribeSelectionChangedEvent = true;
                            gridDataControl.ItemsSource = null;
                            gridDataControl.ItemsSource = Document.GetDataLoggerSettings();

                            if (itemsToSelect == null)
                                gridDataControl.SelectedItem = selected;
                            else
                                gridDataControl.SelectedItems = itemsToSelect;

                            var selectedHandles = gridDataControl.GetSelectedRowHandles();
                            if (selectedHandles.Count() > 0)
                                gridDataControl.View.FocusedRowHandle = selectedHandles.Last();
                            else if (gridDataControl.SelectedItem == null && gridDataControl.SelectedItems.Count == 0)
                            {
                                var selectedItems = (from c in treeListControl.GetSelectedNodes()
                                                     where c.Tag != null
                                                     select c.Tag).ToList();
                                if (selectedItems.Count > 0)
                                {
                                    selectedItems.ForEach((o) => gridDataControl.SelectedItems.Add(o));
                                    gridDataControl.CurrentItem = selectedItems.First();
                                }
                            }

                            foreach (var rh in expandedMasterRowHandles)
                                gridDataControl.ExpandMasterRow(rh);
                        }
                        finally
                        {
                            UnSubscribeSelectionChangedEvent = false;
                        }
                    }
                });

                //if (gridDataControl.IsVisible)
                //    action();
                //else
                {
                    dpFlatGridRefresh = Dispatcher.BeginInvokeAsynchronouslyInRender(gridDataControl, () =>
                    {
                        if (bDisposed)
                            return;
                        dpFlatGridRefresh = null;

                        action();
                    });
                }
            }
        }

        internal void EditSelectedItem()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null)
            {
                if (selected.Tag is DataLoggerModel.DataLoggerSettings)
                {
                    var dataloggersettings = selected.Tag as DataLoggerModel.DataLoggerSettings;
                    EditSettings(dataloggersettings);
                }
                else if (selected.Tag is DataLoggerModel.DataLoggerColumn)
                {
                    var column = selected.Tag as DataLoggerModel.DataLoggerColumn;

                    using (var uow = Document.BeginNestedUnitOfWork())
                    {
                        var newDataLoggerColumnControl = new NewDataLoggerColumn(Document)
                        {
                            DataContext = uow.GetNestedObject(column)
                        };
                        GeneralDialogContent Dialog = new GeneralDialogContent(newDataLoggerColumnControl)
                        {
                            Owner = this.FindParent<Window>(),
                            HelpLink = "EditDataLoggerColumn"
                        };
                        if (Dialog.ShowDialog() == true)
                        {
                            var oldObject = column as DataLoggerModel.DataLoggerColumn;
                            var newObject = newDataLoggerColumnControl.DataContext as DataLoggerModel.DataLoggerColumn;
                            if (oldObject.DataLoggerReference != null && 
                                DataLoggerSettingsHelper.NeedUpdateAggregatedTables(oldObject, newObject))
                            {
                                UFUAEditorControl view = Document.ActiveView as UFUAEditorControl;
                                if (view != null)
                                    view.AddPendingAggregateTables(oldObject.DataLoggerReference.Name, AggregationTypes.CommandType.Update);
                            }

                            Document.AddUndoAction(this, column, UndoRedoAction.Changed);
                            uow.CommitChanges();
                            UpdateContextObjects();
                        }
                    }
                }
            }
        }

        internal void DeleteSelectedItems()
        {
            var items = new List<DataLoggerModel.DataLoggerSettings>();
            var tags = new List<DataLoggerModel.DataLoggerColumn>();
            var treeitems = new List<TreeListNode>();
            var listundo = new List<IXPSimpleObject>();
            
            if (gridControlTab.IsSelected)
            {
                var selectedItems = gridDetailsControl.SelectedItems;
                if (selectedItems.Count == 0)
                    selectedItems = gridDataControl.SelectedItems;
                foreach (var item in selectedItems)
                {
                    if (item is DataLoggerModel.DataLoggerSettings)
                        items.Add(item as DataLoggerModel.DataLoggerSettings);
                    else if (item is DataLoggerModel.DataLoggerColumn)
                        tags.Add(item as DataLoggerModel.DataLoggerColumn);
                    if (IsUndoRedoSupported(item))
                        listundo.Add(item as IXPSimpleObject);
                    if (mapObjectToNode.ContainsKey(item))
                        treeitems.Add(mapObjectToNode[item]);
                }
            }
            else
            {
                treeitems.AddRange((from item in treeListControl.GetSelectedNodes()
                                   where item.Tag != null
                                   select item));

                listundo.AddRange((from item in treeitems
                                   where IsUndoRedoSupported(item.Tag) && 
                                   !treeitems.Contains(item.ParentNode)
                                   select item.Tag as IXPSimpleObject));
            }

            if (listundo.Count > 0)
                Document.AddUndoAction(this, listundo, UndoRedoAction.Removed);

            if (gridControlTab.IsSelected)
            {
                foreach (var ufuaView in items)
                    ufuaView.Delete();
                
                var view = gridDataControl.View.FocusedView;
                foreach (var tag in tags)
                {
                    var mrh = (view.DataControl as GridControl).GetMasterRowHandle();
                    RemoveSettingFromColumn(tag, (view.DataControl as GridControl).GetMasterGrid().GetRow(mrh) as DataLoggerModel.DataLoggerSettings);
                }
            }

            try
            {
                treeListControl.BeginDataUpdate();
                foreach (var item in treeitems)
                    DeleteTreeItem(item);
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }

            if (gridControlTab.IsSelected)
                gridDataControl.SelectedItems.Clear();

            FlatGridRefresh();
        }

        internal void DeleteTreeItem(TreeListNode item)
        {
            //var item = addressSpaceTree.SelectedItem as TreeViewItemAdv;
            var parent = item.ParentNode ?? itemRoot;
            if (item.Tag is DataLoggerModel.DataLoggerSettings)
            {
                var datalogger = item.Tag as DataLoggerModel.DataLoggerSettings;
                //GetSelectedParent(true).Items.Remove(item);
                parent.Nodes.Remove(item);
                datalogger.Delete();
            }
            else if (item.Tag is DataLoggerModel.DataLoggerColumn)
            {
                var column = item.Tag as DataLoggerModel.DataLoggerColumn;
                if (column != null)
                {
                    parent.Nodes.Remove(item);
                    RemoveSettingFromColumn(column, parent.Tag as DataLoggerModel.DataLoggerSettings, parent);
                }
            }

            FlatGridRefresh();
        }

        void RemoveSettingFromColumn(DataLoggerModel.DataLoggerColumn column, DataLoggerModel.DataLoggerSettings parentSettings, TreeListNode parent = null)
        {
            if (column.DataLoggerReference != null && column.DataLoggerReference.UseAggregatedTables &&
                    column.ColumnTag != null && !column.ColumnTag.IsEmpty())
            {
                UFUAEditorControl view = Document.ActiveView as UFUAEditorControl;
                if (view != null)
                    view.AddPendingAggregateTables(column.DataLoggerReference.Name, AggregationTypes.CommandType.Update);
            }
            column.Delete();
        }

        internal void CopySelectedToClipboard()
        {
            var listdataloggers = new List<DataLoggerModel.DataLoggerSettings>();
            var listcolumns = new List<DataLoggerModel.DataLoggerColumn>();
            foreach (var item in treeListControl.GetSelectedNodes())
            {
                if (item.Tag is DataLoggerModel.DataLoggerSettings)
                    listdataloggers.Add(item.Tag as DataLoggerModel.DataLoggerSettings);
                else if (item.Tag is DataLoggerModel.DataLoggerColumn)
                    listcolumns.Add(item.Tag as DataLoggerModel.DataLoggerColumn);
            }

            Document.CleanClipbaord();
            Document.CopyListDataLoggerSettingsToClipbaord(listdataloggers);
            Document.CopyListDataLoggerColumnToClipbaord(listcolumns);
            Document.CheckClipbaord();
        }

        private void OnMasterRowExpanded(object sender, RowEventArgs e)
        {
            if (!expandedMasterRowHandles.Contains(e.RowHandle))
                expandedMasterRowHandles.Add(e.RowHandle);
        }

        private void OnMasterRowCollapsed(object sender, RowEventArgs e)
        {
            if (expandedMasterRowHandles.Contains(e.RowHandle))
                expandedMasterRowHandles.Remove(e.RowHandle);
        }

        private void TreeListControl_DragStart(object sender, StartRecordDragEventArgs e)
        {
            e.AllowDrag = false;
            e.Handled = true;
            return;
        }

        void TreeListControl_CompletedDragDrop(object sender, CompleteRecordDragDropEventArgs e)
        {
            e.Handled = true;
        }

        private void OnCanSelectRow(object sender, CanSelectRowEventArgs e)
        {
            e.CanSelectRow = true;
        }

        internal void PasteFromClipboard()
        {
            List<TreeListNode> pastedNodes = new List<TreeListNode>();
            try
            {
                var parent = GetSelectedParent();
                var expanded = parent.IsExpanded || parent.Nodes.Count > 0 && parent.Nodes[0].Tag != TreeListControlHelper.DummyNode;
                treeListControl.ClearSelection();
                UnSubscribeSelectionChangedEvent = true;
                treeListControl.BeginDataUpdate();

                DataLoggerModel.DataLoggerSettings datalogger = null;
                if (parent.Tag is DataLoggerModel.DataLoggerSettings)
                    datalogger = parent.Tag as DataLoggerModel.DataLoggerSettings;

                var listdataloggers = Document.PasteClipboardDataLoggerSettings();
                var rootExpanded = itemRoot.IsExpanded || itemRoot.Nodes.Count > 0 && itemRoot.Nodes[0].Tag != TreeListControlHelper.DummyNode;
                treeListControl.AddDummyNodeIfNeeded(parent, NeedToBeExpanded);

                pastedNodes.AddRange(treeListControl.AddGetNodesList(itemRoot, listdataloggers, rootExpanded, AddTreeItem));

                var listcolumns = new List<DataLoggerModel.DataLoggerColumn>();
                if (datalogger != null)
                {
                    listcolumns = Document.PasteClipboardDataLoggerColumn(datalogger);
                    treeListControl.AddDummyNodeIfNeeded(parent, NeedToBeExpanded);
                }

                pastedNodes.AddRange(treeListControl.AddGetNodesList(itemRoot, listcolumns, expanded, AddTreeItem, parent));

                UFUAEditorControl view = Document.ActiveView as UFUAEditorControl;
                if (view != null)
                {
                    if (datalogger != null && datalogger.UseAggregatedTables && listcolumns.Count > 0)
                        view.AddPendingAggregateTables(datalogger.Name, AggregationTypes.CommandType.Update);

                    var dataloggers = (from c in listdataloggers.AsParallel() where c.UseAggregatedTables select c.Name).ToList();
                    dataloggers.ForEach(name => view.AddPendingAggregateTables(name, AggregationTypes.CommandType.Update));
                }

                if (listdataloggers.Count > 0 || listcolumns.Count > 0)
                {
                    FlatGridRefresh(listdataloggers);

                    // add list to undo manager
                    var listundo = new List<IXPSimpleObject>();
                    listdataloggers.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
                    listcolumns.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
                    Document.AddUndoAction(this, listundo, UndoRedoAction.Added);
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
                if (pastedNodes.Count > 0)
                    treeListControl.SelectNodes(pastedNodes, true, true);
                UnSubscribeSelectionChangedEvent = false;
                if (pastedNodes.Count > 0)
                    ForceSelectionChangedEvent();
            }
        }

        internal void AddDataLoggerSettings(DataLoggerModel.DataLoggerSettings datalogger)
        {
            Document.AddUndoAction(this, datalogger, UndoRedoAction.Added);
            var item = AddTreeItem(datalogger, itemRoot);
            treeListControl.ClearSelection();
            treeListControl.SelectNode(item);
            FlatGridRefresh();
        }

        internal void AddDataLoggerColumn(DataLoggerModel.DataLoggerColumn column)
        {
            Document.AddUndoAction(this, column, UndoRedoAction.Added);
            var item = AddTreeItem(column, GetSelectedDataLoggerSettingsParentItem());
            treeListControl.ClearSelection();
            treeListControl.SelectNode(item);
        }

        internal List<DataLoggerModel.DataLoggerColumn> AddDataloggerToTags(DataLoggerModel.DataLoggerSettings datalogger, List<TagIdentifier> tags)
        {
            var nodesToSelect = new List<TreeListNode>();
            var addedColumns = new List<DataLoggerModel.DataLoggerColumn>();
            var parentTreeNode = GetSelectedDataLoggerSettingsParentItem();
            if (datalogger == null || parentTreeNode == null)
                return addedColumns;

            try
            {
                UnSubscribeSelectionChangedEvent = true;
                treeListControl.BeginDataUpdate();
                using (new AsyncWaitCursor())
                {
                    var listundo = new List<IXPSimpleObject>();
                    foreach (var tagDef in tags)
                    {
                        var reference = tagDef.TagReference as UFUAModel.TagEntityReference;
                        if (reference != null)
                        {
                            var tag = Document.FindTagByEntityReference(reference);
                            if (tag != null)
                            {
                                var bIsDuplicated = (from col in datalogger.Columns 
                                                     where col.ColumnTag.Guid == reference.Guid &&
                                                     col.ColumnTag.NodeId == reference.NodeId
                                                     select col).FirstOrDefault() != null;
                                if (bIsDuplicated)
                                    continue;

                                var dataloggerColumn = Document.AddNewDataLoggerColumn(datalogger);
                                //dataloggerColumn.DataLoggerReference = datalogger;
                                dataloggerColumn.ColumnTag = reference;
                                dataloggerColumn.ColumnName = Document.NewDataLoggerColumnName(datalogger, tag.Name, format: "{0}_{1}", bRemoveEndsNumbers: false);
                                addedColumns.Add(dataloggerColumn);
                                var item = AddTreeItem(dataloggerColumn, parentTreeNode);
                                if (item != null)
                                    nodesToSelect.Add(item);
                            }
                        }
                    }
                    if (addedColumns.Count > 0)
                    {
                        listundo.AddRange(addedColumns);
                        FlatGridRefresh();
                        Document.NeedsSave = true;
                    }

                    if (listundo.Count > 0)
                        Document.AddUndoAction(this, listundo, UndoRedoAction.Added);
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
                if (nodesToSelect.Count > 0)
                    treeListControl.SelectNodes(nodesToSelect, true, true);
                UnSubscribeSelectionChangedEvent = false;
                if (nodesToSelect.Count > 0)
                    ForceSelectionChangedEvent();
            }
            return addedColumns;
        }

        void ForceSelectionChangedEvent()
        {
            treeListControl_SelectionChanged(this, null);
        }

        internal TreeListNode GetSelectedDataLoggerSettingsParentItem()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is DataLoggerModel.DataLoggerSettings)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is DataLoggerModel.DataLoggerSettings)
                    return selected;
            }

            return itemRoot;
        }

        internal DataLoggerModel.DataLoggerSettings GetSelectedDataLoggerSettingsParent()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is DataLoggerModel.DataLoggerSettings)
                return selected.Tag as DataLoggerModel.DataLoggerSettings;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is DataLoggerModel.DataLoggerSettings)
                    return selected.Tag as DataLoggerModel.DataLoggerSettings;
            }

            return null;
        }

        internal List<DataLoggerModel.DataLoggerSettings> GetSelectedDataLoggers()
        {
            var selectedNodes = treeListControl.GetSelectedNodes();
            if (selectedNodes.Length == 0)
                return null;

            var dataloggers = new List<DataLoggerModel.DataLoggerSettings>();
            foreach (TreeListNode selected in selectedNodes)
            {
                var settings = selected.Tag as DataLoggerModel.DataLoggerSettings;
                if (settings != null)
                    dataloggers.Add(selected.Tag as DataLoggerModel.DataLoggerSettings);
            }

            return dataloggers;
        }

        internal DataLoggerModel.DataLoggerColumn GetSelectedDataLoggerColumn()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is DataLoggerModel.DataLoggerColumn)
                return selected.Tag as DataLoggerModel.DataLoggerColumn;

            return null;
        }

        void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            e.Handled = true;
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null)
            {
                if (isPopup && selected.Tag is DataLoggerModel.DataLoggerSettings)
                {
                    var wnd = this.FindParent<Window>();
                    if (wnd != null)
                    {
                        wnd.DialogResult = true;
                        wnd.Close();
                    }
                }
                else if (!isPopup)
                {
                    if (Document.EditorManagerComponent.PropertyControl != null)
                        Document.EditorManagerComponent.PropertyControl.Activate();
                    else
                        EditSelectedItem();
                }
            }
        }

        private void gridDataControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (!isPopup && Document.EditorManagerComponent.PropertyControl != null)
                Document.EditorManagerComponent.PropertyControl.Activate();
            else
            {
                var dataloggersettings = gridDataControl.SelectedItem as DataLoggerModel.DataLoggerSettings;
                EditSettings(dataloggersettings);
            }
        }

        private void EditSettings(DataLoggerModel.DataLoggerSettings dataloggersettings)
        {
            if (dataloggersettings == null)
                return;

            using (var uow = Document.BeginNestedUnitOfWork())
            {
                var newDataLoggerSettingsControl = new NewDataLoggerSettings(Document)
                {
                    DataContext = uow.GetNestedObject(dataloggersettings)
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newDataLoggerSettingsControl)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "EditDataLogger"
                };
                if (Dialog.ShowDialog() == true)
                {
                    var oldObject = dataloggersettings as DataLoggerModel.DataLoggerSettings;
                    var newObject = newDataLoggerSettingsControl.DataContext as DataLoggerModel.DataLoggerSettings;
                    if (DataLoggerSettingsHelper.NeedUpdateAggregatedTables(oldObject, newObject))
                    {
                        UFUAEditorControl view = Document.ActiveView as UFUAEditorControl;
                        if (view != null)
                            view.AddPendingAggregateTables(newObject.Name, AggregationTypes.CommandType.Update);
                    }

                    Document.AddUndoAction(this, dataloggersettings, UndoRedoAction.Changed);
                    uow.CommitChanges();
                    UpdateContextObjects();
                    FlatGridRefresh();
                }
            }
        }

        #endregion

        #region Drag & Drop

        private void TreeListControl_DragEnd(object sender, DropRecordEventArgs e)
        {
            e.Handled = true;
            var targetContent = (e.TargetRecord as TreeItemControl)?.TreeItemInnerObject;
            var targetDataLoggerSetting = (e.TargetRecord as TreeItemControl)?.TreeItemInnerObject as DataLoggerModel.DataLoggerSettings;
            var bDroppingOnRoot = itemRoot.RowHandle == e.TargetRowHandle;
            var targetTreeNode = treeListView.GetNodeByContent(e.TargetRecord);

            if (targetDataLoggerSetting != null && targetTreeNode != null &&
                !bDroppingOnRoot && targetContent != null && mapObjectToNode.ContainsKey(targetContent)
                && e.OriginalSource == treeListView)
            {
                using (var Cursor = new WaitCursor())
                {   
                    //Dispatcher.InvokeIfRequired((Action)delegate
                    //{
                        var data = e.Data.GetData(typeof(RecordDragDropData)) as RecordDragDropData;
                        if (data == null)
                            return;

                        var listcolumns = new List<IXPSimpleObject>(data.Records.Count());
                        foreach (var v in data.Records)
                        {
                            IDocumentManager model = null;
                            if (v is IDocumentManager)
                                model = v as IDocumentManager;
                            else if (v is TreeItemControl)
                                model = (v as TreeItemControl).TreeItemInnerObject as IDocumentManager;
                            if (model == null)
                                continue;

                            var entity = model.DragContent as OPCUAViewModel.OPCUAEntityReference;
                            if (entity != null)
                            {
                                UFUAModel.UFUATag member;
                                var instance = Document.FindTagByNodeId(entity.ResolvedNodeId, out member);
                                if (instance != null && CanAssignDataLoggerColumn(member ?? instance, member != null ? instance : null))
                                {
                                    var name = entity.ReadablePath;
                                    var index = name.LastIndexOf(':');
                                    if (index != -1)
                                        name = name.Substring(index + 1);
                                    var col = Document.AddNewDataLoggerColumn(targetDataLoggerSetting);
                                    col.ColumnName = Document.NewDataLoggerColumnName(targetDataLoggerSetting, name, format: "{0}_{1}", bRemoveEndsNumbers: false);
                                    col.ColumnTag = new UFUAModel.TagEntityReference(member == null ? instance.NodeId : member.NodeId, entity.RelativePath, entity.ResolvedNodeId, entity.HumanReadable);
                                    listcolumns.Add(col);
                                }
                            }
                        }

                        var expanded = targetTreeNode.IsExpanded || (targetTreeNode.Nodes.Count > 0 && targetTreeNode.Nodes[0].Tag != TreeListControlHelper.DummyNode);
                        treeListControl.ClearSelection();
                        treeListControl.AddDummyNodeIfNeeded(targetTreeNode, NeedToBeExpanded);

                        treeListControl.AddGetNodesList(itemRoot, listcolumns, expanded, AddTreeItem, targetTreeNode);

                        //listcolumns.ForEach(col =>
                        //{
                        //    TreeListNode item = null;
                        //    if (expanded)
                        //        item = AddTreeItem(col, targetTreeNode, true);
                        //    else
                        //        item = treeListControl.GetTreeItem(col, targetTreeNode);
                        //    if (item != null)
                        //        lastItem = item;
                        //});
                        //if (lastItem != null)
                        //    treeListControl.SelectNode(lastItem);

                        if (listcolumns.Count > 0)
                        {
                            Document.AddUndoAction(this, listcolumns, UndoRedoAction.Added);
                            if (targetDataLoggerSetting != null && targetDataLoggerSetting.UseAggregatedTables)
                            {
                                UFUAEditorControl view = Document.ActiveView as UFUAEditorControl;
                                if (view != null)
                                    view.AddPendingAggregateTables(targetDataLoggerSetting.Name, AggregationTypes.CommandType.Update);
                            }
                        }
                    //}, DispatcherPriority.ApplicationIdle);
                }
            }
        }

        private void treeListControl_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(RecordDragDropData)))
                return;

            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
            uint forbidden = 0;

            var data = e.Data.GetData(typeof(RecordDragDropData)) as RecordDragDropData;
            foreach (var v in data.Records)
            {
                IDocumentManager model = null;
                if (v is IDocumentManager)
                    model = v as IDocumentManager;
                else if (v is TreeItemControl)
                    model = (v as TreeItemControl).TreeItemInnerObject as IDocumentManager;
                if (model == null)
                {
                    forbidden++;
                    continue;
                }

                if (model.BrowsableContent is UFUAModel.UFUATag)
                {
                    var tag = (UFUAModel.UFUATag)model.BrowsableContent;
                    if (!CanAssignDataLoggerColumn(tag))
                        forbidden++;
                }
            }

            if (forbidden == data.Records.Count())
                e.Effects = DragDropEffects.None;
        }

        private void addressSpaceTree_PreviewQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (!e.EscapePressed)
                return;

            e.Action = DragAction.Cancel;
            e.Handled = true;
        }

        bool NeedToBeExpanded(TreeListNode item)
        {
            if (item.Tag is DataLoggerModel.DataLoggerSettings)
                return ((DataLoggerModel.DataLoggerSettings)item.Tag).Columns.Count > 0;
            
            return false;
        }

        List<TreeListNode> itemsToRefresh;
        void RefreshAsync(IEnumerable<TreeListNode> items)
        {
            if (itemsToRefresh != null)
                itemsToRefresh.AddRange(items);
            else
            {
                itemsToRefresh = new List<TreeListNode>(items);
                Dispatcher.BeginInvokeAsynchronously(() =>
                {
                    if (bDisposed)
                        return;

                    var refreshed = new List<TreeListNode>();
                    try
                    {
                        treeListControl.BeginDataUpdate();
                        foreach (var item in itemsToRefresh)
                        {
                            if (refreshed.Contains(item))
                                continue;

                            refreshed.Add(item);
                            Refresh(item);
                        }
                    }
                    finally
                    {
                        treeListControl.EndDataUpdate();
                        itemsToRefresh = null;
                    }
                });
            }
        }

        void RefreshAsync(TreeListNode item)
        {
            if (itemsToRefresh == null || !itemsToRefresh.Contains(item))
                RefreshAsync(new List<TreeListNode>() { item });
        }

        void Refresh(TreeListNode item)
        {
            if (item == itemRoot)
                return;

            var wasExpanded = item.IsExpanded;
            if (item.IsExpanded && item.Nodes.Count == 0)
                treeListControl.AddNode(null, item, TreeListControlHelper.DummyNode);
            item.IsExpanded = false;
            ClearNodes(item);

            if (NeedToBeExpanded(item))
            {
                treeListControl.AddNode(null, item, TreeListControlHelper.DummyNode);
                if (wasExpanded)
                    item.IsExpanded = true;
            }
        }

        private void OnHiddenEditor(object sender, EditorEventArgs e)
        {
            Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(((DataViewBase)sender).DataControl.SelectedItem);
        }

        protected override void OnValidateCell(object sender, GridCellValidationEventArgs e)
        {
            if (!(e.Row is DataLoggerModel.DataLoggerColumn))
                return;

            var oldCellValue = e.CellValue;
            var newCellValue = e.Value;
            if (oldCellValue != newCellValue)
            {
                using (var uow = Document.BeginNestedUnitOfWork())
                {
                    var column = uow.GetNestedObject((DataLoggerModel.DataLoggerColumn)e.Row);
                    {
                        var tagProp = typeof(DataLoggerModel.DataLoggerColumn).GetProperty(e.Column.FieldName);
                        if (newCellValue != null)
                            tagProp.SetValue(column, Convert.ChangeType(newCellValue, newCellValue.GetType()), null);
                        else
                            tagProp.SetValue(column, null, null);
                        var error = column[e.Column.FieldName];
                        if (!String.IsNullOrEmpty(error))
                        {
                            e.SetError(error);
                            e.IsValid = false;
                        }
                    }

                    if (e.IsValid)
                    {
                        UFUAEditorControl view = Document.ActiveView as UFUAEditorControl;
                        if (view != null)
                        {
                            var oldObject = (DataLoggerModel.DataLoggerColumn)e.Row;
                            if (oldObject.DataLoggerReference != null &&
                                DataLoggerSettingsHelper.NeedUpdateAggregatedTables(oldObject, column))
                                view.AddPendingAggregateTables(oldObject.DataLoggerReference.Name, AggregationTypes.CommandType.Update);
                        }

                        Document.AddUndoAction(this, uow.GetParentObject(column), UndoRedoAction.Changed);
                        uow.CommitChanges();
                        UpdateContextObjects(gridDataControl);
                    }
                }
            }
        }

        #endregion

        #region Undo/Redo

        protected override bool IsUndoRedoSupported(object obj)
        {
            return obj is IXPSimpleObject;
        }

        internal void UndoAction()
        {
            UndoRedoAction action;
            bool selectionchanged = false;
            var list = Document.UndoAction(this, out action);
            switch (action)
            {
                case UndoRedoAction.Added:
                    {
                        if (list.Count > 0)
                        {
                            try
                            {
                                treeListControl.ClearSelection();
                                UnSubscribeSelectionChangedEvent = true;
                                treeListControl.BeginDataUpdate();
                                foreach (var obj in list)
                                {
                                    if (mapObjectToNode.ContainsKey(obj.Source))
                                    {
                                        var parent = mapObjectToNode[obj.Source].ParentNode;
                                        var item = treeListControl.GetTreeItem(obj.Source, parent);
                                        if (item != null)
                                            DeleteTreeItem(item);
                                    }
                                }
                                FlatGridRefresh();
                            }
                            finally
                            {
                                treeListControl.EndDataUpdate();
                                UnSubscribeSelectionChangedEvent = false;
                                ForceSelectionChangedEvent();
                            }
                        }
                    }
                    break;
                case UndoRedoAction.Changed:
                    {
                        if (list.Count > 0)
                        {
                            var items = new List<TreeListNode>();
                            try
                            {
                                treeListControl.ClearSelection();
                                UnSubscribeSelectionChangedEvent = true;
                                treeListControl.BeginDataUpdate();
                                foreach (var obj in list)
                                {
                                    if (mapObjectToNode.ContainsKey(obj.Source))
                                    {
                                        var parent = mapObjectToNode[obj.Source].ParentNode;
                                        var item = treeListControl.GetTreeItem(obj.Source, parent);
                                        if (item != null)
                                            items.Add(item);
                                    }
                                }
                                FlatGridRefresh();
                            }
                            finally
                            {
                                treeListControl.EndDataUpdate();
                                if (items.Count > 0)
                                    treeListControl.SelectNodes(items, true, true);
                                UnSubscribeSelectionChangedEvent = false;
                                if (selectionchanged)
                                    ForceSelectionChangedEvent();
                            }
                        }
                    }
                    break;
                case UndoRedoAction.Removed:
                    {
                        if (list.Count > 0)
                        {
                            var items = new List<TreeListNode>();
                            try
                            {
                                treeListControl.ClearSelection();
                                UnSubscribeSelectionChangedEvent = true;
                                treeListControl.BeginDataUpdate();
                                foreach (var obj in list)
                                {
                                    var parent = itemRoot;
                                    var parentTag = Document.GetParentObject(obj);
                                    if (parentTag != null && mapObjectToNode.ContainsKey(parentTag))
                                        parent = mapObjectToNode[parentTag];
                                    var item = AddTreeItem(obj.Source, parent);
                                    if (item != null)
                                    {
                                        selectionchanged = true;
                                        items.Add(item);
                                    }
                                }
                                FlatGridRefresh();
                            }
                            finally
                            {
                                treeListControl.EndDataUpdate();
                                if (items.Count > 0)
                                    treeListControl.SelectNodes(items, true, true);
                                UnSubscribeSelectionChangedEvent = false;
                                if (selectionchanged)
                                    ForceSelectionChangedEvent();
                            }
                        }
                    }
                    break;
            }
        }

        internal void RedoAction()
        {
            UndoRedoAction action;
            bool selectionchanged = false;
            var list = Document.RedoAction(this, out action);
            switch (action)
            {
                case UndoRedoAction.Removed:
                    {
                        if (list.Count > 0)
                        {
                            try
                            {
                                treeListControl.ClearSelection();
                                UnSubscribeSelectionChangedEvent = true;
                                treeListControl.BeginDataUpdate();
                                foreach (var obj in list)
                                {
                                    if (mapObjectToNode.ContainsKey(obj.Source))
                                    {
                                        var parent = mapObjectToNode[obj.Source].ParentNode;
                                        var item = treeListControl.GetTreeItem(obj.Source, parent);
                                        if (item != null)
                                            DeleteTreeItem(item);
                                    }
                                }
                                FlatGridRefresh();
                            }
                            finally
                            {
                                treeListControl.EndDataUpdate();
                                UnSubscribeSelectionChangedEvent = false;
                                ForceSelectionChangedEvent();
                            }
                        }
                    }
                    break;
                case UndoRedoAction.Changed:
                    {
                        if (list.Count > 0)
                        {
                            var items = new List<TreeListNode>();
                            try
                            {
                                treeListControl.ClearSelection();
                                UnSubscribeSelectionChangedEvent = true;
                                treeListControl.BeginDataUpdate();
                                foreach (var obj in list)
                                {
                                    if (mapObjectToNode.ContainsKey(obj.Source))
                                    {
                                        var parent = mapObjectToNode[obj.Source].ParentNode;
                                        var item = treeListControl.GetTreeItem(obj.Source, parent);
                                        if (item != null)
                                            items.Add(item);
                                    }
                                }
                                FlatGridRefresh();
                            }
                            finally
                            {
                                treeListControl.EndDataUpdate();
                                if (items.Count > 0)
                                    treeListControl.SelectNodes(items, true, true);
                                UnSubscribeSelectionChangedEvent = false;
                                if (selectionchanged)
                                    ForceSelectionChangedEvent();
                            }
                        }
                    }
                    break;
                case UndoRedoAction.Added:
                    {
                        if (list.Count > 0)
                        {
                            var items = new List<TreeListNode>();
                            try
                            {
                                treeListControl.ClearSelection();
                                UnSubscribeSelectionChangedEvent = true;
                                treeListControl.BeginDataUpdate();
                                foreach (var obj in list)
                                {
                                    var parent = itemRoot;
                                    var parentTag = Document.GetParentObject(obj);
                                    if (parentTag != null && mapObjectToNode.ContainsKey(parentTag))
                                        parent = mapObjectToNode[parentTag];
                                    var item = AddTreeItem(obj.Source, parent);
                                    if (item != null)
                                    {
                                        selectionchanged = true;
                                        items.Add(item);
                                    }
                                }
                                FlatGridRefresh();
                            }
                            finally
                            {
                                treeListControl.EndDataUpdate();
                                if (items.Count > 0)
                                    treeListControl.SelectNodes(items, true, true);
                                UnSubscribeSelectionChangedEvent = false;
                                if (selectionchanged)
                                    ForceSelectionChangedEvent();
                            }
                        }
                    }
                    break;
            }
        }

        #endregion

        #region IEditableObject
        bool bEditingUow;
        public void BeginEdit()
        {
            if (bEditingUow)
                return;

            if (Document.UowContext != null)
            {
                bEditingUow = true;
                Document.UowContext.BeforeFlushChanges += uowContext_BeforeFlushChanges;
                Document.UowContext.ObjectChanged += uowContext_ObjectChanged;
            }
        }

        public void EndEdit()
        {
            if (!bEditingUow)
                return;

            bool bResult = true;
            try
            {
                if (Document.UowContext != null)
                {
                    bResult = Document.UowContext.TryCommitChanges(UFUAServerDocument.logGeneral);
                    Document.UowContext.BeforeFlushChanges -= uowContext_BeforeFlushChanges;
                    Document.UowContext.ObjectChanged -= uowContext_ObjectChanged;
                    if (bResult)
                        Document.NeedsSave = true;
                }
            }
            finally
            {
                bEditingUow = false;
            }

            if (!bResult)
            {
                var uiMsgBox = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(Properties.Resources.ErrorOnApplyingChanges);
            }
        }

        public void CancelEdit()
        {
            if (!bEditingUow)
                return;

            try
            {
                if (Document.UowContext != null)
                {
                    Document.UowContext.RollbackTransaction();
                    Document.UowContext.BeforeFlushChanges -= uowContext_BeforeFlushChanges;
                    Document.UowContext.ObjectChanged -= uowContext_ObjectChanged;
                }
            }
            finally
            {
                bEditingUow = false;
            }
        }

        private void uowContext_BeforeFlushChanges(object sender, DevExpress.Xpo.SessionManipulationEventArgs e)
        {
            var objects = Document.EditorManagerComponent.Workspace.ContextObjects;
            if (objects == null && Document.EditorManagerComponent.Workspace.ContextObject != null)
                objects = new List<object>() { Document.EditorManagerComponent.Workspace.ContextObject };
            if (objects != null)
            {
                UFUAEditorControl view = Document.ActiveView as UFUAEditorControl;
                var list = new List<DevExpress.Xpo.IXPSimpleObject>();
                foreach (var obj in objects)
                {
                    var parent = Document.UowContext.GetParentObject(obj);
                    if (parent != null)
                        list.Add(parent as DevExpress.Xpo.IXPSimpleObject);

                    if (view != null)
                    {
                        if (parent is DataLoggerModel.DataLoggerSettings &&
                            obj is DataLoggerModel.DataLoggerSettings)
                        {
                            var oldObject = parent as DataLoggerModel.DataLoggerSettings;
                            var newObject = obj as DataLoggerModel.DataLoggerSettings;
                            if (DataLoggerSettingsHelper.NeedUpdateAggregatedTables(oldObject, newObject))
                                view.AddPendingAggregateTables(newObject.Name, AggregationTypes.CommandType.Update);
                        }
                        else if (parent is DataLoggerModel.DataLoggerColumn &&
                            obj is DataLoggerModel.DataLoggerColumn)
                        {
                            var oldObject = parent as DataLoggerModel.DataLoggerColumn;
                            var newObject = obj as DataLoggerModel.DataLoggerColumn;
                            if (oldObject.DataLoggerReference != null &&
                                DataLoggerSettingsHelper.NeedUpdateAggregatedTables(oldObject, newObject))
                                view.AddPendingAggregateTables(oldObject.DataLoggerReference.Name, AggregationTypes.CommandType.Update);
                        }
                    }
                }

                if (list.Count > 0)
                    Document.AddUndoAction(this, list, UndoRedoAction.Changed);
            }
        }

        private void uowContext_ObjectChanged(object sender, DevExpress.Xpo.ObjectChangeEventArgs e)
        {
            if (e.Object is DataLoggerModel.DataLoggerSettings &&
                e.PropertyName == "UseAggregatedTables" && e.OldValue != e.NewValue)
            {
                var datalogger = e.Object as DataLoggerModel.DataLoggerSettings;
                UFUAEditorControl view = Document.ActiveView as UFUAEditorControl;
                if (view != null)
                    view.AddPendingAggregateTables(datalogger.Name, datalogger.UseAggregatedTables ? AggregationTypes.CommandType.Add : AggregationTypes.CommandType.Remove);
            }
        }
        #endregion

        #region IDisposable
        bool bDisposed;
        public override void OnDispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            if (itemRoot != null)
                ClearObjectMapNode(itemRoot);

            Document.EditorManagerComponent.Workspace.ContextObject = null;
            Document.ChangedDocument -= Document_ChangedDocument;

            if (dpFlatGridRefresh != null && dpFlatGridRefresh.Status != DispatcherOperationStatus.Aborted &&
                dpFlatGridRefresh.Status != DispatcherOperationStatus.Completed)
            {
                dpFlatGridRefresh.Abort();
            }

            // gridDataControl.Model.Dispose();
            try
            {
                gridDataControl.Dispose();
            }
            catch (Exception ex)
            {
                
            }
        }
        #endregion

        class ParentElement
        {
            #region Declarations
            TreeListNode node;
            public DataLoggerModel.DataLoggerSettings SettingsAss;
            #endregion
            #region Public Props
            public TreeListNode Node
            {
                get
                {
                    return node;
                }
                set
                {
                    if (node != value)
                    {
                        node = value;
                        if (node.Tag is DataLoggerModel.DataLoggerSettings)
                            SettingsAss = (DataLoggerModel.DataLoggerSettings)node.Tag;
                    }
                }
            }
            #endregion
            #region Ctor
            public ParentElement()
            {

            }
            #endregion
        }
    }
}

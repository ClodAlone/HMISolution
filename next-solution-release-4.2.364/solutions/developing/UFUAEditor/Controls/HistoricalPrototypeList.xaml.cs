using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UFUAEditor.Document;
using Utilities;
using Utilities.WPF;
using DevExpress.Xpo;
using WPFUtilities;
using DevExpress.Xpf.Grid;
using System.Windows.Media.Imaging;
using UFUAEditor.ComponentService;
using System.Windows.Data;
using DevExpress.Xpf.Grid.TreeList;
using System.ComponentModel;
using UFUAEditor.Helpers;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DocumentManager.ComponentService;
using UFUAEditor.Converters;
using DevExpress.Xpf.Editors.Settings;
using TempVariablesManager.Helpers;
using UFUAEditor.Extensions;
using DevExpress.Data.TreeList;
using UIMsgBoxAlertService.ComponentService;
using Utilities.Xpo.UndoRedo;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for HistoricalPrototypeList.xaml
    /// </summary>
    public partial class HistoricalPrototypeList : TreeViewEditorHelper, IEditableObject, IDisposable
    {
        #region Declarations
        bool isPopup;
        static int maxItems = Properties.Settings.Default.MaxItemsInTree;
        TreeListNode itemRoot;
        BitmapImage openFolderImg;
        BitmapImage closedFolderImg;

        List<int> expandedMasterRowHandles = new List<int>();
        #endregion

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
            e.CanExecute = IsAnyItemSelected(typeof(UFUAModel.UFUAHistorianSettings));
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
            e.CanExecute = IsAnyItemSelected(typeof(UFUAModel.UFUAHistorianSettings));
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
            e.CanExecute = Document.ClipboardContainsHistorians();
            e.Handled = !e.CanExecute;
        }
        #endregion

        #region Address Space

        public HistoricalPrototypeList(UFUAServerDocument doc, bool bPopup = false)
        {
            InitializeComponent();
            SetTabsContent(treeListControl, gridDataControl);
            Document = doc;
            isPopup = bPopup;

            TagPathColumn.DisplayMemberBinding = new Binding { Path = new PropertyPath("TreeItemInnerObject"), Converter = new TagPathConverter() { Document = doc } };
            if (isPopup)
                tableView.AllowMasterDetail = false;
            else
            {
                (gridDataControl.DetailDescriptor as DataControlDetailDescriptor).ItemsSourceBinding = new Binding { Path = new PropertyPath("Name"), Converter = new HistorianTagsListConverter() { Document = doc } };
                DetailsPathColumn.DisplayMemberBinding = new Binding { Path = new PropertyPath("."), Converter = new TagPathConverter() { Document = doc } };
            }

            Document.CreateUndoRedoHelper();
            Document.ChangedDocument += Document_ChangedDocument;

            tabHistoricalPrototypesTabControl.SelectionChanging += (s, e) =>
            {
                if (e.NewSelectedItem == gridControlTab &&
                    dpFlatGridRefresh != null && dpFlatGridRefresh.Status == DispatcherOperationStatus.Aborted)
                    FlatGridRefresh();
            };
            FlatGridRefresh();
            InitializeAddressSpace();
        }

        void Document_ChangedDocument(object sender, ChangedDocumentEvent e)
        {
            if (e.Source == this)
                return;

            var changedObjects = (from object c in e.ChangedObjects.AsParallel()
                                  where c is UFUAModel.UFUAHistorianSettings ||
                                  c is UFUAModel.UFUATag
                                  select c).ToList();

            if (changedObjects.Count > 0)
            {
                try
                {
                    UnSubscribeSelectionChangedEvent = true;
                    treeListControl.BeginDataUpdate();

                    foreach (var changedObject in changedObjects)
                    {
                        if (e.ChangedType == ChangedType.changed)
                        {
                            if (changedObject is UFUAModel.UFUATag)
                            {
                                var ufuatag = changedObject as UFUAModel.UFUATag;
                                UFUAModel.UFUATag nestedObject = null;
                                if (Document.UowContext != null && Document.UowContext.InTransaction)
                                    nestedObject = Document.UowContext.GetNestedObject(changedObject) as UFUAModel.UFUATag;
                                if (nestedObject == null || 
                                    nestedObject.HistorianSettings != ufuatag.HistorianSettings ||
                                    nestedObject.UseShared.Value != ufuatag.UseShared.Value)
                                {
                                    if (!String.IsNullOrEmpty(ufuatag.HistorianSettings))
                                    {
                                        var historian = Document.GetHistoricalSetting(ufuatag.HistorianSettings);
                                        if (historian != null && mapObjectToNode.ContainsKey(historian))
                                        {
                                            RefreshAsync(mapObjectToNode[historian]);
                                            FlatGridRefresh();
                                        }
                                    }
                                    if (nestedObject != null && !String.IsNullOrEmpty(nestedObject.HistorianSettings))
                                    {
                                        var historian = Document.GetHistoricalSetting(nestedObject.HistorianSettings);
                                        if (historian != null && mapObjectToNode.ContainsKey(historian))
                                        {
                                            RefreshAsync(mapObjectToNode[historian]);
                                            FlatGridRefresh();
                                        }
                                    }
                                }
                                if (nestedObject != null && nestedObject.PrototypeModel != ufuatag.PrototypeModel)
                                {
                                    if (ufuatag.SubPrototypeMembers != null && ufuatag.SubPrototypeMembers.Count > 0)
                                    {
                                        var members = (from c in ufuatag.SubPrototypeMembers[0].GetTagMembers().AsParallel()
                                                       where !c.UseShared.Value && mapObjectToNode.ContainsKey(c)
                                                       select c).ToList();

                                        if (members.Count > 0)
                                        {
                                            foreach (var member in members)
                                                RefreshAsync(mapObjectToNode[member].ParentNode);

                                            FlatGridRefresh();
                                        }
                                    }
                                }
                            }
                        }
                        else if (e.ChangedType == ChangedType.removed)
                        {
                            if (changedObject is UFUAModel.UFUATag)
                            {
                                var ufuatag = changedObject as UFUAModel.UFUATag;
                                if (ufuatag.SubPrototypeMembers != null && ufuatag.SubPrototypeMembers.Count > 0)
                                {
                                    var members = (from c in ufuatag.SubPrototypeMembers[0].GetTagMembers().AsParallel()
                                                   where !c.UseShared.Value && mapObjectToNode.ContainsKey(c)
                                                   select c).ToList();

                                    if (members.Count > 0)
                                    {
                                        foreach (var member in members)
                                        {
                                            var parent = mapObjectToNode[member].ParentNode;
                                            parent.Nodes.Remove(mapObjectToNode[member]);
                                        }

                                        FlatGridRefresh();
                                    }
                                }
                            }
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
                            if (changedObject is UFUAModel.UFUATag)
                            {
                                var ufuatag = changedObject as UFUAModel.UFUATag;
                                if (!String.IsNullOrEmpty(ufuatag.HistorianSettings))
                                {
                                    var historian = Document.GetHistoricalSetting(ufuatag.HistorianSettings);
                                    if (historian != null && mapObjectToNode.ContainsKey(historian))
                                        parent = mapObjectToNode[historian];
                                }
                                if (ufuatag.SubPrototypeMembers != null && ufuatag.SubPrototypeMembers.Count > 0)
                                {
                                    var members = (from c in ufuatag.SubPrototypeMembers[0].GetTagMembers().AsParallel()
                                                   where !c.UseShared.Value && !String.IsNullOrEmpty(c.HistorianSettings)
                                                   select c).ToList();

                                    if (members.Count > 0)
                                    {
                                        foreach (var member in members)
                                        {
                                            var historian = Document.GetHistoricalSetting(member.HistorianSettings);
                                            if (historian != null && mapObjectToNode.ContainsKey(historian) && mapObjectToNode[historian].WasExpanded())
                                                AddTreeItem(member, mapObjectToNode[historian]);
                                        }

                                        FlatGridRefresh();
                                    }
                                }
                            }
                            else if (changedObject is UFUAModel.UFUAHistorianSettings)
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
            else if (!UnSubscribeSelectionChangedEvent)
            {
                try
                {
                    UnSubscribeSelectionChangedEvent = true;

                    var selecteditems = treeListControl.GetTreeSelectedItems();

                    gridDataControl.SelectedItems.Clear();
                    selecteditems.ForEach(o => {
                        try
                        {
                            gridDataControl.SelectedItems.Add(o);
                        }
                        catch
                        { }
                    });
                }
                finally
                {
                    UnSubscribeSelectionChangedEvent = false;
                }
            }
        }

        internal void ForceSelectionChangedEvent()
        {
            treeListControl_SelectionChanged(this, null);
        }

        void InitializeAddressSpace()
        {
            openFolderImg = UFUAEditorManagerComponent.GetBitmapImage("OpenFolderSmall", true);
            closedFolderImg = UFUAEditorManagerComponent.GetBitmapImage("CloseFolderSmall", true);

            treeListView.Nodes.Clear();
            itemRoot = treeListControl.AddNode(new TagPathTreeItemControl(Properties.Resources.HistoricalRibbonTitle) { ResourceIcon = closedFolderImg }, Tag as TreeListNode, Document);
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

        internal void AddHistorianModel(UFUAModel.UFUAHistorianSettings historian)
        {
            Document.AddUndoAction(this, historian, UndoRedoAction.Added);
            var item = AddTreeItem(historian, itemRoot);
            treeListControl.ClearSelection();
            treeListControl.SelectNode(item);
            FlatGridRefresh(new List<UFUAModel.UFUAHistorianSettings> { historian });
        }

        private void FillItems(TreeListNode iRoot, UFUAModel.UFUAHistorianSettings historianRoot = null)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                ClearNodes(iRoot);
                using (new WaitCursor())
                {
                    if (historianRoot == null)
                    {
                        var historianList = Document.GetHistoricalSettings();

                        if (historianList != null)
                        {
                            foreach (var historian in historianList)
                                AddTreeItem(historian, iRoot);
                        }
                    }
                    else
                    {
                        var tags = Document.GetHistoricalSettingsTags(historianRoot.Name);
                        foreach (var tag in tags)
                            AddTreeItem(tag, iRoot);
                    }
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
            {
                mapObjectToNode.Add(e.Node.Tag, e.Node);
                if (e.Node.Tag is UFUAModel.UFUATag)
                    (e.Node.Tag as INotifyPropertyChanged).PropertyChanged += UFUATag_PropertyChanged;
            }
            else if (e.ChangeType == NodeChangeType.Remove && e.Node.Tag != null && e.Node.Tag != TreeListControlHelper.DummyNode)
                ClearObjectMapNode(e.Node);
        }

        void ClearObjectMapNode(TreeListNode node)
        {
            if (node.Tag != null && node.Tag != TreeListControlHelper.DummyNode && mapObjectToNode.ContainsKey(node.Tag))
            {
                mapObjectToNode.Remove(node.Tag);
                if (node.Tag is UFUAModel.UFUATag)
                    (node.Tag as INotifyPropertyChanged).PropertyChanged -= UFUATag_PropertyChanged;
                foreach (var child in node.Nodes)
                    ClearObjectMapNode(child);
            }
        }

        void UFUATag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (UnSubscribeSelectionChangedEvent)
                return;

            if (e.PropertyName == "HistorianSettings" || e.PropertyName == "UFUATagPrototype")
            {
                if (mapObjectToNode.ContainsKey(sender))
                {
                    RefreshAsync(mapObjectToNode[sender].ParentNode);
                    FlatGridRefresh();
                }
                if (!String.IsNullOrEmpty((sender as UFUAModel.UFUATag).HistorianSettings))
                {
                    var historian = Document.GetHistoricalSetting((sender as UFUAModel.UFUATag).HistorianSettings);
                    if (historian != null && mapObjectToNode.ContainsKey(historian))
                    {
                        RefreshAsync(mapObjectToNode[historian]);
                        FlatGridRefresh();
                    }
                }
            }
        }

        void ClearNodes(TreeListNode node)
        {
            node.Nodes.Clear();
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

            var ic = new TagPathTreeItemControl(tag);
            var newitem = treeListControl.AddNode(ic, parent, tag);

            bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (tag is UFUAModel.UFUAHistorianSettings)
            {
                SetBindingOnProp(newitem, tag, "Name");
                (newitem.Content as TreeItemControl).ResourceIcon = UFUAEditorManagerComponent.GetBitmapImage("UFUASHistoricalPrototypesSmall");
                if (!isPopup && NeedToBeExpanded(newitem))
                    treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);
            }
            else if (tag is UFUAModel.UFUATag)
            {
                var ufuatag = tag as UFUAModel.UFUATag;
                SetBindingOnProp(newitem, tag, "Name");
                SetBindingOnProp(newitem, tag, nameof(ufuatag.FolderPath), TagPathTreeItemControl.FolderPathProperty);
                var bmpImage = UFUAEditorManagerComponent.GetBitmapImage("UFUASVariableSmall");

                switch (ufuatag.ModelType)
                {
                    case UFUAModel.ModelType.Method:
                        bmpImage = UFUAEditorManagerComponent.GetBitmapImage("UFUASMethodSmall");
                        break;
                    case UFUAModel.ModelType.ObjectType:
                        bmpImage = UFUAEditorManagerComponent.GetBitmapImage("UFUASVariableTypeSmall");
                        break;
                }
                
                (newitem.Content as TreeItemControl).ResourceIcon = bmpImage;

                if (!bShiftDown && parent.Nodes.Count > maxItems)
                {
                    parent.Nodes.RemoveAt(0);
                    var maxitem = treeListControl.GetTreeItem(Properties.Resources.MaxItemCountVisibleReachedDoubleClick, parent);
                    if (maxitem != null)
                    {
                        parent.Nodes.Remove(maxitem);
                        parent.Nodes.Add(maxitem);
                    }
                    else
                        AddTreeItem(Properties.Resources.MaxItemCountVisibleReachedDoubleClick, parent);
                }
            }

            treeListControl.RefreshRow(newitem.RowHandle); //Otherwise node's header (ItemHeader) can disappear in certain conditions
            return newitem;
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

                if (item.Tag is UFUAModel.UFUAHistorianSettings)
                    FillItems(item, item.Tag as UFUAModel.UFUAHistorianSettings);

                treeListControl.EndDataUpdate();
            }
            finally
            {
                (item.Content as TreeItemControl).IsNodeExpanding = false;
            }
        }

        void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            if (e.Source.DataControl == treeListControl)
            {
                e.Handled = true;
                var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
                if (selected != null)
                {
                    if (isPopup && selected.Tag is UFUAModel.UFUAHistorianSettings)
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
            if (e.Source.DataControl == gridDataControl)
            {
                e.Handled = true;
                if (isPopup)
                {
                    var wnd = this.FindParent<Window>();
                    if (wnd != null)
                    {
                        wnd.DialogResult = true;
                        wnd.Close();
                    }
                }
                else
                {
                    if (!isPopup && Document.EditorManagerComponent.PropertyControl != null)
                        Document.EditorManagerComponent.PropertyControl.Activate();
                    else
                    {
                        var ufuahistoricalSettings = gridDataControl.SelectedItem as UFUAModel.UFUAHistorianSettings;
                        EditSettings(ufuahistoricalSettings);
                        FlatGridRefresh();
                    }
                }
            }
        }

        protected override bool IsUndoRedoSupported(object obj)
        {
            if (obj is UFUAModel.UFUATag)
            {
                var item = (obj as UFUAModel.UFUATag);
                if (!item.IsSubPrototypeMember || !item.UseShared.Value)
                    return true;
            }
            else
                return obj is IXPSimpleObject;

            return false;
        }

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
                var list = new List<DevExpress.Xpo.IXPSimpleObject>();
                foreach (var obj in objects)
                {
                    var parent = Document.UowContext.GetParentObject(obj);
                    if (parent != null)
                        list.Add(parent as DevExpress.Xpo.IXPSimpleObject);

                    if (parent is UFUAModel.UFUAHistorianSettings &&
                        obj is UFUAModel.UFUAHistorianSettings)
                    {
                        var oldObject = parent as UFUAModel.UFUAHistorianSettings;
                        var newObject = obj as UFUAModel.UFUAHistorianSettings;
                        if (oldObject.Name != newObject.Name)
                            Document.HistoricalSettingsNameReplace(oldObject.Name, newObject.Name);
                    }
                }

                if (list.Count > 0)
                    Document.AddUndoAction(this, list, UndoRedoAction.Changed);
            }
        }
        #endregion

        bool NeedToBeExpanded(TreeListNode item)
        {
            if (item.Tag is UFUAModel.UFUAHistorianSettings)
            {
                var hs = (UFUAModel.UFUAHistorianSettings)item.Tag;
                return (from tag in new XPQuery<UFUAModel.UFUATag>(Document.GetSession(), true)/*.AsParallel()*/ where tag.HistorianSettings == hs.Name orderby tag.Name select tag).FirstOrDefault() != null;
            }
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

        DispatcherOperation dpFlatGridRefresh;
        List<UFUAModel.UFUAHistorianSettings> itemsToSelect = null;
        internal void FlatGridRefresh(List<UFUAModel.UFUAHistorianSettings> selectItems = null)
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
                            gridDataControl.ItemsSource = Document.GetHistoricalSettings();

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
            var selected = gridDataControl.SelectedItem;
            if (selected is UFUAModel.UFUAHistorianSettings)
            {
                EditSettings((UFUAModel.UFUAHistorianSettings)selected);
                FlatGridRefresh();
            }
            else if (selected is UFUAModel.UFUATag)
            {
                EditTag((UFUAModel.UFUATag)selected);
                FlatGridRefresh();
            }
        }

        private void EditTag(UFUAModel.UFUATag ufuatag)
        {
            if (ufuatag == null)
                return;

            using (var uow = Document.BeginNestedUnitOfWork())
            {
                var newTagControl = new NewTag(Document)
                {
                    DataContext = uow.GetNestedObject(ufuatag)
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newTagControl)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "TagEditor"
                };
                if (Dialog.ShowDialog() == true)
                {
                    Document.AddUndoAction(this, ufuatag, UndoRedoAction.Changed);
                    uow.CommitChanges();
                    UpdateContextObjects();
                }
            }
        }

        internal void DeleteSelectedItems()
        {
            var historical = new List<UFUAModel.UFUAHistorianSettings>();
            var tags = new List<UFUAModel.UFUATag>();
            var listundo = new List<IXPSimpleObject>();
            var treeitems = new List<TreeListNode>();
            var undoAction = UndoRedoAction.Changed;

            if (gridControlTab.IsSelected)
            {
                var selectedItems = gridDetailsControl.SelectedItems;
                if (selectedItems.Count == 0)
                    selectedItems = gridDataControl.SelectedItems;
                foreach (var item in selectedItems)
                {
                    if (item is UFUAModel.UFUAHistorianSettings)
                        historical.Add(item as UFUAModel.UFUAHistorianSettings);
                    else if (item is UFUAModel.UFUATag)
                        tags.Add(item as UFUAModel.UFUATag);
                    if (IsUndoRedoSupported(item))
                        listundo.Add(item as IXPSimpleObject);
                    if (mapObjectToNode.ContainsKey(item))
                        treeitems.Add(mapObjectToNode[item]);
                }

                if (historical.Count > 0)
                    undoAction = UndoRedoAction.Removed;
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

                var bHasHist = (from TreeListNode node in treeitems where node.Tag is UFUAModel.UFUAHistorianSettings select node).FirstOrDefault() != null;
                if (bHasHist)
                    undoAction = UndoRedoAction.Removed;
            }

            var uiMsgBox = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
            if (uiMsgBox != null)
            {
                var bShowMessage = (from item in treeitems/*.AsParallel()*/
                                    where item.Tag is UFUAModel.UFUAHistorianSettings &&
                                    Document.IsHistoricalSettingsNameUsed((item.Tag as UFUAModel.UFUAHistorianSettings).Name)
                                    select item).ToList().Count > 0;

                if (!bShowMessage)
                {
                    bShowMessage = (from item in historical
                                    where Document.IsHistoricalSettingsNameUsed(item.Name)
                                    select item).ToList().Count > 0;
                }

                if (bShowMessage)
                {
                    using (new ResetCursor())
                    {
                        var ret = uiMsgBox.ShowYesNo(Properties.Resources.SelectedObjectsNotEmpty, CustomDialogIcons.Warning);
                        if (ret == CustomDialogResults.No)
                            return;
                    }
                }
            }

            if (listundo.Count > 0)
                Document.AddUndoAction(this, listundo, undoAction);

            if (gridControlTab.IsSelected)
            {
                foreach (var ufuahistoricalSettings in historical)
                    ufuahistoricalSettings.Delete();

                var view = gridDataControl.View.FocusedView;
                foreach (var tag in tags)
                {
                    var mrh = (view.DataControl as GridControl).GetMasterRowHandle();
                    RemoveHistorianFromTag(tag, (view.DataControl as GridControl).GetMasterGrid().GetRow(mrh) as UFUAModel.UFUAHistorianSettings);
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
            try
            {
                UnSubscribeSelectionChangedEvent = true;
                using (new AsyncWaitCursor())
                {
                    //var item = addressSpaceTree.SelectedItem as TreeViewItemAdv;
                    var parent = item.ParentNode ?? itemRoot;
                    if (item.Tag is UFUAModel.UFUAHistorianSettings)
                    {
                        var hs = item.Tag as UFUAModel.UFUAHistorianSettings;
                        parent.Nodes.Remove(item);
                        hs.Delete();
                    }
                    else if (item.Tag is UFUAModel.UFUATag)
                    {
                        //var tag = item.Tag as UFUAModel.UFUATag;
                        //parent.Nodes.Remove(item);
                        //RemoveHistorianFromTag(tag, item);
                        var tag = item.Tag as UFUAModel.UFUATag;
                        var parentHist = item.ParentNode.Tag as UFUAModel.UFUAHistorianSettings;
                        parent.Nodes.Remove(item);
                        if (parentHist != null)
                            RemoveHistorianFromTag(tag, parentHist, item.ParentNode);
                    }
                }
            }
            finally
            {
                UnSubscribeSelectionChangedEvent = false;
            }

            FlatGridRefresh();
        }

        void RemoveHistorianFromTag(UFUAModel.UFUATag tag, UFUAModel.UFUAHistorianSettings parentHist, TreeListNode node = null)
        {
            if (tag.HistorianSettings == null)
                return;

            using (var uow = Document.BeginNestedUnitOfWork())
            {
                var original = uow.GetNestedObject(tag);
                tag.HistorianSettings = null;
                Document.OnChangedDocument(this, ChangedType.changed, new List<object> { tag }, new List<object> { original });
            }
        }

        internal void CopySelectedToClipboard()
        {
            var lisths = new List<UFUAModel.UFUAHistorianSettings>();
            foreach (var item in gridDataControl.SelectedItems)
            {
                if (item is UFUAModel.UFUAHistorianSettings)
                    lisths.Add(item as UFUAModel.UFUAHistorianSettings);
            }

            Document.CleanClipbaord();
            Document.CopyListHistoriansToClipbaord(lisths);
            Document.CheckClipbaord();
        }

        internal void PasteFromClipboard()
        {
            List<TreeListNode> pastedNodes = new List<TreeListNode>();
            try
            {
                treeListControl.ClearSelection();
                UnSubscribeSelectionChangedEvent = true;
                treeListControl.BeginDataUpdate();

                var lisths = Document.PasteClipboardHistorians();

                pastedNodes.AddRange(treeListControl.AddGetNodesList(itemRoot, lisths, true, AddTreeItem));

                if (lisths.Count > 0)
                {
                    FlatGridRefresh();

                    // add list to undo manager
                    var listundo = new List<IXPSimpleObject>();
                    lisths.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
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

        internal UFUAModel.UFUAHistorianSettings GetSelectedHistorianParent()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUAModel.UFUAHistorianSettings)
                return selected.Tag as UFUAModel.UFUAHistorianSettings;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFUAModel.UFUAHistorianSettings)
                    return selected.Tag as UFUAModel.UFUAHistorianSettings;
            }

            return null;
        }

        internal TreeListNode GetSelectedHistorianParentItem()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUAModel.UFUAHistorianSettings)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFUAModel.UFUAHistorianSettings)
                    return selected;
            }

            return itemRoot;
        }

        internal void AddHistorianToTags(List<TagIdentifier> tags, UFUAModel.UFUAHistorianSettings targetDef = null, TreeListNode targetNode = null)
        {
            var nodesToSelect = new List<TreeListNode>();
            var changedTags = new List<UFUAModel.UFUATag>();
            try
            {
                UnSubscribeSelectionChangedEvent = true;
                treeListControl.BeginDataUpdate();
                using (new AsyncWaitCursor())
                {
                    if (targetDef == null)
                        targetDef = GetSelectedHistorianParent();
                    if (targetNode == null)
                        targetNode = GetSelectedHistorianParentItem();

                    if (targetDef == null || targetNode == null)
                        return;

                    var listundo = new List<IXPSimpleObject>();

                    foreach (var tagDef in tags)
                    {
                        var reference = tagDef.TagReference as UFUAModel.TagEntityReference;
                        if (reference != null)
                        {
                            var tag = Document.FindTagByEntityReference(reference);
                            if (tag != null && tag.HistorianSettings != targetDef.Name)
                            {
                                changedTags.Add(tag);
                                if (IsUndoRedoSupported(tag))
                                    listundo.Add(tag);
                                if (tag.HistorianSettings != null)
                                {
                                    var oldParentNode = (from TreeListNode node in itemRoot.Nodes where (node.Content as TreeItemControl).Header as string == tag.HistorianSettings select node).FirstOrDefault();
                                    if (oldParentNode != null && oldParentNode.Nodes.Count > 0 && oldParentNode.Nodes[0].Tag != TreeListControlHelper.DummyNode)
                                    {
                                        var oldTagNode = (from TreeListNode node in oldParentNode.Nodes where node.Tag == tag select node).FirstOrDefault();
                                        if (oldTagNode != null)
                                            oldParentNode.Nodes.Remove(oldTagNode);
                                    }
                                }
                                
                                var item = AddTreeItem(tag, targetNode);
                                if (item != null)
                                    nodesToSelect.Add(item);
                            }
                        }
                    }

                    if (listundo.Count > 0)
                        Document.AddUndoAction(this, listundo, UndoRedoAction.Changed);

                    if (changedTags.Count > 0)
                    {
                        var originalObjects = new List<object>();
                        using (var uow = Document.BeginNestedUnitOfWork())
                        {
                            foreach (var tag in changedTags)
                            {
                                originalObjects.Add(uow.GetNestedObject(tag));
                                tag.HistorianSettings = targetDef.Name;
                            }

                            Document.OnChangedDocument(this, ChangedType.changed, changedTags, originalObjects);
                        }

                        FlatGridRefresh();
                        Document.NeedsSave = true;
                    }
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

        internal UFUAModel.UFUAHistorianSettings GetSelectedItem()
        {
            return gridDataControl.SelectedItem as UFUAModel.UFUAHistorianSettings;
        }

        private void EditSettings(UFUAModel.UFUAHistorianSettings ufuahistoricalSettings)
        {
            if (ufuahistoricalSettings == null)
                return;

            using (var uow = Document.BeginNestedUnitOfWork())
            {
                var newHistoricalControl = new NewHistoricalSettings(Document)
                {
                    DataContext = uow.GetNestedObject(ufuahistoricalSettings)
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newHistoricalControl)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "EditHistorical"
                };
                if (Dialog.ShowDialog() == true)
                {
                    if ((newHistoricalControl.DataContext as UFUAModel.UFUAHistorianSettings).Name != ufuahistoricalSettings.Name)
                        Document.HistoricalSettingsNameReplace(ufuahistoricalSettings.Name, (newHistoricalControl.DataContext as UFUAModel.UFUAHistorianSettings).Name);

                    Document.AddUndoAction(this, ufuahistoricalSettings, UndoRedoAction.Changed);
                    uow.CommitChanges();
                    UpdateContextObjects();
                }
            }
        }

        private void OnShownEditor(object sender, EditorEventArgs e)
        {
            //if (e.Column.FieldName == "HistorianSettings")
            //{
            //    var settings = e.Column.EditSettings as ComboBoxEditSettings;
            //    List<String> historicalsList = new List<string>() { "" };
            //    foreach (var historical in Document.GetHistoricalSettings())
            //        historicalsList.Add(historical.Name);
            //    settings.ItemsSource = historicalsList;
            //}
            if (e.Column.FieldName == "UFUAEngineeringUnit")
            {
                var settings = e.Column.EditSettings as ComboBoxEditSettings;
                var euList = Document.GetEngineeringUnitNames();
                euList.Insert(0, "");
                settings.ItemsSource = euList;
            }
        }

        private void OnHiddenEditor(object sender, EditorEventArgs e)
        {
            Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(((DataViewBase)sender).DataControl.SelectedItem);
        }

        #endregion

        #region Undo/Redo

        internal void UndoAction()
        {
            UndoRedoAction action;
            var list = Document.UndoAction(this, out action);
            bool selectionchanged = false;
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
                            UndoRedoChangeAction(list);
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
                                    if (obj.Source is UFUAModel.UFUAHistorianSettings)
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
            var list = Document.RedoAction(this, out action);
            bool selectionchanged = false;
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
                            UndoRedoChangeAction(list);
                        }
                    }
                    break;
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
                                    if (obj.Source is UFUAModel.UFUAHistorianSettings)
                                    {
                                        var parent = itemRoot;
                                        var parentTag = Document.GetParentObject(obj);
                                        if (parentTag != null && mapObjectToNode.ContainsKey(parentTag))
                                            parent = mapObjectToNode[parentTag];
                                        var item = AddTreeItem(obj.Source, parent);
                                        if (item != null)
                                        {
                                            selectionchanged = true;
                                            treeListControl.SelectNode(item);
                                        }
                                    }
                                }
                                FlatGridRefresh();
                            }
                            finally
                            {
                                treeListControl.EndDataUpdate();
                                UnSubscribeSelectionChangedEvent = false;
                                if (selectionchanged)
                                    ForceSelectionChangedEvent();
                            }
                        }
                    }
                    break;
            }
        }

        void UndoRedoChangeAction(UndoRedoXpoDataCollection list)
        {
            bool selectionchanged = false;
            var items = new List<TreeListNode>();
            try
            {
                treeListControl.ClearSelection();
                UnSubscribeSelectionChangedEvent = true;
                treeListControl.BeginDataUpdate();
                foreach (var obj in list)
                {
                    if (obj.Source is UFUAModel.UFUATag)
                    {
                        var tag = obj.Source as UFUAModel.UFUATag;
                        var eUnit = Document.GetHistoricalSetting(tag.HistorianSettings);
                        if (eUnit != null && mapObjectToNode.ContainsKey(eUnit))
                            RefreshAsync(mapObjectToNode[eUnit]);
                    }
                    else if (mapObjectToNode.ContainsKey(obj.Source))
                    {
                        var parent = mapObjectToNode[obj.Source].ParentNode;
                        var item = treeListControl.GetTreeItem(obj.Source, parent);
                        if (item != null)
                        {
                            selectionchanged = true;
                            items.Add(item);
                        }
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

        #endregion

        #region Drag & Drop
        private void TreeListControl_DragStart(object sender, StartRecordDragEventArgs e)
        {
            e.AllowDrag = false;
            e.Handled = true;
            return;
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
                    if (!CanAssignHistorian(tag))
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

        private void TreeListControl_DragEnd(object sender, DevExpress.Xpf.Core.DropRecordEventArgs e)
        {
            e.Handled = true;
            var targetHistorian = (e.TargetRecord as TreeItemControl)?.TreeItemInnerObject as UFUAModel.UFUAHistorianSettings;
            var targetTreeNode = treeListView.GetNodeByContent(e.TargetRecord);
            if (targetHistorian != null && targetTreeNode != null)
            {
                using (var Cursor = new WaitCursor())
                {
                    //Dispatcher.InvokeIfRequired((Action)delegate
                    //{
                        var data = e.Data.GetData(typeof(RecordDragDropData)) as RecordDragDropData;
                        if (data == null)
                            return;

                        var listIdentifiers = new List<TagIdentifier>();
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
                                if ((instance != null || member != null) && CanAssignHistorian(member ?? instance))
                                {
                                    var tagRef = new UFUAModel.TagEntityReference(member == null ? instance.NodeId : member.NodeId, entity.RelativePath, entity.ResolvedNodeId, entity.HumanReadable);
                                    listIdentifiers.Add(new TagIdentifier(tagRef));
                                }
                            }
                        }

                        if (listIdentifiers.Count > 0)
                            AddHistorianToTags(listIdentifiers, targetHistorian, targetTreeNode);
                    //}, DispatcherPriority.ApplicationIdle);
                }
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

            if (dpFlatGridRefresh != null && dpFlatGridRefresh.Status != DispatcherOperationStatus.Aborted &&
                dpFlatGridRefresh.Status != DispatcherOperationStatus.Completed)
            {
                dpFlatGridRefresh.Abort();
            }

            Document.EditorManagerComponent.Workspace.ContextObject = null;
            Document.ChangedDocument -= Document_ChangedDocument;

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
            public UFUAModel.UFUAHistorianSettings HistAss;
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
                        if (node.Tag is UFUAModel.UFUAHistorianSettings)
                            HistAss = (UFUAModel.UFUAHistorianSettings)node.Tag;
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

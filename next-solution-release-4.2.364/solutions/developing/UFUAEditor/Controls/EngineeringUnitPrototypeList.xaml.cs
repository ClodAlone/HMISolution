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
using UFUAEditor.Helpers;
using System.ComponentModel;
using DevExpress.Xpf.Grid;
using System.Windows.Data;
using UFUAEditor.Converters;
using System.Windows.Media.Imaging;
using UFUAEditor.ComponentService;
using DevExpress.Xpf.Grid.TreeList;
using DevExpress.Xpf.Editors.Settings;
using DocumentManager.ComponentService;
using DevExpress.Xpf.Core;
using UFUAEditor.Extensions;
using System.Windows.Threading;
using DevExpress.Data.TreeList;
using TempVariablesManager.Helpers;
using UIMsgBoxAlertService.ComponentService;
using Utilities.Xpo.UndoRedo;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for EngineeringUnitPrototypeList.xaml
    /// </summary>
    public partial class EngineeringUnitPrototypeList : TreeViewEditorHelper, IEditableObject, IDisposable
    {
        #region Declarations
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
            e.CanExecute = IsAnyItemSelected(typeof(UFUAModel.UFUAEngineeringUnit));
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
            e.CanExecute = IsAnyItemSelected(typeof(UFUAModel.UFUAEngineeringUnit));
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
            e.CanExecute = Document.ClipboardContainsEngineeringUnits();
            e.Handled = !e.CanExecute;
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

                    if (parent is UFUAModel.UFUAEngineeringUnit &&
                        obj is UFUAModel.UFUAEngineeringUnit)
                    {
                        var oldObject = parent as UFUAModel.UFUAEngineeringUnit;
                        var newObject = obj as UFUAModel.UFUAEngineeringUnit;
                        if (oldObject.Name != newObject.Name)
                            Document.EngineeringUnitsNameReplace(oldObject.Name, newObject.Name);
                    }
                }

                if (list.Count > 0)
                    Document.AddUndoAction(this, list, UndoRedoAction.Changed);
            }
        }
        #endregion

        #region Address Space

        public EngineeringUnitPrototypeList(UFUAServerDocument doc)
        {
            InitializeComponent();
            SetTabsContent(treeListControl, gridDataControl);
            Document = doc;

            (gridDataControl.DetailDescriptor as DataControlDetailDescriptor).ItemsSourceBinding = new Binding { Path = new PropertyPath("Name"), Converter = new EUTagsListConverter() { Document = doc } };

            TagPathColumn.DisplayMemberBinding = new Binding { Path = new PropertyPath("TreeItemInnerObject"), Converter = new TagPathConverter() { Document = doc } };
            DetailsPathColumn.DisplayMemberBinding = new Binding { Path = new PropertyPath("."), Converter = new TagPathConverter() { Document = doc } };

            Document.CreateUndoRedoHelper();
            Document.ChangedDocument += Document_ChangedDocument;

            tabViewListTabControl.SelectionChanging += (s, e) =>
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
                                  where c is UFUAModel.UFUAEngineeringUnit ||
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
                                    nestedObject.UFUAEngineeringUnit != ufuatag.UFUAEngineeringUnit ||
                                    nestedObject.UseShared.Value != ufuatag.UseShared.Value)
                                {
                                    if (!String.IsNullOrEmpty(ufuatag.UFUAEngineeringUnit))
                                    {
                                        var eu = Document.GetEngineeringUnit(ufuatag.UFUAEngineeringUnit);
                                        if (eu != null && mapObjectToNode.ContainsKey(eu))
                                        {
                                            RefreshAsync(mapObjectToNode[eu]);
                                            FlatGridRefresh();
                                        }
                                    }
                                    if (nestedObject != null && !String.IsNullOrEmpty(nestedObject.UFUAEngineeringUnit))
                                    {
                                        var eu = Document.GetEngineeringUnit(nestedObject.UFUAEngineeringUnit);
                                        if (eu != null && mapObjectToNode.ContainsKey(eu))
                                        {
                                            RefreshAsync(mapObjectToNode[eu]);
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
                        if (e.ChangedType == ChangedType.removed)
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
                                if (!String.IsNullOrEmpty(ufuatag.UFUAEngineeringUnit))
                                {
                                    var eu = Document.GetEngineeringUnit(ufuatag.UFUAEngineeringUnit);
                                    if (eu != null && mapObjectToNode.ContainsKey(eu))
                                        parent = mapObjectToNode[eu];
                                }
                                if (ufuatag.SubPrototypeMembers != null && ufuatag.SubPrototypeMembers.Count > 0)
                                {
                                    var members = (from c in ufuatag.SubPrototypeMembers[0].GetTagMembers().AsParallel()
                                                   where !c.UseShared.Value && !String.IsNullOrEmpty(c.UFUAEngineeringUnit)
                                                   select c).ToList();

                                    if (members.Count > 0)
                                    {
                                        foreach (var member in members)
                                        {
                                            var eu = Document.GetEngineeringUnit(member.UFUAEngineeringUnit);
                                            if (eu != null && mapObjectToNode.ContainsKey(eu) && mapObjectToNode[eu].WasExpanded())
                                                AddTreeItem(member, mapObjectToNode[eu]);
                                        }

                                        FlatGridRefresh();
                                    }
                                }
                            }
                            else if (changedObject is UFUAModel.UFUAEngineeringUnit)
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

        void InitializeAddressSpace()
        {
            openFolderImg = UFUAEditorManagerComponent.GetBitmapImage("OpenFolderSmall", true);
            closedFolderImg = UFUAEditorManagerComponent.GetBitmapImage("CloseFolderSmall", true);

            treeListView.Nodes.Clear();
            itemRoot = treeListControl.AddNode(new TagPathTreeItemControl(Properties.Resources.EngineeringUnitPrototypes) { ResourceIcon = closedFolderImg }, Tag as TreeListNode, Document);
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

        internal void AddEU(UFUAModel.UFUAEngineeringUnit ufuaEU)
        {
            Document.AddUndoAction(this, ufuaEU, UndoRedoAction.Added);
            var item = AddTreeItem(ufuaEU, itemRoot);
            treeListControl.ClearSelection();
            treeListControl.SelectNode(item);
            FlatGridRefresh(new List<UFUAModel.UFUAEngineeringUnit> { ufuaEU });
        }

        private void FillItems(TreeListNode iRoot, UFUAModel.UFUAEngineeringUnit eunitRoot = null)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                ClearNodes(iRoot);
                using (new WaitCursor())
                {
                    if (eunitRoot == null)
                    {
                        var euList = Document.GetEngineeringUnits();

                        if (euList != null)
                        {
                            foreach (var eu in euList)
                                AddTreeItem(eu, iRoot);
                        }
                    }
                    else
                    {
                        var tags = Document.GetEngineeringUnitsTags(eunitRoot.Name);
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

        void treeListControl_SelectionChanged(object sender, TreeListSelectionChangedEventArgs e)
        {
            if (!UnSubscribeSelectionChangedEvent)
            {
                EndEdit();

                UpdateContextObjects(sender);
            }
        }

        bool NeedToBeExpanded(TreeListNode item)
        {
            if (item.Tag is UFUAModel.UFUAEngineeringUnit) 
            {
                var eu = (UFUAModel.UFUAEngineeringUnit)item.Tag;
                return (from tag in new XPQuery<UFUAModel.UFUATag>(Document.GetSession(), true)/*.AsParallel()*/ where tag.UFUAEngineeringUnit == eu.Name select tag).FirstOrDefault() != null;
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

            if (e.PropertyName == "UFUAEngineeringUnit" || e.PropertyName == "UFUATagPrototype")
            {
                if (mapObjectToNode.ContainsKey(sender))
                {
                    RefreshAsync(mapObjectToNode[sender].ParentNode);
                    FlatGridRefresh();
                }
                if (!String.IsNullOrEmpty((sender as UFUAModel.UFUATag).UFUAEngineeringUnit))
                {
                    var eu = Document.GetEngineeringUnit((sender as UFUAModel.UFUATag).UFUAEngineeringUnit);
                    if (eu != null && mapObjectToNode.ContainsKey(eu))
                    {
                        RefreshAsync(mapObjectToNode[eu]);
                        FlatGridRefresh();
                    }
                }
            }
        }

        void ClearNodes(TreeListNode node)
        {
            node.Nodes.Clear();
        }

        void OnGridSelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            e.Handled = true;
            if (!UnSubscribeSelectionChangedEvent)
            {
                EndEdit();

                UpdateContextObjects(sender);
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

            var ic = new TagPathTreeItemControl(tag);
            var newitem = treeListControl.AddNode(ic, parent, tag);

            bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (tag is UFUAModel.UFUAEngineeringUnit)
            {
                SetBindingOnProp(newitem, tag, "Name");
                (newitem.Content as TreeItemControl).ResourceIcon = UFUAEditorManagerComponent.GetBitmapImage("UFUASEngineeringUnitSmall");
                if (NeedToBeExpanded(newitem))
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

        void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            e.Handled = true;
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null)
            {
                if (Document.EditorManagerComponent.PropertyControl != null)
                    Document.EditorManagerComponent.PropertyControl.Activate();
                else
                    EditSelectedItem();
            }
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

                if (item.Tag is UFUAModel.UFUAEngineeringUnit)
                    FillItems(item, item.Tag as UFUAModel.UFUAEngineeringUnit);

                treeListControl.EndDataUpdate();
            }
            finally
            {
                (item.Content as TreeItemControl).IsNodeExpanding = false;
            }
        }

        DispatcherOperation dpFlatGridRefresh;
        List<UFUAModel.UFUAEngineeringUnit> itemsToSelect = null;
        internal void FlatGridRefresh(List<UFUAModel.UFUAEngineeringUnit> selectItems = null)
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
                            gridDataControl.ItemsSource = Document.GetEngineeringUnits();

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

        private void gridDataControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (Document.EditorManagerComponent.PropertyControl != null)
                Document.EditorManagerComponent.PropertyControl.Activate();
            else
            {
                var ufuahistoricalSettings = gridDataControl.SelectedItem as UFUAModel.UFUAEngineeringUnit;
                EditSettings(ufuahistoricalSettings);
                FlatGridRefresh();
            }
        }

        internal UFUAModel.UFUAEngineeringUnit GetSelectedEUParent()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUAModel.UFUAEngineeringUnit)
                return selected.Tag as UFUAModel.UFUAEngineeringUnit;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFUAModel.UFUAEngineeringUnit)
                    return selected.Tag as UFUAModel.UFUAEngineeringUnit;
            }

            return null;
        }

        internal TreeListNode GetSelectedEUParentItem()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUAModel.UFUAEngineeringUnit)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFUAModel.UFUAEngineeringUnit)
                    return selected;
            }

            return itemRoot;
        }

        private void OnShownEditor(object sender, EditorEventArgs e)
        {
            if (e.Column.FieldName == "HistorianSettings")
            {
                var settings = e.Column.EditSettings as ComboBoxEditSettings;
                List<String> historicalsList = new List<string>() { "" };
                foreach (var historical in Document.GetHistoricalSettings())
                    historicalsList.Add(historical.Name);
                settings.ItemsSource = historicalsList;
            }
            //if (e.Column.FieldName == "UFUAEngineeringUnit")
            //{
            //    var settings = e.Column.EditSettings as ComboBoxEditSettings;
            //    var euList = Document.GetEngineeringUnitNames();
            //    euList.Insert(0, "");
            //    settings.ItemsSource = euList;
            //}
        }

        private void OnHiddenEditor(object sender, EditorEventArgs e)
        {
            Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(((DataViewBase)sender).DataControl.SelectedItem);
        }

        internal void EditSelectedItem()
        {
            var selected = gridDataControl.SelectedItem as UFUAModel.UFUAEngineeringUnit;
            if (selected != null)
            {
                EditSettings(selected);
                FlatGridRefresh();
            }
        }

        internal void DeleteSelectedItems()
        {
            var historical = new List<UFUAModel.UFUAEngineeringUnit>();
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
                    if (item is UFUAModel.UFUAEngineeringUnit)
                        historical.Add(item as UFUAModel.UFUAEngineeringUnit);
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
                                    select item).ToList());

                listundo.AddRange((from item in treeitems
                                   where IsUndoRedoSupported(item.Tag) && 
                                   !treeitems.Contains(item.ParentNode)
                                   select item.Tag as IXPSimpleObject));

                var bHasEU = (from TreeListNode node in treeitems where node.Tag is UFUAModel.UFUAEngineeringUnit select node).FirstOrDefault() != null;
                if (bHasEU)
                    undoAction = UndoRedoAction.Removed;
            }

            var uiMsgBox = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
            if (uiMsgBox != null)
            {
                var bShowMessage = (from item in treeitems/*.AsParallel()*/
                                    where item.Tag is UFUAModel.UFUAEngineeringUnit &&
                                    Document.IsEngineeringUnitsNameUsed((item.Tag as UFUAModel.UFUAEngineeringUnit).Name)
                                    select item).ToList().Count > 0;
                
                if (!bShowMessage)
                {
                    bShowMessage = (from item in historical
                                    where Document.IsEngineeringUnitsNameUsed(item.Name)
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
                    RemoveEUFromTag(tag, (view.DataControl as GridControl).GetMasterGrid().GetRow(mrh) as UFUAModel.UFUAEngineeringUnit);
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
                    if (item.Tag is UFUAModel.UFUAEngineeringUnit)
                    {
                        var hs = item.Tag as UFUAModel.UFUAEngineeringUnit;
                        parent.Nodes.Remove(item);
                        hs.Delete();
                    }
                    else if (item.Tag is UFUAModel.UFUATag)
                    {
                        var tag = item.Tag as UFUAModel.UFUATag;
                        var parentEU = item.ParentNode.Tag as UFUAModel.UFUAEngineeringUnit;
                        parent.Nodes.Remove(item);
                        if (parentEU != null)
                            RemoveEUFromTag(tag, parentEU, parent);
                    }
                }
            }
            finally
            {
                UnSubscribeSelectionChangedEvent = false;
            }

            FlatGridRefresh();
        }

        internal void AddEUToTags(List<TagIdentifier> tags, UFUAModel.UFUAEngineeringUnit targetDef = null, TreeListNode targetNode = null)
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
                        targetDef = GetSelectedEUParent();
                    if (targetNode == null)
                        targetNode = GetSelectedEUParentItem();

                    if (targetDef == null || targetNode == null)
                        return;

                    var listundo = new List<IXPSimpleObject>();
                    
                    foreach (var tagDef in tags)
                    {
                        var reference = tagDef.TagReference as UFUAModel.TagEntityReference;
                        if (reference != null)
                        {
                            var tag = Document.FindTagByEntityReference(reference);
                            if (tag != null && tag.UFUAEngineeringUnit != targetDef.Name)
                            {
                                changedTags.Add(tag);
                                if (IsUndoRedoSupported(tag))
                                    listundo.Add(tag);
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
                        foreach (var tag in changedTags)
                            tag.UFUAEngineeringUnit = targetDef.Name;
                        
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

        void RemoveEUFromTag(UFUAModel.UFUATag tag, UFUAModel.UFUAEngineeringUnit ufuaEU, TreeListNode node = null)
        {
            if (tag.UFUAEngineeringUnit != ufuaEU.Name)
                return;

            tag.UFUAEngineeringUnit = null;
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

        internal void CopySelectedToClipboard()
        {
            var listeu = new List<UFUAModel.UFUAEngineeringUnit>();
            foreach (var item in gridDataControl.SelectedItems)
            {
                if (item is UFUAModel.UFUAEngineeringUnit)
                    listeu.Add(item as UFUAModel.UFUAEngineeringUnit);
            }

            Document.CleanClipbaord();
            Document.CopyListEngineeringUnitsToClipbaord(listeu);
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

                var listeu = Document.PasteClipboardEngineeringUnits();

                pastedNodes.AddRange(treeListControl.AddGetNodesList(itemRoot, listeu, true, AddTreeItem));

                if (listeu.Count > 0)
                {
                    FlatGridRefresh();

                    // add list to undo manager
                    var listundo = new List<IXPSimpleObject>();
                    listeu.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
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

        internal void ForceSelectionChangedEvent()
        {
            treeListControl_SelectionChanged(this, null);
        }

        internal UFUAModel.UFUAEngineeringUnit GetSelectedItem()
        {
            return gridDataControl.SelectedItem as UFUAModel.UFUAEngineeringUnit;
        }

        private void EditSettings(UFUAModel.UFUAEngineeringUnit engineeringUnit)
        {
            if (engineeringUnit == null)
                return;

            using (var uow = Document.BeginNestedUnitOfWork())
            {
                var newEngineeringControl = new NewEngineeringUnit()
                {
                    DataContext = uow.GetNestedObject(engineeringUnit)
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newEngineeringControl)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "EditEngineeringUnit"
                };
                if (Dialog.ShowDialog() == true)
                {
                    if ((newEngineeringControl.DataContext as UFUAModel.UFUAEngineeringUnit).Name != engineeringUnit.Name)
                        Document.EngineeringUnitsNameReplace(engineeringUnit.Name, (newEngineeringControl.DataContext as UFUAModel.UFUAEngineeringUnit).Name);
                    
                    Document.AddUndoAction(this, engineeringUnit, UndoRedoAction.Changed);
                    uow.CommitChanges();
                    UpdateContextObjects();
                }
            }
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
                                    if (obj.Source is UFUAModel.UFUAEngineeringUnit)
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
                                    if (obj.Source is UFUAModel.UFUAEngineeringUnit)
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
                        var eUnit = Document.GetEngineeringUnit(tag.UFUAEngineeringUnit);
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
                    if (!CanAssignEngineeringUnit(tag))
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
            var targetEU = (e.TargetRecord as TreeItemControl)?.TreeItemInnerObject as UFUAModel.UFUAEngineeringUnit;
            var targetTreeNode = treeListView.GetNodeByContent(e.TargetRecord);
            if (targetEU != null && targetTreeNode != null)
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
                                if ((instance != null || member != null) && CanAssignEngineeringUnit(member ?? instance))
                                {
                                    var tagRef = new UFUAModel.TagEntityReference(member == null ? instance.NodeId : member.NodeId, entity.RelativePath, entity.ResolvedNodeId, entity.HumanReadable);
                                    listIdentifiers.Add(new TagIdentifier(tagRef));
                                }
                            }
                        }

                        if (listIdentifiers.Count > 0)
                            AddEUToTags(listIdentifiers, targetEU, targetTreeNode);
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
            public UFUAModel.UFUAEngineeringUnit EUAss;
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
                        if (node.Tag is UFUAModel.UFUAEngineeringUnit)
                            EUAss = (UFUAModel.UFUAEngineeringUnit)node.Tag;
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

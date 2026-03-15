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
using System.ComponentModel;
using WPFUtilities;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using UFUAEditor.Helpers;
using UIMsgBoxAlertService.ComponentService;
using UFInterfaces.Editors;
using DocumentManager.ComponentService;
using SelectionMode = UFInterfaces.Editors.SelectionMode;
using DevExpress.Data.TreeList;
using Utilities.Xpo.UndoRedo;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for ProtototypeList.xaml
    /// </summary>
    public partial class PrototypeList : TreeViewEditorHelper, IEditableObject, ISelectEntityReference
    {
        #region Declarations
        
        TreeListNode itemRoot;
        BitmapImage openFolderImg;
        BitmapImage closedFolderImg;

        List<TreeListNode> selectedItems;
        bool isPopup;
        bool bLoaded;
        #endregion

        public PrototypeList(UFUAServerDocument doc, bool bPopup = false)
        {
            InitializeComponent();
            isPopup = bPopup;
            SetTabsContent(treeListControl);

            Document = doc;

            Document.CreateUndoRedoHelper();
            Document.ChangedDocument += Document_ChangedDocument;

            if (isPopup)
            {
                Loaded += (o, e) =>
                {
                    var wnd = this.FindParent<Window>();
                    if (wnd != null)
                    {
                        wnd.Activated += (s, c) =>
                        {
                            Document.CopyWinClipboardToInMemoryData();
                        };

                        wnd.Closing += (s, c) =>
                        {
                            var selected = selectedItems == null ? null : new List<TreeListNode>(selectedItems);
                            selectedItems = null;

                            List<UFUAModel.UFUATag> ufuatags = new List<UFUAModel.UFUATag>();
                            var list = new List<UFUAModel.UFUATag>();
                           
                            if (selected == null)
                                selected = treeListControl.GetSelectedNodes().ToList();

                            foreach (var sel in selected)
                            {
                                if (sel != null && sel.Tag is UFUAModel.UFUATag)
                                    ufuatags.Add((UFUAModel.UFUATag)sel.Tag);
                            }

                            if (ufuatags.Count > 0)
                            {
                                var referencesList = new List<TagIdentifier>();
                                foreach (var ufuatag in ufuatags)
                                {
                                    var entity = Document.GetTagOPCUAEntityReference(ufuatag, list);
                                    //if (SelectionType == UFInterfaces.Editors.SelectionType.TagEntityReference)
                                        referencesList.Add(new TagIdentifier(new UFUAModel.TagEntityReference(ufuatag.NodeId, entity.RelativePath, entity.ResolvedNodeId, entity.HumanReadable)));
                                    //else
                                    //    referencesList.Add(entity);
                                }
                                SelectedReference = null;
                                SelectedReferences = new List<object> (referencesList);
                            }
                            else
                            {
                                SelectedReference = null;
                                SelectedReferences = null;
                            }
                        };
                    }

                    if (!bLoaded)
                    {
                        bLoaded = true;
                        InitializeAddressSpace();
                    }
                };
            }
            else
            {
                Loaded += (o, e) =>
                {
                    if (!bLoaded)
                    {
                        bLoaded = true;
                        InitializeAddressSpace();
                    }
                };
            }
        }

        void Document_ChangedDocument(object sender, ChangedDocumentEvent e)
        {
            if (e.Source == this)
                return;

            var changedObjects = (from object c in e.ChangedObjects.AsParallel()
                                  where c is UFUAModel.UFUATagPrototype ||
                                  c is UFUAModel.UFUAArea ||
                                  c is UFUAModel.UFUAAlarmSource ||
                                  c is UFUAModel.UFUAAlarmDefinition ||
                                  c is UFUAModel.UFUAAlarmThreshold ||
                                  (c is UFUAModel.UFUATag && (c as UFUAModel.UFUATag).IsPrototypeMember) ||
                                  (c is UFUAModel.UFUAFolder && (c as UFUAModel.UFUAFolder).IsPrototypeMember)
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
                            if (changedObject is UFUAModel.UFUAArea || changedObject is UFUAModel.UFUAAlarmSource || changedObject is UFUAModel.UFUAAlarmDefinition)
                            {
                                var thresholds = new List<UFUAModel.UFUAAlarmThreshold>();
                                if (changedObject is UFUAModel.UFUAArea)
                                    thresholds.AddRange(Document.GetThresholdsList(changedObject as UFUAModel.UFUAArea));
                                else if (changedObject is UFUAModel.UFUAAlarmSource)
                                    thresholds.AddRange(Document.GetThresholdsList(changedObject as UFUAModel.UFUAAlarmSource));
                                else if (changedObject is UFUAModel.UFUAAlarmDefinition)
                                    thresholds.AddRange((changedObject as UFUAModel.UFUAAlarmDefinition).UFUAAlarmThresholds);
                                foreach (var threshold in thresholds)
                                {
                                    if (mapObjectToNode.ContainsKey(threshold))
                                    {
                                        var parent = mapObjectToNode[threshold].ParentNode;
                                        parent.Nodes.Remove(mapObjectToNode[threshold]);
                                    }
                                }
                            }
                            else if (mapObjectToNode.ContainsKey(changedObject))
                            {
                                var parent = mapObjectToNode[changedObject].ParentNode;
                                parent.Nodes.Remove(mapObjectToNode[changedObject]);
                            }
                        }
                        else if (e.ChangedType == ChangedType.added)
                        {
                            TreeListNode parent = null;
                            if (changedObject is UFUAModel.UFUATag && (changedObject as UFUAModel.UFUATag).UFUAFolder != null)
                            {
                                var folder = (changedObject as UFUAModel.UFUATag).UFUAFolder;
                                if (mapObjectToNode.ContainsKey(folder))
                                    parent = mapObjectToNode[folder];
                            }
                            else if (changedObject is UFUAModel.UFUATag && (changedObject as UFUAModel.UFUATag).UFUATagPrototype != null)
                            {
                                var prototype = (changedObject as UFUAModel.UFUATag).UFUATagPrototype;
                                if (mapObjectToNode.ContainsKey(prototype))
                                    parent = mapObjectToNode[prototype];
                            }
                            else if (changedObject is UFUAModel.UFUAFolder && (changedObject as UFUAModel.UFUAFolder).UFUAFolderAss != null)
                            {
                                var folder = (changedObject as UFUAModel.UFUAFolder).UFUAFolderAss;
                                if (mapObjectToNode.ContainsKey(folder))
                                    parent = mapObjectToNode[folder];
                            }
                            else if (changedObject is UFUAModel.UFUAFolder && (changedObject as UFUAModel.UFUAFolder).UFUATagPrototype != null)
                            {
                                var prototype = (changedObject as UFUAModel.UFUAFolder).UFUATagPrototype;
                                if (mapObjectToNode.ContainsKey(prototype))
                                    parent = mapObjectToNode[prototype];
                            }
                            else if (changedObject is UFUAModel.UFUATagPrototype)
                                parent = itemRoot;

                            if (parent != null && parent.WasExpanded())
                                AddTreeItem(changedObject, parent);
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

        #region Properties
        TargetType targetType = TargetType.None;
        public TargetType TargetType
        {
            get
            {
                if (!isPopup)
                    return TargetType.None;
                return targetType;
            }

            set
            {
                if (!isPopup)
                    return;
                targetType = value;
            }
        }
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
            e.CanExecute = IsAnyItemSelected();
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
            e.CanExecute = IsAnyItemSelected();
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
            e.CanExecute = Document.ClipboardContainsPrototypes() || Document.ClipboardContainsFolders() || Document.ClipboardContainsTags();
        }
        #endregion

        #region IEditableObject Members
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
                }

                if (list.Count > 0)
                    Document.AddUndoAction(this, list, UndoRedoAction.Changed);
            }
        }

        #endregion

        #region Address Space

        class rootHeader
        {
            public String Name { get; set; }
        }

        SelectionMode selectionMode = SelectionMode.SingleRow;
        public SelectionMode SelectionMode
        {
            get
            {
                if (!isPopup)
                    return SelectionMode.SingleRow;
                return selectionMode;
            }

            set
            {
                if (!isPopup || selectionMode == value)
                    return;
                selectionMode = value;

                if (selectionMode == SelectionMode.SingleRow)
                    treeListControl.SelectionMode = MultiSelectMode.None;
                else if (selectionMode == SelectionMode.MultipleRow)
                {
                    treeListControl.UnselectAll();
                    treeListControl.SelectionMode = MultiSelectMode.Row;
                }
            }
        }

        void InitializeAddressSpace()
        {
            openFolderImg = UFUAEditorManagerComponent.GetBitmapImage("OpenFolderSmall", true);
            closedFolderImg = UFUAEditorManagerComponent.GetBitmapImage("CloseFolderSmall", true);

            treeListView.Nodes.Clear();
            itemRoot = treeListControl.AddNode(new PrototypeTreeItemControl(Properties.Resources.PrototypeList) { ResourceIcon = closedFolderImg }, Tag as TreeListNode, Document);
            treeListControl.AddNode(null, itemRoot, TreeListControlHelper.DummyNode);

            //foreach (var prototype in Document.GetPrototypes())
            //    AddTreeItem(prototype, itemRoot);
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

        void treeListControl_SelectionChanged(object sender, TreeListSelectionChangedEventArgs e)
        {
            if (!isPopup && !UnSubscribeSelectionChangedEvent)
            {
                EndEdit();

                UpdateContextObjects(sender);
            }
        }

        private void FillPrototype(TreeListNode item, UFUAModel.UFUATagPrototype prototype)
        {
            prototype.EnsureUniqueMembersOrderId();
            ClearNodes(item);
            using (new WaitCursor())
            {
                var sortedMembers = (from c in prototype.Members orderby c.MemberOrderId ascending select c).ToList();
                foreach (var tag in sortedMembers)
                    AddTreeItem(tag, item);
                var sortedFolders = (from c in prototype.Folders orderby c.MemberOrderId ascending select c).ToList();
                foreach (var folder in sortedFolders)
                    AddTreeItem(folder, item);
            }
        }

        private void FillPrototype(TreeListNode item, UFUAModel.UFUAFolder root)
        {
            ClearNodes(item);
            using (new WaitCursor())
            {
                var sortedMembers = (from c in root.UFUATags orderby c.MemberOrderId ascending select c).ToList();
                foreach (var tag in sortedMembers)
                    AddTreeItem(tag, item);
                var sortedFolders = (from c in root.UFUAFolders orderby c.MemberOrderId ascending select c).ToList();
                foreach (var folder in sortedFolders)
                    AddTreeItem(folder, item);
            }
        }

        private void FillItems(TreeListNode itemRoot, UFUAModel.UFUATag tag = null)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                ClearNodes(itemRoot);
                using (new WaitCursor())
                {
                    if (tag == null)
                    {
                        foreach (var prototype in Document.GetPrototypes())
                            AddTreeItem(prototype, itemRoot);
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(tag.HistorianSettings))
                        {
                            var hs = Document.GetHistoricalSettings(tag.HistorianSettings);
                            if (hs != null)
                                AddTreeItem(hs, itemRoot);
                        }

                        //foreach (var alarm in tag.UFUAAlarmDefinition)
                        //    AddTreeItem(alarm, itemRoot);

                        foreach (var alarm in tag.UFUAAlarmThresholds)
                            AddTreeItem(alarm, itemRoot);
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
                if (e.Node.Tag is UFUAModel.UFUATagPrototype)
                    (e.Node.Tag as INotifyPropertyChanged).PropertyChanged += UFUATagPrototype_PropertyChanged;
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
                if (node.Tag is UFUAModel.UFUATagPrototype)
                    (node.Tag as INotifyPropertyChanged).PropertyChanged -= UFUATagPrototype_PropertyChanged;
                foreach (var child in node.Nodes)
                    ClearObjectMapNode(child);
            }
        }

        void UFUATag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (UnSubscribeSelectionChangedEvent)
                return;

            if (e.PropertyName == "HistorianSettings" || e.PropertyName == "UFUAViews")
            {
                if (mapObjectToNode.ContainsKey(sender))
                    RefreshAsync(mapObjectToNode[sender]);
            }
            else if (e.PropertyName == "DataType")
            {
                if (mapObjectToNode.ContainsKey(sender))
                    (mapObjectToNode[sender].Content as TreeItemControl).ResourceIcon = UFUAEditorManagerComponent.GetTagBitmapImage(sender as UFUAModel.UFUATag);
            }
            //else if (e.PropertyName == "MemberOrderId")
            //{
            //    if (mapObjectToNode.ContainsKey(sender))
            //        RefreshAsync(mapObjectToNode[sender].ParentNode);
            //}
        }

        void UFUATagPrototype_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "InvalidateMembersOrderId")
            {
                if (sender is UFUAModel.UFUATagPrototype)
                {
                    var protoype = sender as UFUAModel.UFUATagPrototype;
                    Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                    {
                        if (bDisposed)
                            return;

                        protoype.EnsureUniqueMembersOrderId();
                    });
                }
            }
        }

        void ClearNodes(TreeListNode node)
        {
            node.Nodes.Clear();
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

        bool NeedToBeExpanded(TreeListNode item)
        {
            if (item.Tag is UFUAModel.UFUATag)
            {
                var ufuatag = item.Tag as UFUAModel.UFUATag;
                if (!String.IsNullOrEmpty(ufuatag.HistorianSettings) ||
                    ufuatag.UFUAAlarmThresholds.Count > 0)
                return true;
            }
            else if (item.Tag is UFUAModel.UFUAFolder)
            {
                var folder = (UFUAModel.UFUAFolder)item.Tag;
                return (folder.UFUATags.Count > 0 || folder.UFUAFolders.Count > 0);
            }
            else if (item.Tag is UFUAModel.UFUATagPrototype)
            {
                var prototype = (UFUAModel.UFUATagPrototype)item.Tag;
                return (prototype.Members.Count > 0 || prototype.Folders.Count > 0);
            }

            return false;
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
                if (item.Tag is UFUAModel.UFUAFolder)
                {
                    UpdateFolderIcon(item, true);
                    FillPrototype(item, item.Tag as UFUAModel.UFUAFolder);
                }
                else if (item.Tag is UFUAModel.UFUATagPrototype)
                    FillPrototype(item, item.Tag as UFUAModel.UFUATagPrototype);
                else if (item.Tag is UFUAModel.UFUATag)
                    FillItems(item, item.Tag as UFUAModel.UFUATag);

                treeListControl.EndDataUpdate();
            }
            finally
            {
                (item.Content as TreeItemControl).IsNodeExpanding = false;
            }
        }

        internal TreeListNode AddTreeItem(Object tag, TreeListNode parent = null, int nPos = -1)
        {
            if (bDisposed)
                return null;

            if (parent == null)
                parent = itemRoot;

            if (!parent.IsExpanded && !(parent.Content as TreeItemControl).IsNodeExpanding)
            {
                parent.IsExpanded = true;
                var list = (from p in parent.Nodes where p.Tag == tag select p).ToList();
                if (list.Count > 0)
                    return list[0];
            }

            var ic = new PrototypeTreeItemControl(tag);

            TreeListNode newitem;
            if (parent.Nodes.Count < 1 || nPos < 0 || parent.Nodes.Count < nPos)
                newitem = treeListControl.AddNode(ic, parent, tag);
            else
                newitem = treeListControl.AddNode(ic, parent, tag, nPos);

            if (NeedToBeExpanded(newitem))
                treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);

            if (tag is UFUAModel.UFUAFolder)
            {
                var t = tag as UFUAModel.UFUAFolder;
                SetBindingOnProp(newitem, tag, "Name");
                SetBindingOnProp(newitem, tag, nameof(t.MemberOrderId), PrototypeTreeItemControl.MemberOrderIdProperty);
                (newitem.Content as TreeItemControl).ResourceIcon = closedFolderImg;
            }
            else if (tag is UFUAModel.UFUATagPrototype)
            {
                SetBindingOnProp(newitem, tag, "Name");
                (newitem.Content as TreeItemControl).ResourceIcon = UFUAEditorManagerComponent.GetBitmapImage("UFUASPrototypesSmall");
            }
            else if (tag is UFUAModel.UFUATag)
            {
                var t = tag as UFUAModel.UFUATag;
                SetBindingOnProp(newitem, tag, nameof(t.Name));
                SetBindingOnProp(newitem, tag, nameof(t.DataType), PrototypeTreeItemControl.DataTypeProperty);
                SetBindingOnProp(newitem, tag, nameof(t.MemberOrderId), PrototypeTreeItemControl.MemberOrderIdProperty);
                SetBindingOnProp(newitem, tag, nameof(t.ModelType), PrototypeTreeItemControl.ModelTypeProperty);
                SetBindingOnProp(newitem, tag, nameof(t.PrototypeName), PrototypeTreeItemControl.PrototypeNameProperty);
                SetBindingOnProp(newitem, tag, nameof(t.Alarms), PrototypeTreeItemControl.AlarmsProperty);
                SetBindingOnProp(newitem, tag, nameof(t.HistorianSettings), PrototypeTreeItemControl.HistorianSettingsProperty);
                (newitem.Content as TreeItemControl).ResourceIcon = UFUAEditorManagerComponent.GetTagBitmapImage(t);
            }
            else if (tag is UFUAModel.UFUAHistorianSettings)
            {
                SetBindingOnProp(newitem, tag, "Name");
                (newitem.Content as TreeItemControl).ResourceIcon = UFUAEditorManagerComponent.GetBitmapImage("UFUASHistoricalPrototypesSmall"); 
            }
            else if (tag is UFUAModel.UFUAAlarmThreshold)
            {
                var alarm = tag as UFUAModel.UFUAAlarmThreshold;
                SetBindingOnProp(newitem, tag, "Name");
                (newitem.Content as TreeItemControl).ResourceIcon = alarm.UFUAAlarmDefinitionRef == null || alarm.UFUAAlarmDefinitionRef.Severity > 0 ?
                    UFUAEditorManagerComponent.GetBitmapImage("UFUASAlarmPrototypeSmall") :
                    UFUAEditorManagerComponent.GetBitmapImage("UFUASMessagePrototypeSmall");
            }

            treeListControl.RefreshRow(newitem.RowHandle); //Otherwise node's header (ItemHeader) can disappear in certain conditions
            return newitem;
        }

        void SetBindingOnProp(TreeListNode node, object bindingSource, string sourcePropName, DependencyProperty targetDP = null)
        {
            if (targetDP == null)
                targetDP = TreeItemControl.ItemHeaderProperty;
            var myBinding = new Binding(sourcePropName);
            myBinding.Source = bindingSource;
            myBinding.Mode = BindingMode.OneWay;
            myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //myBinding.Converter = new TreeItemHeaderConverter(treeListControl, node);
            BindingOperations.SetBinding(node.Content as PrototypeTreeItemControl, targetDP, myBinding);
        }

        private void treeListControl_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (Document.EditorManagerComponent.PropertyControl != null)
                Document.EditorManagerComponent.PropertyControl.Activate();
            else
                EditSelectedItem();
        }

        internal bool IsAnyItemSelected()
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            return item != null && item != itemRoot;
        }

        internal void EditSelectedItem()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null)
            {
                if (selected.Tag is UFUAModel.UFUAFolder)
                {
                    var folder = selected.Tag as UFUAModel.UFUAFolder;

                    using (var uow = Document.BeginNestedUnitOfWork())
                    {
                        var newFolderControl = new NewFolder()
                        {
                            DataContext = uow.GetNestedObject(folder)
                        };
                        GeneralDialogContent Dialog = new GeneralDialogContent(newFolderControl, GeneralDialogButtons.OkCancelButtons)
                        {
                            Owner = this.FindParent<Window>(),
                            HelpLink = ""
                        };
                        if (Dialog.ShowDialog() == true)
                        {
                            Document.AddUndoAction(this, folder, UndoRedoAction.Changed);
                            uow.CommitChanges();
                            UpdateContextObjects();
                        }
                    }
                }
                else if (selected.Tag is UFUAModel.UFUAHistorianSettings)
                {
                    var hs = selected.Tag as UFUAModel.UFUAHistorianSettings;

                    using (var uow = Document.BeginNestedUnitOfWork())
                    {
                        var newhsControl = new NewHistoricalSettings(Document)
                        {
                            DataContext = uow.GetNestedObject(hs)
                        };
                        GeneralDialogContent Dialog = new GeneralDialogContent(newhsControl)
                        {
                            Owner = this.FindParent<Window>(),
                            HelpLink = "EditHistoricalSettings"
                        };
                        if (Dialog.ShowDialog() == true)
                        {
                            Document.AddUndoAction(this, hs, UndoRedoAction.Changed);
                            uow.CommitChanges();
                            UpdateContextObjects();
                        }
                    }
                }
                else if (selected.Tag is UFUAModel.UFUAAlarmThreshold)
                {
                    var alr = selected.Tag as UFUAModel.UFUAAlarmThreshold;

                    using (var uow = Document.BeginNestedUnitOfWork())
                    {
                        var newhsControl = new NewAlarmThreshold(Document)
                        {
                            DataContext = uow.GetNestedObject(alr)
                        };
                        GeneralDialogContent Dialog = new GeneralDialogContent(newhsControl)
                        {
                            Owner = this.FindParent<Window>(),
                            HelpLink = "EditAlarmThreshold"
                        };
                        if (Dialog.ShowDialog() == true)
                        {
                            Document.AddUndoAction(this, alr, UndoRedoAction.Changed);
                            uow.CommitChanges();
                            UpdateContextObjects();
                        }
                    }
                }
                else if (selected.Tag is UFUAModel.UFUATagPrototype)
                {
                    var prototype = selected.Tag as UFUAModel.UFUATagPrototype;

                    using (var uow = Document.BeginNestedUnitOfWork())
                    {
                        var newProtoypeControl = new NewPrototype()
                        {
                            DataContext = uow.GetNestedObject(prototype)
                        };
                        GeneralDialogContent Dialog = new GeneralDialogContent(newProtoypeControl)
                        {
                            Owner = this.FindParent<Window>(),
                            HelpLink = "EditTagPrototype"
                        };
                        if (Dialog.ShowDialog() == true)
                        {
                            Document.AddUndoAction(this, prototype, UndoRedoAction.Changed);
                            uow.CommitChanges();
                            UpdateContextObjects();
                        }
                    }
                }
                else if (selected.Tag is UFUAModel.UFUATag)
                {
                    var ufuatag = selected.Tag as UFUAModel.UFUATag;

                    using (var uow = Document.BeginNestedUnitOfWork())
                    {
                        var newTagControl = new NewTag(Document, true)
                        {
                            DataContext = uow.GetNestedObject(ufuatag)
                        };
                        GeneralDialogContent Dialog = new GeneralDialogContent(newTagControl)
                        {
                            Owner = this.FindParent<Window>(),
                            HelpLink = "EditTag"
                        };
                        if (Dialog.ShowDialog() == true)
                        {
                            Document.AddUndoAction(this, ufuatag, UndoRedoAction.Changed);
                            uow.CommitChanges();
                            UpdateContextObjects();
                        }
                    }
                }
            }
        }

        internal void DeleteSelectedItems()
        {
            var items = (from item in treeListControl.GetSelectedNodes()
                            where item.Tag != null
                            select item).ToList();

            var uiMsgBox = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
            if (uiMsgBox != null)
            {
                var bShowMessage = (from item in items/*.AsParallel()*/
                                    where (item.Tag is UFUAModel.UFUAFolder &&
                                    ((item.Tag as UFUAModel.UFUAFolder).UFUATags.Count > 0 ||
                                    (item.Tag as UFUAModel.UFUAFolder).UFUAFolders.Count > 0)) ||
                                    (item.Tag is UFUAModel.UFUATagPrototype &&
                                    ((item.Tag as UFUAModel.UFUATagPrototype).Members.Count > 0 ||
                                    (item.Tag as UFUAModel.UFUATagPrototype).Folders.Count > 0))
                                    select item).ToList().Count > 0;
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

            var listundo = (from item in items
                            where IsUndoRedoSupported(item.Tag) && !items.Contains(item.ParentNode)
                            select item.Tag as IXPSimpleObject).ToList();

            if (listundo.Count > 0)
                Document.AddUndoAction(this, listundo, UndoRedoAction.Removed);

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
        }

        internal void DeleteTreeItem(TreeListNode item)
        {
            var parent = item.ParentNode ?? itemRoot;
            if (item.Tag is UFUAModel.UFUATag)
            {
                var tag = item.Tag as UFUAModel.UFUATag;
                //GetSelectedParent(true).Items.Remove(item);
                parent.Nodes.Remove(item);
                tag.Delete();
            }
            else if (item.Tag is UFUAModel.UFUATagPrototype)
            {
                var prototype = item.Tag as UFUAModel.UFUATagPrototype;
                //GetSelectedParent(true).Items.Remove(item);
                parent.Nodes.Remove(item);
                prototype.Delete();
            }
            else if (item.Tag is UFUAModel.UFUAFolder)
            {
                var folder = item.Tag as UFUAModel.UFUAFolder;
                //GetSelectedParent(true).Items.Remove(item);
                parent.Nodes.Remove(item);
                folder.Delete();
            }
            else if (item.Tag is UFUAModel.UFUAAlarmThreshold)
            {
                var alarm = item.Tag as UFUAModel.UFUAAlarmThreshold;
                //var parent = GetSelectedParent(true);
                var tag = parent.Tag as UFUAModel.UFUATag;
                parent.Nodes.Remove(item);
                if (parent.Nodes.Count == 0 || parent.Nodes.Count == 1 && parent.Nodes[0] == TreeListControlHelper.DummyNode)
                    parent.IsExpanded = false;
                if (tag != null && tag.UFUAAlarmThresholds.Contains(alarm))
                {
                    tag.UFUAAlarmThresholds.Remove(alarm);
                    alarm.Delete();
                    tag.NotifyPropertyChanged("UFUAAlarmThresholds");
                }
            }
            else if (item.Tag is UFUAModel.UFUAHistorianSettings)
            {
                var hs = item.Tag as UFUAModel.UFUAHistorianSettings;
                //var parent = GetSelectedParent(true);
                var tag = parent.Tag as UFUAModel.UFUATag;
                parent.Nodes.Remove(item);
                if (parent.Nodes.Count == 0 || parent.Nodes.Count == 1 && parent.Nodes[0] == TreeListControlHelper.DummyNode)
                    parent.IsExpanded = false;
                if (tag != null)
                {
                    Document.AddUndoAction(this, tag, UndoRedoAction.Changed);
                    if (tag.HistorianSettings != null)
                    {
                        using (var uow = Document.BeginNestedUnitOfWork())
                        {
                            var original = uow.GetNestedObject(tag);
                            tag.HistorianSettings = null;
                            Document.OnChangedDocument(this, ChangedType.changed, new List<object> { tag }, new List<object> { original });
                        }
                    }
                }
            }
        }

        internal void CopySelectedToClipboard()
        {
            var listprototypes = new List<UFUAModel.UFUATagPrototype>();
            var listfolders = new List<UFUAModel.UFUAFolder>();
            var listtags = new List<UFUAModel.UFUATag>();
            var listalarms = new List<UFUAModel.UFUAAlarmThreshold>();
            foreach (var item in treeListControl.GetSelectedNodes())
            {
                if (item.Tag is UFUAModel.UFUATagPrototype)
                    listprototypes.Add(item.Tag as UFUAModel.UFUATagPrototype);
                else if (item.Tag is UFUAModel.UFUAFolder)
                    listfolders.Add(item.Tag as UFUAModel.UFUAFolder);
                else if (item.Tag is UFUAModel.UFUATag)
                    listtags.Add(item.Tag as UFUAModel.UFUATag);
                else if (item.Tag is UFUAModel.UFUAAlarmThreshold)
                    listalarms.Add(item.Tag as UFUAModel.UFUAAlarmThreshold);
            }

            Document.CleanClipbaord();
            Document.CopyListPrototypesToClipbaord(listprototypes);
            Document.CopyListFoldersToClipbaord(listfolders);
            Document.CopyListTagsToClipbaord(listtags);
            Document.CopyListAlarmThresholdsToClipbaord(listalarms);
            Document.CheckClipbaord();
        }

        internal void AddPrototypeList(List<IXPSimpleObject> list)
        {
            treeListControl.ClearSelection();
            var rootExpanded = itemRoot.IsExpanded || itemRoot.Nodes.Count > 0 && itemRoot.Nodes[0].Tag != TreeListControlHelper.DummyNode;
            AddGetNodesList(list, rootExpanded);
            Document.AddUndoAction(this, list, UndoRedoAction.Added);
        }

        internal void RemovePrototype(IXPSimpleObject obj)
        {
            treeListControl.ClearSelection();
            if (mapObjectToNode.ContainsKey(obj))
            {
                var item = mapObjectToNode[obj];
                var list = new List<IXPSimpleObject>();
                list.Add(obj);
                Document.AddUndoAction(this, list, UndoRedoAction.Removed);
                DeleteTreeItem(item);
            }
        }

        internal void PasteFromClipboard()
        {
            List<TreeListNode> pastedNodes = new List<TreeListNode>();
            try
            {
                var parentPrototype = GetSelectedPrototype();
                var parent = GetSelectedParent();
                var expanded = parent.IsExpanded || parent.Nodes.Count > 0 && parent.Nodes[0].Tag != TreeListControlHelper.DummyNode;

                var parenttag = GetSelectedParentTag();
                var expandedtag = parenttag != null && (parenttag.IsExpanded || parenttag.Nodes.Count > 0 && parenttag.Nodes[0].Tag != TreeListControlHelper.DummyNode);

                treeListControl.ClearSelection();
                UnSubscribeSelectionChangedEvent = true;
                treeListControl.BeginDataUpdate();

                var listprototypes = Document.PasteClipboardPrototypes();
                var rootExpanded = itemRoot.IsExpanded || itemRoot.Nodes.Count > 0 && itemRoot.Nodes[0].Tag != TreeListControlHelper.DummyNode;

                treeListControl.AddDummyNodeIfNeeded(parent, NeedToBeExpanded);
                pastedNodes.AddRange(AddGetNodesList(listprototypes, rootExpanded));

                UFUAModel.UFUATagPrototype prototype = null;
                UFUAModel.UFUAFolder folder = null;
                if (parent.Tag is UFUAModel.UFUATagPrototype)
                    prototype = parent.Tag as UFUAModel.UFUATagPrototype;
                else if (parent.Tag is UFUAModel.UFUAFolder)
                    folder = parent.Tag as UFUAModel.UFUAFolder;

                var listfolders = new List<UFUAModel.UFUAFolder>();
                if (listprototypes.Count == 0 && prototype != null)
                    listfolders = Document.PasteClipboardFolders(prototype);
                else if (listprototypes.Count == 0 && folder != null)
                    listfolders = Document.PasteClipboardFolders(folder);

                treeListControl.AddDummyNodeIfNeeded(parent, NeedToBeExpanded);
                pastedNodes.AddRange(AddGetNodesList(listfolders, expanded, parent, parentPrototype));

                var listtags = new List<UFUAModel.UFUATag>();
                if (listprototypes.Count == 0 && prototype != null)
                    listtags = Document.PasteClipboardTags(prototype);
                else if (listprototypes.Count == 0 && folder != null)
                    listtags = Document.PasteClipboardTags(folder);

                treeListControl.AddDummyNodeIfNeeded(parent, NeedToBeExpanded);
                pastedNodes.AddRange(AddGetNodesList(listtags, expanded, parent, parentPrototype));

                var listalarms = new List<UFUAModel.UFUAAlarmThreshold>();
                if (parenttag != null)
                {
                    //if (!expandedtag && !TreeListControlHelper.CanBeExpanded(parenttag))
                    //    treeListControl.AddNode(null, parenttag, TreeListControlHelper.DummyNode);

                    listalarms.AddRange(Document.PasteClipboardAlarmThresholds(parenttag.Tag as UFUAModel.UFUATag));

                    treeListControl.AddDummyNodeIfNeeded(parenttag, NeedToBeExpanded);
                    pastedNodes.AddRange(AddGetNodesList(listalarms, expandedtag, parenttag));
                }

                if (listprototypes.Count > 0 || listfolders.Count > 0 || listtags.Count > 0 || listalarms.Count > 0)
                {
                    //FlatGridRefresh();

                    // add list to undo manager
                    var listundo = new List<IXPSimpleObject>();
                    listprototypes.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
                    listfolders.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
                    listtags.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
                    listalarms.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
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

        List<TreeListNode> AddGetNodesList(IEnumerable<object> nodesList, bool bAddOrGet, TreeListNode parentNode = null, UFUAModel.UFUATagPrototype parentPrototype = null)
        {
            if (parentNode == null)
                parentNode = itemRoot;
            var items = new List<TreeListNode>();
            foreach (var n in nodesList)
            {
                TreeListNode item = null;
                if (mapObjectToNode.ContainsKey(n))
                    item = mapObjectToNode[n];
                if (item == null)
                {
                    if (bAddOrGet)
                        item = AddTreeItem(n, parentNode, -1);
                    else
                        item = treeListControl.GetTreeItem(n, parentNode);
                }
                if (item != null)
                    items.Add(item);
            }
            return items;
        }

        private void ForceSelectionChangedEvent()
        {
            treeListControl_SelectionChanged(this, null);
        }

        internal void AddPrototype(UFUAModel.UFUATagPrototype prototype)
        {
            prototype.Name = UFUAModel.Helpers.NameValidator.EnsureValidName(prototype.Name);
            Document.EnsureValidNodeId(prototype);
            Document.AddUndoAction(this, prototype, UndoRedoAction.Added);
            var item = AddTreeItem(prototype, itemRoot);
            treeListControl.ClearSelection();
            treeListControl.SelectNode(item);
        }

        internal void AddFolder(UFUAModel.UFUAFolder folder)
        {
            folder.Name = UFUAModel.Helpers.NameValidator.EnsureValidName(folder.Name);
            Document.AddUndoAction(this, folder, UndoRedoAction.Added);
            var item = AddTreeItem(folder, GetSelectedParent());
            treeListControl.ClearSelection();
            treeListControl.SelectNode(item);
        }

        internal void AddTag(UFUAModel.UFUATag tag)
        {
            tag.Name = UFUAModel.Helpers.NameValidator.EnsureValidName(tag.Name);
            Document.EnsureValidNodeId(tag);
            Document.AddUndoAction(this, tag, UndoRedoAction.Added);
            var item = AddTreeItem(tag, GetSelectedParent());
            treeListControl.ClearSelection();
            treeListControl.SelectNode(item);
        }

        public UFUAModel.UFUATag GetSelectedTag()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUAModel.UFUATag)
                return selected.Tag as UFUAModel.UFUATag;
            return null;
        }

        public UFUAModel.UFUATagPrototype GetSelectedPrototype()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUAModel.UFUATagPrototype)
                return selected.Tag as UFUAModel.UFUATagPrototype;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFUAModel.UFUATagPrototype)
                    return selected.Tag as UFUAModel.UFUATagPrototype;
            }

            return null;
        }

        public UFUAModel.UFUAFolder GetSelectedFolder()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUAModel.UFUAFolder)
                return selected.Tag as UFUAModel.UFUAFolder;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFUAModel.UFUAFolder)
                    return selected.Tag as UFUAModel.UFUAFolder;
            }

            return null;
        }

        Guid FindParentPrototypeNodeId(TreeListNode parent)
        {
            while (parent != null)
            {
                if (parent.Tag is UFUAModel.UFUATagPrototype)
                {
                    var prototype = (parent.Tag as UFUAModel.UFUATagPrototype);
                    return prototype.NodeId;
                }
                else if (parent.Tag is UFUAModel.UFUAFolder)
                {
                    var folder = parent.Tag as UFUAModel.UFUAFolder;
                    if (folder.PrototypeReference != null)
                        return folder.PrototypeReference.NodeId;
                }
                else if (parent.Tag is UFUAModel.UFUATag)
                {
                    var tag = parent.Tag as UFUAModel.UFUATag;
                    if (tag.PrototypeReference != null)
                        return tag.PrototypeReference.NodeId;
                }

                parent = parent.ParentNode;
            }

            return Guid.Empty;
        }

        internal void MoveMemberUp()
        {
            bool changed = false;

            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            var listundo = new List<IXPSimpleObject>();

            try
            {
                UnSubscribeSelectionChangedEvent = true;
                if (item != null && item.Tag is UFUAModel.UFUATag)
                {
                    changed = Document.MoveMemberUp(item.Tag as UFUAModel.UFUATag);
                    listundo.Add(item.Tag as IXPSimpleObject);
                }
                else if (item != null && item.Tag is UFUAModel.UFUAFolder)
                {
                    changed = Document.MoveMemberUp(item.Tag as UFUAModel.UFUAFolder);
                    listundo.Add(item.Tag as IXPSimpleObject);
                }
            }
            finally
            {
                UnSubscribeSelectionChangedEvent = false;
            }

            if (changed)
            {
                UpdateItemPositionUp(item);
                Document.AddUndoAction(this, listundo, UndoRedoAction.Changed);
            }
        }

        internal void MoveMemberDown()
        {
            bool changed = false;

            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            var listundo = new List<IXPSimpleObject>();

            try
            {
                UnSubscribeSelectionChangedEvent = true;
                if (item != null && item.Tag is UFUAModel.UFUATag)
                {
                    changed = Document.MoveMemberDown(item.Tag as UFUAModel.UFUATag);
                    listundo.Add(item.Tag as IXPSimpleObject);
                }
                else if (item != null && item.Tag is UFUAModel.UFUAFolder)
                {
                    changed = Document.MoveMemberDown(item.Tag as UFUAModel.UFUAFolder);
                    listundo.Add(item.Tag as IXPSimpleObject);
                }
            }
            finally
            {
                UnSubscribeSelectionChangedEvent = false;
            }

            if (changed)
            {
                UpdateItemPositionDown(item);
                Document.AddUndoAction(this, listundo, UndoRedoAction.Changed);
            }
        }

        internal void SetMemberPosition(int newpos)
        {
            bool changed = false;

            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            var listundo = new List<IXPSimpleObject>();

            try
            {
                UnSubscribeSelectionChangedEvent = true;
                if (item != null && item.Tag is UFUAModel.UFUATag)
                {
                    changed = Document.SetMemberPosition(item.Tag as UFUAModel.UFUATag, newpos);
                    listundo.Add(item.Tag as IXPSimpleObject);
                }
                else if (item != null && item.Tag is UFUAModel.UFUAFolder)
                {
                    changed = Document.SetMemberPosition(item.Tag as UFUAModel.UFUAFolder, newpos);
                    listundo.Add(item.Tag as IXPSimpleObject);
                }
            }
            finally
            {
                UnSubscribeSelectionChangedEvent = false;
            }

            if (changed)
            {
                UpdateItemPosition(item, newpos);
                Document.AddUndoAction(this, listundo, UndoRedoAction.Changed);
            }
        }

        void UpdateItemPositionUp(TreeListNode item)
        {
            var parent = item.ParentNode ?? itemRoot;
            int index = parent.Nodes.IndexOf(item);
            parent.Nodes.Remove(item);
            var newItem = AddTreeItem(item.Tag, parent, index - 1);
            if (newItem != null)
                treeListControl.SelectNode(newItem);
        }

        void UpdateItemPositionDown(TreeListNode item)
        {
            var parent = item.ParentNode ?? itemRoot;
            int index = parent.Nodes.IndexOf(item);
            parent.Nodes.Remove(item);
            var newItem = AddTreeItem(item.Tag, parent, index + 1);
            if (newItem != null)
                treeListControl.SelectNode(newItem);
        }

        void UpdateItemPosition(TreeListNode item, int newpos)
        {
            var parent = item.ParentNode ?? itemRoot;
            parent.Nodes.Remove(item);
            var newItem = AddTreeItem(item.Tag, parent, newpos);
            if (newItem != null)
                treeListControl.SelectNode(newItem);
        }

        public TreeListNode GetSelectedParent(bool bSkipSelected = false)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (!bSkipSelected && selected != null && 
                (selected.Tag is UFUAModel.UFUATagPrototype || selected.Tag is UFUAModel.UFUAFolder))
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && (selected.Tag is UFUAModel.UFUATagPrototype || 
                                         selected.Tag is UFUAModel.UFUAFolder))
                    return selected;
            }

            return itemRoot;
        }

        public TreeListNode GetSelectedParentTag(bool bSkipSelected = false)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (!bSkipSelected && selected != null && selected.Tag is UFUAModel.UFUATag)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFUAModel.UFUATag)
                    return selected;
            }

            return null;
        }

        internal bool CanAssignItem()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUAModel.UFUATag)
            {
                var tag = selected.Tag as UFUAModel.UFUATag;
                return CanAssignItem(tag);
            }

            return false;
        }

        internal void AssignItemToSelect(UFUAModel.UFUAHistorianSettings hs)
        {
            bool selectionchanged = false;
            try
            {
                UnSubscribeSelectionChangedEvent = true;
                treeListControl.BeginDataUpdate();
                using (new AsyncWaitCursor())
                {
                    var listItems = (from p in treeListControl.GetSelectedNodes()
                                     where p.Tag is UFUAModel.UFUATag
                                     select p).ToList();

                    var listundo = new List<IXPSimpleObject>();
                    listItems.ForEach(item => { listundo.Add(item.Tag as IXPSimpleObject); });
                    if (listundo.Count > 0)
                        Document.AddUndoAction(this, listundo, UndoRedoAction.Changed);

                    var changedObjects = new List<object>();
                    var originalObjects = new List<object>();
                    using (var uow = Document.BeginNestedUnitOfWork())
                    {
                        foreach (var selected in listItems)
                        {
                            var tag = selected.Tag as UFUAModel.UFUATag;
                            originalObjects.Add(uow.GetNestedObject(tag));
                            if (selected.Nodes.Count == 0 || selected.Nodes.Count == 1 && selected.Nodes[0] == TreeListControlHelper.DummyNode)
                            {
                                if (selected.Nodes.Count == 0)
                                    treeListControl.AddNode(null, selected, TreeListControlHelper.DummyNode);
                                tag.HistorianSettings = hs.Name;
                                changedObjects.Add(tag);
                                selected.IsExpanded = true;
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(tag.HistorianSettings))
                                {
                                    var hsRemove = Document.GetHistoricalSettings(tag.HistorianSettings);

                                    var list = (from p in selected.Nodes where p.Tag == hsRemove select p).ToList();
                                    if (list.Count > 0)
                                    {
                                        selected.Nodes.Remove(list[0]);
                                    }
                                }
                                tag.HistorianSettings = hs.Name;
                                changedObjects.Add(tag);
                                var item = AddTreeItem(hs, selected);
                                if (item != null)
                                {
                                    if (!selectionchanged)
                                        treeListControl.ClearSelection();

                                    selectionchanged = true;
                                    treeListControl.SelectNode(item);
                                }
                            }

                            var listSel = (from p in selected.Nodes where p.Tag == hs select p).ToList();
                            if (listSel.Count > 0)
                            {
                                selectionchanged = true;
                                treeListControl.SelectNode(listSel[0]);
                            }
                        }

                        if (changedObjects.Count > 0)
                            Document.OnChangedDocument(this, ChangedType.changed, changedObjects, originalObjects);
                    }
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
                UnSubscribeSelectionChangedEvent = false;
                if (selectionchanged)
                    ForceSelectionChangedEvent();
            }
        }

        internal void AssignItemsToSelect(IEnumerable<UFUAModel.UFUAAlarmDefinition> hs, AssignAlarmViewModel model)
        {
            bool selectionchanged = false;
            try
            {
                UnSubscribeSelectionChangedEvent = true;
                treeListControl.BeginDataUpdate();
                using (new AsyncWaitCursor())
                {
                    var listItems = (from p in treeListControl.GetSelectedNodes()
                                     where p.Tag is UFUAModel.UFUATag
                                     select p).ToList();

                    var listundo = new List<IXPSimpleObject>();
                    foreach (var selected in listItems)
                    {
                        var tag = selected.Tag as UFUAModel.UFUATag;
                        foreach (var alrDef in hs)
                        {
                            var list = Document.GetNewThresholdList(alrDef, model, tag);
                            list.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
                            tag.UFUAAlarmThresholds.AddRange(list);
                            tag.NotifyPropertyChanged("UFUAAlarmThresholds");
                            list.ForEach((alr) =>
                            {
                                var item = AddTreeItem(alr, selected);
                                if (item != null)
                                {
                                    if (!selectionchanged)
                                        treeListControl.ClearSelection();

                                    selectionchanged = true;
                                    treeListControl.SelectNode(item);
                                }
                            });
                        }
                    }

                    if (listundo.Count > 0)
                        Document.AddUndoAction(this, listundo, UndoRedoAction.Added);
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
                UnSubscribeSelectionChangedEvent = false;
                if (selectionchanged)
                    ForceSelectionChangedEvent();
            }
        }

        private void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            e.Handled = true;
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && isPopup && selected.Tag is UFUAModel.UFUATagPrototype)
            {
                var wnd = this.FindParent<Window>();
                if (wnd != null)
                {
                    e.Handled = true;
                    selectedItems = new List<TreeListNode> { selected };
                    wnd.DialogResult = true;
                    wnd.Close();
                }
            }
        }

        private void OnColumnSort(object sender, TreeListCustomColumnSortEventArgs e)
        {
            if (e.Column == MemberOrderColumn)
            {
                try
                {
                    var v1 = Convert.ToInt32(e.Value1);
                    var v2 = Convert.ToInt32(e.Value2);
                    e.Result = v1.CompareTo(v2);
                    e.Handled = true;
                }
                catch { }
            }
        }

        private void OnCanSelectRow(object sender, CanSelectRowEventArgs e)
        {
            e.CanSelectRow = true;
            if (TargetType == TargetType.None)
                return;

            var tag = treeListView.GetNodeByRowHandle(e.RowHandle).Tag as UFUAModel.UFUATag;
            if (tag == null)
                return;

            if (TargetType == TargetType.Historian)
                e.CanSelectRow = CanAssignHistorian(tag);
            else if (TargetType == TargetType.AlarmThreshold)
                e.CanSelectRow = CanAssignAlarm(tag);
            else if (TargetType == TargetType.EngineeringUnit)
                e.CanSelectRow = CanAssignEngineeringUnit(tag);
        }

        #endregion

        #region ISelectEntityReference
        public object SelectedReference { get; set; }

        public List<object> SelectedReferences { get; set; }

        public void BringIntoView(object selectedReference)
        {
            //throw new NotImplementedException();
        }
        #endregion

        #region Drag & Drop

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
                if (c is TreeItemControl)
                {
                    var innerOject = (c as TreeItemControl).TreeItemInnerObject;
                    if (innerOject != null && mapObjectToNode.ContainsKey(innerOject))
                        draggedNodes.Add(mapObjectToNode[innerOject]);
                }
            }

            bool bAllow = true;
            foreach (var dragItem in draggedNodes)
            {
                if (!(dragItem.Tag is UFUAModel.UFUATag) &&
                    !(dragItem.Tag is UFUAModel.UFUAFolder))
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
            e.Handled = true;
            var itemContent = (e.TargetRecord as TreeItemControl)?.TreeItemInnerObject;

            if (draggedNodes.Count() == 0 || itemContent == null || !mapObjectToNode.ContainsKey(itemContent))
                return;

            var targetElement = treeListView.GetNodeByContent(e.TargetRecord);
            if (targetElement == null || e.TargetRowHandle == itemRoot.RowHandle ||
                (!(targetElement.Tag is UFUAModel.UFUAFolder) && !(targetElement.Tag is UFUAModel.UFUATagPrototype)) ||
                (e.OriginalSource != treeListView))
            {
                return;
            }

            var draggingItems = new List<TreeListNode>();
            foreach (var dragItem in draggedNodes)
            {
                var parent = dragItem.ParentNode ?? itemRoot;
                if (parent == targetElement || dragItem == targetElement)
                    continue;

                if (dragItem.Tag is IXPSimpleObject)
                    draggingItems.Add(dragItem);
            }

            if (draggingItems.Count > 0)
            {
                var folder = targetElement.Tag as UFUAModel.UFUAFolder;
                var prototype = targetElement.Tag as UFUAModel.UFUATagPrototype;
                var mapStartCounter = new Dictionary<string, ulong>();
                var listTagName = new List<String>();
                var listFolderName = new List<String>();
                if (folder != null)
                {
                    listTagName.AddRange(Document.GetTagsNameList(folder));
                    listFolderName.AddRange(Document.GetFoldersNameList(folder));
                }
                if (prototype != null)
                {
                    listTagName.AddRange(Document.GetTagsNameList(prototype));
                    listFolderName.AddRange(Document.GetFoldersNameList(prototype));
                }

                var atLeastOneNameAlreadyExists = (from c in draggingItems
                                                   where (c.Tag is UFUAModel.UFUATag && listTagName.Contains((c.Tag as UFUAModel.UFUATag).Name) ||
                                                   (c.Tag is UFUAModel.UFUAFolder && listFolderName.Contains((c.Tag as UFUAModel.UFUAFolder).Name)))
                                                   select c).Any();

                if (atLeastOneNameAlreadyExists)
                {
                    var uiInterface = Document.EditorManagerComponent.UIInterface;
                    if (uiInterface == null ||
                        uiInterface.ShowYesNo(Properties.Resources.DropAlreadyExistsWarning.Replace("-newline-", Environment.NewLine),
                        UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question) == UIMsgBoxAlertService.ComponentService.CustomDialogResults.No)
                    {
                        return;
                    }
                }

                var listundo = (from item in draggingItems
                                where IsUndoRedoSupported(item.Tag) && !draggingItems.Contains(item.ParentNode)
                                select item.Tag as IXPSimpleObject).ToList();

                if (listundo.Count > 0)
                    Document.AddUndoAction(this, listundo, UndoRedoAction.Removed);

                var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(Document.GetSession(), Document.GetSession(), false, true, true);
                foreach (var dragItem in draggingItems)
                {
                    var parent = dragItem.ParentNode ?? itemRoot;
                    if (dragItem.Tag is UFUAModel.UFUATag)
                    {
                        parent.Nodes.Remove(dragItem);
                        var item = cloneHelper.Clone(dragItem.Tag as UFUAModel.UFUATag);
                        (dragItem.Tag as UFUAModel.UFUATag).Delete();
                        dragItem.Tag = item;
                        if (folder != null)
                        {
                            item.Name = Document.NewTagName(folder, item.Name, mapStartCounter, listTagName);
                            item.UFUATagPrototype = null;
                            item.UFUAFolder = folder;
                            Document.EnsureValidNodeId(item);
                        }
                        if (prototype != null)
                        {
                            item.Name = Document.NewTagName(prototype, item.Name, mapStartCounter, listTagName);
                            item.UFUAFolder = null;
                            item.UFUATagPrototype = prototype;
                            Document.EnsureValidNodeId(item);
                        }
                    }
                    else if (dragItem.Tag is UFUAModel.UFUAFolder)
                    {
                        parent.Nodes.Remove(dragItem);
                        var item = cloneHelper.Clone(dragItem.Tag as UFUAModel.UFUAFolder);
                        (dragItem.Tag as UFUAModel.UFUAFolder).Delete();
                        dragItem.Tag = item;
                        item.UFUAFolderAss = folder;
                        if (folder != null)
                        {
                            item.Name = Document.NewFolderName(folder, item.Name, mapStartCounter, listFolderName);
                            item.UFUATagPrototype = null;
                            item.UFUAFolderAss = folder;
                            Document.EnsureValidNodeId(item);
                        }
                        if (prototype != null)
                        {
                            item.Name = Document.NewFolderName(prototype, item.Name, mapStartCounter, listFolderName);
                            item.UFUAFolderAss = null;
                            item.UFUATagPrototype = prototype;
                            Document.EnsureValidNodeId(item);
                        }
                    }
                }

                listundo = (from item in draggingItems
                            where IsUndoRedoSupported(item.Tag) && !draggingItems.Contains(item.ParentNode)
                            select item.Tag as IXPSimpleObject).ToList();

                if (listundo.Count > 0)
                    Document.AddUndoAction(this, listundo, UndoRedoAction.Added);

                var expanded = targetElement.IsExpanded || targetElement.Nodes.Count > 0 && targetElement.Nodes[0].Tag != TreeListControlHelper.DummyNode;
                treeListControl.ClearSelection();
                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    try
                    {
                        treeListControl.BeginDataUpdate();
                        treeListControl.AddDummyNodeIfNeeded(targetElement, NeedToBeExpanded);
                        draggingItems.ForEach(itemAdv =>
                        {
                            var tag = itemAdv.Tag as IXPSimpleObject;
                            if (expanded)
                            {
                                var item = AddTreeItem(tag, targetElement);
                                if (item != null)
                                    treeListControl.SelectNode(item);
                            }
                            else
                            {
                                var item = treeListControl.GetTreeItem(tag, targetElement);
                                if (item != null)
                                    treeListControl.SelectNode(item);
                            }
                        });
                    }
                    finally
                    {
                        treeListControl.EndDataUpdate();
                    }
                });
            }
        }

        void TreeListControl_CompletedDragDrop(object sender, CompleteRecordDragDropEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        #region Undo/Redo

        protected override bool IsUndoRedoSupported(object obj)
        {
            return obj is UFUAModel.UFUAFolder || obj is UFUAModel.UFUATag || obj is UFUAModel.UFUATagPrototype || obj is UFUAModel.UFUAAlarmThreshold;
        }

        internal void UndoAction()
        {
            bool selectionchanged = false;
            UndoRedoAction action;
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
                                //FlatGridRefresh();
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
                                        {
                                            Refresh(item);
                                            selectionchanged = true;
                                            treeListControl.SelectNode(item);
                                        }
                                    }
                                }
                                //FlatGridRefresh();
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
                                //FlatGridRefresh();
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

        internal void RedoAction()
        {
            bool selectionchanged = false;
            UndoRedoAction action;
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
                                //FlatGridRefresh();
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
                                        {
                                            Refresh(item);
                                            selectionchanged = true;
                                            treeListControl.SelectNode(item);
                                        }
                                    }
                                }
                                //FlatGridRefresh();
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
                                //FlatGridRefresh();
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
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using UFRecipeEditor.Controls;
using Utilities;
using Utilities.WPF;
using System.Windows.Media.Imaging;
using UFRecipeEditor.ComponentService;
using System.IO;
using System.Xml;
using System.Text;
using UIMsgBoxAlertService.ComponentService;
using VFS;
using System.Windows.Threading;
using DocumentManager.ComponentService;
using UFRecipeSettings.Documents;
using UFRecipeLayout.Helpers;
using UFRecipeSettings.UFRecipeModel;
using WPFUtilities;
using UFRecipeEditor.UndoRedo;
using System.Threading.Tasks;
using System.Threading;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using System.Windows.Data;
using UFProjectManager.ComponentService;
using DevExpress.Xpf.Core;
using DevExpress.Data.TreeList;
using DevExpress.Xpo;

namespace UFRecipeEditor
{
    /// <summary>
    /// Interaction logic for UFRecipeEditorUI.xaml
    /// </summary>
    public partial class UFRecipeEditorUI : UserControl, IEditableObject, IDisposable
    {
        #region Declarations

        protected readonly Dictionary<Object, TreeListNode> mapObjectToNode = new Dictionary<Object, TreeListNode>();
        TreeListNode itemRoot;
        BitmapImage openFolderImg;
        BitmapImage closedFolderImg;
        bool UnSubscribeSelectionChangedEvent;

        CancellationTokenSource cts;
        
        bool disableUndoRedoAction;

        readonly RecipeEditorManagerComponent EditorComponent;

        readonly UndoRedoManager undoRedoManager = new UndoRedoManager(100);
        readonly UndoRedoDataObject lastLayoutState = new UndoRedoDataObject(UndoRedoAction.LayoutState);

        #endregion

		bool bLoaded;
        public bool IsLoaded
        {
            get
            {
                return bLoaded;
            }
        }

        public UFRecipeEditorUI(RecipeEditorManagerComponent editorComponent, UFRecipeDocument doc)
        {
            EditorComponent = editorComponent;
            Document = doc;

            InitializeComponent();

            if (editorComponent.IsLayoutEditorHidden(doc))
                layoutEditorTab.Visibility = Visibility.Collapsed;

            InitializeAddressSpace();
            InitializeLayoutControl();

            EditorComponent.Workspace.DockItemRestored += Workspace_DockItemRestored;

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    SaveLayoutState();
                    // Subscribe layout control event
                    layoutEditorControl.LayoutItems.Controller.ModelChanged += layoutItems_ModelChanged;
                    layoutEditorControl.LayoutItems.AvailableItems.CollectionChanged += layoutItems_CollectionChanged;
                    layoutEditorControl.LayoutItems.IsCustomizationChanged += layoutItems_IsCustomizationChanged;
                    layoutEditorControl.LayoutItems.PreviewKeyDown += layoutControl_PreviewKeyDown;
                }
            };
            Unloaded += (o, e) =>
            {
                if (bLoaded)
                {
                    bLoaded = false;
                    // Unsubscribe layout control event
                    layoutEditorControl.LayoutItems.PreviewKeyDown -= layoutControl_PreviewKeyDown;
                    layoutEditorControl.LayoutItems.IsCustomizationChanged -= layoutItems_IsCustomizationChanged;
                    layoutEditorControl.LayoutItems.AvailableItems.CollectionChanged -= layoutItems_CollectionChanged;
                    layoutEditorControl.LayoutItems.Controller.ModelChanged -= layoutItems_ModelChanged;
                }
            };
        }

        private void Workspace_DockItemRestored(object sender, EventArgs e)
        {
            if (bIsActive)
                layoutEditorControl.LayoutItems.IsCustomization = true;
        }

        private void DXTabControl_SelectionChanged(object sender, DevExpress.Xpf.Core.TabControlSelectionChangedEventArgs e)
        {
            EndEdit();

            if (layoutEditorTab.IsSelected)
                layoutEditorControl.LayoutItems.IsCustomization = true;
            CheckAndAssignWorkspaceContext();
        }

        internal bool IsChildElement(object element)
        {
            if (element is UFRecipeEntity)
            {
                return Document.RecipeEntity == element;
            }
            else if (element is UFGroupEntity)
            {
                var list = Document.GetGroupsCollection();
                return list.Contains(element as UFGroupEntity);
            }
            else if (element is UFDataValueEntity)
            {
                var list = Document.RecipeEntity.GetFlatDataValuesCollection();
                return list.Contains(element as UFDataValueEntity);
            }
            else if (element is FrameworkElement)
            {
                var fe = element as FrameworkElement;
                return LayoutHelper.IsChildElement(layoutEditorControl.LayoutItems, fe);
            }

            return false;
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

        #region Drag & Drop

        private void treeListControl_DragEnd(object sender, DropRecordEventArgs e)
        {
            e.Handled = true;
            var targetContent = (e.TargetRecord as TreeItemControl)?.TreeItemInnerObject;
            var targetUFDataValueEntity = (e.TargetRecord as TreeItemControl)?.TreeItemInnerObject as UFDataValueEntity;
            var targetUFGroupEntity = (e.TargetRecord as TreeItemControl)?.TreeItemInnerObject as UFGroupEntity;
            var bDroppingOnRoot = itemRoot.RowHandle == e.TargetRowHandle;
            var targetTreeNode = treeListView.GetNodeByContent(e.TargetRecord);

            if (targetContent != null && targetTreeNode != null && (mapObjectToNode.ContainsKey(targetContent) || bDroppingOnRoot)
                && e.OriginalSource == treeListView)
            {
                using (var Cursor = new WaitCursor())
                {
                    var data = e.Data.GetData(typeof(RecordDragDropData)) as RecordDragDropData;
                    if (data == null)
                        return;

                    var listcolumns = new List<IXPSimpleObject>(data.Records.Count());

                    UnSubscribeSelectionChangedEvent = true;
                    var nodesToSelect = new List<TreeListNode>();
                    var undoDataObject = new UndoRedoDataObject(UndoRedoAction.Added);
                    var undoChangedDataObject = new UndoRedoDataObject(UndoRedoAction.Changed);
                    try
                    {
                        treeListControl.ClearSelection();
                        treeListControl.BeginDataUpdate();

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
                                if (targetUFDataValueEntity != null)
                                {
                                    var tag = model.BrowsableContent as UFUAModel.UFUATag;
                                    if (tag != null && tag.DataType.HasValue)
                                    {
                                        targetUFDataValueEntity.DataType = tag.DataType.Value;
                                        targetUFDataValueEntity.InheritDataTypeFromTag = true; //When drag & drop, automatically the property has to be set to true
                                    }
                                    if (tag != null)
                                        targetUFDataValueEntity.ArrayDimension = tag.ArrayDimension;
                                    undoChangedDataObject.AddDataObject(targetUFDataValueEntity);
                                    targetUFDataValueEntity.TagIODataValue = entity;
                                    nodesToSelect.Add(targetTreeNode);
                                }
                                else
                                {
                                    var daragableChilds = GetDragableChildDocumentManagers(model);
                                    if (daragableChilds.Count > 0)
                                    {
                                        daragableChilds.ForEach((child) =>
                                        {
                                            var tag = child.BrowsableContent as UFUAModel.UFUATag;
                                            if (!tag.IsObjectType && CanAssignTag(tag))
                                            {
                                                var newDataValue = Document.AddNewDataValue(targetUFGroupEntity, tag?.Name);
                                                newDataValue.TagIODataValue = child.DragContent as OPCUAViewModel.OPCUAEntityReference;
                                                if (tag != null && tag.DataType.HasValue)
                                                    newDataValue.DataType = tag.DataType.Value;
                                                if (tag != null)
                                                    newDataValue.ArrayDimension = tag.ArrayDimension;
                                                var newitem = AddTreeItem(newDataValue, targetTreeNode, -1);
                                                undoDataObject.AddDataObject(newDataValue);
                                                if (newitem != null)
                                                    nodesToSelect.Add(newitem);
                                            }
                                        });
                                    }
                                    else
                                    {
                                        var tag = model.BrowsableContent as UFUAModel.UFUATag;
                                        var newDataValue = Document.AddNewDataValue(targetUFGroupEntity, tag?.Name);
                                        newDataValue.TagIODataValue = entity;
                                        if (tag != null && tag.DataType.HasValue)
                                            newDataValue.DataType = tag.DataType.Value;
                                        if (tag != null)
                                            newDataValue.ArrayDimension = tag.ArrayDimension;
                                        var newitem = AddTreeItem(newDataValue, targetTreeNode, -1);
                                        undoDataObject.AddDataObject(newDataValue);
                                        if (newitem != null)
                                            nodesToSelect.Add(newitem);
                                    }
                                }
                            }
                        }
                        if(nodesToSelect.Count > 0)
                        {
                            if (targetUFDataValueEntity != null)
                                undoRedoManager.AddUndoAction(undoChangedDataObject);
                            else
                                undoRedoManager.AddUndoAction(undoDataObject);
                        }

                        treeListControl.AddDummyNodeIfNeeded(targetTreeNode, NeedToBeExpanded);
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
            }
        }

        List<IDocumentManager> GetDragableChildDocumentManagers(IDocumentManager root)
        {
            var ret = new List<IDocumentManager>();
            if (!(root.BrowsableContent is UFUAModel.UFUATag) ||
                !(root.BrowsableContent as UFUAModel.UFUATag).IsObjectType ||
                !CanAssignTag(root.BrowsableContent as UFUAModel.UFUATag))
                return ret;

            ret.AddRange((from child in root.GetChildDocumentManagers()
                          where child.BrowsableContent is UFUAModel.UFUATag && 
                          CanAssignTag(child.BrowsableContent as UFUAModel.UFUATag)
                          select child));

            var list = new List<IDocumentManager>();
            ret.ForEach((child) => list.AddRange(GetDragableChildDocumentManagers(child)));
            ret.AddRange(list);
            return ret;
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

        private TreeListNode AddDropTreeItem(Object tag, TreeListNode parent)
        {
            return AddTreeItem(tag, parent);
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
                    if (!CanAssignTag(tag))
                        forbidden++;
                }
                else
                    forbidden++;
            }

            if (forbidden == data.Records.Count())
                e.Effects = DragDropEffects.None;
        }

        private bool CanAssignTag(UFUAModel.UFUATag tag)
        {
            return !tag.IsMethod && (!tag.IsPrototypeMember || tag.IsSubPrototypeMember);
        }

        private void treeListControl_DragStart(object sender, StartRecordDragEventArgs e)
        {
            e.AllowDrag = false;
            e.Handled = true;
            return;
        }

        void treeListControl_CompletedDragDrop(object sender, CompleteRecordDragDropEventArgs e)
        {
            e.Handled = true;
        }

        private void treeListControl_PreviewQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (!e.EscapePressed)
                return;

            e.Action = DragAction.Cancel;
            e.Handled = true;
        }

        bool NeedToBeExpanded(TreeListNode item)
        {
            if (item.Tag is UFGroupEntity)
            {
                if (Properties.Settings.Default.FolderAlwaysExpandible)
                    return true;

                return ((UFGroupEntity)item.Tag).DataValues.Count > 0;
            }

            return false;
        }

        #endregion

        #region IEditableObject Members
        bool bEditingUow;
        UndoRedoNotifyDataObject undoRedoDataContext;
        public void BeginEdit()
        {
            if (bEditingUow)
                return;
            bEditingUow = true;

            undoRedoDataContext = new UndoRedoNotifyDataObject(UndoRedoAction.Changed);

            foreach (var item in treeListControl.GetSelectedNodes())
            {
                if (item is TreeListNode && (item as TreeListNode).Tag is UFRecipeEntity)
                    undoRedoDataContext.AddDataObject((item as TreeListNode).Tag as UFRecipeEntity);
                else if (item is TreeListNode && (item as TreeListNode).Tag is UFGroupEntity)
                    undoRedoDataContext.AddDataObject((item as TreeListNode).Tag as UFGroupEntity);
                else if (item is TreeListNode && (item as TreeListNode).Tag is UFDataValueEntity)
                    undoRedoDataContext.AddDataObject((item as TreeListNode).Tag as UFDataValueEntity);
            }
        }

        public void CancelEdit()
        {
            if (!bEditingUow)
                return;
            bEditingUow = false;

            if (undoRedoDataContext != null)
            {
                if (undoRedoDataContext.IsAnyPropertyChanged)
                    CheckUndoRedo(undoRedoDataContext);
                undoRedoDataContext.Dispose();
                undoRedoDataContext = null;
            }
        }

        public void EndEdit()
        {
            if (!bEditingUow)
                return;
            bEditingUow = false;

            if (undoRedoDataContext != null)
            {
                if (undoRedoDataContext.IsAnyPropertyChanged)
                    undoRedoManager.AddUndoAction(undoRedoDataContext);
                undoRedoDataContext.Dispose();
                undoRedoDataContext = null;
            }

            var selecteditems = treeListControl.GetSelectedNodes();
            foreach (var item in selecteditems)
            {
                if (item is TreeListNode && (item as TreeListNode).Tag is UFDataValueEntity)
                    layoutEditorControl.UpdateAvailableItem((item as TreeListNode).Tag as UFDataValueEntity);
            }
        }

        #endregion

        #region Layout Control

        void InitializeLayoutControl()
        {
            // Add available items
            layoutEditorControl.LayoutItems.IsCustomization = true;
            layoutEditorControl.AddAvailableItems(LayoutControlHelper.CompileLayoutItems(Document.RecipeEntity, layoutEditorControl));
            if (Document.LabelItems != null)
                layoutEditorControl.LayoutItems.AddLayoutItemLabelControls(Document.LabelItems);

            // Read items customization
            if (Document.LayoutItems != null)
            {
                using (var memoryStream = new MemoryStream(Document.LayoutItems))
                {
                    ReadLayoutDataStream(memoryStream);
                }
            }

            layoutEditorControl.LayoutItems.AddNewLayoutItemLabelControlIfNecessary();
            
            layoutEditorControl.LayoutItems.Controller.CustomizationController.SelectionChanged += layoutItems_SelectionChanged;
            layoutEditorControl.LayoutItems.Controller.CustomizationController.Control.PreviewMouseDown += layoutControl_PreviewMouseDown;
        }

        internal void SaveRecipeLayout()
        {
            using (var memoryStream = new MemoryStream())
            {
                if (WriteLayoutDataStream(memoryStream))
                {
                    Document.LayoutItems = memoryStream.ToArray();
                }
            }

            var labels = layoutEditorControl.LayoutItems.GetLayoutItemLabelControls();
            if (labels.Count > 0)
                Document.LabelItems = (from c in labels where !String.IsNullOrEmpty(c.Name) select c.Name).ToArray();
            else
                Document.LabelItems = null;

            Document.IsLayoutEmpty = layoutEditorControl.IsLayoutEmpty;
        }

        byte[] layoutState;
        void SaveLayoutState()
        {
            using (var memoryStream = new MemoryStream())
            {
                if (WriteLayoutDataStream(memoryStream))
                {
                    layoutState = memoryStream.ToArray();
                }
            }
        }

        private void layoutItems_ModelChanged(object sender, DevExpress.Xpf.LayoutControl.LayoutControlModelChangedEventArgs e)
        {
            if (disableUndoRedoAction)
                return;

            var previousState = layoutState;
            SaveLayoutState();

            if (lastLayoutState.StoreLayoutMemento(layoutState))
            {
                Document.NeedsSave = true;
                var dataObject = new UndoRedoDataObject(UndoRedoAction.LayoutState);
                dataObject.StoreLayoutMemento(previousState);
                if (dataObject.IsAnyActionAvailable)
                    undoRedoManager.AddUndoAction(dataObject);
            }
        }

        private void layoutItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (disableUndoRedoAction)
                return;

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() => layoutEditorControl.LayoutItems.AddNewLayoutItemLabelControlIfNecessary());

            var previousState = layoutState;
            SaveLayoutState();

            if (lastLayoutState.StoreLayoutMemento(layoutState))
            {
                Document.NeedsSave = true;
                var dataObject = new UndoRedoDataObject(UndoRedoAction.LayoutState);
                dataObject.StoreLayoutMemento(previousState);
                if (dataObject.IsAnyActionAvailable)
                    undoRedoManager.AddUndoAction(dataObject);
            }
        }

        private void layoutControl_NeedSave(object sender, EventArgs e)
        {
            Document.NeedsSave = true;
        }

        private void layoutItems_IsCustomizationChanged(object sender, EventArgs e)
        {
            if (layoutEditorControl.LayoutItems.IsCustomization)
            {
                // Clean the available items list
                layoutEditorControl.CleanAvailableItems();
                layoutEditorControl.LayoutItems.Controller.CustomizationController.SelectionChanged += layoutItems_SelectionChanged;
                layoutEditorControl.LayoutItems.Controller.CustomizationController.Control.PreviewMouseDown += layoutControl_PreviewMouseDown;
            }
            else
            {
                layoutEditorControl.LayoutItems.Controller.CustomizationController.SelectionChanged -= layoutItems_SelectionChanged;
                layoutEditorControl.LayoutItems.Controller.CustomizationController.Control.PreviewMouseDown -= layoutControl_PreviewMouseDown;
            }
        }

        bool bIsActive;
        internal void OnActivate()
        {
            layoutEditorControl.LayoutItems.IsCustomization = true;
            layoutItems_SelectionChanged(null, null);
            bIsActive = true;
        }

        internal void OnDeactivate()
        {
            bIsActive = false;
        }

        List<PropertyChangeNotifier> listPropertyChangeNotifier;
        private void layoutItems_SelectionChanged(object sender, DevExpress.Xpf.LayoutControl.LayoutControlSelectionChangedEventArgs e)
        {
            CheckAndAssignWorkspaceContext();
        }

        private void layoutControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            treeListControl.ClearSelection();
        }

        private void layoutControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                e.Handled = true;
                if (!layoutEditorControl.LayoutItems.Controller.IsDragAndDrop && 
                    layoutEditorControl.LayoutItems.Controller.CustomizationController != null)
                {
                    layoutEditorControl.LayoutItems.Controller.CustomizationController.SelectedElements.ForEach((element) =>
                    {
                        if (IsChildElement(element) && layoutEditorControl.LayoutItems != element)
                            layoutEditorControl.AddAvailableItem(element);
                    });
                }
            }
        }

        #endregion

        #region Address Space

        class rootHeader
        {
            public String Name { get; set; }
        }

        //private void tabSplitterLoaded(object sender, RoutedEventArgs e)
        //{
        //    var tabSplitter = sender as TabSplitter;
        //    if (tabSplitter.HideHeaderOnSingleChild)
        //    {
        //        TabPanelAdv tab = (tabSplitter.Template.FindName("PART_TabPanel", tabSplitter)) as TabPanelAdv;
        //        if (tab != null)
        //            tab.Visibility = System.Windows.Visibility.Collapsed;
        //    }
        //}

        void InitializeAddressSpace()
        {
            openFolderImg = RecipeEditorManagerComponent.GetBitmapImage("OpenFolderSmall", true);
            closedFolderImg = RecipeEditorManagerComponent.GetBitmapImage("CloseFolderSmall", true);

            treeListView.Nodes.Clear();
            itemRoot = treeListControl.AddNode(new TreeItemControl(Document.Title) { ResourceIcon = closedFolderImg }, Tag as TreeListNode, Document.RecipeEntity);
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

        void treeListControl_SelectionChanged(object sender, TreeListSelectionChangedEventArgs e)
        {
            if (e != null)
                e.Handled = true;
            CheckAndAssignWorkspaceContext();
        }

        void ReloadAddressSpace(UFGroupEntity group = null, UFGroupEntity parentGroup = null, UFDataValueEntity value = null)
        {
            treeListView.Nodes.Clear();
            itemRoot = treeListControl.AddNode(new TreeItemControl(Document.Title) { ResourceIcon = closedFolderImg }, Tag as TreeListNode, Document.RecipeEntity);
            treeListControl.AddNode(null, itemRoot, TreeListControlHelper.DummyNode);

            FillItems(itemRoot, group, parentGroup, value);
        }

        void CheckAndAssignWorkspaceContext()
        {
            if (!UnSubscribeSelectionChangedEvent) {
                UnSubscribeSelectionChangedEvent = true;
                try
                {
                    
                    if (layoutEditorTab.IsSelected && layoutEditorControl.LayoutItems.Controller.CustomizationController != null)
                    {
                        var SelectedElements = layoutEditorControl.LayoutItems.Controller.CustomizationController.SelectedElements;
                        foreach (var element in SelectedElements)
                        {
                            if (element.ReadLocalValue(DevExpress.Xpf.LayoutControl.LayoutControl.AllowHorizontalSizingProperty) == DependencyProperty.UnsetValue)
                                DevExpress.Xpf.LayoutControl.LayoutControl.SetAllowHorizontalSizing(element, false);
                            if (element.ReadLocalValue(DevExpress.Xpf.LayoutControl.LayoutControl.AllowVerticalSizingProperty) == DependencyProperty.UnsetValue)
                                DevExpress.Xpf.LayoutControl.LayoutControl.SetAllowVerticalSizing(element, false);
                        }

                        if (SelectedElements.Count == 1 && !ReferenceEquals(SelectedElements[0], layoutEditorControl.LayoutItems))
                        {
                            EditorComponent.Workspace.ContextObject = SelectedElements[0];

                            if (listPropertyChangeNotifier != null)
                            {
                                listPropertyChangeNotifier.ForEach(c => c.Dispose());
                                listPropertyChangeNotifier.Clear();
                            }
                            if (listPropertyChangeNotifier == null)
                                listPropertyChangeNotifier = new List<PropertyChangeNotifier>();

                            // add notifier for all dependency properties of selected element in the layout control
                            foreach (PropertyDescriptor pd in PropertyChangeNotifier.GetPropertyList(SelectedElements[0].GetType()))
                            {
                                DependencyPropertyDescriptor dpd =
                                    DependencyPropertyDescriptor.FromProperty(pd);

                                if (dpd != null && !dpd.IsReadOnly)
                                {
                                    var notifier = new PropertyChangeNotifier(SelectedElements[0], dpd.Name);
                                    notifier.ValueChanged += layoutControl_NeedSave;
                                    listPropertyChangeNotifier.Add(notifier);
                                }
                            }
                        }
                        else if (SelectedElements.Count > 0)
                        {
                            EditorComponent.Workspace.ContextObjects = SelectedElements;
                        }
                        else
                            EditorComponent.Workspace.ContextObject = null;
                    }
                    else if (treeListTab.IsSelected)
                    {
                        var selecteditems = treeListControl.GetTreeSelectedItems();

                        if (selecteditems.Count == 0)
                        {
                            EditorComponent.Workspace.ContextObject = Document;
                        }
                        else if (selecteditems.Count == 1)
                        {
                            EditorComponent.Workspace.ContextObject = selecteditems[0];
                        }
                        else
                        {
                            EditorComponent.Workspace.ContextObjects = selecteditems;
                        }
                    }
                }
                finally
                {
                    UnSubscribeSelectionChangedEvent = false;
                }
            }

            //Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            //{
            //    var item = TreeListControl.GetTreeViewItemFromChildren(treeListControl.SelectedItem as FrameworkElement);
            //    if (item != null)
            //        item.Focus();
            //});
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

        private void FillItems(TreeListNode itemRoot, UFGroupEntity group = null, UFGroupEntity parentGroup = null, UFDataValueEntity value = null)
        {
            treeListControl.BeginDataUpdate();
            TreeListNode selectedNode = null;
            TreeListNode groupToExpand = null;
            try
            {
                ClearNodes(itemRoot);
                using (new WaitCursor())
                {
                    var ordervalues = Document.GetDataValuesCollection();
                    if (ordervalues.Count > 0)
                    {
                        foreach (var val in ordervalues)
                        {
                            var item = AddTreeItem(val, itemRoot);
                            if (value != null && val == value)
                                selectedNode = item;
                        }
                    }

                    var ordergroups = Document.GetGroupsCollection();
                    if (ordergroups.Count > 0)
                    {
                        foreach (var g in ordergroups)
                        {
                            var item = AddTreeItem(g, itemRoot);
                            if (group != null && group == g)
                                selectedNode = item;
                            else if (parentGroup != null && parentGroup == g)
                                groupToExpand = item;
                        }
                    }
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
                if (groupToExpand != null)
                {
                    treeListControl.View.ExpandNode(itemRoot.RowHandle);
                    treeListControl.View.ExpandNode(groupToExpand.RowHandle);
                    var valueNode = (from TreeListNode n in groupToExpand.Nodes where n.Tag == value select n).FirstOrDefault();
                    if (valueNode != null)
                        treeListControl.SelectNode(valueNode);
                }
                else if (selectedNode != null)
                {
                    treeListControl.View.ExpandNode(itemRoot.RowHandle);
                    treeListControl.SelectNode(selectedNode);
                }
            }
        }

        private void FillItems(TreeListNode itemRoot, UFGroupEntity root)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                ClearNodes(itemRoot);
                using (new WaitCursor())
                {
                    var ordervalues = Document.GetDataValuesCollection(root);
                    if (ordervalues.Count > 0)
                    {
                        foreach (var value in ordervalues)
                            AddTreeItem(value, itemRoot);
                    }
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }
        }

        private static bool CanBeExpanded(TreeListNode parent)
        {
            return parent.Nodes.Count == 1 && parent.Nodes[0].Tag == TreeListControlHelper.DummyNode;
        }

        void OnTreeNodeExpanding(object sender, TreeListNodeAllowEventArgs e)
        {
            TreeListNode item = e.Node;
            try
            {
                if (item == null || TreeListControlHelper.WasExpanded(item) ||
                    e != null && !CanBeExpanded(item))
                    return;

                (item.Content as TreeItemControl).IsNodeExpanding = true;
                treeListControl.BeginDataUpdate();

                ClearNodes(item);
                if (item.Tag is UFGroupEntity)
                    FillItems(item, item.Tag as UFGroupEntity);

                treeListControl.EndDataUpdate();
                e.Handled = true;
            }
            finally
            {
                (item.Content as TreeItemControl).IsNodeExpanding = false;
                UpdateFolderIcon(e.Node, true);
            }
        }

        void OnTreeNodeCollapsing(object sender, TreeListNodeAllowEventArgs e)
        {
            if (!(e.Node.Content as TreeItemControl).IsNodeExpanding)
                UpdateFolderIcon(e.Node, false);
        }

        void UpdateFolderIcon(TreeListNode node, bool isOpen)
        {
            if (node != null && node == itemRoot)
                (node.Content as TreeItemControl).ResourceIcon = isOpen ? openFolderImg : closedFolderImg;
        }

        internal TreeListNode AddTreeItem(Object tag, TreeListNode parent, int nPos = -1)
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

            var ic = new RecipeTreeItemControl(tag);

            TreeListNode newitem;
            if (parent.Nodes.Count < 1 || nPos < 0 || parent.Nodes.Count < nPos)
                newitem = treeListControl.AddNode(ic, parent, tag);
            else
                newitem = treeListControl.AddNode(ic, parent, tag, nPos);

            if(NeedToBeExpanded(newitem))
                treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);

            if (tag is UFGroupEntity)
            {
                var group = tag as UFGroupEntity;
                group.PropertyChanged += recipeEntity_PropertyChanged;
                if (parent.Tag is UFRecipeEntity)
                {
                    group.UFRecipeAss = parent.Tag as UFRecipeEntity;
                    if (!group.UFRecipeAss.Groups.Contains(group))
                    {
                        try
                        {
                            disableUndoRedoAction = true;
                            group.UFRecipeAss.Groups.Add(group);
                            layoutEditorControl.AddAvailableItem(LayoutControlHelper.CreateLayoutItemControl(group));
                            Document.NeedsSave = true;
                        }
                        finally
                        {
                            disableUndoRedoAction = false;
                        }
                    }
                }
                SetBindingOnProp(newitem, group, "Name");
                (newitem.Content as RecipeTreeItemControl).ResourceIcon = RecipeEditorManagerComponent.GetBitmapImage("RCPMGroup");
            }
            else if (tag is UFDataValueEntity)
            {
                var value = tag as UFDataValueEntity;
                value.PropertyChanged += recipeEntity_PropertyChanged;
                if (parent.Tag is UFRecipeEntity)
                {
                    value.UFRecipeAss = parent.Tag as UFRecipeEntity;
                    if (!value.UFRecipeAss.DataValues.Contains(value))
                    {
                        try
                        {
                            disableUndoRedoAction = true;
                            value.UFRecipeAss.DataValues.Add(value);
                            layoutEditorControl.AddAvailableItem(LayoutControlHelper.CreateLayoutItemControl(value));
                            Document.NeedsSave = true;
                        }
                        finally
                        {
                            disableUndoRedoAction = false;
                        }
                    }
                }
                else if (parent.Tag is UFGroupEntity)
                {
                    value.UFGroupAss = parent.Tag as UFGroupEntity;
                    if (!value.UFGroupAss.DataValues.Contains(value))
                    {
                        try
                        {
                            disableUndoRedoAction = true;
                            value.UFGroupAss.DataValues.Add(value);
                            layoutEditorControl.AddAvailableItem(LayoutControlHelper.CreateLayoutItemControl(value), value.UFGroupAss);
                            Document.NeedsSave = true;
                        }
                        finally
                        {
                            disableUndoRedoAction = false;
                        }
                    }
                }
                SetBindingOnProp(newitem, value, "Name");
                (newitem.Content as RecipeTreeItemControl).ResourceIcon = RecipeEditorManagerComponent.GetBitmapImage("RCPMDataValue");
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
            BindingOperations.SetBinding(node.Content as RecipeTreeItemControl, targetDP, myBinding);
        }

        void recipeEntity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Document.NeedsSave = true;
            if (e.PropertyName == "AllowNull" ||
                e.PropertyName == "DataType")
                Document.NeedsRebuild = true;
        }

        private TreeListNode GetTreeItem(Object tag, TreeListNode parent = null)
        {
            if (parent == null)
                parent = itemRoot;

            if (!parent.IsExpanded)
                parent.IsExpanded = true;

            var list = (from p in parent.Nodes where p.Tag == tag select p).ToList();
            if (list.Count > 0)
                return list[0];

            return null;
        }

        private TreeListNode GetTreeItem(Guid guid, TreeListNode parent = null)
        {
            if (parent == null)
                parent = itemRoot;

            if (!parent.IsExpanded)
                parent.IsExpanded = true;

            var list = (from p in parent.Nodes
                        where (p.Tag is UFDataValueEntity && (p.Tag as UFDataValueEntity).NodeId == guid) ||
                        (p.Tag is UFGroupEntity && (p.Tag as UFGroupEntity).NodeId == guid)
                        select p).ToList();
            if (list.Count > 0)
                return list[0];

            return null;
        }

        internal bool IsAnyDataValueSelected()
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            return item != null && item.Tag is UFDataValueEntity;
        }

        internal bool IsAnyGroupSelected()
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            return item != null && item.Tag is UFGroupEntity;
        }

        internal bool IsAnyItemSelected()
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            return item != null && item != itemRoot;
        }

        internal void DeleteSelectedItems()
        {
            var items = (from item in treeListControl.GetSelectedNodes()
                         where item.Tag != null select item).ToList();

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

        internal void DeleteTreeItem(TreeListNode item, bool checkEmpty = true)
        {
            var parent = item.ParentNode ?? itemRoot;
            if (item.Tag is UFDataValueEntity)
            {
                var value = item.Tag as UFDataValueEntity;
                parent.Nodes.Remove(item);
                if (value.UFRecipeAss != null)
                {
                    value.UFRecipeAss.DataValues.Remove(value);
                    Document.NeedsSave = true;
                }
                if (value.UFGroupAss != null)
                {
                    value.UFGroupAss.DataValues.Remove(value);
                    Document.NeedsSave = true;
                }
                value.PropertyChanged -= recipeEntity_PropertyChanged;
                try
                {
                    disableUndoRedoAction = true;
                    layoutEditorControl.RemoveAvailableItem(value);
                }
                finally
                {
                    disableUndoRedoAction = false;
                }
            }
            else if (item.Tag is UFGroupEntity)
            {
                var group = item.Tag as UFGroupEntity;
                if (checkEmpty && group.DataValues.Count > 0)
                {
                    using (new ResetCursor())
                    {
                        var ret = MessageBox.Show(Properties.Resources.GroupNotEmpty, group.GroupName, MessageBoxButton.YesNo);
                        if (ret == MessageBoxResult.No)
                            return;
                    }
                }
                parent.Nodes.Remove(item);
                if (group.UFRecipeAss != null)
                {
                    group.UFRecipeAss.Groups.Remove(group);
                    Document.NeedsSave = true;
                }
                group.PropertyChanged -= recipeEntity_PropertyChanged;
                try
                {
                    disableUndoRedoAction = true;
                    layoutEditorControl.RemoveAvailableItem(group);
                }
                finally
                {
                    disableUndoRedoAction = false;
                }
            }
        }

        internal void AddGroup(UFGroupEntity group, TreeListNode parent)
        {
            treeListControl.ClearSelection();
            var newitem = AddTreeItem(group, parent, -1);
            if (newitem != null)
                treeListControl.SelectNode(newitem);
        }

        internal void AddDataValue(UFDataValueEntity value, TreeListNode parent)
        {
            treeListControl.ClearSelection();
            var newitem = AddTreeItem(value, parent, -1);
            if (newitem != null)
                treeListControl.SelectNode(newitem);
        }

        internal TreeListNode GetSelectedParent(bool bSkipSelected = false)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (!bSkipSelected && selected != null && !(selected.Tag is UFDataValueEntity))
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && !(selected.Tag is UFDataValueEntity))
                    return selected;
            }

            return itemRoot;
        }

        internal TreeListNode GetSelectedParentGroup(bool bSkipSelected = false)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (!bSkipSelected && selected != null && selected.Tag is UFGroupEntity)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFGroupEntity)
                    return selected;
            }

            return null;
        }

        private void EditRecipe()
        {
            var clone = Document.RecipeEntity.Clone() as UFRecipeEntity;
            clone.PropertyChanged -= Document.recipeEntity_PropertyChanged;
            var newRecipeControl = new NewRecipeDefinition(Document, EditorComponent)
            {
                DataContext = clone
            };

            GeneralDialogContent Dialog = new GeneralDialogContent(newRecipeControl)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "RecipeEditor"
            };
            if (Dialog.ShowDialog() == true)
            {
                Document.RecipeEntity.copyPropertiesFrom(clone);
            }
        }

        private void EditGroup(UFGroupEntity group)
        {
            if (group == null)
                return;

            var clone = group.Clone() as UFGroupEntity;
            clone.PropertyChanged -= recipeEntity_PropertyChanged;
            var newGroupControl = new NewGroupDefinition(Document, EditorComponent)
            {
                DataContext = clone
            };

            GeneralDialogContent Dialog = new GeneralDialogContent(newGroupControl)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "RecipeGroupEditor"
            };
            if (Dialog.ShowDialog() == true)
            {
                group.copyPropertiesFrom(clone);
                // update UI properties in the layout control
                layoutEditorControl.CleanAvailableItems();
            }
        }

        private void EditDataValue(UFDataValueEntity value)
        {
            if (value == null)
                return;

            var clone = value.Clone() as UFDataValueEntity;
            clone.PropertyChanged -= recipeEntity_PropertyChanged;
            var newDataValueControl = new NewDataValueDefinition(Document, EditorComponent)
            {
                DataContext = clone
            };

            GeneralDialogContent Dialog = new GeneralDialogContent(newDataValueControl)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "RecipeDataValueEditor"
            };
            if (Dialog.ShowDialog() == true)
            {
                value.copyPropertiesFrom(clone);
                // update UI properties in the layout control
                layoutEditorControl.UpdateAvailableItem(value);
                layoutEditorControl.CleanAvailableItems();
            }
        }

        private void treeListControl_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (EditorComponent.PropertyControl != null)
                EditorComponent.PropertyControl.Activate();
            else
                EditSelectedItem();
        }

        private void layoutEditorControl_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            layoutItems_SelectionChanged(this, null);
            if (EditorComponent.PropertyControl != null)
                EditorComponent.PropertyControl.Activate();
        }

        private void treeListControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (layoutEditorControl.LayoutItems.Controller.CustomizationController != null && 
                layoutEditorControl.LayoutItems.Controller.CustomizationController.SelectedElements.Count > 0)
                layoutEditorControl.LayoutItems.Controller.CustomizationController.SelectedElements.Clear();
        }

        void EditSelectedItem()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null)
            {
                if (selected.Tag is UFRecipeEntity)
                {
                    EditRecipe();
                }
                else if (selected.Tag is UFGroupEntity)
                {
                    var group = selected.Tag as UFGroupEntity;
                    EditGroup(group);
                }
                if (selected.Tag is UFDataValueEntity)
                {
                    var value = selected.Tag as UFDataValueEntity;
                    EditDataValue(value);
                }
            }
        }

        void SaveRecipe()
        {
            if (Document.NeedsRebuild && EditorComponent.UIInterface != null)
                EditorComponent.UIInterface.ShowWarning(Properties.Resources.RecipeRebuildWarning);

            SaveRecipeLayout();
            Document.SaveToFile();
        }
        
        #endregion

        #region Properties

        UFRecipeDocument _Document;
        [Browsable(false)]
        public UFRecipeDocument Document
        {
            get
            {
                return _Document;
            }
            private set
            {
                _Document = value;
            }
        }

        #endregion

        #region Commands

        bool ReadLayoutDataStream(Stream istrm)
        {
            bool bRet = false;
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    ConformanceLevel = ConformanceLevel.Document,
                    CloseInput = true
                };

                using (var reader = XmlReader.Create(istrm, settings))
                {
                    layoutEditorControl.LayoutItems.ReadFromXML(reader);
                }

                bRet = true;
            }
            catch (Exception ex)
            {

            }

            return bRet;
        }

        bool WriteLayoutDataStream(Stream ostrm)
        {
            bool bRet = false;
            try
            {
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    Encoding = Encoding.UTF8
                };

                using (var writer = XmlWriter.Create(ostrm, settings))
                {
                    layoutEditorControl.LayoutItems.WriteToXML(writer);
                }

                bRet = true;
            }
            catch (Exception ex)
            {

            }

            return bRet;
        }

        private void OnAddStringId(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (var cursor = new WaitCursor())
            {
                var list = Document.GetControllerDataStrings();

                if (list.Count > 0)
                    RecipeEditorManagerComponent.recipeEditorManagerComponent.StringEditor.AddListStringId(Document, list);
            }
        }

        private void CanAddStringId(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Document != null)
            {
                var list = Document.GetControllerDataStrings();
                e.CanExecute = bIsActive && RecipeEditorManagerComponent.recipeEditorManagerComponent.StringEditor != null && list.Count > 0;
            }
            else
                e.CanExecute = false;
        }

        private void OnCommandSave(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            SaveRecipe();
        }

        private void CanCommandSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && Document.NeedsSave;
        }

        private void OnCommandCut(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                using (new WaitCursor())
                {
                    OnCommandCopy(sender, e);
                    OnRemoveItem(sender, e);
                }
            }
        }

        private void CanCommandCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected();
        }

        private void OnCommandCopy(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                if (IsAnyItemSelected())
                {
                    var selectednodes = treeListControl.GetSelectedNodes();
                    if (selectednodes.Count() == 1)
                    {
                        var dataObject = new DataObject();

                        var listGroup = new UFGroupsList();
                        var listDataValues = new UFDataValuesList();

                        var item = selectednodes[0];
                        if ((item as TreeListNode).Tag is UFGroupEntity)
                            listGroup.Add((item as TreeListNode).Tag as UFGroupEntity);
                        if ((item as TreeListNode).Tag is UFDataValueEntity)
                            listDataValues.Add((item as TreeListNode).Tag as UFDataValueEntity);

                        if (listGroup.Count > 0)
                        {
                            var listGroupString = listGroup.ToXml();
                            dataObject.SetData(listGroup.GetType(), listGroupString);
                            dataObject.SetData(DataFormats.Xaml, listGroupString);
                            dataObject.SetData(DataFormats.Text, listGroupString);
                        }
                        else
                        {
                            var listDataValuesString = listDataValues.ToXml();
                            dataObject.SetData(listDataValues.GetType(), listDataValuesString);
                            dataObject.SetData(DataFormats.Xaml, listDataValuesString);
                            dataObject.SetData(DataFormats.Text, listDataValuesString);
                        }

                        Clipboard.SetDataObject(dataObject, true);
                    }
                    else
                    {
                        var dataObject = new DataObject();

                        var listGroup = new UFGroupsList();
                        var listDataValues = new UFDataValuesList();

                        foreach (var item in selectednodes)
                        {
                            if ((item as TreeListNode).Tag is UFGroupEntity)
                                listGroup.Add((item as TreeListNode).Tag as UFGroupEntity);
                            if ((item as TreeListNode).Tag is UFDataValueEntity)
                                listDataValues.Add((item as TreeListNode).Tag as UFDataValueEntity);
                        }

                        if (listGroup.Count > 0)
                            dataObject.SetData(listGroup.GetType(), listGroup.ToXml());
                        if (listDataValues.Count > 0)
                            dataObject.SetData(listDataValues.GetType(), listDataValues.ToXml());

                        Clipboard.SetDataObject(dataObject, true);
                    }
                }
            }
        }

        private void CanCommandCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected();
        }

        private void OnCommandPaste(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                var undoDataObject = new UndoRedoDataObject(UndoRedoAction.Added);
                var dataObject = Clipboard.GetDataObject() as DataObject;

                var listGroup = new UFGroupsList();
                var listGroupString = dataObject.GetData(listGroup.GetType()) as String;
                if (listGroupString != null)
                {
                    listGroup = listGroupString.FromXml<UFGroupsList>();
                }

                var listDataValues = new UFDataValuesList();
                var listDataValuesString = dataObject.GetData(listDataValues.GetType()) as String;
                if (listDataValuesString != null)
                {
                    listDataValues = listDataValuesString.FromXml<UFDataValuesList>();
                }

                var parent = GetSelectedParentGroup();

                var nodesToSelect = new List<TreeListNode>();
                UnSubscribeSelectionChangedEvent = true;
                try
                {
                    treeListControl.ClearSelection();
                    treeListControl.BeginDataUpdate();

                    listGroup.ForEach(group =>
                    {
                        var newGroup = Document.AddNewGroup(group.Name);
                        newGroup.CopyAll(group);
                        var newitem = AddTreeItem(newGroup, itemRoot, -1);
                        undoDataObject.AddDataObject(newGroup);
                        if (newitem != null)
                            nodesToSelect.Add(newitem);
                    });

                    listDataValues.ForEach(datavalue =>
                    {
                        var newDataValue = Document.AddNewDataValue(parent != null ? parent.Tag as UFGroupEntity : null, datavalue.Name);
                        newDataValue.CopyAll(datavalue);
                        var newitem = AddTreeItem(newDataValue, parent ?? itemRoot, -1);
                        undoDataObject.AddDataObject(newDataValue);
                        if (newitem != null)
                            nodesToSelect.Add(newitem);
                    });

                    // undo/redo handling
                    if (listGroup.Count > 0 || listDataValues.Count > 0)
                    {
                        undoRedoManager.AddUndoAction(undoDataObject);
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
        }

        private void CanCommandPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            var dataObject = Clipboard.GetDataObject() as DataObject;
            if (dataObject != null)
            {
                var listGroup = new UFGroupsList();
                var listDataValues = new UFDataValuesList();
                var listGroupString = dataObject.GetData(listGroup.GetType()) as String;
                var listDataValuesString = dataObject.GetData(listDataValues.GetType()) as String;
                e.CanExecute = listGroupString != null || listDataValuesString != null;
            }
        }

        private void OnCommandProperties(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (EditorComponent.PropertyControl != null)
                EditorComponent.PropertyControl.Activate();
            else
                EditSelectedItem();
        }

        private void CanCommandProperties(object sender, CanExecuteRoutedEventArgs e)
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            e.CanExecute = IsAnyItemSelected() || (item != null && item.Tag is UFRecipeEntity);
        }

        private void ForceSelectionChangedEvent()
        {
            treeListControl_SelectionChanged(this, null);
        }

        UndoRedoDataObject CheckUndoRedo(UndoRedoDataObject dataObject)
        {
            UndoRedoDataObject retDataObject = null;
            if (dataObject.UndoRedoAction == UndoRedoAction.Added)
                retDataObject = new UndoRedoDataObject(UndoRedoAction.Removed);
            else if (dataObject.UndoRedoAction == UndoRedoAction.Removed)
                retDataObject = new UndoRedoDataObject(UndoRedoAction.Added);
            else if (dataObject.UndoRedoAction == UndoRedoAction.Changed)
                retDataObject = new UndoRedoDataObject(UndoRedoAction.Changed);
            else if (dataObject.UndoRedoAction == UndoRedoAction.LayoutState)
                retDataObject = new UndoRedoDataObject(UndoRedoAction.LayoutState);

            if (dataObject.UndoRedoAction == UndoRedoAction.Added)
            {
                try
                {
                    treeListControl.ClearSelection();
                    UnSubscribeSelectionChangedEvent = true;
                    treeListControl.BeginDataUpdate();
                    if (dataObject.UFGroupsList != null)
                    {
                        dataObject.UFGroupsList.ForEach(group =>
                        {
                            var item = GetTreeItem(group.NodeId, itemRoot);
                            if (item != null)
                            {
                                retDataObject.AddDataObject(item.Tag as UFGroupEntity);
                                DeleteTreeItem(item, checkEmpty: false);
                            }
                        });
                    }

                    if (dataObject.UFDataValuesList != null)
                    {
                        dataObject.UFDataValuesList.ForEach(datavalue =>
                        {
                            TreeListNode parent = null;
                            if (datavalue.UFGroupAss != null)
                                parent = GetTreeItem(datavalue.UFGroupAss.NodeId, itemRoot);
                            var item = GetTreeItem(datavalue.NodeId, parent);
                            if (item != null)
                            {
                                retDataObject.AddDataObject(item.Tag as UFDataValueEntity);
                                DeleteTreeItem(item, checkEmpty: false);
                            }
                        });
                    }
                }
                finally
                {
                    treeListControl.EndDataUpdate();
                    UnSubscribeSelectionChangedEvent = false;
                    ForceSelectionChangedEvent();
                }
            }
            else if (dataObject.UndoRedoAction == UndoRedoAction.Removed)
            {
                try
                {
                    treeListControl.ClearSelection();
                    UnSubscribeSelectionChangedEvent = true;
                    treeListControl.BeginDataUpdate();
                    if (dataObject.UFGroupsList != null)
                    {
                        dataObject.UFGroupsList.ForEach(group =>
                        {
                            var newGroup = Document.AddNewGroup(group.Name);
                            newGroup.CopyAll(group);
                            newGroup.NodeId = group.NodeId; // preserve older nodeid
                            retDataObject.AddDataObject(newGroup);
                            AddGroup(newGroup, itemRoot);
                        });
                    }

                    if (dataObject.UFDataValuesList != null)
                    {
                        dataObject.UFDataValuesList.ForEach(datavalue =>
                        {
                            TreeListNode parent = null;
                            if (datavalue.UFGroupAss != null)
                                parent = GetTreeItem(datavalue.UFGroupAss.NodeId, itemRoot);
                            var newDataValue = Document.AddNewDataValue(parent != null ? parent.Tag as UFGroupEntity : null, datavalue.Name);
                            newDataValue.CopyAll(datavalue);
                            newDataValue.NodeId = datavalue.NodeId; // preserve older nodeid
                            retDataObject.AddDataObject(newDataValue);
                            AddDataValue(newDataValue, parent ?? itemRoot);
                        });
                    }
                }
                finally
                {
                    treeListControl.EndDataUpdate();
                    UnSubscribeSelectionChangedEvent = false;
                    ForceSelectionChangedEvent();
                }
            }
            else if (dataObject.UndoRedoAction == UndoRedoAction.Changed)
            {
                bool selectionchanged = false;
                try
                {
                    treeListControl.ClearSelection();
                    UnSubscribeSelectionChangedEvent = true;
                    treeListControl.BeginDataUpdate();
                    if (dataObject.RecipeEntity != null)
                    {
                        retDataObject.AddDataObject(Document.RecipeEntity);
                        Document.RecipeEntity = dataObject.RecipeEntity;
                        ReloadAddressSpace();
                    }

                    if (dataObject.UFGroupsList != null)
                    {
                        dataObject.UFGroupsList.ForEach(group =>
                        {
                            var item = GetTreeItem(group.NodeId, itemRoot);
                            if (item != null)
                            {
                                retDataObject.AddDataObject(item.Tag as UFGroupEntity);
                                (item.Tag as UFGroupEntity).CopyAll(group);
                                (item.Tag as UFGroupEntity).GroupName = group.GroupName;
                                selectionchanged = true;
                                treeListControl.SelectNode(item);
                            }
                        });
                    }

                    if (dataObject.UFDataValuesList != null)
                    {
                        dataObject.UFDataValuesList.ForEach(datavalue =>
                        {
                            TreeListNode parent = null;
                            if (datavalue.UFGroupAss != null)
                                parent = GetTreeItem(datavalue.UFGroupAss.NodeId, itemRoot);
                            var item = GetTreeItem(datavalue.NodeId, parent);
                            if (item != null)
                            {
                                retDataObject.AddDataObject(item.Tag as UFDataValueEntity);
                                (item.Tag as UFDataValueEntity).CopyAll(datavalue);
                                (item.Tag as UFDataValueEntity).DataValueName = datavalue.DataValueName;
                                selectionchanged = true;
                                treeListControl.SelectNode(item);
                            }
                        });
                    }

                    // update UI properties in the layout control
                    layoutEditorControl.CleanAvailableItems();
                }
                finally
                {
                    treeListControl.EndDataUpdate();
                    UnSubscribeSelectionChangedEvent = false;
                    if (selectionchanged)
                        ForceSelectionChangedEvent();
                }
            }
            else // layout action
            {
                // Read items customization
                if (dataObject.LayoutMemento != null)
                {
                    try
                    {
                        disableUndoRedoAction = true;
                        
                        SaveLayoutState();
                        retDataObject.StoreLayoutMemento(layoutState);
                        
                        using (var memoryStream = new MemoryStream(dataObject.LayoutMemento))
                        {
                            ReadLayoutDataStream(memoryStream);
                        }

                        SaveLayoutState();
                        lastLayoutState.StoreLayoutMemento(layoutState);
                    }
                    finally
                    {
                        disableUndoRedoAction = false;
                    }
                }
            }

            return retDataObject;
        }

        private void OnCommandUndo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                var dataObject = undoRedoManager.Undo();
                if (dataObject == null || !dataObject.IsAnyActionAvailable)
                    return;

                undoRedoManager.AddRedoAction(CheckUndoRedo(dataObject));
            }
        }

        private void CanCommandUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = undoRedoManager.CanUndo();
        }

        private void OnCommandRedo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                var dataObject = undoRedoManager.Redo();
                if (dataObject == null || !dataObject.IsAnyActionAvailable)
                    return;

                undoRedoManager.AddUndoAction(CheckUndoRedo(dataObject));
            }
        }

        private void CanCommandRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = undoRedoManager.CanRedo();
        }

        private void OnRemoveItem(object sender, ExecutedRoutedEventArgs e)
        {
            using (new WaitCursor())
            {
                // undo/redo handling
                var dataObject = new UndoRedoDataObject(UndoRedoAction.Removed);
                foreach (var item in treeListControl.GetSelectedNodes())
                {
                    if (item is TreeListNode && (item as TreeListNode).Tag is UFGroupEntity)
                        dataObject.AddDataObject((item as TreeListNode).Tag as UFGroupEntity);
                    else if (item is TreeListNode && (item as TreeListNode).Tag is UFDataValueEntity)
                        dataObject.AddDataObject((item as TreeListNode).Tag as UFDataValueEntity);
                }

                if (dataObject.IsAnyActionAvailable)
                    undoRedoManager.AddUndoAction(dataObject);

                DeleteSelectedItems();
            }
        }

        private void CanRemoveItem(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected();
        }

        private void OnServiceManager(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            try
            {
                var rootParent = Document.Parent;
                while (rootParent.Parent != null)
                    rootParent = rootParent.Parent;

                var dependencies = new List<String>();
                if (EditorComponent.UfuaEditorService != null)
                    dependencies.Add(EditorComponent.UfuaEditorService.GetServiceName(Document.Parent));

                RecipeServiceCMS.RecipeServiceCSMHelpers.OpenServiceManager(rootParent.Title, dependencies.ToArray(), rootParent.FilePath);
            }
            catch (Exception ex)
            {
                if (EditorComponent.UIInterface != null)
                {
                    EditorComponent.UIInterface.ShowError(String.Format(Properties.Resources.ServiceManagerStartFailed.Replace("'newline'", Environment.NewLine), ex.Message));
                }
            }
        }

        private void CanServiceManager(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnAddNewGroup(object sender, ExecutedRoutedEventArgs e)
        {
            var group = Document.AddNewGroup();

            if (EditorComponent.PropertyControl != null)
            {
                AddGroup(group, itemRoot);

                var dataObject = new UndoRedoDataObject(UndoRedoAction.Added);
                dataObject.AddDataObject(group);
                undoRedoManager.AddUndoAction(dataObject);
                EditorComponent.PropertyControl.Activate();
            }
            else
            {
                var newGroup = new NewGroupDefinition(Document, EditorComponent)
                {
                    DataContext = group
                };

                GeneralDialogContent Dialog = new GeneralDialogContent(newGroup)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "RecipeGroupEditor"
                };
                if (Dialog.ShowDialog() == true)
                {
                    AddGroup(group, itemRoot);

                    var dataObject = new UndoRedoDataObject(UndoRedoAction.Added);
                    dataObject.AddDataObject(group);
                    undoRedoManager.AddUndoAction(dataObject);
                }
                else if (group is IDisposable)
                {
                    (group as IDisposable).Dispose();
                }
            }

            e.Handled = true;
        }

        private void CanAddNewGroup(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnAddNewDataValue(object sender, ExecutedRoutedEventArgs e)
        {
            UFGroupEntity group = null;
            
            var parent = GetSelectedParent();
            if (parent != null)
                group = parent.Tag as UFGroupEntity;

            var value = Document.AddNewDataValue(group);
            if (EditorComponent.PropertyControl != null)
            {
                AddDataValue(value, parent);

                var dataObject = new UndoRedoDataObject(UndoRedoAction.Added);
                dataObject.AddDataObject(value);
                undoRedoManager.AddUndoAction(dataObject);
            }
            else
            {
                var newValue = new NewDataValueDefinition(Document, EditorComponent)
                {
                    DataContext = value
                };

                GeneralDialogContent Dialog = new GeneralDialogContent(newValue)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "RecipeDataValueEditor"
                };
                if (Dialog.ShowDialog() == true)
                {
                    AddDataValue(value, parent);

                    var dataObject = new UndoRedoDataObject(UndoRedoAction.Added);
                    dataObject.AddDataObject(value);
                    undoRedoManager.AddUndoAction(dataObject);
                }
                else if (value is IDisposable)
                {
                    (value as IDisposable).Dispose();
                }
            }

            e.Handled = true;
        }

        private void CanAddNewDataValue(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnMoveTagUp(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            bool changed = false;
            var dataObject = new UndoRedoDataObject(UndoRedoAction.Changed);
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (item != null && item.Tag is UFDataValueEntity)
            {
                dataObject.AddDataObject(item.Tag as UFDataValueEntity);
                changed = Document.MoveEntityUp(item.Tag as UFDataValueEntity);
            }
            else if (item != null && item.Tag is UFGroupEntity)
            {
                dataObject.AddDataObject(item.Tag as UFGroupEntity);
                changed = Document.MoveEntityUp(item.Tag as UFGroupEntity);
            }

            if (changed)
            {
                //undoRedoManager.AddUndoAction(dataObject);
                ReloadAddressSpace(item.Tag as UFGroupEntity, (item.Tag as UFDataValueEntity)?.UFGroupAss, item.Tag as UFDataValueEntity);
            }
        }
        private void CanMoveTagUp(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyDataValueSelected() || IsAnyGroupSelected();
        }
        private void OnMoveTagDown(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            bool changed = false;
            var dataObject = new UndoRedoDataObject(UndoRedoAction.Changed);
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (item != null && item.Tag is UFDataValueEntity)
            {
                dataObject.AddDataObject(item.Tag as UFDataValueEntity);
                changed = Document.MoveEntityDown(item.Tag as UFDataValueEntity);
            }
            else if (item != null && item.Tag is UFGroupEntity)
            {
                dataObject.AddDataObject(item.Tag as UFGroupEntity);
                changed = Document.MoveEntityDown(item.Tag as UFGroupEntity);
            }

            if (changed)
            {
                //undoRedoManager.AddUndoAction(dataObject);
                ReloadAddressSpace(item.Tag as UFGroupEntity, (item.Tag as UFDataValueEntity)?.UFGroupAss, item.Tag as UFDataValueEntity);
            }
        }
        //RecipeTreeItemControl GetNextRowOfType(Type type, int index)
        //{
        //    RecipeTreeItemControl row = null;
        //    while (true)
        //    {
        //        var r = treeListControl.GetRow(index + 1);
        //        if (r == null)
        //            return null;
        //        else
        //        {
        //            row = r as RecipeTreeItemControl;
        //            if (row != null && row.Type.ToString() == type.ToString())
        //                return row;
        //        }
        //    }
        //}
        private void CanMoveTagDown(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyDataValueSelected() || IsAnyGroupSelected();
        }

        private void OnImportExportRecipe(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            var importExportControl = new CsvHelper.ImportExportRecipe()
            {
                DataContext = Document
            };

            GeneralDialogContent Dialog = new GeneralDialogContent(importExportControl,
                GeneralDialogButtons.CancelButton | GeneralDialogButtons.HelpButton,
                new Dictionary<GeneralDialogButtons, String>()
                {{ GeneralDialogButtons.CancelButton, Properties.Resources.ImportExportDialogCloseCommand }})
            {
                Title = Properties.Resources.ImportExportDialogTitle,
                Owner = this.FindParent<Window>(),
                HelpLink = "ImportExportRecipe"
            };

            importExportControl.Executed += (s, ev) =>
            {
                if (ev.Type == CsvHelper.OperationType.Import && ev.Result)
                {
                    var dataObject = new UndoRedoDataObject(UndoRedoAction.Changed);
                    dataObject.AddDataObject(Document.RecipeEntity);
                    undoRedoManager.AddUndoAction(dataObject);
                    Document.RecipeEntity = ev.NewRecipeEntity;
                    ReloadAddressSpace();
                    layoutEditorControl.CleanAvailableItems();
                }
            };

            Dialog.ShowDialog();
        }
        private void CanImportExportRecipe(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && Document.RecipeEntity != null;
        }

        private void OnResetRecipeLayout(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (EditorComponent.UIInterface == null || 
                EditorComponent.UIInterface.ShowYesNoCancel(Properties.Resources.RecipeAskResetLayout, CustomDialogIcons.Question) == CustomDialogResults.Yes)
            {
                layoutEditorControl.ClearAvailableItems();
                layoutEditorControl.AddAvailableItems(LayoutControlHelper.CompileLayoutItems(Document.RecipeEntity, layoutEditorControl));
                layoutEditorControl.LayoutItems.AddNewLayoutItemLabelControlIfNecessary();
                Document.NeedsSave = true;
            }
        }

        private void CanResetRecipeLayout(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = layoutEditorTab.Visibility != Visibility.Collapsed;
        }

        private void OnCreateRecipeDatabase(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (EditorComponent.UIInterface == null || 
                EditorComponent.UIInterface.ShowYesNoCancel(Properties.Resources.RecipeAskCreateDatabase, CustomDialogIcons.Question) == CustomDialogResults.Yes)
            {
                if (cts == null)
                    cts = new CancellationTokenSource();
                var token = cts.Token;
                EditorComponent.Workspace.IsBusy = true;
                var task1 = Task.Factory.StartNew(() =>
                {
                    var executer = new UFRecipeExecuter.UFRecipeExecuter(Document, UFRecipeExecuter.OPCUA.ConnectorType.None);
                    executer.CheckAndVerifyDatabase(token, clear: true);
                    return executer;
                }, token, TaskCreationOptions.LongRunning, TaskScheduler.Current);

                task1.ContinueWith((ret) =>
                {
                    EditorComponent.Workspace.IsBusy = false;

                    try
                    {
                        if (token.IsCancellationRequested)
                            return;

                        if (ret.Exception != null)
                        {
                            if (EditorComponent.UIInterface != null)
                            {
                                EditorComponent.UIInterface.ShowError(ret.Exception.InnerException.Message);
                            }
                        }
                    }
                    finally
                    {
                        if (ret.Result != null)
                            ret.Result.Dispose();
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void CanCreateRecipeDatabase(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        
        private void OnShowRecipeEditor(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            if (Document.NeedsSave)
            {
                if (EditorComponent.UIInterface != null)
                {
                    var result = EditorComponent.UIInterface.ShowYesNoCancel(Properties.Resources.RecipeAskSaveChanges, CustomDialogIcons.Question);
                    if (result == CustomDialogResults.Cancel || result == CustomDialogResults.None)
                        return;
                    else if (result == CustomDialogResults.Yes)
                        SaveRecipe();
                }
            }

            EditorComponent.Workspace.IsBusy = true;

            if (cts == null)
                cts = new CancellationTokenSource();
            var token = cts.Token;

            var task1 = Task.Factory.StartNew(() =>
            {
                var executer = new UFRecipeExecuter.UFRecipeExecuter(Document, UFRecipeExecuter.OPCUA.ConnectorType.None);
                executer.CheckAndVerifyDatabase(token);
                return executer;
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Current);

            task1.ContinueWith((ret) =>
            {
                EditorComponent.Workspace.IsBusy = false;

                try
                {
                    if (token.IsCancellationRequested)
                        return;

                    if (ret.Exception != null)
                    {
                        if (EditorComponent.UIInterface != null)
                        {
                            EditorComponent.UIInterface.ShowError(ret.Exception.InnerException.Message);
                        }
                    }
                    else if (ret.Result != null)
                    {
                        var context = new UFRecipeExecutionContext.RecipeExecutionContext()
                        {
                            CommandType = UFRecipeExecutionContext.RecipeCommandType.Show
                        };
                        ret.Result.Execute(Document.Parent, ExecutionMode.Synchro, context);
                    }
                }
                finally
                {
                    if (ret.Result != null)
                        ret.Result.Dispose();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void CanShowRecipeEditor(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = layoutEditorTab.Visibility != Visibility.Collapsed;
        }

        #endregion

        #region IDisposable

        protected bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            if (itemRoot != null)
                ClearObjectMapNode(itemRoot);

            EditorComponent.Workspace.ContextObject = null;
            EditorComponent.Workspace.DockItemRestored -= Workspace_DockItemRestored;

            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }

            if (listPropertyChangeNotifier != null)
            {
                listPropertyChangeNotifier.ForEach(c => c.Dispose());
                listPropertyChangeNotifier.Clear();
                listPropertyChangeNotifier = null;
            }
        }

        #endregion
    }
}

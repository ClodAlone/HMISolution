using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using Utilities;
using Utilities.WPF;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MenuSettings.Documents;
using MenuSettings.MenuModel;
using WPFUtilities;
using UFMenuEditor.UndoRedo;
using UFMenuEditor.ComponentService;
using UFMenuEditor.Controls;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using System.Windows.Data;

namespace UFMenuEditor
{
    /// <summary>
    /// Interaction logic for UFMenuEditorUI.xaml
    /// </summary>
    public partial class UFMenuEditorUI : UserControl, IDisposable
    {
        #region Declarations

        TreeListNode itemRoot;
        BitmapImage openFolderImg;
        BitmapImage closedFolderImg;
        bool UnSubscribeSelectionChangedEvent;
        bool bIsActive;

        readonly MenuEditorManagerComponent EditorComponent;

        readonly UndoRedoManager undoRedoManager = new UndoRedoManager(100);

        private const string TestSession = "test";
        #endregion

		bool bLoaded;
        public bool IsLoaded
        {
            get
            {
                return bLoaded;
            }
        }

        public UFMenuEditorUI(MenuEditorManagerComponent editorComponent, UFMenuDocument doc)
        {
            InitializeComponent();

            EditorComponent = editorComponent;
            Document = doc;

            InitializeAddressSpace();

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                    bLoaded = true;
            };
            Unloaded += (o, e) =>
            {
                if (bLoaded)
                    bLoaded = false;
            };
        }

        #region Address Space

        class rootHeader
        {
            public String Name { get; set; }
        }

        void InitializeAddressSpace()
        {
            openFolderImg = MenuEditorManagerComponent.GetBitmapImage("OpenFolderSmall", true);
            closedFolderImg = MenuEditorManagerComponent.GetBitmapImage("CloseFolderSmall", true);

            treeListView.Nodes.Clear();
            itemRoot = treeListControl.AddNode(new TreeItemControl(Document.Title) { ResourceIcon = closedFolderImg }, Tag as TreeListNode, Document.MenuEntity);
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
            OnActivate(true);
            if (e != null)
                e.Handled = true;
        }
        internal void OnDeactivate()
        {
            bIsActive = false;
        }
        internal void OnActivate(bool bNavigating = false)
        {
            bIsActive = true;
            if (!UnSubscribeSelectionChangedEvent)
            {
                UnSubscribeSelectionChangedEvent = true;
                try
                {
                    var selecteditems = treeListControl.GetTreeSelectedItems();

                    if (selecteditems.Count == 0)
                    {
                        EditorComponent.Workspace.ContextObject = null;
                    }
                    else if (selecteditems.Count == 1)
                    {
                        EditorComponent.Workspace.ContextObject = treeListControl.GetSelectedNodes().FirstOrDefault() == itemRoot ? Document : selecteditems[0];
                    }
                    else
                    {
                        EditorComponent.Workspace.ContextObjects = selecteditems;
                    }
                }
                finally
                {
                    UnSubscribeSelectionChangedEvent = false;
                }
            }
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
                    var ordervalues = Document.GetMenuItemCollection();
                    if (ordervalues.Count > 0)
                    {
                        foreach (var value in ordervalues)
                            AddTreeItem(value, itemRoot, -1);
                    }
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }
        }

        private void FillItems(TreeListNode itemRoot, UFMenuItemEntity root)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                ClearNodes(itemRoot);
                using (new WaitCursor())
                {
                    var ordervalues = Document.GetMenuItemCollection(root);
                    if (ordervalues.Count > 0)
                    {
                        foreach (var value in ordervalues)
                            AddTreeItem(value, itemRoot, -1);
                    }
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }
        }

        void MenuItemEntity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Contains("UFItemAss"))
                return;
            Document.NeedsSave = true;
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

        void UpdateFolderIcon(TreeListNode node, bool isOpen)
        {
            if (node != null && node == itemRoot)
                (node.Content as TreeItemControl).ResourceIcon = isOpen ? openFolderImg : closedFolderImg;
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
                if (item.Tag is UFMenuItemEntity)
                    FillItems(item, item.Tag as UFMenuItemEntity);

                treeListControl.EndDataUpdate();
                e.Handled = true;
            }
            finally
            {
                (item.Content as TreeItemControl).IsNodeExpanding = false;
                UpdateFolderIcon(e.Node, true);
            }
        }

        TreeListNode AddTreeItem(Object tag, TreeListNode parent, int nPos = -1)
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

            var ic = new MenuTreeItemControl(tag);

            TreeListNode newitem;
            if (parent.Nodes.Count < 1 || nPos < 0 || parent.Nodes.Count < nPos)
                newitem = treeListControl.AddNode(ic, parent, tag);
            else
                newitem = treeListControl.AddNode(ic, parent, tag, nPos);

            if (tag is UFMenuItemEntity)
            {
                var value = tag as UFMenuItemEntity;
                value.PropertyChanged += MenuItemEntity_PropertyChanged;//(s, e) => { Document.NeedsSave = true; };
                if (parent.Tag is UFMenuEntity)
                {
                    if (value.UFMenuAss == null)
                    {
                        value.UFMenuAss = parent.Tag as UFMenuEntity;
                        //if(save)
                        //Document.NeedsSave = true;
                    }

                    if (!value.UFMenuAss.MenuItems.Contains(value))
                    {
                        value.UFMenuAss.MenuItems.Add(value);
                        //if (save)
                        //Document.NeedsSave = true;
                    }
                    
                }
                else if (parent.Tag is UFMenuItemEntity)
                {
                    if (value.UFItemAss == null)
                    {
                        value.UFItemAss = parent.Tag as UFMenuItemEntity;
                        //if (save)
                        //Document.NeedsSave = true;
                    }
                    
                    if (!value.UFItemAss.MenuItems.Contains(value))
                    {
                        value.UFItemAss.MenuItems.Add(value);
                        //if (save)
                        //Document.NeedsSave = true;
                    }
                }

                SetBindingOnProp(newitem, value, "Name");
                (newitem.Content as TreeItemControl).ResourceIcon = (value.IsPopup() ? MenuEditorManagerComponent.GetBitmapImage("MMMenuPopup") :
                    (value.MenuItemType == MenuType.Item ? MenuEditorManagerComponent.GetBitmapImage("MMMenuItem") : MenuEditorManagerComponent.GetBitmapImage("MMMenuSeparator")));

                if (value.MenuItems.Count > 0)
                    treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);
            }

            //newitem.Selected += (o, e) =>
            //{
            //    EditorComponent.Workspace.ContextObject = tag;
            //    e.Handled = true;
            //};

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
            BindingOperations.SetBinding(node.Content as MenuTreeItemControl, targetDP, myBinding);
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

        private TreeListNode GetTreeItem(Guid guid, TreeListNode parent/* = null*/)
        {
            if (parent == null)
                parent = itemRoot;

            if (!parent.IsExpanded)
                parent.IsExpanded = true;

            var list = (from p in parent.Nodes
                        where (p.Tag is UFMenuItemEntity && (p.Tag as UFMenuItemEntity).NodeId == guid)
                        select p).ToList();
            if (list.Count > 0)
                return list[0];

            return null;
        }
        private TreeListNode GetTreeItemSub(Guid guid, TreeListNode parent)
        {
            foreach (var item in parent.Nodes)
            {
                var titem = item as TreeListNode;
                if (titem != null)
                {
                    var list = (from p in titem.Nodes
                                where (p.Tag is UFMenuItemEntity && (p.Tag as UFMenuItemEntity).NodeId == guid)
                                select p).ToList();
                    if (list.Count > 0)
                        return list[0];
                    else
                    {
                        TreeListNode adv = GetTreeItemSub(guid, titem);
                        if (adv != null)
                            return adv;
                    }
                }
            }
            return null;
        }
        private TreeListNode GetTreeItem(Guid guid)
        {
            var list = (from p in itemRoot.Nodes
                        where (p.Tag is UFMenuItemEntity && (p.Tag as UFMenuItemEntity).NodeId == guid)
                        select p).ToList();
            if (list.Count > 0)
                return list[0];
            else
                return GetTreeItemSub(guid, itemRoot);
        }

        internal bool IsAddNewMenuItemLegal()
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            return item != null && ((item.Tag is UFMenuItemEntity && (item.Tag as UFMenuItemEntity).MenuItemType != MenuType.Separator) || (item.Tag is UFMenuEntity));
        }

        internal bool IsAnyMenuItemSelected(bool nosep = false)
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (nosep)
                return item != null && item.Tag is UFMenuItemEntity && (item.Tag as UFMenuItemEntity).MenuItemType != MenuType.Separator;
            return item != null && item.Tag is UFMenuItemEntity;
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

        internal void DeleteTreeItem(TreeListNode item)
        {
            var parent = item.ParentNode ?? itemRoot;
            if (item.Tag is UFMenuItemEntity)
            {
                var value = item.Tag as UFMenuItemEntity;
                value.PropertyChanged -= MenuItemEntity_PropertyChanged;
                parent.Nodes.Remove(item);
                if (value.UFMenuAss != null)
                {
                    value.UFMenuAss.MenuItems.Remove(value);
                    Document.NeedsSave = true;
                }
                if (value.UFItemAss != null)
                {
                    value.UFItemAss.MenuItems.Remove(value);
                    Document.NeedsSave = true;
                    if (value.UFItemAss.MenuItems.Count == 0)
                        value.UFItemAss.MenuItemType = MenuType.Item;
                }
            }
        }

        internal void AddMenuItem(UFMenuItemEntity value, TreeListNode parent)
        {
            treeListControl.ClearSelection();
            var newitem = AddTreeItem(value, parent, -1);
            if (newitem != null)
                treeListControl.SelectNode(newitem);
        }

        internal TreeListNode GetSelectedParent(bool bSkipSelected = false)
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

        private void EditShortcut()
        {
            var clone = Document.MenuEntity.Clone();
            var newRecipeControl = new NewMenuDefinition()
            {
                DataContext = clone
            };

            GeneralDialogContent Dialog = new GeneralDialogContent(newRecipeControl)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "MenuEditor"
            };
            if (Dialog.ShowDialog() == true)
            {
                Document.MenuEntity.copyPropertiesFrom(clone);
            }
        }

        private void EditMenuItem(UFMenuItemEntity value, TreeListNode parent)
        {
            if (value == null)
                return;

            var clone = value.Clone() as UFMenuItemEntity;
            var newDataValueControl = new NewMenuItemDefinition(EditorComponent, Document)
            {
                DataContext = clone
            };

            GeneralDialogContent Dialog = new GeneralDialogContent(newDataValueControl)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "MenuEditor",
                DialogKeepContent = true
            };
            if (Dialog.ShowDialog() == true)
            {
                if (clone is MenuSettings.MenuModel.UFMenuItemEntity && (clone as MenuSettings.MenuModel.UFMenuItemEntity).MenuItemType != value.MenuItemType)
                {
                    (parent.Content as TreeItemControl).ResourceIcon = (value.IsPopup() ? MenuEditorManagerComponent.GetBitmapImage("MMMenuPopup") :
                        (value.MenuItemType == MenuType.Item ? MenuEditorManagerComponent.GetBitmapImage("MMMenuItem") : MenuEditorManagerComponent.GetBitmapImage("MMMenuSeparator")));
                }

                EditorComponent.CommandExplorer.PropagateChanges(newDataValueControl.CommandCtrl.Content as UserControl);
                ImageViewModel imageview = (newDataValueControl.textEditImage.DataContext as ImageViewModel);
                if (imageview != null)
                    clone.MenuItemImage = (Uri)imageview.Value;
                value.copyPropertiesFrom(clone);
                (parent.Content as TreeItemControl).Header = value;
                treeListControl.RefreshRow(parent.RowHandle);
                newDataValueControl.Dispose();
                Document.NeedsSave = true;
            }

            newDataValueControl.Dispose();
        } 

        private void treeListControl_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (EditorComponent.PropertyControl != null)
                EditorComponent.PropertyControl.Activate();
            else
                EditSelectedItem();
        }

        void EditSelectedItem()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null)
            {
                if (selected.Tag is UFMenuEntity)
                {
                    EditShortcut();
                }
                if (selected.Tag is UFMenuItemEntity)
                {
                    var value = selected.Tag as UFMenuItemEntity;
                    EditMenuItem(value, selected);
                }
            }
        }

        public void ActivateCommandExplorer()
        {
            if (EditorComponent.CommandExplorer == null)
                return;

            EditorComponent.CommandExplorer.Activate();
        }

        #endregion

        #region Properties

        UFMenuDocument _Document;
        [Browsable(false)]
        public UFMenuDocument Document
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
        private void OnAddStringId(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (var cursor = new WaitCursor())
            {
                var list = Document.GetControllerDataStrings();

                if (list.Count > 0)
                    MenuEditorManagerComponent.menueditorManagerComponent.StringEditor.AddListStringId(Document, list);
            }
        }

        private void CanAddStringId(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Document != null)
            {
                var list = Document.GetControllerDataStrings();
                e.CanExecute = bIsActive && MenuEditorManagerComponent.menueditorManagerComponent.StringEditor != null && list.Count > 0;
            }
            else
                e.CanExecute = false;
        }

        private void OnCommandSave(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Document.SaveToFile();
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
                OnCommandCopy(sender, e);
                OnRemoveItem(sender, e);
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
                    var selectedNodes = treeListControl.GetSelectedNodes();
                    if (selectedNodes.Count() == 1)
                    {
                        var dataObject = new DataObject();

                        var listKeyCommands = new UFMenuItemList();

                        var item = treeListControl.GetSelectedNodes().FirstOrDefault();
                        if ((item as TreeListNode).Tag is UFMenuItemEntity)
                            listKeyCommands.Add((item as TreeListNode).Tag as UFMenuItemEntity);

                        if (listKeyCommands.Count > 0)
                        {
                            var listDataValuesString = listKeyCommands.ToXml();
                            dataObject.SetData(listKeyCommands.GetType(), listDataValuesString);
                            dataObject.SetData(DataFormats.Xaml, listDataValuesString);
                            dataObject.SetData(DataFormats.Text, listDataValuesString);
                        }

                        Clipboard.SetDataObject(dataObject, true);
                    }
                    else
                    {
                        var dataObject = new DataObject();

                        var listMenuItems = new UFMenuItemList();

                        foreach (var item in selectedNodes)
                        {
                            if ((item as TreeListNode).Tag is UFMenuItemEntity)
                                listMenuItems.Add((item as TreeListNode).Tag as UFMenuItemEntity);
                        }

                        if (listMenuItems.Count > 0)
                            dataObject.SetData(listMenuItems.GetType(), listMenuItems.ToXml());

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

                var listMenuItems = new UFMenuItemList();
                var listMenuItemsString = dataObject.GetData(listMenuItems.GetType()) as String;
                if (listMenuItemsString != null)
                {
                    listMenuItems = listMenuItemsString.FromXml<UFMenuItemList>();
                }

                var parent = GetSelectedParent();

                UnSubscribeSelectionChangedEvent = true;
                treeListControl.BeginDataUpdate();
                try
                {
                    listMenuItems.ForEach(datavalue =>
                    {
                        var newKeyCommand = Document.AddNewMenuItem(parent != null ? parent.Tag as UFMenuItemEntity : null, datavalue.Name);
                        newKeyCommand.CopyAll(datavalue, false);
                        AddMenuItem(newKeyCommand, (parent != null ? parent : itemRoot));
                        undoDataObject.AddDataObject(newKeyCommand);

                        if (parent != null && parent.Tag is UFMenuItemEntity)
                        {
                            var val = parent.Tag as UFMenuItemEntity;
                            if (val.MenuItems.Count == 1)
                            {
                                (parent.Content as TreeItemControl).ResourceIcon = (val.IsPopup() ? MenuEditorManagerComponent.GetBitmapImage("MMMenuPopup") :
                                    (val.MenuItemType == MenuType.Item ? MenuEditorManagerComponent.GetBitmapImage("MMMenuItem") : MenuEditorManagerComponent.GetBitmapImage("MMMenuSeparator")));
                                treeListControl.RefreshRow(parent.RowHandle);
                            }
                        }
                    });

                    // undo/redo handling
                    if (listMenuItems.Count > 0)
                    {
                        undoRedoManager.AddUndoAction(undoDataObject);
                    }
                }
                finally
                {
                    treeListControl.EndDataUpdate();
                    UnSubscribeSelectionChangedEvent = false;
                }
            }
        }

        private void CanCommandPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            var dataObject = Clipboard.GetDataObject() as DataObject;
            if (dataObject != null)
            {
                var listMenuItems = new UFMenuItemList();
                var listMenuItemsString = dataObject.GetData(listMenuItems.GetType()) as String;
                e.CanExecute = listMenuItemsString != null;
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
            e.CanExecute = IsAnyItemSelected();
        }

        private void ForceSelectionChangedEvent()
        {
            treeListControl_SelectionChanged(this, null);
        }

        void CheckUndoRedo(UndoRedoDataObject dataObject)
        {
            if (dataObject.UndoRedoAction == UndoRedoAction.Added)
            {
                try
                {
                    treeListControl.ClearSelection();
                    UnSubscribeSelectionChangedEvent = true;
                    treeListControl.BeginDataUpdate();
                    if (dataObject.UFMenuItemList != null)
                    {
                        dataObject.UFMenuItemList.ForEach(keycommand =>
                        {
                            var item = GetTreeItem(keycommand.NodeId/*, parent*/);
                            if (item != null)
                                DeleteTreeItem(item);
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
                    if (dataObject.UFMenuItemList != null)
                    {
                        dataObject.UFMenuItemList.ForEach(datavalue =>
                        {
                            TreeListNode parent = null;
                            if (datavalue.UFMenuAss != null)
                                parent = GetTreeItem(datavalue.UFMenuAss.NodeId/*, itemRoot*/);
                            else if (datavalue.UFItemAss != null)
                                parent = GetTreeItem(datavalue.UFItemAss.NodeId/*, itemRoot*/);

                            var newMenuItem = Document.AddNewMenuItem(parent != null ? parent.Tag as UFMenuItemEntity : null, datavalue.Name);
                            newMenuItem.CopyAll(datavalue);
                            newMenuItem.NodeId = datavalue.NodeId; // preserve older nodeid
                            AddMenuItem(newMenuItem, parent ?? itemRoot);
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
                    if (dataObject.UFMenuItemList != null)
                    {
                        dataObject.UFMenuItemList.ForEach(datavalue =>
                        {
                            TreeListNode parent = null;
                            var item = GetTreeItem(datavalue.NodeId, parent);
                            if (item != null)
                            {
                                treeListControl.SelectNode(item);
                                selectionchanged = true;
                                (item.Tag as UFMenuItemEntity).CopyAll(datavalue);
                            }
                        });
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
        }

        private void OnCommandUndo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                var dataObject = undoRedoManager.Undo();
                if (dataObject == null || !dataObject.IsAnyActionAvailable)
                    return;

                CheckUndoRedo(dataObject);

                if (dataObject.UndoRedoAction == UndoRedoAction.Added)
                    undoRedoManager.AddRedoAction(new UndoRedoDataObject(dataObject, UndoRedoAction.Removed));
                else if (dataObject.UndoRedoAction == UndoRedoAction.Removed)
                    undoRedoManager.AddRedoAction(new UndoRedoDataObject(dataObject, UndoRedoAction.Added));
                else if (dataObject.UndoRedoAction == UndoRedoAction.Changed)
                    undoRedoManager.AddRedoAction(new UndoRedoDataObject(dataObject, UndoRedoAction.Changed));
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

                CheckUndoRedo(dataObject);

                if (dataObject.UndoRedoAction == UndoRedoAction.Added)
                    undoRedoManager.AddUndoAction(new UndoRedoDataObject(dataObject, UndoRedoAction.Removed));
                else if (dataObject.UndoRedoAction == UndoRedoAction.Removed)
                    undoRedoManager.AddUndoAction(new UndoRedoDataObject(dataObject, UndoRedoAction.Added));
                else if (dataObject.UndoRedoAction == UndoRedoAction.Changed)
                    undoRedoManager.AddUndoAction(new UndoRedoDataObject(dataObject, UndoRedoAction.Changed));
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
                    if (item is TreeListNode && (item as TreeListNode).Tag is UFMenuItemEntity)
                        dataObject.AddDataObject((item as TreeListNode).Tag as UFMenuItemEntity);
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

        private void OnAddNewMenuItem(object sender, ExecutedRoutedEventArgs e)
        {

            UFMenuItemEntity selItem = null;

            var parent = GetSelectedParent();
            if (parent != null)
                selItem = parent.Tag as UFMenuItemEntity;

            var value = Document.AddNewMenuItem(selItem);
            if (EditorComponent.PropertyControl != null)
            {
                AddMenuItem(value, parent);
                Document.NeedsSave = true;
                var dataObject = new UndoRedoDataObject(UndoRedoAction.Added);
                dataObject.AddDataObject(value);
                undoRedoManager.AddUndoAction(dataObject);
                if (parent.Tag is UFMenuItemEntity)
                {
                    var val = parent.Tag as UFMenuItemEntity;
                    if (val.MenuItems.Count == 1)
                    {
                        (parent.Content as TreeItemControl).ResourceIcon = (value.IsPopup() ? MenuEditorManagerComponent.GetBitmapImage("MMMenuPopup") :
                            (value.MenuItemType == MenuType.Item ? MenuEditorManagerComponent.GetBitmapImage("MMMenuItem") : MenuEditorManagerComponent.GetBitmapImage("MMMenuSeparator")));
                        treeListControl.RefreshRow(parent.RowHandle);
                    }
                }
                EditorComponent.PropertyControl.Activate();
            }
            else
            {
                var newValue = new NewMenuItemDefinition(EditorComponent, Document)
                {
                    DataContext = value
                };

                GeneralDialogContent Dialog = new GeneralDialogContent(newValue)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "MenuEditor",
                    DialogKeepContent = true
                };
                if (Dialog.ShowDialog() == true)
                {
                    EditorComponent.CommandExplorer.PropagateChanges(newValue.CommandCtrl.Content as UserControl);


                    ImageViewModel imageview = (newValue.textEditImage.DataContext as ImageViewModel);
                    if (imageview != null)
                        value.MenuItemImage = (Uri)imageview.Value;

                    AddMenuItem(value, parent);

                    Document.NeedsSave = true;

                    var dataObject = new UndoRedoDataObject(UndoRedoAction.Added);
                    dataObject.AddDataObject(value);
                    undoRedoManager.AddUndoAction(dataObject);
                    if (parent.Tag is UFMenuItemEntity)
                    {
                        var val = parent.Tag as UFMenuItemEntity;
                        if (val.MenuItems.Count == 1)
                        {
                            (parent.Content as TreeItemControl).ResourceIcon = (value.IsPopup() ? MenuEditorManagerComponent.GetBitmapImage("MMMenuPopup") :
                                (value.MenuItemType == MenuType.Item ? MenuEditorManagerComponent.GetBitmapImage("MMMenuItem") : MenuEditorManagerComponent.GetBitmapImage("MMMenuSeparator")));
                            treeListControl.RefreshRow(parent.RowHandle);
                        }
                    }
                }
                else if (value is IDisposable)
                {
                    (value as IDisposable).Dispose();
                }

                newValue.Dispose();
            }
            e.Handled = true;
        }

        private void CanAddNewMenuItem(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAddNewMenuItemLegal();
        }

        private void OnMoveItemUp(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            bool changed = false;
            var dataObject = new UndoRedoDataObject(UndoRedoAction.Changed);
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (item != null && item.Tag is UFMenuItemEntity)
            {
                dataObject.AddDataObject(item.Tag as UFMenuItemEntity);
                changed = Document.MoveEntityUp(item.Tag as UFMenuItemEntity);
            }
            if (changed)
            {
                undoRedoManager.AddUndoAction(dataObject);
                var parent = item.ParentNode ?? itemRoot;
                int index = parent.Nodes.IndexOf(item);
                if (index != -1 && index > 0)
                {
                    int oid;
                    var rowHandle = parent.Nodes[index - 1].RowHandle;
                    var previtem = treeListControl.GetRow(rowHandle) as MenuTreeItemControl;
                    if (previtem != null && int.TryParse(previtem.OID, out oid))
                    {
                        previtem.OID = (oid + 1).ToString();
                        treeListControl.RefreshRow(rowHandle);
                    }
                    parent.Nodes.Remove(item);
                    var newitem = AddTreeItem(item.Tag, parent, index - 1);
                    if (newitem != null)
                        treeListControl.SelectNode(newitem);
                }
                Document.NeedsSave = true;
            }
        }

        private void CanMoveItemUp(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyMenuItemSelected();
        }

        private void OnMoveItemDown(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            bool changed = false;
            var dataObject = new UndoRedoDataObject(UndoRedoAction.Changed);
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (item != null && item.Tag is UFMenuItemEntity)
            {
                dataObject.AddDataObject(item.Tag as UFMenuItemEntity);
                changed = Document.MoveEntityDown(item.Tag as UFMenuItemEntity);
            }
            
            if (changed)
            {
                undoRedoManager.AddUndoAction(dataObject);
                var parent = item.ParentNode ?? itemRoot;
                int index = parent.Nodes.IndexOf(item);
                if (index != -1 && index < parent.Nodes.Count - 1)
                {
                    int oid;
                    var rowHandle = parent.Nodes[index + 1].RowHandle;
                    var nextitem = treeListControl.GetRow(rowHandle) as MenuTreeItemControl;
                    if (nextitem != null && int.TryParse(nextitem.OID, out oid))
                    {
                        nextitem.OID = (oid - 1).ToString();
                        treeListControl.RefreshRow(rowHandle);
                    }
                    parent.Nodes.Remove(item);
                    var newitem = AddTreeItem(item.Tag, parent, index + 1);
                    if (newitem != null)
                        treeListControl.SelectNode(newitem);
                }
                Document.NeedsSave = true;
            }
        }

        private void CanMoveItemDown(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyMenuItemSelected();
        }

        private void OnTestMenu(object sender, ExecutedRoutedEventArgs e)
        {
            UFMenuEntity selItem = null;

            var parent = GetSelectedParent();
            if (parent != null)
                selItem = parent.Tag as UFMenuEntity;
            
            var test = EditorComponent.GetMenu(Document, new Uri(Document.FilePath, UriKind.RelativeOrAbsolute), TestSession, true, true, true);
            if ((test as ContextMenu) != null)
                (test as ContextMenu).IsOpen = true;
            //parent.ContextMenu = (test as ContextMenu);
            //parent.ContextMenu.IsOpen = true;
        }

        private void CanTestMenu(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAddNewMenuItemLegal();
        }
        #endregion

        #region IDisposable

        protected bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            RemovePropertyChange(itemRoot);
            EditorComponent.Workspace.ContextObject = null;
        }

        private void RemovePropertyChange(TreeListNode itemRoot)
        {
            var listMenuItem = (from p in itemRoot.Nodes where p.Tag is UFMenuItemEntity select p).ToList();
            listMenuItem.ForEach(value =>
            {
                (value.Tag as UFMenuItemEntity).PropertyChanged -= MenuItemEntity_PropertyChanged;
            });
            var listMenuEntity = (from p in itemRoot.Nodes where p.Tag is UFMenuEntity select p).ToList();
            listMenuEntity.ForEach(value =>
            {
                RemovePropertyChange(value);
            });
        }

        #endregion
    }
}

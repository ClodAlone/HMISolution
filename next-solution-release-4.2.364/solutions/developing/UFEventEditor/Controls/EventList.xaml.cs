using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UFEventEditor.Document;
using Utilities.WPF;
using Utilities;
using UFEventModel;
using DevExpress.Xpo;
using UFEventEditor.ComponentService;
using WPFUtilities;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using System.Windows.Data;
using DevExpress.Data.TreeList;
using UIMsgBoxAlertService.ComponentService;

namespace UFEventEditor.Controls
{
    /// <summary>
    /// Interaction logic for EventList.xaml
    /// </summary>
    public partial class EventList : UserControl, IDisposable
    {
        #region Declarations
        BitmapImage openFolderImg;
        BitmapImage closedFolderImg;
        readonly EventEditorDocument Document;
        readonly EventEditorManagerComponent EditorComponent;
        public TreeListNode itemRoot = null;
        Dictionary<TreeItemControl, TreeListNode> contentToNodeMap = new Dictionary<TreeItemControl, TreeListNode>();
        readonly Dictionary<Object, TreeListNode> mapObjectToParent = new Dictionary<Object, TreeListNode>();
        readonly Dictionary<Object, TreeListNode> mapObjectToOldParent = new Dictionary<Object, TreeListNode>();
        readonly IEditableObject editableOwner;
        bool UnSubscribeSelectionChangedEvent;
        bool bDisposed;

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
            e.CanExecute = true; //Document.ClipboardContainsDataLoggerSettings() || Document.ClipboardContainsDataLoggerColumn();
        }
        #endregion

        #region Ctor
        public EventList(EventEditorManagerComponent ed, EventEditorDocument doc, IEditableObject owner)
        {
            InitializeComponent();
            Document = doc;
            EditorComponent = ed;
            editableOwner = owner;

            Document.CreateUndoRedoHelper(this);

            InitializePluginList();
        }
        #endregion

        #region Plugins 
        class rootHeader
        {
            public String Name { get; set; }
        }

        void InitializePluginList()
        {
            openFolderImg = EventEditorManagerComponent.GetBitmapImage("OpenFolderSmall", true);
            closedFolderImg = EventEditorManagerComponent.GetBitmapImage("CloseFolderSmall", true);

            treeListView.Nodes.Clear();
            itemRoot = treeListControl.AddNode(new TreeItemControl(Properties.Resources.RootName) { ResourceIcon = closedFolderImg }, Tag as TreeListNode, Document);
            treeListControl.AddNode(null, itemRoot, TreeListControlHelper.DummyNode);

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                if (bDisposed)
                    return;

                FillItems();
                if (itemRoot.Nodes.Count == 0)
                    treeListControl.AddNode(null, itemRoot, TreeListControlHelper.DummyNode);
                itemRoot.IsExpanded = true;
            });
        }

        void treeListControl_SelectionChanged(object sender, TreeListSelectionChangedEventArgs e)
        {
            if (!UnSubscribeSelectionChangedEvent)
            {
                if (editableOwner != null)
                    editableOwner.EndEdit();

                UnSubscribeSelectionChangedEvent = true;

                try
                {
                    UpdateContextObjects();
                    if (e != null)
                        e.Handled = true;
                }
                finally
                {
                    UnSubscribeSelectionChangedEvent = false;
                }
            }
        }

        private void FillItems()
        {
            treeListControl.BeginDataUpdate();

            try
            {
                ClearNodes(itemRoot);

                using (new WaitCursor())
                {
                    var listFolders = Document.GetFolderCollection();
                    if (listFolders != null)
                    {
                        foreach (var tag in listFolders)
                            AddTreeItem(tag, itemRoot);
                    }
                    var listNotifications = Document.GetEventsCollection();
                    if (listNotifications != null)
                    {
                        foreach (var notifica in listNotifications)
                            AddTreeItem(notifica, itemRoot);
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

        void UpdateFolderIcon(TreeListNode node, bool isOpen)
        {
            if (node != null && (node == itemRoot || node.Tag is UFEventFolder))
                (node.Content as TreeItemControl).ResourceIcon = isOpen ? openFolderImg : closedFolderImg;
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
                if (item.Tag is UFEventFolder)
                    FillItems(item, item.Tag as UFEventFolder);

                treeListControl.EndDataUpdate();
                e.Handled = true;
            }
            finally
            {
                (item.Content as TreeItemControl).IsNodeExpanding = false;
            }
        }

        internal void OnActivate()
        {
            UpdateContextObjects();
        }

        void UpdateContextObjects()
        {
            var selecteditems = treeListControl.GetTreeSelectedItems();
            if (selecteditems.Count == 0)
                Document.EditorManagerComponent.Workspace.ContextObject = null;
            else if (selecteditems.Count == 1)
                Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(selecteditems[0]);
            else
                Document.EditorManagerComponent.Workspace.ContextObjects = Document.GetNestedObjects(selecteditems);
        }

        public class NameModel
        {
            public String Name { get; set; }
        }

        internal TreeListNode AddTreeItem(Object tag, TreeListNode parent)
        {
            if (bDisposed)
                return null;

            bool bKnownType = false;

            if (parent == null)
                parent = itemRoot;

            if (parent == null)
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
            contentToNodeMap.Add(ic, newitem);

            if (mapObjectToOldParent.ContainsKey(tag))
            {
                var oldparent = mapObjectToOldParent[tag];
                mapObjectToOldParent.Remove(tag);

                var keys = mapObjectToParent.Keys.ToList();
                foreach (var key in keys)
                {
                    if (mapObjectToParent[key] == oldparent)
                        mapObjectToParent[key] = newitem;
                }
            }

            if (NeedToBeExpanded(newitem))
                treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);

            bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (tag is UFEventObject)
            {
                bKnownType = true;

                SetBindingOnProp(newitem, tag, "Name");
                (newitem.Content as TreeItemControl).ResourceIcon = EventEditorManagerComponent.GetBitmapImage("EVMEvent");

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

                if (IsUndoRedoSupported(tag))
                    mapObjectToParent[tag] = parent;
            }
            else if (tag is String)
            {
                bKnownType = true;
                (newitem.Content as TreeItemControl).Header = tag as String;
                (newitem.Content as TreeItemControl).ResourceIcon = closedFolderImg;
            }
            else if (tag is UFEventFolder)
            {
                bKnownType = true;
                SetBindingOnProp(newitem, tag, "Name");

                (newitem.Content as TreeItemControl).ResourceIcon = closedFolderImg;

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

                if (IsUndoRedoSupported(tag))
                    mapObjectToParent[tag] = parent;
            }

            treeListControl.RefreshRow(newitem.RowHandle); //Otherwise node's header (ItemHeader) can disappear in certain conditions
            return bKnownType ? newitem : null;
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

        internal bool IsAnyItemSelected()
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            return item != null && item != itemRoot;
        }

        internal void CopySelectedToClipboard()
        {
            var listfolders = new List<UFEventFolder>();
            var listtags = new List<UFEventObject>();
            var selecteditems = treeListControl.GetSelectedNodes();
            foreach (var item in selecteditems)
            {
                if (item.Tag is UFEventFolder)
                    listfolders.Add(item.Tag as UFEventFolder);
                else if (item.Tag is UFEventObject)
                    listtags.Add(item.Tag as UFEventObject);
            }

            Document.CleanClipbaord();
            Document.CopyListFoldersToClipbaord(listfolders);
            Document.CopyListEventsToClipbaord(listtags);
        }

        internal void PasteFromClipboard()
        {
            var pastedTags = new List<TreeListNode>();
            try
            {
                var parent = GetSelectedParentFolder(false);
                var expanded = parent.IsExpanded || parent.Nodes.Count > 0 && parent.Nodes[0].Tag != TreeListControlHelper.DummyNode;
                treeListControl.ClearSelection();
                UnSubscribeSelectionChangedEvent = true;
                treeListControl.BeginDataUpdate();

                UFEventFolder folder = null;
                if (parent.Tag is UFEventFolder)
                    folder = parent.Tag as UFEventFolder;

                var listfolders = Document.PasteClipboardFolders(folder);
                treeListControl.AddDummyNodeIfNeeded(parent, NeedToBeExpanded);

                AddGetNodesList(listfolders, expanded, true, true, parent);

                var listtags = Document.PasteClipboardEvents(folder);
                treeListControl.AddDummyNodeIfNeeded(parent, NeedToBeExpanded);
                listtags.ForEach(tag =>
                {
                    var item = AddTreeItem(tag, parent);
                    if (item != null)
                        pastedTags.Add(item);
                    if (folder != null)
                        folder.UFEventObjects.Add(tag);
                });

                if (listfolders.Count > 0 || listtags.Count > 0)
                {
                    // add list to undo manager
                    var listundo = new List<IXPSimpleObject>();
                    listfolders.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
                    listtags.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
                    Document.AddUndoAction(this, listundo, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
                if (pastedTags.Count > 0)
                    treeListControl.SelectNodes(pastedTags, true, true);
                UnSubscribeSelectionChangedEvent = false;
                if (pastedTags.Count > 0)
                    ForceSelectionChangedEvent();
            }
        }

        bool AddGetNodesList(IEnumerable<object> nodesList, bool bAddOrGet, bool bClear = true, bool bExpand = true, TreeListNode parentNode = null)
        {
            if (parentNode == null)
                parentNode = itemRoot;
            var items = new List<TreeListNode>();
            foreach (var n in nodesList)
            {
                TreeListNode item;
                if (bAddOrGet)
                    item = AddTreeItem(n, parentNode);
                else
                    item = treeListControl.GetTreeItem(n, parentNode);
                if (item != null)
                    items.Add(item);
            }
            if (items.Count > 0)
            {
                treeListControl.SelectNodes(items, bClear, bExpand);
                return true;
            }
            return false;
        }

        static int maxItems = Properties.Settings.Default.MaxItemsInTree;
        private void FillItems(TreeListNode itemRoot, UFEventFolder root = null)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

                using (new WaitCursor())
                {
                    ClearNodes(itemRoot);
                    int i = 0;
                    var listFolders = Document.GetFolderCollection(root);
                    if (listFolders != null)
                    {
                        foreach (var folder in listFolders)
                        {
                            AddTreeItem(folder, itemRoot);
                            if (!bShiftDown && ++i > maxItems)
                            {
                                break;
                            }
                        }
                    }

                    var listTags = Document.GetEventsCollection(root);
                    if (listTags != null)
                    {
                        foreach (var tag in listTags)
                        {
                            AddTreeItem(tag, itemRoot);
                            if (!bShiftDown && ++i > maxItems)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }
        }

        internal void DeleteSelectedItems()
        {
            var items = (from item in treeListControl.GetSelectedNodes()
                         where item.Tag != null select item).ToList();

            var listundo = (from item in items
                            where IsUndoRedoSupported(item.Tag) && !items.Contains(item.ParentNode)
                            select item.Tag as IXPSimpleObject).ToList();
            var uiMsgBox = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
            if (uiMsgBox != null)
            {
                var bShowMessage = (from item in items/*.AsParallel()*/
                                    where item.Tag is UFEventFolder &&
                                    listundo.Contains(item.Tag as UFEventFolder) &&
                                    ((item.Tag as UFEventFolder).UFEventObjects.Count > 0 ||
                                    (item.Tag as UFEventFolder).UFEventFolders.Count > 0)
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

            if (listundo.Count > 0)
                Document.AddUndoAction(this, listundo, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Removed);

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
            if (item.Tag is UFEventObject)
            {
                var parent = item.ParentNode ?? itemRoot;
                var tag = item.Tag as UFEventObject;
                parent.Nodes.Remove(item);
                tag.Delete();

                if (IsUndoRedoSupported(tag))
                    mapObjectToOldParent[tag] = item;
            }
            else if (item.Tag is UFEventFolder)
            {
                var parent = item.ParentNode ?? itemRoot;
                var folder = item.Tag as UFEventFolder;
                parent.Nodes.Remove(item);
                folder.Delete();

                if (IsUndoRedoSupported(folder))
                    mapObjectToOldParent[folder] = item;
            }
        }

        public TreeListNode GetSelectedParentFolder(bool bSkipSelected = false)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (!bSkipSelected && selected != null && selected.Tag is UFEventFolder)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFEventFolder)
                    return selected;
            }

            return itemRoot;
        }
        internal UFEventFolder GetSelectedFolder()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFEventFolder)
                return selected.Tag as UFEventFolder;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFEventFolder)
                    return selected.Tag as UFEventFolder;
            }

            return null;
        }
        #endregion
        private void EditFolder(UFEventFolder folder)
        {
            if (folder == null)
                return;

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
                    Document.AddUndoAction(this, folder, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed);
                    uow.CommitChanges();
                    UpdateContextObjects();
                }
            }
        }
        private void EditNotification(UFEventObject adnot)
        {
            if (adnot == null)
                return;

            using (var uow = Document.BeginNestedUnitOfWork())
            {
                var newNotControl = new NewEvent(EditorComponent, Document)
                {
                    DataContext = uow.GetNestedObject(adnot)
                };

                GeneralDialogContent Dialog = new GeneralDialogContent(newNotControl)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "EditEvent"
                };
                
                if (Dialog.ShowDialog() == true)
                {
                    EditorComponent.CommandExplorer.PropagateChanges(
                        newNotControl.CommandCtrl.Content as UserControl);
                    Document.AddUndoAction(this, adnot, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed);
                    uow.CommitChanges();
                    UpdateContextObjects();

                }
            }
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

        private void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            e.Handled = true;
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null)
            {
                if (selected.Tag as String == Properties.Resources.MaxItemCountVisibleReachedDoubleClick)
                {
                    using (var cursor = new WaitCursor())
                    {
                        var parent = selected.ParentNode;
                        if (parent == itemRoot)
                        {
                            ClearNodes(parent);
                            var oldMaxItem = maxItems;
                            maxItems = Int32.MaxValue;
                            FillItems(itemRoot);
                            maxItems = oldMaxItem;
                        }
                        else
                        {
                            parent.IsExpanded = false;
                            ClearNodes(parent);
                            treeListControl.AddNode(null, parent, TreeListControlHelper.DummyNode);
                            var oldMaxItem = maxItems;
                            maxItems = Int32.MaxValue;
                            parent.IsExpanded = true;
                            maxItems = oldMaxItem;
                        }
                    }
                }
                else
                {
                    if (Document.EditorManagerComponent.PropertyControl != null)
                        Document.EditorManagerComponent.PropertyControl.Activate();
                    else
                        openSelectedObject();
                }
            }

        }

        private void openSelectedObject(object plug = null)
        {
            EditSelectedItem(plug); // should be changed like ADispatcher editor.
        }

        internal void EditSelectedItem(object plug = null)
        {
            UFEventObject adtag = null;
            UFEventFolder folder = null;
            if (plug != null)
            {
                adtag = plug as UFEventObject;
                folder = plug as UFEventFolder;
            }
            else
            { 
                var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
                if (selected != null)
                {
                    adtag = selected.Tag as UFEventObject;
                    folder = selected.Tag as UFEventFolder;
                }
            }
            if (adtag != null)
            {
                EditNotification(adtag);
            }
            else if (folder != null)
                EditFolder(folder);
        }

        #region Undo/Redo

        internal bool IsAnyUndoActionAvailable()
        {
            return Document.UndoContainsSomething(this);
        }

        internal bool IsAnyRedoActionAvailable()
        {
            return Document.RedoContainsSomething(this);
        }

        internal void CleanUndoActions()
        {
            Document.CleanUndoActions(this);
        }

        internal void CleanRedoActions()
        {
            Document.CleanRedoActions(this);
        }

        internal bool IsUndoRedoSupported(object obj)
        {
            return obj is IXPSimpleObject;
        }

        private void ForceSelectionChangedEvent()
        {
            treeListControl_SelectionChanged(this, null);
        }

        internal void UndoAction()
        {
            bool selectionchanged = false;
            XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action;
            var list = Document.UndoAction(this, out action);
            switch (action)
            {
                case XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added:
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
                                    if (mapObjectToParent.ContainsKey(obj))
                                    {
                                        var parent = mapObjectToParent[obj];
                                        var item = GetTreeItem(obj, parent);
                                        if (item != null)
                                            DeleteTreeItem(item);
                                    }
                                }
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
                case XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed:
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
                                    if (mapObjectToParent.ContainsKey(obj))
                                    {
                                        var parent = mapObjectToParent[obj];
                                        var item = GetTreeItem(obj, parent);
                                        if (item != null)
                                        {
                                            Refresh(item);
                                            selectionchanged = true;
                                            treeListControl.SelectNode(item);
                                        }
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
                    }
                    break;
                case XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Removed:
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
                                    if (mapObjectToParent.ContainsKey(obj))
                                    {
                                        var parent = mapObjectToParent[obj];
                                        Document.AddExistingObject(obj, parent.Tag);
                                        var item = AddTreeItem(obj, parent);
                                        selectionchanged = true;
                                        treeListControl.SelectNode(item);
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
                    }
                    break;
            }
        }

        internal void RedoAction()
        {
            XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action;
            var list = Document.RedoAction(this, out action);
            switch (action)
            {
                case XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Removed:
                    {
                        if (list.Count > 0)
                        {
                            try
                            {
                                treeListControl.ClearSelection();
                                treeListControl.BeginDataUpdate();
                                foreach (var obj in list)
                                {
                                    if (mapObjectToParent.ContainsKey(obj))
                                    {
                                        var parent = mapObjectToParent[obj];
                                        var item = GetTreeItem(obj, parent);
                                        if (item != null)
                                            DeleteTreeItem(item);
                                    }
                                }
                            }
                            finally
                            {
                                treeListControl.EndDataUpdate();
                            }
                        }
                    }
                    break;
                case XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed:
                    {
                        if (list.Count > 0)
                        {
                            try
                            {
                                treeListControl.ClearSelection();
                                treeListControl.BeginDataUpdate();
                                foreach (var obj in list)
                                {
                                    if (mapObjectToParent.ContainsKey(obj))
                                    {
                                        var parent = mapObjectToParent[obj];
                                        var item = GetTreeItem(obj, parent);
                                        if (item != null)
                                        {
                                            treeListControl.SelectNode(item);
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                treeListControl.EndDataUpdate();
                            }
                        }
                    }
                    break;
                case XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added:
                    {
                        if (list.Count > 0)
                        {
                            try
                            {
                                treeListControl.ClearSelection();
                                treeListControl.BeginDataUpdate();
                                foreach (var obj in list)
                                {
                                    if (mapObjectToParent.ContainsKey(obj))
                                    {
                                        var parent = mapObjectToParent[obj];
                                        Document.AddExistingObject(obj, parent.Tag);
                                        var item = AddTreeItem(obj, parent);
                                        treeListControl.SelectNode(item);
                                    }
                                }
                            }
                            finally
                            {
                                treeListControl.EndDataUpdate();
                            }
                        }
                    }
                    break;
            }
        }

        #endregion


        internal void AddEvent(UFEventObject newnot)
        {
            Document.AddUndoAction(this, newnot, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
            var item = AddTreeItem(newnot, GetSelectedParentFolder());
            treeListControl.ClearSelection();
            treeListControl.SelectNode(item);
        }
        internal void AddFolder(UFEventFolder folder)
        {
            Document.AddUndoAction(this, folder, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
            var item = AddTreeItem(folder, GetSelectedParentFolder(false));
            treeListControl.ClearSelection();
            treeListControl.SelectNode(item);
        }
        #region IDisposable
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            contentToNodeMap.Clear();
            mapObjectToParent.Clear();
            mapObjectToOldParent.Clear();
        }
        #endregion

        bool NeedToBeExpanded(TreeListNode item)
        {
            if (item.Tag is UFEventFolder)
                return true;
            return false;
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

        #region DragDrop

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
                if (!(dragItem.Tag is UFEventObject) &&
                    !(dragItem.Tag is UFEventFolder))
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
            var itemContent = e.TargetRecord as TreeItemControl;
            var bDragOnRoot = e.TargetRowHandle == itemRoot.RowHandle;

            if (draggedNodes.Count() == 0 || itemContent == null || (!bDragOnRoot && !contentToNodeMap.ContainsKey(itemContent)))
                return;

            var targetElement = treeListView.GetNodeByContent(e.TargetRecord);
            if (targetElement == null || (!bDragOnRoot && !(targetElement.Tag is UFEventFolder)) || (e.OriginalSource != treeListView))
                return;

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
                var folder = targetElement.Tag as UFEventFolder;
                var prototype = targetElement.Tag as UFEventObject;

                var mapStartCounter = new Dictionary<string, ulong>();
                var listTagName = Document.GetEventsNameList(folder);
                var listFolderName = Document.GetFoldersNameList(folder);

                var atLeastOneNameAlreadyExists = (from c in draggingItems
                                                   where (c.Tag is UFEventObject && listTagName.Contains((c.Tag as UFEventObject).Name) ||
                                                   (c.Tag is UFEventFolder && listFolderName.Contains((c.Tag as UFEventFolder).Name)))
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
                    Document.AddUndoAction(this, listundo, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Removed);

                var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(Document.GetSession(), Document.GetSession(), false, true, true);
                foreach (var dragItem in draggingItems)
                {
                    var parent = dragItem.ParentNode ?? itemRoot;
                    if (dragItem.Tag is UFEventObject)
                    {
                        parent.Nodes.Remove(dragItem);
                        var item = cloneHelper.Clone(dragItem.Tag as UFEventObject);
                        (dragItem.Tag as UFEventObject).Delete();
                        dragItem.Tag = item;
                        item.Name = Document.NewEventName(folder, item.Name, mapStartCounter, listTagName);
                        item.UFEventFolder = folder;
                    }
                    else if (dragItem.Tag is UFEventFolder)
                    {
                        parent.Nodes.Remove(dragItem);
                        var item = cloneHelper.Clone(dragItem.Tag as UFEventFolder);
                        (dragItem.Tag as UFEventFolder).Delete();
                        dragItem.Tag = item;
                        item.Name = Document.NewFolderName(folder, item.Name, mapStartCounter, listFolderName);
                        item.UFEventFolderAss = folder;
                    }
                }

                listundo = (from item in draggingItems
                            where IsUndoRedoSupported(item.Tag) && !draggingItems.Contains(item.ParentNode)
                            select item.Tag as IXPSimpleObject).ToList();

                if (listundo.Count > 0)
                    Document.AddUndoAction(this, listundo, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);

                var expanded = targetElement.IsExpanded || targetElement.Nodes.Count > 0 && targetElement.Nodes[0].Tag != TreeListControlHelper.DummyNode;
                treeListControl.ClearSelection();
                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    var toSelect = new List<TreeListNode>();
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
                                    toSelect.Add(item);
                            }
                            else
                            {
                                var item = treeListControl.GetTreeItem(tag, targetElement);
                                if (item != null)
                                    toSelect.Add(item);
                            }
                        });
                    }
                    finally
                    {
                        treeListControl.EndDataUpdate();
                        if (toSelect.Count > 0)
                            treeListControl.SelectNodes(toSelect, true, true);
                    }
                });
            }
        }

        void TreeListControl_CompletedDragDrop(object sender, CompleteRecordDragDropEventArgs e)
        {
            e.Handled = true;
        }

        //private void treeListControl_Drop(object sender, DragEventArgs e)
        //{
        //    if (e.Source as TreeViewItemAdv == null ||
        //        ((e.Source as TreeViewItemAdv).Header as rootHeader == null &&
        //        ((e.Source as TreeViewItemAdv).Tag == null ||
        //            ((e.Source as TreeViewItemAdv).Tag as UFEventFolder) == null))
        //        )
        //        return;

        //    UFEventFolder newfolder = null;

        //    if ((e.Source as TreeViewItemAdv).Tag != null &&
        //        ((e.Source as TreeViewItemAdv).Tag as UFEventFolder) != null)
        //        newfolder = (e.Source as TreeViewItemAdv).Tag as UFEventFolder;

        //    if (e.Data.GetDataPresent(typeof(RecordDragDropData)))
        //    {
        //        var data = e.Data.GetData(typeof(RecordDragDropData)) as RecordDragDropData;
        //        if (data == null)
        //            return;

        //        foreach (var v in data.Records)
        //        {
        //            var subitem = v as TreeItemControl;
        //            if (subitem == null || subitem.TreeItemInnerObject == null)
        //                continue;
        //            if (subitem.TreeItemInnerObject is UFEventObject)
        //            {
        //                var ev = subitem.TreeItemInnerObject as UFEventObject;
        //                ev.UFEventFolder = newfolder;
        //            }
        //            else if (subitem.TreeItemInnerObject is UFEventFolder)
        //            {
        //                var ev = subitem.TreeItemInnerObject as UFEventFolder;
        //                ev.UFEventFolderAss = newfolder;
        //            }

        //        }
        //    }
        //}

        //private void treeListControl_DragStart(object sender, DragTreeViewItemAdvEventArgs e)
        //{
        //    if (e.DraggingItems == null)
        //    {
        //        e.AllowDragDrop = false;
        //        e.Cancel = true;
        //        return;
        //    }

        //    bool bAllow = true;
        //    foreach (var dragItem in e.DraggingItems)
        //    {
        //        if (!(dragItem.Tag is UFEventObject) &&
        //            !(dragItem.Tag is UFEventFolder))
        //        {
        //            bAllow = false;
        //            break;
        //        }
        //    }

        //    e.AllowDragDrop = bAllow;
        //    e.Cancel = !bAllow;
        //}

        //private void treeListControl_DragEnd(object sender, DragTreeViewItemAdvEventArgs e)
        //{
        //    e.AllowDragDrop = false;
        //    e.Cancel = true;

        //    var targetElement = e.TargetDropItem as TreeListNode;
        //    if (targetElement == null ||
        //        (targetElement != itemRoot && !(targetElement.Tag is UFEventFolder)) ||
        //        (e.OriginalSource as TreeListControl != treeListControl))
        //    {
        //        return;
        //    }

        //    var draggingItems = new List<TreeListNode>();
        //    foreach (var dragItem in e.DraggingItems)
        //    {
        //        var parent = dragItem.ParentNode ?? itemRoot;
        //        if (parent == targetElement || dragItem == targetElement)
        //            continue;

        //        if (dragItem.Tag is IXPSimpleObject)
        //            draggingItems.Add(dragItem);
        //    }

        //    if (draggingItems.Count > 0)
        //    {
        //        var folder = targetElement.Tag as UFEventFolder;
        //        var mapStartCounter = new Dictionary<string, long>();
        //        var listTagName = Document.GetEventsNameList(folder);
        //        var listFolderName = Document.GetFoldersNameList(folder);

        //        var atLeastOneNameAlreadyExists = (from c in draggingItems
        //                                           where (c.Tag is UFEventObject && listTagName.Contains((c.Tag as UFEventObject).Name) ||
        //                                           (c.Tag is UFEventFolder && listFolderName.Contains((c.Tag as UFEventFolder).Name)))
        //                                           select c).Any();

        //        if (atLeastOneNameAlreadyExists)
        //        {
        //            var uiInterface = Document.EditorManagerComponent.UIInterface;
        //            if (uiInterface == null ||
        //                uiInterface.ShowYesNo(Properties.Resources.DropAlreadyExistsWarning.Replace("-newline-", Environment.NewLine),
        //                UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question) == UIMsgBoxAlertService.ComponentService.CustomDialogResults.No)
        //            {
        //                return;
        //            }
        //        }

        //        foreach (var dragItem in draggingItems)
        //        {
        //            var parent = dragItem.ParentNode ?? itemRoot;
        //            if (dragItem.Tag is UFEventObject)
        //            {
        //                parent.Nodes.Remove(dragItem);
        //                var item = dragItem.Tag as UFEventObject;
        //                item.Name = Document.NewEventName(folder, item.Name, mapStartCounter, listTagName);
        //                item.UFEventFolder = folder;
        //            }
        //            else if (dragItem.Tag is UFEventFolder)
        //            {
        //                parent.Nodes.Remove(dragItem);
        //                var item = dragItem.Tag as UFEventFolder;
        //                item.Name = Document.NewFolderName(folder, item.Name, mapStartCounter, listFolderName);
        //                item.UFEventFolderAss = folder;
        //            }
        //        }

        //        var expanded = targetElement.IsExpanded || targetElement.Nodes.Count > 0 && targetElement.Nodes[0] != DummyNode;
        //        treeListControl.ClearSelection();
        //        Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
        //        {
        //            draggingItems.ForEach(itemAdv =>
        //            {
        //                var tag = itemAdv.Tag as IXPSimpleObject;
        //                if (expanded)
        //                {
        //                    var item = AddTreeItem(tag, targetElement);
        //                    if (item != null)
        //                    {
        //                        treeListControl.SelectNode(item);
        //                    }
        //                }
        //                else
        //                {
        //                    var item = GetTreeItem(tag, targetElement);
        //                    if (item != null)
        //                    {
        //                        treeListControl.SelectNode(item);
        //                    }
        //                }
        //            });
        //        });
        //    }
        //}

        //private void treeListControl_DragOver(object sender, DragTreeViewItemAdvEventArgs e)
        //{
        //    if (e.Source as TreeListControl == null || (e.Source as TreeListControl) != treeListControl)
        //    {
        //        e.Cancel = true;
        //        e.AllowDragDrop = false;
        //    }
        //}

        #endregion

    }
}

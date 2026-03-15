using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MSSchedulerSettings.Document;
using MSEditor.ComponentService;
using Utilities;
using Utilities.WPF;
using MSModel;
using DevExpress.Xpo;
using MSSchedulerSettings.Controls;
using WPFUtilities;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;
using DevExpress.Data.TreeList;
using UIMsgBoxAlertService.ComponentService;

namespace MSEditor.Controls
{
    /// <summary>
    /// Interaction logic for EventList.xaml
    /// </summary>
    public partial class EventList : UserControl, IDisposable
    {
        #region Declarations

        readonly SchedulerEditorDocument Document;
        TreeListNode itemRoot;
        Dictionary<TreeItemControl, TreeListNode> contentToNodeMap = new Dictionary<TreeItemControl, TreeListNode>();
        BitmapImage openFolderImg;
        BitmapImage closedFolderImg;
        bool UnSubscribeSelectionChangedEvent;
        readonly Dictionary<Object, TreeListNode> mapObjectToParent = new Dictionary<Object, TreeListNode>();
        readonly Dictionary<Object, TreeListNode> mapObjectToOldParent = new Dictionary<Object, TreeListNode>();

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

        public EventList(SchedulerEditorDocument doc)
        {
            InitializeComponent();
            Document = doc;

            Document.CreateUndoRedoHelper(this);

            InitializeEventList();
        }
        class rootHeader
        {
            public String Name { get; set; }
        }
        void InitializeEventList()
        {
            openFolderImg = SchedulerEditorManagerComponent.GetBitmapImage("OpenFolderSmall", true);
            closedFolderImg = SchedulerEditorManagerComponent.GetBitmapImage("CloseFolderSmall", true);

            treeListView.Nodes.Clear();
            contentToNodeMap.Clear();
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
                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    if (bDisposed)
                        return;

                    try
                    {
                        UnSubscribeSelectionChangedEvent = true;
                        UpdateContextObjects();
                    }
                    finally
                    {
                        UnSubscribeSelectionChangedEvent = false;
                    }
                });
            }
        }

        public TreeListNode GetRoot()
        {
            return itemRoot;
        }

        private static bool CanBeExpanded(TreeListNode parent)
        {
            return parent.Nodes.Count == 1 && parent.Nodes[0] == TreeListControlHelper.DummyNode;
        }

        //void subitem_Expanded(object sender, RoutedEventArgs e)
        //{
        //    var item = e.OriginalSource as TreeListNode;
        //    if (item == null || item.Tag == null ||
        //        e != null && !CanBeExpanded(item))
        //        return;
        //    ClearNodes(item);
        //    if (item.Tag is MSFolder)
        //        FillItems(item, item.Tag as MSFolder);
        //    e.Handled = true;
        //}

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
                    var listEvents = Document.GetEventsCollection();
                    if (listEvents != null)
                    {
                        foreach (var notifica in listEvents)
                            AddTreeItem(notifica, itemRoot);
                    }
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }
        }

        static int maxItems = Properties.Settings.Default.MaxItemsInTree;
        private void FillItems(TreeListNode itemRoot, MSFolder root = null)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
                ClearNodes(itemRoot);
                using (new WaitCursor())
                {
                    int i = 0;

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

        public class NameModel
        {
            public String Name { get; set; }
        }

        bool NeedToBeExpanded(TreeListNode item)
        {
            if (item.Tag is MSFolder)
            {
                if (Properties.Settings.Default.FolderAlwaysExpandible)
                    return true;    

                var folder = (MSFolder)item.Tag;
                return Document.GetFolderCollection(folder).Count > 0 || Document.GetEventsCollection(folder).Count > 0;
            }
            return false;
        }

        internal TreeListNode AddTreeItem(Object tag, TreeListNode parent)
        {
            if (bDisposed)
                return null;

            bool bKnownType = false;

            if (parent == null)
            {
                parent = itemRoot;
            }

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

            //TreeListNode newitem = new TreeListNode
            //{
            //    Tag = tag,
            //    DataContext = tag
            //};

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

            if (tag is MSScheduledAction)
            {
                bKnownType = true;

                SetBindingOnProp(newitem, tag, "Name");
                (newitem.Content as TreeItemControl).ResourceIcon = SchedulerEditorManagerComponent.GetBitmapImage("SSAction");

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
            else if (tag is MSFolder)
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
            BindingOperations.SetBinding(node.Content as HeaderedItemsControl, targetDP, myBinding);
        }

        internal bool IsAnyItemSelected()
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault() as TreeListNode;
            return item != null && item != itemRoot;
        }

        internal void CopySelectedToClipboard()
        {
            var listfolders = new List<MSFolder>();
            var listevents = new List<MSScheduledAction>();
            foreach (var item in treeListControl.GetSelectedNodes())
            {
                if ((item as TreeListNode).Tag is MSFolder)
                    listfolders.Add((item as TreeListNode).Tag as MSFolder);
                else if ((item as TreeListNode).Tag is MSScheduledAction)
                    listevents.Add((item as TreeListNode).Tag as MSScheduledAction);
            }

            Document.CleanClipbaord();
            Document.CopyListFoldersToClipbaord(listfolders);
            Document.CopyListEventsToClipbaord(listevents);
        }

        internal void PasteFromClipboard()
        {
            try
            {
                var selected = treeListControl.GetSelectedNodes().FirstOrDefault() as TreeListNode;

                var parent = GetSelectedParentFolder(false);
                var expanded = parent.IsExpanded || parent.Nodes.Count > 0 && parent.Nodes[0].Tag != TreeListControlHelper.DummyNode;
                treeListControl.ClearSelection();
                UnSubscribeSelectionChangedEvent = true;
                treeListControl.BeginDataUpdate();

                MSFolder folder = null;
                if (parent.Tag is MSFolder)
                    folder = parent.Tag as MSFolder;

                var nodesToSelect = new List<TreeListNode>();
                var listfolders = Document.PasteClipboardFolders(folder);

                treeListControl.AddDummyNodeIfNeeded(parent, NeedToBeExpanded);
                AddGetNodesList(listfolders, expanded, true, true, parent);

                var listevents = Document.PasteClipboardEvents(folder);

                treeListControl.AddDummyNodeIfNeeded(parent, NeedToBeExpanded);
                AddGetNodesList(listevents, expanded, true, true, parent);
                treeListControl.SortBy("ItemHeader");

                if (listfolders.Count > 0 || listevents.Count > 0)
                {
                    // add list to undo manager
                    var listundo = new List<IXPSimpleObject>();
                    listfolders.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
                    listevents.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
                    Document.AddUndoAction(this, listundo, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
                UnSubscribeSelectionChangedEvent = false;
                ForceSelectionChangedEvent();
            }
        }

        void ForceSelectionChangedEvent()
        {
            treeListControl_SelectionChanged(this, null);
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

        internal void DeleteSelectedItems()
        {
            var items = (from item in treeListControl.GetSelectedNodes()
                         where item.Tag != null
                         select item).ToList();

            var listundo = (from item in items
                            where IsUndoRedoSupported(item.Tag) && !items.Contains(item.ParentNode)
                            select item.Tag as IXPSimpleObject).ToList();
            var uiMsgBox = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
            if (uiMsgBox != null)
            {
                var bShowMessage = (from item in items/*.AsParallel()*/
                                    where item.Tag is MSFolder &&
                                    listundo.Contains(item.Tag as MSFolder) &&
                                    ((item.Tag as MSFolder).MSScheduledActions.Count > 0 ||
                                    (item.Tag as MSFolder).MSFolders.Count > 0)
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
        }

        internal void DeleteTreeItem(TreeListNode item)
        {
            //var item = addressSpaceTree.SelectedItem as TreeListNode;
            var parent = item.ParentNode ?? itemRoot;
            if (item.Tag is MSScheduledAction)
            {
                var tag = item.Tag as MSScheduledAction;
                //GetSelectedParentFolder(true, true).Items.Remove(item);
                parent.Nodes.Remove(item);
                tag.Delete();

                if (IsUndoRedoSupported(tag))
                    mapObjectToOldParent[tag] = item;
            }
            else if (item.Tag is MSFolder)
            {
                var folder = item.Tag as MSFolder;
                //GetSelectedParentFolder(true).Items.Remove(item);
                parent.Nodes.Remove(item);
                folder.Delete();

                if (IsUndoRedoSupported(folder))
                    mapObjectToOldParent[folder] = item;
            }
        }

        public TreeListNode GetSelectedParentFolder(bool bSkipSelected = false)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (!bSkipSelected && selected != null && selected.Tag is MSFolder)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode as TreeListNode;
                if (selected != null && selected.Tag is MSFolder)
                    return selected;
            }
            return itemRoot;
        }

        internal MSFolder GetSelectedFolder()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is MSFolder)
                return selected.Tag as MSFolder;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is MSFolder)
                    return selected.Tag as MSFolder;
            }

            return null;
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

        internal void UndoAction()
        {
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
                                treeListControl.BeginDataUpdate();
                                foreach (var obj in list)
                                {
                                    if (mapObjectToParent.ContainsKey(obj))
                                    {
                                        var parent = mapObjectToParent[obj];
                                        var item = treeListControl.GetTreeItem(obj, parent);
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
                                        var item = treeListControl.GetTreeItem(obj, parent);
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
                                        var item = treeListControl.GetTreeItem(obj, parent);
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
                                        var item = treeListControl.GetTreeItem(obj, parent);
                                        if (item != null)
                                        {
                                            Refresh(item);
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
                        var parent = selected.ParentNode as TreeListNode;
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
                        EditSelectedItem();
                }
            }
        }

        internal void EditSelectedItem()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null)
            {
                if (selected.Tag is MSScheduledAction)
                {
                    var adtag = selected.Tag as MSScheduledAction;
                    EditEvent(adtag);
                }
                else if (selected.Tag is MSFolder)
                    EditFolder(selected.Tag as MSFolder);
            }
        }

        private void EditFolder(MSFolder folder)
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
        private void EditEvent(MSScheduledAction adnot)
        {
            if (adnot == null)
                return;
            using (var uow = Document.BeginNestedUnitOfWork())
            {
                var newEvControl = new NewEventControl(Document, styleName: ThemeImageHelper.GetTheme(Document), bEditing: true) { DataContext = uow.GetNestedObject(adnot) };
                GeneralDialogContent Dialog = new GeneralDialogContent(newEvControl)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "EventEditor"
                };
                if (Dialog.ShowDialog() == true)
                {
                    Document.AddUndoAction(this, adnot, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed);
                    uow.CommitChanges();
                    UpdateContextObjects();
                }
            }
        }

        internal void AddFolder(MSFolder folder)
        {
            Document.AddUndoAction(this, folder, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
            var item = AddTreeItem(folder, GetSelectedParentFolder());
            treeListControl.ClearSelection();
            treeListControl.SelectNode(item);
        }

        internal void AddEvent(MSScheduledAction evt)
        {
            Document.AddUndoAction(this, evt, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
            var item = AddTreeItem(evt, GetSelectedParentFolder());
            treeListControl.ClearSelection();
            treeListControl.SelectNode(item);
        }

        #region IDisposable
        bool bDisposed;
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

        #region DragDrop
        private void treeListControl_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(RecordDragDropData)))
                return;

            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void addressSpaceTree_PreviewQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (!e.EscapePressed)
                return;

            e.Action = DragAction.Cancel;
            e.Handled = true;
        }

        //private void EventsTree_DragStart(object sender, DragTreeViewItemAdvEventArgs e)
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
        //        if (!(dragItem.Tag is MSScheduledAction) &&
        //            !(dragItem.Tag is MSFolder))
        //        {
        //            bAllow = false;
        //            break;
        //        }
        //    }

        //    e.AllowDragDrop = bAllow;
        //    e.Cancel = !bAllow;
        //}


        //private void EventsTree_DragOver(object sender, DragTreeViewItemAdvEventArgs e)
        //{
        //    if (e.Source as TreeViewAdv == null || (e.Source as TreeViewAdv) != treeListControl)
        //    {
        //        e.Cancel = true;
        //        e.AllowDragDrop = false;
        //    }
        //}        

        private void TreeListControl_DragEnd(object sender, DropRecordEventArgs e)
        {
            e.Handled = true;
            var targetDataLoggerSetting = (e.TargetRecord as TreeItemControl)?.TreeItemInnerObject as MSFolder;
            var targetTreeNode = treeListView.GetNodeByContent(e.TargetRecord);
            if ((targetDataLoggerSetting != null || targetTreeNode == itemRoot) && targetTreeNode != null)
            {
                var data = e.Data.GetData(typeof(RecordDragDropData)) as RecordDragDropData;
                if (data == null)
                    return;
                var draggingItems = new List<TreeListNode>();

                foreach (var v in data.Records)
                {
                    var subitem = v as TreeItemControl;
                    if (subitem == null || subitem.TreeItemInnerObject == null)
                        continue;

                    var draggedNode = treeListView.GetNodeByContent(subitem);
                    var parent = draggedNode.ParentNode ?? itemRoot;
                    if (parent == targetTreeNode || draggedNode == targetTreeNode)
                        continue;

                    draggingItems.Add(draggedNode);
                }

                if (draggingItems.Count > 0)
                {
                    var folder = targetTreeNode.Tag as MSFolder;
                    var mapStartCounter = new Dictionary<string, ulong>();
                    var listTagName = Document.GetEventsNameList(folder);
                    var listFolderName = Document.GetFoldersNameList(folder);
                    var parentFoldersToUpdate = new List<TreeListNode>();

                    var atLeastOneNameAlreadyExists = (from c in draggingItems
                                                        where (c.Tag is MSScheduledAction && listTagName.Contains((c.Tag as MSScheduledAction).Name) ||
                                                        (c.Tag is MSFolder && listFolderName.Contains((c.Tag as MSFolder).Name)))
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
                        if (parent.Nodes.Count > maxItems && !parentFoldersToUpdate.Contains(parent) && (parent == itemRoot || parent.Tag is UFUAModel.UFUAFolder))
                            parentFoldersToUpdate.Add(parent);

                        if (dragItem.Tag is MSScheduledAction)
                        {
                            parent.Nodes.Remove(dragItem);
                            var tag = cloneHelper.Clone(dragItem.Tag as MSScheduledAction);
                            (dragItem.Tag as MSScheduledAction).Delete();
                            dragItem.Tag = tag;
                            tag.Name = Document.NewEventName(folder, tag.Name, mapStartCounter, listTagName);
                            tag.MSFolderAss = folder;
                        }
                        else if (dragItem.Tag is MSFolder)
                        {
                            parent.Nodes.Remove(dragItem);
                            var tag = cloneHelper.Clone(dragItem.Tag as MSFolder);
                            (dragItem.Tag as MSFolder).Delete();
                            dragItem.Tag = tag;
                            tag.Name = Document.NewFolderName(folder, tag.Name, mapStartCounter, listFolderName);
                            tag.MSFolderAss = folder;
                        }
                    }

                    listundo = (from item in draggingItems
                                where IsUndoRedoSupported(item.Tag) && !draggingItems.Contains(item.ParentNode)
                                select item.Tag as IXPSimpleObject).ToList();

                    if (listundo.Count > 0)
                        Document.AddUndoAction(this, listundo, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);

                    TreeListNode lastItem = null;
                    var expanded = targetTreeNode.IsExpanded || targetTreeNode.Nodes.Count > 0 && targetTreeNode.Nodes[0].Tag != TreeListControlHelper.DummyNode;
                    treeListControl.ClearSelection();

                    try
                    {
                        treeListControl.BeginDataUpdate();
                        foreach (var dragItem in draggingItems)
                        {
                            var newtag = dragItem.Tag as IXPSimpleObject;
                            TreeListNode item = null;
                            treeListControl.AddDummyNodeIfNeeded(targetTreeNode, NeedToBeExpanded);
                            if (expanded)
                                item = AddTreeItem(newtag, targetTreeNode);
                            else
                                item = treeListControl.GetTreeItem(newtag, targetTreeNode);
                            if (item != null)
                                lastItem = item;
                        }
                    }
                    finally
                    {
                        treeListControl.EndDataUpdate();
                        if (lastItem != null)
                            treeListControl.SelectNode(lastItem);
                    }

                    foreach (var oldParent in parentFoldersToUpdate)
                    {
                        ClearNodes(oldParent);
                        if (oldParent == itemRoot)
                            FillItems(itemRoot);
                        else
                        {
                            oldParent.IsExpanded = false;
                            treeListControl.AddNode(null, oldParent, TreeListControlHelper.DummyNode);
                            oldParent.IsExpanded = true;
                        }
                    }
                    //treeListControl.SortBy("ItemHeader");
                }
            }
        }

        void TreeListControl_CompletedDragDrop(object sender, CompleteRecordDragDropEventArgs e)
        {
            e.Handled = true;
        }

        //private void EventsTree_Drop(object sender, DragEventArgs e)
        //{
        //    if (e.Source as TreeListNode == null ||
        //        ((e.Source as TreeListNode).Header as rootHeader == null &&
        //        ((e.Source as TreeListNode).Tag == null ||
        //            ((e.Source as TreeListNode).Tag as MSFolder) == null))
        //        )
        //        return;

        //    MSFolder newfolder = null;

        //    if ((e.Source as TreeListNode).Tag != null &&
        //        ((e.Source as TreeListNode).Tag as MSFolder) != null)
        //        newfolder = (e.Source as TreeListNode).Tag as MSFolder;

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
        //            if (subitem.TreeItemInnerObject is MSScheduledAction)
        //            {
        //                var ev = subitem.TreeItemInnerObject as MSScheduledAction;
        //                ev.MSFolderAss = newfolder;
        //            }
        //            else if (subitem.TreeItemInnerObject is MSFolder)
        //            {
        //                var ev = subitem.TreeItemInnerObject as MSFolder;
        //                ev.MSFolderAss = newfolder;
        //            }

        //        }
        //    }
        //}
        #endregion

        void OnTreeNodeCollapsing(object sender, TreeListNodeAllowEventArgs e)
        {
            if (!(e.Node.Content as TreeItemControl).IsNodeExpanding)
                UpdateFolderIcon(e.Node, false);
        }

        void UpdateFolderIcon(TreeListNode node, bool isOpen)
        {
            if (node != null && (node == itemRoot || node.Tag is MSModel.MSFolder))
                (node.Content as TreeItemControl).ResourceIcon = isOpen ? openFolderImg : closedFolderImg;
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

        void Refresh(TreeListNode item)
        {
            if (item == itemRoot)
                return;

            var wasExpanded = item.IsExpanded;
            if (item.IsExpanded && item.Nodes.Count == 0)
                treeListControl.AddNode(null, item, TreeListControlHelper.DummyNode);
            item.IsExpanded = false;
            ClearNodes(item);

            treeListControl.AddNode(null, item, TreeListControlHelper.DummyNode);
            if (wasExpanded)
                item.IsExpanded = true;
        }

        void OnTreeNodeExpanding(object sender, TreeListNodeAllowEventArgs e)
        {
            TreeListNode item = e.Node;
            try
            {
                if (item == null || TreeListControlHelper.WasExpanded(item)) //Node's subtree already populated
                    return;               

                (item.Content as TreeItemControl).IsNodeExpanding = true;
                treeListControl.BeginDataUpdate();

                ClearNodes(item);
                if (item.Tag is MSFolder)
                    FillItems(item, item.Tag as MSFolder);

                treeListControl.EndDataUpdate();
            }
            finally
            {
                (item.Content as TreeItemControl).IsNodeExpanding = false;
            }
        }

        void OnTreeNodeExpanded(object sender, TreeListNodeEventArgs e)
        {
            TreeListNode item = e.Node;
            if (item != null)
                UpdateFolderIcon(item, item.IsExpanded);
        }
    }
}

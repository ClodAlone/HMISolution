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
using ADEditor.Document;
using ADEditor.ComponentService;
using Utilities.WPF;
using Utilities;
using System.Reflection;
using ADPluginSettingsInterface;
using ADModel;
using UFUAModel;
using DevExpress.Xpo;
using ADPluginInterfaces;
using WPFUtilities;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using UIMsgBoxAlertService.ComponentService;
using System.ComponentModel;
using DevExpress.Xpf.Core;
using DocumentManager.ComponentService;
using DevExpress.Data.TreeList;

namespace ADEditor.Controls
{
    /// <summary>
    /// Interaction logic for NotificationList.xaml
    /// </summary>
    public partial class NotificationList : UserControl, IDisposable
    {
        #region Declarations
        BitmapImage openFolderImg;
        BitmapImage closedFolderImg;
        readonly ADEditorDocument Document;
        TreeListNode itemRoot;
        Dictionary<TreeItemControl, TreeListNode> contentToNodeMap = new Dictionary<TreeItemControl, TreeListNode>();
        readonly Dictionary<Object, TreeListNode> mapObjectToParent = new Dictionary<Object, TreeListNode>();
        readonly Dictionary<Object, TreeListNode> mapObjectToOldParent = new Dictionary<Object, TreeListNode>();
        readonly IEditableObject editableOwner;
        bool UnSubscribeSelectionChangedEvent;

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
        public NotificationList(ADEditorDocument doc, IEditableObject owner)
        {
            InitializeComponent();
            Document = doc;
            editableOwner = owner;

            Document.CreateUndoRedoHelper(this);

            InitializeNotificationsList();
        }
        #endregion

        #region Notifications

        class rootHeader
        {
            public String Name { get; set; }
        }

        void InitializeNotificationsList()
        {
            openFolderImg = ADEditorManagerComponent.GetBitmapImage("OpenFolderSmall", true);
            closedFolderImg = ADEditorManagerComponent.GetBitmapImage("CloseFolderSmall", true);

            treeListView.Nodes.Clear();
            itemRoot = treeListControl.AddNode(new NotificationTreeItemControl(Properties.Resources.RootName) { ResourceIcon = closedFolderImg }, Tag as TreeListNode, Document);
            treeListControl.AddNode(null, itemRoot, TreeListControlHelper.DummyNode);

            foreach (var item in contextMenu.Items)
            { 
                var mnu = item as MenuItem;
                if (mnu != null && mnu.Name == "mnuNewNotification")
                    mnu.Visibility = System.Windows.Visibility.Collapsed;
            }

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

                try
                {
                    UnSubscribeSelectionChangedEvent = true;
                    UpdateContextObjects();
                }
                finally
                {
                    UnSubscribeSelectionChangedEvent = false;
                }
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
                    var listNotifications = Document.GetNotificationsCollection();//Document.GetNotifications();
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
            if (node != null && (node == itemRoot || node.Tag is ADModel.ADFolder))
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
                if (item == null || TreeListControlHelper.WasExpanded(item)) //Node's subtree already populated
                    return;                

                (item.Content as TreeItemControl).IsNodeExpanding = true;
                treeListControl.BeginDataUpdate();

                ClearNodes(item);
                if (item.Tag is ADFolder)
                    FillItems(item, item.Tag as ADFolder);

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

        public class NameModel
        {
            public String Name { get; set; }
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
            if (item.Tag is ADFolder)
            {
                if (Properties.Settings.Default.FolderAlwaysExpandible)
                    return true;

                var folder = item.Tag as ADFolder;
                return Document.GetFolderCollection(folder).Count > 0 || Document.GetNotificationsCollection(folder).Count > 0;
            }
            
            return false;
        }

        internal TreeListNode AddTreeItem(Object tag, TreeListNode parent)
        {
            if (bDisposed)
                return null;

            bool bKnownType = false;

            if (parent == null)
                parent = itemRoot;

            if (!parent.IsExpanded && !(parent.Content as TreeItemControl).IsNodeExpanding)
            {
                parent.IsExpanded = true;
                var list = (from p in parent.Nodes where p.Tag == tag select p).ToList();
                if (list.Count > 0)
                    return list[0];
            }

            var ic = new NotificationTreeItemControl(tag);
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

            if (tag is ADNotification)
            {
                bKnownType = true;

                var n = tag as ADNotification;
                SetBindingOnProp(newitem, tag, nameof(n.Name));
                SetBindingOnProp(newitem, tag, nameof(n.Message), NotificationTreeItemControl.MessageProperty);
                SetBindingOnProp(newitem, tag, nameof(n.Priority), NotificationTreeItemControl.PriorityProperty);
                SetBindingOnProp(newitem, tag, nameof(n.PluginName), NotificationTreeItemControl.PluginNameProperty);

                (newitem.Content as TreeItemControl).ResourceIcon = ADEditorManagerComponent.GetBitmapImage("ADNotificationSmall");

                if (!bShiftDown && parent.Nodes.Count > maxItems)
                {
                    parent.Nodes.RemoveAt(0);
                    var maxitem = GetTreeItem(Properties.Resources.MaxItemCountVisibleReachedDoubleClick, parent);
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
            else if (tag is ADFolder)
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
            BindingOperations.SetBinding(node.Content as NotificationTreeItemControl, targetDP, myBinding);
        }

        private TreeListNode GetTreeItem(Object tag, TreeListNode parent = null)
        {
            if (parent == null)
                parent = itemRoot;

            if (!parent.IsExpanded)
                parent.IsExpanded = true;

            var list = (from p in parent.Nodes.OfType<TreeListNode>() where p.Tag == tag select p).ToList();
            if (list.Count > 0)
                return list[0];

            return null;
        }

        internal bool IsAnyItemSelected()
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            return item != null && item != itemRoot;
        }

        public object GetRootItem()
        {
            return itemRoot.Tag;
        }

        internal void CopySelectedToClipboard()
        {
            var listfolders = new List<ADFolder>();
            var listtags = new List<ADNotification>();
            var selecteditems = treeListControl.GetSelectedNodes();
            foreach (var item in selecteditems)
            {
                if (item.Tag is ADFolder)
                    listfolders.Add(item.Tag as ADFolder);
                else if (item.Tag is ADNotification)
                    listtags.Add(item.Tag as ADNotification);
            }

            Document.CleanClipbaord();
            Document.CopyListFoldersToClipbaord(listfolders);
            Document.CopyListNotificationsToClipbaord(listtags);
        }

        internal void PasteFromClipboard()
        {
            bool selectionchanged = false;
            var pastedTags = new List<TreeListNode>();
            try
            {
                var selected = treeListControl.GetSelectedNodes().FirstOrDefault() as TreeListNode;

                var parent = GetSelectedParentFolder();
                var expanded = parent.IsExpanded || parent.Nodes.Count > 0 && parent.Nodes[0].Tag != TreeListControlHelper.DummyNode;
                treeListControl.ClearSelection();
                UnSubscribeSelectionChangedEvent = true;
                treeListControl.BeginDataUpdate();

                ADFolder folder = null;
                if (parent.Tag is ADFolder)
                    folder = parent.Tag as ADFolder;

                var listfolders = Document.PasteClipboardFolders(folder);

                AddGetNodesList(listfolders, expanded, true, true, parent);

                var listtags = Document.PasteClipboardNotifications(folder);
                listtags.ForEach(tag =>
                {
                    var item = AddTreeItem(tag, parent);
                    if (item != null)
                        pastedTags.Add(item);
                    if (folder != null)
                        folder.ADNotifications.Add(tag);
                });

                if (listfolders.Count > 0 || listtags.Count > 0)
                {
                    selectionchanged = true;
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
                if (selectionchanged)
                    ForceSelectionChangedEvent();
            }
        }

        private void ForceSelectionChangedEvent()
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

        static int maxItems = Properties.Settings.Default.MaxItemsInTree;
        private void FillItems(TreeListNode itemRoot, ADFolder root = null)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                int i = 0;
                bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

                using (new WaitCursor())
                {
                    ClearNodes(itemRoot);
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

                    var listTags = Document.GetNotificationsCollection(root);
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
                         where item.Tag != null
                         select item).ToList();

            var listundo = (from item in items
                            where IsUndoRedoSupported(item.Tag) && !items.Contains(item.ParentNode)
                            select item.Tag as IXPSimpleObject).ToList();
            var uiMsgBox = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
            if (uiMsgBox != null)
            {
                var bShowMessage = (from item in items/*.AsParallel()*/
                                    where item.Tag is ADFolder &&
                                    listundo.Contains(item.Tag as ADFolder) &&
                                    ((item.Tag as ADFolder).ADNotifications.Count > 0 ||
                                    (item.Tag as ADFolder).ADFolders.Count > 0)
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
            if (item.Tag is ADNotification)
            {
                var parent = item.ParentNode ?? itemRoot;
                var tag = item.Tag as ADNotification;
                parent.Nodes.Remove(item);
                tag.Delete();

                if (IsUndoRedoSupported(tag))
                    mapObjectToOldParent[tag] = item;
            }
            else if (item.Tag is ADFolder)
            {
                var parent = item.ParentNode ?? itemRoot;
                var folder = item.Tag as ADFolder;
                parent.Nodes.Remove(item);
                folder.Delete();

                if (IsUndoRedoSupported(folder))
                    mapObjectToOldParent[folder] = item;
            }
        }

        public TreeListNode GetSelectedParentFolder(bool bSkipSelected = false)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (!bSkipSelected && selected != null && selected.Tag is ADFolder)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode as TreeListNode;
                if (selected != null && selected.Tag is ADFolder)
                    return selected;
            }

            return itemRoot;
        }
        internal ADFolder GetSelectedFolder()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is ADFolder)
                return selected.Tag as ADFolder;
            while (selected != null)
            {
                selected = selected.ParentNode as TreeListNode;
                if (selected != null && selected.Tag is ADFolder)
                    return selected.Tag as ADFolder;
            }

            return null;
        }

        internal void AddNotification(ADNotification newnot)
        {
            Document.AddUndoAction(this, newnot, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
            var item = AddTreeItem(newnot, GetSelectedParentFolder());
            treeListControl.ClearSelection();
            treeListControl.SelectNode(item);
        }

        internal void AddFolder(ADFolder folder)
        {
            Document.AddUndoAction(this, folder, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
            var item = AddTreeItem(folder, GetSelectedParentFolder());
            treeListControl.ClearSelection();
            treeListControl.SelectNode(item);
        }

        private void EditFolder(ADFolder folder)
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
        private void EditNotification(ADNotification adnot)
        {
            if (adnot == null)
                return;

            using (var uow = Document.BeginNestedUnitOfWork())
            {
                var newNotControl = new NewNotification(Document)
                {
                    DataContext = uow.GetNestedObject(adnot)
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newNotControl)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "EditNotification"
                };
                if (Dialog.ShowDialog() == true)
                {
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
                if (selected.Tag is ADNotification)
                {
                    var item = selected.Tag as ADNotification;
                    EditNotification(item);
                }
                else if (selected.Tag is ADFolder)
                {
                    var item = selected.Tag as ADFolder;
                    EditFolder(item);
                }
            }
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
                if (c as TreeItemControl != null && contentToNodeMap.ContainsKey(c as TreeItemControl))
                    draggedNodes.Add(contentToNodeMap[c as TreeItemControl]);
            }

            bool bAllow = true;
            foreach (var dragItem in draggedNodes)
            {
                if (!(dragItem.Tag is ADNotification) && !(dragItem.Tag is ADFolder))
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
            var targetContent = e.TargetRecord as TreeItemControl;
            var bDroppingOnRoot = itemRoot.RowHandle == e.TargetRowHandle;

            if (draggedNodes.Count() == 0 || targetContent == null || (!bDroppingOnRoot && !contentToNodeMap.ContainsKey(targetContent)) || e.OriginalSource != treeListView)
                return;

            TreeListNode targetItem;
            if (bDroppingOnRoot)
                targetItem = itemRoot;
            else
                targetItem = contentToNodeMap[targetContent];

            if (targetItem == null || (targetItem != itemRoot && !(targetItem.Tag is ADFolder)))
                return;

            List<TreeListNode> draggingItems = new List<TreeListNode>();
            foreach (var dragItem in draggedNodes)
            {
                var parent = dragItem.ParentNode ?? itemRoot;
                if (parent == targetItem || dragItem == targetItem)
                    continue;

                if (dragItem.Tag is IXPSimpleObject)
                    draggingItems.Add(dragItem);
            }

            if (draggingItems.Count > 0)
            {
                var folder = targetItem.Tag as ADFolder;
                var mapStartCounter = new Dictionary<string, ulong>();
                var listTagName = Document.GetNotificationsNameList(folder);
                var listFolderName = Document.GetFoldersNameList(folder);
                var parentFoldersToUpdate = new List<TreeListNode>();

                var atLeastOneNameAlreadyExists = (from c in draggingItems
                                                   where (c.Tag is ADNotification && listTagName.Contains((c.Tag as ADNotification).Name) ||
                                                   (c.Tag is ADFolder && listFolderName.Contains((c.Tag as ADFolder).Name)))
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
                    if (parent.Nodes.Count > maxItems && !parentFoldersToUpdate.Contains(parent) && (parent == itemRoot || parent.Tag is ADFolder))
                        parentFoldersToUpdate.Add(parent);
                    if (dragItem.Tag is ADNotification)
                    {
                        parent.Nodes.Remove(dragItem);
                        var tag = cloneHelper.Clone(dragItem.Tag as ADNotification);
                        (dragItem.Tag as ADNotification).Delete();
                        dragItem.Tag = tag;
                        tag.Name = Document.NewNotificationName(folder, tag.Name, mapStartCounter, listTagName);
                        tag.ADFolder = folder;
                    }
                    else if (dragItem.Tag is ADFolder)
                    {
                        parent.Nodes.Remove(dragItem);
                        var tag = cloneHelper.Clone(dragItem.Tag as ADFolder);
                        (dragItem.Tag as ADFolder).Delete();
                        dragItem.Tag = tag;
                        tag.Name = Document.NewFolderName(folder, tag.Name, mapStartCounter, listFolderName);
                        tag.ADFolderAss = folder;
                    }
                }

                listundo = (from item in draggingItems
                            where IsUndoRedoSupported(item.Tag) && !draggingItems.Contains(item.ParentNode)
                            select item.Tag as IXPSimpleObject).ToList();

                if (listundo.Count > 0)
                    Document.AddUndoAction(this, listundo, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);

                TreeListNode lastItem = null;
                var expanded = targetItem.IsExpanded || targetItem.Nodes.Count > 0 && targetItem.Nodes[0].Tag != TreeListControlHelper.DummyNode;
                treeListControl.ClearSelection();

                try
                {
                    treeListControl.BeginDataUpdate();
                    foreach (var dragItem in draggingItems)
                    {
                        var newtag = dragItem.Tag as IXPSimpleObject;
                        TreeListNode item = null;
                        if (expanded)
                            item = AddTreeItem(newtag, targetItem);
                        else
                            item = treeListControl.GetTreeItem(newtag, targetItem);
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
            }
        }

        void TreeListControl_CompletedDragDrop(object sender, CompleteRecordDragDropEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

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
                                UnSubscribeSelectionChangedEvent = false;
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
                                        if (item != null)
                                        {
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

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            Document.EditorManagerComponent.Workspace.ContextObject = null;

            contentToNodeMap.Clear();
            mapObjectToParent.Clear();
            mapObjectToOldParent.Clear();
        }
        #endregion

        private void treeListView_CustomColumnSort(object sender, TreeListCustomColumnSortEventArgs e)
        {
            if (e.Column.FieldName == "Priority")
            {
                try
                {
                    int Priority1 = int.Parse((string)e.Value1);
                    int Priority2 = int.Parse((string)e.Value2);
                    e.Result = (Priority1 < Priority2 ? -1 : (Priority1 > Priority2 ? 1 : 0));
                    e.Handled = true;
                }
                catch
                {
                    e.Handled = false;
                }
            }
        }
    }
}

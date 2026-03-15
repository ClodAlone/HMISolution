using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UFUserEditor.Document;
using System.ComponentModel;
using Utilities;
using Utilities.WPF;
using UFUserEditor.ComponentService;
using UFUserEditor.Controls;
using DevExpress.Xpo;
using UIMsgBoxAlertService.ComponentService;
using WPFUtilities;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using System.Windows.Data;

namespace UFUserEditor
{
    /// <summary>
    /// Interaction logic for UFUserEditorControl.xaml
    /// </summary>
    public partial class UFUserEditorControl : UserControl, IEditableObject, IDisposable
    {
        #region Declarations
        readonly BitmapImage userImg;
        readonly BitmapImage roleImg;
        readonly BitmapImage editorImg;
        readonly TreeListNode itemRoot;

        readonly Dictionary<Object, TreeListNode> mapObjectToParent = new Dictionary<Object, TreeListNode>();
        readonly Dictionary<Object, TreeListNode> mapObjectToOldParent = new Dictionary<Object, TreeListNode>();

        readonly int maxRuntimeEditAccessLevel;
        readonly bool isPopup;

        bool UnSubscribeSelectionChangedEvent;
        #endregion

        bool bLoaded;
        public bool IsLoaded
        {
            get
            {
                return bLoaded;
            }
        }
        #region Ctors
        public UFUserEditorControl(UFUserDocument doc, bool bIsPopup = false)
        {
            InitializeComponent();
            Document = doc;

            Document.CreateUndoRedoHelper(this);

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;

                bLoaded = true;
            };
            Unloaded += (o, e) =>
            {
                bLoaded = false;
            };

            if (bIsPopup)
                toolbar.Visibility = System.Windows.Visibility.Visible;

            isPopup = bIsPopup;
            if (bIsPopup && Document.GetGeneralUserSettings(true).MaxRuntimeEditAccessLevel.HasValue)
                maxRuntimeEditAccessLevel = Document.GetGeneralUserSettings(true).MaxRuntimeEditAccessLevel.Value;

            if (isPopup)
                gridDataControl.ItemsSource = (from c in Document.GetFlatUserCollection() where c.GlobalAccessLevel <= maxRuntimeEditAccessLevel select c);
            else
                gridDataControl.ItemsSource = Document.GetFlatUserCollection();

            userImg = UFUserEditorManagerComponent.GetBitmapImage("UFUSRUserSmall");
            roleImg = UFUserEditorManagerComponent.GetBitmapImage("UFUSRGroupSmall");
            editorImg = UFUserEditorManagerComponent.GetBitmapImage("UFUSREditorSmall");

            treeListView.Nodes.Clear();
            itemRoot = treeListControl.AddNode(new TreeItemControl(Document.IsSharedConnectionRepository ? Properties.Resources.UserAndRolesSharedRepository : Properties.Resources.UserAndRoles) { ResourceIcon = editorImg }, Tag as TreeListNode, Document);
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
            if (!UnSubscribeSelectionChangedEvent && !isPopup && Document.EditorManagerComponent.Workspace != null)
            {
                EndEdit();
                UnSubscribeSelectionChangedEvent = true;
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

        private void gridDataControl_SelectionChanged(object sender, GridSelectionChangedEventArgs e) {
            e.Handled = true;
            if (!UnSubscribeSelectionChangedEvent && !isPopup && Document.EditorManagerComponent.Workspace != null)
            {
                EndEdit();

                try
                {
                    UnSubscribeSelectionChangedEvent = true;

                    treeListControl.ClearSelection();
                    if (gridDataControl.SelectedItems.Count == 0)
                    {
                        Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(Document.GetGeneralUserSettings());
                    }
                    else if (gridDataControl.SelectedItems.Count == 1)
                    {
                        Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(gridDataControl.SelectedItem);
                    }
                    else
                    {
                        Document.EditorManagerComponent.Workspace.ContextObjects = Document.GetNestedObjects(gridDataControl.SelectedItems);
                    }
                }
                finally
                {
                    UnSubscribeSelectionChangedEvent = false;
                }
            }
        }

        void OnTreeNodeCollapsing(object sender, TreeListNodeAllowEventArgs e)
        {
            //if (!(e.Node.Content as TreeItemControl).IsNodeExpanding)
            //    UpdateFolderIcon(e.Node, false);
        }
        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (CustomFontHelper.CanApplyCustomFont())
            {
                gridDataControl.FontFamily = CustomFontHelper.GetCustomFontFamily();
                gridDataControl.FontSize = CustomFontHelper.GetCustomFontSize();
            }
        }

        internal void OnActivate()
        {
            UpdateContextObjects();
        }

        void UpdateContextObjects()
        {
            if (Document.EditorManagerComponent.Workspace != null)
            {
                var selecteditems = treeListControl.GetTreeSelectedItems<XPObject>();
                if (selecteditems.Count == 0)
                    Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(Document.GetGeneralUserSettings());
                else if (selecteditems.Count == 1)
                    Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(selecteditems[0]);
                else
                    Document.EditorManagerComponent.Workspace.ContextObjects = Document.GetNestedObjects(selecteditems);
            }
        }

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
                    bResult = Document.UowContext.TryCommitChanges(UFUserDocument.log);
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
            //var objects = Document.UowContext.GetObjectsToSave().OfType<DevExpress.Xpo.IXPSimpleObject>().ToList();
            if (isPopup || Document.EditorManagerComponent.Workspace == null)
                return;

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
                    {
                        list.Add(parent as DevExpress.Xpo.IXPSimpleObject);
                        if (obj is UFUserModel.UFUser)
                        {
                            var user = obj as UFUserModel.UFUser;
                            if (user.Password != (parent as UFUserModel.UFUser).Password)
                            {
                                while (user.PasswordHistory.Count > 0)
                                    user.PasswordHistory[0].Delete();
                                user.LastDateTimeChanged = DateTime.UtcNow;
                            }
                        }
                    }
                }

                if (list.Count > 0)
                    Document.AddUndoAction(this, list, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed);
            }
        }

        #endregion IEditableObject Members

        #region Methods
        private void FillItems(TreeListNode itemRoot)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                ClearNodes(itemRoot);
                using (new WaitCursor())
                {
                    var listFolders = Document.GetRoles();
                    if (listFolders == null || listFolders.Count == 0)
                    {
                        var aa1 = Document.AddNewRole();
                        aa1.Name = Properties.Settings.Default.AdminRoleName;
                        aa1.AccessLevel = Properties.Settings.Default.AdminRoleLevel;
                        var aa2 = Document.AddNewRole();
                        aa2.Name = Properties.Settings.Default.PowerUserRoleName;
                        aa2.AccessLevel = Properties.Settings.Default.PowerUserRoleLevel;
                        var aa3 = Document.AddNewRole();
                        aa3.Name = Properties.Settings.Default.GuestRoleName;
                        aa3.AccessLevel = Properties.Settings.Default.GuestRoleLevel;
                        listFolders = Document.GetRoles();
                    }

                    foreach (var folder in listFolders)
                        AddTreeItem(folder, itemRoot);
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }
        }

        private void FillItems(TreeListNode itemRoot, UFUserModel.UFRole role)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                ClearNodes(itemRoot);
                using (new WaitCursor())
                {
                    foreach (var user in role.UFUsers)
                        AddTreeItem(user, itemRoot);
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

        void ClearNodes(TreeListNode node)
        {
            if (node == null)
                return;

            //var mappedChildNodes = (from TreeItemControl data in contentToNodeMap.Keys where node.Nodes.Contains(contentToNodeMap[data]) select data).ToList();
            //foreach (var k in mappedChildNodes)
            //    contentToNodeMap.Remove(k);
            node.Nodes.Clear();
        }

        bool NeedToBeExpanded(TreeListNode item)
        {
            if (item.Tag is UFUserModel.UFRole)
            {
                if(Properties.Settings.Default.FolderAlwaysExpandible)
                    return true;

                var role = item.Tag as UFUserModel.UFRole;                
                
                return role.UFUsers.Count > 0;
            }
            return false;
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
                if (item.Tag is UFUserModel.UFRole)
                    FillItems(item, item.Tag as UFUserModel.UFRole);

                treeListControl.EndDataUpdate();
                e.Handled = true;
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

            if (parent == null)
                parent = itemRoot;

            if (parent == null)
                return null;
            if (!parent.IsExpanded)
            {
                parent.IsExpanded = true;
                var list = (from p in parent.Nodes where p.Tag == tag select p).ToList();
                if (list.Count > 0)
                    return list[0];
            }

            var ic = new TreeItemControl(tag);
            var newitem = treeListControl.AddNode(ic, parent, tag);
            //contentToNodeMap.Add(ic, newitem);

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

            if (tag is UFUserModel.UFRole)
            {
                var folder = tag as UFUserModel.UFRole;
                if (isPopup && folder.AccessLevel > maxRuntimeEditAccessLevel)
                    return null;

                SetBindingOnProp(newitem, folder, nameof(folder.Name));
                (newitem.Content as TreeItemControl).ResourceIcon = roleImg;
            }
            else if (tag is UFUserModel.UFUser)
            {
                var ufuatag = tag as UFUserModel.UFUser;
                if (isPopup && ufuatag.GlobalAccessLevel > maxRuntimeEditAccessLevel)
                    return null;

                SetBindingOnProp(newitem, ufuatag, nameof(ufuatag.Name));
                (newitem.Content as TreeItemControl).ResourceIcon = userImg;

            }

            if (IsUndoRedoSupported(tag))
                mapObjectToParent[tag] = parent;

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
            BindingOperations.SetBinding(node.Content as TreeItemControl, targetDP, myBinding);
        }

        internal UFUserModel.UFRole GetSelectedRoleParent()
        {
            if (treeListControl == null)
                return null;

            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUserModel.UFRole)
                return selected.Tag as UFUserModel.UFRole;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFUserModel.UFRole)
                    return selected.Tag as UFUserModel.UFRole;
            }

            return null;
        }

        internal TreeListNode GetSelectedRoleParentItem()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUserModel.UFRole)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFUserModel.UFRole)
                    return selected;
            }

            return null;
        }

        public TreeListNode GetSelectedParent(bool bSkipSelected = false)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (!bSkipSelected && selected != null && selected.Tag is UFUserModel.UFRole)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFUserModel.UFRole)
                    return selected;
            }

            return itemRoot;
        }

        internal void FlatGridRefresh()
        {
            try
            {
                UnSubscribeSelectionChangedEvent = true;

                //var list = new List<GridDataGroupColumn>();
                //foreach (var group in gridDataControl.GroupedColumns)
                //    list.Add(new GridDataGroupColumn() { ColumnName = group.ColumnName });

                //while (gridDataControl.GroupedColumns.Count > 0)
                //    gridDataControl.GroupedColumns.Remove(gridDataControl.GroupedColumns[0]);

                gridDataControl.ItemsSource = null;
                if (isPopup)
                    gridDataControl.ItemsSource = (from c in Document.GetFlatUserCollection() where c.GlobalAccessLevel <= maxRuntimeEditAccessLevel select c);
                else
                    gridDataControl.ItemsSource = Document.GetFlatUserCollection();

                var selNode = treeListControl.GetSelectedNodes().FirstOrDefault();
                if (selNode != null)
                    gridDataControl.SelectedItem = selNode.Tag;

                //foreach (var group in list)
                //{
                //    gridDataControl.GroupedColumns.Add(new GridDataGroupColumn() { ColumnName = group.ColumnName });
                //}
            }
            finally
            {
                UnSubscribeSelectionChangedEvent = false;
            }
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

        internal bool IsAnyItemSelected()
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            return item != null && item != itemRoot;
        }
        
        internal void DeleteSelectedItems()
        {
            var items = (from item in treeListControl.GetSelectedNodes()
                where (item as TreeListNode).Tag != null
                select item as TreeListNode).ToList();

            var listundo = (from item in items 
                            where IsUndoRedoSupported(item.Tag) && !items.Contains(item.ParentNode) 
                            select item.Tag as IXPSimpleObject).ToList();
            var uiMsgBox = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
            if (uiMsgBox != null)
            {
                var bShowMessage = (from item in items/*.AsParallel()*/
                                    where item.Tag is UFUserModel.UFRole &&
                                    listundo.Contains(item.Tag as UFUserModel.UFRole) &&
                                    ((item.Tag as UFUserModel.UFRole).UFUsers.Count > 0)
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
            FlatGridRefresh();
        }
        internal void DeleteTreeItem(TreeListNode item)
        {
            //var item = addressSpaceTree.SelectedItem as TreeListNode;
            var parent = item.ParentNode ?? itemRoot;
            if (item.Tag is UFUserModel.UFUser)
            {
                var user = item.Tag as UFUserModel.UFUser;
                //GetSelectedParentFolder(true).Items.Remove(item);
                parent.Nodes.Remove(item);
                user.Delete();

                if (IsUndoRedoSupported(user))
                    mapObjectToOldParent[user] = item;
            }
            else if (item.Tag is UFUserModel.UFRole)
            {
                var role = item.Tag as UFUserModel.UFRole;
                //GetSelectedParentFolder(true).Items.Remove(item);
                parent.Nodes.Remove(item);
                role.Delete();

                if (IsUndoRedoSupported(role))
                    mapObjectToOldParent[role] = item;
            }
        }
        internal TreeListNode GetTreeItem(Object tag, TreeListNode parent = null)
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
        internal void CopySelectedToClipboard()
        {
            var listroles = new List<UFUserModel.UFRole>();
            var listusers = new List<UFUserModel.UFUser>();
            foreach (var item in treeListControl.GetSelectedNodes())
            {
                if ((item as TreeListNode).Tag is UFUserModel.UFRole)
                    listroles.Add((item as TreeListNode).Tag as UFUserModel.UFRole);
                else if ((item as TreeListNode).Tag is UFUserModel.UFUser)
                    listusers.Add((item as TreeListNode).Tag as UFUserModel.UFUser);
            }

            Document.CleanClipbaord();
            Document.CopyListRolesToClipbaord(listroles);
            Document.CopyListUsersToClipbaord(listusers);
        }
        public TreeListNode GetSelectedParentRole(bool bSkipSelected = false)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (!bSkipSelected && selected != null && selected.Tag is UFUserModel.UFRole)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFUserModel.UFRole)
                    return selected;
            }

            return itemRoot;
        }

        internal void PasteFromClipboard()
        {
            var parent = GetSelectedParentRole();
            var expanded = parent.IsExpanded || parent.Nodes.Count > 0 && parent.Nodes[0].Tag != TreeListControlHelper.DummyNode;

            var tagsToSelect = new List<TreeListNode>();
            var foldersToSelect = new List<TreeListNode>();
            try
            {
                treeListControl.ClearSelection();
                treeListControl.BeginDataUpdate();

                var listfolders = Document.PasteClipboardRoles();
                listfolders.ForEach(dir =>
                {
                    var item = AddTreeItem(dir, itemRoot);
                    if (item != null)
                        foldersToSelect.Add(item);
                });

                var listtags = new List<UFUserModel.UFUser>();
                if (parent.Tag is UFUserModel.UFRole)
                {
                    listtags.AddRange(Document.PasteClipboardUsers(parent.Tag as UFUserModel.UFRole));
                    listtags.ForEach(tag =>
                    {
                        if (expanded)
                        {
                            var item = AddTreeItem(tag, parent);
                            if (item != null)
                                tagsToSelect.Add(item);
                        }
                        else
                        {
                            var item = GetTreeItem(tag, parent);
                            if (item != null)
                                tagsToSelect.Add(item);
                        }
                    });
                }

                if (listfolders.Count > 0 || listtags.Count > 0)
                {
                    FlatGridRefresh();

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
                if (foldersToSelect.Count > 0 || tagsToSelect.Count > 0)
                    treeListControl.SelectNodes(foldersToSelect.Concat(tagsToSelect).ToList(), true, true);
            }
        }
        #endregion

        #region Properties

        UFUserDocument _Document;
        [Browsable(false)]
        public UFUserDocument Document
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

        IUIMsgBoxAlertService uiInterface;
        public IUIMsgBoxAlertService UIInterface
        {
            get
            {
                if (Document != null && uiInterface == null)
                    uiInterface = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                return uiInterface;
            }
        }

        #endregion

        #region Commands

        private void OnCommandSave(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Document.SaveToFile();
        }

        private void CanCommandSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document.NeedsSave;
        }

        private void OnAddNewRole(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var aa = Document.AddNewRole();
            if (isPopup)
                aa.MaxAccessLevel = maxRuntimeEditAccessLevel;
            if (!isPopup && Document.EditorManagerComponent.PropertyControl != null)
            {
                Document.AddUndoAction(this, aa, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
                var item = AddTreeItem(aa, itemRoot);
                treeListControl.ClearSelection();
                treeListControl.SelectNode(item);
                Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
            {
                using (var uow = Document.BeginNestedUnitOfWork())
                {
                    var contextRole = uow.GetNestedObject(aa);
                    if (isPopup)
                        contextRole.MaxAccessLevel = maxRuntimeEditAccessLevel;
                    var newrole = new NewRole(Document)
                    {
                        DataContext = contextRole
                    };
                    GeneralDialogContent Dialog = new GeneralDialogContent(newrole)
                    {
                        Owner = this.FindParent<Window>(),
                        HelpLink = "RoleEditor",
                        Title = Properties.Resources.RoleProperties
                    };
                    if (Dialog.ShowDialog() == true)
                    {
                        uow.CommitChanges();
                        Document.AddUndoAction(this, aa, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
                        var item = AddTreeItem(aa, itemRoot);
                        treeListControl.ClearSelection();
                        treeListControl.SelectNode(item);
                    }
                    else
                    {
                        aa.Delete();
                    }
                }
            }
        }

        private void CanAddNewRole(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnAddNewUser(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var aa = Document.AddNewUser(GetSelectedRoleParent());
            if (!isPopup && Document.EditorManagerComponent.PropertyControl != null)
            {
                Document.AddUndoAction(this, aa, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
                var item = AddTreeItem(aa, GetSelectedRoleParentItem());
                treeListControl.ClearSelection();
                treeListControl.SelectNode(item);
                Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
            {
                using (var uow = Document.BeginNestedUnitOfWork())
                {
                    var contextUser = uow.GetNestedObject(aa);
                    if (isPopup)
                        contextUser.MaxAccessLevel = maxRuntimeEditAccessLevel;
                    var newuser = new NewUser(Document)
                    {
                        DataContext = contextUser
                    };
                    GeneralDialogContent Dialog = new GeneralDialogContent(newuser)
                    {
                        Owner = this.FindParent<Window>(),
                        HelpLink = "UserEditor",
                        Title = Properties.Resources.UserProperties
                    };
                    if (Dialog.ShowDialog() == true)
                    {
                        uow.CommitChanges();
                        Document.AddUndoAction(this, aa, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
                        var item = AddTreeItem(aa, GetSelectedRoleParentItem());
                        treeListControl.ClearSelection();
                        treeListControl.SelectNode(item);
                    }
                    else
                    {
                        aa.Delete();
                    }
                }
            }

            FlatGridRefresh();
        }

        private void CanAddNewUser(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GetSelectedRoleParent() != null;
        }

        private void OnRemoveItem(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                DeleteSelectedItems();
                //var item = treeListControl.SelectedItem as TreeListNode;
                //if (item.Tag is UFUserModel.UFUser)
                //{
                //    var tag = item.Tag as UFUserModel.UFUser;
                //    GetSelectedParent(true).Items.Remove(item);
                //    tag.Delete();
                //}
                //else if (item.Tag is UFUserModel.UFRole)
                //{
                //    var folder = item.Tag as UFUserModel.UFRole;
                //    if (folder.UFUsers.Count > 0)
                //    {
                //        var ret = MessageBox.Show(Properties.Resources.RoleNotEmpty, folder.Name, MessageBoxButton.YesNo);
                //        if (ret == MessageBoxResult.No)
                //            return;
                //    }
                //    GetSelectedParent(true).Items.Remove(item);
                //    folder.Delete();
                //}

                //FlatGridRefresh();
            }
        }

        private void CanRemoveItem(object sender, CanExecuteRoutedEventArgs e)
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            e.CanExecute = item != null && item != itemRoot;
        }

        private void treeListControl_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (!isPopup && Document.EditorManagerComponent.PropertyControl != null)
                Document.EditorManagerComponent.PropertyControl.Activate();
            else
                EditSelectedItem();
        }

        void EditSelectedItem()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null)
            {
                if (selected.Tag is UFUserModel.UFRole)
                {
                    var role = selected.Tag as UFUserModel.UFRole;
                    EditRole(role);
                }
                else if (selected.Tag is UFUserModel.UFUser)
                {
                    var user = selected.Tag as UFUserModel.UFUser;
                    EditUser(user);
                }
            }
        }

        void EditUser(UFUserModel.UFUser user)
        {
            using (var uow = Document.BeginNestedUnitOfWork())
            {
                var contextUser = uow.GetNestedObject(user);
                if (isPopup)
                    contextUser.MaxAccessLevel = maxRuntimeEditAccessLevel;
                var newUser = new NewUser(Document)
                {
                    DataContext = contextUser
                };
                var Dialog = new GeneralDialogContent(newUser)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "UserEditor",
                    Title = Properties.Resources.UserProperties
                };
                if (Dialog.ShowDialog() == true)
                {
                    if (contextUser.Password != user.Password)
                    {
                        while (contextUser.PasswordHistory.Count > 0)
                            contextUser.PasswordHistory[0].Delete();
                        contextUser.LastDateTimeChanged = DateTime.UtcNow;
                    }

                    Document.AddUndoAction(this, user, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed);
                    uow.CommitChanges();
                    UpdateContextObjects();
                    FlatGridRefresh();
                }
            }
        }

        void EditRole(UFUserModel.UFRole role)
        {
            using (var uow = Document.BeginNestedUnitOfWork())
            {
                var contextRole = uow.GetNestedObject(role);
                if (isPopup)
                    contextRole.MaxAccessLevel = maxRuntimeEditAccessLevel;
                var newRoleControl = new NewRole(Document)
                {
                    DataContext = contextRole
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newRoleControl)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "RoleEditor",
                    Title = Properties.Resources.RoleProperties
                };
                if (Dialog.ShowDialog() == true)
                {
                    Document.AddUndoAction(this, role, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed);
                    uow.CommitChanges();
                    UpdateContextObjects();
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
                var ufuatag = gridDataControl.SelectedItem as UFUserModel.UFUser;
                EditUser(ufuatag);
            }
        }

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
                Document.CopyWinClipboardToInMemoryData();
                PasteFromClipboard();
            }
        }

        private void CanCommandPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            Document.CopyWinClipboardToInMemoryData();
            e.CanExecute = Document.ClipboardContainsUsers() || Document.ClipboardContainsRoles();
        }

        private void OnCommandUndo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                UndoAction();
            }
        }

        private void CanCommandUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyUndoActionAvailable();
        }

        private void OnCommandRedo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                RedoAction();
            }
        }

        private void CanCommandRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyRedoActionAvailable();
        }

        private void OnCommandProperties(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (!isPopup && Document.EditorManagerComponent.PropertyControl != null)
                Document.EditorManagerComponent.PropertyControl.Activate();
            else
                EditSelectedItem();
        }

        private void CanCommandProperties(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected();
        }

        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            if (!isPopup && Document.EditorManagerComponent.Workspace != null)
                Document.EditorManagerComponent.Workspace.ContextObject = null;

            // gridDataControl.Model.Dispose();
            try
            {
                gridDataControl.Dispose();
            }
            catch (Exception ex)
            {
                
            }

            mapObjectToParent.Clear();
            mapObjectToOldParent.Clear();
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
            return obj is UFUserModel.UFRole || obj is UFUserModel.UFUser;
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

                            FlatGridRefresh();
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
                                            treeListControl.SelectNode(item);
                                    }
                                }
                            }
                            finally
                            {
                                treeListControl.EndDataUpdate();
                            }

                            FlatGridRefresh();
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

                            FlatGridRefresh();
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
                            
                            FlatGridRefresh();
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
                                            treeListControl.SelectNode(item);
                                    }
                                }
                            }
                            finally
                            {
                                treeListControl.EndDataUpdate();
                            }
                            
                            FlatGridRefresh();
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

                            FlatGridRefresh();
                        }
                    }
                    break;
            }
        }

        #endregion   
    }
}

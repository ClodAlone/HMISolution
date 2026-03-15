using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UFUserEditor.Document;
using System.ComponentModel;
using Utilities;
using Utilities.WPF;
using UFUserEditor.ComponentService;
using UFUserEditor.Controls;
using DevExpress.Xpo;
using TranslationHelpers;
using StringManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using WPFUtilities;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using System.Windows.Data;
using UFInterfaces.AuthenticationCredentialsProvider;

namespace UFUserEditor
{
    /// <summary>
    /// Interaction logic for UFUserEditorControl.xaml
    /// </summary>
    public partial class UFRuntimeUserEditorControl : UserControl, IDisposable
    {
        #region Declarations
        readonly BitmapImage userImg;
        readonly BitmapImage roleImg;
        readonly BitmapImage editorImg;
        readonly TreeListNode itemRoot;

        readonly Dictionary<Object, TreeListNode> mapObjectToParent = new Dictionary<Object, TreeListNode>();
        readonly Dictionary<Object, TreeListNode> mapObjectToOldParent = new Dictionary<Object, TreeListNode>();

        readonly int maxRuntimeEditAccessLevel;
        readonly uint enforcePasswordHistory;

        bool UnSubscribeSelectionChangedEvent;

        IDictionary<string, string> stringlist;

        string currentUserName;
        IAuthenticationCredentialsProvider authenticationCredentialsProvider;
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
        public UFRuntimeUserEditorControl(UFUserDocument doc)
        {
            InitializeComponent();
            Document = doc;
            ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document);
            Document.CreateUndoRedoHelper(this);

            if (Document.GetGeneralUserSettings(true).MaxRuntimeEditAccessLevel.HasValue)
                maxRuntimeEditAccessLevel = Document.GetGeneralUserSettings(true).MaxRuntimeEditAccessLevel.Value;
            enforcePasswordHistory = Document.GetGeneralUserSettings(true).EnforcePasswordHistory;

            authenticationCredentialsProvider = Document.GetService(typeof(IAuthenticationCredentialsProvider)) as IAuthenticationCredentialsProvider;
            if (authenticationCredentialsProvider != null)
                authenticationCredentialsProvider.UserOnline += AuthenticationProvider_UserOnline;

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;

                    if (Document.ActiveView as UFRuntimeUserEditorControl != null)
                    {
                        var stringManager = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                        if (stringManager != null)
                            stringlist = stringManager.GetListStringForCulture(Document, stringManager.GetActiveCulture(Document));
                    }

                    if (authenticationCredentialsProvider != null)
                        authenticationCredentialsProvider.RefreshCurrentUser(Document.Parent.Title);

                    UIGeneralCommands.AddNewRole.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_AddNewRoleCommandText", stringlist, Properties.UICommandResource.AddNewRoleText);
                    UIGeneralCommands.AddNewRole.Tooltip = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_AddNewRoleCommandTooltip", stringlist, Properties.UICommandResource.AddNewRoleTooltip);
                    UIGeneralCommands.AddNewRole.Description = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_AddNewRoleCommandDescription", stringlist, Properties.UICommandResource.AddNewRoleDescription);
                    UIGeneralCommands.AddNewUser.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_AddNewUserCommandText", stringlist, Properties.UICommandResource.AddNewUserText);
                    UIGeneralCommands.AddNewUser.Tooltip = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_AddNewUserCommandTooltip", stringlist, Properties.UICommandResource.AddNewUserTooltip);
                    UIGeneralCommands.AddNewUser.Description = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_AddNewUserCommandDescription", stringlist, Properties.UICommandResource.AddNewUserDescription);

                    editMenu.Content = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_EditMenuHeader", stringlist, Properties.UICommandResource.EditMenuText);
                }
            };
            Unloaded += (o, e) =>
            {
                bLoaded = false;
            };

            gridDataControl.ItemsSource = (from c in Document.GetFlatUserCollection() where c.GlobalAccessLevel <= maxRuntimeEditAccessLevel select c);

            userImg = UFUserEditorManagerComponent.GetBitmapImage("UFUSRUserSmall");
            roleImg = UFUserEditorManagerComponent.GetBitmapImage("UFUSRGroupSmall");
            editorImg = UFUserEditorManagerComponent.GetBitmapImage("UFUSREditorSmall");

            treeListView.Nodes.Clear();
            itemRoot = treeListControl.AddNode(
                new TreeItemControl(Document.IsSharedConnectionRepository ?
                    TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_UserAndRolesSharedRepo", stringlist, Properties.Resources.UserAndRolesSharedRepository) :
                    TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_UserAndRoles", stringlist, Properties.Resources.UserAndRoles)) { ResourceIcon = editorImg }, 
                    Tag as TreeListNode, Document);
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
        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            if (CustomFontHelper.CanApplyCustomFont())
                gridDataControl.FontFamily = CustomFontHelper.GetCustomFontFamily();

            treeListControl.FontSize = this.FontSize;
            gridDataControl.FontSize = this.FontSize;
        }

        private void AuthenticationProvider_UserOnline(object sender, LoginInfoEventArgs e)
        {
            currentUserName = e.User;
        }

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
                        aa1.Name = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_AdminRoleName", stringlist, Properties.Settings.Default.AdminRoleName);
                        aa1.AccessLevel = Properties.Settings.Default.AdminRoleLevel;
                        Document.AddAuditMessage(String.Format(Properties.Resources.AuditRoleAdded, aa1.Name), null, aa1, currentUserName);
                        var aa2 = Document.AddNewRole();
                        aa2.Name = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_PowerUserRoleName", stringlist, Properties.Settings.Default.PowerUserRoleName);
                        aa2.AccessLevel = Properties.Settings.Default.PowerUserRoleLevel;
                        Document.AddAuditMessage(String.Format(Properties.Resources.AuditRoleAdded, aa2.Name), null, aa2, currentUserName);
                        var aa3 = Document.AddNewRole();
                        aa3.Name = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_GuestRoleName", stringlist, Properties.Settings.Default.GuestRoleName);
                        aa3.AccessLevel = Properties.Settings.Default.GuestRoleLevel;
                        Document.AddAuditMessage(String.Format(Properties.Resources.AuditRoleAdded, aa3.Name), null, aa3, currentUserName);
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

            node.Nodes.Clear();
        }

        bool NeedToBeExpanded(TreeListNode item)
        {
            if (item.Tag is UFUserModel.UFRole)
            {
                if (Properties.Settings.Default.FolderAlwaysExpandible)
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

            if (tag is UFUserModel.UFRole)
            {
                var folder = tag as UFUserModel.UFRole;
                if (folder.AccessLevel > maxRuntimeEditAccessLevel)
                    return null;
            }
            else if (tag is UFUserModel.UFUser)
            {
                var ufuatag = tag as UFUserModel.UFUser;
                if (ufuatag.GlobalAccessLevel > maxRuntimeEditAccessLevel)
                    return null;
            }

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
                SetBindingOnProp(newitem, folder, nameof(folder.Name));
                (newitem.Content as TreeItemControl).ResourceIcon = roleImg;
            }
            else if (tag is UFUserModel.UFUser)
            {
                var ufuatag = tag as UFUserModel.UFUser;
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
                gridDataControl.ItemsSource = null;
                gridDataControl.ItemsSource = (from c in Document.GetFlatUserCollection() where c.GlobalAccessLevel <= maxRuntimeEditAccessLevel select c);

                var selNode = treeListControl.GetSelectedNodes().FirstOrDefault();
                if (selNode != null)
                    gridDataControl.SelectedItem = selNode.Tag;
            }
            finally
            {
                UnSubscribeSelectionChangedEvent = false;
            }
        }

        internal bool IsAnyItemSelected()
        {
            var item = treeListControl?.GetSelectedNodes().FirstOrDefault();
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
                                    (item.Tag as UFUserModel.UFRole).UFUsers.Count > 0
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
                Document.AddAuditMessage(String.Format(Properties.Resources.AuditUserRemoved, user.Name), user, null, currentUserName);
                //GetSelectedParentFolder(true).Items.Remove(item);
                parent.Nodes.Remove(item);
                user.Delete();
                
                if (IsUndoRedoSupported(user))
                    mapObjectToOldParent[user] = item;
            }
            else if (item.Tag is UFUserModel.UFRole)
            {
                var role = item.Tag as UFUserModel.UFRole;
                Document.AddAuditMessage(String.Format(Properties.Resources.AuditRoleRemoved, role.Name), role, null, currentUserName);
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
                    Document.AddAuditMessage(String.Format(Properties.Resources.AuditRoleAdded, dir.Name), null, dir, currentUserName);
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
                    Document.AddAuditMessage(String.Format(Properties.Resources.AuditUserAdded, tag.Name), null, tag, currentUserName);
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
            }
            finally
            {
                treeListControl.EndDataUpdate();
                if (foldersToSelect.Count > 0 || tagsToSelect.Count > 0)
                    treeListControl.SelectNodes(foldersToSelect.Concat(tagsToSelect).ToList());
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
            e.CanExecute = Document != null && Document.NeedsSave;
        }

        private void OnAddNewRole(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (var uow = Document.BeginNestedUnitOfWork())
            {
                var aa = Document.AddNewRole();
                var contextRole = uow.GetNestedObject(aa);
                contextRole.MaxAccessLevel = maxRuntimeEditAccessLevel;
                var newrole = new NewRole(Document, true)
                {
                    DataContext = contextRole
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newrole)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "RoleEditor",
                    Title = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_RoleProperties", stringlist, Properties.Resources.RoleProperties)
                };
                if (Dialog.ShowDialog() == true)
                {
                    uow.CommitChanges();
                    Document.AddAuditMessage(String.Format(Properties.Resources.AuditRoleAdded, aa.Name), null, aa, currentUserName);
                    Document.AddUndoAction(this, aa, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
                    var item = AddTreeItem(aa, itemRoot);
                    if (item != null)
                    {
                        treeListControl.ClearSelection();
                        treeListControl.SelectNode(item);
                    }
                }
                else
                {
                    aa.Delete();
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
            using (var uow = Document.BeginNestedUnitOfWork())
            {
                var aa = Document.AddNewUser(GetSelectedRoleParent());
                var contextUser = uow.GetNestedObject(aa);
                contextUser.MaxAccessLevel = maxRuntimeEditAccessLevel;
                var newuser = new NewUser(Document, true)
                {
                    DataContext = contextUser
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newuser)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "UserEditor",
                    Title = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_UserProperties", stringlist, Properties.Resources.UserProperties)
                };
                if (Dialog.ShowDialog() == true)
                {
                    uow.CommitChanges();
                    Document.AddAuditMessage(String.Format(Properties.Resources.AuditUserAdded, aa.Name), null, aa, currentUserName);
                    Document.AddUndoAction(this, aa, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
                    var item = AddTreeItem(aa, GetSelectedRoleParentItem());
                    if (item != null)
                    {
                        treeListControl.ClearSelection();
                        treeListControl.SelectNode(item);
                    }
                }
                else
                {
                    aa.Delete();
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
                //var item = userTree.SelectedItem as TreeListNode;
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
            var item = treeListControl?.GetSelectedNodes().FirstOrDefault();
            e.CanExecute = item != null && item != itemRoot;
        }

        bool bDobuleClick;
        private void treeListControl_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            bDobuleClick = true;
        }

        private void treeListControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                treeListControl.Focusable = true;
                treeListControl.Focus();
            });

            e.Handled = false;
            if (!bDobuleClick)
                return;
            bDobuleClick = false;

            e.Handled = true;
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
            if (user == null)
                return;
            using (var uow = Document.BeginNestedUnitOfWork())
            {
                var contextUser = uow.GetNestedObject(user);
                contextUser.MaxAccessLevel = maxRuntimeEditAccessLevel;
                contextUser.ValidatePasswordHistory = enforcePasswordHistory > 0;
                var newUser = new NewUser(Document, true)
                {
                    DataContext = contextUser
                };
                var Dialog = new GeneralDialogContent(newUser)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "UserEditor",
                    Title = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_UserProperties", stringlist, Properties.Resources.UserProperties)
                };
                if (Dialog.ShowDialog() == true)
                {
                    if (contextUser.Password != user.Password)
                    {
                        var settings = Document.GetGeneralUserSettings(uow);
                        settings.AddUserPasswordInHistory(contextUser, user.Password, contextUser.Password);
                        contextUser.LastDateTimeChanged = DateTime.UtcNow;
                    }
                    Document.AddAuditMessage(String.Format(Properties.Resources.AuditUserChanged, user.Name), user, newUser.DataContext as UFUserModel.UFUser, currentUserName);
                    Document.AddUndoAction(this, user, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed);
                    uow.CommitChanges();

                    FlatGridRefresh();
                }
            }
        }

        void EditRole(UFUserModel.UFRole role)
        {
            using (var uow = Document.BeginNestedUnitOfWork())
            {
                var contextRole = uow.GetNestedObject(role);
                contextRole.MaxAccessLevel = maxRuntimeEditAccessLevel;
                var newRoleControl = new NewRole(Document, true)
                {
                    DataContext = contextRole
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newRoleControl)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "RoleEditor",
                    Title = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_RoleProperties", stringlist, Properties.Resources.RoleProperties)
                };
                if (Dialog.ShowDialog() == true)
                {
                    Document.AddAuditMessage(String.Format(Properties.Resources.AuditRoleChanged, role.Name), role, newRoleControl.DataContext as UFUserModel.UFRole, currentUserName);
                    Document.AddUndoAction(this, role, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed);
                    uow.CommitChanges();
                }
            }
        }

        private void gridDataControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var ufuatag = gridDataControl.SelectedItem as UFUserModel.UFUser;
            EditUser(ufuatag);
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
            if (Document == null)
                e.CanExecute = false;
            else
            {
                Document.CopyWinClipboardToInMemoryData();
                e.CanExecute = Document.ClipboardContainsUsers() || Document.ClipboardContainsRoles();
            }
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

            if (authenticationCredentialsProvider != null)
                authenticationCredentialsProvider.UserOnline -= AuthenticationProvider_UserOnline;

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
            return Document != null && Document.UndoContainsSomething(this);
        }

        internal bool IsAnyRedoActionAvailable()
        {
            return Document != null && Document.RedoContainsSomething(this);
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

                            FlatGridRefresh();
                        }
                    }
                    break;
            }
        }

        #endregion   
    }
}

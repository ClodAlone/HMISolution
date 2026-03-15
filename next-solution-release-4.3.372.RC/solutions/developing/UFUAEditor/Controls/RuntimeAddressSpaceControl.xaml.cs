using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UFInterfaces.AuthenticationCredentialsProvider;
using UFInterfaces.Editors;
using UFUAEditor.ComponentService;
using UFUAEditor.Converters;
using UFUAEditor.Document;
using UFUAEditor.Helpers;
using UFUserEditor.ComponentService;
using Utilities;
using Utilities.WPF;
using WPFUtilities;
using SelectionMode = UFInterfaces.Editors.SelectionMode;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for RuntimeAddressSpaceControl.xaml
    /// </summary>
    public partial class RuntimeAddressSpaceControl : UserControl, IAddressSpaceControl, IDisposable
    {
        readonly static String rootName = "Root";
        readonly UFUAServerDocument Document;
        TreeListNode itemRoot;
        BitmapImage openFolderImg;
        BitmapImage closedFolderImg;
        bool bLoaded;

        String currentView;
        int currentUserAccessLevel;
        int currentUserReadAccessMask;
        bool enableusermanager;

        List<UFUAServerDocument> listChilds;
        bool multiSelectionAllowed;

        public RuntimeAddressSpaceControl(UFUAServerDocument doc, List<UFUAServerDocument> lc = null, bool multiSelectionAllowed = false)
        {
            InitializeComponent();
            Document = doc;
            listChilds = lc;
            var converter = TryFindResource("TranslateConverter") as TranslateConverter;
            if (converter != null)
                converter.SetConverterDocument(Document);
            this.multiSelectionAllowed = multiSelectionAllowed;
            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;

                    UpdateListViews(Document);

                    InitializeAddressSpace();

                    var authenticationProvider = Document.GetService(typeof(IAuthenticationCredentialsProvider)) as IAuthenticationCredentialsProvider;
                    if (authenticationProvider != null)
                    {
                        authenticationProvider.UserOnline += AuthenticationProvider_UserOnline;
                        authenticationProvider.RefreshCurrentUser(Document.Parent.Title);
                        authenticationProvider.UserOnline -= AuthenticationProvider_UserOnline;
                    }
                }
            };

            treeListView.CustomNodeFilter += (s, e) =>
            {
                if (FilterType == FilterType.None)
                    return;

                if (e.Node.Tag is UFUAModel.UFUATag)
                {
                    var ufuatag = e.Node.Tag as UFUAModel.UFUATag;
                    if (FilterType == FilterType.Historians)
                        e.Visible = !String.IsNullOrEmpty(ufuatag.HistorianSettings) || ufuatag.ModelType == UFUAModel.ModelType.ObjectType;
                }
                else if (!(e.Node.Tag is UFUAModel.UFUAFolder))
                    e.Visible = e.Node == itemRoot || e.Node.Tag == TreeListControlHelper.DummyNode;

                if (!e.Visible)
                    e.Handled = true;
            };

            //Unloaded += (o, e) =>
            //{
            //    if (bLoaded)
            //        bLoaded = false;
            //};
        }

        void treeListControl_SelectionChanged(object sender, TreeListSelectionChangedEventArgs e)
        {
            e.Handled = true;
            var selectedTags = treeListControl.GetSelectedNodes();

            if (multiSelectionAllowed)
            {
                List<OPCUAViewModel.OPCUAEntityReference> list = new List<OPCUAViewModel.OPCUAEntityReference>();
                selectedTags.ToList().ForEach(selected =>
                {
                    var tag = GetTag(selected);
                    if (tag != null)
                        list.Add(GetTag(selected));
                });

                DataContext = list;
            }
            else
            {
                var selected = selectedTags.FirstOrDefault();
                DataContext = GetTag(selected);
            }
        }

        OPCUAViewModel.OPCUAEntityReference GetTag(TreeListNode node)
        {
            if (node == null)
                return null;
            OPCUAViewModel.OPCUAEntityReference oPCUAEntityReference = null;
            UFUAServerDocument docFound = Document;
            var docItem = GetParentDocument(node);
            if (docItem != null && docItem.Tag is UFUAServerDocument)
                docFound = docItem.Tag as UFUAServerDocument;

            UFUAModel.UFUATag ufuatag = null;
            var list = new List<UFUAModel.UFUATag>();
            if (node != null && node.Tag is UFUAModel.UFUATag)
            {
                ufuatag = node.Tag as UFUAModel.UFUATag;
                var parent = GetParentTag(node);
                while (parent != null)
                {
                    list.Add(parent.Tag as UFUAModel.UFUATag);
                    parent = GetParentTag(parent);
                }
            }

            if (ufuatag != null)
            {
                var entity = docFound.GetTagOPCUAEntityReference(ufuatag, list);
                oPCUAEntityReference = entity;
            }

            UpdateListViews(docFound);
            return oPCUAEntityReference;
        }

        void UpdateListViews(UFUAServerDocument doc)
        {
            var listViews = (from c in doc.GetViewsList() select c.Name).ToList();
            if (listViews.Count > 0)
                listViews.Add(Properties.Resources.NoView);
            else
                gridViews.Visibility = Visibility.Collapsed;
            cmbViews.ItemsSource = listViews;
        }

        private void AuthenticationProvider_UserOnline(object sender, LoginInfoEventArgs e)
        {
            currentUserAccessLevel = 0;
            currentUserReadAccessMask = 0;
            enableusermanager = true;
            var userEditor = Document.GetService(typeof(IUFUserEditorManager)) as IUFUserEditorManager;
            if (userEditor != null)
            {
                enableusermanager = userEditor.GetEnableUserManager(Document);

                if (!String.IsNullOrEmpty(e.User))
                {
                    currentUserAccessLevel = userEditor.GetUserAccessLevel(Document, e.User);
                    currentUserReadAccessMask = userEditor.GetUserAccessMask(Document, e.User);
                }
            }
        }

        class rootHeader
        {
            public String Name { get; set; }
        }
        
        void InitializeAddressSpace()
        {
            openFolderImg = UFUAEditorManagerComponent.GetBitmapImage("OpenFolderSmall", true);
            closedFolderImg = UFUAEditorManagerComponent.GetBitmapImage("CloseFolderSmall", true);

            treeListView.Nodes.Clear();
            itemRoot = treeListControl.AddNode(new AddressSpaceTreeItemControl(Document.Parent.Title) { ResourceIcon = closedFolderImg }, null, Document);
            treeListControl.AddNode(null, itemRoot, TreeListControlHelper.DummyNode);

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                if (bDisposed)
                    return;

                FillItems(itemRoot, Document);
                if (itemRoot.Nodes.Count == 0)
                    treeListControl.AddNode(null, itemRoot, TreeListControlHelper.DummyNode);
               
                itemRoot.IsExpanded = true;
            });
        }

        void ClearNodes(TreeListNode node)
        {
            node.Nodes.Clear();
        }

        private static bool CanBeExpanded(TreeListNode parent)
        {
            return parent.Nodes.Count == 1 && parent.Nodes[0].Tag == TreeListControlHelper.DummyNode;
        }

        void UpdateFolderIcon(TreeListNode node, bool isOpen)
        {
            if (node != null && (node == itemRoot || node.Tag is UFUAModel.UFUAFolder))
                (node.Content as AddressSpaceTreeItemControl).ResourceIcon = isOpen ? openFolderImg : closedFolderImg;
        }

        void OnTreeNodeCollapsing(object sender, TreeListNodeAllowEventArgs e)
        {
            if (!(e.Node.Content as AddressSpaceTreeItemControl).IsNodeExpanding)
                UpdateFolderIcon(e.Node, false);
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
                
                (item.Content as AddressSpaceTreeItemControl).IsNodeExpanding = true;
                UFUAServerDocument docFound = Document;
                var docItem = GetParentDocument(item);
                if (docItem != null && docItem.Tag is UFUAServerDocument)
                    docFound = docItem.Tag as UFUAServerDocument;

                treeListControl.BeginDataUpdate();

                ClearNodes(item);

                if (item.Tag is UFUAModel.UFUAFolder)
                    FillItems(item, docFound, item.Tag as UFUAModel.UFUAFolder);
                else if (item.Tag is UFUAModel.UFUATag)
                    FillItems(item, docFound, item.Tag as UFUAModel.UFUATag);
                else
                    FillItems(item, docFound);
                e.Handled = true;

                treeListControl.EndDataUpdate();
            }
            finally
            {
                (item.Content as AddressSpaceTreeItemControl).IsNodeExpanding = false;
            }
        }

        // static int maxItems = Properties.Settings.Default.MaxItemsInTree;
        private void FillItems(TreeListNode itemRoot, UFUAServerDocument doc, UFUAModel.UFUAFolder root = null)
        {
            treeListControl.BeginDataUpdate();

            // bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            try
            {
                using (new AsyncWaitCursor())
                {
                    ClearNodes(itemRoot);
                    var listTags = doc.GetTagCollection(root);
                    if (listTags != null)
                    {
                        foreach (var tag in listTags)
                        {
                            AddTreeItem(tag, itemRoot);
                            //if (!bShiftDown && ++i > maxItems)
                            //{
                            //    //AddTreeItem(Properties.Resources.MaxItemCountVisibleReachedDoubleClick, itemRoot);
                            //    break;
                            //}
                        }
                    }

                    var listFolders = doc.GetFolderCollection(root);
                    if (listFolders != null)
                    {
                        foreach (var folder in listFolders)
                        {
                            AddTreeItem(folder, itemRoot);
                            //if (!bShiftDown && ++i > maxItems)
                            //{
                            //    //AddTreeItem(Properties.Resources.MaxItemCountVisibleReachedDoubleClick, itemRoot);
                            //    break;
                            //}
                        }
                    }
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }
        }

        private void FillItems(TreeListNode itemRoot, UFUAServerDocument doc, UFUAModel.UFUATag tag)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                using (new AsyncWaitCursor())
                {
                    ClearNodes(itemRoot);

                    if (tag.ModelType == UFUAModel.ModelType.ObjectType &&
                        !String.IsNullOrEmpty(tag.PrototypeName))
                    {
                        var prototypeFound = doc.CreateSubPrototype(tag);
                        if (prototypeFound != null)
                        {
                            prototypeFound.EnsureUniqueMembersOrderId();

                            var sortedMembers = (from c in prototypeFound.Members orderby c.MemberOrderId ascending select c).ToList();
                            foreach (var member in sortedMembers)
                                AddTreeItem(member, itemRoot);
                            var sortedFolders = (from c in prototypeFound.Folders orderby c.MemberOrderId ascending select c).ToList();
                            foreach (var folder in sortedFolders)
                                AddTreeItem(folder, itemRoot);
                        }
                    }
                }
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

        bool CanShow(UFUAModel.UFUAFolder folder)
        {
            if (!enableusermanager && String.IsNullOrEmpty(currentView))
                return true;

            var listTags = folder.GetTagMembers();
            if (enableusermanager)
            {
                var exist = (from ufuatag in listTags
                             where (currentUserAccessLevel >= ufuatag.UserAccessLevel &&
                             (ufuatag.UserReadAccessMask == 0 ||
                             (currentUserReadAccessMask & ufuatag.UserReadAccessMask) != 0))
                             select ufuatag).FirstOrDefault();

                if (exist == null)
                    return false;
            }

            if (String.IsNullOrEmpty(currentView))
                return true;

            if (folder.IsSubPrototypeMember)
            {
                var instanceTag = folder.PrototypeReference.UFUATagOwner;
                while (instanceTag.IsSubPrototypeMember)
                    instanceTag = instanceTag.PrototypeReference.UFUATagOwner;

                var foundView = (from c in instanceTag.UFUAViews where c.Name == currentView select c).FirstOrDefault();
                if (foundView != null)
                    return true;
            }

            foreach (var ufuatag in listTags)
            {
                if (CanShow(ufuatag))
                    return true;
            }

            return false;
        }

        bool CanShow(UFUAModel.UFUATag tag)
        {
            if (!enableusermanager && String.IsNullOrEmpty(currentView))
                return true;

            if (enableusermanager && (currentUserAccessLevel < tag.UserAccessLevel || 
                tag.UserReadAccessMask != 0 && (currentUserReadAccessMask & tag.UserReadAccessMask) == 0))
                return false;

            if (String.IsNullOrEmpty(currentView))
                return true;

            {
                var foundView = (from c in tag.UFUAViews where c.Name == currentView select c).FirstOrDefault();
                if (foundView != null)
                    return true;
            }

            if (tag.IsSubPrototypeMember)
            {
                var instanceTag = tag.PrototypeReference.UFUATagOwner;
                while (instanceTag.IsSubPrototypeMember)
                    instanceTag = instanceTag.PrototypeReference.UFUATagOwner;

                var foundView = (from c in instanceTag.UFUAViews where c.Name == currentView select c).FirstOrDefault();
                if (foundView != null)
                    return true;
            }

            if (tag.SubPrototypeMembers != null && tag.SubPrototypeMembers.Count > 0)
            {
                var members = tag.SubPrototypeMembers[0].GetTagMembers();
                foreach (var member in members)
                {
                    if (CanShow(member))
                        return true;
                }
            }

            return false;
        }

        bool NeedToBeExpanded(TreeListNode item)
        {
            if (item.Tag is UFUAModel.UFUAFolder)
                return true;
            if (item.Tag is UFUAModel.UFUATag)
            {
                var ufuatag = item.Tag as UFUAModel.UFUATag;
                if (ufuatag.ModelType == UFUAModel.ModelType.ObjectType)
                    return true;
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

            if (!parent.IsExpanded && !(parent.Content as AddressSpaceTreeItemControl).IsNodeExpanding)
            {
                parent.IsExpanded = true;
                var list = (from p in parent.Nodes where p.Tag == tag select p).ToList();
                if (list.Count > 0)
                    return list[0];
            }

            var ic = new AddressSpaceTreeItemControl(tag);
            var newitem = treeListControl.AddNode(ic, parent, tag);

            if (NeedToBeExpanded(newitem))
                treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);

            bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (tag is UFUAModel.UFUAFolder)
            {
                bKnownType = true;
                var folder = tag as UFUAModel.UFUAFolder;
                if (!CanShow(folder))
                {
                    parent.Nodes.Remove(newitem);
                    return null;
                }

                var itemContent = newitem.Content as AddressSpaceTreeItemControl;
                itemContent.ItemHeader = folder.Name;
                itemContent.ResourceIcon = closedFolderImg;
            }
            else if (tag is UFUAServerDocument)
            {
                bKnownType = true;
                var doc = tag as UFUAServerDocument;

                var itemContent = newitem.Content as AddressSpaceTreeItemControl;
                itemContent.ItemHeader = doc.Parent.Title;
                itemContent.ResourceIcon = closedFolderImg;
            }
            else if (tag is String)
            {
                bKnownType = true;
                var itemContent = newitem.Content as AddressSpaceTreeItemControl;
                itemContent.ItemHeader = tag as String;
                itemContent.ResourceIcon = closedFolderImg;
            }
            else if (tag is UFUAModel.UFUATag)
            {
                var ufuatag = tag as UFUAModel.UFUATag;

                var itemContent = newitem.Content as AddressSpaceTreeItemControl;
                itemContent.ItemHeader = ufuatag.Name;
                
                if (!CanShow(ufuatag))
                {
                    parent.Nodes.Remove(newitem);
                    return null;
                }

                bKnownType = true;

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
                itemContent.ResourceIcon = bmpImage;
            }

            treeListControl.RefreshRow(newitem.RowHandle); //Otherwise node's header (ItemHeader) can disappear in certain conditions
            return bKnownType ? newitem : null;
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

        internal bool IsAnyItemSelected()
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            return item != null && item != itemRoot;
        }

        public TreeListNode GetParentTag(TreeListNode parent)
        {
            while (parent != null)
            {
                parent = parent.ParentNode as TreeListNode;
                if (parent != null && parent.Tag is UFUAModel.UFUATag)
                    return parent;
            }

            return null;
        }

        public TreeListNode GetParentDocument(TreeListNode parent)
        {
            if (parent != null && parent.Tag is UFUAServerDocument)
                return parent;
            while (parent != null)
            {
                parent = parent.ParentNode as TreeListNode;
                if (parent != null && parent.Tag is UFUAServerDocument)
                    return parent;
            }

            return null;
        }

        private void cmbViews_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TreeListNode item = itemRoot;
            UFUAServerDocument docFound = Document;
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null)
            {
                var docItem = GetParentDocument(selected);
                if (docItem != null && docItem.Tag is UFUAServerDocument)
                {
                    docFound = docItem.Tag as UFUAServerDocument;
                    item = docItem;
                }
            }
            currentView = cmbViews.SelectedItem as String;
            if (currentView == Properties.Resources.NoView)
                currentView = String.Empty;
            item.Nodes.Clear();
            treeListControl.AddNode(null, item, TreeListControlHelper.DummyNode);
            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                FillItems(item, docFound);
                item.IsExpanded = true;
            });
        }

        private void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            e.Handled = true;
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUAModel.UFUATag)
            {
                var wnd = this.FindParent<Window>();
                if (wnd != null)
                {
                    wnd.DialogResult = true;
                    wnd.Close();
                }
            }
        }

        #region IAddressSpaceControl
        SelectionType selectionType = SelectionType.OPCUAEntityReference;
        public SelectionType SelectionType
        {
            get
            {
                return selectionType;
            }

            set
            {
                if (selectionType == value)
                    return;
                selectionType = value;
            }
        }

        FilterType filterType = FilterType.None;
        public FilterType FilterType
        {
            get
            {
                return filterType;
            }

            set
            {
                filterType = value;
                treeListControl.RefreshData();
            }
        }

        SelectionMode selectionMode = SelectionMode.SingleRow;
        public SelectionMode SelectionMode
        {
            get
            {
                return selectionMode;
            }

            set
            {
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
        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            if (Document != null)
                Document.Dispose();

            if (listChilds != null)
            {
                foreach (var doc in listChilds)
                    doc.Dispose();
            }
        }
        #endregion
    }
    }

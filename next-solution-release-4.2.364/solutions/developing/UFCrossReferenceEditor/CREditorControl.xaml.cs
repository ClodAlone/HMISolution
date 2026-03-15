using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using UFCrossReferenceEditor.Document;
using HelpProvider.ComponentService;
using Utilities;
using DocumentManager.ComponentService;
using System.IO;
using DevExpress.Xpo;
using UFCrossReferenceModel;
using UFCrossReferenceEditor.ComponentService;
using ScreenManager.ComponentService;
using System.Threading.Tasks;
using System.Threading;
using Utilities.WPF;
using OPCUAViewModel;
using WPFUtilities;
using UIMsgBoxAlertService.ComponentService;
using StringManager.ComponentService;
using TranslationHelpers;
using System.Windows.Media;
using UFProjectManager.ComponentService;
using log4net;
using UFInterfaces;
using DocumentManager.ComponentService.Helpers;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using System.Windows.Data;
using DevExpress.Data;
using UFInterfaces.Editors;
using UFUAEditor.ComponentService;

namespace UFCrossReferenceEditor
{

    /// <summary>
    /// Interaction logic for DocumentEditorControl.xaml
    /// </summary>
    public partial class CREditorControl : UserControl, IDisposable
    {
        #region TextMessage
        public static readonly DependencyProperty TextMessageProperty = DependencyProperty.Register("TextMessage", typeof(string), typeof(CREditorControl), new UIPropertyMetadata(Properties.Resources.DocumentEditorRibbonTitle));
        public string TextMessage
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(TextMessageProperty);
            }
            set
            {
                SetValue(TextMessageProperty, value);
            }
        }
        #endregion

        #region Declaration
        ImageSource openFolderImg;
        ImageSource closedFolderImg;
        ImageSource addressSpaceImg;
        ImageSource screenImg;
        ImageSource stringImg;
        ImageSource connectionImg;
        Dictionary<string,ImageSource> imageMap;
        TreeListNode itemRoot;
        TreeListNode itemScreenRoot;
        TreeListNode itemConnectionRoot;
        TreeListNode itemStringRoot;
        readonly static String rootTagName = "Tags";
        readonly static String rootScreenName = "Screens";
        readonly static String rootConnectionName = "Connections";
        readonly static String rootStringName = "Texts";
        readonly Dictionary<Object, TreeListNode> mapStringIDsObjectToParent = new Dictionary<Object, TreeListNode>();
        List<string> nodeToPrototypesList = new List<string>();
        #region CrossReferenceTree

        class rootHeader
        {
            public String Name { get; set; }
        }

        #endregion   
        #endregion
        #region ctor
        bool bLoaded;
        public bool IsLoaded
        {
            get
            {
                return bLoaded;
            }
        }
        bool bDesignmode;
        IStringEditorManager stringManager;
        IUIMsgBoxAlertService uIInterface;
        IUFProjectManager iUFProjectManager;
        ILog log;
        IWorkspace workspace;
        IDocument parent; 
        IStringEditorManager stringEditor;
        IUFUAEditorManager ufuaEditor;

        public CREditorControl(CREditorDocument doc, IDocument parent, bool bdesign = true)
        {
            InitializeComponent();
            Document = doc;
            this.parent = parent;
            uIInterface = Document.EditorManagerComponent.UIInterface;
            iUFProjectManager = Document.EditorManagerComponent.UFProjectManager;
            log = CrossReferenceEditorManagerComponent.log;
            workspace = Document.EditorManagerComponent.Workspace;

            //gridDataControl.SourceType = typeof(UFCrossReferenceEntity);
            //gridScreenDataControl.SourceType = typeof(UFCrossReferenceEntity);
            //gridDataControl.Model.SuspendRecordUndo();
            //gridScreenDataControl.Model.SuspendRecordUndo();tabTags

            bDesignmode = bdesign;

            ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document, !bdesign);
            closedFolderImg = TryFindResource("CloseFolderSmall") as ImageSource;
            openFolderImg = TryFindResource("OpenFolderSmall") as ImageSource;
            addressSpaceImg = TryFindResource("UFCRAddressSpace") as ImageSource;
            screenImg = TryFindResource("UFCRScreenManager") as ImageSource;
            connectionImg = TryFindResource("UFCRConnectionManager") as ImageSource;
            stringImg = TryFindResource("UFCRStringManager") as ImageSource;
            tagTabImage.Source = addressSpaceImg;
            screenTabImage.Source = screenImg;
            connectionTabImage.Source = connectionImg; 
            stringIDsTabImage.Source = stringImg;


            if(!bdesign)
            {
                contextMenu.IsEnabled = false;
            }
            else
            {
                updateCR.Glyph = TryFindResource("UFCREditor") as ImageSource;
                clearCRDoc.Glyph = TryFindResource("UFCRClear") as ImageSource;
                renameReferences.Glyph = TryFindResource("RenameSmall") as ImageSource;
                addStringId.Glyph = TryFindResource("UFCRAddStringID") as ImageSource;
                removeStringId.Glyph = TryFindResource("UFCRRemoveStringID") as ImageSource;
                removeTags.Glyph = TryFindResource("DeleteSmall") as ImageSource;
            }


            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;

                    if (Document != null)
                    {
                        if (stringManager == null)
                            stringManager = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                        if(ufuaEditor == null)
                            ufuaEditor = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                        if (stringManager != null)
                        {
                            StringManager_CultureChanged(null, null);
                            stringManager.CultureChanged += StringManager_CultureChanged;
                        }
                    }
                }
            };
            Unloaded += (o, e) =>
            {
                bLoaded = false;
            };

            using (new WaitCursor())
            {
                Document.InitDocSummary();
                ClearTreeView();
                ClearGrid(); 

                InitializeCRTree();
                InitializeScreenCRTree();
                InitializeConnectionCRTree();
                InitializeStringCRTree();
                FlatGridRefresh();

            }

            //tabControlExt.SelectionChanged += (o, e) =>
            //{
            //    OnSelectionTabChanged(e.OldSelectedItem, e.NewSelectedItem);
            //};

        }

        private void ConnectionFlatGridRefresh()
        {
            //var slist = new List<GridDataGroupColumn>();
            //foreach (var group in gridConnectionDataControl.GroupedColumns)
            //    slist.Add(new GridDataGroupColumn() { ColumnName = group.ColumnName });

            //while (gridConnectionDataControl.GroupedColumns.Count > 0)
            //    gridConnectionDataControl.GroupedColumns.Remove(gridConnectionDataControl.GroupedColumns[0]);

            DesignerProperties.SetIsInDesignMode(gridConnectionDataControl, false);
            gridConnectionDataControl.ItemsSource = null;
            var _slist = new List<UFCrossReferenceEntity>();
            _slist.AddRange(Document.GetConnectionFlatList());
            gridConnectionDataControl.ItemsSource = _slist;

            var selectedNode = treeListControlConnection.GetSelectedNodes().FirstOrDefault();
            if (selectedNode != null)
                gridConnectionDataControl.SelectedItem = selectedNode.Tag;

            //foreach (var group in slist)
            //{
            //    gridConnectionDataControl.GroupedColumns.Add(new GridDataGroupColumn() { ColumnName = group.ColumnName });
            //}
        }

        #endregion

        #region CrossReferenceList
        private void InitializeCRList()
        {
            //gridDataControl.SourceType = typeof(UFCrossReferenceEntity);
            //using (new WaitCursor())
            //{
            gridDataControl.ItemsSource = Document.GetTagCollection(); /*Document.GetFlatList()*/;
            //}
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

        internal void FlatGridRefresh()
        {
            TagFlatGridRefresh();
            ScreenFlatGridRefresh();
            ConnectionFlatGridRefresh();
            StringFlatGridRefresh();
        }

        internal void StringFlatGridRefresh()
        {
            DesignerProperties.SetIsInDesignMode(gridStringIDsDataControl, false);
            gridStringIDsDataControl.ItemsSource = null;
            var _slist = new List<UFCrossReferenceEntity>();
            _slist.AddRange(Document.GetStringFlatList());
            gridStringIDsDataControl.ItemsSource = _slist;
        }

        internal void ScreenFlatGridRefresh()
        {
            //var slist = new List<GridDataGroupColumn>();
            //foreach (var group in gridScreenDataControl.GroupedColumns)
            //    slist.Add(new GridDataGroupColumn() { ColumnName = group.ColumnName });

            //while (gridScreenDataControl.GroupedColumns.Count > 0)
            //    gridScreenDataControl.GroupedColumns.Remove(gridScreenDataControl.GroupedColumns[0]);

            DesignerProperties.SetIsInDesignMode(gridScreenDataControl, false);
            gridScreenDataControl.ItemsSource = null;
            var _slist = new List<UFCrossReferenceEntity>();
            _slist.AddRange(Document.GetScreenFlatList());
            gridScreenDataControl.ItemsSource = _slist;

            var selectedNode = treeListControlScreen.GetSelectedNodes().FirstOrDefault();
            if (selectedNode != null)
                gridScreenDataControl.SelectedItem = selectedNode.Tag;

            //foreach (var group in slist)
            //{
            //    gridScreenDataControl.GroupedColumns.Add(new GridDataGroupColumn() { ColumnName = group.ColumnName });
            //}
        }


        internal void TagFlatGridRefresh()
        {
            //var list = new List<GridDataGroupColumn>();
            //foreach (var group in gridDataControl.GroupedColumns)
            //    list.Add(new GridDataGroupColumn() { ColumnName = group.ColumnName });

            //while (gridDataControl.GroupedColumns.Count > 0)
            //    gridDataControl.GroupedColumns.Remove(gridDataControl.GroupedColumns[0]);

            DesignerProperties.SetIsInDesignMode(gridDataControl, false);
            gridDataControl.ItemsSource = null;
            gridDataControl.ItemsSource = Document.GetTagCollection(); /*Document.GetFlatList()*/; 

            var selectedNode = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selectedNode != null)
                gridDataControl.SelectedItem = selectedNode.Tag;

            //foreach (var group in list)
            //{
            //    gridDataControl.GroupedColumns.Add(new GridDataGroupColumn() { ColumnName = group.ColumnName });
            //}
        }

        private void EditEntity(UFCrossReferenceEntity entity)
        {
            if (entity == null || iUFProjectManager == null)
                return;
            var p = DocumentHelper.GetRootParent(Document, traverse: false);
            var doc = iUFProjectManager.GetResourceDocumentManager(p, entity.Container);
            if(doc!=null && doc is ICrossReference)
                try
                {
                        if (DesignerProperties.GetIsInDesignMode(this) || bDesignmode)
                        {
                            (doc as ICrossReference).EditCRObject(p, entity.DynamicSettings);
                        }
                        else
                        {
                            if (doc is IScreenManager)
                                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                                {
                                    OpenScreen(doc, p, entity.DynamicSettings);
                                });
                        }
                }
                catch (Exception)
                {
                }
        }

        private void CanAddStringId(object sender, CanExecuteRoutedEventArgs e)
        {
            if (stringEditor == null)
                stringEditor = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
            e.CanExecute = tabStringIDs.IsSelected && stringEditor != null && Document.GetInvalidReferences(CrossReferenceType.Strings) > 0 ;
        }

        private void OnAddStringId(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var list = Document.GetTranslatable();
            if (stringEditor == null)
                stringEditor = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
            try
            {
                treeListControlStringIDs.BeginDataUpdate();
                Document.UpdateTranslatable();
            }
            finally
            {
                treeListControlStringIDs.EndDataUpdate();
            }
            StringFlatGridRefresh();
            stringEditor.AddListStringId(Document, list);
        }

        private void CanRemoveStringId(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                int count = tabStringIDsSplitterTree.IsSelected ? (from n in treeListControlStringIDs.GetSelectedNodes()
                                                                   where n.Tag is UFCrossReferenceString && (n.Tag as UFCrossReferenceString).IsNotInUse
                                                                   select n).ToList().Count() :
                                                                   (from n in (gridStringIDsDataControl.SelectedItems as DevExpress.Xpf.Core.ObservableCollectionCore<object>)
                                                                   where n is UFCrossReferenceModel.UFCrossReferenceEntity && (n as UFCrossReferenceModel.UFCrossReferenceEntity).IsNotRefUsed
                                                                   select n).ToList().Count();

                if (stringEditor == null)
                    stringEditor = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;

                e.CanExecute = tabStringIDs.IsSelected && stringEditor != null && count > 0;
            }
            catch (Exception)
            {
                e.CanExecute = false;
            }
        }

        private void CanRemoveTag(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                var treeSelected = treeListControl.GetSelectedNodes();
                var gridSelected = gridDataControl.SelectedItems as DevExpress.Xpf.Core.ObservableCollectionCore<object>;
                int count = tabSplitterTree.IsSelected ? (from n in treeSelected
                                                          where n.Tag is UFCrossReferenceTag && IsRemovable(n.Tag as UFCrossReferenceTag) ||
                                                          n.Tag is UFCrossReferenceTagFolder && IsRemovable(n.Tag as UFCrossReferenceTagFolder)
                                                          select n).ToList().Count() :
                                                          (from n in gridSelected
                                                           where n is UFCrossReferenceTag && IsRemovable(n as UFCrossReferenceTag) ||
                                                           n is UFCrossReferenceTagFolder && IsRemovable(n as UFCrossReferenceTagFolder)
                                                           select n).ToList().Count();
                if (ufuaEditor == null)
                    ufuaEditor = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;

                e.CanExecute = tabTags.IsSelected && ufuaEditor != null && count > 0;
            }
            catch (Exception ex)
            {
                e.CanExecute = false;
            }
        }
        const string temporaryVariables = "TemporaryVariables";
        bool IsRemovable(UFCrossReferenceEntity entity)
        {
            return entity.IsNotRefUsed && (IsRemovable(entity.CrossReferenceTagFolder) || IsRemovable(entity.CrossReferenceTag));
        }

        bool IsRemovable(UFCrossReferenceTag tag)
        {
            return tag != null && tag.IsNotInUse && !nodeToPrototypesList.Contains(tag.ReadablePathNoProject) && tag.Entities.Count > 0 
                && (tag.Entities[0].ReferencedNodeID != null || tag.AppName == temporaryVariables);
        }

        bool IsRemovable(UFCrossReferenceTagFolder folder)
        {
            return folder != null && folder.IsNotInUse && folder.HasPrototypeModel && !nodeToPrototypesList.Contains(folder.GetRelativeName()) && folder.Entities.Count > 0 && folder.Entities[0].ReferencedNodeID != null; 
        }

        private void OnRemoveTag(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (Document.EditorManagerComponent.UIMsgBoxAlertService?.ShowOkCancel(Properties.Resources.RemovingTagWarning, UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question) == UIMsgBoxAlertService.ComponentService.CustomDialogResults.Cancel)
                return;

            var p = DocumentHelper.GetRootParent(Document, traverse: false);
            (ufuaEditor as IDocumentManager).CloseAllChild(p, bParentClosing:true);

            List<TreeListNode> items = new List<TreeListNode>();
            List<TreeListNode> datasyncitems = new List<TreeListNode>();
            List<string> list = new List<string>();
            List<string> datasyncList = new List<string>();
            List<object> itemTags = new List<object>();
            List<object> datasyncitemTags = new List<object>();
            List<object> tagToRemove = new List<object>();
            if (tabSplitterTree.IsSelected)
            {
                items = (from n in treeListControl.GetSelectedNodes()
                         where ((n.Tag is UFCrossReferenceTag) && IsRemovable(n.Tag as UFCrossReferenceTag) 
                         && (n.Tag as UFCrossReferenceTag).AppName != temporaryVariables) || 
                         ((n.Tag is UFCrossReferenceTagFolder) && IsRemovable(n.Tag as UFCrossReferenceTagFolder))
                         select n).ToList();
                datasyncitems = (from n in treeListControl.GetSelectedNodes()
                                 where (n.Tag is UFCrossReferenceTag) && IsRemovable(n.Tag as UFCrossReferenceTag) 
                                 && (n.Tag as UFCrossReferenceTag).AppName == temporaryVariables
                                 select n).ToList();
                datasyncitemTags = (from item in datasyncitems select item.Tag).ToList();

                datasyncList = (from item in datasyncitemTags
                                where item is UFCrossReferenceTag && (item as UFCrossReferenceTag).AppName == temporaryVariables
                                select (item as UFCrossReferenceTag).ReadablePathNoProject).AsParallel().ToList();

                itemTags = (from item in items select item.Tag).ToList();

                list = (from item in itemTags
                        where  item is UFCrossReferenceTag && (item as UFCrossReferenceTag).AppName != temporaryVariables
                        select (item as UFCrossReferenceTag).Entities[0].ReferencedNodeID).AsParallel().ToList();
                list.AddRange((from item in itemTags
                               where item is UFCrossReferenceTagFolder
                               select (item as UFCrossReferenceTagFolder).Entities[0].ReferencedNodeID).AsParallel().ToList());
            }
            else
            {
                foreach (XPObject o in gridDataControl.SelectedItems)
                {
                    if ((o is UFCrossReferenceTag && IsRemovable(o as UFCrossReferenceTag) && (o as UFCrossReferenceTag).AppName == temporaryVariables))
                    {
                        if (!datasyncitemTags.Contains(o))
                            datasyncitemTags.Add(o);
                    }
                    else if ((o is UFCrossReferenceTag && IsRemovable(o as UFCrossReferenceTag) && (o as UFCrossReferenceTag).AppName != temporaryVariables) 
                        || (o is UFCrossReferenceTagFolder && IsRemovable(o as UFCrossReferenceTagFolder)))
                    {
                        if(!itemTags.Contains(o))
                            itemTags.Add(o);
                    }
                };
                if (itemTags.Count > 0)
                {
                    items = GetItemNodeList(itemTags);
                    list = (from item in itemTags
                            where item is UFCrossReferenceTag
                            select (item as UFCrossReferenceTag).Entities[0].ReferencedNodeID).AsParallel().ToList();

                    list.AddRange((from item in itemTags
                                   where item is UFCrossReferenceTagFolder
                                   select (item as UFCrossReferenceTagFolder).Entities[0].ReferencedNodeID).AsParallel().ToList());
                }
                if (datasyncitemTags.Count > 0)
                {
                    datasyncitems = GetItemNodeList(datasyncitemTags);
                    datasyncList = (from item in datasyncitemTags
                                    select (item as UFCrossReferenceTag).ReadablePathNoProject.Replace("\\","&")).AsParallel().ToList();
                }
            }

            tagToRemove.AddRange(itemTags);
            tagToRemove.AddRange(datasyncitemTags);


            if (list.Count > 0 || datasyncList.Count > 0)
            {
                try
                {
                    gridDataControl.ItemsSource = null;
                    treeListControl.BeginDataUpdate();
                    if(list.Count > 0 && ufuaEditor.RemoveListTags(Document, list))
                    {
                        items.ForEach(item => DeleteTreeItem(item));
                    }
                    if (datasyncList.Count > 0)
                    {
                        var dsInterface = OPCUAViewModel.OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables");
                        if (dsInterface == null)
                            return;
                        dsInterface.SetDocumentParent(Document);
                        foreach (var item in datasyncList)
                            dsInterface.RemoveVariable(item);
                        dsInterface.Save(Document);
                        datasyncitems.ForEach(item => DeleteTreeItem(item));
                    }

                    tagToRemove.ForEach(item => DeleteItem(item));
                }
                finally
                {
                    treeListControl.EndDataUpdate();
                }
                TagFlatGridRefresh();
            }
        }

        List<TreeListNode> GetItemNodeList(List<object> tagList, TreeListNode root = null)
        {
            List<TreeListNode> ret = new List<TreeListNode>();
            if (root == null)
                root = itemRoot;
            root.Nodes.ToList().ForEach(node =>
            {
                if (tagList.Contains(node.Tag))
                    ret.Add(node);
                ret.AddRange(GetItemNodeList(tagList, node));
            });
            return ret;
        }

        private void OnRemoveStringId(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            if (Document.EditorManagerComponent.UIMsgBoxAlertService?.ShowOkCancel(Properties.Resources.RemovingStringWarning, UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question) == UIMsgBoxAlertService.ComponentService.CustomDialogResults.Cancel)
                return;

            var p = DocumentHelper.GetRootParent(Document, traverse: false);
            (stringEditor as IDocumentManager).CloseAllChild(p);

            List<TreeListNode> items = new List<TreeListNode>();
            List<object> itemTags = new List<object>();
            List<string> list = new List<string>();
            if (tabStringIDsSplitterTree.IsSelected)
            {
                items = (from n in treeListControlStringIDs.GetSelectedNodes()
                             where n.Tag is UFCrossReferenceString && (n.Tag as UFCrossReferenceString).IsNotInUse
                             select n).ToList();
                itemTags = (from item in items
                                select item.Tag).AsParallel().ToList();
                list = (from item in itemTags
                            select (item as UFCrossReferenceString).Name).AsParallel().ToList();
            }
            else
            {
                foreach (UFCrossReferenceEntity o in gridStringIDsDataControl.SelectedItems)
                {
                    if (o.IsNotRefUsed && !itemTags.Contains(o.CrossReferenceString))
                        itemTags.Add(o.CrossReferenceString);
                };
                if(itemTags.Count > 0)
                {
                    items = (from node in itemStringRoot.Nodes where itemTags.Contains(node.Tag as UFCrossReferenceString) select node).ToList();
                    list = (from item in itemTags
                                select (item as UFCrossReferenceString).Name).AsParallel().ToList();
                }
            }

            if(list.Count > 0 && stringEditor.RemoveListStringId(Document, list))
            {
                try
                {
                    treeListControlStringIDs.BeginDataUpdate();
                    foreach (var item in items)
                        DeleteTreeItem(item);
                }
                finally
                {
                    treeListControlStringIDs.EndDataUpdate();
                }
                StringFlatGridRefresh();
            }
        }

        void DeleteItem(object item)
        {
            if (item is UFCrossReferenceTag)
                (item as UFCrossReferenceTag).Delete();
            else if (item is UFCrossReferenceTagFolder)
                (item as UFCrossReferenceTagFolder).Delete();
        }

        void DeleteTreeItem(TreeListNode item)
        {
            //var item = addressSpaceTree.SelectedItem as TreeViewItemAdv;
            if (item.Tag is UFCrossReferenceString)
            {
                var tag = item.Tag as UFCrossReferenceString;
                itemStringRoot.Nodes.Remove(item);
                Document.RemoveString(tag);
            }
            else if (item.Tag is UFCrossReferenceTag || item.Tag is UFCrossReferenceTagFolder)
            {
                var parent = item.ParentNode ?? itemRoot;
                parent.Nodes.Remove(item);
            }
        }


        void OpenScreen(IDocumentManager doc, IDocument parent, string settings)
        {
            string[] path = settings.Split('|');
            if (path.Length >= 2)
            {
                var uri = new Uri(path[1], UriKind.RelativeOrAbsolute);
                var context = new Dictionary<String, Object>();
                context.Add("SynchroPopup", false);
                context.Add("SynchroFrame", false);
                context.Add("Entity", null);

                if (uri == null)
                {
                    doc.Execute(null, parent, ExecutionMode.Normal, context);
                }
                else
                {
                    doc.Execute(uri, parent, ExecutionMode.Normal, context);
                }
            }
        }
        String GetDocumentTitle(Uri uri)
        {
            return Path.GetFileNameWithoutExtension(uri.GetPathString());
        }

        #endregion

        #region ScreenReferenceList
        private void InitializeScreenCRList()
        {
            //gridScreenDataControl.SourceType = typeof(UFCrossReferenceEntity);
            //using (new WaitCursor())
            //{
                gridScreenDataControl.ItemsSource = Document.GetScreenFlatList();
            //}
        }
        #endregion

        #region ScreenReferenceTree
        void InitializeScreenCRTree()
        {
            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                if (bDisposed)
                    return;

                FillScreenItems(itemScreenRoot);

                itemScreenRoot.IsExpanded = true;
            });

            //ScreenReferenceTree.SelectedItemChanged += (s, e) =>
            //{
            //    e.Handled = true;
            //    List<Object> selecteditems = new List<Object>();
            //    foreach (TreeListNode item in ScreenReferenceTree.SelectedItems)
            //    {
            //        if (item.Tag != null)
            //            selecteditems.Add(item.Tag);
            //    }
            //};
        }
        #endregion

        #region TreeViewManagement
        static int maxItems = Properties.Settings.Default.MaxItemsInTree;
        private void FillScreenItems(TreeListNode itemRoot, UFCrossReferenceScreenFolder root = null)
        {
            treeListControlScreen.BeginDataUpdate();
            try
            {
                ClearNodes(itemRoot);

                if (workspace != null)
                    workspace.IsBusy = true;

                bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

                using (new AsyncWaitCursor())
                {
                    ClearNodes(itemRoot);
                    int i = 0;
                    var listFolders = Document.GetScreenFolderCollection(root);

                    if (listFolders != null)
                    {
                        foreach (var folder in listFolders)
                        {
                            AddTreeItem(treeListControlScreen, folder, itemRoot);
                            if (!bShiftDown && ++i > maxItems)
                            {
                                break;
                            }
                        }
                    }
                    i = 0;

                    var listScreens = Document.GetScreenCRTags(root);
                    if (listScreens != null)
                    {
                        foreach (var itemScreen in listScreens)
                        {
                            AddTreeItem(treeListControlScreen, itemScreen, itemRoot);
                            if (!bShiftDown && ++i > maxItems)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                treeListControlScreen.EndDataUpdate();
                if (workspace != null)
                    workspace.IsBusy = false;
            }
        }

        private void FillItems(TreeListNode itemRoot, UFCrossReferenceTagFolder root = null)
        {
            treeListControl.BeginDataUpdate();
            try
            {
                ClearNodes(itemRoot);

                if (workspace != null)
                    workspace.IsBusy = true;

                bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

                using (new AsyncWaitCursor())
                {
                    ClearNodes(itemRoot);
                    int i = 0;
                    var listFolders = Document.GetTagFolderCollection(root);
                    if (listFolders != null)
                    {
                        foreach (var folder in listFolders)
                        {
                            AddTreeItem(treeListControl, folder, itemRoot);
                            if (!bShiftDown && ++i > maxItems)
                            {
                                break;
                            }
                        }
                    }
                    i = 0;
                    var listItems = Document.GetCRTags(root);
                    if (listItems != null)
                    {
                        foreach (var itemTag in listItems)
                        {
                            if(!itemTag.IsOtherEntitiesReferencedBy)
                            {
                                AddTreeItem(treeListControl, itemTag, itemRoot);
                                if (!bShiftDown && ++i > maxItems)
                                {
                                    break;
                                }
                            }
                        }
                    }

                   if(itemRoot != this.itemRoot && root.Entities.Count > 0)
                    (from entity in root.Entities
                     select entity).ToList().ForEach(x =>
                     {
                         AddTreeItem(treeListControl, x, itemRoot);
                     });
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
                if (workspace != null)
                    workspace.IsBusy = false;
            }
        }

        private void Fillitems(TreeListNode itemRoot, XPObject tag)
        {
            TreeListControl tree = null;
            List<XPObject> entities = new List<XPObject>();
            string typeIcon = null;
            if (tag is UFCrossReferenceTagFolder)
            {
                tree = treeListControl;
                var t = (UFCrossReferenceTagFolder)tag;
                typeIcon = DocManagerType.UFUAServer.ToString();
                entities.AddRange(t.Entities.ToList());
            }
            if (tag is UFCrossReferenceTag)
            {
                tree = treeListControl;
                var t = (UFCrossReferenceTag)tag;
                typeIcon = t.TypeIcon;
                entities.AddRange(t.Entities.ToList());
            }
            if (tag is UFCrossReferenceScreen)
            {
                tree = treeListControlScreen;
                var t = (UFCrossReferenceScreen)tag;
                typeIcon = t.TypeIcon;
                entities.AddRange(t.Entities.ToList());
            }
            if (tag is UFCrossReferenceConnection)
            {
                var t = (UFCrossReferenceConnection)tag;
                tree = treeListControlConnection;
                typeIcon = t.TypeIcon;
                entities.AddRange(t.Entities.ToList());
            }
            if (tag is UFCrossReferenceString)
            {
                var t = (UFCrossReferenceString)tag;
                tree = treeListControlStringIDs;
                typeIcon = t.TypeIcon;
                entities.AddRange(t.Entities.ToList());
            }

            if (tree == null || String.IsNullOrEmpty(typeIcon) || entities.Count == 0)
                return;

            tree.BeginDataUpdate();
            try
            {
                ClearNodes(itemRoot);

                if (workspace != null)
                    workspace.IsBusy = true;

                ClearNodes(itemRoot);
                //using (new WaitCursor())
                {
                    (from entity in entities
                     orderby typeIcon
                     select entity).ToList().ForEach(x =>
                     {
                         AddTreeItem(tree, x, itemRoot);
                     });
                }
            }
            finally
            {
                tree.EndDataUpdate();
                if (workspace != null)
                    workspace.IsBusy = false;
            }
        }

        //private static bool CanBeExpanded(TreeListNode parent)
        //{
        //    return parent.Nodes.Count == 1 && parent.Nodes[0].Tag == TreeListControlHelper.DummyNode;
        //}

        void UpdateFolderIcon(TreeListNode node, bool isOpen)
        {
            if (node != null)
            {
                if (node == itemRoot || node == itemConnectionRoot || node == itemScreenRoot || node == itemStringRoot ||
                    node.Tag is UFCrossReferenceScreenFolder ||
                    node.Tag is UFCrossReferenceStringFolder ||
                    node.Tag is UFCrossReferenceConnectionFolder)
                    (node.Content as TreeItemControl).ResourceIcon = isOpen ? openFolderImg : closedFolderImg;
                else if (node.Tag is UFCrossReferenceTagFolder)
                {
                    string typeIcon = (node.Tag as UFCrossReferenceTagFolder).TypeDefinition;
                    if (typeIcon != null)
                    {
                        if (imageMap == null)
                            imageMap = new Dictionary<string, ImageSource>();
                        if (!imageMap.ContainsKey(typeIcon))
                        {
                            var typeImg = TryFindResource($"UFCR{typeIcon}") as ImageSource;
                            imageMap.Add(typeIcon, typeImg);
                        }
                        (node.Content as TreeItemControl).ResourceIcon = imageMap[typeIcon];
                    }
                    else
                        (node.Content as TreeItemControl).ResourceIcon = isOpen ? openFolderImg : closedFolderImg;
                }
            }
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

                if (e != null && !TreeListControlHelper.CanBeExpanded(item))
                {
                    if (item.Tag is UFCrossReferenceTagFolder || item.Tag is UFCrossReferenceScreenFolder || 
                        item.Tag is UFCrossReferenceConnectionFolder || 
                        item == itemRoot || item == itemScreenRoot || item == itemConnectionRoot || item == itemStringRoot)
                        UpdateFolderIcon(item, true);
                    return;
                }

                (item.Content as TreeItemControl).IsNodeExpanding = true;
                if (item.Tag is UFCrossReferenceTagFolder)
                    FillItems(item, item.Tag as UFCrossReferenceTagFolder);
                else if (item.Tag is UFCrossReferenceScreenFolder)
                    FillScreenItems(item, item.Tag as UFCrossReferenceScreenFolder);
                else if (item.Tag is UFCrossReferenceConnectionFolder)
                    FillConnectionItems(item, item.Tag as UFCrossReferenceConnectionFolder);
                else if (item.Tag is UFCrossReferenceStringFolder)
                    FillStringItems(item, item.Tag as UFCrossReferenceStringFolder);
                else
                    Fillitems(item, item.Tag as XPObject);
                e.Handled = true;
            }
            finally
            {
                (item.Content as TreeItemControl).IsNodeExpanding = false;
            }
        }

        bool NeedToBeExpanded(TreeListNode item)
        {
            if (item != null && !item.IsExpanded && !(item.Tag is UFCrossReferenceEntity || item.Tag is String)) /*(item.Tag is UFCrossReferenceTagFolder || item.Tag is UFCrossReferenceScreenFolder || item.Tag is UFCrossReferenceConnectionFolder)*/
                return true;
            return false;
        }

        internal TreeListNode AddTreeItem(TreeListControl tree, Object tag, TreeListNode parent)
        {
            if (bDisposed)
                return null;

            bool bKnownType = false;
            bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (!parent.IsExpanded && !(parent.Content as TreeItemControl).IsNodeExpanding)
            {
                parent.IsExpanded = true;
                var list = (from p in parent.Nodes where p.Tag == tag select p).ToList();
                if (list.Count > 0)
                    return list[0];
            }

            //contentToNodeMap.Add(ic, newitem);

            //if (mapObjectToOldParent.ContainsKey(tag))
            //{
            //    var oldparent = mapObjectToOldParent[tag];
            //    mapObjectToOldParent.Remove(tag);

            //    var keys = mapObjectToParent.Keys.ToList();
            //    foreach (var key in keys)
            //    {
            //        if (mapObjectToParent[key] == oldparent)
            //            mapObjectToParent[key] = newitem;
            //    }
            //}

            TreeListNode newitem = null;

            if (tag is UFCrossReferenceTagFolder)
            {
                bKnownType = true;

                newitem = tree.AddNode(new CRTagsTreeItemControl(tag), parent, tag);
                var n = tag as UFCrossReferenceTagFolder;
                SetBindingOnProp(newitem, tag, nameof(n.Name));
                SetBindingOnProp(newitem, tag, nameof(n.IsNotValid), CRTagsTreeItemControl.IsNotValidProperty);
                SetBindingOnProp(newitem, tag, nameof(n.IsNotInUse), CRTagsTreeItemControl.IsNotInUseProperty);
                string typeIcon = (tag as UFCrossReferenceTagFolder).TypeDefinition;
                if (typeIcon != null)
                {
                    if (imageMap == null)
                        imageMap = new Dictionary<string, ImageSource>();
                    if (!imageMap.ContainsKey(typeIcon))
                    {
                        var typeImg = TryFindResource($"UFCR{typeIcon}") as ImageSource;
                        imageMap.Add(typeIcon, typeImg);
                    }
                    (newitem.Content as TreeItemControl).ResourceIcon = imageMap[typeIcon];
                }
                else
                    (newitem.Content as TreeItemControl).ResourceIcon = closedFolderImg;
                if((tag as UFCrossReferenceTagFolder).HasPrototypeModel)
                    UpdateNodeToPrototype(tag as UFCrossReferenceTagFolder, (tag as UFCrossReferenceTagFolder).GetRelativeName());
            }
            else if (tag is UFCrossReferenceStringFolder)
            {
                bKnownType = true;

                newitem = tree.AddNode(new CRTagsTreeItemControl(tag), parent, tag);
                var n = tag as UFCrossReferenceTagFolder;
                SetBindingOnProp(newitem, tag, nameof(n.Name));
                SetBindingOnProp(newitem, tag, nameof(n.IsNotValid), CRTagsTreeItemControl.IsNotValidProperty);
                SetBindingOnProp(newitem, tag, nameof(n.IsNotInUse), CRTagsTreeItemControl.IsNotInUseProperty);
                (newitem.Content as TreeItemControl).ResourceIcon = closedFolderImg;
            }
            else if (tag is UFCrossReferenceScreenFolder)
            {
                bKnownType = true;

                newitem = tree.AddNode(new CRScreenTreeItemControl(tag), parent, tag);
                var n = tag as UFCrossReferenceScreenFolder;
                SetBindingOnProp(newitem, tag, nameof(n.Name));
                SetBindingOnProp(newitem, tag, nameof(n.IsNotValid), CRScreenTreeItemControl.IsNotValidProperty);
                (newitem.Content as TreeItemControl).ResourceIcon = closedFolderImg;
            }
            else if (tag is UFCrossReferenceConnectionFolder)
            {
                bKnownType = true;

                newitem = tree.AddNode(new TreeItemControl(tag), parent, tag);
                var n = tag as UFCrossReferenceConnectionFolder;
                SetBindingOnProp(newitem, tag, nameof(n.Name));
                (newitem.Content as TreeItemControl).ResourceIcon = closedFolderImg;
            }
            else if (tag is UFCrossReferenceTag)
            {
                bKnownType = true;

                newitem = tree.AddNode(new CRTagsTreeItemControl(tag), parent, tag);
                var n = tag as UFCrossReferenceTag;
                SetBindingOnProp(newitem, tag, nameof(n.Name));
                SetBindingOnProp(newitem, tag, nameof(n.EndpointUrl), CRTagsTreeItemControl.EndpointUrlProperty);
                SetBindingOnProp(newitem, tag, nameof(n.ReadablePath), CRTagsTreeItemControl.ReadablePathProperty);
                SetBindingOnProp(newitem, tag, nameof(n.IsNotValid), CRTagsTreeItemControl.IsNotValidProperty);
                SetBindingOnProp(newitem, tag, nameof(n.IsNotInUse), CRTagsTreeItemControl.IsNotInUseProperty);

                string typeIcon = (tag as UFCrossReferenceTag).TypeDefinition;
                if(typeIcon != null)
                {
                    if (imageMap == null)
                        imageMap = new Dictionary<string, ImageSource>();
                    if (!imageMap.ContainsKey(typeIcon))
                    {
                        var typeImg = TryFindResource($"UFCR{typeIcon}") as ImageSource;
                        imageMap.Add(typeIcon, typeImg);
                    }
                    (newitem.Content as TreeItemControl).ResourceIcon = imageMap[typeIcon];
                }
                else
                    (newitem.Content as TreeItemControl).ResourceIcon = addressSpaceImg;
            }
            else if (tag is UFCrossReferenceScreen)
            {
                bKnownType = true;

                newitem = tree.AddNode(new CRScreenTreeItemControl(tag), parent, tag);
                var n = tag as UFCrossReferenceScreen;
                SetBindingOnProp(newitem, tag, nameof(n.Name));
                SetBindingOnProp(newitem, tag, nameof(n.ReadablePath), CRScreenTreeItemControl.ReadablePathProperty);
                SetBindingOnProp(newitem, tag, nameof(n.IsNotValid), CRScreenTreeItemControl.IsNotValidProperty);
                (newitem.Content as TreeItemControl).ResourceIcon = screenImg;
            }
            else if (tag is UFCrossReferenceConnection)
            {
                bKnownType = true;

                newitem = tree.AddNode(new CRConnectionTreeItemControl(tag), parent, tag);
                var n = tag as UFCrossReferenceConnection;
                SetBindingOnProp(newitem, tag, nameof(n.Name));
                (newitem.Content as TreeItemControl).ResourceIcon = connectionImg;
            }
            else if (tag is UFCrossReferenceString)
            {
                bKnownType = true;

                newitem = tree.AddNode(new CRTagsTreeItemControl(tag), parent, tag);
                var n = tag as UFCrossReferenceTag;
                SetBindingOnProp(newitem, tag, nameof(n.Name));
                SetBindingOnProp(newitem, tag, nameof(n.IsNotValid), CRTagsTreeItemControl.IsNotValidProperty);
                SetBindingOnProp(newitem, tag, nameof(n.IsNotInUse), CRTagsTreeItemControl.IsNotInUseProperty);
                (newitem.Content as TreeItemControl).ResourceIcon = stringImg;
            }
            else if (tag is UFCrossReferenceEntity)
            {
                bKnownType = true;

                newitem = tree.AddNode(new CREntityTreeItemControl(tag), parent, tag);
                var n = tag as UFCrossReferenceEntity;
                SetBindingOnProp(newitem, tag, nameof(n.Name));
                SetBindingOnProp(newitem, tag, nameof(n.TagName), CREntityTreeItemControl.TagNameProperty);
                SetBindingOnProp(newitem, tag, nameof(n.EntityReadablePath), CREntityTreeItemControl.EntityReadablePathProperty);
                SetBindingOnProp(newitem, tag, nameof(n.EndpointUrl), CREntityTreeItemControl.EndpointUrlProperty);
                SetBindingOnProp(newitem, tag, nameof(n.IsNotRefValid), CREntityTreeItemControl.IsNotRefValidProperty);
                SetBindingOnProp(newitem, tag, nameof(n.IsNotRefUsed), CREntityTreeItemControl.IsNotRefUsedProperty);
                SetBindingOnProp(newitem, tag, nameof(n.Container), CREntityTreeItemControl.ContainerProperty);

                string typeIcon = (tag as UFCrossReferenceEntity).TypeIcon;
                if (typeIcon != null)
                {
                    if (imageMap == null)
                        imageMap = new Dictionary<string, ImageSource>();
                    if (!imageMap.ContainsKey(typeIcon))
                    {
                        var typeImg = TryFindResource($"UFCR{typeIcon}") as ImageSource;
                        imageMap.Add(typeIcon, typeImg);
                    }
                    (newitem.Content as TreeItemControl).ResourceIcon = imageMap[typeIcon];
                }
                else
                    (newitem.Content as TreeItemControl).ResourceIcon = addressSpaceImg;

                mapStringIDsObjectToParent[tag] = parent;
            }
            else if (tag is String)
            {
                bKnownType = true;

                newitem = tree.AddNode(new TreeItemControl(tag), parent, tag);
                var n = tag as UFCrossReferenceConnection;
                (newitem.Content as TreeItemControl).Header = tag as String;
                (newitem.Content as TreeItemControl).ResourceIcon = connectionImg;
                bShiftDown = true;
            }

            if (!bShiftDown && parent.Nodes.Count > maxItems)
            {
                parent.Nodes.RemoveAt(0);
                var maxitem = GetTreeItem(Properties.Resources.MaxItemCountVisibleReached, parent);
                if (maxitem != null)
                {
                    parent.Nodes.Remove(maxitem);
                    parent.Nodes.Add(maxitem);
                }
                else
                    AddTreeItem(tree, Properties.Resources.MaxItemCountVisibleReached, parent);
            }

            if (NeedToBeExpanded(newitem))
                tree.AddNode(null, newitem, TreeListControlHelper.DummyNode);

            tree.RefreshRow(newitem.RowHandle); //Otherwise node's header (ItemHeader) can disappear in certain conditions
            return bKnownType ? newitem : null;
        }
        
        void UpdateNodeToPrototype(UFCrossReferenceTagFolder folder, string relative)
        {
            if(!nodeToPrototypesList.Contains(relative))
            {
                foreach (UFCrossReferenceTag t in folder.Tags)
                    nodeToPrototypesList.Add(t.ReadablePathNoProject);
                foreach (UFCrossReferenceTagFolder f in folder.UFUAFolders)
                {
                    var _rel = string.Format("{0}\\{1}", relative, f.Name);
                    UpdateNodeToPrototype(f, _rel);
                    nodeToPrototypesList.Add(_rel);
                }
            }
        }

        private void treeListControl_CustomSummary(object sender, DevExpress.Xpf.Grid.TreeList.TreeListCustomSummaryEventArgs e)
        {
            CrossReferenceType total = CrossReferenceType.Tags;
            if ((sender as TreeListView).DataControl == treeListControl)
                total = CrossReferenceType.Tags;
            else if ((sender as TreeListView).DataControl == treeListControlScreen)
                total = CrossReferenceType.Screens;
            else if ((sender as TreeListView).DataControl == treeListControlStringIDs)
                total = CrossReferenceType.Strings;
            else if ((sender as TreeListView).DataControl == treeListControlConnection)
                total = CrossReferenceType.Connections;
            else
                e.TotalValue = 0;

            if (Document == null)
                e.TotalValue = 0;
            else if (e.SummaryProcess == CustomSummaryProcess.Finalize && e.SummaryItem.FieldName == "IsNotRefValid")
                e.TotalValue = Document.GetInvalidReferences(total);
            else if (e.SummaryProcess == CustomSummaryProcess.Finalize && e.SummaryItem.FieldName == "IsNotRefUsed")
                e.TotalValue = Document.GetNotUsedReferences(total);
        }

        private void gridDataControl_CustomSummary(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            CrossReferenceType total = CrossReferenceType.Tags;
            GridControl control = ((DevExpress.Xpf.Grid.GridCustomSummaryEventArgs)e).Source;
            if (control == gridDataControl)
                total = CrossReferenceType.Tags;
            else if (control == gridScreenDataControl)
                total = CrossReferenceType.Screens;
            else if (control == gridStringIDsDataControl)
                total = CrossReferenceType.Strings;
            else if (control == gridConnectionDataControl)
                total = CrossReferenceType.Connections;
            else
                e.TotalValue = 0;

            if (Document == null)
                e.TotalValue = 0;            
            else if (e.SummaryProcess == CustomSummaryProcess.Finalize && (e.Item as GridSummaryItem).FieldName == "IsNotRefValid")
                e.TotalValue = Document.GetInvalidReferences(total);
            else if (e.SummaryProcess == CustomSummaryProcess.Finalize && (e.Item as GridSummaryItem).FieldName == "IsNotRefUsed")
                e.TotalValue = Document.GetNotUsedReferences(total);
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

        private void RefreshReferences()
        {
            throw new NotImplementedException();
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

        //internal void SelectItems(IList<UFCrossReferenceEntity> list)
        //{
        //    foreach (var tag in list)
        //    {
        //        var item = GetTreeItem(tag);
        //        if (item != null)
        //        {
        //            item.IsSelected = true;
                    //if(tabTags.IsSelected)
                    //    treeListControl.BringIntoView(item);
                    //else if(tabScreens.IsSelected)
                    //     ScreenReferenceTree.BringIntoView(item);
        //        }
        //    }

        //}

        // Added to solve FOGBUGZ 11409
        //internal void SelectItems(IList<UFCrossReferenceEntity> list, TreeListNode parent)
        //{
        //    foreach (var tag in list)
        //    {
        //        var item = GetTreeItem(tag, parent);
        //        if (item != null)
        //        {
        //            item.IsSelected = true;
                    //if (tabTags.IsSelected)
                    //    treeListControl.BringIntoView(item);
                    //else if (tabScreens.IsSelected)
                    //    ScreenReferenceTree.BringIntoView(item); 
        //        }
        //    }
        //}

        internal void CopySelectedToClipboard()
        {
            var listscreenfolders = new List<UFCrossReferenceScreen>();
            var listfolders = new List<UFCrossReferenceTag>();
            var listtags = new List<UFCrossReferenceEntity>();
            var listscreentags = new List<UFCrossReferenceEntity>();
            if (tabTags.IsSelected)
            {
                foreach (TreeListNode selectedItem in treeListControl.GetSelectedNodes())
                {
                    if (selectedItem.Tag is UFCrossReferenceTag)
                        listfolders.Add(selectedItem.Tag as UFCrossReferenceTag);
                    else if (selectedItem.Tag is UFCrossReferenceEntity)
                        listtags.Add(selectedItem.Tag as UFCrossReferenceEntity);
                }
            }
            else if (tabScreens.IsSelected)
            {
                foreach (TreeListNode selectedItem in treeListControlScreen.GetSelectedNodes())
                {
                    if (selectedItem.Tag is UFCrossReferenceScreen)
                        listscreenfolders.Add(selectedItem.Tag as UFCrossReferenceScreen);
                    else if (selectedItem.Tag is UFCrossReferenceEntity)
                        listscreentags.Add(selectedItem.Tag as UFCrossReferenceEntity);
                }
            }

            Document.CleanClipbaord();
            Document.CopyListFoldersToClipbaord(listfolders);
            Document.CopyListFoldersToClipbaord(listscreenfolders);
            Document.CopyListTagsToClipbaord(listtags);
            Document.CopyListTagsToClipbaord(listscreentags);
        }
        #endregion

        #region Properties

        CREditorDocument _Document;
        [Browsable(false)]
        public CREditorDocument Document
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

        #region Methods
        string stringPlaceolder = "CREditor";
        private void StringManager_CultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                if (bDisposed)
                    return;

                IDictionary<string, string> stringlist = null;
                bool bUntranslated = bDesignmode && stringManager.GetActiveCulture(Document, false) == String.Empty;
                if (!bUntranslated)
                    stringlist = stringManager.GetListStringForCulture(Document, stringManager.GetActiveCulture(Document));

                tabTagsTxt.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_TabTagsTitle", stringlist, Properties.Resources.TabTagsTitle);
                tabStringIDsSplitterTree.Header = tabConnectionSplitterTree.Header = tabScreenSplitterTree.Header = tabSplitterTree.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_HierarchicalTitle", stringlist, Properties.Resources.Hierarchical);
                tabStringIDsSplitterFlat.Header = tabConnectionSplitterFlat.Header = tabScreenSplitterFlat.Header = tabSplitterFlat.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_FlatTitle", stringlist, Properties.Resources.Flat);
                tabScreensTxt.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_TabScreenTitle", stringlist, Properties.Resources.TabScreensTitle);
                tabStringIDsTxt.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_TabStringIDsTitle", stringlist, Properties.Resources.TabStringIDsTitle);
                tabConnectionsTxt.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_TabConnectionsTitle", stringlist, Properties.Resources.TabConnectionsTitle);

                summaryRenamedStrings.DisplayFormat = summaryRenamedStringsFlat.DisplayFormat = TranslationHelper.TranlslateText($"_{stringPlaceolder}_TotalRemovedRenamedStrings", stringlist, Properties.Resources.TotalRemovedRenamedStrings);
                summaryNotInUseStrings.DisplayFormat = summaryNotInUseStringsFlat.DisplayFormat = TranslationHelper.TranlslateText($"_{stringPlaceolder}_TotalNotInUseStrings", stringlist, Properties.Resources.TotalNotInUseStrings);
                summaryRemovedScreens.DisplayFormat = summaryRemovedScreensFlat.DisplayFormat = TranslationHelper.TranlslateText($"_{stringPlaceolder}_TotalRemovedRenamedScreens", stringlist, Properties.Resources.TotalRemovedRenamedScreens);
                summaryRemovedTags.DisplayFormat = summaryRemovedTagsFlat.DisplayFormat = TranslationHelper.TranlslateText($"_{stringPlaceolder}_TotalRemovedRenamedTags", stringlist, Properties.Resources.TotalRemovedRenamedTags);
                summaryNotInUseTags.DisplayFormat = summaryNotInUseTagsFlat.DisplayFormat = TranslationHelper.TranlslateText($"_{stringPlaceolder}_TotalNotInUseTags", stringlist, Properties.Resources.TotalNotInUseTags);

                TranslationHelper.TranlslateTreeViewColumns(treeListControl.Columns, stringlist, stringPlaceolder, new Tuple<string, string>("ItemHeader", "Tags"));
                TranslationHelper.TranlslateGridDataColumns(gridDataControl.Columns, stringlist, stringPlaceolder);

                TranslationHelper.TranlslateTreeViewColumns(treeListControlScreen.Columns, stringlist, stringPlaceolder);
                TranslationHelper.TranlslateGridDataColumns(gridScreenDataControl.Columns, stringlist, stringPlaceolder);

                TranslationHelper.TranlslateTreeViewColumns(treeListControlConnection.Columns, stringlist, stringPlaceolder, new Tuple<string, string>("ItemHeader", "Conn"));
                TranslationHelper.TranlslateGridDataColumns(gridConnectionDataControl.Columns, stringlist, stringPlaceolder);

                TranslationHelper.TranlslateTreeViewColumns(treeListControlStringIDs.Columns, stringlist, stringPlaceolder, new Tuple<string, string>("ItemHeader", "Text"));
                TranslationHelper.TranlslateGridDataColumns(gridStringIDsDataControl.Columns, stringlist, stringPlaceolder);
            });
        }
        void InitializeCRTree()
        {
            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                if (bDisposed)
                    return;

                FillItems(itemRoot);

                itemRoot.IsExpanded = true;
            });

            //treeListControl.SelectedItemChanged += (s, e) =>
            //{
            //    e.Handled = true;
            //    List<Object> selecteditems = new List<Object>();
            //    foreach (TreeListNode item in treeListControl.SelectedItems)
            //    {
            //        if (item.Tag != null)
            //            selecteditems.Add(item.Tag);
            //    }
            //};
        }
        
       // public event EventHandler<ControlTabChangedEventArgs> SelectionTabChanged;
        #region OnSelectionTabChanged
        /// <summary>
        /// Triggers the SelectionTabChanged event.
        /// </summary>
        //void OnSelectionTabChanged(Object oldItem, Object newItem)
        //{
        //    var e = SelectionTabChanged;
        //    if (e != null)
        //    {
        //        var args = new ControlTabChangedEventArgs()
        //        {
        //            OldTabId = ControlTabEnum.None,
        //            NewTabId = ControlTabEnum.None
        //        };

        //        if (oldItem != null)
        //        {
        //            if (oldItem == tabTags)
        //                args.OldTabId = ControlTabEnum.Tags;
        //            else if (oldItem == tabScreens)
        //                args.OldTabId = ControlTabEnum.Screen;
        //            else if (oldItem == tabConnections)
        //                args.OldTabId = ControlTabEnum.Connection;
        //        }
        //        else
        //        {
        //            ScreenFlatGridRefresh();
        //            InitializeScreenCRTree();
        //            ConnectionFlatGridRefresh();
        //            InitializeConnectionCRTree();
        //        }

        //        if (newItem != null)
        //        {
        //            if (newItem == tabTags)
        //                args.NewTabId = ControlTabEnum.Tags;
        //            else if (newItem == tabScreens)
        //                args.NewTabId = ControlTabEnum.Screen;
        //            else if (newItem == tabConnections)
        //                args.NewTabId = ControlTabEnum.Connection;
        //        }

        //        e(this, args);
        //    }
        //}
        #endregion

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this) || bDesignmode)
            {
                if (e.Key == Key.F1)
                {
                    if (CrossReferenceEditorManagerComponent.crossreferenceManagerComponent.HelpProvider != null)
                        CrossReferenceEditorManagerComponent.crossreferenceManagerComponent.HelpProvider.OpenDialogHelpPage("CrossReference", true);
                    e.Handled = true;
                }
            }
        }

        //private void tabScreenSplitter_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var tabSplitter = sender as TabSplitter;
        //    if (tabSplitter.HideHeaderOnSingleChild)
        //    {
        //        TabPanelAdv tab = (tabSplitter.Template.FindName("PART_TabPanel", tabSplitter)) as TabPanelAdv;
        //        if (tab != null)
        //            tab.Visibility = System.Windows.Visibility.Collapsed;
        //    }
        //}

        private void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            e.Handled = true;
            if (e.Source.DataControl is TreeListControl)
            {
                TreeListControl treeListControl = (e.Source.DataControl as TreeListControl);
                if (treeListControl == null)
                    return;
                var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
                var root = (treeListControl.View as TreeListView).Nodes.FirstOrDefault();
                if (selected != null)
                {
                    if (selected.Tag as String == Properties.Resources.MaxItemCountVisibleReached)
                    {
                        using (var cursor = new WaitCursor())
                        {
                            var parent = selected.ParentNode;
                            if (parent == root)
                            {
                                ClearNodes(parent);
                                var oldMaxItem = maxItems;
                                maxItems = Int32.MaxValue;
                                FillItems(root);
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
                    else if (selected.Tag is UFCrossReferenceEntity)
                    {
                        var ufuatag = selected.Tag as UFCrossReferenceEntity;
                        EditEntity(ufuatag);
                    }
                }
            }
            else
            {
                var selected = e.Source.DataControl.SelectedItem as UFCrossReferenceEntity;
                if (selected != null)
                    EditEntity(selected);
            }
        }
        #endregion 

        #region Commands

        private void OnCommandSave(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var ret = Document.SaveToFile();
            if (!ret && uIInterface != null)
            {
                uIInterface.ShowError(String.Format(Properties.Resources.ErrorSavingDocWithoutClosure,
                    String.Format("{0} ({1})", Document.EditorManagerComponent.TypeTitle, Document.Parent.Title)));
            }
        }

        private void CanCommandSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document.NeedsSave;
        }

        private void OnCommandCut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                Document.CopyInMemoryDataToWinClipboard();
            }
        }

        private void CanCommandCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void OnCommandCopy(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                Document.CopyInMemoryDataToWinClipboard();
            }
        }

        private void CanCommandCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void OnCommandPaste(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                Document.CopyWinClipboardToInMemoryData(true);
            }
        }

        private void CanCommandPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            Document.CopyWinClipboardToInMemoryData();
        }

        private void OnUpdateCRDoc(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            bool bCancel;
            CRManagement cRManagement = GetManagerList(out bCancel);
            if (!bCancel)
                CompileCRDoc(cRManagement, false);
        }

        void CompileCRDoc(CRManagement cRManagement, bool bRename, bool bSilent = false)
        {
            if (iUFProjectManager == null || cRManagement.ManagerList.Count == 0 || cRManagement.CrossReferenceTypeList.Count == 0)
                return;

            var parent = DocumentHelper.GetRootParent(Document, traverse: false);
            var appname = string.Empty;
            if (Document.EditorManagerComponent.UFUAEditorManager != null)
                appname = Document.EditorManagerComponent.UFUAEditorManager.GetDefApplicationName(parent);

            Dictionary<String, String> renamedMap = null;
            var documentManagers = cRManagement.ManagerList;
            var p = DocumentHelper.GetRootParent(Document, traverse: false);
            bool needToClose = false;
            documentManagers.ForEach(d =>
            {
                needToClose = needToClose || (d as ICrossReference).NeedToCloseCRDocuments();
            });

            var UFUAProjectManager = Document.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
            if (bRename)
            {
                if (Document.EditorManagerComponent.UIMsgBoxAlertService?.ShowOkCancel(Properties.Resources.RefactorWarning, UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question) == UIMsgBoxAlertService.ComponentService.CustomDialogResults.Cancel)
                    return;
                renamedMap = new Dictionary<string, string>();
                if (UFUAProjectManager != null)
                    renamedMap = UFUAProjectManager.GetRenamedResources(p);
            }
            
            if (!bSilent && needToClose)
            {
                if(Document.EditorManagerComponent.UIMsgBoxAlertService?.ShowOkCancel(Properties.Resources.CrossReferenceWarning, UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question) == UIMsgBoxAlertService.ComponentService.CustomDialogResults.Cancel)
                    return;
            }

            if(needToClose)
                documentManagers.ForEach(docManager =>
                {
                    if(!(docManager is IUFProjectManager))
                        docManager.CloseAllChild(p);
                });

            workspace?.UpdateProgressState(0, 0, $"{Properties.Resources.StartingCR}", TaskbarItemProgressState.Indeterminate);

            if (workspace != null)
                workspace.IsBusy = true;

            if (cRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Tags))
                tabTags.IsSelected = true;
            else if (cRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Screens))
                tabScreens.IsSelected = true;
            else if (cRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Connections))
                tabConnections.IsSelected = true;
            else if (cRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Strings))
                tabStringIDs.IsSelected = true;

            var messageControl = new Controls.ControlMessage();
            TextMessage = string.Empty;
            messageControl.DataContext = this;
            var captions = GeneralDialogContent.GetDefaultButtonCaptions();
            captions[GeneralDialogButtons.CancelButton] = Properties.Resources.LabelCancel;
            var newDialog1 = new GeneralDialogContent(messageControl, GeneralDialogButtons.CancelButton, captions)
            {
                Owner = this.FindParent<Window>(),
                Title = Properties.Resources.DocumentEditorRibbonTitle,
                HelpLink = "CrossReference"
            };

            var scriptTagMap = new Dictionary<string, object>();
            var tokenSource = new CancellationTokenSource();
            var ct = tokenSource.Token;
            var mapCRItemsPerDocManager = new Dictionary<string, int>();
            var docManagerList = (from item in cRManagement.ManagerList.AsParallel()
                                  where item is ICrossReference
                                  orderby item.TypeScheme
                                  select item).ToList();
            Document.InitDataSyncReferences();
            Document.CRDocError += Document_CRDocError;
            
            TextMessage = $"{TextMessage}{Environment.NewLine}{Properties.Resources.StartingCR}";

            bool bAtLeastOneError = false;
            var pendingTask = new List<Task>();
            foreach (var manager in docManagerList)
            {
                var cREntitiesObject = new CREntitiesObject()
                {
                    DocumentManager = manager,
                    Parent = parent,
                    Cancellation = ct,
                    CRItemsPerDocManagerMap = mapCRItemsPerDocManager,
                    ScriptMap = scriptTagMap,
                    RenamedMap = renamedMap
                };

                Task task = null;
                if ((manager as ICrossReference).NeedSingleThreadedApartment)
                {
                    var source = new TaskCompletionSource<object>();
                    task = source.Task;
                    var thread2 = new Thread(() =>
                    {
                        try
                        {
                            ManageCREntities(cREntitiesObject, cRManagement, bRename);
                        }
                        catch (Exception ex)
                        {
                            source.SetException(ex);
                        }
                        finally
                        {
                            if (ct.IsCancellationRequested)
                                source.SetCanceled();

                            if (!source.Task.IsCanceled && !source.Task.IsFaulted)
                                source.SetResult(null);
                        }
                    });
                    thread2.SetApartmentState(ApartmentState.STA);
                    thread2.IsBackground = true;
                    thread2.Start();
                }
                else
                {
                    task = Task.Factory.StartNew((o) =>
                    {
                        ManageCREntities(cREntitiesObject, cRManagement, bRename);
                    }, null, TaskCreationOptions.LongRunning);
                }
                task.ContinueWith((ret) =>
                {
                    log.Error(ret.Exception.InnerException.Message);
                    cREntitiesObject.ErrorResult = true;
                }, TaskContinuationOptions.OnlyOnFaulted);
                task.ContinueWith((o) =>
                {
                    bAtLeastOneError |= cREntitiesObject.ErrorResult;
                    pendingTask.Remove(task);
                    if (pendingTask.Count == 0)
                    {
                        if (!ct.IsCancellationRequested)
                        {
//#if DEBUG
                            TextMessage = $"{TextMessage}{Environment.NewLine}{Properties.Resources.CRUpdatingItems}";
//#endif
                            var task2 = Task.Factory.StartNew((o2) =>
                            {
                                Document.UpdateItemExistence(ct, (from d in cRManagement.ManagerList where d is IUFUAEditorManager select d).FirstOrDefault() != null);
                            }, null, TaskCreationOptions.LongRunning);
                            task2.ContinueWith((ret) =>
                            {
                                log.Error(ret.Exception.InnerException.Message);
                                cREntitiesObject.ErrorResult = true;
                            }, TaskContinuationOptions.OnlyOnFaulted);
                            task2.ContinueWith(ret =>
                            {
                                bAtLeastOneError |= cREntitiesObject.ErrorResult;
                                pendingTask.Remove(task2);
                                newDialog1.DialogResult = !ct.IsCancellationRequested;
                            }, TaskScheduler.FromCurrentSynchronizationContext());
                            pendingTask.Add(task2);
                        }
                        else
                            newDialog1.DialogResult = !ct.IsCancellationRequested;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
                pendingTask.Add(task);
            }

            bool bLoaded = false;
            messageControl.Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
#if DEBUG
                log.Warn($"CRCompiler START at {DateTime.Now}");
#endif

                if (workspace != null)
                    workspace.IsBusy = false;

                workspace?.UpdateProgressState(0, cRManagement.ManagerList.Count * 2, 
                    $"{Properties.Resources.StartingCR}", TaskbarItemProgressState.Normal);
            };

            newDialog1.Closing += (o, e) =>
            {
                if (pendingTask.Count > 0)
                {
                    if (!ct.IsCancellationRequested)
                    {
                        if (tokenSource != null)
                            tokenSource.Cancel();

                        TextMessage = $"{TextMessage}{Environment.NewLine}{Properties.Resources.AbortingCR}";

                        (o as GeneralDialogContent).DisableCancelButton();
                    }

                    e.Cancel = true;
                }
            };

            var result = newDialog1.ShowDialog();
            if (result == true)
            {
                if (bAtLeastOneError)
                {
                    workspace?.ShowSystemLog();
                    if (Document.EditorManagerComponent.UIMsgBoxAlertService != null)
                        Document.EditorManagerComponent.UIMsgBoxAlertService.ShowWarning(Properties.Resources.CrossReferenceGenericError);
                }

                LoadResult();
                Document.NeedsSave = true;
                if(bRename && UFUAProjectManager != null && cRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Screens))
                {
                    var screenManager = (Document.GetService(typeof(IScreenManager)) as IScreenManager) as IDocumentManager;
                    if (screenManager != null)
                        UFUAProjectManager.ClearRenamedResourcesMap(p, new List<string>() { { screenManager.TypeLabel } });
                }
            }
            else
                Document.ClearCRMaps();

            Document.CRDocError -= Document_CRDocError;

            if (tokenSource != null)
                tokenSource.Dispose();

            workspace?.ResetProgressState();
#if DEBUG
            log.Warn($"CRCompiler STOP at {DateTime.Now}");
#endif
        }

        private void LoadResult()
        {
            if (workspace != null)
                workspace.IsBusy = true;

            try
            {
                Document.ClearList();
                Document.CopyFromCompiler();
                Document.UpdateSummary();
                Document.ClearCRMaps();

                ClearTreeView();
                ClearGrid();

                workspace?.IncrementProgressState();
                InitializeCRTree();
                workspace?.IncrementProgressState();
                InitializeScreenCRTree();
                workspace?.IncrementProgressState();
                InitializeConnectionCRTree();
                workspace?.IncrementProgressState();
                InitializeStringCRTree();
                workspace?.IncrementProgressState();
                FlatGridRefresh();
                workspace?.IncrementProgressState();
            }
            finally
            {
                if (workspace != null)
                    workspace.IsBusy = false;
            }
        }

        void ClearGrid()
        {
            DesignerProperties.SetIsInDesignMode(gridDataControl, false);
            gridDataControl.ItemsSource = null;
            DesignerProperties.SetIsInDesignMode(gridScreenDataControl, false);
            gridScreenDataControl.ItemsSource = null;
            DesignerProperties.SetIsInDesignMode(gridConnectionDataControl, false);
            gridConnectionDataControl.ItemsSource = null;
            DesignerProperties.SetIsInDesignMode(gridStringIDsDataControl, false);
            gridStringIDsDataControl.ItemsSource = null;
        }

        private void InitializeStringCRTree()
        {
            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                if (bDisposed)
                    return;

                FillStringItems(itemStringRoot);

                itemStringRoot.IsExpanded = true;
            });
        }

        private void InitializeConnectionCRTree()
        {
            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                if (bDisposed)
                    return;

                FillConnectionItems(itemConnectionRoot);

                itemConnectionRoot.IsExpanded = true;
            });

            //ConnectionReferenceTree.SelectedItemChanged += (s, e) =>
            //{
            //    e.Handled = true;
            //    List<Object> selecteditems = new List<Object>();
            //    foreach (TreeListNode item in ScreenReferenceTree.SelectedItems)
            //    {
            //        if (item.Tag != null)
            //            selecteditems.Add(item.Tag);
            //    }
            //};
        }

        private void FillStringItems(TreeListNode itemRoot, UFCrossReferenceStringFolder root = null)
        {
            treeListControlStringIDs.BeginDataUpdate();
            try
            {
                ClearNodes(itemRoot);

                if (workspace != null)
                    workspace.IsBusy = true;

                bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.LeftShift);

                using (new AsyncWaitCursor())
                {
                    ClearNodes(itemRoot);
                    int i = 0;
                    var listFolders = Document.GetStringFolderCollection(root);
                    if (listFolders != null)
                    {
                        foreach (var folder in listFolders)
                        {
                            AddTreeItem(treeListControlStringIDs, folder, itemRoot);
                            if (!bShiftDown && ++i > maxItems)
                            {
                                break;
                            }
                        }
                    }
                    i = 0;

                    var listItems = Document.GetStringCRTags(root);
                    if (listItems != null)
                    {
                        foreach (var itemConnection in listItems)
                        {
                            AddTreeItem(treeListControlStringIDs, itemConnection, itemRoot);
                            if (!bShiftDown && ++i > maxItems)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                treeListControlStringIDs.EndDataUpdate();
                if (workspace != null)
                    workspace.IsBusy = false;
            }
        }

        private void FillConnectionItems(TreeListNode itemRoot, UFCrossReferenceConnectionFolder root = null)
        {
            treeListControlConnection.BeginDataUpdate();
            try
            {
                ClearNodes(itemRoot);

                if (workspace != null)
                    workspace.IsBusy = true;

                bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.LeftShift);

                using (new AsyncWaitCursor())
                {
                    ClearNodes(itemRoot);
                    int i = 0;
                    var listFolders = Document.GetConnectionFolderCollection(root);
                    if (listFolders != null)
                    {
                        foreach (var folder in listFolders)
                        {
                            AddTreeItem(treeListControlConnection, folder, itemRoot);
                            if (!bShiftDown && ++i > maxItems)
                            {
                                break;
                            }
                        }
                    }
                    i = 0;

                    var listItems = Document.GetConnectionCRTags(root);
                    if (listItems != null)
                    {
                        foreach (var itemConnection in listItems)
                        {
                            AddTreeItem(treeListControlConnection, itemConnection, itemRoot);
                            if (!bShiftDown && ++i > maxItems)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                treeListControlConnection.EndDataUpdate();
                if (workspace != null)
                    workspace.IsBusy = false;
            }
        }

        private void ManageCREntities(CREntitiesObject cREntitiesObject, CRManagement cRManagement, bool bRename)
        {
            var x = cREntitiesObject.DocumentManager;
            IEnumerable<string> resourcelist;
          
#if DEBUG
            using (var reswatch = new StopWatcher($"CR GetResourceList for {x.TypeScheme} took : " + "{0}"))
#endif
            {
                resourcelist = iUFProjectManager.GetResourceList(Document, x.TypeScheme);
            }
//#if DEBUG
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                String textTypeScheme;
                if (resourcelist.Count() > 0)
                    textTypeScheme = string.Format(Properties.Resources.CRDocManagerStartedMultipleDocument, x.TypeTitle, resourcelist.Count());
                else
                    textTypeScheme = string.Format(Properties.Resources.CRDocManagerStartedSingleDocument, x.TypeTitle);
                TextMessage = $"{TextMessage}{Environment.NewLine}{textTypeScheme}";
            });
//#endif
            if (cREntitiesObject.Cancellation.IsCancellationRequested)
                return;
            try
            {
                var model = new UFInterfaces.Editors.CrossReferenceModel(Document, cREntitiesObject.Cancellation, cREntitiesObject.ScriptMap, resourcelist, cRManagement, cREntitiesObject.RenamedMap);
#if DEBUG
                string watcherTxt = bRename ? "RenameAndGetCRObjects" : "OnlyGetCRObjects";
                using (var reswatch = new StopWatcher($"CR {watcherTxt} from {x.TypeScheme} took : " + "{0}"))
#endif
                {
                    if(bRename)
                        (x as ICrossReference).RenameCRObjects(model);
                    
                    cREntitiesObject.CRResultList = (x as ICrossReference).GetCRObjects(model);

                    if (cREntitiesObject.CRResultList == null || cREntitiesObject.CRResultList.Count == 0)
                        cREntitiesObject.CRItemsPerDocManagerMap[x.TypeScheme] = 0;
                    else
                        cREntitiesObject.CRItemsPerDocManagerMap[x.TypeScheme] = cREntitiesObject.CRResultList.Count;
                }

                if (model.ErrorMessages.Count > 0)
                {
                    string errors = String.Join($"{Environment.NewLine}", model.ErrorMessages.Distinct().ToList());
                    log.Error(errors);
                    cREntitiesObject.ErrorResult = true;
                }
            }
            catch (Exception ex)
            {
                cREntitiesObject.CRItemsPerDocManagerMap[x.TypeScheme] = 0;
                log.Error(ex.Message);
                cREntitiesObject.ErrorResult = true;
            }
            finally
            {
                workspace?.IncrementProgressState();
            }

            if (cREntitiesObject.Cancellation.IsCancellationRequested)
                return;
#if DEBUG
            using (var reswatch = new StopWatcher($"CR AddingEnity from {x.TypeScheme} took : " + "{0}"))
#endif
            {
                try
                {
                    Document.AddEntities(cREntitiesObject.CRResultList, cREntitiesObject.Cancellation);
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    cREntitiesObject.ErrorResult = true;
                }
                finally
                {
                    workspace?.IncrementProgressState();
                }
//#if DEBUG
                Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                {
                    if (cREntitiesObject.CRItemsPerDocManagerMap.ContainsKey(x.TypeScheme))
                    {
                        var textTypeScheme = string.Format(Properties.Resources.CRDocManagerCompleted, x.TypeTitle, cREntitiesObject.CRItemsPerDocManagerMap[x.TypeScheme]);
                        TextMessage = $"{TextMessage}{Environment.NewLine}{textTypeScheme}";
                    }
                });
//#endif
            }
        }

        private void Document_CRDocError(object sender, CREditorDocument.CREventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                if (uIInterface != null)
                    uIInterface.ShowError(e.Message);
            });
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

        private void CanUpdateCRDoc(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void OnClearCRDoc(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (workspace != null)
                workspace.IsBusy = true;
            try
            {
                Document.ClearList();
                Document.UpdateSummary(true);
                ClearTreeView();
                ClearGrid();
                Document.NeedsSave = true;
            }
            finally
            {
                if (workspace != null)
                    workspace.IsBusy = false;
            }
        }

        private void OnRenameReferences(object sender, ExecutedRoutedEventArgs e)
        {
            bool bCancel;
            CRManagement cRManagement = GetManagerList(out bCancel);
            if (!bCancel)
                CompileCRDoc(cRManagement, true);
        }

        private CRManagement GetManagerList(out bool bCancel)
        {
            bCancel = true;
            CRManagement cRManagement = new CRManagement();
            var documentManagers = (from item in CrossReferenceEditorManagerComponent.UriRisolver.GetListInstalledDocumentManagers()
                                    where item is ICrossReference
                                    select item).OrderBy(d => d.TypeTitle).ToList();
            List<IDocumentManager> listDocumentManagers = new List<IDocumentManager>();
            var documentManagerSelector = new Controls.DocumentManagerSelector(documentManagers) { DataContext = Document};
            var dialog = new GeneralDialogContent(documentManagerSelector, GeneralDialogButtons.OkCancelButtons)
            {
                Owner = this.FindParent<Window>(),
                Title = Properties.Resources.DocumentManagerSelector,
                HelpLink = "CrossReference"
            };
            bool result = (bool)dialog.ShowDialog();
            if (result == false)
                return cRManagement;
            documentManagerSelector.SlectionList.ForEach(s =>
            {
                if (s.IsEnabled)
                    cRManagement.ManagerList.Add(s.DocumentManager);
            });
            documentManagerSelector.CrossReferenceTypeList.ForEach(s =>
            {
                if (s.IsEnabled)
                    cRManagement.CrossReferenceTypeList.Add(s.ReferenceType);
            });
            bCancel = false;
            return cRManagement;
        }

        private void CanRenameReferences(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ClearTreeView()
        {
            treeListControl.BeginDataUpdate();
                ClearNodes(itemRoot);
                if (itemRoot == null)
                    itemRoot = treeListControl.AddNode(new CRTagsTreeItemControl(rootTagName) { ResourceIcon = closedFolderImg }, Tag as TreeListNode, Document);
            if(NeedToBeExpanded(itemRoot))
                treeListControl.AddNode(null, itemRoot, TreeListControlHelper.DummyNode);
            treeListControl.EndDataUpdate();

            treeListControlScreen.BeginDataUpdate();
                ClearNodes(itemScreenRoot);
                if (itemScreenRoot == null)
                    itemScreenRoot = treeListControlScreen.AddNode(new CRScreenTreeItemControl(rootScreenName) { ResourceIcon = closedFolderImg }, Tag as TreeListNode, Document);
            if (NeedToBeExpanded(itemScreenRoot))
                treeListControlScreen.AddNode(null, itemScreenRoot, TreeListControlHelper.DummyNode);
            treeListControlScreen.EndDataUpdate();

            treeListControlConnection.BeginDataUpdate();
            ClearNodes(itemConnectionRoot);
                if (itemConnectionRoot == null)
                    itemConnectionRoot = treeListControlConnection.AddNode(new CRConnectionTreeItemControl(rootConnectionName) { ResourceIcon = closedFolderImg }, Tag as TreeListNode, Document);
            if (NeedToBeExpanded(itemScreenRoot))
                treeListControlConnection.AddNode(null, itemConnectionRoot, TreeListControlHelper.DummyNode);
            treeListControlConnection.EndDataUpdate();

            treeListControlStringIDs.BeginDataUpdate();
            ClearNodes(itemStringRoot);
            if (itemStringRoot == null)
                itemStringRoot = treeListControlStringIDs.AddNode(new CRConnectionTreeItemControl(rootStringName) { ResourceIcon = closedFolderImg }, Tag as TreeListNode, Document);
            if (NeedToBeExpanded(itemStringRoot))
                treeListControlStringIDs.AddNode(null, itemStringRoot, TreeListControlHelper.DummyNode);
            treeListControlStringIDs.EndDataUpdate();

            mapStringIDsObjectToParent.Clear();
            nodeToPrototypesList.Clear();
        }
        private void CanClearCRDoc(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

#endregion

#region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            //if(Document.EditorManagerComponent.Workspace != null)
            //    Document.EditorManagerComponent.Workspace.ContextObject = null;if (tokenSource != null)
            mapStringIDsObjectToParent.Clear();
            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;
            if(imageMap != null)
                imageMap.Clear();
            imageMap = null;
            try
            {
                gridDataControl.Dispose();
            }
            catch { }

            try
            {
                gridScreenDataControl.Dispose();
            }
            catch { }

        }
#endregion

        //private void tabConnectionSplitter_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var tabSplitter = sender as TabSplitter;
        //    if (tabSplitter.HideHeaderOnSingleChild)
        //    {
        //        TabPanelAdv tab = (tabSplitter.Template.FindName("PART_TabPanel", tabSplitter)) as TabPanelAdv;
        //        if (tab != null)
        //            tab.Visibility = System.Windows.Visibility.Collapsed;
        //    }

        //}
    }
}

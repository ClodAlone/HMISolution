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
using System.ComponentModel;
using Utilities;
using Utilities.WPF;
using DocumentManager.ComponentService;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Diagnostics;
using Utilities.ProgressDialog;
using UFProjectManager.ComponentService;
using System.Threading;
using UIMsgBoxAlertService.ComponentService;
using OPCUAViewModel;
using System.Text.RegularExpressions;
using UFInterfaces.Service;
using WPFUtilities;
using log4net;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using DevExpress.Xpf.Core;
using UFInterfaces;
using ScreenManager.ComponentService;
using System.Windows.Threading;
using DevExpress.Xpf.Editors;
using System.Threading.Tasks;
using Utilities.Commands;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using DevExpress.Xpf.Bars;
using ViewModelLib;
using DevExpress.Data.TreeList;
using HelpProvider.ComponentService;
using ClientEditor.ComponentService;
using UFUAEditor.ComponentService;

namespace UFProjectManager
{
    /// <summary>
    /// Interaction logic for UFProjectExplorerUI.xaml
    /// </summary>
    public partial class UFProjectExplorerUI : UserControl, IDisposable
    {
        #region Declarations
        readonly BitmapImage openFolderImg;
        readonly BitmapImage closedFolderImg;

        readonly UFProjectManagerComponent projectManagerComponent;
        DispatcherOperation dpActivateExplorer;

        bool IsActiveWindow
        {
            get
            {
                return projectManagerComponent != null && projectManagerComponent.Workspace != null && projectManagerComponent.Workspace.ActiveWindow == this;
            }
        }

        bool IsComponentReady
        {
            get
            {
                return projectManagerComponent != null && projectManagerComponent.Workspace != null;
            }
        }

        readonly Dictionary<Object, TreeListNode> mapObjectToNode = new Dictionary<Object, TreeListNode>();
        Dictionary<ResourceFolderWatcher, TreeListNode> folderNodes = new Dictionary<ResourceFolderWatcher, TreeListNode>();
        Dictionary<UFProjectDocument, Dictionary<string, TreeListNode>> mapMainComponentsRoot = new Dictionary<UFProjectDocument, Dictionary<string, TreeListNode>>();

        static readonly ILog log = LogManager.GetLogger(Properties.Resources.GeneralLog);
        TreeListNode lastAddedItem;
        TreeListNode lastAddedFolder;
        ObservableCollection<IDocumentManager> detailsGridCollection;
        bool bCreatingFolderResource;
        bool bDisposed;

        RelayCommand _openResCommand;
        [Browsable(false)]
        public ICommand OpenResourceCommand
        {
            get
            {
                if (_openResCommand == null)
                {
                    _openResCommand = new RelayCommand(
                        param => OpenResource()
                        );
                }
                return _openResCommand;
            }
        }

#if !DEBUG
        bool webDeployEnabled = false;
        bool geolocal = false;
        bool bLogExceptions = false;
        readonly static Mindscape.Raygun4Net.RaygunClient _raygunClient = new Mindscape.Raygun4Net.RaygunClient("6zdPg/EmwqK4MYa9G1JLhQ==");
#endif
        #endregion

        bool bLoaded;
        public bool IsLoaded
        {
            get
            {
                return bLoaded;
            }
        }

        void OnTreeNodeExpanding(object sender, DevExpress.Xpf.Grid.TreeList.TreeListNodeAllowEventArgs e)
        {
            TreeListNode item = e.Node;

            try
            {
                if (item == null || TreeListControlHelper.WasExpanded(item)) //Node's subtree already populated
                    return;

                //UpdateFolderIcon(item, true);

                if (e != null && !CanBeExpanded(item))
                    return;

                (item.Content as TreeItemControl).IsNodeExpanding = true;
                treeListControl.BeginDataUpdate();
                ClearNodes(item);

                using (var cursor = new WaitCursor())
                {
                    PopulateSubItem(cursor, item);
                }
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

        void ClearNodes(TreeListNode node)
        {
            node.Nodes.Clear();
        }

        void ClearObjectMapNode(TreeListNode node)
        {
            if (node.Tag != null && node.Tag != TreeListControlHelper.DummyNode && mapObjectToNode.ContainsKey(node.Tag))
            {
                mapObjectToNode.Remove(node.Tag);
                var keys = (from key in watchingList.Keys/*.AsParallel()*/
                            where watchingList[key] == node
                            select key).ToList();
                foreach (var watched in keys)
                {
                    watched.CollectionChanged -= OnDocumentManagerCollectionChanged;
                    watchingList.Remove(watched);
                }
                if (node.Tag is IDocumentManager)
                {
                    var docManager = node.Tag as IDocumentManager;
                    if (docManager.BrowsableContent is INotifyPropertyChanged)
                    {
                        var changeNotifier = docManager.BrowsableContent as INotifyPropertyChanged;
                        if (mapSubscribedItems.ContainsKey(changeNotifier))
                        {
                            changeNotifier.PropertyChanged -= ChangeNotifier_PropertyChanged;
                            mapSubscribedItems.Remove(changeNotifier);
                        }
                        if (mapDispatcherOperations.ContainsKey(changeNotifier))
                        {
                            mapDispatcherOperations[changeNotifier].Abort();
                            mapDispatcherOperations.Remove(changeNotifier);
                        }
                    }
                }
            }

            foreach (var child in node.Nodes)
                ClearObjectMapNode(child);
        }

        void OnTreeNodeChanged(object sender, TreeListNodeChangedEventArgs e)
        {
            if (e.ChangeType == NodeChangeType.Add && e.Node.Tag != null && e.Node.Tag != TreeListControlHelper.DummyNode && !mapObjectToNode.ContainsKey(e.Node.Tag))
                mapObjectToNode.Add(e.Node.Tag, e.Node);
            else if (e.ChangeType == NodeChangeType.Remove && e.Node.Tag != null && e.Node.Tag != TreeListControlHelper.DummyNode)
                ClearObjectMapNode(e.Node);
        }

        void OnTreeNodeCollapsing(object sender, DevExpress.Xpf.Grid.TreeList.TreeListNodeAllowEventArgs e)
        {
            if (!(e.Node.Content as TreeItemControl).IsNodeExpanding)
                UpdateFolderIcon(e.Node, false);
        }

        void UpdateFolderIcon(TreeListNode node, bool isOpen)
        {
            if (node != null && node.Tag is ResourceFolderWatcher)
            {
                var folder = node.Tag as ResourceFolderWatcher;
                if (!folder.IsRoot)
                    (node.Content as TreeItemControl).ResourceIcon = isOpen ? openFolderImg : closedFolderImg;
            }
            if(node.Tag is IDocumentManager) 
            {
                var doc = node.Tag as IDocumentManager;
                (node.Content as TreeItemControl).ResourceIcon = isOpen ? doc.TypeIconOpen : doc.TypeIcon;
            }
        }

        void PopulateSubItem(WaitCursor cursor, TreeListNode item)
        {
            var document = GetNearestRoot(item).Tag as UFProjectDocument;
            if (item.Tag is Uri)
            {
                var uri = item.Tag as Uri;
                var doc = document.GetResourceDocumentManager(uri);
                var list = doc.GetChildDocumentManagers(uri, document);
                if (list != null)
                    AddSubItemsList(item, list);
                else
                {
                    var relative = uri.GetPathString();
                    if (!XpoHelpers.XpoHelper.IsDataSource(relative))
                    {
                        var match = String.Format("{0}/", document.Title);
                        if (relative.StartsWith(match))
                            relative = relative.Replace(match, "");
                        else
                        {
                            match = String.Format("{0}\\", document.Title);
                            if (relative.StartsWith(match))
                                relative = relative.Replace(match, "");
                        }
                        uri = document.MakeAbosoluteUri(new Uri(relative, UriKind.RelativeOrAbsolute));
                    }

                    var docGetDocument = doc.GetDocument(uri) as UFProjectDocument;
                    if (docGetDocument == null)
                    {
                        projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, uri, Properties.Resources.NotFound));
                    }
                    else
                    {
                        if (docGetDocument.IsPasswordProtected())
                        {
                            cursor.Release();
                            bool bok = false;
                            while (!bok)
                            {
                                var keyControl = new Controls.EnterKey();
                                var newDialog = new GeneralDialogContent(keyControl)
                                {
                                    Owner = Application.Current.MainWindow,
                                    Title = Properties.Resources.EnterCurrentProjectPassword,
                                    HelpLink = "EnterCurrentProjectPassword"
                                };

                                if (newDialog.ShowDialog() == true)
                                {
                                    if (docGetDocument.MatchPassword(keyControl.keyBox.Password))
                                    {
                                        bok = true;
                                        break;
                                    }
                                }
                                else
                                    break;
                            }
                            cursor.Aquire();

                            if (bok == false)
                            {
                                docGetDocument.Dispose();
                                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                                {
                                    item.IsExpanded = false;
                                    treeListControl.AddNode(null, item, TreeListControlHelper.DummyNode);
                                });
                                return;
                            }
                        }

                        docGetDocument.Parent = document;
                        //item.Tag = docGetDocument;
                        InitializeDocumentTree(docGetDocument, item);
                    }
                }
            }
            else if (item.Tag is IDocumentManager)
            {
                var doc = item.Tag as IDocumentManager;
                if (doc.TypeScheme == projectManagerComponent.TypeScheme)
                {
                    foreach (var resource in document.ListChildProjectPaths)
                        AddTreeItem(resource, item);
                }
                else
                {
                    var list = GetChildList(item, document, doc, Document);
                    if (list != null)
                        AddSubItemsList(item, list);
                }
            }
            else if (item.Tag is ResourceFolderWatcher)
            {
                var type = item.Tag as ResourceFolderWatcher;
                var resources = type.ListResources;
                foreach (var resource in resources)
                    AddTreeItem(resource, item);

                var folders = type.ListFolders;
                foreach (var folder in folders)
                    AddTreeItem(folder, item);

                resources.CollectionChanged += (o, ev) =>
                {
                    Dispatcher.InvokeIfRequired(() =>
                    {
                        var prevNode = treeListControl.GetPreviousNode();
                        treeListControl.ClearSelection();
                        if (ev.NewItems != null)
                        {
                            TreeListNode treeItem = null;
                            foreach (var uri in ev.NewItems)
                            {
                                treeItem = AddTreeItem(uri, item);
                            }
                            if (!type.IsRefreshing)
                            {
                                if (treeItem != null)
                                    treeListControl.SelectNode(treeItem);
                                lastAddedItem = treeItem;
                            }
                        }
                        if (ev.OldItems != null)
                        {
                            var selectedNode = treeListControl.SelectedItem as TreeListNode;
                            foreach (var uri in ev.OldItems)
                            {
                                var i = (from p in item.Nodes
                                         where p.Tag as Uri == uri as Uri
                                         select p).ToList();
                                i.ForEach(j =>
                                {
                                    treeListControl.View.DeleteNode(j);
                                });
                            }
                            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                            {
                                if (treeListControl.View.FocusedRowHandle < 0)
                                {
                                    if (lastAddedItem != null && lastAddedItem.RowHandle > 0)
                                        treeListControl.SelectNode(lastAddedItem);
                                    else
                                        treeListControl.SelectNode(prevNode);
                                }
                                else
                                    treeListControl.SelectItem(treeListControl.View.FocusedRowHandle);
                            });
                        }
                    });
                };

                folders.CollectionChanged += (o, ev) =>
                {
                    Dispatcher.InvokeIfRequired(() =>
                    {
                        var prevNode = treeListControl.GetPreviousNode();
                        treeListControl.ClearSelection();

                        if (ev.NewItems != null)
                        {
                            TreeListNode treeItem = null;
                            foreach (var folder in ev.NewItems)
                            {
                                treeItem = AddTreeItem(folder, item);
                            }
                            if (!type.IsRefreshing)
                            {
                                if (treeItem != null)
                                    treeListControl.SelectNode(treeItem);
                                lastAddedFolder = treeItem;
                            }
                        }
                        if (ev.OldItems != null)
                        {
                            foreach (var folder in ev.OldItems)
                            {
                                var i = (from p in item.Nodes
                                         where p.Tag == folder
                                         select p).ToList();
                                i.ForEach(j => treeListControl.View.DeleteNode(j));
                            }
                            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                            {
                                if (treeListControl.View.FocusedRowHandle < 0)
                                {
                                    if (lastAddedFolder != null && lastAddedFolder.RowHandle > 0)
                                        treeListControl.SelectNode(lastAddedFolder);
                                    else
                                        treeListControl.SelectNode(prevNode);
                                }
                                else
                                    treeListControl.SelectItem(treeListControl.View.FocusedRowHandle);
                            });
                        }
                    });
                };
            }
        }

        void OnDocumentManagerCollectionChanged(object o, System.Collections.Specialized.NotifyCollectionChangedEventArgs ev)
        {
            Dispatcher.InvokeIfRequired(() =>
            {
                var list = (ObservableCollection<IDocumentManager>)o;
                if (!watchingList.ContainsKey(list))
                    return;

                var item = watchingList[list];
                if ((item.Content as TreeItemControl).IsNodeExpanding)
                    return;

                if (ev.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    RefreshAsync(item);
                }
                else if ((itemsToRefresh == null || !itemsToRefresh.Contains(item)) && TreeListControlHelper.WasExpanded(item))
                {
                    if (ev.NewItems != null && ev.NewItems.Count != 0)
                    {
                        treeListControl.BeginDataUpdate();
                        foreach (IDocumentManager dvm in ev.NewItems)
                        {
                            if (mapObjectToNode.ContainsKey(dvm))
                                continue;

                            AddTreeItem(dvm, item);
                        }
                        treeListControl.EndDataUpdate();
                    }

                    if (ev.OldItems != null && ev.OldItems.Count != 0)
                    {
                        treeListControl.BeginDataUpdate();
                        foreach (IDocumentManager dvm in ev.OldItems)
                        {
                            if (mapObjectToNode.ContainsKey(dvm))
                            {
                                var parent = mapObjectToNode[dvm].ParentNode;
                                parent.Nodes.Remove(mapObjectToNode[dvm]);
                            }
                        }
                        treeListControl.EndDataUpdate();
                    }
                }
            });
        }

        List<TreeListNode> itemsToRefresh;
        void RefreshAsync(TreeListNode item)
        {
            if (itemsToRefresh == null || !itemsToRefresh.Contains(item))
                RefreshAsync(new List<TreeListNode>() { item });
        }

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

        void Refresh(TreeListNode item)
        {
            var wasExpanded = item.IsExpanded;
            if (item.IsExpanded && item.Nodes.Count == 0)
                treeListControl.AddNode(null, item, TreeListControlHelper.DummyNode);
            item.IsExpanded = false;
            ClearNodes(item);
            var docManager = item.Tag as IDocumentManager;
            if (docManager != null && docManager.IsResourceExpandable(Document))
            {
                treeListControl.AddNode(null, item, TreeListControlHelper.DummyNode);
                if (wasExpanded)
                    item.IsExpanded = true;
            }
            else {
                var fw = item.Tag as ResourceFolderWatcher;
                if (fw != null)
                    fw.Discover(true);
            }
        }

        Dictionary<ObservableCollection<IDocumentManager>, TreeListNode> watchingList = new Dictionary<ObservableCollection<IDocumentManager>, TreeListNode>();
        private void WatchListChanges(TreeListNode item, ObservableCollection<IDocumentManager> list)
        {
            if (watchingList.ContainsKey(list))
                return;

            watchingList.Add(list, item);
            list.CollectionChanged += OnDocumentManagerCollectionChanged;
        }

        void ClearWatchingLists()
        {
            foreach (var watched in watchingList.Keys)
                watched.CollectionChanged -= OnDocumentManagerCollectionChanged;
            watchingList.Clear();
            foreach (var folder in folderNodes.Keys)
            {
                folder.newCommandEvent -= OnFolderNewCommandEvent;
                folder.deleteCommandEvent -= OnFolderDeleteEvent;
                folder.renameFolderCommandEvent -= OnFolderRenameEvent;
                folder.newFolderCommandEvent -= OnNewFolderCommandEvent;
            }
            folderNodes.Clear();
        }

        private static ObservableCollection<IDocumentManager> GetChildList(TreeListNode item, UFProjectDocument document, IDocumentManager doc, UFProjectDocument mainDocument)
        {
            var list = doc.GetChildDocumentManagers();
            if (list == null)
            {
                var uri = new Uri(document.ProjectFolder, UriKind.RelativeOrAbsolute);
                if (item.Tag is Uri)
                    uri = item.Tag as Uri;
                else
                {
                    var parentUri = item.ParentNode as TreeListNode;
                    while (parentUri != null)
                    {
                        if (parentUri.Tag is Uri)
                        {
                            if (!((GetNearestDocument(parentUri)?.Tag as UFProjectDocument) ?? mainDocument).ListChildProjectPaths.Contains((Uri)parentUri.Tag))
                                uri = parentUri.Tag as Uri;
                            break;
                        }
                        parentUri = parentUri.ParentNode as TreeListNode;
                    }
                }

                try
                {
                    list = doc.GetChildDocumentManagers(uri, document);
                }
                catch (Exception ex)
                {

                }
            }

            return list;
        }

        TreeListNode GetNearestRoot(TreeListNode activeNode = null)
        {
            if (activeNode == null)
                activeNode = treeListControl.GetSelectedNodes().FirstOrDefault();

            var firstNode = treeListView.Nodes.First();

            if (activeNode == null)
                return firstNode;

            return GetNearestDocument(activeNode) ?? firstNode;
        }

        static TreeListNode GetNearestDocument(TreeListNode node)
        {
            var parentNode = node.ParentNode;
            while (parentNode != null)
            {
                if (parentNode.Tag is UFProjectDocument)
                    return parentNode;
                parentNode = parentNode.ParentNode;
            }
            return null;
        }

        public UFProjectExplorerUI(UFProjectManagerComponent c, UFProjectDocument doc, List<GeneralCommand> menuCommands = null)
        {
            InitializeComponent();

            Loaded += (o, e) =>
                {
                    if (bLoaded)
                        return;

                    bLoaded = true;

                    LoadData();

#if !DEBUG
                    webDeployEnabled = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxvQ+f/dCo/tfcUQt0VSH97CjTYKwLVt2qrQp2IkW/O4s="/* WDEP */);
                    geolocal = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx+myJYPxeGnrgbhGui/FFig=="/* GEO */);
                    RegistryKeysHelper.ReadLogException(ref bLogExceptions);
#endif
                    
                    if (!String.IsNullOrEmpty(Properties.Settings.Default.AutoLoadEditor))
                    {
                        var found = (from x in listDocumentManagers
                                     where x.TypeLabel == Properties.Settings.Default.AutoLoadEditor select x).ToList();
                        if (found.Count > 0)
                        {
                            using (var cursor = new WaitCursor())
                            {
                                var uri = new Uri(doc.ProjectFolder, UriKind.RelativeOrAbsolute);
                                projectManagerComponent.Workspace.IsBusy = true;
                                try
                                {
                                    found[0].Edit(uri, doc);
                                }
                                catch (Exception ex)
                                {
                                    projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, uri, ex.Message));
                                }

                                //if (!listDocumentManagers.Contains(docManager))
                                //    listDocumentManagers.Add(docManager);

                                projectManagerComponent.Workspace.IsBusy = false;
                            }
                        }
                    }
                };
            Unloaded += (o, e) =>
                {
                    bLoaded = false;
                };

            projectManagerComponent = c;

            if (doc != null)
            {
                if (menuCommands != null)
                {
                    menuCommands.ForEach(mc =>
                    {
                        var cb = new CommandBinding(mc, OnAddNewExecuted, OnAddNewCanExecute);
                        CommandBindings.Add(cb);
                    });
                }
            }

            openFolderImg = UFProjectManagerComponent.GetControlImage("OpenFolderSmall", true);
            closedFolderImg = UFProjectManagerComponent.GetControlImage("CloseFolderSmall", true);

            InitializeDocumentTree(doc);
        }
        bool IsResourceTypeEnabled(UFProjectDocument doc, string resourceType)
        {
            var visibleHMIDocumentManagers = WebHMIDesignHelper.WebHMIHelper.VisibleHMIDocumentManagers;
            bool isWebHMIProject = !string.IsNullOrEmpty(doc.ProjectType) && doc.ProjectType == ProjectType.WebHMI.ToString();
            return (!isWebHMIProject || (isWebHMIProject && !string.IsNullOrEmpty(resourceType) && visibleHMIDocumentManagers.Contains(resourceType)));
        }
        void InitializeDocumentTree(UFProjectDocument doc, TreeListNode parentNode = null)
        {
            if (bDisposed)
                return;

            using (var cursor = new WaitCursor())
            {
                if (Document == null)
                    Document = doc;

                //treeListView.Nodes.Clear();
                var actRoot = treeListControl.AddNode(new TreeItemControl(doc.Title, UFProjectManagerComponent.GetControlImage("UFPRJEditorSmall")), parentNode, doc);

                treeListControl.PreviewMouseDoubleClick += (ob, ev) =>
                {
                    if (!(ev.OriginalSource is System.IO.Path))
                    {
                        var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
                        if (selected == null || selected.Tag is Uri)
                            ev.Handled = true;
                    }
                };

                AddTreeMainComponents(actRoot);

                if (actRoot != null)
                {
                    actRoot.IsExpanded = true;
                    doc.PropertyChanged += (ob, ev) =>
                    {
                        if (ev.PropertyName == "NeedsSave")
                        {
                            treeListControl.Dispatcher.InvokeIfRequired(() =>
                            {
                                if (doc.NeedsSave)
                                    (actRoot.Content as TreeItemControl).ItemHeader = doc.Title + "*";
                                else
                                    (actRoot.Content as TreeItemControl).ItemHeader = doc.Title;
                            });
                        }
                    };

                    if (doc.Parent is UFProjectDocument)
                    {
                        var docParent = doc.Parent as UFProjectDocument;
                        var controller = docParent.GetChildProjectData(new Uri(doc.ProjectPath, UriKind.RelativeOrAbsolute));
                        controller.PropertyChanged += (ob, ev) =>
                        {
                            if (ev.PropertyName == "ProjectName")
                            {
                                treeListControl.Dispatcher.InvokeIfRequired(() =>
                                {
                                    (actRoot.Content as TreeItemControl).ItemHeader = doc.Title;
                                });
                            }
                        };
                    }
                }
            }
        }

        void AddTreeMainComponents(TreeListNode rootNode)
        {
            var doc = rootNode.Tag as UFProjectDocument;
            if (bDisposed || doc == null)
                return;



            var list = doc.ResourcTypes;
            list.Sort();
            list.ForEach(type =>
            {
                var docManager = doc.GetResourceDocumentManager(type);
                TreeListNode addedNode = null;
                if (IsResourceTypeEnabled(doc, type))
                {
                    if (docManager.isMultipleResource)
                    {
                        var folder = doc.GetResourceFolderWatcher(type);
                        addedNode = AddTreeItem(folder, rootNode);
                    }
                    else
                    {
                        addedNode = AddTreeItem(docManager, rootNode);
                    }
                }
                if (addedNode != null) {
                    if (!mapMainComponentsRoot.ContainsKey(doc))
                        mapMainComponentsRoot.Add(doc, new Dictionary<string, TreeListNode>());
                    mapMainComponentsRoot[(UFProjectDocument)rootNode.Tag][docManager.TypeScheme] = addedNode;
                }
            });
        }

        void treeListControl_SelectionChanged(object sender, TreeListSelectionChangedEventArgs e)
        {
            OnActivate();

            if (!expanderDetails.IsExpanded)
                return;

            UpdateSelectionDetailsGridSource();

            e.Handled = true;
        }

        private void UpdateSelectionDetailsGridSource()
        {
            UnsubscribeDetailsCollectionChanged();
            var listSelected = treeListControl.GetSelectedNodes();
            foreach (TreeListNode newitem in listSelected)
            {
                if (newitem.Tag is IDocumentManager)
                {
                    var docManager = newitem.Tag as IDocumentManager;

                    if (docManager.IsResourceExpandable(Document))
                    {
                        detailsGridCollection = GetChildList(newitem, Document, docManager, Document);
                        SubscribeDetailsCollectionChanged();
                        break;
                    }
                }
            }
        }

        void SubscribeDetailsCollectionChanged()
        {
            if (detailsGridCollection != null)
            {
                gridControl.ItemsSource = new ObservableCollection<IDocumentManager>(detailsGridCollection);
                detailsGridCollection.CollectionChanged += OnDetailsCollectionChanged;
            }
        }

        void UnsubscribeDetailsCollectionChanged()
        {
            if (detailsGridCollection != null)
            {
                gridControl.ItemsSource = null;
                detailsGridCollection.CollectionChanged -= OnDetailsCollectionChanged;
            }
        }

        void OnDetailsCollectionChanged(object o, System.Collections.Specialized.NotifyCollectionChangedEventArgs ev)
        {
            Dispatcher.InvokeIfRequired(() =>
            {
                var items = gridControl.ItemsSource as IList<IDocumentManager>;
                if (items != null)
                {
                    try
                    {
                        gridControl.BeginDataUpdate();
                        if (ev.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                        {
                            items.Clear();
                        }
                        else
                        {
                            if (ev.NewItems != null && ev.NewItems.Count != 0)
                            {
                                foreach (IDocumentManager dvm in ev.NewItems)
                                    items.Add(dvm);
                            }

                            if (ev.OldItems != null && ev.OldItems.Count != 0)
                            {
                                foreach (IDocumentManager dvm in ev.OldItems)
                                    items.Remove(dvm);
                            }
                        }
                    }
                    finally
                    {
                        gridControl.EndDataUpdate();
                    }
                }
            });
        }

        internal void OnActivate()
        {
            var list = new List<Object>();

            var listSelected = treeListControl.GetSelectedNodes();
            foreach (TreeListNode newitem in listSelected)
            {
                if (newitem.Tag is Uri)
                {
                    var uri = newitem.Tag as Uri;
                    if (Document != null)
                    {
                        var docManager = Document.GetResourceDocumentManager(uri);
                        if (docManager != null)
                        {
                            if (docManager.IsStartupControllerAware)
                            {
                                bool isChildProjectUri = ((GetNearestDocument(newitem.ParentNode)?.Tag as UFProjectDocument) ?? Document).ListChildProjectPaths.Contains(uri);
                                if (isChildProjectUri)
                                {
                                    var controller = (GetNearestDocument(newitem.ParentNode).Tag as UFProjectDocument).GetChildProjectData(uri, true);
                                    list.Add(controller);
                                }
                                else
                                    list.Add(Document.GetControllerData(uri, true));
                            }
                            else
                            {
                                try
                                {
                                    docManager.GetChildDocumentManagers(uri, (GetNearestRoot(newitem).Tag as UFProjectDocument) ?? Document);
                                    var doc = docManager.GetDocument(uri);
                                    if (doc != null)
                                        list.Add(doc);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                    }
                }
                else if (newitem.Tag is ResourceFolderWatcher)
                {
                    var folder = newitem.Tag as ResourceFolderWatcher;
                    if (Document != null)
                    {
                        var docManager = Document.GetResourceDocumentManager(folder.Scheme);
                        if (docManager != null && docManager.IsStartupControllerAware)
                        {
                            var path = Document.GetResourceFolderWatcher(folder.Scheme);
                            var uri = Document.MakeRelativeUri(new Uri(folder.Path, UriKind.RelativeOrAbsolute), path.Path);
                            if (!String.IsNullOrEmpty(uri.OriginalString))
                                list.Add(Document.GetControllerData(uri, true));
                        }
                    }
                }
                else if (newitem.Tag is IDocumentManager)
                {
                    var docManager = newitem.Tag as IDocumentManager;
                    if (docManager.BrowsableContent != null)
                        list.Add(docManager.BrowsableContent);
                }
                else
                    list.Add(newitem.Tag);
            }

            projectManagerComponent.Workspace.AddBarManagerGlobalCommands(CommandBindings);

            if (list.Count > 1)
                projectManagerComponent.Workspace.ContextObjects = list;
            else if (list.Count == 1)
                projectManagerComponent.Workspace.ContextObject = list[0];
            else
                projectManagerComponent.Workspace.ContextObject = null;

            if (!bDisposed && bLoaded)
            {
                if (detailsTableView.IsKeyboardFocusWithin && !detailsTableView.IsFocused)
                    detailsTableView.Focus();
                else if (!treeListView.AllowEditing && !treeListView.IsFocused)
                    treeListView.Focus();
            }
        }

        #region Tree Control Mng

        private static bool CanBeExpanded(TreeListNode parent)
        {
            return parent.Nodes.Count == 1 && parent.Nodes[0].Tag == TreeListControlHelper.DummyNode;
        }

        private void AddSubItemsList(TreeListNode item, ObservableCollection<IDocumentManager> list)
        {
            if (list == null)
                return;

            foreach (var child in list)
                AddTreeItem(child, item);
            WatchListChanges(item, list);
        }

        bool bSelectCurrent;
        List<IDocumentManager> listDocumentManagers
        {
            get
            {
                return projectManagerComponent.UriRisolver.GetListInstalledDocumentManagers();
            }
        }

        void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            treeListControl.View.ActiveEditor?.SelectAll();

            var node = treeListControl.View.GetNodeByRowHandle(e.HitInfo.RowHandle);
            if (node == null)
                return;

            treeListControl.SelectNode(node);

            var document = GetNearestRoot(node).Tag as UFProjectDocument;
            if (node.Tag is ResourceFolderWatcher)
                node.IsExpanded = true;
            if (node.Tag is IDocumentManager)
            {
                OpenResource(node);
                e.Handled = true;
            }
            else if (node.Tag is Uri && !((GetNearestDocument(node.ParentNode)?.Tag as UFProjectDocument) ?? document).ListChildProjectPaths.Contains(node.Tag as Uri))
            {
                var uri = node.Tag as Uri;
                var model = new UriModel(uri);
                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    using (var cursor = new WaitCursor())
                    {
                        e.Handled = true;
                        try
                        {
                            var ret = document.EditResource(uri);

                            //if (!listDocumentManagers.Contains(ret))
                            //    listDocumentManagers.Add(ret);
                        }
                        catch (Exception ex)
                        {
                            projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, uri, ex.Message));
                        }
                    }
                });

                e.Handled = true;
            }
            else if (node.Tag is ResourceFolderWatcher && document != null)
            {
                var folder = node.Tag as ResourceFolderWatcher;
                var docManager = document.GetResourceDocumentManager(folder.Scheme);
                if (docManager != null && docManager.isServiceResource)
                {
                    var uri = new Uri(document.ProjectFolder, UriKind.RelativeOrAbsolute);
                    docManager.Edit(uri, document);
                }
            }
        }

        void OpenResource(TreeListNode treeNode = null)
        {
            List<TreeListNode> nodes;
            if (treeNode == null)
                nodes = treeListControl.GetSelectedNodes()?.ToList();
            else
                nodes = new List<TreeListNode>() { treeNode };

            if (bDisposed || nodes.Count == 0)
                return;

            foreach (var node in nodes)
            {
                var docManager = node.Tag as IDocumentManager;
                if (docManager != null)
                {
                    var document = GetNearestRoot(node).Tag as UFProjectDocument;
                    node.IsExpanded = true;
                    if (docManager.TypeScheme != projectManagerComponent.TypeScheme)
                    {
                        Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            using (var cursor = new WaitCursor())
                            {
                                var uri = new Uri(document.ProjectFolder, UriKind.RelativeOrAbsolute);
                                var parentUri = node.ParentNode;
                                while (parentUri != null)
                                {
                                    if (parentUri.Tag is Uri)
                                    {
                                        if (!((GetNearestDocument(parentUri)?.Tag as UFProjectDocument) ?? Document).ListChildProjectPaths.Contains((Uri)parentUri.Tag))
                                            uri = parentUri.Tag as Uri;
                                        break;
                                    }
                                    parentUri = parentUri.ParentNode;
                                }
                                projectManagerComponent.Workspace.IsBusy = true;
                                try
                                {
                                    docManager.Edit(uri, document);
                                }
                                catch (Exception ex)
                                {
                                    projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, uri, ex.Message));
                                }

                            //if (!listDocumentManagers.Contains(docManager))
                            //    listDocumentManagers.Add(docManager);

                            projectManagerComponent.Workspace.IsBusy = false;
                            }
                        });
                    }
                }
                var uriModel = (node.Content as TreeItemControl).InnerControl?.DataContext as UriModel;
                if (uriModel != null)
                    uriModel.OpenCommand.Execute(null);
            }
        }

        void OnTreeHeaderValidate(object sender, TreeListCellValidationEventArgs e)
        {
            try
            {
                var node = e.Node;
                var oldStr = (node.Content as TreeItemControl).ItemHeader as String;
                var newStr = e.Value as String;

                if (node.Tag is ResourceFolderWatcher)
                {
                    var folder = node.Tag as ResourceFolderWatcher;
                    String invalidChars;
                    String checkedNewName;
                    checkedNewName = RemoveInvalidPathCharacters(newStr, out invalidChars);

                    if (checkedNewName != newStr)
                    {
                        e.IsValid = false;
                        projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorRenameCannotUseSpecialChars, newStr, invalidChars));
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(newStr) && String.Compare(newStr, oldStr, false) != 0)
                        {
                            bSelectCurrent = true;
                            if (!folder.RenameFolder(oldStr, newStr))
                                e.IsValid = false;
                            else
                            {
                                bSelectCurrent = false;

                                var oldpath = folder.Path;
                                var newpath = oldpath.Replace("\\" + oldStr, "\\" + newStr);
                                var oldUri = new Uri(oldpath, UriKind.RelativeOrAbsolute);
                            	var newUri = new Uri(newpath, UriKind.RelativeOrAbsolute);
                                Document.CopyControllerData(oldUri, newUri, Document, isFolder: true);
                                Document.RemoveControllerData(oldUri, isFolder: true);

                                Document.AddRenamedFolder(folder.Scheme, oldUri, newUri);
                                (node.Content as TreeItemControl).ItemHeader = newStr;
                            }
                        }
                    }
                }
                else if (node.Tag is Uri)
                {
                    var uri = node.Tag as Uri;

                    bSelectCurrent = true;
                    if (!String.IsNullOrEmpty(newStr) && newStr != oldStr)
                    {
                        string invalidChars;
                        var checkPath = RemoveInvalidFileNameCharacters(newStr, out invalidChars);
                        if (checkPath != newStr)
                        {
                            e.IsValid = false;
                            projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorRenameCannotUseSpecialChars, newStr, invalidChars));
                        }
                        else
                        {
                            bSelectCurrent = true;
                            if (!Document.RenameResource(uri, oldStr, newStr))
                            {
                                e.IsValid = false;
                                projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorOccuredRenaming, newStr));
                            }
                            else
                            {
                                bSelectCurrent = false;

                                var folder = System.IO.Path.GetDirectoryName(uri.GetPathString());
                                var newPathName = String.Format("{0}\\{1}{2}", folder, newStr, System.IO.Path.GetExtension(uri.GetPathString()));
                            	var newUri = new Uri(newPathName, UriKind.RelativeOrAbsolute);
                                Document.CopyControllerData(uri, newUri, Document);
                                Document.RemoveControllerData(uri);

                                Document.AddRenamedResource(uri, newUri);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (dpActivateExplorer == null ||
                dpActivateExplorer.Status == DispatcherOperationStatus.Completed ||
                dpActivateExplorer.Status == DispatcherOperationStatus.Aborted)
                    dpActivateExplorer = Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
                    {
                        if (projectManagerComponent.Workspace.ActiveWindow != this)
                        {
                            projectManagerComponent.Workspace.ActivateDockedElement(this);
                            if (!treeListView.AllowEditing && !treeListView.IsFocused)
                                treeListView.Focus();
                        }
                    });
            }
        }

        void OnHiddenEditor(object sender, TreeListEditorEventArgs e)
        {
            treeListControl.DisableEditing();
        }

        //https://www.devexpress.com/Support/Center/Question/Details/T444753/treelist-arrow-keys-not-working-when-in-the-editor-the-text-is-selected-all
        private void View_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right || e.Key == Key.Left)
            {
                var activeTextEditor = treeListView.ActiveEditor as TextEdit;
                if (activeTextEditor != null && activeTextEditor.SelectionLength == activeTextEditor.Text.Length)
                {
                    activeTextEditor.CaretIndex = e.Key == Key.Left ? 0 : activeTextEditor.SelectionLength;
                    e.Handled = true;
                }
            }
        }

        internal void RestoreComponents()
        {
            var rootNode = treeListView.Nodes.First();
            treeListControl.BeginDataUpdate();
            ClearNodes(rootNode);
            mapObjectToNode.Clear();
            ClearWatchingLists();
            AddTreeMainComponents(rootNode);
            treeListControl.EndDataUpdate();
            projectManagerComponent.Workspace.ContextObject = null;
            rootNode.IsExpanded = true;
            OnActivate();
        }

        internal void HideComponents()
        {
            if (!bLoaded)
                return;

            var keys = (from key in mapObjectToNode.Keys where IsComponentHidden(key) select key).ToList();
            foreach (var key in keys)
            {
                var node = mapObjectToNode[key];
                ClearNodes(node);
                mapObjectToNode.Remove(key);
                node.ParentNode.Nodes.Remove(node);
                treeListView.Nodes.Remove(node);
            }
        }

        bool IsComponentHidden(object tag)
        {
            string componentName = null;
            if (tag is IDocumentManager)
                componentName = (tag as IDocumentManager).TypeScheme;
            if (tag is ResourceFolderWatcher)
                componentName = (tag as ResourceFolderWatcher).Scheme;

            return projectManagerComponent.Workspace.IsComponentHidden(componentName);
        }

        private TreeListNode AddTreeItem(Object tag, TreeListNode parent)
        {
            if (bDisposed || IsComponentHidden(tag))
                return null;
            
            var ic = new TreeItemControl(tag);
            var newitem = treeListControl.AddNode(ic, parent, tag);
            var document = GetNearestRoot(newitem).Tag as UFProjectDocument;

            if (tag is IDocumentManager)
            {
                var docManager = tag as IDocumentManager;
                var itemContent = newitem.Content as TreeItemControl;
                itemContent.IsOpenable = true;
                if (docManager.TypeScheme == projectManagerComponent.TypeScheme)
                {
                    var childControl = new ChildProjectControl() { IsTitleVisible = false };
                    childControl.btnAddChildProject_ContextMenu.ItemClick += (o, e) =>
                    {
                        var filter = projectManagerComponent.UriRisolver.GetOpenFileFilter();
                        var doc = projectManagerComponent.Workspace.ContextDocument as IDocument;
                        IUIMsgBoxAlertService uIMsgBoxAlertService = null;
                        IHelpProvider helpProvider = null;
                        if (doc != null)
                        {
                            uIMsgBoxAlertService = doc.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                            helpProvider = doc.GetService(typeof(IHelpProvider)) as IHelpProvider;
                        }
                        var fileType = new CommonControls.SelectFileType(true, filter, null, uIMsgBoxAlertService, helpProvider);
                        var newDialog = new GeneralDialogContent(fileType)
                        {
                            Owner = this.FindParent<Window>(),
                            HelpLink = "OpenProject",
                        };
                        if (newDialog.ShowDialog() == true)
                        {
                            String fileName = fileType.currentUri;
                            // var fileName = projectManagerComponent.UIInterface.ShowOpenFileDialog(projectManagerComponent.UriRisolver.GetOpenFileFilter());
                            if (!(String.IsNullOrEmpty(fileName)) && fileName != document.ProjectPath)
                            {
                                Uri _uri = null;
                                if (XpoHelpers.XpoHelper.IsDataSource(fileName))
                                    _uri = new Uri(String.Format("{0}:{1}", projectManagerComponent.UriRisolver.GetOpenFileScheme(), fileName), UriKind.RelativeOrAbsolute);
                                else
                                    _uri = new Uri(fileName);

                                var ret = document.MakeRelativeUri(_uri);
                                ret = ret.GetUrlDecodedUri();
                                if (!((GetNearestDocument(newitem.ParentNode)?.Tag as UFProjectDocument) ?? document).ListChildProjectPaths.Contains(ret))
                                {
                                    document.AddChildProject(ret);
                                    if (newitem.IsExpanded)
                                        AddTreeItem(ret, newitem);
                                    else
                                    {
                                        if (newitem.Nodes.Count == 0)
                                            treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);
                                        newitem.IsExpanded = true;
                                    }
                                }

                                treeListControl.ClearSelection();

                                var i = (from p in newitem.Nodes
                                         where p.Tag as Uri == ret
                                         select p).ToList();
                                i.ForEach(j =>
                                {
                                    treeListControl.SelectNode(j);
                                });
                            }
                        }
                    };

                    itemContent.ItemHeader = childControl.Title.Text;
                    itemContent.InnerControl = childControl;
                    //newitem.ContextMenu = docManager.TypeContextMenu;
                }
                else
                    itemContent.ItemHeader = docManager.TypeTitle;

                if (docManager.TypeScheme == projectManagerComponent.TypeScheme)
                    itemContent.ResourceIcon = UFProjectManagerComponent.GetControlImage("UFPRJChildProject");
                else
                    itemContent.ResourceIcon = docManager.TypeIcon;

                if (docManager.BrowsableContent is INotifyPropertyChanged)
                {
                    var changeNotifier = docManager.BrowsableContent as INotifyPropertyChanged;
                    if (!mapSubscribedItems.ContainsKey(changeNotifier))
                    {
                        changeNotifier.PropertyChanged += ChangeNotifier_PropertyChanged;
                        mapSubscribedItems.Add(changeNotifier, 
                            new Tuple<IDocumentManager, TreeItemControl>(docManager, newitem.Content as TreeItemControl));
                    }
                }

                if (docManager.IsResourceExpandable(document))
                {
                    //var list = GetChildList(newitem, document, docManager);
                    //if (list != null)
                    //{
                    //    if (list.Count > 0)
                            treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);
                    //    WatchListChanges(newitem, list);
                    //}
                }
                else
                {
                    var list = GetChildList(newitem, document, docManager, Document);
                    if (list != null)
                        WatchListChanges(newitem, list);
                }

                //newitem.IsEditable = false;
            }
            else if (tag is Uri)
            {
                var uri = tag as Uri;
                var model = new UriModel(uri);

                bool isChildProjectUri = ((GetNearestDocument(newitem.ParentNode)?.Tag as UFProjectDocument) ?? document).ListChildProjectPaths.Contains(uri);
                model.openCommand += (o, e) =>
                {
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        using (var cursor = new WaitCursor())
                        {
                            projectManagerComponent.Workspace.IsBusy = true;
                            try
                            {
                                var ret = document.EditResource(uri);
                            }
                            catch (Exception ex)
                            {
                                projectManagerComponent.Workspace.IsBusy = false;
                                projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, uri, ex.Message));
                            }

                            projectManagerComponent.Workspace.IsBusy = false;
                        }
                    });
                };
                model.deleteCommand += (o, e) =>
                {
                    if (isChildProjectUri)
                    {
                        var text = String.Format(Properties.Resources.RemoveChildProject,
                            model.Title);
                        var res = projectManagerComponent.UIInterface.ShowOkCancel(text, CustomDialogIcons.Warning);
                        if (res == CustomDialogResults.OK)
                        {
                            var abs = uri;
                            var relative = uri.GetPathString();
                            if (!XpoHelpers.XpoHelper.IsDataSource(relative))
                            {
                                var match = String.Format("{0}/", document.Title);
                                if (relative.StartsWith(match))
                                    relative = relative.Replace(match, "");
                                else
                                {
                                    match = String.Format("{0}\\", document.Title);
                                    if (relative.StartsWith(match))
                                        relative = relative.Replace(match, "");
                                }
                                abs = document.MakeAbosoluteUri(new Uri(relative, UriKind.RelativeOrAbsolute));
                            }

                            bool ret = true;
                            var d = projectManagerComponent.GetOpenDocument(abs) as UFProjectDocument;
                            if (d != null)
                            {
                                if (!CloseAllChildProjects(d))
                                    ret = false;
                                else
                                    projectManagerComponent.CloseProject(d);
                            }

                            if (ret)
                            {
                                treeListControl.ClearSelection();
                                document.RemoveChildProject(uri);
                                parent.Nodes.Remove(newitem);
                            }
                        }
                    }
                    else
                    {
                        var text = String.Format(Properties.Resources.DeleteResource,
                            System.IO.Path.GetFileNameWithoutExtension(uri.GetPathString()));
                        var res = projectManagerComponent.UIInterface.ShowOkCancel(text, CustomDialogIcons.Warning);
                        if (res == CustomDialogResults.OK)
                        {
                            try
                            {
                                document.DeleteResource(uri);
                            }
                            catch (Exception ex)
                            {
                                projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorDeletingDocument, uri, ex.Message));
                            }
                        }
                    }
                };
                if (!isChildProjectUri)
                {
                    model.renameCommand += (o, e) =>
                    {
                        treeListControl.EditNode(newitem);
                    };
                }

                var header = new ResourceTreeControl() { DataContext = model, IsTitleVisible = false };
                (newitem.Content as TreeItemControl).ItemHeader = model.Title;
                (newitem.Content as TreeItemControl).ResourceIcon = document.GetResourceImage(uri);
                (newitem.Content as TreeItemControl).InnerControl = header;
                (newitem.Content as TreeItemControl).IsOpenable = true;
                if (isChildProjectUri)
                    treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);
                else if (document.GetResourceDocumentManager(uri).IsResourceExpandable(document))
                {
                    var list = GetChildList(newitem, document, document.GetResourceDocumentManager(uri), Document);
                    if (list != null && list.Count > 0)
                        treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);
                }
                if (isChildProjectUri)
                {
                    header.btnOpen.Visibility = Visibility.Collapsed;
                    header.btnRename.Visibility = Visibility.Collapsed;
                }
                //newitem.IsEditable = false;
            }
            else if (tag is ResourceFolderWatcher)
            {
                var folder = tag as ResourceFolderWatcher;
                var header = new FolderTreeControl() { DataContext = folder, IsTitleVisible = false };
                (newitem.Content as TreeItemControl).ItemHeader = (tag as ResourceFolderWatcher).Title;
                (newitem.Content as TreeItemControl).InnerControl = header;

                var docManager = document.GetResourceDocumentManager(folder.Scheme);
                if (!folder.IsRoot)
                    (newitem.Content as TreeItemControl).ResourceIcon = closedFolderImg;
                else
                    (newitem.Content as TreeItemControl).ResourceIcon = docManager.TypeIcon;

                bCreatingFolderResource = false;

                if (folder.ListFolders.Count > 0 || folder.ListResources.Count > 0)
                    treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);

                if (!folderNodes.ContainsKey(folder))
                {
                    folderNodes[folder] = newitem;
                    folder.newCommandEvent += OnFolderNewCommandEvent;
                    folder.deleteCommandEvent += OnFolderDeleteEvent;
                    folder.renameFolderCommandEvent += OnFolderRenameEvent;
                    folder.newFolderCommandEvent += OnNewFolderCommandEvent;
                }
            }

            if (bSelectCurrent)
            {
                treeListControl.SelectNode(newitem);
                bSelectCurrent = false;
            }
            if (newitem != null)
                treeListControl.RefreshRow(newitem.RowHandle); //Otherwise node's header (ItemHeader) can disappear in certain conditions

            return newitem;
        }

        void OnFolderNewCommandEvent(object sender, EventArgs e)
        {
            if (!bCreatingFolderResource)
            {
                bCreatingFolderResource = true;
                var folder = sender as ResourceFolderWatcher;
                if (!folderNodes.ContainsKey(folder))
                    return;
                var node = folderNodes[folder];
                var document = GetNearestRoot(node).Tag as UFProjectDocument;
                while (true)
                {
                    try
                    {
                        var ret = document.CreateNewResource(folder.Scheme, String.Format("{0}\\", folder.Path));
                        if (ret != null)
                        {
                            if (!CanBeExpanded(node) && node.Nodes.Count == 0)
                                treeListControl.AddNode(null, node, TreeListControlHelper.DummyNode);

                            node.IsExpanded = true;

                            treeListControl.ClearSelection();

                            var i = (from p in node.Nodes
                                     where p.Tag as Uri == ret
                                     select p).ToList();
                            i.ForEach(j =>
                            {
                                treeListControl.SelectNode(j);
                            });
                        }
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (projectManagerComponent.UIInterface != null)
                            projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorCreatingNewResource, ex.Message));
                        continue;
                    }
                    finally
                    {
                    }
                }
                bCreatingFolderResource = false;
            }
        }

        void OnFolderDeleteEvent(object sender, EventArgs e)
        {
            var folder = sender as ResourceFolderWatcher;
            if (!folderNodes.ContainsKey(folder))
                return;
            var node = folderNodes[folder];
            var document = GetNearestRoot(node).Tag as UFProjectDocument;
            var parent = node.ParentNode;

            bool bDelete = true;
            if (folder.ListResources.Count != 0 || folder.ListFolders.Count != 0)
            {
                //var res = projectManagerComponent.UIInterface.ShowOkCancel(Properties.Resources.DeleteFolderNotEmpy,
                //    CustomDialogIcons.Warning);
                //if (res != CustomDialogResults.OK)
                //    bDelete = false;
                //else
                {
                    foreach (var manager in listDocumentManagers)
                    {
                        if (!manager.CloseAllChild(document))
                        {
                            bDelete = false;
                            break;
                        }
                    }
                }
            }

            if (bDelete)
            {
                node.IsExpanded = false;
                try
                {
                    folder.DeleteFolder(folder.Path);
                    
                    if (parent != null)
                    {
                        if (parent.Nodes.Contains(node))
                            parent.Nodes.Remove(node);
                    }
                    else
                    {
                        if (treeListControl.View.Nodes.Contains(node))
                            treeListControl.View.Nodes.Remove(node);
                    }
                }
                catch (Exception ex)
                {
                    if (projectManagerComponent.UIInterface != null)
                        projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorDeletingFolder, ex.Message));
                }
            }
        }

        void OnFolderRenameEvent(object sender, EventArgs e)
        {
            var folder = sender as ResourceFolderWatcher;
            if (!folderNodes.ContainsKey(folder))
                return;

            if (!folder.IsRoot)
                treeListControl.EditNode(folderNodes[folder]);
        }

        void OnNewFolderCommandEvent(object sender, EventArgs e)
        {
            var nfc = new NewFolderControl();

            var folder = sender as ResourceFolderWatcher;
            if (!folderNodes.ContainsKey(folder))
                return;
            var node = folderNodes[folder];

            while (true)
            {
                var newFolderDialog = new GeneralDialogContent(nfc, GeneralDialogButtons.OkCancelButtons)
                {
                    Title = Properties.Resources.NewFolderDialogTitle,
                    Owner = this.FindParent<Window>(),
                    HelpLink = ""
                };
                if (newFolderDialog.ShowDialog() == true)
                {
                    String invalidChars;
                    var checkPath = RemoveInvalidPathCharacters(nfc.folderName.Text, out invalidChars);
                    if (checkPath != nfc.folderName.Text || String.IsNullOrEmpty(nfc.folderName.Text))
                    {
                        if (projectManagerComponent.UIInterface != null)
                            projectManagerComponent.UIInterface.ShowError(Properties.Resources.InvalidFolderName);
                        continue;
                    }
                    treeListControl.ClearSelection();

                    bSelectCurrent = true;
                    var folderName = String.Format("{0}\\{1}", folder.Path, nfc.folderName.Text);
                    try
                    {
                        folder.CreateFolder(folderName);
                        if (!CanBeExpanded(node) && node.Nodes.Count == 0)
                            treeListControl.AddNode(null, node, TreeListControlHelper.DummyNode);

                        node.IsExpanded = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (projectManagerComponent.UIInterface != null)
                            projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorCreatingNewFolder, ex.Message));
                    }
                }
                else
                    break;
            }
        }

        Dictionary<INotifyPropertyChanged, Tuple<IDocumentManager, TreeItemControl>> mapSubscribedItems = new Dictionary<INotifyPropertyChanged, Tuple<IDocumentManager, TreeItemControl>>();
        Dictionary<INotifyPropertyChanged, DispatcherOperation> mapDispatcherOperations = new Dictionary<INotifyPropertyChanged, DispatcherOperation>();
        private void ChangeNotifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var subscribed = sender as INotifyPropertyChanged;
            if (subscribed == null)
                return;

            if (!mapSubscribedItems.ContainsKey(subscribed))
                return;

            if (e.PropertyName == "UFUAAlarmDefinitionRef")
            {
                var innerObject = mapSubscribedItems[subscribed].Item2.TreeItemInnerObject;
                if (innerObject != null && mapObjectToNode.ContainsKey(innerObject))
                {
                    {
                        RefreshAsync(mapObjectToNode[innerObject].ParentNode);
                        if (mapDispatcherOperations.ContainsKey(subscribed))
                        {
                            mapDispatcherOperations[subscribed].Abort();
                            mapDispatcherOperations.Remove(subscribed);
                        }
                        return;
                    }
                }
            }

            DispatcherOperation dp = null;
            if (mapDispatcherOperations.ContainsKey(subscribed))
                dp = mapDispatcherOperations[subscribed];
            if (dp == null || dp.Status == DispatcherOperationStatus.Completed ||
                dp.Status == DispatcherOperationStatus.Aborted)
            {
                mapDispatcherOperations[subscribed] = Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    if (!bDisposed)
                    {
                        mapDispatcherOperations.Remove(subscribed);
                        mapSubscribedItems[subscribed].Item2.ItemHeader = mapSubscribedItems[subscribed].Item1.TypeTitle;
                        mapSubscribedItems[subscribed].Item2.ResourceIcon = mapSubscribedItems[subscribed].Item1.TypeIcon;
                    }
                });
            }
        }

        private void OnRefresh(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItem = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selectedItem == null)
                return;

            using (new WaitCursor(this))
            {
                if (selectedItem.WasExpanded() && !(selectedItem.Tag is UFProjectDocument))
                {
                    try
                    {
                        treeListControl.BeginDataUpdate();
                        Refresh(selectedItem);
                    }
                    finally
                    {
                        treeListControl.EndDataUpdate();
                    }
                }
            }
        }

        private void CanRefresh(object sender, CanExecuteRoutedEventArgs e)
        {
            var selectedItem = treeListControl.GetSelectedNodes().FirstOrDefault();
            e.CanExecute = IsActiveWindow && selectedItem != null && selectedItem.IsExpanded && (selectedItem.Tag is IDocumentManager || selectedItem.Tag is ResourceFolderWatcher);
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (treeListControl.View.AllowEditing)
                return;

            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.A && e.Key <= Key.Z) && !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                var selNode = treeListControl.GetSelectedNodes().FirstOrDefault();
                if (selNode == null)
                    selNode = GetNearestRoot();
                var searchedChar = e.Key.ToString().ToLower()[e.Key.ToString().Length - 1];
                treeListControl.BeginSelection();
                    treeListControl.SelectAll();
                    var nodes = treeListControl.GetSelectedNodes();
                    treeListControl.UnselectAll();
                    var firstFoundNode = (from TreeListNode node in nodes where node.RowHandle > selNode.RowHandle && (node.Content as TreeItemControl)?.ItemHeader != null && (node.Content as TreeItemControl).ItemHeader.ToString().ToLower()[0] == searchedChar select node).FirstOrDefault();
                    if (firstFoundNode != null)
                        treeListControl.SelectNode(firstFoundNode);
                    else
                        treeListControl.ClearSelection();
                treeListControl.EndSelection();
                return;
            }

            var selectedItem = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selectedItem == null)
                return;

            if (e.Key == Key.F5)
            {
                e.Handled = true;
                OnRefresh(null, null);
            }
            else if (e.Key == Key.F2)
            {
                if (selectedItem.Tag is Uri || 
                    (selectedItem.Tag is ResourceFolderWatcher && !(selectedItem.Tag as ResourceFolderWatcher).IsRoot))
                {
                    e.Handled = true;
                    treeListControl.EditNode(selectedItem);
                    //selectedItem.IsEditable = true;
                    //selectedItem.IsInEditMode = true;
                }
            }
            else if (e.Key == Key.Insert)
            {
                var parent = GetParentResourceFolderRoot(selectedItem);
                if (parent != null)
                {
                    var folder = parent.Tag as ResourceFolderWatcher;
                    try
                    {
                        var ret = Document.CreateNewResource(folder.Scheme, String.Format("{0}\\", folder.Path));
                        if (ret != null)
                        {
                            parent.IsExpanded = true;

                            //treeListControl.ClearSelection(); //ExplorerTree.ClearSelection();

                            //var i = (from p in parent.Nodes
                            //         where p.Tag as Uri == ret
                            //         select p).ToList();
                            //treeListControl.SelectNodes(i);
                            
                            //i.ForEach(j =>
                            //    {
                            //j.IsSelected = true;
                            //ExplorerTree.BringIntoView(j);
                            //});
                        }
                        var docManager = Document.GetResourceDocumentManager(folder.Scheme);
                    }
                    catch(Exception ex)
                    {
                        if (projectManagerComponent.UIInterface != null)
                            projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorCreatingNewResource, ex.Message));
                    }
                }
                //if (!listDocumentManagers.Contains(docManager))
                //    listDocumentManagers.Add(docManager);
            }
        }

        public TreeListNode GetParentResourceFolderRoot(TreeListNode parent)
        {
            if (parent.Tag is ResourceFolderWatcher)
                return parent;

            while (parent != null)
            {
                parent = parent.ParentNode as TreeListNode;
                if (parent != null && parent.Tag is ResourceFolderWatcher)
                    return parent;
            }

            return null;
        }

        #endregion
        
        #region Properties Members

        UFProjectDocument _Document;
        [Browsable(false)]
        public UFProjectDocument Document
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

        bool NeedsSave
        {
            get
            {
                if (Document == null)
                    return false;

                if (Document.NeedsSave || projectManagerComponent.IsAnyChildNeedsSave(Document))
                    return true;

                foreach (var docManager in listDocumentManagers)
                {
                    if (docManager.IsAnyChildNeedsSave(Document))
                        return true;
                }

                return false;
            }
        }

        #endregion

        #region Drag Drop

        private void TreeListControl_DragRecordOver(object sender, DragRecordOverEventArgs e)
        {
            var data = (RecordDragDropData)e.Data.GetData(typeof(RecordDragDropData));
            if (data.Records != null && data.Records.Count() > 0)
            {
                var node = treeListControl.View.GetNodeByContent(data.Records[0]);
                var parentNode = node?.ParentNode;
                if ((parentNode != null && parentNode.Content == e.TargetRecord) || (e.TargetRecord as TreeItemControl)?.InnerControl == null || (e.TargetRecord as TreeItemControl)?.InnerControl.GetType() == (node.Content as TreeItemControl)?.InnerControl.GetType())
                {
                    e.Effects = DragDropEffects.None;
                    return;
                }
                //var targetNode = ((TreeListView)e.OriginalSource).GetNodeByContent(e.TargetRecord);
                //var parentTargetNode = targetNode.ParentNode;
                //var parentRowItem = parentNode.Content;
                //var parentTargetRowItem = parentTargetNode.Content;
            }
        }

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

            bool bAllow = false;
            foreach (var dragItem in draggedNodes) //treeListControl.GetSelectedNodes()
            {
                if (dragItem.Tag is Uri)
                {
                    bool isChildProjectUri = ((GetNearestDocument(dragItem.ParentNode)?.Tag as UFProjectDocument) ?? Document).ListChildProjectPaths.Contains(dragItem.Tag as Uri);
                    bAllow = !isChildProjectUri;
                }
                else if (dragItem.Tag is IDocumentManager)
                {
                    var docManager = dragItem.Tag as IDocumentManager;
                    bAllow = docManager.CanBeDragged;
                }
                else
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
            var targetContent = (e.TargetRecord as TreeItemControl)?.TreeItemInnerObject;

            if (draggedNodes.Count() == 0 || targetContent == null || !mapObjectToNode.ContainsKey(targetContent))
                return;

            var item = mapObjectToNode[targetContent];
            TreeListNode folderNode = item;
            ResourceFolderWatcher folder = item.Tag as ResourceFolderWatcher;
            if (folder == null)
            {
                folderNode = GetParentFolderItem(item);
                folder = folderNode?.Tag as ResourceFolderWatcher;
                if (folder == null)
                    return;
            }

            bool bDropped = false;
            foreach (var dragItem in draggedNodes)
            {
                if (dragItem.Tag is Uri)
                {
                    var uri = dragItem.Tag as Uri;
                    var sourcefolder = GetParentFolder(dragItem);
                    if (sourcefolder == null || sourcefolder.Scheme != folder.Scheme || sourcefolder == folder)
                        break;
                    var docManager = Document.GetResourceDocumentManager(uri);

                    var newName = folder.CreateNewName(System.IO.Path.GetFileNameWithoutExtension(uri.OriginalString));
                    var folderName = folder.Path;
                    if (folderName.Length > 0 &&
                        folderName[folderName.Length - 1] != '\\' && folderName[folderName.Length - 1] != '/')
                        folderName += "\\";
                    var newPath = String.Format("{0}{1}{2}", folderName, newName,
                                    folder.FileType);

                    var bIsCopy = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

                    docManager.Copy(uri, newPath, bIsCopy, Document, false);
                    var newUri = new Uri(newPath, UriKind.RelativeOrAbsolute);
                    Document.CopyControllerData(uri, newUri, Document, isMovedItem: true);
                    if (!bIsCopy)
                    {
                        Document.RemoveControllerData(uri);
                    }
                    if (!bDropped)
                    {
                        bDropped = true;
                        Folder_AddDummy(folderNode);
                    }
                }
            }
            draggedNodes.Clear();
        }

        void TreeListControl_CompletedDragDrop(object sender, CompleteRecordDragDropEventArgs e)
        {
            e.Handled = true;
        }

        static ResourceFolderWatcher GetParentFolder(TreeListNode item)
        {
            if (item == null)
                return null;
            if (item.Tag is ResourceFolderWatcher)
                return item.Tag as ResourceFolderWatcher;
            item = item.ParentNode;
            if (item == null)
                return null;
            return item.Tag as ResourceFolderWatcher;
        }

        static TreeListNode GetParentFolderItem(TreeListNode item)
        {
            if (item.Tag is ResourceFolderWatcher)
                return item;

            item = item.ParentNode;
            if (item == null)
                return null;
            if (item.Tag is ResourceFolderWatcher)
                return item;
            return null;
        }
        #endregion

        #region Commands
        private void OnCreateWebApp(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            try
            {
                String arguments = String.Empty;
                
                if (projectManagerComponent.UserEditor != null)
                {
                    var bUserEnabled = projectManagerComponent.UserEditor.GetEnableUserManager(Document);
                    var AutoLogout = projectManagerComponent.UserEditor.GetAutoLogoutSeconds(Document, String.Empty);
                    var maxInvalidPasswordAttempts = projectManagerComponent.UserEditor.GetMaxInvalidPasswordAttempts(Document, bRefresh: true);

                    arguments = string.Format("/A\"{0}\" /P\"{1}\" /T\"{2}\" /L\"{3}\" /M\"{4}\" /E\"{5}\" /I\"{6}\"",
                        Document.Title, Document.ProjectPath, Document.Theme, AutoLogout, maxInvalidPasswordAttempts, bUserEnabled, Process.GetCurrentProcess().Id);

                    var sharedApplicationName = projectManagerComponent.UserEditor.GetSharedApplicationName(Document);
                    if (!String.IsNullOrEmpty(sharedApplicationName))
                        arguments = string.Format("{0} /N\"{1}\"", arguments, sharedApplicationName);

                    var users = projectManagerComponent.UserEditor.GetListUserNames(Document);
                    if(users != null && users.Any())
                        arguments = string.Format("{0} /U\"{1}\"", arguments, string.Join("|", users));
                    var roles = projectManagerComponent.UserEditor.GetListRoleNames(Document);
                    if (roles != null && roles.Any())
                        arguments = string.Format("{0} /R\"{1}\"", arguments, string.Join("|", roles));

                }
                else
                    arguments = string.Format("/A\"{0}\" /P\"{1}\" /T\"{2}\"", Document.Title, Document.ProjectPath, Document.Theme);

                if (projectManagerComponent.ReportManager != null)
                {
                    var reportManager = projectManagerComponent.ReportManager as IDocumentManager;
                    if (reportManager != null)
                    {
                        var reports = projectManagerComponent.GetResourceList(Document, reportManager.TypeScheme);
                        List<string> reportsName = new List<string>();
                        foreach (var report in reports)
                        {
                            if (Document.fileSystemProviderBase == null)
                            {
                                reportsName.Add(report.Replace("/", "\\")
                                                      .Replace($"{reportManager.TypeLabel}\\", "")
                                                      .Replace(reportManager.FileType, "")
                                                      .TrimStart('\\'));                                
                            }
                            else
                            {
                                    reportsName.Add(report.Replace("/", "\\")
                                                          .Replace(Document.rootBaseDB, "")
                                                          .Replace($"{reportManager.TypeLabel}\\", "")                                                          
                                                          .Replace(reportManager.FileType, "")
                                                          .TrimStart('\\'));
                            }

                        }
                        arguments = string.Format("{0} /B\"{1}\"", arguments, string.Join("|", reportsName));
                    }
                }

                var builder = new StringBuilder();
                foreach (SpecialFolders folerType in Enum.GetValues(typeof(SpecialFolders)))
                {
                    var specialPath = System.IO.Path.GetDirectoryName(Document.GetSpecialFolder(folerType).GetPathString());
                    if (System.IO.Directory.Exists(specialPath))
                    {
                        if (builder.Length == 0)
                            builder.Append(" /S");
                        else
                            builder.Append("|");
                        builder.AppendFormat("\"{0}\"", specialPath);
                    }
                }

                if (builder.Length > 0)
                    arguments += string.Format("{0}{1}", arguments, builder);

                builder.Clear();
                Document.ListChildProjectPaths.ForEach((uri) => 
                {
                    var relative = uri.GetPathString();
                    if (!XpoHelpers.XpoHelper.IsDataSource(relative))
                    {
                        var match = String.Format("{0}/", Document.Title);
                        if (relative.StartsWith(match))
                            relative = relative.Replace(match, "");
                        else
                        {
                            match = String.Format("{0}\\", Document.Title);
                            if (relative.StartsWith(match))
                                relative = relative.Replace(match, "");
                        }
                        uri = Document.MakeAbosoluteUri(new Uri(relative, UriKind.RelativeOrAbsolute));

                        var childPath = System.IO.Path.GetDirectoryName(uri.GetPathString());
                        if (System.IO.Directory.Exists(childPath))
                        {
                            if (builder.Length == 0)
                                builder.Append(" /C");
                            else
                                builder.Append("|");
                            builder.AppendFormat("\"{0}\"", childPath);
                        }
                    }
                });
                
                if (builder.Length > 0)
                    arguments += string.Format("{0}{1}", arguments, builder);

                arguments = String.Format("{0} /Y{1}", arguments, ApplicationPropertiesHelper.GetProperty("CurrentSkin"));

                string path = "WebClientInstaller.exe";
                Assembly callingMainAssembly = Assembly.GetEntryAssembly();
                if (callingMainAssembly != null)
                    path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);
                Process.Start(path, arguments);
            }
            catch (Exception ex)
            {
                if (projectManagerComponent.UIInterface != null)
                    projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.WebClientAppErrorOnCreating, ex.Message));
            }
        }
        
        private void CanEditAppNameSettings(object sender, CanExecuteRoutedEventArgs e)
        {
            bool bCanExecute = IsComponentReady && Document != null && Document.GetClientEditorDocumentManager() != null;
            e.CanExecute = bCanExecute;
        }

        private void OnEditAppNameSettings(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                var uri = new Uri(Document.ProjectFolder, UriKind.RelativeOrAbsolute);
                projectManagerComponent.Workspace.IsBusy = true;
                try
                {
                    (projectManagerComponent.ClientEditorManager as IDocumentManager).Edit(uri, Document);
                }
                catch (Exception ex)
                {
                    projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, uri, ex.Message));
                }
                finally
                {
                    projectManagerComponent.Workspace.IsBusy = false;
                }
            }
        }

        private void CanCreateWebApp(object sender, CanExecuteRoutedEventArgs e)
        {
            bool bCanExecute = IsComponentReady && Document != null && !bDisposed && Document.GetScreenDocumentManager() != null;
#if DEBUG
            e.CanExecute = bCanExecute;
#else
            e.CanExecute = bCanExecute && webDeployEnabled;
#endif
        }
        private void OnCreateWebExpress(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            if (NeedsSave)
            {
                if (projectManagerComponent.UIInterface != null)
                {
                    var res = projectManagerComponent.UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc, Document.Title), CustomDialogIcons.Question);
                    if (res == CustomDialogResults.Cancel || res == CustomDialogResults.None)
                        return;
                    else if (res == CustomDialogResults.Yes)
                        OnSaveAll(null, null);
                }
                else
                    OnSaveAll(null, null);
            }

            var control = new Controls.ProgressDialog(Document, projectManagerComponent);
            var newDialog = new GeneralDialogContent(control, GeneralDialogButtons.None)
            {
                Owner = Application.Current.MainWindow,
                Title = Properties.Resources.SVGProgressDialogWindowTitle,
            };
            var ret = newDialog.ShowDialog();

            if (control.DeployServerRemote == CustomDialogResults.Yes)
                OnDeployWebExpress(this, null);
            else if (control.DeployServerRemote == CustomDialogResults.No)
                OpenServicePanel(control.ExportedProjectFilePath);
        }

        private void CanCreateWebExpress(object sender, CanExecuteRoutedEventArgs e)
        {
            bool bCanExecute = IsComponentReady && Document != null && !bDisposed && Document.GetScreenDocumentManager() != null;
#if DEBUG
            e.CanExecute = bCanExecute;
#else
            e.CanExecute = bCanExecute && webDeployEnabled;
#endif
        }

        private void OnCreateRuntimeShortcut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            string regex = string.Format("[{0}]", Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars())));
            var title = Regex.Replace(Document.Title, regex, " - ");
            var shortcut = new WindowsApplicationShortcut(title, GetRuntimeProcessFilePath());
            shortcut.Description = Document.Description;
            shortcut.Arguments = GetRuntimeProcessArgumens(Document.ProjectPath, isDebug: false);
            if (System.IO.File.Exists(shortcut.FullName) && projectManagerComponent.UIInterface != null &&
                projectManagerComponent.UIInterface.ShowYesNo(String.Format(Properties.Resources.RuntimeShortcutAlreadyExists.Replace("'newline'", Environment.NewLine), 
                Document.Title), CustomDialogIcons.Question) != CustomDialogResults.Yes)
                return;

            try
            {
                using (new WaitCursor())
                {
                    shortcut.CreateShortcut();
                }
                if (projectManagerComponent.UIInterface != null)
                    projectManagerComponent.UIInterface.ShowInformation(String.Format(Properties.Resources.RuntimeShortcutSaveSuccessfully, Document.Title));
            }
            catch (Exception ex)
            {
                if (projectManagerComponent.UIInterface != null)
                    projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.RuntimeShortcutSaveError, ex.Message));
            }
        }

        private void CanCreateRuntimeShortcut(object sender, CanExecuteRoutedEventArgs e)
        {
            bool bCanExecute = IsComponentReady && Document != null && !bDisposed && Document.GetScreenDocumentManager() != null;
            e.CanExecute = bCanExecute;
        }

        private void OnDeployWebExpress(object sender, ExecutedRoutedEventArgs e)
        {
            if (e != null)
                e.Handled = true;
            if (Document != null && !bDisposed)
            {
                var deployControl = new Controls.DeployClient(Document, projectManagerComponent.UIInterface);
                var newDialog = new GeneralDialogContent(deployControl, GeneralDialogButtons.CloseButton)
                {
                    Owner = Application.Current.MainWindow,
                    Title = Properties.Resources.ClientDeployWindowTitle,
                };
                var ret = newDialog.ShowDialog();
            }
        }

        private void CanDeployWebExpress(object sender, CanExecuteRoutedEventArgs e)
        {
            bool bCanExecute = IsComponentReady && Document != null && !bDisposed;
#if DEBUG
            e.CanExecute = bCanExecute;
#else
            e.CanExecute = bCanExecute && webDeployEnabled;
#endif
        }

        void OnAddNewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var scheme = e.Parameter as string;
            if (String.IsNullOrEmpty(scheme))
                return;

            var selectedFolder = treeListControl.GetSelectedNodes().FirstOrDefault()?.Tag as ResourceFolderWatcher;

            var root = GetNearestRoot();
            var parentNode = (from TreeListNode n in root.Nodes where ((n.Content as TreeItemControl)?.TreeItemInnerObject as ResourceFolderWatcher)?.Scheme == scheme select n).FirstOrDefault();
            if (parentNode != null)
            {
                if (!CanBeExpanded(parentNode) && parentNode.Nodes.Count == 0)
                    treeListControl.AddNode(null, parentNode, TreeListControlHelper.DummyNode);
                OnTreeNodeExpanding(this, new TreeListNodeAllowEventArgs(parentNode));
            }

            if (selectedFolder != null && selectedFolder.Scheme == scheme && !String.IsNullOrEmpty(selectedFolder.Path))
                (root.Tag as UFProjectDocument).CreateNewResource(scheme, String.Format("{0}\\", selectedFolder.Path));
            else
                (root.Tag as UFProjectDocument).CreateNewResource(scheme);
        }

        void OnAddNewScreenFromTemplate(object sender, ExecutedRoutedEventArgs e)
        {
            var scheme = e.Parameter as string;
            if (String.IsNullOrEmpty(scheme))
                return;

            var selectedFolder = treeListControl.GetSelectedNodes().FirstOrDefault()?.Tag as ResourceFolderWatcher;

            var root = GetNearestRoot();
            var parentNode = (from TreeListNode n in root.Nodes where ((n.Content as TreeItemControl)?.TreeItemInnerObject as ResourceFolderWatcher)?.Scheme == scheme select n).FirstOrDefault();

            if (parentNode != null)
            {
                if (!CanBeExpanded(parentNode) && parentNode.Nodes.Count == 0)
                    treeListControl.AddNode(null, parentNode, TreeListControlHelper.DummyNode);
                OnTreeNodeExpanding(this, new TreeListNodeAllowEventArgs(parentNode));
            }

            if (selectedFolder != null && selectedFolder.Scheme == scheme && !String.IsNullOrEmpty(selectedFolder.Path))
                (root.Tag as UFProjectDocument).CreateNewTemplatedScreen(scheme, String.Format("{0}\\", selectedFolder.Path));
            else
                (root.Tag as UFProjectDocument).CreateNewTemplatedScreen(scheme);
        }

        void OnAddNewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && IsResourceTypeEnabled(Document, e.Parameter?.ToString());
        }

        /*
        private void OnEditAppNameChildProjectSettings(object sender, ExecutedRoutedEventArgs e)
        {
            var editAppNameSettings = new AppNameSettingsEditor(false);

            var map = new Dictionary<String, AppNameSettings>();
            Document.ListChildProjectPaths.ForEach(filename =>
                {
                    var projectName = System.IO.Path.GetFileNameWithoutExtension(filename.GetPathString());
                    if (!map.ContainsKey(projectName))
                    {
                        if (Document.MapAppNameChildProjectSettings.ContainsKey(projectName))
                            map.Add(projectName, Document.MapAppNameChildProjectSettings[projectName]);
                        else
                            map.Add(projectName, new AppNameSettings());
                    }
                });
            editAppNameSettings.MapAppNameSettings = map;

            GeneralDialogContent Dialog = new GeneralDialogContent(editAppNameSettings)
            {
                Owner = this.FindParent<Window>(),
                Title = Properties.Resources.AppNameEditorChildProject
            };
            if (Dialog.ShowDialog() != true)
            {
                return;
            }

            Document.MapAppNameChildProjectSettings = editAppNameSettings.MapAppNameSettings;
        }

        private void CanEditAppNameChildProjectSettings(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && Document.ListChildProjectPaths.Count > 0;
        }
        */

        internal bool CanClose()
        {
            if (!StopRuntime())
                return false;

            SaveData();
            return true;
        }

        static string GetRuntimeProcessFilePath()
        {
            Assembly callingMainAssembly = Assembly.GetEntryAssembly();
            var runtimeFilePath = String.Format("{0}\\{1}", System.IO.Path.GetDirectoryName(callingMainAssembly.Location), Properties.Settings.Default.RuntimeProcessName);
            if (!System.IO.File.Exists(runtimeFilePath))
                runtimeFilePath = callingMainAssembly.Location;
            return runtimeFilePath;
        }

        static string GetRuntimeProcessArgumens(string projectPath, bool isDebug)
        {
            var ret = String.Format("-file=\"{0}\" -start", projectPath);

            if (isDebug)
                ret += String.Format(" -skin=\"{0}\" -nosplash -debug", ApplicationPropertiesHelper.GetProperty("CurrentSkin").ToString());

            if (MSZ.MSZView.IsSuspended())
                ret += " -suspend";

            return ret;
        }

        bool runtimeProcessRunning;
        Process runtime;
        private void OnStartupRuntime(object sender, ExecutedRoutedEventArgs e)
        {
            if (runtimeProcessRunning)
                return;

            if (NeedsSave)
            {
                if (projectManagerComponent.UIInterface != null)
                {
                    var res = projectManagerComponent.UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc, Document.Title), CustomDialogIcons.Question);
                    if (res == CustomDialogResults.Cancel || res == CustomDialogResults.None)
                        return;
                    else if (res == CustomDialogResults.Yes)
                        OnSaveAll(null, null);
                }
                else
                    OnSaveAll(null, null);
            }

            projectManagerComponent.Workspace.BusyContent = Properties.Resources.StartingRuntime;
            projectManagerComponent.Workspace.IsBusy = true;

            runtime = Process.Start(GetRuntimeProcessFilePath(), GetRuntimeProcessArgumens(Document.ProjectPath, isDebug: true));
            runtime.Exited += (o, ev) =>
            {
                Dispatcher.BeginInvokeIfRequired(() =>
                {
                    runtimeProcessRunning = false;
                    if (runtime != null)
                    {
                        try
                        {
                            runtime.Dispose();
                        }
                        catch { }
                        runtime = null;
                    }
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                });
            };
            runtime.EnableRaisingEvents = true;
            if (!runtime.HasExited)
            {
                Thread.Sleep(1000);
                do
                {
                    try
                    {
                        runtime.Refresh();
                        runtime.WaitForInputIdle();
                        if (runtime.Responding)
                            break;
                    }
                    catch
                    {
                        runtimeProcessRunning = false;
                        runtime.Dispose();
                        runtime = null;
                        if (projectManagerComponent.UIInterface != null)
                            projectManagerComponent.UIInterface.ShowError(Properties.Resources.ErrorStartingRuntime);
                    }
                } while (true);
            }
            else
            {
                runtimeProcessRunning = false;
                runtime.Dispose();
                runtime = null;
                if (projectManagerComponent.UIInterface != null)
                    projectManagerComponent.UIInterface.ShowError(Properties.Resources.ErrorStartingRuntime);
            }
            projectManagerComponent.Workspace.IsBusy = false;

            /*
            runtime = new Thread((obj) =>
            {
                try
                {
                    Uri uri = null;
                    if (XpoHelpers.XpoHelper.IsDataSource(Document.ProjectPath))
                        uri = new Uri(String.Format("{0}:{1}", projectManagerComponent.UriRisolver.GetOpenFileScheme(), Document.ProjectPath));
                    else
                    {
                        var temp = new Uri(Document.ProjectPath, UriKind.RelativeOrAbsolute);
                        if (temp.IsUnc)
                        {
                            uri = temp;
                        }
                        else
                            uri = new Uri(String.Format("{0}://{1}", projectManagerComponent.UriRisolver.GetOpenFileScheme(), Document.ProjectPath));
                    }
                    projectManagerComponent.Execute(uri, null, ExecutionMode.Synchro, null);

                    Dispatcher.BeginInvokeIfRequired(() =>
                        {
                            try
                            {
                                projectManagerComponent.Workspace.BusyContent = null;
                                projectManagerComponent.Workspace.IsBusy = false;
                            }
                            catch(Exception ex)
                            {
                                log.Fatal(ex.Message, ex);
#if !DEBUG
                                if (bLogExceptions)
                                {
                                    _raygunClient.User = System.Environment.UserName;
                                    _raygunClient.ApplicationVersion = Utilities.AssemblyInfo.FileVersion;
                                    _raygunClient.Send(ex);
                                }
#endif
                                if (projectManagerComponent.UIInterface != null)
                                    projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorExecutingProject.Replace("--newline--", Environment.NewLine), ex.Message));
                            }
                        });

                    System.Windows.Threading.Dispatcher.CurrentDispatcher.ShutdownStarted += (ob, ev) =>
                        {
                            Utilities.LocalizationHelper.TryApplyCurrentLanguage();

                            try
                            {
                                projectManagerComponent.Terminate(uri, null);
                            }
                            catch(Exception ex)
                            {
                                log.Fatal(ex.Message, ex);
#if !DEBUG
                                if (bLogExceptions)
                                {
                                    _raygunClient.User = System.Environment.UserName;
                                    _raygunClient.ApplicationVersion = Utilities.AssemblyInfo.FileVersion;
                                    _raygunClient.Send(ex);
                                }

#endif
                                if (projectManagerComponent.UIInterface != null)
                                    projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorExecutingProject.Replace("--newline--", Environment.NewLine), ex.Message));
                            }
                        };
                    // Start the new window's Dispatcher
                    bStarting = false;
                    running = true;
                    System.Windows.Threading.Dispatcher.Run();

                    WPFUtilities.MemoryCompressor.minimizeMemory(false);
                }
#if !DEBUG
                catch(Exception ex)
                {
                    log.Fatal(ex.Message, ex);
                    if (bLogExceptions)
                    {
                        _raygunClient.User = System.Environment.UserName;
                        _raygunClient.ApplicationVersion = Utilities.AssemblyInfo.FileVersion;
                        _raygunClient.Send(ex);
                    }
                    if (projectManagerComponent.UIInterface != null)
                        projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorExecutingProject.Replace("--newline--", Environment.NewLine), ex.Message));
                }
#endif
                finally
                {
                    runtime = null;
                    running = false;
                    bStarting = false;
                }
            });

            runtime.SetApartmentState(ApartmentState.STA);
            runtime.IsBackground = true;
            runtime.Start();
            */

            e.Handled = true;
        }

        private void CanStartupRuntime(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsComponentReady && Document != null && runtime == null && Document.GetScreenDocumentManager() != null;
        }

        bool bPendingStopRuntime;
        bool StopRuntime()
        {
            if (bPendingStopRuntime)
                return false;

            if (runtime != null)
            {
                bPendingStopRuntime = true;

                projectManagerComponent.Workspace.BusyContent = Properties.Resources.StoppingRuntime;
                projectManagerComponent.Workspace.IsBusy = true;
                
                var task1 = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        runtime.Refresh();
                        if (runtime.CloseMainWindow())
                            runtime.WaitForExit();
                        else
                            runtime.Kill();
                        runtime = null;
                    }
                    catch { }
                }, TaskCreationOptions.LongRunning);

                var timeout = TimeSpan.FromSeconds(Properties.Settings.Default.StoppingRuntimeTimeout);
                var lastTime = DateTime.UtcNow;
                while(runtime != null)
                {
                    Thread.Sleep(500);
                    WaitForPriority.DoEvents();
                    if (DateTime.UtcNow - lastTime > timeout)
                    {
                        if (projectManagerComponent.UIInterface != null)
                        {
                            projectManagerComponent.Workspace.IsBusy = false;
                            var ret = projectManagerComponent.UIInterface.ShowOkCancel(Properties.Resources.RuntimeCannotBeStoppedAskKilling, CustomDialogIcons.Exclamation);
                            projectManagerComponent.Workspace.IsBusy = true;
                            if (ret != CustomDialogResults.OK)
                            {
                                lastTime = DateTime.UtcNow;
                                continue;
                            }
                        }

                        try
                        {
                            runtime.Kill();
                            runtime = null;
                            break;
                        }
                        catch { }
                    }
                }

                task1.Wait();

                projectManagerComponent.Workspace.IsBusy = false;
                bPendingStopRuntime = false;
                /*
                var dispatcher = System.Windows.Threading.Dispatcher.FromThread(runtime);
                if (dispatcher == null)
                {
                    MessageBox.Show(Properties.Resources.RuntimeRunningCannotClose);
                    return false;
                }
                else
                {
                    dispatcher.BeginInvokeIfRequired(() => dispatcher.InvokeShutdown());

                    var dlg = new ProgressDialog()
                    {
                        AutoShowDelay = 0,
                        Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                        ProgressBarIndeterminate = true,
                        DialogText = String.Format(Properties.Resources.WaitingForRuntimeTemination, Document.Title),
                        IsCancellingEnabled = false
                    };

                    dlg.RunWorkerThread(null, (o, ev) =>
                    {
                        var timeout = DateTime.Now + TimeSpan.FromSeconds(10);
                        while (runtime != null && DateTime.Now < timeout)
                            Thread.Sleep(250);
                    });

                    if (runtime != null)
                    {
                        MessageBox.Show(Properties.Resources.RuntimeRunningCannotClose);
                        return false;
                    }
                }
                */
            }

            return true;
        }

        private void OnStopRuntime(object sender, ExecutedRoutedEventArgs e)
        {
            StopRuntime();
        }

        private void CanStopRuntime(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsComponentReady && Document != null && runtime != null;
        }

        private void OnOpenServicesPanel(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            OpenServicePanel(e.Parameter as string);
        }

        void OpenServicePanel(string projectUri = null)
        {
            var servicesControl = new List<IServiceControl>();
            var document = Document;

            var selectedItem = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selectedItem != null && selectedItem.Tag is Uri)
            {
                var uri = selectedItem.Tag as Uri;
                bool isChildProjectUri = ((GetNearestDocument(selectedItem.ParentNode)?.Tag as UFProjectDocument) ?? Document).ListChildProjectPaths.Contains(uri);
                if (isChildProjectUri)
                {
                    var relative = uri.GetPathString();
                    if (!XpoHelpers.XpoHelper.IsDataSource(relative))
                    {
                        var match = String.Format("{0}/", Document.Title);
                        if (relative.StartsWith(match))
                            relative = relative.Replace(match, "");
                        else
                        {
                            match = String.Format("{0}\\", Document.Title);
                            if (relative.StartsWith(match))
                                relative = relative.Replace(match, "");
                        }
                        uri = Document.MakeAbosoluteUri(new Uri(relative, UriKind.RelativeOrAbsolute));
                    }

                    var doc = projectManagerComponent.GetDocument(uri) as UFProjectDocument;
                    if (doc != null)
                        document = doc;
                }
            }

            listDocumentManagers.ForEach(docManager =>
            {
                bool bEmpty = false;
                if (docManager is LogicManager.ComponentService.ILogicManager)
                {
                    var listStartupLogicWithService = (from c in document.ListStartupLogics where c.ExecuteAsService == true select c).ToList();
                    bEmpty = listStartupLogicWithService.Count == 0;
                }
                else if (docManager is ScriptManager.ComponentService.IScriptManager)
                {
                    var listStartupScriptWithService = (from c in document.ListStartupScripts where c.ExecuteAsService == true select c).ToList();
                    bEmpty = listStartupScriptWithService.Count == 0;
                }
                else if (docManager is UFRecipeEditor.ComponentService.IRecipeEditorManager)
                {
                    bEmpty = document.GetWholeRecipeLists().Count == 0;
                }

                if (!bEmpty)
                {
                    var service = docManager.GetServiceControl(document);
                    if (service != null)
                        servicesControl.Add(service);
                }
            });

#if !DEBUG
            if (webDeployEnabled)
#endif
            {
                if (Document != null && Document.GetScreenDocumentManager() != null)
                {
                    int maxInvalidPasswordAttempts = 0;
                    if (projectManagerComponent.UserEditor != null)
                        maxInvalidPasswordAttempts = projectManagerComponent.UserEditor.GetMaxInvalidPasswordAttempts(document, bRefresh: true);

                    var webservice = new Service.UFWebClientServiceControl(document.Title, document.ProjectPath, maxInvalidPasswordAttempts);
                    servicesControl.Add(webservice);

                    var webhmiservice = new Service.UFWebHMIServiceControl(document.Title, projectUri ?? document.ProjectPath, document.GetSpecialFolder(SpecialFolders.Images), document.GetSpecialFolder(SpecialFolders.Documents));
                    servicesControl.Add(webhmiservice);
                }
            }

            var servicesPanel = new Service.ServicesControl(servicesControl, document);
            GeneralDialogContent Dialog = new GeneralDialogContent(servicesPanel, GeneralDialogButtons.CloseHelpButtons)
            {
                Owner = this.FindParent<Window>(),
                Title = String.Format("{0} ({1})", Properties.Resources.ServicesPanelTitle, document.Title),
                HelpLink = "ServicesPanel"
            };

            Dialog.ShowDialog();
        }

        private void CanOpenServicesPanel(object sender, CanExecuteRoutedEventArgs e)
        {
            var selectedItem = (from TreeListNode n in treeListControl.GetSelectedNodes() where
                n.Tag is UFProjectDocument || n.Tag is Uri
                select n).FirstOrDefault();

            if (selectedItem == null)
                selectedItem = treeListControl.View.FocusedNode;

            e.CanExecute = IsComponentReady && 
                selectedItem != null && (selectedItem.Tag is UFProjectDocument || selectedItem.Tag is Uri);
            if (e.CanExecute && Document != null && selectedItem.Tag is Uri)
            {
                var doc = Document.GetResourceDocumentManager(selectedItem.Tag as Uri);
                if (doc == null || doc.TypeScheme != projectManagerComponent.TypeScheme)
                    e.CanExecute = false;
                //else
                //{
                //    var uri = selectedItem.Tag as Uri;
                //    bool isChildProjectUri = Document.ListChildProjectPaths.Contains(uri);
                //    if (isChildProjectUri)
                //    {
                //        var controller = Document.GetChildProjectData(uri);
                //        if (!controller.IsStartable)
                //            e.CanExecute = false;
                //    }
                //}
            }
        }

        bool CloseAlls(UFProjectDocument doc)
        {
            if (!projectManagerComponent.CloseAllChild(doc))
                return false;

            foreach (var docManager in listDocumentManagers)
            {
                if (!docManager.CloseAllChild(doc))
                    return false;
            }

            foreach(var project in doc.ListChildProjectPaths)
            {
                var abs = project;
                var relative = project.GetPathString();
                if (!XpoHelpers.XpoHelper.IsDataSource(relative))
                {
                    var match = String.Format("{0}/", doc.Title);
                    if (relative.StartsWith(match))
                        relative = relative.Replace(match, "");
                    else
                    {
                        match = String.Format("{0}\\", doc.Title);
                        if (relative.StartsWith(match))
                            relative = relative.Replace(match, "");
                    }
                    abs = Document.MakeAbosoluteUri(new Uri(relative, UriKind.RelativeOrAbsolute));
                }

                var d = projectManagerComponent.GetOpenDocument(abs) as UFProjectDocument;
                if (d != null)
                {
                    if (!CloseAlls(d))
                        return false;
                }
            }

            return true;
        }

        private void OnCloseAll(object sender, ExecutedRoutedEventArgs e)
        {
            projectManagerComponent.Workspace.IsBusy = true;
            try
            {
                CloseAlls(Document);
            }
            finally
            {
                projectManagerComponent.Workspace.IsBusy = false;
            }
            e.Handled = true;
        }

        private void CanCloseAll(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsComponentReady && Document != null;
        }

        internal bool CloseAllChildProjects(UFProjectDocument doc)
        {
            foreach (var docManager in listDocumentManagers)
            {
                if (!docManager.CloseAllChild(doc, bParentClosing: !(docManager is UFProjectManagerComponent)) || docManager.IsAnyChildNeedsSave(doc))
                    return false;
            }

            foreach (var project in doc.ListChildProjectPaths)
            {
                var relative = project.GetPathString();
                var abs = project;
                if (!XpoHelpers.XpoHelper.IsDataSource(relative))
                {
                    var match = String.Format("{0}/", doc.Title);
                    if (relative.StartsWith(match))
                        relative = relative.Replace(match, "");
                    else
                    {
                        match = String.Format("{0}\\", doc.Title);
                        if (relative.StartsWith(match))
                            relative = relative.Replace(match, "");
                    }
                    abs = doc.MakeAbosoluteUri(new Uri(relative, UriKind.RelativeOrAbsolute));
                }

                var d = projectManagerComponent.GetOpenDocument(abs) as UFProjectDocument;
                if (d != null)
                {
                    projectManagerComponent.CloseProject(d);
                    if (!CloseAllChildProjects(d))
                        return false;
                }
            }

            return true;
        }

        private void OnClose(object sender, ExecutedRoutedEventArgs e)
        {
            //projectManagerComponent.Workspace.IsBusy = true;
            try
            {
                if (CloseAllChildProjects(Document))
                {
                    //projectManagerComponent.Workspace.IsBusy = false;
                    projectManagerComponent.CloseProject(this);
                }
            }
            finally
            {
                //projectManagerComponent.Workspace.IsBusy = false;
            }
            e.Handled = true;
        }

        private void CanClose(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsComponentReady && Document != null;
        }

        void SaveAllChild(UFProjectDocument doc, bool SaveBackup)
        {
            if (SaveBackup && doc.NeedsSave)
            {
                if (doc.EnableBackup)
                {
                    projectManagerComponent.Workspace.BusyContent = Properties.Resources.SaveBackup;
                    doc.SaveBackup();
                }
            }

            doc.ListChildProjectPaths.ForEach(project =>
                {
                    var abs = project;
                    var relative = project.GetPathString();
                    if (!XpoHelpers.XpoHelper.IsDataSource(relative))
                    {
                        var match = String.Format("{0}/", doc.Title);
                        if (relative.StartsWith(match))
                            relative = relative.Replace(match, "");
                        else
                        {
                            match = String.Format("{0}\\", doc.Title);
                            if (relative.StartsWith(match))
                                relative = relative.Replace(match, "");
                        }
                        abs = doc.MakeAbosoluteUri(new Uri(relative, UriKind.RelativeOrAbsolute));
                    }

                    var d = projectManagerComponent.GetOpenDocument(abs) as UFProjectDocument;
                    if (d != null)
                    {
                        projectManagerComponent.SaveAllChild(d);
                        listDocumentManagers.ForEach(docManager =>
                        {
                            docManager.SaveAllChild(d);
                        });

                        SaveAllChild(d, SaveBackup);
                    }
                });

            if (doc.NeedsSave)
            {
                projectManagerComponent.Workspace.BusyContent = Properties.Resources.SaveProject;
                doc.SaveToFile();
                doc.NeedsSave = false;
            }
        }

        private void OnSaveAll(object sender, ExecutedRoutedEventArgs e)
        {
            projectManagerComponent.Workspace.BusyContent = Properties.Resources.SaveProject;
            projectManagerComponent.Workspace.IsBusy = true;
            bool bSaveBackup = false;
            if (sender != null && NeedsSave || sender == null)
            {
                if (Document.EnableBackup)
                {
                    projectManagerComponent.Workspace.BusyContent = Properties.Resources.SaveBackup;
                    Document.SaveBackup();
                }
                bSaveBackup = true;
            }

            projectManagerComponent.Workspace.BusyContent = Properties.Resources.SaveProject;

            SaveAllChild(Document, bSaveBackup);
            projectManagerComponent.SaveAllChild(Document);
            listDocumentManagers.ForEach(docManager =>
                {
                    docManager.SaveAllChild(Document);
                });

            if (Document.NeedsSave)
            {
                Document.SaveToFile();
                Document.NeedsSave = false;
            }

            projectManagerComponent.Workspace.IsBusy = false;
            projectManagerComponent.Workspace.BusyContent = null;

            if (e != null)
                e.Handled = true;
        }

        private void CanSaveAll(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsComponentReady && Document != null;
        }

        private void OnCreateQRCode(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            projectManagerComponent.Workspace.IsBusy = true;
            try
            {
                var selectedItem = treeListControl.GetSelectedNodes().FirstOrDefault();
                var uri = selectedItem.Tag as Uri;
                //var imagepath = uri.GetPathString();
                //imagepath = System.IO.Path.ChangeExtension(imagepath, ".png");
                uri = Document.MakeRelativeUri(uri);
                var arguments = string.Format("/Q\"{0}\"", uri);
                //if (System.IO.File.Exists(imagepath))
                //    arguments = String.Format("{0} /I\"{1}\"", arguments, imagepath);
                string path = "tools\\QRCodeRuntimeGenerator.exe";
                Assembly callingMainAssembly = Assembly.GetEntryAssembly();
                if (callingMainAssembly != null)
                    path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);
                var process = Process.Start(path, arguments);
                projectManagerComponent.Workspace.ResetBusy();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                if (projectManagerComponent.UIInterface != null)
                    projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorCreatingQRCode, ex.Message));
            }
            finally
            {
                projectManagerComponent.Workspace.IsBusy = false;
                projectManagerComponent.Workspace.RestoreBusy();
            }
        }

        private void CanCreateQRCode(object sender, CanExecuteRoutedEventArgs e)
        {
            var selectedItem = treeListControl.GetSelectedNodes().FirstOrDefault();
            e.CanExecute = projectManagerComponent.Workspace.ActiveWindow == this && selectedItem != null && selectedItem.Tag is Uri;
        }

        private void OnCommandSave(object sender, ExecutedRoutedEventArgs e)
        {
            Document.SaveToFile();
            Document.NeedsSave = false;
            e.Handled = true;
        }

        private void CanCommandSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsComponentReady && Document != null && Document.NeedsSave;
        }

        TerraBrowser.TerraBrowser terraServer = new TerraBrowser.TerraBrowser();
        private void OnAddGeoLocation(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var selectedItem = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selectedItem.Tag is UFProjectDocument)
            {
                var doc = selectedItem.Tag as UFProjectDocument;
                terraServer.Latitude = doc.Latitude;
                terraServer.Longitude = doc.Longitude;
            }
            else if (selectedItem.Tag is Uri)
            {
                var data = Document.GetControllerData(selectedItem.Tag as Uri, true);
                terraServer.Latitude = data.Latitude;
                terraServer.Longitude = data.Longitude;
            }
            else if (selectedItem.Tag is ResourceFolderWatcher)
            {
                var data = Document.GetControllerData(new Uri((selectedItem.Tag as ResourceFolderWatcher).Path), true);
                terraServer.Latitude = data.Latitude;
                terraServer.Longitude = data.Longitude;
            }
            else
            {
                terraServer.Latitude = Document.Latitude;
                terraServer.Longitude = Document.Longitude;
            }

            GeneralDialogContent Dialog = new GeneralDialogContent(terraServer)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "GeoLocation",
            };
            if (Dialog.ShowDialog() != true)
            {
                return;
            }

            if (selectedItem.Tag is UFProjectDocument)
            {
                var doc = selectedItem.Tag as UFProjectDocument;
                doc.Latitude = terraServer.Latitude;
                doc.Longitude = terraServer.Longitude;
            }
            else if (selectedItem.Tag is Uri)
            {
                var data = Document.GetControllerData(selectedItem.Tag as Uri, true);
                data.Latitude = terraServer.Latitude;
                data.Longitude = terraServer.Longitude;
            }
            else if (selectedItem.Tag is ResourceFolderWatcher)
            {
                var data = Document.GetControllerData(new Uri((selectedItem.Tag as ResourceFolderWatcher).Path), true);
                data.Latitude = terraServer.Latitude;
                data.Longitude = terraServer.Longitude;
            }
        }

        void CanAddGeoLocation(object sender, CanExecuteRoutedEventArgs e)
        {
            bool bCanExecute = IsActiveWindow && Document != null && Document.GetScreenDocumentManager() != null;
            var selectedItem = treeListControl.GetSelectedNodes().FirstOrDefault();
            e.CanExecute = bCanExecute && projectManagerComponent.Workspace.ActiveWindow == this && 
#if !DEBUG
                geolocal && 
#endif
            selectedItem != null && 
                (selectedItem.Tag is UFProjectDocument || selectedItem.Tag is Uri/* ||
                 (selectedItem.Tag is ResourceFolderWatcher && !(selectedItem.Tag as ResourceFolderWatcher).IsRoot)*/);
            if (e.CanExecute && Document != null && selectedItem.Tag is Uri)
            {
                var doc = Document.GetResourceDocumentManager(selectedItem.Tag as Uri);
                if (doc == null || doc.TypeScheme == projectManagerComponent.TypeScheme)
                    e.CanExecute = false;
            }
        }

        private void OnCutEvent(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void CanExecuteCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        bool UriOrFoldersSelected()
        {
            foreach (TreeListNode node in treeListControl.GetSelectedNodes())
                if (node.Tag is Uri || (node.Tag is ResourceFolderWatcher && !((ResourceFolderWatcher)node.Tag).IsRoot))
                    return true;
            return false;
        }

        private void OnDeleteEvent(object sender, ExecutedRoutedEventArgs e)
        {
            var listFolders = treeListControl.GetSelectedNodesOfType<ResourceFolderWatcher>();
            var listUris = treeListControl.GetSelectedNodesOfType<Uri>();
            var text = Properties.Resources.DeleteMultipleResources;
            if (listUris.Count == 1)
                text = String.Format(Properties.Resources.DeleteResource,
                                System.IO.Path.GetFileNameWithoutExtension(listUris[0].GetPathString()));

            var bSelectionContainsFolder = (from folder in listFolders/*.AsParallel()*/
                                where folder.ListResources.Count != 0 || folder.ListFolders.Count != 0
                                select folder).ToList().Count > 0;

            if (bSelectionContainsFolder)
            {
                text = Properties.Resources.SelectedObjectsNotEmpty;
            }

            if (listUris.Count > 0 || listFolders.Count > 0)
            {
                var res = projectManagerComponent.UIInterface.ShowYesNo(text, CustomDialogIcons.Warning);
                if (res == CustomDialogResults.Yes)
                {
                    using(var cursor = new WaitCursor())
                    {
                        listFolders.ForEach(item =>
                        {
                            item.DeleteCommand.Execute(null);
                        });

                        listUris.ForEach(uri =>
                            {
                                try
                                {
                                    var document = Document;
                                    if (mapObjectToNode.ContainsKey(uri))
                                        document = (GetNearestDocument(mapObjectToNode[uri])?.Tag as UFProjectDocument) ?? Document;

                                    bool isChildProjectUri = document.ListChildProjectPaths.Contains(uri);
                                    if (isChildProjectUri)
                                    {
                                        var abs = uri;
                                        var relative = uri.GetPathString();
                                        if (!XpoHelpers.XpoHelper.IsDataSource(relative))
                                        {
                                            var match = String.Format("{0}/", document.Title);
                                            if (relative.StartsWith(match))
                                                relative = relative.Replace(match, "");
                                            else
                                            {
                                                match = String.Format("{0}\\", document.Title);
                                                if (relative.StartsWith(match))
                                                    relative = relative.Replace(match, "");
                                            }
                                            abs = document.MakeAbosoluteUri(new Uri(relative, UriKind.RelativeOrAbsolute));
                                        }

                                        bool ret = true;
                                        var d = projectManagerComponent.GetOpenDocument(abs) as UFProjectDocument;
                                        if (d != null)
                                        {
                                            if (!CloseAllChildProjects(d))
                                                ret = false;
                                            else
                                                projectManagerComponent.CloseProject(d);
                                        }

                                        if (ret)
                                        {
                                            document.RemoveChildProject(uri);

                                            if (mapObjectToNode.ContainsKey(uri))
                                            {
                                                var parent = mapObjectToNode[uri].ParentNode;
                                                parent.Nodes.Remove(mapObjectToNode[uri]);
                                            }
                                        }
                                    }
                                    else
                                        Document.DeleteResource(uri);
                                }
                                catch (Exception ex)
                                {
                                    projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorDeletingDocument, uri, ex.Message));
                                }
                            });
                    }
                }
            }

            e.Handled = true;
        }

        private void CanExecuteDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsActiveWindow && UriOrFoldersSelected() && projectManagerComponent != null && 
                projectManagerComponent.Workspace != null && projectManagerComponent.Workspace.ActiveWindow == this;
        }

        void Folder_AddDummy(TreeListNode folderNode)
        {
            if (folderNode == null)
                return;
            if (folderNode.Nodes.Count == 0)
                treeListControl.AddNode(null, folderNode, TreeListControlHelper.DummyNode);
            folderNode.IsExpanded = true;
        }

        private void OnPasteEvent(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = treeListControl.GetSelectedNodes();
            if (selectedItems == null || selectedItems.Length == 0)
                return;

            Dictionary<TreeListNode, ResourceFolderWatcher> sourceFoldersNodes = new Dictionary<TreeListNode, ResourceFolderWatcher>();
            foreach (var selectedItem in selectedItems)
            {
                sourceFoldersNodes.Add(selectedItem, GetParentFolder(selectedItem));
                Folder_AddDummy(GetParentFolderItem(selectedItem));
            }

            treeListControl.ClearSelection();

            foreach (var uri in listClipboard.Keys)
            {
                var docManager = Document.GetResourceDocumentManager(uri);
                if (docManager == null)
                    continue;

                var selectedItem = (from sf in sourceFoldersNodes.Keys where sourceFoldersNodes[sf].Scheme == docManager.TypeScheme select sf).FirstOrDefault();
                if (selectedItem == null) {
                    var doc = GetNearestRoot(selectedItems.First()).Tag as UFProjectDocument;
                    if (mapMainComponentsRoot.ContainsKey(doc) && mapMainComponentsRoot[doc].ContainsKey(docManager.TypeScheme))
                        selectedItem = mapMainComponentsRoot[doc][docManager.TypeScheme];
                    else
                        continue;
                }

                var sourcefolder = sourceFoldersNodes.ContainsKey(selectedItem) ? sourceFoldersNodes[selectedItem] : GetParentFolder(selectedItem);

                var newName = sourcefolder.CreateNewName(System.IO.Path.GetFileNameWithoutExtension(uri.OriginalString));
                var folderName = sourcefolder.Path;
                if (folderName.Length > 0 &&
                    folderName[folderName.Length - 1] != '\\' && folderName[folderName.Length - 1] != '/')
                    folderName += "\\";
                var newPath = String.Format("{0}{1}{2}", folderName, newName,
                                sourcefolder.FileType);
                if (listClipboard[uri] != Document)
                {
                    if (listClipboard[uri].fileSystemProviderBase != null)
                    {
                        docManager.Copy(uri, listClipboard[uri].ProjectPath, true, listClipboard[uri], false);
                        docManager.Rename(uri, uri.OriginalString, newPath, listClipboard[uri]);
                    }
                    else
                    {
                        docManager.Copy(uri, newPath, true, listClipboard[uri], false);
                        if (docManager.IsStartupControllerAware)
                        {
                            var newUri = new Uri(newPath, UriKind.RelativeOrAbsolute);
                            Document.CopyControllerData(uri, newUri, listClipboard[uri]);
                        }
                    }
                }
                else
                {
                    docManager.Copy(uri, newPath, true, Document, false);
                    if (docManager.IsStartupControllerAware)
                    {
                        var newUri = new Uri(newPath, UriKind.RelativeOrAbsolute);
                        Document.CopyControllerData(uri, newUri, Document);
                    }
                }

                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        var newUri = new Uri(newPath, UriKind.RelativeOrAbsolute);
                        var i = (from p in GetParentFolderItem(selectedItem).Nodes
                                    where p.Tag as Uri == newUri
                                    select p).ToList();
                        treeListControl.SelectNodes(i);
                    });
            }
            e.Handled = true;
        }

        private void CanExecutePaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            var selectedItems = treeListControl.GetSelectedNodes();
            foreach (var selectedItem in selectedItems)
            {
                var sourcefolder = GetParentFolder(selectedItem);
                if (sourcefolder != null && Document != null)
                {
                    foreach (var uri in listClipboard.Keys)
                    {
                        var docManager = Document.GetResourceDocumentManager(uri);
                        if (docManager != null && sourcefolder.Scheme == docManager.TypeScheme)
                        {
                            e.CanExecute = IsActiveWindow;
                            break;
                        }
                    }
                    if (e.CanExecute)
                        break;
                }
            }
        }

        static readonly Dictionary<Uri, UFProjectDocument> listClipboard = new Dictionary<Uri, UFProjectDocument>();

        private void OnCopyEvent(object sender, ExecutedRoutedEventArgs e)
        {
            listClipboard.Clear();
            foreach (TreeListNode selectedItem in treeListControl.GetSelectedNodes())
            {
                if (selectedItem.Tag is Uri)
                {
                    var uri = selectedItem.Tag as Uri;
                    if (!listClipboard.ContainsKey(uri))
                        listClipboard.Add(uri, Document);
                }
            }
            e.Handled = true;
        }

        private void CanExecuteCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            foreach (TreeListNode selectedItem in treeListControl.GetSelectedNodes())
            {
                if (selectedItem.Tag is Uri)
                    e.CanExecute = IsActiveWindow;
                else
                {
                    e.CanExecute = false;
                    break;
                }
            }
        }

        private void OnUndoEvent(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void CanExecuteUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void OnRedoEvent(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        public static string RemoveSpecialCharacters(string input, out string invalidChars)
        {
            var r = new Regex(@"(?:[^a-z0-9\\:._\- ]|(?<=['])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            
            var matches = r.Matches(input, 0);
            var invalidBuilder = new StringBuilder();
            foreach (Match match in matches)
            {
                if (!invalidBuilder.ToString().Contains(match.Value))
                    invalidBuilder.Append(match.Value);
            }
            invalidChars = invalidBuilder.ToString();

            return r.Replace(input, String.Empty);
        }

        public static string RemoveInvalidPathCharacters(string input, out string invalidChars)
        {
            var ret = input;
            var invalidBuilder = new StringBuilder();
            var list = System.IO.Path.GetInvalidPathChars().ToList();
            list.Add(System.IO.Path.DirectorySeparatorChar);
            var invalidPathChars = list.ToArray();
            var startIndex = input.IndexOfAny(invalidPathChars);
            while (startIndex >= 0)
            {
                ret = ret.Remove(startIndex - invalidBuilder.Length, 1);
                invalidBuilder.Append(input.Substring(startIndex, 1));
                startIndex = input.IndexOfAny(invalidPathChars, startIndex + 1);
            } 
            invalidChars = invalidBuilder.ToString();

            return ret;
        }

        public static string RemoveInvalidFileNameCharacters(string input, out string invalidChars)
        {
            var ret = input;
            var invalidBuilder = new StringBuilder();
            var invalidFileNameChars = System.IO.Path.GetInvalidFileNameChars();
            var startIndex = input.IndexOfAny(invalidFileNameChars);
            while (startIndex >= 0)
            {
                ret = ret.Remove(startIndex - invalidBuilder.Length, 1);
                invalidBuilder.Append(input.Substring(startIndex, 1));
                startIndex = input.IndexOfAny(invalidFileNameChars, startIndex + 1);
            }
            invalidChars = invalidBuilder.ToString();

            return ret;
        }

        private void OnSaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            OnSaveAll(null, null);

            var doc = projectManagerComponent.Workspace.ContextDocument as IDocument;
            IUIMsgBoxAlertService uIMsgBoxAlertService = null;
            IHelpProvider helpProvider = null;
            if (doc != null)
            {
                uIMsgBoxAlertService = doc.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                helpProvider = doc.GetService(typeof(IHelpProvider)) as IHelpProvider;
            }
            var filter = projectManagerComponent.UriRisolver.GetOpenFileFilter();
            var fileType = new CommonControls.SelectFileType(false, filter, Properties.Settings.Default.DefaultFileExt, uIMsgBoxAlertService, helpProvider);
            while (true)
            {
                var newDialog = new GeneralDialogContent(fileType)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink="SaveProjectAs"
                };
                if (newDialog.ShowDialog() != true)
                    break;

                bool bTargetDataSource = XpoHelpers.XpoHelper.IsDataSource(fileType.currentUri);
                if (bTargetDataSource && Document.IsPasswordProtected())
                {
                    projectManagerComponent.UIInterface.ShowError(Properties.Resources.ErrorSaveAsNoPassowrdProjectForDB);
                    return;
                }
                if (!bTargetDataSource && Document.fileSystemProviderBase == null)
                {
                    try
                    {
                        var dirDest = System.IO.Path.GetDirectoryName(fileType.currentUri);
                        var dirSource = System.IO.Path.GetDirectoryName(Document.ProjectPath);

                        if (dirDest == null)
                        {
                            projectManagerComponent.UIInterface.ShowError(Properties.Resources.ErrorSaveAsEnterValidPath);
                            continue;
                        }

                        var index = dirDest.IndexOf(dirSource);
                        if (index == 0)
                        {
                            projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorSaveAsCannotSharePath, dirDest, dirSource));
                            continue;
                        }
                    
                        string  invalidChars;
                        var checkPath = RemoveSpecialCharacters(fileType.currentUri, out invalidChars);
                        if (checkPath != fileType.currentUri)
                        {
                            projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorSaveAsCannotUseSpecialChars, fileType.currentUri, invalidChars));
                            continue;
                        }
                    }
                    catch
                    {
                        projectManagerComponent.UIInterface.ShowError(Properties.Resources.ErrorSaveAsEnterValidPath);
                        continue;
                    }
                }

                using (var cursor = new WaitCursor())
                {
                    if (!bTargetDataSource)
                    {
                        var ext = System.IO.Path.GetExtension(fileType.currentUri);
                        if (String.IsNullOrEmpty(ext))
                            ext = Properties.Settings.Default.DefaultFileExt;
                        var prjName = System.IO.Path.GetFileNameWithoutExtension(fileType.currentUri);
                        fileType.currentUri = string.Format("{0}\\{1}\\{2}{3}", System.IO.Path.GetDirectoryName(fileType.currentUri), 
                            prjName, prjName, ext);
                        var pathSimple = System.IO.Path.GetDirectoryName(fileType.currentUri);
                        if (!System.IO.Directory.Exists(pathSimple))
                            System.IO.Directory.CreateDirectory(pathSimple);
                    }
                    if (Document.SaveAs(fileType.currentUri))
                    {
                        bool wasProtected = Document.Protected;
                        if (!bTargetDataSource)
                        {
                            var ext = System.IO.Path.GetExtension(fileType.currentUri);
                            if (String.IsNullOrEmpty(ext))
                                fileType.currentUri = fileType.currentUri + Properties.Settings.Default.DefaultFileExt;
                        }

                        OnClose(sender, e);

                        var uri = new Uri(bTargetDataSource ?
                            String.Format("{0}:{1}", projectManagerComponent.TypeScheme, fileType.currentUri) :
                            fileType.currentUri);

                        projectManagerComponent.Edit(uri, null);
                        projectManagerComponent.StartupWelcome.AddToLatestOpened(uri);

                        if (wasProtected && !bTargetDataSource)
                        {
                            projectManagerComponent.UIInterface.ShowWarning(Properties.Resources.ProjectNotProtectedAnymore);
                        }
                    }
                    else
                        projectManagerComponent.UIInterface.ShowError(Properties.Resources.ErrorSaveAs);
                }

                break;
            }
        }

        private void CanSaveAs(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null;
        }

        private void OnAddStringId(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (var cursor = new WaitCursor())
            {
                var list = Document.GetControllerDataStrings();

                if (list.Count > 0)
                    projectManagerComponent.StringEditor.AddListStringId(Document, list);
            }
        }

        private void CanAddStringId(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Document != null)
            {
                var list = Document.GetControllerDataStrings();
                e.CanExecute = IsActiveWindow && projectManagerComponent.StringEditor != null && list.Count > 0;
            }
            else
                e.CanExecute = false;
        }

        private void OnAddPasswordProject(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var newkeyControl = new Controls.NewKey();

            while (true)
            {
                var newkeyDialog = new GeneralDialogContent(newkeyControl)
                {
                    Owner = Application.Current.MainWindow,
                    Title = Properties.Resources.EnterNewProjectPassword,
                    HelpLink = "EnterNewProjectPassword"
                };
                newkeyControl.txtToHide.Visibility = Document.IsPasswordProtected() ?
                                        Visibility.Visible : Visibility.Collapsed;
                if (newkeyDialog.ShowDialog() == true)
                {
                    if (!Document.IsPasswordProtected() && newkeyControl.keyBox.Password.Length > 0 &&
                        newkeyControl.verifykeyBox.Password == newkeyControl.keyBox.Password)
                    {
                        Document.SetProjectPassword(newkeyControl.verifykeyBox.Password);
                        break;
                    }
                    else if (Document.IsPasswordProtected() && newkeyControl.keyBox.Password.Length == 0)
                    {
                        Document.SetProjectPassword(newkeyControl.verifykeyBox.Password);
                        if (!Document.IsPasswordProtected())
                            projectManagerComponent.UIInterface.ShowWarning(Properties.Resources.ProjectNotProtectedAnymore);
                        break;
                    }
                    else if (Document.IsPasswordProtected() && newkeyControl.keyBox.Password.Length > 0)
                    {
                        Document.SetProjectPassword(newkeyControl.verifykeyBox.Password);
                        break;
                    }
                    else if (newkeyControl.verifykeyBox.Password != newkeyControl.keyBox.Password)
                        projectManagerComponent.UIInterface.ShowError(Properties.Resources.PasswordDidNotMatch);
                    else
                        projectManagerComponent.UIInterface.ShowError(Properties.Resources.PasswordNoEmpty);
                }
                else
                    break;
            }
        }

        private void CanAddPasswordProject(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsComponentReady && Document != null && Document.fileSystemProviderBase == null;
        }
        #endregion

        public void Dispose()
        {
            if (bDisposed)
                return;

            bDisposed = true;
            
            if (dpActivateExplorer != null &&
                dpActivateExplorer.Status != DispatcherOperationStatus.Aborted &&
                dpActivateExplorer.Status != DispatcherOperationStatus.Completed)
                    dpActivateExplorer.Abort();

            mapObjectToNode.Clear();
            mapMainComponentsRoot.Clear();
            ClearWatchingLists();
            projectManagerComponent.Workspace.ContextObject = null;

            foreach(var pair in mapSubscribedItems)
                pair.Key.PropertyChanged -= ChangeNotifier_PropertyChanged;
            mapSubscribedItems.Clear();

            foreach (var dp in mapDispatcherOperations.Values)
                dp.Abort();
            mapDispatcherOperations.Clear();

            var toRemove = (from c in listClipboard.Keys// .AsParallel()
                            where listClipboard[c] == Document
                            select c).ToList();
            toRemove.ForEach(c =>
                {
                    listClipboard.Remove(c);
                });
            Document = null;
            UnsubscribeDetailsCollectionChanged();
            CommandBindings.Clear();
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            rowSplitter.Height = new GridLength(0);
            lastGridHeight = rowDetails.Height;
            rowDetails.Height = new GridLength(0, GridUnitType.Auto);
        }

        GridLength lastGridHeight;
        bool bDetailsHeightSet;
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            rowSplitter.Height = new GridLength(5);
            if (!bDetailsHeightSet)
            {
                bDetailsHeightSet = true;
                var h = ActualHeight / 3;
                rowDetails.Height = new GridLength(h);
                gridControl.MaxHeight = h;

                Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                {
                    gridControl.ClearValue(MaxHeightProperty);
                });
            }
            else
                rowDetails.Height = lastGridHeight;

            UpdateSelectionDetailsGridSource();
        }

        #region Save Load Recents

        readonly String StoreFileName = String.Format("{0}.dat", System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));

        static IsolatedStorageFile GetStorage()
        {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            { }

            return null;
        }

        public class SaveDataStorage
        {
            public bool isExpanded;
            public double height;
        }

        void SaveData()
        {
            IsolatedStorageFile isoStorage = GetStorage();
            if (null == isoStorage || string.IsNullOrEmpty(StoreFileName))
                return;
            var name = Assembly.GetExecutingAssembly().GetName().Name;
            using (var Mutex = new Mutex(false, name))
            {
                try
                {
                    Mutex.WaitOne();
                }
                catch (AbandonedMutexException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

                try
                {
                    using (Stream stream = new IsolatedStorageFileStream(StoreFileName, FileMode.Create, isoStorage))
                    {
                        XmlWriterSettings settings = new XmlWriterSettings
                        {
                            Indent = true,
                            OmitXmlDeclaration = false,
                            Encoding = Encoding.UTF8
                        };

                        using (XmlWriter writer = XmlWriter.Create(stream, settings))
                        {
                            try
                            {
                                var data = new SaveDataStorage()
                                {
                                    isExpanded = expanderDetails.IsExpanded,
                                    height = lastGridHeight.Value
                                };
                                if (expanderDetails.IsExpanded)
                                    data.height = rowDetails.Height.Value;

                                DataContractSerializer serializer = new DataContractSerializer(typeof(SaveDataStorage));
                                serializer.WriteObject(writer, data);
                            }
                            catch (Exception ex)
                            {
                                writer.Close();
                            }
                        }
                    }
                }
                catch
                {

                }
                finally
                {
                    Mutex.ReleaseMutex();
                }
            }
        }

        void LoadData()
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                return;

            IsolatedStorageFile isoStorage = GetStorage();
            if (null == isoStorage || string.IsNullOrEmpty(StoreFileName) ||
                isoStorage.GetFileNames(StoreFileName).Length <= 0)
                return;

            var name = Assembly.GetExecutingAssembly().GetName().Name;
            using (var Mutex = new Mutex(false, name))
            {
                Mutex.WaitOne();

                try
                {
                    using (Stream stream = new IsolatedStorageFileStream(StoreFileName, FileMode.OpenOrCreate, isoStorage))
                    {
                        XmlReaderSettings settings = new XmlReaderSettings
                        {
                            ConformanceLevel = ConformanceLevel.Document,
                            CloseInput = true
                        };

                        using (XmlReader reader = XmlReader.Create(stream, settings))
                        {
                            try
                            {
                                DataContractSerializer serializer = new DataContractSerializer(typeof(SaveDataStorage));
                                var data = serializer.ReadObject(reader) as SaveDataStorage;

                                bDetailsHeightSet = true;
                                lastGridHeight = new GridLength(data.height);
                                expanderDetails.IsExpanded = data.isExpanded;
                            }
                            catch (Exception ex)
                            {
                                reader.Close();
                            }
                        }
                    }
                }
                catch
                {

                }
                finally
                {
                    Mutex.ReleaseMutex();
                }
            }
        }

        #endregion

        private void CanRename(object sender, CanExecuteRoutedEventArgs e)
        {
            var selectedItem = (treeListControl.SelectedItem as TreeItemControl)?.InnerControl?.DataContext;
            if (selectedItem == null || !bLoaded || bDisposed || treeListControl.GetSelectedNodes().Length != 1)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = selectedItem is UriModel || (selectedItem is ResourceFolderWatcher && !((ResourceFolderWatcher)selectedItem).IsRoot);
        }

        private void OnRename(object sender, ExecutedRoutedEventArgs e)
        {
            treeListControl.EditNode(treeListControl.GetSelectedNodes().FirstOrDefault());
        }

        private void OnCellValueChanged(object sender, TreeListCellValueChangedEventArgs e)
        {
            if ((sender as TreeListView).IsEditing && String.IsNullOrEmpty(e.Value as string) && !String.IsNullOrEmpty(e.OldValue as string))
            {
                (e.Node.Content as TreeItemControl).ItemHeader = e.OldValue;
                Dispatcher.BeginInvokeAsynchronously(() => treeListControl.SelectNode(e.Node));
            }
        }

        internal void SelectNodeFromUri(Uri uri)
        {
            if (mapObjectToNode.ContainsKey(uri))
            {
                var parent = mapObjectToNode[uri].ParentNode;
                if (parent != null && !parent.IsExpanded)
                    parent.IsExpanded = true;
                treeListControl.SelectNode(mapObjectToNode[uri]);
            }
        }
    }

    //public class CustomTreeListView : TreeListView
    //{
    //    protected override void OnSelectionChanged(DevExpress.Data.SelectionChangedEventArgs e)
    //    {
    //        if (SelectedRowsSource == null)
    //        {
    //            base.OnSelectionChanged(e);
    //            return;
    //        }
    //        int[] selectionHandles = DataProviderBase.Selection.GetSelectedRows();
    //        bool Equal = true;
    //        if (selectionHandles.Length == SelectedRowsSource.Count)
    //        {
    //            for (int i = 0; i < selectionHandles.Length; i++)
    //            {
    //                if (!SelectedRowsSource.Contains(DataProviderBase.GetRowValue(selectionHandles[i])))
    //                {
    //                    Equal = false;
    //                    break;
    //                }
    //            }
    //        }
    //        else
    //            Equal = false;
    //        if (!Equal)
    //            base.OnSelectionChanged(e);
    //    }
    //}
}

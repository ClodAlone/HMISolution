using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UFUAEditor.Document;
using Utilities;
using Utilities.WPF;
using UFUAEditor.ComponentService;
using DevExpress.Xpf.Grid;
using WPFUtilities;
using DevExpress.Xpf.Grid.TreeList;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for AlarmBrowser.xaml
    /// </summary>
    public partial class AlarmBrowser : UserControl, IDisposable
    {
        #region Declarations

        readonly static String rootName = "Root";
        readonly UFUAServerDocument Document;
        TreeListNode itemRoot;
        BitmapImage openFolderImg;
        BitmapImage closedFolderImg;
        bool bLoaded;

        bool singleselection;
        bool isPopup;
        bool isPopupWasClosed;
        bool isDataContextChanging;
        TreeListNode selectedItem;
        #endregion

        public AlarmBrowser(UFUAServerDocument doc, bool bPopup = false, bool singlesel = true)
        {
            InitializeComponent();
            Document = doc;
            isPopup = bPopup;
            singleselection = singlesel;

            if (isPopup)
            {
                Loaded += (o, e) =>
                {
                    if (singleselection)
                        treeListControl.SelectionMode = MultiSelectMode.None;
                    var wnd = this.FindParent<Window>();
                    if (wnd != null)
                    {
                        selectedItem = null;

                        wnd.Activated += (s, c) =>
                        {
                            //Document.CopyWinClipboardToInMemoryData();

                            if (isPopupWasClosed)
                            {
                                isPopupWasClosed = false;
                            }
                        };

                        wnd.Closing += (s, c) =>
                        {
                            isPopupWasClosed = true;

                            var list = new List<object>();
                            var selected = selectedItem ?? treeListControl.GetSelectedNodes().FirstOrDefault();
                            if(treeListControl.SelectedItems.Count > 0)
                            {
                                foreach(var p in treeListControl.GetSelectedNodes())
                                {
                                    if (p.Tag is UFUAModel.UFUAArea)
                                        list.Add(p.Tag as UFUAModel.UFUAArea);
                                    else if (p.Tag is UFUAModel.UFUAAlarmSource)
                                        list.Add(p.Tag as UFUAModel.UFUAAlarmSource);
                                }
                            }

                            if (list.Count > 0)
                            {
                                isDataContextChanging = true;
                                if (DataContext is String)
                                {
                                    if (list[0] is UFUAModel.UFUAArea)
                                        DataContext = (list[0] as UFUAModel.UFUAArea).GetRelativeName();
                                    else if (list[0] is UFUAModel.UFUAAlarmSource)
                                        DataContext = (list[0] as UFUAModel.UFUAAlarmSource).GetRelativeName();
                                }
                                else
                                {
                                    object[] result = new Object[2];
                                    result[0] = Document.GetServerOPCUAEntityReference().ToXml();
                                    result[1] = list;
                                    DataContext = result;
                                }
                                isDataContextChanging = false;
                            }
                        };
                    }

                    if (!bLoaded)
                    {
                        bLoaded = true;
                        InitializeAlarmBrowser();
                    }
                };

                //DataContextChanged += (o, e) =>
                //    {
                //        if (!isDataContextChanging && DataContext is OPCUAViewModel.OPCUAEntityReference)
                //        {

                //        }
                //    };
            }
            else
            {

                Loaded += (o, e) =>
                {
                    if (!bLoaded)
                    {
                        bLoaded = true;
                        InitializeAlarmBrowser();
                    }

                };
            }
        }

        #region Alarm Browser

        class rootHeader
        {
            public String Name { get; set; }
        }

        void InitializeAlarmBrowser()
        {
            openFolderImg = UFUAEditorManagerComponent.GetBitmapImage("OpenFolderSmall", true);
            closedFolderImg = UFUAEditorManagerComponent.GetBitmapImage("CloseFolderSmall", true);

            treeListView.Nodes.Clear();

            itemRoot = treeListControl.AddNode(new TreeItemControl(rootName) { ResourceIcon = closedFolderImg }, null, Document);
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
            e.Handled = true;
            if (!isPopup)
            {
                List<Object> selecteditems = new List<Object>();
                foreach (var item in treeListControl.GetSelectedNodes())
                {
                    if (item.Tag != null)
                        selecteditems.Add(item.Tag);
                }

                //if (selecteditems.Count == 0)
                //{
                //    Document.EditorManagerComponent.Workspace.ContextObject = null;
                //}
                //else if (selecteditems.Count == 1)
                //{
                //    Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(selecteditems[0]);
                //}
                //else
                //{
                //    Document.EditorManagerComponent.Workspace.ContextObjects = Document.GetNestedObjects(selecteditems);
                //}
            }
        }

        void ClearNodes(TreeListNode node)
        {
            if (node == null)
                return;
            node.Nodes.Clear();
        }

        private void FillItems(TreeListNode itemRoot, UFUAModel.UFUAArea root = null)
        {
            if (!isPopup && root == null)
                Document.EditorManagerComponent.Workspace.IsBusy = true;
            treeListControl.BeginDataUpdate();

            try
            {
                ClearNodes(itemRoot);

                using (new WaitCursor())
                {
                    var listFolders = Document.GetAlarmAreas(root);
                    if (listFolders != null)
                    {
                        foreach (var folder in listFolders)
                            AddTreeItem(folder, itemRoot);
                    }
                    var listSources = Document.GetSourceCollection(root);
                    if (listSources != null)
                    {
                        foreach (var source in listSources)
                            AddTreeItem(source, itemRoot);
                    }
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
                if (!isPopup && root == null)
                    Document.EditorManagerComponent.Workspace.IsBusy = false;
            }
        }

        private void FillItems(TreeListNode itemRoot, UFUAModel.UFUAAlarmSource tag)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                ClearNodes(itemRoot);
                using (new WaitCursor())
                {
                    foreach (var alarm in tag.UFUAAlarmDefinitions)
                        AddTreeItem(alarm, itemRoot);
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
            
        }

        void OnTreeNodeCollapsing(object sender, TreeListNodeAllowEventArgs e)
        {
            
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
                if (item.Tag is UFUAModel.UFUAArea)
                    FillItems(item, item.Tag as UFUAModel.UFUAArea);
                else if (item.Tag is UFUAModel.UFUAAlarmSource)
                    FillItems(item, item.Tag as UFUAModel.UFUAAlarmSource);

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

            if (tag is UFUAModel.UFUAArea)
            {
                bKnownType = true;
                var folder = tag as UFUAModel.UFUAArea;
                (newitem.Content as TreeItemControl).ItemHeader = folder.Name;
                (newitem.Content as TreeItemControl).ResourceIcon = UFUAEditorManagerComponent.GetBitmapImage("UFUASAlarmAreaSmall");
                treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);
            }
            else if (tag is UFUAModel.UFUAAlarmSource)
            {
                bKnownType = true;
                var ufuatag = tag as UFUAModel.UFUAAlarmSource;
                (newitem.Content as TreeItemControl).ItemHeader = ufuatag.Name;
                (newitem.Content as TreeItemControl).ResourceIcon = UFUAEditorManagerComponent.GetBitmapImage("UFUASAlarmSourceSmall");
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

        private void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null)
            {
                if (isPopup && (selected.Tag is UFUAModel.UFUAArea || selected.Tag is UFUAModel.UFUAAlarmSource))
                {
                    var wnd = this.FindParent<Window>();
                    if (wnd != null)
                    {
                        selectedItem = selected;
                        wnd.DialogResult = true;
                        wnd.Close();
                    }
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

            //Document.EditorManagerComponent.Workspace.ContextObject = null;
        }
        #endregion

    }
}

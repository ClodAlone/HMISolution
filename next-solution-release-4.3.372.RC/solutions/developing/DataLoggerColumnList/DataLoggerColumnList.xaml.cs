using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using DocumentManager.ComponentService;
using OPCUAViewModel;
using UFUAEditor.ComponentService;
using Utilities;
using Utilities.WPF;
using WPFUtilities;

namespace DataLoggerColumnListControl
{
    /// <summary>
    /// Interaction logic for DataLoggerColumnList.xaml
    /// </summary>
    public partial class DataLoggerColumnList : UserControl, IDisposable
    {
        readonly static String rootName = "Root";
        TreeListNode itemRoot;
        BitmapImage openFolderImg;
        BitmapImage closedFolderImg;
        IDocument sdoc;
        IUFUAEditorManager UFUAEditor;
        bool bLoaded;
        bool isPopup = true;
        public DataLoggerColumnList(IDocument doc)
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    using (new WaitCursor())
                    {
                        sdoc = doc;
                        if (sdoc != null)
                        {
                            UFUAEditor = sdoc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                        }
                        InitializeAddressSpace();
                    }
                }
            };
        }
        #region Address Space

        void InitializeAddressSpace()
        {
            if (sdoc != null)
            {
                if (UFUAEditor == null)
                    UFUAEditor = sdoc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                if (UFUAEditor != null)
                {
                    openFolderImg = SharedResources.Helpers.ResourceManager.GetCommonImage(null, "OpenFolder", true);
                    closedFolderImg = SharedResources.Helpers.ResourceManager.GetCommonImage(null, "CloseFolder", true);
                }
            }

            treeListView.Nodes.Clear();
            itemRoot = treeListControl.AddNode(new TreeItemControl(rootName) { ResourceIcon = closedFolderImg });
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

        private void treeListControl_SelectionChanged(object sender, TreeListSelectionChangedEventArgs e)
        {
            e.Handled = true;
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
                    if (sdoc != null)
                    {
                        if (UFUAEditor == null)
                            UFUAEditor = sdoc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;

                        if (UFUAEditor != null)
                        {
                            IList<DataLoggerSettings> list = new List<DataLoggerSettings>();
                            //UFUAEditor.GetDataLoggerSettings(sdoc).ToList()./*AsParallel().ForAll*/ForEach(d =>
                            //{
                            //    DataLoggerSettings ds = new DataLoggerSettings() { Name = d[0], TableName = d[1], UtcTimeColumnName = d[2] };
                            //    ds.Columns = new DataLoggerColumns();
                            //    UFUAEditor.GetDataLoggerColumnSettingList(sdoc, ds.Name).ToList().ForEach(s =>
                            //    {
                            //        ds.Columns.Add(new DataLoggerColumn() { Name = s[0], SourceTimeStampColumnName = s[1], AddSourceTimeStampColumn = bool.Parse(s[2]), ColumnTagName = s[3], ColumnTagGuid = s[4] });
                            //    });
                            //    list.Add(ds);
                            //});
                            var list2 = UFUAEditor.GetDataLoggerSettings(sdoc);
                            if (list2 != null)
                            {
                                foreach (List<String> d in list2)
                                {
                                    DataLoggerSettings ds = new DataLoggerSettings() { Name = d[0], TableName = d[1], UtcTimeColumnName = d[2] };
                                    ds.Columns = new DataLoggerColumns();
                                    var columns = UFUAEditor.GetDataLoggerColumnList(sdoc, ds.Name) as List<String>;
                                    if (columns == null)
                                        continue;
                                    columns.ForEach(s => ds.Columns.Add(new DataLoggerColumn() { Name = s }));
                                    list.Add(ds);
                                }
                            }

                            if (list != null)
                            {
                                foreach (var column in list)
                                    AddTreeItem(column, itemRoot);
                            }
                        }
                    }
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }
        }

        private void FillItems(TreeListNode itemRoot, DataLoggerSettings root)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                ClearNodes(itemRoot);
                using (new WaitCursor())
                {
                    foreach (var column in root.Columns)
                        AddTreeItem(column, itemRoot);
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
                if (item.Tag is DataLoggerSettings)
                    FillItems(item, item.Tag as DataLoggerSettings);

                treeListControl.EndDataUpdate();
                e.Handled = true;
            }
            finally
            {
                (item.Content as TreeItemControl).IsNodeExpanding = false;
            }
        }

        public TreeListNode AddTreeItem(Object tag, TreeListNode parent)
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
            //contentToNodeMap.Add(ic, newitem);

            if (tag is DataLoggerSettings)
            {
                bKnownType = true;
                var datalogger = tag as DataLoggerSettings;
                (newitem.Content as TreeItemControl).ItemHeader = datalogger.Name;

                if(sdoc != null)
                {
                    if (UFUAEditor == null)
                        UFUAEditor = sdoc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                    if (UFUAEditor != null)
                    {
                        (newitem.Content as TreeItemControl).ResourceIcon = UFUAEditor.GetBitmapImage(sdoc, "UFUASDataLoggerSettings");
                    }
                }
                treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);
            }
            else if (tag is DataLoggerColumn)
            {
                bKnownType = true;
                var datalogger = tag as DataLoggerColumn;
                (newitem.Content as TreeItemControl).ItemHeader = datalogger.Name;

                if (sdoc != null)
                {
                    if (UFUAEditor == null)
                        UFUAEditor = sdoc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                    if (UFUAEditor != null)
                    {
                        (newitem.Content as TreeItemControl).ResourceIcon = UFUAEditor.GetBitmapImage(sdoc, "UFUASDataLoggerColumn");
                    }
                }
            }

            //newitem.Selected += (o, e) =>
            //{
            //    if (!isPopup)
            //        Document.EditorManagerComponent.Workspace.ContextObject = tag;
            //    e.Handled = true;
            //};

            treeListControl.RefreshRow(newitem.RowHandle); //Otherwise node's header (ItemHeader) can disappear in certain conditions
            return bKnownType ? newitem : null;
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

        public TreeListNode GetSelectedParent(bool bSkipSelected = false)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (!bSkipSelected && selected != null && selected.Tag is DataLoggerSettings)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null)
                    return selected;
            }

            return itemRoot;
        }

        public bool IsAnyItemSelected()
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            return item != null && item != itemRoot;
        }

        public TreeListNode GetSelectedDataLoggerSettingsParentItem()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is DataLoggerSettings)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is DataLoggerSettings)
                    return selected;
            }

            return itemRoot;
        }

        public string GetSelectedColumnName()
        {
            DataLoggerColumn column = GetSelectedDataLoggerColumn();
            //DataLoggerSettings datalogger = GetSelectedDataLoggerSettingsParent();
            try
            {
                //return string.Format("{0}.{1}", datalogger.Name, column.Name);
                return string.Format("{0}", column.Name);
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }

        public string GetSelectedDLRName()
        {
            DataLoggerSettings datalogger = GetSelectedDataLoggerSettingsParent();
            try
            {
                return string.Format("{0}", datalogger.Name);
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }

        public DataLoggerSettings GetSelectedDataLoggerSettingsParent()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is DataLoggerSettings)
                return selected.Tag as DataLoggerSettings;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is DataLoggerSettings)
                    return selected.Tag as DataLoggerSettings;
            }

            return null;
        }

        public DataLoggerColumn GetSelectedDataLoggerColumn()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is DataLoggerColumn)
                return selected.Tag as DataLoggerColumn;

            return null;
        }
        public OPCUAEntityReference GetSelectedColumnReference()
        {
            DataLoggerColumn column = GetSelectedDataLoggerColumn();
            //DataLoggerSettings datalogger = GetSelectedDataLoggerSettingsParent();
            try
            {
                var parent = GetSelectedParent();
                if (sdoc != null && parent != null && parent.Tag is DataLoggerSettings)
                {
                    var reference = UFUAEditor.GetDataLoggerColumnReference(sdoc, (parent.Tag as DataLoggerSettings).Name, column.Name);
                    if (reference != null)
                        return reference.FromXml<OPCUAEntityReference>();
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        private void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            e.Handled = true;
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null)
            {
                if (isPopup && selected.Tag is DataLoggerColumn)
                {
                    var wnd = this.FindParent<Window>();
                    if (wnd != null)
                    {
                        wnd.DialogResult = true;
                        wnd.Close();
                    }
                }
            }
        }

        #endregion

        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
        }
    }
}

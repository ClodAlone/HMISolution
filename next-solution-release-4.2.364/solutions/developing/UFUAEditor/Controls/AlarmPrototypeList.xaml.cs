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
using DevExpress.Xpo;
using WPFUtilities;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using System.Windows.Data;
using UFUAEditor.Helpers;
using System.ComponentModel;
using DevExpress.Xpf.Core;
using DocumentManager.ComponentService;
using System.Windows.Threading;
using UFUAEditor.Extensions;
using DevExpress.Data.TreeList;
using TempVariablesManager.Helpers;
using UFUAEditor.Converters;
using UIMsgBoxAlertService.ComponentService;
using Utilities.Xpo.UndoRedo;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for AlarmPrototypeList.xaml
    /// </summary>
    public partial class AlarmPrototypeList : TreeViewEditorHelper, IEditableObject
    {
        #region Declarations

        TreeListNode itemRoot;
        BitmapImage openFolderImg;
        BitmapImage closedFolderImg;

        AlarmThresholdNodeNameConverter thresholdNameConverter;

        readonly Dictionary<UFUAModel.UFUAAlarmThreshold, UFUAModel.UFUATag> mapAlarmToTag = new Dictionary<UFUAModel.UFUAAlarmThreshold, UFUAModel.UFUATag>();

        List<int> expandedMasterRowHandles = new List<int>();

        bool isPopup;
        bool bLoaded;
        #endregion

        #region Ctor
        public AlarmPrototypeList(UFUAServerDocument doc, bool bPopup = false)
        {
            InitializeComponent();
            SetTabsContent(treeListControl, gridDataControl);
            Document = doc;
            isPopup = bPopup;
            if (isPopup)
                tableView.AllowMasterDetail = false;

            TagPathColumn.DisplayMemberBinding = new Binding { Path = new PropertyPath("TreeItemInnerObject.UFUATagAss[0]"), Converter = new TagPathConverter() { Document = doc } };

            Document.CreateUndoRedoHelper();

            gridDataControl.ItemsSource = Document.GetAlarmDefinitions();
            gridDetailsControl.FixedFilter = new DevExpress.Data.Filtering.BinaryOperator("IsValid", true);
            Document.ChangedDocument += Document_ChangedDocument;

            tabAlarmPrototypesTabControl.SelectionChanging += (s, e) =>
            {
                if (e.NewSelectedItem == gridControlTab &&
                    dpFlatGridRefresh != null && dpFlatGridRefresh.Status == DispatcherOperationStatus.Aborted)
                    FlatGridRefresh();
            };

            if (isPopup)
            {
                Loaded += (o, e) =>
                {
                    var wnd = this.FindParent<Window>();
                    if (wnd != null)
                    {
                        wnd.Activated += (s, c) =>
                        {
                            Document.CopyWinClipboardToInMemoryData();
                        };
                    }

                    if (!bLoaded)
                    {
                        bLoaded = true;
                        InitializeAddressSpace();
                    }
                };
            }
            else
            {
                InitializeAddressSpace();

                Loaded += (o, e) =>
                {
                    if (!bLoaded)
                        bLoaded = true;
                };
            }
        }

        void Document_ChangedDocument(object sender, ChangedDocumentEvent e)
        {
            if (e.Source == this)
                return;

            var changedObjects = (from object c in e.ChangedObjects.AsParallel()
                                  where (!isPopup && c is UFUAModel.UFUAAlarmThreshold) ||
                                  c is UFUAModel.UFUAAlarmDefinition ||
                                  c is UFUAModel.UFUAAlarmSource ||
                                  c is UFUAModel.UFUAArea ||
                                  (!isPopup && c is UFUAModel.UFUATag)
                                  select c).ToList();

            if (changedObjects.Count > 0)
            {
                try
                {
                    UnSubscribeSelectionChangedEvent = true;
                    treeListControl.BeginDataUpdate();

                    foreach (var changedObject in changedObjects)
                    {
                        if (e.ChangedType == ChangedType.changed)
                        {
                            if (changedObject is UFUAModel.UFUATag)
                            {
                                var ufuatag = changedObject as UFUAModel.UFUATag;
                                UFUAModel.UFUATag nestedObject = null;
                                if (Document.UowContext != null && Document.UowContext.InTransaction)
                                    nestedObject = Document.UowContext.GetNestedObject(changedObject) as UFUAModel.UFUATag;
                                if (nestedObject != null && nestedObject.UseShared.Value != ufuatag.UseShared.Value)
                                {
                                    foreach (var threshold in nestedObject.UFUAAlarmThresholds)
                                    {
                                        //if (threshold.UFUAAlarmDefinitionRef == null)
                                        //    continue;

                                        var definition = Document.FindAlarmDefinitionByNodeId(threshold.UFUAAlarmDefinitionNodeIdRef);
                                        if (definition != null && mapObjectToNode.ContainsKey(definition))
                                        {
                                            RefreshAsync(mapObjectToNode[definition]);
                                            FlatGridRefresh();
                                        }
                                    }
                                }
                                if (nestedObject != null && nestedObject.PrototypeModel != ufuatag.PrototypeModel)
                                {
                                    if (ufuatag.SubPrototypeMembers != null && ufuatag.SubPrototypeMembers.Count > 0)
                                    {
                                        var members = (from c in ufuatag.SubPrototypeMembers[0].GetTagMembers().AsParallel()
                                                       where !c.UseShared.Value && c.UFUAAlarmThresholds.Count > 0
                                                       select c).ToList();

                                        if (members.Count > 0)
                                        {
                                            foreach (var member in members)
                                            {
                                                foreach (var threshold in member.UFUAAlarmThresholds)
                                                {
                                                    //if (threshold.UFUAAlarmDefinitionRef == null)
                                                    //    continue;

                                                    var definition = Document.FindAlarmDefinitionByNodeId(threshold.UFUAAlarmDefinitionNodeIdRef);
                                                    if (definition != null && mapObjectToNode.ContainsKey(definition))
                                                        RefreshAsync(mapObjectToNode[definition]);
                                                }
                                            }

                                            FlatGridRefresh();
                                        }
                                    }
                                }
                            }
                        }
                        else if (e.ChangedType == ChangedType.removed)
                        {
                            if (changedObject is UFUAModel.UFUATag)
                            {
                                var ufuatag = changedObject as UFUAModel.UFUATag;
                                var thresholds = (from c in ufuatag.UFUAAlarmThresholds.AsParallel()
                                                    where mapObjectToNode.ContainsKey(c)
                                                    select c).ToList();
                                if (ufuatag.SubPrototypeMembers != null && ufuatag.SubPrototypeMembers.Count > 0)
                                {
                                    var members = (from c in ufuatag.SubPrototypeMembers[0].GetTagMembers().AsParallel()
                                                    where !c.UseShared.Value && c.UFUAAlarmThresholds.Count > 0
                                                    select c).ToList();
                                    foreach (var member in members)
                                    {
                                        thresholds.AddRange(from c in member.UFUAAlarmThresholds.AsParallel()
                                                            where mapObjectToNode.ContainsKey(c)
                                                            select c);
                                    }
                                }

                                if (thresholds.Count > 0)
                                {
                                    foreach (var threshold in thresholds)
                                    {
                                        var parent = mapObjectToNode[threshold].ParentNode;
                                        parent.Nodes.Remove(mapObjectToNode[threshold]);
                                    }

                                    FlatGridRefresh();
                                }
                            }
                            if (mapObjectToNode.ContainsKey(changedObject))
                            {
                                var parent = mapObjectToNode[changedObject].ParentNode;
                                parent.Nodes.Remove(mapObjectToNode[changedObject]);

                                FlatGridRefresh();
                            }
                        }
                        else if (e.ChangedType == ChangedType.added)
                        {
                            TreeListNode parent = null;
                            if (changedObject is UFUAModel.UFUATag)
                            {
                                var ufuatag = changedObject as UFUAModel.UFUATag;
                                if (ufuatag.UFUAAlarmThresholds.Count > 0)
                                {
                                    foreach (var threshold in ufuatag.UFUAAlarmThresholds)
                                    {
                                        //if (threshold.UFUAAlarmDefinitionRef == null)
                                        //    continue;

                                        var definition = Document.FindAlarmDefinitionByNodeId(threshold.UFUAAlarmDefinitionNodeIdRef);
                                        if (definition != null && mapObjectToNode.ContainsKey(definition) && mapObjectToNode[definition].WasExpanded())
                                            AddTreeItem(threshold, mapObjectToNode[definition]);
                                    }

                                    FlatGridRefresh();
                                }
                                if (ufuatag.SubPrototypeMembers != null && ufuatag.SubPrototypeMembers.Count > 0)
                                {
                                    var members = (from c in ufuatag.SubPrototypeMembers[0].GetTagMembers().AsParallel()
                                                   where !c.UseShared.Value && c.UFUAAlarmThresholds.Count > 0
                                                   select c).ToList();

                                    if (members.Count > 0)
                                    {
                                        foreach (var member in members)
                                        {
                                            foreach (var threshold in member.UFUAAlarmThresholds)
                                            {
                                                //if (threshold.UFUAAlarmDefinitionRef == null)
                                                //    continue;

                                                var definition = Document.FindAlarmDefinitionByNodeId(threshold.UFUAAlarmDefinitionNodeIdRef);
                                                if (definition != null && mapObjectToNode.ContainsKey(definition) && mapObjectToNode[definition].WasExpanded())
                                                    AddTreeItem(threshold, mapObjectToNode[definition]);
                                            }
                                        }

                                        FlatGridRefresh();
                                    }
                                }
                            }
                            else if (changedObject is UFUAModel.UFUAAlarmThreshold && (changedObject as UFUAModel.UFUAAlarmThreshold).UFUAAlarmDefinitionRef != null)
                            {
                                var source = (changedObject as UFUAModel.UFUAAlarmThreshold).UFUAAlarmDefinitionRef;
                                if (mapObjectToNode.ContainsKey(source))
                                    parent = mapObjectToNode[source];
                            }
                            else if (changedObject is UFUAModel.UFUAAlarmDefinition && (changedObject as UFUAModel.UFUAAlarmDefinition).UFUAAlarmDefinitions != null)
                            {
                                var source = (changedObject as UFUAModel.UFUAAlarmDefinition).UFUAAlarmDefinitions;
                                if (mapObjectToNode.ContainsKey(source))
                                    parent = mapObjectToNode[source];
                            }
                            else if (changedObject is UFUAModel.UFUAAlarmSource && (changedObject as UFUAModel.UFUAAlarmSource).UFUAArea != null)
                            {
                                var area = (changedObject as UFUAModel.UFUAAlarmSource).UFUAArea;
                                if (mapObjectToNode.ContainsKey(area))
                                    parent = mapObjectToNode[area];
                            }
                            else if (changedObject is UFUAModel.UFUAArea && (changedObject as UFUAModel.UFUAArea).UFUAAreaAss != null)
                            {
                                var area = (changedObject as UFUAModel.UFUAArea).UFUAAreaAss;
                                if (mapObjectToNode.ContainsKey(area))
                                    parent = mapObjectToNode[area];
                            }
                            else if (changedObject is UFUAModel.UFUAArea)
                                parent = itemRoot;

                            if (parent != null)
                            {
                                if (parent.WasExpanded())
                                    AddTreeItem(changedObject, parent);

                                FlatGridRefresh();
                            }
                        }
                    }
                }
                finally
                {
                    treeListControl.EndDataUpdate();
                    UnSubscribeSelectionChangedEvent = false;
                }
            }
        }
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
            e.CanExecute = IsAnyItemSelectedExcept(typeof(UFUAModel.UFUAAlarmThreshold)) && tabAlarmPrototypesTabControl.SelectedItem == treeListTab;
            e.Handled = !e.CanExecute;
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
            e.CanExecute = IsAnyItemSelectedExcept(typeof(UFUAModel.UFUAAlarmThreshold)) && tabAlarmPrototypesTabControl.SelectedItem == treeListTab;
            e.Handled = !e.CanExecute;
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
            e.CanExecute = tabAlarmPrototypesTabControl.SelectedItem == treeListTab && (Document.ClipboardContainsAlarmAreas() || Document.ClipboardContainsAlarmSources() || Document.ClipboardContainsAlarmDefinitions());
            e.Handled = !e.CanExecute;
        }
        #endregion

        #region Address Space

        class rootHeader
        {
            public String Name { get; set; }
        }

        void InitializeAddressSpace()
        {
            openFolderImg = UFUAEditorManagerComponent.GetBitmapImage("OpenFolderSmall", true);
            closedFolderImg = UFUAEditorManagerComponent.GetBitmapImage("CloseFolderSmall", true);

            treeListView.Nodes.Clear();
            itemRoot = treeListControl.AddNode(new TagPathTreeItemControl(Properties.Resources.AlarmRibbonTitle) { ResourceIcon = closedFolderImg }, Tag as TreeListNode, Document);
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

        void OnGridSelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            e.Handled = true;
            if (!isPopup && !UnSubscribeSelectionChangedEvent)
            {
                EndEdit();

                UpdateContextObjects(sender);
            }
        }

        void treeListControl_SelectionChanged(object sender, TreeListSelectionChangedEventArgs e)
        {
            if (!isPopup && !UnSubscribeSelectionChangedEvent)
            {
                EndEdit();

                UpdateContextObjects(sender);
            }
        }

        internal void ForceSelectionChangedEvent()
        {
            treeListControl_SelectionChanged(this, null);
        }

        private void FillItems(TreeListNode iRoot, UFUAModel.UFUAArea areaRoot = null)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                ClearNodes(iRoot);
                using (new WaitCursor())
                {
                    var listAlarmAreas = Document.GetAlarmAreas(areaRoot);

                    if (listAlarmAreas != null)
                    {
                        foreach (var area in listAlarmAreas)
                            AddTreeItem(area, iRoot);
                    }

                    var listSources = Document.GetSourceCollection(areaRoot);
                    if (listSources != null)
                    {
                        foreach (var source in listSources)
                            AddTreeItem(source, iRoot);
                    }
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }
        }

        void OnTreeNodeChanged(object sender, TreeListNodeChangedEventArgs e)
        {
            if (e.ChangeType == NodeChangeType.Add && e.Node.Tag != null && e.Node.Tag != TreeListControlHelper.DummyNode && !mapObjectToNode.ContainsKey(e.Node.Tag))
            {
                mapObjectToNode.Add(e.Node.Tag, e.Node);
                if (e.Node.Tag is UFUAModel.UFUAAlarmThreshold)
                    (e.Node.Tag as INotifyPropertyChanged).PropertyChanged += UFUAAlarmThreshold_PropertyChanged;
            }
            else if (e.ChangeType == NodeChangeType.Remove && e.Node.Tag != null && e.Node.Tag != TreeListControlHelper.DummyNode)
                ClearObjectMapNode(e.Node);
        }

        void ClearObjectMapNode(TreeListNode node)
        {
            if (node.Tag != null && node.Tag != TreeListControlHelper.DummyNode && mapObjectToNode.ContainsKey(node.Tag))
            {
                mapObjectToNode.Remove(node.Tag);
                if (node.Tag is UFUAModel.UFUAAlarmThreshold)
                    (node.Tag as INotifyPropertyChanged).PropertyChanged -= UFUAAlarmThreshold_PropertyChanged;
                foreach (var child in node.Nodes)
                    ClearObjectMapNode(child);
            }
        }

        void UFUAAlarmThreshold_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (UnSubscribeSelectionChangedEvent)
                return;

            if (e.PropertyName == "UFUAAlarmDefinitionRef")
            {
                if (mapObjectToNode.ContainsKey(sender))
                {
                    RefreshAsync(mapObjectToNode[sender].ParentNode);
                    FlatGridRefresh();
                }
            }
        }

        void ClearNodes(TreeListNode node)
        {
            node.Nodes.Clear();
        }

        private void FillItems(TreeListNode iRoot, UFUAModel.UFUAAlarmSource sourceRoot)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                ClearNodes(iRoot);
                using (new WaitCursor())
                {
                    foreach (var ad in sourceRoot.UFUAAlarmDefinitions)
                        AddTreeItem(ad, iRoot);
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }
        }

        private void FillItems(TreeListNode iRoot, UFUAModel.UFUAAlarmDefinition sourceRoot)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                ClearNodes(iRoot);
                using (new WaitCursor())
                {
                    var thresholds = (from t in sourceRoot.UFUAAlarmThresholds 
                                      where t.IsValid && (!t.UFUATagAss.IsSubPrototypeMember || !t.UFUATagAss.UseShared.Value)
                                      orderby t.UFUATagAss.Name select t).ToList();
                    foreach (var threshold in thresholds)
                        AddTreeItemThreshold(threshold, iRoot, threshold.UFUATagAss);
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }
        }

        void OnTreeNodeCollapsing(object sender, TreeListNodeAllowEventArgs e)
        {
            if (!(e.Node.Content as TreeItemControl).IsNodeExpanding)
                UpdateFolderIcon(e.Node, false);
        }

        void UpdateFolderIcon(TreeListNode node, bool isOpen)
        {
            if (node != null && (node == itemRoot || node.Tag is UFUAModel.UFUAFolder))
                (node.Content as TreeItemControl).ResourceIcon = isOpen ? openFolderImg : closedFolderImg;
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

                (item.Content as TreeItemControl).IsNodeExpanding = true;
                treeListControl.BeginDataUpdate();
                ClearNodes(item);

                if (item.Tag is UFUAModel.UFUAArea)
                    FillItems(item, item.Tag as UFUAModel.UFUAArea);
                else if (item.Tag is UFUAModel.UFUAAlarmSource)
                    FillItems(item, item.Tag as UFUAModel.UFUAAlarmSource);
                else if (item.Tag is UFUAModel.UFUAAlarmDefinition)
                    FillItems(item, item.Tag as UFUAModel.UFUAAlarmDefinition);

                treeListControl.EndDataUpdate();
            }
            finally
            {
                (item.Content as TreeItemControl).IsNodeExpanding = false;
            }
        }

        TreeListNode AddTreeItemThreshold(UFUAModel.UFUAAlarmThreshold thres, TreeListNode parent, UFUAModel.UFUATag parentTag)
        {
            var newitem = AddAlarmNode(thres, parent);

            var thresholdTag = parentTag ?? (mapAlarmToTag.ContainsKey(thres) ? mapAlarmToTag[thres] : null);

            AssignThreshold(thres, thresholdTag, parent.Tag as UFUAModel.UFUAAlarmDefinition);

            if (thresholdNameConverter == null)
                thresholdNameConverter = new AlarmThresholdNodeNameConverter();
            SetBindingOnProp(newitem,
                new object[] { thresholdTag, thres, thres.UFUAAlarmDefinitionRef },
                new string[] { "Name", "Expression", "Name" },
                thresholdNameConverter);
            SetBindingOnProp(newitem, thres, nameof(thresholdTag.FolderPath), TagPathTreeItemControl.FolderPathProperty);
            (newitem.Content as TreeItemControl).ResourceIcon = UFUAEditorManagerComponent.GetBitmapImage("UFUASAlarmThreshold");

            treeListControl.RefreshRow(newitem.RowHandle); //Otherwise node's header (ItemHeader) can disappear in certain conditions
            return newitem;
        }

        TreeListNode AddAlarmNode(Object tag, TreeListNode parent)
        {
            if (bDisposed || parent == null)
                return null;

            if (!parent.IsExpanded && !(parent.Content as TreeItemControl).IsNodeExpanding)
            {
                parent.IsExpanded = true;
                var list = (from p in parent.Nodes where p.Tag == tag select p).ToList();
                if (list.Count > 0)
                    return list[0];
            }

            var ic = new TagPathTreeItemControl(tag);
            var newitem = treeListControl.AddNode(ic, parent, tag);

            return newitem;
        }

        internal TreeListNode AddTreeItem(Object tag, TreeListNode parent)
        {
            if (tag is UFUAModel.UFUAAlarmThreshold)
            {
                var thres = tag as UFUAModel.UFUAAlarmThreshold;
                return AddTreeItemThreshold(thres, parent, thres.UFUATagAss);
            }

            var newitem = AddAlarmNode(tag, parent);
            if (tag is UFUAModel.UFUAArea)
            {
                //var area = tag as UFUAModel.UFUAArea;
                //(newitem.Content as TreeItemControl).ItemHeader = area.Name;
                SetBindingOnProp(newitem, tag, "Name");
                (newitem.Content as TreeItemControl).ResourceIcon = UFUAEditorManagerComponent.GetBitmapImage("UFUASAlarmAreaSmall");
                if (NeedToBeExpanded(newitem))
                    treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);
            }
            else if (tag is UFUAModel.UFUAAlarmSource)
            {
                //var alarmSource = tag as UFUAModel.UFUAAlarmSource;
                //(newitem.Content as TreeItemControl).ItemHeader = alarmSource.Name;
                SetBindingOnProp(newitem, tag, "Name");
                (newitem.Content as TreeItemControl).ResourceIcon = UFUAEditorManagerComponent.GetBitmapImage("UFUASAlarmSourceSmall");
                if (NeedToBeExpanded(newitem))
                    treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);
            }
            else if (tag is UFUAModel.UFUAAlarmDefinition)
            {
                //var hs = tag as UFUAModel.UFUAAlarmDefinition;
                //(newitem.Content as TreeItemControl).ItemHeader = hs.Name;
                SetBindingOnProp(newitem, tag, "Name");
                (newitem.Content as TreeItemControl).ResourceIcon = (tag as UFUAModel.UFUAAlarmDefinition).Severity > 0 ?
                    UFUAEditorManagerComponent.GetBitmapImage("UFUASAlarmPrototypeSmall") :
                    UFUAEditorManagerComponent.GetBitmapImage("UFUASMessagePrototypeSmall");
                if (!isPopup && NeedToBeExpanded(newitem))
                    treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);
            }

            treeListControl.RefreshRow(newitem.RowHandle); //Otherwise node's header (ItemHeader) can disappear in certain conditions
            return newitem;
        }

        void AssignThreshold(UFUAModel.UFUAAlarmThreshold thres, UFUAModel.UFUATag tag, UFUAModel.UFUAAlarmDefinition alarmDefinition)
        {
            if (tag != null)
            {
                if (!tag.UFUAAlarmThresholds.Contains(thres))
                {
                    tag.UFUAAlarmThresholds.Add(thres);
                    tag.NotifyPropertyChanged("UFUAAlarmThresholds");
                }
                if (IsUndoRedoSupported(tag))
                    mapAlarmToTag[thres] = tag;
            }
            if (alarmDefinition != null)
            {
                thres.UFUAAlarmDefinitionRef = alarmDefinition;
                thres.UFUAAlarmDefinitionNodeIdRef = alarmDefinition.NodeId;
                if (!alarmDefinition.UFUAAlarmThresholds.Contains(thres))
                    alarmDefinition.UFUAAlarmThresholds.Add(thres);
            }
        }

        public TreeListNode GetSelectedParent(bool bSkipSelected = false)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (!bSkipSelected && selected != null &&
                (selected.Tag is UFUAModel.UFUAArea || selected.Tag is UFUAModel.UFUAAlarmSource))
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null)
                    return selected;
            }

            return itemRoot;
        }

        internal TreeListNode GetSelectedAreaParentItem()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUAModel.UFUAArea)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFUAModel.UFUAArea)
                    return selected;
            }

            return itemRoot;
        }

        internal TreeListNode GetSelectedSourceParentItem()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUAModel.UFUAAlarmSource)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFUAModel.UFUAAlarmSource)
                    return selected;
            }

            return null;
        }

        internal TreeListNode GetSelectedDefinitionParentItem()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUAModel.UFUAAlarmDefinition)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFUAModel.UFUAAlarmDefinition)
                    return selected;
            }

            return null;
        }

        internal UFUAModel.UFUAArea GetSelectedAreaParent()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUAModel.UFUAArea)
                return selected.Tag as UFUAModel.UFUAArea;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFUAModel.UFUAArea)
                    return selected.Tag as UFUAModel.UFUAArea;
            }

            return null;
        }

        internal UFUAModel.UFUAAlarmSource GetSelectedSourceParent()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUAModel.UFUAAlarmSource)
                return selected.Tag as UFUAModel.UFUAAlarmSource;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFUAModel.UFUAAlarmSource)
                    return selected.Tag as UFUAModel.UFUAAlarmSource;
            }

            return null;
        }

        internal UFUAModel.UFUAAlarmDefinition GetSelectedDefinitionParent(bool bSupportFlatMode = false)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUAModel.UFUAAlarmDefinition)
                return selected.Tag as UFUAModel.UFUAAlarmDefinition;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFUAModel.UFUAAlarmDefinition)
                    return selected.Tag as UFUAModel.UFUAAlarmDefinition;
            }

            if (bSupportFlatMode)
            {
                var flatDefSelected = (from object tag in gridDataControl.View.FocusedView.SelectedRows where tag.GetType() == typeof(UFUAModel.UFUAAlarmDefinition) select (UFUAModel.UFUAAlarmDefinition)tag).FirstOrDefault();
                return flatDefSelected;
            }
            return null;
        }

        internal IEnumerable<UFUAModel.UFUAAlarmDefinition> GetSelectedAlarmDefinitions()
        {
            var selected = treeListControl.GetSelectedNodes();
            List<UFUAModel.UFUAAlarmDefinition> alrDef = new List<UFUAModel.UFUAAlarmDefinition>();
            if (selected != null)
            {
                foreach (var sel in selected)
                {
                    if (sel != null)
                    {
                        using (new AsyncWaitCursor())
                        {
                            if (sel.Tag is UFUAModel.UFUAAlarmDefinition)
                                alrDef.Add(sel.Tag as UFUAModel.UFUAAlarmDefinition);
                            else if (sel.Tag is UFUAModel.UFUAAlarmSource)
                                foreach (var def in (sel.Tag as UFUAModel.UFUAAlarmSource).UFUAAlarmDefinitions)
                                    alrDef.Add(def);
                            else if (sel.Tag is UFUAModel.UFUAArea)
                                foreach (var def in (sel.Tag as UFUAModel.UFUAArea).UFUAAlarmSources.SelectMany(source => source.UFUAAlarmDefinitions))
                                    alrDef.Add(def);
                        }
                    }
                }
                return alrDef as IEnumerable<UFUAModel.UFUAAlarmDefinition>;
            }
            if (gridDataControl.SelectedItems != null)
                return gridDataControl.SelectedItems as IEnumerable<UFUAModel.UFUAAlarmDefinition>;
            return null;
        }

        DispatcherOperation dpFlatGridRefresh;
        internal void FlatGridRefresh()
        {
            if (dpFlatGridRefresh == null || dpFlatGridRefresh.Status == DispatcherOperationStatus.Aborted)
            {
                UnSubscribeSelectionChangedEvent = true;
                gridDataControl.ItemsSource = null;
                UnSubscribeSelectionChangedEvent = false;
                var action = new Action(() =>
                {
                    using (new WaitCursor())
                    {
                        try
                        {
                            UnSubscribeSelectionChangedEvent = true;
                            gridDataControl.ItemsSource = null;
                            gridDataControl.ItemsSource = Document.GetAlarmDefinitions();

                            //var selNode = treeListControl.GetSelectedNodes().FirstOrDefault();
                            //if (selNode != null)
                            //    gridDataControl.SelectedItem = selNode.Tag;

                            var selectedItems = (from c in treeListControl.GetSelectedNodes()
                                                 where c.Tag != null
                                                 select c.Tag).ToList();
                            if (selectedItems.Count > 0)
                            {
                                selectedItems.ForEach((o) => gridDataControl.SelectedItems.Add(o));
                                gridDataControl.CurrentItem = selectedItems.First();
                            }

                            foreach (var rh in expandedMasterRowHandles)
                                gridDataControl.ExpandMasterRow(rh);
                        }
                        finally
                        {
                            UnSubscribeSelectionChangedEvent = false;
                        }
                    }
                });

                //if (gridDataControl.IsVisible)
                //    action();
                //else
                {
                    dpFlatGridRefresh = Dispatcher.BeginInvokeAsynchronouslyInRender(gridDataControl, () =>
                    {
                        if (bDisposed)
                            return;
                        dpFlatGridRefresh = null;

                        action();
                    });
                }
            }
        }

        internal void EditSelectedItem()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null)
            {
                if (selected.Tag is UFUAModel.UFUAFolder)
                {
                    var folder = selected.Tag as UFUAModel.UFUAFolder;

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
                            Document.AddUndoAction(this, folder, UndoRedoAction.Changed);
                            uow.CommitChanges();
                            UpdateContextObjects();
                        }
                    }
                }
                else if (selected.Tag is UFUAModel.UFUAArea)
                {
                    var area = selected.Tag as UFUAModel.UFUAArea;

                    using (var uow = Document.BeginNestedUnitOfWork())
                    {
                        var newhsControl = new NewAlarmArea()
                        {
                            DataContext = uow.GetNestedObject(area)
                        };
                        GeneralDialogContent Dialog = new GeneralDialogContent(newhsControl)
                        {
                            Owner = this.FindParent<Window>(),
                            HelpLink = "EditArea"
                        };
                        if (Dialog.ShowDialog() == true)
                        {
                            Document.AddUndoAction(this, area, UndoRedoAction.Changed);
                            uow.CommitChanges();
                            UpdateContextObjects();
                        }
                    }
                }
                else if (selected.Tag is UFUAModel.UFUAAlarmSource)
                {
                    var source = selected.Tag as UFUAModel.UFUAAlarmSource;

                    using (var uow = Document.BeginNestedUnitOfWork())
                    {
                        var newhsControl = new NewAlarmSource()
                        {
                            DataContext = uow.GetNestedObject(source)
                        };
                        GeneralDialogContent Dialog = new GeneralDialogContent(newhsControl)
                        {
                            Owner = this.FindParent<Window>(),
                            HelpLink = "EditAlarmSource"
                        };
                        if (Dialog.ShowDialog() == true)
                        {
                            Document.AddUndoAction(this, source, UndoRedoAction.Changed);
                            uow.CommitChanges();
                            UpdateContextObjects();
                        }
                    }
                }
                else if (selected.Tag is UFUAModel.UFUAAlarmDefinition)
                {
                    var ufuaalarmdefinition = selected.Tag as UFUAModel.UFUAAlarmDefinition;
                    EditSettings(ufuaalarmdefinition);
                    FlatGridRefresh();
                }
                else if (selected.Tag is UFUAModel.UFUAAlarmThreshold)
                {
                    var alr = selected.Tag as UFUAModel.UFUAAlarmThreshold;
                    using (var uow = Document.BeginNestedUnitOfWork())
                    {
                        var newhsControl = new NewAlarmThreshold(Document)
                        {
                            DataContext = uow.GetNestedObject(alr)
                        };
                        GeneralDialogContent Dialog = new GeneralDialogContent(newhsControl)
                        {
                            Owner = this.FindParent<Window>(),
                            HelpLink = "AlarmThresholdEditor"
                        };
                        if (Dialog.ShowDialog() == true)
                        {
                            Document.AddUndoAction(this, alr, UndoRedoAction.Changed);
                            uow.CommitChanges();
                            UpdateContextObjects();
                        }
                    }
                }
            }
        }

        internal void DeleteSelectedItems()
        {
            var treeitems = new List<TreeListNode>();
            var definitions = new List<UFUAModel.UFUAAlarmDefinition>();
            var thresholds = new List<UFUAModel.UFUAAlarmThreshold>();
            var listundo = new List<IXPSimpleObject>();

            if (gridControlTab.IsSelected)
            {
                var selectedItems = gridDetailsControl.SelectedItems;
                if (selectedItems.Count == 0)
                    selectedItems = gridDataControl.SelectedItems;
                foreach (var item in selectedItems)
                {
                    if (item is UFUAModel.UFUAAlarmDefinition)
                        definitions.Add(item as UFUAModel.UFUAAlarmDefinition);
                    else if (item is UFUAModel.UFUAAlarmThreshold)
                        thresholds.Add(item as UFUAModel.UFUAAlarmThreshold);
                    if (IsUndoRedoSupported(item))
                        listundo.Add(item as IXPSimpleObject);
                    if (mapObjectToNode.ContainsKey(item))
                        treeitems.Add(mapObjectToNode[item]);
                }
            }
            else
            {
                treeitems.AddRange((from item in treeListControl.GetSelectedNodes()
                                    where item.Tag != null
                                    select item));

                listundo.AddRange((from item in treeitems
                                   where IsUndoRedoSupported(item.Tag) && 
                                   !treeitems.Contains(item.ParentNode)
                                   select item.Tag as IXPSimpleObject));
            }

            var uiMsgBox = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
            if (uiMsgBox != null)
            {
                var bShowMessage = (from item in treeitems/*.AsParallel()*/
                                    where (item.Tag is UFUAModel.UFUAAlarmDefinition &&
                                    (item.Tag as UFUAModel.UFUAAlarmDefinition).UFUAAlarmThresholds.Count > 0) ||
                                    (item.Tag is UFUAModel.UFUAAlarmSource &&
                                    (item.Tag as UFUAModel.UFUAAlarmSource).UFUAAlarmDefinitions.Count > 0) ||
                                    (item.Tag is UFUAModel.UFUAArea &&
                                    ((item.Tag as UFUAModel.UFUAArea).UFUAAlarmSources.Count > 0 ||
                                    (item.Tag as UFUAModel.UFUAArea).UFUAAreas.Count > 0))
                                    select item).ToList().Count > 0;

                if (!bShowMessage)
                {
                    bShowMessage = (from item in definitions/*.AsParallel()*/
                                    where item.UFUAAlarmThresholds.Count > 0
                                    select item).ToList().Count > 0;
                }

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
                Document.AddUndoAction(this, listundo, UndoRedoAction.Removed);

            foreach (var item in thresholds)
                RemoveAlarmFromTag(item, item.UFUATagAss, item.UFUAAlarmDefinitionRef);
            foreach (var item in definitions)
                item.Delete();

            try
            {
                treeListControl.BeginDataUpdate();
                foreach (var item in treeitems)
                    DeleteTreeItem(item);
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }

            if (gridControlTab.IsSelected)
                gridDataControl.SelectedItems.Clear();

            FlatGridRefresh();
        }

        internal void DeleteTreeItem(TreeListNode item)
        {
            try
            {
                UnSubscribeSelectionChangedEvent = true;
                using (new AsyncWaitCursor())
                {
                    //var item = addressSpaceTree.SelectedItem as TreeViewItemAdv;
                    var parent = item.ParentNode ?? itemRoot;
                    if (item.Tag is UFUAModel.UFUAAlarmDefinition)
                    {
                        var ad = item.Tag as UFUAModel.UFUAAlarmDefinition;
                        //GetSelectedParent(true).Items.Remove(item);
                        parent.Nodes.Remove(item);
                        ad.Delete();
                    }
                    else if (item.Tag is UFUAModel.UFUAAlarmSource)
                    {
                        var source = item.Tag as UFUAModel.UFUAAlarmSource;
                        //GetSelectedParent(true).Items.Remove(item);
                        parent.Nodes.Remove(item);
                        source.Delete();
                    }
                    else if (item.Tag is UFUAModel.UFUAArea)
                    {
                        var source = item.Tag as UFUAModel.UFUAArea;
                        //GetSelectedParent(true).Items.Remove(item);
                        parent.Nodes.Remove(item);
                        source.Delete();
                    }
                    else if (item.Tag is UFUAModel.UFUAAlarmThreshold)
                    {
                        var alarm = item.Tag as UFUAModel.UFUAAlarmThreshold;
                        RemoveAlarmFromTag(alarm, alarm.UFUATagAss, alarm.UFUAAlarmDefinitionRef);
                    }
                }
            }
            finally
            {
                UnSubscribeSelectionChangedEvent = false;
            }

            FlatGridRefresh();
        }

        void RemoveAlarmFromTag(UFUAModel.UFUAAlarmThreshold alarm, UFUAModel.UFUATag tag, UFUAModel.UFUAAlarmDefinition definition)
        {
            if (mapObjectToNode.ContainsKey(alarm))
                mapObjectToNode[alarm].ParentNode.Nodes.Remove(mapObjectToNode[alarm]);

            if (tag == null)
                tag = alarm.UFUATagAss;
            if (tag == null || !tag.UFUAAlarmThresholds.Contains(alarm))
                return;

            tag.UFUAAlarmThresholds.Remove(alarm);
            alarm.Delete();
            tag.NotifyPropertyChanged("UFUAAlarmThresholds");
        }

        internal void AddAlarmToTags(List<TagIdentifier> tags, UFUAModel.UFUAAlarmDefinition targetDef = null, TreeListNode targetNode = null)
        {
            var nodesToSelect = new List<TreeListNode>();
            var thresholds = new List<UFUAModel.UFUAAlarmThreshold>();
            try
            {
                UnSubscribeSelectionChangedEvent = true;
                treeListControl.BeginDataUpdate();
                using (new AsyncWaitCursor())
                {
                    if (targetDef == null)
                        targetDef = GetSelectedDefinitionParent(true);
                    if (targetNode == null)
                        targetNode = GetSelectedDefinitionParentItem();

                    if (targetDef == null)
                        return;

                    var listundo = new List<IXPSimpleObject>();

                    foreach (var tagDef in tags)
                    {
                        var reference = tagDef.TagReference as UFUAModel.TagEntityReference;
                        if (reference != null)
                        {
                            var tag = Document.FindTagByEntityReference(reference);
                            if (tag != null)
                            {
                                var bExists = (from UFUAModel.UFUAAlarmThreshold th in tag.UFUAAlarmThresholds where th.IsValid && th.UFUAAlarmDefinitionRef == targetDef select th).FirstOrDefault() != null;
                                if (bExists)
                                    continue;
                                var list = Document.GetNewThresholdList(targetDef, tag, AssignAlarmType.Single, String.Empty);
                                list.ForEach((thres) => 
                                {
                                    thresholds.Add(thres);
                                    if (targetNode != null)
                                    {
                                        var item = AddTreeItemThreshold(thres, targetNode, tag);
                                        if (item != null)
                                            nodesToSelect.Add(item);
                                    }
                                    else
                                    {
                                        AssignThreshold(thres, tag, targetDef);
                                    }
                                });
                            }
                        }
                    }

                    if (thresholds.Count > 0)
                    {
                        listundo.AddRange(thresholds);
                        FlatGridRefresh();
                        Document.NeedsSave = true;
                    }

                    if (listundo.Count > 0)
                        Document.AddUndoAction(this, listundo, UndoRedoAction.Added);
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
                if (nodesToSelect.Count > 0)
                    treeListControl.SelectNodes(nodesToSelect, true, true);
                UnSubscribeSelectionChangedEvent = false;
                if (nodesToSelect.Count > 0)
                    ForceSelectionChangedEvent();
            }
        }

        internal void CopySelectedToClipboard()
        {
            var listareas = new List<UFUAModel.UFUAArea>();
            var listsources = new List<UFUAModel.UFUAAlarmSource>();
            var listalrdef = new List<UFUAModel.UFUAAlarmDefinition>();
            foreach (var item in treeListControl.GetSelectedNodes())
            {
                if (item.Tag is UFUAModel.UFUAArea)
                    listareas.Add(item.Tag as UFUAModel.UFUAArea);
                else if (item.Tag is UFUAModel.UFUAAlarmSource)
                    listsources.Add(item.Tag as UFUAModel.UFUAAlarmSource);
                else if (item.Tag is UFUAModel.UFUAAlarmDefinition)
                    listalrdef.Add(item.Tag as UFUAModel.UFUAAlarmDefinition);
            }

            Document.CleanClipbaord();
            Document.CopyListAlarmAreasToClipbaord(listareas);
            Document.CopyListAlarmSourcesToClipbaord(listsources);
            Document.CopyListAlarmDefinitionsToClipbaord(listalrdef);
            Document.CheckClipbaord();
        }

        private void OnMasterRowExpanded(object sender, RowEventArgs e)
        {
            if (!expandedMasterRowHandles.Contains(e.RowHandle))
                expandedMasterRowHandles.Add(e.RowHandle);
        }

        private void OnMasterRowCollapsed(object sender, RowEventArgs e)
        {
            if (expandedMasterRowHandles.Contains(e.RowHandle))
                expandedMasterRowHandles.Remove(e.RowHandle);
        }

        internal void PasteFromClipboard()
        {
            List<TreeListNode> pastedNodes = new List<TreeListNode>();
            try
            {
                var parentArea = GetSelectedAreaParentItem();
                var expandedArea = parentArea != null && (parentArea.IsExpanded || parentArea.Nodes.Count > 0 && parentArea.Nodes[0].Tag != TreeListControlHelper.DummyNode);

                var parentSource = GetSelectedSourceParentItem();
                var expandedSource = parentSource != null && (parentSource.IsExpanded || parentSource.Nodes.Count > 0 && parentSource.Nodes[0].Tag != TreeListControlHelper.DummyNode);

                treeListControl.ClearSelection();
                UnSubscribeSelectionChangedEvent = true;
                treeListControl.BeginDataUpdate();

                UFUAModel.UFUAArea area = null;
                if (parentArea != null && parentArea.Tag is UFUAModel.UFUAArea)
                    area = parentArea.Tag as UFUAModel.UFUAArea;

                UFUAModel.UFUAAlarmSource source = null;
                if (parentSource != null && parentSource.Tag is UFUAModel.UFUAAlarmSource)
                    source = parentSource.Tag as UFUAModel.UFUAAlarmSource;

                var listareas = new List<UFUAModel.UFUAArea>();
                listareas = Document.PasteClipboardAlarmAreas(area);
                treeListControl.AddDummyNodeIfNeeded(parentArea, NeedToBeExpanded);

                pastedNodes.AddRange(treeListControl.AddGetNodesList(itemRoot, listareas, expandedArea, AddTreeItem, parentArea));

                var listsources = new List<UFUAModel.UFUAAlarmSource>();
                if (area != null)
                {
                    listsources = Document.PasteClipboardAlarmSources(area);
                    treeListControl.AddDummyNodeIfNeeded(parentArea, NeedToBeExpanded);
                    pastedNodes.AddRange(treeListControl.AddGetNodesList(itemRoot, listsources, expandedArea, AddTreeItem, parentArea));
                }

                var listalrdef = new List<UFUAModel.UFUAAlarmDefinition>();
                if (source != null)
                {
                    listalrdef = Document.PasteClipboardAlarmDefinitions(source);
                    treeListControl.AddDummyNodeIfNeeded(parentSource, NeedToBeExpanded);
                    pastedNodes.AddRange(treeListControl.AddGetNodesList(itemRoot, listalrdef, expandedSource, AddTreeItem, parentSource));
                }

                if (listareas.Count > 0 || listsources.Count > 0 || listalrdef.Count > 0)
                {
                    FlatGridRefresh();

                    // add list to undo manager
                    var listundo = new List<IXPSimpleObject>();
                    listareas.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
                    listsources.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
                    listalrdef.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
                    Document.AddUndoAction(this, listundo, UndoRedoAction.Added);
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
                if (pastedNodes.Count > 0)
                    treeListControl.SelectNodes(pastedNodes, true, true);
                UnSubscribeSelectionChangedEvent = false;
                if (pastedNodes.Count > 0)
                    ForceSelectionChangedEvent();
            }
        }

        internal void AddAlarmArea(UFUAModel.UFUAArea aa)
        {
            Document.AddUndoAction(this, aa, UndoRedoAction.Added);
            var item = AddTreeItem(aa, GetSelectedAreaParentItem());
            //treeListControl.ClearSelection();
            treeListControl.SelectNode(item);
        }

        internal void AddAlarmSource(UFUAModel.UFUAAlarmSource aa)
        {
            Document.AddUndoAction(this, aa, UndoRedoAction.Added);
            var item = AddTreeItem(aa, GetSelectedAreaParentItem());
            //treeListControl.ClearSelection();
            treeListControl.SelectNode(item);
        }

        internal void AddAlarmDefinition(UFUAModel.UFUAAlarmDefinition aa)
        {
            Document.AddUndoAction(this, aa, UndoRedoAction.Added);
            var item = AddTreeItem(aa, GetSelectedSourceParentItem());
            //treeListControl.ClearSelection();
            treeListControl.SelectNode(item);
            FlatGridRefresh();
            gridDataControl.SelectedItem = aa;
        }

        void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            if (e.Source.DataControl == treeListControl)
            {
                e.Handled = true;
                var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
                if (selected != null)
                {
                    if (isPopup && selected.Tag is UFUAModel.UFUAAlarmDefinition)
                    {
                        var wnd = this.FindParent<Window>();
                        if (wnd != null)
                        {
                            wnd.DialogResult = true;
                            wnd.Close();
                        }
                    }
                    else if (!isPopup)
                    {
                        if (Document.EditorManagerComponent.PropertyControl != null)
                            Document.EditorManagerComponent.PropertyControl.Activate();
                        else
                            EditSelectedItem();
                    }
                }
            }
            else if (e.Source.DataControl == gridDataControl)
            {
                e.Handled = true;
                if (!isPopup && Document.EditorManagerComponent.PropertyControl != null)
                    Document.EditorManagerComponent.PropertyControl.Activate();
                else
                {
                    var ufuaalarmdefinition = gridDataControl.SelectedItem as UFUAModel.UFUAAlarmDefinition;
                    EditSettings(ufuaalarmdefinition);
                    FlatGridRefresh();
                }
            }
        }

        private void EditSettings(UFUAModel.UFUAAlarmDefinition ufuaalarmdefinition)
        {
            if (ufuaalarmdefinition == null)
                return;

            using (var uow = Document.BeginNestedUnitOfWork())
            {
                var newAlarmDefinitionControl = new NewAlarmDefinition(Document)
                {
                    DataContext = uow.GetNestedObject(ufuaalarmdefinition)
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newAlarmDefinitionControl)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "EditAlarmDefinition"
                };
                if (Dialog.ShowDialog() == true)
                {
                    Document.AddUndoAction(this, ufuaalarmdefinition, UndoRedoAction.Changed);
                    uow.CommitChanges();
                    UpdateContextObjects();
                }
            }
        }

        private void OnHiddenEditor(object sender, EditorEventArgs e)
        {
            Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(((DataViewBase)sender).DataControl.SelectedItem);
        }

        #endregion

        #region Undo/Redo

        protected override bool IsUndoRedoSupported(object obj)
        {
            if (obj is UFUAModel.UFUAAlarmThreshold)
            {
                var item = (obj as UFUAModel.UFUAAlarmThreshold);
                if (item.UFUATagAss != null && (item.UFUATagAss.IsSubPrototypeMember || !item.UFUATagAss.UseShared.Value))
                    return true;
            }
            if (obj is UFUAModel.UFUATag)
            {
                var item = (obj as UFUAModel.UFUATag);
                if (!item.IsSubPrototypeMember || !item.UseShared.Value)
                    return true;
            }
            else if (obj is IXPSimpleObject)
                return true;

            return false;
        }

        internal void UndoAction()
        {
            UndoRedoAction action;
            var list = Document.UndoAction(this, out action);
            bool selectionchanged = false;
            switch (action)
            {
                case UndoRedoAction.Added:
                    {
                        if (list.Count > 0)
                        {
                            try
                            {
                                treeListControl.ClearSelection();
                                UnSubscribeSelectionChangedEvent = true;
                                treeListControl.BeginDataUpdate();
                                var listundo = new List<IXPSimpleObject>();
                                foreach (var obj in list)
                                {
                                    if (mapObjectToNode.ContainsKey(obj.Source))
                                    {
                                        var parent = mapObjectToNode[obj.Source].ParentNode;
                                        var item = treeListControl.GetTreeItem(obj.Source, parent);
                                        if (item != null)
                                            DeleteTreeItem(item);
                                    }
                                }
                                if (listundo.Count > 0)
                                    Document.AddUndoAction(this, listundo, UndoRedoAction.Removed);
                                FlatGridRefresh();
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
                case UndoRedoAction.Changed:
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
                                    if (mapObjectToNode.ContainsKey(obj.Source))
                                    {
                                        var parent = mapObjectToNode[obj.Source].ParentNode;
                                        var item = treeListControl.GetTreeItem(obj.Source, parent);
                                        if (item != null)
                                        {
                                            selectionchanged = true;
                                            treeListControl.SelectNode(item);
                                        }
                                    }
                                }
                                FlatGridRefresh();
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
                case UndoRedoAction.Removed:
                    {
                        if (list.Count > 0)
                        {
                            var items = new List<TreeListNode>();
                            try
                            {
                                treeListControl.ClearSelection();
                                UnSubscribeSelectionChangedEvent = true;
                                treeListControl.BeginDataUpdate();
                                foreach (var obj in list)
                                {
                                    TreeListNode parent = null;
                                    if (obj.Source is UFUAModel.UFUAAlarmThreshold)
                                    {
                                        var thres = obj.Source as UFUAModel.UFUAAlarmThreshold;
                                        if (thres.UFUAAlarmDefinitionRef != null &&
                                            mapObjectToNode.ContainsKey(thres.UFUAAlarmDefinitionRef))
                                            parent = mapObjectToNode[thres.UFUAAlarmDefinitionRef];
                                    }
                                    else
                                    {
                                        parent = itemRoot;
                                        var parentTag = Document.GetParentObject(obj);
                                        if (parentTag != null && mapObjectToNode.ContainsKey(parentTag))
                                            parent = mapObjectToNode[parentTag];
                                    }

                                    if (parent != null)
                                    {
                                        var item = AddTreeItem(obj.Source, parent);
                                        if (item != null)
                                        {
                                            selectionchanged = true;
                                            items.Add(item);
                                        }
                                    }
                                }
                                FlatGridRefresh();
                            }
                            finally
                            {
                                treeListControl.EndDataUpdate();
                                if (items.Count > 0)
                                    treeListControl.SelectNodes(items, true, true);
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
            UndoRedoAction action;
            var list = Document.RedoAction(this, out action);
            bool selectionchanged = false;
            switch (action)
            {
                case UndoRedoAction.Removed:
                    {
                        if (list.Count > 0)
                        {
                            try
                            {
                                treeListControl.ClearSelection();
                                UnSubscribeSelectionChangedEvent = true;
                                treeListControl.BeginDataUpdate();
                                var listundo = new List<IXPSimpleObject>();
                                foreach (var obj in list)
                                {
                                    if (mapObjectToNode.ContainsKey(obj.Source))
                                    {
                                        var parent = mapObjectToNode[obj.Source].ParentNode;
                                        var item = treeListControl.GetTreeItem(obj.Source, parent);
                                        if (item != null)
                                            DeleteTreeItem(item);
                                    }
                                }
                                if (listundo.Count > 0)
                                    Document.AddUndoAction(this, listundo, UndoRedoAction.Removed);
                                FlatGridRefresh();
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
                case UndoRedoAction.Changed:
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
                                    if (mapObjectToNode.ContainsKey(obj.Source))
                                    {
                                        var parent = mapObjectToNode[obj.Source].ParentNode;
                                        var item = treeListControl.GetTreeItem(obj.Source, parent);
                                        if (item != null)
                                        {
                                            selectionchanged = true;
                                            treeListControl.SelectNode(item);
                                        }
                                    }
                                }
                                FlatGridRefresh();
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
                case UndoRedoAction.Added:
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
                                    TreeListNode parent = null;
                                    if (obj.Source is UFUAModel.UFUAAlarmThreshold)
                                    {
                                        var thres = obj.Source as UFUAModel.UFUAAlarmThreshold;
                                        if (mapObjectToNode.ContainsKey(thres.UFUAAlarmDefinitionRef))
                                            parent = mapObjectToNode[thres.UFUAAlarmDefinitionRef];
                                    }
                                    else
                                    {
                                        parent = itemRoot;
                                        var parentTag = Document.GetParentObject(obj);
                                        if (parentTag != null && mapObjectToNode.ContainsKey(parentTag))
                                            parent = mapObjectToNode[parentTag];
                                    }

                                    if (parent != null)
                                    {
                                        var item = AddTreeItem(obj.Source, parent);
                                        if (item != null)
                                        {
                                            selectionchanged = true;
                                            treeListControl.SelectNode(item);
                                        }
                                    }
                                }
                                FlatGridRefresh();
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

        bool NeedToBeExpanded(TreeListNode item)
        {
            if (item.Tag is UFUAModel.UFUAArea)
            {
                var area = (UFUAModel.UFUAArea)item.Tag;
                return Document.GetAlarmAreas(area).Count > 0 || Document.GetSourceCollection(area).Count > 0;
            }
            else if (item.Tag is UFUAModel.UFUAAlarmSource)
                return ((UFUAModel.UFUAAlarmSource)item.Tag).UFUAAlarmDefinitions.Count > 0;
            else if (item.Tag is UFUAModel.UFUAAlarmDefinition)
                return (from t in ((UFUAModel.UFUAAlarmDefinition)item.Tag).UFUAAlarmThresholds where t.IsValid orderby t.UFUATagAss.Name select t).FirstOrDefault() != null;

            return false;
        }

        List<TreeListNode> itemsToRefresh;
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

        void RefreshAsync(TreeListNode item)
        {
            if (itemsToRefresh == null || !itemsToRefresh.Contains(item))
                RefreshAsync(new List<TreeListNode>() { item });
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

        #endregion

        #region IDisposable
        bool bDisposed;
        public override void OnDispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            if (itemRoot != null)
                ClearObjectMapNode(itemRoot);

            Document.EditorManagerComponent.Workspace.ContextObject = null;
            Document.ChangedDocument -= Document_ChangedDocument;

            if (dpFlatGridRefresh != null && dpFlatGridRefresh.Status != DispatcherOperationStatus.Aborted &&
                dpFlatGridRefresh.Status != DispatcherOperationStatus.Completed)
            {
                dpFlatGridRefresh.Abort();
            }

            // gridDataControl.Model.Dispose();
            try
            {
                gridDataControl.Dispose();
            }
            catch (Exception ex)
            {
                
            }

            mapAlarmToTag.Clear();
        }

        #region IEditableObject
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

        public void EndEdit()
        {
            if (!bEditingUow)
                return;

            bool bResult = true;
            try
            {
                if (Document.UowContext != null)
                {
                    bResult = Document.UowContext.TryCommitChanges(UFUAServerDocument.logGeneral);
                    Document.UowContext.BeforeFlushChanges -= uowContext_BeforeFlushChanges;
                    if (bResult)
                        Document.NeedsSave = true;
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

        private void uowContext_BeforeFlushChanges(object sender, DevExpress.Xpo.SessionManipulationEventArgs e)
        {
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
                        list.Add(parent as DevExpress.Xpo.IXPSimpleObject);
                }

                if (list.Count > 0)
                    Document.AddUndoAction(this, list, UndoRedoAction.Changed);
            }
        }
        #endregion

        #endregion

        #region Drag & Drop
        private void TreeListControl_DragStart(object sender, StartRecordDragEventArgs e)
        {
            e.AllowDrag = false;
            e.Handled = true;
            return;
        }

        private void treeListControl_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(RecordDragDropData)))
                return;

            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
            uint forbidden = 0;

            var data = e.Data.GetData(typeof(RecordDragDropData)) as RecordDragDropData;
            foreach (var v in data.Records)
            {
                IDocumentManager model = null;
                if (v is IDocumentManager)
                    model = v as IDocumentManager;
                else if (v is TreeItemControl)
                    model = (v as TreeItemControl).TreeItemInnerObject as IDocumentManager;
                if (model == null)
                {
                    forbidden++;
                    continue;
                }

                if (model.BrowsableContent is UFUAModel.UFUATag)
                {
                    var tag = (UFUAModel.UFUATag)model.BrowsableContent;
                    if (!CanAssignAlarm(tag))
                        forbidden++;
                }
            }

            if (forbidden == data.Records.Count())
                e.Effects = DragDropEffects.None;
        }

        private void addressSpaceTree_PreviewQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (!e.EscapePressed)
                return;

            e.Action = DragAction.Cancel;
            e.Handled = true;
        }

        private void TreeListControl_DragEnd(object sender, DevExpress.Xpf.Core.DropRecordEventArgs e)
        {
            e.Handled = true;
            var targetAlarmDef = (e.TargetRecord as TreeItemControl)?.TreeItemInnerObject as UFUAModel.UFUAAlarmDefinition;
            var targetTreeNode = treeListView.GetNodeByContent(e.TargetRecord);
            if (targetAlarmDef != null && targetTreeNode != null)
            {
                using (var Cursor = new WaitCursor())
                {
                    //Dispatcher.InvokeIfRequired((Action)delegate
                    //{
                        var data = e.Data.GetData(typeof(RecordDragDropData)) as RecordDragDropData;
                        if (data == null)
                            return;

                        var listIdentifiers = new List<TagIdentifier>();
                        foreach (var v in data.Records)
                        {
                            IDocumentManager model = null;
                            if (v is IDocumentManager)
                                model = v as IDocumentManager;
                            else if (v is TreeItemControl)
                                model = (v as TreeItemControl).TreeItemInnerObject as IDocumentManager;
                            if (model == null)
                                continue;

                            var entity = model.DragContent as OPCUAViewModel.OPCUAEntityReference;
                            if (entity != null)
                            {
                                UFUAModel.UFUATag member;
                                var instance = Document.FindTagByNodeId(entity.ResolvedNodeId, out member);
                                if ((instance != null || member != null) && CanAssignAlarm(member ?? instance))
                                {
                                    var tagRef = new UFUAModel.TagEntityReference(member == null ? instance.NodeId : member.NodeId, entity.RelativePath, entity.ResolvedNodeId, entity.HumanReadable);
                                    listIdentifiers.Add(new TagIdentifier(tagRef));
                                }
                            }
                        }

                        if (listIdentifiers.Count > 0)
                            AddAlarmToTags(listIdentifiers, targetAlarmDef, targetTreeNode);
                    //}, DispatcherPriority.ApplicationIdle);
                }
            }
        }
        #endregion

        class ParentElement
        {
            #region Declarations
            TreeListNode node;
            public UFUAModel.UFUAAlarmDefinition Definition;
            public UFUAModel.UFUATag TagAss;
            #endregion
            #region Public Props
            public TreeListNode Node
            {
                get
                {
                    return node;
                }
                set
                {
                    if (node != value)
                    {
                        node = value;
                        Definition = node.Tag as UFUAModel.UFUAAlarmDefinition;
                    }
                }
            }
            #endregion
            #region Ctor
            public ParentElement()
            {

            }
            #endregion
        }
    }
}

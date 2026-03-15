using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UFUAEditor.Document;
using Utilities;
using Utilities.WPF;
using UFUAEditor.ComponentService;
using DevExpress.Xpo;
using UFInterfaces.Editors;
using System.ComponentModel;
using WPFUtilities;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using DevExpress.Xpf.Core;
using UFUAEditor.Helpers;
using UIMsgBoxAlertService.ComponentService;
using DevExpress.Data.TreeList;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Editors.Settings;
using ViewModelLib;
using SelectionMode = UFInterfaces.Editors.SelectionMode;
using UFInterfaces;
using DevExpress.Data.Filtering;
using DocumentManager.ComponentService;
using System.Windows.Threading;
using Utilities.Xpo.UndoRedo;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UFUAModel;
using DataLoggerModel;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for AddressSpace.xaml
    /// </summary>
    public partial class AddressSpace : TreeViewEditorHelper, IAddressSpaceControl, IEditableObject, INotifyPropertyChanged, ISelectEntityReference
    {
        #region Declarations

        TreeListNode itemRoot;
        BitmapImage openFolderImg;
        BitmapImage closedFolderImg;
        bool bLoaded;
        bool bSearching = false;

        readonly String typeName;

        internal AssignAlarmDefinition alarmList = null;

        bool isPopup;
        bool bDirtySelection = false;
        bool bCanDeleteAlarms = false;
        List<TreeListNode> selectedItems;
        public event PropertyChangedEventHandler PropertyChanged;
        static int maxItems = Properties.Settings.Default.MaxItemsInTree;
        ContainsCaseSensitive customSearchOperator;
        TagsImportProgressViewModel tagsImportProgressViewModel;

        #endregion

        #region Properties
        string searchResultsCount;
        public string SearchResultsCount
        {
            get
            {
                return searchResultsCount;
            }
            set
            {
                if (searchResultsCount != value)
                {
                    searchResultsCount = value;
                    OnPropertyChanged("SearchResultsCount");
                }
            }
        }
        bool isSearchCaseSensitive = true;
        public bool IsSearchCaseSensitive
        {
            get
            {
                return isSearchCaseSensitive;
            }
            set
            {
                if (isSearchCaseSensitive != value)
                {
                    isSearchCaseSensitive = value;
                    if (customSearchOperator != null)
                    {
                        customSearchOperator.IsCaseSensitive = isSearchCaseSensitive;
                        if (tableView.SearchControl != null &&
                            !String.IsNullOrEmpty(tableView.SearchControl.SearchText))
                        {
                            gridDataControl.RefreshData();
                        }
                    }
                    OnPropertyChanged("IsSearchCaseSensitive");
                }
            }
        }
        #endregion

        public AddressSpace(UFUAServerDocument doc, bool bPopup = false)
        {
            InitializeComponent();
            SetTabsContent(treeListControl, gridDataControl);
            customSearchOperator = new ContainsCaseSensitive() { IsCaseSensitive = isSearchCaseSensitive };
            CriteriaOperator.RegisterCustomFunction(customSearchOperator);
            Tag = Document = doc;
            isPopup = bPopup;

            Document.CreateUndoRedoHelper();
            Document.ChangedDocument += Document_ChangedDocument;

            //gridDataControl.GroupDropAreaText
            var childrenSelector = gridDataControl.TryFindResource("childrenSelector") as CustomChildrenSelector;
            childrenSelector.Document = Document;
            gridDataControl.ItemsSource = Document.GetFlatTagCollection();
            tabAddressSpaceTabControl.SelectionChanging += (s, e) =>
            {
                if (e.NewSelectedItem == gridControlTab &&
                    dpFlatGridRefresh != null && dpFlatGridRefresh.Status == DispatcherOperationStatus.Aborted)
                    FlatGridRefresh();
            };

            if (isPopup)
            {
                typeName = GetType().Name;
                toolbar.Visibility = System.Windows.Visibility.Visible;
                // context menu is visible only when tag browser has been opened like popup
                // contextMenu.Visibility = System.Windows.Visibility.Visible;

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

                gridDataControl.CustomRowFilter += (s, e) =>
                {
                    if (FilterType == FilterType.None)
                        return;

                    var row = gridDataControl.GetRowByListIndex(e.ListSourceRowIndex);
                    if (row is UFUAModel.UFUATag)
                    {
                        var ufuatag = row as UFUAModel.UFUATag;
                        if (FilterType == FilterType.Historians)
                            e.Visible = !String.IsNullOrEmpty(ufuatag.HistorianSettings) || ufuatag.ModelType == UFUAModel.ModelType.ObjectType;
                    }

                    if (!e.Visible)
                        e.Handled = true;
                };

                Loaded += (o, e) =>
                {
                    //var childrenSelector = gridDataControl.TryFindResource("childrenSelector") as CustomChildrenSelector;
                    //childrenSelector.Document = Document;
                    //gridDataControl.ItemsSource = Document.GetFlatTagCollection();

                    var wnd = this.FindParent<Window>();
                    if (wnd != null)
                    {
                        wnd.Activated += (s, c) =>
                        {
                            Document.CopyWinClipboardToInMemoryData();
                        };

                        wnd.Closing += (s, c) =>
                        {
                            var selected = selectedItems == null ? null : new List<TreeListNode>(selectedItems);
                            selectedItems = null;

                            List<UFUAModel.UFUATag> ufuatags = new List<UFUAModel.UFUATag>();
                            var mapList = new Dictionary<UFUAModel.UFUATag, List<UFUAModel.UFUATag>>();
                            if (tabAddressSpaceTabControl.SelectedItem == gridControlTab)
                            {
                                foreach (var item in gridDataControl.SelectedItems)
                                {
                                    if (item is UFUAModel.UFUATag)
                                        ufuatags.Add((UFUAModel.UFUATag)item);
                                }
                            }
                            else
                            {
                                if (selected == null)
                                    selected = treeListControl.GetSelectedNodes().ToList();

                                foreach (var sel in selected)
                                {
                                    UFUAModel.UFUATag ufuatag = null;
                                    if (sel != null && sel.Tag is UFUAModel.UFUATag)
                                    {
                                        ufuatag = sel.Tag as UFUAModel.UFUATag;
                                        var parent = GetParentTag(sel);
                                        while (parent != null)
                                        {
                                            if (!mapList.ContainsKey(ufuatag))
                                                mapList.Add(ufuatag, new List<UFUAModel.UFUATag>());
                                            mapList[ufuatag].Add(parent.Tag as UFUAModel.UFUATag);
                                            parent = GetParentTag(parent);
                                        }
                                    }
                                    if (ufuatag != null)
                                        ufuatags.Add(ufuatag);
                                }
                            }

                            if (ufuatags.Count > 0)
                            {
                                var referencesList = new List<TagIdentifier>();
                                foreach (var ufuatag in ufuatags)
                                {
                                    List<UFUAModel.UFUATag> list = null;
                                    if (mapList.ContainsKey(ufuatag))
                                        list = mapList[ufuatag];
                                    var entity = Document.GetTagOPCUAEntityReference(ufuatag, list, refresh: true);
                                    UpdateReferences(entity, ufuatag);
                                    if (SelectionType == SelectionType.TagEntityReference)
                                        referencesList.Add(new TagIdentifier(new UFUAModel.TagEntityReference(ufuatag.NodeId, entity.RelativePath, entity.ResolvedNodeId, entity.HumanReadable)));
                                    else
                                        referencesList.Add(new TagIdentifier(entity));
                                }
                                SelectedReference = null;
                                SelectedReferences = new List<object>(referencesList);
                            }
                            else
                            {
                                SelectedReference = null;
                                SelectedReferences = null;
                            }
                        };
                    }

                    if (!bLoaded)
                    {
                        bLoaded = true;
                        InitializeAddressSpace();
                    }
                    else
                        UpdateSelectedItem();
                };
            }
            else
            {
                Loaded += (o, e) =>
                {
                    //var childrenSelector = gridDataControl.TryFindResource("childrenSelector") as CustomChildrenSelector;
                    //childrenSelector.Document = Document;
                    //gridDataControl.ItemsSource = Document.GetFlatTagCollection(); 
                    if (!bLoaded)
                    {
                        bLoaded = true;
                        InitializeAddressSpace();
                    }
                };
            }
        }

        void Document_ChangedDocument(object sender, ChangedDocumentEvent e)
        {
            if (e.Source == this)
                return;

            var changedObjects = (from object c in e.ChangedObjects.AsParallel()
                                  where c is UFUAModel.UFUATag ||
                                  c is UFUAModel.UFUAFolder ||
                                  c is UFUAModel.UFUAArea ||
                                  c is UFUAModel.UFUAAlarmSource ||
                                  c is UFUAModel.UFUAAlarmDefinition ||
                                  c is UFUAModel.UFUAAlarmThreshold ||
                                  c is UFUAModel.UFUAView
                                  select c).ToList();

            if (changedObjects.Count > 0)
            {
                try
                {
                    UnSubscribeSelectionChangedEvent = true;
                    treeListControl.BeginDataUpdate();

                    foreach (var changedObject in changedObjects)
                    {
                        if (changedObject is UFUAModel.UFUATag &&
                            (changedObject as UFUAModel.UFUATag).IsPrototypeMember &&
                            !(changedObject as UFUAModel.UFUATag).IsSubPrototypeMember)
                        {
                            var tags = Document.GetObjectTagCollection((changedObject as UFUAModel.UFUATag).PrototypeReference);
                            if (tags.Count > 0)
                            {
                                foreach (var tag in tags)
                                {
                                    if (mapObjectToNode.ContainsKey(tag))
                                        RefreshAsync(mapObjectToNode[tag]);
                                }

                                FlatGridRefresh();
                            }
                        }
                        else if (changedObject is UFUAModel.UFUAFolder &&
                            (changedObject as UFUAModel.UFUAFolder).IsPrototypeMember &&
                            !(changedObject as UFUAModel.UFUAFolder).IsSubPrototypeMember)
                        {
                            var tags = Document.GetObjectTagCollection((changedObject as UFUAModel.UFUAFolder).PrototypeReference);
                            if (tags.Count > 0)
                            {
                                foreach (var tag in tags)
                                {
                                    if (mapObjectToNode.ContainsKey(tag))
                                        RefreshAsync(mapObjectToNode[tag]);
                                }

                                FlatGridRefresh();
                            }
                        }

                        if (e.ChangedType == ChangedType.removed)
                        {
                            if (changedObject is UFUAModel.UFUAArea || changedObject is UFUAModel.UFUAAlarmSource || changedObject is UFUAModel.UFUAAlarmDefinition)
                            {
                                var thresholds = new List<UFUAModel.UFUAAlarmThreshold>();
                                if (changedObject is UFUAModel.UFUAArea)
                                    thresholds.AddRange(Document.GetThresholdsList(changedObject as UFUAModel.UFUAArea));
                                else if (changedObject is UFUAModel.UFUAAlarmSource)
                                    thresholds.AddRange(Document.GetThresholdsList(changedObject as UFUAModel.UFUAAlarmSource));
                                else if (changedObject is UFUAModel.UFUAAlarmDefinition)
                                    thresholds.AddRange((changedObject as UFUAModel.UFUAAlarmDefinition).UFUAAlarmThresholds);
                                foreach (var threshold in thresholds)
                                {
                                    if (threshold.UFUATagAss != null && threshold.UFUATagAss.IsPrototypeMember && !threshold.UFUATagAss.IsSubPrototypeMember)
                                    {
                                        var tags = Document.FindSubPrototypeMembersByNodeId(threshold.UFUATagAss.NodeId);
                                        if (tags.Count > 0)
                                        {
                                            foreach (var tag in tags)
                                            {
                                                if (mapObjectToNode.ContainsKey(tag))
                                                    RefreshAsync(mapObjectToNode[tag].ParentNode);
                                            }

                                            FlatGridRefresh();
                                        }
                                    }
                                    else if (mapObjectToNode.ContainsKey(threshold))
                                    {
                                        var parent = mapObjectToNode[threshold].ParentNode;
                                        parent.Nodes.Remove(mapObjectToNode[threshold]);

                                        FlatGridRefresh();
                                    }
                                }
                            }
                            else if (changedObject is UFUAModel.UFUAAlarmThreshold)
                            {
                                if ((changedObject as UFUAModel.UFUAAlarmThreshold).UFUATagAss != null)
                                {
                                    var ufuatag = (changedObject as UFUAModel.UFUAAlarmThreshold).UFUATagAss;
                                    if (ufuatag.IsPrototypeMember && !ufuatag.IsSubPrototypeMember)
                                    {
                                        var tags = Document.FindSubPrototypeMembersByNodeId(ufuatag.NodeId);
                                        if (tags.Count > 0)
                                        {
                                            foreach (var tag in tags)
                                            {
                                                if (mapObjectToNode.ContainsKey(tag))
                                                    RefreshAsync(mapObjectToNode[tag].ParentNode);
                                            }

                                            FlatGridRefresh();
                                        }
                                    }
                                    else if (mapObjectToNode.ContainsKey(changedObject))
                                    {
                                        var parent = mapObjectToNode[changedObject].ParentNode;
                                        parent.Nodes.Remove(mapObjectToNode[changedObject]);

                                        FlatGridRefresh();
                                    }
                                }
                                else if (mapObjectToNode.ContainsKey(changedObject))
                                {
                                    var parent = mapObjectToNode[changedObject].ParentNode;
                                    parent.Nodes.Remove(mapObjectToNode[changedObject]);

                                    FlatGridRefresh();
                                }
                            }
                            else if (changedObject is UFUAModel.UFUAView)
                            {
                                var view = changedObject as UFUAModel.UFUAView;
                                foreach (var ufuatag in view.UFUATags)
                                {
                                    if (ufuatag.IsSubPrototypeMember)
                                    {
                                        var tags = Document.FindSubPrototypeMembersByNodeId(ufuatag.NodeId);
                                        foreach (var tag in tags)
                                        {
                                            if (mapObjectToNode.ContainsKey(tag))
                                            {
                                                var p = mapObjectToNode[tag];
                                                var found = (from c in p.Nodes where c.Tag == changedObject select c).ToList();
                                                foreach (var node in found)
                                                    p.Nodes.Remove(node);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var tag = Document.FindTagByNodeId(ufuatag.NodeId);
                                        if (mapObjectToNode.ContainsKey(tag))
                                        {
                                            var p = mapObjectToNode[tag];
                                            var found = (from c in p.Nodes where c.Tag == changedObject select c).ToList();
                                            foreach (var node in found)
                                                p.Nodes.Remove(node);
                                        }
                                    }
                                }
                            }
                            else if (mapObjectToNode.ContainsKey(changedObject))
                            {
                                var parent = mapObjectToNode[changedObject].ParentNode;
                                parent.Nodes.Remove(mapObjectToNode[changedObject]);

                                FlatGridRefresh();
                            }
                        }
                        else if (e.ChangedType == ChangedType.added)
                        {
                            TreeListNode parent = null;
                            if (changedObject is UFUAModel.UFUATag && !(changedObject as UFUAModel.UFUATag).IsPrototypeMember)
                            {
                                if ((changedObject as UFUAModel.UFUATag).UFUAFolder != null)
                                {
                                    var folder = (changedObject as UFUAModel.UFUATag).UFUAFolder;
                                    if (mapObjectToNode.ContainsKey(folder))
                                        parent = mapObjectToNode[folder];
                                }
                                else
                                    parent = itemRoot;
                            }
                            else if (changedObject is UFUAModel.UFUAFolder && !(changedObject as UFUAModel.UFUAFolder).IsPrototypeMember)
                            {
                                if ((changedObject as UFUAModel.UFUAFolder).UFUAFolderAss != null)
                                {
                                    var folder = (changedObject as UFUAModel.UFUAFolder).UFUAFolderAss;
                                    if (mapObjectToNode.ContainsKey(folder))
                                        parent = mapObjectToNode[folder];
                                }
                                else
                                    parent = itemRoot;
                            }
                            else if (changedObject is UFUAModel.UFUAAlarmThreshold)
                            {
                                if ((changedObject as UFUAModel.UFUAAlarmThreshold).UFUATagAss != null)
                                {
                                    var ufuatag = (changedObject as UFUAModel.UFUAAlarmThreshold).UFUATagAss;
                                    if (ufuatag.IsPrototypeMember && !ufuatag.IsSubPrototypeMember)
                                    {
                                        var tags = Document.FindSubPrototypeMembersByNodeId(ufuatag.NodeId);
                                        if (tags.Count > 0)
                                        {
                                            foreach (var tag in tags)
                                            {
                                                if (mapObjectToNode.ContainsKey(tag))
                                                    RefreshAsync(mapObjectToNode[tag].ParentNode);
                                            }

                                            FlatGridRefresh();
                                        }
                                    }
                                    else
                                    {
                                        if (mapObjectToNode.ContainsKey(ufuatag))
                                            parent = mapObjectToNode[ufuatag];
                                    }
                                }
                            }

                            if (parent != null)
                            {
                                if (parent.WasExpanded())
                                    AddTreeItem(changedObject, parent);

                                FlatGridRefresh();
                            }
                        }
                        else if (e.ChangedType == ChangedType.changed)
                        {
                            List<TreeListNode> parents = new List<TreeListNode>();
                            if (changedObject is UFUAModel.UFUAAlarmDefinition)
                            {
                                foreach (var thres in ((UFUAModel.UFUAAlarmDefinition)changedObject).UFUAAlarmThresholds)
                                    if (mapObjectToNode.ContainsKey(thres) && !parents.Contains(mapObjectToNode[thres].ParentNode))
                                        parents.Add(mapObjectToNode[thres].ParentNode);
                            }
                            else if (changedObject is UFUAModel.UFUAAlarmSource)
                            {
                                var nodes = (from o in mapObjectToNode.Keys
                                             where o is UFUAModel.UFUAAlarmThreshold && ((UFUAModel.UFUAAlarmThreshold)o).UFUAAlarmDefinitionRef?.UFUAAlarmDefinitions == changedObject
                                             && !parents.Contains(mapObjectToNode[o].ParentNode)
                                             select mapObjectToNode[o].ParentNode).ToList();
                                parents.AddRange(nodes);
                            }
                            else if (changedObject is UFUAModel.UFUAArea)
                            {
                                var nodes = (from o in mapObjectToNode.Keys
                                             where o is UFUAModel.UFUAAlarmThreshold && ((UFUAModel.UFUAAlarmThreshold)o).UFUAAlarmDefinitionRef?.UFUAAlarmDefinitions?.UFUAArea == changedObject
                                             && !parents.Contains(mapObjectToNode[o].ParentNode)
                                             select mapObjectToNode[o].ParentNode).ToList();
                                parents.AddRange(nodes);
                            }
                            if (parents.Count > 0)
                            {
                                foreach (var parent in parents)
                                    RefreshAsync(parent);
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

        internal void AddTagFolderList(List<IXPSimpleObject> list)
        {
            FlatGridRefresh();

            var items = new List<TreeListNode>();
            list.ForEach(baseobj =>
            {
                if (baseobj is UFUAModel.UFUAFolder)
                {
                    var folder = baseobj as UFUAModel.UFUAFolder;
                    var parent = itemRoot;
                    var listfolders = new List<UFUAModel.UFUAFolder>();
                    var folders = folder.UFUAFolderAss;
                    while (folders != null)
                    {
                        listfolders.Insert(0, folders);
                        folders = folders.UFUAFolderAss;
                    }
                    listfolders.Add(folder);

                    foreach (var f in listfolders)
                    {
                        parent.IsExpanded = true;
                        var itemfound = treeListControl.GetTreeItem(f, parent);
                        if (itemfound != null)
                            items.Add(itemfound);
                        else
                        {
                            var item = AddTreeItem(f, parent);
                            if (item != null)
                                items.Add(item);
                        }

                        parent = treeListControl.GetTreeItem(f, parent);
                    }
                }
                else if (baseobj is UFUAModel.UFUATag)
                {
                    var tag = baseobj as UFUAModel.UFUATag;
                    var parent = itemRoot;
                    var listfolders = new List<UFUAModel.UFUAFolder>();
                    var folders = tag.UFUAFolder;
                    while (folders != null)
                    {
                        listfolders.Insert(0, folders);
                        folders = folders.UFUAFolderAss;
                    }

                    foreach (var f in listfolders)
                    {
                        parent.IsExpanded = true;
                        var founditem = treeListControl.GetTreeItem(f, parent);
                        if (founditem != null)
                            items.Add(founditem);
                        else
                        {
                            var item = AddTreeItem(f, parent);
                            if (item != null)
                                items.Add(item);
                        }

                        parent = treeListControl.GetTreeItem(f, parent);
                    }

                    parent.IsExpanded = true;
                    var itemfound = treeListControl.GetTreeItem(baseobj, parent);
                    if (itemfound != null)
                        items.Add(itemfound);
                    else
                    {
                        var item = AddTreeItem(baseobj, parent);
                        if (item != null)
                            items.Add(item);
                    }
                }
            });

            if (items.Count > 0)
                treeListControl.SelectNodes(items, true, true);

            Document.AddUndoAction(this, list, UndoRedoAction.Added);
        }

        private void OnCanSelectRow(object sender, CanSelectRowEventArgs e)
        {
            e.CanSelectRow = true;
            if (TargetType == TargetType.None)
                return;

            UFUAModel.UFUATag tag = null;
            if (sender == treeListView)
                tag = treeListView.GetNodeByRowHandle(e.RowHandle).Tag as UFUAModel.UFUATag;
            else if (sender == tableView)
                tag = gridDataControl.GetRow(e.RowHandle) as UFUAModel.UFUATag;
            if (tag == null)
                return;

            if (TargetType == TargetType.Historian)
                e.CanSelectRow = CanAssignHistorian(tag);
            else if (TargetType == TargetType.AlarmThreshold)
                e.CanSelectRow = CanAssignAlarm(tag);
            else if (TargetType == TargetType.DataloggerColumn)
                e.CanSelectRow = CanAssignDataLoggerColumn(tag);
            else if (TargetType == TargetType.View)
                e.CanSelectRow = CanAssignView(tag);
            else if (TargetType == TargetType.EngineeringUnit)
                e.CanSelectRow = CanAssignEngineeringUnit(tag);
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
                        list.Add(parent as IXPSimpleObject);

                    if (parent is UFUAModel.UFUATag && obj is UFUAModel.UFUATag)
                    {
                        if ((parent as UFUAModel.UFUATag).DataType != (obj as UFUAModel.UFUATag).DataType)
                        {
                            List<DataLoggerSettings> dlrToRecalc = new List<DataLoggerSettings>();
                            dlrToRecalc = Document.GetDataLoggersWithTag(obj as UFUAModel.UFUATag);

                            UFUAEditorControl view = Document.ActiveView as UFUAEditorControl;
                            if (view != null)
                            {
                                foreach (DataLoggerSettings dlr in dlrToRecalc)
                                {
                                    if(dlr.UseAggregatedTables)
                                        view.AddPendingAggregateTables(dlr.Name, AggregationTypes.CommandType.Update);
                                }
                            }
                        }
                    }
                }

                if (list.Count > 0)
                    Document.AddUndoAction(this, list, UndoRedoAction.Changed);
            }
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
            itemRoot = treeListControl.AddNode(new TreeItemControl(Properties.Resources.AddressSpaceHeader) { ResourceIcon = closedFolderImg });
            treeListControl.AddNode(null, itemRoot, TreeListControlHelper.DummyNode);

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                if (bDisposed)
                    return;

                if (!isPopup)
                    Document.EditorManagerComponent.Workspace.IsBusy = true;

                try
                {
                    FillItems(itemRoot);
                    if (itemRoot.Nodes.Count == 0)
                        treeListControl.AddNode(null, itemRoot, TreeListControlHelper.DummyNode);
                    if (selectedInstanceOnEdit != null)
                        UpdateSelectedItem();
                    else
                        itemRoot.IsExpanded = true;
                }
                finally
                {
                    if (!isPopup)
                        Document.EditorManagerComponent.Workspace.IsBusy = false;
                }
            });
        }

        void OnGridSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            e.Handled = true;
            if (!isPopup && !UnSubscribeSelectionChangedEvent)
            {
                EndEdit();

                bDirtySelection = true;
                UpdateContextObjects(sender);
            }
        }

        void treeListControl_SelectionChanged(object sender, TreeListSelectionChangedEventArgs e)
        {
            if (!isPopup && !UnSubscribeSelectionChangedEvent)
            {
                EndEdit();

                bDirtySelection = true;
                UpdateContextObjects(sender);
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

        private void FillItems(TreeListNode iRoot, UFUAModel.UFUAFolder root = null)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

                using (new AsyncWaitCursor())
                {
                    ClearNodes(iRoot);
                    int i = 0;
                    var listTags = Document.GetTagCollection(root);
                    if (listTags != null)
                    {
                        foreach (var tag in listTags)
                        {
                            AddTreeItem(tag, iRoot);
                            if (!bShiftDown && ++i > maxItems)
                            {
                                //AddTreeItem(Properties.Resources.MaxItemCountVisibleReachedDoubleClick, iRoot);
                                break;
                            }
                        }
                    }

                    i = 0;
                    var listFolders = Document.GetFolderCollection(root);
                    if (listFolders != null)
                    {
                        foreach (var folder in listFolders)
                        {
                            AddTreeItem(folder, iRoot);
                            if (!bShiftDown && ++i > maxItems)
                            {
                                //AddTreeItem(Properties.Resources.MaxItemCountVisibleReachedDoubleClick, iRoot);
                                break;
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

        private void FillItems(TreeListNode iRoot, UFUAModel.UFUATag tag)
        {
            treeListControl.BeginDataUpdate();

            try
            {
                using (new AsyncWaitCursor())
                {
                    ClearNodes(iRoot);
                    if (!String.IsNullOrEmpty(tag.HistorianSettings))
                    {
                        var hs = Document.GetHistoricalSettings(tag.HistorianSettings);
                        if (hs != null)
                            AddTreeItem(hs, iRoot);
                    }

                    //foreach(var alarm in tag.UFUAAlarmDefinition)
                    //    AddTreeItem(alarm, iRoot);
                    var sortedAlarms = (from c in tag.UFUAAlarmThresholds orderby c.Oid ascending select c).ToList();
                    foreach (var alarm in sortedAlarms)
                        AddTreeItem(alarm, iRoot);
                    var sortedViews = (from c in tag.UFUAViews orderby c.Oid ascending select c).ToList();
                    foreach (var view in sortedViews)
                        AddTreeItem(view, iRoot);

                    if (/*isPopup &&*/
                        tag.ModelType == UFUAModel.ModelType.ObjectType &&
                        !String.IsNullOrEmpty(tag.PrototypeName))
                    {
                        var prototypeFound = Document.CreateSubPrototype(tag);
                        if (prototypeFound != null)
                        {
                            prototypeFound.EnsureUniqueMembersOrderId();

                            var sortedMembers = (from c in prototypeFound.Members orderby c.MemberOrderId ascending select c).ToList();
                            foreach (var member in sortedMembers)
                                AddTreeItem(member, iRoot);
                            var sortedFolders = (from c in prototypeFound.Folders orderby c.MemberOrderId ascending select c).ToList();
                            foreach (var folder in sortedFolders)
                                AddTreeItem(folder, iRoot);
                        }
                    }
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }
        }

        void OnTreeNodeExpanding(object sender, TreeListNodeAllowEventArgs e)
        {
            TreeListNode item = e.Node;
            if (item == null || TreeListControlHelper.WasExpanded(item)) //Node's subtree already populated
                return;

            try
            {
                (item.Content as TreeItemControl).IsNodeExpanding = true;
                treeListControl.BeginDataUpdate();

                ClearNodes(item);
                if (item == itemRoot || item.Tag is UFUAModel.UFUAFolder)
                {
                    UpdateFolderIcon(item, true);
                    FillItems(item, item.Tag as UFUAModel.UFUAFolder);
                }
                else if (item.Tag is UFUAModel.UFUATag)
                    FillItems(item, item.Tag as UFUAModel.UFUATag);
            }
            finally
            {
                treeListControl.EndDataUpdate();
                (item.Content as TreeItemControl).IsNodeExpanding = false;
            }
        }

        void OnTreeNodeExpanded(object sender, TreeListNodeEventArgs e)
        {
            TreeListNode item = e.Node;
            if (item != null)
                UpdateFolderIcon(item, item.IsExpanded);
        }

        void OnTreeNodeChanged(object sender, TreeListNodeChangedEventArgs e)
        {
            if (e.ChangeType == NodeChangeType.Add && e.Node.Tag != null && e.Node.Tag != TreeListControlHelper.DummyNode && !mapObjectToNode.ContainsKey(e.Node.Tag))
            {
                mapObjectToNode.Add(e.Node.Tag, e.Node);
                if (e.Node.Tag is UFUAModel.UFUATag)
                    (e.Node.Tag as INotifyPropertyChanged).PropertyChanged += UFUATag_PropertyChanged;
            }
            else if (e.ChangeType == NodeChangeType.Remove && e.Node.Tag != null && e.Node.Tag != TreeListControlHelper.DummyNode)
                ClearObjectMapNode(e.Node);
        }

        void ClearObjectMapNode(TreeListNode node)
        {
            if (node.Tag != null && node.Tag != TreeListControlHelper.DummyNode && mapObjectToNode.ContainsKey(node.Tag))
            {
                mapObjectToNode.Remove(node.Tag);
                if (node.Tag is UFUAModel.UFUATag)
                    (node.Tag as INotifyPropertyChanged).PropertyChanged -= UFUATag_PropertyChanged;
                foreach (var child in node.Nodes)
                    ClearObjectMapNode(child);
            }
        }

        void UFUATag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (UnSubscribeSelectionChangedEvent)
                return;

            if (e.PropertyName == "HistorianSettings" || e.PropertyName == "PrototypeModel" || e.PropertyName == "UFUAViews")
            {
                if (mapObjectToNode.ContainsKey(sender))
                {
                    var item = mapObjectToNode[sender];
                    if (e.PropertyName != "PrototypeModel" || !(item.Content as TreeItemControl).IsNodeExpanding)
                    {
                        RefreshAsync(item);
                        FlatGridRefresh();
                    }
                }
            }
            else if (e.PropertyName == "UseShared")
            {
                if (mapObjectToNode.ContainsKey(sender))
                {
                    RefreshAsync(mapObjectToNode[sender].ParentNode);
                    FlatGridRefresh();
                }
            }
            else if (e.PropertyName == "DataType")
            {
                if (mapObjectToNode.ContainsKey(sender))
                    (mapObjectToNode[sender].Content as TreeItemControl).ResourceIcon = UFUAEditorManagerComponent.GetTagBitmapImage(sender as UFUAModel.UFUATag);
            }
        }

        void ClearNodes(TreeListNode node)
        {
            node.Nodes.Clear();
        }

        public class NameModel
        {
            public String Name { get; set; }
        }

        internal TreeListNode AddTreeItem(Object tag, TreeListNode parent)
        {
            if (bDisposed)
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

            if (NeedToBeExpanded(newitem))
                treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);

            bool bShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            bool bKnownType = false;

            if (tag is UFUAModel.UFUAFolder)
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
            }
            else if (tag is String)
            {
                bKnownType = true;
                (newitem.Content as TreeItemControl).Header = tag as String;
                (newitem.Content as TreeItemControl).ResourceIcon = closedFolderImg;
            }
            else if (tag is UFUAModel.UFUATag)
            {
                bKnownType = true;
                var ufuatag = tag as UFUAModel.UFUATag;
                //if (!addressSpaceTree.MultiColumnEnable)
                //{
                SetBindingOnProp(newitem, tag, "Name");
                //}
                //else
                //    newitem.Header = ufuatag;
                //newitem.CollapsedImageSource =
                //newitem.ExpandedImageSource = bmpImage;

                (newitem.Content as TreeItemControl).ResourceIcon = UFUAEditorManagerComponent.GetTagBitmapImage(ufuatag);

                //newitem.IsEditable = false;
                //parent.Items.Add(newitem);
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
            }
            else if (tag is UFUAModel.UFUAHistorianSettings)
            {
                bKnownType = true;
                //var hs = tag as UFUAModel.UFUAHistorianSettings;
                //if (!addressSpaceTree.MultiColumnEnable)
                //{
                SetBindingOnProp(newitem, tag, "Name");
                //}
                //else
                //    newitem.Header = hs;
                //newitem.CollapsedImageSource =
                //newitem.ExpandedImageSource = UFUAEditorManagerComponent.GetBitmapImage("UFUASHistoricalPrototypes");

                (newitem.Content as TreeItemControl).ResourceIcon = UFUAEditorManagerComponent.GetBitmapImage("UFUASHistoricalPrototypesSmall");

                //newitem.IsEditable = false;
                //parent.Items.Add(newitem);
            }
            else if (tag is UFUAModel.UFUAAlarmThreshold)
            {
                var alarm = tag as UFUAModel.UFUAAlarmThreshold;
                if (alarm.UFUAAlarmDefinitionRef != null && alarm.UFUAAlarmDefinitionRef.UFUAAlarmDefinitions != null)
                {
                    bKnownType = true;
                    //if (!addressSpaceTree.MultiColumnEnable)
                    //{
                    SetBindingOnProp(newitem, tag, "CompleteName");
                    //}
                    //else
                    //    newitem.Header = alarm;
                    //newitem.CollapsedImageSource =
                    //    newitem.ExpandedImageSource = alarm.UFUAAlarmDefinitionRef == null || alarm.UFUAAlarmDefinitionRef.Severity > 0 ?
                    //    UFUAEditorManagerComponent.GetBitmapImage("UFUASAlarmPrototype") :
                    //    UFUAEditorManagerComponent.GetBitmapImage("UFUASMessagePrototype");

                    (newitem.Content as TreeItemControl).ResourceIcon = alarm.UFUAAlarmDefinitionRef == null || alarm.UFUAAlarmDefinitionRef.Severity > 0 ?
                        UFUAEditorManagerComponent.GetBitmapImage("UFUASAlarmPrototypeSmall") :
                        UFUAEditorManagerComponent.GetBitmapImage("UFUASMessagePrototypeSmall");

                    //newitem.IsEditable = false;
                    //parent.Items.Add(newitem);
                }
                else
                {
                    parent.Nodes.Remove(newitem);
                    return null;
                }
            }
            else if (tag is UFUAModel.UFUAView)
            {
                bKnownType = true;
                //var view = tag as UFUAModel.UFUAView;
                //if (!addressSpaceTree.MultiColumnEnable)
                //{
                SetBindingOnProp(newitem, tag, "Name");
                //}
                //else
                //    newitem.Header = view;
                //newitem.CollapsedImageSource =
                //newitem.ExpandedImageSource = UFUAEditorManagerComponent.GetBitmapImage("UFUASViews");

                (newitem.Content as TreeItemControl).ResourceIcon = UFUAEditorManagerComponent.GetBitmapImage("UFUASViewsSmall");

                //newitem.IsEditable = false;
                //parent.Items.Add(newitem);
            }

            treeListControl.RefreshRow(newitem.RowHandle); //Otherwise node's header (ItemHeader) can disappear in certain conditions
            return bKnownType ? newitem : null;
        }

        internal bool CanEdit()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null)
            {
                if (selected.Tag is UFUAModel.UFUAFolder)
                {
                    var folder = selected.Tag as UFUAModel.UFUAFolder;
                    return !folder.IsSubPrototypeMember;
                }
                else if (selected.Tag is UFUAModel.UFUAAlarmThreshold)
                {
                    var alr = selected.Tag as UFUAModel.UFUAAlarmThreshold;
                    return alr.UFUATagAss == null || (!alr.UFUATagAss.IsSubPrototypeMember || !alr.UFUATagAss.UseShared.Value);
                }
                else
                    return true;
            }

            return false;
        }

        internal void ForceSelectionChangedEvent()
        {
            treeListControl_SelectionChanged(this, null);
        }

        internal void EditSelectedItem()
        {
            if (!CanEdit())
                return;

            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null)
                Edit(selected);
        }

        internal void DeleteSelectedItems()
        {
            var items = new List<TreeListNode>();
            var gitems = new List<UFUAModel.UFUATag>();
            var listundo = new List<IXPSimpleObject>();

            if (gridControlTab.IsSelected)
            {
                foreach (var item in gridDataControl.SelectedItems)
                {
                    if (item is UFUAModel.UFUATag)
                        gitems.Add((item as UFUAModel.UFUATag));
                    if (IsUndoRedoSupported(item))
                        listundo.Add(item as IXPSimpleObject);
                    if (mapObjectToNode.ContainsKey(item))
                        items.Add(mapObjectToNode[item]);
                }
            }
            else
            {
                items.AddRange((from item in treeListControl.GetSelectedNodes()
                                where item.Tag != null
                                select item));

                listundo.AddRange((from item in items
                                   where IsUndoRedoSupported(item.Tag, UndoRedoAction.Removed) &&
                                   !items.Contains(item.ParentNode)
                                   select item.Tag as IXPSimpleObject));

                var uiMsgBox = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                {
                    var bShowMessage = (from item in items/*.AsParallel()*/
                                        where item.Tag is UFUAModel.UFUAFolder &&
                                        listundo.Contains(item.Tag as UFUAModel.UFUAFolder) &&
                                        ((item.Tag as UFUAModel.UFUAFolder).UFUATags.Count > 0 ||
                                        (item.Tag as UFUAModel.UFUAFolder).UFUAFolders.Count > 0)
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
            }

            if (listundo.Count > 0)
                Document.AddUndoAction(this, listundo, UndoRedoAction.Removed);

            foreach (var item in gitems)
                item.Delete();

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

            if (gridControlTab.IsSelected)
                gridDataControl.SelectedItems.Clear();

            FlatGridRefresh();
        }

        internal void DeleteTreeItem(TreeListNode item)
        {
            //var item = addressSpaceTree.SelectedItem as TreeViewItemAdv;
            var parent = item.ParentNode ?? itemRoot;
            if (item.Tag is UFUAModel.UFUATag)
            {
                var tag = item.Tag as UFUAModel.UFUATag;
                if (!tag.IsPrototypeMember)
                {
                    //GetSelectedParentFolder(true).Items.Remove(item);
                    parent.Nodes.Remove(item);
                    if (tagToReference.ContainsKey(tag))
                    {
                        if (referenceToTag.ContainsKey(tagToReference[tag]))
                            referenceToTag.Remove(tagToReference[tag]);
                        tagToReference.Remove(tag);
                    }
                    tag.Delete();

                    var maxitem = treeListControl.GetTreeItem(Properties.Resources.MaxItemCountVisibleReachedDoubleClick, parent);
                    if (maxitem != null)
                    {
                        parent.Nodes.Remove(maxitem);
                        Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            FillItems(parent, parent.Tag as UFUAModel.UFUAFolder);
                        });
                    }
                }
            }
            else if (item.Tag is UFUAModel.UFUAFolder)
            {
                var folder = item.Tag as UFUAModel.UFUAFolder;
                if (!folder.IsPrototypeMember)
                {
                    //GetSelectedParentFolder(true).Items.Remove(item);
                    parent.Nodes.Remove(item);
                    folder.Delete();

                    var maxitem = treeListControl.GetTreeItem(Properties.Resources.MaxItemCountVisibleReachedDoubleClick, parent);
                    if (maxitem != null)
                    {
                        parent.Nodes.Remove(maxitem);
                        Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            FillItems(parent, parent.Tag as UFUAModel.UFUAFolder);
                        });
                    }
                }
            }
            else if (item.Tag is UFUAModel.UFUAView)
            {
                var view = item.Tag as UFUAModel.UFUAView;
                //var parent = GetSelectedParent(true);
                var tag = parent.Tag as UFUAModel.UFUATag;
                if (tag != null && (!tag.IsPrototypeMember || !tag.UseShared.Value))
                {
                    parent.Nodes.Remove(item);
                    if (tag != null && tag.UFUAViews.Contains(view))
                    {
                        Document.AddUndoAction(this, tag, UndoRedoAction.Changed);
                        tag.UFUAViews.Remove(view);
                        tag.NotifyPropertyChanged("UFUAViews");
                        tag.NotifyPropertyChanged("Views");
                        view.UFUATags.Remove(tag);
                        view.NotifyPropertyChanged("UFUATags");
                    }
                }
            }
            else if (item.Tag is UFUAModel.UFUAAlarmThreshold)
            {
                var alarm = item.Tag as UFUAModel.UFUAAlarmThreshold;
                RemoveAlarmFromTag(alarm);
            }
            else if (item.Tag is UFUAModel.UFUAHistorianSettings)
            {
                using (var uow = Document.BeginNestedUnitOfWork())
                {
                    var tag = parent.Tag as UFUAModel.UFUATag;
                    var original = uow.GetNestedObject(tag);
                    if (tag != null && (!tag.IsPrototypeMember || !tag.UseShared.Value))
                    {
                        parent.Nodes.Remove(item);
                        if (tag != null)
                        {
                            Document.AddUndoAction(this, tag, UndoRedoAction.Changed);
                            if (tag.HistorianSettings != null)
                            {
                                tag.HistorianSettings = null;
                                Document.OnChangedDocument(this, ChangedType.changed, new List<object>() { tag }, new List<object>() { original });
                            }
                        }
                    }
                }
            }
        }

        internal void CopySelectedToClipboard()
        {
            var listfolders = new List<UFUAModel.UFUAFolder>();
            var listtags = new List<UFUAModel.UFUATag>();
            var listalarms = new List<UFUAModel.UFUAAlarmThreshold>();
            var listnodes = treeListControl.GetSelectedNodes();
            if (listnodes != null)
                Document.EditorManagerComponent.Workspace.UpdateProgressState(0,
                                                    listnodes.Count(),
                                                   $"{Properties.Resources.WorkInProgress}",
                                                   TaskbarItemProgressState.Normal);
            foreach (var item in treeListControl.GetSelectedNodes())
            {
                if (item.Tag is UFUAModel.UFUAFolder)
                    listfolders.Add(item.Tag as UFUAModel.UFUAFolder);
                else if (item.Tag is UFUAModel.UFUATag)
                    listtags.Add(item.Tag as UFUAModel.UFUATag);
                else if (item.Tag is UFUAModel.UFUAAlarmThreshold)
                    listalarms.Add(item.Tag as UFUAModel.UFUAAlarmThreshold);
                Document.EditorManagerComponent.Workspace.IncrementProgressState();
            }

            Document.CleanClipbaord();
            Document.CopyListFoldersToClipbaord(listfolders);
            Document.CopyListTagsToClipbaord(listtags);
            Document.CopyListAlarmThresholdsToClipbaord(listalarms);
            Document.CheckClipbaord();
            if (Document.EditorManagerComponent.Workspace != null)
                Document.EditorManagerComponent.Workspace.ResetProgressState();
        }

        internal void PasteFromClipboard()
        {
            var items = new List<TreeListNode>();
            try
            {
                var parent = GetSelectedParentFolder();
                var expanded = parent.IsExpanded || parent.Nodes.Count > 0 && parent.Nodes[0].Tag != TreeListControlHelper.DummyNode;

                var parenttag = GetSelectedParentTag();
                var expandedtag = parenttag != null && (parenttag.IsExpanded || parenttag.Nodes.Count > 0 && parenttag.Nodes[0].Tag != TreeListControlHelper.DummyNode);

                treeListControl.ClearSelection();
                UnSubscribeSelectionChangedEvent = true;
                treeListControl.BeginDataUpdate();

                UFUAModel.UFUAFolder folder = null;
                if (parent.Tag is UFUAModel.UFUAFolder)
                    folder = parent.Tag as UFUAModel.UFUAFolder;

                List<XPObject> listTempfoldersandtags = null;
                if (UFUAEditorManagerComponent.ufuaEditorManagerComponent.TempVarLoaded)
                {
                    listTempfoldersandtags = Document.PasteClipboardTempFoldersAndTags(folder);
                    if (listTempfoldersandtags != null && Document.EditorManagerComponent.Workspace != null)
                    {
                        Document.EditorManagerComponent.Workspace.UpdateProgressState(0,
                                                listTempfoldersandtags.Count,
                                                $"{Properties.Resources.WorkInProgress}",
                                                TaskbarItemProgressState.Normal);
                    }
                    listTempfoldersandtags.ForEach(dir =>
                    {
                        TreeListNode item = null;
                        if (expanded)
                            item = AddTreeItem(dir, parent);
                        else
                            item = treeListControl.GetTreeItem(dir, parent);
                        if (item != null)
                            items.Add(item);
                        if (Document.EditorManagerComponent.Workspace != null)
                        {
                            Document.EditorManagerComponent.Workspace.IncrementProgressState();
                        }
                    });
                }

                var listfolders = Document.PasteClipboardFolders(folder);
                if (listfolders != null && Document.EditorManagerComponent.Workspace != null)
                {
                    Document.EditorManagerComponent.Workspace.UpdateProgressState(0,
                                            listfolders.Count,
                                            $"{Properties.Resources.WorkInProgress}",
                                            TaskbarItemProgressState.Normal);
                }
                treeListControl.AddDummyNodeIfNeeded(parent, NeedToBeExpanded);
                listfolders.ForEach(dir =>
                {
                    TreeListNode item = null;
                    if (expanded)
                        item = AddTreeItem(dir, parent);
                    else
                        item = treeListControl.GetTreeItem(dir, parent);
                    if (item != null)
                        items.Add(item);
                    if (Document.EditorManagerComponent.Workspace != null)
                    {
                        Document.EditorManagerComponent.Workspace.IncrementProgressState();
                    }
                });

                var listtags = Document.PasteClipboardTags(folder);
                if (listtags != null && Document.EditorManagerComponent.Workspace != null)
                {
                    Document.EditorManagerComponent.Workspace.UpdateProgressState(0,
                                            listtags.Count,
                                            $"{Properties.Resources.WorkInProgress}",
                                            TaskbarItemProgressState.Normal);
                }
                treeListControl.AddDummyNodeIfNeeded(parent, NeedToBeExpanded);
                listtags.ForEach(tag =>
                {
                    TreeListNode item = null;
                    if (expanded)
                        item = AddTreeItem(tag, parent);
                    else
                        item = treeListControl.GetTreeItem(tag, parent);
                    if (item != null)
                        items.Add(item);
                    if (Document.EditorManagerComponent.Workspace != null)
                    {
                        Document.EditorManagerComponent.Workspace.IncrementProgressState();
                    }
                });

                var listalarms = new List<UFUAModel.UFUAAlarmThreshold>();
                if (parenttag != null)
                {
                    //if (!expandedtag && !TreeListControlHelper.CanBeExpanded(parenttag))
                    //    treeListControl.AddNode(null, parenttag, TreeListControlHelper.DummyNode);

                    listalarms.AddRange(Document.PasteClipboardAlarmThresholds(parenttag.Tag as UFUAModel.UFUATag));
                    if (listalarms != null && Document.EditorManagerComponent.Workspace != null)
                    {
                        Document.EditorManagerComponent.Workspace.UpdateProgressState(0,
                                                listalarms.Count,
                                                $"{Properties.Resources.WorkInProgress}",
                                                TaskbarItemProgressState.Normal);
                    }
                    treeListControl.AddDummyNodeIfNeeded(parent, NeedToBeExpanded);
                    listalarms.ForEach(alr =>
                    {
                        TreeListNode item = null;
                        if (expandedtag)
                            item = AddTreeItem(alr, parenttag);
                        else
                            item = treeListControl.GetTreeItem(alr, parenttag);
                        if (item != null)
                            items.Add(item);
                        if (Document.EditorManagerComponent.Workspace != null)
                        {
                            Document.EditorManagerComponent.Workspace.IncrementProgressState();
                        }
                    });
                }

                if (listfolders.Count > 0 || listtags.Count > 0 || listalarms.Count > 0 || (listTempfoldersandtags != null && listTempfoldersandtags.Count > 0))
                {
                    FlatGridRefresh();

                    // add list to undo manager
                    var listundo = new List<IXPSimpleObject>();
                    listfolders.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
                    listtags.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
                    listalarms.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
                    listTempfoldersandtags?.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
                    Document.AddUndoAction(this, listundo, UndoRedoAction.Added);
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
                if (items.Count > 0)
                    treeListControl.SelectNodes(items, true, true);
                UnSubscribeSelectionChangedEvent = false;
                FlatGridRefresh();
                //if (selectionchanged)
                ForceSelectionChangedEvent();
                if (Document.EditorManagerComponent.Workspace != null)
                {
                    Document.EditorManagerComponent.Workspace.ResetProgressState();
                }
            }
        }

        internal void AddFolder(UFUAModel.UFUAFolder folder)
        {
            folder.Name = UFUAModel.Helpers.NameValidator.EnsureValidName(folder.Name);
            Document.AddUndoAction(this, folder, UndoRedoAction.Added);
            var item = AddTreeItem(folder, GetSelectedParentFolder());
            if (item != null)
            {
                treeListControl.ClearSelection();
                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    treeListControl.SelectNode(item);
                });
            }
        }

        internal void AddTag(UFUAModel.UFUATag tag)
        {
            tag.Name = UFUAModel.Helpers.NameValidator.EnsureValidName(tag.Name);
            Document.EnsureValidNodeId(tag);
            Document.AddUndoAction(this, tag, UndoRedoAction.Added);
            var item = AddTreeItem(tag, GetSelectedParentFolder());
            if (item != null)
            {
                treeListControl.ClearSelection();
                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    treeListControl.SelectNode(item);
                });
            }
            FlatGridRefresh();
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

                            //var list = new List<GridDataGroupColumn>();
                            //foreach (var group in gridDataControl.GroupedColumns)
                            //    list.Add(new GridDataGroupColumn() { ColumnName = group.ColumnName });

                            //while (gridDataControl.GroupedColumns.Count > 0)
                            //    gridDataControl.GroupedColumns.Remove(gridDataControl.GroupedColumns[0]);

                            gridDataControl.ItemsSource = null;
                            gridDataControl.ItemsSource = Document.GetFlatTagCollection();
                            gridDataControl.SelectedItems.Clear();

                            var selectedItems = (from c in treeListControl.GetSelectedNodes()
                                                 where c.Tag != null
                                                 select c.Tag).ToList();
                            if (selectedItems.Count > 0)
                            {
                                selectedItems.ForEach((o) => gridDataControl.SelectedItems.Add(o));
                                gridDataControl.CurrentItem = selectedItems.First();
                            }

                            //foreach (var group in list)
                            //{
                            //    gridDataControl.GroupedColumns.Add(new GridDataGroupColumn() { ColumnName = group.ColumnName });
                            //}
                        }
                        catch
                        {
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
        internal bool IsRootSelected()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            return selected != null && selected == itemRoot;
        }

        internal UFUAModel.UFUAFolder GetSelectedFolder()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUAModel.UFUAFolder && !(selected.Tag as UFUAModel.UFUAFolder).IsPrototypeMember)
                return selected.Tag as UFUAModel.UFUAFolder;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFUAModel.UFUAFolder && !(selected.Tag as UFUAModel.UFUAFolder).IsPrototypeMember)
                    return selected.Tag as UFUAModel.UFUAFolder;
            }

            return null;
        }

        bool CanDeleteAlarms()
        {
            if (bDirtySelection)
            {
                bDirtySelection = bCanDeleteAlarms = false;
                var selected = treeListControl.GetTreeSelectedItems();
                if (selected == null || selected.Count == 0)
                    return false;

                foreach (var tag in selected)
                {
                    if (tag is UFUAModel.UFUATag && CanDeleteAlarms(tag as UFUAModel.UFUATag))
                    {
                        bCanDeleteAlarms = true;
                        break;
                    }
                }
            }

            return bCanDeleteAlarms;
        }

        bool CanDeleteAlarms(UFUAModel.UFUATag tag)
        {
            return (!tag.IsPrototypeMember || !tag.UseShared.Value) && tag.UFUAAlarmThresholds.Count > 0;
        }

        internal bool CanAssignItem()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUAModel.UFUATag)
            {
                var tag = selected.Tag as UFUAModel.UFUATag;
                return CanAssignItem(tag);
            }

            return false;
        }

        internal bool CanAssignView()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUAModel.UFUATag)
            {
                var tag = selected.Tag as UFUAModel.UFUATag;
                return CanAssignView(tag);
            }

            return false;
        }

        void AssignHistorianToNodes(UFUAModel.UFUAHistorianSettings hs, List<TreeListNode> nodes, bool bAllowSelectionChange = true)
        {
            if (nodes.Count == 0)
                return;

            bool selectionchanged = false;
            List<TreeListNode> addedNodes = new List<TreeListNode>();
            try
            {
                UnSubscribeSelectionChangedEvent = true;
                treeListControl.BeginDataUpdate();
                using (new AsyncWaitCursor())
                {
                    var listundo = new List<IXPSimpleObject>();
                    nodes.ForEach(item => { listundo.Add(item.Tag as IXPSimpleObject); });
                    if (listundo.Count > 0)
                        Document.AddUndoAction(this, listundo, UndoRedoAction.Changed);

                    var changedObjects = new List<object>();
                    var originalObjects = new List<object>();
                    using (var uow = Document.BeginNestedUnitOfWork())
                    {
                        foreach (var node in nodes)
                        {
                            var tag = node.Tag as UFUAModel.UFUATag;
                            var original = uow.GetNestedObject(tag);
                            RemoveTagHistorian(node);

                            if (tag.HistorianSettings != hs.Name)
                            {
                                originalObjects.Add(original);
                                tag.HistorianSettings = hs.Name;
                                changedObjects.Add(tag);
                            }

                            var found = (from TreeListNode n in node.Nodes where n.Tag == hs select n).FirstOrDefault();
                            if (found == null)
                            {
                                var item = AddTreeItem(hs, node);
                                if (item != null)
                                {
                                    selectionchanged = true;
                                    addedNodes.Add(item);
                                }
                            }
                        }
                        if (changedObjects.Count > 0)
                            Document.OnChangedDocument(this, ChangedType.changed, changedObjects, originalObjects);
                    }
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
                if (bAllowSelectionChange && addedNodes.Count > 0)
                    treeListControl.SelectNodes(addedNodes, true, true);
                UnSubscribeSelectionChangedEvent = false;
                if (selectionchanged && bAllowSelectionChange)
                    ForceSelectionChangedEvent();
            }
        }

        internal void AssignItemToSelect(UFUAModel.UFUAHistorianSettings hs)
        {
            var nodes = (from p in treeListControl.GetSelectedNodes() where p.Tag is UFUAModel.UFUATag select p).ToList();
            AssignHistorianToNodes(hs, nodes);
        }

        void RemoveTagHistorian(TreeListNode tagNode)
        {
            var list = (from p in tagNode.Nodes where p.Tag is UFUAModel.UFUAHistorianSettings select p).ToList();
            foreach (var node in list)
                tagNode.Nodes.Remove(node);
        }

        internal void AssignItemToSelect(UFUAModel.UFUAView hs)
        {
            bool selectionchanged = false;
            List<TreeListNode> addedNodes = new List<TreeListNode>();
            try
            {
                UnSubscribeSelectionChangedEvent = true;
                treeListControl.BeginDataUpdate();
                using (new AsyncWaitCursor())
                {
                    var listItems = (from p in treeListControl.GetSelectedNodes()
                                     where p.Tag is UFUAModel.UFUATag
                                     select p).ToList();

                    var listundo = new List<IXPSimpleObject>();
                    listItems.ForEach(item => { listundo.Add((item as TreeListNode).Tag as IXPSimpleObject); });
                    if (listundo.Count > 0)
                        Document.AddUndoAction(this, listundo, UndoRedoAction.Changed);

                    bool bChanged = false;
                    foreach (TreeListNode selected in listItems)
                    {
                        var tag = selected.Tag as UFUAModel.UFUATag;
                        if (!tag.UFUAViews.Contains(hs))
                        {
                            bChanged = true;
                            tag.UFUAViews.Add(hs);
                            tag.NotifyPropertyChanged("UFUAViews");
                            tag.NotifyPropertyChanged("Views");
                            hs.UFUATags.Add(tag);
                            hs.NotifyPropertyChanged("UFUATags");
                            var item = AddTreeItem(hs, selected);
                            if (item != null)
                            {
                                selectionchanged = true;
                                addedNodes.Add(item);
                            }
                        }
                    }

                    if (bChanged)
                        Document.NeedsSave = true;
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
                if (addedNodes.Count > 0)
                    treeListControl.SelectNodes(addedNodes, true, true);
                UnSubscribeSelectionChangedEvent = false;
                if (selectionchanged)
                    ForceSelectionChangedEvent();
            }
        }

        internal void AssignItemsToSelect(IEnumerable<UFUAModel.UFUAAlarmDefinition> hs, AssignAlarmViewModel model)
        {
            bool selectionchanged = false;
            List<TreeListNode> addedNodes = new List<TreeListNode>();
            try
            {
                UnSubscribeSelectionChangedEvent = true;
                treeListControl.BeginDataUpdate();
                using (new AsyncWaitCursor())
                {
                    var listItems = (from p in treeListControl.GetSelectedNodes()
                                     where p.Tag is UFUAModel.UFUATag
                                     select p).ToList();

                    var listundo = new List<IXPSimpleObject>();
                    foreach (TreeListNode selected in listItems)
                    {
                        var tag = selected.Tag as UFUAModel.UFUATag;
                        foreach (var alrDef in hs)
                        {
                            var list = Document.GetNewThresholdList(alrDef, model, tag);
                            list.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
                            tag.UFUAAlarmThresholds.AddRange(list);
                            tag.NotifyPropertyChanged("UFUAAlarmThresholds");
                            list.ForEach((alr) =>
                            {
                                alr.UFUAAlarmDefinitionRef = alrDef;
                                alr.UFUAAlarmDefinitionNodeIdRef = alrDef.NodeId;
                                if (!alrDef.UFUAAlarmThresholds.Contains(alr))
                                    alrDef.UFUAAlarmThresholds.Add(alr);

                                var item = AddTreeItem(alr, selected);
                                if (item != null)
                                {
                                    selectionchanged = true;
                                    addedNodes.Add(item);
                                }
                            });
                        }
                    }

                    if (listItems.Count > 0)
                    {
                        foreach (var handle in gridDataControl.GetSelectedRowHandles())
                            gridDataControl.RefreshRow(handle);
                        Document.NeedsSave = true;
                    }

                    if (listundo.Count > 0)
                        Document.AddUndoAction(this, listundo, UndoRedoAction.Added);
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
                if (addedNodes.Count > 0)
                    treeListControl.SelectNodes(addedNodes, true, true);
                UnSubscribeSelectionChangedEvent = false;
                if (selectionchanged)
                    ForceSelectionChangedEvent();
            }
        }

        public TreeListNode GetSelectedParentFolder(bool bSkipSelected = false)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (!bSkipSelected && selected != null && selected.Tag is UFUAModel.UFUAFolder && !(selected.Tag as UFUAModel.UFUAFolder).IsPrototypeMember)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFUAModel.UFUAFolder && !(selected.Tag as UFUAModel.UFUAFolder).IsPrototypeMember)
                    return selected;
            }

            return itemRoot;
        }

        public TreeListNode GetSelectedParentTag(bool bSkipSelected = false)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (!bSkipSelected && selected != null && selected.Tag is UFUAModel.UFUATag &&
                (!(selected.Tag as UFUAModel.UFUATag).IsPrototypeMember || !(selected.Tag as UFUAModel.UFUATag).UseShared.Value))
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is UFUAModel.UFUATag &&
                    (!(selected.Tag as UFUAModel.UFUATag).IsPrototypeMember || !(selected.Tag as UFUAModel.UFUATag).UseShared.Value))
                    return selected;
            }

            return null;
        }

        public TreeListNode GetParentTag(TreeListNode parent)
        {
            while (parent != null)
            {
                parent = parent.ParentNode;
                if (parent != null && parent.Tag is UFUAModel.UFUATag)
                    return parent;
            }

            return null;
        }

        private void EditTag(UFUAModel.UFUATag ufuatag)
        {
            if (ufuatag == null)
                return;

            using (var uow = Document.BeginNestedUnitOfWork())
            {
                var newTagControl = new NewTag(Document)
                {
                    DataContext = uow.GetNestedObject(ufuatag)
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newTagControl)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "TagEditor"
                };
                if (Dialog.ShowDialog() == true)
                {
                    Document.AddUndoAction(this, ufuatag, UndoRedoAction.Changed);
                    uow.CommitChanges();
                    UpdateContextObjects();
                }
            }
        }

        private void Edit(TreeListNode selected)
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
                        FlatGridRefresh();
                    }
                }
            }
            else if (selected.Tag is UFUAModel.UFUAHistorianSettings)
            {
                var hs = selected.Tag as UFUAModel.UFUAHistorianSettings;
                using (var uow = Document.BeginNestedUnitOfWork())
                {
                    var newhsControl = new NewHistoricalSettings(Document)
                    {
                        DataContext = uow.GetNestedObject(hs)
                    };
                    GeneralDialogContent Dialog = new GeneralDialogContent(newhsControl)
                    {
                        Owner = this.FindParent<Window>(),
                        HelpLink = "HistorianSettingEditor"
                    };

                    if (Dialog.ShowDialog() == true)
                    {
                        Document.AddUndoAction(this, hs, UndoRedoAction.Changed);
                        uow.CommitChanges();
                        UpdateContextObjects();
                    }
                }
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
            else if (selected.Tag is UFUAModel.UFUAView)
            {
                var view = selected.Tag as UFUAModel.UFUAView;
                using (var uow = Document.BeginNestedUnitOfWork())
                {
                    var newhsControl = new NewView()
                    {
                        DataContext = uow.GetNestedObject(view)
                    };
                    GeneralDialogContent Dialog = new GeneralDialogContent(newhsControl)
                    {
                        Owner = this.FindParent<Window>(),
                        HelpLink = "ViewEditor"
                    };
                    if (Dialog.ShowDialog() == true)
                    {
                        Document.AddUndoAction(this, view, UndoRedoAction.Changed);
                        uow.CommitChanges();
                        UpdateContextObjects();
                    }
                }
            }
            else if (selected.Tag is UFUAModel.UFUATag)
            {
                var ufuatag = selected.Tag as UFUAModel.UFUATag;
                EditTag(ufuatag);
                FlatGridRefresh();
            }
        }

        void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            if (e.Source.DataControl == treeListControl)
            {
                e.Handled = true;
                var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
                if (selected != null)
                {
                    if (selected.Tag as String == Properties.Resources.MaxItemCountVisibleReachedDoubleClick)
                    {
                        using (var cursor = new WaitCursor())
                        {
                            var parent = selected.ParentNode;
                            if (parent == itemRoot)
                            {
                                ClearNodes(parent);
                                var oldMaxItem = maxItems;
                                maxItems = Int32.MaxValue;
                                FillItems(itemRoot);
                                maxItems = oldMaxItem;
                            }
                            else if (parent != null)
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
                    else if (isPopup && selected.Tag is UFUAModel.UFUATag)
                    {
                        var wnd = this.FindParent<Window>();
                        if (wnd != null)
                        {
                            e.Handled = true;
                            selectedItems = new List<TreeListNode> { selected };
                            wnd.DialogResult = true;
                            wnd.Close();
                        }
                    }
                    else if (!isPopup && Document != null)
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
                var selected = gridDataControl.SelectedItem as UFUAModel.UFUATag;
                if (selected != null)
                {
                    if (isPopup)
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
                        {
                            var ufuatag = gridDataControl.SelectedItem as UFUAModel.UFUATag;
                            EditTag(ufuatag);
                            FlatGridRefresh();
                        }
                    }
                }
            }
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (treeListControl.View.AllowEditing)
                return;

            if (e.Key == Key.F5)
            {
                e.Handled = true;
                OnRefresh(null, null);
            }
            else if (e.Key == Key.F2 && !isPopup)
            {
                var selectedItem = treeListControl.GetSelectedNodes().FirstOrDefault();

                if (selectedItem != null && selectedItem != itemRoot && (selectedItem.Tag is UFUAModel.UFUAFolder && !((UFUAModel.UFUAFolder)selectedItem.Tag).IsPrototypeMember || selectedItem.Tag is UFUAModel.UFUATag && !((UFUAModel.UFUATag)selectedItem.Tag).IsPrototypeMember))
                {
                    e.Handled = true;
                    treeListControl.EditNode(selectedItem);
                }
            }
        }

        //https://www.devexpress.com/Support/Center/Question/Details/T444753/treelist-arrow-keys-not-working-when-in-the-editor-the-text-is-selected-all
        private void View_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right || e.Key == Key.Left)
            {
                var activeTextEditor = (sender as DataViewBase).ActiveEditor as DevExpress.Xpf.Editors.TextEdit;
                if (activeTextEditor != null && activeTextEditor.SelectionLength == activeTextEditor.Text.Length)
                {
                    activeTextEditor.CaretIndex = e.Key == Key.Left ? 0 : activeTextEditor.SelectionLength;
                    e.Handled = true;
                }
            }
        }

        void OnTreeHeaderValidate(object sender, TreeListCellValidationEventArgs e)
        {
            var node = e.Node;
            var oldStr = e.CellValue as String;
            var newStr = e.Value as String;
            var propertyName = "Name";
            XPObject item = null;

            try
            {
                if (node.Tag is UFUAModel.UFUAFolder)
                {
                    var folder = (UFUAModel.UFUAFolder)node.Tag;
                    item = folder;
                }
                else if (node.Tag is UFUAModel.UFUATag)
                {
                    var tag = (UFUAModel.UFUATag)node.Tag;
                    item = tag;
                }
                if (item != null)
                    ValidateNewCellValue(item, propertyName, oldStr, newStr, e);
            }
            finally
            {
                if (e.IsValid)
                    treeListControl.SelectNode(node);
            }
        }

        void ValidateNewCellValue(XPObject obj, string propertyName, string oldValue, string newValue, TreeListCellValidationEventArgs e)
        {
            using (var uow = Document.BeginNestedUnitOfWork())
            {
                var item = uow.GetNestedObject(obj);
                var tagProp = item.GetType().GetProperty(propertyName);
                tagProp.SetValue(item, newValue, null);
                if (item is IDataErrorInfo)
                {
                    var errorInfo = item as IDataErrorInfo;
                    var newStrError = errorInfo[propertyName];
                    if (!String.IsNullOrEmpty(newStrError))
                    {
                        e.IsValid = false;
                        e.SetError(newStrError);
                    }
                }

                if (e.IsValid)
                {
                    Document.AddUndoAction(this, uow.GetParentObject(item), UndoRedoAction.Changed);
                    uow.CommitChanges();
                    UpdateContextObjects(treeListControl);
                }
            }
        }

        void OnHiddenEditor(object sender, TreeListEditorEventArgs e)
        {
            if (sender == treeListView)
            {
                treeListControl.DisableEditing();
                if ((e.Node.Content as TreeItemControl) != null)
                    SetBindingOnProp(e.Node, (e.Node.Content as TreeItemControl).TreeItemInnerObject, "Name");
                treeListControl.SelectNode(e.Node);
            }
            else if (sender == tableView)
                Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(((DataViewBase)sender).DataControl.SelectedItem);
        }

        void OnRefresh(object sender, ExecutedRoutedEventArgs e)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected == null)
                return;

            try
            {
                treeListControl.BeginDataUpdate();
                Refresh(selected);
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }
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
            var wasExpanded = item.IsExpanded;
            if (item.IsExpanded && item.Nodes.Count == 0)
                treeListControl.AddNode(null, item, TreeListControlHelper.DummyNode);
            item.IsExpanded = false;
            ClearNodes(item);
            if (item == itemRoot || NeedToBeExpanded(item))
            {
                treeListControl.AddNode(null, item, TreeListControlHelper.DummyNode);
                if (wasExpanded)
                    item.IsExpanded = true;
            }
        }

        bool NeedToBeExpanded(TreeListNode item)
        {
            if (item.Tag is UFUAModel.UFUATag)
            {
                var ufuatag = item.Tag as UFUAModel.UFUATag;
                if (FilterType != FilterType.Historians &&
                    (!String.IsNullOrEmpty(ufuatag.HistorianSettings) ||
                    (from t in ufuatag.UFUAAlarmThresholds
                     where t.UFUAAlarmDefinitionRef != null &&
                     t.UFUAAlarmDefinitionRef.UFUAAlarmDefinitions != null
                     select t).FirstOrDefault() != null ||
                    ufuatag.UFUAViews.Count > 0 ||
                    (ufuatag.ModelType == UFUAModel.ModelType.ObjectType)) ||
                    FilterType == FilterType.Historians && (ufuatag.ModelType == UFUAModel.ModelType.ObjectType))
                    return true;
            }
            else if (item.Tag is UFUAModel.UFUAFolder)
            {
                if (Properties.Settings.Default.FolderAlwaysExpandible)
                    return true;

                var folder = (UFUAModel.UFUAFolder)item.Tag;
                var tags = Document.GetTagCollection(folder);
                var subFolders = Document.GetFolderCollection(folder);
                return (tags != null && tags.Count > 0) || (subFolders != null && subFolders.Count > 0);
            }
            return false;
        }

        private void CanRefresh(object sender, CanExecuteRoutedEventArgs e)
        {
            var selectedItem = treeListControl.GetSelectedNodes().FirstOrDefault();
            e.CanExecute = selectedItem != null && selectedItem.IsExpanded;
        }

        private void OnShownEditor(object sender, EditorEventArgs e)
        {
            if (e.Column.FieldName == "HistorianSettings")
            {
                var settings = e.Column.EditSettings as ComboBoxEditSettings;
                List<String> historicalsList = new List<string>() { "" };
                foreach (var historical in Document.GetHistoricalSettings())
                    historicalsList.Add(historical.Name);
                settings.ItemsSource = historicalsList;
            }
            else if (e.Column.FieldName == "UFUAEngineeringUnit")
            {
                var settings = e.Column.EditSettings as ComboBoxEditSettings;
                var euList = Document.GetEngineeringUnitNames();
                euList.Insert(0, "");
                settings.ItemsSource = euList;
            }
            else if (e.Column == ViewsColumn)
            {
                var settings = e.Column.EditSettings as ComboBoxEditSettings;
                var viewsList = Document.GetViewsNameList();
                //viewsList.Insert(0, "");
                settings.ItemsSource = viewsList;
            }
            else if (e.Column.FieldName == "PrototypeName")
            {
                var settings = e.Column.EditSettings as ComboBoxEditSettings;
                var protoList = Document.GetPrototypesNames();
                protoList.Insert(0, "");
                settings.ItemsSource = protoList;
            }
        }

        protected override void OnValidateCell(object sender, GridCellValidationEventArgs e)
        {
            if (!(e.Row is UFUAModel.UFUATag))
                return;

            var oldCellValue = e.CellValue;
            var newCellValue = e.Value;

            if (oldCellValue != newCellValue)
            {
                using (var uow = Document.BeginNestedUnitOfWork())
                {
                    bool isChanged = false;
                    var tag = uow.GetNestedObject((UFUAModel.UFUATag)e.Row);
                    if (e.Column == ViewsColumn)
                    {
                        var oldList = oldCellValue as List<object>;
                        var newList = newCellValue as List<object>;

                        if (oldList == null)
                            oldList = new List<object>();
                        if (newList == null)
                            newList = new List<object>();

                        if (oldList.Count != newList.Count || oldList.Except(newList).Any())
                        {
                            isChanged = true;
                            foreach (var view in tag.UFUAViews.ToList())
                            {
                                tag.UFUAViews.Remove(view);
                                tag.NotifyPropertyChanged("UFUAViews");
                                tag.NotifyPropertyChanged("Views");
                                view.UFUATags.Remove(tag);
                                view.NotifyPropertyChanged("UFUATags");
                            }

                            foreach (var v in newList)
                            {
                                var name = v as String;
                                if (String.IsNullOrEmpty(name))
                                    continue;

                                var view = uow.GetNestedObject(Document.GetView(name));
                                if (view != null)
                                {
                                    tag.UFUAViews.Add(view);
                                    tag.NotifyPropertyChanged("UFUAViews");
                                    tag.NotifyPropertyChanged("Views");
                                    view.UFUATags.Add(tag);
                                    view.NotifyPropertyChanged("UFUATags");
                                }
                            }
                        }
                    }
                    else
                    {
                        isChanged = true;
                        var tagProp = typeof(UFUAModel.UFUATag).GetProperty(e.Column.FieldName);
                        if (newCellValue != null)
                            tagProp.SetValue(tag, Convert.ChangeType(newCellValue, newCellValue.GetType()), null);
                        else
                            tagProp.SetValue(tag, null, null);
                        var error = tag[e.Column.FieldName];
                        if (!String.IsNullOrEmpty(error))
                        {
                            e.SetError(error);
                            e.IsValid = false;
                        }
                    }

                    if (isChanged && e.IsValid)
                    {
                        if (IsUndoRedoSupported(tag))
                            Document.AddUndoAction(this, uow.GetParentObject(tag), UndoRedoAction.Changed);
                        uow.CommitChanges();
                        UpdateContextObjects(gridDataControl);
                    }
                }
            }
        }

        #endregion

        #region Import and Export

        internal void ImportDriverTags(UFUAModel.UFUACommunicationDriver driver)
        {
            var ret = new List<UFUAModel.UFUATag>();
            var uidll = UFUAServerInfo.UFUAServerInfo.GetDriversUIName(String.Format("{0}\\{1}", UFUAServerInfo.UFUAServerInfo.GetDriversFolder(), driver.AssemblyName));

            var uiInterface = Document.EditorManagerComponent.UIInterface;
            DriverSettingsInterfaces.ICommunicationDriverWpfEditing driverWpfEditing = null;
            try
            {
                var types = System.Reflection.Assembly.LoadFile(uidll).GetTypes();
                var list = (from t in types/*.AsParallel()*/
                            where !t.IsAbstract && typeof(DriverSettingsInterfaces.ICommunicationDriverWpfEditing).IsAssignableFrom(t)
                            select (DriverSettingsInterfaces.ICommunicationDriverWpfEditing)Activator.CreateInstance(t)).ToList();

                driverWpfEditing = list[0];

                if (driverWpfEditing == null || driverWpfEditing.GeneralSettingsEditor == null)
                    return;
            }
            catch (Exception ex)
            {
                if (uiInterface != null)
                    uiInterface.ShowInformation(String.Format(Properties.Resources.CommDriverNotFound, uidll));

                return;
            }

            var control = driverWpfEditing.ImportTagsEditor;
            control.DataContext = new List<string>() { Document.ConnectionString, Document.Protected.ToString(), Document.Id.ToString() };
            GeneralDialogContent Dialog = new GeneralDialogContent(control)
            {
                DialogKeepContent = true,
                Title = driver.FriendlyName,
                Owner = Application.Current.Windows.Count > 0 ? Application.Current.Windows[0] : Application.Current.MainWindow
            };

            if (Dialog.ShowDialog() == true)
            {
                //Process the list of candidates selected by import UserControl.
                if (driverWpfEditing.CompleteImport(control))
                {
                    var importObject = control.DataContext as UFUAModel.ImportObject;
                    if (importObject != null)
                    {                        
                        var tagsToImport = importObject.TagsToImport;
                        var prototypesToImport = importObject.PrototypesToImport;

                        var tokenSource = new CancellationTokenSource();
                        var cancellationToken = tokenSource.Token;

                        try
                        {
                            ManageImportProgressDialog(tagsToImport, prototypesToImport, tokenSource);
                            CreatePrototypesAndTags(tagsToImport, prototypesToImport, importObject, cancellationToken);
                        }
                        catch (OperationCanceledException ex)
                        {
                            if (tagsImportProgressViewModel.IsAborting)
                            {
                                this.UndoAction();
                                Document.RemoveImportedPrototypes(prototypes);
                            }
                            UFUAServerDocument.logGeneral.Info(Properties.Resources.TagsImportCanceled);
                        }
                        finally
                        {                            
                            tagsImportProgressViewModel?.CloseProgressBarImport();
                            tokenSource.Dispose();
                        }
                    }
                }
            }

            if (control is IDisposable)
                (control as IDisposable).Dispose();
        }

        private void ManageImportProgressDialog(
            List<ImportTag> tagsToImport,
            List<ImportPrototype> prototypesToImport,
            CancellationTokenSource tokenSource)
        {
            var waitEvent = new ManualResetEvent(false);

            tagsImportProgressViewModel = new TagsImportProgressViewModel(prototypesToImport.Count, tagsToImport.Count);

            var threadImportProgressDialog = new Thread(() =>
            {
                var captions = GeneralDialogContent.GetDefaultButtonCaptions();
                captions[GeneralDialogButtons.CancelButton] = Properties.Resources.TagsImportProgressCloseButtonLabel;

                var tagsImportProgressUserControl = new TagsImportProgress() { DataContext = tagsImportProgressViewModel };

                GeneralDialogContent tagsImportDialog = new GeneralDialogContent(
                    tagsImportProgressUserControl,
                    GeneralDialogButtons.CancelButton,
                    captions)
                {
                    DialogKeepContent = true,
                    Title = Properties.Resources.TagsImportProgressTitle,
                };

                tagsImportDialog.SetIsTopmost(true);

                bool bIsHidden = false;
                bool bIsClosing = false;
                var dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;

                tagsImportProgressViewModel.NotifyTagsImportProgressWindowToClose += (o, e) =>
                {
                    dispatcher.InvokeIfRequired(() =>
                    {
                        bIsClosing = true;
                        tokenSource.Cancel();
                        tagsImportDialog.Close();
                        dispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
                    });
                };

                tagsImportDialog.Closing += (o, e) =>
                {
                    e.Cancel = !bIsClosing;
                    if (!bIsHidden && !tokenSource.IsCancellationRequested)
                    {
                        tokenSource.Cancel();
                        tagsImportProgressViewModel.IsAborting = true;
                        tagsImportDialog.DisableCancelButton();                        
                    }
                };

                tagsImportProgressViewModel.NotifyTagsImportProgressWindowToHide += (o, e) =>
                {
                    dispatcher.InvokeIfRequired(() =>
                    {
                        bIsHidden = true;
                        tagsImportDialog.Hide();
                    });
                };

                tagsImportProgressViewModel.NotifyTagsImportProgressWindowToAppear += (o, e) =>
                {
                    dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                    {
                        bIsHidden = false;
                        tagsImportDialog.ShowDialog();
                    });
                };

                tagsImportProgressViewModel.NotifyToDisableAbortButton += (sender, e) =>
                {
                    dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                    {
                        tagsImportDialog.DisableCancelButton();
                    });
                };                    

                waitEvent.Set();
                tagsImportDialog.ShowDialog();

                System.Windows.Threading.Dispatcher.Run();
            });

            threadImportProgressDialog.SetApartmentState(ApartmentState.STA);
            threadImportProgressDialog.IsBackground = true;
            threadImportProgressDialog.Start();
            waitEvent.WaitOne();
            waitEvent.Dispose();
        }

        private void CreatePrototypesAndTags(
            List<ImportTag> tagsToImport,
            List<ImportPrototype> prototypesToImport,
            ImportObject importObject,
            CancellationToken cancellationToken)
        {
            CreatePrototypes(tagsToImport, prototypesToImport, importObject, cancellationToken);
            CreateTags(tagsToImport, importObject, cancellationToken);
        }

        private void CreatePrototypes(
            List<ImportTag> tagsToImport,
            List<ImportPrototype> prototypesToImport,
            ImportObject importObject,
            CancellationToken cancellationToken)
        {
            prototypes = new List<UFUATagPrototype>();
            List<ImportPrototype> currentPrototypes = null;
            List<ImportPrototype> searchPrototypes = null;
            if (importObject.SearchForCompatiblePrototypes)
            {
                currentPrototypes = new List<ImportPrototype>();

                foreach (var p in Document.GetPrototypes())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    currentPrototypes.Add(new ImportPrototype(p));
                }

                searchPrototypes = new List<ImportPrototype>(currentPrototypes);
                searchPrototypes.AddRange(prototypesToImport);
            }

            CustomDialogResults dialogRetValue = CustomDialogResults.None;

            var importedPrototypesCounter = 0;

            foreach (var prototypeToImport in prototypesToImport)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string newName = null;
                if (importObject.SearchForCompatiblePrototypes)
                {
                    var pFound = (from c in currentPrototypes.AsParallel()
                                    where c.IsCompatible(prototypeToImport, searchPrototypes)
                                    select c).FirstOrDefault();
                    if (pFound != null)
                        newName = pFound.Name;
                }

                //Add a prototype, if already present with a different structure, change the prototype name.
                //In this last case, update the prototype name for every tag to import that is related to the renamed prototype.
                if (String.IsNullOrEmpty(newName))
                {
                    var uFUATagPrototype = Document.AddImportedPrototype(prototypeToImport, tagsImportProgressViewModel, ref dialogRetValue);
                    if (dialogRetValue == CustomDialogResults.Cancel)
                        return;
                    newName = uFUATagPrototype.Name;
                    if (importObject.SearchForCompatiblePrototypes)
                        currentPrototypes.Add(new UFUAModel.ImportPrototype(uFUATagPrototype));

                    prototypes.Add(uFUATagPrototype);
                }
                if (prototypeToImport.Name != newName)
                {
                    //Rename related tags
                    var tagToRename = (from tag in tagsToImport where tag.PrototypeModel == prototypeToImport.Name select tag).ToList();
                    for (int ii = 0; ii < tagToRename.Count; ii++)
                        tagToRename[ii].PrototypeModel = newName;
                    for (int ii = 0; ii < prototypesToImport.Count; ii++)
                        prototypesToImport[ii].ReplacePrototypeReferenceName(prototypeToImport.Name, newName);
                }

                if (tagsImportProgressViewModel != null)
                {
                    importedPrototypesCounter++;
                    tagsImportProgressViewModel.SetPrototypesCounterValue(importedPrototypesCounter);
                }                
            }

            Document.AddImportedPrototypes(prototypes);
        }

        List<UFUATagPrototype> prototypes;

        private void CreateTags(List<ImportTag> tagsToImport, ImportObject importObject, CancellationToken cancellationToken)
        {
            var tagsUndoList = new List<IXPSimpleObject>();
            try
            {
                var dialogRetValue = CustomDialogResults.None;
                treeListControl.ClearSelection();
                UnSubscribeSelectionChangedEvent = true;
                treeListControl.BeginDataUpdate();
                var importedTagsCounter = 0;

                while (tagsToImport.Count > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var aggregated = (from c in tagsToImport.AsParallel()
                                        where c.Folder == tagsToImport[0].Folder
                                        select c).ToList();

                    tagsToImport.RemoveAll((c) => c.Folder == aggregated[0].Folder);

                    TreeListNode parent = itemRoot;
                    UFUAModel.UFUAFolder root = null;
                    if (!String.IsNullOrEmpty(aggregated[0].Folder))
                    {
                        string szFolder = ((string)aggregated[0].Folder).Replace("\\", "/");
                        var folders = szFolder.Split('/');
                        foreach (var folder in folders)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var folderName = UFUAModel.Helpers.NameValidator.EnsureValidName(folder);
                            var listFolders = Document.GetFolderCollection(root);

                            var parentFolder = root;
                            root = (from c in listFolders.AsParallel()
                                    where c.Name == folderName
                                    select c).FirstOrDefault();

                            if (root == null)
                            {
                                root = Document.AddNewFolder(parentFolder);
                                root.Name = folderName;

                                var item = AddTreeItem(root, parent);
                                if (item != null)
                                {
                                    parent = item;
                                    treeListControl.SelectNode(item);
                                    tagsUndoList.Add(root);
                                }
                            }
                            else
                            {
                                var item = treeListControl.GetTreeItem(root, parent);
                                if (item != null)
                                {
                                    parent = item;
                                    treeListControl.SelectNode(item);
                                }
                            }
                        }
                    }

                    var expanded = parent.IsExpanded || parent.Nodes.Count > 0 && parent.Nodes[0].Tag != TreeListControlHelper.DummyNode;
                    var importedTags = Document.AddImportedTag(aggregated, root, ref dialogRetValue);

                    foreach (var tag in importedTags)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        TreeListNode item = null;
                        if (expanded)
                            item = AddTreeItem(tag, parent);
                        else
                            item = treeListControl.GetTreeItem(tag, parent);
                        if (item != null)
                        {
                            treeListControl.SelectNode(item);
                            tagsUndoList.Add(tag);
                        }

                        if (tagsImportProgressViewModel != null)
                        {
                            importedTagsCounter++;
                            tagsImportProgressViewModel.SetTagsCounterValue(importedTagsCounter);
                        }
                    }

                    if (dialogRetValue == UIMsgBoxAlertService.ComponentService.CustomDialogResults.Cancel)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        break;
                    }
                }
            }
            finally
            {
                tagsImportProgressViewModel?.DisableAbortButton();
                EndTagsUpdate(tagsUndoList);
            }
        }

        private void EndTagsUpdate(List<IXPSimpleObject> tagsUndoList)
        {
            treeListControl.EndDataUpdate();
            FlatGridRefresh();    
            UnSubscribeSelectionChangedEvent = false;
            ForceSelectionChangedEvent();
            if (tagsUndoList.Count > 0)
            {
                var editView = Document.ActiveView as UFUAEditorControl;
                if (editView != null)
                    editView.AddTagFolderList(tagsUndoList);
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
                if (c is TreeItemControl)
                {
                    var innerOject = (c as TreeItemControl).TreeItemInnerObject;
                    if (innerOject != null && mapObjectToNode.ContainsKey(innerOject))
                        draggedNodes.Add(mapObjectToNode[innerOject]);
                }
            }

            bool bAllow = true;
            foreach (var dragItem in draggedNodes)
            {
                if ((!(dragItem.Tag is UFUAModel.UFUATag) && !(dragItem.Tag is UFUAModel.UFUAFolder)) ||
                    (dragItem.Tag is UFUAModel.UFUATag) && (dragItem.Tag as UFUAModel.UFUATag).IsPrototypeMember ||
                    (dragItem.Tag is UFUAModel.UFUAFolder) && (dragItem.Tag as UFUAModel.UFUAFolder).IsPrototypeMember)
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
            var bDroppingOnRoot = itemRoot.RowHandle == e.TargetRowHandle;

            if (draggedNodes.Count() == 0 || targetContent == null || e.OriginalSource != treeListView)
                return;

            TreeListNode targetItem = null;
            if (bDroppingOnRoot)
                targetItem = itemRoot;
            else if (mapObjectToNode.ContainsKey(targetContent))
                targetItem = mapObjectToNode[targetContent];
            if (targetItem == null || (targetItem != itemRoot && !(targetItem.Tag is UFUAModel.UFUAFolder)))
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
                var folder = targetItem.Tag as UFUAModel.UFUAFolder;
                var mapStartCounter = new Dictionary<string, ulong>();
                var listTagName = Document.GetTagsNameList(folder);
                var listFolderName = Document.GetFoldersNameList(folder);
                var parentFoldersToUpdate = new List<TreeListNode>();

                var atLeastOneNameAlreadyExists = (from c in draggingItems
                                                   where (c.Tag is UFUAModel.UFUATag && listTagName.Contains((c.Tag as UFUAModel.UFUATag).Name) ||
                                                   (c.Tag is UFUAModel.UFUAFolder && listFolderName.Contains((c.Tag as UFUAModel.UFUAFolder).Name)))
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
                    Document.AddUndoAction(this, listundo, UndoRedoAction.Removed);

                var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(Document.GetSession(), Document.GetSession(), false, true, true);
                foreach (var dragItem in draggingItems)
                {
                    var parent = dragItem.ParentNode ?? itemRoot;
                    if (parent.Nodes.Count > maxItems && !parentFoldersToUpdate.Contains(parent) && (parent == itemRoot || parent.Tag is UFUAModel.UFUAFolder))
                        parentFoldersToUpdate.Add(parent);
                    if (dragItem.Tag is UFUAModel.UFUATag)
                    {
                        parent.Nodes.Remove(dragItem);
                        var tag = cloneHelper.Clone(dragItem.Tag as UFUAModel.UFUATag);
                        (dragItem.Tag as UFUAModel.UFUATag).Delete();
                        dragItem.Tag = tag;
                        tag.Name = Document.NewTagName(folder, tag.Name, mapStartCounter, listTagName);
                        tag.UFUAFolder = folder;
                        Document.EnsureValidNodeId(tag);
                    }
                    else if (dragItem.Tag is UFUAModel.UFUAFolder)
                    {
                        parent.Nodes.Remove(dragItem);
                        var tag = cloneHelper.Clone(dragItem.Tag as UFUAModel.UFUAFolder);
                        (dragItem.Tag as UFUAModel.UFUAFolder).Delete();
                        dragItem.Tag = tag;
                        tag.Name = Document.NewFolderName(folder, tag.Name, mapStartCounter, listFolderName);
                        tag.UFUAFolderAss = folder;
                        Document.EnsureValidNodeId(tag);
                    }
                }

                listundo = (from item in draggingItems
                            where IsUndoRedoSupported(item.Tag) && !draggingItems.Contains(item.ParentNode)
                            select item.Tag as IXPSimpleObject).ToList();

                if (listundo.Count > 0)
                    Document.AddUndoAction(this, listundo, UndoRedoAction.Added);

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
                        treeListControl.AddDummyNodeIfNeeded(targetItem, NeedToBeExpanded);
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

        //private void TreeListControl_DragRecordOver(object sender, DragRecordOverEventArgs e)
        //{
        //    var data = (RecordDragDropData)e.Data.GetData(typeof(RecordDragDropData));
        //    if (data.Records != null && data.Records.Count() > 0)
        //    {
        //        var node = treeListControl.View.GetNodeByContent(data.Records[0]);
        //        var parentNode = node.ParentNode;

        //        if (parentNode.Content == e.TargetRecord || (e.TargetRecord as TreeItemControl)?.InnerControl == null || (e.TargetRecord as TreeItemControl)?.InnerControl.GetType() == (node.Content as TreeItemControl)?.InnerControl.GetType())
        //        {
        //            e.Effects = DragDropEffects.None;
        //            return;
        //        }
        //        //var targetNode = ((TreeListView)e.OriginalSource).GetNodeByContent(e.TargetRecord);
        //        //var parentTargetNode = targetNode.ParentNode;
        //        //var parentRowItem = parentNode.Content;
        //        //var parentTargetRowItem = parentTargetNode.Content;
        //    }
        //}

        void TreeListControl_CompletedDragDrop(object sender, CompleteRecordDragDropEventArgs e)
        {
            e.Handled = true;
        }

        private void OnSearchStringChanged(object sender, SearchStringToFilterCriteriaEventArgs e)
        {
            bSearching = true;
            e.Filter = new FunctionOperator("ContainsCaseSensitive", new OperandProperty("Name"), e.SearchString);
        }

        private void OnFilterChanged(object sender, RoutedEventArgs e)
        {
            bSearching = false;
            if (String.IsNullOrEmpty(tableView.SearchString?.Trim()))
                SearchResultsCount = String.Empty;
            else
                SearchResultsCount = String.Format(Properties.Resources.SearchItemsFound, gridDataControl.VisibleItems.Count);
        }

        public class ContainsCaseSensitive : ICustomFunctionOperator
        {
            public bool IsCaseSensitive { get; set; }
            public string Name
            {
                get
                {
                    return "ContainsCaseSensitive";
                }
            }
            public object Evaluate(params object[] operands)
            {
                if (operands.Count() == 2 && operands[0] is string && operands[1] is string)
                {
                    return IsCaseSensitive ? ((string)operands[0]).Contains((string)operands[1]) : operands[0].ToString().ToLower().Contains(operands[1].ToString().ToLower());
                }
                return true;
            }
            public Type ResultType(params Type[] operands)
            {
                return typeof(bool);
            }
        }

        #endregion

        #region Commands

        RelayCommand collapseAllCommand;
        public ICommand CollapseAllCommand
        {
            get
            {
                if (collapseAllCommand == null)
                {
                    collapseAllCommand = new RelayCommand(
                        param =>
                        {
                            using (new AsyncWaitCursor())
                            {
                                tableView.CollapseAllNodes();
                            }
                        },
                        param => gridDataControl.VisibleItems?.Count > 0
                        );
                }
                return collapseAllCommand;
            }
        }

        RelayCommand expandAllCommand;
        public ICommand ExpandAllCommand
        {
            get
            {
                if (expandAllCommand == null)
                {
                    expandAllCommand = new RelayCommand(
                        param =>
                        {
                            using (new AsyncWaitCursor())
                            {
                                tableView.ExpandAllNodes();
                            }
                        },
                        param => gridDataControl.VisibleItems?.Count > 0
                        );
                }
                return expandAllCommand;
            }
        }

        RelayCommand replaceTextCommand;
        public ICommand ReplaceTextCommand
        {
            get
            {
                if (replaceTextCommand == null)
                {
                    replaceTextCommand = new RelayCommand(
                        param =>
                        {
                            using (new AsyncWaitCursor())
                            {
                                Dictionary<UFUAModel.UFUATag, string> tagNewNameMap = new Dictionary<UFUAModel.UFUATag, string>();
                                var listundo = new List<IXPSimpleObject>();
                                int rejected = 0;
                                var list = (from UFUAModel.UFUATag t in gridDataControl.VisibleItems where !t.IsSubPrototypeMember select t).ToList();
                                foreach (var t in list)
                                {
                                    var tag = t as UFUAModel.UFUATag;
                                    string result;
                                    if (IsSearchCaseSensitive)
                                        result = System.Text.RegularExpressions.Regex.Replace(tag.Name, tableView.SearchString, param as String);
                                    else
                                        result = System.Text.RegularExpressions.Regex.Replace(tag.Name, tableView.SearchString, param as String, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                                    if (result != tag.Name)
                                    {
                                        if (Document.TagNameExists(result, tag.UFUAFolder))
                                        {
                                            rejected++;
                                        }
                                        else
                                        {
                                            tagNewNameMap.Add(tag, result);
                                            listundo.Add(tag as IXPSimpleObject);
                                        }
                                    }
                                }
                                if (listundo.Count > 0)
                                    Document.AddUndoAction(this, listundo, UndoRedoAction.Replaced);
                                foreach (var tag in tagNewNameMap.Keys)
                                    tag.Name = tagNewNameMap[tag];
                                var message = String.Format(Properties.Resources.ReplacedItemsCountMessage, tagNewNameMap.Count);
                                if (rejected > 0)
                                {
                                    message = String.Format("{0}{1}{2}", message, Environment.NewLine,
                                        String.Format(Properties.Resources.RejectedItemsCountMessage, rejected));
                                }
                                MessageBox.Show(message);
                            }
                        },
                        param => !String.IsNullOrEmpty(param as String) && !String.IsNullOrEmpty(tableView.SearchString?.Trim()) && !bSearching && gridDataControl.VisibleItems?.Count > 0
                        );
                }
                return replaceTextCommand;
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
            e.CanExecute = IsAnyItemSelected() && tabAddressSpaceTabControl.SelectedItem == treeListTab && FilterType == FilterType.None;
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
            e.CanExecute = IsAnyItemSelected() && tabAddressSpaceTabControl.SelectedItem == treeListTab && FilterType == FilterType.None;
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
            e.CanExecute = tabAddressSpaceTabControl.SelectedItem == treeListTab && (Document.ClipboardContainsTags() || Document.ClipboardContainsFolders());
            e.Handled = !e.CanExecute;
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
            e.CanExecute = IsAnyUndoActionAvailable() && FilterType == FilterType.None;
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
            e.CanExecute = IsAnyRedoActionAvailable() && FilterType == FilterType.None;
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
            e.CanExecute = IsAnyItemSelected() && CanEdit() && FilterType == FilterType.None;
        }

        private void OnAddNewTag(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            UFUAModel.UFUATag tag = Document.AddNewTag(GetSelectedFolder());

            if (!isPopup && Document.EditorManagerComponent.PropertyControl != null)
            {
                AddTag(tag);
                Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
            //using (var t = new TemporaryContextObject(Document.EditorManagerComponent.Workspace, tag))
            {

                var newTagControl = new NewTag(Document, false)
                {
                    DataContext = tag
                };

                GeneralDialogContent Dialog = new GeneralDialogContent(newTagControl)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "AddNewTag"
                };

                if (Dialog.ShowDialog() == true)
                {
                    AddTag(tag);
                }
                else
                {
                    tag.NodeId = Guid.Empty;
                    tag.Delete();
                };
            }
        }

        private void CanAddNewTag(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true && FilterType == FilterType.None;
        }

        private void OnAddNewFolder(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            UFUAModel.UFUAFolder folder = Document.AddNewFolder(GetSelectedFolder());

            if (!isPopup && Document.EditorManagerComponent.PropertyControl != null)
            {
                AddFolder(folder);
                Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
            {
                var newFolderControl = new NewFolder()
                {
                    DataContext = folder
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newFolderControl, GeneralDialogButtons.OkCancelButtons)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = ""
                };
                if (Dialog.ShowDialog() == true)
                {
                    AddFolder(folder);
                }
                else
                {
                    folder.Delete();
                }
            }
        }

        private void CanAddNewFolder(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true && FilterType == FilterType.None;
        }

        private void OnRemoveItem(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                DeleteSelectedItems();
            }
        }

        private void CanRemoveItem(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected() && FilterType == FilterType.None;
        }

        private void OnAssignHistoricalSettings(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var historicalList = new HistoricalPrototypeList(Document, true);
            GeneralDialogContent Dialog = new GeneralDialogContent(historicalList)
            {
                Title = String.Format("{0}: {1}", Properties.Resources.TagHeaderAssign, Properties.Resources.HistoricalRibbonHeader),
                Owner = this.FindParent<Window>(),
                HelpLink = "AssignHistorical"
            };
            if (Dialog.ShowDialog() == true)
            {
                var hs = historicalList.GetSelectedItem();
                if (hs != null)
                {
                    AssignItemToSelect(hs);
                }
            }
        }

        private void CanAssignHistoricalSettings(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected() && CanAssignItem() && FilterType == FilterType.None;
        }

        private void OnAssignAlarmDefinition(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (alarmList == null)
            {
                var prototypeList = new AlarmPrototypeList(Document, true);
                alarmList = new AssignAlarmDefinition() { DataContext = new AssignAlarmViewModel(prototypeList) };
            }
            GeneralDialogContent Dialog = new GeneralDialogContent(alarmList)
            {
                DialogKeepContent = true,
                Title = String.Format("{0}: {1}", Properties.Resources.TagHeaderAssign, Properties.Resources.AlarmRibbonHeader),
                Owner = this.FindParent<Window>(),
                HelpLink = "AssignAlarm"
            };
            if (Dialog.ShowDialog() == true)
            {
                var model = alarmList.DataContext as AssignAlarmViewModel;
                var hs = model.SelectedAlarmDefinitions;
                if (hs != null)
                {
                    AssignItemsToSelect(hs, model);
                }
            }
        }

        private void CanAssignAlarmDefinition(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected() && CanAssignItem() && FilterType == FilterType.None;
        }

        private void CanRemoveAlarmDefinitions(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected() && CanDeleteAlarms() && FilterType == FilterType.None;
        }

        private void OnRemoveAlarmDefinitions(object sender, ExecutedRoutedEventArgs e)
        {
            using (new AsyncWaitCursor())
            {
                var items = new List<UFUAModel.UFUATag>();
                var alarms = new List<UFUAModel.UFUAAlarmThreshold>();
                var listundo = new List<IXPSimpleObject>();

                if (gridControlTab.IsSelected)
                {
                    foreach (var item in gridDataControl.SelectedItems)
                    {
                        if (item is UFUAModel.UFUATag && CanDeleteAlarms(item as UFUAModel.UFUATag))
                            items.Add(item as UFUAModel.UFUATag);
                    }
                }
                else
                {
                    items.AddRange((from item in treeListControl.GetSelectedNodes()
                                    where item.Tag is UFUAModel.UFUATag && CanDeleteAlarms(item.Tag as UFUAModel.UFUATag)
                                    select item.Tag as UFUAModel.UFUATag));
                }

                foreach (var tag in items)
                {
                    alarms.AddRange(tag.UFUAAlarmThresholds);
                    listundo.AddRange((from c in alarms
                                       where IsUndoRedoSupported(c, UndoRedoAction.Removed)
                                       select c));
                }

                if (listundo.Count > 0)
                    Document.AddUndoAction(this, listundo, UndoRedoAction.Removed);

                try
                {
                    treeListControl.BeginDataUpdate();
                    foreach (var alarm in alarms)
                        RemoveAlarmFromTag(alarm);
                }
                finally
                {
                    treeListControl.EndDataUpdate();
                }
            }
        }

        void RemoveAlarmFromTag(UFUAModel.UFUAAlarmThreshold alarm)
        {
            if (mapObjectToNode.ContainsKey(alarm))
                mapObjectToNode[alarm].ParentNode.Nodes.Remove(mapObjectToNode[alarm]);

            var tag = alarm.UFUATagAss;
            if (tag == null || !tag.UFUAAlarmThresholds.Contains(alarm))
                return;

            tag.UFUAAlarmThresholds.Remove(alarm);
            alarm.Delete();
            tag.NotifyPropertyChanged("UFUAAlarmThresholds");
        }

        private void OnAssignView(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var viewList = new ViewList(Document, true);
            GeneralDialogContent Dialog = new GeneralDialogContent(viewList)
            {
                Title = String.Format("{0}: {1}", Properties.Resources.TagHeaderAssign, TranslatableMenu.Properties.Resources.ViewRibbonTitle),
                Owner = this.FindParent<Window>(),
                HelpLink = "AssociateView"
            };
            if (Dialog.ShowDialog() == true)
            {
                var hs = viewList.GetSelectedItem();
                if (hs != null)
                {
                    AssignItemToSelect(hs);
                }
            }
        }

        private void CanAssignView(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected() && CanAssignView() && FilterType == FilterType.None;
        }

        #endregion

        #region Undo/Redo

        bool IsAnyUndoActionAvailable()
        {
            return isPopup && Document.UndoContainsSomething() && Document.GetNextUndoOwner() == typeName;
        }

        bool IsAnyRedoActionAvailable()
        {
            return isPopup && Document.RedoContainsSomething() && Document.GetNextRedoOwner() == typeName;
        }

        protected override bool IsUndoRedoSupported(object obj)
        {
            return IsUndoRedoSupported(obj, UndoRedoAction.None);
        }

        bool IsUndoRedoSupported(object obj, UndoRedoAction action)
        {
            if (obj is UFUAModel.UFUAFolder)
            {
                var item = (obj as UFUAModel.UFUAFolder);
                if (!item.IsPrototypeMember)
                    return true;
            }
            else if (obj is UFUAModel.UFUATag)
            {
                var item = (obj as UFUAModel.UFUATag);
                if (!item.IsPrototypeMember || action != UndoRedoAction.Removed && item.IsSubPrototypeMember)
                    return true;
            }
            else if (obj is UFUAModel.UFUAAlarmThreshold)
            {
                var item = (obj as UFUAModel.UFUAAlarmThreshold);
                if (item.UFUATagAss != null && (!item.UFUATagAss.IsPrototypeMember || item.UFUATagAss.IsSubPrototypeMember))
                    return true;
            }

            return false;
        }

        internal void UndoAction()
        {
            UndoRedoAction action;
            var list = Document.UndoAction(this, out action);
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
                case UndoRedoAction.Replaced:
                    {
                        if (list.Count > 0)
                        {
                            bool selectionchanged = false;
                            TreeListNode item = null;
                            try
                            {
                                treeListControl.ClearSelection();
                                UnSubscribeSelectionChangedEvent = true;
                                treeListControl.BeginDataUpdate();
                                foreach (var obj in list)
                                {
                                    if (obj.Source is UFUAModel.UFUATag)
                                        (obj.Source as UFUAModel.UFUATag).NotifyPropertyChanged("Views");

                                    if (mapObjectToNode.ContainsKey(obj.Source))
                                    {
                                        var parent = mapObjectToNode[obj.Source].ParentNode;
                                        item = treeListControl.GetTreeItem(obj.Source, parent);
                                        if (item != null)
                                        {
                                            Refresh(item);
                                            selectionchanged = true;
                                        }
                                    }
                                }
                                FlatGridRefresh();
                            }
                            finally
                            {
                                treeListControl.EndDataUpdate();
                                if (item != null)
                                    treeListControl.SelectNode(item);
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
                            bool selectionchanged = false;
                            List<TreeListNode> items = new List<TreeListNode>();
                            try
                            {
                                treeListControl.ClearSelection();
                                UnSubscribeSelectionChangedEvent = true;
                                treeListControl.BeginDataUpdate();
                                foreach (var obj in list)
                                {
                                    var parent = itemRoot;
                                    var parentTag = Document.GetParentObject(obj);
                                    if (parentTag != null && mapObjectToNode.ContainsKey(parentTag))
                                        parent = mapObjectToNode[parentTag];
                                    var item = AddTreeItem(obj.Source, parent);
                                    if (item != null)
                                    {
                                        selectionchanged = true;
                                        items.Add(item);
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
                case UndoRedoAction.Replaced:
                    {
                        if (list.Count > 0)
                        {
                            bool selectionchanged = false;
                            TreeListNode item = null;
                            try
                            {
                                treeListControl.ClearSelection();
                                UnSubscribeSelectionChangedEvent = true;
                                treeListControl.BeginDataUpdate();
                                foreach (var obj in list)
                                {
                                    if (obj.Source is UFUAModel.UFUATag)
                                        (obj.Source as UFUAModel.UFUATag).NotifyPropertyChanged("Views");

                                    if (mapObjectToNode.ContainsKey(obj.Source))
                                    {
                                        var parent = mapObjectToNode[obj.Source].ParentNode;
                                        item = treeListControl.GetTreeItem(obj.Source, parent);
                                        if (item != null)
                                        {
                                            Refresh(item);
                                            selectionchanged = true;
                                        }
                                    }
                                }
                                FlatGridRefresh();
                            }
                            finally
                            {
                                treeListControl.EndDataUpdate();
                                if (item != null)
                                    treeListControl.SelectNode(item);
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
                            bool selectionchanged = false;
                            TreeListNode lastItem = null;
                            try
                            {
                                treeListControl.ClearSelection();
                                UnSubscribeSelectionChangedEvent = true;
                                treeListControl.BeginDataUpdate();
                                foreach (var obj in list)
                                {
                                    var parent = itemRoot;
                                    var parentTag = Document.GetParentObject(obj);
                                    if (parentTag != null && mapObjectToNode.ContainsKey(parentTag))
                                        parent = mapObjectToNode[parentTag];
                                    var item = AddTreeItem(obj.Source, parent);
                                    if (item != null)
                                    {
                                        selectionchanged = true;
                                        lastItem = item;
                                    }
                                }
                                FlatGridRefresh();
                            }
                            finally
                            {
                                treeListControl.EndDataUpdate();
                                if (lastItem != null)
                                    treeListControl.SelectNode(lastItem);
                                UnSubscribeSelectionChangedEvent = false;
                                if (selectionchanged)
                                    ForceSelectionChangedEvent();
                            }
                        }
                    }
                    break;
            }
        }

        #endregion

        #region Remote Script Debugger
        internal void AttachDebuggerOnSelected()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUAModel.UFUATag)
            {
                var tag = selected.Tag as UFUAModel.UFUATag;

                var remoteScripteDebugger = new RemoteScriptDebugger(Document, tag.NodeId);
                var Dialog = new GeneralDialogContent(remoteScripteDebugger)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "AttachDebugger"
                };
                if (Dialog.ShowDialog() == true)
                {
                }
            }
        }

        internal bool CanAttachDebuggerOnSelected()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is UFUAModel.UFUATag)
            {
                var tag = selected.Tag as UFUAModel.UFUATag;
                if (!String.IsNullOrEmpty(tag.ScriptCode) &&
                    (tag.Breakpoints == null || tag.Breakpoints.Length == 0) &&
                    Document.ServerCMSHelperAsync.IsServerRunning)
                    return true;
            }
            return false;
        }
        #endregion

        #region INotifyPropertyChanged
        public void OnPropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

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
                if (!isPopup)
                    return FilterType.None;
                return filterType;
            }

            set
            {
                if (!isPopup)
                    return;
                filterType = value;

                treeListControl.RefreshData();
                gridDataControl.RefreshData();
            }
        }

        TargetType targetType = TargetType.None;
        public TargetType TargetType
        {
            get
            {
                if (!isPopup)
                    return TargetType.None;
                return targetType;
            }

            set
            {
                if (!isPopup)
                    return;
                targetType = value;
            }
        }

        SelectionMode selectionMode = SelectionMode.SingleRow;
        public SelectionMode SelectionMode
        {
            get
            {
                if (!isPopup)
                    return SelectionMode.SingleRow;
                return selectionMode;
            }

            set
            {
                if (!isPopup)
                    return;
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

        #region ISelectEntityReference
        public object SelectedReference { get; set; }

        public List<object> SelectedReferences { get; set; }
        UFUAModel.UFUATag selectedInstanceOnEdit;
        Dictionary<string, UFUAModel.UFUATag> referenceToTag = new Dictionary<string, UFUAModel.UFUATag>();
        Dictionary<UFUAModel.UFUATag, string> tagToReference = new Dictionary<UFUAModel.UFUATag, string>();
        public void BringIntoView(object selectedReference)
        {
            if (bDisposed || selectedReference == null || !(selectedReference is OPCUAViewModel.OPCUAEntityReference))
                return;

            if (!isPopup)
                return;
            var tag = (selectedReference as OPCUAViewModel.OPCUAEntityReference);
            string tagString = tag.ToXml();
            if (!referenceToTag.ContainsKey(tagString))
            {
                string fullPath = tag.HumanReadableNoProject;
                string instance = fullPath;
                var split = fullPath.Split(':');
                string member = null;
                if (split.Length > 1)
                {
                    string path = NamespaceTableConverter.GetRelativePath(tag.RelativePath);
                    instance = path.Replace($"\\{split[1]}", "");
                    member = split[1];
                }
                selectedInstanceOnEdit = Document.GetUFUATag(instance, member, true);
                UpdateReferences(tag, selectedInstanceOnEdit);
            }
            else
                selectedInstanceOnEdit = referenceToTag[tagString];

            if (bLoaded)
                UpdateSelectedItem();
        }

        private void UpdateReferences(OPCUAViewModel.OPCUAEntityReference entity, UFUAModel.UFUATag tag)
        {
            if (tag == null)
            {
                if (entity == null)
                    return;
                string tagString = entity.ToXml();

                (from key in tagToReference.Keys where tagToReference[key] == tagString select key).ToList().ForEach(k => tagToReference.Remove(k));
                if (referenceToTag.ContainsKey(tagString))
                    referenceToTag.Remove(tagString);
            }
            else
            {
                string tagString = entity.ToXml();

                if (!referenceToTag.ContainsKey(tagString))
                    referenceToTag.Add(tagString, tag);
                else
                    referenceToTag[tagString] = tag;

                if (!tagToReference.ContainsKey(tag))
                    tagToReference.Add(tag, tagString);
                else
                    tagToReference[tag] = tagString;
            }
        }

        void UpdateSelectedItem()
        {
            if (!isPopup)
                return;

            if (bLoaded && selectedInstanceOnEdit != null)
            {
                UnSubscribeSelectionChangedEvent = true;
                var nodesToSelect = new List<TreeListNode>();
                try
                {
                    tabAddressSpaceTabControl.SelectedItem = treeListTab;
                    if (mapObjectToNode.ContainsKey(selectedInstanceOnEdit) && mapObjectToNode[selectedInstanceOnEdit] != null)
                    {
                        nodesToSelect.Add(mapObjectToNode[selectedInstanceOnEdit]);
                    }
                    else
                    {
                        ExpandParentNode(selectedInstanceOnEdit, null);
                        if (mapObjectToNode.ContainsKey(selectedInstanceOnEdit) && mapObjectToNode[selectedInstanceOnEdit] != null)
                        {
                            nodesToSelect.Add(mapObjectToNode[selectedInstanceOnEdit]);
                        }
                    }
                }
                finally
                {
                    treeListControl.SelectNode(itemRoot);
                    UnSubscribeSelectionChangedEvent = false;
                    treeListControl.SelectNodes(nodesToSelect, false, true);
                    selectedInstanceOnEdit = null;
                }
            }
        }

        void ExpandParentNode(UFUAModel.UFUATag uFUATag, UFUAModel.UFUAFolder uFUAFolder)
        {
            if (uFUATag != null)
            {
                if (uFUATag.UFUAFolder != null)
                {
                    if (mapObjectToNode.ContainsKey(uFUATag.UFUAFolder) && mapObjectToNode[uFUATag.UFUAFolder] != null)
                        mapObjectToNode[uFUATag.UFUAFolder].IsExpanded = true;
                    else
                    {
                        ExpandParentNode(null, uFUATag.UFUAFolder);
                        if (mapObjectToNode.ContainsKey(uFUATag.UFUAFolder) && mapObjectToNode[uFUATag.UFUAFolder] != null)
                            mapObjectToNode[uFUATag.UFUAFolder].IsExpanded = true;
                    }
                }
                else if (uFUATag.IsSubPrototypeMember && uFUATag.PrototypeReference.UFUATagOwner != null)
                {
                    if (mapObjectToNode.ContainsKey(uFUATag.PrototypeReference.UFUATagOwner) && mapObjectToNode[uFUATag.PrototypeReference.UFUATagOwner] != null)
                        mapObjectToNode[uFUATag.PrototypeReference.UFUATagOwner].IsExpanded = true;
                    else
                    {
                        ExpandParentNode(uFUATag.PrototypeReference.UFUATagOwner, null);
                        if (mapObjectToNode.ContainsKey(uFUATag.PrototypeReference.UFUATagOwner) && mapObjectToNode[uFUATag.PrototypeReference.UFUATagOwner] != null)
                            mapObjectToNode[uFUATag.PrototypeReference.UFUATagOwner].IsExpanded = true;
                    }
                }
            }
            if (uFUAFolder != null)
            {
                if (uFUAFolder.UFUAFolderAss != null)
                {
                    if (mapObjectToNode.ContainsKey(uFUAFolder.UFUAFolderAss) && mapObjectToNode[uFUAFolder.UFUAFolderAss] != null)
                        mapObjectToNode[uFUAFolder.UFUAFolderAss].IsExpanded = true;
                    else
                    {
                        ExpandParentNode(null, uFUAFolder.UFUAFolderAss);
                        if (mapObjectToNode.ContainsKey(uFUAFolder.UFUAFolderAss) && mapObjectToNode[uFUAFolder.UFUAFolderAss] != null)
                            mapObjectToNode[uFUAFolder.UFUAFolderAss].IsExpanded = true;
                    }
                }
                else if (uFUAFolder.IsSubPrototypeMember && uFUAFolder.PrototypeReference.UFUATagOwner != null)
                {
                    if (mapObjectToNode.ContainsKey(uFUAFolder.PrototypeReference.UFUATagOwner) && mapObjectToNode[uFUAFolder.PrototypeReference.UFUATagOwner] != null)
                        mapObjectToNode[uFUAFolder.PrototypeReference.UFUATagOwner].IsExpanded = true;
                    else
                    {
                        ExpandParentNode(uFUAFolder.PrototypeReference.UFUATagOwner, null);
                        if (mapObjectToNode.ContainsKey(uFUAFolder.PrototypeReference.UFUATagOwner) && mapObjectToNode[uFUAFolder.PrototypeReference.UFUATagOwner] != null)
                            mapObjectToNode[uFUAFolder.PrototypeReference.UFUATagOwner].IsExpanded = true;
                    }
                }
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

            tagToReference.Clear();
            referenceToTag.Clear();

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

            if (alarmList != null && alarmList.prototypeList.Content is IDisposable)
                (alarmList.prototypeList.Content as IDisposable).Dispose();
        }
        #endregion

        private void OnShownEditor(object sender, TreeListEditorEventArgs e)
        {
            if (e.Column.FieldName == "HistorianSettings")
            {
                var settings = e.Column.EditSettings as ComboBoxEditSettings;
                List<String> historicalsList = new List<string>() { "" };
                foreach (var historical in Document.GetHistoricalSettings())
                    historicalsList.Add(historical.Name);
                settings.ItemsSource = historicalsList;
            }
            else if (e.Column.FieldName == "UFUAEngineeringUnit")
            {
                var settings = e.Column.EditSettings as ComboBoxEditSettings;
                var euList = Document.GetEngineeringUnitNames();
                euList.Insert(0, "");
                settings.ItemsSource = euList;
            }
            else if (e.Column == ViewsColumn)
            {
                var settings = e.Column.EditSettings as ComboBoxEditSettings;
                var viewsList = Document.GetViewsNameList();
                //viewsList.Insert(0, "");
                settings.ItemsSource = viewsList;
            }
            else if (e.Column.FieldName == "PrototypeName")
            {
                var settings = e.Column.EditSettings as ComboBoxEditSettings;
                var protoList = Document.GetPrototypesNames();
                protoList.Insert(0, "");
                settings.ItemsSource = protoList;
            }
        }

        private void tableView_ShowingEditor(object sender, TreeListShowingEditorEventArgs e)
        {
            var tag = gridDataControl.GetRow(e.RowHandle) as UFUAModel.UFUATag;
            if (tag.IsSubPrototypeMember)
                e.Cancel = true;
        }
        void OnGridTreeNodeExpanding(object sender, TreeListNodeAllowEventArgs e)
        {
            TreeListNode item = e?.Node;
            if (item == null)
                return;
            var tag = e.Row as UFUAModel.UFUATag;
            if (tag == null || string.IsNullOrEmpty(tag.PrototypeModel))
                return;
            UnSubscribeSelectionChangedEvent = true;
            try
            {
                gridDataControl.BeginDataUpdate();
                var proto = Document?.CreateSubPrototype(tag);
                var members = proto?.GetTagMembers();
                item.Nodes.Clear();
                if (members != null)
                    foreach (var member in members)
                    {
                        var node = new TreeListNode(member);
                        node.IsExpandButtonVisible = !string.IsNullOrEmpty(member.PrototypeModel) ? DevExpress.Utils.DefaultBoolean.True : DevExpress.Utils.DefaultBoolean.False;
                        item.Nodes.Add(node);
                    }
            }
            finally
            {
                gridDataControl.EndDataUpdate();
                UnSubscribeSelectionChangedEvent = false;
            }
        }
    }
    public class CustomChildrenSelector : IChildNodesSelector
    {
        List<object> dummyList = new List<object>() { { new object() } };
        public UFUAServerDocument Document { get; set; }
        System.Collections.IEnumerable IChildNodesSelector.SelectChildren(object item)
        {
            var tag = item as UFUAModel.UFUATag;
            if (tag == null || string.IsNullOrEmpty(tag.PrototypeModel))
                return null;
            return dummyList;
        }
    }
}

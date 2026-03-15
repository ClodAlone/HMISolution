using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DevExpress.Xpo;
using TempVariablesManager.Document;
using Utilities;
using Utilities.WPF;
using System.ComponentModel;
using UFInterfaces;
using TempVariablesModel;
using TempVarriables.ComponentService;
using WPFUtilities;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid.TreeList;
using TempVariablesManager.Helpers;
using System.Windows.Data;
using UIMsgBoxAlertService.ComponentService;
using UFInterfaces.Editors;
using DevExpress.Data.TreeList;

namespace TempVariablesManager.Controls
{
    /// <summary>
    /// Interaction logic for ControlEditor.xaml
    /// </summary>
    public partial class ControlEditor : UserControl, IEditableObject, IDisposable, ITempVarControl, ISelectEntityReference
    {
        #region Declarations

        readonly Dictionary<Object, TreeListNode> mapObjectToNode = new Dictionary<Object, TreeListNode>();
        readonly Dictionary<Object, TreeListNode> mapObjectToParent = new Dictionary<Object, TreeListNode>();
        readonly Dictionary<Object, TreeListNode> mapObjectToOldParent = new Dictionary<Object, TreeListNode>();

        readonly TempVariables variables;
        readonly TempVariablesPersistence Document;
        readonly FrameworkElement activeView;

        TreeListNode itemRoot;
        TreeListNode selectedItem;

        BitmapImage openFolderImg;
        BitmapImage closedFolderImg;

        static int maxItems = Properties.Settings.Default.MaxItemsInTree;
        bool UnSubscribeSelectionChangedEvent;

        bool bLoaded;
        bool isPopup;
        bool isPopupWasClosed;
        bool bNeedToReload;

        #endregion


        public ControlEditor(TempVariables tempvar, bool bPopup = true)
        {
            InitializeComponent();
            variables = tempvar;
            Document = variables.CurrentDocument;

            isPopup = bPopup;

            if (!isPopup && Workspace != null)
                activeView = Workspace.ActiveWindow;

            Document.CreateUndoRedoHelper(this);

            gridDataControl.ItemsSource = Document.GetFlatTagCollection();

            if (bPopup)
            {
                treeListControl.SelectionMode = MultiSelectMode.None;
                toolbar.Visibility = Visibility.Visible;
                Document.PropertyChanged += Document_PropertyChanged;

                Loaded += (o, e) =>
                {
                    var wnd = this.FindParent<Window>();
                    if (wnd != null)
                    {
                        wnd.Activated += (s, c) =>
                        {
                            if (isPopupWasClosed)
                            {
                                isPopupWasClosed = false;
                                if (bNeedToReload)
                                {
                                    bNeedToReload = false;
                                    gridDataControl.ItemsSource = Document.GetFlatTagCollection();
                                    InitializeAddressSpace();
                                }
                            }
                            
                        };

                        wnd.Closing += (s, c) =>
                        {
                            isPopupWasClosed = true;
                            var selected = selectedItem;
                            selectedItem = null;

                            Document.NeedToReloadAddressSpace = bNeedToReload;

                            //if (Document.NeedsSave)
                            //{
                            //    Document.NeedToReloadAddressSpace = true;
                            //    //Document.SaveToFile();
                            //    tempVariables.LoadMap(true);
                            //};

                            Variable ufuatag = null;
                            if (tabControlEditorTabControl.SelectedItem == gridControlTab)
                            {
                                ufuatag = gridDataControl.SelectedItem as Variable;
                            }
                            else
                            {
                                selected = selectedItem ?? treeListControl.GetSelectedNodes().FirstOrDefault();
                                if (selected != null && selected.Tag is Variable)
                                {
                                    ufuatag = selected.Tag as Variable;
                                }
                            }
                            if (ufuatag != null)
                            {
                                var entity = variables.GetReference(ufuatag);
                                UpdateReferences(entity, ufuatag);
                                SelectedReference = entity;
                                SelectedReferences = null;
                            }
                            else
                            {
                                SelectedReference = null;
                                SelectedReferences = null;
                            }
                        };
                    }

                    if (bNeedToReload)
                    {
                        bNeedToReload = false;
                        gridDataControl.ItemsSource = Document.GetFlatTagCollection();
                        InitializeAddressSpace();
                    }
                    else if (!bLoaded)
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
                Document.PropertyChanged += Document_PropertyChanged;

                Loaded += (o, e) =>
                {
                    if (bNeedToReload)
                    {
                        bNeedToReload = false;
                        gridDataControl.ItemsSource = Document.GetFlatTagCollection();
                        InitializeAddressSpace();
                    }
                    else if (!bLoaded)
                    {
                        bLoaded = true;
                        InitializeAddressSpace();
                    }
                };
            }
        }

        private void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (isPopup)
            {
                if (e.PropertyName == "NeedsSave")
                {
                    if (Document.NeedsSave)
                        bNeedToReload = true;
                }
            }
            else
            {
                if (e.PropertyName == "NeedsSave")
                {
                    PropertyControl_AcceptChanges();
                }
                if (e.PropertyName == "NeedToReloadAddressSpace")
                {
                    if (Document.NeedToReloadAddressSpace)
                        bNeedToReload = true;
                }
            }
        }

        void workspace_PromptDocumentEditorObject(object sender, GetDocumentEditorObjectEventArgs e)
        {
            if (!(sender is IXPSimpleObject))
                return;

            IXPSimpleObject source = sender as IXPSimpleObject;
            e.documentEditor = this;
        }

        void PropertyControl_AcceptChanges()
        {
            if (!isPopup && Workspace != null && activeView != null)
                Workspace.SetChangedDocumentTitle(activeView, Document.NeedsSave);
        }

        #region ITempVarControl
        public void ClearSelection()
        {
            treeListControl.ClearSelection();
        }

        public String AddNewTag(String name, String type)
        {
            var nodesToSelect = new List<TreeListNode>();

            Folder folder = null;
            int foundFolder = name.IndexOf('&');
            var parent = itemRoot;
            while (foundFolder != -1)
            {
                var folderName = name.Remove(foundFolder);
                name = name.Substring(foundFolder + 1);

                var f = Document.FindFolderByName(folderName, folder);
                if (f != null)
                {
                    folder = f;
                    if (mapObjectToParent.ContainsKey(folder))
                        parent = mapObjectToParent[folder];
                    parent.IsExpanded = true;
                    var item = treeListControl.GetTreeItem(folder, parent);
                    if (item != null)
                    {
                        parent = item;
                        nodesToSelect.Add(item);
                    }
                }
                else
                {
                    parent.IsExpanded = true;
                    folder = Document.AddNewFolder(folder);
                    folder.Name = folderName;
                    var item = AddFolder(folder, parent);
                    if (item != null)
                    {
                        parent = item;
                        nodesToSelect.Add(item);
                    }
                }

                foundFolder = name.IndexOf('&');
            }

            Variable tag = Document.AddNewTag(folder);
            tag.Name = name;
            variables.AddTempVariable(tag); //.GetRelativeName());


            if (type == typeof(bool).Name)
                tag.DataType = UFUAModel.DataType.Boolean;
            else if (type == typeof(SByte).Name)
                tag.DataType = UFUAModel.DataType.SByte;
            else if (type == typeof(Byte).Name)
                tag.DataType = UFUAModel.DataType.Byte;
            else if (type == typeof(Int16).Name)
                tag.DataType = UFUAModel.DataType.Int16;
            else if (type == typeof(Int32).Name)
                tag.DataType = UFUAModel.DataType.Int32;
            else if (type == typeof(Int64).Name)
                tag.DataType = UFUAModel.DataType.Int64;
            else if (type == typeof(UInt32).Name)
                tag.DataType = UFUAModel.DataType.UInt32;
            else if (type == typeof(UInt64).Name)
                tag.DataType = UFUAModel.DataType.UInt64;
            else if (type == typeof(float).Name)
                tag.DataType = UFUAModel.DataType.Float;
            else if (type == typeof(double).Name)
                tag.DataType = UFUAModel.DataType.Double;
            else 
                tag.DataType = UFUAModel.DataType.String;

            var parentTag = itemRoot;
            if (folder != null && mapObjectToParent.ContainsKey(folder))
            {
                parentTag = mapObjectToParent[folder];
                parentTag = treeListControl.GetTreeItem(folder, parentTag);
            }

            var toSelect = AddTag(tag, parentTag);
            if (toSelect != null)
                nodesToSelect.Add(toSelect);

            if (nodesToSelect.Count > 0)
                treeListControl.SelectNodes(nodesToSelect, false, true);

            return tag.Name;
        }

        public void AddNewTag()
        {
            Variable tag = Document.AddNewTag(GetSelectedFolder());

            if (!isPopup && Document.PropertyControl != null)
            {
                AddTag(tag);
                Document.PropertyControl.Activate();
            }
            else
            {
                var newTagControl = new NewTag(Document)
                {
                    DataContext = tag
                };

                GeneralDialogContent Dialog = new GeneralDialogContent(newTagControl)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "TempVariableEditor"
                };
                if (Dialog.ShowDialog() == true)
                {
                    AddTag(tag);
                }
                else
                {
                    tag.Delete();
                };
            }
        }

        public void AddNewFolder()
        {
            Folder folder = Document.AddNewFolder(GetSelectedFolder());

            if (!isPopup && Document.PropertyControl != null)
            {
                AddFolder(folder);
                Document.PropertyControl.Activate();
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

        public void OnActivate()
        {
            Document.ActiveView = this;
            if (Workspace != null)
            {
                Workspace.ContextDocument = Document;
                UpdateContextObjects();
            }
        }

        void UpdateContextObjects()
        {
            UpdateContextObjects(gridControlTab.IsSelected);
        }

        void UpdateContextObjects(bool bUseGridSelectedItems)
        {
            if (!isPopup && Workspace != null)
            {
                if (bUseGridSelectedItems)
                {
                    if (gridDataControl.SelectedItems.Count == 0)
                    {
                        Workspace.ContextObject = null;
                    }
                    else if (gridDataControl.SelectedItems.Count == 1)
                    {
                        Workspace.ContextObject = Document.GetNestedObject(gridDataControl.SelectedItem);
                    }
                    else
                    {
                        Workspace.ContextObjects = Document.GetNestedObjects(gridDataControl.SelectedItems);
                    }
                }
                else
                {
                    var selecteditems = treeListControl.GetTreeSelectedItems();
                    if (selecteditems.Count == 0)
                    {
                        Workspace.ContextObject = null;
                    }
                    else if (selecteditems.Count == 1)
                    {
                        Workspace.ContextObject = Document.GetNestedObject(selecteditems[0]);
                    }
                    else
                    {
                        Workspace.ContextObjects = Document.GetNestedObjects(selecteditems);
                    }
                }
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
            openFolderImg = TempVariables.GetBitmapImage("OpenFolderSmall", true);
            closedFolderImg = TempVariables.GetBitmapImage("CloseFolderSmall", true);
            Document.NeedToReloadAddressSpace = false;

            treeListView.Nodes.Clear();
            itemRoot = treeListControl.AddNode(new TempVarTreeItemControl(Properties.Resources.AddressSpaceHeader) { ResourceIcon = closedFolderImg }, Tag as TreeListNode, Document);
            treeListControl.AddNode(null, itemRoot, TreeListControlHelper.DummyNode);

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                if (bDisposed)
                    return;

                FillItems(itemRoot);
                if (itemRoot.Nodes.Count == 0)
                    treeListControl.AddNode(null, itemRoot, TreeListControlHelper.DummyNode);
                if (selectedInstanceOnEdit != null)
                    UpdateSelectedItem();
                else
                    itemRoot.IsExpanded = true;
            });

            gridDataControl.SelectedItemChanged += (s, e) =>
            {
                e.Handled = true;
                if (!UnSubscribeSelectionChangedEvent)
                {
                    EndEdit();

                    try
                    {
                        UnSubscribeSelectionChangedEvent = true;

                        treeListControl.ClearSelection();
                        foreach(object o in gridDataControl.SelectedItems)
                        {
                            if (mapObjectToParent.ContainsKey(o))
                            {
                                var parent = mapObjectToParent[o];
                                var item = treeListControl.GetTreeItem(o, parent);
                                if (item != null)
                                    treeListControl.SelectNode(item);
                            }
                        };

                        UpdateContextObjects(true);
                    }
                    finally
                    {
                        UnSubscribeSelectionChangedEvent = false;
                    }
                }
            };

            //gridDataControl.Model.ClipboardCanCut += (s, e) =>
            //{
            //    e.Handled = true;
            //};
        }

        void OnTreeNodeCollapsing(object sender, TreeListNodeAllowEventArgs e)
        {
            if (!(e.Node.Content as TreeItemControl).IsNodeExpanding)
                UpdateFolderIcon(e.Node, false);
        }

        void UpdateFolderIcon(TreeListNode node, bool isOpen)
        {
            if (node != null && (node == itemRoot || node.Tag is Folder))
                (node.Content as TreeItemControl).ResourceIcon = isOpen ? openFolderImg : closedFolderImg;
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
                if (node.Tag is Variable)
                    (node.Tag as INotifyPropertyChanged).PropertyChanged -= Variable_PropertyChanged;
                foreach (var child in node.Nodes)
                    ClearObjectMapNode(child);
            }
        }

        void OnTreeNodeChanged(object sender, TreeListNodeChangedEventArgs e)
        {
            if (e.ChangeType == NodeChangeType.Add && e.Node.Tag != null && e.Node.Tag != TreeListControlHelper.DummyNode && !mapObjectToNode.ContainsKey(e.Node.Tag))
            {
                mapObjectToNode.Add(e.Node.Tag, e.Node);
                if (e.Node.Tag is Variable)
                    (e.Node.Tag as INotifyPropertyChanged).PropertyChanged += Variable_PropertyChanged;
            }
            else if (e.ChangeType == NodeChangeType.Remove && e.Node.Tag != null && e.Node.Tag != TreeListControlHelper.DummyNode)
                ClearObjectMapNode(e.Node);
        }

        private void Variable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataType")
            {
                if (mapObjectToNode.ContainsKey(sender) && sender is Variable)
                {
                    var item = mapObjectToNode[sender];
                    if (item != null)
                        (item.Content as TreeItemControl).ResourceIcon = variables.GetTagBitmapImage((sender as Variable).DataType);
                }
            }
        }

        private void FillItems(TreeListNode iRoot, Folder root = null)
        {
            treeListControl.BeginDataUpdate();

            if (!isPopup && root == null && Workspace != null)
                Workspace.IsBusy = true;

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
                                //AddTreeItem(Properties.Resources.MaxItemCountVisibleReachedDoubleClick, itemRoot);
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
                                //AddTreeItem(Properties.Resources.MaxItemCountVisibleReachedDoubleClick, itemRoot);
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();

                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    if (!isPopup && root == null && Workspace != null)
                    {
                        Workspace.IsBusy = false;
                    }
                });
            }
        }

        private void FillItems(TreeListNode itemRoot, Variable tag)
        {
            using (new AsyncWaitCursor())
            {
                ClearNodes(itemRoot);
            }
        }

        private static bool CanBeExpanded(TreeListNode parent)
        {
            return parent.Nodes.Count == 1 && parent.Nodes[0] == TreeListControlHelper.DummyNode;
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
                    if (item.Tag is Folder || item == itemRoot)
                        UpdateFolderIcon(item, true);
                    return;
                }

                (item.Content as TreeItemControl).IsNodeExpanding = true;
                treeListControl.BeginDataUpdate();

                ClearNodes(item);
                if (item.Tag is Folder)
                {
                    UpdateFolderIcon(item, true);
                    FillItems(item, item.Tag as Folder);
                }
                else if (item.Tag is Variable)
                    FillItems(item, item.Tag as Variable);

                treeListControl.EndDataUpdate();
            }
            finally
            {
                (item.Content as TreeItemControl).IsNodeExpanding = false;
            }
        }

        public class NameModel
        {
            public String Name { get; set; }
        }

        TreeListNode AddTreeItem(Object tag, TreeListNode parent)
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

            var ic = new TempVarTreeItemControl(tag);
            var newitem = treeListControl.AddNode(ic, parent, tag);

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
            bool bKnownType = false;

            if (tag is Folder)
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
            else if (tag is String)
            {
                bKnownType = true;
                (newitem.Content as TreeItemControl).Header = tag as String;
                (newitem.Content as TreeItemControl).ResourceIcon = closedFolderImg;
            }
            else if (tag is Variable)
            {
                bKnownType = true;
                var t = tag as Variable;

                SetBindingOnProp(newitem, tag, nameof(t.Name));
                SetBindingOnProp(newitem, tag, nameof(t.DataType), TempVarTreeItemControl.DataTypeProperty);
                SetBindingOnProp(newitem, tag, nameof(t.Description), TempVarTreeItemControl.DescriptionProperty);

                (newitem.Content as TreeItemControl).ResourceIcon = variables.GetTagBitmapImage(t.DataType);

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
            BindingOperations.SetBinding(node.Content as HeaderedItemsControl, targetDP, myBinding);
        }

        public bool IsAnyItemSelected()
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            var griditem = gridDataControl.SelectedItem;

            return item != null && item != itemRoot || griditem != null;
        }

        // Added to solve FOGBUGZ 11409
        void SelectItems(IList<Variable> list, TreeListNode parent = null)
        {
            var toSelect = new List<TreeListNode>();
            foreach (var tag in list)
            {
                var item = treeListControl.GetTreeItem(tag, parent);
                if (item != null)
                    toSelect.Add(item);
            }
            if (toSelect.Count > 0)
                treeListControl.SelectNodes(toSelect);
        }

        void EditSelectedItem()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null)
                Edit(selected);
        }

        void DeleteSelectedItems()
        {
            var items = new List<TreeListNode>();
            var gitems = new List<Variable>();
            var listundo = new List<IXPSimpleObject>();

            if (gridControlTab.IsSelected)
            {
                foreach (var item in gridDataControl.SelectedItems)
                {
                    if (item is Variable)
                        gitems.Add((item as Variable));
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
                                   where IsUndoRedoSupported(item.Tag) &&
                                   !items.Contains(item.ParentNode)
                                   select item.Tag as IXPSimpleObject).ToList());

                var uiMsgBox = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                {
                    var bShowMessage = (from item in items/*.AsParallel()*/
                                        where item.Tag is Folder &&
                                        listundo.Contains(item.Tag as Folder) &&
                                        ((item.Tag as Folder).Variables.Count > 0 ||
                                        (item.Tag as Folder).Folders.Count > 0)
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
                Document.AddUndoAction(this, listundo, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Removed);

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

        void DeleteTreeItem(TreeListNode item)
        {
            //var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            var parent = item.ParentNode as TreeListNode ?? itemRoot;
            if (item.Tag is Variable)
            {
                var tag = item.Tag as Variable;
                parent.Nodes.Remove(item);
                if (tagToReference.ContainsKey(tag))
                {
                    if (referenceToTag.ContainsKey(tagToReference[tag]))
                        referenceToTag.Remove(tagToReference[tag]);
                    tagToReference.Remove(tag);
                }
                tag.Delete();

                if (IsUndoRedoSupported(tag))
                    mapObjectToOldParent[tag] = item;

                var maxitem = treeListControl.GetTreeItem(Properties.Resources.MaxItemCountVisibleReachedDoubleClick, parent);
                if (maxitem != null)
                {
                    parent.Nodes.Remove(maxitem);
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        FillItems(parent, parent.Tag as Folder);
                    });
                }
            }
            else if (item.Tag is Folder)
            {
                var folder = item.Tag as Folder;
                //GetSelectedParentFolder(true).Items.Remove(item);
                parent.Nodes.Remove(item);
                folder.Delete();

                if (IsUndoRedoSupported(folder))
                    mapObjectToOldParent[folder] = item;

                var maxitem = treeListControl.GetTreeItem(Properties.Resources.MaxItemCountVisibleReachedDoubleClick, parent);
                if (maxitem != null)
                {
                    parent.Nodes.Remove(maxitem);
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        FillItems(parent, parent.Tag as Folder);
                    });
                }
            }
        }

        void CopySelectedToClipboard()
        {
            var listfolders = new List<Folder>();
            var listtags = new List<Variable>();
            foreach (var item in treeListControl.GetSelectedNodes())
            {
                if ((item as TreeListNode).Tag is Folder)
                    listfolders.Add((item as TreeListNode).Tag as Folder);
                else if ((item as TreeListNode).Tag is Variable)
                    listtags.Add((item as TreeListNode).Tag as Variable);
            }

            Document.CleanClipbaord();
            Document.CopyListFoldersToClipbaord(listfolders);
            Document.CopyListTagsToClipbaord(listtags);
        }

        void PasteFromClipboard()
        {
            List<TreeListNode> pastedNodes = new List<TreeListNode>();
            try
            {
                var parent = GetSelectedParentFolder();
                var expanded = parent.IsExpanded || parent.Nodes.Count > 0 && parent.Nodes[0].Tag != TreeListControlHelper.DummyNode;

                var parenttag = GetSelectedParentTag();
                var expandedtag = parenttag != null && (parenttag.IsExpanded || parenttag.Nodes.Count > 0 && parenttag.Nodes[0].Tag != TreeListControlHelper.DummyNode);

                treeListControl.ClearSelection();
                UnSubscribeSelectionChangedEvent = true;
                treeListControl.BeginDataUpdate();

                Folder folder = null;
                if (parent.Tag is Folder)
                    folder = parent.Tag as Folder;

                var listUFUAfoldersandtags = Document.PasteClipboardUFUAFoldersAndTags(folder);

                treeListControl.AddDummyNodeIfNeeded(parent, NeedToBeExpanded);
                pastedNodes.AddRange(treeListControl.AddGetNodesList(itemRoot, listUFUAfoldersandtags, expanded, AddTreeItem, parent));

                var listfolders = Document.PasteClipboardFolders(folder);

                treeListControl.AddDummyNodeIfNeeded(parent, NeedToBeExpanded);
                pastedNodes.AddRange(treeListControl.AddGetNodesList(itemRoot, listfolders, expanded, AddTreeItem, parent));

                var listtags = Document.PasteClipboardTags(folder);

                treeListControl.AddDummyNodeIfNeeded(parent, NeedToBeExpanded);
                pastedNodes.AddRange(treeListControl.AddGetNodesList(itemRoot, listtags, expanded, AddTreeItem, parent));
                    
                //if (parenttag != null)
                //{
                //    if (!expandedtag && !CanBeExpanded(parenttag))
                //        treeListControl.AddNode(null, parenttag, TreeListControlHelper.DummyNode);
                //}

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
                if (pastedNodes.Count > 0)
                    treeListControl.SelectNodes(pastedNodes, true, true);
                UnSubscribeSelectionChangedEvent = false;
                if (pastedNodes.Count > 0)
                    ForceSelectionChangedEvent();
            }
        }

        TreeListNode AddFolder(Folder folder, TreeListNode parent)
        {
            Document.AddUndoAction(this, folder, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
            return AddTreeItem(folder, parent);
        }

        void AddFolder(Folder folder)
        {
            Document.AddUndoAction(this, folder, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
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

        TreeListNode AddTag(Variable tag, TreeListNode parent)
        {
            Document.AddUndoAction(this, tag, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
            var item = AddTreeItem(tag, parent);
            FlatGridRefresh();
            return item;
        }

        void AddTag(Variable tag)
        {
            Document.AddUndoAction(this, tag, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
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

        void FlatGridRefresh()
        {
            try
            {
                UnSubscribeSelectionChangedEvent = true;

                gridDataControl.ItemsSource = null;
                gridDataControl.ItemsSource = Document.GetFlatTagCollection();

                var selectedItems = (from c in treeListControl.GetSelectedNodes()
                                     where c.Tag != null
                                     select c.Tag).ToList();
                selectedItems.ForEach((o) => gridDataControl.SelectedItems.Add(o));

                var selNode = treeListControl.GetSelectedNodes().FirstOrDefault();
                if (selNode != null)
                    gridDataControl.SelectedItem = selNode.Tag;
            }
            catch
            { }
            finally
            {
                UnSubscribeSelectionChangedEvent = false;
            }
        }

        Folder GetSelectedFolder()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null && selected.Tag is Folder)
                return selected.Tag as Folder;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is Folder)
                    return selected.Tag as Folder;
            }

            return null;
        }

        bool CanAssignItem()
        {
            return false;
        }

        TreeListNode GetSelectedParentFolder(bool bSkipSelected = false)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (!bSkipSelected && selected != null && selected.Tag is Folder)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is Folder)
                    return selected;
            }

            return itemRoot;
        }

        TreeListNode GetSelectedParentTag(bool bSkipSelected = false)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (!bSkipSelected && selected != null && selected.Tag is Variable)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && selected.Tag is Variable)
                    return selected;
            }

            return null;
        }

        TreeListNode GetParentTag(TreeListNode parent)
        {
            while (parent != null)
            {
                parent = parent.ParentNode;
                if (parent != null && parent.Tag is Variable)
                    return parent;
            }

            return null;
        }

        TreeListNode GetSelectedParent(bool bSkipSelected = false)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (!bSkipSelected && selected != null)
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null)
                    return selected;
            }

            return itemRoot;
        }

        private void EditTag(Variable ufuatag)
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
                    HelpLink = "TempVariableEditor"
                };
                if (Dialog.ShowDialog() == true)
                {
                    Document.AddUndoAction(this, ufuatag, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed);
                    uow.CommitChanges();
                    UpdateContextObjects();
                }
            }
        }

        private void Edit(TreeListNode selected)
        {
            if (selected.Tag is Folder)
            {
                var folder = selected.Tag as Folder;

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
                        FlatGridRefresh();
                    }
                }
            }
            else if (selected.Tag is Variable)
            {
                var ufuatag = selected.Tag as Variable;
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
                    else if (isPopup && selected.Tag is Variable)
                    {
                        var wnd = this.FindParent<Window>();
                        if (wnd != null)
                        {
                            selectedItem = selected;
                            wnd.DialogResult = true;
                            wnd.Close();
                        }
                    }
                    else if (!isPopup)
                    {
                        if (Document.PropertyControl != null)
                            Document.PropertyControl.Activate();
                        else
                            EditSelectedItem();
                    }
                }
            }
            else if(e.Source.DataControl == gridDataControl)
            {
                e.Handled = true;
                var selected = gridDataControl.SelectedItem as Variable;
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
                        if (Document.PropertyControl != null)
                            Document.PropertyControl.Activate();
                        else
                        {
                            var ufuatag = gridDataControl.SelectedItem as Variable;
                            EditTag(ufuatag);
                            FlatGridRefresh();
                        }
                    }
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
            e.CanExecute = IsAnyItemSelected() && tabControlEditorTabControl.SelectedItem == treeListTab;
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
            e.CanExecute = IsAnyItemSelected() && tabControlEditorTabControl.SelectedItem == treeListTab;
            e.Handled = !e.CanExecute;
        }

        private void OnCommandPaste(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                Document.CopyWinClipboardToInMemoryData(true);
                PasteFromClipboard();
            }
        }

        private void CanCommandPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            Document.CopyWinClipboardToInMemoryData();
            e.CanExecute = tabControlEditorTabControl.SelectedItem == treeListTab  && (Document.ClipboardContainsTags() || Document.ClipboardContainsFolders());
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
            if (!isPopup && Document.PropertyControl != null)
                Document.PropertyControl.Activate();
            else
                EditSelectedItem();
        }

        private void CanCommandProperties(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected();
        }

        public void OnAddNewTag(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            AddNewTag();
        }

        public void CanAddNewTag(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void OnAddNewFolder(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            AddNewFolder();
        }

        public void CanAddNewFolder(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
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
            e.CanExecute = IsAnyItemSelected();
        }

        #endregion

        #region Undo/Redo

        bool IsAnyUndoActionAvailable()
        {
            return Document.UndoContainsSomething(this);
        }

        bool IsAnyRedoActionAvailable()
        {
            return Document.RedoContainsSomething(this);
        }

        void CleanUndoActions()
        {
            Document.CleanUndoActions(this);
        }

        void CleanRedoActions()
        {
            Document.CleanRedoActions(this);
        }

        bool IsUndoRedoSupported(object obj)
        {
            return obj is IXPSimpleObject;
        }

        void ForceSelectionChangedEvent()
        {
            treeListControl_SelectionChanged(this, null);
        }
        XPObject selectedobject;
        List<XPObject> selectedobjects;
        void treeListControl_SelectionChanged(object sender, TreeListSelectionChangedEventArgs e)
        {
            if (!isPopup && !UnSubscribeSelectionChangedEvent && Workspace != null)
            {
                EndEdit();

                try
                {
                    UnSubscribeSelectionChangedEvent = true;

                    var selecteditems = treeListControl.GetTreeSelectedItems();

                    gridDataControl.SelectedItems.Clear();
                    selecteditems.ForEach((o) => gridDataControl.SelectedItems.Add(o));
                    UpdateContextObjects();
                }
                finally
                {
                    UnSubscribeSelectionChangedEvent = false;
                }
            }
        }

        void UndoAction()
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
                                UnSubscribeSelectionChangedEvent = true;
                                treeListControl.BeginDataUpdate();
                                foreach (var obj in list)
                                {
                                    if (mapObjectToParent.ContainsKey(obj))
                                    {
                                        var parent = mapObjectToParent[obj];
                                        var item = treeListControl.GetTreeItem(obj, parent);
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
                case XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed:
                    {
                        if (list.Count > 0)
                        {
                            bool selectionchanged = false;
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
                                        var item = treeListControl.GetTreeItem(obj, parent);
                                        if (item != null)
                                        {
                                            Refresh(item);
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
                case XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Removed:
                    {
                        if (list.Count > 0)
                        {
                            bool selectionchanged = false;
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

        void RedoAction()
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
                                UnSubscribeSelectionChangedEvent = true;
                                treeListControl.BeginDataUpdate();
                                foreach (var obj in list)
                                {
                                    if (mapObjectToParent.ContainsKey(obj))
                                    {
                                        var parent = mapObjectToParent[obj];
                                        var item = treeListControl.GetTreeItem(obj, parent);
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
                case XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed:
                    {
                        if (list.Count > 0)
                        {
                            bool selectionchanged = false;
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
                                        var item = treeListControl.GetTreeItem(obj, parent);
                                        if (item != null)
                                        {
                                            Refresh(item);
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
                case XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added:
                    {
                        if (list.Count > 0)
                        {
                            bool selectionchanged = false;
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

        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            tagToReference.Clear();
            referenceToTag.Clear();

            if (itemRoot != null)
                ClearObjectMapNode(itemRoot);


            if (isPopup)
                Document.SaveToFile(discargechanges: true);
            Document.PropertyChanged -= Document_PropertyChanged;

            if (Workspace != null)
                Workspace.ContextObject = null;

            Document.CleanUndoRedoHelper(this);
            
            // gridDataControl.Model.Dispose();
            try
            {
                gridDataControl.Dispose();
            }
            catch (Exception ex)
            {

            }

            mapObjectToNode.Clear();
            mapObjectToParent.Clear();
            mapObjectToOldParent.Clear();
        }
        #endregion

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var wnd = this.FindParent<Window>();
            if (wnd != null)
            {
                wnd.DialogResult = true;
                wnd.Close();
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
                    bResult = Document.UowContext.TryCommitChanges(TempVariables.log);
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
           if (item.Tag is Folder)
           {
                var folder = (Folder)item.Tag;
                return Document.GetTagCollection(folder).Count > 0 || Document.GetFolderCollection(folder).Count > 0;
           }

           return false;
        }

        private void uowContext_BeforeFlushChanges(object sender, DevExpress.Xpo.SessionManipulationEventArgs e)
        {
            if (Workspace == null)
                return;

            var objects = Workspace.ContextObjects;
            if (objects == null && Workspace.ContextObject != null)
                objects = new List<object>() { Workspace.ContextObject };
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
                    Document.AddUndoAction(this, list, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed);
            }
        }

        #endregion IEditableObject Members

        #region Drag&Drop
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
                if (!(dragItem.Tag is Variable) &&
                    !(dragItem.Tag is Folder))
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

            var targetElement = treeListView.GetNodeByContent(e.TargetRecord);
            if (targetElement == null || e.TargetRowHandle == itemRoot.RowHandle ||
                !(targetElement.Tag is Folder) ||
                (e.OriginalSource != treeListView))
            {
                return;
            }

            var draggingItems = new List<TreeListNode>();
            foreach (var dragItem in draggedNodes)
            {
                var parent = dragItem.ParentNode ?? itemRoot;
                if (parent == targetElement || dragItem == targetElement)
                    continue;

                if (dragItem.Tag is IXPSimpleObject)
                    draggingItems.Add(dragItem);
            }
            
            if (draggingItems.Count > 0)
            {
                var folder = targetElement.Tag as Folder;
                var mapStartCounter = new Dictionary<string, ulong>();
                var listTagName = Document.GetTagsNameList(folder);
                var listFolderName = Document.GetFoldersNameList(folder);
                var parentFoldersToUpdate = new List<TreeListNode>();

                var atLeastOneNameAlreadyExists = (from c in draggingItems 
                                                   where (c.Tag is Variable && listTagName.Contains((c.Tag as Variable).Name) ||
                                                   (c.Tag is Folder && listFolderName.Contains((c.Tag as Folder).Name)))
                                                   select c).Any();

                if (atLeastOneNameAlreadyExists)
                {
                    var uiInterface = Document.UIInterface;
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
                    if (parent.Nodes.Count > maxItems && !parentFoldersToUpdate.Contains(parent) && (parent == itemRoot || parent.Tag is Folder))
                        parentFoldersToUpdate.Add(parent);
                    if (dragItem.Tag is Variable)
                    {
                        parent.Nodes.Remove(dragItem);
                        var item = cloneHelper.Clone(dragItem.Tag as Variable);
                        (dragItem.Tag as Variable).Delete();
                        dragItem.Tag = item;
                        item.Name = Document.NewTagName(folder, item.Name, mapStartCounter, listTagName);
                        item.Folder = folder;
                    }
                    else if (dragItem.Tag is Folder)
                    {
                        parent.Nodes.Remove(dragItem);
                        var item = cloneHelper.Clone(dragItem.Tag as Folder);
                        (dragItem.Tag as Folder).Delete();
                        dragItem.Tag = item;
                        item.Name = Document.NewFolderName(folder, item.Name, mapStartCounter, listFolderName);
                        item.FolderAss = folder;
                    }
                }

                listundo = (from item in draggingItems
                            where IsUndoRedoSupported(item.Tag) && !draggingItems.Contains(item.ParentNode)
                            select item.Tag as IXPSimpleObject).ToList();

                if (listundo.Count > 0)
                    Document.AddUndoAction(this, listundo, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);

                var expanded = targetElement.IsExpanded || targetElement.Nodes.Count > 0 && targetElement.Nodes[0].Tag != TreeListControlHelper.DummyNode;
                treeListControl.ClearSelection();
                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    var toSelect = new List<TreeListNode>();
                    try
                    {
                        treeListControl.BeginDataUpdate();
                        treeListControl.AddDummyNodeIfNeeded(targetElement, NeedToBeExpanded);
                        draggingItems.ForEach(itemAdv =>
                        {
                            var tag = itemAdv.Tag as IXPSimpleObject;
                            if (expanded)
                            {
                                var item = AddTreeItem(tag, targetElement);
                                if (item != null)
                                    toSelect.Add(item);
                            }
                            else
                            {
                                var item = treeListControl.GetTreeItem(tag, targetElement);
                                if (item != null)
                                    toSelect.Add(item);
                            }
                        });
                    }
                    finally
                    {
                        treeListControl.EndDataUpdate();
                        if (toSelect.Count > 0)
                            treeListControl.SelectNodes(toSelect, true, true);
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
                });
            }
        }

        void TreeListControl_CompletedDragDrop(object sender, CompleteRecordDragDropEventArgs e)
        {
            e.Handled = true;
        }
        #endregion

        #region Properties
        UFInterfaces.IWorkspace workspace;
        public UFInterfaces.IWorkspace Workspace
        {
            get
            {
                if (workspace == null)
                    workspace = Document.GetService(typeof(UFInterfaces.IWorkspace)) as UFInterfaces.IWorkspace;

                return workspace;
            }
        }
        #endregion

        #region ISelectEntityReference
        public object SelectedReference { get; set; }

        public List<object> SelectedReferences { get; set; }
        Variable selectedInstanceOnEdit;
        Dictionary<string, Variable> referenceToTag = new Dictionary<string, Variable>();
        Dictionary<Variable, string> tagToReference = new Dictionary<Variable, string>();
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
                string path = (selectedReference as OPCUAViewModel.OPCUAEntityReference)?.RelativePath;
                if (string.IsNullOrEmpty(path))
                    return;

                List<Variable> variables = gridDataControl.ItemsSource as List<Variable>;
                selectedInstanceOnEdit = Document.GetVariable(path); 
                UpdateReferences(tag, selectedInstanceOnEdit);
            }
            else
                selectedInstanceOnEdit = referenceToTag[tagString];

            if (bLoaded)
                UpdateSelectedItem();
        }

        private void UpdateReferences(OPCUAViewModel.OPCUAEntityReference entity, Variable tag)
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
            if (bLoaded && selectedInstanceOnEdit != null)
            {
                UnSubscribeSelectionChangedEvent = true;
                var nodesToSelect = new List<TreeListNode>();
                try
                {
                    tabControlEditorTabControl.SelectedItem = treeListTab;
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

        void ExpandParentNode(Variable variable, Folder folder)
        {
            if (variable != null)
            {
                if (variable.Folder != null)
                {
                    if (mapObjectToNode.ContainsKey(variable.Folder) && mapObjectToNode[variable.Folder] != null)
                        mapObjectToNode[variable.Folder].IsExpanded = true;
                    else
                    {
                        ExpandParentNode(null, variable.Folder);
                        if (mapObjectToNode.ContainsKey(variable.Folder) && mapObjectToNode[variable.Folder] != null)
                            mapObjectToNode[variable.Folder].IsExpanded = true;
                    }
                }
            }
            if (folder != null)
            {
                if (folder.FolderAss != null)
                {
                    if (mapObjectToNode.ContainsKey(folder.FolderAss) && mapObjectToNode[folder.FolderAss] != null)
                        mapObjectToNode[folder.FolderAss].IsExpanded = true;
                    else
                    {
                        ExpandParentNode(null, folder.FolderAss);
                        if (mapObjectToNode.ContainsKey(folder.FolderAss) && mapObjectToNode[folder.FolderAss] != null)
                            mapObjectToNode[folder.FolderAss].IsExpanded = true;
                    }
                }
            }
        }
        #endregion

        private void OnValidateCell(object sender, GridCellValidationEventArgs e)
        {
            if (!(e.Row is Variable))
                return;

            var oldCellValue = e.CellValue;
            var newCellValue = e.Value;

            if (oldCellValue != newCellValue)
            {
                using (var uow = Document.BeginNestedUnitOfWork())
                {
                    var tag = uow.GetNestedObject((Variable)e.Row);
                    var tagProp = typeof(Variable).GetProperty(e.Column.FieldName);
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
                    if (e.IsValid)
                    {
                        Document.AddUndoAction(this, uow.GetParentObject(tag), XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed);
                        uow.CommitChanges();
                        UpdateContextObjects(true);
                    }
                }
            }
        }
    }
}

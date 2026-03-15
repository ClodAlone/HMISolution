using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using Utilities;
using Utilities.WPF;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using UFShortcutSettings.Documents;
using UFShortcutSettings.ShortcutModel;
using WPFUtilities;
using UFShortcutEditor.UndoRedo;
using UFShortcutEditor.ComponentService;
using UFShortcutEditor.Controls;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using System.Windows.Data;

namespace UFShortcutEditor
{
    /// <summary>
    /// Interaction logic for UFRecipeEditorUI.xaml
    /// </summary>
    public partial class UFShortcutEditorUI : UserControl, IDisposable
    {
        #region Declarations

        TreeListNode itemRoot;
        BitmapImage openFolderImg;
        BitmapImage closedFolderImg;
        bool UnSubscribeSelectionChangedEvent;

        readonly ShortcutEditorManagerComponent EditorComponent;

        readonly UndoRedoManager undoRedoManager = new UndoRedoManager(100);

        #endregion

		bool bLoaded;
        public bool IsLoaded
        {
            get
            {
                return bLoaded;
            }
        }

        public UFShortcutEditorUI(ShortcutEditorManagerComponent editorComponent, UFShortcutDocument doc)
        {
            InitializeComponent();

            EditorComponent = editorComponent;
            Document = doc;

            InitializeAddressSpace();

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                    bLoaded = true;
            };
            Unloaded += (o, e) =>
            {
                if (bLoaded)
                    bLoaded = false;
            };
        }

        #region Address Space

        class rootHeader
        {
            public String Name { get; set; }
        }

        void InitializeAddressSpace()
        {
            openFolderImg = ShortcutEditorManagerComponent.GetBitmapImage("OpenFolderSmall", true);
            closedFolderImg = ShortcutEditorManagerComponent.GetBitmapImage("CloseFolderSmall", true);

            treeListView.Nodes.Clear();
            itemRoot = treeListControl.AddNode(new TreeItemControl(Document.Title) { ResourceIcon = closedFolderImg }, Tag as TreeListNode, Document.ShortcutEntity);
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
            if (!UnSubscribeSelectionChangedEvent)
            {
                UnSubscribeSelectionChangedEvent = true;

                try
                {
                    OnActivate(true);
                    e.Handled = true;
                }
                finally
                {
                    UnSubscribeSelectionChangedEvent = false;
                }
            }
        }

        internal void OnActivate(bool bNavigating = false)
        {
            List<Object> selecteditems = new List<Object>();
            foreach (TreeListNode item in treeListControl.GetSelectedNodes())
            {
                if (item.Tag != null)
                    selecteditems.Add(item.Tag);
            }

            if (selecteditems.Count == 0)
            {
                EditorComponent.Workspace.ContextObject = null;
            }
            else if (selecteditems.Count == 1)
            {
                EditorComponent.Workspace.ContextObject = selecteditems[0] == itemRoot ? Document : selecteditems[0];
            }
            else
            {
                EditorComponent.Workspace.ContextObjects = selecteditems;
            }

            //Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            //{
            //    if (!bNavigating)
            //    {
            //        treeListControl.Focusable = true;
            //        treeListControl.Focus();
            //    }
            //    var item = TreeViewAdv.GetTreeViewItemFromChildren(treeListControl.SelectedItem as FrameworkElement);
            //    if (item != null)
            //        item.Focus();
            //});
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
                    var ordervalues = Document.GetKeyCommandCollection();
                    if (ordervalues.Count > 0)
                    {
                        foreach (var value in ordervalues)
                            AddTreeItem(value, itemRoot);
                    }
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }
        }
        void CommandEntity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Document.NeedsSave = true;
            if (e.PropertyName == nameof(ShortcutTreeItemControl.ShortcutKey))
            {
                var node = (from TreeListNode n in itemRoot.Nodes where n.Tag != null && n.Tag == sender select n).FirstOrDefault();
                if (node != null)
                {
                    (node.Content as ShortcutTreeItemControl).ShortcutKey = (sender as UFKeyCommandEntity).ShortcutKey;
                    treeListControl.RefreshRow(node.RowHandle);
                }
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

            var ic = new ShortcutTreeItemControl(tag);
            var newitem = treeListControl.AddNode(ic, parent, tag);

            if (tag is UFKeyCommandEntity)
            {
                var value = tag as UFKeyCommandEntity;
                value.PropertyChanged += CommandEntity_PropertyChanged;// (s, e) => { Document.NeedsSave = true; };
                if (parent.Tag is UFShortcutEntity)
                {
                    value.UFShortcutAss = parent.Tag as UFShortcutEntity;
                    if (!value.UFShortcutAss.KeyCommands.Contains(value))
                    {
                        value.UFShortcutAss.KeyCommands.Add(value);
                    }
                }

                SetBindingOnProp(newitem, value, nameof(value.Name));
                //SetBindingOnProp(newitem, value, nameof(value.ShortcutKey), ShortcutTreeItemControl.ShortcutKeyProperty);
                (newitem.Content as TreeItemControl).ResourceIcon = ShortcutEditorManagerComponent.GetBitmapImage("SCTMKeyCommand");
            }
            else
                return null;
            
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
            BindingOperations.SetBinding(node.Content as ShortcutTreeItemControl, targetDP, myBinding);
        }

        private static bool CanBeExpanded(TreeListNode parent)
        {
            return parent.Nodes.Count == 1 && parent.Nodes[0].Tag == TreeListControlHelper.DummyNode;
        }

        void OnTreeNodeCollapsing(object sender, TreeListNodeAllowEventArgs e)
        {
            //if (!(e.Node.Content as TreeItemControl).IsNodeExpanding)
            //    UpdateFolderIcon(e.Node, false);
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
                if (item.Tag is UFKeyCommandEntity)
                    FillItems(item);

                treeListControl.EndDataUpdate();
                e.Handled = true;
            }
            finally
            {
                (item.Content as TreeItemControl).IsNodeExpanding = false;
            }
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

        private TreeListNode GetTreeItem(Guid guid, TreeListNode parent = null)
        {
            if (parent == null)
                parent = itemRoot;

            if (!parent.IsExpanded)
                parent.IsExpanded = true;

            var list = (from p in parent.Nodes
                        where (p.Tag is UFKeyCommandEntity && (p.Tag as UFKeyCommandEntity).NodeId == guid)
                        select p).ToList();
            if (list.Count > 0)
                return list[0];

            return null;
        }

        internal bool IsAnyKeyCommandSelected()
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            return item != null && item.Tag is UFKeyCommandEntity;
        }

        internal bool IsAnyItemSelected()
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            return item != null && item != itemRoot;
        }

        internal void DeleteSelectedItems()
        {
            var items = (from item in treeListControl.GetSelectedNodes()
                         where item.Tag != null select item).ToList();

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
        }

        internal void DeleteTreeItem(TreeListNode item)
        {
            var parent = item.ParentNode ?? itemRoot;
            if (item.Tag is UFKeyCommandEntity)
            {
                var value = item.Tag as UFKeyCommandEntity;

                parent.Nodes.Remove(item);
                if (value.UFShortcutAss != null)
                {
                    value.UFShortcutAss.KeyCommands.Remove(value);
                    Document.NeedsSave = true;
                }
            }
        }

        internal void AddKeyCommand(UFKeyCommandEntity value, TreeListNode parent)
        {
            treeListControl.ClearSelection();
            var node = AddTreeItem(value, parent);
            if (node != null)
                treeListControl.SelectNode(node);
            Document.NeedsSave = true;
        }

        internal TreeListNode GetSelectedParent(bool bSkipSelected = false)
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (!bSkipSelected && selected != null && !(selected.Tag is UFKeyCommandEntity))
                return selected;
            while (selected != null)
            {
                selected = selected.ParentNode;
                if (selected != null && !(selected.Tag is UFKeyCommandEntity))
                    return selected;
            }

            return itemRoot;
        }

        private void EditShortcut()
        {
            var clone = Document.ShortcutEntity.Clone();
            var newRecipeControl = new NewShortcutDefinition()
            {
                DataContext = clone
            };

            GeneralDialogContent Dialog = new GeneralDialogContent(newRecipeControl)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "ShortcutEditor"
            };
            if (Dialog.ShowDialog() == true)
            {
                Document.ShortcutEntity.copyPropertiesFrom(clone);
            }
        }

        private void EditKeyCommand(UFKeyCommandEntity value)
        {
            if (value == null)
                return;

            var clone = value.Clone() as UFKeyCommandEntity;
            clone.PropertyChanged -= CommandEntity_PropertyChanged;

            var newDataValueControl = new NewKeyCommandDefinition(EditorComponent, Document)
            {
                DataContext = clone
            };

            GeneralDialogContent Dialog = new GeneralDialogContent(newDataValueControl)
            {
                Owner = this.FindParent<Window>(), DialogKeepContent=true,
                HelpLink = "KeyCommandEditor"
            };
            if (Dialog.ShowDialog() == true)
            {
                EditorComponent.CommandExplorer.PropagateChanges(newDataValueControl.CommandCtrl.Content as UserControl);

                value.copyPropertiesFrom(clone);
            }
        }

        private void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            e.Handled = true;
            if (EditorComponent.PropertyControl != null)
                EditorComponent.PropertyControl.Activate();
            else
                EditSelectedItem();
        }

        void EditSelectedItem()
        {
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (selected != null)
            {
                if (selected.Tag is UFShortcutEntity)
                {
                    EditShortcut();
                }
                if (selected.Tag is UFKeyCommandEntity)
                {
                    var value = selected.Tag as UFKeyCommandEntity;
                    EditKeyCommand(value);
                }
            }
        }

        public void ActivateCommandExplorer()
        {
            if (EditorComponent.CommandExplorer == null)
                return;

            EditorComponent.CommandExplorer.Activate();
        }

        #endregion

        #region Properties

        UFShortcutDocument _Document;
        [Browsable(false)]
        public UFShortcutDocument Document
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

        private void OnCommandCut(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                using (new WaitCursor())
                {
                    OnCommandCopy(sender, e);
                    OnRemoveItem(sender, e);
                }
            }
        }

        private void CanCommandCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected();
        }

        private void OnCommandCopy(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                if (IsAnyItemSelected())
                {
                    var selItems = treeListControl.GetSelectedNodes();
                    if (selItems.Count() == 1)
                    {
                        var dataObject = new DataObject();

                        var listKeyCommands = new UFKeyCommandList();

                        var item = selItems[0];
                        if ((item as TreeListNode).Tag is UFKeyCommandEntity)
                            listKeyCommands.Add((item as TreeListNode).Tag as UFKeyCommandEntity);

                        if (listKeyCommands.Count > 0)
                        {
                            var listDataValuesString = listKeyCommands.ToXml();
                            dataObject.SetData(listKeyCommands.GetType(), listDataValuesString);
                            dataObject.SetData(DataFormats.Xaml, listDataValuesString);
                            dataObject.SetData(DataFormats.Text, listDataValuesString);
                        }

                        Clipboard.SetDataObject(dataObject, true);
                    }
                    else
                    {
                        var dataObject = new DataObject();

                        var listKeyCommands = new UFKeyCommandList();

                        foreach (var item in selItems)
                        {
                            if ((item as TreeListNode).Tag is UFKeyCommandEntity)
                                listKeyCommands.Add((item as TreeListNode).Tag as UFKeyCommandEntity);
                        }

                        if (listKeyCommands.Count > 0)
                            dataObject.SetData(listKeyCommands.GetType(), listKeyCommands.ToXml());

                        Clipboard.SetDataObject(dataObject, true);
                    }
                }
            }
        }

        private void CanCommandCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected();
        }

        private void OnCommandPaste(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                var undoDataObject = new UndoRedoDataObject(UndoRedoAction.Added);
                var dataObject = Clipboard.GetDataObject() as DataObject;

                var listKeyCommands = new UFKeyCommandList();
                var listKeyCommandsString = dataObject.GetData(listKeyCommands.GetType()) as String;
                if (listKeyCommandsString != null)
                {
                    listKeyCommands = listKeyCommandsString.FromXml<UFKeyCommandList>();
                }

                try
                {
                    treeListControl.BeginDataUpdate();
                    listKeyCommands.ForEach(datavalue =>
                    {
                        var newKeyCommand = Document.AddNewKeyCommand(datavalue.Name);
                        newKeyCommand.CopyAll(datavalue, false);
                        undoDataObject.AddDataObject(newKeyCommand);
                        AddKeyCommand(newKeyCommand, itemRoot);
                    });
                }
                finally
                {
                    treeListControl.EndDataUpdate();
                }
                               
                // undo/redo handling
                if (listKeyCommands.Count > 0)
                {
                    undoRedoManager.AddUndoAction(undoDataObject);
                }
            }
        }

        private void CanCommandPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            var dataObject = Clipboard.GetDataObject() as DataObject;
            if (dataObject != null)
            {
                var listKeyCommands = new UFKeyCommandList();
                var listKeyCommandsString = dataObject.GetData(listKeyCommands.GetType()) as String;
                e.CanExecute = listKeyCommandsString != null;
            }
        }

        private void OnCommandProperties(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (EditorComponent.PropertyControl != null)
                EditorComponent.PropertyControl.Activate();
            else
                EditSelectedItem();
        }

        private void CanCommandProperties(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected();
        }

        void CheckUndoRedo(UndoRedoDataObject dataObject)
        {
            if (dataObject.UndoRedoAction == UndoRedoAction.Added)
            {
                try
                {
                    treeListControl.ClearSelection();
                    treeListControl.BeginDataUpdate();
                    if (dataObject.UFKeyCommandList != null)
                    {
                        dataObject.UFKeyCommandList.ForEach(keycommand =>
                        {
                            TreeListNode parent = null;
                            var item = GetTreeItem(keycommand.NodeId, parent);
                            if (item != null)
                                DeleteTreeItem(item);
                        });
                    }
                }
                finally
                {
                    treeListControl.EndDataUpdate();
                }
            }
            else if (dataObject.UndoRedoAction == UndoRedoAction.Removed)
            {
                try
                {
                    treeListControl.ClearSelection();
                    treeListControl.BeginDataUpdate();
                    if (dataObject.UFKeyCommandList != null)
                    {
                        dataObject.UFKeyCommandList.ForEach(datavalue =>
                        {
                            TreeListNode parent = null;
                            var newKeyCommand = Document.AddNewKeyCommand(datavalue.Name);
                            newKeyCommand.CopyAll(datavalue);
                            newKeyCommand.NodeId = datavalue.NodeId; // preserve older nodeid
                        AddKeyCommand(newKeyCommand, parent ?? itemRoot);
                        });
                    }
                }
                finally
                {
                    treeListControl.EndDataUpdate();
                }
            }
            else if (dataObject.UndoRedoAction == UndoRedoAction.Changed)
            {
                try
                {
                    treeListControl.ClearSelection();
                    treeListControl.BeginDataUpdate();
                    if (dataObject.UFKeyCommandList != null)
                    {
                        dataObject.UFKeyCommandList.ForEach(datavalue =>
                        {
                            TreeListNode parent = null;
                            var item = GetTreeItem(datavalue.NodeId, parent);
                            if (item != null)
                            {
                                treeListControl.SelectNode(item);
                                (item.Tag as UFKeyCommandEntity).CopyAll(datavalue);
                            }
                        });
                    }
                }
                finally
                {
                    treeListControl.EndDataUpdate();
                }
            }
        }

        private void OnCommandUndo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                var dataObject = undoRedoManager.Undo();
                if (dataObject == null || !dataObject.IsAnyActionAvailable)
                    return;

                CheckUndoRedo(dataObject);

                if (dataObject.UndoRedoAction == UndoRedoAction.Added)
                    undoRedoManager.AddRedoAction(new UndoRedoDataObject(dataObject, UndoRedoAction.Removed));
                else if (dataObject.UndoRedoAction == UndoRedoAction.Removed)
                    undoRedoManager.AddRedoAction(new UndoRedoDataObject(dataObject, UndoRedoAction.Added));
                else if (dataObject.UndoRedoAction == UndoRedoAction.Changed)
                    undoRedoManager.AddRedoAction(new UndoRedoDataObject(dataObject, UndoRedoAction.Changed));
            }
        }

        private void CanCommandUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = undoRedoManager.CanUndo();
        }

        private void OnCommandRedo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                var dataObject = undoRedoManager.Redo();
                if (dataObject == null || !dataObject.IsAnyActionAvailable)
                    return;

                CheckUndoRedo(dataObject);

                if (dataObject.UndoRedoAction == UndoRedoAction.Added)
                    undoRedoManager.AddUndoAction(new UndoRedoDataObject(dataObject, UndoRedoAction.Removed));
                else if (dataObject.UndoRedoAction == UndoRedoAction.Removed)
                    undoRedoManager.AddUndoAction(new UndoRedoDataObject(dataObject, UndoRedoAction.Added));
                else if (dataObject.UndoRedoAction == UndoRedoAction.Changed)
                    undoRedoManager.AddUndoAction(new UndoRedoDataObject(dataObject, UndoRedoAction.Changed));
            }

        }

        private void CanCommandRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = undoRedoManager.CanRedo();
        }

        private void OnRemoveItem(object sender, ExecutedRoutedEventArgs e)
        {
            using (new WaitCursor())
            {
                // undo/redo handling
                var dataObject = new UndoRedoDataObject(UndoRedoAction.Removed);
                foreach (var item in treeListControl.GetSelectedNodes())
                {
                    if (item is TreeListNode && (item as TreeListNode).Tag is UFKeyCommandEntity)
                        dataObject.AddDataObject((item as TreeListNode).Tag as UFKeyCommandEntity);
                }

                if (dataObject.IsAnyActionAvailable)
                    undoRedoManager.AddUndoAction(dataObject);

                DeleteSelectedItems();
            }
        }

        private void CanRemoveItem(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyItemSelected();
        }

        private void OnAddNewKeyCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var value = Document.AddNewKeyCommand();

            if (EditorComponent.PropertyControl != null)
            {
                AddKeyCommand(value, itemRoot);

                var dataObject = new UndoRedoDataObject(UndoRedoAction.Added);
                dataObject.AddDataObject(value);
                undoRedoManager.AddUndoAction(dataObject);
                EditorComponent.PropertyControl.Activate();
            }
            else
            {
                var newValue = new NewKeyCommandDefinition(EditorComponent, Document)
                {
                    DataContext = value
                };

                GeneralDialogContent Dialog = new GeneralDialogContent(newValue)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "KeyCommandEditor"
                };
                if (Dialog.ShowDialog() == true)
                {
                    EditorComponent.CommandExplorer.PropagateChanges(newValue.CommandCtrl.Content as UserControl);

                    AddKeyCommand(value, itemRoot);

                    var dataObject = new UndoRedoDataObject(UndoRedoAction.Added);
                    dataObject.AddDataObject(value);
                    undoRedoManager.AddUndoAction(dataObject);
                }
                else if (value is IDisposable)
                {
                    (value as IDisposable).Dispose();
                }
            }
#if DEBUG
            System.Diagnostics.Trace.TraceInformation("OnAddNewKeyCommand END");
#endif
            e.Handled = true;
        }

        private void CanAddNewKeyCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

        #region IDisposable

        protected bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            EditorComponent.Workspace.ContextObject = null;
        }

        #endregion
    }
}

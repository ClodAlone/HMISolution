using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UFUAEditor.Document;
using WPFUtilities;

namespace UFUAEditor.Helpers
{
    public abstract partial class TreeViewEditorHelper : UserControl, IDisposable
    {
        #region Declarations
        protected TreeListControl treeListControl;
        protected GridControl gridDataControl;
        protected UFUAServerDocument Document;

        protected bool UnSubscribeSelectionChangedEvent;
        protected readonly Dictionary<Object, TreeListNode> mapObjectToNode = new Dictionary<Object, TreeListNode>();

        bool bDisposed;
        #endregion

        #region Methods
        protected void SetTabsContent(TreeListControl tree, GridControl grid = null)
        {
            treeListControl = tree;
            gridDataControl = grid;
        }

        protected virtual void OnValidateCell(object sender, GridCellValidationEventArgs e)
        {
            if (!(e.Row is DevExpress.Xpo.IXPSimpleObject))
                return;

            var oldCellValue = e.CellValue;
            var newCellValue = e.Value;

            if (oldCellValue != newCellValue)
            {
                using (var uow = Document.BeginNestedUnitOfWork())
                {
                    var nestedObj = uow.GetNestedObject(e.Row) as DevExpress.Xpo.IXPSimpleObject;
                    var propertyInfo = nestedObj.GetType().GetProperty(e.Column.FieldName);
                    if (newCellValue != null)
                        propertyInfo.SetValue(nestedObj, Convert.ChangeType(newCellValue, newCellValue.GetType()), null);
                    else
                        propertyInfo.SetValue(nestedObj, null, null);
                    if (nestedObj is System.ComponentModel.IDataErrorInfo)
                    {
                        var error = (nestedObj as System.ComponentModel.IDataErrorInfo)[e.Column.FieldName];
                        if (!String.IsNullOrEmpty(error))
                        {
                            e.SetError(error);
                            e.IsValid = false;
                        }
                    }

                    if (e.IsValid)
                    {
                        if (IsUndoRedoSupported(nestedObj))
                            Document.AddUndoAction(this, uow.GetParentObject(nestedObj), Utilities.Xpo.UndoRedo.UndoRedoAction.Changed);
                        uow.CommitChanges();
                        UpdateContextObjects(gridDataControl);
                    }
                }
            }
        }

        internal virtual void OnDeactivate()
        {
            treeListControl.View.CancelRowEdit();
            if (gridDataControl != null)
                gridDataControl.View.CancelRowEdit();
        }

        internal virtual void OnActivate()
        {
            UpdateContextObjects();
        }

        protected void UpdateContextObjects()
        {
            DevExpress.Xpf.Core.DXTabItem tabItem = null;
            if (gridDataControl != null)
                tabItem = LogicalTreeHelper.GetParent(gridDataControl) as DevExpress.Xpf.Core.DXTabItem;
            if (tabItem != null && tabItem.IsSelected)
                UpdateContextObjects(gridDataControl);
            else
                UpdateContextObjects(treeListControl);
        }

        protected void UpdateContextObjects(object sender)
        {
            try
            {
                UnSubscribeSelectionChangedEvent = true;

                var selecteditems = new List<Object>();
                if (sender is GridControl)
                {
                    treeListControl.ClearSelection();
                    var nodesToSelect = new List<TreeListNode>();
                    foreach (object o in (sender as GridControl).SelectedItems)
                    {
                        selecteditems.Add(o);
                        if (mapObjectToNode.ContainsKey(o))
                        {
                            var parent = mapObjectToNode[o].ParentNode;
                            var item = treeListControl.GetTreeItem(o, parent);
                            if (item != null)
                                nodesToSelect.Add(item);
                        }
                    };
                    
                    treeListControl.SelectNodes(nodesToSelect, true);
                }
                else
                {
                    selecteditems = treeListControl.GetTreeSelectedItems();

                    if (gridDataControl != null)
                    {
                        gridDataControl.SelectedItems.Clear();
                        selecteditems.ForEach(o =>
                        {
                            try
                            {
                                gridDataControl.SelectedItems.Add(o);
                            }
                            catch
                            { }
                        });
                    }
                }

                if (selecteditems.Count == 0)
                {
                    Document.EditorManagerComponent.Workspace.ContextObject = null;
                }
                else if (selecteditems.Count == 1)
                {
                    Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(selecteditems[0]);
                }
                else
                {
                    Document.EditorManagerComponent.Workspace.ContextObjects = Document.GetNestedObjects(selecteditems);
                }
            }
            finally
            {
                UnSubscribeSelectionChangedEvent = false;
            }
        }

        internal bool IsAnyItemSelected(Type neededType = null)
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            var griditems = gridDataControl.View.FocusedView.SelectedRows;

            if (neededType != null) 
            {
                if (griditems.Count > 0 && griditems[0].GetType() != neededType)
                    griditems = null;
                if (item?.Tag?.GetType() != neededType)
                    item = null;
            }

            return (item != null && item.RowHandle != 0) || (griditems != null && griditems.Count > 0);
        }

        internal bool IsAnyItemSelectedExcept(Type forbiddenType)
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault();
            var griditems = gridDataControl.View.FocusedView.SelectedRows;

            if (griditems.Count > 0 && griditems[0].GetType() == forbiddenType)
                griditems = null;
            if (item?.Tag?.GetType() == forbiddenType)
                item = null;

            return (item != null && item.RowHandle != 0) || (griditems != null && griditems.Count > 0);
        }
        #endregion

        #region Abstract Methods
        abstract protected bool IsUndoRedoSupported(object obj);
        #endregion

        #region Static Methods
        protected static void SetBindingOnProp(TreeListNode node, object bindingSource, string propName, DependencyProperty targetDP = null)
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

        protected static void SetBindingOnProp(TreeListNode node, object[] bindingSources, string[] propNames, IMultiValueConverter converter, DependencyProperty targetDP = null)
        {
            if (bindingSources != null && bindingSources.Length > 0 &&
                propNames != null && propNames.Length > 0 &&
                bindingSources.Length == propNames.Length)
            {
                var multiBinding = new MultiBinding();
                multiBinding.Converter = converter;
                for (int ii = 0; ii < bindingSources.Length; ii++)
                {
                    var bindingSource = bindingSources[ii];
                    var propName = propNames[ii];
                    if (bindingSource == null || String.IsNullOrEmpty(propName))
                        continue;

                    var myBinding = new Binding(propName);
                    myBinding.Source = bindingSource;
                    myBinding.Mode = BindingMode.OneWay;
                    myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    multiBinding.Bindings.Add(myBinding);
                }

                BindingOperations.SetBinding(node.Content as HeaderedItemsControl, targetDP ?? TreeItemControl.ItemHeaderProperty, multiBinding);
            }
        }

        protected static bool CanAssignItem(UFUAModel.UFUATag tag)
        {
            return (!tag.IsSubPrototypeMember || !tag.UseShared.Value) &&
                   (tag.ModelType == UFUAModel.ModelType.Analog ||
                   tag.ModelType == UFUAModel.ModelType.Variable ||
                   tag.ModelType == UFUAModel.ModelType.Digital ||
                   tag.ModelType == UFUAModel.ModelType.Enumerated);
        }

        protected static bool CanAssignView(UFUAModel.UFUATag tag)
        {
            return !tag.IsPrototypeMember || (tag.IsSubPrototypeMember && !tag.UseShared.Value);
        }

        protected static bool CanAssignHistorian(UFUAModel.UFUATag tag)
        {
            return CanAssignItem(tag);
        }

        protected static bool CanAssignAlarm(UFUAModel.UFUATag tag)
        {
            return CanAssignItem(tag);
        }

        protected static bool CanAssignDataLoggerColumn(UFUAModel.UFUATag tag, UFUAModel.UFUATag instance = null)
        {
            return !tag.IsObjectType && (!tag.IsPrototypeMember || tag.IsSubPrototypeMember || instance != null);
        }

        protected static bool CanAssignEngineeringUnit(UFUAModel.UFUATag tag)
        {
            return tag.IsEngineeringUnitSupported && (!tag.IsSubPrototypeMember || !tag.UseShared.Value);
        }
        #endregion

        #region IDisposable
        public virtual void OnDispose()
        { }

        public void Dispose()
        {
            if (!bDisposed)
            {
                bDisposed = true;

                OnDispose();
            }
        }
        #endregion
    }
}

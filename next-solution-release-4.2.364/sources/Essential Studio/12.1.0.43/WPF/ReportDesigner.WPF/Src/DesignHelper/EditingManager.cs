#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Syncfusion.Windows.Reports.Designer.Controls
{
    enum EditActionType
    {
        ItemAdd,
        ItemDelete,
        ItemMove,
        ItemResize,
        ItemPropertyChanged,
        ItemChanged,
        FooterVisiblity,
        HeaderVisiblity,
        DatasetAdd,
        DatasetRemove,
        DatasetChanged,
        DatasourceAdd,
        DatasourceRemove,
        DatasourceChanged,
        EmbeddedImageAdd,
        EmbeddedImageRemove,
        ReportParameterAdd,
        ReportParameterRemove,
        ReportParameterChanged,
        TablixItemAdd,
        TablixItemRemove,
        TablixHorizontalItemPoistionChange,
        TablixVerticalItemPoistionChange,
        TablixItemsChanged,
        TablixContentChanged
    }


    class EditActions : List<EditAction>
    {

    }

    class EditingManager
    {
        private DesignPanel designPanel;

        Stack<EditActions> EditActions
        {
            get;
            set;
        }

        Stack<EditActions> UndoActions
        {
            get;
            set;
        }

        Stack<EditActions> RedoActions
        {
            get;
            set;
        }

        public bool IsMergeAction
        {
            get;
            set;
        }

        public EditingManager(DesignPanel panel)
        {
            this.designPanel = panel;
            this.EditActions = new Stack<EditActions>();
            this.UndoActions = new Stack<EditActions>();
            this.RedoActions = new Stack<EditActions>();
        }

        public void AddAction(EditAction action)
        {
            this.UndoActions.Clear();
            action.DesignPanel = this.designPanel;

            if (IsMergeAction)
            {
                this.MergeAction(action);
            }
            else
            {
                EditActions actions = new EditActions();
                actions.Add(action);
                this.EditActions.Push(actions);
            }
        }

        public void RemoveAction()
        {
            this.EditActions.Pop();
        }

        public void MergeAction(EditAction action)
        {
            if (this.EditActions.Count > 0)
            {
                this.EditActions.Peek().Insert(0, action);
            }
        }

        public void Undo()
        {
            this.designPanel.IsInternalChange = true;

            if (this.EditActions.Count > 0)
            {
                bool isContainsParameterAction = false;
                bool isContainsDataSetAction = false;
                bool isContainsDataSourceAction = false;
                bool isContainsImageAction = false;

                EditActions actions = this.EditActions.Pop();
                this.UndoActions.Push(actions);
                
                foreach (var action in actions)
                {
                    action.PerformUndo();

                    if (action.EditingType == EditActionType.ReportParameterRemove || action.EditingType == EditActionType.ReportParameterAdd || action.EditingType == EditActionType.ReportParameterChanged)
                    {
                        isContainsParameterAction = true;
                    }
                    else if (action.EditingType == EditActionType.EmbeddedImageAdd || action.EditingType == EditActionType.EmbeddedImageRemove)
                    {
                        isContainsImageAction = true;
                    }
                    else if (action.EditingType == EditActionType.DatasetAdd || action.EditingType == EditActionType.DatasetRemove || action.EditingType == EditActionType.DatasetChanged)
                    {
                        isContainsDataSetAction = true;
                    }
                    else if (action.EditingType == EditActionType.DatasourceRemove || action.EditingType == EditActionType.DatasourceAdd || action.EditingType == EditActionType.DatasourceChanged)
                    {
                        isContainsDataSourceAction = true;
                    }
                }

                if (isContainsParameterAction)
                {
                    this.designPanel.RaiseParameterCollectionModifiedEvent();
                }
                if (isContainsDataSourceAction)
                {
                    this.designPanel.RaiseDataSourceCollectionModifiedEvent();
                }
                if (isContainsDataSetAction)
                {
                    this.designPanel.RaiseDataSetCollectionModifiedEvent();
                }
                if (isContainsImageAction)
                {
                    this.designPanel.RaiseEmbeddedImageCollectionModifiedEvent();
                }
            }

            this.designPanel.IsInternalChange = false;
        }

        public void Redo()
        {
            this.designPanel.IsInternalChange = true;

            if (this.UndoActions.Count > 0)
            {
                bool isContainsParameterAction = false;
                bool isContainsDataSetAction = false;
                bool isContainsDataSourceAction = false;
                bool isContainsImageAction = false;

                EditActions actions = this.UndoActions.Pop();
                this.EditActions.Push(actions);

                foreach (var action in actions)
                {
                    action.PerfomRedo();

                    if (action.EditingType == EditActionType.ReportParameterRemove || action.EditingType == EditActionType.ReportParameterAdd)
                    {
                        isContainsParameterAction = true;
                    }
                    else if (action.EditingType == EditActionType.EmbeddedImageAdd || action.EditingType == EditActionType.EmbeddedImageRemove)
                    {
                        isContainsImageAction = true;
                    }
                    else if (action.EditingType == EditActionType.DatasetAdd || action.EditingType == EditActionType.DatasetRemove || action.EditingType == EditActionType.DatasetChanged)
                    {
                        isContainsDataSetAction = true;
                    }
                    else if (action.EditingType == EditActionType.DatasourceRemove || action.EditingType == EditActionType.DatasourceAdd)
                    {
                        isContainsDataSourceAction = true;
                    }
                }

                if (isContainsParameterAction)
                {
                    this.designPanel.RaiseParameterCollectionModifiedEvent();
                }
                if (isContainsDataSourceAction)
                {
                    this.designPanel.RaiseDataSourceCollectionModifiedEvent();
                }
                if (isContainsDataSetAction)
                {
                    this.designPanel.RaiseDataSetCollectionModifiedEvent();
                }
                if (isContainsImageAction)
                {
                    this.designPanel.RaiseEmbeddedImageCollectionModifiedEvent();
                }
            }

            this.designPanel.IsInternalChange = false;
        }
    }

    class EditAction
    {
        public EditActionType EditingType
        {
            get;
            set;
        }

        public DesignPanel DesignPanel
        {
            get;
            set;
        }

        public List<IReportItemControl> EditReportItems
        {
            get;
            set;
        }

        public List<MovingChange> MovedReportItems
        {
            get;
            set;
        }

        public List<SizingChange> ResizedReportItems
        {
            get;
            set;
        }

        public List<IReportItemControl> EditReportItemsChild
        {
            get;
            set;
        }

        public PropertyChanage PropertyChange
        {
            get;
            set;
        }

        public TablixItemSizeChanage TablixItemSizeChanage
        {
            get;
            set;
        }

        public RDL.DOM.DataSource DataSource
        {
            get;
            set;
        }

        public RDL.DOM.DataSet DataSet
        {
            get;
            set;
        }

        public RDL.DOM.ReportParameter ReportParameter
        {
            get;
            set;
        }

        public RDL.DOM.EmbeddedImage EmbeddedImage
        {
            get;
            set;
        }

        public ItemChange ItemChange
        {
            get;
            set;
        }

        public TablixItemChange TablixItemChange
        {
            get;
            set;
        }

        public TablixCellContentChange TablixCellContentChange
        {
            get;
            set;
        }

        public DataSetChange DataSetChange
        {
            get;
            set;
        }

        public DataSourceChange DataSourceChange
        {
            get;
            set;
        }

        public ReportParameterChange ParameterChange
        {
            get;
            set;
        }

        public bool HeaderVisiblity
        {
            get;
            set;
        }

        public bool FooterVisiblity
        {
            get;
            set;
        }

        public EditAction()
        {
            this.EditReportItems = new List<IReportItemControl>();
            this.MovedReportItems = new List<MovingChange>();
            this.ResizedReportItems = new List<SizingChange>();
            this.EditReportItemsChild = new List<IReportItemControl>();
        }

        public void PerformItemUndo(EditActionType editType)
        {
            if (editType == EditActionType.ItemAdd)
            {
                foreach (var reportItem in this.EditReportItems)
                {
                    reportItem.ReportItem = reportItem.GetReportItem();
                    reportItem.Parent.Children.Remove(reportItem as UIElement);

                    if (reportItem.ItemType == DrawingReportItem.Tablix)
                    {
                        this.DesignPanel.UpdateTablixReportItems(reportItem as TablixControl);
                    }
                    else if (reportItem.ItemType == DrawingReportItem.Rectangle)
                    {
                        this.DesignPanel.UpdateRectangleReportItems(reportItem as RectangleControl);
                    }
                }
            }
            else if (editType == EditActionType.ItemDelete)
            {
                foreach (var reportItem in this.EditReportItems)
                {
                    if (reportItem is TablixControl)
                    {
                        var tablix = reportItem as TablixControl;
                        tablix.IsItemRemoved = !tablix.IsItemRemoved;
                    }
                    reportItem.Parent.Children.Add(reportItem as UIElement);
                }
            }
            else if (editType == EditActionType.ItemMove)
            {
                foreach (var movedItem in this.MovedReportItems)
                {
                    if (movedItem.NewParent != movedItem.OldParent)
                    {
                        movedItem.ReportItem.ReportItem = movedItem.ReportItem.GetReportItem();
                        movedItem.NewParent.Children.Remove(movedItem.ReportItem as UIElement);
                        movedItem.OldParent.Children.Add(movedItem.ReportItem as UIElement);
                        movedItem.ReportItem.Parent = movedItem.OldParent;
                    }

                    movedItem.ReportItem.ItemLeft = movedItem.OldLeft;
                    movedItem.ReportItem.ItemTop = movedItem.OldTop;
                    if (movedItem.ReportItem.ItemType == DrawingReportItem.Line)
                    {
                        movedItem.ReportItem.ItemHeight = movedItem.OldHeight;
                        movedItem.ReportItem.ItemWidth = movedItem.OldWidth;
                        if (AdornerLayer.GetAdornerLayer(movedItem.ReportItem as UIElement).GetAdorners(movedItem.ReportItem as UIElement) != null)
                        {
                            Adorner layer = AdornerLayer.GetAdornerLayer(movedItem.ReportItem as UIElement).GetAdorners(movedItem.ReportItem as UIElement).First();
                            layer.InvalidateArrange();
                        }
                    }
                }
            }
            else if (editType == EditActionType.ItemResize)
            {
                foreach (var resizedItem in this.ResizedReportItems)
                {
                    resizedItem.ReportItem.ItemWidth = resizedItem.OldWidth;
                    resizedItem.ReportItem.ItemHeight = resizedItem.OldHeight;
                    resizedItem.ReportItem.ItemLeft = resizedItem.OldLeft;
                    resizedItem.ReportItem.ItemTop = resizedItem.OldTop;
                    if (resizedItem.ReportItem.ItemType == DrawingReportItem.Line)
                    {
                        if (AdornerLayer.GetAdornerLayer(resizedItem.ReportItem as UIElement).GetAdorners(resizedItem.ReportItem as UIElement) != null)
                        {
                            Adorner layer = AdornerLayer.GetAdornerLayer(resizedItem.ReportItem as UIElement).GetAdorners(resizedItem.ReportItem as UIElement).First();
                            layer.InvalidateArrange();
                        }
                    }
                }
            }
            else if (editType == EditActionType.ItemPropertyChanged)
            {
                Type type = this.PropertyChange.PropertyObject.GetType();
                var propertyInfo = type.GetProperty(this.PropertyChange.PropertyName);
                Editors.IDesignerProperties property = this.PropertyChange.PropertyObject as Editors.IDesignerProperties;

                if (property != null)
                {
                    property.IsInternalPropertyChange = true;
                    propertyInfo.SetValue(this.PropertyChange.PropertyObject, this.PropertyChange.OldValue, null);
                    property.IsInternalPropertyChange = false;
                }
                else
                {
                    propertyInfo.SetValue(this.PropertyChange.PropertyObject, this.PropertyChange.OldValue, null);
                }
            }
            else if (editType == EditActionType.ItemChanged)
            {
                this.ItemChange.ReportItem.RestoreReportItem(this.ItemChange.OldValue);
            }
            else if (editType == EditActionType.TablixItemsChanged)
            {
                (this.TablixItemChange.ReportItem as TablixControl).CatchedReportItems = this.TablixItemChange.OldReportItems;
                this.TablixItemChange.ReportItem.RestoreReportItem(this.TablixItemChange.OldValue);
            }
            else if (editType == EditActionType.TablixContentChanged)
            {
                this.TablixCellContentChange.OldContent.ReportItem = this.TablixCellContentChange.OldContent.GetReportItem();
                this.TablixCellContentChange.Parent.Content = this.TablixCellContentChange.OldContent as UIElement;
            }
            else if (editType == EditActionType.TablixHorizontalItemPoistionChange)
            {
                    (this.TablixItemSizeChanage.ReportItem as TablixControl).TablixGrid.Width = this.TablixItemSizeChanage.OldWidth;
                    (this.TablixItemSizeChanage.ReportItem as TablixControl).TablixGrid.ColumnDefinitions[this.TablixItemSizeChanage.index].Width = this.TablixItemSizeChanage.OldGridWidth;
            }

            else if(editType==EditActionType.TablixVerticalItemPoistionChange)
            {
                 (this.TablixItemSizeChanage.ReportItem as TablixControl).TablixGrid.Height = this.TablixItemSizeChanage.OldHeight;
                 (this.TablixItemSizeChanage.ReportItem as TablixControl).TablixGrid.RowDefinitions[this.TablixItemSizeChanage.index].Height = this.TablixItemSizeChanage.OldGridWidth;
            }
            else if (editType == EditActionType.ReportParameterAdd)
            {
                this.DesignPanel.RemoveReportParameter(this.ReportParameter);
            }
            else if (editType == EditActionType.ReportParameterRemove)
            {
                this.DesignPanel.AddReportParameter(this.ReportParameter);
            }
            else if (editType == EditActionType.ReportParameterChanged)
            {
                this.ParameterChange.ReportParameter.Update(this.ParameterChange.OldValue);
            }
            else if (editType == EditActionType.DatasourceAdd)
            {
                this.DesignPanel.RemoveDataSource(this.DataSource);
            }
            else if (editType == EditActionType.DatasourceRemove)
            {
                this.DesignPanel.AddDataSource(this.DataSource);
            }
            else if (editType == EditActionType.DatasourceChanged)
            {
                this.DataSourceChange.DataSource.Update(this.DataSourceChange.OldValue);
            }
            else if (editType == EditActionType.DatasetAdd)
            {
                this.DesignPanel.RemoveDataSet(this.DataSet);
            }
            else if (editType == EditActionType.DatasetRemove)
            {
                this.DesignPanel.AddDataSet(this.DataSet);
            }
            else if (editType == EditActionType.DatasetChanged)
            {
                this.DataSetChange.DataSet.Update(this.DataSetChange.OldValue);
            }
            else if (editType == EditActionType.EmbeddedImageAdd)
            {
                this.DesignPanel.RemoveEmbeddedImage(this.EmbeddedImage);
            }
            else if (editType == EditActionType.EmbeddedImageRemove)
            {
                this.DesignPanel.AddEmbeddedImage(this.EmbeddedImage);
            }
            else if (editType == EditActionType.HeaderVisiblity)
            {
                this.DesignPanel.IsHeaderVisible = !HeaderVisiblity;
            }
            else if (editType == EditActionType.FooterVisiblity)
            {
                this.DesignPanel.IsFooterVisible = !FooterVisiblity;
            }
        }

        public void PerformUndo()
        {
            this.PerformItemUndo(this.EditingType);
        }

        public void PerformItemRedo(EditActionType editType)
        {
            if (this.EditingType == EditActionType.ItemAdd)
            {
                foreach (var reportItem in this.EditReportItems)
                {
                    reportItem.Parent.Children.Add(reportItem as UIElement);
                }
            }
            else if (this.EditingType == EditActionType.ItemDelete)
            {
                foreach (var reportItem in this.EditReportItems)
                {
                    reportItem.ReportItem = reportItem.GetReportItem();

                    if (reportItem.ItemType == DrawingReportItem.Tablix)
                    {
                        var tablix = reportItem as TablixControl;
                        tablix.IsItemRemoved = !tablix.IsItemRemoved;
                        this.DesignPanel.UpdateTablixReportItems(reportItem as TablixControl);
                    }
                    else if (reportItem.ItemType == DrawingReportItem.Rectangle)
                    {
                        this.DesignPanel.UpdateRectangleReportItems(reportItem as RectangleControl);
                    }

                    reportItem.Parent.Children.Remove(reportItem as UIElement);
                }
            }
            else if (this.EditingType == EditActionType.ItemMove)
            {
                foreach (var movedItem in this.MovedReportItems)
                {
                    if (movedItem.NewParent != movedItem.OldParent)
                    {
                        movedItem.ReportItem.ReportItem = movedItem.ReportItem.GetReportItem();
                        movedItem.OldParent.Children.Remove(movedItem.ReportItem as UIElement);
                        movedItem.NewParent.Children.Add(movedItem.ReportItem as UIElement);
                        movedItem.ReportItem.Parent = movedItem.NewParent;
                    }

                    movedItem.ReportItem.ItemLeft = movedItem.NewLeft;
                    movedItem.ReportItem.ItemTop = movedItem.NewTop;
                    if (movedItem.ReportItem.ItemType == DrawingReportItem.Line)
                    {
                        movedItem.ReportItem.ItemHeight = movedItem.NewHeight;
                        movedItem.ReportItem.ItemWidth = movedItem.NewWidth;
                        if (AdornerLayer.GetAdornerLayer(movedItem.ReportItem as UIElement).GetAdorners(movedItem.ReportItem as UIElement) != null)
                        {
                            Adorner layer = AdornerLayer.GetAdornerLayer(movedItem.ReportItem as UIElement).GetAdorners(movedItem.ReportItem as UIElement).First();
                            layer.InvalidateArrange();
                        }
                    }
                }
            }
            else if (this.EditingType == EditActionType.ItemResize)
            {
                foreach (var resizedItem in this.ResizedReportItems)
                {
                    resizedItem.ReportItem.ItemWidth = resizedItem.NewWidth;
                    resizedItem.ReportItem.ItemHeight = resizedItem.NewHeight;
                    resizedItem.ReportItem.ItemLeft = resizedItem.OldLeft;
                    resizedItem.ReportItem.ItemTop = resizedItem.OldTop;
                    if (resizedItem.ReportItem.ItemType == DrawingReportItem.Line)
                    {
                        if (AdornerLayer.GetAdornerLayer(resizedItem.ReportItem as UIElement).GetAdorners(resizedItem.ReportItem as UIElement) != null)
                        {
                            Adorner layer = AdornerLayer.GetAdornerLayer(resizedItem.ReportItem as UIElement).GetAdorners(resizedItem.ReportItem as UIElement).First();
                            layer.InvalidateArrange();
                        }
                    }
                }
            }
            else if (editType == EditActionType.ItemPropertyChanged)
            {
                Type type = this.PropertyChange.PropertyObject.GetType();
                var propertyInfo = type.GetProperty(this.PropertyChange.PropertyName);
                Editors.IDesignerProperties property = this.PropertyChange.PropertyObject as Editors.IDesignerProperties;

                if (property != null)
                {
                    property.IsInternalPropertyChange = true;
                    propertyInfo.SetValue(this.PropertyChange.PropertyObject, this.PropertyChange.NewValue, null);
                    property.IsInternalPropertyChange = false;
                }
                else
                {
                    propertyInfo.SetValue(this.PropertyChange.PropertyObject, this.PropertyChange.NewValue, null);
                }
            }
            else if (editType == EditActionType.ItemChanged)
            {
                this.ItemChange.ReportItem.RestoreReportItem(this.ItemChange.NewValue);
            }
            else if (editType == EditActionType.TablixItemsChanged)
            {
                (this.TablixItemChange.ReportItem as TablixControl).CatchedReportItems = this.TablixItemChange.NewReportItems;
                this.TablixItemChange.ReportItem.RestoreReportItem(this.TablixItemChange.NewValue);
            }
            else if (editType == EditActionType.TablixHorizontalItemPoistionChange)
            {
                (this.TablixItemSizeChanage.ReportItem as TablixControl).ItemWidth = this.TablixItemSizeChanage.NewWidth;
                (this.TablixItemSizeChanage.ReportItem as TablixControl).TablixGrid.ColumnDefinitions[this.TablixItemSizeChanage.index].Width = this.TablixItemSizeChanage.NewGridWidth;
            }

            else if (editType == EditActionType.TablixVerticalItemPoistionChange)
            {
                (this.TablixItemSizeChanage.ReportItem as TablixControl).ItemHeight = this.TablixItemSizeChanage.NewHeight;
                (this.TablixItemSizeChanage.ReportItem as TablixControl).TablixGrid.RowDefinitions[this.TablixItemSizeChanage.index].Height = this.TablixItemSizeChanage.NewGridHeight;
            }
            else if (editType == EditActionType.TablixContentChanged)
            {
                this.TablixCellContentChange.NewContent.ReportItem = this.TablixCellContentChange.NewContent.GetReportItem();
                this.TablixCellContentChange.Parent.Content = this.TablixCellContentChange.NewContent as UIElement;
            }
            else if (editType == EditActionType.ReportParameterAdd)
            {
                this.DesignPanel.AddReportParameter(this.ReportParameter);
            }
            else if (editType == EditActionType.ReportParameterRemove)
            {
                this.DesignPanel.RemoveReportParameter(this.ReportParameter);
            }
            else if (editType == EditActionType.ReportParameterChanged)
            {
                this.ParameterChange.ReportParameter.Update(this.ParameterChange.NewValue);
            }
            else if (editType == EditActionType.DatasourceAdd)
            {
                this.DesignPanel.AddDataSource(this.DataSource);
            }
            else if (editType == EditActionType.DatasourceRemove)
            {
                this.DesignPanel.RemoveDataSource(this.DataSource);
            }
            else if (editType == EditActionType.DatasourceChanged)
            {
                this.DataSourceChange.DataSource.Update(this.DataSourceChange.NewValue);
            }
            else if (editType == EditActionType.DatasetAdd)
            {
                this.DesignPanel.AddDataSet(this.DataSet);
            }
            else if (editType == EditActionType.DatasourceRemove)
            {
                this.DesignPanel.RemoveDataSet(this.DataSet);
            }
            else if (editType == EditActionType.DatasetChanged)
            {
                this.DataSetChange.DataSet.Update(this.DataSetChange.NewValue);
            }
            else if (editType == EditActionType.EmbeddedImageAdd)
            {
                this.DesignPanel.AddEmbeddedImage(this.EmbeddedImage);
            }
            else if (editType == EditActionType.EmbeddedImageRemove)
            {
                this.DesignPanel.RemoveEmbeddedImage(this.EmbeddedImage);
            }
            else if (editType == EditActionType.HeaderVisiblity)
            {
                this.DesignPanel.IsHeaderVisible = HeaderVisiblity;
            }
            else if (editType == EditActionType.FooterVisiblity)
            {
                this.DesignPanel.IsFooterVisible = FooterVisiblity;
            }
        }

        public void PerfomRedo()
        {
            this.PerformItemRedo(this.EditingType);
        }
    }

    class SizingChange
    {
        public IReportItemControl ReportItem
        {
            get;
            set;
        }

        public double NewHeight
        {
            get;
            set;
        }

        public double NewWidth
        {
            get;
            set;
        }

        public double NewLeft
        {
            get;
            set;
        }

        public double NewTop
        {
            get;
            set;
        }

        public double OldHeight
        {
            get;
            set;
        }

        public double OldWidth
        {
            get;
            set;
        }

        public double OldLeft
        {
            get;
            set;
        }

        public double OldTop
        {
            get;
            set;
        }
    }

    class MovingChange
    {
        public IReportItemControl ReportItem
        {
            get;
            set;
        }

        public Canvas NewParent
        {
            get;
            set;
        }

        public double NewTop
        {
            get;
            set;
        }

        public double NewLeft
        {
            get;
            set;
        }

        public double NewHeight
        {
            get;
            set;
        }

        public double NewWidth
        {
            get;
            set;
        }

        public Canvas OldParent
        {
            get;
            set;
        }

        public double OldTop
        {
            get;
            set;
        }

        public double OldLeft
        {
            get;
            set;
        }

        public double OldHeight
        {
            get;
            set;
        }

        public double OldWidth
        {
            get;
            set;
        }
    }

    class ItemChange
    {
        public IReportItemControl ReportItem
        {                                     
            get;
            set;
        }

        public RDL.DOM.ReportItem OldValue
        {
            get;
            set;
        }

        public RDL.DOM.ReportItem NewValue
        {
            get;
            set;
        }
    }

    class TablixItemChange
    {
        public IReportItemControl ReportItem
        {
            get;                                 
            set;
        }

        public RDL.DOM.ReportItem OldValue
        {
            get;
            set;
        }

        public RDL.DOM.ReportItem NewValue
        {
            get;
            set;
        }

        public List<IReportItemControl> OldReportItems
        {
            get;
            set;
        }

        public List<IReportItemControl> NewReportItems
        {
            get;
            set;
        }
    }

    class TablixCellContentChange
    {
        public IReportItemControl ReportItem
        {
            get;
            set;
        }

        public CellContentsControl Parent
        {
            get;
            set;
        }

        public IReportItemControl OldContent
        {
            get;
            set;
        }

        public IReportItemControl NewContent
        {
            get;
            set;
        }
    }

    class PropertyChanage
    {
        public object PropertyObject
        {
            get;
            set;
        }

        public string PropertyName
        {
            get;
            set;
        }

        public object NewValue
        {
            get;
            set;
        }

        public object OldValue
        {
            get;
            set;
        }
    }

    class TablixItemSizeChanage
    {
        public IReportItemControl ReportItem
        {
            get;
            set;
        }

        public int index
        {
            get;
            set;
        }

        public double NewWidth
        {
            get;
            set;
        }

        public double OldWidth
        {
            get;
            set;
        }

        public GridLength NewGridWidth
        {
            get;
            set;
        }
        public GridLength OldGridWidth
        {
            get;
            set;
        }

        public double NewHeight
        {
            get;
            set;
        }

        public double OldHeight
        {
            get;
            set;
        }

        public GridLength NewGridHeight
        {
            get;
            set;
        }
        public GridLength OldGridHeight
        {
            get;
            set;
        }
    }

    class DataSourceChange
    {
        public RDL.DOM.DataSource DataSource
        {
            get;
            set;
        }

        public RDL.DOM.DataSource NewValue
        {
            get;
            set;
        }

        public RDL.DOM.DataSource OldValue
        {
            get;
            set;
        }
    }

    class DataSetChange
    {
        public RDL.DOM.DataSet DataSet
        {
            get;
            set;
        }

        public RDL.DOM.DataSet NewValue
        {
            get;
            set;
        }

        public RDL.DOM.DataSet OldValue
        {
            get;
            set;
        }
    }

    class ReportParameterChange
    {
        public RDL.DOM.ReportParameter ReportParameter
        {
            get;
            set;
        }

        public RDL.DOM.ReportParameter NewValue
        {
            get;
            set;
        }

        public RDL.DOM.ReportParameter OldValue
        {
            get;
            set;
        }
    }
}
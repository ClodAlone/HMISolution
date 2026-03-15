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
using System.Windows.Input;
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.UI.Xaml.Utility;
#if WinRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Data;
using Windows.UI.Core;
using Key = Windows.System.VirtualKey;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls.Primitives;
#else
using System.Windows;
using System.Windows.Controls;
#endif


namespace Syncfusion.UI.Xaml.Grid.Cells
{

    /// <summary>
    /// VirtualizingCellRendererBase is an abstract base class for cell renderers
    /// that need live UIElement visuals displayed in a cell. You can derive from
    /// this class and provide the type of the UIElement you want to show inside cells
    /// as type parameter. The class provides strong typed virtual methods for 
    /// initializing content of the cell and arranging the cell visuals.
    /// <para/>
    /// The class manages the creation 
    /// of cells UIElement objects when the cell is scrolled into view and also 
    /// unloading of the elements. The class offers an optimization in which 
    /// elements can be recycled when <see cref="AllowRecycle"/> is set. 
    /// In this case when a cell is scrolled out of view
    /// it is moved into a recycle bin and the next time a new element is scrolled into
    /// view the element is recovered from the recycle bin and reinitialized with the
    /// new content of the cell.<para/>
    /// when the user moves the mouse over the cell or if the UIElement is needed for
    /// other reasons.<para/>
    /// After a UIElement was created the virtual methods <see cref="WireEditUIElement"/> 
    /// and <see cref="UnwireEditUIElement"/> are called to wire any event listeners.
    /// <para/>
    /// Updates to appearance and content of child elements, creation and unloading
    /// of elements will not trigger ArrangeOverride or Render calls in parent canvas.
    /// <para/>
    /// </summary>
    /// <typeparam name="D">The type of the UIElement that should be placed inside cells</typeparam>
    /// 
#if WinRT
    using Key = Windows.System.VirtualKey;
    using Windows.UI.Core;
#endif
    [ClassReference(IsReviewed = false)]
    public abstract class GridVirtualizingCellRendererBase<D,E> : GridCellRendererBase
          where D : FrameworkElement, new()
          where E : FrameworkElement, new()
    {
        #region Fields

        private bool allowRecycle = true;
        private UIElement currentCellRootElement;
        protected VirtualizingCellUIElementBin<D> DisplayRecycleBin = new VirtualizingCellUIElementBin<D>();
        protected VirtualizingCellUIElementBin<E> EditRecycleBin = new VirtualizingCellUIElementBin<E>();

        #endregion

        #region Ctor

        public GridVirtualizingCellRendererBase()
        {
            
        }

        #endregion

        #region Property

        /// <summary>
        /// Gets or sets a value indicating whether elements can be recycled when scrolled out of view.
        /// In this case when a cell is scrolled out of view
        /// it is moved into a recycle bin and the next time a new element is scrolled into
        /// view the element is recovered from the recycle bin and reinitialized with the
        /// new content of the cell. The default value is false.
        /// </summary>
        /// <value><c>true</c> if elements can be recycled when scrolled out of view; otherwise, <c>false</c>.</value>
        public bool AllowRecycle
        {
            get { return allowRecycle; }
            set { allowRecycle = value; }
        }

        public UIElement CurrentCellRootElement
        {
            get { return currentCellRootElement; }
        }

        #endregion

        #region override methods

        /// <summary>
        /// Called from <see cref="IGridCellRenderer.PrepareUIElments"/> to
        /// prepare the cells UIElement children.
        /// VirtualizingCellRendererBase overrides this method and
        /// creates new UIElements and wires them with the parent cells control.
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Column Index of the cell</param>
        /// <param name="record">Record for the coresponding row</param>
        /// <param name="column">Corresponding Grid Column</param>
        /// <param name="cellContainer">Corresponding Cell Element</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected sealed override UIElement OnPrepareUIElements(RowColumnIndex cellRowColumnIndex,UIElement cellContainer,GridColumn column, object record,bool isInEdit)
        {
            FrameworkElement cellcontent = null;
            this.currentCellRootElement = null;

            if (!UseOnlyRendererElement)
            {
                if (cellContainer == null) cellContainer = new GridCell();
                this.currentCellRootElement = cellContainer;
                //SetAlignmentBinding(column);
            }

            if (this.SupportsRenderOptimization && !isInEdit)
            {
                cellcontent = CreateOrDisplayRecycleUIElement();
                InitializeDisplayElement(cellRowColumnIndex, (D)cellcontent, column, record);
            }
            else
            {
                cellcontent = CreateOrEditRecycleUIElement();
                if (UseOnlyRendererElement)
                    currentCellRootElement = cellcontent;
                InitializeEditElement(cellRowColumnIndex, (E)cellcontent, column, record);
#if !WP
                WireEditUIElement((E)cellcontent);
#endif
            }

            if (!UseOnlyRendererElement)
            {
                if (cellContainer is GridCell)
                {
                    (cellContainer as GridCell).Content = cellcontent;
                }
            }  
            UpdateToolTip(cellRowColumnIndex, this.CurrentCellRootElement, column);
            return this.CurrentCellRootElement;
        }
#if !WP
        public sealed override bool BeginEdit(RowColumnIndex cellRowColumnIndex, UIElement cellElement, GridColumn column, object record)
        {
            if (!this.HasCurrentCellState)
                return false;

            if (this.SupportsRenderOptimization)
            {
                E cellcontent = null;
                this.currentCellRootElement = null;

                if (!UseOnlyRendererElement)
                {
                    if (cellElement == null) cellElement = new GridCell();
                    this.currentCellRootElement = cellElement;
                }

                if (this.SupportsRenderOptimization)
                    OnUnloadUIElements(cellRowColumnIndex, cellElement);

                cellcontent = CreateOrEditRecycleUIElement();
                if (UseOnlyRendererElement)
                    currentCellRootElement = cellcontent;
                InitializeEditElement(cellRowColumnIndex, cellcontent, column, record);
                WireEditUIElement(cellcontent);

                if (!UseOnlyRendererElement)
                {
                    if (cellElement is GridCell)
                        (cellElement as GridCell).Content = cellcontent;
                }
                //SetAlignmentBinding(cellcontent, column);
                OnEnteredEditMode(cellcontent);
            }
            else
            {
                OnEnteredEditMode(this.CurrentCellRendererElement);
            }
            return this.IsInEditing;
        }

        protected virtual void OnEnteredEditMode(UIElement currentRendererElement)
        {
            this.UpdateCurrentCellState(currentRendererElement, true);
        }

        public sealed override bool EndEdit(RowColumnIndex cellRowColumnIndex, UIElement cellElement, GridColumn column, object record)
        {
            if (!this.HasCurrentCellState)
                return false;
            if (!this.IsInEditing)
                return false;
            if (this.SupportsRenderOptimization)
            {
#if WPF
                E uiElement = null;
                if (!UseOnlyRendererElement && cellElement is GridCell)
                    uiElement = (cellElement as GridCell).Content as E;
                else
                    uiElement = cellElement as E;
                uiElement.PreviewLostKeyboardFocus -= OnLostKeyboardFocus;
#endif
                this.IsFocused = false;

                D cellcontent = null;
                this.currentCellRootElement = null;

                if (!UseOnlyRendererElement)
                {
                    if (cellElement == null) cellElement = new GridCell();
                    this.currentCellRootElement = cellElement;
                }

                OnUnloadUIElements(cellRowColumnIndex, cellElement);

                cellcontent = CreateOrDisplayRecycleUIElement();
                if (UseOnlyRendererElement)
                    currentCellRootElement = cellcontent;
                InitializeDisplayElement(cellRowColumnIndex, cellcontent, column, record);
                WireDisplayUIElement(cellcontent);

                if (!UseOnlyRendererElement)
                {
                    if (cellElement is GridCell)
                        (cellElement as GridCell).Content = cellcontent;
                }
                //SetAlignmentBinding(cellcontent,column);
                OnEditingComplete(cellcontent);
            }
            else
            {
                OnEditingComplete(this.CurrentCellRendererElement);
            }
            return !this.IsInEditing;
        }

        protected virtual void OnEditingComplete(UIElement currentRendererElement)
        {
            this.UpdateCurrentCellState(currentRendererElement, false);
        }
#endif
        /// <summary>
        /// Called from <see cref="IGridCellRenderer.UnloadUIElements"/> after a cell is scrolled out of view.
        /// VirtualizingCellRendererBase overrides this method and
        /// creates either removes the cell renderer visuals from the parent canvas
        /// or hide them and reuse it later in same canvas depending on whether
        /// <see cref="AllowRecycle"/> was set.
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Coloumn Index of the cell</param>
        /// <param name="uiElements">UIElement to unload</param>
        /// <remarks></remarks>
        protected override void OnUnloadUIElements(RowColumnIndex cellRowColumnIndex, UIElement uiElements)
        {
            if (AllowRecycle)
            {
                if (this.HasCurrentCellState && this.IsInEditing && this.CurrentCellIndex == cellRowColumnIndex)
                {
                    UnloadEditUIElement(uiElements);
                }
                else
                {
                    if (SupportsRenderOptimization)
                        UnloadDisplayUIElement(uiElements);
                    else
                        UnloadEditUIElement(uiElements);
                }
            }
        }

        /// <summary>
        /// Called from <see cref="IGridCellRenderer.Arrange"/> to
        /// arrange the cells UIElement children. 
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Column Index of the cell</param>
        /// <param name="uiElement">Corresponding UiElement</param>
        /// <param name="cellRect">Cell Rect for arranging the UIElement</param>
        /// <remarks></remarks>
        protected override void OnArrange(RowColumnIndex cellRowColumnIndex, UIElement uiElement, Rect cellRect)
        {
            uiElement.Arrange(cellRect);
        }

        /// <summary>
        /// Called from <see cref="IGridCellRenderer.Measure"/> to
        /// Measure the cells UIElement children. 
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Column Index of the cell</param>
        /// <param name="uiElement">Corresponding UiElement</param>
        /// <param name="availableSize">Corresponding Size for measuring the  UIElement size</param>
        /// <remarks></remarks>
        protected override void OnMeasure(RowColumnIndex cellRowColumnIndex, UIElement uiElement, Size availableSize)
        {
            uiElement.Measure(availableSize);
        }

        /// <summary>
        /// Called from <see cref="IGridCellRenderer.UpdateBindingInfo"/> to
        /// Update the binding of the Cell UIElement corresponding to GridColumn.
        /// In our control we are reusing the cell elements for horizontal scrolling.
        /// Hence we need to update the binding details of the cell UIElement.
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Column Index of the cell</param>
        /// <param name="uiElement">Corresponding UiElement</param>
        /// <param name="column">Corresponding column for update the binding info</param>
        /// <remarks></remarks>
        protected sealed override void OnUpdateBindingInfo(RowColumnIndex cellRowColumnIndex, UIElement uiElement, GridColumn column, object record, bool isInEdit)
        {
            this.currentCellRootElement = !UseOnlyRendererElement ? uiElement : null;
            FrameworkElement rendererElement = null;
            if (UseOnlyRendererElement)
                rendererElement = uiElement as FrameworkElement;
            else if (uiElement is GridCell)
                rendererElement = (uiElement as GridCell).Content as FrameworkElement;

            if (this.SupportsRenderOptimization && !isInEdit)
                OnUpdateDisplayBinding(cellRowColumnIndex, (D)rendererElement, column, record);
            else
                OnUpdateEditBinding(cellRowColumnIndex, (E)rendererElement, column, record);
        }

        
        /// <summary>
        /// Called from <see cref="IGridCellRenderer.UpdateStyleInfo"/> to
        /// update the cell appearance as per the customer need through API's and Selectors.
        /// In our control we are reusing the cell elements for scrolling.
        /// Hence we need to update the styles of the cell UIElement.
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Column Index of the cell</param>
        /// <param name="uiElement">Corresponding UiElement</param>
        /// <param name="column">Corresponding column for update the style info</param>
        /// <remarks></remarks>
        protected sealed override void OnUpdateStyleInfo(RowColumnIndex cellRowColumnIndex, UIElement uiElement, GridColumn column)
        {
            if (uiElement.Visibility == Visibility.Collapsed) return;
            this.currentCellRootElement = !UseOnlyRendererElement ? uiElement : null;
            var record = (uiElement as FrameworkElement) != null ? (uiElement as FrameworkElement).DataContext : null;
            if(record==null)
                return;
            this.InitializeCellStyle(cellRowColumnIndex, record, uiElement, column);
            UpdateToolTip(cellRowColumnIndex, uiElement, column);
        }

        protected void UpdateToolTip(RowColumnIndex cellRowColumnIndex, UIElement uiElement, GridColumn column)
        {
            if (this is GridSummaryCellRenderer || this is GridCaptionSummaryCellRenderer)
                return;
#if !SILVERLIGHT && !WP
            
            if (column != null && (column.ToolTipTemplate != null || column.HeaderToolTipTemplate != null || column.ToolTipTemplateSelector != null))
#else
            if (column != null && (column.ToolTipTemplate != null || column.HeaderToolTipTemplate != null))
#endif
            {
                if (cellRowColumnIndex.RowIndex < this.DataGrid.HeaderLineCount)
                {
                    if (column.HeaderToolTipTemplate != null)
                    {
                        var tooltip = ToolTipService.GetToolTip(uiElement) as ToolTip ?? new ToolTip();
                        tooltip.Content = column.HeaderToolTipTemplate.LoadContent();
                        ToolTipService.SetToolTip(uiElement, tooltip);
                    }
                    return;
                }
#if !SILVERLIGHT && !WP
                else if (column.ToolTipTemplate != null || column.ToolTipTemplateSelector != null)
                {
                    var tooltip = ToolTipService.GetToolTip(uiElement) as ToolTip ?? new ToolTip();
                    DataTemplate template = null;
                    if (column.ToolTipTemplateSelector != null)
                        template = column.ToolTipTemplateSelector.SelectTemplate(DataGrid, uiElement);
#else
                else if (column.ToolTipTemplate != null)
                {
                    var tooltip = ToolTipService.GetToolTip(uiElement) as ToolTip ?? new ToolTip();
                    DataTemplate template = null;
#endif
                    if (template == null && column.ToolTipTemplate != null)
                        template = column.ToolTipTemplate;
                    if (template != null)
                    {
                        tooltip.Content = template.LoadContent();
                        ToolTipService.SetToolTip(uiElement, tooltip);
                        return;
                    }
                }
            }
            ToolTipService.SetToolTip(uiElement, null);
        }

        #endregion

        #region virtual methods

        /// <summary>
        /// Creates a new UIElement of type specified with the class type parameter.
        /// </summary>
        protected virtual E OnCreateEditUIElement()
        {
            var uiElement = new E();
#if WPF
            Validation.SetErrorTemplate(uiElement, null);
#endif
            return uiElement;
        }

        protected virtual D OnCreateDisplayUIElement()
        {
            var uiElement = new D();
#if WPF
            Validation.SetErrorTemplate(uiElement, null);
#endif
            return uiElement;
        }

        #endregion

        #region abstract methods

        public abstract void OnInitializeDisplayElement(RowColumnIndex rowColumnIndex, D uiElement, GridColumn column, object dataContext);

        public abstract void OnUpdateDisplayBinding(RowColumnIndex cellRowcolumnIndex, D uiElement, GridColumn column, object dataContext);

        /// <summary>
        /// Called to initialize the content of the cell through binding 
        /// using the information from the column (MappingName)
        /// You must override this method in your
        /// derived class.
        /// </summary>
        /// <param name="uiElement">Corresponding UIElement</param>
        /// <param name="column">Corresponding column for set the binding</param>
        /// <remarks></remarks>
        public abstract void OnInitializeEditElement(RowColumnIndex rowColumnIndex, E uiElement, GridColumn column, object dataContext);

        /// <summary>
        /// Called to update the binding information of the Cell UIElement 
        /// using the information from column.
        /// You must override this method in your
        /// derived class.
        /// </summary>
        /// <param name="cellRowcolumnIndex">Cell Row Column Index</param>
        /// <param name="element">Corresponding cell UIElement</param>
        /// <param name="column">Corresponding column for update the binding</param>
        /// <remarks></remarks>
        public abstract void OnUpdateEditBinding(RowColumnIndex cellRowcolumnIndex, E element, GridColumn column, object dataContext);

        #endregion

        #region public methods

        /// <summary>
        /// Called from <see cref="OnPrepareUiElement"/> to initialize the content of the cell 
        /// using the information from the column (MappingName)
        /// . The method calls the virtual <see cref="OnInitializeEditElement"/> which 
        /// you should override in your derived class.
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="column"></param>
        /// <remarks></remarks>
        public void InitializeEditElement(RowColumnIndex rowColumnIndex, E uiElement, GridColumn column, object dataContext)
        {
            OnInitializeEditElement(rowColumnIndex, uiElement, column, dataContext);
        }

        public void InitializeDisplayElement(RowColumnIndex rowColumnIndex, D uiElement, GridColumn column, object dataContext)
        {
            OnInitializeDisplayElement(rowColumnIndex, uiElement, column, dataContext);
        }

        #endregion

        #region private methods

        /// <summary>
        /// Method which is return the UIElement for Cell.
        /// This method will create new element or Recycle the old element.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private D CreateOrDisplayRecycleUIElement()
        {
            D uiElement;
            if (AllowRecycle)
            {
                uiElement = DisplayRecycleBin.Dequeue(this);

                if (uiElement != null)
                {
                    return uiElement;
                }
            }
            uiElement = OnCreateDisplayUIElement();
            return uiElement;
        }

        private E CreateOrEditRecycleUIElement()
        {
            E uiElement;
            if (AllowRecycle)
            {
                uiElement = EditRecycleBin.Dequeue(this);

                if (uiElement != null)
                {
                    return uiElement;
                }
            }
            uiElement = OnCreateEditUIElement();
            return uiElement;
        }

        private void UnloadEditUIElement(UIElement uiElements)
        {
            E uiElement = null;
            if (!UseOnlyRendererElement && uiElements is GridCell)
                uiElement = (uiElements as GridCell).Content as E;
            else
                uiElement = uiElements as E;
            if (uiElement != null)
            {
#if !WP
                UnwireEditUIElement(uiElement);
#endif
#if !WinRT
                        EditRecycleBin.Enqueue(this, uiElement);
#endif
                if (!UseOnlyRendererElement && uiElements is GridCell)
                    (uiElements as GridCell).Content = null;
                else if (uiElement.Parent is Panel)
                    (uiElement.Parent as Panel).Children.Remove(uiElement);
            }
        }

        private void UnloadDisplayUIElement(UIElement uiElements)
        {
            D uiElement = null;
            if (!UseOnlyRendererElement && uiElements is GridCell)
                uiElement = (uiElements as GridCell).Content as D;
            else
                uiElement = uiElements as D;
            if (uiElement != null)
            {
                UnwireDisplayUIElement(uiElement);
                DisplayRecycleBin.Enqueue(this, uiElement);
                if (!UseOnlyRendererElement && uiElements is GridCell)
                    (uiElements as GridCell).Content = null;
                else if (uiElement.Parent is Panel)
                    (uiElement.Parent as Panel).Children.Remove(uiElement);
            }
        }

        private void WireDisplayUIElement(D uiElamant)
        {
            OnWireDisplayUIElement(uiElamant);
        }

        private void UnwireDisplayUIElement(D uiElamant)
        {
            OnUnwireDisplayUIElement(uiElamant);
        }

        protected virtual void OnWireDisplayUIElement(D uiElement)
        {
            
        }

        protected virtual void OnUnwireDisplayUIElement(D uiElement)
        {

        }
#if !WP
        /// <summary>
        /// Wire the Events to UIElement
        /// </summary>
        /// <param name="uiElement"></param>
        /// <remarks></remarks>
        private void WireEditUIElement(E uiElement)
        {
            OnWireEditUIElement(uiElement);
            uiElement.LostFocus += OnEditElementLostFocus;
            uiElement.Loaded += OnEditElementLoaded;
#if WPF
            uiElement.PreviewLostKeyboardFocus += OnLostKeyboardFocus;
#endif
        }

        /// <summary>
        /// Unwire the Events from UIElement
        /// </summary>
        /// <param name="uiElement"></param>
        /// <remarks></remarks>
        private void UnwireEditUIElement(E uiElement)
        {
            OnUnwireEditUIElement(uiElement);
            uiElement.LostFocus -= OnEditElementLostFocus;
            uiElement.Loaded -= OnEditElementLoaded;
#if WPF
            uiElement.PreviewLostKeyboardFocus -= OnLostKeyboardFocus;
#elif SILVERLIGHT
            uiElement.Loaded -= OnEditElementLoaded;
#endif
        }


        /// <summary>
        /// Called when [wire edit unique identifier element].
        /// </summary>
        /// <param name="uiElement">The unique identifier element.</param>
        protected virtual void OnWireEditUIElement(E uiElement)
        {
            
        }

        /// <summary>
        /// Called when [unwire edit unique identifier element].
        /// </summary>
        /// <param name="uiElement">The unique identifier element.</param>
        protected virtual void OnUnwireEditUIElement(E uiElement)
        {

        }

        /// <summary>
        /// Called when [edit element loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnEditElementLoaded(object sender, RoutedEventArgs e)
        {
        }

#if WPF
        void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ValidationHelper.IsFocusSetBack = false;
            var uiElement = sender as DependencyObject;
            if (uiElement != null)
            {
                var IsKeyboardFocusWithin = false;
                var focusedElement = e.NewFocus as DependencyObject;
                if (focusedElement != null)
                {
                    if (focusedElement == e.Source) return;
                    List<DependencyObject> childrens = new List<DependencyObject>();
                    GridUtil.Descendant(uiElement, ref childrens);
                    IsKeyboardFocusWithin = childrens.Contains(focusedElement);
                }
                if (IsKeyboardFocusWithin) return;
            }
            if (this.HasCurrentCellState && !CheckToAllowFocus(e.NewFocus, sender))
            {
                e.Handled = true;
                (sender as Control).CaptureMouse();
                ValidationHelper.IsFocusSetBack = true;
            }
            GridHeaderCellControl.isFilterToggleButtonClicked = false;
        }
#endif
        
        protected virtual void OnEditElementLostFocus(object sender, RoutedEventArgs e)
        {
#if !WPF
            if(!GridHeaderCellControl.isFilterToggleButtonClicked)
            ValidationHelper.IsFocusSetBack = false;
#endif
            if (this.HasCurrentCellState && this.CurrentCellRendererElement == sender)
                this.isfocused = false;
#if SILVERLIGHT || WinRT
            var uiElement = sender as UIElement;
            if (uiElement != null)
            {
                var IsKeyboardFocusWithin = false;
                var focusedElement = FocusManager.GetFocusedElement() as DependencyObject;
                if (focusedElement != null)
                {
#if SILVERLIGHT
                    if (focusedElement == sender) return;
#endif
                    List<DependencyObject> childrens = new List<DependencyObject>();
                    GridUtil.Descendant(uiElement, ref childrens);
                    IsKeyboardFocusWithin = childrens.Contains(focusedElement);
                }
                if (IsKeyboardFocusWithin)
                    return;
            }
#endif

#if !WPF
            if (HasCurrentCellState && !CheckToAllowFocus(FocusManager.GetFocusedElement(), sender))
            {
#if WinRT
                (sender as Control).Focus(FocusState.Programmatic);
                ValidationHelper.IsFocusSetBack = true;
#else
                if (!(sender as Control).Focus())
                {
                    (sender as Control).Loaded -= OnEditElementLoaded;
                    (sender as Control).Loaded += OnEditElementLoaded;
                }
                (sender as Control).CaptureMouse();
                ValidationHelper.IsFocusSetBack = true;
#endif
            }
#endif
            GridHeaderCellControl.isFilterToggleButtonClicked = false;
        }
        private bool CheckToAllowFocus(object element, object sender)
        {
            if (this is GridUnBoundCellRenderer || this is GridCellTemplateRenderer || (element is GridCell && sender.Equals(((element as GridCell).Content))))
                return true;

            if (this.DataGrid!=null && this.DataGrid.Validations.RaiseCellValidate(this.CurrentCellIndex, this, true))
            {
                if (!(element is GridCell))
                    return this.DataGrid.Validations.RaiseRowValidate(this.CurrentCellIndex);
                else if ((element as GridCell).ColumnBase.RowIndex != this.CurrentCellIndex.RowIndex)
                    return this.DataGrid.Validations.RaiseRowValidate(this.CurrentCellIndex);

            }
            else
                return false;
            return true;
        }
#endif

        /// <summary>
        /// Method which is used to Initialize the custom style for cell
        /// by using corresponding API's and Selectors.
        /// </summary>
        /// <param name="cellRowColumnIndex">Cell Row Column Index</param>
        /// <param name="element">Corresponding cell UIElement</param>
        /// <param name="column">Corresponding column for update the style of cell</param>
        /// <remarks></remarks>
        protected virtual void InitializeCellStyle(RowColumnIndex cellRowColumnIndex, object record, UIElement cell, GridColumn column)
        {
            this.SetCellStyle(cellRowColumnIndex, record,cell, column);
        }


#if !SILVERLIGHT && !WP
        /// <summary>
        /// Method which is used to set the Custom style for Cell.
        /// </summary>
        /// <param name="cellRowColumnIndex">Cell Row Column Index</param>
        /// <param name="element">Corresponding cell UIElement</param>
        /// <param name="column">Corresponding column for update the style of cell</param>
        /// <remarks></remarks>
        private void SetCellStyle(RowColumnIndex cellRowColumnIndex, object record, UIElement cell, GridColumn column)
        {
            Style newStyle = null;
            if (column == null)
                return;
            bool hasColumnCellStyleSelector = column.hasCellStyleSelector;
            bool hasColumnCellStyle = column.hasCellStyle;
            bool hasGridCellStyleSelector = DataGrid.hasCellStyleSelector;
            bool hasGridCellStyle = DataGrid.hasCellStyle;
            var gridCell = cell as GridCell;
            if (gridCell == null) return;
            if (!hasColumnCellStyleSelector && !hasColumnCellStyle && !hasGridCellStyle && !hasGridCellStyleSelector)
            {
                if (gridCell.ReadLocalValue(FrameworkElement.StyleProperty) != DependencyProperty.UnsetValue)
                    gridCell.ClearValue(FrameworkElement.StyleProperty);
                return;
            }

            if (hasColumnCellStyleSelector && hasColumnCellStyle)
            {
                newStyle = column.CellStyleSelector.SelectStyle(record, cell);
                newStyle = newStyle ?? column.CellStyle;
            }
            else if (hasColumnCellStyleSelector)
            {
                newStyle = column.CellStyleSelector.SelectStyle(record, cell);
            }
            else if (hasColumnCellStyle)
            {
                newStyle = column.CellStyle;
            }
            else if (hasGridCellStyleSelector && hasGridCellStyle)
            {
                newStyle = DataGrid.CellStyleSelector.SelectStyle(record, cell);
                newStyle = newStyle ?? DataGrid.CellStyle;
            }
            else if (hasGridCellStyleSelector)
            {
                newStyle = DataGrid.CellStyleSelector.SelectStyle(record, cell);
            }
            else if (hasGridCellStyle)
            {
                newStyle = DataGrid.CellStyle;
            }

            gridCell.Style = newStyle;
        }
#else
        /// <summary>
        /// Method which is used to set the Custom style for Cell.
        /// </summary>
        /// <param name="cellRowColumnIndex">Cell Row Column Index</param>
        /// <param name="element">Corresponding cell UIElement</param>
        /// <param name="column">Corresponding column for update the style of cell</param>
        /// <remarks></remarks>
        private void SetCellStyle(RowColumnIndex cellRowColumnIndex, object record, UIElement cell, GridColumn column)
        {
            Style newStyle = null;
            if (column == null)
                return;
            bool hasColumnCellStyleSelector = column.hasCellStyleSelector;
            bool hasColumnCellStyle = column.hasCellStyle;
            bool hasGridCellStyleSelector = DataGrid.hasCellStyleSelector;
            bool hasGridCellStyle = DataGrid.hasCellStyle;

            if (!hasColumnCellStyle && !hasGridCellStyle )
                return;

            if (hasColumnCellStyle)
            {
                newStyle = column.CellStyle;
            }
            
            else if (hasGridCellStyle)
            {
                newStyle = DataGrid.CellStyle;
            }

            (cell as GridCell).Style = newStyle;
        }

#endif


        #endregion

        public override void ClearRecycleBin()
        {
            DisplayRecycleBin.Clear();
            EditRecycleBin.Clear();
        }

        public override void Dispose()
        {
            if (this.DisplayRecycleBin != null)
            {
                this.DisplayRecycleBin.Clear();
                this.DisplayRecycleBin = null;
            }
            base.Dispose();
        }
    }
}

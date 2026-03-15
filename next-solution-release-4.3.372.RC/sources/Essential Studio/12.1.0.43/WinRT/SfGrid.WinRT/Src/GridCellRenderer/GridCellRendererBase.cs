#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
#if WinRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#else
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Controls;
#if !WP && !WinRT
using Syncfusion.Windows.Shared;
#endif
#endif


namespace Syncfusion.UI.Xaml.Grid.Cells
{
#if WinRT
    using Key = Windows.System.VirtualKey;
    using KeyEventArgs = KeyRoutedEventArgs;
#endif

    /// <summary>
    /// The <see cref="GridCellRendererBase"/> class provides a default implementation of 
    /// the <see cref="IGridCellRenderer"/> interface for a cell renderer.
    /// You should derive from this class to implement custom cell renderer classes. 
    /// There is however no dependency on GridCellRendererBase inside of the control. 
    /// <para/>
    /// If you want to implement a renderer with support for live UIElements 
    /// inside the cell you should derive from the <see cref="VirtualizingCellRendererBase{T}"/>
    /// or grid adapted VirtualizingCellRendererBase classes.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class GridCellRendererBase : IGridCellRenderer
    {
        #region Fields

        private bool hasCurrentCellState;
        private UIElement currentCellElement;
        private RowColumnIndex currentCellIndex;
        private bool isEditable=true;
        private bool isFocusible=true;
        private bool isDropDownable;
        private bool supportsRenderOptimization = true;
        private SfDataGrid dataGrid;
        private UIElement currentCellRendererElement;
        internal bool isfocused;
        private bool isInEditing;

        #endregion

        #region Ctor

        public GridCellRendererBase()
        {
            SupportsRenderOptimization = true;
        }

        #endregion

        #region Properties
#if !WP
        /// <summary>
        /// Gets or sets the Preview Input Text for the Renderer.
        /// </summary>
        protected dynamic PreviewInputText { get; set; }
#endif
        /// <summary>
        /// The DataGrid
        /// </summary>
        public SfDataGrid DataGrid
        {
            get { return dataGrid; }
            set { dataGrid = value; }
        }
#if !WinRT
        public BindingExpression BindingExpression { get; set; }
#endif

        /// <summary>
        /// Gets or sets whether the renderer supports rendering itsself directly to the
        /// drawing context. When this is possible the UIElement will only be created
        /// when the user moves the mouse over the cell or if the UIElement is needed for
        /// other reasons, e.g. animate after change. The benefit of rendering directly to the 
        /// DrawingContext instead of creating the UIElement is a much improved scrolling 
        /// performance. The default value is false.
        /// </summary>
        public bool SupportsRenderOptimization 
        {
            get
            {
                return supportsRenderOptimization;
            }
            set
            {
                supportsRenderOptimization = value;
            }
        }

        /// <summary>
        /// Gets or sets whether need to place the renderer element inside the GridCell or not
        /// </summary>
        public bool UseOnlyRendererElement { get; set; }

        /// <summary>
        /// Specifies whether the cell state for the current cell has been set.
        /// </summary>
        public bool HasCurrentCellState
        {
            get { return hasCurrentCellState; }
        }

        /// <summary>
        /// Returns the cell's row column index.
        /// </summary>
        public RowColumnIndex CurrentCellIndex
        {
            get
            {
                if (!HasCurrentCellState)
                    throw new InvalidOperationException("CellRowColumnIndex is only accesible when renderer is current cell. Check GridRenderStyleInfo.CellRowColumnIndex instead.");
                return currentCellIndex;
            }
        }

        /// <summary>
        /// Returns the UI element of current cell.
        /// </summary>
        public UIElement CurrentCellElement
        {
            get
            {
                if (!HasCurrentCellState)
                    throw new InvalidOperationException("CurrentCell Element is only accesible when renderer is current cell.");
                return currentCellElement;
            }
        }

        /// <summary>
        /// Returns the content of the current cell element.
        /// </summary>
        public UIElement CurrentCellRendererElement
        {
            get
            {
                if(!HasCurrentCellState)
                    throw new InvalidOperationException("CurrentCell Renderer Element is only accesible when renderer is current cell.");
                return currentCellRendererElement;
            }
        }

        /// <summary>
        /// Returns the current cell is in editing.
        /// </summary>
        public bool IsInEditing
        {
            get
            {
                if (!HasCurrentCellState)
                    throw new InvalidOperationException("IsInEditing only accesible when renderer is current cell.");
                return isInEditing;
            }
            set
            {
                isInEditing = value;
            }
        }

        /// <summary>
        /// Returns the current cell is focused.
        /// </summary>
        public bool IsFocused
        {
            get
            {
                if (!HasCurrentCellState)
                    throw new InvalidOperationException("IsFocused is only accesible when renderer is current cell.");
                return isfocused;
            }
            set
            {
                if (HasCurrentCellState)
                    SetFocus(currentCellRendererElement, value);
            }
        }

        #endregion

        #region virtual method

        /// <summary>
        /// Called from <see cref="IGridCellRenderer.OnPrepareUIElements"/> to
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
        protected virtual UIElement OnPrepareUIElements(RowColumnIndex cellRowColumnIndex, UIElement cellContainer, GridColumn column, object record, bool isInEdit)
        {
            return new GridCell();
        }

        /// <summary>
        /// Called when text is entered in the Data Control
        /// </summary>
        /// <param name="e">KeyRoutedEventArgs</param>
        /// <remarks></remarks>
#if !WinRT 
        protected virtual void OnPreviewTextInput(TextCompositionEventArgs e)
#else
        protected virtual void OnPreviewTextInput(KeyEventArgs e)
#endif
        {
#if !WinRT && !WP
            if (HasCurrentCellState)
                PreviewInputText = e.Text;
#endif
        }

        /// <summary>
        /// Called from <see cref="IGridCellRenderer.Arrange"/> to
        /// arrange the cells UIElement children. 
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Column Index of the cell</param>
        /// <param name="uiElement">Corresponding UiElement</param>
        /// <param name="cellRect">Cell Rect for arranging the UIElement</param>
        /// <remarks></remarks>
        protected virtual void OnArrange(RowColumnIndex cellRowColumnIndex, UIElement uiElement, Rect cellRect)
        {

        }

        /// <summary>
        /// Called from <see cref="IGridCellRenderer.Measure"/> to
        /// Measure the cells UIElement children. 
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Column Index of the cell</param>
        /// <param name="uiElement">Corresponding UiElement</param>
        /// <param name="availableSize">Corresponding Size for measuring the  UIElement size</param>
        /// <remarks></remarks>
        protected virtual void OnMeasure(RowColumnIndex cellRowColumnIndex, UIElement uiElement, Size availableSize)
        {

        }

        /// <summary>
        /// Called from <see cref="IGridCellRenderer.UnloadUIElements"/> after a cell is scrolled out of view.
        /// VirtualizingCellRendererBase overrides this method and
        /// creates either removes the cell renderer visuals from the parent canvas
        /// or hide them and reuse it later in same canvas depending on whether
        /// <see cref="AllowRecycle"/> was set.
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Coloumn Index of the cell</param>
        /// <param name="uiElement">UIElement to unload</param>
        /// <remarks></remarks>
        protected virtual void OnUnloadUIElements(RowColumnIndex cellRowColumnIndex, UIElement uiElement)
        {

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
        protected virtual void OnUpdateBindingInfo(RowColumnIndex cellRowColumnIndex, UIElement uiElement, GridColumn column, object record, bool isInEdit)
        {

        }

        /// <summary>
        /// Called from <see cref="IGridCellRenderer.OnUpdateStyleInfo"/> to
        /// update the cell appearance as per the customer need through API's and Selectors.
        /// In our control we are reusing the cell elements for scrolling.
        /// Hence we need to update the styles of the cell UIElement.
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Column Index of the cell</param>
        /// <param name="uiElemnt">Corresponding UiElement</param>
        /// <param name="column">Corresponding column for update the style info</param>
        /// <remarks></remarks>
        protected virtual void OnUpdateStyleInfo(RowColumnIndex cellRowColumnIndex, UIElement uiElemnt, GridColumn column)
        {

        }

        /// <summary>
        /// Let Renderer decide whether the parent grid should be allowed to handle keys and prevent
        /// the key event from being handled by the visual UIElement for this renderer. If this method
        /// returns true the parent grid will handle arrow keys and set the Handled flag in the event
        /// data. Keys that the grid does not handle will be ignored and be routed to the UIElement 
        /// for this renderer.
        /// </summary>
        /// <param name="e">A <see cref="KeyEventArgs"/> object.</param>
        /// <returns>True if the parent grid should be allowed to handle keys; false otherwise.</returns>
        protected virtual bool ShouldGridTryToHandleKeyDown(KeyEventArgs e)
        {
            return true;
        }

        /// <summary>
        /// Method to Set the focus on Current UI Element
        /// </summary>
        /// <param name="uiElement">Current cell renderer element</param>
        /// <param name="needToFocus">Need to set focus</param>
        protected virtual void SetFocus(UIElement uiElement, bool needToFocus)
        {
            if (uiElement != null && IsFocusible)
            {
                if (needToFocus)
                {
                    if (isfocused) return;
                    if (SupportsRenderOptimization && !IsInEditing)
                    {
#if !WinRT
                        DataGrid.Focus();
#else
                        DataGrid.Focus(FocusState.Programmatic);
#endif
                        isfocused = false;
                        return;
                    }
                    //if (IsInEditing)
                    {
#if WPF
                        isfocused = uiElement.Focus();
#elif SILVERLIGHT || WP
                        isfocused = (uiElement as Control).Focus();
#else
                        isfocused = uiElement is Control ? (uiElement as Control).Focus(FocusState.Programmatic) : false;
#endif
                        isfocused = true;
                        return;
                    }
                }
            }
#if !WinRT
            DataGrid.Focus();
#else
            DataGrid.Focus(FocusState.Programmatic);
#endif
            isfocused = false;

        }

        #endregion

        #region IGridCellRenderer

        /// <summary>
        /// Gets or sets a valude that indicates whether the cell is editable.
        /// </summary>
        public bool IsEditable
        {
            get
            {
                return isEditable;
            }
            set
            {
                isEditable = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the cell is focusable.
        /// </summary>
        public bool IsFocusible
        {
            get
            {
                return isFocusible;
            }
            set
            {
                isFocusible = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the cell can be dropped down.
        /// </summary>
        public bool IsDropDownable
        {
            get { return isDropDownable; }
            set { isDropDownable = value; }
        }

        /// <summary>
        /// Let Renderer decide whether the parent grid should be allowed to handle keys and prevent
        /// the key event from being handled by the visual UIElement for this renderer. If this method
        /// returns true the parent grid will handle arrow keys and set the Handled flag in the event
        /// data. Keys that the grid does not handle will be ignored and be routed to the UIElement 
        /// for this renderer.
        /// </summary>
        /// <param name="e">A <see cref="KeyEventArgs"/> object.</param>
        /// <returns>True if the parent grid should be allowed to handle keys; false otherwise.</returns>
        bool IGridCellRenderer.ShouldGridTryToHandleKeyDown(KeyEventArgs e)
        {
            if (!HasCurrentCellState) return true;
            return ShouldGridTryToHandleKeyDown(e);
        }

        /// <summary>
        /// Renderer will arrange the UIElement using this method
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Column Index of the cell</param>
        /// <param name="uiElement">Corresponding UiElement</param>
        /// <param name="cellRect">Cell Rect for arranging the UIElement</param>
        /// <remarks></remarks>
        public void Arrange(RowColumnIndex cellRowColumnIndex,UIElement uiElement, Rect cellRect)
        {
            OnArrange(cellRowColumnIndex,uiElement, cellRect);
        }

        /// <summary>
        /// Method which is used to Measure the Cell UIElement
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Column Index of the cell</param>
        /// <param name="uiElement">Corresponding UiElement</param>
        /// <param name="availableSize">Corresponding Size for measuring the  UIElement size</param>
        /// <remarks></remarks>
        public void Measure(RowColumnIndex cellRowColumnIndex,UIElement uiElement, Size availableSize)
        {
            OnMeasure(cellRowColumnIndex,uiElement, availableSize);
        }

        /// <summary>
        /// Method which is used to create and initiate the UIElement for Cell
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Column Index of the cell</param>
        /// <param name="record">Record for the coresponding row</param>
        /// <param name="column">Corresponding Grid Column</param>
        /// <param name="cellContainer">Corresponding Cell Element</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public UIElement PrepareUIElements(RowColumnIndex cellRowColumnIndex, UIElement cellContainer, GridColumn column, object record, bool isInEdit)
        {
            return OnPrepareUIElements(cellRowColumnIndex, cellContainer, column, record, isInEdit);
        }

        /// <summary>
        /// Method which is used for Unload the UIElement of the cell.(For Recyclling Purpose)
        /// </summary>
        /// <param name="cellRowColumnIndex">Cell Row Column Index</param>
        /// <param name="uiElement">Corresponding Cell UIElement</param>
        /// <remarks></remarks>
        public void UnloadUIElements(RowColumnIndex cellRowColumnIndex, UIElement uiElement)
        {
            OnUnloadUIElements(cellRowColumnIndex, uiElement);
        }

        /// <summary>
        /// Method which is used to Update the Binding Information of the Cell UIElement
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Column Index of the cell</param>
        /// <param name="uiElement">Corresponding UiElement</param>
        /// <param name="column">Corresponding column for update the binding info</param>
        /// <remarks></remarks>
        void IGridCellRenderer.UpdateBindingInfo(RowColumnIndex cellRowColumnIndex, UIElement uiElement, GridColumn column, object record, bool isInEdit)
        {
            OnUpdateBindingInfo(cellRowColumnIndex, uiElement, column, record, isInEdit);
        }

        /// <summary>
        /// Method which is used to update the style info of the Cell
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Column Index of the cell</param>
        /// <param name="uiElement">Corresponding UiElement</param>
        /// <param name="column">Corresponding column for update the style info</param>
        /// <remarks></remarks>
        public void UpdateCellStyle(RowColumnIndex cellRowColumnIndex, UIElement uiElement, GridColumn column)
        {
            OnUpdateStyleInfo(cellRowColumnIndex, uiElement, column);
        }

        /// <summary>
        /// Called when text is entered in the Data Control
        /// </summary>
        /// <param name="args">KeyRoutedEventArgs</param>
#if !WinRT
        public void PreviewTextInput(TextCompositionEventArgs args)
#else
        public void PreviewTextInput(KeyEventArgs args)
#endif
        {
            if (!HasCurrentCellState) 
                return;
            OnPreviewTextInput(args);
        }

        public virtual object GetControlValue()
        {
            return null;
        }

        public virtual void SetControlValue(object value)
        {
            
        }

        /// <summary>
        /// Update the current cell information in renderer while current cell was activated.
        /// </summary>
        /// <param name="currentCellIndex">CurrentCell Index</param>
        /// <param name="currentCellElement">CurrentCell UIElement</param>
        public void SetCurrentCellState(RowColumnIndex currentCellIndex, UIElement currentCellElement, bool isInEditing, bool isFocused)
        {
            //if (hasCurrentCellState)
            //    throw new InvalidOperationException("Try to set duplicate current cell");
            hasCurrentCellState = true;
            this.currentCellIndex = currentCellIndex;
            if (UseOnlyRendererElement)
            {
                this.currentCellElement = null;
                currentCellRendererElement = currentCellElement;
            }
            else
            {
                this.currentCellElement = currentCellElement;
                if (currentCellElement is GridCell)
                    currentCellRendererElement = (currentCellElement as GridCell).Content as UIElement;
            }
            IsInEditing = isInEditing;
            if (isFocused)
            {
                IsFocused = this.IsFocusible;
            }
        }

        protected void UpdateCurrentCellState(UIElement currentRendererElement, bool isInEdit)
        {
            currentCellRendererElement = currentRendererElement;
            isInEditing = isInEdit;
        }

        /// <summary>
        /// Reset the current cell information in renderer while current cell was deactivated.
        /// </summary>
        public void ResetCurrentCellState()
        {
            hasCurrentCellState = false;
            currentCellIndex = RowColumnIndex.Empty;
            currentCellElement = null;
            currentCellRendererElement = null;
            isfocused = false;
            isInEditing = false;
        }

        public void SetFocus(bool setFocus)
        {
            IsFocused = setFocus;
        }

        public virtual bool BeginEdit(RowColumnIndex cellRowColumnIndex, UIElement cellElement, GridColumn column, object record)
        {
            return HasCurrentCellState && IsInEditing;
        }

        public virtual bool EndEdit(RowColumnIndex cellRowColumnIndex, UIElement cellElement, GridColumn column, object record)
        {
            return HasCurrentCellState && !IsInEditing;
        }

#if !WinRT

        public virtual void UpdateSource(UIElement cellElement)
        {
            if(IsInEditing && BindingExpression!=null)
                BindingExpression.UpdateSource();
        }

#endif

        public virtual void ClearRecycleBin()
        {
            
        }
        #endregion

        #region Dispose method

        /// <summary>
        /// Releases all resources used by the.
        /// <see cref="T:Syncfusion.UI.Xaml.Grid.Cells.GridCellRendererBase">GridCellRendererBase</see>.
        /// </summary>
        /// <remarks></remarks>
        public virtual void Dispose()
        {
            currentCellElement = null;
            currentCellRendererElement = null;
            dataGrid = null;
        }

        #endregion

    }
}

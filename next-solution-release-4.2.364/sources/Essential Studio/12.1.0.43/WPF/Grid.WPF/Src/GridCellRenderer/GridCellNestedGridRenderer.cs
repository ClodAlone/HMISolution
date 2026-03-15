#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;
using System.Windows.Input;
using Syncfusion.Windows.Controls.Grid.Automation.Peers;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Implements the model of a nested grid cell.
    /// </summary>
    /// <remarks>
    ///  You can nest grids inside a row, column or covered range. When you nest a grid inside a covered range 
    ///  you can specify whether the rows or columns derive their state from the parent control. You have multiple
    ///  independent options for both rows and columns.
    ///  
    /// The <see cref="GridNestedAxisLayout"/> enumeration defines the possible options for the rows and columns of a
    /// nested grid.
    /// ------------------------------------------------------------------------------------------
    /// Row     |   Column      |   Description
    /// ------------------------------------------------------------------------------------------
    /// Normal  |   Normal      | Nested grid will have its own row heights and column widths,
    ///                           independent of parent grid.
    ///         
    /// Shared  |   Normal      | Shared Row layout - Nested grid will have unique column widths,
    ///                           but its row heights are shared with the parent grid.
    ///         
    /// Normal  |   Shared      | Shared Column layout - Nested grid will have unique row heights,
    ///                           but its column widths are shared with the parent grid.
    /// 
    /// Nested  |   Nested      | Nested Layout - The rows and columns of the inner grid is nested
    ///                           inside a single row or column of the parent grid.
    /// -------------------------------------------------------------------------------------------
    /// </remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GridCellNestedGridModel : GridCellModelBase
    {
        GridNestedAxisLayout columnLayout;
        GridNestedAxisLayout rowLayout;

        /// <summary>
        /// Initializes a new <see cref="GridCellNestedGridModel"/> that maintains own sizes, independent of the parent grid.
        /// </summary>
        public GridCellNestedGridModel()
            : this(GridNestedAxisLayout.Normal, GridNestedAxisLayout.Normal)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridCellNestedGridModel"/>.
        /// </summary>
        /// <param name="rowLayout">Row layout.</param>
        /// <param name="columnLayout">Column layout.</param>
        public GridCellNestedGridModel(GridNestedAxisLayout rowLayout, GridNestedAxisLayout columnLayout)
        {
            this.rowLayout = rowLayout;
            this.columnLayout = columnLayout;
        }

        /// <summary>
        /// Creates the nested grid cell renderer with the specified row and column layouts.
        /// </summary>
        /// <returns>Cell renderer.</returns>
        public override IGridCellRenderer CreateRenderer()
        {
            IGridCellRenderer r = new GridCellNestedGridRenderer(rowLayout, columnLayout);
            r.RaiseCreated(this);
            return r;
        }

        /// <summary>
        /// Gets or sets the row layout for the nested grid. It can be normal, nested or shared.
        /// </summary>
        public GridNestedAxisLayout RowLayout
        {
            get { return rowLayout; }
            set { rowLayout = value; }
        }

        /// <summary>
        /// Gets or sets the column layout for the nested grid. It can be normal, nested or shared.
        /// </summary>
        public GridNestedAxisLayout ColumnLayout
        {
            get { return columnLayout; }
            set { columnLayout = value; }
        }

        /// <summary>
        /// Creates a copy of the current <see cref="GridCellNestedGridModel"/> object.
        /// </summary>
        /// <returns>A copy of the current object.</returns>
        public override GridCellModelBase Clone()
        {
            return new GridCellNestedGridModel(rowLayout, columnLayout);
        }
    }

    /// <summary>
    /// Implements the renderer part of a nested grid cell.
    /// </summary>
    public class GridCellNestedGridRenderer : GridVirtualizingCellRenderer<GridCellNestedGridEditor>
    {
        GridNestedAxisLayout rowLayout;
        GridNestedAxisLayout columnLayout;

        #region Ctor
        /// <summary>
        /// Initializes a new <see cref="GridCellNestedGridRenderer"/>.
        /// </summary>
        public GridCellNestedGridRenderer()
            : this(GridNestedAxisLayout.Normal, GridNestedAxisLayout.Normal)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridCellNestedGridRenderer"/> with the given row and column layouts.
        /// </summary>
        /// <param name="rowLayout">The row layout.</param>
        /// <param name="columnLayout">The column layout.</param>
        public GridCellNestedGridRenderer(GridNestedAxisLayout rowLayout, GridNestedAxisLayout columnLayout)
        {
            AllowRecycle = true;
            AllowRecycleIfIsKeyboardFocusWithin = true;
            SupportsRenderOptimization = false;
            this.rowLayout = rowLayout;
            this.columnLayout = columnLayout;

            IsControlTextShown = false;
            IsFocusable = true;
            IsModifiable = false;
            IsEditable = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Returns true if the row layout is shared; false otherwise.
        /// </summary>
        public bool ShareRowLayout
        {
            get { return rowLayout == GridNestedAxisLayout.Shared; }
        }

        /// <summary>
        /// Returns true if the column layout is shared; false otherwise.
        /// </summary>
        public bool ShareColumnLayout
        {
            get { return columnLayout == GridNestedAxisLayout.Shared; }
        }

        /// <summary>
        /// Returns true if the column layout is nested; false otherwise.
        /// </summary>
        public bool SingleColumnInParentLayout
        {
            get { return columnLayout == GridNestedAxisLayout.Nested; }
        }

        /// <summary>
        /// Returns true if the row layout is nested; false otherwise.
        /// </summary>
        public bool SingleRowInParentLayout
        {
            get { return rowLayout == GridNestedAxisLayout.Nested; }
        }
        #endregion

        #region Overrides

        /// <summary>
        /// Called to initialize the content of the cell
        /// using the information from the cell style (value, text,
        /// behavior etc.). You must override this method in your
        /// derived class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(GridCellNestedGridEditor element, GridRenderStyleInfo style)
        {
            element.InitializeNested(style.GridControl, style.CellValue as GridModel, rowLayout, columnLayout);
        }

        protected override string GetControlTextFromEditorCore(GridCellNestedGridEditor element)
        {
            return element.ToString();
        }

        protected override void OnUnwireUIElement(GridCellNestedGridEditor uiElement)
        {
            // When grid view goes out of view, unwire the view from the underlying Model so
            // that it can be garbage collected if needed. Model will be set again when
            // OnInitializeContent is called when cell is scrolled into view.
            uiElement.Model = null;
            base.OnUnwireUIElement(uiElement);
        }

        protected override void OnWireUIElement(GridCellNestedGridEditor uiElement)
        {
            base.OnWireUIElement(uiElement);
        }

        protected override void OnPrepareUIElements(ArrangeCellArgs aca, System.Collections.Generic.List<UIElement> uiElements, ScrollControlChildFrame canvas, GridRenderStyleInfo cellInfo)
        {
            base.OnPrepareUIElements(aca, uiElements, canvas, cellInfo);
        }

        public override void CreateRendererElement(GridCellNestedGridEditor uiElement, GridRenderStyleInfo style)
        {
            uiElement.InitializeNested(style.GridControl, style.CellValue as GridModel, rowLayout, columnLayout);
            base.CreateRendererElement(uiElement, style);
        }

        protected override void ArrangeUIElement(ArrangeCellArgs aca, GridCellNestedGridEditor uiElement, GridRenderStyleInfo style)
        {
            uiElement.ArrangeNested(null, aca, style);
            Rect clipRect = aca.SubtractBorderMargins(aca.CellClipRect, style.Padding.ToThickness());
            SetBounds(uiElement, clipRect, aca.ForceMeasure, true);
        }

        protected override void OnCancelMouseCapture(UIElement element)
        {
            VirtualizingCellsControl cellsControl = element as VirtualizingCellsControl;
            if (cellsControl != null)
                cellsControl.MouseControllerDispatcher.CancelMode();
            base.OnCancelMouseCapture(element);
        }

        protected override void OnRecaptureMouse(UIElement element)
        {
            VirtualizingCellsControl cellsControl = element as VirtualizingCellsControl;
            if (cellsControl != null)
                cellsControl.MouseControllerDispatcher.RestoreMode();
            base.OnRecaptureMouse(element);
        }

        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            if (CurrentCellUIElement == null)
                return true;

            // This does not work multiple-level nested grids! Only for the first level.
            if (!CurrentCellUIElement.IsKeyboardFocusWithin)
            {
                if (e.Key == Key.Down)
                {
                    GridRangeInfo r = CurrentCellUIElement.NavigateWithArrowKeysCellsRange;
                    CurrentCellUIElement.CurrentCell.CellRowColumnIndex = new RowColumnIndex(r.Top, r.Left);
                    CurrentCell.BeginEdit();
                    e.Handled = true;
                    return false;
                }

                if (e.Key == Key.Up)
                {
                    GridRangeInfo r = CurrentCellUIElement.NavigateWithArrowKeysCellsRange;
                    CurrentCellUIElement.CurrentCell.CellRowColumnIndex = new RowColumnIndex(r.Bottom, r.Left);
                    CurrentCell.BeginEdit();
                    e.Handled = true;
                    return false;
                }

                if (e.Key == Key.Left)
                {
                    GridRangeInfo r = CurrentCellUIElement.NavigateWithArrowKeysCellsRange;
                    CurrentCellUIElement.CurrentCell.CellRowColumnIndex = new RowColumnIndex(r.Top, r.Right);
                    CurrentCell.BeginEdit();
                    e.Handled = true;
                    return false;
                }

                if (e.Key == Key.Right)
                {
                    GridRangeInfo r = CurrentCellUIElement.NavigateWithArrowKeysCellsRange;
                    CurrentCellUIElement.CurrentCell.CellRowColumnIndex = new RowColumnIndex(r.Top, r.Left);
                    CurrentCell.BeginEdit();
                    e.Handled = true;
                    return false;
                }
            }
            return false;
        }

        //protected override void OnInitialize()
        //{
        //}

        protected override bool OnDeactivating()
        {
            //if (CurrentCellUIElement != null)
            //{
            //    CurrentCellUIElement.CurrentCell.Deactivate();
            //    CurrentCellUIElement.Model.Selections.Clear();
            //}
            return base.OnDeactivating();
        }
        protected override void OnDeactivated()
        {
        }

        protected override void ScrollInView()
        {
            int rowIndex = CellRowColumnIndex.RowIndex;
            // int columnIndex = CellRowColumnIndex.ColumnIndex; Unused local variable
            CoveredCellInfo cc = GridControl.GetCoveredCell(CellRowColumnIndex);
            // GridControlBase g = CurrentCellUIElement; Unused local variable

            if (this.rowLayout == GridNestedAxisLayout.Nested)
            {
                GridModel m = CurrentStyle.CellValue as GridModel;
                PixelScrollAxis px = GridControl.ScrollRows as PixelScrollAxis;
                if (m == null || px == null || GridControl.IsRowVisible(rowIndex))
                    return;

                if (GridControl.TopRowIndex > rowIndex)
                {
                    double rowSize = m.RowHeights[m.RowCount - 1];
                    double scrollPos = px.Distances.GetCumulatedDistanceAt(cc.Bottom);
                    GridControl.VScrollBar.Value = scrollPos + GridControl.ScrollRows.GetLineSize(rowIndex) - rowSize;
                }
                else
                {
                    double rowSize = m.RowHeights[0];
                    double scrollPos = px.Distances.GetCumulatedDistanceAt(rowIndex);
                    GridControl.VScrollBar.Value = scrollPos + rowSize - GridControl.VScrollBar.LargeChange;
                }
            }
            else if (rowLayout == GridNestedAxisLayout.Normal)
            {
            }

        }

        /// <summary>
        /// Refreshes the cell content.
        /// </summary>
        public override void RefreshContent()
        {
        }

        protected override void OnEditingComplete()
        {
            if (CurrentCellUIElement != null)
            {
                CurrentCellUIElement.CurrentCell.Deactivate();
                CurrentCellUIElement.Model.Selections.Clear();
                GridControl.InvalidateVisual(true);
            }
        }

        /// <summary>
        /// Refreshes the cell UI element.
        /// </summary>
        /// <param name="cellsControl">The cell control.</param>
        /// <param name="cellUIElements">The cell UI elements.</param>
        /// <param name="rowColumnIndex">The cell row column index.</param>
        public override void RefreshCellUIElementsContent(VirtualizingCellsControl cellsControl, CellUIElements cellUIElements, RowColumnIndex rowColumnIndex)
        {
        }

        /// <summary>
        /// Hide the given nested grid.
        /// </summary>
        /// <param name="e">The cell element.</param>
        public override void Hide(UIElement e)
        {
            base.Hide(e);
        }

        #endregion
    }

    /// <summary>
    /// Defines the nested grid editor control that is placed inside a cell to form a nested grid cell.
    /// It derives from <see cref="GridControlBase"/> and hence share the basic charateristics of the GridControl.
    /// </summary>
    public class GridCellNestedGridEditor : GridControlBase
    {
        GridControlBase parentGrid;
        GridNestedAxisLayout rowLayout;
        GridNestedAxisLayout columnLayout;
        bool inArrangeNested = false;

        /*~NestedSharedLayoutGrid()
        {
            Console.WriteLine("Finalize NestedSharedLayoutGrid");
        }*/

        /// <summary>
        /// Returns true if the row layout is shared; false otherwise.
        /// </summary>
        public bool ShareRowLayout
        {
            get { return rowLayout == GridNestedAxisLayout.Shared; }
        }

        /// <summary>
        /// Returns true if the column layout is shared; false otherwise.
        /// </summary>
        public bool ShareColumnLayout
        {
            get { return columnLayout == GridNestedAxisLayout.Shared; }
        }

        /// <summary>
        /// Returns true if the column layout is nested; false otherwise.
        /// </summary>
        public bool SingleColumnInParentLayout
        {
            get { return columnLayout == GridNestedAxisLayout.Nested; }
        }

        /// <summary>
        /// Returns true if the row layout is nested; false otherwise.
        /// </summary>
        public bool SingleRowInParentLayout
        {
            get { return rowLayout == GridNestedAxisLayout.Nested; }
        }

        /// <summary>
        /// Initializes a new <see cref="GridCellNestedGridEditor"/>.
        /// </summary>
        public GridCellNestedGridEditor()
        {
            //EnableRenderCellDrawingVisuals = false; // there are some issues with hittesting and 
            // current cell scrolling when this is enabled.
        }

        /// <summary>
        /// Initializes a new <see cref="GridCellNestedGridEditor"/> using the given parameter values.
        /// </summary>
        /// <param name="simpleGridControl">The grid control.</param>
        /// <param name="model">Grid model.</param>
        /// <param name="rowLayout">Row layout.</param>
        /// <param name="columnLayout">Column layout.</param>
        public virtual void InitializeNested(GridControlBase simpleGridControl, GridModel model, GridNestedAxisLayout rowLayout, GridNestedAxisLayout columnLayout)
        {
            if (model == null)
                throw new Exception("CellInfo.Value is not a GridModel");
            this.parentGrid = simpleGridControl;
            this.rowLayout = rowLayout;
            this.columnLayout = columnLayout;
            this.Model = model;
            ClipToBounds = true;
            //AutoScroller.Enabled = false;
            //VisualContainer.SetWantsMouseInput(BackgroundFrame, true);
        }

        protected override void WireModel()
        {
            Model.QueryCellModel += new GridQueryCellModelEventHandler(Model_QueryCellModel);
            base.WireModel();
        }

        protected override void UnwireModel()
        {
            Model.QueryCellModel -= new GridQueryCellModelEventHandler(Model_QueryCellModel);
            base.UnwireModel();
        }

        void Model_QueryCellModel(object sender, GridQueryCellModelEventArgs e)
        {
            if (parentGrid != null)
            {
                e.CellModel = parentGrid.Model.CellModels[e.CellType].Clone();
            }
        }

        /// <summary>
        /// Returns the parent grid.
        /// </summary>
        public GridControlBase ParentGrid
        {
            get { return parentGrid; }
        }

        /// <summary>
        /// Returns the Scroll Rows that are shared.
        /// </summary>
        public SharedSubsetScrollAxis SharedScrollRows
        {
            get { return (SharedSubsetScrollAxis)ScrollRows; }
        }

        /// <summary>
        /// Gets or sets the index of the first row.
        /// </summary>
        public int StartRowIndex
        {
            get
            {
                return ScrollRows.StartLineIndex;
            }
            set
            {
                ScrollRows.StartLineIndex = value;
            }
        }

        /// <summary>
        /// Returns the Scroll Columns that are shared.
        /// </summary>
        public SharedSubsetScrollAxis SharedScrollColumns
        {
            get { return (SharedSubsetScrollAxis)ScrollColumns; }
        }

        /// <summary>
        /// Gets or sets the index of the first column.
        /// </summary>
        public int StartColumnIndex
        {
            get
            {
                return ScrollColumns.StartLineIndex;
            }
            set
            {
                ScrollColumns.StartLineIndex = value;
            }
        }

        /// <summary>
        /// When true, it indicates the nested grid cell content are being arranged.
        /// </summary>
        public bool IsInArrangeNested
        {
            get { return inArrangeNested; }
        }

        /// <summary>
        /// Arranges the given cell content inside the scroll viewer.
        /// </summary>
        /// <param name="sv">Scroll viewer.</param>
        /// <param name="aca">A reference to <see cref="ArrangeCellArgs"/>.</param>
        public void ArrangeNested(ScrollViewer sv, ArrangeCellArgs aca, GridRenderStyleInfo style)
        {
            inArrangeNested = true;

            CellSpanInfo cellSpan;
            if (aca.VisibleCoveredCellInfo != null)
                cellSpan = aca.VisibleCoveredCellInfo.CellSpan;
            else
                cellSpan = new CellSpanInfo(aca.RowIndex, aca.ColumnIndex, aca.RowIndex, aca.ColumnIndex);

            this.StartRowIndex = cellSpan.Top;
            if (ShareRowLayout)
            {
                SharedScrollRows.Distances.Count = cellSpan.Height;
            }

            this.StartColumnIndex = cellSpan.Left;
            if (ShareColumnLayout)
            {
                SharedScrollColumns.Distances.Count = cellSpan.Width;
            }
            Size clipRectSize;
            if (style != null)//Remove the padding from the CellCliprect
                clipRectSize = new Size(aca.CellClipRect.Size.Width - (style.Padding.Left+style.Padding.Right), aca.CellClipRect.Size.Height);
            else
                clipRectSize = aca.CellClipRect.Size;

            this.UpdateAxis(clipRectSize);

            this.ScrollColumns.UpdateScrollBar();
            this.ScrollRows.UpdateScrollBar();

            switch (rowLayout)
            {
                case GridNestedAxisLayout.Normal:
                    this.VScrollBar.Value = Model.CachedVScrollValue + VScrollBar.Minimum + aca.CellClipRect.Top - aca.OriginalCellRect.Top;
                    break;

                case GridNestedAxisLayout.Shared:
                case GridNestedAxisLayout.Nested:
                    this.VScrollBar.Value = VScrollBar.Minimum + aca.CellClipRect.Top - aca.OriginalCellRect.Top;
                    break;
            }

            switch (columnLayout)
            {
                case GridNestedAxisLayout.Normal:
                    this.HScrollBar.Value = HScrollBar.Minimum + aca.CellClipRect.Left -aca.OriginalCellRect.Left +this.parentGrid.HScrollBar.Value-this.parentGrid.HScrollBar.Minimum;
                    break;

                case GridNestedAxisLayout.Shared:
                case GridNestedAxisLayout.Nested:
                    this.HScrollBar.Value = HScrollBar.Minimum + aca.CellClipRect.Left - aca.OriginalCellRect.Left;
                    break;
            }

            #region Possible show scrollbars - not implemented

            //if (false && sv != null)
            //{
            //    if (shareColumnLayout && HScrollBar.Value == this.HScrollBar.Maximum - HScrollBar.LargeChange)
            //        sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            //    else if (!shareRowLayout)
            //        sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;


            //    if (shareRowLayout && VScrollBar.Value == this.VScrollBar.Maximum - VScrollBar.LargeChange)
            //        sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            //    else if (!shareColumnLayout)
            //        sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            //}
            #endregion

            // Checks if rows were resized or if scroll position changed.
            if (!this.HScrollBar.Equals(this.HScrollBarShadow)
                || !this.VScrollBar.Equals(this.VScrollBarShadow))
            {
                this.InvalidateVisual();
                this.HScrollBar.CopyTo(this.HScrollBarShadow);
                this.VScrollBar.CopyTo(this.VScrollBarShadow);
            }
            inArrangeNested = false;
        }

        protected override void OnVScrollBarValueChanging(object sender, ValueChangingEventArgs e)
        {
            base.OnVScrollBarValueChanging(sender, e);
            if (e.Cancel || inArrangeNested || IsInArrangeContent)
                return;

            switch (rowLayout)
            {
                case GridNestedAxisLayout.Normal:
                    break;

                case GridNestedAxisLayout.Shared:
                case GridNestedAxisLayout.Nested:
                    {
                        // scroll the parent grid instead.
                        double delta = e.NewValue - e.OldValue;
                        parentGrid.VScrollBar.Value += delta;
                        //e.Cancel = true;
                    }
                    break;
            }
        }

        protected override void OnHScrollBarValueChanging(object sender, ValueChangingEventArgs e)
        {
            base.OnHScrollBarValueChanging(sender, e);
            if (e.Cancel || inArrangeNested || IsInArrangeContent)
                return;

            switch (columnLayout)
            {
                case GridNestedAxisLayout.Normal:
                    //below lines are commented to avoid the scrolling of parent grid when scroll the child grid
                    //double change= e.NewValue - e.OldValue;
                    //parentGrid.HScrollBar.Value += change;
                    break;

                case GridNestedAxisLayout.Shared:
                case GridNestedAxisLayout.Nested:
                    {
                        // scroll the parent grid instead.
                        double delta = e.NewValue - e.OldValue;
                        parentGrid.HScrollBar.Value += delta;
                        //e.Cancel = true;
                    }
                    break;
            }
        }

        protected override void OnVScrollBarValueChanged(object sender, EventArgs e)
        {
            base.OnVScrollBarValueChanged(sender, e);
            if (inArrangeNested || IsInArrangeContent)
                return;

            Model.CachedVScrollValue = VScrollBar.Value - VScrollBar.Minimum;
        }

        protected override void OnHScrollBarValueChanged(object sender, EventArgs e)
        {
            base.OnHScrollBarValueChanged(sender, e);
            if (inArrangeNested || IsInArrangeContent)
                return;

            Model.CachedHScrollValue = HScrollBar.Value - HScrollBar.Minimum;
        }

        /// <summary>
        /// Sets the height of the row whose index is specified.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="size">Height.</param>
        public override void SetRowHeight(int rowIndex, double size)
        {
            if (ShareRowLayout)
            {
                parentGrid.SetRowHeight(rowIndex + StartRowIndex, size);
                InvalidateRowResize();
            }
            else
                base.SetRowHeight(rowIndex, size);

            //if (SingleRowInParentLayout)
            //    parentGrid.SetRowHeight(StartRowIndex, RowHeights.Distances.TotalDistance);
        }

        public override void InvalidateRowResize()
        {
            base.InvalidateRowResize();
            parentGrid.InvalidateRowResize();
        }

        /// <summary>
        /// Sets the width of the column whose index is specified.
        /// </summary>
        /// <param name="columnIndex">Column index.</param>
        /// <param name="size">Width.</param>
        public override void SetColumnWidth(int columnIndex, double size)
        {
            if (ShareColumnLayout)
            {
                parentGrid.SetColumnWidth(columnIndex + StartColumnIndex, size);
                InvalidateVisual();
            }
            else
                base.SetColumnWidth(columnIndex, size);

            //if (SingleColumnInParentLayout)
            //    parentGrid.SetColumnWidth(StartColumnIndex, ColumnWidths.Distances.TotalDistance);
        }

        protected override ScrollAxisBase CreateScrollAxis(Orientation orientation, bool pixelScroll, IScrollBar scrollBar, ILineSizeHost lineSizes)
        {
            if (orientation == Orientation.Horizontal)
            {
                if (ShareColumnLayout)
                {
                    if (!(parentGrid.ScrollColumns is PixelScrollAxis))
                        throw new InvalidOperationException("You must enable HorizontalPixelScroll for the parent grid.");

                    return new SharedSubsetScrollAxis((PixelScrollAxis)parentGrid.ScrollColumns, scrollBar, lineSizes);
                }
                else if (SingleColumnInParentLayout)
                {
                    return new PixelScrollAxis(parentGrid.ScrollColumns, scrollBar, lineSizes, lineSizes as IDistancesHost);
                }

            }
            else
            {
                if (ShareRowLayout)
                {
                    if (!(parentGrid.ScrollRows is PixelScrollAxis))
                        throw new InvalidOperationException("You must enable VerticalPixelScroll for the parent grid.");
                    return new SharedSubsetScrollAxis((PixelScrollAxis)parentGrid.ScrollRows, scrollBar, lineSizes);
                }
                else if (SingleRowInParentLayout)
                {
                    return new PixelScrollAxis(parentGrid.ScrollRows, scrollBar, lineSizes, lineSizes as IDistancesHost);
                }


            }
            return base.CreateScrollAxis(orientation, pixelScroll, scrollBar, lineSizes);
        }

        #region Scroll Arrows - not implemented
        //bool showScrollRightArrow = false;
        //Rectangle rbScrollRightArrow;

        //public bool ShowScrollRightArrow
        //{
        //    get { return showScrollRightArrow; }
        //    set 
        //    {
        //        if (showScrollRightArrow != value)
        //        {
        //            showScrollRightArrow = value;

        //            if (value)
        //            {
        //                object obj = Application.Current.Resources["LeftArrowBlue"];
        //                DrawingBrush db = (DrawingBrush)obj;

        //                rbScrollRightArrow = new Rectangle();
        //                rbScrollRightArrow.Fill = db;

        //                ForegroundFrame.Children.Add(rbScrollRightArrow);
        //                Rect r = new Rect(RenderSize.Width - 20, RenderSize.Height / 2 - 10, 20, 20);
        //                rbScrollRightArrow.Arrange(r);

        //                rbScrollRightArrow.PreviewMouseMove += new MouseEventHandler(rbScrollRightArrow_PreviewMouseMove);
        //                rbScrollRightArrow.PreviewMouseDown += new MouseButtonEventHandler(rbScrollRightArrow_PreviewMouseDown);

        //            }
        //            else
        //            {
        //                rbScrollRightArrow.PreviewMouseMove -= new MouseEventHandler(rbScrollRightArrow_PreviewMouseMove);
        //                rbScrollRightArrow.PreviewMouseDown -= new MouseButtonEventHandler(rbScrollRightArrow_PreviewMouseDown);
        //                ForegroundFrame.Children.Remove(rbScrollRightArrow);
        //                rbScrollRightArrow = null;
        //            }
        //        }
        //    }
        //}

        //void rbScrollRightArrow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    LineRight();
        //    e.Handled = true;
        //}

        //void rbScrollRightArrow_PreviewMouseMove(object sender, MouseEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //bool showScrollUpArrow = false;
        //bool showScrollLeftArrow = false;
        //bool showScrollDownArrow = false;

        //protected override void OnMouseEnter(MouseEventArgs e)
        //{
        //    ShowScrollRightArrow = !shareColumnLayout && HScrollBar.Value < HScrollBar.Maximum - HScrollBar.LargeChange;

        //    base.OnMouseEnter(e);
        //}

        //protected override void OnMouseLeave(MouseEventArgs e)
        //{
        //    ShowScrollRightArrow = false;

        //    base.OnMouseLeave(e);
        //}
        #endregion

        /// <summary>
        /// Moves the current cell when user tries to move the current cell using the arrow key.
        /// </summary>
        /// <param name="e">The key event arguments.</param>
        /// <returns>True if the current cell has been moved successfully; false otherwise.</returns>
        public override bool MoveCurrentCellWithArrowKey(KeyEventArgs e)
        {
            bool b = base.MoveCurrentCellWithArrowKey(e);
            if (CurrentCell.Renderer is GridDataCellNestedGridRenderer)
            {
                var ChildModel = CurrentCell.Renderer.CurrentCellUIElement as GridDataCellNestedGridEditor;
                if (ChildModel != null)
                    ChildModel.IsInShiftTab = this.IsInShiftTab;
                if (CurrentCell.Renderer.IsFocusable)
                    CurrentCell.Renderer.IsFocused = true;
            }
            bool selectAll = e.Key == Key.A && e.KeyboardDevice.Modifiers == ModifierKeys.Control;
            if (!b && ParentGrid != null && !selectAll)
            {
                if (e.Key == Key.Tab || (e.Key == Key.Tab && e.KeyboardDevice.Modifiers == ModifierKeys.Shift) || e.Key == Key.Down 
                    || e.Key == Key.Up || e.Key == Key.PageDown || e.Key == Key.PageUp || e.Key== Key.Left || e.Key == Key.Right)
                {
                    if (this.CurrentCell.HasCurrentCell)
                        this.CurrentCell.Deactivate();
                }
                b = ParentGrid.MoveCurrentCellWithArrowKey(e);
            }
            return b;
        }

        /// <summary>
        /// Brings the given cell into view.
        /// </summary>
        /// <param name="cellRowColumnIndex">The cell row column index.</param>
        public override void ScrollInView(RowColumnIndex cellRowColumnIndex)
        {
            ScrollRows.ScrollInView(cellRowColumnIndex.RowIndex);
            ScrollColumns.ScrollInView(cellRowColumnIndex.ColumnIndex);
        }

        //protected override void OnInvalidated(bool isArrangeDirty)
        //{
        //    base.OnInvalidated(isArrangeDirty);
        //    if (isArrangeDirty)
        //    {
        //        RowColumnIndex cell = VirtualizingCellsControl.GetCellRowColumnIndex(this);
        //        parentGrid.ArrangedCellUIElements.InvalidateCell(cell);
        //        parentGrid.Invalidate(true);
        //    }
        //}

        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            return new GridControlAutomationPeer(this);
        }

        protected internal override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key != Key.Delete)
            {
                return base.ShouldGridTryToHandlePreviewKeyDown(e);
            }

            return false;
        }
    }

}

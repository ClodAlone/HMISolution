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
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Scroll
{
    /// <summary>
    /// ScrollAxisControl provides support for scrolling through rows and columns. The
    /// logic to map from row or column index to scroll position and back is implemented
    /// in the <see cref="ScrollAxisBase"/> which is accessed through the <see cref="ScrollRows"/>
    /// and <see cref="ScrollColumns"/> property.
    /// <para/>
    /// You can assign a collection that implements <see cref="ILineSizeHost"/> and manages row heights
    /// or column widths to the <see cref="RowHeightsProvider"/> and <see cref="ColumnWidthsProvider"/>.
    /// <see cref="ScrollAxisBase"/> will use information from these objects to map from row or 
    /// column index to scroll position and back map from row or column index to scroll position and back.
    /// <para/>
    /// Rows and Columns can have varying size and they can be hidden. They can be frozen at the 
    /// top, bottom, left and right side. Both pixel scrolling and non-pixel scrolling is supported.
    /// <para/>
    /// ScrollBar logic and child frame logic is implemented in the base <see cref="ScrollControl"/>
    /// class.
    /// </summary>
    public abstract class ScrollAxisControl : ScrollControl
    {
        ILineSizeHost rowHeights = EmptyLineSizeHost.Empty;
        ILineSizeHost columnWidths = EmptyLineSizeHost.Empty;
        ScrollAxisBase _scrollRows;
        ScrollAxisBase _scrollColumns;
        bool verticalPixelScroll = true;
        bool horizontalPixelScroll = true;

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollAxisControl"/> class.
        /// </summary>
        public ScrollAxisControl()
        {
        }

        #endregion

        #region Scroll Axis

        /// <summary>
        /// Gets the scroll axis with logic to map from row index to scroll position and back.
        /// </summary>
        /// <value>The scroll axis for rows.</value>
        public ScrollAxisBase ScrollRows
        {
            get
            {
                if (!ignoreVScrollBarEvents && _scrollRows == null)
                {
                    _scrollRows = CreateScrollAxis(Orientation.Vertical, verticalPixelScroll, VScrollBar, rowHeights);
                    _scrollRows.Changed += new System.EventHandler(_scrollRows_Changed);
                    _scrollRows.Name = "ScrollRows";
                }
                return this._scrollRows;
            }
        }

        void ResetScrollRows()
        {
            if (_scrollRows != null)
            {
                _scrollRows.Changed -= new System.EventHandler(_scrollRows_Changed);
                _scrollRows.Dispose();
            }
            _scrollRows = null;
        }

        void _scrollRows_Changed(object sender, System.EventArgs e)
        {
            if (IsInArrangeOverride)
                return;
            // SH 4/6/10: Let derived control decide whether InvalidateVisual needs to be called when
            // scrollbar changes.
            // InvalidateVisual(true);
            OnScrollLayoutChanged();
            //if (ScrollOwner != null)
            //    ScrollOwner.InvalidateScrollInfo();
            delayInvalidateScrollInfo = true;
        }

        /// <summary>
        /// Called when settings of <see cref="ScrollAxisControl.ScrollRows"/> or 
        /// <see cref="ScrollAxisControl.ScrollColumns"/> were changed.
        /// </summary>
        protected virtual void OnScrollLayoutChanged()
        {
        }

        /// <summary>
        /// Gets the scroll axis with logic to map from column index to scroll position and back.
        /// </summary>
        /// <value>The scroll axis for columns.</value>
        public ScrollAxisBase ScrollColumns
        {
            get
            {
                if (!ignoreHScrollBarEvents && _scrollColumns == null)
                {
                    _scrollColumns = CreateScrollAxis(Orientation.Horizontal, horizontalPixelScroll, HScrollBar, columnWidths);
                    _scrollColumns.Name = "ScrollColumns";
                    _scrollColumns.Changed += new System.EventHandler(_scrollColumns_Changed);
                }
               
                return this._scrollColumns;
            }
        }

        void ResetScrollColumns()
        {
            if (_scrollColumns != null)
            {
                _scrollColumns.Changed -= new System.EventHandler(_scrollColumns_Changed);
                _scrollColumns.Dispose();
            }
            _scrollColumns = null;
        }

        void _scrollColumns_Changed(object sender, System.EventArgs e)
        {
            if (IsInArrangeOverride)
                return;
            InvalidateVisual(true);
            OnScrollLayoutChanged();         
            if (ScrollOwner != null)
                ScrollOwner.InvalidateScrollInfo();
        }     

        /// <summary>
        /// Gets or sets a value indicating whether horizontal pixel scroll is enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if horizontal pixel scrolling is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool HorizontalPixelScroll
        {
            get
            {
                return ScrollColumns.IsPixelScroll;
            }
            set
            {
                if (HorizontalPixelScroll != value)
                {
                    horizontalPixelScroll = value;
                    ResetScrollColumns();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether vertical pixel scroll is enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if vertical pixel scrolling is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool VerticalPixelScroll
        {
            get
            {
                return ScrollRows.IsPixelScroll;
            }
            set
            {
                if (VerticalPixelScroll != value)
                {
                    verticalPixelScroll = value;
                    ResetScrollRows();
                }
            }
        }

        private bool allowPixelScrollPadding = true;
        /// <summary>
        /// Gets or sets a value indicating whether [allow pixel scroll padding]. Set this to false if you don't want the pixel scroll to take a padding value of 1.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [allow pixel scroll padding]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowPixelScrollPadding
        {
            get { return this.allowPixelScrollPadding; }
            set
            {
                if (this.allowPixelScrollPadding != value)
                {
                    this.allowPixelScrollPadding = value;
                    this.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Creates the row or column scroll axis. The default implementation of this method creates either a
        /// <see cref="PixelScrollAxis"/> or <see cref="LineScrollAxis"/> object. You can override this method
        /// if you want to add support for another custom tailored scroll axis object.
        /// </summary>
        /// <param name="orientation">The orientation (Vertical for row scrolling, Horizontal for column scrolling)</param>
        /// <param name="pixelScroll">if set to <c>true</c> pixel scroll; otherwise line scrolling.</param>
        /// <param name="scrollBar">The state of the scroll bar.</param>
        /// <param name="lineSizes">An object that provides row or column sizes.</param>
        /// <returns>The scroll axis object.</returns>
        protected virtual ScrollAxisBase CreateScrollAxis(Orientation orientation, bool pixelScroll, IScrollBar scrollBar, ILineSizeHost lineSizes)
        {
            if (pixelScroll)
                return new PixelScrollAxis(scrollBar, lineSizes, lineSizes as IDistancesHost);
            else
                return new LineScrollAxis(scrollBar, lineSizes);
        }
        #endregion

        protected virtual bool CanAutoCalculateWidth()
        {
            return false;
        }

        #region Measure
        protected override Size MeasureOverride(Size constraint)
        {
            if (ScrollOwner != null && ScrollOwner.CanContentScroll)
            {
                if (!IsDoubleValueSet(FrameworkElement.HeightProperty))
                {
                    if (ScrollRows is PixelScrollAxis)
                    {
                        if (this.AllowPixelScrollPadding)
                            constraint.Height = Math.Min(constraint.Height, ((PixelScrollAxis)ScrollRows).TotalExtent + 1);
                        else
                            constraint.Height = Math.Min(constraint.Height, ((PixelScrollAxis)ScrollRows).TotalExtent);
                    }
                }

                if (!IsDoubleValueSet(FrameworkElement.WidthProperty))
                {
                    var isInfinite = (constraint.Width == double.PositiveInfinity || constraint.Width == double.NegativeInfinity);
                    if ((ScrollColumns is PixelScrollAxis) && (!CanAutoCalculateWidth() || isInfinite))
                    {
                        if (this.AllowPixelScrollPadding)
                            constraint.Width = Math.Min(constraint.Width, ((PixelScrollAxis)ScrollColumns).TotalExtent + 1);
                        else
                            constraint.Width = Math.Min(constraint.Width, ((PixelScrollAxis)ScrollColumns).TotalExtent);
                    }
                }
            }
            else
            {
                if (!IsDoubleValueSet(FrameworkElement.HeightProperty))
                {
                    // Grid is shown in ViewBox or another panel - does no scrolling by itsself.
                    if (ScrollRows is PixelScrollAxis)
                        constraint.Height = ((PixelScrollAxis)ScrollRows).TotalExtent + 1;
                }

                if (!IsDoubleValueSet(FrameworkElement.WidthProperty))
                {
                    var isInfinite = (constraint.Width == double.PositiveInfinity || constraint.Width == double.NegativeInfinity);
                    if ((ScrollColumns is PixelScrollAxis) && (!CanAutoCalculateWidth() || isInfinite))
                            constraint.Width = ((PixelScrollAxis)ScrollColumns).TotalExtent + 1;
                }
            }

            // Call UpdateAxis in Measure to make sure it won't change again 
            // during ArrangeOverride and cause a second layout pass because
            // scrollbar values were changed.
            UpdateAxis(constraint);

            return base.MeasureOverride(constraint);
        }

        bool IsDoubleValueSet(DependencyProperty dp)
        {
            object value = GetValue(dp);
            return value != DependencyProperty.UnsetValue && !double.IsNaN((double) value);
        }
        #endregion

        #region Scroll Position: TopRowIndex, LeftColumnIndex, LineUp, etc.

        /// <summary>
        /// Gets or sets the index of the top row.
        /// </summary>
        /// <value>The index of the top row.</value>
        public int TopRowIndex
        {
            get
            {
                return ScrollRows.ScrollLineIndex;
            }
            set
            {
                ScrollRows.ScrollLineIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the left column.
        /// </summary>
        /// <value>The index of the left column.</value>
        public int LeftColumnIndex
        {
            get
            {
                return ScrollColumns.ScrollLineIndex;
            }
            set
            {
                ScrollColumns.ScrollLineIndex = value;
            }
        }

        /// <summary>
        /// Scrolls down one row.
        /// </summary>
        public override void LineDown()
        {
            ScrollRows.ScrollToNextLine();
        }

        /// <summary>
        /// Scrolls up one row.
        /// </summary>
        public override void LineUp()
        {
            ScrollRows.ScrollToPreviousLine();
        }

        /// <summary>
        /// Scrolls down one page of rows.
        /// </summary>
        public override void PageDown()
        {
            ScrollRows.ScrollToNextPage();
        }

        /// <summary>
        /// Scrolls up one page of rows.
        /// </summary>
        public override void PageUp()
        {
            ScrollRows.ScrollToPreviousPage();
        }

        /// <summary>
        /// Scrolls right one column.
        /// </summary>
        public override void LineRight()
        {
            ScrollColumns.ScrollToNextLine();
        }

        /// <summary>
        /// Scrolls left one left column.
        /// </summary>
        public override void LineLeft()
        {
            ScrollColumns.ScrollToPreviousLine();
        }

        /// <summary>
        /// Scrolls left one page of columns.
        /// </summary>
        public override void PageLeft()
        {
            ScrollColumns.ScrollToPreviousPage();
        }

        /// <summary>
        /// Scrolls right one page of columns.
        /// </summary>
        public override void PageRight()
        {
            ScrollColumns.ScrollToNextPage();
        }

        /// <summary>
        /// Scrolls to top.
        /// </summary>
        public virtual void ScrollToTop()
        {
            VScrollBar.Value = VScrollBar.Minimum;
        }

        /// <summary>
        /// Scrolls to left end.
        /// </summary>
        public virtual void ScrollToLeftEnd()
        {
            HScrollBar.Value = HScrollBar.Minimum;
        }

        /// <summary>
        /// Scrolls to bottom.
        /// </summary>
        public virtual void ScrollToBottom()
        {
            VScrollBar.Value = VScrollBar.Maximum - VScrollBar.LargeChange;
        }

        /// <summary>
        /// Scrolls to right end.
        /// </summary>
        public virtual void ScrollToRightEnd()
        {
            HScrollBar.Value = HScrollBar.Maximum - HScrollBar.LargeChange;
        }

        #endregion

        #region MouseWheel
        /// <summary>
        /// This is the method that responds to the MouseWheel event.
        /// </summary>
        /// <param name="e">Event Arguments</param> 
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Handled) { return; }

            // Only handle when no modifier key or shift is pressed.
            if ((Keyboard.Modifiers & ~ModifierKeys.Shift) == 0)
            {
                bool fShiftDown = ((Keyboard.Modifiers & ModifierKeys.Shift) != 0);
                if (fShiftDown && HScrollBar.Enabled)
                    ScrollColumns.MouseWheel(e.Delta);
                else
                    ScrollRows.MouseWheel(e.Delta);

                e.Handled = true;
            }
        }
        #endregion

        #region Synchronize Axis with RenderSize

        /// <summary>
        /// Arranges all child frames. Each frames <see cref="Canvas.LeftProperty"/>, <see cref="Canvas.TopProperty"/>, <see cref="Canvas.RightProperty"/> and <see cref="Canvas.BottomProperty"/> properties are initialized. <see cref="UIElement.Arrange"/> is called and a the <see cref="UIElement.Clip"/> property is set. After all child frames were arranged the virtual <see cref="ScrollControl.OnArrangeContent"/> method is called.
        /// <para/>
        /// 	<see cref="ScrollAxisControl"/> overrides this method to update the scroll axis with controls dimensions given by arrangeSize parameter.
        /// </summary>
        /// <param name="arrangeSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <param name="isArrangeDirty">if set to <c>true</c> the control will call OnArrangeContent in the next arrange layout pass (i.e. when ArrangeOverride is called).</param>
        /// <returns>Same size as given in arrangeSize.</returns>
        protected override Size OnArrangeOverride(Size arrangeSize, ref bool isArrangeDirty)
        {	
            // REVIEW: OnArrangeOverride
            UpdateAxis(arrangeSize);
#if SILVERLIGHT
            if (isArrangeDirty && ScrollOwner != null)
                ScrollOwner.InvalidateScrollInfo();
#endif

            return base.OnArrangeOverride(arrangeSize, ref isArrangeDirty);
        }

        /// <summary>
        /// Calls the virtuals <see cref="ScrollControl.OnArrangeContent"/> method and raises the <see cref="System.Windows.FrameworkElement.SizeChanged"/> event, using the specified information as part of the eventual event data
        /// <para/>
        /// <see cref="ScrollAxisControl"/> overrides this method to update the scroll axis with controls dimensions given by sizeInfo parameter.
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            UpdateAxis(sizeInfo.NewSize);

            base.OnRenderSizeChanged(sizeInfo);
        }

        /// <summary>
        /// Updates the row and column axis with this control size and initializes Clip position.
        /// </summary>
        /// <param name="arrangeSize">Size of the arrange.</param>
        public void UpdateAxis(Size arrangeSize)
        {
            // Update axis with window size
            //System.Console.WriteLine(this.Tag);
            ScrollRows.RenderSize = arrangeSize.Height;
            ScrollColumns.RenderSize = arrangeSize.Width;

            if (Clip is RectangleGeometry)
            {
                RectangleGeometry rg = (RectangleGeometry)Clip;
                Rect rect = rg.Bounds;
                ScrollRows.Clip = new DoubleSpan(rect.Top, rect.Bottom);
                ScrollColumns.Clip = new DoubleSpan(rect.Left, rect.Right);
            }
            else
            {
                ScrollRows.Clip = DoubleSpan.Empty;
                ScrollColumns.Clip = DoubleSpan.Empty;
            }
            
            // Update Start of Header and Footer area based on axis suggestion. 
            Point corner = new Point(ScrollColumns.ViewCorner, ScrollRows.ViewCorner);
            TopLeftFrameExtent = new Size(ScrollColumns.HeaderExtent, ScrollRows.HeaderExtent);
            BottomRightFrameExtent = new Size(arrangeSize.Width - corner.X, arrangeSize.Height - corner.Y);
        }

        #endregion

        #region RowHeights, ColumnWidths
        /// <summary>
        /// Gets or sets the row heights provider.
        /// </summary>
        /// <value>The row heights provider.</value>
        public ILineSizeHost RowHeightsProvider
        {
            get { return rowHeights; }
            set
            {
                if (value == null)
                    value = EmptyLineSizeHost.Empty;

                if (rowHeights != value)
                {
                    rowHeights = value;
                    ResetScrollRows();
                }
            }
        }

        /// <summary>
        /// Gets or sets the column widths provider.
        /// </summary>
        /// <value>The column widths provider.</value>
        public ILineSizeHost ColumnWidthsProvider
        {
            get { return columnWidths; }
            set
            {
                if (value == null)
                    value = EmptyLineSizeHost.Empty;
                
                if (columnWidths != value)
                {
                    columnWidths = value;
                    ResetScrollColumns();
                }
            }
        }
        #endregion

        #region PointToCellRowColumnIndex
        /// <summary>
        /// Determines the cell under the mouse location.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The cells row and column index under the mouse location.</returns>
        public RowColumnIndex PointToCellRowColumnIndex(MouseEventArgs e)
        {
            return PointToCellRowColumnIndex(e.GetPosition(this));
        }

        /// <summary>
        /// Determines the cell under a given point.
        /// </summary>
        /// <param name="p">The point in client coordinates.</param>
        /// <param name="allowOutsideLines">Set this true if point can be below corner of last line.</param>
        /// <returns>The cells row and column index under the point.</returns>
        public RowColumnIndex PointToCellRowColumnIndexOutsideCells(Point p, bool allowOutsideLines)
        {
            if (!allowOutsideLines && (p.X > ScrollColumns.ViewSize || p.Y > ScrollRows.ViewSize))
                return RowColumnIndex.Empty;

            int rowIndex = ScrollRows.VisiblePointToLineIndex(p.Y, allowOutsideLines);
            int columnIndex = ScrollColumns.VisiblePointToLineIndex(p.X, allowOutsideLines);

            if (rowIndex < 0 || columnIndex < 0)
                return RowColumnIndex.Empty;

            return new RowColumnIndex(rowIndex, columnIndex);
        }

        /// <summary>
        /// Determines the cell under a given point.
        /// </summary>
        /// <param name="p">The point in client coordinates.</param>
        /// <returns>The cells row and column index under the point.</returns>
        public RowColumnIndex PointToCellRowColumnIndex(Point p)
        {
            return PointToCellRowColumnIndexOutsideCells(p, true);
        }
        #endregion

        #region Helper
        /// <summary>
        /// Gets the clipping bounds for the specified row and column region.
        /// </summary>
        /// <param name="rowRegion">The row region.</param>
        /// <param name="columnRegion">The column region.</param>
        /// <returns>A <see cref="Rect"/> with clipping bounds.</returns>
        public Rect GetClipRect(ScrollAxisRegion rowRegion, ScrollAxisRegion columnRegion)
        {
            DoubleSpan ySpan = ScrollRows.GetClipPoints(rowRegion);
            DoubleSpan xSpan = ScrollColumns.GetClipPoints(columnRegion);

            if (ySpan.IsEmpty || xSpan.IsEmpty)
                return Rect.Empty;

            return new Rect(xSpan.Start, ySpan.Start, xSpan.Length, ySpan.Length);
        }

        /// <summary>
        /// Determines whether the row with the specified row index is visible.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns>
        /// 	<c>true</c> if row is visible; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRowVisible(int rowIndex)
        {
            return ScrollRows.IsLineVisible(rowIndex);
        }

        /// <summary>
        /// Determines whether the column with the specified column index is visible.
        /// </summary>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns>
        /// 	<c>true</c> if column is visible; otherwise, <c>false</c>.
        /// </returns>
        public bool IsColumnVisible(int columnIndex)
        {
            return ScrollColumns.IsLineVisible(columnIndex);
        }

        /// <summary>
        /// Determines whether the specified cell is visible.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <returns>
        /// 	<c>true</c> if the specified cell is visible; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCellVisible(RowColumnIndex cellRowColumnIndex)
        {
            return IsRowVisible(cellRowColumnIndex.RowIndex)
                && IsColumnVisible(cellRowColumnIndex.ColumnIndex);
        }

        #endregion 

        /// <summary>
        /// Called when the <see cref="ScrollControl.InvalidateVisual(System.Boolean)"/> method was called.
        /// </summary>
        /// <param name="isArrangeDirty">if set to <c>true</c> indicates that <see cref="ScrollControl.OnArrangeContent"/>
        /// will be called when control gets updated. Otherwise the OnArrangeContent will be skipped
        /// and only OnRender will be called.</param>
        protected override void OnInvalidated(bool isArrangeDirty)
        {
            if (isArrangeDirty)
            {
                if (_scrollRows != null)
                    _scrollRows.MarkDirty();
                if (_scrollColumns != null)
                    _scrollColumns.MarkDirty();
            }
            base.OnInvalidated(isArrangeDirty);
        }

        public override void Dispose(bool disposing)
        {
            this.rowHeights.Dispose();
            this.columnWidths.Dispose();
            this.ignoreHScrollBarEvents = true;
            if (ScrollColumns != null)
                ScrollColumns.Dispose(true);
            this.ignoreHScrollBarEvents = false;
            this.ignoreVScrollBarEvents = true;
            if (ScrollRows != null)
                ScrollRows.Dispose(true);
            this.ignoreVScrollBarEvents = false;
            RowHeightsProvider.Dispose();
            ColumnWidthsProvider.Dispose();
            base.Dispose(disposing);
            _scrollRows = null;
            _scrollColumns = null;
        }
    }

}

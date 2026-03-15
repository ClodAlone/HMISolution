#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.UI.Xaml.Utility;
using System;
using System.Linq;
#if WinRT
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#else
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Input;
#if WP
using System.Windows.Input;
#if !WP7
using Windows.Devices.Input;
using System.Threading.Tasks;
#endif
#endif
#endif

namespace Syncfusion.UI.Xaml.Grid
{
#if WinRT
    using Key = Windows.System.VirtualKey;
    using KeyEventArgs = KeyRoutedEventArgs;
#endif
    [ClassReference(IsReviewed = false)]
    public class VisualContainer : Panel, IScrollableInfo, IDisposable
    {
        #region Fields
        ScrollInfo _hScrollBar;
        internal double previousArrangeWidth;
        ScrollInfo _vScrollBar;
        ScrollViewer _scrollOwner;
        IPaddedEditableLineSizeHost rowHeightsProvider;
        IPaddedEditableLineSizeHost columnWidthsProvider;
        ScrollAxisBase _scrollRows;
        ScrollAxisBase _scrollColumns;
        bool verticalPixelScroll = true;
        bool horizontalPixelScroll = true;
        internal Brush DragBorderBrush;
        internal Thickness DragBorderThickness;
        Size PreviousAvailableSize = Size.Empty;
        Line horizontalLine = new Line();
        Line verticalLine = new Line();

        internal bool NeedToRefreshColumn;
        internal bool SuspendManipulationScroll;

#if !WPF
        internal Func<KeyEventArgs, bool> ContainerKeydown;
#endif

#if WP
#if !WP7
        private double prevZoomScale = 1;
        private int zoomScrollRowIndex = -1;
        private int zoomScrollColumnIndex = -1;
        private double initialHLineOrigin = -1;
        private bool HoffsetIncrement = false;
        Point midPoint = new Point();
        private double thumbWidth = 45;
        internal Action<double> SetZoomScale;
#endif
        private Matrix _transformation;
        internal double _ZoomScale = 1;
        ScaleTransform scaleTransform = new ScaleTransform() { ScaleX = 1.0, ScaleY = 1.0 };
        private MatrixTransform _matrixTransform;
        private Point _cumulativeTranslation;
        private bool _dragStarted;
        private const double PreFeedbackTranslationX = 50d;
        private const double PreFeedbackTranslationY = 50d;
        private double _initialThreshold = 16.0;
        private DateTime _lastTimeStamp;
        private Point _velocity;
        private PanningInfo _panningInfo;
#elif WinRT
        internal Size ViewPortSize;		
#endif

        #endregion

        #region Property

        public IRowGenerator RowsGenerator { get; set; }

        private double verticalPadding;
        public double VerticalPadding
        {
            get
            {
                return verticalPadding;
            }
            set
            {
                if (!SuspendManipulationScroll)
                    verticalPadding = value;
            }
        }

        private double horizontalPadding;
        public double HorizontalPadding
        {
            get
            {
                return horizontalPadding;
            }
            set
            {
                if (!SuspendManipulationScroll)
                    horizontalPadding = value;
            }
        }

        public ScrollInfo HScrollBar
        {
            get { return _hScrollBar ?? (_hScrollBar = new ScrollInfo()); }
        }

        public ScrollInfo VScrollBar
        {
            get { return _vScrollBar ?? (_vScrollBar = new ScrollInfo()); }
        }

        public IPaddedEditableLineSizeHost RowHeights
        {
            get
            {
                return this.rowHeightsProvider;
            }
        }

        public IPaddedEditableLineSizeHost ColumnWidths
        {
            get
            {
                return columnWidthsProvider;
            }
        }

        public ScrollAxisBase ScrollRows
        {
            get
            {
                if (_scrollRows == null)
                {
                    _scrollRows = CreateScrollAxis(Orientation.Vertical, verticalPixelScroll, VScrollBar, RowHeights);
                    _scrollRows.Name = "ScrollRows";
                }
                return this._scrollRows;
            }
        }

        public ScrollAxisBase ScrollColumns
        {
            get
            {
                if (_scrollColumns == null)
                {
                    _scrollColumns = CreateScrollAxis(Orientation.Horizontal, horizontalPixelScroll, HScrollBar, ColumnWidths);
                    _scrollColumns.Name = "ScrollColumns";
                    _scrollColumns.Changed += OnScrollColumnsChanged;
                }

                return this._scrollColumns;
            }
        }

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

        public int RowCount
        {
            get
            {
                return this.rowHeightsProvider.LineCount;
            }
            set
            {
                if (value > RowCount)
                    InsertRows(RowCount, value - RowCount);
                else if (value < RowCount)
                    RemoveRows(value, RowCount - value);
            }
        }

        public int ColumnCount
        {
            get
            {
                return this.columnWidthsProvider.LineCount;
            }
            set
            {
                if (value > ColumnCount)
                    InsertColumns(ColumnCount, value - ColumnCount);
                else if (value < ColumnCount)
                    RemoveColumns(value, ColumnCount - value);
            }
        }

        public int FrozenRows
        {
            get
            {
                return this.rowHeightsProvider.HeaderLineCount;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Negative values not allowed.");
                this.rowHeightsProvider.HeaderLineCount = value;
            }
        }

        public int FooterRows
        {
            get
            {
                return this.rowHeightsProvider.FooterLineCount;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Negative Values not Allowed.");
                this.rowHeightsProvider.FooterLineCount = value;
            }
        }

        public int FrozenColumns
        {
            get
            {
                return this.columnWidthsProvider.HeaderLineCount;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Negative values are not allowed.");
                this.columnWidthsProvider.HeaderLineCount = value;
            }
        }

        public int FooterColumns
        {
            get
            {
                return this.columnWidthsProvider.FooterLineCount;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Negative values are not allowed.");
                this.columnWidthsProvider.FooterLineCount = value;
            }
        }

        public bool AllowFixedGroupCaptions { get; set; }

        #endregion

        #region Dependency Properties
#if WinRT
        /// <summary>
        /// Gets or sets the vertical offset.
        /// </summary>
        /// <value>
        /// The vertical offset.
        /// </value>
        public double VerticalScrollBarOffset
        {
            get { return (double)GetValue(VerticalScrollBarOffsetProperty); }
            set { SetValue(VerticalScrollBarOffsetProperty, value); }
        }

        /// <summary>
        /// The vertical offset property
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarOffsetProperty =
            DependencyProperty.Register("VerticalScrollBarOffset", typeof(double), typeof(VisualContainer), new PropertyMetadata(double.NegativeInfinity,
            (o, args) =>
            {
                var visualContainer = o as VisualContainer;
                if (visualContainer != null)
                    visualContainer.SetVerticalOffset((double)args.NewValue);
            }));

        /// <summary>
        /// Gets or sets the hortizontal offset.
        /// </summary>
        /// <value>
        /// The hortizontal offset.
        /// </value>
        public double HortizontalScrollBarOffset
        {
            get { return (double)GetValue(HortizontalScrollBarOffsetProperty); }
            set { SetValue(HortizontalScrollBarOffsetProperty, value); }
        }

        /// <summary>
        /// The hortizontal offset property
        /// </summary>
        public static readonly DependencyProperty HortizontalScrollBarOffsetProperty =
            DependencyProperty.Register("HortizontalScrollBarOffset", typeof(double), typeof(VisualContainer), new PropertyMetadata(double.NegativeInfinity,
                (o, args) =>
                {
                    var visualContainer = o as VisualContainer;
                    if (visualContainer != null)
                        visualContainer.SetHorizontalOffset((double)args.NewValue);
                }));
#endif

        
#if WP
        internal double ZoomScale
        {
            get { return _ZoomScale; }
            set
            {
                _ZoomScale = value;
                ApplyLayoutTransform();
            }
        }

        public double ScrollableHeight
        {
            get { return Math.Max((double)0.0, (double)(this.ExtentHeight - this.ViewportHeight)); }
            internal set { base.SetValue(ScrollableHeightProperty, value); }
        }

        public double ScrollableWidth
        {
            get { return Math.Max((double)0.0, (double)(this.ExtentWidth - this.ViewportWidth)); }
            internal set { base.SetValue(ScrollableWidthProperty, value); }
        }

        public static readonly DependencyProperty ScrollableHeightProperty =
            DependencyProperty.Register("ScrollableHeight", typeof(double), typeof(ScrollViewer), null);

        public static readonly DependencyProperty ScrollableWidthProperty =
            DependencyProperty.Register("ScrollableWidth", typeof(double), typeof(ScrollViewer), null);
#endif
        #endregion

        #region Ctor
        public VisualContainer()
        {
            RowsGenerator = null;
            this.rowHeightsProvider = OnCreateRowHeights();
            this.columnWidthsProvider = OnCreateColumnWidths();
#if WinRT
            this.rowHeightsProvider.DefaultLineSize = 45;
            this.columnWidthsProvider.DefaultLineSize = 120;
#elif WP
            _matrixTransform = new MatrixTransform();
            RenderTransform = _matrixTransform;
            this.rowHeightsProvider.DefaultLineSize = 75;
            this.columnWidthsProvider.DefaultLineSize = 180;
#else
            this.rowHeightsProvider.DefaultLineSize = 24;
            this.columnWidthsProvider.DefaultLineSize = 150;
#endif

            this.Children.Add(horizontalLine);
            this.Children.Add(verticalLine);
            WireScrollLineEvents();
#if !WPF
            WireEvents();
#endif
        }
        #endregion

        #region Override methods

        #region MeasureOverride

        bool IsDoubleValueSet(DependencyProperty dp)
        {
            object value = GetValue(dp);
            return value != DependencyProperty.UnsetValue && !double.IsNaN((double)value);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (RowsGenerator == null)
                return constraint;
            if (ScrollOwner != null && (double.IsInfinity(constraint.Width) || double.IsInfinity(constraint.Height)))
            {
                if (!IsDoubleValueSet(FrameworkElement.HeightProperty) && double.IsInfinity(constraint.Height))
                {
                    if (ScrollRows is PixelScrollAxis)
                        constraint.Height = Math.Min(constraint.Height, ((PixelScrollAxis) ScrollRows).TotalExtent - RowHeights.PaddingDistance);
                }

                if (!IsDoubleValueSet(FrameworkElement.WidthProperty) && double.IsInfinity(constraint.Width))
                {
                    if (ScrollColumns is PixelScrollAxis)
                        constraint.Width = Math.Min(constraint.Width, ((PixelScrollAxis)ScrollColumns).TotalExtent);
                }
            }
#if WinRT
            Size availableSize = (ScrollOwner != null && !double.IsInfinity(ViewPortSize.Height) && !double.IsInfinity(ViewPortSize.Width)) ? ViewPortSize : constraint;
#endif

#if WP
            if (ZoomScale != 1)
            {
                var size = ComputeLargestTransformedSize(constraint);
                UpdateAxis(size);
            }
            else
#endif
            {
                
#if WinRT
                UpdateAxis(availableSize);
#else
                UpdateAxis(constraint);
#endif
            }

            PreGenerateItems();

#if WinRT
            if ((!PreviousAvailableSize.IsEmpty && PreviousAvailableSize != availableSize) || NeedToRefreshColumn)
#else
            if ((!PreviousAvailableSize.IsEmpty && PreviousAvailableSize != constraint) || NeedToRefreshColumn)
#endif
            {
                EnsureItems(true);
                NeedToRefreshColumn = false;
            }
            else
                EnsureItems(false);
            InvalidatScrollInfo();
#if WinRT
            PreviousAvailableSize = availableSize;
            horizontalLine.Measure(availableSize);
            verticalLine.Measure(availableSize);
#else
            PreviousAvailableSize = constraint;
            horizontalLine.Measure(constraint);
            verticalLine.Measure(constraint);
#endif
            MeasureRows();
            return constraint;
        }

        #endregion

        #region ArrangeOverride

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (RowsGenerator == null)
                return finalSize;
#if WinRT
            Size availableSize;
            availableSize = (ScrollOwner != null && !double.IsInfinity(ViewPortSize.Height) && !double.IsInfinity(ViewPortSize.Width)) ? ViewPortSize : finalSize;
            if (this.ColumnCount > 0 && (previousArrangeWidth == 0 || previousArrangeWidth != availableSize.Width))
            {
                this.RowsGenerator.ApplyColumnSizeronInitial(availableSize.Width);
                previousArrangeWidth = availableSize.Width;
            }
#else
            if (this.ColumnCount > 0 && (previousArrangeWidth == 0))
            {
                this.RowsGenerator.ApplyColumnSizeronInitial(finalSize.Width);
                previousArrangeWidth = finalSize.Width;
            }
#endif
            ArrangeRow();
            this.RowsGenerator.RowsArranged(finalSize);
            return finalSize;
        }

        #endregion

#if WPF
        #region Manipulation Boundry Feedback
        /// <summary>
        /// Called when the <see cref="E:System.Windows.UIElement.ManipulationBoundaryFeedback" /> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnManipulationBoundaryFeedback(System.Windows.Input.ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
        #endregion
#endif

        #endregion

#if !WPF
        #region Event Handlers

        /// <summary>
        /// Wires the events.
        /// </summary>
        private void WireEvents()
        {
            this.KeyDown += OnContainerKeyDown;
#if WinRT
            this.ManipulationDelta += OnContainerOnManipulationDelta;
            this.PointerWheelChanged += OnContainerPointerWheelChanged;
#endif
        }

        /// <summary>
        /// UnWires the scroll viewer events.
        /// </summary>
        private void UnWireEvents()
        {
            this.KeyDown -= OnContainerKeyDown;
#if WinRT
            this.ManipulationDelta -= OnContainerOnManipulationDelta;
            this.PointerWheelChanged -= OnContainerPointerWheelChanged;
            if (_scrollOwner != null)
                _scrollOwner.Loaded -= OnScrollOwnerLoaded;

            var verticalScrollBar = GridUtil.FindDescendantByName(ScrollOwner, "VerticalScrollBar");
            if (verticalScrollBar != null)
                verticalScrollBar.PointerWheelChanged -= OnContainerPointerWheelChanged;
            var scrollBarSeparator = GridUtil.FindDescendantByName(ScrollOwner, "ScrollBarSeparator");
            if (scrollBarSeparator != null)
                scrollBarSeparator.PointerWheelChanged -= OnContainerPointerWheelChanged;
#endif
        }

        /// <summary>
        /// Containers the key down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyRoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnContainerKeyDown(object sender, KeyEventArgs e)
        {
            ContainerKeydown(e);
        }

#if WinRT
        /// <summary>
        /// Containers the on manipulation delta.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ManipulationDeltaRoutedEventArgs"/> instance containing the event data.</param>
        private void OnContainerOnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if ((e.OriginalSource != this && !(e.OriginalSource is DetailsViewDataGrid)) || e.PointerDeviceType == PointerDeviceType.Mouse || ScrollOwner == null || (ScrollOwner.HorizontalScrollMode == ScrollMode.Disabled && ScrollOwner.VerticalScrollMode == ScrollMode.Disabled))
                return;
            var verticalOffset = e.Delta.Translation.Y;
            var horizontalOffset = e.Delta.Translation.X;
            this.ScrollOwner.ScrollToVerticalOffset(VerticalOffset - verticalOffset);
            this.ScrollOwner.ScrollToHorizontalOffset(HorizontalOffset - horizontalOffset);
            e.Handled = true;
        }
        /// <summary>
        /// Called when [container pointer wheel changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>

        private void OnContainerPointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (ScrollOwner == null || (ScrollOwner.HorizontalScrollMode == ScrollMode.Disabled && ScrollOwner.VerticalScrollMode == ScrollMode.Disabled))
                return;
            var verticalOffset = e.GetCurrentPoint(this).Properties.MouseWheelDelta;
            this.ScrollOwner.ScrollToVerticalOffset(VerticalOffset - verticalOffset);
            e.Handled = true;
        }

        /// <summary>
        /// Called when Scroll owner loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        void OnScrollOwnerLoaded(object sender, RoutedEventArgs e)
        {
            var verticalScrollBar = GridUtil.FindDescendantByName(ScrollOwner, "VerticalScrollBar");
            if (verticalScrollBar != null)
                verticalScrollBar.PointerWheelChanged += OnContainerPointerWheelChanged;
            var scrollBarSeparator = GridUtil.FindDescendantByName(ScrollOwner, "ScrollBarSeparator");
            if (scrollBarSeparator != null)
                scrollBarSeparator.PointerWheelChanged += OnContainerPointerWheelChanged;
        }
#endif
        #endregion
#endif

        #region Virtual methods

        protected virtual IPaddedEditableLineSizeHost OnCreateRowHeights()
        {
            var lineSizeCollection = new LineSizeCollection();
            return lineSizeCollection;
        }

        protected virtual IPaddedEditableLineSizeHost OnCreateColumnWidths()
        {
            var lineSizeCollection = new LineSizeCollection();
            return lineSizeCollection;
        }

        protected virtual ScrollAxisBase CreateScrollAxis(Orientation orientation, bool pixelScroll, IScrollBar scrollBar, ILineSizeHost lineSizes)
        {
            if (pixelScroll)
                return new PixelScrollAxis(scrollBar, lineSizes, lineSizes as IDistancesHost);
            else
                return new LineScrollAxis(scrollBar, lineSizes);
        }

        #endregion

        #region Public methods

        public void InsertRows(int insertAtRowIndex, int count)
        {
            this.rowHeightsProvider.InsertLines(insertAtRowIndex, count, null);
        }

        public void RemoveRows(int removeAtRowIndex, int count)
        {
            this.rowHeightsProvider.RemoveLines(removeAtRowIndex, count, null);
        }

        public void InsertColumns(int insertAtColumnIndex, int count)
        {
            this.columnWidthsProvider.InsertLines(insertAtColumnIndex, count, null);
            this.RowsGenerator.ColumnInserted(insertAtColumnIndex, count);
        }

        public void RemoveColumns(int removeAtColumnIndex, int count)
        {
            this.RowsGenerator.ColumnRemoved(removeAtColumnIndex, count);
            this.columnWidthsProvider.RemoveLines(removeAtColumnIndex, count, null);
        }

        /// <summary>
        /// Determines the cell under the mouse location.
        /// </summary>
        /// <param name="p">The point in client coordinates.</param>
        /// <returns>
        /// The cells row and column index under the mouse location.
        /// </returns>
        public RowColumnIndex PointToCellRowColumnIndex(Point p)
        {
            VisibleLineInfo visibleRow = ScrollRows.GetVisibleLineAtPoint(p.Y);
            VisibleLineInfo visibleColumn = ScrollColumns.GetVisibleLineAtPoint(p.X);

            if (visibleRow == null || visibleColumn == null)
                return RowColumnIndex.Empty;

            return new RowColumnIndex(visibleRow.LineIndex, visibleColumn.LineIndex);
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <param name="rowRegion">Scroll axis region for row.</param>
        /// <param name="columnRegion">Scroll axis region for column.</param>
        /// <param name="range">Cell range.</param>
        /// <param name="allowEstimatesForOutOfViewRows">If set to true, allows estimate for out of view rows.</param>
        /// <param name="allowEstimatesForOutOfViewColumns">If set to true, allows estimate for out of view columns.</param>
        /// <returns>Visible rectangle for the given range.</returns>
        public Rect RangeToRect(ScrollAxisRegion rowRegion, ScrollAxisRegion columnRegion, RowColumnIndex rowcolumn, bool allowEstimatesForOutOfViewRows, bool allowEstimatesForOutOfViewColumns)
        {
            if (rowcolumn.IsEmpty)
                return Rect.Empty;

            DoubleSpan ySpan = ScrollRows.RangeToPoints(rowRegion, rowcolumn.RowIndex, rowcolumn.RowIndex, allowEstimatesForOutOfViewRows);
            DoubleSpan xSpan = ScrollColumns.RangeToPoints(columnRegion, rowcolumn.ColumnIndex, rowcolumn.ColumnIndex, allowEstimatesForOutOfViewColumns);

            if (ySpan.IsEmpty || xSpan.IsEmpty)
                return Rect.Empty;

            return new Rect(xSpan.Start, ySpan.Start, xSpan.Length, ySpan.Length);
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Clearing Children
        /// </summary>
        /// <remarks>
        /// Row Generator items are cleared when itemsSource Changed so Child should be clear
        /// </remarks>
        internal void OnItemSourceChanged()
        {
            previousArrangeWidth = 0.0;
            if (this.Children.Count > 0)
            {
                this.Children.Clear();
            }
            else
                this.InvalidateMeasure();
        }

        internal void SetRowGenerator(RowGenerator rg)
        {
            this.RowsGenerator = rg;
            horizontalLine.Stroke = DragBorderBrush;
            horizontalLine.StrokeThickness = DragBorderThickness.Top;
            verticalLine.Stroke = DragBorderBrush;
            verticalLine.StrokeThickness = DragBorderThickness.Left;
        }

        internal void UpdateScrollBars()
        {
            //While updating the Row Column Count we need to update the scroll bar values. Otherwise visiblelines will be calculated wrongly.
            this.ScrollRows.UpdateScrollBar();
            this.ScrollColumns.UpdateScrollBar();
        }

        #endregion

        #region Private methods

        private void OnScrollColumnsChanged(object sender, Syncfusion.UI.Xaml.ScrollAxis.ScrollChangedEventArgs e)
        {
            if (e.Action == ScrollChangedAction.LineResized)
            {
                this.NeedToRefreshColumn = true;
                this.RowsGenerator.LineSizeChanged();
                this.InvalidateMeasure();
            }
            else
            {
                var visibleColumns = ScrollColumns.GetVisibleLines();
                if (visibleColumns.Count > 0)
                {
                    this.NeedToRefreshColumn = true;
                }
            }
        }

        private void ResetScrollRows()
        {
            if (_scrollRows != null)
            {
                _scrollRows.Dispose();
            }
            _scrollRows = null;
        }

        private void ResetScrollColumns()
        {
            if (_scrollColumns != null)
            {
                _scrollColumns.Dispose();
            }
            _scrollColumns = null;
        }

        public void UpdateAxis(Size availableSize)
        {
            ScrollRows.RenderSize = availableSize.Height;
            ScrollColumns.RenderSize = availableSize.Width;

            if (Clip is RectangleGeometry)
            {
                var rg = Clip;
                Rect rect = rg.Bounds;
                ScrollRows.Clip = new DoubleSpan(rect.Top, rect.Bottom);
                ScrollColumns.Clip = new DoubleSpan(rect.Left, rect.Right);
            }
            else
            {
                ScrollRows.Clip = DoubleSpan.Empty;
                ScrollColumns.Clip = DoubleSpan.Empty;
            }
        }

        private void PreGenerateItems()
        {
            var visibleRows = ScrollRows.GetVisibleLines();
            var visibleColumns = ScrollColumns.GetVisibleLines();
            this.RowsGenerator.PregenerateRows(visibleRows, visibleColumns);
        }

        private void EnsureItems(bool ensureColumns)
        {
            var visibleRows = ScrollRows.GetVisibleLines();
            this.RowsGenerator.EnsureRows(visibleRows);
            if (ensureColumns)
            {
                var visibleColumns = ScrollColumns.GetVisibleLines();
                if (visibleColumns.Count > 0 && visibleColumns.FirstBodyVisibleIndex < visibleColumns.Count)
                    this.RowsGenerator.EnsureColumns(visibleColumns);
            }

            //Here we substracting 2 drag lines from children count
            if (this.RowsGenerator != null && this.RowsGenerator.Items.Count != (this.Children.Count - 2))
            {
                foreach (IElement row in this.RowsGenerator.Items)
                {
                    if (!this.Children.Contains(row.Element))
                    {
                        this.Children.Add(row.Element);
                    }
                }
            }
        }


        private IRowElement GetPreviousFixedRow(IEnumerable<IRowElement> enumerable, IRowElement currentrowelement, Predicate<IRowElement> condition)
        {
            var enumerator = enumerable.GetEnumerator();
            IRowElement nextrowelement = null;
            while (enumerator.MoveNext())
            {
                if (enumerator.Current == currentrowelement)
                    break;
                if (condition(enumerator.Current) && enumerator.Current.Element.Visibility == Visibility.Visible)
                    nextrowelement = enumerator.Current;
            }
            if (nextrowelement != null && (nextrowelement.Element.Clip is RectangleGeometry &&
                                           (nextrowelement.Element.Clip as RectangleGeometry).Rect == Rect.Empty))
            {
                nextrowelement = GetPreviousFixedRow(enumerable, nextrowelement, condition);
            }
            return nextrowelement;
        }

        private void MeasureRows()
        {
            var orderedItems = this.RowsGenerator.Items.OrderBy(row => row.Index);
            foreach (var item in orderedItems)
            {
                if (item.Element.Visibility != Visibility.Visible) continue;
                var line = this.GetRowVisibleLineInfo(item.Index);
                if (line != null)
                    item.MeasureElement(new Size(this.ScrollColumns.ViewSize, line.Size));
            }
        }


        private void ArrangeRow()
        {
            if (this.RowsGenerator == null) return;
            double y = this.ScrollRows.HeaderExtent;
            IRowElement previousFixedRowElement = null;
            var extendedHeaderHeight = this.ScrollRows.HeaderExtent;
            double xPosition, yPosition;
            //In WinRT the Panel(VisualContainer) scrolls with the ScrollBar, hence we are measuring the VerticalOffset in panelDelta to compute the Row's Position
            double panelDelta=0.0;

            var orderedItems = this.RowsGenerator.Items.OrderBy(row => row.Index);
            foreach (var item in orderedItems)
            {
                if (item.Element.Visibility == Visibility.Visible)
                {
                    var line = this.GetRowVisibleLineInfo(item.Index);
                    if (line != null)
                    {
                        xPosition = 0.15 * HorizontalPadding;
                        yPosition = (item.RowRegion != RowRegion.Body) ? line.Origin : line.Origin + (0.15 * VerticalPadding);
#if WinRT
                        if (ScrollOwner != null)
                        {
                            panelDelta = this.VerticalOffset;
                            yPosition += this.VerticalOffset;
                            xPosition += this.HorizontalOffset;
                        }
#endif
                        var rect = new Rect(xPosition, yPosition, this.ScrollColumns.ViewSize, line.Size);
                        if (this.AllowFixedGroupCaptions && item.IsFixedRow && (yPosition - panelDelta) < extendedHeaderHeight && (previousFixedRowElement == null || (previousFixedRowElement.Level < item.Level)))
                        {
                            rect.Y = extendedHeaderHeight + panelDelta;
                        }
                        rect = item.RowManupulation(rect);
                        //item.MeasureElement(rect);//.Element.Measure(new Size(rect.Width, rect.Height));

                        if ((HorizontalPadding != 0 || VerticalPadding != 0) && (item.RowRegion == RowRegion.Body) && (!AllowFixedGroupCaptions || VerticalPadding > 0))
                        {
                            if (rect.Y <= this.ScrollRows.HeaderExtent)
                                item.Element.Clip = new RectangleGeometry { Rect = new Rect(0, this.ScrollRows.HeaderExtent - rect.Y, this.ScrollColumns.ViewSize, line.Size) };
                            else if ((rect.Y + line.Size) > (this.ScrollRows.ViewSize - this.ScrollRows.FooterExtent))
                            {
                                var height = (this.ScrollRows.ViewSize - this.ScrollRows.FooterExtent) - (rect.Y + line.Size);
                                item.Element.Clip = new RectangleGeometry { Rect = new Rect(0, height, this.ScrollColumns.ViewSize, line.Size) };
                            }
                            else
                                item.Element.Clip = null;
                        }
                        else
                        {
                            if (!AllowFixedGroupCaptions)
                            {
                                if (line.IsClippedBody && line.IsClippedOrigin && line.IsClippedCorner)
                                    item.Element.Clip = new RectangleGeometry { Rect = new Rect(0, line.Size - line.ClippedSize - line.ClippedCornerExtent, this.ScrollColumns.ViewSize, line.ClippedSize) };
                                else if (line.IsClippedBody && line.IsClippedCorner)
                                    item.Element.Clip = new RectangleGeometry { Rect = new Rect(0, 0, this.ScrollColumns.ViewSize, line.ClippedSize) };
                                else if (line.IsClippedBody && line.IsClippedOrigin)
                                    item.Element.Clip = new RectangleGeometry { Rect = new Rect(0, line.Size - line.ClippedSize - line.ClippedCornerExtent, this.ScrollColumns.ViewSize, line.Size) };
                                else
                                    item.Element.Clip = null;
                            }
                            else
                            {
                                if (!item.IsFixedRow)
                                {
                                    if (line.IsClippedBody && line.IsClippedCorner)
                                        item.Element.Clip = new RectangleGeometry { Rect = new Rect(0, 0, this.ScrollColumns.ViewSize, line.ClippedSize) };
                                    else if (line.IsClippedBody && line.IsClippedOrigin)
                                        item.Element.Clip = new RectangleGeometry { Rect = new Rect(0, line.Size - line.ClippedSize, this.ScrollColumns.ViewSize, line.Size) };
                                    else
                                        item.Element.Clip = null;

                                    if (item.RowType == RowType.CaptionRow || item.RowType == RowType.CaptionCoveredRow)
                                    {
                                        var currentitem = item;
                                        while (true)
                                        {
                                            var previouselement = GetPreviousFixedRow(orderedItems, currentitem, element => (element.RowType == RowType.CaptionRow || element.RowType == RowType.CaptionCoveredRow));
                                            if (previouselement != null && previouselement.IsFixedRow)
                                            {
                                                if (previouselement.Level < item.Level)
                                                {
                                                    if (item.RowRegion == RowRegion.Body && (extendedHeaderHeight + panelDelta) > rect.Y)
                                                    {
                                                        if (((previouselement.ArrangeRect.Y + previouselement.ArrangeRect.Height) - rect.Y) >= rect.Height)
                                                            item.Element.Clip = new RectangleGeometry()
                                                                {
                                                                    Rect = Rect.Empty
                                                                };
                                                        else
                                                            item.Element.Clip = new RectangleGeometry { Rect = new Rect(0, (previouselement.ArrangeRect.Y + previouselement.ArrangeRect.Height) - rect.Y, this.ScrollColumns.ViewSize, line.Size) };
                                                    }
                                                    break;
                                                }
                                                if (rect.Y <= previouselement.ArrangeRect.Y)
                                                {
                                                    previouselement.Element.Clip = new RectangleGeometry() { Rect = Rect.Empty };
                                                }
                                                else if (previouselement.ArrangeRect.Y + previouselement.ArrangeRect.Height > rect.Y)
                                                {
                                                    previouselement.Element.Clip = new RectangleGeometry() { Rect = new Rect(0, (rect.Y - (previouselement.ArrangeRect.Y + previouselement.ArrangeRect.Height)), this.ScrollColumns.ViewSize, line.Size) };
                                                    this.RowsGenerator.ApplyFixedRowVisualState(item.Index, true);
                                                    break;
                                                }
                                                else
                                                    break;
                                                currentitem = previouselement;
                                            }
                                            else
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        if (item.RowRegion == RowRegion.Body && (extendedHeaderHeight + panelDelta) > rect.Y)
                                            item.Element.Clip = new RectangleGeometry { Rect = new Rect(0, ((extendedHeaderHeight + panelDelta) - rect.Y), this.ScrollColumns.ViewSize, line.Size) };
                                    }
                                }
                                else
                                {
                                    item.ArrangeRect = rect;
                                    var currentitem = item;
                                    var extendedHeightChanged = false;
                                    var hasCliped = false;
                                    while (true)
                                    {
                                        var previouselement = GetPreviousFixedRow(orderedItems, currentitem, element => element.IsFixedRow);
                                        if (previouselement != null)
                                        {
#if !SILVERLIGHT && !WP7
                                            var previouselementy = Math.Round((previouselement.ArrangeRect.Y + previouselement.ArrangeRect.Height), 3, MidpointRounding.AwayFromZero);
                                            var recty = Math.Round(rect.Y, 3, MidpointRounding.AwayFromZero);
#else
                                            var previouselementy = Math.Round((previouselement.ArrangeRect.Y + previouselement.ArrangeRect.Height), 3);
                                            var recty = Math.Round(rect.Y, 3);
#endif
                                            if (previouselementy <= recty)
                                            {
                                                //previouselement.Element.Clip = null;
                                                if (currentitem == item)
                                                    extendedHeaderHeight += rect.Height;
                                                break;
                                            }
                                            if (previouselement.Element.Clip == null)
                                            {
                                                previouselement.Element.Clip = new RectangleGeometry { Rect = new Rect(0, rect.Y - (extendedHeaderHeight + panelDelta), ScrollColumns.ViewSize, line.Size) };
                                                hasCliped = true;
                                            }
                                            if ((extendedHeaderHeight + panelDelta) - rect.Y > line.Size)
                                                extendedHeaderHeight -= line.Size;
                                            else
                                            {
                                                var rectangleGeometry = previouselement.Element.Clip as RectangleGeometry;
                                                if (rectangleGeometry != null && !rectangleGeometry.Rect.IsEmpty)
                                                    extendedHeaderHeight += (line.Size - ((extendedHeaderHeight + panelDelta) - previouselement.ArrangeRect.Y));
                                            }
                                            if (previouselement.Level == item.Level && rect.Y < previouselement.ArrangeRect.Y)
                                            {
                                                rect.Y = previouselement.ArrangeRect.Y;
                                                previouselement.Element.Clip = new RectangleGeometry { Rect = Rect.Empty };
                                                hasCliped = false;
                                                this.RowsGenerator.ApplyFixedRowVisualState(item.Index, false);
                                                extendedHeaderHeight += line.Size;
                                                break;
                                            }
                                            currentitem = previouselement;
                                            extendedHeightChanged = true;
                                        }
                                        else
                                        {
                                            item.Element.Clip = null;
                                            if (!extendedHeightChanged)
                                                extendedHeaderHeight += line.Size;
                                            break;
                                        }
                                    }
                                    if (hasCliped)
                                        this.RowsGenerator.ApplyFixedRowVisualState(item.Index, true);
                                    previousFixedRowElement = item;
                                }
                            }
                        }
                        item.ArrangeElement(rect);//.Element.Arrange(rect);
                        item.ArrangeRect = rect;
                        y += line.Size;
                    }
                    else
                    {
                        if (!AllowFixedGroupCaptions)
                        {
                            var rect = new Rect(0, y, this.ScrollColumns.ViewSize, this.ScrollRows.DefaultLineSize);
                            //item.MeasureElement(rect);//.Element.Measure(new Size(rect.Width, rect.Height));
                            item.ArrangeElement(rect);//.Element.Arrange(rect);
                            y += this.ScrollRows.DefaultLineSize;
                        }
                        else
                        {
                            if (item.IsFixedRow)
                            {
                                xPosition = 0.15 * HorizontalPadding;
                                var height = this.RowHeights[item.Index];
                                var rect = new Rect(xPosition, extendedHeaderHeight+panelDelta, this.ScrollColumns.ViewSize, height);
                                previousFixedRowElement = item;
                                extendedHeaderHeight += height;
                                item.Element.Clip = null;
                                item.ArrangeElement(rect);//.Element.Arrange(rect);
                                item.ArrangeRect = rect;
                            }
                            else
                            {
                                var rect = new Rect(0, y, this.ScrollColumns.ViewSize, this.ScrollRows.DefaultLineSize);
                                //item.MeasureElement(rect);//.Element.Measure(new Size(rect.Width, rect.Height));
                                item.ArrangeElement(rect);//.Element.Arrange(rect);
                                item.ArrangeRect = rect;
                                y += this.ScrollRows.DefaultLineSize;
                            }
                        }
                    }
                }
            }

            if (this.ScrollRows.ViewSize - this.ScrollRows.FooterExtent <= 0 || this.RowHeights.TotalExtent - this.ScrollRows.FooterExtent - this.ScrollRows.HeaderExtent <= 0)
                return;
            var horizontalDragRect = new Rect(HorizontalPadding * 0.15, this.ScrollRows.HeaderExtent + VerticalPadding * 0.15,
                                               this.ColumnWidths.TotalExtent, this.ScrollRows.ViewSize - this.ScrollRows.FooterExtent);
            var verticalDragRect = new Rect(HorizontalPadding * 0.15, this.ScrollRows.HeaderExtent + VerticalPadding * 0.15,
                                             this.ScrollColumns.ViewSize, this.RowHeights.TotalExtent - this.ScrollRows.FooterExtent - this.ScrollRows.HeaderExtent);

            horizontalLine.X2 = this.ScrollColumns.ViewSize;
            verticalLine.Y2 = this.ScrollRows.ViewSize - this.ScrollRows.FooterExtent - this.ScrollRows.HeaderExtent;

            if (VerticalPadding > 0 && HorizontalPadding > 0)
            {
                horizontalLine.Arrange(horizontalDragRect);
                verticalLine.Arrange(verticalDragRect);
                if (horizontalLine.Visibility == Visibility.Collapsed)
                    horizontalLine.Visibility = Visibility.Visible;
                if (verticalLine.Visibility == Visibility.Collapsed)
                    verticalLine.Visibility = Visibility.Visible;
            }
            else if (VerticalPadding > 0 && HorizontalPadding <= 0)
            {
                horizontalLine.Arrange(horizontalDragRect);
                if (horizontalLine.Visibility == Visibility.Collapsed)
                    horizontalLine.Visibility = Visibility.Visible;
                if (verticalLine.Visibility == Visibility.Visible)
                    verticalLine.Visibility = Visibility.Collapsed;
            }
            else if (HorizontalPadding > 0 && VerticalPadding <= 0)
            {
                verticalLine.Arrange(verticalDragRect);
                if (verticalLine.Visibility == Visibility.Collapsed)
                    verticalLine.Visibility = Visibility.Visible;
                if (horizontalLine.Visibility == Visibility.Visible)
                    horizontalLine.Visibility = Visibility.Collapsed;
            }
            else if (VerticalPadding > 0)
            {
                horizontalLine.Arrange(horizontalDragRect);
                if (horizontalLine.Visibility == Visibility.Collapsed)
                    horizontalLine.Visibility = Visibility.Visible;
            }
            else if (HorizontalPadding > 0)
            {
                verticalLine.Arrange(verticalDragRect);
                if (verticalLine.Visibility == Visibility.Collapsed)
                    verticalLine.Visibility = Visibility.Visible;
            }
            else
            {
                if (horizontalLine.Visibility == Visibility.Visible)
                    horizontalLine.Visibility = Visibility.Collapsed;
                if (verticalLine.Visibility == Visibility.Visible)
                    verticalLine.Visibility = Visibility.Collapsed;
            }
        }

        private void InvalidatScrollInfoAndMeasure()
        {
            this.InvalidateMeasure();
            this.InvalidatScrollInfo();
        }

        public void InvalidateMeasureInfo()
        {
            this.InvalidateMeasure();
        }

        private void InvalidatScrollInfo()
        {
            if (this.ScrollOwner != null)
            {
#if WinRT
                if (VerticalOffset != ScrollOwner.VerticalOffset)
                    ScrollOwner.ScrollToVerticalOffset(VerticalOffset);

                if (HorizontalOffset != ScrollOwner.HorizontalOffset)
                    ScrollOwner.ScrollToHorizontalOffset(HorizontalOffset);
#endif
                this.ScrollOwner.InvalidateScrollInfo();
            }
            else
            {
                if (scrollableOwner != null)
                    this.scrollableOwner.InvalidateScrollInfo();
            }
        }

        private VisibleLineInfo GetRowVisibleLineInfo(int index)
        {
            return this.ScrollRows.GetVisibleLineAtLineIndex(index);
        }

        private VisibleLineInfo GetColumnVisibleLineInfo(int index)
        {
            return this.ScrollColumns.GetVisibleLineAtLineIndex(index);
        }

        private void WireScrollLineEvents()
        {
            if (this.RowHeights != null)
                this.RowHeights.LineHiddenChanged += OnRowLineHiddenChanged;
            if (this.ColumnWidths != null)
                this.ColumnWidths.LineHiddenChanged += OnColumnLineHiddenChanged;
        }

        private void UnWireScrollLineEvents()
        {
            if (this.RowHeights != null)
                this.RowHeights.LineHiddenChanged -= OnRowLineHiddenChanged;
            if (this.ColumnWidths != null)
                this.ColumnWidths.LineHiddenChanged -= OnColumnLineHiddenChanged;
        }

        private void OnColumnLineHiddenChanged(object sender, HiddenRangeChangedEventArgs e)
        {
            if (this.RowsGenerator != null)
                this.RowsGenerator.ColumnHiddenChanged(e);
            if (this.ScrollOwner != null)
                this.ScrollOwner.InvalidateMeasure();
        }

        private void OnRowLineHiddenChanged(object sender, HiddenRangeChangedEventArgs e)
        {
            if (this.RowsGenerator != null)
                this.RowsGenerator.RowHiddenChanged(e);
            if (this.ScrollOwner != null)
                this.ScrollOwner.InvalidateScrollInfo();
            else if (this.scrollableOwner != null)
                this.scrollableOwner.InvalidateScrollInfo();
        }

        #endregion

        #region IScrollableInfo

        public void LineDown()
        {
            if (!SuspendManipulationScroll)
            {
                ScrollRows.ScrollToNextLine();
                this.InvalidateMeasure();
            }
        }

        public void LineLeft()
        {
            ScrollColumns.ScrollToPreviousLine();
            this.InvalidateMeasure();
        }

        public void LineRight()
        {
            ScrollColumns.ScrollToNextLine();
            this.InvalidateMeasure();
        }

        public void LineUp()
        {
            if (!SuspendManipulationScroll)
            {
                ScrollRows.ScrollToPreviousLine();
                this.InvalidateMeasure();
            }
        }

        public Rect MakeVisible(UIElement visual, Rect rectangle)
        {
            return rectangle;
        }

        public void MouseWheelDown()
        {
            LineDown();
        }

        public void MouseWheelLeft()
        {
            LineLeft();
        }

        public void MouseWheelRight()
        {
            LineRight();
        }

        public void MouseWheelUp()
        {
            LineUp();
        }

        public void PageDown()
        {
            ScrollRows.ScrollToNextPage();
            this.InvalidateMeasure();
        }

        public void PageLeft()
        {
            ScrollColumns.ScrollToPreviousPage();
            this.InvalidateMeasure();
        }

        public void PageRight()
        {
            ScrollColumns.ScrollToNextPage();
            this.InvalidateMeasure();
        }

        public void PageUp()
        {
            ScrollRows.ScrollToPreviousPage();
            this.InvalidateMeasure();
        }

        public void SetHorizontalOffset(double offset)
        {
            if (SuspendManipulationScroll)
                return;
            HScrollBar.Value = (float)offset + HScrollBar.Minimum;
            this.InvalidateMeasure();
        }

        public void SetVerticalOffset(double offset)
        {
            if (SuspendManipulationScroll)
                return;
            VScrollBar.Value = (float)offset + VScrollBar.Minimum;
            this.InvalidateMeasure();
        }

        public bool CanHorizontallyScroll
        {
            get
            {
                return HScrollBar.Enabled;
            }
            set
            {
                HScrollBar.Enabled = value;
            }
        }

        public bool CanVerticallyScroll
        {
            get
            {
                return VScrollBar.Enabled;
            }
            set
            {
                VScrollBar.Enabled = value;
            }
        }

        public double ExtentHeight
        {
            get { return VScrollBar.Maximum - VScrollBar.Minimum; }
        }

        public double ExtentWidth
        {
            get { return HScrollBar.Maximum - HScrollBar.Minimum; }
        }

        public double HorizontalOffset
        {
            get { return HScrollBar.Value - HScrollBar.Minimum; }
        }


        public ScrollViewer ScrollOwner
        {
            get
            {
                return this._scrollOwner;
            }
            set
            {
                this._scrollOwner = value;
#if WinRT
                this._scrollOwner.Loaded += OnScrollOwnerLoaded;
#endif
#if WP
                if (this.ScrollOwner != null)
                {
                    this.ScrollOwner.ManipulationStarted += OnManipulationStarted;
                    this.ScrollOwner.ManipulationDelta += OnManipulationDelta;
                    this.ScrollOwner.ManipulationCompleted += OnManipulationCompleted;
                }
#endif
            }
        }
#if WP
        
        #region WP7 Scroll Behaviour
#if !WP7
        public PointerDeviceType GetPointerType(RoutedEventArgs originalArgs)
        {
            if (originalArgs is ManipulationDeltaEventArgs)
            {
                return PointerDeviceType.Touch;
            }
            return PointerDeviceType.Mouse;
        }
#endif

        public Point GetPosition(RoutedEventArgs args, UIElement relativeTo)
        {
            if (args is System.Windows.Input.MouseEventArgs)
            {
                return ((System.Windows.Input.MouseEventArgs)args).GetPosition(relativeTo);
            }
            if (args is GestureEventArgs)
            {
                return ((GestureEventArgs)args).GetPosition(relativeTo);
            }
            if (args is ManipulationDeltaEventArgs)
            {
                ManipulationDeltaEventArgs args2 = args as ManipulationDeltaEventArgs;
                return args2.ManipulationContainer.TransformToVisual(relativeTo).Transform(args2.ManipulationOrigin);
            }
            if (args is ManipulationStartedEventArgs)
            {
                ManipulationStartedEventArgs args3 = args as ManipulationStartedEventArgs;
                return args3.ManipulationContainer.TransformToVisual(relativeTo).Transform(args3.ManipulationOrigin);
            }
            return new Point();
        }

        protected void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            ChangeScrollVisualStates("Scrolling");

            e.ManipulationContainer = this;
            var info = new PanningInfo
            {
                OriginalHorizontalOffset = this.HorizontalOffset,
                OriginalVerticalOffset = this.VerticalOffset
            };
            this._panningInfo = info;
            double num = this.ViewportWidth + 1.0;
            double num2 = this.ViewportHeight + 1.0;
            this._panningInfo.DeltaPerHorizontalOffet = DoubleUtil.AreClose(num, 0.0) ? 0.0 : (base.ActualWidth / num);
            this._panningInfo.DeltaPerVerticalOffset = DoubleUtil.AreClose(num2, 0.0)
                                                           ? 0.0
                                                           : (base.ActualHeight / num2);

            this.Start(e);
            e.Handled = true;
            //base.OnManipulationStarted(e);
        }

        void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
#if !WP7
            if (e.PinchManipulation != null)
            {
                if (e.PinchManipulation.CumulativeScale == 1)
                {
                    prevZoomScale = 1;
                    return;
                }
                if (e.PinchManipulation.CumulativeScale > 1)
                {
                    if ((e.PinchManipulation.CumulativeScale - prevZoomScale) > 0)
                        NeedToRefreshColumn = false;
                    else
                        NeedToRefreshColumn = true;
                    _ZoomScale = _ZoomScale + (e.PinchManipulation.CumulativeScale - prevZoomScale);
                    prevZoomScale = e.PinchManipulation.CumulativeScale;
                }
                else if (e.PinchManipulation.CumulativeScale < 1)
                {
                    if ((prevZoomScale - e.PinchManipulation.CumulativeScale) > 0)
                        NeedToRefreshColumn = true;
                    else
                        NeedToRefreshColumn = false;
                    _ZoomScale = _ZoomScale - (prevZoomScale - e.PinchManipulation.CumulativeScale);
                    prevZoomScale = e.PinchManipulation.CumulativeScale;
                }
                if (_ZoomScale > 2)
                    _ZoomScale = 2;
                if (_ZoomScale < 0.5)
                    _ZoomScale = 0.5;
                if (zoomScrollRowIndex == -1 && zoomScrollColumnIndex == -1)
                {
                    midPoint.X = (e.PinchManipulation.Current.PrimaryContact.X +
                                  e.PinchManipulation.Current.SecondaryContact.X) / 2;
                    midPoint.Y = (e.PinchManipulation.Current.PrimaryContact.Y +
                                  e.PinchManipulation.Current.SecondaryContact.Y) / 2;
                    if (this.ScrollRows.GetVisibleLineAtPoint(midPoint.Y) == null || this.ScrollColumns.GetVisibleLineAtPoint(midPoint.X) == null)
                        return;
                    zoomScrollRowIndex = this.ScrollRows.GetVisibleLineAtPoint(midPoint.Y).LineIndex;
                    zoomScrollColumnIndex = this.ScrollColumns.GetVisibleLineAtPoint(midPoint.X).LineIndex;
                    if (zoomScrollColumnIndex < 0 && zoomScrollRowIndex < 0)
                        return;
                    var verticalLine = this.ScrollRows.GetVisibleLineAtLineIndex(zoomScrollRowIndex);
                    var horizontalLine = this.ScrollColumns.GetVisibleLineAtLineIndex(zoomScrollColumnIndex);
                    initialHLineOrigin = horizontalLine.Origin;
                    thumbWidth = ViewportWidth * (this.ViewportWidth / (HScrollBar.Maximum - HScrollBar.Minimum + ViewportWidth));

                    if (midPoint.X - horizontalLine.Origin > 50)
                        HoffsetIncrement = true;
                }


                if (_ZoomScale < 2.0 && _ZoomScale >= 0.5)
                {
                    this.SetZoomScale(_ZoomScale);
                    if (initialHLineOrigin > 0)
                        this.ScrollColumns.ScrollInView(zoomScrollColumnIndex);
                    this.ScrollRows.ScrollInView(zoomScrollRowIndex);


                    midPoint.X = (e.PinchManipulation.Current.PrimaryContact.X +
                                  e.PinchManipulation.Current.SecondaryContact.X) / 2;
                    midPoint.Y = (e.PinchManipulation.Current.PrimaryContact.Y +
                                  e.PinchManipulation.Current.SecondaryContact.Y) / 2;

                    var verticalLine = this.ScrollRows.GetVisibleLineAtLineIndex(zoomScrollRowIndex);
                    var horizontalLine = this.ScrollColumns.GetVisibleLineAtLineIndex(zoomScrollColumnIndex);

                    this.SetVerticalOffset(this.VerticalOffset +
                                           ((verticalLine.Origin + verticalLine.Size) - midPoint.Y));

                    if (initialHLineOrigin != horizontalLine.Origin && initialHLineOrigin > 0 && !HoffsetIncrement)
                    {
                        this.SetHorizontalOffset(this.HorizontalOffset + (Math.Abs(horizontalLine.Origin) - midPoint.X));
                        initialHLineOrigin = horizontalLine.Origin;
                    }
                    else if (HoffsetIncrement)
                    {
                        var thumbSize = ViewportWidth *
                                        (this.ViewportWidth / (HScrollBar.Maximum - HScrollBar.Minimum + ViewportWidth));
                        var diffThumb = thumbWidth - thumbSize;

                        this.SetHorizontalOffset(this.HorizontalOffset + diffThumb);
                        thumbWidth = thumbSize;
                    }


                }
                return;
            }
#endif
            Point deltaTranslation = e.DeltaManipulation.Translation;
            Point point2 = e.CumulativeManipulation.Translation;
            if (!this._dragStarted && (((Math.Abs(point2.X) > this._initialThreshold)) || ((Math.Abs(point2.Y) > this._initialThreshold))))
            {
                this.Start(e);
            }
            if (this._dragStarted)
            {
                this.ManipulateScroll(e);
            }

            e.Handled = true;
        }
        TimeSpan duration;
        TimeSpan update;
        void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
#if !WP7
            zoomScrollColumnIndex = zoomScrollRowIndex = -1;
            prevZoomScale = 1;
#endif
            if (this._panningInfo != null)
            {
                if (this._dragStarted)
                {
                    if (e.IsInertial)
                    {
                        this._cumulativeTranslation = e.TotalManipulation.Translation;
                        update = new TimeSpan(0, 0, 0, 0, 1);
                        duration = new TimeSpan(0, 0, 0, 0, Math.Max((int)(Math.Abs(e.FinalVelocities.LinearVelocity.X * ZoomScale / 1000) * 80), (int)(Math.Abs(e.FinalVelocities.LinearVelocity.Y * ZoomScale / 1000) * 80)));
                        this.StartInertia(e, new Point(e.FinalVelocities.LinearVelocity.X * ZoomScale / 1000.0, e.FinalVelocities.LinearVelocity.Y * ZoomScale / 1000.0));
                    }
                    else
                    {
                        this.Complete();
                        ChangeScrollVisualStates("NotScrolling");
                        ManipulationAnimation();
                        //return;
                    }
                }

                if (!e.IsInertial && !this._panningInfo.IsPanning)
                {
                    e.Handled = true;
                    return;
                }

                ManipulationAnimation();
            }
        }
        
        public Transform LayoutTransform
        {
            get { return (Transform)GetValue(LayoutTransformProperty); }
            set { SetValue(LayoutTransformProperty, value); }
        }

        /// <summary>
        /// Identifies the LayoutTransform DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty LayoutTransformProperty = DependencyProperty.Register(
            "LayoutTransform", typeof(Transform), typeof(VisualContainer), new PropertyMetadata(LayoutTransformChanged));

        private static void LayoutTransformChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            // Casts are safe because Silverlight is enforcing the types
            ((o as VisualContainer)).ProcessTransform((Transform)e.NewValue);
        }
        private void ProcessTransform(Transform transform)
        {
            // Get the transform matrix and apply it
            _transformation = RoundMatrix(GetTransformMatrix(transform), 4);
            if (null != _matrixTransform)
            {
                _matrixTransform.Matrix = _transformation;
            }
            // New transform means re-layout is necessary
            InvalidateMeasure();
        }
        private static Matrix RoundMatrix(Matrix matrix, int decimals)
        {
            return new Matrix(
                Math.Round(matrix.M11, decimals),
                Math.Round(matrix.M12, decimals),
                Math.Round(matrix.M21, decimals),
                Math.Round(matrix.M22, decimals),
                matrix.OffsetX,
                matrix.OffsetY);
        }

        private Matrix GetTransformMatrix(Transform transform)
        {
            if (null != transform)
            {
                // WPF equivalent of this entire method:
                // return transform.Value;

                // Process the TransformGroup
                TransformGroup transformGroup = transform as TransformGroup;
                if (null != transformGroup)
                {
                    Matrix groupMatrix = Matrix.Identity;
                    foreach (Transform child in transformGroup.Children)
                    {
                        groupMatrix = MatrixMultiply(groupMatrix, GetTransformMatrix(child));
                    }
                    return groupMatrix;
                }

                // Process the RotateTransform
                RotateTransform rotateTransform = transform as RotateTransform;
                if (null != rotateTransform)
                {
                    double angle = rotateTransform.Angle;
                    double angleRadians = (2 * Math.PI * angle) / 360;
                    double sine = Math.Sin(angleRadians);
                    double cosine = Math.Cos(angleRadians);
                    return new Matrix(cosine, sine, -sine, cosine, 0, 0);
                }

                // Process the ScaleTransform
                ScaleTransform scaleTransform = transform as ScaleTransform;
                if (null != scaleTransform)
                {
                    double scaleX = scaleTransform.ScaleX;
                    double scaleY = scaleTransform.ScaleY;
                    return new Matrix(scaleX, 0, 0, scaleY, 0, 0);
                }

                // Process the SkewTransform
                SkewTransform skewTransform = transform as SkewTransform;
                if (null != skewTransform)
                {
                    double angleX = skewTransform.AngleX;
                    double angleY = skewTransform.AngleY;
                    double angleXRadians = (2 * Math.PI * angleX) / 360;
                    double angleYRadians = (2 * Math.PI * angleY) / 360;
                    return new Matrix(1, angleYRadians, angleXRadians, 1, 0, 0);
                }

                // Process the MatrixTransform
                MatrixTransform matrixTransform = transform as MatrixTransform;
                if (null != matrixTransform)
                {
                    return matrixTransform.Matrix;
                }

                // TranslateTransform has no effect in LayoutTransform
            }

            // Fall back to no-op transformation
            return Matrix.Identity;
        }

        private static Matrix MatrixMultiply(Matrix matrix1, Matrix matrix2)
        {
            // WPF equivalent of following code:
            // return Matrix.Multiply(matrix1, matrix2);
            return new Matrix(
                (matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21),
                (matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22),
                (matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21),
                (matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22),
                ((matrix1.OffsetX * matrix2.M11) + (matrix1.OffsetY * matrix2.M21)) + matrix2.OffsetX,
                ((matrix1.OffsetX * matrix2.M12) + (matrix1.OffsetY * matrix2.M22)) + matrix2.OffsetY);
        }

        public void ApplyLayoutTransform()
        {
            if (scaleTransform.ScaleX != ZoomScale || scaleTransform.ScaleY != ZoomScale)
            {
                scaleTransform.ScaleX = ZoomScale;
                scaleTransform.ScaleY = ZoomScale;
                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(scaleTransform);
                LayoutTransform = transformGroup;
            }
        }

        private Size ComputeLargestTransformedSize(Size arrangeBounds)
        {

            // Computed largest transformed size
            Size computedSize = Size.Empty;

            // Detect infinite bounds and constrain the scenario
            bool infiniteWidth = double.IsInfinity(arrangeBounds.Width);
            if (infiniteWidth)
            {
                arrangeBounds.Width = arrangeBounds.Height;
            }
            bool infiniteHeight = double.IsInfinity(arrangeBounds.Height);
            if (infiniteHeight)
            {
                arrangeBounds.Height = arrangeBounds.Width;
            }

            // Capture the matrix parameters
            double a = _transformation.M11;
            double b = _transformation.M12;
            double c = _transformation.M21;
            double d = _transformation.M22;

            // Compute maximum possible transformed width/height based on starting width/height
            // These constraints define two lines in the positive x/y quadrant
            double maxWidthFromWidth = Math.Abs(arrangeBounds.Width / a);
            double maxHeightFromWidth = Math.Abs(arrangeBounds.Width / c);
            double maxWidthFromHeight = Math.Abs(arrangeBounds.Height / b);
            double maxHeightFromHeight = Math.Abs(arrangeBounds.Height / d);

            // The transformed width/height that maximize the area under each segment is its midpoint
            // At most one of the two midpoints will satisfy both constraints
            double idealWidthFromWidth = maxWidthFromWidth / 2;
            double idealHeightFromWidth = maxHeightFromWidth / 2;
            double idealWidthFromHeight = maxWidthFromHeight / 2;
            double idealHeightFromHeight = maxHeightFromHeight / 2;

            // Compute slope of both constraint lines
            double slopeFromWidth = -(maxHeightFromWidth / maxWidthFromWidth);
            double slopeFromHeight = -(maxHeightFromHeight / maxWidthFromHeight);

            if ((0 == arrangeBounds.Width) || (0 == arrangeBounds.Height))
            {
                // Check for empty bounds
                computedSize = new Size(arrangeBounds.Width, arrangeBounds.Height);
            }
            else if (infiniteWidth && infiniteHeight)
            {
                // Check for completely unbound scenario
                computedSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            }
            //else if (!MatrixHasInverse(_transformation))
            //{
            //    // Check for singular matrix
            //    computedSize = new Size(0, 0);
            //}
            else if ((0 == b) || (0 == c))
            {
                // Check for 0/180 degree special cases
                double maxHeight = (infiniteHeight ? double.PositiveInfinity : maxHeightFromHeight);
                double maxWidth = (infiniteWidth ? double.PositiveInfinity : maxWidthFromWidth);
                if ((0 == b) && (0 == c))
                {
                    // No constraints
                    computedSize = new Size(maxWidth, maxHeight);
                }
                else if (0 == b)
                {
                    // Constrained by width
                    double computedHeight = Math.Min(idealHeightFromWidth, maxHeight);
                    computedSize = new Size(
                        maxWidth - Math.Abs((c * computedHeight) / a),
                        computedHeight);
                }
                else if (0 == c)
                {
                    // Constrained by height
                    double computedWidth = Math.Min(idealWidthFromHeight, maxWidth);
                    computedSize = new Size(
                        computedWidth,
                        maxHeight - Math.Abs((b * computedWidth) / d));
                }
            }
            else if ((0 == a) || (0 == d))
            {
                // Check for 90/270 degree special cases
                double maxWidth = (infiniteHeight ? double.PositiveInfinity : maxWidthFromHeight);
                double maxHeight = (infiniteWidth ? double.PositiveInfinity : maxHeightFromWidth);
                if ((0 == a) && (0 == d))
                {
                    // No constraints
                    computedSize = new Size(maxWidth, maxHeight);
                }
                else if (0 == a)
                {
                    // Constrained by width
                    double computedHeight = Math.Min(idealHeightFromHeight, maxHeight);
                    computedSize = new Size(
                        maxWidth - Math.Abs((d * computedHeight) / b),
                        computedHeight);
                }
                else if (0 == d)
                {
                    // Constrained by height
                    double computedWidth = Math.Min(idealWidthFromWidth, maxWidth);
                    computedSize = new Size(
                        computedWidth,
                        maxHeight - Math.Abs((a * computedWidth) / c));
                }
            }
            else if (idealHeightFromWidth <= ((slopeFromHeight * idealWidthFromWidth) + maxHeightFromHeight))
            {
                // Check the width midpoint for viability (by being below the height constraint line)
                computedSize = new Size(idealWidthFromWidth, idealHeightFromWidth);
            }
            else if (idealHeightFromHeight <= ((slopeFromWidth * idealWidthFromHeight) + maxHeightFromWidth))
            {
                // Check the height midpoint for viability (by being below the width constraint line)
                computedSize = new Size(idealWidthFromHeight, idealHeightFromHeight);
            }
            else
            {
                // Neither midpoint is viable; use the intersection of the two constraint lines instead
                // Compute width by setting heights equal (m1*x+c1=m2*x+c2)
                double computedWidth = (maxHeightFromHeight - maxHeightFromWidth) / (slopeFromWidth - slopeFromHeight);
                // Compute height from width constraint line (y=m*x+c; using height would give same result)
                computedSize = new Size(
                    computedWidth,
                    (slopeFromWidth * computedWidth) + maxHeightFromWidth);
            }

            // Return result
            return computedSize;
        }

        private void ChangeScrollVisualStates(string State)
        {
            if (this.ScrollOwner == null)
                return;
            if (State.Equals("Scrolling"))
                VisualStateManager.GoToState(this.ScrollOwner, "Scrolling", true);
            else
                VisualStateManager.GoToState(this.ScrollOwner, "NotScrolling", true);
        }

        private void ManipulateScroll(double delta, double cumulativeTranslation, bool isHorizontal)
        {
            double num = isHorizontal ? this._panningInfo.UnusedTranslation.X : this._panningInfo.UnusedTranslation.Y;
            double num2 = isHorizontal ? this.HorizontalOffset : this.VerticalOffset;
            double num3 = isHorizontal ? this.ScrollableWidth : this.ScrollableHeight;
            if (DoubleUtil.AreClose(num3, 0.0))
            {
                // If the Scrollable length in this direction is 0,
                // then we should neither scroll nor report the boundary feedback
                num = 0.0;
                delta = 0.0;
            }
            else if ((DoubleUtil.GreaterThan(delta, 0.0) && DoubleUtil.AreClose(num2, 0.0)) ||
                     (DoubleUtil.LessThan(delta, 0.0) && DoubleUtil.AreClose(num2, num3)))
            {
                // If we are past the boundary and the delta is in the same direction,
                // then add the delta to the unused vector
                num += delta;
                delta = 0.0;
            }
            else if (DoubleUtil.LessThan(delta, 0.0) && DoubleUtil.GreaterThan(num, 0.0))
            {
                // If we are past the boundary in positive direction
                // and the delta is in negative direction,
                // then compensate the delta from unused vector.
                double num4 = Math.Max((double)(num + delta), (double)0.0);
                delta += num - num4;
                num = num4;
            }
            else if (DoubleUtil.GreaterThan(delta, 0.0) && DoubleUtil.LessThan(num, 0.0))
            {
                // If we are past the boundary in negative direction
                // and the delta is in positive direction,
                // then compensate the delta from unused vector.
                double num5 = Math.Min((double)(num + delta), (double)0.0);
                delta += num - num5;
                num = num5;
            }
            if (isHorizontal)
            {
                if (!DoubleUtil.AreClose(delta, 0.0))
                {
                    this.SetHorizontalOffset(this._panningInfo.OriginalHorizontalOffset -
                                                  Math.Round(
                                                      (double)
                                                      (cumulativeTranslation / this._panningInfo.DeltaPerHorizontalOffet)));
                }
                this._panningInfo.UnusedTranslation = new Point(num, this._panningInfo.UnusedTranslation.Y);
            }
            else
            {
                if (!DoubleUtil.AreClose(delta, 0.0))
                {
                    this.SetVerticalOffset(this._panningInfo.OriginalVerticalOffset -
                                                Math.Round(
                                                    (double)
                                                    (cumulativeTranslation / this._panningInfo.DeltaPerVerticalOffset)));
                }
                this._panningInfo.UnusedTranslation = new Point(this._panningInfo.UnusedTranslation.X, num);
            }
        }

        private void ManipulateScroll(Point delta)
        {
            this.ManipulateScroll(delta.X, this._cumulativeTranslation.X, true);
            this.ManipulateScroll(delta.Y, this._cumulativeTranslation.Y, false);
        }

        private void ManipulateScroll(ManipulationDeltaEventArgs e)
        {
            this.ManipulateScroll(e.DeltaManipulation.Translation.X, e.CumulativeManipulation.Translation.X, true);
            this.ManipulateScroll(e.DeltaManipulation.Translation.Y, e.CumulativeManipulation.Translation.Y, false);

            if (e.IsInertial)
            {
                e.Complete();
            }
            else
            {
                double y = this._panningInfo.UnusedTranslation.Y;
                if (!this._panningInfo.InVerticalFeedback && DoubleUtil.LessThan(Math.Abs(y), PreFeedbackTranslationY))
                {
                    y = 0.0;
                }
                this._panningInfo.InVerticalFeedback = !DoubleUtil.AreClose(y, 0.0);
                this.VerticalPadding = y;
                double x = this._panningInfo.UnusedTranslation.X;
                if (!this._panningInfo.InHorizontalFeedback && DoubleUtil.LessThan(Math.Abs(x), PreFeedbackTranslationX))
                {
                    x = 0.0;
                }
                this._panningInfo.InHorizontalFeedback = !DoubleUtil.AreClose(x, 0.0);
                this.HorizontalPadding = x;
                if (x != 0 || y != 0)
                {
                    this.SetHorizontalOffset(this.HorizontalOffset);
                }
            }
        }

        private void StartInertia(RoutedEventArgs originalArgs, Point velocities)
        {
            //this.RaiseDragInertiaStarted(originalArgs, velocities);
            CompositionTarget.Rendering += (new EventHandler(this.OnRendering));
            this._velocity = velocities;
            this._lastTimeStamp = DateTime.Now;
        }

        private void Start(RoutedEventArgs originalArgs)
        {
            if (this._dragStarted)
            {
                this.Complete();
            }
            this._cumulativeTranslation = new Point();
            this._dragStarted = true;
        }

        internal void Complete()
        {
            if (this._dragStarted)
            {

                this._dragStarted = false;
                this._velocity = new Point();
                CompositionTarget.Rendering -= new EventHandler(this.OnRendering);
                //ManipulationAnimation();
                //this._panningInfo = null;
            }
        }

        /// <summary>
        /// This Method is used for Translation When the User Pulls the Grid along the Edges
        /// </summary>
#if !WP7
        private async void ManipulationAnimation()
#else
        private void ManipulationAnimation()
#endif
        {
            double YanimationRatio = Math.Abs(this.VerticalPadding) / 2;
            double XanimationRatio = Math.Abs(this.HorizontalPadding) / 2;
            if ((this.VerticalPadding >= 0) && (this.HorizontalPadding >= 0))
            {
                while ((this.VerticalPadding > 0) || (this.HorizontalPadding > 0))
                {
#if !WP7
                    await Task.Delay(1);
#endif
                    this.VerticalPadding = Math.Round(this.VerticalPadding) < 0
                                                          ? 0
                                                          : this.VerticalPadding - YanimationRatio;
                    this.HorizontalPadding = Math.Round(this.HorizontalPadding) < 0
                                                            ? 0
                                                            : this.HorizontalPadding - XanimationRatio;
                    this.VerticalPadding = Math.Max(0.0, this.VerticalPadding);
                    this.HorizontalPadding = Math.Max(0.0, this.HorizontalPadding);
                    this.SetHorizontalOffset(this.HorizontalOffset);
                }
            }
            else if ((this.VerticalPadding <= 0) && (this.HorizontalPadding <= 0))
            {
                while ((this.VerticalPadding < 0) || (this.HorizontalPadding < 0))
                {
#if !WP7
                    await Task.Delay(1);
#endif
                    this.VerticalPadding = Math.Round(this.VerticalPadding) > 0
                                                          ? 0
                                                          : this.VerticalPadding + YanimationRatio;
                    this.HorizontalPadding = Math.Round(this.HorizontalPadding) > 0
                                                            ? 0
                                                            : this.HorizontalPadding + XanimationRatio;
                    this.VerticalPadding = Math.Min(0.0, this.VerticalPadding);
                    this.HorizontalPadding = Math.Min(0.0, this.HorizontalPadding);
                    this.SetHorizontalOffset(this.HorizontalOffset);
                }
            }
            else if ((this.VerticalPadding <= 0) && (this.HorizontalPadding >= 0))
            {
                while ((this.VerticalPadding < 0) || (this.HorizontalPadding > 0))
                {
#if !WP7
                    await Task.Delay(1);
#endif
                    this.VerticalPadding = Math.Round(this.VerticalPadding) > 0
                                                          ? 0
                                                          : this.VerticalPadding + YanimationRatio;
                    this.HorizontalPadding = Math.Round(this.HorizontalPadding) < 0
                                                            ? 0
                                                            : this.HorizontalPadding - XanimationRatio;
                    this.VerticalPadding = Math.Min(0.0, this.VerticalPadding);
                    this.HorizontalPadding = Math.Max(0.0, this.HorizontalPadding);
                    this.SetHorizontalOffset(this.HorizontalOffset);
                }
            }
            else
            {
                while ((this.VerticalPadding > 0) || (this.HorizontalPadding < 0))
                {
#if !WP7
                    await Task.Delay(1);
#endif
                    this.VerticalPadding = Math.Round(this.VerticalPadding) < 0
                                                          ? 0
                                                          : this.VerticalPadding - YanimationRatio;
                    this.HorizontalPadding = Math.Round(this.HorizontalPadding) > 0
                                                            ? 0
                                                            : this.HorizontalPadding + XanimationRatio;
                    this.VerticalPadding = Math.Max(0.0, this.VerticalPadding);
                    this.HorizontalPadding = Math.Min(0.0, this.HorizontalPadding);
                    this.SetHorizontalOffset(this.HorizontalOffset);
                }
            }
        }

        private void OnRendering(object sender, EventArgs e)
        {
            Point delta = new Point();
            DateTime now = DateTime.Now;
            TimeSpan span2 = (TimeSpan)(now - this._lastTimeStamp);
            double num3 = span2.TotalMilliseconds;
            this._lastTimeStamp = now;
            double totalMilliseconds = duration.Milliseconds - update.Milliseconds;
            update += new TimeSpan(0, 0, 0, 0, 12);
            double velocity = Math.Max(Math.Abs(this._velocity.X), Math.Abs(this._velocity.Y));
            Point finalVelocity = this.DeceleratePoint(this._velocity, totalMilliseconds);
            Point point2 = new Point();
            if (velocity != Math.Abs(this._velocity.X))
            {
                point2.X = (((this._velocity.X / velocity) * finalVelocity.X) * num3);
                if (this._velocity.Y < 0)
                    point2.Y = -((0.01 / 2) * Math.Pow(totalMilliseconds, 2));
                else
                    point2.Y = ((0.01 / 2) * Math.Pow(totalMilliseconds, 2));
            }
            else
            {
                point2.Y = (((this._velocity.Y / velocity) * finalVelocity.Y) * num3);
                //point2.X = (((this._velocity.X / velocity) * finalVelocity.X) * num3);
                if (this._velocity.X < 0)
                    point2.X = -((0.01 / 2) * Math.Pow(totalMilliseconds, 2)) * ZoomScale;
                else
                    point2.X = ((0.01 / 2) * Math.Pow(totalMilliseconds, 2)) * ZoomScale;
            }
            delta = point2;
            this._cumulativeTranslation = new Point(this._cumulativeTranslation.X + delta.X, this._cumulativeTranslation.Y + delta.Y);
            if (this._panningInfo != null)
            {
                this.ManipulateScroll(delta);
            }
            if (totalMilliseconds <= 0)
            {
                this.Complete();
                ChangeScrollVisualStates("NotScrolling");
            }
        }


        private Point DeceleratePoint(Point velocity, double elapsedTimeMilliseconds)
        {
            velocity = new Point(Math.Abs(velocity.X) - (0.01 * elapsedTimeMilliseconds), Math.Abs(velocity.Y) - (0.01 * elapsedTimeMilliseconds));
            return velocity;
        }
        #endregion

#endif
        public double VerticalOffset
        {
            get { return VScrollBar.Value - VScrollBar.Minimum; }
        }

        public double ViewportHeight
        {
            get { return VScrollBar.LargeChange; }
        }

        public double ViewportWidth
        {
            get { return HScrollBar.LargeChange; }
        }

        #endregion

        public void Dispose()
        {
            UnWireScrollLineEvents();
#if WinRT
            UnWireEvents();
#endif
            if (_scrollColumns != null)
            {
                if (_scrollColumns != null)
                    _scrollColumns.Changed -= OnScrollColumnsChanged;
                this._scrollColumns.Dispose();
                //this._scrollColumns = null;
            }
            if (this._scrollRows != null)
            {
                this._scrollRows.Dispose();
                //this._scrollRows = null;
            }
            if (this.columnWidthsProvider != null)
            {
                this.columnWidthsProvider.Dispose();
                this.columnWidthsProvider = null;
            }
            if (this.rowHeightsProvider != null)
            {
                this.rowHeightsProvider.Dispose();
                this.rowHeightsProvider = null;
            }
            this.Children.Clear();
            this.DragBorderBrush = null;
            this.horizontalLine = null;
            this.verticalLine = null;
            this._hScrollBar = null;
            this._vScrollBar = null;
            this._scrollOwner = null;
            this.RowsGenerator = null;
        }

#if WPF

        public static DependencyObject GetParent(DependencyObject current)
        {
            if (current == null)
            {
                throw new ArgumentNullException("current");
            }
            var element = current as FrameworkElement;
            if (element != null)
            {
                if (element.Parent != null)
                    return element.Parent;
                else if (element.TemplatedParent != null)
                    return element.TemplatedParent;
            }
            var element2 = current as FrameworkContentElement;
            if (element2 != null)
            {
                return element2.Parent;
            }
            return null;
        }

        public static readonly DependencyProperty WantsMouseInputProperty = DependencyProperty.Register(
            "WantsMouseInput", typeof(bool?), typeof(VisualContainer), null);

        public static bool? GetWantsMouseInput(DependencyObject dpo, UIElement falseIfParent)
        {
            while (dpo.GetValue(WantsMouseInputProperty) == null)
            {
                var parent = VisualContainer.GetParent(dpo);
                if (parent == falseIfParent)
                    return false;
                if (parent == null)
                    return null;
                dpo = parent;
            }
            return (bool?)dpo.GetValue(WantsMouseInputProperty);
        }

        public static void SetWantsMouseInput(DependencyObject dpo, bool? value)
        {
            dpo.SetValue(WantsMouseInputProperty, value);
        }


#endif
#if WPF

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return Rect.Empty;
        }
#endif

        private ScrollableContentViewer scrollableOwner;

        public ScrollableContentViewer ScrollableOwner
        {
            get { return scrollableOwner; }
            set { scrollableOwner = value; }
        }

        internal void UpdateRowInfo(LineSizeCollection lines, double deafultRowHeight)
        {
            this.rowHeightsProvider = lines ?? this.OnCreateRowHeights();
            this.rowHeightsProvider.DefaultLineSize = deafultRowHeight;
#if WinRT
            this.columnWidthsProvider.DefaultLineSize = 120;
#else
            this.columnWidthsProvider.DefaultLineSize = 150;
#endif
            this._scrollRows = null;
        }
    }
}


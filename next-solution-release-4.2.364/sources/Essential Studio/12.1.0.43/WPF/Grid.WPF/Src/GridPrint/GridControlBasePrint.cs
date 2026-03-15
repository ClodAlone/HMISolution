#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Syncfusion.Windows.Documents;
    using System.Windows;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Windows.GridCommon;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Syncfusion.Windows.Controls.Cells;

    partial class GridControlBase : IGridPrintPaginator
    {
        #region IGridPrintPaginator Members

        private Size printSize = Size.Empty;
        private int[] printColRange;
        private ScalingOptions _scalingOption = ScalingOptions.NoScaling;

        public static readonly DependencyProperty PrintHeaderTemplateProperty = DependencyProperty.Register("PrintHeaderTemplate",typeof(DataTemplate),typeof(GridControlBase));
        /// <summary>
        /// Gets or sets a data template for the print header.
        /// </summary>
        public DataTemplate PrintHeaderTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(GridControlBase.PrintHeaderTemplateProperty);
            }

            set
            {
                this.SetValue(GridControlBase.PrintHeaderTemplateProperty, value);
            }
        }

        public static readonly DependencyProperty PrintFooterTemplateProperty = DependencyProperty.Register("PrintFooterTemplate",typeof(DataTemplate),typeof(GridControlBase));

        /// <summary>
        /// Gets or sets a data template for the print footer.
        /// </summary>
        public DataTemplate PrintFooterTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(GridControlBase.PrintFooterTemplateProperty);
            }

            set
            {
                this.SetValue(GridControlBase.PrintFooterTemplateProperty, value);
            }
        }

        private static readonly DependencyProperty PageCountProperty = DependencyProperty.Register(
            "PageCount",
            typeof(int),
            typeof(GridControlBase));

        /// <summary>
        /// Gets the number of pages.
        /// </summary>
        public int PageCount
        {
            get
            {
                return (int)this.GetValue(GridControlBase.PageCountProperty);
            }
        }

        int IGridPrintPaginator.GetPrintTotalPageCount(Size printPageSize)
        {
            var gridPrintSize = new Size(printPageSize.Width- this.PrintPageMargin.Left - this.PrintPageMargin.Right, Math.Round(printPageSize.Height - this.PrintHeaderHeight - this.PrintFooterHeight- this.PrintPageMargin.Top - this.PrintPageMargin.Bottom));
            return this.ComputePageCount(gridPrintSize);
        }

        private GridRangeInfo printRange;
        /// <summary>
        /// Gets or sets the grid range that is selected for printing.
        /// </summary>
        public GridRangeInfo PrintRange
        {
            get
            {
                return this.printRange;
            }
            set
            {
                var expandedRange = value.ExpandRange(0, 0, this.Model.RowCount, this.Model.ColumnCount);
                this.printRange = expandedRange;
                this.isDirty = true;
            }
        }

        public int[] PrintColumns
        {
            get
            {
                return printColRange;
            }
            set
            {
                printColRange = value;
            }
        }

        public ScalingOptions ScalingOptions
        {
            get
            {
                return _scalingOption;
            }
            set
            {
                _scalingOption = value;
            }
        }

        private bool isDirty = true;
        Dictionary<int, Dictionary<VisibleLineInfo, List<VisibleLineInfo>>> pageSizes;
        private int ComputePageCount(Size printPageSize)
        {
            if (!isDirty)
            {
                return this.PageCount;
            }
            this.isDirty = false;
            var counter0 = 0;
            var printRange = this.PrintRange;
            this.pageSizes = new Dictionary<int, Dictionary<VisibleLineInfo, List<VisibleLineInfo>>>();
            var horizontalScrollAxis = this.ScrollColumns;
            var abortWidth = Math.Ceiling(printPageSize.Width);
            var verticalScrollAxis = this.ScrollRows;
            var abortHeight = Math.Ceiling(printPageSize.Height);

            if (ScalingOptions == ScalingOptions.FitAllColumnsonOnePage)
            {
                var scaleratio = abortWidth / this.Model.ColumnWidths.TotalExtent;
                abortHeight = abortHeight / scaleratio;
            }
            else if (ScalingOptions == ScalingOptions.FitAllRowsonOnePage)
            {
                var scaleratio = abortHeight / this.Model.RowHeights.TotalExtent;
                abortWidth = abortWidth / scaleratio;
            }
            
            var rowDictionary = new Dictionary<VisibleLineInfo, List<VisibleLineInfo>>();
            var rowOrigin = 0d;
            var reset = false;
            var colReset = false;
            var reachedEnd = false;
            var leftRange = printRange.Left;
            var tLeftRange = 0;
            var topRange = printRange.Top;
            var tTopRange = 0;
            var lTopRange = printRange.Top;
            var bottomRange = printRange.Bottom;
            for (int r = topRange; r <= bottomRange; r++)
            {
                if (reset)
                {
                    reset = false;
                    this.pageSizes.Add(counter0, rowDictionary);
                    if (colReset)
                    {
                        colReset = false;
                        if (tLeftRange <= printRange.Right)
                        {
                            leftRange = tLeftRange;
                            if (!reachedEnd)
                            {
                                r = lTopRange;
                            }
                        }
                        else
                        {
                            leftRange = printRange.Left;
                            r = tTopRange;
                        }
                    }
                    else
                    {
                        leftRange = printRange.Left;
                        tTopRange = r;
                    }
                    //reset
                    rowOrigin = 0d;
                    counter0 += 1;
                    lTopRange = r;
                    rowDictionary = new Dictionary<VisibleLineInfo, List<VisibleLineInfo>>();
                }

                var vertHeight = verticalScrollAxis.GetLineSize(r);
                var visibleRow = new VisibleLineInfo(r, r, vertHeight, rowOrigin, 0, r < this.Model.HeaderRows, r > (this.Model.RowCount - this.Model.FooterRows));
                var colOrigin = 0d;
                var columnList = new List<VisibleLineInfo>();
                for (int c = leftRange; c <= printRange.Right; c++)
                {
                    //if (this.printColRange != null && !this.printColRange.Contains<int>(c))
                    //    continue;
                    var horzWidth = horizontalScrollAxis.GetLineSize(c);
                    //Checking whethether the ColumnWidth exceeds to Total Page Width
                    if (ScalingOptions == ScalingOptions.FitAllRowsonOnePage || ScalingOptions == ScalingOptions.NoScaling)
                    {
                        if (horzWidth > abortWidth && colOrigin == 0 || horzWidth > abortWidth && colOrigin < abortWidth / 2)
                        {
                        }

                        else if (colOrigin + horzWidth > abortWidth)
                        {
                            colReset = true;
                            tLeftRange = c;
                            break;
                        }
                    }
                    var visibleColumn = new VisibleLineInfo(c, c, horzWidth, colOrigin, 0, c < this.Model.HeaderColumns, c > (this.Model.ColumnCount - this.Model.FooterColumns));
                    columnList.Add(visibleColumn);
                    colOrigin += horzWidth;
                }

                if (ScalingOptions == ScalingOptions.FitAllColumnsonOnePage || ScalingOptions == ScalingOptions.NoScaling)
                {
                    if (rowOrigin + vertHeight > abortHeight)
                    {
                        reset = true;
                        tTopRange = r;
                        r -= 1; // minus 1 since it will be ++ in the loop
                        continue;
                    }
                }

                if (r == printRange.Bottom)
                {
                    if (colReset)
                    {
                        rowDictionary.Add(visibleRow, columnList);
                        r = tTopRange - 1;// minus 1 since it will be ++ in the loop
                        reset = true;
                        reachedEnd = true;
                        continue;
                    }
                    else
                    {
                        rowDictionary.Add(visibleRow, columnList);
                        this.pageSizes.Add(counter0, rowDictionary);
                        break;
                    }
                }
                rowDictionary.Add(visibleRow, columnList);
                rowOrigin += vertHeight;
            }
            var totalPageCount = counter0 + 1;
            this.SetValue(GridControlBase.PageCountProperty, totalPageCount);
            return this.PageCount;
        }

        FrameworkElement IGridPrintPaginator.GetPrintVisualAt(int pageNumber, Size PageSize)
        {
            if (this.pageSizes.Count == 0 || !this.pageSizes.ContainsKey(pageNumber))
            {
                return null;
            }

            var abortWidth = Math.Ceiling(PageSize.Width);
            abortWidth = abortWidth - this.PrintPageMargin.Left - this.PrintPageMargin.Right;
            var abortHeight = Math.Ceiling(PageSize.Height);
            abortHeight = abortHeight - this.PrintHeaderHeight - this.PrintFooterHeight - this.PrintPageMargin.Top - this.PrintPageMargin.Bottom;
            var drawingVisual = new GridPrintVisual();
            var dc = drawingVisual.ContentVisual.RenderOpen();
            var page = this.pageSizes[pageNumber];
            var coveredRangeList = new GridRangeInfoList();
            foreach (var kvp in page)
            {
                var visibleRow = kvp.Key;
                //Rect cellRect = new Rect(0, visibleRow.Origin, this.printSize.Width, visibleRow.Size);
                foreach (var visibleColumn in kvp.Value)
                {
                    //cellRect.X = visibleColumn.Origin;
                    //cellRect.Width = visibleColumn.Size;
                    var currentCellRange = GridRangeInfo.Cell(visibleRow.LineIndex, visibleColumn.LineIndex);
                    if (coveredRangeList.AnyRangeIntersects(currentCellRange))
                    {
                        continue;
                    }

                    var cc = this.CoveredCells.GetCellSpan(visibleRow.LineIndex, visibleColumn.LineIndex);
                    if (cc != null)
                    {
                        //currentCellRange = GridRangeInfo.Cells(cc.Top, cc.Left, cc.Bottom, cc.Right);
                        int right = cc.Right;
                        int left = cc.Left;
                        if (left < kvp.Value[0].LineIndex && visibleColumn.LineIndex >= kvp.Value[0].LineIndex)
                        {
                            left = kvp.Value[0].LineIndex;
                        }
                        if (right > kvp.Value[kvp.Value.Count - 1].LineIndex)
                        {
                            right = kvp.Value[kvp.Value.Count - 1].LineIndex;
                        }
                        currentCellRange = GridRangeInfo.Cells(cc.Top, left, cc.Bottom, right);
                        coveredRangeList.Add(currentCellRange);
                    }

                    var renderStyle = this.GetRenderStyleInfo(visibleRow.LineIndex, visibleColumn.LineIndex);
                    DoubleSpan[] yCurrentCellPos = ScrollRows.RangeToRegionPoints(currentCellRange.Top, currentCellRange.Bottom, true);
                    DoubleSpan[] xCurrentCellPos = ScrollColumns.RangeToRegionPoints(currentCellRange.Left, currentCellRange.Right, true);
                    var rowRegion = 1;
                    var columnRegion = 1;
                    //for (int rowRegion = 0; rowRegion < 3; rowRegion++)
                    //{
                    if (yCurrentCellPos[rowRegion].IsEmpty)
                    {
                        continue;
                    }

                    //for (int columnRegion = 0; columnRegion < 3; columnRegion++)
                    //{
                    if (xCurrentCellPos[columnRegion].IsEmpty)
                    {
                        continue;
                    }

                    Rect clipRect = GetClipRect((ScrollAxisRegion)rowRegion, (ScrollAxisRegion)columnRegion);
                    //Rect r = new Rect(xCurrentCellPos[columnRegion].Start, yCurrentCellPos[rowRegion].Start, xCurrentCellPos[columnRegion].Length, yCurrentCellPos[rowRegion].Length);
                    Rect r = new Rect(visibleColumn.Origin, visibleRow.Origin, xCurrentCellPos[columnRegion].Length, yCurrentCellPos[rowRegion].Length);
                    //var cellRect = r;

                    dc.DrawRectangle(renderStyle.Background, null, r);
                    this.RenderBorder(dc, r, clipRect, CellBorderSide.Top, renderStyle.Borders.Top);
                    this.RenderBorder(dc, r, clipRect, CellBorderSide.Left, renderStyle.Borders.Left);
                    this.RenderBorder(dc, r, clipRect, CellBorderSide.Right, renderStyle.Borders.Right);
                    this.RenderBorder(dc, r, clipRect, CellBorderSide.Bottom, renderStyle.Borders.Bottom);
                    //}
                    //}

                    var rca = new RenderCellArgs(this, visibleRow, visibleColumn, r, renderStyle);
                    if (rca.CellUIElements != null && renderStyle.CellType != "NestedGrid" && renderStyle.CellType != "CheckBox")
                    {
                        foreach (UIElement el in rca.CellUIElements.UIElements)
                        {
                            VisualBrush vb = new VisualBrush(el) { Stretch = Stretch.None, AlignmentX = AlignmentX.Left, AlignmentY = AlignmentY.Top };
                            Rect rect = GridCellTextBoxRenderer.GetBounds(el);
                            rect = new Rect(r.X, r.Y, rect.Width, rect.Height);
                            dc.DrawRectangle(vb, null, rect);
                        }
                        var renderer = this.CellRenderers[renderStyle.ModelStyle.CellType];
                        renderer.Render(dc, rca);
                    }
                    else
                    {
                       // var renderer = this.CellRenderers["Static"];
                        var renderer = this.CellRenderers[renderStyle.ModelStyle.CellType];
                        renderer.RenderForPrinting(dc, rca, renderStyle);
                    }
                }
            }
            dc.Close();
            if (ScalingOptions == ScalingOptions.FitAllColumnsonOnePage)
            {
                var scaleratio = abortWidth / this.Model.ColumnWidths.TotalExtent;
                drawingVisual.RenderTransform = new ScaleTransform(scaleratio, scaleratio);
            }
            else if (ScalingOptions == ScalingOptions.FitAllRowsonOnePage)
            {
               var scaleratio = abortHeight / this.Model.RowHeights.TotalExtent;
                drawingVisual.RenderTransform = new ScaleTransform(scaleratio, scaleratio);
            }
            else if (ScalingOptions == ScalingOptions.FitGridonOnePage)
            {
                var heightscaleratio = abortHeight / this.Model.RowHeights.TotalExtent;
                var widthscaleratio = abortWidth / this.Model.ColumnWidths.TotalExtent;
                var scaleratio = heightscaleratio > widthscaleratio ? widthscaleratio : heightscaleratio;
                drawingVisual.RenderTransform = new ScaleTransform(scaleratio, scaleratio);
            }
            drawingVisual.Margin = new Thickness(this.PrintPageMargin.Left, this.PrintPageMargin.Top, this.PrintPageMargin.Right, this.PrintPageMargin.Bottom);

            //generate grid
            System.Windows.Controls.Grid gridPanel = new System.Windows.Controls.Grid();
            gridPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(this.PrintHeaderHeight) });
            var gridPrintSize = new Size(this.printSize.Width, this.printSize.Height - this.PrintHeaderHeight - this.PrintFooterHeight);
            gridPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(Math.Round(gridPrintSize.Height)) });
            gridPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(this.PrintFooterHeight) });

            if (this.PrintHeaderHeight > 0)
            {
                var headerEl = this.PrintHeaderTemplate.LoadContent() as FrameworkElement;
                GridPrintPageWrapper wrapperClass = new GridPrintPageWrapper();
                wrapperClass.PageNumber = pageNumber+1;
                wrapperClass.TotalPage = PageCount;
                headerEl.DataContext = wrapperClass;
                gridPanel.Children.Add(headerEl);
                System.Windows.Controls.Grid.SetRow(headerEl, 0);
            }

            //body
            gridPanel.Children.Add(drawingVisual);
            System.Windows.Controls.Grid.SetRow(drawingVisual, 1);

            //footer
            if (this.PrintFooterHeight > 0)
            {
                var footerEl = this.PrintFooterTemplate.LoadContent() as FrameworkElement;
                GridPrintPageWrapper wrapperClass = new GridPrintPageWrapper();
                wrapperClass.PageNumber = pageNumber + 1;
                wrapperClass.TotalPage = PageCount;
                footerEl.DataContext = wrapperClass;
                gridPanel.Children.Add(footerEl);
                System.Windows.Controls.Grid.SetRow(footerEl, 2);
            }

            gridPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            // Layout pass.
            gridPanel.Arrange(new Rect(new Point(0, 0), this.printSize));

            gridPanel.FlowDirection = this.model.TableStyle.FlowDirection;
            return gridPanel;
        }

        private void RenderBorder(DrawingContext dc, Rect cellRect, Rect clipRect, CellBorderSide borderSide, Pen pen)
        {
            if (cellRect.Width == 0)
                return;

            //if (cellRect != clipRect)
            //{
            //    cellRect = clipRect;
            //}

            switch (borderSide)
            {
                case CellBorderSide.Top:
                    dc.DrawLine(pen, cellRect.TopLeft, cellRect.TopRight);
                    break;
                case CellBorderSide.Bottom:
                    dc.DrawLine(pen, cellRect.BottomLeft, cellRect.BottomRight);
                    break;
                case CellBorderSide.Left:
                    dc.DrawLine(pen, cellRect.TopLeft, cellRect.BottomLeft);
                    break;
                case CellBorderSide.Right:
                    dc.DrawLine(pen, cellRect.TopRight, cellRect.BottomRight);
                    break;
            }
        }

        public static readonly DependencyProperty HeaderHeightProperty = DependencyProperty.Register(
            "HeaderHeight",
            typeof(double),
            typeof(GridControlBase));

        /// <summary>
        /// Gets or sets the height for the print header.
        /// </summary>
        public double PrintHeaderHeight
        {
            get
            {
                return (double)this.GetValue(GridControlBase.HeaderHeightProperty);
            }

            set
            {
                this.SetValue(GridControlBase.HeaderHeightProperty, value);
            }
        }

        public static readonly DependencyProperty FooterHeightProperty = DependencyProperty.Register(
            "FooterHeight",
            typeof(double),
            typeof(GridControlBase));

        /// <summary>
        /// Gets or sets the height for the print footer.
        /// </summary>
        public double PrintFooterHeight
        {
            get
            {
                return (double)this.GetValue(GridControlBase.FooterHeightProperty);
            }

            set
            {
                this.SetValue(GridControlBase.FooterHeightProperty, value);
            }
        }

        /// <summary>
        /// Sets the print page size.
        /// </summary>
        /// <param name="printSize">Print size.</param>
        public void SetPrintPageSize(Size printSize)
        {
            this.printSize = printSize;
            this.isDirty = true;
        }

        public static readonly DependencyProperty PrintDescriptionProperty = DependencyProperty.Register(
            "PrintDescription",
            typeof(string),
            typeof(GridControlBase));

        /// <summary>
        /// Specifies a description of the job to be printed.
        /// This text will appear in the Print Dialog.
        /// </summary>
        public string PrintDescription
        {
            get
            {
                return (string)this.GetValue(GridControlBase.PrintDescriptionProperty);
            }

            set
            {
                this.SetValue(GridControlBase.PrintDescriptionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the marign for the print page.
        /// </summary>
        public Thickness PrintPageMargin
        {
            get
            {
                return (Thickness)this.GetValue(GridControlBase.PrintPageMarginProperty);
            }
            set
            {
                this.SetValue(GridControlBase.PrintPageMarginProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.PrintPageMargin"/> property.
        /// </summary>
        public static readonly DependencyProperty PrintPageMarginProperty = DependencyProperty.Register("PrintPageMargin",typeof(Thickness),typeof(GridControlBase));
        #endregion
    }

    class GridPrintPageWrapper :DependencyObject
    {

        public int PageNumber
        {
            get { return (int)GetValue(PageNumberProperty); }
            set { SetValue(PageNumberProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="Binding PageNumber"/> property.
        /// </summary>
        public static readonly DependencyProperty PageNumberProperty =
            DependencyProperty.Register("PageNumber", typeof(int), typeof(GridPrintPageWrapper));
        public int TotalPage
        {
            get { return (int)GetValue(TotalPageProperty); }
            set { SetValue(TotalPageProperty, value); }
        }
        /// <summary>
        /// DependencyProperty for <see cref="Binding TotalPage"/> property.
        /// </summary>
        public static readonly DependencyProperty TotalPageProperty =
            DependencyProperty.Register("TotalPage", typeof(int), typeof(GridPrintPageWrapper));
        
    }
}

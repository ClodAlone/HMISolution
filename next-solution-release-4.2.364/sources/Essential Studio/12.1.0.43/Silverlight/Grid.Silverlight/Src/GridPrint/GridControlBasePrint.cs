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
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
// using System.Windows.Controls;
using System.Collections;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Documents;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Controls.Primitives;
using Syncfusion.Windows.Controls.Grid;
using System.Windows.Data;
using System.Windows.Printing;
using System.Windows.Markup;
using Syncfusion.Windows.Controls.Grid.GridUtils;
//using DB.Silverlight.Views;

namespace Syncfusion.Windows.Controls.Grid
{
    partial class GridControlBase : IGridPrintPaginator
    {
        private Size printSize = Size.Empty;

        

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            "HeaderTemplate", typeof(DataTemplate), typeof(GridControlBase), new PropertyMetadata(null));

        public DataTemplate PrintHeaderTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(GridControlBase.HeaderTemplateProperty);
            }

            set
            {
                this.SetValue(GridControlBase.HeaderTemplateProperty, value);
            }
        }

        public static readonly DependencyProperty FooterTemplateProperty = DependencyProperty.Register(
            "FooterTemplate",
            typeof(DataTemplate),
            typeof(GridControlBase),new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a data template for the print footer.
        /// </summary>
        public DataTemplate PrintFooterTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(GridControlBase.FooterTemplateProperty);
            }

            set
            {
                this.SetValue(GridControlBase.FooterTemplateProperty, value);
            }
        }

        private static readonly DependencyProperty PageCountProperty = DependencyProperty.Register(
            "PageCount",
            typeof(int),
            typeof(GridControlBase),new PropertyMetadata(null));

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
            var gridPrintSize = new Size(printPageSize.Width, Math.Round(printPageSize.Height - this.PrintHeaderHeight - this.PrintFooterHeight));
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


        private bool isDirty = true;

        List<int> rowinPage = new List<int>();
        List<int> colinPage = new List<int>();
        List<int> startRowinPage = new List<int>();
        List<int> startColinPage = new List<int>();        

        private int ComputePageCount(Size printPageSize)
        {
            rowinPage.Clear();
            colinPage.Clear();
            startRowinPage.Clear();
            startColinPage.Clear();
            if (!isDirty)
            {
                return this.PageCount;
            }
            
            this.isDirty = false;
            var counter0 = 0;
            var printRange = this.PrintRange;          
            var horizontalScrollAxis = this.ScrollColumns;
            var abortWidth = Math.Ceiling(printPageSize.Width);
            var verticalScrollAxis = this.ScrollRows;
            var abortHeight = Math.Ceiling(printPageSize.Height);         
            var rowOrigin = 0d;
            var reset = false;
            var colReset = false;
            var reachedEnd = false;
            var leftRange = printRange.Left;
            var tLeftRange = 0;
            var topRange = printRange.Top;
            var tTopRange = 0;
            int rowCount = 0;
            int colCount = 0;
            var lTopRange = printRange.Top;
            var bottomRange = printRange.Bottom;
            for (int r = topRange; r <= bottomRange; r++)
            {
                if (reset)
                {
                    reset = false;
                   // col[counter0] = colCount + 1;
                    colinPage.Add(colCount + 1);                   
                    colCount = 0;
                    rowinPage.Add(rowCount + 1);
                    // pag[counter0] = rowCount + 1;                    
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
                    rowCount=0;                   
                }

                var vertHeight = verticalScrollAxis.GetLineSize(r);
               
                var colOrigin = 0d;                 
                 
                for (int c = leftRange; c <= printRange.Right; c++)
                {
                    var horzWidth = horizontalScrollAxis.GetLineSize(c);

                    //Checking whethether the ColumnWidth exceeds to Total Page Width
                    if (horzWidth > abortWidth && colOrigin == 0 || horzWidth > abortWidth && colOrigin < abortWidth / 2)
                    { }

                    else if (colOrigin + horzWidth > abortWidth)
                    {
                        colReset = true;
                        tLeftRange = c;
                        break;
                    }
                    colCount = c - leftRange;                    
                    colOrigin += horzWidth;                    
                } 
                if (rowOrigin + vertHeight > abortHeight)
                {
                    reset = true;
                    tTopRange = r;
                    r -= 1; // minus 1 since it will be ++ in the loop
                    continue;
                }
                if (r == printRange.Bottom)
                {
                    if (colReset)
                    {
                        if (tTopRange + 1 != r)
                        {
                            r = tTopRange - 1;// minus 1 since it will be ++ in the loop
                        }
                        reset = true;
                        reachedEnd = true;
                        continue;
                    }
                    else
                    {
                        rowinPage.Add(rowCount + 1);
                        colinPage.Add(colCount + 1);
                        //pag[counter0] = rowCount + 1;
                        //col[counter0] = colCount + 1;
                        rowCount = rowCount + 1;                       
                        break;
                    }
                }
               rowCount=rowCount+1;            
                rowOrigin += vertHeight;
            }
            var totalPageCount = counter0 + 1;
            this.SetValue(GridControlBase.PageCountProperty, totalPageCount);
           
            int totalcol=0;
            for (int k = 0; k < colinPage.Count; k++)
            {
                if (k == 0)
                {
                    startRowinPage.Add(0);
                    startColinPage.Add(0);
                    totalcol = colinPage.ElementAt(k); //col[k];
                }
                else
                {
                    if (totalcol < this.Model.ColumnCount)
                    {
                        startColinPage.Add(startColinPage.ElementAt(k - 1) + colinPage.ElementAt(k - 1)); //col[k-1];
                        startRowinPage.Add(startRowinPage.ElementAt(k - 1));
                        totalcol = totalcol + colinPage.ElementAt(k);//col[k];
                    }
                    else
                    {
                        startRowinPage.Add(startRowinPage.ElementAt(k - 1) + rowinPage.ElementAt(k - 1));// pag[k-1];
                        startColinPage.Add( startColinPage.ElementAt(0));
                        totalcol = colinPage.ElementAt(k);
                    }

                }
            }
                //return this.PageCount;
            return startRowinPage.Count;
        }

        System.Windows.Controls.StackPanel IGridPrintPaginator.GetPrintVisualAt(int pageNumber)
        { 
            System.Windows.Controls.StackPanel gt=null; 
            System.Windows.Controls.StackPanel sta = new System.Windows.Controls.StackPanel();
            for (int i = startRowinPage.ElementAt(pageNumber); i < startRowinPage.ElementAt(pageNumber) + rowinPage.ElementAt(pageNumber) && i < this.Model.RowCount; i++)//pag[pageNumber] && i < this.Model.RowCount; i++)
            {
                int k = 0;
                gt = new System.Windows.Controls.StackPanel();
                for (int j = startColinPage.ElementAt(pageNumber); j < startColinPage.ElementAt(pageNumber) + colinPage.ElementAt(pageNumber) && j < this.Model.ColumnCount; j++) //col[pageNumber] && j < this.Model.ColumnCount; j++)
                {
                    Border br = new System.Windows.Controls.Border();
                    br.BorderBrush = Brushes.Black;
                    br.Width = this.ScrollColumns.GetLineSize(j);
                    br.Height = this.ScrollRows.GetLineSize(i);
                    br.Background = this.Model[i, j].Background;
                    br.BorderThickness = new Thickness(0.25);
                    if (this.Model[i, j].CellType == "RichText")
                    {
                      
                        RichTextBox Richtextbox = new System.Windows.Controls.RichTextBox();
                        if (this.Model[i, j].CellValue is Paragraph)
                        Richtextbox.Blocks.Add(XamlReader.Load(GridUtility.ConvertParagraphToXaml(this.Model[i, j].CellValue as Paragraph)) as Paragraph);
                        else if (this.Model[i, j].CellValue is List<Paragraph>)
                        {
                            foreach (Paragraph para in this.Model[i, j].CellValue as List<Paragraph>)
                            {
                                Richtextbox.Blocks.Add(XamlReader.Load(GridUtility.ConvertParagraphToXaml(para)) as Paragraph);
                            }
                        }
                        br.Child = Richtextbox;
                    }
                    else
                    {
                        TextBlock tb = new TextBlock();  
                        // br.Margin = new Thickness(0.5);                   
                        //tb.Width = this.Model.ColumnWidths.DefaultLineSize;
                        // tb.Height = this.Model.RowHeights.DefaultLineSize;
                        tb.FontFamily = FontFamily;
                        tb.FontSize = FontSize;
                        tb.FontStretch = FontStretch;
                        tb.FontStyle = FontStyle; //FontStyle;
                        tb.Foreground = this.Model[i, j].Foreground;
                        tb.VerticalAlignment = this.Model[i, j].VerticalAlignment;
                        tb.HorizontalAlignment = this.Model[i, j].HorizontalAlignment;//HorizontalAlignment;
                        // tb.Width = 70;                    
                        if (this.Model[i, j].CellValue != null)
                        {
                            GridStyleInfo style = this.Model[i, j] as GridStyleInfo;
                            tb.Text = style.GetFormattedText(style.CellValue);
                            if (tb.Text == "Syncfusion.Windows.Controls.Grid.GridDataChildTableModel")
                            {
                                //var st = new StackPanel();
                                //var nested = (GridDataCellNestedGridModel)this.Model[i, j].CellValue;
                                //var count= nested.GridModel.RowCount;
                                var NestedModel = (Syncfusion.Windows.Controls.Grid.GridDataChildTableModel)this.Model[i, j].CellValue;
                                var count = NestedModel.Grid.Model.RowCount;
                                var RowStack = new StackPanel();
                                RowStack.Orientation = System.Windows.Controls.Orientation.Vertical;
                                for (int l = 0; l < NestedModel.Grid.Model.RowCount; l++)
                                {
                                    var ColumnStack = new StackPanel();
                                    ColumnStack.Orientation = System.Windows.Controls.Orientation.Horizontal;
                                    for (int m = 0; m < NestedModel.Grid.Model.ColumnCount; m++)
                                    {

                                        TextBlock tb1 = new TextBlock();
                                        Border br1 = new System.Windows.Controls.Border();
                                        br1.BorderBrush = Brushes.Black;

                                        br1.Width = this.ScrollColumns.GetLineSize(m + 1);
                                        br1.Height = this.ScrollRows.GetLineSize(0);
                                        br1.Background = NestedModel.Grid.Model[l, m].Background;

                                        br1.BorderThickness = new Thickness(0.25);
                                        tb1.Text = NestedModel.Grid.Model[l, m].GetFormattedText(NestedModel.Grid.Model[l, m].CellValue);
                                        tb1.HorizontalAlignment = NestedModel.Grid.Model[l, m].HorizontalAlignment;
                                        //tb1.VerticalAlignment=NestedModel.Grid.Model
                                        br1.Child = tb1;
                                        ColumnStack.Children.Add(br1);
                                    }
                                    RowStack.Children.Add(ColumnStack);
                                }

                                gt.Children.Add(RowStack);
                                // var NestedGrid = (GridCellNestedGridModel)this.Model[i, j]).CellValue;
                                // var source = (((Syncfusion.Windows.Controls.Grid.GridDataTableModel)(this.Model[i, j].CellValue)).SourceList);
                                break;
                            }
                        }
                        else
                        {
                            tb.Text = "";
                        }
                        //if (tb.Text == string.Empty)
                        //{
                        //    br.Height = 20;
                        //}
                        br.Background = this.Model[i, j].Background;
                        br.Child = tb;
                    }
                    gt.Orientation = System.Windows.Controls.Orientation.Horizontal;                   
                    gt.Children.Add(br);
                    k++;                    
                }
                sta.Children.Add(gt); 
            }
            return sta;           
        }       
       

        //FrameworkElement IGridPrintPaginator.GetPrintVisualAt(int pageNumber)
        //{
        //    return null;
           // TextBlock tb = new TextBlock();
           // if (this.pageSizes.Count == 0 || !this.pageSizes.ContainsKey(pageNumber))
           // {
           //     return null;
           // }         
 
           // //columnList
           // //pag
           // //col
           // //cellsinpa
           //   int startcells;
           // if(pageNumber>0)
           // {
           //   startcells=cellsinpa[pageNumber-1]+1;
           // }
           // else
           // {
           //     startcells=0;
           // }
           // int rowcountinpage = pag[pageNumber];
           // int columninpage = col[pageNumber];
           // double Cellsinpage = rowcountinpage * columninpage;
           // for (int i = 0; i < Cellsinpage; i++)
           // {
           //     for (int j = startcells; j < cellsinpa[pageNumber]; j++)
           //     {
           //         tb.Width = columnList[i].Size;
           //         tb.Text = this.Model[columnList[i].LineIndex, columnList[i].LineIndex].CellValue.ToString();
           //     }
                

           // }

           // ScrollControlDrawingVisual drawingVisual = this.GetChildVisual(true, true, true, true);           
           
           // DrawingContext dc = drawingVisual.RenderOpen();

           // var page = this.pageSizes[pageNumber];
            
           //// var val=(page).entries[0];
           // var coveredRangeList = new GridRangeInfoList();
      
           // foreach (var kvp in page)
           // {
           //     var visibleRow = kvp.Key;
           //     //Rect cellRect = new Rect(0, visibleRow.Origin, this.printSize.Width, visibleRow.Size);
           //     foreach (var visibleColumn in kvp.Value)
           //     {
           //         var currentCellRange = GridRangeInfo.Cell(visibleRow.LineIndex, visibleColumn.LineIndex);
           //         if (coveredRangeList.AnyRangeIntersects(currentCellRange))
           //         {
           //             continue;
           //         }                   
           //         var cc = this.CoveredCells.GetCellSpan(visibleRow.LineIndex, visibleColumn.LineIndex);
           //         if (cc != null)
           //         {
           //             currentCellRange = GridRangeInfo.Cells(cc.Top, cc.Left, cc.Bottom, cc.Right);
           //             coveredRangeList.Add(currentCellRange);
           //         }
           //         var renderStyle = this.GetRenderStyleInfo(visibleRow.LineIndex, visibleColumn.LineIndex);
           //         DoubleSpan[] yCurrentCellPos = ScrollRows.RangeToRegionPoints(currentCellRange.Top, currentCellRange.Bottom, true);
           //         DoubleSpan[] xCurrentCellPos = ScrollColumns.RangeToRegionPoints(currentCellRange.Left, currentCellRange.Right, true);
           //         var rowRegion = 1;
           //         var columnRegion = 1;

           //         if (yCurrentCellPos[rowRegion].IsEmpty)
           //         {
           //             continue;
           //         }

           //         if (xCurrentCellPos[columnRegion].IsEmpty)
           //         {
           //             continue;
           //         }

           //         Rect clipRect = GetClipRect((ScrollAxisRegion)rowRegion, (ScrollAxisRegion)columnRegion);
           //         //Rect r = new Rect(xCurrentCellPos[columnRegion].Start, yCurrentCellPos[rowRegion].Start, xCurrentCellPos[columnRegion].Length, yCurrentCellPos[rowRegion].Length);
           //         Rect r = new Rect(visibleColumn.Origin, visibleRow.Origin, xCurrentCellPos[columnRegion].Length, yCurrentCellPos[rowRegion].Length);
           //         //var cellRect = r;
                    
           //        // dc.DrawRectangle(new SolidColorBrush(Colors.Transparent), null, r);
           //         this.RenderBorder(dc, r, clipRect, CellBorderSide.Top, renderStyle.Borders.Top);
           //         this.RenderBorder(dc, r, clipRect, CellBorderSide.Left, renderStyle.Borders.Left);
           //         this.RenderBorder(dc, r, clipRect, CellBorderSide.Right, renderStyle.Borders.Right);
           //         this.RenderBorder(dc, r, clipRect, CellBorderSide.Bottom, renderStyle.Borders.Bottom);

           //         var rca = new RenderCellArgs(this, visibleRow, visibleColumn, r, renderStyle);
           //         if (rca.CellUIElements != null)
           //         {
           //             foreach (UIElement el in rca.CellUIElements.UIElements)
           //             {
           //                // Brush nd;
                            
           //                // Brush vb = new Brush(el) { Stretch = Stretch.None, AlignmentX = AlignmentX.Left, AlignmentY = AlignmentY.Top };
           //                 Rect rect = GridCellTextBoxCellRenderer .GetBounds(el);                            
           //                 rect = new Rect(r.X, r.Y, rect.Width, rect.Height);
           //                 dc.DrawRectangle(new SolidColorBrush(Colors.Transparent), null, rect);
           //             }
           //             var renderer = this.CellRenderers[renderStyle.ModelStyle.CellType];
           //             renderer.Render(dc, rca);
           //         }
           //         else
           //         {
           //             // var renderer = this.CellRenderers["Static"];
           //             var renderer = this.CellRenderers[renderStyle.ModelStyle.CellType];
           //             renderer.Render(dc, rca);
           //         }
           //     }
           // }
           // dc.Close();
           // //generate grid
           // System.Windows.Controls.Grid gridPanel = new System.Windows.Controls.Grid();
           // gridPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(this.PrintHeaderHeight) });
           // var gridPrintSize = new Size(this.printSize.Width, this.printSize.Height - this.PrintHeaderHeight - this.PrintFooterHeight);
           // gridPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(Math.Round(gridPrintSize.Height)) });
           // gridPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(this.PrintFooterHeight) });

           // if (this.PrintHeaderHeight > 0)
           // {
           //     var headerEl = this.PrintHeaderTemplate.LoadContent() as FrameworkElement;
           //     gridPanel.Children.Add(headerEl);
           //     System.Windows.Controls.Grid.SetRow(headerEl, 0);
           // }

            
           // //body  
           // //var chil = drawingVisual;
           // //gridPanel.Children.Remove(drawingVisual);
           // //gridPanel.Children.Add(drawingVisual);
           // //gridPanel.Children.Add(drawingVisual);
           // System.Windows.Controls.Grid.SetRow(drawingVisual, 1);
           
           // //gridPanel.Children.Add(dvSelectBorde);
           // //System.Windows.Controls.Grid.SetRow(dvSelectBorde, 1);

           // //footer
           // if (this.PrintFooterHeight > 0)
           // {
           //     var footerEl = this.PrintFooterTemplate.LoadContent() as FrameworkElement;
           //     gridPanel.Children.Add(footerEl);
           //     System.Windows.Controls.Grid.SetRow(footerEl, 2);
           // }

           // gridPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
           // // Layout pass.
           // gridPanel.Arrange(new Rect(new Point(0, 0), this.printSize));
          //  return gridPanel;           
       // }
        
        //private void RenderBorder(DrawingContext dc, Rect cellRect, Rect clipRect, CellBorderSide borderSide, Pen pen1)
        //{
        //    Pen pen = new Pen(Model.Options.CurrentCellBorder, Model.Options.CurrentCellBorderWidth);
        //    if (cellRect.Width == 0)
        //        return;

        //    //if (cellRect != clipRect)
        //    //{
        //    //    cellRect = clipRect;
        //    //}

        //    switch (borderSide)
        //    {
        //        case CellBorderSide.Top:

        //            Point topLeft = ConvertPoint(cellRect.Top, cellRect.Left);
        //            Point topRight = ConvertPoint(cellRect.Top, cellRect.Right);
        //            dc.DrawLine(pen, topLeft, topRight);
        //            break;

        //        case CellBorderSide.Bottom:

        //            Point bottomLeft = ConvertPoint(cellRect.Bottom, cellRect.Left);
        //            Point bottomRight = ConvertPoint(cellRect.Bottom, cellRect.Right);
        //            dc.DrawLine(pen, bottomLeft, bottomRight);
        //            break;

        //        case CellBorderSide.Left:

        //             topLeft = ConvertPoint(cellRect.Top, cellRect.Left);
        //             bottomLeft = ConvertPoint(cellRect.Bottom, cellRect.Left);
        //            dc.DrawLine(pen, topLeft, bottomLeft);
        //            break;

        //        case CellBorderSide.Right:

        //            topRight = ConvertPoint(cellRect.Top, cellRect.Right);
        //            bottomRight = ConvertPoint(cellRect.Bottom, cellRect.Right);
        //            dc.DrawLine(pen, topRight, bottomRight);
        //            break;
        //    }
        //}
        //private Point ConvertPoint(double val1, double val2)
        //{
        //    Point val = new Point(0, 0);
        //    val.X = val1;
        //    val.Y = val2;
        //    return val;
        //}

        public static readonly DependencyProperty HeaderHeightProperty = DependencyProperty.Register(
            "HeaderHeight",
            typeof(double),
            typeof(GridControlBase),new PropertyMetadata(null));

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
            typeof(GridControlBase),new PropertyMetadata(null));

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
            typeof(GridControlBase),new PropertyMetadata(null));

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

    }
}

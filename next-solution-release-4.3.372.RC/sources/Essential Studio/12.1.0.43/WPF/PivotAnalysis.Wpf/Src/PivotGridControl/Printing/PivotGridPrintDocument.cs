#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Documents;
using Syncfusion.Windows.Tools.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Syncfusion.Windows.Controls.PivotGrid
{
    /// <summary>
    /// Class that holds the members that print the pages in pivot grid view.
    /// </summary>
    public class PivotGridPrintDocument: IPrintDocument
    {
        private PivotGridControl pivotGrid;
        private IGridPrintPaginator gridPaginator;
      
        /// <summary>
        /// A constructor that initializes the print pages with pivot grid view.
        /// </summary>
        /// <param name="grid">PivotGridControl</param>
        public PivotGridPrintDocument(PivotGridControl grid)
        {
            this.pivotGrid = grid;
            gridPaginator = this.pivotGrid.InternalGrid.Model.ActiveGridView as IGridPrintPaginator;
        }

        #region IPrintDocument members
        /// <summary>
        /// A method that returns print visual of the respective page.
        /// </summary>
        /// <param name="pageNo">page number</param>
        /// <returns>FrameworkElement</returns>
        public FrameworkElement GetPage(int pageNo)
        {
            FrameworkElement element = gridPaginator.GetPrintVisualAt(pageNo, this.PageSize);
            if (this.pivotGrid.ShowGroupingBar)
            {
                System.Windows.Controls.Grid grid = element as System.Windows.Controls.Grid;
                var drawingVisual = new GridPrintVisual();
                DrawingContext dc = drawingVisual.ContentVisual.RenderOpen();
                double width = this.PrintablePageSize.Width - this.Margin.Left - this.Margin.Right;
                Rect r = new Rect(0, 0, width, 70);
                VisualBrush vb = new VisualBrush(this.pivotGrid.GroupingBar) { AlignmentX = AlignmentX.Left, AlignmentY = AlignmentY.Top };
                dc.DrawRectangle(vb, null, r);
                dc.Close();
                grid.RowDefinitions.Insert(0, new RowDefinition() { Height = new GridLength(r.Height) });
                grid.RowDefinitions[2].Height = new GridLength(this.PrintablePageSize.Height - r.Height);
                grid.Children.Insert(0, drawingVisual);
                System.Windows.Controls.Grid.SetRow(drawingVisual, 0);
                element = grid;
            }
            return element;
        }

        /// <summary>
        /// Gets or sets the thickness of outer border of framework element.
        /// </summary>
        public Thickness Margin
        {
            get;
            set;
        }

        /// <summary>
        /// A method that sets the size of print page and count of total pages.
        /// </summary>
        public void OnSetPageSize()
        {
            this.gridPaginator.SetPrintPageSize(this.PrintablePageSize);
            this.TotalPages = this.gridPaginator.GetPrintTotalPageCount(this.PrintablePageSize);
        }

        /// <summary>
        /// Gets or sets the size of the print page.
        /// </summary>
        public Size PageSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the client size of the print page.
        /// </summary>
        public Size PrintablePageSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the total pages to print.
        /// </summary>
        public int TotalPages
        {
            get;
            set;
        }
        #endregion

    }
}

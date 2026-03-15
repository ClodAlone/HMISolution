#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Documents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Documents;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// A helper class to support creation of multiple page elements from a single grid table.
    /// </summary>
    public class GridPrintTablePaginator : DocumentPaginator
    {
        private PrintDialog printDialog;

        /// <summary>
        /// Initializes a new <see cref="GridPrintTablePaginator"/>.
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        /// <param name="printPaginator">The <see cref="IGridPrintPaginator"/> instance.</param>
        public GridPrintTablePaginator(Size pageSize, IGridPrintPaginator printPaginator, PrintDialog printDialog)
        {
            this.PrintPaginator = printPaginator;
            this.pageSize = pageSize;
            this.printDialog = printDialog;
        }

        /// <summary>
        /// Returns the page whose page number is given.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <returns>Document page.</returns>
        public override DocumentPage GetPage(int pageNumber)
        {
            var visual = this.PrintPaginator.GetPrintVisualAt(pageNumber, PageSize);
            if (visual != null)
            {
                visual.UpdateLayout();
                //get selected printer capabilities
                System.Printing.PrintCapabilities capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);
                //get scale of the print wrt to screen of WPF visual
                double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / this.PageSize.Width, capabilities.PageImageableArea.ExtentHeight /
                               this.PageSize.Height);
                ////Transform the Visual to scale
                visual.LayoutTransform = new ScaleTransform(scale, scale);
                //get the size of the printer page
                Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
                //update the layout of the visual to the printer page size.
                visual.Measure(sz);
                visual.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));
                var page = new DocumentPage(visual, this.PageSize, Rect.Empty, new Rect(0, 0, visual.DesiredSize.Width, visual.DesiredSize.Height));
                return page;
            }

            return null;
        }

        /// <summary>
        /// Specifies whether the page count is valid.
        /// </summary>
        public override bool IsPageCountValid
        {
            get { return this.PrintPaginator != null && this.PageCount > 0; }
        }

        /// <summary>
        /// Gets the number of pages.
        /// </summary>
        public override int PageCount
        {
            get { return !this.pageSize.IsEmpty ? this.PrintPaginator.GetPrintTotalPageCount(this.pageSize) : 0; }
        }

        private Size pageSize;
        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public override System.Windows.Size PageSize
        {
            get
            {
                return this.pageSize;
            }
            set
            {
                this.pageSize = value;
            }
        }

        /// <summary>
        /// Gets the source object that performs tha actual content pagination.
        /// </summary>
        public override IDocumentPaginatorSource Source
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the print paginator.
        /// </summary>
        public IGridPrintPaginator PrintPaginator
        {
            get;
            private set;
        }
    }
}

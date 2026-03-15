#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Documents;
using System.Windows.Printing;

namespace Syncfusion.Windows.Controls.Grid
{
    public class GridPrintTablePaginator
    {

        private PrintDocument printDoc;
        public GridPrintTablePaginator(IGridPrintPaginator printPaginator, PrintDocument printDoc)
        {
            this.PrintPaginator = printPaginator;
            //this.pageSize = pageSize;
            this.printDoc = printDoc;
        }
       

        //public DocumentPage GetPage(int pageNumber)
        //{
        //    var visual = this.PrintPaginator.GetPrintVisualAt(pageNumber);
        //    if (visual != null)
        //    {
        //        //get selected printer capabilities
        //        // System.Printing.PrintCapabilities capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);
        //        //get scale of the print wrt to screen of WPF visual
        //        //double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / this.PageSize.Width, capabilities.PageImageableArea.ExtentHeight /
        //        //                this.PageSize.Height);
        //        ////Transform the Visual to scale
        //        //visual.LayoutTransform = new ScaleTransform(scale, scale);
        //        //get the size of the printer page
        //        Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
        //        //update the layout of the visual to the printer page size.
        //        visual.Measure(sz);
        //        visual.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));
        //        var page = new DocumentPage(visual, this.PageSize, Rect.Empty, new Rect(0, 0, visual.DesiredSize.Width, visual.DesiredSize.Height));
        //        return page;
        //    }

        //    return null;
        //}

        public IGridPrintPaginator PrintPaginator
        {
            get;
            private set;
        }

    }
}

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
using System.Windows.Printing;
using Syncfusion.Windows.Documents;
using System.Windows.Data;


namespace Syncfusion.Windows.Controls.Grid
{
    public  class GridPrintDialog
    {
        private GridControlBase grid;
        
        public  GridPrintDialog(GridControlBase grid)
        {
            
            this.grid = grid;            
            this.grid.PrintRange = GridRangeInfo.Table();
           // printdoc.BeginPrint+=new EventHandler<BeginPrintEventArgs>(printdoc_BeginPrint);
            //this.PrintSize = new System.Windows.Size(printdoc.

        }
        public GridPrintDialog()
        {            
            //printdoc.BeginPrint += new EventHandler<BeginPrintEventArgs>(printdoc_BeginPrint);

        }
         PrintDocument printdoc=new PrintDocument();
         
        public void  ShowPrintDialog(string docname)
        {
            int i = 0;
            int count = 0;
            PrintDocument pd = new PrintDocument();
            //var gcb = new GridControlBase();
            //this.PrintSize = new System.Windows.Size(pd.PrintableArea.Width, pd.PrintableArea.Width);
            //int ItemIndex = 0;
            
            pd.PrintPage += (s, pe) =>
            {
              
                var printPaginator = (IGridPrintPaginator)this.grid;
                //var tablePaginator = new GridPrintTablePaginator(printPaginator, printdoc);               
                var printpaginator = this.grid as IGridPrintPaginator;
                this.PrintSize = new System.Windows.Size(pe.PrintableArea.Width, pe.PrintableArea.Height);
                printpaginator.SetPrintPageSize(PrintSize);
                if (count == 0)
                {
                    count = printpaginator.GetPrintTotalPageCount(PrintSize);
                }
                if (i < count)
                {                
                    StackPanel itemHost = printpaginator.GetPrintVisualAt(i);
                    pe.HasMorePages = (i < count);
                    pe.PageVisual = itemHost;
                    i++;
                }
            };
            pd.Print(docname);
            //printdoc.PrintPage += new EventHandler<PrintPageEventArgs>(printDocument_PrintPage);
            //var printPaginator = (IGridPrintPaginator)this.grid;
            //var tablePaginator = new GridPrintTablePaginator(printPaginator, printdoc);
           // printdoc.Print(docname); 
        }

        //private static readonly DependencyProperty PrintSizeProperty = DependencyProperty.Register(
        //    "PrintSize",
        //    typeof(System.Windows.Size),
        //    typeof(GridPrintDialog),
        //    new PropertyMetadata(OnPrintSizeChanged));

        // private static void OnPrintSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        //{
        //    var gridPrintDlg = d as GridPrintDialog;
        //    gridPrintDlg.OnPrintSizeChanged((Size)args.NewValue);
        //}

        public System.Windows.Size PrintSize;
        //{
        //    get
        //    {
        //        return (System.Windows.Size)this.GetValue(GridPrintDialog.PrintSizeProperty);
        //    }

        //    set
        //    {
        //        this.SetValue(GridPrintDialog.PrintSizeProperty, value);
        //    }
        //}

        //protected void printdoc_BeginPrint(object sender, BeginPrintEventArgs e)
        //{
        //    var printPaginator = (IGridPrintPaginator)this.grid;
        //    var tablePaginator = new GridPrintTablePaginator( printPaginator, printdoc);
            
        //}
        ////protected void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        //{
        //   // LoopThroughControls(this.grid);
        //    var printPaginator = (IGridPrintPaginator)this.grid;
        //    var tablePaginator = new GridPrintTablePaginator(printPaginator, printdoc);
        //   // PrintPage page = new PrintPage();
        //    var pageRoot = new Canvas();
        //    var printpaginator= this.grid as IGridPrintPaginator;
        //    this.PrintSize = new System.Windows.Size(e.PrintableArea.Width,e.PrintableArea.Height);
        //    printpaginator.SetPrintPageSize(PrintSize);
        //    var count = printpaginator.GetPrintTotalPageCount(PrintSize);
        //    StackPanel st = new StackPanel();
        //   // var visual = printpaginator.GetPrintVisualAt1(0, this.grid);
        //    //for (int i = 0; i < count; i++)
        //    //{
        //    while (i < count)
        //    {
        //        var visual = printpaginator.GetPrintVisualAt1(i, this.grid);
        //        e.HasMorePages = (i <= count);
        //        e.PageVisual = visual;
        //        i++;
        //    }
               
        //   // }
        //    //for (int i = 0; i < count; i++)
        //    //{
        //    //    //pageRoot.Children.Add(printpaginator.GetPrintVisualAt1(i, this.grid));
        //    //    st.Children.Add(printpaginator.GetPrintVisualAt1(i, this.grid));
        //    //}
        //    //    //var visual = printpaginator.GetPrintVisualAt(0);
        //        // e.PageVisual = pageRoot;
        //        // pageRoot.Children.Add(visual);
        //        //e.PageVisual = visual;  
        //    //e.PageVisual = visual;
            
        //}
    }
}

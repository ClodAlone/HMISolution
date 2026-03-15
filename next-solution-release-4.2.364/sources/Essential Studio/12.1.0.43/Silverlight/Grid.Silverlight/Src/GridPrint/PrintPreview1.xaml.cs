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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Printing;

namespace Syncfusion.Windows.Controls.Grid.GridPrint
{
    public partial class PrintPreview1 : ChildWindow
    {
        private GridControlBase grid;
        public PrintPreview1(GridControlBase grid)
        {
            InitializeComponent();
            this.grid = grid;
            this.PrintSize = new Size(793, 1122); 
            SetPage(0);
            this.PageDecrease.IsEnabled = false;            
            this.FirstPage.IsEnabled = false;
            if (PageCount == 1)
            {
                this.LastPage.IsEnabled = false;
                this.PageIncrease.IsEnabled = false;
            }
            this.Show();
        }
       

        private void PageDecrease_Click(object sender, RoutedEventArgs e)
        {
            this.LastPage.IsEnabled = true;
            this.PageIncrease.IsEnabled = true;
            int currentPage = Int32.Parse(CurrentPageBox.Text);
            var printPaginator = (Syncfusion.Windows.Documents.IGridPrintPaginator)this.grid;          
            printPaginator.PrintRange = GridRangeInfo.Table();
            var count = printPaginator.GetPrintTotalPageCount(this.PrintSize);
            if (currentPage > 1)
            {
                var visual = printPaginator.GetPrintVisualAt(currentPage - 2);
                this.layoutContent.Content = visual;
                CurrentPageBox.Text = (currentPage-1).ToString();
            }
            else
            {
                this.FirstPage.IsEnabled = false;
                this.LastPage.IsEnabled = true;
                this.PageDecrease.IsEnabled = false;
                this.PageIncrease.IsEnabled = true;
            }
            
        }
        private void PageIncrease_Click(object sender, RoutedEventArgs e)
        {
            this.FirstPage.IsEnabled = true;
            this.PageDecrease.IsEnabled = true;
            int currentPage = Int32.Parse(CurrentPageBox.Text);
            var printPaginator = (Syncfusion.Windows.Documents.IGridPrintPaginator)this.grid;           
            printPaginator.PrintRange = GridRangeInfo.Table();
            PageCount = printPaginator.GetPrintTotalPageCount(this.PrintSize);
            if (PageCount > currentPage)
            {
                var visual = printPaginator.GetPrintVisualAt(currentPage);
                this.layoutContent.Content = visual;
                CurrentPageBox.Text = (currentPage + 1).ToString();
            }
            else
            {
                this.LastPage.IsEnabled = false;
                this.FirstPage.IsEnabled = true;
                this.PageIncrease.IsEnabled = false;
                this.PageDecrease.IsEnabled = true;
            }
        }
        // int count=0;
        private void OnPrintClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            int i = 0;
           
            PrintDocument pd = new PrintDocument();
            var printPaginator = (Syncfusion.Windows.Documents.IGridPrintPaginator)this.grid;
            var printpaginator = this.grid as Syncfusion.Windows.Documents.IGridPrintPaginator;
            printpaginator.SetPrintPageSize(this.PrintSize);

            pd.PrintPage += (s, pe) =>
            {               
                if (PageCount == 0)
                {
                    PageCount = printpaginator.GetPrintTotalPageCount(this.PrintSize);
                }
                if (i < PageCount)
                {
                    StackPanel itemHost = printpaginator.GetPrintVisualAt(i);
                    pe.HasMorePages = (i < PageCount);
                    pe.PageVisual = itemHost;
                    i++;
                }
            };
            pd.Print("Printing");
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void PageSizeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PageSizeCombo != null)
            {
                this.CurrentPageBox.Text = "1";
                if (PageSizeCombo.SelectedIndex == 0)
                {
                    this.PrintSize = new Size(3178, 4493);
                    this.SetPage(0);
                }
                if (PageSizeCombo.SelectedIndex == 1)
                {
                    //PrintDocument pd = new PrintDocument();
                   
                    this.PrintSize = new Size(2245, 3178);  //559 × 864  A1
                    this.SetPage(0);
                }
                if (PageSizeCombo.SelectedIndex == 2)
                {
                    this.PrintSize = new Size(1587,2245);//420x594  A2
                    this.SetPage(0);
                }
                if (PageSizeCombo.SelectedIndex == 3)
                {
                    this.PrintSize = new Size(1122, 1587); //297x520  A3
                    this.SetPage(0);
                }
                if (PageSizeCombo.SelectedIndex == 4)
                {
                    this.PrintSize = new Size(793, 1122);//210x297  A4 2328 px x 3264
                    this.SetPage(0);
                }
                if (PageSizeCombo.SelectedIndex == 5)
                {
                    this.PrintSize = new Size(559, 773);//210x297  A4 2328 px x 3264
                    this.SetPage(0);
                }
                if (PageSizeCombo.SelectedIndex == 6)
                {
                    this.PrintSize = new Size(396, 559);//210x297  A4 2328 px x 3264
                    this.SetPage(0);
                }
                if (PageSizeCombo.SelectedIndex == 7)
                {
                    this.PrintSize = new Size(274, 396);//210x297  A4 2328 px x 3264
                    this.SetPage(0);
                }
            }

        }

        public int PageCount
        {
            get { return (int)GetValue(PageCountProperty); }
            internal set { SetValue(PageCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageCountProperty =
            DependencyProperty.Register("PageCount", typeof(int), typeof(PrintPreview1), new PropertyMetadata(1));

        public System.Windows.Size PrintSize
        {
            get
            {
                return (System.Windows.Size)this.GetValue(PrintPreview1.PrintSizeProperty);
            }

            set
            {
                this.SetValue(PrintPreview1.PrintSizeProperty, value);
            }
        }
        private static readonly DependencyProperty PrintSizeProperty = DependencyProperty.Register(
            "PrintSize",
            typeof(System.Windows.Size),
            typeof(PrintPreview1),
            new PropertyMetadata(OnPrintSizeChanged));

        private static void OnPrintSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridPrintDlg = d as PrintPreview1;
            gridPrintDlg.OnPrintSizeChanged((Size)args.NewValue);
        }
        private void OnPrintSizeChanged(Size size)
        {
            this.layoutContent.MinWidth = this.PrintSize.Width;
            this.layoutContent.MinHeight = this.PrintSize.Height;
        }

        
        private void SetPage(int page)
        {
            var printPaginator = (Syncfusion.Windows.Documents.IGridPrintPaginator)this.grid;            
            printPaginator.PrintRange = GridRangeInfo.Table();
            PageCount = printPaginator.GetPrintTotalPageCount(this.PrintSize);
            var visual = printPaginator.GetPrintVisualAt(page);
            //var visual = printPaginator.GetPrintVisualAt1();
            if (PageCount > 1)
                {
                this.LastPage.IsEnabled = true;
                this.PageIncrease.IsEnabled = true;
                }
            this.layoutContent.Content = visual;
        }

        private void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            if (Int32.Parse( CurrentPageBox.Text) > 1)
            {
                SetPage(0);
                this.CurrentPageBox.Text = "1";
                this.FirstPage.IsEnabled = false;
                this.PageDecrease.IsEnabled = false;
                this.PageIncrease.IsEnabled = true;
                this.LastPage.IsEnabled = true;
            }            

        }

        private void LastPage_Click(object sender, RoutedEventArgs e)
        {
            SetPage(PageCount - 1);
            this.CurrentPageBox.Text = PageCount.ToString();
            this.LastPage.IsEnabled = false;
            this.PageIncrease.IsEnabled = false;
            this.PageDecrease.IsEnabled = true;
            this.FirstPage.IsEnabled = true;
        }
    }
}


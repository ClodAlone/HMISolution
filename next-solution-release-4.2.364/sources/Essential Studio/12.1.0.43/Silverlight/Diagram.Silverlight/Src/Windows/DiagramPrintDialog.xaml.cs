#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Windows.Printing;
using System.Text;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Syncfusion.Windows.Diagram
{
    public partial class DiagramPrintDialog : ChildWindow
    {
        public DiagramPrintDialog()
        {
            this.DataContext = this;
            InitializeComponent();
            PART_Stretch.SelectedIndex = 0;
            _printDocument = new PrintDocument();
            _printDocument.PrintPage += new EventHandler<PrintPageEventArgs>(document_PrintPage);
            //Size(827, 1169) //A4
            PageWidth = 827;
            PageHeight = 1169;
            PageMargin = new Thickness(50);
            _process = new Point(0, 0);
        }
        Point _process;
        internal PrintDocument _printDocument;
        public string DocumentName
        {
            get { return (string)GetValue(DocumentNameProperty); }
            set { SetValue(DocumentNameProperty, value); }
        }
        public double PageWidth
        {
            get { return (double)GetValue(PageWidthProperty); }
            set { SetValue(PageWidthProperty, value); }
        }

        internal Size PreviewSize;
     

        // Using a DependencyProperty as the backing store for PageWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageWidthProperty =
            DependencyProperty.Register("PageWidth", typeof(double), typeof(DiagramPrintDialog), new PropertyMetadata(0d, new PropertyChangedCallback(OnPageWidthChanged)));
        public static readonly DependencyProperty DocumentNameProperty =
            DependencyProperty.Register("DocumentName", typeof(string), typeof(DiagramPrintDialog), new PropertyMetadata("Untitled", new PropertyChangedCallback(OnPageDocumentNameChanged)));
        public double PageHeight
        {
            get { return (double)GetValue(PageHeightProperty); }
            set { SetValue(PageHeightProperty, value); }
        }

        private double ContentWidth
        {
            get { return this.PageWidth - this.PageMargin.Left - this.PageMargin.Right; }
        }

        private double ContentHeight
        {
            get { return this.PageHeight - this.PageMargin.Top - this.PageMargin.Bottom; }
        }

        // Using a DependencyProperty as the backing store for PageHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageHeightProperty =
            DependencyProperty.Register("PageHeight", typeof(double), typeof(DiagramPrintDialog), new PropertyMetadata(0d, new PropertyChangedCallback(OnPageHeightChanged)));

        public Stretch PageStretch
        {
            get { return (Stretch)GetValue(PageStretchProperty); }
            set { SetValue(PageStretchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageStretch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageStretchProperty =
            DependencyProperty.Register("PageStretch", typeof(Stretch), typeof(DiagramPrintDialog), new PropertyMetadata(Stretch.Fill, OnPageStretchChanged));

        public Thickness PageMargin
        {
            get { return (Thickness)GetValue(PageMarginProperty); }
            set { SetValue(PageMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageMarginProperty =
            DependencyProperty.Register("PageMargin", typeof(Thickness), typeof(DiagramPrintDialog), new PropertyMetadata(new Thickness(0), new PropertyChangedCallback(OnPageMarginChanged)));

        private static void OnPageStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            DiagramPrintDialog dial = d as DiagramPrintDialog;
            switch (dial.PageStretch)
            {
                case Stretch.Fill:
                    dial.PART_Stretch.SelectedIndex = 0;
                    dial.CurrentPage = 1;
                    break;
                case Stretch.None:
                    dial.PART_Stretch.SelectedIndex = 1;
                    break;
                case Stretch.Uniform:
                    dial.PART_Stretch.SelectedIndex = 2;
                    dial.CurrentPage = 1;
                    break;
                case Stretch.UniformToFill:
                    dial.PART_Stretch.SelectedIndex = 3;
                    dial.CurrentPage = 1;
                    break;
            }
            dial.UpdateVisual();
            dial.SetPage();
        }

        public ImageSource PreviewSource
        {
            get { return (ImageSource)GetValue(PreviewSourceProperty); }
            set { SetValue(PreviewSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PreviewSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviewSourceProperty =
            DependencyProperty.Register("PreviewSource", typeof(ImageSource), typeof(DiagramPrintDialog), new PropertyMetadata(null));
        
        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentPage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register("CurrentPage", typeof(int), typeof(DiagramPrintDialog), new PropertyMetadata(1, OnCurrentPageChanged));

        private static void OnCurrentPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            DiagramPrintDialog dpd = d as DiagramPrintDialog;
            if (dpd.CurrentPage > dpd.PageCount)
            {
                dpd.CurrentPage = dpd.PageCount;
            }
            dpd.SetPage();
        }

        public int PageCount
        {
            get { return (int)GetValue(PageCountProperty); }
            internal set { SetValue(PageCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageCountProperty =
            DependencyProperty.Register("PageCount", typeof(int), typeof(DiagramPrintDialog), new PropertyMetadata(1));

        internal UIElement _Visual;

        private void UpdateVisual()
        {
            UpdatePageCount();
            if (PageStretch == Stretch.None)
            {
                SetPage();
            }
            else
            {
                SetVisual();
            }
        }

        private void OnPrintClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            _printDocument.Print(this.DocumentName);
        }
      internal void document_PrintPage(object sender, PrintPageEventArgs e)
        {
            switch (PageStretch)
            {
                case Stretch.Fill:
                case Stretch.Uniform:
                case Stretch.UniformToFill:
                    e.PageVisual = new Image() { Source = GetImage(_Visual, new Point(0, 0)), Width = e.PrintableArea.Width, Height = e.PrintableArea.Height, Stretch = PageStretch };
                    break;
                case Stretch.None:
                    e.PageVisual = new Image() { Source = GetImage(_Visual, _process), Width = e.PrintableArea.Width, Height = e.PrintableArea.Height, Stretch = PageStretch };
                    _process.X += e.PrintableArea.Width;
                    if (_process.X > this._Visual.DesiredSize.Width)
                    {
                        _process.X = 0;
                        _process.Y += e.PrintableArea.Height;
                    }

                    if ((_process.Y > this._Visual.DesiredSize.Height))
                    {
                        e.HasMorePages = false;
                    }
                    else
                    {
                        e.HasMorePages = true;
                    }
                    //e.PageVisual = new Image() { Source = GetImage(_Visual, new Point(0, 0)) };
                    break;
            }
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        internal void UpdatePageCount()
        {
            if(_Visual!=null)
            {
                double hor = Math.Ceiling(_Visual.DesiredSize.Width / ContentWidth);
                double ver = Math.Ceiling(_Visual.DesiredSize.Height / ContentHeight);
                Point _pos = new Point(0, 0);
                int count = 1;
                while (true)
                {
                    _pos.X += ContentWidth;
                    if (_pos.X >= _Visual.DesiredSize.Width)
                    {
                        _pos.X = 0;
                        _pos.Y += ContentHeight;
                    }

                    if ((_pos.Y >= this._Visual.DesiredSize.Height))
                    {
                        break;
                    }
                    count++;
                }
                PageCount = count;
            }
           
        }
      
        internal void SetVisual()
        {
           WriteableBitmap wb = GetImage(_Visual, new Point(0, 0));
            PreviewSource = wb;
        }

        internal void SetPage()
        {
          
            WriteableBitmap wb = GetImage(CurrentPage);      
        
            PreviewSource = wb;
        }

        private WriteableBitmap GetImage(UIElement _visual, Point _pos)
        {
            WriteableBitmap wb = new WriteableBitmap(_visual, new TranslateTransform() { X = -_pos.X, Y = -_pos.Y });            
            return wb;
        }
      
        private WriteableBitmap GetImage(int _pageNo)
        {
            Point _pos = new Point(0, 0);         
        
            for (int i = 1; i < _pageNo; i++)
            {
                _pos.X += ContentWidth;
                if (_pos.X > _Visual.DesiredSize.Width)
                {
                    _pos.X = 0;
                    _pos.Y += ContentHeight;
                }                
            }          
         
            WriteableBitmap wb = new WriteableBitmap((int)PreviewSize.Width,(int)PreviewSize.Height);
            wb.Render(_Visual, new TranslateTransform() { X = -_pos.X, Y = -_pos.Y });
            wb.Invalidate();
            return wb;
        }

        private void StretchChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            switch (combo.SelectedIndex)
            {
                case 0:
                    PageStretch = Stretch.Fill;
                    Multipagestack.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    PageStretch = Stretch.None;
                    Multipagestack.Visibility = Visibility.Visible;
                    CurrentPageBox.Text = this.CurrentPage.ToString();
                    UpdatePageCount();
                    break;
                case 2:
                    PageStretch = Stretch.Uniform;
                    Multipagestack.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    PageStretch = Stretch.UniformToFill;
                    Multipagestack.Visibility = Visibility.Collapsed;
                    break;
            }
        }
        #region PrintPreview Helping Methods

        private double Margin_L;
        private double Margin_T;
        private double Margin_R;
        private double Margin_B;
        private static void OnPageWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramPrintDialog diagramPrint = (DiagramPrintDialog)d;
            if (diagramPrint != null)
            {
                diagramPrint.PART_Image.Width = diagramPrint.ContentWidth;
                //if (diagramPrint.ContentWidth == 0)
                //{
                //    MessageBox.Show("Argument Out of Range- Provide corect range value to Width");
                //}
                //else
                //{
                    diagramPrint.page_Width.Text = e.NewValue.ToString();
                    diagramPrint.UpdatePageCount();
                ////}
            }
        }
        private static void OnPageDocumentNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
 
        }
        private static void OnPageHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramPrintDialog diagramPrint = (DiagramPrintDialog)d;
            if (diagramPrint != null)
            {
                diagramPrint.PART_Image.Height = diagramPrint.ContentHeight;
                diagramPrint.Page_Height.Text = e.NewValue.ToString();
                diagramPrint.UpdatePageCount();
            }
        }
        private static void OnPageMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramPrintDialog diagramPrint = (DiagramPrintDialog)d;
            if (diagramPrint != null)
            {
                diagramPrint.T_Margin.Text = ((Thickness)e.NewValue).Top.ToString();
                diagramPrint.L_Margin.Text = ((Thickness)e.NewValue).Left.ToString();
                diagramPrint.R_Margin.Text = ((Thickness)e.NewValue).Right.ToString();
                diagramPrint.B_Margin.Text = ((Thickness)e.NewValue).Bottom.ToString();
            }
        }
        private void page_Width_LostFocus(object sender, RoutedEventArgs e)
        {
            double num = 0;
            bool success = double.TryParse(((TextBox)sender).Text, out num);
            if (success)
            {
                bool check = ValidatePageValue("Width", num);
                if (check)
                {
                    this.PageWidth = num;
                    SetPage();
                }
                else
                {
                    page_Width.Text = this.PageWidth.ToString();
                }
            }
            else
            {
                page_Width.Text = this.PageWidth.ToString();
                
            }
        }

        private bool ValidatePageValue(string para,double value)
        {
            double pagevalue=0;
            switch (para)
            {
                case "Width":
                    pagevalue=value-this.PageMargin.Left - this.PageMargin.Right;                   
                    break;
                case "Height":
                     pagevalue=value-this.PageMargin.Top - this.PageMargin.Bottom;
                    break;

                case "Left":
                     pagevalue=this.PageWidth-value- this.PageMargin.Right;     
                    break;

                case "Right":
                    pagevalue=this.PageWidth-this.PageMargin.Left - value;    
                    break;
                case "Top":
                    pagevalue=this.PageHeight-value - this.PageMargin.Bottom;
                    break;

                case "Bottom":
                     pagevalue=this.PageHeight-value - this.PageMargin.Top;
                    break;
            }
            if(pagevalue<=0)
            {
                return false;
            }
            else
            {
                return true;
            }          
        
        }
        private void Page_Height_LostFocus(object sender, RoutedEventArgs e)
        {
            double num = 0;
            bool success = double.TryParse(((TextBox)sender).Text, out num);
            if (success)
            {
                bool check = ValidatePageValue("Height", num);
                if (check)
                {
                    this.PageHeight = num;
                    SetPage();
                }
                else
                {
                    Page_Height.Text = this.PageHeight.ToString();
                }
            }
            else
            {
                Page_Height.Text = this.PageHeight.ToString();
            }
        }

        private void T_Margin_LostFocus(object sender, RoutedEventArgs e)
        {

            double num = 0;
            bool success = double.TryParse(((TextBox)sender).Text, out num);
            if (success)
            {

                bool check = ValidatePageValue("Top", num);
                if (check)
                {
                    Margin_T = num;
                    ApplyMargin(new Thickness(this.PageMargin.Left, num, this.PageMargin.Right, this.PageMargin.Bottom));
                }
                else
                {
                    T_Margin.Text = this.PageMargin.Top.ToString();
                }
            }
            else
            {
                T_Margin.Text = this.PageMargin.Top.ToString();
            }
        }

        private void L_Margin_LostFocus(object sender, RoutedEventArgs e)
        {

            double num = 0;
            bool success = double.TryParse(((TextBox)sender).Text, out num);
            if (success)
            {
                bool check = ValidatePageValue("Left", num);
                if (check)
                {
                    Margin_L = num;
                    ApplyMargin(new Thickness(num, this.PageMargin.Top, this.PageMargin.Right, this.PageMargin.Bottom));
                }
                else
                {
                    L_Margin.Text = this.PageMargin.Left.ToString();
                }
            }
            else
            {
                L_Margin.Text = this.PageMargin.Left.ToString();
            }
        }

        private void R_Margin_LostFocus(object sender, RoutedEventArgs e)
        {

            double num = 0;
            bool success = double.TryParse(((TextBox)sender).Text, out num);
            if (success)
            {
                bool check = ValidatePageValue("Right", num);
                if (check)
                {
                    Margin_R = num;
                    ApplyMargin(new Thickness(this.PageMargin.Left, this.PageMargin.Top, num, this.PageMargin.Bottom));
                }
                else
                {
                    R_Margin.Text = this.PageMargin.Right.ToString();
                }
            }
            else
            {
                R_Margin.Text = this.PageMargin.Right.ToString();
            }
        }

        private void B_Margin_LostFocus(object sender, RoutedEventArgs e)
        {

            double num = 0;
            bool success = double.TryParse(((TextBox)sender).Text, out num);
            if (success)
            {
                bool check = ValidatePageValue("Bottom", num);
                if (check)
                {
                    Margin_B = num;
                    ApplyMargin(new Thickness(this.PageMargin.Left, this.PageMargin.Top, this.PageMargin.Right, num));
                }
                else
                {
                    B_Margin.Text = this.PageMargin.Bottom.ToString();
                }
            }
            else
            {
                B_Margin.Text = this.PageMargin.Bottom.ToString();
            }
        }

        private void ApplyMargin( Thickness NewMargin)
        {
            this.PageMargin = NewMargin;
        }

        private void PageDecrease_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage != 1)
            {
                CurrentPage -= 1;
                CurrentPageBox.Text = CurrentPage.ToString();
            }
        }
        private void PageIncrease_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage!=PageCount&&CurrentPage<PageCount)
            {
                CurrentPage += 1;
                CurrentPageBox.Text = CurrentPage.ToString();
            }
        }
        private void CurrentPage_TextChanged(object sender, TextChangedEventArgs e)
        {
            int num = 0;
            bool success = int.TryParse(((TextBox)sender).Text, out num);
            if (success)
            {
                if (num <= PageCount)
                {
                    CurrentPage = num;
                }
                else
                {
                    CurrentPageBox.Text = CurrentPage.ToString(); 
                }
            }
            else
            {
                CurrentPageBox.Text = CurrentPage.ToString();
            }
        }

        #endregion
    }
}


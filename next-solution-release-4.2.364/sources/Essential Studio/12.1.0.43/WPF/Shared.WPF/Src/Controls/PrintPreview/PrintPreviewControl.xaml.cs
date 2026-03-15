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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using System.ComponentModel;

#if ! SILVERLIGHT
using System.Printing;
using System.Windows.Markup;
using System.Text.RegularExpressions;
#else
using System.Windows.Printing;
using System.Text.RegularExpressions;
# endif

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Interaction logic for PrintNavigationControl.xaml
    /// </summary>
    public partial class PrintPreviewControl : UserControl
    {
        bool isLoaded = false;
        private int currentpage = 1;
        private const double ZOOM_STARTING_SIZE = 12.5;
        private double zoomFactor;
        private PageSetupUI PageSetup;
        PageInformation pageInfo = null;

        #region Public Properties

        /// <summary>
        /// Specifies whether it is possible to navigate to the next page.
        /// </summary>
        public bool CanMoveNext
        {
            get
            {
                return this.buttonNext.IsEnabled;
            }
        }

        /// <summary>
        /// Specifies whether it is possible to navigate to the previous page.
        /// </summary>
        public bool CanMovePrevious
        {
            get
            {
                return this.buttonPrevious.IsEnabled;
            }
        }

        /// <summary>
        /// Specifies whether it is possible to navigate to the first page.
        /// </summary>
        public bool CanMoveFirst
        {
            get
            {
                return this.buttonFirst.IsEnabled;
            }
        }

        /// <summary>
        /// Specifies whether it is possible to navigate to the first page.
        /// </summary>
        public bool CanMoveLast
        {
            get
            {
                return this.buttonLast.IsEnabled;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            set
            {
                SetValue(HeaderTemplateProperty, value);
            }
            internal get
            {
                return (DataTemplate)GetValue(HeaderTemplateProperty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DataTemplate FooterTemplate
        {
            set
            {
                SetValue(FooterTemplateProperty, value);
            }
            internal get
            {
                return (DataTemplate)GetValue(FooterTemplateProperty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CurrentPage
        {
            get
            {
                return this.currentpage;
            }
            set
            {
                this.currentpage = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int TotalPage
        {
            get
            {
                return this.PrintDocument.TotalPages;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IPrintDocument PrintDocument
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public double ZoomFactor
        {
            get
            {
                return this.zoomFactor;
            }
            set
            {
                if (value >= 0.25 && value <= 5)
                {
                    this.Zoom.ScaleX = value;
                    this.Zoom.ScaleY = value;
                }

                else if (value >= 0.25)
                {
                    this.Zoom.ScaleX = .25;
                    this.Zoom.ScaleY = .25;
                }

                else if (value <= 5)
                {
                    this.Zoom.ScaleX = 5;
                    this.Zoom.ScaleY = 5;
                }
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(PrintPreviewControl), new PropertyMetadata(null, OnHeaderTemplatePropertyChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="e"></param>
        public static void OnHeaderTemplatePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            PrintPreviewControl printdialog = dependencyObject as PrintPreviewControl;
            printdialog.renderHeader.ContentTemplate = e.NewValue as DataTemplate;
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty FooterTemplateProperty =
              DependencyProperty.Register("FooterTemplate", typeof(DataTemplate), typeof(PrintPreviewControl), new PropertyMetadata(null, OnFooterTemplatePropertyChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="e"></param>
        public static void OnFooterTemplatePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            PrintPreviewControl printdialog = dependencyObject as PrintPreviewControl;
            printdialog.renderFooter.ContentTemplate = e.NewValue as DataTemplate;
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ZoomFactorProperty =
              DependencyProperty.Register("ZoomFactor", typeof(double), typeof(PrintPreviewControl), new PropertyMetadata((double)1, OnZoomFactorPropertyChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="e"></param>
        public static void OnZoomFactorPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            PrintPreviewControl printdialog = dependencyObject as PrintPreviewControl;
            printdialog.ZoomFactor = (int)e.NewValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintPreviewControl"/> class.
        /// </summary>
        public PrintPreviewControl()
        {
            InitializeComponent();
            WireEvents();
            pageInfo = new PageInformation();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintPreviewControl"/> class.
        /// </summary>
        /// <param name="printDocument"></param>
        public PrintPreviewControl(IPrintDocument printDocument)
        {
            InitializeComponent();
            WireEvents();
            pageInfo = new PageInformation();
            this.PrintDocument = printDocument;
        }

        void WireEvents()
        {
            this.Loaded += new RoutedEventHandler(PrintNavigationControl_Loaded);
            this.comboBoxPageZoom.SelectionChanged += new SelectionChangedEventHandler(comboBoxPageZoom_SelectionChanged);
        }

        void PrintNavigationControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.isLoaded = true;
            this.SetPageSize();
            this.IntializePrintDialog();
            this.DataContext = this.pageInfo;
        }

        void IntializePrintDialog()
        {
            this.renderArea.Margin = this.PrintDocument.Margin;
            this.PageView.InvalidateArrange();

            if (this.PrintDocument != null)
            {
                double headerHeight = this.GetHeight(this.HeaderTemplate);
                double footerHeight = this.GetHeight(this.FooterTemplate);

                this.PrintDocument.PrintablePageSize = new Size(this.PrintDocument.PageSize.Width - (this.PrintDocument.Margin.Left + this.PrintDocument.Margin.Right),
                    this.PrintDocument.PageSize.Height - (headerHeight + footerHeight) - (this.PrintDocument.Margin.Top + this.PrintDocument.Margin.Bottom));

                this.PrintDocument.OnSetPageSize();
                this.PageView.Width = this.PrintDocument.PageSize.Width;
                this.PageView.Height = this.PrintDocument.PageSize.Height;

                this.pageInfo.TotalPages = this.PrintDocument.TotalPages;

                if (this.PrintDocument.TotalPages > 0)
                {
                    this.textBoxTotalPages.Text = this.PrintDocument.TotalPages.ToString();

                    if (this.currentpage > this.PrintDocument.TotalPages && this.currentpage < 0)
                    {
                        this.currentpage = 1;
                    }

                    this.textBoxCurrentPage.Text = this.currentpage.ToString();
                    this.SetPageContent(this.currentpage);
                    this.buttonNext.IsEnabled = true;
                    this.buttonLast.IsEnabled = true;
                }
            }
        }

        double GetHeight(DataTemplate dataTemplate)
        {
            if (dataTemplate != null)
            {
                System.Windows.Controls.Border parentBorder = new System.Windows.Controls.Border();
                ContentControl panel = new ContentControl();
                panel.ContentTemplate = dataTemplate;
                parentBorder.Child = panel;
                panel.UpdateLayout();
                panel.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
                parentBorder.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
                parentBorder.Arrange(new Rect(0, 0, panel.ActualWidth, panel.ActualHeight));
                return panel.ActualHeight;
            }
            return 0;
        }

        void comboBoxPageZoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Regex reg = new Regex("[0-9]*");
            var value = reg.Match((this.comboBoxPageZoom.SelectedItem as ComboBoxItem).Content.ToString()).Value;
            this.zoomFactor = double.Parse(value) / 100;
            this.Zoom.ScaleX = this.zoomFactor;
            this.Zoom.ScaleY = this.zoomFactor;
        }

        void SetPageContent(int pageNo)
        {
            this.pageInfo.PageNumber = pageNo;
            this.renderCanvas.Content = this.PrintDocument.GetPage(pageNo - 1);
        }

        void SetPageSize()
        {
            if (this.PrintDocument.PageSize == null || (this.PrintDocument.PageSize.Height == 0 || this.PrintDocument.PageSize.Width == 0))
            {
                this.PrintDocument.PageSize = new Size(816, 1056);
            }
            if (this.PrintDocument.Margin == null)
            {
                this.PrintDocument.Margin = new Thickness(96);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdatePrintDialog()
        {
            if (this.isLoaded)
            {
                IntializePrintDialog();
            }
        }

        private void UpdateImageContent()
        {
            if (this.buttonFirst.IsEnabled)
            {
                (this.buttonFirst.Content as Image).Source = (BitmapImage)this.Resources["First_Nav"];
            }
            else
            {
                (this.buttonFirst.Content as Image).Source = (BitmapImage)this.Resources["First_NavDisabled"];
            }

            if (this.buttonPrevious.IsEnabled)
            {
                (this.buttonPrevious.Content as Image).Source = (BitmapImage)this.Resources["Previous_Nav"];
            }
            else
            {
                (this.buttonPrevious.Content as Image).Source = (BitmapImage)this.Resources["Previous_NavDisabled"];
            }

            if (this.buttonNext.IsEnabled)
            {
                (this.buttonNext.Content as Image).Source = (BitmapImage)this.Resources["Next_Nav"];
            }
            else
            {
                (this.buttonNext.Content as Image).Source = (BitmapImage)this.Resources["Next_NavDisabled"];
            }

            if (this.buttonLast.IsEnabled)
            {
                (this.buttonLast.Content as Image).Source = (BitmapImage)this.Resources["Last_Nav"];
            }
            else
            {
                (this.buttonLast.Content as Image).Source = (BitmapImage)this.Resources["Last_NavDisabled"];
            }
        }

        private void buttonFirst_Click(object sender, RoutedEventArgs e)
        {
            currentpage = 1;
            this.SetPageContent(currentpage);
            this.textBoxCurrentPage.Text = "1";
            this.buttonPrevious.IsEnabled = false;
            this.buttonFirst.IsEnabled = false;
            this.buttonNext.IsEnabled = true;
            this.buttonLast.IsEnabled = true;
            UpdateImageContent();
        }

        private void buttonPrevious_Click(object sender, RoutedEventArgs e)
        {
            currentpage--;
            this.SetPageContent(currentpage);
            this.textBoxCurrentPage.Text = currentpage.ToString();
            this.buttonLast.IsEnabled = true;
            this.buttonNext.IsEnabled = true;
            if (currentpage == 1)
            {
                this.buttonFirst.IsEnabled = false;
                this.buttonPrevious.IsEnabled = false;
            }
            UpdateImageContent();
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            currentpage++;
            this.textBoxCurrentPage.Text = currentpage.ToString();
            this.SetPageContent(currentpage);
            if (currentpage == this.PrintDocument.TotalPages)
            {
                this.buttonNext.IsEnabled = false;
                this.buttonLast.IsEnabled = false;
            }
            this.buttonFirst.IsEnabled = true;
            this.buttonPrevious.IsEnabled = true;
            UpdateImageContent();
        }

        private void buttonLast_Click(object sender, RoutedEventArgs e)
        {
            currentpage = this.PrintDocument.TotalPages;
            this.SetPageContent(currentpage);
            this.textBoxCurrentPage.Text = this.PrintDocument.TotalPages.ToString();
            this.buttonFirst.IsEnabled = true;
            this.buttonPrevious.IsEnabled = true;
            this.buttonNext.IsEnabled = false;
            this.buttonLast.IsEnabled = false;
            UpdateImageContent();
        }

        private void buttonprint_Click(object sender, RoutedEventArgs e)
        {
            this.Print();
        }

        StackPanel GetPageVisual(int pageNo)
        {
            PageInformation pageDetails = new PageInformation();
            pageDetails.TotalPages = this.TotalPage;
            pageDetails.PageNumber = pageNo;
            StackPanel panel = new StackPanel();
            panel.DataContext =
            panel.Orientation = Orientation.Vertical;

            Grid renderArea = new Grid();
            renderArea.Margin = this.PrintDocument.Margin;
            panel.Width = this.PrintDocument.PageSize.Width;
            panel.Height = this.PrintDocument.PageSize.Height;
            panel.Children.Add(renderArea);

            renderArea.RowDefinitions.Clear();

            for (int i = 0; i < 3; i++)
            {
                RowDefinition def = new RowDefinition();
                def.Height = new GridLength(0, GridUnitType.Auto);
                renderArea.RowDefinitions.Add(def);
            }

            int row = 0;
            // Header
            ContentControl header = new ContentControl();
            Grid.SetRow(header, row++);
            renderArea.Children.Add(header);

            if (this.HeaderTemplate != null)
            {
                header.ContentTemplate = this.HeaderTemplate;
            }

            // Body
            ContentControl body = new ContentControl();
            Grid.SetRow(body, row++);
            renderArea.Children.Add(body);
            body.Content = this.PrintDocument.GetPage(pageNo - 1);

            // Footer
            ContentControl footer = new ContentControl();
            Grid.SetRow(footer, row);
            renderArea.Children.Add(footer);

            if (this.FooterTemplate != null)
            {
                footer.ContentTemplate = this.FooterTemplate;
            }

            this.SetPageContent(pageNo);
            panel.UpdateLayout();
            return this.PageView;
        }

#if ! SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        public void Print()
        {
            PrintDialog printDialog = new System.Windows.Controls.PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintDocument(GetFixedDocument().DocumentPaginator, "Print Document");
            }
        }

        private FixedDocument GetFixedDocument()
        {
            this.comboBoxPageZoom.SelectedIndex = 3;
            FixedDocument document = new FixedDocument();
            PrintDialog printDialog = new System.Windows.Controls.PrintDialog();
            int currentPage = this.currentpage;
            int selectedZoom = this.comboBoxPageZoom.SelectedIndex;
            this.comboBoxPageZoom.SelectedIndex = 3;
            StackPanel viewerControl = new StackPanel();
            for (int i = 1; i <= this.PrintDocument.TotalPages; i++)
            {
                viewerControl = this.GetPageVisual(i);
                Size controlSize = new Size(this.PrintDocument.PageSize.Width, this.PrintDocument.PageSize.Height);
                viewerControl.Measure(controlSize);
                viewerControl.Arrange(new Rect(new Point(0, 0), controlSize));
                //Capture the image of the visual in the same size as Printing page.  
                RenderTargetBitmap bmp = new RenderTargetBitmap((int)viewerControl.ActualWidth, (int)viewerControl.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                bmp.Render(viewerControl);
                DrawingVisual pageVisual = new DrawingVisual();
                DrawingContext drawingContext = pageVisual.RenderOpen();
                drawingContext.PushTransform(new TranslateTransform(0, 0));
                drawingContext.DrawImage(bmp, new System.Windows.Rect(new Size(this.PageView.Width, this.PageView.Height)));
                drawingContext.Close();
                PageContent m_PageContent = new PageContent();
                FixedPage page = new FixedPage();
                page.Width = this.PrintDocument.PageSize.Width;
                page.Height = this.PrintDocument.PageSize.Height;
                PrintVisualContainer myContainer = new PrintVisualContainer();
                myContainer.AddVisual(pageVisual);
                page.Children.Add(myContainer);
                ((IAddChild)m_PageContent).AddChild(page);
                document.Pages.Add(m_PageContent);
            }

            this.comboBoxPageZoom.SelectedIndex = selectedZoom;
            return document;
        }
#else
        /// <summary>
        /// 
        /// </summary>
        public void Print()
        {
            PrintDocument printDocument = new PrintDocument();
            int printPageCount = this.PrintDocument.TotalPages;
            int startPage = 1;

            printDocument.PrintPage += (s, args) =>
            {
                args.PageVisual = this.GetPageVisual(startPage);
                startPage++;
                args.HasMorePages = startPage <= printPageCount;
            };

            printDocument.Print("ReportViewer Print");
        }
#endif
        private void PageLayout_Click(object sender, RoutedEventArgs e)
        {
            PageSetupUI pagesetup = new PageSetupUI();
            this.PageSetup = pagesetup;
            pagesetup.IsInternalChange = true;

#if !SILVERLIGHT
            pagesetup.Owner = Window.GetWindow(this);
#endif
            pagesetup.top.Value = this.PrintDocument.Margin.Top / 96;
            pagesetup.left.Value = this.PrintDocument.Margin.Left / 96;
            pagesetup.right.Value = this.PrintDocument.Margin.Right / 96;
            pagesetup.bottom.Value = this.PrintDocument.Margin.Bottom / 96;
            pagesetup.pageWidth.Value = this.PrintDocument.PageSize.Width / 96;
            pagesetup.pageHeight.Value = this.PrintDocument.PageSize.Height / 96;
            pagesetup.IsInternalChange = false;

#if ! SILVERLIGHT
            if (pagesetup.ShowDialog() == true)
            {
                this.PrintDocument.PageSize = new Size(pagesetup.PageWidth, pagesetup.PageHeight);
                this.PrintDocument.Margin = new Thickness(pagesetup.LeftMargin, pagesetup.TopMargin, pagesetup.RightMargin, pagesetup.BottomMargin);
                this.UpdatePrintDialog();
            }
#else
            pagesetup.ShowDialog();

            pagesetup.Ok_button.Click += new RoutedEventHandler(Ok_button_Click);

#endif
        }

        void Ok_button_Click(object sender, RoutedEventArgs e)
        {
            if (PageSetup.DialogResult == true)
            {
                this.PrintDocument.PageSize = new Size(PageSetup.PageWidth, PageSetup.PageHeight);
                this.PrintDocument.Margin = new Thickness(PageSetup.LeftMargin, PageSetup.TopMargin, PageSetup.RightMargin, PageSetup.BottomMargin);
                this.UpdatePrintDialog();
                PageSetup.DialogResult = false;
            }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public interface IPrintDocument
    {
        /// <summary>
        /// 
        /// </summary>
        int TotalPages { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Size PageSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Size PrintablePageSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Thickness Margin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageNo"></param>
        /// <returns></returns>
        FrameworkElement GetPage(int pageNo);

        /// <summary>
        /// 
        /// </summary>
        void OnSetPageSize();
    }

#if !SILVERLIGHT
    class PrintVisualContainer : FrameworkElement
    {
        private readonly VisualCollection children;
        public PrintVisualContainer()
        {
            children = new VisualCollection(this);
        }

        public void AddVisual(Visual v)
        {
            children.Add(v);
        }
        protected override Visual GetVisualChild(int index)
        {
            return children[index];
        }
        protected override int VisualChildrenCount
        {
            get { return children.Count; }
        }
    }
#endif
    /// <summary>
    /// 
    /// </summary>
    public class PageInformation : INotifyPropertyChanged
    {
        private int pageNumber = 0;
        private int totalPages = 0;

        internal PageInformation()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public int PageNumber
        {
            internal set
            {
                this.pageNumber = value;
                this.OnPropertyChanged("PageNumber");
            }
            get
            {
                return this.pageNumber;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int TotalPages
        {
            internal set
            {
                this.totalPages = value;
                this.OnPropertyChanged("TotalPages");
            }
            get
            {
                return this.totalPages;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
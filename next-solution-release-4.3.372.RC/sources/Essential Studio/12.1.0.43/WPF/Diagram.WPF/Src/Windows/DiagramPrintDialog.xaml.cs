// <copyright file="DiagramPrintDialog.xaml.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Xps;
using System.ComponentModel;
using System.Windows.Markup;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Interaction logic for DiagramPrintDialog.xaml
    /// </summary>
    /// <exclude/>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public partial class DiagramPrintDialog : Window
    {
        #region Members
        /// <summary>
        /// Used to store the element to be printed.
        /// </summary>
        private FrameworkElement m_elementToPrint;

        /// <summary>
        /// Represents Print dialog
        /// </summary>
        private PrintDialog m_nativePrintDialog = new PrintDialog();

        ///// <summary>
        ///// Represents the Visual brush
        ///// </summary>
        //private VisualBrush m_visualBrush;

        #endregion

        #region Dependency properties

        ///// <summary>
        ///// Using a DependencyProperty as the backing store for PrintStrech.  This enables animation, styling, binding, etc...
        ///// </summary>
        //public static readonly DependencyProperty PrintStretchProperty =
        //        DependencyProperty.Register("PrintStretch", typeof(Stretch), typeof(DiagramPrintDialog), new FrameworkPropertyMetadata(Stretch.Uniform, new PropertyChangedCallback(OnPrintStretchChanged)));

        #endregion

        #region Events

        ///// <summary>
        ///// Event that is raised when PrintStretch property is changed.
        ///// </summary>
        //public event PropertyChangedCallback PrintStretchChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramPrintDialog"/> class.
        /// </summary>
        public DiagramPrintDialog()
        {
            InitializeComponent();
            this.DataContext = this;

            UpdatePreviewSize();
        }

        private void UpdatePreviewSize()
        {
            // Get the print capabilities
            PrintCapabilities capabilities = m_nativePrintDialog.PrintQueue.GetPrintCapabilities(m_nativePrintDialog.PrintTicket);
            Size pageSize = new Size(m_nativePrintDialog.PrintableAreaWidth, m_nativePrintDialog.PrintableAreaHeight);
            Size printableSize = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

            PreviewRect.Width = printableSize.Width;
            PreviewRect.Height = printableSize.Height;
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Shows the print dialog.
        /// </summary>
        /// <param name="element">The element to be printed.</param>
        /// <returns>The boolean value indicating the Dialog box is shown or not.</returns>
        public bool? ShowPrintDialog(FrameworkElement element)
        {
            m_elementToPrint = element;

            VisualBrush visualBrush = new VisualBrush(element);
            visualBrush.Stretch = Stretch.Uniform;
            visualBrush.ViewboxUnits = BrushMappingMode.Absolute;
            visualBrush.Viewbox = new Rect(0, 0, m_elementToPrint.ActualWidth, m_elementToPrint.ActualHeight);
            if (m_elementToPrint is DiagramPage)
                visualBrush.Viewbox = new Rect(-(m_elementToPrint as DiagramPage).Left, -(m_elementToPrint as DiagramPage).Top, m_elementToPrint.ActualWidth + Math.Abs((m_elementToPrint as DiagramPage).Left), m_elementToPrint.ActualHeight + Math.Abs((m_elementToPrint as DiagramPage).Top));
           
            VisualBrush m_visualBrush = visualBrush;
            this.PreviewRect.Fill = visualBrush;

            return this.ShowDialog();
        }

        /// <summary>
        /// Shows the print dialog.
        /// </summary>
        /// <param name="element">The element to be printed.</param>
        /// <param name="printArea">The print area.</param>
        /// <returns> /// <returns>The boolean value indicating the Dialog box is shown or not.</returns></returns>
        public bool? ShowPrintDialog(FrameworkElement element, Rect printArea)
        {
            m_elementToPrint = element;
            VisualBrush visualBrush = new VisualBrush(element);
            visualBrush.Stretch = Stretch.Uniform;
            visualBrush.ViewboxUnits = BrushMappingMode.Absolute;
            visualBrush.Viewbox = printArea;

            this.PreviewRect.Fill = visualBrush;
            return this.ShowDialog();
        }

        /// <summary>
        /// Prints the Diagram Page Directly.
        /// </summary>
        /// <param name="element">The element to be printed.</param>
        /// <param name="stretch">The s is a Stretch options.</param>        
        internal void Print(FrameworkElement element, Stretch stretch)
        {
            m_elementToPrint = element;
            VisualBrush visualBrush = new VisualBrush(element);
            visualBrush.Stretch = stretch;
            visualBrush.ViewboxUnits = BrushMappingMode.Absolute;
            visualBrush.Viewbox = new Rect(0, 0, m_elementToPrint.ActualWidth, m_elementToPrint.ActualHeight);
            //this.PrintStretch = s;
            this.PreviewRect.Fill = visualBrush;

            StartPrint();
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Starts the print.
        /// </summary>
        private void StartPrint()
        {
            PrintCapabilities printCapabilities = m_nativePrintDialog.PrintQueue.GetPrintCapabilities(m_nativePrintDialog.PrintTicket);

            Size pageSize = new Size(m_nativePrintDialog.PrintableAreaWidth, m_nativePrintDialog.PrintableAreaHeight);
            Size pageAreaSize = new Size(printCapabilities.PageImageableArea.ExtentWidth, printCapabilities.PageImageableArea.ExtentHeight);
            Rectangle rect = new Rectangle();

            rect.Fill = this.PreviewRect.Fill;
            VisualBrush m_visualBrush = rect.Fill as VisualBrush;
            SetViewport(m_visualBrush, pageAreaSize);
            rect.Arrange(new Rect(new Point(0, 0), pageAreaSize));

            if (m_visualBrush.Stretch != Stretch.None)
            {
                XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(m_nativePrintDialog.PrintQueue);
                writer.Write(rect, m_nativePrintDialog.PrintTicket);
            }
            else
            {
                PrintMultiplePages(m_elementToPrint);
            }

            SetViewport(m_visualBrush, new Size(this.PreviewRect.ActualWidth, this.PreviewRect.ActualHeight));
        }

        private void PrintMultiplePages(FrameworkElement printVisual)
        {
            // Get the print capabilities
            PrintCapabilities capabilities = m_nativePrintDialog.PrintQueue.GetPrintCapabilities(m_nativePrintDialog.PrintTicket);
            Size pageSize = new Size(m_nativePrintDialog.PrintableAreaWidth, m_nativePrintDialog.PrintableAreaHeight);
            Size printableSize = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

            // Split the diagram into MultiplePages and save it in fixed document.
            FixedDocument fixedDoc = new FixedDocument();
            printVisual.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            printVisual.Arrange(new Rect(new Point(0, 0), printVisual.DesiredSize));
            Size size = printVisual.DesiredSize;
            double runY = 0;
            double runX = 0;
            while (runY < size.Height)
            {
                while (runX < size.Width)
                {
                    VisualBrush vb = new VisualBrush(printVisual);
                    vb.Stretch = Stretch.None;
                    vb.AlignmentX = AlignmentX.Left;
                    vb.AlignmentY = AlignmentY.Top;
                    vb.ViewboxUnits = BrushMappingMode.Absolute;
                    vb.TileMode = TileMode.None;
                    vb.Viewbox = new Rect(runX, runY, printableSize.Width, printableSize.Height);
                    PageContent pageContent = new PageContent();
                    FixedPage page = new FixedPage();
                    ((IAddChild)pageContent).AddChild(page);
                    fixedDoc.Pages.Add(pageContent);
                    page.Width = pageSize.Width;
                    page.Height = pageSize.Height;
                    Canvas canvas = new Canvas();
                    FixedPage.SetLeft(canvas, capabilities.PageImageableArea.OriginWidth);
                    FixedPage.SetTop(canvas, capabilities.PageImageableArea.OriginHeight);
                    canvas.Width = printableSize.Width;
                    canvas.Height = printableSize.Height;
                    canvas.Background = vb;
                    page.Children.Add(canvas);
                    runX += printableSize.Width;
                }
                runX = 0;
                runY += printableSize.Height;
            }

            XpsDocumentWriter doc = PrintQueue.CreateXpsDocumentWriter(m_nativePrintDialog.PrintQueue);
            doc.Write(fixedDoc, m_nativePrintDialog.PrintTicket);
        }

        /// <summary>
        /// Called when [print click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnPrintClick(object sender, RoutedEventArgs args)
        {
            this.StartPrint();
            this.DialogResult = true;
        }

        /// <summary>
        /// Called when [cancel click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCancelClick(object sender, RoutedEventArgs args)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// Called when [color click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnColorClick(object sender, RoutedEventArgs args)
        {
            m_nativePrintDialog.PrintTicket.OutputColor = OutputColor.Color;
        }

        /// <summary>
        /// Called when [black and white click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnBlackAndWhiteClick(object sender, RoutedEventArgs args)
        {
            m_nativePrintDialog.PrintTicket.OutputColor = OutputColor.Monochrome;
        }

        /// <summary>
        /// Called when [advanced click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnAdvancedClick(object sender, RoutedEventArgs args)
        {
            bool? result = m_nativePrintDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                StartPrint();
                this.DialogResult = result.Value;
            }
            UpdatePreview();
        }

        /// <summary>
        /// Gets the size by specified stretch.
        /// </summary>
        /// <param name="stretch">The stretch.</param>
        /// <param name="viewport">The viewport.</param>
        /// <param name="original">The original.</param>
        /// <returns>The size to be printed.</returns>
        private static Size GetPrintSize(Stretch stretch, Size viewport, Size original)
        {
            Size result = Size.Empty;

            switch (stretch)
            {
                case Stretch.Fill:
                    result = viewport;
                    break;

                case Stretch.None:
                    result = original;
                    break;

                case Stretch.Uniform:
                    {
                        double dx = viewport.Width / original.Width;
                        double dy = viewport.Height / original.Height;

                        if (dx < dy)
                        {
                            result = new Size(viewport.Width, dx * original.Height);
                        }
                        else
                        {
                            result = new Size(dy * original.Width, viewport.Height);
                        }
                    }

                    break;

                case Stretch.UniformToFill:
                    {
                        double dx = viewport.Width / original.Width;
                        double dy = viewport.Height / original.Height;

                        if (dx > dy)
                        {
                            result = new Size(viewport.Width, dx * original.Height);
                        }
                        else
                        {
                            result = new Size(dy * original.Width, viewport.Height);
                        }
                    }

                    break;
            }

            return result;
        }

        /// <summary>
        /// Calls SelectionChanged Event 
        /// </summary>
        private void PrintStrech_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {            
            VisualBrush m_visualBrush = this.PreviewRect.Fill as VisualBrush;
            UpdatePreviewSize();
            PageInfo.Visibility = System.Windows.Visibility.Collapsed;
            if (m_visualBrush != null)
            {
                m_visualBrush.Stretch = Stretch.Uniform;
                m_visualBrush.ViewboxUnits = BrushMappingMode.Absolute;
                m_visualBrush.Viewbox = new Rect(0, 0, m_elementToPrint.ActualWidth, m_elementToPrint.ActualHeight);
                if (m_elementToPrint is DiagramPage)
                 m_visualBrush.Viewbox = new Rect(-(m_elementToPrint as DiagramPage).Left, -(m_elementToPrint as DiagramPage).Top, m_elementToPrint.ActualWidth + Math.Abs((m_elementToPrint as DiagramPage).Left), m_elementToPrint.ActualHeight + Math.Abs((m_elementToPrint as DiagramPage).Top));
                switch (Print_Stretch.SelectedIndex)
                {
                    case 0:
                        m_visualBrush.Stretch = Stretch.Fill;
                        PageInfo.Visibility = System.Windows.Visibility.Visible;
                        UpdatePageCount(m_visualBrush);
                        UpdateNoneStretch();

                        break;
                    case 1:
                        m_visualBrush.Stretch = Stretch.Fill;
                        break;
                    case 2:
                        m_visualBrush.Stretch = Stretch.Uniform;
                        break;
                    case 3:
                        m_visualBrush.Stretch = Stretch.UniformToFill;
                        break;
                }
                SetViewport(m_visualBrush, new Size(this.PreviewRect.ActualWidth, this.PreviewRect.ActualHeight));
            }
        }

        /// <summary>
        /// Sets the viewport.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="size">The size of the viewport.</param>
        private void SetViewport(VisualBrush brush, Size size)
        {
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }

            if (this.Print_Stretch.SelectedIndex == 2)
            {
                double coefficientHeight = size.Height / brush.Viewbox.Height;
                double coefficientWidth = size.Width / brush.Viewbox.Width;

                if (coefficientHeight < coefficientWidth)
                {
                    double width = coefficientHeight * brush.Viewbox.Width / size.Width;
                    double x = (1 - width) / 2;
                    brush.Viewport = new Rect(new Point(x, 0), new Size(width, 1));
                }
                else if (coefficientHeight > coefficientWidth)
                {
                    double height = coefficientWidth * brush.Viewbox.Height / size.Height;
                    double y = (1 - height) / 2;
                    brush.Viewport = new Rect(new Point(0, y), new Size(1, height));
                }
            }
            else if (this.Print_Stretch.SelectedIndex == 0)
            {
                if (size.Width > brush.Viewbox.Width || size.Height > brush.Viewbox.Height)
                {
                    double coefficientHeight = size.Width - brush.Viewbox.Width;
                    double coefficientWidth = size.Height - brush.Viewbox.Height;
                    double width = brush.Viewbox.Width / size.Width;
                    double height = brush.Viewbox.Height / size.Height;
                    double x = (1 - width) / 2;
                    double y = (1 - height) / 2;
                    brush.Viewport = new Rect(new Point(x, y), new Size(width, height));
                }
                else
                {
                    brush.Viewport = new Rect(0, 0, 1, 1);
                }
            }
            else
            {
                brush.Viewport = new Rect(0, 0, 1, 1);
            }
        }
        
        #endregion

        #region MultiPage

        internal void UpdatePageCount(VisualBrush m_visualBrush)
        {
            Point _pos = new Point(m_visualBrush.Viewbox.Left, m_visualBrush.Viewbox.Top);
            PrintCapabilities capabilities = m_nativePrintDialog.PrintQueue.GetPrintCapabilities(m_nativePrintDialog.PrintTicket);
            Size pageSize = new Size(m_nativePrintDialog.PrintableAreaWidth, m_nativePrintDialog.PrintableAreaHeight);
            Size printableSize = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

            if (m_elementToPrint != null)
            {
                double hor = Math.Ceiling(m_elementToPrint.DesiredSize.Width / printableSize.Width);
                double ver = Math.Ceiling(m_elementToPrint.DesiredSize.Height / printableSize.Height);
                int count = 1;
                while (true)
                {
                    _pos.X += printableSize.Width;
                    if (_pos.X >= m_elementToPrint.DesiredSize.Width)
                    {
                        _pos.X = 0;
                        _pos.Y += printableSize.Height;
                    }

                    if ((_pos.Y >= this.m_elementToPrint.DesiredSize.Height))
                    {
                        break;
                    }
                    count++;
                }
                PageCount = count;
            }
        }

        public int PageCount
        {
            get { return (int)GetValue(PageCountProperty); }
            internal set { SetValue(PageCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageCountProperty =
            DependencyProperty.Register("PageCount", typeof(int), typeof(DiagramPrintDialog), new PropertyMetadata(1));

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
            if (dpd.CurrentPage > dpd.PageCount || dpd.CurrentPage < 1)
            {
                dpd.CurrentPage = (int)args.OldValue;
            }
            dpd.UpdatePreview();
        }

        private void UpdateNoneStretch()
        {
            VisualBrush vb = this.PreviewRect.Fill as VisualBrush;
            int _pageNo = this.CurrentPage;
            Point _pos = new Point(vb.Viewbox.Left, vb.Viewbox.Top);
            PrintCapabilities capabilities = m_nativePrintDialog.PrintQueue.GetPrintCapabilities(m_nativePrintDialog.PrintTicket);
            Size pageSize = new Size(m_nativePrintDialog.PrintableAreaWidth, m_nativePrintDialog.PrintableAreaHeight);
            Size printableSize = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

            for (int i = 1; i < _pageNo; i++)
            {
                _pos.X += printableSize.Width;
                if (_pos.X > this.m_elementToPrint.DesiredSize.Width)
                {
                    _pos.X = vb.Viewbox.Left;
                    _pos.Y += printableSize.Height;
                }
            }

            //vb = new VisualBrush(m_elementToPrint);
            vb.Stretch = Stretch.None;
            vb.AlignmentX = AlignmentX.Left;
            vb.AlignmentY = AlignmentY.Top;
            vb.ViewboxUnits = BrushMappingMode.Absolute;
            vb.TileMode = TileMode.None;
            vb.Viewbox = new Rect(_pos.X, _pos.Y, printableSize.Width, printableSize.Height);
        }

        private void PageDecrease_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage != 1)
            {
                CurrentPage -= 1;
            }
        }

        private void PageIncrease_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage != PageCount && CurrentPage < PageCount)
            {
                CurrentPage += 1;
            }
        }

        private void CurrentPage_TextChanged(object sender, TextChangedEventArgs e)
        {
            int num = 0;
            bool success = int.TryParse(((TextBox)sender).Text, out num);
            if (success)
            {
                CurrentPage = num;
            }
            else
            {
                CurrentPageBox.Text = CurrentPage.ToString();
            }
        } 

        #endregion
    }
}

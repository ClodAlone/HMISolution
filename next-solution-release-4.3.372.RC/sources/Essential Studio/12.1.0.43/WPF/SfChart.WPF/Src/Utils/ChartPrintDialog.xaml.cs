#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Documents.Serialization;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace Syncfusion.UI.Xaml.Charts
{
    public partial class ChartPrintDialog : Window
    {
        #region Members
        /// <summary>
        /// Initializes m_elementToPrint
        /// </summary>
        private FrameworkElement m_elementToPrint;

        /// <summary>
        /// Initializes m_nativePrintDialog
        /// </summary>
        private PrintDialog m_nativePrintDialog = new PrintDialog();

        /// <summary>
        /// Initializes m_visualBrush
        /// </summary>
        private VisualBrush m_visualBrush;

        /// <summary>
        /// Initializes ChartHeight
        /// </summary>
        private double ChartHeight;

        /// <summary>
        /// Initializes ChartWidth
        /// </summary>
        private double ChartWidth;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Using a DependencyProperty as the backing store for PrintStrech.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PrintStretchProperty =
            DependencyProperty.Register("PrintStretch", typeof(Stretch), typeof(ChartPrintDialog), new FrameworkPropertyMetadata(Stretch.Uniform, new PropertyChangedCallback(OnPrintStretchChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for ChartPrintMode.  This enables to choose chart print mode...
        /// </summary>
        public static readonly DependencyProperty PrintModeProperty =
            DependencyProperty.Register("PrintMode", typeof(ChartPrintMode), typeof(ChartPrintDialog), new FrameworkPropertyMetadata(ChartPrintMode.Portrait, new PropertyChangedCallback(OnPrintModeChanged)));
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the print mode.
        /// </summary>
        /// <value>The print stretch.</value>
        public ChartPrintMode PrintMode
        {
            get
            {
                return (ChartPrintMode)GetValue(PrintModeProperty);
            }

            set
            {
                SetValue(PrintModeProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the print stretch.
        /// </summary>
        /// <value>The print stretch.</value>
        public Stretch PrintStretch
        {
            get
            {
                return (Stretch)GetValue(PrintStretchProperty);
            }

            set
            {
                SetValue(PrintStretchProperty, value);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when PrintStretch property is changed.
        /// </summary>
        public event PropertyChangedCallback PrintStretchChanged;

        /// <summary>
        /// Event that is raised when PrintMode property is changed.
        /// </summary>
        public event PropertyChangedCallback PrintModeChanged;
        #endregion

        public ChartPrintDialog()
        {
            InitializeComponent();
        }

        #region Public methods
        /// <summary>
        /// Shows the print dialog.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Returns ShowDialog</returns>
        public bool? ShowPrintDialog(FrameworkElement element)
        {
            return this.ShowPrintDialog(element, Rect.Empty, 335, 252);
        }


        public virtual Rectangle GetPrintVisual(FrameworkElement element)
        {
            m_elementToPrint = element;
            SfChart chart = element as SfChart; 
            VisualBrush visualBrush = new VisualBrush(CloneVisualState(element));
            this.ChartHeight = element.ActualHeight;
            this.ChartWidth = element.ActualWidth;
            visualBrush.Stretch = Stretch.Uniform;
            visualBrush.ViewboxUnits = BrushMappingMode.Absolute;
            visualBrush.Viewbox = new Rect(0, 0, m_elementToPrint.ActualWidth, m_elementToPrint.ActualHeight);
            m_visualBrush = visualBrush;

            Rectangle outputRect= new Rectangle();
            outputRect.Height = ChartHeight;
            outputRect.Width = ChartWidth;
            outputRect.Fill = m_visualBrush;
            return outputRect;
        }

        /// <summary>
        /// Shows the print dialog.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="printArea">The print area.</param>
        /// <returns>Returns ShowDialog</returns>
        /// <seealso cref="ChartPrintDialog"/>
        public virtual bool? ShowPrintDialog(FrameworkElement element, Rect printArea, double elem_height, double elem_width)
        {
            m_elementToPrint = element;

            SfChart chart = element as SfChart;           

            //setting the Actual height and Actual Width of the Chart to ChartHeight and ChartWidth property
            this.ChartHeight = element.ActualHeight;
            this.ChartWidth = element.ActualWidth;

            //element.Height = elem_height; //Setting the Actual height of the preview Rectangle
            //element.Width = elem_width; //Setting the Actual width of the preview Rectangle

           

            VisualBrush visualBrush = new VisualBrush(CloneVisualState(element));
                      
            visualBrush.Stretch = Stretch.Uniform;
            visualBrush.ViewboxUnits = BrushMappingMode.Absolute;
            visualBrush.Viewbox = printArea.IsEmpty ? new Rect(0, 0, m_elementToPrint.ActualWidth, m_elementToPrint.ActualHeight) : printArea;
            m_visualBrush = visualBrush;
            this.PreviewRect.Fill = visualBrush;            

            //Restoring the stored Width and Height back to the Chart
            //element.Height = this.ChartHeight;
            //element.Width = this.ChartWidth;
            return this.ShowDialog();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Starts the print.
        /// </summary>
        /// <seealso cref="ChartPrintDialog"/>
        private void StartPrint()
        {
            PrintCapabilities printCapabilities = m_nativePrintDialog.PrintQueue.GetPrintCapabilities(m_nativePrintDialog.PrintTicket);           
            Size pageSize = new Size(m_nativePrintDialog.PrintableAreaWidth, m_nativePrintDialog.PrintableAreaHeight);
            Size pageAreaSize = new Size(printCapabilities.PageImageableArea.ExtentWidth, printCapabilities.PageImageableArea.ExtentHeight);
            ////Size printSize = this.GetPrintSize(this.PrintStretch, pageAreaSize, visualSize);
            ////Point printPt = ChartLayoutUtils.GetStartPointBy(printSize, pageSize, ChartAlignment.Center, ChartAlignment.Center);

            Rectangle rect = new Rectangle();            
            rect.Fill = m_visualBrush;

            SetViewport(m_visualBrush, pageAreaSize);
            rect.Arrange(new Rect(new Point(0, 0), pageAreaSize));

            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)pageAreaSize.Width, (int)pageAreaSize.Height, 96, 96, PixelFormats.Default);
            bitmap.Render(rect);            

            FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();
            newFormatedBitmapSource.BeginInit();
            newFormatedBitmapSource.Source = bitmap;
            if (colorMode.IsChecked == true)
                newFormatedBitmapSource.DestinationFormat = PixelFormats.Default;
            else
                newFormatedBitmapSource.DestinationFormat = PixelFormats.Gray32Float;
            newFormatedBitmapSource.EndInit();

            Image myImage = new Image();
            myImage.Height = (int)pageAreaSize.Height;
            myImage.Width = (int)pageAreaSize.Width;
            myImage.Source = newFormatedBitmapSource;

            if (colorMode.IsChecked == false)
                rect.Fill = new ImageBrush(newFormatedBitmapSource);

            XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(m_nativePrintDialog.PrintQueue);
            writer.Write(rect, m_nativePrintDialog.PrintTicket);

            SetViewport(m_visualBrush, new Size(this.PreviewRect.ActualWidth, this.PreviewRect.ActualHeight));
        }
        
        /// <summary>
        /// Called when [print click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <seealso cref="ChartPrintDialog"/>
        private void OnPrintClick(object sender, RoutedEventArgs args)
        {
            this.StartPrint();
            base.DialogResult = true;
        }

        /// <summary>
        /// Called when [cancel click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCancelClick(object sender, RoutedEventArgs args)
        {
            base.DialogResult = false;
        }

        /// <summary>
        /// Called when [color click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnColorClick(object sender, RoutedEventArgs args)
        {
            //m_nativePrintDialog.PrintTicket.OutputColor = OutputColor.Color;
            RefreshRect();
        }

        /// <summary>
        /// Called when [black and white click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnBlackAndWhiteClick(object sender, RoutedEventArgs args)
        {
            //m_nativePrintDialog.PrintTicket.OutputColor = OutputColor.Grayscale;
            RefreshRect();
        }

        private void OnPrintStretchChanged(object sender, RoutedEventArgs args)
        {
            RefreshRect();
        }
        private void OnPrintModeChanged(object sender, RoutedEventArgs args)
        {
            RefreshRect();
        }

        private void RefreshRect()
        {
            PrintCapabilities printCapabilities = m_nativePrintDialog.PrintQueue.GetPrintCapabilities(m_nativePrintDialog.PrintTicket);
            Size pageSize = new Size(m_nativePrintDialog.PrintableAreaWidth, m_nativePrintDialog.PrintableAreaHeight);
            Size pageAreaSize = new Size(printCapabilities.PageImageableArea.ExtentWidth, printCapabilities.PageImageableArea.ExtentHeight);            

            Rectangle rect = new Rectangle();

            rect.Fill = m_visualBrush;

            SetViewport(m_visualBrush, pageAreaSize);
            rect.Arrange(new Rect(new Point(0, 0), pageAreaSize));

            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)pageAreaSize.Width, (int)pageAreaSize.Height, 96, 96, PixelFormats.Default);
            bitmap.Render(rect);

            FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();
            newFormatedBitmapSource.BeginInit();
            
            newFormatedBitmapSource.Source = bitmap;
            if (colorMode.IsChecked == true)
                newFormatedBitmapSource.DestinationFormat = PixelFormats.Default;
            else
                newFormatedBitmapSource.DestinationFormat = PixelFormats.Gray32Float;
            newFormatedBitmapSource.EndInit();

            Image myImage = new Image();
            myImage.Height = (int)pageAreaSize.Height;
            myImage.Width = (int)pageAreaSize.Width;
            myImage.Source = newFormatedBitmapSource;            
            PreviewRect.Fill = new ImageBrush(newFormatedBitmapSource);
        }

        /// <summary>
        /// Called when [advanced click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnAdvancedClick(object sender, RoutedEventArgs args)
        {
            m_nativePrintDialog.ShowDialog();
        }

        /// <summary>
        /// Gets the size by specified stretch.
        /// </summary>
        /// <param name="stretch">The stretch.</param>
        /// <param name="viewport">The viewport.</param>
        /// <param name="original">The original.</param>
        /// <returns>The PRint size</returns>
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
        /// Calls OnPrintStretchChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPrintStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartPrintDialog instance = (ChartPrintDialog)d;
            instance.OnPrintStretchChanged(e);
        }

        /// <summary>
        /// Calls OnPrintModeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPrintModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartPrintDialog instance = (ChartPrintDialog)d;
            instance.OnPrintModeChanged(e);
        }

        /// <summary>
        /// Sets the viewport.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="size">The size value.</param>
        private void SetViewport(VisualBrush brush, Size size)
        {
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }

            if (this.PrintStretch == Stretch.Uniform)
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
            else if (this.PrintStretch == Stretch.None)
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

        /// <summary>
        /// Updates property value cache and raises PrintStretchChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnPrintStretchChanged(DependencyPropertyChangedEventArgs e)
        {
            m_visualBrush.Stretch = this.PrintStretch;
            SetViewport(m_visualBrush, new Size(this.PreviewRect.ActualWidth, this.PreviewRect.ActualHeight));

            if (PrintStretchChanged != null)
            {
                PrintStretchChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises PrintModeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnPrintModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.PrintMode == ChartPrintMode.Landscape)
            {
                TransformGroup transgroup = new TransformGroup();
                transgroup.Children.Add(new RotateTransform() { Angle = 90, CenterX = 0.5, CenterY = 0.5 });
                m_visualBrush.RelativeTransform = transgroup;
                SetViewport(m_visualBrush, new Size(this.PreviewRect.ActualWidth, this.PreviewRect.ActualHeight));
            }
            else
            {
                TransformGroup transgroup = new TransformGroup();
                transgroup.Children.Add(new RotateTransform() { Angle = 0, CenterX = 0.5, CenterY = 0.5 });
                m_visualBrush.RelativeTransform = transgroup;
                SetViewport(m_visualBrush, new Size(this.PreviewRect.ActualWidth, this.PreviewRect.ActualHeight));
            }
            if (PrintModeChanged != null)
            {
                PrintModeChanged(this, e);
            }
        }

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            SetViewport(m_visualBrush, new Size(this.PreviewRect.ActualWidth, this.PreviewRect.ActualHeight));
        }

        public Rect GetUIElementBounds(UIElement element)
        {
            return new Rect((Point)VisualTreeHelper.GetOffset(element), element.DesiredSize);
        }

        public Visual CloneVisualState(FrameworkElement targetElement)
        {            
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                VisualBrush contentBrush = new VisualBrush(targetElement)
                {
                    Stretch = Stretch.None,
                    AlignmentX = AlignmentX.Left,
                    AlignmentY = AlignmentY.Top
                };
                drawingContext.DrawRectangle(contentBrush, null, new Rect(0, 0,
                    targetElement.ActualWidth, targetElement.ActualHeight));
            }
            return drawingVisual;           
        }
        #endregion
    }

    public enum ChartPrintMode
    {
        Portrait,
        Landscape
    }

    class ChartPrintResources
    {

    }
}

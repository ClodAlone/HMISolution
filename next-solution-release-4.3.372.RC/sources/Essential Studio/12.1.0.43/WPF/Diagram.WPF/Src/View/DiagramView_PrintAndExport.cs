// <copyright file="DiagramView_PrintAndExport.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents the Diagram View Print and Export.
    /// </summary>
    /// <remarks>
    /// <para>The view obtains data from the model and presents them to the user. It typically manages the overall layout of the data obtained from model.
    /// Apart from presenting the data, view also handles navigation between the items, and some aspects of item selection. 
    /// The views also implements basic user interface features, such as rulers, and drag and drop, printing and exporting. 
    /// It handles the events, which occur on the objects, obtained from the model. 
    /// Command mechanism is also implemented by the view.
    /// </para>
    /// <para>
    /// The print feature enables the user to set a printer to be used, and it allows the user to define the pages and the number of copies that should be printed. It also provides an overview of the document, showing how the document will appear when printed.
    /// </para>
    /// <para>
    /// The page can also be exported to various image formats.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para/>The following example shows how to create a <see cref="DiagramView"/> in XAML.
    /// <code language="XAML">
    /// &lt;Window x:Class="RulersAndUnits.Window1"
    /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
    /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
    /// xmlns:local="clr-namespace:Sample" FontWeight="Bold"
    /// Icon="Images/App.ico" &gt;
    /// &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
    ///                                IsSymbolPaletteEnabled="True" 
    ///                                Background="WhiteSmoke"&gt;
    ///           &lt;syncfusion:DiagramControl.View&gt;
    ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
    ///                                          Background="LightGray"  
    ///                                          Bounds="0,0,12,12"  
    ///                                          ShowHorizontalGridLine="False" 
    ///                                          ShowVerticalGridLine="False"
    ///                                          Name="diagramView"  &gt;
    ///            &lt;syncfusion:DiagramView.HorizontalRuler&gt;
    ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
    ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
    ///           &lt;syncfusion:DiagramView.VerticalRuler&gt;
    ///               &lt;syncfusion:VerticalRuler Name="verticalRuler" /&gt;
    ///           &lt;/syncfusion:DiagramView.VerticalRuler &gt;
    ///       &lt;/syncfusion:DiagramView&gt;
    ///    &lt;/syncfusion:DiagramControl.View&gt;
    /// &lt;/syncfusion:DiagramControl&gt;
    /// &lt;/Window&gt;
    /// </code>
    /// <para/>The following example shows how to create a <see cref="DiagramView"/> in C#.
    /// <code language="C#">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using System.Windows.Data;
    /// using System.Windows.Documents;
    /// using System.Windows.Input;
    /// using System.Windows.Media;
    /// using System.Windows.Media.Imaging;
    /// using System.Windows.Navigation;
    /// using System.Windows.Shapes;
    /// using System.ComponentModel;
    /// using Syncfusion.Core;
    /// using Syncfusion.Windows.Diagram;
    /// namespace WpfApplication1
    /// {
    /// public partial class Window1 : Window
    /// {
    ///    public DiagramControl Control;
    ///    public DiagramModel Model;
    ///    public DiagramView View;
    ///    public Window1 ()
    ///    {
    ///       InitializeComponent ();
    ///       Control = new DiagramControl ();
    ///       View = new DiagramView ();
    ///       Control.View = View;
    ///       HorizontalRuler hruler = new HorizontalRuler();
    ///       View.HorizontalRuler = hruler;
    ///       View.ShowHorizontalGridLine = false;
    ///       View.ShowVerticalGridLine = false;
    ///       VerticalRuler vruler = new VerticalRuler();
    ///       View.VerticalRuler = vruler;
    ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
    ///       View.IsPageEditable = true;
    ///    }
    ///    }
    ///    }
    /// </code>
    /// </example>
    public partial class DiagramView
    {
        #region Constant Fields

        /// <summary>
        /// Dimensions for printing.
        /// </summary>
        private const int DefaultDim = 96;

        #endregion

        #region Print and Saving

        /// <summary>
        /// Called when [print execute].
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnPrintExecute(object target, ExecutedRoutedEventArgs args)
        {
            DiagramView diagramView = target as DiagramView;

            if (diagramView != null)
            {
                diagramView.Print();
            }
        }

        /// <summary>
        /// Displays the PrintDialog.
        /// </summary>
        /// <returns>True, if it is shown, false otherwise.</returns>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// diagramView.Print();
        /// </code>
        /// </example>
        public bool Print()
        {
            DiagramPrintDialog printDialog = new DiagramPrintDialog();
            printDialog.Loaded += new RoutedEventHandler(printDialog_Loaded);
            bool retValue = (bool)printDialog.ShowPrintDialog(Page);
            Keyboard.Focus(Page);
            return retValue;
        }

        void printDialog_Loaded(object sender, RoutedEventArgs e)
        {
            DiagramPrintDialog localizationprint = (sender as DiagramPrintDialog);

            localizationprint.diagram_print.Content = dc.m_ResourceWrapper.PrintDialog_Print;
            localizationprint.diagram_cancel.Content = dc.m_ResourceWrapper.PrintDialog_Cancel;
            localizationprint.diagram_advanced.Content = dc.m_ResourceWrapper.PrintDialog_Advanced;
            localizationprint.diagram_Stretch.Text = dc.m_ResourceWrapper.PrintDialog_PrintStretch;
            localizationprint.diagram_color.Text = dc.m_ResourceWrapper.PrintDialog_ColorMode;
            localizationprint.diagram_BlackandWhite.Text = dc.m_ResourceWrapper.PrintDialog_BlackandWhiteMode;
            localizationprint.printdiaglog.Title = dc.m_ResourceWrapper.PrintDialog_Name;
            localizationprint.diagram_Fill.Content = dc.m_ResourceWrapper.PrintDialog_Stretch_Fill;
            localizationprint.diagarm_None.Content = dc.m_ResourceWrapper.PrintDialog_Stretch_None;
            localizationprint.diagram_Uniform.Content = dc.m_ResourceWrapper.PrintDialog_Stretch_Uniform;
            localizationprint.diagram_UniformtoFill.Content = dc.m_ResourceWrapper.PrintDialog_Stretch_UniformtoFill;
            //unregister the loaded event
            localizationprint.Loaded -= new RoutedEventHandler(printDialog_Loaded);
        }

        /// <summary>
        /// Print based on PrintParameter
        /// </summary>
        /// <param name="printParam"></param>
        public void Print(PrintParameters printParam)
        {
            if (printParam.ShowDialog)
            {
                this.Print();
            }
            else
            {
                DiagramPrintDialog printDialog = new DiagramPrintDialog();
                printDialog.Print(this.Page, printParam.PrintStretch);
            }
        }

        internal void SaveinternallyDiagram(Stream stream, Rect saveArea, BitmapEncoder encoder, Size imageSize,bool useVisualBrush)
        {
            int bmpSourceWidth = imageSize == default(Size) ? (int)saveArea.Width : (int)imageSize.Width;
            int bmpSourceHeight = imageSize == default(Size) ? (int)saveArea.Height : (int)imageSize.Height;
            RenderTargetBitmap bmpSource = new RenderTargetBitmap(
                     bmpSourceWidth,
                     bmpSourceHeight,
                     DefaultDim,
                     DefaultDim,
                     PixelFormats.Default);

            double x = -(this.Page as DiagramPage).Left;
            double y = -(this.Page as DiagramPage).Top;
            double w = (this.Page as DiagramPage).ActualWidth - x;
            double h = (this.Page as DiagramPage).ActualHeight - y;
            double backgroundWidth = w > bmpSourceWidth ? w : bmpSourceWidth;
            double backgroundHeight = h > bmpSourceHeight ? h : bmpSourceHeight;

            // Whole background of the image (white coloured area)
            Rectangle backgroud = new Rectangle();
            backgroud.Fill = Brushes.White;
            backgroud.Arrange(new Rect(0, 0, backgroundWidth, backgroundHeight));
            bmpSource.Render(backgroud);

            // Set the colour behind the diagram area in the exported image if PageBackground has been set
            Rectangle betweenRect = new Rectangle();
            if (this.PageBackground != null)
                betweenRect.Fill = this.PageBackground;
            else
                betweenRect.Fill = Brushes.White;

            if (this.BoundaryConstraintsEnabled)
                betweenRect.Arrange(new Rect(-(saveArea.X - this.BoundaryConstraintsArea.X), -(saveArea.Y - this.BoundaryConstraintsArea.Y), this.BoundaryConstraintsArea.Width, this.BoundaryConstraintsArea.Height));
            else
            {
                double betweenRectX = 0;
                double betweenRectY = 0;
                double betweenRectWidth = 0d;
                double betweenRectHeight = 0d;

                if (x >= 0)
                {
                    if (saveArea.X < 0)
                    {
                        betweenRectX = -saveArea.X;
                        betweenRectWidth = w;
                    }
                    else
                    {
                        betweenRectX = 0;
                        betweenRectWidth = w - saveArea.X;
                    }
                }
                else
                {
                    if (saveArea.X < x)
                    {
                        betweenRectX = x - saveArea.X;
                        betweenRectWidth = w;
                    }
                    else
                    {
                        betweenRectX = 0;
                        betweenRectWidth = w - (saveArea.X - x);
                    }
                }

                if (y >= 0)
                {
                    if (saveArea.Y < 0)
                    {
                        betweenRectY = -saveArea.Y;
                        betweenRectHeight = h;
                    }
                    else
                    {
                        betweenRectY = 0;
                        betweenRectHeight = h - saveArea.Y;
                    }
                }
                else
                {
                    if (saveArea.Y < y)
                    {
                        betweenRectY = y - saveArea.Y;
                        betweenRectHeight = h;
                    }
                    else
                    {
                        betweenRectY = 0;
                        betweenRectHeight = h - (saveArea.Y - y);
                    }
                }
                betweenRect.Arrange(new Rect(betweenRectX, betweenRectY, betweenRectWidth, betweenRectHeight));
            }

            bmpSource.Render(betweenRect);
            
            if (imageSize == default(Size))
            {
                if (useVisualBrush)
                {
                    VisualBrush visualBrush = new VisualBrush(Page);
                    Rectangle rect = new Rectangle();
                    visualBrush.ViewboxUnits = BrushMappingMode.Absolute;
                    visualBrush.Viewbox = new Rect(saveArea.X, saveArea.Y, saveArea.Width, saveArea.Height);
                    rect.Fill = visualBrush;
                    rect.Stretch = Stretch.Fill;
                    rect.Arrange(new Rect(saveArea.Size));
                    bmpSource.Render(rect);
                }
                else
                {
                    Visual visual = viewgrid as Visual;
                    bmpSource.Render(visual);
                }
            }
            else
            {
                VisualBrush visualBrush = new VisualBrush(Page);
                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                {
                    visualBrush.Stretch = Stretch.Fill;
                    drawingContext.DrawRectangle(visualBrush, null, new Rect(saveArea.X, saveArea.Y, imageSize.Width, imageSize.Height));
                }
                bmpSource.Render(drawingVisual);
            }
            encoder.Frames.Add(BitmapFrame.Create(bmpSource));
            encoder.Save(stream);
        }

        /*
        /// <summary>
        /// Saves Diagram to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="saveArea">The area rect.</param>
        /// <param name="encoder">The encoder.</param>
        internal void Saveinternally(Stream stream, Rect saveArea, BitmapEncoder encoder)
        {

            RenderTargetBitmap bmpSource = new RenderTargetBitmap(
                (int)saveArea.Width,
                (int)saveArea.Height,
                DefaultDim,
                DefaultDim,
                PixelFormats.Default);
            Rectangle backgroud = new Rectangle();
            backgroud.Fill = Brushes.White;
            backgroud.Arrange(new Rect(saveArea.X, saveArea.Y, saveArea.Width, saveArea.Height));           
            Visual visual = viewgrid as Visual;
            bmpSource.Render(backgroud);
            bmpSource.Render(visual);
            encoder.Frames.Add(BitmapFrame.Create(bmpSource));
            encoder.Save(stream);


        }

        internal void SaveinternallyHugeDiagram(Stream stream, Rect saveArea, BitmapEncoder encoder, string path, Size imageSize)
        {
            RenderTargetBitmap bmpSource = new RenderTargetBitmap(
                    (int)imageSize.Width,
                    (int)imageSize.Height,
                    DefaultDim,
                    DefaultDim,
                    PixelFormats.Default);
            Rectangle backgroud = new Rectangle();
            backgroud.Fill = Brushes.White;
            backgroud.Arrange(new Rect(saveArea.X, saveArea.Y, imageSize.Width, imageSize.Height));
            DrawingVisual drawingVisual = new DrawingVisual();

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(Page);
                brush.Stretch = Stretch.Fill;
                drawingContext.DrawRectangle(brush, null, new Rect(saveArea.X, saveArea.Y, imageSize.Width, imageSize.Height));
            }
            bmpSource.Render(backgroud);
            bmpSource.Render(drawingVisual);
            encoder.Frames.Add(BitmapFrame.Create(bmpSource));
            encoder.Save(stream);
        }

        /// <summary>
        /// Saves Diagram to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="saveArea">The area rect.</param>
        /// <param name="encoder">The encoder.</param>
        public void Save(Stream stream, Rect saveArea, BitmapEncoder encoder)
        {
            RenderTargetBitmap bmpSource = new RenderTargetBitmap(
                     (int)saveArea.Width,
                     (int)saveArea.Height,
                     DefaultDim,
                     DefaultDim,
                     PixelFormats.Default);
            VisualBrush visualBrush = new VisualBrush(Page);
            Rectangle rect = new Rectangle();
            Rectangle backgroundRect = new Rectangle();
            Rectangle betweenRect = new Rectangle();
            if (this.PageBackground != null)
                betweenRect.Fill = this.PageBackground;
            else
                betweenRect.Fill = Brushes.White;

            if (this.BoundaryConstraintsEnabled)
                betweenRect.Arrange(new Rect(-(saveArea.X - this.BoundaryConstraintsArea.X), -(saveArea.Y - this.BoundaryConstraintsArea.Y), this.BoundaryConstraintsArea.Width, this.BoundaryConstraintsArea.Height));
            else
                betweenRect.Arrange(new Rect(saveArea.Size));

            backgroundRect.Arrange(new Rect(saveArea.Size));
            visualBrush.ViewboxUnits = BrushMappingMode.Absolute;
            visualBrush.Viewbox = new Rect(saveArea.X, saveArea.Y, saveArea.Width, saveArea.Height);

            rect.Fill = visualBrush;
            rect.Stretch = Stretch.Fill;
            rect.Arrange(new Rect(saveArea.Size));

            bmpSource.Render(backgroundRect);
            bmpSource.Render(betweenRect);
            bmpSource.Render(rect);

            encoder.Frames.Add(BitmapFrame.Create(bmpSource));
            encoder.Save(stream);
        }
        */

        /// <summary>
        /// Saves Diagram to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="saveArea">The area rect.</param>
        /// <example>
        /// C#:
        /// <code language="C#">
        ///  System.IO.MemoryStream stream = new System.IO.MemoryStream();
        ///  Rect rect = new Rect(new Point(100, 100), new Point(500, 500));
        ///  diagramView.Save(stream, rect);
        /// </code>
        /// </example>
        public void Save(Stream stream, Rect saveArea)
        {
            //this.Save(stream, saveArea, new BmpBitmapEncoder());
            this.SaveinternallyDiagram(stream, saveArea, new BmpBitmapEncoder(),default(Size),true);
        }

        /// <summary>
        /// Saves Diagram to Diagram to specified file.
        /// </summary>
        /// <param name="fileName">The fileName.</param>
        /// <param name="saveArea">The save area.</param>
        /// <example>
        /// C#:
        /// <code language="C#">
        ///  sting filename = "NewFile";
        ///  Rect rect = new Rect(new Point(100, 100), new Point(500, 500));
        ///  diagramView.Save(filename, rect);
        /// </code>
        /// </example>
        public void Save(string fileName, Rect saveArea)
        {
            string extension = new FileInfo(fileName).Extension.ToLower(CultureInfo.InvariantCulture);

            using (Stream stream = File.Create(fileName))
            {
                if (extension == "xps")
                {
                    this.SaveToXps(stream, saveArea);
                }
                else
                {
                    //this.Save(stream, saveArea, DiagramView.CreateByExtension(extension));
                    this.SaveinternallyDiagram(stream, saveArea, DiagramView.CreateByExtension(extension),default(Size),true);
                }
            }
        }

        /// <summary>
        /// Saves the diagram into specified extensions.
        /// </summary>
        /// <param name="extension">The format into which to save.</param>
        /// <returns>The encoder.</returns>
        private static BitmapEncoder CreateByExtension(string extension)
        {
            BitmapEncoder encoder = null;

            switch (extension)
            {
                case ".bmp":
                    encoder = new BmpBitmapEncoder();
                    break;

                case ".jpg":
                case ".jpeg":
                    encoder = new JpegBitmapEncoder();
                    break;

                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;

                case ".gif":
                    encoder = new GifBitmapEncoder();
                    break;

                case ".tif":
                case ".tiff":
                    encoder = new TiffBitmapEncoder();
                    break;

                case ".wdp":
                    encoder = new WmpBitmapEncoder();
                    break;

                default:
                    encoder = new BmpBitmapEncoder();
                    break;
            }

            return encoder;
        }

        /// <summary>
        /// Saves Diagram to specified file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="saveArea">The save area.</param>
        /// <param name="encoder">The encoder.</param>
        public void Save(string fileName, Rect saveArea, BitmapEncoder encoder)
        {
            using (Stream stream = File.Create(fileName))
            {
                //this.Save(stream, saveArea, encoder);
                this.SaveinternallyDiagram(stream, saveArea, encoder,default(Size),true);
            }
        }

        /// <summary>
        /// Saves Diagram to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoder">The encoder.</param>
        /// <exception cref="ArgumentNullException">Internal border of Diagram control cannot be retrieved.</exception>
        public void Save(Stream stream, BitmapEncoder encoder)
        {
            Visual visual = this.Page as Visual;

            if (visual != null)
            {
                RenderTargetBitmap bmpSource = new RenderTargetBitmap(
                    (int)this.Page.ActualWidth,
                    (int)this.Page.ActualHeight,
                    DefaultDim,
                    DefaultDim,
                    PixelFormats.Default);

                Rectangle backgroundRect = new Rectangle();

                backgroundRect.Fill = Brushes.White;
                backgroundRect.Arrange(new Rect(this.Page.RenderSize));

                bmpSource.Render(backgroundRect);
                bmpSource.Render(visual);

                encoder.Frames.Add(BitmapFrame.Create(bmpSource));
                encoder.Save(stream);
            }
            else
            {
                throw new ArgumentNullException("Diagram View cannot be retrieved");
            }
        }

        /// <summary>
        /// Saves Diagram to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Save(Stream stream)
        {
            this.Save(stream, new BmpBitmapEncoder());
        }

        /// <summary>
        /// Saves Diagram to the file with specified filename.
        /// </summary>
        /// <param name="fileName">The filename.</param>
        public void Save(string fileName)
        {
            InternalSave(fileName, true);
        }

        /// <summary>
        /// Saves Diagram to the file with specified filename.
        /// </summary>
        /// <param name="fileName">The filename.</param>
        /// <param name="useVisualBrush"><para>Specify to export the diagram using visualbrush. </para> 
        /// <para>true:  Negative and positive side elements can be export </para>
        /// <para>false: Positive side elements only can export </para>
        /// </param>
        public void Save(string fileName,bool useVisualBrush)
        {
            InternalSave(fileName, useVisualBrush);
        }

        private void InternalSave(string fileName, bool useVisualBrush)
        {
            string extension = new FileInfo(fileName).Extension.ToLower(CultureInfo.InvariantCulture);

            double x = -(this.Page as DiagramPage).Left;
            double y = -(this.Page as DiagramPage).Top;
            double w = (this.Page as DiagramPage).DesiredSize.Width - x;
            double h = (this.Page as DiagramPage).DesiredSize.Height - y;

            using (Stream stream = File.Create(fileName))
            {
                if (extension == ".xps")
                {
                    this.SaveToXps(stream, new Rect(x, y, w, h));
                }
                else
                {
                    //this.Saveinternally(stream, new Rect(x, y, w, h), DiagramView.CreateByExtension(extension));
                    this.SaveinternallyDiagram(stream, new Rect(x, y, w, h), DiagramView.CreateByExtension(extension),default(Size), useVisualBrush);
                }
            }
        }

        /// <summary>
        /// Exports diagram as an image file.
        /// </summary>
        /// <param name="fileName">Name of the image file.</param>
        /// <param name="imageSize">Size of the image file</param>
        /// <param name="imageStretch"><para>Specify how to stretch the image. </para>
        /// <para>Expand: Image can expand but not shrink to fit the desired Size (image size) </para>
        /// <para>Shrink: Image can shrink but not expand to fit the desired Size (image size) </para>
        /// <para>Best Fit : Image will expand or shrink to fit the desired Size (image size) </para>
        /// </param>
        public void Save(string fileName, Size imageSize, ImageStretch imageStretch)
        {
            string extension = new FileInfo(fileName).Extension.ToLower(CultureInfo.InvariantCulture);

            double x = -(this.Page as DiagramPage).Left;
            double y = -(this.Page as DiagramPage).Top;
            double w = ((this.Page as DiagramPage).ActualWidth - x);
            double h = ((this.Page as DiagramPage).ActualHeight - y);
            using (Stream stream = File.Create(fileName))
            {
                if (extension == ".xps")
                {
                    this.SaveToXps(stream, new Rect(x, y, w, h));
                }
                else
                {
                    if (imageStretch == (ImageStretch.Shrink))
                    {
                        double xx = 0;
                        double yy = 0;
                        if (imageSize.Width < w)
                        {
                            xx = w / imageSize.Width;
                        }
                        else 
                        {
                            xx = imageSize.Width / w;
                        }
                        if (imageSize.Height < h)
                        {
                            yy =  h/imageSize.Height;
                        }
                        else
                        {
                            yy = imageSize.Height / h;
                        }
                        if (xx > yy)
                        {
                            if (imageSize.Width < w)
                            {
                                imageSize.Width = w / xx;
                                imageSize.Height = h / xx;
                            }
                            else
                            {
                                imageSize.Width = w;
                                imageSize.Height = h;
                            }

                        }
                        else
                        {
                            if (imageSize.Height < h)
                            {
                                imageSize.Width = w / yy;
                                imageSize.Height = h / yy;
                            }
                            else
                            {
                                imageSize.Width = w;
                                imageSize.Height = h;
                            }
                        }
                    }
                    else if (imageStretch == (ImageStretch.Expand))
                    {
                        double xx = 0;
                        double yy = 0;
                        if (imageSize.Width < w)
                        {
                            xx = w / imageSize.Width;
                        }
                        else
                        {
                            xx = imageSize.Width / w;
                        }
                        if (imageSize.Height < h)
                        {
                            yy = h / imageSize.Height;
                        }
                        else
                        {
                            yy = imageSize.Height / h;
                        }

                        if (xx > yy)
                        {
                            if (imageSize.Width < w)
                            {
                                imageSize.Width = w;
                                imageSize.Height = h;
                            }
                            else
                            {
                                imageSize.Width = w * yy;
                                imageSize.Height = h * yy;
                            }
                        }
                        else
                        {
                            if (imageSize.Height < h)
                            {
                                imageSize.Width = w;
                                imageSize.Height = h;
                            }
                            else
                            {
                                imageSize.Width = w * xx;
                                imageSize.Height = h * xx;
                            }
                        }
                    }
                    else if (imageStretch == ImageStretch.BestFit)
                    {
                        double xx = 0;
                        double yy = 0;
                        xx = imageSize.Width / w;
                        yy = imageSize.Height / h;
                        if (xx > yy)
                        {
                            imageSize.Width = imageSize.Width / (xx / yy);
                        }
                        else if (xx < yy)
                        {
                            imageSize.Height = imageSize.Height / (yy / xx);
                        }
                    }
                    else if (imageStretch == ImageStretch.None)
                    {
                        imageSize = new Size(w, h);
                    }

                    imageSize = new Size(Math.Round(imageSize.Width, 0), Math.Round(imageSize.Height, 0));
                    //this.SaveinternallyHugeDiagram(stream, new Rect(x, y, w, h), DiagramView.CreateByExtension(extension), fileName, ImageSize);
                    this.SaveinternallyDiagram(stream, new Rect(x, y, w, h), DiagramView.CreateByExtension(extension), imageSize,true);
                    
                }
            }
        }

        /// <summary>
        /// Saves Diagram to the file with specified filename using encoder.
        /// </summary>
        /// <param name="fileName">The fileName.</param>
        /// <param name="encoder">The encoder.</param>
        public void Save(string fileName, BitmapEncoder encoder)
        {
            using (Stream stream = File.Create(fileName))
            {
                this.Save(stream, encoder);
            }
        }

        /// <summary>
        /// Saves to XPS format.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void SaveToXps(Stream stream)
        {
            Package package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite);
            XpsDocument doc = new XpsDocument(package, CompressionOption.Normal);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);

            writer.Write(this.Page);

            doc.Close();
            package.Close();
        }

        /// <summary>
        /// Saves to XPS format.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void SaveToXps(string filename)
        {
            using (Stream stream = File.Create(filename))
            {
                this.SaveToXps(stream);
            }
        }

        /// <summary>
        /// Saves to XPS format.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="saveArea">The save area.</param>
        public void SaveToXps(Stream stream, Rect saveArea)
        {
            Package package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite);
            XpsDocument doc = new XpsDocument(package);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);

            VisualBrush visualBrush = new VisualBrush(this.Page);
            Rectangle rect = new Rectangle();

            visualBrush.Stretch = Stretch.Fill;
            visualBrush.Viewbox = new Rect(
                saveArea.X / this.Page.ActualWidth,
                saveArea.Y / this.Page.ActualHeight,
                saveArea.Width / this.Page.ActualWidth,
                saveArea.Height / this.Page.ActualHeight);

            rect.Fill = visualBrush;
            rect.Stretch = Stretch.Fill;
            rect.Arrange(new Rect(saveArea.Size));

            writer.Write(rect);

            doc.Close();
            package.Close();
        }

        /// <summary>
        /// Saves to XPS format.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="saveArea">The save area.</param>
        public void SaveToXps(string filename, Rect saveArea)
        {
            using (Stream stream = File.Create(filename))
            {
                this.SaveToXps(stream, saveArea);
            }
        }

        /// <summary>
        /// Copies Diagram to clipboard.
        /// </summary>
        /// <exception cref="ArgumentNullException">Internal border of Diagram control cannot be retrieved.</exception>
        public void CopyToClipboard()
        {
            Visual visual = this.Page as Visual;

            if (visual != null)
            {
                RenderTargetBitmap bmpSource = new RenderTargetBitmap(
                    (int)this.Page.ActualWidth,
                    (int)this.Page.ActualHeight,
                    DefaultDim,
                    DefaultDim,
                    PixelFormats.Pbgra32);

                Rectangle backgroundRect = new Rectangle();

                backgroundRect.Fill = Brushes.White;
                backgroundRect.Arrange(new Rect(this.Page.RenderSize));

                bmpSource.Render(backgroundRect);
                bmpSource.Render(visual);

                Clipboard.SetImage(bmpSource);
            }
            else
            {
                throw new ArgumentNullException("visual");
            }
        }
        #endregion
    }
}

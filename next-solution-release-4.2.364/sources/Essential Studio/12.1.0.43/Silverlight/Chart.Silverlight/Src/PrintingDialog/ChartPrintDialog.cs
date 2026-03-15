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
using System.Windows.Media.Imaging;
using System.Windows.Printing;
using System.Reflection;
using System.IO;

namespace Syncfusion.Windows.Chart
{
   
    /// <summary>
    /// Class implementation for ChartPrintDialog
    /// </summary>
    public class ChartPrintDialog: Control
    {


        /// <summary>
        ///  Identifies the PrintDialogStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ChartPrintDialogStyleProperty =
         DependencyProperty.Register("PrintDialogStyle", typeof(Style), typeof(ChartPrintDialog), new PropertyMetadata(null, new PropertyChangedCallback(OnChartPrintDialogStyleChanged)));

        /// <summary>
        /// Gets or sets the PrintDialogStyle value.
        /// </summary>
        /// <value>The PrintDialogStyle.</value>
        public Style PrintDialogStyle
        {
            get
            {
                return (Style)GetValue(ChartPrintDialogStyleProperty);
            }

            set
            {
                SetValue(ChartPrintDialogStyleProperty, value);
            }
        }

        /// <summary>
        /// Called when ChartPrintDislogStyle property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnChartPrintDialogStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartPrintDialog printdialog = d as ChartPrintDialog;
            if (printdialog != null)
            {

                printdialog.Style = printdialog.PrintDialogStyle;               

            }
        }
       
        private Chart PrintChart;
        private bool zoomtoolkit;
        private bool isprinting;
        private Visibility verticalscroll;
        private Visibility horizontalscroll;
        private Image PrintImage = new Image();
        private Grid mainGrid;
        private PrintDocument printDocument;
        private ChildWindow mychild;
        double height;
        double width;
        /// <summary>
        /// Called when instance created for ChartPrintDialog
       /// </summary>
        /// <param name="chart"></param>
        public ChartPrintDialog(Chart chart)
        {
            DefaultStyleKey = typeof(ChartPrintDialog);
            zoomtoolkit = chart.Areas[0].isSwitchzoom;
            verticalscroll = chart.Areas[0].VerticalBar.Visibility;
            horizontalscroll = chart.Areas[0].HorizontalBar.Visibility;
            mainGrid = chart.Parent as Grid;
            PrintChart = chart;
            WriteableBitmap bmpsource = new WriteableBitmap(PrintChart, null);
            PrintImage.Source = bmpsource;
            PrintImage.MaxHeight = chart.MaxHeight;
            PrintImage.MaxWidth = chart.MaxWidth;
            PrintImage.MinHeight = chart.MinHeight;
            PrintImage.MinWidth = chart.MinWidth;
            PrintImage.RenderTransform = chart.RenderTransform;
            PrintImage.RenderTransformOrigin = chart.RenderTransformOrigin;
            mainGrid.Children.Add(PrintImage);
            this.Loaded += new RoutedEventHandler(ChartPrintDialog_Loaded);
            this.Unloaded += new RoutedEventHandler(ChartPrintDialog_Unloaded);

        }

        /// <summary>
        /// Method implementation for Show Property window
        /// </summary>
        public void Show()
        {
            mychild = new ChildWindow();
            mychild.Title = "Chart Print Dialog";
            mychild.Content = this;
            mychild.Show();
        }

        void ChartPrintDialog_Unloaded(object sender, RoutedEventArgs e)
        {
            if (isprinting == false)
            {
                if (zoomtoolkit == true)
                {
                    ZoomingToolKit.SetZoomingToolkitVisibility(PrintChart.Areas[0], Visibility.Visible);
                }
                PrintChart.Areas[0].VerticalBar.Visibility = verticalscroll;
                PrintChart.Areas[0].HorizontalBar.Visibility = horizontalscroll;
                if (PrintChart.Areas[0].InteractiveCursors != null)
                {
                    PrintChart.Areas[0].InteractiveCursors[0].VerticalCursorVisibility = Visibility.Visible;
                    PrintChart.Areas[0].InteractiveCursors[0].HorizontalCursorVisibility = Visibility.Visible;
                }
                mainGrid.Children.Remove(PrintImage);
                PrintImage = null;
            }
        }

        void ChartPrintDialog_Loaded(object sender, RoutedEventArgs e)
        {
            
            height = PrintChart.Height;
            width = PrintChart.Width;            
            PrintChart.Height = BorderRect.Height/2;
            PrintChart.Width = BorderRect.Width;
            PrintChart.UpdateLayout();          

            WriteableBitmap bmpsource = new WriteableBitmap(PrintChart, null);
            ViewPortImage.Height = PrintChart.ActualHeight;
            ViewPortImage.Width = PrintChart.ActualWidth;
            ViewPortImage.Source = bmpsource;

            PrintChart.Height = height;
            PrintChart.Width = width;
            PrintChart.UpdateLayout();
           
            Printbutton.Click += new RoutedEventHandler(Printbutton_Click);
            Cancelbutton.Click += new RoutedEventHandler(Cancelbutton_Click);
            ShrinkToFitChk.Checked += new RoutedEventHandler(ShrinkToFitChk_Checked);
            ShrinkToFitChk.Unchecked += new RoutedEventHandler(ShrinkToFitChk_Unchecked);
            PrintModeCmb.SelectionChanged += new SelectionChangedEventHandler(PrintModeCmb_SelectionChanged);
            HideZoomingToolKitChk.Checked += new RoutedEventHandler(HideZoomingToolKitChk_Checked);
            HideZoomingToolKitChk.Unchecked += new RoutedEventHandler(HideZoomingToolKitChk_Unchecked);
            if (PrintChart.Areas[0].isSwitchzoom || PrintChart.Areas[0].InteractiveCursors != null)
            {
                HideZoomingToolKitChk.IsEnabled = true;
                HideZoomingToolKitChk.IsChecked = true;
            }
            
            
        }

        void PrintModeCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PrintModeCmb.SelectedIndex == 0)
            {
                if (ShrinkToFitChk.IsChecked == true)
                {
                    PrintChart.Height = BorderRect.Height / 2;
                    PrintChart.Width = BorderRect.Width - 1;
                    PrintChart.UpdateLayout();

                    WriteableBitmap bmpsource = new WriteableBitmap(PrintChart, null);
                    ViewPortImage.Height = PrintChart.ActualHeight;
                    ViewPortImage.Width = PrintChart.ActualWidth;
                    ViewPortImage.Source = bmpsource;
                    TransformGroup transformGroup = new TransformGroup();

                    transformGroup.Children.Add(new RotateTransform()
                    {
                        Angle = 0,
                        CenterX = ViewPortImage.Width / 2,
                        CenterY = ViewPortImage.Height / 2
                    });

                    ViewPortImage.RenderTransform = transformGroup;


                    PrintChart.Height = height;
                    PrintChart.Width = width;
                    PrintChart.UpdateLayout();
                }
                else
                {
                    PrintChart.Height = BorderRect.Height / 2;
                    PrintChart.Width = BorderRect.Width - 1;
                    PrintChart.UpdateLayout();

                    WriteableBitmap bmpsource = new WriteableBitmap(PrintChart, null);
                    ViewPortImage.Height = PrintChart.ActualHeight;
                    ViewPortImage.Width = PrintChart.ActualWidth;
                    ViewPortImage.Source = bmpsource;
                    TransformGroup transformGroup = new TransformGroup();

                    transformGroup.Children.Add(new RotateTransform()
                    {
                        Angle = 0,
                        CenterX = ViewPortImage.Width / 2,
                        CenterY = ViewPortImage.Height / 2
                    });

                    ViewPortImage.RenderTransform = transformGroup;


                    PrintChart.Height = height;
                    PrintChart.Width = width;
                    PrintChart.UpdateLayout();
                    WriteableBitmap bmpsourc = new WriteableBitmap(PrintChart, null);
                    ViewPortImage.Height = PrintChart.ActualHeight / 2;
                    ViewPortImage.Width = PrintChart.ActualWidth / 2;
                    ViewPortImage.Source = bmpsourc;
                }
            }
            else
            {
                if (ShrinkToFitChk.IsChecked == true)
                {
                    PrintChart.Height = BorderRect.Height / 2;
                    PrintChart.Width = BorderRect.Width;
                    PrintChart.UpdateLayout();

                    WriteableBitmap bmpsource = new WriteableBitmap(PrintChart, null);
                    ViewPortImage.Height = PrintChart.ActualHeight;
                    ViewPortImage.Width = PrintChart.ActualWidth;
                    ViewPortImage.Source = bmpsource;
                    TransformGroup transformGroup = new TransformGroup();

                    transformGroup.Children.Add(new RotateTransform()
                    {
                        Angle = 90,
                        CenterX = ViewPortImage.Width / 2,
                        CenterY = ViewPortImage.Height / 2
                    });

                    ViewPortImage.RenderTransform = transformGroup;

                    PrintChart.Height = height;
                    PrintChart.Width = width;
                    PrintChart.UpdateLayout();
                }
                else
                {
                    PrintChart.Height = BorderRect.Width;
                    PrintChart.Width = BorderRect.Height;
                    PrintChart.UpdateLayout();
                    WriteableBitmap bmpsource = new WriteableBitmap(PrintChart, null);
                    ViewPortImage.Height = PrintChart.ActualHeight;
                    ViewPortImage.Width = PrintChart.ActualWidth;
                    ViewPortImage.Source = bmpsource;
                    TransformGroup transformGroup = new TransformGroup();

                    transformGroup.Children.Add(new RotateTransform()
                    {
                        Angle = 90,
                        CenterX = ViewPortImage.Height / 2,
                        CenterY = 175
                    });

                    ViewPortImage.RenderTransform = transformGroup;
                    PrintChart.Height = height;
                    PrintChart.Width = width;
                    PrintChart.UpdateLayout();
                }
                
            }
        }

        void ShrinkToFitChk_Unchecked(object sender, RoutedEventArgs e)
        {
            if (PrintModeCmb.SelectedIndex == 0)
            {
                WriteableBitmap bmpsource = new WriteableBitmap(PrintChart, null);
                ViewPortImage.Height = PrintChart.ActualHeight / 2;
                ViewPortImage.Width = PrintChart.ActualWidth / 2;
                ViewPortImage.Source = bmpsource;
            }
            else
            {
                PrintChart.Height = BorderRect.Width;
                PrintChart.Width = BorderRect.Height;
                PrintChart.UpdateLayout();
                WriteableBitmap bmpsource = new WriteableBitmap(PrintChart, null);
                ViewPortImage.Height = PrintChart.ActualHeight;
                ViewPortImage.Width = PrintChart.ActualWidth;
                ViewPortImage.Source = bmpsource;
                TransformGroup transformGroup = new TransformGroup();

                transformGroup.Children.Add(new RotateTransform()
                {
                    Angle = 90,
                    CenterX =ViewPortImage.Height/2,
                    CenterY = 175
                });

                ViewPortImage.RenderTransform = transformGroup;
                PrintChart.Height = height;
                PrintChart.Width = width;
                PrintChart.UpdateLayout();
            }

        }

        void ShrinkToFitChk_Checked(object sender, RoutedEventArgs e)
        {
            if (PrintModeCmb.SelectedIndex != 0)
            {
                PrintChart.Height = BorderRect.Height / 2;
                PrintChart.Width = BorderRect.Width;
                PrintChart.UpdateLayout();

                WriteableBitmap bmpsource = new WriteableBitmap(PrintChart, null);
                ViewPortImage.Height = PrintChart.ActualHeight;
                ViewPortImage.Width = PrintChart.ActualWidth;
                ViewPortImage.Source = bmpsource;
                TransformGroup transformGroup = new TransformGroup();

                transformGroup.Children.Add(new RotateTransform()
                {
                    Angle = 90,
                    CenterX = ViewPortImage.Width / 2,
                    CenterY = ViewPortImage.Height / 2
                });

                ViewPortImage.RenderTransform = transformGroup;

                PrintChart.Height = height;
                PrintChart.Width = width;
                PrintChart.UpdateLayout();
            }
            else
            {
                PrintChart.Height = BorderRect.Height / 2;
                PrintChart.Width = BorderRect.Width;
                PrintChart.UpdateLayout();

                WriteableBitmap bmpsource = new WriteableBitmap(PrintChart, null);
                ViewPortImage.Height = PrintChart.ActualHeight;
                ViewPortImage.Width = PrintChart.ActualWidth;
                ViewPortImage.Source = bmpsource;

                PrintChart.Height = height;
                PrintChart.Width = width;
                PrintChart.UpdateLayout();
            }
        }       

        void HideZoomingToolKitChk_Unchecked(object sender, RoutedEventArgs e)
        {
            ZoomingToolKit.SetZoomingToolkitVisibility(PrintChart.Areas[0], Visibility.Visible);
            if (PrintChart.Areas[0].PrimaryAxis.ZoomFactor < 1)
            PrintChart.Areas[0].HorizontalBar.Visibility = System.Windows.Visibility.Visible;
            if (PrintChart.Areas[0].SecondaryAxis.ZoomFactor < 1)
            PrintChart.Areas[0].VerticalBar.Visibility = System.Windows.Visibility.Visible;
            if (PrintChart.Areas[0].InteractiveCursors != null)
            {
                PrintChart.Areas[0].InteractiveCursors[0].VerticalCursorVisibility = Visibility.Visible;
                PrintChart.Areas[0].InteractiveCursors[0].HorizontalCursorVisibility = Visibility.Visible;
            }
            if (ShrinkToFitChk.IsChecked == true)
            {
                PrintChart.Height = BorderRect.Height / 2;
                PrintChart.Width = BorderRect.Width ;
                PrintChart.UpdateLayout();

                WriteableBitmap bmpsource = new WriteableBitmap(PrintChart, null);
                ViewPortImage.Height = PrintChart.ActualHeight;
                ViewPortImage.Width = PrintChart.ActualWidth;
                ViewPortImage.Source = bmpsource;

                PrintChart.Height = height;
                PrintChart.Width = width;
                PrintChart.UpdateLayout();
            }
            else
            {
                if (PrintModeCmb.SelectedIndex != 0)
                {
                    PrintChart.Height = BorderRect.Width;
                    PrintChart.Width = BorderRect.Height;
                    PrintChart.UpdateLayout();
                    WriteableBitmap bmpsource = new WriteableBitmap(PrintChart, null);
                    ViewPortImage.Height = PrintChart.ActualHeight;
                    ViewPortImage.Width = PrintChart.ActualWidth;
                    ViewPortImage.Source = bmpsource;
                    TransformGroup transformGroup = new TransformGroup();

                    transformGroup.Children.Add(new RotateTransform()
                    {
                        Angle = 90,
                        CenterX = ViewPortImage.Height / 2,
                        CenterY = 175
                    });

                    ViewPortImage.RenderTransform = transformGroup;
                    PrintChart.Height = height;
                    PrintChart.Width = width;
                    PrintChart.UpdateLayout();
                }
                else
                {
                    WriteableBitmap bmpsource = new WriteableBitmap(PrintChart, null);
                    ViewPortImage.Height = PrintChart.ActualHeight / 2;
                    ViewPortImage.Width = PrintChart.ActualWidth / 2;
                    ViewPortImage.Source = bmpsource;
                }
                
            }

        }

        void HideZoomingToolKitChk_Checked(object sender, RoutedEventArgs e)
        {
            ZoomingToolKit.SetZoomingToolkitVisibility(PrintChart.Areas[0], Visibility.Collapsed);
            if(PrintChart.Areas[0].PrimaryAxis.ZoomFactor<1)
            PrintChart.Areas[0].HorizontalBar.Visibility = System.Windows.Visibility.Collapsed;
            if(PrintChart.Areas[0].SecondaryAxis.ZoomFactor<1)
            PrintChart.Areas[0].VerticalBar.Visibility = System.Windows.Visibility.Collapsed;
            if (PrintChart.Areas[0].InteractiveCursors != null)
            {
                PrintChart.Areas[0].InteractiveCursors[0].VerticalCursorVisibility = Visibility.Collapsed;
                PrintChart.Areas[0].InteractiveCursors[0].HorizontalCursorVisibility = Visibility.Collapsed;
            }
            if (ShrinkToFitChk.IsChecked == true)
            {
                PrintChart.Height = BorderRect.Height / 2;
                PrintChart.Width = BorderRect.Width ;
                PrintChart.UpdateLayout();

                WriteableBitmap bmpsource = new WriteableBitmap(PrintChart, null);
                ViewPortImage.Height = PrintChart.ActualHeight;
                ViewPortImage.Width = PrintChart.ActualWidth;
                ViewPortImage.Source = bmpsource;

                PrintChart.Height = height;
                PrintChart.Width = width;
                PrintChart.UpdateLayout();
            }
            else
            {
                if (PrintModeCmb.SelectedIndex != 0)
                {
                    PrintChart.Height = BorderRect.Width;
                    PrintChart.Width = BorderRect.Height;
                    PrintChart.UpdateLayout();
                    WriteableBitmap bmpsource = new WriteableBitmap(PrintChart, null);
                    ViewPortImage.Height = PrintChart.ActualHeight;
                    ViewPortImage.Width = PrintChart.ActualWidth;
                    ViewPortImage.Source = bmpsource;
                    TransformGroup transformGroup = new TransformGroup();

                    transformGroup.Children.Add(new RotateTransform()
                    {
                        Angle = 90,
                        CenterX = ViewPortImage.Height / 2,
                        CenterY = 175
                    });

                    ViewPortImage.RenderTransform = transformGroup;
                    PrintChart.Height = height;
                    PrintChart.Width = width;
                    PrintChart.UpdateLayout();
                }
                else
                {

                    WriteableBitmap bmpsource = new WriteableBitmap(PrintChart, null);
                    ViewPortImage.Height = PrintChart.ActualHeight / 2;
                    ViewPortImage.Width = PrintChart.ActualWidth / 2;
                    ViewPortImage.Source = bmpsource;
                }
                
            }
        }

        void Cancelbutton_Click(object sender, RoutedEventArgs e)
        {
            if (isprinting == false)
            {
                mainGrid.Children.Remove(PrintImage);
                PrintImage = null;
            }
            if (PrintChart.PrintDialogChildWindow != null)
                PrintChart.PrintDialogChildWindow.Close();
            if (mychild != null)
                mychild.Close();
        }

        void Printbutton_Click(object sender, RoutedEventArgs e)
        {
            
            printDocument = new PrintDocument();
            Thickness PageMargin = new Thickness(10);
            string Document = "Syncfusion Silverlight Chart";
            printDocument.PrintPage += delegate(object sender1, PrintPageEventArgs evt)
            {
                isprinting = true;
                TransformGroup transformGroup = new TransformGroup();

                //First move to middle of page...
                transformGroup.Children.Add(new TranslateTransform()
                {
                    X = (evt.PrintableArea.Width - PrintChart.ActualWidth) / 2,
                    Y = (evt.PrintableArea.Height - PrintChart.ActualHeight) / 2
                });
                double scale = 1;
                if (PrintModeCmb.SelectedIndex==1)
                {
                  //  Then, rotate around the center
                    transformGroup.Children.Add(new RotateTransform()
                    {
                        Angle = 90,
                        CenterX = evt.PrintableArea.Width / 2,
                        CenterY = evt.PrintableArea.Height / 2
                    });

                    if (ShrinkToFitChk.IsChecked==true)
                    {
                    if ((PrintChart.ActualWidth + PageMargin.Left +
                          PageMargin.Right) > evt.PrintableArea.Height)
                    {
                        scale = Math.Round(evt.PrintableArea.Height /
                          (PrintChart.ActualWidth + PageMargin.Left + PageMargin.Right), 2);
                    }
                    if ((PrintChart.ActualHeight + PageMargin.Top + PageMargin.Bottom) >
                                                evt.PrintableArea.Width)
                    {
                        double scale2 = Math.Round(evt.PrintableArea.Width /
                          (PrintChart.ActualHeight + PageMargin.Top + PageMargin.Bottom), 2);
                        scale = (scale2 < scale) ? scale2 : scale;
                    }
                    }
                }
                else if (ShrinkToFitChk.IsChecked==true)
                {
               // Scale down to fit the page + margin

                if ((PrintChart.ActualWidth + PageMargin.Left +
                        PageMargin.Right) > evt.PrintableArea.Width)
                {
                    scale = Math.Round(evt.PrintableArea.Width /
                      (PrintChart.ActualWidth + PageMargin.Left + PageMargin.Right), 2);
                }
                if ((PrintChart.ActualHeight + PageMargin.Top + PageMargin.Bottom) >
                             evt.PrintableArea.Height)
                {
                    double scale2 = Math.Round(evt.PrintableArea.Height /
                      (PrintChart.ActualHeight + PageMargin.Top + PageMargin.Bottom), 2);
                    scale = (scale2 < scale) ? scale2 : scale;
                }
                }

                //Scale down to fit the page + margin
                if (scale != 1)
                {
                    transformGroup.Children.Add(new ScaleTransform()
                    {
                        ScaleX = scale,
                        ScaleY = scale,
                        CenterX = evt.PrintableArea.Width / 2,
                        CenterY = evt.PrintableArea.Height / 2
                    });
                }

                evt.PageVisual = PrintChart;
                evt.PageVisual.RenderTransform = transformGroup;

            };

            printDocument.EndPrint += delegate(object sender2, EndPrintEventArgs evt)
            {
                    //Reset everything...
                    TransformGroup transformGroup = new TransformGroup();
                    transformGroup.Children.Add(
                      new ScaleTransform() { ScaleX = 1, ScaleY = 1 });
                    transformGroup.Children.Add(new RotateTransform() { Angle = 0 });
                    transformGroup.Children.Add(
                      new TranslateTransform() { X = 0, Y = 0 });
                    PrintChart.RenderTransform = transformGroup;
                if(PrintChart.PrintDialogChildWindow!=null)
                    PrintChart.PrintDialogChildWindow.Close();
                if(mychild!=null)
                mychild.Close();
                    if (zoomtoolkit == true)
                    {
                        ZoomingToolKit.SetZoomingToolkitVisibility(PrintChart.Areas[0], Visibility.Visible);
                    }
                    PrintChart.Areas[0].VerticalBar.Visibility = verticalscroll;
                    PrintChart.Areas[0].HorizontalBar.Visibility = horizontalscroll;
                    if (PrintChart.Areas[0].InteractiveCursors != null)
                    {
                        PrintChart.Areas[0].InteractiveCursors[0].VerticalCursorVisibility = Visibility.Visible;
                        PrintChart.Areas[0].InteractiveCursors[0].HorizontalCursorVisibility = Visibility.Visible;
                    }
                    isprinting = false;
                    mainGrid.Children.Remove(PrintImage);
                    PrintImage = null;
               
            };

            printDocument.Print(Document);

            
        }

        private Image ViewPortImage;
        private Button Cancelbutton;
        private Button Printbutton;
        private CheckBox ShrinkToFitChk;
        private ComboBox PrintModeCmb;
        private CheckBox HideZoomingToolKitChk;
        private CheckBox HideVertiScrollChk;
        private CheckBox HideHoriScrollChk;
        private Border previewborder;
        private Rectangle BorderRect;

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
           
            BorderRect = this.GetTemplateChild("BorderRect") as Rectangle;
            previewborder = this.GetTemplateChild("previewborder") as Border;
            ViewPortImage = this.GetTemplateChild("ViewPortImage") as Image;
            Printbutton = this.GetTemplateChild("Printbutton") as Button;
            Cancelbutton = this.GetTemplateChild("Cancelbutton") as Button;
            PrintModeCmb = this.GetTemplateChild("PrintModeCmb") as ComboBox;
            ShrinkToFitChk = this.GetTemplateChild("ShrinkToFitChk") as CheckBox;
            HideZoomingToolKitChk = this.GetTemplateChild("HideZoomingToolKitChk") as CheckBox;
            HideVertiScrollChk = this.GetTemplateChild("HideVertiScrollChk") as CheckBox;
            HideHoriScrollChk = this.GetTemplateChild("HideHoriScrollChk") as CheckBox;
            
        }
    }


}

#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
#if WPF
using System.Drawing.Printing;
using System.Printing;
using System.IO;
#endif
using System.Net;
using System.Windows;
#if !NETFX_CORE
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Ink;
#endif
using System.Windows.Input;

#if SILVERLIGHT_UNCOMMON
using System.Windows.Printing;
using System.Windows.Media.Imaging;
#endif
#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Printing;
using Windows.UI.Core;
using Windows.Graphics.Printing;
using Windows.Graphics.Printing.OptionDetails;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI;
#endif
#if NETFX_CORE8_1
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Pickers;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using System.IO;
#endif


namespace Syncfusion.UI.Xaml.Charts
{
    public class Printing
    {
        public Printing(ChartBase chart)
        {
            this.Chart = chart;
        }

        private ChartBase chart;
        public ChartBase Chart 
        {
            get { return chart; }
            set { chart = value; }
        }


#if WPF


        internal DrawingVisual GetDrawingVisual(FrameworkElement contentElement)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                VisualBrush contentBrush = new VisualBrush(contentElement)
                {
                    Stretch = Stretch.None,
                    AlignmentX = AlignmentX.Left,
                    AlignmentY = AlignmentY.Top
                };
                drawingContext.DrawRectangle(contentBrush, null, new Rect(0, 0,
                    contentElement.ActualWidth, contentElement.ActualHeight));
            }
            return drawingVisual;
        }
#endif
#if SILVERLIGHT_UNCOMMON
        public Image Layout(FrameworkElement element, Size printableArea, string Document,
                            HorizontalAlignment HorizontalAlignment, VerticalAlignment VerticalAlignment,
                            Thickness PageMargin, bool PrintLandscape, bool ShrinkToFit)
#endif

#if WPF
public Visual Layout(FrameworkElement element,Size PrintableArea, string Document, HorizontalAlignment HorizontalAlignment, VerticalAlignment VerticalAlignment, Thickness PageMargin, bool PrintLandscape, bool ShrinkToFit)
#endif
#if NETFX_CORE
        public FrameworkElement Layout(FrameworkElement element, Size PrintableArea, string Document, HorizontalAlignment HorizontalAlignment, VerticalAlignment VerticalAlignment, Thickness PageMargin, bool PrintLandscape, bool ShrinkToFit)
#endif
        {
#if SILVERLIGHT_UNCOMMON
            PrintDocument printDocument = new PrintDocument();
            if (PageMargin == null)
                PageMargin = new Thickness(10);
            Document = (string.IsNullOrEmpty(Document)) ? "Print Document" : Document;            
            Size PrintableArea = printableArea;
#endif
#if SILVERLIGHT_UNCOMMON || WPF
            Size elementSize=new Size(element.ActualWidth,element.ActualHeight);
#else
            Size elementSize = new Size(element.Width, element.Height);
#endif
            if (element.ActualWidth == double.NaN ||
                element.ActualHeight == double.NaN)
            {
                throw new Exception("Element must be rendered, " +
                                    "and must have a parent in order to print.");
            }

            TransformGroup transformGroup = new TransformGroup();
            ScaleTransform scaleTransform = new ScaleTransform();
            ScaleTransform horizonatlStretch = null;
            ScaleTransform verticalStrecth = null;
            //First move to middle of page...
            transformGroup.Children.Add(new TranslateTransform()
                {
                    X = (PrintableArea.Width - elementSize.Width)/2,
                    Y = (PrintableArea.Height - elementSize.Height)/2
                });
            double scaleX = 1;
            double scaleY = 1;
            if (PrintLandscape)
            {
                //Then, rotate around the center
                transformGroup.Children.Add(new RotateTransform()
                    {
                        Angle = 90,
                        CenterX = PrintableArea.Width/2,
                        CenterY = PrintableArea.Height/2
                    });

                if (ShrinkToFit)
                {
                    if ((elementSize.Width + PageMargin.Left +
                         PageMargin.Right) > PrintableArea.Height)
                    {
                        //elementSize.Width = PrintableArea.Width;
                        scaleX = Math.Round(PrintableArea.Height/
                                            (elementSize.Width + PageMargin.Left + PageMargin.Right), 2);
                    }
                    if ((elementSize.Height + PageMargin.Top + PageMargin.Bottom) >
                        PrintableArea.Width)
                    {
                        double scale2 = Math.Round(PrintableArea.Width/
                                                   (elementSize.Height + PageMargin.Top + PageMargin.Bottom), 2);
                        scaleY = (scale2 < scaleY) ? scale2 : scaleY;
                    }
                }
            }
            else if (ShrinkToFit)
            {
                //Scale down to fit the page + margin

                if ((elementSize.Width + PageMargin.Left +
                     PageMargin.Right) > PrintableArea.Width)
                {
                    //elementSize.Width = PrintableArea.Width;
                    scaleX = Math.Round(PrintableArea.Width/
                                        (elementSize.Width + PageMargin.Left + PageMargin.Right), 2);
                }
                if ((elementSize.Height + PageMargin.Top + PageMargin.Bottom) >
                    PrintableArea.Height)
                {
                    double scale2 = Math.Round(PrintableArea.Height/
                                               (elementSize.Height + PageMargin.Top + PageMargin.Bottom), 2);
                    scaleY = (scale2 < scaleY) ? scale2 : scaleY;
                }
            }

            //Scale down to fit the page + margin
            //if (scaleX != 1)
            //{
            scaleTransform = new ScaleTransform()
                {
                    ScaleX = scaleX,
                    ScaleY = scaleY,
                    CenterX = PrintableArea.Width/2,
                    CenterY = PrintableArea.Height/2
                };
            //}

            if (VerticalAlignment == VerticalAlignment.Top)
            {
                //Now move to Top
                if (PrintLandscape)
                {
                    transformGroup.Children.Add(new TranslateTransform()
                        {
                            X = 0,
                            Y = PageMargin.Top - (PrintableArea.Height -
                                                  (elementSize.Width*scaleY))/2
                        });
                }
                else
                {
                    transformGroup.Children.Add(new TranslateTransform()
                        {
                            X = 0,
                            Y = PageMargin.Top - (PrintableArea.Height -
                                                  (elementSize.Height*scaleX))/2
                        });
                }
            }
            else if (VerticalAlignment == VerticalAlignment.Bottom)
            {
                //Now move to Bottom
                if (PrintLandscape)
                {
                    transformGroup.Children.Add(new TranslateTransform()
                        {
                            X = 0,
                            Y = ((PrintableArea.Height -
                                  (elementSize.Width*scaleY))/2) - PageMargin.Bottom
                        });
                }
                else
                {
                    transformGroup.Children.Add(new TranslateTransform()
                        {
                            X = 0,
                            Y = ((PrintableArea.Height -
                                  (elementSize.Height*scaleY))/2) - PageMargin.Bottom
                        });
                }
            }
            else if (VerticalAlignment == VerticalAlignment.Stretch)
            {
                scaleY = Math.Round(PrintableArea.Height/(elementSize.Height + PageMargin.Top + PageMargin.Bottom), 2);
                verticalStrecth = new ScaleTransform()
                    {
                        ScaleX = scaleX,
                        ScaleY =
                            Math.Round(PrintableArea.Height/(elementSize.Height + PageMargin.Top + PageMargin.Bottom), 2),
                        CenterX = PrintableArea.Width/2,
                        CenterY = PrintableArea.Height/2
                    };

            }
            if (HorizontalAlignment == HorizontalAlignment.Left)
            {
                //Now move to Left
                if (PrintLandscape)
                {
                    transformGroup.Children.Add(new TranslateTransform()
                        {
                            X = PageMargin.Left - (PrintableArea.Width -
                                                   (elementSize.Height*scaleY))/2,
                            Y = 0
                        });
                }
                else
                {
                    transformGroup.Children.Add(new TranslateTransform()
                        {
                            X = PageMargin.Left - (PrintableArea.Width -
                                                   (elementSize.Width*scaleY))/2,
                            Y = 0
                        });
                }
            }
            else if (HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                scaleX = Math.Round(PrintableArea.Width/(elementSize.Width + PageMargin.Left + PageMargin.Right), 2);
                horizonatlStretch = new ScaleTransform()
                    {
                        ScaleX = Math.Round(PrintableArea.Width/(elementSize.Width + PageMargin.Left + PageMargin.Right), 2),
                        ScaleY = scaleY,
                        CenterX = PrintableArea.Width/2,
                        CenterY = PrintableArea.Height/2
                    };

            }
            else if (HorizontalAlignment == HorizontalAlignment.Right)
            {
                //Now move to Right
                if (PrintLandscape)
                {
                    transformGroup.Children.Add(new TranslateTransform()
                        {
                            X = ((PrintableArea.Width -
                                  (elementSize.Height*scaleX))/2) - PageMargin.Right,
                            Y = 0
                        });
                }
                else
                {
                    transformGroup.Children.Add(new TranslateTransform()
                        {
                            X = ((PrintableArea.Width -
                                  (elementSize.Width*scaleX))/2) - PageMargin.Right,
                            Y = 0
                        });
                }
            }
            if (verticalStrecth != null)
            {
                transformGroup.Children.Add(verticalStrecth);
            }
            if (horizonatlStretch != null)
            {
                if (transformGroup.Children.Contains(verticalStrecth))
                    transformGroup.Children.Remove(verticalStrecth);
                transformGroup.Children.Add(horizonatlStretch);
            }
            if (horizonatlStretch == null && verticalStrecth == null)
            {
                transformGroup.Children.Add(scaleTransform);
            }
#if NETFX_CORE
            element.RenderTransform = transformGroup;
            return element;
#endif
#if WPF
                DrawingVisual visual=GetDrawingVisual(element);
                visual.Transform = transformGroup;       
                return visual;
#endif
#if SILVERLIGHT_UNCOMMON
            WriteableBitmap bitmap = new WriteableBitmap((int) PrintableArea.Width, (int) PrintableArea.Height);
            bitmap.Render(element, transformGroup);
            bitmap.Invalidate();

            Image img = new Image();
            img.Width = (int) PrintableArea.Width;
            img.Height = (int) PrintableArea.Height;
            img.Source = bitmap;

            return img;
#endif
        }

        //Print Chart in WinRT
#if NETFX_CORE
#if NETFX_CORE8_1
        Image image;
        const double imageResolution = 96.0;
#else
        private ChartBase printChart;
#endif
        PrintManager printManager;
        private PrintDocument document;
        private Size? _pageSize;
        private Rect? _imageableRect;
        private UIElement page;
        private Thickness margin;
        private HorizontalAlignment horizontalAllignment;
        private VerticalAlignment verticalAllignment;
        private bool isLandscape=false;
        private bool isRegisteredForPrinting;

        public void RegisterPrinting()
        {
#if !NETFX_CORE8_1
            this.printChart =(ChartBase) this.chart.Clone();
#else
            RenderImage();
#endif
            printManager = PrintManager.GetForCurrentView();
            printManager.PrintTaskRequested += printManager_PrintTaskRequested;
            margin = new Thickness(0);
            horizontalAllignment = HorizontalAlignment.Stretch;
            verticalAllignment = VerticalAlignment.Stretch;
            
        }
#if NETFX_CORE8_1      
        private async void RenderImage()
        {
            image = new Image();
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(chart);
            var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();
            var memoryStream = new InMemoryRandomAccessStream();
            var dataWriter = new Windows.Storage.Streams.DataWriter(memoryStream);
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, memoryStream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)renderTargetBitmap.PixelWidth,
                                (uint)renderTargetBitmap.PixelHeight, imageResolution, imageResolution, pixelBuffer.ToArray());
            await encoder.FlushAsync();
            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(memoryStream);
            image.Height = renderTargetBitmap.PixelHeight;
            image.Width = renderTargetBitmap.PixelWidth;
            image.Source = bitmap;
            image.UpdateLayout();
        }
#endif
        public void UnRegisterPrinting()
        {
                printManager.PrintTaskRequested -= printManager_PrintTaskRequested;
                if (document == null)
                    return;
                document.Paginate -= document_Paginate;
                document.GetPreviewPage -= document_GetPreviewPage;
                document.AddPages -= document_AddPages;
                page = null;
        }
        void AddCustomPrintOption(PrintTask printTask)
        {
            PrintTaskOptionDetails Options = PrintTaskOptionDetails.GetFromPrintTaskOptions(printTask.Options);
            PrintCustomItemListOptionDetails horizontalOptions = Options.CreateItemListOption("horizontalAllignment", "HorizontalAllignment");
            var horizontaloptions = Enum.GetNames(typeof(HorizontalAlignment));
            foreach (var item in horizontaloptions)
            {
                horizontalOptions.AddItem(item, item);
            }
            horizontalOptions.TrySetValue(horizontaloptions[3]);
            PrintCustomItemListOptionDetails verticalOptions = Options.CreateItemListOption("verticalAllignment", "VerticalAllignment");
            var verticaloptions = Enum.GetNames(typeof(VerticalAlignment));
            foreach (var item in verticaloptions)
            {
                verticalOptions.AddItem(item, item);
            }
            verticalOptions.TrySetValue(verticaloptions[3]);
            PrintCustomTextOptionDetails marginText = Options.CreateTextOption("marginText", "Margin");
            Options.DisplayedOptions.Add("horizontalAllignment");
            Options.DisplayedOptions.Add("verticalAllignment");
            Options.DisplayedOptions.Add("marginText");
            Options.OptionChanged += details_OptionChanged;
        }

        void details_OptionChanged(PrintTaskOptionDetails sender, PrintTaskOptionChangedEventArgs args)
        {
            if (args.OptionId != null)
            {
               
                if ((string)args.OptionId == "horizontalAllignment")
                {
                    PrintTaskOptionDetails optionDetails = (PrintTaskOptionDetails)sender;
                    string value = optionDetails.Options["horizontalAllignment"].Value as string;

                    if (!string.IsNullOrEmpty(value))
                    {
                     IAsyncAction action = Chart.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                            () =>
                            {
                                if (value == "Left")
                                    horizontalAllignment = HorizontalAlignment.Left;
                                if (value == "Right")
                                    horizontalAllignment = HorizontalAlignment.Right;
                                if (value == "Center")
                                    horizontalAllignment = HorizontalAlignment.Center;
                                if (value == "Stretch")
                                    horizontalAllignment = HorizontalAlignment.Stretch;
                                document.InvalidatePreview();
                            });
                    }
                }

                if ((string)args.OptionId == "verticalAllignment")
                {
                    PrintTaskOptionDetails optionDetails = (PrintTaskOptionDetails)sender;
                    string value = optionDetails.Options["verticalAllignment"].Value as string;

                    if (!string.IsNullOrEmpty(value))
                    {
                        IAsyncAction action = Chart.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                            () =>
                            {
                                if (value == "Bottom")
                                    verticalAllignment = VerticalAlignment.Bottom;
                                if (value == "Top")
                                    verticalAllignment = VerticalAlignment.Top;
                                if (value == "Center")
                                    verticalAllignment = VerticalAlignment.Center;
                                if (value == "Stretch")
                                    verticalAllignment = VerticalAlignment.Stretch;
                                document.InvalidatePreview();
                            });
                    }
                }
                if ((string)args.OptionId == "marginText")
                {
                    PrintTaskOptionDetails optionDetails = (PrintTaskOptionDetails)sender;
                    string value = optionDetails.Options["marginText"].Value as string;

                    if (!string.IsNullOrEmpty(value))
                    {
                        IAsyncAction action = Chart.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                            () =>
                            {
                                double marginValue = 0;

                                if (double.TryParse(value, out marginValue))
                                {
                                    margin = new Thickness(marginValue);
                                }
                                document.InvalidatePreview();
                            });
                    }
                }
            }

        }

        void printManager_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            var deferral = args.Request.GetDeferral();
            PrintTask printTask = args.Request.CreatePrintTask("Printing", OnPrintTaskSourceRequestedHandler);
            printTask.Completed += OnPrintTaskCompleted;
            this.AddCustomPrintOption(printTask);
            deferral.Complete();
        }

       async void OnPrintTaskCompleted(PrintTask sender, PrintTaskCompletedEventArgs args)
        {
            if (isRegisteredForPrinting)
            {
                await Chart.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, UnRegisterPrinting);
                isRegisteredForPrinting = false;
#if !NETFX_CORE8_1
                printChart = null;
#endif
            }
        }

        async void OnPrintTaskSourceRequestedHandler(PrintTaskSourceRequestedArgs args)
        {
            var deferral = args.GetDeferral();

            await Chart.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(
                 () =>
                 {
                     document = new PrintDocument();
                     document.GetPreviewPage += document_GetPreviewPage;
                     document.AddPages += document_AddPages;
                     document.Paginate += document_Paginate;
                     args.SetSource(document.DocumentSource);
                 }));

            deferral.Complete();
        }

        async void document_Paginate(object sender, PaginateEventArgs e)
        {
            await Chart.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
             () =>
             {
                 this.GetPageSize(e);
                 document.SetPreviewPageCount(1, PreviewPageCountType.Intermediate);
             });
        }

        async void document_AddPages(object sender, AddPagesEventArgs e)
        {
            await Chart.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () =>
                 {
#if NETFX_CORE8_1
                     document.AddPage(image);
#else
                     document.AddPage(printChart);
#endif
                     document.AddPagesComplete();
                 });
        }

        async void document_GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            await Chart.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    PrintDocument printDoc = (PrintDocument)sender;
#if !NETFX_CORE8_1      
                        printChart.Height = printChart.Height > _pageSize.Value.Height ? _pageSize.Value.Height : printChart.Height;
                        printChart.Width = printChart.Width > _pageSize.Value.Width ? _pageSize.Value.Width : printChart.Width;

                        page = this.Layout(printChart, new Size(_pageSize.Value.Width, _pageSize.Value.Height), "Printing", horizontalAllignment, verticalAllignment, margin, isLandscape, true);
                        printDoc.SetPreviewPage(e.PageNumber, page);
#else
                        image.Height = image.Height > _pageSize.Value.Height ? _pageSize.Value.Height : image.Height;
                        image.Width = image.Width > _pageSize.Value.Width ? _pageSize.Value.Width : image.Width;
                        page = this.Layout(image, new Size(_pageSize.Value.Width, _pageSize.Value.Height), "Printing", horizontalAllignment, verticalAllignment, margin, isLandscape, true);
                        page.UpdateLayout();
#endif
#if !NETFX_CORE8_1
                        if (printChart is SfChart)
                        {
                            var sfChart = printChart as SfChart;

                            for (int i = 0; i < sfChart.Behaviors.Count; i++)
                            {
                                if (sfChart.Behaviors[i] is ChartTrackBallBehavior)
                                {
                                    (sfChart.Behaviors[i] as ChartTrackBallBehavior).IsActivated = true;
                                    (sfChart.Behaviors[i] as ChartTrackBallBehavior).CurrentPoint = ((chart as SfChart).Behaviors[i] as ChartTrackBallBehavior).CurrentPoint;
                                    (sfChart.Behaviors[i] as ChartTrackBallBehavior).OnPointerPositionChanged();
                                }
                                if (sfChart.Behaviors[i] is ChartCrossHairBehavior)
                                {
                                    (sfChart.Behaviors[i] as ChartCrossHairBehavior).SetPosition(((chart as SfChart).Behaviors[i] as ChartCrossHairBehavior).CurrentPoint);
                                }
                            }
                            if (sfChart.renderSeriesAction != null)
                            {
                                sfChart.RenderSeries();
                            }
                        }
                        else
                        {
                            var sfChart3D = printChart as SfChart3D;
                            if (sfChart3D.renderSeriesAction != null)
                            {
                                sfChart3D.RenderSeries();
                            }
                        }
#endif
                        printDoc.SetPreviewPage(e.PageNumber, page);
                });
        }

        void GetPageSize(PaginateEventArgs e)
        {
            //if (this._pageSize == null)
            //{
                PrintPageDescription description = e.PrintTaskOptions.GetPageDescription(
                  (uint)e.CurrentPreviewPageNumber);

                this._pageSize = description.PageSize;
                this._imageableRect = description.ImageableRect;
            //}
        }

        async public void Print()
        {
            if (!isRegisteredForPrinting)
            {
                this.RegisterPrinting();
                isRegisteredForPrinting = true;
            }
            if (ApplicationView.Value != ApplicationViewState.Snapped)
            {
                await Windows.Graphics.Printing.PrintManager.ShowPrintUIAsync();
            }
        }

#endif

    }
  
}

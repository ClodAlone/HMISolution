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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Syncfusion.UI.Xaml.Diagram.Utility;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Shapes;
namespace Syncfusion.UI.Xaml.Diagram.Controller
{
    internal class ExportController : ISharedData
    {
        private SharedData _mSharedData;

        internal Canvas PrintContainer { get; set; }

        public void Init(SharedData shared)
        {
            _mSharedData = shared;
        }

        public void Dispose()
        {

        }

        /// <summary>
        /// Export the Sfdiagram as single Image when the ImageStretch is Stretch
        /// </summary>
        /// <param name="Settings"></param>
        /// <returns></returns>
        internal async Task Export(ExportSettings Settings)
        {
            Rect CompareRect = Rect.Empty;
            if (!Settings.Clip.Equals(Rect.Empty))
            {
                CompareRect = Settings.Clip;
            }
            else
            {
                if (Settings.ExportMode == ExportMode.Content)
                {
                    if (_mSharedData.SpatialSearch._pageRight != long.MinValue)
                    {
                        CompareRect = new Rect(0, 0, _mSharedData.SpatialSearch._pageRight - _mSharedData.SpatialSearch._pageLeft, _mSharedData.SpatialSearch._pageBottom - _mSharedData.SpatialSearch._pageTop);
                    }
                    else
                    {
                        CompareRect = new Rect(0, 0, 0, 0);
                    }
                }
                else
                {
                    CompareRect = new Rect(0, 0, _mSharedData.Graph.PageSettings.PageWidth, _mSharedData.Graph.PageSettings.PageHeight);
                }

            }
            await CommonExport(Settings, CompareRect);
        }

        /// <summary>
        /// Common funtion for exporting Sfdiagram into Single Page and Multiple Page
        /// </summary>
        /// <param name="Settings"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private async Task CommonExport(ExportSettings Settings, Rect Bounds)
        {
            ////Converting Grid as Image
            Grid g = await RenderDiagramasImage(Bounds, Settings.RowCount, Settings.ColumnCount, Settings.ImageStretch, false);
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(g);

            var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();
            BitmapEncoder encoder = _mSharedData.Graph.ExportSettings.ExportBitmapEncoder;
            if (_mSharedData.Graph.ExportSettings.ImageStretch == Stretch.None)
            {
                encoder.BitmapTransform.Bounds = new BitmapBounds() { X = 0, Y = 0, Width = (uint)Bounds.Width, Height = (uint)Bounds.Height };
            }

            encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;
            encoder.SetPixelData(
            _mSharedData.Graph.ExportSettings.BitmapPixelFormat,
            _mSharedData.Graph.ExportSettings.BitmapAlphaMode,
            (uint)renderTargetBitmap.PixelWidth,
            (uint)renderTargetBitmap.PixelHeight,
            DisplayInformation.GetForCurrentView().LogicalDpi,
            DisplayInformation.GetForCurrentView().LogicalDpi,
             pixelBuffer.ToArray());
            await encoder.FlushAsync();

        }

        //Clipping-While Multiple Page is False
        private void UpdatePageSetting(Grid g, Image i)
        {
            g.Width = _mSharedData.Graph.PageSettings.PageWidth;
            g.Height = _mSharedData.Graph.PageSettings.PageHeight;
            if (_mSharedData.Graph.ExportSettings.ImageStretch == Stretch.Fill)
            {
                i.Width = _mSharedData.SpatialSearch._pageRight;
                i.Height = _mSharedData.SpatialSearch._pageBottom;
            }
            else if (_mSharedData.Graph.ExportSettings.ImageStretch == Stretch.Uniform)
            {
                double ratio = Math.Min(_mSharedData.Graph.PageSettings.PageWidth / _mSharedData.Graph.PageSettings.PageWidth, _mSharedData.Graph.PageSettings.PageHeight / _mSharedData.Graph.PageSettings.PageHeight);
                i.Width = _mSharedData.SpatialSearch._pageRight * ratio;
                i.Height = _mSharedData.SpatialSearch._pageBottom * ratio;
            }
            Rect rt = _mSharedData.Graph.ExportSettings.Clip;
            if (!_mSharedData.Graph.ExportSettings.Clip.Equals(Rect.Empty))
            {
                if (_mSharedData.Graph.PageSettings.PageWidth < _mSharedData.Graph.ExportSettings.Clip.Width)
                {
                    rt.Width = _mSharedData.Graph.PageSettings.PageWidth;
                }

                if (_mSharedData.Graph.PageSettings.PageHeight < _mSharedData.Graph.ExportSettings.Clip.Height)
                {
                    rt.Height = _mSharedData.Graph.PageSettings.PageHeight;
                }

                if (_mSharedData.Graph.PageSettings.PageWidth < (_mSharedData.Graph.ExportSettings.Clip.Width + _mSharedData.Graph.ExportSettings.Clip.X))
                {
                    rt.Width = _mSharedData.Graph.PageSettings.PageWidth;
                }

                if (_mSharedData.Graph.PageSettings.PageHeight < (_mSharedData.Graph.ExportSettings.Clip.Height + _mSharedData.Graph.ExportSettings.Clip.Y))
                {
                    rt.Width = _mSharedData.Graph.PageSettings.PageHeight;
                }
                g.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, rt.Width, rt.Height) };

            }
        }

        /// <summary>
        /// Used to Export the SfDiagram as Image based on the Current PageNumber when the ImageStretch is None
        /// </summary>
        /// <param name="Settings"></param>
        /// <param name="CurrentPagenumber"></param>
        /// <returns></returns>
        internal async Task Export(ExportSettings Settings, int RowNo, int ColumnNo)
        {
            if (_mSharedData.Graph.ExportSettings.RowCount > 1 || _mSharedData.Graph.ExportSettings.ColumnCount > 1)
            {
                double CompareWidth = 0;
                double CompareHeight = 0;
                double GridWidth = 0;
                double GridHeight = 0;
                CompareWidth = _mSharedData.Graph.PageSettings.PageWidth;
                CompareHeight = _mSharedData.Graph.PageSettings.PageHeight;
                uint offx = (uint)ColumnNo * (uint)CompareWidth;
                uint offy = (uint)RowNo * (uint)CompareHeight;
                if (!Settings.Clip.Equals(Rect.Empty))
                {
                    if ((Settings.Clip.X + Settings.Clip.Width) < _mSharedData.Graph.PageSettings.PageWidth)
                    {
                        CompareWidth = Settings.Clip.Width;
                    }
                    if ((Settings.Clip.Y + Settings.Clip.Height) < _mSharedData.Graph.PageSettings.PageHeight)
                    {
                        CompareHeight = Settings.Clip.Height;
                    }
                    offx = (uint)Settings.Clip.X;
                    offy = (uint)Settings.Clip.Y;
                }
                //Width and Height Calculation
                if (_mSharedData.SpatialSearch._pageRight < CompareWidth)
                {
                    GridWidth = CompareWidth;
                }
                else if ((offx + CompareWidth) < _mSharedData.SpatialSearch._pageRight)
                {
                    GridWidth = _mSharedData.SpatialSearch._pageRight;
                }
                else
                {
                    GridWidth = _mSharedData.SpatialSearch._pageRight + CompareWidth;
                }

                if (_mSharedData.SpatialSearch._pageBottom < _mSharedData.Graph.PageSettings.PageHeight)
                {
                    GridHeight = _mSharedData.Graph.PageSettings.PageHeight;
                }
                else if ((offy + _mSharedData.Graph.PageSettings.PageHeight) < _mSharedData.SpatialSearch._pageBottom)
                {
                    GridHeight = _mSharedData.SpatialSearch._pageBottom;
                }
                else if ((offy + _mSharedData.Graph.PageSettings.PageHeight) > _mSharedData.SpatialSearch._pageBottom)
                {
                    GridHeight = _mSharedData.SpatialSearch._pageBottom + _mSharedData.Graph.PageSettings.PageHeight;
                }

                //Realization Part
                if (_mSharedData.Graph.Constraints.Contains(GraphConstraints.Virtualize))
                {
                    if (_mSharedData.Graph.ExportSettings.RowCount > 1)
                    {
                        if ((offy + _mSharedData.Graph.PageSettings.PageHeight) > _mSharedData.Graph.ScrollInfo.Viewport.Height)
                        {
                            _mSharedData.VirtualizingController.Realize(new Rect(offx, offy, CompareWidth, CompareHeight));
                        }
                    }
                    if (_mSharedData.Graph.ExportSettings.ColumnCount > 1)
                    {
                        if ((offx + CompareWidth) > _mSharedData.Graph.ScrollInfo.Viewport.Width)
                        {
                            _mSharedData.VirtualizingController.Realize(new Rect(offx, offy, CompareWidth, CompareHeight));

                        }
                    }
                }

                //Invoking the Common funtion for exporting
                await CommonExport(Settings, new Rect(offx, offy, CompareWidth, CompareHeight));

                //Virtualization Part
                if (_mSharedData.Graph.Constraints.Contains(GraphConstraints.Virtualize))
                {
                    if (_mSharedData.Graph.ExportSettings.RowCount > 1)
                    {
                        if ((offy + _mSharedData.Graph.PageSettings.PageHeight) > _mSharedData.Graph.ScrollInfo.Viewport.Height)
                        {
                            _mSharedData.VirtualizingController.Virtualize(new Rect(offx, offy, CompareWidth, CompareHeight));
                        }
                    }
                    if (_mSharedData.Graph.ExportSettings.ColumnCount > 1)
                    {
                        if ((offx + CompareWidth) > _mSharedData.Graph.ScrollInfo.Viewport.Width)
                        {
                            _mSharedData.VirtualizingController.Virtualize(new Rect(offx, offy, CompareWidth, CompareHeight));

                        }
                    }
                }
            }
            else if (_mSharedData.Graph.ExportSettings.RowCount == 1 || _mSharedData.Graph.ExportSettings.ColumnCount == 1)
            {
                await Export(Settings);
            }
        }

        internal async Task<Grid> RenderDiagramasImage(Rect Bounds, int vcount, int hcount, Stretch Currentstretch, bool IsPrint)
        {
            if ((IsPrint && Currentstretch == Stretch.None) || (_mSharedData.Graph.ExportSettings != null && _mSharedData.Graph.ExportSettings.ExportMode == ExportMode.PageSettings && Currentstretch == Stretch.None))
            {
                RenderTargetBitmap bitmap = new RenderTargetBitmap();
                await bitmap.RenderAsync(_mSharedData.Graph.Page);
                Grid g = new Grid();
                g.HorizontalAlignment = HorizontalAlignment.Left;
                g.VerticalAlignment = VerticalAlignment.Top;

                g.Background = new SolidColorBrush(Colors.SteelBlue);
                g.Width = Bounds.Width;
                g.Height = Bounds.Height;
                Rectangle rt = new Rectangle();
                rt.HorizontalAlignment = HorizontalAlignment.Left;
                rt.VerticalAlignment = VerticalAlignment.Top;
                rt.Fill = _mSharedData.Graph.PageSettings.PageBackground;
                rt.Width = Bounds.Width * hcount; //_mSharedData.ScrollViewer._pageBackground.Width / _mSharedData.ScrollViewer.CurrentZoom;
                rt.Height = Bounds.Height * vcount; // _mSharedData.ScrollViewer._pageBackground.Height / _mSharedData.ScrollViewer.CurrentZoom;
                Grid back = new Grid();
                back.Background = new SolidColorBrush(Colors.White);
                back.Height = Bounds.Height * vcount;
                back.Width = Bounds.Width * hcount;
                Image i = new Image();
                i.HorizontalAlignment = HorizontalAlignment.Left;
                i.VerticalAlignment = VerticalAlignment.Top;
                ImageBrush ib = new ImageBrush();
                ib.ImageSource = bitmap;
                i.Stretch = Currentstretch;
                i.Source = bitmap;
                i.Width = _mSharedData.SpatialSearch._pageRight - _mSharedData.SpatialSearch._pageLeft;
                i.Height = _mSharedData.SpatialSearch._pageBottom - _mSharedData.SpatialSearch._pageTop;
                double left = 0;
                double top = 0;
                left = _mSharedData.SpatialSearch._pageLeft - _mSharedData.PageSettingsController._left;
                top = _mSharedData.SpatialSearch._pageTop - _mSharedData.PageSettingsController._top;
                back.Children.Add(rt);
                back.Children.Add(i);
                g.Children.Add(back);
                PrintContainer.Children.Clear();
                PrintContainer.Children.Add(g);
                PrintContainer.InvalidateMeasure();
                PrintContainer.UpdateLayout();
                TranslateTransform tt = new TranslateTransform();
                tt.X = left - Bounds.X;
                tt.Y = top - Bounds.Y;
                i.RenderTransform = tt;
                TranslateTransform t = new TranslateTransform();
                t.X = -Bounds.X;
                t.Y = -Bounds.Y;
                rt.RenderTransform = t;
                return g;
            }
            else
            {
                int leftcount = 0;
                int topcount = 0;
                _mSharedData.PageSettingsController.UpdateRowandColoumn(out topcount, out leftcount, Bounds.Width, Bounds.Height);
                RenderTargetBitmap bitmap = new RenderTargetBitmap();
                await bitmap.RenderAsync(_mSharedData.Graph.Page);
                Rectangle rt = new Rectangle();
                rt.HorizontalAlignment = HorizontalAlignment.Left;
                rt.VerticalAlignment = VerticalAlignment.Top;
                rt.Fill = _mSharedData.Graph.PageSettings.PageBackground;
                rt.Width = Bounds.Width;
                rt.Height = Bounds.Height;
                Grid back = new Grid();
                back.Background = new SolidColorBrush(Colors.White);
                back.Height = Bounds.Height;
                back.Width = Bounds.Width;
                Image i = new Image();
                i.Source = bitmap;
                i.HorizontalAlignment = HorizontalAlignment.Left;
                i.VerticalAlignment = VerticalAlignment.Top;
                i.Stretch = Currentstretch;
                if (Currentstretch != Stretch.UniformToFill)
                {
                    if (IsPrint || (_mSharedData.Graph.ExportSettings != null && _mSharedData.Graph.ExportSettings.ExportMode == ExportMode.PageSettings))
                    {
                        i.Width = (_mSharedData.SpatialSearch._pageRight - _mSharedData.SpatialSearch._pageLeft) / leftcount;
                        i.Height = (_mSharedData.SpatialSearch._pageBottom - _mSharedData.SpatialSearch._pageTop) / topcount;
                    }
                    else
                    {
                        i.Width = Bounds.Width;
                        i.Height = Bounds.Height;
                    }
                }
                double left = 0;
                double top = 0;
                if (IsPrint || (_mSharedData.Graph.ExportSettings != null && _mSharedData.Graph.ExportSettings.ExportMode == ExportMode.PageSettings))
                {
                    left = _mSharedData.SpatialSearch._pageLeft - _mSharedData.PageSettingsController._left;
                    top = _mSharedData.SpatialSearch._pageTop - _mSharedData.PageSettingsController._top;
                }
                TranslateTransform t = new TranslateTransform();
                if (Currentstretch != Stretch.UniformToFill)
                {
                    if (IsPrint || (_mSharedData.Graph.ExportSettings != null && _mSharedData.Graph.ExportSettings.ExportMode == ExportMode.PageSettings))
                    {
                        t.X = left / leftcount;
                        t.Y = top / topcount;
                    }
                    else
                    {
                        if (_mSharedData.Graph.ExportSettings != null && _mSharedData.Graph.ExportSettings.ExportMode == ExportMode.Content)
                        {
                            t.X = left;
                            t.Y = top;
                        }
                    }

                }
                else
                {
                    t.X = left;
                    t.Y = top;
                }
                i.RenderTransform = t;
                i.UpdateLayout();
                back.Children.Add(rt);
                back.Children.Add(i);
                PrintContainer.Children.Clear();
                PrintContainer.Children.Add(back);
                PrintContainer.InvalidateMeasure();
                PrintContainer.UpdateLayout();
                return back;
            }

        }
    }
}

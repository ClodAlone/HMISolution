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
using System.ComponentModel;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// VisualImage
    /// </summary>
    public class VisualImage : FrameworkElement
    {
        #region Visual DependencyProperty

        private Point m_PreviousStart = new Point(0, 0);

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty VisualProperty = DependencyProperty.Register(
            "Visual",
            typeof(FrameworkElement),
            typeof(VisualImage),
            new PropertyMetadata(OnVisualChanged));

        /// <summary>
        /// 
        /// </summary>
        public FrameworkElement Visual
        {
            get { return (FrameworkElement)GetValue(VisualProperty); }
            set { SetValue(VisualProperty, value); }
        }

        private static void OnVisualChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var visualImage = obj as VisualImage;
            visualImage.OnVisualChanged(args);
        }

        private void OnVisualChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue != null) ((FrameworkElement)args.OldValue).SizeChanged -= VisualImage_SizeChanged;
            if (args.NewValue != null)
            {
                var visual = (FrameworkElement)args.NewValue;
                visual.SizeChanged += VisualImage_SizeChanged;
                visual.LayoutUpdated += new EventHandler(ScrollSource_LayoutUpdated);
                PrepareBitmap();
            }
        }

        private bool _SizeChanged = false;

        private void VisualImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!ImageThread.IsBusy)
            {
                _SizeChanged = true;
                ImageThread.RunWorkerAsync();
            }
            else
            {
                ImageThread.CancelAsync();
                _SizeChanged = true;
                if (RunWorkerCompleted_Handler == null)
                {
                    RunWorkerCompleted_Handler = (s, evt) =>
                    {
                        ImageThread.RunWorkerCompleted -= RunWorkerCompleted_Handler;
                        if (!ImageThread.IsBusy)
                        {
                            ImageThread.RunWorkerAsync();
                        }
                        else
                        {
                        }
                    };
                }
                ImageThread.RunWorkerCompleted -= RunWorkerCompleted_Handler;
                ImageThread.RunWorkerCompleted += RunWorkerCompleted_Handler;
            }
        }

        RunWorkerCompletedEventHandler RunWorkerCompleted_Handler;

        //private double GetScale()
        //{
        //    double delta_ScaleX = Visual.ActualWidth / Math.Max(1, this.RenderSize.Width);
        //    double delta_ScaleY = Visual.ActualHeight / Math.Max(1, this.RenderSize.Height);
        //    double s = Math.Min(delta_ScaleX, delta_ScaleY);
        //    return s;
        //}

        #endregion // Visual DependencyProperty

        #region Bitmap DependencyProperty

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty BitmapProperty = DependencyProperty.Register(
            "Bitmap",
            typeof(WriteableBitmap),
            typeof(VisualImage),
            null);

        /// <summary>
        /// 
        /// </summary>
        public WriteableBitmap Bitmap
        {
            get { return (WriteableBitmap)GetValue(BitmapProperty); }
            set { SetValue(BitmapProperty, value); }
        }

        #endregion // Bitmap DependencyProperty

        private BackgroundWorker ImageThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualImage"/> class.
        /// </summary>
        public VisualImage()
        {
            ImageThread = new BackgroundWorker();
            ImageThread.WorkerSupportsCancellation = true;
            ImageThread.DoWork += new DoWorkEventHandler(ImageThread_DoWork);
            SizeChanged += new SizeChangedEventHandler(VisualImage_SizeChanged);
        }

        void ImageThread_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_SizeChanged)
            {
                _SizeChanged = false;
                System.Threading.Thread.Sleep(500);
                PrepareBitmap();
            }
            else
            {
                for (int i = 0; i < 50; i++)
                {
                    if (!ImageThread.CancellationPending)
                    {
                        System.Threading.Thread.Sleep(50);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            Invalidate();
        }

        void ScrollSource_LayoutUpdated(object sender, EventArgs e)
        {
            if (!ImageThread.IsBusy)
            {
                ImageThread.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Prepares the bitmap.
        /// </summary>
        private void PrepareBitmap()
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                if (Visual == null)
                {
                    return;
                }
                Point pt = OverviewContentHolder.GetStart(Visual);
                //double scale = GetScale();
                double delta_ScaleX = (Visual.ActualWidth + pt.X) / Math.Max(1, this.RenderSize.Width);
                double delta_ScaleY = (Visual.ActualHeight + pt.Y) / Math.Max(1, this.RenderSize.Height);
                double scale = Math.Min(delta_ScaleX, delta_ScaleY);
                if (scale == 0)
                {
                    scale = 1;
                }
                Size _NewSize = new Size((Visual.ActualWidth + pt.X) / scale, (Visual.ActualHeight + pt.Y) / scale);
                //Bitmap = new WriteableBitmap((int)(width), (int)(height));
                Bitmap = new WriteableBitmap((int)(_NewSize.Width), (int)(_NewSize.Height));

                if (VisualImage_LayoutUpdated == null)
                {
                    VisualImage_LayoutUpdated = (s, e) =>
                        {
                            this.LayoutUpdated -= VisualImage_LayoutUpdated;
                            this.Visual.LayoutUpdated -= VisualImage_LayoutUpdated;
                            Invalidate();
                        };
                }

                this.LayoutUpdated -= VisualImage_LayoutUpdated;
                this.Visual.LayoutUpdated -= VisualImage_LayoutUpdated;
                this.LayoutUpdated += VisualImage_LayoutUpdated;
                this.Visual.LayoutUpdated += VisualImage_LayoutUpdated;
            });
        }

        private EventHandler VisualImage_LayoutUpdated;

        /// <summary>
        /// Invalidates the VisualImage and causes WriteableBitmap to be refreshed.
        /// </summary>
        public void Invalidate()
        {
            this.Dispatcher.BeginInvoke(delegate()
            {
                if (Bitmap != null && Visual != null)
                {
                    Point pt = OverviewContentHolder.GetStart(Visual);
                    if (!pt.Equals(m_PreviousStart))
                    {
                        PrepareBitmap();
                    }
                    m_PreviousStart = pt;
                    Array.Clear(Bitmap.Pixels, 0, Bitmap.Pixels.Length);
                    if (Visual.ActualHeight > 0 || Visual.ActualWidth > 0)
                    {
                        Bitmap.Render(Visual, new CompositeTransform()
                        {
                            TranslateX = pt.X * Bitmap.PixelWidth / (Visual.ActualWidth + pt.X),// * OverviewPanel.GetInverseScale(Visual),
                            TranslateY = pt.Y * Bitmap.PixelHeight / (Visual.ActualHeight + pt.Y),// * OverviewPanel.GetInverseScale(Visual),
                            ScaleX = Bitmap.PixelWidth / (Visual.ActualWidth + pt.X) * OverviewContentHolder.GetInverseScale(Visual),
                            ScaleY = Bitmap.PixelHeight / (Visual.ActualHeight + pt.Y) * OverviewContentHolder.GetInverseScale(Visual)
                        });
                    }
                    //Bitmap.Render(Visual, new ScaleTransform() { ScaleX = 1 / GetScale(), ScaleY = 1 / GetScale() });//*/, Visual.RenderTransform);
                    Bitmap.Invalidate();
                }
            });
        }
    }
}

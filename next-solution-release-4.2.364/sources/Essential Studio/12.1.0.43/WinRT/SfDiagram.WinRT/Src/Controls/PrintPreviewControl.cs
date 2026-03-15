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
#if WINRT_USING
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
#else
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
#endif
namespace Syncfusion.UI.Xaml.Diagram.Controls
{
    public class PrintPreviewControl : Control
    {
        public PrintPreviewControl()
        {
            this.DefaultStyleKey = typeof(PrintPreviewControl);
            this.Loaded += PreviewControl_Loaded;
        }
        void PreviewControl_Loaded(object sender, RoutedEventArgs e)
        {
            PrintableArea = GetTemplateChild("PrintableArea") as Grid;
        }

        public double GridWidth
        {
            get { return (double)GetValue(GridWidthProperty); }
            set { SetValue(GridWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridWidthProperty =
            DependencyProperty.Register("GridWidth", typeof(double), typeof(PrintPreviewControl), new PropertyMetadata(0));

        public double GridHeight
        {
            get { return (double)GetValue(GridHeightProperty); }
            set { SetValue(GridHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridHeightProperty =
            DependencyProperty.Register("GridHeight", typeof(double), typeof(PrintPreviewControl), new PropertyMetadata(0));

        internal Grid PrintableArea
        {
            get { return (Grid)GetValue(PrintableAreaProperty); }
            set { SetValue(PrintableAreaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PrintableArea.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty PrintableAreaProperty =
            DependencyProperty.Register("PrintableArea", typeof(Grid), typeof(PrintPreviewControl), new PropertyMetadata(null, OnAreaChanged));

        private static void OnAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PrintPreviewControl p = d as PrintPreviewControl;
        }



        public int PageCount
        {
            get { return (int)GetValue(PageCountProperty); }
            set { SetValue(PageCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageCountProperty =
            DependencyProperty.Register("PageCount", typeof(int), typeof(PrintPreviewControl), new PropertyMetadata(0));




        public int CurrentPageNo
        {
            get { return (int)GetValue(CurrentPageNoProperty); }
            set { SetValue(CurrentPageNoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentPageNo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPageNoProperty =
            DependencyProperty.Register("CurrentPageNo", typeof(int), typeof(PrintPreviewControl), new PropertyMetadata(0));


        public ImageSource PrintSource
        {
            get { return (ImageSource)GetValue(PrintSourceProperty); }
            set { SetValue(PrintSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PrintSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrintSourceProperty =
            DependencyProperty.Register("PrintSource", typeof(ImageSource), typeof(PrintPreviewControl), new PropertyMetadata(null));
        
        internal void AddChildren(Image i)
        {
            if (PrintableArea != null)
            {
                PrintableArea.Children.Add(i);
            }
        }
    }
}

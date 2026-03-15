#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;
#endif



namespace Syncfusion.UI.Xaml.Maps
{
    public class LegendPanel : Panel
    {
        Size MyDesiredSize;

        #region Dependency Property

        public int Split
        {
            get { return (int)GetValue(SplitProperty); }
            set { SetValue(SplitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Split.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SplitProperty =
            DependencyProperty.Register("Split", typeof(int), typeof(LegendPanel), new PropertyMetadata(1));

        #region LegendWidth

        internal Thickness LegendWidth
        {
            get { return (Thickness)GetValue(LegendWidthProperty); }
            set { SetValue(LegendWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendWidthProperty =
            DependencyProperty.Register("LegendWidth", typeof(Thickness), typeof(LegendPanel), new PropertyMetadata(new Thickness()));

        #endregion

        #endregion

        #region Overide methods
#if WINRT
        protected override Size ArrangeOverride(Size finalSize)
        {
#else
        protected override Size ArrangeOverride(Size finalSize)
        {
#endif
            double PositionX = 0d;
            double PositionY = 0d;
            double width = 0d;
            double height = 0d;
            if (Split < 1)
                Split = 1;
            if (Split > Children.Count)
                Split = Children.Count;
            int i = 0;
            int j = 0;
            foreach (UIElement item in Children)
            {
                width = Math.Max(width, item.DesiredSize.Width);
                height = Math.Max(height, item.DesiredSize.Height);
            }
            var s2 = SfMap.FindParent<ShapeFileLayer>(this);

            foreach (UIElement item in Children)
            {
                if (j == Children.Count - 1)
                {

                    double rowWidth = PositionX;

                    if (i >= Split)
                    {
                        PositionY += height;
                        PositionX = 0;

                    }
                    if (rowWidth > PositionX)
                        rowWidth -= PositionX;
                    item.Arrange(new Rect(PositionX, PositionY, rowWidth, height));
                }
                else
                {
                    if (i < Split)
                    {
                        item.Arrange(new Rect(PositionX, PositionY, width, height));
                        PositionX += width;
                        i++;
                    }
                    else
                    {
                        i = 0;
                        PositionY += height;
                        PositionX = 0;
                        item.Arrange(new Rect(PositionX, PositionY, width, height));
                        i++;
                        PositionX += width;
                    }
                }

                j++;
                if (Split % 2 == 0)
                {
                    int temp1 = Split / 2;
                    double temp2 = (width / 2);
                    LegendWidth = new Thickness { Left = ((temp1 * width) - temp2) };
                }
                else
                {
                    int temp1 = Split / 2;
                    LegendWidth = new Thickness { Left = (temp1 * width) };
                }
                if (s2 != null)
                {
                    SetBinding(LegendWidthProperty, new Binding { Source = s2, Path = new PropertyPath("LegendWidth"), Mode = BindingMode.TwoWay });
                }

            }
            return finalSize;
        }


#if WINRT
        protected override Size MeasureOverride(Size availableSize)
        {
#else
        protected override Size MeasureOverride(Size availableSize)
        {
#endif


            var sl = SfMap.FindParent<ShapeFileLayer>(this);
            if (sl != null)
            {
                SetBinding(SplitProperty, new Binding { Source = sl, Path = new PropertyPath("LegendColumnSplit") });
            }
            if (Split < 1)
                Split = 1;
            if (Split > Children.Count)
                Split = Children.Count;
            MyDesiredSize.Height = 0d;
            MyDesiredSize.Width = 0d;
            double height = 0;
            double width = 0;
            foreach (UIElement item in Children)
            {
                item.Measure(availableSize);
                item.UpdateLayout();
                width = Math.Max(width, item.DesiredSize.Width);
                height = Math.Max(height, item.DesiredSize.Height);
                double PositionX = width * Split;
                int temp = Children.Count / Split;
                double PositionY = temp * height;
                if (Children.Count % Split != 0)
                {
                    PositionY += height;
                }
                MyDesiredSize.Width = PositionX;
                MyDesiredSize.Height = PositionY;
                item.Measure(new Size(width, height));
            }
            return MyDesiredSize;
        }

        #endregion

    }
}


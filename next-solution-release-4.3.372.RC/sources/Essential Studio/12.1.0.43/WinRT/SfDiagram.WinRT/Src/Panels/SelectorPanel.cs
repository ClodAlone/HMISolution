#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes; 
#else
using System.Windows.Controls;
using System.Windows.Shapes; 
#endif
using System.Windows;
using Syncfusion.UI.Xaml.Diagram.Utility;

namespace Syncfusion.UI.Xaml.Diagram.Panels
{
#if !WINRT
    [DesignTimeVisible(false)] 
#endif
    public sealed class SelectorPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in Children)
            {
                if (child is Line)
                {
                    child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }
                else
                {
                    child.Measure(availableSize);
                }
            }
            return availableSize.Valid();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Line line = null;
            foreach (FrameworkElement child in Children)
            {
                double x = 0, y = 0;
                if (child is Line)
                {
                    line = child as Line;
                }
                if (child.Name == "PART_Pivot" )
                {
                    x = line.X2 = Canvas.GetLeft(child)*finalSize.Width;
                    //line.X1 -= line.StrokeThickness / 2;
                    x -= child.DesiredSize.Width/2;
                    y = Canvas.GetTop(child) * finalSize.Height;
                    if (child.Visibility == Visibility.Visible)
                        line.Y2 = y;
                    else line.Y2 = 0;
                    //line.Y1 -= line.StrokeThickness / 2;
                    y -= child.DesiredSize.Height/2;
                }
                else if (child.Name == "PART_Rotator")
                {
                    x = line.X1 = Canvas.GetLeft(child)*finalSize.Width;
                    //line.X2 -= line.StrokeThickness / 2;
                    x -= child.DesiredSize.Width/2;
                    y = line.Y1 = Math.Min(0, line.Y2) - 50;
                }

                switch (child.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        x = -child.DesiredSize.Width/2;
                        break;
                    case HorizontalAlignment.Center:
                        x = finalSize.Width/2 - child.DesiredSize.Width/2;
                        break;
                    case HorizontalAlignment.Right:
                        x = finalSize.Width - child.DesiredSize.Width/2;
                        break;
                }
                switch (child.VerticalAlignment)
                {
                    case VerticalAlignment.Top:
                        y = -child.DesiredSize.Height/2;
                        break;
                    case VerticalAlignment.Center:
                        y = finalSize.Height/2 - child.DesiredSize.Height/2;
                        break;
                    case VerticalAlignment.Bottom:
                        y = finalSize.Height - child.DesiredSize.Height/2;
                        break;
                }
                if (child is Rectangle)
                {
                    child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                }
                else
                {
                    Size avail = child.DesiredSize;
                    if (child.Name != "PART_Rotator" && child.Name != "PART_Pivot")
                    {
                        if (child.HorizontalAlignment == HorizontalAlignment.Stretch)
                        {
                            avail.Width = finalSize.Width;
                        }
                        if (child.VerticalAlignment == VerticalAlignment.Stretch)
                        {
                            avail.Height = finalSize.Height;
                        }
                    }
                    child.Arrange(new Rect(x, y, avail.Width, avail.Height));
                }
            }
            line.Arrange(new Rect(0, 0, line.DesiredSize.Width, line.DesiredSize.Height));
            return finalSize;
        }
    }
}

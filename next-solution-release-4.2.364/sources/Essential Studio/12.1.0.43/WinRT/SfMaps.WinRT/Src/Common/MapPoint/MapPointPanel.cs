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

#if WINRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
#else
using System.Windows.Controls;
using System.Windows;
#endif

namespace Syncfusion.UI.Xaml.Maps
{
    public class MapPointPanel : Panel
    {
        public MapPointPanel()
        {
        }

#if WINRT
         protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
#else
        protected override Size ArrangeOverride(Size finalSize)
        {
#endif

            foreach (UIElement element in this.Children)
            {
                var mappoint = element as MapPoint;
                if (mappoint != null)
                {
                    element.Arrange(new Rect(new Point(mappoint.PointMargin.Left - (element.DesiredSize.Width), mappoint.PointMargin.Top - (element.DesiredSize.Height)), element.DesiredSize));
                }
            }
            return base.ArrangeOverride(finalSize);
        }


#if WINRT
        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
#else
        protected override Size MeasureOverride(Size availableSize)
        {
#endif
            foreach (UIElement element in this.Children)
            {
                element.Measure(availableSize);
            }

            return base.MeasureOverride(availableSize);
        }
    }
}

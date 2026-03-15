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
using System.Windows;
using System.Windows.Controls;
#endif


namespace Syncfusion.UI.Xaml.Maps
{

    /// <summary>
    /// Represents the MapItemsPanel class in the map.
    /// </summary>
    /// <remarks>
    /// MapItemsPanel class measures and arranges the MapItems in the map.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class MapItemsPanel : Panel
    {
        SfMap map;

#if WINRT

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {

#else
        protected override Size ArrangeOverride(Size finalSize)
        {
#endif
            ShapeFileLayer shapeFileLayer = SfMap.FindParent<ShapeFileLayer>(this);
            List<Rect> RectList = new List<Rect>();
            bool? isIntersected = null;
            if (shapeFileLayer != null && shapeFileLayer.HideIntersectLabels)
            {
                isIntersected = false;
            }
            foreach (UIElement child in Children)
            {
                Size size = child.DesiredSize;
                ContentPresenter content = child as ContentPresenter;

                var mapItem = content.DataContext;
                if (mapItem is MapItem)
                {
                    Rect mapItemRect = new Rect() { X = (mapItem as MapItem).Margin.Left - (size.Width / 2), Y = (mapItem as MapItem).Margin.Top - (size.Height / 2), Height = size.Height, Width = size.Width };
                    if (isIntersected.HasValue)
                    {
                        isIntersected = false;
                        foreach (Rect rect in RectList)
                        {
                            Rect itemRect = mapItemRect;
                            itemRect.Intersect(rect);
                            if (!itemRect.IsEmpty)
                            {
                                isIntersected = true;
                                break;
                            }
                        }
                        RectList.Add(mapItemRect);
                    }
                    if (isIntersected == null || !(bool)isIntersected)
                        child.Arrange(mapItemRect);
#if !WPF && !WINRT
                    else
                        child.Arrange(new Rect(mapItemRect.X, mapItemRect.Y, 0, 0));
#endif
                }
                else if (mapItem is Bubble)
                {
                    child.Arrange(new Rect() { X = (mapItem as Bubble).Margin.Left - (size.Width / 2), Y = (mapItem as Bubble).Margin.Top - (size.Height / 2), Height = size.Height, Width = size.Width });
                }                
            }
            if (double.IsInfinity(finalSize.Height) || double.IsInfinity(finalSize.Width))
            {
                if (this.map == null)
                {
                    this.map = SfMap.FindParent<SfMap>(this);
                }
                return map.DesiredSize;
            }
            RectList.Clear();
            RectList = null;
            shapeFileLayer = null;
            return (finalSize);
        }

#if WINRT
       protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
#else
        protected override Size MeasureOverride(Size availableSize)
        {
#endif
            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);
            }
           if(double.IsInfinity(availableSize.Height) || double.IsInfinity(availableSize.Width))
            {
                if (this.map == null)
                {
                    this.map = SfMap.FindParent<SfMap>(this);
                }
                return map.DesiredSize;
            }
            return availableSize;
        }
    }

}

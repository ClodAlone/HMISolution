#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Controls;
    using System.Windows;


    internal class SelectionPanel : Panel
    {
        #region Private Fields

        private MapControl mapControl;

        #endregion
        public SelectionPanel()
        {
        }
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            this.mapControl = MapControl.FindParent<MapControl>(this) as MapControl;
            if (this.mapControl != null)
            {
                if (mapControl.SymbolPaletteVisibility == Visibility.Visible)
                {
                    foreach (UIElement element in this.Children)
                    {
                        var y = Canvas.GetTop(element);
                        var x = Canvas.GetLeft(element);
                        element.Arrange(new Rect(new Point(x - this.mapControl.SymbolPalette.ActualWidth - this.mapControl.Margin.Left - (this.mapControl.LayeredContent as ShapeFileLayer).Margin.Left, y - this.mapControl.Margin.Top - (this.mapControl.LayeredContent as ShapeFileLayer).Margin.Top), element.DesiredSize));
                    }
                }
                else
                {
                    foreach (UIElement element in this.Children)
                    {
                        var y = Canvas.GetTop(element);
                        var x = Canvas.GetLeft(element);
                        element.Arrange(new Rect(new Point(x + this.mapControl.Margin.Left - (this.mapControl.LayeredContent as ShapeFileLayer).Margin.Left, y - this.mapControl.Margin.Top - (this.mapControl.LayeredContent as ShapeFileLayer).Margin.Top), element.DesiredSize));
                    }
                }
            }
            return finalSize;
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            Size avialSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement element in this.Children)
            {
                element.Measure(avialSize);
            }

            return new Size();
        }
    }
}

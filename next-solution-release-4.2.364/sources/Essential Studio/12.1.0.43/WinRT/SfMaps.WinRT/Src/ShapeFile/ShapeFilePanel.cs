#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.UI.Xaml.Maps
{
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



    /// <summary>
    /// Represents the ShapeFilePanel class in the SfMap. Inherites from the Panel class.
    /// </summary>
    /// <remarks>
    /// Measures and arranges the map shapes in the shape file layer.
    /// </remarks>
    //[ClassReference(IsReviewed = false)]
    public class ShapeFilePanel : Panel
    {
        #region PrivateFields

        private double minWidth = 500;
        private double minHeight = 500;

        #endregion
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.ShapeFilePanel">ShapeFilePanel</see> class. 
        /// </summary>
        //[ClassReference(IsReviewed = false)]
        public ShapeFilePanel()
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
                element.Arrange(new Rect(0, 0, element.DesiredSize.Width, element.DesiredSize.Height));
            }
            return finalSize;
        }

#if WINRT
        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
#else
        protected override Size MeasureOverride(Size availableSize)
        {
#endif
            Size desiredSize = new Size(0, 0);
            Size totalSizeAllocated = new Size(Double.IsPositiveInfinity(availableSize.Width) ? minWidth : availableSize.Width, Double.IsPositiveInfinity(availableSize.Height) ? minHeight : availableSize.Height);
            foreach (UIElement element in this.Children)
            {
                element.Measure(totalSizeAllocated);
            }
            return totalSizeAllocated;
        }
    }
}

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
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    ///  ImageryPanel is a panel that arranges the elements of BingMap
    /// </summary>
    public class ImageryPanel : Panel
    {
        #region Private Fields

        private ImageryLayer imageryLayer;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.ImageryPanel">ImageryPanel</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public ImageryPanel()
        {

        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for
        /// child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class. 
        /// </summary>
        /// <param name="constraint">The available size that this element can give to
        /// child elements. Infinity can be specified as a value to indicate that the
        /// element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its
        /// calculations of child element sizes.
        /// </returns>
        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            Size availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            Size panalSize = new Size();
            if (this.Children.Count > 0)
            {
                this.imageryLayer = MapControl.FindParent<ImageryLayer>(this.Children[0]);
            }
            if (this.imageryLayer != null)
            {
                if (this.imageryLayer.enableInvalidate)
                {
                    foreach (UIElement element in this.imageryLayer.Tiles)
                    {
                        if (element != null)
                        {
                            element.Measure(availableSize);
                        }
                    }
                    panalSize.Width = 256 * this.imageryLayer.xcount;
                    panalSize.Height = 256 * this.imageryLayer.ycount;
                }
            }

            return panalSize;
        }
        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a
        /// size for a <see cref="T:System.Windows.FrameworkElement"/> derived class. 
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element
        /// should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            double x = 0;
            double y = 0;
            foreach (UIElement element in this.Children)
            {
                if (element == null)
                {
                    continue;
                }
                x = 256 * (element as Tile).X;
                y = 256 * (element as Tile).Y;
                if (this.imageryLayer != null)
                {
                    x = 256 * (element as Tile).X + this.imageryLayer.bingmapPanTransform.X;
                    y = 256 * (element as Tile).Y + this.imageryLayer.bingmapPanTransform.Y;
                }
                element.Arrange(new Rect(new Point(x, y), element.DesiredSize));
            }
            return finalSize;
        }
    }
}

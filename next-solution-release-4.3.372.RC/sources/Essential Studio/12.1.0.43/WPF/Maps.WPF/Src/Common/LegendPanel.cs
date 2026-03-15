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
    /// <summary>
    ///  LegendPanel is a Panel where the Legends are arranged
    /// </summary>
    public class LegendPanel : Panel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.LegendPanel">LegendPanel</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public LegendPanel()
        {
        }
        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for
        /// child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class. 
        /// </summary>
        /// <param name="availSize">The available size that this element can give to
        /// child elements. Infinity can be specified as a value to indicate that the
        /// element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its
        /// calculations of child element sizes.
        /// </returns>
        protected override System.Windows.Size MeasureOverride(System.Windows.Size availSize)
        {

            Size availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

            foreach (UIElement element in this.Children)
            {
                if (element != null)
                {
                    element.Measure(availableSize);
                }
            }

            return new Size();
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
            double x = 0.0;
            double y = 0.0;         

            foreach (UIElement element in this.Children)
            {
                if (element == null)
                {
                    continue;
                }

                y = y + 20;
                element.Arrange(new Rect(new Point(x, y), element.DesiredSize));
            }

            return finalSize;
        }
    }
}

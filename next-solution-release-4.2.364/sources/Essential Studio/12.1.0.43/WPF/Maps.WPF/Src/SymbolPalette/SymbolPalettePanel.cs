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
    ///  SymbolPalettePanel is Panel which arranges the SymbolPalette in the map
    /// </summary>
    public class SymbolPalettePanel : Panel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.SymbolPalettePanel"/> class.
        /// </summary>
        public SymbolPalettePanel()
        {

        }

        #endregion

        #region Override Methods

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for
        /// child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class. 
        /// </summary>
        /// <param name="constrain">The available size that this element can give to
        /// child elements. Infinity can be specified as a value to indicate that the
        /// element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its
        /// calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size constrain)
        {
            int count = 0;
            Size avialabelSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            double requiredHeight = 0;
            foreach (UIElement element in this.Children)
            {
                if (element != null)
                {
                    element.Measure(avialabelSize);
                }

                if ((count % 3) == 0)
                {
                    requiredHeight += 50;
                }
                count++;
            }
            return new Size(150, requiredHeight);
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
        protected override Size ArrangeOverride(Size finalSize)
        {
            double x = 0;
            double y = 0;
            foreach (UIElement element in this.Children)
            {
                if (element != null)
                {
                    if (x >= 150)
                    {
                        x = 0;
                        y = y + 50;
                    }
                    element.Arrange(new Rect(new Point(x, y), element.DesiredSize));
                    x += 50;
                }
            }
            return finalSize;
        }
        #endregion
    }
}

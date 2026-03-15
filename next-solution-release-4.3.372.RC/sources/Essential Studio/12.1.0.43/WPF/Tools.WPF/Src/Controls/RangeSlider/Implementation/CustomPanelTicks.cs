#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    /// <summary>
    /// Class contains Custom Panel ticks
    /// </summary>
    [Browsable(false)]
    public class CustomPanelTicks : Panel
    {
        #region Internal variables

        /// <summary>
        /// Gets or sets the label orientation.
        /// </summary>
        /// <value>The label orientation.</value>
        internal Itemorientation LabelOrientation { get; set; }

        /// <summary>
        /// Gets or sets the X offset.
        /// </summary>
        /// <value>The X offset.</value>
        internal double XOffset { get; set; }

        /// <summary>
        /// Gets or sets the Y offset down.
        /// </summary>
        /// <value>The Y offset down.</value>
        internal double YOffsetDown { get; set; }

        /// <summary>
        /// Gets or sets the width of the panel.
        /// </summary>
        /// <value>The width of the panel.</value>
        internal double PanelWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the panel.
        /// </summary>
        /// <value>The height of the panel.</value>
        internal double PanelHeight { get; set; }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>The maximum.</value>
        internal double maximum { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [up orientation].
        /// </summary>
        /// <value><c>true</c> if [up orientation]; otherwise, <c>false</c>.</value>
        internal bool UpOrientation { get; set; }

        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>The minimum.</value>
        internal double minimum { get; set; }

        /// <summary>
        /// Member Variable for the offset
        /// </summary>
        internal double offset = 0d;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is item.
        /// </summary>
        /// <value><c>true</c> if this instance is item; otherwise, <c>false</c>.</value>
        internal bool IsItem { get; set; }

        /// <summary>
        /// Gets or sets the custom items.
        /// </summary>
        /// <value>The custom items.</value>
        internal ObservableCollection<Items> CustomItems { get; set; }

        #endregion Internal variables

        #region Protected Override Methods

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size infinite = new Size(double.PositiveInfinity,
                             double.PositiveInfinity);
            foreach (FrameworkElement child in Children)
            {
                child.Measure(infinite);
            }

            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double currentX = 0, currentY = 0;
            int count = 0, count1 = 0;
            foreach (FrameworkElement child in Children)
            {
                count++;
                Point location;

                if (XOffset != 0)
                {
                    if (IsItem && CustomItems != null && count <= CustomItems.Count && CustomItems.Count > 0)
                    {
                        double value = CustomItems[count - 1].value;
                        if ((this.XOffset * calculate(value) - 1) != -1)
                        {
                            currentX = this.XOffset * calculate(value) - 1;
                        }

                        location = new Point(currentX, currentY);
                        child.Arrange(new Rect(location, child.DesiredSize));
                        count1++;
                    }
                    else
                    {
                        location = new Point(currentX, currentY);
                        child.Arrange(new Rect(location, child.DesiredSize));
                        if (count == Children.Count)
                        {
                            if (currentX - XOffset != this.PanelWidth)
                            {
                                location.X = this.PanelWidth;
                                child.Arrange(new Rect(location, child.DesiredSize));
                            }
                        }
                        currentX += (this.XOffset);
                        child.HorizontalAlignment = HorizontalAlignment.Left;
                    }
                }
                else if (YOffsetDown != 0)
                {
                    if (IsItem && CustomItems.Count > 0)
                    {
                        double value = CustomItems[count - 1].value;
                        currentY = this.YOffsetDown * calculate(value) - 1;
                        location = new Point(currentX, currentY);
                        child.Arrange(new Rect(location, child.DesiredSize));
                    }
                    else
                    {
                        location = new Point(currentX, currentY);
                        child.Arrange(new Rect(location, child.DesiredSize));
                        if (count == Children.Count)
                        {
                            location = new Point(currentX, currentY);
                            if (currentY - XOffset != this.PanelHeight)
                            {
                                location.Y = this.PanelHeight;
                                child.Arrange(new Rect(location, child.DesiredSize));
                            }
                        }
                        currentY += (this.YOffsetDown);
                        child.VerticalAlignment = VerticalAlignment.Top;
                    }
                }

            }

            if (currentY < 0)
            {
                currentY = 0;
            }
            if (currentX < 0)
            {
                currentX = 0;
            }

            return new Size(currentX, currentY);
        }

        #endregion Protected Override Methods

        #region Private Methods

        /// <summary>
        /// Calculates the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private double calculate(double value)
        {
            if (value > maximum)
            {
                value = maximum;
            }
            if (value < minimum)
            {
                return 0;
            }
            else
            {
                return value - minimum;
            }
        }

        #endregion Private Methods
    }
}
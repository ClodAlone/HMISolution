#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
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
    /// Class which has custom panel label
    /// </summary>
    [Browsable(false)]
    public class CustomPanelLabel : Panel
    {
        #region Internal Variables

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
        /// Gets or sets a value indicating whether this instance is item.
        /// </summary>
        /// <value><c>true</c> if this instance is item; otherwise, <c>false</c>.</value>
        internal bool IsItem { get; set; }

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
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>The maximum.</value>
        internal double maximum { get; set; }

        /// <summary>
        /// Member Variable for the offset
        /// </summary>
        internal double offset = 0d;

        /// <summary>
        /// Member Variable for the actual height
        /// </summary>
        //SU I78477
        //internal double actualheight;
        internal double actualheight = 0d;

        //EU I78477

        /// <summary>
        /// Gets or sets the custom items.
        /// </summary>
        /// <value>The custom items.</value>
        internal ObservableCollection<Items> CustomItems { get; set; }

        /// <summary>
        /// Dependency property for panel height
        /// </summary>
        internal static readonly DependencyProperty PanelheightProperty = DependencyProperty.Register("Panelheight", typeof(double), typeof(CustomPanelLabel), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the panelheight.
        /// </summary>
        /// <value>The panelheight.</value>
        internal double Panelheight
        {
            get
            {
                return (double)GetValue(PanelheightProperty);
            }
            set
            {
                SetValue(PanelheightProperty, value);
            }
        }

        #endregion Internal Variables

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
            int count = 0;
            string prevstring = string.Empty;
            Point location;
            bool entered = false;

            if (Panelheight != 0d && !double.IsNaN(Panelheight))
            {
                this.Height = Panelheight;
            }

            foreach (FrameworkElement child in Children)
            {
                count++;

                if (XOffset != 0)
                {
                    if (IsItem && CustomItems != null && count <= CustomItems.Count)
                    {
                        double value = CustomItems[count - 1].value;
                        currentX = this.XOffset * calculate(value);
                        location = new Point(currentX, currentY);
                        if (entered)
                        {
                            location.Y = 7;
                        }
                        if (count < CustomItems.Count && ((currentX + child.DesiredSize.Width) > this.XOffset * calculate(CustomItems[count].value)))
                        {
                            if ((this.XOffset * calculate(CustomItems[count].value) - (currentX)) <= 1)
                            {
                                child.Height = 0;
                                child.Width = 0;
                            }
                            else if ((this.XOffset * calculate(CustomItems[count].value) - currentX) != 0)
                            {
                                child.Height = (child.DesiredSize.Height * Math.Ceiling(((child.DesiredSize.Width) / (this.XOffset * calculate(CustomItems[count].value) - currentX))));

                                child.Width = (this.XOffset * calculate(CustomItems[count].value) - (currentX)) - 1;
                            }
                            (child as TextBlock).TextWrapping = TextWrapping.Wrap;

                            entered = true;
                            if (UpOrientation && child.Height > this.Height)
                            {
                                child.Arrange(new Rect(location, child.DesiredSize));
                            }
                            else
                            {
                                Point loc = new Point(location.X, this.Height - child.DesiredSize.Height);
                                child.Arrange(new Rect(loc, child.DesiredSize));
                            }
                        }
                        else if (Math.Floor((currentX) + child.DesiredSize.Width) > this.PanelWidth + 23)
                        {
                            if (!(Math.Round((this.PanelWidth + 23 - (currentX))) < 0))
                            {
                                child.Height = (child.DesiredSize.Height * (Math.Ceiling(((child.DesiredSize.Width) / (this.PanelWidth + 23 - (currentX)))) + 1));
                                if (this.Height < child.Height)
                                {
                                    this.Height = child.Height;
                                }
                            }
                            if ((this.PanelWidth + 23 - (currentX)) < 0)
                            {
                                child.Width = this.PanelWidth + 23;
                            }
                            else
                            {
                                child.Width = this.PanelWidth + 23 - (currentX);
                            }
                            (child as TextBlock).TextWrapping = TextWrapping.Wrap;
                        }
                        else
                        {
                            if (UpOrientation)
                            {
                                Point loc = new Point(location.X, this.Height - child.DesiredSize.Height);
                                child.Arrange(new Rect(loc, child.DesiredSize));
                            }
                            else
                            {
                                double x = location.X - (child.DesiredSize.Width / 2);
                                if (x >= 0)
                                    location.X = x;
                                child.Arrange(new Rect(location, child.DesiredSize));
                            }

                            child.HorizontalAlignment = HorizontalAlignment.Left;
                            (child as TextBlock).TextAlignment = TextAlignment.Center;
                        }
                    }
                    else
                    {
                        if (child.ActualHeight < this.Height)
                        {
                            //currentY = this.Height - child.ActualHeight;
                        }
                        location = new Point(currentX, currentY);

                        //Point des = new Point(Math.Round(child.Width,1), child.DesiredSize.Height);
                        if (!((child as TextBlock).Text.Equals(prevstring)))
                        {
                            child.Arrange(new Rect(location, child.DesiredSize));
                        }
                        if (count == Children.Count && !(IsItem))
                        {
                            if (!((child as TextBlock).Text.Equals(prevstring)))
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
                    if (UpOrientation)
                    {
                        // child.Width = 10d;
                        // (child as TextBlock).Width = this.Width;
                        //(child as TextBlock).Background = new SolidColorBrush(Colors.Black);
                        //(child as TextBlock).Margin = new Thickness(0, 0, 5, 0);
                        (child as TextBlock).TextAlignment = TextAlignment.Right;
                    }
                    else
                    {
                        (child as TextBlock).TextAlignment = TextAlignment.Left;
                    }

                    if (IsItem)
                    {
                        double value = CustomItems[count - 1].value;
                        currentY = this.YOffsetDown * calculate(value);
                        if (entered)
                        {
                            currentX = 10;
                        }

                        child.Width = this.Width;
                        location = new Point(currentX, currentY);
                        if (count < CustomItems.Count && child.DesiredSize.Width > this.Width)
                        {
                            child.Width = this.Width;
                            (child as TextBlock).TextWrapping = TextWrapping.Wrap;

                            if (!(Math.Round(currentY) >= PanelHeight) && (currentY + (child as TextBlock).ActualHeight) > this.YOffsetDown * calculate(CustomItems[count].value))
                            {
                                (child as TextBlock).Height = (child as TextBlock).ActualHeight - (((currentY + (child as TextBlock).ActualHeight)) - (this.YOffsetDown * calculate(CustomItems[count].value)));
                                //(child as TextBlock).Clip = new RectangleGeometry() {Rect= new Rect(0, 0, child.Width, 50) };
                            }
                            if (Math.Round(currentY) >= PanelHeight)
                            {
                                (child as TextBlock).Height = 0;
                            }
                        }
                        else if (child.DesiredSize.Width > this.Width)
                        {
                            child.Width = this.Width;
                            (child as TextBlock).TextWrapping = TextWrapping.Wrap;
                            if ((currentY + (child as TextBlock).ActualHeight) > this.PanelHeight)
                            {
                                child.Height = (child as TextBlock).ActualHeight - ((currentY + (child as TextBlock).ActualHeight) - this.PanelHeight);
                            }
                        }
                        else
                        {
                            child.Arrange(new Rect(location, child.DesiredSize));
                            child.VerticalAlignment = VerticalAlignment.Top;
                        }
                    }
                    else
                    {
                        location = new Point(currentX, currentY);

                        if (!((child as TextBlock).Text.Equals(prevstring)))
                        {
                            location.Y = currentY;
                            child.Arrange(new Rect(location, child.DesiredSize));
                        }
                        if (count == Children.Count && !IsItem)
                        {
                            if (!((child as TextBlock).Text.Equals(prevstring)))
                            {
                                location.Y = this.PanelHeight;
                                child.Arrange(new Rect(location, child.DesiredSize));
                            }
                            else
                            {
                                (child as TextBlock).Text = string.Empty;
                            }
                        }
                        ////location = new Point(currentX, currentY);
                        //if (currentY - XOffset != this.PanelHeight)
                        //{
                        //    location.Y = this.PanelHeight;
                        //    child.Arrange(new Rect(location, child.DesiredSize));
                        //}
                        currentY += (this.YOffsetDown);
                        if (currentY > this.PanelHeight)
                        {
                            currentY = this.PanelHeight;
                        }
                        child.VerticalAlignment = VerticalAlignment.Top;
                    }
                }
                prevstring = (child as TextBlock).Text;
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

    /// <summary>
    /// Orientation the item
    /// </summary>
    public enum Itemorientation
    {
        /// <summary>
        /// Above the ticks
        /// </summary>
        AboveTicks,

        /// <summary>
        /// Between the ticks
        /// </summary>
        InBetweenTicks,
    }
}
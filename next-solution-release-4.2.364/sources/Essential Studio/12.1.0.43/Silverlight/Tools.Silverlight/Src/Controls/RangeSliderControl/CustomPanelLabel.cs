#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
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
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Tools.Controls
{

    /// <summary>
    /// Represents the Item Orientation Enumeration.
    /// </summary>
    public enum Itemorientation
    {
        /// <summary>
        /// 
        /// </summary>
        AboveTicks,
        /// <summary>
        /// 
        /// </summary>
        InBetweenTicks,
    }
    /// <summary>
    /// Represents the Custom Panel label.
    /// </summary>
    public class CustomPanelLabel : Panel
    {
        #region Internal Variables
        internal Itemorientation LabelOrientation { get; set; }
        internal double XOffset { get; set; }
        internal double YOffsetDown { get; set; }
        internal double PanelWidth { get; set; }
        internal double PanelHeight { get; set; }
        internal bool IsItem { get; set; }
        internal bool UpOrientation { get; set; }
        internal double minimum { get; set; }
        internal double maximum { get; set; }
        internal ObservableCollection<Items> CustomItems { get; set; }
        internal double offset = 0d;
        internal double actualheight=0d;
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

        #endregion

        #region Protected Override Methods
        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of child object allotted sizes.
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
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size used once the element is arranged.
        /// </returns>
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

                    if (IsItem)
                    {
                        double value = 0;
                        if (CustomItems != null)
                        {
                            value = CustomItems[count - 1].value;


                            currentX = this.XOffset * calculate(value);
                            location = new Point(currentX, currentY);
                            if (entered)
                            {
                                location.Y = 7;
                            }
                            if (count < CustomItems.Count && ((currentX + child.DesiredSize.Width) > this.XOffset * calculate(CustomItems[count].value)))
                            {
                                if ((this.XOffset * calculate(CustomItems[count].value) - currentX) != 0)
                                {
                                    if (Application.Current != null && Application.Current.RootVisual != null && (Application.Current.RootVisual as FrameworkElement).ActualWidth == 1024 && !UpOrientation)
                                        child.Height = (child.DesiredSize.Height * Math.Round(((child.DesiredSize.Width) / (this.XOffset) * calculate(CustomItems[count].value) - currentX)));
                                    else
                                        child.Height = (child.DesiredSize.Height * Math.Ceiling(((child.DesiredSize.Width) / (this.XOffset * calculate(CustomItems[count].value) - currentX))));                                   

                                    child.Width = (this.XOffset * calculate(CustomItems[count].value) - (currentX)) - 1;
                                }
                                (child as TextBlock).TextWrapping = TextWrapping.Wrap;

                                entered = true;
                                if (UpOrientation && child.Height > this.Height)
                                {
                                    child.Arrange(new Rect(location, child.DesiredSize));

                                }
                            }
                            else if (Math.Floor((currentX) + child.DesiredSize.Width) > this.PanelWidth + 23)
                            {
                                if (!(Math.Round((this.PanelWidth + 23 - (currentX))) < 0))
                                {
                                    child.Height = (child.DesiredSize.Height * Math.Ceiling(((child.DesiredSize.Width) / (this.PanelWidth + 23 - (currentX)))));
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

                                    child.Arrange(new Rect(location, child.DesiredSize));
                                }

                                child.HorizontalAlignment = HorizontalAlignment.Left;
                            }
                        }


                    }
                    else
                    {
                        if (child.ActualHeight < this.Height)
                        {
                            currentY = this.Height - child.ActualHeight;
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
                        child.Width = this.Width;
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

            return new Size(currentX, currentY);

        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Calculates the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        double calculate(double value)
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
        #endregion
    }
}

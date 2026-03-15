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
    /// Represents the Custom Panel Ticks Class.
    /// </summary>
    public class CustomPanelTicks : Panel
    {
        #region Internal Variables
        internal Itemorientation LabelOrientation { get; set; }
        internal double XOffset { get; set; }
        internal double YOffsetDown { get; set; }
        internal double PanelWidth { get; set; }
        internal double PanelHeight { get; set; }
        internal double offset = 0d;
        internal double maximum { get; set; }
        internal bool UpOrientation { get; set; }
        internal double minimum { get; set; }
        internal bool IsItem { get; set; }
        internal ObservableCollection<Items> CustomItems { get; set; }
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
            int count = 0,count1=0;
            foreach (FrameworkElement child in Children)
            {
                count++;
                Point location ;
                     
                if (XOffset != 0)
                {
                    if (IsItem  )
                    {
                          double value = CustomItems[count-1].value;
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
                    if (IsItem)
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

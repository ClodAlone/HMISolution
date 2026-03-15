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
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Tools.Controls 
{
    /// <summary>
    /// Represents the Custom Tab Panel Class.
    /// </summary>
    public class CustomTabPanel : TabPanel
    {
        /// <summary>
        /// The Raw Height.
        /// </summary>
        private double _rowHeight;

        /// <summary>
        /// The Scale factor.
        /// </summary>
        private double _scaleFactor;

        /// <summary>
        /// See how much size the Children Want.If not enough size, scale the Children to
        /// the Available size.
        /// </summary>
        /// <param name="availableSize">It give the parent Height and Width.</param>
        /// <returns>
        /// Size of the Each Children.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            //// See how much room the children want  
                double width = 0.0;
                this._rowHeight = 0.0;
                foreach (UIElement element in this.Children)
                {
                    if (element is CustomTabItem)
                    {
                        CustomTabItem custab = element as CustomTabItem;
                        custab.visibilitytxtDotSelected = false;
                        custab.visibilitytxtDotUnselected = false;
                        if ((bool)custab.IsSelected)
                        {
                            if (custab.txtDotSelected != null)
                            {
                                custab.txtDotSelected.Visibility = Visibility.Collapsed;
                                if (custab.txtDotUnselected != null)
                                {
                                    custab.txtDotUnselected.Visibility = Visibility.Collapsed;
                                }
                            }
                        }
                        else
                        {
                            if (custab.txtDotUnselected != null)
                            {
                                custab.txtDotUnselected.Visibility = Visibility.Collapsed;
                                if (custab.txtDotSelected != null)
                                {
                                    custab.txtDotSelected.Visibility = Visibility.Collapsed;
                                }
                            }
                        }
                    }
                    element.Measure(availableSize);
                    Size size = this.GetDesiredSizeLessMargin(element);
                    this._rowHeight = Math.Max(this._rowHeight, size.Height);
                    width += size.Width;
                    
                }

                //// If not enough room, scale the
                //// children to the available width
                if (width > availableSize.Width)
                {
                    this._scaleFactor = availableSize.Width / width;
                    width = 0.0;
                    foreach (UIElement element in this.Children)
                    {
                        element.Measure(new Size(element.DesiredSize.Width * this._scaleFactor, availableSize.Height));
                        width += element.DesiredSize.Width;
                        if (element is CustomTabItem)
                        {
                            CustomTabItem custab = element as CustomTabItem;
                            if ((bool)custab.IsSelected)
                            {
                                if (custab.txtDotSelected != null)
                                {
                                    custab.txtDotSelected.Visibility = Visibility.Visible;
                                    if (custab.txtDotUnselected != null)
                                    {
                                        custab.txtDotUnselected.Visibility = Visibility.Collapsed;
                                    }
                                }
                                else
                                {
                                    custab.visibilitytxtDotSelected = true;
                                    custab.visibilitytxtDotUnselected = false;
                                }
                            }
                            else
                            {
                                if (custab.txtDotUnselected != null)
                                {
                                    custab.txtDotUnselected.Visibility = Visibility.Visible;
                                    if (custab.txtDotSelected != null)
                                    {
                                        custab.txtDotSelected.Visibility = Visibility.Collapsed;
                                    }
                                }
                                else
                                {
                                    custab.visibilitytxtDotSelected = false;
                                    custab.visibilitytxtDotUnselected = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    this._scaleFactor = 1.0;
                }

                return new Size(width, this._rowHeight);            
        }

        /// <summary>
        /// Perform arranging of Children based on the Final size.
        /// </summary>
        /// <param name="arrangeSize">It gives the ActualHeight and Atualwidth of the
        /// parent.</param>
        /// <returns>
        /// Size of the Children.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        { 
                Point point = new Point();
                foreach (UIElement element in this.Children)
                {
                    Size size1 = element.DesiredSize;
                    Size size2 = this.GetDesiredSizeLessMargin(element);
                    Thickness margin = (Thickness)element.GetValue(FrameworkElement.MarginProperty);
                    double width = size2.Width;
                    if (element.DesiredSize.Width != size2.Width)
                    {
                        ////width = arrangeSize.Width - point.X; // Last-tab-selected "fix"
                    }

                    element.Arrange(new Rect(
                        point,
                        new Size(Math.Min(width, size2.Width), this._rowHeight)));
                    double leftRightMargin = Math.Max(0.0, -(margin.Left + margin.Right));
                    point.X += size1.Width + (leftRightMargin * this._scaleFactor);
                }

                return arrangeSize; 
        }

        /// <summary>
        /// This function return's size and after Subtracting Margin.
        /// </summary>
        /// <param name="element">Children of the tabpanel.</param>
        /// <returns>
        /// Size of the Children.
        /// </returns>
        private Size GetDesiredSizeLessMargin(UIElement element)
        {
            Thickness margin = (Thickness)element.GetValue(FrameworkElement.MarginProperty);
            Size size = new Size();
            size.Height = Math.Max(0.0, element.DesiredSize.Height - (margin.Top + margin.Bottom));
            size.Width = Math.Max(0.0, element.DesiredSize.Width - (margin.Left + margin.Right));
            return size;
        } 
    }
}

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

namespace Syncfusion.Windows.Tools.Controls 
{
    /// <summary>
    /// Represents the SidePanel Class.
    /// </summary>
    public class SidePanel : Panel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SidePanel"/> class.
        /// </summary>
        public SidePanel()
        {
          ////  this.Background = new SolidColorBrush(Colors.Orange);
        }

        /// <summary>
        /// Gets or sets the Window which is AutoHide.
        /// </summary>
        /// <value>The pinned window.</value>
        protected internal Window PinnedWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Dock Position of the Window.
        /// </summary>
        /// <value>The dock.</value>
        protected internal Dock Dock
        {
            get;
            set;
        }

       
        /// <summary>
        /// Measures the children of a Sidepanel in anticipation of arranging
        /// them during the ArrangeOverride pass.
        /// </summary>
        /// <param name="availableSize">A maximum Size to not exceed.</param>
        /// <returns>The desired size of the SidePanel.</returns>        
        protected override Size MeasureOverride(Size availableSize)
        {
            //if (PinnedWindow != null)
            //{
                //if (this.Dock == Dock.Left || this.Dock == Dock.Right)
                //{
                    double width = 0;
                    double height = 0;
                    foreach (var child in Children)
                    {
                        child.Measure(new Size(Double.PositiveInfinity, availableSize.Width));
                        height += child.DesiredSize.Width;
                        width = Math.Max(width, child.DesiredSize.Height);
                    }
                    return new Size(width, height);
                //}
                //else
                //{
                //    foreach (var child in Children)
                //    {
                //        child.Measure(new Size(Double.PositiveInfinity, availableSize.Width));
                //    }
                //    return availableSize;
                //}
            //}
            //else
            //{
            //    return availableSize;
            //}
           
        }

        /// <summary>
        /// Arranges the content (child elements) of a SidePanel element.
        /// </summary>
        /// <param name="finalSize">
        /// The Size the SidePanel uses to arrange its child elements.
        /// </param>
        /// <returns>The arranged size of the SidePanel.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double y = 1;
            double x = 1;
            foreach (var child in Children)
            {
                if (this.Dock == Dock.Left || this.Dock == Dock.Right)
                {
                    Rect arrangeRect = new Rect();
                    arrangeRect.X = 0;                    
                    if (child is SideButton)
                    {
                        if ((child as SideButton).initial == 3)
                        {
                            y += 3;
                        }
                    }
                    arrangeRect.Y = y;
                    arrangeRect.Width = child.DesiredSize.Width;
                    arrangeRect.Height = finalSize.Width;
                    child.Arrange(arrangeRect);
                    y += child.DesiredSize.Width;
                   

                    RotateTransform rotateTransform = new RotateTransform
                    {
                        Angle = 90,
                    };

                    TranslateTransform translateTransform = new TranslateTransform
                    {
                        X = finalSize.Width
                    };

                    TransformGroup transformGroup = new TransformGroup();
                    transformGroup.Children.Add(rotateTransform);
                    transformGroup.Children.Add(translateTransform);
                    child.RenderTransform = transformGroup;
                }
                else
                {
                    Rect arrangeRect = new Rect();
                    if (child is SideButton)
                    {
                        if ((child as SideButton).initial == 3)
                        {
                            x += 3;
                        }
                    }
                    arrangeRect.X = x;
                    arrangeRect.Y = 0;
                    arrangeRect.Width = child.DesiredSize.Width;
                    arrangeRect.Height = child.DesiredSize.Height;
                    child.Arrange(arrangeRect);
                    x += child.DesiredSize.Width;
                    
                }
            }
            return finalSize;
        }
    }
}

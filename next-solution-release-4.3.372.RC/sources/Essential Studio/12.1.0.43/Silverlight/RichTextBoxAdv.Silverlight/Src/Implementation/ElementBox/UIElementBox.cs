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
    public class UIElementBox: ElementBox
    {
        private double width;
        private double height;
        private UIElement uiElement;

        public UIElementBox()
        {
            IsUIBox = true;
        }

        /// <summary>
        /// Gets or Sets the UIElement
        /// </summary>
        public UIElement UIElement
        {
            get
            {
                return uiElement;
            }
            set
            {
                uiElement = value;
                Element = uiElement as FrameworkElement;
                Element.MouseMove += new MouseEventHandler(Element_MouseMove);
                Element.Width = Width;
                Element.Height = Height;
            }
        }

        void Element_MouseMove(object sender, MouseEventArgs e)
        {
            ((FrameworkElement)sender).Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Gets or Sets the Width of the UIElement
        /// </summary>
        public double Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                if ((uiElement as FrameworkElement) != null)
                {
                    (uiElement as FrameworkElement).Width = width;
                }
            }
        }

        /// <summary>
        /// Gets or Sets the height of the UIElement
        /// </summary>
        public double Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
                if ((uiElement as FrameworkElement) != null)
                {
                    (uiElement as FrameworkElement).Height = height;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal override void SetElementPosition()
        {
            double offset = Size.Height - Element.Height - CalculatedLineSpacing - AfterSpacing - Offset;

            Point point = new Point(Location.X, Location.Y + offset);

            ElementLocation = point;

            Canvas.SetLeft(Element, point.X);
            Canvas.SetTop(Element, point.Y);
            Canvas.SetZIndex(Element, 1);
        }

        internal override Point GetApproxRight(int index)
        {
            double y = BoundingRectangle.Top + BoundingRectangle.Height / 2;
            double x = 0;
            if (index == 0)
            {
                x = ElementLocation.X;
            }
            else
            {
                x = ElementLocation.X + Width;
            }
            return new Point(x, y);
        }
    }
}

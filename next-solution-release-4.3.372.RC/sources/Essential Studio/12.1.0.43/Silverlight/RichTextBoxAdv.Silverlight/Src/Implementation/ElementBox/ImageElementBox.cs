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
    public class ImageElementBox: ElementBox
    {
        private ImageSource imageSource;
        private double width;
        private double height; 
        private Image image;

        public ImageElementBox()
        {
            image = new Image();
            image.MouseMove += new MouseEventHandler(image_MouseMove);
            image.Stretch = Stretch.Fill;
            Element = image;
            IsImageBox = true;
        }

        void image_MouseMove(object sender, MouseEventArgs e)
        {
            ((Image)sender).Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Gets or Sets the image source
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                return imageSource;
            }
            set
            {
                imageSource = value;
                image.Source = imageSource;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Image Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
            }
        }

        /// <summary>
        /// 
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
                image.Width = width;
            }
        }

        /// <summary>
        /// 
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
                image.Height = height;
                baselineOffset = height;
            }
        }

        internal override void SetElementPosition()
        {
            double offset = LineInfo.GetMaximumAscent() - BaselineOffset;

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

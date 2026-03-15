#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{
    public partial class ImageResizer : UserControl
    {
        bool flag = true;
        ImageSource imageSource;
        double width = 0;
        double height = 0;
        double left = 0;
        double top = 0;
        readonly double VerticalAdjustment = 0;
        readonly double HorizontalAdjustment = 0;
        private ImageContainerAdv imageContainer;
        internal DragCompletedCallBack DragCompletedCallBack;
        internal RichTextBoxAdv OwnerControl;

        internal bool IsResizable
        {
            get
            {
                return !OwnerControl.IsReadOnly;
            }
        }

        private const double PI = 3.14159265358979323846264338327950288;

        /// <summary>
        /// Initializes the new instance of ImageResizer class
        /// </summary>
        public ImageResizer()
        {
            InitializeComponent();
            resizeGrid.SizeChanged += new SizeChangedEventHandler(resizeGrid_SizeChanged);
            DataContext = this;
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
                preViewImage.Source = imageSource;
                Canvas.SetZIndex(this, 3);
            }
        }

        internal ImageContainerAdv ImageContainer
        {
            get
            {
                return imageContainer;
            }
            set
            {
                imageContainer = value;
                flag = true;
            }
        }

        /// <summary>
        /// Gets or Sets the image width
        /// </summary>
        public double ImageWidth
        {
            get
            {
                return width;
            }
            set
            {
                width = value + (HorizontalAdjustment * 2);
                Width = width;
            }
        }

        /// <summary>
        /// Gets or Sets the image height
        /// </summary>
        public double ImageHeight
        {
            get
            {
                return height;
            }
            set
            {
                height = value + (VerticalAdjustment * 2);
                Height = height;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Left
        {
            get
            {
                return Canvas.GetLeft(this);
            }
            set
            {
                left = value;
                Canvas.SetLeft(this, left - HorizontalAdjustment);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Top
        {
            get
            {
                return Canvas.GetTop(this);
            }
            set
            {
                top = value;
                Canvas.SetTop(this, top - VerticalAdjustment);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal Image PreviewImage
        {
            get
            {
                return preViewImage;
            }
        }

        void resizeGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (flag)
            {
                if (!OwnerControl.IsReadOnly)
                {
                    rect.Width = resizeGrid.ActualWidth;
                    rect.Height = resizeGrid.ActualHeight;
                }
            }
        }

        private void resizeTopLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!OwnerControl.IsReadOnly)
            {
                flag = false;
                Left = rect.Width - e.HorizontalChange < 8 ? Left + (rect.Width - 8) : Left + e.HorizontalChange;
                Top = rect.Height - e.HorizontalChange < 8 ? Top + (rect.Height - 8) : Top + e.HorizontalChange;
                rect.Width = rect.Width - e.HorizontalChange < 8 ? 8 : rect.Width - e.HorizontalChange;
                rect.Height = rect.Height - e.HorizontalChange < 8 ? 8 : rect.Height - e.HorizontalChange;
                Width = rect.Width;
                Height = rect.Height;
                preViewImage.Clip = null;
            }
        }

        private void resizeMiddleLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!OwnerControl.IsReadOnly)
            {
                flag = false;
                Left = rect.Width - e.HorizontalChange < 8 ? Left + (rect.Width - 8) : Left + e.HorizontalChange;
                rect.Width = rect.Width - e.HorizontalChange < 8 ? 8 : rect.Width - e.HorizontalChange;
                Width = rect.Width;
                preViewImage.Clip = null;
            }
        }

        private void resizeBottomLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!OwnerControl.IsReadOnly)
            {
                flag = false;
                Left = rect.Width - e.HorizontalChange < 8 ? Left + (rect.Width - 8) : Left + e.HorizontalChange;
                rect.Width = rect.Width - e.HorizontalChange < 8 ? 8 : rect.Width - e.HorizontalChange;
                rect.Height = rect.Height - e.HorizontalChange < 8 ? 8 : rect.Height - e.HorizontalChange;
                Width = rect.Width;
                Height = rect.Height; preViewImage.Clip = null;
            }
        }

        private void resizeTopMiddle_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!OwnerControl.IsReadOnly)
            {
                flag = false;
                Top = rect.Height - e.VerticalChange < 8 ? Top + (rect.Height - 8) : Top + e.VerticalChange;
                rect.Height = rect.Height - e.VerticalChange < 8 ? 8 : rect.Height - e.VerticalChange;
                Height = rect.Height;
                preViewImage.Clip = null;
            }
        }

        private void resizeBottomMiddle_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!OwnerControl.IsReadOnly)
            {
                flag = false;
                rect.Height = rect.Height + e.VerticalChange < 8 ? 8 : rect.Height + e.VerticalChange;
                Height = rect.Height;
                preViewImage.Clip = null;
            }
        }

        private void resizeBottomRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!OwnerControl.IsReadOnly)
            {
                flag = false;
                rect.Width = rect.Width + e.HorizontalChange < 8 ? 8 : rect.Width + e.HorizontalChange;
                rect.Height = rect.Height + e.HorizontalChange < 8 ? 8 : rect.Height + e.HorizontalChange;
                Width = rect.Width;
                Height = rect.Height;
                preViewImage.Clip = null;
            }
        }

        private void resizeTopRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!OwnerControl.IsReadOnly)
            {
                flag = false;
                Top = rect.Height + e.HorizontalChange < 8 ? Top + (rect.Height - 8) : Top - e.HorizontalChange;
                rect.Width = rect.Width + e.HorizontalChange < 8 ? 8 : rect.Width + e.HorizontalChange;
                rect.Height = rect.Height + e.HorizontalChange < 8 ? 8 : rect.Height + e.HorizontalChange;
                Width = rect.Width;
                Height = rect.Height;
                preViewImage.Clip = null;
            }
        }

        private void resizeMiddleRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!OwnerControl.IsReadOnly)
            {
                flag = false;
                rect.Width = rect.Width + e.HorizontalChange < 8 ? 8 : rect.Width + e.HorizontalChange;
                Width = rect.Width;
                preViewImage.Clip = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragCompleted(object sender, DragCompletedEventArgs e)
        {
            DragCompletedCallBack(this, ImageContainer);
        }

        internal void UpdateResizerLocation()
        {
            if (ImageContainer != null && Visibility != Visibility.Collapsed)
            {
                Left = Canvas.GetLeft(ImageContainer.ElementBox.Element);
                Top = Canvas.GetTop(ImageContainer.ElementBox.Element);
            }
        }

        internal void UpdateResizerLocationAndParent()
        {
            if (ImageContainer != null && Visibility != Visibility.Collapsed)
            {
                Left = Canvas.GetLeft(ImageContainer.ElementBox.Element);
                Top = Canvas.GetTop(ImageContainer.ElementBox.Element);

                if (ImageContainer.ElementBox.Element.Parent != null && ImageContainer.ElementBox.Element.Parent is Canvas)
                {
                    Canvas canvas = ImageContainer.ElementBox.Element.Parent as Canvas;
                    if (this.Parent != null && this.Parent is Canvas)
                    {
                        (this.Parent as Canvas).Children.Remove(this);
                    }

                    canvas.Children.Add(this);
                }
            }

            
        }
    }
}

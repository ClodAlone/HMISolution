// <copyright file="MosaicTileContent.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows;
using System.Windows.Media;
namespace Syncfusion.WP.Controls.Notification
#else
#if SILVERLIGHT
using System.Windows;
using System.Windows.Media;
namespace Syncfusion.Tools.Controls.Notification
#else
#if WPF
using System.Windows;
using System.Windows.Media;
namespace Syncfusion.Windows.Controls.Notification
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
namespace Syncfusion.UI.Xaml.Controls.Notification
#endif
#endif
#endif
{
    /// <summary>
    /// Represents a MosaicTileContent Control
    /// </summary>
    [ClassReference(IsReviewed = false,ShouldInclude=false)]
    public class  MosaicTileContent : DependencyObject
    {
        #region Constructor

        internal MosaicTileContent()
        {

        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { 
                SetValue(ImageProperty, value); 
            }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(string), typeof(MosaicTileContent), new PropertyMetadata(null));




        /// <summary>
        /// Gets or sets the width of the image.
        /// </summary>
        /// <value>The width of the image.</value>
        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ImageWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(MosaicTileContent), new PropertyMetadata(double.NaN));



        /// <summary>
        /// Gets or sets the height of the image.
        /// </summary>
        /// <value>The height of the image.</value>
        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ImageHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(MosaicTileContent), new PropertyMetadata(double.NaN));


        /// <summary>
        /// Gets or sets the horizontal alignment of the image.
        /// </summary>
        /// <value>The horizontal image alignment.</value>
        public HorizontalAlignment HorizontalImageAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalImageAlignmentProperty); }
            set { SetValue(HorizontalImageAlignmentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HorizontalImageAlignment.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HorizontalImageAlignmentProperty =
            DependencyProperty.Register("HorizontalImageAlignment", typeof(HorizontalAlignment), typeof(MosaicTileContent), new PropertyMetadata(HorizontalAlignment.Left));




        /// <summary>
        /// Gets or sets the vertical alignment of the image.
        /// </summary>
        /// <value>The vertical image alignment.</value>
        public VerticalAlignment VerticalImageAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalImageAlignmentProperty); }
            set { SetValue(VerticalImageAlignmentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for VerticalImageAlignment.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VerticalImageAlignmentProperty =
            DependencyProperty.Register("VerticalImageAlignment", typeof(VerticalAlignment), typeof(MosaicTileContent), new PropertyMetadata(VerticalAlignment.Top));



        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>The background.</value>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(MosaicTileContent), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the opacity.
        /// </summary>
        /// <value>The opacity.</value>
        public double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Opacity.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register("Opacity", typeof(double), typeof(MosaicTileContent), new PropertyMetadata(0.8));

        #endregion

    }
}

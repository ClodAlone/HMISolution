// <copyright file="TextImageControl.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#region file using
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Syncfusion.Licensing;
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents TextImageControl class
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TextImageControl : Control
    {
        #region Constants
        /// <summary>
        /// Width of the image.
        /// </summary>
        private const double DEF_IMAGE_WIDTH = 20;
        
        /// <summary>
        /// Height of the image.
        /// </summary>
        private const double DEF_IMAGE_HEIGHT = 20;
        
        /// <summary>
        /// Horizontal alignment of the image.
        /// </summary>
        private const HorizontalAlignment DEF_IMAGE_HALIGNMENT = HorizontalAlignment.Left;
        
        /// <summary>
        /// Horizontal alignment of the text.
        /// </summary>
        private const HorizontalAlignment DEF_TEXT_HALIGNMENT = HorizontalAlignment.Center;
        
        /// <summary>
        /// Vertical alignment of the image.
        /// </summary>
        private const VerticalAlignment DEF_IMAGE_VALIGNMENT = VerticalAlignment.Center;
        
        /// <summary>
        /// Vertical alignment of the text.
        /// </summary>
        private const VerticalAlignment DEF_TEXT_VALIGNMENT = VerticalAlignment.Center;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies <see cref="ImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(TextImageControl));
        
        /// <summary>
        /// Identifies <see cref="ImageWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register("ImageWidth", typeof(double), typeof(TextImageControl), new UIPropertyMetadata(DEF_IMAGE_WIDTH));
        
        /// <summary>
        /// Identifies <see cref="ImageHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register("ImageHeight", typeof(double), typeof(TextImageControl), new UIPropertyMetadata(DEF_IMAGE_HEIGHT));
        
        /// <summary>
        /// Identifies <see cref="ImageHorizontalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageHorizontalAlignmentProperty = DependencyProperty.Register("ImageHorizontalAlignment", typeof(HorizontalAlignment), typeof(TextImageControl), new UIPropertyMetadata(DEF_IMAGE_HALIGNMENT));
        
        /// <summary>
        /// Identifies <see cref="ImageVerticalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageVerticalAlignmentProperty = DependencyProperty.Register("ImageVerticalAlignment", typeof(VerticalAlignment), typeof(TextImageControl), new UIPropertyMetadata(DEF_IMAGE_VALIGNMENT));
        
        /// <summary>
        /// Identifies <see cref="ImageMargin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageMarginProperty = DependencyProperty.Register("ImageMargin", typeof(Thickness), typeof(TextImageControl));
        
        /// <summary>
        /// Identifies <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TextImageControl), new UIPropertyMetadata(null));
        
        /// <summary>
        /// Identifies <see cref="TextHorizontalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextHorizontalAlignmentProperty = DependencyProperty.Register("TextHorizontalAlignment", typeof(HorizontalAlignment), typeof(TextImageControl), new UIPropertyMetadata(DEF_TEXT_HALIGNMENT));
        
        /// <summary>
        /// Identifies <see cref="TextVerticalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextVerticalAlignmentProperty = DependencyProperty.Register("TextVerticalAlignment", typeof(VerticalAlignment), typeof(TextImageControl), new UIPropertyMetadata(DEF_TEXT_VALIGNMENT));
        
        /// <summary>
        /// Identifies <see cref="TextMargin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextMarginProperty = DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(TextImageControl));

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =DependencyProperty.Register("Content", typeof(object), typeof(TextImageControl), new FrameworkPropertyMetadata(null));

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the source of the image to display.
        /// </summary>
        /// <value>
        /// Type: <see cref="ImageSource"/>
        /// </value>
        /// <seealso cref="ImageSource"/>
        public ImageSource ImageSource
        {
            get
            {
                return (ImageSource)GetValue(ImageSourceProperty);
            }

            set
            {
                SetValue(ImageSourceProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets image width.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        /// <seealso cref="double"/>
        public double ImageWidth
        {
            get
            {
                return (double)GetValue(ImageWidthProperty);
            }

            set
            {
                SetValue(ImageWidthProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets image height.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        /// <seealso cref="double"/>
        public double ImageHeight
        {
            get
            {
                return (double)GetValue(ImageHeightProperty);
            }

            set
            {
                SetValue(ImageHeightProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets image horizontal alignment.
        /// </summary>
        /// <value>
        /// Type: <see cref="HorizontalAlignment"/>
        /// </value>
        /// <seealso cref="HorizontalAlignment"/>
        public HorizontalAlignment ImageHorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment)GetValue(ImageHorizontalAlignmentProperty);
            }

            set
            {
                SetValue(ImageHorizontalAlignmentProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets image vertical alignment.
        /// </summary>
        /// <value>
        /// Type: <see cref="VerticalAlignment"/>
        /// </value>
        /// <seealso cref="VerticalAlignment"/>
        public VerticalAlignment ImageVerticalAlignment
        {
            get
            {
                return (VerticalAlignment)GetValue(ImageVerticalAlignmentProperty);
            }

            set
            {
                SetValue(ImageVerticalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets image margin.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// </value>
        /// <seealso cref="Thickness"/>
        public Thickness ImageMargin
        {
            get
            {
                return (Thickness)GetValue(ImageMarginProperty);
            }

            set
            {
                SetValue(ImageMarginProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets text to display.
        /// </summary>
        /// <value>
        /// Type: <see cref="string"/>
        /// </value>
        /// <seealso cref="string"/>
        public virtual string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets text horizontal alignment.
        /// </summary>
        /// <value>
        /// Type: <see cref="HorizontalAlignment"/>
        /// </value>
        /// <seealso cref="HorizontalAlignment"/>
        public HorizontalAlignment TextHorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment)GetValue(TextHorizontalAlignmentProperty);
            }

            set
            {
                SetValue(TextHorizontalAlignmentProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets text vertical alignment.
        /// </summary>
        /// <value>
        /// Type: <see cref="VerticalAlignment"/>
        /// </value>
        /// <seealso cref="VerticalAlignment"/>
        public VerticalAlignment TextVerticalAlignment
        {
            get
            {
                return (VerticalAlignment)GetValue(TextVerticalAlignmentProperty);
            }

            set
            {
                SetValue(TextVerticalAlignmentProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets text margin.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// </value>
        /// <seealso cref="Thickness"/>
        public Thickness TextMargin
        {
            get
            {
                return (Thickness)GetValue(TextMarginProperty);
            }

            set
            {
                SetValue(TextMarginProperty, value);
            }
        }

        public object Content
        {
            get 
            { 
                return (object)GetValue(ContentProperty); 
            }
            set 
            { 
                SetValue(ContentProperty, value); 
            }
        }
        
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes static members of the <see cref="TextImageControl"/> class.
        /// </summary>
        static TextImageControl()
        {
            EnvironmentTest.ValidateLicense(typeof(TextImageControl));
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TextImageControl"/> class.
        /// </summary>
        public TextImageControl()
            : base()
        {
        }
        #endregion
    }
}

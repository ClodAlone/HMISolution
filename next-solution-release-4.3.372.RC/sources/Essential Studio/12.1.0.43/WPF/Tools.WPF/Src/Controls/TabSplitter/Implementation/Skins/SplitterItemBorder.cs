// <copyright file="SplitterItemBorder.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represent the SplitterItemBorder control
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SplitterItemBorder : Decorator
    {
        #region Public properties
        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>The background.</value>
        [DefaultValue((string)null)]
        public Brush Background
        {
            get
            {
                return (Brush)GetValue(BackgroundProperty);
            }

            set
            {
                SetValue(BackgroundProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the border brush.
        /// </summary>
        /// <value>The border brush.</value>
        [DefaultValue((string)null)]
        public Brush BorderBrush
        {
            get
            {
                return (Brush)GetValue(BorderBrushProperty);
            }

            set
            {
                SetValue(BorderBrushProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the border inner brush.
        /// </summary>
        /// <value>The border inner brush.</value>
        [DefaultValue((string)null)]
        public Brush BorderInnerBrush
        {
            get
            {
                return (Brush)GetValue(BorderInnerBrushProperty);
            }

            set
            {
                SetValue(BorderInnerBrushProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the bottom border brush.
        /// </summary>
        /// <value>The bottom border brush.</value>
        [DefaultValue((string)null)]
        public Brush BottomBorderBrush
        {
            get
            {
                return (Brush)GetValue(BottomBorderBrushProperty);
            }

            set
            {
                SetValue(BottomBorderBrushProperty, value);
            }
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Represents Background dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty = Panel.BackgroundProperty.AddOwner(typeof(SplitterItemBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.AffectsRender));
        
        /// <summary>
        /// Represents BorderBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(SplitterItemBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.AffectsRender));
        
        /// <summary>
        /// Represents BorderInnerBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderInnerBrushProperty = DependencyProperty.Register("BorderInnerBrush", typeof(Brush), typeof(SplitterItemBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.AffectsRender));
        
        /// <summary>
        /// Represents BottomBorderBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomBorderBrushProperty = DependencyProperty.Register("BottomBorderBrush", typeof(Brush), typeof(SplitterItemBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion
    }
}

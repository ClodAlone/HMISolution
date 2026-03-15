// <copyright file="GaugeElement.cs" company="Syncfusion Software">
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
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the Gauge element's border and background brushes.
    /// </summary>
    /// <remarks>
    /// All the base classes of the gauge elements, such as <see cref="RangeBase"/> and <see cref="TickBase"/>  and classes such as
    /// <see cref="LinearPointer"/>, <see cref="CircularPointer"/> and <see cref="PointerCap"/> derives from this class.
    /// </remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(true)]
#endif
    public class GaugeElement : Control
    {

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="BackgroundBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundBrushProperty =
            DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(GaugeElement), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Transparent), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="BorderBrush"/> dependency property.
        /// </summary>
        public static readonly new DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(GaugeElement), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Transparent), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="BorderWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderWidthProperty =
            DependencyProperty.Register("BorderWidth", typeof(double), typeof(GaugeElement), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the background brush of the element.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public Brush BackgroundBrush
        {
            get
            {
                return (Brush)GetValue(BackgroundBrushProperty);
            }

            set
            {
                SetValue(BackgroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the border brush of the element.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public new Brush BorderBrush
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
        /// Gets or sets the border width of the element.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        public double BorderWidth
        {
            get
            {
                return (double)GetValue(BorderWidthProperty);
            }

            set
            {
                SetValue(BorderWidthProperty, value);
            }
        }
        #endregion DP Getters & Setters

        #region Implementation
     
        #endregion Implementation        
    }
}

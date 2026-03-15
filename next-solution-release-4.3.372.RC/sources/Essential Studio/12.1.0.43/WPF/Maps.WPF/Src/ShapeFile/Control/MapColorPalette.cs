#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    ///  MapColorPalette is palette of colors to be filled in the Map shapes
    /// </summary>
    public class MapColorPallette : DependencyObject
    {
        /// <summary>
        /// Gets or sets the shape fill.
        /// </summary>
        /// <value>The shape fill.</value>
        public Brush ShapeFill
        {
            get { return (Brush)GetValue(ShapeFillProperty); }
            set { SetValue(ShapeFillProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for ShapeFill.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShapeFillProperty =
            DependencyProperty.Register("ShapeFill", typeof(Brush), typeof(MapColorPallette), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the path stroke.
        /// </summary>
        /// <value>The path stroke.</value>
        public Brush PathStroke
        {
            get { return (Brush)GetValue(PathStrokeProperty); }
            set { SetValue(PathStrokeProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for PathStrock.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PathStrokeProperty =
            DependencyProperty.Register("PathStroke", typeof(Brush), typeof(MapColorPallette), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the path stroke thickness.
        /// </summary>
        /// <value>The path stroke thickness.</value>
        public double PathStrokeThickness
        {
            get { return (double)GetValue(PathStrokeThicknessProperty); }
            set { SetValue(PathStrokeThicknessProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for PathStrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PathStrokeThicknessProperty =
            DependencyProperty.Register("PathStrokeThickness", typeof(double), typeof(MapColorPallette), new PropertyMetadata(null));
    }
}

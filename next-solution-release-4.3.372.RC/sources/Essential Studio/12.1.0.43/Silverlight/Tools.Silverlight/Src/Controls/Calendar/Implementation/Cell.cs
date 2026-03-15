#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
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

    /// <summary>
    /// Implements the basic functionality required by the cell.
    /// </summary>
    public abstract class Cell : ContentControl
    {
        /// <summary>
        /// Identifies the BorderBrush Dependency Property.
        /// </summary>
        public static readonly new DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(Cell), new PropertyMetadata(new SolidColorBrush(Colors.Magenta)));

        /// <summary>
        /// Identifies the IsMouseHover Dependency Property.
        /// </summary>
        public static readonly DependencyProperty IsMouseHoverProperty =
            DependencyProperty.Register("IsMouseHover", typeof(bool), typeof(Cell), new PropertyMetadata(false));

        /// <summary>
        /// Identifies <see cref="IsSelected"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(Cell), new PropertyMetadata(false));

        /// <summary>
        /// Corner radius for the cell.
        /// </summary>
        private CornerRadius mcornerradius;

        /// <summary>
        /// Gets or sets a brush that describes the border background of a control.
        /// </summary>
        /// <value></value>
        /// <returns>The brush that is used to fill the control's border; the default is null.</returns>
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
        /// Gets or sets corner radius for the cell.
        /// </summary>
        /// <value>
        /// Type: <see cref="CornerRadius"/>
        /// </value>
        /// <seealso cref="CornerRadius"/>
        public CornerRadius CornerRadius
        {
            get
            {
                return this.mcornerradius;
            }

            set
            {
                this.mcornerradius = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is mouse hover.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is mouse hover; otherwise, <c>false</c>.
        /// </value>
        public bool IsMouseHover
        {
            get
            {
                return (bool)GetValue(IsMouseHoverProperty);
            }

            set
            {
                SetValue(IsMouseHoverProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether cell is selected.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }

            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }
    }
}
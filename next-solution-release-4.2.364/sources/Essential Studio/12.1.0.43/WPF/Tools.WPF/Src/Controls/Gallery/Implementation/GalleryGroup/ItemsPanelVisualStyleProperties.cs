// <copyright file="ItemsPanelVisualStyleProperties.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Gallery Class
    /// </summary>

    public partial class Gallery
    {
        #region Properties
        /// <summary>
        /// Gets or sets the value that represents corner radius of group items panel.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Default value is 3.
        /// </value>
        public double PanelCornerRadius
        {
            get
            {
                return (double)GetValue(PanelCornerRadiusProperty);
            }

            set
            {
                SetValue(PanelCornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents border thickness of group items panel.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Default value is 1.
        /// </value>
        public double PanelBorderThickness
        {
            get
            {
                return (double)GetValue(PanelBorderThicknessProperty);
            }

            set
            {
                SetValue(PanelBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents border brush of group items panel.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Default value is transparent brush.
        /// </value>
        public Brush PanelBorderBrush
        {
            get
            {
                return (Brush)GetValue(PanelBorderBrushProperty);
            }

            set
            {
                SetValue(PanelBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents background brush of group items panel.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Default value is transparent brush.
        /// </value>
        public Brush PanelBackground
        {
            get
            {
                return (Brush)GetValue(PanelBackgroundProperty);
            }

            set
            {
                SetValue(PanelBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents background brush of group items panel when mouse is over that group.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Default value is transparent brush.
        /// </value>
        public Brush PanelMouseOverBackground
        {
            get
            {
                return (Brush)GetValue(PanelMouseOverBackgroundProperty);
            }

            set
            {
                SetValue(PanelMouseOverBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents background brush of group items panel when group is selected.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Default value is transparent brush.
        /// </value>
        public Brush PanelSelectedBackground
        {
            get
            {
                return (Brush)GetValue(PanelSelectedBackgroundProperty);
            }

            set
            {
                SetValue(PanelSelectedBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents padding for group items panel. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>        
        /// </value>
        public Thickness PanelPadding
        {
            get
            {
                return (Thickness)GetValue(PanelPaddingProperty);
            }

            set
            {
                SetValue(PanelPaddingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents margins of group items panel. This is dependency properties.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// <para/>
        /// Default value is 0.
        /// </value> 
        public Thickness PanelMargin
        {
            get
            {
                return (Thickness)GetValue(PanelMarginProperty);
            }

            set
            {
                SetValue(PanelMarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents border brush of group items panel when mouse is over that group.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Default value is transparent brush.
        /// </value>
        public Brush PanelMouseOverBorderBrush
        {
            get
            {
                return (Brush)GetValue(PanelMouseOverBorderBrushProperty);
            }

            set
            {
                SetValue(PanelMouseOverBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents border brush of group items panel when group is selected.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Default value is transparent brush.
        /// </value>
        public Brush PanelSelectedBorderBrush
        {
            get
            {
                return (Brush)GetValue(PanelSelectedBorderBrushProperty);
            }

            set
            {
                SetValue(PanelSelectedBorderBrushProperty, value);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Raises <see cref="PanelCornerRadiusChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPanelCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PanelCornerRadiusChanged != null)
            {
                PanelCornerRadiusChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPanelCornerRadiusChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPanelCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnPanelCornerRadiusChanged(e);
        }

        /// <summary>
        /// Raises <see cref="PanelBorderThicknessChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPanelBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PanelBorderThicknessChanged != null)
            {
                PanelBorderThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPanelBorderThicknessChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPanelBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnPanelBorderThicknessChanged(e);
        }

        /// <summary>
        /// Raises <see cref="PanelBorderBrushChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnPanelBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PanelBorderBrushChanged != null)
            {
                PanelBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPanelBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPanelBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnPanelBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises <see cref="PanelBackgroundChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPanelBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PanelBackgroundChanged != null)
            {
                PanelBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPanelBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPanelBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnPanelBackgroundChanged(e);
        }

        /// <summary>
        /// Raises <see cref="PanelMouseOverBackgroundChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPanelMouseOverBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PanelMouseOverBackgroundChanged != null)
            {
                PanelMouseOverBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPanelMouseOverBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPanelMouseOverBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnPanelMouseOverBackgroundChanged(e);
        }

        /// <summary>
        /// Raises <see cref="PanelSelectedBackgroundChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPanelSelectedBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PanelSelectedBackgroundChanged != null)
            {
                PanelSelectedBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPanelSelectedBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPanelSelectedBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnPanelSelectedBackgroundChanged(e);
        }

        /// <summary>
        /// Raises <see cref="PanelPaddingChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPanelPaddingChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PanelPaddingChanged != null)
            {
                PanelPaddingChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPanelPaddingChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPanelPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnPanelPaddingChanged(e);
        }

        /// <summary>
        /// Raises <see cref="PanelMarginChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPanelMarginChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PanelMarginChanged != null)
            {
                PanelMarginChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPanelMarginChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPanelMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnPanelMarginChanged(e);
        }

        /// <summary>
        /// Raises <see cref="PanelMouseOverBorderBrushChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPanelMouseOverBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PanelMouseOverBorderBrushChanged != null)
            {
                PanelMouseOverBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPanelMouseOverBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPanelMouseOverBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnPanelMouseOverBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises <see cref="PanelSelectedBorderBrushChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPanelSelectedBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PanelSelectedBorderBrushChanged != null)
            {
                PanelSelectedBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPanelSelectedBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPanelSelectedBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnPanelSelectedBorderBrushChanged(e);
        }

        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="PanelCornerRadius"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PanelCornerRadiusChanged;

        /// <summary>
        /// Event that is raised when <see cref="PanelBorderThickness"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PanelBorderThicknessChanged;

        /// <summary>
        /// Event that is raised when <see cref="PanelBorderBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PanelBorderBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="PanelBackground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PanelBackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="PanelMouseOverBackground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PanelMouseOverBackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="PanelSelectedBackground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PanelSelectedBackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="PanelPadding"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PanelPaddingChanged;

        /// <summary>
        /// Event that is raised when <see cref="PanelMargin"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PanelMarginChanged;

        /// <summary>
        /// Event that is raised when <see cref="PanelMouseOverBorderBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PanelMouseOverBorderBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="PanelSelectedBorderBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PanelSelectedBorderBrushChanged;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies <see cref="PanelCornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanelCornerRadiusProperty =
            DependencyProperty.Register("PanelCornerRadius", typeof(double), typeof(Gallery), new FrameworkPropertyMetadata(3d, new PropertyChangedCallback(OnPanelCornerRadiusChanged)));

        /// <summary>
        /// Identifies <see cref="PanelBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanelBorderThicknessProperty =
            DependencyProperty.Register("PanelBorderThickness", typeof(double), typeof(Gallery), new FrameworkPropertyMetadata(1d, new PropertyChangedCallback(OnPanelBorderThicknessChanged)));

        /// <summary>
        /// Identifies <see cref="PanelBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanelBorderBrushProperty =
            DependencyProperty.Register("PanelBorderBrush", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnPanelBorderBrushChanged)));

        /// <summary>
        /// Identifies <see cref="PanelBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanelBackgroundProperty =
            DependencyProperty.Register("PanelBackground", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnPanelBackgroundChanged)));

        /// <summary>
        /// Identifies <see cref="PanelMouseOverBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanelMouseOverBackgroundProperty =
            DependencyProperty.Register("PanelMouseOverBackground", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnPanelMouseOverBackgroundChanged)));

        /// <summary>
        /// Identifies <see cref="PanelSelectedBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanelSelectedBackgroundProperty =
            DependencyProperty.Register("PanelSelectedBackground", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnPanelSelectedBackgroundChanged)));

        /// <summary>
        /// Identifies <see cref="PanelPadding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanelPaddingProperty =
            DependencyProperty.Register("PanelPadding", typeof(Thickness), typeof(Gallery), new FrameworkPropertyMetadata(new Thickness(7.5, 4, 7.5, 4), new PropertyChangedCallback(OnPanelPaddingChanged)));

        /// <summary>
        /// Identifies <see cref="PanelMargin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanelMarginProperty =
            DependencyProperty.Register("PanelMargin", typeof(Thickness), typeof(Gallery), new FrameworkPropertyMetadata(new Thickness(0), new PropertyChangedCallback(OnPanelMarginChanged)));

        /// <summary>
        /// Identifies <see cref="PanelMouseOverBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanelMouseOverBorderBrushProperty =
            DependencyProperty.Register("PanelMouseOverBorderBrush", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(Brushes.LightGray, new PropertyChangedCallback(OnPanelMouseOverBorderBrushChanged)));

        /// <summary>
        /// Identifies <see cref="PanelSelectedBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanelSelectedBorderBrushProperty =
            DependencyProperty.Register("PanelSelectedBorderBrush", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(Brushes.LightGray, new PropertyChangedCallback(OnPanelSelectedBorderBrushChanged)));
        #endregion
    }
}

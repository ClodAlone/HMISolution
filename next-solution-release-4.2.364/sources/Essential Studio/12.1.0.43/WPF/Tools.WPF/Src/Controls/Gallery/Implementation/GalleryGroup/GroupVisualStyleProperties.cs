// <copyright file="GroupVisualStyleProperties.cs" company="Syncfusion">
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
    /// Gallery class
    /// </summary>

    public partial class Gallery
    {
        #region Properties
        /// <summary>
        /// Gets or sets the value that represents corner radius of groups.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Default value is 3.
        /// </value>
        public double GroupCornerRadius
        {
            get
            {
                return (double)GetValue(GroupCornerRadiusProperty);
            }

            set
            {
                SetValue(GroupCornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents border thickness of groups.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Default value is 1.
        /// </value>
        public double GroupBorderThickness
        {
            get
            {
                return (double)GetValue(GroupBorderThicknessProperty);
            }

            set
            {
                SetValue(GroupBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents border brush of groups.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Default value is transparent brush.
        /// </value>
        public Brush GroupBorderBrush
        {
            get
            {
                return (Brush)GetValue(GroupBorderBrushProperty);
            }

            set
            {
                SetValue(GroupBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents background brush of groups.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Default value is transparent brush.
        /// </value>
        public Brush GroupBackground
        {
            get
            {
                return (Brush)GetValue(GroupBackgroundProperty);
            }

            set
            {
                SetValue(GroupBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents foreground brush of groups.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Default value is black brush.
        /// </value>
        public Brush GroupForeground
        {
            get
            {
                return (Brush)GetValue(GroupForegroundProperty);
            }

            set
            {
                SetValue(GroupForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents background brush of group when mouse is over that group.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Default value is transparent brush.
        /// </value>
        public Brush GroupMouseOverBackground
        {
            get
            {
                return (Brush)GetValue(GroupMouseOverBackgroundProperty);
            }

            set
            {
                SetValue(GroupMouseOverBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents foreground brush of group when mouse is over that group.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Default value is black brush.
        /// </value>
        public Brush GroupMouseOverForeground
        {
            get
            {
                return (Brush)GetValue(GroupMouseOverForegroundProperty);
            }

            set
            {
                SetValue(GroupMouseOverForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents background brush of group when it is selected.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Default value is transparent brush.
        /// </value>
        public Brush GroupSelectedBackground
        {
            get
            {
                return (Brush)GetValue(GroupSelectedBackgroundProperty);
            }

            set
            {
                SetValue(GroupSelectedBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents foreground brush of group when it is selected.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Default value is black brush.
        /// </value>
        public Brush GroupSelectedForeground
        {
            get
            {
                return (Brush)GetValue(GroupSelectedForegroundProperty);
            }

            set
            {
                SetValue(GroupSelectedForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents padding for groups. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>        
        /// </value>
        public Thickness GroupPadding
        {
            get
            {
                return (Thickness)GetValue(GroupPaddingProperty);
            }

            set
            {
                SetValue(GroupPaddingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents margins of groups. This is dependency properties.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// <para/>
        /// Default value is 0.
        /// </value> 
        public Thickness GroupMargin
        {
            get
            {
                return (Thickness)GetValue(GroupMarginProperty);
            }

            set
            {
                SetValue(GroupMarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents border brush of group when mouse is over that group.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Default value is transparent brush.
        /// </value>
        public Brush GroupMouseOverBorderBrush
        {
            get
            {
                return (Brush)GetValue(GroupMouseOverBorderBrushProperty);
            }

            set
            {
                SetValue(GroupMouseOverBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents border brush of group when it is selected.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Default value is transparent brush.
        /// </value>
        public Brush GroupSelectedBorderBrush
        {
            get
            {
                return (Brush)GetValue(GroupSelectedBorderBrushProperty);
            }

            set
            {
                SetValue(GroupSelectedBorderBrushProperty, value);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Raises <see cref="GroupCornerRadiusChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnGroupCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GroupCornerRadiusChanged != null)
            {
                GroupCornerRadiusChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnGroupCornerRadiusChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGroupCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnGroupCornerRadiusChanged(e);
        }

        /// <summary>
        /// Raises <see cref="GroupBorderThicknessChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnGroupBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GroupBorderThicknessChanged != null)
            {
                GroupBorderThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnGroupBorderThicknessChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGroupBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnGroupBorderThicknessChanged(e);
        }

        /// <summary>
        /// Raises <see cref="GroupBorderBrushChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnGroupBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GroupBorderBrushChanged != null)
            {
                GroupBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnGroupBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGroupBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnGroupBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises <see cref="GroupBackgroundChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnGroupBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GroupBackgroundChanged != null)
            {
                GroupBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnGroupBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGroupBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnGroupBackgroundChanged(e);
        }

        /// <summary>
        /// Calls OnGroupForegroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGroupForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnGroupForegroundChanged(e);
        }

        /// <summary>
        /// Raises <see cref="GroupForegroundChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnGroupForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GroupForegroundChanged != null)
            {
                GroupForegroundChanged(this, e);
            }
        }

        /// <summary>
        /// Raises <see cref="GroupMouseOverBackgroundChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnGroupMouseOverBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GroupMouseOverBackgroundChanged != null)
            {
                GroupMouseOverBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnGroupMouseOverBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGroupMouseOverBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnGroupMouseOverBackgroundChanged(e);
        }

        /// <summary>
        /// Calls OnGroupMouseOverForegroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGroupMouseOverForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnGroupMouseOverForegroundChanged(e);
        }

        /// <summary>
        /// Raises <see cref="GroupMouseOverForegroundChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnGroupMouseOverForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GroupMouseOverForegroundChanged != null)
            {
                GroupMouseOverForegroundChanged(this, e);
            }
        }

        /// <summary>
        /// Raises <see cref="GroupSelectedBackgroundChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnGroupSelectedBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GroupSelectedBackgroundChanged != null)
            {
                GroupSelectedBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnGroupSelectedBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGroupSelectedBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnGroupSelectedBackgroundChanged(e);
        }

        /// <summary>
        /// Calls OnGroupSelectedForegroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGroupSelectedForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnGroupSelectedForegroundChanged(e);
        }

        /// <summary>
        /// Raises <see cref="GroupForegroundChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnGroupSelectedForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GroupSelectedForegroundChanged != null)
            {
                GroupSelectedForegroundChanged(this, e);
            }
        }

        /// <summary>
        /// Raises <see cref="GroupPaddingChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnGroupPaddingChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GroupPaddingChanged != null)
            {
                GroupPaddingChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnGroupPaddingChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGroupPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnGroupPaddingChanged(e);
        }

        /// <summary>
        /// Raises <see cref="GroupMarginChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnGroupMarginChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GroupMarginChanged != null)
            {
                GroupMarginChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnGroupMarginChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGroupMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnGroupMarginChanged(e);
        }

        /// <summary>
        /// Raises <see cref="GroupMouseOverBorderBrushChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnGroupMouseOverBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GroupMouseOverBorderBrushChanged != null)
            {
                GroupMouseOverBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnGroupMouseOverBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGroupMouseOverBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnGroupMouseOverBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises <see cref="GroupSelectedBorderBrushChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnGroupSelectedBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GroupSelectedBorderBrushChanged != null)
            {
                GroupSelectedBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnGroupSelectedBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGroupSelectedBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnGroupSelectedBorderBrushChanged(e);
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="GroupCornerRadius"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback GroupCornerRadiusChanged;

        /// <summary>
        /// Event that is raised when <see cref="GroupBorderThickness"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback GroupBorderThicknessChanged;

        /// <summary>
        /// Event that is raised when <see cref="GroupBorderBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback GroupBorderBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="GroupBackground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback GroupBackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="GroupForeground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback GroupForegroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="GroupMouseOverBackground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback GroupMouseOverBackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="GroupMouseOverForeground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback GroupMouseOverForegroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="GroupSelectedBackground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback GroupSelectedBackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="GroupSelectedForeground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback GroupSelectedForegroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="GroupPadding"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback GroupPaddingChanged;

        /// <summary>
        /// Event that is raised when <see cref="GroupMargin"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback GroupMarginChanged;

        /// <summary>
        /// Event that is raised when <see cref="GroupMouseOverBorderBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback GroupMouseOverBorderBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="GroupSelectedBorderBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback GroupSelectedBorderBrushChanged;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies <see cref="GroupCornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupCornerRadiusProperty =
            DependencyProperty.Register("GroupCornerRadius", typeof(double), typeof(Gallery), new FrameworkPropertyMetadata(3d, new PropertyChangedCallback(OnGroupCornerRadiusChanged)));

        /// <summary>
        /// Identifies <see cref="GroupBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupBorderThicknessProperty =
            DependencyProperty.Register("GroupBorderThickness", typeof(double), typeof(Gallery), new FrameworkPropertyMetadata(1d, new PropertyChangedCallback(OnGroupBorderThicknessChanged)));

        /// <summary>
        /// Identifies <see cref="GroupBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupBorderBrushProperty =
            DependencyProperty.Register("GroupBorderBrush", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnGroupBorderBrushChanged)));

        /// <summary>
        /// Identifies <see cref="GroupBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupBackgroundProperty =
            DependencyProperty.Register("GroupBackground", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnGroupBackgroundChanged)));

        /// <summary>
        /// Identifies <see cref="GroupForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupForegroundProperty =
            DependencyProperty.Register("GroupForeground", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnGroupForegroundChanged)));

        /// <summary>
        /// Identifies <see cref="GroupMouseOverBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupMouseOverBackgroundProperty =
            DependencyProperty.Register("GroupMouseOverBackground", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnGroupMouseOverBackgroundChanged)));

        /// <summary>
        /// Identifies <see cref="GroupMouseOverForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupMouseOverForegroundProperty =
            DependencyProperty.Register("GroupMouseOverForeground", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnGroupMouseOverForegroundChanged)));

        /// <summary>
        /// Identifies <see cref="GroupSelectedBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupSelectedBackgroundProperty =
            DependencyProperty.Register("GroupSelectedBackground", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnGroupSelectedBackgroundChanged)));

        /// <summary>
        /// Identifies <see cref="GroupSelectedForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupSelectedForegroundProperty =
            DependencyProperty.Register("GroupSelectedForeground", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnGroupSelectedForegroundChanged)));

        /// <summary>
        /// Identifies <see cref="GroupPadding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupPaddingProperty =
            DependencyProperty.Register("GroupPadding", typeof(Thickness), typeof(Gallery), new FrameworkPropertyMetadata(new Thickness(7.5, 4, 7.5, 4), new PropertyChangedCallback(OnGroupPaddingChanged)));

        /// <summary>
        /// Identifies <see cref="GroupMargin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupMarginProperty =
            DependencyProperty.Register("GroupMargin", typeof(Thickness), typeof(Gallery), new FrameworkPropertyMetadata(new Thickness(0), new PropertyChangedCallback(OnGroupMarginChanged)));

        /// <summary>
        /// Identifies <see cref="GroupMouseOverBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupMouseOverBorderBrushProperty =
            DependencyProperty.Register("GroupMouseOverBorderBrush", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(Brushes.LightGray, new PropertyChangedCallback(OnGroupMouseOverBorderBrushChanged)));
      
        /// <summary>
        /// Identifies <see cref="GroupSelectedBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupSelectedBorderBrushProperty =
            DependencyProperty.Register("GroupSelectedBorderBrush", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(Brushes.LightGray, new PropertyChangedCallback(OnGroupSelectedBorderBrushChanged)));
        #endregion
    }
}

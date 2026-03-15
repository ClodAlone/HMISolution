// <copyright file="DocMenuItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents menu item of header in MDI mode.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DocMenuItem : MenuItem
    {
        #region Events
        /// <summary>
        /// Event that is raised when IconBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback IconBrushChanged;
        
        /// <summary>
        /// Event that is raised when ActiveIconBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback ActiveIconBrushChanged;
        
        /// <summary>
        /// Event that is raised when DisableIconBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback DisableIconBrushChanged;
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes static members of the <see cref="DocMenuItem"/> class.
        /// </summary>
        static DocMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocMenuItem), new FrameworkPropertyMetadata(typeof(DocMenuItem)));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the value of the IconBrush dependency property.
        /// </summary>
        public DrawingBrush IconBrush
        {
            get
            {
                return (DrawingBrush)GetValue(IconBrushProperty);
            }

            set
            {
                SetValue(IconBrushProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the value of the ActiveIconBrush dependency property.
        /// </summary>
        public DrawingBrush ActiveIconBrush
        {
            get
            {
                return (DrawingBrush)GetValue(ActiveIconBrushProperty);
            }

            set
            {
                SetValue(ActiveIconBrushProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the value of the DisableIconBrush dependency property.
        /// </summary>
        public DrawingBrush DisableIconBrush
        {
            get
            {
                return (DrawingBrush)GetValue(DisableIconBrushProperty);
            }

            set
            {
                SetValue(DisableIconBrushProperty, value);
            }
        }
        #endregion

        #region Implemantation
        /// <summary>
        /// Raises IconBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIconBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != IconBrushChanged)
            {
                IconBrushChanged(this, e);
            }
        }
        
        /// <summary>
        /// Raises ActiveIconBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnActiveIconBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != ActiveIconBrushChanged)
            {
                ActiveIconBrushChanged(this, e);
            }
        }
        
        /// <summary>
        /// Raises DisableIconBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnDisableIconBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != DisableIconBrushChanged)
            {
                DisableIconBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIconBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIconBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocMenuItem instance = (DocMenuItem)d;
            instance.OnIconBrushChanged(e);
        }
        
        /// <summary>
        /// Calls OnActiveIconBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnActiveIconBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocMenuItem instance = (DocMenuItem)d;
            instance.OnActiveIconBrushChanged(e);
        }
        
        /// <summary>
        /// Calls OnDisableIconBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDisableIconBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocMenuItem instance = (DocMenuItem)d;
            instance.OnDisableIconBrushChanged(e);
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Presents icon brush.
        /// </summary>
        public static readonly DependencyProperty IconBrushProperty = DependencyProperty.Register("IconBrush", typeof(DrawingBrush), typeof(DocMenuItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnIconBrushChanged)));
        
        /// <summary>
        /// Presents active icon brush.
        /// </summary>
        public static readonly DependencyProperty ActiveIconBrushProperty = DependencyProperty.Register("ActiveIconBrush", typeof(DrawingBrush), typeof(DocMenuItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnActiveIconBrushChanged)));
        
        /// <summary>
        /// Presents disable icon brush.
        /// </summary>
        public static readonly DependencyProperty DisableIconBrushProperty = DependencyProperty.Register("DisableIconBrush", typeof(DrawingBrush), typeof(DocMenuItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDisableIconBrushChanged)));
        #endregion
    }
}
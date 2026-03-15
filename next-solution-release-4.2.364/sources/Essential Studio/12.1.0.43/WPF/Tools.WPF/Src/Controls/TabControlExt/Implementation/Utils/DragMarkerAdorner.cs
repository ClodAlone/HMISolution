// <copyright file="DragMarkerAdorner.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents class for showing drag marker.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DragMarkerAdorner : TemplatedAdornerBase
    {
        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="DragMarkerAdorner"/> class.
        /// </summary>
        static DragMarkerAdorner()
        {
            EnvironmentTest.ValidateLicense(typeof(DragMarkerAdorner));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TemplatedAdornerInternalControl), new FrameworkPropertyMetadata(typeof(DragMarkerAdorner)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DragMarkerAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">The element to be adorned</param>
        public DragMarkerAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            Loaded += new RoutedEventHandler(OnDragMarkerAdornerLoaded);
        }
        #endregion

        /// <summary>
        /// Stores the adorner value.
        /// </summary>
        private int vsadorner = 42;

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether rotate text when vertical is true.
        /// </summary>
        /// <value>
        ///    <c>true</c> if [rotate text when vertical]; otherwise, <c>false</c>.
        /// </value>
        public bool RotateTextWhenVertical
        {
            get
            {
                return (bool)GetValue(RotateTextWhenVerticalProperty);
            }

            set
            {
                SetValue(RotateTextWhenVerticalProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tab strip placement.
        /// </summary>
        /// <value>The tab strip placement.</value>
        public Dock TabStripPlacement
        {
            get
            {
                return (Dock)GetValue(TabStripPlacementProperty);
            }

            set
            {
                SetValue(TabStripPlacementProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the MarkerColor dependency property.
        /// </summary>
        /// <value>The color of the marker.</value>
        public Brush MarkerColor
        {
            get
            {
                return (Brush)GetValue(MarkerColorProperty);
            }

            protected internal set
            {
                SetValue(MarkerColorPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the AdornerAlignment dependency property.
        /// </summary>
        /// <value>The adorner alignment.</value>
        public AdornerAlignment AdornerAlignment
        {
            get
            {
                return (AdornerAlignment)GetValue(AdornerAlignmentProperty);
            }

            protected internal set
            {
                SetValue(AdornerAlignmentPropertyKey, value);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when MarkerColor property is changed.
        /// </summary>
        public event PropertyChangedCallback MarkerColorChanged;

        /// <summary>
        /// Event that is raised when AdornerAlignment property is changed.
        /// </summary>
        public event PropertyChangedCallback AdornerAlignmentChanged;
        #endregion

        #region Implementation
        /// <summary>
        /// Coerces the offset.
        /// </summary>
        internal void CoerceOffset()
        {
            if (!RotateTextWhenVertical)
            {
                switch (TabStripPlacement)
                {
                    case Dock.Top:
                        OffsetX = AdornerAlignment == AdornerAlignment.SecondSide
                            ? AdornedElement.RenderSize.Width : 0;
                        break;
                    case Dock.Left:
                        OffsetY = AdornerAlignment == AdornerAlignment.SecondSide
                            ? -AdornedElement.RenderSize.Width : 0;
                        break;
                    case Dock.Right:
                        OffsetY = AdornerAlignment == AdornerAlignment.SecondSide
                            ? AdornedElement.RenderSize.Width : 0;
                        break;
                    case Dock.Bottom:
                        OffsetX = AdornerAlignment == AdornerAlignment.SecondSide
                            ? -AdornedElement.RenderSize.Width : 0;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (TabStripPlacement)
                {
                    case Dock.Left:
                        OffsetY = AdornerAlignment == AdornerAlignment.SecondSide
                            ? AdornedElement.RenderSize.Width / vsadorner : AdornedElement.RenderSize.Width / vsadorner;
                        break;
                    case Dock.Right:
                        OffsetY = AdornerAlignment == AdornerAlignment.SecondSide
                            ? AdornedElement.RenderSize.Width / vsadorner : AdornedElement.RenderSize.Width / vsadorner;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Sets the style.
        /// </summary>
        /// <param name="style">The style.</param>
        protected internal void SetStyle(Style style)
        {
            InnerControl.Style = style;
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Updates property value cache and raises MarkerColorChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnMarkerColorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MarkerColorChanged != null)
            {
                MarkerColorChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises AdornerAlignmentChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnAdornerAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AdornerAlignmentChanged != null)
            {
                AdornerAlignmentChanged(this, e);
            }
        }

        /// <summary>
        /// Handles the Loaded event of the DragMarkerAdorner control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnDragMarkerAdornerLoaded(object sender, RoutedEventArgs e)
        {
            Size size = AdornedElement.RenderSize;
            MinHeight = size.Height;
            MinWidth = size.Width;
        }

        /// <summary>
        /// Calls OnMarkerColorChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMarkerColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DragMarkerAdorner instance = (DragMarkerAdorner)d;
            instance.OnMarkerColorChanged(e);
        }

        /// <summary>
        /// Calls OnAdornerAlignmentChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAdornerAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DragMarkerAdorner instance = (DragMarkerAdorner)d;
            instance.OnAdornerAlignmentChanged(e);
        }
        #endregion

        #region Dependency Properties
        /// <summary>
        /// Identifies the RotateTextWhenVertical Dependency property
        /// </summary>
        public static readonly DependencyProperty RotateTextWhenVerticalProperty =
             DependencyProperty.Register("RotateTextWhenVertical", typeof(bool), typeof(DragMarkerAdorner), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the TabStripPlacement Dependency property
        /// </summary>
        public static readonly DependencyProperty TabStripPlacementProperty =
            DependencyProperty.Register("TabStripPlacement", typeof(Dock), typeof(DragMarkerAdorner), new FrameworkPropertyMetadata(Dock.Top));

        /// <summary>
        ///  Identifies the MarkerColorProperty Key
        /// </summary>
        protected static readonly DependencyPropertyKey MarkerColorPropertyKey =
            DependencyProperty.RegisterReadOnly("MarkerColor", typeof(Brush), typeof(DragMarkerAdorner), new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnMarkerColorChanged)));

        /// <summary>
        /// Identifies the MarkerColor Dependency property
        /// </summary>
        public static readonly DependencyProperty MarkerColorProperty = MarkerColorPropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies the AdornerAlignmentProperty Key value
        /// </summary>
        protected static readonly DependencyPropertyKey AdornerAlignmentPropertyKey =
             DependencyProperty.RegisterReadOnly("AdornerAlignment", typeof(AdornerAlignment), typeof(DragMarkerAdorner), new FrameworkPropertyMetadata(AdornerAlignment.FirstSide, new PropertyChangedCallback(OnAdornerAlignmentChanged)));

        /// <summary>
        /// Identifies the AdornerAlignment Dependency Property
        /// </summary>
        public static readonly DependencyProperty AdornerAlignmentProperty = AdornerAlignmentPropertyKey.DependencyProperty;
        #endregion
    }
}
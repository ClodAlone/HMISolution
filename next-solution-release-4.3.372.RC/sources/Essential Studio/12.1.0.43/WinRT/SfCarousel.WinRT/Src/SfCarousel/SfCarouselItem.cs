// <copyright file="SfCarouselItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
#endif

#if WINDOWS_PHONE
    namespace Syncfusion.WP.Controls.Layout
#else
    namespace Syncfusion.UI.Xaml.Controls.Layout
#endif
{
    /// <summary>
    /// Implements a selectable item inside a <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel"/>
    /// </summary>
    /// <remarks>
    /// The SfCarouselItem is a <see cref="N:Windows.UI.Xaml.Controls.ContentControl"/>.
    /// </remarks>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarouselPanel"/>
    [ClassReference(IsReviewed = false)]
    public class SfCarouselItem : ContentControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarouselItem"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfCarouselItem()
        {
            DefaultStyleKey = typeof(SfCarouselItem);
#if !WINDOWS_PHONE
            this.Loaded += SfCarouselItem_Loaded;
            this.Unloaded += SfCarouselItem_Unloaded;
#endif
        }

        #endregion

        #region Variables

        internal SfCarousel parentItemsControl;

        private PlaneProjection planeProjection;

        private Storyboard Animation;

        private ScaleTransform scaleTransform;

        private EasingDoubleKeyFrame rotationKeyFrame, offestZKeyFrame, scaleXKeyFrame, scaleYKeyFrame;

        private TimeSpan _duration;
        
        private EasingFunctionBase easingFunction;

        private DoubleAnimation xAnimation;

        internal bool isAnimating;

        private FrameworkElement LayoutRoot;

        private SfCarouselItem previousItem;

        internal DataTemplate normalcontenttemplate;

        internal bool isPointerOver = false;

        private double zOffset;

        internal double ZOffset
        {
            get
            {
                return zOffset;
            }
            set
            {
                zOffset = value;
                if (planeProjection != null)
                {
                    planeProjection.LocalOffsetZ = value;
                }
            }
        }


        private double yRotation;

        internal double YRotation
        {
            get
            {
                return yRotation;
            }
            set
            {
                yRotation = value;
                if (planeProjection != null)
                {
                    planeProjection.RotationY = value;
                }
            }
        }


        private double scale;

        internal double Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
                if (scaleTransform != null)
                {
                    scaleTransform.ScaleX = scale;
                    scaleTransform.ScaleY = scale;
                }
            }
        }

        internal double Left
        {
            get
            {
                return Canvas.GetLeft(this);
            }

            set
            {
                Canvas.SetLeft(this, value);
            }
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets a value indicating whether <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarouselItem"/> is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        /// </summary>        
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(SfCarouselItem), new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));



        internal DataTemplate SelectedTemplate
        {
            get { return (DataTemplate)GetValue(SelectedTemplateProperty); }
            set { SetValue(SelectedTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedTemplate.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SelectedTemplateProperty =
            DependencyProperty.Register("SelectedTemplate", typeof(DataTemplate), typeof(SfCarouselItem), new PropertyMetadata(null));

        
        #endregion

        #region Helper Methods

        internal void Arrange(double left, int zIndex, double rotation, double zoffset, double scale, TimeSpan duration, EasingFunctionBase ease, bool useAnimation)
        {
            if (useAnimation)
            {
                if (!isAnimating && Canvas.GetLeft(this) != left)
                    Canvas.SetLeft(this, this.Left);

                SetTransform(left, rotation, zoffset, scale);

                if (_duration != duration)
                {
                    _duration = duration;
                    duration = SetDuration(duration);
                }
                if (easingFunction != ease)
                {
                    easingFunction = ease;
                    SetEasing(ease);
                }

                isAnimating = true;
                Animation.Begin();
                Canvas.SetZIndex(this, zIndex);
            }

            this.Left = left;
        }

        private void SetEasing(EasingFunctionBase ease)
        {
            offestZKeyFrame.EasingFunction = ease;
            rotationKeyFrame.EasingFunction = ease;
            scaleYKeyFrame.EasingFunction = ease;
            scaleXKeyFrame.EasingFunction = ease;
            xAnimation.EasingFunction = ease;
        }

        private TimeSpan SetDuration(TimeSpan duration)
        {
            offestZKeyFrame.KeyTime = KeyTime.FromTimeSpan(duration);
            rotationKeyFrame.KeyTime = KeyTime.FromTimeSpan(duration);
            scaleYKeyFrame.KeyTime = KeyTime.FromTimeSpan(duration);
            scaleXKeyFrame.KeyTime = KeyTime.FromTimeSpan(duration);
            xAnimation.Duration = duration;
            return duration;
        }

        private void SetTransform(double left, double rotation, double zoffset, double scale)
        {
            rotationKeyFrame.Value = rotation;
            offestZKeyFrame.Value = zoffset;
            scaleYKeyFrame.Value = scale;
            scaleXKeyFrame.Value = scale;
            xAnimation.To = left;
        }

        void Animation_Completed(object sender, object e)
        {
            if (!isPointerOver)
            {
                ApplyContentTemplate();
            }
            isAnimating = false;
        }

        private void ApplyContentTemplate()
        {
            if (parentItemsControl != null && parentItemsControl.SelectedItemTemplate != null && SelectedTemplate != null)
            {
                if (IsSelected)
                {
                    if (parentItemsControl.ItemsSource != null && this.DataContext as object != parentItemsControl.SelectedItem)
                    {
                        this.IsSelected = false;
                        ContentTemplate = normalcontenttemplate;
                    }
                    else
                    {
                        ContentTemplate = SelectedTemplate;
                        if (parentItemsControl.SelectedItem == null)
                            parentItemsControl.SelectedItem = this;
                    }
                }
                else
                    ContentTemplate = normalcontenttemplate;
            }
        }   

        #endregion

        #region Override Methods
        /// <summary>
        /// Applies the template for<see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.SfBusyIndicator"/> control.
        /// </summary>
#if WINDOWS_PHONE
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            
            planeProjection = (PlaneProjection)GetTemplateChild("Rotator");
            LayoutRoot = (FrameworkElement)GetTemplateChild("LayoutRoot");

            Animation = (Storyboard)GetTemplateChild("Animation");
            Animation.Completed += Animation_Completed;
            rotationKeyFrame = (EasingDoubleKeyFrame)GetTemplateChild("rotationKeyFrame");
            offestZKeyFrame = (EasingDoubleKeyFrame)GetTemplateChild("offsetZKeyFrame"); 
            scaleXKeyFrame = (EasingDoubleKeyFrame)GetTemplateChild("scaleXKeyFrame");
            scaleYKeyFrame = (EasingDoubleKeyFrame)GetTemplateChild("scaleYKeyFrame");
            scaleTransform = (ScaleTransform)GetTemplateChild("scaleTransform");

            planeProjection.RotationY = yRotation;
            planeProjection.LocalOffsetZ = zOffset;

            if (Animation != null)
            {
                xAnimation = new DoubleAnimation();
                Animation.Children.Add(xAnimation);

                Storyboard.SetTarget(xAnimation, this);
#if WINDOWS_PHONE
                Storyboard.SetTargetProperty(xAnimation, new PropertyPath("(Canvas.Left)"));
            }
#else
                Storyboard.SetTargetProperty(xAnimation, "(Canvas.Left)");
            }
#endif
            if (ContentTemplate != null && ContentTemplate != SelectedTemplate)
            {
                normalcontenttemplate = ContentTemplate;
            }
            base.OnApplyTemplate();
        }
#if !WINDOWS_PHONE
        void SfCarouselItem_Unloaded(object sender, RoutedEventArgs e)
        {
            this.SizeChanged -= SfCarouselItem_SizeChanged;
        }

        void SfCarouselItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (parentItemsControl != null)
                parentItemsControl.Refresh();
            this.SizeChanged += SfCarouselItem_SizeChanged;
        }

        void SfCarouselItem_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (parentItemsControl != null)
                parentItemsControl.ArrangeSfCarouselItem(sender, e);
        }
#endif
        /// <summary>
        /// Sets the parentItemsControl when pointer is pressed for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.SfBusyIndicator"/> control.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is changed; otherwise, <c>false</c>.
        /// </value>
#if WINDOWS_PHONE
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
#else
        protected override void OnPointerPressed(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            parentItemsControl.canselect = true;
#if WINDOWS_PHONE
            base.OnMouseLeftButtonDown(e);
#else
            base.OnPointerPressed(e);
#endif
        }

        /// <summary>
        /// Sets the focus when pointer is moved for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.SfBusyIndicator"/> control.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is changed; otherwise, <c>false</c>.
        /// </value>
#if WINDOWS_PHONE
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
#else
        protected override void OnPointerMoved(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {          
#if WINDOWS_PHONE
            isPointerOver = true;
            base.OnMouseMove(e);
#else
             if (e.Pointer.IsInContact)
                isPointerOver = true;
            base.OnPointerMoved(e);
#endif
        }

        /// <summary>
        /// Sets the parentItemsControl when pointer is released for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.SfBusyIndicator"/> control.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is changed; otherwise, <c>false</c>.
        /// </value>
#if WINDOWS_PHONE
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
#else
        protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
            #endif
        {
            isPointerOver = false;
            ApplyContentTemplate();
            if (parentItemsControl.canselect)
            {
                SfCarouselItem selectitem = parentItemsControl.SelectedItem != null ? parentItemsControl.ItemContainerGenerator.ContainerFromItem(parentItemsControl.SelectedItem) as SfCarouselItem : null;
                if (previousItem != null && selectitem != null && selectitem != previousItem)
                {
                    previousItem.IsSelected = false;
                    previousItem.ContentTemplate = normalcontenttemplate;
                }
                if (Selected != null)
                {
                    Selected(this, e);
                }
                previousItem = this;
                parentItemsControl.canselect = false;
            }
            
#if WINDOWS_PHONE 
            base.OnMouseLeftButtonUp(e);
#else
            base.OnPointerReleased(e);
#endif
        }
        #endregion

        #region Callback Methods

        private static void OnIsSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfCarouselItem item = sender as SfCarouselItem;
            if (item != null && item.parentItemsControl!=null && (bool)args.NewValue)
            {
                item.parentItemsControl.SelectedIndex = item.parentItemsControl.ItemContainerGenerator.IndexFromContainer(item);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the selected item has changed.
        /// </summary>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Layout.SfCarouselItem.IsSelected"/>
        [ClassReference(IsReviewed = false)]
        public event RoutedEventHandler Selected;

        #endregion
       
    }
}

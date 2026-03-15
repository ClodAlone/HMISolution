// <copyright file="RatingItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#if !WINRT
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
#endif

#if WINDOWS_PHONE || WINDOWS_PHONE_7
using Syncfusion.WP.Primitives;
namespace Syncfusion.WP.Controls.Input

#elif WPF
using Syncfusion.Windows.Primitives;
namespace Syncfusion.Windows.Controls.Input

#elif SILVERLIGHT
using Syncfusion.Tools.Primitives;
namespace Syncfusion.Tools.Controls.Input


#else
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.UI.Xaml.Primitives;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Represent a selectable item inside the <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Input.Rating"/> control.
    /// </summary>
    /// <remarks>
    /// The RatingItem is a <see cref="N:Windows.UI.Xaml.Controls.ConentControl"/>.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class SfRatingItem : ContentControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.RatingItem"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfRatingItem()
        {
            DefaultStyleKey = typeof(SfRatingItem);
        }

        #endregion

        #region Variables

        internal SfRating Rating;

        internal Path LinearCliperPath;

        internal Path RatedPath;

        internal Grid RatingGrid;

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the value that fills the rated area with the specified solid color.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.Rating"/>
        [ClassReference(IsReviewed = false)]
        public Brush RatedFill
        {
            get { return (Brush)GetValue(RatedFillProperty); }
            set { SetValue(RatedFillProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AccentBrushBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RatedFillProperty =
            DependencyProperty.Register("RatedFill", typeof(Brush), typeof(SfRatingItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the brush that paints the rated area outline with a specified solid
        /// color.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.Rating"/>
        [ClassReference(IsReviewed = false)]
        public Brush RatedStroke
        {
            get { return (Brush)GetValue(RatedStrokeProperty); }
            set { SetValue(RatedStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RatedFillBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RatedStrokeProperty =
            DependencyProperty.Register("RatedStroke", typeof(Brush), typeof(SfRatingItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the value that specifies the outline thickness for the rated area.
        /// </summary>
        /// <value>
        /// The default value is 1.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.Rating"/>
        [ClassReference(IsReviewed = false)]
        public double RatedStrokeThickness
        {
            get { return (double)GetValue(RatedStrokeThicknessProperty); }
            set { SetValue(RatedStrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RatedStrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RatedStrokeThicknessProperty =
         DependencyProperty.Register("RatedStrokeThickness", typeof(double), typeof(SfRatingItem), new PropertyMetadata(1d));



        /// <summary>
        /// Gets or sets the value that fills the unrated area with the specified solid
        /// color.
        /// </summary>
        /// <value>
        /// The default value is <see cref="T:Windows.UI.Colors.Transparent">Colors.Transparent</see>.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.Rating"/>
        [ClassReference(IsReviewed = false)]
        public Brush UnratedFill
        {
            get { return (Brush)GetValue(UnratedFillProperty); }
            set { SetValue(UnratedFillProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for UnratedFill.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UnratedFillProperty =
            DependencyProperty.Register("UnratedFill", typeof(Brush), typeof(SfRatingItem), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Gets or sets the brush that paints the unrated area outline with a specified
        /// solid color.
        /// </summary>
        /// <value>
        /// The default value is <see cref="T:Windows.UI.Colors.Gray">Colors.Gray</see>.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.Rating"/>
        [ClassReference(IsReviewed = false)]
        public Brush UnratedStroke
        {
            get { return (Brush)GetValue(UnratedStrokeProperty); }
            set { SetValue(UnratedStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for UnratedStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UnratedStrokeProperty =
            DependencyProperty.Register("UnratedStroke", typeof(Brush), typeof(SfRatingItem), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>
        /// Gets or sets the value that specifies the outline thickness for the unrated
        /// area.
        /// </summary>
        /// <value>
        /// The default value is 1.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.Rating"/>
        [ClassReference(IsReviewed = false)]
        public double UnratedStrokeThickness
        {
            get { return (double)GetValue(UnratedStrokeThicknessProperty); }
            set { SetValue(UnratedStrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for UnratedStrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UnratedStrokeThicknessProperty =
         DependencyProperty.Register("UnratedStrokeThickness", typeof(double), typeof(SfRatingItem), new PropertyMetadata(1d));



        /// <summary>
        /// Gets or sets the value that fills the pointer over area with the specified solid
        /// color.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.Rating"/>
        [ClassReference(IsReviewed = false)]
        public Brush PointerOverFill    
        {
            get { return (Brush)GetValue(PointerOverFillProperty); }
            set { SetValue(PointerOverFillProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for PointerOverFill.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PointerOverFillProperty =
            DependencyProperty.Register("PointerOverFill", typeof(Brush), typeof(SfRatingItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the brush that paints the pointer over area outline with a
        /// specified solid color.
        /// </summary>
        /// <value>
        /// The default value is <see cref="T:Windows.UI.Colors.Gray">Colors.Gray</see>.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.Rating"/>
        [ClassReference(IsReviewed = false)]
        public Brush PointerOverStroke  
        {
            get { return (Brush)GetValue(PointerOverStrokeProperty); }
            set { SetValue(PointerOverStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for PointerOverStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PointerOverStrokeProperty =
            DependencyProperty.Register("PointerOverStroke", typeof(Brush), typeof(SfRatingItem), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>
        /// Gets or sets the value that specifies the outline thickness for the pointer over
        /// area.
        /// </summary>
        /// <value>
        /// The default value is 1.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.Rating"/>
        [ClassReference(IsReviewed = false)]
        public double PointerOverStrokeThickness  
        {
            get { return (double)GetValue(PointerOverStrokeThicknessProperty); }
            set { SetValue(PointerOverStrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for PointerOverStrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PointerOverStrokeThicknessProperty =
         DependencyProperty.Register("PointerOverStrokeThickness", typeof(double), typeof(SfRatingItem), new PropertyMetadata(1d));



        /// <summary>
        /// Gets or sets the internal precision.
        /// </summary>
        /// <value>The internal precision.</value>
        internal Precision InternalPrecision
        {
            get { return (Precision)GetValue(InternalPrecisionProperty); }
            set { SetValue(InternalPrecisionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for InternalPrecision.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InternalPrecisionProperty =
            DependencyProperty.Register("InternalPrecision", typeof(Precision), typeof(SfRatingItem), new PropertyMetadata(Precision.Standard));


        /// <summary>
        /// Gets or sets the internal value.
        /// </summary>
        /// <value>The internal value.</value>
        internal double InternalValue
        {
            get { return (double)GetValue(InternalValueProperty); }
            set { SetValue(InternalValueProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for InternalValue.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InternalValueProperty =
            DependencyProperty.Register("InternalValue", typeof(double), typeof(SfRatingItem), new PropertyMetadata(0.0));

        #endregion

        #region Helper Methods

        internal void UpdateValue(double xvalue)
        {
            if (Rating.Precision == Precision.Exact)
            {
                double value;
                if (RatingGrid != null)
                {
                    if (xvalue > RatingGrid.ActualWidth)
                        xvalue = RatingGrid.ActualWidth;
                    if (xvalue < 0)
                        xvalue = 0;
                    value = xvalue / RatingGrid.ActualWidth;
                }
                else
                    value = xvalue / this.ActualWidth;
             
                int index = this.Rating.Items.IndexOf(this);
                if (index == -1)
                    index = this.Rating.ItemContainerGenerator.IndexFromContainer(this);
                this.Rating.PreviewValue = Math.Round((double)(value + index),Rating.AutoToolTipPrecision);
                if (!Rating.IsReadOnly)
                    this.Rating.UpdateRatedState(value + index);

            }
            else if (Rating.Precision == Precision.Half)
            {
                int index = this.Rating.Items.IndexOf(this);
                if (index == -1)
                    index = this.Rating.ItemContainerGenerator.IndexFromContainer(this);
                double width;
                if (RatingGrid != null)
                    width = RatingGrid.ActualWidth;
                else
                    width = this.ActualWidth;

                if (xvalue > (width / 2))
                {
                    this.Rating.PreviewValue = 1 + index;
                    if (!Rating.IsReadOnly)
                        this.Rating.UpdateRatedState(1 + index);
                }
                else
                {
                    this.Rating.PreviewValue = 0.5 + index;
                    if (!Rating.IsReadOnly)
                        this.Rating.UpdateRatedState(0.5 + index);
                }
            }
        }

        internal void UpdateVisualState(string statename)
        {
            VisualStateManager.GoToState(this, statename, false);
        }

#if !WINRT
        private void ShowToolTip(System.Windows.Input.MouseEventArgs e)
#else
        private void ShowToolTip(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            if (null != Rating && Rating.ShowToolTip)
            {
#if !WINRT
                Point point = e.GetPosition(Rating);
#else
                Point point = e.GetCurrentPoint(Rating).Position;
#endif
                if (Rating.toolTip != null)
                {
                    Rating.toolTip.IsOpen = true;
                    if (Rating.popupBorder.ActualWidth == 0.0)
                    {
                        Rating.popupBorder.UpdateLayout();
                    }
                    Rating.toolTip.HorizontalOffset = point.X - Rating.popupBorder.ActualWidth / 2;
                }
#if !WINRT
#if !SILVERLIGHT && !WINDOWS_PHONE && !WINDOWS_PHONE_7
                GeneralTransform RatingTransform = Rating.TransformToVisual((UIElement)Application.Current.MainWindow.Content);
#else
                GeneralTransform RatingTransform = Rating.TransformToVisual((UIElement)Application.Current.RootVisual);
#endif
                Point point1 = RatingTransform.Transform(new Point());
                Rect rect = new Rect(point1.X, point1.Y, Rating.ActualWidth, Rating.ActualHeight);
#if WPF
                var windowBounds = Application.Current.MainWindow.RestoreBounds;
#endif
#if SILVERLIGHT|| WINDOWS_PHONE || WINDOWS_PHONE_7
                var windowBounds = Application.Current.Host.Content.ActualHeight;
#endif
#else
                GeneralTransform RatingTransform = Rating.TransformToVisual(Window.Current.Content);
                Point point1 = RatingTransform.TransformPoint(new Point());
                Rect rect = new Rect(point1.X, point1.Y, Rating.ActualWidth, Rating.ActualHeight);
                var windowBounds = Window.Current.CoreWindow.Bounds;
#endif
#if SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                if (rect.Top - 50 > windowBounds)
#else
                if (rect.Top - 50 > windowBounds.Top)
#endif
                {
#if WPF
                    Rating.toolTip.Placement=PlacementMode.Bottom;
                    Rating.toolTip.VerticalOffset = 5;
#elif WINDOWS_PHONE || WINDOWS_PHONE_7
                    Rating.toolTip.VerticalOffset = 5;
#else
                    Rating.toolTip.VerticalOffset = -65;
#endif
                }
                else
                {
#if WPF
                    Rating.toolTip.VerticalOffset = 5;
                    Rating.toolTip.Placement = PlacementMode.Top;
#elif WINDOWS_PHONE || WINDOWS_PHONE_7
                    Rating.toolTip.VerticalOffset = -5;
#else
                    Rating.toolTip.VerticalOffset = 65;
#endif
                }
            }
        }

        #endregion

        #region Override Methods
        /// <summary>
        /// Occurs when the pointer is moved
        /// </summary>
        /// <param name="e"></param>
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
#else
        protected override void OnPointerMoved(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            if (Rating != null)
            {
                Rating.previewItem = this;
                if (Rating.Precision == Precision.Exact || Rating.Precision == Precision.Half)
                {
                    int index = this.Rating.ItemContainerGenerator.IndexFromContainer(this);
                    for (int i = 0; i < this.Rating.Items.Count; i++)
                    {
                        SfRatingItem ratingItem = this.Rating.ItemContainerGenerator.ContainerFromIndex(i) as SfRatingItem;
                        ratingItem.LinearCliperPath.Fill = PointerOverFill;
                        ratingItem.LinearCliperPath.Stroke = PointerOverStroke;
                        ratingItem.LinearCliperPath.StrokeThickness = PointerOverStrokeThickness;
                    }
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                    this.UpdateValue(e.GetPosition(RatingGrid).X < 0 ? 0 : e.GetPosition(RatingGrid).X);
#else
                    this.UpdateValue(e.GetCurrentPoint(RatingGrid).Position.X < 0 ? 0 : e.GetCurrentPoint(RatingGrid).Position.X);
#endif
                }
                else if (Rating.Precision == Precision.Standard)
                {
                    if (!Rating.IsReadOnly)
                        Rating.UpdateMouseOverState(this);

                    if (this.Rating.ItemsSource == null)
                        this.Rating.PreviewValue = this.Rating.Items.IndexOf(this) + 1;
                    else
                        this.Rating.PreviewValue = this.Rating.ItemContainerGenerator.IndexFromContainer(this) + 1;

                }

                if (Rating.toolTip != null)
                {
                    ShowToolTip(e);                   
                }

            }
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            base.OnMouseMove(e);
#else
            base.OnPointerMoved(e);
#endif

        }

        /// <summary>
        /// Occurs when the pointer is pressed
        /// </summary>
        /// <param name="e"></param>
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
#else
        protected override void OnPointerPressed(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            if (null != Rating)
            {
                if (!Rating.IsReadOnly)
                {
                    if (Rating.Precision == Precision.Standard)
                    {
                        if (Rating.ItemsSource == null)
                        {
                            this.Rating.PreviewValue = this.Rating.Items.IndexOf(this) + 1;
                            ShowToolTip(e);
                        }
                        else
                        {
                            this.Rating.PreviewValue = this.Rating.ItemContainerGenerator.IndexFromContainer(this) + 1;
                            ShowToolTip(e);
                        }
                    }
                    else
                    {
                        if (Rating.ItemsSource == null)
                        {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                            this.UpdateValue(e.GetPosition(this).X);
#else
                            this.UpdateValue(e.GetCurrentPoint(this).Position.X);
#endif
                            if (Rating.Precision == Precision.Exact)
                                this.InternalValue = Math.Round(InternalValue, Rating.AutoToolTipPrecision);
                            this.Rating.PreviewValue = this.Rating.Items.IndexOf(this) + this.InternalValue;
                            ShowToolTip(e);
                        }
                        else
                        {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                            this.UpdateValue(e.GetPosition(this).X);
#else
                            this.UpdateValue(e.GetCurrentPoint(this).Position.X);
#endif
                            if (Rating.Precision == Precision.Exact)
                                this.InternalValue = Math.Round(InternalValue, Rating.AutoToolTipPrecision);
                            Rating.Value = this.Rating.ItemContainerGenerator.IndexFromContainer(this) + this.InternalValue;
                            this.Rating.PreviewValue = this.Rating.ItemContainerGenerator.IndexFromContainer(this) + this.InternalValue;
                            ShowToolTip(e);
                        }
                    }
                }
            }          
        }

        /// <summary>
        /// Occurs when the pointer is released
        /// </summary>
        /// <param name="e"></param>
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
#else
        protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            if (null != Rating)
            {
                if (!Rating.IsReadOnly)
                {
                    if (Rating.Precision == Precision.Standard)
                    {
                        if (Rating.ItemsSource == null)
                        {
                            Rating.Value = this.Rating.Items.IndexOf(this) + 1;
                        }
                        else
                        {
                            Rating.Value = this.Rating.ItemContainerGenerator.IndexFromContainer(this) + 1;
                        }
                    }
                    else
                    {
                        if (Rating.ItemsSource == null)
                        {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                            this.UpdateValue(e.GetPosition(this).X);
#else
                            this.UpdateValue(e.GetCurrentPoint(this).Position.X);
#endif
                            Rating.Value = this.Rating.Items.IndexOf(this) + this.InternalValue;
                        }
                        else
                        {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                            this.UpdateValue(e.GetPosition(this).X);
#else
                            this.UpdateValue(e.GetCurrentPoint(this).Position.X);
#endif
                            Rating.Value = this.Rating.ItemContainerGenerator.IndexFromContainer(this) + this.InternalValue;
                        }
                    }
                }
            }
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            base.OnMouseLeftButtonUp(e);
#else
            base.OnPointerReleased(e);
#endif
        }

#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Occurs when the Manipulation Completed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            Rating.UpdateVisualStates();
            base.OnManipulationCompleted(e);
        }
#endif

        /// <summary>
        /// Occurs when the pointer exited
        /// </summary>
        /// <param name="e"></param>
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
#else
        protected override void OnPointerExited(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            if (Rating != null)
            {
#if  WINDOWS_PHONE || WINDOWS_PHONE_7
                if (!Rating.IsReadOnly)
                {
                    if (Rating.Precision == Precision.Standard)
                    {
                        if (Rating.ItemsSource == null)
                        {
                            Rating.Value = this.Rating.Items.IndexOf(this) + 1;
                        }
                        else
                        {
                            Rating.Value = this.Rating.ItemContainerGenerator.IndexFromContainer(this) + 1;
                        }
                    }
                    else
                        Rating.Value = Rating.PreviewValue;
                }
#endif
                Rating.UpdateVisualStates();
            }

#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            base.OnMouseLeave(e);
#else
            base.OnPointerExited(e);
#endif
        }

        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.RatingItem"/> control.
        /// </summary>
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            if(Rating!=null)
            this.Rating.UpdateRatedState(Rating.Value);
            LinearCliperPath = GetTemplateChild("LinearCliperPath") as Path;
            RatedPath = GetTemplateChild("RatedPath") as Path;
            RatingGrid = GetTemplateChild("PART_RatingGrid") as Grid;
            base.OnApplyTemplate();
        }

        #endregion
    }
}

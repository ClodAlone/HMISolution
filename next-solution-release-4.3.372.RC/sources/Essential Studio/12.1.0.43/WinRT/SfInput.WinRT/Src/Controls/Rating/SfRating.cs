// <copyright file="Rating.cs" company="Syncfusion">
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
using System.Windows.Media;
using System.Windows.Input;
#endif

#if WINDOWS_PHONE || WINDOWS_PHONE_7
using Syncfusion.WP.Primitives;
namespace Syncfusion.WP.Controls.Input

#elif WPF
using Syncfusion.Windows.Primitives;
using Syncfusion.Licensing;
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
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Syncfusion.UI.Xaml.Primitives;

namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Rating is a <see cref="N:Windows.UI.Xaml.Controls.ItemsControl">ItemsControl</see> which
    /// allows the user to rate the items from the available predefined number of items.
    /// </summary>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.RatingItem"/>
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Rated", GroupName = "CommonStates")]   

    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(SfRatingItem))] 
    [ClassReference(IsReviewed = false)]
    public class SfRating : ItemsControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.Rating"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfRating()
        {
#if WPF
            if (EnvironmentTestInput.IsSecurityGranted)
            {
                EnvironmentTestInput.StartValidateLicense(typeof(SfRating));
            }
#endif
            DefaultStyleKey = typeof(SfRating);
#if !WPF&& !SILVERLIGHT && !WINDOWS_PHONE && !WINDOWS_PHONE_7
            this.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.TranslateX;
#endif
        }

        #endregion

        #region Variables

        private Border host;

        internal Popup toolTip;

        internal Grid toopTipGrid;

        internal SfRatingItem previewItem;

        internal ContentPresenter popupBorder;

        internal bool isPointerPressed;

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets a <see cref="T:Windows.UI.Xaml.CornerRadius"/> that represents the
        /// degree to which the corners of a <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.RatingItem">RatingItem&apos;s</see> are
        /// rounded.
        /// </summary>
        /// <value>
        /// The corner radius.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.RatingItem"/>
        [ClassReference(IsReviewed = false)]
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(SfRating), new PropertyMetadata(null));

#if SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets a value to enable the user to apply style for the ItemContainer.
        /// <see cref="T:Syncfusion.WP.Controls.Input.SfRating"/>
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemContainerStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(SfRating), new PropertyMetadata(null));        
#endif

        /// <summary>
        /// Gets or sets the <see cref="T:Syncfusion.UI.Xaml.Primitives.Precision"/> to rate
        /// the items.
        /// </summary>
        /// <value>
        /// The default value is <see
        /// cref="F:Syncfusion.UI.Xaml.Primitives.Precision.Standard">Precision.Standard</see>.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.RatingItem"/>
        [ClassReference(IsReviewed = false)]
#if WPF
        [CLSCompliant(false)]
#endif
        public Precision Precision
        {
            get { return (Precision)GetValue(PrecisionProperty); }
            set { SetValue(PrecisionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Precision.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PrecisionProperty =
            DependencyProperty.Register("Precision", typeof(Precision), typeof(SfRating), new PropertyMetadata(Precision.Standard, new PropertyChangedCallback(OnPrecisionChanged)));


        /// <summary>
        /// Gets or sets the value that determine number of decimal shown in the tool tip
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.Rating"/>.
        /// </summary>
        /// <value>
        /// The default value is 1.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.RatingItem"/>
        [ClassReference(IsReviewed = false)]
        public int AutoToolTipPrecision
        {
            get { return (int)GetValue(AutoToolTipPrecisionProperty); }
            set { SetValue(AutoToolTipPrecisionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AutoToolTipPrecision.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AutoToolTipPrecisionProperty =
            DependencyProperty.Register("AutoToolTipPrecision", typeof(int), typeof(SfRating), new PropertyMetadata(1,new PropertyChangedCallback(OnAutoToolTipPrecisionChanged)));

        /// <summary>
        /// Gets or sets the value associated with the popup.
        /// </summary>
        /// <value>
        /// The default value is zero.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.RatingItem"/>
        [ClassReference(IsReviewed = false)]
        public double PreviewValue
        {
            get { return (double)GetValue(PreviewValueProperty); }
            set { SetValue(PreviewValueProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for PreviewValue.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PreviewValueProperty =
            DependencyProperty.Register("PreviewValue", typeof(double), typeof(SfRating), new PropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets a value indicating whether show or hide the tool tip.
        /// </summary>
        /// <value>
        /// <c>true</c> the tool tip is shown; otherwise, <c>false</c>. The default value is true.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.RatingItem"/>
        [ClassReference(IsReviewed = false)]
        public bool ShowToolTip
        {
            get { return (bool)GetValue(ShowToolTipProperty); }
            set { SetValue(ShowToolTipProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowToolTip.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowToolTipProperty =
            DependencyProperty.Register("ShowToolTip", typeof(bool), typeof(SfRating), new PropertyMetadata(true));


        /// <summary>
        /// Gets or sets value that determines the count of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.RatingItem"/> with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.Rating"/> control.
        /// </summary>
        /// <value>
        /// The default value is zero.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public int ItemsCount
        {
            get { return (int)GetValue(ItemsCountProperty); }
            set { SetValue(ItemsCountProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemsCount.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemsCountProperty =
            DependencyProperty.Register("ItemsCount", typeof(int), typeof(SfRating), new PropertyMetadata(0, new PropertyChangedCallback(OnItemsCountChanged)));

        /// <summary>
        /// Gets or sets a value indicating whether <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.Rating"/> is read only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is read only; otherwise, <c>false</c>. The default value is false.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.RatingItem"/>
        [ClassReference(IsReviewed = false)]
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsReadOnly.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(SfRating), new PropertyMetadata(false));


        /// <summary>
        /// Gets or sets the current value with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.Rating"/>.
        /// </summary>
        /// <value>
        /// The default value is zero.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.RatingItem"/>
        [ClassReference(IsReviewed = false)]
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(SfRating), new PropertyMetadata(0.0, new PropertyChangedCallback(OnValueChanged)));

        #endregion

        #region Callback Methods

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfRating control = sender as SfRating;
            if (control != null)
            {
                control.OnValueChanged(args);
            }
            
        }

        private void OnValueChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateRatedState(Value);
            if(ValueChanged!=null)
            {
                ValueChangedEventArgs valueArgs = new ValueChangedEventArgs() { NewValue = args.NewValue, OldValue = args.OldValue };
                this.ValueChanged(this, valueArgs);
            }
        }

          private static void OnAutoToolTipPrecisionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
          {
                if((int)args.NewValue<0)
                {
		            throw new ArgumentException("AutoToolTipPrecision must be a positive value");
                }
          }


        private static void OnPrecisionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfRating control = sender as SfRating;
            if (control != null)
            {
                for (int i = 0; i < control.Items.Count; i++)
                {
                    SfRatingItem item = control.ItemContainerGenerator.ContainerFromIndex(i) as SfRatingItem;
                    if (null != item)
                    {
                        item.InternalPrecision = (Precision)args.NewValue;
                    }
                }
                control.UpdateRatedState(control.Value);
            }
        }

        private static void OnItemsCountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfRating control = sender as SfRating;
            if (control != null)
            {
                if (control.ItemsSource == null)
                {
                    int value;
                    if ((int)args.NewValue < 0)
                        throw new ArgumentException("ItemsCount must be a positive value");
                    else
                    {
                        value = (int)args.NewValue;
                        control.OnItemCountChanged(value);
                    }
                }
            }
        }

        #endregion      

        #region Override Methods

        /// <summary>
        /// Checks if the item is a <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRatingItem"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns>
        /// <c>true</c> if it is a SfRatingItem; otherwise, <c>false</c>
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is SfRatingItem;
        }

        /// <summary>
        /// Checks if the item is a <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRatingItem"/>
        /// </summary>
        /// <returns>DependencyObject</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new SfRatingItem();
        }

        /// <summary>
        /// Arranges the container for overrided items
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            SfRatingItem ratingItem = element as SfRatingItem;
            ratingItem.Rating = this;
#if SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            if (ratingItem != null && this.ItemContainerStyle != null)
                (ratingItem as SfRatingItem).Style = this.ItemContainerStyle;
#endif
            base.PrepareContainerForItemOverride(element, item);
        }
        /// <summary>
        /// Occurs when the pointer exited
        /// </summary>
        /// <param name="e"></param>
#if WPF || SILVERLIGHT|| WINDOWS_PHONE || WINDOWS_PHONE_7
        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
#else
        protected override void OnPointerExited(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            if (null != toolTip)
            {
                toolTip.IsOpen = false;
            }
            UpdateVisualStates();
            if (isPointerPressed)
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
#if WPF || SILVERLIGHT       
              CaptureMouse();
#endif
            base.OnMouseLeave(e);
#else
                CapturePointer(e.Pointer);
            base.OnPointerExited(e);
#endif
        }

        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.Rating"/> control.
        /// </summary>
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            Populate();
            host = GetTemplateChild("PART_Host") as Border;
            toolTip = GetTemplateChild("PART_Popup") as Popup;
            toopTipGrid = GetTemplateChild("PART_ToolTip") as Grid;
            popupBorder = GetTemplateChild("PART_Border") as ContentPresenter;
            if (host != null)
            {
#if !WPF && !SILVERLIGHT && !WINDOWS_PHONE && !WINDOWS_PHONE_7
                host.PointerMoved += host_PointerMoved;
#endif
            }
            base.OnApplyTemplate();
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
            isPointerPressed = true;
#if WINDOWS_PHONE || WINDOWS_PHONE_7
            UpdateVisualStates();
            toolTip.IsOpen = true;
#endif
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            base.OnMouseLeftButtonDown(e);
#else
            base.OnPointerPressed(e);
#endif
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
            isPointerPressed = false;
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            ReleaseMouseCapture();
            UpdateVisualStates();
#if WINDOWS_PHONE || WINDOWS_PHONE_7
            toolTip.IsOpen = true;
#endif
            base.OnMouseLeftButtonUp(e);
#else
            ReleasePointerCapture(e.Pointer);
            UpdateVisualStates();
            base.OnPointerReleased(e);
#endif
        }
        /// <summary>
        /// Occurs when the pointer entered
        /// </summary>
        /// <param name="e"></param>
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
#else
        protected override void OnPointerEntered(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            ReleaseMouseCapture();
            base.OnMouseEnter(e);
#else
            ReleasePointerCapture(e.Pointer);
            base.OnPointerEntered(e);
#endif
        }

        /// <summary>
        /// Occurs when the manipulation started
        /// </summary>
        /// <param name="e"></param>
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        protected override void OnManipulationDelta(System.Windows.Input.ManipulationDeltaEventArgs e)
#else
        protected override void OnManipulationDelta(Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
#endif
        {
           
            if (Precision == Precision.Standard)
            {
                if (!IsReadOnly)
                    UpdateMouseOverState(previewItem);

                if (ItemsSource == null)
                    PreviewValue = Items.IndexOf(previewItem) + 1;
                else
                    PreviewValue = ItemContainerGenerator.IndexFromContainer(this) + 1;
            }

#if !(WINDOWS_PHONE || WINDOWS_PHONE_7)
            if (toolTip != null)
            {
#if WPF || SILVERLIGHT 
                toolTip.HorizontalOffset = e.ManipulationOrigin.X - (popupBorder.ActualWidth / 2);
#else
                toolTip.HorizontalOffset = e.Position.X - (popupBorder.ActualWidth / 2);
#endif
            }
#endif

            base.OnManipulationDelta(e);
        }
        #endregion

        #region Helper Methods

        private void OnItemCountChanged(int newValue)
        {
            if (newValue > 0)
            {
                int amountToAdd = newValue - this.Items.Count;
                if (amountToAdd > 0)
                {
                    for (int cnt = 0; cnt < amountToAdd; cnt++)
                    {
                        this.Items.Add(new SfRatingItem());
                    }
                }
                else if (amountToAdd < 0)
                {
                    for (int cnt = 0; cnt < Math.Abs(amountToAdd); cnt++)
                    {
                        this.Items.RemoveAt(this.Items.Count - 1);
                    }
                }
            }
            else
            {
                this.Items.Clear();
            }

        }

        private void Populate()
        {
            if (ItemsSource == null)
            {
                if (Items.Count == 0 && ItemsCount > 0)
                {
                    for (int i = 0; i < ItemsCount; i++)
                    {
                        SfRatingItem item = new SfRatingItem();
                        Items.Add(item);
                    }
                }
                UpdateRatedState(Value);
            }
        }

        internal void UpdateMouseOverState(SfRatingItem item)
        {
            for (int i = 0; i <= this.Items.Count - 1; i++)
            {
                SfRatingItem ratingItem = this.ItemContainerGenerator.ContainerFromIndex(i) as SfRatingItem;
                if (ratingItem != null)
                {
                    ratingItem.InternalPrecision = Precision;
                    if (ItemsSource == null)
                    {
                        if (i <= this.Items.IndexOf(item))
                        {
                            ratingItem.UpdateVisualState("MouseOver");
                        }
                        else
                        {
                            ratingItem.UpdateVisualState("Normal");
                        }
                    }
                    else
                    {
                        if (i <= this.ItemContainerGenerator.IndexFromContainer(item))
                        {
                            ratingItem.UpdateVisualState("MouseOver");
                        }
                        else
                        {
                            ratingItem.UpdateVisualState("Normal");
                        }
                    }
                }
            }
        }

        internal void UpdateMouseLeaveState(SfRatingItem item)
        {
            for (int i = 0; i <= this.Items.Count - 1; i++)
            {
                SfRatingItem ratingItem = this.ItemContainerGenerator.ContainerFromIndex(i) as SfRatingItem;
                if (ratingItem != null)
                {
                    if (ItemsSource == null)
                    {
                        if (i < this.Items.IndexOf(item))
                        {
                            ratingItem.UpdateVisualState("MouseOver");
                        }
                        else
                        {
                            ratingItem.UpdateVisualState("Normal");
                        }
                    }
                    else
                    {
                        if (i < this.ItemContainerGenerator.IndexFromContainer(item))
                        {
                            ratingItem.UpdateVisualState("MouseOver");
                        }
                        else
                        {
                            ratingItem.UpdateVisualState("Normal");
                        }
                    }
                }
            }
        }

        internal void UpdateRatedState(double index)
        {
            double value = index;
            if (this.Precision == Precision.Standard)
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    SfRatingItem item = this.ItemContainerGenerator.ContainerFromIndex(i) as SfRatingItem;
                    if (null != item)
                    {
                        if (i < index)
                        {
                            item.UpdateVisualState("Rated");
                        }
                        else
                        {
                            item.UpdateVisualState("Normal");
                        }
                        if (item.RatedPath != null && item.LinearCliperPath != null)
                        {
                            item.RatedPath.Visibility = Visibility.Visible;
                            item.LinearCliperPath.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
            else
            {
                double exactValue = Math.Floor(index);
                double preciseValue;
                if(this.Precision==Precision.Exact)
                {
                   preciseValue=exactValue == 0 ? value : value % exactValue;
                }
                else
                {
                    if ((exactValue == 0 || value % exactValue == 0) && index != 0.5)
                    {
                        preciseValue = 0;
                    }
                    else
                    {
                        preciseValue = 0.5;
                    }
                 }
                for (int i = 0; i < this.Items.Count; i++)
                {
                    SfRatingItem item = this.ItemContainerGenerator.ContainerFromIndex(i) as SfRatingItem;
                    if (null != item)
                    {
                        if (i == exactValue)
                        {
                            item.InternalValue = preciseValue;
                        }
                        else if (i < exactValue)
                        {
                            item.InternalValue = 1.0;
                        }
                        else
                        {
                            item.InternalValue = 0.0;
                        }
                        if (item.RatedPath != null && item.LinearCliperPath != null)
                        {
                            item.RatedPath.Visibility = Visibility.Collapsed;
                            item.LinearCliperPath.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }

        internal void UpdateVisualStates()
        {
            if (Precision == Precision.Standard)
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        SfRatingItem item = ItemContainerGenerator.ContainerFromIndex(i) as SfRatingItem;
                        if (item != null)
                        {
                            if (i < Value)
                            {
                                VisualStateManager.GoToState(item, "Rated", false);
                            }
                            else
                            {
                                VisualStateManager.GoToState(item, "Normal", false);
                            }
                        }
                    }
                }
                else
                {
                    UpdateRatedState(Value);
                }
                if (toolTip != null)
                {
                    toolTip.IsOpen = false;
                }
                for (int i = 0; i < Items.Count; i++)
                {
                    SfRatingItem ratingItem = ItemContainerGenerator.ContainerFromIndex(i) as SfRatingItem;
                    ratingItem.LinearCliperPath.Fill = ratingItem.RatedFill;
                    ratingItem.LinearCliperPath.Stroke = ratingItem.RatedStroke;
                    ratingItem.LinearCliperPath.StrokeThickness = ratingItem.RatedStrokeThickness;
                }
            
        }

#if !WPF && !SILVERLIGHT && !WINDOWS_PHONE && !WINDOWS_PHONE_7
        void host_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {

        }
#endif
        #endregion

        #region Events

        /// <summary>
        /// Occurs when the value changed with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.Rating"/> control.
        /// </summary>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.Rating.Value"/>
        [ClassReference(IsReviewed = false)]
        public event ValueChangedEventHandler ValueChanged;

        #endregion
    }

}

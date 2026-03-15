// <copyright file="SfCarousel.cs" company="Syncfusion">
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

#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using Syncfusion.WP.Primitives;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Syncfusion.UI.Xaml.Primitives;
using Windows.UI.Xaml.Input;
#endif
#if WINDOWS_PHONE
namespace Syncfusion.WP.Controls.Layout
#else
    namespace Syncfusion.UI.Xaml.Controls.Layout
#endif
{
    /// <summary>
    /// The SfCarousel control allows the user to navigate between their items with rich
    /// animated UI
    /// </summary>
    /// <remarks>
    /// SfCarousel is a <see cref="T:Syncfusion.UI.Xaml.Primitives.Selector"/> in navigator
    /// layout that renders list of items in rich UI, based the properties set with
    /// the control animate items on navigate. The item has rich customization support and
    /// animation will reflected based on the user values.
    /// </remarks>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarouselItem"/>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarouselPanel"/>
    [ClassReference(IsReviewed = false)]
    public class SfCarousel :Selector
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel"/> class.
        /// </summary>
        public SfCarousel()
        {
            DefaultStyleKey = typeof(SfCarousel);
            this.Loaded += SfCarousel_Loaded;
            this.Unloaded += SfCarousel_Unloaded;
            this.LayoutUpdated += SfCarousel_LayoutUpdated;
        }

        void SfCarousel_Unloaded(object sender, RoutedEventArgs e)
        {
            this.LayoutUpdated -= SfCarousel_LayoutUpdated;
            this.Loaded -= SfCarousel_Loaded;
            this.Unloaded -= SfCarousel_Unloaded;
        }

        void SfCarousel_LayoutUpdated(object sender, object e)
        {
            Refresh();
        }

        #endregion

        #region Variables

        internal SfCarouselPanel itemspanel;

        private FrameworkElement LayoutRoot;

        private ItemsPresenter ItemsPresenter;

        internal bool canselect = false;

        private double tapoffset;

        private const double C_DesiredDeceleration = 1000.0 * 96.0 / (1000.0 * 1000.0);

        private SfCarouselItem previousselitem;
        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the space between items.
        /// </summary>
        /// <value>
        /// The default value is 60.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel.ScaleOffset"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel.SelectedItemOffset"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel.ZOffset"/>
        [ClassReference(IsReviewed = false)]
        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Offset.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(double), typeof(SfCarousel), new PropertyMetadata(60d, new PropertyChangedCallback(RefreshSfCarouselItems)));



        /// <summary>
        /// Gets or sets the distance between the current item and other.
        /// </summary>
        /// <value>
        /// The default value is 120.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel.Offset"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel.ScaleOffset"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel.ZOffset"/>
        [ClassReference(IsReviewed = false)]
        public double SelectedItemOffset
        {
            get { return (double)GetValue(SelectedItemOffsetProperty); }
            set { SetValue(SelectedItemOffsetProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedItemOffset.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedItemOffsetProperty =
            DependencyProperty.Register("SelectedItemOffset", typeof(double), typeof(SfCarousel), new PropertyMetadata(120d, new PropertyChangedCallback(RefreshSfCarouselItems)));



        /// <summary>
        /// Gets or sets the rotation angle for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarouselItem"/>.
        /// </summary>
        /// <remarks>
        /// Rotation angel of the items.
        /// </remarks>
        /// <value>
        /// The default value is 45.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public double RotationAngle
        {
            get { return (double)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RotationAngle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngle", typeof(double), typeof(SfCarousel), new PropertyMetadata(45d, new PropertyChangedCallback(RefreshSfCarouselItems)));



        /// <summary>
        /// Gets or sets the zooming offset.
        /// </summary>
        /// <value>
        /// The default value is zero. The Zoffset should be set in between 0 and 1.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel.Offset"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel.ScaleOffset"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel.SelectedItemOffset"/>
        [ClassReference(IsReviewed = false)]
        public double ZOffset
        {
            get { return (double)GetValue(ZOffsetProperty); }
            set { SetValue(ZOffsetProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ZOffset.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ZOffsetProperty =
            DependencyProperty.Register("ZOffset", typeof(double), typeof(SfCarousel), new PropertyMetadata(0.0, new PropertyChangedCallback(RefreshSfCarouselItems)));



        /// <summary>
        /// Gets or sets the length of time for which this timeline plays, not counting
        /// repetitions.
        /// </summary>
        /// <remarks>
        /// Specify the time taken for move an item.
        /// </remarks>
        /// <value>
        /// The default value is <see cref="T:System.TimeSpan"/>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(SfCarousel), new PropertyMetadata(TimeSpan.FromMilliseconds(600)));



        /// <summary>
        /// Gets or sets the easing function applied to this animation with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel"/>.
        /// </summary>
        /// <remarks>
        /// Customize the animation effect.
        /// </remarks>
        /// <value>
        /// The default value is <see
        /// cref="N:Windows.UI.Xaml.Media.Animation.EasingFunctionBase"/>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public EasingFunctionBase EasingFunction
        {
            get { return (EasingFunctionBase)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EasingFunction.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register("EasingFunction", typeof(EasingFunctionBase), typeof(SfCarousel), new PropertyMetadata(new CubicEase()));


        /// <summary>
        /// Gets or sets the scale offset with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel"/>.
        /// </summary>
        /// <value>
        /// The default value is 0.7.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel.Offset"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel.SelectedItemOffset"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel.ZOffset"/>
        [ClassReference(IsReviewed = false)]
        public double ScaleOffset
        {
            get { return (double)GetValue(ScaleOffsetProperty); }
            set { SetValue(ScaleOffsetProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ScaleOffset.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScaleOffsetProperty =
            DependencyProperty.Register("ScaleOffset", typeof(double), typeof(SfCarousel), new PropertyMetadata(0.7d, new PropertyChangedCallback(RefreshSfCarouselItems)));


        /// <summary>
        /// Gets or sets the template for the selected item
        /// </summary>
        public DataTemplate SelectedItemTemplate
        {
            get { return (DataTemplate)GetValue(SelectedItemTemplateProperty); }
            set { SetValue(SelectedItemTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedItemTemplateProperty =
            DependencyProperty.Register("SelectedItemTemplate", typeof(DataTemplate), typeof(SfCarousel), new PropertyMetadata(null));

        #endregion

        #region Helper Methods

        void SfCarousel_Loaded(object sender, RoutedEventArgs e)
        {
            if (SelectedIndex >= 0)
            {
                Refresh();
            }
            RefreshSelectedItem();
        }

        private void RefreshSelectedItem()
        {
            var selectedItem = this.Items.OfType<SfCarouselItem>().Where(item => item.IsSelected).FirstOrDefault();
            if (selectedItem != null)
            {
                this.SelectedIndex = this.ItemContainerGenerator.IndexFromContainer(selectedItem as SfCarouselItem);
            }
        }

        internal void ArrangeSfCarouselItem(object sender, SizeChangedEventArgs e)
        {
            SfCarouselItem item = sender as SfCarouselItem;
            int index = ItemContainerGenerator.IndexFromContainer(item);

            ArrangeChild(item, index);
        }

        private void SelectSfCarouselItem(object sender, RoutedEventArgs e)
        {
            SfCarouselItem item = sender as SfCarouselItem;
            if (item == null)
                return;

            SelectedIndex = ItemContainerGenerator.IndexFromContainer(item);
            //SelectItem(item);
        }

        private void SelectItem(SfCarouselItem item)
        {
            if (item == null)
                return;

            if (previousselitem != null && previousselitem.IsSelected)
            {
                previousselitem.IsSelected = false;
                previousselitem.ContentTemplate = previousselitem.normalcontenttemplate;
                previousselitem = null;
            }

            item.IsSelected = true;

            int index = ItemContainerGenerator.IndexFromContainer(item);

            if (index >= 0)
                UpdateSelection(index);
        }

        internal void UpdateSelection(int index)
        {
            if (Items.Count > 0)
            {
                ArrangeChildren();
            }
        }

        internal void ArrangeChildren()
        {
            foreach (var element in Items)
            {
                SfCarouselItem item = ItemContainerGenerator.ContainerFromItem(element) as SfCarouselItem;
                if (item != null)
                {
                    int index = ItemContainerGenerator.IndexFromContainer(item);
                    ArrangeChild(item, index);
                }
            }
        }

        internal void ArrangeChild(SfCarouselItem element, int index)
        {
            double centerpoint = ActualWidth / 2;
            int _index = index - SelectedIndex;
            double indexfactor = 0;

            if (_index < 0)
                indexfactor = -1;
            else if (_index > 0)
                indexfactor = 1;

            double _center = (centerpoint + ((double)_index * Offset + (SelectedItemOffset * indexfactor))) - (element.ActualWidth / 2);
            double scaleoffset = indexfactor == 0 ? 1 : ScaleOffset;
            int zindex = Items.Count - Math.Abs(_index);

            if (((_center + element.ActualWidth) < 0 || _center > ActualWidth)
                && ((element.Left + element.ActualWidth) < 0 || element.Left > ActualWidth)
                && !((_center + element.ActualWidth) < 0 && element.Left > ActualWidth)
                && !((element.Left + element.ActualWidth) < 0 && _center > ActualWidth))
            {
                element.Arrange(_center, zindex, RotationAngle * indexfactor, ZOffset * Math.Abs(indexfactor), scaleoffset, Duration, EasingFunction, false);
            }
            else
            {
                element.Arrange(_center, zindex, RotationAngle * indexfactor, ZOffset * Math.Abs(indexfactor), scaleoffset, Duration, EasingFunction, true);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Moves the selection next <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarouselItem"/>.
        /// </summary>
        /// <remarks>
        /// The MoveNext method moves the current item position, one position forward.
        /// Return value is void.
        /// </remarks>
        /// <seealso cref="M:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel.MovePrevious"/>
        [ClassReference(IsReviewed = false)]
        public void MoveNext()
        {
            if (SelectedIndex < Items.Count - 1)
            {
                SelectedIndex++;
            }
        }
        /// <summary>
        /// Moves the selection to previous <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarouselItem"/>.
        /// </summary>
        /// <remarks>
        /// The MovePrevious method moves the current item position, one position backward.
        /// Return value is void.
        /// </remarks>
        /// <seealso cref="M:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel.MoveNext"/>
        [ClassReference(IsReviewed = false)]
        public void MovePrevious()
        {
            if (SelectedIndex > 0)
            {
                SelectedIndex--;
            }
        }

        /// <summary>
        /// Refresh the layout and rearrange the children with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel"/>.
        /// </summary>
        /// <remarks>
        /// Return value is void.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public void Refresh()
        {
            ArrangeChildren();
        }


        #endregion

        #region Override Methods
        /// <summary>
        /// Checks whether the item is a <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarouselItem"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is SfCarouselItem;
        }

        /// <summary>
        /// Gets a <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarouselItem"/>  for override.
        /// </summary>
        /// <returns>
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarouselItem"/>
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new SfCarouselItem();
        }

        /// <summary>
        /// Sets the properties for the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarouselItem"/> item.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            SfCarouselItem SfCarouselitem = element as SfCarouselItem;

            if (SfCarouselitem != null)
            {
                SfCarouselitem.parentItemsControl = this;
                SfCarouselitem.SelectedTemplate = SelectedItemTemplate;
                SfCarouselitem.Selected += SelectSfCarouselItem;
#if WINDOWS_PHONE
                SfCarouselitem.SizeChanged += ArrangeSfCarouselItem;
#endif
            }

            base.PrepareContainerForItemOverride(element, item);
        }
        /// <summary>
        /// Sets the offset values
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE
        protected override void OnManipulationStarted(System.Windows.Input.ManipulationStartedEventArgs e)
#else
        protected override void OnManipulationStarted(Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
#endif
        {
#if !WINDOWS_PHONE
            canselect = false;
#endif
            tapoffset = 0.0;
            base.OnManipulationStarted(e);
        }

        /// <summary>
        /// Sets the selected item as the next or previous carousal item <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarouselItem"/> based on the mouse movement
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE
        protected override void OnMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                MovePrevious();
            }
            else if (e.Delta < 0)
            {
                MoveNext();
            }
            base.OnMouseWheel(e);
        }
#else
        protected override void OnPointerWheelChanged(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.MouseWheelDelta > 0)
            {
                MovePrevious();
            }
            else if (e.GetCurrentPoint(this).Properties.MouseWheelDelta < 0)
            {
                MoveNext();
            }
            SfCarouselItem selectitem = SelectedItem != null ? ItemContainerGenerator.ContainerFromItem(SelectedItem) as SfCarouselItem : null;
            if (selectitem != null)
                selectitem.isPointerOver = false;
            base.OnPointerWheelChanged(e);
        }
#endif
        /// <summary>
        /// Sets the selected item as the next or previous carousal item <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarouselItem"/> based on the keys pressed.
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE
        protected override void OnKeyDown(KeyEventArgs e)
        {

            if (e.Key == Key.Left || e.Key == Key.Down)
            {
                MovePrevious();
            }
            else if (e.Key == Key.Right || e.Key == Key.Up)
            {
                MoveNext();
            }
            else if (e.Key == Key.PageUp || e.Key == Key.Home)
            {
                SelectedIndex = 0;
            }
            else if (e.Key == Key.PageDown || e.Key == Key.End)
            {
                SelectedIndex = this.Items.Count - 1;
            }
            base.OnKeyDown(e);

        }
#else
            protected override void OnKeyDown(KeyRoutedEventArgs e)

        {
            if (e.Key == Windows.System.VirtualKey.Left || e.Key == Windows.System.VirtualKey.Down)
            {
                MovePrevious();
            }
            else if (e.Key == Windows.System.VirtualKey.Right || e.Key == Windows.System.VirtualKey.Up)
            {
                MoveNext();
            }
            else if (e.Key == Windows.System.VirtualKey.PageUp || e.Key == Windows.System.VirtualKey.Home)
            {
                SelectedIndex = 0;
            }
            else if (e.Key == Windows.System.VirtualKey.PageDown || e.Key == Windows.System.VirtualKey.End)
            {
                SelectedIndex = this.Items.Count - 1;
            }
            base.OnKeyDown(e);
        }

        /// <summary>
        /// Sets the translation information associated with the event.
        /// </summary>
        /// <param name="e"></param>
            protected override void OnManipulationInertiaStarting(Windows.UI.Xaml.Input.ManipulationInertiaStartingRoutedEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = C_DesiredDeceleration;
            base.OnManipulationInertiaStarting(e);
        }

#endif

        /// <summary>
        /// Sets the selected index based on the tapoffset
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE
        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            canselect = false;
            tapoffset += e.DeltaManipulation.Translation.X;
#else
            protected override void OnManipulationDelta(Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
            {
                tapoffset += e.Delta.Translation.X;
#endif
            if (tapoffset > 100)
            {
                MovePrevious();
                tapoffset = 0.0;
            }
            else if (tapoffset < -100)
            {
                MoveNext();
                tapoffset = 0.0;
            }
            base.OnManipulationDelta(e);
        }

        /// <summary>
        /// Sets the selected item as <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarouselItem"/>
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE
        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
#else
        protected override void OnManipulationCompleted(Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
#endif
        {
            canselect = true;
            if (SelectedItem != null)
            {
                SfCarouselItem selitem = ItemContainerGenerator.ContainerFromItem(SelectedItem) as SfCarouselItem;
                if (selitem != null)
                    selitem.ContentTemplate = SelectedItemTemplate != null ? SelectedItemTemplate : selitem.normalcontenttemplate;
            }
            base.OnManipulationCompleted(e);
        }

        /// <summary>
        /// Sets the selected item when the selection has been changed by the user
        /// </summary>
        /// <param name="args"></param>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarouselItem"/>
        protected override void OnSelectionChanged(DependencyPropertyChangedEventArgs args)
        {
            if (SelectedIndex >= 0)
            {
                SfCarouselItem item = ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as SfCarouselItem;
                previousselitem = args.OldValue as SfCarouselItem;
                if ((args.NewValue as SfCarouselItem) != null)
                    (args.NewValue as SfCarouselItem).IsSelected = true;
                SelectItem(item);
                base.OnSelectionChanged(args);
            }
        }

        /// <summary>
        /// Sets the size for the arranging behaviour
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns> The size </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = base.ArrangeOverride(finalSize);
            RectangleGeometry visibleArea = new RectangleGeometry();
            Rect clip = new Rect(0, 0, ItemsPresenter.ActualWidth, ItemsPresenter.ActualHeight);
            foreach (var item in Items)
            {
                SfCarouselItem coverItem = ItemContainerGenerator.ContainerFromItem(item) as SfCarouselItem;
                coverItem.MaxHeight = ItemsPresenter.ActualHeight;
            }
            visibleArea.Rect = clip;
            ItemsPresenter.Clip = visibleArea;

            double m = ItemsPresenter.ActualWidth / 2;

            for (int index = 0; index < Items.Count; index++)
            {
                SfCarouselItem item = ItemContainerGenerator.ContainerFromIndex(index) as SfCarouselItem;
                int b = index - SelectedIndex;
                double mu = 0;
                if (b < 0)
                    mu = -1;
                else if (b > 0)
                    mu = 1;
                double x = (m + ((double)b * Offset + (SelectedItemOffset * mu))) - (item.ActualWidth / 2);

                double s = mu == 0 ? 1 : ScaleOffset;

                int zindex = Items.Count - Math.Abs(b);

                item.Left = x;
                item.YRotation = RotationAngle * mu;
                item.ZOffset = ZOffset * Math.Abs(mu);
                item.Scale = s;
            }

            return size;
        }

        /// <summary>
        /// Gets or sets the Framework elements on applying the templates.
        /// </summary>
#if WINDOWS_PHONE
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            LayoutRoot = (FrameworkElement)GetTemplateChild("LayoutRoot");
            ItemsPresenter = (ItemsPresenter)GetTemplateChild("ItemsPresenter");

        }


        #endregion

        #region Callback Methods

        private static void RefreshSfCarouselItems(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SfCarousel control = sender as SfCarousel;
#if WINDOWS_PHONE
            if (control != null && !DesignerProperties.IsInDesignTool)
            {
                control.Dispatcher.BeginInvoke(() =>
                {
                    control.Refresh();
                });
#else
            if (control != null && !Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                control.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, (() =>
                {
                    control.Refresh();
                })).AsTask();
#endif
            }
        }

        #endregion
    }
}

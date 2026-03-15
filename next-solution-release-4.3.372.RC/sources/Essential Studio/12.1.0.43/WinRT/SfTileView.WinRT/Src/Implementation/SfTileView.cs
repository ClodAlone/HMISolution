// <copyright file="TileView.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
#if !WINDOWS_PHONE_7
using System.Threading.Tasks;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
#if !WINDOWS_PHONE_7
using Windows.UI.Core;
#endif
using System.Windows.Input;
using Microsoft.Xna.Framework.Input;
using Syncfusion.WP.Primitives;
#else
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Syncfusion.UI.Xaml.Primitives;
#endif

#if WINDOWS_PHONE||WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls.Layout
#else
namespace Syncfusion.UI.Xaml.Controls.Layout
#endif
{
    /// <summary>
    /// Represents a TileView that allows the user to layout the items as Tiles.
    /// </summary>
    /// <remarks>
    /// Tile View is the <see cref="T:Syncfusion.UI.Xaml.Primitives.Selector"/> acts as
    /// a container that can hold a set of <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/>&apos;s which can host
    /// rich information.TileViewItems can be render in the following states
    /// Normal,Maximized,Minimized.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public  class SfTileView:Selector,IDisposable 
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileView"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfTileView()
        {
            DefaultStyleKey = typeof(SfTileView);
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("ItemsSource");
            this.SetBinding(SfTileView.InternalItemsSourceProperty, binding);
            this.Loaded += TileView_Loaded;
            this.Unloaded += TileView_Unloaded;
            minimizeButtonTimer = new DispatcherTimer();
            minimizeButtonTimer.Interval = new TimeSpan(0, 0, 0, 0, C_MinimizeButtonShowTime);
            minimizeButtonTimer.Tick += timer_Tick;
#if !WINDOWS_PHONE && !WINDOWS_PHONE_7
            popupTimer = new DispatcherTimer();
            popupTimer.Interval = TimeSpan.FromSeconds(0.5);
            popupTimer.Tick += popupTimer_Tick;

            dragTimer = new DispatcherTimer();
            dragTimer.Interval = TimeSpan.FromSeconds(0.5);
            dragTimer.Tick += dragTimer_Tick;
#endif
        }
#if !WINDOWS_PHONE && !WINDOWS_PHONE_7
        void dragTimer_Tick(object sender, object e)
        {
            IsPointerPressed = true;
            dragTimer.Stop();
        }
#endif

        void TileView_Loaded(object sender, RoutedEventArgs e)
        {
            if (ItemsSource != null && ItemsSource is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)ItemsSource).CollectionChanged += TileView_CollectionChanged;
#if !WINDOWS_PHONE && !WINDOWS_PHONE_7
                if (AllowDragDrop && PART_Items!=null)
                {
                    PART_Items.scrollViewer.GotFocus += scrollViewer_GotFocus;
                    PART_Items.scrollViewer.PointerExited += scrollViewer_PointerExited;
                    PART_Items.scrollViewer.LostFocus += scrollViewer_LostFocus;
                    this.PointerCaptureLost += SfTileView_PointerCaptureLost;
                    this.PointerReleased += SfTileView_PointerReleased;
                    this.PointerExited += SfTileView_PointerExited;

                    this.PointerMoved += Content_PointerMoved;
                    Window.Current.Content.PointerReleased += Content_PointerReleased;
                
                }
#endif
            }
            if (MaximizedItemHeight != 0)
                maxItemHeightSet = true;
            if (MaximizedItemWidth != 0)
                maxItemWidthSet = true;
           UpdateSelection();
           this.IsEnabledChanged += SfTileView_IsEnabledChanged;
           if (!IsEnabled)
               VisualStateManager.GoToState(this, "Disabled", true);
           else
               VisualStateManager.GoToState(this, "Normal", true);
          
        }

        void SfTileView_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsEnabled)
                VisualStateManager.GoToState(this, "Disabled", true);
            else
                VisualStateManager.GoToState(this, "Normal", true);   
        }
#if !WINDOWS_PHONE && !WINDOWS_PHONE_7

        void SfTileView_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            CancelDragging();
        }

        void scrollViewer_LostFocus(object sender, RoutedEventArgs e)
        {
            PART_Items.itemsPresenter.ManipulationMode = ManipulationModes.All;
        }

        void scrollViewer_GotFocus(object sender, RoutedEventArgs e)
        {
            ChangeMinimizedItemsOrientation();
        }

        void SfTileView_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(this).PointerDevice.PointerDeviceType == PointerDeviceType.Touch && !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                popupTimer.Start();
        }

        void SfTileView_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            CancelDragging();
        }

        void Content_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            CancelDragging();
        }

        void Content_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!e.GetCurrentPoint(Window.Current.Content).Properties.IsLeftButtonPressed || !IsPointerPressed)
            {
                CancelDragging();
            }

            if (e.OriginalSource is SfTileViewItem && IsPointerPressed && e.GetCurrentPoint(Window.Current.Content).Properties.IsLeftButtonPressed)
            {
                SfTileViewItem draggingItem = e.OriginalSource as SfTileViewItem;
                int currentIndex;
                if (ItemsSource != null)
                {
                    currentIndex = MinimizedItems.IndexOf(draggingItem.DataContext);
                    MinimizedItems.Remove(previousDraggingItem.DataContext);
                    MinimizedItems.Insert(currentIndex, previousDraggingItem.DataContext);
                }
                else
                {
                    currentIndex = MinimizedItems.IndexOf(draggingItem);
                    previousDraggingItem.Opacity = 0.5;
                    MinimizedItems.Remove(previousDraggingItem);
                    MinimizedItems.Insert(currentIndex, previousDraggingItem);
                }
            }
            else if (IsPointerPressed && !e.GetCurrentPoint(Window.Current.Content).Properties.IsLeftButtonPressed)
            {
                CancelDragging();
            }
        }

        private void CancelDragging()
        {
            if (AllowDragDrop && previousDraggingItem != null && previousDraggingItem.PART_Preview != null)
            {
                previousDraggingItem.PART_Preview.IsOpen = false;
                IsPointerPressed = false;
                CloseDraggingPopup();
                previousDraggingItem.PART_Preview.ReleasePointerCaptures();
            }
        }

        void scrollViewer_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            CancelDragging();
        }

        private void popupTimer_Tick(object sender, object e)
        {
            popupTimer.Stop();
            CancelDragging();
        }
#endif
        void TileView_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= TileView_Unloaded;
            this.IsEnabledChanged -=SfTileView_IsEnabledChanged;
        }


        void TileView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        MinimizedItems.Add(item);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        if (SelectedItem == item)
                        {
                            MaximizedItem = null;
                            SelectedItem = null;
                            RestoreAll();
                        }
                        MinimizedItems.Remove(item);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                MinimizedItems.Clear();
                if (MaximizedItem != null)
                    MaximizedItem = null;
                InitializeItems();
            }

        }

        #endregion

        #region Variables



        internal object InternalItemsSource
        {
            get { return (object)GetValue(InternalItemsSourceProperty); }
            set { SetValue(InternalItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InternalItemsSource.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty InternalItemsSourceProperty =
            DependencyProperty.Register("InternalItemsSource", typeof(object), typeof(SfTileView), new PropertyMetadata(null, OnInternalItemsChanged));

        private static void OnInternalItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SfTileView view = sender as SfTileView;
            if (view != null)
            {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                view.Dispatcher.BeginInvoke(() =>
                {
                    view.InitializeItems();
                });
#else
                IAsyncAction action= view.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, view.InitializeItems);
                    if(view.DataContext==null)
                    {
                        view.MaximizedItem=null;
                        view.UpdateLayout();
                    }
#endif
            }
        }

        internal TileItemsControl PART_Items;

        private ContentControl PART_Content;

        internal SfTileViewItem previousItem;

        internal TileViewPanel tileViewPanel;

        private Grid PART_Root;

        private Grid PART_Grid;

        private Button PART_MinimizeButton;

        private const int C_MinimizeButtonShowTime = 4000;

        DispatcherTimer minimizeButtonTimer;

        private bool maxItemHeightSet = false, maxItemWidthSet = false;

#if !WINDOWS_PHONE && !WINDOWS_PHONE_7
        //To Close the popup when the mouse dragging outside the item
        internal DispatcherTimer popupTimer,dragTimer;
		
		internal SfTileViewItem previousDraggingItem;
       	
		internal bool IsPointerPressed = false;
#endif
        #endregion

        #region Dependency Properties

        /// <summary>
        /// Using a DependencyProperty as the backing store for AllowReorder.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowReorderProperty =
           DependencyProperty.Register("AllowReorder", typeof (bool), typeof (SfTileView), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value to enable the user to reorder the elements in the tileview
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfTileView"/>
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>
        /// </value>
        public bool AllowReorder
        {
            get { return (bool) GetValue(AllowReorderProperty); }
            set { SetValue(AllowReorderProperty, value); }
        }


#if !WINDOWS_PHONE && !WINDOWS_PHONE_7
        /// <summary>
        /// Determines whether Tile Items can be dragged and dropped in TileView.
        /// </summary>
        public bool AllowDragDrop
        {
            get { return (bool)GetValue(AllowDragDropProperty); }
            set { SetValue(AllowDragDropProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AllowDragDrop.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowDragDropProperty =
            DependencyProperty.Register("AllowDragDrop", typeof(bool), typeof(SfTileView), new PropertyMetadata(false));
#endif

#if WINDOWS_PHONE||WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets a value to enable the user to apply style for the ItemContainer.
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfTileView"/>
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
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(SfTileView), new PropertyMetadata(null));
#endif

        /// <summary>
        /// Gets or sets the minimized items orientation.
        /// </summary>
        /// <value>
        /// It accepts the type of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.MinimizedItemsOrientation"/>. The
        /// default value is <see
        /// cref="F:Syncfusion.UI.Xaml.Controls.Layout.MinimizedItemsOrientation.Right">MinimizedItemsOrientation.Right</see>.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItem"/>
        [ClassReference(IsReviewed = false)]
        public MinimizedItemsOrientation MinimizedItemsOrientation
        {
            get { return (MinimizedItemsOrientation)GetValue(MinimizedItemsOrientationProperty); }
            set { SetValue(MinimizedItemsOrientationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinimizedItemsOrientation.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinimizedItemsOrientationProperty =
            DependencyProperty.Register("MinimizedItemsOrientation", typeof(MinimizedItemsOrientation), typeof(SfTileView), new PropertyMetadata(MinimizedItemsOrientation.Right, new PropertyChangedCallback(OnMinimizedItemsOrientationChanged)));



        /// <summary>
        /// Gets or sets the collection of minimized <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem">TileViewItem&apos;s</see>.
        /// </summary>
        /// <value>
        /// It accepts the type of <see
        /// cref="T:System.Collections.ObjectModel.ObservableCollection`1">ObservableCollection&lt;&gt;</see>.
        /// The default value is null.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MinimizedItemsOrientation"/>
        [ClassReference(IsReviewed = false)]
        public ObservableCollection<object> MinimizedItems
        {
            get { return (ObservableCollection<object>)GetValue(MinimizedItemsProperty); }
            set { SetValue(MinimizedItemsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinimizedItems.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinimizedItemsProperty =
            DependencyProperty.Register("MinimizedItems", typeof(ObservableCollection<object>), typeof(SfTileView), new PropertyMetadata(null));




        /// <summary>
        /// Gets the current maximized <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedContentTransitions"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItemContainerStyle"/>
        /// <seealso
        /// cref="F:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItemHeightProperty"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItemTemplate"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItemWidth"/>
        [ClassReference(IsReviewed = false)]
        public SfTileViewItem MaximizedItem
        {
            get { return (SfTileViewItem)GetValue(MaximizedItemProperty); }
            internal set { SetValue(MaximizedItemProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaximizedItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaximizedItemProperty =
            DependencyProperty.Register("MaximizedItem", typeof(SfTileViewItem), typeof(SfTileView), new PropertyMetadata(null, new PropertyChangedCallback(OnMaximizedItemChanged)));



        /// <summary>
        /// Gets or sets the width of the current maximized <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/>.
        /// </summary>
        /// <value>
        /// The default value is zero.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedContentTransitions"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItemContainerStyle"/>
        /// <seealso
        /// cref="F:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItemHeightProperty"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItemTemplate"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItem"/>
        [ClassReference(IsReviewed = false)]
        public double MaximizedItemWidth
        {
            get { return (double)GetValue(MaximizedItemWidthProperty); }
            set { SetValue(MaximizedItemWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaximizedItemWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaximizedItemWidthProperty =
            DependencyProperty.Register("MaximizedItemWidth", typeof(double), typeof(SfTileView), new PropertyMetadata(0.0,new PropertyChangedCallback(OnMaximizedItemWidthChanged)));




        /// <summary>
        /// Gets or sets the style of the current maximized <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/> containers.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedContentTransitions"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItem"/>
        /// <seealso
        /// cref="F:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItemHeightProperty"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItemTemplate"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItemWidth"/>
        [ClassReference(IsReviewed = false)]
        public Style MaximizedItemContainerStyle
        {
            get { return (Style)GetValue(MaximizedItemContainerStyleProperty); }
            set { SetValue(MaximizedItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaximizedItemStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaximizedItemContainerStyleProperty =
            DependencyProperty.Register("MaximizedItemContainerStyle", typeof(Style), typeof(SfTileView), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the <see cref="N:Windows.UI.Xaml.DataTemplate"/> that is
        /// used to display the content of the maximized <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/>.
        /// </summary>
        /// <remarks>
        /// Used to specify the visualization of the minimized data objects.
        /// </remarks>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedContentTransitions"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItemContainerStyle"/>
        /// <seealso
        /// cref="F:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItemHeightProperty"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItem"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItemWidth"/>
        [ClassReference(IsReviewed = false)]
        public DataTemplate MaximizedItemTemplate
        {
            get { return (DataTemplate)GetValue(MaximizedItemTemplateProperty); }
            set { SetValue(MaximizedItemTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaximizedItemTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaximizedItemTemplateProperty =
            DependencyProperty.Register("MaximizedItemTemplate", typeof(DataTemplate), typeof(SfTileView), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the height of the current maximized <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/>.
        /// </summary>
        /// <value>
        /// The default value is zero.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedContentTransitions"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItemContainerStyle"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItem"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItemTemplate"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItemWidth"/>
        [ClassReference(IsReviewed = false)]
        public double MaximizedItemHeight
        {
            get { return (double)GetValue(MaximizedItemHeightProperty); }
            set { SetValue(MaximizedItemHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaximizedItemHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaximizedItemHeightProperty =
            DependencyProperty.Register("MaximizedItemHeight", typeof(double), typeof(SfTileView), new PropertyMetadata(0.0,new PropertyChangedCallback(OnMaximizedItemHeightChanged)));



        /// <summary>
        /// Gets or sets the width of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/>.
        /// </summary>
        /// <value>
        /// The default value is zero.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.ItemHeight"/>
        [ClassReference(IsReviewed = false)]
        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(SfTileView), new PropertyMetadata(0.0));


        /// <summary>
        /// Gets or sets the height of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/>.
        /// </summary>
        /// <value>
        /// The height of the item.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.ItemWidth"/>
        [ClassReference(IsReviewed = false)]
        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(SfTileView), new PropertyMetadata(0.0));


#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        /// <summary>
        /// Gets or sets the collection of transitions applied to the Maximized <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/>.
        /// </summary>
        /// <value>
        /// It will accepts the type of <see
        /// cref="N:Windows.UI.Xaml.Media.Animation.TransitionCollection"/>. The default
        /// value is null.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItem"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItemContainerStyle"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItem"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItemTemplate"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MaximizedItemWidth"/>
        [ClassReference(IsReviewed = false)]
        public TransitionCollection MaximizedContentTransitions
        {
            get { return (TransitionCollection)GetValue(MaximizedContentTransitionsProperty); }
            set { SetValue(MaximizedContentTransitionsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaximizedContentTransition.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaximizedContentTransitionsProperty =
            DependencyProperty.Register("MaximizedContentTransitions", typeof(TransitionCollection), typeof(SfTileView), new PropertyMetadata(null));
#endif


        /// <summary>
        /// Gets or sets a value that indicates the dimension by which child elements are
        /// stacked.
        /// </summary>
        /// <remarks>
        /// The orientations are Vertical,Horizontal.
        /// </remarks>
        /// <value>
        /// The Default value is <see cref="N:Windows.UI.Xaml.Controls.Orientation">Orientation</see>
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileView.MinimizedItemsOrientation"/>
        [ClassReference(IsReviewed = false)]
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SfTileView), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationChanged)));

        #endregion

        #region Helper Methods
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        void PART_Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (PART_MinimizeButton != null)
                PART_MinimizeButton.Visibility = Visibility.Visible;
            minimizeButtonTimer.Start();
        }
#else
        void PART_Grid_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                if (PART_MinimizeButton != null)
                    PART_MinimizeButton.Visibility = Visibility.Visible;
                minimizeButtonTimer.Start();
            }
        }
#endif
        void timer_Tick(object sender, object e)
        {
            minimizeButtonTimer.Stop();
            if (PART_MinimizeButton != null)
                PART_MinimizeButton.Visibility = Visibility.Collapsed;
        }

#if WINDOWS_PHONE||WINDOWS_PHONE_7
        void PART_Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (PART_MinimizeButton != null)
                PART_MinimizeButton.Visibility = Visibility.Collapsed;
        }
#else
        void PART_Grid_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (PART_MinimizeButton != null)
                PART_MinimizeButton.Visibility = Visibility.Collapsed;
        }
#endif
        void PART_MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
           RestoreAll();
        }

        private void RestoreAll()
        {
            this.SelectedIndex = -1;
            if(this.MaximizedItem != null)
                this.MaximizedItem.State = TileViewItemState.Normal;
            this.MaximizedItem = null;
            previousItem = null;
#if !WINDOWS_PHONE && !WINDOWS_PHONE_7
            previousDraggingItem = null;
#endif
            if (this.PART_MinimizeButton != null)
                PART_MinimizeButton.Visibility = Visibility.Collapsed;
        }
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        void PART_Content_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (MinimizedItems.Count != Items.Count)
            {
                ScaleTransform scale = new ScaleTransform();
                if (e.CumulativeManipulation.Scale.X > 0.33 & e.CumulativeManipulation.Scale.X < 1.0)
                {
                    scale.ScaleX = scale.ScaleY = e.CumulativeManipulation.Scale.X;
                    PART_Grid.RenderTransformOrigin = new Point(0.5, 0.5);
                    PART_Grid.RenderTransform = scale;
                }
                else if (e.CumulativeManipulation.Scale.X > .65 && e.CumulativeManipulation.Scale.Y > .65)
                {
                    scale.ScaleX = scale.ScaleY = 1;
                    e.Handled = true;
                }
            }
        }
#else
        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            if (e.Delta.Scale == 1 && (e.Container is ItemsPresenter))
            {
                if (MaximizedItem != null)
                {
                    if (MinimizedItemsOrientation == MinimizedItemsOrientation.Top || MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                        PART_Items.scrollViewer.ScrollToHorizontalOffset(PART_Items.scrollViewer.HorizontalOffset - e.Delta.Translation.X);
                    else
                        PART_Items.scrollViewer.ScrollToVerticalOffset(PART_Items.scrollViewer.VerticalOffset - e.Delta.Translation.Y);
                }
                else
                {
                    if (Orientation == Orientation.Vertical)
                        PART_Items.scrollViewer.ScrollToHorizontalOffset(PART_Items.scrollViewer.HorizontalOffset - e.Delta.Translation.X);
                    else if (Orientation == Orientation.Horizontal)
                        PART_Items.scrollViewer.ScrollToVerticalOffset(PART_Items.scrollViewer.VerticalOffset - e.Delta.Translation.Y);
                }
                dragTimer.Stop();
            }
            base.OnManipulationDelta(e);
        }

        void PART_Content_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)

        {
            if (MinimizedItems.Count != Items.Count)
            {
                ScaleTransform scale = new ScaleTransform();
                if (e.Cumulative.Expansion < 0 && e.Cumulative.Scale > 0.33)
                {
                    scale.ScaleX = scale.ScaleY = e.Cumulative.Scale;
                    PART_Grid.RenderTransformOrigin = new Point(0.5, 0.5);
                    PART_Grid.RenderTransform = scale;
                }
                else if (e.Cumulative.Scale > .65)
                {
                    scale.ScaleX = scale.ScaleY = 1;
                }
            }
        }
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        void PART_Content_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (MinimizedItems.Count != Items.Count)
            {
                ScaleTransform scale = new ScaleTransform() { ScaleX = 1, ScaleY = 1 };
                if (e.TotalManipulation.Scale.X < .75)
                {
                    this.MaximizedItem.State = TileViewItemState.Normal;
                    PART_Grid.RenderTransform = scale;
                    this.MaximizedItem = null;
                }
                else
                {
                    PART_Grid.RenderTransform = scale;
                }
            }
        }
#else
        void PART_Content_ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)

        {
            if (MinimizedItems.Count != Items.Count)
            {
                ScaleTransform scale = new ScaleTransform() { ScaleX = 1, ScaleY = 1 };
                if (e.Cumulative.Scale < .75)
                {
                    this.MaximizedItem.State = TileViewItemState.Normal;
                    SelectedIndex = -1;
                    PART_Grid.RenderTransform = scale;
                    this.MaximizedItem = null;
                }
                else
                {
                    PART_Grid.RenderTransform = scale;
                }
            }
        }
#endif
        private void InitializeItems()
        {
            if (MinimizedItems == null)
            {
                MinimizedItems = new ObservableCollection<object>();
            }
            MinimizedItems.Clear();
            foreach (var item in Items)
            {
                if (MaximizedItem==null || SelectedItem != item)
                    MinimizedItems.Add(item);
            }
        }

        internal void ChangeMinimizedItemsOrientation()
        {
            if (PART_Items != null && PART_Items.scrollViewer != null)
            {
                if (MaximizedItem != null)
                {
                    if (MinimizedItemsOrientation == MinimizedItemsOrientation.Left || MinimizedItemsOrientation == MinimizedItemsOrientation.Right)
                    {
                        if (MaximizedItemHeight == ActualHeight || MaximizedItemWidth == ActualWidth - ItemWidth - 10 || MaximizedItemWidth == ItemWidth || MaximizedItemWidth == ActualWidth || MaximizedItemHeight == ActualHeight - ItemHeight - 10 || MaximizedItemHeight == ItemHeight)
                        {
                            maxItemHeightSet = false;
                            maxItemWidthSet = false;
                        } 
                        if (!maxItemHeightSet)
                            MaximizedItemHeight = ActualHeight;
                        if (!maxItemWidthSet)
                        {
                            if (ActualWidth > ItemWidth)
                                MaximizedItemWidth = ActualWidth - ItemWidth - 10;
                            else
                                MaximizedItemWidth = ItemWidth;
                        }
                        PART_Items.scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                        PART_Items.scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    }
                    else
                    {
                        if (MaximizedItemWidth == ActualWidth || MaximizedItemHeight == ActualHeight - ItemHeight - 10 || MaximizedItemHeight == ItemHeight || MaximizedItemHeight == ActualHeight || MaximizedItemWidth == ActualWidth - ItemWidth - 10 || MaximizedItemWidth == ItemWidth)
                        {
                            maxItemHeightSet = false;
                            maxItemWidthSet = false;
                        }
                        if (!maxItemHeightSet)
                        {
                            if (ActualHeight > ItemHeight)
                                MaximizedItemHeight = ActualHeight - ItemHeight - 10;
                            else
                                MaximizedItemHeight = ItemHeight;
                        }
                        if (!maxItemWidthSet)
                            MaximizedItemWidth = ActualWidth;
                        PART_Items.scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                        PART_Items.scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    }
                }
                else
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        PART_Items.scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                        PART_Items.scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    }
                    else
                    {
                        PART_Items.scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                        PART_Items.scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    }
                }
            }
            if (PART_Root != null && PART_Grid != null && PART_Items != null)
            {
                if (MaximizedItem != null)
                {
                    if (MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                    {
                        PART_Root.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
                        PART_Root.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                        PART_Root.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Auto);
                        PART_Root.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
                        Grid.SetColumn(PART_Items, 0);
                        Grid.SetColumn(PART_Grid, 1);
                        Grid.SetRowSpan(PART_Items, 2);
                        Grid.SetRowSpan(PART_Grid, 2);
                        Grid.SetRow(PART_Items, 0);
                        Grid.SetRow(PART_Grid, 0);
                    }
                    else if (MinimizedItemsOrientation == MinimizedItemsOrientation.Right)
                    {
                        PART_Root.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                        PART_Root.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
                        PART_Root.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Auto);
                        PART_Root.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
                        Grid.SetColumn(PART_Grid, 0);
                        Grid.SetColumn(PART_Items, 1);
                        Grid.SetRowSpan(PART_Items, 2);
                        Grid.SetRowSpan(PART_Grid, 2);
                        Grid.SetRow(PART_Items, 0);
                        Grid.SetRow(PART_Grid, 0);
                    }
                    else if (MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                    {
                        PART_Root.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                        PART_Root.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
                        PART_Root.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                        PART_Root.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Auto);
                        Grid.SetColumn(PART_Items, 0);
                        Grid.SetColumn(PART_Grid, 0);
                        Grid.SetRowSpan(PART_Items, 1);
                        Grid.SetRowSpan(PART_Grid, 1);
                        Grid.SetRow(PART_Items, 1);
                        Grid.SetRow(PART_Grid, 0);
                    }
                    else if (MinimizedItemsOrientation == MinimizedItemsOrientation.Top)
                    {
                        PART_Root.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                        PART_Root.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
                        PART_Root.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Auto);
                        PART_Root.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
                        Grid.SetColumn(PART_Items, 0);
                        Grid.SetColumn(PART_Grid, 0);
                        Grid.SetRowSpan(PART_Items, 1);
                        Grid.SetRowSpan(PART_Grid, 1);
                        Grid.SetRow(PART_Items, 0);
                        Grid.SetRow(PART_Grid, 1);
                    }
                }
                else
                {
                    PART_Root.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                    PART_Root.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
                    PART_Root.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                    PART_Root.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Auto);
                    Grid.SetColumn(PART_Items, 0);
                    Grid.SetColumn(PART_Grid, 1);
                    Grid.SetRow(PART_Items, 0);
                    Grid.SetRow(PART_Grid, 0);
                }
            }
        }

     
	  private void UpdateSelection()
        {
            if (PART_Items != null)
            {
                if (SelectedIndex >= 0 && Items.Count >0)
                {
                    SfTileViewItem sfTileViewItem =
                        PART_Items.ItemContainerGenerator.ContainerFromItem(Items[SelectedIndex]) as
                        SfTileViewItem;
                    if (sfTileViewItem != null && MaximizedItem != sfTileViewItem)
                    {
                        if (previousItem != null)
                            previousItem.State = TileViewItemState.Normal;
                        previousItem = sfTileViewItem;
                        sfTileViewItem.State = TileViewItemState.Maximized;
                    }
                }
                else if (SelectedIndex < 0 )
                {
                    RestoreAll();
                }
            }
            
        }

        #endregion       

        #region Override Methods

      /// <summary>
      /// Initializes all the child elements of the <see 
      /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTileView"/> control.
      /// </summary>
#if WINDOWS_PHONE||WINDOWS_PHONE_7
      public override void OnApplyTemplate()
#else
      protected override void OnApplyTemplate()
#endif
        {
            PART_Items = GetTemplateChild("PART_Items") as TileItemsControl;
            PART_Content = GetTemplateChild("PART_Content") as ContentControl;
            PART_Root = GetTemplateChild("PART_Root") as Grid;
            PART_Grid = GetTemplateChild("PART_Grid") as Grid;
            PART_MinimizeButton = GetTemplateChild("PART_MinimizeButton") as Button;
            if (PART_MinimizeButton != null)
            {
                PART_MinimizeButton.Click += PART_MinimizeButton_Click;
            }
            if (PART_Grid != null)
            {
                PART_Grid.ManipulationDelta += PART_Content_ManipulationDelta;
                PART_Grid.ManipulationCompleted += PART_Content_ManipulationCompleted;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                //PART_Grid.MouseLeave += PART_Grid_MouseLeave;
                PART_Grid.MouseEnter += PART_Grid_MouseMove;
#else
                PART_Grid.PointerMoved += PART_Grid_PointerMoved;
                PART_Grid.PointerExited += PART_Grid_PointerExited;
#endif
            }

            if (PART_Items != null)
            {
                PART_Items.tileview = this;
                InitializeItems();
            }

            ChangeMinimizedItemsOrientation();
            base.OnApplyTemplate();
        }

        #endregion

        #region Callback Methods

        private static void OnMinimizedItemsOrientationChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SfTileView instance = obj as SfTileView;
            instance.OnMinimizedItemsOrientationChanged(args);
        }

        /// <summary>
        /// Occurs when the selected TileView item <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTileView"/> 
        /// minimized items orientation is changed.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnMinimizedItemsOrientationChanged(DependencyPropertyChangedEventArgs args)
        {
            ChangeMinimizedItemsOrientation();
            if (tileViewPanel != null)
                tileViewPanel.InvalidateMeasure();
        }

        private static void OnMaximizedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SfTileView tileview = sender as SfTileView;
            if (tileview != null)
            {
                tileview.ChangeMinimizedItemsOrientation();
            }
        }

        private static void OnMaximizedItemWidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SfTileView tileview = sender as SfTileView;
            if (tileview != null)
            {
                if (Convert.ToDouble(e.NewValue) == tileview.ActualWidth || Convert.ToDouble(e.NewValue) == tileview.ActualWidth - tileview.ItemWidth - 10 || Convert.ToDouble(e.NewValue) == tileview.ItemWidth)
                    tileview.maxItemWidthSet = true;
            }
        }

        private static void OnMaximizedItemHeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SfTileView tileview = sender as SfTileView;
            if (tileview != null)
            {
                if (Convert.ToDouble(e.NewValue) == tileview.ActualHeight || Convert.ToDouble(e.NewValue) == tileview.ActualHeight - tileview.ItemHeight - 10 || Convert.ToDouble(e.NewValue) == tileview.ItemHeight)
                    tileview.maxItemHeightSet = true;
            }
        }

        private static void OnOrientationChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SfTileView instance = obj as SfTileView;
            instance.OnOrientationChanged(args);
        }

        /// <summary>
        /// Occurs when the selected TileView item <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTileView"/> orientation is changed.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnOrientationChanged(DependencyPropertyChangedEventArgs args)
        {
            if (MaximizedItem == null)
            {
                if (PART_Items != null && PART_Items.scrollViewer != null)
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        PART_Items.scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                        PART_Items.scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    }
                    else
                    {
                        PART_Items.scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                        PART_Items.scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    }
                }
                if (tileViewPanel != null)
                {
                    tileViewPanel.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Occurs when the selected TileView item <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTileView"/> is changed.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnSelectionChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateSelection();
            base.OnSelectionChanged(args);
        }

#if !WINDOWS_PHONE && !WINDOWS_PHONE_7
        /// <summary>
        /// Method to initialize the TileViewItem Drag events.
        /// </summary>
        /// <param name="tileViewItem">The tileViewItem.</param>
        internal virtual void StartTileViewItemDragEvents(SfTileViewItem tileViewItem)
        {
            tileViewItem.PointerPressed += tileViewItem_PointerPressed;
            tileViewItem.PointerMoved += tileViewItem_PointerMoved;
            tileViewItem.PointerReleased += tileViewItem_PointerReleased;

            if (tileViewItem.State == TileViewItemState.Maximized)
            {
                MaximizedItem = tileViewItem;
				previousItem = tileViewItem;
            }
        }



        private void tileViewItem_PointerReleased(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            if (previousDraggingItem != null)
            {
                previousDraggingItem.Opacity = 1;
                CloseDraggingPopup();
                UpdateLayout();
            }
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            if (AllowDragDrop)
                CancelDragging();
            base.OnPointerExited(e);
        }
        internal PointerPoint point, previousPoint, pointPressed;

        internal SfTileViewItem draggedItem = null;

        void tileViewItem_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            SfTileViewItem draggingItem = sender as SfTileViewItem;

            if (e.Handled == false)
            {
                if (AllowDragDrop && this.AllowReorder && IsPointerPressed)
                {
                    point = e.GetCurrentPoint(Window.Current.Content);

                    if (IsPointerPressed && draggingItem != null && previousDraggingItem != null && IsPointerPressed && point.Properties.IsLeftButtonPressed && (draggedItem == null || draggedItem != draggingItem))
                    {
                        popupTimer.Stop();

                        if (previousDraggingItem != null)
                        {
                            if (!previousDraggingItem.PART_Preview.IsOpen)
                            previousDraggingItem.PART_Preview.IsOpen = true;
                            previousDraggingItem.PART_Preview.HorizontalOffset = point.Position.X -((ItemWidth+ 50)/2);
                            previousDraggingItem.PART_Preview.VerticalOffset = point.Position.Y - 100;
                            int currentIndex;
                            if (ItemsSource != null)
                            {
                                VisualStateManager.GoToState(previousDraggingItem, "ScaleIn", true);
                                currentIndex = MinimizedItems.IndexOf(draggingItem.DataContext);
                                MinimizedItems.Remove(previousDraggingItem.DataContext);
                                MinimizedItems.Insert(currentIndex, previousDraggingItem.DataContext);
                            }
                            else
                            {
                                currentIndex = MinimizedItems.IndexOf(draggingItem);
                                previousDraggingItem.Opacity = 0.5;
                                MinimizedItems.Remove(previousDraggingItem);
                                MinimizedItems.Insert(currentIndex, previousDraggingItem);
                            }
                            foreach (var item in tileViewPanel.Children)
                            {
                                SfTileViewItem tileItem = item as SfTileViewItem;
                                if (tileItem != previousDraggingItem && tileItem.PART_Host != null)
                                    tileItem.PART_Host.Margin = new Thickness(5);
                                if (PART_Content != null)
                                    PART_Content.Margin = new Thickness(5);
                            }
                        }
                        draggedItem = draggingItem;
                        UpdateLayout();

                        #region Scrolling_ScrollViewer
                        if (previousPoint == null)
                            previousPoint = point;
                        if (MaximizedItem == null)
                        {
                            double mouseDelta;
                            if (Orientation == Orientation.Horizontal && tileViewPanel.ActualHeight > PART_Items.scrollViewer.ActualHeight)
                            {
                                mouseDelta = point.Position.Y - previousPoint.Position.Y;
                                if (mouseDelta > 10)
                                    PART_Items.scrollViewer.ScrollToVerticalOffset(PART_Items.scrollViewer.VerticalOffset + 25);
                                else
                                    PART_Items.scrollViewer.ScrollToVerticalOffset(PART_Items.scrollViewer.VerticalOffset - 25);
                            }
                            else if (tileViewPanel.ActualWidth > PART_Items.scrollViewer.ActualWidth)
                            {
                                mouseDelta = point.Position.X - previousPoint.Position.X;
                                if (mouseDelta > 10)
                                    PART_Items.scrollViewer.ScrollToHorizontalOffset(PART_Items.scrollViewer.HorizontalOffset + 25);
                                else
                                    PART_Items.scrollViewer.ScrollToHorizontalOffset(PART_Items.scrollViewer.HorizontalOffset - 25);
                            }

                        }
                        else
                        {
                            double mouseDelta;
                            if (MinimizedItemsOrientation == MinimizedItemsOrientation.Left || MinimizedItemsOrientation == MinimizedItemsOrientation.Right)
                            {
                                mouseDelta = point.Position.Y - previousPoint.Position.Y;
                                if (mouseDelta > 0)
                                    PART_Items.scrollViewer.ScrollToVerticalOffset(PART_Items.scrollViewer.VerticalOffset + 25);
                                else
                                    PART_Items.scrollViewer.ScrollToVerticalOffset(PART_Items.scrollViewer.VerticalOffset - 25);
                            }
                            else
                                if (MinimizedItemsOrientation == MinimizedItemsOrientation.Top || MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                                {
                                    mouseDelta = point.Position.X - previousPoint.Position.X;
                                    if (mouseDelta > 0)
                                        PART_Items.scrollViewer.ScrollToHorizontalOffset(PART_Items.scrollViewer.HorizontalOffset + 25);
                                    else
                                        PART_Items.scrollViewer.ScrollToHorizontalOffset(PART_Items.scrollViewer.HorizontalOffset - 25);
                                }
                        }
                        previousPoint = point;
                        #endregion
                    }
                    if (!e.GetCurrentPoint(Window.Current.Content).IsInContact || !e.GetCurrentPoint(Window.Current.Content).Properties.IsLeftButtonPressed)
                        CancelDragging();
                }
            }
        }

        internal void CloseDraggingPopup()
        {
            foreach (var item in tileViewPanel.Children)
            {
                SfTileViewItem tileItem = item as SfTileViewItem;

                if (tileItem.PART_Host != null)
                {
                    tileItem.PART_Host.Margin = new Thickness(0);
                    tileItem.PART_Host.Opacity = 1;
                }
                VisualStateManager.GoToState(tileItem, "Normal", true);
                if (tileItem.PART_Preview != null)
                {
                    tileItem.PART_Preview.IsOpen = false;
                    tileItem.PART_Preview.Opacity = 0;
                }
                if (PART_Content != null)
                    PART_Content.Margin = tileItem.Margin;
            }
           
        }

        void tileViewItem_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            SfTileViewItem draggingItem = sender as SfTileViewItem;

            if (AllowDragDrop && AllowReorder)
            {
                pointPressed = e.GetCurrentPoint(Window.Current.Content);
                CancelDragging();
                dragTimer.Start();
                previousDraggingItem = draggingItem;
                previousDraggingItem.PART_PreviewHost.MinWidth = this.ItemWidth + 10;
                previousDraggingItem.PART_PreviewHost.MinHeight = this.ItemHeight + 10;
            }
        }

        /// Method to delete the TileViewItem Drag events.
        /// <summary>
        /// <param name="RC">draggable report card</param>
        /// </summary>
        internal virtual void EndTileViewItemDragEvents(SfTileViewItem RC)
        {
            RC.PointerPressed -= tileViewItem_PointerPressed;
            RC.PointerMoved -= tileViewItem_PointerMoved;
            RC.PointerReleased -= tileViewItem_PointerReleased;
        }
#endif
        #endregion


        public void Dispose()
        {
            if (ItemsSource is INotifyCollectionChanged)
                ((INotifyCollectionChanged)ItemsSource).CollectionChanged -= TileView_CollectionChanged;

            if (PART_Items != null)
            {
                PART_Items.tileview = null;
            }
            SfTileViewItem item;
            if (tileViewPanel != null && tileViewPanel.Children.Count > 0)
            {
                foreach (var vitem in tileViewPanel.Children)
                {
                    if (!(vitem is SfTileViewItem))
                        item = ItemContainerGenerator.ContainerFromItem(vitem) as SfTileViewItem;
                    else
                        item = vitem as SfTileViewItem;
                    if (item != null)
                    {
#if !WINDOWS_PHONE && !WINDOWS_PHONE_7
                        EndTileViewItemDragEvents(item);
#endif
                        item.ParentTileView = null;
                    }
                }
            }

            if (MaximizedItem != null)
            {
#if !WINDOWS_PHONE && !WINDOWS_PHONE_7
                EndTileViewItemDragEvents(MaximizedItem);
#endif
                MaximizedItem.ParentTileView = null;
                MaximizedItem = null;
            }
                
      
#if !WINDOWS_PHONE && !WINDOWS_PHONE_7
            if (AllowDragDrop)
            {
                if (PART_Items != null)
                {
                    PART_Items.scrollViewer.GotFocus -= scrollViewer_GotFocus;
                    PART_Items.scrollViewer.PointerExited -= scrollViewer_PointerExited;
                    PART_Items.scrollViewer.LostFocus -= scrollViewer_LostFocus;
                }
                this.PointerCaptureLost -= SfTileView_PointerCaptureLost;
                this.PointerReleased -= SfTileView_PointerReleased;
                this.PointerExited -= SfTileView_PointerExited;

                this.PointerMoved -= Content_PointerMoved;
                Window.Current.Content.PointerReleased -= Content_PointerReleased;   
             }
#endif

            this.Loaded -= TileView_Loaded;
            if(minimizeButtonTimer !=null)
                minimizeButtonTimer.Tick -= timer_Tick;
#if !WINDOWS_PHONE && !WINDOWS_PHONE_7
            if(popupTimer!=null)
                popupTimer.Tick -= popupTimer_Tick;
            if(dragTimer!=null)
                dragTimer.Tick -= dragTimer_Tick;
#endif

            
            if (PART_MinimizeButton != null)
            {
                PART_MinimizeButton.Click -= PART_MinimizeButton_Click;
            }
            if (PART_Grid != null)
            {
                PART_Grid.ManipulationDelta -= PART_Content_ManipulationDelta;
                PART_Grid.ManipulationCompleted -= PART_Content_ManipulationCompleted;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                PART_Grid.MouseEnter -= PART_Grid_MouseMove;
#else
                PART_Grid.PointerMoved -= PART_Grid_PointerMoved;
                PART_Grid.PointerExited -= PART_Grid_PointerExited;
#endif
            }
        }
    }
}

// <copyright file="TileViewItem.cs" company="Syncfusion">
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
#if !WINDOWS_PHONE_7
using System.Threading.Tasks;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls.Layout
#else
namespace Syncfusion.UI.Xaml.Controls.Layout
#endif
{
    /// <summary>
    /// Represents a selectable item inside a <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileView"/>.
    /// </summary>
    /// <remarks>
    /// Tileview item is a <see cref="T:Windows.UI.Xaml.Controls.ContentControl">ContentControl</see>
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class SfTileViewItem : ContentControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfTileViewItem()
        {
            DefaultStyleKey = typeof(SfTileViewItem);
            this.Loaded += SfTileViewItem_Loaded;
            this.Unloaded += SfTileViewItem_Unloaded;
            this.IsEnabledChanged += SfTileViewItem_IsEnabledChanged;
        }

        void SfTileViewItem_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= SfTileViewItem_Loaded;
            this.Unloaded -= SfTileViewItem_Unloaded;
            this.IsEnabledChanged -= SfTileViewItem_IsEnabledChanged;
#if !WINDOWS_PHONE && !WINDOWS_PHONE_7
			if(this.ParentTileView != null)
  			this.ParentTileView.PART_Items.scrollViewer.ManipulationDelta -= scrollViewer_ManipulationDelta;
#endif
        }

        void SfTileViewItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (ParentTileView != null)
            {
#if !WINDOWS_PHONE && !WINDOWS_PHONE_7
                ParentTileView.EndTileViewItemDragEvents(this);
               ParentTileView.StartTileViewItemDragEvents(this);
               this.ParentTileView.PART_Items.scrollViewer.ManipulationDelta += scrollViewer_ManipulationDelta;
#else
                if (this.State == TileViewItemState.Maximized)
                {
                    if (ParentTileView.ItemsSource == null)
                    {
                        ParentTileView.MinimizedItems.Remove(this);
                        if (ParentTileView.previousItem != null && ParentTileView.previousItem != this && ParentTileView.previousItem.State != TileViewItemState.Normal)
                            ParentTileView.previousItem.State = TileViewItemState.Normal;
                        ParentTileView.MaximizedItem = this;
                        ParentTileView.previousItem = this;
                        ParentTileView.SelectedIndex = ParentTileView.Items.IndexOf(this);
                    }
                    else
                    {
                        ParentTileView.MinimizedItems.Remove(this.Content);
                        if (ParentTileView.previousItem != null && ParentTileView.previousItem != this && ParentTileView.previousItem.State != TileViewItemState.Normal)
                            ParentTileView.previousItem.State = TileViewItemState.Normal;
                        ParentTileView.MaximizedItem = this;
                        if (this.DataContext != null)
                            ParentTileView.SelectedIndex = ParentTileView.Items.IndexOf(this.DataContext);
                        if (ParentTileView.MaximizedItem != null && ParentTileView.MaximizedItem.MaximizedContent == null)
                        {
                            ParentTileView.MaximizedItem.MaximizedContent = Content;
                        }
                        ParentTileView.previousItem = this;
                    }
                }
#endif
               if (!IsEnabled)
                   VisualStateManager.GoToState(this, "Disabled", true);
               else
                   VisualStateManager.GoToState(this, "Normal", true);   
            }
        }

        void SfTileViewItem_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsEnabled)
                VisualStateManager.GoToState(this, "Disabled", true);
            else
                VisualStateManager.GoToState(this, "Normal", true);   
        }

        #endregion

        #region Variables

        internal SfTileView ParentTileView = null;
#if !WINDOWS_PHONE && !WINDOWS_PHONE_7
        internal Popup PART_Preview = null;
        internal Grid PART_PreviewHost = null,PART_Host=null;
#endif
        private bool _canExecute = true;

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the content in maximized <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItemState"/>.
        /// </summary>
        /// <remarks>
        /// Used to customize the Maximized item content.
        /// </remarks>
        /// <value>
        /// The default value is null.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public object MaximizedContent
        {
            get
            {
                return (object)GetValue(MaximizedContentProperty);
            }
            set
            {
                SetValue(MaximizedContentProperty, value);
            }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaximizedContent.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaximizedContentProperty =
            DependencyProperty.Register("MaximizedContent", typeof(object), typeof(SfTileViewItem), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItemState"/>.
        /// </summary>
        /// <value>
        /// The default value is <see
        /// cref="F:Syncfusion.UI.Xaml.Controls.Layout.TileViewItemState.Normal">TileViewItemState.Normal</see>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public TileViewItemState State
        {
            get { return (TileViewItemState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TileViewItemState.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(TileViewItemState), typeof(SfTileViewItem), new PropertyMetadata(TileViewItemState.Normal,new PropertyChangedCallback(OnStateChanged)));

        #endregion

        #region Override Methods

#if WINDOWS_PHONE
        /// <summary>
        /// Invoked when the Manipulation started
        /// </summary>
        /// <param name="e"></param>
        protected override void OnManipulationDelta(System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            _canExecute = false;
            base.OnManipulationDelta(e);
        }
#elif !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        void scrollViewer_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            _canExecute = false;
        }

#endif

        /// <summary>
        /// Invoked when the pointer is released
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
#else    
            protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
          
            Windows.UI.Input.PointerPoint p = e.GetCurrentPoint(Window.Current.Content);
            if (ParentTileView.AllowDragDrop &&  ParentTileView.pointPressed!=null && p.Position!=ParentTileView.pointPressed.Position && ParentTileView.IsPointerPressed)
            {
                ParentTileView.IsPointerPressed = false;
                if (ParentTileView.previousDraggingItem != null)
                {
                    int previousIndex, currentIndex;
                    if (ParentTileView.ItemsSource != null)
                    {
                        previousIndex = ParentTileView.MinimizedItems.IndexOf(ParentTileView.previousDraggingItem.DataContext);
                        currentIndex = ParentTileView.MinimizedItems.IndexOf(this.DataContext);

                        ParentTileView.MinimizedItems.Remove(ParentTileView.previousDraggingItem.DataContext);
                        ParentTileView.MinimizedItems.Insert(currentIndex, ParentTileView.previousDraggingItem.DataContext);
                        ParentTileView.MinimizedItems.Remove(this.DataContext);
                        ParentTileView.MinimizedItems.Insert(previousIndex, this.DataContext);
                    }
                    else
                    {
                        previousIndex = ParentTileView.MinimizedItems.IndexOf(ParentTileView.previousDraggingItem);
                        currentIndex = ParentTileView.MinimizedItems.IndexOf(this);

                        ParentTileView.MinimizedItems.Remove(ParentTileView.previousDraggingItem);
                        ParentTileView.MinimizedItems.Insert(currentIndex, ParentTileView.previousDraggingItem);
                        ParentTileView.MinimizedItems.Remove(this);
                        ParentTileView.MinimizedItems.Insert(previousIndex, this);
                    }
                }
            }
            else
#endif
            {
                if (_canExecute)
                {
                    if (State == TileViewItemState.Normal)
                    {
                        if (ParentTileView.previousItem != null &&
                            ParentTileView.previousItem.State != TileViewItemState.Normal)
                        {
                            ParentTileView.previousItem.State = TileViewItemState.Normal;
                        }
                        State = TileViewItemState.Maximized;
                    }
                    ParentTileView.previousItem = this;
                }
                _canExecute = true;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            base.OnMouseLeftButtonUp(e);
#else
            }
            if (ParentTileView.previousDraggingItem!=null && ParentTileView.previousDraggingItem.PART_Preview.IsOpen)
            {
                ParentTileView.previousDraggingItem.PART_Preview.IsOpen = false;
                ParentTileView.previousDraggingItem.PART_Host.Opacity = 1;
            }
            VisualStateManager.GoToState(this, "Normal", true);
                
            ParentTileView.CloseDraggingPopup();
            ParentTileView.previousDraggingItem = this;
            UpdateLayout();
            base.OnPointerReleased(e);
#endif
        }

#if !WINDOWS_PHONE && !WINDOWS_PHONE_7
            /// <summary>
            /// Occurs when Pointer entered
            /// </summary>
            /// <param name="e"></param>
            protected override void OnPointerEntered(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
            {
                if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                {
                    if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
                    {
                        VisualStateManager.GoToState(this, "PointerOver", true);
                    }
                    base.OnPointerEntered(e);
                }
            }

            /// <summary>
            /// Occurs when Pointer exited
            /// </summary>
            /// <param name="e"></param>    
            protected override void OnPointerExited(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
            {
               if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                {
                    VisualStateManager.GoToState(this, "Normal", true);
                    base.OnPointerExited(e);
                }
            }

            /// <summary>
            /// Initializes all the child elements of <see
            /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfTileViewItem"/> control.
            /// </summary>    
            protected override void OnApplyTemplate()
            {
                PART_PreviewHost = GetTemplateChild("PART_PreviewHost") as Grid;
                PART_Preview = GetTemplateChild("PART_Preview") as Popup;
                PART_Host = GetTemplateChild("PART_Host") as Grid;
                if (ParentTileView!=null && ParentTileView.previousDraggingItem!=null && ParentTileView.previousDraggingItem.PART_Host != null && ParentTileView.IsPointerPressed &&
                    ParentTileView.previousDraggingItem.PART_Host.Opacity == 0)
                {
                    int previousIndex=-1, currentIndex=-1;
                    if (ParentTileView.ItemsSource != null)
                    {
                        previousIndex = ParentTileView.MinimizedItems.IndexOf(ParentTileView.previousDraggingItem.DataContext);
                        currentIndex = ParentTileView.MinimizedItems.IndexOf(this.DataContext);

                        if (previousIndex != -1 && currentIndex != -1 && previousIndex == currentIndex)
                            VisualStateManager.GoToState(this, "ScaleIn", true);
                    }
                }
                base.OnApplyTemplate();
            }
#endif
        #endregion

        #region Callback Methods

        /// <summary>
        /// Called when the TileViewItemState Property changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
            /// <param name="args">The DependencyPropertyChangedEventArgs instance containing the event data.</param>
        private static void OnStateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SfTileViewItem instance = obj as SfTileViewItem;
            instance.OnStateChanged(args);
        }

        /// <summary>
        /// Called when the TileViewItemState Property changed.
        /// </summary>
        /// <param name="args">The DependencyPropertyChangedEventArgs instance containing the event data.</param>
        protected virtual void OnStateChanged(DependencyPropertyChangedEventArgs args)
        {
            if (ParentTileView != null)
            {
                if (State == TileViewItemState.Maximized)
                {
                    if (ParentTileView.ItemsSource == null)
                    {
                        ParentTileView.MinimizedItems.Remove(this);
                        if (ParentTileView.previousItem != null && ParentTileView.previousItem != this && ParentTileView.previousItem.State != TileViewItemState.Normal)
                            ParentTileView.previousItem.State = TileViewItemState.Normal;
                        ParentTileView.MaximizedItem = this;
						ParentTileView.previousItem = this;
                        ParentTileView.SelectedIndex = ParentTileView.Items.IndexOf(this);
                    }
                    else
                    {
                        ParentTileView.MinimizedItems.Remove(this.Content);
                        if (ParentTileView.previousItem != null && ParentTileView.previousItem != this && ParentTileView.previousItem.State != TileViewItemState.Normal)
                            ParentTileView.previousItem.State = TileViewItemState.Normal;
                        ParentTileView.MaximizedItem = this;
                        if(this.DataContext!=null)
                        ParentTileView.SelectedIndex = ParentTileView.Items.IndexOf(this.DataContext);
                        if (ParentTileView.MaximizedItem!=null && ParentTileView.MaximizedItem.MaximizedContent == null)
                        {
                            ParentTileView.MaximizedItem.MaximizedContent = Content;
                        }
                        ParentTileView.previousItem = this;
                    }
                }
                else
                {
                    if (ParentTileView.ItemsSource == null)
                        UpdateMinimizedItems(this);
                    else
                        UpdateMinimizedItems(this.Content);
                    
                    ParentTileView.MaximizedItem = null;
                }
                if (this.StateChanged != null)
                {
                    StateChangedEventArgs stateArgs = new StateChangedEventArgs() { NewValue = args.NewValue, OldValue = args.OldValue };
                    this.StateChanged(this, stateArgs);
                }
            }
        }

        private void UpdateMinimizedItems(object item)
        {
            if (ParentTileView != null && !ParentTileView.MinimizedItems.Contains(item))
            {
                if (ParentTileView.AllowReorder)
                    ParentTileView.MinimizedItems.Add(item);
                else
                {
                    if (ParentTileView.SelectedIndex >= 0 &&
                        ParentTileView.SelectedIndex < ParentTileView.MinimizedItems.Count)
                        ParentTileView.MinimizedItems.Insert(ParentTileView.SelectedIndex, item);
                    else
                    {
                        if (ParentTileView.previousItem != null)
                        {
                            int index = -1;
                            if(ParentTileView.ItemsSource != null)
                                index =ParentTileView.Items.IndexOf(ParentTileView.previousItem.DataContext);
                            else
                            {
                                index = ParentTileView.Items.IndexOf(ParentTileView.previousItem);
                            }

                            if (index >= 0 && index < ParentTileView.MinimizedItems.Count)
                            {
                                ParentTileView.MinimizedItems.Insert(index, item);
                            }
                            else
                            {
                                ParentTileView.MinimizedItems.Add(item);
                            }
                        }
                        else
                            ParentTileView.MinimizedItems.Add(item);
                    }
                }
            }
        }

        #endregion
        #region Events
        /// <summary>
        /// Occurs when current <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileviewItem.State"/> is changed.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public event StateChangedEventHandler StateChanged;
        #endregion
    }
}

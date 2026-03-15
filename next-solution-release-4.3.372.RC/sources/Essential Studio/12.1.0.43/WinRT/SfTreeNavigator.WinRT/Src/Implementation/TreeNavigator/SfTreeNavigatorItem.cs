#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
#if !(WPFSILVERLIGHT||WINDOWS_PHONE_7)
using Windows.System;
#endif
// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using Syncfusion.WP.Primitives;
using System.Windows.Media;
using System.Windows.Input;
namespace Syncfusion.WP.Controls.Navigation
#elif SILVERLIGHT
using System.Windows.Input;
using Syncfusion.Tools.Primitives;
using System.Windows.Media;
namespace Syncfusion.Tools.Controls.Navigation
#elif WPF
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
namespace Syncfusion.Windows.Controls.Navigation
#else
using Syncfusion.UI.Xaml.Primitives;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.Controls.Navigation
#endif
{
    /// <summary>
    /// Represents a selectable item inside <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigator"/>.
    /// </summary>
    /// <remarks>
    /// <b>SfTreeNavigatorItem </b>is a <see
    /// cref="N:Windows.UI.Xaml.Controls.ContentControl.">ContentControl</see>.
    /// </remarks>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigator"/>
    /// <seealso cref="N:Syncfusion.UI.Xaml.Controls">Syncfusion.UI.Xaml.Controls
    /// Namespace</seealso>
    public class SfTreeNavigatorItem : HeaderedItemsControl, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/> class.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigator"/>
        /// <seealso cref="N:Syncfusion.UI.Xaml.Controls">Syncfusion.UI.Xaml.Controls
        /// Namespace</seealso>
        public SfTreeNavigatorItem()
        {
            this.DefaultStyleKey = typeof(SfTreeNavigatorItem);
            Loaded += SfTreeNavigatorItem_Loaded;
        }
#if WINRT

        /// <summary>
        /// Returns a delegate when the <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/>
        /// is clicked
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="args"></param>
        public delegate void ItemClickEventHandler(Object Sender, ItemClickEventArgs args);

        /// <summary>
        /// Occurs when the <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/>
        /// is clicked
        /// </summary>
        public event ItemClickEventHandler ItemClicked;

#endif
        void SfTreeNavigatorItem_Loaded(object sender, RoutedEventArgs e)
        {
            SfTreeNavigatorItem treeitem = sender as SfTreeNavigatorItem;
            if (treeitem != null && parentHost != null &&  parentHost.parentTree != null && (parentHost.parentTree.hierarchyselectionlist.Contains(treeitem.DataContext)||treeitem.IsSelected))
            {
                if (treeitem.HasItems)
                    parentHost.parentTree.NavigateItem(treeitem);
                else
                {
                    if(parentHost.parentTree.SelectedItem==null||(parentHost.parentTree.SelectedItem != null && treeitem!=null && treeitem.Header != null && parentHost.parentTree.SelectedItem != treeitem.Header))
                    {
                        parentHost.parentTree.SelectedItem = treeitem;
                        parentHost.parentTree.Select(treeitem);
                    }
                }
            }
        }

        //The host in which this tree item present.
        internal TreeNavigatorItemsHost parentHost;

        //The host that will be created, when select this tree (in case of sub items). While navigating back, this will be null.
        internal TreeNavigatorItemsHost childHost;

        /// <summary>
        /// Invoked when the pointer is released
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
#if WPF
            e.Handled = true;
#endif
            if (parentHost != null &&
                parentHost.parentTree != null)
            {
                if (parentHost.parentTree.prevItem != null)
                    parentHost.parentTree.prevItem.IsSelected = false;

                IsSelected = true;

                parentHost.parentTree.prevItem = this;
            }
            base.OnMouseLeftButtonUp(e);
        }
#else
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (parentHost != null &&
                parentHost.parentTree != null)
            {
                if (parentHost.parentTree.prevItem != null)
                    parentHost.parentTree.prevItem.IsSelected = false;

                IsSelected = true;

               // parentHost.parentTree.prevItem = this;
            }
            base.OnPointerReleased(e);
        }
#endif

        /// <summary>
        /// Gets or sets a value indicating whether <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/> has items.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
#if WPF
        public new bool HasItems
#else
        public bool HasItems
#endif
        {
            get { return Items.Count > 0; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/> is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(SfTreeNavigatorItem), new PropertyMetadata(false, OnIsSelectedChanged));

        /// <summary>
        /// Returns a value defining the AccentBrush of the item <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/>
        /// </summary>
        public SolidColorBrush AccentBrush
        {
            get { return (SolidColorBrush)GetValue(AccentBrushProperty); }
            set { SetValue(AccentBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AccentBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AccentBrushProperty =
            DependencyProperty.Register("AccentBrush", typeof(SolidColorBrush), typeof(SfTreeNavigatorItem), new PropertyMetadata(null));

        private static void OnIsSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = sender as SfTreeNavigatorItem;
            if (control != null)
            {
                VisualStateManager.GoToState(control, "UnSelected", true);
                if (!control.HasItems)
                {
                    if ((bool) args.NewValue)
                    {
                        VisualStateManager.GoToState(control, "Normal", true);
                        VisualStateManager.GoToState(control, "Selected", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(control, "UnSelected", true);
                    }
                }

                if ((bool) args.NewValue)
                {
                    if (control.parentHost != null && control.parentHost.parentTree != null)
                    {
                        control.parentHost.parentTree.prevItem = control;
                        if (control.parentHost.parentTree.ItemsSource == null)
                        {
                            control.parentHost.parentTree.SelectedItem = control;
                        }
                        else
                        {
                            if(!(control.parentHost.parentTree.SelectedItem is SfTreeNavigatorItem) || (control.parentHost.parentTree.SelectedItem is SfTreeNavigatorItem && (control.parentHost.parentTree.SelectedItem  as SfTreeNavigatorItem).DataContext!=control.DataContext))
                                control.parentHost.parentTree.SelectedItem = control.DataContext;
                        }
                    }
                }
               
            }
        }
#if !WINRT

#else
        /// <summary>
        /// Invoked when the pointer is pressed.
        /// </summary>
        /// <param name="e"></param>
         protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (!IsSelected)
            {
                VisualStateManager.GoToState(this, "Pressed", true);
            }
            base.OnPointerPressed(e);
        }

        /// <summary>
         /// Occurs when the item <see
         /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/> is clicked.
        /// </summary>
        /// <param name="e"></param>
         protected override void OnTapped(TappedRoutedEventArgs e)
         {
             if (ItemClicked != null)
             {
                 Windows.Foundation.Point position = this.TransformToVisual(Window.Current.Content).TransformPoint(new Windows.Foundation.Point());
                 ItemClickEventArgs args = new ItemClickEventArgs(position, MouseMode.Left);
                 ItemClicked(this, args);
             } 
             base.OnTapped(e);
         }
#endif

        /// <summary>
        /// Invoked when the pointer is entered.
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE||WPF
#elif SILVERLIGHT
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (!IsSelected)
            {
                VisualStateManager.GoToState(this, "PointerOver", true);
            }
            base.OnMouseEnter(e);
        }

#else
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (!IsSelected)
            {
                VisualStateManager.GoToState(this, "PointerOver", true);
            }
            base.OnPointerEntered(e);
        }
#endif
        /// <summary>
        /// Invoked when the focus is lost.
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE||WPF
#elif SILVERLIGHT
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
            base.OnMouseLeave(e);
        }
#else

        protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
            base.OnPointerCaptureLost(e);
        }

        /// <summary>
        /// Invoked when the focus is lost.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
            base.OnPointerExited(e);
        }
#endif
        /// <summary>
        /// Invoked when the focus is obtained. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Focused", true);
            base.OnGotFocus(e);
        }

        /// <summary>
        /// Invoked when the focus is lost.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "UnFocused", true);
            base.OnLostFocus(e);
        }
#if !WINRT

#else
        /// <summary>
        /// Invoked when a key is pressed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Space || e.Key == VirtualKey.Enter)
            {
                IsSelected = true;
            }
          
            base.OnKeyDown(e);
        }

        /// <summary>
        /// Invoked when an item <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/> is clicked.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRightTapped(RightTappedRoutedEventArgs e)
        {
            if (ItemClicked != null)
            {
                Windows.Foundation.Point position = this.TransformToVisual(Window.Current.Content).TransformPoint(new Windows.Foundation.Point());
                ItemClickEventArgs args = new ItemClickEventArgs(position, MouseMode.Right);
                ItemClicked(this, args);
            }
            base.OnRightTapped(e);
        }

#endif
        /// <summary>
        /// Removes all host instances.
        /// </summary>
        public void Dispose()
        {
            parentHost = null;
            childHost = null;
        }
    }

#if WINRT
    /// <summary>
    /// Occurs when current <see
    /// cref="Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem.ItemClickEventHandler"/> is invoked.
    /// </summary>
    public class ItemClickEventArgs : RoutedEventArgs
    {
        private Windows.Foundation.Point itemPosition;
        private MouseMode  mouseMode;
        /// <summary>
        ///  Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem.ItemClickEventHandler"/> class.
        /// </summary>
        /// <param name="_itemPosition"></param>
        /// <param name="_mouseMode"></param>
        public ItemClickEventArgs(Windows.Foundation.Point _itemPosition, MouseMode _mouseMode)
        {
            itemPosition = _itemPosition;
            mouseMode = _mouseMode;
        }

        /// <summary>
        /// Gets the position of the item <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/>
        /// </summary>
        public Windows.Foundation.Point ItemPosition { get { return itemPosition; } }

        /// <summary>
        /// Gets the Mouse mode 
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.MouseMode"/>
        /// </summary>
        public MouseMode MouseMode{get { return mouseMode;  }}
    } 

    /// <summary>
    /// Defines an enum list for the mouse modes
    /// </summary>
    public enum MouseMode
    {
        /// <summary>
        /// Left mode
        /// </summary>
        Left,
        /// <summary>
        /// Right mode
        /// </summary>
        Right
    }

#endif
}

#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class CarouselItem : ContentControl
    {
        
        /// <summary>
        /// 
        /// </summary>
        public CarouselItem()
        {
            DefaultStyleKey = typeof(CarouselItem);
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(CarouselItem), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));


        /// <summary>
        /// Called when [is selected changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            CarouselItem item = sender as CarouselItem;
            object selectedItem = null;
            if (item != null)
            {
                if (item.Owner != null)
                {
                    int index = item.Owner.Items.IndexOf(item.Content);
                    selectedItem = item.Content;
                    if (index == -1)
                    {
                        index = item.Owner.Items.IndexOf(item);
                        selectedItem = item;
                    }
                    if ((bool)args.NewValue)
                    {
                        if (item.Owner.SelectedIndex != index)
                            item.Owner.SelectedIndex = index;
                        if (item.Owner.SelectedItem!=null && !item.Owner.SelectedItem.Equals(selectedItem))
                        {
                            item.Owner.SelectedItem = selectedItem;
                        }
                        foreach (var child in item.Owner.Items)
                        {
                            if (child!=null && child is CarouselItem && child != item)
                            {
                                (child as CarouselItem).IsSelected = false;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>The owner.</value>
        public Carousel Owner
        {
            get { return (Carousel)GetValue(OwnerProperty); }
            set { SetValue(OwnerProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty OwnerProperty =
            DependencyProperty.Register("Owner", typeof(Carousel), typeof(CarouselItem), new PropertyMetadata(null));

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.IsSelected)
            {
                this.Owner.SelectedItem = this;
            }
            if (this.Content != null)
            {
                if (this.Owner.SelectedIndex >= 0)
                {
                    if (this.Owner.Items.Contains(this.Content))
                    {
                        if (this.Owner.SelectedIndex == (this.Owner.Items.IndexOf(this.Content)))
                        {
                            this.IsSelected = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {

            if (Owner != null && Owner.previousSelected!=null)
            {
                Owner.previousSelected.IsSelected = false;
                Owner.previousSelected = this;
            }
            else if (Owner != null)
            {
                Owner.previousSelected = this;
            }
            this.IsSelected = true;            
        }


    }
}

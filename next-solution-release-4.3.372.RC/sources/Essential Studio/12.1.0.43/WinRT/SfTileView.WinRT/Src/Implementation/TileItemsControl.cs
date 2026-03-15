// <copyright file="TileItemsControl.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
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
    /// Represents a TileItemsControl that used to present a collection of <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/>&apos;s as Tiled
    /// layout.
    /// </summary>
    /// <remarks>
    /// The TileItemscontrol is a <see cref="N:Windows.UI.Xaml.Controls.ItemsControl"/>.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public sealed class TileItemsControl : ItemsControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileItemsControl"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public TileItemsControl()
        {
            this.DefaultStyleKey = typeof(TileItemsControl);
        }

        #endregion

        #region Variables

        internal SfTileView tileview;

        internal ScrollViewer scrollViewer;

        internal ItemsPresenter itemsPresenter;

        #endregion

        #region Dependency Properties

#if WINDOWS_PHONE||WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the style for Item container
        /// </summary>
        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemContainerStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(TileItemsControl), new PropertyMetadata(null));
#endif

        /// <summary>
        /// Gets or sets the width of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/>.
        /// </summary>
        /// <value>
        /// The default value is zero.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileItemsControl.ItemHeight"/>
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
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(TileItemsControl), new PropertyMetadata(0.0));


        /// <summary>
        /// Gets or sets the height of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/>.
        /// </summary>
        /// <value>
        /// The default value is zero.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileItemsControl.ItemWidth"/>
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
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(TileItemsControl), new PropertyMetadata(0.0));


        /// <summary>
        /// Gets or sets a value that indicates the dimension by which <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/>&apos;s are stacked.
        /// </summary>
        /// <value>
        /// It will accepts the type is <see
        /// cref="N:Windows.UI.Xaml.Controls.Orientation"/>. The default value is <see
        /// cref="N:Windows.UI.Xaml.Controls.Orientation"/>.Horizontal.
        /// </value>
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
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(TileItemsControl), new PropertyMetadata(Orientation.Horizontal));

        #endregion

        #region Override Methods
        /// <summary>
        /// Initializes all the child elements of the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.TileItemsControl"/> control.
        /// </summary>
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        public override void OnApplyTemplate()
#else
         protected override void OnApplyTemplate()
#endif
        {
            scrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
            itemsPresenter = GetTemplateChild("Part_ItemsPresenter") as ItemsPresenter;
         	tileview.ChangeMinimizedItemsOrientation();
            base.OnApplyTemplate();
        }

         /// <summary>
         /// Checks if the item is a <see 
         /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTileViewItem"/>
         /// </summary>
         /// <param name="item"></param>
         /// <returns>
         /// <c>true</c> if this instance is selected; otherwise, <c>false</c>
         /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is SfTileViewItem;
        }

        /// <summary>
        /// Checks if the item is a <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTileViewItem"/>
        /// </summary>
        /// <returns>Dependency Object</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new SfTileViewItem();
        }

        /// <summary>
        /// Arranges the container for overrided items
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            SfTileViewItem tileItem = element as SfTileViewItem;
            if (tileItem != null)
            {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                (tileItem as SfTileViewItem).Style = this.ItemContainerStyle;
#endif
                tileItem.ParentTileView = tileview;
            }
            base.PrepareContainerForItemOverride(element, item);
        }

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
               if (tileview != null && tileview.AllowDragDrop)
                {
                    tileview.CloseDraggingPopup();
                }
            base.OnPointerExited(e);
        }
#endif
        #endregion
    }
}

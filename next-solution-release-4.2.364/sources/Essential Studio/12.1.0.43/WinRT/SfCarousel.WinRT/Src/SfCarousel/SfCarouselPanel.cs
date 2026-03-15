
// <copyright file="SfCarouselPanel.cs" company="Syncfusion">
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
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

#if WINDOWS_PHONE
namespace Syncfusion.WP.Controls.Layout
#else
    namespace Syncfusion.UI.Xaml.Controls.Layout
#endif
{
    /// <summary>
    /// Used to group and arrange the collections of <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarouselItem"/>
    /// </summary>
    /// <remarks>
    /// SfCarouselPanel is a <see cref="N:Windows.UI.Xaml.Controls.Canvas"/>.<see
    /// href=""/>
    /// </remarks>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel"/>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class SfCarouselPanel : Canvas
    {

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarouselPanel"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfCarouselPanel()
        {
            this.Loaded += SfCarouselPanel_Loaded;
        }

        #endregion

        #region Variables

        private SfCarousel parentItemsControl;

        internal SfCarousel ParentItemsControl
        {
            get
            {
                if (parentItemsControl == null)
                {
                    parentItemsControl = ItemsControl.GetItemsOwner(this) as SfCarousel;
                }
                return parentItemsControl;
            }
        }

        #endregion

        #region Helper Methods
#if WINDOWS_PHONE
        private void SfCarouselPanel_Loaded(object sender, EventArgs e)
#else
        private void SfCarouselPanel_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
#endif
        {
            ParentItemsControl.itemspanel = this;
        }

        #endregion

        #region Override Methods 
#if !WINDOWS_PHONE
        /// <summary>
        /// Sets the Size of the<see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.SfCarousel"/> control.
        /// </summary>
        protected override Size MeasureOverride(Size availableSize)
        {
            double width = 0.0;
            double height = 0.0;

            foreach (UIElement element in Children)
            {
                element.Measure(availableSize);
                height = Math.Max(height, element.DesiredSize.Height);
                width += element.DesiredSize.Width;
            }

            return new Size(width, height);
        }
#endif
        #endregion

    }

}

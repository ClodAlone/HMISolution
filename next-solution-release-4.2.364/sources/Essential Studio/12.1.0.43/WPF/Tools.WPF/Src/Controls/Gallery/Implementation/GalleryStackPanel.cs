// <copyright file="GalleryStackPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class extends standard StackPanel, is used as item panel in <see cref="Gallery"/> in standard
    /// visual mode.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GalleryStackPanel : StackPanel
    {
        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="GalleryStackPanel"/> class.
        /// Makes this class the owner of the IntervalProperty.
        /// </summary>
        static GalleryStackPanel()
        {
            EnvironmentTest.ValidateLicense(typeof(GalleryStackPanel));
            Gallery.IntervalProperty.AddOwner(typeof(GalleryStackPanel));
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Measures children in anticipation of arranging them during
        /// the ArrangeOverride pass.
        /// </summary>
        /// <param name="constraint">Size given for panel.</param>
        /// <returns>
        /// Size which is needed for the panel.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (double.IsInfinity(constraint.Width) || double.IsInfinity(constraint.Height))
            {
                constraint = CorrectPanelSize(constraint);
            }

            double interval = (double)GetValue(Gallery.IntervalProperty);
            UIElementCollection collection = this.Children;
            int collCount = collection.Count;

            Size size = base.MeasureOverride(constraint);
            size.Height = 0;

            for (int i = 0; i < collCount; i++)
            {
                size.Height += collection[i].DesiredSize.Height;
            }

            if (collCount != 0)
            {
                size.Height += (collCount - 1) * interval;
            }

            size.Height = Math.Min(size.Height, constraint.Height);
            return size;
        }

        /// <summary>
        /// Corrects panel size (it can`t be infinity)
        /// </summary>
        /// <param name="constraint">size to correct</param>
        /// <returns>corrected size</returns>
        private Size CorrectPanelSize(Size constraint)
        {
            ItemsPresenter itemsPresenter = (ItemsPresenter)TemplatedParent;
            Gallery gallery = (Gallery)itemsPresenter.TemplatedParent;

            if (double.IsInfinity(constraint.Width) || double.IsInfinity(constraint.Height))
            {
                int galleryItemsCount = gallery.Items.Count;
                int groupItemsCount = 1;
                int maxCount = 1;

                for (int i = 0; i < galleryItemsCount; ++i)
                {
                    GalleryGroup group = (GalleryGroup)gallery.Items[i];

                    if (group.VisualMode == GalleryVisualMode.Detailed)
                    {
                        groupItemsCount += group.Items.Count;
                    }
                    else if (maxCount < group.Items.Count)
                    {
                        maxCount = group.Items.Count;
                        groupItemsCount++;
                    }
                }

                if (double.IsInfinity(constraint.Width))
                {
                    double val = gallery.ItemWidth > 0 ? gallery.ItemWidth : gallery.ItemMaxWidth;
                    double width = (val * maxCount) +
                        ((gallery.GroupStandardBorderMargin.Left + gallery.GroupStandardBorderMargin.Right) * 2);

                    if (gallery.AllowedItemResizeMode == AllowedItemResizeModes.Space)
                    {
                        width += (maxCount - 1) * gallery.SpaceLimitBetweenItems;
                    }

                    if (width < gallery.MinWidth)
                    {
                        width = gallery.MinWidth;
                    }

                    constraint.Width = width;
                }

                if (double.IsInfinity(constraint.Height))
                {
                    double val = gallery.ItemHeight > 0 ? gallery.ItemHeight : gallery.ItemMaxHeight;
                    double height = val * groupItemsCount;

                    if (height < gallery.MinHeight)
                    {
                        height = gallery.MinHeight;
                    }

                    constraint.Height = height;
                }
            }

            return constraint;
        }

        /// <summary>
        /// Arranges the content.
        /// </summary>
        /// <param name="arrangeSize">Size in which the content should
        /// be arranged.</param>
        /// <returns>
        /// The size where the content was arranged.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            UIElementCollection collection = this.Children;
            int collCount = collection.Count;

            Gallery gallery = GetGallery(collCount, collection);

            if (Gallery.GetInterval(this) == 0 || collCount == 0)
            {
                Size result = base.ArrangeOverride(arrangeSize);
                return result;
            }

            double interval = Gallery.GetInterval(this);

            Rect rect = new Rect();
            rect.Height = collection[0].DesiredSize.Height;
            rect.Width = Math.Max(arrangeSize.Width, collection[0].DesiredSize.Width);

            collection[0].Arrange(rect);

            double originalHeight = collection[0].DesiredSize.Height;

            for (int i = 1; i < collCount; i++)
            {
                rect.Y += interval + collection[i - 1].DesiredSize.Height;
                rect.Height = collection[i].DesiredSize.Height;
                rect.Width = Math.Max(arrangeSize.Width, collection[i].DesiredSize.Width);
                collection[i].Arrange(rect);

                originalHeight += interval + collection[i].DesiredSize.Height;
            }

            arrangeSize.Height = Math.Max(arrangeSize.Height, originalHeight);
            SetGalleryHeight(gallery, originalHeight);
            return arrangeSize;
        }

        /// <summary>
        /// Gets gallery instance from the gallery group.
        /// </summary>
        /// <param name="collCount">Groups count in gallery</param>
        /// <param name="collection">Groups collection</param>
        /// <returns>Returns the Gallery</returns>
        private Gallery GetGallery(int collCount, UIElementCollection collection)
        {
            Gallery gallery = null;

            if (collCount > 0)
            {
                FrameworkElement group = collection[0] as FrameworkElement;
                gallery = group.Parent as Gallery;
            }

            return gallery;
        }

        /// <summary>
        /// Sets the height of the gallery.
        /// </summary>
        /// <param name="gallery">The gallery.</param>
        /// <param name="height">The height.</param>
        private void SetGalleryHeight(Gallery gallery, double height)
        {
            if (gallery != null)
            {
                gallery.Height = height;
            }
        }
        #endregion
    }
}

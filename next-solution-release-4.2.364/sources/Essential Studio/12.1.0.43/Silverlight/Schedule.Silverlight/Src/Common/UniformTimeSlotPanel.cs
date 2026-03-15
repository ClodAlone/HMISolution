#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Schedule
{
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
    using System.Linq;
    using Syncfusion.Windows.ComponentModel;
    using System.Windows.Threading;

    /// <summary>
    /// A UniformWrapPanel behaves similar to an horizontal WrapPanel that places the items in a 
    /// "vertical first" approach and tries to keep all columns with the same number of items.
    /// </summary>
    /// <remarks>
    /// If given Infinite width, the UniformWrapPanel will layout in a single column
    /// to avoid an horizontal scroll bar.
    /// </remarks>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class UniformTimeSlotPanel : Panel
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.UniformTimeSlotPanel"/> class.
        /// </summary>
        public UniformTimeSlotPanel()
        {
        }

        #region Layout Related Fields

        // The total columns used by the panel
        private int columns;

        // The total rows used by the panel
        private int rows;

        // The number of columns that items in all the rows. Remaining columns do not
        // have an item in the last row.
        private int fullColumns;

        // The calculated width of the items
        private double itemWidth;

        // The calculated height of the items
        private double itemHeight;

        #endregion

        #region Measure and Arrange Methods
        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for
        /// child elements and determines a size for the <see
        /// cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to
        /// child elements. Infinity can be specified as a value to indicate that the
        /// element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its
        /// calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.Children.Count == 0)
            {
                return Size.Empty;
            }

#if SILVERLIGHT
            var visibleItemsCount = this.Children.Where(item => item.Visibility == Visibility.Visible).Count();
#else
            int visibleItemsCount = this.Children.ToTypedList<UIElement>().Where(el => el.Visibility != Visibility.Collapsed).Count();
#endif
            if (visibleItemsCount == 0)
            {
                return Size.Empty;
            }

            var itemSize = availableSize;
            var frameworkEl = (FrameworkElement)this.Children[0];
            var itemsContainer = frameworkEl.FindParentElementOfType<IScheduleCalendarViewModelHost>();
            var intervalCount = ScheduleTimeLineHourControl.MaxValue + 1;
            if (!double.IsInfinity(itemSize.Height))
            {
                this.itemHeight = itemSize.Height /= intervalCount;
            }

            var maxWidth = 0d;
            var height = 0d;
            this.columns = itemsContainer.Model.SelectedDates.Count; //(int)Math.Ceiling((double)visibleItemsCount / rows);
            this.rows = intervalCount;
            //if (!double.IsNaN(availableSize.Height) && itemHeight < availableSize.Height)
            //{
            //    this.rows = (int)Math.Floor(availableSize.Height / itemHeight) / this.columns;
            //}

            fullColumns = visibleItemsCount - (rows * (columns - 1));

            if (double.IsPositiveInfinity(height))
            {
                height = rows * itemHeight;
            }
            else
            {
                height = Math.Max(availableSize.Height, rows * itemHeight);
            }

            itemSize.Width = this.itemWidth = availableSize.Width / columns;

            // measure again to apply uniform width / height
            foreach (UIElement item in this.Children)
            {
                if (item.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                item.Measure(itemSize);
                maxWidth = Math.Max(maxWidth, item.DesiredSize.Width);
            }

            if (double.IsInfinity(availableSize.Height))
            {
                availableSize.Height = height;
            }

            if (double.IsInfinity(availableSize.Width))
            {
                availableSize.Width = maxWidth;
            }

            return availableSize;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a
        /// size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element
        /// should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            int currentItem = 0;
            int currentRow = 0;
            int currentColumn = 0;

            UIElementCollection internalChildren = this.Children;
            int count = internalChildren.Count;
            while (currentItem < count)
            {
                UIElement element = internalChildren[currentItem];
                if (element != null)
                {
                    Rect childArea = new Rect(
                       currentColumn * itemWidth,
                       currentRow * itemHeight,
                       itemWidth,
                       itemHeight);

                    element.Arrange(childArea);

                    currentRow++;

                    if ((currentRow >= rows) ||
                       ((currentRow >= rows - 1) && (currentColumn >= fullColumns)))
                    {
                        currentRow = 0;
                        currentColumn++;
                    }
                }
                currentItem++;
            }
            return finalSize;
        }

        #endregion
    }
}

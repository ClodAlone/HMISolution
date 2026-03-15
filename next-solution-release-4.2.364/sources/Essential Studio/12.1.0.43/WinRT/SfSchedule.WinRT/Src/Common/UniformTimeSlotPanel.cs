#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
#else
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
#endif
#if !WPF
using System.Linq;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a uniform time slot panel.
    /// </summary>
    public class UniformTimeSlotPanel : Panel
    {
        #region Private Fields

        // The total columns used by the panel
        private int columns;

        // The calculated width of the items
        private double itemWidth;

        // The calculated height of the items
        private double itemHeight;

        #endregion

        #region Methods

        #region CalculateLeafChildCount

        int CalculateLeafChildCount(ResourceType restype)
        {
            int leafchild = 1;
            leafchild = leafchild * restype.ResourceCollection.Count;
            ResourceType tempresotype = restype.SubResourceType;
            while (tempresotype != null)
            {
                leafchild = leafchild * tempresotype.ResourceCollection.Count;
                tempresotype = tempresotype.SubResourceType;
            }
            return leafchild;
        }

        #endregion

        #endregion

        #region Override Methods

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count == 0)
            {
                return new Size();
            }
#if WPF
            var visibleItemsCount = Children.Count;
#else
            var visibleItemsCount = Children.Count(item => item.Visibility == Visibility.Visible);
#endif

            if (visibleItemsCount == 0)
            {
                return new Size();
            }

            var itemSize = availableSize;
            var line = Children[0] as Line;
            if (line != null && line.Y1.Equals(1))
            {
                var itemsContainer = (Children[0] as FrameworkElement).FindParentElementOfType<ScheduleVerticalTimeSlotItemsControl>();
                int intervalCount = ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue;
                if (itemSize.Height == 0)
                {
                    itemHeight = 0;
                }
                else if (!double.IsInfinity(itemSize.Height))
                {
                    itemHeight = itemSize.Height /= intervalCount;
                }

                var maxWidth = 0d;
                columns = itemsContainer.SelectedDates.Count;

                var scroll = this.FindParentElementOfType<ScrollViewer>();
                var schedule = this.FindParentElementOfType<SfSchedule>();
                if (schedule != null && schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
                {
                    double resourcecount = CalculateLeafChildCount(schedule.ScheduleResourceType);
                    double actualwidth;
                    if (schedule.ScheduleType == ScheduleType.Day)
                    {
                        double resourceColumns;
                        if (schedule.DayViewColumnCount > 0)
                        {
                            resourceColumns = schedule.DayViewColumnCount > resourcecount ? resourcecount : schedule.DayViewColumnCount;
                        }
                        else
                        {
                            if (scroll.ViewportWidth != 0 && scroll.ViewportWidth / resourcecount < schedule.MinResourceWidth)
                                resourceColumns = Math.Floor(scroll.ViewportWidth / schedule.MinResourceWidth);
                            else
                                resourceColumns = resourcecount;
                        }
                        actualwidth = (scroll.ViewportWidth / resourceColumns) * resourcecount;
                    }
                    else
                    {
                        actualwidth = scroll.ViewportWidth * resourcecount;
                    }
                    availableSize.Width = actualwidth;
                    itemSize.Width = itemWidth = availableSize.Width / (columns * (resourcecount));
                }
                else
                {
                    if (double.IsInfinity(availableSize.Width))
                    {
                        availableSize.Width = scroll.ViewportWidth;
                    }
                    if (availableSize.Width > 0)
                    {
                        itemSize.Width = itemWidth = availableSize.Width / columns;
                    }
                    else
                    {
                        itemSize.Width = itemWidth = 0;
                    }
                }
                if (double.IsInfinity(itemSize.Width) || double.IsNaN(itemSize.Width))
                {
                    itemSize.Width = itemWidth = 0;
                }
                // measure again to apply uniform width / height
                foreach (UIElement item in Children)
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
                    availableSize.Height = itemSize.Height;
                }
                if (double.IsInfinity(availableSize.Width))
                {
                    availableSize.Width = maxWidth;
                }

            }
            else
            {
                var itemsContainer = (Children[0] as FrameworkElement).FindParentElementOfType<ScheduleHorizontalTimeSlotItemsControl>();
                var intervalCount = (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue) * ScheduleTimeLineItemsControl.IntervalCount[(int)itemsContainer.TimeInterval];
                if (itemSize.Height == 0)
                {
                    itemHeight = 0;
                }
                else if (!double.IsInfinity(itemSize.Height))
                {
                    itemHeight = itemSize.Height /= intervalCount;
                }

                var maxWidth = 0d;

                var scroll = this.FindParentElementOfType<ScrollViewer>();
                var schedule = this.FindParentElementOfType<SfSchedule>();
                if (schedule != null && schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
                {
                    double resourcecount = schedule.ScheduleResourceType.ResourceCollection.Count;
                    double actualwidth;
                    if (schedule.ScheduleType == ScheduleType.Day)
                    {
                        double resourceColumns;
                        if (schedule.DayViewColumnCount > 0)
                        {
                            resourceColumns = schedule.DayViewColumnCount > resourcecount ? resourcecount : schedule.DayViewColumnCount;
                        }
                        else
                        {
                            if (scroll.ViewportWidth != 0 && scroll.ViewportWidth / resourcecount <= schedule.MinResourceWidth)
                                resourceColumns = Math.Floor(scroll.ViewportWidth / schedule.MinResourceWidth);
                            else
                                resourceColumns = resourcecount;
                        }
                        actualwidth = (scroll.ViewportWidth / resourceColumns) * resourcecount;
                    }
                    else
                    {
                        actualwidth = scroll.ViewportWidth * resourcecount;
                    }
                    availableSize.Width = actualwidth;
                    itemSize.Width = itemWidth = availableSize.Width / (columns * resourcecount);
                }
                else
                {
                    if (double.IsInfinity(availableSize.Width))
                    {
                        availableSize.Width = scroll.ViewportWidth;
                    }
                    if (availableSize.Width > 0)
                    {
                        itemSize.Width = itemWidth = availableSize.Width / columns;
                    }
                    else
                    {
                        itemSize.Width = itemWidth = 0;
                    }
                }
                if (double.IsInfinity(itemSize.Width) || double.IsNaN(itemSize.Width))
                {
                    itemSize.Width = itemWidth = 0;
                }
                // measure again to apply uniform width / height
                foreach (UIElement item in Children)
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
                    availableSize.Height = itemSize.Height;
                }

                if (double.IsInfinity(availableSize.Width))
                {
                    availableSize.Width = maxWidth;
                }
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int currentItem = 0;

            UIElementCollection internalChildren = Children;
            int count = internalChildren.Count;
#if !WINRT
            if (internalChildren.Count > 0)
            {
#endif
                var line = internalChildren[0] as Line;
                if (line != null && (internalChildren.Count > 0 && line.Y1.Equals(1)))
                {
                    double individualItemWidth = itemWidth;
                    while (currentItem < count)
                    {
                        UIElement element = internalChildren[currentItem];
#if WPF
                        if (!internalChildren[count - 1].Equals(element))
#else
                        if (internalChildren.Last() != element)
#endif
                        {
                            var childArea = new Rect(
                               currentItem * individualItemWidth,
                               0,
                               individualItemWidth,
                               finalSize.Height);
                            element.Arrange(childArea);

                        }
                        else
                        {
                            var childArea = new Rect(
                                                      (currentItem * individualItemWidth) - 1,
                                                      0,
                                                      individualItemWidth,
                                                      finalSize.Height);
                            if (element != null) element.Arrange(childArea);
                        }
                        currentItem++;
                    }
                }
                else
                {
                    double finwid = finalSize.Width;

                    while (currentItem < count)
                    {
                        UIElement element = internalChildren[currentItem];
                        if (element != null)
                        {
#if WPF
                            if (!internalChildren[count - 1].Equals(element))
#else
                            if (internalChildren.Last() != element)
#endif
                            {
                                var childArea = new Rect(
                                    0,
                                    currentItem * itemHeight,
                                    finwid,
                                    itemHeight);
                                element.Arrange(childArea);
                            }
                            else
                            {
                                var childArea = new Rect(
                                    0,
                                    (currentItem * itemHeight) - 1,
                                    finwid,
                                    itemHeight);
                                element.Arrange(childArea);
                            }
                        }
                        currentItem++;
                    }
                }
#if !WINRT
            }
#endif
            return finalSize;
        }

        #endregion
    }
}

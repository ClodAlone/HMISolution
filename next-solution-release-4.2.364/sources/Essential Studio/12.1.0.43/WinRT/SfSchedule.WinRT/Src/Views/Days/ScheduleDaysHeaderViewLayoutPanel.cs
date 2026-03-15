#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;
#if WINRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#else
using System.Windows.Controls;
using System.Windows;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a layout panel for arranging day view headers.
    /// </summary>
    public class ScheduleDaysHeaderViewLayoutPanel : Panel
    {
        #region Constructor

        public ScheduleDaysHeaderViewLayoutPanel()
        {
#if WINRT
            ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateInertia;
            ManipulationDelta += ScheduleDaysHeaderViewLayoutPanel_ManipulationDelta;
#endif
        }

        #endregion

        #region Methods

#if WINRT
        private void ScheduleDaysHeaderViewLayoutPanel_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
                return;
            SfSchedule schedule = this.FindParentElementOfType<SfSchedule>();
            ScheduleDaysView dayview = this.FindParentElementOfType<ScheduleDaysView>();
            if (Children.Count > 0 && Children[0].GetType() == typeof(ScheduleDaysHeaderViewControl))
            {
                double delta = dayview.scrollviewer.HorizontalOffset + (e.Delta.Translation.X * -1);
                if (delta <= dayview.scrollviewer.ScrollableWidth)
                {
#if SyncfusionFramework4_5_11
                    dayview.scrollviewer.ChangeView(delta, null, null);
#else
                    dayview.scrollviewer.ScrollToHorizontalOffset(delta);
#endif
                    if (delta <= 0)
                    {
                        if (e.IsInertial)
                        {
                            e.Complete();
                        }
                        else
                        {
                            schedule.ScrollManipulationCompleted = true;
                        }
                    }
                }
                else
                {
                    if (e.IsInertial)
                    {
                        e.Complete();
                    }
                    else
                    {
                        schedule.ScrollManipulationCompleted = true;
                    }
                }

            }
        }

#endif
        #region CalculateLeafChildCount

        int CalculateLeafChildCount(ResourceType restype)
        {
            int leafchild = 1;
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

        #region Private Members

        double maxElementWidth = 0.0;

        #endregion

        #region Overrides

        #region Measure Override

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count <= 0)
            {
                return new Size();
            }
#if WPF
            int visibleItemsCount = Children.Cast<UIElement>().Count(child => child.Visibility != Visibility.Collapsed);
#else
            int visibleItemsCount = Children.Count(child => child.Visibility != Visibility.Collapsed);
#endif

            Size itemSize = availableSize;
            var schedule = this.FindParentElementOfType<SfSchedule>();
            if (schedule != null && schedule.EnableAutoFormat)
            {
                foreach (UIElement item in Children)
                {
                    if (item.Visibility != Visibility.Collapsed)
                    {
                        item.Measure(itemSize);
                        maxElementWidth = Math.Max(maxElementWidth, item.DesiredSize.Width);
                    }
                }
            }
            if (!double.IsInfinity(itemSize.Width))
            {
                itemSize.Width /= visibleItemsCount;
                itemSize.Width = Math.Round(itemSize.Width, 0);
            }
            if (schedule != null)
            {
                schedule.needAutoFormat = (itemSize.Width <= maxElementWidth);
                schedule.HeaderFormat = (schedule.EnableAutoFormat && schedule.needAutoFormat) ? "ddd" : schedule.HeaderDateFormat;
            }
            double maxHeight = 0.0;
            foreach (UIElement item in Children)
            {
                if (item.Visibility != Visibility.Collapsed)
                {
                    item.Measure(itemSize);
                    maxHeight = Math.Max(maxHeight, item.DesiredSize.Height);
                }
            }
            var scroll = this.FindParentElementOfType<ScrollViewer>();
            double actualwidth;
            if (schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
            {
                int leafcntofsingleitem = CalculateLeafChildCount(schedule.ScheduleResourceType);
                if (schedule.ScheduleType == ScheduleType.Day)
                {
                    double resourceColumns;
                    leafcntofsingleitem = leafcntofsingleitem * schedule.ScheduleResourceType.ResourceCollection.Count;
                    if (schedule.DayViewColumnCount > 0)
                    {
                        resourceColumns = schedule.DayViewColumnCount > leafcntofsingleitem ? leafcntofsingleitem : schedule.DayViewColumnCount;
                    }
                    else
                    {
                        if (scroll.ViewportWidth != 0 && scroll.ViewportWidth / leafcntofsingleitem <= schedule.MinResourceWidth)
                            resourceColumns = Math.Floor(scroll.ViewportWidth / schedule.MinResourceWidth);
                        else
                            resourceColumns = leafcntofsingleitem;
                    }
                    actualwidth = (scroll.ViewportWidth / resourceColumns) * leafcntofsingleitem;
                }
                else
                {
                    actualwidth = scroll.ViewportWidth * (schedule.ScheduleResourceType.ResourceCollection.Count * leafcntofsingleitem);
                }
            }
            else
            {
                actualwidth = scroll.ViewportWidth;
            }
            if (double.IsInfinity(itemSize.Width))
            {
                itemSize.Width = Math.Round(actualwidth / (visibleItemsCount), 0);
                itemSize.Height = maxHeight;
                foreach (UIElement item in Children)
                {
                    if (item.Visibility != Visibility.Collapsed)
                    {
                        item.Measure(itemSize);
                    }
                }
            }

            if (double.IsInfinity(availableSize.Width))
            {
                availableSize.Width = actualwidth;
            }

            if (double.IsInfinity(availableSize.Height))
            {
                availableSize.Height = maxHeight;
            }

            return availableSize;
        }

        #endregion

        #region ArrangeOverride

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count <= 0)
            {
                return finalSize;
            }

#if WPF
            int visibleItemsCount = Children.Cast<UIElement>().Count(child => child.Visibility != Visibility.Collapsed);
#else
            int visibleItemsCount = Children.Count(child => child.Visibility != Visibility.Collapsed);
#endif

            double itemWidth = finalSize.Width / visibleItemsCount;
            var itemRect = new Rect(0, 0, itemWidth, finalSize.Height);
            foreach (UIElement item in Children)
            {
                if (item.Visibility != Visibility.Collapsed)
                {
                    item.Arrange(itemRect);
                    itemRect.X += itemWidth;
                }
            }
            return finalSize;
        }

        #endregion

        #endregion
    }
}

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
    /// Represents a layout panel for arranging month view headers.
    /// </summary>
    public class ScheduleMonthViewHeaderItemsLayoutPanel : Panel
    {
        #region Constructor

        public ScheduleMonthViewHeaderItemsLayoutPanel()
        {
#if WINRT
             ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateInertia;
             ManipulationDelta += ScheduleMonthViewHeaderItemsLayoutPanel_ManipulationDelta;
#endif
        }

        #endregion

        #region Private Members

        double maxElementWidth = 0.0;

        #endregion

        #region Events

#if WINRT
        private void ScheduleMonthViewHeaderItemsLayoutPanel_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
                return;
            SfSchedule schedule = this.FindParentElementOfType<SfSchedule>();
            ScheduleMonthView monthview = this.FindParentElementOfType<ScheduleMonthView>();
            if (Children.Count > 0 && Children[0].GetType() == typeof(ScheduleMonthViewHeaderControl))
            {
                schedule.ScrollManipulationCompleted = true;
                if (e.IsInertial)
                {
                    e.Complete();
                }
            }
        }
#endif

        #endregion

        #region Overrides

        #region MeasureOverride

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
            double maxHeight = 0.0;
            double width = 0.0;
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
            }
            if (schedule != null)
            {
                schedule.needAutoFormat = (itemSize.Width <= maxElementWidth);
            }
            foreach (UIElement item in Children)
            {
                if (item.Visibility != Visibility.Collapsed)
                {
                    item.Measure(itemSize);
                    width += double.IsInfinity(itemSize.Width) ? item.DesiredSize.Width : itemSize.Width;
                    maxHeight = Math.Max(maxHeight, item.DesiredSize.Height);
                    if (schedule != null)
                    {
                        ScheduleMonthViewHeaderControl headerControl = item as ScheduleMonthViewHeaderControl;
                        headerControl.DayText = (schedule.EnableAutoFormat && schedule.needAutoFormat) ? headerControl.DayOfWeek.Substring(0, 3) : headerControl.DayOfWeek;
                    }
                }
            }

            if (double.IsInfinity(itemSize.Width))
            {
                itemSize.Width = width / visibleItemsCount;
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
                availableSize.Width = width;
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

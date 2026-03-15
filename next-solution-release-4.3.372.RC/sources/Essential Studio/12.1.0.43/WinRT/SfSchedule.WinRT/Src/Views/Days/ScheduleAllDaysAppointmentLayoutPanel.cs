#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Linq;
#if WINRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
#else
using System.Windows.Controls;
using System.Windows;
using System;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a layout panel for arranging all day appointments.
    /// </summary>
    public class ScheduleAllDaysAppointmentLayoutPanel : Panel
    {
        #region Public Fields

        public const double AllDayAppiontmentHeight = 30d;

        #endregion

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

        #region Override Methods

        #region MeasureOverride

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count == 0)
            {
                return new Size();
            }

            var itemsContainer = ((FrameworkElement)Children[0]).FindParentElementOfType<ScheduleAllDaysAppointmentItemsControl>();
            if (itemsContainer == null)
            {
                return new Size();
            }
            double totalHeight = 0.0;
            foreach (AllDayAppointmentItemscontrol element in Children)
            {
#if WPF
                var counterHeight = element.Items.Cast<object>().Sum(childelement => AllDayAppiontmentHeight);
#else
                var counterHeight = element.Items.Sum(childelement => AllDayAppiontmentHeight);
#endif
                if (counterHeight > 120)
                {
                    counterHeight = 120;
                }
                totalHeight = Math.Max(totalHeight, counterHeight);
            }
            //To show the AllDay items control even when all day appointment is not present inside it. For UI improvement.
            if (totalHeight.Equals(0))
            {
                totalHeight = 30;
            }
            if (double.IsInfinity(availableSize.Width))
            {
                availableSize.Width = 0;
            }
            if (double.IsInfinity(availableSize.Height))
            {
                availableSize.Height = totalHeight;
            }

            foreach (UIElement element in Children)
            {
                element.Measure(availableSize);
            }
            return availableSize;
        }

        #endregion

        #region ArrangeOverride

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
            {
                return new Size(0, 0);
            }

            var itemsContainer = ((FrameworkElement)Children[0]).FindParentElementOfType<ScheduleAllDaysAppointmentItemsControl>();
            var scheduleday = ((FrameworkElement)Children[0]).FindParentElementOfType<ScheduleDaysView>();
            var schedule = ((FrameworkElement)Children[0]).FindParentElementOfType<SfSchedule>();
            if (itemsContainer == null)
            {
                return new Size(0, 0);
            }

            double resourcecount = 1;
            if (schedule != null && schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
            {
                resourcecount = CalculateLeafChildCount(schedule.ScheduleResourceType);
            }
            var itemWidth = finalSize.Width / (scheduleday.SelectedDates.Count * resourcecount);
            var colIdx = 0;
            foreach (UIElement element in Children)
            {
                var rect = new Rect((itemWidth * colIdx), 0, itemWidth, finalSize.Height);
                element.Arrange(rect);
                colIdx++;
            }

            return finalSize;
        }

        #endregion

        #endregion
    }
}

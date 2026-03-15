#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#else
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents an items control for arranging month view headers.
    /// </summary>
    public class ScheduleMonthViewHeaderItemsControl : ItemsControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleMonthViewHeaderItemsControl">ScheduleMonthViewHeaderItemsControl</see>
        /// class.
        /// </summary>
        public ScheduleMonthViewHeaderItemsControl()
        {
            DefaultStyleKey = typeof(ScheduleMonthViewHeaderItemsControl);
        }

        #endregion

        #region Dependency Properties

        internal bool EnableAutoFormat
        {
            get { return (bool)GetValue(EnableAutoFormatProperty); }
            set { SetValue(EnableAutoFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableAutoFormat.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty EnableAutoFormatProperty =
            DependencyProperty.Register("EnableAutoFormat", typeof(bool), typeof(ScheduleMonthViewHeaderItemsControl), new PropertyMetadata(true, OnEnableAutoFormatChanged));

        private static void OnEnableAutoFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleMonthViewHeaderItemsControl)
            {
                var scheduleMonthViewHeaderItemsControl = (d as ScheduleMonthViewHeaderItemsControl);
                if (scheduleMonthViewHeaderItemsControl.schedule != null && scheduleMonthViewHeaderItemsControl.Items != null)
                {
                    var schedule = scheduleMonthViewHeaderItemsControl.schedule;
                    foreach (ScheduleMonthViewHeaderControl item in scheduleMonthViewHeaderItemsControl.Items)
                    {
                        item.DayText = (schedule.EnableAutoFormat && schedule.needAutoFormat) ? item.DayOfWeek.Substring(0, 3) : item.DayOfWeek;
                    }
                }
            }
        }

        #endregion

        #region Private Members

        SfSchedule schedule;

        #endregion

        #region Methods

        #region Generate Days

        private void GenerateDays()
        {
            DateTimeFormatInfo dateTimeFormat = GetCurrentDateFormat();
            List<string> dayNames = dateTimeFormat.DayNames.ToList();
            if (Items != null)
            {
                Items.Clear();
                for (int i = 0; i < dayNames.Count(); i++)
                {
                    var item = (ScheduleMonthViewHeaderControl)GetContainerForItemOverride();
                    item.DayText = dayNames[i];
                    item.DayOfWeek = dayNames[i];
                    Items.Add(item);
                }
            }
        }

        #endregion

        #region Get Current Date Format

        private DateTimeFormatInfo GetCurrentDateFormat()
        {
            foreach (var cal in CultureInfo.CurrentCulture.OptionalCalendars)
            {
                DateTimeFormatInfo dateTimeFormatInfo = new CultureInfo(CultureInfo.CurrentCulture.Name).DateTimeFormat;
                dateTimeFormatInfo.Calendar = cal;
                return dateTimeFormatInfo;
            }

            DateTimeFormatInfo dateTime = new CultureInfo(CultureInfo.InvariantCulture.Name).DateTimeFormat;
            return dateTime;
        }

        #endregion

        #endregion

        #region Overrides

        #region GetContainerForItemOverride

        protected override DependencyObject GetContainerForItemOverride()
        {
            var headerContent = new ScheduleMonthViewHeaderControl();
#if WINRT
            if (ItemContainerStyle != null)
            {
                headerContent.Style = ItemContainerStyle;
            } 
#endif
            return headerContent;
        }

        #endregion

        #region IsItemItsOwnContainerOverride

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ScheduleMonthViewHeaderControl);
        }

        #endregion

        #region OnApplyTemplate

#if WINRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            GenerateDays();
            schedule = this.FindParentElementOfType<SfSchedule>();
            if (schedule != null)
            {
                var enableAutoFormatBinding = new Binding { Source = schedule, Path = new PropertyPath("EnableAutoFormat") };
                SetBinding(ScheduleMonthViewHeaderItemsControl.EnableAutoFormatProperty, enableAutoFormatBinding);
            }
        }

        #endregion

        #endregion
    }
}

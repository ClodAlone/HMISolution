#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design;
using Syncfusion.Windows.Controls.Schedule;

#if SyncfusionFramework4_0
[assembly: ProvideMetadata(typeof(Syncfusion.Schedule.WPF.dll.Design.Metadata))]
#endif

namespace Syncfusion.Schedule.WPF.dll.Design
{
#if SyncfusionFramework4_0
    internal class Metadata : IProvideAttributeTable
    {
        #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();

                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.Schedule), new ToolboxBrowsableAttribute(true));

                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.ChildWindow), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Calendar), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.DatePicker), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Primitives.CalendarDayButton), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Primitives.CalendarItem), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Primitives.CalendarButton), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.CalendarBlackoutDatesCollection), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.CalendarDateChangedEventArgs), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.CalendarDateRange), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.CalendarModeChangedEventArgs), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ContentViewItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.MaxHeightPanel), ToolboxBrowsableAttribute.No);

                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Primitives.DatePickerTextBox), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.DatePickerDateValidationErrorEventArgs), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.SelectedDatesCollection), ToolboxBrowsableAttribute.No);

                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleDaysView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAllDaysAppointmentItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAllDaysAppointmentLayoutPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleDaysAppointmentLayoutItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleDaysAppointmentLayoutPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleDaysAppointmentViewControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleDaysHeaderViewControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleDaysHeaderViewItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleTimeLineHourControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleTimeLineItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleTimeSlotControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleTimeSlotItemsControl), ToolboxBrowsableAttribute.No);

                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthAppointmentLayoutItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthAppointmentLayoutPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthAppointmentViewControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthDateContentControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthViewHeaderControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthViewHeaderItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthViewItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthViewSideContentControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthViewSideItemsControl), ToolboxBrowsableAttribute.No);

                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleButton), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentEditorControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAllDayCheckedToEnabledConverter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleCalendarViewItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHeaderTitleBar), ToolboxBrowsableAttribute.No);               
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleRectangleBorder), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleRectangleBorderExt), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleReminderControl), ToolboxBrowsableAttribute.No);

                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAppointment), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentCollection), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentMapping), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentProxy), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentWrapper), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleCalendarViewModel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleReminderEventArgs), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleReminderWrapper), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalAppointmentLayoutItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalAppointmentLayoutPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalAppointmentViewControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalDaysHeaderViewControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalDaysHeaderViewItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalRectangleSel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalTimeLineHourControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalTimeLineItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalTimeSlotControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalTimeSlotItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleRecurrenceConfirmationWindow), ToolboxBrowsableAttribute.No);

                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.UniformGrid), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.UniformStackPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.UniformTimeSlotPanel), ToolboxBrowsableAttribute.No);



                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleDaysHeaderViewLayoutPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthViewHeaderItemsLayoutPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthViewSideItemsLayoutPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleSideTextBlock), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentNavigatorControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleCrosslineControl), ToolboxBrowsableAttribute.No);

                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleToggleButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalRectangleBorderExt), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalRectangleBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ColorButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.GoToDateWindow), new ToolboxBrowsableAttribute(false));
                return builder.CreateTable();
            }
        }

        #endregion
    }
#elif SyncfusionFramework3_5

    internal class Metadata : IRegisterMetadata
    {
    #region IRegisterMetadata Members
        /// <summary>
        /// Attaches design-time metadata to a particular control type.
        /// </summary>
        /// 
        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();

            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.Schedule), new ToolboxBrowsableAttribute(true));

            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Primitives.CalendarDayButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Primitives.CalendarItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Primitives.DatePickerTextBox), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Primitives.CalendarButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.MaxHeightPanel), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Calendar), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.ChildWindow), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.DatePicker), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ContentViewItemsControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAllDaysAppointmentItemsControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAllDaysAppointmentLayoutPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAppointment), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentCollection), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentEditorControl), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentEventArgs), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentMapping), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentProxy), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentWrapper), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleCalendarViewItemsControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleCalendarViewModel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleDaysAppointmentLayoutItemsControl), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleDaysAppointmentLayoutPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleDaysAppointmentViewControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleDaysHeaderViewControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleDaysHeaderViewItemsControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleDaysView), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHeaderTitleBar), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalAppointmentLayoutItemsControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalAppointmentLayoutPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalAppointmentViewControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalDaysHeaderViewControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalDaysHeaderViewItemsControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalRectangleSel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalTimeLineHourControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalTimeLineItemsControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalTimeSlotControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalTimeSlotItemsControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalView), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthAppointmentLayoutItemsControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthAppointmentLayoutPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthAppointmentViewControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthDateContentControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthView), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthViewHeaderControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthViewHeaderItemsControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthViewItemsControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthViewSideContentControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthViewSideItemsControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleRectangleBorder), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleRectangleBorderExt), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleRecurrenceConfirmationWindow), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleRecurrenceIsCheckedConverter), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleReminderControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleReminderEventArgs), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleReminderWrapper), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleTimeLineHourControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleTimeLineItemsControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleTimeSlotControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleTimeSlotItemsControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.SelectedIndexToVisibilityConverter), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.UniformGrid), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.UniformStackPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.UniformTimeSlotPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleDaysHeaderViewLayoutPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthViewHeaderItemsLayoutPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleMonthViewSideItemsLayoutPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleSideTextBlock), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentNavigatorControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleCrosslineControl), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleToggleButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalRectangleBorderExt), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalRectangleBorder), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ColorButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.GoToDateWindow), new ToolboxBrowsableAttribute(false));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    #endregion
    }
#endif
}

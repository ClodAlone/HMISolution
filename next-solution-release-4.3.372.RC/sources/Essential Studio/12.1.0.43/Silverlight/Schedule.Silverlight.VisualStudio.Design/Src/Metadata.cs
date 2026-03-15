#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using Microsoft.Windows.Design.Metadata;
#if SyncfusionFramework4_0||SyncfusionSLFramework4_0
using Microsoft.Windows.Design.Features;
[assembly: Microsoft.Windows.Design.Metadata.ProvideMetadata(typeof(Syncfusion.Schedule.Silverlight.VisualStudio.Design.Metadata))]
#endif

namespace Syncfusion.Schedule.Silverlight.VisualStudio.Design
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Windows.Design.Metadata;
    using System.Text;
    using Microsoft.Windows.Design;
    using Syncfusion.Windows.Controls.Schedule;
    using Microsoft.Windows.Design.PropertyEditing;

#if SyncfusionFramework4_0 || SyncfusionSLFramework4_0 || SyncfusionFramework4_5

    internal class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();

                // tool box filtering
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.Schedule), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.MaxHeightPanel), new ToolboxBrowsableAttribute(false));
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
                
                // Category Editor

                return builder.CreateTable();
            }
        }
    }

#else
    internal class ScheduleMetadata : IRegisterMetadata
    {
        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();

            // tool box filtering
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.Schedule), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.MaxHeightPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ContentViewItemsControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAllDaysAppointmentItemsControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAllDaysAppointmentLayoutPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentEditorControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentMapping), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Schedule.ScheduleCalendarViewItemsControl), new ToolboxBrowsableAttribute(false));
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
            
            // Category Editor

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}

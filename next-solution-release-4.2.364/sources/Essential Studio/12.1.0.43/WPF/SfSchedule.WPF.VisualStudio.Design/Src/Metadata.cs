#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Windows.Design.Metadata;
using System.ComponentModel;
using Microsoft.Windows.Design.PropertyEditing;
using System.Media;
using Microsoft.Windows.Design;
using Syncfusion.UI.Xaml.Schedule;
using System.Windows.Controls;

[assembly: ProvideMetadata(typeof(Syncfusion.SfSchedule.WPF.VisualStudio.Design.Metadata))]

namespace Syncfusion.SfSchedule.WPF.VisualStudio.Design
{
    internal class Metadata : IProvideAttributeTable
    {
        #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {

                AttributeTableBuilder builder = new AttributeTableBuilder();

                // tool box filtering
                builder.AddCustomAttributes(typeof(Clip), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SelectedDatesToHeaderTextConverter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(BoolToVisibilityConverter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(BoolToBoolConverter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(CustomTextBlock), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(MethodInvokerCache), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(MethodInvokerFactory), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PropertyAccessorCache), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PropertyAccessor), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(MethodInvoker), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(UniformGrid), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(UniformStackPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(UniformTimeSlotPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(HeaderTitleBarView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(RecurrenceProperties), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Resource), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ResourceType), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleAppointment), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleAppointmentCollection), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleAppointmentMapping), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleAppointmentStatus), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleAppointmentStatusCollection), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleHelper), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.UI.Xaml.Schedule.TimeZone), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(TimeZoneCollection), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(AllDayAppointmentItemscontrol), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(CollapsedScheduleAppointment), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(DayViewItemHeader), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleAllDaysAppointmentItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleAllDaysAppointmentLayoutPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleDaysAppointmentLayoutItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleDaysAppointmentLayoutPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(AppointmentPostionInfo), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleDaysAppointmentViewControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleDaysHeaderViewControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleDaysHeaderViewItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleDaysHeaderViewLayoutPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleDaysHeaderViewLayoutPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleDaysView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleHorizontalTimeSlotItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleNonWorkingDayItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleNonWorkingDayPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleTimeLineItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleVerticalTimeSlotItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(MonthViewItem), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(MonthViewItemHeader), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleMonthAppointmentLayoutItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleMonthAppointmentLayoutPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleMonthAppointmentViewControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleMonthDateContentControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleMonthView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleMonthViewHeaderControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleMonthViewHeaderItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleMonthViewHeaderItemsLayoutPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleMonthViewItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleRectangleBorder), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleHorizontalAppointmentLayoutItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleHorizontalAppointmentViewControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleHorizontalDaysHeaderViewControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleHorizontalDaysHeaderViewItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleHorizontalTimeLineItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleHorizontalTimeSlotControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleTimeLineNonWorkingDayPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleTimelineTimeSlotItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleTimeLineView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(TimeLineViewItemHeader), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleHorizontalAppointmentLayoutPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(AppointmentEditor), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(OpenRecurringAppointment), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScheduleReminderControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(AddAppintmentControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(DragDropControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ResourceHeaderItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(AddRadialMenuControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(EditRadialMenuControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(LoopItemsPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(HeaderedItemsControl), ToolboxBrowsableAttribute.No);
                return builder.CreateTable();

            }
        }

        #endregion
    }
}

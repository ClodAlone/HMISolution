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
using System.Web;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public class ScheduleHeaderOptionsBuilder
    {
        private ScheduleHeaderOptions scheduleHeaderOption = new ScheduleHeaderOptions();

        public ScheduleHeaderOptionsBuilder(ScheduleHeaderOptions scheduleHeader)
        {
            scheduleHeaderOption = scheduleHeader;
        }

        public ScheduleHeaderOptionsBuilder ScheduleView(String scheduleView)
        {
            scheduleHeaderOption.ScheduleView = scheduleView;
            return this;
        }

        public ScheduleHeaderOptionsBuilder WeekHeaderFormat(String weekHeaderFormat)
        {
            scheduleHeaderOption.WeekHeaderFormat = weekHeaderFormat;
            return this;
        }

        public ScheduleHeaderOptionsBuilder DayHeaderFormat(String dayHeaderFormat)
        {
            scheduleHeaderOption.DayHeaderFormat = dayHeaderFormat;
            return this;
        }

        public ScheduleHeaderOptionsBuilder HighlightScheduleWeekend(bool highlightScheduleWeekend)
        {
            scheduleHeaderOption.HighlightScheduleWeekend = highlightScheduleWeekend;
            return this;
        }

        public ScheduleHeaderOptionsBuilder ScheduleWeekendBackground(String scheduleWeekendBackground)
        {
            scheduleHeaderOption.ScheduleWeekendBackground = scheduleWeekendBackground;
            return this;
        }
    }
}
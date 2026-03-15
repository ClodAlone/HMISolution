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
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.Models
{
    public class ScheduleHeaderOptions
    {

        private string scheduleView = "day";
        private string weekHeaderFormat = "dd ddd MMM yyyy";
        private string dayHeaderFormat = "ddd";
        private bool highlightScheduleWeekend = true;
        private string scheduleWeekendBackground = "#F2F2F2";


        [JsonProperty("scheduleView")]

        [DefaultValue(null)]
        public string ScheduleView
        {
            get { return this.scheduleView; }
            set { this.scheduleView = value; }
        }

        [JsonProperty("weekHeaderFormat")]

        [DefaultValue("dd ddd MMM yyyy")]
        public string WeekHeaderFormat
        {
            get { return this.weekHeaderFormat; }
            set { this.weekHeaderFormat = value; }
        }
        [JsonProperty("dayHeaderFormat")]

        [DefaultValue("ddd")]
        public string DayHeaderFormat
        {
            get { return this.dayHeaderFormat; }
            set { this.dayHeaderFormat = value; }
        }

        [JsonProperty("highlightScheduleWeekend")]

        [DefaultValue(true)]
        public bool HighlightScheduleWeekend
        {
            get { return this.highlightScheduleWeekend; }
            set { this.highlightScheduleWeekend = value; }
        }

        [JsonProperty("scheduleWeekendBackground")]
        [DefaultValue("#F2F2F2")]
        public string ScheduleWeekendBackground
        {
            get { return this.scheduleWeekendBackground; }
            set { this.scheduleWeekendBackground = value; }
        }
        
    }
}

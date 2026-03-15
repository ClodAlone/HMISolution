#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    public interface IDateTimePickerCalendarSource
    {
        IDateTimePickerAdvCalendar GetCalendar();
    }

    /// <summary>
    /// Interface used by DateTimePickerAdv. If the DateTimePickerAdv customDrop is true and the 
    /// CustomPopupWindow is the interface`s parent and Active is true then the interface will communicate with the picker through events.
    /// </summary>
    public interface IDateTimePickerAdvCalendar
    {
        /// <summary>
        /// Gets or sets a value indicating whether the interface`s events are to be considered by the DateTimePickerAdv.
        /// </summary>
        bool Active
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the font used to draw the calendar that implements the interface.
        /// </summary>
        Font CalendarFont
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color used to draw the foreground of calendar that implements the interface.
        /// </summary>
        Color CalendarForeColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color used to draw the month background of calendar that implements the interface.
        /// </summary>
        Color CalendarMonthBackground
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color used to draw the title background of calendar that implements the interface.
        /// </summary>
        Color TitleBackColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color used to draw the foreground of the title of calendar that implements the interface.
        /// </summary>
        Color TitleForeColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color used to draw the trailing foreground of calendar that implements the interface.
        /// </summary>
        Color TrailingForeColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minimum date of the calendar that implements the interface.
        /// </summary>
        DateTime MinDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum date of the calendar that implements the interface.
        /// </summary>
        DateTime MaxDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the date of the calendar that implements the interface.
        /// </summary>
        DateTime Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the culture of the calendar that implements the interface.
        /// </summary>
        System.Globalization.CultureInfo Culture
        {
            get;
            set;
        }

        /// <summary>
        /// Fired when the null button of the calendar that implements the interface is clicked.
        /// </summary>
        event DateTimePickerAdv.NullButtonEventHandler NullButtonDown;

        /// <summary>
        /// Fired when a date is selected in the calendar that implements the interface.
        /// </summary>
        event DateTimePickerAdv.SelectDateEventHandler SelectDate;

        /// <summary>
        /// Fired when the date is changed in the calendar that implements the interface.
        /// </summary>
        event DateTimePickerAdv.DateChangedEventHandler DateChange;
        /* /// <summary>
                /// Fired when the null button of the calendar that implements the interface is clicked.
                /// </summary>
                event EventHandler NullButtonDown;
                /// <summary>
                /// Fired when a date is selected in the calendar that implements the interface.
                /// </summary>
                event EventHandler SelectDate;
                /// <summary>
                /// Fired when the date is changed in the calendar that implements the interface.
                /// </summary>
               event EventHandler DateChange;
        */
    }
    public class DateTimePickerAdvCalendarAdapter : IDateTimePickerAdvCalendar
    {
        private MonthCalendar monthCalendarObject;
        private bool bactive;

        public DateTimePickerAdvCalendarAdapter(MonthCalendar calendar)
        {
            this.monthCalendarObject = calendar;

            this.monthCalendarObject.DateSelected += new System.Windows.Forms.DateRangeEventHandler(OnDateSelected);
            this.monthCalendarObject.DateChanged += new System.Windows.Forms.DateRangeEventHandler(OnDateChanged);

            this.monthCalendarObject.AnnuallyBoldedDates = new System.DateTime[0];
            this.monthCalendarObject.BoldedDates = new System.DateTime[0];
            this.monthCalendarObject.MonthlyBoldedDates = new System.DateTime[0];
            this.monthCalendarObject.SelectionRange = new System.Windows.Forms.SelectionRange(new System.DateTime(2002, 10, 26, 0, 0, 0, 0), new System.DateTime(2002, 10, 26, 0, 0, 0, 0));
        }

        public bool Active
        {
            get { return bactive; }
            set { bactive = value; }
        }

        public Font CalendarFont
        {
            get { return this.monthCalendarObject.Font; }
            set { this.monthCalendarObject.Font = value; }
        }

        public Color CalendarForeColor
        {
            get { return this.monthCalendarObject.ForeColor; }
            set { this.monthCalendarObject.ForeColor = value; }
        }

        public Color CalendarMonthBackground
        {
            get { return this.monthCalendarObject.BackColor; }
            set { this.monthCalendarObject.BackColor = value; }
        }

        public DateTime Value
        {
            get { return this.monthCalendarObject.SelectionStart; }
            set { this.monthCalendarObject.SelectionStart = this.monthCalendarObject.SelectionEnd = value; }
        }

        public event DateTimePickerAdv.NullButtonEventHandler NullButtonDown;
        public event DateTimePickerAdv.SelectDateEventHandler SelectDate;
        public event DateTimePickerAdv.DateChangedEventHandler DateChange;
        /*
                public event EventHandler NullButtonDown;
                public event EventHandler SelectDate;
                public event EventHandler DateChange;

        */
        protected void OnDateSelected(object sender, System.Windows.Forms.DateRangeEventArgs e)
        {
            if (SelectDate != null)
            {
                SelectDate(this, EventArgs.Empty);
            }
        }
        protected void OnDateChanged(object sender, System.Windows.Forms.DateRangeEventArgs e)
        {
            if (DateChange != null)
            {
                DateChange(this, EventArgs.Empty);
            }
        }

        public System.Globalization.CultureInfo Culture
        {
            get { return System.Globalization.CultureInfo.CurrentCulture; }
            set { }
        }

        public void FireNullEvent()
        {
            if (NullButtonDown != null)
            {
                NullButtonDown(this, EventArgs.Empty);
            }
        }

        public DateTime MinDate
        {
            get { return this.monthCalendarObject.MinDate; }
            set { this.monthCalendarObject.MinDate = value; }
        }

        public DateTime MaxDate
        {
            get { return this.monthCalendarObject.MaxDate; }
            set { this.monthCalendarObject.MaxDate = value; }
        }

        public Color TitleForeColor
        {
            get { return this.monthCalendarObject.TitleForeColor; }
            set { this.monthCalendarObject.TitleForeColor = value; }
        }

        public Color TitleBackColor
        {
            get { return this.monthCalendarObject.TitleBackColor; }
            set { this.monthCalendarObject.TitleBackColor = value; }
        }

        public Color TrailingForeColor
        {
            get { return this.monthCalendarObject.TrailingForeColor; }
            set { this.monthCalendarObject.TrailingForeColor = value; }
        }
    }
}

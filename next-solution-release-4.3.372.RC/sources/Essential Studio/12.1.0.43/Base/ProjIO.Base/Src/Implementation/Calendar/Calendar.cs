#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;

namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Represents a Calendar
    /// </summary>
    [System.Serializable()]
    public class Calendar
    {
        #region Fields
        private int m_uID;
        private string m_name;
        private bool m_bisBaseCalendar;
        private bool m_bisBaselineCalendar;
        private int m_baseCalendarUID;
        private List<WeekDay> m_weekDays = new List<WeekDay>();
        private List<CalendarException> m_exceptions = new List<CalendarException>();
        private List<WorkWeek> m_workWeeks = new List<WorkWeek>();
        #endregion

        #region Initializer

        public Calendar() { }

        public Calendar(string name)
        {
            this.Name = name;
        }

        #endregion

        #region Properties
        /// <summary>        
        /// Gets or sets the unique identifier of the calendar
        /// </summary>
        [XmlIgnore]
        public int UID
        {
            get
            {
                return this.m_uID;
            }
            set
            {
                this.m_uID = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("UID")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string UIDSerialized
        {
            get
            {
                return UID.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_uID);
            }
        }

        /// <summary>
        /// Gets or sets the name of the calendar
        /// </summary>
        [XmlElement("Name", DataType = "string")]
        public string Name
        {
            get
            {
                return this.m_name;
            }
            set
            {
                this.m_name = value;
            }
        }

        /// <summary>
        /// Checks whether the calendar is a base calendar
        /// </summary>
        [XmlIgnore()]
        public bool IsBaseCalendar
        {
            get
            {
                return this.m_bisBaseCalendar;
            }
            set
            {
                this.m_bisBaseCalendar = value;
            }
        }

        [XmlElement("IsBaseCalendar")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsBaseCalendarString
        {
            get
            {
                return IsBaseCalendar ? "1" : "0";
            }
            set
            {
                bool ParsedValue;
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                IsBaseCalendar = ParsedValue;
            }
        }

        /// <summary>
        /// Checks whether the calendar is a baseline calendar
        /// </summary>
        [XmlIgnore()]
        public bool IsBaselineCalendar
        {
            get
            {
                return this.m_bisBaselineCalendar;
            }
            set
            {
                this.m_bisBaselineCalendar = value;
            }
        }
        
        [XmlElement("IsBaselineCalendar")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsBaselineCalendarString
        {
            get
            {
                return this.m_bisBaselineCalendar ? "1" : "0";
            }
            set
            {
                this.m_bisBaselineCalendar = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the base calendar on which this calendar depends
        /// </summary>
        [XmlIgnore]
        public int BaseCalendarUID
        {
            get
            {
                return this.m_baseCalendarUID;
            }
            set
            {
                this.m_baseCalendarUID = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("BaseCalendarUID")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string BaseCalendarUIDSerialized
        {
            get
            {
                return BaseCalendarUID.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_baseCalendarUID);
            }
        }
        /// <summary>
        /// The collection of weekdays that defines this calendar
        /// </summary>
        [XmlArrayItem("WeekDay", IsNullable = false)]
        public List<WeekDay> WeekDays
        {
            get
            {
                return this.m_weekDays;
            }
            set
            {
                this.m_weekDays = value;
            }
        }

        /// <summary>
        /// The collection of exceptions that is associated with the calendar
        /// </summary>
        [XmlElement("CalendarException")]
        public List<CalendarException> Exceptions
        {
            get
            {
                return this.m_exceptions;
            }
            set
            {
                this.m_exceptions = value;
            }
        }

        /// <summary>
        /// The collection of effective work weeks associated with the calendar
        /// </summary>
        [XmlElement("WorkWeek")]
        public List<WorkWeek> WorkWeeks
        {
            get
            {
                return this.m_workWeeks;
            }
            set
            {
                this.m_workWeeks = value;
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Add default calendar
        /// </summary>
        public static Calendar StandardCalendar()
        {
            return StandardCalendar("Standard");
        }

        /// <summary>
        /// Add Default calendar
        /// </summary>
        /// <param name="name">Name of the Calendar</param>
        /// <returns>Instance of calendar created</returns>
        public static Calendar StandardCalendar(string name)
        {
            Calendar c = new Calendar();
            c.UID = 1;
            c.Name = name;
            c.IsBaseCalendar = true;
            c.IsBaselineCalendar = false;
            c.BaseCalendarUID = -1;
            c.WeekDays.Add(new WeekDay(DayType.Sunday));
            c.WeekDays.Add(WeekDay.DefaultWorkTiming(DayType.Monday));
            c.WeekDays.Add(WeekDay.DefaultWorkTiming(DayType.Tuesday));
            c.WeekDays.Add(WeekDay.DefaultWorkTiming(DayType.Wednesday));
            c.WeekDays.Add(WeekDay.DefaultWorkTiming(DayType.Thursday));
            c.WeekDays.Add(WeekDay.DefaultWorkTiming(DayType.Friday));
            c.WeekDays.Add(new WeekDay(DayType.Saturday));
            return c;
        }

        #endregion
    }
}

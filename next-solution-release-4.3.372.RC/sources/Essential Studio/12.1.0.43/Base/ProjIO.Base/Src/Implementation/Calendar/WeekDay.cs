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

namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Represents a Week day
    /// </summary>
    [System.Serializable()]
    public class WeekDay
    {
        #region Fields
        private DayType m_dayType;
        private bool m_bdayWorking;
        private bool dayWorkingFieldSpecified;
        private TimePeriod m_timePeriod;
        private WorkingTimes m_workingTimes;
        #endregion
        
        #region Initializer
        /// <summary>
        /// Default Constructor
        /// </summary>
        public WeekDay() { }

        /// <summary>
        /// Create an instance of WeekDay
        /// </summary>
        /// <param name="type">Type of day</param>
        public WeekDay(DayType type)
        {
            this.m_dayType = type;
            if ((this.m_dayType == DayType.Sunday) || (this.m_dayType == DayType.Saturday))
                this.m_bdayWorking = false;
            else
                this.m_bdayWorking = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets os sets the type of day
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "DayType")]
        public DayType DayType
        {
            get
            {
                return this.m_dayType;
            }
            set
            {
                this.m_dayType = value;
            }
        }

        /// <summary>
        /// Checks whether the specified date or day type is working
        /// </summary>
        [XmlIgnore()]
        public bool DayWorking 
        {
            get
            {
                return this.m_bdayWorking;
            }
            set
            {
                this.m_bdayWorking = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement(ElementName = "DayWorking")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DayWorkingString
        {
            get
            {
                return m_bdayWorking ? "1" : "0";
            }
            set
            {
                this.m_bdayWorking = XmlConvert.ToBoolean(value);
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DayWorkingSpecified
        {
            get
            {
                return this.dayWorkingFieldSpecified;
            }
            set
            {
                this.dayWorkingFieldSpecified = value;
            }
        }
        /// <summary>
        /// Gets or sets a contiguous set of exception days
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="TimePeriod")]
        public TimePeriod TimePeriod
        {
            get
            {
                return this.m_timePeriod;
            }
            set
            {
                this.m_timePeriod = value;
            }
        }

        /// <summary>
        /// Gets or sets the collection of working times that define the time worked on the weekday
        /// </summary>
        [System.Xml.Serialization.XmlElement("WorkingTimes")]
        public WorkingTimes WorkingTimes
        {
            get
            {
                return this.m_workingTimes;
            }
            set
            {
                this.m_workingTimes = value;
            }
        }
        #endregion

        #region Methods
        public static WeekDay DefaultWorkTiming(DayType day)
        {
            WeekDay Day = new WeekDay();
            Day.DayType = day;
            Day.DayWorking = true;
            List<WorkingTime> WorkingTime = new List<WorkingTime>();
            WorkingTime Time1 = new WorkingTime();
            Time1.FromTime = new TimeSpan(8, 0, 0);
            Time1.ToTime = new TimeSpan(12, 0, 0);
            WorkingTime.Add(Time1);
            WorkingTime Time2 = new WorkingTime();
            Time2.FromTime = new TimeSpan(13, 0, 0);
            Time2.ToTime = new TimeSpan(17, 0, 0);
            WorkingTime.Add(Time2);
            Day.WorkingTimes = new WorkingTimes();
            Day.WorkingTimes.Items = new List<WorkingTime>();
            Day.WorkingTimes.Items = WorkingTime;
            return Day;
        }
        #endregion
    }
}

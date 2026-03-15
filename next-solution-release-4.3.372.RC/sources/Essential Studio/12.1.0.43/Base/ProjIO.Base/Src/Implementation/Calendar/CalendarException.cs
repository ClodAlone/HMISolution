#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;

namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Defines a Calendar exception
    /// </summary>
    [System.Serializable()]
    public class CalendarException
    {
        #region Fields
        private bool m_benteredByOccurrences;
        private TimePeriod m_timePeriod;
        private int m_occurrences;
        private string m_name;
        private ExceptionType m_type;
        private bool m_btypeSpecified;
        private int m_period;
        private int m_daysOfWeek;
        private ExceptionMonthItem m_monthItem;
        private bool m_bmonthItemSpecified;
        private ExceptionMonthPosition m_monthPosition;
        private bool m_bmonthPositionSpecified;
        private ExceptionMonth m_month;
        private bool m_bmonthSpecified;
        private int m_monthDay;
        private bool m_bdayWorking;
        private WorkingTimes m_workingTimes;
        #endregion

        #region Properties
        /// <summary>
        /// Checks whether the range of recurrence is defined by entering a number of occurrences
        /// </summary>
        [XmlIgnore()]
        public bool EnteredByOccurences
        {
            get
            {
                return this.m_benteredByOccurrences;
            }
            set
            {
                this.m_benteredByOccurrences = value;
            }
        }

        [XmlElement("EnteredByOccurences")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string EnteredByOccurrencesString
        {
            get
            {
                return this.m_benteredByOccurrences ? "1" : "0";
            }
            set
            {
                this.m_benteredByOccurrences = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Defines a contiguous set of exception days
        /// </summary>
        [XmlElement("TimePeriod")]
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
        /// Gets or sets the number of occurrences for which the calendar exception is valid
        /// </summary>
        [XmlIgnore]
        public int Occurrences
        {
            get
            {
                return this.m_occurrences;
            }
            set
            {
                this.m_occurrences = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("Occurrences")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string OccurrencesSerialized
        {
            get
            {
                return Occurrences.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_occurrences);
            }
        }
        /// <summary>
        /// Gets or sets the name of the exception
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
        /// Gets ot sets the exception type
        /// </summary>
        [XmlElement("Type")]
        public ExceptionType Type
        {
            get
            {
                return this.m_type;
            }
            set
            {
                this.m_type = value;
            }
        }

        /// <summary>
        /// Checks whether the exception type is specified
        /// </summary>
        [XmlIgnoreAttribute()]
        public bool TypeSpecified
        {
            get
            {
                return this.m_btypeSpecified;
            }
            set
            {
                this.m_btypeSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the period of recurrence for the exception
        /// </summary>
        [XmlIgnore]
        public int Period
        {
            get
            {
                return this.m_period;
            }
            set
            {
                this.m_period = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("Period")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string PeriodSerialized
        {
            get
            {
                return Period.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_period);
            }
        }
        /// <summary>
        /// Gets or sets the days of the week on which the exception is valid
        /// </summary>
        [XmlIgnore]
        public int DaysOfWeek
        {
            get
            {
                return this.m_daysOfWeek;
            }
            set
            {
                this.m_daysOfWeek = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("DaysOfWeek")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string DaysOfWeekSerialized
        {
            get
            {
                return DaysOfWeek.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_daysOfWeek);
            }
        }
        /// <summary>
        /// Gets or sets the month item for which an exception recurrence is scheduled
        /// </summary>
        [XmlElement("MonthItem")]
        public ExceptionMonthItem MonthItem
        {
            get
            {
                return this.m_monthItem;
            }
            set
            {
                this.m_monthItem = value;
            }
        }

        /// <summary>
        /// Checks whether the month item for which an exception recurrence is scheduled is specified
        /// </summary>
        [XmlIgnoreAttribute()]
        public bool MonthItemSpecified
        {
            get
            {
                return this.m_bmonthItemSpecified;
            }
            set
            {
                this.m_bmonthItemSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the position of a month item within a month
        /// </summary>
        [XmlElement("MonthPosition")]
        public ExceptionMonthPosition MonthPosition
        {
            get
            {
                return this.m_monthPosition;
            }
            set
            {
                this.m_monthPosition = value;
            }
        }

        /// <summary>
        /// Checks whether the position of a month item within a month is specified
        /// </summary>
        [XmlIgnoreAttribute()]
        public bool MonthPositionSpecified
        {
            get
            {
                return this.m_bmonthPositionSpecified;
            }
            set
            {
                this.m_bmonthPositionSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the month for which an exception recurrence is scheduled
        /// </summary>
        [XmlElement("Month")]
        public ExceptionMonth Month
        {
            get
            {
                return this.m_month;
            }
            set
            {
                this.m_month = value;
            }
        }

        /// <summary>
        /// Checks whether the month for which an exception recurrence is scheduled is specified
        /// </summary>
        [XmlIgnoreAttribute()]
        public bool MonthSpecified
        {
            get
            {
                return this.m_bmonthSpecified;
            }
            set
            {
                this.m_bmonthSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the day of the month on which an exception recurrence is scheduled
        /// </summary>
        [XmlIgnore]
        public int MonthDay
        {
            get
            {
                return this.m_monthDay;
            }
            set
            {
                this.m_monthDay = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("MonthDay")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string MonthDaySerialized
        {
            get
            {
                return MonthDay.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_monthDay);
            }
        }
        /// <summary>
        /// Checks whether the specified date or day type is working
        /// </summary>
        [XmlIgnore()]
        public bool IsDayWorking 
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
        
        [XmlElement("DayWorking")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsDayWorkingString
        {
            get
            {
                return IsDayWorking ? "1" : "0";
            }
            set
            {
                IsDayWorking = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets the collection of working times that define the time worked on the weekday
        /// </summary>
        [XmlElement("WorkingTimes")]
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
    }
}

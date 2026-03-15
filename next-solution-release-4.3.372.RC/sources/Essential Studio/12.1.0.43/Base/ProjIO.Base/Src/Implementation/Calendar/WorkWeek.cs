#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;

namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Represents a Working Week
    /// </summary>
    [System.Serializable()]
    public class WorkWeek
    {
        #region Fields
        private TimePeriod m_timePeriod;
        private string m_name;
        private List<WorkWeekDay> m_weekDay=new List<WorkWeekDay>();
        #endregion

        #region Properties
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
        /// Gets or sets the name of the effective week
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Name",DataType="string")]
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
        /// A weekday either defines regular days of the week or exception days in the calendar
        /// </summary>
        //[System.Xml.Serialization.XmlElement("WeekDay")]
        [System.Xml.Serialization.XmlArrayItem("WorkWeekDay",IsNullable=false)]
        public List<WorkWeekDay> WeekDay
        {
            get
            {
                return this.m_weekDay;
            }
            set
            {
                this.m_weekDay = value;
            }
        }
        #endregion
    }
}

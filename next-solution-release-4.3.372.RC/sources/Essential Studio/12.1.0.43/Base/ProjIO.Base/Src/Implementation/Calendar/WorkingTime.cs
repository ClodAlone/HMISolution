#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Represents a working time period
    /// </summary>
    [System.Serializable()]
    public class WorkingTime
    {
        #region Fields
        private System.TimeSpan m_fromTime;
        private System.DateTime m_fromTimeDate;
        private bool m_bfromTimeSpecified;
        private System.TimeSpan m_toTime;
        private System.DateTime m_toTimeDate;
        private bool m_btoTimeSpecified;
        #endregion
        
        #region Properties
        /// <summary>
        /// Gets or sets the beginning of the working time
        /// </summary>
        [System.Xml.Serialization.XmlElement("FromTime")]
        public string FromTimeString
        {
            get
            {
                return FromTime.ToString();
            }
            set
            {
                FromTime = TimeSpan.Parse(value);
            }
        }

        [System.Xml.Serialization.XmlIgnore()]
        public System.TimeSpan FromTime
        {
            get
            {
                return this.m_fromTime;
            }
            set
            {
                this.m_fromTime = value;
                this.m_fromTimeDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, value.Hours, value.Minutes, value.Seconds);
            }
        }

        
        /// <summary>
        /// Checks whether the beginning of the working time is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool FromTimeSpecified
        {
            get
            {
                return this.m_bfromTimeSpecified;
            }
            set
            {
                this.m_bfromTimeSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the end of the working time
        /// </summary>
        [System.Xml.Serialization.XmlElement("ToTime")]
        public string ToTimeString
        {
            get
            {
                return ToTime.ToString();
            }
            set
            {
                ToTime = TimeSpan.Parse(value);
            }
        }

        [System.Xml.Serialization.XmlIgnore()]
        public System.TimeSpan ToTime
        {
            get
            {
                return this.m_toTime;
            }
            set
            {
                this.m_toTime = value;
                this.m_toTimeDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, value.Hours, value.Minutes, value.Seconds);
            }
        }

        /// <summary>
        /// Checks whether the end of the working time is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ToTimeSpecified
        {
            get
            {
                return this.m_btoTimeSpecified;
            }
            set
            {
                this.m_btoTimeSpecified = value;
            }
        }
        #endregion
    }
}

#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;
using System.Xml.Serialization;

namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Represents a day in a working week
    /// </summary>
    [System.Serializable()]
    public class WorkWeekDay
    {
        #region Fields
        private DayType m_dayType;
        private bool m_bdayWorking;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the type of day
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
                return this.m_bdayWorking ? "1" : "0";
            }
            set
            {
                this.m_bdayWorking = System.Xml.XmlConvert.ToBoolean(value);
            }
        }
        #endregion
    }
}

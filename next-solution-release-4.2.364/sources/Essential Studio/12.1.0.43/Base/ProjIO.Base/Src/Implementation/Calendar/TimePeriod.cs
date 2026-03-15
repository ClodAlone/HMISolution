#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.ProjIO
{
    /// <summary>
    /// TimePeriod class
    /// </summary>
    [System.Serializable()]
    public class TimePeriod
    {
        #region Fields
        private System.DateTime m_fromDate;
        private bool m_bfromDateSpecified;
        private System.DateTime m_toDate;
        private bool m_btoDateSpecified;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the beginning of the exception time
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "FromDate", DataType = "dateTime")]
        public System.DateTime FromDate
        {
            get
            {
                return this.m_fromDate;
            }
            set
            {
                this.m_fromDate = value;
            }
        }

        /// <summary>
        /// Checks whether the beginning of the exception time is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool FromDateSpecified
        {
            get
            {
                return this.m_bfromDateSpecified;
            }
            set
            {
                this.m_bfromDateSpecified = value;
            }
        }

        /// <summary>
        /// Gets os sets the end of the exception time
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "ToDate", DataType = "dateTime")]
        public System.DateTime ToDate
        {
            get
            {
                return this.m_toDate;
            }
            set
            {
                this.m_toDate = value;
            }
        }

        /// <summary>
        /// Checks whether the end of the exception time is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ToDateSpecified
        {
            get
            {
                return this.m_btoDateSpecified;
            }
            set
            {
                this.m_btoDateSpecified = value;
            }
        }
        #endregion
    }
}

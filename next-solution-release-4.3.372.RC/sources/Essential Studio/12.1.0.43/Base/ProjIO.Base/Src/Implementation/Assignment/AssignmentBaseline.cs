#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Globalization;
using System.ComponentModel;

namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Defines baseline for resource assignments
    /// </summary>
    [System.Serializable()]
    public class AssignmentBaseline
    {
        #region Fields
        private List<TimephasedDataType> m_timephasedData=new List<TimephasedDataType>();
        private string m_number;
        private string m_start;
        private string m_finish;
        private string m_work;
        private string m_cost;
        private float m_bCWS;
        private bool m_bbCWSSpecified;
        private float m_bCWP;
        private bool m_bbCWPSpecified;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the time phased data associated with the baseline of the assignment
        /// </summary>
        [System.Xml.Serialization.XmlElement("TimephasedData")]
        public List<TimephasedDataType> TimephasedData
        {
            get
            {
                return this.m_timephasedData;
            }
            set
            {
                this.m_timephasedData = value;
            }
        }

        /// <summary>
        /// Gets or sets the unique number of the baseline of the assignment
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="Number",DataType="string")]
        public string Number
        {
            get
            {
                return this.m_number;
            }
            set
            {
                this.m_number = value;
            }
        }

        /// <summary>
        /// Gets or sets the scheduled start date and time of the assignment when the baseline was saved
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Start", DataType = "string")]
        public string Start
        {
            get
            {
                return this.m_start;
            }
            set
            {
                this.m_start = value;
            }
        }

        /// <summary>
        /// Gets or sets the scheduled finish date of the assignment when the baseline was saved
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Finish", DataType = "string")]
        public string Finish
        {
            get
            {
                return this.m_finish;
            }
            set
            {
                this.m_finish = value;
            }
        }

        /// <summary>
        /// Gets or sets the scheduled work for the assignment when the baseline was saved
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="Work",DataType = "duration")]
        public string Work
        {
            get
            {
                return this.m_work;
            }
            set
            {
                this.m_work = value;
            }
        }

        /// <summary>
        /// Gets or sets the total scheduled/projected cost for an assignment
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Cost", DataType = "string")]
        public string Cost
        {
            get
            {
                return this.m_cost;
            }
            set
            {
                this.m_cost = value;
            }
        }

        /// <summary>
        /// Gets or sets the budgeted cost of work on the assignment to the current date
        /// </summary>
        [XmlIgnore]
        public float BCWS
        {
            get
            {
                return this.m_bCWS;
            }
            set
            {
                this.m_bCWS = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("BCWS")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string BCWSSerialized
        {
            get
            {
                return BCWS.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_bCWS);
            }
        }
        /// <summary>
        /// Checks whether the budgeted cost of work on the assignment to the current date is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BCWSSpecified
        {
            get
            {
                return this.m_bbCWSSpecified;
            }
            set
            {
                this.m_bbCWSSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the budgeted cost of the work performed on the assignment to-date
        /// </summary>
        [XmlIgnore]
        public float BCWP
        {
            get
            {
                return this.m_bCWP;
            }
            set
            {
                this.m_bCWP = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("BCWP")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string BCWPSerialized
        {
            get
            {
                return BCWP.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_bCWP);
            }
        }
        /// <summary>
        /// Checks whether the budgeted cost of the work performed on the assignment to-date is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BCWPSpecified
        {
            get
            {
                return this.m_bbCWPSpecified;
            }
            set
            {
                this.m_bbCWPSpecified = value;
            }
        }
        #endregion
    }
}

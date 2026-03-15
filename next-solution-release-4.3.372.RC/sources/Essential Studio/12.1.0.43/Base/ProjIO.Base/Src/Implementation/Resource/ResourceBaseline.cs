#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.ComponentModel;
using System.Xml.Serialization;
using System.Globalization;
namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Defines baseline for resources
    /// </summary>
    [System.Serializable()]
    public class ResourceBaseline
    {
        #region Fields
        private int m_number;
        private string m_work;
        private float m_cost;
        private bool m_bcostSpecified;
        private float m_bCWS;
        private bool m_bbCWSSpecified;
        private float m_bCWP;
        private bool m_bbCWPSpecified;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the unique number of the baseline data record
        /// </summary>
        [XmlIgnore]
        public int Number
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
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("Number")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string NumberSerialized
        {
            get
            {
                return Number.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_number);
            }
        }
        /// <summary>
        /// Gets or sets the work assigned to the resource when the baseline is saved
        /// </summary>
        [System.Xml.Serialization.XmlElement("Work",DataType = "duration")]
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
        /// Gets or sets the projected cost for the resource when the baseline was saved
        /// </summary>
        [XmlIgnore]
        public float Cost
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
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("Cost")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string CostSerialized
        {
            get
            {
                return Cost.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_cost);
            }
        }
        /// <summary>
        /// Checks whether the projected cost for the resource when the baseline was saved is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CostSpecified
        {
            get
            {
                return this.m_bcostSpecified;
            }
            set
            {
                this.m_bcostSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the budgeted cost of work scheduled for the resource
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
        /// Checks whether the budgeted cost of work scheduled for the resource is specified
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
        /// Gets or sets the budgeted cost of the work performed by the resource for the project to date
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
        /// Checks whether the budgeted cost of the work performed by the resource for the project to date is specified
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

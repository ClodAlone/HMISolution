#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Xml.Serialization;
using System.ComponentModel;
using System.Globalization;
namespace Syncfusion.ProjIO
{
    /// <summary>
    /// ResourceAvailabilityPeriod class
    /// </summary>
    [System.Serializable()]
    public class ResourceAvailabilityPeriod
    {
        #region Fields
        private System.DateTime m_availableFrom;
        private bool m_bavailableFromSpecified;
        private System.DateTime m_availableTo;
        private bool m_bavailableToSpecified;
        private float m_availableUnits;
        private bool m_availableUnitsSpecified;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the date that the resource becomes available for the specified period
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="AvailableFrom",DataType="date")]
        public System.DateTime AvailableFrom
        {
            get
            {
                return this.m_availableFrom;
            }
            set
            {
                this.m_availableFrom = value;
            }
        }

        /// <summary>
        /// Checks whether the date that the resource becomes available for the specified period is set
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AvailableFromSpecified
        {
            get
            {
                return this.m_bavailableFromSpecified;
            }
            set
            {
                this.m_bavailableFromSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the last date that the resource is available for the specified period
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "AvailableTo", DataType = "date")]
        public System.DateTime AvailableTo
        {
            get
            {
                return this.m_availableTo;
            }
            set
            {
                this.m_availableTo = value;
            }
        }

        /// <summary>
        /// Checks whether the last date that the resource is available for the specified period is set
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AvailableToSpecified
        {
            get
            {
                return this.m_bavailableToSpecified;
            }
            set
            {
                this.m_bavailableToSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the percentage that the resource is available during the specified period
        /// </summary>
        [XmlIgnore]
        public float AvailableUnits
        {
            get
            {
                return this.m_availableUnits;
            }
            set
            {
                this.m_availableUnits = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("AvailableUnits")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string AvailableUnitsSerialized
        {
            get
            {
                return AvailableUnits.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_availableUnits);
            }
        }

        /// <summary>
        /// Checks whether the percentage that the resource is available during the specified period is set
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AvailableUnitsSpecified
        {
            get
            {
                return this.m_availableUnitsSpecified;
            }
            set
            {
                this.m_availableUnitsSpecified = value;
            }
        }
        #endregion
    }
}

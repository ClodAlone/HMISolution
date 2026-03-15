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
    /// Base class for ExtendedAttributes
    /// </summary>
    [System.Serializable()]
    public class ExtendedAttributesBase
    {
        #region Fields
        private string m_fieldID;
        private string m_value;
        private string m_valueGUID;
        private DurationFormat m_durationFormat;
        private bool m_bdurationFormatSpecified;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the fieldID for the extended attribute
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="FieldID",DataType="string")]
        public string FieldID
        {
            get
            {
                return this.m_fieldID;
            }
            set
            {
                this.m_fieldID = value;
            }
        }

        /// <summary>
        /// Gets or sets the Actual value of the extended attribute
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Value", DataType = "string")]
        public string Value
        {
            get
            {
                return this.m_value;
            }
            set
            {
                this.m_value = value;
            }
        }

        /// <summary>
        /// Gets or sets the GUID of the value in the value list
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="ValueGUID",DataType = "integer")]
        public string ValueGUID
        {
            get
            {
                return this.m_valueGUID;
            }
            set
            {
                this.m_valueGUID = value;
            }
        }

        /// <summary>
        /// Gets or sets the duration format for the extended attribute
        /// </summary>
        [System.ComponentModel.DefaultValue(DurationFormat.EstimatedDays)]
        public DurationFormat DurationFormat
        {
            get
            {
                return this.m_durationFormat;
            }
            set
            {
                this.m_durationFormat = value;
            }
        }

        /// <summary>
        /// Checks whether the duration format for the extended attribute is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DurationFormatSpecified
        {
            get
            {
                return this.m_bdurationFormatSpecified;
            }
            set
            {
                this.m_bdurationFormatSpecified = value;
            }
        }
        #endregion
    }
}

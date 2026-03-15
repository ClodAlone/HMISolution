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
    /// Represents list of values for Extended attributes
    /// </summary>
    [System.Serializable()]
    public class ExtendedAttributeValueList
    {
        #region Fields
        private string m_id;
        private string m_value;
        private string m_description;
        private string m_phonetic;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the unique ID of value across the project
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="ID",DataType = "integer")]
        public string ID
        {
            get
            {
                return this.m_id;
            }
            set
            {
                this.m_id = value;
            }
        }

        /// <summary>
        /// Gets or sets the actual value
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
        /// Gets or sets the description of the value in the list
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Description", DataType = "string")]
        public string Description
        {
            get
            {
                return this.m_description;
            }
            set
            {
                this.m_description = value;
            }
        }

        /// <summary>
        /// Gets or sets the phonetic information for custom field names
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Phonetic", DataType = "string")]
        public string Phonetic
        {
            get
            {
                return this.m_phonetic;
            }
            set
            {
                this.m_phonetic = value;
            }
        }
        #endregion
    }
}

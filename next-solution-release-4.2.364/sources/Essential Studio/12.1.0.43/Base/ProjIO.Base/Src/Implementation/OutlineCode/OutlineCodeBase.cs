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
    /// Base class for defining outline codes
    /// </summary>
    [System.Serializable()]
    public class OutlineCodeBase
    {
        #region Fields
        private string m_fieldID;
        private string m_valueID;
        private string m_valueGUID;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the number value of the custom field project ID (PID)
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
        /// Gets or sets the ID in the value list associated with the definition in the outline code collection
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="ValueID",DataType = "integer")]
        public string ValueID
        {
            get
            {
                return this.m_valueID;
            }
            set
            {
                this.m_valueID = value;
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
        #endregion
    }
}

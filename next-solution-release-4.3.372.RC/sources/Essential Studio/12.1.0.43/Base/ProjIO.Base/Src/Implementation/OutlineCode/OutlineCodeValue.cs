#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Represents an Outline code value
    /// </summary>
    [System.Serializable()]
    public class OutlineCodeValue
    {
        #region Fields
        private string m_valueID;
        private string m_fieldGUID;
        private OutlineCodeValueType m_type;
        private bool m_bisCollapsed;
        private bool m_bisCollapsedFieldSpecified;
        private string m_parentValueID;
        private string m_value;
        private string m_description;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the unique ID of the outline code value within the project
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
        /// Gets or sets the GUID of the outline code value
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "FieldGUID", DataType = "string")]
        public string FieldGUID
        {
            get
            {
                return this.m_fieldGUID;
            }
            set
            {
                this.m_fieldGUID = value;
            }
        }

        /// <summary>
        /// Gets or sets the outline code type
        /// </summary>
        [System.Xml.Serialization.XmlElement("Type")]
        public OutlineCodeValueType Type
        {
            get
            {
                return this.m_type;
            }
            set
            {
                this.m_type = value;
            }
        }

        /// <summary>
        /// Checks whether the outline code element is collapsed
        /// </summary>
        [XmlIgnore()]
        public bool IsCollapsed
        {
            get
            {
                return this.m_bisCollapsed;
            }
            set
            {
                this.m_bisCollapsed = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("IsCollapsed")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsCollapsedString
        {
            get
            {
                return this.m_bisCollapsed ? "1" : "0";
            }
            set
            {
                this.m_bisCollapsed = XmlConvert.ToBoolean(value);
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsCollapsedSpecified
        {
            get
            {
                return this.m_bisCollapsedFieldSpecified;
            }
            set
            {
                this.m_bisCollapsedFieldSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the parent node of the outline code
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="ParentValueID",DataType = "integer")]
        public string ParentValueID
        {
            get
            {
                return this.m_parentValueID;
            }
            set
            {
                this.m_parentValueID = value;
            }
        }

        /// <summary>
        /// Gets or sets the actual value
        /// </summary>
        //[System.Xml.Serialization.XmlElement(ElementName = "Value", DataType = "string")]
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
        /// Gets or sets a description of this value
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
        #endregion
    }
}

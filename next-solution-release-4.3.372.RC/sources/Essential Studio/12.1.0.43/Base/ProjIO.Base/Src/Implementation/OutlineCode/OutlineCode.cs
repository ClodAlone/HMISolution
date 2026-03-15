#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Represents an Outline code
    /// </summary>
    [System.Serializable()]
    public class OutlineCode
    {
        #region Fields
        private string m_guid;
        private string m_fieldID;
        private string m_fieldName;
        private string m_alias;
        private string m_phoneticAlias;
        private OulineCodeValues m_values;
        private bool m_benterprise;
        private bool m_benterpriseSpecified;
        private string m_enterpriseOutlineCodeAlias;
        private bool m_bresourceSubstitutionEnabled;
        private bool m_bresourceSubstitutionEnabledSpecified;
        private bool m_bleafOnly;
        private bool m_ballLevelsRequired;
        private bool m_ballLevelsRequiredSpecified;
        private bool m_bonlyTableValuesAllowed;
        private bool m_bonlyTableValuesAllowedSpecified;
        private bool m_bshowIndent;
        private bool m_bshowIndentSpecified;
        private List<OutlineCodeMask> m_masks=new List<OutlineCodeMask>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the GUID of the outline code
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="Guid",DataType="string")]
        public string Guid
        {
            get
            {
                return this.m_guid;
            }
            set
            {
                this.m_guid = value;
            }
        }

        /// <summary>
        /// Gets or sets the field number of the outline code
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "FieldID", DataType = "string")]
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
        /// Gets or sets the name of the custom outline code
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "FieldName", DataType = "string")]
        public string FieldName
        {
            get
            {
                return this.m_fieldName;
            }
            set
            {
                this.m_fieldName = value;
            }
        }

        /// <summary>
        /// Gets pr sets the alias of the custom outline code
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Alias", DataType = "string")]
        public string Alias
        {
            get
            {
                return this.m_alias;
            }
            set
            {
                this.m_alias = value;
            }
        }

        /// <summary>
        /// Gets or sets the phonetic pronunciation of the alias of the custom outline code
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "PhoneticAlias", DataType = "string")]
        public string PhoneticAlias
        {
            get
            {
                return this.m_phoneticAlias;
            }
            set
            {
                this.m_phoneticAlias = value;
            }
        }

        /// <summary>
        /// Gets or sets the values of the table associated with this outline code
        /// </summary>
        [System.Xml.Serialization.XmlElement("Values")]
        public OulineCodeValues Values
        {
            get
            {
                return this.m_values;
            }
            set
            {
                this.m_values = value;
            }
        }

        /// <summary>
        /// Checks whether the custom outline code is an enterprise custom outline code
        /// </summary>
        [XmlIgnore()]
        public bool IsEnterprise
        {
            get
            {
                return this.m_benterprise;
            }
            set
            {
                this.m_benterprise = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("IsEnterprise")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsEnterpriseString
        {
            get
            {
                return this.m_benterprise ? "1" : "0";
            }
            set
            {
                this.m_benterprise = XmlConvert.ToBoolean(value);
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool EnterpriseSpecified
        {
            get
            {
                return this.m_benterpriseSpecified;
            }
            set
            {
                this.m_benterpriseSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets reference to another custom field for which this is an alias
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="EnterpriseOutlineCodeAlias",DataType = "integer")]
        public string EnterpriseOutlineCodeAlias
        {
            get
            {
                return this.m_enterpriseOutlineCodeAlias;
            }
            set
            {
                this.m_enterpriseOutlineCodeAlias = value;
            }
        }

        /// <summary>
        /// Checks whether the custom outline code can be used by the Resource Substitution Wizard in Microsoft Project.
        /// </summary>
        [XmlIgnore()]
        public bool IsResourceSubstitutionEnabled
        {
            get
            {
                return this.m_bresourceSubstitutionEnabled;
            }
            set
            {
                this.m_bresourceSubstitutionEnabled = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("IsResourceSubstitutionEnabled")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsResourceSubstitutionEnabledString
        {
            get
            {
                return this.m_bresourceSubstitutionEnabled ? "1" : "0";
            }
            set
            {
                this.m_bresourceSubstitutionEnabled = XmlConvert.ToBoolean(value);
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ResourceSubstitutionEnabledSpecified
        {
            get
            {
                return this.m_bresourceSubstitutionEnabledSpecified;
            }
            set
            {
                this.m_bresourceSubstitutionEnabledSpecified = value;
            }
        }

        /// <summary>
        /// Checks whether or not values specified in this outline code field must be leaf values
        /// </summary>
        [XmlIgnore()]
        public bool LeafOnly
        {
            get
            {
                return this.m_bleafOnly;
            }
            set
            {
                this.m_bleafOnly = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("LeafOnly")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string LeafOnlyString
        {
            get
            {
                return this.m_bleafOnly ? "1" : "0";
            }
            set
            {
                this.m_bleafOnly = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Checks whether new codes must have all levels present
        /// </summary>
        [XmlIgnore()]
        public bool AllLevelsRequired
        {
            get
            {
                return this.m_ballLevelsRequired;
            }
            set
            {
                this.m_ballLevelsRequired = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("AllLevelsRequired")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string AllLevelsRequiredString
        {
            get
            {
                return this.m_ballLevelsRequired ? "1" : "0";
            }
            set
            {
                this.m_ballLevelsRequired = XmlConvert.ToBoolean(value);
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AllLevelsRequiredSpecified
        {
            get
            {
                return this.m_ballLevelsRequiredSpecified;
            }
            set
            {
                this.m_ballLevelsRequiredSpecified = value;
            }
        }

        /// <summary>
        /// Checks whether values specified must come from values table
        /// </summary>
        [XmlIgnore()]
        public bool OnlyTableValuesAllowed
        {
            get
            {
                return this.m_bonlyTableValuesAllowed;
            }
            set
            {
                this.m_bonlyTableValuesAllowed = value;
            }
        }

        
        [System.Xml.Serialization.XmlElement("OnlyTableValuesAllowed")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string OnlyTableValuesAllowedString
        {
            get
            {
                return this.m_bonlyTableValuesAllowed ? "1" : "0";
            }
            set
            {
                this.m_bonlyTableValuesAllowed = XmlConvert.ToBoolean(value);
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool OnlyTableValuesAllowedSpecified
        {
            get
            {
                return this.m_bonlyTableValuesAllowedSpecified;
            }
            set
            {
                this.m_bonlyTableValuesAllowedSpecified = value;
            }
        }

        /// <summary>
        /// Specifies whether to show indenting in the outline code
        /// </summary>
        [XmlIgnore()]
        public bool ShowIndent
        {
            get
            {
                return this.m_bshowIndent;
            }
            set
            {
                this.m_bshowIndent = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("ShowIndent")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ShowIndentString
        {
            get
            {
                return this.m_bshowIndent ? "1" : "0";
            }
            set
            {
                this.m_bshowIndent = XmlConvert.ToBoolean(value);
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ShowIndentSpecified
        {
            get
            {
                return this.m_bshowIndentSpecified;
            }
            set
            {
                this.m_bshowIndentSpecified = value;
            }
        }

        /// <summary>
        /// Gets or set the table of entries that define the outline code mask
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("Mask", IsNullable = false)]
        public List<OutlineCodeMask> Masks
        {
            get
            {
                return this.m_masks;
            }
            set
            {
                this.m_masks = value;
            }
        }
        #endregion
    }
}

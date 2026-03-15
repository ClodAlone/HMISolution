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
    /// Represents an Extended Attribute
    /// </summary>
    [System.Serializable()]
    public class ExtendedAttribute
    {
        #region Fields
        private string m_fieldID;
        private string m_fieldName;
        private CFType m_cFType;
        private bool m_bcFTypeSpecified;
        private string m_guid;
        private ElemType m_elemType;
        private bool m_belemTypeSpecified;
        private string m_maxMultiValues;
        private bool m_buserDef;
        private bool m_buserDefSpecified;
        private string m_alias;
        private string m_secondaryPID;
        private bool m_bautoRollDown;
        private bool m_bautoRollDownSpecified;
        private string m_defaultGuid;
        private string m_ltuid;
        private string m_secondaryGuid;
        private string m_phoneticAlias;
        private RollupType m_rollupType;
        private bool m_brollupTypeSpecified;
        private CalculationType m_calculationType;
        private bool m_bcalculationTypeFieldSpecified;
        private string m_formula;
        private bool m_brestrictValues;
        private bool m_brestrictValuesFieldSpecified;
        private ValuelistSortOrder m_valuelistSortOrder;
        private bool m_bvaluelistSortOrderSpecified;
        private bool m_bappendNewValues;
        private bool m_bappendNewValuesFieldSpecified;
        private string m_defaultField;
        private List<ExtendedAttributeValueList> m_valueList;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the PID of the custom field
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
        /// Gets or sets the name of the custom field
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
        /// Gets or sets the custom field type
        /// </summary>
        [System.Xml.Serialization.XmlElement("CFType")]
        public CFType CFType
        {
            get
            {
                return this.m_cFType;
            }
            set
            {
                this.m_cFType = value;
            }
        }

        /// <summary>
        /// Checks whether the custom field type is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnore()]
        public bool CFTypeSpecified
        {
            get
            {
                return this.m_bcFTypeSpecified;
            }
            set
            {
                this.m_bcFTypeSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the GUID of the custom field
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Guid", DataType = "string")]
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
        /// Gets or sets the Element type of the Extended attribute
        /// </summary>
        [System.Xml.Serialization.XmlElement("ElemType")]
        public ElemType ElemType
        {
            get
            {
                return this.m_elemType;
            }
            set
            {
                this.m_elemType = value;
            }
        }

        /// <summary>
        /// Checks whether the Element type of ExtendedAttribute is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ElemTypeSpecified
        {
            get
            {
                return this.m_belemTypeSpecified;
            }
            set
            {
                this.m_belemTypeSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of values that can be set in a picklist
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="MaxMultiValues",DataType = "integer")]
        public string MaxMultiValues
        {
            get
            {
                return this.m_maxMultiValues;
            }
            set
            {
                this.m_maxMultiValues = value;
            }
        }

        /// <summary>
        /// Checks whether the custom field is user defined
        /// </summary>
        [XmlIgnore()]
        public bool IsUserDef
        {
            get
            {
                return this.m_buserDef;
            }
            set
            {
                this.m_buserDef = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("IsUserDef")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsUserDefString
        {
            get
            {
                return this.m_buserDef ? "1" : "0";
            }
            set
            {
                this.m_buserDef = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Checks whether User Defined Property is defined
        /// </summary>
        [System.Xml.Serialization.XmlIgnore()]
        public bool IsUserDefSpecified
        {
            get
            {
                return this.m_buserDefSpecified;
            }
            set
            {
                this.m_buserDefSpecified = value;
            }
        }
        /// <summary>
        /// Gets or sets the alias of the custom field
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
        /// Gets or sets the secondary PID of the custom field
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "SecondaryPID", DataType = "string")]
        public string SecondaryPID
        {
            get
            {
                return this.m_secondaryPID;
            }
            set
            {
                this.m_secondaryPID = value;
            }
        }

        /// <summary>
        /// Checks whether automatic rolldown to assignments is enabled
        /// </summary>
        [XmlIgnore()]
        public bool AutoRollDown
        {
            get
            {
                return this.m_bautoRollDown;
            }
            set
            {
                this.m_bautoRollDown = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("AutoRollDown")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string AutoRollDownString
        {
            get
            {
                return this.m_bautoRollDown ? "1" : "0";
            }
            set
            {
                this.m_bautoRollDown = XmlConvert.ToBoolean(value);
            }
        }
        [System.Xml.Serialization.XmlIgnore()]
        public bool AutoRollDownSpecified
        {
            get
            {
                return this.m_bautoRollDownSpecified;
            }
            set
            {
                m_bautoRollDownSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the GUID of the default lookup table entry
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "DefaultGuid", DataType = "string")]
        public string DefaultGuid
        {
            get
            {
                return this.m_defaultGuid;
            }
            set
            {
                this.m_defaultGuid = value;
            }
        }

        /// <summary>
        /// Gets or sets the GUID of the lookup table associated with the custom field
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Ltuid", DataType = "string")]
        public string Ltuid
        {
            get
            {
                return this.m_ltuid;
            }
            set
            {
                this.m_ltuid = value;
            }
        }

        /// <summary>
        /// Gets or sets the GUID of the secondary PID of the custom field
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "SecondaryGuid", DataType = "string")]
        public string SecondaryGuid
        {
            get
            {
                return this.m_secondaryGuid;
            }
            set
            {
                this.m_secondaryGuid = value;
            }
        }

        /// <summary>
        /// Gets or sets the phonetic pronunciation of the alias of the custom field
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
        /// Gets or sets the way rollups are calculated
        /// </summary>
        [System.Xml.Serialization.XmlElement("RollupType")]
        public RollupType RollupType
        {
            get
            {
                return this.m_rollupType;
            }
            set
            {
                this.m_rollupType = value;
            }
        }

        /// <summary>
        /// Checks whether the way rollups are calculated is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RollupTypeSpecified
        {
            get
            {
                return this.m_brollupTypeSpecified;
            }
            set
            {
                this.m_brollupTypeSpecified = value;
            }
        }

        /// <summary>
        /// Checks whether rollups are calculated for task and group summary rows
        /// </summary>
        [System.Xml.Serialization.XmlElement("CalculationType")]
        public CalculationType CalculationType
        {
            get
            {
                return this.m_calculationType;
            }
            set
            {
                this.m_calculationType = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CalculationTypeSpecified
        {
            get
            {
                return this.m_bcalculationTypeFieldSpecified;
            }
            set
            {
                this.m_bcalculationTypeFieldSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the formula that Microsoft Project uses to populate the custom task field
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Formula", DataType = "string")]
        public string Formula
        {
            get
            {
                return this.m_formula;
            }
            set
            {
                this.m_formula = value;
            }
        }

        /// <summary>
        /// Checks whether only values in the list are allowed in the file
        /// </summary>
        [XmlIgnore()]
        public bool RestrictValues
        {
            get
            {
                return this.m_brestrictValues;
            }
            set
            {
                this.m_brestrictValues = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("RestrictValues")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string RestrictValuesString
        {
            get
            {
                return this.m_brestrictValues ? "1" : "0";
            }
            set
            {
                this.m_brestrictValues = XmlConvert.ToBoolean(value);
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RestrictValuesSpecified
        {
            get
            {
                return this.m_brestrictValuesFieldSpecified;
            }
            set
            {
                this.m_brestrictValuesFieldSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the way value lists are sorted
        /// </summary>
        [System.Xml.Serialization.XmlElement("ValuelistSortOrder")]
        public ValuelistSortOrder ValuelistSortOrder
        {
            get
            {
                return this.m_valuelistSortOrder;
            }
            set
            {
                this.m_valuelistSortOrder = value;
            }
        }

        /// <summary>
        /// Checks whether the way value lists are sorted is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ValuelistSortOrderSpecified
        {
            get
            {
                return this.m_bvaluelistSortOrderSpecified;
            }
            set
            {
                this.m_bvaluelistSortOrderSpecified = value;
            }
        }

        /// <summary>
        /// Checks whether new values added to the project are automatically added to the list
        /// </summary>
        [XmlIgnore()]
        public bool AppendNewValues
        {
            get
            {
                return this.m_bappendNewValues;
            }
            set
            {
                this.m_bappendNewValues = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("AppendNewValues")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string AppendNewValuesString
        {
            get
            {
                return this.m_bappendNewValues ? "1" : "0";
            }
            set
            {
                this.m_bappendNewValues = XmlConvert.ToBoolean(value);
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AppendNewValuesSpecified
        {
            get
            {
                return this.m_bappendNewValuesFieldSpecified;
            }
            set
            {
                this.m_bappendNewValuesFieldSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the default value in the list
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="Default",DataType="string")]
        public string Default
        {
            get
            {
                return this.m_defaultField;
            }
            set
            {
                this.m_defaultField = value;
            }
        }

        /// <summary>
        /// Gets or sets the values that make up the value list
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("ExtendedAttributeValueList", IsNullable = false)]
        public List<ExtendedAttributeValueList> ValueList
        {
            get
            {
                return this.m_valueList;
            }
            set
            {
                this.m_valueList = value;
            }
        }
        #endregion
    }
}

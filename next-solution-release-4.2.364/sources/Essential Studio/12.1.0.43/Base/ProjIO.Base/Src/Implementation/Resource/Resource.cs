#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;

namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Represents a Resource in a Project
    /// </summary>
    [System.Serializable()]
    public class Resource
    {
        #region Fields
        private int m_uID;
        private int m_id;
        private string m_name;
        private ResourceType m_type;
        private bool m_bisNull;
        private string m_initials;
        private string m_phonetics;
        private string m_nTAccount;
        private string m_materialLabel;
        private string m_code;
        private string m_group;
        private ResourceWorkGroup m_workGroup;
        private string m_emailAddress;
        private string m_hyperlink;
        private string m_hyperlinkAddress;
        private string m_hyperlinkSubAddress;
        private float m_maxUnits;
        private float m_peakUnits;
        private bool m_bpeakUnitsSpecified;
        private bool m_boverAllocated;
        private System.DateTime m_availableFrom;
        private bool m_bavailableFromSpecified;
        private System.DateTime m_availableTo;
        private bool m_bavailableToSpecified;
        private System.DateTime m_start;
        private System.DateTime m_finish;
        private bool m_bcanLevel;
        private ResourceAccrueAt m_accrueAt;
        private string m_work;
        private string m_regularWork;
        private string m_overtimeWork;
        private string m_actualWork;
        private string m_remainingWork;
        private string m_actualOvertimeWork;
        private string m_remainingOvertimeWork;
        private int m_percentWorkComplete;        
        private decimal m_standardRate;
        private ResourceStandardRateFormat m_standardRateFormat;
        private decimal m_cost;
        private decimal m_overtimeRate;
        private ResourceOvertimeRateFormat m_overtimeRateFormat;
        private decimal m_overtimeCost;        
        private decimal m_costPerUse;
        private decimal m_actualCost;        
        private decimal m_actualOvertimeCost;       
        private decimal m_remainingCost;
        private decimal m_remainingOvertimeCost;
        private float m_workVariance;
        private float m_costVariance;
        private float m_sv;
        private float m_cv;
        private float m_aCWP;
        private int m_calendarUID;
        private string m_notes;
        private float m_bCWS;
        private float m_bCWP;
        private bool m_bisGeneric;
        private bool m_bisInactive;
        private bool m_bisEnterprise;
        private BookingType m_bookingType;
        private string m_actualWorkProtected;
        private string m_actualOvertimeWorkProtected;
        private string m_activeDirectoryGUID;
        private System.DateTime m_creationDate;
        private List<ExtendedAttributesBase> m_extendedAttribute=new List<ExtendedAttributesBase>();
        private List<ResourceBaseline> m_baseline = new List<ResourceBaseline>();
        private List<OutlineCodeBase> m_outlineCode = new List<OutlineCodeBase>();
        private bool m_bisCostResource;
        private string m_assnOwner;
        private string m_assnOwnerGuid;
        private bool m_bisBudget;
        private List<ResourceAvailabilityPeriod> m_availabilityPeriods = new List<ResourceAvailabilityPeriod>();
        private List<ResourceRate> m_rates = new List<ResourceRate>();
        private List<TimephasedDataType> m_timephasedData = new List<TimephasedDataType>();
        #endregion

        #region Initializer
        public Resource()
        {
            this.m_maxUnits = ((float)(1F));
            this.m_accrueAt = (ResourceAccrueAt)3;
            this.m_standardRateFormat = (ResourceStandardRateFormat)2;
            this.m_overtimeRateFormat = (ResourceOvertimeRateFormat)2;
            this.m_calendarUID = 2;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the unique identifier of the resource
        /// </summary>
        [XmlIgnore]
        public int UID
        {
            get
            {
                return this.m_uID;
            }
            set
            {
                this.m_uID = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("UID")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string UIDSerialized
        {
            get
            {
                return UID.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_uID);
            }
        }

        /// <summary>
        /// Gets or sets the position identifier of the resource within the list of resources
        /// </summary>
        [XmlIgnore]
        public int ID
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
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("ID")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string IDSerialized
        {
            get
            {
                return ID.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_id);
            }
        }

        /// <summary>
        /// Gets or sets the name of the resource
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Name", DataType = "string")]
        public string Name
        {
            get
            {
                return this.m_name;
            }
            set
            {
                this.m_name = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of resource
        /// </summary>
        [System.Xml.Serialization.XmlElement("Type")]
        public ResourceType Type
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
        /// Checks whether the resource is null
        /// </summary>
        [XmlIgnore()]
        public bool IsNull
        {
            get
            {
                return this.m_bisNull;
            }
            set
            {
                this.m_bisNull = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("IsNull")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsNullString
        {
            get
            {
                return this.m_bisNull ? "1" : "0";
            }
            set
            {
                this.m_bisNull = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets the initials of the resource
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Initials", DataType = "string")]
        public string Initials
        {
            get
            {
                return this.m_initials;
            }
            set
            {
                this.m_initials = value;
            }
        }

        /// <summary>
        /// Gets or sets the phonetic spelling of the resource name
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Phonetics", DataType = "string")]
        public string Phonetics
        {
            get
            {
                return this.m_phonetics;
            }
            set
            {
                this.m_phonetics = value;
            }
        }

        /// <summary>
        /// Gets or sets the NT account associated with the resource
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "NTAccount", DataType = "string")]
        public string NTAccount
        {
            get
            {
                return this.m_nTAccount;
            }
            set
            {
                this.m_nTAccount = value;
            }
        }

        /// <summary>
        /// Gets or sets the unit of measure for the material resource
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "MaterialLabel", DataType = "string")]
        public string MaterialLabel
        {
            get
            {
                return this.m_materialLabel;
            }
            set
            {
                this.m_materialLabel = value;
            }
        }

        /// <summary>
        /// Gets or sets the code or other information about the resource
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Code", DataType = "string")]
        public string Code
        {
            get
            {
                return this.m_code;
            }
            set
            {
                this.m_code = value;
            }
        }

        /// <summary>
        /// Gets or sets the group to which the resource belongs
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Group", DataType = "string")]
        public string Group
        {
            get
            {
                return this.m_group;
            }
            set
            {
                this.m_group = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of workgroup to which the resource belongs
        /// </summary>
        [System.Xml.Serialization.XmlElement("WorkGroup")]
        public ResourceWorkGroup WorkGroup
        {
            get
            {
                return this.m_workGroup;
            }
            set
            {
                this.m_workGroup = value;
            }
        }

       
        /// <summary>
        /// Gets os sets the email address of the resource
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "EmailAddress", DataType = "string")]
        public string EmailAddress
        {
            get
            {
                return this.m_emailAddress;
            }
            set
            {
                this.m_emailAddress = value;
            }
        }

        /// <summary>
        /// Gets or sets the title of the hyperlink associated with the resource
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Hyperlink", DataType = "string")]
        public string Hyperlink
        {
            get
            {
                return this.m_hyperlink;
            }
            set
            {
                this.m_hyperlink = value;
            }
        }

        /// <summary>
        /// Gets os sets the hyperlink associated with the resource
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "HyperlinkAddress", DataType = "string")]
        public string HyperlinkAddress
        {
            get
            {
                return this.m_hyperlinkAddress;
            }
            set
            {
                this.m_hyperlinkAddress = value;
            }
        }

        /// <summary>
        /// Gets os sets the document bookmark of the hyperlink associated with the resource
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "HyperlinkSubAddress", DataType = "string")]
        public string HyperlinkSubAddress
        {
            get
            {
                return this.m_hyperlinkSubAddress;
            }
            set
            {
                this.m_hyperlinkSubAddress = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of units that the resource is available
        /// </summary>
        //[System.ComponentModel.DefaultValue(typeof(float), "1")]
        [XmlIgnore]
        public float MaxUnits
        {
            get
            {
                return this.m_maxUnits;
            }
            set
            {
                this.m_maxUnits = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("MaxUnits")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string MaxUnitsSerialized
        {
            get
            {
                return MaxUnits.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_maxUnits);
            }
        }

        /// <summary>
        /// Gets or sets the largest number of units assigned to the resource at any time
        /// </summary>
        [XmlIgnore]
        public float PeakUnits
        {
            get
            {
                return this.m_peakUnits;
            }
            set
            {
                this.m_peakUnits = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("PeakUnits")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string PeakUnitsSerialized
        {
            get
            {
                return PeakUnits.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_peakUnits);
            }
        }

        /// <summary>
        /// Checks whether the largest number of units assigned to the resource at any time is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PeakUnitsSpecified
        {
            get
            {
                return this.m_bpeakUnitsSpecified;
            }
            set
            {
                this.m_bpeakUnitsSpecified = value;
            }
        }

        /// <summary>
        /// Checks whether the resource is overallocated
        /// </summary>
        [XmlIgnore()]
        public bool IsOverAllocated
        {
            get
            {
                return this.m_boverAllocated;
            }
            set
            {
                this.m_boverAllocated = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("OverAllocated")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsOverAllocatedString
        {
            get
            {
                return this.m_boverAllocated ? "1" : "0";
            }
            set
            {
                this.m_boverAllocated = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets the first date that the resource is available
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="AvailableFrom",DataType="dateTime")]
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
        /// Checks whether the first date that the resource is available is specified
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
        /// Gets or sets the last date the resource is available
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "AvailableTo", DataType = "dateTime")]
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
        /// Checks whether the last date the resource is available is specified
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
        /// Gets or sets the scheduled start date of the resource
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Start", DataType = "dateTime")]
        public System.DateTime Start
        {
            get
            {
                if(this.m_start.Hour==0)
                    return new DateTime(this.m_start.Year,this.m_start.Month,this.m_start.Day,8,0,0);
                else
                    return this.m_start;
            }
            set
            {
                this.m_start = value;
            }
        }

        /// <summary>
        /// Checks whether the scheduled start date of the resource is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool StartSpecified
        {
            get
            {
                return this.m_bstartSpecified;
            }
            set
            {
                this.m_bstartSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the scheduled finish date of the resource
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Finish", DataType = "dateTime")]
        public System.DateTime Finish
        {
            get
            {
                if (this.m_finish.Hour == 0)
                    return new DateTime(this.m_finish.Year, this.m_finish.Month, this.m_finish.Day, 17, 0, 0);
                else
                    return this.m_finish;
            }
            set
            {
                this.m_finish = value;
            }
        }

        /// <summary>
        /// Checks whether the scheduled finish date of the resource is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool FinishSpecified
        {
            get
            {
                return this.m_bfinishSpecified;
            }
            set
            {
                this.m_bfinishSpecified = value;
            }
        }*/

        /// <summary>
        /// Checks whether the resource can be leveled
        /// </summary>
        [XmlIgnore()]
        public bool CanLevel
        {
            get
            {
                return this.m_bcanLevel;
            }
            set
            {
                this.m_bcanLevel = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("CanLevel")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string CanLevelString
        {
            get
            {
                return this.m_bcanLevel ? "1" : "0";
            }
            set
            {
                this.m_bcanLevel = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets how cost is accrued against the resource
        /// </summary>
        [System.Xml.Serialization.XmlElement("AccrueAt")]
        public ResourceAccrueAt AccrueAt
        {
            get
            {
                return this.m_accrueAt;
            }
            set
            {
                this.m_accrueAt = value;
            }
        }

        /// <summary>
        /// Checks whether how cost is accrued against the resource is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AccrueAtSpecified
        {
            get
            {
                return this.m_baccrueAtSpecified;
            }
            set
            {
                this.m_baccrueAtSpecified = value;
            }
        }*/

        /// <summary>
        /// gets or sets the total work assigned to the resource across all assigned tasks
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
        /// Gets or sets the amount of non-overtime work assigned to the resource
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="RegularWork",DataType = "duration")]
        public string RegularWork
        {
            get
            {
                return this.m_regularWork;
            }
            set
            {
                this.m_regularWork = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of overtime work assigned to the resource
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="OvertimeWork",DataType = "duration")]
        public string OvertimeWork
        {
            get
            {
                return this.m_overtimeWork;
            }
            set
            {
                this.m_overtimeWork = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of actual work performed by the resource
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="ActualWork",DataType = "duration")]
        public string ActualWork
        {
            get
            {
                return this.m_actualWork;
            }
            set
            {
                this.m_actualWork = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of remaining work required to complete all assigned tasks
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="RemainingWork",DataType = "duration")]
        public string RemainingWork
        {
            get
            {
                return this.m_remainingWork;
            }
            set
            {
                this.m_remainingWork = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of actual overtime work performed by the resource
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "ActualOvertimeWork", DataType = "duration")]
        public string ActualOvertimeWork
        {
            get
            {
                return this.m_actualOvertimeWork;
            }
            set
            {
                this.m_actualOvertimeWork = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of remaining overtime work required to complete all tasks
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "RemainingOvertimeWork", DataType = "duration")]
        public string RemainingOvertimeWork
        {
            get
            {
                return this.m_remainingOvertimeWork;
            }
            set
            {
                this.m_remainingOvertimeWork = value;
            }
        }

        /// <summary>
        /// Gets or sets the percentage of work completed across all tasks
        /// </summary>
        [XmlIgnore]
        public int PercentWorkComplete
        {
            get
            {
                return this.m_percentWorkComplete;
            }
            set
            {
                this.m_percentWorkComplete = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("PercentWorkComplete")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string PercentWorkCompleteSerialized
        {
            get
            {
                return PercentWorkComplete.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_percentWorkComplete);
            }
        }

        /// <summary>
        /// Gets or sets the standard rate of the resource
        /// </summary>
        [XmlIgnore]
        public decimal StandardRate
        {
            get
            {
                return this.m_standardRate;
            }
            set
            {
                this.m_standardRate = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("StandardRate")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string StandardRateSerialized
        {
            get
            {
                return StandardRate.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_standardRate);
            }
        }
        /// <summary>
        /// Cecks whether the standard rate of the resource is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool StandardRateSpecified
        {
            get
            {
                return this.m_bstandardRateSpecified;
            }
            set
            {
                this.m_bstandardRateSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the units used by Microsoft Project to display the standard rate
        /// </summary>
        [System.Xml.Serialization.XmlElement("StandardRateFormat")]
        public ResourceStandardRateFormat StandardRateFormat
        {
            get
            {
                return this.m_standardRateFormat;
            }
            set
            {
                this.m_standardRateFormat = value;
            }
        }

        /// <summary>
        /// Checks whether the units used by Microsoft Project to display the standard rate is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool StandardRateFormatSpecified
        {
            get
            {
                return this.m_bstandardRateFormatSpecified;
            }
            set
            {
                this.m_bstandardRateFormatSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the total project cost for the resource across all assigned tasks
        /// </summary>
        [XmlIgnore]
        public decimal Cost
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
                decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_cost);
            }
        }
        /// <summary>
        /// Checks whether the total project cost for the resource across all assigned tasks is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }*/

        /// <summary>
        /// Gets or sets the overtime rate of the resource
        /// </summary>
        [XmlIgnore]
        public decimal OvertimeRate
        {
            get
            {
                return this.m_overtimeRate;
            }
            set
            {
                this.m_overtimeRate = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("OvertimeRate")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string OvertimeRateSerialized
        {
            get
            {
                return OvertimeRate.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_overtimeRate);
            }
        }
        /// <summary>
        /// Checks whether the overtime rate of the resource is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool OvertimeRateSpecified
        {
            get
            {
                return this.m_bovertimeRateSpecified;
            }
            set
            {
                this.m_bovertimeRateSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the units used by Microsoft Project to display the overtime rate
        /// </summary>
        [System.Xml.Serialization.XmlElement("OvertimeRateFormat")]
        public ResourceOvertimeRateFormat OvertimeRateFormat
        {
            get
            {
                return this.m_overtimeRateFormat;
            }
            set
            {
                this.m_overtimeRateFormat = value;
            }
        }

        /// <summary>
        /// Checks whether the units used by Microsoft Project to display the overtime rate is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool OvertimeRateFormatSpecified
        {
            get
            {
                return this.m_bovertimeRateFormatSpecified;
            }
            set
            {
                this.m_bovertimeRateFormatSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the total overtime cost for the resource including actual and remaining overtime costs
        /// </summary>
        [XmlIgnore]
        public decimal OvertimeCost
        {
            get
            {
                return this.m_overtimeCost;
            }
            set
            {
                this.m_overtimeCost = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("OvertimeCost")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string OvertimeCostSerialized
        {
            get
            {
                return OvertimeCost.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_overtimeCost);
            }
        }

        /// <summary>
        /// Checks whether the total overtime cost for the resource including actual and remaining overtime costs is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool OvertimeCostSpecified
        {
            get
            {
                return this.m_bovertimeCostSpecified;
            }
            set
            {
                this.m_bovertimeCostSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the cost per use of the resource
        /// </summary>
        [XmlIgnore]
        public decimal CostPerUse
        {
            get
            {
                return this.m_costPerUse;
            }
            set
            {
                this.m_costPerUse = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("CostPerUse")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string CostPerUseSerialized
        {
            get
            {
                return CostPerUse.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_costPerUse);
            }
        }
        /// <summary>
        /// Checks whether the cost per use of the resource
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CostPerUseSpecified
        {
            get
            {
                return this.m_bcostPerUseSpecified;
            }
            set
            {
                this.m_bcostPerUseSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the actual cost incurred by the resource across all assigned tasks
        /// </summary>
        [XmlIgnore]
        public decimal ActualCost
        {
            get
            {
                return this.m_actualCost;
            }
            set
            {
                this.m_actualCost = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("ActualCost")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string ActualCostSerialized
        {
            get
            {
                return ActualCost.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_actualCost);
            }
        }

        /// <summary>
        /// Checks whether the actual cost incurred by the resource across all assigned tasks is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ActualCostSpecified
        {
            get
            {
                return this.m_bactualCostSpecified;
            }
            set
            {
                this.m_bactualCostSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the actual overtime cost incurred by the resource across all assigned tasks
        /// </summary>
        [XmlIgnore]
        public decimal ActualOvertimeCost
        {
            get
            {
                return this.m_actualOvertimeCost;
            }
            set
            {
                this.m_actualOvertimeCost = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("ActualOvertimeCost")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string ActualOvertimeCostSerialized
        {
            get
            {
                return ActualOvertimeCost.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_actualOvertimeCost);
            }
        }

        /// <summary>
        /// Checks whether the actual overtime cost incurred by the resource across all assigned tasks is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ActualOvertimeCostSpecified
        {
            get
            {
                return this.m_bactualOvertimeCostSpecified;
            }
            set
            {
                this.m_bactualOvertimeCostSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the remaining projected cost of the resource to complete all assigned tasks
        /// </summary>
        [XmlIgnore]
        public decimal RemainingCost
        {
            get
            {
                return this.m_remainingCost;
            }
            set
            {
                this.m_remainingCost = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("RemainingCost")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string RemainingCostSerialized
        {
            get
            {
                return RemainingCost.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_remainingCost);
            }
        }

        /// <summary>
        /// Checks whether the remaining projected cost of the resource to complete all assigned tasks is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RemainingCostSpecified
        {
            get
            {
                return this.m_bremainingCostSpecified;
            }
            set
            {
                this.m_bremainingCostSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the remaining projected overtime cost of the resource to complete all assigned tasks
        /// </summary>
        [XmlIgnore]
        public decimal RemainingOvertimeCost
        {
            get
            {
                return this.m_remainingOvertimeCost;
            }
            set
            {
                this.m_remainingOvertimeCost = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("RemainingOvertimeCost")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string RemainingOvertimeCostSerialized
        {
            get
            {
                return RemainingOvertimeCost.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_remainingOvertimeCost);
            }
        }

        /// <summary>
        /// Checks whether the remaining projected overtime cost of the resource to complete all assigned tasks is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RemainingOvertimeCostSpecified
        {
            get
            {
                return this.m_bremainingOvertimeCostSpecified;
            }
            set
            {
                this.m_bremainingOvertimeCostSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the difference between the baseline work and the work as minutes x 1000
        /// </summary>
        [XmlIgnore]
        public float WorkVariance
        {
            get
            {
                return this.m_workVariance;
            }
            set
            {
                this.m_workVariance = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("WorkVariance")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string WorkVarianceSerialized
        {
            get
            {
                return WorkVariance.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_workVariance);
            }
        }

        /// <summary>
        /// Checks whether Work variance is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool WorkVarianceSpecified
        {
            get
            {
                return this.m_bworkVarianceSpecified;
            }
            set
            {
                this.m_bworkVarianceSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the difference between the baseline cost and the cost
        /// </summary>
        [XmlIgnore]
        public float CostVariance
        {
            get
            {
                return this.m_costVariance;
            }
            set
            {
                this.m_costVariance = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("CostVariance")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string CostVarianceSerialized
        {
            get
            {
                return CostVariance.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_costVariance);
            }
        }

        /// <summary>
        /// Checks whether Cost Variance is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CostVarianceSpecified
        {
            get
            {
                return this.m_bcostVarianceSpecified;
            }
            set
            {
                this.m_bcostVarianceSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the earned value schedule variance, through the project status date
        /// </summary>
        [XmlIgnore]
        public float SV
        {
            get
            {
                return this.m_sv;
            }
            set
            {
                this.m_sv = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("SV")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string SVSerialized
        {
            get
            {
                return SV.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_sv);
            }
        }

        /// <summary>
        /// Checks whether Scheduled Variance s specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SVSpecified
        {
            get
            {
                return this.m_bsvSpecified;
            }
            set
            {
                this.m_bsvSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the earned value cost variance, through the project status date
        /// </summary>
        [XmlIgnore]
        public float CV
        {
            get
            {
                return this.m_cv;
            }
            set
            {
                this.m_cv = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("CV")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string CVSerialized
        {
            get
            {
                return CV.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_cv);
            }
        }

        /// <summary>
        /// Checks whether Cost Variance is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CVSpecified
        {
            get
            {
                return this.m_bcvSpecified;
            }
            set
            {
                this.m_bcvSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the actual cost of the work performed by the resource for the project to-date
        /// </summary>
        [XmlIgnore]
        public float ACWP
        {
            get
            {
                return this.m_aCWP;
            }
            set
            {
                this.m_aCWP = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("ACWP")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string ACWPSerialized
        {
            get
            {
                return ACWP.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_aCWP);
            }
        }

        /// <summary>
        /// Checks whether the actual cost of the work performed by the resource for the project to-date is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ACWPSpecified
        {
            get
            {
                return this.m_baCWPSpecified;
            }
            set
            {
                this.m_baCWPSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the UID of the resource calendar
        /// </summary>
        [XmlIgnore]
        public int CalendarUID
        {
            get
            {
                return this.m_calendarUID;
            }
            set
            {
                this.m_calendarUID = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("CalendarUID")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string CalendarUIDSerialized
        {
            get
            {
                return CalendarUID.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_calendarUID);
            }
        }

        /// <summary>
        /// Gets or sets the text notes associated with the resource
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Notes", DataType = "string")]
        public string Notes
        {
            get
            {
                return this.m_notes;
            }
            set
            {
                this.m_notes = value;
            }
        }

        /// <summary>
        /// Gets or sets the budget cost of work scheduled for the resource
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
        /// Checks whether the budget cost of work scheduled for the resource is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }*/

        /// <summary>
        /// Gets or sets the budgeted cost of the work performed by the resource for the project to-date
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
        /// Checks whether the budgeted cost of the work performed by the resource for the project to-date is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }*/

        /// <summary>
        /// Checks whether the resource is generic
        /// </summary>
        [XmlIgnore()]
        public bool IsGeneric
        {
            get
            {
                return this.m_bisGeneric;
            }
            set
            {
                this.m_bisGeneric = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("IsGeneric")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsGenericString
        {
            get
            {
                return this.m_bisGeneric ? "1" : "0";
            }
            set
            {
                this.m_bisGeneric = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Checks whether the resource is set to inactive
        /// </summary>
        [XmlIgnore()]
        public bool IsInactive
        {
            get
            {
                return this.m_bisInactive;
            }
            set
            {
                this.m_bisInactive = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("IsInactive")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsInactiveString
        {
            get
            {
                return this.m_bisInactive ? "1" : "0";
            }
            set
            {
                this.m_bisInactive = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Checks whether the resource is an Enterprise resource
        /// </summary>
        [XmlIgnore()]
        public bool IsEnterprise
        {
            get
            {
                return this.m_bisEnterprise;
            }
            set
            {
                this.m_bisEnterprise = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("IsEnterprise")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsEnterpriseString
        {
            get
            {
                return this.m_bisEnterprise ? "1" : "0";
            }
            set
            {
                this.m_bisEnterprise = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets the booking type of the resource
        /// </summary>
        [System.Xml.Serialization.XmlElement("BookingType")]
        public BookingType BookingType
        {
            get
            {
                return this.m_bookingType;
            }
            set
            {
                this.m_bookingType = value;
            }
        }

        /// <summary>
        /// Checks whether the booking type of the resource is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BookingTypeSpecified
        {
            get
            {
                return this.m_bbookingTypeSpecified;
            }
            set
            {
                this.m_bbookingTypeSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the duration through which actual work is protected
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="ActualWorkProtected",DataType = "duration")]
        public string ActualWorkProtected
        {
            get
            {
                return this.m_actualWorkProtected;
            }
            set
            {
                this.m_actualWorkProtected = value;
            }
        }

        /// <summary>
        /// Gets or sets the duration through which actual overtime work is protected
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="ActualOvertimeWorkProtected",DataType = "duration")]
        public string ActualOvertimeWorkProtected
        {
            get
            {
                return this.m_actualOvertimeWorkProtected;
            }
            set
            {
                this.m_actualOvertimeWorkProtected = value;
            }
        }

        /// <summary>
        /// Gets or sets the Active Directory GUID for the resource
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "ActiveDirectoryGUID", DataType = "string")]
        public string ActiveDirectoryGUID
        {
            get
            {
                return this.m_activeDirectoryGUID;
            }
            set
            {
                this.m_activeDirectoryGUID = value;
            }
        }

        /// <summary>
        /// Gets or sets the date that the resource was created
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "CreationDate", DataType = "dateTime")]
        public System.DateTime CreationDate
        {
            get
            {
                return this.m_creationDate;
            }
            set
            {
                this.m_creationDate = value;
            }
        }

        /// <summary>
        /// Checks whether the creation date of the resource is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CreationDateSpecified
        {
            get
            {
                return this.m_bcreationDateSpecified;
            }
            set
            {
                this.m_bcreationDateSpecified = value;
            }
        }*/

        /// <summary>
        /// Extended attribute
        /// </summary>
        [System.Xml.Serialization.XmlElement("ExtendedAttribute")]
        public List<ExtendedAttributesBase> ExtendedAttribute
        {
            get
            {
                return this.m_extendedAttribute;
            }
            set
            {
                this.m_extendedAttribute = value;
            }
        }

        /// <summary>
        /// Resource baseline
        /// </summary>
        [System.Xml.Serialization.XmlElement("Baseline")]
        public List<ResourceBaseline> Baseline
        {
            get
            {
                return this.m_baseline;
            }
            set
            {
                this.m_baseline = value;
            }
        }

        /// <summary>
        /// Outline code of the resource
        /// </summary>
        [System.Xml.Serialization.XmlElement("OutlineCode")]
        public List<OutlineCodeBase> OutlineCode
        {
            get
            {
                return this.m_outlineCode;
            }
            set
            {
                this.m_outlineCode = value;
            }
        }

        /// <summary>
        /// Checks whether the resource is a cost resource
        /// </summary>
        [XmlIgnore()]
        public bool IsCostResource
        {
            get
            {
                return this.m_bisCostResource;
            }
            set
            {
                this.m_bisCostResource = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("IsCostResource")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsCostResourceString
        {
            get
            {
                return this.m_bisCostResource ? "1" : "0";
            }
            set
            {
                this.m_bisCostResource = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the assignment owner
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "AssnOwner", DataType = "string")]
        public string AssnOwner
        {
            get
            {
                return this.m_assnOwner;
            }
            set
            {
                this.m_assnOwner = value;
            }
        }

        /// <summary>
        /// Gets or sets the GUID of the assignment owner
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "AssnOwnerGuid", DataType = "string")]
        public string AssnOwnerGuid
        {
            get
            {
                return this.m_assnOwnerGuid;
            }
            set
            {
                this.m_assnOwnerGuid = value;
            }
        }

        /// <summary>
        /// Checks whether the resource is a budget resource
        /// </summary>
        [XmlIgnore()]
        public bool IsBudget
        {
            get
            {
                return this.m_bisBudget;
            }
            set
            {
                this.m_bisBudget = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("IsBudget")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsBudgetString
        {
            get
            {
                return this.m_bisBudget ? "1" : "0";
            }
            set
            {
                this.m_bisBudget = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Resource Availability Periods
        /// </summary>
        [System.Xml.Serialization.XmlElement("AvailabilityPeriod", IsNullable = false)]
        public List<ResourceAvailabilityPeriod> AvailabilityPeriods
        {
            get
            {
                return this.m_availabilityPeriods;
            }
            set
            {
                this.m_availabilityPeriods = value;
            }
        }

        /// <summary>
        /// Resource rates
        /// </summary>
        [System.Xml.Serialization.XmlElement("Rate")]
        public List<ResourceRate> Rates
        {
            get
            {
                return this.m_rates;
            }
            set
            {
                this.m_rates = value;
            }
        }

        /// <summary>
        /// Timephased data
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
        #endregion
    }   
}

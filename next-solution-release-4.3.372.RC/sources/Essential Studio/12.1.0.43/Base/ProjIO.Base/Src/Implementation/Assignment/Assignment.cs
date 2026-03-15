#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Globalization;

namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Represents the allocation of a resource to a task
    /// </summary>
    [System.Serializable()]
    public class Assignment
    {
        #region Fields
        private int m_uID;
        private int m_taskUID;
        private Task m_task;
        private int m_resourceUID;
        private Resource m_resource;
        private int m_percentWorkComplete;
        private decimal m_actualCost;
        private bool m_bactualCostSpecified;
        private System.DateTime m_actualFinish;
        private bool m_bactualFinishSpecified;
        private decimal m_actualOvertimeCost;
        private bool m_bactualOvertimeCostSpecified;
        private string m_actualOvertimeWork;
        private System.DateTime m_actualStart;
        private bool m_bactualStartSpecified;
        private string m_actualWork;
        private float m_aCWP;
        private bool m_baCWPSpecified;
        private bool m_bconfirmed;
        private decimal m_cost;
        private bool m_bcostSpecified;
        private RateTable m_costRateTable;
        private bool m_bcostRateTableSpecified;
        private float m_costVariance;
        private bool m_bcostVarianceSpecified;
        private float m_cv;
        private bool m_bcvSpecified;
        private int m_delay;
        private System.DateTime m_finish;
        private bool m_bfinishSpecified;
        private int m_finishVariance;
        private string m_hyperlink;
        private string m_hyperlinkAddress;
        private string m_hyperlinkSubAddress;
        private float m_workVariance;
        private bool m_bworkVarianceSpecified;
        private bool m_bhasFixedRateUnits;
        private bool m_bfixedMaterial;
        private int m_levelingDelay;
        private DelayFormat m_levelingDelayFormat;
        private bool m_blevelingDelayFormatSpecified;
        private bool m_blinkedFields;
        private bool m_bmilestone;
        private string m_notes;
        private bool m_boverallocated;
        private decimal m_overtimeCost;
        private bool m_bovertimeCostSpecified;
        private string m_overtimeWork;
        private float m_peakUnits;
        private bool m_bpeakUnitsSpecified;
        private RateScale m_rateScale;
        private string m_regularWork;
        private decimal m_remainingCost;
        private bool m_bremainingCostSpecified;
        private decimal m_remainingOvertimeCost;
        private bool m_bremainingOvertimeCostSpecified;
        private string m_remainingOvertimeWork;
        private string m_remainingWork;
        private bool m_bresponsePending;
        private System.DateTime m_start;
        private bool m_bstartSpecified;
        private System.DateTime m_stop;
        private bool m_bstopSpecified;
        private System.DateTime m_resume;
        private bool m_bresumeSpecified;
        private int m_startVariance;
        private bool m_bsummary;
        private float m_sv;
        private bool m_bsvSpecified;
        private float m_units;
        private bool m_bupdateNeeded;
        private float m_vAC;
        private bool m_bvACSpecified;
        private string m_work;
        private AssignmentWorkContour m_workContour;
        private bool m_bworkContourSpecified;
        private float m_bCWS;
        private bool m_bbCWSSpecified;
        private float m_bCWP;
        private bool m_bbCWPSpecified;
        private BookingType m_bookingType;
        private bool m_bbookingTypeSpecified;
        private string m_actualWorkProtected;
        private string m_actualOvertimeWorkProtected;
        private System.DateTime m_creationDate;
        private bool m_bcreationDateSpecified;
        private string m_assnOwner;
        private string m_assnOwnerGuid;
        private decimal m_budgetCost;
        private bool m_bbudgetCostSpecified;
        private string m_budgetWork;
        private List<ExtendedAttributesBase> m_extendedAttribute=new List<ExtendedAttributesBase>();
        private List<AssignmentBaseline> m_baseline=new List<AssignmentBaseline>();
        private object m_f404000Field;
        private object m_f404001Field;
        private object m_f404002Field;
        private object m_f404003Field;
        private object m_f404004Field;
        private object m_f404005Field;
        private object m_f404006Field;
        private object m_f404007Field;
        private object m_f404008Field;
        private object m_f404009Field;
        private object m_f40400aField;
        private object m_f40400bField;
        private object m_f40400cField;
        private object m_f40400dField;
        private object m_f40400eField;
        private object m_f40400fField;
        private object m_f404010Field;
        private object m_f404011Field;        
        private object m_f404012Field;
        private object m_f404013Field;
        private object m_f404014Field;
        private object m_f404015Field;
        private object m_f404016Field;
        private object m_f404017Field;
        private object m_f404018Field;
        private object m_f404019Field;
        private object m_f40401aField;
        private object m_f40401bField;
        private object m_f40401cField;
        private object m_f40401dField;
        private object m_f40401eField;
        private object m_f40401fField;
        private object m_f404020Field;
        private object m_f404021Field;
        private object m_f404022Field;
        private object m_f404023Field;
        private object m_f404024Field;
        private object m_f404025Field;
        private object m_f404026Field;
        private object m_f404027Field;
        private object m_f404028Field;
        private object m_f404029Field;
        private object m_f40402aField;
        private object m_f40402bField;
        private object m_f40402cField;
        private object m_f40402dField;
        private object m_f40402eField;
        private object m_f40402fField;
        private object m_f404030Field;
        private object m_f404031Field;
        private object m_f404032Field;
        private object m_f404033Field;
        private object m_f404034Field;
        private object m_f404035Field;
        private object m_f404036Field;        
        private object m_f404037Field;
        private object m_f404038Field;
        private object m_f404039Field;
        private object m_f40403aField;
        private object m_f40403bField;
        private object m_f40403cField;
        private object m_f40403dField;
        private object m_f40403eField;
        private object m_f40403fField;
        private object m_f404040Field;
        private object m_f404041Field;
        private object m_f404042Field;
        private object m_f404043Field;
        private object m_f404044Field;
        private object m_f404045Field;
        private object m_f404046Field;
        private object m_f404047Field;
        private object m_f404048Field;
        private object m_f404049Field;
        private object m_f40404aField;
        private object m_f40404bField;
        private object m_f40404cField;
        private object m_f40404dField;
        private object m_f40404eField;
        private object m_f40404fField;
        private object m_f404050Field;
        private object m_f404051Field;
        private object m_f404052Field;
        private object m_f404053Field;
        private object m_f404054Field;
        private object m_f404055Field;
        private object m_f404056Field;
        private object m_f404057Field;
        private object m_f404058Field;
        private object m_f404059Field;
        private object m_f40405aField;
        private object m_f40405bField;
        private object m_f40405cField;
        private object m_f40405dField;
        private object m_f40405eField;
        private object m_f40405fField;
        private object m_f404060Field;
        private object m_f404061Field;
        private object m_f404062Field;
        private object m_f404063Field;
        private object m_f404064Field;
        private object m_f404065Field;
        private object m_f404066Field;
        private object m_f404067Field;        
        private object m_f404068Field;
        private object m_f404069Field;
        private object m_f40406aField;
        private object m_f40406bField;
        private object m_f40406cField;
        private object m_f40406dField;
        private object m_f40406eField;
        private object m_f40406fField;
        private object m_f404070Field;
        private object m_f404071Field;
        private object m_f404072Field;
        private object m_f404073Field;
        private object m_f404074Field;
        private object m_f404075Field;
        private object m_f404076Field;
        private object m_f404077Field;
        private object m_f404078Field;
        private object m_f404079Field;
        private object m_f40407aField;        
        private object m_f40407bField;
        private object m_f40407cField;
        private object m_f40407dField;
        private object m_f40407eField;
        private object m_f40407fField;
        private object m_f404080Field;
        private object m_f404081Field;
        private object m_f404082Field;
        private object m_f404083Field;
        private object m_f404084Field;
        private object m_f404085Field;
        private object m_f404086Field;
        private object m_f404087Field;        
        private object m_f404088Field;
        private object m_f404089Field;
        private object m_f40408aField;
        private object m_f40408bField;
        private object m_f40408cField;
        private object m_f40408dField;
        private object m_f40408eField;
        private object m_f40408fField;
        private object m_f404090Field;
        private object m_f404091Field;
        private object m_f404092Field;
        private object m_f404093Field;
        private object m_f404094Field;
        private object m_f404095Field;
        private object m_f404096Field;
        private object m_f404097Field;
        private object m_f404098Field;
        private object m_f404099Field;
        private object m_f40409aField;
        private object m_f40409bField;
        private object m_f40409cField;
        private object m_f40409dField;
        private object m_f40409eField;
        private object m_f40409fField;
        private object m_f4040a0Field;
        private object m_f4040a1Field;
        private object m_f4040a2Field;
        private object m_f4040a3Field;
        private object m_f4040a4Field;
        private object m_f4040a5Field;
        private object m_f4040a6Field;
        private object m_f4040a7Field;
        private object m_f4040a8Field;
        private object m_f4040a9Field;
        private object m_f4040aaField;
        private object m_f4040abField;
        private object m_f4040acField;
        private object m_f4040adField;
        private object m_f4040aeField;
        private object m_f4040afField;
        private object m_f4040b0Field;
        private object m_f4040b1Field;
        private object m_f4040b2Field;
        private object m_f4040b3Field;
        private object m_f4040b4Field;
        private object m_f4040b5Field;
        private object m_f4040b6Field;
        private object m_f4040b7Field;
        private object m_f4040b8Field;
        private object m_f4040b9Field;
        private object m_f4040baField;
        private object m_f4040bbField;
        private object m_f4040bcField;
        private object m_f4040bdField;
        private object m_f4040beField;
        private object m_f4040bfField;
        private object m_f4040c0Field;
        private object m_f4040c1Field;
        private object m_f4040c2Field;
        private object m_f4040c3Field;
        private object m_f4040c4Field;
        private object m_f4040c5Field;
        private object m_f4040c6Field;
        private object m_f4040c7Field;
        private object m_f4040c8Field;
        private List<TimephasedDataType> timephasedData = new List<TimephasedDataType>();
        #endregion

        #region Initializer
        public Assignment()
        { }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier of the assignment
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
        [XmlIgnore()]
        public Task Task
        {
            get
            {
                return this.m_task;
            }
            set
            {
                this.m_task = value;
            }
        }

        /// <summary>
        /// Gets or sets the unique idenifier of the task
        /// </summary>
        [XmlIgnore]
        public int TaskUID
        {
            get
            {
                if (this.m_task != null)
                    return this.m_task.UID;
                else
                    return this.m_taskUID;
            }
            set
            {
                if (this.m_task != null)
                    this.m_taskUID = this.m_task.UID;
                else
                    this.m_taskUID = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("TaskUID")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string TaskUIDSerialized
        {
            get
            {
                return TaskUID.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_taskUID);
            }
        }
        [XmlIgnore()]
        public Resource Resource
        {
            get
            {
                return this.m_resource;
            }
            set
            {
                this.m_resource = value;
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the resource
        /// </summary>
        [XmlIgnore]
        public int ResourceUID
        {
            get
            {
                if (this.m_resource != null)
                    return this.m_resource.UID;
                else
                    return this.m_resourceUID;
            }
            set
            {
                if (this.m_resource != null)
                    this.m_resourceUID = this.m_resource.UID;
                else
                    this.m_resourceUID = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("ResourceUID")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string ResourceUIDSerialized
        {
            get
            {
                return ResourceUID.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_resourceUID);
            }
        }
        

        /// <summary>
        /// Gets or sets the amount of work completed on the assignment
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
        /// Gets or sets the actual cost incurred on the assignment
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
        /// Checks if actual cost incurred on the assignment is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }

        /// <summary>
        /// Gets or sets the actual finish date of the assignment
        /// </summary>
        [XmlElement(ElementName="ActualFinish",DataType = "dateTime")]
        public System.DateTime ActualFinish
        {
            get
            {
                return this.m_actualFinish;
            }
            set
            {
                this.m_actualFinish = value;
            }
        }

        /// <summary>
        /// Checks if actual finish date of the assignment is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ActualFinishSpecified
        {
            get
            {
                return this.m_bactualFinishSpecified;
            }
            set
            {
                this.m_bactualFinishSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the actual overtime cost incurred on the assignment
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
        /// Checks if the actual overtime cost incurred on the assignment is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }

        /// <summary>
        /// Gets or sets the actual overtime work incurred on the assignment
        /// </summary>
        [XmlElement(ElementName="ActualOvertimeWork",DataType = "duration")]
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
        /// Gets or sets the acual start date of the assignment
        /// </summary>
        [XmlElement(ElementName="ActualStart",DataType="dateTime")]
        public System.DateTime ActualStart
        {
            get
            {
                return this.m_actualStart;
            }
            set
            {
                this.m_actualStart = value;
            }
        }

        /// <summary>
        /// Checks if actual start date of the assignment is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ActualStartSpecified
        {
            get
            {
                return this.m_bactualStartSpecified;
            }
            set
            {
                this.m_bactualStartSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of actual work incurred on the assignment
        /// </summary>
        [XmlElementAttribute(ElementName="ActualWork",DataType = "duration")]
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
        /// Gets or sets the actual cost of work performed on the assignment to-date
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
        /// Checks if the actual cost of work performed on the assignment to-date is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }

        /// <summary>
        /// Checks whether the Resource has accepted all of his or her assignments
        /// </summary>
        [XmlIgnore()]
        public bool Confirmed 
        {
            get
            {
                return this.m_bconfirmed;
            }
            set
            {
                this.m_bconfirmed = value;
            }
        }

        [XmlElement("Confirmed")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ConfirmedString
        {
            get
            {
                return Confirmed ? "1" : "0";
            }
            set
            {
                Confirmed = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets the projected or scheduled cost of the assignment
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
        /// Checks whether the projected or scheduled cost of the assignment is specified
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
        /// Gets or sets the cost rate table used for the assignment
        /// </summary>
        [XmlElement("CostRateTable")]
        public RateTable CostRateTable
        {
            get
            {
                return this.m_costRateTable;
            }
            set
            {
                this.m_costRateTable = value;
            }
        }

        /// <summary>
        /// Checks whether the cost rate table used for the assignment
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CostRateTableSpecified
        {
            get
            {
                return this.m_bcostRateTableSpecified;
            }
            set
            {
                this.m_bcostRateTableSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the difference between the cost and baseline cost for a resource
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
        /// Checks whether the difference between the cost and baseline cost for a resource is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }

        /// <summary>
        /// Gets or sets the earned value cost variance
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
        /// Checks whether the earned value cost variance is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }

        /// <summary>
        /// Gets or sets the amount that the assignment is delayed
        /// </summary>
        [XmlIgnore]
        public int Delay
        {
            get
            {
                return this.m_delay;
            }
            set
            {
                this.m_delay = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("Delay")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string DelaySerialized
        {
            get
            {
                return Delay.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_delay);
            }
        }
        /// <summary>
        /// Gets or sets the scheduled finish date of the assignment
        /// </summary>
        [XmlElement(ElementName = "Finish", DataType = "dateTime")]
        public System.DateTime Finish
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
        /// Checks whether the scheduled finish date of the assignment is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }

        /// <summary>
        /// Gets or sets the variance of the assignment finish date from the baseline finish date
        /// </summary>
        [XmlIgnore]
        public int FinishVariance
        {
            get
            {
                return this.m_finishVariance;
            }
            set
            {
                this.m_finishVariance = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("FinishVariance")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string FinishVarianceSerialized
        {
            get
            {
                return FinishVariance.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_finishVariance);
            }
        }
        /// <summary>
        /// Gets or sets the title of the hyperlink associated with the assignment
        /// </summary>
        [XmlElement(ElementName = "Hyperlink", DataType = "string")]
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
        /// Gets or sets the hyperlink associated with the assignment
        /// </summary>
        [XmlElement(ElementName = "HyperlinkAddress", DataType = "string")]
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
        /// Gets or sets the document bookmark of the hyperlink associated with the assignment
        /// </summary>
        [XmlElement(ElementName = "HyperlinkSubAddress", DataType = "string")]
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
        /// Gets or sets the variance of assignment work from the baseline work as minutes x 1000
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
        /// Checks whether the variance of assignment work from the baseline work as minutes x 1000 is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }

        /// <summary>
        /// Checks whether the Units are Fixed Rate
        /// </summary>
        [XmlIgnore()]
        public bool HasFixedRateUnits 
        {
            get
            {
                return this.m_bhasFixedRateUnits;
            }
            set
            {
                this.m_bhasFixedRateUnits = value;
            }
        }

        [XmlElement("HasFixedRateUnits")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string HasFixedRateUnitsString
        {
            get
            {
                return HasFixedRateUnits ? "1" : "0";
            }
            set
            {
                HasFixedRateUnits = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Checks whether the consumption of the assigned material resource occurs in a single, fixed amount
        /// </summary>
        [XmlIgnore()]
        public bool FixedMaterial 
        {
            get
            {
                return this.m_bfixedMaterial;
            }
            set
            {
                this.m_bfixedMaterial = value;
            }
        }
        
        [XmlElement("FixedMaterial")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string FixedMaterialString
        {
            get
            {
                return FixedMaterial ? "1" : "0";
            }
            set
            {
                FixedMaterial = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets the delay caused by leveling
        /// </summary>
        [XmlIgnore]
        public int LevelingDelay
        {
            get
            {
                return this.m_levelingDelay;
            }
            set
            {
                this.m_levelingDelay = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("LevelingDelay")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string LevelingDelaySerialized
        {
            get
            {
                return LevelingDelay.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_levelingDelay);
            }
        }
        /// <summary>
        /// Gets or sets the format for expressing the duration of the delay
        /// </summary>
        [XmlElement("LevelingDelayFormat")]
        public DelayFormat LevelingDelayFormat
        {
            get
            {
                return this.m_levelingDelayFormat;
            }
            set
            {
                this.m_levelingDelayFormat = value;
            }
        }

        /// <summary>
        /// Checks whether the format for expressing the duration of the delay is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LevelingDelayFormatSpecified
        {
            get
            {
                return this.m_blevelingDelayFormatSpecified;
            }
            set
            {
                this.m_blevelingDelayFormatSpecified = value;
            }
        }

        /// <summary>
        /// Checks whether the Project is linked to another OLE object
        /// </summary>
        [XmlIgnore()]
        public bool LinkedFields 
        { 
            get
            {
                return this.m_blinkedFields;
            }
            set
            {
                this.m_blinkedFields = value;
            }
        }
        
        [XmlElement("LinkedFields")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string LinkedFieldsString
        {
            get
            {
                return LinkedFields ? "1" : "0";
            }
            set
            {
                LinkedFields = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Checks whether the assignment is a milestone
        /// </summary>
        [XmlIgnore()]
        public bool IsMilestone 
        {
            get
            {
                return this.m_bmilestone;
            }
            set
            {
                this.m_bmilestone = value;
            }
        }
        
        [XmlElement("Milestone")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsMilestoneString
        {
            get
            {
                return IsMilestone ? "1" : "0";
            }
            set
            {
                IsMilestone = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets the text notes associated with the assignment
        /// </summary>
        [XmlElement(ElementName = "Notes", DataType = "string")]
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
        /// Checks whether the assignment is overallocated
        /// </summary>
        [XmlIgnore()]
        public bool IsOverallocated 
        {
            get
            {
                return this.m_boverallocated;
            }
            set
            {
                this.m_boverallocated = value;
            }
        }
        
        [XmlElement("Overallocated")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsOverallocatedString
        {
            get
            {
                return IsOverallocated ? "1" : "0";
            }
            set
            {
                IsOverallocated = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets the sum of the actual and remaining overtime cost of the assignment
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
        /// Checks whether the sum of the actual and remaining overtime cost of the assignment is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }

        /// <summary>
        /// Gets or sets the scheduled overtime work scheduled for the assignment
        /// </summary>
        [XmlElementAttribute(ElementName="OvertimeWork",DataType = "duration")]
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
        /// Gets or sets the largest number of units that a resource is assigned for a task
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
        /// Checks whether the largest number of units that a resource is assigned for a task is specified
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
        /// Gets or sets the time unit for the usage rate of the material resource assignment
        /// </summary>
        [XmlElement("RateScale")]
        public RateScale RateScale
        {
            get
            {
                return this.m_rateScale;
            }
            set
            {
                this.m_rateScale = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of non-overtime work scheduled for the assignment
        /// </summary>
        [XmlElement(ElementName="RegularWork",DataType = "duration")]
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
        /// Gets or sets the remaining projected cost of completing the assignment
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
        /// Checks whether the remaining projected cost of completing the assignment is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }

        /// <summary>
        /// Gets or sets the remaining projected overtime cost of completing the assignment
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
        /// Checks whether the remaining projected overtime cost of completing the assignment is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }

        /// <summary>
        /// Gets or sets the remaining overtime work scheduled to complete the assignment
        /// </summary>
        [XmlElement(ElementName="RemainingOvertimeWork",DataType = "duration")]
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
        /// Gets or sets the remaining work scheduled to complete the assignment
        /// </summary>
        [XmlElement(ElementName="RemainingWork",DataType = "duration")]
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
        /// Checks whether a response has been received for a TeamAssign message
        /// </summary>
        [XmlIgnore()]
        public bool ResponsePending 
        {
            get
            {
                return this.m_bresponsePending;
            }
            set
            {
                this.m_bresponsePending = value;
            }
        }
        
        [XmlElement("ResponsePending")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ResponsePendingString
        {
            get
            {
                return ResponsePending ? "1" : "0";
            }
            set
            {
                ResponsePending = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets the scheduled start date of the assignment
        /// </summary>
        [XmlElement(ElementName = "Start", DataType = "dateTime")]
        public System.DateTime Start
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
        /// Checks whether the scheduled start date of the assignment is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }

        /// <summary>
        /// Gets or sets the date that the assignment was stopped
        /// </summary>
        [XmlElement(ElementName = "Stop", DataType = "dateTime")]
        public System.DateTime Stop
        {
            get
            {
                return this.m_stop;
            }
            set
            {
                this.m_stop = value;
            }
        }

        /// <summary>
        /// Checks whether the date that the assignment was stopped is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool StopSpecified
        {
            get
            {
                return this.m_bstopSpecified;
            }
            set
            {
                this.m_bstopSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the date that the assignment resumed
        /// </summary>
        [XmlElement(ElementName = "Resume", DataType = "dateTime")]
        public System.DateTime Resume
        {
            get
            {
                return this.m_resume;
            }
            set
            {
                this.m_resume = value;
            }
        }

        /// <summary>
        /// Checks whether the date that the assignment resumed is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ResumeSpecified
        {
            get
            {
                return this.m_bresumeSpecified;
            }
            set
            {
                this.m_bresumeSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the variance of the assignment start date from the baseline start date
        /// </summary>
        [XmlIgnore]
        public int StartVariance
        {
            get
            {
                return this.m_startVariance;
            }
            set
            {
                this.m_startVariance = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("StartVariance")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string StartVarianceSerialized
        {
            get
            {
                return StartVariance.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_startVariance);
            }
        }
        /// <summary>
        /// Checks whether the task is a summary task
        /// </summary>
        [XmlIgnore()]
        public bool IsSummary 
        {
            get
            {
                return this.m_bsummary;
            }
            set
            {
                this.m_bsummary = value;
            }
        }
        
        [XmlElement("Summary")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsSummaryString
        {
            get
            {
                return IsSummary ? "1" : "0";
            }
            set
            {
                IsSummary = XmlConvert.ToBoolean(value);
            }
        }

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
        /// Checks whether the earned value schedule variance, through the project status date is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }

        /// <summary>
        /// Gets or sets the number of units for the assignment
        /// </summary>
        [XmlIgnore]
        public float Units
        {
            get
            {
                return this.m_units;
            }
            set
            {
                this.m_units = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("Units")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string UnitsSerialized
        {
            get
            {
                return Units.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_units);
            }
        }
         /// <summary>
        /// Checks whether the resource assigned to a task needs to be updated as to the status of the task
        /// </summary>
        [XmlIgnore()]
        public bool UpdateNeeded 
        {
            get
            {
                return this.m_bupdateNeeded;
            }
            set
            {
                this.m_bupdateNeeded = value;
            }
        }
        
        [XmlElement("UpdateNeeded")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string UpdateNeededString
        {
            get
            {
                return UpdateNeeded ? "1" : "0";
            }
            set
            {
                UpdateNeeded = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets the difference between baseline cost and total cost
        /// </summary>
        [XmlIgnore]
        public float VAC
        {
            get
            {
                return this.m_vAC;
            }
            set
            {
                this.m_vAC = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("VAC")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string VACSerialized
        {
            get
            {
                return VAC.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_vAC);
            }
        }
        /// <summary>
        /// Checks whether the difference between baseline cost and total cost is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool VACSpecified
        {
            get
            {
                return this.m_bvACSpecified;
            }
            set
            {
                this.m_bvACSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of scheduled work for the assignment
        /// </summary>
        [XmlElement(ElementName="Work",DataType = "duration")]
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
        /// Gets or sets the work contour of the assignment
        /// </summary>
        [XmlElement("WorkContour")]
        public AssignmentWorkContour WorkContour
        {
            get
            {
                return this.m_workContour;
            }
            set
            {
                this.m_workContour = value;
            }
        }

        /// <summary>
        /// Checks whether the work contour of the assignment is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool WorkContourSpecified
        {
            get
            {
                return this.m_bworkContourSpecified;
            }
            set
            {
                this.m_bworkContourSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the budgeted cost of work on the assignment
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
        /// Checks whether the budgeted cost of work on the assignment is specified
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
        /// Gets or sets the budgeted cost of work performed on the assignment to-date
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
        /// Checks whether the budgeted cost of work performed on the assignment to-date is specified
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

        /// <summary>
        /// Gets or sets the booking type of the assignment
        /// </summary>
        [XmlElement("BookingType")]
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
        /// Checks whether the booking type of the assignment is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }

        /// <summary>
        /// Gets or sets the duration through which actual work is protected
        /// </summary>
        [XmlElement(ElementName="ActualWorkProtected",DataType = "duration")]
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
        [XmlElement(ElementName="ActualOvertimeWorkProtected",DataType = "duration")]
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
        /// Gets or sets the date that the assignment was created
        /// </summary>
        [XmlElement(ElementName = "CreationDate", DataType = "dateTime")]
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
        /// Checks whether the date that the assignment was created is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }

        /// <summary>
        /// Gets or sets the name of the assignment owner
        /// </summary>
        [XmlElement(ElementName = "AssnOwner", DataType = "string")]
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
        [XmlElement(ElementName = "AssnOwnerGuid", DataType = "string")]
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
        /// Gets or sets the budgeted amount for cost resources on this assignment
        /// </summary>
        [XmlIgnore]
        public decimal BudgetCost
        {
            get
            {
                return this.m_budgetCost;
            }
            set
            {
                this.m_budgetCost = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("BudgetCost")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string BudgetCostSerialized
        {
            get
            {
                return BudgetCost.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_budgetCost);
            }
        }
        /// <summary>
        /// Checks whether the budgeted amount for cost resources on this assignment is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BudgetCostSpecified
        {
            get
            {
                return this.m_bbudgetCostSpecified;
            }
            set
            {
                this.m_bbudgetCostSpecified = value;
            }
        }

        /// <summary>
        /// Gets os sets the budgeted work amount for work or material resources on this assignment
        /// </summary>
        [XmlElement(ElementName="BudgetWork",DataType = "duration")]
        public string BudgetWork
        {
            get
            {
                return this.m_budgetWork;
            }
            set
            {
                this.m_budgetWork = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlArrayItem("ExtendedAttributesBase",IsNullable=false)]
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
        /// Gets or sets the collection of baseline values associated with the assignment
        /// </summary>
        [System.Xml.Serialization.XmlArrayItem("AssignmentBaseline", IsNullable = false)]
        public List<AssignmentBaseline> Baseline
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

        /// <remarks/>
        public object f404000
        {
            get
            {
                return this.m_f404000Field;
            }
            set
            {
                this.m_f404000Field = value;
            }
        }

        /// <remarks/>
        public object f404001
        {
            get
            {
                return this.m_f404001Field;
            }
            set
            {
                this.m_f404001Field = value;
            }
        }

        /// <remarks/>
        public object f404002
        {
            get
            {
                return this.m_f404002Field;
            }
            set
            {
                this.m_f404002Field = value;
            }
        }

        /// <remarks/>
        public object f404003
        {
            get
            {
                return this.m_f404003Field;
            }
            set
            {
                this.m_f404003Field = value;
            }
        }

        /// <remarks/>
        public object f404004
        {
            get
            {
                return this.m_f404004Field;
            }
            set
            {
                this.m_f404004Field = value;
            }
        }

        /// <remarks/>
        public object f404005
        {
            get
            {
                return this.m_f404005Field;
            }
            set
            {
                this.m_f404005Field = value;
            }
        }

        /// <remarks/>
        public object f404006
        {
            get
            {
                return this.m_f404006Field;
            }
            set
            {
                this.m_f404006Field = value;
            }
        }

        /// <remarks/>
        public object f404007
        {
            get
            {
                return this.m_f404007Field;
            }
            set
            {
                this.m_f404007Field = value;
            }
        }

        /// <remarks/>
        public object f404008
        {
            get
            {
                return this.m_f404008Field;
            }
            set
            {
                this.m_f404008Field = value;
            }
        }

        /// <remarks/>
        public object f404009
        {
            get
            {
                return this.m_f404009Field;
            }
            set
            {
                this.m_f404009Field = value;
            }
        }

        /// <remarks/>
        public object f40400a
        {
            get
            {
                return this.m_f40400aField;
            }
            set
            {
                this.m_f40400aField = value;
            }
        }

        /// <remarks/>
        public object f40400b
        {
            get
            {
                return this.m_f40400bField;
            }
            set
            {
                this.m_f40400bField = value;
            }
        }

        /// <remarks/>
        public object f40400c
        {
            get
            {
                return this.m_f40400cField;
            }
            set
            {
                this.m_f40400cField = value;
            }
        }

        /// <remarks/>
        public object f40400d
        {
            get
            {
                return this.m_f40400dField;
            }
            set
            {
                this.m_f40400dField = value;
            }
        }

        /// <remarks/>
        public object f40400e
        {
            get
            {
                return this.m_f40400eField;
            }
            set
            {
                this.m_f40400eField = value;
            }
        }

        /// <remarks/>
        public object f40400f
        {
            get
            {
                return this.m_f40400fField;
            }
            set
            {
                this.m_f40400fField = value;
            }
        }

        /// <remarks/>
        public object f404010
        {
            get
            {
                return this.m_f404010Field;
            }
            set
            {
                this.m_f404010Field = value;
            }
        }

        /// <remarks/>
        public object f404011
        {
            get
            {
                return this.m_f404011Field;
            }
            set
            {
                this.m_f404011Field = value;
            }
        }

        /// <remarks/>
        public object f404012
        {
            get
            {
                return this.m_f404012Field;
            }
            set
            {
                this.m_f404012Field = value;
            }
        }

        /// <remarks/>
        public object f404013
        {
            get
            {
                return this.m_f404013Field;
            }
            set
            {
                this.m_f404013Field = value;
            }
        }

        /// <remarks/>
        public object f404014
        {
            get
            {
                return this.m_f404014Field;
            }
            set
            {
                this.m_f404014Field = value;
            }
        }

        /// <remarks/>
        public object f404015
        {
            get
            {
                return this.m_f404015Field;
            }
            set
            {
                this.m_f404015Field = value;
            }
        }

        /// <remarks/>
        public object f404016
        {
            get
            {
                return this.m_f404016Field;
            }
            set
            {
                this.m_f404016Field = value;
            }
        }

        /// <remarks/>
        public object f404017
        {
            get
            {
                return this.m_f404017Field;
            }
            set
            {
                this.m_f404017Field = value;
            }
        }

        /// <remarks/>
        public object f404018
        {
            get
            {
                return this.m_f404018Field;
            }
            set
            {
                this.m_f404018Field = value;
            }
        }

        /// <remarks/>
        public object f404019
        {
            get
            {
                return this.m_f404019Field;
            }
            set
            {
                this.m_f404019Field = value;
            }
        }

        /// <remarks/>
        public object f40401a
        {
            get
            {
                return this.m_f40401aField;
            }
            set
            {
                this.m_f40401aField = value;
            }
        }

        /// <remarks/>
        public object f40401b
        {
            get
            {
                return this.m_f40401bField;
            }
            set
            {
                this.m_f40401bField = value;
            }
        }

        /// <remarks/>
        public object f40401c
        {
            get
            {
                return this.m_f40401cField;
            }
            set
            {
                this.m_f40401cField = value;
            }
        }

        /// <remarks/>
        public object f40401d
        {
            get
            {
                return this.m_f40401dField;
            }
            set
            {
                this.m_f40401dField = value;
            }
        }

        /// <remarks/>
        public object f40401e
        {
            get
            {
                return this.m_f40401eField;
            }
            set
            {
                this.m_f40401eField = value;
            }
        }

        /// <remarks/>
        public object f40401f
        {
            get
            {
                return this.m_f40401fField;
            }
            set
            {
                this.m_f40401fField = value;
            }
        }

        /// <remarks/>
        public object f404020
        {
            get
            {
                return this.m_f404020Field;
            }
            set
            {
                this.m_f404020Field = value;
            }
        }

        /// <remarks/>
        public object f404021
        {
            get
            {
                return this.m_f404021Field;
            }
            set
            {
                this.m_f404021Field = value;
            }
        }

        /// <remarks/>
        public object f404022
        {
            get
            {
                return this.m_f404022Field;
            }
            set
            {
                this.m_f404022Field = value;
            }
        }

        /// <remarks/>
        public object f404023
        {
            get
            {
                return this.m_f404023Field;
            }
            set
            {
                this.m_f404023Field = value;
            }
        }

        /// <remarks/>
        public object f404024
        {
            get
            {
                return this.m_f404024Field;
            }
            set
            {
                this.m_f404024Field = value;
            }
        }

        /// <remarks/>
        public object f404025
        {
            get
            {
                return this.m_f404025Field;
            }
            set
            {
                this.m_f404025Field = value;
            }
        }

        /// <remarks/>
        public object f404026
        {
            get
            {
                return this.m_f404026Field;
            }
            set
            {
                this.m_f404026Field = value;
            }
        }

        /// <remarks/>
        public object f404027
        {
            get
            {
                return this.m_f404027Field;
            }
            set
            {
                this.m_f404027Field = value;
            }
        }

        /// <remarks/>
        public object f404028
        {
            get
            {
                return this.m_f404028Field;
            }
            set
            {
                this.m_f404028Field = value;
            }
        }

        /// <remarks/>
        public object f404029
        {
            get
            {
                return this.m_f404029Field;
            }
            set
            {
                this.m_f404029Field = value;
            }
        }

        /// <remarks/>
        public object f40402a
        {
            get
            {
                return this.m_f40402aField;
            }
            set
            {
                this.m_f40402aField = value;
            }
        }

        /// <remarks/>
        public object f40402b
        {
            get
            {
                return this.m_f40402bField;
            }
            set
            {
                this.m_f40402bField = value;
            }
        }

        /// <remarks/>
        public object f40402c
        {
            get
            {
                return this.m_f40402cField;
            }
            set
            {
                this.m_f40402cField = value;
            }
        }

        /// <remarks/>
        public object f40402d
        {
            get
            {
                return this.m_f40402dField;
            }
            set
            {
                this.m_f40402dField = value;
            }
        }

        /// <remarks/>
        public object f40402e
        {
            get
            {
                return this.m_f40402eField;
            }
            set
            {
                this.m_f40402eField = value;
            }
        }

        /// <remarks/>
        public object f40402f
        {
            get
            {
                return this.m_f40402fField;
            }
            set
            {
                this.m_f40402fField = value;
            }
        }

        /// <remarks/>
        public object f404030
        {
            get
            {
                return this.m_f404030Field;
            }
            set
            {
                this.m_f404030Field = value;
            }
        }

        /// <remarks/>
        public object f404031
        {
            get
            {
                return this.m_f404031Field;
            }
            set
            {
                this.m_f404031Field = value;
            }
        }

        /// <remarks/>
        public object f404032
        {
            get
            {
                return this.m_f404032Field;
            }
            set
            {
                this.m_f404032Field = value;
            }
        }

        /// <remarks/>
        public object f404033
        {
            get
            {
                return this.m_f404033Field;
            }
            set
            {
                this.m_f404033Field = value;
            }
        }

        /// <remarks/>
        public object f404034
        {
            get
            {
                return this.m_f404034Field;
            }
            set
            {
                this.m_f404034Field = value;
            }
        }

        /// <remarks/>
        public object f404035
        {
            get
            {
                return this.m_f404035Field;
            }
            set
            {
                this.m_f404035Field = value;
            }
        }

        /// <remarks/>
        public object f404036
        {
            get
            {
                return this.m_f404036Field;
            }
            set
            {
                this.m_f404036Field = value;
            }
        }

        /// <remarks/>
        public object f404037
        {
            get
            {
                return this.m_f404037Field;
            }
            set
            {
                this.m_f404037Field = value;
            }
        }

        /// <remarks/>
        public object f404038
        {
            get
            {
                return this.m_f404038Field;
            }
            set
            {
                this.m_f404038Field = value;
            }
        }

        /// <remarks/>
        public object f404039
        {
            get
            {
                return this.m_f404039Field;
            }
            set
            {
                this.m_f404039Field = value;
            }
        }

        /// <remarks/>
        public object f40403a
        {
            get
            {
                return this.m_f40403aField;
            }
            set
            {
                this.m_f40403aField = value;
            }
        }

        /// <remarks/>
        public object f40403b
        {
            get
            {
                return this.m_f40403bField;
            }
            set
            {
                this.m_f40403bField = value;
            }
        }

        /// <remarks/>
        public object f40403c
        {
            get
            {
                return this.m_f40403cField;
            }
            set
            {
                this.m_f40403cField = value;
            }
        }

        /// <remarks/>
        public object f40403d
        {
            get
            {
                return this.m_f40403dField;
            }
            set
            {
                this.m_f40403dField = value;
            }
        }

        /// <remarks/>
        public object f40403e
        {
            get
            {
                return this.m_f40403eField;
            }
            set
            {
                this.m_f40403eField = value;
            }
        }

        /// <remarks/>
        public object f40403f
        {
            get
            {
                return this.m_f40403fField;
            }
            set
            {
                this.m_f40403fField = value;
            }
        }

        /// <remarks/>
        public object f404040
        {
            get
            {
                return this.m_f404040Field;
            }
            set
            {
                this.m_f404040Field = value;
            }
        }

        /// <remarks/>
        public object f404041
        {
            get
            {
                return this.m_f404041Field;
            }
            set
            {
                this.m_f404041Field = value;
            }
        }

        /// <remarks/>
        public object f404042
        {
            get
            {
                return this.m_f404042Field;
            }
            set
            {
                this.m_f404042Field = value;
            }
        }

        /// <remarks/>
        public object f404043
        {
            get
            {
                return this.m_f404043Field;
            }
            set
            {
                this.m_f404043Field = value;
            }
        }

        /// <remarks/>
        public object f404044
        {
            get
            {
                return this.m_f404044Field;
            }
            set
            {
                this.m_f404044Field = value;
            }
        }

        /// <remarks/>
        public object f404045
        {
            get
            {
                return this.m_f404045Field;
            }
            set
            {
                this.m_f404045Field = value;
            }
        }

        /// <remarks/>
        public object f404046
        {
            get
            {
                return this.m_f404046Field;
            }
            set
            {
                this.m_f404046Field = value;
            }
        }

        /// <remarks/>
        public object f404047
        {
            get
            {
                return this.m_f404047Field;
            }
            set
            {
                this.m_f404047Field = value;
            }
        }

        /// <remarks/>
        public object f404048
        {
            get
            {
                return this.m_f404048Field;
            }
            set
            {
                this.m_f404048Field = value;
            }
        }

        /// <remarks/>
        public object f404049
        {
            get
            {
                return this.m_f404049Field;
            }
            set
            {
                this.m_f404049Field = value;
            }
        }

        /// <remarks/>
        public object f40404a
        {
            get
            {
                return this.m_f40404aField;
            }
            set
            {
                this.m_f40404aField = value;
            }
        }

        /// <remarks/>
        public object f40404b
        {
            get
            {
                return this.m_f40404bField;
            }
            set
            {
                this.m_f40404bField = value;
            }
        }

        /// <remarks/>
        public object f40404c
        {
            get
            {
                return this.m_f40404cField;
            }
            set
            {
                this.m_f40404cField = value;
            }
        }

        /// <remarks/>
        public object f40404d
        {
            get
            {
                return this.m_f40404dField;
            }
            set
            {
                this.m_f40404dField = value;
            }
        }

        /// <remarks/>
        public object f40404e
        {
            get
            {
                return this.m_f40404eField;
            }
            set
            {
                this.m_f40404eField = value;
            }
        }

        /// <remarks/>
        public object f40404f
        {
            get
            {
                return this.m_f40404fField;
            }
            set
            {
                this.m_f40404fField = value;
            }
        }

        /// <remarks/>
        public object f404050
        {
            get
            {
                return this.m_f404050Field;
            }
            set
            {
                this.m_f404050Field = value;
            }
        }

        /// <remarks/>
        public object f404051
        {
            get
            {
                return this.m_f404051Field;
            }
            set
            {
                this.m_f404051Field = value;
            }
        }

        /// <remarks/>
        public object f404052
        {
            get
            {
                return this.m_f404052Field;
            }
            set
            {
                this.m_f404052Field = value;
            }
        }

        /// <remarks/>
        public object f404053
        {
            get
            {
                return this.m_f404053Field;
            }
            set
            {
                this.m_f404053Field = value;
            }
        }

        /// <remarks/>
        public object f404054
        {
            get
            {
                return this.m_f404054Field;
            }
            set
            {
                this.m_f404054Field = value;
            }
        }

        /// <remarks/>
        public object f404055
        {
            get
            {
                return this.m_f404055Field;
            }
            set
            {
                this.m_f404055Field = value;
            }
        }

        /// <remarks/>
        public object f404056
        {
            get
            {
                return this.m_f404056Field;
            }
            set
            {
                this.m_f404056Field = value;
            }
        }

        /// <remarks/>
        public object f404057
        {
            get
            {
                return this.m_f404057Field;
            }
            set
            {
                this.m_f404057Field = value;
            }
        }

        /// <remarks/>
        public object f404058
        {
            get
            {
                return this.m_f404058Field;
            }
            set
            {
                this.m_f404058Field = value;
            }
        }

        /// <remarks/>
        public object f404059
        {
            get
            {
                return this.m_f404059Field;
            }
            set
            {
                this.m_f404059Field = value;
            }
        }

        /// <remarks/>
        public object f40405a
        {
            get
            {
                return this.m_f40405aField;
            }
            set
            {
                this.m_f40405aField = value;
            }
        }

        /// <remarks/>
        public object f40405b
        {
            get
            {
                return this.m_f40405bField;
            }
            set
            {
                this.m_f40405bField = value;
            }
        }

        /// <remarks/>
        public object f40405c
        {
            get
            {
                return this.m_f40405cField;
            }
            set
            {
                this.m_f40405cField = value;
            }
        }

        /// <remarks/>
        public object f40405d
        {
            get
            {
                return this.m_f40405dField;
            }
            set
            {
                this.m_f40405dField = value;
            }
        }

        /// <remarks/>
        public object f40405e
        {
            get
            {
                return this.m_f40405eField;
            }
            set
            {
                this.m_f40405eField = value;
            }
        }

        /// <remarks/>
        public object f40405f
        {
            get
            {
                return this.m_f40405fField;
            }
            set
            {
                this.m_f40405fField = value;
            }
        }

        /// <remarks/>
        public object f404060
        {
            get
            {
                return this.m_f404060Field;
            }
            set
            {
                this.m_f404060Field = value;
            }
        }

        /// <remarks/>
        public object f404061
        {
            get
            {
                return this.m_f404061Field;
            }
            set
            {
                this.m_f404061Field = value;
            }
        }

        /// <remarks/>
        public object f404062
        {
            get
            {
                return this.m_f404062Field;
            }
            set
            {
                this.m_f404062Field = value;
            }
        }

        /// <remarks/>
        public object f404063
        {
            get
            {
                return this.m_f404063Field;
            }
            set
            {
                this.m_f404063Field = value;
            }
        }

        /// <remarks/>
        public object f404064
        {
            get
            {
                return this.m_f404064Field;
            }
            set
            {
                this.m_f404064Field = value;
            }
        }

        /// <remarks/>
        public object f404065
        {
            get
            {
                return this.m_f404065Field;
            }
            set
            {
                this.m_f404065Field = value;
            }
        }

        /// <remarks/>
        public object f404066
        {
            get
            {
                return this.m_f404066Field;
            }
            set
            {
                this.m_f404066Field = value;
            }
        }

        /// <remarks/>
        public object f404067
        {
            get
            {
                return this.m_f404067Field;
            }
            set
            {
                this.m_f404067Field = value;
            }
        }

        /// <remarks/>
        public object f404068
        {
            get
            {
                return this.m_f404068Field;
            }
            set
            {
                this.m_f404068Field = value;
            }
        }

        /// <remarks/>
        public object f404069
        {
            get
            {
                return this.m_f404069Field;
            }
            set
            {
                this.m_f404069Field = value;
            }
        }

        /// <remarks/>
        public object f40406a
        {
            get
            {
                return this.m_f40406aField;
            }
            set
            {
                this.m_f40406aField = value;
            }
        }

        /// <remarks/>
        public object f40406b
        {
            get
            {
                return this.m_f40406bField;
            }
            set
            {
                this.m_f40406bField = value;
            }
        }

        /// <remarks/>
        public object f40406c
        {
            get
            {
                return this.m_f40406cField;
            }
            set
            {
                this.m_f40406cField = value;
            }
        }

        /// <remarks/>
        public object f40406d
        {
            get
            {
                return this.m_f40406dField;
            }
            set
            {
                this.m_f40406dField = value;
            }
        }

        /// <remarks/>
        public object f40406e
        {
            get
            {
                return this.m_f40406eField;
            }
            set
            {
                this.m_f40406eField = value;
            }
        }

        /// <remarks/>
        public object f40406f
        {
            get
            {
                return this.m_f40406fField;
            }
            set
            {
                this.m_f40406fField = value;
            }
        }

        /// <remarks/>
        public object f404070
        {
            get
            {
                return this.m_f404070Field;
            }
            set
            {
                this.m_f404070Field = value;
            }
        }

        /// <remarks/>
        public object f404071
        {
            get
            {
                return this.m_f404071Field;
            }
            set
            {
                this.m_f404071Field = value;
            }
        }

        /// <remarks/>
        public object f404072
        {
            get
            {
                return this.m_f404072Field;
            }
            set
            {
                this.m_f404072Field = value;
            }
        }

        /// <remarks/>
        public object f404073
        {
            get
            {
                return this.m_f404073Field;
            }
            set
            {
                this.m_f404073Field = value;
            }
        }

        /// <remarks/>
        public object f404074
        {
            get
            {
                return this.m_f404074Field;
            }
            set
            {
                this.m_f404074Field = value;
            }
        }

        /// <remarks/>
        public object f404075
        {
            get
            {
                return this.m_f404075Field;
            }
            set
            {
                this.m_f404075Field = value;
            }
        }

        /// <remarks/>
        public object f404076
        {
            get
            {
                return this.m_f404076Field;
            }
            set
            {
                this.m_f404076Field = value;
            }
        }

        /// <remarks/>
        public object f404077
        {
            get
            {
                return this.m_f404077Field;
            }
            set
            {
                this.m_f404077Field = value;
            }
        }

        /// <remarks/>
        public object f404078
        {
            get
            {
                return this.m_f404078Field;
            }
            set
            {
                this.m_f404078Field = value;
            }
        }

        /// <remarks/>
        public object f404079
        {
            get
            {
                return this.m_f404079Field;
            }
            set
            {
                this.m_f404079Field = value;
            }
        }

        /// <remarks/>
        public object f40407a
        {
            get
            {
                return this.m_f40407aField;
            }
            set
            {
                this.m_f40407aField = value;
            }
        }

        /// <remarks/>
        public object f40407b
        {
            get
            {
                return this.m_f40407bField;
            }
            set
            {
                this.m_f40407bField = value;
            }
        }

        /// <remarks/>
        public object f40407c
        {
            get
            {
                return this.m_f40407cField;
            }
            set
            {
                this.m_f40407cField = value;
            }
        }

        /// <remarks/>
        public object f40407d
        {
            get
            {
                return this.m_f40407dField;
            }
            set
            {
                this.m_f40407dField = value;
            }
        }

        /// <remarks/>
        public object f40407e
        {
            get
            {
                return this.m_f40407eField;
            }
            set
            {
                this.m_f40407eField = value;
            }
        }

        /// <remarks/>
        public object f40407f
        {
            get
            {
                return this.m_f40407fField;
            }
            set
            {
                this.m_f40407fField = value;
            }
        }

        /// <remarks/>
        public object f404080
        {
            get
            {
                return this.m_f404080Field;
            }
            set
            {
                this.m_f404080Field = value;
            }
        }

        /// <remarks/>
        public object f404081
        {
            get
            {
                return this.m_f404081Field;
            }
            set
            {
                this.m_f404081Field = value;
            }
        }

        /// <remarks/>
        public object f404082
        {
            get
            {
                return this.m_f404082Field;
            }
            set
            {
                this.m_f404082Field = value;
            }
        }

        /// <remarks/>
        public object f404083
        {
            get
            {
                return this.m_f404083Field;
            }
            set
            {
                this.m_f404083Field = value;
            }
        }

        /// <remarks/>
        public object f404084
        {
            get
            {
                return this.m_f404084Field;
            }
            set
            {
                this.m_f404084Field = value;
            }
        }

        /// <remarks/>
        public object f404085
        {
            get
            {
                return this.m_f404085Field;
            }
            set
            {
                this.m_f404085Field = value;
            }
        }

        /// <remarks/>
        public object f404086
        {
            get
            {
                return this.m_f404086Field;
            }
            set
            {
                this.m_f404086Field = value;
            }
        }

        /// <remarks/>
        public object f404087
        {
            get
            {
                return this.m_f404087Field;
            }
            set
            {
                this.m_f404087Field = value;
            }
        }

        /// <remarks/>
        public object f404088
        {
            get
            {
                return this.m_f404088Field;
            }
            set
            {
                this.m_f404088Field = value;
            }
        }

        /// <remarks/>
        public object f404089
        {
            get
            {
                return this.m_f404089Field;
            }
            set
            {
                this.m_f404089Field = value;
            }
        }

        /// <remarks/>
        public object f40408a
        {
            get
            {
                return this.m_f40408aField;
            }
            set
            {
                this.m_f40408aField = value;
            }
        }

        /// <remarks/>
        public object f40408b
        {
            get
            {
                return this.m_f40408bField;
            }
            set
            {
                this.m_f40408bField = value;
            }
        }

        /// <remarks/>
        public object f40408c
        {
            get
            {
                return this.m_f40408cField;
            }
            set
            {
                this.m_f40408cField = value;
            }
        }

        /// <remarks/>
        public object f40408d
        {
            get
            {
                return this.m_f40408dField;
            }
            set
            {
                this.m_f40408dField = value;
            }
        }

        /// <remarks/>
        public object f40408e
        {
            get
            {
                return this.m_f40408eField;
            }
            set
            {
                this.m_f40408eField = value;
            }
        }

        /// <remarks/>
        public object f40408f
        {
            get
            {
                return this.m_f40408fField;
            }
            set
            {
                this.m_f40408fField = value;
            }
        }

        /// <remarks/>
        public object f404090
        {
            get
            {
                return this.m_f404090Field;
            }
            set
            {
                this.m_f404090Field = value;
            }
        }

        /// <remarks/>
        public object f404091
        {
            get
            {
                return this.m_f404091Field;
            }
            set
            {
                this.m_f404091Field = value;
            }
        }

        /// <remarks/>
        public object f404092
        {
            get
            {
                return this.m_f404092Field;
            }
            set
            {
                this.m_f404092Field = value;
            }
        }

        /// <remarks/>
        public object f404093
        {
            get
            {
                return this.m_f404093Field;
            }
            set
            {
                this.m_f404093Field = value;
            }
        }

        /// <remarks/>
        public object f404094
        {
            get
            {
                return this.m_f404094Field;
            }
            set
            {
                this.m_f404094Field = value;
            }
        }

        /// <remarks/>
        public object f404095
        {
            get
            {
                return this.m_f404095Field;
            }
            set
            {
                this.m_f404095Field = value;
            }
        }

        /// <remarks/>
        public object f404096
        {
            get
            {
                return this.m_f404096Field;
            }
            set
            {
                this.m_f404096Field = value;
            }
        }

        /// <remarks/>
        public object f404097
        {
            get
            {
                return this.m_f404097Field;
            }
            set
            {
                this.m_f404097Field = value;
            }
        }

        /// <remarks/>
        public object f404098
        {
            get
            {
                return this.m_f404098Field;
            }
            set
            {
                this.m_f404098Field = value;
            }
        }

        /// <remarks/>
        public object f404099
        {
            get
            {
                return this.m_f404099Field;
            }
            set
            {
                this.m_f404099Field = value;
            }
        }

        /// <remarks/>
        public object f40409a
        {
            get
            {
                return this.m_f40409aField;
            }
            set
            {
                this.m_f40409aField = value;
            }
        }

        /// <remarks/>
        public object f40409b
        {
            get
            {
                return this.m_f40409bField;
            }
            set
            {
                this.m_f40409bField = value;
            }
        }

        /// <remarks/>
        public object f40409c
        {
            get
            {
                return this.m_f40409cField;
            }
            set
            {
                this.m_f40409cField = value;
            }
        }

        /// <remarks/>
        public object f40409d
        {
            get
            {
                return this.m_f40409dField;
            }
            set
            {
                this.m_f40409dField = value;
            }
        }

        /// <remarks/>
        public object f40409e
        {
            get
            {
                return this.m_f40409eField;
            }
            set
            {
                this.m_f40409eField = value;
            }
        }

        /// <remarks/>
        public object f40409f
        {
            get
            {
                return this.m_f40409fField;
            }
            set
            {
                this.m_f40409fField = value;
            }
        }

        /// <remarks/>
        public object f4040a0
        {
            get
            {
                return this.m_f4040a0Field;
            }
            set
            {
                this.m_f4040a0Field = value;
            }
        }

        /// <remarks/>
        public object f4040a1
        {
            get
            {
                return this.m_f4040a1Field;
            }
            set
            {
                this.m_f4040a1Field = value;
            }
        }

        /// <remarks/>
        public object f4040a2
        {
            get
            {
                return this.m_f4040a2Field;
            }
            set
            {
                this.m_f4040a2Field = value;
            }
        }

        /// <remarks/>
        public object f4040a3
        {
            get
            {
                return this.m_f4040a3Field;
            }
            set
            {
                this.m_f4040a3Field = value;
            }
        }

        /// <remarks/>
        public object f4040a4
        {
            get
            {
                return this.m_f4040a4Field;
            }
            set
            {
                this.m_f4040a4Field = value;
            }
        }

        /// <remarks/>
        public object f4040a5
        {
            get
            {
                return this.m_f4040a5Field;
            }
            set
            {
                this.m_f4040a5Field = value;
            }
        }

        /// <remarks/>
        public object f4040a6
        {
            get
            {
                return this.m_f4040a6Field;
            }
            set
            {
                this.m_f4040a6Field = value;
            }
        }

        /// <remarks/>
        public object f4040a7
        {
            get
            {
                return this.m_f4040a7Field;
            }
            set
            {
                this.m_f4040a7Field = value;
            }
        }

        /// <remarks/>
        public object f4040a8
        {
            get
            {
                return this.m_f4040a8Field;
            }
            set
            {
                this.m_f4040a8Field = value;
            }
        }

        /// <remarks/>
        public object f4040a9
        {
            get
            {
                return this.m_f4040a9Field;
            }
            set
            {
                this.m_f4040a9Field = value;
            }
        }

        /// <remarks/>
        public object f4040aa
        {
            get
            {
                return this.m_f4040aaField;
            }
            set
            {
                this.m_f4040aaField = value;
            }
        }

        /// <remarks/>
        public object f4040ab
        {
            get
            {
                return this.m_f4040abField;
            }
            set
            {
                this.m_f4040abField = value;
            }
        }

        /// <remarks/>
        public object f4040ac
        {
            get
            {
                return this.m_f4040acField;
            }
            set
            {
                this.m_f4040acField = value;
            }
        }

        /// <remarks/>
        public object f4040ad
        {
            get
            {
                return this.m_f4040adField;
            }
            set
            {
                this.m_f4040adField = value;
            }
        }

        /// <remarks/>
        public object f4040ae
        {
            get
            {
                return this.m_f4040aeField;
            }
            set
            {
                this.m_f4040aeField = value;
            }
        }

        /// <remarks/>
        public object f4040af
        {
            get
            {
                return this.m_f4040afField;
            }
            set
            {
                this.m_f4040afField = value;
            }
        }

        /// <remarks/>
        public object f4040b0
        {
            get
            {
                return this.m_f4040b0Field;
            }
            set
            {
                this.m_f4040b0Field = value;
            }
        }

        /// <remarks/>
        public object f4040b1
        {
            get
            {
                return this.m_f4040b1Field;
            }
            set
            {
                this.m_f4040b1Field = value;
            }
        }

        /// <remarks/>
        public object f4040b2
        {
            get
            {
                return this.m_f4040b2Field;
            }
            set
            {
                this.m_f4040b2Field = value;
            }
        }

        /// <remarks/>
        public object f4040b3
        {
            get
            {
                return this.m_f4040b3Field;
            }
            set
            {
                this.m_f4040b3Field = value;
            }
        }

        /// <remarks/>
        public object f4040b4
        {
            get
            {
                return this.m_f4040b4Field;
            }
            set
            {
                this.m_f4040b4Field = value;
            }
        }

        /// <remarks/>
        public object f4040b5
        {
            get
            {
                return this.m_f4040b5Field;
            }
            set
            {
                this.m_f4040b5Field = value;
            }
        }

        /// <remarks/>
        public object f4040b6
        {
            get
            {
                return this.m_f4040b6Field;
            }
            set
            {
                this.m_f4040b6Field = value;
            }
        }

        /// <remarks/>
        public object f4040b7
        {
            get
            {
                return this.m_f4040b7Field;
            }
            set
            {
                this.m_f4040b7Field = value;
            }
        }

        /// <remarks/>
        public object f4040b8
        {
            get
            {
                return this.m_f4040b8Field;
            }
            set
            {
                this.m_f4040b8Field = value;
            }
        }

        /// <remarks/>
        public object f4040b9
        {
            get
            {
                return this.m_f4040b9Field;
            }
            set
            {
                this.m_f4040b9Field = value;
            }
        }

        /// <remarks/>
        public object f4040ba
        {
            get
            {
                return this.m_f4040baField;
            }
            set
            {
                this.m_f4040baField = value;
            }
        }

        /// <remarks/>
        public object f4040bb
        {
            get
            {
                return this.m_f4040bbField;
            }
            set
            {
                this.m_f4040bbField = value;
            }
        }

        /// <remarks/>
        public object f4040bc
        {
            get
            {
                return this.m_f4040bcField;
            }
            set
            {
                this.m_f4040bcField = value;
            }
        }

        /// <remarks/>
        public object f4040bd
        {
            get
            {
                return this.m_f4040bdField;
            }
            set
            {
                this.m_f4040bdField = value;
            }
        }

        /// <remarks/>
        public object f4040be
        {
            get
            {
                return this.m_f4040beField;
            }
            set
            {
                this.m_f4040beField = value;
            }
        }

        /// <remarks/>
        public object f4040bf
        {
            get
            {
                return this.m_f4040bfField;
            }
            set
            {
                this.m_f4040bfField = value;
            }
        }

        /// <remarks/>
        public object f4040c0
        {
            get
            {
                return this.m_f4040c0Field;
            }
            set
            {
                this.m_f4040c0Field = value;
            }
        }

        /// <remarks/>
        public object f4040c1
        {
            get
            {
                return this.m_f4040c1Field;
            }
            set
            {
                this.m_f4040c1Field = value;
            }
        }

        /// <remarks/>
        public object f4040c2
        {
            get
            {
                return this.m_f4040c2Field;
            }
            set
            {
                this.m_f4040c2Field = value;
            }
        }

        /// <remarks/>
        public object f4040c3
        {
            get
            {
                return this.m_f4040c3Field;
            }
            set
            {
                this.m_f4040c3Field = value;
            }
        }

        /// <remarks/>
        public object f4040c4
        {
            get
            {
                return this.m_f4040c4Field;
            }
            set
            {
                this.m_f4040c4Field = value;
            }
        }

        /// <remarks/>
        public object f4040c5
        {
            get
            {
                return this.m_f4040c5Field;
            }
            set
            {
                this.m_f4040c5Field = value;
            }
        }

        /// <remarks/>
        public object f4040c6
        {
            get
            {
                return this.m_f4040c6Field;
            }
            set
            {
                this.m_f4040c6Field = value;
            }
        }

        /// <remarks/>
        public object f4040c7
        {
            get
            {
                return this.m_f4040c7Field;
            }
            set
            {
                this.m_f4040c7Field = value;
            }
        }

        /// <remarks/>
        public object f4040c8
        {
            get
            {
                return this.m_f4040c8Field;
            }
            set
            {
                this.m_f4040c8Field = value;
            }
        }

        /// <summary>
        /// Gets or sets the time phased data associated with the assignment
        /// </summary>
        [XmlElementAttribute("TimephasedData")]
        public List<TimephasedDataType> TimephasedData
        {
            get
            {
                return this.timephasedData;
            }
            set
            {
                this.timephasedData = value;
            }
        }
        #endregion

    }   
}

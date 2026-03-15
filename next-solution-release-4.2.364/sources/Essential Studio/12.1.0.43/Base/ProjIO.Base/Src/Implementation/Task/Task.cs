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
    /// Represents a Task in a Project
    /// </summary>
    [System.Serializable()]
    public class Task
    {   
        #region Fields
        private int m_uID;
        private int m_id;
        private string m_name;        
        private TaskType m_type;
        private System.DateTime m_createDate;
        private string m_contact;
        private string m_wBS;
        private string m_wBSLevel;
        private string m_outlineNumber;
        private int m_outlineLevel;
        private int m_priority;
        private System.DateTime m_start;
        private System.DateTime m_finish;
        private System.TimeSpan m_duration;
        private DurationFormat m_durationFormat;
        private bool m_bdurationFormatSpecified;
        private System.TimeSpan m_work;
        private System.DateTime m_stop;
        private bool m_bstopSpecified;
        private System.DateTime m_resume;
        private bool m_bresumeSpecified;
        private bool m_bresumeValid;
        private bool m_beffortDriven;
        private bool m_brecurring;
        private bool m_boverAllocated;
        private bool m_bestimated;
        private bool m_bmilestone;
        private bool m_bsummary;
        private bool m_bdisplayAsSummary;
        private bool m_bcritical;
        private bool m_bisSubproject;
        private bool m_bisSubprojectReadOnly;
        private string m_subprojectName;
        private bool m_bexternalTask;
        private string m_externalTaskProject;
        private System.DateTime m_earlyStart;
        private System.DateTime m_earlyFinish;
        private System.DateTime m_lateStart;
        private System.DateTime m_lateFinish;
        private int m_startVariance;
        private int m_finishVariance;
        private float m_workVariance;
        private int m_freeSlack;
        private int m_startSlack;
        private int m_finishSlack;
        private int m_totalSlack;
        private float m_fixedCost;
        private TaskFixedCostAccrual m_fixedCostAccrual;
        private bool m_bfixedCostAccrualSpecified;
        private int m_percentComplete;
        private int m_percentWorkComplete;
        private decimal m_cost;
        private decimal m_overtimeCost;
        private System.TimeSpan m_overtimeWork;
        private System.DateTime m_actualStart;
        private bool m_bactualStartSpecified;
        private System.DateTime m_actualFinish;
        private bool m_bactualFinishSpecified;
        private System.TimeSpan m_actualDuration;
        private decimal m_actualCost;
        private bool m_bactualCostSpecified;
        private decimal m_actualOvertimeCost;
        private bool m_bactualOvertimeCostSpecified;
        private TimeSpan m_actualWork;
        private TimeSpan m_actualOvertimeWork;
        private TimeSpan m_regularWork;
        private TimeSpan m_remainingDuration;
        private decimal m_remainingCost;
        private TimeSpan m_remainingWork;
        private decimal m_remainingOvertimeCost;
        private TimeSpan m_remainingOvertimeWork;
        private float m_aCWP;
        private float m_cv;
        private TaskConstraintType m_constraintType;
        private int m_calendarUID;
        private System.DateTime m_constraintDate;
        private bool m_bconstraintDateSpecified;
        private System.DateTime m_deadline;
        private bool m_bdeadlineSpecified;
        private bool m_blevelAssignments;
        private bool m_blevelingCanSplit;
        private int m_levelingDelay;
        private DelayFormat m_levelingDelayFormat;
        private bool m_blevelingDelayFormatSpecified;
        private System.DateTime m_preLeveledStart;
        private bool m_bpreLeveledStartSpecified;
        private System.DateTime m_preLeveledFinish;
        private bool m_bpreLeveledFinishSpecified;
        private string m_hyperlink;
        private string m_hyperlinkAddress;
        private string m_hyperlinkSubAddress;
        private bool m_bignoreResourceCalendar;
        private string m_notes;
        private bool m_bhideBar;
        private bool m_brollup;
        private float m_bCWS;
        private float m_bCWP;
        private int m_physicalPercentComplete;
        private EarnedValueMethod m_earnedValueMethod;
        private List<TaskLink> m_predecessorLink = new List<TaskLink>();
        private TimeSpan m_actualWorkProtected;
        private TimeSpan m_actualOvertimeWorkProtected;
        private ExtendedAttributesBase[] m_extendedAttribute;
        private TaskBaseline[] m_baseline;
        private OutlineCodeBase[] m_outlineCode;
        private bool m_bisPublished;
        private string m_statusManager;
        private System.DateTime m_commitmentStart;
        private bool m_bcommitmentStartSpecified;
        private System.DateTime m_commitmentFinish;
        private bool m_bcommitmentFinishSpecified;
        private int m_commitmentType;
        private bool m_bactive;
        private bool m_bpinned;
        private string m_pinnedStart;
        private string m_pinnedFinish;
        private string m_pinnedDuration;
        private TimephasedDataType[] m_timephasedData;
        private List<Task> m_children = new List<Task>();
        private Task m_parent;
        private bool m_bIsNull;
        #endregion

        #region Initializer/Constructor
        public Task()
        {
            
        }
        public Task(string name)
        {
            this.Name = name;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the unique ID of the task
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
        /// Gets or sets the position identifier of the task within the list of tasks
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
        /// Gets or sets the name of the task
        /// </summary>
        [System.Xml.Serialization.XmlElement("Name", DataType = "string")]
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
        /// Gets or sets the type of task
        /// </summary>
        [System.Xml.Serialization.XmlElement("Type")]
        public TaskType Type
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
        /// Specifies whether the task is null
        /// </summary>
        
        [System.Xml.Serialization.XmlIgnore()]
        public bool IsNull
        {
            get
            {
                return m_bIsNull;
            }
            set
            {
                m_bIsNull = value;
            }
        }

        [System.Xml.Serialization.XmlElement("IsNull")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsNullString
        {
            get
            {
                return IsNull ? "1" : "0";
            }
            set
            {
                bool ParsedValue;

                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = System.Xml.XmlConvert.ToBoolean(value);

                IsNull = ParsedValue;
            }
        }
        
        /// <summary>
        /// Gets or sets the date that the task was created
        /// </summary>
        [System.Xml.Serialization.XmlElement("CreateDate", DataType = "dateTime")]
        public System.DateTime CreateDate
        {
            get
            {
                return this.m_createDate;
            }
            set
            {
                this.m_createDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the contact person for the task
        /// </summary>
        [System.Xml.Serialization.XmlElement("Contact", DataType = "string")]
        public string Contact
        {
            get
            {
                return this.m_contact;
            }
            set
            {
                this.m_contact = value;
            }
        }

        /// <summary>
        /// Gets or sets the work breakdown structure (WBS) code of the task
        /// </summary>
        [System.Xml.Serialization.XmlElement("WBS", DataType = "string")]
        public string WBS
        {
            get
            {
                return this.m_wBS;
            }
            set
            {
                this.m_wBS = value;
            }
        }

        /// <summary>
        /// Gets or sets the right-most WBS level of the task
        /// </summary>
        [System.Xml.Serialization.XmlElement("WBSLevel", DataType = "string")]
        public string WBSLevel
        {
            get
            {
                return this.m_wBSLevel;
            }
            set
            {
                this.m_wBSLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets the outline number of the task
        /// </summary>
        [System.Xml.Serialization.XmlElement("OutlineNumber", DataType = "string")]
        public string OutlineNumber
        {
            get
            {
                return this.m_outlineNumber;
            }
            set
            {
                this.m_outlineNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the outline level of the task
        /// </summary>
        [XmlIgnore]
        public int OutlineLevel
        {
            get
            {
                return this.m_outlineLevel;
            }
            set
            {
                this.m_outlineLevel = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("OutlineLevel")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string OutlineLevelSerialized
        {
            get
            {
                return OutlineLevel.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_outlineLevel);
            }
        }
        /// <summary>
        /// Gets or sets the priority of the task from 0 to 1000
        /// </summary>
        [XmlIgnore]
        public int Priority
        {
            get
            {
                return this.m_priority;
            }
            set
            {
                this.m_priority = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("Priority")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string PrioritySerialized
        {
            get
            {
                return Priority.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_priority);
            }
        }
        /// <summary>
        /// Gets or sets the scheduled start date of the task
        /// </summary>
        //[System.Xml.Serialization.XmlElement("Start", DataType = "dateTime")]
        [XmlIgnore()]
        public System.DateTime Start
        {
            get
            {
                return this.m_start;
            }
            set
            {
                if (this.m_bmilestone == true)
                    this.m_start = new DateTime(value.Year, value.Month, value.Day, 17, 0, 0);
                else
                    this.m_start = new DateTime(value.Year, value.Month, value.Day, 8, 0, 0);
            }
        }

        [XmlElement("Start")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string StartString
        {
            get
            {
                return this.m_start.ToString("yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                this.m_start = DateTime.ParseExact(value, "yyyy-MM-ddTHH:mm:ss", null);
            }
        }

        /// <summary>
        /// Gets or sets the scheduled finish date of the task
        /// </summary>
        //[System.Xml.Serialization.XmlElement("Finish", DataType = "dateTime")]
        [XmlIgnore()]
        public System.DateTime Finish
        {
            get
            {
                return this.m_finish;
            }
            set
            {
                this.m_finish = new DateTime(value.Year, value.Month, value.Day, 17, 0, 0);
                int days = GetNoOfDays(this.m_start, this.m_finish);
                this.m_duration = new TimeSpan(days * 8, 0, 0);
            }
        }

        [XmlElement("Finish")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string FinishString
        {
            get
            {
                return this.m_finish.ToString("yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                this.m_finish = DateTime.ParseExact(value, "yyyy-MM-ddTHH:mm:ss", null);
            }
        }

        /// <summary>
        /// Gets or sets the planned duration of the task
        /// </summary>
        [XmlIgnore()]
        public System.TimeSpan Duration
        {
            get
            {
                return this.m_duration;
            }
            set
            {
                this.m_duration = value;
                if (this.m_duration.Equals(new TimeSpan()))
                {
                    this.m_finish = this.m_start;
                }
                else
                {
                    int hr = (int)this.m_duration.TotalHours / 8;
                    this.m_finish = AddDays(this.m_start, hr);
                }
            }
        }
        
        [System.Xml.Serialization.XmlElement("Duration",DataType = "duration")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DurationString
        {
            get
            {
                return "PT" + this.m_duration.TotalHours + "H" + this.m_duration.Minutes + "M" + this.m_duration.Seconds + "S";
            }
            set
            {
                this.m_duration = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the format for expressing the Duration of the Task
        /// </summary>
        [System.Xml.Serialization.XmlElement("DurationFormat")]
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
        /// Checks whether Duration Format is specified
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

        /// <summary>
        /// Gets or sets the amount of scheduled work for the task
        /// </summary>
        [XmlIgnore()]
        public System.TimeSpan Work
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
        
        [System.Xml.Serialization.XmlElement("Work",DataType = "duration")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string WorkString
        {
            get
            {
                return "PT" + this.m_work.TotalHours + "H" + this.m_work.Minutes + "M" + this.m_work.Seconds + "S";
            }
            set
            {
                this.m_work = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the date that the task was stopped
        /// </summary>
        [System.Xml.Serialization.XmlElement("Stop", DataType = "dateTime")]
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
        /// Checks whether the date that the task was stopped is specified
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
        /// Gets or sets the date that the task resumed
        /// </summary>
        [System.Xml.Serialization.XmlElement("Resume", DataType = "dateTime")]
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
        /// Checks whether the date that the task resumed is specified
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
        /// Checks whether the task can be resumed
        /// </summary>
        [XmlIgnore()]
        public bool IsResumeValid
        {
            get
            {
                return this.m_bresumeValid;
            }
            set
            {
                this.m_bresumeValid = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("ResumeValid")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsResumeValidString
        {
            get
            {
                return this.m_bresumeValid ? "1" : "0";
            }
            set
            {
                this.m_bresumeValid = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Checks whether the task is effort-driven
        /// </summary>
        [XmlIgnore()]
        public bool IsEffortDriven
        {
            get
            {
                return this.m_beffortDriven;
            }
            set
            {
                this.m_beffortDriven = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("EffortDriven")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsEffortDrivenString
        {
            get
            {
                return this.m_beffortDriven ? "1" : "0";
            }
            set
            {
                this.m_beffortDriven = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Checks whether the task is a recurring task
        /// </summary>
        [XmlIgnore()]
        public bool IsRecurring
        {
            get
            {
                return this.m_brecurring;
            }
            set
            {
                this.m_brecurring = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("Recurring")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsRecurringString
        {
            get
            {
                return this.m_brecurring ? "1" : "0";
            }
            set
            {
                this.m_brecurring = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Checks whether the task is overallocated
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
        /// Checks whether the task is estimated
        /// </summary>
        [XmlIgnore()]
        public bool IsEstimated
        {
            get
            {
                return this.m_bestimated;
            }
            set
            {
                this.m_bestimated = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("Estimated")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsEstimatedString
        {
            get
            {
                return this.m_bestimated ? "1" : "0";
            }
            set
            {
                this.m_bestimated = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Checks whether the task is a milestone
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
        
        [System.Xml.Serialization.XmlElement("Milestone")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsMilestoneString
        {
            get
            {
                return this.m_bmilestone ? "1" : "0";
            }
            set
            {
                this.m_bmilestone = XmlConvert.ToBoolean(value);
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
        
        [System.Xml.Serialization.XmlElement("Summary")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsSummaryString
        {
            get
            {
                return this.m_bsummary ? "1" : "0";
            }
            set
            {
                this.m_bsummary = XmlConvert.ToBoolean(value);
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool DisplayAsSummary
        {
            get
            {
                return this.m_bdisplayAsSummary;
            }
            set
            {
                this.m_bdisplayAsSummary = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("DisplayAsSummary")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DisplayAsSummaryString
        {
            get
            {
                return this.m_bdisplayAsSummary ? "1" : "0";
            }
            set
            {
                this.m_bdisplayAsSummary = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Checks whether the task is in the critical chain
        /// </summary>
        [XmlIgnore()]
        public bool IsCritical
        {
            get
            {
                return this.m_bcritical;
            }
            set
            {
                this.m_bcritical = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("Critical")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsCriticalString
        {
            get
            {
                return this.m_bcritical ? "1" : "0";
            }
            set
            {
                this.m_bcritical = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Checks whether the task is an inserted project
        /// </summary>
        [XmlIgnore()]
        public bool IsSubproject
        {
            get
            {
                return this.m_bisSubproject;
            }
            set
            {
                this.m_bisSubproject = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("IsSubproject")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsSubprojectString
        {
            get
            {
                return this.m_bisSubproject ? "1" : "0";
            }
            set
            {
                this.m_bisSubproject = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Checks whether the inserted project is read-only
        /// </summary>
        [XmlIgnore()]
        public bool IsSubprojectReadOnly
        {
            get
            {
                return this.m_bisSubprojectReadOnly;
            }
            set
            {
                this.m_bisSubprojectReadOnly = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("IsSubprojectReadOnly")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsSubprojectReadOnlyString
        {
            get
            {
                return this.m_bisSubprojectReadOnly ? "1" : "0";
            }
            set
            {
                this.m_bisSubprojectReadOnly = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets the source location of the inserted project
        /// </summary>
        [System.Xml.Serialization.XmlElement("SubProjectName",DataType="string")]
        public string SubprojectName
        {
            get
            {
                return this.m_subprojectName;
            }
            set
            {
                this.m_subprojectName = value;
            }
        }

        /// <summary>
        /// Checks whether the task is external
        /// </summary>
        [XmlIgnore()]
        public bool IsExternalTask
        {
            get
            {
                return this.m_bexternalTask;
            }
            set
            {
                this.m_bexternalTask = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("ExternalTask")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsExternalTaskString
        {
            get
            {
                return this.m_bexternalTask ? "1" : "0";
            }
            set
            {
                this.m_bexternalTask = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets the source location and task identifier of the external task
        /// </summary>
        [System.Xml.Serialization.XmlElement("ExternalTaskProject", DataType = "string")]
        public string ExternalTaskProject
        {
            get
            {
                return this.m_externalTaskProject;
            }
            set
            {
                this.m_externalTaskProject = value;
            }
        }

        /// <summary>
        /// Gets or sets the early start date of the task
        /// </summary>
        [System.Xml.Serialization.XmlElement("EarlyStart", DataType = "dateTime")]
        public System.DateTime EarlyStart
        {
            get
            {
                return this.m_earlyStart;
            }
            set
            {
                this.m_earlyStart = value;
            }
        }

        /// <summary>
        /// Checks whether the early start date of the task is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool EarlyStartSpecified
        {
            get
            {
                return this.m_bearlyStartSpecified;
            }
            set
            {
                this.m_bearlyStartSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the early finish date of the task
        /// </summary>
        [System.Xml.Serialization.XmlElement("EarlyFinish", DataType = "dateTime")]
        public System.DateTime EarlyFinish
        {
            get
            {
                return this.m_earlyFinish;
            }
            set
            {
                this.m_earlyFinish = value;
            }
        }

        /// <summary>
        /// Checks whether the early finish date of the task
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool EarlyFinishSpecified
        {
            get
            {
                return this.m_bearlyFinishSpecified;
            }
            set
            {
                this.m_bearlyFinishSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the late start date of the task
        /// </summary>
        [System.Xml.Serialization.XmlElement("LateStart", DataType = "dateTime")]
        public System.DateTime LateStart
        {
            get
            {
                return this.m_lateStart;
            }
            set
            {
                this.m_lateStart = value;
            }
        }

        /// <summary>
        /// Checks whether the late start date of the task is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LateStartSpecified
        {
            get
            {
                return this.m_blateStartSpecified;
            }
            set
            {
                this.m_blateStartSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the late finish date of the task
        /// </summary>
        [System.Xml.Serialization.XmlElement("LateFinish", DataType = "dateTime")]
        public System.DateTime LateFinish
        {
            get
            {
                return this.m_lateFinish;
            }
            set
            {
                this.m_lateFinish = value;
            }
        }

        /// <summary>
        /// Checks whether the late finish date of the task is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LateFinishSpecified
        {
            get
            {
                return this.m_blateFinishSpecified;
            }
            set
            {
                this.m_blateFinishSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the variance of the task start date from the baseline start date as minutes x 1000
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
        /// Gets or sets the variance of the task finish date from the baseline finish date as minutes x 1000
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
        /// Gets or sets the variance of task work from the baseline task work as minutes x 1000
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
        /// Checks whether the Work variance is specified
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
        /// Gets or sets the amount of free slack
        /// </summary>
        [XmlIgnore]
        public int FreeSlack
        {
            get
            {
                return this.m_freeSlack;
            }
            set
            {
                this.m_freeSlack = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("FreeSlack")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string FreeSlackSerialized
        {
            get
            {
                return FreeSlack.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_freeSlack);
            }
        }
        /// <summary>
        /// Gets or sets the amount of free slack at the start of the task
        /// </summary>
        [XmlIgnore]
        public int StartSlack
        {
            get
            {
                return this.m_startSlack;
            }
            set
            {
                this.m_startSlack = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("StartSlack")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string StartSlackSerialized
        {
            get
            {
                return StartSlack.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_startSlack);
            }
        }
        /// <summary>
        /// Gets or sets the amount of free slack at the end of the task
        /// </summary>
        [XmlIgnore]
        public int FinishSlack
        {
            get
            {
                return this.m_finishSlack;
            }
            set
            {
                this.m_finishSlack = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("FinishSlack")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string FinishSlackSerialized
        {
            get
            {
                return FinishSlack.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_finishSlack);
            }
        }
        /// <summary>
        /// Gets or sets the amount of total slack
        /// </summary>
        [XmlIgnore]
        public int TotalSlack
        {
            get
            {
                return this.m_totalSlack;
            }
            set
            {
                this.m_totalSlack = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("TotalSlack")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string TotalSlackSerialized
        {
            get
            {
                return TotalSlack.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_totalSlack);
            }
        }
        /// <summary>
        /// Gets or sets the fixed cost of the task
        /// </summary>
        [XmlIgnore]
        public float FixedCost
        {
            get
            {
                return this.m_fixedCost;
            }
            set
            {
                this.m_fixedCost = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("FixedCost")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string FixedCostSerialized
        {
            get
            {
                return FixedCost.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_fixedCost);
            }
        }
        /// <summary>
        /// Checks whether fixed cost of the task is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool FixedCostSpecified
        {
            get
            {
                return this.m_bfixedCostSpecified;
            }
            set
            {
                this.m_bfixedCostSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets how the fixed cost is accrued against the task
        /// </summary>
        [System.Xml.Serialization.XmlElement("FixedCostAccrual")]
        public TaskFixedCostAccrual FixedCostAccrual
        {
            get
            {
                return this.m_fixedCostAccrual;
            }
            set
            {
                this.m_fixedCostAccrual = value;
            }
        }

        /// <summary>
        /// Checks whether Fixed Cost Accrual is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool FixedCostAccrualSpecified
        {
            get
            {
                return this.m_bfixedCostAccrualSpecified;
            }
            set
            {
                this.m_bfixedCostAccrualSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the percentage of the task duration completed
        /// </summary>
        [XmlIgnore]
        public int PercentComplete
        {
            get
            {
                return this.m_percentComplete;
            }
            set
            {
                this.m_percentComplete = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("PercentComplete")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string PercentCompleteSerialized
        {
            get
            {
                return PercentComplete.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_percentComplete);
            }
        }

        /// <summary>
        /// Gets or sets the percentage of the task work completed
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
        /// Gets or sets the projected or scheduled cost of the task
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
        /// Checks whether the projected or scheduled cost of the task is specified
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
        /// Gets or sets the sum of the actual and remaining overtime cost of the task
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
        /// Checks whether the sum of the actual and remaining overtime cost of the task is specified
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
        /// Gets or sets the amount of overtime work scheduled for the task
        /// </summary>
        [XmlIgnore()]
        public System.TimeSpan OvertimeWork
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
        
        [System.Xml.Serialization.XmlElement("OvertimeWork",DataType = "duration")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string OvertimeWorkString
        {
            get
            {
                return "PT" + this.m_overtimeWork.TotalHours + "H" + this.m_overtimeWork.Minutes + "M" + this.m_overtimeWork.Seconds + "S";
            }
            set
            {
                this.m_overtimeWork = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the actual start date of the task
        /// </summary>
        [System.Xml.Serialization.XmlElement("ActualStart",DataType="dateTime")]
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
        /// Checks whether the actual start date of the task is specified
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
        /// Gets or sets the actual finish date of the task
        /// </summary>
        [System.Xml.Serialization.XmlElement("ActualFinish", DataType = "dateTime")]
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
        /// Checks whether the actual finish date of the task is specified
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
        /// Gets or sets the actual duration of the task
        /// </summary>
        [XmlIgnore()]
        public System.TimeSpan ActualDuration
        {
            get
            {
                return this.m_actualDuration;
            }
            set
            {
                this.m_actualDuration = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("ActualDuration",DataType = "duration")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ActualDurationString
        {
            get
            {
                return "PT" + this.m_actualDuration.TotalHours + "H" + this.m_actualDuration.Minutes + "M" + this.m_actualDuration.Seconds + "S";
            }
            set
            {
                this.m_actualDuration = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the actual cost of the task
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
        /// Checks whether the actual cost of the task is specified
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
        /// Gets or sets the actual overtime cost of the task
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
        /// Checks whether the actual overtime cost of the task is specified
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
        /// Gets or sets the actual work for the task
        /// </summary>
        [XmlIgnore()]
        public TimeSpan ActualWork
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
        
        [System.Xml.Serialization.XmlElement("ActualWork",DataType = "duration")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ActualWorkString
        {
            get
            {
                return "PT" + this.m_actualWork.TotalHours + "H" + this.m_actualWork.Minutes + "M" + this.m_actualWork.Seconds + "S";
            }
            set
            {
                this.m_actualWork = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the actual overtime work for the task
        /// </summary>
        [XmlIgnore()]
        public TimeSpan ActualOvertimeWork
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
        
        [System.Xml.Serialization.XmlElement("ActualOvertimeWork",DataType = "duration")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ActualOvertimeWorkString
        {
            get
            {
                return "PT" + this.m_actualOvertimeWork.TotalHours + "H" + this.m_actualOvertimeWork.Minutes + "M" + this.m_actualOvertimeWork.Seconds + "S";
            }
            set
            {
                this.m_actualOvertimeWork = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the amount of non-overtime work scheduled for the task
        /// </summary>
        [XmlIgnore()]
        public TimeSpan RegularWork
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
        
        [System.Xml.Serialization.XmlElement("RegularWork",DataType = "duration")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string RegularWorkString
        {
            get
            {
                return "PT" + this.m_regularWork.TotalHours + "H" + this.m_regularWork.Minutes + "M" + this.m_regularWork.Seconds + "S";
            }
            set
            {
                this.m_regularWork = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the amount of time required to complete the unfinished portion of the task
        /// </summary>
        [XmlIgnore()]
        public TimeSpan RemainingDuration
        {
            get
            {
                return this.m_remainingDuration;
            }
            set
            {
                this.m_remainingDuration = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("RemainingDuration",DataType = "duration")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string RemainingDurationString
        {
            get
            {
                return "PT" + this.m_remainingDuration.TotalHours + "H" + this.m_remainingDuration.Minutes + "M" + this.m_remainingDuration.Seconds + "S";
            }
            set
            {
                this.m_remainingDuration = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the remaining projected cost of completing the task
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
        /// Checks whether the remaining projected cost of completing the task is specified
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
        /// Gets or sets the remaining work scheduled to complete the task
        /// </summary>
        [XmlIgnore()]
        public TimeSpan RemainingWork
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
        
        [System.Xml.Serialization.XmlElement("RemainingWork",DataType = "duration")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string RemainingWorkString
        {
            get
            {
                return "PT" + this.m_remainingWork.TotalHours + "H" + this.m_remainingWork.Minutes + "M" + this.m_remainingWork.Seconds + "S";
            }
            set
            {
                this.m_remainingWork = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the remaining overtime cost projected to finish the task
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
        /// Gets or sets the remaining overtime work scheduled to finish the task
        /// </summary>
        [XmlIgnore()]
        public TimeSpan RemainingOvertimeWork
        {
            get
            {
                return this.m_remainingOvertimeWork;
            }
            set
            {
                this.m_remainingOvertimeWork=value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("RemainingOvertimeWork",DataType = "duration")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string RemainingOvertimeWorkString
        {
            get
            {
                return "PT" + this.m_remainingOvertimeWork.TotalHours + "H" + this.m_remainingOvertimeWork.Minutes + "M" + this.m_remainingOvertimeWork.Seconds + "S";
            }
            set
            {
                this.m_remainingOvertimeWork = XmlConvert.ToTimeSpan(value);
            }
        }


        /// <summary>
        /// Gets or sets the actual cost of work performed on the task to date
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
        /// Gets or sets Earned value cost variance
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
        /// Gets or sets the constraint on the start or finish date of the task
        /// </summary>
        [System.Xml.Serialization.XmlElement("ConstraintType")]
        public TaskConstraintType ConstraintType
        {
            get
            {
                return this.m_constraintType;
            }
            set
            {
                this.m_constraintType = value;
            }
        }        

        /// <summary>
        /// Gets or sets the task calendar
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
        /// Gets or sets the date argument for the task constraint type
        /// </summary>
        [System.Xml.Serialization.XmlElement("ConstraintDate", DataType = "dateTime")]
        public System.DateTime ConstraintDate
        {
            get
            {
                return this.m_constraintDate;
            }
            set
            {
                this.m_constraintDate = value;
            }
        }

       
        /// <summary>
        /// Gets or sets the deadline for the task to be completed
        /// </summary>
        [System.Xml.Serialization.XmlElement("Deadline", DataType = "dateTime")]
        public System.DateTime Deadline
        {
            get
            {
                return this.m_deadline;
            }
            set
            {
                this.m_deadline = value;
            }
        }

        /// <summary>
        /// Checks whether the deadline for the task to be completed is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DeadlineSpecified
        {
            get
            {
                return this.m_bdeadlineSpecified;
            }
            set
            {
                this.m_bdeadlineSpecified = value;
            }
        }

        /// <summary>
        /// Checks whether leveling can adjust assignments
        /// </summary>
        [XmlIgnore()]
        public bool LevelAssignments
        {
            get
            {
                return this.m_blevelAssignments;
            }
            set
            {
                this.m_blevelAssignments = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("LevelAssignments")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string LevelAssignmentsString
        {
            get
            {
                return this.m_blevelAssignments ? "1" : "0";
            }
            set
            {
                this.m_blevelAssignments = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Checks whether leveling can split the task
        /// </summary>
        [XmlIgnore()]
        public bool LevelingCanSplit
        {
            get
            {
                return this.m_blevelingCanSplit;
            }
            set
            {
                this.m_blevelingCanSplit = value;
            }
        }

        [System.Xml.Serialization.XmlElement("LevelingCanSplit")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string LevelingCanSplitString
        {
            get
            {
                return this.m_blevelingCanSplit ? "1" : "0";
            }
            set
            {
                this.m_blevelingCanSplit = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets the delay caused by leveling the task
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
        [System.Xml.Serialization.XmlElement("LevelingDelayFormat")]
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
        /// Gets or sets the start date of the task before it was leveled
        /// </summary>
        [System.Xml.Serialization.XmlElement("PreLeveledStart", DataType = "dateTime")]
        public System.DateTime PreLeveledStart
        {
            get
            {
                return this.m_preLeveledStart;
            }
            set
            {
                this.m_preLeveledStart = value;
            }
        }

        /// <summary>
        /// Checks whether the start date of the task before it was leveled is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PreLeveledStartSpecified
        {
            get
            {
                return this.m_bpreLeveledStartSpecified;
            }
            set
            {
                this.m_bpreLeveledStartSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the finish date of the task before it was leveled
        /// </summary>
        [System.Xml.Serialization.XmlElement("PreLeveledFinish", DataType = "dateTime")]
        public System.DateTime PreLeveledFinish
        {
            get
            {
                return this.m_preLeveledFinish;
            }
            set
            {
                this.m_preLeveledFinish = value;
            }
        }

        /// <summary>
        /// Checks whether the finish date of the task before it was leveled is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PreLeveledFinishSpecified
        {
            get
            {
                return this.m_bpreLeveledFinishSpecified;
            }
            set
            {
                this.m_bpreLeveledFinishSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the title of the hyperlink associated with the task
        /// </summary>
        [System.Xml.Serialization.XmlElement("Hyperlink", DataType = "string")]
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
        /// Gets or sets the hyperlink associated with the task
        /// </summary>
        [System.Xml.Serialization.XmlElement("HyperlinkAddress", DataType = "string")]
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
        /// Gets or sets the document bookmark of the hyperlink associated with the task
        /// </summary>
        [System.Xml.Serialization.XmlElement("HyperlinkSubAddress", DataType = "string")]
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
        /// Checks whether the task ignores the resource calendar
        /// </summary>
        [XmlIgnore()]
        public bool IgnoreResourceCalendar
        {
            get
            {
                return this.m_bignoreResourceCalendar;
            }
            set
            {
                this.m_bignoreResourceCalendar = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("IgnoreResourceCalendar")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IgnoreResourceCalendarString
        {
            get
            {
                return this.m_bignoreResourceCalendar ? "1" : "0";
            }
            set
            {
                this.m_bignoreResourceCalendar = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets Text notes associated with the task
        /// </summary>
        [System.Xml.Serialization.XmlElement("Notes", DataType = "string")]
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
        /// Checks whether the GANTT bar of the task is hidden when displayed in Microsoft Office Project
        /// </summary>
        [XmlIgnore()]
        public bool HideBar
        {
            get
            {
                return this.m_bhideBar;
            }
            set
            {
                this.m_bhideBar = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("HideBar")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string HideBarString
        {
            get
            {
                return this.m_bhideBar ? "1" : "0";
            }
            set
            {
                this.m_bhideBar = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Checks whether the task is rolled up
        /// </summary>
        [XmlIgnore()]
        public bool IsRollup
        {
            get
            {
                return this.m_brollup;
            }
            set
            {
                this.m_brollup = value;
            }
        }
        
        /// <summary>
        /// IsRollup value as string
        /// </summary>
        [System.Xml.Serialization.XmlElement("Rollup")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsRollupString
        {
            get
            {
                return this.m_brollup ? "1" : "0";
            }
            set
            {
                this.m_brollup = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets he budgeted cost of work scheduled for the task
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
        /// Gets or sets the budgeted cost of work performed on the task to date
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
        /// Gets or sets the percentage complete value entered by the Project Manager
        /// </summary>
        [XmlIgnore]
        public int PhysicalPercentComplete
        {
            get
            {
                return this.m_physicalPercentComplete;
            }
            set
            {
                this.m_physicalPercentComplete = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("PhysicalPercentComplete")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string PhysicalPercentCompleteSerialized
        {
            get
            {
                return PhysicalPercentComplete.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_physicalPercentComplete);
            }
        }
        /// <summary>
        /// Gets or sets the method for calculating earned value
        /// </summary>
        [System.Xml.Serialization.XmlElement("EarnedValueMethod")]
        public EarnedValueMethod EarnedValueMethod
        {
            get
            {
                return this.m_earnedValueMethod;
            }
            set
            {
                this.m_earnedValueMethod = value;
            }
        }

        /// <summary>
        /// Defines the predecessor task of the task that contains it
        /// </summary>
        [System.Xml.Serialization.XmlElement("PredecessorLink")]
        public List<TaskLink> PredecessorLink
        {
            get
            {
                return this.m_predecessorLink;
            }
            set
            {
                this.m_predecessorLink = value;
            }
        }

        /// <summary>
        /// Gets or sets the duration through which actual work is protected
        /// </summary>
        [XmlIgnore()]
        public TimeSpan ActualWorkProtected
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
        /// ActualWorkProtected value as string
        /// </summary>
        [System.Xml.Serialization.XmlElement("ActualWorkProtected",DataType = "duration")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ActualWorkProtectedString
        {
            get
            {
                return "PT" + this.m_actualWorkProtected.TotalHours + "H" + this.m_actualWorkProtected.Minutes + "M" + this.m_actualWorkProtected.Seconds + "S";
            }
            set
            {
                this.m_actualWorkProtected = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the duration through which actual overtime work is protected
        /// </summary>
        [XmlIgnore()]
        public TimeSpan ActualOvertimeWorkProtected
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
        /// ActualOvertimeWorkProtected string value
        /// </summary>
        [System.Xml.Serialization.XmlElement("ActualOvertimeWorkProtected", DataType = "duration")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ActualOvertimeWorkProtectedString
        {
            get
            {
                return "PT" + this.m_actualOvertimeWorkProtected.TotalHours + "H" + this.m_actualOvertimeWorkProtected.Minutes + "M" + this.m_actualOvertimeWorkProtected.Seconds + "S";
            }
            set
            {
                this.m_actualOvertimeWorkProtected = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// The value of an extended attribute
        /// </summary>
        [System.Xml.Serialization.XmlElement("ExtendedAttribute")]
        public ExtendedAttributesBase[] ExtendedAttribute
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
        /// The collection of baseline values of the task
        /// </summary>
        [System.Xml.Serialization.XmlElement("Baseline")]
        public TaskBaseline[] Baseline
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
        /// The value of an outline code
        /// </summary>
        [System.Xml.Serialization.XmlElement("OutlineCode")]
        public OutlineCodeBase[] OutlineCode
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
        /// Checks whether the task is published
        /// </summary>
        [XmlIgnore()]
        public bool IsPublished
        {
            get
            {
                return this.m_bisPublished;
            }
            set
            {
                this.m_bisPublished = value;
            }
        }
        
        /// <summary>
        /// IsPublished value in string
        /// </summary>
        [System.Xml.Serialization.XmlElement("IsPublished")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsPublishedString
        {
            get
            {
                return this.m_bisPublished ? "1" : "0";
            }
            set
            {
                this.m_bisPublished = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the task status manager
        /// </summary>
        [System.Xml.Serialization.XmlElement("StatusManager",DataType="string")]
        public string StatusManager
        {
            get
            {
                return this.m_statusManager;
            }
            set
            {
                this.m_statusManager = value;
            }
        }

        /// <summary>
        /// Gets or sets the start date of the deliverable
        /// </summary>
        [System.Xml.Serialization.XmlElement("CommitmentStart", DataType = "dateTime")]
        public System.DateTime CommitmentStart
        {
            get
            {
                return this.m_commitmentStart;
            }
            set
            {
                this.m_commitmentStart = value;
            }
        }

        /// <summary>
        /// Checks whether the start date of the deliverable is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CommitmentStartSpecified
        {
            get
            {
                return this.m_bcommitmentStartSpecified;
            }
            set
            {
                this.m_bcommitmentStartSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the finish date of the deliverable
        /// </summary>
        [System.Xml.Serialization.XmlElement("CommitmentFinish", DataType = "dateTime")]
        public System.DateTime CommitmentFinish
        {
            get
            {
                return this.m_commitmentFinish;
            }
            set
            {
                this.m_commitmentFinish = value;
            }
        }

        /// <summary>
        /// Checks whether the finish date of the deliverable is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CommitmentFinishSpecified
        {
            get
            {
                return this.m_bcommitmentFinishSpecified;
            }
            set
            {
                this.m_bcommitmentFinishSpecified = value;
            }
        }
        /// <summary>
        /// Specifies whether the task has an associated deliverable or a dependency on an associated deliverable
        /// </summary>
        [System.Xml.Serialization.XmlElement("CommitmentType")]
        public int CommitmentType
        {
            get
            {
                return this.m_commitmentType;
            }
            set
            {
                if (value >= 0 && value <=2)
                    this.m_commitmentType = value;
            }
        }
        /// <summary>
        /// Checks whether the task is active
        /// </summary>
        [XmlIgnore()]
        public bool IsActive
        {
            get
            {
                return this.m_bactive;
            }
            set
            {
                this.m_bactive = value;
            }
        }
        
        /// <summary>
        /// Active string value
        /// </summary>
        [System.Xml.Serialization.XmlElement("Active")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsActiveString
        {
            get
            {
                return this.m_bactive ? "1" : "0";
            }
            set
            {
                this.m_bactive = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Checks whether the task is in manually scheduled mode
        /// </summary>
        [XmlIgnore()]
        public bool IsManual
        {
            get
            {
                return this.m_bpinned;
            }
            set
            {
                this.m_bpinned = value;
            }
        }
        
        /// <summary>
        /// Manual task string
        /// </summary>
        [System.Xml.Serialization.XmlElement("Manual")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsPinnedString
        {
            get
            {
                return this.m_bpinned ? "1" : "0";
            }
            set
            {
                this.m_bpinned = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets Text displayed in start  when the task is in Manually Scheduled mode
        /// </summary>
        [System.Xml.Serialization.XmlElement("ManualStart", DataType = "string")]
        public string ManualStart
        {
            get
            {
                return this.m_pinnedStart;
            }
            set
            {
                this.m_pinnedStart = value;
            }
        }

        /// <summary>
        /// Gets or sets Text displayed in finish  when the task is in Manually Scheduled mode
        /// </summary>
        [System.Xml.Serialization.XmlElement("ManualFinish", DataType = "string")]
        public string ManualFinish
        {
            get
            {
                return this.m_pinnedFinish;
            }
            set
            {
                this.m_pinnedFinish = value;
            }
        }

        /// <summary>
        /// Gets or sets Text displayed in duration  when the task is in Manually Scheduled mode
        /// </summary>
        [System.Xml.Serialization.XmlElement("ManualDuration", DataType = "string")]
        public string ManualDuration
        {
            get
            {
                return this.m_pinnedDuration;
            }
            set
            {
                this.m_pinnedDuration = value;
            }
        }

        /// <summary>
        /// Collection of Timephased Data
        /// </summary>
        [System.Xml.Serialization.XmlElement("TimephasedData")]
        public TimephasedDataType[] TimephasedData
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

        /// <summary>
        /// Gets or sets the list of Child tasks of the current task
        /// </summary>
        [XmlElement("Task")]
        public List<Task> Children
        {
            get
            {
                return this.m_children;
            }
            set
            {
                this.m_children = value;
            }
        }

        /// <summary>
        /// Gets or sets the Parent task of the current task
        /// </summary>
        [XmlIgnore()]
        public Task Parent
        {
            get
            {
                if (this.m_parent == null)
                    return new Task();
                else
                    return this.m_parent;
            }
            set
            {
                this.m_parent = value;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Calculate no. of days between task start date and task finish date
        /// </summary>
        /// <param name="start">Start date of task</param>
        /// <param name="finish">Finish date of task</param>
        /// <returns>No. of days</returns>
        internal int GetNoOfDays(DateTime start, DateTime finish)
        {
            int days = 0;
            while ((start.DayOfWeek == DayOfWeek.Saturday) || (start.DayOfWeek == DayOfWeek.Sunday))
                start = start.AddDays(1);
            while (start < finish)
            {
                if (start.DayOfWeek == DayOfWeek.Saturday)
                    start = start.AddDays(2);
                else
                {
                    start = start.AddDays(1);
                    days++;
                }
            }
            return days;
        }

        /// <summary>
        /// Adds days to given date
        /// </summary>
        /// <param name="dt">Given date</param>
        /// <param name="n">No. of days to add</param>
        /// <returns>New date</returns>
        internal DateTime AddDays(DateTime dt, int n)
        {
            DateTime d = dt;
            while (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday)
                d = d.AddDays(1);
            while (n > 0)
            {
                if (d.DayOfWeek == DayOfWeek.Saturday)
                    d = d.AddDays(2);
                else
                    d = d.AddDays(1);
                n--;
            }
            return d;
        }
        #endregion
    }   
}

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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;

namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Represents a Project
    /// </summary>
    [System.Serializable()]
    [System.Xml.Serialization.XmlRootAttribute("Project",Namespace = "http://schemas.microsoft.com/project", IsNullable = false)]
    public class Project
    {
        #region Fields
        private int m_saveVersion;
        private string m_uID;
        private string m_name;
        private string m_title;
        private string m_subject;
        private string m_category;
        private string m_company;
        private string m_manager;
        private string m_author;
        private DateTime m_creationDate;
        private int m_revision;
        private bool m_brevisionSpecified;
        private System.DateTime m_lastSaved;
        private bool m_bscheduleFromStart;
        private bool m_bscheduleFromStartSpecified;
        private System.DateTime m_startDate;
        private System.DateTime m_finishDate;
        private FYStartDate m_fYStartDate;
        private int m_criticalSlackLimit;
        private int m_currencyDigits;
        private string m_currencySymbol;
        private string m_currencyCode;
        private CurrencySymbolPosition m_currencySymbolPosition;
        private bool m_bcurrencySymbolPositionSpecified;
        private int m_calendarUID;
        private Calendar m_calendar;
        private System.DateTime m_defaultStartTime;
        private System.DateTime m_defaultStartTime1;
        private bool m_bdefaultStartTimeSpecified;
        private System.DateTime m_defaultFinishTime;
        private System.DateTime m_defaultFinishTime1;
        private bool m_bdefaultFinishTimeSpecified;
        private int m_minutesPerDay;
        private int m_minutesPerWeek;
        private int m_daysPerMonth;
        private TaskType m_defaultTaskType;
        private DefaultFixedCostAccrual m_defaultFixedCostAccrual;
        private float m_defaultStandardRate;
        private float m_defaultOvertimeRate;
        private DurationFormat m_durationFormat;
        private bool m_bdurationFormatSpecified;
        private WorkFormat m_workFormat;
        private bool m_bworkFormatSpecified;
        private bool m_beditableActualCosts;
        private bool m_bhonorConstraints;
        private EarnedValueMethod m_earnedValueMethod;
        private bool m_bearnedValueMethodSpecified;
        private bool m_binsertedProjectsLikeSummary;
        private bool m_bmultipleCriticalPaths;
        private bool m_bnewTasksEffortDriven;
        private bool m_bnewTasksEstimated;
        private bool m_bsplitsInProgressTasks;
        private bool m_bspreadActualCost;
        private bool m_bspreadPercentComplete;
        private bool m_btaskUpdatesResource;
        private bool m_bfiscalYearStart;
        private WeekStartDay m_weekStartDay;
        private bool m_bweekStartDaySpecified;
        private bool m_bmoveCompletedEndsBack;
        private bool m_bmoveRemainingStartsBack;
        private bool m_bmoveRemainingStartsForward;
        private bool m_bmoveCompletedEndsForward;
        private BaselineForEarnedValue m_baselineForEarnedValue;
        private bool m_bbaselineForEarnedValueSpecified;
        private bool m_bautoAddNewResourcesAndTasks;
        private System.DateTime m_statusDate;
        private bool m_bstatusDateSpecified;
        private System.DateTime m_currentDate;
        private bool m_bmicrosoftProjectServerURL;
        private bool m_bautolink;
        private NewTaskStartDate m_newTaskStartDate;
        private EarnedValueMethod m_defaultTaskEVMethod;
        private bool m_bprojectExternallyEdited;
        private System.DateTime m_extendedCreationDate;
        private bool m_bactualsInSync;
        private bool m_bremoveFileProperties;
        private bool m_badminProject;
        private string m_baselineCalendar;
        private bool m_bnewTasksAreManual;
        private bool m_bupdateManuallyScheduledTasksWhenEditingLinks;
        private bool m_bkeepTaskOnNearestWorkingTimeWhenMadeAutoScheduled;
        private List<OutlineCode> m_outlineCodes = new List<OutlineCode>();
        private WBSMasks m_wBSMasks;
        private List<ExtendedAttribute> m_extendedAttributes = new List<ExtendedAttribute>();
        private List<Calendar> m_calendars = new List<Calendar>();
        private List<Resource> m_resources = new List<Resource>();
        private List<Assignment> m_assignments = new List<Assignment>();
        private Tasks m_task = new Tasks();
        private Dictionary<Task, List<Task>> m_taskTree = new Dictionary<Task, List<Task>>();
        bool ParsedValue;
        #endregion Fields

        #region Initializer/Finalizer
        public Project()
        {
            this.m_bscheduleFromStart = true;
            this.m_defaultTaskType = TaskType.FixedDuration;
            this.m_beditableActualCosts = false;
            this.m_bhonorConstraints = true;
            this.m_binsertedProjectsLikeSummary = true;
            this.m_bmultipleCriticalPaths = false;
            this.m_bnewTasksEffortDriven = true;
            this.m_bnewTasksEstimated = true;
            this.m_bsplitsInProgressTasks = true;
            this.m_bspreadActualCost = true;
            this.m_bspreadPercentComplete = false;
            this.m_bmoveCompletedEndsBack = false;
            this.m_bmoveRemainingStartsBack = false;
            this.m_bmoveRemainingStartsForward = false;
            this.m_bmoveCompletedEndsForward = false;
            this.m_bautoAddNewResourcesAndTasks = true;
            AddProjectInfo();            
        }
        #endregion Initializer/Finalizer

        #region Properties
        /// <summary>
        /// Gets or sets the version of Microsoft Office Project from which the project was saved
        /// </summary>
        //[System.Xml.Serialization.XmlElementAttribute(DataType = "integer")]
        [XmlIgnore]
        public int SaveVersion
        {
            get
            {
                return this.m_saveVersion;
            }
            set
            {
                this.m_saveVersion = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("SaveVersion")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string SaveVersionSerialized
        {
            get
            {
                return SaveVersion.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_saveVersion);
            }
        }
        /*[XmlIgnore()]
        public bool SaveVersionSpecified
        {
            get
            {
                return this.m_bsaveVersionSpecified;
            }
            set
            {
                this.m_bsaveVersionSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the unique ID of the project
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "UID", DataType = "string")]
        public string UID
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
        /// Gets or sets the name of the project
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
        /// Gets or sets the title of the project
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Title", DataType = "string")]
        public string Title
        {
            get
            {
                return this.m_title;
            }
            set
            {
                this.m_title = value;
            }
        }

        /// <summary>
        /// Gets or sets the subject of the project
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Subject", DataType = "string")]
        public string Subject
        {
            get
            {
                return this.m_subject;
            }
            set
            {
                this.m_subject = value;
            }
        }

        /// <summary>
        /// Gets or sets the category of the project
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Category", DataType = "string")]
        public string Category
        {
            get
            {
                return this.m_category;
            }
            set
            {
                this.m_category = value;
            }
        }

        /// <summary>
        /// Gets or sets the company that owns the project
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Company", DataType = "string")]
        public string Company
        {
            get
            {
                return this.m_company;
            }
            set
            {
                this.m_company = value;
            }
        }

        /// <summary>
        /// Gets or sets the manager of the project
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Manager", DataType = "string")]
        public string Manager
        {
            get
            {
                return this.m_manager;
            }
            set
            {
                this.m_manager = value;
            }
        }

        /// <summary>
        /// Gets or sets the author of the project
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Author", DataType = "string")]
        public string Author
        {
            get
            {
                return this.m_author;
            }
            set
            {
                this.m_author = value;
            }
        }

        /// <summary>
        /// Gets or sets the date that the project was created
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute("CreationDate",DataType = "dateTime")]
        public System.DateTime CreationDate
        {
            get
            {
                return this.m_creationDate;
            }
            set
            {
                this.m_creationDate =value; 
            }
        }
        
        /// <summary>
        /// Checks whether the date that the project was created is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
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
        /// Gets or sets the number of times a project has been saved
        /// </summary>
        [XmlIgnore]
        public int Revision
        {
            get
            {
                return this.m_revision;
            }
            set
            {
                this.m_revision = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("Revision")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string RevisionSerialized
        {
            get
            {
                return Revision.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_revision);
            }
        }
        [XmlIgnore()]
        public bool RevisionSpecified
        {
            get
            {
                return this.m_brevisionSpecified;
            }
            set
            {
                this.m_brevisionSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the date that the project was last saved
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute("LastSaved",DataType="dateTime")]
        public System.DateTime LastSaved
        {
            get
            {
                return this.m_lastSaved;
            }
            set
            {
                this.m_lastSaved = value;
            }
        }
        
        /// <summary>
        /// Checks whether the date that the project was last saved is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public bool LastSavedSpecified
        {
            get
            {
                return this.m_blastSavedSpecified;
            }
            set
            {
                this.m_blastSavedSpecified = value;
            }
        }*/

        [XmlIgnore()]
        public bool ScheduleFromStart
        {
            get
            {
                return this.m_bscheduleFromStart;
            }
            set
            {
                this.m_bscheduleFromStart = value;
            }
        }

        [XmlElement(ElementName = "ScheduleFromStart")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ScheduleFromStartString
        {
            get
            {
                return ScheduleFromStart ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                ScheduleFromStart = ParsedValue;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public bool ScheduleFromStartSpecified
        {
            get
            {
                return this.m_bscheduleFromStartSpecified;
            }
            set
            {
                this.m_bscheduleFromStartSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the start date of the project
        /// </summary>
        [System.Xml.Serialization.XmlElement(DataType = "dateTime",ElementName="StartDate")]
        //[XmlIgnore()]
        public DateTime StartDate
        {
            get
            {
                return this.m_startDate;
            }
            set
            {
                this.m_startDate = value;
            }
        }
        /*
        [XmlElement("StartDate")]
        public string StartDateString
        {
            get
            {
                return this.m_startDate.ToString("yyyy-mm-ddThh:mm:ss");
            }
            set
            {
                this.m_startDate = DateTime.ParseExact(value, "yyyy-mm-ddThh:mm:ss", null);
            }
        }*/

        /// <summary>
        /// Checks whether the start date of the project is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public bool StartDateSpecified
        {
            get
            {
                return this.m_bstartDateSpecified;
            }
            set
            {
                this.m_bstartDateSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the finish date of the project
        /// </summary>
        [System.Xml.Serialization.XmlElement(DataType="dateTime",ElementName="FinishDate")]
        //[XmlIgnore()]
        public System.DateTime FinishDate
        {
            get
            {
                return this.m_finishDate;
            }
            set
            {
                this.m_finishDate = value;
            }
        }

        /*
        [XmlElement("FinishDate")]
        public string FinishDateString
        {
            get
            {
                return this.m_finishDate.ToString("yyyy-mm-ddThh:mm:ss");
            }
            set
            {
                this.m_finishDate = DateTime.ParseExact(value, "yyyy-mm-ddThh:mm:ss", null);
            }
        }*/
        
        /// <summary>
        /// Checks whether the finish date of the project is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public bool FinishDateSpecified
        {
            get
            {
                return this.m_bfinishDateSpecified;
            }
            set
            {
                this.m_bfinishDateSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the Fiscal Year starting month
        /// </summary>
        [System.Xml.Serialization.XmlElement("FYStartDate")]
        public FYStartDate FYStartDate
        {
            get
            {
                return this.m_fYStartDate;
            }
            set
            {
                this.m_fYStartDate = value;
            }
        }
        
        /*/// <summary>
        /// Checks whether the Fiscal Year starting month is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public bool FYStartDateSpecified
        {
            get
            {
                return this.m_bfYStartDateSpecified;
            }
            set
            {
                this.m_bfYStartDateSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the number of days past its end date that a task can go before Microsoft Project marks that task as a critical task
        /// </summary>
        [XmlIgnore]
        public int CriticalSlackLimit
        {
            get
            {
                return this.m_criticalSlackLimit;
            }
            set
            {
                this.m_criticalSlackLimit = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("CriticalSlackLimit")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string CriticalSlackLimitSerialized
        {
            get
            {
                return CriticalSlackLimit.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_criticalSlackLimit);
            }
        }
        /// <summary>
        /// Gets or sets the number of digits after the decimal symbol
        /// </summary>
        [XmlIgnore]
        public int CurrencyDigits
        {
            get
            {
                return this.m_currencyDigits;
            }
            set
            {
                this.m_currencyDigits = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("CurrencyDigits")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string CurrencyDigitsSerialized
        {
            get
            {
                return CurrencyDigits.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_currencyDigits);
            }
        }
        /// <summary>
        /// Gets or sets the currency symbol used in the project
        /// </summary>
        [System.Xml.Serialization.XmlElement(DataType = "string",ElementName="CurrencySymbol")]
        public string CurrencySymbol
        {
            get
            {
                return this.m_currencySymbol;
            }
            set
            {
                this.m_currencySymbol = value;
            }
        }

        /// <summary>
        /// Gets or sets the three letter currency character code as defined in ISO 4217
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="CurrencyCode",DataType="string")]
        public string CurrencyCode
        {
            get
            {
                return this.m_currencyCode;
            }
            set
            {
                this.m_currencyCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the position of the currency symbol
        /// </summary>
        [System.Xml.Serialization.XmlElement("CurrencySymbolPosition")]
        public CurrencySymbolPosition CurrencySymbolPosition
        {
            get
            {
                return this.m_currencySymbolPosition;
            }
            set
            {
                this.m_currencySymbolPosition = value;
            }
        }
        
        /// <summary>
        /// Checks whether the position of the currency symbol is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public bool CurrencySymbolPositionSpecified
        {
            get
            {
                return this.m_bcurrencySymbolPositionSpecified;
            }
            set
            {
                this.m_bcurrencySymbolPositionSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the project calendar
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
                this.m_calendar = Calendar.StandardCalendar();
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
        /// Gets or sets the calendar of the project
        /// </summary>
        [XmlIgnore()]
        public Calendar Calendar
        {
            get
            {
                return GetCalendarByUID(this.m_calendarUID);
            }
            set
            {
                this.m_calendar = value;
                this.m_calendarUID = this.m_calendar.UID;
            }
        }

        /// <summary>
        /// Gets or sets the default start time of new tasks
        /// </summary>
        [System.Xml.Serialization.XmlIgnore()]
        public System.TimeSpan DefaultStartTime
        {
            get
            {
                return this.m_startDate.TimeOfDay;
            }
            set
            {
                this.m_defaultStartTime1 = new DateTime(this.m_startDate.Year, this.m_startDate.Month, this.m_startDate.Day, value.Hours, value.Minutes, value.Seconds);
            }
        }

        [System.Xml.Serialization.XmlElementAttribute("DefaultStartTime")]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public string StartTimeString
        {
            get
            {
                return this.m_startDate.TimeOfDay.ToString();
            }
            set
            {
                this.m_defaultStartTime = this.m_defaultStartTime1;
            }
        }        
        
        /// <summary>
        /// Checks whether the default start time of new tasks is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public bool DefaultStartTimeSpecified
        {
            get
            {
                return this.m_bdefaultStartTimeSpecified;
            }
            set
            {
                this.m_bdefaultStartTimeSpecified = value;
            }
        }


        [System.Xml.Serialization.XmlIgnore()]
        public System.TimeSpan DefaultFinishTime
        {
            get
            {
                return this.m_finishDate.TimeOfDay;
            }
            set
            {
                this.m_defaultFinishTime1 = new DateTime(this.m_finishDate.Year, this.m_finishDate.Month, this.m_finishDate.Day, value.Hours, value.Minutes, value.Seconds);
            }
        }

        /// <summary>
        /// Gets or sets the default finish time of new tasks
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute("DefaultFinishTime", DataType = "string")]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public string FinshTimeString
        {
            get
            {
                return this.m_finishDate.TimeOfDay.ToString();
            }
            set
            {
                this.m_defaultFinishTime = this.m_defaultFinishTime1;
            }
        } 

        
        /// <summary>
        /// Checks whether the default finish time of new tasks is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public bool DefaultFinishTimeSpecified
        {
            get
            {
                return this.m_bdefaultFinishTimeSpecified;
            }
            set
            {
                this.m_bdefaultFinishTimeSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of minutes per day
        /// </summary>
        [XmlIgnore]
        public int MinutesPerDay
        {
            get
            {
                return this.m_minutesPerDay;
            }
            set
            {
                this.m_minutesPerDay = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("MinutesPerDay")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string MinutesPerDaySerialized
        {
            get
            {
                return MinutesPerDay.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_minutesPerDay);
            }
        }
        /// <summary>
        /// Gets or sets the number of minutes per week
        /// </summary>
        [XmlIgnore]
        public int MinutesPerWeek
        {
            get
            {
                return this.m_minutesPerWeek;
            }
            set
            {
                this.m_minutesPerWeek = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("MinutesPerWeek")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string MinutesPerWeekSerialized
        {
            get
            {
                return MinutesPerWeek.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_minutesPerWeek);
            }
        }
        /// <summary>
        /// Gets or sets the number of days per month
        /// </summary>
        [XmlIgnore]
        public int DaysPerMonth
        {
            get
            {
                return this.m_daysPerMonth;
            }
            set
            {
                this.m_daysPerMonth = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("DaysPerMonth")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string DaysPerMonthSerialized
        {
            get
            {
                return DaysPerMonth.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_daysPerMonth);
            }
        }
        /// <summary>
        /// Gets or sets the default type of new tasks
        /// </summary>
        //[System.ComponentModel.DefaultValue(TaskType.FixedDuration)]
        [XmlElement("DefaultTaskType")]
        public TaskType DefaultTaskType
        {
            get
            {
                return this.m_defaultTaskType;
            }
            set
            {
                this.m_defaultTaskType = value;
            }
        }

        /// <summary>
        /// Gets or sets the default from where fixed costs are accrued
        /// </summary>
        [System.Xml.Serialization.XmlElement("DefaultFixedCostAccrual")]
        public DefaultFixedCostAccrual DefaultFixedCostAccrual
        {
            get
            {
                return this.m_defaultFixedCostAccrual;
            }
            set
            {
                this.m_defaultFixedCostAccrual = value;
            }
        }

        /// <summary>
        /// Gets or sets the default standard rate for new resources
        /// </summary>
        [XmlIgnore]
        public float DefaultStandardRate
        {
            get
            {
                return this.m_defaultStandardRate;
            }
            set
            {
                this.m_defaultStandardRate = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("DefaultStandardRate")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string DefaultStandardRateSerialized
        {
            get
            {
                return DefaultStandardRate.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_defaultStandardRate);
            }
        }
        /// <summary>
        /// Checks whether the default standard rate for new resources is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public bool DefaultStandardRateSpecified
        {
            get
            {
                return this.m_bdefaultStandardRateSpecified;
            }
            set
            {
                this.m_bdefaultStandardRateSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the default overtime rate for new resources
        /// </summary>
        [XmlIgnore]
        public float DefaultOvertimeRate
        {
            get
            {
                return this.m_defaultOvertimeRate;
            }
            set
            {
                this.m_defaultOvertimeRate = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("DefaultOvertimeRate")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string DefaultOvertimeRateSerialized
        {
            get
            {
                return DefaultOvertimeRate.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_defaultOvertimeRate);
            }
        }
        
        /*/// <summary>
        /// Checks whether the default overtime rate for new resources
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public bool DefaultOvertimeRateSpecified
        {
            get
            {
                return this.m_bdefaultOvertimeRateSpecified;
            }
            set
            {
                this.m_bdefaultOvertimeRateSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the format for expressing the bulk duration
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
        /// Checks whether Duration format is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
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
        /// Gets or sets the default work unit format
        /// </summary>
        [System.Xml.Serialization.XmlElement("WorkFormat")]
        public WorkFormat WorkFormat
        {
            get
            {
                return this.m_workFormat;
            }
            set
            {
                this.m_workFormat = value;
            }
        }
        
        /// <summary>
        /// Checks whether Work format is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public bool WorkFormatSpecified
        {
            get
            {
                return this.m_bworkFormatSpecified;
            }
            set
            {
                this.m_bworkFormatSpecified = value;
            }
        }

        /// <summary>
        /// Checks whether or not actual costs are editable
        /// </summary>
        [XmlIgnore()]
        public bool IsEditableActualCosts 
        {
            get
            {
                return this.m_beditableActualCosts;
            }
            set
            {
                this.m_beditableActualCosts = value;
            }
        }

        [XmlElement("EditableActualCosts")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string EditableCostSting
        {
            get
            {
                return IsEditableActualCosts ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                IsEditableActualCosts = ParsedValue;
            }
        }

        /// <summary>
        /// Checks whether tasks honour their constraint dates
        /// </summary>
        [XmlIgnore()]
        public bool HonorConstraints
        {
            get
            {
                return this.m_bhonorConstraints;
            }
            set
            {
                this.m_bhonorConstraints = value;
            }
        }

        [XmlElement("HonorConstraints")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string HonorConstraintsString
        {
            get
            {
                return HonorConstraints ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                HonorConstraints = ParsedValue;
            }
        }

        /// <summary>
        /// Gets or sets the default method for calculating earned value
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
        /// Checks whether Earned Value calculation method is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public bool EarnedValueMethodSpecified
        {
            get
            {
                return this.m_bearnedValueMethodSpecified;
            }
            set
            {
                this.m_bearnedValueMethodSpecified = value;
            }
        }
        
        /// <summary>
        /// Checks whether to calculate subtasks as summary tasks
        /// </summary>
        [XmlIgnore()]
        public bool InsertedProjectsLikeSummary
        {
            get
            {
                return this.m_binsertedProjectsLikeSummary;
            }
            set
            {
                this.m_binsertedProjectsLikeSummary = value;
            }
        }

        [XmlElement("InsertedProjectsLikeSummary")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string InsertedProjectsLikeSummaryString
        {
            get
            {
                return InsertedProjectsLikeSummary ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                InsertedProjectsLikeSummary = ParsedValue;
            }
        }

        /// <summary>
        /// Checks wether multiple critical paths are calculated
        /// </summary>
        [XmlIgnore()]
        public bool MultipleCriticalPaths 
        {
            get
            {
                return this.m_bmultipleCriticalPaths;
            }
            set
            {
                this.m_bmultipleCriticalPaths = value;
            }
        }

        [XmlElement("MultipleCriticalPaths")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string MultipleCriticalPathsString
        {
            get
            {
                return MultipleCriticalPaths ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                MultipleCriticalPaths = ParsedValue;
            }
        }

        /// <summary>
        /// Checks whether new tasks are effort driven
        /// </summary>
        [XmlIgnore()]
        public bool NewTasksEffortDriven 
        {
            get
            {
                return this.m_bnewTasksEffortDriven;
            }
            set
            {
                this.m_bnewTasksEffortDriven = value;
            }
        }
        
        [XmlElement("NewTasksEffortDriven")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string NewTasksEffortDrivenString
        {
            get
            {
                return NewTasksEffortDriven ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                NewTasksEffortDriven = ParsedValue;
            }
        }

        /// <summary>
        /// Checks whether to show the estimated duration by default
        /// </summary>
        [XmlIgnore()]
        public bool NewTasksEstimated 
        {
            get
            {
                return this.m_bnewTasksEstimated;
            }
            set
            {
                this.m_bnewTasksEstimated = value;
            }
        }

        [XmlElement("NewTasksEstimated")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string NewTasksEstimatedString
        {
            get
            {
                return NewTasksEstimated ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                NewTasksEstimated = ParsedValue;
            }
        }

        /// <summary>
        /// Checks whether in-progress tasks can be split
        /// </summary>
        [XmlIgnore()]
        public bool SplitsInProgressTasks
        {
            get
            {
                return this.m_bsplitsInProgressTasks;
            }
            set
            {
                this.m_bsplitsInProgressTasks = value;
            }
        }

        [XmlElement("SplitsInProgressTasks")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string SplitsInProgressTasksString
        {
            get
            {
                return SplitsInProgressTasks ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                SplitsInProgressTasks = ParsedValue;
            }
        }

        /// <summary>
        /// Checks whether actual costs are spread to the status date
        /// </summary>
        [XmlIgnore()]
        public bool SpreadActualCost
        {
            get
            {
                return this.m_bspreadActualCost;
            }
            set
            {
                this.m_bspreadActualCost = value;
            }
        }

        [XmlElement("SpreadActualCost")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string SpreadActualCostString
        {
            get
            {
                return SpreadActualCost ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                SpreadActualCost = ParsedValue;
            }
        }

        /// <summary>
        /// Checks whether percent complete is spread to the status date
        /// </summary>
        [XmlIgnore()]
        public bool SpreadPercentComplete 
        {
            get
            {
                return this.m_bspreadPercentComplete;
            }
            set
            {
                this.m_bspreadPercentComplete = value;
            }
        }

        [XmlElement("SpreadPercentComplete")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string SpreadPercentCompleteString
        {
            get
            {
                return SpreadPercentComplete ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                SpreadPercentComplete = ParsedValue;
            }
        }

        /// <summary>
        /// Checks whether updates to tasks update resources
        /// </summary>
        [XmlIgnore()]
        public bool TaskUpdatesResource 
        {
            get
            {
                return this.m_btaskUpdatesResource;
            }
            set
            {
                this.m_btaskUpdatesResource = value;
            }
        }

        [XmlElement("TaskUpdatesResource")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string TaskUpdatesResourceString
        {
            get
            {
                return TaskUpdatesResource ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                TaskUpdatesResource = ParsedValue;
            }
        }

        /// <summary>
        /// Checks whether to use fiscal year numbering
        /// </summary>
        [XmlIgnore()]
        public bool FiscalYearStart 
        {
            get
            {
                return this.m_bfiscalYearStart;
            }
            set
            {
                this.m_bfiscalYearStart = value;
            }
        }

        [XmlElement("FiscalYearStart")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string FiscalYearStartString
        {
            get
            {
                return FiscalYearStart ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                FiscalYearStart = ParsedValue;
            }
        }

        /// <summary>
        /// Gets or sets the Start day of the week
        /// </summary>
        [System.Xml.Serialization.XmlElement("WeekStartDay")]
        public WeekStartDay WeekStartDay
        {
            get
            {
                return this.m_weekStartDay;
            }
            set
            {
                this.m_weekStartDay = value;
            }
        }
        
        
        /// <summary>
        /// Checks whether the end of completed portions of tasks scheduled to begin after the status date but begun early should be moved back to the status date
        /// </summary>
        [XmlIgnore()]
        public bool MoveCompletedEndsBack 
        {
            get
            {
                return this.m_bmoveCompletedEndsBack;
            }
            set
            {
                this.m_bmoveCompletedEndsBack = value;
            }
        }

        [XmlElement("MoveCompletedEndsBack")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string MoveCompletedEndsBackString
        {
            get
            {
                return MoveCompletedEndsBack ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                MoveCompletedEndsBack = ParsedValue;
            }
        }

        /// <summary>
        /// Checks whether the beginning of remaining portions of tasks scheduled to begin after the status date but begun early should be moved back to the status date
        /// </summary>
        [XmlIgnore()]
        public bool MoveRemainingStartsBack 
        {
            get
            {
                return this.m_bmoveRemainingStartsBack;
            }
            set
            {
                this.m_bmoveRemainingStartsBack = value;
            }
        }

        [XmlElement("MoveRemainingStartsBack")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string MoveRemainingStartsBackString
        {
            get
            {
                return MoveRemainingStartsBack ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                MoveRemainingStartsBack = ParsedValue;
            }
        }

        /// <summary>
        /// Checks whether the beginning of remaining portions of tasks scheduled to have begun late should be moved up to the status date
        /// </summary>
        [XmlIgnore()]
        public bool MoveRemainingStartsForward
        {
            get
            {
                return this.m_bmoveRemainingStartsForward;
            }
            set
            {
                this.m_bmoveRemainingStartsForward = value;
            }
        }

        [XmlElement("MoveRemainingStartsForward")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string MoveRemainingStartsForwardString
        {
            get
            {
                return MoveRemainingStartsForward ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                MoveRemainingStartsForward = ParsedValue;
            }
        }

        /// <summary>
        /// Checks whether the end of completed portions of tasks scheduled to have been completed before the status date but begun late should be moved up to the status date
        /// </summary>
        [XmlIgnore()]
        public bool MoveCompletedEndsForward 
        {
            get
            {
                return this.m_bmoveCompletedEndsForward;
            }
            set
            {
                this.m_bmoveCompletedEndsForward = value;
            }
        }

        [XmlElement("MoveCompletedEndsForward")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string MoveCompletedEndsForwardString
        {
            get
            {
                return MoveCompletedEndsForward ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                MoveCompletedEndsForward = ParsedValue;
            }
        }

        /// <summary>
        /// Gets or sets the specific baseline used to calculate Variance values
        /// </summary>
        [System.Xml.Serialization.XmlElement("BaselineForEarnedValue")]
        public BaselineForEarnedValue BaselineForEarnedValue
        {
            get
            {
                return this.m_baselineForEarnedValue;
            }
            set
            {
                this.m_baselineForEarnedValue = value;
            }
        }
        
        /// <summary>
        /// Checks whether the specific baseline used to calculate Variance values is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public bool BaselineForEarnedValueSpecified
        {
            get
            {
                return this.m_bbaselineForEarnedValueSpecified;
            }
            set
            {
                this.m_bbaselineForEarnedValueSpecified = value;
            }
        }

        /// <summary>
        /// Checks whether to automatically add new resources to the resource pool
        /// </summary>
        [XmlIgnore()]
        public bool AutoAddNewResourcesAndTasks 
        {
            get
            {
                return this.m_bautoAddNewResourcesAndTasks;
            }
            set
            {
                this.m_bautoAddNewResourcesAndTasks = value;
            }
        }

        [XmlElement("AutoAddNewResourcesAndTasks")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string AutoAddNewResourcesAndTasksString
        {
            get
            {
                return AutoAddNewResourcesAndTasks ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                AutoAddNewResourcesAndTasks = ParsedValue;
            }
        }

        /// <summary>
        /// Gets or sets Date used for calculation and reporting
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="StatusDate",DataType="dateTime")]
        public System.DateTime StatusDate
        {
            get
            {
                return this.m_statusDate;
            }
            set
            {
                this.m_statusDate = new DateTime(value.Year, value.Month, value.Day, 8, 0, 0);
            }
        }
        
        
        /// <summary>
        /// Gets or sets the system date that the XML was generated
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "CurrentDate", DataType = "dateTime")]
        public System.DateTime CurrentDate
        {
            get
            {
                return this.m_currentDate;
            }
            set
            {
                this.m_currentDate = new DateTime(value.Year, value.Month, value.Day, 8, 0, 0);
            }
        }

        /*///<summary>
        /// Checks whether the system date that the XML was generated is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public bool CurrentDateSpecified
        {
            get
            {
                return this.m_bcurrentDateSpecified;
            }
            set
            {
                this.m_bcurrentDateSpecified = value;
            }
        }*/

        /// <summary>
        /// Checks whether the project was created by a Project Server user as opposed to an NT user
        /// </summary>
        [XmlIgnore()]
        public bool MicrosoftProjectServerURL
        {
            get
            {
                return this.m_bmicrosoftProjectServerURL;
            }
            set
            {
                this.m_bmicrosoftProjectServerURL = value;
            }
        }

        [XmlElement("MicrosoftProjectServerURL")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string MicrosoftProjectServerURLString
        {
            get
            {
                return MicrosoftProjectServerURL ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                MicrosoftProjectServerURL = ParsedValue;
            }
        }

        /// <summary>
        /// Checks whether to autolink inserted or moved tasks
        /// </summary>
        [XmlIgnore()]
        public bool Autolink 
        {
            get
            {
                return this.m_bautolink;
            }
            set
            {
                this.m_bautolink = value;
            }
        }

        [XmlElement("Autolink")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string AutolinkString
        {
            get
            {
                return Autolink ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                Autolink = ParsedValue;
            }
        }

        /// <summary>
        /// Gets or sets the default date for new tasks start
        /// </summary>
        [System.Xml.Serialization.XmlElement("NewTaskStartDate")]
        public NewTaskStartDate NewTaskStartDate
        {
            get
            {
                return this.m_newTaskStartDate;
            }
            set
            {
                this.m_newTaskStartDate = value;
            }
        }
        
        /*/// <summary>
        /// Checks whether the default date for new tasks start is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public bool NewTaskStartDateSpecified
        {
            get
            {
                return this.m_bnewTaskStartDateSpecified;
            }
            set
            {
                this.m_bnewTaskStartDateSpecified = value;
            }
        }*/

        /// <summary>
        /// Gets or sets the default earned value method for tasks
        /// </summary>
        [System.Xml.Serialization.XmlElement("DefaultTaskEVMethod")]
        public EarnedValueMethod DefaultTaskEVMethod
        {
            get
            {
                return this.m_defaultTaskEVMethod;
            }
            set
            {
                this.m_defaultTaskEVMethod = value;
            }
        }
        
        /*/// <summary>
        /// Checks whether the default earned value method for tasks is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public bool DefaultTaskEVMethodSpecified
        {
            get
            {
                return this.m_bdefaultTaskEVMethodSpecified;
            }
            set
            {
                this.m_bdefaultTaskEVMethodSpecified = value;
            }
        }*/

        /// <summary>
        /// Checks whether the project XML was edited
        /// </summary>
        [XmlIgnore()]
        public bool ProjectExternallyEdited 
        {
            get
            {
                return this.m_bprojectExternallyEdited;
            }
            set
            {
                this.m_bprojectExternallyEdited = value;
            }
        }

        [XmlElement("ProjectExternallyEdited")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ProjectExternallyEditedString
        {
            get
            {
                return ProjectExternallyEdited ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                ProjectExternallyEdited = ParsedValue;
            }
        }

        /// <summary>
        /// Gets or sets Date used for calculation and reporting
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="ExtendedCreationDate",DataType="dateTime")]
        public System.DateTime ExtendedCreationDate
        {
            get
            {
                return this.m_extendedCreationDate;
            }
            set
            {
                this.m_extendedCreationDate = value;
            }
        }
        
        /// <summary>
        /// Checks whether all actual work has been synchronized with the project
        /// </summary>
        [XmlIgnore()]
        public bool ActualsInSync 
        {
            get
            {
                return this.m_bactualsInSync;
            }
            set
            {
                this.m_bactualsInSync = value;
            }
        }

        [XmlElement("ActualsInSync")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ActualsInSyncString
        {
            get
            {
                return ActualsInSync ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                ActualsInSync = ParsedValue;
            }
        }

        /// <summary>
        /// Checks whether to remove all file properties on save
        /// </summary>
        [XmlIgnore()]
        public bool RemoveFileProperties 
        {
            get
            {
                return this.m_bremoveFileProperties;
            }
            set
            {
                this.m_bremoveFileProperties = value;
            }
        }

        [XmlElement("RemoveFileProperties")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string RemoveFilePropertiesString
        {
            get
            {
                return RemoveFileProperties ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                RemoveFileProperties = ParsedValue;
            }
        }

        /// <summary>
        /// Checks whether the project is an administrative project
        /// </summary>
        [XmlIgnore()]
        public bool AdminProject 
        {
            get
            {
                return this.m_badminProject;
            }
            set
            {
                this.m_badminProject = value;
            }
        }

        [XmlElement("AdminProject")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string AdminProjectString
        {
            get
            {
                return AdminProject ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                AdminProject = ParsedValue;
            }
        }

        /// <summary>
        /// Gets or sets the name of the Baseline Calendar
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="BaselineCalendar",DataType="string")]
        public string BaselineCalendar
        {
            get
            {
                return this.m_baselineCalendar;
            }
            set
            {
                this.m_baselineCalendar = value;
            }
        }

        /// <summary>
        /// Checks whether or not new tasks should be made in Manual mode
        /// </summary>
        [XmlIgnore()]
        public bool NewTasksAreManual 
        {
            get
            {
                return this.m_bnewTasksAreManual;
            }
            set
            {
                this.m_bnewTasksAreManual = value;
            }
        }

        [XmlElement("NewTasksAreManual")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string NewTaskAreManualString
        {
            get
            {
                return NewTasksAreManual ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                NewTasksAreManual = ParsedValue;
            }
        }

        /// <summary>
        /// Decides whether or not to update manually scheduled tasks when editing links
        /// </summary>
        [XmlIgnore()]
        public bool UpdateManuallyScheduledTasksWhenEditingLinks 
        {
            get
            {
                return this.m_bupdateManuallyScheduledTasksWhenEditingLinks;
            }
            set
            {
                this.m_bupdateManuallyScheduledTasksWhenEditingLinks = value;
            }
        }

        [XmlElement("UpdateManuallyScheduledTasksWhenEditingLinks")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string UpdateManuallyScheduledTasksWhenEditingLinksString
        {
            get
            {
                return UpdateManuallyScheduledTasksWhenEditingLinks ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                UpdateManuallyScheduledTasksWhenEditingLinks = ParsedValue;
            }
        }

        /// <summary>
        /// Checks whether or not tasks moving from Manual to Auto Scheduled should be moved to the nearest working time
        /// </summary>
        [XmlIgnore()]
        public bool KeepTaskOnNearestWorkingTimeWhenMadeAutoScheduled 
        {
            get
            {
                return this.m_bkeepTaskOnNearestWorkingTimeWhenMadeAutoScheduled;
            }
            set
            {
                this.m_bkeepTaskOnNearestWorkingTimeWhenMadeAutoScheduled = value;
            }
        }

        [XmlElement("KeepTaskOnNearestWorkingTimeWhenMadeAutoScheduled")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string KeepTaskOnNearestWorkingTimeWhenMadeAutoScheduledString
        {
            get
            {
                return KeepTaskOnNearestWorkingTimeWhenMadeAutoScheduled ? "1" : "0";
            }
            set
            {
                if (!Boolean.TryParse(value, out ParsedValue))
                    ParsedValue = XmlConvert.ToBoolean(value);
                KeepTaskOnNearestWorkingTimeWhenMadeAutoScheduled = ParsedValue;
            }
        }

        /// <summary>
        /// Gets or sets the collection of outline code definitions associated with the project
        /// </summary>
        [System.Xml.Serialization.XmlArrayItem("OutlineCode", IsNullable = false)]
        public List<OutlineCode> OutlineCodes
        {
            get
            {
                return this.m_outlineCodes;
            }
            set
            {
                this.m_outlineCodes = value;
            }
        }

        /// <summary>
        /// Gets or sets the table of entries that define the outline code mask
        /// </summary>
        [System.Xml.Serialization.XmlElement("WBSMasks")]
        public WBSMasks WBSMasks
        {
            get
            {
                return this.m_wBSMasks;
            }
            set
            {
                this.m_wBSMasks = value;
            }
        }

        /// <summary>
        /// Gets or sets the collection of extended attribute (custom field) definitions associated with the project
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("ExtendedAttribute", IsNullable = false)]
        public List<ExtendedAttribute> ExtendedAttributes
        {
            get
            {
                return this.m_extendedAttributes;
            }
            set
            {
                this.m_extendedAttributes = value;
            }
        }

        /// <summary>
        /// Gets or sets the collection of calendars that is associated with the project
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("Calendar", IsNullable = false)]
        public List<Calendar> Calendars
        {
            get
            {
                return this.m_calendars;
            }
            set
            {
                this.m_calendars =value;
            }
        }

        /// <summary>
        /// Gets or sets the collection of tasks that make up the project
        /// </summary>
        //[System.Xml.Serialization.XmlArrayItemAttribute("Task", IsNullable = false)]
        [XmlElement("Tasks")]
        public Tasks RootTask
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
        /// Gets or sets the collection of resources that make up the project
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("Resource", IsNullable = false)]
        public List<Resource> Resources
        {
            get
            {
                return this.m_resources;
            }
            set
            {
                this.m_resources = value;
            }
        }

        /// <summary>
        /// Gets or sets the collection of assignments that make up the project
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("Assignment", IsNullable = false)]
        public List<Assignment> Assignments
        {
            get
            {
                return this.m_assignments;
            }
            set
            {
                this.m_assignments = value;
            }
        }

        #endregion Properties

        #region Methods
        
        
        /// <summary>
        /// Writes Project to XML file
        /// </summary>
        /// <param name="filename">XML file name</param>
        public void Save(string filename)
        {
            if (this.RootTask.Children.Count > 0 && this.RootTask.Children[0].WBS != "0")
                AddTaskInfo();
            if (this.RootTask.Children.Count > 0)
            {
                UpdateRootTask();
            }
            int x = 0;
            for (int i = 0; i < this.Calendars.Count; i++)
            {
                if (this.Calendars[i].WeekDays.Count == 0)
                    this.Calendars[i].WeekDays = null;
            }
            if (this.Calendars.Count > 0)
            {
                for (int i = 0; i < this.Calendars.Count; i++)
                {
                    if (this.Calendars[i].Name.Equals("Standard"))
                    {
                        x = 1;
                        break;
                    }
                }
            }
            if ((x != 1) || this.Calendars.Count == 0)
            {
                this.Calendars.Insert(0, Calendar.StandardCalendar());
                this.Calendar = this.Calendars[0];
            }
            if (this.Assignments.Count > 0)
                CalculateAssignmentIDs();
            ProjectWriter Writer = new ProjectWriter();
            FileStream f = new FileStream(filename, FileMode.Create, FileAccess.Write);
            this.Name = Path.GetFileName(filename);
            Writer.Write(this, f);
        }

        /// <summary>
        /// Calculate Assignment IDs and assign other assignment information
        /// </summary>
        internal void CalculateAssignmentIDs()
        {
            int uid = 0;
            for (int i = 0; i < this.Assignments.Count; i++)
            {
                if (this.Assignments[i].UID == 0)
                    this.Assignments[i].UID = uid;
                if (this.Assignments[i].Task != null)
                {
                    this.Assignments[i].Start = this.Assignments[i].Task.Start;
                    this.Assignments[i].Finish = this.Assignments[i].Task.Finish;
                    this.Assignments[i].Work = this.Assignments[i].Task.DurationString;
                }
                this.Assignments[i].PercentWorkComplete = 0;
                this.Assignments[i].ActualCost = 0;
                this.Assignments[i].ActualOvertimeCost = 0;
                this.Assignments[i].Confirmed = false;
                this.Assignments[i].Units = 1f;
                this.Assignments[i].RegularWork = this.Assignments[i].Work;
                this.Assignments[i].RemainingWork = this.Assignments[i].Work;
                if (this.Assignments[i].TimephasedData.Count == 0)
                {
                    TimephasedDataType tdp = new TimephasedDataType();
                    tdp.Type = TimephasedDataTypeType.AssignmentRemainingWork;
                    tdp.UID = this.Assignments[i].TaskUID;
                    this.Assignments[i].TimephasedData.Add(tdp);
                }
                uid++;
            }
        }

        /// <summary>
        /// Calculate WBS of tasks
        /// </summary>
        /// <param name="parent">Parent Task</param>
        /// <param name="t">Current Task</param>
        /// <param name="w">WBS</param>
        /// <param name="index">index position of task in task collection</param>
        internal void CalculateWBS(Task parent, Task t, string w, int index)
        {
            string wbs = w;
            int k = index;
            string outlineNumber = w;
            if (t.Parent.WBS.Equals("0"))
            {
                t.WBS = wbs;
                t.OutlineNumber = outlineNumber;
            }
            else
            {
                t.WBS = wbs + "." + (k + 1).ToString();
                t.OutlineNumber = t.WBS;                
            }
            t.OutlineLevel = t.Parent.OutlineLevel + 1;
            if (t.Children.Count > 0)
            {
                for (int i = 0; i < t.Children.Count; i++)
                {
                    t.Children[i].Parent = t;
                    CalculateWBS(t, t.Children[i], t.WBS, i);
                }
            }
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
        /// Assign Default task information
        /// </summary>
        /// <param name="t">Task object</param>
        internal void AssignDefaultTaskInfo(Task t)
        {            
            t.Type = (TaskType)0;
            t.IsActive = true;
            t.CreateDate = this.CreationDate;
            t.Priority = 500;
            if (t.Children.Count > 0)
            {
                for (int i = 0; i < t.Children.Count; i++)
                {
                    if (t.Children[i].Start < t.Start)
                        t.Start = t.Children[i].Start;
                    if (t.Children[i].Finish > t.Finish)
                        t.Finish = t.Children[i].Finish;
                }
            }
            if (t.Start.Equals(new DateTime()))
                t.Start = DateTime.Now;
            if (t.IsMilestone == true)
                t.Start = new DateTime(t.Start.Year, t.Start.Month, t.Start.Day, 17, 0, 0);
            t.ManualStart = t.StartString;
            t.ManualFinish = t.FinishString;
            t.ManualDuration = "PT" + t.Duration.TotalHours + "H" + t.Duration.Minutes + "M" + t.Duration.Seconds + "S";
            t.Work = new TimeSpan(0, 0, 0);
            t.IsCritical = true;
            t.EarlyStart = t.Start;
            t.EarlyFinish = t.Finish;
            t.LateStart = t.Start;
            t.LateFinish = t.Finish;
            t.WorkVariance = (float)t.Duration.TotalMinutes * 1000;
            t.RegularWork = t.Duration;
            t.RemainingDuration = t.Duration;
            t.RemainingWork = t.Duration;
            t.LevelAssignments = false;
            t.LevelingCanSplit = false;
            t.LevelingDelayFormat = DelayFormat.ElapsedDays;
            t.CommitmentType = 0;
        }

        /// <summary>
        /// Calculate UID of tasks
        /// </summary>
        /// <param name="t">Current Task</param>
        /// <param name="uid">UID to be inserted</param>
        internal int CalculateUID(Task t, int uid)
        {
            t.UID = uid;
            t.ID = t.UID;
            AssignDefaultTaskInfo(t);
            if (t.Children.Count > 0)
            {
                for (int i = 0; i < t.Children.Count; i++)
                {
                    uid++;
                    uid = CalculateUID(t.Children[i], uid);
                }
            }
            return uid;
        }

        /// <summary>
        /// Calculate Task IDs
        /// </summary>
        public void CalculateTaskIDs()
        {
            if (this.RootTask.Children.Count > 0 && this.RootTask.Children[0].WBS == null)
                AddTaskInfo();
            
            List<Task> childTask = new List<Task>();
            List<Task> children = new List<Task>();
            int uid = 1;
            int id = 1;
            int wbs = 1;
            for (int i = 1; i < this.RootTask.Children.Count; i++)
            {
                this.RootTask.Children[i].UID = uid;
                this.RootTask.Children[i].ID = id;
                this.RootTask.Children[i].Parent = this.RootTask.Children[0];
                AssignDefaultTaskInfo(this.RootTask.Children[i]);
                CalculateWBS(this.RootTask.Children[0], this.RootTask.Children[i], wbs.ToString(), i);
                uid = CalculateUID(this.RootTask.Children[i], uid);
                uid++;
                id++;
                wbs++;
            }
        }

        /// <summary>
        /// Calculate Resource IDs
        /// </summary>
        public void CalculateResourceIDs()
        {
            AddResourceInfo();
            int uid = 1;
            int id = 1;
            for (int i = 1; i < this.Resources.Count; i++)
            {
                this.Resources[i].UID = uid;
                this.Resources[i].ID = id;
                this.Resources[i].CanLevel = true;
                this.Resources[i].Work = "PT16H0M0S";
                this.Resources[i].OvertimeWork = "PT0H0M0S";
                this.Resources[i].ActualWork = "PT0H0M0S";
                this.Resources[i].RemainingWork = this.Resources[i].Work;
                this.Resources[i].ActualOvertimeWork = "PT0H0M0S";
                this.Resources[i].RemainingOvertimeWork = "PT0H0M0S";
                this.Resources[i].WorkVariance = 960000;
                uid++;
                id++;
            }
            InsertCalendars();
        }

        /// <summary>
        /// Get the Task using UID
        /// </summary>
        /// <param name="uid">UID of the Task</param>
        /// <returns>Task object</returns>
        public Task GetTaskByUID(int uid)
        {
            Task target = null;
            for (int i = 0; i < this.RootTask.Children.Count; i++)
            {
                if (this.RootTask.Children[i].UID == uid)
                    return this.RootTask.Children[i];
                if (this.RootTask.Children[i].Children.Count > 0)
                {
                    for (int j = 0; j < this.RootTask.Children[i].Children.Count; j++)
                    {
                        target = GetTaskByUID(uid,this.RootTask.Children[i].Children[j]);
                        if (target != null)
                            return target;
                    }
                }
            }
            return target;
        }

        /// <summary>
        /// Get a task using UID
        /// </summary>
        /// <param name="uid">UID of the task</param>
        /// <param name="t">Parent task</param>
        /// <returns>Task object</returns>
        internal Task GetTaskByUID(int uid, Task t)
        {
            Task task = null;
            if (t.UID == uid)
                return t;
            if (t.Children.Count > 0)
            {
                for (int i = 0; i < t.Children.Count; i++)
                {
                    if (t.Children[i].UID == uid)
                        return t.Children[i];
                }
            }
            return task;
        }

        /// <summary>
        /// Get the Resource using UID
        /// </summary>
        /// <param name="uid">UID of the Resource</param>
        /// <returns>Resource object</returns>
        public Resource GetResourceByUID(int uid)
        {
            Resource resource = null;
            for (int i = 1; i < this.Resources.Count; i++)
            {
                if (this.Resources[i].UID == uid)
                {
                    return this.Resources[i];
                }
            }
            return resource;
        }

        /// <summary>
        /// Get Assignment using UID
        /// </summary>
        /// <param name="uid">UID of the Assignment</param>
        /// <returns>Assignment Object</returns>
        public Assignment GetAssignmentByUID(int uid)
        {
            Assignment assignment = null;
            for (int i = 0; i < this.Assignments.Count; i++)
            {
                if (this.Assignments[i].UID == uid)
                {
                    return this.Assignments[i];
                }
            }
            return assignment;
        }

        /// <summary>
        /// Get calendar by UID
        /// </summary>
        /// <param name="uid">UID of calendar</param>
        /// <returns>Calendar object</returns>
        internal Calendar GetCalendarByUID(int uid)
        {
            Calendar cal = null;
            for (int i = 0; i < this.Calendars.Count; i++)
            {
                if (this.Calendars[i].UID == uid)
                    return this.Calendars[i];
            }
            return cal;
        }

        /// <summary> 
        /// Add Default Project Details
        /// </summary>
        internal void AddProjectInfo()
        {
            this.SaveVersion = 14;
            this.Author = System.Environment.MachineName;
            this.CreationDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
            this.LastSaved = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            this.ScheduleFromStart = true;
            this.StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
            this.FinishDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 0, 0);
            this.StatusDate = this.StartDate;
            this.FYStartDate = (FYStartDate)1;
            this.CriticalSlackLimit = 0;
            this.CurrencyDigits = 2;
            this.CurrencySymbol = "$";
            this.CurrencyCode = "USD";
            this.CurrencySymbolPosition = (CurrencySymbolPosition)0;
            this.CalendarUID = 1;
            this.MinutesPerDay = 480;
            this.MinutesPerWeek = 2400;
            this.DaysPerMonth = 20;
            this.DefaultTaskType = (TaskType)0;
            this.DefaultFixedCostAccrual = (DefaultFixedCostAccrual)2;
            this.DefaultStandardRate = 0;
            this.DefaultOvertimeRate = 0;
            this.DurationFormat = (DurationFormat)7;
            this.WorkFormat = (WorkFormat)2;
            this.IsEditableActualCosts = false;
            this.HonorConstraints = true;
            this.InsertedProjectsLikeSummary = true;
            this.MultipleCriticalPaths = false;
            this.NewTaskStartDate = (NewTaskStartDate)0;
            this.NewTasksEffortDriven = false;
            this.NewTasksEstimated = true;
            this.SplitsInProgressTasks = true;
            this.SpreadActualCost = false;
            this.SpreadPercentComplete = false;
            this.TaskUpdatesResource = true;
            this.FiscalYearStart = false;
            this.WeekStartDay = (WeekStartDay)0;
            this.MoveCompletedEndsBack = false;
            this.MoveCompletedEndsForward = false;
            this.MoveRemainingStartsBack = false;
            this.MoveRemainingStartsForward = false;
            this.BaselineForEarnedValue = (BaselineForEarnedValue)0;
            this.AutoAddNewResourcesAndTasks = true;
            this.CurrentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
            this.MicrosoftProjectServerURL = true;
            this.Autolink = false;
            this.NewTaskStartDate = (NewTaskStartDate)0;
            this.NewTasksAreManual = true;
            this.DefaultTaskEVMethod = (EarnedValueMethod)0;
            this.ProjectExternallyEdited = false;
            this.ExtendedCreationDate = new DateTime(1984, 1, 1);
            this.ActualsInSync = true;
            this.RemoveFileProperties = false;
            this.AdminProject = false;
            this.UpdateManuallyScheduledTasksWhenEditingLinks = true;
            this.KeepTaskOnNearestWorkingTimeWhenMadeAutoScheduled = false;
            this.WBSMasks = new WBSMasks();
        }

        /// <summary>
        /// Get list of children/subtasks of a task
        /// </summary>
        /// <param name="t">Task</param>
        /// <returns>List of subtasks</returns>
        internal List<Task> GetChildTasks(Task t)
        {
            List<Task> childTasks = new List<Task>();
            List<Task> children = new List<Task>();
            if (t.Children.Count == 0)
                return null;
            for (int x = 0; x < t.Children.Count; x++)
            {
                children.Add(t.Children[x]);
                childTasks = GetChildTasks(t.Children[x]);
                if (childTasks != null)
                {
                    for (int y = 0; y < childTasks.Count; y++)
                        children.Add(childTasks[y]);
                }
            }
            return children;
        }

        /// <summary>
        /// Update root task
        /// </summary>
        internal void UpdateRootTask()
        {
            List<Task> childTask = new List<Task>();
            List<Task> children = new List<Task>();
            for (int x = 0; x < this.RootTask.Children.Count; x++)
            {
                childTask.Add(this.RootTask.Children[x]);
                children = GetChildTasks(this.RootTask.Children[x]);
                if (children != null)
                {
                    for (int y = 0; y < children.Count; y++)
                        childTask.Add(children[y]);
                }
            }
            this.RootTask.Children = childTask;
            int taskcount = this.RootTask.Children.Count;
            this.StartDate = this.RootTask.Children[1].Start;
            this.FinishDate = this.RootTask.Children[taskcount - 1].Finish;
            this.RootTask.Children[0].Start = this.StartDate;
            this.RootTask.Children[0].Finish = this.FinishDate;
            if (this.RootTask.Children[0].Duration.Equals(new TimeSpan()))
            {
                int days = GetNoOfDays(this.RootTask.Children[0].Start, this.RootTask.Children[0].Finish);
                this.RootTask.Children[0].Duration = new TimeSpan(days * 8, 0, 0);
            }
            this.RootTask.Children[0].ManualStart = this.RootTask.Children[0].StartString;
            this.RootTask.Children[0].ManualFinish = this.RootTask.Children[0].FinishString;
            this.RootTask.Children[0].WorkVariance = (float)this.RootTask.Children[0].Work.TotalMinutes * 1000;
            this.RootTask.Children[0].RegularWork = this.RootTask.Children[0].Work;
            this.RootTask.Children[0].RemainingWork = this.RootTask.Children[0].Work;
            this.RootTask.Children[0].RemainingDuration = this.RootTask.Children[0].Duration;
            this.RootTask.Children[0].ManualDuration = this.RootTask.Children[0].DurationString;

            for (int x = 0; x < this.RootTask.Children.Count; x++)
            {
                DeleteChildren(this.RootTask.Children[x]);
            }
        }

        /// <summary>
        /// Remove children/subtasks of a task
        /// </summary>
        /// <param name="t">Task</param>
        internal void DeleteChildren(Task t)
        {
            if (t.Children.Count != 0)
            {
                for (int i = 0; i < t.Children.Count; i++)
                    DeleteChildren(t.Children[i]);
            }
            t.Children = new List<Task>();
        }

        /// <summary>
        /// Insert Calendars for resources
        /// </summary>
        internal void InsertCalendars()
        {
            int uid = 3;
            for (int i = 1; i < this.Resources.Count; i++)
            {
                Calendar c = new Calendar(this.Resources[i].Name);
                c.UID = uid;
                c.IsBaseCalendar = false;
                c.IsBaselineCalendar = false;
                c.BaseCalendarUID = 1;
                this.Calendars.Add(c);
                this.Resources[i].CalendarUID = c.UID;
                uid++;
            }
        }

        /// <summary>
        /// Add Default task
        /// </summary>
        internal void AddTaskInfo()
        {
            Task t = new Task();
            t.UID = 0;
            t.ID = 0;
            t.IsActive = true;
            t.IsManual = false;
            t.Type = (TaskType)0;
            t.IsNull = false;
            t.WBS = "0";
            t.OutlineNumber = "0";
            t.OutlineLevel = 0;
            t.Priority = 500;
            t.Start = this.StartDate;
            t.Finish = this.FinishDate;
            t.Duration = new TimeSpan(0, 0, 0);
            t.ManualStart = "Today";
            t.ManualFinish = "Today";
            t.ManualDuration = "PT8H0M0S";
            t.DurationFormat = (DurationFormat)53;
            t.Work = new TimeSpan(0, 0, 0);
            t.IsResumeValid = false;
            t.IsEffortDriven = false;
            t.IsRecurring = false;
            t.IsOverAllocated = false;
            t.IsEstimated = true;
            t.IsMilestone = false;
            t.IsSummary = true;
            t.DisplayAsSummary = false;
            t.IsCritical = true;
            t.IsSubproject = false;
            t.IsSubprojectReadOnly = false;
            t.EarlyStart = this.StartDate;
            t.EarlyFinish = this.FinishDate;
            t.LateStart = this.StartDate;
            t.LateFinish = this.FinishDate;
            t.StartVariance = 0;
            t.FinishVariance = 0;
            t.WorkVariance = (float)0.0;
            t.FreeSlack = 0;
            t.TotalSlack = 0;
            t.StartSlack = 0;
            t.FinishSlack = 0;
            t.FixedCost = (float)0.0;
            t.FixedCostAccrual = (TaskFixedCostAccrual)3;
            t.PercentComplete = 0;
            t.PercentWorkComplete = 0;
            t.Cost = 0;
            t.OvertimeCost = 0;
            t.OvertimeWork = new TimeSpan(0, 0, 0);
            t.ActualDuration = new TimeSpan(0, 0, 0);
            t.ActualCost = 0;
            t.ActualOvertimeCost = 0;
            t.ActualWork = new TimeSpan(0, 0, 0);
            t.ActualOvertimeWork = new TimeSpan(0, 0, 0);
            t.RegularWork = new TimeSpan(0, 0, 0);
            t.RemainingDuration = new TimeSpan(8, 0, 0);
            t.RemainingCost = 0;
            t.RemainingWork = new TimeSpan(0, 0, 0);
            t.RemainingOvertimeCost = 0;
            t.RemainingOvertimeWork = new TimeSpan(0, 0, 0);
            t.ACWP = (float)0.0;
            t.CV = (float)0.0;
            t.ConstraintType = (TaskConstraintType)0;
            t.CalendarUID = -1;
            t.LevelAssignments = false;
            t.LevelingCanSplit = false;
            t.LevelingDelay = 0;
            t.LevelingDelayFormat = DelayFormat.ElapsedDays;
            t.IgnoreResourceCalendar = false;
            t.HideBar = false;
            t.IsRollup = false;
            t.BCWS = (float)0.0;
            t.BCWP = (float)0.0;
            t.PhysicalPercentComplete = 0;
            t.EarnedValueMethod = (EarnedValueMethod)0;
            t.IsPublished = false;
            t.CommitmentType = 0;
            t.Parent = new Task();
            this.RootTask.Children.Insert(0, t);
        }

        /// <summary>
        /// Add Default Resource
        /// </summary>
        internal void AddResourceInfo()
        {
            Resource r = new Resource();
            r.UID = 0;
            r.ID = 0;
            r.Type = (ResourceType)1;
            r.IsNull = false;
            r.WorkGroup = (ResourceWorkGroup)0;
            r.MaxUnits = (float)1.0;
            r.PeakUnits = (float)0.0;
            r.IsOverAllocated = false;
            r.Start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 8, 0, 0);
            r.Finish = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 17, 0, 0);
            r.CanLevel = true;
            r.AccrueAt = (ResourceAccrueAt)3;
            r.Work = "PT0H0M0S";
            r.RegularWork = "PT0H0M0S";
            r.OvertimeWork = "PT0H0M0S";
            r.RemainingWork = "PT0H0M0S";
            r.ActualWork = "PT0H0M0S";
            r.ActualOvertimeWork = "PT0H0M0S";
            r.RemainingOvertimeWork = "PT0H0M0S";
            r.PercentWorkComplete = 0;
            r.StandardRate = 0;
            r.StandardRateFormat = (ResourceStandardRateFormat)2;
            r.Cost = 0;
            r.OvertimeRate = 0;
            r.OvertimeRateFormat = (ResourceOvertimeRateFormat)2;
            r.OvertimeCost = 0;
            r.CostPerUse = 0;
            r.ActualCost = 0;
            r.ActualOvertimeCost = 0;
            r.RemainingCost = 0;
            r.RemainingOvertimeCost = 0;
            r.WorkVariance = (float)0.0;
            r.CostVariance = (float)0.0;
            r.SV = (float)0.00;
            r.CV = (float)0.0;
            r.ACWP = (float)0.0;
            r.CalendarUID = 2;
            r.BCWP = (float)0.0;
            r.BCWP = (float)0.0;
            r.IsGeneric = false;
            r.IsInactive = false;
            r.IsEnterprise = false;
            r.BookingType = (BookingType)0;
            r.CreationDate = DateTime.Today;
            r.IsCostResource = false;
            r.IsBudget = false;
            this.Resources.Insert(0, r);
        }

        /// <summary>
        /// Add Default Assignment
        /// </summary>
        internal void AddAssignmentInfo()
        {
            Assignment a = new Assignment();
            a.TaskUID = 0;
            a.ResourceUID = -65535;
            a.PercentWorkComplete = 0;
            a.ActualCost = 0;
            a.ActualOvertimeCost = 0;
            a.ActualOvertimeWork = "PT0H0M0S";
            a.ActualWork = "PT0H0M0S";
            a.ACWP = (float)0.0;
            a.Confirmed = false;
            a.Cost = 0;
            a.CostRateTable = (RateTable)0;
            a.RateScale = (RateScale)0;
            a.CostVariance = (float)0.0;
            a.CV = (float)0.0;
            a.Delay = 0;
            a.Finish = DateTime.Today;
            a.FinishVariance = 0;
            a.WorkVariance = (float)0.0;
            a.HasFixedRateUnits = true;
            a.FixedMaterial = false;
            a.LevelingDelay = 0;
            a.LevelingDelayFormat = (DelayFormat)7;
            a.LinkedFields = false;
            a.IsMilestone = false;
            a.IsOverallocated = false;
            a.OvertimeCost = 0;
            a.OvertimeWork = "PT0H0M0S";
            a.RegularWork = "PT8H0M0S";
            a.RemainingCost = 0;
            a.RemainingOvertimeCost = 0;
            a.RemainingOvertimeWork = "PT0H0M0S";
            a.RemainingWork = "PT8H0M0S";
            a.ResponsePending = false;
            a.Start = DateTime.Today;
            a.StartVariance = 0;
            a.Units = 1;
            a.UpdateNeeded = false;
            a.VAC = (float)0.00;
            a.Work = "PT8H0M0S";
            a.WorkContour = (AssignmentWorkContour)0;
            a.BCWS = (float)0.0;
            a.BCWP = (float)0.0;
            a.BookingType = (BookingType)0;
            a.CreationDate = DateTime.Today;
            a.BudgetCost = 0;
            a.BudgetWork = "PT0H0M0S";
            TimephasedDataType tdp = new TimephasedDataType();
            tdp.Type = (TimephasedDataTypeType)1;
            tdp.UID = 1;
            tdp.Start = DateTime.Today;
            tdp.Finish = DateTime.Today;
            tdp.Unit = (TimephasedDataTypeUnit)2;
            tdp.Value = "PT8H0M0S";
            a.TimephasedData.Add(tdp); 
            this.Assignments.Add(a);
        }
                
        #endregion Methods
    }    
}

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
using System.Globalization;

namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Baseline class for Tasks
    /// </summary>
    [System.Serializable()]
    public class TaskBaseline
    {
        #region Fields
        private List<TimephasedDataType> m_timephasedData = new List<TimephasedDataType>();
        private int m_number;
        private bool m_binterim;
        private System.DateTime m_start;
        private bool m_bstartSpecified;
        private System.DateTime m_finish;
        private bool m_bfinishSpecified;
        private string m_duration;
        private DurationFormat m_durationFormat;
        private bool m_bdurationFormatSpecified;
        private bool m_bestimatedDuration;
        private string m_work;
        private decimal m_cost;
        private bool m_bcostSpecified;
        private float m_bCWS;
        private bool m_bbCWSSpecified;
        private float m_bCWP;
        private bool m_bbCWPSpecified;
        private float m_fixedCost;
        private bool m_bfixedCostSpecified;
        #endregion

        #region Initializer
        public TaskBaseline()
        {
            this.m_binterim = false;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The timephased data block associated with the task baseline
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

        /// <summary>
        /// Gets or sets the unique number of the baseline data record
        /// </summary>
        [XmlIgnore]
        public int Number
        {
            get
            {
                return this.m_number;
            }
            set
            {
                this.m_number = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("Number")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string NumberSerialized
        {
            get
            {
                return Number.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_number);
            }
        }

        /// <summary>
        /// Checks whether this is an interim baseline
        /// </summary>
        //[System.ComponentModel.DefaultValueAttribute("0")]
        [XmlIgnore()]
        public bool Interim
        {
            get
            {
                return this.m_binterim;
            }
            set
            {
                this.m_binterim = value;
            }
        }
        
        [XmlElement("Interim")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string InterimString
        {
            get
            {
                return this.m_binterim ? "1" : "0";
            }
            set
            {
                this.m_binterim = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets the scheduled start date of the task when the baseline was saved
        /// </summary>
        [System.Xml.Serialization.XmlElement("Start",DataType="date")]
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
        /// Checks whether the scheduled start date of the task when the baseline was saved is set
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
        /// Gets or sets the scheduled finish date of the task when the baseline was saved
        /// </summary>
        [System.Xml.Serialization.XmlElement("Finish", DataType = "date")]
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
        /// Checks whether the scheduled finish date of the task when the baseline was saved is set
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
        /// Gets or sets the scheduled duration of the task when the baseline was saved
        /// </summary>
        [System.Xml.Serialization.XmlElement("Duration",DataType = "duration")]
        public string Duration
        {
            get
            {
                return this.m_duration;
            }
            set
            {
                this.m_duration = value;
            }
        }

        /// <summary>
        /// Gets or sets the format for expressing the Duration of the Task baseline
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
        /// Checks whether the baseline duration of the task was estimated
        /// </summary>
        [XmlIgnore()]
        public bool EstimatedDuration
        {
            get
            {
                return this.m_bestimatedDuration;
            }
            set
            {
                this.m_bestimatedDuration = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("EstimatedDuration")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string EstimatedDurationString
        {
            get
            {
                return this.m_bestimatedDuration ? "1" : "0";
            }
            set
            {
                this.m_bestimatedDuration = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets the scheduled work of the task when the baseline was saved
        /// </summary>
        [System.Xml.Serialization.XmlElement("Work",DataType = "duration")]
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
        /// Gets or sets the projected cost of the task when the baseline was saved
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
        /// Checks whether the projected cost of the task when the baseline was saved is set
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
        /// Gets or sets the budgeted cost of work scheduled for the task
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
        /// Checks whether the budgeted cost of work scheduled for the task is set
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
        /// Checks whether the budgeted cost of work performed on the task to date is set
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
        /// Gets or sets the fixed cost of the task when the baseline was saved
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
        /// Checks whether the fixed cost of the task when the baseline was saved is set
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }
        #endregion
    }
}

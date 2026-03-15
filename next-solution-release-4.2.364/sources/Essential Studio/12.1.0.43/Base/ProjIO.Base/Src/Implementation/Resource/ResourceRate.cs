#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.ComponentModel;
using System.Xml.Serialization;
using System.Globalization;
namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Defines Rate information for Resources
    /// </summary>
    [System.Serializable()]
    public class ResourceRate
    {
        #region Fields
        private System.DateTime m_ratesFrom;
        private System.DateTime m_ratesTo;
        private RateTable m_rateTable;
        private bool m_brateTableSpecified;
        private decimal m_standardRate;
        private bool m_bstandardRateSpecified;
        private ResourceStandardRateFormat m_standardRateFormat;
        private bool m_bstandardRateFormatSpecified;
        private decimal m_overtimeRate;
        private bool m_bovertimeRateSpecified;
        private ResourceOvertimeRateFormat m_overtimeRateFormat;
        private bool m_bovertimeRateFormatSpecified;
        private decimal m_costPerUse;
        private bool m_bcostPerUseSpecified;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the date that the rate becomes effective
        /// </summary>
        [System.Xml.Serialization.XmlElement("RatesFrom",DataType="date")]
        public System.DateTime RatesFrom
        {
            get
            {
                return this.m_ratesFrom;
            }
            set
            {
                this.m_ratesFrom = value;
            }
        }

        /// <summary>
        /// Gets or sets the last date that the rate is effective
        /// </summary>
        [System.Xml.Serialization.XmlElement("RatesTo", DataType = "date")]
        public System.DateTime RatesTo
        {
            get
            {
                return this.m_ratesTo;
            }
            set
            {
                this.m_ratesTo = value;
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the rate table for the resource
        /// </summary>
        [System.Xml.Serialization.XmlElement("RateTable")]
        public RateTable RateTable
        {
            get
            {
                return this.m_rateTable;
            }
            set
            {
                this.m_rateTable = value;
            }
        }

        /// <summary>
        /// Checks whether the unique identifier of the rate table for the resource is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RateTableSpecified
        {
            get
            {
                return this.m_brateTableSpecified;
            }
            set
            {
                this.m_brateTableSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the standard rate for the resource for the period specified
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
        /// Checks whether the standard rate for the resource for the period specified is set
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }

        /// <summary>
        /// Gets or ses the units used by Microsoft Office Project to display the standard rate
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
        /// Checks whether Standard Rate Format is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }

        /// <summary>
        /// Gets or sets the overtime rate for the resource for the period specified
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
        /// Checks whether Overtime Rate is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }

        /// <summary>
        /// Gets or sets the units used by Microsoft Office Project to display the overtime rate
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
        /// Checks whether Overtime Rate Format is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }

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
        /// Checks whether Cost Per use of the resource is spcified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        }
        #endregion
    }
}

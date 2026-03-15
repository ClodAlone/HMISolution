#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Xml.Serialization;
using System.ComponentModel;
using System.Globalization;
namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Defines Timephased datatype
    /// </summary>
    [System.Serializable()]
    public class TimephasedDataType
    {
        #region Fields
        private TimephasedDataTypeType m_type;
        private int m_uID;
        private System.DateTime m_start;
        private System.DateTime m_finish;
        private TimephasedDataTypeUnit m_unit;
        private string m_value;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the type of task timephased data
        /// </summary>
        [System.Xml.Serialization.XmlElement("Type")]
        public TimephasedDataTypeType Type
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
        /// Gets or sets the unique identifier of the timephased data record
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
        /// Gets or sets the start date of the timephased data period
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="Start",DataType="dateTime")]
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
        /// Gets or sets the finish date of the timephased data period
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Finish", DataType = "dateTime")]
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
        /// Gets or sets the time unit of the timephased data period
        /// </summary>
        [System.Xml.Serialization.XmlElement("Unit")]
        public TimephasedDataTypeUnit Unit
        {
            get
            {
                return this.m_unit;
            }
            set
            {
                this.m_unit = value;
            }
        }

        
        /// <summary>
        /// Gets or sets the value per unit of time for the timephased data period
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="Value",DataType="string")]
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
        #endregion
    }   
}

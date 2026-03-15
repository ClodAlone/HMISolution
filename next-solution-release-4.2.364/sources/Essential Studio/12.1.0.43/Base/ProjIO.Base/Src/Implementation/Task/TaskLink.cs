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
using System.Globalization;

namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Represents the link between two tasks
    /// </summary>
    [System.Serializable()]
    public class TaskLink
    {
        #region Fields
        private int m_predecessorUID;
        private TaskLinkType m_type;
        private bool m_bcrossProject;
        private string m_crossProjectName;
        private int m_linkLag;
        private DelayFormat m_lagFormat;
        private Task m_predecessor=new Task();
        private Task m_successor=new Task();
        #endregion

        #region Initializer
        public TaskLink() { }

        public TaskLink(Task t1, Task t2, TaskLinkType linktype)
        {
            m_predecessorUID = t1.UID;
            m_predecessor = t1;
            m_successor = t2;
            m_type = linktype;
            m_lagFormat = DelayFormat.Days;
            t2.PredecessorLink.Add(this);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the unique identifier of the predecessor task
        /// </summary>
        [XmlIgnore]
        public int PredecessorUID
        {
            get
            {
                return this.m_predecessorUID;
            }
            set
            {
                this.m_predecessorUID = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("PredecessorUID")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string NPredecessorUIDSerialized
        {
            get
            {
                return PredecessorUID.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_predecessorUID);
            }
        }
        /// <summary>
        /// Gets or sets the link type
        /// </summary>
        [System.Xml.Serialization.XmlElement("Type")]
        public TaskLinkType Type
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
        /// Checks whether the predecessor is part of another project
        /// </summary>
        [XmlIgnore()]
        public bool CrossProject
        {
            get
            {
                return this.m_bcrossProject;
            }
            set
            {
                this.m_bcrossProject = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("CrossProject")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string CrossProjectString
        {
            get
            {
                return this.m_bcrossProject ? "1" : "0";
            }
            set
            {
                this.m_bcrossProject = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets the external predecessor project
        /// </summary>
        [System.Xml.Serialization.XmlElement("CrossProjectName", DataType = "string")]
        public string CrossProjectName
        {
            get
            {
                return this.m_crossProjectName;
            }
            set
            {
                this.m_crossProjectName = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of lag in tenths of a minute
        /// </summary>
        [XmlIgnore]
        public int LinkLag
        {
            get
            {
                return this.m_linkLag;
            }
            set
            {
                this.m_linkLag = value;
            }
        }
        /// <summary>
        /// This property is used while Serializing and Deserializing the Project XML file
        /// </summary>
        [XmlElement("LinkLag")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string LinkLagSerialized
        {
            get
            {
                return LinkLag.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out m_linkLag);
            }
        }
        /// <summary>
        /// Gets or sets the format for expressing the lag format
        /// </summary>
        [System.Xml.Serialization.XmlElement("LagFormat")]
        public DelayFormat LagFormat
        {
            get
            {
                return this.m_lagFormat;
            }
            set
            {
                this.m_lagFormat = value;
            }
        }

        /// <summary>
        /// Checks whether the Link Lag Format is specified
        /// </summary>
        /*[System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LagFormatSpecified
        {
            get
            {
                return this.m_blagFormatSpecified;
            }
            set
            {
                this.m_blagFormatSpecified = value;
            }
        }*/

        [XmlIgnore()]
        public Task Predecessor
        {
            get
            {
                return this.m_predecessor;
            }
            set
            {
                this.m_predecessor = value;
            }
        }

        [XmlIgnore()]
        public Task Successor
        {
            get
            {
                return this.m_successor;
            }
            set
            {
                this.m_successor = value;
            }
        }

        #endregion
    }    
}

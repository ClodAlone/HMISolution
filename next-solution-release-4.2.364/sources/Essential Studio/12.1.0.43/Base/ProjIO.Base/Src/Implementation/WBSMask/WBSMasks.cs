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
    /// Represents WBS Masks
    /// </summary>
    [System.Serializable()]
    public class WBSMasks
    {
        #region Fields
        private bool m_bverifyUniqueCodes;
        private bool m_bgenerateCodes;
        private string m_prefix;
        private List<WBSMask> m_wBSMask = new List<WBSMask>();
        #endregion

        #region Initializer
        /// <summary>
        /// Default Constructor
        /// </summary>
        public WBSMasks()
        {
            this.m_bverifyUniqueCodes = false;
            this.m_bgenerateCodes = false;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Checks whether WBS codes are unique for new tasks
        /// </summary>
        [XmlIgnore()]
        public bool VerifyUniqueCodes
        {
            get
            {
                return this.m_bverifyUniqueCodes;
            }
            set
            {
                this.m_bverifyUniqueCodes = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("VerifyUniqueCodes")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.ComponentModel.DefaultValue("0")]
        public string VerifyUniqueCodesString
        {
            get
            {
                return this.m_bverifyUniqueCodes ? "1" : "0";
            }
            set
            {
                this.m_bverifyUniqueCodes = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Checks whether WBS codes are generated for new tasks
        /// </summary>
        [XmlIgnore()]
        public bool GenerateCodes
        {
            get
            {
                return this.m_bgenerateCodes;
            }
            set
            {
                this.m_bgenerateCodes = value;
            }
        }
        
        [System.Xml.Serialization.XmlElement("GenerateCodes")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.ComponentModel.DefaultValue("0")]
        public string GenerateCodesString
        {
            get
            {
                return this.m_bgenerateCodes ? "1" : "0";
            }
            set
            {
                this.m_bgenerateCodes = XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Gets or sets the prefix for all WBS codes
        /// </summary>
        [System.Xml.Serialization.XmlElement("Prefix",DataType="string")]
        public string Prefix
        {
            get
            {
                return this.m_prefix;
            }
            set
            {
                this.m_prefix = value;
            }
        }

        /// <summary>
        /// Gets or sets the WBS Mask that is applied to all tasks in the Project
        /// </summary>
        [System.Xml.Serialization.XmlElement("WBSMask")]
        public List<WBSMask> WBSMask
        {
            get
            {
                return this.m_wBSMask;
            }
            set
            {
                this.m_wBSMask = value;
            }
        }
        #endregion
    }
}

#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.ProjIO
{
    /// <summary>
    /// WBSMask class
    /// </summary>
    [System.Serializable()]
    public class WBSMask
    {
        #region Fields
        private string m_level;
        private WBSMaskType m_type;
        private string m_length;
        private string m_separator;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the level of the mask
        /// </summary>
        [System.Xml.Serialization.XmlElement("Level",DataType = "integer")]
        public string Level
        {
            get
            {
                return this.m_level;
            }
            set
            {
                this.m_level = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the node value
        /// </summary>
        [System.Xml.Serialization.XmlElement("Type")]
        public WBSMaskType Type
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
        /// Gets or sets the maximum length in characters
        /// </summary>
        [System.Xml.Serialization.XmlElement("Length",DataType="string")]
        public string Length
        {
            get
            {
                return this.m_length;
            }
            set
            {
                this.m_length = value;
            }
        }

        /// <summary>
        /// Gets or sets the separator character of the node
        /// </summary>
        [System.Xml.Serialization.XmlElement("Separator", DataType = "string")]
        public string Separator
        {
            get
            {
                return this.m_separator;
            }
            set
            {
                this.m_separator = value;
            }
        }
        #endregion
    }
}

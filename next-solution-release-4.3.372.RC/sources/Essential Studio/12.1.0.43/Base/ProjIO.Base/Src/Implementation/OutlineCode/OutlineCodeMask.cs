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
    /// OutlineCodeMask class
    /// </summary>
    [System.Serializable()]
    public class OutlineCodeMask
    {
        #region Fields
        private string m_level;
        private OutlineCodeMaskType m_type;
        private bool m_btypeSpecified;
        private string m_length;
        private string m_separator;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the level of the mask
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="Level",DataType = "integer")]
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
        /// Gets or sets the type of mask
        /// </summary>
        [System.Xml.Serialization.XmlElement("Type")]
        public OutlineCodeMaskType Type
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
        /// Checks whether the type of mask is specified
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TypeSpecified
        {
            get
            {
                return this.m_btypeSpecified;
            }
            set
            {
                this.m_btypeSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum length in characters of the outline code values
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName="Length",DataType = "integer")]
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
        /// Gets or sets the separator value of the code values
        /// </summary>
        [System.Xml.Serialization.XmlElement(ElementName = "Separator", DataType = "string")]
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

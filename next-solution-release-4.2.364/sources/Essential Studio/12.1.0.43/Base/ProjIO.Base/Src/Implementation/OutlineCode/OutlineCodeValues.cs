#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;

namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Represents a list of Outline codes
    /// </summary>
    [System.Serializable()]
    public class OulineCodeValues
    {
        #region Fields
        private List<OutlineCodeValue> m_value=new List<OutlineCodeValue>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the individual values
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute("Value")]
        public List<OutlineCodeValue> Value
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

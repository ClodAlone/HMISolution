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
    /// Represents working times 
    /// </summary>
    [System.Serializable()]
    public class WorkingTimes
    {
        #region Fields
        private List<WorkingTime> m_items = new List<WorkingTime>();
        #endregion

        #region Properties
        /// <summary>
        /// Defines the working time during the weekday
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute("WorkingTime")]
        public List<WorkingTime> Items
        {
            get
            {
                return this.m_items;
            }
            set
            {
                this.m_items = value;
            }
        }
        #endregion
    }
}

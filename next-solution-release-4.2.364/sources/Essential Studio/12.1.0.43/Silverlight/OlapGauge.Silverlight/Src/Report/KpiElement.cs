//-------------------------------------------------------------------------------------------------
// <copyright file="KPIElement.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Base.Report
{
    [DataContract]
    public class KpiElement : Element
    {
        #region Constructor
        public KpiElement()
        {
            this.Name = string.Empty;
            this.ShowKPIGoal = true;
            this.ShowKPIStatus = true;
            this.ShowKPITrend = true;
            this.ShowKPIValue = true;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the KPI unique name.
        /// </summary>
        /// <value>The name of the unique.</value>
        [DataMember, DefaultValue(true)]
        public bool ShowKPIGoal { get; set; }

        /// <summary>
        /// Gets or sets the KPI unique name.
        /// </summary>
        /// <value>The name of the unique.</value>
        [DataMember, DefaultValue(true)]
        public bool ShowKPIStatus { get; set; }

        /// <summary>
        /// Gets or sets the KPI unique name.
        /// </summary>
        /// <value>The name of the unique.</value>
        [DataMember, DefaultValue(true)]
        public bool ShowKPITrend { get; set; }

        /// <summary>
        /// Gets or sets the KPI unique name.
        /// </summary>
        /// <value>The name of the unique.</value>
        [DataMember, DefaultValue(true)]
        public bool ShowKPIValue { get; set; }

        /// <summary>
        /// Gets or sets the KPI unique name.
        /// </summary>
        /// <value>The name of the unique.</value>
        [DataMember, DefaultValue("")]
        public string UniqueName { get; set; }
        #endregion

    }
}

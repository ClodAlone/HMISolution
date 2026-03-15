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
using System.ComponentModel;

#if !SILVERLIGHT
using Syncfusion.Olap.Common;
using Syncfusion.Olap.Data;

namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// Represents the Key Performance Indicators(KPI) item.
    /// </summary>
    [Serializable]
    public class KpiElement : Element, ICloneable<KpiElement>
#else

using System.Runtime.Serialization;
namespace Syncfusion.OlapSilverlight.Reports
{
    [DataContract]
    public class KpiElement : Element

#endif
    {
#if !SILVERLIGHT
        #region Private Variables
        [NonSerialized]
        PropertyCollection __properties;
        #endregion
#endif

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="KpiElement"/> class.
        /// </summary>
        public KpiElement()
        {
            this.Name = string.Empty;
            this.ShowKPIGoal = true;
            this.ShowKPIStatus = true;
            this.ShowKPITrend = true;
            this.ShowKPIValue = true;
#if !SILVERLIGHT
            this.__properties = new PropertyCollection();
#endif
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the KPI unique name.
        /// </summary>
        /// <value>The name of the unique.</value>
#if SILVERLIGHT
        [DataMember]
#endif
        [DefaultValue(true)]
        public bool ShowKPIGoal { get; set; }

        /// <summary>
        /// Gets or sets the KPI unique name.
        /// </summary>
        /// <value>The name of the unique.</value>
#if SILVERLIGHT
        [DataMember]
#endif        
        [DefaultValue(true)]
        public bool ShowKPIStatus { get; set; }

        /// <summary>
        /// Gets or sets the KPI unique name.
        /// </summary>
        /// <value>The name of the unique.</value>
#if SILVERLIGHT
        [DataMember]
#endif
        [DefaultValue(true)]
        public bool ShowKPITrend { get; set; }

        /// <summary>
        /// Gets or sets the KPI unique name.
        /// </summary>
        /// <value>The name of the unique.</value>
#if SILVERLIGHT
        [DataMember]
#endif
        [DefaultValue(true)]
        public bool ShowKPIValue { get; set; }

        /// <summary>
        /// Gets or sets the KPI unique name.
        /// </summary>
        /// <value>The name of the unique.</value>
#if SILVERLIGHT
        [DataMember]
#endif
        [DefaultValue("")]
        public string UniqueName { get; set; }
        #endregion

#if !SILVERLIGHT
        #region Public Methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of <see cref="KpiElement"/></returns>
        public new KpiElement Clone()
        {
            KpiElement kpiElement = new KpiElement();
            kpiElement.Name = this.Name;
            kpiElement.ShowKPIGoal = this.ShowKPIGoal;
            kpiElement.ShowKPIStatus = this.ShowKPIStatus;
            kpiElement.ShowKPITrend = this.ShowKPITrend;
            kpiElement.ShowKPIValue = this.ShowKPIValue;
            kpiElement.UniqueName = this.UniqueName;
            kpiElement.Properties = this.Properties.Clone();
            return kpiElement;
        }
        #endregion
#endif
    }
}

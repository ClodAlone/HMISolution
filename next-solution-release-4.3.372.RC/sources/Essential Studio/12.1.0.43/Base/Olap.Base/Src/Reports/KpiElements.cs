//-------------------------------------------------------------------------------------------------
// <copyright file="KPIElements.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;

#if !SILVERLIGHT
using Syncfusion.Olap.Common;

namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// Represents the KPI element information.
    /// </summary>
    [Serializable]
    public class KpiElements : Element, ICloneable<KpiElements>
#else

using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace Syncfusion.OlapSilverlight.Reports
{
    [DataContract]
    public class KpiElements : Element
#endif
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="KpiElements"/> class.
        /// </summary>
        public KpiElements()
        {
            this.Elements = new KpiElementCollection();
        }
        #endregion

        #region Public Methods
#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the elements.
        /// </summary>
        /// <value>The elements.</value>
        public KpiElementCollection Elements { get; set; }

        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="kpiValue">if set to <c>true</c> [kpi value].</param>
        /// <param name="kpiGoal">if set to <c>true</c> [kpi goal].</param>
        /// <param name="kpiStatus">if set to <c>true</c> [kpi status].</param>
        /// <param name="kpiTrend">if set to <c>true</c> [kpi trend].</param>
        public void Add(string name, bool kpiValue, bool kpiGoal, bool kpiStatus, bool kpiTrend)
        {
            this.Elements.Add(new KpiElement
            {
                Name = name,
                ShowKPIGoal = kpiGoal,
                ShowKPITrend = kpiTrend,
                ShowKPIStatus = kpiStatus,
                ShowKPIValue = kpiValue
            });
        }

        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void Add(string name)
        {
            this.Add(name, true, true, true, true);
        }

#if !SILVERLIGHT
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public new KpiElements Clone()
        {
            KpiElements kpiElements = new KpiElements();
            kpiElements.Elements = this.Elements.Clone();
            return kpiElements;
        }
#endif
        #endregion
    }
}

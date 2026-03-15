//-------------------------------------------------------------------------------------------------
// <copyright file="KPIElementCollection.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !SILVERLIGHT
using Syncfusion.Olap.Common;
using Syncfusion.Olap.Manager;

namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// Represents the collection of <see cref="KpiElement"/>.
    /// </summary>
    [Serializable]
    public class KpiElementCollection : CollectionBase, ICloneable<KpiElementCollection>
#else

using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Reports
{
    [CollectionDataContract]
    public class KpiElementCollection : Collection<KpiElement>
#endif
    {
        #region Public Methods
#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Reports.KpiElement"/> at the specified index.
        /// </summary>
        /// <value><see cref="KpiElement"/></value>
        public KpiElement this[int index]
        {
            get
            {
                return (KpiElement)base.List[index];
            }
            set
            {
                base.List[index] = value;
            }
        }

        /// <summary>
        /// Adds the specified KPI element.
        /// </summary>
        /// <param name="kpiElement">The <see cref="KpiElement"/>.</param>
        /// <returns></returns>
        public int Add(KpiElement kpiElement)
        {
            return base.List.Add(kpiElement);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of <see cref="KpiElementCollection"/>.</returns>
        public KpiElementCollection Clone()
        {
            KpiElementCollection kpiElementCollection = new KpiElementCollection();
            foreach (KpiElement kpiElement in kpiElementCollection)
            {
                kpiElementCollection.Add(kpiElement.Clone());
            }

            return kpiElementCollection;
        }

        /// <summary>
        /// Removes the specified KPI element.
        /// </summary>
        /// <param name="kpiElement">The KPI element.</param>
        public void Remove(KpiElement kpiElement)
        {
            base.List.Remove(kpiElement);
        }
#endif
        /// <summary>
        /// Finds the kpi by Name.
        /// </summary>
        /// <param name="kpiName">Name of the kpi.</param>
        /// <returns></returns>
        public KpiElement FindKpiByName(string kpiName)
        {
#if !SILVERLIGHT
            foreach (KpiElement kpiElement in base.List)
#else
            foreach (KpiElement kpiElement in base.Items)
#endif
            {
                if (kpiElement.Name.ToLower() == kpiName.ToLower())
                {
                    return kpiElement;
                }
            }
            return null;
        }
        #endregion
    }
}

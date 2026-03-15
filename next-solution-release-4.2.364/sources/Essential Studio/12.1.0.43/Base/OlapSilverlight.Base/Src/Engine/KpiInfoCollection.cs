#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Syncfusion.OlapSilverlight.Reports;

namespace Syncfusion.OlapSilverlight.Engine
{
    public class KpiInfoCollection : Collection<KpiInfo>
    {
        #region Public Methods
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Descriptor.KPIInformation"/> at the specified index.
        /// </summary>
        /// <value></value>
        public KpiInfo this[int index]
        {
            get
            {
                return (KpiInfo)base.Items[index];
            }

            set
            {
                base.Items[index] = value;
            }
        }

        /// <summary>
        /// Adds the specified kpi information.
        /// </summary>
        /// <param name="kpiInformation">The kpi information.</param>
        /// <returns></returns>
        public void Add(KpiInfo kpiInformation)
        {
            base.Items.Add(kpiInformation);
        }

        /// <summary>
        /// Determines whether [contains] [the specified kpi information].
        /// </summary>
        /// <param name="kpiInformation">The kpi information.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified kpi information]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(KpiInfo kpiInfo)
        {
            foreach (KpiInfo kpiInformation in base.Items)
            {
                if (kpiInformation.MemberName == kpiInfo.MemberName && kpiInformation.Kpi_Name == kpiInfo.Kpi_Name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether [contains] [the specified kpi name].
        /// </summary>
        /// <param name="kpiName">Name of the kpi.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified kpi name]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string kpiName)
        {
            foreach (KpiInfo kpiInformation in base.Items)
            {
                if (kpiInformation.Kpi_Name == kpiName)
                {
                    return true;
                }
            }
            return false;
        }

        public KpiInfo FindByName(string name)
        {
            foreach (KpiInfo kpiInformation in base.Items)
            {
                if (kpiInformation.Kpi_Name == name)
                {
                    return kpiInformation;
                }
            }
            return null;
        }

        public int FindMemberIndexByName(string kpiName, string memberName)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Kpi_Name == kpiName && this[i].MemberName == memberName)
                {
                    return i;
                }
            }
            return -1;
        }


        /// <summary>
        /// Gets or sets the type of the axis.
        /// </summary>
        /// <value>The type of the axis.</value>
        [DefaultValue(KpiAxisType.None)]
        public KpiAxisType AxisType { get; set; }

        /// <summary>
        /// Removes the specified kpi information.
        /// </summary>
        /// <param name="kpiInformation">The kpi information.</param>
        public void Remove(KpiInfo kpiInformation)
        {
            base.Items.Remove(kpiInformation);
        }

        public void RemoveMeasures()
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].GoalIndex == 0 && this[i].StatusIndex == 0 && this[i].TrendIndex == 0)
                {
                    this.RemoveAt(i);
                    i--;
                }
            }
        }
        #endregion
    }
}

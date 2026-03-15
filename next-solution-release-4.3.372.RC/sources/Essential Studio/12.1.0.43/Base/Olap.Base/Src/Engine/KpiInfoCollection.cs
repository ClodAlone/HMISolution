#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections;

#if !SILVERLIGHT
using Syncfusion.Olap.Common;

#endif
using System.ComponentModel;



#if SILVERLIGHT
using Syncfusion.OlapSilverlight.Reports;
using System.Collections.ObjectModel;
namespace Syncfusion.OlapSilverlight.Engine
#else
namespace Syncfusion.Olap.Engine
#endif
{
    /// <summary>
    /// This class holds the Collection of KPI Information.
    /// </summary>
#if SILVERLIGHT
    public class KpiInfoCollection:Collection<KpiInfo>
#else
    public class KpiInfoCollection : CollectionBase, ICloneable<KpiInfoCollection>
#endif
    {
        #region Public Methods
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Engine.KpiInfo"/> at the specified index.
        /// </summary>
        /// <value><see cref="Syncfusion.Olap.Engine.KpiInfo"/></value>
        public KpiInfo this[int index]
        {
            get
            {
#if SILVERLIGHT
                return base.Items[index] as KpiInfo;
#else
                return (KpiInfo)base.List[index];
#endif
            }

            set
            {
#if SILVERLIGHT
                base.Items[index] = value;
#else
                base.List[index] = value;
#endif
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Adds the specified KPI information.
        /// </summary>
        /// <param name="kpiInformation">The KPI information.</param>
        /// <returns></returns>
        public int Add(KpiInfo kpiInformation)
        {
            return base.List.Add(kpiInformation);
        }
#else
        /// <summary>
        /// Adds the specified kpi information.
        /// </summary>
        /// <param name="kpiInformation">The kpi information.</param>
        /// <returns></returns>
        public void Add(KpiInfo kpiInformation)
        {
            base.Items.Add(kpiInformation);

        }
#endif

        /// <summary>
        /// Determines whether [contains] [the specified kpi information].
        /// </summary>
        /// <param name="kpiInfo">The kpi info.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified kpi information]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(KpiInfo kpiInfo)
        {
#if !SILVERLIGHT
            foreach (KpiInfo kpiInformation in base.List)
#else
            foreach(KpiInfo kpiInformation in base.Items)
#endif
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
#if !SILVERLIGHT
            foreach (KpiInfo kpiInformation in base.List)
#else
            foreach (KpiInfo kpiInformation in base.Items)

#endif
            {
                if (kpiInformation.Kpi_Name == kpiName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Finds the name of the KpiInfo.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public KpiInfo FindByName(string name)
        {
#if !SILVERLIGHT
            foreach (KpiInfo kpiInformation in base.List)
#else
            foreach (KpiInfo kpiInformation in base.Items)

#endif
            {
                if (kpiInformation.Kpi_Name == name)
                {
                    return kpiInformation;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds the name of the member by index.
        /// </summary>
        /// <param name="kpiName">Name of the kpi.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns></returns>
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
        /// Merges the specified kpi info collection.
        /// </summary>
        /// <param name="kpiInfoCollection">The kpi info collection.</param>
        /// <returns></returns>
        public KpiInfoCollection Merge(KpiInfoCollection kpiInfoCollection)
        {
            KpiInfoCollection kpis = new KpiInfoCollection();
            KpiInfo kpiInfo;
            for (int i = 0; i < kpiInfoCollection.Count; i++)
            {
                kpiInfo = new KpiInfo(kpiInfoCollection[i]);
                if (kpiInfo.MemberName == null || kpiInfo.MemberName == string.Empty)
                {
                    kpis.Add(kpiInfo);
                }
                else
                {
                    for (int j = (i + 1); j < kpiInfoCollection.Count; j++)
                    {
                        int nextPos = FindNextMemberPosition(i, kpiInfo.Kpi_Name, kpiInfo.MemberName, kpiInfoCollection);
                        if (nextPos > 0)
                        {
                            kpiInfo = kpiInfo.Copy(kpiInfoCollection[nextPos], kpiInfo);
                            kpiInfoCollection.RemoveAt(nextPos);
                            j--;
                        }
                        else
                        {
                            break;
                        }
                    }
                    kpis.Add(kpiInfo);
                }
            }
            return kpis;
        }

        /// <summary>
        /// Finds the next member position.
        /// </summary>
        /// <param name="pos">The pos.</param>
        /// <param name="kpiName">Name of the kpi.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="kpiInfoCollection">The kpi info collection.</param>
        /// <returns></returns>
        public int FindNextMemberPosition(int pos, string kpiName, string memberName, KpiInfoCollection kpiInfoCollection)
        {
            for (int i = (pos + 1); i < kpiInfoCollection.Count; i++)
            {
                if (kpiInfoCollection[i].Kpi_Name == kpiName)
                {
                    if (kpiInfoCollection[i].MemberName == memberName)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public KpiInfoCollection Clone()
        {
            KpiInfoCollection kpiInfoCollection = new KpiInfoCollection();
            foreach (KpiInfo kpiInformation in kpiInfoCollection)
            {
                kpiInfoCollection.Add(kpiInformation);
            }

            return kpiInfoCollection;
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
#if !SILVERLIGHT
            base.List.Remove(kpiInformation);
#else
            base.Items.Remove(kpiInformation); 
#endif
        }

        /// <summary>
        /// Removes the measures.
        /// </summary>
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

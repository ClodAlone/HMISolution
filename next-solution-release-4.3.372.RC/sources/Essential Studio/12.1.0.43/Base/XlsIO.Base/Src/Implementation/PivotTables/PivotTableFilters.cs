#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    class PivotTableFilters
    {
        List<PivotTableFilter> m_pivotFilter = new List<PivotTableFilter>();

        /// <summary> 
        /// get the pivot Filter based on Index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public PivotTableFilter this[int index]
        {
            get
            {
                if (m_pivotFilter.Count > 0 && index < m_pivotFilter.Count)
                    return m_pivotFilter[index];
                else
                    throw new ArgumentOutOfRangeException("Index");
            }
        }

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                return m_pivotFilter.Count;
            }
        }

        /// <summary>
        /// Remove the filter from filter collections of pivot table.
        /// </summary>
        /// <param name="index"></param>
        public void Remove(PivotTableFilter filter)
        {
            m_pivotFilter.Remove(filter);
        }

        /// <summary>
        /// adding the pivot filter 
        /// </summary>
        /// <param name="Parent"></param>
        public void Add(PivotTableFilter pivotTableFilter)
        {
            m_pivotFilter.Add(pivotTableFilter);
        }
        #endregion
    }
}

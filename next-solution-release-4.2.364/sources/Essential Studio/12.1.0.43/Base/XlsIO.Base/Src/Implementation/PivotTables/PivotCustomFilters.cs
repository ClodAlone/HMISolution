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
    class PivotCustomFilters
    {
        #region members
        private bool m_bHasAnd;
        #endregion

        #region Properties
        public bool HasAnd
        {
            get
            {
                return m_bHasAnd;
            }
            set
            {
                m_bHasAnd = value;
            }
        }
        #endregion

        List<PivotCustomFilter> m_pivotCustomFilter = new List<PivotCustomFilter>();

        /// <summary> 
        /// get the pivot Filter based on Index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public PivotCustomFilter this[int index]
        {
            get
            {
                if (m_pivotCustomFilter.Count > 0 && index < m_pivotCustomFilter.Count)
                    return m_pivotCustomFilter[index];
                else
                    throw new ArgumentOutOfRangeException("Index");
            }
        }

        /// <summary>
        /// Get the count of the custom filters.
        /// </summary>
        public int Count
        {
            get
            {
                return m_pivotCustomFilter.Count;
            }
        }

        /// <summary>
        /// adding the pivot filter 
        /// </summary>
        /// <param name="Parent"></param>
        public void Add(PivotCustomFilter customFilter)
        {
            m_pivotCustomFilter.Add(customFilter);
        }
    }
}

#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    class PivotAutoFilter
    {
        #region members

        /// <summary>
        /// Range to which auto filter is applied
        /// </summary>
        private string str_filterRange;
        #endregion

        #region Properties
        /// <summary>
        /// property of range to whcih filter is applied
        /// </summary>
        public string FilterRange
        {
            get
            {
                return str_filterRange;
            }
            set
            {
                str_filterRange = value;
            }
        }
        #endregion

        #region Methods
        List<PivotFilterColumn> m_pivotFilterColumn = new List<PivotFilterColumn>();

        /// <summary> 
        /// get the pivot Filter based on Index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public PivotFilterColumn this[int index]
        {
            get
            {
                if (m_pivotFilterColumn.Count > 0 && index < m_pivotFilterColumn.Count)
                    return m_pivotFilterColumn[index];
                else
                    throw new ArgumentOutOfRangeException("Index");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                return m_pivotFilterColumn.Count;
            }
        }

        /// <summary>
        /// adding the pivot filter 
        /// </summary>
        /// <param name="Parent"></param>
        public void Add(PivotFilterColumn filterColumn)
        {
            m_pivotFilterColumn.Add(filterColumn);
        }
        #endregion
    }
}

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
    class PivotFieldItemsCollections : IPivotFieldItems
    {
        List<PivotFieldItem> m_pivotFilterItem = new List<PivotFieldItem>();

        /// <summary> 
        /// get the pivot items based on index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IPivotFieldItem this[int index]
        {
            get
            {
                if (m_pivotFilterItem.Count > 0 && index < m_pivotFilterItem.Count)
                    return m_pivotFilterItem[index];
                else
                    throw new ArgumentOutOfRangeException("Index");
            }
        }

        public int Count
        {
            get
            {
                return m_pivotFilterItem.Count;
            }
        }
        /// <summary>
        /// Get the field items based on filter text
        /// </summary>
        /// <param name="FilterText"></param>
        /// <returns></returns>
        public IPivotFieldItem this[string FilterText]
        {
            get
            {
                if (m_pivotFilterItem.Count > 0)
                {
                    foreach (PivotFieldItem item in m_pivotFilterItem)
                    {
                        if (item.Text != null && item.Text.Equals(FilterText))
                        {
                            return item;
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// adding the pivot filter item
        /// </summary>
        /// <param name="Parent"></param>
        public void Add(object Parent, string ItemValue)
        {
            PivotFieldItem filterItem = new PivotFieldItem();
            filterItem.Parent = Parent as PivotFieldImpl;
           // filterItem.Visible = true;
            filterItem.Text = ItemValue;
            m_pivotFilterItem.Add(filterItem);
        }
    }
}

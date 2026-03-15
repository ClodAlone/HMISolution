#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    class PivotFilterColumn
    {
        #region Members
        /// <summary>
        /// Zero-based index indicating the AutoFilter column to which this filter information applies
        /// </summary>
        private int m_iColumnId;
        /// <summary>
        /// Flag indicating whether the AutoFilter button for this column is hidden.
        /// </summary>
        private bool m_bHiddenButton;
        /// <summary>
        /// Flag indicating whether the filter button is visible.
        /// </summary>
        private bool m_bShowButton;
        /// <summary>
        /// Custom filter.
        /// </summary>
        private PivotCustomFilters m_customFilters;
        /// <summary>
        /// Filter column type filter.
        /// </summary>
        private FilterColumnFilters m_filterColumnFiltes;
        /// <summary>
        /// Top 10 filter
        /// </summary>
        private PivotTop10Filter m_top10Filter;
        #endregion

        #region Properties
        /// <summary>
        ///  Zero-based index indicating the AutoFilter column to which this filter information applies
        /// </summary>
        public int ColumnId
        {
            get
            {
                return m_iColumnId;
            }
            set
            {
                m_iColumnId = value;
            }
        }
        /// <summary>
        /// Flag indicating whether the AutoFilter button for this column is hidden.
        /// </summary>
        public bool HiddenButton
        {
            get
            {
                return m_bHiddenButton;
            }
            set
            {
                m_bHiddenButton = value;
            }
        }
        /// <summary>
        /// Flag indicating whether the filter button is visible.
        /// </summary>
        public bool ShowButton
        {
            get
            {
                return m_bShowButton;
            }
            set
            {
                m_bShowButton = value;
            }
        }
        /// <summary>
        /// Custom filter
        /// </summary>
        public PivotCustomFilters CustomFilters
        {
            get
            {
                return m_customFilters;
            }
            set
            {
                m_customFilters = value;
            }
        }
        /// <summary>
        /// Filter Column Filter
        /// </summary>
        public FilterColumnFilters FilterColumnFilter
        {
            get
            {
                return m_filterColumnFiltes;
            }
            set
            {
                m_filterColumnFiltes = value;
            }
        }
        /// <summary>
        /// Top 10 filter
        /// </summary>
        public PivotTop10Filter Top10Filters
        {
            get
            {
                return m_top10Filter;
            }
            set
            {
                m_top10Filter = value;
            }
        }
        #endregion
    }
}

#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    class PivotTop10Filter
    {
        #region Members
        /// <summary>
        /// The actual cell value in the range which is used to perform the comparison for this filter.
        /// </summary>
        private double m_dFilterValue;

        /// <summary>
        /// Value of the top 10 filter.
        /// </summary>
        private double m_dValue;

        /// <summary>
        /// Flag indicating whether or not to filter by percent value of the column
        /// </summary>
        private bool m_bPercent;

        /// <summary>
        /// Flag indicating whether or not to filter by top order
        /// </summary>
        private bool m_bTop;
        #endregion

        #region Properties
        /// <summary>
        /// The actual cell value in the range which is used to perform the comparison for this filter.
        /// </summary>
        public double FilterValue
        {
            get
            {
                return m_dFilterValue;
            }
            set
            {
                m_dFilterValue = value;
            }
        }

        /// <summary>
        /// Property for specifying the value of the top 10 filter.
        /// </summary>
        public double Value
        {
            get
            {
                return m_dValue;
            }
            set
            {
                m_dValue = value;
            }
        }

        /// <summary>
        /// Flag indicating whether or not to filter by percent value of the column
        /// </summary>
        public bool IsPercent
        {
            get
            {
                return m_bPercent;
            }
            set
            {
                m_bPercent = value;
            }
        }

        /// <summary>
        /// Flag indicating whether or not to filter by top order
        /// </summary>
        public bool IsTop
        {
            get
            {
                return m_bTop;
            }
            set
            {
                m_bTop = value;
            }
        }

        #endregion
    }
}

#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    class PivotCustomFilter
    {
        #region members
        /// <summary>
        /// Filter operator of the custom filter
        /// </summary>
        private FilterOperator2007 m_filterOperator;
        /// <summary>
        /// String value of the customer filters
        /// </summary>
        private string m_strValue;
        #endregion

        #region Properties

        /// <summary>
        /// Property for filter operator of the custom filter.
        /// </summary>
        public FilterOperator2007 FilterOperator
        {
            get
            {
                return m_filterOperator;
            }
            set
            {
                m_filterOperator = value;
            }
        }

        /// <summary>
        /// Property for string value of the custom filter.
        /// </summary>
        public string Value
        {
            get
            {
                return m_strValue;
            }
            set
            {
                m_strValue = value;
            }
        }
        #endregion
    }
}

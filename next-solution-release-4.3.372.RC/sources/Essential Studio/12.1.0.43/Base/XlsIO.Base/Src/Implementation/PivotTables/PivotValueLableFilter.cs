#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    class PivotValueLableFilter : IPivotValueLableFilter
    {
        #region Members
        /// <summary>
        /// Values 1 of the pivot filter
        /// </summary>
        private string m_strValue1;
        /// <summary>
        /// Value 2 of the pivot filter
        /// </summary>
        private string m_strValue2;
        /// <summary>
        /// Types of the filter applied to pivot filter.
        /// </summary>
        private PivotFilterType m_type;
        /// <summary>
        /// Field to which filter is applied
        /// </summary>
        private IPivotField m_field;
        #endregion

        #region Properties
        /// <summary>
        /// Value 1 of the pivot filter.
        /// </summary>
        public string Value1
        {
            get
            {
                return m_strValue1;
            }
            set
            {
                m_strValue1 = value;
            }
        }
        /// <summary>
        /// Value 2 of the pivot filter.
        /// </summary>
        public string Value2
        {
            get
            {
                return m_strValue2;
            }
            set
            {
                m_strValue2 = value;
            }
        }
        /// <summary>
        /// Type of the pivot filter applied to pivot table.
        /// </summary>
        public PivotFilterType Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
            }
        }
        /// <summary>
        /// Fiedl to which pivot filter is applied.
        /// </summary>
        public IPivotField DataField
        {
            get
            {
                return m_field;
            }
            set
            {
                m_field = value;
            }
        }
        #endregion

    }
}

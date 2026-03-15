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
    class PivotTableFilter
    {

        #region Members
        /// <summary>
        /// Specifies the description of the pivot filter.
        /// </summary>
        private string m_DescriptionAttribute;
        /// <summary>
        /// Specifies the evaluation order of the pivot filter
        /// </summary>
        private int i_EvalOrder;
        /// <summary>
        /// Specifies the index of the field to which this pivot filter belongs.
        /// </summary>
        private int i_Field;
        /// <summary>
        /// Specifies the unique identifier of the pivot filter as assigned by the PivotTable
        /// </summary>
        private int i_FilterId;
        /// <summary>
        /// Specifies the string value "1" used by label pivot filters.
        /// </summary>
        private string str_Value1;
        /// <summary>
        /// Specifies the string value "2" used by label pivot filters.
        /// </summary>
        private string str_Value2;
        /// <summary>
        /// Pivot filter type.
        /// </summary>
        private PivotFilterType type;
        /// <summary>
        /// Specifies the index of the measure field
        /// </summary>
        private int m_iMeasureFld;
        #endregion

        #region Properties
        /// <summary>
        /// Specifies the description of the pivot filter.
        /// </summary>
        public string DescriptionAttribute
        {
            get
            {
                return m_DescriptionAttribute;
            }
            set
            {
                m_DescriptionAttribute = value;
            }
        }
        /// <summary>
        /// Specifies the evaluation order of the pivot filter
        /// </summary>
        public int EvalOrder
        {
            get
            {
                return i_EvalOrder;
            }
            set
            {
                i_EvalOrder = value;
            }
        }
        /// <summary>
        /// Specifies the index of the field to which this pivot filter belongs.
        /// </summary>
        public int Field
        {
            get
            {
                return i_Field;
            }
            set
            {
                i_Field = value;
            }
        }
        /// <summary>
        /// Specifies the index of the measure field
        /// </summary>
        public int MeasureFld
        {
            get
            {
                return m_iMeasureFld;
            }
            set
            {
                m_iMeasureFld = value;
            }
        }
        /// <summary>
        /// Specifies the unique identifier of the pivot filter as assigned by the PivotTable
        /// </summary>
        public int FilterId
        {
            get
            {
                return i_FilterId;
            }
            set
            {
                i_FilterId = value;
            }
        }
        /// <summary>
        /// Specifies the string value "1" used by label pivot filters.
        /// </summary>
        public string Value1
        {
            get
            {
                return str_Value1;
            }
            set
            {
                str_Value1 = value;
            }
        }
        /// <summary>
        /// Specifies the string value "2" used by label pivot filters.
        /// </summary>
        public string Value2
        {
            get
            {
                return str_Value2;
            }
            set
            {
                str_Value2 = value;
            }
        }
        /// <summary>
        /// Pivot filter type
        /// </summary>
        public PivotFilterType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
        #endregion

        #region Methods
        List<PivotAutoFilter> m_pivotAutoFilter = new List<PivotAutoFilter>();

        /// <summary> 
        /// get the pivot Filter based on Index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public PivotAutoFilter this[int index]
        {
            get
            {
                if (m_pivotAutoFilter.Count > 0 && index < m_pivotAutoFilter.Count)
                    return m_pivotAutoFilter[index];
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
                return m_pivotAutoFilter.Count;
            }
        }

        /// <summary>
        /// adding the pivot filter 
        /// </summary>
        /// <param name="Parent"></param>
        public void Add(PivotAutoFilter AutoFilter)
        {
            m_pivotAutoFilter.Add(AutoFilter);
        }
        #endregion
    }
}

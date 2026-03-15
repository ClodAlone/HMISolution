#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    /// <summary>
    /// 
    /// </summary>
    public enum CellType
    {
        None,

        RowHeader,

        ColumnHeader,

        RowGrandTotal,

        RowSubTotal,

        ColumnSubTotal,

        ColumnGrandTotal,

        ValueCell,
    }

    [Flags]
    public enum PivotTableParts
    {

        WholeTable = 1 << 0,

        PageFieldsLabels = 1 << 1,

        PageFieldsValues = 1 << 2,

        FirstColumnStripe = 1 << 3,

        SecondColumnStripe = 1 << 4,

        FirstRowStripe = 1 << 5,

        SecondRowStripe = 1 << 6,

        FirstColumn = 1 << 7,

        HeaderRow = 1 << 8,

        FirstHeaderCell = 1 << 9,

        SubtotalColumn1 = 1 << 10,

        SubtotalColumn2 = 1 << 11,

        SubtotalColumn3 = 1 << 12,

        BlankRow = 1 << 13,

        SubtotalRow1 = 1 << 14,

        SubtotalRow2 = 1 << 15,

        SubtotalRow3 = 1 << 16,

        ColumnSubHeading1 = 1 << 17,

        ColumnSubHeading2 = 1 << 18,

        ColumnSubHeading3 = 1 << 19,

        RowSubHeading1 = 1 << 20,

        RowSubHeading2 = 1 << 21,

        RowSubHeading3 = 1 << 22,

        GrandTotalColumn = 1 << 23,

        GrandTotalRow = 1 << 24,

        None = 1 << 25,


    }

    /// <summary>
    /// 
    /// </summary>
    public class PivotValueCollections
    {
        #region Members
        /// <summary>
        /// Specify the immediate row header for any pivot value.
        /// </summary>
        IRange m_strImmediateRowHeader;
        /// <summary>
        /// Specify the immediate column header for any pivot value
        /// </summary>
        IRange m_strImmediateColumnHeader;

        /// <summary>
        /// Specifies the cell type.
        /// </summary>
        CellType m_pivotCellType;

        /// <summary>
        /// Specifies the Value
        /// </summary>
        string m_strValue;

        /// <summary>
        /// 
        /// </summary>
        PivotTableParts m_PivotTablePartStyle;
        ExtendedFormatImpl m_XF;
        #endregion

        #region Properties

        public PivotTableParts PivotTablePartStyle
        {
            get
            {
                return m_PivotTablePartStyle;
            }
            set
            {
                m_PivotTablePartStyle = value;
            }
        }
        /// <summary>
        /// Property which specifies the immediate row header for any pivot value.
        /// </summary>
        public IRange ImmediateRowHeader
        {
            get
            {
                return m_strImmediateRowHeader;
            }
            set
            {
                m_strImmediateRowHeader = value;
            }
        }

        /// <summary>
        /// Property which specifies the immediate column header for any pivot value
        /// </summary>
        public IRange ImmediateColumnHeader
        {
            get
            {
                return m_strImmediateColumnHeader;
            }
            set
            {
                m_strImmediateColumnHeader = value;
            }
        }

        /// <summary>
        /// Property which specifies the cell type
        /// </summary>
        public CellType PivotCellType
        {
            get
            {
                return m_pivotCellType;
            }
            set
            {
                m_pivotCellType = value;
            }
        }

        /// <summary>
        /// Property indicating the values
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

        public ExtendedFormatImpl XF
        {
            get
            {
                return m_XF;
            }
            set
            {
                m_XF = value;

            }
        }
        #endregion

        #region Methods
        public PivotValueCollections()
        {
            PivotCellType = CellType.None;
            ImmediateRowHeader = null;
            ImmediateColumnHeader = null;
            Value = null;
        }
        #endregion
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="rowIndex"></param>
        ///// <param name="colIndex"></param>
        ///// <returns></returns>
        //public PivotValue this[int rowIndex, int colIndex]
        //{
        //    get { return this[rowIndex][colIndex]; }
        //    set { this[rowIndex][colIndex] = value; }
        //}
    }
}


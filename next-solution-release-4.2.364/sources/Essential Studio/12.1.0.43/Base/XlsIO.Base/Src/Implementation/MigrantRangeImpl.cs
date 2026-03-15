#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.XlsIO.Implementation
{
    /// <summary>
    /// Summary description for MigrantRangeImpl.
    /// </summary>
    public class MigrantRangeImpl
    : RangeImpl
    , IMigrantRange
    {
        #region Members
        IWorksheet sheet;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="parent"></param>
        public MigrantRangeImpl(IApplication application, IWorksheet parent)
            : base(application, parent)
        {
            sheet = parent;
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Resets row and column values.
        /// </summary>
        /// <param name="iRow">One-based row index of the new cell address.</param>
        /// <param name="iColumn">One-based column index of the new cell address.</param>
        public void ResetRowColumn(int iRow, int iColumn)
        {
            m_dataValidation = null;
            m_rtfString = null;
            //m_iTopLeftCell = m_iBottomRightCell = RangeImpl.GetCellIndex( iColumn, iRow );
            m_iTopRow = m_iBottomRow = iRow;
            m_iLeftColumn = m_iRightColumn = iColumn;

            if (m_style != null)
            {
                m_style.SetFormatIndex(ExtendedFormatIndex);
            }
        }
        #endregion

        #region Overrided Methods
        public void SetValue(int value)
        {
            sheet.SetNumber(m_iTopRow, m_iLeftColumn, value);
        }
        public void SetValue(double value)
        {
            sheet.SetNumber(m_iTopRow, m_iLeftColumn, value);
        }
        public void SetValue(DateTime value)
        {
            DateTime = value;
        }
        public void SetValue(bool value)
        {
            sheet.SetBoolean(m_iTopRow, m_iLeftColumn, value);
        }
        public void SetValue(string value)
        {
            sheet.SetText(m_iTopRow, m_iLeftColumn, value);
        }
        #endregion
    }
}

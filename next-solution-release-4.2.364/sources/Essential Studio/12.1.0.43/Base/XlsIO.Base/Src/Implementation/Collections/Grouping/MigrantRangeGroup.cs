#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.XlsIO.Implementation.Collections.Grouping
{
	/// <summary>
	/// Summary description for MigrantRangeGroup.
	/// </summary>
	public class MigrantRangeGroup
    : RangeGroup
    , IMigrantRange
	{
        #region Members
        IWorksheet sheet;
        #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance of the object.
    /// </summary>
    /// <param name="application">Application object for the new object.</param>
    /// <param name="parent">Parent object for the new object.</param>
    public MigrantRangeGroup( IApplication application, object parent )
      : base( application, parent )
    {
        sheet = parent as IWorksheet;
    }
    #endregion

    #region IMigrantRange Members
    /// <summary>
    /// Resets row and column values.
    /// </summary>
    /// <param name="iRow">One-based row index of the new cell address.</param>
    /// <param name="iColumn">One-based column index of the new cell address.</param>
    public void ResetRowColumn(int iRow, int iColumn)
    {
      m_iFirstColumn = m_iLastColumn = iColumn;
      m_iFirstRow = m_iLastRow = iRow;
    }

    #endregion
    #region Overrided Methods
    public void SetValue(int value)
    {
        sheet.SetNumber(m_iFirstRow, m_iFirstColumn, value);
    }
    public void SetValue(double value)
    {
        sheet.SetNumber(m_iFirstRow, m_iFirstColumn, value);
    }
    public void SetValue(DateTime value)
    {
        DateTime = value;
    }
    public void SetValue(bool value)
    {
        sheet.SetBoolean(m_iFirstRow, m_iFirstColumn, value);
    }
    public void SetValue(string value)
    {
        sheet.SetText(m_iFirstRow, m_iFirstColumn, value);
    }
    #endregion
  }
}

#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.Collections;

namespace Syncfusion.XlsIO.Implementation
{
  class RowsClearer : IOperation
  {
    #region Members
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    private WorksheetImpl m_sheet;
    /// <summary>
    /// Index of the remove row operation.
    /// </summary>
    private int m_iIndex;
    /// <summary>
    /// Number of rows to remove.
    /// </summary>
    private int m_iCount;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the class.
    /// </summary>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="index">Index of the remove row operation.</param>
    /// <param name="count">Number of rows being removed.</param>
    public RowsClearer( WorksheetImpl sheet, int index, int count )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      m_sheet = sheet;
      m_iIndex = index;
      m_iCount = count;
    }
    /// <summary>
    /// Clears necessary rows.
    /// </summary>
    public void Do()
    {
      ArrayListEx arrRows = m_sheet.CellRecords.Table.Rows;
      for( int i = 0, iRowIndex = m_iIndex - 1; i < m_iCount; i++, iRowIndex++ )
      {
        RowStorage arrRow = arrRows[ iRowIndex ];
        arrRows[ iRowIndex ] = null;

        if( arrRow != null )
          arrRow.Dispose();
      }
    }
    #endregion
  }
}

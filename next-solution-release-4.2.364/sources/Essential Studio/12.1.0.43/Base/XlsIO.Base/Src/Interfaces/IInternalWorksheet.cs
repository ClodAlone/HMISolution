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
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.XlsIO.Interfaces
{
  /// <summary>
  /// Represents internal worksheet object. It can be internal or external.
  /// </summary>
  public interface IInternalWorksheet : IWorksheet
  {
    #region Methods
    /// <summary>
    /// Return default row height in pixel.
    /// </summary>
    int DefaultRowHeight
    {
      get;
    }
    /// <summary>
    /// Gets or sets one-based index of the first row of the worksheet.
    /// </summary>
    int FirstRow
    {
      get;
      set;
    }
    /// <summary>
    /// Gets or sets one-based index of the first column of the worksheet.
    /// </summary>
    int FirstColumn
    {
      get;
      set;
    }
    /// <summary>
    /// Gets or sets one-based index of the last row of the worksheet.
    /// </summary>
    int LastRow
    {
      get;
      set;
    }
    /// <summary>
    /// Gets or sets one-based index of the last column of the worksheet.
    /// </summary>
    int LastColumn
    {
      get;
      set;
    }
    /// <summary>
    /// Returns collection of cell records. Read-only.
    /// </summary>
    CellRecordCollection CellRecords
    {
      get;
    }
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    WorkbookImpl ParentWorkbook
    {
      get;
    }
    ExcelVersion Version
    {
      get;
    }
    bool IsArrayFormula( long index );
    /// <summary>
    /// Gets object that is clone of current worksheet in the specified workbook.
    /// </summary>
    /// <param name="hashNewNames">Dictionary with update worksheet names.</param>
    /// <param name="book">New workbook object.</param>
    /// <returns>Object that is clone of the current worksheet.</returns>
    IInternalWorksheet GetClonedObject( Dictionary<string, string> hashNewNames, WorkbookImpl book );
    #endregion
  }
}

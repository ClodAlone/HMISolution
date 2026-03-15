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
using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents single list object.
  /// </summary>
  public interface IListObject
  {
    /// <summary>
    /// Gets or sets name of the list object.
    /// </summary>
    string Name
    {
      get;
      set;
    }
    /// <summary>
    /// Gets or sets list object's location.
    /// </summary>
    IRange Location
    {
      get;
      set;
    }
    /// <summary>
    /// Gets collection of all columns of the list object.
    /// </summary>
    IList<IListObjectColumn> Columns
    {
      get;
    }
    /// <summary>
    /// Gets index of the current list object.
    /// </summary>
    int Index
    {
      get;
    }
    /// <summary>
    /// Gets or sets the built-in table style for the specified ListObject object.
    /// </summary>
    TableBuiltInStyles BuiltInTableStyle
    {
      get;
      set;
    }
    /// <summary>
    /// Gets parent worksheet object.
    /// </summary>
    IWorksheet Worksheet
    {
      get;
    }
    /// <summary>
    /// Gets or sets list object name.
    /// </summary>
    string DisplayName
    {
      get;
      set;
    }
    /// <summary>
    /// Gets number of totals rows.
    /// </summary>
    int TotalsRowCount
    {
      get;
    }
    /// <summary>
    /// Gets or sets a value indicating whether the Total row is visible.
    /// </summary>
    bool ShowTotals
    {
      get;
      set;
    }
    /// <summary>
    /// Gets or sets a value indicating whether row stripes should be present.
    /// </summary>
    bool ShowTableStyleRowStripes
    {
      get;
      set;
    }
    /// <summary>
    /// Gets or sets a value indicating whether column stripes should be present.
    /// </summary>
    bool ShowTableStyleColumnStripes
    {
      get;
      set;
    }
    /// <summary>
    /// Gets or sets a value indicating whether last column is present.
    /// </summary>    
    bool ShowLastColumn
    {
        get;
        set;
    }
    /// <summary>
    /// Gets or sets a value indicating whether first column is present.
    /// </summary>
    bool ShowFirstColumn
    {
        get;
        set;
    }
    bool ShowHeaderRow
    {
        get;
        set;
    }
    QueryTableImpl QueryTable { get; }
    ExcelTableType TableType { get; }
      #if !SILVERLIGHT && !WINRT && !WP
    void Refresh();
#endif
  }
}

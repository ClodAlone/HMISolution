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

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents single column in the list object.
  /// </summary>
  public interface IListObjectColumn
  {
    /// <summary>
    /// Gets or sets name of the column.
    /// </summary>
    string Name
    {
      get;
      set;
    }
    /// <summary>
    /// Gets column index.
    /// </summary>
    int Index
    {
      get;
    }
    int Id
    {
      get;
    }
    /// <summary>
    /// Gets or sets function used for totals calculation.
    /// </summary>
    ExcelTotalsCalculation TotalsCalculation
    {
      get;
      set;
    }
    /// <summary>
    /// Gets or sets label of the totals row.
    /// </summary>
    string TotalsRowLabel
    {
      get;
      set;
    }
    /// <summary>
    /// Gets or sets calculated formula value.
    /// </summary>
    string CalculatedFormula
    {
      get;
      set;
    }
    int QueryTableFieldId
    {
        get;
    }
  }
}

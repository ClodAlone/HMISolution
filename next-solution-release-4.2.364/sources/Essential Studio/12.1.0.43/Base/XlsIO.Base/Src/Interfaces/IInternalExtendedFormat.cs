#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.XlsIO.Interfaces
{
	/// <summary>
	/// Summary description for IInternalExtendedFormat.
	/// </summary>
	public interface IInternalExtendedFormat : IExtendedFormat
	{
    #region Border properties
    /// <summary>
    /// Get/set BottomBorder color.
    /// </summary>
    ColorObject  BottomBorderColor { get; }
    /// <summary>
    /// Get/set TopBorder color.
    /// </summary>
    ColorObject  TopBorderColor { get; }
    /// <summary>
    /// Get/set LeftBorder color.
    /// </summary>
    ColorObject  LeftBorderColor { get; }
    /// <summary>
    /// Get/set RightBorder color.
    /// </summary>
    ColorObject  RightBorderColor { get; }
    /// <summary>
    /// Get/set DiagonalUpBorder color.
    /// </summary>
    ColorObject  DiagonalBorderColor { get; }
    /// <summary>
    /// Gets / sets line style of the left border.
    /// </summary>
    ExcelLineStyle LeftBorderLineStyle { get; set; }
    /// <summary>
    /// Gets / sets line style of the right border.
    /// </summary>
    ExcelLineStyle RightBorderLineStyle { get; set; }
    /// <summary>
    /// Gets / sets line style of the top border.
    /// </summary>
    ExcelLineStyle TopBorderLineStyle { get; set; }
    /// <summary>
    /// Gets / sets line style of the bottom border.
    /// </summary>
    ExcelLineStyle BottomBorderLineStyle { get; set; }
    /// <summary>
    /// Gets / sets line style of the diagonal border.
    /// </summary>
    ExcelLineStyle DiagonalUpBorderLineStyle { get; set; }
    /// <summary>
    /// Gets / sets line style of the diagonal border.
    /// </summary>
    ExcelLineStyle DiagonalDownBorderLineStyle { get; set; }
    /// <summary>
    /// Indicates whether DiagonalUp line is visible.
    /// </summary>
    bool DiagonalUpVisible { get; set; }
    /// <summary>
    /// Indicates whether DiagonalDown line is visible.
    /// </summary>
    bool DiagonalDownVisible { get; set; }

    #endregion

    #region Other properties
    /// <summary>
    /// Parent workbook.
    /// </summary>
    WorkbookImpl Workbook { get; }
    #endregion

    #region Methods
    /// <summary>
    /// Starts updating process.
    /// </summary>
    void BeginUpdate();
    /// <summary>
    /// Ends updating process.
    /// </summary>
    void EndUpdate();
    #endregion
  }
}

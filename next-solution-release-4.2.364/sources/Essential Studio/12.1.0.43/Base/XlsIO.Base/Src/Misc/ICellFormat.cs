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

#region file using directives
using System;
using System.Collections;
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents the search criteria for the cell format.
  /// </summary>
  public interface ICellFormat
  {
    #region Not supported methods/properties
#if NOT_SUPPORTED
/*
    XlCreator Creator { get; }
    Interior Interior { get; set; }
*/
#endif
    #endregion

    #region Skipped
#if SKIPPED
/*
    /// <summary>
    /// True if the range or style contains merged cells. Read / write Variant.
    /// </summary>
    object        MergeCells { get; set; }
    /// <summary>
    /// Clears the criterias set in the FindFormat and ReplaceFormat properties.
    /// </summary>
    void Clear();
*/
#endif
    #endregion

    #region Interface properties
    /// <summary>
    /// True if text is automatically indented when the text alignment
    /// in a cell is set to equal distribution either horizontally or
    /// vertically. Read / write Variant.
    /// </summary>
    object        AddIndent { get; set; }
    /// <summary>
    /// This property returns an Application object that represents the Microsoft
    /// Excel application.
    /// </summary>
    IApplication  Application { get; }
    /// <summary>
    /// Allows the user to set or return the search criteria based on the
    /// cell's border format.
    /// </summary>
    IBorders      Borders { get; set; }
    /// <summary>
    /// Returns a Font object, allowing the user to set or return the search
    /// criteria based on the cell's font format.
    /// </summary>
    IFont         Font { get; set; }
    /// <summary>
    /// True if the formula will be hidden when the worksheet is protected.
    /// Returns NULL if the specified range contains some cells with
    /// FormulaHidden equal to True and some cells with FormulaHidden equal
    /// to False. Read/Write Variant.
    /// </summary>
    object        FormulaHidden { get; set; }
    /// <summary>
    /// Returns or sets the horizontal alignment for the specified object.
    /// Read / write Variant.
    /// </summary>
    ExcelHAlign      HorizontalAlignment { get; set; }
    /// <summary>
    /// Returns or sets the indent level for the cell or range. Can be an integer
    /// from 0 to 15 for Excel 97-2003 and 250 for Excel 2007. Read / write integer.
    /// </summary>
    int          IndentLevel { get; set; }
    /// <summary>
    /// True if the object is locked, False if the object can be modified
    /// when the sheet is protected. Returns NULL if the specified range
    /// contains both locked and unlocked cells. Read / write Variant.
    /// </summary>
    object        Locked { get; set; }
    /// <summary>
    /// Returns or sets the format code for the object. Returns NULL if all
    /// cells in the specified range don't have the same number format.
    /// Read / write Variant.
    /// </summary>
    string        NumberFormat { get; set; }
    /// <summary>
    /// Returns or sets the format code for the object as a string in the
    /// language of the user. Read / write Variant.
    /// </summary>
    string        NumberFormatLocal { get; set; }
    /// <summary>
    /// The text orientation. Can be an integer value from 0 to 90
    /// degrees. Read / write Variant.
    /// </summary>
    int           Orientation { get; set; }
    /// <summary>
    /// Returns the parent object for the specified object.
    /// </summary>
    object        Parent { get; }
    /// <summary>
    /// True if text automatically shrinks to fit in the available column
    /// width. Returns NULL if this property isn't set to the same value
    /// for all cells in the specified range. Read / write Variant.
    /// </summary>
    object        ShrinkToFit { get; set; }
    /// <summary>
    /// Returns or sets the vertical alignment of the specified object.
    /// Read / write Variant.
    /// </summary>
    object        VerticalAlignment { get; set; }
    /// <summary>
    /// True if Excel wraps the text in the object. Returns NULL
    /// if the specified range contains some cells that wrap text and other
    /// cells that don't. Read / write Variant.
    /// </summary>
    object        WrapText { get; set; }
    #endregion

    #region Interface methods
    #endregion
  }
}

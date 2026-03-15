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
  /// A collection of all the Worksheet objects in the specified or
  /// active workbook. Each Worksheet object represents a worksheet.
  /// </summary>
  public interface IWorksheets : IEnumerable
  {
    #region Not supported methods/properties
#if NOT_SUPPORTED
/*
    XlCreator Creator { get; }
    object _Default { get; }

    void _PrintOut(object From, object To, object Copies, object Preview, object ActivePrinter, object PrintToFile, object Collate);
    void PrintOut(object From, object To, object Copies, object Preview, object ActivePrinter, object PrintToFile, object Collate, object PrToFileName);
    void PrintPreview(object EnableChanges);
    void FillAcrossSheets(Excel.Range Range, Excel.XlFillWith Type);
    void Move(object Before, object After);
    /// <summary>
    /// Determines whether the object is visible. Read / write Variant.
    /// </summary>
    object Visible { get; set; }
*/
#endif
    #endregion

    #region Skipped
#if SKIPPED
/*
    /// <summary>
    /// Creates a new worksheet, chart, or macro sheet. The new worksheet
    /// becomes the active sheet.
    /// </summary>
    /// <param name="Before"></param>
    /// <param name="After"></param>
    /// <param name="Count"></param>
    /// <param name="Type"></param>
    /// <returns></returns>
    IWorksheet Add( object Before, object After, object Count, object Type );
    /// <summary>
    /// Copies the sheet to another location in the workbook.
    /// </summary>
    /// <param name="Before"></param>
    /// <param name="After"></param>
    void Copy( object Before, object After );
    /// <summary>
    /// Deletes the object.
    /// </summary>
    void Delete();
    /// <summary>
    /// Selects the object.
    /// </summary>
    /// <param name="Replace"></param>
    void Select( object Replace );
*/
#endif
    #endregion

    #region Interface properties
    /// <summary>
    /// Used without an object qualifier, this property returns an
    /// Application object that represents the Microsoft Excel application.
    /// </summary>
    IApplication  Application { get; }
    /// <summary>
    /// Returns the number of objects in the collection. Read-only Long.
    /// </summary>
    int           Count { get; }
//    /// <summary>
//    /// Returns an HPageBreaks collection that represents the horizontal
//    /// page breaks on the sheet. Read-only.
//    /// </summary>
//    IHPageBreaks  HPageBreaks { get; }
    /// <summary>
    /// Returns a single object from a collection.
    /// </summary>
    IWorksheet    this[ int Index ] { get; }
    /// <summary>
    /// Returns a single object from a collection.
    /// </summary>
    IWorksheet    this[ string sheetName ] { get; }
    /// <summary>
    /// Returns the parent object for the specified object.
    /// </summary>
    object        Parent { get; }
//    /// <summary>
//    /// Returns a VPageBreaks collection that represents the vertical page
//    /// breaks on the sheet. Read-only.
//    /// </summary>
//    IVPageBreaks  VPageBreaks { get; }
    /// <summary>
    /// Indicates whether all created range objects should be cached.
    /// </summary>
    bool UseRangesCache { get; set; }
    #endregion

    #region interface methods
    /// <summary>
    /// Add a copy of the specified worksheet to the worksheet collection.
    /// </summary>
    /// <param name="sheetIndex">Index of the workbook that should be copied</param>
    /// <returns></returns>
    IWorksheet AddCopy( int sheetIndex );
    /// <summary>
    /// Add a copy of the specified worksheet to the worksheet collection.
    /// </summary>
    /// <param name="sheetIndex">Index of the workbook that should be copied</param>
    /// <param name="flags">Represents copy options flags.</param>
    /// <returns>Returns copied sheet.</returns>
    IWorksheet AddCopy( int sheetIndex, ExcelWorksheetCopyFlags flags );
    /// <summary>
    /// Add a copy of the specified worksheet to the worksheet collection.
    /// </summary>
    /// <param name="sourceSheet">
    /// Worksheet to add. It is not necessary; should be worksheet
    /// from this collection.
    /// </param>
    /// <returns>Added worksheet.</returns>
    IWorksheet AddCopy( IWorksheet sourceSheet );
    /// <summary>
    /// Adds copy of worksheet.
    /// </summary>
    /// <param name="sheet">Worksheet to copy.</param>
    /// <param name="flags">Represents copy flags.</param>
    /// <returns>Copy of worksheet that was added.</returns>
    IWorksheet AddCopy( IWorksheet sheet, ExcelWorksheetCopyFlags flags );
    /// <summary>
    /// Add a copy of the specified worksheets to the worksheet collection.
    /// </summary>
    /// <param name="worksheets">Worksheets to add.</param>
    void       AddCopy( IWorksheets worksheets );
    /// <summary>
    /// Adding worksheets collection to current workbook.
    /// </summary>
    /// <param name="worksheets">Source worksheets collection.</param>
    /// <param name="flags">Represents copy option flags.</param>
    void AddCopy( IWorksheets worksheets, ExcelWorksheetCopyFlags flags );
    /// <summary>
    /// Create worksheet with specified name.
    /// </summary>
    /// <param name="name">New name of worksheet. Must be unique for collection.</param>
    /// <returns>Reference on created worksheet.</returns>
    IWorksheet Create( string name );
    /// <summary>
    /// Create a new worksheet.
    /// </summary>
    /// <returns>Reference on created worksheet.</returns>
    IWorksheet Create();
    /// <summary>
    /// Remove worksheet from collection.
    /// </summary>
    /// <param name="sheet">Reference on worksheet to remove.</param>
    void Remove( IWorksheet sheet );
    /// <summary>
    /// Removes specified worksheet from the collection.
    /// </summary>
    /// <param name="sheetName">Name of the sheet to remove.</param>
    void Remove( string sheetName );
    /// <summary>
    /// Removes specified worksheet from the collection.
    /// </summary>
    /// <param name="index">Index of the sheet to remove.</param>
    void Remove( int index );
    /// <summary>
    /// This method searches for the first cell with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    IRange FindFirst( string findValue, ExcelFindType flags );
    /// <summary>
    /// This method searches for the first cell with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <param name="findOptions">Way to search.</param>
    /// <returns>
    /// First found cell, or Null if value was not found.
    /// </returns>
    IRange FindFirst(string findValue, ExcelFindType flags, ExcelFindOptions findOptions);
    /// <summary>
    /// This method searches for the first cell with specified double value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    IRange FindFirst( double findValue, ExcelFindType flags );
    /// <summary>
    /// This method searches for the first cell with specified bool value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    IRange FindFirst( bool findValue );
    /// <summary>
    /// This method searches for the first cell with specified DateTime value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    IRange FindFirst( DateTime findValue );
    /// <summary>
    /// This method searches for the first cell with specified TimeSpan value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    IRange FindFirst( TimeSpan findValue );
    /// <summary>
    /// This method searches for the all cells with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    IRange[] FindAll( string findValue, ExcelFindType flags );
    /// <summary>
    /// This method searches for the all cells with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <param name="findOptions">Way to search.</param>
    /// <returns>
    /// All found cells, or Null if value was not found.
    /// </returns>
    IRange[] FindAll(string findValue, ExcelFindType flags, ExcelFindOptions findOptions);
    ///<summary>
    /// This method searches for the all cells with specified double value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    IRange[] FindAll( double findValue, ExcelFindType flags );
    /// <summary>
    /// This method searches for the all cells with specified bool value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found</returns>
    IRange[] FindAll( bool findValue );
    /// <summary>
    /// This method searches for the all cells with specified DateTime value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    IRange[] FindAll( DateTime findValue );
    /// <summary>
    /// This method searches for the all cells with specified TimeSpan value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    IRange[] FindAll( TimeSpan findValue );
    /// <summary>
    /// Adds copy of sheet to collection before chosen sheet.
    /// </summary>
    /// <param name="toCopy">Represents worksheet to copy.</param>
    /// <returns>Returns copied sheet.</returns>
    IWorksheet AddCopyBefore( IWorksheet toCopy );
    /// <summary>
    /// Adds copy of sheet to collection before chosen sheet.
    /// </summary>
    /// <param name="toCopy">Represents worksheet to copy.</param>
    /// <param name="sheetAfter">Represents sheet that, in collection must be after copied sheet.</param>
    /// <returns>Returns copied sheet.</returns>
    IWorksheet AddCopyBefore( IWorksheet toCopy, IWorksheet sheetAfter );
    /// <summary>
    /// Adds copy of sheet to collection after chosen sheet.
    /// </summary>
    /// <param name="toCopy">Represents worksheet to copy.</param>
    /// <returns>Returns copied sheet.</returns>
    IWorksheet AddCopyAfter( IWorksheet toCopy );
    /// <summary>
    /// Adds copy of sheet to collection before chosen sheet.
    /// </summary>
    /// <param name="toCopy">Represents worksheet to copy.</param>
    /// <param name="sheetBefore">Represents sheet that, in collection must be before copied sheet.</param>
    /// <returns>Returns copied sheet.</returns>
    IWorksheet AddCopyAfter( IWorksheet toCopy, IWorksheet sheetBefore );
    #endregion
  }
}

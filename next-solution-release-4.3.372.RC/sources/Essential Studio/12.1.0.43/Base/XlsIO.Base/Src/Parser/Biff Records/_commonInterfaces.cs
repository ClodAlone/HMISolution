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

using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Interfaces;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// 
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public interface ICellPositionFormat
  {
    /// <summary>
    /// Row zero-based index.
    /// </summary>
    int Row{ get; set; }
    /// <summary>
    /// Column zero-based index.
    /// </summary>
    int Column{ get; set; }
    /// <summary>
    /// Index of extended format.
    /// </summary>
    ushort ExtendedFormatIndex{ get; set; }
    /// <summary>
    /// Returns type code. Read-only.
    /// </summary>
    TBIFFRecord TypeCode { get; }
  }
  /// <summary>
  /// 
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public interface IMultiCellRecord : ICellPositionFormat
  {
    /// <summary>
    /// Zero-based index of the first column.
    /// </summary>
    int FirstColumn { get; set; }
    /// <summary>
    /// Zero-based index of the last column.
    /// </summary>
    int LastColumn { get; set; }
    /// <summary>
    /// Returns size of the sub record. Read-only.
    /// </summary>
    int SubRecordSize { get; }
    /// <summary>
    /// Returns size of the subrecord if it was placed as separate record (including BiffRecord header). Read-only.
    /// </summary>
    int GetSeparateSubRecordSize( ExcelVersion version );
    /// <summary>
    /// Returns type of the subrecord. Read-only.
    /// </summary>
    TBIFFRecord SubRecordType { get; }
    /// <summary>
    /// Inserts cell inside this record.
    /// </summary>
    /// <param name="cell">Cell to insert.</param>
    void Insert( ICellPositionFormat cell );
    /// <summary>
    /// Removes information about specified column from the record and splits record into two.
    /// </summary>\
    /// <param name="iColumnIndex">Zero-based index of the column to remove.</param>
    /// <returns>Split records.</returns>
    ICellPositionFormat[] Split( int iColumnIndex );
    /// <summary>
    /// Splits record into subrecords.
    /// </summary>
    /// <param name="bIgnoreStyles">Indicates whether styles must be ignored.</param>
    /// <returns>Array with all subrecords.</returns>
    BiffRecordRaw[] Split( bool bIgnoreStyles );
  }
  /// <summary>
  /// Contains outline information about.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public interface IOutline
  {
    /// <summary>
    /// Outline level.
    /// </summary>
    ushort  OutlineLevel{ get; set; }
    /// <summary>
    /// Indicates whether object is collapsed.
    /// </summary>
    bool    IsCollapsed{ get; set; }
    /// <summary>
    /// Indicates whether object is hidden.
    /// </summary>
    bool    IsHidden{ get; set; }
    /// <summary>
    /// Index of extended format.
    /// </summary>
    ushort  ExtendedFormatIndex { get; set; }
    /// <summary>
    /// Row or column index.
    /// </summary>
    ushort  Index { get; set; }
  }
  /// <summary>
  /// Contains outline information about.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [CLSCompliant(false)]
  public interface IOutlineWrapper : IOutline
  {
      /// <summary>
      /// First Index of the row/column.
      /// </summary>
      int FirstIndex { get; set; }
      /// <summary>
      /// Last index of the row/column.
      /// </summary>
      int LastIndex { get; set; }
      /// <summary>
      /// Indicates the Outline object
      /// </summary>
      IOutline Outline { get; set; }
      /// <summary>
      /// Indicates the Grouped range
      /// </summary>
      IRange OutlineRange { get; set; }
      /// <summary>
      /// Indicates the ExcelGroupBy type
      /// </summary>
      ExcelGroupBy GroupBy{ get; set; }
  }
  /// <summary>
  /// Contains information about chart type.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public interface IChartType
  {
    /// <summary>
    /// Indicates whether values should be stacked.
    /// </summary>
    bool StackValues { get; set; }
    /// <summary>
    /// Indicates whether values should be shown as percents.
    /// </summary>
    bool ShowAsPercents { get; set; }
  }

  /// <summary>
  /// Interface for shared formula and array formula.
  /// </summary>
  [ CLSCompliant( false ) ]
  public interface ISharedFormula
  {
    /// <summary>
    /// Index to first row of the array formula range.
    /// </summary>
    int FirstRow { get; set; }

    /// <summary>
    /// Index to last row of the array formula range.
    /// </summary>
    int LastRow { get; set; }

    /// <summary>
    /// Index to first column of the array formula range.
    /// </summary>
    int   FirstColumn { get; set; }
    /// <summary>
    /// Index to last column of the array formula range.
    /// </summary>
    int   LastColumn { get; set; }
    /// <summary>
    /// Parsed formula.
    /// </summary>
    Ptg[]  Formula { get; }
  }
  /// <summary>
  /// Interface for records that have Value property.
  /// </summary>
  public interface IValueHolder
  {
    /// <summary>
    /// Value of the record.
    /// </summary>
    object Value{ get; set; }
  }

  /// <summary>
  /// This interface supports DoubleValue property.
  /// </summary>
  public interface IDoubleValue
  {
    /// <summary>
    /// Returns double value. Read-only.
    /// </summary>
    double DoubleValue { get; }
    /// <summary>
    /// Returns type code. Read-only.
    /// </summary>
    TBIFFRecord TypeCode { get; }
  }
  /// <summary>
  /// This interface supports StringValue property.
  /// </summary>
  public interface IStringValue
  {
    /// <summary>
    /// Returns string value. Read-only.
    /// </summary>
    string StringValue { get; }
  }
}

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

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Constants
{
  /// <summary>
  /// Defines constants required for protection parsing/serialization in Excel 2007 format.
  /// </summary>
  sealed class Protection
  {
    #region Constants
    /// <summary>
    /// This collection expresses the sheet protection options to enforce when the sheet is protected.
    /// </summary>
    public const string SheetProtectionTag = "sheetProtection";
    /// <summary>
    /// Specifies the hash of the password required for editing this worksheet.
    /// This protection is optional and may be ignored by applications that choose not
    /// to support this functionality. The hash is generated from an 8-bit wide character.
    /// </summary>
    public const string PasswordAttribute = "password";
    public const string ContentAttribute = "content";
    /// <summary>
    /// Objects are locked when the sheet is protected.
    /// </summary>
    public const string ObjectsAttribute = "objects";
    /// <summary>
    /// Scenarios are locked when the sheet is protected.
    /// </summary>
    public const string ScenariosAttribute = "scenarios";
    /// <summary>
    /// Formatting cells is locked when the sheet is protected.
    /// </summary>
    public const string FormatCellsAttribute = "formatCells";
    /// <summary>
    /// Formatting columns is locked when the sheet is protected.
    /// </summary>
    public const string FormatColumnsAttribute = "formatColumns";
    /// <summary>
    /// Formatting rows is locked when the sheet is protected.
    /// </summary>
    public const string FormatRowsAttribute = "formatRows";
    /// <summary>
    /// Inserting columns is locked when the sheet is protected.
    /// </summary>
    public const string InsertColumnsAttribute = "insertColumns";
    /// <summary>
    /// Inserting rows is locked when the sheet is protected.
    /// </summary>
    public const string InsertRowsAttribute = "insertRows";
    /// <summary>
    /// Inserting hyperlinks is locked when the sheet is protected.
    /// </summary>
    public const string InsertHyperlinksAttribute = "insertHyperlinks";
    /// <summary>
    /// Deleting columns is locked when the sheet is protected.
    /// </summary>
    public const string DeleteColumnsAttribute = "deleteColumns";
    /// <summary>
    /// Deleting rows is locked when the sheet is protected.
    /// </summary>
    public const string DeleteRowsAttribute = "deleteRows";
    /// <summary>
    /// Selection of locked cells is locked when the sheet is protected.
    /// </summary>
    public const string SelectLockedCells = "selectLockedCells";
    /// <summary>
    /// Sorting is locked when the sheet is protected.
    /// </summary>
    public const string SortAttribute = "sort";
    /// <summary>
    /// Autofilters are locked when the sheet is protected.
    /// </summary>
    public const string AutoFilterAttribute = "autoFilter";
    /// <summary>
    /// Selection of unlocked cells is locked when the sheet is protected.
    /// </summary>
    public const string SelectUnlockedCells = "selectUnlockedCells";
    /// <summary>
    /// Pivot tables are locked when the sheet is protected.
    /// </summary>
    public const string PivotTablesAttribute = "pivotTables";
    /// <summary>
    /// Sheet is locked when the sheet is protected.
    /// </summary>
    public const string SheetAttribute = "sheet";
    /// <summary>
    /// List of protection attribute names in the resulting file.
    /// </summary>
    public static readonly string[] ChartProtectionAttributes = new string[]
    {
      Protection.ContentAttribute,
      Protection.ObjectsAttribute,
    };
    /// <summary>
    /// List of default values for protection options (order corresponds to ProtectionFlags and ProtectionAttributes).
    /// </summary>
    public static readonly bool[] ChartDefaultValues = new bool[]
    {
      false,
      false,
    };
    /// <summary>
    /// List of protection attribute names in the resulting file.
    /// </summary>
    public static readonly string[] ProtectionAttributes = new string[]
    {
      Protection.SheetAttribute,
      Protection.ObjectsAttribute,
      Protection.ScenariosAttribute,
      Protection.FormatCellsAttribute,
      Protection.FormatColumnsAttribute,
      Protection.FormatRowsAttribute,
      Protection.InsertColumnsAttribute,
      Protection.InsertRowsAttribute,
      Protection.InsertHyperlinksAttribute,
      Protection.DeleteColumnsAttribute,
      Protection.DeleteRowsAttribute,
      Protection.SelectLockedCells,
      Protection.SortAttribute,
      Protection.AutoFilterAttribute,
      Protection.PivotTablesAttribute,
      Protection.SelectUnlockedCells,
    };
    /// <summary>
    /// Protection flags that correspond to ProtectionAttributes (order must be the same).
    /// </summary>
    public static readonly ExcelSheetProtection[] ProtectionFlags = new ExcelSheetProtection[]
    {
      ExcelSheetProtection.Content,
      ExcelSheetProtection.Objects,
      ExcelSheetProtection.Scenarios,
      ExcelSheetProtection.FormattingCells,
      ExcelSheetProtection.FormattingColumns,
      ExcelSheetProtection.FormattingRows,
      ExcelSheetProtection.InsertingColumns,
      ExcelSheetProtection.InsertingRows,
      ExcelSheetProtection.InsertingHyperlinks,
      ExcelSheetProtection.DeletingColumns, 
      ExcelSheetProtection.DeletingRows,
      ExcelSheetProtection.LockedCells,
      ExcelSheetProtection.Sorting,
      ExcelSheetProtection.Filtering,
      ExcelSheetProtection.UsingPivotTables,
      ExcelSheetProtection.UnLockedCells,
    };
    /// <summary>
    /// List of default values for protection options (order corresponds to ProtectionFlags and ProtectionAttributes).
    /// </summary>
    public static readonly bool[] DefaultValues = new bool[]
    {
      false,
      false,
      false,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      false,
      true,
      true,
      false,
      false,
    };
    /// <summary>
    /// This element specifies options for protecting data in the workbook. Applications
    /// may use workbook protection to prevent anyone from accidentally changing, moving,
    /// or deleting important data.
    /// </summary>
    public const string WorkbookProtectionTag = "workbookProtection";
    /// <summary>
    /// Specifies a boolean value that indicates whether structure of workbook is locked.
    /// </summary>
    public const string LockStructureTag = "lockStructure";
    /// <summary>
    /// Specifies a boolean value that indicates whether the windows that comprise the workbook are locked.
    /// </summary>
    public const string LockWindowsTag = "lockWindows";
    /// <summary>
    /// Specifies the hash of the password required for unlocking this workbook.
    /// </summary>
    public const string WorkbookPassword = "workbookPassword";
    #endregion

    #region Methods
    /// <summary>
    /// Prevents a default instance of the Protection class from being created.
    /// </summary>
    private Protection()
    {
    }
    #endregion
  }
}

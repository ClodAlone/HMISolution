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

using Syncfusion.XlsIO.Implementation;
#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif
#if  SILVERLIGHT
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents a cell, row, column, selection of cells
  /// containing one or more contiguous blocks of cells,
  /// or a 3-D range.
  /// </summary>
  public interface IRange :
    IParentApplication,
    IEnumerable
    //, IDisposable
  {
    #region Not supported properties/methods
#if NOT_SUPPORTED
/*
    object _Default { get; set; }
    XlCreator Creator { get; }
    /// <summary>
    /// If the specified cell is part of an array, it returns a Range object
    /// that represents the entire array. Read-only.
    /// </summary>
    Range         CurrentArray { get; }
    /// <summary>
    /// Returns a Range object that represents the current region. The
    /// current region is a range bound by any combination of blank rows
    /// and blank columns. Read-only.
    /// </summary>
    Range CurrentRegion { get; }
    Range Dependents { get; }
    Range DirectDependents { get; }
    Range DirectPrecedents { get; }
    Errors Errors { get; }
    FormatConditions FormatConditions { get; }
    Hyperlinks Hyperlinks { get; }
    string ID { get; set; }
    Interior Interior { get; }
    int ListHeaderRows { get; }
    XlLocationInTable LocationInTable { get; }
    object Locked { get; set; }
    /// <summary>
    ///
    /// </summary>
    object OutlineLevel { get; set; }
    int PageBreak { get; set; }
    Phonetic Phonetic { get; }
    Phonetics Phonetics { get; }
    PivotCell PivotCell { get; }
    PivotField PivotField { get; }
    PivotItem PivotItem { get; }
    PivotTable PivotTable { get; }
    Range Precedents { get; }
    object PrefixCharacter { get; }
    QueryTable QueryTable { get; }
    Range Range { get; }
    int ReadingOrder { get; set; }
    /// <summary>
    /// True if the outline is expanded for the specified range (so
    /// that the detail of the column or row is visible). The specified
    /// range must be a single summary column or row in an outline.
    /// Read/write Variant.
    /// </summary>
    object        ShowDetail { get; set; }
    SmartTags SmartTags { get; }
    SoundNote SoundNote { get; }
    Validation Validation { get; }

    object _PasteSpecial(Excel.XlPasteType Paste, Excel.XlPasteSpecialOperation Operation, object SkipBlanks, object Transpose);
    object _PrintOut(object From, object To, object Copies, object Preview, object ActivePrinter, object PrintToFile, object Collate);
    object AdvancedFilter(Excel.XlFilterAction Action, object CriteriaRange, object CopyToRange, object Unique);
    object ApplyNames(object Names, object IgnoreRelativeAbsolute, object UseRowColumnNames, object OmitColumn, object OmitRow, Excel.XlApplyNamesOrder Order, object AppendLast);
    object ApplyOutlineStyles();
    string AutoComplete(string String);
    object AutoFill(Excel.Range Destination, Excel.XlAutoFillType Type);
    object AutoFilter(object Field, object Criteria1, Excel.XlAutoFilterOperator Operator, object Criteria2, object VisibleDropDown);
    object AutoFit();
    object AutoFormat(Excel.XlRangeAutoFormat Format, object Number, object Font, object Alignment, object Border, object Pattern, object Width);
    object AutoOutline();
    object Calculate();
    object CheckSpelling(object CustomDictionary, object IgnoreUppercase, object AlwaysSuggest, object SpellLang);
    object ClearNotes();
    object ClearOutline();
    Excel.Range ColumnDifferences(object Comparison);
    object Consolidate(object Sources, object Function, object TopRow, object LeftColumn, object CreateLinks);
    int CopyFromRecordset(object Data, object MaxRows, object MaxColumns);
    object CopyPicture(Excel.XlPictureAppearance Appearance, Excel.XlCopyPictureFormat Format);
    object CreateNames(object Top, object Left, object Bottom, object Right);
    object CreatePublisher(object Edition, Excel.XlPictureAppearance Appearance, object ContainsPICT, object ContainsBIFF, object ContainsRTF, object ContainsVALU);
    object DataSeries(object Rowcol, Excel.XlDataSeriesType Type, Excel.XlDataSeriesDate Date, object Step, object Stop, object Trend);
    object DialogBox();
    void Dirty();
    object EditionOptions(Excel.XlEditionType Type, Excel.XlEditionOptionsOption Option, object Name, object Reference, Excel.XlPictureAppearance Appearance, Excel.XlPictureAppearance ChartSize, object Format);
    object FillDown();
    object FillLeft();
    object FillRight();
    object FillUp();
    Excel.Range Find(object What, object After, object LookIn, object LookAt, object SearchOrder, Excel.XlSearchDirection SearchDirection, object MatchCase, object MatchByte, object SearchFormat);
    Excel.Range FindNext(object After);
    Excel.Range FindPrevious(object After);
    object FunctionWizard();
    bool GoalSeek(object Goal, Excel.Range ChangingCell);
    object Group(object Start, object End, object By, object Periods);
    object ListNames();
    object NavigateArrow(object TowardPrecedent, object ArrowNumber, object LinkNumber);
    string NoteText(object Text, object Start, object Length);
    object Parse(object ParseLine, object Destination);
    object PrintOut(object From, object To, object Copies, object Preview, object ActivePrinter, object PrintToFile, object Collate, object PrToFileName);
    object PrintPreview(object EnableChanges);
    object RemoveSubtotal();
    bool Replace(object What, object Replacement, object LookAt, object SearchOrder, object MatchCase, object MatchByte, object SearchFormat, object ReplaceFormat);
    Excel.Range RowDifferences(object Comparison);
    object Run(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    /// <summary>
    /// Selects the object
    /// </summary>
    /// <returns></returns>
    object Select();
    void SetPhonetic();
    object Show();
    object ShowDependents(object Remove);
    object ShowErrors();
    object ShowPrecedents(object Remove);
    object Sort(object Key1, Excel.XlSortOrder Order1, object Key2, object Type, Excel.XlSortOrder Order2, object Key3, Excel.XlSortOrder Order3, Excel.XlYesNoGuess Header, object OrderCustom, object MatchCase, Excel.XlSortOrientation Orientation, Excel.XlSortMethod SortMethod, Excel.XlSortDataOption DataOption1, Excel.XlSortDataOption DataOption2, Excel.XlSortDataOption DataOption3);
    object SortSpecial(Excel.XlSortMethod SortMethod, object Key1, Excel.XlSortOrder Order1, object Type, object Key2, Excel.XlSortOrder Order2, object Key3, Excel.XlSortOrder Order3, Excel.XlYesNoGuess Header, object OrderCustom, object MatchCase, Excel.XlSortOrientation Orientation, Excel.XlSortDataOption DataOption1, Excel.XlSortDataOption DataOption2, Excel.XlSortDataOption DataOption3);
    void Speak(object SpeakDirection, object SpeakFormulas);
    Excel.Range SpecialCells(Excel.XlCellType Type, object Value);
    object SubscribeTo(string Edition, Excel.XlSubscribeToFormat Format);
    object Subtotal(int GroupBy, Excel.XlConsolidationFunction Function, object TotalList, object Replace, object PageBreaks, Excel.XlSummaryRow SummaryBelowData);
    object Table(object RowInput, object ColumnInput);
    object TextToColumns(object Destination, Excel.XlTextParsingType DataType, Excel.XlTextQualifier TextQualifier, object ConsecutiveDelimiter, object Tab, object Semicolon, object Comma, object Space, object Other, object OtherChar, object FieldInfo, object DecimalSeparator, object ThousandsSeparator, object TrailingMinusNumbers);
*/
#endif
    #endregion

    #region Skipped
#if SKIPPED
/*
    /// <summary>
    /// Returns or sets the formula label type for the specified
    /// range. Can be xlNone if the range contains no labels, or one of
    /// the following ExcelFormulaLabel constants. Read/write ExcelFormulaLabel.
    /// </summary>
    ExcelFormulaLabel FormulaLabel { get; set; }
    /// <summary>
    /// True if the range can be edited on a protected worksheet.
    /// Read-only Boolean.
    /// </summary>
    bool          AllowEdit { get; }
    /// <summary>
    /// True if text is automatically indented when the text alignment in a
    /// cell is set to equal distribution either horizontally or vertically.
    /// Read/write Variant.
    /// </summary>
    object        AddIndent { get; set; }
    /// <summary>
    /// Returns or sets the name of the object. The name of a Range object
    /// is a Name object. For every other type of object, the name is a
    /// string. Read/write Variant.
    /// </summary>
    object        Name { get; set; }
    /// <summary>
    /// Returns a Characters object that represents a range of characters within
    /// the object text. You can use the Characters object to format characters
    /// within a text string.
    /// </summary>
    ICharacters   Characters { get; }
    /// <summary>
    /// Returns an Areas collection that represents all the ranges in a
    /// multiple-area selection. Read-only.
    /// </summary>
    IAreas        Areas { get; }
    /// <summary>
    /// Returns a Range object that represents the merged range
    /// containing the specified cell. If the specified cell isn't in
    /// a merged range, this property returns the specified cell.
    /// Read-only.
    /// </summary>
    IRange        MergeArea { get; }
    /// <summary>
    /// The text orientation. Can be an integer value.
    /// Read / write Variant.
    /// </summary>
    object        Orientation { get; set; }
    /// <summary>
    /// Returns a Range object that represents the columns in the specified range.
    /// Read-only.
    /// </summary>
    IRange        Columns { get; }
    /// <summary>
    /// Returns a Comment object that represents the comment associated with
    /// the cell in the upper-left corner of the range.
    /// Read-only Comment object.
    /// </summary>
    IComment      Comment { get; }
    /// <summary>
    /// Returns or sets the array formula of a range. Returns (or can be
    /// set to) a single formula or a Visual Basic array. If the specified
    /// range doesn't contain an array formula, this property returns NULL.
    /// Read/write Variant.
    /// </summary>
    object        FormulaArray { get; set; }
    /// <summary>
    /// Returns or sets the formula for the object, using A1-style
    /// references in the language of the user. Read/write Variant
    /// for Range objects, read / write String for Series objects.
    /// </summary>
    object        FormulaLocal { get; set; }
    /// <summary>
    /// Returns or sets the formula for the object, using R1C1-style
    /// notation in the language of the macro. Read/write Variant for
    /// Range objects, read / write String for Series objects.
    /// </summary>
    object        FormulaR1C1 { get; set; }
    /// <summary>
    /// Returns or sets the formula for the object, using R1C1-style
    /// notation in the language of the user. Read/write Variant for
    /// Range objects, read/write String for Series objects.
    /// </summary>
    object        FormulaR1C1Local { get; set; }
    /// <summary>
    /// True if the specified cell is part of an array formula.
    /// </summary>
    object        HasArray { get; }
    /// <summary>
    /// The height of the range. Read-only Variant.
    /// </summary>
    object        Height { get; }
    /// <summary>
    /// True if the rows or columns are hidden. The specified range must
    /// span an entire column or row. Read/write Variant.
    /// </summary>
    object        Hidden { get; set; }
    /// <summary>
    /// The distance from the left edge of column A to the left edge of the
    /// range. If the range is discontinuous, the first area is used. If the
    /// range is more than one column wide, the leftmost column in the range
    /// is used. Read-only Variant.
    /// </summary>
    object        Left { get; }
    /// <summary>
    /// True if the range or style contains merged cells. Read / write Variant.
    /// </summary>
    object        MergeCells { get; set; }
    /// <summary>
    /// Returns a Chart, Range, or Worksheet object that represents the
    /// next sheet or cell. Read-only.
    /// </summary>
    IRange        Next { get; }
    /// <summary>
    /// Returns or sets the format code for the object. Returns NULL
    /// if all cells in the specified range don't have the same number
    /// format. Read/write String.
    /// </summary>
    string        NumberFormat { get; set; }
    /// <summary>
    /// Returns or sets the format code for the object as a string in the
    /// language of the user. Read/write String.
    /// </summary>
    string        NumberFormatLocal { get; set; }
    /// <summary>
    /// Returns a Range object that represents a range that is offset from
    /// the specified range. Read-only.
    /// </summary>
    IRange        Offset { get; }
    /// <summary>
    /// Returns a Chart, Range, or Worksheet object that represents the
    /// previous sheet or cell. Read-only.
    /// </summary>
    IRange        Previous { get; }
    /// <summary>
    /// Resizes the specified range. Returns a Range object that represents
    /// the resized range.
    /// </summary>
    IRange        Resize { get; }
    /// <summary>
    /// True if the range is an outlining summary row or column. The range should
    /// be a row or a column. Read-only Variant.
    /// </summary>
    object        Summary { get; }
    /// <summary>
    /// Returns or sets the text for the specified object. Read-only
    /// String for the Range object, read / write String for all other objects.
    /// </summary>
    object        Text { get; }
    /// <summary>
    /// The distance from the top edge of row 1 to the top edge of
    /// the range. If the range is discontinuous, the first area is
    /// used. If the range is more than one row high, the top (lowest
    /// numbered) row in the range is used. Read-only Variant.
    /// </summary>
    object        Top { get; }
    /// <summary>
    /// True if the row height of the Range object equals the standard
    /// height of the sheet. Returns NULL if the range contains more
    /// than one row and the rows aren't all the same height.
    /// Read/write Variant.
    /// </summary>
    object        UseStandardHeight { get; set; }
    /// <summary>
    /// True if the column width of the Range object equals the standard
    /// width of the sheet. Returns Null if the range contains more than
    /// one column and the columns aren't all the same width.
    /// Read/write Variant.
    /// </summary>
    object        UseStandardWidth { get; set; }
    /// <summary>
    /// The width of the range. Read-only Variant.
    /// </summary>
    object        Width { get; }
    /// <summary>
    /// Adds a comment to the range.
    /// </summary>
    /// <param name="Text"></param>
    /// <returns></returns>
    IComment AddComment(object Text);
    /// <summary>
    /// Adds a border to a range and sets the Color, LineStyle, and Weight
    /// properties for the new border. Variant.
    /// </summary>
    /// <param name="LineStyle"></param>
    /// <param name="Weight"></param>
    /// <param name="ColorIndex"></param>
    /// <param name="Color"></param>
    /// <returns></returns>
    object BorderAround( object LineStyle, XlBorderWeight Weight,
      XlColorIndex ColorIndex, object Color );
    /// <summary>
    /// Clears the entire object
    /// </summary>
    /// <returns></returns>
    object  Clear();
    /// <summary>
    /// Clears all cell comments from the specified range.
    /// </summary>
    void    ClearComments();
    /// <summary>
    /// Clears the formulas from the range. Clears the data from a chart but leaves
    /// the formatting.
    /// </summary>
    /// <returns></returns>
    object  ClearContents();
    /// <summary>
    /// Clears the formatting of the object.
    /// </summary>
    /// <returns></returns>
    object  ClearFormats();
    /// <summary>
    /// Copies the range to the specified range or to the Clipboard.
    /// </summary>
    /// <param name="Destination"></param>
    /// <returns></returns>
    object  Copy(object Destination);
    /// <summary>
    /// Cuts the object to the Clipboard or pastes it into a specified destination.
    /// </summary>
    /// <param name="Destination"></param>
    /// <returns></returns>
    object  Cut(object Destination);
    /// <summary>
    /// Deletes the object.
    /// </summary>
    /// <param name="Shift"></param>
    /// <returns></returns>
    object  Delete(object Shift);
    /// <summary>
    /// Get enumerator of cells in range.
    /// </summary>
    /// <returns></returns>
    IEnumerator GetEnumerator();
    /// <summary>
    /// Inserts a cell or a range of cells into the worksheet or macro
    /// sheet and shifts other cells away to make space.
    /// </summary>
    /// <param name="Shift"></param>
    /// <param name="CopyOrigin"></param>
    /// <returns></returns>
    object  Insert(object Shift, object CopyOrigin);
    /// <summary>
    /// Adds an indent to the specified range.
    /// </summary>
    /// <param name="InsertAmount"></param>
    void    InsertIndent(int InsertAmount);
    /// <summary>
    /// Rearranges the text in a range so that it fills the range evenly.
    /// </summary>
    /// <returns></returns>
    object  Justify();
    /// <summary>
    /// Pastes a Range from the Clipboard into the specified range.
    /// </summary>
    /// <param name="Paste"></param>
    /// <param name="Operation"></param>
    /// <param name="SkipBlanks"></param>
    /// <param name="Transpose"></param>
    /// <returns></returns>
    object PasteSpecial( XlPasteType Paste,
      XlPasteSpecialOperation Operation, object SkipBlanks, object Transpose );
*/
#endif
    #endregion

    #region Interface properties
    /// <summary>
    /// Returns the range reference in the language of the macro.
    /// Read-only String.
    /// </summary>
    string        Address { get; }
    /// <summary>
    /// Returns the range reference for the specified range in the language
    /// of the user. Read-only String.
    /// </summary>
    string        AddressLocal { get; }
    /// <summary>
    /// Returns range Address in format "'Sheet1'!$A$1".
    /// </summary>
    string        AddressGlobal { get; }
    /// <summary>
    /// Returns the range reference using R1C1 notation.
    /// Read-only String.
    /// </summary>
    string        AddressR1C1 { get; }
    /// <summary>
    /// Returns the range reference using R1C1 notation.
    /// Read-only String.
    /// </summary>
    string        AddressR1C1Local { get; }
    /// <summary>
    /// Gets / sets boolean value that is contained by this range.
    /// </summary>
    bool          Boolean { get; set; }
    /// <summary>
    /// Returns a  Borders collection that represents the borders of a style
    /// or a range of cells (including a range defined as part of a
    /// conditional format).
    /// </summary>
    IBorders      Borders { get; }
    /// <summary>
    /// Returns a Range object that represents the cells in the specified range.
    /// Read-only.
    /// </summary>
    IRange[]      Cells { get; }
    /// <summary>
    /// Returns the number of the first column in the first area in the specified
    /// range. Read-only.
    /// </summary>
    int           Column { get; }
    /// <summary>
    /// Column group level. Read-only.
    /// -1 - Not all columns in the range have same group level.
    /// 0 - No grouping,
    /// 1 - 7 - Group level.
    /// </summary>
    int           ColumnGroupLevel { get; }
    /// <summary>
    /// Returns or sets the width of all columns in the specified range.
    /// Read/write Double.
    /// </summary>
    double        ColumnWidth { get; set; }
    /// <summary>
    /// Returns the number of objects in the collection. Read-only.
    /// </summary>
    int           Count { get; }
    /// <summary>
    /// Gets / sets DateTime contained by this cell. Read-write DateTime.
    /// </summary>
    DateTime      DateTime{ get; set; }
    /// <summary>
    /// Returns cell value after number format application. Read-only.
    /// </summary>
    string        DisplayText { get; }
    /// <summary>
    /// Returns a Range object that represents the cell at the end of the
    /// region that contains the source range.
    /// </summary>
    IRange        End { get; }
    /// <summary>
    /// Returns a Range object that represents the entire column (or
    /// columns) that contains the specified range. Read-only.
    /// </summary>
    IRange        EntireColumn { get; }
    /// <summary>
    /// Returns a Range object that represents the entire row (or
    /// rows) that contains the specified range. Read-only.
    /// </summary>
    IRange        EntireRow { get; }
    /// <summary>
    /// Gets / sets error value that is contained by this range.
    /// </summary>
    string        Error { get; set; }
    /// <summary>
    /// Returns or sets the object's formula in A1-style notation and in
    /// the language of the macro. Read/write Variant.
    /// </summary>
    string        Formula { get; set; }
    /// <summary>
    /// Represents array formula which can perform multiple calculations on one or more of the items in an array.
    /// </summary>
    string        FormulaArray { get; set; }
    /// <summary>
    /// Returns or sets the formula array for the range, using R1C1-style notation.
    /// </summary>
    string        FormulaArrayR1C1 { get; set; }
    /// <summary>
    /// True if the formula will be hidden when the worksheet is protected.
    /// False if at least part of formula in the range is not hidden.
    /// </summary>
    bool          FormulaHidden { get; set; }
    /// <summary>
    /// Get / set formula DateTime value contained by this cell.
    /// DateTime.MinValue if not all cells of the range have same DateTime value.
    /// </summary>
    DateTime      FormulaDateTime { get; set; }
    /// <summary>
    /// Returns or sets the formula for the range, using R1C1-style notation.
    /// </summary>
    string        FormulaR1C1 { get; set; }
    /// <summary>
    /// Returns the calculated value of the formula as a boolean.
    /// </summary>
    bool          FormulaBoolValue { get; set; }
    /// <summary>
    /// Returns the calculated value of the formula as a string.
    /// </summary>
    string        FormulaErrorValue { get; set; }
    /// <summary>
    /// Indicates whether specified range object has data validation.
    /// If Range is not single cell, then returns true only if all cells have data validation. Read-only.
    /// </summary>
    bool          HasDataValidation { get; }
    /// <summary>
    /// Indicates whether range contains bool value. Read-only.
    /// </summary>
    bool          HasBoolean { get; }
    /// <summary>
    /// Indicates whether range contains DateTime value. Read-only.
    /// </summary>
    bool          HasDateTime { get; }
    /// <summary>
    /// True if all cells in the range contain formulas; False if
    /// at least one of the cells in the range doesn't contain a formula.
    /// Read-only Boolean.
    /// </summary>
    bool          HasFormula { get; }
    /// <summary>
    /// Indicates whether range contains array-entered formula. Read-only.
    /// </summary>
    bool          HasFormulaArray { get; }
    /// <summary>
    /// Indicates whether the range contains number. Read-only.
    /// </summary>
    bool          HasNumber { get; }
    /// <summary>
    /// Indicates whether cell contains formatted rich text string.
    /// </summary>
    bool          HasRichText { get; }
    /// <summary>
    /// Indicates whether the range contains String. Read-only.
    /// </summary>
    bool          HasString { get; }
    /// <summary>
    /// Indicates whether range has default style. False means default style.
    /// Read-only.
    /// </summary>
    bool          HasStyle { get; }
    /// <summary>
    /// Returns or sets the horizontal alignment for the specified object.
    /// Read/write ExcelHAlign.
    /// </summary>
    ExcelHAlign   HorizontalAlignment { get; set; }
    /// <summary>
    /// Returns hyperlinks for this range. Read-only.
    /// </summary>
    IHyperLinks    Hyperlinks { get; }
    /// <summary>
    /// Returns or sets the indent level for the cell or range. Can be an integer
    /// from 0 to 15 for Excel 97-2003 and 250 for Excel 2007. Read/write Integer.
    /// </summary>
    int           IndentLevel { get; set; }
    /// <summary>
    /// Indicates whether the range is blank. Read-only.
    /// </summary>
    bool          IsBlank { get; }
    /// <summary>
    /// Indicates whether range contains boolean value. Read-only.
    /// </summary>
    bool          IsBoolean { get; }
    /// <summary>
    /// Indicates whether range contains error value.
    /// </summary>
    bool          IsError { get; }
    /// <summary>
    /// Indicates whether this range is grouped by column. Read-only.
    /// </summary>
    bool          IsGroupedByColumn { get; }
    /// <summary>
    /// Indicates whether this range is grouped by row. Read-only.
    /// </summary>
    bool          IsGroupedByRow { get; }
    /// <summary>
    /// Indicates whether cell is initialized. Read-only.
    /// </summary>
    bool          IsInitialized { get; }
    /// <summary>
    /// Returns last column of the range. Read-only.
    /// </summary>
    int           LastColumn { get; }
    /// <summary>
    /// Returns last row of the range. Read-only.
    /// </summary>
    int           LastRow { get; }
    /// <summary>
    /// Gets / sets double value of the range.
    /// </summary>
    double        Number{ get; set; }
    /// <summary>
    /// Format of current cell. Analog of Style.NumberFormat property.
    /// </summary>
    string        NumberFormat{ get; set; }
    /// <summary>
    /// Returns the number of the first row of the first area in
    /// the range. Read-only Long.
    /// </summary>
    int           Row { get; }
    /// <summary>
    /// Row group level. Read-only.
    /// -1 - Not all rows in the range have same group level.
    /// 0 - No grouping,
    /// 1 - 7 - Group level.
    /// </summary>
    int           RowGroupLevel { get; }
    /// <summary>
    /// Returns the height of all the rows in the range specified,
    /// measured in points. Returns Double.MinValue if the rows in the specified range
    /// aren't all the same height. Read / write Double.
    /// </summary>
    double        RowHeight { get; set; }
    /// <summary>
    /// For a Range object, returns an array of Range objects that represent the
    /// rows in the specified range.
    /// </summary>
    IRange[]      Rows { get; }
    /// <summary>
    /// For a Range object, returns an array of Range objects that represent the
    /// columns in the specified range.
    /// </summary>
    IRange[]      Columns { get; }
    /// <summary>
    /// Returns a Style object that represents the style of the specified
    /// range. Read/write IStyle.
    /// </summary>
    IStyle        CellStyle { get; set; }
    /// <summary>
    /// Returns name of the Style object that represents the style of the specified
    /// range. Read/write String.
    /// </summary>
    string        CellStyleName { get; set; }
    /// <summary>
    /// Gets / sets string value of the range.
    /// </summary>
    string        Text{ get; set; }
    /// <summary>
    /// Gets / sets time value of the range.
    /// </summary>
    TimeSpan      TimeSpan{ get; set; }
    /// <summary>
    /// Returns or sets the value of the specified range.
    /// Read/write Variant. Does not support FormulaArray value.
    /// </summary>
    string        Value { get; set; }
    /// <summary>
    /// Returns the calculated value of a formula using the most current inputs.
    /// </summary>
    string CalculatedValue { get; }
    /// <summary>
    /// Returns or sets the cell value. Read/write Variant.
    /// The only difference between this property and the Value property is
    /// that the Value2 property doesn't use the Currency and Date data types.
    /// Does not support FormulaArray value.
    /// </summary>
    object        Value2 { get; set; }
    /// <summary>
    /// Returns or sets the vertical alignment of the specified object.
    /// Read/write ExcelVAlign.
    /// </summary>
    ExcelVAlign   VerticalAlignment { get; set; }
    /// <summary>
    /// Returns a Worksheet object that represents the worksheet
    /// containing the specified range. Read-only.
    /// </summary>
    IWorksheet    Worksheet { get; }
    /// <summary>
    /// Gets / sets cell by row and column index. Row and column indexes are one-based.
    /// </summary>
    IRange        this[ int row, int column ] { get; set; }
    /// <summary>
    /// Get cell range. Row and column indexes are one-based. Read-only.
    /// </summary>
    IRange        this[ int row, int column, int lastRow, int lastColumn ]{ get; }
    /// <summary>
    /// Get cell range. Read-only.
    /// </summary>
    IRange        this[ string name ]{ get; }
    /// <summary>
    /// Gets cell range. Read-only.
    /// </summary>
    IRange        this[ string name, bool IsR1C1Notation ]{ get; }
    /// <summary>
    /// Collection of conditional formats.
    /// </summary>
    IConditionalFormats ConditionalFormats { get; }
    /// <summary>
    /// Data validation for the range.
    /// </summary>
    IDataValidation DataValidation { get; }
    /// <summary>
    /// Gets / sets string value evaluated by formula.
    /// </summary>
    string        FormulaStringValue { get; set; }
    /// <summary>
    /// Gets / sets number value evaluated by formula.
    /// </summary>
    double        FormulaNumberValue { get; set; }
    /// <summary>
    /// Indicates if current range has formula bool value. Read-only.
    /// </summary>
    bool          HasFormulaBoolValue { get; }
    /// <summary>
    /// Indicates if current range has formula error value. Read-only.
    /// </summary>
    bool HasFormulaErrorValue { get; }
    /// <summary>
    /// Indicates if current range has formula value formatted as DateTime. Read-only.
    /// </summary>
    bool HasFormulaDateTime { get; }
    /// <summary>
    /// Indicates if the current range has formula number value. Read-only.
    /// </summary>
    bool HasFormulaNumberValue { get; }
    /// <summary>
    /// Indicates if the current range has formula string value. Read-only.
    /// </summary>
    bool HasFormulaStringValue { get; }
    /// <summary>
    /// Comment assigned to the range. Read-only.
    /// </summary>
    ICommentShape Comment { get; }
    /// <summary>
    /// String with rich text formatting. Read-only.
    /// </summary>
    IRichTextString RichText { get; }
    /// <summary>
    /// Indicates whether this range is part of merged range. Read-only.
    /// </summary>
    bool          IsMerged { get; }
    /// <summary>
    /// Returns a Range object that represents the merged range containing
    /// the specified cell. If the specified cell isn�t in a merged range,
    /// this property returns NULL. Read-only.
    /// </summary>
    IRange        MergeArea { get; }
    /// <summary>
    /// True if Microsoft Excel wraps the text in the object.
    /// Read/write Boolean.
    /// </summary>
    bool          WrapText
    { get; set; }
    /// <summary>
    /// Indicates is current range has external formula. Read-only.
    /// </summary>
    bool HasExternalFormula { get; }
    /// <summary>
    /// Represents ignore error options.
    /// </summary>
    ExcelIgnoreError IgnoreErrorOptions { get; set; }
    /// <summary>
    /// Indicates whether all values in the range are preserved as strings.
    /// </summary>
    bool? IsStringsPreserved { get; set; }
    /// <summary>
    /// Gets/sets built in style.
    /// </summary>
    BuiltInStyles? BuiltInStyle { get; set; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Activates a single cell, which must be inside the current selection.
    /// To select a range of cells, use the Select method.
    /// </summary>
    /// <returns></returns>
    IRange  Activate();
      /// <summary>
      /// Activates a single cell, scroll to it and activates the corresponding sheet.
      /// To select a range of cells, use the Select method.
      /// </summary>
    /// <param name="scroll">True to scroll to the cell</param>
      /// <returns></returns>
    IRange Activate(bool scroll);
    /// <summary>
    /// This method groups current range.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether the grouping should be performed by rows or by columns. 
    /// </param>
    /// <returns>Current range after grouping.</returns>
    IRange  Group( ExcelGroupBy groupBy );
    /// <summary>
    /// This method groups current range.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether grouping should
    /// be performed by rows or by columns. 
    /// </param>
    /// <param name="bCollapsed">Indicates whether group should be collapsed.</param>
    /// <returns>Current range after grouping.</returns>
    IRange  Group( ExcelGroupBy groupBy, bool bCollapsed );
    /// <summary>
    /// This method creates subtotal on Corresponding ranges
    /// </summary>
    /// <param name="groupBy">Indicates the Group By Column</param>
    /// <param name="function">ConsolidationFunction to be applied</param>
    /// <param name="totalList">Columns to be added</param>
    void SubTotal(int groupBy, ConsolidationFunction function, int[] totalList);
    /// <summary>
    /// This method creates subtotal on Corresponding ranges
    /// </summary>
    /// <param name="groupBy">Indicates the Group By Column</param>
    /// <param name="function">ConsolidationFunction to be applied</param>
    /// <param name="totalList">Columns to be added</param>
    /// <param name="replace">Replaces Exisiting SubTotal</param>
    /// <param name="pageBreaks">Insert PageBreaks</param>
    /// <param name="summaryBelowData">SummaryBelowData</param>
    void SubTotal(int groupBy, ConsolidationFunction function, int[] totalList, bool replace, bool pageBreaks, bool summaryBelowData);
    /// <summary>
    /// Creates a merged cell from the specified Range object.
    /// </summary>
    void    Merge();
    /// <summary>
    /// Creates a merged cell from the specified Range object.
    /// </summary>
    /// <param name="clearCells">Indicates whether to clear unnecessary cells.</param>
    void Merge( bool clearCells );
    /// <summary>
    /// Ungroups current range.
    /// </summary>
    /// <param name="groupBy">Indicates type of ungrouping. Ungroup by columns or by rows.</param>
    /// <returns>Current range after ungrouping.</returns>
    IRange  Ungroup( ExcelGroupBy groupBy );
    /// <summary>
    /// Separates a merged area into individual cells.
    /// </summary>
    void    UnMerge();
    /// <summary>
    /// Freezes pane at the current range.
    /// </summary>
    void    FreezePanes();
    /// <summary>
    /// Clear the contents of the Range.
    /// </summary>
    void    Clear();
    /// <summary>
    /// Clear the contents of the Range with formatting.
    /// </summary>
    /// <param name="isClearFormat">True if formatting should also be cleared.</param>
    void    Clear( bool isClearFormat );
    /// <summary>
    /// Clears the cell content, formats, comments based on clear option.
    /// </summary>
    /// <param name="option"></param>
    void Clear(ExcelClearOptions option);
    /// <summary>
    /// Clear the contents of the Range and shifts the cells Up or Left
    /// without formula or merged ranges update.
    /// </summary>
    /// <param name="direction">Cells shift direction Up/Left.</param>
    void    Clear( ExcelMoveDirection direction );
    /// <summary>
    /// Clear the contents of the Range and shifts the cells Up or Left.
    /// </summary>
    /// <param name="direction">Cells shift direction Up/Left.</param>
    /// <param name="options">Cells shifting options.</param>
    void    Clear( ExcelMoveDirection direction, ExcelCopyRangeOptions options );
    /// <summary>
    /// Moves the cells to the specified Range (without updating formulas).
    /// </summary>
    /// <param name="destination">Destination Range.</param>
    void    MoveTo( IRange destination );
    /// <summary>
    /// Copies the range to the specified destination Range (without updating formulas).
    /// </summary>
    /// <param name="destination">Destination range.</param>
    /// <returns>Range were this range was copied.</returns>
    IRange  CopyTo( IRange destination );
    /// <summary>
    /// Copies this range into another location.
    /// </summary>
    /// <param name="destination">Destination range.</param>
    /// <param name="options">Copy range options.</param>
    /// <returns>Destination range.</returns>
    IRange CopyTo( IRange destination, ExcelCopyRangeOptions options );
    /// <summary>
    /// Returns intersection of this range with the specified one.
    /// </summary>
    /// <param name="range">The Range with which to intersect.</param>
    /// <returns>Range intersection; if there is no intersection, NULL is returned.</returns>
    IRange  IntersectWith( IRange range );
    /// <summary>
    /// Returns merge of this range with the specified one.
    /// </summary>
    /// <param name="range">The Range to merge with.</param>
    /// <returns>Merged ranges or NULL if wasn't able to merge ranges.</returns>
    IRange  MergeWith( IRange range );
    /// <summary>
    /// Autofits all rows in the range.
    /// </summary>
    void    AutofitRows();
    /// <summary>
    /// Autofits all columns in the range.
    /// </summary>
    void    AutofitColumns();
    /// <summary>
    /// Adds comment to the range.
    /// </summary>
    /// <returns>Range's comment.</returns>
    ICommentShape AddComment();
    /// <summary>
    /// This method searches for the first cell with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Flag that represent type of search.</param>
    /// <returns></returns>
    IRange FindFirst( string findValue, ExcelFindType flags );
    /// <summary>
    /// This method searches for the first cell with specified double value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Flag that represent type of search.</param>
    /// <returns>First found cell, or Null if value was not found. </returns>    
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
    /// <param name="flags">Flag that represent type of search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>    
    IRange[] FindAll( string findValue, ExcelFindType flags );
    /// <summary>
    /// This method searches for the all cells with specified double value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Flag that represent type of search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>    
    IRange[] FindAll( double findValue, ExcelFindType flags );
    /// <summary>
    /// This method searches for the all cells with specified bool value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
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
//    /// <summary>
//    /// Copies range to the clipboard.
//    /// </summary>
//    void CopyToClipboard();
    /// <summary>
    /// Sets around border for current range.
    /// </summary>
    void BorderAround();
    /// <summary>
    /// Sets around border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    void BorderAround( ExcelLineStyle borderLine );
    /// <summary>
    /// Sets around border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    /// <param name="borderColor">Represents border color.</param>
    void BorderAround( ExcelLineStyle borderLine, Color borderColor );
    /// <summary>
    /// Sets around border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    /// <param name="borderColor">Represents border color as ExcelKnownColors.</param>
    void BorderAround( ExcelLineStyle borderLine, ExcelKnownColors borderColor );
    /// <summary>
    /// Sets inside border for current range.
    /// </summary>
    void BorderInside();
    /// <summary>
    /// Sets inside border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    void BorderInside( ExcelLineStyle borderLine );
    /// <summary>
    /// Sets inside border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    /// <param name="borderColor">Represents border color.</param>
    void BorderInside( ExcelLineStyle borderLine, Color borderColor );
    /// <summary>
    /// Sets inside border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    /// <param name="borderColor">Represents border color as ExcelKnownColors.</param>
    void BorderInside( ExcelLineStyle borderLine, ExcelKnownColors borderColor );
    /// <summary>
    /// Sets none border for current range.
    /// </summary>
    void BorderNone();
    /// <summary>
    /// Collapses current group.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether the grouping should be performed by rows or by columns. 
    /// </param>
    void CollapseGroup( ExcelGroupBy groupBy );
    /// <summary>
    /// Expands current group.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether the grouping should be performed by rows or by columns. 
    /// </param>
    void ExpandGroup( ExcelGroupBy groupBy );
    /// <summary>
    /// Expands current group.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether the grouping should be performed by rows or by columns. 
    /// </param>
    /// <param name="flags">Additional option flags.</param>
    void ExpandGroup( ExcelGroupBy groupBy, ExpandCollapseFlags flags );
    #endregion
  }
}

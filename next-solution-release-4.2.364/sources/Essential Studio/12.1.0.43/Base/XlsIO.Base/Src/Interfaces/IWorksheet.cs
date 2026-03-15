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

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO;
#endif

#if !SILVERLIGHT && !WINRT && !WP
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
#endif

using System.IO;
using System.Text;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.PivotTables;

#if ( WINRT )
using System.Threading.Tasks;
using Windows.Storage;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#endif

#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents a worksheet. The Worksheet object is a member of the
  /// Worksheets collection. The Worksheets collection contains all the
  /// Worksheet objects in a workbook.
  /// </summary>
  public interface IWorksheet : ITabSheet

      ,Syncfusion.Calculate.ICalcData

  {

      #region Calculate methods
      Syncfusion.Calculate.CalcEngine CalcEngine { get;  set; }
      void EnableSheetCalculations();
      void DisableSheetCalculations();
      event MissingFunctionEventHandler MissingFunction;
      #endregion

    #region Not supported methods/properties
#if NOT_SUPPORTED
/*
    string _CodeName { get; set; }
    int _DisplayRightToLeft { get; set; }
    AutoFilter AutoFilter { get; }
    bool AutoFilterMode { get; set; }
    Range CircularReference { get; }
    string CodeName { get; }
    XlConsolidationFunction ConsolidationFunction { get; }
    object ConsolidationOptions { get; }
    object ConsolidationSources { get; }
    XlCreator Creator { get; }
    CustomProperties CustomProperties { get; }
    bool DisplayRightToLeft { get; set; }
    bool EnableAutoFilter { get; set; }
    bool EnableCalculation { get; set; }
    bool EnableOutlining { get; set; }
    bool EnablePivotTable { get; set; }
    bool FilterMode { get; }
    Hyperlinks Hyperlinks { get; }
    MsoEnvelope MailEnvelope { get; }
    string OnCalculate { get; set; }
    string OnData { get; set; }
    string OnDoubleClick { get; set; }
    string OnEntry { get; set; }
    string OnSheetActivate { get; set; }
    string OnSheetDeactivate { get; set; }
    Outline Outline { get; }
    bool ProtectContents { get; }
    bool ProtectDrawingObjects { get; }
    Protection Protection { get; }
    bool ProtectionMode { get; }
    bool ProtectScenarios { get; }
    QueryTables QueryTables { get; }
    Scripts Scripts { get; }
    string ScrollArea { get; set; }
    Shapes Shapes { get; }
    SmartTags SmartTags { get; }
    Tab Tab { get; }
    bool TransitionExpEval { get; set; }
    bool TransitionFormEntry { get; set; }
    /// <summary>
    ///
    /// </summary>
    bool DisplayAutomaticPageBreaks { get; set; }
    /// <summary>
    ///
    /// </summary>
    XlEnableSelection EnableSelection { get; set; }
    XlSheetVisibility Visible { get; set; }

    // Methods
    void _CheckSpelling(object CustomDictionary, object IgnoreUppercase, object AlwaysSuggest, object SpellLang, object IgnoreFinalYaa, object SpellScript);
    object _Evaluate(object Name);
    void _PasteSpecial(object Format, object Link, object DisplayAsIcon, object IconFileName, object IconIndex, object IconLabel);
    void _PrintOut(object From, object To, object Copies, object Preview, object ActivePrinter, object PrintToFile, object Collate);
    void _Protect(object Password, object DrawingObjects, object Contents, object Scenarios, object UserInterfaceOnly);
    void _SaveAs(string Filename, object FileFormat, object Password, object WriteResPassword, object ReadOnlyRecommended, object CreateBackup, object AddToMru, object TextCodepage, object TextVisualLayout);
    object Arcs(object Index);
    object Buttons(object Index);
    void Calculate();
    object ChartObjects(object Index);
    object CheckBoxes(object Index);
    void CheckSpelling(object CustomDictionary, object IgnoreUppercase, object AlwaysSuggest, object SpellLang);
    void CircleInvalid();
    void ClearArrows();
    void ClearCircles();
    object DrawingObjects(object Index);
    object Drawings(object Index);
    object DropDowns(object Index);
    object Evaluate(object Name);
    object GroupBoxes(object Index);
    object GroupObjects(object Index);
    object Labels(object Index);
    object Lines(object Index);
    object ListBoxes(object Index);
    void Move(object Before, object After);
    object OLEObjects(object Index);
    object OptionButtons(object Index);
    object Ovals(object Index);
    object Pictures(object Index);
    object PivotTables(object Index);
    Excel.PivotTable PivotTableWizard(object SourceType, object SourceData, object TableDestination, object TableName, object RowGrand, object ColumnGrand, object SaveData, object HasAutoFormat, object AutoPage, object Reserved, object BackgroundQuery, object OptimizeCache, object PageFieldOrder, object PageFieldWrapCount, object ReadData, object Connection);
    void PrintOut(object From, object To, object Copies, object Preview, object ActivePrinter, object PrintToFile, object Collate, object PrToFileName);
    void PrintPreview(object EnableChanges);
    void Protect(object Password, object DrawingObjects, object Contents, object Scenarios, object UserInterfaceOnly, object AllowFormattingCells, object AllowFormattingColumns, object AllowFormattingRows, object AllowInsertingColumns, object AllowInsertingRows, object AllowInsertingHyperlinks, object AllowDeletingColumns, object AllowDeletingRows, object AllowSorting, object AllowFiltering, object AllowUsingPivotTables);
    object Rectangles(object Index);
    object Scenarios(object Index);
    object ScrollBars(object Index);
    void SetBackgroundPicture(string Filename);
    object Spinners(object Index);
    object TextBoxes(object Index);
    void Unprotect(object Password);
    void ShowDataForm();
    void ShowAllData();
*/
#endif
    #endregion

    #region Skipped
#if SKIPPED
/*
    /// <summary>
    /// Returns an HPageBreaks collection that represents the horizontal
    /// page breaks on the sheet. Read-only.
    /// </summary>
    IHPageBreaks HPageBreaks { get; }
    /// <summary>
    /// Returns a Range object that represents all the columns on the
    /// specified worksheet. Read-only.
    /// </summary>
    IRange Columns { get; }
    /// <summary>
    /// Returns a Comments collection that represents all the comments
    /// for the specified worksheet. Read-only.
    /// </summary>
    IComments Comments { get; }
    /// <summary>
    /// Returns a Chart, Range, or Worksheet object that represents the next
    /// sheet or cell. Read-only.
    /// </summary>
    object Next { get; }
    /// <summary>
    /// Returns a Chart, Range, or Worksheet object that represents the previous
    /// sheet or cell. Read-only.
    /// </summary>
    object Previous { get; }

    /// <summary>
    /// Copies the sheet to another location in the workbook.
    /// </summary>
    /// <param name="Before"></param>
    /// <param name="After"></param>
    void Copy(object Before, object After);
    /// <summary>
    /// Deletes the object.
    /// </summary>
    void Delete();
    /// <summary>
    /// Pastes the contents of the Clipboard onto the sheet.
    /// </summary>
    /// <param name="Destination"></param>
    /// <param name="Link"></param>
    void Paste(object Destination, object Link);
    /// <summary>
    /// Pastes the contents of the Clipboard onto the sheet, using a
    /// specified format. Use this method to paste data from other
    /// applications or to paste data in a specific format.
    /// </summary>
    /// <param name="Format"></param>
    /// <param name="Link"></param>
    /// <param name="DisplayAsIcon"></param>
    /// <param name="IconFileName"></param>
    /// <param name="IconIndex"></param>
    /// <param name="IconLabel"></param>
    /// <param name="NoHTMLFormatting"></param>
    void PasteSpecial(object Format, object Link, object DisplayAsIcon, object IconFileName, object IconIndex, object IconLabel, object NoHTMLFormatting);
    /// <summary>
    /// Resets all page breaks on the specified worksheet.
    /// </summary>
    void ResetAllPageBreaks();
    /// <summary>
    /// Saves changes to the chart or worksheet in a different file.
    /// </summary>
    /// <param name="Filename"></param>
    /// <param name="FileFormat"></param>
    /// <param name="Password"></param>
    /// <param name="WriteResPassword"></param>
    /// <param name="ReadOnlyRecommended"></param>
    /// <param name="CreateBackup"></param>
    /// <param name="AddToMru"></param>
    /// <param name="TextCodepage"></param>
    /// <param name="TextVisualLayout"></param>
    /// <param name="Local"></param>
    void SaveAs(string Filename, object FileFormat, object Password, object WriteResPassword, object ReadOnlyRecommended, object CreateBackup, object AddToMru, object TextCodepage, object TextVisualLayout, object Local);
    /// <summary>
    /// Selects the object.
    /// </summary>
    /// <param name="Replace"></param>
    void Select(object Replace);
    /// <summary>
    /// Returns a VPageBreaks collection that represents the vertical page
    /// breaks on the sheet. Read-only.
    /// </summary>
    IVPageBreaks VPageBreaks { get; }
*/
#endif
    #endregion

    #region Interface properties
    /// <summary>
    /// Returns collection of worksheet's autofilters. Read-only.
    /// </summary>
    IAutoFilters    AutoFilters { get; }
    /// <summary>
    /// Returns all used cells in the worksheet. Read-only.
    /// </summary>
    IRange[]        Cells { get; }
    /// <summary>
    /// True if page breaks (both automatic and manual) on the specified
    /// worksheet are displayed. Read / write Boolean.
    /// </summary>
    bool            DisplayPageBreaks { get; set; }
    /// <summary>
    /// Returns the index number of the object within the collection of
    /// similar objects. Read-only.
    /// </summary>
    int             Index { get; }
    /// <summary>
    /// Returns all merged ranges. Read-only.
    /// </summary>
    IRange[]        MergedCells{ get; }
    /// <summary>
    /// For a Worksheet object, returns a Names collection that represents
    /// all the worksheet-specific names (names defined with the "WorksheetName!"
    /// prefix). Read-only Names object.
    /// </summary>
    INames          Names { get; }
    /// <summary>
    /// Returns a PageSetup object that contains all the page setup settings
    /// for the specified object. Read-only.
    /// </summary>
    IPageSetup      PageSetup { get; }
    /// <summary>
    /// Returns a Range object that represents a cell or a range of cells.
    /// </summary>
    IRange          Range { get; }
    /// <summary>
    /// For a Worksheet object, returns an array of Range objects that represents
    /// all the rows on the specified worksheet. Read-only Range object.
    /// </summary>
    IRange[]        Rows { get; }
    /// <summary>
    /// For a Worksheet object, returns an array of Range objects that represents
    /// all the columns on the specified worksheet. Read-only Range object.
    /// </summary>
    IRange[]        Columns { get; }
    /// <summary>
    /// Returns or sets the standard (default) height of all the rows in the worksheet,
    /// in points. Read/write Double.
    /// </summary>
    double          StandardHeight { get; set; }
    /// <summary>
    /// Returns or sets the standard (default) height option flag, which defines that
    /// standard (default) row height and book default font height do not match.
    /// Read/write Bool.
    /// </summary>
    bool            StandardHeightFlag { get; set; }
    /// <summary>
    /// Returns or sets the standard (default) width of all the columns in the
    /// worksheet. Read/write Double.
    /// </summary>
    double          StandardWidth { get; set; }
    /// <summary>
    /// Returns or sets the worksheet type. Read-only ExcelSheetType.
    /// </summary>
    ExcelSheetType  Type { get; }
    /// <summary>
    /// Returns a Range object that represents the used range on the
    /// specified worksheet. Read-only.
    /// </summary>
    IRange          UsedRange { get; }
    /// <summary>
    /// Zoom factor of document. Value must be in range from 10 till 400.
    /// </summary>
    int             Zoom{ get; set; }
    /// <summary>
    /// Position of the vertical split (px, 0 = No vertical split):
    /// Unfrozen pane: Width of the left pane(s) (in twips = 1/20 of a point)
    /// Frozen pane: Number of visible columns in left pane(s)
    /// </summary>
    int             VerticalSplit { get; set; }
    /// <summary>
    /// Position of the horizontal split (by, 0 = No horizontal split):
    /// Unfrozen pane: Height of the top pane(s) (in twips = 1/20 of a point)
    /// Frozen pane: Number of visible rows in top pane(s)
    /// </summary>
    int             HorizontalSplit{ get; set; }
    /// <summary>
    /// Index to first visible row in bottom pane(s).
    /// </summary>
    int             FirstVisibleRow { get; set; }
    /// <summary>
    /// Index to first visible column in right pane(s).
    /// </summary>
    int             FirstVisibleColumn { get; set; }
    /// <summary>
    /// Identifier of pane with active cell cursor.
    /// </summary>
    int             ActivePane { get; set; }
    /// <summary>
    /// True if zero values to be displayed
    /// False otherwise.
    /// </summary>
    bool IsDisplayZeros { get; set; }
    /// <summary>
    /// True if gridlines are visible;
    /// False otherwise.
    /// </summary>
    bool            IsGridLinesVisible { get; set; }
    /// <summary>
    /// Gets / sets Grid line color.
    /// </summary>
    ExcelKnownColors GridLineColor { get; set; }   
    /// <summary>
    /// True if row and column headers are visible;
    /// False otherwise.
    /// </summary>
    bool            IsRowColumnHeadersVisible { get; set; }
    /// <summary>
    /// Returns a VPageBreaks collection that represents the vertical page
    /// breaks on the sheet. Read-only.
    /// </summary>
    IVPageBreaks    VPageBreaks { get; }
    /// <summary>
    /// Returns an HPageBreaks collection that represents the horizontal
    /// page breaks on the sheet. Read-only.
    /// </summary>
    IHPageBreaks    HPageBreaks { get; }
    /// <summary>
    /// Indicates if all values in the workbook are preserved as strings.
    /// </summary>
    bool            IsStringsPreserved { get; set; }
    /// <summary>
    /// Comments collection.
    /// </summary>
    IComments       Comments { get; }
    /// <summary>
    /// Gets / sets cell by row and index.
    /// </summary>
    IRange          this[ int row, int column ]{ get; }
    /// <summary>
    /// Get cells range.
    /// </summary>
    IRange          this[ int row, int column, int lastRow, int lastColumn ]{ get; }
    /// <summary>
    /// Get cell range.
    /// </summary>
    IRange          this[ string name ]{ get; }
    /// <summary>
    /// Get cell range.
    /// </summary>
    IRange          this[ string name, bool IsR1C1Notation ]{ get; }
    /// <summary>
    /// Collection of all worksheet's hyperlinks.
    /// </summary>
    IHyperLinks  HyperLinks { get; }
    /// <summary>
    /// Returns all not empty or accessed cells. Read-only.
    /// WARNING: This property creates Range object for each cell in the worksheet
    /// and creates new array each time user calls to it. It can cause huge memory
    /// usage especially if called frequently.
    /// </summary>
    IRange[]     UsedCells { get; }
    /// <summary>
    /// Returns collection of custom properties. Read-only.
    /// </summary>
    IWorksheetCustomProperties CustomProperties { get; }
    /// <summary>
    /// Indicates whether all created range objects should be cached. Default value is false.
    /// </summary>
    bool UseRangesCache { get; set; }
    /// <summary>
    /// Defines whether freezed panes are applied.
    /// </summary>
    bool IsFreezePanes { get; }
    /// <summary>
    /// Return split cell range.
    /// </summary>
    IRange SplitCell { get; }
    /// <summary>
    /// Gets/sets top visible row of the worksheet.
    /// </summary>
    int TopVisibleRow
    {
      get;
      set;
    }
    /// <summary>
    /// Gets/sets left visible column of the worksheet.
    /// </summary>
    int LeftVisibleColumn
    {
      get;
      set;
    }
    /// <summary>
    /// There are two different algorithms to create UsedRange object:
    /// 1) Default. This property = true. The cell is included into UsedRange when
    /// it has some record created for it even if data is empty (maybe some formatting
    /// changed, maybe not - cell was accessed and record was created).
    /// 2) This property = false. In this case XlsIO tries to remove empty rows and
    /// columns from all sides to make UsedRange smaller.
    /// </summary>
    bool UsedRangeIncludesFormatting { get; set; }
    /// <summary>
    /// Returns pivot table collection containing all pivot tables in the worksheet. Read-only.
    /// </summary>
    IPivotTables PivotTables { get; }
    /// <summary>
    /// Gets collection of all list objects in the worksheet.
    /// </summary>
    IListObjects ListObjects { get; }
    /// <summary>
    /// Gets or sets the view setting of the sheet.
    /// </summary>    
    SheetView View { get; set; }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Gets the OLE objects.
    /// </summary>
    /// <value>The OLE objects.</value>
    IOleObjects OleObjects { get; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is OLE object.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is OLE object; otherwise, <c>false</c>.
    /// </value>
    bool HasOleObject { get; }
# endif
    ISparklineGroups SparklineGroups { get; }
    #endregion

    #region Interface methods
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Copies worksheet into the clipboard.
    /// </summary>
    void CopyToClipboard();
#endif
    /// <summary>
    /// Clears worksheet data. Removes all formatting and merges.
    /// </summary>
    void Clear();
    /// <summary>
    /// Clears worksheet. Only the data is removed from each cell.
    /// </summary>
    void ClearData();
    /// <summary>
    /// Indicates whether a cell was initialized or accessed by the user.
    /// </summary>
    /// <param name="iRow">One-based row index of the cell.</param>
    /// <param name="iColumn">One-based column index of the cell.</param>
    /// <returns>Value indicating whether the cell was initialized or accessed by the user.</returns>
    bool Contains( int iRow, int iColumn );
    /// <summary>
    /// Creates new instance of IRanges.
    /// </summary>
    /// <returns>New instance of ranges collection.</returns>
    IRanges CreateRangesCollection();
      /// <summary>
      /// Create Named Ranges
      /// </summary>
      /// <param name="namedRange">Names to create</param>
      /// <param name="referRange">Refers to range</param>
      /// <param name="vertical">True if the named range values are vertically placed in the sheet.</param>
    void CreateNamedRanges(string namedRange, string referRange, bool vertical);
    /// <summary>
    /// Creates object that can be used for template markers processing.
    /// </summary>
    /// <returns>Object that can be used for template markers processing.</returns>
    ITemplateMarkersProcessor CreateTemplateMarkersProcessor();
    /// <summary>
    /// Method check is Column with specified index visible to end user or not.
    /// </summary>
    /// <param name="columnIndex">Index of column.</param>
    /// <returns>True - column is visible; otherwise False.</returns>
    bool IsColumnVisible( int columnIndex );
    /// <summary>
    /// Shows / Hides the specified column.
    /// </summary>
    /// <param name="columnIndex">Index at which the column should be hidden.</param>
    /// <param name="isVisible">True - Column is visible; false - hidden.</param>
    void ShowColumn( int columnIndex, bool isVisible );
    /// <summary>
    /// Hides the specified column.
    /// </summary>
    /// <param name="columnIndex">One-based column index to hide.</param>
    void HideColumn(int columnIndex);
    /// <summary>
    /// Hides the specified row.
    /// </summary>
    /// <param name="rowIndex">One-based row index to hide.</param>
    void HideRow(int rowIndex);
    /// <summary>
    /// Method check is Row with specified index visible to user or not.
    /// </summary>
    /// <param name="rowIndex">Index of row visibility of each must be checked.</param>
    /// <returns>True - row is visible to user, otherwise False.</returns>
    bool IsRowVisible( int rowIndex );
    /// <summary>
    /// Shows / Hides the specified row.
    /// </summary>
    /// <param name="rowIndex">Index at which the row should be hidden.</param>
    /// <param name="isVisible">True - Row is visible; false - hidden.</param>
    void ShowRow( int rowIndex, bool isVisible );
	/// <summary>
    /// Shows / Hides the specified range.
    /// </summary>
    /// <param name="range">Range specifies the particular range to show / hide</param>
    /// <param name="isVisible">True - Row is visible; false - hidden.</param>
    void ShowRange(IRange range, bool isVisible);
    /// <summary>
    /// Shows/ Hides the collection of range.
    /// </summary>
    /// <param name="ranges">Ranges specifies the range collection.</param>
    /// <param name="isVisible">True - Row is visible; false - hidden.</param>
    void ShowRange(RangesCollection ranges, bool isVisible);
    /// <summary>
    /// Shows/ Hides an array of range.
    /// </summary>
    /// <param name="ranges">Ranges specifies the range array.</param>
    /// <param name="isVisible">True - Row is visible; false - hidden.</param>
    void ShowRange(IRange[] ranges, bool isVisible);
    /// <summary>
    /// Inserts an empty row with default formatting (with formulas update).
    /// </summary>
    /// <param name="index">Index at which new row should be inserted.</param>
    void InsertRow( int index );
    /// <summary>
    /// Inserts an empty row with default formatting.
    /// </summary>
    /// <param name="iRowIndex">Index at which new row should be inserted.</param>
    /// <param name="iRowCount">Number of rows to insert.</param>
    void InsertRow( int iRowIndex, int iRowCount );
    /// <summary>
    /// Inserts an empty row with default formatting.
    /// </summary>
    /// <param name="iRowIndex">Index at which new row should be inserted.</param>
    /// <param name="iRowCount">Number of rows to insert.</param>
    /// <param name="insertOptions">Insert options.</param>
    void InsertRow( int iRowIndex, int iRowCount, ExcelInsertOptions insertOptions );
    /// <summary>
    /// Inserts an empty column with default formatting.
    /// </summary>
    /// <param name="index">Index at which new column should be inserted.</param>
    void InsertColumn( int index );
    /// <summary>
    /// Inserts an empty column with default formatting (with formulas update).
    /// </summary>
    /// <param name="iColumnIndex">Index at which new column should be inserted.</param>
    /// <param name="iColumnCount">Number of columns to insert.</param>
    void InsertColumn( int iColumnIndex, int iColumnCount );
    /// <summary>
    /// Inserts an empty column with default formatting.
    /// </summary>
    /// <param name="iColumnIndex">Index at which new column should be inserted.</param>
    /// <param name="iColumnCount">Number of columns to insert.</param>
    /// <param name="insertOptions">Insert options.</param>
    void InsertColumn( int iColumnIndex, int iColumnCount,
      ExcelInsertOptions insertOptions );
    /// <summary>
    /// Removes specified row (with formulas update).
    /// </summary>
    /// <param name="index">One-based row index to remove.</param>
    void DeleteRow( int index );
    /// <summary>
    /// Removes specified row (with formulas update).
    /// </summary>
    /// <param name="index">One-based row index to remove.</param>
    /// <param name="count">Number of rows to remove.</param>
    void DeleteRow( int index, int count );
    /// <summary>
    /// Removes specified column (with formulas update).
    /// </summary>
    /// <param name="index">One-based column index to remove.</param>
    void DeleteColumn( int index );
    /// <summary>
    /// Removes specified column (without updating formulas).
    /// </summary>
    /// <param name="index">One-based column index to remove.</param>
    /// <param name="count">Number of columns to remove.</param>
    void DeleteColumn( int index, int count );
    /// <summary>
    /// Imports an array of objects into a worksheet.
    /// </summary>
    /// <param name="arrObject">Array to import.</param>
    /// <param name="firstRow">Row of the first cell where array should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where array should be imported.</param>
    /// <param name="isVertical">True if array should be imported vertically; False - horizontally.</param>
    /// <returns>Number of imported elements.</returns>
    int  ImportArray( object[] arrObject, int firstRow, int firstColumn
      , bool isVertical );
    /// <summary>
    /// Imports an array of strings into a worksheet.
    /// </summary>
    /// <param name="arrString">Array to import.</param>
    /// <param name="firstRow">Row of the first cell where array should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where array should be imported.</param>
    /// <param name="isVertical">True if array should be imported vertically; False - horizontally.</param>
    /// <returns>Number of imported elements.</returns>
    int  ImportArray( string[] arrString, int firstRow, int firstColumn
      , bool isVertical );
    /// <summary>
    /// Imports an array of integers into a worksheet.
    /// </summary>
    /// <param name="arrInt">Array to import.</param>
    /// <param name="firstRow">Row of the first cell where array should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where array should be imported.</param>
    /// <param name="isVertical">True if array should be imported vertically; False - horizontally.</param>
    /// <returns>Number of imported elements.</returns>
    int  ImportArray( int[] arrInt, int firstRow, int firstColumn
      , bool isVertical );
    /// <summary>
    /// Imports an array of doubles into a worksheet.
    /// </summary>
    /// <param name="arrDouble">Array to import.</param>
    /// <param name="firstRow">Row of the first cell where array should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where array should be imported.</param>
    /// <param name="isVertical">True if array should be imported vertically; False - horizontally.</param>
    /// <returns>Number of imported elements.</returns>
    int  ImportArray( double[] arrDouble, int firstRow, int firstColumn
      , bool isVertical );
    /// <summary>
    /// Imports an array of DateTimes into worksheet.
    /// </summary>
    /// <param name="arrDateTime">Array to import.</param>
    /// <param name="firstRow">Row of the first cell where array should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where array should be imported.</param>
    /// <param name="isVertical">True if array should be imported vertically; False - horizontally.</param>
    /// <returns>Number of imported elements.</returns>
    int  ImportArray( DateTime[] arrDateTime, int firstRow, int firstColumn
      , bool isVertical );

    /// <summary>
    /// Imports an array of objects into a worksheet.
    /// </summary>
    /// <param name="arrObject">Array to import.</param>
    /// <param name="firstRow">Row of the first cell where array should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where array should be imported.</param>
    /// <returns>Number of imported rows.</returns>
    int  ImportArray( object[,] arrObject, int firstRow, int firstColumn );
    /// <summary>
    /// Imports data from class objects into worksheet
    /// </summary>
    /// <param name="arrObject">IEnumerable object with desired data</param>
    /// <param name="firstRow">Row of the First cell to be imported</param>
    /// <param name="firstColumn">Column of the first cell to be imported</param>
    /// <param name="includeHeader">TRUE if class properties names must also be imported</param>
    /// <returns></returns>
    int ImportData(IEnumerable arrObject, int firstRow, int firstColumn, bool includeHeader);
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Imports data from a DataColumn into worksheet.
    /// </summary>
    /// <param name="dataColumn">DataColumn with desired data.</param>
    /// <param name="isFieldNameShown">True if column name must also be imported.</param>
    /// <param name="firstRow">Row of the first cell where DataTable should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where DataTable should be imported.</param>
    /// <returns>Number of imported rows.</returns>
    int ImportDataColumn( DataColumn dataColumn, bool isFieldNameShown
      , int firstRow, int firstColumn );
    /// <summary>
    /// Imports data from a DataTable into worksheet.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="isFieldNameShown">True if column names must also be imported.</param>
    /// <param name="firstRow">Row of the first cell where DataTable should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where DataTable should be imported.</param>
    /// <returns>Number of imported rows.</returns>
    int ImportDataTable( DataTable dataTable, bool isFieldNameShown
      , int firstRow, int firstColumn );
    /// <summary>
    /// Imports data from a DataTable into worksheet.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="isFieldNameShown">True if column names must also be imported.</param>
    /// <param name="firstRow">Row of the first cell where DataTable should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where DataTable should be imported.</param>
    /// <param name="preserveTypes">
    /// Indicates whether XlsIO should try to preserve types in DataTable,
    /// i.e. if it is set to False (default) and in DataTable we have in string column
    /// value that contains only numbers, it would be converted to number.
    /// </param>
    /// <returns>Number of imported rows.</returns>
    int ImportDataTable( DataTable dataTable, bool isFieldNameShown
      , int firstRow, int firstColumn, bool preserveTypes );
    /// <summary>
    /// Imports data from a DataTable into worksheet.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="isFieldNameShown">True if column names must also be imported.</param>
    /// <param name="firstRow">Row of the first cell where DataTable should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where DataTable should be imported.</param>
    /// <param name="maxRows">Maximum number of rows to import.</param>
    /// <param name="maxColumns">Maximum number of columns to import.</param>
    /// <returns>Number of imported rows.</returns>
    int ImportDataTable( DataTable dataTable, bool isFieldNameShown
      , int firstRow, int firstColumn, int maxRows, int maxColumns );
    /// <summary>
    /// Imports data from a DataTable into worksheet.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="isFieldNameShown">True if column names must also be imported.</param>
    /// <param name="firstRow">Row of the first cell where DataTable should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where DataTable should be imported.</param>
    /// <param name="maxRows">Maximum number of rows to import.</param>
    /// <param name="maxColumns">Maximum number of columns to import.</param>
    /// <param name="preserveTypes">
    /// Indicates whether XlsIO should try to preserve types in DataTable,
    /// i.e. if it is set to False (default) and in DataTable we have in string column
    /// value that contains only numbers, it would be converted to number.
    /// </param>
    /// <returns>Number of imported rows.</returns>
    int ImportDataTable( DataTable dataTable, bool isFieldNameShown
      , int firstRow, int firstColumn, int maxRows, int maxColumns, bool preserveTypes );

    /// <summary>
    /// Imports data from a DataTable into namedRange.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="namedRange">Represents named range.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <returns>Number of imported rows.</returns>
    int ImportDataTable( DataTable dataTable, IName namedRange, bool isFieldNameShown );
    /// <summary>
    /// Imports data from a DataTable into namedRange.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="namedRange">Represents named range.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="rowOffset">Represents row offset into named range to import.</param>
    /// <param name="columnOffset">Represents column offset into named range to import.</param>
    /// <returns>Number of imported rows.</returns>
    int ImportDataTable( DataTable dataTable, IName namedRange, bool isFieldNameShown
      , int rowOffset, int columnOffset );
    /// <summary>
    /// Imports data from a DataTable into namedRange.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="namedRange">Represents named range.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="rowOffset">Represents row offset into named range to import.</param>
    /// <param name="columnOffset">Represents column offset into named range to import.</param>
    /// <param name="iMaxRow">Represents count of rows to import.</param>
    /// <param name="iMaxCol">Represents count of rows to import.</param>
    /// <returns>Number of imported rows.</returns>
    int ImportDataTable( DataTable dataTable, IName namedRange, bool isFieldNameShown
      , int rowOffset, int columnOffset, int iMaxRow, int iMaxCol );
    /// <summary>
    /// Imports data from a DataTable into namedRange.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="namedRange">Represents named range.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="rowOffset">Represents row offset into named range to import.</param>
    /// <param name="columnOffset">Represents column offset into named range to import.</param>
    /// <param name="iMaxRow">Represents count of rows to import.</param>
    /// <param name="iMaxCol">Represents count of rows to import.</param>
    /// <param name="bPreserveTypes">Indicates whether to preserve column types.</param>
    /// <returns>Number of imported rows.</returns>
    int ImportDataTable( DataTable dataTable, IName namedRange, bool isFieldNameShown
      , int rowOffset, int columnOffset, int iMaxRow, int iMaxCol, bool bPreserveTypes );

    /// <summary>
    /// Imports data from a DataView into worksheet.
    /// </summary>
    /// <param name="dataView">DataView with desired data.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="firstRow">
    /// Row of the first cell where DataView should be imported.
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where DataView should be imported.
    /// </param>
    /// <returns>Number of imported rows.</returns>
    int ImportDataView( DataView dataView, bool isFieldNameShown
      , int firstRow, int firstColumn );
    /// <summary>
    /// Imports data from a DataView into worksheet.
    /// </summary>
    /// <param name="dataView">DataView with desired data.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="firstRow">
    /// Row of the first cell where DataView should be imported.
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where DataView should be imported.
    /// </param>
    /// <param name="bPreserveTypes">Indicates whether to preserve column types.</param>
    /// <returns>Number of imported rows.</returns>
    int ImportDataView( DataView dataView, bool isFieldNameShown
      , int firstRow, int firstColumn, bool bPreserveTypes );
    /// <summary>
    /// Imports data from a DataView into worksheet.
    /// </summary>
    /// <param name="dataView">DataView with desired data.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="firstRow">
    /// Row of the first cell where DataView should be imported.
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where DataView should be imported.
    /// </param>
    /// <param name="maxRows">Maximum number of rows to import.</param>
    /// <param name="maxColumns">Maximum number of columns to import.</param>
    /// <returns>Number of imported rows.</returns>
    int ImportDataView( DataView dataView, bool isFieldNameShown
      , int firstRow, int firstColumn, int maxRows, int maxColumns );
    /// <summary>
    /// Imports data from a DataView into worksheet.
    /// </summary>
    /// <param name="dataView">DataView with desired data.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="firstRow">
    /// Row of the first cell where DataView should be imported.
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where DataView should be imported.
    /// </param>
    /// <param name="maxRows">Maximum number of rows to import.</param>
    /// <param name="maxColumns">Maximum number of columns to import.</param>
    /// <param name="bPreserveTypes">Indicates whether to preserve column types.</param>
    /// <returns>Number of imported rows</returns>
    int ImportDataView( DataView dataView, bool isFieldNameShown,
      int firstRow, int firstColumn, int maxRows, int maxColumns,
      bool bPreserveTypes );
#endif

    /// <summary>
    /// Removes panes from a worksheet.
    /// </summary>
    void RemovePanes();
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Exports worksheet data into a DataTable.
    /// </summary>
    /// <param name="firstRow">Row of the first cell from where DataTable should be exported.</param>
    /// <param name="firstColumn">Column of the first cell from where DataTable should be exported.</param>
    /// <param name="maxRows">Maximum number of rows to export.</param>
    /// <param name="maxColumns">Maximum number of columns to export.</param>
    /// <param name="options">Export options.</param>
    /// <returns>DataTable with worksheet data.</returns>
    DataTable ExportDataTable( int firstRow, int firstColumn
      , int maxRows, int maxColumns, ExcelExportDataTableOptions options );
    /// <summary>
    /// Exports worksheet data into a DataTable.
    /// </summary>
    /// <param name="dataRange">Range to export.</param>
    /// <param name="options">Export options.</param>
    /// <returns>DataTable with worksheet data.</returns>
    DataTable ExportDataTable( IRange dataRange, ExcelExportDataTableOptions options );
    
#endif
    /// <summary>
    /// Intersects two ranges.
    /// </summary>
    /// <param name="range1">First range to intersect.</param>
    /// <param name="range2">Second range to intersect.</param>
    /// <returns>Intersection of two ranges or NULL if there is no range intersection.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When range1 or range2 is NULL.
    /// </exception>
    IRange IntersectRanges( IRange range1, IRange range2 );
    /// <summary>
    /// Merges two ranges.
    /// </summary>
    /// <param name="range1">First range to merge.</param>
    /// <param name="range2">Second range to merge.</param>
    /// <returns>Merged ranges or NULL if wasn't able to merge ranges.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When range1 or range2 is NULL.
    /// </exception>
    IRange MergeRanges( IRange range1, IRange range2 );
    /// <summary>
    /// Autofits specified row.
    /// </summary>
    /// <param name="rowIndex">One-based row index.</param>
    void AutofitRow( int rowIndex );
    /// <summary>
    /// Autofits specified column.
    /// </summary>
    /// <param name="colIndex">One-based column index.</param>
    void AutofitColumn( int colIndex );
    /// <summary>
    /// Replaces specified string by specified value.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValue">New value for the range with specified string.</param>
    void Replace( string oldValue, string newValue );
    /// <summary>
    /// Replaces specified string by specified value.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValue">New value for the range with specified string.</param>
    void Replace( string oldValue, double newValue );
    /// <summary>
    /// Replaces specified string by specified value.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValue">New value for the range with specified string.</param>
    void Replace( string oldValue, DateTime newValue );
    /// <summary>
    /// Replaces specified string by data from array.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Array of new values.</param>
    /// <param name="isVertical">
    /// Indicates whether array should be inserted vertically.
    /// </param>
    void Replace( string oldValue, string[] newValues, bool isVertical );
    /// <summary>
    /// Replaces specified string by data from array.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Array of new values.</param>
    /// <param name="isVertical">
    /// Indicates whether array should be inserted vertically.
    /// </param>
    void Replace( string oldValue, int[] newValues, bool isVertical );
    /// <summary>
    /// Replaces specified string by data from array.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Array of new values.</param>
    /// <param name="isVertical">
    /// Indicates whether array should be inserted vertically.
    /// </param>
    void Replace( string oldValue, double[] newValues, bool isVertical );
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Replaces specified string by data table values.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Data table with new data.</param>
    /// <param name="isFieldNamesShown">Indicates whether field name must be shown.</param>
    void Replace( string oldValue, DataTable newValues, bool isFieldNamesShown );
    /// <summary>
    /// Replaces specified string by data column values.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Data table with new data.</param>
    /// <param name="isFieldNamesShown">Indicates whether field name must be shown.</param>
    void Replace( string oldValue, DataColumn newValues, bool isFieldNamesShown );
#endif
    /// <summary>
    /// Removes worksheet from parent worksheets collection.
    /// </summary>
    void Remove();
    /// <summary>
    /// Moves worksheet.
    /// </summary>
    /// <param name="iNewIndex">New index of the worksheet.</param>
    void Move( int iNewIndex );
    /// <summary>
    /// Converts column width into pixels.
    /// </summary>
    /// <param name="widthInChars">Width in characters.</param>
    /// <returns>Width in pixels</returns>
    int ColumnWidthToPixels( double widthInChars );
    /// <summary>
    /// Converts pixels into column width (in characters).
    /// </summary>
    /// <param name="pixels">Width in pixels</param>
    /// <returns>Width in characters.</returns>
    double PixelsToColumnWidth( int pixels );
    /// <summary>
    /// Sets column width.
    /// </summary>
    /// <param name="iColumnIndex">One-based column index.</param>
    /// <param name="value">Width to set.</param>
    void SetColumnWidth( int iColumnIndex, double value );
    /// <summary>
    /// Sets column width.
    /// </summary>
    /// <param name="iColumnIndex">One-based column index.</param>
    /// <param name="value">Width in pixels to set.</param>
    void SetColumnWidthInPixels( int iColumnIndex, int value );
      /// <summary>
      /// Set Column width from start Column index to End Column index
      /// </summary>
    /// <param name="iStartColumnIndex">start Column index</param>
    /// <param name="iCount">No of Column to be set width</param>
      /// <param name="value">Value in pixel to set</param>
    void SetColumnWidthInPixels(int iStartColumnIndex, int iCount, int value);
    /// <summary>
    /// Sets row height.
    /// </summary>
    /// <param name="iRow">One-based row index.</param>
    /// <param name="value">Height to set.</param>
    void SetRowHeight( int iRow, double value );
    /// <summary>
    /// Sets row height in pixels.
    /// </summary>
    /// <param name="iRowIndex">One-based row index to set height.</param>
    /// <param name="value">Value in pixels to set.</param>
    void SetRowHeightInPixels( int iRowIndex, double value );
      /// <summary>
      /// Set row height
      /// </summary>
      /// <param name="iStartRowIndex">Start index of Row</param>
    /// <param name="iEndRowIndex">No of Row to be set width</param>
      /// <param name="value"></param>
    void SetRowHeightInPixels(int iStartRowIndex, int iCount, double value);
    /// <summary>
    /// Returns width from ColumnInfoRecord if there is corresponding ColumnInfoRecord
    /// or StandardWidth if not.
    /// </summary>
    /// <param name="iColumnIndex">One-based index of the column.</param>
    /// <returns>Width of the specified column.</returns>
    double GetColumnWidth( int iColumnIndex );
    /// <summary>
    /// Returns width in pixels from ColumnInfoRecord if there is corresponding ColumnInfoRecord
    /// or StandardWidth if not.
    /// </summary>
    /// <param name="iColumnIndex">One-based index of the column.</param>
    /// <returns>Width in pixels of the specified column.</returns>
    int GetColumnWidthInPixels( int iColumnIndex );
    /// <summary>
    /// Returns height from RowRecord if there is a corresponding RowRecord.
    /// Otherwise returns StandardHeight. 
    /// </summary>
    /// <param name="iRow">One-based index of the row</param>
    /// <returns>
    /// Height from RowRecord if there is corresponding RowRecord.
    /// Otherwise returns StandardHeight.
    /// </returns>
    double GetRowHeight( int iRow );
    /// <summary>
    /// Returns height from RowRecord if there is a corresponding RowRecord.
    /// Otherwise returns StandardHeight. 
    /// </summary>
    /// <param name="iRowIndex">One-based index of the row.</param>
    /// <returns>
    /// Height in pixels from RowRecord if there is corresponding RowRecord.
    /// Otherwise returns StandardHeight.
    /// </returns>
    int GetRowHeightInPixels( int iRowIndex );
    /// <summary>
    /// This method searches for the first cell with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    IRange FindFirst( string findValue, ExcelFindType flags );
    /// <summary>
    /// This method searches for the first cell with specified string value based on the Excelfindoptions
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <param name="findOptions">Way to search the value.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    IRange FindFirst(string findValue, ExcelFindType flags,ExcelFindOptions findOptions);
    /// <summary>
    /// This method searches for the first cell that starts with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    IRange FindStringStartsWith(string findValue, ExcelFindType flags);
    /// <summary>
    /// This method searches for the first cell that starts with specified string value which igonres the case.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <param name="ignoreCase">true to ignore case wen comparing this string to the value;otherwise,false</param>
    /// <returns>First found cell, or Null if value was not found.</returns>       
    IRange FindStringStartsWith(string findValue, ExcelFindType flags, bool ignoreCase);
    /// <summary>
    /// This method searches for the first cell that ends  with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    IRange FindStringEndsWith(string findValue, ExcelFindType flags);
    /// <summary>
    /// This method searches for the first cell that ends with specified string value which igonres the case.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <param name="ignoreCase">true to ignore case wen comparing this string to the value;otherwise,false</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    IRange FindStringEndsWith(string findValue, ExcelFindType flags, bool ignoreCase);
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
    /// This method searches for the all cells with specified string value based on the Excel find options.
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
#if !(WINRT )
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="fileName">File to save.</param>
    /// <param name="separator">Current separator.</param>
    void SaveAs( string fileName, string separator );
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="fileName">File to save.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use.</param>
    void SaveAs( string fileName, string separator, Encoding encoding );
#endif
#if ( WINRT )
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="storageFile">StorageFile to save. </param>
    /// <param name="separator">Current separator.</param>
    Task<bool> SaveAsAsync(StorageFile storageFile, string separator);
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="storageFile">StorageFile to save. </param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use.</param>
    Task<bool> SaveAsAsync(StorageFile storageFile, string separator, Encoding encoding);
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="stream">Stream to save. </param>
    /// <param name="separator">Current separator.</param>
    Task<bool> SaveAsAsync(Stream stream, string separator);
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="stream">Stream to save. </param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use.</param>
    Task<bool> SaveAsAsync(Stream stream, string separator, Encoding encoding);
#else
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="stream">Stream to save. </param>
    /// <param name="separator">Current separator.</param>
    void SaveAs( Stream stream, string separator );
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="stream">Stream to save. </param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use.</param>
    void SaveAs( Stream stream, string separator, Encoding encoding );
#endif
    /// <summary>
    /// Sets by column index default style for column.
    /// </summary>
    /// <param name="iColumnIndex">Column index.</param>
    /// <param name="defaultStyle">Default style.</param>
    void SetDefaultColumnStyle( int iColumnIndex, IStyle defaultStyle );
    /// <summary>
    /// Sets by column index default style for column.
    /// </summary>
    /// <param name="iStartColumnIndex">Start column index.</param>
    /// <param name="iEndColumnIndex">End column index.</param>
    /// <param name="defaultStyle">Default style.</param>
    void SetDefaultColumnStyle( int iStartColumnIndex, int iEndColumnIndex,
      IStyle defaultStyle );
    /// <summary>
    /// Sets by column index default style for row.
    /// </summary>
    /// <param name="iRowIndex">Row index.</param>
    /// <param name="defaultStyle">Default style.</param>
    void SetDefaultRowStyle( int iRowIndex, IStyle defaultStyle );
    /// <summary>
    /// Sets by column index default style for row.
    /// </summary>
    /// <param name="iStartRowIndex">Start row index.</param>
    /// <param name="iEndRowIndex">End row index.</param>
    /// <param name="defaultStyle">Default style.</param>
    void SetDefaultRowStyle( int iStartRowIndex, int iEndRowIndex,
      IStyle defaultStyle );
    /// <summary>
    /// Returns default column style.
    /// </summary>
    /// <param name="iColumnIndex">Column index.</param>
    /// <returns>Default column style or null if style wasn't set.</returns>
    IStyle GetDefaultColumnStyle( int iColumnIndex );
    /// <summary>
    /// Returns default row style.
    /// </summary>
    /// <param name="iRowIndex">Row index.</param>
    /// <returns>Default row style or null if style wasn't set.</returns>
    IStyle GetDefaultRowStyle( int iRowIndex );
    /// <summary>
    /// Free's range object.
    /// </summary>
    /// <param name="range">Range to remove from internal cache.</param>
    void FreeRange( IRange range );
    /// <summary>
    /// Free's range object.
    /// </summary>
    /// <param name="iRow">One-based row index of the range object to remove from internal cache.</param>
    /// <param name="iColumn">One-based column index of the range object to remove from internal cache.</param>
    void FreeRange( int iRow, int iColumn );
    /// <summary>
    /// Sets value in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Value to set.</param>
    void SetValue( int iRow, int iColumn, string value );
    /// <summary>
    /// Sets value in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Value to set.</param>
    void SetNumber( int iRow, int iColumn, double value );
    /// <summary>
    /// Sets value in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Value to set.</param>
    void SetBoolean( int iRow, int iColumn, bool value );
    /// <summary>
    /// Sets text in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Text to set.</param>
    void SetText( int iRow, int iColumn, string value );
    /// <summary>
    /// Sets formula in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Formula to set.</param>
    void SetFormula( int iRow, int iColumn, string value );
    /// <summary>
    /// Sets error in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Error to set.</param>
    void SetError( int iRow, int iColumn, string value );
    /// <summary>
    /// Sets blank in specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    void SetBlank( int iRow, int iColumn );
    /// <summary>
    /// Sets formula number value.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iColumn">One based column index.</param>
    /// <param name="value">Represents formula number value for set.</param>
    void SetFormulaNumberValue( int iRow, int iColumn, double value );
    /// <summary>
    /// Sets formula error value.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iColumn">One based column index.</param>
    /// <param name="value">Represents formula error value for set.</param>
    void SetFormulaErrorValue( int iRow, int iColumn, string value );
    /// <summary>
    /// Sets formula bool value.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iColumn">One based column index.</param>
    /// <param name="value">Represents formula bool value for set.</param>
    void SetFormulaBoolValue( int iRow, int iColumn, bool value );
    /// <summary>
    /// Sets formula string value.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iColumn">One based column index.</param>
    /// <param name="value">Represents formula string value for set.</param>
    void SetFormulaStringValue( int iRow, int iColumn, string value );
    /// <summary>
    /// Returns instance of migrant range - row and column of this range
    /// object can be changed by user. Read-only.
    /// </summary>
    IMigrantRange MigrantRange { get; }

    /// <summary>
    /// Returns string value corresponding to the cell.
    /// </summary>
    /// <param name="row">One-based row index of the cell to get value from.</param>
    /// <param name="column">One-based column index of the cell to get value from.</param>
    /// <returns>String contained by the cell.</returns>
    string GetText( int row, int column );
    /// <summary>
    /// Returns number value corresponding to the cell.
    /// </summary>
    /// <param name="row">One-based row index of the cell to get value from.</param>
    /// <param name="column">One-based column index of the cell to get value from.</param>
    /// <returns>Number contained by the cell.</returns>
    double GetNumber( int row, int column );
    /// <summary>
    /// Returns formula value corresponding to the cell.
    /// </summary>
    /// <param name="row">One-based row index of the cell to get value from.</param>
    /// <param name="column">One-based column index of the cell to get value from.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation should be used.</param>
    /// <returns>Formula contained by the cell.</returns>
    string GetFormula( int row, int column, bool bR1C1 );
    /// <summary>
    /// Gets error value from cell.
    /// </summary>
    /// <param name="row">Row index.</param>
    /// <param name="column">Column index.</param>
    /// <returns>Returns error value or null.</returns>
    string GetError( int row, int column );
    /// <summary>
    /// Gets bool value from cell.
    /// </summary>
    /// <param name="row">Represents row index.</param>
    /// <param name="column">Represents column index.</param>
    /// <returns>Returns found bool value. If cannot found returns false.</returns>
    bool GetBoolean( int row, int column );
    /// <summary>
    /// Gets formula bool value from cell.
    /// </summary>
    /// <param name="row">Represents row index.</param>
    /// <param name="column">Represents column index.</param>
    /// <returns>Returns found bool value. If cannot found returns false.</returns>
    bool GetFormulaBoolValue( int row, int column );
    /// <summary>
    /// Gets formula error value from cell.
    /// </summary>
    /// <param name="row">Row index.</param>
    /// <param name="column">Column index.</param>
    /// <returns>Returns error value or null.</returns>
    string GetFormulaErrorValue( int row, int column );
    /// <summary>
    /// Returns formula number value corresponding to the cell.
    /// </summary>
    /// <param name="row">One-based row index of the cell to get value from.</param>
    /// <param name="column">One-based column index of the cell to get value from.</param>
    /// <returns>Number contained by the cell.</returns>
    double GetFormulaNumberValue( int row, int column );
    /// <summary>
    /// Returns formula string value corresponding to the cell.
    /// </summary>
    /// <param name="row">One-based row index of the cell to get value from.</param>
    /// <param name="column">One-based column index of the cell to get value from.</param>
    /// <returns>String contained by the cell.</returns>
    string GetFormulaStringValue( int row, int column );
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Converts range into image (Bitmap).
    /// </summary>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <returns>Created image.</returns>
    Image ConvertToImage( int firstRow, int firstColumn, int lastRow, int lastColumn );
    /// <summary>
    /// Converts range into image.
    /// </summary>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <param name="imageType">Type of the image to create.</param>
    /// <param name="stream">Output stream. It is ignored if null.</param>
    /// <returns>Created image.</returns>
    Image ConvertToImage( int firstRow, int firstColumn, int lastRow, int lastColumn,
      ImageType imageType, Stream stream );

      void SaveAsHtml(string filename);

      void SaveAsHtml(Stream stream);

    void SaveAsHtml(string filename,HtmlSaveOptions saveOptions);

    void SaveAsHtml(Stream stream,HtmlSaveOptions saveOptions);
    /// <summary>
    /// Converts range into metafile image.
    /// </summary>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <param name="emfType">Metafile EmfType.</param>
    /// <param name="outputStream">Output stream. It is ignored if null.</param>
    /// <returns>Created image.</returns>
    Image ConvertToImage( int firstRow, int firstColumn, int lastRow, int lastColumn,
      EmfType emfType, Stream outputStream );
    /// <summary>
    /// Converts range into image.
    /// </summary>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <param name="imageType">Type of the image to create.</param>
    /// <param name="outputStream">Output stream. It is ignored if null.</param>
    /// <param name="emfType">Metafile EmfType.</param>
    /// <returns>Created image.</returns>
    Image ConvertToImage( int firstRow, int firstColumn, int lastRow, int lastColumn,
      ImageType imageType, Stream outputStream, EmfType emfType );
#endif
    #endregion
    event Syncfusion.XlsIO.Implementation.RangeImpl.CellValueChangedEventHandler CellValueChanged;
  }
}
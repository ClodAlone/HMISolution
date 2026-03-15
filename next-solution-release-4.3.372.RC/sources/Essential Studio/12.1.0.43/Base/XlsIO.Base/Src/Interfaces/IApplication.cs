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
using System.Globalization;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.Compression;
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents the Excel application.
  /// </summary>
  public interface IApplication
    : IParentApplication
  {
    #region Not Supported properties/methods
#if NOT_SUPPORTED
/*
    AddIns AddIns { get; }
    AnswerWizard AnswerWizard { get; }
    Assistant Assistant { get; }
    AutoCorrect AutoCorrect { get; }
    AutoRecover AutoRecover { get; }
    COMAddIns COMAddIns { get; }
    Chart ActiveChart { get; }
    CommandBars CommandBars { get; }
    DefaultWebOptions DefaultWebOptions { get; }
    DialogSheet ActiveDialog { get; }
    Dialogs Dialogs { get; }
    FileDialog FileDialog { get; }
    FileSearch FileSearch { get; }
    IFind FileFind { get; }
    LanguageSettings LanguageSettings { get; }
    MenuBar ActiveMenuBar { get; }
    MenuBars MenuBars { get; }
    MsoAutomationSecurity AutomationSecurity { get; set; }
    MsoFeatureInstall FeatureInstall { get; set; }
    ODBCErrors ODBCErrors { get; }
    OLEDBErrors OLEDBErrors { get; }
    Sheets Excel4IntlMacroSheets { get; }
    Sheets Excel4MacroSheets { get; }
    Window ActiveWindow { get; }
    XlCalculation Calculation { get; set; }
    XlCalculationInterruptKey CalculationInterruptKey { get; set; }
    XlCalculationState CalculationState { get; }
    XlCommandUnderlines CommandUnderlines { get; set; }
    XlCommentDisplayMode DisplayCommentIndicator { get; set; }
    XlCreator Creator { get; }
    XlCutCopyMode CutCopyMode { get; set; }
    XlEnableCancelKey EnableCancelKey { get; set; }
    XlFileFormat DefaultSaveFormat { get; set; }
    XlMailSystem MailSystem { get; }
    XlMousePointer Cursor { get; set; }
    bool AskToUpdateLinks { get; set; }
    bool AutoFormatAsYouTypeReplaceHyperlinks { get; set; }
    bool AutoPercentEntry { get; set; }
    bool CanPlaySounds { get; }
    bool CanRecordSounds { get; }
    bool CellDragAndDrop { get; set; }
    bool ColorButtons { get; set; }
    bool DisplayAlerts { get; set; }
    bool DisplayClipboardWindow { get; set; }
    bool DisplayExcel4Menus { get; set; }
    bool DisplayFormulaBar { get; set; }
    bool DisplayFullScreen { get; set; }
    bool DisplayFunctionToolTips { get; set; }
    bool DisplayInfoWindow { get; set; }
    bool DisplayInsertOptions { get; set; }
    bool DisplayNoteIndicator { get; set; }
    bool DisplayPasteOptions { get; set; }
    bool DisplayRecentFiles { get; set; }
    bool DisplayScrollBars { get; set; }
    bool DisplayStatusBar { get; set; }
    bool EnableAnimations { get; set; }
    bool EnableAutoComplete { get; set; }
    bool EnableEvents { get; set; }
    bool EnableSound { get; set; }
    bool EnableTipWizard { get; set; }
    bool IgnoreRemoteRequests { get; set; }
    bool Interactive { get; set; }
    bool Iteration { get; set; }
    bool LargeButtons { get; set; }
    bool MathCoprocessorAvailable { get; }
    double Height { get; set; }
    double Left { get; set; }
    int CalculationVersion { get; }
    int CursorMovement { get; set; }
    int CustomListCount { get; }
    int DDEAppReturnCode { get; }
    int DataEntryMode { get; set; }
    int DefaultSheetDirection { get; set; }
    int Hinstance { get; }
    int Hwnd { get; }
    int ODBCTimeout { get; set; }
    object Caller { get; }
    object ClipboardFormats { get; }
    object Dummy101 { get; }
    object FileConverters { get; }
    object International { get; }
    object MailSession { get; }
    string ActivePrinter { get; set; }
    string AltStartupPath { get; set; }
    string OnCalculate { get; set; }
    string OnData { get; set; }
    string OnDoubleClick { get; set; }
    string OnEntry { get; set; }
    string OnSheetActivate { get; set; }
    string OnSheetDeactivate { get; set; }
    string OnWindow { get; set; }
    string OperatingSystem { get; }
    string OrganizationName { get; }
    string _Default { get; }
    ErrorCheckingOptions ErrorCheckingOptions { get; }
    Menu ShortcutMenus { get; }
    Modules Modules { get; }
    NewFile NewWorkbook { get; }
    RTD RTD { get; }
    Range ThisCell { get; }
    RecentFiles RecentFiles { get; }
    Sheets Charts { get; }
    Sheets DialogSheets { get; }
    SmartTagRecognizers SmartTagRecognizers { get; }
    Speech Speech { get; }
    SpellingOptions SpellingOptions { get; }
    Toolbars Toolbars { get; }
    UsedObjects UsedObjects { get; }
    VBE VBE { get; }
    Watches Watches { get; }
    Workbook ThisWorkbook { get; }
    XlDirection MoveAfterReturnDirection { get; set; }
    XlReferenceStyle ReferenceStyle { get; set; }
    XlWindowState WindowState { get; set; }
    /// <summary>
    /// For an Application object, returns a Windows collection that represents
    /// all the windows in all the workbooks. For a Workbook object, returns a
    /// Windows collection that represents all the windows in the specified
    /// workbook. Read-only Windows object.
    /// </summary>
    Windows Windows { get; }
    /// <summary>
    /// True if workbooks are calculated before they're saved to disk (if the
    /// Calculation property is set to xlManual). This property is preserved
    /// even if you change the Calculation property. Read/write Boolean.
    /// </summary>
    bool CalculateBeforeSave { get; set; }
    bool ConstrainNumeric { get; set; }
    bool ControlCharacters { get; set; }
    /// <summary>
    /// ???
    /// </summary>
    bool CopyObjectsWithCells { get; set; }
    /// <summary>
    /// True if Microsoft Excel displays a message before overwriting nonblank
    /// cells during a drag-and-drop editing operation. Read / write Boolean.
    /// </summary>
    bool AlertBeforeOverwriting { get; set; }
    bool EditDirectlyInCell { get; set; }
    bool ExtendList { get; set; }
    bool GenerateGetPivotData { get; set; }
    bool MapPaperSize { get; set; }
    bool MouseAvailable { get; }
    bool MoveAfterReturn { get; set; }
    bool PivotTableSelection { get; set; }
    bool PromptForSummaryInfo { get; set; }
    bool Ready { get; }
    bool RecordRelative { get; }
    bool RollZoom { get; set; }
    bool ScreenUpdating { get; set; }
    bool ShowChartTipNames { get; set; }
    bool ShowChartTipValues { get; set; }
    bool ShowStartupDialog { get; set; }
    bool ShowToolTips { get; set; }
    bool ShowWindowsInTaskbar { get; set; }
    bool TransitionNavigKeys { get; set; }
    bool UserControl { get; set; }
    bool Visible { get; set; }
    bool WindowsForPens { get; }
    double MaxChange { get; set; }
    double Top { get; set; }
    double UsableHeight { get; }
    double UsableWidth { get; }
    double Width { get; set; }
    int MaxIterations { get; set; }
    int MemoryFree { get; }
    int MemoryTotal { get; }
    int MemoryUsed { get; }
    int TransitionMenuKeyAction { get; set; }
    int UILanguage { get; set; }
    object PreviousSelections { get; }
    object RegisteredFunctions { get; }
    object StatusBar { get; set; }
    string Caption { get; set; }
    string LibraryPath { get; }
    string NetworkTemplatesPath { get; }
    string ProductCode { get; }
    string TemplatesPath { get; }
    string TransitionMenuKey { get; set; }
    string UserLibraryPath { get; }

    object ConvertFormula(object Formula, Excel.XlReferenceStyle FromReferenceStyle, object ToReferenceStyle, object ToAbsolute, object RelativeTo);
    object _Evaluate(object Name);
    void _FindFile();
    object _Run2(object Macro, object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    void _Wait(object Time);
    object _WSFunction(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    void ActivateMicrosoftApp(Excel.XlMSApplication Index);
    void AddChartAutoFormat(object Chart, string Name, object Description);
    void AddCustomList(object ListArray, object ByRow);
    void Calculate();
    void CalculateFull();
    void CalculateFullRebuild();
    void CheckAbort(object KeepAbort);
    bool CheckSpelling(string Word, object CustomDictionary, object IgnoreUppercase);
    void DDEExecute(int Channel, string String);
    int DDEInitiate(string App, string Topic);
    void DDEPoke(int Channel, object Item, object Data);
    object DDERequest(int Channel, string Item);
    void DDETerminate(int Channel);
    void DeleteChartAutoFormat(string Name);
    void DeleteCustomList(int ListNum);
    void DoubleClick();
    object Dummy1(object Arg1, object Arg2, object Arg3, object Arg4);
    bool Dummy10(object arg);
    void Dummy11();
    void Dummy12(Excel.PivotTable p1, Excel.PivotTable p2);
    object Dummy13(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    void Dummy14();
    object Dummy2(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8);
    object Dummy3();
    object Dummy4(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15);
    object Dummy5(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13);
    object Dummy6();
    object Dummy7();
    object Dummy8(object Arg1);
    object Dummy9();
    object Evaluate(object Name);
    object ExecuteExcel4Macro(string String);
    bool FindFile();
    object GetCustomListContents(int ListNum);
    int GetCustomListNum(object ListArray);
    object GetOpenFilename(object FileFilter, object FilterIndex, object Title, object ButtonText, object MultiSelect);
    string GetPhonetic(object Text);
    object GetSaveAsFilename(object InitialFilename, object FileFilter, object FilterIndex, object Title, object ButtonText);
    void Goto(object Reference, object Scroll);
    void Help(object HelpFile, object HelpContextID);
    object InputBox(string Prompt, object Title, object Default, object Left, object Top, object HelpFile, object HelpContextID, object Type);
    Excel.Range Intersect(Excel.Range Arg1, Excel.Range Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    void MacroOptions(object Macro, object Description, object HasMenu, object MenuText, object HasShortcutKey, object ShortcutKey, object Category, object StatusBar, object HelpContextID, object HelpFile);
    void MailLogoff();
    void MailLogon(object Name, object Password, object DownloadNewMail);
    Excel.Workbook NextLetter();
    void OnKey(string Key, object Procedure);
    void OnRepeat(string Text, string Procedure);
    void OnTime(object EarliestTime, string Procedure, object LatestTime, object Schedule);
    void OnUndo(string Text, string Procedure);
    void RecordMacro(object BasicCode, object XlmCode);
    bool RegisterXLL(string Filename);
    void Repeat();
    void ResetTipWizard();
    object Run(object Macro, object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    void SendKeys(object Keys, object Wait);
    void SetDefaultChart(object FormatName, object Gallery);
    void Undo();
    Excel.Range Union(Excel.Range Arg1, Excel.Range Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    void Volatile(object Volatile);
    bool Wait(object Time);
*/
#endif
    #endregion

    #region Skipped
#if SKIPPED
/*
    /// <summary>
    /// For an Application object, returns a Range object that represents all
    /// the rows on the active worksheet. If the active document isn't a worksheet,
    /// the Rows property fails. For a Range object, returns a Range object that
    /// represents the rows in the specified range. For a Worksheet object, returns
    /// a Range object that represents all the rows on the specified worksheet.
    /// Read-only Range object.
    /// </summary>
    IRange            Rows { get; }
    /// <summary>
    /// Returns a Range object that represents all the cells on the
    /// active worksheet. If the active document isn't a worksheet, this
    /// property fails. Read-only.
    /// </summary>
    IRange            Cells { get; }
    /// <summary>
    /// Returns a Range object that represents all the columns on the
    /// active worksheet. If the active document isn't a worksheet, the
    /// Columns property fails. Read-only.
    /// </summary>
    IRange            Columns { get; }
    /// <summary>
    /// Returns the WorksheetFunction object. Read-only
    /// </summary>
    WorksheetFunction WorksheetFunction { get; }
    /// <summary>
    /// Returns a Sheets collection that represents all the sheets in the
    /// active workbook, for an Application object. Returns a Sheets collection
    /// that represents all the sheets in the specified workbook, for a Workbook
    /// object. Read-only Sheets object.
    /// </summary>
    ISheets           Sheets { get; }
    /// <summary>
    /// A collection of all the Name objects in the application or workbook.
    /// Each Name object represents a defined name for a range of cells.
    /// </summary>
    INames            Names { get; }
    /// <summary>
    /// Returns or sets the name of the object. Read-only String.
    /// </summary>
    string            Name { get; }
    /// <summary>
    /// Sets or returns the search criteria for the type of cell formats to find.
    /// </summary>
    ICellFormat       FindFormat { get; set; }
    /// <summary>
    /// Quits Microsoft Excel.
    /// </summary>
    void Quit();
    /// <summary>
    /// Sets the replacement criteria to use in replacing cell formats.
    /// </summary>
    ICellFormat       ReplaceFormat { get; set; }
    /// <summary>
    /// Returns the selected object in the active window for an Application
    /// object, and a specified window for a Windows object.
    /// </summary>
    object            Selection { get; }
    /// <summary>
    /// Returns the complete path of the startup folder, excluding the
    /// final separator. Read-only String.
    /// </summary>
    string            StartupPath { get; }
    /// <summary>
    /// Saves the current workspace.
    /// </summary>
    /// <param name="Filename">File name of result file.</param>
    void SaveWorkspace( string Filename );
*/
#endif
    #endregion

    #region Interface Properties
    /// <summary>
    /// Returns a Range object that represents the active cell in the
    /// active window (the window on top) or in the specified window.
    /// If the window isn't displaying a worksheet, this property fails.
    /// Read-only.
    /// </summary>
    IRange            ActiveCell { get; }
    /// <summary>
    /// Returns an object that represents the active sheet (the sheet on
    /// top) in the active workbook or in the specified window or workbook.
    /// Returns Nothing if no sheet is active. Read-only.
    /// </summary>
    IWorksheet        ActiveSheet { get; }
    /// <summary>
    /// Returns a Workbook object that represents the workbook in the active
    /// window (the window on top). Read-only. Returns Nothing if there are
    /// no windows open or if either the Info window or the Clipboard window
    /// is the active window.
    /// </summary>
    IWorkbook         ActiveWorkbook { get; }
    /// <summary>
    /// Returns a Workbooks collection that represents all the open workbooks.
    /// Read-only.
    /// </summary>
    IWorkbooks        Workbooks { get; }
    /// <summary>
    /// For an Application object, returns a Sheets collection that represents
    /// all the worksheets in the active workbook. For a Workbook object,
    /// returns a Sheets collection that represents all the worksheets in the
    /// specified workbook. Read-only Sheets object.
    /// </summary>
    IWorksheets       Worksheets { get; }
    /// <summary>
    /// Returns a Range object that represents a cell or a range of cells.
    /// </summary>
    IRange            Range { get; }
    /// <summary>
    /// All data entered after this property is set to True will be formatted
    /// with the number of fixed decimal places set by the FixedDecimalPlaces
    /// property. Read/write Boolean.
    /// </summary>
    bool              FixedDecimal { get; set; }
    /// <summary>
    /// True (default) if the system separators of Microsoft Excel are
    /// enabled. Read/write Boolean.
    /// </summary>
    bool              UseSystemSeparators { get; set; }
    /// <summary>
    /// Returns the Microsoft Excel build number. Read-only.
    /// </summary>
    int               Build { get; }
    /// <summary>
    /// Returns or sets the number of fixed decimal places used when
    /// the FixedDecimal property is set to True. Read/write.
    /// </summary>
    int               FixedDecimalPlaces { get; set; }
    /// <summary>
    /// Returns or sets the number of sheets that Microsoft Excel
    /// automatically inserts into new workbooks. Read/write Long.
    /// </summary>
    int               SheetsInNewWorkbook { get; set; }
    /// <summary>
    /// Sets or returns the character used for the decimal separator as a
    /// String. Read/write.
    /// </summary>
    string            DecimalSeparator { get; set; }
    /// <summary>
    /// Returns or sets the default path that Microsoft Excel uses when it
    /// opens files. Read/write String.
    /// </summary>
    string            DefaultFilePath { get; 
#if !SILVERLIGHT && !WINRT && !WP
        set;
#endif
        }
    ///// <summary>
    ///// Returns the complete path to the application, excluding the
    ///// final separator and name of the application. Read-only String.
    ///// </summary>
    //string            Path { get; }
#if !(WINRT )
    /// <summary>
    /// Returns the path separator character ("\"). Read-only String.
    /// </summary>
    string            PathSeparator { get; }
#endif
    /// <summary>
    /// Sets or returns the character used for the thousands separator
    /// as a String. Read / write.
    /// </summary>
    string            ThousandsSeparator { get; set; }
    /// <summary>
    /// Returns or sets the name of the current user. Read/write String.
    /// </summary>
    string            UserName { get; set; }
    /// <summary>
    /// For the Application object, always returns "Microsoft Excel". For
    /// the CubeField object, the name of the specified field. For the Style
    /// object, the name of the specified style. Read-only String.
    /// </summary>
    string            Value { get; }
    /// <summary>
  /// If this property is set to True, then if some cells have reference 
  /// to the same style, changes will influence all these cells.
    /// Default value: False
    /// </summary>
    bool              ChangeStyleOnCellEdit{ get; set; }
    /// <summary>
    /// This flag controls behavior of workbook save methods. Each flag controls 
    /// one aspect of the save code. Can be set one or more flags which will influence 
    /// the output produced by the library.
    /// </summary>
    SkipExtRecords    SkipOnSave{ get; set; }
    /// <summary>
    /// Returns or sets the standard (default) height of all the rows in the worksheet,
    /// in points. This value is used only for newly created worksheets.
    /// Read/write Double.
    /// </summary>
    double            StandardHeight { get; set; }
    /// <summary>
    /// Returns or sets the standard (default) height option flag, which defines that
    /// standard (default) row height and book default font height do not match.
    /// This value is used only for newly created worksheets. Read/write Bool.
    /// </summary>
    bool              StandardHeightFlag { get; set; }
    /// <summary>
    /// Returns or sets the standard (default) width of all the columns in the
    /// worksheet. This value is used only for newly created worksheets.
    /// Read/write Double.
    /// </summary>
    double            StandardWidth { get; set; }
    /// <summary>
    /// Indicates whether to optimize fonts count. This option will
    /// take effect only on workbooks that will be added after setting
    /// this property.
    /// WARNING: Setting this property to True can decrease performance significantly,
    /// but will reduce resulting file size.
    /// </summary>
    bool              OptimizeFonts { get; set; }
    /// <summary>
    /// Indicates whether to optimize Import data. This option will
    /// take effect only on Import methods that are available with the worksheet
    /// WARNING: Setting this property to True can decrease memory significantly,
    /// but will increase the performance of data import .
    /// </summary>    
    bool             OptimizeImport { get; set; }
    /// <summary>
    /// Gets / sets row separator for array parsing.
    /// </summary>
    char              RowSeparator { get; set; }
    /// <summary>
    /// Formula arguments separator.
    /// </summary>
    char              ArgumentsSeparator { get; set; }
    /// <summary>
    /// Represents CSV Separator. Using for Auto recognize file type.
    /// </summary>
    string            CSVSeparator { get; set; }
    /// <summary>
    /// Returns or sets the name of the standard font. Read/write String.
    /// </summary>
    string            StandardFont { get; set; }
    /// <summary>
    /// Returns or sets the standard font size, in points. Read/write.
    /// </summary>
    double            StandardFontSize { get; set; }
    /// <summary>
    /// Indicates is use unsafe code.
    /// </summary>
    [ Obsolete( "Use DataProviderType property instead" ) ]
    bool              UseNativeOptimization{ get; set; }
    /// <summary>
    /// Indicates whether to try fast record parsing.
    /// </summary>
    bool UseFastRecordParsing { get; set; }
    /// <summary>
    /// Gets / sets memory allocation block for single row. Each row will allocate memory block
    /// that can be divided on this number. Smaller value means smaller memory usage but slower
    /// speed when changing cell's value. Default value is 128. That is enough to allocate 9 string records,
    /// or 9 integer numbers (or floating numbers with 1 or 2 digits after decimal point) or 7 double numbers.
    /// </summary>
    int RowStorageAllocationBlockSize { get; set; }
    /// <summary>
    /// Indicates whether XlsIO should delete destination file before saving into it.
    /// Default value is TRUE.
    /// </summary>
    bool DeleteDestinationFile { get; set; }
    /// <summary>
    /// Gets / sets default excel version. This value is used in create methods.
    /// </summary>
    ExcelVersion DefaultVersion { get; set; }
    /// <summary>
    /// Indicates whether we should use native storage (standard windows COM object)
    /// or our .Net implementation to open excel 97-2003 files.
    /// </summary>
    bool UseNativeStorage { get; set; }
    /// <summary>
    /// Changes data provider type for all operations after it.
    /// </summary>
    ExcelDataProviderType DataProviderType { get; set; }
    /// <summary>
    /// Compression level for workbooks serialization.
    /// </summary>
    CompressionLevel? CompressionLevel { get; set; }
    /// <summary>
    /// Indicates whether to preserve the datatypes for the CSV file formats.
    /// </summary>  
    bool PreserveCSVDataTypes { get; set; }
    #if ((SyncfusionFramework4_0 || SyncfusionFramework4_5) && !SILVERLIGHT && !WINRT && !WP)
    /// <summary>
    /// Represents the chart to image converter instance.
    /// </summary>
    IChartToImageConverter ChartToImageConverter { get; set; }
    #endif
    #endregion

    #region Interface utility methods
    /// <summary>
    /// Converts a measurement from centimeters to points
    /// (one point equals 0.035 centimeters).
    /// </summary>
    /// <param name="Centimeters"></param>
    /// <returns></returns>
    double CentimetersToPoints( double Centimeters );
    /// <summary>
    /// Converts a measurement from inches to points.
    /// </summary>
    /// <param name="Inches"></param>
    /// <returns></returns>
    double InchesToPoints( double Inches );
#if !(WINRT || WP)
    /// <summary>
    /// Saves changes to the active workbook.
    /// </summary>
    /// <param name="Filename">File name of result file.</param>
    void Save( string Filename );
#endif
    /// <summary>
    /// Converts units.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="from">Form option.</param>
    /// <param name="to">To option.</param>
    /// <returns>Returns converted result.</returns>
    double ConvertUnits( double value, MeasureUnits from, MeasureUnits to );
    #endregion

    #region Interface events
    /// <summary>
    /// When workbook is read from stream and position of the stream changed,
    /// this event is raised.
    /// </summary>
    event ProgressEventHandler ProgressEvent;
    /// <summary>
    /// This event is fired when user tries to open password protected workbook
    /// without specifying password. It is used to obtain password.
    /// </summary>
    event PasswordRequiredEventHandler OnPasswordRequired;
    /// <summary>
    /// This event is fired when user specified wrong password when trying to open
    /// password protected workbook. It is used to obtain correct password.
    /// </summary>
    event PasswordRequiredEventHandler OnWrongPassword;
    #endregion
  }
  /// <summary>
  /// Event arguments for notifying read progress.
  /// </summary>
  public class ProgressEventArgs: EventArgs
  {
    #region Class members
    /// <summary>
    /// Current read position.
    /// </summary>
    private long m_lPosition;
    /// <summary>
    /// Full stream size.
    /// </summary>
    private long m_lSize;
    #endregion

    #region Class properties
    /// <summary>
    /// Current read position.
    /// </summary>
    public long Position
    {
      get
      {
        return m_lPosition;
      }
    }
    /// <summary>
    /// Full stream size.
    /// </summary>
    public long FullSize
    {
      get
      {
        return m_lSize;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance of the event arguments.
    /// </summary>
    /// <param name="curPos">Current read position.</param>
    /// <param name="fullSize">Full stream size.</param>
    public ProgressEventArgs( long curPos, long fullSize )
    {
      m_lPosition = curPos;
      m_lSize = fullSize;
    }
    #endregion

  }
  /// <summary>
  /// Progress event handler delegate
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public delegate void ProgressEventHandler( object sender, ProgressEventArgs args );
}

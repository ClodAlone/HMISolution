#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.IO;
using System.Xml;
using Syncfusion.XlsIO.Implementation;

#if ( WINRT || WP)
using System.Threading.Tasks;
using Windows.Storage;
#endif

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#endif

#if !SILVERLIGHT && !WINRT && !WP
using System.Drawing;
using System.Web;
using System.Data;
using Syncfusion.XlsIO.Interfaces;
#endif


namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents an MS Excel Workbook.
  /// </summary>
  public interface IWorkbook : IParentApplication
  {
    #region Not supported properties/methods
#if NOT_SUPPORTED
/*
    string _CodeName { get; set; }
    bool _ReadOnlyRecommended { get; }
    bool AcceptLabelsInFormulas { get; set; }
    Chart ActiveChart { get; }
    int AutoUpdateFrequency { get; set; }
    bool AutoUpdateSaveChanges { get; set; }
    object BuiltinDocumentProperties { get; }
    int CalculationVersion { get; }
    int ChangeHistoryDuration { get; set; }
    Sheets Charts { get; }
    string CodeName { get; }
    CommandBars CommandBars { get; }
    XlSaveConflictResolution ConflictResolution { get; set; }
    object Container { get; }
    bool CreateBackup { get; }
    XlCreator Creator { get; }
    CustomViews CustomViews { get; }
    Sheets DialogSheets { get; }
    XlDisplayDrawingObjects DisplayDrawingObjects { get; set; }
    bool EnableAutoRecover { get; set; }
    bool EnvelopeVisible { get; set; }
    Sheets Excel4IntlMacroSheets { get; }
    Sheets Excel4MacroSheets { get; }
    XlFileFormat FileFormat { get; }
    bool HasMailer { get; set; }
    bool HasPassword { get; }
    bool HasRoutingSlip { get; set; }
    bool HighlightChangesOnScreen { get; set; }
    HTMLProject HTMLProject { get; }
    bool IsAddin { get; set; }
    bool IsInplace { get; }
    bool KeepChangeHistory { get; set; }
    string Keywords { get; set; }
    bool ListChangesOnNewSheet { get; set; }
    Mailer Mailer { get; }
    Sheets Modules { get; }
    bool MultiUserEditing { get; }
    string OnSave { get; set; }
    string OnSheetActivate { get; set; }
    string OnSheetDeactivate { get; set; }
    string Password { get; set; }
    string PasswordEncryptionAlgorithm { get; }
    bool PasswordEncryptionFileProperties { get; }
    int PasswordEncryptionKeyLength { get; }
    string PasswordEncryptionProvider { get; }
    bool PersonalViewListSettings { get; set; }
    bool PersonalViewPrintSettings { get; set; }
    bool PrecisionAsDisplayed { get; set; }
    bool ProtectStructure { get; }
    bool ProtectWindows { get; }
    PublishObjects PublishObjects { get; }
    bool ReadOnlyRecommended { get; set; }
    int RevisionNumber { get; }
    bool Routed { get; }
    RoutingSlip RoutingSlip { get; }
    bool ShowConflictHistory { get; set; }
    bool ShowPivotTableFieldList { get; set; }
    SmartTagOptions SmartTagOptions { get; }
    string Subject { get; set; }
    bool TemplateRemoveExtData { get; set; }
    string Title { get; set; }
    XlUpdateLinks UpdateLinks { get; set; }
    bool UpdateRemoteReferences { get; set; }
    bool UserControl { get; set; }
    object UserStatus { get; }
    bool VBASigned { get; }
    VBProject VBProject { get; }
    WebOptions WebOptions { get; }
    Windows Windows { get; }
    string WritePassword { get; set; }
    bool WriteReserved { get; }
    string WriteReservedBy { get; }
    /// <summary>
    /// Returns a Sheets collection that represents all the sheets in the
    /// specified workbook, for a Workbook object. Read-only Sheets object.
    /// </summary>
    ISheets        Sheets { get; }

    void _PrintOut(object From, object To, object Copies, object Preview, object ActivePrinter, object PrintToFile, object Collate);
    void _Protect(object Password, object Structure, object Windows);
    void _SaveAs(object Filename, object FileFormat, object Password, object WriteResPassword, object ReadOnlyRecommended, object CreateBackup, Excel.XlSaveAsAccessMode AccessMode, object ConflictResolution, object AddToMru, object TextCodepage, object TextVisualLayout);
    void AcceptAllChanges(object When, object Who, object Where);
    void AddToFavorites();
    void BreakLink(string Name, Excel.XlLinkType Type);
    bool CanCheckIn();
    void ChangeFileAccess(Excel.XlFileAccess Mode, object WritePassword, object Notify);
    void ChangeLink(string Name, string NewName, Excel.XlLinkType Type);
    void CheckIn(object SaveChanges, object Comments, object MakePublic);
    void Dummy16();
    void Dummy17(int calcid);
    void EndReview();
    bool ExclusiveAccess();
    void FollowHyperlink(string Address, object SubAddress, object NewWindow, object AddHistory, object ExtraInfo, object Method, object HeaderInfo);
    void ForwardMailer();
    void HighlightChangesOptions(object When, object Who, object Where);
    object LinkInfo(string Name, Excel.XlLinkInfo LinkInfo, object Type, object EditionRef);
    object LinkSources(object Type);
    void MergeWorkbook(object Filename);
    Excel.Window NewWindow();
    void OpenLinks(string Name, object ReadOnly, object Type);
    Excel.PivotCaches PivotCaches();
    void PivotTableWizard(object SourceType, object SourceData, object TableDestination, object TableName, object RowGrand, object ColumnGrand, object SaveData, object HasAutoFormat, object AutoPage, object Reserved, object BackgroundQuery, object OptimizeCache, object PageFieldOrder, object PageFieldWrapCount, object ReadData, object Connection);
    void Post(object DestName);
    void PrintOut(object From, object To, object Copies, object Preview, object ActivePrinter, object PrintToFile, object Collate, object PrToFileName);
    void PrintPreview(object EnableChanges);
    void Protect(object Password, object Structure, object Windows);
    void ProtectSharing(object Filename, object Password, object WriteResPassword, object ReadOnlyRecommended, object CreateBackup, object SharingPassword);
    void PurgeChangeHistoryNow(int Days, object SharingPassword);
    void RecheckSmartTags();
    void RejectAllChanges(object When, object Who, object Where);
    void ReloadAs(Microsoft.Office.Core.MsoEncoding Encoding);
    void RemoveUser(int Index);
    void Reply();
    void ReplyAll();
    void ReplyWithChanges(object ShowMessage);
    void Route();
    void RunAutoMacros(Excel.XlRunAutoMacro Which);
    void sblt(string s);
    void SendForReview(object Recipients, object Subject, object ShowMessage, object IncludeAttachment);
    void SendMail(object Recipients, object Subject, object ReturnReceipt);
    void SendMailer(object FileFormat, Excel.XlPriority Priority);
    void SetLinkOnData(string Name, object Procedure);
    void SetPasswordEncryptionOptions(object PasswordEncryptionProvider, object PasswordEncryptionAlgorithm, object PasswordEncryptionKeyLength, object PasswordEncryptionFileProperties);
    void Unprotect(object Password);
    void UnprotectSharing(object SharingPassword);
    void UpdateLink(object Name, object Type);
    void WebPagePreview();
*/
#endif
    #endregion

    #region Skipped
#if SKIPPED
/*
    /// <summary>
    /// Returns or sets the name of the object. Read-only String.
    /// </summary>
    string        Name { get; }
    /// <summary>
    /// Updates a read-only workbook from the saved disk version of the workbook
    /// if the disk version is more recent than the copy of the workbook that is
    /// loaded in memory.
    /// </summary>
    void UpdateFromFile();
    /// <summary>
    /// Returns a String indicating the name of the object, including its
    /// path on disk, as a string. Read-only.
    /// </summary>
    string        FullNameURLEncoded { get; }
    /// <summary>
    /// Returns or sets colors in the palette for the workbook. The palette has
    /// 56 entries, each represented by an RGB value. Read/write Variant.
    /// </summary>
    object        Colors { get; set; }
    /// <summary>
    /// Returns a Comments collection that represents all the comments for the
    /// specified worksheet. Read-only.
    /// </summary>
    IComments     Comments { get; set; }
    /// <summary>
    /// Returns or sets a DocumentProperties collection that represents
    /// all the custom document properties for the specified workbook.
    /// </summary>
    object        CustomDocumentProperties { get; }
    /// <summary>
    /// Returns the name of the object, including its path on disk, as a
    /// string. Read-only String.
    /// </summary>
    string        FullName { get; }
    /// <summary>
    /// Returns the complete path to the application, excluding the final
    /// separator and name of the application. Read-only String.
    /// </summary>
    string        Path { get; }
    /// <summary>
    /// True if personal information can be removed from the specified
    /// workbook. The default value is False. Read/write Boolean.
    /// </summary>
    bool          RemovePersonalInformation { get; set; }
    /// <summary>
    /// True if Microsoft Excel saves external link values with the
    /// workbook. Read/write Boolean.
    /// </summary>
    bool          SaveLinkValues { get; set; }
    /// <summary>
    /// Deletes a custom number format from the workbook.
    /// </summary>
    /// <param name="NumberFormat"></param>
    void DeleteNumberFormat(string NumberFormat);
    /// <summary>
    /// Refreshes all external data ranges and PivotTable reports in the
    /// specified workbook.
    /// </summary>
    void RefreshAll();
    /// <summary>
    /// Resets the color palette to the default colors.
    /// </summary>
    void ResetColors();
    /// <summary>
    /// Saves a copy of the workbook to a file but doesn't modify the
    /// open workbook in memory.
    /// </summary>
    /// <param name="Filename"></param>
    void SaveCopyAs( string Filename );
    /// <summary>
    /// Saves changes to the workbook in a different file.
    /// </summary>
    /// <param name="Filename"></param>
    /// <param name="FileFormat"></param>
    /// <param name="Password"></param>
    /// <param name="WriteResPassword"></param>
    /// <param name="ReadOnlyRecommended"></param>
    /// <param name="CreateBackup"></param>
    /// <param name="AccessMode"></param>
    /// <param name="ConflictResolution"></param>
    /// <param name="AddToMru"></param>
    /// <param name="TextCodepage"></param>
    /// <param name="TextVisualLayout"></param>
    /// <param name="Local"></param>
    void SaveAs( string Filename, object FileFormat, object Password,
      object WriteResPassword, object ReadOnlyRecommended, object CreateBackup,
      XlSaveAsAccessMode AccessMode, object ConflictResolution,
      object AddToMru, object TextCodepage, object TextVisualLayout, object Local );
*/
#endif
    #endregion

    #region Interface Properties
    /// <summary>
    /// Returns an object that represents the active sheet (the sheet on top)
    /// in the active workbook or in the specified window or workbook. Returns
    /// Nothing if no sheet is active. Read-only.
    /// </summary>
    IWorksheet    ActiveSheet { get; }
    /// <summary>
    /// Gets / sets index of the active sheet.
    /// </summary>
    int           ActiveSheetIndex { get; set; }
    /// <summary>
    /// Returns collection of all workbook's add-in functions. Read-only.
    /// </summary>
    IAddInFunctions AddInFunctions { get; }
    /// <summary>
    /// Returns or sets the author of the comment. Read-only String.
    /// </summary>
    string        Author { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether to display horizontal scroll bar. 
    /// </summary>
    bool IsHScrollBarVisible { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether to display vertical scroll bar. 
    /// </summary>
    bool IsVScrollBarVisible { get; set; }
    /// <summary>
    /// Returns collection that represents all the built-in document properties
    /// for the specified workbook. Read-only.
    /// </summary>
    IBuiltInDocumentProperties BuiltInDocumentProperties { get; }
    /// <summary>
    /// Name which is used by macros to access the workbook items.
    /// </summary>
    string        CodeName{ get; set; }
    /// <summary>
    /// Returns collection that represents all the custom document properties
    /// for the specified workbook. Read-only.
    /// </summary>
    ICustomDocumentProperties CustomDocumentProperties { get; }
    /// <summary>
    /// Returns collection that represents all the Content Type Properties
    /// for the specified workbook. Read-only.
    /// </summary>
    IMetaProperties ContentTypeProperties { get; }
    /// <summary>
    /// Returns collection that represents all the Custom Xml parts 
    /// for the specified workbook. Read-only.
    /// </summary>
    ICustomXmlPartCollection CustomXmlparts { get; }
    ///<summary>
    /// True if the workbook uses the 1904 date system. Read / write Boolean.
    /// </summary>
    bool          Date1904 { get; set; }
    /// <summary>
    /// True if the precision to be used as displayed.
    /// </summary>
    bool PrecisionAsDisplayed { get; set; }
    /// <summary>
    /// True if cell is protected.
    /// </summary>
    bool          IsCellProtection { get;  }
    /// <summary>
    /// True if window is protected.
    /// </summary>
    bool          IsWindowProtection { get; }
    /// <summary>
    /// For an Application object, returns a Names collection that represents
    /// all the names in the active workbook. For a Workbook object, returns
    /// a Names collection that represents all the names in the specified
    /// workbook (including all worksheet-specific names).
    /// </summary>
    INames        Names { get; }
    /// <summary>
    /// True if the workbook has been opened as Read-only. Read-only Boolean.
    /// </summary>
    bool          ReadOnly { get; }
    /// <summary>
    /// True if no changes have been made to the specified workbook since 
    /// it was last saved. If current value is false then setting it to true cause Save() method call.
    /// Read/write Boolean.
    /// </summary>
    bool          Saved { get; set; }
    /// <summary>
    /// Returns a Styles collection that represents all the styles
    /// in the specified workbook. Read-only.
    /// </summary>
    IStyles       Styles { get; }
    /// <summary>
    /// Returns a Sheets collection that represents all the worksheets
    /// in the specified workbook. Read-only Sheets object.
    /// </summary>
    IWorksheets   Worksheets { get; }
    /// <summary>
    /// True indicate that opened workbook contains VBA macros.
    /// </summary>
    bool          HasMacros{ get; }
    /// <summary>
    /// Get Palette of colors which an Excel document can have. 
    /// Here is a table of color indexes to places in the color tool box 
    /// provided by Excel application:
    /// --------------------------------------------
    /// |  | 1  | 2  | 3  | 4  | 5  | 6  | 7  | 8  |
    /// ---+----------------------------------------
    /// |1 | 00 | 51 | 50 | 49 | 47 | 10 | 53 | 54 |
    /// |2 | 08 | 45 | 11 | 09 | 13 | 04 | 46 | 15 |
    /// |3 | 02 | 44 | 42 | 48 | 41 | 40 | 12 | 55 |
    /// |4 | 06 | 43 | 05 | 03 | 07 | 32 | 52 | 14 |
    /// |5 | 37 | 39 | 35 | 34 | 33 | 36 | 38 | 01 |
    /// ---+----------------------------------------
    /// |6 | 16 | 17 | 18 | 19 | 20 | 21 | 22 | 23 |
    /// |7 | 24 | 25 | 26 | 27 | 28 | 29 | 30 | 31 |
    /// --------------------------------------------
    /// </summary>
    [ Obsolete( "IWorkbook.Palettte property is obsolete so please use the"
        + " IWorkbook.Palette property instead. IWorkbook.Palettte will be removed"
        + " in July 2006. Sorry for the inconvenience" ) ]
    Color[]       Palettte{ get; }
    /// <summary>
    /// Get Palette of colors which an Excel document can have. 
    /// Here is a table of color indexes to places in the color tool box 
    /// provided by Excel application:
    /// --------------------------------------------
    /// |  | 1  | 2  | 3  | 4  | 5  | 6  | 7  | 8  |
    /// ---+----------------------------------------
    /// |1 | 00 | 51 | 50 | 49 | 47 | 10 | 53 | 54 |
    /// |2 | 08 | 45 | 11 | 09 | 13 | 04 | 46 | 15 |
    /// |3 | 02 | 44 | 42 | 48 | 41 | 40 | 12 | 55 |
    /// |4 | 06 | 43 | 05 | 03 | 07 | 32 | 52 | 14 |
    /// |5 | 37 | 39 | 35 | 34 | 33 | 36 | 38 | 01 |
    /// ---+----------------------------------------
    /// |6 | 16 | 17 | 18 | 19 | 20 | 21 | 22 | 23 |
    /// |7 | 24 | 25 | 26 | 27 | 28 | 29 | 30 | 31 |
    /// --------------------------------------------
    /// </summary>
    Color[]       Palette{ get; }
    /// <summary>
    /// Index of tab which will be displayed on document open.
    /// </summary>
    int           DisplayedTab{ get; set; }
    /// <summary>
    /// Collection of the chart objects.
    /// </summary>
    ICharts       Charts { get; }
    /// <summary>
    /// Indicates whether exception should be thrown when unknown
    /// name was found in a formula.
    /// </summary>
    bool          ThrowOnUnknownNames { get; set; }
    /// <summary>
    /// This Property allows users to disable load of macros from 
    /// document. Excel on file open will simply skip macros and will 
    /// work as if document does not contain them. This options works
    /// only when file contains macros (HasMacros property is True).
    /// </summary>
    bool          DisableMacrosStart{ get; set; }
    /// <summary>
    /// Returns or sets the standard font size, in points. Read/write.
    /// </summary>
    double        StandardFontSize { get; set; }
    /// <summary>
    /// Returns or sets the name of the standard font. Read/write String.
    /// </summary>
    string        StandardFont { get; set; }
    /// <summary>
    /// Indicates whether to allow usage of 3D ranges in DataValidation
    /// list property (MS Excel doesn't allow).
    /// </summary>
    bool          Allow3DRangesInDataValidation { get; set; }
    /// <summary>
    /// Returns calculation options. Read-only.
    /// </summary>
    ICalculationOptions CalculationOptions { get; }
    /// <summary>
    /// Gets / sets row separator for array parsing.
    /// </summary>
    string            RowSeparator { get; }
    /// <summary>
    /// Formula arguments separator.
    /// </summary>
    string            ArgumentsSeparator { get; }
    /// <summary>
    /// Returns grouped worksheets. Read-only.
    /// </summary>
    IWorksheetGroup WorksheetGroup { get; }
    /// <summary>
    /// Indicates whether worksheet is displayed right to left.
    /// </summary>
    bool IsRightToLeft { get; set; }
    /// <summary>
    /// Indicates whether tabs are visible.
    /// </summary>
    bool DisplayWorkbookTabs { get; set; }
    /// <summary>
    /// Returns collection of tab sheets. Read-only.
    /// </summary>
    ITabSheets TabSheets { get; }
    /// <summary>
    /// Indicates whether library should try to detect string value passed to Value (and Value2)
    /// property as DateTime. Setting this property to false can increase performance greatly for
    /// such operations especially on Framework 1.0 and 1.1. Default value is true.
    /// </summary>
    bool DetectDateTimeInValue { get; set; }
    /// <summary>
    /// Toggles string searching algorithm. If true then Dictionary will be used
    /// to locate string inside strings dictionary. This mode is faster but uses
    /// more memory. If false then each time string is added to strings dictionary
    /// we will have to iterate through it and compare new strings with existing ones.
    /// Default value is TRUE.
    /// </summary>
    bool UseFastStringSearching { get; set; }
    /// <summary>
    /// True to display a message when the file is opened, recommending that the file be opened as read-only.
    /// </summary>
    bool ReadOnlyRecommended { get; set; }
    /// <summary>
    /// Gets / sets password to encrypt document.
    /// </summary>
    string PasswordToOpen { get; set; }
    /// <summary>
    /// Returns maximum row count for each worksheet in this workbook. Read-only.
    /// </summary>
    int MaxRowCount { get; }
    /// <summary>
    /// Returns maximum column count for each worksheet in this workbook. Read-only.
    /// </summary>
    int MaxColumnCount { get; }
    /// <summary>
    /// Gets / sets excel version.
    /// </summary>
    ExcelVersion Version { get; set; }
    /// <summary>
    /// Returns pivot caches collection. Read-only.
    /// </summary>
    IPivotCaches PivotCaches  { get; }
    /// <summary>
    /// Returns the connection.Read Only.
    /// </summary>
    IConnections Connections { get; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Creates the Data sorter to sort the data..
    /// </summary>
    /// <returns>Data Sorter.</returns>
    IDataSort CreateDataSorter();
    /// <summary>
    /// Activates the first window associated with the workbook.
    /// </summary>
    void Activate();
    /// <summary>
    /// Adds font to the inner fonts collection and makes this font read-only.
    /// </summary>
    /// <param name="fontToAdd">Font to add.</param>
    /// <returns>Added font.</returns>
    IFont AddFont( IFont fontToAdd );
    /// <summary>
    /// Closes the object.
    /// </summary>
    /// <param name="SaveChanges"></param>
    /// <param name="Filename"></param>
    void Close( bool SaveChanges, string Filename );
    /// <summary>
    /// Closes the object.
    /// </summary>
    /// <param name="saveChanges">If True, all changes will be saved.</param>
    void Close( bool saveChanges );
    /// <summary>
    /// Closes the object without saving.
    /// </summary>
    void Close();
#if !(WINRT || WP)
    /// <summary>
    /// Closes the object and saves changes into specified file.
    /// </summary>
    /// <param name="Filename">
    /// File name in which workbook will be saved if SaveChanges is true.
    /// </param>
    void Close( string Filename );
    /// <summary>
    /// Saves changes to the specified workbook.
    /// </summary>
    void Save();
    /// <summary>
    /// Short variant of SaveAs method.
    /// </summary>
    /// <param name="Filename"></param>
    void SaveAs( string Filename );
    /// <summary>
    /// Short variant of SaveAs method.
    /// </summary>
    /// <param name="Filename"></param>
    /// <param name="saveType"></param>
    void SaveAs( string Filename, ExcelSaveType saveType );
    /// <summary>
    /// Saves workbook in xml format.
    /// </summary>
    /// <param name="strFileName">File name to save into.</param>
    /// <param name="saveType">Xml save type.</param>
    void SaveAsXml( string strFileName, ExcelXmlSaveType saveType );
    /// <summary>
    /// Save active WorkSheet using separator.
    /// </summary>
    /// <param name="fileName">Path to save.</param>
    /// <param name="separator">Current separator.</param>
    void SaveAs( string fileName, string separator );
#if !(SILVERLIGHT || WP)
    /// <summary>
    /// Saves as Html.
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="saveOptions"></param>
    void SaveAsHtml(string filename,HtmlSaveOptions saveOptions);    
    void SaveAsHtml(Stream stream);
    void SaveAsHtml(Stream stream, HtmlSaveOptions saveOptions);
#endif
#endif
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Copies workbook to the clipboard.
    /// </summary>
    void CopyToClipboard();
    /// <summary>
    /// Method creates a font object based on native font and register it in the workbook.
    /// </summary>]
    /// <param name="nativeFont">Native font to get settings from.</param>
    /// <returns>Newly created font.</returns>
    IFont CreateFont( Font nativeFont );
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
    /// <summary>
    /// Creates header/footer engine.
    /// </summary>
    /// <returns>New instance of header/footer engine.</returns>
    IHFEngine CreateHFEngine();
# if !ClientProfile
    /// <summary>
    /// Saves changes to the specified HttpResponse.
    /// </summary>
    /// <param name="fileName">Name of the file in HttpResponse.</param>
    /// <param name="separator">string separator.</param>
    /// <param name="response">HttpResponse that will receive workbook's data.</param>
    /// <param name="downloadType">Download type.</param>
    /// <param name="contentType">Http content type.</param>
    void SaveAs(string fileName, string separator, HttpResponse response, ExcelDownloadType downloadType, ExcelHttpContentType contentType);
    /// <summary>
    /// Saves changes to the specified HttpResponse.
    /// </summary>
    /// <param name="fileName">Name of the file in HttpResponse.</param>
    /// <param name="saveType">Type of the Excel file.</param>
    /// <param name="response">HttpResponse that will receive workbook's data.</param>
    void SaveAs( string fileName, ExcelSaveType saveType, HttpResponse response );
    /// <summary>
    /// Saves changes to the specified HttpResponse.
    /// </summary>
    /// <param name="fileName">Name of the file in HttpResponse.</param>
    /// <param name="saveType">Type of the Excel file.</param>
    /// <param name="response">HttpResponse that will receive workbook's data.</param>
    /// <param name="contentType">Content type to use.</param>
    void SaveAs( string fileName, ExcelSaveType saveType, HttpResponse response,
      ExcelHttpContentType contentType );
    ///// <summary>
    ///// Saves changes to the specified HttpResponse.
    ///// </summary>
    ///// <param name="fileName">Name of the file in HttpResponse.</param>
    ///// <param name="response">HttpResponse to save in.</param>
    //void SaveAs( string fileName, HttpResponse response );
    /// <summary>
    /// Saves changes to the specified HttpResponse.
    /// </summary>
    /// <param name="fileName">Name of the file in HttpResponse.</param>
    /// <param name="response">HttpResponse to save in.</param>
    /// <param name="contentType">Content type to use.</param>
    void SaveAs( string fileName, HttpResponse response, ExcelHttpContentType contentType );
    /// <summary>
    /// Saves changes to the specified HttpResponse.
    /// </summary>
    /// <param name="fileName">Name of the file in HttpResponse.</param>
    /// <param name="saveType">Type of the Excel file.</param>
    /// <param name="response">HttpResponse that will receive workbook's data.</param>
    /// <param name="downloadType">Download type.</param>
    void SaveAs( string fileName, ExcelSaveType saveType, HttpResponse response
      , ExcelDownloadType downloadType );
    /// <summary>
    /// Saves changes to the specified HttpResponse.
    /// </summary>
    /// <param name="fileName">Name of the file in HttpResponse.</param>
    /// <param name="saveType">Type of the Excel file.</param>
    /// <param name="response">HttpResponse that will receive workbook's data.</param>
    /// <param name="downloadType">Download type.</param>
    /// <param name="contentType">Content type to use.</param>
    void SaveAs( string fileName, ExcelSaveType saveType, HttpResponse response
      , ExcelDownloadType downloadType, ExcelHttpContentType contentType );
    /// <summary>
    /// Saves changes to the specified HttpResponse.
    /// </summary>
    /// <param name="fileName">Name of the file in HttpResponse.</param>
    /// <param name="response">HttpResponse that will receive workbook's data.</param>
    /// <param name="downloadType">Download type.</param>
    void SaveAs( string fileName, HttpResponse response
      , ExcelDownloadType downloadType );
    /// <summary>
    /// Saves changes to the specified HttpResponse.
    /// </summary>
    /// <param name="fileName">Name of the file in HttpResponse.</param>
    /// <param name="response">HttpResponse that will receive workbook's data.</param>
    /// <param name="downloadType">Download type.</param>
    /// <param name="contentType">Content type to use.</param>
    void SaveAs( string fileName, HttpResponse response
      , ExcelDownloadType downloadType, ExcelHttpContentType contentType );
#endif
#endif
    /// <summary>
    /// Creates object that can be used for template markers processing.
    /// </summary>
    /// <returns>Object that can be used for template markers processing.</returns>
    ITemplateMarkersProcessor CreateTemplateMarkersProcessor();
    /// <summary>
    /// Marks workbook as final and Read-Only.
    /// </summary>
    void MarkAsFinal();
#if ( WINRT || WP)
    /// <summary>
    /// Saves changes to the specified stream.
    /// </summary>
    /// <param name="stream">Stream that will receive workbook data.</param>
    Task<bool> SaveAsAsync(Stream stream);
    /// <summary>
    /// Saves changes to the specified stream.
    /// </summary>
    /// <param name="stream">Stream that will receive workbook data.</param>
    /// <param name="saveType">Type of the Excel file.</param>
    Task<bool> SaveAsAsync(Stream stream, ExcelSaveType saveType);
    /// <summary>
    /// Saves workbook in xml format.
    /// </summary>
    /// <param name="writer">XmlWriter to save into.</param>
    /// <param name="saveType">Xml save type.</param>
    Task<bool> SaveAsXmlAsync(XmlWriter writer, ExcelXmlSaveType saveType);
    /// <summary>
    /// Saves workbook in xml format.
    /// </summary>
    /// <param name="stream">Stream to save into.</param>
    /// <param name="saveType">Xml save type.</param>
    Task<bool> SaveAsXmlAsync(Stream stream, ExcelXmlSaveType saveType);

    /// <summary>
    /// Saves changes to the specified storage file.
    /// </summary>
    /// <param name="storageFile">Storage that will receive workbook data.</param>
    Task<bool> SaveAsAsync(StorageFile storageFile);
    /// <summary>
    /// Saves changes to the specified storage file.
    /// </summary>
    /// <param name="storageFile">Storage File that will receive workbook data.</param>
    /// <param name="saveType">Type of the Excel file.</param>
    Task<bool> SaveAsAsync(StorageFile storageFile, ExcelSaveType saveType);
    /// <summary>
    /// Saves changes to the specified stream.
    /// </summary>
    /// <param name="stream">Stream that will receive workbook data.</param>
    /// <param name="separator">Separator to use.</param>
    Task<bool> SaveAsAsync(Stream stream, string separator);
    /// <summary>
    /// Saves changes to the specified storage file.
    /// </summary>
    /// <param name="storageFile">Storage File that will receive workbook data.</param>
    /// <param name="separator">Separator to use.</param>
    Task<bool> SaveAsAsync(StorageFile storageFile, string separator);
    /// <summary>
    /// Saves workbook in xml format.
    /// </summary>
    /// <param name="storageFile">Storage to save into.</param>
    /// <param name="saveType">Xml save type.</param>
    Task<bool> SaveAsXmlAsync(StorageFile storageFile, ExcelXmlSaveType saveType);
#else
    /// <summary>
    /// Saves changes to the specified stream.
    /// </summary>
    /// <param name="stream">Stream that will receive workbook data.</param>
    void SaveAs( Stream stream );
    /// <summary>
    /// Saves changes to the specified stream.
    /// </summary>
    /// <param name="stream">Stream that will receive workbook data.</param>
    /// <param name="saveType">Type of the Excel file.</param>
    void SaveAs( Stream stream, ExcelSaveType saveType );
    /// <summary>
    /// Saves workbook in xml format.
    /// </summary>
    /// <param name="writer">XmlWriter to save into.</param>
    /// <param name="saveType">Xml save type.</param>
    void SaveAsXml( XmlWriter writer, ExcelXmlSaveType saveType );
    /// <summary>
    /// Saves workbook in xml format.
    /// </summary>
    /// <param name="stream">Stream to save into.</param>
    /// <param name="saveType">Xml save type.</param>
    void SaveAsXml( Stream stream, ExcelXmlSaveType saveType );
    /// <summary>
    /// Save active WorkSheet using separator.
    /// </summary>
    /// <param name="stream">Stream to save.</param>
    /// <param name="separator">Current separator.</param>
    void SaveAs( Stream stream, string separator );
#endif
#if MVC
    /// <summary>
    /// Save as ActionResult
    /// </summary>
    /// <param name="filename">Name of the file.</param>
    /// <param name="response">The Response.</param>
    /// <param name="contentType">Http content type.</param>
    /// <returns></returns>
    ExcelResult SaveAsActionResult(string filename, HttpResponse response, ExcelHttpContentType contentType);
    /// <summary>
    /// Save as ActionResult
    /// </summary>
    /// <param name="filename">Name of the file.</param>
    /// <param name="response">The Response.</param>
    /// <param name="DownloadType">Download type.</param>
    /// <param name="contentType">Http content type.</param>
    /// <returns></returns>
    ExcelResult SaveAsActionResult(string filename, HttpResponse response, ExcelDownloadType DownloadType, ExcelHttpContentType contentType);
    /// <summary>
    /// Save as ActionResult
    /// </summary>
    /// <param name="filename">Name of the file.</param>
    /// <param name="saveType">Type of the Excel file.</param>
    /// <param name="response">The Response.</param>
    /// <param name="DownloadType">Download type.</param>
    /// <param name="contentType">Http content type.</param>
    /// <returns></returns>
    ExcelResult SaveAsActionResult(string filename, ExcelSaveType saveType, HttpResponse response, ExcelDownloadType DownloadType, ExcelHttpContentType contentType);
    /// <summary>
    /// Save as ActionResult
    /// </summary>
    /// <param name="filename">Name of the file.</param>
    /// <param name="response">The Response.</param>
    /// <param name="DownloadType">Download type.</param>
    /// <param name="contentType">Http content type.</param>
    /// <returns></returns>
    ExcelResult SaveAsActionResult(string filename, string separator, HttpResponse response, ExcelDownloadType DownloadType, ExcelHttpContentType contentType);
      /// <summary>
      /// Save as ActionResult
      /// </summary>
      /// <param name="filename">Name of the file.</param>
      /// <param name="response">The Response.</param>
      /// <param name="DownloadType">Download type.</param>
      /// <returns></returns>
    ExcelResult SaveAsActionResult(string filename, HttpResponse response, ExcelDownloadType DownloadType);
#endif
    /// <summary>
    /// Set user color for specified element in Color table.
    /// </summary>
    /// <param name="index">Index of Color in array.</param>
    /// <param name="color">New color which must be set.</param>
    void SetPaletteColor( int index, Color color );
    /// <summary>
    /// Recover palette to default values.
    /// </summary>
    void ResetPalette();
    /// <summary>
    /// Method return Color object from workbook palette by its index.
    /// </summary>
    /// <param name="color">Index from palette array.</param>
    /// <returns>RGB Color.</returns>
    Color GetPaletteColor( ExcelKnownColors color );
    /// <summary>
    /// Gets the nearest color to the specified Color structure
    /// from Workbook palette.
    /// </summary>
    /// <param name="color"></param>
    /// <returns>Color index from workbook palette.</returns>
    ExcelKnownColors GetNearestColor( Color color );
    /// <summary>
    /// Gets the nearest color to the specified by red, green, and blue 
    /// values color from Workbook palette.
    /// </summary>
    /// <param name="r">Red component of the color.</param>
    /// <param name="g">Green component of the color.</param>
    /// <param name="b">Blue component of the color.</param>
    /// <returns>Color index from workbook palette.</returns>
    ExcelKnownColors GetNearestColor( int r, int g, int b );
    /// <summary>
    /// If there is at least one free color, define a new color;
    /// if not, search for the closest one.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    ExcelKnownColors SetColorOrGetNearest( Color color );
    /// <summary>
    /// If there is at least one free color, define a new color;
    /// if not, search for the closest one.
    /// </summary>
    /// <param name="r">Red component of the color.</param>
    /// <param name="g">Green component of the color.</param>
    /// <param name="b">Blue component of the color.</param>
    /// <returns></returns>
    ExcelKnownColors SetColorOrGetNearest( int r, int g, int b );
    /// <summary>
    /// Method to create a font object and register it in the workbook.
    /// </summary>]
    /// <returns>Newly created font.</returns>
    IFont CreateFont();
    /// <summary>
    /// Method that creates font object based on another font object
    /// and registers it in the workbook.
    /// </summary>
    /// <param name="baseFont">Base font for the new one.</param>
    /// <returns>Newly created font.</returns>
    IFont CreateFont( IFont baseFont );
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
    IRange FindFirst(string findValue, ExcelFindType flags, ExcelFindOptions findOptions);
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
    IRange FindStringStartsWith(string findValue, ExcelFindType flags,bool ignoreCase);
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
    IRange FindStringEndsWith(string findValue, ExcelFindType flags,bool ignoreCase);
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

    /// <summary>
    /// Sets separators for formula parsing.
    /// </summary>
    /// <param name="argumentsSeparator">Arguments separator to set.</param>
    /// <param name="arrayRowsSeparator">Array rows separator to set.</param>
    void SetSeparators( char argumentsSeparator, char arrayRowsSeparator );
    /// <summary>
    /// Sets protection for workbook.
    /// </summary>
    /// <param name="bIsProtectWindow">Indicates if protect workbook window.</param>
    /// <param name="bIsProtectContent">Indicates if protect workbook content.</param>
    void Protect( bool bIsProtectWindow, bool bIsProtectContent );
    /// <summary>
    /// Sets protection for workbook.
    /// </summary>
    /// <param name="bIsProtectWindow">Indicates if protect workbook window.</param>
    /// <param name="bIsProtectContent">Indicates if protect workbook content.</param>
    /// <param name="password">Password to protect with.</param>
    void Protect( bool bIsProtectWindow, bool bIsProtectContent, string password );
    /// <summary>
    /// Unprotects workbook.
    /// </summary>
    void Unprotect();
    /// <summary>
    /// Unprotects workbook. Throws ArgumentOutOfRangeException when password is wrong.
    /// </summary>
    /// <param name="password">Password to unprotect workbook.</param>
    void Unprotect( string password );
    /// <summary>
    /// Creates copy of the current instance.
    /// </summary>
    /// <returns>Copy of the current instance.</returns>
    IWorkbook Clone();
    /// <summary>
    /// This method sets write protection password.
    /// </summary>
    /// <param name="password">Password to set.</param>
    void SetWriteProtectionPassword( string password );
    #endregion

    #region Interface events
    /// <summary>
    /// This event is fired after workbook is successfully saved.
    /// </summary>
    event EventHandler OnFileSaved;
    /// <summary>
    /// This event is fired when trying to save to a Read-only file.
    /// </summary>
    event ReadOnlyFileEventHandler OnReadOnlyFile;
    #endregion
  }
}

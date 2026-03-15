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
using System.IO;
using System.Xml;
using System.Text;
#if ( WINRT || WP)
using System.Threading.Tasks;
using Windows.Storage;
#endif
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// A collection of all the Workbook objects that are currently open in
  /// the Excel application.
  /// </summary>
  public interface IWorkbooks : IEnumerable
  {
    #region Not supported methods/properties
#if NOT_SUPPORTED
/*
    Workbook _Default { get; }
    XlCreator Creator { get; }

    void __OpenText(string Filename, object Origin, object StartRow, object DataType, Excel.XlTextQualifier TextQualifier, object ConsecutiveDelimiter, object Tab, object Semicolon, object Comma, object Space, object Other, object OtherChar, object FieldInfo, object TextVisualLayout);
    void _OpenText(string Filename, object Origin, object StartRow, object DataType, Excel.XlTextQualifier TextQualifier, object ConsecutiveDelimiter, object Tab, object Semicolon, object Comma, object Space, object Other, object OtherChar, object FieldInfo, object TextVisualLayout, object DecimalSeparator, object ThousandsSeparator);
    bool CanCheckOut(string Filename);
    void CheckOut(string Filename);
    Excel.Workbook Open(string Filename, object UpdateLinks, object ReadOnly, object Format, object Password, object WriteResPassword, object IgnoreReadOnlyRecommended, object Origin, object Delimiter, object Editable, object Notify, object Converter, object AddToMru, object Local, object CorruptLoad);
    Excel.Workbook OpenDatabase(string Filename, object CommandText, object CommandType, object BackgroundQuery, object ImportDataAs);
    void OpenText(string Filename, object Origin, object StartRow, object DataType, Excel.XlTextQualifier TextQualifier, object ConsecutiveDelimiter, object Tab, object Semicolon, object Comma, object Space, object Other, object OtherChar, object FieldInfo, object TextVisualLayout, object DecimalSeparator, object ThousandsSeparator, object TrailingMinusNumbers, object Local);
    Excel.Workbook OpenXML(string Filename, object Stylesheets);
*/
#endif
    #endregion

    #region Skipped
#if SKIPPED
/*
    /// <summary>
    /// Creates a new workbook. The new workbook becomes the active workbook.
    /// Returns a Workbook object.
    /// </summary>
    /// <param name="Template"></param>
    /// <returns></returns>
    IWorkbook Add( object Template );
    /// <summary>
    /// Opens a workbook.
    /// </summary>
    /// <param name="Filename"></param>
    /// <param name="UpdateLinks"></param>
    /// <param name="ReadOnly"></param>
    /// <param name="Format"></param>
    /// <param name="Password"></param>
    /// <param name="WriteResPassword"></param>
    /// <param name="IgnoreReadOnlyRecommended"></param>
    /// <param name="Origin"></param>
    /// <param name="Delimiter"></param>
    /// <param name="Editable"></param>
    /// <param name="Notify"></param>
    /// <param name="Converter"></param>
    /// <param name="AddToMru"></param>
    /// <returns></returns>
    IWorkbook Open( string Filename, object UpdateLinks, object ReadOnly,
      object Format, object Password, object WriteResPassword,
      object IgnoreReadOnlyRecommended, object Origin, object Delimiter,
      object Editable, object Notify, object Converter, object AddToMru );
*/
#endif
    #endregion

    #region Interface properties
    /// <summary>
    /// Used without an object qualifier, this property returns an
    /// Application object that represents the Excel application.
    /// </summary>
    IApplication Application { get; }
    /// <summary>
    /// Returns the number of objects in the collection. Read-only Long.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Returns a single object from a collection.
    /// </summary>
    IWorkbook this[ int Index ] { get; }
    /// <summary>
    /// Returns the parent object for the specified object.
    /// </summary>
    object Parent { get; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Create empty workbook. The new workbook becomes the active workbook.
    /// </summary>
    /// <returns>Interface on instance of created workbook.</returns>
    IWorkbook Create();
    /// <summary>
    /// Create workbook with specified quantity of empty worksheets.
    /// </summary>
    /// <param name="worksheetsQuantity">Quantity of worksheets to create.</param>
    /// <returns>Interface on instance of created workbook.</returns>
    IWorkbook Create( int worksheetsQuantity );
    /// <summary>
    /// Create workbook with specified names.
    /// Each worksheet name will be set to corresponding names array element.
    /// </summary>
    /// <param name="names">Array of names for each worksheet.</param>
    /// <returns>Interface on instance of created workbook.</returns>
    IWorkbook Create( string[] names );   
#if !(WINRT || WP)
    /// <summary>
    /// Creates a new workbook. The new workbook becomes the active workbook.
    /// Returns a Workbook object.
    /// </summary>
    /// <returns>Added Workbook object.</returns>
    IWorkbook Add();
    /// <summary>
    /// Creates a new workbook. The new workbook becomes the active workbook.
    /// </summary>
    /// <param name="version">Excel version - defines file format (excel 97-2003 or excel 2007).</param>
    /// <returns>Added Workbook object.</returns>
    IWorkbook Add( ExcelVersion version );
    /// <summary>
    /// Creates a new workbook. The new workbook becomes the active workbook.
    /// Returns a Workbook object.
    /// </summary>
    /// <param name="strTemplateFile">File that contains required workbook.</param>
    /// <returns>Added Workbook object.</returns>
    IWorkbook Add( string strTemplateFile );
    /// <summary>
    /// Creates a new workbook. The new workbook becomes the active workbook.
    /// Returns a Workbook object.
    /// </summary>
    /// <param name="strTemplateFile">File that contains required workbook.</param>
    /// <param name="version">Excel version - defines file format (excel 97-2003 or excel 2007).</param>
    /// <returns>Added Workbook object.</returns>
    IWorkbook Add( string strTemplateFile, ExcelVersion version );
    /// <summary>
    /// Creates a new workbook. The new workbook becomes the active workbook.
    /// Returns a Workbook object.
    /// </summary>
    /// <param name="strTemplateFile">File that contains required workbook.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Added Workbook object.</returns>
    IWorkbook Add( string strTemplateFile, ExcelParseOptions options );
    /// <summary>
    /// Creates a new workbook. The new workbook becomes the active workbook.
    /// Returns a Workbook object.
    /// </summary>
    /// <param name="strTemplateFile">File that contains required workbook.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="version">Excel version - defines file format (excel 97-2003 or excel 2007).</param>
    /// <returns>Added Workbook object.</returns>
    IWorkbook Add( string strTemplateFile, ExcelParseOptions options, ExcelVersion version );
    /// <summary>
    /// Opens a workbook.
    /// </summary>
    /// <param name="filename">File name that contains workbook.</param>
    /// <returns>Opened workbook.</returns>
    IWorkbook Open( string filename );
    /// <summary>
    /// Opens a workbook.
    /// </summary>
    /// <param name="filename">File name that contains workbook.</param>
    /// <param name="version">Excel version - defines file format (excel 97-2003 or excel 2007).</param>
    /// <returns>Opened workbook.</returns>
    IWorkbook Open( string filename, ExcelVersion version );
    /// <summary>
    /// Opens a workbook.
    /// </summary>
    /// <param name="Filename">File name that contains workbook.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Opened workbook.</returns>
    IWorkbook Open( string Filename, ExcelParseOptions options );
    /// <summary>
    /// Opens a Workbook using separator.
    /// </summary>
    /// <param name="fileName">File name to open.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <returns>Opened WorkBook.</returns>
    IWorkbook Open( string fileName, string separator, int row, int column );
    /// <summary>
    /// Opens a Workbook using separator.
    /// </summary>
    /// <param name="fileName">File name to open.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <param name="encoding">Encoding to use to parse text data.</param>
    /// <returns>Opened WorkBook.</returns>
    IWorkbook Open( string fileName, string separator, int row, int column, Encoding encoding );
    /// <summary>
    /// Opens a Workbook using separator.
    /// </summary>
    /// <param name="fileName">Stream to open.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use to parse text data.</param>
    /// <returns>Opened WorkBook.</returns>
    IWorkbook Open( string fileName, string separator );
    /// <summary>
    /// Opens a Workbook using separator.
    /// </summary>
    /// <param name="fileName">Stream to open.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use to parse text data.</param>
    /// <returns>Opened WorkBook.</returns>
    IWorkbook Open( string fileName, string separator, Encoding encoding );
    /// <summary>
    /// Open new workbook in read-only mode.
    /// </summary>
    /// <param name="strFileName">File to open.</param>
    /// <returns>Newly created workbook.</returns>
    IWorkbook OpenReadOnly( string strFileName );
    /// <summary>
    /// Open new CSV file in read-only mode.
    /// </summary>
    /// <param name="strFileName">File to open.</param>
    /// <param name="seperator">String separator.</param>
    /// <returns>Newly created workbook.</returns>
    IWorkbook OpenReadOnly(string strFileName, string seperator);
    /// <summary>
    /// Open new workbook in read-only mode.
    /// </summary>
    /// <param name="strFileName">File to open.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Newly created workbook.</returns>
    IWorkbook OpenReadOnly( string strFileName, ExcelParseOptions options );
    /// <summary>
    /// Open new workbook in read-only mode.
    /// </summary>
    /// <param name="strFileName">File to open.</param>
    /// <param name="openType">Represents open type for open.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Newly created workbook.</returns>
    IWorkbook OpenReadOnly( string strFileName, ExcelOpenType openType, ExcelParseOptions options );
    /// <summary>
    /// Open new workbook in read-only mode.
    /// </summary>
    /// <param name="strFileName">File to open.</param>
    /// <param name="options"></param>
    /// <param name="bReadOnly"></param>
    /// <param name="password">Password that should be used for decryption.</param>
    /// <returns>Newly created workbook.</returns>
    IWorkbook Open( string strFileName, ExcelParseOptions options, bool bReadOnly, string password );
    /// <summary>
    /// Opens a workbook.
    /// </summary>
    /// <param name="fileName">File name that contains workbook.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="isReadOnly">Indicates is open book in read - only mode.</param>
    /// <param name="password">Represents valid password for opening workbook.</param>
    /// <param name="version">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    IWorkbook Open( string fileName, ExcelParseOptions options, bool isReadOnly,string password, ExcelVersion version );
    /// <summary>
    /// Opens a workbook.
    /// </summary>
    /// <param name="fileName">File name that contains workbook.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="isReadOnly">Indicates is open book in read - only mode.</param>
    /// <param name="password">Represents valid password for opening workbook.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <returns>Opened workbook.</returns>
    IWorkbook Open( string fileName, ExcelParseOptions options, bool isReadOnly,string password, ExcelOpenType openType );
    /// <summary>
    /// Opens  a workbook.
    /// </summary>
    /// <param name="fileName">File name that contains workbook.</param>
    /// <param name="openType">Represents open type for open.</param>
    /// <returns>Opened workbook.</returns>
    IWorkbook Open( string fileName, ExcelOpenType openType );
    /// <summary>
    /// Opens  a workbook.
    /// </summary>
    /// <param name="fileName">File name that contains workbook.</param>
    /// <param name="openType">Represents open type for open.</param>
    /// <returns>Opened workbook.</returns>
    IWorkbook Open( string fileName, ExcelOpenType openType, ExcelParseOptions options );
    /// <summary>
    /// Opens  a workbook.
    /// </summary>
    /// <param name="fileName">File name that contains workbook.</param>
    /// <param name="openType">Represents open type for open.</param>
    /// <param name="version">Desired version of the workbook.</param>
    /// <returns>Opened workbook.</returns>
    IWorkbook Open( string fileName, ExcelOpenType openType, ExcelVersion version );
    /// <summary>
    /// Read workbook from xml file.
    /// </summary>
    /// <param name="strPath">Full path to xml file.</param>
    /// <param name="openType">Xml open type.</param>
    /// <returns>Returns opened workbook.</returns>
    IWorkbook OpenFromXml( string strPath, ExcelXmlOpenType openType );
#endif
#if ( WINRT || WP)
    /// <summary>
    /// Reads workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(Stream);
    /// </example>
    Task<IWorkbook> OpenAsync(Stream stream);
    /// <summary>
    /// Reads workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(Stream, ExcelOpenType);
    /// </example>
    Task<IWorkbook> OpenAsync(Stream stream, ExcelOpenType openType);
    /// <summary>
    /// Reads workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="openType">Represents open type for open.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(Stream, ExcelOpenType, ExcelParseOptions);
    /// </example>
    Task<IWorkbook> OpenAsync(Stream stream, ExcelOpenType openType, ExcelParseOptions options);
    /// <summary>
    /// Reads workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="isReadOnly">Indicates is open book in read - only mode.</param>
    /// <param name="password">Represents valid password for opening workbook.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(Stream, ExcelParseOptions, bool, string);
    /// </example>    
    Task<IWorkbook> OpenAsync(Stream stream, ExcelParseOptions options, bool isReadOnly, string password);
    /// <summary>
    /// Reads workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="isReadOnly">Indicates is open book in read - only mode.</param>
    /// <param name="password">Represents valid password for opening workbook.</param>
    /// <param name="excelVersion">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(Stream, ExcelParseOptions, bool, string, ExcelVersion);
    /// </example>    
    Task<IWorkbook> OpenAsync(Stream stream, ExcelParseOptions options, bool isReadOnly, string password, ExcelVersion excelVersion);
    /// <summary>
    /// Reads workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="version">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(Stream, ExcelVersion);
    /// </example>        
    Task<IWorkbook> OpenAsync(Stream stream, ExcelVersion version);
    /// <summary>
    /// Reads workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="version">Workbook version.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(Stream, ExcelVersion, ExcelParseOptions);
    /// </example>        
    Task<IWorkbook> OpenAsync(Stream stream, ExcelVersion version, ExcelParseOptions options);
    /// <summary>
    /// Reads workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="separator">Current seperator.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(Stream, string);
    /// </example>
    Task<IWorkbook> OpenAsync(Stream stream, string separator);
    /// <summary>
    /// Reads workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="separator">Current seperator.</param>
    /// <param name="encoding">Encoding to use to parse text data.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(Stream, string, Encoding);
    /// </example>
    Task<IWorkbook> OpenAsync(Stream stream, string separator, Encoding encoding);
    /// <summary>
    /// Reads workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="separator">Current seperator.</param>
    /// <param name="version">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(Stream, string, ExcelVersion);
    /// </example>
    Task<IWorkbook> OpenAsync(Stream stream, string separator, ExcelVersion version);
    /// <summary>
    /// Reads workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="separator">Separator to use.</param>
    /// <param name="row">Number of first row to write</param>
    /// <param name="column">Number of first column to write.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(Stream, string, int, int);
    /// </example>
    Task<IWorkbook> OpenAsync(Stream stream, string separator, int row, int column);
    /// <summary>
    /// Reads workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="separator">Separator to use.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <param name="encoding">Encoding to use to parse text data.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(Stream, string, int, int, Encoding);
    /// </example>
    Task<IWorkbook> OpenAsync(Stream stream, string separator, int row, int column, Encoding encoding);
    /// <summary>
    /// Reads workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="separator">Separator to use.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="encoding">Encoding to use to parse text data.</param>
    /// <param name="version">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(Stream, string, int, int, string, Encoding, ExcelVersion);
    /// </example>
    Task<IWorkbook> OpenAsync(Stream stream, string separator, int row, int column,string fileName, Encoding encoding, ExcelVersion version);
    /// <summary>
    /// Reads workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="openType">Represents open type for open.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(Stream, ExcelXmlOpenType);
    /// </example>
    Task<IWorkbook> OpenFromXmlAsync(Stream stream, ExcelXmlOpenType openType);
    /// <summary>
    /// Read workbook from the reader.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="openType">Represents open type for open.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(XmlReader, ExcelXmlOpenType);
    /// </example>
    Task<IWorkbook> OpenFromXmlAsync(XmlReader reader, ExcelXmlOpenType openType);
    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(StorageFile);
    /// </example>
    Task<IWorkbook> OpenAsync(StorageFile storageFile);
    /// <summary>
    /// Opens a workbook.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(StorageFile, ExcelOpenType);
    /// </example>
    Task<IWorkbook> OpenAsync(StorageFile storageFile, ExcelOpenType openType);
    /// <summary>
    /// Opens a workbook.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(StorageFile, ExcelOpenType, ExcelParseOptions);
    /// </example>
    Task<IWorkbook> OpenAsync(StorageFile storageFile, ExcelOpenType openType, ExcelParseOptions options);
    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="isReadOnly">Indicates is open book in read - only mode.</param>
    /// <param name="password">Represents valid password for opening workbook.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(StorageFile, ExcelParseOptions, bool, string);
    /// </example>
    Task<IWorkbook> OpenAsync(StorageFile storageFile, ExcelParseOptions options, bool isReadOnly, string password);
    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="isReadOnly">Indicates is open book in read - only mode.</param>
    /// <param name="password">Represents valid password for opening workbook.</param>
    /// <param name="excelVersion">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(StorageFile, ExcelParseOptions, bool, string, ExcelVersion);
    /// </example>
    Task<IWorkbook> OpenAsync(StorageFile storageFile, ExcelParseOptions options, bool isReadOnly, string password, ExcelVersion excelVersion);
    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="version">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(StorageFile, ExcelVersion);
    /// </example>
    Task<IWorkbook> OpenAsync(StorageFile storageFile, ExcelVersion version);
    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="version">Workbook version.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(StorageFile, ExcelVersion, ExcelParseOptions);
    /// </example>
    Task<IWorkbook> OpenAsync(StorageFile storageFile, ExcelVersion version, ExcelParseOptions options);
    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="separator">Current separator.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(StorageFile, string);
    /// </example>
    Task<IWorkbook> OpenAsync(StorageFile storageFile, string separator);
    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use to parse text data.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(StorageFile, string, Encoding);
    /// </example>
    Task<IWorkbook> OpenAsync(StorageFile storageFile, string separator, Encoding encoding);
    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="version">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(StorageFile, string, ExcelVersion);
    /// </example>
    Task<IWorkbook> OpenAsync(StorageFile storageFile, string separator, ExcelVersion version);
    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="separator">Separator to use.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(StorageFile, string, int, int);
    /// </example>
    Task<IWorkbook> OpenAsync(StorageFile storageFile, string separator, int row, int column);
    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="separator">Separator to use.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <param name="encoding">Encoding to use to parse text data.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(StorageFile, string, int, int, Ecoding);
    /// </example>
    Task<IWorkbook> OpenAsync(StorageFile storageFile, string separator, int row, int column, Encoding encoding);
    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="separator">Separator to use.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="encoding">Encoding to use to parse text data.</param>
    /// <param name="version">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(StorageFile, string, int, int, string, Encoding, ExcelVersion);
    /// </example>
    Task<IWorkbook> OpenAsync(StorageFile storageFile, string separator, int row, int column, string fileName, Encoding encoding, ExcelVersion version);
    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <returns>Opened workbook.</returns>
    /// <example> 
    /// ExcelEngine engine = new ExcelEngine();
    /// IWorkbook workbook = engine.Application.Workbooks.OpenAsync(StorageFile, ExcelXmlOpenType);
    /// </example>
    Task<IWorkbook> OpenFromXmlAsync(StorageFile storageFile, ExcelXmlOpenType openType);
#else
    /// <summary>
    /// Reads workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <returns>Newly created workbook.</returns>
    IWorkbook Open( Stream stream );
    /// <summary>
    /// Reads workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="version">Excel version - defines file format (excel 97-2003 or excel 2007).</param>
    /// <returns>Newly created workbook.</returns>
    IWorkbook Open( Stream stream, ExcelVersion version );
    /// <summary>
    /// Reads workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Newly created workbook.</returns>
    IWorkbook Open( Stream stream, ExcelParseOptions options );
    /// <summary>
    /// Opens a Workbook using separator.
    /// </summary>
    /// <param name="stream">Stream to open.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <returns>Opened WorkBook.</returns>
    IWorkbook Open( Stream stream, string separator, int row, int column );
    /// <summary>
    /// Opens a Workbook using separator.
    /// </summary>
    /// <param name="stream">Stream to open.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <param name="encoding">Encoding to use to parse text data.</param>
    /// <returns>Opened WorkBook.</returns>
    IWorkbook Open( Stream stream, string separator, int row, int column, Encoding encoding );
    /// <summary>
    /// Opens a Workbook using separator.
    /// </summary>
    /// <param name="stream">Stream to open.</param>
    /// <param name="separator">Current separator.</param>
    /// <returns>Opened WorkBook.</returns>
    IWorkbook Open( Stream stream, string separator );
    /// <summary>
    /// Opens a Workbook using separator.
    /// </summary>
    /// <param name="stream">Stream to open.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use to parse text data.</param>
    /// <returns>Opened WorkBook.</returns>
    IWorkbook Open( Stream stream, string separator, Encoding encoding );
    /// <summary>
    /// Opens the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="options"></param>
    /// <param name="bReadOnly"></param>
    /// <param name="password">Password that should be used for decryption.</param>
    /// <returns>Newly created workbook.</returns>
    IWorkbook Open(Stream stream, ExcelParseOptions options, bool bReadOnly, string password);
    /// <summary>
    /// Opens the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="isReadOnly">Indicates is open book in read - only mode.</param>
    /// <param name="password">Represents valid password for opening workbook.</param>
    /// <param name="version">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    IWorkbook Open(Stream stream, ExcelParseOptions options, bool isReadOnly, string password, ExcelVersion version);
    /// <summary>
    /// Opens the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="isReadOnly">Indicates is open book in read - only mode.</param>
    /// <param name="password">Represents valid password for opening workbook.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <returns>Opened workbook.</returns>
    IWorkbook Open(Stream stream, ExcelParseOptions options, bool isReadOnly,string password, ExcelOpenType openType);
    /// <summary>
    /// Opens  a workbook.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="openType">Represents open type for open.</param>
    /// <returns>Opened workbook.</returns>
    IWorkbook Open( Stream stream, ExcelOpenType openType );
    /// <summary>
    /// Opens  a workbook.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="openType">Represents open type for open.</param>
    /// <returns>Opened workbook.</returns>
    IWorkbook Open( Stream stream, ExcelOpenType openType, ExcelParseOptions options );
    /// <summary>
    /// Opens  a workbook.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="openType">Represents open type for open.</param>
    /// <param name="version">Desired version of the workbook.</param>
    /// <returns>Opened workbook.</returns>
    IWorkbook Open( Stream stream, ExcelOpenType openType, ExcelVersion version );
    /// <summary>
    /// Read workbook from file stream.
    /// </summary>
    /// <param name="stream">File stream.</param>
    /// <param name="openType">Xml open type.</param>
    /// <returns>Returns opened workbook.</returns>
    IWorkbook OpenFromXml( Stream stream, ExcelXmlOpenType openType );
    /// <summary>
    /// Read workbook from xml stream.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="openType">Xml open type.</param>
    /// <returns>Returns opened workbook.</returns>
    IWorkbook OpenFromXml( XmlReader reader, ExcelXmlOpenType openType );
#endif
    /// <summary>
    /// Closes the object.
    /// </summary>
    void Close();
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Pastes workbook from clipboard.
    /// </summary>
    /// <returns></returns>
    IWorkbook PasteWorkbook();
#endif

    #endregion
  }
}

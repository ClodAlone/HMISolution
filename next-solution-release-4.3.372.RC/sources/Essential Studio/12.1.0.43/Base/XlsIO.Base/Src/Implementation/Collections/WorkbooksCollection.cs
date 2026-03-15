#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

#region file using directives
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;
using System.Text;
//using System.Windows.Forms;
using System.Xml;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Parser;

//using Syncfusion.XlsIO.IO;
//using Syncfusion.XlsIO.IO.Stream;

using Syncfusion.CompoundFile.XlsIO;
using Syncfusion.XlsIO.Implementation.Security;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.Compression.Zip;
#if ( WINRT || WP)
using System.Threading.Tasks;
using Windows.Storage;
#endif

#if  (SILVERLIGHT) || (WINRT) || (WP)

#else
using Syncfusion.XlsIO.Implementation.Clipboard;
#endif

#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// A collection of all the Workbook objects that are currently open in
  /// the XlsIO application.
  /// </summary>
  public class WorkbooksCollection
    : CollectionBaseEx<object>
    , IWorkbooks
  {
    #region Class constants
    /// <summary>
    /// Represents default header for .xls and .xlt files.
    /// </summary>
    private static readonly byte[] DEF_XLS_FILE_HEADER =
    {
      0xd0, 0xcf, 0x11, 0xe0, 0xa1, 0xb1, 0x1a, 0xe1
    };
    /// <summary>
    /// Represents size of biff file header.
    /// </summary>
    private const byte DEF_BIFF_HEADER_SIZE = 8;
    /// <summary>
    /// Represents xml header.
    /// </summary>
    private const string DEF_XML_HEADER = "<?xml";
    /// <summary>
    /// Represents xml header.
    /// </summary>
    private const string DEF_HTML_HEADER = "<html";
    /// <summary>
    /// Represents default buffer size for auto recognize.
    /// </summary>
    private const int DEF_BUFFER_SIZE = 512;
    #endregion

    #region IWorkbooks Properties
    /// <summary>
    /// Returns a single object from a collection.
    /// </summary>
    public IWorkbook this[ int Index ]
    {
      get
      {
        return ( IWorkbook )InnerList[ Index ];
      }
    }

    #endregion

    #region IWorkbooks Methods
    /// <summary>
    /// Create workbook with names.Length quantity of worksheets.
    /// Each worksheet name will be set to corresponding names array element.
    /// </summary>
    /// <param name="names">Array of names for each worksheet.</param>
    /// <returns>Interface on instance of created workbook.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When names array is NULL.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// When names does not contain any value.
    /// </exception>
    public IWorkbook Create( string[] names )
    {
      if( names == null )
        throw new ArgumentNullException( "names" );

      if( names.Length == 0 )
        throw new ArgumentException( "Names array must contain at least one name." );

      WorkbookImpl book = AppImplementation.CreateWorkbook( this, names.Length, Application.DefaultVersion );

      // set names
      for( int i=0; i<names.Length; i++ )
      {
        book.Worksheets[ i ].Name = names[ i ];
      }

      base.Add( book );
      book.Activate();
      SetAplicatioName(book);
      return book;
    }
    /// <summary>
    /// Create workbook with specified number of empty worksheets.
    /// </summary>
    /// <param name="sheetsQuantity">Number of worksheets to create.</param>
    /// <returns>Interface on instance of created workbook.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When sheetsQuantity is less than zero.
    /// </exception>
    public IWorkbook Create( int sheetsQuantity )
    {
      if( sheetsQuantity < 0 )
      {
        throw new ArgumentOutOfRangeException( "sheetsQuantity",
          "Quantity of worksheets must be greater than zero." );
      }

      WorkbookImpl book = AppImplementation.CreateWorkbook( this, sheetsQuantity, Application.DefaultVersion );
      if (Application.DefaultVersion == ExcelVersion.Excel97to2003)
      book.BeginVersion = 2;
      base.Add( book );
      book.Activate();
      SetAplicatioName(book);
      return book;
    }

    /// <summary>
    /// Create new empty workbook and make it active.
    /// </summary>
    /// <returns>Interface on instance of newly created workbook.</returns>
    public IWorkbook Create()
    {
      WorkbookImpl book = AppImplementation.CreateWorkbook( this, Application.DefaultVersion );
      if (Application.DefaultVersion==ExcelVersion.Excel97to2003)
      book.BeginVersion = 2;
      base.Add( book );
      book.Activate();
      SetAplicatioName(book);
      return book;
    }
    /// <summary>
    /// Sets the Application name as Essential XlsIO.
    /// </summary>
    /// <param name="book"></param>
    private void SetAplicatioName(IWorkbook book)
    {
        book.BuiltInDocumentProperties.ApplicationName = Syncfusion.XlsIO.Implementation.XmlSerialization.Constants.DocProp.EssentialXlsIO;
    }
#if !(WINRT || WP)
    /// <summary>
    /// Creates a new workbook. The new workbook becomes the active workbook.
    /// Returns a Workbook object.
    /// </summary>
    /// <returns>Added Workbook object.</returns>
    public IWorkbook Add()
    {
      return Add( null, Application.DefaultVersion );
    }
    /// <summary>
    /// Creates a new workbook. The new workbook becomes the active workbook.
    /// </summary>
    /// <param name="version">Excel version - defines file format (excel 97-2003 or excel 2007).</param>
    /// <returns>Added Workbook object.</returns>
    public IWorkbook Add( ExcelVersion version )
    {
      return Add( null, version );
    }
    /// <summary>
    /// Creates a new workbook. The new workbook becomes the active workbook.
    /// Returns a Workbook object.
    /// </summary>
    /// <param name="strTemplateFile">File that contains required workbook.</param>
    /// <returns>Added Workbook object.</returns>
    public IWorkbook Add( string strTemplateFile )
    {   
        return Add(strTemplateFile, ExcelParseOptions.Default);
    }
    /// <summary>
    /// Creates a new workbook. The new workbook becomes the active workbook.
    /// Returns a Workbook object.
    /// </summary>
    /// <param name="strTemplateFile">File that contains required workbook.</param>
    /// <param name="version">Excel version - defines file format (excel 97-2003 or excel 2007).</param>
    /// <returns>Added Workbook object.</returns>
    public IWorkbook Add( string strTemplateFile, ExcelVersion version )
    {
      return Add( strTemplateFile, ExcelParseOptions.Default, version );
    }
    /// <summary>
    /// Adds workbook to the collection.
    /// </summary>
    /// <param name="strTemplateFile">File to parse.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Created workbook.</returns>
    public IWorkbook Add( string strTemplateFile, ExcelParseOptions options )
    {
      ExcelVersion version =  DetectVersion(strTemplateFile);        
      return Add( strTemplateFile, options,  version);
    }
    /// <summary>
    /// Creates a new workbook. The new workbook becomes the active workbook.
    /// Returns a Workbook object.
    /// </summary>
    /// <param name="strTemplateFile">File that contains required workbook.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="version">Excel version - defines file format (excel 97-2003 or excel 2007).</param>
    /// <returns>Added Workbook object.</returns>
    public IWorkbook Add( string strTemplateFile, ExcelParseOptions options, ExcelVersion version )
    {
      WorkbookImpl book = null;

      if( strTemplateFile == null )
      {
        book = AppImplementation.CreateWorkbook( this, version );
      }
      else
      {
        book = AppImplementation.CreateWorkbook( this, strTemplateFile, options, version );
      }

      base.Add( book );
      book.Activate();
      book.Worksheets[ 0 ].Activate();
      return book;
    }
    /// <summary>
    /// Opens a workbook.
    /// </summary>
    /// <param name="filename">File name that contains workbook.</param>
    /// <returns>Opened workbook.</returns>
    public IWorkbook Open( string filename )
    {
      return Open( filename, ExcelOpenType.Automatic );
    }
    /// <summary>
    /// Opens a workbook.
    /// </summary>
    /// <param name="filename">File name that contains workbook.</param>
    /// <param name="version">Version of the excel file format.</param>
    /// <returns>Opened workbook.</returns>
    public IWorkbook Open( string filename, ExcelVersion version )
    {
      //WorkbookImpl book = new WorkbookImpl( Application, this, Filename );
      WorkbookImpl book = AppImplementation.CreateWorkbook( this, filename, version );
      book.InternalSaved = true;

      // Store workbook into collection and return it.
      base.Add( book );
      return book;
    }
#endif
    /// <summary>
    /// Opens a Workbook using separator.
    /// </summary>
    /// <param name="stream">Stream to open.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <returns>Opened WorkBook.</returns>
    public IWorkbook Open( Stream stream, string separator, int row, int column )
    {
      return Open( stream, separator, row, column, null, null, Application.DefaultVersion );
    }
    /// <summary>
    /// Opens a Workbook using separator.
    /// </summary>
    /// <param name="stream">Stream to open.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <param name="encoding">Encoding to use to parse text data.</param>
    /// <returns>Opened WorkBook.</returns>
    public IWorkbook Open( Stream stream, string separator, int row, int column, Encoding encoding )
    {
      return Open( stream, separator, row, column, null, encoding, Application.DefaultVersion );
    }
    /// <summary>
    /// Opens a Workbook using separator.
    /// </summary>
    /// <param name="stream">Stream to open.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <param name="fileName">Name of the file that is being opened.</param>
    /// <returns>Opened WorkBook.</returns>
    private IWorkbook Open( Stream stream, string separator, int row, int column,
      string fileName, Encoding encoding, ExcelVersion version )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      if( separator == null )
        throw new ArgumentNullException( "separator" );

      if( separator.Length == 0 )
        throw new ArgumentException( "separator" );
      return OpenInternal(stream, separator, row, column, fileName, encoding, version);
    }

    /// <summary>
    /// Opens a Workbook using separator.
    /// </summary>
    /// <param name="stream">Stream to open.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <param name="fileName">Name of the file that is being opened.</param>
    /// <returns>Opened WorkBook.</returns>
    private IWorkbook OpenInternal(Stream stream, string separator, int row, int column,
      string fileName, Encoding encoding, ExcelVersion version)
    {

        WorkbookImpl book = AppImplementation.CreateWorkbook(this, stream,
          separator, row, column, version, fileName, encoding);

        base.Add(book);
        return book;
    }
#if !(WINRT || WP)
    /// <summary>
    /// Opens a Workbook using separator.
    /// </summary>
    /// <param name="fileName">File name to open.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <returns>Opened WorkBook.</returns>
    public IWorkbook Open( string fileName, string separator, int row, int column )
    {
      return Open( fileName, separator, row, column, null );
    }
    /// <summary>
    /// Opens a Workbook using separator.
    /// </summary>
    /// <param name="fileName">File name to open.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <returns>Opened WorkBook.</returns>
    public IWorkbook Open( string fileName, string separator, int row, int column, Encoding encoding )
    {
      if( fileName == null )
        throw new ArgumentNullException( "fileName" );

      if( fileName.Length == 0 )
        throw new ArgumentException( "fileName" );

      using( FileStream streamToRead = new FileStream( fileName, FileMode.Open, FileAccess.Read, FileShare.Read ) )
      {
        WorkbookImpl result = ( WorkbookImpl )Open( streamToRead, separator, row, column, fileName,
          encoding, Application.DefaultVersion );
        result.FullFileName = Path.GetFullPath( fileName );
        result.InternalSaved = true;
        return result;
      }
    }
#endif
    /// <summary>
    /// Opens a Workbook using separator.
    /// </summary>
    /// <param name="stream">Stream to open.</param>
    /// <param name="separator">Current separator.</param>
    /// <returns>Opened WorkBook.</returns>
    public IWorkbook Open( Stream stream, string separator )
    {
      return Open( stream, separator, 1, 1, null );
    }
    /// <summary>
    /// Opens a Workbook using separator.
    /// </summary>
    /// <param name="stream">Stream to open.</param>
    /// <param name="separator">Current separator.</param>
    /// <returns>Opened WorkBook.</returns>
    public IWorkbook Open( Stream stream, string separator, ExcelVersion version )
    {
      return Open( stream, separator, 1, 1, null, null, version );
    }
#if !(WINRT || WP)
    /// <summary>
    /// Opens a Workbook using separator.
    /// </summary>
    /// <param name="fileName">Stream to open.</param>
    /// <param name="separator">Current separator.</param>
    /// <returns>Opened WorkBook.</returns>
    public IWorkbook Open( string fileName, string separator )
    {
      return Open( fileName, separator, 1, 1 );
    }
#endif
#if ( WINRT || WP)
    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="separator">Separator to use.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="encoding">Encoding to use to parse text data.</param>
    /// <param name="version">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    public Task<IWorkbook> OpenAsync(Stream stream, string separator, int row, int column,string fileName, Encoding encoding, ExcelVersion version)
    {
        if (stream == null)
            throw new ArgumentNullException("stream");

        if (separator == null)
            throw new ArgumentNullException("separator");

        if (separator.Length == 0)
            throw new ArgumentException("separator");

        return OpenAsyncInternal(stream, separator, row, column, fileName, encoding, version);
    }

    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="separator">Separator to use.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="encoding">Encoding to use to parse text data.</param>
    /// <param name="version">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    private async Task<IWorkbook> OpenAsyncInternal(Stream stream, string separator, int row, int column, string fileName,Encoding encoding, ExcelVersion version)
    {
        TaskCompletionSource<IWorkbook> taskCompletionSource = new TaskCompletionSource<IWorkbook>();
        IWorkbook book = null;
        await Task.Run(() =>
        {
            try
            {
                book = OpenInternal(stream, separator, row, column, fileName, encoding, version);
                if (book == null)
                    taskCompletionSource.SetException(new ApplicationException("Unable to read from XML."));
                else
                    taskCompletionSource.SetResult(book);
            }
            catch (Exception exception)
            {
                taskCompletionSource.SetException(exception);
            }
        });
        return await taskCompletionSource.Task;
    }

    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="separator">Separator to use.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <param name="encoding">Encoding to use to parse text data.</param>
    /// <returns>Opened workbook.</returns>
    public Task<IWorkbook> OpenAsync(Stream stream, string separator, int row, int column, Encoding encoding)
    {
        return OpenAsync(stream, separator, row, column, null, encoding, Application.DefaultVersion);
    }

    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="separator">Separator to use.</param>
    /// <param name="row">Number of first row to write</param>
    /// <param name="column">Number of first column to write.</param>
    /// <returns>Opened workbook.</returns>
    public Task<IWorkbook> OpenAsync(Stream stream, string separator, int row, int column)
    {
        return OpenAsync(stream, separator, row, column, null, null, Application.DefaultVersion);
    }

    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <returns>Opened workbook.</returns>
    public Task<IWorkbook> OpenAsync(Stream stream)
    {
        return OpenAsync(stream, ExcelOpenType.Automatic);
    }

    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <returns>Opened workbook.</returns>
    public Task<IWorkbook> OpenAsync(Stream stream, ExcelOpenType openType)
    {
        if (stream == null)
            throw new ArgumentNullException("stream");

        openType = DetectFileFromStream(stream, openType);

        switch (openType)
        {
            case ExcelOpenType.BIFF:
                return OpenAsync(stream, ExcelVersion.Excel97to2003);

            case ExcelOpenType.SpreadsheetML:
                return OpenFromXmlAsync(stream, ExcelXmlOpenType.MSExcel);

            case ExcelOpenType.CSV:
                return OpenAsync(stream, Application.CSVSeparator);

            case ExcelOpenType.SpreadsheetML2007:
                return OpenAsync(stream, ExcelVersion.Excel2007);

            case ExcelOpenType.SpreadsheetML2010:
                return OpenAsync(stream, ExcelVersion.Excel2010);

            default:
                throw new ArgumentOutOfRangeException("openType");
            //return DetectFileFromStream( stream );
        }
    }

    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="openType">Represents open type for open.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Opened workbook.</returns>
    public Task<IWorkbook> OpenAsync(Stream stream, ExcelOpenType openType, ExcelParseOptions options)
    {
        if (stream == null)
            throw new ArgumentNullException("stream");

        openType = DetectFileFromStream(stream, openType);

        switch (openType)
        {
            case ExcelOpenType.BIFF:
                return OpenAsync(stream, ExcelVersion.Excel97to2003, options);

            case ExcelOpenType.SpreadsheetML:
                return OpenFromXmlAsync(stream, ExcelXmlOpenType.MSExcel);

            case ExcelOpenType.CSV:
                return OpenAsync(stream, Application.CSVSeparator);

            case ExcelOpenType.SpreadsheetML2007:
                return OpenAsync(stream, ExcelVersion.Excel2007);

            case ExcelOpenType.SpreadsheetML2010:
                return OpenAsync(stream, ExcelVersion.Excel2010);

            default:
                throw new ArgumentOutOfRangeException("openType");
        }
    }
    
    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="isReadOnly">Indicates is open book in read - only mode.</param>
    /// <param name="password">Represents valid password for opening workbook.</param>
    /// <returns>Opened workbook.</returns>
    public Task<IWorkbook> OpenAsync(Stream stream, ExcelParseOptions options, bool isReadOnly, string password)
    {
        return OpenAsync(stream, options, isReadOnly, password, Application.DefaultVersion);
    }

    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="isReadOnly">Indicates is open book in read - only mode.</param>
    /// <param name="password">Represents valid password for opening workbook.</param>
    /// <param name="excelVersion">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    public Task<IWorkbook> OpenAsync(Stream stream, ExcelParseOptions options, bool isReadOnly, string password, ExcelVersion excelVersion)
    {
        return OpenAsyncInternal(stream, options, isReadOnly, password, excelVersion);
    }

    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="isReadOnly">Indicates is open book in read - only mode.</param>
    /// <param name="password">Represents valid password for opening workbook.</param>
    /// <param name="excelVersion">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    private async Task<IWorkbook> OpenAsyncInternal(Stream stream, ExcelParseOptions options, bool isReadOnly, string password, ExcelVersion excelVersion)
    {
        TaskCompletionSource<IWorkbook> taskCompletionSource = new TaskCompletionSource<IWorkbook>();
        IWorkbook book = null;
        await Task.Run(() =>
        {
            try
            {
                book = Open(stream, options, isReadOnly, password, excelVersion);
                if (book == null)
                    taskCompletionSource.SetException(new ApplicationException("Unable to read from XML."));
                else
                    taskCompletionSource.SetResult(book);
            }
            catch (Exception exception)
            {
                taskCompletionSource.SetException(exception);
            }
        });
        return await taskCompletionSource.Task;
    }	

    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="version">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    public Task<IWorkbook> OpenAsync(Stream stream, ExcelVersion version)
    {
        return OpenAsync(stream, version, ExcelParseOptions.Default);
    }

    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="version">Workbook version.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Opened workbook.</returns>
    public Task<IWorkbook> OpenAsync(Stream stream, ExcelVersion version, ExcelParseOptions options)
    {
        if (stream == null)
            throw new ArgumentNullException("stream should not be null");

        return OpenAsyncInternal(stream, version, options);
    }

    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="version">Workbook version.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Opened workbook.</returns>
    private async Task<IWorkbook> OpenAsyncInternal(Stream stream, ExcelVersion version, ExcelParseOptions options)
    {
        TaskCompletionSource<IWorkbook> taskCompletionSource = new TaskCompletionSource<IWorkbook>();
        IWorkbook book = null;
        await Task.Run(() =>
        {
            try
            {
                book = Open(stream, version, options);
                taskCompletionSource.SetResult(book);
            }
            catch (Exception exception)
            {
                taskCompletionSource.SetException(exception);
            }
        }
          );

        return await taskCompletionSource.Task;
    }

    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="separator">Current seperator.</param>
    /// <returns>Opened workbook.</returns>
    public Task<IWorkbook> OpenAsync(Stream stream, string separator)
    {
        return OpenAsync(stream, separator, 1, 1, null);
    }
 
    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="separator">Current seperator.</param>
    /// <param name="version">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    public Task<IWorkbook> OpenAsync(Stream stream, string separator, ExcelVersion version)
    {
        return OpenAsync(stream, separator, 1, 1, null, null, version);
    }

    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="openType">Represents open type for open.</param>
    /// <returns>Opened workbook.</returns>
    public Task<IWorkbook> OpenFromXmlAsync(Stream stream, ExcelXmlOpenType openType)
    {
        if (stream == null)
            throw new ArgumentNullException("stream");
        XmlReader reader = UtilityMethods.CreateReader(stream, false);
        return OpenFromXmlAsync(reader, openType);
    }

    /// <summary>
    /// Read workbook from the reader.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="openType">Represents open type for open.</param>
    /// <returns>Opened workbook.</returns>
    public Task<IWorkbook> OpenFromXmlAsync(XmlReader reader, ExcelXmlOpenType openType)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        return OpenFromXmlAsyncInternal(reader, openType);
    }

    /// <summary>
    /// Read workbook from the reader.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="openType">Represents open type for open.</param>
    /// <returns>Opened workbook.</returns>
    private async Task<IWorkbook> OpenFromXmlAsyncInternal(XmlReader reader, ExcelXmlOpenType openType)
    {
        TaskCompletionSource<IWorkbook> taskCompletionSource=new TaskCompletionSource<IWorkbook>();
        IWorkbook book=null;
        await Task.Run(() =>
            {
                try
                {
                    book = OpenFromXmlInternal(reader, openType);
                    if (book == null)
                        taskCompletionSource.SetException(new ApplicationException("Unable to read from XML."));
                    else
                        taskCompletionSource.SetResult(book);
                }
                catch (Exception exception)
                {
                    taskCompletionSource.SetException(exception);
                }
            });
        return await taskCompletionSource.Task;
    }

    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="separator">Current seperator.</param>
    /// <param name="encoding">Encoding to use to parse text data.</param>
    /// <returns>Opened workbook.</returns>
    public Task<IWorkbook> OpenAsync(Stream stream, string separator, Encoding encoding)
    {
        return OpenAsync(stream, separator, 1, 1, encoding);
    }

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
    public async Task<IWorkbook> OpenAsync(StorageFile storageFile, string separator, int row, int column,string fileName, Encoding encoding, ExcelVersion version)
    {
        if (storageFile == null)
            throw new ArgumentNullException("storageFile");

        if (separator == null)
            throw new ArgumentNullException("separator");

        if (separator.Length == 0)
            throw new ArgumentException("separator");

        Stream stream = await storageFile.OpenStreamForReadAsync();

        return await OpenAsyncInternal(stream, separator, row, column, fileName, encoding, version);
    }

    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="separator">Separator to use.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <param name="encoding">Encoding to use to parse text data.</param>
    /// <returns>Opened workbook.</returns>
    public async Task<IWorkbook> OpenAsync(StorageFile storageFile, string separator, int row, int column, Encoding encoding)
    {
        Stream stream = await storageFile.OpenStreamForReadAsync();
        return await OpenAsync(stream, separator, row, column, null, encoding, Application.DefaultVersion);
    }

    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="separator">Separator to use.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <returns>Opened workbook.</returns>
    public async Task<IWorkbook> OpenAsync(StorageFile storageFile, string separator, int row, int column)
    {
        Stream stream = await storageFile.OpenStreamForReadAsync();
        return await OpenAsync(stream, separator, row, column, null, null, Application.DefaultVersion);
    }

    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <returns>Opened workbook.</returns>
    public async Task<IWorkbook> OpenAsync(StorageFile storageFile)
    {
        Stream stream = await storageFile.OpenStreamForReadAsync();
        return await OpenAsync(stream);
    }

    /// <summary>
    /// Opens a workbook.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <returns>Opened workbook.</returns>
    public async Task<IWorkbook> OpenAsync(StorageFile storageFile, ExcelOpenType openType)
    {
        if (storageFile == null)
            throw new ArgumentNullException("storageFile");

        Stream stream = await storageFile.OpenStreamForReadAsync();
        return await OpenAsync(stream, openType);
    }

    /// <summary>
    /// Opens a workbook.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Opened workbook.</returns>
    public async Task<IWorkbook> OpenAsync(StorageFile storageFile, ExcelOpenType openType, ExcelParseOptions options)
    {
        if (storageFile == null)
            throw new ArgumentNullException("storageFile");

        Stream stream = await storageFile.OpenStreamForReadAsync();
        return await OpenAsync(stream, openType, options);
    }

    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="isReadOnly">Indicates is open book in read - only mode.</param>
    /// <param name="password">Represents valid password for opening workbook.</param>
    /// <returns>Opened workbook.</returns>
    public async Task<IWorkbook> OpenAsync(StorageFile storageFile, ExcelParseOptions options, bool isReadOnly, string password)
    {
        Stream stream = await storageFile.OpenStreamForReadAsync();
        return await OpenAsync(stream, options, isReadOnly, password, Application.DefaultVersion);
    }

    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="isReadOnly">Indicates is open book in read - only mode.</param>
    /// <param name="password">Represents valid password for opening workbook.</param>
    /// <param name="excelVersion">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    public async Task<IWorkbook> OpenAsync(StorageFile storageFile, ExcelParseOptions options, bool isReadOnly, string password, ExcelVersion excelVersion)
    {
        Stream stream = await storageFile.OpenStreamForReadAsync();
        return await OpenAsyncInternal(stream, options, isReadOnly, password, excelVersion);
    }

    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="version">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    public async Task<IWorkbook> OpenAsync(StorageFile storageFile, ExcelVersion version)
    {
        Stream stream = await storageFile.OpenStreamForReadAsync();
        return await OpenAsync(stream, version, ExcelParseOptions.Default);
    }

    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="version">Workbook version.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Opened workbook.</returns>
    public async Task<IWorkbook> OpenAsync(StorageFile storageFile, ExcelVersion version, ExcelParseOptions options)
    {
        if (storageFile == null)
            throw new ArgumentNullException("storageFile should not be null");

        Stream stream = await storageFile.OpenStreamForReadAsync();
        return await OpenAsyncInternal(stream, version, options);
    }

    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="separator">Current separator.</param>
    /// <returns>Opened workbook.</returns>
    public async Task<IWorkbook> OpenAsync(StorageFile storageFile, string separator)
    {
        Stream stream = await storageFile.OpenStreamForReadAsync();
        return await OpenAsync(stream, separator, 1, 1, null);
    }

    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="version">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    public async Task<IWorkbook> OpenAsync(StorageFile storageFile, string separator, ExcelVersion version)
    {
        Stream stream = await storageFile.OpenStreamForReadAsync();
        return await OpenAsync(stream, separator, 1, 1, null, null, version);
    }

    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <returns>Opened workbook.</returns>
    public async Task<IWorkbook> OpenFromXmlAsync(StorageFile storageFile, ExcelXmlOpenType openType)
    {
        if (storageFile == null)
            throw new ArgumentNullException("storageFile");

        Stream stream = await storageFile.OpenStreamForReadAsync();

        XmlReader reader = UtilityMethods.CreateReader(stream, false);
        return await OpenFromXmlAsync(reader, openType);
    }

    /// <summary>
    /// Opens a random-access storage over the file.
    /// </summary>
    /// <param name="storageFile">Provides information about the file and its content, and ways to manipulate them.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use to parse text data.</param>
    /// <returns>Opened workbook.</returns>
    public async Task<IWorkbook> OpenAsync(StorageFile storageFile, string separator, Encoding encoding)
    {
        Stream stream = await storageFile.OpenStreamForReadAsync();
        return await OpenAsync(stream, separator, 1, 1, encoding);
    }

#endif
    /// <summary>
    /// Opens a Workbook using separator.
    /// </summary>
    /// <param name="stream">Stream to open.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use to parse text data.</param>
    /// <returns>Opened WorkBook.</returns>
    public IWorkbook Open( Stream stream, string separator, Encoding encoding )
    {
      return Open( stream, separator, 1, 1, encoding );
    }
#if !(WINRT || WP)
    /// <summary>
    /// Opens a Workbook using separator.
    /// </summary>
    /// <param name="fileName">Stream to open.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use to parse text data.</param>
    /// <returns>Opened WorkBook.</returns>
    public IWorkbook Open( string fileName, string separator, Encoding encoding )
    {
      return Open( fileName, separator, 1, 1, encoding );
    }
    /// <summary>
    /// Opens a workbook.
    /// </summary>
    /// <param name="fileName">File name that contains workbook.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Opened workbook.</returns>
    public IWorkbook Open( string fileName, ExcelParseOptions options )
    {
      //WorkbookImpl book = AppImplementation.CreateWorkbook( this, fileName,
      //  options, false, null, Application.DefaultVersion );

      //// Store workbook into collection and return it.
      //base.Add( book );
      //return book;

        return Open(fileName, ExcelOpenType.Automatic, options);
    }
    /// <summary>
    /// Opens a workbook.
    /// </summary>
    /// <param name="fileName">File name that contains workbook.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="isReadOnly">Indicates is open book in read - only mode.</param>
    /// <param name="password">Represents valid password for opening workbook.</param>
    /// <returns>Opened workbook.</returns>
    public IWorkbook Open( string fileName, ExcelParseOptions options, bool isReadOnly, string password )
    {
      return Open( fileName, options, isReadOnly, password, Application.DefaultVersion );
    }
#endif
    /// <summary>
    /// Opens the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="isReadOnly">Indicates is open book in read - only mode.</param>
    /// <param name="password">Represents valid password for opening workbook.</param>
    /// <returns>Opened workbook.</returns>
    public IWorkbook Open(Stream stream, ExcelParseOptions options, bool isReadOnly, string password)
    {
        return Open(stream, options, isReadOnly, password, Application.DefaultVersion);
    }

    /// <summary>
    /// Opens the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="isReadOnly">Indicates is open book in read - only mode.</param>
    /// <param name="password">Represents valid password for opening workbook.</param>
    /// <param name="version">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    public IWorkbook Open(Stream stream, ExcelParseOptions options, bool isReadOnly, string password, ExcelVersion excelVersion)
    {
        WorkbookImpl book = AppImplementation.CreateWorkbook(this, stream,
         options, isReadOnly, password, excelVersion);

        // Store workbook into collection and return it.
        base.Add(book);
        return book; 
    }
    /// <summary>
    /// Opens the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="isReadOnly">Indicates is open book in read - only mode.</param>
    /// <param name="password">Represents valid password for opening workbook.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <returns>Opened workbook.</returns>
    public IWorkbook Open(Stream stream, ExcelParseOptions options, bool isReadOnly,
      string password, ExcelOpenType openType)
    {      
        openType = DetectFileFromStream(stream, openType);       

        switch (openType)
        {
            case ExcelOpenType.BIFF:
                return Open(stream, options, isReadOnly, password, ExcelVersion.Excel97to2003);

            case ExcelOpenType.SpreadsheetML:
                return OpenFromXml(stream, ExcelXmlOpenType.MSExcel);

            case ExcelOpenType.CSV:
                return Open(stream, Application.CSVSeparator);

            case ExcelOpenType.SpreadsheetML2007:
                return Open(stream, options, isReadOnly, password, ExcelVersion.Excel2007);

            case ExcelOpenType.SpreadsheetML2010:
                return Open(stream, options, isReadOnly, password, ExcelVersion.Excel2010);

            default:
                throw new ArgumentOutOfRangeException("openType");
        }
    }
#if !(WINRT || WP)
    /// <summary>
    /// Opens a workbook.
    /// </summary>
    /// <param name="fileName">File name that contains workbook.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="isReadOnly">Indicates is open book in read - only mode.</param>
    /// <param name="password">Represents valid password for opening workbook.</param>
    /// <param name="version">Workbook version.</param>
    /// <returns>Opened workbook.</returns>
    public IWorkbook Open( string fileName, ExcelParseOptions options, bool isReadOnly,
      string password, ExcelVersion version )
    {
      //// We support password protection only for Excel97to2003.
      //ExcelVersion version = ( password == null ) ?
      //  Application.DefaultVersion :
      //  ExcelVersion.Excel97to2003;

      WorkbookImpl book = AppImplementation.CreateWorkbook( this, fileName,
        options, isReadOnly, password, version );

      // Store workbook into collection and return it.
      base.Add( book );
      return book;
    }
    /// <summary>
    /// Opens a workbook.
    /// </summary>
    /// <param name="fileName">File name that contains workbook.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="isReadOnly">Indicates is open book in read - only mode.</param>
    /// <param name="password">Represents valid password for opening workbook.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <returns>Opened workbook.</returns>
    public IWorkbook Open( string fileName, ExcelParseOptions options, bool isReadOnly,
      string password, ExcelOpenType openType )
    {
        if (isReadOnly)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                openType = DetectFileFromStream(stream, openType);
            }
        }
        else
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                openType = DetectFileFromStream(stream, openType);
            }
        }
      switch( openType )
      {
        case ExcelOpenType.BIFF:
          return Open( fileName, options, isReadOnly, password, ExcelVersion.Excel97to2003 );

        case ExcelOpenType.SpreadsheetML:
          return OpenFromXml( fileName, ExcelXmlOpenType.MSExcel );

        case ExcelOpenType.CSV:
          return Open( fileName, Application.CSVSeparator );

        case ExcelOpenType.SpreadsheetML2007:
          return Open( fileName, options, isReadOnly, password, ExcelVersion.Excel2007 );

        case ExcelOpenType.SpreadsheetML2010:
          return Open( fileName, options, isReadOnly, password, ExcelVersion.Excel2010 );

        default:
          throw new ArgumentOutOfRangeException( "openType" );
      }
    }
#endif
    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <returns>Newly created workbook.</returns>
    public IWorkbook Open( Stream stream )
    {
      return Open( stream, ExcelOpenType.Automatic );
    }
    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="version">Version of the excel file format.</param>
    /// <returns>Newly created workbook.</returns>
    public IWorkbook Open( Stream stream, ExcelVersion version )
    {
      return Open( stream, version, ExcelParseOptions.Default );
    }
    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="version">Version of the excel file format.</param>
    /// <returns>Newly created workbook.</returns>
    public IWorkbook Open( Stream stream, ExcelVersion version, ExcelParseOptions options )
    {
      WorkbookImpl book = AppImplementation.CreateWorkbook( this, stream, version, options );

      // Store workbook into collection and return it.
      base.Add( book );
      return book;
    }
    /// <summary>
    /// Read workbook from the stream.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Newly created workbook.</returns>
    public IWorkbook Open( Stream stream, ExcelParseOptions options )
    {
      WorkbookImpl book = AppImplementation.CreateWorkbook( this, stream, options, Application.DefaultVersion );

      // Store workbook into collection and return it.
      base.Add( book );
      return book;
    }
#if !(WINRT || WP)
    /// <summary>
    /// Opens  a workbook.
    /// </summary>
    /// <param name="fileName">File name that contains workbook.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <returns>Opened workbook.</returns>
    public IWorkbook Open( string fileName, ExcelOpenType openType )
    {
      return Open( fileName, openType, ExcelParseOptions.Default );
    }
    /// <summary>
    /// Opens  a workbook.
    /// </summary>
    /// <param name="fileName">File name that contains workbook.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <returns>Opened workbook.</returns>
    public IWorkbook Open( string fileName, ExcelOpenType openType, ExcelParseOptions options )
    {
      return Open( fileName, openType, Application.DefaultVersion, options );
    }
    /// <summary>
    /// Opens  a workbook.
    /// </summary>
    /// <param name="fileName">File name that contains workbook.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <returns>Opened workbook.</returns>
    public IWorkbook Open( string fileName, ExcelOpenType openType, ExcelVersion version )
    {
      return Open( fileName, openType, version, ExcelParseOptions.Default );
    }
    /// <summary>
    /// Opens  a workbook.
    /// </summary>
    /// <param name="fileName">File name that contains workbook.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <returns>Opened workbook.</returns>
    public IWorkbook Open( string fileName, ExcelOpenType openType, ExcelVersion version, ExcelParseOptions options)
    {
      if( File.Exists( fileName ) )
      {
        using( FileStream stream = new FileStream( fileName, FileMode.Open, FileAccess.Read, FileShare.Read ) )
        {
#if !SILVERLIGHT && !WINRT && !WP
          string currentDir = Environment.CurrentDirectory;
          fileName = Path.GetFullPath( fileName );
          Environment.CurrentDirectory = Path.GetDirectoryName( fileName );
#endif

          WorkbookImpl book = ( WorkbookImpl )Open( stream, openType, fileName, version, options );
          book.FullFileName = Path.GetFullPath( fileName );

#if !SILVERLIGHT && !WINRT && !WP
          Environment.CurrentDirectory = currentDir;
#endif
          return book;
        }
      }

      throw new FileNotFoundException(
        string.Format( "File {0} could not be found. Please verify the file path.", fileName ) );
    }
#endif
    /// <summary>
    /// Opens  a workbook.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <returns>Opened workbook.</returns>
    public IWorkbook Open( Stream stream, ExcelOpenType openType, ExcelVersion version )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      openType = DetectFileFromStream( stream, openType );

      switch( openType )
      {
        case ExcelOpenType.BIFF:
          return Open( stream, ExcelVersion.Excel97to2003 );

        case ExcelOpenType.SpreadsheetML:
          return OpenFromXml( stream, ExcelXmlOpenType.MSExcel );

        case ExcelOpenType.CSV:
          return Open( stream, Application.CSVSeparator, version );

        case ExcelOpenType.SpreadsheetML2007:
          return Open( stream, ExcelVersion.Excel2007 );

        case ExcelOpenType.SpreadsheetML2010:
          return Open( stream, ExcelVersion.Excel2010 );

        default:
          throw new ArgumentOutOfRangeException( "openType" );
        //return DetectFileFromStream( stream );
      }
    }
    /// <summary>
    /// Opens  a workbook.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <returns>Opened workbook.</returns>
    public IWorkbook Open( Stream stream, ExcelOpenType openType )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      openType = DetectFileFromStream( stream, openType );

      switch( openType )
      {
        case ExcelOpenType.BIFF:
          return Open( stream, ExcelVersion.Excel97to2003 );

        case ExcelOpenType.SpreadsheetML:
          return OpenFromXml( stream, ExcelXmlOpenType.MSExcel );

        case ExcelOpenType.CSV:
          return Open( stream, Application.CSVSeparator );

        case ExcelOpenType.SpreadsheetML2007:
          return Open( stream, ExcelVersion.Excel2007 );

        case ExcelOpenType.SpreadsheetML2010:
          return Open( stream, ExcelVersion.Excel2010 );

        default:
          throw new ArgumentOutOfRangeException( "openType" );
        //return DetectFileFromStream( stream );
      }
    }
    /// <summary>
    /// Opens  a workbook.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <returns>Opened workbook.</returns>
    public IWorkbook Open( Stream stream, ExcelOpenType openType, ExcelParseOptions options )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      openType = DetectFileFromStream( stream, openType );

      switch( openType )
      {
        case ExcelOpenType.BIFF:
          return Open( stream, ExcelVersion.Excel97to2003, options );

        case ExcelOpenType.SpreadsheetML:
          return OpenFromXml( stream, ExcelXmlOpenType.MSExcel );

        case ExcelOpenType.CSV:
          return Open( stream, Application.CSVSeparator );

        case ExcelOpenType.SpreadsheetML2007:
          return Open( stream, ExcelVersion.Excel2007 );

        case ExcelOpenType.SpreadsheetML2010:
          return Open( stream, ExcelVersion.Excel2010 );

        default:
          throw new ArgumentOutOfRangeException( "openType" );
        //return DetectFileFromStream( stream );
      }
    }
    /// <summary>
    /// Opens  a workbook.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <returns>Opened workbook.</returns>
    private IWorkbook Open( Stream stream, ExcelOpenType openType, string fileName, ExcelVersion version )
    {
      return Open( stream, openType, fileName, version, ExcelParseOptions.Default );
    }
    /// <summary>
    /// Opens  a workbook.
    /// </summary>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="openType">Represents type of the file for open operation.</param>
    /// <returns>Opened workbook.</returns>
    private IWorkbook Open( Stream stream, ExcelOpenType openType, string fileName, ExcelVersion version, ExcelParseOptions options )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      openType = DetectFileFromStream( stream, openType );

      switch( openType )
      {
        case ExcelOpenType.BIFF:
          return Open( stream, ExcelVersion.Excel97to2003, options );

        case ExcelOpenType.SpreadsheetML:
          return OpenFromXml( stream, ExcelXmlOpenType.MSExcel );

        case ExcelOpenType.CSV:
          return Open( stream, Application.CSVSeparator, 1, 1, fileName, null, version );

        case ExcelOpenType.SpreadsheetML2007:
          return Open(stream, ExcelVersion.Excel2007, options);

        case ExcelOpenType.SpreadsheetML2010:
          return Open(stream, ExcelVersion.Excel2010, options);

        default:
          throw new ArgumentOutOfRangeException( "openType" );
          //return DetectFileFromStream( stream );
      }
    }
#if !(WINRT || WP)
    /// <summary>
    /// Read workbook from xml file.
    /// </summary>
    /// <param name="strPath">Path to xml file.</param>
    /// <param name="openType">Xml open type.</param>
    /// <returns>Returns opened workbook.</returns>
    public IWorkbook OpenFromXml( string strPath, ExcelXmlOpenType openType )
    {
      if( File.Exists( strPath ) )
      {
        using( FileStream stream = new FileStream( strPath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
        {
          return OpenFromXml( stream, openType );
        }
      }

      throw new FileNotFoundException( "File could not be found. Please verify the file path." );
    }
#endif
    /// <summary>
    /// Read workbook from xml file.
    /// </summary>
    /// <param name="stream">Path to xml file.</param>
    /// <param name="openType">Xml open type.</param>
    /// <returns>Returns opened workbook.</returns>
    public IWorkbook OpenFromXml( Stream stream, ExcelXmlOpenType openType )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      StreamReader sr = new StreamReader(stream);
      XmlReader reader = XmlReader.Create(sr);

      return OpenFromXml( reader, openType );
    }
    /// <summary>
    /// Read workbook from xml stream.
    /// </summary>
    /// <param name="reader">Xml reader.</param>
    /// <param name="openType">Xml open type.</param>
    /// <returns>Returns opened workbook.</returns>
    public IWorkbook OpenFromXml( XmlReader reader, ExcelXmlOpenType openType )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      IWorkbook book = OpenFromXmlInternal(reader, openType);
      if (book != null)
          return book;
      throw new ApplicationException( "Unable to read from XML." );
    }
    private IWorkbook OpenFromXmlInternal(XmlReader reader, ExcelXmlOpenType openType)
    {
#if !SILVERLIGHT && !WINRT && !WP
        XmlTextReader textReader = reader as XmlTextReader;

        if( textReader != null )
          textReader.WhitespaceHandling = WhitespaceHandling.Significant;
#endif

        WorkbookImpl book = new WorkbookImpl(Application, this, reader, openType);

        if (book != null)
        {
            base.Add(book);
            return book;
        }
        return book;
    }
#if !(WINRT || WP)
    /// <summary>
    /// Open new workbook in read-only mode.
    /// </summary>
    /// <param name="strFileName">File to open.</param>
    /// <returns>Newly created workbook.</returns>
    public IWorkbook OpenReadOnly( string strFileName )
    {
      return OpenReadOnly( strFileName, ExcelOpenType.Automatic, ExcelParseOptions.Default );
    }
    /// <summary>
    /// Open new CSV workbook in read-only mode.
    /// </summary>
    /// <param name="strFileName">File to open.</param>
    /// <param name="separator">Separator to use.</param>
    /// <returns>Newly created workbook.</returns>
    public IWorkbook OpenReadOnly( string strFileName, string separator )
    {
      using( FileStream stream = new FileStream( strFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) )
      {
        IWorkbook book= Open( stream, separator );
        (book as WorkbookImpl).ReadOnly= true;
        return book;
      }
    }
    /// <summary>
    /// Open new workbook in read-only mode.
    /// </summary>
    /// <param name="strFileName">File to open.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Newly created workbook.</returns>
    public IWorkbook OpenReadOnly( string strFileName, ExcelParseOptions options )
    {
      using( FileStream stream = new FileStream( strFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) )
      {
        IWorkbook book= Open( stream, options );
        (book as WorkbookImpl).ReadOnly = true;
        return book;
      }

      //      WorkbookImpl book = AppImplementation.CreateWorkbook( this, strFileName, options, true );
      //
      //      // Store workbook into collection and return it.
      //      base.Add( book );
      //      return book;
    }
    /// <summary>
    /// Open new workbook in read-only mode.
    /// </summary>
    /// <param name="strFileName">File to open.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Newly created workbook.</returns>
    public IWorkbook OpenReadOnly( string strFileName, ExcelOpenType openType, ExcelParseOptions options )
    {
      using( FileStream stream = new FileStream( strFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) )
      {
        IWorkbook book= Open( stream, openType, options );
        (book as WorkbookImpl).ReadOnly = true;
        return book;
      }

      //      WorkbookImpl book = AppImplementation.CreateWorkbook( this, strFileName, options, true );
      //
      //      // Store workbook into collection and return it.
      //      base.Add( book );
      //      return book;
    }
#endif
    /// <summary>
    /// Closes the object.
    /// </summary>
    public void Close()
    {
        //TODO:WINRT
#if !(WINRT || WP)
      IWorkbook book = Application.ActiveWorkbook;

      if( book != null )
      {
        int index = InnerList.IndexOf( book );

        if( index >= 0 && InnerList.Count > 0 )
        {
          // Close workbook and save its changes.
          book.Close( true, null );
        }
      }
#endif
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates workbook and sets its Application and Parent properties.
    /// </summary>
    /// <param name="application">Application object for the collection.</param>
    /// <param name="parent">Parent object for the collection.</param>
    public WorkbooksCollection( IApplication application, object parent )
      : base( application, parent )
    {
    }
    #endregion

    #region Clipboard Paste Operations
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Copies workbook from the clipboard.
    /// </summary>
    /// <returns>Pasted workbook.</returns>
    public IWorkbook PasteWorkbook()
    {
      ClipboardProvider provider = AppImplementation.CreateClipboardProvider();
      return provider.GetBookFromClipboard( this );

      //      IDataObject obj = Clipboard.GetDataObject();
      //
      //      if( obj != null )
      //      {
      //        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, string.Join( ", ", obj.GetFormats() ), "Clipboard Data Format" );
      //
      //        if( obj.GetDataPresent( "Biff8", true ) )
      //        {
      //          // try to get Biff8 format with convertion if needed
      //          object objData = obj.GetData( "Biff8", true );
      //          if( objData != null )
      //          {
      //            //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "type: " + objData.GetType().FullName, "Data storage" );
      //
      //            BiffReader reader = new BiffReader( (Stream)objData );
      //            reader.SeekOnBOFRecord();
      //
      //            return AppImplementation.CreateWorkbook( this, reader );
      //          }
      //        }
      //      }
      //
      //      return null;
    }
#endif
    #endregion

    #region Class helper methods
    /// <summary>
    /// Gets file type based on stream data.
    /// </summary>
    /// <param name="stream">Represents data stream.</param>
    /// <param name="currentOpenType">Represents type of the file for open operation.</param>
    /// <returns>Returns defined open type (cannot return automatic).</returns>
    private ExcelOpenType DetectFileFromStream( Stream stream, ExcelOpenType currentOpenType )
    {
      if( currentOpenType != ExcelOpenType.Automatic )
        return currentOpenType;

      if( stream == null )
        throw new ArgumentNullException( "stream" );

      long iPos = stream.Position;

      if( ZipArchive.ReadInt32( stream ) == Constants.HeaderSignature )
      {
        stream.Position = iPos;
        currentOpenType = ExcelOpenType.SpreadsheetML2007;
      }
      else
      {
        stream.Position = iPos;
        byte[] arr = new byte[ DEF_BUFFER_SIZE ];
        int iCount = stream.Read( arr, 0, DEF_BUFFER_SIZE );
        bool bIsBiff = true;

        if( iCount != 0 )
        {
          if( iCount >= DEF_BIFF_HEADER_SIZE )
          {
            for( int i = 0; i < DEF_BIFF_HEADER_SIZE; i++ )
            {
              if( DEF_XLS_FILE_HEADER[ i ] != arr[ i ] )
              {
                bIsBiff = false;
                break;
              }
            }
          }

          stream.Position = iPos;

          if( bIsBiff )
          {
            // Check whether it is Excel 97 workbook or encrypted Excel 2007
            using( ICompoundFile file = AppImplementation.CreateCompoundFile( stream ) )
            {
              currentOpenType = 
                ( Excel2007Decryptor.CheckEncrypted( file.RootStorage ) ) ?
                ExcelOpenType.SpreadsheetML2007 :
                ExcelOpenType.BIFF;
            }
          }
          else
          {
            using( MemoryStream memoryStream = new MemoryStream( arr, 0, iCount ) )
            {
              currentOpenType = DetectIsCSVOrXML( stream, memoryStream, iPos );
            }
          }
        }
      }

      if( currentOpenType == ExcelOpenType.Automatic )
        throw new ArgumentException( "Cannot recognize current file type." );

      stream.Position = iPos;
      return currentOpenType;
    }
    /// <summary>
    /// Detects is csv or xml file type.
    /// </summary>
    /// <param name="stream">Represents data stream.</param>
    /// <param name="memoryStream">MemoryStream with data from the file that will be used for file type detection.</param>
    /// <param name="lPosition">Represents start position in stream.</param>
    /// <returns>Parsed workbook if detected or null.</returns>
    private ExcelOpenType DetectIsCSVOrXML( Stream stream, MemoryStream memoryStream, long lPosition )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      if( memoryStream == null )
        throw new ArgumentNullException( "memoryStream" );

      string strSeparator = Application.CSVSeparator;
      bool bIsSeperatorContainSurrogate = IsContainSurrogate( strSeparator, "", false );
      bool bIsContainSurrogate = false;

      StreamReader reader = new StreamReader( memoryStream, true );
      string value = reader.ReadLine();
      Encoding curEncoding = reader.CurrentEncoding;
      ExcelOpenType result = ExcelOpenType.Automatic;

      while( value != null )
      {
        value = value.ToLower();
        int iIndex = value.IndexOf( DEF_XML_HEADER );

        if( iIndex != -1 )
        {
          stream.Position = lPosition + iIndex;
          result = ExcelOpenType.SpreadsheetML;
          break;
        }
        else if( value.IndexOf( DEF_HTML_HEADER ) != -1 )
        {
          break;
        }
        else if( !bIsContainSurrogate )
        {
          bIsContainSurrogate = IsContainSurrogate( value, strSeparator, bIsSeperatorContainSurrogate );
        }

        lPosition += curEncoding.GetByteCount( value );
        lPosition += curEncoding.GetByteCount( "\n" ) * 2;
        value = reader.ReadLine();
      }

      if( result == ExcelOpenType.Automatic && !bIsContainSurrogate )
        result = ExcelOpenType.CSV;

      return ( bIsContainSurrogate )
        ? ExcelOpenType.Automatic
        : result;
    }
    /// <summary>
    /// Indicates is surrogate value.
    /// </summary>
    /// <param name="strValue">Value for search.</param>
    /// <param name="strSeparator">Represents current separator.</param>
    /// <param name="bIsCompare">Indicates is compare with separator content, or not.</param>
    /// <returns></returns>
    private bool IsContainSurrogate( string strValue, string strSeparator, bool bIsCompare )
    {
      if( strValue == null )
        throw new ArgumentNullException( "strValue" );

      if( strSeparator == null )
        throw new ArgumentNullException( "strSeparator" );

      for( int i = 0, iLen = strValue.Length; i < iLen; i++ )
      {
        char ch = strValue[ i ];

        bool bIsSurrogate = !( char.IsLetterOrDigit( ch ) || char.IsPunctuation( ch )
          || char.IsSeparator( ch ) || char.IsSymbol( ch ) || char.IsWhiteSpace( ch ) );

        if( bIsSurrogate )
        {
          if( !bIsCompare || ( strSeparator.IndexOf( ch ) == -1 ) )
            return true;
        }
      }

      return false;
    }
    /// <summary>
    /// Version has been detect while adding the workbook
    /// </summary>
    /// <param name="strTemplateFile">Represent the file name</param>
    /// <returns>return the version</returns>
    private ExcelVersion DetectVersion(string strTemplateFile)
    {
#if !(WINRT || WP)
        if (strTemplateFile != null && File.Exists(strTemplateFile))
        {
            ExcelOpenType openType;
            ExcelVersion version = Application.DefaultVersion;
            using (FileStream stream = new FileStream(strTemplateFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                openType = DetectFileFromStream(stream, ExcelOpenType.Automatic);
                switch (openType)
                {
                    case ExcelOpenType.BIFF:
                        version = ExcelVersion.Excel97to2003;
                        break;
                    case ExcelOpenType.SpreadsheetML2007:
                        version = ExcelVersion.Excel2007;
                        break;
                    case ExcelOpenType.SpreadsheetML2010:
                        version = ExcelVersion.Excel2010;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("openType");
                }
            }
            return version;
        }
#endif
        return Application.DefaultVersion;
    }
    #endregion
  }
}

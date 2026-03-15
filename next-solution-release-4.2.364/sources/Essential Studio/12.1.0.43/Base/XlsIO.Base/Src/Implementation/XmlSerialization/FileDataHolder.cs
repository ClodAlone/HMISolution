#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
#if ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

using Syncfusion.Compression.Zip;
using Syncfusion.XlsIO.Implementation.XmlReaders;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using System.Text.RegularExpressions;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Charts;
using Syncfusion.CompoundFile.XlsIO;
using Syncfusion.XlsIO.Implementation.Security;
using Syncfusion.XlsIO.Implementation.PivotTables;
using Syncfusion.XlsIO.Implementation.XmlReaders.PivotTables;
using Syncfusion.XlsIO.Implementation.XmlSerialization.PivotTables;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Windows.Storage;
#endif

# if !SILVERLIGHT && !WINRT && !WP
using System.Drawing.Imaging;
using Syncfusion.XlsIO.Implementation.XmlReaders.Shapes;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#endif

namespace Syncfusion.XlsIO.Implementation.XmlSerialization
{
  /// <summary>
  /// Class used for holding file data.
  /// </summary>
    public class FileDataHolder : IWorkbookSerializator, IDisposable
  {
    #region Constants
    /// <summary>
    /// Name of the zip item with content types description.
    /// </summary>
    private const string ContentTypesItemName = "[Content_Types].xml";
    /// <summary>
    /// Name of the directory with relations.
    /// </summary>
    internal const string RelationsDirectory = "_rels";
    /// <summary>
    /// Extension for relations file.
    /// </summary>
    internal const string RelationExtension = ".rels";
    /// <summary>
    /// Path to the top relations.
    /// </summary>
    private const string TopRelationsPath = RelationsDirectory + "/.rels";
    /// <summary>
    /// Xml files extension.
    /// </summary>
    private const string XmlExtension = "xml";
    /// <summary>
    /// Relations default extension.
    /// </summary>
    private const string RelsExtension = "rels";
    /// <summary>
    /// Binary item extension.
    /// </summary>
    public const string BinaryExtension = "bin";
    /// <summary>
    /// Default name of the workbook part.
    /// </summary>
    private const string WorkbookPartName = "xl/workbook.xml";
    /// <summary>
    /// Default name of the CustomXml Parts
    /// </summary>
    private const string CustomXmlPartName = "customXml/item{0}.xml";
    /// <summary>
    /// Default name of the shared strings part.
    /// </summary>
    private const string SSTPartName = "/xl/sharedStrings.xml";
    /// <summary>
    /// Default name of the styles part name.
    /// </summary>
    private const string StylesPartName = "xl/styles.xml";
    /// <summary>
    /// Default name of the themes part name.
    /// </summary>
    private const string ThemesPartName = "xl/theme/theme1.xml";
    /// <summary>
    /// Path format for the worksheet part.
    /// </summary>
    private const string DefaultWorksheetPathFormat = "xl/worksheets/sheet{0}.xml";
    /// <summary>
    /// Path format for the chartsheet part.
    /// </summary>
    private const string DefaultChartsheetPathFormat = "xl/chartsheets/sheet{0}.xml";
    /// <summary>
    /// Path format for pictures.
    /// </summary>
    public const string DefaultPicturePathFormat = "xl/media/image{0}.";
    /// <summary>
    /// Default name of the extended properties part.
    /// </summary>
    public const string ExtendedPropertiesPartName = "docProps/app.xml";
    /// <summary>
    /// Default name of the core properties part.
    /// </summary>
    public const string CorePropertiesPartName = "docProps/core.xml";
    /// <summary>
    /// Default name of the custom properties part.
    /// </summary>
    public const string CustomPropertiesPartName = "docProps/custom.xml";
    /// <summary>
    /// Format for relation id generation.
    /// </summary>
    private const string RelationIdFormat = "rId{0}";
    /// <summary>
    /// Default name format for the external link items.
    /// </summary>
    public const string ExternLinksPathFormat = "xl/externalLinks/externalLink{0}.xml";
    /// <summary>
    /// Start of the External links item name.
    /// </summary>
    private const string ExtenalLinksPathStart = "xl/externalLinks/externalLink";
    /// <summary>
    /// Start of the worksheet custom property item.
    /// </summary>
    public const string CustomPropertyPathStart = "xl/customProperty";
    /// <summary>
    /// Default name format for cache definition.
    /// </summary>
    public const string PivotCacheDefinitionPathFormat = "xl/pivotCache/pivotCacheDefinition{0}.xml";
    /// <summary>
    /// Default name format for cache records.
    /// </summary>
    public const string PivotCacheRecordsPathFormat = "xl/pivotCache/pivotCacheRecords{0}.xml";
    /// <summary>
    /// Default name format for pivot table.
    /// </summary>
    public const string PivotTablePathFormat = "xl/pivotTables/pivotTable{0}.xml";
    /// <summary>
    /// Format to get full path to zip archive item that stores tables.
    /// </summary>
    private const string TablePathFormat = "xl/tables/table{0}.xml";
    /// <summary>
    /// External connection
    /// </summary>
    private const string ConnectionPathFormat = "xl/connections.xml";
    private const string QueryTablePathFormat = "/xl/queryTables/queryTable{0}.xml";
    #endregion

    #region Members
        /// <summary>
        /// Dictionary which contains the stream of the emf image file and wmf image file
        /// </summary>
    Dictionary<string, MemoryStream > m_metafileStream = new Dictionary<string, MemoryStream >();
    /// <summary>
    /// Represents Zip Archive.
    /// </summary>
    private ZipArchive m_archive = new ZipArchive();
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Parser to parse data if necessary.
    /// </summary>
    private Excel2007Parser m_parser;
    /// <summary>
    /// Dictionary which is used to identify content type and stores default types.
    /// Key - file extension (string), Value - content type (string).
    /// </summary>
    private IDictionary<string, string> m_dicDefaultTypes = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
    /// <summary>
    /// Dictionary which is used to identify content type and stores type overrides.
    /// Key - part name, Value - content type (string).
    /// </summary>
    private IDictionary<string, string> m_dicOverriddenTypes = new Dictionary<string, string>( 
#if ( WINRT )  
        StringComparer.OrdinalIgnoreCase
#else
        System.StringComparer.InvariantCultureIgnoreCase 
#endif
        );
    /// <summary>
    /// Top-level relations.
    /// </summary>
    private RelationCollection m_topRelations;
    /// <summary>
    /// Name of the workbook part.
    /// </summary>
    private string m_strWorkbookPartName = WorkbookPartName;
    /// <summary>
    /// Name of the shared strings table part.
    /// </summary>
    private string m_strSSTPartName = SSTPartName;
    /// <summary>
    /// Name of the styles part.
    /// </summary>
    private string m_strStylesPartName = StylesPartName;
    /// <summary>
    /// Connection part.
    /// </summary>
    private string m_connectionPartName = ConnectionPathFormat;
    private string m_queryTablePartName = QueryTablePathFormat;
    /// <summary>
    /// Name of the themes part.
    /// </summary>
    private string m_strThemesPartName = ThemesPartName;
    /// <summary>
    /// Object used for workbook serialization.
    /// </summary>
    private Excel2007Serializator m_serializator;
    /// <summary>
    /// Represents list of workbook styles.
    /// </summary>
    private List<int> m_arrCellFormats;
    /// <summary>
    /// Workbook-level relations.
    /// </summary>
    private RelationCollection m_workbookRelations;
    /// <summary>
    /// Specifies style relation id.
    /// </summary>
    private string m_strStylesRelationId;
    /// <summary>
    /// Specifies shares string relation id.
    /// </summary>
    private string m_strSSTRelationId;
    /// <summary>
    /// Specifies theme relation id.
    /// </summary>
    private string m_strThemeRelationId;
    /// <summary>
    /// Specifies workbook content type.
    /// </summary>
    private string m_strWorkbookContentType = ContentTypes.Workbook;
    /// <summary>
    /// Memory stream that will get workbook part after /sheets tag or after /definedNames tag.
    /// </summary>
    private Stream m_streamEnd = new MemoryStream();
    /// <summary>
    /// Memory stream that will get workbook part before sheets tag.
    /// </summary>
    private Stream m_streamStart = new MemoryStream();
    /// <summary>
    /// Stream that can contains Dxfs formatting tags.
    /// </summary>
    private Stream m_streamDxfs;
    /// <summary>
    /// Index used to generate comments zip item names.
    /// </summary>
    private int m_iCommentIndex;
    /// <summary>
    /// Index used to generate vml zip item names.
    /// </summary>
    private int m_iVmlIndex;
    /// <summary>
    /// Index used to generate drawing zip item names.
    /// </summary>
    private int m_iDrawingIndex;
    /// <summary>
    /// Index used to generate image zip item names.
    /// </summary>
    private int m_iImageIndex;
    /// <summary>
    /// Represents image id.
    /// </summary>
    private int m_iImageId;
    /// <summary>
    /// Index used to generate chart zip item names.
    /// </summary>
    private int m_iLastChartIndex;
    /// <summary>
    /// Index used to generate the pivotCache item name
    /// </summary>
    private int m_iLastPivotCacheIndex;

    /// <summary>
    ///  Index used to generate the pivotCacheRecords item name
    /// </summary>
    private int m_iLastPivotCacheRecordsIndex;
    /// <summary>
    /// Index used to generate extern link zip item name.
    /// </summary>
    private int m_iExternLinkIndex;
    /// <summary>
    /// Array that contains names of the image items.
    /// </summary>
    private string[] m_arrImageItemNames;
    /// <summary>
    /// Parsed dfx style list.
    /// </summary>
    private List<DxfImpl> m_lstParsedDxfs;
    /// <summary>
    /// Workbook views collection.
    /// </summary>
    private List<Dictionary<string, string>> m_lstBookViews;
    /// <summary>
    /// Items that must be removed after parsing complete.
    /// </summary>
    private Dictionary<string, object> m_dictItemsToRemove = new Dictionary<string,object>();
    /// <summary>
    /// Stream containing functionGroups tag.
    /// </summary>
    private Stream m_functionGroups;
    /// <summary>
    /// Build version, last edited version, etc..
    /// </summary>
    private FileVersion m_fileVersion = new FileVersion();
    /// <summary>
    /// Calculation id.
    /// </summary>
    private string m_strCalculationId = "125725";
    private Dictionary<string, string> m_preservedCaches = new Dictionary<string, string>();
    private Stream m_extensions;
    private string m_strConnectionId;
    private int m_queryTableCount = 1;
    #endregion

    #region Constructors
    /// <summary>
    /// Prevents a default instance of the FileDataHolder class from being created.
    /// </summary>
	private FileDataHolder()
	{
	}
    /// <summary>
    /// Initializes a new instance of the FileDataHolder class.
    /// </summary>
    /// <param name="book">Parent workbook for the new instance.</param>
    public FileDataHolder( WorkbookImpl book )
    {
      if( book == null )
        throw new ArgumentNullException( "book" );

      m_book = book;
#if !(WINRT )
      m_archive.CreateCompressor = book.AppImplementation.CreateCompressor;
#endif
    }
#if !(WINRT )
    /// <summary>
    /// Initializes a new instance of the FileDataHolder class.
    /// </summary>
    /// <param name="book">Parent workbook for the new instance.</param>
    /// <param name="filename">File name to get initial data from.</param>
    /// <param name="password">Password to use during for decryption.</param>
    public FileDataHolder( WorkbookImpl book, string filename, string password )
      : this( book )
    {
      if( filename == null || filename.Length == 0 )
        throw new ArgumentOutOfRangeException( "filename" );

      m_archive.Open( filename );
    }
#endif
    /// <summary>
    /// Initializes a new instance of the FileDataHolder class.
    /// </summary>
    /// <param name="book">Parent workbook for the new instance.</param>
    /// <param name="stream">Stream to get initial data from.</param>
    /// <param name="password">Password to use during for decryption.</param>
    public FileDataHolder( WorkbookImpl book, Stream stream, string password )
      : this( book )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      // 1. Check whether we have zip or compound header in the file.
      long lStartPosition = stream.Position;
      bool bEncrypted = false;

      if( CompoundFile.XlsIO.Net.CompoundFile.CheckHeader( stream ) )
      {
        // 1. Version is correct but it is encrypted and decryption required.
        bEncrypted = true;

        using( ICompoundFile file = book.AppImplementation.CreateCompoundFile( stream ) )
        {
            Excel2007Decryptor decryptor;
          ICompoundStorage storage = file.RootStorage;
          if( ( bEncrypted = Excel2007Decryptor.CheckEncrypted( storage ) ) )
          {
            decryptor = ( CheckVersion(storage)==ExcelVersion.Excel2007)? new Excel2007Decryptor(): new Excel2010Decryptor();
            decryptor.Initialize( file.RootStorage );

            ApplicationImpl excel = book.AppImplementation;
            bool bStandardPassword = false;

            if( password == null )
            {
              bStandardPassword = decryptor.CheckPassword( WorkbookImpl.StandardPassword );
            }

            if( !bStandardPassword )
            {
              RequestPassword( ref password, excel );

              while( !decryptor.CheckPassword( password ) )
              {
                RerequestPassword( ref password, excel );
              }
            }

            stream = decryptor.Decrypt();

#if SAVE_DECRYPTED
            if( stream is MemoryStream )
            {
              using( FileStream fileStream = new FileStream( "d:\\decompressed.xlsx", FileMode.Create, FileAccess.Write, FileShare.None ) )
              {
                MemoryStream memStream = stream as MemoryStream;
                memStream.WriteTo( fileStream );
                memStream.Position = 0;
              }
            }

#endif
            m_book.m_encryptionType = ExcelEncryptionType.Standard;
          }
        }

        // 2. Wrong version - we have Excel 97 file
        if( !bEncrypted )
          throw new ApplicationException( "Wrong excel version" );
      }

      if( bEncrypted )
        m_book.PasswordToOpen = password;

      m_archive.Open( stream, false );
    }
    private ExcelVersion CheckVersion(ICompoundStorage storage)
    {
        Stream stream = storage.OpenStream(SecurityHelper.EncryptionInfoStream);
        byte[] arrBuffer = new byte[ExcelConstants.IntSize];
        int version= SecurityHelper.ReadInt32(stream, arrBuffer);
        stream.Close();
        if (version == SecurityHelper.Excel2010Version)
            return ExcelVersion.Excel2010;
        else
            return ExcelVersion.Excel2007;

    }
    /// <summary>
    /// Requests password if necessary.
    /// </summary>
    /// <param name="password">Current password provided to the parsing method.</param>
    /// <param name="excel">Application object used for password request.</param>
    private void RerequestPassword( ref string password, ApplicationImpl excel )
    {
      PasswordRequiredEventArgs eventArgs = new PasswordRequiredEventArgs();

      if( excel.RaiseOnWrongPassword( this, eventArgs ) )
      {
        password = eventArgs.NewPassword;
      }
      else
      {
        eventArgs = null;
      }

      if( password == null || eventArgs == null || eventArgs.StopParsing )
        throw new ArgumentException( "Workbook is protected and password wasn't specified." );
    }
    /// <summary>
    /// Re-requests password (called when provided password is incorrect).
    /// </summary>
    /// <param name="password">Current password.</param>
    /// <param name="excel">Application object used for password request.</param>
    private void RequestPassword( ref string password, ApplicationImpl excel )
    {
      PasswordRequiredEventArgs eventArgs = new PasswordRequiredEventArgs();

      if( password == null )
      {
        if( excel.RaiseOnPasswordRequired( this, eventArgs ) )
        {
          password = eventArgs.NewPassword;
        }
        else
        {
          eventArgs = null;
        }
      }

      if( password == null || eventArgs == null || eventArgs.StopParsing )
        throw new ArgumentException( "Workbook is protected and password wasn't specified." );
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets parent workbook. Read-only.
    /// </summary>
    public WorkbookImpl Workbook
    {
      get
      {
        return m_book;
      }
    }
    /// <summary>
    /// Gets Excel 2007 parser.
    /// </summary>
    public Excel2007Parser Parser
    {
      get
      {
        if( m_parser == null )
          m_parser = new Excel2007Parser( m_book );

        return m_parser;
      }
    }
    /// <summary>
    /// Returns archive item corresponding to the relation.
    /// </summary>
    public ZipArchiveItem this[ Relation relation, string parentPath ]
    {
      get
      {
        return ( relation != null ) ?
          m_archive[ CombinePath( parentPath, relation.Target ) ] :
          null;
      }
    }
    /// <summary>
    /// Gets object used for serialization.
    /// </summary>
    public Excel2007Serializator Serializator
    {
      get
      {
        if( m_serializator == null || m_serializator.Version != m_book.Version )
        {
          switch( m_book.Version )
          {
            case ExcelVersion.Excel2007:
              m_serializator = new Excel2007Serializator( m_book );
              break;

            case ExcelVersion.Excel2010:           
              m_serializator = new Excel2010Serializator( m_book );
              break;
            case ExcelVersion.Excel2013:
              m_serializator = new Excel2013Serializator(m_book);              
              break;
            default:
              throw new NotImplementedException();
          }
        }


        return m_serializator;
      }
    }
    /// <summary>
    /// Gets cell styles.
    /// </summary>
    public List<int> XFIndexes
    {
      get
      {
        return m_arrCellFormats;
      }
    }
    /// <summary>
    /// Gets zip archive object that stores Excel 2007 document.
    /// </summary>
    public ZipArchive Archive
    {
      get
      {
        return m_archive;
      }
    }
    /// <summary>
    /// Gets or sets last used index of the comment item.
    /// </summary>
    public int LastCommentIndex
    {
      get
      {
        return m_iCommentIndex;
      }
      set
      {
        m_iCommentIndex = value;
      }
    }
    /// <summary>
    /// Gets or sets last used index of the vml item.
    /// </summary>
    public int LastVmlIndex
    {
      get
      {
        return m_iVmlIndex;
      }
      set
      {
        m_iVmlIndex = value;
      }
    }
    /// <summary>
    /// Gets or sets last used index of the drawing item.
    /// </summary>
    public int LastDrawingIndex
    {
      get
      {
        return m_iDrawingIndex;
      }
      set
      {
        m_iDrawingIndex = value;
      }
    }
    /// <summary>
    /// Gets or sets last used index of the image item.
    /// </summary>
    public int LastImageIndex
    {
      get
      {
        return m_iImageIndex;
      }
      set
      {
        m_iImageIndex = value;
      }
    }
    /// <summary>
    /// Gets or sets last used image id.
    /// </summary>
    public int LastImageId
    {
      get
      {
        return m_iImageId;
      }
      set
      {
        m_iImageId = value;
      }
    }
    /// <summary>
    /// Gets or sets last used index of the chart item.
    /// </summary>
    public int LastChartIndex
    {
      get
      {
        return m_iLastChartIndex;
      }
      set
      {
        m_iLastChartIndex = value;
      }
    }
    /// <summary>
    ///  gets or set the pivotCache index
    /// </summary>
    internal int LastPivotCacheIndex
    {
        get
        {
            return m_iLastPivotCacheIndex;
        }
        set
        {
            m_iLastPivotCacheIndex = value;
        }
    }
    /// <summary>
    /// gets or set the pivotCache index
    /// </summary>
    internal int LastPivotCacheRecordsIndex
    {
        get
        {
            return m_iLastPivotCacheRecordsIndex;
        }
        set
        {
            m_iLastPivotCacheRecordsIndex = value;
        }
    }
    /// <summary>
    /// Gets dictionary with default content types. Read-only.
    /// </summary>
    public IDictionary< string, string > DefaultContentTypes
    {
      get
      {
        return m_dicDefaultTypes;
      }
    }
    /// <summary>
    /// Gets dictionary with overridden content types. Read-only.
    /// </summary>
    public IDictionary<string, string> OverriddenContentTypes
    {
      get
      {
        return m_dicOverriddenTypes;
      }
    }
    /// <summary>
    /// Gets number of parsed dfx styles or int min value if nothing was parsed.
    /// </summary>
    public int ParsedDxfsCount
    {
      get
      {
        return ( m_lstParsedDxfs != null ) ? m_lstParsedDxfs.Count : int.MinValue;
      }
    }
    /// <summary>
    /// Gets Items that must be removed after parsing complete. Read-only.
    /// </summary>
    public Dictionary<string, object> ItemsToRemove
    {
      get
      {
        return m_dictItemsToRemove;
      }
    }
    public string CalculationId
    {
      get
      {
        return m_strCalculationId;
      }
      set
      {
        m_strCalculationId = value;
      }
    }
    public FileVersion FileVersion
    {
      get
      {
        return m_fileVersion;
      }
    }
    public Dictionary<string, string> PreservedCaches
    {
      get
      {
        return m_preservedCaches;
      }
    }
    public Stream ExtensionStream
    {
      get
      {
        return m_extensions;
      }
      set
      {
        m_extensions = value;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Adds overriden content type.
    /// </summary>
    /// <param name="fileName">File name to override content type for.</param>
    /// <param name="contentType">Content type to set.</param>
    public void AddOverriddenContentType( string fileName, string contentType )
    {
      if( fileName == null || fileName.Length == 0 )
        throw new ArgumentOutOfRangeException( "fileName" );

      if( fileName[0 ] != '/' )
        fileName = '/' + fileName;

      OverriddenContentTypes[ fileName ] = contentType;
    }
    /// <summary>
    /// Parses Dxf styles collection.
    /// </summary>
    /// <returns>Dxf style collection.</returns>
    public List<DxfImpl> ParseDxfsCollection()
    {
      List<DxfImpl> lstResult = null;

      if( m_streamDxfs == null )
        return m_lstParsedDxfs;

      if (m_streamDxfs.Length == 0)
          return m_lstParsedDxfs;

      if( m_lstParsedDxfs == null )
      {
        m_streamDxfs.Position = 0;
        XmlReader reader = UtilityMethods.CreateReader( m_streamDxfs );
		if (reader.LocalName != Excel2007Serializator.DiffXFsTagName)
          reader.Read();
        lstResult = Parser.ParseDxfCollection( reader );
        m_streamDxfs.Flush();
        m_streamDxfs.Dispose();
        m_streamDxfs=null;
        m_lstParsedDxfs = lstResult;
      }
      else
      {
        lstResult = m_lstParsedDxfs;
      }
      
      return lstResult;
    }
    /// <summary>
    /// Extracts sheet data.
    /// </summary>
    /// <param name="sheetPath">Represents sheet path.</param>
    /// <returns>Extracted sheet data</returns>
    public WorksheetDataHolder GetSheetData( string sheetPath )
    {
      if( sheetPath == null || sheetPath.Length == 0 )
        throw new ArgumentOutOfRangeException( "sheetPath" );

      throw new NotImplementedException();
    }
    /// <summary>
    /// Extracts worksheet.
    /// </summary>
    /// <param name="sheetName">Represents sheet name.</param>
    /// <returns>Sheet extracted.</returns>
    public WorksheetBaseImpl GetSheet( string sheetName )
    {
      if( sheetName == null || sheetName.Length == 0 )
        throw new ArgumentOutOfRangeException( "sheetName" );

      return m_book.Objects[ sheetName ] as WorksheetBaseImpl;
    }
    /// <summary>
    /// Parses document.
    /// </summary>
    /// <param name="themeColors">Represents theme colors in document.</param>
    public void ParseDocument( ref List<Color> themeColors, bool parseOnDemand )
    {
#if MEASURE_PERFORMANCE
      DateTime parseDocumentStart = DateTime.Now;
#endif
      m_book.Loading = true;
      bool bOldThrow = m_book.ThrowOnUnknownNames;
      m_book.ThrowOnUnknownNames = false;
      ParseContentType();
      m_topRelations = ParseRelations( TopRelationsPath );
      m_strWorkbookPartName = FindItemByContent( ContentTypes.Workbook );
      m_dictItemsToRemove.Add( TopRelationsPath, null );

      if( m_strWorkbookPartName == null &&
        ( FindWorkbookPartName( ContentTypes.MacroWorkbook )
        || FindWorkbookPartName( ContentTypes.MacroTemplate ) ) )
      {
        m_book.HasMacros = true;
      }

      if( m_strWorkbookPartName == null )
        FindWorkbookPartName( ContentTypes.Template );

      if( m_strWorkbookPartName == null )
        throw new NotSupportedException( "File cannot be opened - format is not supported." );

      if( m_strWorkbookPartName[ 0 ] == '/' )
        m_strWorkbookPartName = UtilityMethods.RemoveFirstCharUnsafe( m_strWorkbookPartName );

      ParseDocumentProperties();
      ParseWorkbook( ref themeColors, parseOnDemand );
      ParseMetaProperties();  
      ParseCustomXmlParts();
      foreach( string itemName in m_dictItemsToRemove.Keys )
      {
        m_archive.RemoveItem( itemName );
      }

      m_dictItemsToRemove.Clear();

      // TODO: remove this string when we will support active worksheets.
      //m_book.TabSheets[ 0 ].Activate();
      m_book.ThrowOnUnknownNames = bOldThrow;
      m_book.Loading = false;

#if MEASURE_PERFORMANCE
      DateTime parseDocumentEnd = DateTime.Now;
      Console.WriteLine( "Document parsing took: {0}", parseDocumentEnd - parseDocumentStart );
#endif
    }
    /// <summary>
    /// Finds workbook part name according to content type.
    /// </summary>
    /// <param name="strContentType">Content type.</param>
    /// <returns>True if item was found; false otherwise.</returns>
    private bool FindWorkbookPartName( string strContentType )
    {
      m_strWorkbookContentType = strContentType;
      m_strWorkbookPartName = FindItemByContent( strContentType );
      return m_strWorkbookPartName != null;
    }
    /// <summary>
    /// Gets the Workbook Save Type.
    /// </summary>
    /// <returns>Workbook Content Type.</returns>
    internal ExcelSaveType GetWorkbookPartType()
    {
        ExcelSaveType saveType = ExcelSaveType.SaveAsXLS;
        if (FindWorkbookPartName(ContentTypes.MacroTemplate)
         || FindWorkbookPartName(ContentTypes.Template))
            saveType = ExcelSaveType.SaveAsTemplate;

        else if(FindWorkbookPartName(ContentTypes.Workbook)
        || FindWorkbookPartName(ContentTypes.MacroWorkbook))
            saveType=ExcelSaveType.SaveAsXLS;

        if (m_strWorkbookPartName[0] == '/')
            m_strWorkbookPartName = UtilityMethods.RemoveFirstCharUnsafe(m_strWorkbookPartName);
        return saveType;
    }
    /// <summary>
    /// Parses content type.
    /// </summary>
    public void ParseContentType()
    {
      m_dicDefaultTypes.Clear();
      m_dicOverriddenTypes.Clear();
      ZipArchiveItem item = m_archive[ ContentTypesItemName ];

      if( item == null )
        throw new NotSupportedException( "File cannot be opened - format is not supported" );

      XmlReader reader = UtilityMethods.CreateReader( item.DataStream );
      Parser.ParseContentTypes( reader, m_dicDefaultTypes, m_dicOverriddenTypes );

      string defaultKey = "/xl/workbook.xml";
      string defaultValue = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml";
      if (!m_dicOverriddenTypes.ContainsKey(defaultKey))
          m_dicOverriddenTypes.Add(defaultKey, defaultValue);

      m_dictItemsToRemove.Add( ContentTypesItemName, null );
    }
    /// <summary>
    /// Parses all document properties.
    /// </summary>
    public void ParseDocumentProperties()
    {
      ParseArchiveItemByContentType( ContentTypes.CoreProperties );
      ParseArchiveItemByContentType( ContentTypes.ExtendedProperties );
      ParseArchiveItemByContentType( ContentTypes.CustomProperties );
    }
    /// <summary>
    /// Parses archive item by content type. Removes relation for this item and the item itself.
    /// </summary>
    /// <param name="strContentType">Content type.</param>
    public void ParseArchiveItemByContentType( string strContentType )
    {
      string strItemName;
      XmlReader reader = GetXmlReaderByContentType( strContentType, out strItemName );

      if( reader == null )
        return;

      switch( strContentType )
      {
        case ContentTypes.CoreProperties:
          Parser.ParseDocumentCoreProperties( reader );
          m_topRelations.RemoveByContentType( RelationTypes.CoreProperties );
          break;

        case ContentTypes.ExtendedProperties:
          Parser.ParseExtendedProperties( reader );
          m_topRelations.RemoveByContentType( RelationTypes.ExtendedProperties );
          break;

        case ContentTypes.CustomProperties:
          Parser.ParseCustomProperties( reader );
          m_topRelations.RemoveByContentType( RelationTypes.CustomProperties );
          break;

        default:
          throw new ArgumentException( "strContentType" );
      }

      m_archive.RemoveItem( strItemName );
    }
    /// <summary>
    /// Returns XmlReader for corresponding content type.
    /// </summary>
    /// <param name="strContentType">Content type.</param>
    /// <param name="strItemName">Name of the item that has specified content type.</param>
    /// <returns>Item name.</returns>
    public XmlReader GetXmlReaderByContentType( string strContentType, out string strItemName )
    {
      string strPath = FindItemByContent( strContentType );

      if( strPath == null )
      {
        strItemName = string.Empty;
        return null;
      }

      m_dicOverriddenTypes.Remove( strPath );

      if( strPath.StartsWith( "/" ) )
        strPath = UtilityMethods.RemoveFirstCharUnsafe( strPath );

      ZipArchiveItem item = m_archive[ strPath ];
      strItemName = item.ItemName;
      Stream stream = item.DataStream;
      stream.Position = 0;
      XmlReader reader = UtilityMethods.CreateReader( stream );
      return reader;
    }
#if !(WINRT )
    /// <summary>
    /// Saves document into specified file.
    /// </summary>
    /// <param name="filename">Name of the file to save into.</param>
    /// <param name="saveType">Type of the saving format.</param>
    public void SaveDocument( string filename, ExcelSaveType saveType )
    {
      using( FileStream stream = new FileStream( filename, FileMode.Create, FileAccess.ReadWrite, FileShare.None ) )
      {
        SaveDocument( stream, saveType );
      }
    }
#endif
    /// <summary>
    /// Saves document into specified stream.
    /// </summary>
    /// <param name="stream">Stream to save into.</param>
    /// <param name="saveType">Type of the saving format.</param>
    public void SaveDocument( Stream stream, ExcelSaveType saveType )
    {
      SaveDocument( saveType );

      if( m_book.PasswordToOpen != null && m_book.m_encryptionType != ExcelEncryptionType.None )
      {
        MemoryStream streamTemp = new MemoryStream();
        m_archive.Save( streamTemp, false );

        using( ICompoundFile file = m_book.AppImplementation.CreateCompoundFile() )
        {
            Excel2007Encryptor encryptor = (m_book.Version == ExcelVersion.Excel2007) ? new Excel2007Encryptor() : new Excel2010Encryptor();
          streamTemp.Position = 0;
          string password = m_book.PasswordToOpen;

          if( password == null )
            password = WorkbookImpl.StandardPassword;

          encryptor.Encrypt( streamTemp, password, file.RootStorage );
          file.Save( stream );
        }
        //throw new NotImplementedException();
      }
      else
      {
        m_archive.Save( stream, false );
      }
    }
    /// <summary>
    /// Saves document inside internal zip archive.
    /// </summary>
    /// <param name="saveType">Type of the saving format.</param>
    public void SaveDocument( ExcelSaveType saveType )
    {
#if MEASURE_PERFORMANCE
      DateTime saveDocumentStart = DateTime.Now;
#endif

      m_iVmlIndex = 0;
      m_iCommentIndex = 0;
      m_iDrawingIndex = 0;
      m_iImageIndex = 0;
      m_iImageId = 0;
      m_iExternLinkIndex = 0;
      m_iLastChartIndex = 0;
      m_iLastPivotCacheIndex = 0;
      m_iLastPivotCacheRecordsIndex = 0;
      m_book.LastPivotTableIndex = 0;
      SaveWorkbook( saveType );
      SaveDocumentProperties();
      SaveContentTypes();
      SaveTopLevelRelations();
      SaveContentTypeProperties();
      //SaveWorksheets();

#if MEASURE_PERFORMANCE
      DateTime saveDocumentEnd = DateTime.Now;
      Console.WriteLine( "SaveDocument() took: {0}", saveDocumentEnd - saveDocumentStart );
#endif
    }
    /// <summary>
    /// Registers correct content type into collections of default content types.
    /// </summary>
    /// <param name="imageFormat">Image format of the picture to register content type for.</param>
    /// <returns>Proposed picture file extension.</returns>
    public string RegisterContentTypes( ImageFormat imageFormat )
    {
      if( imageFormat == null )
        throw new ArgumentNullException( "imageFormat" );

      // TODO: maybe later we would have to add some property to the picture class to return type.
      string strExtension;
      string strContentType = GetPictureContentType( imageFormat, out strExtension );

      m_dicDefaultTypes[ strExtension ] = strContentType;
      return strExtension;
    }
    /// <summary>
    /// Converts image format into content type and file extension.
    /// </summary>
    /// <param name="format">Image format to convert.</param>
    /// <param name="strExtension">Resulting file extension.</param>
    /// <returns>Content type for this image format.</returns>
    public static string GetPictureContentType( ImageFormat format, out string strExtension )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      string strResult;
      if( format.Equals( ImageFormat.Bmp ) )
      {
        strResult = ContentTypes.Bitmap;
        strExtension = "bmp";
      }
      else if( format.Equals( ImageFormat.Jpeg ) )
      {
        strResult = ContentTypes.Jpeg;
        strExtension = "jpeg";
      }
      else if( format.Equals( ImageFormat.Png ) )
      {
        strResult = ContentTypes.Png;
        strExtension = "png";
      }
      else if( format.Equals( ImageFormat.Emf ) )
      {
        strResult = ContentTypes.Emf;
        strExtension = "emf";
      }
      else if( format.Equals( ImageFormat.Gif ) )
      {
        strResult = ContentTypes.Gif;
        strExtension = "gif";
      }
      //      else if( format.Equals( ImageFormat.Wmf ) )
      //      {
      //        return MsoBlipType.msoblipWMF;
      //      }
      else
      {
        strResult = ContentTypes.Png;
        strExtension = "png";
      }

      return strResult;
    }
    /// <summary>
    /// Saves image into appropriate location inside document.
    /// </summary>
    /// <param name="image">Image to save.</param>
    /// <param name="proposedPath">Proposed item name, null - autogenerate.</param>
    /// <returns>Generated item name.</returns>
    public string SaveImage( Image image, string proposedPath )
    {

      return SaveImage( image, image.RawFormat, proposedPath );
    }
    /// <summary>
    /// Saves image into appropriate location inside document.
    /// </summary>
    /// <param name="image">Image to save.</param>
    /// <param name="imageFormat">Destination image format.</param>
    /// <param name="proposedPath">Proposed item name, null - autogenerate.</param>
    /// <returns>Generated item name.</returns>
    public string SaveImage( Image image, ImageFormat imageFormat, string proposedPath )
    {
      if( image == null )
        throw new ArgumentNullException( "image" );

      // 1. Determine image type (png, bmp, etc.), generate file name and fill relations.
      string strExtension = GetExtension( imageFormat );
      RegisterContentTypes( imageFormat );
      string strItemName = null;
      Regex regex;

      if( proposedPath == null )
      {
        do
        {
          m_iImageIndex++;
          strItemName = string.Format( DefaultPicturePathFormat, m_iImageIndex );
          regex = new Regex( strItemName );
        }
        while( m_archive.Find( regex ) != -1 );

        strItemName += strExtension;
      }
      else if (m_archive.Find(new Regex(proposedPath.Split(new char[] { '.' })[0])) != -1)
      {
          m_iImageIndex++;
          strItemName = string.Format(DefaultPicturePathFormat, m_iImageIndex);         
          strItemName += strExtension;
      }
      else
      {
        m_iImageIndex++;
        strItemName = proposedPath;
      }
        
      imageFormat = GetImageFormat(strExtension);
      // 2. Save picture into necessary file
      MemoryStream stream;
      ImageFormat format = image.RawFormat;

      if( format.Equals( ImageFormat.Emf ) || format.Equals( ImageFormat.Wmf ) )
      {
          MemoryStream currentStream = new MemoryStream();
        stream = MsoMetafilePicture.SerializeMetafile( image );
        if (m_metafileStream.ContainsKey(strItemName))
        {
            currentStream = m_metafileStream[strItemName];
            if (stream.Length != currentStream.Length)
                stream = currentStream;
        }

      }
      else
      {
        stream = new MemoryStream();
        image.Save( stream, imageFormat );
      }

      m_archive.UpdateItem( strItemName, stream, true, FileAttributes.Archive );
      return strItemName;
    }
    /// <summary>
    ///Get Extension of the image and return Image format of the image
    /// </summary>
    /// <param name="extension">Extension of the image</param>
    /// <returns>Image format of the image</returns>
    private ImageFormat GetImageFormat(string extension)
    {
        ImageFormat returnImageFormat;
        if (extension == null)
            throw new ArgumentNullException("format");
        switch (extension)
        {
            case "bmp":
                returnImageFormat = ImageFormat.Bmp;
                break;
            case "jpeg":
                returnImageFormat = ImageFormat.Jpeg;
                break;
#if !SILVERLIGHT && !WINRT && !WP
            case "tiff":
                returnImageFormat = ImageFormat.Tiff;
                break;
            case "exif":
                returnImageFormat = ImageFormat.Exif;
                break;
#endif
            case "png":
                returnImageFormat = ImageFormat.Png;
                break;
            case "emf":
                returnImageFormat = ImageFormat.Emf;
                break;
            case "icon":
                returnImageFormat = ImageFormat.Jpeg;
                break;
            case "wmf":
                returnImageFormat = ImageFormat.Wmf;
                break;
            case "gif":
                returnImageFormat = ImageFormat.Gif;
                break;
            default:
                returnImageFormat = ImageFormat.Png;
                break;
        }
        return returnImageFormat;
    }

    /// <summary>
    /// Gets image item name.
    /// </summary>
    /// <param name="i">Image index to get name for.</param>
    /// <returns>Name of Image item.</returns>
    public string GetImageItemName( int i )
    {
      return m_arrImageItemNames[ i ];
    }
    /// <summary>
    /// Prepares a new archive item.
    /// </summary>
    /// <param name="itemNameStart">Start of the item's name.</param>
    /// <param name="extension">Represents file extension.</param>
    /// <param name="contentType">Content type for the new item.</param>
    /// <param name="relations">Parent relations collection.</param>
    /// <param name="relationType">Relation type.</param>
    /// <param name="itemsCounter">Variable used as counter of already created items of the same type.</param>
    /// <param name="item">Created archive item.</param>
    /// <returns>Represents relation ID</returns>
    public string PrepareNewItem( string itemNameStart, string extension, string contentType,
      RelationCollection relations, string relationType, ref int itemsCounter, out ZipArchiveItem item )
    {
      // 1. Register content type
      m_dicDefaultTypes[ extension ] = contentType;

      // Generate item name.
      string itemName = GenerateItemName( ref itemsCounter, itemNameStart, extension );

      item = m_archive.AddItem( itemName, new MemoryStream(), true, FileAttributes.Archive );
      string relationId = relations.GenerateRelationId();
      Relation result = new Relation( '/' + itemName, relationType );
      relations[ relationId ] = result;

      return relationId;
    }
    /// <summary>
    /// Converts image format into picture file extension.
    /// </summary>
    /// <param name="format">Format to convert.</param>
    /// <returns>Extension for picture file.</returns>
    private string GetExtension( ImageFormat format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      string strResult;
      if( format.Equals( ImageFormat.Bmp ) )
      {
        strResult = "bmp";
      }
#if !SILVERLIGHT && !WINRT && !WP
      else if (format.Equals(ImageFormat.Tiff))
      {
          strResult = "tiff";
      }
      else if (format.Equals(ImageFormat.Exif))
      {
          strResult = "exif";
      }
#endif
      else if (format.Equals(ImageFormat.Wmf))
      {
          strResult = "wmf";
      }
      else if (format.Equals(ImageFormat.Icon))
      {
          strResult = "icon";
      }
      else if( format.Equals( ImageFormat.Jpeg ) )
      {
        strResult = "jpeg";
      }
      else if( format.Equals( ImageFormat.Png ) )
      {
        strResult = "png";
      }
      else if( format.Equals( ImageFormat.Emf ) )
      {
        strResult = "emf";
      }
      else if( format.Equals( ImageFormat.Gif ) )
      {
        strResult = "gif";
      }
      //      else if( format.Equals( ImageFormat.Wmf ) )
      //      {
      //        return MsoBlipType.msoblipWMF;
      //      }
      else
      {
        strResult = "png";
      }

      return strResult;
    }
    /// <summary>
    /// Parses workbook.
    /// </summary>
    /// <param name="themeColors">Represents theme color in workbook.</param>
    private void ParseWorkbook( ref List<Color> themeColors, bool parseOnDemand )
    {
      string workbookItemName = m_strWorkbookPartName;

      if( workbookItemName == null || workbookItemName.Length == 0 )
        throw new ArgumentOutOfRangeException( "workbookItemName" );

      if( workbookItemName[ 0 ] == '/' )
        workbookItemName = UtilityMethods.RemoveFirstCharUnsafe( workbookItemName );

      ZipArchiveItem workbookItem = m_archive[ workbookItemName ];

      if( workbookItem == null )
        throw new XmlException( "Cannot locate workbook item: " + workbookItemName );

      string strWorkbookRelations = GetCorrespondingRelations( workbookItemName );
      m_workbookRelations = ParseRelations( strWorkbookRelations );
      m_dictItemsToRemove.Add( strWorkbookRelations, null );

      // Here we have to find and parse styles and SST dictionary.
      Relation styles = m_workbookRelations.FindRelationByContentType( RelationTypes.Styles, out m_strStylesRelationId );
      if (styles == null)
      {
          this.Workbook.InsertDefaultFonts();
          this.Workbook.InsertDefaultValues();
      }
      Relation sst = m_workbookRelations.FindRelationByContentType( RelationTypes.SST, out m_strSSTRelationId );
      Relation theme = m_workbookRelations.FindRelationByContentType( RelationTypes.Themes, out m_strThemeRelationId );
      XmlReader reader;
      string strWorkbookPath;
      SeparateItemName( workbookItemName, out strWorkbookPath );

      MemoryStream functionGroups = new MemoryStream();
      reader = CreateReader( workbookItem );
      Parser.ParseWorkbook( reader, m_workbookRelations, this, strWorkbookPath,
        m_streamStart, m_streamEnd, ref m_lstBookViews, functionGroups );
      
      int maxProgressValue = m_book.TabSheets.Count + Excel2007Parser.AdditionalProgressItems;

      if( theme != null )
      {
        m_strThemesPartName = strWorkbookPath + theme.Target;
        reader = CreateReader( theme, strWorkbookPath );
        themeColors = Parser.ParseThemes( reader );
      }
      
      int progressIndex = 1;

      ITabSheets arrSheets = m_book.Objects;
      ApplicationImpl app = m_book.AppImplementation;
      app.RaiseProgressEvent( progressIndex, arrSheets.Count + Excel2007Parser.AdditionalProgressItems );

      if( styles != null )
      {
        m_strStylesPartName = strWorkbookPath + styles.Target;
        reader = CreateReader( styles, strWorkbookPath );
        m_arrCellFormats = Parser.ParseStyles( reader, ref m_streamDxfs );
        m_dictItemsToRemove.Add( m_strStylesPartName, null );
      }
      Relation connect = m_workbookRelations.FindRelationByContentType(RelationTypes.Connection, out m_strConnectionId);
      if (connect != null)
      {
          m_connectionPartName = strWorkbookPath + connect.Target;
          reader = CreateReader(connect, strWorkbookPath);
          Parser.ParseConnections(reader);
          m_dictItemsToRemove.Add(m_connectionPartName, null);
          m_workbookRelations.RemoveByContentType(RelationTypes.Connection);
      }
      progressIndex++;
      app.RaiseProgressEvent( progressIndex, maxProgressValue );

      Dictionary<int, int> dictUpdatedSSTIndex = null;
      if (sst != null)
      {
          string strItemPath;
          m_strSSTPartName = (sst.Target[0] != '/') ?
            strWorkbookPath + sst.Target :
            sst.Target;

          ZipArchiveItem item = GetItem(sst, strWorkbookPath, out strItemPath);
          if (item != null)
          {
              reader = CreateReader(item);
              dictUpdatedSSTIndex = Parser.ParseSST(reader, parseOnDemand);
              m_dictItemsToRemove.Add(m_strSSTPartName, null);
          }

      }

      progressIndex++;
      app.RaiseProgressEvent( progressIndex, maxProgressValue );

      if( functionGroups.Length != 0 )
      {
        m_functionGroups = functionGroups;
      }
      else
      {
#if ( WINRT )
          functionGroups.Dispose();
#else
        functionGroups.Close();
#endif
      }

      Parser.ParseWorksheets( dictUpdatedSSTIndex, parseOnDemand );
      ParsePivotCaches(strWorkbookPath);
      Parser.ParsePivotTables();
      //if( dictUpdatedSSTIndex != null && dictUpdatedSSTIndex.Count > 0 )
      //  m_book.InnerSST.UpdateLabelSSTIndexes( dictUpdatedSSTIndex );

      // TODO: we don't know calc chain format and don't support it, so we have to remove
      // this item from the file.
      RemoveCalcChain();
    }
    /// <summary>
    /// Parses the metaProperties
    /// </summary>
    private void ParseMetaProperties()
    {
        for (int i = 1; i < m_archive.Count; i++)
        {
            string customXmlItemName = string.Format(CustomXmlPartName, i);

            if (customXmlItemName == null || customXmlItemName.Length == 0)
                throw new ArgumentOutOfRangeException("CustomXmlPartName");

            if (customXmlItemName[0] == '/')
                customXmlItemName = UtilityMethods.RemoveFirstCharUnsafe(customXmlItemName);

            ZipArchiveItem customXmlItem = m_archive[customXmlItemName];

            if (customXmlItem != null)
            {
                XmlReader reader = CreateReader(customXmlItem);

                Parser.ParseMetaProperties(reader, this, customXmlItem.DataStream,customXmlItem.ItemName);
            }
        }
    }
    /// <summary>
    /// Parses the CustomXmlParts
    /// </summary>
    private void ParseCustomXmlParts()
    {
        List<string> xmlID = new List<string>();
        List<string> itemProperties = new List<string>();
        List<string> customXmlName = new List<string>();
        List<string> schemas = null;
        

        foreach (KeyValuePair<string, Relation> value in m_workbookRelations)
        {
            Relation relation = value.Value;

            if (relation.Type == Excel2007Serializator.CustomXmlPartName)
            {
                if (!xmlID.Contains(value.Key))
                {
                    xmlID.Add(value.Key);
                    customXmlName.Add(value.Value.Target);
                }
            }
        }
        foreach (string value in xmlID)
        {
            m_workbookRelations.Remove(value);
        }

        foreach (KeyValuePair<string, string> value in m_dicOverriddenTypes)
        {
            if (value.Value == ContentTypes.CustomXmlProperties) 
            {
                if(!itemProperties.Contains(value.Key))
                {
                    itemProperties.Add(value.Key);
                }
            }
        }
        foreach (string value in itemProperties)
        {
            m_dicOverriddenTypes.Remove(value);
        }

        for (int i = 0; i < itemProperties.Count; i++)
        {
            string XmlId = null;

            string xmlName = customXmlName[i];
            string propertyName = itemProperties[i];

            if (xmlName[0] == '.')
                xmlName = xmlName.Substring(3, xmlName.Length - 3);

            if (propertyName.StartsWith("/"))
                propertyName = UtilityMethods.RemoveFirstCharUnsafe(propertyName);

            string relations = string.Format(Excel2007Serializator.CustomXmlRelation,xmlName.Substring(0,xmlName.IndexOf('/')), i + 1);

            ICustomXmlPartCollection customXmlParts = m_book.CustomXmlparts;

            XmlId = ParseCustomXmlItemProperties(propertyName, ref schemas);
            ParseCustomXmlParts(xmlName, customXmlParts, XmlId, schemas);

            m_dictItemsToRemove.Add(relations, null);
        }
    }
    /// <summary>
    /// Parses the CustomXml Items
    /// </summary>
    /// <param name="customXmlParts">CustomXmlParts collections</param>
    /// <param name="schemas">Schemas</param>
    /// <param name="xmlName">XmlPartName</param>
    /// <param name="xmlId">XmlID</param>
    private void ParseCustomXmlParts(string xmlName,ICustomXmlPartCollection customXmlParts,string xmlId,List<string> schemas)
    {
        if (xmlName == null || xmlName.Length == 0)
            throw new ArgumentOutOfRangeException("itemName");

        if (xmlId == null || xmlId.Length == 0)
            throw new ArgumentOutOfRangeException("itemName");

        ZipArchiveItem item = m_archive[xmlName];
        if (item != null)
        {
            Stream stream = item.DataStream;
            stream.Position = 0;

            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                ICustomXmlPart customxmlpart = customXmlParts.Add(xmlId, ms.ToArray());

                if (schemas != null && schemas.Count > 0)
                {
                    foreach (string schema in schemas)
                    {
                        customxmlpart.Schemas.Add(schema);
                    }
                }
            }

            m_dictItemsToRemove.Add(xmlName, null);
        }
        
    }
    /// <summary>
    /// Parses the CustomXmlParts ItemPropeties
    /// </summary>
    /// <param name="propertyName">PropertyName</param>
    /// <param name="schemas">Schemas</param>
    private string ParseCustomXmlItemProperties(string propertyName,ref List<string> schemas)
    {
        string id = null;
        if (propertyName == null || propertyName.Length == 0)
            throw new ArgumentOutOfRangeException("propertyName");

        ZipArchiveItem item = m_archive[propertyName];

        if (item != null)
        {
            Stream stream = item.DataStream;
            stream.Position = 0;
            XmlReader reader = UtilityMethods.CreateReader(stream);
            id=Parser.ParseItemProperties(reader,ref schemas);

            m_dictItemsToRemove.Add(propertyName, null);
        }

        return id;
    }
    
    internal void ParsePivotCaches(string strWorkbookPath)
    {
        PivotCacheCollection pivotCaches = new PivotCacheCollection(m_book.Application, m_book);
        string strItemName="";
        foreach (KeyValuePair<string, string> preservedCache in m_preservedCaches)
        {
            string relationID = preservedCache.Value;
            PivotCacheImpl pivotCache = ParsePivotCache(strWorkbookPath, relationID, out strItemName);
            int index = Convert.ToInt32(preservedCache.Key);
            m_book.PivotCaches.Add(index,pivotCache);
            ItemsToRemove.Add(strItemName, null);
            m_workbookRelations.Remove(relationID);
        }
        m_preservedCaches.Clear();
    }
    internal PivotCacheImpl ParsePivotCache(string strWorkbookPath,string relationID,out string strItemName)
    {
        Relation relation = m_workbookRelations[relationID];
        string parentPath = strWorkbookPath;
        XmlReader pivotCacheReader = CreateReader(relation, parentPath, out strItemName);
        string path = null;
        SeparateItemName(strItemName, out path);
        string relationPath = GetCorrespondingRelations(strItemName);
        RelationCollection pivotCacheRelations = ParseRelations(relationPath);
        PivotCacheImpl pivotCache = new PivotCacheImpl(m_book.Application, m_book);
        if (pivotCacheRelations == null)
            pivotCache.HasCacheRecords = false;
        else
        {
            pivotCache.HasCacheRecords = true;
        }
        string cacheRecordRelationID = null;
        pivotCache.preservedCacheRelations = pivotCacheRelations;
        PivotCacheParser.ParsePivotCacheDefinition(pivotCacheReader, pivotCache, m_book, path, pivotCacheRelations, out cacheRecordRelationID);
        if (cacheRecordRelationID != null)
        {
            Relation tRelation = pivotCacheRelations[cacheRecordRelationID];
            string itemPath;
            CreateReader(tRelation, path, out itemPath);
            ItemsToRemove.Add(itemPath, null);
            pivotCacheRelations.Remove(cacheRecordRelationID);
            pivotCache.HasCacheRecords = false;
        }
        return pivotCache;
    }
    /// <summary>
    /// Parses relations item.
    /// </summary>
    /// <param name="itemPath">Path to the item to parse.</param>
    /// <returns>Parsed relations collection; null if there are no such relation item.</returns>
    internal RelationCollection ParseRelations( string itemPath )
    {
      RelationCollection result = null;
      ZipArchiveItem item = m_archive[ itemPath ];

      if( item != null )
      {
        XmlReader reader = CreateReader( item );
        result = Parser.ParseRelations( reader );
        result.ItemPath = itemPath;
      }

      return result;
    }
    /// <summary>
    /// Tries to find path to the item by content type.
    /// </summary>
    /// <param name="contentType">Content type to locate.</param>
    /// <returns>First occurrence of the item with specified content type.</returns>
    private string FindItemByContent( string contentType )
    {
      string strResult = FindItemByContentInOverride( contentType );

      if( strResult == null )
        strResult = FindItemByContentInDefault( contentType );

      return strResult;
    }
    /// <summary>
    /// Tries to find path to the item by content type inside default types.
    /// </summary>
    /// <param name="contentType">Content type to locate.</param>
    /// <returns>First occurrence of the item with specified content type.</returns>
    private string FindItemByContentInDefault( string contentType )
    {
      string strResult = null;

      foreach( KeyValuePair<string, string> entry in m_dicDefaultTypes )
      {
        string strContentType = entry.Value;

        if( strContentType == contentType )
        {
          string extension = entry.Key;

          for( int i = 0, len = m_archive.Count; i < len; i++ )
          {
            ZipArchiveItem item = m_archive[ i ];
            string itemName = item.ItemName;

            if( itemName[ 0 ] != '/' )
              itemName = '/' + itemName;

            if( itemName.EndsWith( extension ) && !m_dicOverriddenTypes.ContainsKey( itemName ) )
            {
              strResult = itemName;
              break;
            }
          }

          break;
        }
      }

      return strResult;
    }
    /// <summary>
    /// Tries to find path to the item by content type inside overridden types.
    /// </summary>
    /// <param name="contentType">Content type to locate.</param>
    /// <returns>First occurrence of the item with specified content type.</returns>
    private string FindItemByContentInOverride( string contentType )
    {
      string strResult = null;

      foreach( KeyValuePair<string, string> entry in m_dicOverriddenTypes )
      {
        string strContentType = entry.Value;

        if( strContentType == contentType )
        {
          strResult = entry.Key;
          break;
        }
      }

      return strResult;
    }
    /// <summary>
    /// Gets relations.
    /// </summary>
    /// <param name="itemName">Item name to get corresponding relation.</param>
    /// <returns>Extracted relation</returns>
    internal static string GetCorrespondingRelations( string itemName )
    {
      if( itemName == null || itemName.Length == 0 )
        throw new ArgumentOutOfRangeException( "itemName" );

      string strPath;
      string strShortName = SeparateItemName( itemName, out strPath );
      string strResult = strPath + RelationsDirectory + '/' + strShortName + RelationExtension;

      return strResult;
    }
    /// <summary>
    /// Splits item name (including path) into item name and path to this item.
    /// </summary>
    /// <param name="itemName">Item name to split.</param>
    /// <param name="path">Path without item name.</param>
    /// <returns>Item name without path.</returns>
    internal static string SeparateItemName( string itemName, out string path )
    {
      int iLastSeparator = itemName.LastIndexOf( '/' );

      path = ( iLastSeparator >= 0 ) ?
        itemName.Substring( 0, iLastSeparator + 1 ) :
        string.Empty;

      return itemName.Substring( iLastSeparator + 1 );
    }
    /// <summary>
    /// Gets image.
    /// </summary>
    /// <param name="strFullPath">Path of the image.</param>
    /// <returns>Extracted image</returns>
    internal Image GetImage( string strFullPath )
    {
      if( strFullPath == null || strFullPath.Length == 0 )
        throw new ArgumentOutOfRangeException( "strFullPath" );

      Image result = null;
      ZipArchiveItem item = m_archive[ strFullPath ];

      if( item != null )
      {
        MemoryStream source = ( MemoryStream )item.DataStream;
        MemoryStream stream = new MemoryStream( ( int )source.Length );
        source.WriteTo( stream );
        stream.Position = 0;
        result = ApplicationImpl.CreateImage( stream );
        m_dictItemsToRemove[ strFullPath ] = null;
        if (result.RawFormat.Equals(ImageFormat.Emf) || result.RawFormat.Equals(ImageFormat.Wmf))
        {
            if(!m_metafileStream .ContainsKey (strFullPath ))
            m_metafileStream.Add(strFullPath, stream);
        }
      }

      return result;
    }
    /// <summary>
    /// Creates XmlReader for specified zip archive item.
    /// </summary>
    /// <param name="item">Item to create reader for.</param>
    /// <returns>Created reader.</returns>
    private static XmlReader CreateReader( ZipArchiveItem item )
    {
      if( item == null )
        throw new ArgumentNullException( "item" );

      Stream stream = item.DataStream;

      if( stream.CanSeek )
        stream.Position = 0;

      return UtilityMethods.CreateReader( stream );
    }
    /// <summary>
    /// Creates XmlReader for specified zip archive item.
    /// </summary>
    /// <param name="relation">Relation that points to the archive item.</param>
    /// <param name="parentItemPath">Path to the parent item.</param>
    /// <returns>Created reader.</returns>
    internal XmlReader CreateReader( Relation relation, string parentItemPath )
    {
      string strItemPath;
      return CreateReader( relation, parentItemPath, out strItemPath );
    }
    /// <summary>
    /// Creates XmlReader and fixes potential br tags issue.
    /// </summary>
    /// <param name="relation">Relation that points to the archive item.</param>
    /// <param name="parentItemPath">Path to the parent item.</param>
    /// <param name="strItemPath">Path to the desired item.</param>
    /// <returns>Created reader.</returns>
    public XmlReader CreateReaderAndFixBr( Relation relation, string parentItemPath, out string strItemPath )
    {
      ZipArchiveItem item = GetItem( relation, parentItemPath, out strItemPath );
      StreamReader reader = new StreamReader( item.DataStream );
      string data = reader.ReadToEnd();

      data = data.Replace( "<br></br>", "<br/>" );
      data = data.Replace( "<br>", "<br/>" );
      byte[] newData = Encoding.UTF8.GetBytes( data );
      MemoryStream newStream = new MemoryStream( newData );
      return UtilityMethods.CreateReader( newStream );
    }
    /// <summary>
    /// Creates XmlReader for specified zip archive item.
    /// </summary>
    /// <param name="relation">Relation that points to the archive item.</param>
    /// <param name="parentItemPath">Path to the parent item.</param>
    /// <param name="strItemPath">Path to the desired item.</param>
    /// <returns>Created reader.</returns>
    internal XmlReader CreateReader( Relation relation, string parentItemPath, out string strItemPath )
    {
      ZipArchiveItem item = GetItem( relation, parentItemPath, out strItemPath );

      return CreateReader( item );
    }
    /// <summary>
    /// Returns single zip item based on the relation and parent path.
    /// </summary>
    /// <param name="relation">Relation that points to the archive item.</param>
    /// <param name="parentItemPath">Path to the parent item.</param>
    /// <param name="strItemPath">Path to the desired item.</param>
    /// <returns>Requested zip item.</returns>
    private ZipArchiveItem GetItem( Relation relation, string parentItemPath, out string strItemPath )
    {
      if( relation == null )
        throw new ArgumentNullException( "relation" );

      string strTarget = relation.Target;

      if( parentItemPath != null )
      {
        strTarget = CombinePath( parentItemPath, strTarget );//Path.Combine( parentItemPath, strTarget );//parentItemPath + strTarget;
        strTarget.Replace( '\\', '/' );
      }

      ZipArchiveItem item = m_archive[ strTarget ];
      strItemPath = strTarget;
      return item;
    }
    /// <summary>
    /// Creates XmlReader for specified zip archive item.
    /// </summary>
    /// <param name="relation">Relation that points to the archive item.</param>
    /// <param name="parentItemPath">Path to the parent item.</param>
    /// <param name="removeItem">Indicates whether item must be removed after extracting data.</param>
    /// <returns>Created reader.</returns>
    internal byte[] GetData( Relation relation, string parentItemPath, bool removeItem )
    {
      if( relation == null )
        throw new ArgumentNullException( "relation" );

      string strTarget = relation.Target;

      if( parentItemPath != null )
      {
        strTarget = CombinePath( parentItemPath, strTarget );//Path.Combine( parentItemPath, strTarget );//parentItemPath + strTarget;
        strTarget.Replace( '\\', '/' );
      }

      ZipArchiveItem item = m_archive[ strTarget ];
      Stream dataStream = item.DataStream;
      byte[] result = new byte[ dataStream.Length ];
      dataStream.Position = 0;
      dataStream.Read( result, 0, ( int )dataStream.Length );

      if( removeItem )
        m_archive.RemoveItem( strTarget );

      return result;
    }
    /// <summary>
    /// Parses external link.
    /// </summary>
    /// <param name="relationId">Represents relation id.</param>
    internal void ParseExternalLink( string relationId )
    {
      Relation linkRelation = m_workbookRelations[ relationId ];
      string parentItemPath;
      string strItemPath;
      SeparateItemName( m_strWorkbookPartName, out parentItemPath );
      XmlReader reader = CreateReader( linkRelation, parentItemPath, out strItemPath );
      string strRelations = GetCorrespondingRelations( strItemPath );
      RelationCollection relations = ParseRelations( strRelations );
      bool removeRelation = Parser.ParseExternalLink( reader, relations );
      if (removeRelation)
      {
          m_dictItemsToRemove.Add(strItemPath, null);
          m_workbookRelations.Remove(relationId);
      }
      else
      {
          this.m_book.PreservedExternalLinks.Add(relationId);
      }
      //throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Tries to combine two paths into one.
    /// </summary>
    /// <param name="startPath">First part of the path.</param>
    /// <param name="endPath">Second part of the path.</param>
    /// <returns>Combined path.</returns>
    internal static string CombinePath( string startPath, string endPath )
    {
      if( startPath == null )
        throw new ArgumentOutOfRangeException( "startPath" );

      if( endPath == null || endPath.Length == 0 )
        throw new ArgumentOutOfRangeException( "endPath" );

      if( startPath.Length > 0 && startPath[ startPath.Length - 1 ] == '/' )
        startPath = startPath.Substring( 0, startPath.Length - 1 );

      while( endPath.StartsWith( "../" ) )
      {
        int iSlashIndex = startPath.LastIndexOf( '/' );

        if( iSlashIndex >= 0 )
        {
          endPath = endPath.Substring( 3, endPath.Length - 3 );
          startPath = startPath.Substring( 0, iSlashIndex );
        }
        else
        {
          break;
        }
      }

      return ( endPath.StartsWith( "/" ) ) ?
        UtilityMethods.RemoveFirstCharUnsafe( endPath ) :
        (startPath != "") ? startPath + '/' + endPath : endPath;
    }
    /// <summary>
    /// Creates or updates content types and saves them inside internal zip archive.
    /// </summary>
    private void SaveContentTypes()
    {
      //1. Add Default types.
      FillDefaultContentTypes();

      SaveArchiveItem( ContentTypesItemName );
    }
    /// <summary>
    /// Saves all document properties.
    /// </summary>
    private void SaveDocumentProperties()
    {
      SaveArchiveItemRelationContentType( ExtendedPropertiesPartName, ContentTypes.ExtendedProperties,
        RelationTypes.ExtendedProperties );
      SaveArchiveItemRelationContentType( CorePropertiesPartName, ContentTypes.CoreProperties,
        RelationTypes.CoreProperties );
      SaveArchiveItemRelationContentType( CustomPropertiesPartName, ContentTypes.CustomProperties,
        RelationTypes.CustomProperties );
    }
    /// <summary>
    /// Saves all ContentTypeProperties
    /// </summary>
    private void SaveContentTypeProperties()
    {
        string itemName = m_book.InnerContentTypeProperties.ItemName;

        if (m_book.InnerContentTypeProperties.IsValid && itemName != null && itemName.Length > 0 )
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(stream);
            XmlWriter writer = UtilityMethods.CreateWriter(streamWriter);

            Serializator.SerializeContentTypeProperties(writer);

            writer.Flush();


            ZipArchiveItem item = m_archive[itemName];

            if (item != null)
            {
                item.Update(stream, true);
            }
        }
    }
    /// <summary>
    /// Saves archive item, adds corresponding record into content type and relation collections.
    /// </summary>
    /// <param name="partName">Part name value.</param>
    /// <param name="contentType">Content type value.</param>
    /// <param name="relationType">Relation type value.</param>
    private void SaveArchiveItemRelationContentType( string partName, string contentType, string relationType )
    {
      m_dicOverriddenTypes[ "/" + partName ] = contentType;
      string strRelationId;

      m_topRelations.FindRelationByContentType( relationType, out strRelationId );

      if( strRelationId == null )
        strRelationId = m_topRelations.GenerateRelationId();

      m_topRelations[ strRelationId ] = new Relation( partName, relationType );

      SaveArchiveItem( partName );
    }
    /// <summary>
    /// Saves archive item by part name into zip archive item.
    /// </summary>
    /// <param name="strItemPartName">Item part name.</param>
    private void SaveArchiveItem( string strItemPartName )
    {
      MemoryStream stream = new MemoryStream();
      StreamWriter streamWriter = new StreamWriter( stream );
      XmlWriter writer = UtilityMethods.CreateWriter( streamWriter );

      switch( strItemPartName )
      {
        case ExtendedPropertiesPartName:
          Serializator.SerializeExtendedProperties( writer );
          break;

        case CorePropertiesPartName:
          Serializator.SerializeCoreProperties( writer );
          break;
          
        case ContentTypesItemName:
          Serializator.SerializeContentTypes( writer, m_dicDefaultTypes, m_dicOverriddenTypes );
          break;

        case CustomPropertiesPartName:
          Serializator.SerializeCustomProperties( writer );
          break;

        default:
          throw new ArgumentException( "strItemPartName" ); 
      }
      
      writer.Flush();

      ZipArchiveItem item = m_archive[ strItemPartName ];

      if( item != null )
      {
        item.Update( stream, true );
      }
      else
      {
        m_archive.AddItem( strItemPartName, stream, true, FileAttributes.Archive );
      }
    }
    /// <summary>
    /// Fills default content types.
    /// </summary>
    private void FillDefaultContentTypes()
    {
      m_dicDefaultTypes[ XmlExtension ] = ContentTypes.Xml;
      m_dicDefaultTypes[ RelsExtension ] = ContentTypes.Relations;
    }
    /// <summary>
    /// Saves top level relation in zip.
    /// </summary>
    private void SaveTopLevelRelations()
    {
      //if( m_topRelations == null )
      //{
      //  m_topRelations = new RelationsCollection();
      //  m_topRelations[ GetRelationId( 1 ) ] = new Relation( m_strWorkbookPartName,
      //    RelationTypes.Workbook );
      //}

      MemoryStream stream = new MemoryStream();
      StreamWriter streamWriter = new StreamWriter( stream );
      XmlWriter writer = UtilityMethods.CreateWriter( streamWriter/*, Encoding.UTF8*/ );
      Serializator.SerializeRelations( writer, m_topRelations, null );
      writer.Flush();

      ZipArchiveItem item = m_archive[ TopRelationsPath ];

      if( item != null )
      {
        item.Update( stream, true );
      }
      else
      {
        m_archive.AddItem( TopRelationsPath, stream, true, FileAttributes.Archive );
      }
    }
    /// <summary>
    /// Generates relation id for specified relation index.
    /// </summary>
    /// <param name="relationIndex">Relation index to generate relation id.</param>
    /// <returns>Generated relation id.</returns>
    private static string GetRelationId( int relationIndex )
    {
      return string.Format( RelationIdFormat, relationIndex );
    }
    /// <summary>
    /// Saves workbook.
    /// </summary>
    private void SaveWorkbook( ExcelSaveType saveAsType )
    {
#if MEASURE_PERFORMANCE
      DateTime saveWorkbookStart = DateTime.Now;
#endif

      m_strWorkbookContentType = SelectWorkbookContentType( saveAsType );

      if( m_topRelations == null )
      {
        m_topRelations = new RelationCollection();
        m_topRelations[ GetRelationId( 1 ) ] = new Relation( m_strWorkbookPartName,
          RelationTypes.Workbook );
      }

      Dictionary<int, int> hashNewXFIndexes = SaveStyles();
      Dictionary<PivotCacheImpl, string> dictCacheFiles = SavePivotCaches();
      SaveSST();
      
      CustomXmlParts();      
      m_arrImageItemNames = SaveWorkbookImages();
      SaveWorkbookPart( hashNewXFIndexes, dictCacheFiles );

      Connections();
      m_arrImageItemNames = null;

#if MEASURE_PERFORMANCE
      DateTime saveWorkbookEnd = DateTime.Now;
      Console.WriteLine( "SaveWorkbook() took: {0}", saveWorkbookEnd - saveWorkbookStart );
#endif
    }
    /// <summary>
    /// Selects correct workbook content type depending on workbook's content and desired save type.
    /// </summary>
    /// <returns>Workbook content type.</returns>
    private string SelectWorkbookContentType (ExcelSaveType saveType )
    {
      string result;

      if( m_book.HasMacros )
      {
        result = ( saveType == ExcelSaveType.SaveAsTemplate ) ?
          ContentTypes.MacroTemplate :
          ContentTypes.MacroWorkbook;
      }
      else
      {
        result = ( saveType == ExcelSaveType.SaveAsTemplate ) ?
          ContentTypes.Template :
          ContentTypes.Workbook;
      }

      return result;
    }
    /// <summary>
    /// Saves all exisiting pivot caches.
    /// </summary>
    private Dictionary<PivotCacheImpl, string> SavePivotCaches()
    {
      Dictionary<PivotCacheImpl, string> result = new Dictionary<PivotCacheImpl, string>();
      PivotCacheCollection pivotCaches = m_book.PivotCaches;

      int iCount = ( pivotCaches != null ) ? pivotCaches.Count : 0;

      if( iCount > 0 )
      {
        foreach( PivotCacheImpl cache in pivotCaches )
        {
          string strCacheItem = SavePivotCache( cache );
          result[ cache ] = strCacheItem;
        }
      }

      return result;
    }
    /// <summary>
    /// Saves single pivot cache item.
    /// </summary>
    /// <param name="cache">Cache object to save.</param>
    /// <returns>Name of the file item with cache definition.</returns>
    private string SavePivotCache( PivotCacheImpl cache )
    {
      string strCacheRecordFileName =null;
        if(!cache.HasCacheRecords)
            strCacheRecordFileName= SavePivotCacheRecords( cache );
      string result = SavePivotCacheDefinition( cache, strCacheRecordFileName );
      return result;
    }
    /// <summary>
    /// Saves pivot cache definition for the specified cache.
    /// </summary>
    /// <param name="cache">Cache to save definition for.</param>
    /// <param name="cacheRecordFileName">Name of the file with pivot cache records for this pivot cache.</param>
    /// <returns>Name of the file item with cache definition.</returns>
    private string SavePivotCacheDefinition( PivotCacheImpl cache, string cacheRecordFileName )
    {
      string strCacheDefinitionFileName = GeneratePivotCacheFileName( cache );
      RelationCollection relations = new RelationCollection();
      if (cache.preservedCacheRelations !=null && cache.preservedCacheRelations.Count > 0)
          relations = cache.preservedCacheRelations;
      string recordsRelation = null;
      if (cacheRecordFileName != null)
      {
          recordsRelation = relations.GenerateRelationId();
          relations[recordsRelation] = new Relation('/' + cacheRecordFileName, RelationTypes.PivotCacheRecords);
      }
        if(cache.PreservedExtenalRelation!=null)
            relations[cache.RelationId]= cache.PreservedExtenalRelation;

      MemoryStream streamCache = new MemoryStream();
      XmlWriter writer = UtilityMethods.CreateWriter( streamCache, Encoding.UTF8 );
      PivotCacheSerializator.SerializePivotCacheDefinition( writer, cache, m_book, recordsRelation, relations );
      writer.Flush();

      AddOverriddenContentType( strCacheDefinitionFileName, ContentTypes.PivotCacheDefinition );
      m_archive.UpdateItem( strCacheDefinitionFileName, streamCache, true, FileAttributes.Archive );
      SaveRelations( strCacheDefinitionFileName, relations );
      return strCacheDefinitionFileName;
    }
    /// <summary>
    /// Saves pivot cache records.
    /// </summary>
    /// <param name="cache">Cache to save records for.</param>
    /// <returns>Name of the save item.</returns>
    private string SavePivotCacheRecords( PivotCacheImpl cache )
    {
      string strCacheRecordFileName = GeneratePivotCacheRecordsFileName( cache );

      MemoryStream streamRecords = new MemoryStream();
      XmlWriter writer = UtilityMethods.CreateWriter( streamRecords, Encoding.UTF8 );
      PivotCacheSerializator.SerializePivotCacheRecords( writer, cache,streamRecords );
      writer.Flush();

      m_archive.UpdateItem( strCacheRecordFileName, streamRecords, true, FileAttributes.Archive );
      AddOverriddenContentType( strCacheRecordFileName, ContentTypes.PivotCacheRecords );

      return strCacheRecordFileName;
    }
    /// <summary>
    /// Generates unique name for pivot cache records.
    /// </summary>
    /// <param name="cache">Cache to generate name for.</param>
    /// <returns>Generated file name.</returns>
    private string GeneratePivotCacheRecordsFileName( PivotCacheImpl cache )
    {
      // TODO: store caches index.
      int iCounter = 0;
      return string.Format(PivotCacheRecordsPathFormat, ++LastPivotCacheRecordsIndex);
    }
    /// <summary>
    /// Generates cache file anme.
    /// </summary>
    /// <param name="cache">Cache to generate file name for.</param>
    /// <returns>Generated file name.</returns>
    private string GeneratePivotCacheFileName( PivotCacheImpl cache )
    {
      // TODO: store caches index.
      int iCounter = 0;
      return string.Format(PivotCacheDefinitionPathFormat, ++LastPivotCacheIndex);
    }
    /// <summary>
    /// Saves all workbook images.
    /// </summary>
    /// <returns>Array containing zip name with images.</returns>
    private string[] SaveWorkbookImages()
    {
#if MEASURE_PERFORMANCE
      DateTime methodStart = DateTime.Now;
#endif
      WorkbookShapeDataImpl shapesData = m_book.ShapesData;
      List<MsofbtBSE> arrPictures = shapesData.Pictures;
      int iCount = ( arrPictures != null ) ? arrPictures.Count : 0;
      string[] arrItemNames = new string[ iCount ];

      // List with bse indexes that doesn't have item name.
      List<int> lstNoNameBse = new List<int>();

      for( int i = 0, len = iCount; i < len; i++ )
      {
        MsofbtBSE bse = arrPictures[ i ];

        if( bse.PicturePath != null )
        {
          arrItemNames[ i ] = SerializeBSE( bse );
        }
        else
        {
          lstNoNameBse.Add( i );
        }
      }

      for( int i = 0, len = lstNoNameBse.Count; i < len; i++ )
      {
        int iBSEIndex = lstNoNameBse[ i ];
        MsofbtBSE bse = arrPictures[ iBSEIndex ];
        arrItemNames[ iBSEIndex ] = SerializeBSE( bse );
      }

#if MEASURE_PERFORMANCE
      DateTime methodEnd = DateTime.Now;
      Console.WriteLine( "SaveWorkbookImages() took: {0}", methodEnd - methodStart );
#endif

      return arrItemNames;
    }
    /// <summary>
    /// Serializes image from MsofbtBSE record.
    /// </summary>
    /// <param name="bse">Record to serialize.</param>
    /// <returns>Name of the zip item with image.</returns>
    private string SerializeBSE( MsofbtBSE bse )
    {
      if( bse == null )
        throw new ArgumentNullException( "bse" );
#if !SILVERLIGHT && !WINRT && !WP
      if (bse.BlipType != MsoBlipType.msoblipERROR && bse .PictureRecord.PictureStream != null )
      {
          bse.PictureRecord.Picture = Image.FromStream(bse.PictureRecord.PictureStream);
      }
#endif
      return ( bse.BlipType != MsoBlipType.msoblipERROR ) ?
        SaveImage( bse.PictureRecord.Picture, bse.PicturePath ) :
        null;
      //throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Saves CustomXmlParts into internal zip archive.
    /// </summary>
    private void CustomXmlParts()
    {

        ICustomXmlPartCollection customXmlParts = m_book.CustomXmlparts;

        if (customXmlParts!=null && customXmlParts.Count>0)
        {
            for (int i = 0; i < customXmlParts.Count; i++)
            {

                string xmlpartname = string.Format(Excel2007Serializator.XmlItemName, i + 1);
                string propertyname = string.Format(Excel2007Serializator.XmlPropertiesName, i + 1);

                ICustomXmlPart customXmlPart = customXmlParts[i];

                if (customXmlPart != null)
                {
                    SerializeCustomXmlRelation(xmlpartname,propertyname);
                    SerializeCustomXmlPart(xmlpartname, customXmlPart.Data);
                    SerializeCustomXmlItemProperty(xmlpartname,propertyname,customXmlPart);
                }
               
            }
               
        }
    }
    /// <summary>
    /// Saves ItemProperties Relation in the zip Archive
    /// </summary>
    private void SerializeCustomXmlRelation(string xmlpartname,string propertyname)
    {
        string type = Excel2007Serializator.CustomXmlItemPropertiesRelation;
        string target = propertyname;

        RelationCollection m_customXmlRelations = new RelationCollection();
        Relation relation = new Relation(target, type);
                
        m_customXmlRelations.Add(relation);

        string path = "customXml/" + xmlpartname;
        SaveRelations(path, m_customXmlRelations);
    }
    /// <summary>
    /// Saves the Xml Data in Zip Archive
    /// </summary>
    private void SerializeCustomXmlPart(string xmlpartname,byte[] data)
    {
        string path = "customXml/" + xmlpartname;

        if (data != null)
        {
            MemoryStream stream = new MemoryStream(data);
            m_archive.UpdateItem(path, stream, true, FileAttributes.Archive);
        }
        else
        {
            MemoryStream stream = new MemoryStream();
            m_archive.UpdateItem(path, stream, true, FileAttributes.Archive);
        }
    }
    /// <summary>
    /// Saves CustomXmlParts Items Properties and Schema Collections
    /// </summary>
    /// <param name="customXmlPart">customXmlPart</param>
    /// <param name="propertyname">PropertyName to be serialized</param>
    /// <param name="xmlpartname">xml PartName to be Serialized</param>
    private void SerializeCustomXmlItemProperty(string xmlpartname, string propertyname, ICustomXmlPart customXmlPart)
    {
        string propertypath = "/customXml/" + propertyname;
        string path = "customXml/" + propertyname;

        AddSlashPreprocessor preprocessor = new AddSlashPreprocessor();
        m_dicOverriddenTypes[preprocessor.PreprocessName(propertypath)] = ContentTypes.CustomXmlProperties;

        MemoryStream stream = new MemoryStream();
        StreamWriter streamWriter = new StreamWriter(stream);
        XmlWriter writer = UtilityMethods.CreateWriter(streamWriter/*, Encoding.UTF8*/ );
        Serializator.SerializeCustomXmlPartProperty(writer, customXmlPart);

        writer.Flush();

        m_archive.UpdateItem(path, stream, true, FileAttributes.Archive);

    }
    /// <summary>
    /// Saves styles into internal zip archive.
    /// </summary>
    /// <returns>Dictionary with new XF indexes.</returns>
    private Dictionary<int, int> SaveStyles()
    {
#if MEASURE_PERFORMANCE
      DateTime methodStart = DateTime.Now;
#endif
      AddSlashPreprocessor preprocessor = new AddSlashPreprocessor();
      m_dicOverriddenTypes[ preprocessor.PreprocessName( m_strStylesPartName ) ] = ContentTypes.Styles;

      MemoryStream stream = new MemoryStream();
      StreamWriter streamWriter = new StreamWriter( stream );
      XmlWriter writer = UtilityMethods.CreateWriter( streamWriter/*, Encoding.UTF8*/ );

      Dictionary<int, int> hashNewXFIndexes = Serializator.SerializeStyles( writer, ref m_streamDxfs );
      writer.Flush();

      string strStylesItem = m_strStylesPartName;

      if( m_strStylesPartName[ 0 ] == '/' )
        strStylesItem = UtilityMethods.RemoveFirstCharUnsafe( m_strStylesPartName );

      int stylesCount = 0;
      foreach (KeyValuePair<string, string> dictionary in m_dicOverriddenTypes)
          if (dictionary.Value == ContentTypes.Styles.ToString())
              stylesCount += 1;

      if ((hashNewXFIndexes.Count > 0) && (stylesCount == 1))
      {
          m_archive.UpdateItem(strStylesItem, stream, true, FileAttributes.Archive);
      }

#if MEASURE_PERFORMANCE
      DateTime methodEnd = DateTime.Now;
      Console.WriteLine( "SaveStyles() took: {0}", methodEnd - methodStart );
#endif

      return hashNewXFIndexes;
    }
    /// <summary>
    /// Saves shared strings table into internal zip archive.
    /// </summary>
    private void SaveSST()
    {
#if MEASURE_PERFORMANCE
      DateTime saveSSTStart = DateTime.Now;
#endif
      SSTDictionary sst = m_book.InnerSST;
      int iActiveCount = sst.ActiveCount;

      if( iActiveCount <= 0 )
        return;

      AddSlashPreprocessor preprocessor = new AddSlashPreprocessor();
      m_dicOverriddenTypes[ preprocessor.PreprocessName( m_strSSTPartName ) ] = ContentTypes.SharedStrings;

#if !SILVERLIGHT && !WINRT && !WP && !SyncfusionFramework1_1 && !SyncfusionFramework1_0
      ZippedContentStream stream = new ZippedContentStream( m_book.AppImplementation.CreateCompressor );
#else
      MemoryStream stream = new MemoryStream();
#endif
      StreamWriter streamWriter = new StreamWriter( stream );
      XmlWriter writer = UtilityMethods.CreateWriter( streamWriter/*, Encoding.UTF8*/ );
      Serializator.SerializeSST( writer );
      writer.Flush();

      string strItemName = m_strSSTPartName;

      if( m_strSSTPartName[ 0 ] == '/' )
        strItemName = UtilityMethods.RemoveFirstCharUnsafe( m_strSSTPartName );
        
#if !SILVERLIGHT && !WINRT && !WP && !SyncfusionFramework1_1 && !SyncfusionFramework1_0
      ZipArchiveItem item = m_archive[ strItemName ];

      if( item == null )
        item = m_archive.AddItem( strItemName, null, false, FileAttributes.Archive );

      item.Update( stream );
#else
      m_archive.UpdateItem( strItemName, stream, true, FileAttributes.Archive );
#endif

#if MEASURE_PERFORMANCE
      DateTime saveSSTEnd = DateTime.Now;
      Console.WriteLine( "SaveSST() took: {0}", saveSSTEnd - saveSSTStart );
#endif
    }
    /// <summary>
    /// Saves workbook part into internal zip archive.
    /// </summary>
    private void SaveWorkbookPart( Dictionary<int, int> hashNewXFIndexes, Dictionary<PivotCacheImpl, string> cacheFiles )
    {
#if MEASURE_PERFORMANCE
      DateTime methodStart = DateTime.Now;
#endif
      AddSlashPreprocessor preprocessor = new AddSlashPreprocessor();
      m_dicOverriddenTypes[ preprocessor.PreprocessName( m_strWorkbookPartName ) ] = m_strWorkbookContentType;

      if( m_workbookRelations == null )
        m_workbookRelations = new RelationCollection();

      SaveSheets( m_workbookRelations, m_strWorkbookPartName, hashNewXFIndexes, cacheFiles );

      Stream stream = new MemoryStream();
      StreamWriter streamWriter = new StreamWriter( stream );
      XmlWriter writer = UtilityMethods.CreateWriter( streamWriter/*, Encoding.UTF8*/ );
      Serializator.SerializeWorkbook( writer, m_streamStart, m_streamEnd, m_lstBookViews,
        m_workbookRelations, cacheFiles, m_functionGroups );
      writer.Flush();

      m_archive.UpdateItem( m_strWorkbookPartName, stream, true, FileAttributes.Archive );

      string strPath;
      SeparateItemName( m_strWorkbookPartName, out strPath );

      m_strStylesRelationId = AddRelation( m_workbookRelations, m_strStylesPartName,
        strPath, RelationTypes.Styles, m_strStylesRelationId );

      if( m_book.InnerSST.ActiveCount > 0 )
      {
        m_strSSTRelationId = AddRelation( m_workbookRelations, m_strSSTPartName,
          strPath, RelationTypes.SST, m_strSSTRelationId );
      }

      if (m_book.CustomXmlparts!=null && m_book.CustomXmlparts.Count>0)
      {
          for (int i = 0; i < m_book.CustomXmlparts.Count; i++)
          {
              string path = "../customXml/item{0}.xml";

              path = string.Format(path, i+1);

              AddRelation(m_workbookRelations, path, "customXml", Excel2007Serializator.CustomXmlPartName, null);
          }
      }
      if ((m_book.Connections != null && m_book.Connections.Count > 0) || m_book.DeletedConnections != null && m_book.DeletedConnections.Count > 0)
      {
          string conn_str;
          m_workbookRelations.FindRelationByContentType(RelationTypes.Connection, out conn_str);
          if(conn_str==null)
          {
          string path = "connections.xml";
          AddRelation(m_workbookRelations, path, "connection.xml", RelationTypes.Connection, null);
          }
          //m_workbookRelations.Add(connection);
      }

      SaveRelations( m_strWorkbookPartName, m_workbookRelations );

#if MEASURE_PERFORMANCE
      DateTime methodEnd = DateTime.Now;
      Console.WriteLine( "SaveWorkbookPart() took: {0}", methodEnd - methodStart );
#endif
    }
    /// <summary>
    /// Adds relations to the collection, tries to re-use specified relation id if not null.
    /// </summary>
    /// <param name="relations">Collection to add new relation to.</param>
    /// <param name="target">Relation target.</param>
    /// <param name="parentPath">Parent path for the relation target.</param>
    /// <param name="type">Relation type.</param>
    /// <param name="relationId">Relation id to re-use if not null.</param>
    /// <returns>Used relation id.</returns>
    private string AddRelation( RelationCollection relations, string target, string parentPath,
      string type, string relationId )
    {
      if( target[ 0 ] == '/' )
      {
        target = UtilityMethods.RemoveFirstCharUnsafe( target );
      }

      if( target.StartsWith( parentPath ) )
      {
        target = target.Substring( parentPath.Length );
      }

      Relation relation = new Relation( target, type );

      if( relationId != null )
      {
        relations[ relationId ] = relation;
      }
      else
      {
        relationId = relations.Add( relation );
      }

      return relationId;
    }
    /// <summary>
    /// Serializes relations collection.
    /// </summary>
    /// <param name="parentPartName">Name of the parent</param>
    /// <param name="relations">Represents relations</param>
    public void SaveRelations( string parentPartName, RelationCollection relations )
    {
      if( relations == null || relations.Count == 0 )
        return;

      if( parentPartName == null || parentPartName.Length == 0 )
        throw new ArgumentOutOfRangeException( "parentPartName" );

      string strRelationItem = GetCorrespondingRelations( parentPartName );
      MemoryStream stream = new MemoryStream();
      StreamWriter streamWriter = new StreamWriter( stream );
      XmlWriter writer = UtilityMethods.CreateWriter( streamWriter/*, Encoding.UTF8*/ );
      Serializator.SerializeRelations( writer, relations, null );
      writer.Flush();

      m_archive.UpdateItem( strRelationItem, stream, true, FileAttributes.Archive );
    }
    /// <summary>
    /// Saves all sheets (worksheets and chartsheets) into internal zip archive
    /// and updates relations collection.
    /// </summary>
    /// <param name="relations">Workbook relations collection.</param>
    /// <param name="workbookItemName">Name of the workbook item.</param>
    /// <param name="hashNewXFIndexes">Dictionary with new XF indexes, key - old index, value - new index.</param>
    /// <param name="cacheFiles">Dictionary that will contain pivot cache files
    /// (key - cache object, value - cache file name).</param>
    private void SaveSheets( RelationCollection relations, string workbookItemName,
      Dictionary<int, int> hashNewXFIndexes, Dictionary<PivotCacheImpl, string> cacheFiles )
    {
#if MEASURE_PERFORMANCE
      DateTime methodStart = DateTime.Now;
#endif
      string strPath;
      SeparateItemName( workbookItemName, out strPath );

      WorkbookObjectsCollection sheets = m_book.Objects;
      ReserveSheetRelations( sheets, relations );

        for (int i = 0, len = sheets.Count; i < len; i++)
      {
            WorksheetBaseImpl sheet = (WorksheetBaseImpl) sheets[i];
        // TODO: re-use existing name.
            string strFileNameFormat = (sheet is WorksheetImpl)
                                            ? DefaultWorksheetPathFormat
                                            : DefaultChartsheetPathFormat;

            string strItemName = string.Format(strFileNameFormat, sheet.Index + 1);
            
            SaveSheet(sheet, strItemName, relations, strPath, hashNewXFIndexes, cacheFiles);
      }
#if MEASURE_PERFORMANCE
      DateTime methodEnd = DateTime.Now;
      Console.WriteLine( "SaveSheets() took: {0}", methodEnd - methodStart );
#endif
    }
    /// <summary>
    /// Reserves ids for worksheets that were extracted from original file.
    /// </summary>
    /// <param name="sheets">Worksheets to iterate through.</param>
    /// <param name="relations">Collection to put reservation into.</param>
    private void ReserveSheetRelations( WorkbookObjectsCollection sheets, RelationCollection relations )
    {
      // Reserve sheet ids
      for( int i = 0, len = sheets.Count; i < len; i++ )
      {
        WorksheetBaseImpl sheet = ( WorksheetBaseImpl )sheets[ i ];
        WorksheetDataHolder dataHolder = sheet.DataHolder;

        if( dataHolder != null )
        {
          string relationId = dataHolder.RelationId;

          if( relationId != null )
            relations[ relationId ] = null;
        }
      }
    }
    /// <summary>
    /// Saves single sheet into internal zip archive.
    /// </summary>
    /// <param name="sheet">Sheet to save.</param>
    /// <param name="itemName">Name of the sheet's item in the zip archive.</param>
    /// <param name="relations">Workbook relations collection.</param>
    /// <param name="workbookPath">Path to the workbook without file name.</param>
    /// <param name="hashNewXFIndexes">Dictionary with new XF indexes, key - old index, value - new index.</param>
    /// <param name="cacheFiles">Dictionary that will contain pivot cache files
    /// (key - cache object, value - cache file name).</param>
    private void SaveSheet( WorksheetBaseImpl sheet, string itemName,
      RelationCollection relations, string workbookPath,
      Dictionary<int, int> hashNewXFIndexes, Dictionary<PivotCacheImpl, string> cacheFiles )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( itemName == null || itemName.Length == 0 )
        throw new ArgumentOutOfRangeException( "itemName" );

      if( relations == null )
        throw new ArgumentNullException( "relations" );

      string strPartType = null;

      if( sheet is WorksheetImpl )
      {
        strPartType = Excel2007Serializator.WorksheetPartType;
        SaveWorksheet( ( WorksheetImpl )sheet, itemName, hashNewXFIndexes, cacheFiles );
      }
      else if( sheet is ChartImpl )
      {
        //if( sheet.m_dataHolder == null )
        //  return; // we don't support ChartImpl serialization as separate worksheet.

        strPartType = Excel2007Serializator.ChartSheetPartType;
        //itemName = sheet.m_dataHolder.ArchiveItem.ItemName;
        SaveChartsheet( ( ChartImpl )sheet, itemName );
      }

      // 2. Register in the workbook relations.
      // TODO: should we keep this code here or move it somewhere?
      if( itemName.StartsWith( workbookPath ) )
      {
        itemName = itemName.Substring( workbookPath.Length );
      }

      string strRelationId = sheet.m_dataHolder.RelationId;

      if( strRelationId == null )
      {
        strRelationId = relations.GenerateRelationId();
        sheet.m_dataHolder.RelationId = strRelationId;
      }

      Relation relation = new Relation( itemName, strPartType );
      relations[ strRelationId ] = relation;
    }
    /// <summary>
    /// Serializes single worksheet object.
    /// </summary>
    /// <param name="sheet">Worksheet to serialize.</param>
    /// <param name="itemName">Name of a zip item to use.</param>
    /// <param name="hashNewXFIndexes">Dictionary with new XF indexes, key - old index, value - new index.</param>
    /// <param name="cacheFiles">Dictionary that will contain pivot cache files
    /// (key - cache object, value - cache file name).</param>
    private void SaveWorksheet( WorksheetImpl sheet, string itemName,
      Dictionary<int, int> hashNewXFIndexes, Dictionary<PivotCacheImpl, string> cacheFiles )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( itemName == null || itemName.Length == 0 )
        throw new ArgumentOutOfRangeException( "itemName" );

      // 1. Register in the content overrides.
      string strFullItemName = itemName;

      if( itemName[ 0 ] != '/' )
        strFullItemName = '/' + itemName;

      m_dicOverriddenTypes[ strFullItemName ] = ContentTypes.Worksheet;

      if( sheet.IsSaved && sheet.m_dataHolder != null )
      {
        SerializeExistingData( sheet, itemName );
      }
      else
      {
          UpdateArchiveItem(sheet, itemName);

          sheet.m_dataHolder.SerializeWorksheet( sheet, hashNewXFIndexes, cacheFiles );
      }
    }
    /// <summary>
    /// Updates archive item
    /// </summary>
    /// <param name="sheet">Worksheet to add</param>
    /// <param name="itemName">Worksheet name to add</param>
    private void UpdateArchiveItem(WorksheetImpl sheet, string itemName)
    {
        // TODO: we should remove items somewhere else.
        bool bNewHolder = sheet.m_dataHolder == null;

        if( bNewHolder )
        {
          m_archive.RemoveItem( itemName );
          ZipArchiveItem item = m_archive.AddItem( itemName, null, false, FileAttributes.Archive );
          sheet.m_dataHolder = new WorksheetDataHolder( this, item );
        }
        else
        {
          ZipArchiveItem item = sheet.m_dataHolder.ArchiveItem;

          if( item == null || item.ItemName != itemName )
          {
            //item.Remove();
            //item.Dispose();
            if( m_archive.Find( itemName ) >= 0 )
            {
              m_archive.UpdateItem( itemName, null, false, FileAttributes.Archive );
            }
            else
            {
              m_archive.AddItem( itemName, null, false, FileAttributes.Archive );
            }

            sheet.m_dataHolder.ArchiveItem = m_archive[ itemName ];
          }
        }
      }
    /// <summary>
    /// Saves chartsheet into internal zip archive.
    /// </summary>
    /// <param name="chart">Chartsheet to save.</param>
    /// <param name="itemName">Name of a zip item to use.</param>
    private void SaveChartsheet( ChartImpl chart, string itemName )
    {
      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( itemName == null || itemName.Length == 0 )
        throw new ArgumentOutOfRangeException( "itemName" );

      // 1. Register in the content overrides.
      string strFullItemName = itemName;

      if( itemName[ 0 ] != '/' )
        strFullItemName = '/' + itemName;

      m_dicOverriddenTypes[ strFullItemName ] = ContentTypes.Chartsheet;

      if( chart.IsSaved && chart.m_dataHolder != null )
      {
        // we should reuse existing item.
        SerializeExistingData( chart, itemName );
      }
      else
      {
        // TODO: we should remove items somewhere else.
        bool bNewHolder = chart.m_dataHolder == null;

        if( bNewHolder )
        {
          m_archive.RemoveItem( itemName );
          ZipArchiveItem item = m_archive.AddItem( itemName, null, false, FileAttributes.Archive );
          chart.m_dataHolder = new WorksheetDataHolder( this, item );
        }

        chart.m_dataHolder.SerializeChartsheet( chart );
      }
    }
    /// <summary>
    /// Serializes existing worksheet/chartsheet data without any modifications.
    /// </summary>
    /// <param name="sheet">Sheet to serialize.</param>
    /// <param name="itemName">New corresponding zip archive item name.</param>
    private void SerializeExistingData( WorksheetBaseImpl sheet, string itemName )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( itemName == null || itemName.Length == 0 )
        throw new ArgumentOutOfRangeException( "itemName" );

      WorksheetDataHolder holder = sheet.m_dataHolder;

      if( holder == null )
        throw new ApplicationException( "Cannot serialize sheet " + sheet.Name );

      ZipArchiveItem item = holder.ArchiveItem;

      if( item.ItemName != itemName )
      {
        m_archive.RemoveItem( item.ItemName );
        item.ItemName = itemName;
        m_archive.AddItem( item );

        // TODO: don't forget about relations renaming and serialization.
      }
    }
    /// <summary>
    /// This method removes calculation chain item from the document.
    /// </summary>
    private void RemoveCalcChain()
    {
      string strRelationId;
      Relation relation = m_workbookRelations.FindRelationByContentType( RelationTypes.CalcChain, out strRelationId );

      if( relation != null )
      {
        string strBookPath;
        SeparateItemName( m_strWorkbookPartName, out strBookPath );
        string strItemName = strBookPath + relation.Target;
        m_archive.RemoveItem( strItemName );
        m_workbookRelations.Remove(strRelationId);
        m_dicOverriddenTypes.Remove( strItemName );
      }
    }
    internal void RemoveRelation(string strItemName,string relationID)
    {
        ItemsToRemove.Add(strItemName, null);
        m_workbookRelations.Remove(relationID);
    }
    /// <summary>
    /// Serializes single external link.
    /// </summary>
    /// <param name="externBook">Extern workbook item that contains link information to serialize.</param>
    /// <returns>Name of the external link item.</returns>
    public string SerializeExternalLink( ExternWorkbookImpl externBook )
    {
      MemoryStream stream = new MemoryStream();
      StreamWriter streamWriter = new StreamWriter( stream );
      XmlWriter writer = UtilityMethods.CreateWriter( streamWriter );
      RelationCollection relations = Serializator.SerializeLinkItem( writer, externBook );
      writer.Flush();
      streamWriter.Flush();

      string strExternalLinkItem = GenerateExternalLinkName();
      m_archive.UpdateItem( strExternalLinkItem, stream, true, FileAttributes.Archive );
      SaveRelations( strExternalLinkItem, relations );
      m_dicOverriddenTypes[ '/' + strExternalLinkItem ] = ContentTypes.ExternLink;

      return strExternalLinkItem;
    }
    /// <summary>
    /// Generates zip archive item name for external link.
    /// </summary>
    /// <returns>Generated item name.</returns>
    private string GenerateExternalLinkName()
    {
      return GenerateItemName( ref m_iExternLinkIndex, ExtenalLinksPathStart, XmlExtension );
    }
    /// <summary>
    /// Generates unique item name.
    /// </summary>
    /// <param name="itemsCount">Items counter. Contains current item index to try and is updated during operation.</param>
    /// <param name="pathStart">Starting part of the path.</param>
    /// <param name="extension">Item's extension.</param>
    /// <returns>Generated name.</returns>
    private string GenerateItemName( ref int itemsCount, string pathStart, string extension )
    {
      string strFormat = pathStart + "{0}." + extension;
      return string.Format(strFormat, ++itemsCount);
    }
    /// <summary>
    /// Generates unique item name.
    /// </summary>
    /// <param name="itemsCount">Items counter. Contains current item index to try and is updated during operation.</param>
    /// <param name="pathFormat">Path format string.</param>
    /// <returns>Generated name.</returns>
    private string GenerateItemName( ref int itemsCount, string pathFormat )
    {
      string strItemName = null;

      do
      {
        itemsCount++;
        strItemName = string.Format( pathFormat, itemsCount );
      }
      while( m_archive.Find( strItemName ) >= 0 );

      return strItemName;
    }
    private string GenerateQueryItemName(ref int itemsCount, string pathFormat)
    {
        string strItemName = null;

        do
        {
            itemsCount++;
            strItemName = string.Format(pathFormat, itemsCount);
        }
        while (m_archive.Find(strItemName) >= 0);

        return strItemName;
    }
    /// <summary>
    /// Genreates unique name for pivot table item.
    /// </summary>
    /// <returns>Generated name.</returns>
    internal string GeneratePivotTableName(int lastIndex)
    {
      int itemsCount = 0;
      return string.Format(PivotTablePathFormat, ++lastIndex);
    }
    /// <summary>
    /// Creates data holder for the specified worksheet.
    /// </summary>
    /// <param name="tabSheet">Tabsheet to create data holder for.</param>
    /// <param name="fileName">File name for the tabsheet item.</param>
    internal void CreateDataHolder( WorksheetBaseImpl tabSheet, string fileName )
    {
      if( tabSheet == null )
        throw new ArgumentNullException( "tabSheet" );

      if( fileName == null || fileName.Length == 0 )
        throw new ArgumentOutOfRangeException( "fileName" );

      if( fileName[ 0 ] == '/' )
        fileName = UtilityMethods.RemoveFirstCharUnsafe( fileName );

      int iItemIndex = m_archive.Find( fileName );
      ZipArchiveItem item = iItemIndex >= 0 ?
        m_archive[ iItemIndex ] :
        m_archive.AddItem( fileName, null, false, FileAttributes.Archive );

      tabSheet.DataHolder = new WorksheetDataHolder( this, item );
    }
    /// <summary>
    /// Serializes table object.
    /// </summary>
    /// <param name="listObject">Table to serialize.</param>
    /// <returns>Name of the created file.</returns>
    internal string SerializeTable( IListObject listObject )
    {
      MemoryStream stream = new MemoryStream();
      XmlWriter writer = UtilityMethods.CreateWriter( stream, Encoding.UTF8 );
      TableSerializator serializator = new TableSerializator();
      serializator.Serialize( writer, listObject );
      writer.Flush();
      stream.Flush();
      
      string itemName = GenerateTableFileName();
      
      if (listObject.QueryTable != null)
      {
          QueryTableImpl QueryTable = listObject.QueryTable as QueryTableImpl;
          string queryItemName = string.Format(m_queryTablePartName, m_queryTableCount);          
          SerializeQueryTable(listObject, queryItemName,itemName);
          m_queryTableCount++;
          if (QueryTable.ConnectionDeleted && checkconnection(QueryTable.ExternalConnection.ConncetionId))
              m_book.DeletedConnections.Add(QueryTable.ExternalConnection);
          //string RelationName = itemName + ".rels";
          //Console.Write(RelationName);
          //RelationCollection QueryTable = new RelationCollection();
          //QueryTable.ItemPath = RelationName;
          //QueryTable.ItemPath.ToString();
      }
      OverriddenContentTypes[ '/' + itemName ] = ContentTypes.Table;

      m_archive.UpdateItem( itemName, stream, true, FileAttributes.Archive );
      return itemName;
    }
    private bool checkconnection(uint id)
    {
        ExternalConnectionCollection connections=m_book.DeletedConnections as ExternalConnectionCollection;
        for (int i = 0; i < connections.Count; i++)
            if (id == connections[i].ConncetionId)
                return false;
            return true;
    }
    /// <summary>
    /// Generates file name for the table item.
    /// </summary>
    /// <returns>Generated item name.</returns>
    private string GenerateTableFileName()
    {
      int iCounter = 0;
      return GenerateItemName( ref iCounter, TablePathFormat );
    }
    private string GenerateQueryTableFileName()
    {
        int iCounter = 0;
        return GenerateQueryItemName(ref iCounter, QueryTablePathFormat);
    }
    /// <summary>
    /// Gets content type by item name.
    /// </summary>
    /// <param name="strTarget"></param>
    /// <returns></returns>
    public string GetContentType( string strTarget )
    {
      string result;

      if( !m_dicOverriddenTypes.TryGetValue( strTarget, out result ) )
      {
        strTarget = UtilityMethods.RemoveFirstCharUnsafe( Path.GetExtension( strTarget ) );
        result = m_dicDefaultTypes[ strTarget ];
      }

      return result;
    }
    public void Connections()
    {
        if ((m_book.Connections != null && m_book.Connections.Count>0)||(m_book.DeletedConnections!=null && m_book.DeletedConnections.Count>0))
        {
            AddSlashPreprocessor preprocessor = new AddSlashPreprocessor();
            m_dicOverriddenTypes[preprocessor.PreprocessName(m_connectionPartName)] = ContentTypes.Connections;
            
            MemoryStream stream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(stream);
            XmlWriter writer = UtilityMethods.CreateWriter(streamWriter/*, Encoding.UTF8*/ );
            Serializator.SerializeConnections(writer);
            writer.Flush();

            string strItemName = m_connectionPartName;

            if (m_connectionPartName[0] == '/')
                strItemName = UtilityMethods.RemoveFirstCharUnsafe(m_connectionPartName);

            m_archive.UpdateItem(strItemName, stream, true, FileAttributes.Archive);
        }
    }
    public void SerializeQueryTable(IListObject listobject, string itemName,string tablerels)
    {
        AddSlashPreprocessor preprocessor = new AddSlashPreprocessor();
        m_dicOverriddenTypes[preprocessor.PreprocessName(itemName)] = ContentTypes.QueryTable;

        MemoryStream stream = new MemoryStream();
        StreamWriter streamWriter = new StreamWriter(stream);
        XmlWriter writer = UtilityMethods.CreateWriter(streamWriter/*, Encoding.UTF8*/ );
        TableSerializator Table = new TableSerializator();
        Table.SerializeQueryTable(listobject, writer);
        writer.Flush();
        
      // NameImpl queryname= m_book.Names.Add(QueryTable.QueryTable.Name) as NameImpl;
       // AdddefinedName(listobject);
        //WorksheetImpl sheet = m_book.Objects[listobject.Location.Worksheet.Index] as WorksheetImpl;
        //sheet.Names.Add(listobject.QueryTable.Name);
        //queryname.Record.IndexOrGlobal = 1;
        string strItemName = itemName;

        if (itemName[0] == '/')
            strItemName = UtilityMethods.RemoveFirstCharUnsafe(itemName);
       
        m_archive.UpdateItem(strItemName, stream, true, FileAttributes.Archive);
        SerializeTableRelation(tablerels,itemName);
        
    }
    public void SerializeTableRelation(string ItemName,string queryTable)
    {
        string strItemName = ItemName;
        int iSlashIndex = strItemName.LastIndexOf('/');
        string strRelationItem = strItemName.Insert(iSlashIndex, '/' +
          FileDataHolder.RelationsDirectory) + FileDataHolder.RelationExtension;
        MemoryStream memStream = new MemoryStream();
        StreamWriter streamWriter = new StreamWriter(memStream);
        XmlWriter writer = UtilityMethods.CreateWriter(streamWriter);
        Excel2007Serializator ser = new Excel2007Serializator(m_book);
        RelationCollection rel = new RelationCollection();
        Relation querttable = new Relation(queryTable, RelationTypes.QueryTable);
        rel.Add(querttable);
        ser.SerializeRelations(writer, rel, null);
        writer.Flush();
        streamWriter.Flush();

        m_archive.UpdateItem(strRelationItem, memStream, true, FileAttributes.Archive);
    }
    /*public void AdddefinedName(IListObject listobject)
    {  
        string  strName = listobject.QueryTable.Name; 
        bool bLocal = true;                 
        WorksheetImpl sheet = m_book.Objects[ listobject.Location.Worksheet.Index ] as WorksheetImpl;
        bool bAddLocal = bLocal || ( sheet != null );
        IName name;
        name = sheet.Names.Add( strName );        
        string strValue = listobject.Location.AddressGlobal;
        
        m_book.HasApostrophe = strValue.Contains("'") ? true : false;
        NameImpl nameImpl1 = ( NameImpl )name;      
        if( bLocal )
        {
          nameImpl1.Record.IndexOrGlobal = ( ushort )( listobject.Location.Worksheet.Index + 1 );
        }
        INames names = m_book.Names;
        m_book.Names.Add(listobject.Location.AddressLocal);
        m_book.AppImplementation.IsFormulaParsed = false;
        NameImpl nameImpl = ( NameImpl )names[0];
        FormulaUtil util = m_book.FormulaUtil;
        nameImpl.SetValue(util.ParseString(strValue));
        //names[ i ].Value = arrValues[ i ];    
    }*/
    #endregion

    #region IWorkbookSerializator Members
#if !(WINRT )
    /// <summary>
    /// Saves workbook into specified file.
    /// </summary>
    /// <param name="fullName">Destination file name.</param>
    /// <param name="book">Workbook to save.</param>
    /// <param name="saveType">Save type.</param>
    public void Serialize( string fullName, WorkbookImpl book, Syncfusion.XlsIO.ExcelSaveType saveType )
    {
      if( book != m_book )
        throw new ArgumentOutOfRangeException( "book" );

      SaveDocument( fullName, saveType );
    }
#endif
    /// <summary>
    /// Saves workbook into stream.
    /// </summary>
    /// <param name="stream">Stream to save into.</param>
    /// <param name="book">Workbook to save.</param>
    /// <param name="saveType">Save type (template or ordinary xls).</param>
    public void Serialize(Stream stream, WorkbookImpl book, Syncfusion.XlsIO.ExcelSaveType saveType)
    {
      if( book != m_book )
        throw new ArgumentOutOfRangeException( "book" );

      SaveDocument( stream, saveType );
    }
    /// <summary>
    /// Creates copy of the current object.
    /// </summary>
    /// <param name="newParent">Parent workbook for the new object.</param>
    /// <returns>A copy of the current object.</returns>
    internal FileDataHolder Clone( WorkbookImpl newParent )
    {
      FileDataHolder result = ( FileDataHolder )MemberwiseClone();
      result.m_book = newParent;
      result.m_parser = null;
      result.m_serializator = null;
      result.m_workbookRelations = m_workbookRelations.Clone();
      result.m_topRelations = m_topRelations.Clone();
      result.m_arrImageItemNames = CloneUtils.CloneStringArray( m_arrImageItemNames );
      result.m_streamEnd = CloneUtils.CloneStream( m_streamEnd );
      result.m_streamStart = CloneUtils.CloneStream( m_streamStart );
      result.m_streamDxfs = CloneUtils.CloneStream( m_streamDxfs );
      result.m_functionGroups = CloneUtils.CloneStream( m_functionGroups );

      if( m_dictItemsToRemove != null )
        result.m_dictItemsToRemove = new Dictionary<string, object>( m_dictItemsToRemove );

      if( m_dicDefaultTypes != null )
        result.m_dicDefaultTypes = new Dictionary<string, string>( m_dicDefaultTypes );

      if( m_dicOverriddenTypes != null )
        result.m_dicOverriddenTypes = new Dictionary<string, string>( m_dicOverriddenTypes );

      if( m_arrCellFormats != null )
        result.m_arrCellFormats = new List<int>( m_arrCellFormats );

      result.m_lstParsedDxfs = CloneDxfs();
      result.m_lstBookViews = CloneViews();
      result.m_archive = m_archive.Clone();

     return result;
    }
    /// <summary>
    /// Creates copy of the workbook's views.
    /// </summary>
    /// <returns>List with cloned items.</returns>
    private List<Dictionary<string, string>> CloneViews()
    {
      List<Dictionary<string, string>> result;

      if( m_lstBookViews != null )
      {
        int iCount = m_lstBookViews.Count;
        result = new List<Dictionary<string, string>>( iCount );

        for( int i = 0; i < iCount; i++ )
        {
          Dictionary<string, string> item = m_lstBookViews[ i ];
          result.Add( new Dictionary<string, string>( item ) );
        }
      }
      else
      {
        result = null;
      }

      return result;
    }
    /// <summary>
    /// Creates copy of the parsed Dxf items.
    /// </summary>
    /// <returns>List with copied items.</returns>
    private List<DxfImpl> CloneDxfs()
    {
      List<DxfImpl> result;

      if( m_lstParsedDxfs != null )
      {
        int iCount = m_lstParsedDxfs.Count;
        result = new List<DxfImpl>( iCount );

        for( int i = 0; i < iCount; i++ )
        {
          DxfImpl item = m_lstParsedDxfs[ i ];
          result.Add( item.Clone( m_book ) );
        }
      }
      else
      {
        result = null;
      }

      return result;
    }
    internal void RegisterCache( string cacheId, string relationId )
    {
      m_preservedCaches.Add( cacheId, relationId );
    }
    #endregion
    #region IDisposable Members

    public void Dispose()
    {
        if (m_serializator != null)
        {
            m_serializator.Dispose();
            m_serializator = null;
        }
        if (m_archive != null)
        {
            m_archive.Dispose();
            m_archive = null;
        }
        m_functionGroups=null;
        m_extensions=null;
        m_streamDxfs=null;
        m_streamStart=null;
        m_streamEnd = null;
        GC.SuppressFinalize(this);
    }

    #endregion
  }
}

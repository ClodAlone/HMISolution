#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections.Generic;
using System.IO;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Implementation.Security;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.PivotTable;
using Syncfusion.XlsIO.Implementation.XmlSerialization;

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
  /// <summary>
  /// Summary description for PivotCacheImpl.
  /// </summary>
  public class PivotCacheImpl
    : CommonObject
    , ICloneParent
    , IBiffStorage
    , IPivotCache
  {


      #region Class members
      /// <summary>
    /// Main cache record.
    /// </summary>
    private CacheDataRecord m_cacheData = ( CacheDataRecord )
      BiffRecordFactory.GetRecord( TBIFFRecord.CacheData );
    /// <summary>
    /// Cache data extended record.
    /// </summary>
    private CacheDataExRecord m_cacheDataEx = ( CacheDataExRecord )
      BiffRecordFactory.GetRecord( TBIFFRecord.CacheDataEx );
    /// <summary>
    /// Array of all unparsed cache records.
    /// </summary>
    private List<BiffRecordRaw> m_arrRecords = new List<BiffRecordRaw>();
    /// <summary>
    /// Stream with preserved data.
    /// </summary>
    private MemoryStream m_preservedData = new MemoryStream();
    /// <summary>
    /// List with cached fields.
    /// </summary>
    //private List<PivotCacheFieldImpl> m_lstCacheFields = new List<PivotCacheFieldImpl>();
    private PivotCacheFieldsCollection m_lstCacheFields = new PivotCacheFieldsCollection();
    /// <summary>
    /// Indixes to the pivto values. Each item contains indexes to the pivot cache field data for corresponding row.
    /// </summary>
    private List<PivotIndexListRecord> m_lstPivotIndexes = new List<PivotIndexListRecord>();
    /// <summary>
    /// Contains source range.
    /// </summary>
    private IRange m_sourceRange;
      /// <summary>
      /// Contains the name of the pivot cache NamedRange
      /// </summary>
    private string m_rangeName;
    /// <summary>
    /// Cache index.
    /// </summary>
    private int m_iIndex = -1;
    /// <summary>
    /// Contains additional information regarding pivot cache.
    /// </summary>
    private PivotCacheInfo m_info;
    /// <summary>
    /// Specifies whether the cache's data source supports attribute drilldown.
    /// </summary>
    private bool m_bsupportAdvancedDrill;
    /// <summary>
    /// Specifies the version of the application that created the cache. This attribute is
    /// application-dependent.
    /// </summary>
    private int m_iCreatedVersion;
    /// <summary>
    /// Specifies the earliest version of the application that is required to refresh the cache. 
    /// </summary>
    private int m_iMinRefreshableVersion;
    /// <summary>
    /// Specifies the version of the application that last refreshed the cache. 
    /// </summary>
    private int m_iRefreshedVersion;
      /// <summary>
    /// Specifies whether the cache's data source supports subqueries
      /// </summary>
    private bool m_bSupportSubQuery;
      /// <summary>
      /// Specifies a boolean value that indicates whether the cache is scheduled for version
      /// upgrade.
      /// </summary>
    private bool m_bUpgradeOnRefresh;
      /// <summary>
      /// Preserved XlsIO unsupported elements
      /// </summary>
    private Dictionary<string, Stream> m_preservedElements;
      /// <summary>
      /// Presreved the Extenal cache source relations
      /// </summary>
    private Relation m_preservedExtenalRelation;
      /// <summary>
      /// External cache source relation ID.
      /// </summary>
    private string m_relationId;
    /// <summary>
    /// Indicates wheather the pivot cache has records.
    /// </summary>
    private bool m_bHasCacheRecords;
    /// <summary>
    /// Preserved cache relations of Consolidation source.
    /// </summary>
    public  RelationCollection preservedCacheRelations;
    /// <summary>
    /// Stream which preserves the consolidation source of the pivot table.
    /// </summary>
    private Stream m_consolidation;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Main class constructor. Application and Parent properties are set.
    /// </summary>
    /// <param name="application">Reference to Application instance.</param>
    /// <param name="parent">
    /// Reference to the Parent object which will host this object
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    /// If specified application or parent is null.
    /// </exception>
    public PivotCacheImpl( IApplication application, object parent )
      : base( application, parent )
    {
    }
    /// <summary>
    /// Main class constructor. Application and Parent properties are set.
    /// </summary>
    /// <param name="application">Reference to Application instance.</param>
    /// <param name="parent">
    /// Reference to the Parent object which will host this object.
    /// </param>
    /// <param name="reader">Reader to get pivot cache records from.</param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If at least one of arguments is Null.
    /// </exception>
    [ CLSCompliant( false ) ]
    public PivotCacheImpl( IApplication application, object parent, BiffReader reader, IDecryptor decryptor, string streamName )
      : this( application, parent )
    {
      Parse( reader, decryptor, streamName );
    }
    /// <summary>
    /// Initializes new instance of the cache.
    /// </summary>
    /// <param name="application">Reference to Application instance.</param>
    /// <param name="parent">
    /// Reference to the Parent object which will host this object.
    /// </param>
    /// <param name="dataRange">Range object containing cached data.</param>
    public PivotCacheImpl( IApplication application, object parent, IRange dataRange )
      : base( application, parent )
    {
      m_cacheData.SourceType = ExcelDataSourceType.Worksheet;
      int iRow = dataRange.Row;
      int iLastRow = dataRange.LastRow;
      int iRowCount = iLastRow - iRow; // we don't add 1 because first cell contains field name.
      int iColumn = dataRange.Column;
      int iLastColumn = dataRange.LastColumn;
      int iColumnCount = iLastColumn - iColumn + 1;
      int[][] arrValueIndexes = new int[ iColumnCount ][];
      preservedCacheRelations = new RelationCollection();
      for( int i = iColumn, iCurrentItem = 0; i <= iLastColumn; i++, iCurrentItem++ )
      {
         CreateField( dataRange.Worksheet, iRow, iLastRow, i );
      }

     
      RefreshDate = DateTime.Now;
      byte version = 0;
      CreatedVersion = MinRefreshableVersion = RefreshedVersion = version;
      m_sourceRange = dataRange;
      SourceType = ExcelDataSourceType.Worksheet;
      //HasCacheRecords = true;
    }
    /// <summary>
    /// Creates cached field.
    /// </summary>
    /// <param name="sheet">Worksheet to create cache for.</param>
    /// <param name="row">First row to get data from.</param>
    /// <param name="lastRow">Last row of the field.</param>
    /// <param name="column">Column index.</param>
    private void CreateField(IWorksheet sheet, int row, int lastRow, int column)
    {
        PivotCacheFieldImpl field = m_lstCacheFields.AddNewField(sheet[row, column].Value);
        field.ItemRange = sheet[row + 1, column, lastRow, column];
        field.Fill(sheet, row, lastRow, column);
    }
    public int AddIndexes(byte[] indexes)
    {
        PivotIndexListRecord indexList = new PivotIndexListRecord();
        indexList.Indexes = indexes;
        m_lstPivotIndexes.Add(indexList);
        return m_lstPivotIndexes.Count - 1;
    }
    /// <summary>
    /// Returns specified field value.
    /// </summary>
    /// <param name="fieldIndex">Field index.</param>
    /// <param name="row">Row index.</param>
    /// <returns>Extracted value.</returns>
    public object GetValue( int fieldIndex, int row )
    {
      int iValueIndex = m_lstPivotIndexes[ row ].Indexes[ fieldIndex ];
      PivotCacheFieldImpl field = m_lstCacheFields[ fieldIndex ];
      return field.GetValue( iValueIndex );
    }
    public byte PutValue(int fieldIndex, object value)
    {
        PivotCacheFieldImpl field = m_lstCacheFields[fieldIndex];
        return (byte)field.AddValue(value);
    }
   
    #endregion

    #region Methods
    /// <summary>
    /// Parses pivot cache.
    /// </summary>
    /// <param name="data">Records with pivot table cache.</param>
    /// <param name="iPos">Offset to the first pivot cache record.</param>
    /// <returns>Offset to the record after cache records.</returns>
    private int Parse( BiffRecordRaw[] data, int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      if( iPos < 0 || iPos > data.Length - 1 )
        throw new ArgumentOutOfRangeException( "iPos", "Value cannot be less than 0 and greater than data.Length - 1" );

      BiffRecordRaw record = data[ iPos ];
      iPos++;

      record.CheckTypeCode( TBIFFRecord.CacheData );
      record = data[ iPos ];
      iPos++;

      while( record.TypeCode != TBIFFRecord.EOF )
      {
        m_arrRecords.Add( record );
        record = data[ iPos ];
        iPos++;
      }

      return iPos;
    }
    /// <summary>
    /// Parses data from BiffReader.
    /// </summary>
    /// <param name="reader">BiffReader with data to parse.</param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    /// <param name="streamCode">Represent the pivot cache stream code 
    /// (helps to preserve unknown pivot cache streams)</param>
    private void Parse( BiffReader reader, IDecryptor decryptor, string streamCode )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      //Stream stream = reader.BaseStream;
      //const int BufferSize = 32768;
      //byte[] arrBuffer = new byte[ BufferSize ];
      //int iReadSize = 0;

      //long lStartPosition = stream.Position;
      //while( ( iReadSize = stream.Read( arrBuffer, 0, BufferSize ) ) != 0 )
      //{
      //  m_preservedData.Write( arrBuffer, 0, iReadSize );
      //}

      //stream.Position = lStartPosition;
      ///if the record is empty, that might be the 0xD5 record (SXStreamID)
      ///TODO: support the 0xD5 record
      if (reader.BaseStream.Length == 0)
      {
          int code = 0;
          int.TryParse(streamCode, System.Globalization.NumberStyles.AllowHexSpecifier
              , null, out code);
          ushort streamId = (int)TBIFFRecord.StreamId;
          if (code == streamId)
          {
              m_cacheData.StreamId = streamId;
              return;
          }
      }
      if (reader.BaseStream.Length > 0)
      {
          TBIFFRecord recordType = reader.PeekRecordType();

          if (recordType != TBIFFRecord.CacheData)
              throw new UnexpectedRecordException(recordType);

          m_cacheData = (CacheDataRecord)reader.GetRecord(decryptor);
          BiffRecordRaw raw = m_cacheData;

          //Dictionary<TBIFFRecord, object> dictRecords = new Dictionary<TBIFFRecord, object>();

          while (raw.TypeCode != TBIFFRecord.EOF)
          {
              raw = reader.GetRecord(decryptor);
              m_arrRecords.Add(raw);

              //switch( raw.TypeCode )
              //{
              //  case TBIFFRecord.PivotField:
              //    //new PivotCacheFieldImpl( IApplication
              //    break;

              //  case TBIFFRecord.PivotIndexList:
              //    break;
              //}
              //dictRecords[ raw.TypeCode ] = null;
          }
      }
      //foreach( TBIFFRecord code in dictRecords.Keys )
      //{
      //  Debug.WriteLine( code, "Pivot cache record" );
      //}
    }
    /// <summary>
    /// Saves pivot cache into OffsetArrayList.
    /// </summary>
    /// <param name="records">OffsetArrayList that will get all pivot cache records.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      records.Add( m_cacheData );
      records.AddList( m_arrRecords );
      //records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.EOF ) );

      //records.Add( this );
    }
    /// <summary>
    /// Saves pivot cache into stream using specified encryptor.
    /// </summary>
    /// <param name="stream">Stream to save into.</param>
    /// <param name="encryptor">Encryptor to use.</param>
    public void Serialize( Stream stream, IEncryptor encryptor )
    {
      if( m_preservedData != null && m_preservedData.Length > 0 )
      {
        m_preservedData.WriteTo( stream );
      }
      else if( m_arrRecords != null && m_arrRecords.Count > 0 )
      {
        OffsetArrayList records = new OffsetArrayList();
        Serialize( records );
        using( BiffWriter writer = new BiffWriter( stream, false ) )
        {
          writer.WriteRecord( records, encryptor );
        }
      }
    }
    /// <summary>
    /// Update pivot cache after row/column insert operation.
    /// </summary>
    /// <param name="worksheet"></param>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="isRow"></param>
    public void UpdateAfterInsertRemove( WorksheetImpl worksheet, int index, int count,
      bool isRow, bool isRemove )
    {
      if( m_sourceRange != null && m_sourceRange.Worksheet == worksheet )
      {
        if( isRemove )
        {
          RemoveRowColumn( worksheet, index, count, isRow );
        }
        else
        {
          InsertRowColumn( worksheet, index, count, isRow );
        }
      }
    }

    private void RemoveRowColumn( WorksheetImpl worksheet, int index, int count, bool isRow )
    {
      int iRow = m_sourceRange.Row;
      int iColumn = m_sourceRange.Column;
      int iLastRow = m_sourceRange.LastRow;
      int iLastColumn = m_sourceRange.LastColumn;

      if( InRange( m_sourceRange, worksheet, index, count, isRow ) )
      {
        if( isRow )
        {
          iLastRow = Math.Max( iLastRow - count, index - 1 );
        }
        else
        {
          iLastColumn = Math.Max( iLastColumn - count, index - 1 );
        }
      }
      else if( isRow && index <= iRow )
      {
        iRow = Math.Max( index, iRow - count );
        iLastRow -= count;
      }
      else if( !isRow && index <= iColumn )
      {
        iColumn = Math.Max( index, iColumn - count );
        iLastColumn -= count;
      }

      m_sourceRange = worksheet[ iRow, iColumn, iLastRow, iLastColumn ];
    }

    private void InsertRowColumn( WorksheetImpl worksheet, int index, int count, bool isRow )
    {
      if( InRange( m_sourceRange, worksheet, index, count, isRow ) )
      {
        int iRow = m_sourceRange.Row;
        int iColumn = m_sourceRange.Column;
        int iLastRow = m_sourceRange.LastRow;
        int iLastColumn = m_sourceRange.LastColumn;
        if (isRow)
        {
            m_sourceRange= worksheet[iRow, iColumn, iLastRow + count, iLastColumn];
        }
        else
        {
            m_sourceRange = worksheet[iRow, iColumn, iLastRow, iLastColumn + count];
        }
        //m_sourceRange = (isRow) ?
        //  worksheet[iRow, iColumn, iLastRow + count, iLastColumn] :
        //  worksheet[iRow, iColumn, iLastRow, iLastColumn + count];
      }
    }
    /// <summary>
    /// Indicates whether specified insert row/column operation affected pivot cache in some way.
    /// </summary>
    /// <param name="m_sourceRange"></param>
    /// <param name="worksheet"></param>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="isRow"></param>
    /// <returns></returns>
    private static bool InRange( IRange sourceRange, WorksheetImpl worksheet, int index, int count, bool isRow )
    {
      bool result = sourceRange.Worksheet == worksheet;

      result = result & ( ( isRow ) ?
        sourceRange.Row < index && sourceRange.LastRow >= index :
        sourceRange.Column < index && sourceRange.LastColumn >= index );

      return result;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Stream id.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ushort StreamId
    {
      get
      {
        return m_cacheData.StreamId;
      }
      set
      {
        m_cacheData.StreamId = value;
      }
    }
    /// <summary>
    /// Data source is one of:
    /// 1 - Excel worksheet,
    /// 2 - external data,
    /// 4 - consolidation,
    /// 8 - scenario PivotTable.
    /// </summary>
    public ExcelDataSourceType SourceType
    {
        get
        {
            return m_cacheData.SourceType;
        }
        set
        {
            m_cacheData.SourceType = value;
        }
    }
      /// <summary>
      /// Specifies a boolean value that indicates whether the cache is scheduled for version
      /// upgrade.
      /// </summary>
    public bool IsUpgradeOnRefresh
    {
        get
        {
            return m_bUpgradeOnRefresh;
        }
        set
        {
            m_bUpgradeOnRefresh = value;
        }
    }
    public string RefreshedBy
    {
        get
        {
            return m_cacheData.UserName;

        }
        set
        {
            m_cacheData.UserName = value;
        }
    }
      /// <summary>
    /// Specifies whether the cache's data source supports subqueries
      /// </summary>
    public bool IsSupportSubQuery
    {
        get
        {
            return m_bSupportSubQuery;
        }
        set
        {
            m_bSupportSubQuery = value;
        }
    }
      /// <summary>
      /// Specifies a boolean value that indicates whether the pivot records are saved with the
    /// cache.
      /// </summary>
    public bool IsSaveData
    {
        get
        {
            return m_cacheData.IsSaveData;
        }
        set
        {
            m_cacheData.IsSaveData = value;
        }
    }
      /// <summary>
      /// Specifies a boolean value that indicates whether the application will apply optimizations
    /// to the cache to reduce memory usage
      /// </summary>
    public bool IsOptimizedCache
    {
        get
        {
            return m_cacheData.IsOptimizeCache;
        }
        set
        {
            m_cacheData.IsOptimizeCache = value;
        }
    }
      /// <summary>
      /// Specifies a boolean value that indicates whether the user can refresh the cache. 
      /// </summary>
    public bool EnableRefresh
    {
        get
        {
            return m_cacheData.IsEnableRefresh;
        }
        set
        {
            m_cacheData.IsEnableRefresh = false;
        }
    }

    /// <summary>
    /// Specifies a boolean value that indicates whether the application should query and
    ///    retrieve records asynchronously from the cache.
    /// </summary>
    public bool IsBackgroundQuery
    {
        get
        {
            return m_cacheData.IsBackgroundQuery;
        }
        set
        {
            m_cacheData.IsBackgroundQuery = value;
        }
    }
    /// <summary>
    /// Specifies the version of the application that created the cache. This attribute is
    /// application-dependent.
    /// </summary>
    public int CreatedVersion
    {
        get
        {
            return m_iCreatedVersion;
        }
        set
        {
            m_iCreatedVersion = value;
        }
    }
    /// <summary>
    /// Specifies the earliest version of the application that is required to refresh the cache. 
    /// </summary>
    public int MinRefreshableVersion
    {
        get
        {
            return m_iMinRefreshableVersion;
        }
        set
        {
            m_iMinRefreshableVersion = value;
        }
    }
    /// <summary>
    /// Specifies the version of the application that last refreshed the cache. This attribute
    ///depends on whether the application exposes mechanisms via the user interface whereby
    ///the end-user can refresh the cache.
    /// </summary>
    public int RefreshedVersion
    {
        get
        {
            return m_iRefreshedVersion;
        }
        set
        {
            m_iRefreshedVersion = value;
        }
    }
    /// <summary>
    /// Specifies a boolean value that indicates whether the cache needs to be refreshed.
    /// </summary>
    public bool IsInvalidData
    {
        get
        {
            return m_cacheData.IsInvalid;
        }
        set
        {
            m_cacheData.IsInvalid = value;
        }
    }
    /// <summary>
    /// Specifies whether the cache's data source supports attribute drilldown.
    /// </summary>
    public bool SupportAdvancedDrill
    {
        get
        {
            return m_bsupportAdvancedDrill;
        }
        set
        {
            m_bsupportAdvancedDrill = value;
        }
    }
    /// <summary>
    /// Specifies a boolean value that indicates whether the application will refresh the cache
    /// </summary>
    public bool IsRefreshOnLoad
    {
        get
        {
            return m_cacheData.IsRefreshOnLoad;
        }
        set
        {
            m_cacheData.IsRefreshOnLoad = value;
        }
    }
    /// <summary>
    /// Gets/sets refresh date of the cache.
    /// </summary>
    public DateTime RefreshDate
    {
      get
      {
        return 
#if !(WINRT )
            DateTime.FromOADate( m_cacheDataEx.RefreshDate );
#else
            DateTimeExtension.FromOADate(m_cacheDataEx.RefreshDate);
#endif
      }
      set
      {
        m_cacheDataEx.RefreshDate = value.ToOADate();
      }
    }
    /// <summary>
    /// Returns number of records inside this cache.
    /// </summary>
    public int RecordCount
    {
      get
      {
        return m_lstPivotIndexes.Count;
      }
    }
    
    /// <summary>
    /// Stream which preserves the consolidation source of the pivot table
    /// </summary>
    public Stream Consolidation
    {
        get
        {
            return m_consolidation;
        }
        set
        {
            if (value != null)
                m_consolidation = value;
        }
    }
    /// <summary>
    /// Returns the data source for the PivotTable report.
    /// </summary>
    public IRange SourceRange
    {
      get
      {
        return m_sourceRange;
      }
      set
      {
          m_sourceRange = value;
      }
    }
    /// <summary>
    /// Returns collection of cache fields. Read-only.
    /// </summary>
    public PivotCacheFieldsCollection CacheFields
    {
      get
      {
        return m_lstCacheFields;
      }
    }
    /// <summary>
    /// Gets/sets cache index.
    /// </summary>
    public int Index
    {
      get
      {
        return m_iIndex;
      }
      set
      {
        m_iIndex = value;
      }
    }
    /// <summary>
    /// Gets or sets additional information regarding pivot cache.
    /// </summary>
    internal PivotCacheInfo Info
    {
      get
      {
        return m_info;
      }
      set
      {
        m_info = value;
      }
    }

    /// <summary>
    /// Preserved XlsIO unsupported elements
    /// </summary>
    internal Dictionary<string, Stream> PreservedElements
    {
        get
        {
            if (m_preservedElements == null)
                m_preservedElements = new Dictionary<string, Stream>();
            return m_preservedElements;
        }
    }
    /// <summary>
    /// Contains the name of the pivot cache NamedRange
    /// </summary>
    public string RangeName
    {
        get
        {
            return m_rangeName;
        }
        set
        {
            m_rangeName = value;
        }
    }
      /// <summary>
      /// Indicates whether the pivot cache has named range
      /// </summary>
    public bool HasNamedRange
    {
        get
        {
            return m_rangeName != null;
        }
    }
    public int CalculatedItemIndex
    {
        get
        {
            return 1;
        }
    }
    /// <summary>
    /// Presreved the Extenal cache source relations
    /// </summary>
    internal Relation PreservedExtenalRelation
    {
        get
        {
            return m_preservedExtenalRelation;
        }
        set
        {
            m_preservedExtenalRelation = value;
        }
    }
    /// <summary>
    /// External cache source relation ID.
    /// </summary>
    internal string RelationId
    {
        get
        {
            return m_relationId;
        }
        set
        {
            m_relationId = value;
        }
    }
    /// <summary>
    /// Indicates wheather the pivot cache has records.
    /// </summary>
    internal bool HasCacheRecords
    {
        get
        {
            return m_bHasCacheRecords;
        }
        set
        {
            m_bHasCacheRecords = value;
        }
    }
    #endregion

    #region ICloneParent Members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone( object parent )
    {
      return Clone( parent, null );
    }
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone( object parent, Dictionary<string, string> hashNewNames )
    {
      PivotCacheImpl result = ( PivotCacheImpl )MemberwiseClone();
      result.SetParent( parent );

      result.m_cacheData = ( CacheDataRecord )CloneUtils.CloneCloneable( m_cacheData );
      result.m_cacheDataEx = ( CacheDataExRecord )CloneUtils.CloneCloneable( m_cacheDataEx );
      result.m_arrRecords = CloneUtils.CloneCloneable( m_arrRecords );
      result.m_info = ( PivotCacheInfo )CloneUtils.CloneCloneable( m_info );

      result.m_preservedData = new MemoryStream();
      m_preservedData.WriteTo( result.m_preservedData );

      if( m_sourceRange != null )
      {
        WorkbookImpl book = ( WorkbookImpl )result.FindParent( typeof( WorkbookImpl ) );
        string sourceSheetName = m_sourceRange.Worksheet.Name;

        if(
          /*hashNewNames.ContainsKey( sourceSheetName ) ||
          book.Worksheets[ sourceSheetName ] != null ||
          m_sourceRange is ExternalRange ||*/
          book == m_sourceRange.Worksheet.Workbook )
        {
          result.m_sourceRange = ( ( ICombinedRange )m_sourceRange ).Clone( parent, hashNewNames, book );
        }
        else
        {

          WorksheetImpl sourceSheet = m_sourceRange.Worksheet as WorksheetImpl;
          IWorksheets sheets = book.Worksheets;
          IWorksheet copy = sheets.AddCopy( sourceSheet, ExcelWorksheetCopyFlags.CopyCells );
          copy.Visibility = WorksheetVisibility.StrongHidden;
          Dictionary<string, string> hashNames = new Dictionary<string, string>();
          hashNames.Add( sourceSheet.Name, copy.Name );

          result.m_sourceRange = ( ( ICombinedRange )m_sourceRange ).Clone( parent, hashNames, book );
          //result.m_sourceRange = CreateExternalRange( parent, book, m_sourceRange );
        }
      }

      return result;
    }

    private IRange CreateExternalRange( object parent, WorkbookImpl book, IRange sourceRange )
    {
      WorkbookImpl sourceBook = ( WorkbookImpl )sourceRange.Worksheet.Workbook;
      string fileName = sourceBook.FullFileName;

      if( fileName == null )
        fileName = "Book1.xlsx";
      int iBookIndex;
#if ( WINRT )
       //TODO:WINRT Analyze and implement this. Need to use StorageFile.
      iBookIndex = 0;
#else
      iBookIndex = book.ExternWorkbooks.Add( fileName, book, sourceRange );
#endif
      ExternWorkbookImpl externBook = book.ExternWorkbooks[ iBookIndex ];
      ExternWorksheetImpl sheet = externBook.Worksheets[ sourceRange.Worksheet.Index ];
      return new ExternalRange( sheet, sourceRange.Row, sourceRange.Column,
        sourceRange.LastRow, sourceRange.LastColumn );//.Range[ sourceRange.AddressLocal ];
    }

    private IRange CreateInvalidRange( object parent, WorkbookImpl book, IRange sourceRange )
    {
      return new InvalidRange( parent, sourceRange );
    }

    public bool ComparePreservedData( PivotCacheImpl cache )
    {
      bool result = m_preservedData.Length == cache.m_preservedData.Length &&
        BiffRecordRaw.CompareArrays( m_preservedData.GetBuffer(), cache.m_preservedData.GetBuffer() );

      return result;
    }
    #endregion

    #region IBiffStorage Members
    /// <summary>
    /// Returns type code of the biff storage. Read-only.
    /// </summary>
    public TBIFFRecord TypeCode
    {
      get
      {
        return 0;
      }
    }
    /// <summary>
    /// Returns code of the biff storage. Read-only.
    /// </summary>
    public int RecordCode
    {
      get
      {
        return 0;
      }
    }
    /// <summary>
    /// Indicates whether data array is required by this record.
    /// </summary>
    public bool NeedDataArray
    {
      get
      {
        return true;
      }
    }
    /// <summary>
    /// Indicates record position in stream. This is a utility member of class and
    /// is used only in the serialization process. Does not influence the data.
    /// </summary>
    public long StreamPos
    {
      get
      {
        return 0;
      }
      set
      {
      }
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public int GetStoreSize( ExcelVersion version )
    {
      return ( int )m_preservedData.Length;
    }
    /// <summary>
    /// Save record data to stream.
    /// </summary>
    /// <param name="writer">Writer that will receive record data.</param>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="encryptor">Object to encrypt data.</param>
    /// <param name="streamPosition">Position in the output stream. Used to increase performance.</param>
    /// <returns>Size of the record.</returns>
    /// <exception cref="System.ArgumentNullException">If writer is NULL.</exception>
    /// <exception cref="System.ApplicationException">
    ///   If m_iLength of internal record data array is less than zero.
    /// </exception>
    public int FillStream( BinaryWriter writer, DataProvider provider, IEncryptor encryptor, int streamPosition )
    {
      Stream stream = writer.BaseStream;
      m_preservedData.WriteTo( stream );

      return ( int )m_preservedData.Length;
    }
    #endregion
  }
}

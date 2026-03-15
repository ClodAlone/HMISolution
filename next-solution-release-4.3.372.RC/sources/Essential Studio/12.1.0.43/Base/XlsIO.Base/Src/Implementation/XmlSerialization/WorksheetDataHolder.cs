#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Xml;
using System.IO;
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
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Charts;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes;
using Syncfusion.XlsIO.Implementation.PivotTables;
using System.Text;
using Syncfusion.XlsIO.Implementation.XmlReaders.PivotTables;
using Syncfusion.XlsIO.Implementation.XmlSerialization.PivotTables;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using Syncfusion.CompoundFile.XlsIO;
using Syncfusion.XlsIO.Implementation.XmlReaders.Shapes;
#if ( WINRT )
using Windows.Storage;
#endif

namespace Syncfusion.XlsIO.Implementation.XmlSerialization
{
  /// <summary>
  /// This class stores worksheet data extracted from document and is responsible
  /// for parsing and serialization of this data into special FileDataHolder.
  /// </summary>
  public class WorksheetDataHolder: IDisposable
  {
    #region Constants
    /// <summary>
    /// Format to get full path to zip archive item that stores vml drawings for worksheet.
    /// </summary>
    private const string VmlDrawingItemFormat = "xl/drawings/vmlDrawing{0}.vml";
    /// <summary>
    /// Format got get full path to zip archive item that stores comments description for worksheet.
    /// </summary>
    private const string CommentItemFormat = "xl/comments{0}.xml";
    /// <summary>
    /// Format to get full path to zip archive item that stores drawings for worksheet.
    /// </summary>
    private const string DrawingItemFormat = "xl/drawings/drawing{0}.xml";
    /// <summary>
    /// Relations default extension.
    /// </summary>
    private const string VmlExtension = "vml";
    #endregion

    #region Members
    /// <summary>
    /// Archive item that stores worksheet data.
    /// </summary>
    private ZipArchiveItem m_archiveItem;
    /// <summary>
    /// Objects that stores workbook data.
    /// </summary>
    private FileDataHolder m_parentHolder;
    /// <summary>
    /// This stream stores xml text starting just after "worksheet" tag to
    /// "col" or "sheetData" tag.
    /// </summary>
    private MemoryStream m_startStream = new MemoryStream();
    /// <summary>
    /// This stream stores xml text with conditional formatting.
    /// </summary>
    private MemoryStream m_cfStream/* = new MemoryStream()*/;
    /// <summary>
    /// This stream stores xml text with conditional formattings in extended list.
    /// </summary>
    internal Stream m_cfsStream;
    /// <summary>
    /// Relation id in the parent workbook. Null means that item hasn't been serialized
    /// yet and we must generate new id.
    /// </summary>
    private string m_strBookRelationId = null;
    /// <summary>
    /// Sheet id.
    /// </summary>
    private string m_strSheetId = null;
    /// <summary>
    /// Worksheet relations.
    /// </summary>
    private RelationCollection m_relations;
    /// <summary>
    /// Drawings relations.
    /// </summary>
    private RelationCollection m_drawingsRelation;
    /// <summary>
    /// Header/footer drawings relations.
    /// </summary>
    private RelationCollection m_hfDrawingsRelation;
    /// <summary>
    /// Relation id for vml drawings. Null means no vml drawings present (or were present).
    /// </summary>
    private string m_strVmlDrawingsId;
    /// <summary>
    /// Relation id for vml header/footer drawings. Null means no vml drawings present (or were present).
    /// </summary>
    private string m_strVmlHFDrawingsId;
    /// <summary>
    /// Relation id for comment notes. Null means no comment note present (or were present).
    /// </summary>
    private string m_strCommentsId;
    /// <summary>
    /// Relation id for drawings. Null means no drawings present (or were present).
    /// </summary>
    private string m_strDrawingsId;
    /// <summary>
    /// Stream with controls data.
    /// </summary>
    private Stream m_streamControls;
    private Dictionary<string,RelationCollection> m_preservedPivotTable;

    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the WorksheetDataHolder class.
    /// </summary>
    /// <param name="holder">Objects that stores workbook data.</param>
    /// <param name="relation">Relation that points at (relative path) necessary worksheet.</param>
    /// <param name="parentPath">Path to the relation parent object (to convert relative
    /// path into absolute).</param>
    public WorksheetDataHolder( FileDataHolder holder, Relation relation, string parentPath )
    {
      if( holder == null )
        throw new ArgumentNullException( "holder" );

      if( relation == null )
        throw new ArgumentNullException( "relation" );

      m_archiveItem = holder[ relation, parentPath ];
      m_parentHolder = holder;
    }
    /// <summary>
    /// Initializes new instance of the  WorksheetDataHolder class.
    /// </summary>
    /// <param name="holder">Objects that stores workbook data.</param>
    /// <param name="item">Archive item with sheet data.</param>
    public WorksheetDataHolder( FileDataHolder holder, ZipArchiveItem item )
    {
      if( item == null )
        throw new ArgumentNullException( "item" );

      m_archiveItem = item;
      m_parentHolder = holder;
    }
    #endregion

    #region Properties

    /// <summary>
    /// Gets parent FileDataHolder. Read-only.
    /// </summary>
    public FileDataHolder ParentHolder
    {
      get
      {
        return m_parentHolder;
      }
    }
    /// <summary>
    /// Gets or sets archive item that has all worksheet data.
    /// </summary>
    public ZipArchiveItem ArchiveItem
    {
      get
      {
        return m_archiveItem;
      }
      set
      {
        m_archiveItem = value;
      }
    }
    /// <summary>
    /// Gets or sets Worksheet's relation id in the workbook.
    /// </summary>
    public string RelationId
    {
      get
      {
        return m_strBookRelationId;
      }
      set
      {
        m_strBookRelationId = value;
      }
    }
    /// <summary>
    /// Gets or sets sheet id extracted from the file.
    /// </summary>
    public string SheetId
    {
      get
      {
        return m_strSheetId;
      }
      set
      {
        m_strSheetId = value;
      }
    }
    /// <summary>
    /// Gets relations collection. Read-only.
    /// </summary>
    public RelationCollection Relations
    {
      get
      {
        if( m_relations == null )
          m_relations = new RelationCollection();

        return m_relations;
      }
    }
    /// <summary>
    /// Gets drawings relation collection. Read-only.
    /// </summary>
    public RelationCollection DrawingsRelations
    {
      get
      {
        if( m_drawingsRelation == null )
          m_drawingsRelation = new RelationCollection();

        return m_drawingsRelation;
      }
    }
    /// <summary>
    /// Gets header/footer drawings relations collection. Read-only.
    /// </summary>
    public RelationCollection HFDrawingsRelations
    {
      get
      {
        if( m_hfDrawingsRelation == null )
          m_hfDrawingsRelation = new RelationCollection();

        return m_hfDrawingsRelation;
      }
    }
    /// <summary>
    /// Gets or sets relation id for vml drawings. Null means no vml drawings present (or were present).
    /// </summary>
    public string VmlDrawingsId
    {
      get
      {
        return m_strVmlDrawingsId;
      }
      set
      {
        m_strVmlDrawingsId = value;
      }
    }
    /// <summary>
    /// Gets or sets relation id for vml drawings. Null means no vml drawings present (or were present).
    /// </summary>
    public string VmlHFDrawingsId
    {
      get
      {
        return m_strVmlHFDrawingsId;
      }
      set
      {
        m_strVmlHFDrawingsId = value;
      }
    }
    /// <summary>
    /// Gets or sets relation id for comment notes item. Null means no comments present (or were present).
    /// </summary>
    public string CommentNotesId
    {
      get
      {
        return m_strCommentsId;
      }
      set
      {
        m_strCommentsId = value;
      }
    }
    /// <summary>
    /// Gets or sets relation id for drawings. Null means no drawings present (or were present).
    /// </summary>
    public string DrawingsId
    {
      get
      {
        return m_strDrawingsId;
      }
      set
      {
        m_strDrawingsId = value;
      }
    }
    /// <summary>
    /// Gets or sets stream with controls data.
    /// </summary>
    public Stream ControlsStream
    {
      get
      {
        return m_streamControls;
      }
      set
      {
        m_streamControls = value;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Parses conditional formats.
    /// </summary>
    /// <param name="dxfStyles">Dxf styles collection.</param>
    /// <param name="sheet">Worksheet to parse CF into.</param>
    public void ParseConditionalFormatting( List<DxfImpl> dxfStyles, WorksheetImpl sheet )
    {
      if( m_cfStream != null && m_cfStream.Length != 0 )
      {
        m_cfStream.Position = 0;
        Excel2007Parser parser = m_parentHolder.Parser;
        XmlReader reader = UtilityMethods.CreateReader( m_cfStream );
        //reader.Read();

        if( reader.LocalName == Excel2007Serializator.TemporaryRoot )
          reader.Read();

        sheet.m_parseCondtionalFormats = false;

        parser.ParseSheetConditionalFormatting( reader, sheet.ConditionalFormats, dxfStyles );

#if ( WINRT )
          reader.Dispose();
          m_cfStream.Dispose();
#else
        reader.Close();
        m_cfStream.Close();
#endif
         m_cfStream = null;
      }
      else if (m_cfsStream != null && m_cfsStream.Length != 0)
      {
          m_cfsStream.Position = 0;
          Excel2007Parser parser = m_parentHolder.Parser;
          XmlReader reader = UtilityMethods.CreateReader(m_cfsStream);
          //reader.Read();

          if (reader.LocalName == Excel2007Serializator.TemporaryRoot)
              reader.Read();
          if (reader.LocalName == CF.ConditionalFormattingsTagName)
              reader.Read();

          sheet.m_parseCondtionalFormats = false;

          parser.ParseSheetConditionalFormatting(reader, sheet.ConditionalFormats, dxfStyles);
#if ( WINRT )
          reader.Dispose();
          m_cfStream.Dispose();
#else
          //reader.Close();
          
#endif
            m_cfStream = null;
      }
    }
    /// <summary>
    /// Parses worksheet data.
    /// </summary>
    /// <param name="sheet">Worksheet to parse.</param>
    public void ParseWorksheetData( WorksheetImpl sheet, Dictionary<int, int> dictUpdateSSTIndexes, bool parseOnDemand )
    {
      if (m_archiveItem != null)
      {
          XmlReader reader = null;
          Excel2007Parser parser = m_parentHolder.Parser;

          // Here we have to locate relations collection and parse it if present.
          string strItemName = m_archiveItem.ItemName;
          int iSlashIndex = strItemName.LastIndexOf('/');
          string strParentPath = strItemName.Substring(0, iSlashIndex);
          string strRelationItem = strItemName.Insert(iSlashIndex, '/' +
                                                                   FileDataHolder.RelationsDirectory) +
                                   FileDataHolder.RelationExtension;
          ZipArchiveItem relationItem = m_parentHolder.Archive[strRelationItem];

          if (relationItem != null)
          {
              relationItem.DataStream.Position = 0;
              reader = UtilityMethods.CreateReader(relationItem.DataStream);
              m_relations = parser.ParseRelations(reader);
              m_relations.ItemPath = strRelationItem;
          }
#if !(WINRT )
          // Here we have to parse worksheet data or to call some parser to parse it.
          m_archiveItem.OptimizedDecompress = true;
#endif
          if (sheet.ParseDataOnDemand && !sheet.ParseOnDemand)
              sheet.ParseOnDemand = true;
          else
          {
              if (parseOnDemand)
              {
                  sheet.ParseDataOnDemand = false;
                  sheet.ParseOnDemand = true;
              }
              
          reader = UtilityMethods.CreateReader(m_archiveItem.DataStream);

          parser.ParseSheet(reader, sheet, strParentPath, ref m_startStream,
                                ref m_cfStream, m_parentHolder.XFIndexes, m_parentHolder.ItemsToRemove,
                                dictUpdateSSTIndexes);

          if (m_relations != null && m_relations.Count > 0)
              CollectPivotRelations(strItemName);

          m_parentHolder.ItemsToRemove.Add(m_archiveItem.ItemName, null);
          m_parentHolder.ItemsToRemove.Add(strRelationItem, null);
          m_archiveItem = null;
      }
    }
    }
    public void CollectPivotRelations(string itemName)
    {
        string strRelationId=null;
        m_preservedPivotTable = new Dictionary<string, RelationCollection>();
        RelationCollection relations = new RelationCollection();
        RelationCollection itemsToRemove = new RelationCollection();
        
        foreach (KeyValuePair<string,Relation> keyValuePair in m_relations)
        {
            if (keyValuePair.Value.Type ==RelationTypes.PivotTable)
            {
                relations.Add(keyValuePair.Value);
            }
            relations.ItemPath = m_relations.ItemPath;
        }
        m_preservedPivotTable.Add(itemName, relations);
    }
    public void ParsePivotTable(IWorksheet sheet)
    {
        if (m_preservedPivotTable!=null && m_preservedPivotTable.Count > 0)
        {
            foreach (KeyValuePair<string, RelationCollection> keyValuePair in m_preservedPivotTable)
            {
                string strItemName = keyValuePair.Key;
                int iSlashIndex = strItemName.LastIndexOf('/');
                string strParentPath = strItemName.Substring(0, iSlashIndex);
                ParsePivotTables(sheet, strParentPath, keyValuePair.Value);
     
            }
        }
    }
    /// <summary>
    /// Parses chart worksheet data.
    /// </summary>
    /// <param name="chart">Represents chart object to be parsed.</param>
    public void ParseChartsheetData( ChartImpl chart )
    {
      Excel2007Parser parser = m_parentHolder.Parser;
      string strItemName = m_archiveItem.ItemName;
      int iSlashIndex = strItemName.LastIndexOf( '/' );
      string strParentPath = strItemName.Substring( 0, iSlashIndex );

      string strRelations = FileDataHolder.GetCorrespondingRelations( strItemName );
      m_relations = m_parentHolder.ParseRelations( strRelations );

      XmlReader reader = UtilityMethods.CreateReader( m_archiveItem.DataStream );
      parser.ParseChartsheet( reader, chart );
    }
    /// <summary>
    /// Serializes worksheet into internal zip archive item.
    /// </summary>
    /// <param name="sheet">Worksheet to serialize.</param>
    /// <param name="hashNewXFIndexes">Dictionary with updated xf indexes.</param>
    /// <param name="cacheFiles">Dictionary that will contain pivot cache files
    /// (key - cache object, value - cache file name).</param>
    public void SerializeWorksheet( WorksheetImpl sheet, Dictionary<int, int> hashNewXFIndexes,
      Dictionary<PivotCacheImpl, string> cacheFiles )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      SerializeWorksheetPart( sheet, hashNewXFIndexes );
      SerializeWorksheetDrawings( sheet );
      SerializeComments( sheet );
      SerializeHeaderFooterImages( sheet, null );

# if !(SILVERLIGHT) && !(WINRT) && !(WP)
      SerializeOleStreamFile( sheet );
#endif
      SerializePivotTables( sheet, cacheFiles );
      SerializeWorksheetRelations();
    }
# if !(SILVERLIGHT) && !(WINRT) && !(WP)
    /// <summary>
    /// Serializes the OLE stream file.
    /// </summary>
    /// <param name="sheet">The sheet.</param>
    private void SerializeOleStreamFile( WorksheetImpl sheet )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( !sheet.HasOleObject )
        return;

      OleObjects oleObjects = ( OleObjects )sheet.OleObjects;

      foreach( OleObject oleObject in oleObjects )
      {
        if( oleObject.OleType == OleLinkType.Embed )
        {
          SerializeOle( sheet, oleObject );
        }
      }

    }
    /// <summary>
    /// Serializes the OLE.
    /// </summary>
    /// <param name="sheet">The sheet.</param>
    /// <param name="oleObject">The OLE object.</param>
    private void SerializeOle( WorksheetImpl sheet, OleObject oleObject )
    {
      MemoryStream stream;
      //string Extension = Excel2007Serializator.OleObjectFileExtension;
      //m_parentHolder.DefaultContentTypes[ Extension ] = Excel2007Serializator.OleObjectContentType;
      Excel2007Serializator serializator = m_parentHolder.Serializator;

      if( oleObject.StorageName == null )
      {
        oleObject.StorageName = OleTypeConvertor.GetOleFileName();
      }
      //else if( ( oleObject.OleSheetIndex != sheet.Index ) && ( oleObject.IsContainer == true ) )
      //{
      //  oleObject.StorageName = OleTypeConvertor.GetOleFileName();
      //}
      string strOleObjectItemName = "xl/embeddings/" + oleObject.StorageName;

      if( oleObject.IsContainer == true )
      {
        stream = ( MemoryStream )oleObject.Container;
      }
      else
      {
        stream = ( MemoryStream )GetOleDataStreamBin( oleObject, serializator );
      }

      m_parentHolder.Archive.UpdateItem( strOleObjectItemName, stream, true, FileAttributes.Archive );
      Relations[ oleObject.ShapeRId ] = new Relation( '/' + strOleObjectItemName, oleObject.RelationType );
      m_parentHolder.OverriddenContentTypes[ '/' + strOleObjectItemName ] = oleObject.ContentType;
    }
    /// <summary>
    /// Gets the OleObject data stream for bin file.
    /// </summary>
    /// <param name="oleObject">The OleObject.</param>
    /// <returns></returns>
    private Stream GetOleDataStreamBin( OleObject oleObject, Excel2007Serializator serializator )
    {
      string StorageName = oleObject.StorageName;
      oleObject.Container.Position = 0;
      if( oleObject.Container == null )
        throw new Exception( "Null value" );

      CompoundFile.XlsIO.Net.CompoundFile source = new CompoundFile.XlsIO.Net.CompoundFile( oleObject.Container );
      ICompoundStorage sourceRootStor = source.RootStorage.OpenStorage( StorageName );

      CompoundFile.XlsIO.Net.CompoundFile destination = new Syncfusion.CompoundFile.XlsIO.Net.CompoundFile();// StorageName, true );
      ( destination.Directory.Entries[ 0 ] as CompoundFile.XlsIO.Net.DirectoryEntry ).StorageGuid = OleTypeConvertor.GetGUID();

      for( int i = 0, cnt = sourceRootStor.Streams.Length; i < cnt; i++ )
      {
        CompoundStream destStream = destination.RootStorage.CreateStream( sourceRootStor.Streams[ i ] );
        CompoundStream srcStream = sourceRootStor.OpenStream( sourceRootStor.Streams[ i ] );
        byte[] buffer = new byte[ srcStream.Length ];
        srcStream.Read( buffer, 0, buffer.Length );
        destStream.Write( buffer, 0, buffer.Length );

        srcStream.Close();
        srcStream.Dispose();
        destStream.Close();
        destStream.Dispose();
      }

      MemoryStream memStream = new MemoryStream();
      destination.Save( memStream );
      destination.Dispose();

      memStream.Position = 0;
      return memStream;
    }
#endif
    /// <summary>
    /// Serializes worksheet into internal zip archive item.
    /// </summary>
    /// <param name="chart">Worksheet to serialize.</param>
    public void SerializeChartsheet( ChartImpl chart )
    {
      if( chart == null )
        throw new ArgumentNullException( "chart" );

      SerializeChartsheetPart( chart );
      //SerializeWorksheetDrawings( chart );
      SerializeVmlDrawings( chart );
      SerializeHeaderFooterImages( chart, null );
      SerializeWorksheetRelations();
    }
    /// <summary>
    /// This method tries to parse vml shapes.
    /// </summary>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="relationId">Relation id of the vml shapes relation.</param>
    /// <returns>Collection of the corresponding vml relations.</returns>
    public RelationCollection ParseVmlShapes( ShapeCollectionBase shapes, string relationId,
      RelationCollection relations )
    {
      if( shapes == null )
        throw new ArgumentNullException( "shapes" );

      RelationCollection result = null;

      if( relations == null )
        relations = m_relations;

      if( relations != null )
      {
        Relation vmlRelation = relations[ relationId ];

        if( vmlRelation == null )
          throw new ArgumentException( "relationId" );

        string strSheetPath = Path.GetDirectoryName( m_archiveItem.ItemName );
        strSheetPath = strSheetPath.Replace( '\\', '/' );
        string strVmlPath;
        XmlReader reader = m_parentHolder.CreateReaderAndFixBr( vmlRelation, strSheetPath, out strVmlPath );

        string strRelationsPath = FileDataHolder.GetCorrespondingRelations( strVmlPath );
        result = m_parentHolder.ParseRelations( strRelationsPath );
        int iIndex = strVmlPath.LastIndexOf( '/' );

        if( iIndex >= 0 )
        {
          strVmlPath = strVmlPath.Substring( 0, iIndex );
        }

        //WorksheetBaseImpl sheet = shapes.WorksheetBase;
        m_parentHolder.Parser.ParseVmlShapes( reader, shapes, result, strVmlPath );
        WorksheetImpl sheet = shapes.Worksheet;

        if( sheet != null )
        {
          IComments comments = sheet.Comments;
          int iCommentsCount = ( comments != null ) ? comments.Count : 0;

          Relation commentsRelation = m_relations.FindRelationByContentType(
              RelationTypes.WorksheetComments, out m_strCommentsId );

          if( commentsRelation != null )
          {              
           reader = m_parentHolder.CreateReader(commentsRelation, strSheetPath);
           m_parentHolder.Parser.ParseComments(reader, sheet as WorksheetImpl);             
          }
          else
          {
              sheet.Comments.Clear();
          }
        }
      }

      return result;
    }
# if !(SILVERLIGHT) && !(WINRT) && !(WP)
    /// <summary>
    /// Parses the OLE data.
    /// </summary>
    /// <param name="sheet">The sheet.</param>
    /// <param name="relationId">The relation id.</param>
    /// <param name="oleObject">The OLE object.</param>
    public void ParseOleData( WorksheetBaseImpl sheet, string relationId, OleObject oleObject )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      RelationCollection result = null;

      if( m_relations != null )
      {
        Relation oleRelation = m_relations[ relationId ];

        if( oleRelation == null )
          throw new ArgumentException( "relationId" );

        string strSheetPath = Path.GetDirectoryName( m_archiveItem.ItemName );
        strSheetPath = strSheetPath.Replace( '\\', '/' );
        byte[] data = sheet.DataHolder.ParentHolder.GetData( oleRelation, strSheetPath, true );

        string strTarget = oleRelation.Target;
        strTarget = FileDataHolder.CombinePath( strSheetPath, strTarget );
        strTarget = strTarget.Replace( '\\', '/' );
        string contentType = m_parentHolder.GetContentType( '/' + strTarget );
        oleObject.ContentType = contentType;
        oleObject.RelationType = oleRelation.Type;

        if( oleObject.OleObjectType == OleTypeConvertor.ToOleType("Document") )//!GetOleObjectType( oleObject.ObjectType ) )
        {
          oleObject.Container = new MemoryStream( data );
          oleObject.Container.Position = 0;
          oleObject.IsContainer = true;
          oleObject.FileNativeData = new byte[ 0 ];

          strTarget = Path.GetFileName( strTarget );
          //oleObject.FileName = strTarget;
          oleObject.StorageName = strTarget;
        }
        else
        {
          oleObject.Container = new MemoryStream( data );
          oleObject.Container.Position = 0;
          oleObject.IsContainer = true;
          oleObject.FileNativeData = new byte[ 0 ];

          oleObject.StorageName = Path.GetFileName( strTarget );
          //oleObject.SetOleFile( strTarget, oleObject.StorageName, data );
        }
      }
    }
    /// <summary>
    /// Gets the type of the OLE object.
    /// </summary>
    /// <param name="oleType">Type of the OLE.</param>
    /// <returns></returns>
    private bool GetOleObjectType( string oleType )
    {
      bool oleReturn = false;
      switch( oleType )
      {
        case "PowerPoint.Show.8":
        case "PowerPoint.Slide.12":
        case "PowerPoint.Show.12":
        case "Word.DocumentMacroEnabled.12":
        case "PowerPoint.SlideMacroEnabled.12":
        case "Word.Document.12":
        case "PowerPoint.ShowMacroEnabled.12":
        case "Word.Document.8":
          oleReturn = true;
          return oleReturn;
          break;
      }
      return oleReturn;
    }
#endif
    ///// <summary>
    ///// This method tries to parse vml shapes.
    ///// </summary>
    ///// <param name="sheet">Parent worksheet.</param>
    ///// <param name="relationId">Relation id of the vml shapes relation.</param>
    //public void ParseHFShapes( WorksheetImpl sheet, string relationId )
    //{
    //  if( sheet == null )
    //    throw new ArgumentNullException( "sheet" );

    //  if( m_relations != null )
    //  {
    //    Relation vmlRelation = m_relations[ relationId ];

    //    if( vmlRelation == null )
    //      throw new ArgumentException( "relationId" );

    //    string strSheetPath = Path.GetDirectoryName( m_archiveItem.ItemName );
    //    strSheetPath = strSheetPath.Replace( '\\', '/' );
    //    XmlReader reader = m_parentHolder.CreateReader( vmlRelation, strSheetPath );
    //    m_parentHolder.Parser.ParseVmlShapes( reader, sheet );
    //  }
    //}
    /// <summary>
    /// This method tries to parse drawings part.
    /// </summary>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="relationId">Relation id of the shapes relation.</param>
    /// <param name="dictItemsToRemove">Dictionary with archive items to remove after parsing.</param>
    public void ParseDrawings( WorksheetBaseImpl sheet, string relationId, Dictionary<string, object> dictItemsToRemove )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( m_relations != null )
      {
        Relation drawingRelation = m_relations[ relationId ];
        ParseDrawings( sheet, drawingRelation, dictItemsToRemove );
      }
    }
    /// <summary>
    /// This method tries to parse drawings part.
    /// </summary>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="drawingRelation">Relation pointing at drawings xml item..</param>
    /// <param name="dictItemsToRemove">Dictionary with archive items to remove after parsing.</param>
    public void ParseDrawings( WorksheetBaseImpl sheet, Relation drawingRelation,
      Dictionary<string, object> dictItemsToRemove )
    {
      if( drawingRelation == null )
        throw new ArgumentException( "relationId" );

      string strSheetPath = Path.GetDirectoryName( m_archiveItem.ItemName );
      strSheetPath = strSheetPath.Replace( '\\', '/' );
      XmlReader reader = m_parentHolder.CreateReader( drawingRelation, strSheetPath );
      string strFullDrawingsPath = FileDataHolder.CombinePath( strSheetPath, drawingRelation.Target );
      Excel2007Parser parser = m_parentHolder.Parser;
      //parser.ParseRelations( strDrawingRelationsPath );
      string strDrawingRelationsPath = FileDataHolder.GetCorrespondingRelations( strFullDrawingsPath );
      RelationCollection collection = m_parentHolder.ParseRelations( strDrawingRelationsPath );
      if (collection != null)
          m_drawingsRelation = collection;
      string strDrawingsPath;
      FileDataHolder.SeparateItemName( strFullDrawingsPath, out strDrawingsPath );

      // This list contains relation ids that were parsed and should be removed.
      List<string> lstParsedRelations = new List<string>();
      parser.ParseDrawings( reader, sheet, strDrawingsPath, lstParsedRelations, dictItemsToRemove );

      // Remove items that were parsed in order to reduce possibility that
      // there will be unnecessary items present in the resulting document.
      ZipArchive archive = m_parentHolder.Archive;
      archive.RemoveItem( strDrawingRelationsPath );
      archive.RemoveItem( strFullDrawingsPath );

      if( m_drawingsRelation != null )
      {
        for( int i = 0, len = lstParsedRelations.Count; i < len; i++ )
        {
          string strId = lstParsedRelations[ i ];
          m_drawingsRelation.Remove( strId );
        }
        // Here we have to remove parsed relations.
        //m_drawingsRelation.Clear();
      }
    }
    /// <summary>
    /// Serializes worksheet part into internal zip archive item.
    /// </summary>
    /// <param name="sheet">Worksheet to serialize.</param>
    /// <param name="hashNewXFIndexes">Dictionary with updated xf indexes.</param>
    private void SerializeWorksheetPart( WorksheetImpl sheet, Dictionary<int, int> hashNewXFIndexes )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );
        if (!sheet.ParseDataOnDemand)
        {
      Excel2007Serializator serializator = m_parentHolder.Serializator;
#if !SILVERLIGHT && !WINRT && !WP && !SyncfusionFramework1_1 && !SyncfusionFramework1_0
            ZippedContentStream stream = new ZippedContentStream(sheet.AppImplementation.CreateCompressor);
#else
          MemoryStream stream = new MemoryStream();
#endif
            StreamWriter streamWriter = new StreamWriter(stream);
            XmlWriter writer = UtilityMethods.CreateWriter(streamWriter);
            serializator.SerializeWorksheet(writer, sheet, m_startStream,
                                            m_cfStream, hashNewXFIndexes, m_cfsStream);

      writer.Flush();
      stream.Flush();

#if !SILVERLIGHT && !WINRT && !WP && !SyncfusionFramework1_1 && !SyncfusionFramework1_0
            m_archiveItem.Update(stream);
#else
      m_archiveItem.Update( stream, true );
#endif            
    }
    }
    /// <summary>
    /// Serializes chartsheet part into internal zip archive item.
    /// </summary>
    /// <param name="chart">Chartsheet to serialize.</param>
    private void SerializeChartsheetPart( ChartImpl chart )
    {
      if( chart == null )
        throw new ArgumentNullException( "chart" );

      string strDrawingItemName = GenerateDrawingsName(chart.Workbook);

      //if( m_strDrawingsId == null )
      //{
      //  m_strDrawingsId = Relations.GenerateRelationId();
      //}

      string strDrawingsId = Relations.GenerateRelationId();

      m_relations[ strDrawingsId ] = new Relation( '/' + strDrawingItemName, RelationTypes.Drawings );
      m_parentHolder.OverriddenContentTypes[ '/' + strDrawingItemName ] = ContentTypes.Drawings;
      // serialize drawing + chart object.

      ChartSerializator serializator = new ChartSerializator();
#if !SILVERLIGHT && !WINRT && !WP && !SyncfusionFramework1_1 && !SyncfusionFramework1_0
      ZippedContentStream stream = new ZippedContentStream( chart.AppImplementation.CreateCompressor );
#else
      MemoryStream stream = new MemoryStream();
#endif
      StreamWriter streamWriter = new StreamWriter( stream );
      XmlWriter writer = UtilityMethods.CreateWriter( streamWriter );
      serializator.SerializeChartsheet( writer, chart, strDrawingsId );
      writer.Flush();
      stream.Flush();

#if !SILVERLIGHT && !WINRT && !WP && !SyncfusionFramework1_1 && !SyncfusionFramework1_0
      m_archiveItem.Update( stream );
#else
      m_archiveItem.Update( stream, true );
#endif


      // Serialize chart item.
      RelationCollection drawingRelations = new RelationCollection();
      string strChartSheetDrawingsId = SerializeChartSheetDrawing( chart, strDrawingItemName, drawingRelations );
      SerializeChartObject( chart, drawingRelations, strChartSheetDrawingsId );

      SerializeRelations( drawingRelations, strDrawingItemName, null );
      //SerializeRelations( m_relations, m_archiveItem.ItemName );
    }
    /// <summary>
    /// Serializes chart object.
    /// </summary>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="drawingRelations">Drawing relations that should contain reference to this chart.</param>
    /// <param name="chartSheetDrawingId">Chart drawing id in the relations collection.</param>
    private void SerializeChartObject( ChartImpl chart, RelationCollection drawingRelations,
      string chartSheetDrawingId )
    {
      string strChartName = ChartShapeSerializator.GetChartFileName( this, chart );
      drawingRelations[ chartSheetDrawingId ] = new Relation( strChartName, RelationTypes.Chart );
      m_parentHolder.OverriddenContentTypes[ strChartName ] = ContentTypes.Chart;
      MemoryStream memStream = new MemoryStream();
      StreamWriter streamWriter = new StreamWriter( memStream );
      XmlWriter writer = UtilityMethods.CreateWriter( streamWriter );

      ChartSerializator serializator = new ChartSerializator();

      if( chart.DataHolder == null )
        m_parentHolder.CreateDataHolder( chart, strChartName );

      serializator.SerializeChart( writer, chart, strChartName );
      writer.Flush();
      streamWriter.Flush();
      strChartName = UtilityMethods.RemoveFirstCharUnsafe( strChartName );
      m_parentHolder.Archive.UpdateItem( strChartName, memStream, true, FileAttributes.Archive );

      //if( m_relations != null && m_relations.Count > 0 )
      SerializeRelations( chart.Relations, strChartName, null );
    }
    /// <summary>
    /// Serializes drawing part of the chartsheet.
    /// </summary>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="drawingItemName">Name of the drawing zip archive item.</param>
    /// <param name="drawingRelations">Drawing relations.</param>
    /// <returns>Relation id to the chart object.</returns>
    private string SerializeChartSheetDrawing( ChartImpl chart, string drawingItemName,
      RelationCollection drawingRelations )
    {
      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( drawingRelations == null )
        throw new ArgumentNullException();

      if( drawingItemName == null || drawingItemName.Length == 0 )
        throw new ArgumentOutOfRangeException( "drawingItemName" );

      ShapeCollectionBase shapes = chart.InnerShapesBase;

      // serialize chart with drawing
      string strChartSheetDrawingsId = drawingRelations.GenerateRelationId();
      drawingRelations[ strChartSheetDrawingsId ] = null;

      MemoryStream memStream = new MemoryStream();
      StreamWriter streamWriter = new StreamWriter( memStream );
      XmlWriter writer = UtilityMethods.CreateWriter( streamWriter );
      ChartSerializator serializator = new ChartSerializator();

      serializator.SerializeChartsheetDrawing( writer, chart, strChartSheetDrawingsId );

      writer.Flush();
      streamWriter.Flush();

      m_parentHolder.Archive.UpdateItem( drawingItemName, memStream, true, FileAttributes.Archive );

      return strChartSheetDrawingsId;
    }
    /// <summary>
    /// Serializes pivot tables.
    /// </summary>
    /// <param name="sheet">Worksheet to serialize pivot tables for.</param>
    /// <param name="dictCacheFiles">Contains pivot cache file names.</param>
    private void SerializePivotTables( WorksheetImpl sheet, Dictionary<PivotCacheImpl, string> dictCacheFiles )
    {
      PivotTableCollection pivotTables = sheet.InnerPivotTables;

      if( pivotTables != null && pivotTables.Count > 0 )
      {
        for( int i = 0, len = pivotTables.Count; i < len; i++ )
        {
          PivotTableImpl table = ( PivotTableImpl )pivotTables[ i ];
          SerializePivotTable( table, dictCacheFiles );
        }
      }
    }
    /// <summary>
    /// Serializes single pivot table.
    /// </summary>
    /// <param name="table">Table to serialize.</param>
    /// <param name="dictCacheFiles">Contains pivot cache file names.</param>
    private void SerializePivotTable( PivotTableImpl table, Dictionary<PivotCacheImpl, string> dictCacheFiles )
    {
      if( table == null )
        throw new ArgumentNullException( "table" );
      string strPivotTableItem = m_parentHolder.GeneratePivotTableName(table.Workbook.LastPivotTableIndex++);
      MemoryStream stream = new MemoryStream();
      XmlWriter writer = UtilityMethods.CreateWriter( stream, Encoding.UTF8 );
      PivotTableSerializator.SerializePivotTable( writer, table );
      writer.Flush();
      m_parentHolder.Archive.UpdateItem( strPivotTableItem, stream, true, FileAttributes.Archive );
      m_parentHolder.AddOverriddenContentType( strPivotTableItem, ContentTypes.PivotTable );
      string strPivotRelation = Relations.GenerateRelationId();
      if(Relations.FindRelationByTarget("/"+strPivotTableItem)==null )
          Relations[ strPivotRelation ] = new Relation( '/' + strPivotTableItem, RelationTypes.PivotTable );

      RelationCollection relations = new RelationCollection();
      string cacheRelation = relations.GenerateRelationId();
      string strCacheItem = dictCacheFiles[ table.Cache ];
      relations[ cacheRelation ] = new Relation( '/' + strCacheItem, RelationTypes.PivotCacheDefinition );
      m_parentHolder.SaveRelations( strPivotTableItem, relations );
    }
    /// <summary>
    /// Serializes worksheet relations.
    /// </summary>
    private void SerializeWorksheetRelations()
    {
      if( m_relations != null && m_relations.Count > 0 )
      {
        string strItemName = m_archiveItem.ItemName;
        int iSlashIndex = strItemName.LastIndexOf( '/' );
        string strRelationItem = strItemName.Insert( iSlashIndex, '/' +
          FileDataHolder.RelationsDirectory ) + FileDataHolder.RelationExtension;
        MemoryStream memStream = new MemoryStream();
        StreamWriter streamWriter = new StreamWriter( memStream );
        XmlWriter writer = UtilityMethods.CreateWriter( streamWriter );
        Excel2007Serializator serializator = m_parentHolder.Serializator;
        serializator.SerializeRelations( writer, m_relations, null);
        writer.Flush();
        streamWriter.Flush();

        m_parentHolder.Archive.UpdateItem( strRelationItem, memStream, true, FileAttributes.Archive );
      }
    }
    /// <summary>
    /// Serializes all worksheet drawings (including vml).
    /// </summary>
    /// <param name="sheet">Current worksheet.</param>
    private void SerializeWorksheetDrawings( WorksheetBaseImpl sheet )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      int iCount = sheet.Shapes.Count;

      if( iCount == 0 && sheet is IWorksheet && !sheet.UnknownVmlShapes )
      {
        if( m_strDrawingsId != null )
        {
          m_relations.Remove( m_strDrawingsId );
          m_strDrawingsId = null;
        }
      }
      else if( iCount != 0 || sheet.UnknownVmlShapes )
      {
        SerializeVmlDrawings( sheet );

        if( !SerializeDrawings( sheet ) && m_strDrawingsId != null )
        {
          m_relations.Remove( m_strDrawingsId );
          m_strDrawingsId = null;
        }
      }
    }
    /// <summary>
    /// Serializes worksheet drawings, except vml drawings.
    /// </summary>
    /// <param name="sheet">Worksheet to serialize drawings for.</param>
    public bool SerializeDrawings( WorksheetBaseImpl sheet )
    {
      return SerializeDrawings( sheet, Relations, ref m_strDrawingsId, ContentTypes.Drawings, RelationTypes.Drawings );
    }
    /// <summary>
    /// Serializes worksheet drawings, except vml drawings.
    /// </summary>
    /// <param name="sheet">Worksheet to serialize drawings for.</param>
    /// <param name="relations">Relations to put drawings relation into.</param>
    /// <param name="id">Drawing id.</param>
    /// <param name="contentType">Content type.</param>
    /// <param name="relationType">Relation type.</param>
    public bool SerializeDrawings( WorksheetBaseImpl sheet, RelationCollection relations,
      ref string id, string contentType, string relationType )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      ShapesCollection shapes = sheet.InnerShapes;

      // Here we should serialize all shapes, except comments
      WorksheetImpl worksheet = sheet as WorksheetImpl;

      int iAutoFilterCount = ( worksheet != null ) ?
        worksheet.AutoFilters.Count :
        0;

      if( shapes.Count - sheet.VmlShapesCount - iAutoFilterCount <= 0 && !Excel2007Serializator.HasAlternateContent( shapes ) )
        return false;

      // Step 1. Serialize all shapes into stream.
      MemoryStream stream = new MemoryStream();
      StreamWriter streamWriter = new StreamWriter( stream );
      XmlWriter writer = UtilityMethods.CreateWriter( streamWriter );
      Excel2007Serializator serializator = m_parentHolder.Serializator;
      serializator.SerializeDrawings( writer, shapes, this );
      writer.Flush();
      streamWriter.Flush();
      stream.Flush();

      // Step 2 - update correspond zip archive item and relations collection.
      string strDrawingsItemName = GenerateDrawingsName(sheet.Workbook);
      m_parentHolder.Archive.UpdateItem( strDrawingsItemName, stream, true, FileAttributes.Archive );
      // TODO: correct item path if necessary!
      string strAbsoluteItemPath = '/' + strDrawingsItemName;

      //TODO: Should use the below commented lines to fix  multiple instance of relationship
      relations[id] = new Relation(strAbsoluteItemPath, relationType);
      m_parentHolder.OverriddenContentTypes[strAbsoluteItemPath] = contentType;

      if (m_drawingsRelation != null && m_drawingsRelation.Count > 0)//sheet is WorksheetImpl )
          SerializeRelations(m_drawingsRelation, strDrawingsItemName, null);

      return true;
    }
    /// <summary>
    /// Serializes worksheet vml drawings.
    /// </summary>
    /// <param name="sheet">Worksheet to serialize drawings for.</param>
    private void SerializeVmlDrawings( WorksheetBaseImpl sheet )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      // On the current moment we support just comments.
      if( !sheet.HasVmlShapes )//( sheet.Comments == null || sheet.Comments.Count == 0 ) && !sheet.UnknownVmlShapes )
      {
        if( m_strVmlDrawingsId != null )
        {
          m_relations.Remove( m_strVmlDrawingsId );
          m_strVmlDrawingsId = null;
        }

        return;
      }

      m_parentHolder.DefaultContentTypes[ VmlExtension ] = ContentTypes.Vml;
      Excel2007Serializator serializator = m_parentHolder.Serializator;
      // Step 1 - get correct file name (reuse existing document if there is any).
      string strVmlDrawingsItemName = GenerateVmlDrawingsName();
      RelationCollection vmlRelations = new RelationCollection();

      // Step 2 - serialize into stream.
      MemoryStream stream = new MemoryStream();
      StreamWriter streamWriter = new StreamWriter( stream );
      XmlWriter writer = UtilityMethods.CreateWriter( streamWriter, true );
      serializator.SerializeVmlShapes( writer, sheet.InnerShapes, this,
        serializator.VmlSerializators, vmlRelations );
      streamWriter.Flush();
      stream.Flush();

      // Step 3 - update correspond zip archive item and relations collection.
      m_parentHolder.Archive.UpdateItem( strVmlDrawingsItemName, stream, true, FileAttributes.Archive );
      // TODO: correct item path if necessary!
      Relations[ m_strVmlDrawingsId ] = new Relation( '/' + strVmlDrawingsItemName, RelationTypes.VmlDrawings );

      SerializeRelations( vmlRelations, strVmlDrawingsItemName, null );
    }
    /// <summary>
    /// This method serializes header/footer images.
    /// </summary>
    /// <param name="sheet">Worksheet to serialize header/footer images for.</param>
    public void SerializeHeaderFooterImages( WorksheetBaseImpl sheet, RelationCollection relations )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      HeaderFooterShapeCollection hfShapes = sheet.InnerHeaderFooterShapes;

      if( relations == null )
        relations = Relations;

      if( hfShapes == null || hfShapes.Count == 0 )
      {
        if( m_strVmlHFDrawingsId != null )
          relations.Remove( m_strVmlHFDrawingsId );

        return;
      }

      m_parentHolder.DefaultContentTypes[ VmlExtension ] = ContentTypes.Vml;
      Excel2007Serializator serializator = m_parentHolder.Serializator;

      // Step 1 - get correct file name (reuse existing document if there is any).
      string strVmlHFDrawingsItemName = GenerateVmlDrawingsName();

      // Step 2 - serialize into stream.
      MemoryStream stream = new MemoryStream();
      StreamWriter streamWriter = new StreamWriter( stream );
      XmlWriter writer = UtilityMethods.CreateWriter( streamWriter, true );
      serializator.SerializeVmlShapes( writer, hfShapes, this, serializator.HFVmlSerializators, relations );
      streamWriter.Flush();
      stream.Flush();

      // Step 3 - update correspond zip archive item and relations collection.
      m_parentHolder.Archive.UpdateItem( strVmlHFDrawingsItemName, stream, true, FileAttributes.Archive );
      // TODO: correct item path if necessary!
      relations[ m_strVmlHFDrawingsId ] = new Relation( '/' + strVmlHFDrawingsItemName, RelationTypes.VmlDrawings );
      SerializeRelations( m_hfDrawingsRelation, strVmlHFDrawingsItemName, null );
    }
    /// <summary>
    /// Serializes relations.
    /// </summary>
    /// <param name="strParentItemName">Represents parent item name.</param>
    public void SerializeRelations( string strParentItemName )
    {
      if( m_relations != null && m_relations.Count > 0 )
      {
        MemoryStream stream = new MemoryStream();
        StreamWriter streamWriter = new StreamWriter( stream );
        XmlWriter writer = UtilityMethods.CreateWriter( streamWriter );
        m_parentHolder.Serializator.SerializeRelations( writer, m_relations, null );
        writer.Flush();
        streamWriter.Flush();
        stream.Flush();

        int iSlashIndex = strParentItemName.LastIndexOf( '/' );
        string strRelationsFile = strParentItemName.Insert( iSlashIndex, '/' + FileDataHolder.RelationsDirectory )
          + FileDataHolder.RelationExtension;

        m_parentHolder.Archive.UpdateItem( strRelationsFile, stream, true, FileAttributes.Archive );
      }
    }
    /// <summary>
    /// Serializes relations.
    /// </summary>
    /// <param name="relations">Relation to be serialized.</param>
    /// <param name="strParentItemName">Parent item name.</param>
    public void SerializeRelations( RelationCollection relations, string strParentItemName, WorksheetDataHolder holder )
    {
      if( strParentItemName == null || strParentItemName.Length == 0 )
        throw new ArgumentOutOfRangeException( strParentItemName );

      if( relations != null && relations.Count > 0 )
      {
        MemoryStream stream = new MemoryStream();
        StreamWriter streamWriter = new StreamWriter( stream );
        XmlWriter writer = UtilityMethods.CreateWriter( streamWriter );
        m_parentHolder.Serializator.SerializeRelations( writer, relations, holder );
        writer.Flush();
        streamWriter.Flush();
        stream.Flush();

        int iSlashIndex = strParentItemName.LastIndexOf( '/' );

        if( strParentItemName[ 0 ] == '/' )
        {
          strParentItemName = UtilityMethods.RemoveFirstCharUnsafe( strParentItemName );
          iSlashIndex--;
        }

        string strRelationsFile = strParentItemName.Insert( iSlashIndex, '/' + FileDataHolder.RelationsDirectory )
          + FileDataHolder.RelationExtension;

        m_parentHolder.Archive.UpdateItem( strRelationsFile, stream, true, FileAttributes.Archive );
      }
    }
    /// <summary>
    /// Serializes worksheet comments.
    /// </summary>
    /// <param name="sheet">Worksheet to get comments from.</param>
    private void SerializeComments( WorksheetImpl sheet )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      CommentsCollection comments = sheet.InnerComments;

      if( comments == null || comments.Count == 0 )
        return;

      // 1. Generate item name
      string strCommentsItemName = GenerateCommentsName();

      // 2. Serialize into MemoryStream
      MemoryStream stream = new MemoryStream();
      StreamWriter streamWriter = new StreamWriter( stream );
      XmlWriter writer = UtilityMethods.CreateWriter( streamWriter );
      Excel2007Serializator serializator = m_parentHolder.Serializator;
      serializator.SerializeCommentNotes( writer, sheet );
      writer.Flush();
      streamWriter.Flush();
      stream.Flush();

      // 3. Update archive and relations.
      m_parentHolder.Archive.UpdateItem( strCommentsItemName, stream, true, FileAttributes.Archive );
      Relations[ m_strCommentsId ] = new Relation( '/' + strCommentsItemName, RelationTypes.WorksheetComments );
      m_parentHolder.OverriddenContentTypes[ '/' + strCommentsItemName ] = ContentTypes.Comments;
    }
    /// <summary>
    /// Generates unique zip archive item name for drawings of the specified
    /// worksheet or reuses existing one.
    /// </summary>
    /// <returns>Name that can be used for drawings item.</returns>
    private string GenerateDrawingsName(IWorkbook workbook)
    {
      string strResult;

      if (workbook.Saved)
      {
          int iIndex = ++m_parentHolder.LastDrawingIndex;
          strResult = string.Format(DrawingItemFormat, iIndex);
      }
      else
      {
          do
          {
              int iIndex = ++m_parentHolder.LastDrawingIndex;
              strResult = string.Format(DrawingItemFormat, iIndex);
          }
          while (m_parentHolder.Archive.Find(strResult) != -1);
      }

      return strResult;
    }
    /// <summary>
    /// Generates unique zip archive item name for vml drawings of the specified
    /// worksheet or reuses existing one.
    /// </summary>
    /// <returns>Name that can be used for vml drawings item.</returns>
    private string GenerateVmlDrawingsName()
    {
      int iIndex = ++m_parentHolder.LastVmlIndex;
      string strResult = string.Format( VmlDrawingItemFormat, iIndex );
      return strResult;
    }
    /// <summary>
    /// Generates unique zip archive item name for comments of the specified
    /// worksheet or reuses existing one.
    /// </summary>
    /// <returns>Name that can be used for comments item.</returns>
    private string GenerateCommentsName()
    {
      int iIndex = ++m_parentHolder.LastCommentIndex;
      string strResult = string.Format( CommentItemFormat, iIndex );
      return strResult;
    }
    /// <summary>
    /// Serializes all tables from the specified worksheet.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize worksheet's part of the tables serialization.</param>
    public void SerializeTables( XmlWriter writer, WorksheetImpl sheet )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      IListObjects listObjects = sheet.InnerListObjects;
      int iCount = ( listObjects != null ) ? listObjects.Count : 0;

      if( iCount == 0 )
        return;

      writer.WriteStartElement( ListObjects.TableParts );
      writer.WriteAttributeString( Excel2007Serializator.CountAttributeName, iCount.ToString() );

      for( int i = 0; i < iCount; i++ )
      {
        IListObject listObject = listObjects[ i ];
        string strRelationId = SerializeTable( listObject );
        writer.WriteStartElement( ListObjects.TablePart );

        writer.WriteAttributeString( Excel2007Serializator.RelationAttribute,
          Excel2007Serializator.RelationNamespace, strRelationId );
        
        writer.WriteEndElement();
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes single table item.
    /// </summary>
    /// <param name="listObject">Item to serialize.</param>
    /// <returns>Relation id of the serialized item.</returns>
    private string SerializeTable( IListObject listObject )
    {
      string fileName = m_parentHolder.SerializeTable( listObject );
      Relation relation = new Relation( '/' + fileName, RelationTypes.Table );
      return Relations.Add( relation );
    }
    internal void ParseTablePart( IWorksheet sheet, string strRelation, string sheetPath )
    {
        XmlReader queryTableReader;
        Excel2007Parser queryTableParser = m_parentHolder.Parser;
      Relation relation = m_relations[ strRelation ];

      if( relation == null )
        throw new XmlException();

      string strItemPath;
      XmlReader reader = m_parentHolder.CreateReader( relation, sheetPath, out strItemPath );

      while( reader.NodeType != XmlNodeType.Element )
        reader.Read();
      TableParser parser = new TableParser();
      IListObject Table = parser.Parse( reader, sheet );
      if (Table.TableType == ExcelTableType.queryTable)
      {
          string TablePathformat = "xl/tables";
          string TablePath;
          string strItemName = strItemPath;
          int iSlashIndex = strItemName.LastIndexOf('/');
          string strParentPath = strItemName.Substring(0, iSlashIndex);
          string strRelationItem = strItemName.Insert(iSlashIndex, '/' +
            FileDataHolder.RelationsDirectory) + FileDataHolder.RelationExtension;
          ZipArchiveItem relationItem = m_parentHolder.Archive[strRelationItem];
          RelationCollection QueryRelation = new RelationCollection();
          if (relationItem != null)
          {
              reader = UtilityMethods.CreateReader(relationItem.DataStream);
              QueryRelation = queryTableParser.ParseRelations(reader);
              QueryRelation.ItemPath = strRelationItem;
          }
          Relation Relation = QueryRelation["rId1"];
          XmlReader queryReader = m_parentHolder.CreateReader(Relation, TablePathformat, out TablePath);
          parser.ParseQueryTable(queryReader, Table);
          m_relations.Remove(strRelation);
          m_parentHolder.Archive.RemoveItem(strRelationItem);
          m_parentHolder.ItemsToRemove[TablePath] = null;
      }
      
      m_parentHolder.ItemsToRemove[ strItemPath ] = null;
    }
    internal void ParsePivotTables(IWorksheet sheet,string strParentPath,RelationCollection relations)
    {
        string strItemPath = null;
        PivotTableCollection pivotTables=sheet.PivotTables as PivotTableCollection;
        foreach (KeyValuePair<string, Relation> pivotRelations in relations)
        {
            if (pivotRelations.Value.Type == RelationTypes.PivotTable)
            {
                XmlReader reader = m_parentHolder.CreateReader(pivotRelations.Value, strParentPath, out strItemPath);
                PivotCacheCollection caches = sheet.Workbook.PivotCaches as PivotCacheCollection;
                PivotTableImpl pivotTable = new PivotTableImpl(sheet.Application, sheet);
                PivotTableParser.ParsePivotTable(reader,pivotTable);
                pivotTables.Add(pivotTable);
                m_parentHolder.ItemsToRemove.Add(strItemPath, null);
                m_relations.Remove(pivotRelations.Key);
            }
        }
    }
    internal void AssignDrawingrelation(RelationCollection relation)
    {
        m_drawingsRelation = relation;
    }
    #endregion

    #region ICloneable Members
    /// <summary>
    /// Create copy of the current object.
    /// </summary>
    /// <returns>A copy of the current object.</returns>
    public WorksheetDataHolder Clone( FileDataHolder dataHolder )
    {
      WorksheetDataHolder result = ( WorksheetDataHolder )MemberwiseClone();
      result.m_parentHolder = dataHolder;

      if( m_archiveItem != null )
        result.m_archiveItem = dataHolder.Archive[ m_archiveItem.ItemName ];
      //result.m_startStream = new MemoryStream();
      //MemoryStream m_cfStream/* = new MemoryStream()*/;
      result.m_relations = ( RelationCollection )CloneUtils.CloneCloneable( m_relations );
      result.m_drawingsRelation = ( RelationCollection )CloneUtils.CloneCloneable( m_drawingsRelation );
      result.m_hfDrawingsRelation = ( RelationCollection )CloneUtils.CloneCloneable( m_hfDrawingsRelation );

      byte[] data = new byte[m_cfStream.Length];
      m_cfStream.Position = 0;
      m_cfStream.Read(data, 0, data.Length);
      m_cfStream.Position = 0;
      result.m_cfStream = new MemoryStream(data);

      if (m_cfsStream != null)
      {
          byte[] extData = new byte[m_cfsStream.Length];
          m_cfsStream.Position = 0;
          m_cfsStream.Read(extData, 0, extData.Length);
          m_cfsStream.Position = 0;
          result.m_cfsStream = new MemoryStream(extData);
      }

      result.m_startStream = (MemoryStream)CloneUtils.CloneStream(m_startStream);
      result.m_streamControls = (MemoryStream)CloneUtils.CloneStream(m_streamControls);
      //Stream m_streamControls;

      return result;
    }

    #endregion
    #region IDisposable Members

    public void Dispose()
    {
        m_archiveItem = null;
        m_cfStream = null;
        m_cfsStream = null;
        m_startStream = null;
        m_parentHolder.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion
  }
}

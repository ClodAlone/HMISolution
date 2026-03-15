#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO;
#endif

#if  (SILVERLIGHT || WP) 
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif !(WINRT )
using System.Drawing;
using System.Drawing.Imaging;
#endif
namespace Syncfusion.XlsIO.Implementation.Collections
{
	/// <summary>
	/// Summary description for ShapeCollectionBase.
	/// </summary>
  public abstract class ShapeCollectionBase : CollectionBaseEx<IShape>
  {
    #region Class constants
    /// <summary>
    /// Default id increment when group changes.
    /// </summary>
    public const int DEF_ID_PER_GROUP = 1024;
    /// <summary>
    /// Shapes count would be rounded to this value.
    /// </summary>
    public const int DEF_SHAPES_ROUND_VALUE = 1024;
    #endregion

    #region Class members
    /// <summary>
    /// Container of all worksheet's shapes.
    /// </summary>
    private MsofbtSpContainer m_groupInfo;
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    protected WorksheetBaseImpl m_sheet;
    /// <summary>
    /// Index of the collection.
    /// </summary>
    private int m_iCollectionIndex;
    /// <summary>
    /// Id of the last shape.
    /// </summary>
    private int m_iLastId;
    /// <summary>
    /// Id of the first shape.
    /// </summary>
    private int m_iStartId;
    private List<MsofbtRegroupItems> m_arrRegroundItems = new List<MsofbtRegroupItems>();
    private Stream m_layoutStream;
    #endregion
    
    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public ShapeCollectionBase( IApplication application, object parent )
      : base( application, parent )
    {
      InitializeCollection();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    /// <param name="container"></param>
    /// <param name="options">Parse options.</param>
    [ CLSCompliant( false ) ]
    public ShapeCollectionBase( IApplication application, object parent, MsofbtSpgrContainer container
      , ExcelParseOptions options )
      : this( application, parent )
    {
      Parse( container, options );
    }
    /// <summary>
    /// Initializes collection.
    /// </summary>
    protected virtual void InitializeCollection()
    {
      SetParents();
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    protected void SetParents()
    {
      m_sheet = FindParent( typeof( WorksheetBaseImpl ), true ) as WorksheetBaseImpl;

      if( m_sheet == null )
        throw new ArgumentNullException( "Can't find parent worksheet." );

      //      m_book = m_sheet.ParentWorkbook;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public int ShapesCount
    {
      get
      {
        int iResult = Count;
        return ( iResult > 0 ) ? iResult + 1 : 0;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int ShapesTotalCount
    {
      get
      {
        // NOTE: Iterate through all children and
        // if they are groups, then increase count.
        int iResult = 0;

        for( int i = 0, len = Count; i < len; i++ )
        {
          ShapeImpl shape = ( ShapeImpl )this[ i ];
          iResult += shape.ShapeCount;
        }

        return ( iResult > 0 ) ? iResult + 1 : 0;
      }
    }
    /// <summary>
    /// Returns parent WorksheetBase. Read-only.
    /// </summary>
    public WorksheetBaseImpl WorksheetBase
    {
      get
      {
        return m_sheet;
      }
    }
    /// <summary>
    /// Returns parent worksheet. Read-only.
    /// </summary>
    public WorksheetImpl Worksheet
    {
      get
      {
        return m_sheet as WorksheetImpl;
      }
    }
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    public WorkbookImpl Workbook
    {
      get
      {
        return m_sheet.ParentWorkbook;
      }
    }

    /// <summary>
    /// Returns single shape from the collection by its index. Read-only.
    /// </summary>
    public IShape this[ int index ]
    {
      get
      {
        return List[ index ] as IShape;
      }
    }
    /// <summary>
    /// Returns single shape from the collection by its name or null when cannot find. Read-only.
    /// </summary>
    public IShape this[ string strShapeName ]
    {
      get
      {
        if( strShapeName == null )
          throw new ArgumentNullException( "strShapeName" );

        if( strShapeName.Length == 0 )
          throw new ArgumentException( "Shape name cannot be null." );

        IList<IShape> list = List;

        for( int i = 0, iLen = list.Count; i < iLen; i++ )
        {
          IShape shape = list[ i ];

          if( shape.Name == strShapeName )
            return shape;
        }

        return null;
      }
    }
    /// <summary>
    /// Gets or sets the shape layout stream.
    /// </summary>
    /// <value>The shape layout stream.</value>
    internal Stream ShapeLayoutStream
    {
        get
        {
            return m_layoutStream;
        }
        set
        {
            m_layoutStream = value;
        }
    }
    /// <summary>
    /// Code of the Biff record in which all data is stored. Read-only.
    /// </summary>
    public abstract TBIFFRecord RecordCode { get; }
    /// <summary>
    /// Returns shared shape data for all shapes in this collection. Read-only.
    /// </summary>
    public abstract WorkbookShapeDataImpl ShapeData { get; }
    /// <summary>
    /// Index of the collection.
    /// </summary>
    internal int CollectionIndex
    {
      get
      {
        return m_iCollectionIndex;
      }
      set
      {
        m_iCollectionIndex = value;
      }
    }
    /// <summary>
    /// Id of the last shape.
    /// </summary>
    internal int LastId
    {
      get
      {
        return m_iLastId;
      }
      set
      {
        m_iLastId = value;
      }
    }
    /// <summary>
    /// Id of the first shape.
    /// </summary>
    internal int StartId
    {
      get
      {
        return m_iStartId;
      }
      set
      {
        m_iStartId = value;
      }
    }
    #endregion

    #region Class Parse methods
    /// <summary>
    /// Parses shapes group container.
    /// </summary>
    /// <param name="container">Container to parse.</param>
    /// <param name="options">Parse options.</param>
    private void Parse( MsofbtSpgrContainer container, ExcelParseOptions options )
    {
      // First container must be group description.
      List<MsoBase> items = container.ItemsList;
      MsofbtSpContainer record = ( MsofbtSpContainer )items[ 0 ];

      ParseGroupDescription( record );

      for( int i = 1, len = items.Count; i < len; i++ )
      {
        AddShape( items[ i ], options );
      }
    }

    /// <summary>
    /// Parses shapes group description record.
    /// </summary>
    /// <param name="groupDescription">Record to parse.</param>
    private void ParseGroupDescription( MsofbtSpContainer groupDescription )
    {
      m_groupInfo = ( MsofbtSpContainer )groupDescription.Clone();
      // First record must be Spgr container
      // The second one must be Sp
      MsofbtSp sp = m_groupInfo.Items[ 1 ] as MsofbtSp;

      if( sp != null )
        m_iStartId = sp.ShapeId;
    }
    /// <summary>
    /// Parses MsoDrawing records.
    /// </summary>
    /// <param name="arrStructures">Array of MsoDrawing records.</param>
    /// <param name="options">Parse options.</param>
    public void ParseMsoStructures( List<MsoBase> arrStructures, ExcelParseOptions options )
    {
      for( int i = 0, len = arrStructures.Count; i < len; i++ )
      {
        // Use 'as' to increase performance.
        MsoBase record = arrStructures[ i ];

        switch( record.MsoRecordType )
        {
          case MsoRecords.msofbtDgContainer:
            ParseMsoDgContainer( ( MsofbtDgContainer ) record, options );
            break;

          default:
            // TODO: this unexpected/unknown record maybe we should just save it,
            // but for the moment we just throw an exception
            throw new ArgumentOutOfRangeException( "Unexcpected MsoDrawing record" );
        }
      }
    }

    /// <summary>
    /// Parses MsofbtDgContainer.
    /// </summary>
    /// <param name="dgContainer">Container to parse.</param>
    /// <param name="options">Parse options.</param>
    private void ParseMsoDgContainer( MsofbtDgContainer dgContainer, ExcelParseOptions options )
    {
      List<MsoBase> items = dgContainer.ItemsList;

      for( int i = 0, len = items.Count; i < len; i++ )
      {
        // Use 'as' to increase performance.
        MsoBase record = items[ i ];

        switch( record.MsoRecordType )
        {
          case MsoRecords.msofbtDg:
            ParseMsoDg( ( MsofbtDg ) record );
            break;

          case MsoRecords.msofbtSpgrContainer:
            //m_shapes = new ShapesCollection( Application, this, ( MsofbtSpgrContainer ) record, options );
            Parse( ( MsofbtSpgrContainer )record, options );
            break;

          case MsoRecords.msofbtRegroupItems:
            m_arrRegroundItems.Add( ( MsofbtRegroupItems )record );
            break;

          default:
            // NOTE: this is unknown for us record, we're throwing an exception, 
            // but maybe we should just save it somewhere;
            //throw new ArgumentOutOfRangeException( "Unexpected MsoDrawing record" );
            break;
        }
      }
    }

    /// <summary>
    /// Parses MsofbtDg record.
    /// </summary>
    /// <param name="dgRecord">Record to parse.</param>
    private void ParseMsoDg( MsofbtDg dgRecord )
    {
      // TODO: implement parsing
      CollectionIndex = dgRecord.Instance;
      m_iLastId = dgRecord.LastId;
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Adds shape copy to the collection.
    /// </summary>
    /// <param name="sourceShape">Shape to copy.</param>
    /// <returns>Added shape.</returns>
    public IShape AddCopy( ShapeImpl sourceShape )
    {
      return AddCopy( sourceShape, null, null );
    }
    /// <summary>
    /// Adds shape copy to shapes collection.
    /// </summary>
    /// <param name="sourceShape">Shape to copy.</param>
    /// <param name="hashNewNames">Dictionary with new names of worksheets.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <returns>Added shape.</returns>
    public IShape AddCopy( ShapeImpl sourceShape
      , Dictionary<string, string> hashNewNames, Dictionary<int, int> dicFontIndexes )
    {
      IShape result = sourceShape.Clone( this, hashNewNames, dicFontIndexes, true );
      ( result as ShapeImpl ).ShapeId = 0;
      return result;
    }
    /// <summary>
    /// Adds shape copy to the collection.
    /// </summary>
    /// <param name="sourceShape">Shape to copy.</param>
    /// <returns>Added shape.</returns>
    public IShape AddCopy( IShape sourceShape )
    {
      return AddCopy( ( ShapeImpl )sourceShape );
    }
    /// <summary>
    /// Adds shape copy to shapes collection.
    /// </summary>
    /// <param name="sourceShape">Shape to copy.</param>
    /// <param name="hashNewNames">Dictionary with new names of worksheets.</param>
    /// <param name="arrFontIndexes">List with new font indexes.</param>
    /// <returns>Added shape.</returns>
    public IShape AddCopy( IShape sourceShape
      , Dictionary<string, string> hashNewNames, List<int> arrFontIndexes )
    {
      return AddCopy( ( ShapeImpl )sourceShape, hashNewNames, arrFontIndexes );
    }
    /// <summary>
    /// Adds new shape to the collection.
    /// </summary>
    /// <param name="newShape">Shape to add.</param>
    /// <returns>Newly added shape.</returns>
    public ShapeImpl AddShape( ShapeImpl newShape )
    {
      if( newShape == null )
        throw new ArgumentNullException( "newShape" );

      base.Add( newShape );
      return newShape;
    }
    /// <summary>
    /// Adds new shape to the collection.
    /// </summary>
    /// <param name="shape">Shape record to add.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Newly added shape.</returns>
    [ CLSCompliant( false ) ]
    protected ShapeImpl AddShape( MsoBase shape, ExcelParseOptions options )
    {
      if( shape is MsofbtSpContainer )
      {
        return AddShape( shape as MsofbtSpContainer, options );
      }
      // Here group is also possible but we don't support them fully yet.
      else if (shape is MsofbtSpgrContainer)
      {
          return AddGroupShape(shape as MsofbtSpgrContainer, options);
      }

      ShapeImpl newShape = new ShapeImpl( Application, this, shape, options );
      return AddShape( newShape );
    }
    /// <summary>
    /// Adds the group container shapes.
    /// </summary>
    /// <param name="shapes">Group Shape record to add.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Newly added shape.</returns>
    protected ShapeImpl AddGroupShape(MsofbtSpgrContainer shapes, ExcelParseOptions options)
    {
       ShapeImpl groupShape= CreateGroupShape(shapes, options);
        return AddShape(groupShape);
    }
      /// <summary>
      /// Create the group container shapes.
      /// </summary>
      /// <param name="shapes">Group Shape record to create.</param>
      /// <param name="options">Parse options.</param>
      /// <returns>Newly created shape.</returns>
    protected ShapeImpl CreateGroupShape(MsofbtSpgrContainer shapes, ExcelParseOptions options)
    {
        ShapeImpl groupShape = new ShapeImpl(Application, this, shapes, options);
        List<MsoBase> arrShape = shapes.ItemsList;
        ShapeImpl newShape = null;
        foreach (MsoBase shape in arrShape)
        {
            if (shape is MsofbtSpContainer)
                groupShape.ChildShapes.Add(AddChildShapes(shape as MsofbtSpContainer, options));
            else if (shape is MsofbtSpgrContainer)
                groupShape.ChildShapes.Add(CreateGroupShape(shape as MsofbtSpgrContainer, options));
            else
                groupShape.ChildShapes.Add(new ShapeImpl(Application, this, shape, options));
        }
        return groupShape;
    }
    /// <summary>
    /// Adds new child shape to the collection.
    /// </summary>
    /// <param name="shapeContainer">Shape container to add.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Newly added shape.</returns>
    [CLSCompliant(false)]
    protected ShapeImpl AddChildShapes(MsofbtSpContainer shapeContainer, ExcelParseOptions options)
    {
        ShapeImpl newShape = null;
        List<MsoBase> items = shapeContainer.ItemsList;

        // find out shape type
        for (int i = 0, len = items.Count; i < len; i++)
        {
            if (items[i] is MsofbtClientData)
            {
                MsofbtClientData clientData = items[i] as MsofbtClientData;

                List<ObjSubRecord> subRecords = clientData.ObjectRecord.RecordsList;

                for (int j = 0, sublen = subRecords.Count; j < sublen; j++)
                {
                    if (subRecords[j].Type == TObjSubRecordType.ftCmo)
                    {
                        ftCmo cmo = subRecords[j] as ftCmo;
                        newShape = CreateShape(cmo.ObjectType, shapeContainer, options, subRecords, cmo.ID);
                        break;
                    }
                }

                break;
            }
        }

        if (newShape == null)
            newShape = new ShapeImpl(Application, this, shapeContainer, ExcelParseOptions.Default);
        return newShape;
    }
    /// <summary>
    /// Adds new shape to the collection.
    /// </summary>
    /// <param name="shapeContainer">Shape container to add.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Newly added shape.</returns>
    [ CLSCompliant( false ) ]
    protected virtual ShapeImpl AddShape( MsofbtSpContainer shapeContainer, ExcelParseOptions options )
    {
      ShapeImpl newShape = null;
      List<MsoBase> items = shapeContainer.ItemsList;

      // find out shape type
      for( int i = 0, len = items.Count; i < len; i++ )
      {
        if( items[ i ] is MsofbtClientData )
        {
          MsofbtClientData clientData = items[ i ] as MsofbtClientData;

          List<ObjSubRecord> subRecords = clientData.ObjectRecord.RecordsList;

          for( int j = 0, sublen = subRecords.Count; j < sublen; j++ )
          {
            if( subRecords[ j ].Type == TObjSubRecordType.ftCmo )
            {
              ftCmo cmo = subRecords[ j ] as ftCmo;
              newShape = CreateShape( cmo.ObjectType, shapeContainer, options, subRecords, j );
              break;
            }
          }

          break;
        }
      }

      if( newShape == null )
        newShape = new ShapeImpl( Application, this, shapeContainer, ExcelParseOptions.Default );

      return AddShape( newShape );
    }

    /// <summary>
    /// Creates new shape object.
    /// </summary>
    /// <param name="objType">Object type to create.</param>
    /// <param name="shapeContainer">Shape container.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="subRecords">Subrecords of the shape's OBJRecord.</param>
    /// <param name="cmoIndex">Index to the cmo record inside subrecords.</param>
    [ CLSCompliant( false ) ]
    protected virtual ShapeImpl CreateShape( TObjType objType, MsofbtSpContainer shapeContainer,
      ExcelParseOptions options, List<ObjSubRecord> subRecords, int cmoIndex )
    {
      throw new NotImplementedException( "This method must be overriden in child classes" );
    }
    /// <summary>
    /// Removes shape from the collection.
    /// </summary>
    /// <param name="shape">Shape to remove.</param>
    public void Remove( IShape shape )
    {
      // NOTE: If there will be a lot of shapes in the collection,
      // then we'll have to change algorithm.
      for( int i = 0, len = Count; i < len; i++ )
      {
        if( this[ i ] == shape )
        {
          RemoveAt( i );
          break;
        }
      }
    }
    /// <summary>
    /// Creates copy of the collection.
    /// </summary>
    /// <param name="parent">Parent object for the new collection.</param>
    /// <returns>Copy of the collection.</returns>
    public override object Clone( object parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      //ShapeCollectionBase result = ( ShapeCollectionBase )base.Clone( parent );
      Type type = GetType();
      ConstructorInfo constructor = type.GetConstructor(
        new Type[] { typeof( IApplication ), typeof( object ) } );

      if( constructor == null )
        throw new ApplicationException( "Cannot find required constructor." );

      ShapeCollectionBase result = constructor.Invoke( new object[] { Application, parent } )
        as ShapeCollectionBase;

      result.m_iCollectionIndex = m_iCollectionIndex;
      result.m_iLastId = m_iLastId;
      result.m_iStartId = m_iStartId;

      result.RegisterInWorksheet();
      //result.WorksheetBase.InnerShapesBase = result;

      List<IShape> list = InnerList;
      //IList<ShapeImpl> destList = result.List;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IShape item = list[ i ];

        if( item is ShapeImpl )
        {
          ShapeImpl toClone = ( ShapeImpl )item;
          item = ( IShape )toClone.Clone( result );
        }
        else if( item is ICloneable )
        {
          ICloneable toClone = ( ICloneable )item;
          item = ( IShape )toClone.Clone();
        }

        //destList.Add( item );
      }

      result.SetParent( parent );
      result.SetParents();

      result.m_groupInfo = ( MsofbtSpContainer )CloneUtils.CloneCloneable( m_groupInfo );

      return result;
    }
    /// <summary>
    /// Registers in the parent worksheet.
    /// </summary>
    protected virtual void RegisterInWorksheet()
    {
      WorksheetBase.InnerShapesBase = this;
    }
    #endregion

    #region Serialization Methods
    /// <summary>
    /// Serializes shapes collection.
    /// </summary>
    /// <param name="records">Array of records that will receive all shapes.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( ShapesCount == 0 ) return;

      MsofbtDgContainer dgContainer = ( MsofbtDgContainer )MsoFactory.GetRecord(
        MsoRecords.msofbtDgContainer );//new MsofbtDgContainer( null );

      MsofbtDg dg = ( MsofbtDg )MsoFactory.GetRecord(
        MsoRecords.msofbtDg );//new MsofbtDg( dgContainer );

      MsofbtSpgrContainer spgrContainer = ( MsofbtSpgrContainer  )MsoFactory.GetRecord(
        MsoRecords.msofbtSpgrContainer );//new MsofbtSpgrContainer( dgContainer );

      MsofbtSpContainer spContainer = ( MsofbtSpContainer )MsoFactory.GetRecord(
        MsoRecords.msofbtSpContainer );//new MsofbtSpContainer( spgrContainer );

      MsofbtSpgr spgr = ( MsofbtSpgr )MsoFactory.GetRecord(
        MsoRecords.msofbtSpgr );//new MsofbtSpgr( spContainer );

      MsofbtSp sp = ( MsofbtSp )MsoFactory.GetRecord( MsoRecords.msofbtSp );//new MsofbtSp( spgr );
      sp.IsGroup = true;
      sp.IsPatriarch = true;
      sp.ShapeId = m_iStartId;
      //sp.Version = 2;

      //MsofbtRegroupItems regroup = ( MsofbtRegroupItems )MsoFactory.GetRecord( MsoRecords.msofbtRegroupItems );
      //regroup.Instance = 1;

      dgContainer.AddItem( dg );
      //dgContainer.AddItem( regroup );

      foreach( MsofbtRegroupItems regroup in m_arrRegroundItems )
        dgContainer.AddItem( regroup );

      dgContainer.AddItem( spgrContainer );
      spgrContainer.AddItem( spContainer );
      spContainer.AddItem( spgr );
      spContainer.AddItem( sp );

      //      foreach( ShapeImpl shape in List )/
      List<IShape> list = InnerList;
      for( int j = 0, len = list.Count; j < len; j++ )
      {
        ShapeImpl shape = list[ j ] as ShapeImpl;
        shape.PrepareForSerialization();
        shape.Serialize( spgrContainer );
      }

      List<int> arrBreaks = new List<int>();
      List<List<BiffRecordRaw>> arrRecords = new List<List<BiffRecordRaw>>();

      dg.ShapesNumber = ( uint )ShapesTotalCount;
      dg.LastId = m_iLastId;

      if( m_iCollectionIndex > 0 )
        dg.Instance = m_iCollectionIndex;

      MemoryStream buffer = new MemoryStream();
      buffer.Position = 8;
      CreateData( buffer, dgContainer, arrBreaks, arrRecords );

      if( arrBreaks.Count != arrRecords.Count )
        throw new ArgumentException( "Breaks and records do not fit each other." );

      int iCurPos = 0;
      //MSODrawingRecord msoDrawing;
      BiffRecordRaw msoDrawing;
      TBIFFRecord recordCode = RecordCode;

      if( arrBreaks.Count > 0 )
      {
        for( int i = 0, len = arrBreaks.Count; i < len; i++ )
        {
          int iCurBreak = arrBreaks[ i ];
          List<BiffRecordRaw> curRecords = arrRecords[ i ];
          int size = iCurBreak - iCurPos;
          
          if( iCurBreak > BiffRecordRaw.DEF_RECORD_MAX_SIZE )
          {
            //int iDataSize = size;

            while( size > 0 )
            {
              int iRealSize = Math.Min( size, BiffRecordRaw.DEF_RECORD_MAX_SIZE );
              msoDrawing = BiffRecordFactory.GetRecord( TBIFFRecord.Continue );
              ( msoDrawing as ContinueRecord ).SetLength( iRealSize );

              WriteData( msoDrawing, buffer, iCurPos, iRealSize );
              size -= iRealSize;
              iCurPos += iRealSize;
              records.Add( msoDrawing );
            }
          }
          else
          {
            msoDrawing = BiffRecordFactory.GetRecord( recordCode );
            WriteData( msoDrawing, buffer, iCurPos, size );
            records.Add( msoDrawing );
          }
          
          //byte[] arrNewData = new byte[ size ];
          ////Array.Copy( buffer, iCurPos, arrNewData, 0, size );
          //buffer.Position = iCurPos + 8;
          //buffer.Read( arrNewData, 0, size );
          //msoDrawing.Data = arrNewData;
          //( ( ILengthSetter ) msoDrawing ).SetLength( size );

          iCurPos = iCurBreak;
          
          records.AddList( curRecords );
        }
      }
      else
      {
        msoDrawing = BiffRecordFactory.GetRecord( recordCode );
        int iLength = ( int )( buffer.Length - 8 );
        byte[] arrData = new byte[ iLength ];
        buffer.Position = 8;
        buffer.Read( arrData, 0, iLength );

        msoDrawing.Data = arrData;
        ( ( ILengthSetter ) msoDrawing ).SetLength( iLength );
        records.Add( msoDrawing );
      }
    }
    private void WriteData( BiffRecordRaw record, MemoryStream buffer, int iCurPos, int size )
    {
      byte[] arrNewData = new byte[ size ];
      //Array.Copy( buffer, iCurPos, arrNewData, 0, size );
      buffer.Position = iCurPos + 8;
      buffer.Read( arrNewData, 0, size );
      record.Data = arrNewData;
      ( ( ILengthSetter )record ).SetLength( size );

    }
    /// <summary>
    /// Converts array of records with break indexes into bytes sequence.
    /// </summary>
    /// <param name="stream">Stream to put resulting data into.</param>
    /// <param name="dgContainer">Container.</param>
    /// <param name="arrBreaks">List with break indexes.</param>
    /// <param name="arrRecords">List with records to serialize.</param>
    /// <returns>Corresponding byte array.</returns>
    [ CLSCompliant( false ) ]
    protected virtual void CreateData( Stream stream, MsofbtDgContainer dgContainer,
      List<int> arrBreaks, List<List<BiffRecordRaw>> arrRecords )
    {
      dgContainer.FillArray( stream, 8, arrBreaks, arrRecords );
    }
    #endregion
  }
}

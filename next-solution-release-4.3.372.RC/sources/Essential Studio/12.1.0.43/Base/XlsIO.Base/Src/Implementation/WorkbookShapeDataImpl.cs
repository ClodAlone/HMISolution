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
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using FOPTE = Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing.MsofbtOPT.FOPTE;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Rectangle=Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using System.Drawing;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using System.Drawing;
#elif !(WINRT )
using System.Drawing;
using System.Drawing.Imaging;
#endif
namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Summary description for WorkbookShapeDataImpl.
  /// </summary>
  public class WorkbookShapeDataImpl
    : CommonObject
    , ICloneParent
  {
    #region Class constants
    /// <summary>
    /// Array of blips that should be parsed by MsoMetafilePicture.
    /// </summary>
    private static readonly MsoBlipType[] METAFILEBLIPS = new MsoBlipType[]
    {
      MsoBlipType.msoblipEMF,
      MsoBlipType.msoblipWMF,
      MsoBlipType.msoblipPICT,
    };
    #endregion

    #region Internal classes
    /// <summary>
    /// Contains information about different properties of image.
    /// </summary>
    protected class BlipParams
    {
      /// <summary>
      /// Instance property value.
      /// </summary>
      public int Instance;
      /// <summary>
      /// ReqMac property value.
      /// </summary>
      public byte ReqMac;
      /// <summary>
      /// ReqWin32 property value.
      /// </summary>
      public byte ReqWin32;
      /// <summary>
      /// SubRecordType property value.
      /// </summary>
      public int SubRecordType;

      /// <summary>
      /// 
      /// </summary>
      /// <param name="inst"></param>
      /// <param name="mac"></param>
      /// <param name="win32"></param>
      /// <param name="subrec"></param>
      public BlipParams( int inst, byte mac, byte win32, int subrec )
      {
        Instance = inst;
        ReqMac = mac;
        ReqWin32 = win32;
        SubRecordType = subrec;
      }
    }
    #endregion

    #region Class members
    /// <summary>
    /// Contains all workbook's pictures.
    /// </summary>
    private List<MsofbtBSE> m_arrPictures = new List<MsofbtBSE>();
    /// <summary>
    /// Drawing group not parsed records.
    /// </summary>
    private List<MsoBase> m_arrDGRecords = new List<MsoBase>();
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Dictionary, key - image hash (ArrayWrapper), value - MsofbtBSE structure that describes
    /// </summary>
    private Dictionary<ArrayWrapper, MsofbtBSE> m_dicImageIdToImage = new Dictionary<ArrayWrapper, MsofbtBSE>();
    /// <summary>
    /// Shape getter.
    /// </summary>
    //private IShapeGetter m_shapeGetter;
    private WorkbookImpl.ShapesGetterMethod m_shapeGetter;
    /// <summary>
    /// Last used worksheet shapes collection id.
    /// </summary>
    private int m_iLastCollectionId;
    /// <summary>
    /// Dictionary blip type - to - Instance, RequiredMac, RequiredWin32, subrecord type values.
    /// </summary>
    private static readonly Dictionary<MsoBlipType, BlipParams> s_hashBlipTypeToParams = new Dictionary<MsoBlipType, BlipParams>();
    /// <summary>
    /// Preserved Clusters.
    /// </summary>
    private MsofbtDgg m_preservedDgg;
    /// <summary>
    /// Indexed Pixel Types and not supported image format
    /// </summary>
    private string[] m_indexedpixel_notsupport = { "Format1bppIndexed", "Format4bppIndexed", "Format8bppIndexed", "b96b3cac-0728-11d3-9d7b-0000f81ef32e", "b96b3cad-0728-11d3-9d7b-0000f81ef32e" };
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Static constructor.
    /// </summary>
    static WorkbookShapeDataImpl()
    {
      s_hashBlipTypeToParams.Add( MsoBlipType.msoblipEMF, new BlipParams( 980, 4, 2, 61466 ) );
      s_hashBlipTypeToParams.Add( MsoBlipType.msoblipWMF, new BlipParams( 534, 4, 3, 61467 ) );
      s_hashBlipTypeToParams.Add( MsoBlipType.msoblipPNG, new BlipParams( 1760, 6, 6, 61470 ) );
      s_hashBlipTypeToParams.Add( MsoBlipType.msoblipJPEG, new BlipParams( 1130, 5, 5, 61469 ) );
    }
    /// <summary>
    /// Initializes new instance.
    /// </summary>
    /// <param name="application">Application object for the new instance.</param>
    /// <param name="parent">Parent object for the new instance.</param>
    /// <param name="shapeGetter">Shape getter to use for retrieving shapes from worksheet.</param>
    public WorkbookShapeDataImpl( IApplication application, object parent,
      WorkbookImpl.ShapesGetterMethod shapeGetter )//IShapeGetter shapeGetter )
      //IShapeGetter shapeGetter, TBIFFRecord recordCode )
      : base( application, parent )
    {
      if( shapeGetter == null )
        throw new ArgumentNullException( "shapeGetter" );

      m_shapeGetter = shapeGetter;
      SetParents();
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void SetParents()
    {
      m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( m_book == null )
        throw new ArgumentNullException( "parent", "Can't find parent workbook" );
    }
    #endregion

    #region Parse methods
    /// <summary>
    /// Parses drawing group record.
    /// </summary>
    /// <param name="drawGroup">Record to parse.</param>
    [ CLSCompliant( false ) ]
    public void ParseDrawGroup( MSODrawingGroupRecord drawGroup )
    {
      if( drawGroup == null )
        throw new ArgumentNullException( "drawGroup" );

      MsofbtDggContainer dggContainer = ( MsofbtDggContainer )drawGroup.StructuresList[ 0 ];
      List<MsoBase> arrItems = dggContainer.ItemsList;

      for( int i = 0, len = arrItems.Count; i < len; i++ )
      {
        // Use 'as' to increase performance.
        MsoBase record = arrItems[ i ];

        switch( record.MsoRecordType )
        {
          case MsoRecords.msofbtBstoreContainer:
            MsofbtBstoreContainer bStore = ( MsofbtBstoreContainer )record;
            ParsePictures( bStore );
            break;

          case MsoRecords.msofbtOPT:
            //m_arrDGOptions.AddRange( ( ( MsofbtOPT ) record ).Properties );
            break;

          case MsoRecords.msofbtDgg:
            m_preservedDgg = ( MsofbtDgg )record;
            // TODO: Parse this record.
            break;

          default:
            m_arrDGRecords.Add( record );
            break;
        }
      }
    }

    /// <summary>
    /// Parses picture container.
    /// </summary>
    private void ParsePictures( MsofbtBstoreContainer bStore )
    {
      if( bStore == null )
        throw new ArgumentNullException( "bStore" );

      List<MsoBase> arrBlips = bStore.ItemsList;

      for( int i = 0, len = arrBlips.Count; i < len; i++ )
      {
        // Use 'as' to increase performance.
        MsofbtBSE blip = arrBlips[ i ] as MsofbtBSE;
        m_arrPictures.Add( blip );
      }
    }

    #endregion

    #region Serialization methods

    protected override void OnDispose()
    {
        base.OnDispose();
        m_arrDGRecords.Clear();
        m_arrPictures.Clear();
        m_arrDGRecords = null;
        m_arrPictures = null;
        m_dicImageIdToImage.Clear();
        m_dicImageIdToImage = null;
        
        if (m_preservedDgg != null)
            m_preservedDgg.Dispose();
        m_book = null;
        m_bIsDisposed = true;
    }
    /// <summary>
    /// Serializes MsoDrawingGroupRecord if necessary.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive all records.</param>
    /// <param name="recordCode">Record code for serialization.</param>
    [ CLSCompliant( false ) ]
    public void SerializeMsoDrawingGroup( OffsetArrayList records, TBIFFRecord recordCode, IdReserver shapeIds )
    {
      bool bNeedMsoDrawingGroup = NeedMsoDrawingGroup;

      if( !bNeedMsoDrawingGroup && m_preservedDgg == null )
        return;

      MSODrawingGroupRecord result = ( MSODrawingGroupRecord )
        BiffRecordFactory.GetRecord( recordCode );

      MsofbtDggContainer dggContainer = new MsofbtDggContainer( null );

      MsofbtDgg dgg = new MsofbtDgg( dggContainer );
      dggContainer.AddItem( dgg );

      FillMsoDgg( dgg, m_shapeGetter, shapeIds );

      int iPicturesCount = ( m_arrPictures != null ) ? m_arrPictures.Count : 0;

      if( iPicturesCount > 0 )
      {
        MsofbtBstoreContainer blipsContainer = new MsofbtBstoreContainer( dggContainer );
        dggContainer.AddItem( blipsContainer );
        //int counter = 10;

        for( int i = 0; i < iPicturesCount; i++ )
        {

          // Use 'as' to increase performance.
          MsofbtBSE blip = m_arrPictures[ i ];
          blipsContainer.AddItem( blip );
        }
      }

      if( bNeedMsoDrawingGroup )
        SerializeDrawingGroupOptions( dggContainer );

      dggContainer.AddItems( m_arrDGRecords );

      result.AddStructure( dggContainer );

      records.Add( result );
    }
    /// <summary>
    /// Serializes drawing group options.
    /// </summary>
    /// <param name="dggContainer">Options container.</param>
    private void SerializeDrawingGroupOptions( MsofbtDggContainer dggContainer )
    {
      MsofbtOPT options = new MsofbtOPT( dggContainer );

      //      if( m_arrDGOptions != null && m_arrDGOptions.Count > 0 )
      //      {
      //        options.AddOptions( m_arrDGOptions );
      //      }
      //      else
    {
      SerializeDefaultOptions( options );
    }

      dggContainer.AddItem( options );
    }
    /// <summary>
    /// Serialize default options
    /// </summary>
    /// <param name="options">Options to serialize.</param>
    private void SerializeDefaultOptions( MsofbtOPT options )
    {
      FOPTE option = new FOPTE();
      option.Id = ( MsoOptions )191;
      option.IsComplex = false;
      option.IsValid = false;
      option.UInt32Value = 524296;

      options.AddOptionsOrReplace( option );

      option = new FOPTE();
      option.Id = ( MsoOptions )385;
      option.IsComplex = false;
      option.IsValid = false;
      option.UInt32Value = 134217793;

      options.AddOptionsOrReplace( option );

      option = new FOPTE();
      option.Id = ( MsoOptions )448;
      option.IsComplex = false;
      option.IsValid = false;
      option.UInt32Value = 134217792;

      options.AddOptionsOrReplace( option );
    }
    /// <summary>
    /// Fills MsofbtDgg record.
    /// </summary>
    /// <param name="dgg">Record to fill.</param>
    /// <param name="shapeGetter">Shape getter.</param>
    [ CLSCompliant( false ) ]
    protected void FillMsoDgg( MsofbtDgg dgg, /*IShapeGetter shapeGetter*/
      WorkbookImpl.ShapesGetterMethod shapeGetter, IdReserver shapeIds )
    {
      if( dgg == null )
        throw new ArgumentNullException( "dgg" );

      if( m_preservedDgg != null )
      {
        CopyData( m_preservedDgg, dgg );
      }
      else
      {
        uint uiTotalShapes = 0;
        uint uiTotalDrawings = 0;
        uint uiMaxId = ShapesCollection.DEF_ID_PER_GROUP;
        uint uiCurGroup = 0;
        WorkbookObjectsCollection arrObjects = m_book.Objects;

        for( int i = 0, len = arrObjects.Count; i < len; i++ )
        {
          // Use 'as' to increase performance.
          WorksheetBaseImpl sheet = arrObjects[ i ] as WorksheetBaseImpl;
          ShapeCollectionBase shapes = shapeGetter( sheet );//.GetShapes( sheet );//sheet.InnerShapes;
          WorksheetImpl worksheet = shapes.Worksheet;

          if( shapes.ShapesCount != 0 )
          {
            uiCurGroup++;
            uiTotalDrawings++;
            uiTotalShapes += ( uint )shapes.ShapesCount;
            uint uiCount = ( uint )shapes.ShapesCount;

            // NOTE: we don't have enough information to fill clusters correctly, but this
            // could fixes some strange shapes behaviour when data validation is selected
            // (jumping images, comments, etc.).
            if( worksheet != null && worksheet.InnerDVTable != null )
            {
              uiCount += ( uint )( worksheet.InnerDVTable.Count + 1 );
            }

            if( shapeIds == null )
            {
              dgg.AddCluster( uiCurGroup, uiCount );
            }

            uint mod = uiMaxId % ShapesCollection.DEF_ID_PER_GROUP;

            if( mod != 0 )
            {
              uiMaxId = uiMaxId - mod + ShapesCollection.DEF_ID_PER_GROUP;
            }

            uiMaxId += uiCount;//( uint )shapes.ShapesCount;
          }
        }

        if( shapeIds != null )
        {
          int iMaxId = shapeIds.MaximumId;
          uiMaxId = ( uint )iMaxId;

          int iCurrentId = 1024;

          while( iCurrentId < iMaxId )
          {
            int iGroupId = shapeIds.ReservedBy( iCurrentId );
            int iCount = shapeIds.GetAdditionalShapesNumber( iGroupId );
            dgg.AddCluster( ( uint )iGroupId, ( uint )iCount );
            iCurrentId += IdReserver.SegmentSize;
          }
        }

        dgg.TotalShapes = uiTotalShapes;
        dgg.TotalDrawings = uiTotalDrawings;
        // TODO: Change id.
        dgg.IdMax = uiMaxId;
      }
    }

    private void CopyData( MsofbtDgg source, MsofbtDgg destination )
    {
      foreach( MsofbtDgg.ClusterID cluster in source.ClusterIDs )
      {
        destination.AddCluster( cluster.GroupId, cluster.Number );
      }

      destination.IdMax = source.IdMax;
      destination.TotalDrawings = source.TotalDrawings;
      destination.TotalShapes = source.TotalShapes;
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Adds picture to the storage.
    /// </summary>
    /// <param name="image">Picture to add.</param>
    /// <param name="imageFormat">Desired image format.</param>
    /// <param name="strPictureName">Name of the picture.</param>
    /// <returns>Number of pictures after adding new picture (blip id).</returns>
    public int AddPicture( Image image, ExcelImageFormat imageFormat, string strPictureName )
    {
      if( image == null )
        throw new ArgumentNullException( "image" ); 
#if !SILVERLIGHT && !WINRT && !WP
      int indexed = Array.IndexOf(m_indexedpixel_notsupport, image.PixelFormat.ToString());
      int unsupport = Array.IndexOf(m_indexedpixel_notsupport, image.RawFormat.Guid.ToString());
      if (indexed == -1 && unsupport==-1 )
      {
          MemoryStream stream = new MemoryStream();
          // Save image to stream.
          if ((image.RawFormat.Guid.ToString() == ImageFormat.Jpeg.Guid.ToString() && Enum.GetName(typeof(PixelFormat), image.PixelFormat) == null) || 
              image.RawFormat.Guid.ToString() == ImageFormat.Emf.Guid.ToString() ||
              image.RawFormat.Guid.ToString() == ImageFormat.MemoryBmp.Guid.ToString())
             image.Save(stream, ImageFormat.Bmp);
          else
              image.Save(stream, image.RawFormat);
          //The input image having error so we need to resave it
          Image temp_img = Image.FromStream(stream);
          //Resaved image assign to input image
          image = temp_img;
      
          //That image handled by graphics
          Graphics gr = Graphics.FromImage(image);
          GraphicsUnit unit = GraphicsUnit.Pixel;
          RectangleF rect = image.GetBounds(ref unit);
          gr.DrawImage(image, rect);
          gr.Dispose();
        }
           
#endif
      MsofbtBSE newBse = new MsofbtBSE( null );
      newBse.BlipName = strPictureName;
      newBse.BlipType = ImageFormatToBlipType( image.RawFormat, imageFormat );
      newBse.BlipUsage = MsoBlipUsage.msoblipUsageDefault;

      BlipParams blipParams = GetBlipParams( newBse );
      newBse.RequiredMac = blipParams.ReqMac;
      newBse.Instance = newBse.RequiredWin32 = blipParams.ReqWin32;
      newBse.Version = 2;
      newBse.RefCount = 1;

      IPictureRecord pict = ( IsBitmapBlip( newBse.BlipType ) )
        ? ( IPictureRecord )new MsoBitmapPicture( newBse )
        : ( IPictureRecord )new MsoMetafilePicture( newBse );

      ( pict as MsoBase ).Instance = blipParams.Instance;
      ( pict as MsoBase ).MsoRecordType = ( MsoRecords )blipParams.SubRecordType;
      pict.Picture = image;
#if !SILVERLIGHT && !WINRT && !WP
      if (unsupport == -1)
      {
          MemoryStream imgStream;
          ImageFormat format = image.RawFormat;

          imgStream = new MemoryStream();
          image.Save(imgStream, format);
          pict.PictureStream = imgStream;
      }
#endif
      ArrayWrapper wrapper = new ArrayWrapper( pict.RgbUid );

      MsofbtBSE oldBse;
      m_dicImageIdToImage.TryGetValue( wrapper, out oldBse );

      if( oldBse != null && oldBse.BlipType == newBse.BlipType )
      {
        oldBse.RefCount++;
        return oldBse.Index + 1;
        // Here somehow we have get index of the bse record.
      }
      else
      {
        newBse.PictureRecord = pict;
        m_dicImageIdToImage.Add( wrapper, newBse );
        return AddPicture( newBse );
      }
    }
    /// <summary>
    /// Adds picture to the storage.
    /// </summary>
    /// <param name="picture">Picture to add.</param>
    /// <returns>Number of pictures after adding new picture..</returns>
    [CLSCompliant( false )]
    public int AddPicture( MsofbtBSE picture )
    {
      int iCount = m_arrPictures.Count;
      picture.Index = iCount;
      m_arrPictures.Add( picture );
      return iCount + 1;
    }
    /// <summary>
    /// Returns picture record.
    /// </summary>
    /// <param name="iPictureId">Picture index.</param>
    /// <returns>Picture record.</returns>
    [CLSCompliant( false )]
    public MsofbtBSE GetPicture( int iPictureId )
    {

      return m_arrPictures[ iPictureId - 1 ];

        //return (iPictureId > 0) ? m_arrPictures[iPictureId - 1] : m_arrPictures[iPictureId];
    }
    /// <summary>
    /// Removes picture from this collection.
    /// </summary>
    /// <param name="id">Picture id to remove.</param>
    /// <param name="removeImage">Indicates whether to remove image (not picture
    /// shape) from workbook if we didn't detect any reference to it.</param>
    [ CLSCompliant( false ) ]
    public void RemovePicture( uint id, bool removeImage )
    {
      if( id < 1 || id > m_arrPictures.Count )
        return;
        //throw new ArgumentOutOfRangeException( "id" );

      MsofbtBSE bseToRemove = m_arrPictures[ ( int )id - 1 ];
      bseToRemove.RefCount--;

      if( bseToRemove.RefCount <= 0 && removeImage )
      {
        m_arrPictures.RemoveAt( ( int )id - 1 );

        byte[] arr = bseToRemove.PictureRecord.RgbUid;
        ArrayWrapper wr = new ArrayWrapper( arr );
        
        if( m_dicImageIdToImage.ContainsKey( wr ) )
          m_dicImageIdToImage.Remove( wr );

        for( int j = ( int )id - 1, iCount = m_arrPictures.Count; j < iCount; j++ )
        {
          MsofbtBSE bse = m_arrPictures[ j ];
          bse.Index--;
        }

        WorkbookObjectsCollection arrObjects = m_book.Objects;

        for( int i = 0, len = arrObjects.Count; i < len; i++ )
        {
          WorksheetBaseImpl sheet = arrObjects[ i ] as WorksheetBaseImpl;
          ShapeCollectionBase shapes = m_shapeGetter( sheet );//.GetShapes( sheet );

          for( int j = 0, iShapesLen = shapes.Count; j < iShapesLen; j++ )
          {
            BitmapShapeImpl bmp = shapes[ j ] as BitmapShapeImpl;

            if( bmp != null && bmp.BlipId > id ) bmp.SetBlipId( bmp.BlipId - 1 );
          }
        }
      }
    }
    /// <summary>
    /// Clears all internal data.
    /// </summary>
    public void Clear()
    {
        if(m_dicImageIdToImage!=null)
        m_dicImageIdToImage.Clear();
        if(m_arrDGRecords!=null)
            m_arrDGRecords.Clear();
        if(m_arrPictures!=null)
      m_arrPictures.Clear();
    }
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone( object parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      WorkbookShapeDataImpl result = ( WorkbookShapeDataImpl )MemberwiseClone();
      result.SetParent( parent );
      result.SetParents();
      result.m_arrPictures = CloneUtils.CloneCloneable( m_arrPictures );;
      result.m_arrDGRecords = CloneUtils.CloneCloneable( m_arrDGRecords );
      result.m_dicImageIdToImage = CloneUtils.CloneHash( m_dicImageIdToImage );
      //result.m_shapeGetter = //( IShapeGetter )CloneUtils.CloneCloneable( m_shapeGetter );

      return result;
    }
    /// <summary>
    /// Registers new shapes collection.
    /// </summary>
    /// <returns></returns>
    public int RegisterShapes()
    {
      ++m_iLastCollectionId;
      return m_iLastCollectionId;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// List with all pictures. Read-only.
    /// </summary>
    public List<MsofbtBSE> Pictures
    {
      get
      {
        return m_arrPictures;
      }
    }
    /// <summary>
    /// Indicates whether this mso drawing group has to be serialized.
    /// </summary>
    protected bool NeedMsoDrawingGroup
    {
      get
      {
        WorkbookObjectsCollection arrObjects = m_book.Objects;

        foreach( ShapeCollectionBase shapes in m_book.EnumerateShapes( m_shapeGetter ) )
        {
          if( shapes != null && shapes.Count > 0 )
            return true;
        }
        //for( int i = 0, len = arrObjects.Count; i < len; i++ )
        //{
        //  // Use 'as' to increase performance.
        //  WorksheetBaseImpl sheet = arrObjects[ i ] as WorksheetBaseImpl;
        //  ShapeCollectionBase shapes = m_shapeGetter.GetShapes( sheet );

        //  if( shapes != null && shapes.Count > 0 ) return true;
        //}

        return false;//false;
      }
    }
    internal MsofbtDgg PreservedClusters
    {
      get
      {
        return m_preservedDgg;
      }
    }
    public void ClearPreservedClusters()
    {
      m_preservedDgg = null;
    }
    #endregion

    #region Class static methods
    /// <summary>
    /// Converts image format into blip type.
    /// </summary>
    /// <param name="format">Format to convert.</param>
    /// <returns>Returns appropriate image format.</returns>
    public static MsoBlipType ImageFormatToBlipType( ImageFormat format )
    {
      // TODO: add format types
      //      if( format == ImageFormat.Bmp )
      if( format.Equals( ImageFormat.Bmp ) )
      {
        return MsoBlipType.msoblipDIB;
        //return MsoBlipType.msoblipPNG;
      }
      else if( format.Equals( ImageFormat.Jpeg ) )
      {
        return MsoBlipType.msoblipJPEG;
        //return MsoBlipType.msoblipPNG;
      }
      else if( format.Equals( ImageFormat.Png ) )
      {
        return MsoBlipType.msoblipPNG;
      }
      else if( format.Equals( ImageFormat.Emf ) )
      {
        return MsoBlipType.msoblipEMF;
      }
      //      else if( format.Equals( ImageFormat.Wmf ) )
      //      {
      //        return MsoBlipType.msoblipWMF;
      //      }
      else
      {
        return MsoBlipType.msoblipPNG;
        //return MsoBlipType.msoblipUNKNOWN;
      }
    }
    /// <summary>
    /// Converts image format into blip type.
    /// </summary>
    /// <param name="format">Format to convert.</param>
    /// <param name="imageFormat">Desired image format.</param>
    /// <returns></returns>
    public static MsoBlipType ImageFormatToBlipType( ImageFormat format, ExcelImageFormat imageFormat )
    {
      MsoBlipType result = ImageFormatToBlipType( format );

      if( imageFormat == ExcelImageFormat.Original )
      {
        return result;
      }
      else
      {
        return ( MsoBlipType )imageFormat;
      }
    }
    /// <summary>
    /// Indicates whether blip is bitmap blip.
    /// </summary>
    /// <param name="blipType">Blip type to check.</param>
    /// <returns>True if specified blip is bitmap blip; False otherwise.</returns>
    public static bool IsBitmapBlip( MsoBlipType blipType )
    {
      return Array.IndexOf( METAFILEBLIPS, blipType ) == -1;
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Returns BlipParams for picture record.
    /// </summary>
    /// <param name="bse">MsofbtBSE record to set fields.</param>
    /// <returns>BlipParams for picture record.</returns>
    [ CLSCompliant( false ) ]
    protected static BlipParams GetBlipParams( MsofbtBSE bse )
    {
      MsoBlipType blipType = bse.BlipType;
      BlipParams blipParams = ( s_hashBlipTypeToParams.ContainsKey( blipType ) ) ?
        s_hashBlipTypeToParams[ blipType ] :
        s_hashBlipTypeToParams[ MsoBlipType.msoblipPNG ];

      return blipParams;
    }
    #endregion
  }
}

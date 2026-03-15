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
using System.Text;

using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords;
using Syncfusion.XlsIO.Implementation.Collections;

using FOPTE = Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing.MsofbtOPT.FOPTE;
using System.IO;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  SILVERLIGHT
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
using System.Drawing;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
using System.Drawing;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation.Shapes
{
  /// <summary>
  /// Class used for Bitmap shape implementation.
  /// </summary>
  public class BitmapShapeImpl
    : ShapeImpl
    , IShape
    , IParentApplication
    , IDisposable
    , IPictureShape
  {
    #region Consntants
    /// <summary>
    /// Instance field value for this shape.
    /// </summary>
    public const int ShapeInstance = 75;
    #endregion

    #region Class members
    /// <summary>
    /// Represents Blip id.
    /// </summary>
    private uint m_uiBlipId;
    /// <summary>
    /// Name of the blip file.
    /// </summary>
    private string m_strBlipFileName;
    /// <summary>
    /// Shape's picture (maybe should be moved to BitmapShape class).
    /// </summary>
    private MsofbtBSE m_picture;
    /// <summary>
    /// Represents a bitmap image.
    /// </summary>
    private Image m_bitmap;
    
    private Stream m_bitmapStream;
    /// <summary>
    /// Stream that contains subnodes of the blip xml node in Excel 2007 format.
    /// </summary>
    private Stream m_streamBlipSubNodes;
    /// <summary>
    /// Stream that contains shape properties xml-tag in Excel 2007 format.
    /// </summary>
    private Stream m_streamShapeProperties;
    /// <summary>
    /// Stream that contains srcRect tag if shape was extracted from Excel 2007 file.
    /// </summary>
    private Stream m_srcRectStream;
    /// <summary>
    /// Macro name associated with this shape.
    /// </summary>
    private string m_strMacro;
    private bool m_bDDE;
    private bool m_bCamera;
    protected MsoOptions[] cropOptions = new MsoOptions[]
    {
        MsoOptions.CropFromBottom,
        MsoOptions.CropFromLeft,
        MsoOptions.CropFromRight,
        MsoOptions.CropFromTop,
      };
    private int m_cropLeftOffset=0;
    private int m_cropRightOffset=0;
    private int m_cropBottomOffset=0;
    private int m_cropTopOffset = 0;
      /// <summary>
      /// Refer <clrChange></clrChange> in Open XML specification for more details:
      /// TODO: Need to add support transparancy details.
      /// </summary>
    private bool m_hasTransparentDetails;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the BitmapShapeImpl class.
    /// </summary>
    /// <param name="application">Current application.</param>
    /// <param name="parent">Parent object</param>
    public BitmapShapeImpl( IApplication application, object parent )
      : this( application, parent, true )
    {
    }
    /// <summary>
    /// Initializes a new instance of the BitmapShapeImpl class.
    /// </summary>
    /// <param name="application">Current application.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="IncludeShapeOptions">Indicates is shape include options. False supports only for header / footer image.</param>
    public BitmapShapeImpl( IApplication application, object parent, bool IncludeShapeOptions )
      : base( application, parent )
    {
      m_bSupportOptions = true;

      if( IncludeShapeOptions )
      {
        m_bUpdateLineFill = true;
        Fill.Visible = false;
        Line.Visible = false;
      }
            
      ShapeType = ExcelShapeType.Picture;
    }
    /// <summary>
    /// Initializes a new instance of the BitmapShapeImpl class.
    /// </summary>
    /// <param name="application">Current application.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="records">Represents array of records.</param>
    /// <param name="index">Represent index.</param>
    [ CLSCompliant( false ) ]
    public BitmapShapeImpl( IApplication application, object parent, MsoBase[] records, int index )
      : base( application, parent, records, index )
    {
      m_bSupportOptions = true;
//      Fill.Visible = false;
//      Line.Visible = false;
      ShapeType = ExcelShapeType.Picture;

      m_bitmap = m_picture.PictureRecord.Picture;
    }
    /// <summary>
    /// Initializes a new instance of the BitmapShapeImpl class.
    /// </summary>
    /// <param name="application">Current application.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="container">Represents container.</param>
    [ CLSCompliant( false ) ]
    public BitmapShapeImpl( IApplication application, object parent, MsofbtSpContainer container )
      : base( application, parent, container, ExcelParseOptions.Default )
    {
      m_bSupportOptions = true;
      ShapeType = ExcelShapeType.Picture;

      if( m_picture != null )
      {
        m_bitmap = m_picture.PictureRecord.Picture;
      }
    }
    #endregion

    #region Class properties
      /// <summary>
      /// Used for internal purpose.
      /// </summary>
    public bool HasTransparency
    {
        get
        {
            return m_hasTransparentDetails;
        }
        set
        {
            m_hasTransparentDetails = value;
        }
    }
    /// <summary>
    /// Gets or sets Blip file name
    /// </summary>
    public string FileName
    {
      get
      {
        return m_strBlipFileName;
      }
      set
      {
        m_strBlipFileName = value;
      }
    }
    /// <summary>
    /// Gets or sets Blip id.
    /// </summary>
    [ CLSCompliant( false ) ]
    public uint BlipId
    {
      get
      {
        return m_uiBlipId;
      }
      set
      {
        m_uiBlipId = value;
        m_picture = ( MsofbtBSE )m_shapes.ShapeData.Pictures[ ( int )( value - 1 ) ];
        m_bitmap = m_picture.PictureRecord.Picture;
#if !SILVERLIGHT && !WINRT && !WP
        m_bitmapStream = m_picture.PictureRecord.PictureStream;
#endif
      }
    }
      /// <summary>
      /// In and above MS Excel 2007 format, this value should be divided by
      /// 1000 to get the left crop percentage of actual image size.
      /// </summary>
    internal Int32 CropLeftOffset
    {
        get
        {
            return m_cropLeftOffset;
        }
        set
        {
            m_cropLeftOffset = value;
        }
    }
    /// <summary>
    /// In and above MS Excel 2007 format, this value should be divided by
    /// 1000 to get the right crop percentage of actual image size.
    /// </summary>
    internal Int32 CropRightOffset
    {
        get
        {
            return m_cropRightOffset;
        }
        set
        {
            m_cropRightOffset = value;
        }
    }
    /// <summary>
    /// In and above MS Excel 2007 format, this value should be divided by
    /// 1000 to get the bottom crop percentage of actual image size.
    /// </summary>
    internal Int32 CropBottomOffset
    {
        get
        {
            return m_cropBottomOffset;
        }
        set
        {
            m_cropBottomOffset = value;
        }
    }
    /// <summary>
    /// In and above MS Excel 2007 format, this value should be divided by
    /// 1000 to get the top crop percentage of actual image size.
    /// </summary>
    internal Int32 CropTopOffset
    {
        get
        {
            return m_cropTopOffset;
        }   
        set
        {
            m_cropTopOffset = value;
        }
    }
    /// <summary>
    /// Gets or sets picture.
    /// </summary>
    public Image Picture
    {
      get
      {
#if !SILVERLIGHT && !WINRT && !WP
          if(m_bitmapStream != null )
              m_bitmap = Image.FromStream( m_bitmapStream );
#endif
        return m_bitmap;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "Bitmap" );

        //m_bitmap = value;
        WorkbookShapeDataImpl shapeData = m_shapes.ShapeData;

        shapeData.RemovePicture( BlipId, true );
        int id = shapeData.AddPicture( value, ExcelImageFormat.Png, Name );
        BlipId = ( uint )id;
      }
    }
    /// <summary>
    /// Gets or sets stream that contains subnodes of the blip xml node in Excel 2007 format.
    /// </summary>
    public Stream BlipSubNodesStream
    {
      get
      {
        return m_streamBlipSubNodes;
      }
      set
      {
        m_streamBlipSubNodes = value;
      }
    }
    /// <summary>
    /// Gets or sets Stream that contains shape properties xml-tag in Excel 2007 format.
    /// </summary>
    public Stream ShapePropertiesStream
    {
      get
      {
        return m_streamShapeProperties;
      }
      set
      {
        m_streamShapeProperties = value;
      }
    }
    /// <summary>
    /// Gets or sets stream that contains srcRect tag if shape was extracted from Excel 2007 file.
    /// </summary>
    public Stream SourceRectStream
    {
      get
      {
        return m_srcRectStream;
      }
      set
      {
        m_srcRectStream = value;
      }
    }
    /// <summary>
    /// Returns instance value. Read-only.
    /// </summary>
    public override int Instance
    {
      get
      {
        return ( m_shape != null ) ?
          m_shape.Instance :
          ShapeInstance;
      }
    }
    /// <summary>
    /// Gets or sets macro name associated with this shape.
    /// </summary>
    public string Macro
    {
      get
      {
        return m_strMacro;
      }
      set
      {
        m_strMacro = value;
      }
    }
    public bool IsDDE
    {
      get
      {
        return m_bDDE;
      }
      set
      {
        m_bDDE = value;
      }
    }
    public bool IsCamera
    {
      get
      {
        return m_bCamera;
      }
      set
      {
        m_bCamera = value;
      }
    }

    #endregion

    #region Class Parse methods
    /// <summary>
    /// Checks Blip parse option.
    /// </summary>
    /// <param name="option">Represents option.</param>
    /// <returns>Value indicating parse option.</returns>
    [ CLSCompliant( false ) ]
    protected override bool ParseOption( FOPTE option )
    {
      if( base.ParseOption ( option )) return true;

      switch( option.Id )
      {
        case MsoOptions.BlipId:
          ParseBlipId( option );
          return true;

        case MsoOptions.BlipName:
          ParseBlipName( option );
          return true;

          case MsoOptions.CropFromTop:
          case MsoOptions.CropFromRight:
          case MsoOptions.CropFromLeft:
          case MsoOptions.CropFromBottom:
          ParseCropRectangle(option);
          return true;

          return true;
      }

      return false;

    }
    protected void ParseCropRectangle(FOPTE option)
    {
        if (option == null)
            throw new ArgumentNullException("option");
        if(!(Array.IndexOf(cropOptions,option.Id) >=0))
            throw new ArgumentOutOfRangeException("Crop option expected");

        switch (option.Id)
        {
          case MsoOptions.CropFromBottom:
          m_cropBottomOffset = option.Int32Value+(option.Int32Value/2);
          break;
          case MsoOptions.CropFromLeft:
          m_cropLeftOffset = option.Int32Value + (option.Int32Value / 2);
          break;
          case MsoOptions.CropFromRight:
          m_cropRightOffset = option.Int32Value + (option.Int32Value / 2);
          break;
          case MsoOptions.CropFromTop:
          m_cropTopOffset = option.Int32Value + (option.Int32Value / 2);
          break;
        }
    }
    /// <summary>
    /// Parses blip id option.
    /// </summary>
    /// <param name="option">Option to parse.</param>
    [ CLSCompliant( false ) ]
    protected virtual void ParseBlipId( FOPTE option )
    {
      if( option == null )
        throw new ArgumentNullException( "option" );

      if( option.Id != MsoOptions.BlipId )
        throw new ArgumentOutOfRangeException( "BlipId option expected" );

      m_uiBlipId = option.UInt32Value;
      IList list = m_shapes.ShapeData.Pictures;

      m_picture = ( m_uiBlipId > 0 ) ?
        ( MsofbtBSE )list[ ( int )( m_uiBlipId - 1 ) ] :
        null;
    }
    /// <summary>
    /// Parses blip name option.
    /// </summary>
    /// <param name="option">Option to parse.</param>
    [ CLSCompliant( false ) ]
    protected virtual void ParseBlipName( FOPTE option )
    {
      if( option == null )
        throw new ArgumentNullException( "option" );

      if( option.Id != MsoOptions.BlipName )
        throw new ArgumentOutOfRangeException( "BlipName option expected" );

      //m_shapeType = ExcelShapeType.Picture;

      if( option.AdditionalData != null )
      {
        byte[] data = option.AdditionalData;
        m_strBlipFileName = Encoding.Unicode.GetString( data, 0, data.Length );
      }
    }
    /// <summary>
    /// Extract necessary option.
    /// </summary>
    /// <param name="option">Option to extract.</param>
    /// <returns>value indicating extracted option.</returns>
    [ CLSCompliant( false ) ]
    protected override bool ExtractNecessaryOption( FOPTE option )
    {
      if( base.ExtractNecessaryOption( option ) )
        return true;

      switch( option.Id )
      {
        case MsoOptions.BlipId:
          ParseBlipId( option );
          return true;

        case MsoOptions.BlipName:
          ParseBlipName( option );
          return true;
      }

      return false;
    }
    #endregion

    #region IDisposable Members
    /// <summary>
    /// Dispose object.
    /// </summary>
    new public void Dispose()
    {
      base.Dispose();
    }
    #endregion

    #region Class Serialization methods
    /// <summary>
    /// Serializes shape.
    /// </summary>
    /// <param name="spgrContainer"> Represents Spgr container</param>
    [ CLSCompliant( false ) ]
    protected override void SerializeShape( MsofbtSpgrContainer spgrContainer )
    {
      MsofbtSpContainer spContainer = ( MsofbtSpContainer  )MsoFactory.GetRecord(
        MsoRecords.msofbtSpContainer );//new MsofbtSpContainer( spgrContainer );
      
      spContainer.AddItem( m_shape );

      SerializeOptions( spContainer );
      SerializeClientAnchor( spContainer );
      SerializeClientData( spContainer );

      spgrContainer.AddItem( spContainer );
    }
    /// <summary>
    /// This method is called inside of PrepareForSerialization to make shape-dependent preparations.
    /// </summary>
    protected override void OnPrepareForSerialization()
    {
      if( m_shape == null )
      {
        m_shape = ( MsofbtSp )MsoFactory.GetRecord( MsoRecords.msofbtSp );//new MsofbtSp( spgrContainer );
        m_shape.Instance = ShapeInstance;
      }

      m_shape.IsHaveAnchor = true;
      m_shape.IsHaveSpt = true;
    }
    /// <summary>
    /// Serializes options if necessary.
    /// </summary>
    /// <param name="spContainer">Low level shapes container.</param>
    private void SerializeOptions( MsofbtSpContainer spContainer )
    {
      MsofbtOPT options = m_options;
      bool bIsNullOptions = options == null;

      if( bIsNullOptions )
      {
        options = CreateDefaultOptions();

        SerializeOptionSorted( options, MsoOptions.NoLineDrawDash, 0x080008 );
        SerializeOptionSorted( options, MsoOptions.NoFillHitTest, 0x100000 );
      }

      if( m_bUpdateLineFill )
        options = SerializeMsoOptions( options );

      FOPTE option = new FOPTE();
      option.Id = MsoOptions.BlipId;
      option.UInt32Value = m_uiBlipId;
      option.IsValid = true;
      option.IsComplex = false;
      //options.AddOptions( option );
      options.AddOptionSorted( option );

      option = new FOPTE();
      option.Id = MsoOptions.BlipName;
      
      //      m_strBlipFileName = "testSmall_Green";
  
      if( m_strBlipFileName != null )
      {
        if( m_strBlipFileName[ m_strBlipFileName.Length - 1 ] != '\0' )
        {
          m_strBlipFileName += '\0';
        }

        option.UInt32Value = ( uint )( m_strBlipFileName.Length * 2 );
        option.IsValid = true;
        option.IsComplex = true;
        
        option.AdditionalData = Encoding.Unicode.GetBytes( m_strBlipFileName );
        //options.AddOptions( option );
        options.AddOptionSorted( option );
      }

      //if( m_bUpdateLineFill  )
        SerializeShapeName( options );
        SerializeName( options, MsoOptions.AlternativeText, AlternativeText );

      options.Version = 3;
      options.Instance = 2;
      spContainer.AddItem( options );
    }
    /// <summary>
    /// Serializes client anchor if necessary.
    /// </summary>
    /// <param name="spContainer">Low level shapes container.</param>
    private void SerializeClientAnchor( MsofbtSpContainer spContainer )
    {
      if( spContainer == null )
        throw new ArgumentNullException( "spContainer" );

      //ClientAnchor.Options = 2;
      spContainer.AddItem( ClientAnchor );
    }
    /// <summary>
    /// Serializes client data if necessary.
    /// </summary>
    /// <param name="spContainer">Low level shapes container.</param>
    private void SerializeClientData( MsofbtSpContainer spContainer )
    {
      if( IsShortVersion ) return;

      if( spContainer == null )
        throw new ArgumentNullException( "spContainer" );

      MsofbtClientData clientData = ( MsofbtClientData )MsoFactory.GetRecord(
        MsoRecords.msofbtClientData ); //new MsofbtClientData( spContainer );

      OBJRecord obj = Obj;
      ftCmo cmo;
      
      if( obj == null )
      {
        obj = ( OBJRecord )BiffRecordFactory.GetRecord( TBIFFRecord.OBJ );
      
        cmo = new ftCmo();

        cmo.ObjectType = TObjType.otPicture;
        cmo.Printable = true;
        cmo.Locked = true;
        cmo.AutoFill = true;
        cmo.AutoLine = true;

        //      m_book.CurrentObjectId++;

        ftEnd end = new ftEnd();

        obj.AddSubRecord( cmo );
        obj.AddSubRecord( end );
      }
      else
      {
        cmo = ( ftCmo )obj.Records[ 0 ];
      }

      cmo.ID = ( OldObjId > 0 ) ? ( ushort )OldObjId : ( ushort )ParentWorkbook.CurrentObjectId;

      clientData.AddRecord( obj );
      spContainer.AddItem( clientData );
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Registers shape in all required sub collections.
    /// </summary>
    public override void RegisterInSubCollection()
    {
      m_shapes.WorksheetBase.InnerPictures.AddPicture( this );
    }
    /// <summary>
    /// This method is called when shapes is deleted.
    /// </summary>
    protected override void OnDelete()
    {
      OnDelete( true );
    }
    /// <summary>
    /// This method is called when shapes is deleted.
    /// </summary>
    /// <param name="removeImage">Removes image that is referenced by this shape from collection too,
    /// if we didn't detect image usage. XlsIO doesn't detect this situation correctly in all cases
    /// if there are shapes in charts in Excel 2007 or if some image shapes are grouped in any excel version.
    /// If you are not sure whether image is referenced in charts or grouped shapes and you are working with
    /// Excel 2007 version, set this argument to true (this could cause file size increase, but will keep
    /// document in the correct state).</param>
    protected void OnDelete( bool removeImage )
    {
      base.OnDelete ();

      if( BlipId > 0 )
      {
        WorkbookImpl book = ParentShapes.Workbook;
        book.ShapesData.RemovePicture( BlipId, removeImage );
        m_uiBlipId = 0;
      }

      PicturesCollection pictures = ( PicturesCollection )m_shapes.Worksheet.Pictures;
      pictures.RemovePicture( ( IPictureShape )this );
    }

    /// <summary>
    /// Removes shape from the collection.
    /// </summary>
    /// <param name="removeImage">Removes image that is referenced by this shape from collection too,
    /// if we didn't detect image usage. XlsIO doesn't detect this situation correctly in all cases
    /// if there are shapes in charts in Excel 2007 or if some image shapes are grouped in any excel version.
    /// If you are not sure whether image is referenced in charts or grouped shapes and you are working with
    /// Excel 2007 version, set this argument to true (this could cause file size increase, but will keep
    /// document in the correct state).</param>
    public void Remove( bool removeImage )
    {
      OnDelete( removeImage );
      m_shapes.Remove( this );
    }
    /// <summary>
    /// Creates a clone of the current shape.
    /// </summary>
    /// <param name="parent">New parent for the shape object.</param>
    /// <param name="hashNewNames">Dictionary with new worksheet names.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <param name="addToCollection">Indicates whether we should add created
    /// shape into all necessary parent collections. This argument is ignored.</param>
    /// <returns>A copy of the current shape.</returns>
    public override IShape Clone( object parent, Dictionary<string, string> hashNewNames,
      Dictionary<int, int> dicFontIndexes, bool addToCollection )
    {
      WorksheetBaseImpl sheet;
      BitmapShapeImpl result;
      bool bIsPicture = true;

      ShapeCollectionBase parentShapes = ( ShapeCollectionBase  )FindParent( parent,
        typeof( ShapeCollectionBase ), true );

      ShapesCollection shapes = parentShapes as ShapesCollection;

      if( parentShapes != null )
      {
        sheet = parentShapes.WorksheetBase;
      }
      else
      {
        sheet = FindParent( parent, typeof( WorksheetBaseImpl ), true ) as WorksheetBaseImpl;
        bIsPicture = false;
      }

      WorkbookImpl book = ParentWorkbook;
      WorkbookImpl resultBook = sheet.ParentWorkbook;

      int iNewId = ( int )BlipId;

      if( bIsPicture )
        bIsPicture = !( parentShapes is HeaderFooterShapeCollection );

      WorkbookShapeDataImpl resultShapeData = ( bIsPicture )
        ? resultBook.ShapesData
        : resultBook.HeaderFooterData;

      if( resultBook != book )
      {
        WorkbookShapeDataImpl shapeData = ( bIsPicture )
          ? book.ShapesData
          : book.HeaderFooterData;

        MsofbtBSE bse = shapeData.GetPicture( iNewId );

        iNewId = resultShapeData.AddPicture( ( MsofbtBSE )bse.Clone() );
      }

      if( bIsPicture || !addToCollection )
      {
        result = ( BitmapShapeImpl )MemberwiseClone();
        result.SetParent( parent );//sheet.InnerShapes );
        result.SetParents();
        //        result = sheet.InnerShapes.AddPicture( iNewId, FileName );
        result.CopyFrom( this, hashNewNames, dicFontIndexes );
        result.CloneLineFill( this );

        if( addToCollection )
          sheet.InnerShapes.AddPicture( result );

        if( iNewId > 0 )
        {
          result.BlipId = ( uint )iNewId;
          MsofbtBSE bse = ( MsofbtBSE )resultShapeData.Pictures[ iNewId - 1 ];
          bse.RefCount++;
        }
      }
      else
      {
#if !SILVERLIGHT && !WINRT && !WP
        if( !bIsPicture )
        {
          result = sheet.HeaderFooterShapes.SetPicture( Name, Picture, iNewId, false,null ) as BitmapShapeImpl;
          result.m_options = ( MsofbtOPT )CloneUtils.CloneCloneable( m_options );
          result.m_srcRectStream = CloneUtils.CloneStream( m_srcRectStream );
          result.m_streamBlipSubNodes = CloneUtils.CloneStream( m_streamBlipSubNodes );
          result.m_streamShapeProperties = CloneUtils.CloneStream( m_streamShapeProperties );
          result.AttachEvents();
        }
        else
        {
          throw new NotImplementedException();
        }
#else
        throw new NotSupportedException();
#endif
      }
      if (this.ImageRelation != null)
          result.ImageRelation = (XmlSerialization.Relation)ImageRelation.Clone();
      return result;
    }
    /// <summary>
    /// Updates mso object.
    /// </summary>
    /// <param name="mso">Represents mso object to update.</param>
    /// <returns>Returns true if updated otherwise - false.</returns>
    [ CLSCompliant( false ) ]
    protected override bool UpdateMso( MsoBase mso )
    {
      if( base.UpdateMso( mso ) )
        return true;

      if( mso is MsofbtBSE )
      {
        m_picture = mso as MsofbtBSE;
        m_bitmap = m_picture.PictureRecord.Picture;

        return true;
      }

      return false;
    }
    /// <summary>
    /// Generates default shape name and sets it.
    /// </summary>
    public override void GenerateDefaultName()
    {
      this.Name = CollectionBaseEx<IShape>.GenerateDefaultName( m_shapes, ShapesCollection.DefaultPictureNameStart );
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Sets Blip id.
    /// </summary>
    /// <param name="newId">new Blip id</param>
    [ CLSCompliant( false ) ]
    public void SetBlipId( uint newId )
    {
      m_uiBlipId = newId;
    }
    #endregion
  }
}

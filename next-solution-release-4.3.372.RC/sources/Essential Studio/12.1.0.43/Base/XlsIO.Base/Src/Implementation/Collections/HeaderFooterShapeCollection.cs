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
using System.Diagnostics;
using System.Collections;

using Syncfusion.XlsIO.Implementation.Shapes;

using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords;
using System.IO;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Rectangle=Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using System.Drawing;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using System.Drawing;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
	/// <summary>
	/// Summary description for HeaderFooterShapesCollection.
	/// </summary>
	public class HeaderFooterShapeCollection : ShapeCollectionBase
	{
    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public HeaderFooterShapeCollection( IApplication application, object parent )
      : base( application, parent )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    /// <param name="container"></param>
    /// <param name="options">Parse options.</param>
    [ CLSCompliant( false ) ]
    public HeaderFooterShapeCollection( IApplication application, object parent, MsofbtSpgrContainer container
      , ExcelParseOptions options )
      : base( application, parent, container, options )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Code of the Biff record in which all data is stored. Read-only.
    /// </summary>
    public override TBIFFRecord RecordCode
    {
      get
      {
        return TBIFFRecord.HeaderFooterImage;
      }
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Creates new shape object.
    /// </summary>
    /// <param name="objType">Object type to create.</param>
    /// <param name="shapeContainer">Shape container.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="subRecords">Subrecords of the shape's OBJRecord.</param>
    /// <param name="cmoIndex">Index to the cmo record inside subrecords.</param>
    [CLSCompliant( false )]
    protected override ShapeImpl CreateShape( TObjType objType, MsofbtSpContainer shapeContainer,
      ExcelParseOptions options, List<ObjSubRecord> subRecords, int cmoIndex )
    {
      ShapeImpl newShape = null;

      switch( objType )
      {
        case TObjType.otPicture:
          newShape = new BitmapShapeImpl( Application, this, shapeContainer );
          break;

        default:
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, objType, "Not supported shape type" );
          //                  newShape = new ShapeImpl( Application, this, shapeContainer, ExcelParseOptions.Default );
          break;
      }

      return newShape;
    }

    /// <summary>
    /// Converts array of records with break indexes into bytes sequence.
    /// </summary>
    /// <param name="stream">Stream to put record data into.</param>
    /// <param name="dgContainer">Container.</param>
    /// <param name="arrBreaks">List with break indexes.</param>
    /// <param name="arrRecords">List with records to serialize.</param>
    /// <returns>Corresponding byte array.</returns>
    [ CLSCompliant( false ) ]
    protected override void CreateData( Stream stream, MsofbtDgContainer dgContainer,
      List<int> arrBreaks, List<List<BiffRecordRaw>> arrRecords )
    {
      stream.Write( HeaderFooterImageRecord.DEF_WORKSHEET_RECORD_START, 0,
        HeaderFooterImageRecord.DEF_WORKSHEET_RECORD_START.Length );
      base.CreateData( stream, dgContainer, arrBreaks, arrRecords );
    }

    /// <summary>
    /// Adds new shape to the collection.
    /// </summary>
    /// <param name="shapeContainer">Shape container to add.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Newly added shape.</returns>
    [ CLSCompliant( false ) ]
    protected override ShapeImpl AddShape( MsofbtSpContainer shapeContainer, ExcelParseOptions options )
    {
      ShapeImpl newShape = null;
      List<MsoBase> items = shapeContainer.ItemsList;

      newShape = CreateShape( TObjType.otPicture, shapeContainer, options, null, -1 );

//      if( newShape == null )
//        newShape = new ShapeImpl( Application, this, shapeContainer, ExcelParseOptions.Default );

      return AddShape( newShape );
    }

    /// <summary>
    /// Returns shared shape data for all shapes in this collection. Read-only.
    /// </summary>
    public override WorkbookShapeDataImpl ShapeData
    {
      get
      {
        return Workbook.HeaderFooterData;
      }
    }
    /// <summary>
    /// Registers in the parent worksheet.
    /// </summary>
    protected override void RegisterInWorksheet()
    {
      WorksheetBase.InnerHeaderFooterShapes = this;
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Parses record with image data.
    /// </summary>
    /// <param name="record">Record to parse.</param>
    /// <param name="options">Parse options.</param>
    [ CLSCompliant( false ) ]
    public void Parse( HeaderFooterImageRecord record, ExcelParseOptions options )
    {
      //MsoBase record = ( MsoBase )record.StructuresList[ 0 ];
      ParseMsoStructures( record.StructuresList, options );
    }
    /// <summary>
    /// Sets picture.
    /// </summary>
    /// <param name="strShapeName">Shape name.</param>
    /// <param name="image">Image to set.</param>
    public ShapeImpl SetPicture( string strShapeName, Image image )
    {
      return SetPicture( strShapeName, image, -1 );
    }
    /// <summary>
    /// Sets picture.
    /// </summary>
    /// <param name="strShapeName">Shape name.</param>
    /// <param name="image">Image to set.</param>
    /// <param name="iIndex">Represents new shape blip id index. If set -1 - auto indicate.</param>
    public ShapeImpl SetPicture( string strShapeName, Image image, int iIndex )
    {
      return SetPicture( strShapeName, image, iIndex, true,null );
    }
    /// <summary>
    /// Sets picture.
    /// </summary>
    /// <param name="strShapeName">Shape name.</param>
    /// <param name="image">Image to set.</param>
    /// <param name="iIndex">Represents new shape blip id index. If set -1 - auto indicate.</param>
    /// <param name="bIncludeOptions">Indicates is current picture include options.</param>
    public ShapeImpl SetPicture( string strShapeName, Image image, int iIndex, bool bIncludeOptions ,string preservedStyles)
    {
      if( strShapeName == null )
        throw new ArgumentNullException( "strShapeName" );

      if( strShapeName.Length == 0 )
        throw new ArgumentException( "strShapeName - string cannot be empty." );

      ShapeImpl result = null;
      BitmapShapeImpl shape = this[ strShapeName ] as BitmapShapeImpl;
      bool bContains = ( shape != null );
      WorkbookShapeDataImpl shapeData = ShapeData;

      if( shape != null )
      {
        uint id = shape.BlipId;
        shapeData.RemovePicture( id, true );
      }

      if( image != null )
      {
        if( !bContains )
          shape = new BitmapShapeImpl( Application, this, bIncludeOptions );

        int id = ( iIndex != -1 )
          ? iIndex
          : shapeData.AddPicture( image, ExcelImageFormat.Original/*Png*/, strShapeName );

        shape.BlipId = ( uint )id;
        shape.SetName( strShapeName );
        //shape.SetOption( MsoOptions.LockAgainstGrouping, 16777472 );
        //shape.SetOption( MsoOptions.SizeTextToFitShape, FormControlShapeImpl.DEF_SIZETEXT_VALUE );
        //shape.FileName = strFileName;
        shape.IsShortVersion = true;
#if  SILVERLIGHT || WINRT || WP
        shape.ClientAnchor.TopRow = image.Height;
        shape.ClientAnchor.LeftColumn = image.Width;
#else
        double inch = ApplicationImpl.ConvertToPixels( 1, MeasureUnits.Inch );
        shape.ClientAnchor.TopRow = ( int )Math.Round( image.Height * inch / image.VerticalResolution );
        shape.ClientAnchor.LeftColumn = ( int )Math.Round( image.Width * inch / image.HorizontalResolution );
#endif
        shape.VmlShape = true;
        if (preservedStyles != null && preservedStyles.Length > 0)
            shape.PreserveStyleString = preservedStyles;

        result = AddShape(shape);

      }
      else if( bContains )
      {
        Remove( shape );
      }

      return result;
    }
    #endregion
	}
}

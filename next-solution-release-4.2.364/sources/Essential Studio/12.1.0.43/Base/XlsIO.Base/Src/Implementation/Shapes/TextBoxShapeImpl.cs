#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser;
using System.IO;
using System.Collections;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Collections;
using System.Drawing;
#elif !(WINRT )
using System.Drawing;
using System.Drawing.Imaging;
using Syncfusion.XlsIO.Implementation.Collections;
#endif


namespace Syncfusion.XlsIO.Implementation.Shapes
{
  /// <summary>
  /// This class represents TextBox form control.
  /// </summary>
  public class TextBoxShapeImpl :
    TextBoxShapeBase,
    ITextBoxShapeEx
  {
    #region Constants
    /// <summary>
    /// Correct shape instance.
    /// </summary>
    private const int ShapeInstance = 202;
    /// <summary>
    /// Correct shape version.
    /// </summary>
    private const int ShapeVersion = 2;
    /// <summary>
    /// Represents embedded string.
    /// </summary>
    internal const string EmbedString = "Forms.TextBox.1";
    #endregion

    #region Members
    /////// <summary>
    /////// Metafile that contains control's image.
    /////// </summary>
    ////private int m_iImageIndex = -1;
    ///// <summary>
    ///// Textbox data.
    ///// </summary>
    //private MsofbtClientTextBox m_textBoxData;
    //private Metafile m_image;
    ///// <summary>
    ///// Control's properties.
    ///// </summary>
    //private ControlProperties m_properties = new ControlProperties();
    private Rectangle m_2007Coordinates = new Rectangle( 0, 1, 2076450, 1557338 );
    private string m_id;
    private string m_type;
    /// <summary>
    /// Represents a formula linking to spreadsheet cell data
    /// </summary>
    private string m_textLink;
    #endregion

    #region Properties
    public Rectangle Coordinates2007
    {
      get
      {
        return m_2007Coordinates;
      }
      set
      {
        m_2007Coordinates = value;
      }
    }
    public string FieldId
    {
        get
        {
            return m_id;
        }
        set
        {
            m_id = value;
        }
    }
    public string FieldType
    {
        get
        {
            return m_type;
        }
        set
        {
            m_type = value;
        }
    }
    /// <summary>
    /// Specifies a formula linking to spreadhseet cell data
    /// </summary>
    public string TextLink
    {
        get
        {
            return m_textLink;
        }
        set
        {
            if (value != null && value.Length > 0 && value[0] == '=')
                m_textLink = value;
            else
                throw new ArgumentException("Refrence is not valid");
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initialize new instance of the text box shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    public TextBoxShapeImpl( IApplication application, object parent, WorksheetImpl sheet )
      : base( application, parent )
    {
      //m_textBoxData = ( MsofbtClientTextBox )MsoFactory.GetRecord( MsoRecords.msofbtClientTextbox );
      //m_textBoxData.TextObject = ( TextObjectRecord )BiffRecordFactory.GetRecord( TBIFFRecord.TextObject );
      ShapeType = ExcelShapeType.TextBox;
      Fill.ForeColor = ColorExtension.White;
      Line.ForeColor = ColorExtension.DarkGray;
      Line.BackColor = ColorExtension.DarkGray;
      //Name = "TextBox2";
      m_sheet = sheet;
    }
    /// <summary>
    /// Initializes new instance of the text box shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    /// <param name="shapeContainer">Shape's container.</param>
    /// <param name="options">Parsing options.</param>
    [ CLSCompliant( false ) ]
    public TextBoxShapeImpl( IApplication application, object parent, MsofbtSpContainer shapeContainer,
      ExcelParseOptions options )
      : base( application, parent, shapeContainer, options )
    {
      ShapeType = ExcelShapeType.TextBox;
      //m_iImageIndex = m_options.IndexOf( MsoOptions.BlipId );

      //if( m_options.PropertyList.Count <= m_iImageIndex )
      //  m_iImageIndex = -1;
    }
    /// <summary>
    /// Initializes shape items.
    /// </summary>
    private void InitializeShape()
    {
      //InitializeVariables();
      ShapeType = ExcelShapeType.TextBox;

      m_bUpdateLineFill = true;
      //Fill.ForeColor = ShapeFillImpl.DEF_COMENT_PARSE_COLOR;
      //Line.ForeColor = ColorExtension.Black;

      //m_strAuthor = m_shapes.Worksheet.Workbook.Author;
      //this.IsMoveWithCell = false;
      //this.IsSizeWithCell = false;
    }
    /// <summary>
    /// This method is called inside of PrepareForSerialization to make shape-dependent preparations.
    /// </summary>
    protected override void OnPrepareForSerialization()
    {
      m_shape = ( MsofbtSp )MsoFactory.GetRecord( MsoRecords.msofbtSp );
      m_shape.Version = ShapeVersion;
      m_shape.Instance = ShapeInstance;
      m_shape.IsHaveAnchor = true;
      m_shape.IsHaveSpt = true;
      //m_shape.IsOleShape = true;

      // Unknown values, numbers are not accurate.
      //m_properties.Width = ( int )( Width * 3810 / 144.0 );
      //m_properties.Height = ( int )( Height * 3810 / 144.0 );

      //if( m_iImageIndex < 0 )
      //  SerializeImage();
    }
    /// <summary>
    /// Serializes current shape.
    /// </summary>
    /// <param name="spgrContainer">Container that will receive this comment.</param>
    [CLSCompliant( false )]
    protected override void SerializeShape( MsofbtSpgrContainer spgrContainer )
    {
      if( spgrContainer == null )
        throw new ArgumentNullException( "spgrContainer" );

      MsofbtSpContainer spContainer = ( MsofbtSpContainer )MsoFactory.GetRecord(
        MsoRecords.msofbtSpContainer );//new MsofbtSpContainer( spgrContainer );

      MsofbtClientData clientData = ( MsofbtClientData )MsoFactory.GetRecord(
        MsoRecords.msofbtClientData );

      ftCmo cmo = null;

      if( Obj == null )
      {
        OBJRecord obj = ( OBJRecord )BiffRecordFactory.GetRecord( TBIFFRecord.OBJ );

        cmo = new ftCmo();

        cmo.ObjectType = TObjType.otText;
        cmo.Printable = true;

        cmo.Locked = true;
        cmo.AutoLine = true;
        ftEnd end = new ftEnd();

        obj.AddSubRecord( cmo );
        obj.AddSubRecord( end );
        clientData.AddRecord( obj );
      }
      else
      {
        cmo = Obj.RecordsList[ 0 ] as ftCmo;
        clientData.AddRecord( Obj );
      }

      cmo.ID = ( OldObjId > 0 ) ? ( ushort )OldObjId : ( ushort )ParentWorkbook.CurrentObjectId;
      spContainer.AddItem( m_shape );

      MsofbtOPT options = SerializeOptions( spContainer );

      if( options.Properties.Length > 0 )
        spContainer.AddItem( options );

      spContainer.AddItem( ClientAnchor );
      spContainer.AddItem( clientData );
      //spContainer.AddItem( m_textBoxData );

      if( Text.Length > 0 )
        spContainer.AddItem( GetClientTextBoxRecord( spContainer ) );

      spgrContainer.AddItem( spContainer );
    }
    ///// <summary>
    ///// Serializes comment's options.
    ///// </summary>
    ///// <param name="parent">Parent record for options.</param>
    ///// <returns>All options in MsofbtOPT record.</returns>
    //[CLSCompliant( false )]
    //protected override MsofbtOPT SerializeOptions( MsoBase parent )
    //{
    //  if( m_bUpdateLineFill || m_options == null )
    //  {
    //    // TODO: uncomment one of the lines
    //    //MsofbtOPT result = base.SerializeOptions( parent );
    //    m_options = CreateDefaultOptions();

    //    //if( m_iImageIndex == -1 )
    //    //  m_iImageIndex = SerializeImage();

    //    //MsofbtOPT.FOPTE option = new MsofbtOPT.FOPTE();
    //    //option.Id = MsoOptions.BlipId;
    //    //option.Int32Value = m_iImageIndex;
    //    //m_options.AddOptionsOrReplace( option );

    //    MsofbtOPT result = SerializeMsoOptions( m_options );

    //    return result;
    //  }

    //  return m_options;
    //}
    /// <summary>
    /// Creates default shape options.
    /// </summary>
    /// <returns>Record with default option specified.</returns>
    [CLSCompliant( false )]
    protected override MsofbtOPT CreateDefaultOptions()
    {
      MsofbtOPT result = base.CreateDefaultOptions();
      result.Version = 3;
      result.Instance = 2;
      //SerializeTextId( result );

      //SerializeTextDirection( result );
      //SerializeOption( result, MsoOptions.TextId, 119315524 );
      //SerializeOption( result, MsoOptions.TextDirection, 2 );
      //SerializeSizeTextToFit( result );

      //MsofbtOPT.FOPTE option = SerializeOption( result, MsoOptions.BlipId, m_iImageIndex );
      //option.IsValid = true;

      //SerializeOption( result, MsoOptions.PictureId, 1 );//m_iImageIndex );
      //SerializeOption( result, MsoOptions.ForeColor, 134217793 );
      //SerializeOption( result, MsoOptions.LineColor, 134217792 );

      //string strName = Name;

      //SerializeShapeName( result );
      //SerializeName( result, MsoOptions.AlternativeText, AlternativeText );

      //SerializeOption( result, MsoOptions.NoLineDrawDash, 524288 );
      //SerializeShapeName( result );
      // ShapeName
      //SerializeOption344( result );

      return result;
    }
        /// <summary>
    /// Creates a clone of the current shape.
    /// </summary>
    /// <param name="parent">New parent for the shape object.</param>
    /// <param name="hashNewNames">Dictionary with new worksheet names.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <param name="addToCollections">Indicates whether we should add created
    /// shape into all necessary parent collections.</param>
    /// <returns>A copy of the current shape.</returns>
    public override IShape Clone( object parent, Dictionary<string, string> hashNewNames,
      Dictionary<int, int> dicFontIndexes, bool addToCollections )
    {
      TextBoxShapeImpl result = ( TextBoxShapeImpl )base.Clone( parent, hashNewNames, dicFontIndexes, addToCollections );

      if( addToCollections )
        ( result.Worksheet.TextBoxes as TextBoxCollection ).AddTextBox( result );

      return result;
    }
    #endregion
  }
}

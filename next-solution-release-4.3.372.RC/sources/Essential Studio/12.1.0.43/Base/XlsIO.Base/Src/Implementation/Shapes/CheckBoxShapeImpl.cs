#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
#if ( WINRT )
using Windows.UI;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#else

#endif


namespace Syncfusion.XlsIO.Implementation.Shapes
{
  /// <summary>
  /// This class represents Checkbox form control.
  /// </summary>
  public class CheckBoxShapeImpl :
    TextBoxShapeBase,
    ICheckBoxShape
  {
    #region Constants
    /// <summary>
    /// Correct shape instance.
    /// </summary>
    public const int ShapeInstance = 201;
    /// <summary>
    /// Correct shape version.
    /// </summary>
    private const int ShapeVersion = 2;
    #endregion

    #region Members
    /// <summary>
    /// Indicates whether check box is checked.
    /// </summary>
    private ExcelCheckState m_checkState;
    /// <summary>
    /// Stores formula link value.
    /// </summary>
    private IRange m_cellLinkRange;
    /// <summary>
    /// Indicates whether check box button is firstbutton.
    /// </summary>
    private bool m_display3DShading = false;
    #endregion

    #region Methods
    /// <summary>
    /// Initialize new instance of the text box shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    public CheckBoxShapeImpl( IApplication application, object parent )
      : base( application, parent )
    {
      //m_textBoxData = ( MsofbtClientTextBox )MsoFactory.GetRecord( MsoRecords.msofbtClientTextbox );
      //m_textBoxData.TextObject = ( TextObjectRecord )BiffRecordFactory.GetRecord( TBIFFRecord.TextObject );
      ShapeType = ExcelShapeType.TextBox;
      Fill.BackColor = ColorExtension.Empty;        
      Fill.ForeColor = ColorExtension.White;
      Line.ForeColor = ColorExtension.DarkGray;
      Line.BackColor = ColorExtension.Empty;      
      Fill.Transparency = 1.0;
      VmlShape = true;
      HasFill = false;
      //Name = "TextBox2";
    }
    /// <summary>
    /// Initializes new instance of the text box shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    /// <param name="shapeContainer">Shape's container.</param>
    /// <param name="options">Parsing options.</param>
    [ CLSCompliant( false ) ]
    public CheckBoxShapeImpl( IApplication application, object parent, MsofbtSpContainer shapeContainer,
      ExcelParseOptions options )
      : base( application, parent, shapeContainer, options )
    {
      ShapeType = ExcelShapeType.TextBox;
      VmlShape = true;
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
      OBJRecord obj = Obj;
      ftCbls cbls;
      ftCblsData cblsData;
      ftCblsFmla cblsFormula=null;

      if( obj == null )
      {
        obj = ( OBJRecord )BiffRecordFactory.GetRecord( TBIFFRecord.OBJ );

        cmo = new ftCmo();

        cmo.ObjectType = TObjType.otCheckBox;
        cmo.Printable = true;

        cmo.Locked = true;
        cmo.AutoLine = false;
        //cmo.Reserved = new byte[] { 0xEC, 0x88, 0xC8, 0x01, 0x14, 0x5A, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00 };
        ftEnd end = new ftEnd();

        cbls = new ftCbls();

        cblsData = new ftCblsData();
        obj.AddSubRecord( cmo );
        obj.AddSubRecord( cbls );

        if( LinkedCell != null )
        {
          cblsFormula = new ftCblsFmla();
          obj.AddSubRecord( cblsFormula );
        }

        obj.AddSubRecord( cblsData );
        obj.AddSubRecord( end );
      }
      else
      {
        cmo = obj.RecordsList[ 0 ] as ftCmo;
        cbls = ( ftCbls )FindSubRecord( obj.RecordsList, TObjSubRecordType.ftCbls );
        cblsData = ( ftCblsData )FindSubRecord( obj.RecordsList, TObjSubRecordType.ftCblsData );

        if( LinkedCell != null )
          cblsFormula = ( ftCblsFmla )FindSubRecord( obj.RecordsList, TObjSubRecordType.ftCblsFmla );
      }

      clientData.AddRecord( obj );
      cblsData.Display3DShading = Display3DShading;
      cblsData.CheckState = m_checkState;

      if (LinkedCell != null)
          cblsFormula.Formula = (LinkedCell as INativePTG).GetNativePtg();

      cmo.ID = ( OldObjId > 0 ) ? ( ushort )OldObjId : ( ushort )ParentWorkbook.CurrentObjectId;
      spContainer.AddItem( m_shape );

      MsofbtOPT options = SerializeOptions( spContainer );

      if( options.Properties.Length > 0 )
        spContainer.AddItem( options );

      if (ChildAnchor != null)
          spContainer.AddItem(ChildAnchor);
      else
          spContainer.AddItem(ClientAnchor);
      spContainer.AddItem( clientData );
      //spContainer.AddItem( m_textBoxData );
      IsTextLocked = false;
      MsofbtClientTextBox clientTextBox = GetClientTextBoxRecord( spContainer );
      //clientTextBox.TextObject.IsLockText = false;
      spContainer.AddItem( clientTextBox );
      spgrContainer.AddItem( spContainer );
    }

    private ObjSubRecord FindSubRecord( List<ObjSubRecord> records, TObjSubRecordType recordType )
    {
      ObjSubRecord result = null;

      for( int i = 0, len = records.Count; i < len; i++ )
      {
        ObjSubRecord current = records[ i ];

        if( current.Type == recordType )
        {
          result = current;
          break;
        }
      }

      return result;
    }
    /// <summary>
    /// Serializes comment's options.
    /// </summary>
    /// <param name="parent">Parent record for options.</param>
    /// <returns>All options in MsofbtOPT record.</returns>
    [CLSCompliant( false )]
    protected override MsofbtOPT SerializeOptions( MsoBase parent )
    {
      //if( m_bUpdateLineFill || m_options == null )
      //{
      //  // TODO: uncomment one of the lines
      //  //MsofbtOPT result = base.SerializeOptions( parent );
      //  m_options = CreateDefaultOptions();

      //  //if( m_iImageIndex == -1 )
      //  //  m_iImageIndex = SerializeImage();

      //  //MsofbtOPT.FOPTE option = new MsofbtOPT.FOPTE();
      //  //option.Id = MsoOptions.BlipId;
      //  //option.Int32Value = m_iImageIndex;
      //  //m_options.AddOptionsOrReplace( option );

      //  MsofbtOPT result = SerializeMsoOptions( m_options );

      //  return result;
      //}
      MsofbtOPT result = base.SerializeOptions( parent );

      MsofbtOPT.FOPTE option = new MsofbtOPT.FOPTE();
      //option.Id = ( MsoOptions )127;
      //option.Int32Value = 16777472;
      //result.AddOptionSorted( option );

      //option = new MsofbtOPT.FOPTE();
      //option.Id = ( MsoOptions )133;
      //option.Int32Value = 1;
      //result.AddOptionSorted( option );

      //option = new MsofbtOPT.FOPTE();
      //option.Id = MsoOptions.NoFillHitTest;
      //option.Int32Value = 1048576;
      //result.AddOptionSorted( option );

      option = new MsofbtOPT.FOPTE();
      option.Id = MsoOptions.SizeTextToFitShape;
      option.Int32Value = 1703944;
      result.AddOptionsOrReplace( option );

      return result;
    }
    /// <summary>
    /// Creates default options.
    /// </summary>
    /// <returns>MsofbtOPT record</returns>
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
    /// Parses client data record.
    /// </summary>
    /// <param name="clientData">Record to parse.</param>
    /// <param name="options">Parse options.</param>
    [CLSCompliant( false )]
    protected override void ParseClientData( MsofbtClientData clientData, ExcelParseOptions options )
    {
      base.ParseClientData( clientData, options );
      OBJRecord obj = clientData.ObjectRecord;
      List<ObjSubRecord> arrRecords = obj.RecordsList;

      for( int i = 0, len = arrRecords.Count; i < len; i++ )
      {
        ObjSubRecord subRecord = arrRecords[ i ];

        switch( subRecord.Type )
        {
          case TObjSubRecordType.ftCbls:
           
            break;
            case TObjSubRecordType.ftCblsFmla:
            IRangeGetter rangeGetter = ((ftCblsFmla) subRecord).Formula[0] as IRangeGetter;
             m_cellLinkRange = rangeGetter.GetRange( Workbook, Worksheet as IWorksheet );
            break;

            case TObjSubRecordType.ftCblsData:
            m_checkState = ((ftCblsData)subRecord).CheckState;
            m_display3DShading = ((ftCblsData)subRecord).Display3DShading;
            break;
        }
      }
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
      CheckBoxShapeImpl result = ( CheckBoxShapeImpl )base.Clone( parent, hashNewNames, dicFontIndexes, addToCollections );
      WorksheetBaseImpl newWorksheet = result.Worksheet;
      WorkbookImpl newBook = newWorksheet.ParentWorkbook;

      if( m_cellLinkRange != null )
        result.m_cellLinkRange = ( m_cellLinkRange as ICombinedRange ).Clone( newWorksheet, hashNewNames, newBook );

      if( addToCollections )
        ( result.Worksheet.CheckBoxes as Syncfusion.XlsIO.Implementation.Collections.CheckBoxCollection ).AddCheckBox( result );

      return result;
    }
    #endregion

    #region TextBoxSape Member
    /// <summary>
    /// Horizontal alignment of the text.
    /// </summary>
    public ExcelCommentHAlign HAlignment
    {
        get
        {
            throw new NotSupportedException("Alignment");
        }
        set
        {
            throw new NotSupportedException("Alignment");
        }
    }
    /// <summary>
    /// Vertical alignment of the text.
    /// </summary>
    public ExcelCommentVAlign VAlignment
    {
        get
        {
            throw new NotSupportedException("Alignment");
        }
        set
        {
            throw new NotSupportedException("Alignment");
        }
    }
    /// <summary>
    /// Text rotation.
    /// </summary>
    public ExcelTextRotation TextRotation
    {
        get
        {
            throw new NotSupportedException("Rotation");
        }
        set
        {
            throw new NotSupportedException("Rotation");
        }
    }
    #endregion

    #region ICheckBoxShape Members
    /// <summary>
    /// Indicates whether check box is checked.
    /// </summary>
    public ExcelCheckState CheckState
    {
      get
      {
        return m_checkState;
      }
      set
      {
          if (m_checkState != value)
          {
              m_checkState = value;
              if (m_cellLinkRange != null)
              {
                  m_cellLinkRange.Boolean = Convert.ToBoolean(GetCheckState(m_checkState));
              }
          }
      }
    }
    /// <summary>
    /// Gets or sets formula link value.
    /// </summary>
    public IRange LinkedCell
    {
        get
        {
            return m_cellLinkRange;
        }
        set
        {
            if (value != null && (value.Row != value.LastRow || value.Column != value.LastColumn))
                throw new ArgumentOutOfRangeException("LinkedCell must be single cell.");

            if (value != m_cellLinkRange)
            {
                string checkState = GetCheckState(m_checkState);
                m_cellLinkRange = value;
                m_cellLinkRange.Boolean  = Convert.ToBoolean(checkState);
                // Force external references creation.
                (m_cellLinkRange as INativePTG).GetNativePtg();
            }
            
        }
    }
    /// <summary>
    /// indicates whether the option button is in 3D shading
    /// </summary>
    public bool Display3DShading
    {
        get
        {
            return m_display3DShading;
        }
        set
        {
            m_display3DShading = value;
        }
    }

    #endregion

    #region Helper Methods
    private string GetCheckState( ExcelCheckState excelCheckState )
    {
      switch( excelCheckState )
      {
        case ExcelCheckState.Checked:
          return "TRUE";
        case ExcelCheckState.Unchecked:
          return "FALSE";
        case ExcelCheckState.Mixed:
          return "#N/A";
      }
      return null;
    }
    #endregion
  }
}

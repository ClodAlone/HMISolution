#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords;
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
  /// This class represents option button form control.
  /// </summary>
  public class OptionButtonShapeImpl :
    TextBoxShapeBase,
    IOptionButtonShape
  {
    #region Constants
    /// <summary>
    /// Correct shape instance.
    /// </summary>
    internal const int ShapeInstance = 201;
    /// <summary>
    /// Correct shape version.
    /// </summary>
    private const int ShapeVersion = 2;
    #endregion

    #region Members
    /// <summary>
    /// Static variable to represent a linked cell value
    /// </summary>
    private static double m_linkedCellValue;
    /// <summary>
    /// Static variable to represent index of objects
    /// </summary>
    private static int m_objIndex = 0;
    /// <summary>
    /// Indicates whether option button is checked.
    /// </summary>
    private ExcelCheckState m_checkState;
    /// <summary>
    /// Stores formula link value.
    /// </summary>
    private IRange m_cellLinkRange;
    /// <summary>
    /// Indicates whether option button is firstbutton.
    /// </summary>
    private bool m_isFirstButton = false;
    /// <summary>
    /// Indicates whether option button is firstbutton.
    /// </summary>
    private bool m_display3DShading = false;
    /// <summary>
    /// Indicates whether option button is firstbutton.
    /// </summary>
    private byte m_nextButton;
    /// <summary>
    /// Indicates whether the event invokes are not
    /// </summary>
    private bool m_invokeEvent;
    /// <summary>
    /// Represents the Option button index.
    /// </summary>
    private int m_iIndex;
    #endregion

    #region Events
    /// <summary>
    /// Raise when Check state Changed
    /// </summary>
    internal event Syncfusion.XlsIO.Implementation.ValueChangedEventHandler CheckStateChanged;
    #endregion

    #region Properties
    public bool InvokeEvent
    {
      get
      {
        return m_invokeEvent;
      }
      set
      {
        m_invokeEvent = value;
      }
    }
    /// <summary>
    /// Represents the Option button index.
    /// </summary>
    internal int Index
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
    /// Represents the Next Button in the Group.
    /// </summary>
    internal int NextButtonId
    {
        get
        {
            return m_nextButton;
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initialize new instance of the text box shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    public OptionButtonShapeImpl( IApplication application, object parent )
      : base( application, parent )
    {
      //m_textBoxData = ( MsofbtClientTextBox )MsoFactory.GetRecord( MsoRecords.msofbtClientTextbox );
      //m_textBoxData.TextObject = ( TextObjectRecord )BiffRecordFactory.GetRecord( TBIFFRecord.TextObject );
      ShapeType = ExcelShapeType.TextBox;
      Fill.ForeColor = ColorExtension.White;
      Line.ForeColor = ColorExtension.DarkGray;
      Line.BackColor = ColorExtension.DarkGray;
      FillColor = ColorExtension.Empty;
      Line.HasPattern = false;
      HasFill = false;
      AlternativeText = null;
      VmlShape = true;
      //Fill.Transparency = 1.0;
    }
    /// <summary>
    /// Initializes new instance of the option button shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    /// <param name="shapeContainer">Shape's container.</param>
    /// <param name="options">Parsing options.</param>
    [CLSCompliant( false )]
    public OptionButtonShapeImpl( IApplication application, object parent, MsofbtSpContainer shapeContainer,
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
    /// Initializes new instance of the option button shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    /// <param name="shapeContainer">Shape's container.</param>
    /// <param name="options">Parsing options.</param>
    [CLSCompliant(false)]
    public OptionButtonShapeImpl(IApplication application, object parent, MsofbtSpContainer shapeContainer,
      ExcelParseOptions options, int optionButtonId)
        : base(application, parent, shapeContainer, options)
    {
        ShapeType = ExcelShapeType.TextBox;
        VmlShape = true;
        m_iIndex = optionButtonId;
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
      ftRbo rbo;
      ftRboData rboData;
      ftCblsFmla cblsFormula = null;

      if( obj == null )
      {
        obj = ( OBJRecord )BiffRecordFactory.GetRecord( TBIFFRecord.OBJ );

        cmo = new ftCmo();

        cmo.ObjectType = TObjType.otOptionBtn;
        cmo.Printable = true;

        cmo.Locked = IsTextLocked;
        cmo.AutoLine = false;
        //cmo.Reserved = new byte[] { 0xEC, 0x88, 0xC8, 0x01, 0x14, 0x5A, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00 };
        ftEnd end = new ftEnd();

        cbls = new ftCbls();

        cblsData = new ftCblsData();
        cblsFormula = new ftCblsFmla();
        rbo = new ftRbo();
        rboData = new ftRboData();
        obj.AddSubRecord( cmo );
        obj.AddSubRecord( cbls );
        obj.AddSubRecord( rbo );

        if( IsFirstButton && LinkedCell != null )
          obj.AddSubRecord( cblsFormula );

        obj.AddSubRecord( cblsData );
        obj.AddSubRecord( rboData );


        obj.AddSubRecord( end );
      }
      else
      {
        cmo = obj.RecordsList[ 0 ] as ftCmo;
        cbls = ( ftCbls )FindSubRecord( obj.RecordsList, TObjSubRecordType.ftCbls );
        cblsData = ( ftCblsData )FindSubRecord( obj.RecordsList, TObjSubRecordType.ftCblsData );
        rbo = ( ftRbo )FindSubRecord( obj.RecordsList, TObjSubRecordType.ftRbo );
        rboData = ( ftRboData )FindSubRecord( obj.RecordsList, TObjSubRecordType.ftRboData );
        if( LinkedCell != null )
        {
          cblsFormula = ( ftCblsFmla )FindSubRecord( obj.RecordsList, TObjSubRecordType.ftCblsFmla );
          m_linkedCellValue = LinkedCell.Number;
        }

        if (this.IsFirstButton)
            m_objIndex = 0;

        m_objIndex++;

        if (m_linkedCellValue != null && m_linkedCellValue == m_objIndex)
          this.CheckState = ExcelCheckState.Checked;
      }

      clientData.AddRecord( obj );

      cblsData.CheckState = m_checkState;
      cblsData.Display3DShading = Display3DShading;

      rboData.IsFirstButton = IsFirstButton;
      rboData.NextButton = this.m_nextButton;

      if(cblsFormula!=null && cblsFormula.Formula==null && LinkedCell != null )
        cblsFormula.Formula = ( LinkedCell as INativePTG ).GetNativePtg();

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
      IsTextLocked = IsTextLocked;
      MsofbtClientTextBox clientTextBox = GetClientTextBoxRecord( spContainer );
      //clientTextBox.TextObject.IsLockText = false;
      spContainer.AddItem( clientTextBox );
      spgrContainer.AddItem( spContainer );
    }
    /// <summary>
    /// Finds the Sub records of option button
    /// </summary>
    /// <param name="records"></param>
    /// <param name="recordType"></param>
    /// <returns></returns>
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

      MsofbtOPT result = base.SerializeOptions( parent );

      MsofbtOPT.FOPTE option = new MsofbtOPT.FOPTE();
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
          case TObjSubRecordType.ftCblsData:
            m_checkState = ( ( ftCblsData )subRecord ).CheckState;
            m_display3DShading = ( ( ftCblsData )subRecord ).Display3DShading;
            break;

          case TObjSubRecordType.ftRboData:
            m_nextButton = ( ( ftRboData )subRecord ).NextButton;
            m_isFirstButton = ( ( ftRboData )subRecord ).IsFirstButton;
            break;

          case TObjSubRecordType.ftCblsFmla:
            m_cellLinkRange = ( ( ( ftCblsFmla )subRecord ).Formula[ 0 ] as IRangeGetter ).GetRange( ParentWorkbook, Worksheet as IWorksheet );
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
      OptionButtonShapeImpl result = ( OptionButtonShapeImpl )base.Clone( parent, hashNewNames, dicFontIndexes, addToCollections );
      WorksheetBaseImpl newWorksheet = result.Worksheet;
      WorkbookImpl newBook = newWorksheet.ParentWorkbook;

      if( m_cellLinkRange != null )
        result.m_cellLinkRange = ( m_cellLinkRange as ICombinedRange ).Clone( newWorksheet, hashNewNames, newBook );

      if( addToCollections )
        ( result.Worksheet.OptionButtons as Syncfusion.XlsIO.Implementation.Collections.OptionButtonCollection ).AddOptionButton( result );

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
        throw new NotSupportedException( "Alignment" );
      }
      set
      {
        throw new NotSupportedException( "Alignment" );
      }
    }
    /// <summary>
    /// Vertical alignment of the text.
    /// </summary>
    public ExcelCommentVAlign VAlignment
    {
      get
      {
        throw new NotSupportedException( "Alignment" );
      }
      set
      {
        throw new NotSupportedException( "Alignment" );
      }
    }
    /// <summary>
    /// Text rotation.
    /// </summary>
    public ExcelTextRotation TextRotation
    {
      get
      {
        throw new NotSupportedException( "Rotation" );
      }
      set
      {
        throw new NotSupportedException( "Rotation" );
      }
    }
    /// <summary>
    /// Rich Text
    /// </summary>
    public RichTextString RichText
    {
      set
      {
        throw new NotSupportedException( "Rich Text" );
      }
    }
    #endregion

    #region IOptionButtonShape Members
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
        if( value == ExcelCheckState.Mixed )
          throw new NotSupportedException( "Mixed state not Supported" );

        ExcelCheckState old = m_checkState;
        m_checkState = value;

        if (old != value && CheckStateChanged != null && m_invokeEvent)
        {
          CheckStateChanged( this, new ValueChangedEventArgs( old, value, Name ) );
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
        if( value != null && ( value.Row != value.LastRow || value.Column != value.LastColumn ) )
          throw new ArgumentOutOfRangeException( "LinkedCell must be single cell." );

        IRange old = m_cellLinkRange;
        string fullAddress = ( old != null ) ? old.AddressGlobal : null;
        string fullNewAddress = ( value != null ) ? value.AddressGlobal : null;
        m_cellLinkRange = value;

        if( fullAddress != fullNewAddress && m_invokeEvent && LinkedCellValueChanged != null )
        {
          LinkedCellValueChanged( this, new ValueChangedEventArgs( old, value, Name ) );
        }

        // Force external references creation.
        ( m_cellLinkRange as INativePTG ).GetNativePtg();
      }
    }
    /// <summary>
    /// indicates whether the option button is first button in the group
    /// </summary>
    public bool IsFirstButton
    {
      get
      {
        return m_isFirstButton;
      }
      set
      {
        m_isFirstButton = value;
      }
    }
    /// <summary>
    /// Gets or sets value indicating whether 3D shadow is present.
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

    #region Event
    /// <summary>
    /// Raise when Linked Cell Value Changed
    /// </summary>
    internal event ValueChangedEventHandler LinkedCellValueChanged;
    #endregion

  }
}

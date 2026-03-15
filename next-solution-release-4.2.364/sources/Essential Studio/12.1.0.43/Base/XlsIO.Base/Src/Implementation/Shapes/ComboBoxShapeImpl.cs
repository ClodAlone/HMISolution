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
using System.Collections;
using Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Implementation.Shapes
{
  /// <summary>
  /// This class represents combo box object.
  /// </summary>
  public class ComboBoxShapeImpl :
    ShapeImpl,
    IComboBoxShape
  {
    #region Constants
    /// <summary>
    /// Shape instance.
    /// </summary>
    public const int ShapeInstance = 201;
    /// <summary>
    /// Shape version.
    /// </summary>
    public const int ShapeVersion = 2;
    /// <summary>
    /// Default number of drop lines.
    /// </summary>
    private const int DefaultDropLinesCount = 8;
    #endregion

    #region Members
    /// <summary>
    /// The worksheet range used to fill the specified list box.
    /// </summary>
    private IRange m_inputRange;
    /// <summary>
    /// Gets or sets the worksheet range linked to the control's value.
    /// </summary>
    private IRange m_cellLinkRange;
    /// <summary>
    /// Gets or sets selected item index of the combo box.
    /// </summary>
    private int m_iSelectedIndex;
    /// <summary>
    /// Number of list lines displayed in the drop-down portion of a combo box.
    /// </summary>
    private int m_iDropLines = DefaultDropLinesCount;
    /// <summary>
    /// Type of the combobox object.
    /// </summary>
    private ExcelComboType m_comboType = ExcelComboType.Regular;
    /// <summary>
    /// Indicates whether combo box has 3D shadow.
    /// </summary>
    private bool m_bThreeD;
    /// <summary>
    /// Indicates the formula string associated with Macro.
    /// </summary>
    private string m_formulaMacro;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the worksheet range used to fill the specified list box.
    /// </summary>
    public IRange ListFillRange
    {
      get
      {
        return m_inputRange;
      }
      set
      {
        m_inputRange = value;

        if (value != null)
        {
            // Force external references creation.
            (m_inputRange as INativePTG).GetNativePtg();
        }
      }
    }
    /// <summary>
    /// Gets or sets the worksheet range linked to the control's value.
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
          throw new ArgumentOutOfRangeException( "CellLink must be single cell." );

        m_cellLinkRange = value;

        if( value != null )
        {
          // Force external references creation.
          ( m_cellLinkRange as INativePTG ).GetNativePtg();
        }
      }
    }
    /// <summary>
    /// Gets or sets selected item index of the combo box.
    /// </summary>
    public int SelectedIndex
    {
      get
      {
        if( m_cellLinkRange != null )
        {
          if( m_cellLinkRange.HasNumber )
          {
            m_iSelectedIndex = ( int )m_cellLinkRange.Number;
          }
          else if( m_cellLinkRange.IsBlank )
          {
            m_iSelectedIndex = 0;
          }
        }

        return m_iSelectedIndex;
      }
      set
      {
        m_iSelectedIndex = value;

        if( m_cellLinkRange != null && m_inputRange != null )
        {
          m_cellLinkRange.Number = m_iSelectedIndex;
        }
      }
    }
    /// <summary>
    /// Gets or sets the number of list lines displayed in the drop-down portion of a combo box.
    /// </summary>
    public int DropDownLines
    {
      get
      {
        return m_iDropLines;
      }
      set
      {
        if( value < 0 )
          throw new ArgumentOutOfRangeException( "DropLines" );

        m_iDropLines = value;
      }
    }
    /// <summary>
    /// Gets type of the combo box object.
    /// </summary>
    public ExcelComboType ComboType
    {
      get
      {
        return m_comboType;
      }
    }
    /// <summary>
    /// Gets or sets value indicating whether 3D shadow is present.
    /// </summary>
    public bool Display3DShading
    {
      get
      {
        return m_bThreeD;
      }
      set
      {
        m_bThreeD = value;
      }
    }
    /// <summary>
    /// Gets value selected in combobox.
    /// </summary>
    public string SelectedValue
    {
      get
      {
        int iSelectedIndex = SelectedIndex;

        return ( iSelectedIndex > 0 ) ?
          m_inputRange.Cells[ iSelectedIndex - 1 ].Value :
          null;
      }
    }
    /// <summary>
    /// Gets or sets the formula associated with macro.
    /// </summary>
    internal string FormulaMacro
    {
        get
        {
            return m_formulaMacro;
        }
        set
        {
            m_formulaMacro = value;
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the combo box item.
    /// </summary>
    /// <param name="application">Application object for the new combo box object.</param>
    /// <param name="parent">Parent object for the new combo box object.</param>
    public ComboBoxShapeImpl( IApplication application, object parent )
      : base( application, parent )
    {
      VmlShape = true;
    }
    /// <summary>
    /// Initializes new instance of the combo box item.
    /// </summary>
    /// <param name="application">Application object for the new combo box object.</param>
    /// <param name="parent">Parent object for the new combo box object.</param>
    /// <param name="shapeContainer">Shape's container.</param>
    /// <param name="options">Parsing options.</param>
    public ComboBoxShapeImpl( IApplication application, object parent, MsofbtSpContainer shapeContainer,
      ExcelParseOptions options, List<ObjSubRecord> subRecords )
      : base( application, parent, shapeContainer, options )
    {
      VmlShape = true;

      ParseSubRecords( subRecords );
    }
    /// <summary>
    /// Parses OBJ subrecords.
    /// </summary>
    /// <param name="subRecords"></param>
    private void ParseSubRecords( List<ObjSubRecord> subRecords )
    {
      if( subRecords == null )
        throw new ArgumentNullException( "subRecords" );

      for( int i = 0, len = subRecords.Count; i < len; i++ )
      {
        ObjSubRecord subRecord = subRecords[ i ];

        switch( subRecord.Type )
        {
          case TObjSubRecordType.ftSbs:
            ParseSbsRecord( ( ftSbs )subRecord );
            break;

          case TObjSubRecordType.ftLbsData:
            ParseLbsData( ( ftLbsData )subRecord );
            break;

          case TObjSubRecordType.ftSbsFormula:
            ParseSbsFormula( ( ftSbsFormula )subRecord );
            break;
        }

      }
    }
    /// <summary>
    /// Parses SbsFormula record.
    /// </summary>
    /// <param name="ftSbsFormula">Record to parse.</param>
    private void ParseSbsFormula( ftSbsFormula ftSbsFormula )
    {
      if( ftSbsFormula == null )
        throw new ArgumentNullException( "ftSbsFormula" );

      // There must be only one token and that token has to be convertable to range object.
      IRangeGetter rangeGetter = ftSbsFormula.Formula[ 0 ] as IRangeGetter;
      m_cellLinkRange = rangeGetter.GetRange( Workbook, Worksheet as IWorksheet );
    }
    /// <summary>
    /// Parses LbsData record.
    /// </summary>
    /// <param name="ftLbsData">Record to parse.</param>
    private void ParseLbsData( ftLbsData ftLbsData )
    {
      if( ftLbsData == null )
        throw new ArgumentNullException( "ftLbsData" );

      // There must be only one token and that token has to be convertable to range object.
      Ptg[] tokens = ftLbsData.Formula;
      int iLength = ( tokens != null ) ?
        tokens.Length :
        0;

      if( iLength > 0 )
      {
        IRangeGetter rangeGetter = tokens[ 0 ] as IRangeGetter;
        m_inputRange = rangeGetter.GetRange( Workbook, Worksheet as IWorksheet );
      }

      m_iSelectedIndex = ftLbsData.SelectedIndex;
      m_comboType = ftLbsData.ComboType;
      m_bThreeD = !ftLbsData.NoThreeD;
      m_iDropLines = ftLbsData.DropData.LinesNumber;
    }
    /// <summary>
    /// Parses Sbs record.
    /// </summary>
    /// <param name="ftSbs">Record to parse.</param>
    private void ParseSbsRecord( ftSbs ftSbs )
    {
      if( ftSbs == null )
        throw new ArgumentNullException( "ftSbs" );

      // NOTE: not all values are supported by us on the current moment.
      m_iSelectedIndex = ftSbs.Value;
      m_iDropLines = ftSbs.Page;
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

      OBJRecord obj = Obj;
      bool bObjExist = ( obj != null );

      UpdateCmo( ref obj, bObjExist, clientData );
      UpdateSbs( obj, bObjExist );
      UpdateSbsFormula( obj, bObjExist );
      UpdateLbsData( obj, bObjExist );

      //cbls.CheckState = m_checkState;

      if( m_cellLinkRange == null && SelectedIndex > 0 )
      {
        //IRange cell = m_inputRange.Cells[ m_iSelectedIndex - 1 ];

        //if( cell.HasString )
        //  lbsData.DropData.Value = cell.Value;
      }

      if( !bObjExist )
      {
        ftEnd end = new ftEnd();
        obj.AddSubRecord( end );
      }

      clientData.AddRecord( obj );
      spContainer.AddItem( m_shape );

      MsofbtOPT options = SerializeOptions( spContainer );

      if( options.Properties.Length > 0 )
        spContainer.AddItem( options );

      spContainer.AddItem( ClientAnchor );
      spContainer.AddItem( clientData );
      spgrContainer.AddItem( spContainer );
    }
    /// <summary>
    /// Updates ftLbsData sub record.
    /// </summary>
    /// <param name="obj">OBJ record to update sub record in.</param>
    /// <param name="bObjExist">Indicates whether OBJRecord existed before or it was created from scratch.</param>
    private void UpdateLbsData( OBJRecord obj, bool bObjExist )
    {
      ftLbsData lbsData =null;
      bool bNeedAdd = !bObjExist ||
        ( lbsData = ( ftLbsData )obj.FindSubRecord( TObjSubRecordType.ftLbsData ) ) == null;

      if( bNeedAdd )
        lbsData = new ftLbsData();

      //  m_inputRange.Count :
      //  0;

      lbsData.Formula = ( m_inputRange != null ) ?
        ( m_inputRange as INativePTG ).GetNativePtg() :
        null;

      lbsData.SelectedIndex = SelectedIndex;
      lbsData.DropData .LinesNumber  = (short)DropDownLines;
      lbsData.EditId = 0;  // unknown id.

      if( m_comboType != ExcelComboType.Regular )
      {
        lbsData.ComboType = m_comboType;
      }

      lbsData.NoThreeD = !m_bThreeD;

      if( bNeedAdd )
        obj.AddSubRecord( lbsData );
    }
    /// <summary>
    /// Updates ftSbsFormula sub record.
    /// </summary>
    /// <param name="obj">OBJ record to update sub record in.</param>
    /// <param name="bObjExist">Indicates whether OBJRecord existed before or it was created from scratch.</param>
    private void UpdateSbsFormula( OBJRecord obj, bool bObjExist )
    {
      ftSbsFormula sbsFormula = null;
      int iSbsIndex;

      if( m_cellLinkRange != null )
      {
        bool bNeedAdd = 
          ( !bObjExist ||
          ( sbsFormula = ( ftSbsFormula )obj.FindSubRecord( TObjSubRecordType.ftSbsFormula ) ) == null );

        if( bNeedAdd )
        {
          sbsFormula = new ftSbsFormula();
        }

        sbsFormula.Formula = ( m_cellLinkRange as INativePTG ).GetNativePtg();

        if( bNeedAdd )
          obj.AddSubRecord( sbsFormula );
      }
      else if( ( iSbsIndex = obj.FindSubRecordIndex( TObjSubRecordType.ftSbsFormula ) ) >= 0 )
      {
        obj.RecordsList.RemoveAt( iSbsIndex );
      }
    }
    /// <summary>
    /// Updates ftSbs sub record.
    /// </summary>
    /// <param name="obj">OBJ record to update sub record in.</param>
    /// <param name="bObjExist">Indicates whether OBJRecord existed before or it was created from scratch.</param>
    private void UpdateSbs( OBJRecord obj, bool bObjExist )
    {
      ftSbs sbs;
      if( !bObjExist ||
        ( sbs = ( ftSbs )obj.FindSubRecord( TObjSubRecordType.ftSbs ) ) == null )
      {
        sbs = new ftSbs();
        sbs.Value = 2; // unknown value.
        sbs.ScrollBarWidth = 16;
        sbs.Minimum = 0;
        sbs.Maximum = 2; // unknown value.
        sbs.Increment = 1;
        sbs.Horizontal = 0;

        obj.AddSubRecord( sbs );
      }

      sbs.Page = m_iDropLines;
    }
    /// <summary>
    /// Updates ftCmo sub record.
    /// </summary>
    /// <param name="obj">OBJ record to update sub record in.</param>
    /// <param name="bObjExist">Indicates whether OBJRecord existed before or it was created from scratch.</param>
    private void UpdateCmo( ref OBJRecord obj, bool bObjExist, MsofbtClientData clientData )
    {
      ftCmo cmo;

      if( !bObjExist )
      {
        obj = ( OBJRecord )BiffRecordFactory.GetRecord( TBIFFRecord.OBJ );
        cmo = new ftCmo();

        cmo.ObjectType = TObjType.otComboBox;
        cmo.Printable = true;

        cmo.Locked = true;
        cmo.AutoLine = false;
        obj.AddSubRecord( cmo );
      }
      else
      {
        cmo = Obj.RecordsList[ 0 ] as ftCmo;
      }

      cmo.ID = ( OldObjId > 0 ) ? ( ushort )OldObjId : ( ushort )ParentWorkbook.CurrentObjectId;
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
      ComboBoxShapeImpl result = ( ComboBoxShapeImpl )base.Clone( parent, hashNewNames, dicFontIndexes, addToCollections );
      WorksheetBaseImpl newWorksheet = result.Worksheet;
      WorkbookImpl newBook = newWorksheet.ParentWorkbook;

      if( m_inputRange != null )
        result.m_inputRange = ( m_inputRange as ICombinedRange ).Clone( newWorksheet, hashNewNames, newBook );

      if( m_cellLinkRange != null )
        result.m_cellLinkRange = ( m_cellLinkRange as ICombinedRange ).Clone( newWorksheet, hashNewNames, newBook );

      if( addToCollections )
        ( result.Worksheet.ComboBoxes as Syncfusion.XlsIO.Implementation.Collections.ComboBoxCollection ).AddComboBox( result );

      return result;
    }
    #endregion
  }
}

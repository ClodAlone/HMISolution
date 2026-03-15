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

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords;

using FOPTE = Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing.MsofbtOPT.FOPTE;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Implementation.Shapes
{
  /// <summary>
  /// Class used for Form control shapes.
  /// </summary>
  public class FormControlShapeImpl
    : ShapeImpl
    , IShape
  {
    #region Class constants
    /// <summary>
    /// Represents default lock against grouping value.
    /// </summary>
    private const int DEF_LOCKAGAINSGROUPDING_VALUE = 17039620;
    /// <summary>
    /// 
    /// </summary>
    internal const int DEF_SIZETEXT_VALUE = 524296;
    /// <summary>
    /// Represents default value for no line .
    /// </summary>
    private const int DEF_NOLINE_VALUE = 524288;
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_959_VALUE = 131072;
    /// <summary>
    /// 
    /// </summary>
    private static readonly byte[] DEF_CMO_DATA = new byte[]
    {
      //00, 00, 00, 00, 0x14, 06, 0x2F, 01, 00, 00, 00, 00,   - blue color.
      00, 00, 00, 00, 0x6C, 0x19, 0x2A, 01, 00, 00, 00, 00, //- black color.
    };
    #endregion

    #region Class members
    /// <summary>
    /// List box data.
    /// </summary>
    private ftLbsData m_lbsData = new ftLbsData();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance of the shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    public FormControlShapeImpl( IApplication application, object parent )
      : base( application, parent )
    {
      Initialize();
    }
    /// <summary>
    /// Initializes new instance of the shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    /// <param name="records">Array of records that contains the record for new shape.</param>
    /// <param name="index">Index of the records for new shape.</param>
    [ CLSCompliant( false ) ]
    public FormControlShapeImpl( IApplication application, object parent, MsoBase[] records, int index )
      : base( application, parent, records, index )
    {
      Initialize();
    }
    /// <summary>
    /// Initializes new instance of the shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    /// <param name="container">Shape container record that describes the new shape.</param>
    [ CLSCompliant( false ) ]
    public FormControlShapeImpl( IApplication application, object parent, MsofbtSpContainer container )
      : base( application, parent, container )
    {
      Initialize();
    }
    /// <summary>
    /// Initializes new instance of the shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    /// <param name="shapeRecord">Record that describes the new shape.</param>
    [ CLSCompliant( false ) ]
    public FormControlShapeImpl( IApplication application, object parent, MsoBase shapeRecord )
      : base( application, parent, shapeRecord, ExcelParseOptions.Default )
    {
      Initialize();
    }
    /// <summary>
    /// Initializes internal variables.
    /// </summary>
    private void Initialize()
    {
      ShapeType = ExcelShapeType.FormControl;
      ClientAnchor.IsSizeWithCell = true;
      m_lbsData.ComboType = ExcelComboType.AutoFilter;
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Creates a clone of the current shape.
    /// </summary>
    /// <param name="parent">New parent for the shape object.</param>
    /// <param name="hashNewNames">Dictionary with new worksheet names.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <param name="addToCollection">Indicates whether we should add created
    /// shape into all necessary parent collections.</param>
    /// <returns>A copy of the current shape.</returns>
    public override IShape Clone( object parent, Dictionary<string, string> hashNewNames,
      Dictionary<int, int> dicFontIndexes, bool addToCollection )
    {
      FormControlShapeImpl result = ( FormControlShapeImpl )
        base.Clone( parent, hashNewNames, dicFontIndexes, addToCollection );

      result.m_lbsData = ( ftLbsData )m_lbsData.Clone();

      return result;
    }
    /// <summary>
    /// This method is called inside of PrepareForSerialization to make shape-dependent preparations.
    /// </summary>
    protected override void OnPrepareForSerialization()
    {
      if( m_shape == null )
      {
        m_shape = ( MsofbtSp )MsoFactory.GetRecord( MsoRecords.msofbtSp );
        m_shape.Instance = 201;
        m_shape.IsHaveAnchor = true;
        m_shape.IsHaveSpt = true;
      }
    }
    /// <summary>
    /// Serializes shape.
    /// </summary>
    /// <param name="spgrContainer">Container to add shape.</param>
    [ CLSCompliant( false ) ]
    protected override void SerializeShape(MsofbtSpgrContainer spgrContainer)
    {
//      base.SerializeShape (spgrContainer);
//----------------------------------
      MsofbtSpContainer spContainer = new MsofbtSpContainer( spgrContainer );
      MsofbtClientAnchor clientAnchor = new MsofbtClientAnchor( spContainer );
      MsofbtClientData clientData = new MsofbtClientData( spContainer );
      OBJRecord obj = ( OBJRecord )BiffRecordFactory.GetRecord( TBIFFRecord.OBJ );
      ftCmo cmo = new ftCmo();

      cmo.ObjectType = TObjType.otComboBox;
      cmo.Printable = false;
      cmo.Locked    = true;
      cmo.AutoFill  = true;
      cmo.AutoLine  = false;
      cmo.ChangeColor = true;

      cmo.ID = ( OldObjId > 0 ) ? ( ushort )OldObjId : ( ushort )ParentWorkbook.CurrentObjectId;
      DEF_CMO_DATA.CopyTo( cmo.Reserved, 0 );

      ftSbs sbs = new ftSbs();

      ftEnd end = new ftEnd();

      obj.AddSubRecord( cmo );
      obj.AddSubRecord( sbs );
      obj.AddSubRecord( m_lbsData );
      obj.AddSubRecord( end );

      clientData.AddRecord( obj );

      MsofbtOPT options = SerializeOptions( m_shape );

      spContainer.AddItem( m_shape );
      spContainer.AddItem( options );

      ClientAnchor.Options = 1;
      spContainer.AddItem( ClientAnchor );
      spContainer.AddItem( clientData );

      spgrContainer.AddItem( spContainer );
//----------------------------------
    }
    /// <summary>
    /// Serialize shape options.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <returns>Record with serialized option.</returns>
    [ CLSCompliant( false ) ]
    protected override MsofbtOPT SerializeOptions( MsoBase parent )
    {
      if( m_bUpdateLineFill || m_options == null )
      {
        MsofbtOPT result = base.SerializeOptions (parent);
      
        SerializeOption( result, MsoOptions.LockAgainstGrouping,
          DEF_LOCKAGAINSGROUPDING_VALUE );

        SerializeOption( result, MsoOptions.SizeTextToFitShape, DEF_SIZETEXT_VALUE );
        SerializeOption( result, MsoOptions.NoLineDrawDash, DEF_NOLINE_VALUE );
        SerializeOption( result, ( MsoOptions )959, DEF_959_VALUE );

        result.Version = 3;
        result.Instance = 2;

        return result;
      }

      return m_options;
    }
    /// <summary>
    /// Parses client data.
    /// </summary>
    /// <param name="clientData">Data to parse.</param>
    /// <param name="options">Parse options.</param>
    [ CLSCompliant( false ) ]
    protected override void ParseClientData( MsofbtClientData clientData, ExcelParseOptions options )
    {
      if( clientData == null )
        throw new ArgumentNullException( "clientData" );

      base.ParseClientData( clientData, options );

      List<ObjSubRecord> arrRecords = Obj.RecordsList;

      for( int i = 0, len = arrRecords.Count; i < len; i++ )
      {
        ObjSubRecord subRecord = arrRecords[ i ];

        if( subRecord.Type == TObjSubRecordType.ftLbsData )
        {
          m_lbsData = ( ftLbsData )subRecord;
        }
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Indicates whether list box arrow has default or selected color.
    /// </summary>
    public bool IsArrowSelectedColor
    {
      get
      {
        return m_lbsData.IsSelectedColor;
      }
      set
      {
        m_lbsData.IsSelectedColor = value;
      }
    }
    #endregion
  }
}

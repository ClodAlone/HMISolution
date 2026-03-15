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
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;

namespace Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords
{
  /// <summary>
  /// Note structure.
  /// </summary>
  [CLSCompliant( false )]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class ftLbsData : ObjSubRecord
  {
    #region Class constants
    /// <summary>
    /// Enum contains possible list selection type.
    /// </summary>
    public enum ExcelListSelectionType
    {
      /// <summary>
      /// The list control is only allowed to have one selected item.
      /// </summary>
      Single = 0,
      /// <summary>
      /// The list control is allowed to have multiple items selected by clicking on each item.
      /// </summary>
      MultiClick = 1,
      /// <summary>
      /// The list control is allowed to have multiple items selected by holding the CTRL key
      /// and clicking on each item.
      /// </summary>
      MultiCtrl = 2,
    }
    /// <summary>
    /// Size of the record.
    /// </summary>
    private const int DEF_RECORD_SIZE = 20;
    /// <summary>
    /// Bit position for value that defines arrow color.
    /// </summary>
    private const int DEF_COLOR_BIT_INDEX = 3;
    /// <summary>
    /// Index to the byte that defines arrow color.
    /// </summary>
    private const int DEF_COLOR_BYTE = 10;
    /// <summary>
    /// Defoult record data.
    /// </summary>
    private static readonly byte[] DEF_SAMPLE_RECORD_DATA = new byte[]
    {//                       selected index  
      //00, 00, /*0x0C*/07, 00, /*02*/01, 00, 01, 03, 00, 00, 0x0A, 00, 0x14, 00, 0x57, 00
      //00, 00, 8, 0, 5, 0, 1, 3, 0, 0, 10, 0, 20, 0, 162, 0  // - blue arrow
      00, 00, 8, 0, 4, 0, 1, 3, 0, 0, 2, 0, 20, 0, 162, 0     // - black arrow
    };
    /// <summary>
    /// Bitmask for TypeValid property.
    /// </summary>
    private const int TypeValidMask = 0x01;
    /// <summary>
    /// Bitmask for combo box Type property.
    /// </summary>
    private const int TypeMask = 0xFF00;
    /// <summary>
    /// Starting bit for the combo box Type property.
    /// </summary>
    private const int TypeMaskStartBit = 8;
    private const int ThreeDMask = 0x08;
    private const int SelectionTypeMask = 0x30;
    private const int SelectionTypeStartBit = 4;
    #endregion

    #region Class members
    /// <summary>
    /// Internal data array.
    /// </summary>
    private byte[] m_data;
    /// <summary>
    /// Number of lines in the list box.
    /// </summary>
    private int m_iLinesNumber;
    /// <summary>
    /// Formula holding referenced area.
    /// </summary>
    private Ptg[] m_arrFormula;
    /// <summary>
    /// Selected item index.
    /// </summary>
    private int m_iSelectedIndex;
    /// <summary>
    /// Option flags.
    /// </summary>
    private int m_iOptions;
    /// <summary>
    /// Edit id.
    /// </summary>
    private int m_iEditId;
    private LbsDropData m_dropData = new LbsDropData();
    //private bool m_bI3D;
    //private string[] m_arrLines;
    /// <summary>
    /// Array indicating whether so item was selected or not.
    /// </summary>
    private bool[] m_arrSelections;
    /// <summary>
    /// Indicates whether this record is short representation of the list box data record
    /// (without options, and any additional data).
    /// </summary>
    private bool m_bShortVersion;
    /// <summary>
    /// Shape's type.
    /// </summary>
    private TObjType m_parentObjectType = TObjType.otComboBox;
    #endregion

    #region Class properties
    /// <summary>
    /// Returns internal data array. Read-only.
    /// </summary>
    public byte[] Data1
    {
      get
      {
        return m_data;
      }
      set
      {
        m_data = value;
      }
    }
    /// <summary>
    /// Indicates whether arrow has default or selected color.
    /// </summary>
    public bool IsSelectedColor
    {
      get
      {
        return BiffRecordRaw.GetBit( m_data, DEF_COLOR_BYTE, DEF_COLOR_BIT_INDEX );
      }
      set
      {
        BiffRecordRaw.SetBit( m_data, DEF_COLOR_BYTE, value, DEF_COLOR_BIT_INDEX );
      }
    }
    /// <summary>
    /// Number of items in the list.
    /// </summary>
    public int LinesNumber
    {
      get
      {
        return m_iLinesNumber;
      }
      set
      {
        if( m_iLinesNumber != value )
        {
          m_iLinesNumber = value;

          if( m_arrSelections != null )
          {
            bool[] oldSelection = m_arrSelections;
            m_arrSelections = new bool[ value ];
            Buffer.BlockCopy( oldSelection, 0, m_arrSelections, 0, oldSelection.Length );
          }
        }
      }
    }
    /// <summary>
    /// Formula token specifying referenced area.
    /// </summary>
    public Ptg[] Formula
    {
      get
      {
        return m_arrFormula;
      }
      set
      {
        m_arrFormula = value;
      }
    }
    /// <summary>
    /// One-based selected index. 0 - no item is selected.
    /// </summary>
    public int SelectedIndex
    {
      get
      {
        return m_iSelectedIndex;
      }
      set
      {
        m_iSelectedIndex = value;
      }
    }
    /// <summary>
    /// Options.
    /// </summary>
    public int Options
    {
      get
      {
        return m_iOptions;
      }
      set
      {
        m_iOptions = value;
      }
    }
    /// <summary>
    /// An ObjId that specifies the edit box associated with this list.
    /// </summary>
    public int EditId
    {
      get
      {
        return m_iEditId;
      }
      set
      {
        m_iEditId = value;
      }
    }
    /// <summary>
    /// List box drop data.
    /// </summary>
    public LbsDropData DropData
    {
      get
      {
        return m_dropData;
      }
    }
    /// <summary>
    /// Gets or sets value indicating whether combo box type has valid value.
    /// </summary>
    public bool ComboTypeValid
    {
      get
      {
        return ( m_iOptions & TypeValidMask ) != 0;
      }
      set
      {
        m_iOptions = ( value ) ?
          m_iOptions | TypeValidMask :
          m_iOptions & ~TypeValidMask;
      }
    }
    /// <summary>
    /// Gets or sets combo box type.
    /// </summary>
    public ExcelComboType ComboType
    {
      get
      {
        return ( ComboTypeValid ) ?
          ( ExcelComboType )( ( m_iOptions & TypeMask ) >> TypeMaskStartBit ) :
          ExcelComboType.Regular;
      }
      set
      {
        int iValue = ( int )value << TypeMaskStartBit;
        m_iOptions &= ~TypeMask;
        m_iOptions |= iValue;

        ComboTypeValid = ( value != ExcelComboType.Regular );
      }
    }
    /// <summary>
    /// Gets or sets value indicates whether control has 3-D effect.
    /// </summary>
    public bool NoThreeD
    {
      get
      {
        return ( m_iOptions & ThreeDMask ) != 0;
      }
      set
      {
        if( value )
        {
          m_iOptions |= ThreeDMask;
        }
        else
        {
          m_iOptions &= ~ThreeDMask;
        }
      }
    }
    /// <summary>
    /// Gets or sets list selection type.
    /// </summary>
    public ExcelListSelectionType SelectionType
    {
      get
      {
        return ( ExcelListSelectionType )( ( m_iOptions & SelectionTypeMask ) >> SelectionTypeStartBit );
      }
      set
      {
        if( value != SelectionType )
        {
          int iValue = ( int )value << SelectionTypeStartBit;
          m_iOptions &= ~SelectionTypeMask;
          m_iOptions |= iValue;

          if( value == ExcelListSelectionType.Single )
          {
            m_arrSelections = null;
          }
          else
          {
            m_arrSelections = new bool[ LinesNumber ];
          }
        }
      }
    }
    /// <summary>
    /// Gets value indicating whether we have multi selection or not.
    /// </summary>
    public bool IsMultiSelection
    {
      get
      {
        return SelectionType != ExcelListSelectionType.Single;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public ftLbsData()
      : base( TObjSubRecordType.ftLbsData )//, DEF_RECORD_SIZE - 4, DEF_SAMPLE_RECORD_DATA )
    {
    }
    /// <summary>
    /// Initializes new instance of subrecord.
    /// </summary>
    /// <param name="type">Type of the subrecord.</param>
    /// <param name="length">Length of the subrecord's data.</param>
    /// <param name="buffer">Buffer that contains subrecord's data.</param>
    public ftLbsData( TObjSubRecordType type, ushort length, byte[] buffer )
      : base( type, length, buffer )
    {
    }
    /// <summary>
    /// Initializes new instance of subrecord.
    /// </summary>
    /// <param name="type">Type of the subrecord.</param>
    /// <param name="length">Length of the subrecord's data.</param>
    /// <param name="buffer">Buffer that contains subrecord's data.</param>
    public ftLbsData( TObjSubRecordType type, ushort length, byte[] buffer, TObjType objectType )
      : base( type )
    {
      Length = length;
      m_parentObjectType = objectType;
      Parse( buffer );
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Parses byte array.
    /// </summary>
    /// <param name="buffer">Array to parse.</param>
    /// <param name="objectType">Type of the object this record is part of.</param>
    protected override void Parse( byte[] buffer )
    {
      m_data = ( byte[] )buffer.Clone();
      //const int FormulaOffset = 8;
      const int ObjFormulaSizeOffset = 0;

      // Size of the ObjFormula data (must be even).
      int iOffset = ObjFormulaSizeOffset;
      int iFormulaSize = BitConverter.ToInt16( buffer, iOffset );
      iOffset += ExcelConstants.ShortSize;

      if( iFormulaSize > 0 )
      {
        // Size of the actual formula tokens
        int iTokensSize = BitConverter.ToInt16( buffer, iOffset );
        iOffset += ExcelConstants.ShortSize;

        //int FormulaOffset = ObjFormulaSizeOffset + ExcelConstants.ShortSize + ExcelConstants.ShortSize;
        int Reserverd = BitConverter.ToInt32( buffer, iOffset );
        iOffset += ExcelConstants.IntSize;

        byte[] arrFormulaData = new byte[ iTokensSize ];
        Buffer.BlockCopy( m_data, iOffset, arrFormulaData, 0, iTokensSize );

        m_arrFormula = FormulaUtil.ParseExpression( new ByteArrayDataProvider( arrFormulaData ),
          iTokensSize, ExcelVersion.Excel97to2003 );

        iOffset = ObjFormulaSizeOffset + iFormulaSize + ExcelConstants.ShortSize;
      }

      m_iLinesNumber = BitConverter.ToInt16( buffer, iOffset );
      iOffset += ExcelConstants.ShortSize;

      m_iSelectedIndex = BitConverter.ToInt16( buffer, iOffset );
      iOffset += ExcelConstants.ShortSize;

      if( iOffset >= buffer.Length )
      {
        m_bShortVersion = true;
      }
      else
      {
        m_iOptions = BitConverter.ToInt16( buffer, iOffset );
        iOffset += ExcelConstants.ShortSize;

        m_iEditId = BitConverter.ToInt16( buffer, iOffset );
        iOffset += ExcelConstants.ShortSize;

        if( m_parentObjectType == TObjType.otComboBox )
          //if( IsDropDataAvailable )
          iOffset = m_dropData.Parse( new ByteArrayDataProvider( buffer ), iOffset );

        //if( IsLinesAvailable )
        //  iOffset = ParseLines( buffer, iOffset );

        if( IsMultiSelection )
          iOffset = ParseMultiSelection( buffer, iOffset );
      }
    }
    /// <summary>
    /// Parses selected items in the case of multiple selection enabled.
    /// </summary>
    /// <param name="buffer">Buffer to get data from.</param>
    /// <param name="iOffset">Offset to the selection data start.</param>
    /// <returns>Offset after extracting necessary data.</returns>
    private int ParseMultiSelection( byte[] buffer, int iOffset )
    {
      m_arrSelections = new bool[ m_iLinesNumber ];

      for( int i = 0; i < m_iLinesNumber && iOffset < buffer.Length; i++, iOffset++ )
      {
        m_arrSelections[ i ] = buffer[ iOffset ] != 0;
      }

      return iOffset;
    }

    private void ParseLines( byte[] buffer, int iOffset )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    ///// <summary>
    ///// Fills array with binary representation of the subrecord.
    ///// </summary>
    ///// <param name="provider">Object that provides access to the data.</param>
    ///// <param name="iOffset">Offset in the buffer to copy data to.</param>
    //public override void FillArray( DataProvider provider, int iOffset )
    //{
    //  provider.WriteInt16( iOffset, ( short )Type );
    //  iOffset += 2;

    //  provider.WriteInt16( iOffset, ( short )DEF_RECORD_SIZE - 4 );
    //  iOffset += 2;

    //  if( m_data != null )
    //  {
    //    provider.WriteBytes( iOffset, m_data, 0, m_data.Length );
    //  }
    //}
    /// <summary>
    /// Clones current objects.
    /// </summary>
    /// <returns>Returns instance of cloned object.</returns>
    public override object Clone()
    {
      ftLbsData result = ( ftLbsData )base.Clone();

      result.m_data = CloneUtils.CloneByteArray( m_data );
      result.m_arrFormula = CloneUtils.ClonePtgArray( m_arrFormula );
      result.m_dropData = m_dropData.Clone();
      result.m_arrSelections = CloneUtils.CloneBoolArray( m_arrSelections );

      return result;
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iFormulaSize = DVRecord.GetFormulaSize( m_arrFormula, ExcelVersion.Excel97to2003, false );

      if( iFormulaSize % 2 != 0 )
        iFormulaSize++;

      if( iFormulaSize > 0 )
      {
        iFormulaSize +=
          ExcelConstants.ShortSize + // header size (formula + reserved + padding)
          ExcelConstants.ShortSize + // formula size (formula)
          ExcelConstants.IntSize; // reserved
      }

      if( !m_bShortVersion )
        iFormulaSize += ExcelConstants.ShortSize * 3;

      iFormulaSize += ExcelConstants.ShortSize * 2 + HeaderSize;

      if( m_parentObjectType == TObjType.otComboBox )
        iFormulaSize += m_dropData.GetStoreSize();

      if( m_arrSelections != null )
        iFormulaSize += m_arrSelections.Length;

      return iFormulaSize;
    }
    protected override void Serialize( DataProvider provider, int iOffset )
    {
      // Size of the ObjFormula data (must be even).

      byte[] arrFormulaTokens = ( m_arrFormula != null ) ?
        FormulaUtil.PtgArrayToByteArray( m_arrFormula, ExcelVersion.Excel97to2003 ) :
        new byte[ 0 ];

      // Size of the actual formula tokens
      int iTokensSize = arrFormulaTokens.Length;
      int iTotalSize = ( iTokensSize > 0 ) ?
        iTokensSize + ExcelConstants.ShortSize + ExcelConstants.IntSize :
        0;

      bool bAddPadding = iTotalSize % 2 != 0;

      if( bAddPadding )
        iTotalSize++;

      provider.WriteInt16( iOffset, ( short )iTotalSize );
      iOffset += ExcelConstants.ShortSize;

      if( iTokensSize > 0 )
      {
        provider.WriteInt16( iOffset, ( short )iTokensSize );
        iOffset += ExcelConstants.ShortSize;

        //int FormulaOffset = ObjFormulaSizeOffset + ExcelConstants.ShortSize + ExcelConstants.ShortSize;
        provider.WriteInt32( iOffset, 0 );
        iOffset += ExcelConstants.IntSize;

        if( iTokensSize > 0 )
        {
          provider.WriteBytes( iOffset, arrFormulaTokens );
          iOffset += iTokensSize;
        }

        if( bAddPadding )
        {
          provider.WriteByte( iOffset, 0xF0 );
          iOffset++;
        }
      }

      provider.WriteInt16( iOffset, ( short )m_iLinesNumber );
      iOffset += ExcelConstants.ShortSize;

      provider.WriteInt16( iOffset, ( short )m_iSelectedIndex );
      iOffset += ExcelConstants.ShortSize;

      if( !m_bShortVersion )
      {
        provider.WriteInt16( iOffset, ( short )m_iOptions );
        iOffset += ExcelConstants.ShortSize;

        provider.WriteInt16( iOffset, ( short )m_iEditId );
        iOffset += ExcelConstants.ShortSize;

        if( m_parentObjectType == TObjType.otComboBox )
        {
          if( ComboType == ExcelComboType.AutoFilter )
            m_dropData.Options = 2; // simple combo

          m_dropData.Serialize( provider, iOffset );
        }

        //if( m_arrLines != null )
        //  SerializeLines( provider, iOffset );

        if( m_arrSelections != null )
          iOffset = SerializeMultiSelection( provider, iOffset );
      }
    }
    /// <summary>
    /// Serializes 
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="iOffset"></param>
    /// <returns></returns>
    private int SerializeMultiSelection( DataProvider provider, int iOffset )
    {
      for( int i = 0, len = m_arrSelections.Length; i < len; i++, iOffset++ )
      {
        byte btValue = ( byte )( m_arrSelections[ i ] ? 1 : 0 );
        provider.WriteByte( iOffset, btValue );
      }

      return iOffset;
    }
    #endregion
  }
}

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
using System.IO;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Interfaces;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Summary description for ArrayRecord.
  /// </summary>
  [ Biff( TBIFFRecord.Array ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ArrayRecord
    : BiffRecordRaw
    , ISharedFormula
    , ICloneable
    , IFormulaRecord
  {
    #region Class constants
    /// <summary>
    /// Minimum size of the record.
    /// </summary>
    private const int DEF_RECORD_MIN_SIZE = 14;
    /// <summary>
    /// Offset to the formula data.
    /// </summary>
    private const int DEF_FORMULA_OFFSET = DEF_RECORD_MIN_SIZE;
    #endregion

    #region Class members
    /// <summary>
    /// First row.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private int m_iFirstRow;
    /// <summary>
    /// Last row.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private int m_iLastRow;
    /// <summary>
    /// First row.
    /// </summary>
    [ BiffRecordPos( 4, 1 ) ]
    private int m_iFirstColumn;
    /// <summary>
    /// Last row.
    /// </summary>
    [ BiffRecordPos( 5, 1 ) ]
    private int m_iLastColumn;
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// Indicates whether formula value should always be recalculated.
    /// </summary>
    [ BiffRecordPos( 6, 0, TFieldType.Bit ) ]
    private bool m_bRecalculateAlways;
    /// <summary>
    /// Indicates whether formula value should be recalculated when the file is opened.
    /// </summary>
    [ BiffRecordPos( 6, 1, TFieldType.Bit ) ]
    private bool m_bRecalculateOnOpen;
    /// <summary>
    /// Unused must be 0.
    /// </summary>
    [ BiffRecordPos( 8, 4, true ) ]
    private int m_iReserved;
    /// <summary>
    /// Length of the parsed expression.
    /// </summary>
    [ BiffRecordPos( 12, 2 ) ]
    private ushort m_usExpressionLength;

    /// <summary>
    /// Formula expression.
    /// </summary>
    private byte[] m_arrExpression;

    /// <summary>
    /// 
    /// </summary>
    private Ptg[] m_arrFormula;
    #endregion

    #region Class properties
    /// <summary>
    /// Index to first row of the array formula range. Zero-based.
    /// </summary>
    public int FirstRow
    {
      get
      {
        return m_iFirstRow;
      }
      set
      {
        m_iFirstRow = value;
      }
    }
    /// <summary>
    /// Index to last row of the array formula range. Zero-based.
    /// </summary>
    public int LastRow
    {
      get
      {
        return m_iLastRow;
      }
      set
      {
        m_iLastRow = value;
      }
    }
    /// <summary>
    /// Index to first column of the array formula range. Zero-based.
    /// </summary>
    public int   FirstColumn
    {
      get
      {
        return m_iFirstColumn;
      }
      set
      {
        m_iFirstColumn = value;
      }
    }
    /// <summary>
    /// Index to last column of the array formula range. Zero-based.
    /// </summary>
    public int   LastColumn
    {
      get
      {
        return m_iLastColumn;
      }
      set
      {
        m_iLastColumn = value;
      }
    }
    /// <summary>
    /// Read-only. Size of the formula data.
    /// </summary>
    public ushort ExpressionLen
    {
      get
      {
        return m_usExpressionLength;
      }
    }
    /// <summary>
    /// Token array of the shared formula.
    /// </summary>
    public byte[] Expression
    {
      get
      {
        return m_arrExpression;
      }
      set
      {
        m_arrExpression = value;
        m_usExpressionLength = ( value != null ) ?
          ( ushort ) value.Length : ( ushort ) 0;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public Ptg[]  Formula
    {
      get
      {
        return m_arrFormula;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "Formula" );

        int iFormulaLength;
        m_arrExpression = FormulaUtil.PtgArrayToByteArray( value, out iFormulaLength, ExcelVersion.Excel2007 );
        m_usExpressionLength = ( ushort )iFormulaLength;
        m_arrFormula = value;
      }
    }
    /// <summary>
    /// Read-only. Reserved.
    /// </summary>
    public int Reserved
    {
      get
      {
        return m_iReserved;
      }
    }
    /// <summary>
    /// Read-only. Returns minimum possible size of record's
    /// internal data array.
    /// </summary>
    override public int MinimumRecordSize
    {
      get
      {
        return DEF_RECORD_MIN_SIZE;
      }
    }

    /// <summary>
    /// Indicates whether formula value should be always recalculated.
    /// </summary>
    public bool IsRecalculateAlways
    {
      get
      {
        return m_bRecalculateAlways;
      }
      set
      {
        m_bRecalculateAlways = value;
      }
    }
    /// <summary>
    /// Indicates whether formula value should be recalculated when the file is opened.
    /// </summary>
    public bool IsRecalculateOnOpen
    {
      get
      {
        return m_bRecalculateOnOpen;
      }
      set
      {
        m_bRecalculateOnOpen = value;
      }
    }
    /// <summary>
    /// Option flags.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
#if DEBUG
      set
      {
        m_usOptions = value;
      }
#endif
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default Constructor.
    /// </summary>
    public  ArrayRecord()
      : base()
    {
    }
    /// <summary>
    /// Read / initialize Constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">When stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">When stream does not support read or seek operations.</exception>
    public  ArrayRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for the record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ArrayRecord( int iReserve )
      : base( iReserve )
    {
    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      iOffset = ParseDimensions( this, provider, iOffset, version );

      m_usOptions = provider.ReadUInt16( iOffset );
      m_bRecalculateAlways = provider.ReadBit( iOffset, 0 );
      m_bRecalculateOnOpen = provider.ReadBit( iOffset, 1 );
      iOffset += 2;

      m_iReserved = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_usExpressionLength = provider.ReadUInt16( iOffset );
      iOffset += 2;

      int iFinalOffset;

      m_arrFormula = FormulaUtil.ParseExpression( provider, iOffset,
        m_usExpressionLength, out iFinalOffset, version );

      m_arrExpression = new byte[ iFinalOffset - iOffset ];
      provider.ReadArray( iOffset, m_arrExpression );
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    /// <returns>Size of the record data.</returns>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_iReserved = 0;
      int iStartOffset = iOffset;

      int iFormulaLength;
      m_arrExpression = FormulaUtil.PtgArrayToByteArray( m_arrFormula, out iFormulaLength, version );
      m_usExpressionLength = ( ushort )iFormulaLength;

      iOffset = SerializeDimensions( this, provider, iOffset, version );

      provider.WriteUInt16( iOffset, m_usOptions );
      provider.WriteBit( iOffset, m_bRecalculateAlways, 0 );
      provider.WriteBit( iOffset, m_bRecalculateOnOpen, 1 );
      iOffset += 2;

      provider.WriteInt32( iOffset, m_iReserved );
      iOffset += 4;

      provider.WriteUInt16( iOffset, m_usExpressionLength );
      iOffset += 2;

      m_iLength = iOffset - iStartOffset;
      int iLength = m_arrExpression.Length;

      provider.WriteBytes( iOffset, m_arrExpression, 0, iLength );
      m_iLength += iLength;
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iResult = DEF_FORMULA_OFFSET + DVRecord.GetFormulaSize( m_arrFormula, version, true );

      if( version != ExcelVersion.Excel97to2003 )
      {
        iResult += 10; // ushort, ushort, byte, byte -> int, int, int, int.
      }

      return iResult;
    }
    /// <summary>
    /// Serializes area that is covered by array formula.
    /// </summary>
    /// <param name="shared">Object that contains dimensions data.</param>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    /// <returns>Updated offset.</returns>
    public static int SerializeDimensions( ISharedFormula shared, DataProvider provider,
      int iOffset, ExcelVersion version )
    {
      if( version == ExcelVersion.Excel97to2003 )
      {
        provider.WriteUInt16( iOffset, ( ushort )shared.FirstRow );
        iOffset += 2;

        provider.WriteUInt16( iOffset, ( ushort )shared.LastRow );
        iOffset += 2;

        provider.WriteByte( iOffset, ( byte )shared.FirstColumn );
        iOffset++;

        provider.WriteByte( iOffset, ( byte )shared.LastColumn );
        iOffset++;
      }
      else if( version !=ExcelVersion.Excel97to2003 )
      {
        provider.WriteInt32( iOffset, shared.FirstRow );
        iOffset += ExcelConstants.IntSize;

        provider.WriteInt32( iOffset, shared.LastRow );
        iOffset += ExcelConstants.IntSize;

        provider.WriteInt32( iOffset, shared.FirstColumn );
        iOffset += ExcelConstants.IntSize;

        provider.WriteInt32( iOffset, shared.LastColumn );
        iOffset += ExcelConstants.IntSize;
      }
      else
      {
        throw new ArgumentOutOfRangeException( "version" );
      }

      return iOffset;
    }
    /// <summary>
    /// Serializes area that is covered by array formula.
    /// </summary>
    /// <param name="shared">Object that receives dimensions data.</param>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    /// <returns>Updated offset.</returns>
    public static int ParseDimensions( ISharedFormula shared, DataProvider provider,
      int iOffset, ExcelVersion version )
    {
      if( version == ExcelVersion.Excel97to2003 )
      {
        shared.FirstRow = provider.ReadUInt16( iOffset );
        iOffset += 2;

        shared.LastRow = provider.ReadUInt16( iOffset );
        iOffset += 2;

        shared.FirstColumn = provider.ReadByte( iOffset );
        iOffset++;

        shared.LastColumn = provider.ReadByte( iOffset );
        iOffset++;
      }
      else if( version !=ExcelVersion.Excel97to2003 )
      {
        shared.FirstRow = provider.ReadInt32( iOffset );
        iOffset += ExcelConstants.IntSize;

        shared.LastRow = provider.ReadInt32( iOffset );
        iOffset += ExcelConstants.IntSize;

        shared.FirstColumn = provider.ReadInt32( iOffset );
        iOffset += ExcelConstants.IntSize;

        shared.LastColumn = provider.ReadInt32( iOffset );
        iOffset += ExcelConstants.IntSize;
      }
      else
      {
        throw new ArgumentOutOfRangeException( "version" );
      }

      return iOffset;
    }
    /// <summary>
    /// Determines whether the specified Object is equal to the current Object.
    /// </summary>
    /// <param name="obj">The Object to compare with the current Object.</param>
    /// <returns>true if the specified Object is equal to the current Object; otherwise, false.</returns>
    public override bool Equals( object obj )
    {
      ArrayRecord array = obj as ArrayRecord;

      if( array != null )
      {
        return array.FirstColumn == FirstColumn && array.FirstRow == FirstRow &&
          array.LastColumn == LastColumn &&
          array.LastRow == LastRow &&
          Ptg.CompareArrays( array.m_arrFormula, m_arrFormula );
      }
      return base.Equals( obj );
    }
    /// <summary>
    /// Serves as a hash function for a particular type.
    /// </summary>
    /// <returns>A hash code for the current Object.</returns>
    public override int GetHashCode()
    {
      return FirstColumn.GetHashCode() ^ FirstRow.GetHashCode() ^
        LastColumn.GetHashCode() ^
        LastRow.GetHashCode();// ^
        //m_arrFormula.GetHashCode();
    }
    #endregion

    #region ICloneable members
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    new public object Clone()
    {
      ArrayRecord result = ( ArrayRecord )base.Clone();
      result.m_arrExpression = CloneUtils.CloneByteArray( m_arrExpression );
      result.m_arrFormula = CloneUtils.ClonePtgArray( m_arrFormula );

      return result;
    }
    #endregion
  }
}

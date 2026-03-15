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
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record stores the token array of a shared formula.
  /// Shared formulas are similar to array formulas in that they store
  /// a formula used in a range of cells. The SharedFormula record
  /// is not a real cell record but follows the first FORMULA
  /// record of the cell range.
  /// </summary>
  [ Biff( TBIFFRecord.SharedFormula2 ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class SharedFormulaRecord
    : BiffRecordRaw
    , ISharedFormula
  {
    #region Class constants
    /// <summary>
    /// Size of the record's fixed part.
    /// </summary>
    private const int DEF_FIXED_SIZE = 10;
    #endregion

    #region Class members
    /// <summary>
    /// Index to first row of the shared formula range.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private int m_iFirstRow = 0;
    /// <summary>
    /// Index to last row of the shared formula range.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private int m_iLastRow = 0;
    /// <summary>
    /// Index to first column of the shared formula range.
    /// </summary>
    [ BiffRecordPos( 4, 1 ) ]
    private int m_iFirstColumn = 0;
    /// <summary>
    /// Index to last column of the shared formula range.
    /// </summary>
    [ BiffRecordPos( 5, 1 ) ]
    private int m_iLastColumn = 0;
    /// <summary>
    /// Not used.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usReserved = 0;
    /// <summary>
    /// Size of the formula data.
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usExpressionLen = 0;
    /// <summary>
    /// Token array of the shared formula.
    /// </summary>
    private byte[] m_arrExpression = null;
    /// <summary>
    /// 
    /// </summary>
    private Ptg[]  m_arrFormula;
    #endregion

    #region Class Properties
    /// <summary>
    /// Index to first row of the shared formula range.
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
    /// Index to last row of the shared formula range.
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
    /// Index to first column of the shared formula range.
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
    /// Index to last column of the shared formula range.
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
        return m_usExpressionLen;
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
        m_usExpressionLen = ( value != null ) ?
          ( ushort ) value.Length : ( ushort ) 0;
      }
    }
    /// <summary>
    /// Gets/sets formula into/from shared formula record.
    /// </summary>
    public Ptg[]  Formula
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
    /// Read-only. Reserved.
    /// </summary>
    public ushort Reserved
    {
      get
      {
        return m_usReserved;
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
        return 8;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor fills all data with default values.
    /// </summary>
    public  SharedFormulaRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is not specified.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If stream does not support read or seek operations.
    /// </exception>
    public  SharedFormulaRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If amount of bytes requested is less than zero.
    /// </exception>
    public  SharedFormulaRecord( int iReserve )
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
    public override void ParseStructure( DataProvider provider, int iOffset,
      int iLength, ExcelVersion version )
    {
#if AUTOEXTRACTING
      AutoExtractFields();
#else
      iOffset = ArrayRecord.ParseDimensions( this, provider, iOffset, version );

      m_usReserved = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usExpressionLen = provider.ReadUInt16( iOffset );
      iOffset += 2;
#endif
      m_arrExpression = new byte[ m_usExpressionLen ];
      provider.ReadArray( iOffset, m_arrExpression );

      int iFinalOffset;
      m_arrFormula = FormulaUtil.ParseExpression( provider, iOffset, m_usExpressionLen,
        out iFinalOffset, version );
    }

    /// <summary>
    /// In this method, class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      iOffset = ArrayRecord.SerializeDimensions( this, provider, iOffset, version );

      provider.WriteUInt16( iOffset, m_usReserved );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usExpressionLen );
      iOffset += 2;

      provider.WriteBytes( iOffset, m_arrExpression, 0, m_usExpressionLen );
      m_iLength = iOffset + m_usExpressionLen;
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iResult = DEF_FIXED_SIZE + m_usExpressionLen;

      if( version != ExcelVersion.Excel97to2003 )
      {
        iResult += 10; // us, us, bt, bt -> i, i, i, i
      }

      return iResult;
    }
    #endregion
  }
}
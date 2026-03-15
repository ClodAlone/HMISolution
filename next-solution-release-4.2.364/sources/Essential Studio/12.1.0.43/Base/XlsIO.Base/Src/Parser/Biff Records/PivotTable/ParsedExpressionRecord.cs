#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.IO;

namespace Syncfusion.XlsIO.Parser.Biff_Records.PivotTable
{
  /// <summary>
  /// This record stores a PivotTable parsed expression.
  /// </summary>
  [ Biff( TBIFFRecord.ParsedExpression ) ]
  [ CLSCompliant( false ) ]
  public class ParsedExpressionRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Offset to parsed expression data.
    /// </summary>
    private const int DEF_EXPRESSION_OFFSET = 4;
    #endregion

    #region Class members
    /// <summary>
    /// Size of the parsed expression.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usSize;
    /// <summary>
    /// Number of RTSXNAME records to follow this record.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usNameCount;
    /// <summary>
    /// Parsed expression.
    /// </summary>
    private byte[] m_arrParsedExpression;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  ParsedExpressionRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">When stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">When stream does not support read or seek operations.</exception>
    public  ParsedExpressionRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ParsedExpressionRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion
    
    #region Class properties
    /// <summary>
    /// Size of the parsed expression. Read-only.
    /// </summary>
    public ushort Size
    {
      get
      {
        return m_usSize;
      }
    }
    /// <summary>
    /// Number of RTSXNAME records to follow this record.
    /// </summary>
    public ushort NameCount
    {
      get
      {
        return m_usNameCount;
      }
      set
      {
        m_usNameCount = value;
      }
    }
    /// <summary>
    /// Parsed expression.
    /// </summary>
    public byte[] ParsedExpression
    {
      get
      {
        return m_arrParsedExpression;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_arrParsedExpression = value;
        m_usSize = ( ushort )m_arrParsedExpression.Length;
      }
    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_usSize = provider.ReadUInt16( iOffset + 0 );
      m_usNameCount = provider.ReadUInt16( iOffset + 2 );
      m_arrParsedExpression = new byte[ m_usSize ];
      provider.ReadArray( iOffset + DEF_EXPRESSION_OFFSET, m_arrParsedExpression );
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
      provider.WriteUInt16( iOffset + 0, m_usSize );
      provider.WriteUInt16( iOffset + 2, m_usNameCount );
      provider.WriteBytes( iOffset + DEF_EXPRESSION_OFFSET, m_arrParsedExpression );
      m_iLength += m_arrParsedExpression.Length + 4;
    }
    /// <summary>
    /// Evaluates size of the required storage space.
    /// </summary>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      return m_arrParsedExpression.Length + 4;
    }
    #endregion

  }
}

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

namespace Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords
{
  /// <summary>
  /// Note structure.
  /// </summary>
  [CLSCompliant( false )]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class ftPictFmla : ObjSubRecord
  {
    #region Constants
    /// <summary>
    /// Start of the string data.
    /// </summary>
    private const int FormulaStart = 14;
    /// <summary>
    /// Default header (data is unknown for us).
    /// </summary>
    private static readonly byte[] DefaultHeader = new byte[ 14 ]
      {
        0x1E, 0x00, 0x05, 0x00, 0x0C, 0x97, 0x41, 0x07, 0x02, 0x08, 0x08, 0xE8, 0x07, 0x03,
      };
    /// <summary>
    /// Default footer (data is unknown for us).
    /// </summary>
    private static readonly byte[] DefaultFooter = new byte[ 16 ]
      {
        0x00, 0x00, 0x00, 0x00, 0x44, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
      };
    #endregion

    #region Class members
    /// <summary>
    /// Internal data array.
    /// </summary>
    private byte[] m_arrHeader;
    /// <summary>
    /// Internal data array.
    /// </summary>
    private byte[] m_arrFooter;
    /// <summary>
    /// 
    /// </summary>
    private string m_strFormula;
    #endregion

    #region Class properties
    /// <summary>
    /// Formula value.
    /// </summary>
    public string Formula
    {
      get
      {
        return m_strFormula;
      }
      set
      {
        m_strFormula = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public ftPictFmla()
      : base( TObjSubRecordType.ftPictFmla )
    {
      int iLength = DefaultHeader.Length;
      m_arrHeader = new byte[ iLength ];
      Buffer.BlockCopy( DefaultHeader, 0, m_arrHeader, 0, iLength );

      iLength = DefaultFooter.Length;
      m_arrFooter = new byte[ iLength ];
      Buffer.BlockCopy( DefaultFooter, 0, m_arrFooter, 0, iLength );
    }
    /// <summary>
    /// Initializes new instance of subrecord.
    /// </summary>
    /// <param name="type">Type of the subrecord.</param>
    /// <param name="length">Length of the subrecord's data.</param>
    /// <param name="buffer">Buffer that contains subrecord's data.</param>
    public ftPictFmla( TObjSubRecordType type, ushort length, byte[] buffer )
      : base( type, length, buffer )
    {
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Parses byte array.
    /// </summary>
    /// <param name="buffer">Array to parse.</param>
    protected override void Parse( byte[] buffer )
    {
      //m_data = ( byte[] )buffer.Clone();
      m_arrHeader = new byte[ FormulaStart ];
      Buffer.BlockCopy( buffer, 0, m_arrHeader, 0, FormulaStart );

      int iOffset = FormulaStart;
      m_strFormula = BiffRecordRaw.GetString16BitUpdateOffset( buffer, ref iOffset );

      int iLengthLeft = buffer.Length - iOffset;
      m_arrFooter = new byte[ iLengthLeft ];
      Buffer.BlockCopy( buffer, iOffset, m_arrFooter, 0, iLengthLeft );
    }

    /// <summary>
    /// Fills array with binary representation of the subrecord.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer to copy data to.</param>
    public override void FillArray( DataProvider provider, int iOffset )
    {
      provider.WriteInt16( iOffset, ( short )Type );
      iOffset += 2;

      int iLength = GetStoreSize( ExcelVersion.Excel97to2003 ) - HeaderSize;
      provider.WriteInt16( iOffset, ( short )iLength );
      iOffset += 2;

      iLength = m_arrHeader.Length;
      provider.WriteBytes( iOffset, m_arrHeader, 0, iLength );
      iOffset += iLength;

      iOffset += provider.WriteString16Bit( iOffset, m_strFormula, false );

      iLength = m_arrFooter.Length;
      provider.WriteBytes( iOffset, m_arrFooter, 0, iLength );
      iOffset += iLength;
    }
    /// <summary>
    /// Clones current objects.
    /// </summary>
    /// <returns>Returns instance of cloned object.</returns>
    public override object Clone()
    {
      ftPictFmla result = ( ftPictFmla )base.Clone();

      result.m_arrHeader = CloneUtils.CloneByteArray( m_arrHeader );
      result.m_arrFooter = CloneUtils.CloneByteArray( m_arrFooter );

      return result;
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return HeaderSize + m_arrFooter.Length + m_arrHeader.Length + 3 + m_strFormula.Length;
      //( ( m_data != null ) ? m_data.Length : 0 );
    }
    #endregion
  }
}

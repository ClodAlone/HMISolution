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
  [Syncfusion.Documentation.DocumentationExclude()]
  [CLSCompliant( false )]
  public class ftSbs : ObjSubRecord
  {
    #region Class constants
    /// <summary>
    /// Size of the record.
    /// </summary>
    private const int DEF_RECORD_SIZE = 24;
    /// <summary>
    /// Default record data.
    /// </summary>
    private static readonly byte[] DEF_SAMPLE_RECORD_DATA = new byte[]
      {
        00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 01, 00, /*0x0C*/ /*07*/ 0x08, 00, 00, 00,
        0x10, 00, 00, 00,
      };
    #endregion

    #region Class members
    /// <summary>
    /// Internal data array.
    /// </summary>
    private byte[] m_data;
    private int m_iValue;
    private int m_iMinimum;
    private int m_iMaximum;
    private int m_iIncrement;
    private int m_iPage;
    private int m_iHorizontal;
    private int m_iScrollBarWidth;
    private short m_sOptions;
    #endregion

    #region Class properties
    /// <summary>
    /// Returns internal data array. Read-only.
    /// </summary>
    public byte[] Data
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
    public int Value
    {
      get
      {
        return m_iValue;
      }
      set
      {
        m_iValue = value;
      }
    }
    public int Minimum
    {
      get
      {
        return m_iMinimum;
      }
      set
      {
        m_iMinimum = value;
      }
    }
    public int Maximum
    {
      get
      {
        return m_iMaximum;
      }
      set
      {
        m_iMaximum = value;
      }
    }
    public int Increment
    {
      get
      {
        return m_iIncrement;
      }
      set
      {
        m_iIncrement = value;
      }
    }
    public int Page
    {
      get
      {
        return m_iPage;
      }
      set
      {
        m_iPage = value;
      }
    }
    public int Horizontal
    {
      get
      {
        return m_iHorizontal;
      }
      set
      {
        m_iHorizontal = value;
      }
    }
    public int ScrollBarWidth
    {
      get
      {
        return m_iScrollBarWidth;
      }
      set
      {
        m_iScrollBarWidth = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    [CLSCompliant( false )]
    public ftSbs()
      : base( TObjSubRecordType.ftSbs, DEF_RECORD_SIZE - 4, DEF_SAMPLE_RECORD_DATA )
    {
    }
    /// <summary>
    /// Initializes new instance of subrecord.
    /// </summary>
    /// <param name="type">Type of the subrecord.</param>
    /// <param name="length">Length of the subrecord's data.</param>
    /// <param name="buffer">Buffer that contains subrecord's data.</param>
    public ftSbs( TObjSubRecordType type, ushort length, byte[] buffer )
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
      m_data = ( byte[] )buffer.Clone();
      m_iValue = BitConverter.ToInt16( m_data, 4 );
      m_iMinimum = BitConverter.ToInt16( m_data, 6 );
      m_iMaximum = BitConverter.ToInt16( m_data, 8 );
      m_iIncrement = BitConverter.ToInt16( m_data, 10 );
      m_iPage = BitConverter.ToInt16( m_data, 12 );
      m_iHorizontal = BitConverter.ToInt16( m_data, 14 );
      m_iScrollBarWidth = BitConverter.ToInt16( m_data, 16 );
      m_sOptions = BitConverter.ToInt16( m_data, 18 );
    }

    protected override void Serialize( DataProvider provider, int iOffset )
    {
      provider.WriteInt32( iOffset, 0 ); //unused value
      iOffset += ExcelConstants.IntSize;

      provider.WriteInt16( iOffset, ( short )m_iValue );
      iOffset += ExcelConstants.ShortSize;

      provider.WriteInt16( iOffset, ( short )m_iMinimum );
      iOffset += ExcelConstants.ShortSize;

      provider.WriteInt16( iOffset, ( short )m_iMaximum );
      iOffset += ExcelConstants.ShortSize;

      provider.WriteInt16( iOffset, ( short )m_iIncrement );
      iOffset += ExcelConstants.ShortSize;

      provider.WriteInt16( iOffset, ( short )m_iPage );
      iOffset += ExcelConstants.ShortSize;

      provider.WriteInt16( iOffset, ( short )m_iHorizontal );
      iOffset += ExcelConstants.ShortSize;

      provider.WriteInt16( iOffset, ( short )m_iScrollBarWidth );
      iOffset += ExcelConstants.ShortSize;

      provider.WriteInt16( iOffset, m_sOptions ); // options - currently unused.

      //base.Serialize( provider, iOffset );
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
      ftSbs result = ( ftSbs )base.Clone();

      result.m_data = CloneUtils.CloneByteArray( m_data );

      return result;
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_RECORD_SIZE;
    }
    #endregion

  }
}

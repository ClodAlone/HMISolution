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
  public class ftNts : ObjSubRecord
  {
    #region Class constants
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 26;
    #endregion

    #region Class members
    /// <summary>
    /// Internal data array.
    /// </summary>
    private byte[] m_data;
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
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance of subrecord.
    /// </summary>
    /// <param name="type">Type of the subrecord.</param>
    /// <param name="length">Length of the subrecord's data.</param>
    /// <param name="buffer">Buffer that contains subrecord's data.</param>
    [CLSCompliant( false )]
    public ftNts( TObjSubRecordType type, ushort length, byte[] buffer )
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

      provider.WriteInt16( iOffset, ( short )DEF_RECORD_SIZE - 4 );
      iOffset += 2;

      provider.WriteBytes( iOffset, m_data, 0, m_data.Length );
    }
    /// <summary>
    /// Clones current objects.
    /// </summary>
    /// <returns>Returns instance of cloned object.</returns>
    public override object Clone()
    {
      ftNts result = ( ftNts )base.Clone();

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

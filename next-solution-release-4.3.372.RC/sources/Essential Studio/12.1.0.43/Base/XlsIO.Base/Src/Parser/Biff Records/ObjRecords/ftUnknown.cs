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
  /// <summary></summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [CLSCompliant( false )]
  public class ftUnknown : ObjSubRecord
  {
    #region Class members
    /// <summary>
    /// Subrecord's internal data.
    /// </summary>
    private byte[] m_data;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new subrecord.
    /// </summary>
    /// <param name="type">Type of the new subrecord.</param>
    /// <param name="length">Length of the subrecord.</param>
    /// <param name="buffer">Array that contains subrecord's data.</param>
    [CLSCompliant( false )]
    public ftUnknown( TObjSubRecordType type, ushort length, byte[] buffer )
      : base( type, length, buffer )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Returns internal data array. Read-only.
    /// </summary>
    public byte[] RecordData
    {
      get
      {
        return m_data;
      }
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Parses bye array.
    /// </summary>
    /// <param name="buffer">Array to parse.</param>
    protected override void Parse( byte[] buffer )
    {
      m_data = new byte[ Length ];
      Array.Copy( buffer, 0, m_data, 0, Length );
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

      provider.WriteInt16( iOffset, ( short )Length );
      iOffset += 2;

      provider.WriteBytes( iOffset, m_data, 0, m_data.Length );
      //BitConverter.GetBytes( ( ushort )Type ).CopyTo( arrBuffer, iOffset );
      //BitConverter.GetBytes( ( ushort ) Length ).CopyTo( arrBuffer, iOffset + 2 );
      //m_data.CopyTo( arrBuffer, iOffset + 4 );
    }
    /// <summary>
    /// Clones current objects.
    /// </summary>
    /// <returns>Returns instance of cloned object.</returns>
    public override object Clone()
    {
      ftUnknown result = ( ftUnknown )base.Clone();

      result.m_data = CloneUtils.CloneByteArray( m_data );

      return result;
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return Length + 4;
    }
    #endregion

  }
}

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
  /// Common object data.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [CLSCompliant( false )]
  public class ftCbls : ObjSubRecord
  {
    #region Class members
    /// <summary>
    /// Type of the object.
    /// </summary>
    private byte m_btChecked;
    #endregion

    #region Class Properties
    /// <summary>
    /// Indicates whether object is locked.
    /// </summary>
    public ExcelCheckState CheckState
    {
      get
      {
        return ( ExcelCheckState )m_btChecked;
      }
      set
      {
        m_btChecked = ( byte )value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public ftCbls()
      : base( TObjSubRecordType.ftCbls )
    {
    }
    /// <summary>
    /// Initialize new instance.
    /// </summary>
    /// <param name="type">Type of the subrecord.</param>
    /// <param name="length">Length of the subrecord's data.</param>
    /// <param name="buffer">Array that contains subrecord's data.</param>
    public ftCbls( TObjSubRecordType type, ushort length, byte[] buffer )
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
      if( buffer == null )
        throw new ArgumentNullException( "buffer" );

      m_btChecked = buffer[ 0 ];
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

      //BitConverter.GetBytes( ( short ) 18 ).CopyTo( arrBuffer, iOffset );
      short sLength = ( short )( GetStoreSize( ExcelVersion.Excel97to2003 ) - 4 );
      provider.WriteInt16( iOffset, sLength );
      iOffset += 2;

      provider.WriteByte( iOffset, m_btChecked );
      iOffset++;

      provider.WriteInt32( iOffset, 0 );
      iOffset += ExcelConstants.IntSize;

      provider.WriteInt32( iOffset, 0 );
      iOffset += ExcelConstants.IntSize;

      provider.WriteByte( iOffset, 0 );
      iOffset++;

      provider.WriteByte( iOffset, 3 );// Unknown value.
      iOffset++;

      provider.WriteByte( iOffset, 0 );
      iOffset++;
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return 16;
    }

    #endregion
  }
}

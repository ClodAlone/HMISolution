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
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords
{
  /// <summary>
  /// Base class for all obj subrecords.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public abstract class ObjSubRecord : ICloneable
  {
    #region Constants
    /// <summary>
    /// Size of the header data.
    /// </summary>
    protected const int HeaderSize = 4;
    #endregion

    #region Class members
    /// <summary>
    /// Type of the subrecord.
    /// </summary>
    private TObjSubRecordType m_Type;
    /// <summary>
    /// Length of the subrecord's data.
    /// </summary>
    private ushort m_usLength;
    #endregion

    #region Class Properties
    /// <summary>
    /// Type of the subrecord.
    /// </summary>
    public TObjSubRecordType Type
    {
      get
      {
        return m_Type;
      }
    }

    /// <summary>
    /// Length of the subrecord's data.
    /// </summary>
    [CLSCompliant( false )]
    public ushort Length
    {
      get
      {
        return m_usLength;
      }
      protected set
      {
        m_usLength = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Private constructor. To prevent creation without parameters.
    /// </summary>
    private ObjSubRecord()
    {

    }

    /// <summary>
    /// Initializes new instance.
    /// </summary>
    /// <param name="type">Type of the subrecord.</param>
    protected ObjSubRecord( TObjSubRecordType type )
    {
      m_Type = type;
    }

    /// <summary>
    /// Initializes new instance.
    /// </summary>
    /// <param name="type">Type of the subrecord.</param>
    /// <param name="length">Length of the subrecord's data.</param>
    /// <param name="buffer">Array that contains subrecord's data.</param>
    [CLSCompliant( false )]
    protected ObjSubRecord( TObjSubRecordType type, ushort length, byte[] buffer )
    {
      m_Type = type;
      m_usLength = length;

      //if( type == TObjSubRecordType.ftLbsData ) m_usLength = 16;

      Parse( buffer );
    }
    #endregion

    #region Class Helper Methods
    /// <summary>
    /// Parses byte array.
    /// </summary>
    /// <param name="buffer">Array to parse.</param>
    protected abstract void Parse( byte[] buffer );
    /// <summary>
    /// Fills array with binary representation of the subrecord.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer to copy data to.</param>
    public virtual void FillArray( DataProvider provider, int iOffset )
    {
      provider.WriteInt16( iOffset, ( short )Type );
      iOffset += 2;

      ushort usLength = ( ushort )( GetStoreSize( ExcelVersion.Excel97to2003 ) - HeaderSize );
      provider.WriteUInt16( iOffset, usLength );
      iOffset += 2;
      //provider.WriteByte( iOffset, 0 );
      //iOffset++;
      //provider.WriteByte( iOffset, 0 );
      //iOffset++;

      Serialize( provider, iOffset );
    }
    /// <summary>
    /// Serializes record's data.
    /// </summary>
    /// <param name="provider">Provider to serialize into.</param>
    /// <param name="iOffset">Offset to start serialization from.</param>
    protected virtual void Serialize( DataProvider provider, int iOffset )
    {
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public abstract int GetStoreSize( ExcelVersion version );
    #endregion

    #region ICloneable methods
    /// <summary>
    /// Clones current objects.
    /// </summary>
    /// <returns>Returns instance of cloned object.</returns>
    public virtual object Clone()
    {
      return MemberwiseClone();
    }
    #endregion
  }
}

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

namespace Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords
{
  /// <summary>
  /// Common object data.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [CLSCompliant( false )]
  public class ftCmo : ObjSubRecord
  {
    #region Class constants
    /// <summary>
    /// Bit mask for ChangeColor property.
    /// </summary>
    private const int DEF_CHANGE_COLOR_MASK = 0x0100;
    #endregion

    #region Class members
    /// <summary>
    /// Type of the object.
    /// </summary>
    private TObjType m_ObjType;
    /// <summary>
    /// Object's id.
    /// </summary>
    private ushort m_usId;
    /// <summary>
    /// Option flags.
    /// </summary>
    private ushort m_usOptions;
    /// <summary>
    /// Reserved.
    /// </summary>
    private byte[] m_reserved = new byte[ 12 ];
    /// <summary>
    /// Indicates that record length is zero.
    /// </summary>
    private bool m_bBadLength;
    #endregion

    #region Class Properties
    /// <summary>
    /// Indicates whether object is locked.
    /// </summary>
    public bool Locked
    {
      get
      {
        return ( ( m_usOptions & 0x01 ) > 0 );
      }
      set
      {
        if( value )
        {
          m_usOptions |= 0x01;
        }
        else
        {
          unchecked
          {
            m_usOptions &= ( ushort )( ~0x01 );
          }
        }
      }
    }

    /// <summary>
    /// Indicates whether object is printable.
    /// </summary>
    public bool Printable
    {
      get
      {
        return ( ( m_usOptions & 0x10 ) > 0 );
      }
      set
      {
        if( value )
        {
          m_usOptions |= 0x10;
        }
        else
        {
          unchecked
          {
            m_usOptions &= ( ushort )( ~0x10 );
          }
        }
      }
    }

    /// <summary>
    /// Indicates whether auto fill is turned on.
    /// </summary>
    public bool AutoFill
    {
      get
      {
        return ( ( m_usOptions & 0x2000 ) > 0 );
      }
      set
      {
        if( value )
        {
          m_usOptions |= 0x2000;
        }
        else
        {
          unchecked
          {
            m_usOptions &= ( ushort )( ~0x2000 );
          }
        }
      }
    }

    /// <summary>
    /// Indicates whether auto line option is turned on.
    /// </summary>
    public bool AutoLine
    {
      get
      {
        return ( ( m_usOptions & 0x4000 ) > 0 );
      }
      set
      {
        if( value )
        {
          m_usOptions |= 0x4000;
        }
        else
        {
          unchecked
          {
            m_usOptions &= ( ushort )( ~0x4000 );
          }
        }
      }
    }

    /// <summary>
    /// Indicates whether excel is allowed to change color of combo box (used in autofilters).
    /// </summary>
    public bool ChangeColor
    {
      get
      {
        return ( ( m_usOptions & DEF_CHANGE_COLOR_MASK ) > 0 );
      }
      set
      {
        if( value )
        {
          m_usOptions |= DEF_CHANGE_COLOR_MASK;
        }
        else
        {
          unchecked
          {
            m_usOptions &= ( ushort )( ~DEF_CHANGE_COLOR_MASK );
          }
        }
      }
    }

    /// <summary>
    /// Object's ID.
    /// </summary>
    public ushort ID
    {
      get
      {
        return m_usId;
      }
      set
      {
        m_usId = value;
      }
    }

    /// <summary>
    /// Object type.
    /// </summary>
    public TObjType ObjectType
    {
      get
      {
        return m_ObjType;
      }
      set
      {
        m_ObjType = value;
      }
    }
    /// <summary>
    /// Reserved.
    /// </summary>
    public byte[] Reserved
    {
      get
      {
        return m_reserved;
      }
      internal set
      {
        m_reserved = value;
      }
    }
    /// <summary>
    /// Returns record options. Read-only.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
      internal set
      {
        m_usOptions = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public ftCmo()
      : base( TObjSubRecordType.ftCmo )
    {
    }
    /// <summary>
    /// Initialize new instance.
    /// </summary>
    /// <param name="type">Type of the subrecord.</param>
    /// <param name="length">Length of the subrecord's data.</param>
    /// <param name="buffer">Array that contains subrecord's data.</param>
    public ftCmo( TObjSubRecordType type, ushort length, byte[] buffer )
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

      if( buffer.Length == 0 )
      {
        m_bBadLength = true;
        //System.Diagnostics.Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "Bad length of the buffer" );
        return;
      }

      m_ObjType = ( TObjType )BitConverter.ToInt16( buffer, 0 );
      m_usId = BitConverter.ToUInt16( buffer, 2 );
      m_usOptions = BitConverter.ToUInt16( buffer, 4 );

      int j = 0;
      for( int i = 6; i < 18; i++, j++ )
      {
        m_reserved[ j ] = buffer[ i ];
      }
    }

    /// <summary>
    /// Fills array with binary representation of the subrecord.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer to copy data to.</param>
    public override void FillArray( DataProvider provider, int iOffset )
    {
      if( !m_bBadLength )
      {
        //BitConverter.GetBytes( ( short ) Type ).CopyTo( arrBuffer, iOffset );
        provider.WriteInt16( iOffset, ( short )Type );
        iOffset += 2;

        //BitConverter.GetBytes( ( short ) 18 ).CopyTo( arrBuffer, iOffset );
        provider.WriteInt16( iOffset, 18 );
        iOffset += 2;

        //BitConverter.GetBytes( ( short )m_ObjType ).CopyTo( arrBuffer, iOffset );
        provider.WriteInt16( iOffset, ( short )m_ObjType );
        iOffset += 2;

        //BitConverter.GetBytes( m_usId ).CopyTo( arrBuffer, iOffset );
        provider.WriteUInt16( iOffset, m_usId );
        iOffset += 2;

        //BitConverter.GetBytes( m_usOptions ).CopyTo( arrBuffer, iOffset );
        provider.WriteUInt16( iOffset, m_usOptions );
        iOffset += 2;

        //m_reserved.CopyTo( arrBuffer, iOffset );
        provider.WriteBytes( iOffset, m_reserved, 0, m_reserved.Length );
        //iOffset += m_reserved.Length;
      }
      else
      {
        //BitConverter.GetBytes( ( short ) Type ).CopyTo( arrBuffer, iOffset );
        provider.WriteInt16( iOffset, ( short )Type );
        iOffset += 2;

        //BitConverter.GetBytes( ( short ) 0 ).CopyTo( arrBuffer, iOffset );
        provider.WriteInt16( iOffset, 0 );
        //iOffset += 2;
      }
    }
    /// <summary>
    /// Clones current objects.
    /// </summary>
    /// <returns>Returns instance of cloned object.</returns>
    public override object Clone()
    {
      ftCmo result = ( ftCmo )base.Clone();

      result.m_reserved = CloneUtils.CloneByteArray( m_reserved );

      return result;
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return ( !m_bBadLength )
        ? 22
        : 4;
    }

    #endregion
  }
}

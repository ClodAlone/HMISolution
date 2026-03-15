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
  /// This record contains PivotTable view fields and other information.
  /// </summary>
  [ Biff( TBIFFRecord.PivotViewFields ) ]
  [ CLSCompliant( false ) ]
  public class PivotViewFieldsRecord : BiffRecordRawWithArray
  {
    #region Class constants
    /// <summary>
    /// Length indicating that name is null.
    /// </summary>
    private const ushort DEF_NULL_LENGTH = 0xFFFF;
    /// <summary>
    /// Offset to the string data.
    /// </summary>
    private const int DEF_STRING_OFFSET = 10;
    #endregion

    #region Class members
    /// <summary>
    /// Axis.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usAxis;
    /// <summary>
    /// Number of subtotals attached.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usSubtotalCount;
    /// <summary>
    /// Item subtotal type.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usSubtotalType;
    /// <summary>
    /// Number of items.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usNumberItems;
    /// <summary>
    /// Length of the name; if it is equals to 0xFFFF then name is null
    /// and the name in the cache is used.
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usNameLength;
    /// <summary>
    /// Name.
    /// </summary>
    private string m_strName;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  PivotViewFieldsRecord()
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
    public  PivotViewFieldsRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  PivotViewFieldsRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Axis.
    /// </summary>
    public PivotAxisTypes Axis
    {
      get
      {
        return ( PivotAxisTypes )m_usAxis;
      }
      set
      {
        m_usAxis = ( ushort )value;
      }
    }
    /// <summary>
    /// Number of subtotals attached.
    /// </summary>
    public ushort SubtotalCount
    {
      get
      {
        return m_usSubtotalCount;
      }
      set
      {
        m_usSubtotalCount = value;
      }
    }
    /// <summary>
    /// Item subtotal type.
    /// </summary>
    public PivotSubtotalTypes SubtotalType
    {
      get
      {
        return ( PivotSubtotalTypes )m_usSubtotalType;
      }
      set
      {
        m_usSubtotalType = ( ushort )value;
      }
    }
    /// <summary>
    /// Number of items.
    /// </summary>
    public ushort NumberItems
    {
      get
      {
        return m_usNumberItems;
      }
      set
      {
        m_usNumberItems = value;
      }
    }
    /// <summary>
    /// Length of the name; if it is equals to 0xFFFF then name is null
    /// and the name in the cache is used.
    /// </summary>
    public ushort NameLength
    {
      get
      {
        return m_usNameLength;
      }
    }
    /// <summary>
    /// Name.
    /// </summary>
    public string Name
    {
      get
      {
        return m_strName;
      }
      set
      {
        m_strName = value;

        if( value == null )
        {
          m_usNameLength = DEF_NULL_LENGTH;
        }
        else
        {
          if( value.Length > DEF_NULL_LENGTH )
            throw new ArgumentOutOfRangeException( "value.Length", "Value cannot be greater DEF_NULL_LENGTH" );

          m_usNameLength = ( ushort )value.Length;
        }
      }
    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    public override void ParseStructure()
    {
      m_usAxis = GetUInt16( 0 );
      m_usSubtotalCount = GetUInt16( 2 );
      m_usSubtotalType = GetUInt16( 4 );
      m_usNumberItems = GetUInt16( 6 );
      m_usNameLength = GetUInt16( 8 );
      m_strName = null;

      if( m_usNameLength != DEF_NULL_LENGTH )
      {
        int iBytesCount;
        m_strName = GetString( DEF_STRING_OFFSET, m_usNameLength, out iBytesCount, false );
      }
    }
    /// <summary>
    /// In this method, the class must pack all of its properties into an
    /// internal data array, m_data. This method is called by
    /// FillStream when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      m_iLength = DEF_STRING_OFFSET;
      m_data = new byte[ m_iLength ];
      SetUInt16( 0, m_usAxis );
      SetUInt16( 2, m_usSubtotalCount );
      SetUInt16( 4, m_usSubtotalType );
      SetUInt16( 6, m_usNumberItems );
      SetUInt16( 8, m_usNameLength );

      if( m_strName != null )
      {
        m_iLength += SetStringNoLenDetectEncoding( DEF_STRING_OFFSET, m_strName );
      }
    }

    #endregion
  }
}

#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
using System.IO;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
	/// <summary>
	/// Summary description for SheetProtection.
	/// </summary>
	[ Biff( TBIFFRecord.SheetProtection ) ]
  [ CLSCompliant( false ) ]
	public class SheetProtectionRecord : BiffRecordRaw
	{
    #region Class constants
    /// <summary>
    /// Represents the Error Indicator sheet protection id.
    /// </summary>
    public const int ErrorIndicatorType = 3;
    /// <summary>
    /// Represents option offset.
    /// </summary>
    private const int DEF_OPTION_OFFSET = 19;
    /// <summary>
    /// Represents default record store size.
    /// </summary>
    private const int DEF_STORE_SIZE = 23;
    /// <summary>
    /// Represents default embedded record data.
    /// </summary>
    private readonly byte[] DEF_EMBEDED_DATA = new byte[] { 0, 2, 0, 1, 0xff, 0xff, 0xff, 0xff };
    #endregion

    #region Class members
    /// <summary>
    /// Options flag.
    /// </summary>
    [ BiffRecordPos( 19, 2 ) ]
    private ushort m_usOpt = 0x4400;
    /// <summary>
    /// Indicates is contain sheet protection.
    /// </summary>
    private bool m_bIsContainProtection;
    /// <summary>
    /// Represents the Protection type.
    /// </summary>
    private short m_sType;
    #endregion

    #region Class properties
    /// <summary>
    /// Represents protected options.
    /// </summary>
    public int ProtectedOptions
    {
      get
      {
        return m_usOpt;
      }
      set
      {
        m_usOpt = ( ushort )value;
      }
    }
    /// <summary>
    /// Indicates is record contain sheet protection.
    /// </summary>
    public bool ContainProtection
    {
      get
      {
        return m_bIsContainProtection;
      }
      set
      {
        m_bIsContainProtection = value;
      }
    }
    /// <summary>
    /// Read-only. Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return DEF_OPTION_OFFSET;
      }
    }
    /// <summary>
    /// Read-only. Maximum possible size of the record.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return DEF_STORE_SIZE;
      }
    }
    /// <summary>
    /// Represents the protection type.
    /// </summary>
    internal short Type
    {
        get
        {
            return m_sType;
        }
        set
        {
            m_sType = value;
        }

    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  SheetProtectionRecord()
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
    public  SheetProtectionRecord( Stream stream, out int itemSize )
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
    public  SheetProtectionRecord( int iReserve )
      : base( iReserve )
    {
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
      m_bIsContainProtection = iLength > DEF_OPTION_OFFSET;

      m_sType = provider.ReadInt16(iOffset + 12);

      if( m_bIsContainProtection )
        m_usOpt = ( ushort )provider.ReadInt32( iOffset + DEF_OPTION_OFFSET );
    }
    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_iLength = GetStoreSize( version );
      provider.WriteBytes( iOffset, new byte[ m_iLength ] );
      provider.WriteUInt16( iOffset + 0, ( ushort )TBIFFRecord.SheetProtection );

      if (m_sType==ErrorIndicatorType)
      { 
          iOffset += 12;
          provider.WriteInt16(iOffset, m_sType);
          iOffset += 2;
          provider.WriteByte(iOffset, 1);
          iOffset += 1;
          provider.WriteInt32(iOffset, 0);
      }
      else 
      {
          provider.WriteBytes(iOffset + 11, DEF_EMBEDED_DATA, 0, 8);
          provider.WriteUInt16(iOffset + DEF_OPTION_OFFSET, m_usOpt);
      }
    }
    /// <summary>
    /// Gets default record store size. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return ( m_bIsContainProtection )
        ? DEF_STORE_SIZE
        : DEF_OPTION_OFFSET;
    }
    #endregion
	}
}

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

using Syncfusion.XlsIO.Implementation.Exceptions;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record is part of the file protection. It contains information about
  /// the read/write password of the file. All record contents following this
  /// record will be encrypted.
  /// </summary>
  [ Biff( TBIFFRecord.FilePass ) ]
  public class FilePassRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Value of hash field indicating that standard encryption algorithm was used.
    /// </summary>
    internal const int DEF_STANDARD_HASH = 1;
    /// <summary>
    /// Value of hash field indicating that strong encryption algorithm was used.
    /// </summary>
    internal const int DEF_STRONG_HASH = 2;
    #endregion

    #region Class members

    /// <summary>
    /// Indicates whether weak (xor) or not weak encryption was used. 
    /// 0 - BIFF2-BIFF7 weak XOR encryption,
    /// 1 - BIFF8 standard encryption or BIFF8X strong encryption.
    /// </summary>
    private ushort m_usNotWeakEncryption = 0;
    /// <summary>
    /// Encryption key calculated from the read/write password.
    /// </summary>
    private ushort m_usKey;
    /// <summary>
    /// Hash value calculated from the read/write password.
    /// </summary>
    private ushort m_usHash;
    /// <summary>
    /// Record content for BIFF8 standard encryption.
    /// </summary>
    private FilePassStandardBlock m_standardBlock;
    /// <summary>
    /// Record content for BIFF8X strong encryption.
    /// </summary>
    private FilePassStrongBlock m_strongBlock;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  FilePassRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
    public  FilePassRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  FilePassRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Indicates whether weak encryption is used.
    /// </summary>
    public bool IsWeakEncryption
    {
      get
      {
        return m_usNotWeakEncryption == 0;
      }
      set
      {
        m_usNotWeakEncryption = ( ushort )( value ? 0 : 1 );
      }
    }

    /// <summary>
    /// Encryption key calculated from the read/write password.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ushort Key
    {
      get
      {
        return m_usKey;
      }
      set
      {
        m_usKey = value;
      }
    }
    /// <summary>
    /// Hash value calculated from the read/write password.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ushort Hash
    {
      get
      {
        return m_usHash;
      }
      set
      {
        m_usHash = value;
      }
    }

    /// <summary>
    /// Returns content for BIFF8 standard encryption. Read-only.
    /// </summary>
    public FilePassStandardBlock StandardBlock
    {
      get
      {
        return m_standardBlock;
      }
    }
    /// <summary>
    /// Creates internal standard encryption block.
    /// </summary>
    public void CreateStandardBlock()
    {
      m_standardBlock = new FilePassStandardBlock();
    }
    /// <summary>
    /// 
    /// </summary>
    public override bool NeedDecoding
    {
      get
      {
        return false;
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
      if( provider == null )
        throw new ArgumentNullException( "provider" );

      m_usNotWeakEncryption = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usKey = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usHash = provider.ReadUInt16( iOffset );
      iOffset += 2;

      if( IsWeakEncryption ) return;

      switch( m_usHash )
      {
        case DEF_STANDARD_HASH:
          m_standardBlock = new FilePassStandardBlock();
          m_standardBlock.ParseStructure( provider, iOffset, iLength );
          break;

        case DEF_STRONG_HASH:
          m_strongBlock = new FilePassStrongBlock();
          m_strongBlock.ParseStructure( provider, iOffset, iLength );
          break;

        default:
          throw new ParseException( "Cannot parse FilePass record" );
      }
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

      provider.WriteUInt16( iOffset, m_usNotWeakEncryption );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usKey );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usHash );
      iOffset += 2;

      if( !IsWeakEncryption )
      {
        if( m_usHash == DEF_STANDARD_HASH )
        {
          m_standardBlock.InfillInternalData( provider, iOffset, int.MaxValue );
        }
        else
        {
          throw new NotImplementedException();
        }
      }
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iResult = 6;

      if( !IsWeakEncryption )
      {
        if( m_usHash == DEF_STANDARD_HASH )
        {
          iResult += FilePassStandardBlock.GetStoreSize( version );
        }
        else
        {
          throw new NotImplementedException();
        }
      }

      return iResult;
    }
    #endregion
  }
}

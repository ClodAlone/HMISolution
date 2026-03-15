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
using System.IO;

namespace Syncfusion.XlsIO.Implementation.Security
{
  public class EncryptionHeader
  {
    #region Members
    /// <summary>
    /// An EncryptionHeaderFlags structure that specifies properties of the encryption algorithm used.
    /// </summary>
    private int m_iFlags;
    /// <summary>
    /// Reserved, MUST be 0x00000000.
    /// </summary>
    private int m_iSizeExtra;
    /// <summary>
    /// A signed integer that specifies the encryption algorithm.
    /// </summary>
    private int m_iAlgorithmId;
    /// <summary>
    /// A signed integer that specifies the hashing algorithm in concert with the Flags.fExternal bit.
    /// </summary>
    private int m_iAlgorithmIdHash;
    /// <summary>
    /// An unsigned integer that specifies the number of bits in the encryption key.
    /// MUST be a multiple of 8.
    /// </summary>
    private int m_iKeySize;
    /// <summary>
    /// An implementation specified value which corresponds to constants accepted by
    /// the specified CSP. MUST be compatible with the chosen CSP.
    /// </summary>
    private int m_iProviderType;
    /// <summary>
    /// Undefined and MUST be ignored.
    /// </summary>
    private int m_iReserved1;
    /// <summary>
    /// MUST be 0x00000000 and MUST be ignored.
    /// </summary>
    private int m_iReserved2;
    /// <summary>
    /// A null-terminated Unicode string that specifies the CSP name.
    /// </summary>
    private string m_strCSPName;
    #endregion

    #region Properties
    /// <summary>
    /// An EncryptionHeaderFlags structure that specifies properties of the encryption algorithm used.
    /// </summary>
    public int Flags
    {
      get
      {
        return m_iFlags;
      }
      set
      {
        m_iFlags = value;
      }
    }
    /// <summary>
    /// Reserved, MUST be 0x00000000.
    /// </summary>
    public int SizeExtra
    {
      get
      {
        return m_iSizeExtra;
      }
      set
      {
        m_iSizeExtra = value;
      }
    }
    /// <summary>
    /// A signed integer that specifies the encryption algorithm.
    /// </summary>
    public int AlgorithmId
    {
      get
      {
        return m_iAlgorithmId;
      }
      set
      {
        m_iAlgorithmId = value;
      }
    }
    /// <summary>
    /// A signed integer that specifies the hashing algorithm in concert with the Flags.fExternal bit.
    /// </summary>
    public int AlgorithmIdHash
    {
      get
      {
        return m_iAlgorithmIdHash;
      }
      set
      {
        m_iAlgorithmIdHash = value;
      }
    }
    /// <summary>
    /// An unsigned integer that specifies the number of bits in the encryption key.
    /// MUST be a multiple of 8.
    /// </summary>
    public int KeySize
    {
      get
      {
        return m_iKeySize;
      }
      set
      {
        m_iKeySize = value;
      }
    }
    /// <summary>
    /// An implementation specified value which corresponds to constants accepted by
    /// the specified CSP. MUST be compatible with the chosen CSP.
    /// </summary>
    public int ProviderType
    {
      get
      {
        return m_iProviderType;
      }
      set
      {
        m_iProviderType = value;
      }
    }
    /// <summary>
    /// Undefined and MUST be ignored.
    /// </summary>
    public int Reserved1
    {
      get
      {
        return m_iReserved1;
      }
      set
      {
        m_iReserved1 = value;
      }
    }
    /// <summary>
    /// MUST be 0x00000000 and MUST be ignored.
    /// </summary>
    public int Reserved2
    {
      get
      {
        return m_iReserved2;
      }
      set
      {
        m_iReserved2 = value;
      }
    }
    /// <summary>
    /// A null-terminated Unicode string that specifies the CSP name.
    /// </summary>
    public string CSPName
    {
      get
      {
        return m_strCSPName;
      }
      set
      {
        if( value == null || value.Length == 0 )
          throw new ArgumentOutOfRangeException();

        m_strCSPName = value;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public EncryptionHeader()
    {
    }
    /// <summary>
    /// Initializes new instance of the header and extracts its data from the stream.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    public EncryptionHeader( Stream stream )
    {
      Parse( stream );
    }
    /// <summary>
    /// Extracts item's data from the specified stream.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    public void Parse( Stream stream )
    {
      byte[] arrBuffer = new byte[ ExcelConstants.IntSize ];
      long lStart = stream.Position;
      int iHeaderSize = SecurityHelper.ReadInt32( stream, arrBuffer );
      m_iFlags = SecurityHelper.ReadInt32( stream, arrBuffer );
      m_iSizeExtra = SecurityHelper.ReadInt32( stream, arrBuffer );
      m_iAlgorithmId = SecurityHelper.ReadInt32( stream, arrBuffer );
      m_iAlgorithmIdHash = SecurityHelper.ReadInt32( stream, arrBuffer );
      m_iKeySize = SecurityHelper.ReadInt32( stream, arrBuffer );
      m_iProviderType = SecurityHelper.ReadInt32( stream, arrBuffer );
      m_iReserved1 = SecurityHelper.ReadInt32( stream, arrBuffer );
      m_iReserved2 = SecurityHelper.ReadInt32( stream, arrBuffer );
      m_strCSPName = SecurityHelper.ReadUnicodeStringZero( stream );

      // TODO: check this.
      stream.Position = lStart + iHeaderSize + ExcelConstants.IntSize;
    }
    /// <summary>
    /// Serialize item in the specified stream.
    /// </summary>
    /// <param name="stream">Stream to serialize data into.</param>
    public void Serialize( Stream stream )
    {
      long lStart = stream.Position;
      stream.Position += ExcelConstants.IntSize;
      SecurityHelper.WriteInt32( stream, m_iFlags );
      SecurityHelper.WriteInt32( stream, m_iSizeExtra );
      SecurityHelper.WriteInt32( stream, m_iAlgorithmId );
      SecurityHelper.WriteInt32( stream, m_iAlgorithmIdHash );
      SecurityHelper.WriteInt32( stream, m_iKeySize );
      SecurityHelper.WriteInt32( stream, m_iProviderType );
      SecurityHelper.WriteInt32( stream, m_iReserved1 );
      SecurityHelper.WriteInt32( stream, m_iReserved2 );
      SecurityHelper.WriteUnicodeStringZero( stream, m_strCSPName );
      long lEnd = stream.Position;
      int iHeaderSize = ( int )( lEnd - lStart ) - ExcelConstants.IntSize;
      stream.Position = lStart;
      SecurityHelper.WriteInt32( stream, iHeaderSize );
      stream.Position = lEnd;
    }
    #endregion
  }
}

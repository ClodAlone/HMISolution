#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
	/// <summary>
	/// Summary description for FilePassStrongBlock.
	/// </summary>
	[ CLSCompliant( false ) ]
	public class FilePassStrongBlock
	{
    #region Class members
    /// <summary>
    /// Option flags.
    /// </summary>
    private uint m_uiOptions;
    /// <summary>
    /// Unknown value.
    /// </summary>
    private uint m_uiReserved;
    /// <summary>
    /// Stream encryption algorithm identifier.
    /// 00006801H = RC4 (Ron's Code 4)
    /// 00006802H = SEAL (Secure Encryption Algorithm)
    /// </summary>
    private uint m_uiStreamEncryption;
    /// <summary>
    /// Password hashing algorithm identifier:
    /// 00008001H = MD2 (Message Digest 2)
    /// 00008002H = MD4 (Message Digest 4)
    /// 00008003H = MD5 (Message Digest 5)
    /// 00008004H = SHA-1 (Secure Hash Algorithm)
    /// </summary>
    private uint m_uiPassword;
    /// <summary>
    /// Hash key length (bits).
    /// </summary>
    private uint m_uiHashKeyLength;
    /// <summary>
    /// Cryptographic provider type:
    /// 00000001H = RSA
    /// 0000000CH = RSA SChannel
    /// 0000000DH = DSS and Diffie-Hellman
    /// 00000012H = DH SChannel
    /// 00000018H = RSA and AES
    /// </summary>
    private uint m_uiCryptographicProvider;
    /// <summary>
    /// Unknown or not used.
    /// </summary>
    private byte[] m_arrUnknown = new byte[ 8 ];
    /// <summary>
    /// Cryptographic provider name, Unicode character array with trailing null character.
    /// </summary>
    private string m_strProviderName;
    /// <summary>
    /// Unique document identifier used to initialize the encryption algorithm.
    /// </summary>
    private byte[] m_arrDocumentId;
    /// <summary>
    /// Encrypted document identifier used to verify the entered password.
    /// </summary>
    private byte[] m_arrEncryptedDocumentId;
    /// <summary>
    /// Digest used to verify the entered password.
    /// </summary>
    private byte[] m_arrDigest;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public FilePassStrongBlock()
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Option flags.
    /// </summary>
    public uint Options
    {
      get
      {
        return m_uiOptions;
      }
      set
      {
        m_uiOptions = value;
      }
    }
    /// <summary>
    /// Unknown value.
    /// </summary>
    public uint Reserved
    {
      get
      {
        return m_uiReserved;
      }
      set
      {
        m_uiReserved = value;
      }
    }
    /// <summary>
    /// Stream encryption algorithm identifier.
    /// 00006801H = RC4 (Ron's Code 4)
    /// 00006802H = SEAL (Secure Encryption Algorithm)
    /// </summary>
    public uint StreamEncryption
    {
      get
      {
        return m_uiStreamEncryption;
      }
      set
      {
        m_uiStreamEncryption = value;
      }
    }
    /// <summary>
    /// Password hashing algorithm identifier:
    /// 00008001H = MD2 (Message Digest 2)
    /// 00008002H = MD4 (Message Digest 4)
    /// 00008003H = MD5 (Message Digest 5)
    /// 00008004H = SHA-1 (Secure Hash Algorithm)
    /// </summary>
    public uint Password
    {
      get
      {
        return m_uiPassword;
      }
      set
      {
        m_uiPassword = value;
      }
    }
    /// <summary>
    /// Hash key length (bits).
    /// </summary>
    public uint HashKeyLength
    {
      get
      {
        return m_uiHashKeyLength;
      }
      set
      {
        m_uiHashKeyLength = value;
      }
    }
    /// <summary>
    /// Cryptographic provider type:
    /// 00000001H = RSA
    /// 0000000CH = RSA SChannel
    /// 0000000DH = DSS and Diffie-Hellman
    /// 00000012H = DH SChannel
    /// 00000018H = RSA and AES
    /// </summary>
    public uint CryptographicProvider
    {
      get
      {
        return m_uiCryptographicProvider;
      }
      set
      {
        m_uiCryptographicProvider = value;
      }
    }
    /// <summary>
    /// Unknown or not used. Read-only.
    /// </summary>
    public byte[] UnknownData
    {
      get
      {
        return m_arrUnknown;
      }
    }
    /// <summary>
    /// Cryptographic provider name, Unicode character array with trailing null character.
    /// </summary>
    public string ProviderName
    {
      get
      {
        return m_strProviderName;
      }
      set
      {
        m_strProviderName = value;
      }
    }
    /// <summary>
    /// Returns digest used to verify the entered password. Read-only.
    /// </summary>
    public byte[] Digest
    {
      get
      {
        return m_arrDigest;
      }
    }
    #endregion

    #region Class serialization methods
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    public int ParseStructure( DataProvider provider, int iOffset, int iLength )
    {
      if( provider == null )
        throw new ArgumentNullException( "provider" );

      m_uiOptions = provider.ReadUInt32( iOffset );
      iOffset += 4;

      // Start of info block reading.
      uint uiLength = provider.ReadUInt32( iOffset );
      uiLength += 4;
      int iStartOffset = iOffset;

      m_uiOptions = provider.ReadUInt32( iOffset );
      iOffset += 4;

      m_uiReserved = provider.ReadUInt32( iOffset );
      iOffset += 4;

      m_uiStreamEncryption = provider.ReadUInt32( iOffset );
      iOffset += 4;

      m_uiPassword = provider.ReadUInt32( iOffset );
      iOffset += 4;

      m_uiHashKeyLength = provider.ReadUInt32( iOffset );
      iOffset += 4;

      m_uiCryptographicProvider = provider.ReadUInt32( iOffset );
      iOffset += 4;

      iOffset = provider.ReadArray( iOffset, m_arrUnknown );

      // End of info block reading.
      iOffset = iStartOffset + ( int )uiLength;

      uiLength = provider.ReadUInt32( iOffset );
      uiLength += 4;
      m_arrDocumentId = new byte[ uiLength ];
      m_arrEncryptedDocumentId = new byte[ uiLength ];

      iOffset = provider.ReadArray( iOffset, m_arrDocumentId );
      iOffset = provider.ReadArray( iOffset, m_arrEncryptedDocumentId );

      uiLength = provider.ReadUInt32( iOffset );
      uiLength += 4;

      //m_arrDigest = new byte[ uiLength ];
      //provider.ReadArray( iOffset, m_arrDigest );

      throw new NotSupportedException( "Strong encryption algorithms are not supported." );
    }
    #endregion
  }
}


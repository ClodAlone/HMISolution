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
  public class EncryptionVerifier
  {
    #region Members
    /// <summary>
    /// An array of bytes that specifies the salt value used during password hash
    /// generation. MUST NOT be the same data used for the verifier stored encrypted
    /// in the EncryptedVerifier field.
    /// </summary>
    private byte[] m_arrSalt;
    /// <summary>
    /// MUST be the randomly generated Verifier value encrypted using the algorithm
    /// chosen by the implementation.
    /// </summary>
    private byte[] m_arrEncryptedVerifier = new byte[ 16 ];
    /// <summary>
    /// An array of bytes that contains the encrypted form of the hash of the randomly
    /// generated Verifier value. The length of the array MUST be the size of the
    /// encryption block size multiplied by the number of blocks needed to encrypt
    /// the hash of the Verifier. If the encryption algorithm is RC4, the length
    /// MUST be 20 bytes. If the encryption algorithm is AES, the length MUST be 32 bytes.
    /// </summary>
    private byte[] m_arrEncryptedVerifierHash;
    /// <summary>
    /// Size of the verifier hash.
    /// </summary>
    private int m_iVerifierHashSize;
    #endregion

    #region Properties
    /// <summary>
    /// An array of bytes that specifies the salt value used during password hash
    /// generation. MUST NOT be the same data used for the verifier stored encrypted
    /// in the EncryptedVerifier field.
    /// </summary>
    public byte[] Salt
    {
      get
      {
        return m_arrSalt;
      }
      set
      {
        m_arrSalt = value;
      }
    }
    /// <summary>
    /// MUST be the randomly generated Verifier value encrypted using the algorithm
    /// chosen by the implementation.
    /// </summary>
    public byte[] EncryptedVerifier
    {
      get
      {
        return m_arrEncryptedVerifier;
      }
      set
      {
        m_arrEncryptedVerifier = value;
      }
    }
    /// <summary>
    /// An array of bytes that contains the encrypted form of the hash of the randomly
    /// generated Verifier value. The length of the array MUST be the size of the
    /// encryption block size multiplied by the number of blocks needed to encrypt
    /// the hash of the Verifier. If the encryption algorithm is RC4, the length
    /// MUST be 20 bytes. If the encryption algorithm is AES, the length MUST be 32 bytes.
    /// </summary>
    public byte[] EncryptedVerifierHash
    {
      get
      {
        return m_arrEncryptedVerifierHash;
      }
      set
      {
        m_arrEncryptedVerifierHash = value;
      }
    }
    /// <summary>
    /// Gets/sets size of the verifier hash.
    /// </summary>
    public int VerifierHashSize
    {
      get
      {
        return m_iVerifierHashSize;
      }
      set
      {
        m_iVerifierHashSize = value;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public EncryptionVerifier()
    {
    }
    /// <summary>
    /// Initializes new instance of the verifier.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    public EncryptionVerifier( Stream stream )
    {
      Parse( stream );
    }
    /// <summary>
    /// Extracts object from stream.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    public void Parse( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      byte[] arrBuffer = new byte[ ExcelConstants.IntSize ];

      int iSaltSize = SecurityHelper.ReadInt32( stream, arrBuffer );
      m_arrSalt = new byte[ iSaltSize ];
      stream.Read( m_arrSalt, 0, iSaltSize );

      stream.Read( m_arrEncryptedVerifier, 0, m_arrEncryptedVerifier.Length );

      m_iVerifierHashSize = SecurityHelper.ReadInt32( stream, arrBuffer );
      int iVerifierRealSize = ( int )( stream.Length - stream.Position );
      m_arrEncryptedVerifierHash = new byte[ iVerifierRealSize ];
      stream.Read( m_arrEncryptedVerifierHash, 0, iVerifierRealSize );
    }
    /// <summary>
    /// Serializes object into stream.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    public void Serialize( Stream stream )
    {
      int iSaltSize = m_arrSalt.Length;
      SecurityHelper.WriteInt32( stream, iSaltSize );
      stream.Write( m_arrSalt, 0, iSaltSize );

      stream.Write( m_arrEncryptedVerifier, 0, m_arrEncryptedVerifier.Length );

      SecurityHelper.WriteInt32( stream, m_iVerifierHashSize );
      int iVerifierRealSize = m_arrEncryptedVerifierHash.Length;
      stream.Write( m_arrEncryptedVerifierHash, 0, iVerifierRealSize );
    }
    #endregion
  }
}

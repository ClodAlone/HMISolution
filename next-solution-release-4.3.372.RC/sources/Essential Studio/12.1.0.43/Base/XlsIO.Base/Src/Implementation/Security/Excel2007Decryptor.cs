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
//using Syncfusion.CompoundFile.XlsIO.Net;
using Syncfusion.CompoundFile.XlsIO;
using System.IO;
#if ( WINRT )
using Syncfusion.XlsIO.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif
using Syncfusion.XlsIO.Implementation.Exceptions;

namespace Syncfusion.XlsIO.Implementation.Security
{
  /// <summary>
  /// This class is responsible for decryption of Excel 2007 files.
  /// </summary>
  public class Excel2007Decryptor
  {
    #region Constants
    /// <summary>
    /// Size of the decryption block.
    /// </summary>
    private int BlockSize = 0x10;
    #endregion

    #region Members
    /// <summary>
    /// Dataspace map.
    /// </summary>
    private DataSpaceMap m_dataSpaceMap;
    /// <summary>
    /// Encryption info.
    /// </summary>
    protected EncryptionInfo m_info;
    /// <summary>
    /// Compound storage that should be decrypted.
    /// </summary>
    private ICompoundStorage m_storage;
    /// <summary>
    /// Array containing key data.
    /// </summary>
    protected byte[] m_arrKey;
    #endregion

    #region Properties
    /// <summary>
    /// Compound storage that should be decrypted.
    /// </summary>
    protected ICompoundStorage Storage
    {
        get
        {
            return m_storage;
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Decrypts internal storage storage.
    /// </summary>
    /// <returns>Decrypted stream.</returns>
    public virtual Stream Decrypt()
    {
      if( m_arrKey == null )
        throw new InvalidOperationException( "Incorrect password." );

      MemoryStream result = new MemoryStream();

      using( CompoundStream stream = m_storage.OpenStream( SecurityHelper.EncryptedPackageStream ) )
      {
        //First 8 bytes is length of the stream.
        byte[] arrInt64 = new byte[ ExcelConstants.LongSize ];
        stream.Read( arrInt64, 0, ExcelConstants.LongSize );
        int iLength = BitConverter.ToInt32( arrInt64, 0 );

        // We have decrypted data after it.
        int iMod = iLength % BlockSize;

        int iReadLength = ( iMod > 0 ) ?
          iLength + BlockSize - iMod :
          iLength;

        byte[] arrBuffer = new byte[ iReadLength ];
        stream.Read( arrBuffer, 0, iReadLength );
        byte[] arrResult = Decrypt( arrBuffer, m_arrKey );
        result.Write( arrResult, 0, iLength );
        result.Position = 0;
      }

      return result;
    }
    /// <summary>
    /// Prepares decryptor for actual decryption.
    /// </summary>
    /// <param name="storage">Compound storage to get required data from.</param>
    public void Initialize( ICompoundStorage storage )
    {
      if( storage == null )
        throw new ArgumentNullException( "storage" );

      m_storage = storage;

      using( Stream stream = storage.OpenStream( SecurityHelper.EncryptionInfoStream ) )
      {
        stream.Position = 0;
        m_info = new EncryptionInfo( stream );
      }

      using( ICompoundStorage dataSpaces = storage.OpenStorage( SecurityHelper.DataSpacesStorage ) )
      {
        ParseDataSpaceMap( dataSpaces );
        ParseTransfrom( dataSpaces );

        //List<DataSpaceReferenceComponent> lstComponents = entry.Components;

        //if( lstComponents.Count != 1 )
        //  throw new InvalidDataException();

        //DataSpaceReferenceComponent component = lstComponents[ 0 ];

        //if( component.ComponentType == 0 ) // stream
        //{
        //  // Decrypt encrypted component.
        //}
      }
    }
    /// <summary>
    /// Checks whether storage contains encrypted data.
    /// </summary>
    /// <param name="storage">Storage to check.</param>
    /// <returns>True if storage is encrypted.</returns>
    public static bool CheckEncrypted( ICompoundStorage storage )
    {
      return storage.ContainsStream( SecurityHelper.EncryptionInfoStream ) &&
        storage.ContainsStorage( SecurityHelper.DataSpacesStorage );
    }
    /// <summary>
    /// Checks whether password is correct.
    /// </summary>
    /// <param name="password">Password to check.</param>
    /// <returns>True if password verification succeeded.</returns>
    public virtual bool CheckPassword(string password)
    {
      EncryptionVerifier verifier = m_info.Verifier;
      m_arrKey = VerifyPassword( password, verifier );

      return ( m_arrKey != null );
    }
    /// <summary>
    /// Verifies password.
    /// </summary>
    /// <param name="password">Password to check.</param>
    /// <param name="verifier">Verifier object.</param>
    /// <returns>Encryption key.</returns>
    private byte[] VerifyPassword( string password, EncryptionVerifier verifier )
    {
      byte[] salt = verifier.Salt;
      byte[] arrKey = SecurityHelper.CreateKey( password, salt, 16 );
      //byte[] verifier2 = new byte[ verifier.EncryptedVerifier.Length + 4 ];
      //Buffer.BlockCopy( verifier.EncryptedVerifier, 0, verifier2, 0, verifier.EncryptedVerifier.Length );

      //byte[] arrVerifierHash = RijndaelDecrypt( verifier.EncryptedVerifierHash, arrKey, new byte[ 16 ] );
      byte[] arrVerifier = Decrypt( verifier.EncryptedVerifier, arrKey );//RijndaelDecrypt( verifier.EncryptedVerifier, arrKey, new byte[ 16 ] );
      byte[] arrVerifierHash = Decrypt( verifier.EncryptedVerifierHash, arrKey );
      SHA1 sha1 = 
#if !SILVERLIGHT && !WINRT && !WP
        new SHA1CryptoServiceProvider();
#else
        new SHA1Managed();
#endif
      byte[] arrNewHash = sha1.ComputeHash( arrVerifier );

      if( !Syncfusion.XlsIO.Parser.Biff_Records.BiffRecordRaw.CompareArrays(
        arrNewHash, 0,
        arrVerifierHash, 0,
        arrNewHash.Length ) )
      {
        arrKey = null;
        //throw new ArgumentOutOfRangeException( "Wrong password." );
      }

      return arrKey;
    }
    /// <summary>
    /// Decrypts specified buffer.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    private static byte[] Decrypt( byte[] data, byte[] key )
    {
      Aes aes = new Aes( Aes.KeySize.Bits128, key );
      return SecurityHelper.EncryptDecrypt( data, aes.InvCipher, key.Length );
    }
    /// <summary>
    /// Extracts transform data from the storage.
    /// </summary>
    /// <param name="dataSpaces">Storage to get data from.</param>
    private void ParseTransfrom( ICompoundStorage dataSpaces )
    {
      List<DataSpaceMapEntry> lstEntries = m_dataSpaceMap.MapEntries;

      if( lstEntries.Count != 1 )
        throw new InvalidDataException();

      DataSpaceMapEntry entry = lstEntries[ 0 ];
      string dataSpaceName = entry.DataSpaceName;
      string strTransformName = null;

      using( ICompoundStorage dataSpaceInfoStorage = dataSpaces.OpenStorage( SecurityHelper.DataSpaceInfoStorage ) )
      {
        using( Stream transformStream = dataSpaceInfoStorage.OpenStream( dataSpaceName ) )
        {
          DataSpaceDefinition definition = new DataSpaceDefinition( transformStream );
          List<string> lstTransforms = definition.TransformRefs;

          if( lstTransforms.Count != 1 )
            throw new InvalidDataException();

          strTransformName = lstTransforms[ 0 ];
        }
      }

      using( ICompoundStorage transformInfoStorage = dataSpaces.OpenStorage( SecurityHelper.TransformInfoStorage ) )
      {
        using( ICompoundStorage transformStorage = transformInfoStorage.OpenStorage( strTransformName ) )
        {
          ParseTransformInfo( transformStorage );
        }
      }
    }
    /// <summary>
    /// Extracts dataspace map from the storage.
    /// </summary>
    /// <param name="dataSpaces">Storage to get data from.</param>
    private void ParseDataSpaceMap( ICompoundStorage dataSpaces )
    {
      if( dataSpaces == null )
        throw new ArgumentNullException( "dataSpaces" );

      using( CompoundStream stream = dataSpaces.OpenStream( SecurityHelper.DataSpaceMapStream ) )
      {
        m_dataSpaceMap = new DataSpaceMap( stream );
      }
    }
    /// <summary>
    /// Extracts TransformInfo from the storage.
    /// </summary>
    /// <param name="transformStorage">Storage to get data from.</param>
    private void ParseTransformInfo( ICompoundStorage transformStorage )
    {
      using( Stream stream = transformStorage.OpenStream( SecurityHelper.TransformPrimaryStream ) )
      {
        TransformInfoHeader header = new TransformInfoHeader( stream );
        EncryptionTransformInfo encryptionTransformInfo = new EncryptionTransformInfo( stream );
      }
    }
    #endregion
  }
}

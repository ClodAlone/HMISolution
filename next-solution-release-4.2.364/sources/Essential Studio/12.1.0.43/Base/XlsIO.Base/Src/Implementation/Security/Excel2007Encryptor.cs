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
using Syncfusion.CompoundFile.XlsIO;
#if ( WINRT )
using Syncfusion.XlsIO.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif

namespace Syncfusion.XlsIO.Implementation.Security
{
  /// <summary>
  /// This class used to encrypt data using Excel 2007 encryption with AES 128
  /// encryption algorithm and SHA-1 hashing algorithm.
  /// </summary>
  class Excel2007Encryptor
  {
    #region Constants
    /// <summary>
    /// Key length.
    /// </summary>
    internal const int KeyLength = 16;
    /// <summary>
    /// Default version.
    /// </summary>
    private const int DefaultVersion = 0x20003;
    /// <summary>
    /// Default flags.
    /// </summary>
    private const int DefaultFlags = 0x24;
    /// <summary>
    /// Encryption algorithm id (AES-128).
    /// </summary>
    private const int AES128AlgorithmId = 0x660e;
    /// <summary>
    /// Hashing algorithm id (SHA-1).
    /// </summary>
    private const int SHA1AlgorithmHash = 0x8004;
    /// <summary>
    /// Provider type.
    /// </summary>
    private const int DefaultProviderType = 0x18;
    /// <summary>
    /// Default CSP name.
    /// </summary>
    private const string DefaultCSPName = "Microsoft Enhanced RSA and AES Cryptographic Provider (Prototype)";
    #endregion

    #region Methods
    /// <summary>
    /// Encrypts specified stream.
    /// </summary>
    /// <param name="data">Data to encrypt.</param>
    /// <param name="password">Password to use.</param>
    /// <param name="root">Root storage to put encrypted data into.</param>
    public virtual void Encrypt( Stream data, string password, ICompoundStorage root )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      if( password == null || password.Length == 0 )
        throw new ArgumentOutOfRangeException( "password" );

      // 1. EncryptionInfo stream
      byte[] arrKey = PrepareEncryptionInfo( root, password );
      // 2. DataSpaces substorage
      // 2a. DataSpace info substorage and StrongEncryptionDataSpace stream
      // 2b. TransformInfo substorage -> StrongEncryptionTransform substorage -> Primary stream
      PrepareDataSpaces( root );
      // 3. EncryptedPackage stream
      using( CompoundStream stream = root.CreateStream( SecurityHelper.EncryptedPackageStream ) )
      {
        long lLength = data.Length;
        byte[] arrLength = BitConverter.GetBytes( lLength );
        stream.Write( arrLength, 0, ExcelConstants.LongSize );

        Encrypt( data, arrKey, stream );
      }
    }
    /// <summary>
    /// Preparse data spaces structures inside specified storage.
    /// </summary>
    /// <param name="root">Storage to put DataSpaces inside.</param>
    private void PrepareDataSpaces( ICompoundStorage root )
    {
      if( root == null )
        throw new ArgumentNullException( "root" );

      using( ICompoundStorage dataSpaces = root.CreateStorage( SecurityHelper.DataSpacesStorage ) )
      {
        // DataSpaceInfo - storage
        SerializeDataSpaceInfo( dataSpaces );
        // TransformInfo - storage
        SerializeTransformInfo( dataSpaces );
        // Version - stream
        SerializeVersion( dataSpaces );
        //// DataSpaceMap - stream
        SerializeDataSpaceMap( dataSpaces );
      }
    }
    /// <summary>
    /// Serializes VersionInfo stream inside specified storage.
    /// </summary>
    /// <param name="dataSpaces">Storage to serialize VersionInfo into.</param>
    protected void SerializeVersion( ICompoundStorage dataSpaces )
    {
      if( dataSpaces == null )
        throw new ArgumentNullException( "dataSpaces" );

      using( CompoundStream stream = dataSpaces.CreateStream( SecurityHelper.VersionStream ) )
      {
        // We use only default value on the current moment.
        VersionInfo version = new VersionInfo();
        version.Serialize( stream );
      }
    }
    /// <summary>
    /// Serializes transformation info.
    /// </summary>
    /// <param name="dataSpaces">Storage to serialize into.</param>
    protected void SerializeTransformInfo( ICompoundStorage dataSpaces )
    {
      using( ICompoundStorage transformInfo = dataSpaces.CreateStorage( SecurityHelper.TransformInfoStorage ) )
      {
        using( ICompoundStorage storage = transformInfo.CreateStorage( SecurityHelper.StrongEncryptionTransformStream ) )
        {
          using( CompoundStream primary = storage.CreateStream( SecurityHelper.TransformPrimaryStream ) )
          {
            // NOTE: all values are default to standard Excel 2007 encryption.
            TransformInfoHeader header = new TransformInfoHeader();
            header.TransformType = 1;
            header.TransformId = "{FF9A3F03-56EF-4613-BDD5-5A41C1D07246}";
            header.TransformName = "Microsoft.Container.EncryptionTransform";
            header.ReaderVersion = 1;
            header.UpdaterVersion = 1;
            header.WriterVersion = 1;

            header.Serialize( primary );

            // TODO: there is some other data after the header.
            EncryptionTransformInfo info = new EncryptionTransformInfo();
            info.Name = "AES128";
            info.Serialize( primary );

            //SecurityHelper.WriteInt32( primary, 7 );
            //byte[] arrString = Encoding.ASCII.GetBytes( "AES128\0" );
            //primary.Write( arrString, 0, arrString.Length );
            //SecurityHelper.WriteInt32( primary, 0x10 );
            //SecurityHelper.WriteInt32( primary, 0 );
            //SecurityHelper.WriteInt32( primary, 0x04 );
            

            //throw new Exception( "The method or operation is not implemented." );
          }
        }
      }
    }
    /// <summary>
    /// Serializes dataspace info.
    /// </summary>
    /// <param name="dataSpaces">Storage to serialize into.</param>
    protected void SerializeDataSpaceInfo( ICompoundStorage dataSpaces )
    {
      using( ICompoundStorage dataSpaceInfo = dataSpaces.CreateStorage( SecurityHelper.DataSpaceInfoStorage ) )
      {
        using( CompoundStream stream = dataSpaceInfo.CreateStream( SecurityHelper.StrongEncryptionDataSpaceStream ) )
        {
          DataSpaceDefinition definition = new DataSpaceDefinition();
          definition.TransformRefs.Add( SecurityHelper.StrongEncryptionTransformStream );
          definition.Serialize( stream );
        }
      }
    }
    /// <summary>
    /// Serializes DataSpaceMap stream.
    /// </summary>
    /// <param name="dataSpaces">Storage to place stream into.</param>
    protected void SerializeDataSpaceMap( ICompoundStorage dataSpaces )
    {
      if( dataSpaces == null )
        throw new ArgumentNullException( "dataSpaces" );

      DataSpaceMap dataSpaceMap = new DataSpaceMap();
      DataSpaceMapEntry entry = new DataSpaceMapEntry();
      DataSpaceReferenceComponent component = new DataSpaceReferenceComponent( 0, SecurityHelper.EncryptedPackageStream );
      dataSpaceMap.MapEntries.Add( entry );
      entry.Components.Add( component );
      entry.DataSpaceName = "StrongEncryptionDataSpace";

      using( CompoundStream stream = dataSpaces.CreateStream( SecurityHelper.DataSpaceMapStream ) )
      {
        dataSpaceMap.Serialize( stream );
      }
    }
    /// <summary>
    /// Fills in EncryptionInfor record and stores it at appropriate stream.
    /// </summary>
    /// <param name="root">Root storage.</param>
    /// <param name="password">Encryption password.</param>
    /// <returns>Encryption key.</returns>
    protected virtual byte[] PrepareEncryptionInfo( ICompoundStorage root, string password )
    {
      byte[] salt = CreateSalt( KeyLength );
      byte[] arrKey = SecurityHelper.CreateKey( password, salt, KeyLength );

      byte[] arrVerifier = CreateSalt( KeyLength );
      SHA1 sha1 = 
#if !SILVERLIGHT && !WINRT && !WP
        new SHA1CryptoServiceProvider();
#else
        new SHA1Managed();
#endif

      using( CompoundStream stream = root.CreateStream( SecurityHelper.EncryptionInfoStream ) )
      {
        EncryptionInfo info = new EncryptionInfo();
        info.VersionInfo = DefaultVersion;
        info.Flags = DefaultFlags;

        EncryptionHeader header = info.Header;
        header.Flags = DefaultFlags;
        header.AlgorithmId = AES128AlgorithmId;
        header.AlgorithmIdHash = SHA1AlgorithmHash;
        header.KeySize = KeyLength * ExcelConstants.BitsInByte;
        header.ProviderType = DefaultProviderType;
        header.Reserved1 = 0;//0x0248d0e0; // ???
        header.Reserved2 = 0;
        header.CSPName = DefaultCSPName;

        EncryptionVerifier verifier = info.Verifier;
        verifier.Salt = salt;
        verifier.EncryptedVerifier = Encrypt( arrVerifier, arrKey );
        byte[] verifierHash = sha1.ComputeHash( arrVerifier );
        int iMod = verifierHash.Length % KeyLength;

        verifier.VerifierHashSize = verifierHash.Length;

        if( iMod != 0 )
        {
          verifierHash = SecurityHelper.CombineArray( verifierHash, new byte[ KeyLength - iMod ] );
        }

        verifier.EncryptedVerifierHash = Encrypt( verifierHash, arrKey );

        info.Serialize( stream );
      }

      return arrKey;
    }
    /// <summary>
    /// Creates random salt.
    /// </summary>
    /// <param name="length">Desired salt length.</param>
    /// <returns>Array with random data.</returns>
    protected byte[] CreateSalt( int length )
    {
      if( length <= 0 )
        throw new ArgumentOutOfRangeException( "length" );

      byte[] result = new byte[ length ];
      Random rnd = new Random( ( int )DateTime.Now.Ticks );

      int iMaxValue = byte.MaxValue + 1;

      for( int i = 0; i < length; i++ )
      {
        result[ i ] = ( byte )rnd.Next( iMaxValue );
      }

      return result;
    }
    /// <summary>
    /// Encrypts specified buffer.
    /// </summary>
    /// <param name="data">Data to encrypt.</param>
    /// <param name="key">Encryption key.</param>
    /// <returns>Encrypted data.</returns>
    private byte[] Encrypt( byte[] data, byte[] key )
    {
      Aes aes = new Aes( Aes.KeySize.Bits128, key );
      return SecurityHelper.EncryptDecrypt( data, aes.Cipher, key.Length );
    }
    /// <summary>
    /// Encrypt specified stream.
    /// </summary>
    /// <param name="stream">Stream to encrypt.</param>
    /// <param name="key">Encryption key.</param>
    /// <param name="output">Output stream.</param>
    private void Encrypt( Stream stream, byte[] key, Stream output )
    {
      Aes aes = new Aes( Aes.KeySize.Bits128, key );
      byte[] arrBuffer = new byte[ KeyLength ];
      byte[] arrBuffer2 = new byte[ KeyLength ];

      while( stream.Read( arrBuffer, 0, KeyLength ) > 0 )
      {
        aes.Cipher( arrBuffer, arrBuffer2 );
        output.Write( arrBuffer2, 0, KeyLength );
      }
    }
    #endregion
  }
}

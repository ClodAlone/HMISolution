#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Syncfusion.CompoundFile.DocIO;
#if WINRT
using Syncfusion.DocIO.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Security
{
    /// <summary>
    /// This class used to encrypt data using Standard encryption (Word 2007) with AES 128
    /// encryption algorithm and SHA-1 hashing algorithm.
    /// </summary>
    [CLSCompliant(false)]
    internal class StandardEncryptor
    {
        #region Constants
        /// <summary>
        /// Key length.
        /// </summary>
        private const int KeyLength = 16;
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

        #region Members
        /// <summary>
        /// Security helper contains utility methods for encryption/decryption.
        /// </summary>
        private SecurityHelper m_securityHelper = new SecurityHelper();
        #endregion

        #region Implementation
        /// <summary>
        /// Encrypts specified stream.
        /// </summary>
        /// <param name="data">Data to encrypt.</param>
        /// <param name="password">Password to use.</param>
        /// <param name="root">Root storage to put encrypted data into.</param>
        internal void Encrypt(Stream data, string password, ICompoundStorage root)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (password == null || password.Length == 0)
                throw new ArgumentOutOfRangeException("password");

            // 1. EncryptionInfo stream
            byte[] arrKey = PrepareEncryptionInfo(root, password);
            // 2. DataSpaces substorage
            // 2a. DataSpace info substorage and StrongEncryptionDataSpace stream
            // 2b. TransformInfo substorage -> StrongEncryptionTransform substorage -> Primary stream
            PrepareDataSpaces(root);
            // 3. EncryptedPackage stream
            using (CompoundStream stream = root.CreateStream(SecurityHelper.EncryptedPackageStream))
            {
                long lLength = data.Length;
                byte[] arrLength = BitConverter.GetBytes(lLength);
                stream.Write(arrLength, 0, DLSConstants.LongSize);

                Encrypt(data, arrKey, stream);
            }
        }
        /// <summary>
        /// Preparse data spaces structures inside specified storage.
        /// </summary>
        /// <param name="root">Storage to put DataSpaces inside.</param>
        private void PrepareDataSpaces(ICompoundStorage root)
        {
            if (root == null)
                throw new ArgumentNullException("root");

            using (ICompoundStorage dataSpaces = root.CreateStorage(SecurityHelper.DataSpacesStorage))
            {
                // DataSpaceInfo - storage
                SerializeDataSpaceInfo(dataSpaces);
                // TransformInfo - storage
                SerializeTransformInfo(dataSpaces);
                // Version - stream
                SerializeVersion(dataSpaces);
                // DataSpaceMap - stream
                SerializeDataSpaceMap(dataSpaces);
            }
        }
        /// <summary>
        /// Serializes VersionInfo stream inside specified storage.
        /// </summary>
        /// <param name="dataSpaces">Storage to serialize VersionInfo into.</param>
        private void SerializeVersion(ICompoundStorage dataSpaces)
        {
            if (dataSpaces == null)
                throw new ArgumentNullException("dataSpaces");

            using (CompoundStream stream = dataSpaces.CreateStream(SecurityHelper.VersionStream))
            {
                VersionInfo version = new VersionInfo();
                version.Serialize(stream);
            }
        }
        /// <summary>
        /// Serializes transformation info.
        /// </summary>
        /// <param name="dataSpaces">Storage to serialize into.</param>
        private void SerializeTransformInfo(ICompoundStorage dataSpaces)
        {
            using (ICompoundStorage transformInfo = dataSpaces.CreateStorage(SecurityHelper.TransformInfoStorage))
            {
                using (ICompoundStorage storage = transformInfo.CreateStorage(SecurityHelper.StrongEncryptionTransformStream))
                {
                    using (CompoundStream primary = storage.CreateStream(SecurityHelper.TransformPrimaryStream))
                    {
                        // NOTE: all values are default to Word 2007 encryption (standard encryption).
                        TransformInfoHeader header = new TransformInfoHeader();
                        header.TransformType = 1;
                        header.TransformId = "{FF9A3F03-56EF-4613-BDD5-5A41C1D07246}";
                        header.TransformName = "Microsoft.Container.EncryptionTransform";
                        header.ReaderVersion = 1;
                        header.UpdaterVersion = 1;
                        header.WriterVersion = 1;

                        header.Serialize(primary);

                        // TODO: there is some other data after the header.
                        EncryptionTransformInfo info = new EncryptionTransformInfo();
                        info.Name = string.Empty;
                        info.Serialize(primary);
                    }
                }
            }
        }
        /// <summary>
        /// Serializes dataspace info.
        /// </summary>
        /// <param name="dataSpaces">Storage to serialize into.</param>
        private void SerializeDataSpaceInfo(ICompoundStorage dataSpaces)
        {
            using (ICompoundStorage dataSpaceInfo = dataSpaces.CreateStorage(SecurityHelper.DataSpaceInfoStorage))
            {
                using (CompoundStream stream = dataSpaceInfo.CreateStream(SecurityHelper.StrongEncryptionDataSpaceStream))
                {
                    DataSpaceDefinition definition = new DataSpaceDefinition();
                    definition.TransformRefs.Add(SecurityHelper.StrongEncryptionTransformStream);
                    definition.Serialize(stream);
                }
            }
        }
        /// <summary>
        /// Serializes DataSpaceMap stream.
        /// </summary>
        /// <param name="dataSpaces">Storage to place stream into.</param>
        private void SerializeDataSpaceMap(ICompoundStorage dataSpaces)
        {
            if (dataSpaces == null)
                throw new ArgumentNullException("dataSpaces");

            DataSpaceMap dataSpaceMap = new DataSpaceMap();
            DataSpaceMapEntry entry = new DataSpaceMapEntry();
            DataSpaceReferenceComponent component = new DataSpaceReferenceComponent(0, SecurityHelper.EncryptedPackageStream);
            dataSpaceMap.MapEntries.Add(entry);
            entry.Components.Add(component);
            entry.DataSpaceName = "StrongEncryptionDataSpace";

            using (CompoundStream stream = dataSpaces.CreateStream(SecurityHelper.DataSpaceMapStream))
            {
                dataSpaceMap.Serialize(stream);
            }
        }
        /// <summary>
        /// Prepare EncryptionInfo record and stores it in appropriate stream.
        /// </summary>
        /// <param name="root">Root storage.</param>
        /// <param name="password">Encryption password.</param>
        /// <returns>Encryption key.</returns>
        private byte[] PrepareEncryptionInfo(ICompoundStorage root, string password)
        {
            byte[] salt = CreateSalt(KeyLength);
            byte[] arrKey = m_securityHelper.CreateKey(password, salt, KeyLength);

            byte[] arrVerifier = CreateSalt(KeyLength);
            SHA1 sha1 = new SHA1Managed();

            using (CompoundStream stream = root.CreateStream(SecurityHelper.EncryptionInfoStream))
            {
                StandardEncryptionInfo info = new StandardEncryptionInfo();
                info.VersionInfo = DefaultVersion;
                info.Flags = DefaultFlags;

                EncryptionHeader header = info.Header;
                header.Flags = DefaultFlags;
                header.AlgorithmId = AES128AlgorithmId;
                header.AlgorithmIdHash = SHA1AlgorithmHash;
                header.KeySize = KeyLength * DLSConstants.BitsInByte;
                header.ProviderType = DefaultProviderType;
                header.Reserved1 = 0;
                header.Reserved2 = 0;
                header.CSPName = DefaultCSPName;

                EncryptionVerifier verifier = info.Verifier;
                verifier.Salt = salt;
                verifier.EncryptedVerifier = Encrypt(arrVerifier, arrKey);
                byte[] verifierHash = sha1.ComputeHash(arrVerifier);
                int iMod = verifierHash.Length % KeyLength;

                verifier.VerifierHashSize = verifierHash.Length;

                if (iMod != 0)
                {
                    verifierHash = m_securityHelper.CombineArray(verifierHash, new byte[KeyLength - iMod]);
                }

                verifier.EncryptedVerifierHash = Encrypt(verifierHash, arrKey);

                info.Serialize(stream);
            }

            return arrKey;
        }
        /// <summary>
        /// Creates random salt.
        /// </summary>
        /// <param name="length">Desired salt length.</param>
        /// <returns>Array with random data.</returns>
        private byte[] CreateSalt(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException("length");

            byte[] result = new byte[length];
            Random rnd = new Random((int)DateTime.Now.Ticks);

            int iMaxValue = byte.MaxValue + 1;

            for (int i = 0; i < length; i++)
            {
                result[i] = (byte)rnd.Next(iMaxValue);
            }

            return result;
        }
        /// <summary>
        /// Encrypts specified buffer.
        /// </summary>
        /// <param name="data">Data to encrypt.</param>
        /// <param name="key">Encryption key.</param>
        /// <returns>Encrypted data.</returns>
        private byte[] Encrypt(byte[] data, byte[] key)
        {
            Aes aes = new Aes(Aes.KeySize.Bits128, key);
            return m_securityHelper.EncryptDecrypt(data, aes.Cipher, key.Length);
        }
        /// <summary>
        /// Encrypt specified stream.
        /// </summary>
        /// <param name="stream">Stream to encrypt.</param>
        /// <param name="key">Encryption key.</param>
        /// <param name="output">Output stream.</param>
        private void Encrypt(Stream stream, byte[] key, Stream output)
        {
            Aes aes = new Aes(Aes.KeySize.Bits128, key);
            byte[] arrBuffer = new byte[KeyLength];
            byte[] arrBuffer2 = new byte[KeyLength];

            while (stream.Read(arrBuffer, 0, KeyLength) > 0)
            {
                aes.Cipher(arrBuffer, arrBuffer2);
                output.Write(arrBuffer2, 0, KeyLength);
            }
        }
        #endregion
    }
}

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
    /// This class used to encrypt data using Agile encryption (Word 2010/2013).
    /// </summary>
    [CLSCompliant(false)]
    internal class AgileEncryptor
    {
        #region Constants
        /// <summary>
        /// Default version.
        /// </summary>
        private const int DefaultVersion = 0x40004;
        /// <summary>
        /// Reserved bytes.
        /// </summary>
        private const int Reserved = 0x40;
        /// <summary>
        /// Segment size.
        /// </summary>
        private const int SegmentSize = 4096;
        #endregion

        #region Members
        /// <summary>
        /// Security helper contains utility methods for encryption/decryption.
        /// </summary>
        private SecurityHelper m_securityHelper = new SecurityHelper();
        /// <summary>
        /// Hashing Algorithm used to compute hash.
        /// </summary>
        private HashAlgorithm m_hashAlgorithm = new SHA1Managed();
        /// <summary>
        /// Computes a Hash-based Message Authentication Code (HMAC) using the System.Security.Cryptography.SHA1 hash function.
        /// </summary>
        private HMAC m_hmacSha = new HMACSHA1();
        /// <summary>
        /// Hashing Algorithm name used to compute hash.
        /// </summary>
        private string m_hashAlgorithmName = "SHA1";
        /// <summary>
        /// Encryption key bits.
        /// </summary>
        private int m_keyBits = 128;
        /// <summary>
        /// Hash size.
        /// </summary>
        private int m_hashSize = 20;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes encryptor for agile encryption.
        /// </summary>
        internal AgileEncryptor()
        {
        }
        /// <summary>
        /// Initializes encryptor for agile encryption.
        /// </summary>
        /// <param name="hashAlgorithm">The hash algorithm.</param>
        /// <param name="keyBits">The key bits.</param>
        /// <param name="hashSize">Size of the hash.</param>
        internal AgileEncryptor(string hashAlgorithm, int keyBits, int hashSize)
        {
#if (!SILVERLIGHT && !WP) || WINRT
            m_hashAlgorithmName = hashAlgorithm;
            m_keyBits = keyBits;
            m_hashSize = hashSize;
            //Initializes the SHA512 hashing algorithm used for encryption.
            if (m_hashAlgorithmName == "SHA512")
            {
                m_hashAlgorithm = new SHA512Managed();
                m_hmacSha = new HMACSHA512();
            }
#endif
        }
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

            PrepareEncryptionInfo(data, root, password);

            PrepareDataSpaces(root);
        }
        /// <summary>
        /// Prepares the encryption info.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="root">The root.</param>
        /// <param name="password">The password.</param>
        private void PrepareEncryptionInfo(Stream data, ICompoundStorage root, string password)
        {
            using (CompoundStream stream = root.CreateStream(SecurityHelper.EncryptionInfoStream))
            {
                AgileEncryptionInfo info = new AgileEncryptionInfo();
                info.VersionInfo = DefaultVersion;
                info.Reserved = Reserved;

                #region Key Data
                // Generate Key Data.
                KeyData keyData = info.XmlEncryptionDescriptor.KeyData;
                InitializeKeyData(keyData);
                keyData.Salt = CreateSalt(keyData.SaltSize);
                #endregion

                #region Encrypted Key
                // Generate Encrypted Key.
                EncryptedKey encryptorKey = info.XmlEncryptionDescriptor.KeyEncryptors.EncryptedKey;
                InitializeEncryptedKey(encryptorKey);
                encryptorKey.Salt = CreateSalt(encryptorKey.SaltSize);

                byte[] randomBytes = CreateSalt(encryptorKey.SaltSize);
                byte[] encryptedVerifierHashInputBlockKey = new byte[] { 0xfe, 0xa7, 0xd2, 0x76, 0x3b, 0x4b, 0x9e, 0x79 };
                byte[] arrKey = m_securityHelper.CreateAgileEncryptionKey(m_hashAlgorithm, password, encryptorKey.Salt, encryptedVerifierHashInputBlockKey, encryptorKey.KeyBits >> 3, encryptorKey.SpinCount);
                encryptorKey.EncryptedVerifierHashInput = Encrypt(randomBytes, encryptorKey.BlockSize, arrKey, encryptorKey.Salt);

                byte[] encryptedVerifierHashValueBlockKey = new byte[] { 0xd7, 0xaa, 0x0f, 0x6d, 0x30, 0x61, 0x34, 0x4e };
                arrKey = m_securityHelper.CreateAgileEncryptionKey(m_hashAlgorithm, password, encryptorKey.Salt, encryptedVerifierHashValueBlockKey, encryptorKey.KeyBits >> 3, encryptorKey.SpinCount);
                encryptorKey.EncryptedVerifierHashValue = Encrypt(m_hashAlgorithm.ComputeHash(randomBytes), encryptorKey.BlockSize, arrKey, encryptorKey.Salt);

                // keyData.KeyBits is 128 (Word 2010) / 256 (Word 2013) bits.
                byte[] intermediateKey = CreateSalt(keyData.KeyBits / 8);
                byte[] encryptedKeyValueBlockKey = new byte[] { 0x14, 0x6e, 0x0b, 0xe7, 0xab, 0xac, 0xd0, 0xd6 };
                arrKey = m_securityHelper.CreateAgileEncryptionKey(m_hashAlgorithm, password, encryptorKey.Salt, encryptedKeyValueBlockKey, encryptorKey.KeyBits >> 3, encryptorKey.SpinCount);
                encryptorKey.EncryptedKeyValue = Encrypt(intermediateKey, encryptorKey.BlockSize, arrKey, encryptorKey.Salt);
                #endregion

                #region Data Integrity
                // Generate Data Integrity.
                DataIntegrity dataIntegrity = info.XmlEncryptionDescriptor.DataIntegrity;
                byte[] encryptedDataIntegrityHmacKeyBlockKey = new byte[] { 0x5f, 0xb2, 0xad, 0x01, 0x0c, 0xb9, 0xe1, 0xf6 };
                byte[] dataIntegrityHmackeyIV = m_hashAlgorithm.ComputeHash(m_securityHelper.CombineArray(keyData.Salt, encryptedDataIntegrityHmacKeyBlockKey));
                dataIntegrityHmackeyIV = m_securityHelper.CorrectSize(dataIntegrityHmackeyIV, keyData.BlockSize, 0x00);
                byte[] salt = CreateSalt(keyData.HashSize);
                dataIntegrity.EncryptedHmacKey = Encrypt(salt, keyData.BlockSize, intermediateKey, dataIntegrityHmackeyIV);

                // Create encrypted package stream.
                byte[] encryptedPackage = PrepareEncryptedPackage(data, root, keyData, intermediateKey);

                // Generation of Hmac value.
                m_hmacSha.Key = m_securityHelper.CorrectSize(salt, keyData.HashSize, 0x00);

                byte[] encryptedDataIntegrityHmacValueBlockKey = new byte[] { 0xa0, 0x67, 0x7f, 0x02, 0xb2, 0x2c, 0x84, 0x33 };
                byte[] dataIntegrityHmacValueIV = m_hashAlgorithm.ComputeHash(m_securityHelper.CombineArray(keyData.Salt, encryptedDataIntegrityHmacValueBlockKey));
                dataIntegrityHmacValueIV = m_securityHelper.CorrectSize(dataIntegrityHmacValueIV, keyData.BlockSize, 0x00);
                dataIntegrity.EncryptedHmacValue = Encrypt(m_hmacSha.ComputeHash(encryptedPackage), keyData.BlockSize, intermediateKey, dataIntegrityHmacValueIV);
                #endregion

                info.Serialize(stream);
            }
        }
        /// <summary>
        /// Prepares the encrypted package.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="root">The root.</param>
        /// <param name="keyData">The key data.</param>
        /// <param name="intermediateKey">The intermediate key.</param>
        /// <returns></returns>
        private byte[] PrepareEncryptedPackage(Stream data, ICompoundStorage root, KeyData keyData, byte[] intermediateKey)
        {
            byte[] encryptedPackage = BitConverter.GetBytes(data.Length);

            using (CompoundStream encryptedPackageStream = root.CreateStream(SecurityHelper.EncryptedPackageStream))
            {
                int segmentCount = (int)(data.Length / SegmentSize);
                if (data.Length % SegmentSize != 0)
                    segmentCount += 1;

                for (int i = 0; i < segmentCount; i++)
                {
                    int size = Math.Min(SegmentSize, (int)(data.Length - i * SegmentSize));
                    byte[] inputBuf = new byte[size];
                    byte[] outputBuf = new byte[size];
                    data.Read(inputBuf, 0, size);
                    byte[] IV = m_hashAlgorithm.ComputeHash(m_securityHelper.CombineArray(keyData.Salt, BitConverter.GetBytes(i)));
                    outputBuf = Encrypt(inputBuf, keyData.BlockSize, intermediateKey, IV);
                    encryptedPackage = m_securityHelper.CombineArray(encryptedPackage, outputBuf);
                }
                encryptedPackageStream.Write(encryptedPackage, 0, encryptedPackage.Length);
            }
            return encryptedPackage;
        }
        /// <summary>
        /// Initializes the key data.
        /// </summary>
        /// <param name="keyData">The key data.</param>
        private void InitializeKeyData(KeyData keyData)
        {
            keyData.SaltSize = 16;
            keyData.BlockSize = 16;
            keyData.KeyBits = m_keyBits;
            keyData.HashSize = m_hashSize;
            keyData.CipherAlgorithm = "AES";
            keyData.CipherChaining = "ChainingModeCBC";
            keyData.HashAlgorithm = m_hashAlgorithmName;
        }
        /// <summary>
        /// Initializes the encrypted key.
        /// </summary>
        /// <param name="key">The key.</param>
        private void InitializeEncryptedKey(EncryptedKey key)
        {
            key.SpinCount = 100000;
            key.SaltSize = 16;
            key.BlockSize = 16;
            key.KeyBits = m_keyBits;
            key.HashSize = m_hashSize;
            key.CipherAlgorithm = "AES";
            key.CipherChaining = "ChainingModeCBC";
            key.HashAlgorithm = m_hashAlgorithmName;
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
                        // NOTE: all values are default to standard Excel 2007 encryption.
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
        /// Creates the salt.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
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
        /// Encrypts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="blockSize">Size of the block.</param>
        /// <param name="key">The key.</param>
        /// <param name="IV">The IV.</param>
        /// <returns></returns>
        private byte[] Encrypt(byte[] data, int blockSize, byte[] key, byte[] IV)
        {
            int iLength = data.Length;
            byte[] input;
            if (iLength % blockSize != 0)
            {
                iLength = (iLength / blockSize + 1) * blockSize;
                input = m_securityHelper.CorrectSize(data, iLength, 0x00);
            }
            else
                input = data;
            byte[] result = new byte[iLength];
            byte[] buffer = new byte[blockSize];
            byte[] arrTemp = new byte[blockSize];
            Aes.KeySize keySize = Aes.KeySize.Bits128;
            if (key.Length == 32)
                keySize = Aes.KeySize.Bits256;
            Aes aes = new Aes(keySize, key);
            int iOffset = 0;

            while (iOffset < iLength)
            {                
                Buffer.BlockCopy(input, iOffset, buffer, 0, blockSize);
                if(iOffset == 0)
                    buffer = m_securityHelper.ConcatenateIV(buffer, IV);
                else
                    buffer = m_securityHelper.ConcatenateIV(buffer, arrTemp);
                aes.Cipher(buffer, arrTemp);
                Buffer.BlockCopy(arrTemp, 0, result, iOffset, blockSize);
                iOffset += blockSize;
            }
            return result;
        }
        #endregion
    }
}

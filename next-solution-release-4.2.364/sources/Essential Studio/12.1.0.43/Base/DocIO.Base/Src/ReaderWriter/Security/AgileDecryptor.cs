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
using Syncfusion.CompoundFile.DocIO;
using System.IO;
using Syncfusion.DocIO;
#if WINRT
using Syncfusion.DocIO.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Security
{
    /// <summary>
    /// This class is responsible for decryption of Agile encryption (Word 2010/2013) files.
    /// </summary>
    [CLSCompliant(false)]
    internal class AgileDecryptor
    {
        #region Constants
        /// <summary>
        /// Segment size.
        /// </summary>
        private const int SegmentSize = 4096;
        #endregion

        #region Members
        /// <summary>
        /// Dataspace map.
        /// </summary>
        private DataSpaceMap m_dataSpaceMap;
        /// <summary>
        /// Encryption info.
        /// </summary>
        private AgileEncryptionInfo m_info;
        /// <summary>
        /// Compound storage that should be decrypted.
        /// </summary>
        private ICompoundStorage m_storage;
        /// <summary>
        /// Intermediate key.
        /// </summary>
        private byte[] m_intermediateKey;
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
        #endregion

        #region Implementation
        /// <summary>
        /// Decrypts internal storage.
        /// </summary>
        /// <returns>Decrypted stream.</returns>
        internal Stream Decrypt()
        {
            if (m_intermediateKey == null)
                throw new InvalidOperationException("Incorrect password.");

            MemoryStream result = new MemoryStream();
            KeyData keyData = m_info.XmlEncryptionDescriptor.KeyData;

            using (CompoundStream stream = m_storage.OpenStream(SecurityHelper.EncryptedPackageStream))
            {
                //First 8 bytes is length of the stream.
                byte[] arrSize = new byte[DLSConstants.LongSize];
                stream.Read(arrSize, 0, DLSConstants.LongSize);
                int iLength = BitConverter.ToInt32(arrSize, 0);

                int iMod = iLength % keyData.BlockSize;

                int iReadLength = (iMod > 0) ?
                  iLength + keyData.BlockSize - iMod :
                  iLength;

                byte[] arrData = new byte[iReadLength];
                stream.Read(arrData, 0, iReadLength);

                byte[] encryptedPackage = m_securityHelper.CombineArray(arrSize, arrData);

                if (!CheckEncryptedPackage(encryptedPackage))
                    throw new Exception("Encrypted package is invalid");

                #region Decrypts encrypted data
                byte[] arrResult = new byte[iLength];
                int segmentCount = (iReadLength % SegmentSize == 0) ? (int)(iReadLength / SegmentSize) : (int)(iReadLength / SegmentSize) + 1;
                
                for (int i = 0; i < segmentCount; i++)
                {
                    int size = Math.Min(SegmentSize, (int)(iReadLength - i * SegmentSize));
                    byte[] inputBuf = new byte[size];
                    byte[] outputBuf = new byte[size];
                    Buffer.BlockCopy(arrData, i * SegmentSize, inputBuf, 0, size);
                    byte[] IV = m_hashAlgorithm.ComputeHash(m_securityHelper.CombineArray(keyData.Salt, BitConverter.GetBytes(i)));
                    outputBuf = Decrypt(inputBuf, keyData.BlockSize, m_intermediateKey, IV, size);
                    size = (i == segmentCount - 1) ? (size - (iReadLength - iLength)) : size;
                    Buffer.BlockCopy(outputBuf, 0, arrResult, i * SegmentSize, size);
                }
                result.Write(arrResult, 0, iLength);
                result.Position = 0;
                #endregion
            }

            return result;
        }
        /// <summary>
        /// Prepares decryptor for actual decryption.
        /// </summary>
        /// <param name="storage">Compound storage to get required data.</param>
        internal void Initialize(ICompoundStorage storage)
        {
            if (storage == null)
                throw new ArgumentNullException("storage");

            m_storage = storage;

            using (Stream stream = storage.OpenStream(SecurityHelper.EncryptionInfoStream))
            {
                m_info = new AgileEncryptionInfo(stream);
                //Initializes the SHA512 hashing algorithm used for encryption.
                if (m_info.XmlEncryptionDescriptor.KeyEncryptors.EncryptedKey.HashAlgorithm == "SHA512")
                {
#if (!SILVERLIGHT && !WP) || WINRT
                    m_hashAlgorithm = new SHA512Managed();
                    m_hmacSha = new HMACSHA512();
#endif
                }
            }

            using (ICompoundStorage dataSpaces = storage.OpenStorage(SecurityHelper.DataSpacesStorage))
            {
                ParseDataSpaceMap(dataSpaces);
                ParseTransform(dataSpaces);
            }
        }
        /// <summary>
        /// Checks whether password is correct.
        /// </summary>
        /// <param name="password">Password to check.</param>
        /// <returns>True if password verification succeeded.</returns>
        internal bool CheckPassword(string password)
        {
            KeyData keyData = m_info.XmlEncryptionDescriptor.KeyData;
            EncryptedKey key = m_info.XmlEncryptionDescriptor.KeyEncryptors.EncryptedKey;
            // Decrypt encrypted verifier Hash input.
            byte[] encryptedVerifierHashInputBlockKey = new byte[] { 0xfe, 0xa7, 0xd2, 0x76, 0x3b, 0x4b, 0x9e, 0x79 };
            byte[] verifierInputKey = m_securityHelper.CreateAgileEncryptionKey(m_hashAlgorithm, password, key.Salt, encryptedVerifierHashInputBlockKey, key.KeyBits >> 3, key.SpinCount);
            byte[] decryptedVerifierHashInputBytes = Decrypt(key.EncryptedVerifierHashInput, key.BlockSize, verifierInputKey, key.Salt, key.SaltSize);

            // Decrypt encrypted verifier Hash value.
            byte[] encryptedVerifierHashValueBlockKey = new byte[] { 0xd7, 0xaa, 0x0f, 0x6d, 0x30, 0x61, 0x34, 0x4e };
            byte[] verifierHashKey = m_securityHelper.CreateAgileEncryptionKey(m_hashAlgorithm, password, key.Salt, encryptedVerifierHashValueBlockKey, key.KeyBits >> 3, key.SpinCount);
            byte[] decryptedVerifierHashBytes = Decrypt(key.EncryptedVerifierHashValue, key.BlockSize, verifierHashKey, key.Salt, key.HashSize);

            byte[] verifierHashInputBytes = m_hashAlgorithm.ComputeHash(decryptedVerifierHashInputBytes);
            bool passwordVerificationMatch = m_securityHelper.CompareArray(decryptedVerifierHashBytes, verifierHashInputBytes);

            // Decrypt the encrypted key to obtain intermediate key.
            byte[] encryptedKeyValueBlockKey = new byte[] { 0x14, 0x6e, 0x0b, 0xe7, 0xab, 0xac, 0xd0, 0xd6 };
            byte[] arrKey = m_securityHelper.CreateAgileEncryptionKey(m_hashAlgorithm, password, key.Salt, encryptedKeyValueBlockKey, key.KeyBits >> 3, key.SpinCount);
            m_intermediateKey = Decrypt(key.EncryptedKeyValue, key.BlockSize, arrKey, key.Salt, keyData.KeyBits / 8);

            return passwordVerificationMatch;
        }
        /// <summary>
        /// Checks the encrypted package.
        /// </summary>
        /// <param name="encryptedPackage">The encrypted package.</param>
        /// <returns></returns>
        private bool CheckEncryptedPackage(byte[] encryptedPackage)
        {
            KeyData keyData = m_info.XmlEncryptionDescriptor.KeyData;
            DataIntegrity dataIntegrity = m_info.XmlEncryptionDescriptor.DataIntegrity;
            
            // Decrypt the Data Integrity Encrypted Hmac Key.
            byte[] encryptedDataIntegrityHmacKeyBlockKey = new byte[] { 0x5f, 0xb2, 0xad, 0x01, 0x0c, 0xb9, 0xe1, 0xf6 };
            byte[] dataIntegrityHmackeyIV = m_hashAlgorithm.ComputeHash(m_securityHelper.CombineArray(keyData.Salt, encryptedDataIntegrityHmacKeyBlockKey));
            dataIntegrityHmackeyIV = m_securityHelper.CorrectSize(dataIntegrityHmackeyIV, keyData.BlockSize, 0x00);
            byte[] decryptedDataIntegritySaltValue = Decrypt(dataIntegrity.EncryptedHmacKey, keyData.BlockSize, m_intermediateKey, dataIntegrityHmackeyIV, keyData.HashSize);

            // Generation of Hmac value.
            m_hmacSha.Key = m_securityHelper.CorrectSize(decryptedDataIntegritySaltValue, keyData.HashSize, 0x00);
            byte[] dataIntegrityHash = m_hmacSha.ComputeHash(encryptedPackage);

            // Decrypt the Data Integrity Encrypted Hmac value.
            byte[] encryptedDataIntegrityHmacValueBlockKey = new byte[] { 0xa0, 0x67, 0x7f, 0x02, 0xb2, 0x2c, 0x84, 0x33 };
            byte[] dataIntegrityHmacValueIV = m_hashAlgorithm.ComputeHash(m_securityHelper.CombineArray(keyData.Salt, encryptedDataIntegrityHmacValueBlockKey));
            dataIntegrityHmacValueIV = m_securityHelper.CorrectSize(dataIntegrityHmacValueIV, keyData.BlockSize, 0x00);
            byte[] decryptedDataIntegrityHmacValue = Decrypt(dataIntegrity.EncryptedHmacValue, keyData.BlockSize, m_intermediateKey, dataIntegrityHmacValueIV, keyData.HashSize);

            bool dataIntegrityMatch = m_securityHelper.CompareArray(dataIntegrityHash, decryptedDataIntegrityHmacValue);

            return dataIntegrityMatch;
        }
        /// <summary>
        /// Decrypts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="blockSize">Size of the block.</param>
        /// <param name="arrKey">The arr key.</param>
        /// <param name="IV">The IV.</param>
        /// <param name="actualLength">The actual length.</param>
        /// <returns></returns>
        private byte[] Decrypt(byte[] data, int blockSize, byte[] arrKey, byte[] IV, int actualLength)
        {
            int iLength = data.Length;
            byte[] result = new byte[iLength];
            byte[] buffer = new byte[blockSize];
            byte[] input;
            byte[] output = new byte[blockSize];
            byte[] previous = new byte[blockSize];
            Aes.KeySize keySize = Aes.KeySize.Bits128;
            if(arrKey.Length == 32)
                keySize = Aes.KeySize.Bits256;
            Aes aes = new Aes(keySize, arrKey);
            int iOffset = 0;

            if (iLength % blockSize != 0)
            {
                iLength = (iLength / blockSize + 1) * blockSize;
                input = m_securityHelper.CorrectSize(data, iLength, 0x00);
            }
            else
                input = data;

            while (iOffset < iLength)
            {
                if (iOffset == 0)
                    Buffer.BlockCopy(IV, 0, previous, 0, blockSize);
                else
                    Buffer.BlockCopy(buffer, 0, previous, 0, blockSize);
                Buffer.BlockCopy(input, iOffset, buffer, 0, blockSize);
                aes.InvCipher(buffer, output);
                output = m_securityHelper.ConcatenateIV(output, previous);
                Buffer.BlockCopy(output, 0, result, iOffset, blockSize);
                iOffset += blockSize;
            }

            if (result.Length > actualLength)
            {
                buffer = new byte[actualLength];
                Buffer.BlockCopy(result, 0, buffer, 0, actualLength);
                result = buffer;
            }
            return result;
        }
        /// <summary>
        /// Parses the transform.
        /// </summary>
        /// <param name="dataSpaces">The data spaces.</param>
        private void ParseTransform(ICompoundStorage dataSpaces)
        {
            List<DataSpaceMapEntry> lstEntries = m_dataSpaceMap.MapEntries;

            if (lstEntries.Count != 1)
                throw new Exception("Invalid data");

            DataSpaceMapEntry entry = lstEntries[0];
            string dataSpaceName = entry.DataSpaceName;
            string strTransformName = null;

            using (ICompoundStorage dataSpaceInfoStorage = dataSpaces.OpenStorage(SecurityHelper.DataSpaceInfoStorage))
            {
                using (Stream transformStream = dataSpaceInfoStorage.OpenStream(dataSpaceName))
                {
                    DataSpaceDefinition definition = new DataSpaceDefinition(transformStream);
                    List<string> lstTransforms = definition.TransformRefs;

                    if (lstTransforms.Count != 1)
                        throw new Exception("Invalid data");

                    strTransformName = lstTransforms[0];
                }
            }

            using (ICompoundStorage transformInfoStorage = dataSpaces.OpenStorage(SecurityHelper.TransformInfoStorage))
            {
                using (ICompoundStorage transformStorage = transformInfoStorage.OpenStorage(strTransformName))
                {
                    ParseTransformInfo(transformStorage);
                }
            }
        }
        /// <summary>
        /// Parses the data space map.
        /// </summary>
        /// <param name="dataSpaces">The data spaces.</param>
        private void ParseDataSpaceMap(ICompoundStorage dataSpaces)
        {
            if (dataSpaces == null)
                throw new ArgumentNullException("dataSpaces");

            using (CompoundStream stream = dataSpaces.OpenStream(SecurityHelper.DataSpaceMapStream))
            {
                m_dataSpaceMap = new DataSpaceMap(stream);
            }
        }
        /// <summary>
        /// Parses the transform info.
        /// </summary>
        /// <param name="transformStorage">The transform storage.</param>
        private void ParseTransformInfo(ICompoundStorage transformStorage)
        {
            using (Stream stream = transformStorage.OpenStream(SecurityHelper.TransformPrimaryStream))
            {
                TransformInfoHeader header = new TransformInfoHeader(stream);
                EncryptionTransformInfo encryptionTransformInfo = new EncryptionTransformInfo(stream);
            }
        }
        #endregion
    }
}

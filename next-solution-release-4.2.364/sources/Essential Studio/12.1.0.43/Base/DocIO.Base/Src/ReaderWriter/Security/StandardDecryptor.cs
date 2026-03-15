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
#if WINRT
using Syncfusion.DocIO.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif
using Syncfusion.DocIO;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Security
{
    /// <summary>
    /// This class is responsible for decryption of Standard encryption (Word 2007) files.
    /// </summary>
    [CLSCompliant(false)]
    internal class StandardDecryptor
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
        private StandardEncryptionInfo m_info;
        /// <summary>
        /// Compound storage that should be decrypted.
        /// </summary>
        private ICompoundStorage m_storage;
        /// <summary>
        /// Array containing key data.
        /// </summary>
        private byte[] m_arrKey;
        /// <summary>
        /// Security helper contains utility methods for encryption/decryption.
        /// </summary>
        private SecurityHelper m_securityHelper = new SecurityHelper();
        #endregion

        #region Methods
        /// <summary>
        /// Decrypts internal storage.
        /// </summary>
        /// <returns>Decrypted stream.</returns>
        internal Stream Decrypt()
        {
            if (m_arrKey == null)
                throw new InvalidOperationException("Incorrect password.");

            MemoryStream result = new MemoryStream();

            using (CompoundStream stream = m_storage.OpenStream(SecurityHelper.EncryptedPackageStream))
            {
                //First 8 bytes is length of the stream.
                byte[] arrInt64 = new byte[DLSConstants.LongSize];
                stream.Read(arrInt64, 0, DLSConstants.LongSize);
                int iLength = BitConverter.ToInt32(arrInt64, 0);

                int iMod = iLength % BlockSize;

                int iReadLength = (iMod > 0) ?
                  iLength + BlockSize - iMod :
                  iLength;

                byte[] arrBuffer = new byte[iReadLength];
                stream.Read(arrBuffer, 0, iReadLength);
                byte[] arrResult = Decrypt(arrBuffer, m_arrKey);
                result.Write(arrResult, 0, iLength);
                result.Position = 0;
            }
            return result;
        }
        /// <summary>
        /// Prepares decryptor for actual decryption.
        /// </summary>
        /// <param name="storage">Compound storage to get required data from.</param>
        internal void Initialize(ICompoundStorage storage)
        {
            if (storage == null)
                throw new ArgumentNullException("storage");

            m_storage = storage;

            using (Stream stream = storage.OpenStream(SecurityHelper.EncryptionInfoStream))
            {
                m_info = new StandardEncryptionInfo(stream);
            }

            using (ICompoundStorage dataSpaces = storage.OpenStorage(SecurityHelper.DataSpacesStorage))
            {
                ParseDataSpaceMap(dataSpaces);
                ParseTransfrom(dataSpaces);
            }
        }
        /// <summary>
        /// Checks whether password is correct.
        /// </summary>
        /// <param name="password">Password to check.</param>
        /// <returns>True if password verification succeeded.</returns>
        internal bool CheckPassword(string password)
        {
            EncryptionVerifier verifier = m_info.Verifier;
            //Creates key for decryption from password and salt bytes used for key generation.
            m_arrKey = m_securityHelper.CreateKey(password, verifier.Salt, 16);
            //Decrypts the encrypted verifier and gets the salt bytes used for password verification.
            byte[] arrVerifier = Decrypt(verifier.EncryptedVerifier, m_arrKey);
            SHA1 sha1 = new SHA1Managed();
            //Computes hash for decrypted verifier array.
            byte[] arrVerifierHash = sha1.ComputeHash(arrVerifier);

            //Decrypts the encrypted verifier hash and gets the hashed array of salt bytes used for password verification.
            byte[] buffer = Decrypt(verifier.EncryptedVerifierHash, m_arrKey);
            byte[] decryptedVerifierHash = new byte[verifier.VerifierHashSize];
            Buffer.BlockCopy(buffer, 0, decryptedVerifierHash, 0, decryptedVerifierHash.Length);

            //Compares the decrypted verifier hash with computed verifier hash.
            bool passwordVerificationMatch = m_securityHelper.CompareArray(decryptedVerifierHash, arrVerifierHash);
            return passwordVerificationMatch;
        }
        /// <summary>
        /// Decrypts specified buffer.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private byte[] Decrypt(byte[] data, byte[] key)
        {
            Aes aes = new Aes(Aes.KeySize.Bits128, key);
            return m_securityHelper.EncryptDecrypt(data, aes.InvCipher, key.Length);
        }
        /// <summary>
        /// Extracts transform data from the storage.
        /// </summary>
        /// <param name="dataSpaces">Storage to get data from.</param>
        private void ParseTransfrom(ICompoundStorage dataSpaces)
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
        /// Extracts dataspace map from the storage.
        /// </summary>
        /// <param name="dataSpaces">Storage to get data from.</param>
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
        /// Extracts TransformInfo from the storage.
        /// </summary>
        /// <param name="transformStorage">Storage to get data from.</param>
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
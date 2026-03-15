#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using Syncfusion.CompoundFile.XlsIO;
using System.IO;
#if ( WINRT )
using Syncfusion.XlsIO.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif

namespace Syncfusion.XlsIO.Implementation.Security
{
    /// <summary>
    /// This class used to encrypt data using Excel 2010 encryption with AES 128
    /// encryption algorithm with CBC Mode and SHA-1 hashing algorithm.
    /// </summary>
    class Excel2010Encryptor : Excel2007Encryptor
    {
        #region Constants
        /// <summary>
        /// Default Segment Size to encrypt.
        /// </summary>
        internal const int DefaultSegmentSize = 4096;
        /// <summary>
        /// Excel 2010 Version identifier.
        /// </summary>
        internal const int Excel2010Version = 0x40004;
        /// <summary>
        /// Version Flag.
        /// </summary>
        internal const int DefaultFlag = 0x00000040;
        #endregion

        #region Member
        /// <summary>
        /// Key to Encrypt the data.
        /// </summary>
        private byte[] m_arrKey;
        #endregion

        #region Methods
        /// <summary>
        /// Encrypts specified stream.
        /// </summary>
        /// <param name="data">Data to encrypt.</param>
        /// <param name="password">Password to use.</param>
        /// <param name="root">Root storage to put encrypted data into.</param>
        public override void Encrypt(Stream data, string password, ICompoundStorage root)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (password == null || password.Length == 0)
                throw new ArgumentOutOfRangeException("password");

            // 1. EncryptionInfo stream
            EncryptionInfo info = PrepareEncryptionInfo2010(root, password);
            // 2. DataSpaces substorage
            // 2a. DataSpace info substorage and StrongEncryptionDataSpace stream
            // 2b. TransformInfo substorage -> StrongEncryptionTransform substorage -> Primary stream
            PrepareDataSpaces(root);
            // 3. EncryptedPackage stream
            using (CompoundStream stream = root.CreateStream(SecurityHelper.EncryptedPackageStream))
            {
                long lLength = data.Length;
                byte[] arrLength = BitConverter.GetBytes(lLength);
                stream.Write(arrLength, 0, ExcelConstants.LongSize);

                Stream encrypted = Encrypt(data, m_arrKey, info.DataEncryption.SaltValue);
                byte[] arrEncrypted = new byte[encrypted.Length];
                encrypted.Read(arrEncrypted, 0, arrEncrypted.Length);
                stream.Write(arrEncrypted, 0, arrEncrypted.Length);
#if ( WINRT )
                encrypted.Dispose();
#else
                encrypted.Close();
#endif

                stream.Position = 0;
                PrepareDataIntegrity(stream, m_arrKey, info.DataEncryption.SaltValue, info.KeyInfo.HashSize, info);
            }
            using (CompoundStream stream = root.CreateStream(SecurityHelper.EncryptionInfoStream))
            {

                info.Serialize(stream);
            }
        }
        /// <summary>
        /// Encrypt the Stream with the given Key.
        /// </summary>
        /// <param name="stream">Stream data to encrypt.</param>
        /// <param name="arrIntermediateKey">Key to encrypt data.</param>
        /// <param name="arrKeyData_SaltValue">Salt value in byte array.</param>
        /// <returns></returns>
        internal Stream Encrypt(Stream stream, byte[] arrIntermediateKey, byte[] arrKeyData_SaltValue)
        {
            if (stream == null)
                throw new ArgumentNullException("Stream");

            byte[] arrData = new byte[stream.Length];
            stream.Read(arrData, 0, arrData.Length);
            int length = arrData.Length;
            int diff = length % KeyLength;
            int encryptionLength = length;
            if (diff > 0)
            {
                encryptionLength = length + (KeyLength - diff);
                byte[] tmpBuffer = SecurityHelper2010.TryPadOrTruncate(arrData, encryptionLength, 0);
                arrData = tmpBuffer;
            }
            int segmentCount = arrData.Length / DefaultSegmentSize;
            uint uSegmentNumber = 0;
            byte[] resEncrypted;
            if (segmentCount == 0)
            {

                byte[] arrIV = SecurityHelper2010.GenerateVector(arrKeyData_SaltValue, uSegmentNumber);

                byte[] encrypted = SecurityHelper2010.Encrypt(arrData, arrIntermediateKey, arrIV, 16);
                resEncrypted = encrypted;
            }
            else
            {
                resEncrypted = new byte[arrData.Length];
                byte[] tmpBuffer = new byte[DefaultSegmentSize];
                int iOffset = 0;
                for (; uSegmentNumber < segmentCount; uSegmentNumber++)
                {
                    Buffer.BlockCopy(arrData, iOffset, tmpBuffer, 0, tmpBuffer.Length);
                    byte[] arrIV = SecurityHelper2010.GenerateVector(arrKeyData_SaltValue, uSegmentNumber);

                    byte[] encrypted = SecurityHelper2010.Encrypt(tmpBuffer, arrIntermediateKey, arrIV, KeyLength);
                    Buffer.BlockCopy(encrypted, 0, resEncrypted, iOffset, encrypted.Length);
                    iOffset += DefaultSegmentSize;
                }
                diff = arrData.Length % DefaultSegmentSize;
                if (diff > 0)
                {
                    tmpBuffer = new byte[diff];
                    Buffer.BlockCopy(arrData, iOffset, tmpBuffer, 0, tmpBuffer.Length);
                    byte[] arrIV = SecurityHelper2010.GenerateVector(arrKeyData_SaltValue, uSegmentNumber);

                    byte[] encrypted = SecurityHelper2010.Encrypt(tmpBuffer, arrIntermediateKey, arrIV, KeyLength);
                    Buffer.BlockCopy(encrypted, 0, resEncrypted, iOffset, encrypted.Length);
                }
            }
            Stream memoryStream = new MemoryStream(resEncrypted);
            return memoryStream;

        }
        /// <summary>
        /// Preparse data spaces structures inside specified storage.
        /// </summary>
        /// <param name="root">Storage to put DataSpaces inside.</param>
        protected void PrepareDataSpaces(ICompoundStorage root)
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
                //// DataSpaceMap - stream
                SerializeDataSpaceMap(dataSpaces);
            }
        }
        /// <summary>
        /// Creates the Key with the given password.
        /// </summary>
        /// <param name="arrPassword">Password in byte array.</param>
        /// <param name="arrSalt">Salt Value in byte array.</param>
        /// <param name="spinCount">Iteration count to generate the key.</param>
        /// <returns>Intermediate Key to encrypt the data.</returns>
        internal byte[] CreateIntermediateKey(byte[] arrPassword, byte[] arrSalt, int spinCount)
        {
            byte[] arrIntermediateKey = CreateSalt(KeyLength);
            byte[] arrBlock_Key = { 0x14, 0x6e, 0x0b, 0xe7, 0xab, 0xac, 0xd0, 0xd6 };
            byte[] arrKey = CreateKey(arrPassword, arrSalt, arrBlock_Key, spinCount, arrSalt.Length);
            m_arrKey = arrIntermediateKey;
            return SecurityHelper2010.Encrypt(arrIntermediateKey, arrKey, arrSalt, arrKey.Length);
            
        }
        /// <summary>
        /// Creates the Data Integrity Verifier Hash Input. 
        /// </summary>
        /// <param name="random">Random values in byte array.</param>
        /// <param name="arrPassword">Password in byte array.</param>
        /// <param name="arrSalt">SaltValue in byte array.</param>
        /// <param name="spinCount">Iteration count to generate the key.</param>
        /// <returns>DataIntegrity verifier hash input.</returns>
        internal byte[] CreateVerifierHashInput(byte[] random,byte[] arrPassword, byte[] arrSalt, int spinCount)
        {
            byte[] arrVerifierHashInput = random;
           
            byte[] arrBlock_Key = { 0xfe , 0xa7, 0xd2, 0x76, 0x3b, 0x4b, 0x9e, 0x79 };
            byte[] arrKey = CreateKey(arrPassword, arrSalt, arrBlock_Key, spinCount, 16);
            byte[] encryptedHashInput = SecurityHelper2010.Encrypt(arrVerifierHashInput, arrKey, arrSalt, arrKey.Length);
            return encryptedHashInput;
        }
        /// <summary>
        /// Creates the Data Integrity Verifier Hash Value. 
        /// </summary>
        /// <param name="random">Random values in byte array.</param>
        /// <param name="arrPassword">Password in byte array.</param>
        /// <param name="arrSalt">SaltValue in byte array.</param>
        /// <param name="spinCount">Iteration count to generate the key.</param>
        /// <returns>DataIntegrity verifier hash Value.</returns>
        internal byte[] CreateVerifierHashValue(byte[] random,byte[] arrPassword, byte[] arrSalt, int spinCount)
        {
            byte[] arrVerifierValue = random;
            byte[] arrVerifierHashValue= SecurityHelper2010.Hash(arrVerifierValue);
            arrVerifierHashValue = SecurityHelper2010.TryPadOrTruncate(arrVerifierHashValue, 2 * KeyLength, 0x00);
            byte[] arrBlock_Key = { 0xd7, 0xaa, 0x0f, 0x6d, 0x30, 0x61, 0x34, 0x4e };
            byte[] arrKey = CreateKey(arrPassword, arrSalt, arrBlock_Key, spinCount, arrSalt.Length);
            byte[] encryptedHashValue = SecurityHelper2010.Encrypt(arrVerifierHashValue, arrKey, arrSalt, arrKey.Length);
            return encryptedHashValue;
        }
        /// <summary>
        /// Creates the Key to Encrypt the data.
        /// </summary>
        /// <param name="arrPassword">Password in byte array.</param>
        /// <param name="arrSalt">Salt Value in byte array.</param>
        /// <param name="arrBlock_Key">Block Key to Generate vector.</param>
        /// <param name="spinCount">The number of times to iterate the password hash when creating the key.</param>
        /// <param name="keySize"></param>
        /// <returns></returns>
        internal byte[] CreateKey(byte[] arrPassword, byte[] arrSalt, byte[] arrBlock_Key, int spinCount, int keySize)
        {
            //byte[] arrBlock_Key = { 0x14, 0x6e, 0x0b, 0xe7, 0xab, 0xac, 0xd0, 0xd6 };
#if !SILVERLIGHT && !WINRT && !WP
      SHA1CryptoServiceProvider
#else
            SHA1Managed
#endif
 sha1 = new
#if !SILVERLIGHT && !WINRT && !WP
          SHA1CryptoServiceProvider();
#else
 SHA1Managed();
#endif
            byte[] tmp = SecurityHelper.CombineArray(arrSalt, arrPassword);

            byte[] H0 = sha1.ComputeHash(tmp);
            byte[] Hi = H0;
            for (uint iterator = 0; iterator < spinCount; iterator++)
            {
                byte[] arrIterator = BitConverter.GetBytes(iterator);
                tmp = SecurityHelper.CombineArray(arrIterator, Hi);
                Hi = sha1.ComputeHash(tmp);
            }


            tmp = SecurityHelper.CombineArray(Hi, arrBlock_Key);
            byte[] Hfinal = sha1.ComputeHash(tmp);


            byte[] arrKey = SecurityHelper2010.TryPadOrTruncate(Hfinal, keySize, 0x36);

            return arrKey;

        }
        /// <summary>
        /// Fills in EncryptionInfo record and stores it at appropriate stream.
        /// </summary>
        /// <param name="root">Root storage.</param>
        /// <param name="password">Encryption password.</param>
        /// <returns>Encryption key.</returns>
        protected EncryptionInfo PrepareEncryptionInfo2010(ICompoundStorage root, string password)
        {
            if (root == null)
                throw new ArgumentNullException("root");

            EncryptionInfo info = new EncryptionInfo();
            info.VersionInfo = Excel2010Version;
            info.Flags = DefaultFlag;
            int spinCount = info.KeyInfo.SpinCount;

            byte[] arrPassword = Encoding.Unicode.GetBytes(password);
            byte[] arrSalt = CreateSalt(KeyLength);
            info.KeyInfo.KeyValue = CreateIntermediateKey(arrPassword, arrSalt, spinCount);
            byte[] random = CreateSalt(KeyLength);
            info.KeyInfo.VerifierHashInput = CreateVerifierHashInput(random, arrPassword, arrSalt, spinCount);
            info.KeyInfo.VerifierHashValue = CreateVerifierHashValue(random, arrPassword, arrSalt, spinCount);
            info.KeyInfo.SaltValue = arrSalt;
            arrSalt=info.DataEncryption.SaltValue = CreateSalt(KeyLength);
           
          

            return info;
        }
        /// <summary>
        ///  Fills in Data Integrity info record and stores it at appropriate stream.
        /// </summary>
        /// <param name="encryptedPackage">Encypted Package.</param>
        /// <param name="arrKey">Key to Encrypt the Hmac Input and Value.</param>
        /// <param name="arrSalt">Salt value in byte array.</param>
        /// <param name="hashSize">Represents the SHA1 hash size.</param>
        /// <param name="info">Represents the EncyptionInfo.</param>
        internal void PrepareDataIntegrity(Stream encryptedPackage,byte[] arrKey, byte[] arrSalt, int hashSize,EncryptionInfo info)
        {
            if (encryptedPackage== null)
                throw new ArgumentNullException("encrypted Package");

            byte[] arrHmacKey = CreateSalt(hashSize);
            byte[] blockKey = { 0x5f, 0xb2, 0xad, 0x01, 0x0c, 0xb9, 0xe1, 0xf6 };
            byte[] arrIV = SecurityHelper2010.GenerateVector(arrSalt, blockKey);
            arrHmacKey = SecurityHelper2010.TryPadOrTruncate(arrHmacKey, 2 * arrKey.Length, 0);
            byte[] arrEncryptedHmacKey = SecurityHelper2010.Encrypt(arrHmacKey, arrKey, arrIV, arrKey.Length);
            
             info.DataIntegrity.HMacKey= arrEncryptedHmacKey;
       
            HMACSHA1 hmac1 = new HMACSHA1();
            hmac1.Key = arrHmacKey;
            byte[] package = new byte[encryptedPackage.Length];
            encryptedPackage.Read(package, 0, package.Length);
            byte[] currentHmacValue = hmac1.ComputeHash(package);

            currentHmacValue = SecurityHelper2010.TryPadOrTruncate(currentHmacValue, 2 * arrKey.Length, 0);
             blockKey = new byte[]{ 0xa0, 0x67, 0x7f, 0x02, 0xb2, 0x2c, 0x84, 0x33 };
            arrIV = SecurityHelper2010.GenerateVector(arrSalt, blockKey);
           info.DataIntegrity.HMacValue = SecurityHelper2010.Encrypt(currentHmacValue, arrKey, arrIV, arrKey.Length);

        }
        #endregion
    }
}

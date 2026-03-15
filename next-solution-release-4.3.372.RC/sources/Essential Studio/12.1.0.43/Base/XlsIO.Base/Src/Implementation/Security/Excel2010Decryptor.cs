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
#if ( WINRT )
using Syncfusion.XlsIO.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif
using System.IO;

namespace Syncfusion.XlsIO.Implementation.Security
{
    /// <summary>
    /// This class used to decrypt data using Excel 2010 encryption with AES 128
    /// encryption algorithm with CBC Mode and SHA-1 hashing algorithm.
    /// </summary>
    class Excel2010Decryptor : Excel2007Decryptor
    {

        #region Methods
        /// <summary>
        /// Decrypts internal storage storage.
        /// </summary>
        /// <returns>Decrypted stream.</returns>
        public override Stream Decrypt()
        {
            if (m_arrKey == null)
                throw new InvalidOperationException("Incorrect password.");

            MemoryStream result = new MemoryStream();
            int blockSize = m_info.KeyInfo.BlockSize;
            using (CompoundStream stream = Storage.OpenStream(SecurityHelper.EncryptedPackageStream))
            {
              
                bool bIntegrity = CheckDataIntegrity(stream, m_arrKey, m_info.DataEncryption.SaltValue, m_info.DataIntegrity.HMacKey, m_info.DataIntegrity.HMacValue);
                if (!bIntegrity)
                    throw new InvalidOperationException("Not a valid encrypted file.");

                stream.Position = 0;
                //First 8 bytes is length of the stream.
                byte[] arrInt64 = new byte[ExcelConstants.LongSize];
                stream.Read(arrInt64, 0, ExcelConstants.LongSize);
                int actualLength = BitConverter.ToInt32(arrInt64, 0);

                int segmentSize = Excel2010Encryptor.DefaultSegmentSize;


                //Decrypt after the 8 bytes.
                byte[] arrCipherData = new byte[stream.Length - 8];
                stream.Read(arrCipherData, 0, arrCipherData.Length);
                int cipherLength = arrCipherData.Length;
                byte[] decryptedData;
                int segmentCount = cipherLength / segmentSize;
                int diff = cipherLength % segmentSize;
                byte[] tmpBuffer;
                if (diff > 0)
                {
                    segmentCount++;
                    int validatedSize = cipherLength + segmentSize - diff;
                    tmpBuffer = new byte[validatedSize];
                    Buffer.BlockCopy(arrCipherData, 0, tmpBuffer, 0, arrCipherData.Length);
                    SecurityHelper2010.TryPadOrTruncate(tmpBuffer, tmpBuffer.Length, 0);
                    arrCipherData = tmpBuffer;
                }
                decryptedData = new byte[arrCipherData.Length];
                tmpBuffer = new byte[segmentSize];

                int iOffset = 0;
                for (uint segmentNumber = 0; segmentNumber < segmentCount; segmentNumber++)
                {

                    Buffer.BlockCopy(arrCipherData, iOffset, tmpBuffer, 0, tmpBuffer.Length);
                    byte[] arrIV = SecurityHelper2010.GenerateVector(m_info.DataEncryption.SaltValue, segmentNumber);

                    byte[] decrypted = SecurityHelper2010.Decrypt(tmpBuffer, m_arrKey, arrIV, 16, segmentSize);
                    Buffer.BlockCopy(decrypted, 0, decryptedData, iOffset, decrypted.Length);
                    iOffset += segmentSize;
                }

                byte[] tmp = SecurityHelper2010.TryPadOrTruncate(decryptedData, actualLength, 0);
                result = new MemoryStream(tmp);
            }

            return result;
        }
        /// <summary>
        /// Checks whether password is correct.
        /// </summary>
        /// <param name="password">Password to check.</param>
        /// <returns>True if password verification succeeded.</returns>
        public override bool CheckPassword(string password)
        {
            EncryptedKeyInfo keyInfo = m_info.KeyInfo;
            m_arrKey = VerifyPassword(password, keyInfo);

            return (m_arrKey != null);
        }
        /// <summary>
        /// Verifies password.
        /// </summary>
        /// <param name="password">Password to check.</param>
        /// <param name="verifier">Verifier object.</param>
        /// <returns>Encryption key.</returns>
        private byte[] VerifyPassword(string password, EncryptedKeyInfo verifier)
        {
            if (verifier == null)
                throw new ArgumentNullException("Key Verifier");

            byte[] arrKey = null;
            byte[] arrSalt = verifier.SaltValue;
            byte[] arrPassword = Encoding.Unicode.GetBytes(password);
            byte[] arrVerifierHashInput = GetVerifierHashInput(verifier.VerifierHashInput, arrPassword, arrSalt, verifier.SpinCount, verifier.KeyBits);
            byte[] arrVerifierHashValue = GetVerifierHashValue(verifier.VerifierHashValue, arrPassword, arrSalt, verifier.SpinCount, verifier.KeyBits);
            byte[] resultHashInput = SecurityHelper2010.Hash(arrVerifierHashInput);

            if (Syncfusion.XlsIO.Parser.Biff_Records.BiffRecordRaw.CompareArrays(
       resultHashInput, 0,
       arrVerifierHashValue, 0,
       resultHashInput.Length))
            {
                arrKey = GetIntermediateKey(verifier.KeyValue, arrPassword, arrSalt, verifier.SpinCount, verifier.KeyBits);
            }
            return arrKey;
        }
        /// <summary>
        /// Get the Encryption/Decryption Key from the encrypted data.
        /// </summary>
        /// <param name="arrEncryptedKeyValue">Encrypted key in byte array.</param>
        /// <param name="arrPassword">Password in byte array.</param>
        /// <param name="arrSalt">Salt Value in byte array.</param>
        /// <param name="spinCount">The number of times to iterate the password hash when creating the key.</param>
        /// <param name="keySize">Represent the size of the key.</param>
        /// <returns>Key to encrypt and decrypt the Encrypted Package.</returns>
        internal byte[] GetIntermediateKey(byte[] arrEncryptedKeyValue, byte[] arrPassword, byte[] arrSalt, int spinCount, int keySize)
        {
            byte[] arrBlock_Key = { 0x14, 0x6e, 0x0b, 0xe7, 0xab, 0xac, 0xd0, 0xd6 };
            byte[] arrKey = SecurityHelper2010.CreateKey(arrPassword, arrSalt, arrBlock_Key, spinCount, keySize);
            byte[] arrIntermediateKey = SecurityHelper2010.Decrypt(arrEncryptedKeyValue, arrKey, arrSalt, keySize, keySize);
            return arrIntermediateKey;
        }
        /// <summary>
        /// Get the password verifier hash.
        /// </summary>
        /// <param name="arrVerifierHashInput">Encrypted the verifier hash.</param>
        /// <param name="arrPassword">Password in byte array.</param>
        /// <param name="arrSalt">Salt Value in byte array.</param>
        /// <param name="spinCount">The number of times to iterate the password hash when creating the key</param>
        /// <param name="keySize">Represent the size of the key.</param>
        /// <returns>Decrypted Verifier hash.</returns>
        private byte[] GetVerifierHashInput(byte[] arrVerifierHashInput, byte[] arrPassword, byte[] arrSalt, int spinCount, int keySize)
        {
            byte[] arrBlock_Key = { 0xfe, 0xa7, 0xd2, 0x76, 0x3b, 0x4b, 0x9e, 0x79 };
            byte[] arrKey = SecurityHelper2010.CreateKey(arrPassword, arrSalt, arrBlock_Key, spinCount, arrSalt.Length);
            byte[] decryptedHashInput = SecurityHelper2010.Decrypt(arrVerifierHashInput, arrKey, arrSalt, keySize, keySize);
            return decryptedHashInput;
        }
        /// <summary>
        /// Get the password verifier hash value.
        /// </summary>
        /// <param name="arrVerifierHashInput">Encrypted the verifier hash.</param>
        /// <param name="arrPassword">Password in byte array.</param>
        /// <param name="arrSalt">Salt Value in byte array.</param>
        /// <param name="spinCount">The number of times to iterate the password hash when creating the key</param>
        /// <param name="keySize">Represent the size of the key.</param>
        /// <returns>Decrypted Verifier hash value.</returns>
        internal byte[] GetVerifierHashValue(byte[] arrVerifierHashValue, byte[] arrPassword, byte[] arrSalt, int spinCount, int keySize)
        {
            byte[] arrBlock_Key = { 0xd7, 0xaa, 0x0f, 0x6d, 0x30, 0x61, 0x34, 0x4e };
            byte[] arrKey = SecurityHelper2010.CreateKey(arrPassword, arrSalt, arrBlock_Key, spinCount, keySize);
            byte[] decryptedHashValue = SecurityHelper2010.Decrypt(arrVerifierHashValue, arrKey, arrSalt, keySize, keySize);
            return decryptedHashValue;

        }
        /// <summary>
        /// Get the Data Integrity verifier HmacKey.
        /// </summary>
        /// <param name="arrEncryptedHmacKey">Encrypted Hmac Key.</param>
        /// <param name="arrKey">Key to Decrypt the Hmac key.</param>
        /// <param name="arrSalt">Salt Value in byte array.</param>
        /// <param name="hashSize">Represents the hash size to compute.</param>
        /// <returns>Hmac Key.</returns>
        internal byte[] GetHMacKey(byte[] arrEncryptedHmacKey, byte[] arrKey, byte[] arrSalt, int hashSize)
        {
            byte[] blockKey = { 0x5f, 0xb2, 0xad, 0x01, 0x0c, 0xb9, 0xe1, 0xf6 };
            byte[] arrIV = SecurityHelper2010.GenerateVector(arrSalt, blockKey);
            byte[] arrHmacKey = SecurityHelper2010.Decrypt(arrEncryptedHmacKey, arrKey, arrIV, arrKey.Length, arrKey.Length);
            arrHmacKey = SecurityHelper2010.TryPadOrTruncate(arrHmacKey, hashSize, 0);
            return arrHmacKey;
        }
        /// <summary>
        /// Get the Data Integiryt verifier HMac Value.
        /// </summary>
        /// <param name="arrEncryptedHmacValue">Encrypted Hmac Value.</param>
        /// <param name="arrKey">Key to Decrypt the Hmac key.</param>
        /// <param name="arrSalt">Salt Value in byte array.</param>
        /// <returns>Hmac Value.</returns>
        internal byte[] GetHmacValue(byte[] arrEncryptedHmacValue, byte[] arrKey, byte[] arrSalt)
        {
            byte[] blockKey = { 0xa0, 0x67, 0x7f, 0x02, 0xb2, 0x2c, 0x84, 0x33 };
            byte[] arrIV = SecurityHelper2010.GenerateVector(arrSalt, blockKey);
            byte[] arrHmacValue = SecurityHelper2010.Decrypt(arrEncryptedHmacValue, arrKey, arrIV, arrKey.Length, arrKey.Length);

            return arrHmacValue;
        }
        /// <summary>
        /// Ensures the Data Integrity of the Encrypted Package.
        /// </summary>
        /// <param name="EncryptedPackage">Encrypted data.</param>
        /// <param name="arrKey">Key to Decrypt the Hmac key.</param>
        /// <param name="arrSalt">Salt Value in byte array.</param>
        /// <param name="arrEncryptedHmacKey">Hmac Key to Compute Hmac hash.</param>
        /// <param name="arrEncryptedHmacValue">Hmac Value to Compare.</param>
        /// <returns>True, if the data is in original structure.</returns>
        internal bool CheckDataIntegrity(Stream EncryptedPackage, byte[] arrKey, byte[] arrSalt, byte[] arrEncryptedHmacKey, byte[] arrEncryptedHmacValue)
        {
            byte[] arrHmacKey = GetHMacKey(arrEncryptedHmacKey, arrKey, arrSalt, m_info.KeyInfo.HashSize);
            HMACSHA1 hmac1 = new HMACSHA1();
            hmac1.Key = arrHmacKey;
            byte[] package = new byte[EncryptedPackage.Length];
            EncryptedPackage.Read(package, 0, package.Length);
            byte[] currentHmacValue = hmac1.ComputeHash(package);

            byte[] arrHmacValue = GetHmacValue(arrEncryptedHmacValue, arrKey, arrSalt);
            arrHmacValue = SecurityHelper2010.TryPadOrTruncate(arrHmacValue, currentHmacValue.Length, 0);
            return Syncfusion.XlsIO.Parser.Biff_Records.BiffRecordRaw.CompareArrays(arrHmacValue, currentHmacValue);
        }
        #endregion

    }
}

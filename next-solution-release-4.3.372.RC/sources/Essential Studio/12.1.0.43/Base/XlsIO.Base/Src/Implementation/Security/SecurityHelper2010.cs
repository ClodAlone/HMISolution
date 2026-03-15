#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if ( WINRT )
using Syncfusion.XlsIO.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif
namespace Syncfusion.XlsIO.Implementation.Security
{
    /// <summary>
    /// This class contains utility methods used by Excel 2010 security implementation.
    /// </summary>
    sealed class SecurityHelper2010
    {
        #region Encrypt and Decrypt Method
        /// <summary>
        /// Encrypt the Data in AES-128 algorithm with CBC mode.
        /// </summary>
        /// <param name="arrPlainData">Plain data to encrypt.</param>
        /// <param name="arrKey">Key to Encrypt the data.</param>
        /// <param name="arrIV">Vector to encrypt the data.</param>
        /// <param name="blockSize">The number of bytes used to encrypt one block of data</param>
        /// <returns></returns>
        internal static byte[] Encrypt(byte[] arrPlainData, byte[] arrKey, byte[] arrIV, int blockSize)
        {
            int iOffset = 0;

            Aes aes = new Aes(Aes.KeySize.Bits128, arrKey);
            byte[] arrResult = new byte[arrPlainData.Length];
            byte[] arrPlain0 = new byte[blockSize];
            Buffer.BlockCopy(arrPlainData, 0, arrPlain0, 0, arrPlain0.Length);
            byte[] arrXOR = SecurityHelper2010.XOR(arrPlain0, arrIV);
            byte[] cipherI = new byte[blockSize];
            aes.Cipher(arrXOR, cipherI);
            Buffer.BlockCopy(cipherI, 0, arrResult, 0, cipherI.Length);
            iOffset += blockSize;
            while (iOffset < arrPlainData.Length)
            {
                byte[] arrPlainI = new byte[blockSize];
                Buffer.BlockCopy(arrPlainData, iOffset, arrPlainI, 0, arrPlain0.Length);
                arrXOR = SecurityHelper2010.XOR(arrPlainI, cipherI);
                aes.Cipher(arrXOR, cipherI);

                Buffer.BlockCopy(cipherI, 0, arrResult, iOffset, cipherI.Length);
                iOffset += blockSize;
            }
            return arrResult;
        }
        /// <summary>
        /// Decrypt the data in AES-128 algorithm with CBC mode.
        /// </summary>
        /// <param name="arrCipherData"></param>
        /// <param name="arrKey"></param>
        /// <param name="arrIV"></param>
        /// <param name="keySize"></param>
        /// <param name="actualLength"></param>
        /// <returns></returns>
        internal static byte[] Decrypt(byte[] arrCipherData, byte[] arrKey, byte[] arrIV, int keySize, int actualLength)
        {
            int iOffset = 0;
            byte[] arrResult = new byte[arrCipherData.Length];
            Aes aes = new Aes(Aes.KeySize.Bits128, arrKey);
            byte[] arrCipher0 = new byte[keySize];
            Buffer.BlockCopy(arrCipherData, 0, arrCipher0, 0, arrCipher0.Length);

            byte[] output = new byte[keySize];
            aes.InvCipher(arrCipher0, output);
            byte[] plain = XOR(output, arrIV);

            Buffer.BlockCopy(plain, 0, arrResult, 0, plain.Length);
            iOffset += plain.Length;

            int iLength = arrCipherData.Length - iOffset;
            while (iOffset <= iLength)
            {
                byte[] arrCiphern = new byte[keySize];
                Buffer.BlockCopy(arrCipherData, iOffset, arrCiphern, 0, arrCiphern.Length);

                output = new byte[keySize];
                aes.InvCipher(arrCiphern, output);
                plain = XOR(output, arrCipher0);

                Buffer.BlockCopy(plain, 0, arrResult, iOffset, plain.Length);
                iOffset += plain.Length;
                arrCipher0 = arrCiphern;
            }

            TryPadOrTruncate(arrResult, actualLength, 0);
            return arrResult;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Creates the Key to Decrypt and Encrypt the Data.
        /// </summary>
        /// <param name="arrPassword">password in byte array.</param>
        /// <param name="arrSalt">Salt Value in bytes.</param>
        /// <param name="arrBlock_Key">Block Key in bytes.</param>
        /// <param name="spinCount">Loop count to compute Hash.</param>
        /// <param name="keySize">Creates the key with the size.</param>
        /// <returns></returns>
        internal static byte[] CreateKey(byte[] arrPassword, byte[] arrSalt, byte[] arrBlock_Key, int spinCount, int keySize)
        {
#if !SILVERLIGHT && !WINRT && !WP
            SHA1CryptoServiceProvider
#else
      SHA1Managed
#endif
 sha1 =
#if !SILVERLIGHT && !WINRT && !WP
 new SHA1CryptoServiceProvider();

#else    
     new     SHA1Managed();
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


            byte[] arrKey = TryPadOrTruncate(Hfinal, keySize, 0x36);

            return arrKey;

        }
        
        /// <summary>
        /// Generate Vecotor with the block key with integer.
        /// </summary>
        /// <param name="arrSalt">Salt Value in byte array.</param>
        /// <param name="segmentNumber">unsinged integer as block Key.</param>
        /// <returns>Vector in byte array.</returns>
        internal static byte[] GenerateVector(byte[] arrSalt, uint segmentNumber)
        {
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
            byte[] blockKey = BitConverter.GetBytes(segmentNumber);
            byte[] tmp = SecurityHelper.CombineArray(arrSalt, blockKey);
            byte[] Hiv = sha1.ComputeHash(tmp);


            byte[] arrIV = new byte[16];

            Buffer.BlockCopy(Hiv, 0, arrIV, 0, arrIV.Length);


            return arrIV;
        }
        /// <summary>
        /// Generates the Vector.
        /// </summary>
        /// <param name="arrSalt">Salt value in byte array.</param>
        /// <param name="blockKey">Block key value in byte array.</param>
        /// <returns>Vector in byte array.</returns>
        internal static byte[] GenerateVector(byte[] arrSalt, byte[] blockKey)
        {
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
            byte[] tmp = SecurityHelper.CombineArray(arrSalt, blockKey);
            byte[] Hiv = sha1.ComputeHash(tmp);


            byte[] arrIV = new byte[16];

            Buffer.BlockCopy(Hiv, 0, arrIV, 0, arrIV.Length);


            return arrIV;
        }
        /// <summary>
        /// Compute SHA1 Hash.
        /// </summary>
        /// <param name="input">Input to compute hash.</param>
        /// <returns></returns>
        internal static byte[] Hash(byte[] input)
        {
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
            return sha1.ComputeHash(input);

        }
        /// <summary>
        /// XOR the two byte array.
        /// </summary>
        /// <param name="fByte">First byte.</param>
        /// <param name="sByte">Second byte.</param>
        /// <returns></returns>
        internal static byte[] XOR(byte[] fByte, byte[] sByte)
        {
            byte[] result = new byte[fByte.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (byte)(fByte[i] ^ sByte[i]);
            }
            return result;
        }
        /// <summary>
        /// Tries to pad or Truncate the given byte array.
        /// </summary>
        /// <param name="arrData">Byte array to pad or Truncate based on the length.</param>
        /// <param name="length">Represents the limit of the array.</param>
        /// <param name="padValue">Value to pad upto the length.</param>
        /// <returns></returns>
        internal static byte[] TryPadOrTruncate(byte[] arrData, int length, byte padValue)
        {
            byte[] result = new byte[length];
            int diff = length - arrData.Length;
            if (diff == 0)
                return arrData;
            if (diff < 0)
            {
                Buffer.BlockCopy(arrData, 0, result, 0, result.Length);
            }
            else
            {
                byte[] arrPadValue = { padValue };
                Buffer.BlockCopy(arrData, 0, result, 0, arrData.Length);

                for (int iOffset = arrData.Length; iOffset < length; iOffset += arrPadValue.Length)
                {
                    Buffer.BlockCopy(arrPadValue, 0, result, iOffset, arrPadValue.Length);
                }

            }
            return result;
        }
       
        #endregion
    }
}

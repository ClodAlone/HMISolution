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
using System.IO;
using System.Text;
#if WINRT
using Syncfusion.DocIO.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Security
{
    /// <summary>
    /// This class contains utility methods used by Word 2007 and Word 2010 document protection implementation.
    /// </summary>
    [CLSCompliant(false)]
    internal class DocxProtection
    {
        #region Constants
        /// <summary>
        /// Specifies number of iterations used for key generation.
        /// </summary>
        internal const int SpinCount = 100000;
        /// <summary>
        /// Specifies the cryptography type.
        /// </summary>
        internal const string CryptographicType = "rsaFull";
        /// <summary>
        /// Specifies the cryptographic algorithm class.
        /// </summary>
        internal const string CryptographicAlgorithmClass = "hash";
        /// <summary>
        /// Specifies the cryptographic algorithm type.
        /// </summary>
        internal const string CryptographicAlgorithmType = "typeAny";
        /// <summary>
        /// Specifies the cryptographic algorithm id (SHA1).
        /// </summary>
        internal const int CryptographicAlgorithmId = 4;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DocxProtection"/> class.
        /// </summary>
        internal DocxProtection()
        {
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Computes the hash.
        /// </summary>
        /// <param name="salt">The salt.</param>
        /// <param name="encryptedPassword">The encrypted password.</param>
        /// <returns></returns>
        internal byte[] ComputeHash(byte[] salt, uint encryptedPassword)
        {
            // Reverse the byte order.
            byte[] key = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                key[i] = Convert.ToByte(((uint)(encryptedPassword & (0x000000FF << (i * 8)))) >> (i * 8));
            }

            // Converts the byte to hexadecimal string.
            string reversedText = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                reversedText += key[i].ToString("X2");
            }
            
            // Removes BOM leading characters.
            reversedText = reversedText.Trim().Trim(new char[] { '\uFEFF' });
            reversedText = reversedText.ToUpper();

            // Converts the encrypted password to bytes.
            byte[] passwordKey = Encoding.Unicode.GetBytes(reversedText);

            // Combines the random salt value with the key.
            byte[] buffer = CombineByteArrays(salt, passwordKey);
            
            SHA1 sha1 = new SHA1Managed();
            // Compute hash function.
            buffer = sha1.ComputeHash(buffer);
            byte[] buffer2 = new byte[0x18];

            // Compute hash function for specified iteration count.
            for (int i = 0; i < SpinCount; i++)
            {
                buffer.CopyTo(buffer2, 0);
                int temp = i;
                for (int j = 0; j < 4; j++)
                {
                    buffer2[buffer.Length + j] = (byte)temp;
                    temp = (int)(temp >> 8);
                }
                buffer = sha1.ComputeHash(buffer2);
            }
            return buffer;
        }
        /// <summary>
        /// Combines the byte arrays.
        /// </summary>
        /// <param name="array1">The array1.</param>
        /// <param name="array2">The array2.</param>
        /// <returns></returns>
        private byte[] CombineByteArrays(byte[] array1, byte[] array2)
        {
            byte[] result = new byte[array1.Length + array2.Length];
            Buffer.BlockCopy(array1, 0, result, 0, array1.Length);
            Buffer.BlockCopy(array2, 0, result, array1.Length, array2.Length);
            return result;
        }
        /// <summary>
        /// Creates the salt.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        internal byte[] CreateSalt(int length)
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
        #endregion
    }
}

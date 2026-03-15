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
    /// This class contains utility methods used by Word 2007 and Word 2010 Encryption/Decryption implementation.
    /// </summary>
    [CLSCompliant(false)]
    internal sealed class SecurityHelper
    {
        #region Class enums
        /// <summary>
        /// Represents type of the encryption.
        /// </summary>
        internal enum EncrytionType
        {
            /// <summary>
            /// Word 2007 encryption format.
            /// </summary>
            Standard,
            /// <summary>
            /// Word 2010 encryption format.
            /// </summary>
            Agile,
            /// <summary>
            /// Wrong encryption format.
            /// </summary>
            None
        }
        #endregion

        #region Constants
        /// <summary>
        /// Number of iterations used for key generation.
        /// </summary>
        private const int PasswordIterationCount = 50000;
        /// <summary>
        /// Name of encryption info stream.
        /// </summary>
        internal const string EncryptionInfoStream = "EncryptionInfo";
        /// <summary>
        /// Name of dataspaces storage.
        /// </summary>
        internal const string DataSpacesStorage = "\x0006DataSpaces";
        /// <summary>
        /// Name of dataspace map stream.
        /// </summary>
        internal const string DataSpaceMapStream = "DataSpaceMap";
        /// <summary>
        /// Name of the transform primary stream.
        /// </summary>
        internal const string TransformPrimaryStream = "\x06Primary";
        /// <summary>
        /// Name of dataspace info storage.
        /// </summary>
        internal const string DataSpaceInfoStorage = "DataSpaceInfo";
        /// <summary>
        /// Name of transform info storage.
        /// </summary>
        internal const string TransformInfoStorage = "TransformInfo";
        /// <summary>
        /// Name of encrypted package stream.
        /// </summary>
        internal const string EncryptedPackageStream = "EncryptedPackage";
        internal const string StrongEncryptionDataSpaceStream = "StrongEncryptionDataSpace";
        internal const string StrongEncryptionTransformStream = "StrongEncryptionTransform";
        internal const string VersionStream = "Version";
        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets the type of the encryption.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <returns></returns>
        internal EncrytionType GetEncryptionType(ICompoundStorage storage)
        {
            EncrytionType encryptionType = EncrytionType.None;
            if (storage.ContainsStream(EncryptionInfoStream) &&
                storage.ContainsStorage(DataSpacesStorage))
            {
                using (Stream stream = storage.OpenStream(EncryptionInfoStream))
                {
                    byte[] arrBuffer = new byte[DLSConstants.IntSize];
                    int versionInfo = ReadInt32(stream, arrBuffer);
                    stream.Position = 0;
                    // EncryptionVersionInfo (4 bytes): A Version structure where Version.vMajor MUST be 0x0003 or 0x0004, 
                    // and Version.vMinor MUST be 0x0002 for standard encryption (Word 2007).
                    if (versionInfo == 0x20003 || versionInfo == 0x20004)
                    {
                        encryptionType = EncrytionType.Standard;
                    }
                    // EncryptionVersionInfo (4 bytes): A Version structure where Version.vMajor MUST be 0x0004, 
                    // and Version.vMinor MUST be 0x0004 for agile encryption (Word 2010 / Word 2013).
                    else if (versionInfo == 0x40004)
                    {
                        encryptionType = EncrytionType.Agile;
                    }
                }
            }
            return encryptionType;
        }
        /// <summary>
        /// Reads Int32 value from the stream.
        /// </summary>
        /// <param name="stream">Stream to get data from.</param>
        /// <param name="buffer">Temporary buffer to put extracted bytes into.</param>
        /// <returns>Extracted Int32 value.</returns>
        internal int ReadInt32(Stream stream, byte[] buffer)
        {
            if (stream.Read(buffer, 0, DLSConstants.IntSize) != DLSConstants.IntSize)
                throw new Exception("Invalid data");

            return BitConverter.ToInt32(buffer, 0);
        }
        /// <summary>
        /// Extracts padded unicode string from a stream.
        /// </summary>
        /// <param name="stream">Stream to get data from.</param>
        /// <returns>Extracted string.</returns>
        internal string ReadUnicodeString(Stream stream)
        {
            byte[] arrBuffer = new byte[DLSConstants.IntSize];
            int iLength = ReadInt32(stream, arrBuffer);
            arrBuffer = new byte[iLength];

            if (stream.Read(arrBuffer, 0, iLength) != iLength)
                throw new Exception("Invalid data");

            string result = Encoding.Unicode.GetString(arrBuffer, 0, arrBuffer.Length);

            int iPadding = iLength % 4;

            if (iPadding != 0)
                stream.Position += 4 - iPadding;

            return result;
        }
        /// <summary>
        /// Read zero-terminated string from the stream.
        /// </summary>
        /// <param name="stream">Stream to get string from.</param>
        /// <returns>Extracted string (without trailing zero).</returns>
        internal string ReadUnicodeStringZero(Stream stream)
        {
            StringBuilder builder = new StringBuilder();
            byte[] arrCharacter = new byte[2];

            while (stream.Read(arrCharacter, 0, 2) > 0)
            {
                string chr = Encoding.Unicode.GetString(arrCharacter, 0, arrCharacter.Length);

                if (chr[0] != '\0')
                {
                    builder.Append(chr);
                }
                else
                {
                    break;
                }
            }

            return builder.ToString();
        }
        /// <summary>
        /// Writes Int32 value into the stream.
        /// </summary>
        /// <param name="stream">Stream to put data into.</param>
        /// <param name="value">Value to write.</param>
        internal void WriteInt32(Stream stream, int value)
        {
            byte[] arrData = BitConverter.GetBytes(value);
            stream.Write(arrData, 0, DLSConstants.IntSize);
        }
        /// <summary>
        /// Writes padded unicode string from a stream.
        /// </summary>
        /// <param name="stream">Stream to get data from.</param>
        /// <param name="value">Value to write.</param>
        internal void WriteUnicodeString(Stream stream, string value)
        {
            byte[] arrBuffer = Encoding.Unicode.GetBytes(value);
            int iLength = arrBuffer.Length;
            WriteInt32(stream, iLength);
            stream.Write(arrBuffer, 0, iLength);

            int iPadding = iLength % 4;

            if (iPadding != 0)
            {
                for (int i = 0, len = 4 - iPadding; i < len; i++)
                    stream.WriteByte(0);
            }
        }
        /// <summary>
        /// Writes zero-terminated string into the stream.
        /// </summary>
        /// <param name="stream">Stream to put string into.</param>
        /// <param name="value">Value to write.</param>
        internal void WriteUnicodeStringZero(Stream stream, string value)
        {
            int iStringLength = value.Length;

            byte[] arrData = Encoding.Unicode.GetBytes(value);
            stream.Write(arrData, 0, arrData.Length);

            if (iStringLength == 0 || value[iStringLength - 1] != '\0')
            {
                stream.WriteByte(0);
                stream.WriteByte(0);
            }
        }
        /// <summary>
        /// Creates key object based on the salt and password.
        /// </summary>
        /// <param name="password">Password to use.</param>
        /// <param name="salt">Salt to use.</param>
        /// <param name="keyLength">Required key length.</param>
        /// <returns>Array with created key.</returns>
        internal byte[] CreateKey(string password, byte[] salt, int keyLength)
        {
            SHA1 sha1 = new SHA1Managed();
            byte[] arrPassword = Encoding.Unicode.GetBytes(password);
            byte[] saltPassword = new byte[salt.Length + arrPassword.Length];
            Buffer.BlockCopy(salt, 0, saltPassword, 0, salt.Length);
            Buffer.BlockCopy(arrPassword, 0, saltPassword, salt.Length, arrPassword.Length);

            byte[] H0 = sha1.ComputeHash(saltPassword);
            byte[] inputData = new byte[H0.Length + 4];
            byte[] Hi = H0;
            byte[] iterator;

            for (int i = 0; i < PasswordIterationCount; i++)
            {
                iterator = BitConverter.GetBytes(i);
                Buffer.BlockCopy(iterator, 0, inputData, 0, iterator.Length);
                Buffer.BlockCopy(Hi, 0, inputData, iterator.Length, Hi.Length);
                Hi = sha1.ComputeHash(inputData);
            }

            iterator = BitConverter.GetBytes(0);
            Buffer.BlockCopy(Hi, 0, inputData, 0, Hi.Length);
            Buffer.BlockCopy(iterator, 0, inputData, Hi.Length, iterator.Length);
            byte[] HFinal = sha1.ComputeHash(inputData);

            byte[] vector64 = new byte[64];

            for (int i = 0; i < 64; i++)
                vector64[i] = 0x36;

            for (int i = 0, len = HFinal.Length; i < len; i++)
                vector64[i] ^= HFinal[i];

            byte[] x1 = sha1.ComputeHash(vector64);

            for (int i = 0; i < 64; i++)
                vector64[i] = 0x5C;

            for (int i = 0, len = HFinal.Length; i < len; i++)
                vector64[i] ^= HFinal[i];

            byte[] x2 = sha1.ComputeHash(vector64);

            if (keyLength <= x1.Length)
            {
                byte[] arrResult = new byte[keyLength];
                Buffer.BlockCopy(x1, 0, arrResult, 0, keyLength);
                return arrResult;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Creates the agile encryption key.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="blockKey">The block key.</param>
        /// <param name="keyLength">Length of the key.</param>
        /// <param name="iterationCount">The iteration count.</param>
        /// <returns></returns>
        internal byte[] CreateAgileEncryptionKey(HashAlgorithm hashAlgorithm, string password, byte[] salt, byte[] blockKey, int keyLength, int iterationCount)
        {
            byte[] arrPassword = Encoding.Unicode.GetBytes(password);
            
            // H(0) = H(salt, password);
            byte[] hashBuf = hashAlgorithm.ComputeHash(CombineArray(salt, arrPassword));

            for (int i = 0; i < iterationCount; i++)
            {
                // Generate each hash in turn
                // H(n) = H(i, H(n-1))
                hashBuf = hashAlgorithm.ComputeHash(CombineArray(BitConverter.GetBytes(i), hashBuf));
            }

            // Finally, append "block" (0) to H(n)
            hashBuf = hashAlgorithm.ComputeHash(CombineArray(hashBuf, blockKey));

            // Correct the size as per the specification
            hashBuf = CorrectSize(hashBuf, keyLength, 0x36);

            return hashBuf;
        }
        /// <summary>
        /// Encrypts/decrypts buffer with specified method.
        /// </summary>
        /// <param name="data">Data to process.</param>
        /// <param name="method">Method to use.</param>
        /// <param name="blockSize">Size of the encryption block.</param>
        /// <returns>Modified (encrypted/decrypted) data.</returns>
        internal byte[] EncryptDecrypt(byte[] data, EncryptionMethod method, int blockSize)
        {
            int iLength = data.Length;
            byte[] result = new byte[iLength];
            byte[] arrBuffer = new byte[blockSize];
            byte[] arrTemp2 = new byte[blockSize];

            int iOffset = 0;
            while (iOffset < iLength)
            {
                int iDataLeft = iLength - iOffset;
                int iDataSize = Math.Min(iDataLeft, blockSize);
                Buffer.BlockCopy(data, iOffset, arrBuffer, 0, iDataSize);
                method(arrBuffer, arrTemp2);
                Buffer.BlockCopy(arrTemp2, 0, result, iOffset, iDataSize);
                iOffset += blockSize;
            }

            return result;
        }
        /// <summary>
        /// Combines two arrays into one.
        /// </summary>
        /// <param name="buffer1">The first buffer to combine.</param>
        /// <param name="buffer2">The second buffer to combine.</param>
        /// <returns>Combined array.</returns>
        internal byte[] CombineArray(byte[] buffer1, byte[] buffer2)
        {
            int iLength1 = buffer1.Length;
            int iLength2 = buffer2.Length;
            int iCombinedLength = iLength1 + iLength2;
            byte[] arrResult = new byte[iCombinedLength];

            Buffer.BlockCopy(buffer1, 0, arrResult, 0, iLength1);
            Buffer.BlockCopy(buffer2, 0, arrResult, iLength1, iLength2);

            return arrResult;
        }
        /// <summary>
        /// Corrects the size.
        /// </summary>
        /// <param name="hashBuf">The hash buf.</param>
        /// <param name="size">The size.</param>
        /// <param name="padding">The padding.</param>
        /// <returns></returns>
        internal byte[] CorrectSize(byte[] data, int size, byte padding)
        {
            byte[] arrResult = new byte[size];
            if (data.Length < size)
            {
                Buffer.BlockCopy(data, 0, arrResult, 0, data.Length);
                for (int i = data.Length; i < size; i++)
                {
                    arrResult[i] = padding;
                }
            }
            else if (data.Length >= size)
            {
                Buffer.BlockCopy(data, 0, arrResult, 0, size);
            }
            return arrResult;
        }
        /// <summary>
        /// Concatenates the IV.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="IV">The IV.</param>
        /// <returns></returns>
        internal byte[] ConcatenateIV(byte[] data, byte[] IV)
        {
            byte[] result = new byte[data.Length];
            for (int i = 0; i < result.Length; i++)
            {
                // XOR: combines data with IV - Initialization vector.
                result[i] = (byte)(data[i] ^ IV[i]);
            }
            return result;
        }
        /// <summary>
        /// Compares the array.
        /// </summary>
        /// <param name="buffer1">The buffer1.</param>
        /// <param name="buffer2">The buffer2.</param>
        /// <returns></returns>
        internal bool CompareArray(byte[] buffer1, byte[] buffer2)
        {
            bool equal = true;
            for (int i = 0; i < buffer1.Length; i++)
            {
                if (buffer1[i] != buffer2[i])
                {
                    equal = false;
                    break;
                }
            }
            return equal;
        }
        internal delegate void EncryptionMethod(byte[] buffer1, byte[] buffer2);
        #endregion
    }
}

#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// Abstract class which represents data structures.
    /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
    [StructLayout(LayoutKind.Sequential)]
#endif
    internal abstract class DataStructure
    {
        #region Implementation
        /// <summary>
        /// Parse the data strucure
        /// </summary>
        /// <param name="arrData">Bytes with data</param>
        /// <param name="iOffset">Offset</param>
        internal abstract void Parse(byte[] arrData, int iOffset);
        /// <summary>
        /// Saves the data structure.
        /// </summary>
        /// <param name="arrData">The destination array.</param>
        /// <param name="iOffset">The offset.</param>
        /// <returns>Length</returns>
        internal abstract int Save(byte[] arrData, int iOffset);
        /// <summary>
        /// Gets the size of the structure.
        /// </summary>
        /// <value>The length.</value>
        internal abstract int Length { get; }
        /// <summary>
        /// Reads the int16.
        /// </summary>
        /// <param name="arrData">The data array.</param>
        /// <param name="iOffset">The offset.</param>
        /// <returns></returns>
        internal static short ReadInt16(byte[] arrData, ref int iOffset)
        {
            short val = BitConverter.ToInt16(arrData, iOffset);
            iOffset += Constants.BytesInWord;

            return val;
        }

        /// <summary>
        /// Reads the int32.
        /// </summary>
        /// <param name="arrData">The data array.</param>
        /// <param name="iOffset">The offset.</param>
        /// <returns></returns>
        internal static int ReadInt32(byte[] arrData, ref int iOffset)
        {
            int val = BitConverter.ToInt32(arrData, iOffset);
            iOffset += Constants.BytesInInt;

            return val;
        }
        /// <summary>
        /// Reads the int64.
        /// </summary>
        /// <param name="arrData">The data array.</param>
        /// <param name="iOffset">The offset.</param>
        /// <returns></returns>
        internal static long ReadInt64(byte[] arrData, ref int iOffset)
        {
            long val = BitConverter.ToInt64(arrData, iOffset);
            iOffset += Constants.BytesInLong;

            return val;
        }
        /// <summary>
        /// Reads the int16.
        /// </summary>
        /// <param name="arrData">The data array.</param>
        /// <param name="iOffset">The offset.</param>
        /// <returns></returns>
        internal static ushort ReadUInt16(byte[] arrData, ref int iOffset)
        {
            ushort val = BitConverter.ToUInt16(arrData, iOffset);
            iOffset += Constants.BytesInWord;

            return val;
        }

        /// <summary>
        /// Reads the int32.
        /// </summary>
        /// <param name="arrData">The data array.</param>
        /// <param name="iOffset">The offset.</param>
        /// <returns></returns>
        internal static uint ReadUInt32(byte[] arrData, ref int iOffset)
        {
            uint val = BitConverter.ToUInt32(arrData, iOffset);
            iOffset += Constants.BytesInInt;

            return val;
        }

        /// <summary>
        /// Reads the array of bytes.
        /// </summary>
        /// <param name="arrData">The data array.</param>
        /// <param name="length">The length.</param>
        /// <param name="iOffset">The offset.</param>
        /// <returns></returns>
        internal static byte[] ReadBytes(byte[] arrData, int length, ref int iOffset)
        {
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = arrData[iOffset + i];
            }

            iOffset += length;

            return bytes;
        }

        /// <summary>
        /// Saves the specified int16 value in the data array.
        /// </summary>
        /// <param name="arrData">The arr data.</param>
        /// <param name="iOffset">The offset.</param>
        internal static void WriteInt16(byte[] destination, ref int iOffset, short val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            WriteBytes(destination, ref iOffset, bytes);
        }

        /// <summary>
        /// Saves the specified uint16 value in the data array.
        /// </summary>
        /// <param name="arrData">The arr data.</param>
        /// <param name="iOffset">The offset.</param>
        internal static void WriteUInt16(byte[] destination, ref int iOffset, ushort val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            WriteBytes(destination, ref iOffset, bytes);
        }

        /// <summary>
        /// Saves the specified int32 value in the data array.
        /// </summary>
        /// <param name="arrData">The arr data.</param>
        /// <param name="iOffset">The offset.</param>
        internal static void WriteInt32(byte[] destination, ref int iOffset, int val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            WriteBytes(destination, ref iOffset, bytes);
        }
        /// <summary>
        /// Saves the specified int64 value in the data array.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="iOffset">The offset.</param>
        /// <param name="val">The value.</param>
        internal static void WriteInt64(byte[] destination, ref int iOffset, long val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            WriteBytes(destination, ref iOffset, bytes);
        }
        /// <summary>
        /// Saves the specified uint32 value in the data array.
        /// </summary>
        /// <param name="arrData">The arr data.</param>
        /// <param name="iOffset">The offset.</param>
        internal static void WriteUInt32(byte[] destination, ref int iOffset, uint val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            WriteBytes(destination, ref iOffset, bytes);
        }

        /// <summary>
        /// Saves the bytes byte array.
        /// </summary>
        /// <param name="destination">The destination array.</param>
        /// <param name="iOffset">The offset.</param>
        /// <param name="bytes">The bytes.</param>
        internal static void WriteBytes(byte[] destination, ref int iOffset, byte[] bytes)
        {
            int len = bytes.Length;
            for (int i = 0; i < len; i++)
            {
                destination[iOffset + i] = bytes[i];
            }

            iOffset += len;
        }

        /// <summary>
        /// Copies the memory.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="source">The source.</param>
        /// <param name="length">The length.</param>
        internal static void CopyMemory(byte[] destination, byte[] source, int length)
        {
            for (int i = 0; i < length; i++)
            {
                destination[i] = source[i];
            }
        }
        #endregion
    }
}

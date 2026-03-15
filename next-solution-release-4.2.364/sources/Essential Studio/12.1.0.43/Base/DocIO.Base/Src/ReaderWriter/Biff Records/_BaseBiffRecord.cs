#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using Syncfusion.DocIO.DLS;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Base class for all records in Word document.
    /// </summary>
    [CLSCompliant(false)]
#if AllowUnsafeCode
    [StructLayout(LayoutKind.Sequential)]
#endif
    [Syncfusion.Documentation.DocumentationExclude()]
    internal abstract class BaseWordRecord
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        internal static readonly int[] WordKnownColors =
      {
        0, -16777216, -16776961, -16711681, -16744448, -65281, -65536, -256, -1,
        -16777077, -16741493, -16751616, -7667573, -7667712, -256, -5658199, -2894893
      };

        /// <summary>
        /// Number of bits in byte.
        /// </summary>
        private const int DEF_BITS_IN_BYTE = 8;

        /// <summary>
        /// Number of bits in short.
        /// </summary>
        private const int DEF_BITS_IN_SHORT = 16;

        /// <summary>
        /// Number of bits in int.
        /// </summary>
        private const int DEF_BITS_IN_INT = 32;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWordRecord"/> class.
        /// </summary>
        internal BaseWordRecord()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWordRecord"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        internal BaseWordRecord(byte[] data)
        {
            Parse(data);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWordRecord"/> class.
        /// </summary>
        /// <param name="arrData">The arr data.</param>
        /// <param name="iOffset">The i offset.</param>
        internal BaseWordRecord(byte[] arrData, int iOffset)
        {
            Parse(arrData, iOffset);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWordRecord"/> class.
        /// </summary>
        /// <param name="arrData">The arr data.</param>
        /// <param name="iOffset">The i offset.</param>
        /// <param name="iCount">The i count.</param>
        internal BaseWordRecord(byte[] arrData, int iOffset, int iCount)
        {
            Parse(arrData, iOffset, iCount);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWordRecord"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="iCount">The i count.</param>
        internal BaseWordRecord(Stream stream, int iCount)
        {
            Parse(stream, iCount);
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        internal virtual int Length
        {
            get
            {
                DataStructure structure = UnderlyingStructure;

                if (structure != null)
                {
                    return structure.Length;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the underlying structure.
        /// </summary>
        /// <value>The underlying structure.</value>
        protected virtual DataStructure UnderlyingStructure
        {
            get
            {
                throw new Exception("UnderlyingStructure of BiffRecord");
                return null;
            }
        }
        #endregion

        #region Class Static Methods
        /// <summary>
        /// Gets the index of the known color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        internal static int GetKnownColorIndex(Color color)
        {
            int clIndex = 0;

            for (int i = 0, len = WordKnownColors.Length; i < len; i++)
            {
                if (Color.FromArgb(WordKnownColors[i]) == color)
                {
                    clIndex = i;
                    break;
                }
            }

            return clIndex;
        }

        /// <summary>
        /// Returns value of the single bit from byte.
        /// </summary>
        /// <param name="btOptions">Byte to get bit value from.</param>
        /// <param name="bitPos">Bit index.</param>
        /// <returns>Value of the single bit from byte.</returns>
        internal static bool GetBit(byte btOptions, int bitPos)
        {
            if (bitPos < 0 || bitPos >= DEF_BITS_IN_BYTE)
#if SILVERLIGHT || WP
        throw new ArgumentOutOfRangeException( "bitPos", "Bit Position cannot be less than 0 or greater than 7." );
#else
                throw new ArgumentOutOfRangeException("bitPos", bitPos, "Bit Position cannot be less than 0 or greater than 7.");
#endif


            return GetBit((int)btOptions, bitPos);
        }

        /// <summary>
        /// Returns value of the single bit from byte.
        /// </summary>
        /// <param name="sOptions">Int16 to get bit value from.</param>
        /// <param name="bitPos">Bit index.</param>
        /// <returns>Value of the single bit from byte.</returns>
        internal static bool GetBit(short sOptions, int bitPos)
        {
            if (bitPos < 0 || bitPos >= DEF_BITS_IN_SHORT)
#if SILVERLIGHT || WP
        throw new ArgumentOutOfRangeException( "bitPos", "Bit Position cannot be less than 0 or greater 15." );
#else
                throw new ArgumentOutOfRangeException("bitPos", bitPos, "Bit Position cannot be less than 0 or greater 15.");
#endif

            if (bitPos == DEF_BITS_IN_SHORT - 1) return (sOptions < 0);

            return GetBit((int)sOptions, bitPos);
        }

        /// <summary>
        /// Returns value of the single bit from byte.
        /// </summary>
        /// <param name="iOptions">Byte to get bit value from.</param>
        /// <param name="bitPos">Bit index.</param>
        /// <returns>Value of the single bit from byte.</returns>
        internal static bool GetBit(int iOptions, int bitPos)
        {
            if (bitPos < 0 || bitPos >= DEF_BITS_IN_INT)
#if SILVERLIGHT || WP
        throw new ArgumentOutOfRangeException( "bitPos", "Bit Position cannot be less than 0 or greater 31." );
#else
                throw new ArgumentOutOfRangeException("bitPos", bitPos, "Bit Position cannot be less than 0 or greater 31.");
#endif

            if (bitPos == DEF_BITS_IN_INT - 1) return (iOptions < 0);

            return (iOptions & (1 << bitPos)) != 0;
        }

        /// <summary>
        /// Gets the bits by mask.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="BitMask">The bit mask.</param>
        /// <param name="iStartBit">The i start bit.</param>
        /// <returns></returns>
        internal static int GetBitsByMask(int value, int BitMask, int iStartBit)
        {
            return (value & BitMask) >> iStartBit;
        }

        /// <summary>
        /// Gets the bits by mask.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="BitMask">The bit mask.</param>
        /// <param name="iStartBit">The i start bit.</param>
        /// <returns></returns>
        internal static uint GetBitsByMask(uint value, int BitMask, int iStartBit)
        {
            return (uint)((value & BitMask) >> iStartBit);
        }

        /// <summary>
        /// Sets the bit.
        /// </summary>
        /// <param name="iValue">The i value.</param>
        /// <param name="bitPos">The bit pos.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns></returns>
        internal static int SetBit(int iValue, int bitPos, bool value)
        {
            if (bitPos < 0 || bitPos >= DEF_BITS_IN_INT)
#if SILVERLIGHT || WP
        throw new ArgumentOutOfRangeException( "bitPos", "Bit Position cannot be less than 0 or greater 32." );
#else
                throw new ArgumentOutOfRangeException("bitPos", bitPos, "Bit Position cannot be less than 0 or greater 32.");
#endif

            if (bitPos == DEF_BITS_IN_INT - 1)
            {
                iValue = Math.Abs(iValue);

                if (!value) iValue = -iValue;
            }
            else if (value)
            {
                iValue |= (1 << bitPos);
            }
            else
            {
                iValue &= (~(1 << bitPos));
            }

            return iValue;
        }

        /// <summary>
        /// Sets value with all bits that correspond to the specified
        /// mask to zero to the same values as in value.
        /// </summary>
        /// <param name="destination">Variable that bits of which will be set.</param>
        /// <param name="BitMask">Bit mask.</param>
        /// <param name="value">Value from which bit values will be taken.</param>
        /// <returns>Value after set operation.</returns>
        internal static int SetBitsByMask(int destination, int BitMask, int value)
        {
            destination &= ~BitMask;
            destination += value & BitMask;

            return destination;
        }

        /// <summary>
        /// Sets the bits by mask.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="BitMask">The bit mask.</param>
        /// <param name="iStartBit">The i start bit.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        internal static int SetBitsByMask(int destination, int BitMask, int iStartBit, int value)
        {
            destination &= ~BitMask;
            destination += (value << iStartBit) & BitMask;

            return destination;
        }

        /// <summary>
        /// Sets value with all bits that correspond to the specified
        /// mask to zero to the same values as in value.
        /// </summary>
        /// <param name="destination">Variable that bits of which will be set.</param>
        /// <param name="BitMask">Bit mask.</param>
        /// <param name="value">Value from which bit values will be taken.</param>
        /// <returns>Value after set operation.</returns>
        internal static uint SetBitsByMask(uint destination, int BitMask, int value)
        {
            destination &= (uint)(~BitMask);
            destination += (uint)(value & BitMask);

            return destination;
        }

        /// <summary>
        /// Gets the bit.
        /// </summary>
        /// <param name="uiOptions">The UI options.</param>
        /// <param name="bitPos">The bit pos.</param>
        /// <returns></returns>
        internal static bool GetBit(uint uiOptions, int bitPos)
        {
            if (bitPos < 0 || bitPos > DEF_BITS_IN_INT)
                throw new ArgumentOutOfRangeException("bitPos", "Bit Position cannot be less than 0 or greater 31.");

            return (uiOptions & (1 << bitPos)) != 0;
        }

        /// <summary>
        /// Sets one bit in specified Int32.
        /// </summary>
        /// <param name="uiValue">Int32 to set bit.</param>
        /// <param name="bitPos">Bit position in the byte</param>
        /// <param name="value">Value of bit</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// If bitPos is less than zero or more than 7
        /// </exception>
        /// <returns>Value after</returns>
        internal static uint SetBit(uint uiValue, int bitPos, bool value)
        {
            if (bitPos < 0 || bitPos >= DEF_BITS_IN_INT)
                throw new ArgumentOutOfRangeException("bitPos", "Bit Position can be zeroless or greater 32.");

            if (value)
            {
                uiValue |= (uint)(1 << bitPos);
            }
            else
            {
                uiValue &= (uint)(~(1 << bitPos));
            }

            return uiValue;
        }

        /// <summary>
        /// Reads UInt16 value from the stream.
        /// </summary>
        /// <param name="stream">Stream to read value from.</param>
        /// <returns>Read value.</returns>
        internal static ushort ReadUInt16(Stream stream)
        {
            byte[] arrBuffer = new byte[Constants.BytesInWord];
            int iReadCount = stream.Read(arrBuffer, 0, Constants.BytesInWord);

            if (iReadCount != Constants.BytesInWord)
                throw new StreamReadException();

            return BitConverter.ToUInt16(arrBuffer, 0);
        }

        /// <summary>
        /// Reads UInt32 value from the stream.
        /// </summary>
        /// <param name="stream">Stream to read value from.</param>
        /// <returns>Read value.</returns>
        internal static uint ReadUInt32(Stream stream)
        {
            byte[] arrBuffer = new byte[Constants.BytesInInt];
            int iReadCount = stream.Read(arrBuffer, 0, Constants.BytesInInt);

            if (iReadCount != Constants.BytesInInt)
                throw new StreamReadException();

            return BitConverter.ToUInt32(arrBuffer, 0);
        }

        /// <summary>
        /// Reads UInt16 value from the stream.
        /// </summary>
        /// <param name="stream">Stream to read value from.</param>
        /// <returns>Read value.</returns>
        internal static short ReadInt16(Stream stream)
        {
            byte[] arrBuffer = new byte[Constants.BytesInWord];
            int iReadCount = stream.Read(arrBuffer, 0, Constants.BytesInWord);

            if (iReadCount != Constants.BytesInWord)
                throw new StreamReadException();

            return BitConverter.ToInt16(arrBuffer, 0);
        }

        /// <summary>
        /// Reads UInt32 value from the stream.
        /// </summary>
        /// <param name="stream">Stream to read value from.</param>
        /// <returns>Read value.</returns>
        internal static int ReadInt32(Stream stream)
        {
            byte[] arrBuffer = new byte[Constants.BytesInInt];
            int iReadCount = stream.Read(arrBuffer, 0, Constants.BytesInInt);

            if (iReadCount != Constants.BytesInInt)
                throw new StreamReadException();

            return BitConverter.ToInt32(arrBuffer, 0);
        }

        /// <summary>
        /// Gets UInt16 value from the array.
        /// </summary>
        /// <param name="arrData">Array of bytes to get value from.</param>
        /// <param name="iOffset">Offset of the UInt16 value.</param>
        /// <returns>UInt16 value read from array.</returns>
        internal static ushort ReadUInt16(byte[] arrData, int iOffset)
        {
            if (arrData == null)
                throw new ArgumentNullException("arrData");

            if (iOffset < 0 || iOffset > arrData.Length - Constants.BytesInWord)
                throw new ArgumentOutOfRangeException("iOffset", "Value can not be less 0 and greater arrData.Length - Constants.BytesInWord");

            return BitConverter.ToUInt16(arrData, iOffset);
        }

        /// <summary>
        /// Gets UInt16 value from the array and moves position.
        /// </summary>
        /// <param name="arrData">Array of bytes to get value from.</param>
        /// <param name="iOffset">Offset of the UInt16 value.</param>
        /// <returns>UInt16 value read from array.</returns>
        internal static ushort ReadUInt16(byte[] arrData, ref int iOffset)
        {
            ushort result = ReadUInt16(arrData, iOffset);
            iOffset += Constants.BytesInWord;
            return result;
        }

        /// <summary>
        /// Reads the string.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        internal static string ReadString(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            ushort usSize = ReadUInt16(stream);
            int usSizeInBytes = usSize * 2;

            if ((usSizeInBytes + stream.Position) > stream.Length)
            {
                ////        usSizeInBytes = ( int )( stream.Length - stream.Position - 1 );
                return string.Empty;
            }

            byte[] arrBuffer = new byte[usSizeInBytes];
            int iReadCount = stream.Read(arrBuffer, 0, usSizeInBytes);

            if (iReadCount != usSizeInBytes)
                throw new Exception("Unable to read required data from the stream");
#if SILVERLIGHT || WP
      return Encoding.Unicode.GetString( arrBuffer, 0, arrBuffer.Length );
#else
            return Encoding.Unicode.GetString(arrBuffer);
#endif
        }

        /// <summary>
        /// Writes the string.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="str">The STR.</param>
        internal static void WriteString(Stream stream, string str)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            WriteUInt16(stream, (ushort)(Encoding.Unicode.GetByteCount(str) / 2));

            byte[] arrBuffer = Encoding.Unicode.GetBytes(str);
            stream.Write(arrBuffer, 0, arrBuffer.Length);
        }

        /// <summary>
        /// Gets string from array of bytes.
        /// </summary>
        /// <param name="arrData">Array with string data.</param>
        /// <param name="iOffset">Offset to the string data.</param>
        /// <returns>String that was extracted from the array.</returns>
        internal static string ReadString(byte[] arrData, int iOffset)
        {
            if (arrData == null)
                throw new ArgumentNullException("arrData");

            if (iOffset < 0 || iOffset > arrData.Length - Constants.BytesInWord)
                throw new ArgumentOutOfRangeException("iOffset",
                  "Value can not be less 0 and greater arrData.Length - Constants.BytesInWord");

            ushort usCount = ReadUInt16(arrData, iOffset);
            iOffset += Constants.BytesInWord;

            return Encoding.Unicode.GetString(arrData, iOffset, usCount);
        }

        /// <summary>
        /// Reads the string.
        /// </summary>
        /// <param name="arrData">The arr data.</param>
        /// <param name="iOffset">The i offset.</param>
        /// <param name="usCount">The us count.</param>
        /// <returns></returns>
        internal static string ReadString(byte[] arrData, int iOffset, ushort usCount)
        {
            if (arrData == null)
                throw new ArgumentNullException("arrData");

            if (iOffset < 0 || iOffset > arrData.Length - Constants.BytesInWord)
                throw new ArgumentOutOfRangeException("iOffset",
                  "Value can not be less 0 and greater arrData.Length - Constants.BytesInWord");

            return Encoding.Unicode.GetString(arrData, iOffset, usCount);
        }

        /// <summary>
        /// Gets the zero terminated string.
        /// </summary>
        /// <param name="arrData">The arr data.</param>
        /// <param name="iOffset">The i offset.</param>
        /// <param name="iEndPos">The i end pos.</param>
        /// <returns></returns>
        internal static string GetZeroTerminatedString(byte[] arrData, int iOffset, out int iEndPos)
        {
            byte btLength = arrData[iOffset];
            iOffset += 2;
#if DEBUG_BIFFRECORD
      Debug.WriteLine( btLength, "Length of the string" );
#endif
            string result = string.Empty;
            iEndPos = iOffset + btLength * 2;

            if (btLength != 0)
            {
                result = Encoding.Unicode.GetString(arrData, iOffset, btLength * 2);
            }

            int endPosition = iEndPos;
            //Loop till we get continuous zeros
            while (endPosition < arrData.Length - 1)
            {
                // Check if it is really zero-ended
                if (arrData[endPosition] == 0 && arrData[endPosition + 1] == 0)
                {
                    iEndPos += 2;
                    return result;
                }
                endPosition += 1;
            }

            throw new Exception("Stored string should be zero-ended");
        }

        /// <summary>
        /// Toes the zero terminated array.
        /// </summary>
        /// <param name="str">The sting.</param>
        /// <returns></returns>
        internal static byte[] ToZeroTerminatedArray(string str)
        {
            byte[] arrZT = new byte[str.Length * 2 + 4];
            arrZT[0] = (byte)str.Length;
            Encoding.Unicode.GetBytes(str.ToCharArray(), 0, str.Length, arrZT, 2);
            return arrZT;
        }

        /// <summary>
        /// Writes the U int16.
        /// </summary>
        /// <param name="arrData">The arr data.</param>
        /// <param name="usValue">The us value.</param>
        /// <param name="iOffset">The i offset.</param>
        internal static void WriteUInt16(byte[] arrData, ushort usValue, ref int iOffset)
        {
            iOffset = WriteBytes(arrData, BitConverter.GetBytes(usValue), iOffset);
        }

        /// <summary>
        /// Writes the U int32.
        /// </summary>
        /// <param name="arrData">The arr data.</param>
        /// <param name="uintValue">The uint value.</param>
        /// <param name="iOffset">The i offset.</param>
        internal static void WriteUInt32(byte[] arrData, uint uintValue, ref int iOffset)
        {
            iOffset = WriteBytes(arrData, BitConverter.GetBytes(uintValue), iOffset);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        internal static void WriteUInt32(Stream stream, uint value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            stream.Write(arr, 0, arr.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        internal static void WriteInt32(Stream stream, int value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            stream.Write(arr, 0, arr.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        internal static void WriteInt16(Stream stream, short value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            stream.Write(arr, 0, arr.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        internal static void WriteUInt16(Stream stream, ushort value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            stream.Write(arr, 0, arr.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrData"></param>
        /// <param name="strValue"></param>
        /// <param name="iOffset"></param>
        internal static void WriteString(byte[] arrData, string strValue, ref int iOffset)
        {
            Encoding encoding = Encoding.Unicode;
            iOffset = WriteBytes(arrData, encoding.GetBytes(strValue), iOffset);
        }

        /// <summary>
        /// Copy array of bytes to target array 
        /// and changes iOffset parameter.
        /// </summary>
        /// <param name="arrData"></param>
        /// <param name="bytes"></param>
        /// <param name="iOffset"></param>
        /// <returns></returns>
        internal static int WriteBytes(byte[] arrData, byte[] bytes, int iOffset)
        {
            if (arrData == null)
                throw new ArgumentNullException("arrData");
            //// no check for usValue
            if (iOffset < 0 || iOffset > arrData.Length - Constants.BytesInWord)
                throw new ArgumentOutOfRangeException("iOffset",
                  "Value can not be less 0 and greater arrData.Length - Constants.BytesInWord");

            bytes.CopyTo(arrData, iOffset);
            return iOffset + bytes.Length;
        }

        /// <summary>
        /// Reads the bytes.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        internal byte[] ReadBytes(Stream stream, int i)
        {
            byte[] arrBuffer = new byte[i];
            int iReadCount = stream.Read(arrBuffer, 0, i);

            if (iReadCount != i)
                throw new StreamReadException();

            return arrBuffer;
        }
        #endregion

        #region Class Methods
        /// <summary>
        /// Parses record.
        /// </summary>
        /// <param name="data">Data to parse.</param>    
        internal virtual void Parse(byte[] data)
        {
            Parse(data, 0);
        }

        /// <summary>
        /// Parses record.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset in the data array to the records data.</param>    
        internal virtual void Parse(byte[] arrData, int iOffset)
        {
            Parse(arrData, iOffset, arrData.Length - iOffset);
        }

        /// <summary>
        /// Parses the specified arr data.
        /// </summary>
        /// <param name="arrData">The arr data.</param>
        /// <param name="iOffset">The i offset.</param>
        /// <param name="iCount">The i count.</param>
        internal virtual void Parse(byte[] arrData, int iOffset, int iCount)
        {
            DataStructure toParse = UnderlyingStructure;

            if (toParse == null)
                throw new ArgumentNullException("UnderlyingStructure");

            if (arrData == null)
                throw new ArgumentNullException("arrData");

            if (iOffset < 0)
                throw new ArgumentOutOfRangeException("iOffset");

            if (iCount < 0)
                throw new ArgumentOutOfRangeException("iCount");

            if (iOffset + iCount > arrData.Length)
                throw new ArgumentOutOfRangeException("iOffset + iCount");

#if AllowUnsafeCode && !SILVERLIGHT && !WP
            MemoryConverter.Instance.Copy(arrData, iOffset, iCount, toParse);
#else      
      UnderlyingStructure.Parse( arrData, iOffset );
#endif
        }

        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="iCount">The i count.</param>
        internal virtual void Parse(Stream stream, int iCount)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (iCount < 0)
                throw new ArgumentOutOfRangeException("iCount cannot be less than 0");

            byte[] arrBuffer = new byte[iCount];
            int iReadCount = stream.Read(arrBuffer, 0, iCount);

            if (iReadCount != iCount)
                throw new Exception("Couldn't read required bytes from the stream");

            Parse(arrBuffer, 0, iCount);
        }

        /// <summary>
        /// Saves the specified arr data.
        /// </summary>
        /// <param name="arrData">The arr data.</param>
        /// <param name="iOffset">The i offset.</param>
        /// <returns></returns>
        internal virtual int Save(byte[] arrData, int iOffset)
        {
            if (arrData == null)
                throw new ArgumentNullException("arrData");

            if (iOffset < 0 || iOffset > arrData.Length)
                throw new ArgumentOutOfRangeException("iOffset");

            int iLength = arrData.Length - iOffset;

            ////if( iLength - iOffset < Length )
            ////  throw new ArgumentOutOfRangeException( "Length" );      

            DataStructure toSave = UnderlyingStructure;

            if (toSave == null)
                throw new ArgumentNullException("UnderlyingStructure");

#if AllowUnsafeCode && !SILVERLIGHT && !WP
            MemoryConverter.Instance.Copy(toSave, arrData, iOffset, iLength);
#else
      toSave.Save( arrData, iOffset );      
#endif

            return iLength;
        }

        /// <summary>
        /// Saves the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        internal virtual int Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            ////      int iLength = Length;
            int iLength = this.Length;

            if (iLength < 0)
                throw new ArgumentOutOfRangeException("iLength");

            byte[] arrBuffer = new byte[iLength];
            Save(arrBuffer, 0);

            stream.Write(arrBuffer, 0, iLength);

            return iLength;
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal virtual void Close()
        {
        }
        #endregion
    }
}

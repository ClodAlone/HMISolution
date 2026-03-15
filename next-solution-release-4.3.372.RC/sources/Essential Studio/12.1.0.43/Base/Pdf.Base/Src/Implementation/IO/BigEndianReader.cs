#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.IO;
using System.Text;

namespace Syncfusion.Pdf.IO
{
    /// <summary>
    /// Reader of the big endian data.
    /// </summary>
    internal class BigEndianReader
    {
        #region Constants
        /// <summary>
        /// Size of Int32 type.
        /// </summary>
        internal const int Int32Size = 4;
        /// <summary>
        /// Size of Int16 type.
        /// </summary>
        internal const int Int16Size = 2;
        /// <summary>
        /// Size of long type.
        /// </summary>
        internal const int Int64Size = 8;
        /// <summary>
        /// Reader encoding.
        /// </summary>
#if SILVERLIGHT || NETFX_CORE || WP
        private readonly Encoding c_encoding = new Windows1252Encoding();
#else
        private readonly Encoding c_encoding = Encoding.GetEncoding(1252);
#endif
        /// <summary>
        /// Fraction coefficient for getting fixed type.
        /// </summary>
        private const float c_fraction = 16384f;
        #endregion

        #region Fields
        /// <summary>
        /// Binary reader.
        /// </summary>
        private BinaryReader m_reader;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets binary reader.
        /// </summary>
        public BinaryReader Reader
        {
            get
            {
                return m_reader;
            }
            set
            {
                m_reader = value;
            }
        }

        /// <summary>
        /// Gets base stream.
        /// </summary>
        public Stream BaseStream
        {
            get
            {
                return m_reader.BaseStream;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BigEndianReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public BigEndianReader(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            m_reader = reader;
        }

        /// <summary>
        /// Closes all resources.
        /// </summary>
        public void Close()
        {

            if (m_reader != null)
            {
                if (m_reader.BaseStream != null)
                {
#if NETFX_CORE || WP
                    m_reader.BaseStream.Dispose();
#else
                    m_reader.BaseStream.Close();
#endif
                }

#if NETFX_CORE || WP
                m_reader.Dispose();
#else
                m_reader.Close();
#endif
                m_reader = null;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Seeks reader to the sepcified position.
        /// </summary>
        /// <param name="position">Position of the reader.</param>
        public void Seek(long position)
        {
            if (m_reader.BaseStream.CanSeek)
            {
                m_reader.BaseStream.Position = position;
            }
        }

        /// <summary>
        /// Skips number of bytes.
        /// </summary>
        /// <param name="numBytes">Number of bytes to skip.</param>
        public void Skip(long numBytes)
        {
            Seek(m_reader.BaseStream.Position + numBytes);
        }

        /// <summary>
        /// Reverts array elements.
        /// </summary>
        /// <param name="buffer">Byte array.</param>
        /// <returns>Reverted array.</returns>
        public byte[] Reverse(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            Array.Reverse(buffer);

            return buffer;
        }

        /// <summary>
        /// Reads 64 bit integer.
        /// </summary>
        /// <returns>64 bit integer.</returns>
        public long ReadInt64()
        {
            byte[] buffer = m_reader.ReadBytes(Int64Size);
            buffer = Reverse(buffer);
            long value = BitConverter.ToInt64(buffer, 0);

            return value;
        }

        /// <summary>
        /// Reads 64 bit integer.
        /// </summary>
        /// <returns>64 bit integer.</returns>
        public ulong ReadUInt64()
        {
            byte[] buffer = m_reader.ReadBytes(Int64Size);
            buffer = Reverse(buffer);
            ulong value = BitConverter.ToUInt64(buffer, 0);

            return value;
        }

        /// <summary>
        /// Reads 32 bit integer.
        /// </summary>
        /// <returns>32 bit integer.</returns>
        public int ReadInt32()
        {
            byte[] buffer = m_reader.ReadBytes(Int32Size);
            buffer = Reverse(buffer);
            int value = BitConverter.ToInt32(buffer, 0);

            return value;
        }

        /// <summary>
        /// Reads 32 bit integer.
        /// </summary>
        /// <returns>32 bit integer.</returns>
        public uint ReadUInt32()
        {
            byte[] buffer = m_reader.ReadBytes(Int32Size);
            buffer = Reverse(buffer);
            uint value = BitConverter.ToUInt32(buffer, 0);

            return value;
        }

        /// <summary>
        /// Reads 16 bit integer.
        /// </summary>
        /// <returns>16 bit integer.</returns>
        public short ReadInt16()
        {
            byte[] buffer = m_reader.ReadBytes(Int16Size);
            buffer = Reverse(buffer);
            short value = BitConverter.ToInt16(buffer, 0);

            return value;
        }

        /// <summary>
        /// Reads 16 bit integer.
        /// </summary>
        /// <returns>16 bit integer.</returns>
        public ushort ReadUInt16()
        {
            byte[] buffer = m_reader.ReadBytes(Int16Size);
            buffer = Reverse(buffer);
            ushort value = BitConverter.ToUInt16(buffer, 0);

            return value;
        }

        /// <summary>
        /// Reads one byte.
        /// </summary>
        /// <returns>One byte.</returns>
        public byte ReadByte()
        {
            byte val = m_reader.ReadByte();
            return val;
        }

        /// <summary>
        /// Reads FIXED data type (16.16) fixed point number.
        /// </summary>
        /// <returns>FIXED data type (16.16) fixed point number.</returns>
        public float ReadFixed()
        {
            byte[] buffer = m_reader.ReadBytes(Int16Size);
            buffer = Reverse(buffer);
            short integer = BitConverter.ToInt16(buffer, 0);

            buffer = m_reader.ReadBytes(Int16Size);
            buffer = Reverse(buffer);
            short sFraction = BitConverter.ToInt16(buffer, 0);
            float fraction = sFraction / c_fraction;

            float value = integer + fraction;

            return value;
        }

        /// <summary>
        /// Reads bytes from the reader.
        /// </summary>
        /// <param name="count">Number of bytes.</param>
        /// <returns>Byte array.</returns>
        public byte[] ReadBytes(int count)
        {
            return m_reader.ReadBytes(count);
        }

        /// <summary>
        /// Reads string.
        /// </summary>
        /// <param name="len">Size of the string in bytes.</param>
        /// <returns>String data.</returns>
        public string ReadString(int len)
        {
            string text = ReadString(len, false);
            return text;
        }

        /// <summary>
        /// Reads string.
        /// </summary>
        /// <param name="len">Size of the string in bytes.</param>
        /// <param name="unicode">Indicates whethere string is unicode or not.</param>
        /// <returns>String data.</returns>
        public string ReadString(int len, bool unicode)
        {
            string text = null;

            if (unicode)
            {
                byte[] buffer = ReadBytes(len);
                text = Encoding.BigEndianUnicode.GetString(buffer, 0, buffer.Length);
            }
            else
            {
                byte[] buffer = ReadBytes(len);
                text = c_encoding.GetString(buffer, 0, buffer.Length);
            }

            return text;
        }

        /// <summary>
        /// Reads bytes to array in BigEndian order.
        /// </summary>
        /// <param name="buffer">Byte array.</param>
        /// <param name="index">Start index.</param>
        /// <param name="count">Number bytes to read.</param>
        /// <returns>Number bytes that was read.</returns>
        public int Read(byte[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            int written = 0;
            int read = 0;

            do
            {
                read = m_reader.Read(buffer, index + written, count - written);
                written += read;
            }
            while (written < count);

            return written;
        }
        #endregion
    }
}

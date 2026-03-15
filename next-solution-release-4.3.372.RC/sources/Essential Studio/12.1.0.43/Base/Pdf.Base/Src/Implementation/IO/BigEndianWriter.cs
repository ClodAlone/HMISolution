#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Text;

namespace Syncfusion.Pdf.IO
{
    /// <summary>
    /// Writes data in BigEndian order.
    /// </summary>
    internal class BigEndianWriter
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
#if SILVERLIGHT
        private readonly Encoding c_encoding = new Windows1252Encoding();
#else
        private readonly Encoding c_encoding = Encoding.GetEncoding("windows-1252");
#endif
        /// <summary>
        /// Fraction coefficient for getting fixed type.
        /// </summary>
        private const float c_fraction = 16384f;
        #endregion

        #region Fields
        /// <summary>
        /// Internal buffer.
        /// </summary>
        private byte[] m_buffer;
        /// <summary>
        /// Current position.
        /// </summary>
        private int m_position;
        #endregion

        #region Properties
        /// <summary>
        /// Gets data written to the writter.
        /// </summary>
        public byte[] Data
        {
            get
            {
                return m_buffer;
            }
        }
        /// <summary>
        /// Gets position of the internal buffer.
        /// </summary>
        public int Position
        {
            get
            {
                return m_position;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new writer.
        /// </summary>
        /// <param name="capacity">Capacity of the data.</param>
        public BigEndianWriter(int capacity)
        {
            m_buffer = new byte[capacity];
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Writes short value.
        /// </summary>
        /// <param name="value">Value.</param>
        public void Write(short value)
        {
            byte[] buff = BitConverter.GetBytes(value);
            Array.Reverse(buff);
            Flush(buff);
        }

        /// <summary>
        /// Writes ushort value.
        /// </summary>
        /// <param name="value">Value.</param>
        public void Write(ushort value)
        {
            byte[] buff = BitConverter.GetBytes(value);
            Array.Reverse(buff);
            Flush(buff);
        }

        /// <summary>
        /// Writes int value.
        /// </summary>
        /// <param name="value">Value.</param>
        public void Write(int value)
        {
            byte[] buff = BitConverter.GetBytes(value);
            Array.Reverse(buff);
            Flush(buff);
        }

        /// <summary>
        /// Writes uint value.
        /// </summary>
        /// <param name="value">Value.</param>
        public void Write(uint value)
        {
            byte[] buff = BitConverter.GetBytes(value);
            Array.Reverse(buff);
            Flush(buff);
        }

        /// <summary>
        /// Writes string value.
        /// </summary>
        /// <param name="value">Value.</param>
        public void Write(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            byte[] buff = c_encoding.GetBytes(value);
            Flush(buff);
        }

        /// <summary>
        /// Writes byte[] value.
        /// </summary>
        /// <param name="value">Value.</param>
        public void Write(byte[] value)
        {
            Flush(value);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Writes array to the buffer.
        /// </summary>
        /// <param name="buff">Byte data.</param>
        private void Flush(byte[] buff)
        {
            if (buff == null)
                throw new ArgumentNullException("buff");

            Array.Copy(buff, 0, m_buffer, m_position, buff.Length);
            m_position += buff.Length;
        }
        #endregion
    }
}

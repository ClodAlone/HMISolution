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
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// 
    /// </summary>
    //  [ StructLayout( LayoutKind.Sequential ) ]
    //[ CLSCompliant( false ) ]
    internal class FIBStructure : DataStructure
    {
        #region Class constants
        /// <summary>
        /// Offset to count of shorts field
        /// </summary>
        private const int DEF_OFFSET_SHORTS_COUNT = 32;

        /// <summary>
        /// Number of bytes in short value.
        /// </summary>
        private const int DEF_SHORT_BYTES = 2;

        /// <summary>
        /// Number of bytes in member of "array of longs".
        /// </summary>
        private const int DEF_LONG_BYTES = 4;

        /// <summary>
        /// Size of FC / LCB pair.
        /// </summary>
        private const int DEF_FCLCB_BYTES = 8;
        #endregion

        #region Class members
        /// <summary>
        /// Fib header.
        /// </summary>
        internal FIBHeader header = new FIBHeader();

        /// <summary>
        /// Array of shorts.
        /// </summary>
        internal ArrayOfShorts arrShorts = new ArrayOfShorts();

        /// <summary>
        /// Number of fields in the array of longs
        /// </summary>
        //[ FieldOffset( 62 ) ]
        internal ushort clw;

        /// <summary>
        /// Array of longs
        /// </summary>
        internal ArrayOfLongs arrLongs = new ArrayOfLongs();

        /// <summary>
        /// Number of fields in the array of FC/LCB pairs.
        /// </summary>
        //[ FieldOffset( 152 ) ]
        internal ushort cfclcb;

        /// <summary>
        /// Array of FC/LCB.
        /// </summary>
        internal ArrayOfFCLCB arrFCLCB = new ArrayOfFCLCB();

        private ushort m_prevCsw;
        private ushort m_prevClw;
        #endregion
       
        #region Class properties
        /// <summary>
        /// Gets the size of the structure.
        /// </summary>
        /// <value>The length.</value>
        internal override int Length
        {
            get
            {
                return FIBHeader.DEF_HEADER_SIZE + arrShorts.BytesCount + Constants.BytesInWord
                  + arrLongs.BytesCount + Constants.BytesInWord + arrFCLCB.BytesCount;
            }
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="isDecryptedStream">if it is decrypted stream, set to <c>true</c>.</param>
        public void Parse(Stream stream, bool isDecryptedStream)
        {
            // Read header.
            byte[] arrBuffer = new byte[FIBHeader.DEF_HEADER_SIZE];
            stream.Read(arrBuffer, 0, FIBHeader.DEF_HEADER_SIZE);
#if AllowUnsafeCode && !SILVERLIGHT && !WP
            MemoryConverter.Instance.Copy(arrBuffer, header);
#else
      header.Parse( arrBuffer, 0 );
#endif

            // Read array of shorts.
            if (isDecryptedStream)
                header.csw = m_prevCsw;
            else
                m_prevCsw = header.csw;

            byte[] arrArrayBuffer = new byte[header.csw * DEF_SHORT_BYTES];
            stream.Read(arrArrayBuffer, 0, arrArrayBuffer.Length);
            arrShorts.SetBuffer(arrArrayBuffer);

            // Read size of the array of longs.
            stream.Read(arrBuffer, 0, 2);
            clw = BitConverter.ToUInt16(arrBuffer, 0);

            if (isDecryptedStream)
                clw = m_prevClw;
            else
                m_prevClw = clw;

            // Read array of longs.
            arrArrayBuffer = new byte[clw * DEF_LONG_BYTES];
            stream.Read(arrArrayBuffer, 0, arrArrayBuffer.Length);
            arrLongs.SetBuffer(arrArrayBuffer);

            // Read size of the array of FC/LCB.
            stream.Read(arrBuffer, 0, 2);
            cfclcb = BitConverter.ToUInt16(arrBuffer, 0);

            // Read array of FC/LCB.
            arrArrayBuffer = new byte[cfclcb * DEF_FCLCB_BYTES];
            stream.Read(arrArrayBuffer, 0, arrArrayBuffer.Length);
            arrFCLCB.SetBuffer(arrArrayBuffer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrData"></param>
        /// <param name="iOffset"></param>
        /// <param name="converter"></param>
        internal override int Save(byte[] arrData, int iOffset)
        {
#if AllowUnsafeCode && !SILVERLIGHT && !WP
            MemoryConverter.Instance.Copy(header, arrData, iOffset, FIBHeader.DEF_HEADER_SIZE);
#else
      header.Save( arrData, iOffset );
#endif

            int iLength = FIBHeader.DEF_HEADER_SIZE;
            iOffset += FIBHeader.DEF_HEADER_SIZE;

            int iPartSize = arrShorts.Save(arrData, iOffset);
            iOffset += iPartSize;
            iLength += iPartSize;

            BitConverter.GetBytes(clw).CopyTo(arrData, iOffset);
            iOffset += Constants.BytesInWord;
            iLength += Constants.BytesInWord;

            iPartSize = arrLongs.Save(arrData, iOffset);
            iOffset += iPartSize;
            iLength += iPartSize;

            BitConverter.GetBytes(cfclcb).CopyTo(arrData, iOffset);
            iOffset += Constants.BytesInWord;
            iLength += Constants.BytesInWord;

            iPartSize = arrFCLCB.Save(arrData, iOffset);
            iOffset += iPartSize;
            iLength += iPartSize;

            return iLength;
        }

        /// <summary>
        /// Parse the data strucure
        /// </summary>
        /// <param name="arrData">Bytes with data</param>
        /// <param name="iOffset">Offset</param>
        internal override void Parse(byte[] arrData, int iOffset)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

    }
}
#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using Syncfusion.CompoundFile.DocIO.Native;
using Syncfusion.CompoundFile;
using Syncfusion.CompoundFile.DocIO;
using System.IO;

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.OLEObject
{
    class LinkInfoStream : DataStructure
    {
        #region Constants
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_STRUCT_SIZE = 0;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_UNICODE_MARKER = -858997829;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_UNICODE_MARKER_SIZE = 4;
        #endregion

        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private byte[] m_filePathDataASCII;
        /// <summary>
        /// 
        /// </summary>
        private byte[] m_filePathDataUNICOD;
        /// <summary>
        /// 
        /// </summary>
        private string m_filePath;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the size of the structure.
        /// </summary>
        /// <value>The length.</value>
        internal override int Length
        {
            get
            {
                if (m_filePathDataASCII != null && m_filePathDataUNICOD != null)
                    return m_filePathDataASCII.Length + m_filePathDataUNICOD.Length + DEF_UNICODE_MARKER_SIZE + 8;
                else
                    return DEF_STRUCT_SIZE;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkInfoStream"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal LinkInfoStream(Stream stream)
        {
            Parse((stream as MemoryStream).ToArray(), 0);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkInfoStream"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        internal LinkInfoStream(string filePath)
        {
            m_filePath = filePath;
            Encoding ascEnc =
#if !SILVERLIGHT && !WP && !WINRT
            new ASCIIEncoding();
#else
            new UTF8Encoding();
#endif
            UnicodeEncoding unicEnc = new UnicodeEncoding();

            m_filePathDataASCII = ascEnc.GetBytes(m_filePath);
            m_filePathDataUNICOD = unicEnc.GetBytes(m_filePath);
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parse the data strucure
        /// </summary>
        /// <param name="arrData">Bytes with data</param>
        /// <param name="iOffset">Offset</param>
        internal override void Parse( byte[] arrData, int iOffset )
        {
            int ascCount = 0;

            for (int i = 0, leng = arrData.Length; i < leng; i++)
            {
                if (ascCount > 0)
                    ascCount -= 3;
                int x = ReadInt32(arrData, ref ascCount);
                if (x == DEF_UNICODE_MARKER)
                    break;
            }

            byte[] asciiData = new byte[ascCount - DEF_UNICODE_MARKER_SIZE];
            int start = 0;
            m_filePathDataASCII= ReadBytes(arrData, ascCount - DEF_UNICODE_MARKER_SIZE, ref start);

            start += DEF_UNICODE_MARKER_SIZE;

            int unicodeDataLeng = arrData.Length - asciiData.Length - DEF_UNICODE_MARKER_SIZE;
            byte[] unicodeData = new byte[unicodeDataLeng];
            m_filePathDataUNICOD = ReadBytes(arrData, unicodeDataLeng, ref start );
        }
        /// <summary>
        /// Saves the data structure.
        /// </summary>
        /// <param name="arrData">The destination array.</param>
        /// <param name="iOffset">The offset.</param>
        /// <returns>Length</returns>
        internal override int Save(byte[] arrData, int iOffset)
        {
            throw new NotImplementedException( "Not implemented" );
        }
        /// <summary>
        /// Saves the data to stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void SaveTo(Stream stream)
        {
            byte[] buffer = new byte[Length];
            int iOffset = 0;

            buffer[iOffset] = (byte)m_filePathDataASCII.Length;
            iOffset += 2;
            WriteBytes(buffer, ref iOffset, m_filePathDataASCII);
            iOffset += 2;
            WriteBytes(buffer, ref iOffset, BitConverter.GetBytes(DEF_UNICODE_MARKER));
            buffer[iOffset] = (byte)m_filePathDataASCII.Length;
            iOffset += 2;
            WriteBytes(buffer, ref iOffset, m_filePathDataUNICOD);

            stream.Write(buffer, 0, buffer.Length);
        }
        #endregion
    }
}


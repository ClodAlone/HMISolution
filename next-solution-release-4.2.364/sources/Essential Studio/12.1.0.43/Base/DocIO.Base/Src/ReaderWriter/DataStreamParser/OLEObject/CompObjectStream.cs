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


using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using Syncfusion.CompoundFile;
using Syncfusion.DocIO.DLS;
using Syncfusion.CompoundFile.DocIO;
using Syncfusion.CompoundFile.DocIO.Net;
using Syncfusion.CompoundFile.DocIO.Native;

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.OLEObject
{

    internal class CompObjectStream : DataStructure
    {
        #region Constants
        private const int DEF_STREAM_SIZE = 93;
        private const int DEF_MARKER_OR_LENGTH4 = 0x00000190;
        private const int DEF_MARKER_OR_LENGTH5 = 0x00000028;
        private const uint DEF_UNICODE_MARKER = 0x71B239F4;
        #endregion

        #region Fields
        /// <summary>
        /// Length of compObject stream
        /// </summary>
        private int m_streamLength;
        /// <summary>
        /// This MUST be a CompObjHeader structure.
        /// </summary>
        private CompObjHeader m_header;
        /// <summary>
        /// This MUST be a LengthPrefixedAnsiString structure that contains a display
        /// name of the linked object or embedded object
        /// </summary>
        private string m_ansiUserTypeData = string.Empty;
        /// <summary>
        /// This MUST be a ClipboardFormatOrAnsiString structure that contains the
        /// Clipboard Format of the linked object or embedded object. If the MarkerOrLength
        /// field of the ClipboardFormatOrAnsiString structure contains a value other than
        /// 0x00000000, 0xffffffff, or 0xfffffffe, the value MUST NOT be greater than 0x00000190.
        /// Otherwise the CompObjStream structure is invalid
        /// </summary>
        private string m_ansiClipboardFormatData = string.Empty;
        /// <summary>
        /// If present, this MUST be a LengthPrefixedAnsiString structure. If the Length field of
        /// the LengthPrefixedAnsiString contains a value of 0 or a value that is greater than
        /// 0x00000028, the remaining fields of the structure starting with the String field
        /// of the LengthPrefixedAnsiString MUST be ignored on processing.
        /// </summary>
        private string m_reserved1Data = string.Empty;
        /// <summary>
        /// If this field is present and is NOT set to 0x71B239F4,
        /// the remaining fields of the structure MUST be ignored on processing
        /// </summary>
        private uint m_unicodeMarker = 1907505652;
        /// <summary>
        /// This MUST be a LengthPrefixedUnicodeString structure that contains a display name
        /// of the linked object or embedded object.
        /// </summary>
        private string m_unicodeUserTypeData = string.Empty;
        /// <summary>
        /// This MUST be a ClipboardFormatOrUnicodeString structure that contains a Clipboard
        /// Format of the linked object or embedded object. If the MarkerOrLength field of the
        /// ClipboardFormatOrUnicodeString structure contains a value other than 0x00000000,
        /// 0xffffffff, or 0xfffffffe, the value MUST NOT be more than 0x00000190. Otherwise,
        /// the CompObjStream structure is invalid
        /// </summary>
        private string m_unicodeClipboardFormatData = string.Empty;
        /// <summary>
        /// This MUST be a LengthPrefixedUnicodeString. The String field of the LengthPrefixedUnicodeString
        /// can contain any arbitrary value and MUST be ignored on processing.
        /// </summary>
        private string m_reserved2Data = string.Empty;

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
                if (m_streamLength == 0)
                {
                    m_streamLength = DEF_STREAM_SIZE;
                }
                return m_streamLength;
            }
        }
        /// <summary>
        /// Gets the type of the object.
        /// </summary>
        /// <value>The type of the object.</value>
        internal string ObjectType
        {
            get
            {
                return m_ansiUserTypeData;
            }
        }
        /// <summary>
        /// Gets the type of the object.
        /// </summary>
        /// <value>The type of the object.</value>
        internal string ObjectTypeReserved
        {
            get
            {
                return m_reserved1Data;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectInfoStream"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal CompObjectStream(Stream stream)
        {
            Parse((stream as MemoryStream).ToArray(), 0);
        }
        /// <summary>
        /// Initializes a default instance of the <see cref="CompObjectStream"/> class.
        /// </summary>
        internal CompObjectStream(OleObjectType oleType)
        {
            m_header = new CompObjHeader();
            switch (oleType)
            {
                case OleObjectType.AdobeAcrobatDocument:
                    m_ansiUserTypeData = "Acrobat Document\0";
                    m_reserved1Data = "AcroExch.Document.7\0";
                    break;
                case OleObjectType.WaveSound:
                    m_ansiUserTypeData = "Wave Sound\0";
                    m_reserved1Data = "SoundRec\0";
                    break;
                case OleObjectType.Equation:
                case OleObjectType.MediaClip:
                case OleObjectType.BitmapImage:
                case OleObjectType.GraphChart:
                case OleObjectType.Excel_97_2003_Worksheet:
                case OleObjectType.ExcelBinaryWorksheet:
                case OleObjectType.ExcelChart:
                case OleObjectType.ExcelMacroWorksheet:
                case OleObjectType.ExcelWorksheet:
                case OleObjectType.PowerPoint_97_2003_Presentation:
                case OleObjectType.PowerPoint_97_2003_Slide:
                case OleObjectType.PowerPointMacroPresentation:
                case OleObjectType.PowerPointMacroSlide:
                case OleObjectType.PowerPointPresentation:
                case OleObjectType.PowerPointSlide:
                case OleObjectType.Word_97_2003_Document:
                case OleObjectType.WordDocument:
                case OleObjectType.WordMacroDocument:
                case OleObjectType.MIDISequence:
                case OleObjectType.VideoClip:
                case OleObjectType.Package:
                    m_ansiUserTypeData = OleTypeConvertor.ToString(oleType, true) + "\0";
                    break;
            }
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parse the data strucure
        /// </summary>
        /// <param name="arrData">Bytes with data</param>
        /// <param name="iOffset">Offset</param>
        internal override void Parse(byte[] arrData, int iOffset)
        {
            m_streamLength = arrData.Length;
            int dataLength = 0;

            Encoding asciiEnc =
#if !SILVERLIGHT && !WP && !WINRT
                new ASCIIEncoding();
#else
                new UTF8Encoding();
#endif
            UnicodeEncoding unicodeEnc = new UnicodeEncoding();
            
            // Parse Header
            m_header = new CompObjHeader();
            m_header.Parse( arrData, iOffset );
            iOffset += m_header.Length;

            // AnsiUserType
            if (arrData.Length > iOffset + Constants.BytesInInt)
                dataLength = ReadInt32(arrData, ref iOffset);
            if (dataLength > 0)
            {
                byte[] bytes = ReadBytes(arrData, dataLength, ref iOffset);
                m_ansiUserTypeData = asciiEnc.GetString(bytes, 0, bytes.Length);
            }

            // AnsiClipboardFormat
            uint uDataLength = 0;
            if (arrData.Length > iOffset + Constants.BytesInInt)
                uDataLength = ReadUInt32(arrData, ref iOffset);
            if (dataLength > 0)
            {
                if (uDataLength == 0xFFFFFFFF || uDataLength == 0xFFFFFFFE)
                {
                    byte[] bytes = ReadBytes(arrData, 4, ref iOffset);
                    m_ansiUserTypeData = asciiEnc.GetString(bytes, 0, bytes.Length);
                }
                else if (uDataLength > DEF_MARKER_OR_LENGTH4)
#if SILVERLIGHT || WP
                    throw new Exception("OLE stream is not valid");
#else
                    throw new InvalidDataException("OLE stream is not valid");
#endif
            }

            // Reserved1
            if (arrData.Length > iOffset + Constants.BytesInInt)
                dataLength = ReadInt32(arrData, ref iOffset);
            if (dataLength > 0 && dataLength <= DEF_MARKER_OR_LENGTH5)
            {
                byte[] bytes = ReadBytes(arrData, dataLength, ref iOffset);
                m_reserved1Data = asciiEnc.GetString(bytes, 0, bytes.Length);
            }

            // UnicodeMarker
            if (arrData.Length > iOffset + Constants.BytesInInt)
                m_unicodeMarker = ReadUInt32(arrData, ref iOffset);
            if (m_unicodeMarker == DEF_UNICODE_MARKER)
            {
                // UnicodeUserType
                if (arrData.Length > iOffset + Constants.BytesInInt)
                    dataLength = ReadInt32(arrData, ref iOffset);
                if (dataLength > 0)
                {
                    byte[] bytes = ReadBytes(arrData, dataLength, ref iOffset);
                    m_unicodeUserTypeData = unicodeEnc.GetString(bytes, 0, bytes.Length);
                }                

                // UnicodeClipboardFormat
                if (arrData.Length > iOffset + Constants.BytesInInt)
                    uDataLength = ReadUInt32(arrData, ref iOffset);
                if (uDataLength > 0)
                {
                    if (uDataLength == 0xFFFFFFFF || uDataLength == 0xFFFFFFFE)
                    {
                        byte[] bytes = ReadBytes(arrData, 4, ref iOffset);
                        m_unicodeClipboardFormatData = unicodeEnc.GetString(bytes, 0, bytes.Length);
                    }
                    else if (uDataLength > DEF_MARKER_OR_LENGTH4)
#if SILVERLIGHT || WP
                        throw new Exception("OLE stream is not valid");
#else
                        throw new InvalidDataException("OLE stream is not valid");
#endif
                }

                // Reserved2
                if (arrData.Length > iOffset + Constants.BytesInInt)
                    dataLength = ReadInt32(arrData, ref iOffset);
                if (dataLength > 0 && dataLength <= DEF_MARKER_OR_LENGTH5)
                {
                    byte[] bytes = ReadBytes(arrData, dataLength, ref iOffset);
                    m_reserved2Data = unicodeEnc.GetString(bytes, 0, bytes.Length);
                }
            }
        }
        /// <summary>
        /// Saves the data structure.
        /// </summary>
        /// <param name="arrData">The destination array.</param>
        /// <param name="iOffset">The offset.</param>
        /// <returns>Length</returns>
        internal override int Save(byte[] arrData, int iOffset)
        {
            throw new NotImplementedException("Not implemented");
        }
        /// <summary>
        /// Saves data to STG stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void SaveTo(Stream stream)
        {
            int bytesInInt = 4;
            // Write header
            m_header.SaveTo(stream);
            // AnsiUserType
            WriteLengthPrefixedString(stream, m_ansiUserTypeData);
            // AnsiClipboardFormat
            WriteLengthPrefixedString(stream, m_ansiClipboardFormatData);
            // Reserved1
            WriteLengthPrefixedString(stream, m_reserved1Data);
            // UnicodeMarker
            WriteZeroByteArr(stream, bytesInInt);
            // UnicodeUserType
            WriteZeroByteArr(stream, bytesInInt);
            // UnicodeClipboardFormat
            WriteZeroByteArr(stream, bytesInInt);
            // Reserved2
            WriteZeroByteArr(stream, bytesInInt);
        }
        /// <summary>
        /// Writes the zero byte array.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="byteLength">Length of the byte.</param>
        private void WriteZeroByteArr(Stream stream, int byteLength)
        {
            byte[] bytes = new byte[byteLength];
            stream.Write(bytes, 0, byteLength);
        }
        /// <summary>
        /// Writes the length prefixed string.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="data">The data.</param>
        private void WriteLengthPrefixedString(Stream stream, String data)
        {
            byte[] lengthBytes = new byte[4];
            Encoding enc =
#if !SILVERLIGHT && !WP && !WINRT
                new ASCIIEncoding();
#else
                new UTF8Encoding();
#endif
            int iOffset = 0;

            byte[] dataBytes = enc.GetBytes(data);
            WriteInt32(lengthBytes, ref iOffset, dataBytes.Length);
            stream.Write(lengthBytes, 0, lengthBytes.Length);

            if (dataBytes.Length > 0)
                stream.Write(dataBytes, 0, dataBytes.Length);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        private class CompObjHeader : DataStructure
        {
            #region Constants
            internal const int DEF_STRUCT_LEN = 28;
            internal const int DEF_RESERVED2_ARR_LEN = 20;
            #endregion

            #region Fields
            /// <summary>
            /// This can be set to any arbitrary value and MUST be ignored on processing.
            /// </summary>
            internal int m_reserved1;
            /// <summary>
            /// This can be set to any arbitrary value and MUST be ignored on processing.
            /// </summary>
            internal int m_version;
            /// <summary>
            /// This can be set to any arbitrary value and MUST be ignored on processing.
            /// </summary>
            internal byte[] m_reserved2;
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
                    return DEF_STRUCT_LEN;
                }
            }
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CompObjHeader"/> class.
            /// </summary>
            internal CompObjHeader()
            {
                m_reserved1 = -131071;
                m_version = 2563;
                m_reserved2 = new byte[DEF_RESERVED2_ARR_LEN];
            }
            #endregion

            #region Helper methods
            /// <summary>
            /// Parse the data strucure
            /// </summary>
            /// <param name="arrData">Bytes with data</param>
            /// <param name="iOffset">Offset</param>
            internal override void Parse(byte[] arrData, int iOffset)
            {
                m_reserved1 = ReadInt32(arrData, ref iOffset);
                m_version = ReadInt32(arrData, ref iOffset);
                if (arrData.Length > iOffset)
                    m_reserved2 = ReadBytes(arrData, 20, ref iOffset);
            }
            /// <summary>
            /// Saves the data structure.
            /// </summary>
            /// <param name="arrData">The destination array.</param>
            /// <param name="iOffset">The offset.</param>
            /// <returns>Length</returns>
            internal override int Save(byte[] arrData, int iOffset)
            {
                WriteInt32(arrData, ref iOffset, -131071);
                WriteInt32(arrData, ref iOffset, 2563);
                m_reserved2 = new byte[20] { 255, 255, 255, 255,
                101, 202, 1, 184, 
                252, 161, 208, 17,
                133, 173, 68, 69, 
                83, 84, 0, 0};
                WriteBytes(arrData, ref iOffset, m_reserved2);

                return iOffset;
            }
            /// <summary>
            /// Saves to STG stream.
            /// </summary>
            /// <param name="stream">The stream.</param>
            internal void SaveTo(Stream stream)
            {
                int bytesInInt32 = 4;
                byte[] bytes = BitConverter.GetBytes(m_reserved1);
                stream.Write(bytes, 0, bytesInInt32);
                bytes = BitConverter.GetBytes(m_version);
                stream.Write(bytes, 0, bytesInInt32);
                if (m_reserved2 == null)
                    stream.Write(new byte[DEF_RESERVED2_ARR_LEN], 0, DEF_RESERVED2_ARR_LEN);
                else
                    stream.Write(m_reserved2, 0, DEF_RESERVED2_ARR_LEN);
            }
            #endregion
        }
    }
}

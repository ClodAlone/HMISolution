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

#if !SILVERLIGHT

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Syncfusion.CompoundFile;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.CompoundFile.XlsIO;
using Syncfusion.CompoundFile.XlsIO.Net;
using Syncfusion.CompoundFile.XlsIO.Native;

namespace Syncfusion.XlsIO.Implementation
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
        if( m_streamLength == 0 )
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
    /// <param name="compStream">The comp stream.</param>
    internal CompObjectStream( CompoundStream compStream )
    {
      byte[] bytes = new byte[ compStream.Length ];
      compStream.Read( bytes, 0, bytes.Length );
      Parse( bytes, 0 );
    }
    /// <summary>
    /// Initializes a default instance of the <see cref="CompObjectStream"/> class.
    /// </summary>
    internal CompObjectStream()
    {
      m_header = new CompObjHeader();
      m_ansiUserTypeData = OleTypeConvertor.OleType + "\0";
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
      m_streamLength = arrData.Length;
      int dataLength = 0;

      ASCIIEncoding asciiEnc = new ASCIIEncoding();
      UnicodeEncoding unicodeEnc = new UnicodeEncoding();

      // Parse Header
      m_header = new CompObjHeader();
      m_header.Parse( arrData, iOffset );
      iOffset += m_header.Length;

      // AnsiUserType
      dataLength = ReadInt32( arrData, ref iOffset );
      if( dataLength > 0 )
      {
        byte[] bytes = ReadBytes( arrData, dataLength, ref iOffset );
        m_ansiUserTypeData = asciiEnc.GetString( bytes );
      }

      // AnsiClipboardFormat
      uint uDataLength = ReadUInt32( arrData, ref iOffset );
      if( dataLength > 0 )
      {
        if( uDataLength == 0xFFFFFFFF || uDataLength == 0xFFFFFFFE )
        {
          byte[] bytes = ReadBytes( arrData, 4, ref iOffset );
          m_ansiUserTypeData = asciiEnc.GetString( bytes );
        }
        else if( uDataLength > DEF_MARKER_OR_LENGTH4 )
          throw new InvalidDataException( "OLE stream in not valid" );
      }

      // Reserved1
      dataLength = ReadInt32( arrData, ref iOffset );
      if( dataLength > 0 && dataLength <= DEF_MARKER_OR_LENGTH5 )
      {
        byte[] bytes = ReadBytes( arrData, dataLength, ref iOffset );
        m_reserved1Data = asciiEnc.GetString( bytes );
      }

      // UnicodeMarker
      m_unicodeMarker = ReadUInt32( arrData, ref iOffset );
      if( m_unicodeMarker == DEF_UNICODE_MARKER )
      {
        // UnicodeUserType
        dataLength = ReadInt32( arrData, ref iOffset );
        if( dataLength > 0 )
        {
          byte[] bytes = ReadBytes( arrData, dataLength, ref iOffset );
          m_unicodeUserTypeData = unicodeEnc.GetString( bytes );
        }

        // UnicodeClipboardFormat
        uDataLength = ReadUInt32( arrData, ref iOffset );
        if( uDataLength > 0 )
        {
          if( uDataLength == 0xFFFFFFFF || uDataLength == 0xFFFFFFFE )
          {
            byte[] bytes = ReadBytes( arrData, 4, ref iOffset );
            m_unicodeClipboardFormatData = unicodeEnc.GetString( bytes );
          }
          else if( uDataLength > DEF_MARKER_OR_LENGTH4 )
            throw new InvalidDataException( "OLE stream in not valid" );
        }

        // Reserved2
        dataLength = ReadInt32( arrData, ref iOffset );
        if( dataLength > 0 && dataLength <= DEF_MARKER_OR_LENGTH5 )
        {
          byte[] bytes = ReadBytes( arrData, dataLength, ref iOffset );
          m_reserved2Data = unicodeEnc.GetString( bytes );
        }
      }
    }
    /// <summary>
    /// Saves the data structure.
    /// </summary>
    /// <param name="arrData">The destination array.</param>
    /// <param name="iOffset">The offset.</param>
    /// <returns>Length</returns>
    internal override int Save( byte[] arrData, int iOffset )
    {
      throw new NotImplementedException( "Not implemented" );
    }
    /// <summary>
    /// Saves data to STG stream.
    /// </summary>
    /// <param name="stgStream">The STG stream.</param>
    internal void SaveTo( StgStream stgStream )
    {
      int bytesInInt = 4;
      // Write header
      m_header.SaveTo( stgStream );
      // AnsiUserType
      WriteLengthPrefixedString( stgStream, m_ansiUserTypeData );
      // AnsiClipboardFormat
      WriteLengthPrefixedString( stgStream, m_ansiClipboardFormatData );
      // Reserved1
      WriteLengthPrefixedString( stgStream, m_reserved1Data );
      // UnicodeMarker
      WriteZeroByteArr( stgStream, bytesInInt );
      // UnicodeUserType
      WriteZeroByteArr( stgStream, bytesInInt );
      // UnicodeClipboardFormat
      WriteZeroByteArr( stgStream, bytesInInt );
      // Reserved2
      WriteZeroByteArr( stgStream, bytesInInt );
    }
    /// <summary>
    /// Writes the zero byte array.
    /// </summary>
    /// <param name="stgStream">The STG stream.</param>
    /// <param name="byteLength">Length of the byte.</param>
    private void WriteZeroByteArr( StgStream stgStream, int byteLength )
    {
      byte[] bytes = new byte[ byteLength ];
      stgStream.Write( bytes, 0, byteLength );
    }
    /// <summary>
    /// Writes the length prefixed string.
    /// </summary>
    /// <param name="stgStream">The STG stream.</param>
    /// <param name="data">The data.</param>
    private void WriteLengthPrefixedString( StgStream stgStream, String data )
    {
      byte[] lengthBytes = new byte[ 4 ];
      ASCIIEncoding enc = new ASCIIEncoding();
      int iOffset = 0;

      byte[] dataBytes = enc.GetBytes( data );
      WriteInt32( lengthBytes, ref iOffset, dataBytes.Length );
      stgStream.Write( lengthBytes, 0, lengthBytes.Length );

      if( dataBytes.Length > 0 )
        stgStream.Write( dataBytes, 0, dataBytes.Length );
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
        m_reserved2 = new byte[ DEF_RESERVED2_ARR_LEN ];
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
        m_reserved1 = ReadInt32( arrData, ref iOffset );
        m_version = ReadInt32( arrData, ref iOffset );
        m_reserved2 = ReadBytes( arrData, 20, ref iOffset );
      }
      /// <summary>
      /// Saves the data structure.
      /// </summary>
      /// <param name="arrData">The destination array.</param>
      /// <param name="iOffset">The offset.</param>
      /// <returns>Length</returns>
      internal override int Save( byte[] arrData, int iOffset )
      {
        WriteInt32( arrData, ref iOffset, -131071 );
        WriteInt32( arrData, ref iOffset, 2563 );
        m_reserved2 = new byte[ 20 ] { 255, 255, 255, 255,
                101, 202, 1, 184, 
                252, 161, 208, 17,
                133, 173, 68, 69, 
                83, 84, 0, 0};
        WriteBytes( arrData, ref iOffset, m_reserved2 );

        return iOffset;
      }
      /// <summary>
      /// Saves to STG stream.
      /// </summary>
      /// <param name="stgStream">The STG stream.</param>
      internal void SaveTo( StgStream stgStream )
      {
        byte[] bytes = BitConverter.GetBytes( m_reserved1 );
        stgStream.Write( bytes, 0, ExcelConstants.IntSize );

        bytes = BitConverter.GetBytes( m_version );
        stgStream.Write( bytes, 0, ExcelConstants.IntSize );

        if( m_reserved2 == null )
        {
          stgStream.Write( new byte[ DEF_RESERVED2_ARR_LEN ], 0, DEF_RESERVED2_ARR_LEN );
        }
        else
        {
          stgStream.Write( m_reserved2, 0, DEF_RESERVED2_ARR_LEN );
        }
      }
      #endregion
    }
  }
}

#endif

#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;


using Syncfusion.CompoundFile;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.CompoundFile.XlsIO.Native;
using Syncfusion.CompoundFile.XlsIO;

namespace Syncfusion.XlsIO.Implementation
{
  internal class OleStream : DataStructure
  {
    #region Constants
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_VERSION_CONSTANT = 0x02000001;
    private const int DEF_RESERVED_VALUE = 0x00000000;
    private const int DEF_EMBEDDED_SIZE = 20;
    private const int DEF_CLSID_INDICATOR = -1;
    private const int DEF_EMBED_FLAG = 8;
    private const int DEF_LINK_FLAG = 1;

    #endregion

    #region Fields
    /// <summary>
    /// 
    /// </summary>
    private int m_streamLeng;
    /// <summary>
    /// 
    /// </summary>
    private int m_oleVersion;
    /// <summary>
    /// This MUST be set to 0x00000001 or 0x00000002. If this field has a value of 1, the OLEStream
    /// structure MUST be for a linked object. If this field has a value of 0, then the OLEStream
    /// structure MUST be for an embedded object.
    /// </summary>
    private int m_flags;
    /// <summary>
    /// This field contains an implementation-specific hint supplied by the application or by a
    /// higher-level protocol that creates the data structure. The hint MAY be ignored on
    /// processing of this data structure.
    /// </summary>
    private int m_linkUpdateOption;
    /// <summary>
    /// This MUST be set to 0x00000000. Otherwise, the OLEStream structure is invalid.
    /// </summary>
    private int m_reserved1;
    /// <summary>
    /// This MUST be set to the size, in bytes, of the ReservedMonikerStream field.
    /// If this field has a value 0x00000000, the ReservedMonikerStream field MUST NOT be present.
    /// </summary>
    private int m_reservedMonikerStreamSize;
    /// <summary>
    /// This MUST be a MONIKERSTREAM structure that can contain any arbitrary value 
    /// and MUST be ignored on processing.
    /// </summary>
    private MonokerStream m_reservedMonikerStream;
    /// <summary>
    /// This MUST be set to the size, in bytes, of the RelativeSourceMonikerStream field.
    /// If this field has a value 0x00000000, the RelativeSourceMonikerStream field MUST NOT be present.
    /// </summary>
    private int m_relativeSourceMonikerStreamSize;
    /// <summary>
    /// This MUST be a MONIKERSTREAM structure that specifies the relative path to the linked object.
    /// </summary>
    private MonokerStream m_relativeSourceMonikerStream;
    /// <summary>
    /// This MUST be set to the size, in bytes, of the AbsoluteSourceMonikerStream field. 
    /// This field MUST NOT contain the value 0x00000000.
    /// </summary>
    private int m_absoluteSourceMonikerStreamSize;
    /// <summary>
    /// This MUST be a MONIKERSTREAM structure that specifies the full path to the linked object.
    /// </summary>
    private MonokerStream m_absoluteSourceMonikerStream;
    /// <summary>
    /// This MUST be the LONG value -1
    /// </summary>
    private int m_clsidIndicator;
    /// <summary>
    /// This MUST be the CLSID containing the object class GUID of the creating application.
    /// </summary>
    private CLSID m_clsid;
    /// <summary>
    /// This MUST be a LengthPrefixedUnicodeString that can contain 
    /// any arbitrary value and MUST be ignored on processing.
    /// </summary>
    private int m_reservedDisplayName;
    /// <summary>
    /// This can contain any arbitrary value and MUST be ignored on processing.
    /// </summary>
    private int m_reserved2;
    /// <summary>
    /// This MUST be a FILETIME that contains the time 
    /// when the container application last updated the RemoteUpdateTime field.
    /// </summary>
    private int m_localUpdateTime;
    /// <summary>
    /// This MUST be a FILETIME that contains the time 
    /// when the container application last checked the update time of the linked object.
    /// </summary>
    private int m_localCheckUpdateTime;
    /// <summary>
    /// This MUST be a FILETIME that contains the time when the linked object was last updated.
    /// </summary>
    private int m_remoteUpdateTime;
    /// <summary>
    /// The type of the link;
    /// </summary>

    /// <summary>
    /// Path to linked file
    /// </summary>
    private string m_filePath = string.Empty;
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
        if( m_streamLeng == 0 )
        {
          m_streamLeng = DEF_EMBEDDED_SIZE;
        }
        return m_streamLeng;
      }
    }
    #endregion

    #region Methods

    /// <summary>
    /// Initializes a new instance of the <see cref="OLEStream"/> class.
    /// </summary>
    /// <param name="compStream">The comp stream.</param>
    internal OleStream( CompoundStream compStream )
    {
      byte[] bytes = new byte[ compStream.Length ];
      compStream.Read( bytes, 0, bytes.Length );
      Parse( bytes, 0 );
    }
    /// <summary>
    /// Initializes a default instance of the <see cref="OLEStream"/> class.
    /// </summary>
    internal OleStream( string filePath )
    {

      m_oleVersion = DEF_VERSION_CONSTANT;
      m_reserved1 = DEF_RESERVED_VALUE;
      m_reservedMonikerStreamSize = 0;

      m_filePath = filePath;
      // update link method: manual - 3, auto - 1
      m_linkUpdateOption = 3;
    }
    /// <summary>
    /// Parse the data strucure
    /// </summary>
    /// <param name="arrData">Bytes with data</param>
    /// <param name="iOffset">Offset</param>
    internal override void Parse( byte[] arrData, int iOffset )
    {
      m_streamLeng = arrData.Length;

      m_oleVersion = ReadInt32( arrData, ref iOffset );
      if( m_oleVersion != DEF_VERSION_CONSTANT )
        throw new Exception( "OLE stream in not valid" );

      m_flags = ReadInt32( arrData, ref iOffset );

      m_linkUpdateOption = ReadInt32( arrData, ref iOffset );

      m_reserved1 = ReadInt32( arrData, ref iOffset );
      if( m_reserved1 != DEF_RESERVED_VALUE )
        throw new InvalidDataException( "OLE stream in not valid" );

      if( m_flags != 0 && m_flags != 8 )
      {
        m_reservedMonikerStreamSize = ReadInt32( arrData, ref iOffset );
        if( m_reservedMonikerStreamSize != 0 )
        {
          byte[] data = ReadBytes( arrData, m_reservedMonikerStreamSize, ref iOffset );
          m_reservedMonikerStream = new MonokerStream( m_filePath );
          m_reservedMonikerStream.Parse( data, 0 );
        }

        m_relativeSourceMonikerStreamSize = ReadInt32( arrData, ref iOffset );
        if( m_relativeSourceMonikerStreamSize != 0 )
        {
          byte[] data = ReadBytes( arrData, m_relativeSourceMonikerStreamSize, ref iOffset );
          m_relativeSourceMonikerStream = new MonokerStream( m_filePath );
          m_relativeSourceMonikerStream.Parse( data, 0 );
        }

        m_absoluteSourceMonikerStreamSize = ReadInt32( arrData, ref iOffset );
        byte[] absSourcMonStr = ReadBytes( arrData, m_absoluteSourceMonikerStreamSize, ref iOffset );
        m_absoluteSourceMonikerStream = new MonokerStream( m_filePath );
        m_absoluteSourceMonikerStream.Parse( absSourcMonStr, 0 );

        m_clsidIndicator = ReadInt32( arrData, ref iOffset );
        if( m_clsidIndicator == DEF_CLSID_INDICATOR )
          throw new InvalidDataException( "OLE stream in not valid" );

        byte[] clsidData = ReadBytes( arrData, 16, ref iOffset );
        m_clsid = new CLSID();
        m_clsid.Parse( clsidData, 0 );

        m_reservedDisplayName = ReadInt32( arrData, ref iOffset );
        m_reserved2 = ReadInt32( arrData, ref iOffset );
        m_localUpdateTime = ReadInt32( arrData, ref iOffset );
        m_localCheckUpdateTime = ReadInt32( arrData, ref iOffset );
        m_remoteUpdateTime = ReadInt32( arrData, ref iOffset );
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
      WriteInt32( arrData, ref iOffset, m_oleVersion );
      WriteInt32( arrData, ref iOffset, m_flags );
      WriteInt32( arrData, ref iOffset, m_linkUpdateOption );
      WriteInt32( arrData, ref iOffset, m_reserved1 );

      if( m_flags == 0 )
        return arrData.Length;

      // Writer linked ole object data
      WriteInt32( arrData, ref iOffset, m_reservedMonikerStreamSize );

      if( m_reservedMonikerStreamSize != 0 )
      {
        m_reservedMonikerStream.Save( arrData, iOffset );
        iOffset += m_reservedMonikerStream.Length;
      }
      WriteInt32( arrData, ref iOffset, m_relativeSourceMonikerStreamSize );

      if( m_relativeSourceMonikerStreamSize != 0 )
      {
        m_relativeSourceMonikerStream.Save( arrData, iOffset );
        iOffset += m_relativeSourceMonikerStream.Length;
      }

      WriteInt32( arrData, ref iOffset, m_absoluteSourceMonikerStreamSize );
      m_absoluteSourceMonikerStream.Save( arrData, iOffset );
      iOffset += m_absoluteSourceMonikerStream.Length;

      WriteInt32( arrData, ref iOffset, m_clsidIndicator );

      m_clsid.Save( arrData, iOffset );
      iOffset += m_clsid.Length;

      WriteInt32( arrData, ref iOffset, m_reservedDisplayName );
      WriteInt32( arrData, ref iOffset, m_reserved2 );
      WriteInt32( arrData, ref iOffset, m_localUpdateTime );
      WriteInt32( arrData, ref iOffset, m_localCheckUpdateTime );
      WriteInt32( arrData, ref iOffset, m_remoteUpdateTime );

      return arrData.Length;
    }
    /// <summary>
    /// Saves to compound stream.
    /// </summary>
    /// <param name="compStream">The comp stream.</param>
    internal void SaveTo( CompoundStream compStream )
    {
      int iOffset = 0;
      byte[] tempBytes = new byte[ DEF_EMBEDDED_SIZE ];

      WriteInt32( tempBytes, ref iOffset, m_oleVersion );
      WriteInt32( tempBytes, ref iOffset, m_flags );
      WriteInt32( tempBytes, ref iOffset, m_linkUpdateOption );
      WriteInt32( tempBytes, ref iOffset, m_reserved1 );
      WriteInt32( tempBytes, ref iOffset, m_reservedMonikerStreamSize );
      compStream.Write( tempBytes, 0, tempBytes.Length );

    }
    /// <summary>
    /// Saves to stg stream.
    /// </summary>
    /// <param name="compStream">The comp stream.</param>
    internal void SaveTo( StgStream stgStream )
    {
      int iOffset = 0;
      byte[] tempBytes = new byte[ DEF_EMBEDDED_SIZE ];
      WriteInt32( tempBytes, ref iOffset, m_oleVersion );
      WriteInt32( tempBytes, ref iOffset, m_flags );
      WriteInt32( tempBytes, ref iOffset, m_linkUpdateOption );
      WriteInt32( tempBytes, ref iOffset, m_reserved1 );
      WriteInt32( tempBytes, ref iOffset, m_reservedMonikerStreamSize );
      stgStream.Write( tempBytes, 0, tempBytes.Length );
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

    private class MonokerStream : DataStructure
    {
      #region Constants
      private const int DEF_STRUCT_LEN = 0;
      private const int DEF_UNICODE_MARKER = -559022081;
      private const int DEF_UNICODE_MARKER_SIZE = 4;
      #endregion

      #region Fields
      /// <summary>
      /// 
      /// </summary>
      internal CLSID m_Clsid;
      /// <summary>
      /// 
      /// </summary>
      internal byte[] m_streamData;
      /// <summary>
      /// Path to file
      /// </summary>
      internal string m_stringData;
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
          if( m_streamData != null )
            return m_streamData.Length + CLSID.DEF_STRUCT_LEN;
          else
            return DEF_STRUCT_LEN;
        }
      }
      #endregion

      #region Constructor
      /// <summary>
      /// Initializes a new instance of the <see cref="MonokerStream"/> class.
      /// </summary>
      internal MonokerStream( string data )
      {
        m_stringData = data;
      }
      #endregion

      #region Public methods
      /// <summary>
      /// Parse the data strucure
      /// </summary>
      /// <param name="arrData">Bytes with data</param>
      /// <param name="iOffset">Offset</param>
      internal override void Parse( byte[] arrData, int iOffset )
      {
        m_Clsid = new CLSID();
        m_Clsid.Parse( arrData, iOffset );
        iOffset += m_Clsid.Length;

        int streamDataLeng = arrData.Length - m_Clsid.Length;
        m_streamData = ReadBytes( arrData, streamDataLeng, ref iOffset );

        //int ascCount = 0;
        //for (int i = 0, leng = m_streamData.Length; i < leng; i++)
        //{
        //    if (ascCount > 0)
        //        ascCount -= 3;
        //    int x = ReadInt32(m_streamData, ref ascCount);
        //    if (x == DEF_UNICODE_MARKER)
        //        break;
        //}

        //byte[] asciiData = new byte[ascCount - DEF_UNICODE_MARKER_SIZE];
        //int start = 0;
        //byte[] asasd = ReadBytes(m_streamData, ascCount - DEF_UNICODE_MARKER_SIZE, ref start);
        //string asdasdfasdfasd = (new ASCIIEncoding()).GetString(asasd);

        //start += DEF_UNICODE_MARKER_SIZE;

        //int unicodeDataLeng = m_streamData.Length - asasd.Length - DEF_UNICODE_MARKER_SIZE;
        //byte[] unicodeData = ReadBytes(m_streamData, unicodeDataLeng, ref start);
        //string asdasdfasdfasd1 = (new UnicodeEncoding()).GetString(unicodeData);
        //char[] saddghdfg = asdasdfasdfasd1.ToCharArray();
      }
      /// <summary>
      /// Saves the data structure.
      /// </summary>
      /// <param name="arrData">The destination array.</param>
      /// <param name="iOffset">The offset.</param>
      /// <returns>Length</returns>
      internal override int Save( byte[] arrData, int iOffset )
      {
        m_Clsid.Save( arrData, iOffset );
        iOffset += m_Clsid.Length;

        WriteBytes( arrData, ref iOffset, m_streamData );
        return DEF_STRUCT_LEN;
      }
      /// <summary>
      /// Gets the short name of the path.
      /// </summary>
      /// <param name="path">The path.</param>
      /// <param name="shortPath">The short path.</param>
      /// <param name="shortPathLength">Short length of the path.</param>
      /// <returns></returns>
      [DllImport( "kernel32.dll", CharSet = CharSet.Auto )]
      public static extern int GetShortPathName(
          [MarshalAs( UnmanagedType.LPTStr )] string path,
          [MarshalAs( UnmanagedType.LPTStr )] StringBuilder shortPath,
          int shortPathLength );
      /// <summary>
      /// Save data structure.
      /// </summary>
      /// <param name="stgStream">The STG stream.</param>
      internal void SaveTo( StgStream stgStream, bool isAbsolute )
      {
        m_Clsid = new CLSID();
        byte[] clsidBytes = new byte[ CLSID.DEF_STRUCT_LEN ];
        m_Clsid.Save( clsidBytes, 0 );

        ASCIIEncoding asciiEnc = new ASCIIEncoding();
        UnicodeEncoding unicodeEnc = new UnicodeEncoding();

        string asciiData = string.Empty;
        string unicodeData = string.Empty;

        if( isAbsolute )
        {
          StringBuilder strBuilder = new StringBuilder( 255 );
          GetShortPathName( m_stringData, strBuilder, strBuilder.Capacity );
          string file = strBuilder.ToString();
          asciiData = file;
          unicodeData = file;
        }
        else
        {
          asciiData = m_stringData;
          unicodeData = m_stringData;
        }
        byte[] asciiBytes = asciiEnc.GetBytes( asciiData );
        byte[] markerBytes = BitConverter.GetBytes( DEF_UNICODE_MARKER );
        byte[] unicodeBytes = unicodeEnc.GetBytes( unicodeData );

        int monokerStrSize = clsidBytes.Length + asciiBytes.Length
            + markerBytes.Length + unicodeBytes.Length;
        int bytesInInt32 = 4;

        stgStream.Write( BitConverter.GetBytes( monokerStrSize ), 0, bytesInInt32 );
        stgStream.Write( clsidBytes, 0, clsidBytes.Length );
        stgStream.Write( asciiBytes, 0, asciiBytes.Length );
        stgStream.Write( markerBytes, 0, markerBytes.Length );
        stgStream.Write( unicodeBytes, 0, unicodeBytes.Length );
      }
      #endregion
    }

    private class CLSID : DataStructure
    {
      #region Constants
      /// <summary>
      /// 
      /// </summary>
      internal const int DEF_STRUCT_LEN = 16;
      #endregion

      #region Fields
      /// <summary>
      /// 
      /// </summary>
      internal int m_data1;
      /// <summary>
      /// 
      /// </summary>
      internal short m_data2;
      /// <summary>
      /// 
      /// </summary>
      internal short m_data3;
      /// <summary>
      /// 
      /// </summary>
      internal long m_data4;
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

      #region Constructors
      /// <summary>
      /// Initializes a new instance of the <see cref="CLSID"/> class.
      /// </summary>
      internal CLSID()
      {
      }
      #endregion

      #region Public methods
      /// <summary>
      /// Parse the data strucure
      /// </summary>
      /// <param name="arrData">Bytes with data</param>
      /// <param name="iOffset">Offset</param>
      internal override void Parse( byte[] arrData, int iOffset )
      {
        m_data1 = ReadInt32( arrData, ref iOffset );
        m_data2 = ReadInt16( arrData, ref iOffset );
        m_data3 = ReadInt16( arrData, ref iOffset );
        m_data4 = ReadInt64( arrData, ref iOffset );
      }
      /// <summary>
      /// Saves the data structure.
      /// </summary>
      /// <param name="arrData">The destination array.</param>
      /// <param name="iOffset">The offset.</param>
      /// <returns>Length</returns>
      internal override int Save( byte[] arrData, int iOffset )
      {
        WriteInt32( arrData, ref iOffset, m_data1 );
        WriteInt16( arrData, ref iOffset, m_data2 );
        WriteInt16( arrData, ref iOffset, m_data3 );
        WriteInt64( arrData, ref iOffset, m_data4 );

        return DEF_STRUCT_LEN;
      }
      #endregion
    }
  }
}


#endif

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
using System.IO;

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO.Net
#else
namespace Syncfusion.CompoundFile.XlsIO.Net
#endif
{
  /// <summary>
  /// Document properties enumerator.
  /// </summary>
  public class DocumentPropertyCollection
  {
    #region Constants
    private const int ByteOrder = 0xFFFE;
    private static readonly Guid FirstSectionGuid = new Guid( "f29f85e0-4ff9-1068-ab91-08002b27b3d9" );
    #endregion

    #region Members
    /// <summary>
    /// Offset to the first section.
    /// </summary>
    private int m_iFirstSectionOffset = -1;
    /// <summary>
    /// List of all sections.
    /// </summary>
    List<PropertySection> m_lstSections = new List<PropertySection>();
    #endregion

    #region Properties
    /// <summary>
    /// Returns list lf all sections.
    /// </summary>
    public List<PropertySection> Sections
    {
      get
      {
        return m_lstSections;
      }
    }

    #endregion

    #region Methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public DocumentPropertyCollection()
    {
    }
    /// <summary>
    /// Initializes new instance of the enumerator.
    /// </summary>
    /// <param name="stream">Stream to parse.</param>
    public DocumentPropertyCollection( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      stream.Position = 0;
      ReadHeader( stream );
      ParseSections( stream );
    }
    /// <summary>
    /// Extracts sections data from the stream.
    /// </summary>
    /// <param name="stream"></param>
    private void ParseSections( Stream stream )
    {
      for( int i = 0, len = m_lstSections.Count; i < len; i++ )
      {
        PropertySection section = m_lstSections[ i ];
        section.Parse( stream );
      }
    }
    /// <summary>
    /// Extracts header information from the stream.
    /// </summary>
    /// <param name="stream"></param>
    private void ReadHeader( Stream stream )
    {
      byte[] buffer = new byte[ 16 ];

      stream.Read( buffer, 0, 4 );
      int iValue = BitConverter.ToInt32( buffer, 0 );

      if( iValue != ByteOrder )
        throw new IOException( string.Format( "iValue = {0} instead of {1}", iValue, ByteOrder ) );

      // OS Version
      stream.Read( buffer, 0, 2 );

      // Some ID, probably OS indicator.
      stream.Read( buffer, 0, 2 );

      // Stream GUID = 0.
      stream.Read( buffer, 0, 16 );

      // Section count, must be 1.
      stream.Read( buffer, 0, 4 );
      int iSectionCount = BitConverter.ToInt32( buffer, 0 );

      //if( iValue != 1 )
      //  throw new IOException();

      // Section GUID f29f85e0-4ff9-1068-ab91-08002b27b3d9
      for( int i = 0; i < iSectionCount; i++ )
      {
        stream.Read( buffer, 0, 16 );
        Guid guid = new Guid( buffer );
        //if( guid.CompareTo( FirstSectionGuid ) != 0 )
        //  throw new IOException();

        int iSectionOffset = StreamHelper.ReadInt32( stream, buffer );
        m_lstSections.Add( new PropertySection( guid, iSectionOffset ) );
      }
    }
    /// <summary>
    /// Saves all sections into stream.
    /// </summary>
    /// <param name="stream">Stream to save sections into.</param>
    private void WriteSections( Stream stream )
    {
      for( int i = 0, len = m_lstSections.Count; i < len; i++ )
      {
        m_lstSections[ i ].Serialize( stream );
      }
    }
    /// <summary>
    /// Extracts header information from the stream.
    /// </summary>
    /// <param name="stream"></param>
    private void WriteHeader( Stream stream )
    {
      byte[] buffer = new byte[ 16 ];

      StreamHelper.WriteInt32( stream, ByteOrder );

      // OS Version
      StreamHelper.WriteInt16( stream, 0x105 );

      // Some ID, probably OS indicator.
      StreamHelper.WriteInt16( stream, 2 );

      // Stream GUID = 0.
      for( int i = 0; i < 16; i++ )
      {
        stream.WriteByte( 0 );
      }

      // Section count, must be 1.
      int iSectionCount = m_lstSections.Count;
      StreamHelper.WriteInt32( stream, iSectionCount );
      List<long> lstOffsets = new List<long>();

      // Section GUID f29f85e0-4ff9-1068-ab91-08002b27b3d9
      for( int i = 0; i < iSectionCount; i++ )
      {
        PropertySection section = m_lstSections[ i ];
        byte[] guid = section.Id.ToByteArray();
        stream.Write( guid, 0, guid.Length );

        //Reserve space for offset.
        lstOffsets.Add( stream.Position );
        StreamHelper.WriteInt32( stream, 0 );
        //int iSectionOffset = StreamHelper.ReadInt32( stream, buffer );
        //m_lstSections.Add( new PropertySection( guid, iSectionOffset ) );
      }

      for( int i = 0; i < iSectionCount; i++ )
      {
        PropertySection section = m_lstSections[ i ];

        // Write offset to the section at correct place.
        long currentPos = stream.Position;
        stream.Position = lstOffsets[ i ];
        StreamHelper.WriteInt32( stream, ( int )currentPos );
        stream.Position = currentPos;

        section.Serialize( stream );
      }
    }
    /// <summary>
    /// Saves collection into stream.
    /// </summary>
    /// <param name="stream"></param>
    public void Serialize( Stream stream )
    {
      WriteHeader( stream );
      //WriteSections( stream );
    }
    #endregion
  }
}

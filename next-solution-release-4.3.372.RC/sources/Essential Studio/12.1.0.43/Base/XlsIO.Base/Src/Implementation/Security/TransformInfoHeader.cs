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

namespace Syncfusion.XlsIO.Implementation.Security
{
  class TransformInfoHeader
  {
    #region Members
    /// <summary>
    /// An unsigned integer that specifies the type of transform to be applied.
    /// </summary>
    private int m_iTransformType = 1;
    /// <summary>
    /// An identifier associated with a specific transform.
    /// </summary>
    private string m_strTransformId;
    /// <summary>
    /// The friendly name of the transform.
    /// </summary>
    private string m_strTransformName;
    /// <summary>
    /// The reader version.
    /// </summary>
    private int m_iReaderVersion = 1;
    /// <summary>
    /// The updater version.
    /// </summary>
    private int m_iUpdaterVersion = 1;
    /// <summary>
    /// The writer version.
    /// </summary>
    private int m_iWriterVersion = 1;
    #endregion

    #region Properties
    /// <summary>
    /// An unsigned integer that specifies the type of transform to be applied.
    /// </summary>
    public int TransformType
    {
      get
      {
        return m_iTransformType;
      }
      set
      {
        m_iTransformType = value;
      }
    }
    /// <summary>
    /// An identifier associated with a specific transform.
    /// </summary>
    public string TransformId
    {
      get
      {
        return m_strTransformId;
      }
      set
      {
        m_strTransformId = value;
      }
    }
    /// <summary>
    /// The friendly name of the transform.
    /// </summary>
    public string TransformName
    {
      get
      {
        return m_strTransformName;
      }
      set
      {
        m_strTransformName = value;
      }
    }
    /// <summary>
    /// The reader version.
    /// </summary>
    public int ReaderVersion
    {
      get
      {
        return m_iReaderVersion;
      }
      set
      {
        m_iReaderVersion = value;
      }
    }
    /// <summary>
    /// The updater version.
    /// </summary>
    public int UpdaterVersion
    {
      get
      {
        return m_iUpdaterVersion;
      }
      set
      {
        m_iUpdaterVersion = value;
      }
    }
    /// <summary>
    /// The writer version.
    /// </summary>
    public int WriterVersion
    {
      get
      {
        return m_iWriterVersion;
      }
      set
      {
        m_iWriterVersion = value;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public TransformInfoHeader()
    {
    }
    /// <summary>
    /// Initializes new instance of the class.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    public TransformInfoHeader( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "steram" );

      byte[] arrBuffer = new byte[ ExcelConstants.IntSize ];
      int iLength = SecurityHelper.ReadInt32( stream, arrBuffer );
      m_iTransformType = SecurityHelper.ReadInt32( stream, arrBuffer );
      m_strTransformId = SecurityHelper.ReadUnicodeStringP4( stream );
      m_strTransformName = SecurityHelper.ReadUnicodeStringP4( stream );
      m_iReaderVersion = SecurityHelper.ReadInt32( stream, arrBuffer );
      m_iUpdaterVersion = SecurityHelper.ReadInt32( stream, arrBuffer );
      m_iWriterVersion = SecurityHelper.ReadInt32( stream, arrBuffer );
    }
    /// <summary>
    /// Serializes object into stream.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    public void Serialize( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      long lStart = stream.Position;

      stream.Position += ExcelConstants.IntSize;
      SecurityHelper.WriteInt32( stream, m_iTransformType );
      SecurityHelper.WriteUnicodeStringP4( stream, m_strTransformId);
      SecurityHelper.WriteUnicodeStringP4( stream, m_strTransformName);
      SecurityHelper.WriteInt32( stream, m_iReaderVersion );
      SecurityHelper.WriteInt32( stream, m_iUpdaterVersion );
      SecurityHelper.WriteInt32( stream, m_iWriterVersion );

      long lEnd = stream.Position;
      int iLength = ( int )( lEnd - lStart );
      stream.Position = lStart;
      SecurityHelper.WriteInt32( stream, iLength );
      stream.Position = lEnd;

      SecurityHelper.WriteUnicodeStringP4( stream, m_strTransformName);
      SecurityHelper.WriteInt32( stream, m_iReaderVersion );
      SecurityHelper.WriteInt32( stream, m_iUpdaterVersion );
      SecurityHelper.WriteInt32( stream, m_iWriterVersion );

      //long lEnd = stream.Position;
      //int iLength = ( int )( lEnd - lStart );
      //stream.Position = lStart;
      //SecurityHelper.WriteInt32( stream, iLength );
      //stream.Position = lEnd;
    }
    #endregion
  }
}

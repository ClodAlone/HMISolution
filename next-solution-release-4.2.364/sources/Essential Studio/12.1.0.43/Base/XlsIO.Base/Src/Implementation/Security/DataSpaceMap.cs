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
  class DataSpaceMap
  {
    #region Constants
    private const int DefaultHeaderSize = 8;
    #endregion

    #region Members
    /// <summary>
    /// Size of the header.
    /// </summary>
    private int m_iHeaderSize = DefaultHeaderSize;
    /// <summary>
    /// Map entries.
    /// </summary>
    private List<DataSpaceMapEntry> m_lstMapEntries = new List<DataSpaceMapEntry>();
    #endregion

    #region Properties
    /// <summary>
    /// Map entries.
    /// </summary>
    public List<DataSpaceMapEntry> MapEntries
    {
      get
      {
        return m_lstMapEntries;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public DataSpaceMap()
    {
    }
    /// <summary>
    /// Initializes new instance of the DataSpaceMap.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    public DataSpaceMap( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      byte[] arrBuffer = new byte[ ExcelConstants.IntSize ];

      m_iHeaderSize = SecurityHelper.ReadInt32( stream, arrBuffer );
      int iCount = SecurityHelper.ReadInt32( stream, arrBuffer );

      if( m_lstMapEntries.Capacity < iCount )
        m_lstMapEntries.Capacity = iCount;

      if( m_iHeaderSize != DefaultHeaderSize )
        stream.Position += m_iHeaderSize - DefaultHeaderSize;

      for( int i = 0; i < iCount; i++ )
      {
        DataSpaceMapEntry entry = new DataSpaceMapEntry( stream );
        m_lstMapEntries.Add( entry );
      }
    }
    /// <summary>
    /// Serializes dataspace map into the steram.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    public void Serialize( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      SecurityHelper.WriteInt32( stream, m_iHeaderSize );
      int iCount = m_lstMapEntries.Count;
      SecurityHelper.WriteInt32( stream, iCount );

      for( int i = 0; i < iCount; i++ )
      {
        DataSpaceMapEntry entry = m_lstMapEntries[ i ];
        entry.Serialize( stream );
      }
    }
    #endregion
  }
}

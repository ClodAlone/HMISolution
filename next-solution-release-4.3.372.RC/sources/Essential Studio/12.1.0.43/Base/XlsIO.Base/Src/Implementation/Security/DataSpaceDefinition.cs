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
  class DataSpaceDefinition
  {
    #region Constants
    /// <summary>
    /// Default header size.
    /// </summary>
    private const int DefaultHeaderLength = 8;
    #endregion

    #region Members
    /// <summary>
    /// Header length.
    /// </summary>
    private int m_iHeaderLength = DefaultHeaderLength;
    /// <summary>
    /// List with transform references.
    /// </summary>
    private List<string> m_lstTransformRefs = new List<string>();
    #endregion

    #region Properties
    /// <summary>
    /// List with transform references.
    /// </summary>
    public List<string> TransformRefs
    {
      get
      {
        return m_lstTransformRefs;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance.
    /// </summary>
    public DataSpaceDefinition()
    {
    }
    /// <summary>
    /// Initializes new instance of DataSpaceDefinition.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    public DataSpaceDefinition( Stream stream )
    {
      byte[] arrBuffer = new byte[ ExcelConstants.IntSize ];
      m_iHeaderLength = SecurityHelper.ReadInt32( stream, arrBuffer );
      int iCount = SecurityHelper.ReadInt32( stream, arrBuffer );

      if( m_iHeaderLength != DefaultHeaderLength )
        stream.Position += m_iHeaderLength - DefaultHeaderLength;

      for( int i = 0; i < iCount; i++ )
      {
        string strTransform = SecurityHelper.ReadUnicodeStringP4( stream );
        m_lstTransformRefs.Add( strTransform );
      }
    }
    /// <summary>
    /// Serializes dataspace definition into the stream.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    public void Serialize( Stream stream )
    {
      SecurityHelper.WriteInt32( stream, m_iHeaderLength );

      int iCount = m_lstTransformRefs.Count;
      SecurityHelper.WriteInt32( stream, iCount );

      for( int i = 0; i < iCount; i++ )
      {
        string strTransform = m_lstTransformRefs[ i ];
        SecurityHelper.WriteUnicodeStringP4( stream, strTransform );
      }
    }
    #endregion
  }
}

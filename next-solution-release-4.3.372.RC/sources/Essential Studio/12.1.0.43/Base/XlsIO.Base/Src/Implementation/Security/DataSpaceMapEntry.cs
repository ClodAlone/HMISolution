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
  class DataSpaceMapEntry
  {
    #region Members
    /// <summary>
    /// List of the reference components.
    /// </summary>
    private List<DataSpaceReferenceComponent> m_lstComponents = new List<DataSpaceReferenceComponent>();
    /// <summary>
    /// DataSpace name.
    /// </summary>
    private string m_strDataSpaceName;
    #endregion

    #region Properties
    /// <summary>
    /// List of the reference components.
    /// </summary>
    public List<DataSpaceReferenceComponent> Components
    {
      get
      {
        return m_lstComponents;
      }
    }
    /// <summary>
    /// DataSpace name.
    /// </summary>
    public string DataSpaceName
    {
      get
      {
        return m_strDataSpaceName;
      }
      set
      {
        m_strDataSpaceName = value;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public DataSpaceMapEntry()
    {
    }
    /// <summary>
    /// Initializes new instance of the map entry.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    public DataSpaceMapEntry( Stream stream )
    {
      byte[] arrBuffer = new byte[ ExcelConstants.IntSize ];
      int iLength = SecurityHelper.ReadInt32( stream, arrBuffer );
      int iReferenceCount = SecurityHelper.ReadInt32( stream, arrBuffer );

      for( int i = 0; i < iReferenceCount; i++ )
      {
        DataSpaceReferenceComponent component = new DataSpaceReferenceComponent( stream );
        m_lstComponents.Add( component );
      }

      m_strDataSpaceName = SecurityHelper.ReadUnicodeStringP4( stream );
    }
    /// <summary>
    /// Serializes single dataspace map entry into the stream.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    public void Serialize( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      //int iLength = SecurityHelper.ReadInt32( stream, arrBuffer );
      long lStartPosition = stream.Position;
      stream.Position += ExcelConstants.IntSize;

      int iReferenceCount = m_lstComponents.Count;
      SecurityHelper.WriteInt32( stream, iReferenceCount );

      for( int i = 0; i < iReferenceCount; i++ )
      {
        DataSpaceReferenceComponent component = m_lstComponents[ i ];
        component.Serialize( stream );
      }

      SecurityHelper.WriteUnicodeStringP4( stream, m_strDataSpaceName );

      // TODO: we can evaluate this value before serialization.
      long lEndPosition = stream.Position;
      stream.Position = lStartPosition;
      SecurityHelper.WriteInt32( stream, ( int )( lEndPosition - lStartPosition ) );
      stream.Position = lEndPosition;
    }
    #endregion
  }
}

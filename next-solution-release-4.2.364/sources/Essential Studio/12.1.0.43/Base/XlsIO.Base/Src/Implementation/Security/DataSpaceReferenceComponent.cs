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
  class DataSpaceReferenceComponent
  {
    #region Members
    /// <summary>
    /// Component type.
    /// </summary>
    private int m_iComponentType;
    /// <summary>
    /// Component name.
    /// </summary>
    private string m_strName;
    #endregion

    #region Properties
    /// <summary>
    /// Component type.
    /// </summary>
    public int ComponentType
    {
      get
      {
        return m_iComponentType;
      }
    }
    /// <summary>
    /// Component name.
    /// </summary>
    public string Name
    {
      get
      {
        return m_strName;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the reference component.
    /// </summary>
    /// <param name="type">Component type.</param>
    /// <param name="name">Component name.</param>
    public DataSpaceReferenceComponent( int type, string name )
    {
      m_iComponentType = type;
      m_strName = name;
    }
    /// <summary>
    /// Initializes new instance of the component reference.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    public DataSpaceReferenceComponent( Stream stream )
    {
      byte[] arrBuffer = new byte[ ExcelConstants.IntSize ];
      m_iComponentType = SecurityHelper.ReadInt32( stream, arrBuffer );
      m_strName = SecurityHelper.ReadUnicodeStringP4( stream );
    }
    /// <summary>
    /// Serializes object into stream.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    public void Serialize( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      SecurityHelper.WriteInt32( stream, m_iComponentType );
      SecurityHelper.WriteUnicodeStringP4( stream, m_strName );
    }
    #endregion
  }
}

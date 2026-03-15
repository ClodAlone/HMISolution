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
  class VersionInfo
  {
    #region Members
    /// <summary>
    /// The functionality for which the DataSpaceVersionInfo structure specifies
    /// version information. MUST be "Microsoft.Container.DataSpaces".
    /// </summary>
    private string m_strFeatureId = "Microsoft.Container.DataSpaces";
    /// <summary>
    /// The reader version of the data spaces structure.
    /// </summary>
    private int m_iReaderVersion = 1;
    /// <summary>
    /// The updater version of the data spaces structure.
    /// </summary>
    private int m_iUpdaterVersion = 1;
    /// <summary>
    /// The writer version of the data spaces structure.
    /// </summary>
    private int m_iWriterVersion = 1;
    #endregion

    #region Properties
    /// <summary>
    /// The functionality for which the DataSpaceVersionInfo structure specifies
    /// version information. MUST be "Microsoft.Container.DataSpaces".
    /// </summary>
    public string FeatureId
    {
      get
      {
        return m_strFeatureId;
      }
      set
      {
        m_strFeatureId = value;
      }
    }
    /// <summary>
    /// The reader version of the data spaces structure.
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
    /// The updater version of the data spaces structure.
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
    /// The writer version of the data spaces structure.
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
    public VersionInfo()
    {
    }
    /// <summary>
    /// Serializes object into specified stream.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    public void Serialize( Stream stream )
    {
      SecurityHelper.WriteUnicodeStringP4( stream, m_strFeatureId );
      SecurityHelper.WriteInt32( stream, m_iReaderVersion );
      SecurityHelper.WriteInt32( stream, m_iUpdaterVersion );
      SecurityHelper.WriteInt32( stream, m_iWriterVersion );
    }
    #endregion
  }
}

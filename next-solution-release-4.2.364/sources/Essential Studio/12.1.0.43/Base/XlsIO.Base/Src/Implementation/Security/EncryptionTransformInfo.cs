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
  class EncryptionTransformInfo
  {
    #region Members
    /// <summary>
    /// Transform name.
    /// </summary>
    private string m_strName;
    /// <summary>
    /// Block size.
    /// </summary>
    private int m_iBlockSize = 0x10;
    /// <summary>
    /// Cipher mode.
    /// </summary>
    private int m_iCipherMode;
    /// <summary>
    /// Reserved.
    /// </summary>
    private int m_iReserved = 0x04;
    #endregion

    #region Members
    /// <summary>
    /// Transform name.
    /// </summary>
    public string Name
    {
      get
      {
        return m_strName;
      }
      set
      {
        m_strName = value;
      }
    }
    /// <summary>
    /// Block size.
    /// </summary>
    public int BlockSize
    {
      get
      {
        return m_iBlockSize;
      }
      set
      {
        m_iBlockSize = value;
      }
    }
    /// <summary>
    /// Cipher mode.
    /// </summary>
    public int CipherMode
    {
      get
      {
        return m_iCipherMode;
      }
    }
    /// <summary>
    /// Reserved.
    /// </summary>
    public int Reserved
    {
      get
      {
        return m_iReserved;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public EncryptionTransformInfo()
    {
    }
    /// <summary>
    /// Initializes new instance of the class.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    public EncryptionTransformInfo( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      byte[] arrBuffer = new byte[ ExcelConstants.IntSize ];
      m_strName = SecurityHelper.ReadUnicodeStringP4( stream );
      m_iBlockSize = SecurityHelper.ReadInt32( stream, arrBuffer );
      m_iCipherMode = SecurityHelper.ReadInt32( stream, arrBuffer );
      m_iReserved = SecurityHelper.ReadInt32( stream, arrBuffer );
    }
    /// <summary>
    /// Serializes object into stream.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    public void Serialize( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      SecurityHelper.WriteUnicodeStringP4( stream, m_strName );
      SecurityHelper.WriteInt32( stream, m_iBlockSize );
      SecurityHelper.WriteInt32( stream, m_iCipherMode );
      SecurityHelper.WriteInt32( stream, m_iReserved );
    }
    #endregion
  }
}

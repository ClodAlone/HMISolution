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
using Syncfusion.CompoundFile;
using Syncfusion.CompoundFile.XlsIO.Native;
using Syncfusion.CompoundFile.XlsIO;
using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.XlsIO.Implementation
{
  internal class ObjectInfoStream : DataStructure
  {
    #region Constants
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_STRUCT_SIZE = 6;
    #endregion

    #region Fields
    /// <summary>
    /// 
    /// </summary>
    private byte[] m_dataBytes;
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
        return DEF_STRUCT_SIZE;
      }
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectInfoStream"/> class.
    /// </summary>
    /// <param name="compStream">The comp stream.</param>
    internal ObjectInfoStream( CompoundStream compStream )
    {
      byte[] bytes = new byte[ compStream.Length ];
      compStream.Read( bytes, 0, bytes.Length );
      Parse( bytes, 0 );
    }
    /// <summary>
    /// Initializes a default instance of the <see cref="ObjectInfoStream"/> class.
    /// </summary>
    internal ObjectInfoStream()
    {
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
      m_dataBytes = arrData;
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
    /// Saves the data to stream.
    /// </summary>
    /// <param name="stgStream">The STG stream.</param>
    internal void SaveTo( StgStream stgStream )
    {
      m_dataBytes = new byte[ DEF_STRUCT_SIZE ] { 128, 0, 3, 0, 4, 0 };
      stgStream.Write( m_dataBytes, 0, m_dataBytes.Length );
    }
    #endregion
  }
}

#endif
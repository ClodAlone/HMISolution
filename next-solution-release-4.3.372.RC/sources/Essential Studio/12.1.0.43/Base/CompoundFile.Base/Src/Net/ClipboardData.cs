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
using Syncfusion.CompoundFile.DocIO.Net;
#else
using Syncfusion.CompoundFile.XlsIO.Net;

#if SILVERLIGHT || WINRT || WP
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO;
#endif

#endif

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO
#else
namespace Syncfusion.CompoundFile.XlsIO
#endif
{
  public class ClipboardData : ICloneable
  {
    #region Members
    /// <summary>
    /// Clipboard format.
    /// </summary>
    public int Format;
    /// <summary>
    /// Clipboard data.
    /// </summary>
    public byte[] Data;
    #endregion

    #region ICloneable Members
    /// <summary>
    /// Createas copy of the current object.
    /// </summary>
    /// <returns>A copy of the current object.</returns>
    public object Clone()
    {
      ClipboardData result = ( ClipboardData )MemberwiseClone();
      result.Data = CloneUtils.CloneByteArray( Data );
      return result;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Saves clipboard data into stream.
    /// </summary>
    /// <param name="stream">Stream to write data into.</param>
    /// <returns>Size of the written data.</returns>
    public int Serialize( Stream stream )
    {
      int iWrittenSize = 0;
      int iSize = Data.Length;

      iWrittenSize += StreamHelper.WriteInt32( stream, iSize );
      iWrittenSize += StreamHelper.WriteInt32( stream, Format );
      stream.Write( Data, 0, iSize );

      iWrittenSize += iSize;
      return iWrittenSize;
    }
    /// <summary>
    /// Extracts data from the stream.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    public void Parse( Stream stream )
    {
      byte[] buffer = new byte[ StreamHelper.IntSize ];
      int iSize = StreamHelper.ReadInt32( stream, buffer );
      Format = StreamHelper.ReadInt32( stream, buffer );
      Data = new byte[ iSize ];
      stream.Read( Data, 0, iSize );
    }
    #endregion
  }
}

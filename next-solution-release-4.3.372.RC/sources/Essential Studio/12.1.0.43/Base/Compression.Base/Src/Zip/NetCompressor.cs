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
using Syncfusion.Compression;
using System.IO;

namespace Syncfusion.Compression.Zip
{
  public class NetCompressor : Stream
  {
    CompressedStreamWriter writer;

    public NetCompressor( CompressionLevel compressionLevel, Stream outputStream )
    {
      writer = new CompressedStreamWriter(
        outputStream, true, compressionLevel, false );
    }

    public void Write( byte[] data, int size, bool close )
    {
      writer.Write( data, 0, size, close );
    }

    public override bool CanRead
    {
      get { return false; }
    }

    public override bool CanSeek
    {
      get { return false; }
    }

    public override bool CanWrite
    {
      get { return true; }
    }

    public override void Flush()
    {
      //if( !writer.PendingBufferIsFlushed )
      //  writer.Write( new byte[] { ( byte )'x' }, 0, 1, true );
      //writer.SaveStored( true, true );
    }

    public override long Length
    {
      get { throw new NotImplementedException(); }
    }

    public override long Position
    {
      get
      {
        throw new NotImplementedException();
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    public override int Read( byte[] buffer, int offset, int count )
    {
      throw new NotImplementedException();
    }

    public override long Seek( long offset, SeekOrigin origin )
    {
      throw new NotImplementedException();
    }

    public override void SetLength( long value )
    {
      throw new NotImplementedException();
    }

    public override void Write( byte[] buffer, int offset, int count )
    {
      writer.Write( buffer, offset, count, false );
    }

    public override void Close()
    {
      writer.Close();
      base.Close();
    }
  }
}

#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.IO;
using System.Collections;

using Syncfusion.Windows.Forms.Edit.Implementation.IO;
#endregion

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// Interface for the streams.
  /// </summary>
  public interface IStream
  {
    /// <summary>
    /// GET sing of ability of reading.
    /// </summary>
    bool CanRead{ get; }
    /// <summary>
    /// GET sing of ability of seeking.
    /// </summary>
    bool CanSeek{ get; }
    /// <summary>
    /// GET sing of ability of writing.
    /// </summary>
    bool CanWrite{ get; }
    /// <summary>
    /// GET length of the stream.
    /// </summary>
    long Length{ get; }
    /// <summary>
    /// GET current position in the stream.
    /// </summary>
    long Position{ get; set; }

    /// <summary>
    /// Reads specified count of bytes from the stream.
    /// </summary>
    /// <param name="buffer">Buffer for reading.</param>
    /// <param name="offset">Offset in buffer.</param>
    /// <param name="count">Count of bytes to read to the buffer.</param>
    /// <returns>Count of bytes, really read.</returns>
    int  Read( byte[] buffer, int offset, int count );
    /// <summary>
    /// Writes specified count of bytes from the buffer to stream.
    /// </summary>
    /// <param name="buffer">Buffer with data.</param>
    /// <param name="offset">Offset in buffer.</param>
    /// <param name="count">Count of bytes to write to the stream.</param>
    /// <returns>Count of bytes, really written.</returns>
    void Write( byte[] buffer, int offset, int count );
    /// <summary>
    /// Sets current position in stream to specified value.
    /// </summary>
    /// <param name="offset">Needed value.</param>
    /// <param name="origin">Origin of the value.</param>
    /// <returns>Position in stream, really set.</returns>
    long Seek( long offset, SeekOrigin origin );
    /// <summary>
    /// Sets new length of the stream.
    /// </summary>
    /// <param name="value">New length.</param>
    void SetLength( long value );
    /// <summary>
    /// Closes stream.
    /// </summary>
    void Close();
    /// <summary>
    /// Reads single byte.
    /// </summary>
    /// <returns>Byte read.</returns>
    int  ReadByte();
    /// <summary>
    /// Writes byte to stream.
    /// </summary>
    /// <param name="value">Data to be written.</param>
    void WriteByte( byte value );
    /// <summary>
    /// Flushed internal buffers.
    /// </summary>
    void Flush();
  }

  /// <summary>
  /// Stream, that trackes changes.
  /// </summary>
  public interface IChangesStream : IStream
  {
    /// <summary>
    /// GET sing of posibility of undo.
    /// </summary>
    bool CanUndo{ get; }
    /// <summary>
    /// GET, SET count of changes, to be made, to execute autopush.
    /// </summary>
    int  AutoPush{ get; set; }

    /// <summary>
    /// Fushes changes to source and empties undo buffer.
    /// </summary>
    void FlushChanges();

    /// <summary>
    /// Addes and applies new changes.
    /// </summary>
    /// <param name="context">Change context.</param>
    void AddChange( ChangeContext context );
    /// <summary>
    /// Addes and applies new changes.
    /// </summary>
    /// <param name="context">Change context.</param>
    /// <param name="position">Position in stream.</param>
    void AddChange( long position, ChangeContext context );
    /// <summary>
    /// Undo last action.
    /// </summary>
    void Undo();
    /// <summary>
    /// Redoes last undone action.
    /// </summary>
    void Redo();
    /// <summary>
    /// Pops current state.
    /// </summary>
    void PopState();
    /// <summary>
    /// Pushes current state.
    /// </summary>
    void PushState();
    /// <summary>
    /// Copies all data to specified stream.
    /// </summary>
    /// <param name="stream">Destination stream.</param>
    void CopyTo( Stream stream );
  }
}
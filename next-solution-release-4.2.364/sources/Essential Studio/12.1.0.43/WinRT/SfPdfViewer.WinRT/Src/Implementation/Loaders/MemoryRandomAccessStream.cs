#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Syncfusion.Pdf
{
    class MemoryRandomAccessStream : IRandomAccessStream
    {
        private Stream m_InternalStream;

        public MemoryRandomAccessStream(Stream stream)
        {
            this.m_InternalStream = stream;
        }

        public MemoryRandomAccessStream(byte[] bytes)
        {
            this.m_InternalStream = new MemoryStream(bytes);
        }

        public IInputStream GetInputStreamAt(ulong position)
        {
            this.m_InternalStream.Seek((long)position, SeekOrigin.Begin);

            return this.m_InternalStream.AsInputStream();
        }

        public IOutputStream GetOutputStreamAt(ulong position)
        {
            this.m_InternalStream.Seek((long)position, SeekOrigin.Begin);

            return this.m_InternalStream.AsOutputStream();
        }

        public ulong Size
        {
            get { return (ulong)this.m_InternalStream.Length; }
            set { this.m_InternalStream.SetLength((long)value); }
        }

        public bool CanRead
        {
            get { return true; }
        }

        public bool CanWrite
        {
            get { return true; }
        }

        public IRandomAccessStream CloneStream()
        {
            throw new NotSupportedException();
        }

        public ulong Position
        {
            get { return (ulong)this.m_InternalStream.Position; }
        }

        public void Seek(ulong position)
        {
            this.m_InternalStream.Seek((long)position, 0);
        }

        public void Dispose()
        {
            this.m_InternalStream.Dispose();
        }

        public global::Windows.Foundation.IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
        {
            var inputStream = this.GetInputStreamAt(0);
            return inputStream.ReadAsync(buffer, count, options);
        }

        public global::Windows.Foundation.IAsyncOperation<bool> FlushAsync()
        {
            var outputStream = this.GetOutputStreamAt(0);
            return outputStream.FlushAsync();
        }

        public global::Windows.Foundation.IAsyncOperationWithProgress<uint, uint> WriteAsync(IBuffer buffer)
        {
            var outputStream = this.GetOutputStreamAt(0);
            return outputStream.WriteAsync(buffer);
        }
    }
}

#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
namespace Syncfusion.Pdf.JPEG2000.entropy.decoder
{

    internal class ByteInputBuffer
    {
        private byte[] buf;
        private int count;
        private int pos;
        public ByteInputBuffer(byte[] buf)
        {
            this.buf = buf;
            count = buf.Length;
        }
        public ByteInputBuffer(byte[] buf, int offset, int length)
        {
            this.buf = buf;
            pos = offset;
            count = offset + length;
        }
        public virtual void setByteArray(byte[] buf, int offset, int length)
        {

            if (buf == null)
            {
                if (length < 0 || count + length > this.buf.Length)
                {
                    throw new System.ArgumentException();
                }
                if (offset < 0)
                {
                    pos = count;
                    count += length;
                }
                else
                {
                    count = offset + length;
                    pos = offset;
                }
            }
            else
            {

                if (offset < 0 || length < 0 || offset + length > buf.Length)
                {
                    throw new System.ArgumentException();
                }
                this.buf = buf;
                count = offset + length;
                pos = offset;
            }
        }
        public virtual void addByteArray(byte[] data, int off, int len)
        {
            lock (this)
            {

                if (len < 0 || off < 0 || len + off > buf.Length)
                {
                    throw new System.ArgumentException();
                }

                if (count + len <= buf.Length)
                {

                    Array.Copy(data, off, buf, count, len);
                    count += len;
                }
                else
                {
                    if (count - pos + len <= buf.Length)
                    {

                        Array.Copy(buf, pos, buf, 0, count - pos);
                    }
                    else
                    {

                        byte[] oldbuf = buf;
                        buf = new byte[count - pos + len];

                        Array.Copy(oldbuf, count, buf, 0, count - pos);
                    }
                    count -= pos;
                    pos = 0;

                    Array.Copy(data, off, buf, count, len);
                    count += len;
                }
            }
        }
        public virtual int readChecked()
        {
            if (pos < count)
            {
                return (int)buf[pos++] & 0xFF;
            }
            else
            {
                throw new System.IO.EndOfStreamException();
            }
        }
        public virtual int read()
        {
            if (pos < count)
            {
                return (int)buf[pos++] & 0xFF;
            }
            else
            {
                return -1;
            }
        }
    }
}
#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
namespace Syncfusion.Pdf.JPEG2000.io
{
    internal class JPXRandomAccessStream
    {
        private System.IO.Stream is_Renamed;
        private int maxsize;
        private int inc;
        private byte[] buf;
        private int len;
        private int pos;
        private bool complete;
        virtual internal int Pos
        {
            get
            {
                return pos;
            }
        }
        internal JPXRandomAccessStream()
        { }
        internal JPXRandomAccessStream(System.IO.Stream is_Renamed, int size, int inc, int maxsize)
        {
            if (size < 0 || inc <= 0 || maxsize <= 0 || is_Renamed == null)
            {
                throw new System.ArgumentException();
            }
            this.is_Renamed = is_Renamed;
            if (size < System.Int32.MaxValue)
                size++;
            buf = new byte[size];
            this.inc = inc;
            if (maxsize < System.Int32.MaxValue)
                maxsize++;
            this.maxsize = maxsize;
            pos = 0;
            len = 0;
            complete = false;
        }
        private void growBuffer()
        {
            byte[] newbuf;
            int effinc;
            effinc = inc;
            if (buf.Length + effinc > maxsize)
                effinc = maxsize - buf.Length;
            if (effinc <= 0)
            {
                throw new System.IO.IOException("Reached maximum cache size (" + maxsize + ")");
            }
            try
            {
                newbuf = new byte[buf.Length + inc];
            }
            catch (System.OutOfMemoryException)
            {
                throw new System.IO.IOException("Out of memory to cache input data");
            }
            Array.Copy(buf, 0, newbuf, 0, len);
            buf = newbuf;
        }
        private void readInput()
        {
            int n;
            int k;
            if (complete)
            {
                throw new System.ArgumentException("Already reached EOF");
            }
            long available;
            available = is_Renamed.Length - is_Renamed.Position;
            n = (int)available;
            if (n == 0)
                n = 1;
            while (len + n > buf.Length)
            {
                growBuffer();
            }
            do
            {
                k = is_Renamed.Read(buf, len, n);
                if (k > 0)
                {
                    len += k;
                    n -= k;
                }
            }
            while (n > 0 && k > 0);
            if (k <= 0)
            {
                complete = true;
                
                is_Renamed = null;
            }
        }
        internal virtual void close()
        {
            buf = null;
            if (!complete)
            {
               
                is_Renamed = null;
            }
        }
        internal virtual void seek(int off)
        {
            if (complete)
            {
                if (off > len)
                {
                    throw new System.IO.EndOfStreamException();
                }
            }
            pos = off;
        }
        internal virtual int length()
        {
            while (!complete)
            {
                readInput();
            }
            return len;
        }
        internal virtual byte readByte() { return read(); }
        internal virtual byte read()
        {
            if (pos < len)
            {
                return buf[pos++];
            }
            while (!complete && pos >= len)
            {
                readInput();
            }
            if (pos == len)
            {
            }
            else if (pos > len)
            {
            }
            return buf[pos++];
        }
        internal virtual void readFully(byte[] b, int off, int n)
        {
            if (pos + n <= len)
            {
                Array.Copy(buf, pos, b, off, n);
                pos += n;
                return;
            }
            while (!complete && pos + n > len)
            {
                readInput();
            }
            if (pos + n > len)
            {
                throw new System.IO.EndOfStreamException();
            }
            Array.Copy(buf, pos, b, off, n);
            pos += n;
        }
        internal virtual byte readUnsignedByte()
        {
            if (pos < len)
            {
                return buf[pos++];
            }
            return read();
        }
        internal virtual short readShort()
        {
            if (pos + 1 < len)
            {
                return (short)((buf[pos++] << 8) | (0xFF & buf[pos++]));
            }
            return (short)((read() << 8) | read());
        }
        internal virtual int readUnsignedShort()
        {
            if (pos + 1 < len)
            {
                return ((0xFF & buf[pos++]) << 8) | (0xFF & buf[pos++]);
            }
            return (read() << 8) | read();
        }
        internal virtual int readInt()
        {
            if (pos + 3 < len)
            {
                return ((buf[pos++] << 24) | ((0xFF & buf[pos++]) << 16) | ((0xFF & buf[pos++]) << 8) | (0xFF & buf[pos++]));
            }
            return (read() << 24) | (read() << 16) | (read() << 8) | read();
        }
        internal virtual long readUnsignedInt()
        {
            if (pos + 3 < len)
            {
                return (unchecked((int)0xFFFFFFFFL) & (long)((buf[pos++] << 24) | ((0xFF & buf[pos++]) << 16) | ((0xFF & buf[pos++]) << 8) | (0xFF & buf[pos++])));
            }
            return (unchecked((int)0xFFFFFFFFL) & (long)((read() << 24) | (read() << 16) | (read() << 8) | read()));
        }
        internal virtual long readLong()
        {
            if (pos + 7 < len)
            {
                return (((long)buf[pos++] << 56) | ((long)buf[pos++] << 48) | ((long)buf[pos++] << 40) | ((long)buf[pos++] << 32) | ((long)buf[pos++] << 24) | ((long)buf[pos++] << 16) | ((long)buf[pos++] << 8) | (long)buf[pos++]);
            }
            return (((long)read() << 56) | ((long)read() << 48) | ((long)read() << 40) | ((long)read() << 32) | ((long)read() << 24) | ((long)read() << 16) | ((long)read() << 8) | (long)read());
        }
        internal virtual float readFloat()
        {
            int floatint;
            if (pos + 3 < len)
                floatint = (buf[pos++] << 24) | ((0xFF & buf[pos++]) << 16) | ((0xFF & buf[pos++]) << 8) | (0xFF & buf[pos++]);
            else
                floatint = (read() << 24) | (read() << 16) | (read() << 8) | read();
            return BitConverter.ToSingle(BitConverter.GetBytes(floatint), 0);
        }
        internal virtual double readDouble()
        {
            long doublelong;
            if (pos + 7 < len)
                doublelong = ((long)buf[pos++] << 56) | ((long)buf[pos++] << 48) | ((long)buf[pos++] << 40) | ((long)buf[pos++] << 32) | ((long)buf[pos++] << 24) | ((long)buf[pos++] << 16) | ((long)buf[pos++] << 8) | (long)buf[pos++];
            else
                doublelong = ((long)read() << 56) | ((long)read() << 48) | ((long)read() << 40) | ((long)read() << 32) | ((long)read() << 24) | ((long)read() << 16) | ((long)read() << 8) | (long)read();
            return BitConverter.ToDouble(BitConverter.GetBytes(doublelong), 0);
        }
        internal virtual int skipBytes(int n)
        {
            if (complete)
            {
                if (pos + n > len)
                {
                    throw new System.IO.EndOfStreamException();
                }
            }
            pos += n;
            return n;
        }
        internal virtual void flush()
        {
        }
        internal virtual void write(byte b)
        {
            throw new System.IO.IOException("read-only");
        }
        internal virtual void writeByte(int v)
        {
            throw new System.IO.IOException("read-only");
        }
        internal virtual void writeShort(int v)
        {
            throw new System.IO.IOException("read-only");
        }
        internal virtual void writeInt(int v)
        {
            throw new System.IO.IOException("read-only");
        }
        internal virtual void writeLong(long v)
        {
            throw new System.IO.IOException("read-only");
        }
        internal virtual void writeFloat(float v)
        {
            throw new System.IO.IOException("read-only");
        }
        internal virtual void writeDouble(double v)
        {
            throw new System.IO.IOException("read-only");
        }
    }
}
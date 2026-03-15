using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentLog4Net
{
    internal class CachedEntry
    {
        private byte[] buffer;
        private int offset;
        private int count;
        internal byte[] Buffer
        {
            get { return buffer; }
        }
        internal int Offset
        {
            get { return offset; }
        }
        internal int Count
        {
            get { return count; }
        }
        internal CachedEntry(byte[] buffer, int offset, int count)
        {
            this.buffer = new byte[buffer.Length];
            buffer.CopyTo(this.buffer, 0);
            this.offset = offset;
            this.count = count;
        }
    }
}

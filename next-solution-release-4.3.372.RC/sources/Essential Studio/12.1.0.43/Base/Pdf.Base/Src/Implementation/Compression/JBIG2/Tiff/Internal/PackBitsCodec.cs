#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;

namespace Syncfusion.Pdf.Compression.JBIG2.Internal
{
    class PackBitsCodec : TiffCodec
    {
        private enum EncodingState
        {
            BASE,
            LITERAL,
            RUN,
            LITERAL_RUN
        };

        private int m_rowsize;

        public PackBitsCodec(Tiff tif, Compression scheme, string name)
            : base(tif, scheme, name)
        {
        }

        public override bool Init()
        {
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether this codec can decode data.
        /// </summary>
        public override bool CanDecode
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Decodes one row of image data.
        /// </summary>
        public override bool DecodeRow(byte[] buffer, int offset, int count, short plane)
        {
            return PackBitsDecode(buffer, offset, count, plane);
        }

        /// <summary>
        /// Decodes one strip of image data.
        /// </summary>
        public override bool DecodeStrip(byte[] buffer, int offset, int count, short plane)
        {
            return PackBitsDecode(buffer, offset, count, plane);
        }

        /// <summary>
        /// Decodes one tile of image data.
        /// </summary>
        public override bool DecodeTile(byte[] buffer, int offset, int count, short plane)
        {
            return PackBitsDecode(buffer, offset, count, plane);
        }

        private bool PackBitsDecode(byte[] buffer, int offset, int count, short plane)
        {
            int bp = m_tif.m_rawcp;
            int cc = m_tif.m_rawcc;
            while (cc > 0 && count > 0)
            {
                int n = m_tif.m_rawdata[bp];
                bp++;
                cc--;

                // Watch out for compilers that don't sign extend chars...
                if (n >= 128)
                    n -= 256;

                if (n < 0)
                {
                    if (n == -128)
                    {
                        continue;
                    }

                    n = -n + 1;
                    if (count < n)
                    {
                        n = count;
                    }
                    count -= n;
                    int b = m_tif.m_rawdata[bp];
                    bp++;
                    cc--;
                    while (n-- > 0)
                    {
                        buffer[offset] = (byte)b;
                        offset++;
                    }
                }
                else
                {
                    if (count < n + 1)
                    {
                        n = count - 1;
                    }

                    Buffer.BlockCopy(m_tif.m_rawdata, bp, buffer, offset, ++n);
                    offset += n;
                    count -= n;
                    bp += n;
                    cc -= n;
                }
            }

            m_tif.m_rawcp = bp;
            m_tif.m_rawcc = cc;
            if (count > 0)
            {
                return false;
            }

            return true;
        }
    }

    internal class TagCompare : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            TiffFieldInfo ta = x as TiffFieldInfo;
            TiffFieldInfo tb = y as TiffFieldInfo;

            if (ta.Tag != tb.Tag)
                return ((int)ta.Tag - (int)tb.Tag);

            return (ta.Type == TiffType.ANY) ? 0 : ((int)tb.Type - (int)ta.Type);
        }
    }
}

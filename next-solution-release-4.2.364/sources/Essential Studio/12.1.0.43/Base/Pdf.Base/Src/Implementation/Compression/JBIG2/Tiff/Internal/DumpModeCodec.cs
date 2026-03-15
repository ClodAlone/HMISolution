#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.Pdf.Compression.JBIG2.Internal
{
    class DumpModeCodec : TiffCodec
    {
        public DumpModeCodec(Tiff tif, Compression scheme, string name)
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
            return DumpModeDecode(buffer, offset, count, plane);
        }

        /// <summary>
        /// Decodes one strip of image data.
        /// </summary>
        public override bool DecodeStrip(byte[] buffer, int offset, int count, short plane)
        {
            return DumpModeDecode(buffer, offset, count, plane);
        }

        /// <summary>
        /// Decodes one tile of image data.
        /// </summary>
        public override bool DecodeTile(byte[] buffer, int offset, int count, short plane)
        {
            return DumpModeDecode(buffer, offset, count, plane);
        }
      
        /// <summary>
        /// Seeks the specified row in the strip being processed.
        /// </summary>
        public override bool Seek(int row)
        {
            m_tif.m_rawcp += row * m_tif.m_scanlinesize;
            m_tif.m_rawcc -= row * m_tif.m_scanlinesize;
            return true;
        }
        
        /// <summary>
        /// Decode a hunk of pixels.
        /// </summary>
        private bool DumpModeDecode(byte[] buffer, int offset, int count, short plane)
        {
            if (m_tif.m_rawcc < count)
            {
                return false;
            }

            Buffer.BlockCopy(m_tif.m_rawdata, m_tif.m_rawcp, buffer, offset, count);
            m_tif.m_rawcp += count;
            m_tif.m_rawcc -= count;
            return true;
        }
    }
}

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

namespace Syncfusion.Pdf
{
    class PageInformationSegment : JBIG2Segment
    {
        private int m_pageBitmapHeight, m_pageBitmapWidth;
        private int m_yResolution, m_xResolution;
        private BitOperation m_bitOperation = new BitOperation();
        private int m_pageStriping;
        private JBIG2Image m_pageBitmap;
        private PageInformationFlags m_pageInformationFlags = new PageInformationFlags();

        internal PageInformationSegment(JBIG2StreamDecoder streamDecoder)
            : base(streamDecoder)
        {

        }

        internal  PageInformationFlags pageInformationFlags
        {
            get
            {
                return m_pageInformationFlags;
            }
        }

        internal  JBIG2Image pageBitmap
        {
            get
            {
                return m_pageBitmap;
            }
        }

        public override void readSegment()
        {
            short[] buff = new short[4];
            m_decoder.ReadByte(buff);
            m_pageBitmapWidth = m_bitOperation.GetInt32(buff);

            buff = new short[4];
            m_decoder.ReadByte(buff);
            m_pageBitmapHeight = m_bitOperation.GetInt32(buff);
            buff = new short[4];
            m_decoder.ReadByte(buff);
            m_xResolution = m_bitOperation.GetInt32(buff);

            buff = new short[4];
            m_decoder.ReadByte(buff);
            m_yResolution = m_bitOperation.GetInt32(buff);
            /// <summary>
            /// extract page information flags </summary>
            short pageInformationFlagsField = m_decoder.ReadByte();

            m_pageInformationFlags.setFlags(pageInformationFlagsField);

            buff = new short[2];
            m_decoder.ReadByte(buff);
            m_pageStriping = m_bitOperation.GetInt16(buff);
            int defPix = m_pageInformationFlags.GetFlagValue(PageInformationFlags.DEFAULT_PIXEL_VALUE);
            int height;

            if (m_pageBitmapHeight == -1)
            {
                height = m_pageStriping & 0x7fff;
            }
            else
            {
                height = m_pageBitmapHeight;
            }

            m_pageBitmap = new JBIG2Image(m_pageBitmapWidth, height, m_arithmeticDecoder, m_huffmanDecoder, m_mmrDecoder);
            m_pageBitmap.Clear(defPix);
        }

        internal int pageBitmapHeight
        {
            get
            {
                return m_pageBitmapHeight;
            }
        }
    }

    class PageInformationFlags : JBIG2BaseFlags
    {
        internal const string DEFAULT_PIXEL_VALUE = "DEFAULT_PIXEL_VALUE";
        internal const string DEFAULT_COMBINATION_OPERATOR = "DEFAULT_COMBINATION_OPERATOR";

        public override void setFlags(int flagAsInt)
        {
            this.flagsAsInt = flagAsInt;

            /// <summary>
            /// extract DEFAULT_PIXEL_VALUE </summary>
            flags.Add(DEFAULT_PIXEL_VALUE, new int?((flagAsInt >> 2) & 1));

            /// <summary>
            /// extract DEFAULT_COMBINATION_OPERATOR </summary>
            flags.Add(DEFAULT_COMBINATION_OPERATOR, new int?((flagAsInt >> 3) & 3));
        }
    }
}

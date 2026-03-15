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
    class PatternDictionarySegment : JBIG2Segment
    {
        PatternDictionaryFlags m_patternDictionaryFlags = new PatternDictionaryFlags();
        private int m_width;
        private int m_height;
        private int m_grayMax;
        private JBIG2Image[] m_bitmaps;
        private int m_size;
        private BitOperation m_bitOperation = new BitOperation();

        internal int Size
        {
            get
            {
                return m_size;
            }
        }

        public PatternDictionarySegment(JBIG2StreamDecoder streamDecoder)
            : base(streamDecoder)
        {
        }

        public override void readSegment()
        {
            ReadPatternDictionaryFlags();

            m_width = m_decoder.ReadByte();
            m_height = m_decoder.ReadByte();

            short[] buf = new short[4];
            m_decoder.ReadByte(buf);
            m_grayMax = m_bitOperation.GetInt32(buf);
            bool useMMR = m_patternDictionaryFlags.GetFlagValue(PatternDictionaryFlags.HD_MMR) == 1;
            int template = m_patternDictionaryFlags.GetFlagValue(PatternDictionaryFlags.HD_TEMPLATE);

            if (!useMMR)
            {
                m_arithmeticDecoder.ResetGenericStats(template, null);
                m_arithmeticDecoder.Start();
            }

            short[] genericBAdaptiveTemplateX = new short[4], genericBAdaptiveTemplateY = new short[4];

            genericBAdaptiveTemplateX[0] = (short)-m_width;
            genericBAdaptiveTemplateY[0] = 0;
            genericBAdaptiveTemplateX[1] = -3;
            genericBAdaptiveTemplateY[1] = -1;
            genericBAdaptiveTemplateX[2] = 2;
            genericBAdaptiveTemplateY[2] = -2;
            genericBAdaptiveTemplateX[3] = -2;
            genericBAdaptiveTemplateY[3] = -2;

            m_size = m_grayMax + 1;

            JBIG2Image bitmap = new JBIG2Image(m_size * m_width, m_height, m_arithmeticDecoder, m_huffmanDecoder, m_mmrDecoder);
            bitmap.Clear(0);
            bitmap.ReadBitmap(useMMR, template, false, false, null, genericBAdaptiveTemplateX, genericBAdaptiveTemplateY, m_segmentHeader.DataLength - 7);

            JBIG2Image[] bitmaps = new JBIG2Image[m_size];

            int x = 0;
            for (int i = 0; i < m_size; i++)
            {
                bitmaps[i] = bitmap.GetSlice(x, 0, m_width, m_height);
                x += m_width;
            }
            this.m_bitmaps = bitmaps;
        }

        internal JBIG2Image[] GetBitmaps()
        {
            return m_bitmaps;
        }

        private void ReadPatternDictionaryFlags()
        {
            short patternDictionaryFlagsField = m_decoder.ReadByte();
            m_patternDictionaryFlags.setFlags(patternDictionaryFlagsField);
        }
    }

    class PatternDictionaryFlags : JBIG2BaseFlags
    {
        public const String HD_MMR = "HD_MMR";
        public const String HD_TEMPLATE = "HD_TEMPLATE";

        public override void setFlags(int flagsAsInt)
        {
            this.flagsAsInt = flagsAsInt;

            /** extract HD_MMR */
            flags.Add(HD_MMR, (flagsAsInt & 1));

            /** extract HD_TEMPLATE */
            flags.Add(HD_TEMPLATE, ((flagsAsInt >> 1) & 3));
        }
    }
}

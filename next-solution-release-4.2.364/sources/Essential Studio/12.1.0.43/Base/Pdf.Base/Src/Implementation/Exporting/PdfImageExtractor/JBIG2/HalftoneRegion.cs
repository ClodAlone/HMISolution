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
    internal class HalftoneRegionSegment : JBIG2BaseSegment
    {
        private HalftoneRegionFlags m_halftoneRegionFlags = new HalftoneRegionFlags();
        private BitOperation m_bitOperation = new BitOperation();
        private bool inlineImage;

        internal HalftoneRegionSegment(JBIG2StreamDecoder streamDecoder, bool inlineImage)
            : base(streamDecoder)
        {
            this.inlineImage = inlineImage;
        }

        public override void readSegment()
        {
            base.readSegment();

            /// <summary>
            /// read text region Segment flags </summary>
            ReadHalftoneRegionFlags();

            short[] buf = new short[4];
            m_decoder.ReadByte(buf);
            int gridWidth = m_bitOperation.GetInt32(buf);

            buf = new short[4];
            m_decoder.ReadByte(buf);
            int gridHeight = m_bitOperation.GetInt32(buf);

            buf = new short[4];
            m_decoder.ReadByte(buf);
            int gridX = m_bitOperation.GetInt32(buf);

            buf = new short[4];
            m_decoder.ReadByte(buf);
            int gridY = m_bitOperation.GetInt32(buf);

            buf = new short[2];
            m_decoder.ReadByte(buf);
            int stepX = m_bitOperation.GetInt16(buf);

            buf = new short[2];
            m_decoder.ReadByte(buf);
            int stepY = m_bitOperation.GetInt16(buf);

            int[] referedToSegments = m_segmentHeader.ReferredToSegments;
            JBIG2Segment segment = m_decoder.FindSegment(referedToSegments[0]);
            PatternDictionarySegment patternDictionarySegment = (PatternDictionarySegment)segment;

            int bitsPerValue = 0, i = 1;
            while (i < patternDictionarySegment.Size)
            {
                bitsPerValue++;
                i <<= 1;
            }

            JBIG2Image bitmap = patternDictionarySegment.GetBitmaps()[0];
            int patternWidth = bitmap.Width;
            int patternHeight = bitmap.Height;

            bool useMMR = m_halftoneRegionFlags.GetFlagValue(HalftoneRegionFlags.H_MMR) != 0;
            int template = m_halftoneRegionFlags.GetFlagValue(HalftoneRegionFlags.H_TEMPLATE);

            if (!useMMR)
            {
                m_arithmeticDecoder.ResetGenericStats(template, null);
                m_arithmeticDecoder.Start();
            }

            int halftoneDefaultPixel = m_halftoneRegionFlags.GetFlagValue(HalftoneRegionFlags.H_DEF_PIXEL);
            bitmap = new JBIG2Image(regionBitmapWidth, regionBitmapHeight, m_arithmeticDecoder, m_huffmanDecoder, m_mmrDecoder);
            bitmap.Clear(halftoneDefaultPixel);
            bool enableSkip = m_halftoneRegionFlags.GetFlagValue(HalftoneRegionFlags.H_ENABLE_SKIP) != 0;

            JBIG2Image skipBitmap = null;
            if (enableSkip)
            {
                skipBitmap = new JBIG2Image(gridWidth, gridHeight, m_arithmeticDecoder, m_huffmanDecoder, m_mmrDecoder);
                skipBitmap.Clear(0);
                for (int y = 0; y < gridHeight; y++)
                {
                    for (int x = 0; x < gridWidth; x++)
                    {
                        int xx = gridX + y * stepY + x * stepX;
                        int yy = gridY + y * stepX - x * stepY;

                        if (((xx + patternWidth) >> 8) <= 0 || (xx >> 8) >= regionBitmapWidth || ((yy + patternHeight) >> 8) <= 0 || (yy >> 8) >= regionBitmapHeight)
                        {
                            skipBitmap.SetPixel(y, x, 1);
                        }
                    }
                }
            }
            int[] grayScaleImage = new int[gridWidth * gridHeight];

            short[] genericBAdaptiveTemplateX = new short[4], genericBAdaptiveTemplateY = new short[4];

            genericBAdaptiveTemplateX[0] = (short)(template <= 1 ? 3 : 2);
            genericBAdaptiveTemplateY[0] = -1;
            genericBAdaptiveTemplateX[1] = -3;
            genericBAdaptiveTemplateY[1] = -1;
            genericBAdaptiveTemplateX[2] = 2;
            genericBAdaptiveTemplateY[2] = -2;
            genericBAdaptiveTemplateX[3] = -2;
            genericBAdaptiveTemplateY[3] = -2;

            JBIG2Image grayBitmap;

            for (int j = bitsPerValue - 1; j >= 0; --j)
            {
                grayBitmap = new JBIG2Image(gridWidth, gridHeight, m_arithmeticDecoder, m_huffmanDecoder, m_mmrDecoder);

                grayBitmap.ReadBitmap(useMMR, template, false, enableSkip, skipBitmap, genericBAdaptiveTemplateX, genericBAdaptiveTemplateY, -1);

                i = 0;
                for (int row = 0; row < gridHeight; row++)
                {
                    for (int col = 0; col < gridWidth; col++)
                    {
                        int bit = grayBitmap.GetPixel(col, row) ^ (grayScaleImage[i] & 1);
                        grayScaleImage[i] = (grayScaleImage[i] << 1) | bit;
                        i++;
                    }
                }
            }

            int combinationOperator = m_halftoneRegionFlags.GetFlagValue(HalftoneRegionFlags.H_COMB_OP);

            i = 0;
            for (int col = 0; col < gridHeight; col++)
            {
                int xx = gridX + col * stepY;
                int yy = gridY + col * stepX;
                for (int row = 0; row < gridWidth; row++)
                {
                    if (!(enableSkip && skipBitmap.GetPixel(col, row) == 1))
                    {
                        JBIG2Image patternBitmap = patternDictionarySegment.GetBitmaps()[grayScaleImage[i]];
                        bitmap.Combine(patternBitmap, xx >> 8, yy >> 8, combinationOperator);
                    }

                    xx += stepX;
                    yy -= stepY;

                    i++;
                }
            }

            if (inlineImage)
            {
                PageInformationSegment pageSegment = m_decoder.FindPageSegement(m_segmentHeader.PageAssociation);
                JBIG2Image pageBitmap = pageSegment.pageBitmap;

                int externalCombinationOperator = regionFlags.GetFlagValue(RegionFlags.EXTERNAL_COMBINATION_OPERATOR);
                pageBitmap.Combine(bitmap, regionBitmapXLocation, regionBitmapYLocation, externalCombinationOperator);
            }
            else
            {
                bitmap.BitmapNumber = m_segmentHeader.SegmentNumber;
                m_decoder.appendBitmap(bitmap);
            }

        }

        private void ReadHalftoneRegionFlags()
        {
            /// <summary>
            /// extract text region Segment flags </summary>
            short halftoneRegionFlagsField = m_decoder.ReadByte();
            m_halftoneRegionFlags.setFlags(halftoneRegionFlagsField);
        }
    }

    internal class HalftoneRegionFlags : JBIG2BaseFlags
    {
        internal const String H_MMR = "H_MMR";
        internal const String H_TEMPLATE = "H_TEMPLATE";
        internal const String H_ENABLE_SKIP = "H_ENABLE_SKIP";
        internal const String H_COMB_OP = "H_COMB_OP";
        internal const String H_DEF_PIXEL = "H_DEF_PIXEL";

        public override void setFlags(int flagsAsInt)
        {
            this.flagsAsInt = flagsAsInt;

            /** extract H_MMR */
            flags.Add(H_MMR, (flagsAsInt & 1));

            /** extract H_TEMPLATE */
            flags.Add(H_TEMPLATE, ((flagsAsInt >> 1) & 3));

            /** extract H_ENABLE_SKIP */
            flags.Add(H_ENABLE_SKIP, ((flagsAsInt >> 3) & 1));

            /** extract H_COMB_OP */
            flags.Add(H_COMB_OP, ((flagsAsInt >> 4) & 7));

            /** extract H_DEF_PIXEL */
            flags.Add(H_DEF_PIXEL, ((flagsAsInt >> 7) & 1));
        }
    }
}

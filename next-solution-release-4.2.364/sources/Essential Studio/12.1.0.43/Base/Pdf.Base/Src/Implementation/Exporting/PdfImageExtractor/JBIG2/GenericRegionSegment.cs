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
    class GenericRegionSegment:JBIG2BaseSegment
    {
        private GenericRegionFlags m_genericRegionFlags = new GenericRegionFlags();
        private bool m_inlineImage;
        private bool m_unknownLength = false;

        public GenericRegionSegment(JBIG2StreamDecoder streamDecoder, bool inlineImage)
            : base(streamDecoder)
        {
            this.m_inlineImage = inlineImage;
        }

        public override void readSegment()
        {
            base.readSegment();

            /// <summary>
            /// read text region Segment flags </summary>
            ReadGenericRegionFlags();

            bool useMMR = m_genericRegionFlags.GetFlagValue(GenericRegionFlags.MMR) != 0;
            int template = m_genericRegionFlags.GetFlagValue(GenericRegionFlags.GB_TEMPLATE);

            short[] genericBAdaptiveTemplateX = new short[4];
            short[] genericBAdaptiveTemplateY = new short[4];

            if (!useMMR)
            {
                if (template == 0)
                {
                    genericBAdaptiveTemplateX[0] = ReadAtValue();
                    genericBAdaptiveTemplateY[0] = ReadAtValue();
                    genericBAdaptiveTemplateX[1] = ReadAtValue();
                    genericBAdaptiveTemplateY[1] = ReadAtValue();
                    genericBAdaptiveTemplateX[2] = ReadAtValue();
                    genericBAdaptiveTemplateY[2] = ReadAtValue();
                    genericBAdaptiveTemplateX[3] = ReadAtValue();
                    genericBAdaptiveTemplateY[3] = ReadAtValue();
                }
                else
                {
                    genericBAdaptiveTemplateX[0] = ReadAtValue();
                    genericBAdaptiveTemplateY[0] = ReadAtValue();
                }

                m_arithmeticDecoder.ResetGenericStats(template, null);
                m_arithmeticDecoder.Start();
            }

            bool typicalPredictionGenericDecodingOn = m_genericRegionFlags.GetFlagValue(GenericRegionFlags.TPGDON) != 0;
            int length = m_segmentHeader.DataLength;

            if (length == -1)
            {
                /// <summary>
                /// length of data is unknown, so it needs to be determined through examination of the data.
                /// See 7.2.7 - Segment data length of the JBIG2 specification.
                /// </summary>

                m_unknownLength = true;

                short match1;
                short match2;

                if (useMMR)
                {
                    // look for 0x00 0x00 (0, 0)

                    match1 = 0;
                    match2 = 0;
                }
                else
                {
                    // look for 0xFF 0xAC (255, 172)

                    match1 = 255;
                    match2 = 172;
                }

                int bytesRead = 0;
                while (true)
                {
                    short bite1 = m_decoder.ReadByte();
                    bytesRead++;

                    if (bite1 == match1)
                    {
                        short bite2 = m_decoder.ReadByte();
                        bytesRead++;

                        if (bite2 == match2)
                        {
                            length = bytesRead - 2;
                            break;
                        }
                    }
                }
                m_decoder.MovePointer(-bytesRead);
            }

            JBIG2Image bitmap = new JBIG2Image(regionBitmapWidth, regionBitmapHeight, m_arithmeticDecoder, m_huffmanDecoder, m_mmrDecoder);
            bitmap.Clear(0);
            bitmap.ReadBitmap(useMMR, template, typicalPredictionGenericDecodingOn, false, null, genericBAdaptiveTemplateX, genericBAdaptiveTemplateY, useMMR ? 0 : length - 18);
            if (m_inlineImage)
            {
                PageInformationSegment pageSegment = m_decoder.FindPageSegement(m_segmentHeader.PageAssociation);
                JBIG2Image pageBitmap = pageSegment.pageBitmap;

                int extCombOp = regionFlags.GetFlagValue(RegionFlags.EXTERNAL_COMBINATION_OPERATOR);

                if (pageSegment.pageBitmapHeight == -1 && regionBitmapYLocation + regionBitmapHeight > pageBitmap.Height)
                {
                    pageBitmap.Expand(regionBitmapYLocation + regionBitmapHeight, pageSegment.pageInformationFlags.GetFlagValue(PageInformationFlags.DEFAULT_PIXEL_VALUE));
                }

                pageBitmap.Combine(bitmap, regionBitmapXLocation, regionBitmapYLocation, extCombOp);
            }
            else
            {
                bitmap.BitmapNumber = m_segmentHeader.SegmentNumber;
                m_decoder.appendBitmap(bitmap);
            }

            if (m_unknownLength)
            {
                m_decoder.MovePointer(4);
            }
        }

        private void ReadGenericRegionFlags()
        {
            /// <summary>
            /// extract text region Segment flags </summary>
            short genericRegionFlagsField = m_decoder.ReadByte();

            m_genericRegionFlags.setFlags(genericRegionFlagsField);
        }
    }

    public class GenericRegionFlags : JBIG2BaseFlags
    {
        public const string MMR = "MMR";
        public const string GB_TEMPLATE = "GB_TEMPLATE";
        public const string TPGDON = "TPGDON";

        public override void setFlags(int flagsAsInt)
        {
            this.flagsAsInt = flagsAsInt;

            /// <summary>
            /// extract MMR </summary>
            flags.Add(MMR, new int?(flagsAsInt & 1));

            /// <summary>
            /// extract GB_TEMPLATE </summary>
            flags.Add(GB_TEMPLATE, new int?((flagsAsInt >> 1) & 3));

            /// <summary>
            /// extract TPGDON </summary>
            flags.Add(TPGDON, new int?((flagsAsInt >> 3) & 1));
        }
    }
}

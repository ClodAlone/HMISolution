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
using System.Collections;

namespace Syncfusion.Pdf
{
    class TextRegionSegment : JBIG2BaseSegment
    {
        private TextRegionFlags m_textRegionFlags = new TextRegionFlags();
        private TextRegionHuffmanFlags m_textRegionHuffmanFlags = new TextRegionHuffmanFlags();
        private int m_noOfSymbolInstances;
        private bool m_inlineImage;
        private short[] m_symbolRegionAdaptiveTemplateX = new short[2], symbolRegionAdaptiveTemplateY = new short[2];
        private HuffmanDecoder m_huffDecoder = new HuffmanDecoder();
        private BitOperation m_bitOperation = new BitOperation();
        private RectangularArrays m_rectangularArrays = new RectangularArrays();

        public TextRegionSegment(JBIG2StreamDecoder streamDecoder, bool inlineImage)
            : base(streamDecoder)
        {
            this.m_inlineImage = inlineImage;
        }

        public override void readSegment()
        {
            base.readSegment();

            /// <summary>
            /// read text region Segment flags </summary>
            ReadTextRegionFlags();

            short[] buff = new short[4];
            m_decoder.ReadByte(buff);
            m_noOfSymbolInstances = m_bitOperation.GetInt32(buff);

            int noOfReferredToSegments = m_segmentHeader.ReferedToSegCount;
            int[] referredToSegments = m_segmentHeader.ReferredToSegments;

            IList codeTables = new List<object>();
            IList segmentsReferenced = new List<object>();
            int noOfSymbols = 0;

            for (int i = 0; i < noOfReferredToSegments; i++)
            {
                JBIG2Segment seg = m_decoder.FindSegment(referredToSegments[i]);
                int type = seg.m_segmentHeader.SegmentType;

                if (type == JBIG2Segment.SYMBOL_DICTIONARY)
                {
                    segmentsReferenced.Add(seg);
                    noOfSymbols += ((SymbolDictionarySegment)seg).NoOfExportedSymbols;
                }
                else if (type == JBIG2Segment.TABLES)
                {
                    codeTables.Add(seg);
                }
            }

            int symbolCodeLength = 0;
            int count = 1;

            while (count < noOfSymbols)
            {
                symbolCodeLength++;
                count <<= 1;
            }

            int currentSymbol = 0;
            JBIG2Image[] symbols = new JBIG2Image[noOfSymbols];
            for (IEnumerator it = segmentsReferenced.GetEnumerator(); it.MoveNext(); )
            {
                JBIG2Segment seg = (JBIG2Segment)it.Current;
                if (seg.m_segmentHeader.SegmentType == JBIG2Segment.SYMBOL_DICTIONARY)
                {
                    JBIG2Image[] bitmaps = ((SymbolDictionarySegment)seg).getBitmaps();
                    for (int j = 0; j < bitmaps.Length; j++)
                    {
                        symbols[currentSymbol] = bitmaps[j];
                        currentSymbol++;
                    }
                }
            }

            int[][] huffmanFSTable = null;
            int[][] huffmanDSTable = null;
            int[][] huffmanDTTable = null;
            int[][] huffmanRDWTable = null;
            int[][] huffmanRDHTable = null;
            int[][] huffmanRDXTable = null;
            int[][] huffmanRDYTable = null;
            int[][] huffmanRSizeTable = null;

            bool sbHuffman = m_textRegionFlags.GetFlagValue(TextRegionFlags.SB_HUFF) != 0;

            int s = 0;
            if (sbHuffman)
            {
                int sbHuffFS = m_textRegionHuffmanFlags.GetFlagValue(TextRegionHuffmanFlags.SB_HUFF_FS);
                if (sbHuffFS == 0)
                {
                    huffmanFSTable = m_huffDecoder.huffmanTableF;
                }
                else if (sbHuffFS == 1)
                {
                    huffmanFSTable = m_huffDecoder.huffmanTableG;
                }

                int sbHuffDS = m_textRegionHuffmanFlags.GetFlagValue(TextRegionHuffmanFlags.SB_HUFF_DS);
                if (sbHuffDS == 0)
                {
                    huffmanDSTable = m_huffDecoder.huffmanTableH;
                }
                else if (sbHuffDS == 1)
                {
                    huffmanDSTable = m_huffDecoder.huffmanTableI;
                }
                else if (sbHuffDS == 2)
                {
                    huffmanDSTable = m_huffDecoder.huffmanTableJ;
                }

                int sbHuffDT = m_textRegionHuffmanFlags.GetFlagValue(TextRegionHuffmanFlags.SB_HUFF_DT);
                if (sbHuffDT == 0)
                {
                    huffmanDTTable = m_huffDecoder.huffmanTableK;
                }
                else if (sbHuffDT == 1)
                {
                    huffmanDTTable = m_huffDecoder.huffmanTableL;
                }
                else if (sbHuffDT == 2)
                {
                    huffmanDTTable = m_huffDecoder.huffmanTableM;
                }

                int sbHuffRDW = m_textRegionHuffmanFlags.GetFlagValue(TextRegionHuffmanFlags.SB_HUFF_RDW);
                if (sbHuffRDW == 0)
                {
                    huffmanRDWTable = m_huffDecoder.huffmanTableN;
                }
                else if (sbHuffRDW == 1)
                {
                    huffmanRDWTable = m_huffDecoder.huffmanTableO;
                }

                int sbHuffRDH = m_textRegionHuffmanFlags.GetFlagValue(TextRegionHuffmanFlags.SB_HUFF_RDH);
                if (sbHuffRDH == 0)
                {
                    huffmanRDHTable = m_huffDecoder.huffmanTableN;
                }
                else if (sbHuffRDH == 1)
                {
                    huffmanRDHTable = m_huffDecoder.huffmanTableO;
                }

                int sbHuffRDX = m_textRegionHuffmanFlags.GetFlagValue(TextRegionHuffmanFlags.SB_HUFF_RDX);
                if (sbHuffRDX == 0)
                {
                    huffmanRDXTable = m_huffDecoder.huffmanTableN;
                }
                else if (sbHuffRDX == 1)
                {
                    huffmanRDXTable = m_huffDecoder.huffmanTableO;
                }

                int sbHuffRDY = m_textRegionHuffmanFlags.GetFlagValue(TextRegionHuffmanFlags.SB_HUFF_RDY);
                if (sbHuffRDY == 0)
                {
                    huffmanRDYTable = m_huffDecoder.huffmanTableN;
                }
                else if (sbHuffRDY == 1)
                {
                    huffmanRDYTable = m_huffDecoder.huffmanTableO;
                }

                int sbHuffRSize = m_textRegionHuffmanFlags.GetFlagValue(TextRegionHuffmanFlags.SB_HUFF_RSIZE);
                if (sbHuffRSize == 0)
                {
                    huffmanRSizeTable = m_huffDecoder.huffmanTableA;
                }
            }

            int[][] runLengthTable = m_rectangularArrays.ReturnRectangularIntArray(36, 4);
            int[][] symbolCodeTable = m_rectangularArrays.ReturnRectangularIntArray(noOfSymbols + 1, 4);
            if (sbHuffman)
            {
                m_decoder.ConsumeRemainingBits();
                for (s = 0; s < 32; s++)
                {
                    runLengthTable[s] = new int[] { s, m_decoder.ReadBits(4), 0, 0 };
                }

                runLengthTable[32] = new int[] { 0x103, m_decoder.ReadBits(4), 2, 0 };
                runLengthTable[33] = new int[] { 0x203, m_decoder.ReadBits(4), 3, 0 };
                runLengthTable[34] = new int[] { 0x20b, m_decoder.ReadBits(4), 7, 0 };
                runLengthTable[35] = new int[] { 0, 0, m_huffDecoder.jbig2HuffmanEOT };
                runLengthTable = m_huffDecoder.BuildTable(runLengthTable, 35);
                for (s = 0; s < noOfSymbols; s++)
                {
                    symbolCodeTable[s] = new int[] { s, 0, 0, 0 };
                }

                s = 0;
                while (s < noOfSymbols)
                {
                    int j = m_huffmanDecoder.DecodeInt(runLengthTable).IntResult;
                    if (j > 0x200)
                    {
                        for (j -= 0x200; j != 0 && s < noOfSymbols; j--)
                        {
                            symbolCodeTable[s++][1] = 0;
                        }
                    }
                    else if (j > 0x100)
                    {
                        for (j -= 0x100; j != 0 && s < noOfSymbols; j--)
                        {
                            symbolCodeTable[s][1] = symbolCodeTable[s - 1][1];
                            s++;
                        }
                    }
                    else
                    {
                        symbolCodeTable[s++][1] = j;
                    }
                }

                symbolCodeTable[noOfSymbols][1] = 0;
                symbolCodeTable[noOfSymbols][2] = m_huffDecoder.jbig2HuffmanEOT;
                symbolCodeTable = m_huffDecoder.BuildTable(symbolCodeTable, noOfSymbols);

                m_decoder.ConsumeRemainingBits();
            }
            else
            {
                symbolCodeTable = null;
                m_arithmeticDecoder.ResetIntegerStats(symbolCodeLength);
                m_arithmeticDecoder.Start();
            }

            bool symbolRefine = m_textRegionFlags.GetFlagValue(TextRegionFlags.SB_REFINE) != 0;
            int logStrips = m_textRegionFlags.GetFlagValue(TextRegionFlags.LOG_SB_STRIPES);
            int defaultPixel = m_textRegionFlags.GetFlagValue(TextRegionFlags.SB_DEF_PIXEL);
            int combinationOperator = m_textRegionFlags.GetFlagValue(TextRegionFlags.SB_COMB_OP);
            bool transposed = m_textRegionFlags.GetFlagValue(TextRegionFlags.TRANSPOSED) != 0;
            int referenceCorner = m_textRegionFlags.GetFlagValue(TextRegionFlags.REF_CORNER);
            int sOffset = m_textRegionFlags.GetFlagValue(TextRegionFlags.SB_DS_OFFSET);
            int template = m_textRegionFlags.GetFlagValue(TextRegionFlags.SB_R_TEMPLATE);

            if (symbolRefine)
            {
                m_arithmeticDecoder.ResetRefinementStats(template, null);
            }

            JBIG2Image bitmap = new JBIG2Image(regionBitmapWidth, regionBitmapHeight, m_arithmeticDecoder, m_huffmanDecoder, m_mmrDecoder);

            bitmap.ReadTextRegion(sbHuffman, symbolRefine, m_noOfSymbolInstances, logStrips, noOfSymbols, symbolCodeTable, symbolCodeLength, symbols, defaultPixel, combinationOperator, transposed, referenceCorner, sOffset, huffmanFSTable, huffmanDSTable, huffmanDTTable, huffmanRDWTable, huffmanRDHTable, huffmanRDXTable, huffmanRDYTable, huffmanRSizeTable, template, m_symbolRegionAdaptiveTemplateX, symbolRegionAdaptiveTemplateY, m_decoder);

            if (m_inlineImage)
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
            m_decoder.ConsumeRemainingBits();
        }

        private void ReadTextRegionFlags()
        {
            /// <summary>
            /// extract text region Segment flags </summary>
            short[] textRegionFlagsField = new short[2];
            m_decoder.ReadByte(textRegionFlagsField);

            int flags = m_bitOperation.GetInt16(textRegionFlagsField);
            m_textRegionFlags.setFlags(flags);
            bool sbHuff = m_textRegionFlags.GetFlagValue(TextRegionFlags.SB_HUFF) != 0;
            if (sbHuff)
            {
                /// <summary>
                /// extract text region Segment Huffman flags </summary>
                short[] textRegionHuffmanFlagsField = new short[2];
                m_decoder.ReadByte(textRegionHuffmanFlagsField);

                flags = m_bitOperation.GetInt16(textRegionHuffmanFlagsField);
                m_textRegionHuffmanFlags.setFlags(flags);
            }

            bool sbRefine = m_textRegionFlags.GetFlagValue(TextRegionFlags.SB_REFINE) != 0;
            int sbrTemplate = m_textRegionFlags.GetFlagValue(TextRegionFlags.SB_R_TEMPLATE);
            if (sbRefine && sbrTemplate == 0)
            {
                m_symbolRegionAdaptiveTemplateX[0] = ReadAtValue();
                symbolRegionAdaptiveTemplateY[0] = ReadAtValue();
                m_symbolRegionAdaptiveTemplateX[1] = ReadAtValue();
                symbolRegionAdaptiveTemplateY[1] = ReadAtValue();
            }
        }
    }

    internal class TextRegionFlags : JBIG2BaseFlags
    {
        public const string SB_HUFF = "SB_HUFF";
        public const string SB_REFINE = "SB_REFINE";
        public const string LOG_SB_STRIPES = "LOG_SB_STRIPES";
        public const string REF_CORNER = "REF_CORNER";
        public const string TRANSPOSED = "TRANSPOSED";
        public const string SB_COMB_OP = "SB_COMB_OP";
        public const string SB_DEF_PIXEL = "SB_DEF_PIXEL";
        public const string SB_DS_OFFSET = "SB_DS_OFFSET";
        public const string SB_R_TEMPLATE = "SB_R_TEMPLATE";

        public override void setFlags(int flagsAsInt)
        {
            this.flagsAsInt = flagsAsInt;

            /// <summary>
            /// extract SB_HUFF </summary>
            flags.Add(SB_HUFF, new int?(flagsAsInt & 1));

            /// <summary>
            /// extract SB_REFINE </summary>
            flags.Add(SB_REFINE, new int?((flagsAsInt >> 1) & 1));

            /// <summary>
            /// extract LOG_SB_STRIPES </summary>
            flags.Add(LOG_SB_STRIPES, new int?((flagsAsInt >> 2) & 3));

            /// <summary>
            /// extract REF_CORNER </summary>
            flags.Add(REF_CORNER, new int?((flagsAsInt >> 4) & 3));

            /// <summary>
            /// extract TRANSPOSED </summary>
            flags.Add(TRANSPOSED, new int?((flagsAsInt >> 6) & 1));

            /// <summary>
            /// extract SB_COMB_OP </summary>
            flags.Add(SB_COMB_OP, new int?((flagsAsInt >> 7) & 3));

            /// <summary>
            /// extract SB_DEF_PIXEL </summary>
            flags.Add(SB_DEF_PIXEL, new int?((flagsAsInt >> 9) & 1));

            int sOffset = (flagsAsInt >> 10) & 0x1f;
            if ((sOffset & 0x10) != 0)
            {
                sOffset |= -1 - 0x0f;
            }
            flags.Add(SB_DS_OFFSET, new int?(sOffset));

            /// <summary>
            /// extract SB_R_TEMPLATE </summary>
            flags.Add(SB_R_TEMPLATE, new int?((flagsAsInt >> 15) & 1));
        }

    }

    internal class TextRegionHuffmanFlags : JBIG2BaseFlags
    {
        public const string SB_HUFF_FS = "SB_HUFF_FS";
        public const string SB_HUFF_DS = "SB_HUFF_DS";
        public const string SB_HUFF_DT = "SB_HUFF_DT";
        public const string SB_HUFF_RDW = "SB_HUFF_RDW";
        public const string SB_HUFF_RDH = "SB_HUFF_RDH";
        public const string SB_HUFF_RDX = "SB_HUFF_RDX";
        public const string SB_HUFF_RDY = "SB_HUFF_RDY";
        public const string SB_HUFF_RSIZE = "SB_HUFF_RSIZE";

        public override void setFlags(int flagsAsInt)
        {
            this.flagsAsInt = flagsAsInt;

            /// <summary>
            /// extract SB_HUFF_FS </summary>
            flags.Add(SB_HUFF_FS, new int?(flagsAsInt & 3));

            /// <summary>
            /// extract SB_HUFF_DS </summary>
            flags.Add(SB_HUFF_DS, new int?((flagsAsInt >> 2) & 3));

            /// <summary>
            /// extract SB_HUFF_DT </summary>
            flags.Add(SB_HUFF_DT, new int?((flagsAsInt >> 4) & 3));

            /// <summary>
            /// extract SB_HUFF_RDW </summary>
            flags.Add(SB_HUFF_RDW, new int?((flagsAsInt >> 6) & 3));

            /// <summary>
            /// extract SB_HUFF_RDH </summary>
            flags.Add(SB_HUFF_RDH, new int?((flagsAsInt >> 8) & 3));

            /// <summary>
            /// extract SB_HUFF_RDX </summary>
            flags.Add(SB_HUFF_RDX, new int?((flagsAsInt >> 10) & 3));

            /// <summary>
            /// extract SB_HUFF_RDY </summary>
            flags.Add(SB_HUFF_RDY, new int?((flagsAsInt >> 12) & 3));

            /// <summary>
            /// extract SB_HUFF_RSIZE </summary>
            flags.Add(SB_HUFF_RSIZE, new int?((flagsAsInt >> 14) & 1));
        }
    }
}

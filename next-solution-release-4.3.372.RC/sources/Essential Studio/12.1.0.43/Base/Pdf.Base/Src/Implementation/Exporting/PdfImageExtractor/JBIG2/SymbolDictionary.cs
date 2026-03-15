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
    class SymbolDictionarySegment : JBIG2Segment
    {
        private int m_noOfExportedSymbols;
        private int m_noOfNewSymbols;
        private JBIG2Image[] bitmaps;
        private SymbolDictionaryFlags m_symbolDictionaryFlags = new SymbolDictionaryFlags();
        private ArithmeticDecoderStats m_genericRegionStats;
        private ArithmeticDecoderStats m_refinementRegionStats;
        private HuffmanDecoder m_huffDecoder = new HuffmanDecoder();
        private BitOperation m_bitOperation = new BitOperation();
        private RectangularArrays m_rectangularArrays = new RectangularArrays();
        private short[] m_symbolDictionaryAdaptiveTemplateX = new short[4], m_symbolDictionaryAdaptiveTemplateY = new short[4];
        private short[] m_symbolDictionaryRAdaptiveTemplateX = new short[2], m_symbolDictionaryRAdaptiveTemplateY = new short[2];

        internal int NoOfExportedSymbols
        {
            get
            {
                return m_noOfExportedSymbols;
            }
            set
            {
                m_noOfExportedSymbols = value;
            }
        }

        public SymbolDictionarySegment(JBIG2StreamDecoder streamDecoder)
            : base(streamDecoder)
        {

        }
                
        public override void readSegment()
        {
            /// <summary>
            /// read symbol dictionary flags </summary>
            ReadSymbolDictionaryFlags();

            IList codeTables = new List<object>();
            int numberOfInputSymbols = 0;
            int noOfReferredToSegments = m_segmentHeader.ReferedToSegCount;
            int[] referredToSegments = m_segmentHeader.ReferredToSegments;

            for (int i = 0; i < noOfReferredToSegments; i++)
            {
                JBIG2Segment seg = m_decoder.FindSegment(referredToSegments[i]);
                int type = seg.m_segmentHeader.SegmentType;

                if (type == JBIG2Segment.SYMBOL_DICTIONARY)
                {
                    numberOfInputSymbols += ((SymbolDictionarySegment)seg).m_noOfExportedSymbols;
                }
                else if (type == JBIG2Segment.TABLES)
                {
                    codeTables.Add(seg);
                }
            }

            int symbolCodeLength = 0;
            int s = 1;
            while (s < numberOfInputSymbols + m_noOfNewSymbols)
            {
                symbolCodeLength++;
                s <<= 1;
            }

            JBIG2Image[] bitmaps = new JBIG2Image[numberOfInputSymbols + m_noOfNewSymbols];

            int k = 0;
            SymbolDictionarySegment inputSymbolDictionary = null;
            for (s = 0; s < noOfReferredToSegments; s++)
            {
                JBIG2Segment seg = m_decoder.FindSegment(referredToSegments[s]);
                if (seg.m_segmentHeader.SegmentType == JBIG2Segment.SYMBOL_DICTIONARY)
                {
                    inputSymbolDictionary = (SymbolDictionarySegment)seg;
                    for (int j = 0; j < inputSymbolDictionary.m_noOfExportedSymbols; j++)
                    {
                        bitmaps[k++] = inputSymbolDictionary.bitmaps[j];
                    }
                }
            }

            int[][] huffmanDHTable = null;
            int[][] huffmanDWTable = null;

            int[][] huffmanBMSizeTable = null;
            int[][] huffmanAggInstTable = null;

            bool sdHuffman = m_symbolDictionaryFlags.GetFlagValue(SymbolDictionaryFlags.SD_HUFF) != 0;
            int sdHuffmanDifferenceHeight = m_symbolDictionaryFlags.GetFlagValue(SymbolDictionaryFlags.SD_HUFF_DH);
            int sdHuffmanDiferrenceWidth = m_symbolDictionaryFlags.GetFlagValue(SymbolDictionaryFlags.SD_HUFF_DW);
            int sdHuffBitmapSize = m_symbolDictionaryFlags.GetFlagValue(SymbolDictionaryFlags.SD_HUFF_BM_SIZE);
            int sdHuffAggregationInstances = m_symbolDictionaryFlags.GetFlagValue(SymbolDictionaryFlags.SD_HUFF_AGG_INST);

            s = 0;
            if (sdHuffman)
            {
                if (sdHuffmanDifferenceHeight == 0)
                {
                    huffmanDHTable = m_huffDecoder.huffmanTableD;
                }
                else if (sdHuffmanDifferenceHeight == 1)
                {
                    huffmanDHTable = m_huffDecoder.huffmanTableE;
                }
                else
                {
                    huffmanDHTable = null;
                }

                if (sdHuffmanDiferrenceWidth == 0)
                {
                    huffmanDWTable = m_huffDecoder.huffmanTableB;
                }
                else if (sdHuffmanDiferrenceWidth == 1)
                {
                    huffmanDWTable = m_huffDecoder.huffmanTableC;
                }
                else
                {
                    huffmanDWTable = null;
                }

                if (sdHuffBitmapSize == 0)
                {
                    huffmanBMSizeTable = m_huffDecoder.huffmanTableA;
                }
                else
                {
                    huffmanBMSizeTable = null;
                }

                if (sdHuffAggregationInstances == 0)
                {
                    huffmanAggInstTable = m_huffDecoder.huffmanTableA;
                }
                else
                {
                    huffmanAggInstTable = null;
                }
            }

            int contextUsed = m_symbolDictionaryFlags.GetFlagValue(SymbolDictionaryFlags.BITMAP_CC_USED);
            int sdTemplate = m_symbolDictionaryFlags.GetFlagValue(SymbolDictionaryFlags.SD_TEMPLATE);

            if (!sdHuffman)
            {
                if (contextUsed != 0 && inputSymbolDictionary != null)
                {
                    m_arithmeticDecoder.ResetGenericStats(sdTemplate, inputSymbolDictionary.m_genericRegionStats);
                }
                else
                {
                    m_arithmeticDecoder.ResetGenericStats(sdTemplate, null);
                }
                m_arithmeticDecoder.ResetIntegerStats(symbolCodeLength);
                m_arithmeticDecoder.Start();
            }
            int sdRefinementAggregate = m_symbolDictionaryFlags.GetFlagValue(SymbolDictionaryFlags.SD_REF_AGG);
            int sdRefinementTemplate = m_symbolDictionaryFlags.GetFlagValue(SymbolDictionaryFlags.SD_R_TEMPLATE);
            if (sdRefinementAggregate != 0)
            {
                if (contextUsed != 0 && inputSymbolDictionary != null)
                {
                    m_arithmeticDecoder.ResetRefinementStats(sdRefinementTemplate, inputSymbolDictionary.m_refinementRegionStats);
                }
                else
                {
                    m_arithmeticDecoder.ResetRefinementStats(sdRefinementTemplate, null);
                }
            }

            int[] deltaWidths = new int[m_noOfNewSymbols];

            int deltaHeight = 0;
            s = 0;

            while (s < m_noOfNewSymbols)
            {

                int instanceDeltaHeight = 0;

                if (sdHuffman)
                {
                    instanceDeltaHeight = m_huffmanDecoder.DecodeInt(huffmanDHTable).IntResult;
                }
                else
                {
                    instanceDeltaHeight = m_arithmeticDecoder.DecodeInt(m_arithmeticDecoder.IadhStats).IntResult;
                }

                deltaHeight += instanceDeltaHeight;
                int symbolWidth = 0;
                int totalWidth = 0;
                int j = s;

                while (true)
                {
                    int deltaWidth = 0;

                    DecodeIntResult decodeIntResult;
                    if (sdHuffman)
                    {
                        decodeIntResult = m_huffmanDecoder.DecodeInt(huffmanDWTable);
                    }
                    else
                    {
                        decodeIntResult = m_arithmeticDecoder.DecodeInt(m_arithmeticDecoder.IadwStats);
                    }

                    if (!decodeIntResult.BooleanResult)
                    {
                        break;
                    }

                    deltaWidth = decodeIntResult.IntResult;
                    symbolWidth += deltaWidth;

                    if (sdHuffman && sdRefinementAggregate == 0)
                    {
                        deltaWidths[s] = symbolWidth;
                        totalWidth += symbolWidth;

                    }
                    else if (sdRefinementAggregate == 1)
                    {
                        int refAggNum = 0;

                        if (sdHuffman)
                        {
                            refAggNum = m_huffmanDecoder.DecodeInt(huffmanAggInstTable).IntResult;
                        }
                        else
                        {
                            refAggNum = m_arithmeticDecoder.DecodeInt(m_arithmeticDecoder.IaaiStats).IntResult;
                        }
                        if (refAggNum == 1)
                        {

                            int symbolID = 0, referenceDX = 0, referenceDY = 0;

                            if (sdHuffman)
                            {
                                symbolID = m_decoder.ReadBits(symbolCodeLength);
                                referenceDX = m_huffmanDecoder.DecodeInt(m_huffDecoder.huffmanTableO).IntResult;
                                referenceDY = m_huffmanDecoder.DecodeInt(m_huffDecoder.huffmanTableO).IntResult;

                                m_decoder.ConsumeRemainingBits();
                                m_arithmeticDecoder.Start();
                            }
                            else
                            {
                                symbolID = (int)m_arithmeticDecoder.DecodeIAID(symbolCodeLength, m_arithmeticDecoder.IaidStats);
                                referenceDX = m_arithmeticDecoder.DecodeInt(m_arithmeticDecoder.IardxStats).IntResult;
                                referenceDY = m_arithmeticDecoder.DecodeInt(m_arithmeticDecoder.IardyStats).IntResult;
                            }

                            JBIG2Image referredToBitmap = bitmaps[symbolID];

                            JBIG2Image bitmap = new JBIG2Image(symbolWidth, deltaHeight, m_arithmeticDecoder, m_huffmanDecoder, m_mmrDecoder);
                            bitmap.ReadGenericRefinementRegion(sdRefinementTemplate, false, referredToBitmap, referenceDX, referenceDY, m_symbolDictionaryRAdaptiveTemplateX, m_symbolDictionaryRAdaptiveTemplateY);

                            bitmaps[numberOfInputSymbols + s] = bitmap;

                        }
                        else
                        {
                            JBIG2Image bitmap = new JBIG2Image(symbolWidth, deltaHeight, m_arithmeticDecoder, m_huffmanDecoder, m_mmrDecoder);
                            bitmap.ReadTextRegion(sdHuffman, true, refAggNum, 0, numberOfInputSymbols + s, null, symbolCodeLength, bitmaps, 0, 0, false, 1, 0, m_huffDecoder.huffmanTableF, m_huffDecoder.huffmanTableH, m_huffDecoder.huffmanTableK, m_huffDecoder.huffmanTableO, m_huffDecoder.huffmanTableO, m_huffDecoder.huffmanTableO, m_huffDecoder.huffmanTableO, m_huffDecoder.huffmanTableA, sdRefinementTemplate, m_symbolDictionaryRAdaptiveTemplateX, m_symbolDictionaryRAdaptiveTemplateY, m_decoder);

                            bitmaps[numberOfInputSymbols + s] = bitmap;
                        }
                    }
                    else
                    {
                        JBIG2Image bitmap = new JBIG2Image(symbolWidth, deltaHeight, m_arithmeticDecoder, m_huffmanDecoder, m_mmrDecoder);
                        bitmap.ReadBitmap(false, sdTemplate, false, false, null, m_symbolDictionaryAdaptiveTemplateX, m_symbolDictionaryAdaptiveTemplateY, 0);
                        bitmaps[numberOfInputSymbols + s] = bitmap;
                    }

                    s++;
                }

                if (sdHuffman && sdRefinementAggregate == 0)
                {
                    int bmSize = m_huffmanDecoder.DecodeInt(huffmanBMSizeTable).IntResult;
                    m_decoder.ConsumeRemainingBits();

                    JBIG2Image collectiveBitmap = new JBIG2Image(totalWidth, deltaHeight, m_arithmeticDecoder, m_huffmanDecoder, m_mmrDecoder);

                    if (bmSize == 0)
                    {

                        int padding = totalWidth % 8;
                        int bytesPerRow = (int)Math.Ceiling(totalWidth / 8d);

                        int size = deltaHeight * ((totalWidth + 7) >> 3);
                        short[] bitmap = new short[size];
                        m_decoder.ReadByte(bitmap);
                        short[][] logicalMap = m_rectangularArrays.ReturnRectangularShortArray(deltaHeight, bytesPerRow);
                        int count = 0;
                        for (int row = 0; row < deltaHeight; row++)
                        {
                            for (int col = 0; col < bytesPerRow; col++)
                            {
                                logicalMap[row][col] = bitmap[count];
                                count++;
                            }
                        }

                        int collectiveBitmapRow = 0, collectiveBitmapCol = 0;

                        for (int row = 0; row < deltaHeight; row++)
                        {
                            for (int col = 0; col < bytesPerRow; col++)
                            {
                                if (col == (bytesPerRow - 1)) // this is the last
                                {
                                    // byte in the row
                                    short currentByte = logicalMap[row][col];
                                    for (int bitPointer = 7; bitPointer >= padding; bitPointer--)
                                    {
                                        short mask = (short)(1 << bitPointer);
                                        int bit = (currentByte & mask) >> bitPointer;

                                        collectiveBitmap.SetPixel(collectiveBitmapCol, collectiveBitmapRow, bit);
                                        collectiveBitmapCol++;
                                    }
                                    collectiveBitmapRow++;
                                    collectiveBitmapCol = 0;
                                }
                                else
                                {
                                    short currentByte = logicalMap[row][col];
                                    for (int bitPointer = 7; bitPointer >= 0; bitPointer--)
                                    {
                                        short mask = (short)(1 << bitPointer);
                                        int bit = (currentByte & mask) >> bitPointer;

                                        collectiveBitmap.SetPixel(collectiveBitmapCol, collectiveBitmapRow, bit);
                                        collectiveBitmapCol++;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        collectiveBitmap.ReadBitmap(true, 0, false, false, null, null, null, bmSize);
                    }

                    int x = 0;
                    while (j < s)
                    {
                        bitmaps[numberOfInputSymbols + j] = collectiveBitmap.GetSlice(x, 0, deltaWidths[j], deltaHeight);
                        x += deltaWidths[j];

                        j++;
                    }
                }
            }

            this.bitmaps = new JBIG2Image[m_noOfExportedSymbols];

            int g = s = 0;
            bool export = false;
            while (s < numberOfInputSymbols + m_noOfNewSymbols)
            {

                int run = 0;
                if (sdHuffman)
                {
                    run = m_huffmanDecoder.DecodeInt(m_huffDecoder.huffmanTableA).IntResult;
                }
                else
                {
                    run = m_arithmeticDecoder.DecodeInt(m_arithmeticDecoder.IaexStats).IntResult;
                }

                if (export)
                {
                    for (int cnt = 0; cnt < run; cnt++)
                    {
                        this.bitmaps[g++] = bitmaps[s++];
                    }
                }
                else
                {
                    s += run;
                }

                export = !export;
            }

            int contextRetained = m_symbolDictionaryFlags.GetFlagValue(SymbolDictionaryFlags.BITMAP_CC_RETAINED);
            if (!sdHuffman && contextRetained == 1)
            {
                m_genericRegionStats = m_genericRegionStats.copy();
                if (sdRefinementAggregate == 1)
                {
                    m_refinementRegionStats = m_refinementRegionStats.copy();
                }
            }

            /// <summary>
            /// consume any remaining bits </summary>
            m_decoder.ConsumeRemainingBits();
        }

        private void ReadSymbolDictionaryFlags()
        {
            /// <summary>
            /// extract symbol dictionary flags </summary>
            short[] symbolDictionaryFlagsField = new short[2];
            m_decoder.ReadByte(symbolDictionaryFlagsField);

            int flags = m_bitOperation.GetInt16(symbolDictionaryFlagsField);
            m_symbolDictionaryFlags.setFlags(flags);

            int sdHuff = m_symbolDictionaryFlags.GetFlagValue(SymbolDictionaryFlags.SD_HUFF);
            int sdTemplate = m_symbolDictionaryFlags.GetFlagValue(SymbolDictionaryFlags.SD_TEMPLATE);
            if (sdHuff == 0)
            {
                if (sdTemplate == 0)
                {
                    m_symbolDictionaryAdaptiveTemplateX[0] = ReadAtValue();
                    m_symbolDictionaryAdaptiveTemplateY[0] = ReadAtValue();
                    m_symbolDictionaryAdaptiveTemplateX[1] = ReadAtValue();
                    m_symbolDictionaryAdaptiveTemplateY[1] = ReadAtValue();
                    m_symbolDictionaryAdaptiveTemplateX[2] = ReadAtValue();
                    m_symbolDictionaryAdaptiveTemplateY[2] = ReadAtValue();
                    m_symbolDictionaryAdaptiveTemplateX[3] = ReadAtValue();
                    m_symbolDictionaryAdaptiveTemplateY[3] = ReadAtValue();
                }
                else
                {
                    m_symbolDictionaryAdaptiveTemplateX[0] = ReadAtValue();
                    m_symbolDictionaryAdaptiveTemplateY[0] = ReadAtValue();
                }
            }

            // symbol dictionary refinement AT flags
            int refAgg = m_symbolDictionaryFlags.GetFlagValue(SymbolDictionaryFlags.SD_REF_AGG);
            int sdrTemplate = m_symbolDictionaryFlags.GetFlagValue(SymbolDictionaryFlags.SD_R_TEMPLATE);
            if (refAgg != 0 && sdrTemplate == 0)
            {
                m_symbolDictionaryRAdaptiveTemplateX[0] = ReadAtValue();
                m_symbolDictionaryRAdaptiveTemplateY[0] = ReadAtValue();
                m_symbolDictionaryRAdaptiveTemplateX[1] = ReadAtValue();
                m_symbolDictionaryRAdaptiveTemplateY[1] = ReadAtValue();
            }

            /// <summary>
            /// extract no of exported symbols </summary>
            short[] noOfExportedSymbolsField = new short[4];
            m_decoder.ReadByte(noOfExportedSymbolsField);

            int noOfExportedSymbols = m_bitOperation.GetInt32(noOfExportedSymbolsField);
            this.m_noOfExportedSymbols = noOfExportedSymbols;
            /** extract no of new symbols */
            short[] noOfNewSymbolsField = new short[4];
            m_decoder.ReadByte(noOfNewSymbolsField);

            int noOfNewSymbols = m_bitOperation.GetInt32(noOfNewSymbolsField);
            this.m_noOfNewSymbols = noOfNewSymbols;
        }

        public JBIG2Image[] getBitmaps()
        {
            return bitmaps;
        }        
    }

    class SymbolDictionaryFlags : JBIG2BaseFlags
    {
        public const string SD_HUFF = "SD_HUFF";
        public const string SD_REF_AGG = "SD_REF_AGG";
        public const string SD_HUFF_DH = "SD_HUFF_DH";
        public const string SD_HUFF_DW = "SD_HUFF_DW";
        public const string SD_HUFF_BM_SIZE = "SD_HUFF_BM_SIZE";
        public const string SD_HUFF_AGG_INST = "SD_HUFF_AGG_INST";
        public const string BITMAP_CC_USED = "BITMAP_CC_USED";
        public const string BITMAP_CC_RETAINED = "BITMAP_CC_RETAINED";
        public const string SD_TEMPLATE = "SD_TEMPLATE";
        public const string SD_R_TEMPLATE = "SD_R_TEMPLATE";

        public override void setFlags(int flagsAsInt)
        {
            this.flagsAsInt = flagsAsInt;

            /// <summary>
            /// extract SD_HUFF </summary>
            flags.Add(SD_HUFF, new int?(flagsAsInt & 1));

            /// <summary>
            /// extract SD_REF_AGG </summary>
            flags.Add(SD_REF_AGG, new int?((flagsAsInt >> 1) & 1));

            /// <summary>
            /// extract SD_HUFF_DH </summary>
            flags.Add(SD_HUFF_DH, new int?((flagsAsInt >> 2) & 3));

            /// <summary>
            /// extract SD_HUFF_DW </summary>
            flags.Add(SD_HUFF_DW, new int?((flagsAsInt >> 4) & 3));

            /// <summary>
            /// extract SD_HUFF_BM_SIZE </summary>
            flags.Add(SD_HUFF_BM_SIZE, new int?((flagsAsInt >> 6) & 1));

            /// <summary>
            /// extract SD_HUFF_AGG_INST </summary>
            flags.Add(SD_HUFF_AGG_INST, new int?((flagsAsInt >> 7) & 1));

            /// <summary>
            /// extract BITMAP_CC_USED </summary>
            flags.Add(BITMAP_CC_USED, new int?((flagsAsInt >> 8) & 1));

            /// <summary>
            /// extract BITMAP_CC_RETAINED </summary>
            flags.Add(BITMAP_CC_RETAINED, new int?((flagsAsInt >> 9) & 1));

            /// <summary>
            /// extract SD_TEMPLATE </summary>
            flags.Add(SD_TEMPLATE, new int?((flagsAsInt >> 10) & 3));

            /// <summary>
            /// extract SD_R_TEMPLATE </summary>
            flags.Add(SD_R_TEMPLATE, new int?((flagsAsInt >> 12) & 1));
        }
    }
}

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
    class ArithmeticDecoder
    {
        private Jbig2StreamReader reader;
        private ArithmeticDecoderStats m_genericRegionStats, m_refinementRegionStats;
        private ArithmeticDecoderStats m_iadhStats, m_iadwStats, m_iaexStats, m_iaaiStats, m_iadtStats, m_iaitStats, m_iafsStats, m_iadsStats, m_iardxStats, m_iardyStats, m_iardwStats, m_iardhStats, m_iariStats, m_iaidStats;
        private BitOperation m_bitOperation = new BitOperation();
        private int[] m_contextSize = { 16, 13, 10, 10 }; internal int[] referredToContextSize = { 13, 10 };
        private long m_buffer0, m_buffer1;
        private long c, a;
        private long m_previous;
        private int m_counter;

        internal ArithmeticDecoderStats GenericRegionStats
        {
            get
            {
                return m_genericRegionStats;
            }
        }
        internal ArithmeticDecoderStats RefinementRegionStats
        {
            get
            {
                return m_refinementRegionStats;
            }
        }

        internal ArithmeticDecoderStats IadhStats
        {
            get
            {
                return m_iadhStats;
            }
        }

        internal ArithmeticDecoderStats IadwStats
        {
            get
            {
                return m_iadwStats;
            }
        }
        internal ArithmeticDecoderStats IaexStats
        {
            get
            {
                return m_iaexStats;
            }
        }
        internal ArithmeticDecoderStats IaaiStats
        {
            get
            {
                return m_iaaiStats;
            }
        }
        internal ArithmeticDecoderStats IadtStats
        {
            get
            {
                return m_iadtStats;
            }
        }
        internal ArithmeticDecoderStats IaitStats
        {
            get
            {
                return m_iaitStats;
            }
        }
        internal ArithmeticDecoderStats IafsStats
        {
            get
            {
                return m_iafsStats;
            }
        }
        internal ArithmeticDecoderStats IadsStats
        {
            get
            {
                return m_iadsStats;
            }
        }
        internal ArithmeticDecoderStats IardxStats
        {
            get
            {
                return m_iardxStats;
            }
        }
        internal ArithmeticDecoderStats IardyStats
        {
            get
            {
                return m_iardyStats;
            }
        }
        internal ArithmeticDecoderStats IardwStats
        {
            get
            {
                return m_iardwStats;
            }
        }
        internal ArithmeticDecoderStats IardhStats
        {
            get
            {
                return m_iardhStats;
            }
        }
        internal ArithmeticDecoderStats IariStats
        {
            get
            {
                return m_iariStats;
            }
        }
        internal ArithmeticDecoderStats IaidStats
        {
            get
            {
                return m_iaidStats;
            }
        }
        
        private ArithmeticDecoder()
        {
        }

        internal ArithmeticDecoder(Jbig2StreamReader reader)
        {
            this.reader = reader;

            m_genericRegionStats = new ArithmeticDecoderStats(1 << 1);
            m_refinementRegionStats = new ArithmeticDecoderStats(1 << 1);

            m_iadhStats = new ArithmeticDecoderStats(1 << 9);
            m_iadwStats = new ArithmeticDecoderStats(1 << 9);
            m_iaexStats = new ArithmeticDecoderStats(1 << 9);
            m_iaaiStats = new ArithmeticDecoderStats(1 << 9);
            m_iadtStats = new ArithmeticDecoderStats(1 << 9);
            m_iaitStats = new ArithmeticDecoderStats(1 << 9);
            m_iafsStats = new ArithmeticDecoderStats(1 << 9);
            m_iadsStats = new ArithmeticDecoderStats(1 << 9);
            m_iardxStats = new ArithmeticDecoderStats(1 << 9);
            m_iardyStats = new ArithmeticDecoderStats(1 << 9);
            m_iardwStats = new ArithmeticDecoderStats(1 << 9);
            m_iardhStats = new ArithmeticDecoderStats(1 << 9);
            m_iariStats = new ArithmeticDecoderStats(1 << 9);
            m_iaidStats = new ArithmeticDecoderStats(1 << 1);
        }

        internal void ResetIntegerStats(int symbolCodeLength)
        {
            m_iadhStats.reset();
            m_iadwStats.reset();
            m_iaexStats.reset();
            m_iaaiStats.reset();
            m_iadtStats.reset();
            m_iaitStats.reset();
            m_iafsStats.reset();
            m_iadsStats.reset();
            m_iardxStats.reset();
            m_iardyStats.reset();
            m_iardwStats.reset();
            m_iardhStats.reset();
            m_iariStats.reset();

            if (m_iaidStats.ContextSize == 1 << (symbolCodeLength + 1))
            {
                m_iaidStats.reset();
            }
            else
            {
                m_iaidStats = new ArithmeticDecoderStats(1 << (symbolCodeLength + 1));
            }
        }

        internal void ResetGenericStats(int template, ArithmeticDecoderStats previousStats)
        {
            int size = m_contextSize[template];

            if (previousStats != null && previousStats.ContextSize == size)
            {
                if (m_genericRegionStats.ContextSize == size)
                {
                    m_genericRegionStats.overwrite(previousStats);
                }
                else
                {
                    m_genericRegionStats = previousStats.copy();
                }
            }
            else
            {
                if (m_genericRegionStats.ContextSize == size)
                {
                    m_genericRegionStats.reset();
                }
                else
                {
                    m_genericRegionStats = new ArithmeticDecoderStats(1 << size);
                }
            }
        }

        internal void ResetRefinementStats(int template, ArithmeticDecoderStats previousStats)
        {
            int size = referredToContextSize[template];
            if (previousStats != null && previousStats.ContextSize == size)
            {
                if (m_refinementRegionStats.ContextSize == size)
                {
                    m_refinementRegionStats.overwrite(previousStats);
                }
                else
                {
                    m_refinementRegionStats = previousStats.copy();
                }
            }
            else
            {
                if (m_refinementRegionStats.ContextSize == size)
                {
                    m_refinementRegionStats.reset();
                }
                else
                {
                    m_refinementRegionStats = new ArithmeticDecoderStats(1 << size);
                }
            }
        }

        internal void Start()
        {
            m_buffer0 = reader.ReadByte();
            m_buffer1 = reader.ReadByte();

            c = m_bitOperation.Bit32Shift((m_buffer0 ^ 0xff), 16, BitOperation.LEFT_SHIFT);
            ReadByte();
            c = m_bitOperation.Bit32Shift(c, 7, BitOperation.LEFT_SHIFT);
            m_counter -= 7;
            a = 0x80000000L;
        }

        internal DecodeIntResult DecodeInt(ArithmeticDecoderStats stats)
        {
            long value;

            m_previous = 1;
            int s = DecodeIntBit(stats);
            if (DecodeIntBit(stats) != 0)
            {
                if (DecodeIntBit(stats) != 0)
                {
                    if (DecodeIntBit(stats) != 0)
                    {
                        if (DecodeIntBit(stats) != 0)
                        {
                            if (DecodeIntBit(stats) != 0)
                            {
                                value = 0;
                                for (int i = 0; i < 32; i++)
                                {
                                    value = m_bitOperation.Bit32Shift(value, 1, BitOperation.LEFT_SHIFT) | DecodeIntBit(stats);
                                }
                                value += 4436;
                            }
                            else
                            {
                                value = 0;
                                for (int i = 0; i < 12; i++)
                                {
                                    value = m_bitOperation.Bit32Shift(value, 1, BitOperation.LEFT_SHIFT) | DecodeIntBit(stats);
                                }
                                value += 340;
                            }
                        }
                        else
                        {
                            value = 0;
                            for (int i = 0; i < 8; i++)
                            {
                                value = m_bitOperation.Bit32Shift(value, 1, BitOperation.LEFT_SHIFT) | DecodeIntBit(stats);
                            }
                            value += 84;
                        }
                    }
                    else
                    {
                        value = 0;
                        for (int i = 0; i < 6; i++)
                        {
                            value = m_bitOperation.Bit32Shift(value, 1, BitOperation.LEFT_SHIFT) | DecodeIntBit(stats);
                        }
                        value += 20;
                    }
                }
                else
                {
                    value = DecodeIntBit(stats);
                    value = m_bitOperation.Bit32Shift(value, 1, BitOperation.LEFT_SHIFT) | DecodeIntBit(stats);
                    value = m_bitOperation.Bit32Shift(value, 1, BitOperation.LEFT_SHIFT) | DecodeIntBit(stats);
                    value = m_bitOperation.Bit32Shift(value, 1, BitOperation.LEFT_SHIFT) | DecodeIntBit(stats);
                    value += 4;
                }
            }
            else
            {
                value = DecodeIntBit(stats);
                value = m_bitOperation.Bit32Shift(value, 1, BitOperation.LEFT_SHIFT) | DecodeIntBit(stats);
            }
            int decodedInt;
            if (s != 0)
            {
                if (value == 0)
                {
                    return new DecodeIntResult((int)value, false);
                }
                decodedInt = (int)-value;
            }
            else
            {
                decodedInt = (int)value;
            }
            return new DecodeIntResult(decodedInt, true);
        }

        internal long DecodeIAID(long codeLen, ArithmeticDecoderStats stats)
        {
            m_previous = 1;
            for (long i = 0; i < codeLen; i++)
            {
                int bit = DecodeBit(m_previous, stats);
                m_previous = m_bitOperation.Bit32Shift(m_previous, 1, BitOperation.LEFT_SHIFT) | bit;
            }
            return m_previous - (1 << (int)codeLen);
        }

        internal int DecodeBit(long context, ArithmeticDecoderStats stats)
        {
            int iCX = m_bitOperation.Bit8Shift(stats.getContextCodingTableValue((int)context), 1, BitOperation.RIGHT_SHIFT);
            int mpsCX = stats.getContextCodingTableValue((int)context) & 1;
            int qe = qeTable[iCX];

            a -= qe;

            int bit;
            if (c < a)
            {
                if ((a & 0x80000000) != 0)
                {
                    bit = mpsCX;
                }
                else
                {
                    if (a < qe)
                    {
                        bit = 1 - mpsCX;
                        if (switchTable[iCX] != 0)
                        {
                            stats.setContextCodingTableValue((int)context, (nlpsTable[iCX] << 1) | (1 - mpsCX));
                        }
                        else
                        {
                            stats.setContextCodingTableValue((int)context, (nlpsTable[iCX] << 1) | mpsCX);
                        }
                    }
                    else
                    {
                        bit = mpsCX;
                        stats.setContextCodingTableValue((int)context, (nmpsTable[iCX] << 1) | mpsCX);
                    }
                    do
                    {
                        if (m_counter == 0)
                        {
                            ReadByte();
                        }
                        a = m_bitOperation.Bit32Shift(a, 1, BitOperation.LEFT_SHIFT);
                        c = m_bitOperation.Bit32Shift(c, 1, BitOperation.LEFT_SHIFT);
                        m_counter--;
                    } while ((a & 0x80000000) == 0);
                }
            }
            else
            {
                c -= a;

                if (a < qe)
                {
                    bit = mpsCX;
                    stats.setContextCodingTableValue((int)context, (nmpsTable[iCX] << 1) | mpsCX);
                }
                else
                {
                    bit = 1 - mpsCX;
                    if (switchTable[iCX] != 0)
                    {
                        stats.setContextCodingTableValue((int)context, (nlpsTable[iCX] << 1) | (1 - mpsCX));
                    }
                    else
                    {
                        stats.setContextCodingTableValue((int)context, (nlpsTable[iCX] << 1) | mpsCX);
                    }
                }
                a = qe;

                do
                {
                    if (m_counter == 0)
                    {
                        ReadByte();
                    }
                    a = m_bitOperation.Bit32Shift(a, 1, BitOperation.LEFT_SHIFT);
                    c = m_bitOperation.Bit32Shift(c, 1, BitOperation.LEFT_SHIFT);
                    m_counter--;
                } while ((a & 0x80000000) == 0);
            }
            return bit;
        }

        private void ReadByte()
        {
            if (m_buffer0 == 0xff)
            {
                if (m_buffer1 > 0x8f)
                {
                    m_counter = 8;
                }
                else
                {
                    m_buffer0 = m_buffer1;
                    m_buffer1 = reader.ReadByte();
                    c = c + 0xfe00 - (m_bitOperation.Bit32Shift(m_buffer0, 9, BitOperation.LEFT_SHIFT));
                    m_counter = 7;
                }
            }
            else
            {
                m_buffer0 = m_buffer1;
                m_buffer1 = reader.ReadByte();
                c = c + 0xff00 - (m_bitOperation.Bit32Shift(m_buffer0, 8, BitOperation.LEFT_SHIFT));
                m_counter = 8;
            }
        }

        private int DecodeIntBit(ArithmeticDecoderStats stats)
        {
            int bit;
            bit = DecodeBit(m_previous, stats);
            if (m_previous < 0x100)
            {
                m_previous = m_bitOperation.Bit32Shift(m_previous, 1, BitOperation.LEFT_SHIFT) | bit;
            }
            else
            {
                m_previous = (((m_bitOperation.Bit32Shift(m_previous, 1, BitOperation.LEFT_SHIFT)) | bit) & 0x1ff) | 0x100;
            }
            return bit;
        }
        int[] qeTable = { 0x56010000, 0x34010000, 0x18010000, 0x0AC10000, 0x05210000, 0x02210000, 0x56010000, 0x54010000, 0x48010000, 0x38010000, 0x30010000, 0x24010000, 0x1C010000, 0x16010000, 0x56010000, 0x54010000, 0x51010000, 0x48010000, 0x38010000, 0x34010000, 0x30010000, 0x28010000, 0x24010000, 0x22010000, 0x1C010000, 0x18010000, 0x16010000, 0x14010000, 0x12010000, 0x11010000, 0x0AC10000, 0x09C10000, 0x08A10000, 0x05210000, 0x04410000, 0x02A10000, 0x02210000, 0x01410000, 0x01110000, 0x00850000, 0x00490000, 0x00250000, 0x00150000, 0x00090000, 0x00050000, 0x00010000, 0x56010000 };
        int[] nmpsTable = { 1, 2, 3, 4, 5, 38, 7, 8, 9, 10, 11, 12, 13, 29, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 45, 46 };
        int[] nlpsTable = { 1, 6, 9, 12, 29, 33, 6, 14, 14, 14, 17, 18, 20, 21, 14, 14, 15, 16, 17, 18, 19, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 46 };
        int[] switchTable = { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    }

    internal class ArithmeticDecoderStats
    {
        private int m_contextSize;
        private int[] m_codingContextTable;

        internal int ContextSize
        {
            get
            {
                return m_contextSize;
            }
        }
        internal ArithmeticDecoderStats(int contextSize)
        {
            this.m_contextSize = contextSize;
            this.m_codingContextTable = new int[contextSize];
            reset();
        }

        internal void reset()
        {
            for (int i = 0; i < m_contextSize; i++)
            {
                m_codingContextTable[i] = 0;
            }
        }

        internal void setEntry(int codingContext, int i, int moreProbableSymbol)
        {
            m_codingContextTable[codingContext] = (i << i) + moreProbableSymbol;
        }

        internal int getContextCodingTableValue(int index)
        {
            return m_codingContextTable[index];
        }

        internal void setContextCodingTableValue(int index, int value)
        {
            m_codingContextTable[index] = value;
        }

        internal void overwrite(ArithmeticDecoderStats stats)
        {
            System.Array.Copy(stats.m_codingContextTable, 0, m_codingContextTable, 0, m_contextSize);
        }

        internal ArithmeticDecoderStats copy()
        {
            ArithmeticDecoderStats stats = new ArithmeticDecoderStats(m_contextSize);
            System.Array.Copy(m_codingContextTable, 0, stats.m_codingContextTable, 0, m_contextSize);
            return stats;
        }
    }

    internal class DecodeIntResult
    {
        private int m_intResult;
        private bool m_booleanResult;

        internal DecodeIntResult(int intResult, bool booleanResult)
        {
            this.m_intResult = intResult;
            this.m_booleanResult = booleanResult;
        }

        internal int IntResult
        {
            get
            {
                return m_intResult;
            }
        }

        internal bool BooleanResult
        {
            get
            {
                return m_booleanResult;
            }
        }
    }
}

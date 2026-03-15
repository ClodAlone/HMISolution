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
    partial class CCITTCodec : TiffCodec
    {
        public const int FIELD_BADFAXLINES = (FieldBit.Codec + 0);
        public const int FIELD_CLEANFAXDATA = (FieldBit.Codec + 1);
        public const int FIELD_BADFAXRUN = (FieldBit.Codec + 2);
        public const int FIELD_RECVPARAMS = (FieldBit.Codec + 3);
        public const int FIELD_SUBADDRESS = (FieldBit.Codec + 4);
        public const int FIELD_RECVTIME = (FieldBit.Codec + 5);
        public const int FIELD_FAXDCS = (FieldBit.Codec + 6);
        public const int FIELD_OPTIONS = (FieldBit.Codec + 7);

        internal FaxMode m_mode; 
        internal Group3Opt m_groupoptions;
        internal CleanFaxData m_cleanfaxdata;
        internal int m_badfaxlines;
        internal int m_badfaxrun;
        internal int m_recvparams;
        internal string m_subaddress;
        internal int m_recvtime;
        internal string m_faxdcs;

        internal Tiff.FaxFillFunc fill;

        private const int EOL_CODE = 0x001;

        private const byte S_Null = 0;
        private const byte S_Pass = 1;
        private const byte S_Horiz = 2;
        private const byte S_V0 = 3;
        private const byte S_VR = 4;
        private const byte S_VL = 5;
        private const byte S_Ext = 6;
        private const byte S_TermW = 7;
        private const byte S_TermB = 8;
        private const byte S_MakeUpW = 9;
        private const byte S_MakeUpB = 10;
        private const byte S_MakeUp = 11;
        private const byte S_EOL = 12;

        private const short G3CODE_EOL = -1;
        private const short G3CODE_INVALID = -2;
        private const short G3CODE_EOF = -3;
        private const short G3CODE_INCOMP = -4;

        private struct tableEntry
        {
            public short length;
            public short code;
            public short runlen;

            public tableEntry(short _length, short _code, short _runlen)
            {
                length = _length;
                code = _code;
                runlen = _runlen;
            }

            public static tableEntry FromArray(short[] array, int entryNumber)
            {
                int offset = entryNumber * 3;
                return new tableEntry(array[offset], array[offset + 1], array[offset + 2]);
            }
        };

        private struct faxTableEntry
        {
            public faxTableEntry(byte _State, byte _Width, int _Param)
            {
                State = _State;
                Width = _Width;
                Param = _Param;
            }

            public static faxTableEntry FromArray(int[] array, int entryNumber)
            {
                int offset = entryNumber * 3;
                return new faxTableEntry((byte)array[offset], (byte)array[offset + 1], array[offset + 2]);
            }

            public byte State;
            public byte Width;
            public int Param;
        };

        private enum Decoder
        {
            useFax3_1DDecoder,
            useFax3_2DDecoder,
            useFax4Decoder,
            useFax3RLEDecoder
        };
        
        private enum Fax3Encoder
        {
            useFax1DEncoder, 
            useFax2DEncoder
        };

        private static readonly TiffFieldInfo[] m_faxFieldInfo =
        {
            new TiffFieldInfo(TiffTag.FAXMODE, 0, 0, TiffType.ANY, FieldBit.Pseudo, false, false, "FaxMode"), 
            new TiffFieldInfo(TiffTag.FAXFILLFUNC, 0, 0, TiffType.ANY, FieldBit.Pseudo, false, false, "FaxFillFunc"), 
            new TiffFieldInfo(TiffTag.BADFAXLINES, 1, 1, TiffType.LONG, FIELD_BADFAXLINES, true, false, "BadFaxLines"), 
            new TiffFieldInfo(TiffTag.BADFAXLINES, 1, 1, TiffType.SHORT, FIELD_BADFAXLINES, true, false, "BadFaxLines"), 
            new TiffFieldInfo(TiffTag.CLEANFAXDATA, 1, 1, TiffType.SHORT, FIELD_CLEANFAXDATA, true, false, "CleanFaxData"), 
            new TiffFieldInfo(TiffTag.CONSECUTIVEBADFAXLINES, 1, 1, TiffType.LONG, FIELD_BADFAXRUN, true, false, "ConsecutiveBadFaxLines"), 
            new TiffFieldInfo(TiffTag.CONSECUTIVEBADFAXLINES, 1, 1, TiffType.SHORT, FIELD_BADFAXRUN, true, false, "ConsecutiveBadFaxLines"), 
            new TiffFieldInfo(TiffTag.FAXRECVPARAMS, 1, 1, TiffType.LONG, FIELD_RECVPARAMS, true, false, "FaxRecvParams"), 
            new TiffFieldInfo(TiffTag.FAXSUBADDRESS, -1, -1, TiffType.ASCII, FIELD_SUBADDRESS, true, false, "FaxSubAddress"), 
            new TiffFieldInfo(TiffTag.FAXRECVTIME, 1, 1, TiffType.LONG, FIELD_RECVTIME, true, false, "FaxRecvTime"), 
            new TiffFieldInfo(TiffTag.FAXDCS, -1, -1, TiffType.ASCII, FIELD_FAXDCS, true, false, "FaxDcs"), 
        };

        private static readonly TiffFieldInfo[] m_fax3FieldInfo = 
        {
            new TiffFieldInfo(TiffTag.GROUP3OPTIONS, 1, 1, TiffType.LONG, FIELD_OPTIONS, false, false, "Group3Options"), 
        };

        private static readonly TiffFieldInfo[] m_fax4FieldInfo = 
        {
            new TiffFieldInfo(TiffTag.GROUP4OPTIONS, 1, 1, TiffType.LONG, FIELD_OPTIONS, false, false, "Group4Options"), 
        };

        private TiffTagMethods m_parentTagMethods;
        private TiffTagMethods m_tagMethods;

        private int m_rw_mode;
        private int m_rowbytes;
        private int m_rowpixels;

        private Decoder m_decoder;
        private byte[] m_bitmap;
        private int m_data;
        private int m_bit;
        private int m_EOLcnt;
        private int[] m_runs;
        private int m_refruns;
        private int m_curruns;

        private int m_a0;
        private int m_RunLength;
        private int m_thisrun;
        private int m_pa;
        private int m_pb;

        private Fax3Encoder m_encoder;
        private bool m_encodingFax4;
        private byte[] m_refline;
        private int m_k;
        private int m_maxk;
        private int m_line;

        private byte[] m_buffer;
        private int m_offset;

        public CCITTCodec(Tiff tif, Compression scheme, string name)
            : base(tif, scheme, name)
        {
            m_tagMethods = new CCITTCodecTagMethods();
        }

        private void cleanState()
        {
            m_mode = FaxMode.CLASSIC;
            m_groupoptions = Group3Opt.UNKNOWN;
            m_cleanfaxdata = CleanFaxData.CLEAN;
            m_badfaxlines = 0;
            m_badfaxrun = 0;
            m_recvparams = 0;
            m_subaddress = null;
            m_recvtime = 0;
            m_faxdcs = null;

            fill = null;
            m_rw_mode = 0;
            m_rowbytes = 0;
            m_rowpixels = 0;

            m_decoder = 0;
            m_bitmap = null;
            m_data = 0;
            m_bit = 0;
            m_EOLcnt = 0;
            m_runs = null;
            m_refruns = 0;
            m_curruns = 0;

            m_a0 = 0;
            m_RunLength = 0;
            m_thisrun = 0;
            m_pa = 0;
            m_pb = 0;

            m_encoder = 0;
            m_encodingFax4 = false;
            m_refline = null;
            m_k = 0;
            m_maxk = 0;
            m_line = 0;

            m_buffer = null;
            m_offset = 0;
        }

        public override bool Init()
        {
            switch (m_scheme)
            {
                case Compression.CCITTRLE:
                    return TIFFInitCCITTRLE();
                case Compression.CCITTRLEW:
                    return TIFFInitCCITTRLEW();
                case Compression.CCITTFAX3:
                    return TIFFInitCCITTFax3();
                case Compression.CCITTFAX4:
                    return TIFFInitCCITTFax4();
            }

            return false;
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

        public override bool SetupDecode()
        {
            return setupState();
        }

        /// <summary>
        /// Prepares the decoder part of the codec for a decoding.
        /// </summary>
        public override bool PreDecode(short plane)
        {
            m_bit = 0;
            m_data = 0;
            m_EOLcnt = 0;

            m_bitmap = Tiff.GetBitRevTable(m_tif.m_dir.td_fillorder != FillOrder.LSB2MSB);
            if (m_refruns >= 0)
            {
                m_runs[m_refruns] = m_rowpixels;
                m_runs[m_refruns + 1] = 0;
            }
            
            m_line = 0;
            return true;
        }

        /// <summary>
        /// Decodes one row of image data.
        /// </summary>
        public override bool DecodeRow(byte[] buffer, int offset, int count, short plane)
        {
            switch (m_decoder)
            {
                case Decoder.useFax3_1DDecoder:
                    return Fax3Decode1D(buffer, offset, count);
                case Decoder.useFax3_2DDecoder:
                    return Fax3Decode2D(buffer, offset, count);
                case Decoder.useFax4Decoder:
                    return Fax4Decode(buffer, offset, count);
                case Decoder.useFax3RLEDecoder:
                    return Fax3DecodeRLE(buffer, offset, count);
            }

            return false;
        }

        /// <summary>
        /// Decodes one strip of image data.
        /// </summary>
        public override bool DecodeStrip(byte[] buffer, int offset, int count, short plane)
        {
            return DecodeRow(buffer, offset, count, plane);
        }

        /// <summary>
        /// Decodes one tile of image data.
        /// </summary>
        public override bool DecodeTile(byte[] buffer, int offset, int count, short plane)
        {
            return DecodeRow(buffer, offset, count, plane);
        }

        /// <summary>
        /// Flushes any internal data buffers and terminates current operation.
        /// </summary>
        public override void Close()
        {
            if ((m_mode & FaxMode.NORTC) == 0)
            {
                int code = EOL_CODE;
                int length = 12;
                if (is2DEncoding())
                {
                    bool b = ((code << 1) != 0) | (m_encoder == Fax3Encoder.useFax1DEncoder);
                    if (b)
                        code = 1;
                    else
                        code = 0;

                    length++;
                }
            }
        }

        /// <summary>
        /// Cleanups the state of the codec.
        /// </summary>
        public override void Cleanup()
        {
            m_tif.m_tagmethods = m_parentTagMethods;
        }

        private bool is2DEncoding()
        {
            return (m_groupoptions & Group3Opt.ENCODING2D) != 0;
        }

        private void CHECK_b1(ref int b1)
        {
            if (m_pa != m_thisrun)
            {
                while (b1 <= m_a0 && b1 < m_rowpixels)
                {
                    b1 += m_runs[m_pb] + m_runs[m_pb + 1];
                    m_pb += 2;
                }
            }
        }

        private static void SWAP(ref int a, ref int b)
        {
            int x = a;
            a = b;
            b = x;
        }

        private static bool isLongAligned(int offset)
        {
            return (offset % sizeof(int) == 0);
        }

        private static bool isShortAligned(int offset)
        {
            return (offset % sizeof(short) == 0);
        }

        private static void FILL(int n, byte[] cp, ref int offset, byte value)
        {
            const int max = 7;

            if (n <= max && n > 0)
            {
                for (int i = n; i > 0; i--)
                    cp[offset + i - 1] = value;

                offset += n;
            }
        }

        private static void fax3FillRuns(byte[] buffer, int offset, int[] runs,
            int thisRunOffset, int nextRunOffset, int width)
        {
            if (((nextRunOffset - thisRunOffset) & 1) != 0)
            {
                runs[nextRunOffset] = 0;
                nextRunOffset++;
            }

            int x = 0;
            for (; thisRunOffset < nextRunOffset; thisRunOffset += 2)
            {
                int run = runs[thisRunOffset];

                if ((uint)x + (uint)run > (uint)width || (uint)run > (uint)width)
                {
                    runs[thisRunOffset] = width - x;
                    run = runs[thisRunOffset];
                }

                if (run != 0)
                {
                    int cp = offset + (x >> 3);
                    int bx = x & 7;
                    if (run > 8 - bx)
                    {
                        if (bx != 0)
                        {
                            buffer[cp] &= (byte)(0xff << (8 - bx));
                            cp++;
                            run -= 8 - bx;
                        }

                        int n = run >> 3;
                        if (n != 0)
                        {
                            if ((n / sizeof(int)) > 1)
                            {
                                for ( ; n != 0 && !isLongAligned(cp); n--)
                                {
                                    buffer[cp] = 0x00;
                                    cp++;
                                }

                                int bytesToFill = n - (n % sizeof(int));
                                n -= bytesToFill;
                                
                                int stop = bytesToFill + cp;
                                for ( ; cp < stop; cp++)
                                    buffer[cp] = 0;
                            }

                            FILL(n, buffer, ref cp, 0);
                            run &= 7;
                        }

                        if (run != 0)
                            buffer[cp] &= (byte)(0xff >> run);
                    }
                    else
                        buffer[cp] &= (byte)(~(fillMasks[run] >> bx));

                    x += runs[thisRunOffset];
                }

                run = runs[thisRunOffset + 1];

                if ((uint)x + (uint)run > (uint)width || (uint)run > (uint)width)
                {
                    runs[thisRunOffset + 1] = width - x;
                    run = runs[thisRunOffset + 1];
                }
                
                if (run != 0)
                {
                    int cp = offset + (x >> 3);
                    int bx = x & 7;
                    if (run > 8 - bx)
                    {
                        if (bx != 0)
                        {
                            // align to byte boundary
                            buffer[cp] |= (byte)(0xff >> bx);
                            cp++;
                            run -= 8 - bx;
                        }

                        int n = run >> 3;
                        if (n != 0)
                        {
                            if ((n / sizeof(int)) > 1)
                            {
                                for ( ; n != 0 && !isLongAligned(cp); n--)
                                {
                                    buffer[cp] = 0xff;
                                    cp++;
                                }
                                
                                int bytesToFill = n - (n % sizeof(int));
                                n -= bytesToFill;
                                
                                int stop = bytesToFill + cp;
                                for ( ; cp < stop; cp++)
                                    buffer[cp] = 0xff;
                            }

                            FILL(n, buffer, ref cp, 0xff);
                            run &= 7;
                        }

                        if (run != 0)
                            buffer[cp] |= (byte)(0xff00 >> run);
                    }
                    else
                        buffer[cp] |= (byte)(fillMasks[run] >> bx);

                    x += runs[thisRunOffset + 1];
                }
            }
        }

        private bool EndOfData()
        {
            return (m_tif.m_rawcp >= m_tif.m_rawcc);
        }

        private int GetBits(int n)
        {
            return (m_data & ((1 << n) - 1));
        }

        private void ClrBits(int n)
        {
            m_bit -= n;
            m_data >>= n;
        }

        private bool NeedBits8(int n)
        {
            if (m_bit < n)
            {
                if (EndOfData())
                {
                    if (m_bit == 0)
                    {
                        return false;
                    }

                    m_bit = n;
                }
                else
                {
                    m_data |= m_bitmap[m_tif.m_rawdata[m_tif.m_rawcp]] << m_bit;
                    m_tif.m_rawcp++;
                    m_bit += 8;
                }
            }

            return true;
        }

        private bool NeedBits16(int n)
        {
            if (m_bit < n)
            {
                if (EndOfData())
                {
                    if (m_bit == 0)
                    {
                        return false;
                    }

                    m_bit = n;
                }
                else
                {
                    m_data |= m_bitmap[m_tif.m_rawdata[m_tif.m_rawcp]] << m_bit;
                    m_tif.m_rawcp++;
                    m_bit += 8;
                    if (m_bit < n)
                    {
                        if (EndOfData())
                        {
                            m_bit = n;
                        }
                        else
                        {
                            m_data |= m_bitmap[m_tif.m_rawdata[m_tif.m_rawcp]] << m_bit;
                            m_tif.m_rawcp++;
                            m_bit += 8;
                        }
                    }
                }
            }

            return true;
        }

        private bool LOOKUP8(out faxTableEntry TabEnt, int wid)
        {
            if (!NeedBits8(wid))
            {
                TabEnt = new faxTableEntry();
                return false;
            }

            TabEnt = faxTableEntry.FromArray(m_faxMainTable, GetBits(wid));
            ClrBits(TabEnt.Width);

            return true;
        }

        private bool LOOKUP16(out faxTableEntry TabEnt, int wid, bool useBlack)
        {
            if (!NeedBits16(wid))
            {
                TabEnt = new faxTableEntry();
                return false;
            }

            if (useBlack)
                TabEnt = faxTableEntry.FromArray(m_faxBlackTable, GetBits(wid));
            else
                TabEnt = faxTableEntry.FromArray(m_faxWhiteTable, GetBits(wid));

            ClrBits(TabEnt.Width);

            return true;
        }

        private bool SYNC_EOL()
        {
            if (m_EOLcnt == 0)
            {
                for ( ; ; )
                {
                    if (!NeedBits16(11))
                        return false;

                    if (GetBits(11) == 0)
                        break;

                    ClrBits(1);
                }
            }

            for ( ; ; )
            {
                if (!NeedBits8(8))
                    return false;

                if (GetBits(8) != 0)
                    break;

                ClrBits(8);
            }

            while (GetBits(1) == 0)
                ClrBits(1);

            ClrBits(1);
            m_EOLcnt = 0;

            return true;
        }

        private bool setupState()
        {
            if (m_tif.m_dir.td_bitspersample != 1)
            {
                return false;
            }

            int rowbytes = 0;
            int rowpixels = 0;
            if (m_tif.IsTiled())
            {
                rowbytes = m_tif.TileRowSize();
                rowpixels = m_tif.m_dir.td_tilewidth;
            }
            else
            {
                rowbytes = m_tif.ScanlineSize();
                rowpixels = m_tif.m_dir.td_imagewidth;
            }
            
            m_rowbytes = rowbytes;
            m_rowpixels = rowpixels;
            
            bool needsRefLine = ((m_groupoptions & Group3Opt.ENCODING2D) != 0 ||
                m_tif.m_dir.td_compression == Compression.CCITTFAX4);

            m_runs = null;
            int nruns = Tiff.roundUp(rowpixels, 32);
            if (needsRefLine)
            {
                long multiplied = (long)nruns * 2;
                if (multiplied > int.MaxValue)
                {
                    return false;
                }
                else
                {
                    nruns = (int)multiplied;
                }
            }

            if (nruns == 0 || ((long)nruns * 2) > int.MaxValue)
            {
                return false;
            }

            m_runs = new int[2 * nruns];
            m_curruns = 0;

            if (needsRefLine)
                m_refruns = nruns;
            else
                m_refruns = -1;
            
            if (m_tif.m_dir.td_compression == Compression.CCITTFAX3 && is2DEncoding())
            {
                m_decoder = Decoder.useFax3_2DDecoder;
            }

            if (needsRefLine)
            {
                m_refline = new byte [rowbytes + 1];
            }
            else
            {
                m_refline = null;
            }

            return true;
        }

        private void Fax3Unexpected(string module)
        {
        }

        private void Fax3Extension(string module)
        {
        }

        private void Fax3BadLength(string module)
        {
        }

        private void Fax3PrematureEOF(string module)
        {
        }

        /// <summary>
        /// Decode the requested amount of G3 1D-encoded data.
        /// </summary>
        private bool Fax3Decode1D(byte[] buffer, int offset, int count)
        {
            const string module = "Fax3Decode1D";
    
            m_thisrun = m_curruns;
            while (count > 0)
            {
                m_a0 = 0;
                m_RunLength = 0;
                m_pa = m_thisrun;

                if (!SYNC_EOL())
                {
                    CLEANUP_RUNS(module);
                    fill(buffer, offset, m_runs, m_thisrun, m_pa, m_rowpixels);
                    return false;
                }

                bool expandSucceeded = EXPAND1D(module);
                if (!expandSucceeded)
                {
                    fill(buffer, offset, m_runs, m_thisrun, m_pa, m_rowpixels);
                    return false;
                }

                fill(buffer, offset, m_runs, m_thisrun, m_pa, m_rowpixels);
                offset += m_rowbytes;
                count -= m_rowbytes;
                m_line++;
            }

            return true;
        }

        /// <summary>
        /// Decode the requested amount of G3 2D-encoded data.
        /// </summary>
        private bool Fax3Decode2D(byte[] buffer, int offset, int count)
        {
            const string module = "Fax3Decode2D";

            while (count > 0)
            {
                m_a0 = 0;
                m_RunLength = 0;
                m_pa = m_curruns;
                m_thisrun = m_curruns;

                bool prematureEOF = false;
                if (!SYNC_EOL())
                    prematureEOF = true;

                if (!prematureEOF && !NeedBits8(1))
                    prematureEOF = true;

                if (!prematureEOF)
                {
                    int is1D = GetBits(1); 
                    ClrBits(1);
                    m_pb = m_refruns;
                    int b1 = m_runs[m_pb];
                    m_pb++; 

                    bool expandSucceeded = false;
                    if (is1D != 0)
                        expandSucceeded = EXPAND1D(module);
                    else
                        expandSucceeded = EXPAND2D(module, b1);

                    if (expandSucceeded)
                    {
                        fill(buffer, offset, m_runs, m_thisrun, m_pa, m_rowpixels);
                        SETVALUE(0);
                        SWAP(ref m_curruns, ref m_refruns);
                        offset += m_rowbytes;
                        count -= m_rowbytes;
                        m_line++;
                        continue;
                    }
                }
                else
                {
                    CLEANUP_RUNS(module);
                }

                fill(buffer, offset, m_runs, m_thisrun, m_pa, m_rowpixels);
                return false;
            }

            return true;
        }

        private void InitCCITTFax3()
        {
            m_tif.MergeFieldInfo(m_faxFieldInfo, m_faxFieldInfo.Length);
            cleanState();
            m_rw_mode = m_tif.m_mode;

            m_parentTagMethods = m_tif.m_tagmethods;
            m_tif.m_tagmethods = m_tagMethods;
            
            m_groupoptions = 0;
            m_recvparams = 0;
            m_subaddress = null;
            m_faxdcs = null;

            if (m_rw_mode == Tiff.O_RDONLY)
            {
                m_tif.m_flags |= TiffFlags.NOBITREV;
            }

            m_runs = null;
            m_tif.SetField(TiffTag.FAXFILLFUNC, new Tiff.FaxFillFunc(fax3FillRuns));
            m_refline = null;

            m_decoder = Decoder.useFax3_1DDecoder;
            m_encodingFax4 = false;
        }

        private bool TIFFInitCCITTFax3()
        {
            InitCCITTFax3();
            m_tif.MergeFieldInfo(m_fax3FieldInfo, m_fax3FieldInfo.Length);

            return m_tif.SetField(TiffTag.FAXMODE, FaxMode.CLASSF);
        }

        private void SETVALUE(int x)
        {
            m_runs[m_pa] = m_RunLength + x;
            m_pa++;
            m_a0 += x;
            m_RunLength = 0;
        }

        private void CLEANUP_RUNS(string module)
        {
            if (m_RunLength != 0)
                SETVALUE(0);

            if (m_a0 != m_rowpixels)
            {
                Fax3BadLength(module);

                while (m_a0 > m_rowpixels && m_pa > m_thisrun)
                {
                    m_pa--;
                    m_a0 -= m_runs[m_pa];
                }

                if (m_a0 < m_rowpixels)
                {
                    if (m_a0 < 0)
                        m_a0 = 0;

                    if (((m_pa - m_thisrun) & 1) != 0)
                        SETVALUE(0);

                    SETVALUE(m_rowpixels - m_a0);
                }
                else if (m_a0 > m_rowpixels)
                {
                    SETVALUE(m_rowpixels);
                    SETVALUE(0);
                }
            }
        }

        private void handlePrematureEOFinExpand2D(string module)
        {
            Fax3PrematureEOF(module);
            CLEANUP_RUNS(module);
        }

        private bool EXPAND1D(string module)
        {
            faxTableEntry TabEnt;

            for ( ; ; )
            {
                for ( ; ; )
                {
                    if (!LOOKUP16(out TabEnt, 12, false))
                    {
                        Fax3PrematureEOF(module);
                        CLEANUP_RUNS(module);
                        return false;
                    }

                    bool whiteDecodingDone = false;
                    switch (TabEnt.State)
                    {
                        case S_EOL:
                            m_EOLcnt = 1;
                            CLEANUP_RUNS(module);
                            return true;

                        case S_TermW:
                            SETVALUE(TabEnt.Param);
                            whiteDecodingDone = true;
                            break;

                        case S_MakeUpW:
                        case S_MakeUp:
                            m_a0 += TabEnt.Param;
                            m_RunLength += TabEnt.Param;
                            break;

                        default:
                            /* "WhiteTable" */
                            Fax3Unexpected(module);
                            CLEANUP_RUNS(module);
                            return true;
                    }

                    if (whiteDecodingDone)
                        break;
                }

                if (m_a0 >= m_rowpixels)
                {
                    CLEANUP_RUNS(module);
                    return true;
                }

                for ( ; ; )
                {
                    if (!LOOKUP16(out TabEnt, 13, true))
                    {
                        Fax3PrematureEOF(module);
                        CLEANUP_RUNS(module);
                        return false;
                    }

                    bool blackDecodingDone = false;
                    switch (TabEnt.State)
                    {
                        case S_EOL:
                            m_EOLcnt = 1;
                            CLEANUP_RUNS(module);
                            return true;

                        case S_TermB:
                            SETVALUE(TabEnt.Param);
                            blackDecodingDone = true;
                            break;

                        case S_MakeUpB:
                        case S_MakeUp:
                            m_a0 += TabEnt.Param;
                            m_RunLength += TabEnt.Param;
                            break;

                        default:
                            /* "BlackTable" */
                            Fax3Unexpected(module);
                            CLEANUP_RUNS(module);
                            return true;
                    }

                    if (blackDecodingDone)
                        break;
                }

                if (m_a0 >= m_rowpixels)
                {
                    CLEANUP_RUNS(module);
                    return true;
                }

                if (m_runs[m_pa - 1] == 0 && m_runs[m_pa - 2] == 0)
                    m_pa -= 2;
            }
        }

        private bool EXPAND2D(string module, int b1)
        {
            faxTableEntry TabEnt;
            bool decodingDone = false;

            while (m_a0 < m_rowpixels)
            {
                if (!LOOKUP8(out TabEnt, 7))
                {
                    handlePrematureEOFinExpand2D(module);
                    return false;
                }

                switch (TabEnt.State)
                {
                    case S_Pass:
                        CHECK_b1(ref b1);
                        b1 += m_runs[m_pb];
                        m_pb++;
                        m_RunLength += b1 - m_a0;
                        m_a0 = b1;
                        b1 += m_runs[m_pb];
                        m_pb++;
                        break;

                    case S_Horiz:
                        if (((m_pa - m_thisrun) & 1) != 0)
                        {
                            for ( ; ; )
                            {
                                /* black first */
                                if (!LOOKUP16(out TabEnt, 13, true))
                                {
                                    handlePrematureEOFinExpand2D(module);
                                    return false;
                                }

                                bool doneWhite2d = false;
                                switch (TabEnt.State)
                                {
                                    case S_TermB:
                                        SETVALUE(TabEnt.Param);
                                        doneWhite2d = true;
                                        break;

                                    case S_MakeUpB:
                                    case S_MakeUp:
                                        m_a0 += TabEnt.Param;
                                        m_RunLength += TabEnt.Param;
                                        break;

                                    default:
                                        /* "BlackTable" */
                                        Fax3Unexpected(module);
                                        decodingDone = true;
                                        break;
                                }

                                if (doneWhite2d || decodingDone)
                                    break;
                            }

                            if (decodingDone)
                                break;

                            for ( ; ; )
                            {
                                /* then white */
                                if (!LOOKUP16(out TabEnt, 12, false))
                                {
                                    handlePrematureEOFinExpand2D(module);
                                    return false;
                                }

                                bool doneBlack2d = false;
                                switch (TabEnt.State)
                                {
                                    case S_TermW:
                                        SETVALUE(TabEnt.Param);
                                        doneBlack2d = true;
                                        break;

                                    case S_MakeUpW:
                                    case S_MakeUp:
                                        m_a0 += TabEnt.Param;
                                        m_RunLength += TabEnt.Param;
                                        break;

                                    default:
                                        /* "WhiteTable" */
                                        Fax3Unexpected(module);
                                        decodingDone = true;
                                        break;
                                }

                                if (doneBlack2d || decodingDone)
                                    break;
                            }

                            if (decodingDone)
                                break;
                        }
                        else
                        {
                            for ( ; ; )
                            {
                                /* white first */
                                if (!LOOKUP16(out TabEnt, 12, false))
                                {
                                    handlePrematureEOFinExpand2D(module);
                                    return false;
                                }

                                bool doneWhite2d = false;
                                switch (TabEnt.State)
                                {
                                    case S_TermW:
                                        SETVALUE(TabEnt.Param);
                                        doneWhite2d = true;
                                        break;

                                    case S_MakeUpW:
                                    case S_MakeUp:
                                        m_a0 += TabEnt.Param;
                                        m_RunLength += TabEnt.Param;
                                        break;

                                    default:
                                        /* "WhiteTable" */
                                        Fax3Unexpected(module);
                                        decodingDone = true;
                                        break;
                                }

                                if (doneWhite2d || decodingDone)
                                    break;
                            }

                            if (decodingDone)
                                break;

                            for ( ; ; )
                            {
                                /* then black */
                                if (!LOOKUP16(out TabEnt, 13, true))
                                {
                                    handlePrematureEOFinExpand2D(module);
                                    return false;
                                }

                                bool doneBlack2d = false;
                                switch (TabEnt.State)
                                {
                                    case S_TermB:
                                        SETVALUE(TabEnt.Param);
                                        doneBlack2d = true;
                                        break;

                                    case S_MakeUpB:
                                    case S_MakeUp:
                                        m_a0 += TabEnt.Param;
                                        m_RunLength += TabEnt.Param;
                                        break;

                                    default:
                                        /* "BlackTable" */
                                        Fax3Unexpected(module);
                                        decodingDone = true;
                                        break;
                                }

                                if (doneBlack2d || decodingDone)
                                    break;
                            }
                        }

                        if (decodingDone)
                            break;

                        CHECK_b1(ref b1);
                        break;

                    case S_V0:
                        CHECK_b1(ref b1);
                        SETVALUE(b1 - m_a0);
                        b1 += m_runs[m_pb];
                        m_pb++;
                        break;

                    case S_VR:
                        CHECK_b1(ref b1);
                        SETVALUE(b1 - m_a0 + TabEnt.Param);
                        b1 += m_runs[m_pb];
                        m_pb++;
                        break;

                    case S_VL:
                        CHECK_b1(ref b1);
                        SETVALUE(b1 - m_a0 - TabEnt.Param);
                        m_pb--;
                        b1 -= m_runs[m_pb];
                        break;

                    case S_Ext:
                        m_runs[m_pa] = m_rowpixels - m_a0;
                        m_pa++;
                        Fax3Extension(module);
                        decodingDone = true;
                        break;

                    case S_EOL:
                        m_runs[m_pa] = m_rowpixels - m_a0;
                        m_pa++;

                        if (!NeedBits8(4))
                        {
                            handlePrematureEOFinExpand2D(module);
                            return false;
                        }

                        if (GetBits(4) != 0)
                        {
                            /* "EOL" */
                            Fax3Unexpected(module);
                        }

                        ClrBits(4);
                        m_EOLcnt = 1;
                        decodingDone = true;
                        break;

                    default:
                        Fax3Unexpected(module);
                        decodingDone = true;
                        break;
                }
            }

            if (!decodingDone && m_RunLength != 0)
            {
                if (m_RunLength + m_a0 < m_rowpixels)
                {
                    /* expect a final V0 */
                    if (!NeedBits8(1))
                    {
                        handlePrematureEOFinExpand2D(module);
                        return false;
                    }

                    if (GetBits(1) == 0)
                    {
                        /* "MainTable" */
                        Fax3Unexpected(module);
                        decodingDone = true;
                    }

                    if (!decodingDone)
                        ClrBits(1);
                }

                if (!decodingDone)
                    SETVALUE(0);
            }

            CLEANUP_RUNS(module);
            return true;
        }

        private bool TIFFInitCCITTRLE()
        {
            InitCCITTFax3();

            m_decoder = Decoder.useFax3RLEDecoder;

            return m_tif.SetField(TiffTag.FAXMODE, 
                FaxMode.NORTC | FaxMode.NOEOL | FaxMode.BYTEALIGN);
        }

        private bool TIFFInitCCITTRLEW()
        {
            InitCCITTFax3();

            m_decoder = Decoder.useFax3RLEDecoder;
            return m_tif.SetField(TiffTag.FAXMODE, 
                FaxMode.NORTC | FaxMode.NOEOL | FaxMode.WORDALIGN);
        }

        /// <summary>
        /// Decode the requested amount of RLE-encoded data.
        /// </summary>
        private bool Fax3DecodeRLE(byte[] buffer, int offset, int count)
        {
            const string module = "Fax3DecodeRLE";

            int thisrun = m_curruns; // current row's run array

            while (count > 0)
            {
                m_a0 = 0;
                m_RunLength = 0;
                m_pa = thisrun;

                bool expandSucceeded = EXPAND1D(module);
                if (expandSucceeded)
                {
                    fill(buffer, offset, m_runs, thisrun, m_pa, m_rowpixels);

                    // Cleanup at the end of the row.
                    if ((m_mode & FaxMode.BYTEALIGN) != 0)
                    {
                        int n = m_bit - (m_bit & ~7);
                        ClrBits(n);
                    }
                    else if ((m_mode & FaxMode.WORDALIGN) != 0)
                    {
                        int n = m_bit - (m_bit & ~15);
                        ClrBits(n);
                        if (m_bit == 0 && !isShortAligned(m_tif.m_rawcp))
                            m_tif.m_rawcp++;
                    }

                    offset += m_rowbytes;
                    count -= m_rowbytes;
                    m_line++;
                    continue;
                }

                // premature EOF
                fill(buffer, offset, m_runs, thisrun, m_pa, m_rowpixels);
                return false;
            }

            return true;
        }

        private bool TIFFInitCCITTFax4()
        {
            /* reuse G3 support */
            InitCCITTFax3();

            m_tif.MergeFieldInfo(m_fax4FieldInfo, m_fax4FieldInfo.Length);

            m_decoder = Decoder.useFax4Decoder;
            m_encodingFax4 = true;

            return m_tif.SetField(TiffTag.FAXMODE, FaxMode.NORTC);
        }

        /// <summary>
        /// Decode the requested amount of G4-encoded data.
        /// </summary>
        private bool Fax4Decode(byte[] buffer, int offset, int count)
        {
            const string module = "Fax4Decode";

            while (count > 0)
            {
                m_a0 = 0;
                m_RunLength = 0;
                m_thisrun = m_curruns;
                m_pa = m_curruns;
                m_pb = m_refruns;
                int b1 = m_runs[m_pb];
                m_pb++; // next change on prev line

                bool expandSucceeded = EXPAND2D(module, b1);
                if (expandSucceeded && m_EOLcnt != 0)
                    expandSucceeded = false;

                if (expandSucceeded)
                {
                    fill(buffer, offset, m_runs, m_thisrun, m_pa, m_rowpixels);
                    SETVALUE(0); // imaginary change for reference
                    SWAP(ref m_curruns, ref m_refruns);
                    offset += m_rowbytes;
                    count -= m_rowbytes;
                    m_line++;
                    continue;
                }

                NeedBits16(13);
                ClrBits(13);
                fill(buffer, offset, m_runs, m_thisrun, m_pa, m_rowpixels);
                return false;
            }

            return true;
        }

        private static readonly int[] m_faxMainTable =
        {
            12, 7, 0, 3, 1, 0, 5, 3, 1, 3, 1, 0, 2, 3, 0, 3, 1, 0, 4, 3, 1, 3, 1, 0, 1, 4, 0, 3, 1, 0,
            5, 3, 1, 3, 1, 0, 2, 3, 0, 3, 1, 0, 4, 3, 1, 3, 1, 0, 5, 6, 2, 3, 1, 0, 5, 3, 1, 3, 1, 0,
            2, 3, 0, 3, 1, 0, 4, 3, 1, 3, 1, 0, 1, 4, 0, 3, 1, 0, 5, 3, 1, 3, 1, 0, 2, 3, 0, 3, 1, 0,
            4, 3, 1, 3, 1, 0, 5, 7, 3, 3, 1, 0, 5, 3, 1, 3, 1, 0, 2, 3, 0, 3, 1, 0, 4, 3, 1, 3, 1, 0,
            1, 4, 0, 3, 1, 0, 5, 3, 1, 3, 1, 0, 2, 3, 0, 3, 1, 0, 4, 3, 1, 3, 1, 0, 4, 6, 2, 3, 1, 0,
            5, 3, 1, 3, 1, 0, 2, 3, 0, 3, 1, 0, 4, 3, 1, 3, 1, 0, 1, 4, 0, 3, 1, 0, 5, 3, 1, 3, 1, 0,
            2, 3, 0, 3, 1, 0, 4, 3, 1, 3, 1, 0, 6, 7, 0, 3, 1, 0, 5, 3, 1, 3, 1, 0, 2, 3, 0, 3, 1, 0,
            4, 3, 1, 3, 1, 0, 1, 4, 0, 3, 1, 0, 5, 3, 1, 3, 1, 0, 2, 3, 0, 3, 1, 0, 4, 3, 1, 3, 1, 0,
            5, 6, 2, 3, 1, 0, 5, 3, 1, 3, 1, 0, 2, 3, 0, 3, 1, 0, 4, 3, 1, 3, 1, 0, 1, 4, 0, 3, 1, 0,
            5, 3, 1, 3, 1, 0, 2, 3, 0, 3, 1, 0, 4, 3, 1, 3, 1, 0, 4, 7, 3, 3, 1, 0, 5, 3, 1, 3, 1, 0,
            2, 3, 0, 3, 1, 0, 4, 3, 1, 3, 1, 0, 1, 4, 0, 3, 1, 0, 5, 3, 1, 3, 1, 0, 2, 3, 0, 3, 1, 0,
            4, 3, 1, 3, 1, 0, 4, 6, 2, 3, 1, 0, 5, 3, 1, 3, 1, 0, 2, 3, 0, 3, 1, 0, 4, 3, 1, 3, 1, 0,
            1, 4, 0, 3, 1, 0, 5, 3, 1, 3, 1, 0, 2, 3, 0, 3, 1, 0, 4, 3, 1, 3, 1, 0
        };

        private static readonly int[] m_faxWhiteTable =
        {
            12, 11, 0, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128,
            7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5,
            7, 8, 39, 7, 6, 16, 9, 8, 576, 7, 4, 6, 7, 7, 19, 7, 5, 8, 7, 8, 55, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 8, 45, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 8, 53, 7, 5, 9, 9, 8, 448, 7, 4, 6,
            7, 8, 35, 9, 5, 128, 7, 8, 51, 7, 6, 15, 7, 8, 63, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3,
            9, 9, 1472, 7, 4, 5, 7, 8, 43, 7, 6, 17, 9, 9, 1216, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 29, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9,
            9, 6, 1664, 7, 4, 6, 7, 8, 33, 9, 5, 128, 7, 8, 49, 7, 6, 14, 7, 8, 61, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 8, 47, 7, 4, 3, 7, 8, 59, 7, 4, 5, 7, 8, 41, 7, 6, 16, 9, 9, 960, 7, 4, 6, 7, 8, 31, 7, 5, 8,
            7, 8, 57, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 7, 26, 7, 5, 9, 9, 9, 704, 7, 4, 6, 7, 8, 37, 9, 5, 128, 7, 7, 25, 7, 6, 15, 9, 8, 320, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6,
            7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 11, 11, 1792, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14,
            7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5, 7, 8, 40, 7, 6, 16,
            9, 9, 832, 7, 4, 6, 7, 7, 19, 7, 5, 8, 7, 8, 56, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 8, 46, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 8, 54, 7, 5, 9, 9, 8, 512, 7, 4, 6, 7, 8, 36, 9, 5, 128,
            7, 8, 52, 7, 6, 15, 7, 8, 0, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 9, 9, 1600, 7, 4, 5,
            7, 8, 44, 7, 6, 17, 9, 9, 1344, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 8, 30, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6,
            7, 8, 34, 9, 5, 128, 7, 8, 50, 7, 6, 14, 7, 8, 62, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 48, 7, 4, 3,
            7, 8, 60, 7, 4, 5, 7, 8, 42, 7, 6, 16, 9, 9, 1088, 7, 4, 6, 7, 8, 32, 7, 5, 8, 7, 8, 58, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9,
            9, 8, 640, 7, 4, 6, 7, 8, 38, 9, 5, 128, 7, 7, 25, 7, 6, 15, 9, 8, 384, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8,
            9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 0, 0, 0, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5, 7, 8, 39, 7, 6, 16, 9, 8, 576, 7, 4, 6,
            7, 7, 19, 7, 5, 8, 7, 8, 55, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 45, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 8, 53, 7, 5, 9, 9, 8, 448, 7, 4, 6, 7, 8, 35, 9, 5, 128, 7, 8, 51, 7, 6, 15,
            7, 8, 63, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 9, 9, 1536, 7, 4, 5, 7, 8, 43, 7, 6, 17,
            9, 9, 1280, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 8, 29, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 8, 33, 9, 5, 128,
            7, 8, 49, 7, 6, 14, 7, 8, 61, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 47, 7, 4, 3, 7, 8, 59, 7, 4, 5,
            7, 8, 41, 7, 6, 16, 9, 9, 1024, 7, 4, 6, 7, 8, 31, 7, 5, 8, 7, 8, 57, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9, 9, 9, 768, 7, 4, 6,
            7, 8, 37, 9, 5, 128, 7, 7, 25, 7, 6, 15, 9, 8, 320, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3,
            7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 11, 11, 1856, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9,
            9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5, 7, 8, 40, 7, 6, 16, 9, 9, 896, 7, 4, 6, 7, 7, 19, 7, 5, 8,
            7, 8, 56, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 46, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 8, 54, 7, 5, 9, 9, 8, 512, 7, 4, 6, 7, 8, 36, 9, 5, 128, 7, 8, 52, 7, 6, 15, 7, 8, 0, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 9, 9, 1728, 7, 4, 5, 7, 8, 44, 7, 6, 17, 9, 9, 1408, 7, 4, 6,
            7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 30, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 8, 34, 9, 5, 128, 7, 8, 50, 7, 6, 14,
            7, 8, 62, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 48, 7, 4, 3, 7, 8, 60, 7, 4, 5, 7, 8, 42, 7, 6, 16,
            9, 9, 1152, 7, 4, 6, 7, 8, 32, 7, 5, 8, 7, 8, 58, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9, 9, 8, 640, 7, 4, 6, 7, 8, 38, 9, 5, 128,
            7, 7, 25, 7, 6, 15, 9, 8, 384, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5,
            7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 0, 0, 0, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6,
            7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3,
            7, 7, 27, 7, 4, 5, 7, 8, 39, 7, 6, 16, 9, 8, 576, 7, 4, 6, 7, 7, 19, 7, 5, 8, 7, 8, 55, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 45, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 8, 53, 7, 5, 9,
            9, 8, 448, 7, 4, 6, 7, 8, 35, 9, 5, 128, 7, 8, 51, 7, 6, 15, 7, 8, 63, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 6, 13, 7, 4, 3, 9, 9, 1472, 7, 4, 5, 7, 8, 43, 7, 6, 17, 9, 9, 1216, 7, 4, 6, 7, 6, 1, 7, 5, 8,
            9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 29, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 8, 33, 9, 5, 128, 7, 8, 49, 7, 6, 14, 7, 8, 61, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 8, 47, 7, 4, 3, 7, 8, 59, 7, 4, 5, 7, 8, 41, 7, 6, 16, 9, 9, 960, 7, 4, 6,
            7, 8, 31, 7, 5, 8, 7, 8, 57, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9, 9, 9, 704, 7, 4, 6, 7, 8, 37, 9, 5, 128, 7, 7, 25, 7, 6, 15,
            9, 8, 320, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17,
            9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            11, 12, 2112, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128,
            7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5,
            7, 8, 40, 7, 6, 16, 9, 9, 832, 7, 4, 6, 7, 7, 19, 7, 5, 8, 7, 8, 56, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 8, 46, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 8, 54, 7, 5, 9, 9, 8, 512, 7, 4, 6,
            7, 8, 36, 9, 5, 128, 7, 8, 52, 7, 6, 15, 7, 8, 0, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3,
            9, 9, 1600, 7, 4, 5, 7, 8, 44, 7, 6, 17, 9, 9, 1344, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 30, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9,
            9, 6, 1664, 7, 4, 6, 7, 8, 34, 9, 5, 128, 7, 8, 50, 7, 6, 14, 7, 8, 62, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 8, 48, 7, 4, 3, 7, 8, 60, 7, 4, 5, 7, 8, 42, 7, 6, 16, 9, 9, 1088, 7, 4, 6, 7, 8, 32, 7, 5, 8,
            7, 8, 58, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 7, 26, 7, 5, 9, 9, 8, 640, 7, 4, 6, 7, 8, 38, 9, 5, 128, 7, 7, 25, 7, 6, 15, 9, 8, 384, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6,
            7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 0, 0, 0, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14,
            7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5, 7, 8, 39, 7, 6, 16,
            9, 8, 576, 7, 4, 6, 7, 7, 19, 7, 5, 8, 7, 8, 55, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 8, 45, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 8, 53, 7, 5, 9, 9, 8, 448, 7, 4, 6, 7, 8, 35, 9, 5, 128,
            7, 8, 51, 7, 6, 15, 7, 8, 63, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 9, 9, 1536, 7, 4, 5,
            7, 8, 43, 7, 6, 17, 9, 9, 1280, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 8, 29, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6,
            7, 8, 33, 9, 5, 128, 7, 8, 49, 7, 6, 14, 7, 8, 61, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 47, 7, 4, 3,
            7, 8, 59, 7, 4, 5, 7, 8, 41, 7, 6, 16, 9, 9, 1024, 7, 4, 6, 7, 8, 31, 7, 5, 8, 7, 8, 57, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9,
            9, 9, 768, 7, 4, 6, 7, 8, 37, 9, 5, 128, 7, 7, 25, 7, 6, 15, 9, 8, 320, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8,
            9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 11, 12, 2368, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5, 7, 8, 40, 7, 6, 16, 9, 9, 896, 7, 4, 6,
            7, 7, 19, 7, 5, 8, 7, 8, 56, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 46, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 8, 54, 7, 5, 9, 9, 8, 512, 7, 4, 6, 7, 8, 36, 9, 5, 128, 7, 8, 52, 7, 6, 15,
            7, 8, 0, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 9, 9, 1728, 7, 4, 5, 7, 8, 44, 7, 6, 17,
            9, 9, 1408, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 8, 30, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 8, 34, 9, 5, 128,
            7, 8, 50, 7, 6, 14, 7, 8, 62, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 48, 7, 4, 3, 7, 8, 60, 7, 4, 5,
            7, 8, 42, 7, 6, 16, 9, 9, 1152, 7, 4, 6, 7, 8, 32, 7, 5, 8, 7, 8, 58, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9, 9, 8, 640, 7, 4, 6,
            7, 8, 38, 9, 5, 128, 7, 7, 25, 7, 6, 15, 9, 8, 384, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3,
            7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 0, 0, 0, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9,
            9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5, 7, 8, 39, 7, 6, 16, 9, 8, 576, 7, 4, 6, 7, 7, 19, 7, 5, 8,
            7, 8, 55, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 45, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 8, 53, 7, 5, 9, 9, 8, 448, 7, 4, 6, 7, 8, 35, 9, 5, 128, 7, 8, 51, 7, 6, 15, 7, 8, 63, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 9, 9, 1472, 7, 4, 5, 7, 8, 43, 7, 6, 17, 9, 9, 1216, 7, 4, 6,
            7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 29, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 8, 33, 9, 5, 128, 7, 8, 49, 7, 6, 14,
            7, 8, 61, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 47, 7, 4, 3, 7, 8, 59, 7, 4, 5, 7, 8, 41, 7, 6, 16,
            9, 9, 960, 7, 4, 6, 7, 8, 31, 7, 5, 8, 7, 8, 57, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9, 9, 9, 704, 7, 4, 6, 7, 8, 37, 9, 5, 128,
            7, 7, 25, 7, 6, 15, 9, 8, 320, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5,
            7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 11, 12, 1984, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6,
            7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3,
            7, 7, 27, 7, 4, 5, 7, 8, 40, 7, 6, 16, 9, 9, 832, 7, 4, 6, 7, 7, 19, 7, 5, 8, 7, 8, 56, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 46, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 8, 54, 7, 5, 9,
            9, 8, 512, 7, 4, 6, 7, 8, 36, 9, 5, 128, 7, 8, 52, 7, 6, 15, 7, 8, 0, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 6, 13, 7, 4, 3, 9, 9, 1600, 7, 4, 5, 7, 8, 44, 7, 6, 17, 9, 9, 1344, 7, 4, 6, 7, 6, 1, 7, 5, 8,
            9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 30, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 8, 34, 9, 5, 128, 7, 8, 50, 7, 6, 14, 7, 8, 62, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 8, 48, 7, 4, 3, 7, 8, 60, 7, 4, 5, 7, 8, 42, 7, 6, 16, 9, 9, 1088, 7, 4, 6,
            7, 8, 32, 7, 5, 8, 7, 8, 58, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9, 9, 8, 640, 7, 4, 6, 7, 8, 38, 9, 5, 128, 7, 7, 25, 7, 6, 15,
            9, 8, 384, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17,
            9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            0, 0, 0, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128,
            7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5,
            7, 8, 39, 7, 6, 16, 9, 8, 576, 7, 4, 6, 7, 7, 19, 7, 5, 8, 7, 8, 55, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 8, 45, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 8, 53, 7, 5, 9, 9, 8, 448, 7, 4, 6,
            7, 8, 35, 9, 5, 128, 7, 8, 51, 7, 6, 15, 7, 8, 63, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3,
            9, 9, 1536, 7, 4, 5, 7, 8, 43, 7, 6, 17, 9, 9, 1280, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 29, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9,
            9, 6, 1664, 7, 4, 6, 7, 8, 33, 9, 5, 128, 7, 8, 49, 7, 6, 14, 7, 8, 61, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 8, 47, 7, 4, 3, 7, 8, 59, 7, 4, 5, 7, 8, 41, 7, 6, 16, 9, 9, 1024, 7, 4, 6, 7, 8, 31, 7, 5, 8,
            7, 8, 57, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 7, 26, 7, 5, 9, 9, 9, 768, 7, 4, 6, 7, 8, 37, 9, 5, 128, 7, 7, 25, 7, 6, 15, 9, 8, 320, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6,
            7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 11, 11, 1920, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14,
            7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5, 7, 8, 40, 7, 6, 16,
            9, 9, 896, 7, 4, 6, 7, 7, 19, 7, 5, 8, 7, 8, 56, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 8, 46, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 8, 54, 7, 5, 9, 9, 8, 512, 7, 4, 6, 7, 8, 36, 9, 5, 128,
            7, 8, 52, 7, 6, 15, 7, 8, 0, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 9, 9, 1728, 7, 4, 5,
            7, 8, 44, 7, 6, 17, 9, 9, 1408, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 8, 30, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6,
            7, 8, 34, 9, 5, 128, 7, 8, 50, 7, 6, 14, 7, 8, 62, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 48, 7, 4, 3,
            7, 8, 60, 7, 4, 5, 7, 8, 42, 7, 6, 16, 9, 9, 1152, 7, 4, 6, 7, 8, 32, 7, 5, 8, 7, 8, 58, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9,
            9, 8, 640, 7, 4, 6, 7, 8, 38, 9, 5, 128, 7, 7, 25, 7, 6, 15, 9, 8, 384, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8,
            9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 0, 0, 0, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5, 7, 8, 39, 7, 6, 16, 9, 8, 576, 7, 4, 6,
            7, 7, 19, 7, 5, 8, 7, 8, 55, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 45, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 8, 53, 7, 5, 9, 9, 8, 448, 7, 4, 6, 7, 8, 35, 9, 5, 128, 7, 8, 51, 7, 6, 15,
            7, 8, 63, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 9, 9, 1472, 7, 4, 5, 7, 8, 43, 7, 6, 17,
            9, 9, 1216, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 8, 29, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 8, 33, 9, 5, 128,
            7, 8, 49, 7, 6, 14, 7, 8, 61, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 47, 7, 4, 3, 7, 8, 59, 7, 4, 5,
            7, 8, 41, 7, 6, 16, 9, 9, 960, 7, 4, 6, 7, 8, 31, 7, 5, 8, 7, 8, 57, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9, 9, 9, 704, 7, 4, 6,
            7, 8, 37, 9, 5, 128, 7, 7, 25, 7, 6, 15, 9, 8, 320, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3,
            7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 11, 12, 2240, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9,
            9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5, 7, 8, 40, 7, 6, 16, 9, 9, 832, 7, 4, 6, 7, 7, 19, 7, 5, 8,
            7, 8, 56, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 46, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 8, 54, 7, 5, 9, 9, 8, 512, 7, 4, 6, 7, 8, 36, 9, 5, 128, 7, 8, 52, 7, 6, 15, 7, 8, 0, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 9, 9, 1600, 7, 4, 5, 7, 8, 44, 7, 6, 17, 9, 9, 1344, 7, 4, 6,
            7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 30, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 8, 34, 9, 5, 128, 7, 8, 50, 7, 6, 14,
            7, 8, 62, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 48, 7, 4, 3, 7, 8, 60, 7, 4, 5, 7, 8, 42, 7, 6, 16,
            9, 9, 1088, 7, 4, 6, 7, 8, 32, 7, 5, 8, 7, 8, 58, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9, 9, 8, 640, 7, 4, 6, 7, 8, 38, 9, 5, 128,
            7, 7, 25, 7, 6, 15, 9, 8, 384, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5,
            7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 0, 0, 0, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6,
            7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3,
            7, 7, 27, 7, 4, 5, 7, 8, 39, 7, 6, 16, 9, 8, 576, 7, 4, 6, 7, 7, 19, 7, 5, 8, 7, 8, 55, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 45, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 8, 53, 7, 5, 9,
            9, 8, 448, 7, 4, 6, 7, 8, 35, 9, 5, 128, 7, 8, 51, 7, 6, 15, 7, 8, 63, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 6, 13, 7, 4, 3, 9, 9, 1536, 7, 4, 5, 7, 8, 43, 7, 6, 17, 9, 9, 1280, 7, 4, 6, 7, 6, 1, 7, 5, 8,
            9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 29, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 8, 33, 9, 5, 128, 7, 8, 49, 7, 6, 14, 7, 8, 61, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 8, 47, 7, 4, 3, 7, 8, 59, 7, 4, 5, 7, 8, 41, 7, 6, 16, 9, 9, 1024, 7, 4, 6,
            7, 8, 31, 7, 5, 8, 7, 8, 57, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9, 9, 9, 768, 7, 4, 6, 7, 8, 37, 9, 5, 128, 7, 7, 25, 7, 6, 15,
            9, 8, 320, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17,
            9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            11, 12, 2496, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128,
            7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5,
            7, 8, 40, 7, 6, 16, 9, 9, 896, 7, 4, 6, 7, 7, 19, 7, 5, 8, 7, 8, 56, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 8, 46, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 8, 54, 7, 5, 9, 9, 8, 512, 7, 4, 6,
            7, 8, 36, 9, 5, 128, 7, 8, 52, 7, 6, 15, 7, 8, 0, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3,
            9, 9, 1728, 7, 4, 5, 7, 8, 44, 7, 6, 17, 9, 9, 1408, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 30, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9,
            9, 6, 1664, 7, 4, 6, 7, 8, 34, 9, 5, 128, 7, 8, 50, 7, 6, 14, 7, 8, 62, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 8, 48, 7, 4, 3, 7, 8, 60, 7, 4, 5, 7, 8, 42, 7, 6, 16, 9, 9, 1152, 7, 4, 6, 7, 8, 32, 7, 5, 8,
            7, 8, 58, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 7, 26, 7, 5, 9, 9, 8, 640, 7, 4, 6, 7, 8, 38, 9, 5, 128, 7, 7, 25, 7, 6, 15, 9, 8, 384, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6,
            7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 12, 11, 0, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14,
            7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5, 7, 8, 39, 7, 6, 16,
            9, 8, 576, 7, 4, 6, 7, 7, 19, 7, 5, 8, 7, 8, 55, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 8, 45, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 8, 53, 7, 5, 9, 9, 8, 448, 7, 4, 6, 7, 8, 35, 9, 5, 128,
            7, 8, 51, 7, 6, 15, 7, 8, 63, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 9, 9, 1472, 7, 4, 5,
            7, 8, 43, 7, 6, 17, 9, 9, 1216, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 8, 29, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6,
            7, 8, 33, 9, 5, 128, 7, 8, 49, 7, 6, 14, 7, 8, 61, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 47, 7, 4, 3,
            7, 8, 59, 7, 4, 5, 7, 8, 41, 7, 6, 16, 9, 9, 960, 7, 4, 6, 7, 8, 31, 7, 5, 8, 7, 8, 57, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9,
            9, 9, 704, 7, 4, 6, 7, 8, 37, 9, 5, 128, 7, 7, 25, 7, 6, 15, 9, 8, 320, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8,
            9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 11, 11, 1792, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5, 7, 8, 40, 7, 6, 16, 9, 9, 832, 7, 4, 6,
            7, 7, 19, 7, 5, 8, 7, 8, 56, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 46, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 8, 54, 7, 5, 9, 9, 8, 512, 7, 4, 6, 7, 8, 36, 9, 5, 128, 7, 8, 52, 7, 6, 15,
            7, 8, 0, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 9, 9, 1600, 7, 4, 5, 7, 8, 44, 7, 6, 17,
            9, 9, 1344, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 8, 30, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 8, 34, 9, 5, 128,
            7, 8, 50, 7, 6, 14, 7, 8, 62, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 48, 7, 4, 3, 7, 8, 60, 7, 4, 5,
            7, 8, 42, 7, 6, 16, 9, 9, 1088, 7, 4, 6, 7, 8, 32, 7, 5, 8, 7, 8, 58, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9, 9, 8, 640, 7, 4, 6,
            7, 8, 38, 9, 5, 128, 7, 7, 25, 7, 6, 15, 9, 8, 384, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3,
            7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 0, 0, 0, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9,
            9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5, 7, 8, 39, 7, 6, 16, 9, 8, 576, 7, 4, 6, 7, 7, 19, 7, 5, 8,
            7, 8, 55, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 45, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 8, 53, 7, 5, 9, 9, 8, 448, 7, 4, 6, 7, 8, 35, 9, 5, 128, 7, 8, 51, 7, 6, 15, 7, 8, 63, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 9, 9, 1536, 7, 4, 5, 7, 8, 43, 7, 6, 17, 9, 9, 1280, 7, 4, 6,
            7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 29, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 8, 33, 9, 5, 128, 7, 8, 49, 7, 6, 14,
            7, 8, 61, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 47, 7, 4, 3, 7, 8, 59, 7, 4, 5, 7, 8, 41, 7, 6, 16,
            9, 9, 1024, 7, 4, 6, 7, 8, 31, 7, 5, 8, 7, 8, 57, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9, 9, 9, 768, 7, 4, 6, 7, 8, 37, 9, 5, 128,
            7, 7, 25, 7, 6, 15, 9, 8, 320, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5,
            7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 11, 11, 1856, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6,
            7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3,
            7, 7, 27, 7, 4, 5, 7, 8, 40, 7, 6, 16, 9, 9, 896, 7, 4, 6, 7, 7, 19, 7, 5, 8, 7, 8, 56, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 46, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 8, 54, 7, 5, 9,
            9, 8, 512, 7, 4, 6, 7, 8, 36, 9, 5, 128, 7, 8, 52, 7, 6, 15, 7, 8, 0, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 6, 13, 7, 4, 3, 9, 9, 1728, 7, 4, 5, 7, 8, 44, 7, 6, 17, 9, 9, 1408, 7, 4, 6, 7, 6, 1, 7, 5, 8,
            9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 30, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 8, 34, 9, 5, 128, 7, 8, 50, 7, 6, 14, 7, 8, 62, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 8, 48, 7, 4, 3, 7, 8, 60, 7, 4, 5, 7, 8, 42, 7, 6, 16, 9, 9, 1152, 7, 4, 6,
            7, 8, 32, 7, 5, 8, 7, 8, 58, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9, 9, 8, 640, 7, 4, 6, 7, 8, 38, 9, 5, 128, 7, 7, 25, 7, 6, 15,
            9, 8, 384, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17,
            9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            0, 0, 0, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128,
            7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5,
            7, 8, 39, 7, 6, 16, 9, 8, 576, 7, 4, 6, 7, 7, 19, 7, 5, 8, 7, 8, 55, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 8, 45, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 8, 53, 7, 5, 9, 9, 8, 448, 7, 4, 6,
            7, 8, 35, 9, 5, 128, 7, 8, 51, 7, 6, 15, 7, 8, 63, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3,
            9, 9, 1472, 7, 4, 5, 7, 8, 43, 7, 6, 17, 9, 9, 1216, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 29, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9,
            9, 6, 1664, 7, 4, 6, 7, 8, 33, 9, 5, 128, 7, 8, 49, 7, 6, 14, 7, 8, 61, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 8, 47, 7, 4, 3, 7, 8, 59, 7, 4, 5, 7, 8, 41, 7, 6, 16, 9, 9, 960, 7, 4, 6, 7, 8, 31, 7, 5, 8,
            7, 8, 57, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 7, 26, 7, 5, 9, 9, 9, 704, 7, 4, 6, 7, 8, 37, 9, 5, 128, 7, 7, 25, 7, 6, 15, 9, 8, 320, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6,
            7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 11, 12, 2176, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14,
            7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5, 7, 8, 40, 7, 6, 16,
            9, 9, 832, 7, 4, 6, 7, 7, 19, 7, 5, 8, 7, 8, 56, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 8, 46, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 8, 54, 7, 5, 9, 9, 8, 512, 7, 4, 6, 7, 8, 36, 9, 5, 128,
            7, 8, 52, 7, 6, 15, 7, 8, 0, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 9, 9, 1600, 7, 4, 5,
            7, 8, 44, 7, 6, 17, 9, 9, 1344, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 8, 30, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6,
            7, 8, 34, 9, 5, 128, 7, 8, 50, 7, 6, 14, 7, 8, 62, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 48, 7, 4, 3,
            7, 8, 60, 7, 4, 5, 7, 8, 42, 7, 6, 16, 9, 9, 1088, 7, 4, 6, 7, 8, 32, 7, 5, 8, 7, 8, 58, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9,
            9, 8, 640, 7, 4, 6, 7, 8, 38, 9, 5, 128, 7, 7, 25, 7, 6, 15, 9, 8, 384, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8,
            9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 0, 0, 0, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5, 7, 8, 39, 7, 6, 16, 9, 8, 576, 7, 4, 6,
            7, 7, 19, 7, 5, 8, 7, 8, 55, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 45, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 8, 53, 7, 5, 9, 9, 8, 448, 7, 4, 6, 7, 8, 35, 9, 5, 128, 7, 8, 51, 7, 6, 15,
            7, 8, 63, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 9, 9, 1536, 7, 4, 5, 7, 8, 43, 7, 6, 17,
            9, 9, 1280, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 8, 29, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 8, 33, 9, 5, 128,
            7, 8, 49, 7, 6, 14, 7, 8, 61, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 47, 7, 4, 3, 7, 8, 59, 7, 4, 5,
            7, 8, 41, 7, 6, 16, 9, 9, 1024, 7, 4, 6, 7, 8, 31, 7, 5, 8, 7, 8, 57, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9, 9, 9, 768, 7, 4, 6,
            7, 8, 37, 9, 5, 128, 7, 7, 25, 7, 6, 15, 9, 8, 320, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3,
            7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 11, 12, 2432, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9,
            9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5, 7, 8, 40, 7, 6, 16, 9, 9, 896, 7, 4, 6, 7, 7, 19, 7, 5, 8,
            7, 8, 56, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 46, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 8, 54, 7, 5, 9, 9, 8, 512, 7, 4, 6, 7, 8, 36, 9, 5, 128, 7, 8, 52, 7, 6, 15, 7, 8, 0, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 9, 9, 1728, 7, 4, 5, 7, 8, 44, 7, 6, 17, 9, 9, 1408, 7, 4, 6,
            7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 30, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 8, 34, 9, 5, 128, 7, 8, 50, 7, 6, 14,
            7, 8, 62, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 48, 7, 4, 3, 7, 8, 60, 7, 4, 5, 7, 8, 42, 7, 6, 16,
            9, 9, 1152, 7, 4, 6, 7, 8, 32, 7, 5, 8, 7, 8, 58, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9, 9, 8, 640, 7, 4, 6, 7, 8, 38, 9, 5, 128,
            7, 7, 25, 7, 6, 15, 9, 8, 384, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5,
            7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 0, 0, 0, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6,
            7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3,
            7, 7, 27, 7, 4, 5, 7, 8, 39, 7, 6, 16, 9, 8, 576, 7, 4, 6, 7, 7, 19, 7, 5, 8, 7, 8, 55, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 45, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 8, 53, 7, 5, 9,
            9, 8, 448, 7, 4, 6, 7, 8, 35, 9, 5, 128, 7, 8, 51, 7, 6, 15, 7, 8, 63, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 6, 13, 7, 4, 3, 9, 9, 1472, 7, 4, 5, 7, 8, 43, 7, 6, 17, 9, 9, 1216, 7, 4, 6, 7, 6, 1, 7, 5, 8,
            9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 29, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 8, 33, 9, 5, 128, 7, 8, 49, 7, 6, 14, 7, 8, 61, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 8, 47, 7, 4, 3, 7, 8, 59, 7, 4, 5, 7, 8, 41, 7, 6, 16, 9, 9, 960, 7, 4, 6,
            7, 8, 31, 7, 5, 8, 7, 8, 57, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9, 9, 9, 704, 7, 4, 6, 7, 8, 37, 9, 5, 128, 7, 7, 25, 7, 6, 15,
            9, 8, 320, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17,
            9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            11, 12, 2048, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128,
            7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5,
            7, 8, 40, 7, 6, 16, 9, 9, 832, 7, 4, 6, 7, 7, 19, 7, 5, 8, 7, 8, 56, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 8, 46, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 8, 54, 7, 5, 9, 9, 8, 512, 7, 4, 6,
            7, 8, 36, 9, 5, 128, 7, 8, 52, 7, 6, 15, 7, 8, 0, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3,
            9, 9, 1600, 7, 4, 5, 7, 8, 44, 7, 6, 17, 9, 9, 1344, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 30, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9,
            9, 6, 1664, 7, 4, 6, 7, 8, 34, 9, 5, 128, 7, 8, 50, 7, 6, 14, 7, 8, 62, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 8, 48, 7, 4, 3, 7, 8, 60, 7, 4, 5, 7, 8, 42, 7, 6, 16, 9, 9, 1088, 7, 4, 6, 7, 8, 32, 7, 5, 8,
            7, 8, 58, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 7, 26, 7, 5, 9, 9, 8, 640, 7, 4, 6, 7, 8, 38, 9, 5, 128, 7, 7, 25, 7, 6, 15, 9, 8, 384, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6,
            7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 0, 0, 0, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14,
            7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5, 7, 8, 39, 7, 6, 16,
            9, 8, 576, 7, 4, 6, 7, 7, 19, 7, 5, 8, 7, 8, 55, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 8, 45, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 8, 53, 7, 5, 9, 9, 8, 448, 7, 4, 6, 7, 8, 35, 9, 5, 128,
            7, 8, 51, 7, 6, 15, 7, 8, 63, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 9, 9, 1536, 7, 4, 5,
            7, 8, 43, 7, 6, 17, 9, 9, 1280, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 8, 29, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6,
            7, 8, 33, 9, 5, 128, 7, 8, 49, 7, 6, 14, 7, 8, 61, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 47, 7, 4, 3,
            7, 8, 59, 7, 4, 5, 7, 8, 41, 7, 6, 16, 9, 9, 1024, 7, 4, 6, 7, 8, 31, 7, 5, 8, 7, 8, 57, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9,
            9, 9, 768, 7, 4, 6, 7, 8, 37, 9, 5, 128, 7, 7, 25, 7, 6, 15, 9, 8, 320, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8,
            9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 11, 11, 1920, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5, 7, 8, 40, 7, 6, 16, 9, 9, 896, 7, 4, 6,
            7, 7, 19, 7, 5, 8, 7, 8, 56, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 46, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 8, 54, 7, 5, 9, 9, 8, 512, 7, 4, 6, 7, 8, 36, 9, 5, 128, 7, 8, 52, 7, 6, 15,
            7, 8, 0, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 9, 9, 1728, 7, 4, 5, 7, 8, 44, 7, 6, 17,
            9, 9, 1408, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 8, 30, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 8, 34, 9, 5, 128,
            7, 8, 50, 7, 6, 14, 7, 8, 62, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 48, 7, 4, 3, 7, 8, 60, 7, 4, 5,
            7, 8, 42, 7, 6, 16, 9, 9, 1152, 7, 4, 6, 7, 8, 32, 7, 5, 8, 7, 8, 58, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9, 9, 8, 640, 7, 4, 6,
            7, 8, 38, 9, 5, 128, 7, 7, 25, 7, 6, 15, 9, 8, 384, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3,
            7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 0, 0, 0, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9,
            9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5, 7, 8, 39, 7, 6, 16, 9, 8, 576, 7, 4, 6, 7, 7, 19, 7, 5, 8,
            7, 8, 55, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 45, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 8, 53, 7, 5, 9, 9, 8, 448, 7, 4, 6, 7, 8, 35, 9, 5, 128, 7, 8, 51, 7, 6, 15, 7, 8, 63, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 9, 9, 1472, 7, 4, 5, 7, 8, 43, 7, 6, 17, 9, 9, 1216, 7, 4, 6,
            7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 29, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 8, 33, 9, 5, 128, 7, 8, 49, 7, 6, 14,
            7, 8, 61, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 47, 7, 4, 3, 7, 8, 59, 7, 4, 5, 7, 8, 41, 7, 6, 16,
            9, 9, 960, 7, 4, 6, 7, 8, 31, 7, 5, 8, 7, 8, 57, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9, 9, 9, 704, 7, 4, 6, 7, 8, 37, 9, 5, 128,
            7, 7, 25, 7, 6, 15, 9, 8, 320, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5,
            7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 11, 12, 2304, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6,
            7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3,
            7, 7, 27, 7, 4, 5, 7, 8, 40, 7, 6, 16, 9, 9, 832, 7, 4, 6, 7, 7, 19, 7, 5, 8, 7, 8, 56, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 46, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 8, 54, 7, 5, 9,
            9, 8, 512, 7, 4, 6, 7, 8, 36, 9, 5, 128, 7, 8, 52, 7, 6, 15, 7, 8, 0, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 6, 13, 7, 4, 3, 9, 9, 1600, 7, 4, 5, 7, 8, 44, 7, 6, 17, 9, 9, 1344, 7, 4, 6, 7, 6, 1, 7, 5, 8,
            9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 30, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 8, 34, 9, 5, 128, 7, 8, 50, 7, 6, 14, 7, 8, 62, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 8, 48, 7, 4, 3, 7, 8, 60, 7, 4, 5, 7, 8, 42, 7, 6, 16, 9, 9, 1088, 7, 4, 6,
            7, 8, 32, 7, 5, 8, 7, 8, 58, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9, 9, 8, 640, 7, 4, 6, 7, 8, 38, 9, 5, 128, 7, 7, 25, 7, 6, 15,
            9, 8, 384, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17,
            9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            0, 0, 0, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128,
            7, 7, 24, 7, 6, 14, 7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5,
            7, 8, 39, 7, 6, 16, 9, 8, 576, 7, 4, 6, 7, 7, 19, 7, 5, 8, 7, 8, 55, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 8, 45, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 8, 53, 7, 5, 9, 9, 8, 448, 7, 4, 6,
            7, 8, 35, 9, 5, 128, 7, 8, 51, 7, 6, 15, 7, 8, 63, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3,
            9, 9, 1536, 7, 4, 5, 7, 8, 43, 7, 6, 17, 9, 9, 1280, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 29, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9,
            9, 6, 1664, 7, 4, 6, 7, 8, 33, 9, 5, 128, 7, 8, 49, 7, 6, 14, 7, 8, 61, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 8, 47, 7, 4, 3, 7, 8, 59, 7, 4, 5, 7, 8, 41, 7, 6, 16, 9, 9, 1024, 7, 4, 6, 7, 8, 31, 7, 5, 8,
            7, 8, 57, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5,
            7, 7, 26, 7, 5, 9, 9, 9, 768, 7, 4, 6, 7, 8, 37, 9, 5, 128, 7, 7, 25, 7, 6, 15, 9, 8, 320, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6,
            7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 11, 12, 2560, 7, 4, 3,
            7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6, 7, 7, 20, 9, 5, 128, 7, 7, 24, 7, 6, 14,
            7, 7, 28, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 23, 7, 4, 3, 7, 7, 27, 7, 4, 5, 7, 8, 40, 7, 6, 16,
            9, 9, 896, 7, 4, 6, 7, 7, 19, 7, 5, 8, 7, 8, 56, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 8, 46, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 8, 54, 7, 5, 9, 9, 8, 512, 7, 4, 6, 7, 8, 36, 9, 5, 128,
            7, 8, 52, 7, 6, 15, 7, 8, 0, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 6, 13, 7, 4, 3, 9, 9, 1728, 7, 4, 5,
            7, 8, 44, 7, 6, 17, 9, 9, 1408, 7, 4, 6, 7, 6, 1, 7, 5, 8, 9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4,
            7, 4, 2, 7, 4, 7, 7, 8, 30, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 6, 12, 7, 5, 9, 9, 6, 1664, 7, 4, 6,
            7, 8, 34, 9, 5, 128, 7, 8, 50, 7, 6, 14, 7, 8, 62, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 8, 48, 7, 4, 3,
            7, 8, 60, 7, 4, 5, 7, 8, 42, 7, 6, 16, 9, 9, 1152, 7, 4, 6, 7, 8, 32, 7, 5, 8, 7, 8, 58, 9, 5, 64,
            7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7, 7, 7, 22, 7, 4, 3, 7, 5, 11, 7, 4, 5, 7, 7, 26, 7, 5, 9,
            9, 8, 640, 7, 4, 6, 7, 8, 38, 9, 5, 128, 7, 7, 25, 7, 6, 15, 9, 8, 384, 7, 4, 4, 7, 4, 2, 7, 4, 7,
            7, 6, 13, 7, 4, 3, 7, 7, 18, 7, 4, 5, 7, 7, 21, 7, 6, 17, 9, 7, 256, 7, 4, 6, 7, 6, 1, 7, 5, 8,
            9, 6, 192, 9, 5, 64, 7, 5, 10, 7, 4, 4, 7, 4, 2, 7, 4, 7
        };

        private static readonly int[] m_faxBlackTable = 
        { 
            12, 11, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 9, 15, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 10, 18, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 10, 17, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 11, 11, 1792, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 11, 23, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 11, 20, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 11, 25, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 12, 128, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 12, 56, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 12, 30, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 11, 11, 1856, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 57, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 11, 21, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 54, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 9, 15, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 52, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 48, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            11, 12, 2112, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 12, 44, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 36, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 12, 384, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 28, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 12, 60, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 40, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 11, 12, 2368, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 10, 16, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 10, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            10, 10, 64, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 9, 15, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 10, 18, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 10, 17, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 11, 12, 1984, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 50, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 12, 34, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 13, 1664, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 26, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 13, 1408, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 32, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 11, 11, 1920, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 12, 61, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 42, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 10, 13, 1024, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 9, 15, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            10, 13, 768, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 12, 62, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 11, 12, 2240, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 46, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 12, 38, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 13, 512, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 11, 19, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 11, 24, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 11, 22, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            11, 12, 2496, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 10, 16, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 10, 0, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 10, 64, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 12, 11, 0, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 9, 15, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 10, 18, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 10, 17, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 11, 11, 1792, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 11, 23, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 11, 20, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 11, 25, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 10, 12, 192, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 13, 1280, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 12, 31, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 11, 11, 1856, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 58, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 11, 21, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 13, 896, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 9, 15, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 13, 640, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 49, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 11, 12, 2176, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 12, 45, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 37, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 10, 12, 448, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 29, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            10, 13, 1536, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 12, 41, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 11, 12, 2432, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 10, 16, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 10, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 10, 64, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 9, 15, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 10, 18, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 10, 17, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            11, 12, 2048, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 12, 51, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 35, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 12, 320, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 27, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 12, 59, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 33, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 11, 11, 1920, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 12, 256, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 43, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            10, 13, 1152, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 9, 15, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 55, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 12, 63, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 11, 12, 2304, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 47, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 12, 39, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 53, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 11, 19, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 11, 24, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 11, 22, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 11, 12, 2560, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 10, 16, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 10, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 10, 10, 64, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 12, 11, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 9, 15, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 10, 18, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 10, 17, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 11, 11, 1792, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 11, 23, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 11, 20, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 11, 25, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            10, 12, 128, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 56, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 30, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            11, 11, 1856, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 12, 57, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 11, 21, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 54, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 9, 15, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 12, 52, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 48, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 11, 12, 2112, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 44, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 36, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            10, 12, 384, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 12, 28, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 60, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 12, 40, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 11, 12, 2368, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 10, 16, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 10, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 10, 64, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 9, 15, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 10, 18, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 10, 17, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 11, 12, 1984, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 12, 50, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 34, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 10, 13, 1728, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 26, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            10, 13, 1472, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 12, 32, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 11, 11, 1920, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 61, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 12, 42, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 13, 1088, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 9, 15, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 13, 832, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 62, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            11, 12, 2240, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 12, 46, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 38, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 13, 576, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 11, 19, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 11, 24, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 11, 22, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 11, 12, 2496, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 10, 16, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 10, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            10, 10, 64, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 12, 11, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 9, 15, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 10, 18, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 10, 17, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 11, 11, 1792, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 11, 23, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 11, 20, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 11, 25, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 12, 192, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 13, 1344, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 31, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 11, 11, 1856, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 12, 58, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 11, 21, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 10, 13, 960, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 9, 15, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            10, 13, 704, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 12, 49, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 11, 12, 2176, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 45, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 12, 37, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 12, 448, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 12, 29, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 13, 1600, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 41, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            11, 12, 2432, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 10, 16, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 10, 0, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 10, 64, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 9, 15, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 10, 18, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 10, 17, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 11, 12, 2048, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 51, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 35, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            10, 12, 320, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 12, 27, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 59, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 12, 33, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 11, 11, 1920, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 12, 256, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 12, 43, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 13, 1216, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 9, 15, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 55, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 63, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 11, 12, 2304, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 12, 47, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 12, 39, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 12, 53, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 0, 0, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 8, 13, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 11, 19, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 11, 24, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 11, 22, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 11, 12, 2560, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 7, 10, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 10, 16, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 10, 0, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 10, 10, 64, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 9, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 11, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3,
            8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2,
            8, 8, 14, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 6, 8, 8, 2, 3,
            8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 7, 12, 8, 2, 3, 8, 3, 1, 8, 2, 2,
            8, 4, 6, 8, 2, 3, 8, 3, 4, 8, 2, 2, 8, 5, 7, 8, 2, 3, 8, 3, 1, 8, 2, 2, 8, 4, 5, 8, 2, 3,
            8, 3, 4, 8, 2, 2
        };

        private static readonly byte[] fillMasks = 
        {
            0x00, 0x80, 0xc0, 0xe0, 0xf0, 0xf8, 0xfc, 0xfe, 0xff
        };
    }

    class CCITTCodecTagMethods : TiffTagMethods
    {
        public override bool SetField(Tiff tif, TiffTag tag, FieldValue[] ap)
        {
            CCITTCodec sp = tif.m_currentCodec as CCITTCodec;
            switch (tag)
            {
                case TiffTag.FAXMODE:
                    sp.m_mode = (FaxMode)ap[0].ToShort();
                    return true; /* NB: pseudo tag */
                case TiffTag.FAXFILLFUNC:
                    sp.fill = ap[0].Value as Tiff.FaxFillFunc;
                    return true; /* NB: pseudo tag */
                case TiffTag.GROUP3OPTIONS:
                    if (tif.m_dir.td_compression == Compression.CCITTFAX3)
                        sp.m_groupoptions = (Group3Opt)ap[0].ToShort();
                    break;
                case TiffTag.GROUP4OPTIONS:
                    if (tif.m_dir.td_compression == Compression.CCITTFAX4)
                        sp.m_groupoptions = (Group3Opt)ap[0].ToShort();
                    break;
                case TiffTag.BADFAXLINES:
                    sp.m_badfaxlines = ap[0].ToInt();
                    break;
                case TiffTag.CLEANFAXDATA:
                    sp.m_cleanfaxdata = (CleanFaxData)ap[0].ToByte();
                    break;
                case TiffTag.CONSECUTIVEBADFAXLINES:
                    sp.m_badfaxrun = ap[0].ToInt();
                    break;
                case TiffTag.FAXRECVPARAMS:
                    sp.m_recvparams = ap[0].ToInt();
                    break;
                case TiffTag.FAXSUBADDRESS:
                    Tiff.setString(out sp.m_subaddress, ap[0].ToString());
                    break;
                case TiffTag.FAXRECVTIME:
                    sp.m_recvtime = ap[0].ToInt();
                    break;
                case TiffTag.FAXDCS:
                    Tiff.setString(out sp.m_faxdcs, ap[0].ToString());
                    break;
                default:
                    return base.SetField(tif, tag, ap);
            }

            TiffFieldInfo fip = tif.FieldWithTag(tag);
            if (fip != null)
                tif.setFieldBit(fip.Bit);
            else
                return false;

            tif.m_flags |= TiffFlags.DIRTYDIRECT;
            return true;
        }

        public override FieldValue[] GetField(Tiff tif, TiffTag tag)
        {
            CCITTCodec sp = tif.m_currentCodec as CCITTCodec;
            FieldValue[] result = new FieldValue[1];

            switch (tag)
            {
                case TiffTag.FAXMODE:
                    result[0].Set(sp.m_mode);
                    break;
                case TiffTag.FAXFILLFUNC:
                    result[0].Set(sp.fill);
                    break;
                case TiffTag.GROUP3OPTIONS:
                case TiffTag.GROUP4OPTIONS:
                    result[0].Set(sp.m_groupoptions);
                    break;
                case TiffTag.BADFAXLINES:
                    result[0].Set(sp.m_badfaxlines);
                    break;
                case TiffTag.CLEANFAXDATA:
                    result[0].Set(sp.m_cleanfaxdata);
                    break;
                case TiffTag.CONSECUTIVEBADFAXLINES:
                    result[0].Set(sp.m_badfaxrun);
                    break;
                case TiffTag.FAXRECVPARAMS:
                    result[0].Set(sp.m_recvparams);
                    break;
                case TiffTag.FAXSUBADDRESS:
                    result[0].Set(sp.m_subaddress);
                    break;
                case TiffTag.FAXRECVTIME:
                    result[0].Set(sp.m_recvtime);
                    break;
                case TiffTag.FAXDCS:
                    result[0].Set(sp.m_faxdcs);
                    break;
                default:
                    return base.GetField(tif, tag);
            }

            return result;
        }
    }
}

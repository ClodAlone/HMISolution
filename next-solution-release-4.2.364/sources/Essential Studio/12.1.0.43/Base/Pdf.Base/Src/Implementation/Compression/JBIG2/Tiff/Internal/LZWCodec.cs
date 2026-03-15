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
    class LZWCodec : CodecWithPredictor
    {
        private bool LZW_CHECKEOS = true;
        private const short BITS_MIN = 9;
        private const short BITS_MAX = 12;
        private const short CODE_CLEAR = 256; 
        private const short CODE_EOI = 257;   
        private const short CODE_FIRST = 258; 
        private const short CODE_MAX = ((1 << BITS_MAX) - 1);
        private const short CODE_MIN = ((1 << BITS_MIN) - 1);

        private const int HSIZE = 9001;       
        private const int HSHIFT = (13 - 8);
        private const int CSIZE = (((1 << BITS_MAX) - 1) + 1024);

        private const int CHECK_GAP = 10000;  

        private struct code_t
        {
            public int next;
            public short length;
            public byte value;
            public byte firstchar; 
        };

        private struct hash_t
        {
            public int hash;
            public short code;
        };

        private bool m_compatDecode;

        private short m_nbits;
        private short m_maxcode;
        private short m_free_ent;
        private int m_nextdata;
        private int m_nextbits;

        private int m_rw_mode; 

        private int m_dec_nbitsmask;
        private int m_dec_restart;
        private int m_dec_bitsleft;
        private bool m_oldStyleCodeFound;
        private int m_dec_codep;
        private int m_dec_oldcodep;
        private int m_dec_free_entp;
        private int m_dec_maxcodep;
        private code_t[] m_dec_codetab;

        private int m_enc_oldcode;
        private int m_enc_checkpoint;
        private int m_enc_ratio;
        private int m_enc_incount;
        private int m_enc_outcount; 
        private int m_enc_rawlimit; 
        private hash_t[] m_enc_hashtab;

        public LZWCodec(Tiff tif, Compression scheme, string name)
            : base(tif, scheme, name)
        {
        }

        public override bool Init()
        {
            m_dec_codetab = null;
            m_oldStyleCodeFound = false;
            m_enc_hashtab = null;
            m_rw_mode = m_tif.m_mode;
            m_compatDecode = false;

            TIFFPredictorInit(null);
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
        /// Prepares the decoder part of the codec for a decoding.
        /// </summary>
        public override bool PreDecode(short plane)
        {
            return LZWPreDecode(plane);
        }

        /// <summary>
        /// Cleanups the state of the codec.
        /// </summary>
        public override void Cleanup()
        {
            LZWCleanup();
            m_tif.m_mode = m_rw_mode;
        }

        public override bool predictor_setupdecode()
        {
            return LZWSetupDecode();
        }

        public override bool predictor_decoderow(byte[] buffer, int offset, int count, short plane)
        {
            if (m_compatDecode)
                return LZWDecodeCompat(buffer, offset, count, plane);

            return LZWDecode(buffer, offset, count, plane);
        }

        public override bool predictor_decodestrip(byte[] buffer, int offset, int count, short plane)
        {
            if (m_compatDecode)
                return LZWDecodeCompat(buffer, offset, count, plane);

            return LZWDecode(buffer, offset, count, plane);
        }

        public override bool predictor_decodetile(byte[] buffer, int offset, int count, short plane)
        {
            if (m_compatDecode)
                return LZWDecodeCompat(buffer, offset, count, plane);

            return LZWDecode(buffer, offset, count, plane);
        }

        private bool LZWSetupDecode()
        {
            if (m_dec_codetab == null)
            {
                m_dec_codetab = new code_t [CSIZE];

                int code = 255;
                do
                {
                    m_dec_codetab[code].value = (byte)code;
                    m_dec_codetab[code].firstchar = (byte)code;
                    m_dec_codetab[code].length = 1;
                    m_dec_codetab[code].next = -1;
                }
                while (code-- != 0);

                Array.Clear(m_dec_codetab, CODE_CLEAR, CODE_FIRST - CODE_CLEAR);
            }

            return true;
        }

        private bool LZWPreDecode(short s)
        {
            if (m_dec_codetab == null)
                SetupDecode();

            if (m_tif.m_rawdata[0] == 0 && (m_tif.m_rawdata[1] & 0x1) != 0)
            {
                if (!m_oldStyleCodeFound)
                {
                    m_compatDecode = true;

                    SetupDecode();
                    m_oldStyleCodeFound = true;
                }

                m_maxcode = CODE_MIN;
            }
            else
            {
                m_maxcode = CODE_MIN - 1;
                m_oldStyleCodeFound = false;
            }

            m_nbits = BITS_MIN;
            m_nextbits = 0;
            m_nextdata = 0;

            m_dec_restart = 0;
            m_dec_nbitsmask = CODE_MIN;
            m_dec_bitsleft = m_tif.m_rawcc << 3;
            m_dec_free_entp = CODE_FIRST;

            Array.Clear(m_dec_codetab, m_dec_free_entp, CSIZE - CODE_FIRST);
            m_dec_oldcodep = -1;
            m_dec_maxcodep = m_dec_nbitsmask - 1;
            return true;
        }

        private bool LZWDecode(byte[] buffer, int offset, int count, short plane)
        {
            if (m_dec_restart != 0)
            {
                int codep = m_dec_codep;
                int residue = m_dec_codetab[codep].length - m_dec_restart;
                if (residue > count)
                {
                    m_dec_restart += count;

                    do
                    {
                        codep = m_dec_codetab[codep].next;
                    }
                    while (--residue > count && codep != -1);

                    if (codep != -1)
                    {
                        int tp = count;
                        do
                        {
                            tp--;
                            buffer[offset + tp] = m_dec_codetab[codep].value;
                            codep = m_dec_codetab[codep].next;
                        }
                        while (--count != 0 && codep != -1);
                    }

                    return true;
                }

                offset += residue;
                count -= residue;
                int ttp = 0;
                do
                {
                    --ttp;
                    int t = m_dec_codetab[codep].value;
                    codep = m_dec_codetab[codep].next;
                    buffer[offset + ttp] = (byte)t;
                }
                while (--residue != 0 && codep != -1);

                m_dec_restart = 0;
            }

            while (count > 0)
            {
                short code;
                NextCode(out code, false);
                if (code == CODE_EOI)
                    break;

                if (code == CODE_CLEAR)
                {
                    m_dec_free_entp = CODE_FIRST;
                    Array.Clear(m_dec_codetab, m_dec_free_entp, CSIZE - CODE_FIRST);

                    m_nbits = BITS_MIN;
                    m_dec_nbitsmask = CODE_MIN;
                    m_dec_maxcodep = m_dec_nbitsmask - 1;
                    NextCode(out code, false);
                    
                    if (code == CODE_EOI)
                        break;
                    
                    if (code == CODE_CLEAR)
                    {
                        return false;
                    }

                    buffer[offset] = (byte)code;
                    offset++;
                    count--;
                    m_dec_oldcodep = code;
                    continue;
                }

                int codep = code;

                if (m_dec_free_entp < 0 || m_dec_free_entp >= CSIZE)
                {
                    return false;
                }

                m_dec_codetab[m_dec_free_entp].next = m_dec_oldcodep;
                if (m_dec_codetab[m_dec_free_entp].next < 0 || m_dec_codetab[m_dec_free_entp].next >= CSIZE)
                {
                    return false;
                }

                m_dec_codetab[m_dec_free_entp].firstchar = m_dec_codetab[m_dec_codetab[m_dec_free_entp].next].firstchar;
                m_dec_codetab[m_dec_free_entp].length = (short)(m_dec_codetab[m_dec_codetab[m_dec_free_entp].next].length + 1);
                m_dec_codetab[m_dec_free_entp].value = (codep < m_dec_free_entp) ? m_dec_codetab[codep].firstchar : m_dec_codetab[m_dec_free_entp].firstchar;

                if (++m_dec_free_entp > m_dec_maxcodep)
                {
                    if (++m_nbits > BITS_MAX)
                    {
                        m_nbits = BITS_MAX;
                    }

                    m_dec_nbitsmask = MAXCODE(m_nbits);
                    m_dec_maxcodep = m_dec_nbitsmask - 1;
                }

                m_dec_oldcodep = code;
                if (code >= 256)
                {
                    if (m_dec_codetab[codep].length == 0)
                    {
                        return false;
                    }

                    if (m_dec_codetab[codep].length > count)
                    {
                        m_dec_codep = code;
                        do
                        {
                            codep = m_dec_codetab[codep].next;
                        }
                        while (codep != -1 && m_dec_codetab[codep].length > count);

                        if (codep != -1)
                        {
                            m_dec_restart = count;
                            int tp = count;
                            do
                            {
                                tp--;
                                buffer[offset + tp] = m_dec_codetab[codep].value;
                                codep = m_dec_codetab[codep].next;
                            }
                            while (--count != 0 && codep != -1);
                        }
                        break;
                    }

                    int len = m_dec_codetab[codep].length;
                    int ttp = len;
                    do
                    {
                        --ttp;
                        int t = m_dec_codetab[codep].value;
                        codep = m_dec_codetab[codep].next;
                        buffer[offset + ttp] = (byte)t;
                    }
                    while (codep != -1 && ttp > 0);

                    if (codep != -1)
                    {
                        break;
                    }

                    offset += len;
                    count -= len;
                }
                else
                {
                    buffer[offset] = (byte)code;
                    offset++;
                    count--;
                }
            }

            if (count > 0)
            {
                return false;
            }

            return true;
        }

        private bool LZWDecodeCompat(byte[] buffer, int offset, int count, short plane)
        {
            if (m_dec_restart != 0)
            {
                int residue;

                int codep = m_dec_codep;
                residue = m_dec_codetab[codep].length - m_dec_restart;
                if (residue > count)
                {
                    m_dec_restart += count;
                    do
                    {
                        codep = m_dec_codetab[codep].next;
                    }
                    while (--residue > count);

                    int tp = count;
                    do
                    {
                        --tp;
                        buffer[offset + tp] = m_dec_codetab[codep].value;
                        codep = m_dec_codetab[codep].next;
                    }
                    while (--count != 0);

                    return true;
                }

                offset += residue;
                count -= residue;
                int ttp = 0;
                do
                {
                    --ttp;
                    buffer[offset + ttp] = m_dec_codetab[codep].value;
                    codep = m_dec_codetab[codep].next;
                }
                while (--residue != 0);

                m_dec_restart = 0;
            }

            while (count > 0)
            {
                short code;
                NextCode(out code, true);
                if (code == CODE_EOI)
                    break;
                
                if (code == CODE_CLEAR)
                {
                    m_dec_free_entp = CODE_FIRST;
                    Array.Clear(m_dec_codetab, m_dec_free_entp, CSIZE - CODE_FIRST);

                    m_nbits = BITS_MIN;
                    m_dec_nbitsmask = CODE_MIN;
                    m_dec_maxcodep = m_dec_nbitsmask;
                    NextCode(out code, true);
                    
                    if (code == CODE_EOI)
                        break;

                    if (code == CODE_CLEAR)
                    {
                        return false;
                    }

                    buffer[offset] = (byte)code;
                    offset++;
                    count--;
                    m_dec_oldcodep = code;
                    continue;
                }

                int codep = code;

                if (m_dec_free_entp < 0 || m_dec_free_entp >= CSIZE)
                {
                    return false;
                }

                m_dec_codetab[m_dec_free_entp].next = m_dec_oldcodep;
                if (m_dec_codetab[m_dec_free_entp].next < 0 || m_dec_codetab[m_dec_free_entp].next >= CSIZE)
                {
                    return false;
                }

                m_dec_codetab[m_dec_free_entp].firstchar = m_dec_codetab[m_dec_codetab[m_dec_free_entp].next].firstchar;
                m_dec_codetab[m_dec_free_entp].length = (short)(m_dec_codetab[m_dec_codetab[m_dec_free_entp].next].length + 1);
                m_dec_codetab[m_dec_free_entp].value = (codep < m_dec_free_entp) ? m_dec_codetab[codep].firstchar : m_dec_codetab[m_dec_free_entp].firstchar;
                if (++m_dec_free_entp > m_dec_maxcodep)
                {
                    if (++m_nbits > BITS_MAX)
                    {
                        // should not happen
                        m_nbits = BITS_MAX;
                    }
                    m_dec_nbitsmask = MAXCODE(m_nbits);
                    m_dec_maxcodep = m_dec_nbitsmask;
                }

                m_dec_oldcodep = code;
                if (code >= 256)
                {
                    int op_orig = offset;

                    if (m_dec_codetab[codep].length == 0)
                    {
                        return false;
                    }

                    if (m_dec_codetab[codep].length > count)
                    {
                        m_dec_codep = code;
                        do
                        {
                            codep = m_dec_codetab[codep].next;
                        }
                        while (m_dec_codetab[codep].length > count);

                        m_dec_restart = count;
                        int tp = count;
                        do
                        {
                            --tp;
                            buffer[offset + tp] = m_dec_codetab[codep].value;
                            codep = m_dec_codetab[codep].next;
                        }
                        while (--count != 0);

                        break;
                    }

                    offset += m_dec_codetab[codep].length;
                    count -= m_dec_codetab[codep].length;
                    int ttp = offset;
                    do
                    {
                        --ttp;
                        buffer[ttp] = m_dec_codetab[codep].value;
                        codep = m_dec_codetab[codep].next;
                    }
                    while (codep != -1 && ttp > op_orig);
                }
                else
                {
                    buffer[offset] = (byte)code;
                    offset++;
                    count--;
                }
            }

            if (count > 0)
            {
                return false;
            }

            return true;
        }
        
        private void LZWCleanup()
        {
            m_dec_codetab = null;
            m_enc_hashtab = null;
        }

        private static int MAXCODE(int n)
        {
            return ((1 << n) - 1);
        }

        private void NextCode(out short _code, bool compat)
        {
            if (LZW_CHECKEOS)
            {
                if (m_dec_bitsleft < m_nbits)
                {
                    _code = CODE_EOI;
                }
                else
                {
                    if (compat)
                        GetNextCodeCompat(out _code);
                    else
                        GetNextCode(out _code);

                    m_dec_bitsleft -= m_nbits;
                }
            }
            else
            {
                if (compat)
                    GetNextCodeCompat(out _code);
                else
                    GetNextCode(out _code);
            }
        }

        private void GetNextCode(out short code)
        {
            m_nextdata = (m_nextdata << 8) | m_tif.m_rawdata[m_tif.m_rawcp];
            m_tif.m_rawcp++;
            m_nextbits += 8;
            if (m_nextbits < m_nbits)
            {
                m_nextdata = (m_nextdata << 8) | m_tif.m_rawdata[m_tif.m_rawcp];
                m_tif.m_rawcp++;
                m_nextbits += 8;
            }
            code = (short)((m_nextdata >> (m_nextbits - m_nbits)) & m_dec_nbitsmask);
            m_nextbits -= m_nbits;
        }

        private void GetNextCodeCompat(out short code)
        {
            m_nextdata |= m_tif.m_rawdata[m_tif.m_rawcp] << m_nextbits;
            m_tif.m_rawcp++;
            m_nextbits += 8;
            if (m_nextbits < m_nbits)
            {
                m_nextdata |= m_tif.m_rawdata[m_tif.m_rawcp] << m_nextbits;
                m_tif.m_rawcp++;
                m_nextbits += 8;
            }
            code = (short)(m_nextdata & m_dec_nbitsmask);
            m_nextdata >>= m_nbits;
            m_nextbits -= m_nbits;
        }
    }
}
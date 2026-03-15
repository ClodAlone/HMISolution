#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.Compression.JBIG2.ZLib;

namespace Syncfusion.Pdf.Compression.JBIG2.Internal
{
    class DeflateCodec : CodecWithPredictor
    {
        public const int ZSTATE_INIT_DECODE = 0x01;
        public const int ZSTATE_INIT_ENCODE = 0x02;

        public ZStream m_stream = new ZStream();
        public int m_zipquality;
        public int m_state;

        private static readonly TiffFieldInfo[] zipFieldInfo = 
        {
            new TiffFieldInfo(TiffTag.ZIPQUALITY, 0, 0, TiffType.ANY, FieldBit.Pseudo, true, false, string.Empty), 
        };

        private TiffTagMethods m_tagMethods;

        public DeflateCodec(Tiff tif, Compression scheme, string name)
            : base(tif, scheme, name)
        {
            m_tagMethods = new DeflateCodecTagMethods();
        }

        public override bool Init()
        {
            m_tif.MergeFieldInfo(zipFieldInfo, zipFieldInfo.Length);

            m_zipquality = zlibConst.Z_DEFAULT_COMPRESSION; /* default comp. level */
            m_state = 0;

            TIFFPredictorInit(m_tagMethods);
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
            return ZIPPreDecode(plane);
        }

        /// <summary>
        /// Cleanups the state of the codec.
        /// </summary>
        public override void Cleanup()
        {
            ZIPCleanup();
        }

        public override bool predictor_setupdecode()
        {
            return ZIPSetupDecode();
        }

        public override bool predictor_decoderow(byte[] buffer, int offset, int count, short plane)
        {
            return ZIPDecode(buffer, offset, count, plane);
        }

        public override bool predictor_decodestrip(byte[] buffer, int offset, int count, short plane)
        {
            return ZIPDecode(buffer, offset, count, plane);
        }

        public override bool predictor_decodetile(byte[] buffer, int offset, int count, short plane)
        {
            return ZIPDecode(buffer, offset, count, plane);
        }

        private void ZIPCleanup()
        {
            base.TIFFPredictorCleanup();

            if ((m_state & ZSTATE_INIT_ENCODE) != 0)
            {
                m_stream.deflateEnd();
                m_state = 0;
            }
            else if ((m_state & ZSTATE_INIT_DECODE) != 0)
            {
                m_stream.inflateEnd();
                m_state = 0;
            }
        }

        private bool ZIPDecode(byte[] buffer, int offset, int count, short plane)
        {
            const string module = "ZIPDecode";

            m_stream.next_out = buffer;
            m_stream.next_out_index = offset;
            m_stream.avail_out = count;
            do
            {
                int state = m_stream.inflate(zlibConst.Z_PARTIAL_FLUSH);
                if (state == zlibConst.Z_STREAM_END)
                    break;

                if (state == zlibConst.Z_DATA_ERROR)
                {
                    if (m_stream.inflateSync() != zlibConst.Z_OK)
                        return false;
                    
                    continue;
                }

                if (state != zlibConst.Z_OK)
                {
                    return false;
                }
            }
            while (m_stream.avail_out > 0);

            if (m_stream.avail_out != 0)
            {
                return false;
            }

            return true;
        }

        private bool ZIPPreDecode(short s)
        {
            if ((m_state & ZSTATE_INIT_DECODE) == 0)
                SetupDecode();

            m_stream.next_in = m_tif.m_rawdata;
            m_stream.next_in_index = 0;
            m_stream.avail_in = m_tif.m_rawcc;
            return (m_stream.inflateInit() == zlibConst.Z_OK);
        }

        private bool ZIPSetupDecode()
        {
            const string module = "ZIPSetupDecode";

            if ((m_state & ZSTATE_INIT_ENCODE) != 0)
            {
                m_stream.deflateEnd();
                m_state = 0;
            }

            if (m_stream.inflateInit() != zlibConst.Z_OK)
            {
                return false;
            }

            m_state |= ZSTATE_INIT_DECODE;
            return true;
        }
    }

    class DeflateCodecTagMethods : TiffTagMethods
    {
        public override bool SetField(Tiff tif, TiffTag tag, FieldValue[] ap)
        {
            DeflateCodec sp = tif.m_currentCodec as DeflateCodec;
            const string module = "ZIPVSetField";

            switch (tag)
            {
                case TiffTag.ZIPQUALITY:
                    sp.m_zipquality = ap[0].ToInt();
                    if ((sp.m_state & DeflateCodec.ZSTATE_INIT_ENCODE) != 0)
                    {
                        if (sp.m_stream.deflateParams(sp.m_zipquality, zlibConst.Z_DEFAULT_STRATEGY) != zlibConst.Z_OK)
                        {
                            return false;
                        }
                    }

                    return true;
            }

            return base.SetField(tif, tag, ap);
        }

        public override FieldValue[] GetField(Tiff tif, TiffTag tag)
        {
            DeflateCodec sp = tif.m_currentCodec as DeflateCodec;
            switch (tag)
            {
                case TiffTag.ZIPQUALITY:
                    FieldValue[] result = new FieldValue[1];
                    result[0].Set(sp.m_zipquality);
                    return result;
            }

            return base.GetField(tif, tag);
        }
    }
}

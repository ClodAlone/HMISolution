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
    class CodecWithPredictor : TiffCodec
    {
        public const int FIELD_PREDICTOR = (FieldBit.Codec + 0);
        
        private enum PredictorType
        {
            ptNone,
            ptHorAcc8,
            ptHorAcc16,
            ptHorAcc32,
            ptSwabHorAcc16,
            ptSwabHorAcc32,
            ptHorDiff8,
            ptHorDiff16,
            ptHorDiff32,
            ptFpAcc,
            ptFpDiff,
        };

        private static readonly TiffFieldInfo[] m_predictFieldInfo = 
        {
            new TiffFieldInfo(TiffTag.PREDICTOR, 1, 1, TiffType.SHORT, CodecWithPredictor.FIELD_PREDICTOR, false, false, "Predictor"), 
        };

        /// <summary>
        /// predictor tag value
        /// </summary>
        private Predictor m_predictor;

        /// <summary>
        /// sample stride over data
        /// </summary>
        private int m_stride;

        /// <summary>
        /// tile/strip row size
        /// </summary>
        private int m_rowSize;

        private TiffTagMethods m_parentTagMethods;
        private TiffTagMethods m_tagMethods;
        private TiffTagMethods m_childTagMethods; // could be null

        private bool m_passThruDecode;
        private bool m_passThruEncode;

        /// <summary>
        /// horizontal differencer/accumulator
        /// </summary>
        private PredictorType m_predictorType;

        public CodecWithPredictor(Tiff tif, Compression scheme, string name)
            : base(tif, scheme, name)
        {
            m_tagMethods = new CodecWithPredictorTagMethods();
        }

        public void TIFFPredictorInit(TiffTagMethods tagMethods)
        {
            m_tif.MergeFieldInfo(m_predictFieldInfo, m_predictFieldInfo.Length);
            m_childTagMethods = tagMethods;
            m_parentTagMethods = m_tif.m_tagmethods;
            m_tif.m_tagmethods = m_tagMethods;

            m_predictor = Predictor.NONE; // default value
            m_predictorType = PredictorType.ptNone; // no predictor method
        }

        public void TIFFPredictorCleanup()
        {
            m_tif.m_tagmethods = m_parentTagMethods;
        }

        /// <summary>
        /// Setups the decoder part of the codec.
        /// </summary>
        public override bool SetupDecode()
        {
            return PredictorSetupDecode();
        }

        /// <summary>
        /// Decodes one row of image data.
        /// </summary>
        public override bool DecodeRow(byte[] buffer, int offset, int count, short plane)
        {
            if (!m_passThruDecode)
                return PredictorDecodeRow(buffer, offset, count, plane);

            return predictor_decoderow(buffer, offset, count, plane);
        }

        /// <summary>
        public override bool DecodeStrip(byte[] buffer, int offset, int count, short plane)
        {
            if (!m_passThruDecode)
                return PredictorDecodeTile(buffer, offset, count, plane);

            return predictor_decodestrip(buffer, offset, count, plane);
        }

        /// <summary>
        /// Decodes one tile of image data.
        /// </summary>
        public override bool DecodeTile(byte[] buffer, int offset, int count, short plane)
        {
            if (!m_passThruDecode)
                return PredictorDecodeTile(buffer, offset, count, plane);

            return predictor_decodetile(buffer, offset, count, plane);
        }

        public virtual bool predictor_setupdecode()
        {
            return base.SetupDecode();
        }

        public virtual bool predictor_decoderow(byte[] buffer, int offset, int count, short plane)
        {
            return base.DecodeRow(buffer, offset, count, plane);
        }

        public virtual bool predictor_decodestrip(byte[] buffer, int offset, int count, short plane)
        {
            return base.DecodeStrip(buffer, offset, count, plane);
        }

        public virtual bool predictor_decodetile(byte[] buffer, int offset, int count, short plane)
        {
            return base.DecodeTile(buffer, offset, count, plane);
        }

        public Predictor GetPredictorValue()
        {
            return m_predictor;
        }

        public void SetPredictorValue(Predictor value)
        {
            m_predictor = value;
        }

        // retrieves child object's tag methods (could be null)
        public TiffTagMethods GetChildTagMethods()
        {
            return m_childTagMethods;
        }

        private void predictorFunc(byte[] buffer, int offset, int count)
        {
            switch (m_predictorType)
            {
                case PredictorType.ptHorAcc8:
                    horAcc8(buffer, offset, count);
                    break;
                case PredictorType.ptHorAcc16:
                    horAcc16(buffer, offset, count);
                    break;
                case PredictorType.ptHorAcc32:
                    horAcc32(buffer, offset, count);
                    break;
                case PredictorType.ptSwabHorAcc16:
                    swabHorAcc16(buffer, offset, count);
                    break;
                case PredictorType.ptSwabHorAcc32:
                    swabHorAcc32(buffer, offset, count);
                    break;
                case PredictorType.ptHorDiff8:
                    horDiff8(buffer, offset, count);
                    break;
                case PredictorType.ptHorDiff16:
                    horDiff16(buffer, offset, count);
                    break;
                case PredictorType.ptHorDiff32:
                    horDiff32(buffer, offset, count);
                    break;
                case PredictorType.ptFpAcc:
                    fpAcc(buffer, offset, count);
                    break;
                case PredictorType.ptFpDiff:
                    fpDiff(buffer, offset, count);
                    break;
            }
        }

        private void horAcc8(byte[] buffer, int offset, int count)
        {
            int cp = offset;
            if (count > m_stride)
            {
                count -= m_stride;
                
                if (m_stride == 3)
                {
                    int cr = buffer[cp];
                    int cg = buffer[cp + 1];
                    int cb = buffer[cp + 2];
                    do
                    {
                        count -= 3;
                        cp += 3;

                        cr += buffer[cp];
                        buffer[cp] = (byte)cr;
                        
                        cg += buffer[cp + 1];
                        buffer[cp + 1] = (byte)cg;

                        cb += buffer[cp + 2];
                        buffer[cp + 2] = (byte)cb;
                    }
                    while (count > 0);
                }
                else if (m_stride == 4)
                {
                    int cr = buffer[cp];
                    int cg = buffer[cp + 1];
                    int cb = buffer[cp + 2];
                    int ca = buffer[cp + 3];
                    do
                    {
                        count -= 4;
                        cp += 4;

                        cr += buffer[cp];
                        buffer[cp] = (byte)cr;

                        cg += buffer[cp + 1];
                        buffer[cp + 1] = (byte)cg;

                        cb += buffer[cp + 2];
                        buffer[cp + 2] = (byte)cb;

                        ca += buffer[cp + 3];
                        buffer[cp + 3] = (byte)ca;
                    }
                    while (count > 0);
                }
                else
                {
                    do
                    {
                        for (int i = m_stride; i > 0; i--)
                        {
                            buffer[cp + m_stride] = (byte)(buffer[cp + m_stride] + buffer[cp]);
                            cp++;
                        }

                        count -= m_stride;
                    }
                    while (count > 0);
                }
            }
        }

        private void horAcc16(byte[] buffer, int offset, int count)
        {
            short[] wBuffer = Tiff.ByteArrayToShorts(buffer, offset, count);
            int wOffset = 0;

            int wCount = count / 2;
            if (wCount > m_stride)
            {
                wCount -= m_stride;
                do
                {
                    for (int i = m_stride; i > 0; i--)
                    {
                        wBuffer[wOffset + m_stride] += wBuffer[wOffset];
                        wOffset++;
                    }

                    wCount -= m_stride;
                }
                while (wCount > 0);
            }

            Tiff.ShortsToByteArray(wBuffer, 0, count / 2, buffer, offset);
        }

        private void horAcc32(byte[] buffer, int offset, int count)
        {
            int[] wBuffer = Tiff.ByteArrayToInts(buffer, offset, count);
            int wOffset = 0;

            int wCount = count / 4;
            if (wCount > m_stride)
            {
                wCount -= m_stride;
                do
                {
                    for (int i = m_stride; i > 0; i--)
                    {
                        wBuffer[wOffset + m_stride] += wBuffer[wOffset];
                        wOffset++;
                    }

                    wCount -= m_stride;
                } while (wCount > 0);
            }

            Tiff.IntsToByteArray(wBuffer, 0, count / 4, buffer, offset);
        }

        private void swabHorAcc16(byte[] buffer, int offset, int count)
        {
            short[] wBuffer = Tiff.ByteArrayToShorts(buffer, offset, count);
            int wOffset= 0;
            
            int wCount = count / 2;
            if (wCount > m_stride)
            {
                Tiff.SwabArrayOfShort(wBuffer, wCount);
                wCount -= m_stride;
                do
                {
                    for (int i = m_stride; i > 0; i--)
                    {
                        wBuffer[wOffset + m_stride] += wBuffer[wOffset];
                        wOffset++;
                    }

                    wCount -= m_stride;
                }
                while (wCount > 0);
            }

            Tiff.ShortsToByteArray(wBuffer, 0, count / 2, buffer, offset);
        }
        
        private void swabHorAcc32(byte[] buffer, int offset, int count)
        {
            int[] wBuffer = Tiff.ByteArrayToInts(buffer, offset, count);
            int wOffset = 0;

            int wCount = count / 4;
            if (wCount > m_stride)
            {
                Tiff.SwabArrayOfLong(wBuffer, wCount);
                wCount -= m_stride;
                do
                {
                    for (int i = m_stride; i > 0; i--)
                    {
                        wBuffer[wOffset + m_stride] += wBuffer[wOffset];
                        wOffset++;
                    }

                    wCount -= m_stride;
                } while (wCount > 0);
            }

            Tiff.IntsToByteArray(wBuffer, 0, count / 4, buffer, offset);
        }

        private void horDiff8(byte[] buffer, int offset, int count)
        {
            if (count > m_stride)
            {
                count -= m_stride;
                int cp = offset;

                // Pipeline the most common cases.

                if (m_stride == 3)
                {
                    int r2 = buffer[cp];
                    int g2 = buffer[cp + 1];
                    int b2 = buffer[cp + 2];
                    do
                    {
                        int r1 = buffer[cp + 3];
                        buffer[cp + 3] = (byte)(r1 - r2);
                        r2 = r1;

                        int g1 = buffer[cp + 4];
                        buffer[cp + 4] = (byte)(g1 - g2);
                        g2 = g1;

                        int b1 = buffer[cp + 5];
                        buffer[cp + 5] = (byte)(b1 - b2);
                        b2 = b1;

                        cp += 3;
                    }
                    while ((count -= 3) > 0);
                }
                else if (m_stride == 4)
                {
                    int r2 = buffer[cp];
                    int g2 = buffer[cp + 1];
                    int b2 = buffer[cp + 2];
                    int a2 = buffer[cp + 3];
                    do
                    {
                        int r1 = buffer[cp + 4];
                        buffer[cp + 4] = (byte)(r1 - r2);
                        r2 = r1;

                        int g1 = buffer[cp + 5];
                        buffer[cp + 5] = (byte)(g1 - g2);
                        g2 = g1;

                        int b1 = buffer[cp + 6];
                        buffer[cp + 6] = (byte)(b1 - b2);
                        b2 = b1;

                        int a1 = buffer[cp + 7];
                        buffer[cp + 7] = (byte)(a1 - a2);
                        a2 = a1;

                        cp += 4;
                    }
                    while ((count -= 4) > 0);
                }
                else
                {
                    cp += count - 1;
                    do
                    {
                        for (int i = m_stride; i > 0; i--)
                        {
                            buffer[cp + m_stride] -= buffer[cp];
                            cp--;
                        }
                    }
                    while ((count -= m_stride) > 0);
                }
            }
        }

        private void horDiff16(byte[] buffer, int offset, int count)
        {
            short[] wBuffer = Tiff.ByteArrayToShorts(buffer, offset, count);
            int wOffset = 0;

            int wCount = count / 2;
            if (wCount > m_stride)
            {
                wCount -= m_stride;
                wOffset += wCount - 1;
                do
                {
                    for (int i = m_stride; i > 0; i--)
                    {
                        wBuffer[wOffset + m_stride] -= wBuffer[wOffset];
                        wOffset--;
                    }

                    wCount -= m_stride;
                }
                while (wCount > 0);
            }

            Tiff.ShortsToByteArray(wBuffer, 0, count / 2, buffer, offset);
        }

        private void horDiff32(byte[] buffer, int offset, int count)
        {
            int[] wBuffer = Tiff.ByteArrayToInts(buffer, offset, count);
            int wOffset = 0;

            int wCount = count / 4;
            if (wCount > m_stride)
            {
                wCount -= m_stride;
                wOffset += wCount - 1;
                do
                {
                    for (int i = m_stride; i > 0; i--)
                    {
                        wBuffer[wOffset + m_stride] -= wBuffer[wOffset];
                        wOffset--;
                    }

                    wCount -= m_stride;
                } while (wCount > 0);
            }

            Tiff.IntsToByteArray(wBuffer, 0, count / 4, buffer, offset);
        }
        
        /// <summary>
        /// Floating point predictor accumulation routine.
        /// </summary>
        private void fpAcc(byte[] buffer, int offset, int count)
        {
            int bps = m_tif.m_dir.td_bitspersample / 8;
            int wCount = count / bps;
            int left = count;
            int cp = offset;

            while (left > m_stride)
            {
                for (int i = m_stride; i > 0; i--)
                {
                    buffer[cp + m_stride] += buffer[cp];
                    cp++;
                }

                left -= m_stride;
            }

            byte[] tmp = new byte[count];
            Buffer.BlockCopy(buffer, offset, tmp, 0, count);
            for (int i = 0; i < wCount; i++)
            {
                for (int b = 0; b < bps; b++)
                    buffer[offset + bps * i + b] = tmp[(bps - b - 1) * wCount + i];
            }
        }

        /// <summary>
        /// Floating point predictor differencing routine.
        /// </summary>
        private void fpDiff(byte[] buffer, int offset, int count)
        {
            byte[] tmp = new byte [count];
            Buffer.BlockCopy(buffer, offset, tmp, 0, count);

            int bps = m_tif.m_dir.td_bitspersample / 8;
            int wCount = count / bps;
            for (int c = 0; c < wCount; c++)
            {
                for (int b = 0; b < bps; b++)
                    buffer[offset + (bps - b - 1) * wCount + c] = tmp[bps * c + b];
            }

            int cp = offset + count - m_stride - 1;
            for (int c = count; c > m_stride; c -= m_stride)
            {
                for (int i = m_stride; i > 0; i--)
                {
                    buffer[cp + m_stride] -= buffer[cp];
                    cp--;
                }
            }
        }
                
        /// <summary>
        /// Decode a scanline and apply the predictor routine.
        /// </summary>
        private bool PredictorDecodeRow(byte[] buffer, int offset, int count, short plane)
        {
            if (predictor_decoderow(buffer, offset, count, plane))
            {
                predictorFunc(buffer, offset, count);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Decode a tile/strip and apply the predictor routine. Note that horizontal differencing
        /// must be done on a row-by-row basis. The width of a "row" has already been calculated
        /// at pre-decode time according to the strip/tile dimensions.
        /// </summary>
        private bool PredictorDecodeTile(byte[] buffer, int offset, int count, short plane)
        {
            if (predictor_decodetile(buffer, offset, count, plane))
            {
                while (count > 0)
                {
                    predictorFunc(buffer, offset, m_rowSize);
                    count -= m_rowSize;
                    offset += m_rowSize;
                }

                return true;
            }

            return false;
        }

        private bool PredictorSetupDecode()
        {
            if (!predictor_setupdecode() || !PredictorSetup())
                return false;

            m_passThruDecode = true;
            if (m_predictor == Predictor.HORIZONTAL)
            {
                switch (m_tif.m_dir.td_bitspersample)
                {
                    case 8:
                        m_predictorType = PredictorType.ptHorAcc8;
                        break;
                    case 16:
                        m_predictorType = PredictorType.ptHorAcc16;
                        break;
                    case 32:
                        m_predictorType = PredictorType.ptHorAcc32;
                        break;
                }

                m_passThruDecode = false;
                
                if ((m_tif.m_flags & TiffFlags.SWAB) == TiffFlags.SWAB)
                {
                    if (m_predictorType == PredictorType.ptHorAcc16)
                    {
                        m_predictorType = PredictorType.ptSwabHorAcc16;
                        m_tif.m_postDecodeMethod = Tiff.PostDecodeMethodType.pdmNone;
                    }
                    else if (m_predictorType == PredictorType.ptHorAcc32)
                    {
                        m_predictorType = PredictorType.ptSwabHorAcc32;
                        m_tif.m_postDecodeMethod = Tiff.PostDecodeMethodType.pdmNone;
                    }
                }
            }
            else if (m_predictor == Predictor.FLOATINGPOINT)
            {
                m_predictorType = PredictorType.ptFpAcc;
                
                m_passThruDecode = false;
                
                if ((m_tif.m_flags & TiffFlags.SWAB) == TiffFlags.SWAB)
                    m_tif.m_postDecodeMethod = Tiff.PostDecodeMethodType.pdmNone;
            }

            return true;
        }

        private bool PredictorSetup()
        {
            const string module = "PredictorSetup";
            TiffDirectory td = m_tif.m_dir;

            switch (m_predictor)
            {
                case Predictor.NONE:
                    // no differencing
                    return true;

                case Predictor.HORIZONTAL:
                    if (td.td_bitspersample != 8 &&
                        td.td_bitspersample != 16 &&
                        td.td_bitspersample != 32)
                    {
                        return false;
                    }
                    break;

                case Predictor.FLOATINGPOINT:
                    if (td.td_sampleformat != SampleFormat.IEEEFP)
                    {
                        return false;
                    }
                    break;

                default:
                    return false;
            }

            m_stride = (td.td_planarconfig == PlanarConfig.CONTIG ? (int)td.td_samplesperpixel : 1);
            
            if (m_tif.IsTiled())
                m_rowSize = m_tif.TileRowSize();
            else
                m_rowSize = m_tif.ScanlineSize();

            return true;
        }
    }

    class CodecWithPredictorTagMethods : TiffTagMethods
    {
        public override bool SetField(Tiff tif, TiffTag tag, FieldValue[] ap)
        {
            CodecWithPredictor sp = tif.m_currentCodec as CodecWithPredictor;
            switch (tag)
            {
                case TiffTag.PREDICTOR:
                    sp.SetPredictorValue((Predictor)ap[0].ToByte());
                    tif.setFieldBit(CodecWithPredictor.FIELD_PREDICTOR);
                    tif.m_flags |= TiffFlags.DIRTYDIRECT;
                    return true;
            }

            TiffTagMethods childMethods = sp.GetChildTagMethods();
            if (childMethods != null)
                return childMethods.SetField(tif, tag, ap);

            return base.SetField(tif, tag, ap);
        }

        public override FieldValue[] GetField(Tiff tif, TiffTag tag)
        {
            CodecWithPredictor sp = tif.m_currentCodec as CodecWithPredictor;

            switch (tag)
            {
                case TiffTag.PREDICTOR:
                    FieldValue[] result = new FieldValue[1];
                    result[0].Set(sp.GetPredictorValue());
                    return result;
            }

            TiffTagMethods childMethods = sp.GetChildTagMethods();
            if (childMethods != null)
                return childMethods.GetField(tif, tag);

            return base.GetField(tif, tag);
        }
    }
}

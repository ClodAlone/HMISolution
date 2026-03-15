#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.wavelet.analysis;
using Syncfusion.Pdf.JPEG2000.wavelet;
using Syncfusion.Pdf.JPEG2000.image;
using Syncfusion.Pdf.JPEG2000.encoder;
namespace Syncfusion.Pdf.JPEG2000.quantization.quantizer
{
    internal class StdQuantizer : Quantizer
    {
        virtual public QuantTypeSpec QuantTypeSpec
        {
            get
            {
                return qts;
            }
        }
        public const int QSTEP_MANTISSA_BITS = 11;
        public const int QSTEP_EXPONENT_BITS = 5;
        public static readonly int QSTEP_MAX_MANTISSA = (1 << QSTEP_MANTISSA_BITS) - 1;
        public static readonly int QSTEP_MAX_EXPONENT = (1 << QSTEP_EXPONENT_BITS) - 1;
        private static double log2 = System.Math.Log(2);
        private QuantTypeSpec qts;
        private QuantStepSizeSpec qsss;
        private GuardBitsSpec gbs;
        private CBlkWTDataFloat infblk;
        internal StdQuantizer(CBlkWTDataSrc src, EncoderSpecs encSpec)
            : base(src)
        {
            qts = encSpec.qts;
            qsss = encSpec.qsss;
            gbs = encSpec.gbs;
        }
        public override int getNumGuardBits(int t, int c)
        {
            return ((System.Int32)gbs.getTileCompVal(t, c));
        }
        public override bool isReversible(int t, int c)
        {
            return qts.isReversible(t, c);
        }
        public override bool isDerived(int t, int c)
        {
            return qts.isDerived(t, c);
        }
        public override CBlkWTData getNextCodeBlock(int c, CBlkWTData cblk)
        {
            return getNextInternCodeBlock(c, cblk);
        }
        public override CBlkWTData getNextInternCodeBlock(int c, CBlkWTData cblk)
        {
            int k, j;
            int tmp, shiftBits, jmin;
            int w, h;
            int[] outarr;
            float[] infarr = null;
            CBlkWTDataFloat infblk;
            float invstep;
            bool intq;
            SubbandAn sb;
            float stepUDR;
            int g = ((System.Int32)gbs.getTileCompVal(tIdx, c));
            intq = (src.getDataType(tIdx, c) == DataBlock.TYPE_INT);
            if (cblk == null)
            {
                cblk = new CBlkWTDataInt();
            }
            infblk = this.infblk;
            if (intq)
            {
                cblk = src.getNextCodeBlock(c, cblk);
                if (cblk == null)
                {
                    return null;
                }
                outarr = (int[])cblk.Data;
            }
            else
            {
                infblk = (CBlkWTDataFloat)src.getNextInternCodeBlock(c, infblk);
                if (infblk == null)
                {
                    this.infblk.Data = null;
                    return null;
                }
                this.infblk = infblk;
                infarr = (float[])infblk.Data;
                outarr = (int[])cblk.Data;
                if (outarr == null || outarr.Length < infblk.w * infblk.h)
                {
                    outarr = new int[infblk.w * infblk.h];
                    cblk.Data = outarr;
                }
                cblk.m = infblk.m;
                cblk.n = infblk.n;
                cblk.sb = infblk.sb;
                cblk.ulx = infblk.ulx;
                cblk.uly = infblk.uly;
                cblk.w = infblk.w;
                cblk.h = infblk.h;
                cblk.wmseScaling = infblk.wmseScaling;
                cblk.offset = 0;
                cblk.scanw = cblk.w;
            }
            w = cblk.w;
            h = cblk.h;
            sb = cblk.sb;
            if (isReversible(tIdx, c))
            {
                cblk.magbits = g - 1 + src.getNomRangeBits(c) + sb.anGainExp;
                shiftBits = 31 - cblk.magbits;
                cblk.convertFactor = (1 << shiftBits);
                for (j = w * h - 1; j >= 0; j--)
                {
                    tmp = (outarr[j] << shiftBits);
                    outarr[j] = ((tmp < 0) ? (1 << 31) | (-tmp) : tmp);
                }
            }
            else
            {
                float baseStep = (float)((System.Single)qsss.getTileCompVal(tIdx, c));
                if (isDerived(tIdx, c))
                {
                    cblk.magbits = g - 1 + sb.level - (int)System.Math.Floor(System.Math.Log(baseStep) / log2);
                    stepUDR = baseStep / (1 << sb.level);
                }
                else
                {
                    cblk.magbits = g - 1 - (int)System.Math.Floor(System.Math.Log(baseStep / (sb.l2Norm * (1 << sb.anGainExp))) / log2);
                    stepUDR = baseStep / (sb.l2Norm * (1 << sb.anGainExp));
                }
                shiftBits = 31 - cblk.magbits;
                stepUDR = convertFromExpMantissa(convertToExpMantissa(stepUDR));
                invstep = 1.0f / ((1L << (src.getNomRangeBits(c) + sb.anGainExp)) * stepUDR);
                invstep *= (1 << (shiftBits - src.getFixedPoint(c)));
                cblk.convertFactor = invstep;
                cblk.stepSize = ((1L << (src.getNomRangeBits(c) + sb.anGainExp)) * stepUDR);
                if (intq)
                {
                    for (j = w * h - 1; j >= 0; j--)
                    {
                        tmp = (int)(outarr[j] * invstep);
                        outarr[j] = ((tmp < 0) ? (1 << 31) | (-tmp) : tmp);
                    }
                }
                else
                {
                    for (j = w * h - 1, k = infblk.offset + (h - 1) * infblk.scanw + w - 1, jmin = w * (h - 1); j >= 0; jmin -= w)
                    {
                        for (; j >= jmin; k--, j--)
                        {
                            tmp = (int)(infarr[k] * invstep);
                            outarr[j] = ((tmp < 0) ? (1 << 31) | (-tmp) : tmp);
                        }
                        k -= (infblk.scanw - w);
                    }
                }
            }
            return cblk;
        }
        internal override void calcSbParams(SubbandAn sb, int c)
        {
            float baseStep;
            if (sb.stepWMSE > 0f)
                return;
            if (!sb.isNode)
            {
                if (isReversible(tIdx, c))
                {
                    sb.stepWMSE = (float)System.Math.Pow(2, -(src.getNomRangeBits(c) << 1)) * sb.l2Norm * sb.l2Norm;
                }
                else
                {
                    baseStep = (float)((System.Single)qsss.getTileCompVal(tIdx, c));
                    if (isDerived(tIdx, c))
                    {
                        sb.stepWMSE = baseStep * baseStep * (float)System.Math.Pow(2, (sb.anGainExp - sb.level) << 1) * sb.l2Norm * sb.l2Norm;
                    }
                    else
                    {
                        sb.stepWMSE = baseStep * baseStep;
                    }
                }
            }
            else
            {
                calcSbParams((SubbandAn)sb.LL, c);
                calcSbParams((SubbandAn)sb.HL, c);
                calcSbParams((SubbandAn)sb.LH, c);
                calcSbParams((SubbandAn)sb.HH, c);
                sb.stepWMSE = 1f;
            }
        }
        public static int convertToExpMantissa(float step)
        {
            int exp;
            exp = (int)System.Math.Ceiling((-System.Math.Log(step)) / log2);
            if (exp > QSTEP_MAX_EXPONENT)
            {
                return (QSTEP_MAX_EXPONENT << QSTEP_MANTISSA_BITS);
            }
            return (exp << QSTEP_MANTISSA_BITS) | ((int)(((-step) * (-1 << exp) - 1f) * (1 << QSTEP_MANTISSA_BITS) + 0.5f));
        }
        private static float convertFromExpMantissa(int ems)
        {
            return (-1f - ((float)(ems & QSTEP_MAX_MANTISSA)) / ((float)(1 << QSTEP_MANTISSA_BITS))) / (float)(-1 << ((ems >> QSTEP_MANTISSA_BITS) & QSTEP_MAX_EXPONENT));
        }
        public override int getMaxMagBits(int c)
        {
            Subband sb = getAnSubbandTree(tIdx, c);
            if (isReversible(tIdx, c))
            {
                return getMaxMagBitsRev(sb, c);
            }
            else
            {
                if (isDerived(tIdx, c))
                {
                    return getMaxMagBitsDerived(sb, tIdx, c);
                }
                else
                {
                    return getMaxMagBitsExpounded(sb, tIdx, c);
                }
            }
        }
        private int getMaxMagBitsRev(Subband sb, int c)
        {
            int tmp, max = 0;
            int g = ((System.Int32)gbs.getTileCompVal(tIdx, c));
            if (!sb.isNode)
                return g - 1 + src.getNomRangeBits(c) + sb.anGainExp;
            max = getMaxMagBitsRev(sb.LL, c);
            tmp = getMaxMagBitsRev(sb.LH, c);
            if (tmp > max)
                max = tmp;
            tmp = getMaxMagBitsRev(sb.HL, c);
            if (tmp > max)
                max = tmp;
            tmp = getMaxMagBitsRev(sb.HH, c);
            if (tmp > max)
                max = tmp;
            return max;
        }
        private int getMaxMagBitsDerived(Subband sb, int t, int c)
        {
            int tmp, max = 0;
            int g = ((System.Int32)gbs.getTileCompVal(t, c));
            if (!sb.isNode)
            {
                float baseStep = (float)((System.Single)qsss.getTileCompVal(t, c));
                return g - 1 + sb.level - (int)System.Math.Floor(System.Math.Log(baseStep) / log2);
            }
            max = getMaxMagBitsDerived(sb.LL, t, c);
            tmp = getMaxMagBitsDerived(sb.LH, t, c);
            if (tmp > max)
                max = tmp;
            tmp = getMaxMagBitsDerived(sb.HL, t, c);
            if (tmp > max)
                max = tmp;
            tmp = getMaxMagBitsDerived(sb.HH, t, c);
            if (tmp > max)
                max = tmp;
            return max;
        }
        private int getMaxMagBitsExpounded(Subband sb, int t, int c)
        {
            int tmp, max = 0;
            int g = ((System.Int32)gbs.getTileCompVal(t, c));
            if (!sb.isNode)
            {
                float baseStep = (float)((System.Single)qsss.getTileCompVal(t, c));
                return g - 1 - (int)System.Math.Floor(System.Math.Log(baseStep / (((SubbandAn)sb).l2Norm * (1 << sb.anGainExp))) / log2);
            }
            max = getMaxMagBitsExpounded(sb.LL, t, c);
            tmp = getMaxMagBitsExpounded(sb.LH, t, c);
            if (tmp > max)
                max = tmp;
            tmp = getMaxMagBitsExpounded(sb.HL, t, c);
            if (tmp > max)
                max = tmp;
            tmp = getMaxMagBitsExpounded(sb.HH, t, c);
            if (tmp > max)
                max = tmp;
            return max;
        }
    }
}
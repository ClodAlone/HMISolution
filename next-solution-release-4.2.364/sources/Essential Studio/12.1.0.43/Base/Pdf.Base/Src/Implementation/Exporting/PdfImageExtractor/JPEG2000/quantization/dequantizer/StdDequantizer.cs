#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.wavelet.synthesis;
using Syncfusion.Pdf.JPEG2000.decoder;
using Syncfusion.Pdf.JPEG2000.image;
namespace Syncfusion.Pdf.JPEG2000.quantization.dequantizer
{
    internal class StdDequantizer : Dequantizer
    {
        private QuantTypeSpec qts;
        private QuantStepSizeSpec qsss;
        private GuardBitsSpec gbs;
        private DataBlockInt inblk;
        private int outdtype;
        internal StdDequantizer(CBlkQuantDataSrcDec src, int[] utrb, DecodeHelper decSpec)
            : base(src, utrb, decSpec)
        {
            if (utrb.Length != src.NumComps)
            {
                throw new System.ArgumentException("Invalid rb argument");
            }
            this.qsss = decSpec.qsss;
            this.qts = decSpec.qts;
            this.gbs = decSpec.gbs;
        }
        public override int getFixedPoint(int c)
        {
            return 0;
        }
        public override DataBlock getCodeBlock(int c, int m, int n, SubbandSyn sb, DataBlock cblk)
        {
            return getInternCodeBlock(c, m, n, sb, cblk);
        }
        public override DataBlock getInternCodeBlock(int c, int m, int n, SubbandSyn sb, DataBlock cblk)
        {
            int j, jmin, k;
            int temp;
            float step;
            int shiftBits;
            int magBits;
            int[] outiarr, inarr;
            float[] outfarr;
            int w, h;
            bool reversible = qts.isReversible(tIdx, c);
            bool derived = qts.isDerived(tIdx, c);
            StdDequantizerParams params_Renamed = (StdDequantizerParams)qsss.getTileCompVal(tIdx, c);
            int G = ((System.Int32)gbs.getTileCompVal(tIdx, c));
            outdtype = cblk.DataType;
            if (reversible && outdtype != DataBlock.TYPE_INT)
            {
                throw new System.ArgumentException("Reversible quantizations " + "must use int data");
            }
            outiarr = null;
            outfarr = null;
            inarr = null;
            switch (outdtype)
            {
                case DataBlock.TYPE_INT:
                    cblk = src.getCodeBlock(c, m, n, sb, cblk);
                    outiarr = (int[])cblk.Data;
                    break;
                case DataBlock.TYPE_FLOAT:
                    inblk = (DataBlockInt)src.getInternCodeBlock(c, m, n, sb, inblk);
                    inarr = inblk.DataInt;
                    if (cblk == null)
                    {
                        cblk = new DataBlockFloat();
                    }
                    cblk.ulx = inblk.ulx;
                    cblk.uly = inblk.uly;
                    cblk.w = inblk.w;
                    cblk.h = inblk.h;
                    cblk.offset = 0;
                    cblk.scanw = cblk.w;
                    cblk.progressive = inblk.progressive;
                    outfarr = (float[])cblk.Data;
                    if (outfarr == null || outfarr.Length < cblk.w * cblk.h)
                    {
                        outfarr = new float[cblk.w * cblk.h];
                        cblk.Data = outfarr;
                    }
                    break;
            }
            magBits = sb.magbits;
            if (reversible)
            {
                shiftBits = 31 - magBits;
                for (j = outiarr.Length - 1; j >= 0; j--)
                {
                    temp = outiarr[j];
                    outiarr[j] = (temp >= 0) ? (temp >> shiftBits) : -((temp & 0x7FFFFFFF) >> shiftBits);
                }
            }
            else
            {
                if (derived)
                {
                    int mrl = src.getSynSubbandTree(TileIdx, c).resLvl;
                    step = params_Renamed.nStep[0][0] * (1L << (rb[c] + sb.anGainExp + mrl - sb.level));
                }
                else
                {
                    step = params_Renamed.nStep[sb.resLvl][sb.sbandIdx] * (1L << (rb[c] + sb.anGainExp));
                }
                shiftBits = 31 - magBits;
                step /= (1 << shiftBits);
                switch (outdtype)
                {
                    case DataBlock.TYPE_INT:
                        for (j = outiarr.Length - 1; j >= 0; j--)
                        {
                            temp = outiarr[j];
                            outiarr[j] = (int)(((float)((temp >= 0) ? temp : -(temp & 0x7FFFFFFF))) * step);
                        }
                        break;
                    case DataBlock.TYPE_FLOAT:
                        w = cblk.w;
                        h = cblk.h;
                        for (j = w * h - 1, k = inblk.offset + (h - 1) * inblk.scanw + w - 1, jmin = w * (h - 1); j >= 0; jmin -= w)
                        {
                            for (; j >= jmin; k--, j--)
                            {
                                temp = inarr[k];
                                outfarr[j] = ((float)((temp >= 0) ? temp : -(temp & 0x7FFFFFFF))) * step;
                            }
                            k -= (inblk.scanw - w);
                        }
                        break;
                }
            }
            return cblk;
        }
    }
}
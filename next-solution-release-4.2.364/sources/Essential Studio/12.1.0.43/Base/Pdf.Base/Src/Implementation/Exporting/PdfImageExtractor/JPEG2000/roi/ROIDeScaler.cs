#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.quantization.dequantizer;
using Syncfusion.Pdf.JPEG2000.wavelet.synthesis;
using Syncfusion.Pdf.JPEG2000.decoder;
using Syncfusion.Pdf.JPEG2000.image;
using Syncfusion.Pdf.JPEG2000.util;
namespace Syncfusion.Pdf.JPEG2000.roi
{
    internal class DeScalerROI : MultiResImgDataAdapter, CBlkQuantDataSrcDec
    {
        virtual public int CbULX
        {
            get
            {
                return src.CbULX;
            }
        }
        virtual public int CbULY
        {
            get
            {
                return src.CbULY;
            }
        }
        public static System.String[][] ParameterInfo
        {
            get
            {
                return pinfo;
            }
        }
        private MaxShiftSpec mss;
        public const char OPT_PREFIX = 'R';
        private static readonly System.String[][] pinfo = new System.String[][] { new System.String[] { "Rno_roi", null, "This argument makes sure that the no ROI de-scaling is performed. " + "Decompression is done like there is no ROI in the image", null } };
        private CBlkQuantDataSrcDec src;
        internal DeScalerROI(CBlkQuantDataSrcDec src, MaxShiftSpec mss)
            : base(src)
        {
            this.src = src;
            this.mss = mss;
        }
        public override SubbandSyn getSynSubbandTree(int t, int c)
        {
            return src.getSynSubbandTree(t, c);
        }
        public virtual DataBlock getCodeBlock(int c, int m, int n, SubbandSyn sb, DataBlock cblk)
        {
            return getInternCodeBlock(c, m, n, sb, cblk);
        }
        public virtual DataBlock getInternCodeBlock(int c, int m, int n, SubbandSyn sb, DataBlock cblk)
        {
            int i, j, k, wrap;
            int ulx, uly, w, h;
            int[] data;
            int tmp;
            cblk = src.getInternCodeBlock(c, m, n, sb, cblk);
            bool noRoiInTile = false;
            if (mss == null || mss.getTileCompVal(TileIdx, c) == null)
                noRoiInTile = true;
            if (noRoiInTile || cblk == null)
            {
                return cblk;
            }
            data = (int[])cblk.Data;
            ulx = cblk.ulx;
            uly = cblk.uly;
            w = cblk.w;
            h = cblk.h;
            int boost = ((System.Int32)mss.getTileCompVal(TileIdx, c));
            int mask = ((1 << sb.magbits) - 1) << (31 - sb.magbits);
            int mask2 = (~mask) & 0x7FFFFFFF;
            wrap = cblk.scanw - w;
            i = cblk.offset + cblk.scanw * (h - 1) + w - 1;
            for (j = h; j > 0; j--)
            {
                for (k = w; k > 0; k--, i--)
                {
                    tmp = data[i];
                    if ((tmp & mask) == 0)
                    {
                        data[i] = (tmp & unchecked((int)0x80000000)) | (tmp << boost);
                    }
                    else
                    {
                        if ((tmp & mask2) != 0)
                        {
                            data[i] = (tmp & (~mask2)) | (1 << (30 - sb.magbits));
                        }
                    }
                }
                i -= wrap;
            }
            return cblk;
        }
        internal static DeScalerROI createInstance(CBlkQuantDataSrcDec src, JPXParameters pl, DecodeHelper decSpec)
        {
            System.String noRoi;
            pl.checkList(OPT_PREFIX, Syncfusion.Pdf.JPEG2000.util.JPXParameters.toNameArray(pinfo));
            noRoi = pl.getParameter("Rno_roi");
            if (noRoi != null || decSpec.rois == null)
            {
                return new DeScalerROI(src, null);
            }
            return new DeScalerROI(src, decSpec.rois);
        }
    }
}
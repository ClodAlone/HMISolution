#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Pdf.JPEG2000.entropy;
using Syncfusion.Pdf.JPEG2000.image;
using Syncfusion.Pdf.JPEG2000.util;
using Syncfusion.Pdf.JPEG2000.encoder;
namespace Syncfusion.Pdf.JPEG2000.wavelet.analysis
{
    internal class ForwWTFull : ForwardWT
    {
        override public int CbULX
        {
            get
            {
                return cb0x;
            }
        }
        override public int CbULY
        {
            get
            {
                return cb0y;
            }
        }
        private bool intData;
        private SubbandAn[][] subbTrees;
        private BlockImageDataSource src;
        private int cb0x;
        private int cb0y;
        private IntegerSpec dls;
        private AnWTFilterSpec filters;
        private CBlkSizeSpec cblks;
        private PrecinctSizeSpec pss;
        private DataBlock[] decomposedComps;
        private int[] lastn;
        private int[] lastm;
        internal SubbandAn[] currentSubband;
        internal JPXImageCoordinates ncblks;
        internal ForwWTFull(BlockImageDataSource src, EncoderSpecs encSpec, int cb0x, int cb0y)
            : base(src)
        {
            this.src = src;
            this.cb0x = cb0x;
            this.cb0y = cb0y;
            this.dls = encSpec.dls;
            this.filters = encSpec.wfs;
            this.cblks = encSpec.cblks;
            this.pss = encSpec.pss;
            int ncomp = src.NumComps;
            int ntiles = src.getNumTiles();
            currentSubband = new SubbandAn[ncomp];
            decomposedComps = new DataBlock[ncomp];
            subbTrees = new SubbandAn[ntiles][];
            for (int i = 0; i < ntiles; i++)
            {
                subbTrees[i] = new SubbandAn[ncomp];
            }
            lastn = new int[ncomp];
            lastm = new int[ncomp];
        }
        public override int getImplementationType(int c)
        {
            return Syncfusion.Pdf.JPEG2000.wavelet.WaveletTransform_Fields.WT_IMPL_FULL;
        }
        public override int getDecompLevels(int t, int c)
        {
            return ((System.Int32)dls.getTileCompVal(t, c));
        }
        public override int getDecomp(int t, int c)
        {
            return WT_DECOMP_DYADIC;
        }
        public override AnWTFilter[] getHorAnWaveletFilters(int t, int c)
        {
            return filters.getHFilters(t, c);
        }
        public override AnWTFilter[] getVertAnWaveletFilters(int t, int c)
        {
            return filters.getVFilters(t, c);
        }
        public override bool isReversible(int t, int c)
        {
            return filters.isReversible(t, c);
        }
        public override int getFixedPoint(int c)
        {
            return src.getFixedPoint(c);
        }
        public override CBlkWTData getNextInternCodeBlock(int c, CBlkWTData cblk)
        {
            int cbm, cbn, cn, cm;
            int acb0x, acb0y;
            SubbandAn sb;
            intData = (filters.getWTDataType(tIdx, c) == DataBlock.TYPE_INT);
            if (decomposedComps[c] == null)
            {
                int k, w, h;
                DataBlock bufblk;
                System.Object dst_data;
                w = getTileComponentWidth(tIdx, c);
                h = getTileComponentHeight(tIdx, c);
                if (intData)
                {
                    decomposedComps[c] = new DataBlockInt(0, 0, w, h);
                    bufblk = new DataBlockInt();
                }
                else
                {
                    decomposedComps[c] = new DataBlockFloat(0, 0, w, h);
                    bufblk = new DataBlockFloat();
                }
                dst_data = decomposedComps[c].Data;
                int lstart = getCompUpperLeftCornerX(c);
                bufblk.ulx = lstart;
                bufblk.w = w;
                bufblk.h = 1;
                int kk = getCompUpperLeftCornerY(c);
                for (k = 0; k < h; k++, kk++)
                {
                    bufblk.uly = kk;
                    bufblk.ulx = lstart;
                    bufblk = src.getInternCompData(bufblk, c);
                    Array.Copy((System.Array)bufblk.Data, bufblk.offset, (System.Array)dst_data, k * w, w);
                }
                waveletTreeDecomposition(decomposedComps[c], getAnSubbandTree(tIdx, c), c);
                currentSubband[c] = getNextSubband(c);
                lastn[c] = -1;
                lastm[c] = 0;
            }
            do
            {
                ncblks = currentSubband[c].numCb;
                lastn[c]++;
                if (lastn[c] == ncblks.x)
                {
                    lastn[c] = 0;
                    lastm[c]++;
                }
                if (lastm[c] < ncblks.y)
                {
                    break;
                }
                currentSubband[c] = getNextSubband(c);
                lastn[c] = -1;
                lastm[c] = 0;
                if (currentSubband[c] == null)
                {
                    decomposedComps[c] = null;
                    return null;
                }
            }
            while (true);
            acb0x = cb0x;
            acb0y = cb0y;
            switch (currentSubband[c].sbandIdx)
            {
                case Subband.WT_ORIENT_LL:
                    break;
                case Subband.WT_ORIENT_HL:
                    acb0x = 0;
                    break;
                case Subband.WT_ORIENT_LH:
                    acb0y = 0;
                    break;
                case Subband.WT_ORIENT_HH:
                    acb0x = 0;
                    acb0y = 0;
                    break;
                //default:
                    //throw new System.ApplicationException("Internal JJ2000 error");
            }
            if (cblk == null)
            {
                if (intData)
                {
                    cblk = new CBlkWTDataInt();
                }
                else
                {
                    cblk = new CBlkWTDataFloat();
                }
            }
            cbn = lastn[c];
            cbm = lastm[c];
            sb = currentSubband[c];
            cblk.n = cbn;
            cblk.m = cbm;
            cblk.sb = sb;
            cn = (sb.ulcx - acb0x + sb.nomCBlkW) / sb.nomCBlkW - 1;
            cm = (sb.ulcy - acb0y + sb.nomCBlkH) / sb.nomCBlkH - 1;
            if (cbn == 0)
            {
                cblk.ulx = sb.ulx;
            }
            else
            {
                cblk.ulx = (cn + cbn) * sb.nomCBlkW - (sb.ulcx - acb0x) + sb.ulx;
            }
            if (cbm == 0)
            {
                cblk.uly = sb.uly;
            }
            else
            {
                cblk.uly = (cm + cbm) * sb.nomCBlkH - (sb.ulcy - acb0y) + sb.uly;
            }
            if (cbn < ncblks.x - 1)
            {
                cblk.w = (cn + cbn + 1) * sb.nomCBlkW - (sb.ulcx - acb0x) + sb.ulx - cblk.ulx;
            }
            else
            {
                cblk.w = sb.ulx + sb.w - cblk.ulx;
            }
            if (cbm < ncblks.y - 1)
            {
                cblk.h = (cm + cbm + 1) * sb.nomCBlkH - (sb.ulcy - acb0y) + sb.uly - cblk.uly;
            }
            else
            {
                cblk.h = sb.uly + sb.h - cblk.uly;
            }
            cblk.wmseScaling = 1f;
            cblk.offset = cblk.uly * decomposedComps[c].w + cblk.ulx;
            cblk.scanw = decomposedComps[c].w;
            cblk.Data = decomposedComps[c].Data;
            return cblk;
        }
        public override CBlkWTData getNextCodeBlock(int c, CBlkWTData cblk)
        {
            int j, k;
            int w;
            System.Object dst_data;
            int[] dst_data_int;
            float[] dst_data_float;
            System.Object src_data;
            intData = (filters.getWTDataType(tIdx, c) == DataBlock.TYPE_INT);
            dst_data = null;
            if (cblk != null)
            {
                dst_data = cblk.Data;
            }
            cblk = getNextInternCodeBlock(c, cblk);
            if (cblk == null)
            {
                return null;
            }
            if (intData)
            {
                dst_data_int = (int[])dst_data;
                if (dst_data_int == null || dst_data_int.Length < cblk.w * cblk.h)
                {
                    dst_data = new int[cblk.w * cblk.h];
                }
            }
            else
            {
                dst_data_float = (float[])dst_data;
                if (dst_data_float == null || dst_data_float.Length < cblk.w * cblk.h)
                {
                    dst_data = new float[cblk.w * cblk.h];
                }
            }
            src_data = cblk.Data;
            w = cblk.w;
            for (j = w * (cblk.h - 1), k = cblk.offset + (cblk.h - 1) * cblk.scanw; j >= 0; j -= w, k -= cblk.scanw)
            {
                Array.Copy((System.Array)src_data, k, (System.Array)dst_data, j, w);
            }
            cblk.Data = dst_data;
            cblk.offset = 0;
            cblk.scanw = w;
            return cblk;
        }
        public override int getDataType(int t, int c)
        {
            return filters.getWTDataType(t, c);
        }
        private SubbandAn getNextSubband(int c)
        {
            int down = 1;
            int up = 0;
            int direction = down;
            SubbandAn nextsb;
            nextsb = currentSubband[c];
            if (nextsb == null)
            {
                nextsb = getAnSubbandTree(tIdx, c);
                if (!nextsb.isNode)
                {
                    return nextsb;
                }
            }
            do
            {
                if (nextsb == null)
                {
                    break;
                }
                else if (!nextsb.isNode)
                {
                    switch (nextsb.orientation)
                    {
                        case Subband.WT_ORIENT_HH:
                            nextsb = (SubbandAn)nextsb.Parent.LH;
                            direction = down;
                            break;
                        case Subband.WT_ORIENT_LH:
                            nextsb = (SubbandAn)nextsb.Parent.HL;
                            direction = down;
                            break;
                        case Subband.WT_ORIENT_HL:
                            nextsb = (SubbandAn)nextsb.Parent.LL;
                            direction = down;
                            break;
                        case Subband.WT_ORIENT_LL:
                            nextsb = (SubbandAn)nextsb.Parent;
                            direction = up;
                            break;
                    }
                }
                else if (nextsb.isNode)
                {
                    if (direction == down)
                    {
                        nextsb = (SubbandAn)nextsb.HH;
                    }
                    else if (direction == up)
                    {
                        switch (nextsb.orientation)
                        {
                            case Subband.WT_ORIENT_HH:
                                nextsb = (SubbandAn)nextsb.Parent.LH;
                                direction = down;
                                break;
                            case Subband.WT_ORIENT_LH:
                                nextsb = (SubbandAn)nextsb.Parent.HL;
                                direction = down;
                                break;
                            case Subband.WT_ORIENT_HL:
                                nextsb = (SubbandAn)nextsb.Parent.LL;
                                direction = down;
                                break;
                            case Subband.WT_ORIENT_LL:
                                nextsb = (SubbandAn)nextsb.Parent;
                                direction = up;
                                break;
                        }
                    }
                }
                if (nextsb == null)
                {
                    break;
                }
            }
            while (nextsb.isNode);
            return nextsb;
        }
        private void waveletTreeDecomposition(DataBlock band, SubbandAn subband, int c)
        {
            if (!subband.isNode)
            {
                return;
            }
            else
            {
                wavelet2DDecomposition(band, (SubbandAn)subband, c);
                waveletTreeDecomposition(band, (SubbandAn)subband.HH, c);
                waveletTreeDecomposition(band, (SubbandAn)subband.LH, c);
                waveletTreeDecomposition(band, (SubbandAn)subband.HL, c);
                waveletTreeDecomposition(band, (SubbandAn)subband.LL, c);
            }
        }
        private void wavelet2DDecomposition(DataBlock band, SubbandAn subband, int c)
        {
            int ulx, uly, w, h;
            int band_w, band_h;
            if (subband.w == 0 || subband.h == 0)
            {
                return;
            }
            ulx = subband.ulx;
            uly = subband.uly;
            w = subband.w;
            h = subband.h;
            band_w = getTileComponentWidth(tIdx, c);
            band_h = getTileComponentHeight(tIdx, c);
            if (intData)
            {
                int i, j;
                int offset;
                int[] tmpVector = new int[System.Math.Max(w, h)];
                int[] data = ((DataBlockInt)band).DataInt;
                if (subband.ulcy % 2 == 0)
                {
                    for (j = 0; j < w; j++)
                    {
                        offset = uly * band_w + ulx + j;
                        for (i = 0; i < h; i++)
                            tmpVector[i] = data[offset + (i * band_w)];
                        subband.vFilter.analyze_lpf(tmpVector, 0, h, 1, data, offset, band_w, data, offset + ((h + 1) / 2) * band_w, band_w);
                    }
                }
                else
                {
                    for (j = 0; j < w; j++)
                    {
                        offset = uly * band_w + ulx + j;
                        for (i = 0; i < h; i++)
                            tmpVector[i] = data[offset + (i * band_w)];
                        subband.vFilter.analyze_hpf(tmpVector, 0, h, 1, data, offset, band_w, data, offset + (h / 2) * band_w, band_w);
                    }
                }
                if (subband.ulcx % 2 == 0)
                {
                    for (i = 0; i < h; i++)
                    {
                        offset = (uly + i) * band_w + ulx;
                        for (j = 0; j < w; j++)
                            tmpVector[j] = data[offset + j];
                        subband.hFilter.analyze_lpf(tmpVector, 0, w, 1, data, offset, 1, data, offset + (w + 1) / 2, 1);
                    }
                }
                else
                {
                    for (i = 0; i < h; i++)
                    {
                        offset = (uly + i) * band_w + ulx;
                        for (j = 0; j < w; j++)
                            tmpVector[j] = data[offset + j];
                        subband.hFilter.analyze_hpf(tmpVector, 0, w, 1, data, offset, 1, data, offset + w / 2, 1);
                    }
                }
            }
            else
            {
                int i, j;
                int offset;
                float[] tmpVector = new float[System.Math.Max(w, h)];
                float[] data = ((DataBlockFloat)band).DataFloat;
                if (subband.ulcy % 2 == 0)
                {
                    for (j = 0; j < w; j++)
                    {
                        offset = uly * band_w + ulx + j;
                        for (i = 0; i < h; i++)
                            tmpVector[i] = data[offset + (i * band_w)];
                        subband.vFilter.analyze_lpf(tmpVector, 0, h, 1, data, offset, band_w, data, offset + ((h + 1) / 2) * band_w, band_w);
                    }
                }
                else
                {
                    for (j = 0; j < w; j++)
                    {
                        offset = uly * band_w + ulx + j;
                        for (i = 0; i < h; i++)
                            tmpVector[i] = data[offset + (i * band_w)];
                        subband.vFilter.analyze_hpf(tmpVector, 0, h, 1, data, offset, band_w, data, offset + (h / 2) * band_w, band_w);
                    }
                }
                if (subband.ulcx % 2 == 0)
                {
                    for (i = 0; i < h; i++)
                    {
                        offset = (uly + i) * band_w + ulx;
                        for (j = 0; j < w; j++)
                            tmpVector[j] = data[offset + j];
                        subband.hFilter.analyze_lpf(tmpVector, 0, w, 1, data, offset, 1, data, offset + (w + 1) / 2, 1);
                    }
                }
                else
                {
                    for (i = 0; i < h; i++)
                    {
                        offset = (uly + i) * band_w + ulx;
                        for (j = 0; j < w; j++)
                            tmpVector[j] = data[offset + j];
                        subband.hFilter.analyze_hpf(tmpVector, 0, w, 1, data, offset, 1, data, offset + w / 2, 1);
                    }
                }
            }
        }
        public override void setTile(int x, int y)
        {
            int i;
            base.setTile(x, y);
            if (decomposedComps != null)
            {
                for (i = decomposedComps.Length - 1; i >= 0; i--)
                {
                    decomposedComps[i] = null;
                    currentSubband[i] = null;
                }
            }
        }
        public override void nextTile()
        {
            int i;
            base.nextTile();
            if (decomposedComps != null)
            {
                for (i = decomposedComps.Length - 1; i >= 0; i--)
                {
                    decomposedComps[i] = null;
                    currentSubband[i] = null;
                }
            }
        }
        public override SubbandAn getAnSubbandTree(int t, int c)
        {
            if (subbTrees[t][c] == null)
            {
                subbTrees[t][c] = new SubbandAn(getTileComponentWidth(t, c), getTileComponentHeight(t, c), getCompUpperLeftCornerX(c), getCompUpperLeftCornerY(c), getDecompLevels(t, c), getHorAnWaveletFilters(t, c), getVertAnWaveletFilters(t, c));
                initSubbandsFields(t, c, subbTrees[t][c]);
            }
            return subbTrees[t][c];
        }
        private void initSubbandsFields(int t, int c, Subband sb)
        {
            int cbw = cblks.getCBlkWidth(ModuleSpec.SPEC_TILE_COMP, t, c);
            int cbh = cblks.getCBlkHeight(ModuleSpec.SPEC_TILE_COMP, t, c);
            if (!sb.isNode)
            {
                int ppx, ppy;
                int ppxExp, ppyExp, cbwExp, cbhExp;
                ppx = pss.getPPX(t, c, sb.resLvl);
                ppy = pss.getPPY(t, c, sb.resLvl);
                if (ppx != Syncfusion.Pdf.JPEG2000.codestream.Markers.PRECINCT_PARTITION_DEF_SIZE || ppy != Syncfusion.Pdf.JPEG2000.codestream.Markers.PRECINCT_PARTITION_DEF_SIZE)
                {
                    ppxExp = MathUtil.log2(ppx);
                    ppyExp = MathUtil.log2(ppy);
                    cbwExp = MathUtil.log2(cbw);
                    cbhExp = MathUtil.log2(cbh);
                    switch (sb.resLvl)
                    {
                        case 0:
                            sb.nomCBlkW = (cbwExp < ppxExp ? (1 << cbwExp) : (1 << ppxExp));
                            sb.nomCBlkH = (cbhExp < ppyExp ? (1 << cbhExp) : (1 << ppyExp));
                            break;
                        default:
                            sb.nomCBlkW = (cbwExp < ppxExp - 1 ? (1 << cbwExp) : (1 << (ppxExp - 1)));
                            sb.nomCBlkH = (cbhExp < ppyExp - 1 ? (1 << cbhExp) : (1 << (ppyExp - 1)));
                            break;
                    }
                }
                else
                {
                    sb.nomCBlkW = cbw;
                    sb.nomCBlkH = cbh;
                }
                if (sb.numCb == null)
                    sb.numCb = new JPXImageCoordinates();
                if (sb.w != 0 && sb.h != 0)
                {
                    int acb0x = cb0x;
                    int acb0y = cb0y;
                    int tmp;
                    switch (sb.sbandIdx)
                    {
                        case Subband.WT_ORIENT_LL:
                            break;
                        case Subband.WT_ORIENT_HL:
                            acb0x = 0;
                            break;
                        case Subband.WT_ORIENT_LH:
                            acb0y = 0;
                            break;
                        case Subband.WT_ORIENT_HH:
                            acb0x = 0;
                            acb0y = 0;
                            break;
                        //default:
                            //throw new System.ApplicationException("Internal JJ2000 error");
                    }
                    if (sb.ulcx - acb0x < 0 || sb.ulcy - acb0y < 0)
                    {
                        throw new System.ArgumentException("Invalid code-blocks " + "partition origin or " + "image offset in the " + "reference grid.");
                    }
                    tmp = sb.ulcx - acb0x + sb.nomCBlkW;
                    sb.numCb.x = (tmp + sb.w - 1) / sb.nomCBlkW - (tmp / sb.nomCBlkW - 1);
                    tmp = sb.ulcy - acb0y + sb.nomCBlkH;
                    sb.numCb.y = (tmp + sb.h - 1) / sb.nomCBlkH - (tmp / sb.nomCBlkH - 1);
                }
                else
                {
                    sb.numCb.x = sb.numCb.y = 0;
                }
            }
            else
            {
                initSubbandsFields(t, c, sb.LL);
                initSubbandsFields(t, c, sb.HL);
                initSubbandsFields(t, c, sb.LH);
                initSubbandsFields(t, c, sb.HH);
            }
        }
    }
}
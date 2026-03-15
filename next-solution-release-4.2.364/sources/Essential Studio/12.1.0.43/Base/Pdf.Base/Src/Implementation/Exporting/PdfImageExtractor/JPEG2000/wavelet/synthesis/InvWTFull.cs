#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using Syncfusion.Pdf.JPEG2000.decoder;
using Syncfusion.Pdf.JPEG2000.image;
namespace Syncfusion.Pdf.JPEG2000.wavelet.synthesis
{
    internal class InvWTFull : WaveletTransformInverse
    {
        private int cblkToDecode = 0;
        private CBlkWTDataSrcDec src;
        private int dtype;
        private DataBlock[] reconstructedComps;
        private int[] ndl;
        private Dictionary<int, bool[]> reversible = new Dictionary<int, bool[]>();
        internal InvWTFull(CBlkWTDataSrcDec src, DecodeHelper decSpec)
            : base(src, decSpec)
        {
            this.src = src;
            int nc = src.NumComps;
            reconstructedComps = new DataBlock[nc];
            ndl = new int[nc];
        }
        private bool isSubbandReversible(Subband subband)
        {
            if (subband.isNode)
            {
                return isSubbandReversible(subband.LL) && isSubbandReversible(subband.HL) && isSubbandReversible(subband.LH) && isSubbandReversible(subband.HH) && ((SubbandSyn)subband).hFilter.Reversible && ((SubbandSyn)subband).vFilter.Reversible;
            }
            else
            {
                return true;
            }
        }
        public override bool isReversible(int t, int c)
        {
            if (reversible[t] == null)
            {               
                reversible[t] = new bool[NumComps];
                for (int i = reversible[t].Length - 1; i >= 0; i--)
                {
                    reversible[t][i] = isSubbandReversible(src.getSynSubbandTree(t, i));
                }
            }
            return reversible[t][c];
        }
        public override int getNomRangeBits(int c)
        {
            return src.getNomRangeBits(c);
        }
        public override int getFixedPoint(int c)
        {
            return src.getFixedPoint(c);
        }
        public override DataBlock getInternCompData(DataBlock blk, int c)
        {
            int tIdx = TileIdx;
            if (src.getSynSubbandTree(tIdx, c).HorWFilter == null)
            {
                dtype = DataBlock.TYPE_INT;
            }
            else
            {
                dtype = src.getSynSubbandTree(tIdx, c).HorWFilter.DataType;
            }
            if (reconstructedComps[c] == null)
            {
                switch (dtype)
                {
                    case DataBlock.TYPE_FLOAT:
                        reconstructedComps[c] = new DataBlockFloat(0, 0, getTileComponentWidth(tIdx, c), getTileComponentHeight(tIdx, c));
                        break;
                    case DataBlock.TYPE_INT:
                        reconstructedComps[c] = new DataBlockInt(0, 0, getTileComponentWidth(tIdx, c), getTileComponentHeight(tIdx, c));
                        break;
                }
                waveletTreeReconstruction(reconstructedComps[c], src.getSynSubbandTree(tIdx, c), c);
            }
            if (blk.DataType != dtype)
            {
                if (dtype == DataBlock.TYPE_INT)
                {
                    blk = new DataBlockInt(blk.ulx, blk.uly, blk.w, blk.h);
                }
                else
                {
                    blk = new DataBlockFloat(blk.ulx, blk.uly, blk.w, blk.h);
                }
            }
            blk.Data = reconstructedComps[c].Data;
            blk.offset = reconstructedComps[c].w * blk.uly + blk.ulx;
            blk.scanw = reconstructedComps[c].w;
            blk.progressive = false;
            return blk;
        }
        public override DataBlock getCompData(DataBlock blk, int c)
        {
            System.Object dst_data;
            int[] dst_data_int;
            float[] dst_data_float;
            dst_data = null;
            switch (blk.DataType)
            {
                case DataBlock.TYPE_INT:
                    dst_data_int = (int[])blk.Data;
                    if (dst_data_int == null || dst_data_int.Length < blk.w * blk.h)
                    {
                        dst_data_int = new int[blk.w * blk.h];
                    }
                    dst_data = dst_data_int;
                    break;
                case DataBlock.TYPE_FLOAT:
                    dst_data_float = (float[])blk.Data;
                    if (dst_data_float == null || dst_data_float.Length < blk.w * blk.h)
                    {
                        dst_data_float = new float[blk.w * blk.h];
                    }
                    dst_data = dst_data_float;
                    break;
            }
            blk = getInternCompData(blk, c);
            blk.Data = dst_data;
            blk.offset = 0;
            blk.scanw = blk.w;
            return blk;
        }
        private void wavelet2DReconstruction(DataBlock db, SubbandSyn sb, int c)
        {
            System.Object data;
            System.Object buf;
            int ulx, uly, w, h;
            int i, j, k;
            int offset;
            if (sb.w == 0 || sb.h == 0)
            {
                return;
            }
            data = db.Data;
            ulx = sb.ulx;
            uly = sb.uly;
            w = sb.w;
            h = sb.h;
            buf = null;
            switch (sb.HorWFilter.DataType)
            {
                case DataBlock.TYPE_INT:
                    buf = new int[(w >= h) ? w : h];
                    break;
                case DataBlock.TYPE_FLOAT:
                    buf = new float[(w >= h) ? w : h];
                    break;
            }
            offset = (uly - db.uly) * db.w + ulx - db.ulx;
            if (sb.ulcx % 2 == 0)
            {
                for (i = 0; i < h; i++, offset += db.w)
                {
                    Array.Copy((System.Array)data, offset, (System.Array)buf, 0, w);
                    sb.hFilter.synthetize_lpf(buf, 0, (w + 1) / 2, 1, buf, (w + 1) / 2, w / 2, 1, data, offset, 1);
                }
            }
            else
            {
                for (i = 0; i < h; i++, offset += db.w)
                {
                    Array.Copy((System.Array)data, offset, (System.Array)buf, 0, w);
                    sb.hFilter.synthetize_hpf(buf, 0, w / 2, 1, buf, w / 2, (w + 1) / 2, 1, data, offset, 1);
                }
            }
            offset = (uly - db.uly) * db.w + ulx - db.ulx;
            switch (sb.VerWFilter.DataType)
            {
                case DataBlock.TYPE_INT:
                    int[] data_int, buf_int;
                    data_int = (int[])data;
                    buf_int = (int[])buf;
                    if (sb.ulcy % 2 == 0)
                    {
                        for (j = 0; j < w; j++, offset++)
                        {
                            for (i = h - 1, k = offset + i * db.w; i >= 0; i--, k -= db.w)
                                buf_int[i] = data_int[k];
                            sb.vFilter.synthetize_lpf(buf, 0, (h + 1) / 2, 1, buf, (h + 1) / 2, h / 2, 1, data, offset, db.w);
                        }
                    }
                    else
                    {
                        for (j = 0; j < w; j++, offset++)
                        {
                            for (i = h - 1, k = offset + i * db.w; i >= 0; i--, k -= db.w)
                                buf_int[i] = data_int[k];
                            sb.vFilter.synthetize_hpf(buf, 0, h / 2, 1, buf, h / 2, (h + 1) / 2, 1, data, offset, db.w);
                        }
                    }
                    break;
                case DataBlock.TYPE_FLOAT:
                    float[] data_float, buf_float;
                    data_float = (float[])data;
                    buf_float = (float[])buf;
                    if (sb.ulcy % 2 == 0)
                    {
                        for (j = 0; j < w; j++, offset++)
                        {
                            for (i = h - 1, k = offset + i * db.w; i >= 0; i--, k -= db.w)
                                buf_float[i] = data_float[k];
                            sb.vFilter.synthetize_lpf(buf, 0, (h + 1) / 2, 1, buf, (h + 1) / 2, h / 2, 1, data, offset, db.w);
                        }
                    }
                    else
                    {
                        for (j = 0; j < w; j++, offset++)
                        {
                            for (i = h - 1, k = offset + i * db.w; i >= 0; i--, k -= db.w)
                                buf_float[i] = data_float[k];
                            sb.vFilter.synthetize_hpf(buf, 0, h / 2, 1, buf, h / 2, (h + 1) / 2, 1, data, offset, db.w);
                        }
                    }
                    break;
            }
        }
        private void waveletTreeReconstruction(DataBlock img, SubbandSyn sb, int c)
        {
            DataBlock subbData;
            if (!sb.isNode)
            {
                int i, m, n;
                System.Object src_data, dst_data;
                JPXImageCoordinates ncblks;
                if (sb.w == 0 || sb.h == 0)
                {
                    return;
                }
                if (dtype == DataBlock.TYPE_INT)
                {
                    subbData = new DataBlockInt();
                }
                else
                {
                    subbData = new DataBlockFloat();
                }
                ncblks = sb.numCb;
                dst_data = img.Data;
                for (m = 0; m < ncblks.y; m++)
                {
                    for (n = 0; n < ncblks.x; n++)
                    {
                        subbData = src.getInternCodeBlock(c, m, n, sb, subbData);
                        src_data = subbData.Data;
                        for (i = subbData.h - 1; i >= 0; i--)
                        {
                            Array.Copy((System.Array)src_data, subbData.offset + i * subbData.scanw, (System.Array)dst_data, (subbData.uly + i) * img.w + subbData.ulx, subbData.w);
                        }
                    }
                }
            }
            else if (sb.isNode)
            {
                waveletTreeReconstruction(img, (SubbandSyn)sb.LL, c);
                if (sb.resLvl <= reslvl - maxImgRes + ndl[c])
                {
                    waveletTreeReconstruction(img, (SubbandSyn)sb.HL, c);
                    waveletTreeReconstruction(img, (SubbandSyn)sb.LH, c);
                    waveletTreeReconstruction(img, (SubbandSyn)sb.HH, c);
                    wavelet2DReconstruction(img, (SubbandSyn)sb, c);
                }
            }
        }
        public override int getImplementationType(int c)
        {
            return Syncfusion.Pdf.JPEG2000.wavelet.WaveletTransform_Fields.WT_IMPL_FULL;
        }
        public override void setTile(int x, int y)
        {
            int i;
            base.setTile(x, y);
            int nc = src.NumComps;
            int tIdx = src.TileIdx;
            for (int c = 0; c < nc; c++)
            {
                ndl[c] = src.getSynSubbandTree(tIdx, c).resLvl;
            }
            if (reconstructedComps != null)
            {
                for (i = reconstructedComps.Length - 1; i >= 0; i--)
                {
                    reconstructedComps[i] = null;
                }
            }
            cblkToDecode = 0;
            SubbandSyn root, sb;
            for (int c = 0; c < nc; c++)
            {
                root = src.getSynSubbandTree(tIdx, c);
                for (int r = 0; r <= reslvl - maxImgRes + root.resLvl; r++)
                {
                    if (r == 0)
                    {
                        sb = (SubbandSyn)root.getSubbandByIdx(0, 0);
                        if (sb != null)
                            cblkToDecode += sb.numCb.x * sb.numCb.y;
                    }
                    else
                    {
                        sb = (SubbandSyn)root.getSubbandByIdx(r, 1);
                        if (sb != null)
                            cblkToDecode += sb.numCb.x * sb.numCb.y;
                        sb = (SubbandSyn)root.getSubbandByIdx(r, 2);
                        if (sb != null)
                            cblkToDecode += sb.numCb.x * sb.numCb.y;
                        sb = (SubbandSyn)root.getSubbandByIdx(r, 3);
                        if (sb != null)
                            cblkToDecode += sb.numCb.x * sb.numCb.y;
                    }
                }
            }
        }
        public override void nextTile()
        {
            int i;
            base.nextTile();
            int nc = src.NumComps;
            int tIdx = src.TileIdx;
            for (int c = 0; c < nc; c++)
            {
                ndl[c] = src.getSynSubbandTree(tIdx, c).resLvl;
            }
            if (reconstructedComps != null)
            {
                for (i = reconstructedComps.Length - 1; i >= 0; i--)
                {
                    reconstructedComps[i] = null;
                }
            }
        }
    }
}
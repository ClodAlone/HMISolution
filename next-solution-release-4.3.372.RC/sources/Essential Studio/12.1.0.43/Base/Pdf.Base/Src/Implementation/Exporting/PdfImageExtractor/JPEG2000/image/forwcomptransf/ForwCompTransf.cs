#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Pdf.JPEG2000.wavelet.analysis;
using Syncfusion.Pdf.JPEG2000.util;
using Syncfusion.Pdf.JPEG2000.encoder;
namespace Syncfusion.Pdf.JPEG2000.image.forwcomptransf
{
    internal class ForwCompTransf : ImgDataAdapter, BlockImageDataSource
    {
        public static System.String[][] ParameterInfo
        {
            get
            {
                return pinfo;
            }
        }
        virtual public bool Reversible
        {
            get
            {
                switch (transfType)
                {
                    case NONE:
                    case FORW_RCT:
                        return true;
                    case FORW_ICT:
                        return false;
                    default:
                        throw new System.ArgumentException("Non JPEG 2000 part I" + " component transformation");
                }
            }
        }
        public const int NONE = 0;
        public const int FORW_RCT = 1;
        public const int FORW_ICT = 2;
        private BlockImageDataSource src;
        private CompTransfSpec cts;
        private AnWTFilterSpec wfs;
        private int transfType = NONE;
        private int[] tdepth;
        private DataBlock outBlk;
        private DataBlockInt block0;
        private DataBlockInt block1;
        private DataBlockInt block2;
        internal ForwCompTransf(BlockImageDataSource imgSrc, EncoderSpecs encSpec)
            : base(imgSrc)
        {
            this.cts = encSpec.cts;
            this.wfs = encSpec.wfs;
            src = imgSrc;
        }
        internal const char OPT_PREFIX = 'M';
        private static readonly System.String[][] pinfo = new System.String[][] { new System.String[] { "Mct", "[<tile index>] [on|off] ...", "Specifies in which tiles to use a multiple component transform. " + "Note that this multiple component transform can only be applied " + "in tiles that contain at least three components and whose " + "components are processed with the same wavelet filters and " + "quantization type. " + "If the wavelet transform is reversible (w5x3 filter), the " + "Reversible Component Transformation (RCT) is applied. If not " + "(w9x7 filter), the Irreversible Component Transformation (ICT)" + " is used.", null } };
        public virtual int getFixedPoint(int c)
        {
            return src.getFixedPoint(c);
        }
        public static int[] calcMixedBitDepths(int[] ntdepth, int ttype, int[] tdepth)
        {
            if (ntdepth.Length < 3 && ttype != NONE)
            {
                throw new System.ArgumentException();
            }
            if (tdepth == null)
            {
                tdepth = new int[ntdepth.Length];
            }
            switch (ttype)
            {
                case NONE:
                    Array.Copy(ntdepth, 0, tdepth, 0, ntdepth.Length);
                    break;
                case FORW_RCT:
                    if (ntdepth.Length > 3)
                    {
                        Array.Copy(ntdepth, 3, tdepth, 3, ntdepth.Length - 3);
                    }
                    tdepth[0] = MathUtil.log2((1 << ntdepth[0]) + (2 << ntdepth[1]) + (1 << ntdepth[2]) - 1) - 2 + 1;
                    tdepth[1] = MathUtil.log2((1 << ntdepth[2]) + (1 << ntdepth[1]) - 1) + 1;
                    tdepth[2] = MathUtil.log2((1 << ntdepth[0]) + (1 << ntdepth[1]) - 1) + 1;
                    break;
                case FORW_ICT:
                    if (ntdepth.Length > 3)
                    {
                        Array.Copy(ntdepth, 3, tdepth, 3, ntdepth.Length - 3);
                    }
                    tdepth[0] = MathUtil.log2((int)System.Math.Floor((1 << ntdepth[0]) * 0.299072 + (1 << ntdepth[1]) * 0.586914 + (1 << ntdepth[2]) * 0.114014) - 1) + 1;
                    tdepth[1] = MathUtil.log2((int)System.Math.Floor((1 << ntdepth[0]) * 0.168701 + (1 << ntdepth[1]) * 0.331299 + (1 << ntdepth[2]) * 0.5) - 1) + 1;
                    tdepth[2] = MathUtil.log2((int)System.Math.Floor((1 << ntdepth[0]) * 0.5 + (1 << ntdepth[1]) * 0.418701 + (1 << ntdepth[2]) * 0.081299) - 1) + 1;
                    break;
            }
            return tdepth;
        }
        private void initForwRCT()
        {
            int i;
            int tIdx = TileIdx;
            if (src.NumComps < 3)
            {
                throw new System.ArgumentException();
            }
            if (src.getTileComponentWidth(tIdx, 0) != src.getTileComponentWidth(tIdx, 1) || src.getTileComponentWidth(tIdx, 0) != src.getTileComponentWidth(tIdx, 2) || src.getTileComponentHeight(tIdx, 0) != src.getTileComponentHeight(tIdx, 1) || src.getTileComponentHeight(tIdx, 0) != src.getTileComponentHeight(tIdx, 2))
            {
                throw new System.ArgumentException("Can not use RCT " + "on components with different " + "dimensions");
            }
            int[] utd;
            utd = new int[src.NumComps];
            for (i = utd.Length - 1; i >= 0; i--)
            {
                utd[i] = src.getNomRangeBits(i);
            }
            tdepth = calcMixedBitDepths(utd, FORW_RCT, null);
        }
        private void initForwICT()
        {
            int i;
            int tIdx = TileIdx;
            if (src.NumComps < 3)
            {
                throw new System.ArgumentException();
            }
            if (src.getTileComponentWidth(tIdx, 0) != src.getTileComponentWidth(tIdx, 1) || src.getTileComponentWidth(tIdx, 0) != src.getTileComponentWidth(tIdx, 2) || src.getTileComponentHeight(tIdx, 0) != src.getTileComponentHeight(tIdx, 1) || src.getTileComponentHeight(tIdx, 0) != src.getTileComponentHeight(tIdx, 2))
            {
                throw new System.ArgumentException("Can not use ICT " + "on components with different " + "dimensions");
            }
            int[] utd;
            utd = new int[src.NumComps];
            for (i = utd.Length - 1; i >= 0; i--)
            {
                utd[i] = src.getNomRangeBits(i);
            }
            tdepth = calcMixedBitDepths(utd, FORW_ICT, null);
        }
        public override System.String ToString()
        {
            switch (transfType)
            {
                case FORW_RCT:
                    return "Forward RCT";
                case FORW_ICT:
                    return "Forward ICT";
                case NONE:
                    return "No component transformation";
                default:
                    throw new System.ArgumentException("Non JPEG 2000 part I" + " component transformation");
            }
        }
        public override int getNomRangeBits(int c)
        {
            switch (transfType)
            {
                case FORW_RCT:
                case FORW_ICT:
                    return tdepth[c];
                case NONE:
                    return src.getNomRangeBits(c);
                default:
                    throw new System.ArgumentException("Non JPEG 2000 part I" + " component transformation");
            }
        }
        public virtual DataBlock getCompData(DataBlock blk, int c)
        {
            if (c >= 3 || transfType == NONE)
            {
                return src.getCompData(blk, c);
            }
            else
            {
                return getInternCompData(blk, c);
            }
        }
        public virtual DataBlock getInternCompData(DataBlock blk, int c)
        {
            switch (transfType)
            {
                case NONE:
                    return src.getInternCompData(blk, c);
                case FORW_RCT:
                    return forwRCT(blk, c);
                case FORW_ICT:
                    return forwICT(blk, c);
                default:
                    throw new System.ArgumentException("Non JPEG 2000 part 1 " + "component" + " transformation for tile: " + tIdx);
            }
        }
        private DataBlock forwRCT(DataBlock blk, int c)
        {
            int k, k0, k1, k2, mink, i;
            int w = blk.w;
            int h = blk.h;
            int[] outdata;
            if (c >= 0 && c <= 2)
            {
                if (blk.DataType != DataBlock.TYPE_INT)
                {
                    if (outBlk == null || outBlk.DataType != DataBlock.TYPE_INT)
                    {
                        outBlk = new DataBlockInt();
                    }
                    outBlk.w = w;
                    outBlk.h = h;
                    outBlk.ulx = blk.ulx;
                    outBlk.uly = blk.uly;
                    blk = outBlk;
                }
                outdata = (int[])blk.Data;
                if (outdata == null || outdata.Length < h * w)
                {
                    outdata = new int[h * w];
                    blk.Data = outdata;
                }
                int[] data0, data1, bdata;
                if (block0 == null)
                    block0 = new DataBlockInt();
                if (block1 == null)
                    block1 = new DataBlockInt();
                if (block2 == null)
                    block2 = new DataBlockInt();
                block0.w = block1.w = block2.w = blk.w;
                block0.h = block1.h = block2.h = blk.h;
                block0.ulx = block1.ulx = block2.ulx = blk.ulx;
                block0.uly = block1.uly = block2.uly = blk.uly;
                block0 = (DataBlockInt)src.getInternCompData(block0, 0);
                data0 = (int[])block0.Data;
                block1 = (DataBlockInt)src.getInternCompData(block1, 1);
                data1 = (int[])block1.Data;
                block2 = (DataBlockInt)src.getInternCompData(block2, 2);
                bdata = (int[])block2.Data;
                blk.progressive = block0.progressive || block1.progressive || block2.progressive;
                blk.offset = 0;
                blk.scanw = w;
                k = w * h - 1;
                k0 = block0.offset + (h - 1) * block0.scanw + w - 1;
                k1 = block1.offset + (h - 1) * block1.scanw + w - 1;
                k2 = block2.offset + (h - 1) * block2.scanw + w - 1;
                switch (c)
                {
                    case 0:
                        for (i = h - 1; i >= 0; i--)
                        {
                            for (mink = k - w; k > mink; k--, k0--, k1--, k2--)
                            {
                                outdata[k] = (data0[k] + 2 * data1[k] + bdata[k]) >> 2;
                            }
                            k0 -= (block0.scanw - w);
                            k1 -= (block1.scanw - w);
                            k2 -= (block2.scanw - w);
                        }
                        break;
                    case 1:
                        for (i = h - 1; i >= 0; i--)
                        {
                            for (mink = k - w; k > mink; k--, k1--, k2--)
                            {
                                outdata[k] = bdata[k2] - data1[k1];
                            }
                            k1 -= (block1.scanw - w);
                            k2 -= (block2.scanw - w);
                        }
                        break;
                    case 2:
                        for (i = h - 1; i >= 0; i--)
                        {
                            for (mink = k - w; k > mink; k--, k0--, k1--)
                            {
                                outdata[k] = data0[k0] - data1[k1];
                            }
                            k0 -= (block0.scanw - w);
                            k1 -= (block1.scanw - w);
                        }
                        break;
                }
            }
            else if (c >= 3)
            {
                return src.getInternCompData(blk, c);
            }
            else
            {
                throw new System.ArgumentException();
            }
            return blk;
        }
        private DataBlock forwICT(DataBlock blk, int c)
        {
            int k, k0, k1, k2, mink, i;
            int w = blk.w;
            int h = blk.h;
            float[] outdata;
            if (blk.DataType != DataBlock.TYPE_FLOAT)
            {
                if (outBlk == null || outBlk.DataType != DataBlock.TYPE_FLOAT)
                {
                    outBlk = new DataBlockFloat();
                }
                outBlk.w = w;
                outBlk.h = h;
                outBlk.ulx = blk.ulx;
                outBlk.uly = blk.uly;
                blk = outBlk;
            }
            outdata = (float[])blk.Data;
            if (outdata == null || outdata.Length < w * h)
            {
                outdata = new float[h * w];
                blk.Data = outdata;
            }
            if (c >= 0 && c <= 2)
            {
                int[] data0, data1, data2;
                if (block0 == null)
                {
                    block0 = new DataBlockInt();
                }
                if (block1 == null)
                {
                    block1 = new DataBlockInt();
                }
                if (block2 == null)
                {
                    block2 = new DataBlockInt();
                }
                block0.w = block1.w = block2.w = blk.w;
                block0.h = block1.h = block2.h = blk.h;
                block0.ulx = block1.ulx = block2.ulx = blk.ulx;
                block0.uly = block1.uly = block2.uly = blk.uly;
                block0 = (DataBlockInt)src.getInternCompData(block0, 0);
                data0 = (int[])block0.Data;
                block1 = (DataBlockInt)src.getInternCompData(block1, 1);
                data1 = (int[])block1.Data;
                block2 = (DataBlockInt)src.getInternCompData(block2, 2);
                data2 = (int[])block2.Data;
                blk.progressive = block0.progressive || block1.progressive || block2.progressive;
                blk.offset = 0;
                blk.scanw = w;
                k = w * h - 1;
                k0 = block0.offset + (h - 1) * block0.scanw + w - 1;
                k1 = block1.offset + (h - 1) * block1.scanw + w - 1;
                k2 = block2.offset + (h - 1) * block2.scanw + w - 1;
                switch (c)
                {
                    case 0:
                        for (i = h - 1; i >= 0; i--)
                        {
                            for (mink = k - w; k > mink; k--, k0--, k1--, k2--)
                            {
                                outdata[k] = 0.299f * data0[k0] + 0.587f * data1[k1] + 0.114f * data2[k2];
                            }
                            k0 -= (block0.scanw - w);
                            k1 -= (block1.scanw - w);
                            k2 -= (block2.scanw - w);
                        }
                        break;
                    case 1:
                        for (i = h - 1; i >= 0; i--)
                        {
                            for (mink = k - w; k > mink; k--, k0--, k1--, k2--)
                            {
                                outdata[k] = (-0.16875f) * data0[k0] - 0.33126f * data1[k1] + 0.5f * data2[k2];
                            }
                            k0 -= (block0.scanw - w);
                            k1 -= (block1.scanw - w);
                            k2 -= (block2.scanw - w);
                        }
                        break;
                    case 2:
                        for (i = h - 1; i >= 0; i--)
                        {
                            for (mink = k - w; k > mink; k--, k0--, k1--, k2--)
                            {
                                outdata[k] = 0.5f * data0[k0] - 0.41869f * data1[k1] - 0.08131f * data2[k2];
                            }
                            k0 -= (block0.scanw - w);
                            k1 -= (block1.scanw - w);
                            k2 -= (block2.scanw - w);
                        }
                        break;
                }
            }
            else if (c >= 3)
            {
                DataBlockInt indb = new DataBlockInt(blk.ulx, blk.uly, w, h);
                int[] indata;
                src.getInternCompData(indb, c);
                indata = (int[])indb.Data;
                k = w * h - 1;
                k0 = indb.offset + (h - 1) * indb.scanw + w - 1;
                for (i = h - 1; i >= 0; i--)
                {
                    for (mink = k - w; k > mink; k--, k0--)
                    {
                        outdata[k] = (float)indata[k0];
                    }
                    k0 += indb.w - w;
                }
                blk.progressive = indb.progressive;
                blk.offset = 0;
                blk.scanw = w;
                return blk;
            }
            else
            {
                throw new System.ArgumentException();
            }
            return blk;
        }
        public override void setTile(int x, int y)
        {
            src.setTile(x, y);
            tIdx = TileIdx;
            System.String str = (System.String)cts.getTileDef(tIdx);
            if (str.Equals("none"))
            {
                transfType = NONE;
            }
            else if (str.Equals("rct"))
            {
                transfType = FORW_RCT;
                initForwRCT();
            }
            else if (str.Equals("ict"))
            {
                transfType = FORW_ICT;
                initForwICT();
            }
            else
            {
                throw new System.ArgumentException("Component transformation" + " not recognized");
            }
        }
        public override void nextTile()
        {
            src.nextTile();
            tIdx = TileIdx;
            System.String str = (System.String)cts.getTileDef(tIdx);
            if (str.Equals("none"))
            {
                transfType = NONE;
            }
            else if (str.Equals("rct"))
            {
                transfType = FORW_RCT;
                initForwRCT();
            }
            else if (str.Equals("ict"))
            {
                transfType = FORW_ICT;
                initForwICT();
            }
            else
            {
                throw new System.ArgumentException("Component transformation" + " not recognized");
            }
        }
    }
}
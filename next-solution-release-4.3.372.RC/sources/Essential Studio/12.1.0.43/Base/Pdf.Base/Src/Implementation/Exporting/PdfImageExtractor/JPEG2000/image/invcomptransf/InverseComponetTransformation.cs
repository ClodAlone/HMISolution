#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Pdf.JPEG2000.wavelet.synthesis;
using Syncfusion.Pdf.JPEG2000.decoder;
using Syncfusion.Pdf.JPEG2000.image;
using Syncfusion.Pdf.JPEG2000.util;
using Syncfusion.Pdf.JPEG2000;
namespace Syncfusion.Pdf.JPEG2000.image.invcomptransf
{
    internal class InverseComponetTransformation : ImgDataAdapter, BlockImageDataSource
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
                    case INV_RCT:
                        return true;
                    case INV_ICT:
                        return false;
                    default:
                        throw new System.ArgumentException("Non JPEG 2000 part I" + " component transformation");
                }
            }
        }
        public const int NONE = 0;
        public const char OPT_PREFIX = 'M';
        private static readonly System.String[][] pinfo = null;
        public const int INV_RCT = 1;
        public const int INV_ICT = 2;
        private BlockImageDataSource src;
        private CompTransfSpec cts;
        private SynWTFilterSpec wfs;
        private int transfType = NONE;
        private int[][] outdata = new int[3][];
        private DataBlock block0;
        private DataBlock block1;
        private DataBlock block2;
        private DataBlockInt dbi = new DataBlockInt();
        private int[] utdepth;
        private bool noCompTransf = false;
        internal InverseComponetTransformation(BlockImageDataSource imgSrc, DecodeHelper decSpec, int[] utdepth, JPXParameters pl)
            : base(imgSrc)
        {
            this.cts = decSpec.cts;
            this.wfs = decSpec.wfs;
            src = imgSrc;
            this.utdepth = utdepth;
            noCompTransf = !(pl.getBooleanParameter("comp_transf"));
        }
        public override System.String ToString()
        {
            switch (transfType)
            {
                case INV_RCT:
                    return "Inverse RCT";
                case INV_ICT:
                    return "Inverse ICT";
                case NONE:
                    return "No component transformation";
                default:
                    throw new System.ArgumentException("Non JPEG 2000 part I" + " component transformation");
            }
        }
        public virtual int getFixedPoint(int c)
        {
            return src.getFixedPoint(c);
        }
        public static int[] calcMixedBitDepths(int[] utdepth, int ttype, int[] tdepth)
        {
            if (utdepth.Length < 3 && ttype != NONE)
            {
                throw new System.ArgumentException();
            }
            if (tdepth == null)
            {
                tdepth = new int[utdepth.Length];
            }
            switch (ttype)
            {
                case NONE:
                    Array.Copy(utdepth, 0, tdepth, 0, utdepth.Length);
                    break;
                case INV_RCT:
                    if (utdepth.Length > 3)
                    {
                        Array.Copy(utdepth, 3, tdepth, 3, utdepth.Length - 3);
                    }
                    tdepth[0] = MathUtil.log2((1 << utdepth[0]) + (2 << utdepth[1]) + (1 << utdepth[2]) - 1) - 2 + 1;
                    tdepth[1] = MathUtil.log2((1 << utdepth[2]) + (1 << utdepth[1]) - 1) + 1;
                    tdepth[2] = MathUtil.log2((1 << utdepth[0]) + (1 << utdepth[1]) - 1) + 1;
                    break;
                case INV_ICT:
                    if (utdepth.Length > 3)
                    {
                        Array.Copy(utdepth, 3, tdepth, 3, utdepth.Length - 3);
                    }
                    tdepth[0] = MathUtil.log2((int)System.Math.Floor((1 << utdepth[0]) * 0.299072 + (1 << utdepth[1]) * 0.586914 + (1 << utdepth[2]) * 0.114014) - 1) + 1;
                    tdepth[1] = MathUtil.log2((int)System.Math.Floor((1 << utdepth[0]) * 0.168701 + (1 << utdepth[1]) * 0.331299 + (1 << utdepth[2]) * 0.5) - 1) + 1;
                    tdepth[2] = MathUtil.log2((int)System.Math.Floor((1 << utdepth[0]) * 0.5 + (1 << utdepth[1]) * 0.418701 + (1 << utdepth[2]) * 0.081299) - 1) + 1;
                    break;
            }
            return tdepth;
        }
        public override int getNomRangeBits(int c)
        {
            return utdepth[c];
        }
        public virtual DataBlock getCompData(DataBlock blk, int c)
        {
            if (c >= 3 || transfType == NONE || noCompTransf)
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
            if (noCompTransf)
                return src.getInternCompData(blk, c);
            switch (transfType)
            {
                case NONE:
                    return src.getInternCompData(blk, c);
                case INV_RCT:
                    return invRCT(blk, c);
                case INV_ICT:
                    return invICT(blk, c);
                default:
                    throw new System.ArgumentException("Non JPEG 2000 part I" + " component transformation");
            }
        }
        private DataBlock invRCT(DataBlock blk, int c)
        {
            if (c >= 3 && c < NumComps)
            {
                return src.getInternCompData(blk, c);
            }
            else if ((outdata[c] == null) || (dbi.ulx > blk.ulx) || (dbi.uly > blk.uly) || (dbi.ulx + dbi.w < blk.ulx + blk.w) || (dbi.uly + dbi.h < blk.uly + blk.h))
            {
                int k, k0, k1, k2, mink, i;
                int w = blk.w;
                int h = blk.h;
                outdata[c] = (int[])blk.Data;
                if (outdata[c] == null || outdata[c].Length != h * w)
                {
                    outdata[c] = new int[h * w];
                    blk.Data = outdata[c];
                }
                outdata[(c + 1) % 3] = new int[outdata[c].Length];
                outdata[(c + 2) % 3] = new int[outdata[c].Length];
                if (block0 == null || block0.DataType != DataBlock.TYPE_INT)
                    block0 = new DataBlockInt();
                if (block1 == null || block1.DataType != DataBlock.TYPE_INT)
                    block1 = new DataBlockInt();
                if (block2 == null || block2.DataType != DataBlock.TYPE_INT)
                    block2 = new DataBlockInt();
                block0.w = block1.w = block2.w = blk.w;
                block0.h = block1.h = block2.h = blk.h;
                block0.ulx = block1.ulx = block2.ulx = blk.ulx;
                block0.uly = block1.uly = block2.uly = blk.uly;
                int[] data0, data1, data2; // input data arrays
                block0 = (DataBlockInt)src.getInternCompData(block0, 0);
                data0 = (int[])block0.Data;
                block1 = (DataBlockInt)src.getInternCompData(block1, 1);
                data1 = (int[])block1.Data;
                block2 = (DataBlockInt)src.getInternCompData(block2, 2);
                data2 = (int[])block2.Data;
                blk.progressive = block0.progressive || block1.progressive || block2.progressive;
                blk.offset = 0;
                blk.scanw = w;
                dbi.progressive = blk.progressive;
                dbi.ulx = blk.ulx;
                dbi.uly = blk.uly;
                dbi.w = blk.w;
                dbi.h = blk.h;
                k = w * h - 1;
                k0 = block0.offset + (h - 1) * block0.scanw + w - 1;
                k1 = block1.offset + (h - 1) * block1.scanw + w - 1;
                k2 = block2.offset + (h - 1) * block2.scanw + w - 1;
                for (i = h - 1; i >= 0; i--)
                {
                    for (mink = k - w; k > mink; k--, k0--, k1--, k2--)
                    {
                        outdata[1][k] = (data0[k0] - ((data1[k1] + data2[k2]) >> 2));
                        outdata[0][k] = data2[k2] + outdata[1][k];
                        outdata[2][k] = data1[k1] + outdata[1][k];
                    }
                    k0 -= (block0.scanw - w);
                    k1 -= (block1.scanw - w);
                    k2 -= (block2.scanw - w);
                }
                outdata[c] = null;
            }
            else if ((c >= 0) && (c < 3))
            {
                blk.Data = outdata[c];
                blk.progressive = dbi.progressive;
                blk.offset = (blk.uly - dbi.uly) * dbi.w + blk.ulx - dbi.ulx;
                blk.scanw = dbi.w;
                outdata[c] = null;
            }
            else
            {
                throw new System.ArgumentException();
            }
            return blk;
        }
        private DataBlock invICT(DataBlock blk, int c)
        {
            if (c >= 3 && c < NumComps)
            {
                int k, k0, mink, i;
                int w = blk.w;
                int h = blk.h;
                int[] out_data;
                out_data = (int[])blk.Data;
                if (out_data == null)
                {
                    out_data = new int[h * w];
                    blk.Data = out_data;
                }
                DataBlockFloat indb = new DataBlockFloat(blk.ulx, blk.uly, w, h);
                float[] indata;
                src.getInternCompData(indb, c);
                indata = (float[])indb.Data;
                k = w * h - 1;
                k0 = indb.offset + (h - 1) * indb.scanw + w - 1;
                for (i = h - 1; i >= 0; i--)
                {
                    for (mink = k - w; k > mink; k--, k0--)
                    {
                        out_data[k] = (int)(indata[k0]);
                    }
                    k0 -= (indb.scanw - w);
                }
                blk.progressive = indb.progressive;
                blk.offset = 0;
                blk.scanw = w;
            }
            else if ((outdata[c] == null) || (dbi.ulx > blk.ulx) || (dbi.uly > blk.uly) || (dbi.ulx + dbi.w < blk.ulx + blk.w) || (dbi.uly + dbi.h < blk.uly + blk.h))
            {
                int k, k0, k1, k2, mink, i;
                int w = blk.w;
                int h = blk.h;
                outdata[c] = (int[])blk.Data;
                if (outdata[c] == null || outdata[c].Length != w * h)
                {
                    outdata[c] = new int[h * w];
                    blk.Data = outdata[c];
                }
                outdata[(c + 1) % 3] = new int[outdata[c].Length];
                outdata[(c + 2) % 3] = new int[outdata[c].Length];
                if (block0 == null || block0.DataType != DataBlock.TYPE_FLOAT)
                    block0 = new DataBlockFloat();
                if (block2 == null || block2.DataType != DataBlock.TYPE_FLOAT)
                    block2 = new DataBlockFloat();
                if (block1 == null || block1.DataType != DataBlock.TYPE_FLOAT)
                    block1 = new DataBlockFloat();
                block0.w = block2.w = block1.w = blk.w;
                block0.h = block2.h = block1.h = blk.h;
                block0.ulx = block2.ulx = block1.ulx = blk.ulx;
                block0.uly = block2.uly = block1.uly = blk.uly;
                float[] data0, data1, data2;
                block0 = (DataBlockFloat)src.getInternCompData(block0, 0);
                data0 = (float[])block0.Data;
                block2 = (DataBlockFloat)src.getInternCompData(block2, 1);
                data2 = (float[])block2.Data;
                block1 = (DataBlockFloat)src.getInternCompData(block1, 2);
                data1 = (float[])block1.Data;
                blk.progressive = block0.progressive || block1.progressive || block2.progressive;
                blk.offset = 0;
                blk.scanw = w;
                dbi.progressive = blk.progressive;
                dbi.ulx = blk.ulx;
                dbi.uly = blk.uly;
                dbi.w = blk.w;
                dbi.h = blk.h;
                k = w * h - 1;
                k0 = block0.offset + (h - 1) * block0.scanw + w - 1;
                k2 = block2.offset + (h - 1) * block2.scanw + w - 1;
                k1 = block1.offset + (h - 1) * block1.scanw + w - 1;
                for (i = h - 1; i >= 0; i--)
                {
                    for (mink = k - w; k > mink; k--, k0--, k2--, k1--)
                    {
                        outdata[0][k] = (int)(data0[k0] + 1.402f * data1[k1] + 0.5f);
                        outdata[1][k] = (int)(data0[k0] - 0.34413f * data2[k2] - 0.71414f * data1[k1] + 0.5f);
                        outdata[2][k] = (int)(data0[k0] + 1.772f * data2[k2] + 0.5f);
                    }
                    k0 -= (block0.scanw - w);
                    k2 -= (block2.scanw - w);
                    k1 -= (block1.scanw - w);
                }
                outdata[c] = null;
            }
            else if ((c >= 0) && (c <= 3))
            {
                blk.Data = outdata[c];
                blk.progressive = dbi.progressive;
                blk.offset = (blk.uly - dbi.uly) * dbi.w + blk.ulx - dbi.ulx;
                blk.scanw = dbi.w;
                outdata[c] = null;
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
            if (((System.Int32)cts.getTileDef(tIdx)) == NONE)
                transfType = NONE;
            else
            {
                int nc = src.NumComps > 3 ? 3 : src.NumComps;
                int rev = 0;
                for (int c = 0; c < nc; c++)
                {
                    rev += (wfs.isReversible(tIdx, c) ? 1 : 0);
                }
                if (rev == 3)
                {
                    transfType = INV_RCT;
                }
                else if (rev == 0)
                {
                    transfType = INV_ICT;
                }
                else
                {
                    throw new System.ArgumentException("Wavelet transformation and " + "component transformation" + " not coherent in tile" + tIdx);
                }
            }
        }
        public override void nextTile()
        {
            src.nextTile();
            tIdx = TileIdx;
            if (((System.Int32)cts.getTileDef(tIdx)) == NONE)
                transfType = NONE;
            else
            {
                int nc = src.NumComps > 3 ? 3 : src.NumComps;
                int rev = 0;
                for (int c = 0; c < nc; c++)
                {
                    rev += (wfs.isReversible(tIdx, c) ? 1 : 0);
                }
                if (rev == 3)
                {
                    transfType = INV_RCT;
                }
                else if (rev == 0)
                {
                    transfType = INV_ICT;
                }
                else
                {
                    throw new System.ArgumentException("Wavelet transformation and " + "component transformation" + " not coherent in tile" + tIdx);
                }
            }
        }
    }
}
#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.quantization.dequantizer;
using Syncfusion.Pdf.JPEG2000.wavelet.synthesis;
using Syncfusion.Pdf.JPEG2000.entropy.decoder;
using Syncfusion.Pdf.JPEG2000.wavelet;
using Syncfusion.Pdf.JPEG2000.decoder;
using Syncfusion.Pdf.JPEG2000.image;
using Syncfusion.Pdf.JPEG2000.util;
using Syncfusion.Pdf.JPEG2000.io;
namespace Syncfusion.Pdf.JPEG2000.codestream.reader
{
    public abstract class BitstreamReader : CodedCBlkDataSrcDec
    {
        virtual public int CbULX
        {
            get
            {
                return hd.CbULX;
            }
        }
        virtual public int CbULY
        {
            get
            {
                return hd.CbULY;
            }
        }
        virtual public int NumComps
        {
            get
            {
                return nc;
            }
        }
        virtual public int TileIdx
        {
            get
            {
                return ctY * ntX + ctX;
            }
        }
        public static System.String[][] ParameterInfo
        {
            get
            {
                return pinfo;
            }
        }
        virtual public int ImgRes
        {
            get
            {
                return targetRes;
            }
        }
        virtual public float TargetRate
        {
            get
            {
                return trate;
            }
        }
        virtual public float ActualRate
        {
            get
            {
                arate = anbytes * 8f / hd.MaxCompImgWidth / hd.MaxCompImgHeight;
                return arate;
            }
        }
        virtual public int TargetNbytes
        {
            get
            {
                return tnbytes;
            }
        }
        virtual public int ActualNbytes
        {
            get
            {
                return anbytes;
            }
        }
        virtual public int TilePartULX
        {
            get
            {
                return hd.getTilingOrigin(null).x;
            }
        }
        virtual public int TilePartULY
        {
            get
            {
                return hd.getTilingOrigin(null).y;
            }
        }
        virtual public int NomTileWidth
        {
            get
            {
                return hd.NomTileWidth;
            }
        }
        virtual public int NomTileHeight
        {
            get
            {
                return hd.NomTileHeight;
            }
        }
        internal DecodeHelper decSpec;
        internal bool[] derived = null;
        internal int[] gb = null;
        internal StdDequantizerParams[] params_Renamed = null;
        public const char OPT_PREFIX = 'B';
        private static readonly System.String[][] pinfo = null;
        internal int[] mdl;
        internal int nc;
        internal int targetRes;
        internal SubbandSyn[] subbTrees;
        internal int imgW;
        internal int imgH;
        internal int ax;
        internal int ay;
        internal int px;
        internal int py;
        internal int[] offX;
        internal int[] offY;
        internal int[] culx;
        internal int[] culy;
        internal int ntW;
        internal int ntH;
        internal int ntX;
        internal int ntY;
        internal int nt;
        internal int ctX;
        internal int ctY;
        internal HeaderDecoder hd;
        internal int tnbytes;
        internal int anbytes;
        internal float trate;
        internal float arate;
        internal BitstreamReader(HeaderDecoder hd, DecodeHelper decSpec)
        {
            JPXImageCoordinates co;
            this.decSpec = decSpec;
            this.hd = hd;
            nc = hd.NumComps;
            offX = new int[nc];
            offY = new int[nc];
            culx = new int[nc];
            culy = new int[nc];
            imgW = hd.ImgWidth;
            imgH = hd.ImgHeight;
            ax = hd.ImgULX;
            ay = hd.ImgULY;
            co = hd.getTilingOrigin(null);
            px = co.x;
            py = co.y;
            ntW = hd.NomTileWidth;
            ntH = hd.NomTileHeight;
            ntX = (ax + imgW - px + ntW - 1) / ntW;
            ntY = (ay + imgH - py + ntH - 1) / ntH;
            nt = ntX * ntY;
        }
        public int getCompSubsX(int c)
        {
            return hd.getCompSubsX(c);
        }
        public virtual int getCompSubsY(int c)
        {
            return hd.getCompSubsY(c);
        }
        public virtual int getTileWidth(int rl)
        {
            int mindl = decSpec.dls.getMinInTile(TileIdx);
            if (rl > mindl)
            {
                throw new System.ArgumentException("Requested resolution level" + " is not available for, at " + "least, one component in " + "tile: " + ctX + "x" + ctY);
            }
            int ctulx, ntulx;
            int dl = mindl - rl;
            ctulx = (ctX == 0) ? ax : px + ctX * ntW;
            ntulx = (ctX < ntX - 1) ? px + (ctX + 1) * ntW : ax + imgW;
            return (ntulx + (1 << dl) - 1) / (1 << dl) - (ctulx + (1 << dl) - 1) / (1 << dl);
        }
        public virtual int getTileHeight(int rl)
        {
            int mindl = decSpec.dls.getMinInTile(TileIdx);
            if (rl > mindl)
            {
                throw new System.ArgumentException("Requested resolution level" + " is not available for, at " + "least, one component in" + " tile: " + ctX + "x" + ctY);
            }
            int ctuly, ntuly;
            int dl = mindl - rl;
            ctuly = (ctY == 0) ? ay : py + ctY * ntH;
            ntuly = (ctY < ntY - 1) ? py + (ctY + 1) * ntH : ay + imgH;
            return (ntuly + (1 << dl) - 1) / (1 << dl) - (ctuly + (1 << dl) - 1) / (1 << dl);
        }
        public virtual int getImgWidth(int rl)
        {
            int mindl = decSpec.dls.Min;
            if (rl > mindl)
            {
                throw new System.ArgumentException("Requested resolution level" + " is not available for, at " + "least, one tile-component");
            }
            int dl = mindl - rl;
            return (ax + imgW + (1 << dl) - 1) / (1 << dl) - (ax + (1 << dl) - 1) / (1 << dl);
        }
        public virtual int getImgHeight(int rl)
        {
            int mindl = decSpec.dls.Min;
            if (rl > mindl)
            {
                throw new System.ArgumentException("Requested resolution level" + " is not available for, at " + "least, one tile-component");
            }
            int dl = mindl - rl;
            return (ay + imgH + (1 << dl) - 1) / (1 << dl) - (ay + (1 << dl) - 1) / (1 << dl);
        }
        public virtual int getImgULX(int rl)
        {
            int mindl = decSpec.dls.Min;
            if (rl > mindl)
            {
                throw new System.ArgumentException("Requested resolution level" + " is not available for, at " + "least, one tile-component");
            }
            int dl = mindl - rl;
            return (ax + (1 << dl) - 1) / (1 << dl);
        }
        public virtual int getImgULY(int rl)
        {
            int mindl = decSpec.dls.Min;
            if (rl > mindl)
            {
                throw new System.ArgumentException("Requested resolution level" + " is not available for, at " + "least, one tile-component");
            }
            int dl = mindl - rl;
            return (ay + (1 << dl) - 1) / (1 << dl);
        }
        public int getTileCompWidth(int t, int c, int rl)
        {
            int tIdx = TileIdx;
            if (t != tIdx)
            {
                //throw new System.ApplicationException("Asking the tile-component width of a tile " + "different  from the current one.");
            }
            int ntulx;
            int dl = mdl[c] - rl;
            ntulx = (ctX < ntX - 1) ? px + (ctX + 1) * ntW : ax + imgW;
            ntulx = (ntulx + hd.getCompSubsX(c) - 1) / hd.getCompSubsX(c);
            return (ntulx + (1 << dl) - 1) / (1 << dl) - (culx[c] + (1 << dl) - 1) / (1 << dl);
        }
        public int getTileCompHeight(int t, int c, int rl)
        {
            int tIdx = TileIdx;
            if (t != tIdx)
            {
                //throw new System.ApplicationException("Asking the tile-component width of a tile " + "different  from the current one.");
            }
            int ntuly;
            int dl = mdl[c] - rl;
            ntuly = (ctY < ntY - 1) ? py + (ctY + 1) * ntH : ay + imgH;
            ntuly = (ntuly + hd.getCompSubsY(c) - 1) / hd.getCompSubsY(c);
            return (ntuly + (1 << dl) - 1) / (1 << dl) - (culy[c] + (1 << dl) - 1) / (1 << dl);
        }
        public int getCompImgWidth(int c, int rl)
        {
            int sx, ex;
            int dl = decSpec.dls.getMinInComp(c) - rl;
            sx = (ax + hd.getCompSubsX(c) - 1) / hd.getCompSubsX(c);
            ex = (ax + imgW + hd.getCompSubsX(c) - 1) / hd.getCompSubsX(c);
            return (ex + (1 << dl) - 1) / (1 << dl) - (sx + (1 << dl) - 1) / (1 << dl);
        }
        public int getCompImgHeight(int c, int rl)
        {
            int sy, ey;
            int dl = decSpec.dls.getMinInComp(c) - rl;
            sy = (ay + hd.getCompSubsY(c) - 1) / hd.getCompSubsY(c);
            ey = (ay + imgH + hd.getCompSubsY(c) - 1) / hd.getCompSubsY(c);
            return (ey + (1 << dl) - 1) / (1 << dl) - (sy + (1 << dl) - 1) / (1 << dl);
        }
        public abstract void setTile(int x, int y);
        public abstract void nextTile();
        public JPXImageCoordinates getTile(JPXImageCoordinates co)
        {
            if (co != null)
            {
                co.x = ctX;
                co.y = ctY;
                return co;
            }
            else
            {
                return new JPXImageCoordinates(ctX, ctY);
            }
        }
        public int getResULX(int c, int rl)
        {
            int dl = mdl[c] - rl;
            if (dl < 0)
            {
                throw new System.ArgumentException("Requested resolution level" + " is not available for, at " + "least, one component in " + "tile: " + ctX + "x" + ctY);
            }
            int tx0 = (int)System.Math.Max(px + ctX * ntW, ax);
            int tcx0 = (int)System.Math.Ceiling(tx0 / (double)getCompSubsX(c));
            return (int)System.Math.Ceiling(tcx0 / (double)(1 << dl));
        }
        public int getResULY(int c, int rl)
        {
            int dl = mdl[c] - rl;
            if (dl < 0)
            {
                throw new System.ArgumentException("Requested resolution level" + " is not available for, at " + "least, one component in " + "tile: " + ctX + "x" + ctY);
            }
            int ty0 = (int)System.Math.Max(py + ctY * ntH, ay);
            int tcy0 = (int)System.Math.Ceiling(ty0 / (double)getCompSubsY(c));
            return (int)System.Math.Ceiling(tcy0 / (double)(1 << dl));
        }
        public JPXImageCoordinates getNumTiles(JPXImageCoordinates co)
        {
            if (co != null)
            {
                co.x = ntX;
                co.y = ntY;
                return co;
            }
            else
            {
                return new JPXImageCoordinates(ntX, ntY);
            }
        }
        public int getNumTiles()
        {
            return ntX * ntY;
        }
        public SubbandSyn getSynSubbandTree(int t, int c)
        {
            if (t != TileIdx)
            {
                throw new System.ArgumentException("Can not request subband" + " tree of a different tile" + " than the current one");
            }
            if (c < 0 || c >= nc)
            {
                throw new System.ArgumentException("Component index out of range");
            }
            return subbTrees[c];
        }
        internal static BitstreamReader createInstance(JPXRandomAccessStream in_Renamed, HeaderDecoder hd, JPXParameters pl, DecodeHelper decSpec, bool cdstrInfo, HeaderInformation hi)
        {
            pl.checkList(BitstreamReader.OPT_PREFIX, Syncfusion.Pdf.JPEG2000.util.JPXParameters.toNameArray(BitstreamReader.ParameterInfo));
            return new FileBitstreamReaderAgent(hd, in_Renamed, decSpec, pl, cdstrInfo, hi);
        }
        public int getPPX(int t, int c, int rl)
        {
            return decSpec.pss.getPPX(t, c, rl);
        }
        public int getPPY(int t, int c, int rl)
        {
            return decSpec.pss.getPPY(t, c, rl);
        }
        internal virtual void initSubbandsFields(int c, SubbandSyn sb)
        {
            int t = TileIdx;
            int rl = sb.resLvl;
            int cbw, cbh;
            cbw = decSpec.cblks.getCBlkWidth(ModuleSpec.SPEC_TILE_COMP, t, c);
            cbh = decSpec.cblks.getCBlkHeight(ModuleSpec.SPEC_TILE_COMP, t, c);
            if (!sb.isNode)
            {
                if (hd.precinctPartitionUsed())
                {
                    int ppxExp, ppyExp, cbwExp, cbhExp;
                    ppxExp = MathUtil.log2(getPPX(t, c, rl));
                    ppyExp = MathUtil.log2(getPPY(t, c, rl));
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
                if (sb.w == 0 || sb.h == 0)
                {
                    sb.numCb.x = 0;
                    sb.numCb.y = 0;
                }
                else
                {
                    int cb0x = CbULX;
                    int cb0y = CbULY;
                    int tmp;
                    int acb0x = cb0x;
                    int acb0y = cb0y;
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
                if (derived[c])
                {
                    sb.magbits = gb[c] + (params_Renamed[c].exp[0][0] - (mdl[c] - sb.level)) - 1;
                }
                else
                {
                    sb.magbits = gb[c] + params_Renamed[c].exp[sb.resLvl][sb.sbandIdx] - 1;
                }
            }
            else
            {
                initSubbandsFields(c, (SubbandSyn)sb.LL);
                initSubbandsFields(c, (SubbandSyn)sb.HL);
                initSubbandsFields(c, (SubbandSyn)sb.LH);
                initSubbandsFields(c, (SubbandSyn)sb.HH);
            }
        }
        public abstract Syncfusion.Pdf.JPEG2000.entropy.decoder.DecLyrdCBlk getCodeBlock(int param1, int param2, int param3, Syncfusion.Pdf.JPEG2000.wavelet.synthesis.SubbandSyn param4, int param5, int param6, Syncfusion.Pdf.JPEG2000.entropy.decoder.DecLyrdCBlk param7);
    }
}
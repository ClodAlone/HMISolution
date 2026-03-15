#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Pdf.JPEG2000.wavelet.synthesis;
using Syncfusion.Pdf.JPEG2000.codestream;
using Syncfusion.Pdf.JPEG2000.entropy;
using Syncfusion.Pdf.JPEG2000.wavelet;
using Syncfusion.Pdf.JPEG2000.decoder;
using Syncfusion.Pdf.JPEG2000.image;
using Syncfusion.Pdf.JPEG2000.util;
using Syncfusion.Pdf.JPEG2000.io;
using HashTable = System.Collections.Generic.Dictionary<object, object>;
using ArrayList = System.Collections.Generic.List<object>;
namespace Syncfusion.Pdf.JPEG2000.codestream.reader
{
    internal class PktDecoder
    {
        private BitstreamReader src;
        private bool pph = false;
        private System.IO.MemoryStream pphbais;
        private DecodeHelper decSpec;
        private HeaderDecoder hd;
        private int INIT_LBLOCK = 3;
        private PktHeaderBitReader bin;
        private JPXRandomAccessStream ehs;
        private JPXImageCoordinates[][] numPrec;
        private int tIdx;
        private PrecInfo[][][] ppinfo;
        private int[][][][][] lblock;
        private TagTreeDecoder[][][][] ttIncl;
        private TagTreeDecoder[][][][] ttMaxBP;
        private int nl = 0;
        private int nc;
        private bool sopUsed = false;
        private bool ephUsed = false;
        private int pktIdx;
        private ArrayList[] cblks;
        private int ncb;
        private int maxCB;
        private bool ncbQuit;
        private int tQuit;
        private int cQuit;
        private int sQuit;
        private int rQuit;
        private int xQuit;
        private int yQuit;
        private bool isTruncMode;
        internal PktDecoder(DecodeHelper decSpec, HeaderDecoder hd, JPXRandomAccessStream ehs, BitstreamReader src, bool isTruncMode, int maxCB)
        {
            this.decSpec = decSpec;
            this.hd = hd;
            this.ehs = ehs;
            this.isTruncMode = isTruncMode;
            bin = new PktHeaderBitReader(ehs);
            this.src = src;
            ncb = 0;
            ncbQuit = false;
            this.maxCB = maxCB;
        }
        internal virtual CBlkInfo[][][][][] restart(int nc, int[] mdl, int nl, CBlkInfo[][][][][] cbI, bool pph, System.IO.MemoryStream pphbais)
        {
            this.nc = nc;
            this.nl = nl;
            this.tIdx = src.TileIdx;
            this.pph = pph;
            this.pphbais = pphbais;
            sopUsed = ((System.Boolean)decSpec.sops.getTileDef(tIdx));
            pktIdx = 0;
            ephUsed = ((System.Boolean)decSpec.ephs.getTileDef(tIdx));
            cbI = new CBlkInfo[nc][][][][];
            lblock = new int[nc][][][][];
            ttIncl = new TagTreeDecoder[nc][][][];
            ttMaxBP = new TagTreeDecoder[nc][][][];
            numPrec = new JPXImageCoordinates[nc][];
            ppinfo = new PrecInfo[nc][][];
            int tcx0, tcy0, tcx1, tcy1;
            int trx0, try0, trx1, try1;
            SubbandSyn root, sb;
            int mins, maxs;
            JPXImageCoordinates nBlk = null;
            int cb0x = src.CbULX;
            int cb0y = src.CbULY;
            for (int c = 0; c < nc; c++)
            {
                cbI[c] = new CBlkInfo[mdl[c] + 1][][][];
                lblock[c] = new int[mdl[c] + 1][][][];
                ttIncl[c] = new TagTreeDecoder[mdl[c] + 1][][];
                ttMaxBP[c] = new TagTreeDecoder[mdl[c] + 1][][];
                numPrec[c] = new JPXImageCoordinates[mdl[c] + 1];
                ppinfo[c] = new PrecInfo[mdl[c] + 1][];
                tcx0 = src.getResULX(c, mdl[c]);
                tcy0 = src.getResULY(c, mdl[c]);
                tcx1 = tcx0 + src.getTileCompWidth(tIdx, c, mdl[c]);
                tcy1 = tcy0 + src.getTileCompHeight(tIdx, c, mdl[c]);
                for (int r = 0; r <= mdl[c]; r++)
                {
                    trx0 = (int)System.Math.Ceiling(tcx0 / (double)(1 << (mdl[c] - r)));
                    try0 = (int)System.Math.Ceiling(tcy0 / (double)(1 << (mdl[c] - r)));
                    trx1 = (int)System.Math.Ceiling(tcx1 / (double)(1 << (mdl[c] - r)));
                    try1 = (int)System.Math.Ceiling(tcy1 / (double)(1 << (mdl[c] - r)));
                    double twoppx = (double)getPPX(tIdx, c, r);
                    double twoppy = (double)getPPY(tIdx, c, r);
                    numPrec[c][r] = new JPXImageCoordinates();
                    if (trx1 > trx0)
                    {
                        numPrec[c][r].x = (int)System.Math.Ceiling((trx1 - cb0x) / twoppx) - (int)System.Math.Floor((trx0 - cb0x) / twoppx);
                    }
                    else
                    {
                        numPrec[c][r].x = 0;
                    }
                    if (try1 > try0)
                    {
                        numPrec[c][r].y = (int)System.Math.Ceiling((try1 - cb0y) / twoppy) - (int)System.Math.Floor((try0 - cb0y) / twoppy);
                    }
                    else
                    {
                        numPrec[c][r].y = 0;
                    }
                    mins = (r == 0) ? 0 : 1;
                    maxs = (r == 0) ? 1 : 4;
                    int maxPrec = numPrec[c][r].x * numPrec[c][r].y;
                    ttIncl[c][r] = new TagTreeDecoder[maxPrec][];
                    for (int i = 0; i < maxPrec; i++)
                    {
                        ttIncl[c][r][i] = new TagTreeDecoder[maxs + 1];
                    }
                    ttMaxBP[c][r] = new TagTreeDecoder[maxPrec][];
                    for (int i2 = 0; i2 < maxPrec; i2++)
                    {
                        ttMaxBP[c][r][i2] = new TagTreeDecoder[maxs + 1];
                    }
                    cbI[c][r] = new CBlkInfo[maxs + 1][][];
                    lblock[c][r] = new int[maxs + 1][][];
                    ppinfo[c][r] = new PrecInfo[maxPrec];
                    fillPrecInfo(c, r, mdl[c]);
                    root = (SubbandSyn)src.getSynSubbandTree(tIdx, c);
                    for (int s = mins; s < maxs; s++)
                    {
                        sb = (SubbandSyn)root.getSubbandByIdx(r, s);
                        nBlk = sb.numCb;
                        cbI[c][r][s] = new CBlkInfo[nBlk.y][];
                        for (int i3 = 0; i3 < nBlk.y; i3++)
                        {
                            cbI[c][r][s][i3] = new CBlkInfo[nBlk.x];
                        }
                        lblock[c][r][s] = new int[nBlk.y][];
                        for (int i4 = 0; i4 < nBlk.y; i4++)
                        {
                            lblock[c][r][s][i4] = new int[nBlk.x];
                        }
                        for (int i = nBlk.y - 1; i >= 0; i--)
                        {
                            ArrayUtil.intArraySet(lblock[c][r][s][i], INIT_LBLOCK);
                        }
                    }
                }
            }
            return cbI;
        }
        private void fillPrecInfo(int c, int r, int mdl)
        {
            if (ppinfo[c][r].Length == 0)
                return;
            JPXImageCoordinates tileI = src.getTile(null);
            JPXImageCoordinates nTiles = src.getNumTiles(null);
            int xsiz, ysiz, x0siz, y0siz;
            int xt0siz, yt0siz;
            int xtsiz, ytsiz;
            xt0siz = src.TilePartULX;
            yt0siz = src.TilePartULY;
            xtsiz = src.NomTileWidth;
            ytsiz = src.NomTileHeight;
            x0siz = hd.ImgULX;
            y0siz = hd.ImgULY;
            xsiz = hd.ImgWidth;
            ysiz = hd.ImgHeight;
            int tx0 = (tileI.x == 0) ? x0siz : xt0siz + tileI.x * xtsiz;
            int ty0 = (tileI.y == 0) ? y0siz : yt0siz + tileI.y * ytsiz;
            int tx1 = (tileI.x != nTiles.x - 1) ? xt0siz + (tileI.x + 1) * xtsiz : xsiz;
            int ty1 = (tileI.y != nTiles.y - 1) ? yt0siz + (tileI.y + 1) * ytsiz : ysiz;
            int xrsiz = hd.getCompSubsX(c);
            int yrsiz = hd.getCompSubsY(c);
            int tcx0 = src.getResULX(c, mdl);
            int tcy0 = src.getResULY(c, mdl);
            int tcx1 = tcx0 + src.getTileCompWidth(tIdx, c, mdl);
            int tcy1 = tcy0 + src.getTileCompHeight(tIdx, c, mdl);
            int ndl = mdl - r;
            int trx0 = (int)System.Math.Ceiling(tcx0 / (double)(1 << ndl));
            int try0 = (int)System.Math.Ceiling(tcy0 / (double)(1 << ndl));
            int trx1 = (int)System.Math.Ceiling(tcx1 / (double)(1 << ndl));
            int try1 = (int)System.Math.Ceiling(tcy1 / (double)(1 << ndl));
            int cb0x = src.CbULX;
            int cb0y = src.CbULY;
            double twoppx = (double)getPPX(tIdx, c, r);
            double twoppy = (double)getPPY(tIdx, c, r);
            int twoppx2 = (int)(twoppx / 2);
            int twoppy2 = (int)(twoppy / 2);
            int maxPrec = ppinfo[c][r].Length;
            int nPrec = 0;
            int istart = (int)System.Math.Floor((try0 - cb0y) / twoppy);
            int iend = (int)System.Math.Floor((try1 - 1 - cb0y) / twoppy);
            int jstart = (int)System.Math.Floor((trx0 - cb0x) / twoppx);
            int jend = (int)System.Math.Floor((trx1 - 1 - cb0x) / twoppx);
            int acb0x, acb0y;
            SubbandSyn root = src.getSynSubbandTree(tIdx, c);
            SubbandSyn sb = null;
            int p0x, p0y, p1x, p1y;
            int s0x, s0y, s1x, s1y;
            int cw, ch;
            int kstart, kend, lstart, lend, k0, l0;
            int prg_ulx, prg_uly;
            int prg_w = (int)twoppx << ndl;
            int prg_h = (int)twoppy << ndl;
            int tmp1, tmp2;
            CBlkCoordInfo cb;
            for (int i = istart; i <= iend; i++)
            {
                for (int j = jstart; j <= jend; j++, nPrec++)
                {
                    if (j == jstart && (trx0 - cb0x) % (xrsiz * ((int)twoppx)) != 0)
                    {
                        prg_ulx = tx0;
                    }
                    else
                    {
                        prg_ulx = cb0x + j * xrsiz * ((int)twoppx << ndl);
                    }
                    if (i == istart && (try0 - cb0y) % (yrsiz * ((int)twoppy)) != 0)
                    {
                        prg_uly = ty0;
                    }
                    else
                    {
                        prg_uly = cb0y + i * yrsiz * ((int)twoppy << ndl);
                    }
                    ppinfo[c][r][nPrec] = new PrecInfo(r, (int)(cb0x + j * twoppx), (int)(cb0y + i * twoppy), (int)twoppx, (int)twoppy, prg_ulx, prg_uly, prg_w, prg_h);
                    if (r == 0)
                    {
                        acb0x = cb0x;
                        acb0y = cb0y;
                        p0x = acb0x + j * (int)twoppx;
                        p1x = p0x + (int)twoppx;
                        p0y = acb0y + i * (int)twoppy;
                        p1y = p0y + (int)twoppy;
                        sb = (SubbandSyn)root.getSubbandByIdx(0, 0);
                        s0x = (p0x < sb.ulcx) ? sb.ulcx : p0x;
                        s1x = (p1x > sb.ulcx + sb.w) ? sb.ulcx + sb.w : p1x;
                        s0y = (p0y < sb.ulcy) ? sb.ulcy : p0y;
                        s1y = (p1y > sb.ulcy + sb.h) ? sb.ulcy + sb.h : p1y;
                        cw = sb.nomCBlkW;
                        ch = sb.nomCBlkH;
                        k0 = (int)System.Math.Floor((sb.ulcy - acb0y) / (double)ch);
                        kstart = (int)System.Math.Floor((s0y - acb0y) / (double)ch);
                        kend = (int)System.Math.Floor((s1y - 1 - acb0y) / (double)ch);
                        l0 = (int)System.Math.Floor((sb.ulcx - acb0x) / (double)cw);
                        lstart = (int)System.Math.Floor((s0x - acb0x) / (double)cw);
                        lend = (int)System.Math.Floor((s1x - 1 - acb0x) / (double)cw);
                        if (s1x - s0x <= 0 || s1y - s0y <= 0)
                        {
                            ppinfo[c][r][nPrec].nblk[0] = 0;
                            ttIncl[c][r][nPrec][0] = new TagTreeDecoder(0, 0);
                            ttMaxBP[c][r][nPrec][0] = new TagTreeDecoder(0, 0);
                        }
                        else
                        {
                            ttIncl[c][r][nPrec][0] = new TagTreeDecoder(kend - kstart + 1, lend - lstart + 1);
                            ttMaxBP[c][r][nPrec][0] = new TagTreeDecoder(kend - kstart + 1, lend - lstart + 1);
                            CBlkCoordInfo[][] tmpArray = new CBlkCoordInfo[kend - kstart + 1][];
                            for (int i2 = 0; i2 < kend - kstart + 1; i2++)
                            {
                                tmpArray[i2] = new CBlkCoordInfo[lend - lstart + 1];
                            }
                            ppinfo[c][r][nPrec].cblk[0] = tmpArray;
                            ppinfo[c][r][nPrec].nblk[0] = (kend - kstart + 1) * (lend - lstart + 1);
                            for (int k = kstart; k <= kend; k++)
                            {
                                // Vertical cblks
                                for (int l = lstart; l <= lend; l++)
                                {
                                    // Horiz. cblks
                                    cb = new CBlkCoordInfo(k - k0, l - l0);
                                    if (l == l0)
                                    {
                                        cb.ulx = sb.ulx;
                                    }
                                    else
                                    {
                                        cb.ulx = sb.ulx + l * cw - (sb.ulcx - acb0x);
                                    }
                                    if (k == k0)
                                    {
                                        cb.uly = sb.uly;
                                    }
                                    else
                                    {
                                        cb.uly = sb.uly + k * ch - (sb.ulcy - acb0y);
                                    }
                                    tmp1 = acb0x + l * cw;
                                    tmp1 = (tmp1 > sb.ulcx) ? tmp1 : sb.ulcx;
                                    tmp2 = acb0x + (l + 1) * cw;
                                    tmp2 = (tmp2 > sb.ulcx + sb.w) ? sb.ulcx + sb.w : tmp2;
                                    cb.w = tmp2 - tmp1;
                                    tmp1 = acb0y + k * ch;
                                    tmp1 = (tmp1 > sb.ulcy) ? tmp1 : sb.ulcy;
                                    tmp2 = acb0y + (k + 1) * ch;
                                    tmp2 = (tmp2 > sb.ulcy + sb.h) ? sb.ulcy + sb.h : tmp2;
                                    cb.h = tmp2 - tmp1;
                                    ppinfo[c][r][nPrec].cblk[0][k - kstart][l - lstart] = cb;
                                }
                            }
                        }
                    }
                    else
                    {
                        acb0x = 0;
                        acb0y = cb0y;
                        p0x = acb0x + j * twoppx2;
                        p1x = p0x + twoppx2;
                        p0y = acb0y + i * twoppy2;
                        p1y = p0y + twoppy2;
                        sb = (SubbandSyn)root.getSubbandByIdx(r, 1);
                        s0x = (p0x < sb.ulcx) ? sb.ulcx : p0x;
                        s1x = (p1x > sb.ulcx + sb.w) ? sb.ulcx + sb.w : p1x;
                        s0y = (p0y < sb.ulcy) ? sb.ulcy : p0y;
                        s1y = (p1y > sb.ulcy + sb.h) ? sb.ulcy + sb.h : p1y;
                        cw = sb.nomCBlkW;
                        ch = sb.nomCBlkH;
                        k0 = (int)System.Math.Floor((sb.ulcy - acb0y) / (double)ch);
                        kstart = (int)System.Math.Floor((s0y - acb0y) / (double)ch);
                        kend = (int)System.Math.Floor((s1y - 1 - acb0y) / (double)ch);
                        l0 = (int)System.Math.Floor((sb.ulcx - acb0x) / (double)cw);
                        lstart = (int)System.Math.Floor((s0x - acb0x) / (double)cw);
                        lend = (int)System.Math.Floor((s1x - 1 - acb0x) / (double)cw);
                        if (s1x - s0x <= 0 || s1y - s0y <= 0)
                        {
                            ppinfo[c][r][nPrec].nblk[1] = 0;
                            ttIncl[c][r][nPrec][1] = new TagTreeDecoder(0, 0);
                            ttMaxBP[c][r][nPrec][1] = new TagTreeDecoder(0, 0);
                        }
                        else
                        {
                            ttIncl[c][r][nPrec][1] = new TagTreeDecoder(kend - kstart + 1, lend - lstart + 1);
                            ttMaxBP[c][r][nPrec][1] = new TagTreeDecoder(kend - kstart + 1, lend - lstart + 1);
                            CBlkCoordInfo[][] tmpArray2 = new CBlkCoordInfo[kend - kstart + 1][];
                            for (int i3 = 0; i3 < kend - kstart + 1; i3++)
                            {
                                tmpArray2[i3] = new CBlkCoordInfo[lend - lstart + 1];
                            }
                            ppinfo[c][r][nPrec].cblk[1] = tmpArray2;
                            ppinfo[c][r][nPrec].nblk[1] = (kend - kstart + 1) * (lend - lstart + 1);
                            for (int k = kstart; k <= kend; k++)
                            {
                                // Vertical cblks
                                for (int l = lstart; l <= lend; l++)
                                {
                                    // Horiz. cblks
                                    cb = new CBlkCoordInfo(k - k0, l - l0);
                                    if (l == l0)
                                    {
                                        cb.ulx = sb.ulx;
                                    }
                                    else
                                    {
                                        cb.ulx = sb.ulx + l * cw - (sb.ulcx - acb0x);
                                    }
                                    if (k == k0)
                                    {
                                        cb.uly = sb.uly;
                                    }
                                    else
                                    {
                                        cb.uly = sb.uly + k * ch - (sb.ulcy - acb0y);
                                    }
                                    tmp1 = acb0x + l * cw;
                                    tmp1 = (tmp1 > sb.ulcx) ? tmp1 : sb.ulcx;
                                    tmp2 = acb0x + (l + 1) * cw;
                                    tmp2 = (tmp2 > sb.ulcx + sb.w) ? sb.ulcx + sb.w : tmp2;
                                    cb.w = tmp2 - tmp1;
                                    tmp1 = acb0y + k * ch;
                                    tmp1 = (tmp1 > sb.ulcy) ? tmp1 : sb.ulcy;
                                    tmp2 = acb0y + (k + 1) * ch;
                                    tmp2 = (tmp2 > sb.ulcy + sb.h) ? sb.ulcy + sb.h : tmp2;
                                    cb.h = tmp2 - tmp1;
                                    ppinfo[c][r][nPrec].cblk[1][k - kstart][l - lstart] = cb;
                                }
                            }
                        }
                        acb0x = cb0x;
                        acb0y = 0;
                        p0x = acb0x + j * twoppx2;
                        p1x = p0x + twoppx2;
                        p0y = acb0y + i * twoppy2;
                        p1y = p0y + twoppy2;
                        sb = (SubbandSyn)root.getSubbandByIdx(r, 2);
                        s0x = (p0x < sb.ulcx) ? sb.ulcx : p0x;
                        s1x = (p1x > sb.ulcx + sb.w) ? sb.ulcx + sb.w : p1x;
                        s0y = (p0y < sb.ulcy) ? sb.ulcy : p0y;
                        s1y = (p1y > sb.ulcy + sb.h) ? sb.ulcy + sb.h : p1y;
                        cw = sb.nomCBlkW;
                        ch = sb.nomCBlkH;
                        k0 = (int)System.Math.Floor((sb.ulcy - acb0y) / (double)ch);
                        kstart = (int)System.Math.Floor((s0y - acb0y) / (double)ch);
                        kend = (int)System.Math.Floor((s1y - 1 - acb0y) / (double)ch);
                        l0 = (int)System.Math.Floor((sb.ulcx - acb0x) / (double)cw);
                        lstart = (int)System.Math.Floor((s0x - acb0x) / (double)cw);
                        lend = (int)System.Math.Floor((s1x - 1 - acb0x) / (double)cw);
                        if (s1x - s0x <= 0 || s1y - s0y <= 0)
                        {
                            ppinfo[c][r][nPrec].nblk[2] = 0;
                            ttIncl[c][r][nPrec][2] = new TagTreeDecoder(0, 0);
                            ttMaxBP[c][r][nPrec][2] = new TagTreeDecoder(0, 0);
                        }
                        else
                        {
                            ttIncl[c][r][nPrec][2] = new TagTreeDecoder(kend - kstart + 1, lend - lstart + 1);
                            ttMaxBP[c][r][nPrec][2] = new TagTreeDecoder(kend - kstart + 1, lend - lstart + 1);
                            CBlkCoordInfo[][] tmpArray3 = new CBlkCoordInfo[kend - kstart + 1][];
                            for (int i4 = 0; i4 < kend - kstart + 1; i4++)
                            {
                                tmpArray3[i4] = new CBlkCoordInfo[lend - lstart + 1];
                            }
                            ppinfo[c][r][nPrec].cblk[2] = tmpArray3;
                            ppinfo[c][r][nPrec].nblk[2] = (kend - kstart + 1) * (lend - lstart + 1);
                            for (int k = kstart; k <= kend; k++)
                            {
                                for (int l = lstart; l <= lend; l++)
                                {
                                    cb = new CBlkCoordInfo(k - k0, l - l0);
                                    if (l == l0)
                                    {
                                        cb.ulx = sb.ulx;
                                    }
                                    else
                                    {
                                        cb.ulx = sb.ulx + l * cw - (sb.ulcx - acb0x);
                                    }
                                    if (k == k0)
                                    {
                                        cb.uly = sb.uly;
                                    }
                                    else
                                    {
                                        cb.uly = sb.uly + k * ch - (sb.ulcy - acb0y);
                                    }
                                    tmp1 = acb0x + l * cw;
                                    tmp1 = (tmp1 > sb.ulcx) ? tmp1 : sb.ulcx;
                                    tmp2 = acb0x + (l + 1) * cw;
                                    tmp2 = (tmp2 > sb.ulcx + sb.w) ? sb.ulcx + sb.w : tmp2;
                                    cb.w = tmp2 - tmp1;
                                    tmp1 = acb0y + k * ch;
                                    tmp1 = (tmp1 > sb.ulcy) ? tmp1 : sb.ulcy;
                                    tmp2 = acb0y + (k + 1) * ch;
                                    tmp2 = (tmp2 > sb.ulcy + sb.h) ? sb.ulcy + sb.h : tmp2;
                                    cb.h = tmp2 - tmp1;
                                    ppinfo[c][r][nPrec].cblk[2][k - kstart][l - lstart] = cb;
                                }
                            }
                        }
                        acb0x = 0;
                        acb0y = 0;
                        p0x = acb0x + j * twoppx2;
                        p1x = p0x + twoppx2;
                        p0y = acb0y + i * twoppy2;
                        p1y = p0y + twoppy2;
                        sb = (SubbandSyn)root.getSubbandByIdx(r, 3);
                        s0x = (p0x < sb.ulcx) ? sb.ulcx : p0x;
                        s1x = (p1x > sb.ulcx + sb.w) ? sb.ulcx + sb.w : p1x;
                        s0y = (p0y < sb.ulcy) ? sb.ulcy : p0y;
                        s1y = (p1y > sb.ulcy + sb.h) ? sb.ulcy + sb.h : p1y;
                        cw = sb.nomCBlkW;
                        ch = sb.nomCBlkH;
                        k0 = (int)System.Math.Floor((sb.ulcy - acb0y) / (double)ch);
                        kstart = (int)System.Math.Floor((s0y - acb0y) / (double)ch);
                        kend = (int)System.Math.Floor((s1y - 1 - acb0y) / (double)ch);
                        l0 = (int)System.Math.Floor((sb.ulcx - acb0x) / (double)cw);
                        lstart = (int)System.Math.Floor((s0x - acb0x) / (double)cw);
                        lend = (int)System.Math.Floor((s1x - 1 - acb0x) / (double)cw);
                        if (s1x - s0x <= 0 || s1y - s0y <= 0)
                        {
                            ppinfo[c][r][nPrec].nblk[3] = 0;
                            ttIncl[c][r][nPrec][3] = new TagTreeDecoder(0, 0);
                            ttMaxBP[c][r][nPrec][3] = new TagTreeDecoder(0, 0);
                        }
                        else
                        {
                            ttIncl[c][r][nPrec][3] = new TagTreeDecoder(kend - kstart + 1, lend - lstart + 1);
                            ttMaxBP[c][r][nPrec][3] = new TagTreeDecoder(kend - kstart + 1, lend - lstart + 1);
                            CBlkCoordInfo[][] tmpArray4 = new CBlkCoordInfo[kend - kstart + 1][];
                            for (int i5 = 0; i5 < kend - kstart + 1; i5++)
                            {
                                tmpArray4[i5] = new CBlkCoordInfo[lend - lstart + 1];
                            }
                            ppinfo[c][r][nPrec].cblk[3] = tmpArray4;
                            ppinfo[c][r][nPrec].nblk[3] = (kend - kstart + 1) * (lend - lstart + 1);
                            for (int k = kstart; k <= kend; k++)
                            {
                                for (int l = lstart; l <= lend; l++)
                                {
                                    cb = new CBlkCoordInfo(k - k0, l - l0);
                                    if (l == l0)
                                    {
                                        cb.ulx = sb.ulx;
                                    }
                                    else
                                    {
                                        cb.ulx = sb.ulx + l * cw - (sb.ulcx - acb0x);
                                    }
                                    if (k == k0)
                                    {
                                        cb.uly = sb.uly;
                                    }
                                    else
                                    {
                                        cb.uly = sb.uly + k * ch - (sb.ulcy - acb0y);
                                    }
                                    tmp1 = acb0x + l * cw;
                                    tmp1 = (tmp1 > sb.ulcx) ? tmp1 : sb.ulcx;
                                    tmp2 = acb0x + (l + 1) * cw;
                                    tmp2 = (tmp2 > sb.ulcx + sb.w) ? sb.ulcx + sb.w : tmp2;
                                    cb.w = tmp2 - tmp1;
                                    tmp1 = acb0y + k * ch;
                                    tmp1 = (tmp1 > sb.ulcy) ? tmp1 : sb.ulcy;
                                    tmp2 = acb0y + (k + 1) * ch;
                                    tmp2 = (tmp2 > sb.ulcy + sb.h) ? sb.ulcy + sb.h : tmp2;
                                    cb.h = tmp2 - tmp1;
                                    ppinfo[c][r][nPrec].cblk[3][k - kstart][l - lstart] = cb;
                                }
                            }
                        }
                    }
                }
            }
        }
        public virtual int getNumPrecinct(int c, int r)
        {
            return numPrec[c][r].x * numPrec[c][r].y;
        }
        public virtual bool readPktHead(int l, int r, int c, int p, CBlkInfo[][][] cbI, int[] nb)
        {
            CBlkInfo ccb;
            int nSeg;
            int cbLen;
            int ltp;
            int passtype;
            TagTreeDecoder tdIncl, tdBD;
            int tmp, tmp2, totnewtp, lblockCur, tpidx;
            int sumtotnewtp = 0;
            JPXImageCoordinates cbc;
            int startPktHead = ehs.Pos;
            if (startPktHead >= ehs.length())
            {
                return true;
            }
            int tIdx = src.TileIdx;
            PktHeaderBitReader bin;
            int mend, nend;
            int b;
            SubbandSyn sb;
            SubbandSyn root = src.getSynSubbandTree(tIdx, c);
            if (pph)
            {
                bin = new PktHeaderBitReader(pphbais);
            }
            else
            {
                bin = this.bin;
            }
            int mins = (r == 0) ? 0 : 1;
            int maxs = (r == 0) ? 1 : 4;
            bool precFound = false;
            for (int s = mins; s < maxs; s++)
            {
                if (p < ppinfo[c][r].Length)
                {
                    precFound = true;
                }
            }
            if (!precFound)
            {
                return false;
            }
            PrecInfo prec = ppinfo[c][r][p];
            bin.sync();
            if (bin.readBit() == 0)
            {
                cblks = new ArrayList[maxs + 1];
                for (int s = mins; s < maxs; s++)
                {
                    cblks[s] =new ArrayList(10);
                }
                pktIdx++;
                if (isTruncMode && maxCB == -1)
                {
                    tmp = ehs.Pos - startPktHead;
                    if (tmp > nb[tIdx])
                    {
                        nb[tIdx] = 0;
                        return true;
                    }
                    else
                    {
                        nb[tIdx] -= tmp;
                    }
                }
                if (ephUsed)
                {
                    readEPHMarker(bin);
                }
                return false;
            }
            if (cblks == null || cblks.Length < maxs + 1)
            {
                cblks = new ArrayList[maxs + 1];
            }
            for (int s = mins; s < maxs; s++)
            {
                if (cblks[s] == null)
                {
                    cblks[s] = new ArrayList(10);
                }
                else
                {
                    cblks[s].Clear();
                }
                sb = (SubbandSyn)root.getSubbandByIdx(r, s);
                if (prec.nblk[s] == 0)
                {
                    continue;
                }
                tdIncl = ttIncl[c][r][p][s];
                tdBD = ttMaxBP[c][r][p][s];
                mend = (prec.cblk[s] == null) ? 0 : prec.cblk[s].Length;
                for (int m = 0; m < mend; m++)
                {
                    nend = (prec.cblk[s][m] == null) ? 0 : prec.cblk[s][m].Length;
                    for (int n = 0; n < nend; n++)
                    {
                        cbc = prec.cblk[s][m][n].idx;
                        b = cbc.x + cbc.y * sb.numCb.x;
                        ccb = cbI[s][cbc.y][cbc.x];
                        try
                        {
                            if (ccb == null || ccb.ctp == 0)
                            {
                                if (ccb == null)
                                {
                                    ccb = cbI[s][cbc.y][cbc.x] = new CBlkInfo(prec.cblk[s][m][n].ulx, prec.cblk[s][m][n].uly, prec.cblk[s][m][n].w, prec.cblk[s][m][n].h, nl);
                                }
                                ccb.pktIdx[l] = pktIdx;
                                tmp = tdIncl.update(m, n, l + 1, bin);
                                if (tmp > l)
                                {
                                    continue;
                                }
                                tmp = 1;
                                for (tmp2 = 1; tmp >= tmp2; tmp2++)
                                {
                                    tmp = tdBD.update(m, n, tmp2, bin);
                                }
                                ccb.msbSkipped = tmp2 - 2;
                                totnewtp = 1;
                                ccb.addNTP(l, 0);
                                ncb++;
                                if (maxCB != -1 && !ncbQuit && ncb == maxCB)
                                {
                                    ncbQuit = true;
                                    tQuit = tIdx;
                                    cQuit = c;
                                    sQuit = s;
                                    rQuit = r;
                                    xQuit = cbc.x;
                                    yQuit = cbc.y;
                                }
                            }
                            else
                            {
                                ccb.pktIdx[l] = pktIdx;
                                if (bin.readBit() != 1)
                                {
                                    continue;
                                }
                                totnewtp = 1;
                            }
                            if (bin.readBit() == 1)
                            {
                                totnewtp++;
                                if (bin.readBit() == 1)
                                {
                                    totnewtp++;
                                    tmp = bin.readBits(2);
                                    totnewtp += tmp;
                                    if (tmp == 0x3)
                                    {
                                        tmp = bin.readBits(5);
                                        totnewtp += tmp;
                                        if (tmp == 0x1F)
                                        {
                                            totnewtp += bin.readBits(7);
                                        }
                                    }
                                }
                            }
                            ccb.addNTP(l, totnewtp);
                            sumtotnewtp += totnewtp;
                            cblks[s].Add(prec.cblk[s][m][n]);
                            int options = ((System.Int32)decSpec.ecopts.getTileCompVal(tIdx, c));
                            if ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_TERM_PASS) != 0)
                            {
                                nSeg = totnewtp;
                            }
                            else if ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_BYPASS) != 0)
                            {
                                if (ccb.ctp <= Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.FIRST_BYPASS_PASS_IDX)
                                {
                                    nSeg = 1;
                                }
                                else
                                {
                                    nSeg = 1;
                                    for (tpidx = ccb.ctp - totnewtp; tpidx < ccb.ctp - 1; tpidx++)
                                    {
                                        if (tpidx >= Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.FIRST_BYPASS_PASS_IDX - 1)
                                        {
                                            passtype = (tpidx + Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.NUM_EMPTY_PASSES_IN_MS_BP) % Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.NUM_PASSES;
                                            if (passtype == 1 || passtype == 2)
                                            {
                                                nSeg++;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                nSeg = 1;
                            }
                            while (bin.readBit() != 0)
                            {
                                lblock[c][r][s][cbc.y][cbc.x]++;
                            }
                            if (nSeg == 1)
                            {
                                cbLen = bin.readBits(lblock[c][r][s][cbc.y][cbc.x] + MathUtil.log2(totnewtp));
                            }
                            else
                            {
                                ccb.segLen[l] = new int[nSeg];
                                cbLen = 0;
                                int j;
                                if ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_TERM_PASS) != 0)
                                {
                                    for (tpidx = ccb.ctp - totnewtp, j = 0; tpidx < ccb.ctp; tpidx++, j++)
                                    {
                                        lblockCur = lblock[c][r][s][cbc.y][cbc.x];
                                        tmp = bin.readBits(lblockCur);
                                        ccb.segLen[l][j] = tmp;
                                        cbLen += tmp;
                                    }
                                }
                                else
                                {
                                    ltp = ccb.ctp - totnewtp - 1;
                                    for (tpidx = ccb.ctp - totnewtp, j = 0; tpidx < ccb.ctp - 1; tpidx++)
                                    {
                                        if (tpidx >= Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.FIRST_BYPASS_PASS_IDX - 1)
                                        {
                                            passtype = (tpidx + Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.NUM_EMPTY_PASSES_IN_MS_BP) % Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.NUM_PASSES;
                                            if (passtype == 0)
                                                continue;
                                            lblockCur = lblock[c][r][s][cbc.y][cbc.x];
                                            tmp = bin.readBits(lblockCur + MathUtil.log2(tpidx - ltp));
                                            ccb.segLen[l][j] = tmp;
                                            cbLen += tmp;
                                            ltp = tpidx;
                                            j++;
                                        }
                                    }
                                    lblockCur = lblock[c][r][s][cbc.y][cbc.x];
                                    tmp = bin.readBits(lblockCur + MathUtil.log2(tpidx - ltp));
                                    cbLen += tmp;
                                    ccb.segLen[l][j] = tmp;
                                }
                            }
                            ccb.len[l] = cbLen;
                            if (isTruncMode && maxCB == -1)
                            {
                                tmp = ehs.Pos - startPktHead;
                                if (tmp > nb[tIdx])
                                {
                                    nb[tIdx] = 0;
                                    if (l == 0)
                                    {
                                        cbI[s][cbc.y][cbc.x] = null;
                                    }
                                    else
                                    {
                                        ccb.off[l] = ccb.len[l] = 0;
                                        ccb.ctp -= ccb.ntp[l];
                                        ccb.ntp[l] = 0;
                                        ccb.pktIdx[l] = -1;
                                    }
                                    return true;
                                }
                            }
                        }
                        catch (System.IO.EndOfStreamException)
                        {
                            if (l == 0)
                            {
                                cbI[s][cbc.y][cbc.x] = null;
                            }
                            else
                            {
                                ccb.off[l] = ccb.len[l] = 0;
                                ccb.ctp -= ccb.ntp[l];
                                ccb.ntp[l] = 0;
                                ccb.pktIdx[l] = -1;
                            }
                            return true;
                        }
                    }
                }
            }
            if (ephUsed)
            {
                readEPHMarker(bin);
            }
            pktIdx++;
            if (isTruncMode && maxCB == -1)
            {
                tmp = ehs.Pos - startPktHead;
                if (tmp > nb[tIdx])
                {
                    nb[tIdx] = 0;
                    return true;
                }
                else
                {
                    nb[tIdx] -= tmp;
                }
            }
            return false;
        }
        public virtual bool readPktBody(int l, int r, int c, int p, CBlkInfo[][][] cbI, int[] nb)
        {
            int curOff = ehs.Pos;
            CBlkInfo ccb;
            bool stopRead = false;
            int tIdx = src.TileIdx;
            JPXImageCoordinates cbc;
            bool precFound = false;
            int mins = (r == 0) ? 0 : 1;
            int maxs = (r == 0) ? 1 : 4;
            for (int s = mins; s < maxs; s++)
            {
                if (p < ppinfo[c][r].Length)
                {
                    precFound = true;
                }
            }
            if (!precFound)
            {
                return false;
            }
            for (int s = mins; s < maxs; s++)
            {
                for (int numCB = 0; numCB < cblks[s].Count; numCB++)
                {
                    cbc = ((CBlkCoordInfo)cblks[s][numCB]).idx;
                    ccb = cbI[s][cbc.y][cbc.x];
                    ccb.off[l] = curOff;
                    curOff += ccb.len[l];
                    try
                    {
                        ehs.seek(curOff);
                    }
                    catch (System.IO.EndOfStreamException)
                    {
                        if (l == 0)
                        {
                            cbI[s][cbc.y][cbc.x] = null;
                        }
                        else
                        {
                            ccb.off[l] = ccb.len[l] = 0;
                            ccb.ctp -= ccb.ntp[l];
                            ccb.ntp[l] = 0;
                            ccb.pktIdx[l] = -1;
                        }
                        throw new System.IO.EndOfStreamException();
                    }
                    if (isTruncMode)
                    {
                        if (stopRead || ccb.len[l] > nb[tIdx])
                        {
                            if (l == 0)
                            {
                                cbI[s][cbc.y][cbc.x] = null;
                            }
                            else
                            {
                                ccb.off[l] = ccb.len[l] = 0;
                                ccb.ctp -= ccb.ntp[l];
                                ccb.ntp[l] = 0;
                                ccb.pktIdx[l] = -1;
                            }
                            stopRead = true;
                        }
                        if (!stopRead)
                        {
                            nb[tIdx] -= ccb.len[l];
                        }
                    }
                    if (ncbQuit && r == rQuit && s == sQuit && cbc.x == xQuit && cbc.y == yQuit && tIdx == tQuit && c == cQuit)
                    {
                        cbI[s][cbc.y][cbc.x] = null;
                        stopRead = true;
                    }
                }
            }
            ehs.seek(curOff);
            if (stopRead)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int getPPX(int t, int c, int r)
        {
            return decSpec.pss.getPPX(t, c, r);
        }
        public int getPPY(int t, int c, int rl)
        {
            return decSpec.pss.getPPY(t, c, rl);
        }
        public virtual bool readSOPMarker(int[] nBytes, int p, int c, int r)
        {
            int val;
            byte[] sopArray = new byte[6];
            int tIdx = src.TileIdx;
            int mins = (r == 0) ? 0 : 1;
            int maxs = (r == 0) ? 1 : 4;
            bool precFound = false;
            for (int s = mins; s < maxs; s++)
            {
                if (p < ppinfo[c][r].Length)
                {
                    precFound = true;
                }
            }
            if (!precFound)
            {
                return false;
            }
            if (!sopUsed)
            {
                return false;
            }
            int pos = ehs.Pos;
            if ((short)((ehs.read() << 8) | ehs.read()) != Syncfusion.Pdf.JPEG2000.codestream.Markers.SOP)
            {
                ehs.seek(pos);
                return false;
            }
            ehs.seek(pos);
            if (nBytes[tIdx] < 6)
            {
                return true;
            }
            nBytes[tIdx] -= 6;
            ehs.readFully(sopArray, 0, Syncfusion.Pdf.JPEG2000.codestream.Markers.SOP_LENGTH);
            val = sopArray[0];
            val <<= 8;
            val |= sopArray[1];
            if (val != Syncfusion.Pdf.JPEG2000.codestream.Markers.SOP)
            {
                //throw new System.ApplicationException("Corrupted Bitstream: Could not parse SOP " + "marker !");
            }
            val = (sopArray[2] & 0xff);
            val <<= 8;
            val |= (sopArray[3] & 0xff);
            if (val != 4)
            {
                //throw new System.ApplicationException("Corrupted Bitstream: Corrupted SOP marker !");
            }
            val = (sopArray[4] & 0xff);
            val <<= 8;
            val |= (sopArray[5] & 0xff);
            if (!pph && val != pktIdx)
            {
                //throw new System.ApplicationException("Corrupted Bitstream: SOP marker out of " + "sequence !");
            }
            if (pph && val != pktIdx - 1)
            {
                //throw new System.ApplicationException("Corrupted Bitstream: SOP marker out of " + "sequence !");
            }
            return false;
        }
        public virtual void readEPHMarker(PktHeaderBitReader bin)
        {
            int val;
            byte[] ephArray = new byte[2];
            if (bin.usebais)
            {
                bin.bais.Read(ephArray, 0, Syncfusion.Pdf.JPEG2000.codestream.Markers.EPH_LENGTH);
            }
            else
            {
                bin.in_Renamed.readFully(ephArray, 0, Syncfusion.Pdf.JPEG2000.codestream.Markers.EPH_LENGTH);
            }
            val = ephArray[0];
            val <<= 8;
            val |= ephArray[1];
            if (val != Syncfusion.Pdf.JPEG2000.codestream.Markers.EPH)
            {
                //throw new System.ApplicationException("Corrupted Bitstream: Could not parse EPH " + "marker ! ");
            }
        }
        public virtual PrecInfo getPrecInfo(int c, int r, int p)
        {
            return ppinfo[c][r][p];
        }
    }
}
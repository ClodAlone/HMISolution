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
using Syncfusion.Pdf.JPEG2000.decoder;
using Syncfusion.Pdf.JPEG2000.image;
using Syncfusion.Pdf.JPEG2000.util;
using Syncfusion.Pdf.JPEG2000.io;
using System.Collections.Generic;
using System;
using HashTable = System.Collections.Generic.Dictionary<object, object>;
using ArrayList = System.Collections.Generic.List<object>;
namespace Syncfusion.Pdf.JPEG2000.codestream.reader
{
    internal class FileBitstreamReaderAgent : BitstreamReader
    {
        virtual public CBlkInfo[][][][][] CBlkInfo
        {
            get
            {
                return cbI;
            }
        }
        private bool isPsotEqualsZero = true;
        internal PktDecoder pktDec;
        private JPXParameters pl;
        private JPXRandomAccessStream in_Renamed;
        private int[][] firstPackOff;
        public virtual int getNumTileParts(int t)
        {
            if (firstPackOff == null || firstPackOff[t] == null)
            {
                //throw new System.ApplicationException("Tile " + t + " not found in input codestream.");
            }
            return firstPackOff[t].Length;
        }
        private int[] nBytes;
        private bool printInfo = false;
        private int[] baknBytes;
        private int[][] tilePartLen;
        private int[] totTileLen;
        private int[] totTileHeadLen;
        private int firstTilePartHeadLen;
        private double totAllTileLen;
        private int mainHeadLen;
        private int headLen = 0;
        private int[][] tilePartHeadLen;
        private List<Object> pktHL;
        private bool isTruncMode;
        private int remainingTileParts;
        private int[] tilePartsRead;
        private int totTilePartsRead = 0;
        private int[] tileParts;
        private int curTilePart;
        private int[][] tilePartNum;
        private bool isEOCFound = false;
        private HeaderInformation hi;
        private CBlkInfo[][][][][] cbI;
        private int lQuit;
        private bool usePOCQuit = false;
        internal FileBitstreamReaderAgent(HeaderDecoder hd, JPXRandomAccessStream ehs, DecodeHelper decSpec, JPXParameters pl, bool cdstrInfo, HeaderInformation hi)
            : base(hd, decSpec)
        {
            this.pl = pl;
            this.printInfo = cdstrInfo;
            this.hi = hi;
            System.String strInfo = "Codestream elements information in bytes " + "(offset, total length, header length):\n\n";
            usePOCQuit = pl.getBooleanParameter("poc_quit");
            bool rateInBytes;
            bool parsing = pl.getBooleanParameter("parsing");
            try
            {
                trate = pl.getFloatParameter("rate");
                if (trate == -1)
                {
                    trate = System.Single.MaxValue;
                }
            }
            catch (System.FormatException)
            {
                //throw new System.ApplicationException("Invalid value in 'rate' option: " + pl.getParameter("rate"));
            }
            catch (System.ArgumentException)
            {
                //throw new System.ApplicationException("'rate' option is missing");
            }
            try
            {
                tnbytes = pl.getIntParameter("nbytes");
            }
            catch (System.FormatException)
            {
                //throw new System.ApplicationException("Invalid value in 'nbytes' option: " + pl.getParameter("nbytes"));
            }
            catch (System.ArgumentException)
            {
                //throw new System.ApplicationException("'nbytes' option is missing");
            }
            JPXParameters defaults = pl.DefaultParameterList;
            if (tnbytes != defaults.getFloatParameter("nbytes"))
            {
                rateInBytes = true;
            }
            else
            {
                rateInBytes = false;
            }
            if (rateInBytes)
            {
                trate = tnbytes * 8f / hd.MaxCompImgWidth / hd.MaxCompImgHeight;
            }
            else
            {
                tnbytes = (int)(trate * hd.MaxCompImgWidth * hd.MaxCompImgHeight) / 8;
                if (tnbytes < 0) tnbytes = int.MaxValue;
            }
            isTruncMode = !pl.getBooleanParameter("parsing");
            int ncbQuit=0;
            try
            {
                ncbQuit = pl.getIntParameter("ncb_quit");
            }
            catch (System.FormatException)
            {
                //throw new System.ApplicationException("Invalid value in 'ncb_quit' option: " + pl.getParameter("ncb_quit"));
            }
            catch (System.ArgumentException)
            {
                //throw new System.ApplicationException("'ncb_quit' option is missing");
            }
            if (ncbQuit != -1 && !isTruncMode)
            {
                //throw new System.ApplicationException("Cannot use -parsing and -ncb_quit condition at " + "the same time.");
            }
            try
            {
                lQuit = pl.getIntParameter("l_quit");
            }
            catch (System.FormatException)
            {
                //throw new System.ApplicationException("Invalid value in 'l_quit' option: " + pl.getParameter("l_quit"));
            }
            catch (System.ArgumentException)
            {
                //throw new System.ApplicationException("'l_quit' option is missing");
            }
            in_Renamed = ehs;
            pktDec = new PktDecoder(decSpec, hd, ehs, this, isTruncMode, ncbQuit);
            tileParts = new int[nt];
            totTileLen = new int[nt];
            tilePartLen = new int[nt][];
            tilePartNum = new int[nt][];
            firstPackOff = new int[nt][];
            tilePartsRead = new int[nt];
            totTileHeadLen = new int[nt];
            tilePartHeadLen = new int[nt][];
            nBytes = new int[nt];
            baknBytes = new int[nt];
            hd.nTileParts = new int[nt];
            int t = 0, pos, tp = 0, tptot = 0;
            int cdstreamStart = hd.mainHeadOff;
            mainHeadLen = in_Renamed.Pos - cdstreamStart;
            headLen = mainHeadLen;
            if (ncbQuit == -1)
            {
                anbytes = mainHeadLen;
            }
            else
            {
                anbytes = 0;
            }
            strInfo += ("Main header length    : " + cdstreamStart + ", " + mainHeadLen + ", " + mainHeadLen + "\n");
            if (anbytes > tnbytes)
            {
                //throw new System.ApplicationException("Requested bitrate is too small.");
            }
            int tilePartStart;
            int mdl;
            totAllTileLen = 0;
            remainingTileParts = nt;
            int maxTP = nt;
            try
            {
                while (remainingTileParts != 0)
                {
                    tilePartStart = in_Renamed.Pos;
                    try
                    {
                        t = readTilePartHeader();
                        if (isEOCFound)
                        {
                            break;
                        }
                        tp = tilePartsRead[t];
                        if (isPsotEqualsZero)
                        {
                            tilePartLen[t][tp] = in_Renamed.length() - 2 - tilePartStart;
                        }
                    }
                    catch (System.IO.EndOfStreamException e)
                    {
                        firstPackOff[t][tp] = in_Renamed.length();
                        throw e;
                    }
                    pos = in_Renamed.Pos;
                    if (isTruncMode && ncbQuit == -1)
                    {
                        if ((pos - cdstreamStart) > tnbytes)
                        {
                            firstPackOff[t][tp] = in_Renamed.length();
                            break;
                        }
                    }
                    firstPackOff[t][tp] = pos;
                    tilePartHeadLen[t][tp] = (pos - tilePartStart);
                    strInfo += ("Tile-part " + tp + " of tile " + t + " : " + tilePartStart + ", " + tilePartLen[t][tp] + ", " + tilePartHeadLen[t][tp] + "\n");
                    totTileLen[t] += tilePartLen[t][tp];
                    totTileHeadLen[t] += tilePartHeadLen[t][tp];
                    totAllTileLen += tilePartLen[t][tp];
                    if (isTruncMode)
                    {
                        if (anbytes + tilePartLen[t][tp] > tnbytes)
                        {
                            anbytes += tilePartHeadLen[t][tp];
                            headLen += tilePartHeadLen[t][tp];
                            nBytes[t] += (tnbytes - anbytes);
                            break;
                        }
                        else
                        {
                            anbytes += tilePartHeadLen[t][tp];
                            headLen += tilePartHeadLen[t][tp];
                            nBytes[t] += (tilePartLen[t][tp] - tilePartHeadLen[t][tp]);
                        }
                    }
                    else
                    {
                        if (anbytes + tilePartHeadLen[t][tp] > tnbytes)
                        {
                            break;
                        }
                        else
                        {
                            anbytes += tilePartHeadLen[t][tp];
                            headLen += tilePartHeadLen[t][tp];
                        }
                    }
                    if (tptot == 0)
                        firstTilePartHeadLen = tilePartHeadLen[t][tp];
                    tilePartsRead[t]++;
                    in_Renamed.seek(tilePartStart + tilePartLen[t][tp]);
                    remainingTileParts--;
                    maxTP--;
                    tptot++;
                    if (isPsotEqualsZero)
                    {
                        break;
                    }
                }
            }
            catch (System.IO.EndOfStreamException)
            {
                int fileLen = in_Renamed.length();
                if (fileLen < tnbytes)
                {
                    tnbytes = fileLen;
                    trate = tnbytes * 8f / hd.MaxCompImgWidth / hd.MaxCompImgHeight;
                }
                if (!isTruncMode)
                {
                    allocateRate();
                }
                if (pl.getParameter("res") == null)
                {
                    targetRes = decSpec.dls.Min;
                }
                else
                {
                    try
                    {
                        targetRes = pl.getIntParameter("res");
                        if (targetRes < 0)
                        {
                            throw new System.ArgumentException("Specified negative " + "resolution level " + "index: " + targetRes);
                        }
                    }
                    catch (System.FormatException)
                    {
                        throw new System.ArgumentException("Invalid resolution level " + "index ('-res' option) " + pl.getParameter("res"));
                    }
                }
                mdl = decSpec.dls.Min;
                if (targetRes > mdl)
                {
                    targetRes = mdl;
                }
                for (int tIdx = 0; tIdx < nt; tIdx++)
                {
                    baknBytes[tIdx] = nBytes[tIdx];
                }
                return;
            }
            remainingTileParts = 0;
            if (pl.getParameter("res") == null)
            {
                targetRes = decSpec.dls.Min;
            }
            else
            {
                try
                {
                    targetRes = pl.getIntParameter("res");
                    if (targetRes < 0)
                    {
                        throw new System.ArgumentException("Specified negative " + "resolution level index: " + targetRes);
                    }
                }
                catch (System.FormatException)
                {
                    throw new System.ArgumentException("Invalid resolution level " + "index ('-res' option) " + pl.getParameter("res"));
                }
            }
            mdl = decSpec.dls.Min;
            if (targetRes > mdl)
            {
                targetRes = mdl;
            }
            if (!isEOCFound && !isPsotEqualsZero)
            {
            }
            if (!isTruncMode)
            {
                allocateRate();
            }
            else
            {
                if (in_Renamed.Pos >= tnbytes)
                    anbytes += 2;
            }
            for (int tIdx = 0; tIdx < nt; tIdx++)
            {
                baknBytes[tIdx] = nBytes[tIdx];
            }
        }
        private void allocateRate()
        {
            int stopOff = tnbytes;
            anbytes += 2;
            if (anbytes > stopOff)
            {
                //throw new System.ApplicationException("Requested bitrate is too small for parsing");
            }
            int rem = stopOff - anbytes;
            int totnByte = rem;
            for (int t = nt - 1; t > 0; t--)
            {
                rem -= (nBytes[t] = (int)(totnByte * (totTileLen[t] / totAllTileLen)));
            }
            nBytes[0] = rem;
        }
        private int readTilePartHeader()
        {
            HeaderInformation.SOT ms = hi.NewSOT;
            short marker = in_Renamed.readShort();
            if (marker != Syncfusion.Pdf.JPEG2000.codestream.Markers.SOT)
            {
                if (marker == Syncfusion.Pdf.JPEG2000.codestream.Markers.EOC)
                {
                    isEOCFound = true;
                    return -1;
                }
                else
                {
                    throw new System.Exception();
                }
            }
            isEOCFound = false;
            int lsot = in_Renamed.readUnsignedShort();
            ms.lsot = lsot;
            if (lsot != 10)
                throw new System.Exception();
            int tile = in_Renamed.readUnsignedShort();
            ms.isot = tile;
            if (tile > 65534)
            {
                throw new System.Exception();
            }
            int psot = in_Renamed.readInt();
            ms.psot = psot;
            isPsotEqualsZero = (psot != 0) ? false : true;
            if (psot < 0)
            {
                throw new System.ArgumentException("Maximum tile length exceeded");
            }
            int tilePart = in_Renamed.read();
            ms.tpsot = tilePart;
            if (tilePart != tilePartsRead[tile] || tilePart < 0 || tilePart > 254)
            {
                throw new System.Exception();
            }
            int nrOfTileParts = in_Renamed.read();
            ms.tnsot = nrOfTileParts;
            hi.sotValue["t" + tile + "_tp" + tilePart] = ms;
            if (nrOfTileParts == 0)
            {
                int nExtraTp;
                if (tileParts[tile] == 0 || tileParts[tile] == tilePartLen.Length)
                {
                    nExtraTp = 2;
                    remainingTileParts += 1;
                }
                else
                {
                    nExtraTp = 1;
                }
                tileParts[tile] += nExtraTp;
                nrOfTileParts = tileParts[tile];
                int[] tmpA = tilePartLen[tile];
                tilePartLen[tile] = new int[nrOfTileParts];
                for (int i = 0; i < nrOfTileParts - nExtraTp; i++)
                {
                    tilePartLen[tile][i] = tmpA[i];
                }
                tmpA = tilePartNum[tile];
                tilePartNum[tile] = new int[nrOfTileParts];
                for (int i = 0; i < nrOfTileParts - nExtraTp; i++)
                {
                    tilePartNum[tile][i] = tmpA[i];
                }
                tmpA = firstPackOff[tile];
                firstPackOff[tile] = new int[nrOfTileParts];
                for (int i = 0; i < nrOfTileParts - nExtraTp; i++)
                {
                    firstPackOff[tile][i] = tmpA[i];
                }
                tmpA = tilePartHeadLen[tile];
                tilePartHeadLen[tile] = new int[nrOfTileParts];
                for (int i = 0; i < nrOfTileParts - nExtraTp; i++)
                {
                    tilePartHeadLen[tile][i] = tmpA[i];
                }
            }
            else
            {
                if (tileParts[tile] == 0)
                {
                    remainingTileParts += nrOfTileParts - 1;
                    tileParts[tile] = nrOfTileParts;
                    tilePartLen[tile] = new int[nrOfTileParts];
                    tilePartNum[tile] = new int[nrOfTileParts];
                    firstPackOff[tile] = new int[nrOfTileParts];
                    tilePartHeadLen[tile] = new int[nrOfTileParts];
                }
                else if (tileParts[tile] > nrOfTileParts)
                {
                    throw new System.Exception();
                }
                else
                {
                    remainingTileParts += nrOfTileParts - tileParts[tile];
                    if (tileParts[tile] != nrOfTileParts)
                    {
                        int[] tmpA = tilePartLen[tile];
                        tilePartLen[tile] = new int[nrOfTileParts];
                        for (int i = 0; i < tileParts[tile] - 1; i++)
                        {
                            tilePartLen[tile][i] = tmpA[i];
                        }
                        tmpA = tilePartNum[tile];
                        tilePartNum[tile] = new int[nrOfTileParts];
                        for (int i = 0; i < tileParts[tile] - 1; i++)
                        {
                            tilePartNum[tile][i] = tmpA[i];
                        }
                        tmpA = firstPackOff[tile];
                        firstPackOff[tile] = new int[nrOfTileParts];
                        for (int i = 0; i < tileParts[tile] - 1; i++)
                        {
                            firstPackOff[tile][i] = tmpA[i];
                        }
                        tmpA = tilePartHeadLen[tile];
                        tilePartHeadLen[tile] = new int[nrOfTileParts];
                        for (int i = 0; i < tileParts[tile] - 1; i++)
                        {
                            tilePartHeadLen[tile][i] = tmpA[i];
                        }
                    }
                }
            }
            hd.resetHeaderMarkers();
            hd.nTileParts[tile] = nrOfTileParts;
            do
            {
                hd.extractTilePartMarkSeg(in_Renamed.readShort(), in_Renamed, tile, tilePart);
            }
            while ((hd.NumFoundMarkSeg & Syncfusion.Pdf.JPEG2000.codestream.reader.HeaderDecoder.SOD_FOUND) == 0);
            hd.readFoundTilePartMarkSeg(tile, tilePart);
            tilePartLen[tile][tilePart] = psot;
            tilePartNum[tile][tilePart] = totTilePartsRead;
            totTilePartsRead++;
            hd.TileOfTileParts = tile;
            return tile;
        }
        private bool readLyResCompPos(int[][] lys, int lye, int ress, int rese, int comps, int compe)
        {
            int minlys = 10000;
            for (int c = comps; c < compe; c++)
            {
                if (c >= mdl.Length)
                    continue;
                for (int r = ress; r < rese; r++)
                {
                    if (lys[c] != null && r < lys[c].Length && lys[c][r] < minlys)
                    {
                        minlys = lys[c][r];
                    }
                }
            }
            int t = TileIdx;
            int start;
            bool status = false;
            int lastByte = firstPackOff[t][curTilePart] + tilePartLen[t][curTilePart] - 1 - tilePartHeadLen[t][curTilePart];
            int numLayers = ((System.Int32)decSpec.nls.getTileDef(t));
            int nPrec = 1;
            int hlen, plen;
            System.String strInfo = "Tile " + TileIdx + " (tile-part:" + curTilePart + "): offset, length, header length\n"; ;
            bool pph = false;
            if (((System.Boolean)decSpec.pphs.getTileDef(t)))
            {
                pph = true;
            }
            for (int l = minlys; l < lye; l++)
            {
                for (int r = ress; r < rese; r++)
                {
                    for (int c = comps; c < compe; c++)
                    {
                        if (c >= mdl.Length)
                            continue;
                        if (r >= lys[c].Length)
                            continue;
                        if (r > mdl[c])
                            continue;
                        if (l < lys[c][r] || l >= numLayers)
                            continue;
                        nPrec = pktDec.getNumPrecinct(c, r);
                        for (int p = 0; p < nPrec; p++)
                        {
                            start = in_Renamed.Pos;
                            if (pph)
                            {
                                pktDec.readPktHead(l, r, c, p, cbI[c][r], nBytes);
                            }
                            if (start > lastByte && curTilePart < firstPackOff[t].Length - 1)
                            {
                                curTilePart++;
                                in_Renamed.seek(firstPackOff[t][curTilePart]);
                                lastByte = in_Renamed.Pos + tilePartLen[t][curTilePart] - 1 - tilePartHeadLen[t][curTilePart];
                            }
                            status = pktDec.readSOPMarker(nBytes, p, c, r);
                            if (status)
                            {
                                return true;
                            }
                            if (!pph)
                            {
                                status = pktDec.readPktHead(l, r, c, p, cbI[c][r], nBytes);
                            }
                            if (status)
                            {
                                return true;
                            }
                            hlen = in_Renamed.Pos - start;
                            pktHL.Add((System.Int32)hlen);
                            status = pktDec.readPktBody(l, r, c, p, cbI[c][r], nBytes);
                            plen = in_Renamed.Pos - start;
                            strInfo += (" Pkt l=" + l + ",r=" + r + ",c=" + c + ",p=" + p + ": " + start + ", " + plen + ", " + hlen + "\n");
                            if (status)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        private bool readResLyCompPos(int[][] lys, int lye, int ress, int rese, int comps, int compe)
        {
            int t = TileIdx;
            bool status = false;
            int lastByte = firstPackOff[t][curTilePart] + tilePartLen[t][curTilePart] - 1 - tilePartHeadLen[t][curTilePart];
            int minlys = 10000;
            for (int c = comps; c < compe; c++)
            {
                if (c >= mdl.Length)
                    continue;
                for (int r = ress; r < rese; r++)
                {
                    if (r > mdl[c])
                        continue;
                    if (lys[c] != null && r < lys[c].Length && lys[c][r] < minlys)
                    {
                        minlys = lys[c][r];
                    }
                }
            }
            System.String strInfo = "Tile " + TileIdx + " (tile-part:" + curTilePart + "): offset, length, header length\n"; ;
            int numLayers = ((System.Int32)decSpec.nls.getTileDef(t));
            bool pph = false;
            if (((System.Boolean)decSpec.pphs.getTileDef(t)))
            {
                pph = true;
            }
            int nPrec = 1;
            int start;
            int hlen, plen;
            for (int r = ress; r < rese; r++)
            {
                for (int l = minlys; l < lye; l++)
                {
                    for (int c = comps; c < compe; c++)
                    {
                        if (c >= mdl.Length)
                            continue;
                        if (r > mdl[c])
                            continue;
                        if (r >= lys[c].Length)
                            continue;
                        if (l < lys[c][r] || l >= numLayers)
                            continue;
                        nPrec = pktDec.getNumPrecinct(c, r);
                        for (int p = 0; p < nPrec; p++)
                        {
                            start = in_Renamed.Pos;
                            if (pph)
                            {
                                pktDec.readPktHead(l, r, c, p, cbI[c][r], nBytes);
                            }
                            if (start > lastByte && curTilePart < firstPackOff[t].Length - 1)
                            {
                                curTilePart++;
                                in_Renamed.seek(firstPackOff[t][curTilePart]);
                                lastByte = in_Renamed.Pos + tilePartLen[t][curTilePart] - 1 - tilePartHeadLen[t][curTilePart];
                            }
                            status = pktDec.readSOPMarker(nBytes, p, c, r);
                            if (status)
                            {
                                return true;
                            }
                            if (!pph)
                            {
                                status = pktDec.readPktHead(l, r, c, p, cbI[c][r], nBytes);
                            }
                            if (status)
                            {
                                return true;
                            }
                            hlen = in_Renamed.Pos - start;
                            pktHL.Add((System.Int32)hlen);
                            status = pktDec.readPktBody(l, r, c, p, cbI[c][r], nBytes);
                            plen = in_Renamed.Pos - start;
                            strInfo += (" Pkt l=" + l + ",r=" + r + ",c=" + c + ",p=" + p + ": " + start + ", " + plen + ", " + hlen + "\n");
                            if (status)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        private bool readResPosCompLy(int[][] lys, int lye, int ress, int rese, int comps, int compe)
        {
            JPXImageCoordinates nTiles = getNumTiles(null);
            JPXImageCoordinates tileI = getTile(null);
            int x0siz = hd.ImgULX;
            int y0siz = hd.ImgULY;
            int xsiz = x0siz + hd.ImgWidth;
            int ysiz = y0siz + hd.ImgHeight;
            int xt0siz = TilePartULX;
            int yt0siz = TilePartULY;
            int xtsiz = NomTileWidth;
            int ytsiz = NomTileHeight;
            int tx0 = (tileI.x == 0) ? x0siz : xt0siz + tileI.x * xtsiz;
            int ty0 = (tileI.y == 0) ? y0siz : yt0siz + tileI.y * ytsiz;
            int tx1 = (tileI.x != nTiles.x - 1) ? xt0siz + (tileI.x + 1) * xtsiz : xsiz;
            int ty1 = (tileI.y != nTiles.y - 1) ? yt0siz + (tileI.y + 1) * ytsiz : ysiz;
            int t = TileIdx;
            PrecInfo prec;
            int p;
            int gcd_x = 0;
            int gcd_y = 0;
            int nPrec = 0;
            int[][] nextPrec = new int[compe][];
            int minlys = 100000;
            int minx = tx1;
            int miny = ty1;
            int maxx = tx0;
            int maxy = ty0;
            for (int c = comps; c < compe; c++)
            {
                for (int r = ress; r < rese; r++)
                {
                    if (c >= mdl.Length)
                        continue;
                    if (r > mdl[c])
                        continue;
                    nextPrec[c] = new int[mdl[c] + 1];
                    if (lys[c] != null && r < lys[c].Length && lys[c][r] < minlys)
                    {
                        minlys = lys[c][r];
                    }
                    p = pktDec.getNumPrecinct(c, r) - 1;
                    for (; p >= 0; p--)
                    {
                        prec = pktDec.getPrecInfo(c, r, p);
                        if (prec.rgulx != tx0)
                        {
                            if (prec.rgulx < minx)
                                minx = prec.rgulx;
                            if (prec.rgulx > maxx)
                                maxx = prec.rgulx;
                        }
                        if (prec.rguly != ty0)
                        {
                            if (prec.rguly < miny)
                                miny = prec.rguly;
                            if (prec.rguly > maxy)
                                maxy = prec.rguly;
                        }
                        if (nPrec == 0)
                        {
                            gcd_x = prec.rgw;
                            gcd_y = prec.rgh;
                        }
                        else
                        {
                            gcd_x = MathUtil.gcd(gcd_x, prec.rgw);
                            gcd_y = MathUtil.gcd(gcd_y, prec.rgh);
                        }
                        nPrec++;
                    }
                }
            }
            if (nPrec == 0)
            {
                //throw new System.ApplicationException("Image cannot have no precinct");
            }
            int pyend = (maxy - miny) / gcd_y + 1;
            int pxend = (maxx - minx) / gcd_x + 1;
            int x, y;
            int hlen, plen;
            int start;
            bool status = false;
            int lastByte = firstPackOff[t][curTilePart] + tilePartLen[t][curTilePart] - 1 - tilePartHeadLen[t][curTilePart];
            int numLayers = ((System.Int32)decSpec.nls.getTileDef(t));
            System.String strInfo = "Tile " + TileIdx + " (tile-part:" + curTilePart + "): offset, length, header length\n"; ;
            bool pph = false;
            if (((System.Boolean)decSpec.pphs.getTileDef(t)))
            {
                pph = true;
            }
            for (int r = ress; r < rese; r++)
            {
                y = ty0;
                x = tx0;
                for (int py = 0; py <= pyend; py++)
                {
                    for (int px = 0; px <= pxend; px++)
                    {
                        for (int c = comps; c < compe; c++)
                        {
                            if (c >= mdl.Length)
                                continue;
                            if (r > mdl[c])
                                continue;
                            if (nextPrec[c][r] >= pktDec.getNumPrecinct(c, r))
                            {
                                continue;
                            }
                            prec = pktDec.getPrecInfo(c, r, nextPrec[c][r]);
                            if ((prec.rgulx != x) || (prec.rguly != y))
                            {
                                continue;
                            }
                            for (int l = minlys; l < lye; l++)
                            {
                                if (r >= lys[c].Length)
                                    continue;
                                if (l < lys[c][r] || l >= numLayers)
                                    continue;
                                start = in_Renamed.Pos;
                                if (pph)
                                {
                                    pktDec.readPktHead(l, r, c, nextPrec[c][r], cbI[c][r], nBytes);
                                }
                                if (start > lastByte && curTilePart < firstPackOff[t].Length - 1)
                                {
                                    curTilePart++;
                                    in_Renamed.seek(firstPackOff[t][curTilePart]);
                                    lastByte = in_Renamed.Pos + tilePartLen[t][curTilePart] - 1 - tilePartHeadLen[t][curTilePart];
                                }
                                status = pktDec.readSOPMarker(nBytes, nextPrec[c][r], c, r);
                                if (status)
                                {
                                    return true;
                                }
                                if (!pph)
                                {
                                    status = pktDec.readPktHead(l, r, c, nextPrec[c][r], cbI[c][r], nBytes);
                                }
                                if (status)
                                {
                                    return true;
                                }
                                hlen = in_Renamed.Pos - start;
                                pktHL.Add((System.Int32)hlen);
                                status = pktDec.readPktBody(l, r, c, nextPrec[c][r], cbI[c][r], nBytes);
                                plen = in_Renamed.Pos - start;
                                strInfo += (" Pkt l=" + l + ",r=" + r + ",c=" + c + ",p=" + nextPrec[c][r] + ": " + start + ", " + plen + ", " + hlen + "\n");
                                if (status)
                                {
                                    return true;
                                }
                            }
                            nextPrec[c][r]++;
                        }
                        if (px != pxend)
                        {
                            x = minx + px * gcd_x;
                        }
                        else
                        {
                            x = tx0;
                        }
                    }
                    if (py != pyend)
                    {
                        y = miny + py * gcd_y;
                    }
                    else
                    {
                        y = ty0;
                    }
                }
            }
            return false;
        }
        private bool readPosCompResLy(int[][] lys, int lye, int ress, int rese, int comps, int compe)
        {
            JPXImageCoordinates nTiles = getNumTiles(null);
            JPXImageCoordinates tileI = getTile(null);
            int x0siz = hd.ImgULX;
            int y0siz = hd.ImgULY;
            int xsiz = x0siz + hd.ImgWidth;
            int ysiz = y0siz + hd.ImgHeight;
            int xt0siz = TilePartULX;
            int yt0siz = TilePartULY;
            int xtsiz = NomTileWidth;
            int ytsiz = NomTileHeight;
            int tx0 = (tileI.x == 0) ? x0siz : xt0siz + tileI.x * xtsiz;
            int ty0 = (tileI.y == 0) ? y0siz : yt0siz + tileI.y * ytsiz;
            int tx1 = (tileI.x != nTiles.x - 1) ? xt0siz + (tileI.x + 1) * xtsiz : xsiz;
            int ty1 = (tileI.y != nTiles.y - 1) ? yt0siz + (tileI.y + 1) * ytsiz : ysiz;
            int t = TileIdx;
            PrecInfo prec;
            int p;
            int gcd_x = 0;
            int gcd_y = 0;
            int nPrec = 0;
            int[][] nextPrec = new int[compe][];
            int minlys = 100000;
            int minx = tx1;
            int miny = ty1;
            int maxx = tx0;
            int maxy = ty0;
            for (int c = comps; c < compe; c++)
            {
                for (int r = ress; r < rese; r++)
                {
                    if (c >= mdl.Length)
                        continue;
                    if (r > mdl[c])
                        continue;
                    nextPrec[c] = new int[mdl[c] + 1];
                    if (lys[c] != null && r < lys[c].Length && lys[c][r] < minlys)
                    {
                        minlys = lys[c][r];
                    }
                    p = pktDec.getNumPrecinct(c, r) - 1;
                    for (; p >= 0; p--)
                    {
                        prec = pktDec.getPrecInfo(c, r, p);
                        if (prec.rgulx != tx0)
                        {
                            if (prec.rgulx < minx)
                                minx = prec.rgulx;
                            if (prec.rgulx > maxx)
                                maxx = prec.rgulx;
                        }
                        if (prec.rguly != ty0)
                        {
                            if (prec.rguly < miny)
                                miny = prec.rguly;
                            if (prec.rguly > maxy)
                                maxy = prec.rguly;
                        }
                        if (nPrec == 0)
                        {
                            gcd_x = prec.rgw;
                            gcd_y = prec.rgh;
                        }
                        else
                        {
                            gcd_x = MathUtil.gcd(gcd_x, prec.rgw);
                            gcd_y = MathUtil.gcd(gcd_y, prec.rgh);
                        }
                        nPrec++;
                    }
                }
            }
            if (nPrec == 0)
            {
                //throw new System.ApplicationException("Image cannot have no precinct");
            }
            int pyend = (maxy - miny) / gcd_y + 1;
            int pxend = (maxx - minx) / gcd_x + 1;
            int hlen, plen;
            int start;
            bool status = false;
            int lastByte = firstPackOff[t][curTilePart] + tilePartLen[t][curTilePart] - 1 - tilePartHeadLen[t][curTilePart];
            int numLayers = ((System.Int32)decSpec.nls.getTileDef(t));
            System.String strInfo = "Tile " + TileIdx + " (tile-part:" + curTilePart + "): offset, length, header length\n"; ;
            bool pph = false;
            if (((System.Boolean)decSpec.pphs.getTileDef(t)))
            {
                pph = true;
            }
            int y = ty0;
            int x = tx0;
            for (int py = 0; py <= pyend; py++)
            {
                for (int px = 0; px <= pxend; px++)
                {
                    for (int c = comps; c < compe; c++)
                    {
                        if (c >= mdl.Length)
                            continue;
                        for (int r = ress; r < rese; r++)
                        {
                            if (r > mdl[c])
                                continue;
                            if (nextPrec[c][r] >= pktDec.getNumPrecinct(c, r))
                            {
                                continue;
                            }
                            prec = pktDec.getPrecInfo(c, r, nextPrec[c][r]);
                            if ((prec.rgulx != x) || (prec.rguly != y))
                            {
                                continue;
                            }
                            for (int l = minlys; l < lye; l++)
                            {
                                if (r >= lys[c].Length)
                                    continue;
                                if (l < lys[c][r] || l >= numLayers)
                                    continue;
                                start = in_Renamed.Pos;
                                if (pph)
                                {
                                    pktDec.readPktHead(l, r, c, nextPrec[c][r], cbI[c][r], nBytes);
                                }
                                if (start > lastByte && curTilePart < firstPackOff[t].Length - 1)
                                {
                                    curTilePart++;
                                    in_Renamed.seek(firstPackOff[t][curTilePart]);
                                    lastByte = in_Renamed.Pos + tilePartLen[t][curTilePart] - 1 - tilePartHeadLen[t][curTilePart];
                                }
                                status = pktDec.readSOPMarker(nBytes, nextPrec[c][r], c, r);
                                if (status)
                                {
                                    return true;
                                }
                                if (!pph)
                                {
                                    status = pktDec.readPktHead(l, r, c, nextPrec[c][r], cbI[c][r], nBytes);
                                }
                                if (status)
                                {
                                    return true;
                                }
                                hlen = in_Renamed.Pos - start;
                                pktHL.Add((System.Int32)hlen);
                                // Reads packet's body
                                status = pktDec.readPktBody(l, r, c, nextPrec[c][r], cbI[c][r], nBytes);
                                plen = in_Renamed.Pos - start;
                                strInfo += (" Pkt l=" + l + ",r=" + r + ",c=" + c + ",p=" + nextPrec[c][r] + ": " + start + ", " + plen + ", " + hlen + "\n");
                                if (status)
                                {
                                    return true;
                                }
                            }
                            nextPrec[c][r]++;
                        }
                    }
                    if (px != pxend)
                    {
                        x = minx + px * gcd_x;
                    }
                    else
                    {
                        x = tx0;
                    }
                }
                if (py != pyend)
                {
                    y = miny + py * gcd_y;
                }
                else
                {
                    y = ty0;
                }
            }
            return false;
        }
        private bool readCompPosResLy(int[][] lys, int lye, int ress, int rese, int comps, int compe)
        {
            JPXImageCoordinates nTiles = getNumTiles(null);
            JPXImageCoordinates tileI = getTile(null);
            int x0siz = hd.ImgULX;
            int y0siz = hd.ImgULY;
            int xsiz = x0siz + hd.ImgWidth;
            int ysiz = y0siz + hd.ImgHeight;
            int xt0siz = TilePartULX;
            int yt0siz = TilePartULY;
            int xtsiz = NomTileWidth;
            int ytsiz = NomTileHeight;
            int tx0 = (tileI.x == 0) ? x0siz : xt0siz + tileI.x * xtsiz;
            int ty0 = (tileI.y == 0) ? y0siz : yt0siz + tileI.y * ytsiz;
            int tx1 = (tileI.x != nTiles.x - 1) ? xt0siz + (tileI.x + 1) * xtsiz : xsiz;
            int ty1 = (tileI.y != nTiles.y - 1) ? yt0siz + (tileI.y + 1) * ytsiz : ysiz;
            int t = TileIdx;
            PrecInfo prec;
            int p;
            int gcd_x = 0;
            int gcd_y = 0;
            int nPrec = 0;
            int[][] nextPrec = new int[compe][];
            int minlys = 100000;
            int minx = tx1;
            int miny = ty1;
            int maxx = tx0;
            int maxy = ty0;
            for (int c = comps; c < compe; c++)
            {
                for (int r = ress; r < rese; r++)
                {
                    if (c >= mdl.Length)
                        continue;
                    if (r > mdl[c])
                        continue;
                    nextPrec[c] = new int[mdl[c] + 1];
                    if (lys[c] != null && r < lys[c].Length && lys[c][r] < minlys)
                    {
                        minlys = lys[c][r];
                    }
                    p = pktDec.getNumPrecinct(c, r) - 1;
                    for (; p >= 0; p--)
                    {
                        prec = pktDec.getPrecInfo(c, r, p);
                        if (prec.rgulx != tx0)
                        {
                            if (prec.rgulx < minx)
                                minx = prec.rgulx;
                            if (prec.rgulx > maxx)
                                maxx = prec.rgulx;
                        }
                        if (prec.rguly != ty0)
                        {
                            if (prec.rguly < miny)
                                miny = prec.rguly;
                            if (prec.rguly > maxy)
                                maxy = prec.rguly;
                        }
                        if (nPrec == 0)
                        {
                            gcd_x = prec.rgw;
                            gcd_y = prec.rgh;
                        }
                        else
                        {
                            gcd_x = MathUtil.gcd(gcd_x, prec.rgw);
                            gcd_y = MathUtil.gcd(gcd_y, prec.rgh);
                        }
                        nPrec++;
                    }
                }
            }
            if (nPrec == 0)
            {
                //throw new System.ApplicationException("Image cannot have no precinct");
            }
            int pyend = (maxy - miny) / gcd_y + 1;
            int pxend = (maxx - minx) / gcd_x + 1;
            int hlen, plen;
            int start;
            bool status = false;
            int lastByte = firstPackOff[t][curTilePart] + tilePartLen[t][curTilePart] - 1 - tilePartHeadLen[t][curTilePart];
            int numLayers = ((System.Int32)decSpec.nls.getTileDef(t));
            System.String strInfo = "Tile " + TileIdx + " (tile-part:" + curTilePart + "): offset, length, header length\n"; ;
            bool pph = false;
            if (((System.Boolean)decSpec.pphs.getTileDef(t)))
            {
                pph = true;
            }
            int x, y;
            for (int c = comps; c < compe; c++)
            {
                if (c >= mdl.Length)
                    continue;
                y = ty0;
                x = tx0;
                for (int py = 0; py <= pyend; py++)
                {
                    for (int px = 0; px <= pxend; px++)
                    {
                        for (int r = ress; r < rese; r++)
                        {
                            if (r > mdl[c])
                                continue;
                            if (nextPrec[c][r] >= pktDec.getNumPrecinct(c, r))
                            {
                                continue;
                            }
                            prec = pktDec.getPrecInfo(c, r, nextPrec[c][r]);
                            if ((prec.rgulx != x) || (prec.rguly != y))
                            {
                                continue;
                            }
                            for (int l = minlys; l < lye; l++)
                            {
                                if (r >= lys[c].Length)
                                    continue;
                                if (l < lys[c][r])
                                    continue;
                                start = in_Renamed.Pos;
                                if (pph)
                                {
                                    pktDec.readPktHead(l, r, c, nextPrec[c][r], cbI[c][r], nBytes);
                                }
                                if (start > lastByte && curTilePart < firstPackOff[t].Length - 1)
                                {
                                    curTilePart++;
                                    in_Renamed.seek(firstPackOff[t][curTilePart]);
                                    lastByte = in_Renamed.Pos + tilePartLen[t][curTilePart] - 1 - tilePartHeadLen[t][curTilePart];
                                }
                                status = pktDec.readSOPMarker(nBytes, nextPrec[c][r], c, r);
                                if (status)
                                {
                                    return true;
                                }
                                if (!pph)
                                {
                                    status = pktDec.readPktHead(l, r, c, nextPrec[c][r], cbI[c][r], nBytes);
                                }
                                if (status)
                                {
                                    return true;
                                }
                                hlen = in_Renamed.Pos - start;
                                pktHL.Add((System.Int32)hlen);
                                status = pktDec.readPktBody(l, r, c, nextPrec[c][r], cbI[c][r], nBytes);
                                plen = in_Renamed.Pos - start;
                                strInfo += (" Pkt l=" + l + ",r=" + r + ",c=" + c + ",p=" + nextPrec[c][r] + ": " + start + ", " + plen + ", " + hlen + "\n");
                                if (status)
                                {
                                    return true;
                                }
                            }
                            nextPrec[c][r]++;
                        }
                        if (px != pxend)
                        {
                            x = minx + px * gcd_x;
                        }
                        else
                        {
                            x = tx0;
                        }
                    }
                    if (py != pyend)
                    {
                        y = miny + py * gcd_y;
                    }
                    else
                    {
                        y = ty0;
                    }
                }
            }
            return false;
        }
        private void readTilePkts(int t)
        {
            pktHL =new ArrayList(new ArrayList(10));
            int nl = ((System.Int32)decSpec.nls.getTileDef(t));
            if (((System.Boolean)decSpec.pphs.getTileDef(t)))
            {
                System.IO.MemoryStream pphbais = hd.getPackedPktHead(t);
                cbI = pktDec.restart(this.nc, mdl, nl, cbI, true, pphbais);
            }
            else
            {
                cbI = pktDec.restart(this.nc, mdl, nl, cbI, false, null);
            }
            int[][] pocSpec = ((int[][])decSpec.pcs.getTileDef(t));
            int nChg = (pocSpec == null) ? 1 : pocSpec.Length;
            int[][] change = new int[nChg][];
            for (int i = 0; i < nChg; i++)
            {
                change[i] = new int[6];
            }
            int idx = 0;
            change[0][1] = 0;
            if (pocSpec == null)
            {
                change[idx][0] = ((System.Int32)decSpec.pos.getTileDef(t));
                change[idx][1] = nl;
                change[idx][2] = 0;
                change[idx][3] = decSpec.dls.getMaxInTile(t) + 1;
                change[idx][4] = 0;
                change[idx][5] = this.nc;
            }
            else
            {
                for (idx = 0; idx < nChg; idx++)
                {
                    change[idx][0] = pocSpec[idx][5];
                    change[idx][1] = pocSpec[idx][2];
                    change[idx][2] = pocSpec[idx][0];
                    change[idx][3] = pocSpec[idx][3];
                    change[idx][4] = pocSpec[idx][1];
                    change[idx][5] = pocSpec[idx][4];
                }
            }
            try
            {
                if (isTruncMode && firstPackOff == null || firstPackOff[t] == null)
                {
                    return;
                }
                in_Renamed.seek(firstPackOff[t][0]);
            }
            catch (System.IO.EndOfStreamException)
            {
                return;
            }
            curTilePart = 0;
            int lye, ress, rese, comps, compe;
            bool status = false;
            int nb = nBytes[t];
            int[][] lys = new int[this.nc][];
            for (int c = 0; c < this.nc; c++)
            {
                lys[c] = new int[((System.Int32)decSpec.dls.getTileCompVal(t, c)) + 1];
            }
            try
            {
                for (int chg = 0; chg < nChg; chg++)
                {
                    lye = change[chg][1];
                    ress = change[chg][2];
                    rese = change[chg][3];
                    comps = change[chg][4];
                    compe = change[chg][5];
                    switch (change[chg][0])
                    {
                        case Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.LY_RES_COMP_POS_PROG:
                            status = readLyResCompPos(lys, lye, ress, rese, comps, compe);
                            break;
                        case Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.RES_LY_COMP_POS_PROG:
                            status = readResLyCompPos(lys, lye, ress, rese, comps, compe);
                            break;
                        case Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.RES_POS_COMP_LY_PROG:
                            status = readResPosCompLy(lys, lye, ress, rese, comps, compe);
                            break;
                        case Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.POS_COMP_RES_LY_PROG:
                            status = readPosCompResLy(lys, lye, ress, rese, comps, compe);
                            break;
                        case Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.COMP_POS_RES_LY_PROG:
                            status = readCompPosResLy(lys, lye, ress, rese, comps, compe);
                            break;
                        default:
                            throw new System.ArgumentException("Not recognized " + "progression type");
                    }
                    for (int c = comps; c < compe; c++)
                    {
                        if (c >= lys.Length)
                            continue;
                        for (int r = ress; r < rese; r++)
                        {
                            if (r >= lys[c].Length)
                                continue;
                            lys[c][r] = lye;
                        }
                    }
                    if (status || usePOCQuit)
                    {
                        break;
                    }
                }
            }
            catch (System.IO.EndOfStreamException e)
            {
                throw e;
            }
            if (isTruncMode)
            {
                anbytes += nb - nBytes[t];
                if (status)
                {
                    nBytes[t] = 0;
                }
            }
            else if (nBytes[t] < (totTileLen[t] - totTileHeadLen[t]))
            {
                CBlkInfo cb;
                bool reject;
                bool stopCount = false;
                int[] pktHeadLen = new int[pktHL.Count];
                for (int i = pktHL.Count - 1; i >= 0; i--)
                {
                    pktHeadLen[i] = ((System.Int32)pktHL[i]);
                }
                reject = false;
                for (int l = 0; l < nl; l++)
                {
                    if (cbI == null)
                        continue;
                    int nc = cbI.Length;
                    int mres = 0;
                    for (int c = 0; c < nc; c++)
                    {
                        if (cbI[c] != null && cbI[c].Length > mres)
                            mres = cbI[c].Length;
                    }
                    for (int r = 0; r < mres; r++)
                    {
                        int msub = 0;
                        for (int c = 0; c < nc; c++)
                        {
                            if (cbI[c] != null && cbI[c][r] != null && cbI[c][r].Length > msub)
                                msub = cbI[c][r].Length;
                        }
                        for (int s = 0; s < msub; s++)
                        {
                            if (r == 0 && s != 0)
                            {
                                continue;
                            }
                            else if (r != 0 && s == 0)
                            {
                                continue;
                            }
                            int mnby = 0;
                            for (int c = 0; c < nc; c++)
                            {
                                if (cbI[c] != null && cbI[c][r] != null && cbI[c][r][s] != null && cbI[c][r][s].Length > mnby)
                                    mnby = cbI[c][r][s].Length;
                            }
                            for (int m = 0; m < mnby; m++)
                            {
                                int mnbx = 0;
                                for (int c = 0; c < nc; c++)
                                {
                                    if (cbI[c] != null && cbI[c][r] != null && cbI[c][r][s] != null && cbI[c][r][s][m] != null && cbI[c][r][s][m].Length > mnbx)
                                        mnbx = cbI[c][r][s][m].Length;
                                }
                                for (int n = 0; n < mnbx; n++)
                                {
                                    for (int c = 0; c < nc; c++)
                                    {
                                        if (cbI[c] == null || cbI[c][r] == null || cbI[c][r][s] == null || cbI[c][r][s][m] == null || cbI[c][r][s][m][n] == null)
                                        {
                                            continue;
                                        }
                                        cb = cbI[c][r][s][m][n];
                                        if (!reject)
                                        {
                                            if (nBytes[t] < pktHeadLen[cb.pktIdx[l]])
                                            {
                                                stopCount = true;
                                                reject = true;
                                            }
                                            else
                                            {
                                                if (!stopCount)
                                                {
                                                    nBytes[t] -= pktHeadLen[cb.pktIdx[l]];
                                                    anbytes += pktHeadLen[cb.pktIdx[l]];
                                                    pktHeadLen[cb.pktIdx[l]] = 0;
                                                }
                                            }
                                        }
                                        if (cb.len[l] == 0)
                                        {
                                            continue;
                                        }
                                        if (cb.len[l] < nBytes[t] && !reject)
                                        {
                                            nBytes[t] -= cb.len[l];
                                            anbytes += cb.len[l];
                                        }
                                        else
                                        {
                                            cb.len[l] = cb.off[l] = cb.ntp[l] = 0;
                                            reject = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                anbytes += totTileLen[t] - totTileHeadLen[t];
                if (t < getNumTiles() - 1)
                {
                    nBytes[t + 1] += nBytes[t] - (totTileLen[t] - totTileHeadLen[t]);
                }
            }
        }
        public override void setTile(int x, int y)
        {
            int i;
            if (x < 0 || y < 0 || x >= ntX || y >= ntY)
            {
                throw new System.ArgumentException();
            }
            int t = (y * ntX + x);
            if (t == 0)
            {
                anbytes = headLen;
                if (!isTruncMode)
                {
                    anbytes += 2;
                }
                for (int tIdx = 0; tIdx < nt; tIdx++)
                {
                    nBytes[tIdx] = baknBytes[tIdx];
                }
            }
            ctX = x;
            ctY = y;
            int ctox = (x == 0) ? ax : px + x * ntW;
            int ctoy = (y == 0) ? ay : py + y * ntH;
            for (i = nc - 1; i >= 0; i--)
            {
                culx[i] = (ctox + hd.getCompSubsX(i) - 1) / hd.getCompSubsX(i);
                culy[i] = (ctoy + hd.getCompSubsY(i) - 1) / hd.getCompSubsY(i);
                offX[i] = (px + x * ntW + hd.getCompSubsX(i) - 1) / hd.getCompSubsX(i);
                offY[i] = (py + y * ntH + hd.getCompSubsY(i) - 1) / hd.getCompSubsY(i);
            }
            subbTrees = new SubbandSyn[nc];
            mdl = new int[nc];
            derived = new bool[nc];
            params_Renamed = new StdDequantizerParams[nc];
            gb = new int[nc];
            for (int c = 0; c < nc; c++)
            {
                derived[c] = decSpec.qts.isDerived(t, c);
                params_Renamed[c] = (StdDequantizerParams)decSpec.qsss.getTileCompVal(t, c);
                gb[c] = ((System.Int32)decSpec.gbs.getTileCompVal(t, c));
                mdl[c] = ((System.Int32)decSpec.dls.getTileCompVal(t, c));
                subbTrees[c] = new SubbandSyn(getTileCompWidth(t, c, mdl[c]), getTileCompHeight(t, c, mdl[c]), getResULX(c, mdl[c]), getResULY(c, mdl[c]), mdl[c], decSpec.wfs.getHFilters(t, c), decSpec.wfs.getVFilters(t, c));
                initSubbandsFields(c, subbTrees[c]);
            }
            try
            {
                readTilePkts(t);
            }
            catch (System.IO.IOException)
            {
                //throw new System.ApplicationException("IO Error when reading tile " + x + " x " + y);
            }
        }
        public override void nextTile()
        {
            if (ctX == ntX - 1 && ctY == ntY - 1)
            {
                throw new System.Exception();
            }
            else if (ctX < ntX - 1)
            {
                setTile(ctX + 1, ctY);
            }
            else
            {
                setTile(0, ctY + 1);
            }
        }
        public override DecLyrdCBlk getCodeBlock(int c, int m, int n, SubbandSyn sb, int fl, int nl, DecLyrdCBlk ccb)
        {
            int t = TileIdx;
            CBlkInfo rcb;
            int r = sb.resLvl;
            int s = sb.sbandIdx;
            int tpidx;
            int passtype;
            int numLayers = ((System.Int32)decSpec.nls.getTileDef(t));
            int options = ((System.Int32)decSpec.ecopts.getTileCompVal(t, c));
            if (nl < 0)
            {
                nl = numLayers - fl + 1;
            }
            if (lQuit != -1 && fl + nl > lQuit)
            {
                nl = lQuit - fl;
            }
            int maxdl = getSynSubbandTree(t, c).resLvl;
            if (r > targetRes + maxdl - decSpec.dls.Min)
            {
                //throw new System.ApplicationException("JJ2000 error: requesting a code-block " + "disallowed by the '-res' option.");
            }
            try
            {
                rcb = cbI[c][r][s][m][n];
                if (fl < 1 || fl > numLayers || fl + nl - 1 > numLayers)
                {
                    throw new System.ArgumentException();
                }
            }
            catch (System.IndexOutOfRangeException)
            {
                throw new System.ArgumentException("Code-block (t:" + t + ", c:" + c + ", r:" + r + ", s:" + s + ", " + m + "x" + (+n) + ") not found in codestream");
            }
            catch (System.NullReferenceException)
            {
                throw new System.ArgumentException("Code-block (t:" + t + ", c:" + c + ", r:" + r + ", s:" + s + ", " + m + "x" + n + ") not found in bit stream");
            }
            if (ccb == null)
            {
                ccb = new DecLyrdCBlk();
            }
            ccb.m = m;
            ccb.n = n;
            ccb.nl = 0;
            ccb.dl = 0;
            ccb.nTrunc = 0;
            if (rcb == null)
            {
                ccb.skipMSBP = 0;
                ccb.prog = false;
                ccb.w = ccb.h = ccb.ulx = ccb.uly = 0;
                return ccb;
            }
            ccb.skipMSBP = rcb.msbSkipped;
            ccb.ulx = rcb.ulx;
            ccb.uly = rcb.uly;
            ccb.w = rcb.w;
            ccb.h = rcb.h;
            ccb.ftpIdx = 0;
            int l = 0;
            while ((l < rcb.len.Length) && (rcb.len[l] == 0))
            {
                ccb.ftpIdx += rcb.ntp[l];
                l++;
            }
            for (l = fl - 1; l < fl + nl - 1; l++)
            {
                ccb.nl++;
                ccb.dl += rcb.len[l];
                ccb.nTrunc += rcb.ntp[l];
            }
            int nts;
            if ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_TERM_PASS) != 0)
            {
                // Regular termination in use One segment per pass
                // (i.e. truncation point)
                nts = ccb.nTrunc - ccb.ftpIdx;
            }
            else if ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_BYPASS) != 0)
            {
                if (ccb.nTrunc <= Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.FIRST_BYPASS_PASS_IDX)
                {
                    nts = 1;
                }
                else
                {
                    nts = 1;
                    for (tpidx = ccb.ftpIdx; tpidx < ccb.nTrunc; tpidx++)
                    {
                        if (tpidx >= Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.FIRST_BYPASS_PASS_IDX - 1)
                        {
                            passtype = (tpidx + Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.NUM_EMPTY_PASSES_IN_MS_BP) % Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.NUM_PASSES;
                            if (passtype == 1 || passtype == 2)
                            {
                                nts++;
                            }
                        }
                    }
                }
            }
            else
            {
                nts = 1;
            }
            if (ccb.data == null || ccb.data.Length < ccb.dl)
            {
                ccb.data = new byte[ccb.dl];
            }
            if (nts > 1 && (ccb.tsLengths == null || ccb.tsLengths.Length < nts))
            {
                ccb.tsLengths = new int[nts];
            }
            else if (nts > 1 && (options & (Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_BYPASS | Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_TERM_PASS)) == Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_BYPASS)
            {
                ArrayUtil.intArraySet(ccb.tsLengths, 0);
            }
            int dataIdx = -1;
            tpidx = ccb.ftpIdx;
            int ctp = ccb.ftpIdx;
            int tsidx = 0;
            int j;
            for (l = fl - 1; l < fl + nl - 1; l++)
            {
                ctp += rcb.ntp[l];
                if (rcb.len[l] == 0)
                    continue;
                try
                {
                    in_Renamed.seek(rcb.off[l]);
                    in_Renamed.readFully(ccb.data, dataIdx + 1, rcb.len[l]);
                    dataIdx += rcb.len[l];
                }
                catch (System.IO.IOException)
                {
                }
                if (nts == 1)
                    continue;
                if ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_TERM_PASS) != 0)
                {
                    for (j = 0; tpidx < ctp; j++, tpidx++)
                    {
                        if (rcb.segLen[l] != null)
                        {
                            ccb.tsLengths[tsidx++] = rcb.segLen[l][j];
                        }
                        else
                        {
                            ccb.tsLengths[tsidx++] = rcb.len[l];
                        }
                    }
                }
                else
                {
                    for (j = 0; tpidx < ctp; tpidx++)
                    {
                        if (tpidx >= Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.FIRST_BYPASS_PASS_IDX - 1)
                        {
                            passtype = (tpidx + Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.NUM_EMPTY_PASSES_IN_MS_BP) % Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.NUM_PASSES;
                            if (passtype != 0)
                            {
                                if (rcb.segLen[l] != null)
                                {
                                    ccb.tsLengths[tsidx++] += rcb.segLen[l][j++];
                                    rcb.len[l] -= rcb.segLen[l][j - 1];
                                }
                                else
                                {
                                    ccb.tsLengths[tsidx++] += rcb.len[l];
                                    rcb.len[l] = 0;
                                }
                            }
                        }
                    }
                    if (rcb.segLen[l] != null && j < rcb.segLen[l].Length)
                    {
                        ccb.tsLengths[tsidx] += rcb.segLen[l][j];
                        rcb.len[l] -= rcb.segLen[l][j];
                    }
                    else
                    {
                        if (tsidx < nts)
                        {
                            ccb.tsLengths[tsidx] += rcb.len[l];
                            rcb.len[l] = 0;
                        }
                    }
                }
            }
            if (nts == 1 && ccb.tsLengths != null)
            {
                ccb.tsLengths[0] = ccb.dl;
            }
            int lastlayer = fl + nl - 1;
            if (lastlayer < numLayers - 1)
            {
                for (l = lastlayer + 1; l < numLayers; l++)
                {
                    if (rcb.len[l] != 0)
                    {
                        ccb.prog = true;
                    }
                }
            }
            return ccb;
        }
    }
}
#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.wavelet;
using Syncfusion.Pdf.JPEG2000.image;
namespace Syncfusion.Pdf.JPEG2000.roi.encoder
{
    internal class RectROIMaskGenerator : ROIMaskGenerator
    {
        private int[] ulxs;
        private int[] ulys;
        private int[] lrxs;
        private int[] lrys;
        private int[] nrROIs;
        private SubbandRectROIMask[] sMasks;
        public RectROIMaskGenerator(ROI[] ROIs, int nrc)
            : base(ROIs, nrc)
        {
            int nr = ROIs.Length;
            int r;
            nrROIs = new int[nrc];
            sMasks = new SubbandRectROIMask[nrc];
            for (r = nr - 1; r >= 0; r--)
            {
                nrROIs[ROIs[r].comp]++;
            }
        }
        internal override bool getROIMask(DataBlockInt db, Subband sb, int magbits, int c)
        {
            int x = db.ulx;
            int y = db.uly;
            int w = db.w;
            int h = db.h;
            int[] mask = db.DataInt;
            int i, j, k, r, maxk, maxj;
            int ulx = 0, uly = 0, lrx = 0, lry = 0;
            int wrap;
            int maxROI;
            int[] culxs;
            int[] culys;
            int[] clrxs;
            int[] clrys;
            SubbandRectROIMask srm;
            if (!tileMaskMade[c])
            {
                makeMask(sb, magbits, c);
                tileMaskMade[c] = true;
            }
            if (!roiInTile)
            {
                return false;
            }
            srm = (SubbandRectROIMask)sMasks[c].getSubbandRectROIMask(x, y);
            culxs = srm.ulxs;
            culys = srm.ulys;
            clrxs = srm.lrxs;
            clrys = srm.lrys;
            maxROI = culxs.Length - 1;
            x -= srm.ulx;
            y -= srm.uly;
            for (r = maxROI; r >= 0; r--)
            {
                ulx = culxs[r] - x;
                if (ulx < 0)
                {
                    ulx = 0;
                }
                else if (ulx >= w)
                {
                    ulx = w;
                }
                uly = culys[r] - y;
                if (uly < 0)
                {
                    uly = 0;
                }
                else if (uly >= h)
                {
                    uly = h;
                }
                lrx = clrxs[r] - x;
                if (lrx < 0)
                {
                    lrx = -1;
                }
                else if (lrx >= w)
                {
                    lrx = w - 1;
                }
                lry = clrys[r] - y;
                if (lry < 0)
                {
                    lry = -1;
                }
                else if (lry >= h)
                {
                    lry = h - 1;
                }
                i = w * lry + lrx;
                maxj = (lrx - ulx);
                wrap = w - maxj - 1;
                maxk = lry - uly;
                for (k = maxk; k >= 0; k--)
                {
                    for (j = maxj; j >= 0; j--, i--)
                        mask[i] = magbits;
                    i -= wrap;
                }
            }
            return true;
        }
        public override System.String ToString()
        {
            return ("Fast rectangular ROI mask generator");
        }
        public override void makeMask(Subband sb, int magbits, int n)
        {
            int nr = nrROIs[n];
            int r;
            int ulx, uly, lrx, lry;
            int tileulx = sb.ulcx;
            int tileuly = sb.ulcy;
            int tilew = sb.w;
            int tileh = sb.h;
            ROI[] ROIs = roi_array;
            ulxs = new int[nr];
            ulys = new int[nr];
            lrxs = new int[nr];
            lrys = new int[nr];
            nr = 0;
            for (r = ROIs.Length - 1; r >= 0; r--)
            {
                if (ROIs[r].comp == n)
                {
                    ulx = ROIs[r].ulx;
                    uly = ROIs[r].uly;
                    lrx = ROIs[r].w + ulx - 1;
                    lry = ROIs[r].h + uly - 1;
                    if (ulx > (tileulx + tilew - 1) || uly > (tileuly + tileh - 1) || lrx < tileulx || lry < tileuly)
                        continue;
                    ulx -= tileulx;
                    lrx -= tileulx;
                    uly -= tileuly;
                    lry -= tileuly;
                    ulx = (ulx < 0) ? 0 : ulx;
                    uly = (uly < 0) ? 0 : uly;
                    lrx = (lrx > (tilew - 1)) ? tilew - 1 : lrx;
                    lry = (lry > (tileh - 1)) ? tileh - 1 : lry;
                    ulxs[nr] = ulx;
                    ulys[nr] = uly;
                    lrxs[nr] = lrx;
                    lrys[nr] = lry;
                    nr++;
                }
            }
            if (nr == 0)
            {
                roiInTile = false;
            }
            else
            {
                roiInTile = true;
            }
            sMasks[n] = new SubbandRectROIMask(sb, ulxs, ulys, lrxs, lrys, nr);
        }
    }
}
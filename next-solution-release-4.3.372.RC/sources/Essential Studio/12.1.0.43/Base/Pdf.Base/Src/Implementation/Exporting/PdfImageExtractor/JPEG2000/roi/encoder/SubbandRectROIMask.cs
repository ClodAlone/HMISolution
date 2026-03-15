#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.wavelet;
namespace Syncfusion.Pdf.JPEG2000.roi.encoder
{
    internal class SubbandRectROIMask : SubbandROIMask
    {
        public int[] ulxs;
        public int[] ulys;
        public int[] lrxs;
        public int[] lrys;
        public SubbandRectROIMask(Subband sb, int[] ulxs, int[] ulys, int[] lrxs, int[] lrys, int nr)
            : base(sb.ulx, sb.uly, sb.w, sb.h)
        {
            this.ulxs = ulxs;
            this.ulys = ulys;
            this.lrxs = lrxs;
            this.lrys = lrys;
            int r;
            if (sb.isNode)
            {
                isNode = true;
                int horEvenLow = sb.ulcx % 2;
                int verEvenLow = sb.ulcy % 2;
                WaveletFilter hFilter = sb.HorWFilter;
                WaveletFilter vFilter = sb.VerWFilter;
                int hlnSup = hFilter.SynLowNegSupport;
                int hhnSup = hFilter.SynHighNegSupport;
                int hlpSup = hFilter.SynLowPosSupport;
                int hhpSup = hFilter.SynHighPosSupport;
                int vlnSup = vFilter.SynLowNegSupport;
                int vhnSup = vFilter.SynHighNegSupport;
                int vlpSup = vFilter.SynLowPosSupport;
                int vhpSup = vFilter.SynHighPosSupport;
                int x, y;
                int[] lulxs = new int[nr];
                int[] lulys = new int[nr];
                int[] llrxs = new int[nr];
                int[] llrys = new int[nr];
                int[] hulxs = new int[nr];
                int[] hulys = new int[nr];
                int[] hlrxs = new int[nr];
                int[] hlrys = new int[nr];
                for (r = nr - 1; r >= 0; r--)
                {
                    x = ulxs[r];
                    if (horEvenLow == 0)
                    {
                        lulxs[r] = (x + 1 - hlnSup) / 2;
                        hulxs[r] = (x - hhnSup) / 2;
                    }
                    else
                    {
                        lulxs[r] = (x - hlnSup) / 2;
                        hulxs[r] = (x + 1 - hhnSup) / 2;
                    }
                    y = ulys[r];
                    if (verEvenLow == 0)
                    {
                        lulys[r] = (y + 1 - vlnSup) / 2;
                        hulys[r] = (y - vhnSup) / 2;
                    }
                    else
                    {
                        lulys[r] = (y - vlnSup) / 2;
                        hulys[r] = (y + 1 - vhnSup) / 2;
                    }
                    x = lrxs[r];
                    if (horEvenLow == 0)
                    {
                        llrxs[r] = (x + hlpSup) / 2;
                        hlrxs[r] = (x - 1 + hhpSup) / 2;
                    }
                    else
                    {
                        llrxs[r] = (x - 1 + hlpSup) / 2;
                        hlrxs[r] = (x + hhpSup) / 2;
                    }
                    y = lrys[r];
                    if (verEvenLow == 0)
                    {
                        llrys[r] = (y + vlpSup) / 2;
                        hlrys[r] = (y - 1 + vhpSup) / 2;
                    }
                    else
                    {
                        llrys[r] = (y - 1 + vlpSup) / 2;
                        hlrys[r] = (y + vhpSup) / 2;
                    }
                }
                hh = new SubbandRectROIMask(sb.HH, hulxs, hulys, hlrxs, hlrys, nr);
                lh = new SubbandRectROIMask(sb.LH, lulxs, hulys, llrxs, hlrys, nr);
                hl = new SubbandRectROIMask(sb.HL, hulxs, lulys, hlrxs, llrys, nr);
                ll = new SubbandRectROIMask(sb.LL, lulxs, lulys, llrxs, llrys, nr);
            }
        }
    }
}
#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
namespace Syncfusion.Pdf.JPEG2000.codestream.reader
{
    internal class CBlkInfo
    {
        public int ulx;
        public int uly;
        public int w;
        public int h;
        public int msbSkipped;
        public int[] len;
        public int[] off;
        public int[] ntp;
        public int ctp;
        public int[][] segLen;
        public int[] pktIdx;
        public CBlkInfo(int ulx, int uly, int w, int h, int nl)
        {
            this.ulx = ulx;
            this.uly = uly;
            this.w = w;
            this.h = h;
            off = new int[nl];
            len = new int[nl];
            ntp = new int[nl];
            segLen = new int[nl][];
            pktIdx = new int[nl];
            for (int i = nl - 1; i >= 0; i--)
            {
                pktIdx[i] = -1;
            }
        }
        public virtual void addNTP(int l, int newtp)
        {
            ntp[l] = newtp;
            ctp = 0;
            for (int lIdx = 0; lIdx <= l; lIdx++)
            {
                ctp += ntp[lIdx];
            }
        }
        public override System.String ToString()
        {
            System.String string_Renamed = "(ulx,uly,w,h)= (" + ulx + "," + uly + "," + w + "," + h;
            string_Renamed += (") " + msbSkipped + " MSB bit(s) skipped\n");
            if (len != null)
                for (int i = 0; i < len.Length; i++)
                {
                    string_Renamed += ("\tl:" + i + ", start:" + off[i] + ", len:" + len[i] + ", ntp:" + ntp[i] + ", pktIdx=" + pktIdx[i]);
                    if (segLen != null && segLen[i] != null)
                    {
                        string_Renamed += " { ";
                        for (int j = 0; j < segLen[i].Length; j++)
                            string_Renamed += (segLen[i][j] + " ");
                        string_Renamed += "}";
                    }
                    string_Renamed += "\n";
                }
            string_Renamed += ("\tctp=" + ctp);
            return string_Renamed;
        }
    }
}
#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
namespace Syncfusion.Pdf.JPEG2000.codestream
{
    internal class PrecInfo
    {
        public int rgulx;
        public int rguly;
        public int rgw;
        public int rgh;
        public int ulx;
        public int uly;
        public int w;
        public int h;
        public int r;
        public CBlkCoordInfo[][][] cblk;
        public int[] nblk;
        public PrecInfo(int r, int ulx, int uly, int w, int h, int rgulx, int rguly, int rgw, int rgh)
        {
            this.r = r;
            this.ulx = ulx;
            this.uly = uly;
            this.w = w;
            this.h = h;
            this.rgulx = rgulx;
            this.rguly = rguly;
            this.rgw = rgw;
            this.rgh = rgh;
            if (r == 0)
            {
                cblk = new CBlkCoordInfo[1][][];
                nblk = new int[1];
            }
            else
            {
                cblk = new CBlkCoordInfo[4][][];
                nblk = new int[4];
            }
        }
        public override System.String ToString()
        {
            return "ulx=" + ulx + ",uly=" + uly + ",w=" + w + ",h=" + h + ",rgulx=" + rgulx + ",rguly=" + rguly + ",rgw=" + rgw + ",rgh=" + rgh;
        }
    }
}
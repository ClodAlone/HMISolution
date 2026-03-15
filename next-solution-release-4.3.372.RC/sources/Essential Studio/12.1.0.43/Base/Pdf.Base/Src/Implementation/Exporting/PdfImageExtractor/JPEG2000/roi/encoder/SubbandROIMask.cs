#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.roi.encoder
{
    public abstract class SubbandROIMask
    {
        internal SubbandROIMask ll;
        internal SubbandROIMask lh;
        internal SubbandROIMask hl;
        internal SubbandROIMask hh;
        internal bool isNode;
        public int ulx;
        public int uly;
        public int w;
        public int h;
        public SubbandROIMask(int ulx, int uly, int w, int h)
        {
            this.ulx = ulx;
            this.uly = uly;
            this.w = w;
            this.h = h;
        }
        public virtual SubbandROIMask getSubbandRectROIMask(int x, int y)
        {
            SubbandROIMask cur, hhs;
            if (x < ulx || y < uly || x >= ulx + w || y >= uly + h)
            {
                throw new System.ArgumentException();
            }
            cur = this;
            while (cur.isNode)
            {
                hhs = cur.hh;
                if (x < hhs.ulx)
                {
                    if (y < hhs.uly)
                    {
                        cur = cur.ll;
                    }
                    else
                    {
                        cur = cur.lh;
                    }
                }
                else
                {
                    if (y < hhs.uly)
                    {
                        cur = cur.hl;
                    }
                    else
                    {
                        cur = cur.hh;
                    }
                }
            }
            return cur;
        }
    }
}
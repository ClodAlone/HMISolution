#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.codestream
{
    public abstract class CoordInfo
    {
        public int ulx;
        public int uly;
        public int w;
        public int h;
        public CoordInfo(int ulx, int uly, int w, int h)
        {
            this.ulx = ulx;
            this.uly = uly;
            this.w = w;
            this.h = h;
        }
        public CoordInfo()
        {
        }
        public override System.String ToString()
        {
            return "ulx=" + ulx + ",uly=" + uly + ",w=" + w + ",h=" + h;
        }
    }
}
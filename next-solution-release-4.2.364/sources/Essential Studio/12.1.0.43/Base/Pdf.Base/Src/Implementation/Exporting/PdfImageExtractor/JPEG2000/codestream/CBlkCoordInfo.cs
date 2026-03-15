#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Pdf.JPEG2000.image;
namespace Syncfusion.Pdf.JPEG2000.codestream
{
    internal class CBlkCoordInfo : CoordInfo
    {
        public JPXImageCoordinates idx;
        public CBlkCoordInfo()
        {
            this.idx = new JPXImageCoordinates();
        }
        public CBlkCoordInfo(int m, int n)
        {
            this.idx = new JPXImageCoordinates(n, m);
        }
        public override System.String ToString()
        {
            return base.ToString() + ",idx=" + idx;
        }
    }
}
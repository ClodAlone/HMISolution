#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.image.input;
namespace Syncfusion.Pdf.JPEG2000.roi.encoder
{
    internal class ROI
    {
        public ImgReaderPGM maskPGM = null;
        public bool arbShape;
        public bool rect;
        public int comp;
        public int ulx;
        public int uly;
        public int w;
        public int h;
        public int x;
        public int y;
        public int r;
        public ROI(int comp, ImgReaderPGM maskPGM)
        {
            arbShape = true;
            rect = false;
            this.comp = comp;
            this.maskPGM = maskPGM;
        }
        public ROI(int comp, int ulx, int uly, int w, int h)
        {
            arbShape = false;
            this.comp = comp;
            this.ulx = ulx;
            this.uly = uly;
            this.w = w;
            this.h = h;
            rect = true;
        }
        public ROI(int comp, int x, int y, int rad)
        {
            arbShape = false;
            this.comp = comp;
            this.x = x;
            this.y = y;
            this.r = rad;
        }
        public override System.String ToString()
        {
            if (arbShape)
            {
                return "ROI with arbitrary shape, PGM file= " + maskPGM;
            }
            else if (rect)
                return "Rectangular ROI, comp=" + comp + " ulx=" + ulx + " uly=" + uly + " w=" + w + " h=" + h;
            else
                return "Circular ROI,  comp=" + comp + " x=" + x + " y=" + y + " radius=" + r;
        }
    }
}
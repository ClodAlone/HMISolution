#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.image;
using Syncfusion.Pdf.JPEG2000.wavelet;
namespace Syncfusion.Pdf.JPEG2000.roi.encoder
{
    public abstract class ROIMaskGenerator
    {
        virtual internal ROI[] ROIs
        {
            get
            {
                return roi_array;
            }
        }
        internal ROI[] roi_array;
        internal int nrc;
        internal bool[] tileMaskMade;
        internal bool roiInTile;
        internal ROIMaskGenerator(ROI[] rois, int nrc)
        {
            this.roi_array = rois;
            this.nrc = nrc;
            tileMaskMade = new bool[nrc];
        }
        internal abstract bool getROIMask(DataBlockInt db, Subband sb, int magbits, int c);
        public abstract void makeMask(Subband sb, int magbits, int n);
        public virtual void tileChanged()
        {
            for (int i = 0; i < nrc; i++)
                tileMaskMade[i] = false;
        }
    }
}
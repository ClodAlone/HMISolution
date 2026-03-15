#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.quantization.quantizer;
using Syncfusion.Pdf.JPEG2000.wavelet.analysis;
using Syncfusion.Pdf.JPEG2000.image.input;
using Syncfusion.Pdf.JPEG2000.wavelet;
using Syncfusion.Pdf.JPEG2000.image;
using Syncfusion.Pdf.JPEG2000.util;
using Syncfusion.Pdf.JPEG2000.encoder;
namespace Syncfusion.Pdf.JPEG2000.roi.encoder
{
    internal class ROIScaler
    {
        virtual public int CbULX
        {
            get
            {
                return src.CbULX;
            }
        }
        virtual public int CbULY
        {
            get
            {
                return src.CbULY;
            }
        }
        virtual public ROIMaskGenerator ROIMaskGenerator
        {
            get
            {
                return mg;
            }
        }
        virtual public bool BlockAligned
        {
            get
            {
                return blockAligned;
            }
        }
        public static System.String[][] ParameterInfo
        {
            get
            {
                return pinfo;
            }
        }
        public const char OPT_PREFIX = 'R';
        private static readonly System.String[][] pinfo = new System.String[][] { new System.String[] { "Rroi", "[<component idx>] R <left> <top> <width> <height>" + " or [<component idx>] C <centre column> <centre row> " + "<radius> or [<component idx>] A <filename>", "Specifies ROIs shape and location. The shape can be either " + "rectangular 'R', or circular 'C' or arbitrary 'A'. " + "Each new occurrence of an 'R', a 'C' or an 'A' is a new ROI. " + "For circular and rectangular ROIs, all values are " + "given as their pixel values relative to the canvas origin. " + "Arbitrary shapes must be included in a PGM file where non 0 " + "values correspond to ROI coefficients. The PGM file must have " + "the size as the image. " + "The component idx specifies which components " + "contain the ROI. The component index is specified as described " + "by points 3 and 4 in the general comment on tile-component idx. " + "If this option is used, the codestream is layer progressive by " + "default unless it is overridden by the 'Aptype' option.", null }, new System.String[] { "Ralign", "[on|off]", "By specifying this argument, the ROI mask will be " + "limited to covering only entire code-blocks. The ROI coding can " + "then be performed without any actual scaling of the coefficients " + "but by instead scaling the distortion estimates.", "off" }, new System.String[] { "Rstart_level", "<level>", "This argument forces the lowest <level> resolution levels to " + "belong to the ROI. By doing this, it is possible to avoid only " + "getting information for the ROI at an early stage of " + "transmission.<level> = 0 means the lowest resolution level " + "belongs to the ROI, 1 means the two lowest etc. (-1 deactivates" + " the option)", "-1" }, new System.String[] { "Rno_rect", "[on|off]", "This argument makes sure that the ROI mask generation is not done " + "using the fast ROI mask generation for rectangular ROIs " + "regardless of whether the specified ROIs are rectangular or not", "off" } };
        private int[][] maxMagBits;
        private bool roi;
        private bool blockAligned;
        private int useStartLevel;
        private ROIMaskGenerator mg;
        private DataBlockInt roiMask;
        private Quantizer src;
        
        public virtual bool isReversible(int t, int c)
        {
            return src.isReversible(t, c);
        }
        public virtual SubbandAn getAnSubbandTree(int t, int c)
        {
            return src.getAnSubbandTree(t, c);
        }

        public virtual bool useRoi()
        {
            return roi;
        }
       
        private void calcMaxMagBits(EncoderSpecs encSpec)
        {
            int tmp;
            MaxShiftSpec rois = encSpec.rois;
            int nt = src.getNumTiles();
            int nc = src.NumComps;
            maxMagBits = new int[nt][];
            for (int i = 0; i < nt; i++)
            {
                maxMagBits[i] = new int[nc];
            }
            src.setTile(0, 0);
            for (int t = 0; t < nt; t++)
            {
                for (int c = nc - 1; c >= 0; c--)
                {
                    tmp = src.getMaxMagBits(c);
                    maxMagBits[t][c] = tmp;
                    rois.setTileCompVal(t, c, (System.Object)tmp);
                }
                if (t < nt - 1)
                    src.nextTile();
            }
            src.setTile(0, 0);
        }
    }
}
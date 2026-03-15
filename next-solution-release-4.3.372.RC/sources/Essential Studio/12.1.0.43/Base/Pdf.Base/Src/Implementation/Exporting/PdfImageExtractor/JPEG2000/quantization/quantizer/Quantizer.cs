#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.wavelet.analysis;
using Syncfusion.Pdf.JPEG2000.wavelet;
using Syncfusion.Pdf.JPEG2000.image;
using Syncfusion.Pdf.JPEG2000.encoder;
namespace Syncfusion.Pdf.JPEG2000.quantization.quantizer
{
    internal abstract class Quantizer : ImgDataAdapter, CBlkQuantDataSrcEnc
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
        public static System.String[][] ParameterInfo
        {
            get
            {
                return pinfo;
            }
        }
        public const char OPT_PREFIX = 'Q';
        private static readonly System.String[][] pinfo = new System.String[][] { new System.String[] { "Qtype", "[<tile-component idx>] <id> " + "[ [<tile-component idx>] <id> ...]", "Specifies which quantization type to use for specified " + "tile-component. The default type is either 'reversible' or " + "'expounded' depending on whether or not the '-lossless' option " + " is specified.\n" + "<tile-component idx> : see general note.\n" + "<id>: Supported quantization types specification are : " + "'reversible' " + "(no quantization), 'derived' (derived quantization step size) and " + "'expounded'.\n" + "Example: -Qtype reversible or -Qtype t2,4-8 c2 reversible t9 " + "derived.", null }, new System.String[] { "Qstep", "[<tile-component idx>] <bnss> " + "[ [<tile-component idx>] <bnss> ...]", "This option specifies the base normalized quantization step " + "size (bnss) for tile-components. It is normalized to a " + "dynamic range of 1 in the image domain. This parameter is " + "ignored in reversible coding. The default value is '1/128'" + " (i.e. 0.0078125).", "0.0078125" }, new System.String[] { "Qguard_bits", "[<tile-component idx>] <gb> " + "[ [<tile-component idx>] <gb> ...]", "The number of bits used for each tile-component in the quantizer" + " to avoid overflow (gb).", "2" } };
        internal CBlkWTDataSrc src;
        internal Quantizer(CBlkWTDataSrc src)
            : base(src)
        {
            this.src = src;
        }
        public abstract int getNumGuardBits(int t, int c);
        public abstract bool isDerived(int t, int c);
        internal abstract void calcSbParams(SubbandAn sb, int n);
        public virtual SubbandAn getAnSubbandTree(int t, int c)
        {
            SubbandAn sbba;
            sbba = src.getAnSubbandTree(t, c);
            calcSbParams(sbba, c);
            return sbba;
        }
        internal static Quantizer createInstance(CBlkWTDataSrc src, EncoderSpecs encSpec)
        {
            return new StdQuantizer(src, encSpec);
        }
        public abstract int getMaxMagBits(int c);
        public abstract Syncfusion.Pdf.JPEG2000.wavelet.analysis.CBlkWTData getNextInternCodeBlock(int param1, Syncfusion.Pdf.JPEG2000.wavelet.analysis.CBlkWTData param2);
        public abstract Syncfusion.Pdf.JPEG2000.wavelet.analysis.CBlkWTData getNextCodeBlock(int param1, Syncfusion.Pdf.JPEG2000.wavelet.analysis.CBlkWTData param2);
        public abstract bool isReversible(int param1, int param2);
    }
}
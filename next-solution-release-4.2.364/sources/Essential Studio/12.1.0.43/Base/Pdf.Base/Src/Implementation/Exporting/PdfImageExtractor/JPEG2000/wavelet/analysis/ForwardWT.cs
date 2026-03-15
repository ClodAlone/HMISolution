#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.image;
using Syncfusion.Pdf.JPEG2000.util;
using Syncfusion.Pdf.JPEG2000.encoder;
namespace Syncfusion.Pdf.JPEG2000.wavelet.analysis
{
    internal abstract class ForwardWT : ImgDataAdapter, ForwWT, CBlkWTDataSrc
    {
        public static System.String[][] ParameterInfo
        {
            get
            {
                return pinfo;
            }
        }
        public abstract int CbULY { get; }
        public abstract int CbULX { get; }
        public const int WT_DECOMP_DYADIC = 0;
        public const char OPT_PREFIX = 'W';
        private static readonly System.String[][] pinfo = new System.String[][] { new System.String[] { "Wlev", "<number of decomposition levels>", "Specifies the number of wavelet decomposition levels to apply to " + "the image. If 0 no wavelet transform is performed. All components " + "and all tiles have the same number of decomposition levels.", "5" }, new System.String[] { "Wwt", "[full]", "Specifies the wavelet transform to be used. Possible value is: " + "'full' (full page). The value 'full' performs a normal DWT.", "full" }, new System.String[] { "Wcboff", "<x y>", "Code-blocks partition offset in the reference grid. Allowed for " + "<x> and <y> are 0 and 1.\n" + "Note: This option is defined in JPEG 2000 part 2 and may not" + " be supported by all JPEG 2000 decoders.", "0 0" } };
        internal ForwardWT(ImageData src)
            : base(src)
        {
        }
        internal static ForwardWT createInstance(BlockImageDataSource src, JPXParameters pl, EncoderSpecs encSpec)
        {
            int deflev;
            pl.checkList(OPT_PREFIX, Syncfusion.Pdf.JPEG2000.util.JPXParameters.toNameArray(pinfo));
            deflev = ((System.Int32)encSpec.dls.getDefault());
            System.String str = "";
            if (pl.getParameter("Wcboff") == null)
            {
                //throw new System.ApplicationException("You must specify an argument to the '-Wcboff' " + "option. See usage with the '-u' option");
            }
            SupportClass.Tokenizer stk = new SupportClass.Tokenizer(pl.getParameter("Wcboff"));
            if (stk.Count != 2)
            {
                throw new System.ArgumentException("'-Wcboff' option needs two" + " arguments. See usage with " + "the '-u' option.");
            }
            int cb0x = 0;
            str = stk.NextToken();
            try
            {
                cb0x = (System.Int32.Parse(str));
            }
            catch (System.FormatException)
            {
                throw new System.ArgumentException("Bad first parameter for the " + "'-Wcboff' option: " + str);
            }
            if (cb0x < 0 || cb0x > 1)
            {
                throw new System.ArgumentException("Invalid horizontal " + "code-block partition origin.");
            }
            int cb0y = 0;
            str = stk.NextToken();
            try
            {
                cb0y = (System.Int32.Parse(str));
            }
            catch (System.FormatException)
            {
                throw new System.ArgumentException("Bad second parameter for the " + "'-Wcboff' option: " + str);
            }
            if (cb0y < 0 || cb0y > 1)
            {
                throw new System.ArgumentException("Invalid vertical " + "code-block partition origin.");
            }
            if (cb0x != 0 || cb0y != 0)
            {
            }
            return new ForwWTFull(src, encSpec, cb0x, cb0y);
        }
        public abstract bool isReversible(int param1, int param2);
        public abstract Syncfusion.Pdf.JPEG2000.wavelet.analysis.CBlkWTData getNextInternCodeBlock(int param1, Syncfusion.Pdf.JPEG2000.wavelet.analysis.CBlkWTData param2);
        public abstract int getFixedPoint(int param1);
        public abstract Syncfusion.Pdf.JPEG2000.wavelet.analysis.AnWTFilter[] getHorAnWaveletFilters(int param1, int param2);
        public abstract Syncfusion.Pdf.JPEG2000.wavelet.analysis.AnWTFilter[] getVertAnWaveletFilters(int param1, int param2);
        public abstract Syncfusion.Pdf.JPEG2000.wavelet.analysis.SubbandAn getAnSubbandTree(int param1, int param2);
        public abstract int getDecompLevels(int param1, int param2);
        public abstract Syncfusion.Pdf.JPEG2000.wavelet.analysis.CBlkWTData getNextCodeBlock(int param1, Syncfusion.Pdf.JPEG2000.wavelet.analysis.CBlkWTData param2);
        public abstract int getImplementationType(int param1);
        public abstract int getDataType(int param1, int param2);
        public abstract int getDecomp(int param1, int param2);
    }
}
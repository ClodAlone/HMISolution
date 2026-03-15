#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.quantization.dequantizer;
using Syncfusion.Pdf.JPEG2000.wavelet.synthesis;
namespace Syncfusion.Pdf.JPEG2000.entropy.decoder
{
    public abstract class EntropyDecoder : MultiResImgDataAdapter, CBlkQuantDataSrcDec
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
        public const char OPT_PREFIX = 'C';
        private static readonly System.String[][] pinfo = new System.String[][] { new System.String[] { "Cverber", "[on|off]", "Specifies if the entropy decoder should be verbose about detected " + "errors. If 'on' a message is printed whenever an error is detected.", "on" }, new System.String[] { "Cer", "[on|off]", "Specifies if error detection should be performed by the entropy " + "decoder engine. If errors are detected they will be concealed and " + "the resulting distortion will be less important. Note that errors " + "can only be detected if the encoder that generated the data " + "included error resilience information.", "on" } };
        internal CodedCBlkDataSrcDec src;
        internal EntropyDecoder(CodedCBlkDataSrcDec src)
            : base(src)
        {
            this.src = src;
        }
        public override SubbandSyn getSynSubbandTree(int t, int c)
        {
            return src.getSynSubbandTree(t, c);
        }
        public abstract Syncfusion.Pdf.JPEG2000.image.DataBlock getCodeBlock(int param1, int param2, int param3, Syncfusion.Pdf.JPEG2000.wavelet.synthesis.SubbandSyn param4, Syncfusion.Pdf.JPEG2000.image.DataBlock param5);
        public abstract Syncfusion.Pdf.JPEG2000.image.DataBlock getInternCodeBlock(int param1, int param2, int param3, Syncfusion.Pdf.JPEG2000.wavelet.synthesis.SubbandSyn param4, Syncfusion.Pdf.JPEG2000.image.DataBlock param5);
    }
}
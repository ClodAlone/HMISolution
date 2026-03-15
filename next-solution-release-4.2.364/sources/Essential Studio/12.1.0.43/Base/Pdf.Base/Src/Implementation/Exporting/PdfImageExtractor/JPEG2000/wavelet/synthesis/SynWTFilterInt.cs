#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.image;
namespace Syncfusion.Pdf.JPEG2000.wavelet.synthesis
{
    public abstract class SynWTFilterInt : SynWTFilter
    {
        override public int DataType
        {
            get
            {
                return DataBlock.TYPE_INT;
            }
        }
        public abstract void synthetize_lpf(int[] lowSig, int lowOff, int lowLen, int lowStep, int[] highSig, int highOff, int highLen, int highStep, int[] outSig, int outOff, int outStep);
        public override void synthetize_lpf(System.Object lowSig, int lowOff, int lowLen, int lowStep, System.Object highSig, int highOff, int highLen, int highStep, System.Object outSig, int outOff, int outStep)
        {
            synthetize_lpf((int[])lowSig, lowOff, lowLen, lowStep, (int[])highSig, highOff, highLen, highStep, (int[])outSig, outOff, outStep);
        }
        public abstract void synthetize_hpf(int[] lowSig, int lowOff, int lowLen, int lowStep, int[] highSig, int highOff, int highLen, int highStep, int[] outSig, int outOff, int outStep);
        public override void synthetize_hpf(System.Object lowSig, int lowOff, int lowLen, int lowStep, System.Object highSig, int highOff, int highLen, int highStep, System.Object outSig, int outOff, int outStep)
        {
            synthetize_hpf((int[])lowSig, lowOff, lowLen, lowStep, (int[])highSig, highOff, highLen, highStep, (int[])outSig, outOff, outStep);
        }
    }
}
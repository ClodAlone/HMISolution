#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.wavelet.synthesis
{
    public abstract class SynWTFilter : WaveletFilter
    {
        public abstract int AnHighPosSupport { get; }
        public abstract int AnLowNegSupport { get; }
        public abstract int AnLowPosSupport { get; }
        public abstract bool Reversible { get; }
        public abstract int ImplType { get; }
        public abstract int SynHighNegSupport { get; }
        public abstract int SynHighPosSupport { get; }
        public abstract int AnHighNegSupport { get; }
        public abstract int DataType { get; }
        public abstract int SynLowNegSupport { get; }
        public abstract int SynLowPosSupport { get; }
        public abstract void synthetize_lpf(System.Object lowSig, int lowOff, int lowLen, int lowStep, System.Object highSig, int highOff, int highLen, int highStep, System.Object outSig, int outOff, int outStep);
        public abstract void synthetize_hpf(System.Object lowSig, int lowOff, int lowLen, int lowStep, System.Object highSig, int highOff, int highLen, int highStep, System.Object outSig, int outOff, int outStep);
        public abstract bool isSameAsFullWT(int param1, int param2, int param3);
    }
}
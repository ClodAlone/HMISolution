#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.image;
namespace Syncfusion.Pdf.JPEG2000.wavelet.analysis
{
    public abstract class AnWTFilterInt : AnWTFilter
    {
        override public int DataType
        {
            get
            {
                return DataBlock.TYPE_INT;
            }
        }
        public abstract void analyze_lpf(int[] inSig, int inOff, int inLen, int inStep, int[] lowSig, int lowOff, int lowStep, int[] highSig, int highOff, int highStep);
        public override void analyze_lpf(System.Object inSig, int inOff, int inLen, int inStep, System.Object lowSig, int lowOff, int lowStep, System.Object highSig, int highOff, int highStep)
        {
            analyze_lpf((int[])inSig, inOff, inLen, inStep, (int[])lowSig, lowOff, lowStep, (int[])highSig, highOff, highStep);
        }
        public abstract void analyze_hpf(int[] inSig, int inOff, int inLen, int inStep, int[] lowSig, int lowOff, int lowStep, int[] highSig, int highOff, int highStep);
        public override void analyze_hpf(System.Object inSig, int inOff, int inLen, int inStep, System.Object lowSig, int lowOff, int lowStep, System.Object highSig, int highOff, int highStep)
        {
            analyze_hpf((int[])inSig, inOff, inLen, inStep, (int[])lowSig, lowOff, lowStep, (int[])highSig, highOff, highStep);
        }
    }
}
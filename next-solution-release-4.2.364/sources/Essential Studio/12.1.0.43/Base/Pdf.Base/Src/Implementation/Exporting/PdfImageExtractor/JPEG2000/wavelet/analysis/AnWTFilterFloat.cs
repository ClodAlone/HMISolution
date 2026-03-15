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
    public abstract class AnWTFilterFloat : AnWTFilter
    {
        override public int DataType
        {
            get
            {
                return DataBlock.TYPE_FLOAT;
            }
        }
        public abstract void analyze_lpf(float[] inSig, int inOff, int inLen, int inStep, float[] lowSig, int lowOff, int lowStep, float[] highSig, int highOff, int highStep);
        public override void analyze_lpf(System.Object inSig, int inOff, int inLen, int inStep, System.Object lowSig, int lowOff, int lowStep, System.Object highSig, int highOff, int highStep)
        {
            analyze_lpf((float[])inSig, inOff, inLen, inStep, (float[])lowSig, lowOff, lowStep, (float[])highSig, highOff, highStep);
        }
        public abstract void analyze_hpf(float[] inSig, int inOff, int inLen, int inStep, float[] lowSig, int lowOff, int lowStep, float[] highSig, int highOff, int highStep);
        public override void analyze_hpf(System.Object inSig, int inOff, int inLen, int inStep, System.Object lowSig, int lowOff, int lowStep, System.Object highSig, int highOff, int highStep)
        {
            analyze_hpf((float[])inSig, inOff, inLen, inStep, (float[])lowSig, lowOff, lowStep, (float[])highSig, highOff, highStep);
        }
    }
}
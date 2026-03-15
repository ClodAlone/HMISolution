#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.wavelet.analysis
{
    public abstract class AnWTFilter : WaveletFilter
    {
        public abstract int FilterType { get; }
        public static System.String[][] ParameterInfo
        {
            get
            {
                return pinfo;
            }
        }
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
        public const char OPT_PREFIX = 'F';
        private static readonly System.String[][] pinfo = new System.String[][] { new System.String[] { "Ffilters", "[<tile-component idx>] <id> " + "[ [<tile-component idx>] <id> ...]", "Specifies which filters to use for specified tile-component. " + "If this option is not used, the encoder choses the filters " + " of the tile-components according to their quantization  type." + " If this option is used, a component transformation is applied " + "to the three first components.\n" + "<tile-component idx>: see general note\n" + "<id>: ',' separates horizontal and vertical filters, ':' separates" + " decomposition levels filters. JPEG 2000 part 1 only supports w5x3" + " and w9x7 filters.", null } };
        public abstract void analyze_lpf(System.Object inSig, int inOff, int inLen, int inStep, System.Object lowSig, int lowOff, int lowStep, System.Object highSig, int highOff, int highStep);
        public abstract void analyze_hpf(System.Object inSig, int inOff, int inLen, int inStep, System.Object lowSig, int lowOff, int lowStep, System.Object highSig, int highOff, int highStep);
        public abstract float[] getLPSynthesisFilter();
        public abstract float[] getHPSynthesisFilter();
        public virtual float[] getLPSynWaveForm(float[] in_Renamed, float[] out_Renamed)
        {
            return upsampleAndConvolve(in_Renamed, getLPSynthesisFilter(), out_Renamed);
        }
        public virtual float[] getHPSynWaveForm(float[] in_Renamed, float[] out_Renamed)
        {
            return upsampleAndConvolve(in_Renamed, getHPSynthesisFilter(), out_Renamed);
        }
        private static float[] upsampleAndConvolve(float[] in_Renamed, float[] wf, float[] out_Renamed)
        {
            int i, k, j;
            float tmp;
            int maxi, maxk;
            if (in_Renamed == null)
            {
                in_Renamed = new float[1];
                in_Renamed[0] = 1.0f;
            }
            if (out_Renamed == null)
            {
                out_Renamed = new float[in_Renamed.Length * 2 + wf.Length - 2];
            }
            for (i = 0, maxi = in_Renamed.Length * 2 + wf.Length - 2; i < maxi; i++)
            {
                tmp = 0.0f;
                k = (i - wf.Length + 2) / 2;
                if (k < 0)
                    k = 0;
                maxk = i / 2 + 1;
                if (maxk > in_Renamed.Length)
                    maxk = in_Renamed.Length;
                for (j = 2 * k - i + wf.Length - 1; k < maxk; k++, j += 2)
                {
                    tmp += in_Renamed[k] * wf[j];
                }
                out_Renamed[i] = tmp;
            }
            return out_Renamed;
        }
        public abstract bool isSameAsFullWT(int param1, int param2, int param3);
    }
}
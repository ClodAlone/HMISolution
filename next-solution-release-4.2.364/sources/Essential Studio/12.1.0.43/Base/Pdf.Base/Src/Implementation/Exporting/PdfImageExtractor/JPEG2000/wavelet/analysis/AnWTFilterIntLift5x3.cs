#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.wavelet.analysis
{
    internal class AnWTFilterIntLift5x3 : AnWTFilterInt
    {
        override public int AnLowNegSupport
        {
            get
            {
                return 2;
            }
        }
        override public int AnLowPosSupport
        {
            get
            {
                return 2;
            }
        }
        override public int AnHighNegSupport
        {
            get
            {
                return 1;
            }
        }
        override public int AnHighPosSupport
        {
            get
            {
                return 1;
            }
        }
        override public int SynLowNegSupport
        {
            get
            {
                return 1;
            }
        }
        override public int SynLowPosSupport
        {
            get
            {
                return 1;
            }
        }
        override public int SynHighNegSupport
        {
            get
            {
                return 2;
            }
        }
        override public int SynHighPosSupport
        {
            get
            {
                return 2;
            }
        }
        override public int ImplType
        {
            get
            {
                return Syncfusion.Pdf.JPEG2000.wavelet.WaveletFilter_Fields.WT_FILTER_INT_LIFT;
            }
        }
        override public bool Reversible
        {
            get
            {
                return true;
            }
        }
        override public int FilterType
        {
            get
            {
                return Syncfusion.Pdf.JPEG2000.wavelet.FilterTypes_Fields.W5X3;
            }
        }
        private static readonly float[] LPSynthesisFilter = new float[] { 0.5f, 1f, 0.5f };
        private static readonly float[] HPSynthesisFilter = new float[] { -0.125f, -0.25f, 0.75f, -0.25f, -0.125f };
        public override void analyze_lpf(int[] inSig, int inOff, int inLen, int inStep, int[] lowSig, int lowOff, int lowStep, int[] highSig, int highOff, int highStep)
        {
            int i;
            int iStep = 2 * inStep;
            int ik;
            int lk;
            int hk;
            ik = inOff + inStep;
            hk = highOff;
            for (i = 1; i < inLen - 1; i += 2)
            {
                highSig[hk] = inSig[ik] - ((inSig[ik - inStep] + inSig[ik + inStep]) >> 1);
                ik += iStep;
                hk += highStep;
            }
            if (inLen % 2 == 0)
            {
                highSig[hk] = inSig[ik] - ((2 * inSig[ik - inStep]) >> 1);
            }
            ik = inOff;
            lk = lowOff;
            hk = highOff;
            if (inLen > 1)
            {
                lowSig[lk] = inSig[ik] + ((highSig[hk] + 1) >> 1);
            }
            else
            {
                lowSig[lk] = inSig[ik];
            }
            ik += iStep;
            lk += lowStep;
            hk += highStep;
            for (i = 2; i < inLen - 1; i += 2)
            {
                lowSig[lk] = inSig[ik] + ((highSig[hk - highStep] + highSig[hk] + 2) >> 2);
                ik += iStep;
                lk += lowStep;
                hk += highStep;
            }
            if (inLen % 2 == 1)
            {
                if (inLen > 2)
                {
                    lowSig[lk] = inSig[ik] + ((2 * highSig[hk - highStep] + 2) >> 2);
                }
            }
        }
        public override void analyze_hpf(int[] inSig, int inOff, int inLen, int inStep, int[] lowSig, int lowOff, int lowStep, int[] highSig, int highOff, int highStep)
        {
            int i;
            int iStep = 2 * inStep;
            int ik;
            int lk;
            int hk;
            ik = inOff;
            hk = highOff;
            if (inLen > 1)
            {
                highSig[hk] = inSig[ik] - inSig[ik + inStep];
            }
            else
            {
                highSig[hk] = inSig[ik] << 1;
            }
            ik += iStep;
            hk += highStep;
            if (inLen > 3)
            {
                for (i = 2; i < inLen - 1; i += 2)
                {
                    highSig[hk] = inSig[ik] - ((inSig[ik - inStep] + inSig[ik + inStep]) >> 1);
                    ik += iStep;
                    hk += highStep;
                }
            }
            if (inLen % 2 == 1 && inLen > 1)
            {
                highSig[hk] = inSig[ik] - inSig[ik - inStep];
            }
            ik = inOff + inStep;
            lk = lowOff;
            hk = highOff;
            for (i = 1; i < inLen - 1; i += 2)
            {
                lowSig[lk] = inSig[ik] + ((highSig[hk] + highSig[hk + highStep] + 2) >> 2);
                ik += iStep;
                lk += lowStep;
                hk += highStep;
            }
            if (inLen > 1 && inLen % 2 == 0)
            {
                lowSig[lk] = inSig[ik] + ((2 * highSig[hk] + 2) >> 2);
            }
        }
        public override float[] getLPSynthesisFilter()
        {
            return LPSynthesisFilter;
        }
        public override float[] getHPSynthesisFilter()
        {
            return HPSynthesisFilter;
        }
        public override bool isSameAsFullWT(int tailOvrlp, int headOvrlp, int inLen)
        {
            if (inLen % 2 == 0)
            {
                if (tailOvrlp >= 2 && headOvrlp >= 1)
                    return true;
                else
                    return false;
            }
            else
            {
                if (tailOvrlp >= 2 && headOvrlp >= 2)
                    return true;
                else
                    return false;
            }
        }
        public override bool Equals(System.Object obj)
        {
            return obj == this || obj is AnWTFilterIntLift5x3;
        }
        public override System.String ToString()
        {
            return "w5x3";
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
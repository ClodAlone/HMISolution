#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.wavelet.analysis
{
    internal class AnWTFilterFloatLift9x7 : AnWTFilterFloat
    {
        override public int AnLowNegSupport
        {
            get
            {
                return 4;
            }
        }
        override public int AnLowPosSupport
        {
            get
            {
                return 4;
            }
        }
        override public int AnHighNegSupport
        {
            get
            {
                return 3;
            }
        }
        override public int AnHighPosSupport
        {
            get
            {
                return 3;
            }
        }
        override public int SynLowNegSupport
        {
            get
            {
                return 3;
            }
        }
        override public int SynLowPosSupport
        {
            get
            {
                return 3;
            }
        }
        override public int SynHighNegSupport
        {
            get
            {
                return 4;
            }
        }
        override public int SynHighPosSupport
        {
            get
            {
                return 4;
            }
        }
        override public int ImplType
        {
            get
            {
                return Syncfusion.Pdf.JPEG2000.wavelet.WaveletFilter_Fields.WT_FILTER_FLOAT_LIFT;
            }
        }
        override public bool Reversible
        {
            get
            {
                return false;
            }
        }
        override public int FilterType
        {
            get
            {
                return Syncfusion.Pdf.JPEG2000.wavelet.FilterTypes_Fields.W9X7;
            }
        }
        private static readonly float[] LPSynthesisFilter = new float[] { -0.091272f, -0.057544f, 0.591272f, 1.115087f, 0.591272f, -0.057544f, -0.091272f };
        private static readonly float[] HPSynthesisFilter = new float[] { 0.026749f, 0.016864f, -0.078223f, -0.266864f, 0.602949f, -0.266864f, -0.078223f, 0.016864f, 0.026749f };
        public const float ALPHA = -1.586134342f;
        public const float BETA = -0.05298011854f;
        public const float GAMMA = 0.8829110762f;
        public const float DELTA = 0.4435068522f;
        public const float KL = 0.8128930655f;
        public const float KH = 1.230174106f;
        public override void analyze_lpf(float[] inSig, int inOff, int inLen, int inStep, float[] lowSig, int lowOff, int lowStep, float[] highSig, int highOff, int highStep)
        {
            int i, maxi;
            int iStep = 2 * inStep;
            int ik;
            int lk;
            int hk;
            ik = inOff + inStep;
            lk = lowOff;
            hk = highOff;
            for (i = 1, maxi = inLen - 1; i < maxi; i += 2)
            {
                highSig[hk] = inSig[ik] + ALPHA * (inSig[ik - inStep] + inSig[ik + inStep]);
                ik += iStep;
                hk += highStep;
            }
            if (inLen % 2 == 0)
            {
                highSig[hk] = inSig[ik] + 2 * ALPHA * inSig[ik - inStep];
            }
            ik = inOff;
            lk = lowOff;
            hk = highOff;
            if (inLen > 1)
            {
                lowSig[lk] = inSig[ik] + 2 * BETA * highSig[hk];
            }
            else
            {
                lowSig[lk] = inSig[ik];
            }
            ik += iStep;
            lk += lowStep;
            hk += highStep;
            for (i = 2, maxi = inLen - 1; i < maxi; i += 2)
            {
                lowSig[lk] = inSig[ik] + BETA * (highSig[hk - highStep] + highSig[hk]);
                ik += iStep;
                lk += lowStep;
                hk += highStep;
            }
            if ((inLen % 2 == 1) && (inLen > 2))
            {
                lowSig[lk] = inSig[ik] + 2 * BETA * highSig[hk - highStep];
            }
            lk = lowOff;
            hk = highOff;
            for (i = 1, maxi = inLen - 1; i < maxi; i += 2)
            {
                highSig[hk] += GAMMA * (lowSig[lk] + lowSig[lk + lowStep]);
                lk += lowStep;
                hk += highStep;
            }
            if (inLen % 2 == 0)
            {
                highSig[hk] += 2 * GAMMA * lowSig[lk];
            }
            lk = lowOff;
            hk = highOff;
            if (inLen > 1)
            {
                lowSig[lk] += 2 * DELTA * highSig[hk];
            }
            lk += lowStep;
            hk += highStep;
            for (i = 2, maxi = inLen - 1; i < maxi; i += 2)
            {
                lowSig[lk] += DELTA * (highSig[hk - highStep] + highSig[hk]);
                lk += lowStep;
                hk += highStep;
            }
            if ((inLen % 2 == 1) && (inLen > 2))
            {
                lowSig[lk] += 2 * DELTA * highSig[hk - highStep];
            }
            lk = lowOff;
            hk = highOff;
            for (i = 0; i < (inLen >> 1); i++)
            {
                lowSig[lk] *= KL;
                highSig[hk] *= KH;
                lk += lowStep;
                hk += highStep;
            }
            if (inLen % 2 == 1 && inLen != 1)
            {
                lowSig[lk] *= KL;
            }
        }
        public override void analyze_hpf(float[] inSig, int inOff, int inLen, int inStep, float[] lowSig, int lowOff, int lowStep, float[] highSig, int highOff, int highStep)
        {
            int i;
            int iStep = 2 * inStep;
            int ik;
            int lk;
            int hk;
            ik = inOff;
            lk = lowOff;
            hk = highOff;
            if (inLen > 1)
            {
                highSig[hk] = inSig[ik] + 2 * ALPHA * inSig[ik + inStep];
            }
            else
            {
                highSig[hk] = inSig[ik] * 2;
            }
            ik += iStep;
            hk += highStep;
            for (i = 2; i < inLen - 1; i += 2)
            {
                highSig[hk] = inSig[ik] + ALPHA * (inSig[ik - inStep] + inSig[ik + inStep]);
                ik += iStep;
                hk += highStep;
            }
            if ((inLen % 2 == 1) && (inLen > 1))
            {
                highSig[hk] = inSig[ik] + 2 * ALPHA * inSig[ik - inStep];
            }
            ik = inOff + inStep;
            lk = lowOff;
            hk = highOff;
            for (i = 1; i < inLen - 1; i += 2)
            {
                lowSig[lk] = inSig[ik] + BETA * (highSig[hk] + highSig[hk + highStep]);
                ik += iStep;
                lk += lowStep;
                hk += highStep;
            }
            if (inLen > 1 && inLen % 2 == 0)
            {
                lowSig[lk] = inSig[ik] + 2 * BETA * highSig[hk];
            }
            lk = lowOff;
            hk = highOff;
            if (inLen > 1)
            {
                highSig[hk] += GAMMA * 2 * lowSig[lk];
            }
            hk += highStep;
            for (i = 2; i < inLen - 1; i += 2)
            {
                highSig[hk] += GAMMA * (lowSig[lk] + lowSig[lk + lowStep]);
                lk += lowStep;
                hk += highStep;
            }
            if (inLen > 1 && inLen % 2 == 1)
            {
                highSig[hk] += GAMMA * 2 * lowSig[lk];
            }
            lk = lowOff;
            hk = highOff;
            for (i = 1; i < inLen - 1; i += 2)
            {
                lowSig[lk] += DELTA * (highSig[hk] + highSig[hk + highStep]);
                lk += lowStep;
                hk += highStep;
            }
            if (inLen > 1 && inLen % 2 == 0)
            {
                lowSig[lk] += DELTA * 2 * highSig[hk];
            }
            lk = lowOff;
            hk = highOff;
            for (i = 0; i < (inLen >> 1); i++)
            {
                lowSig[lk] *= KL;
                highSig[hk] *= KH;
                lk += lowStep;
                hk += highStep;
            }
            if (inLen % 2 == 1 && inLen != 1)
            {
                highSig[hk] *= KH;
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
                if (tailOvrlp >= 4 && headOvrlp >= 3)
                    return true;
                else
                    return false;
            }
            else
            {
                if (tailOvrlp >= 4 && headOvrlp >= 4)
                    return true;
                else
                    return false;
            }
        }
        public override bool Equals(System.Object obj)
        {
            return obj == this || obj is AnWTFilterFloatLift9x7;
        }
        public override System.String ToString()
        {
            return "w9x7";
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
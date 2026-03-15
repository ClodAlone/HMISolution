#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.wavelet.synthesis
{
    internal class SynWTFilterFloatLift9x7 : SynWTFilterFloat
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
        public const float ALPHA = -1.586134342f;
        public const float BETA = -0.05298011854f;
        public const float GAMMA = 0.8829110762f;
        public const float DELTA = 0.4435068522f;
        public const float KL = 0.8128930655f;
        public const float KH = 1.230174106f;
        public override void synthetize_lpf(float[] lowSig, int lowOff, int lowLen, int lowStep, float[] highSig, int highOff, int highLen, int highStep, float[] outSig, int outOff, int outStep)
        {
            int i;
            int outLen = lowLen + highLen;
            int iStep = 2 * outStep;
            int ik;
            int lk;
            int hk;
            lk = lowOff;
            hk = highOff;
            ik = outOff;
            if (outLen > 1)
            {
                outSig[ik] = lowSig[lk] / KL - 2 * DELTA * highSig[hk] / KH;
            }
            else
            {
                outSig[ik] = lowSig[lk];
            }
            lk += lowStep;
            hk += highStep;
            ik += iStep;
            for (i = 2; i < outLen - 1; i += 2, ik += iStep, lk += lowStep, hk += highStep)
            {
                outSig[ik] = lowSig[lk] / KL - DELTA * (highSig[hk - highStep] + highSig[hk]) / KH;
            }
            if (outLen % 2 == 1)
            {
                if (outLen > 2)
                {
                    outSig[ik] = lowSig[lk] / KL - 2 * DELTA * highSig[hk - highStep] / KH;
                }
            }
            lk = lowOff;
            hk = highOff;
            ik = outOff + outStep;
            for (i = 1; i < outLen - 1; i += 2, ik += iStep, hk += highStep, lk += lowStep)
            {
                outSig[ik] = highSig[hk] / KH - GAMMA * (outSig[ik - outStep] + outSig[ik + outStep]);
            }
            if (outLen % 2 == 0)
            {
                outSig[ik] = highSig[hk] / KH - 2 * GAMMA * outSig[ik - outStep];
            }
            ik = outOff;
            if (outLen > 1)
            {
                outSig[ik] -= 2 * BETA * outSig[ik + outStep];
            }
            ik += iStep;
            for (i = 2; i < outLen - 1; i += 2, ik += iStep)
            {
                outSig[ik] -= BETA * (outSig[ik - outStep] + outSig[ik + outStep]);
            }
            if (outLen % 2 == 1 && outLen > 2)
            {
                outSig[ik] -= 2 * BETA * outSig[ik - outStep];
            }
            ik = outOff + outStep;
            for (i = 1; i < outLen - 1; i += 2, ik += iStep)
            {
                outSig[ik] -= ALPHA * (outSig[ik - outStep] + outSig[ik + outStep]);
            }
            if (outLen % 2 == 0)
            {
                outSig[ik] -= 2 * ALPHA * outSig[ik - outStep];
            }
        }
        public override void synthetize_hpf(float[] lowSig, int lowOff, int lowLen, int lowStep, float[] highSig, int highOff, int highLen, int highStep, float[] outSig, int outOff, int outStep)
        {
            int i;
            int outLen = lowLen + highLen;
            int iStep = 2 * outStep;
            int ik;
            int lk;
            int hk;
            lk = lowOff;
            hk = highOff;
            if (outLen != 1)
            {
                int outLen2 = outLen >> 1;
                for (i = 0; i < outLen2; i++)
                {
                    lowSig[lk] /= KL;
                    highSig[hk] /= KH;
                    lk += lowStep;
                    hk += highStep;
                }
                if (outLen % 2 == 1)
                {
                    highSig[hk] /= KH;
                }
            }
            else
            {
                highSig[highOff] /= 2;
            }
            lk = lowOff;
            hk = highOff;
            ik = outOff + outStep;
            for (i = 1; i < outLen - 1; i += 2)
            {
                outSig[ik] = lowSig[lk] - DELTA * (highSig[hk] + highSig[hk + highStep]);
                ik += iStep;
                lk += lowStep;
                hk += highStep;
            }
            if (outLen % 2 == 0 && outLen > 1)
            {
                outSig[ik] = lowSig[lk] - 2 * DELTA * highSig[hk];
            }
            hk = highOff;
            ik = outOff;
            if (outLen > 1)
            {
                outSig[ik] = highSig[hk] - 2 * GAMMA * outSig[ik + outStep];
            }
            else
            {
                outSig[ik] = highSig[hk];
            }
            ik += iStep;
            hk += highStep;
            for (i = 2; i < outLen - 1; i += 2)
            {
                outSig[ik] = highSig[hk] - GAMMA * (outSig[ik - outStep] + outSig[ik + outStep]);
                ik += iStep;
                hk += highStep;
            }
            if (outLen % 2 == 1 && outLen > 1)
            {
                outSig[ik] = highSig[hk] - 2 * GAMMA * outSig[ik - outStep];
            }
            ik = outOff + outStep;
            for (i = 1; i < outLen - 1; i += 2)
            {
                outSig[ik] -= BETA * (outSig[ik - outStep] + outSig[ik + outStep]);
                ik += iStep;
            }
            if (outLen % 2 == 0 && outLen > 1)
            {
                outSig[ik] -= 2 * BETA * outSig[ik - outStep];
            }
            ik = outOff;
            if (outLen > 1)
            {
                outSig[ik] -= 2 * ALPHA * outSig[ik + outStep];
            }
            ik += iStep;
            for (i = 2; i < outLen - 1; i += 2)
            {
                outSig[ik] -= ALPHA * (outSig[ik - outStep] + outSig[ik + outStep]);
                ik += iStep;
            }
            if ((outLen % 2 == 1) && (outLen > 1))
            {
                outSig[ik] -= 2 * ALPHA * outSig[ik - outStep];
            }
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
        public override System.String ToString()
        {
            return "w9x7 (lifting)";
        }
    }
}
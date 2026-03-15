#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.wavelet.synthesis
{
    internal class SynWTFilterIntLift5x3 : SynWTFilterInt
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
        public override void synthetize_lpf(int[] lowSig, int lowOff, int lowLen, int lowStep, int[] highSig, int highOff, int highLen, int highStep, int[] outSig, int outOff, int outStep)
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
                outSig[ik] = lowSig[lk] - ((highSig[hk] + 1) >> 1);
            }
            else
            {
                outSig[ik] = lowSig[lk];
            }
            lk += lowStep;
            hk += highStep;
            ik += iStep;
            for (i = 2; i < outLen - 1; i += 2)
            {
                outSig[ik] = lowSig[lk] - ((highSig[hk - highStep] + highSig[hk] + 2) >> 2);
                lk += lowStep;
                hk += highStep;
                ik += iStep;
            }
            if ((outLen % 2 == 1) && (outLen > 2))
            {
                outSig[ik] = lowSig[lk] - ((2 * highSig[hk - highStep] + 2) >> 2);
            }
            hk = highOff;
            ik = outOff + outStep;
            for (i = 1; i < outLen - 1; i += 2)
            {
                outSig[ik] = highSig[hk] + ((outSig[ik - outStep] + outSig[ik + outStep]) >> 1);
                hk += highStep;
                ik += iStep;
            }
            if (outLen % 2 == 0 && outLen > 1)
            {
                outSig[ik] = highSig[hk] + outSig[ik - outStep];
            }
        }
        public override void synthetize_hpf(int[] lowSig, int lowOff, int lowLen, int lowStep, int[] highSig, int highOff, int highLen, int highStep, int[] outSig, int outOff, int outStep)
        {
            int i;
            int outLen = lowLen + highLen;
            int iStep = 2 * outStep;
            int ik;
            int lk;
            int hk;
            lk = lowOff;
            hk = highOff;
            ik = outOff + outStep;
            for (i = 1; i < outLen - 1; i += 2)
            {
                outSig[ik] = lowSig[lk] - ((highSig[hk] + highSig[hk + highStep] + 2) >> 2);
                lk += lowStep;
                hk += highStep;
                ik += iStep;
            }
            if ((outLen > 1) && (outLen % 2 == 0))
            {
                outSig[ik] = lowSig[lk] - ((2 * highSig[hk] + 2) >> 2);
            }
            hk = highOff;
            ik = outOff;
            if (outLen > 1)
            {
                outSig[ik] = highSig[hk] + outSig[ik + outStep];
            }
            else
            {
                outSig[ik] = highSig[hk] >> 1;
            }
            hk += highStep;
            ik += iStep;
            for (i = 2; i < outLen - 1; i += 2)
            {
                outSig[ik] = highSig[hk] + ((outSig[ik - outStep] + outSig[ik + outStep]) >> 1);
                hk += highStep;
                ik += iStep;
            }
            if (outLen % 2 == 1 && outLen > 1)
            {
                outSig[ik] = highSig[hk] + outSig[ik - outStep];
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
            return "w5x3 (lifting)";
        }
    }
}
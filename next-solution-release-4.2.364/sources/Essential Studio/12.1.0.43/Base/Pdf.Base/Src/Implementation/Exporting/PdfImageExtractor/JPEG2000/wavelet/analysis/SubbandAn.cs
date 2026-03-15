#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.wavelet.analysis
{
    internal class SubbandAn : Subband
    {
        override public Subband Parent
        {
            get
            {
                return parentband;
            }
        }
        override public Subband LL
        {
            get
            {
                return subb_LL;
            }
        }
        override public Subband HL
        {
            get
            {
                return subb_HL;
            }
        }
        override public Subband LH
        {
            get
            {
                return subb_LH;
            }
        }
        override public Subband HH
        {
            get
            {
                return subb_HH;
            }
        }
        override internal WaveletFilter HorWFilter
        {
            get
            {
                return hFilter;
            }
        }
        override internal WaveletFilter VerWFilter
        {
            get
            {
                return hFilter;
            }
        }
        public SubbandAn parentband = null;
        public SubbandAn subb_LL;
        public SubbandAn subb_HL;
        public SubbandAn subb_LH;
        public SubbandAn subb_HH;
        public AnWTFilter hFilter;
        public AnWTFilter vFilter;
        public float l2Norm = -1.0f;
        public float stepWMSE;
        public SubbandAn()
        {
        }
        internal SubbandAn(int w, int h, int ulcx, int ulcy, int lvls, WaveletFilter[] hfilters, WaveletFilter[] vfilters)
            : base(w, h, ulcx, ulcy, lvls, hfilters, vfilters)
        {
            calcL2Norms();
        }
        internal override Subband split(WaveletFilter hfilter, WaveletFilter vfilter)
        {
            if (isNode)
            {
                throw new System.ArgumentException();
            }
            isNode = true;
            this.hFilter = (AnWTFilter)hfilter;
            this.vFilter = (AnWTFilter)vfilter;
            subb_LL = new SubbandAn();
            subb_LH = new SubbandAn();
            subb_HL = new SubbandAn();
            subb_HH = new SubbandAn();
            subb_LL.parentband = this;
            subb_HL.parentband = this;
            subb_LH.parentband = this;
            subb_HH.parentband = this;
            initChilds();
            return subb_LL;
        }
        private void calcBasisWaveForms(float[][] wfs)
        {
            if (l2Norm < 0)
            {
                if (isNode)
                {
                    if (subb_LL.l2Norm < 0f)
                    {
                        subb_LL.calcBasisWaveForms(wfs);
                        wfs[0] = hFilter.getLPSynWaveForm(wfs[0], null);
                        wfs[1] = vFilter.getLPSynWaveForm(wfs[1], null);
                    }
                    else if (subb_HL.l2Norm < 0f)
                    {
                        subb_HL.calcBasisWaveForms(wfs);
                        wfs[0] = hFilter.getHPSynWaveForm(wfs[0], null);
                        wfs[1] = vFilter.getLPSynWaveForm(wfs[1], null);
                    }
                    else if (subb_LH.l2Norm < 0f)
                    {
                        subb_LH.calcBasisWaveForms(wfs);
                        wfs[0] = hFilter.getLPSynWaveForm(wfs[0], null);
                        wfs[1] = vFilter.getHPSynWaveForm(wfs[1], null);
                    }
                    else if (subb_HH.l2Norm < 0f)
                    {
                        subb_HH.calcBasisWaveForms(wfs);
                        wfs[0] = hFilter.getHPSynWaveForm(wfs[0], null);
                        wfs[1] = vFilter.getHPSynWaveForm(wfs[1], null);
                    }
                    else
                    {
                        //throw new System.ApplicationException("You have found a bug in JJ2000!");
                    }
                }
                else
                {
                    wfs[0] = new float[1];
                    wfs[0][0] = 1.0f;
                    wfs[1] = new float[1];
                    wfs[1][0] = 1.0f;
                }
            }
            else
            {
                //throw new System.ApplicationException("You have found a bug in JJ2000!");
            }
        }
        private void assignL2Norm(float l2n)
        {
            if (l2Norm < 0)
            {
                if (isNode)
                {
                    if (subb_LL.l2Norm < 0f)
                    {
                        subb_LL.assignL2Norm(l2n);
                    }
                    else if (subb_HL.l2Norm < 0f)
                    {
                        subb_HL.assignL2Norm(l2n);
                    }
                    else if (subb_LH.l2Norm < 0f)
                    {
                        subb_LH.assignL2Norm(l2n);
                    }
                    else if (subb_HH.l2Norm < 0f)
                    {
                        subb_HH.assignL2Norm(l2n);
                        if (subb_HH.l2Norm >= 0f)
                        {
                            l2Norm = 0f;
                        }
                    }
                    else
                    {
                        //throw new System.ApplicationException("You have found a bug in JJ2000!");
                    }
                }
                else
                {
                    l2Norm = l2n;
                }
            }
            else
            {
                //throw new System.ApplicationException("You have found a bug in JJ2000!");
            }
        }
        private void calcL2Norms()
        {
            int i;
            float[][] wfs = new float[2][];
            double acc;
            float l2n;
            while (l2Norm < 0f)
            {
                calcBasisWaveForms(wfs);
                acc = 0.0;
                for (i = wfs[0].Length - 1; i >= 0; i--)
                {
                    acc += wfs[0][i] * wfs[0][i];
                }
                l2n = (float)System.Math.Sqrt(acc);
                acc = 0.0;
                for (i = wfs[1].Length - 1; i >= 0; i--)
                {
                    acc += wfs[1][i] * wfs[1][i];
                }
                l2n *= (float)System.Math.Sqrt(acc);
                wfs[0] = null;
                wfs[1] = null;
                assignL2Norm(l2n);
            }
        }
    }
}
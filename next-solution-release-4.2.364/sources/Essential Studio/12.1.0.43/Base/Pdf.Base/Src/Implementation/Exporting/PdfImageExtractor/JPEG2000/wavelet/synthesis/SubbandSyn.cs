#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.wavelet.synthesis
{
    public class SubbandSyn : Subband
    {
        override public Subband Parent
        {
            get
            {
                return parent;
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
        private SubbandSyn parent;
        private SubbandSyn subb_LL;
        private SubbandSyn subb_HL;
        private SubbandSyn subb_LH;
        private SubbandSyn subb_HH;
        public SynWTFilter hFilter;
        public SynWTFilter vFilter;
        public int magbits = 0;
        public SubbandSyn()
        {
        }
        internal SubbandSyn(int w, int h, int ulcx, int ulcy, int lvls, WaveletFilter[] hfilters, WaveletFilter[] vfilters)
            : base(w, h, ulcx, ulcy, lvls, hfilters, vfilters)
        {
        }
        internal override Subband split(WaveletFilter hfilter, WaveletFilter vfilter)
        {
            if (isNode)
            {
                throw new System.ArgumentException();
            }
            isNode = true;
            this.hFilter = (SynWTFilter)hfilter;
            this.vFilter = (SynWTFilter)vfilter;
            subb_LL = new SubbandSyn();
            subb_LH = new SubbandSyn();
            subb_HL = new SubbandSyn();
            subb_HH = new SubbandSyn();
            subb_LL.parent = this;
            subb_HL.parent = this;
            subb_LH.parent = this;
            subb_HH.parent = this;
            initChilds();
            return subb_LL;
        }
    }
}
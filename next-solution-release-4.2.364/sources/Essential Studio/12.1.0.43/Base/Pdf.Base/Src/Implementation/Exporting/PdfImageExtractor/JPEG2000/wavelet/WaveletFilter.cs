#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.wavelet
{
    public struct WaveletFilter_Fields
    {
        public readonly static int WT_FILTER_INT_LIFT = 0;
        public readonly static int WT_FILTER_FLOAT_LIFT = 1;
        public readonly static int WT_FILTER_FLOAT_CONVOL = 2;
    }
    internal interface WaveletFilter
    {
        int AnLowNegSupport
        {
            get;
        }
        int AnLowPosSupport
        {
            get;
        }
        int AnHighNegSupport
        {
            get;
        }
        int AnHighPosSupport
        {
            get;
        }
        int SynLowNegSupport
        {
            get;
        }
        int SynLowPosSupport
        {
            get;
        }
        int SynHighNegSupport
        {
            get;
        }
        int SynHighPosSupport
        {
            get;
        }
        int ImplType
        {
            get;
        }
        int DataType
        {
            get;
        }
        bool Reversible
        {
            get;
        }
        bool isSameAsFullWT(int tailOvrlp, int headOvrlp, int inLen);
    }
}
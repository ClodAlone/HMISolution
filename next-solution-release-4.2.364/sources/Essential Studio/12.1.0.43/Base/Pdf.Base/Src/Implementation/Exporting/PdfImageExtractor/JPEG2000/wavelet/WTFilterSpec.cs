#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.wavelet
{
    internal abstract class WTFilterSpec
    {
        public abstract int WTDataType { get; }
        public const byte FILTER_SPEC_MAIN_DEF = 0;
        public const byte FILTER_SPEC_COMP_DEF = 1;
        public const byte FILTER_SPEC_TILE_DEF = 2;
        public const byte FILTER_SPEC_TILE_COMP = 3;
        internal byte[] specValType;
        internal WTFilterSpec(int nc)
        {
            specValType = new byte[nc];
        }
        public virtual byte getKerSpecType(int n)
        {
            return specValType[n];
        }
    }
}
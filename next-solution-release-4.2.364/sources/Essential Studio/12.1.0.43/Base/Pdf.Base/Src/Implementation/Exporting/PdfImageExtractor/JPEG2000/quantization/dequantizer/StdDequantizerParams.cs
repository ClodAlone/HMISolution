#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.quantization.dequantizer
{
    internal class StdDequantizerParams : DequantizerParams
    {
        override public int DequantizerType
        {
            get
            {
                return Syncfusion.Pdf.JPEG2000.quantization.QuantizationType_Fields.Q_TYPE_SCALAR_DZ;
            }
        }
        public int[][] exp;
        public float[][] nStep;
    }
}
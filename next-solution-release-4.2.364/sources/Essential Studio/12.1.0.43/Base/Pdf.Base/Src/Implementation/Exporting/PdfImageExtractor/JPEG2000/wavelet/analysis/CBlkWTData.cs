#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.image;
namespace Syncfusion.Pdf.JPEG2000.wavelet.analysis
{
    public abstract class CBlkWTData
    {
        public abstract int DataType { get; }
        public abstract System.Object Data { get; set; }
        public int ulx;
        public int uly;
        public int n;
        public int m;
        internal SubbandAn sb;
        public int w;
        public int h;
        public int offset;
        public int scanw;
        public int magbits;
        public float wmseScaling = 1f;
        public double convertFactor = 1.0;
        public double stepSize = 1.0;
        public int nROIcoeff = 0;
        public int nROIbp = 0;
        public override System.String ToString()
        {
            System.String typeString = "";
            switch (DataType)
            {
                case DataBlock.TYPE_BYTE:
                    typeString = "Unsigned Byte";
                    break;
                case DataBlock.TYPE_SHORT:
                    typeString = "Short";
                    break;
                case DataBlock.TYPE_INT:
                    typeString = "Integer";
                    break;
                case DataBlock.TYPE_FLOAT:
                    typeString = "Float";
                    break;
            }
            return "ulx=" + ulx + ", uly=" + uly + ", idx=(" + m + "," + n + "), w=" + w + ", h=" + h + ", off=" + offset + ", scanw=" + scanw + ", wmseScaling=" + wmseScaling + ", convertFactor=" + convertFactor + ", stepSize=" + stepSize + ", type=" + typeString + ", magbits=" + magbits + ", nROIcoeff=" + nROIcoeff + ", nROIbp=" + nROIbp;
        }
    }
}
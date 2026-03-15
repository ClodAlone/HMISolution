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
    internal class CBlkWTDataInt : CBlkWTData
    {
        override public int DataType
        {
            get
            {
                return DataBlock.TYPE_INT;
            }
        }
        override public System.Object Data
        {
            get
            {
                return data_array;
            }
            set
            {
                data_array = (int[])value;
            }
        }
        virtual public int[] DataInt
        {
            get
            {
                return data_array;
            }
            set
            {
                data_array = value;
            }
        }
        public int[] data_array;
    }
}
#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
namespace Syncfusion.Pdf.JPEG2000.image
{
    internal class DataBlockInt : DataBlock
    {
        override public int DataType
        {
            get
            {
                return TYPE_INT;
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
        public DataBlockInt()
        {
        }
        public DataBlockInt(int ulx, int uly, int w, int h)
        {
            this.ulx = ulx;
            this.uly = uly;
            this.w = w;
            this.h = h;
            offset = 0;
            scanw = w;
            data_array = new int[w * h];
        }
        public DataBlockInt(DataBlockInt src)
        {
            this.ulx = src.ulx;
            this.uly = src.uly;
            this.w = src.w;
            this.h = src.h;
            this.offset = 0;
            this.scanw = this.w;
            this.data_array = new int[this.w * this.h];
            for (int i = 0; i < this.h; i++)
                Array.Copy(src.data_array, i * src.scanw, this.data_array, i * this.scanw, this.w);
        }
        public override System.String ToString()
        {
            System.String str = base.ToString();
            if (data_array != null)
            {
                str += (",data=" + data_array.Length + " bytes");
            }
            return str;
        }
    }
}
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
    internal class DataBlockFloat : DataBlock
    {
        override public int DataType
        {
            get
            {
                return TYPE_FLOAT;
            }
        }
        override public System.Object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = (float[])value;
            }
        }
        virtual public float[] DataFloat
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }
        private float[] data;
        public DataBlockFloat()
        {
        }
        public DataBlockFloat(int ulx, int uly, int w, int h)
        {
            this.ulx = ulx;
            this.uly = uly;
            this.w = w;
            this.h = h;
            offset = 0;
            scanw = w;
            data = new float[w * h];
        }
        public DataBlockFloat(DataBlockFloat src)
        {
            this.ulx = src.ulx;
            this.uly = src.uly;
            this.w = src.w;
            this.h = src.h;
            this.offset = 0;
            this.scanw = this.w;
            this.data = new float[this.w * this.h];
            for (int i = 0; i < this.h; i++)
                Array.Copy(src.data, i * src.scanw, this.data, i * this.scanw, this.w);
        }
        public override System.String ToString()
        {
            System.String str = base.ToString();
            if (data != null)
            {
                str += (",data=" + data.Length + " bytes");
            }
            return str;
        }
    }
}
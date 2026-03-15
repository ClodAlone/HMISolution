#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.image
{
    public abstract class DataBlock
    {
        public abstract int DataType { get; }
        public abstract System.Object Data { get; set; }
        public const int TYPE_BYTE = 0;
        public const int TYPE_SHORT = 1;
        public const int TYPE_INT = 3;
        public const int TYPE_FLOAT = 4;
        public int ulx;
        public int uly;
        public int w;
        public int h;
        public int offset;
        public int scanw;
        public bool progressive;
        public static int getSize(int type)
        {
            switch (type)
            {
                case TYPE_BYTE:
                    return 8;
                case TYPE_SHORT:
                    return 16;
                case TYPE_INT:
                case TYPE_FLOAT:
                    return 32;
                default:
                    throw new System.ArgumentException();
            }
        }
        public override System.String ToString()
        {
            System.String typeString = "";
            switch (DataType)
            {
                case TYPE_BYTE:
                    typeString = "Unsigned Byte";
                    break;
                case TYPE_SHORT:
                    typeString = "Short";
                    break;
                case TYPE_INT:
                    typeString = "Integer";
                    break;
                case TYPE_FLOAT:
                    typeString = "Float";
                    break;
            }
            return "DataBlk: " + "upper-left(" + ulx + "," + uly + "), width=" + w + ", height=" + h + ", progressive=" + progressive + ", offset=" + offset + ", scanw=" + scanw + ", type=" + typeString;
        }
    }
}
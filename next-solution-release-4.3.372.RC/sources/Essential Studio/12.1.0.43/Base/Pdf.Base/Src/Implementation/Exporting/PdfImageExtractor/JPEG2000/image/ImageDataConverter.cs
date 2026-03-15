#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.image
{
    internal class ImageDataConverter : ImgDataAdapter, BlockImageDataSource
    {
        private DataBlock srcBlk = new DataBlockInt();
        private BlockImageDataSource src;
        private int fp;
        internal ImageDataConverter(BlockImageDataSource imgSrc, int fp)
            : base(imgSrc)
        {
            src = imgSrc;
            this.fp = fp;
        }
        internal ImageDataConverter(BlockImageDataSource imgSrc)
            : base(imgSrc)
        {
            src = imgSrc;
            fp = 0;
        }
        public virtual int getFixedPoint(int c)
        {
            return fp;
        }
        public virtual DataBlock getCompData(DataBlock blk, int c)
        {
            return getData(blk, c, false);
        }
        public DataBlock getInternCompData(DataBlock blk, int c)
        {
            return getData(blk, c, true);
        }
        private DataBlock getData(DataBlock blk, int c, bool intern)
        {
            DataBlock reqBlk;
            int otype = blk.DataType;
            if (otype == srcBlk.DataType)
            {
                reqBlk = blk;
            }
            else
            {
                reqBlk = srcBlk;
                reqBlk.ulx = blk.ulx;
                reqBlk.uly = blk.uly;
                reqBlk.w = blk.w;
                reqBlk.h = blk.h;
            }
            if (intern)
            {
                srcBlk = src.getInternCompData(reqBlk, c);
            }
            else
            {
                srcBlk = src.getCompData(reqBlk, c);
            }
            if (srcBlk.DataType == otype)
            {
                return srcBlk;
            }
            int i;
            int k, kSrc, kmin;
            float mult;
            int w = srcBlk.w;
            int h = srcBlk.h;
            switch (otype)
            {
                case DataBlock.TYPE_FLOAT:
                    float[] farr;
                    int[] srcIArr;
                    farr = (float[])blk.Data;
                    if (farr == null || farr.Length < w * h)
                    {
                        farr = new float[w * h];
                        blk.Data = farr;
                    }
                    blk.scanw = srcBlk.w;
                    blk.offset = 0;
                    blk.progressive = srcBlk.progressive;
                    srcIArr = (int[])srcBlk.Data;
                    fp = src.getFixedPoint(c);
                    if (fp != 0)
                    {
                        mult = 1.0f / (1 << fp);
                        for (i = h - 1, k = w * h - 1, kSrc = srcBlk.offset + (h - 1) * srcBlk.scanw + w - 1; i >= 0; i--)
                        {
                            for (kmin = k - w; k > kmin; k--, kSrc--)
                            {
                                farr[k] = ((srcIArr[kSrc] * mult));
                            }
                            kSrc -= (srcBlk.scanw - w);
                        }
                    }
                    else
                    {
                        for (i = h - 1, k = w * h - 1, kSrc = srcBlk.offset + (h - 1) * srcBlk.scanw + w - 1; i >= 0; i--)
                        {
                            for (kmin = k - w; k > kmin; k--, kSrc--)
                            {
                                farr[k] = ((float)(srcIArr[kSrc]));
                            }
                            kSrc -= (srcBlk.scanw - w);
                        }
                    }
                    break;
                case DataBlock.TYPE_INT:
                    int[] iarr;
                    float[] srcFArr;
                    iarr = (int[])blk.Data;
                    if (iarr == null || iarr.Length < w * h)
                    {
                        iarr = new int[w * h];
                        blk.Data = iarr;
                    }
                    blk.scanw = srcBlk.w;
                    blk.offset = 0;
                    blk.progressive = srcBlk.progressive;
                    srcFArr = (float[])srcBlk.Data;
                    if (fp != 0)
                    {
                        mult = (float)(1 << fp);
                        for (i = h - 1, k = w * h - 1, kSrc = srcBlk.offset + (h - 1) * srcBlk.scanw + w - 1; i >= 0; i--)
                        {
                            for (kmin = k - w; k > kmin; k--, kSrc--)
                            {
                                if (srcFArr[kSrc] > 0.0f)
                                {
                                    iarr[k] = (int)(srcFArr[kSrc] * mult + 0.5f);
                                }
                                else
                                {
                                    iarr[k] = (int)(srcFArr[kSrc] * mult - 0.5f);
                                }
                            }
                            kSrc -= (srcBlk.scanw - w);
                        }
                    }
                    else
                    {
                        for (i = h - 1, k = w * h - 1, kSrc = srcBlk.offset + (h - 1) * srcBlk.scanw + w - 1; i >= 0; i--)
                        {
                            for (kmin = k - w; k > kmin; k--, kSrc--)
                            {
                                if (srcFArr[kSrc] > 0.0f)
                                {
                                    iarr[k] = (int)(srcFArr[kSrc] + 0.5f);
                                }
                                else
                                {
                                    iarr[k] = (int)(srcFArr[kSrc] - 0.5f);
                                }
                            }
                            kSrc -= (srcBlk.scanw - w);
                        }
                    }
                    break;
                default:
                    throw new System.ArgumentException("Only integer and float data " + "are " + "supported by JJ2000");
            }
            return blk;
        }
    }
}
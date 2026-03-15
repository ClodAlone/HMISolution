#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Drawing;
using System.IO;
//using System.Drawing.Imaging;

namespace Syncfusion.Pdf
{
    class JBIG2Image
    {
        private int m_width, m_height, m_line;
        private BitArray data;
        private ArithmeticDecoder m_arithmeticDecoder;
        private HuffmanDecoder m_huffmanDecoder;
        private MMRDecoder m_mmrDecoder;
        private BitOperation m_bitOperation = new BitOperation();

        internal int BitmapNumber;
        internal JBIG2Image(int width, int height, ArithmeticDecoder arithmeticDecoder, HuffmanDecoder huffmanDecoder, MMRDecoder mmrDecoder)
        {
            this.m_width = width;
            this.m_height = height;
            this.m_arithmeticDecoder = arithmeticDecoder;
            this.m_huffmanDecoder = huffmanDecoder;
            this.m_mmrDecoder = mmrDecoder;

            this.m_line = (width + 7) >> 3;

            this.data = new BitArray(width * height);
        }

        internal int Width
        {
            get
            {
                return m_width;
            }
        }

        internal int Height
        {
            get
            {
                return m_height;
            }
        }
        internal void ReadBitmap(bool useMMR, int template, bool typicalPredictionGenericDecodingOn, bool useSkip, JBIG2Image skipBitmap, short[] adaptiveTemplateX, short[] adaptiveTemplateY, int mmrDataLength)
        {
            if (useMMR)
            {
                m_mmrDecoder.Reset();

                int[] referenceLine = new int[m_width + 2];
                int[] codingLine = new int[m_width + 2];
                codingLine[0] = codingLine[1] = m_width;

                for (int row = 0; row < m_height; row++)
                {

                    int i = 0;
                    for (; codingLine[i] < m_width; i++)
                    {
                        referenceLine[i] = codingLine[i];
                    }
                    referenceLine[i] = referenceLine[i + 1] = m_width;

                    int referenceI = 0;
                    int codingI = 0;
                    int a0 = 0;

                    do
                    {
                        int code1 = m_mmrDecoder.Get2DCode(), code2, code3;

                        switch (code1)
                        {
                            case MMRDecoder.TwoDimensionalPass:
                                if (referenceLine[referenceI] < m_width)
                                {
                                    a0 = referenceLine[referenceI + 1];
                                    referenceI += 2;
                                }
                                break;
                            case MMRDecoder.TwoDimensionalHorizontal:
                                if ((codingI & 1) != 0)
                                {
                                    code1 = 0;
                                    do
                                    {
                                        code1 += code3 = m_mmrDecoder.GetblackCode();
                                    } while (code3 >= 64);

                                    code2 = 0;
                                    do
                                    {
                                        code2 += code3 = m_mmrDecoder.GetWhiteCode();
                                    } while (code3 >= 64);
                                }
                                else
                                {
                                    code1 = 0;
                                    do
                                    {
                                        code1 += code3 = m_mmrDecoder.GetWhiteCode();
                                    } while (code3 >= 64);

                                    code2 = 0;
                                    do
                                    {
                                        code2 += code3 = m_mmrDecoder.GetblackCode();
                                    } while (code3 >= 64);

                                }
                                if (code1 > 0 || code2 > 0)
                                {
                                    a0 = codingLine[codingI++] = a0 + code1;
                                    a0 = codingLine[codingI++] = a0 + code2;

                                    while (referenceLine[referenceI] <= a0 && referenceLine[referenceI] < m_width)
                                    {
                                        referenceI += 2;
                                    }

                                }
                                break;
                            case MMRDecoder.TwoDimensionalVertical0:
                                a0 = codingLine[codingI++] = referenceLine[referenceI];
                                if (referenceLine[referenceI] < m_width)
                                {
                                    referenceI++;
                                }
                                break;
                            case MMRDecoder.TwoDimensionalVerticalR1:
                                a0 = codingLine[codingI++] = referenceLine[referenceI] + 1;
                                if (referenceLine[referenceI] < m_width)
                                {
                                    referenceI++;
                                    while (referenceLine[referenceI] <= a0 && referenceLine[referenceI] < m_width)
                                    {
                                        referenceI += 2;
                                    }
                                }

                                break;
                            case MMRDecoder.TwoDimensionalVerticalR2:
                                a0 = codingLine[codingI++] = referenceLine[referenceI] + 2;
                                if (referenceLine[referenceI] < m_width)
                                {
                                    referenceI++;
                                    while (referenceLine[referenceI] <= a0 && referenceLine[referenceI] < m_width)
                                    {
                                        referenceI += 2;
                                    }
                                }

                                break;
                            case MMRDecoder.TwoDimensionalVerticalR3:
                                a0 = codingLine[codingI++] = referenceLine[referenceI] + 3;
                                if (referenceLine[referenceI] < m_width)
                                {
                                    referenceI++;
                                    while (referenceLine[referenceI] <= a0 && referenceLine[referenceI] < m_width)
                                    {
                                        referenceI += 2;
                                    }
                                }

                                break;
                            case MMRDecoder.TwoDimensionalVerticalL1:
                                try
                                {
                                    a0 = codingLine[codingI++] = referenceLine[referenceI] - 1;
                                    if (referenceI > 0)
                                    {
                                        referenceI--;
                                    }
                                    else
                                    {
                                        referenceI++;
                                    }

                                    while (referenceLine[referenceI] <= a0 && referenceLine[referenceI] < m_width)
                                    {
                                        referenceI += 2;
                                    }
                                }
                                catch
                                {
                                }
                                break;
                            case MMRDecoder.TwoDimensionalVerticalL2:
                                a0 = codingLine[codingI++] = referenceLine[referenceI] - 2;
                                if (referenceI > 0)
                                {
                                    referenceI--;
                                }
                                else
                                {
                                    referenceI++;
                                }

                                while (referenceLine[referenceI] <= a0 && referenceLine[referenceI] < m_width)
                                {
                                    referenceI += 2;
                                }

                                break;
                            case MMRDecoder.TwoDimensionalVerticalL3:
                                a0 = codingLine[codingI++] = referenceLine[referenceI] - 3;
                                if (referenceI > 0)
                                {
                                    referenceI--;
                                }
                                else
                                {
                                    referenceI++;
                                }

                                while (referenceLine[referenceI] <= a0 && referenceLine[referenceI] < m_width)
                                {
                                    referenceI += 2;
                                }

                                break;
                        }
                    } while (a0 < m_width);

                    codingLine[codingI++] = m_width;

                    for (int j = 0; codingLine[j] < m_width; j += 2)
                    {
                        for (int col = codingLine[j]; col < codingLine[j + 1]; col++)
                        {
                            SetPixel(col, row, 1);
                        }
                    }
                }

                if (mmrDataLength >= 0)
                {
                    m_mmrDecoder.SkipTo(mmrDataLength);
                }
                else
                {
                    m_mmrDecoder.Get24Bits();
                }
            }
            else
            {
                ImagePointer cxPtr0 = new ImagePointer(this), cxPtr1 = new ImagePointer(this);
                ImagePointer atPtr0 = new ImagePointer(this), atPtr1 = new ImagePointer(this), atPtr2 = new ImagePointer(this), atPtr3 = new ImagePointer(this);

                long ltpCX = 0;
                if (typicalPredictionGenericDecodingOn)
                {
                    switch (template)
                    {
                        case 0:
                            ltpCX = 0x3953;
                            break;
                        case 1:
                            ltpCX = 0x079a;
                            break;
                        case 2:
                            ltpCX = 0x0e3;
                            break;
                        case 3:
                            ltpCX = 0x18a;
                            break;
                    }
                }

                bool ltp = false;
                long cx, cx0, cx1, cx2;

                for (int row = 0; row < m_height; row++)
                {
                    if (typicalPredictionGenericDecodingOn)
                    {
                        int bit = m_arithmeticDecoder.DecodeBit(ltpCX, m_arithmeticDecoder.GenericRegionStats);
                        if (bit != 0)
                        {
                            ltp = !ltp;
                        }

                        if (ltp)
                        {
                            DuplicateRow(row, row - 1);
                            continue;
                        }
                    }

                    int pixel;

                    switch (template)
                    {
                        case 0:

                            cxPtr0.SetPointer(0, row - 2);
                            cx0 = cxPtr0.NextPixel();
                            cx0 = (m_bitOperation.Bit32Shift(cx0, 1, BitOperation.LEFT_SHIFT)) | cxPtr0.NextPixel();

                            cxPtr1.SetPointer(0, row - 1);
                            cx1 = cxPtr1.NextPixel();

                            cx1 = (m_bitOperation.Bit32Shift(cx1, 1, BitOperation.LEFT_SHIFT)) | cxPtr1.NextPixel();
                            cx1 = (m_bitOperation.Bit32Shift(cx1, 1, BitOperation.LEFT_SHIFT)) | cxPtr1.NextPixel();

                            cx2 = 0;

                            atPtr0.SetPointer(adaptiveTemplateX[0], row + adaptiveTemplateY[0]);
                            atPtr1.SetPointer(adaptiveTemplateX[1], row + adaptiveTemplateY[1]);
                            atPtr2.SetPointer(adaptiveTemplateX[2], row + adaptiveTemplateY[2]);
                            atPtr3.SetPointer(adaptiveTemplateX[3], row + adaptiveTemplateY[3]);

                            for (int col = 0; col < m_width; col++)
                            {

                                cx = (m_bitOperation.Bit32Shift(cx0, 13, BitOperation.LEFT_SHIFT)) | (m_bitOperation.Bit32Shift(cx1, 8, BitOperation.LEFT_SHIFT)) | (m_bitOperation.Bit32Shift(cx2, 4, BitOperation.LEFT_SHIFT)) | (atPtr0.NextPixel() << 3) | (atPtr1.NextPixel() << 2) | (atPtr2.NextPixel() << 1) | atPtr3.NextPixel();

                                if (useSkip && skipBitmap.GetPixel(col, row) != 0)
                                {
                                    pixel = 0;
                                }
                                else
                                {
                                    pixel = m_arithmeticDecoder.DecodeBit(cx, m_arithmeticDecoder.GenericRegionStats);
                                    if (pixel != 0)
                                    {
                                        SetPixel(col, row, 1);
                                    }
                                }

                                cx0 = ((m_bitOperation.Bit32Shift(cx0, 1, BitOperation.LEFT_SHIFT)) | cxPtr0.NextPixel()) & 0x07;
                                cx1 = ((m_bitOperation.Bit32Shift(cx1, 1, BitOperation.LEFT_SHIFT)) | cxPtr1.NextPixel()) & 0x1f;
                                cx2 = ((m_bitOperation.Bit32Shift(cx2, 1, BitOperation.LEFT_SHIFT)) | pixel) & 0x0f;
                            }
                            break;

                        case 1:

                            cxPtr0.SetPointer(0, row - 2);
                            cx0 = cxPtr0.NextPixel();
                            cx0 = (m_bitOperation.Bit32Shift(cx0, 1, BitOperation.LEFT_SHIFT)) | cxPtr0.NextPixel();
                            cx0 = (m_bitOperation.Bit32Shift(cx0, 1, BitOperation.LEFT_SHIFT)) | cxPtr0.NextPixel();

                            cxPtr1.SetPointer(0, row - 1);
                            cx1 = cxPtr1.NextPixel();
                            cx1 = (m_bitOperation.Bit32Shift(cx1, 1, BitOperation.LEFT_SHIFT)) | cxPtr1.NextPixel();
                            cx1 = (m_bitOperation.Bit32Shift(cx1, 1, BitOperation.LEFT_SHIFT)) | cxPtr1.NextPixel();

                            cx2 = 0;

                            atPtr0.SetPointer(adaptiveTemplateX[0], row + adaptiveTemplateY[0]);
                            for (int col = 0; col < m_width; col++)
                            {

                                cx = (m_bitOperation.Bit32Shift(cx0, 9, BitOperation.LEFT_SHIFT)) | (m_bitOperation.Bit32Shift(cx1, 4, BitOperation.LEFT_SHIFT)) | (m_bitOperation.Bit32Shift(cx2, 1, BitOperation.LEFT_SHIFT)) | atPtr0.NextPixel();

                                if (useSkip && skipBitmap.GetPixel(col, row) != 0)
                                {
                                    pixel = 0;
                                }
                                else
                                {
                                    pixel = m_arithmeticDecoder.DecodeBit(cx, m_arithmeticDecoder.GenericRegionStats);
                                    if (pixel != 0)
                                    {
                                        SetPixel(col, row, 1);
                                    }
                                }

                                cx0 = ((m_bitOperation.Bit32Shift(cx0, 1, BitOperation.LEFT_SHIFT)) | cxPtr0.NextPixel()) & 0x0f;
                                cx1 = ((m_bitOperation.Bit32Shift(cx1, 1, BitOperation.LEFT_SHIFT)) | cxPtr1.NextPixel()) & 0x1f;
                                cx2 = ((m_bitOperation.Bit32Shift(cx2, 1, BitOperation.LEFT_SHIFT)) | pixel) & 0x07;
                            }
                            break;

                        case 2:

                            cxPtr0.SetPointer(0, row - 2);
                            cx0 = cxPtr0.NextPixel();
                            cx0 = (m_bitOperation.Bit32Shift(cx0, 1, BitOperation.LEFT_SHIFT)) | cxPtr0.NextPixel();

                            cxPtr1.SetPointer(0, row - 1);
                            cx1 = cxPtr1.NextPixel();
                            cx1 = (m_bitOperation.Bit32Shift(cx1, 1, BitOperation.LEFT_SHIFT)) | cxPtr1.NextPixel();

                            cx2 = 0;

                            atPtr0.SetPointer(adaptiveTemplateX[0], row + adaptiveTemplateY[0]);

                            for (int col = 0; col < m_width; col++)
                            {

                                cx = (m_bitOperation.Bit32Shift(cx0, 7, BitOperation.LEFT_SHIFT)) | (m_bitOperation.Bit32Shift(cx1, 3, BitOperation.LEFT_SHIFT)) | (m_bitOperation.Bit32Shift(cx2, 1, BitOperation.LEFT_SHIFT)) | atPtr0.NextPixel();

                                if (useSkip && skipBitmap.GetPixel(col, row) != 0)
                                {
                                    pixel = 0;
                                }
                                else
                                {
                                    pixel = m_arithmeticDecoder.DecodeBit(cx, m_arithmeticDecoder.GenericRegionStats);
                                    if (pixel != 0)
                                    {
                                        SetPixel(col, row, 1);
                                    }
                                }

                                cx0 = ((m_bitOperation.Bit32Shift(cx0, 1, BitOperation.LEFT_SHIFT)) | cxPtr0.NextPixel()) & 0x07;
                                cx1 = ((m_bitOperation.Bit32Shift(cx1, 1, BitOperation.LEFT_SHIFT)) | cxPtr1.NextPixel()) & 0x0f;
                                cx2 = ((m_bitOperation.Bit32Shift(cx2, 1, BitOperation.LEFT_SHIFT)) | pixel) & 0x03;
                            }
                            break;

                        case 3:

                            cxPtr1.SetPointer(0, row - 1);
                            cx1 = cxPtr1.NextPixel();
                            cx1 = (m_bitOperation.Bit32Shift(cx1, 1, BitOperation.LEFT_SHIFT)) | cxPtr1.NextPixel();

                            cx2 = 0;

                            atPtr0.SetPointer(adaptiveTemplateX[0], row + adaptiveTemplateY[0]);

                            for (int col = 0; col < m_width; col++)
                            {

                                cx = (m_bitOperation.Bit32Shift(cx1, 5, BitOperation.LEFT_SHIFT)) | (m_bitOperation.Bit32Shift(cx2, 1, BitOperation.LEFT_SHIFT)) | atPtr0.NextPixel();

                                if (useSkip && skipBitmap.GetPixel(col, row) != 0)
                                {
                                    pixel = 0;

                                }
                                else
                                {
                                    pixel = m_arithmeticDecoder.DecodeBit(cx, m_arithmeticDecoder.GenericRegionStats);
                                    if (pixel != 0)
                                    {
                                        SetPixel(col, row, 1);
                                    }
                                }

                                cx1 = ((m_bitOperation.Bit32Shift(cx1, 1, BitOperation.LEFT_SHIFT)) | cxPtr1.NextPixel()) & 0x1f;
                                cx2 = ((m_bitOperation.Bit32Shift(cx2, 1, BitOperation.LEFT_SHIFT)) | pixel) & 0x0f;
                            }
                            break;
                    }
                }
            }
        }

        internal void ReadGenericRefinementRegion(int template, bool typicalPredictionGenericRefinementOn, JBIG2Image referredToBitmap, int referenceDX, int referenceDY, short[] adaptiveTemplateX, short[] adaptiveTemplateY)
        {
            ImagePointer cxPtr0, cxPtr1, cxPtr2, cxPtr3, cxPtr4, cxPtr5, cxPtr6, typicalPredictionGenericRefinementCXPtr0, typicalPredictionGenericRefinementCXPtr1, typicalPredictionGenericRefinementCXPtr2;
            long ltpCX;
            if (template != 0)
            {
                ltpCX = 0x008;

                cxPtr0 = new ImagePointer(this);
                cxPtr1 = new ImagePointer(this);
                cxPtr2 = new ImagePointer(referredToBitmap);
                cxPtr3 = new ImagePointer(referredToBitmap);
                cxPtr4 = new ImagePointer(referredToBitmap);
                cxPtr5 = new ImagePointer(this);
                cxPtr6 = new ImagePointer(this);
                typicalPredictionGenericRefinementCXPtr0 = new ImagePointer(referredToBitmap);
                typicalPredictionGenericRefinementCXPtr1 = new ImagePointer(referredToBitmap);
                typicalPredictionGenericRefinementCXPtr2 = new ImagePointer(referredToBitmap);
            }
            else
            {
                ltpCX = 0x0010;

                cxPtr0 = new ImagePointer(this);
                cxPtr1 = new ImagePointer(this);
                cxPtr2 = new ImagePointer(referredToBitmap);
                cxPtr3 = new ImagePointer(referredToBitmap);
                cxPtr4 = new ImagePointer(referredToBitmap);
                cxPtr5 = new ImagePointer(this);
                cxPtr6 = new ImagePointer(referredToBitmap);
                typicalPredictionGenericRefinementCXPtr0 = new ImagePointer(referredToBitmap);
                typicalPredictionGenericRefinementCXPtr1 = new ImagePointer(referredToBitmap);
                typicalPredictionGenericRefinementCXPtr2 = new ImagePointer(referredToBitmap);
            }

            long cx, cx0, cx2, cx3, cx4;
            long typicalPredictionGenericRefinementCX0, typicalPredictionGenericRefinementCX1, typicalPredictionGenericRefinementCX2;
            bool ltp = false;

            for (int row = 0; row < m_height; row++)
            {

                if (template != 0)
                {

                    cxPtr0.SetPointer(0, row - 1);
                    cx0 = cxPtr0.NextPixel();

                    cxPtr1.SetPointer(-1, row);

                    cxPtr2.SetPointer(-referenceDX, row - 1 - referenceDY);

                    cxPtr3.SetPointer(-1 - referenceDX, row - referenceDY);
                    cx3 = cxPtr3.NextPixel();
                    cx3 = (m_bitOperation.Bit32Shift(cx3, 1, BitOperation.LEFT_SHIFT)) | cxPtr3.NextPixel();

                    cxPtr4.SetPointer(-referenceDX, row + 1 - referenceDY);
                    cx4 = cxPtr4.NextPixel();

                    typicalPredictionGenericRefinementCX0 = typicalPredictionGenericRefinementCX1 = typicalPredictionGenericRefinementCX2 = 0;

                    if (typicalPredictionGenericRefinementOn)
                    {
                        typicalPredictionGenericRefinementCXPtr0.SetPointer(-1 - referenceDX, row - 1 - referenceDY);
                        typicalPredictionGenericRefinementCX0 = typicalPredictionGenericRefinementCXPtr0.NextPixel();
                        typicalPredictionGenericRefinementCX0 = (m_bitOperation.Bit32Shift(typicalPredictionGenericRefinementCX0, 1, BitOperation.LEFT_SHIFT)) | typicalPredictionGenericRefinementCXPtr0.NextPixel();
                        typicalPredictionGenericRefinementCX0 = (m_bitOperation.Bit32Shift(typicalPredictionGenericRefinementCX0, 1, BitOperation.LEFT_SHIFT)) | typicalPredictionGenericRefinementCXPtr0.NextPixel();

                        typicalPredictionGenericRefinementCXPtr1.SetPointer(-1 - referenceDX, row - referenceDY);
                        typicalPredictionGenericRefinementCX1 = typicalPredictionGenericRefinementCXPtr1.NextPixel();
                        typicalPredictionGenericRefinementCX1 = (m_bitOperation.Bit32Shift(typicalPredictionGenericRefinementCX1, 1, BitOperation.LEFT_SHIFT)) | typicalPredictionGenericRefinementCXPtr1.NextPixel();
                        typicalPredictionGenericRefinementCX1 = (m_bitOperation.Bit32Shift(typicalPredictionGenericRefinementCX1, 1, BitOperation.LEFT_SHIFT)) | typicalPredictionGenericRefinementCXPtr1.NextPixel();

                        typicalPredictionGenericRefinementCXPtr2.SetPointer(-1 - referenceDX, row + 1 - referenceDY);
                        typicalPredictionGenericRefinementCX2 = typicalPredictionGenericRefinementCXPtr2.NextPixel();
                        typicalPredictionGenericRefinementCX2 = (m_bitOperation.Bit32Shift(typicalPredictionGenericRefinementCX2, 1, BitOperation.LEFT_SHIFT)) | typicalPredictionGenericRefinementCXPtr2.NextPixel();
                        typicalPredictionGenericRefinementCX2 = (m_bitOperation.Bit32Shift(typicalPredictionGenericRefinementCX2, 1, BitOperation.LEFT_SHIFT)) | typicalPredictionGenericRefinementCXPtr2.NextPixel();
                    }

                    for (int col = 0; col < m_width; col++)
                    {

                        cx0 = ((m_bitOperation.Bit32Shift(cx0, 1, BitOperation.LEFT_SHIFT)) | cxPtr0.NextPixel()) & 7;
                        cx3 = ((m_bitOperation.Bit32Shift(cx3, 1, BitOperation.LEFT_SHIFT)) | cxPtr3.NextPixel()) & 7;
                        cx4 = ((m_bitOperation.Bit32Shift(cx4, 1, BitOperation.LEFT_SHIFT)) | cxPtr4.NextPixel()) & 3;

                        if (typicalPredictionGenericRefinementOn)
                        {
                            typicalPredictionGenericRefinementCX0 = ((m_bitOperation.Bit32Shift(typicalPredictionGenericRefinementCX0, 1, BitOperation.LEFT_SHIFT)) | typicalPredictionGenericRefinementCXPtr0.NextPixel()) & 7;
                            typicalPredictionGenericRefinementCX1 = ((m_bitOperation.Bit32Shift(typicalPredictionGenericRefinementCX1, 1, BitOperation.LEFT_SHIFT)) | typicalPredictionGenericRefinementCXPtr1.NextPixel()) & 7;
                            typicalPredictionGenericRefinementCX2 = ((m_bitOperation.Bit32Shift(typicalPredictionGenericRefinementCX2, 1, BitOperation.LEFT_SHIFT)) | typicalPredictionGenericRefinementCXPtr2.NextPixel()) & 7;

                            int decodeBit = m_arithmeticDecoder.DecodeBit(ltpCX, m_arithmeticDecoder.RefinementRegionStats);
                            if (decodeBit != 0)
                            {
                                ltp = !ltp;
                            }
                            if (typicalPredictionGenericRefinementCX0 == 0 && typicalPredictionGenericRefinementCX1 == 0 && typicalPredictionGenericRefinementCX2 == 0)
                            {
                                SetPixel(col, row, 0);
                                continue;
                            }
                            else if (typicalPredictionGenericRefinementCX0 == 7 && typicalPredictionGenericRefinementCX1 == 7 && typicalPredictionGenericRefinementCX2 == 7)
                            {
                                SetPixel(col, row, 1);
                                continue;
                            }
                        }
                        cx = (m_bitOperation.Bit32Shift(cx0, 7, BitOperation.LEFT_SHIFT)) | (cxPtr1.NextPixel() << 6) | (cxPtr2.NextPixel() << 5) | (m_bitOperation.Bit32Shift(cx3, 2, BitOperation.LEFT_SHIFT)) | cx4;

                        int pixel = m_arithmeticDecoder.DecodeBit(cx, m_arithmeticDecoder.RefinementRegionStats);
                        if (pixel == 1)
                        {
                            SetPixel(col, row, 1);
                        }
                    }

                }
                else
                {

                    cxPtr0.SetPointer(0, row - 1);
                    cx0 = cxPtr0.NextPixel();

                    cxPtr1.SetPointer(-1, row);

                    cxPtr2.SetPointer(-referenceDX, row - 1 - referenceDY);
                    cx2 = cxPtr2.NextPixel();

                    cxPtr3.SetPointer(-1 - referenceDX, row - referenceDY);
                    cx3 = cxPtr3.NextPixel();
                    cx3 = (m_bitOperation.Bit32Shift(cx3, 1, BitOperation.LEFT_SHIFT)) | cxPtr3.NextPixel();

                    cxPtr4.SetPointer(-1 - referenceDX, row + 1 - referenceDY);
                    cx4 = cxPtr4.NextPixel();
                    cx4 = (m_bitOperation.Bit32Shift(cx4, 1, BitOperation.LEFT_SHIFT)) | cxPtr4.NextPixel();

                    cxPtr5.SetPointer(adaptiveTemplateX[0], row + adaptiveTemplateY[0]);

                    cxPtr6.SetPointer(adaptiveTemplateX[1] - referenceDX, row + adaptiveTemplateY[1] - referenceDY);

                    typicalPredictionGenericRefinementCX0 = typicalPredictionGenericRefinementCX1 = typicalPredictionGenericRefinementCX2 = 0;
                    if (typicalPredictionGenericRefinementOn)
                    {
                        typicalPredictionGenericRefinementCXPtr0.SetPointer(-1 - referenceDX, row - 1 - referenceDY);
                        typicalPredictionGenericRefinementCX0 = typicalPredictionGenericRefinementCXPtr0.NextPixel();
                        typicalPredictionGenericRefinementCX0 = (m_bitOperation.Bit32Shift(typicalPredictionGenericRefinementCX0, 1, BitOperation.LEFT_SHIFT)) | typicalPredictionGenericRefinementCXPtr0.NextPixel();
                        typicalPredictionGenericRefinementCX0 = (m_bitOperation.Bit32Shift(typicalPredictionGenericRefinementCX0, 1, BitOperation.LEFT_SHIFT)) | typicalPredictionGenericRefinementCXPtr0.NextPixel();

                        typicalPredictionGenericRefinementCXPtr1.SetPointer(-1 - referenceDX, row - referenceDY);
                        typicalPredictionGenericRefinementCX1 = typicalPredictionGenericRefinementCXPtr1.NextPixel();
                        typicalPredictionGenericRefinementCX1 = (m_bitOperation.Bit32Shift(typicalPredictionGenericRefinementCX1, 1, BitOperation.LEFT_SHIFT)) | typicalPredictionGenericRefinementCXPtr1.NextPixel();
                        typicalPredictionGenericRefinementCX1 = (m_bitOperation.Bit32Shift(typicalPredictionGenericRefinementCX1, 1, BitOperation.LEFT_SHIFT)) | typicalPredictionGenericRefinementCXPtr1.NextPixel();

                        typicalPredictionGenericRefinementCXPtr2.SetPointer(-1 - referenceDX, row + 1 - referenceDY);
                        typicalPredictionGenericRefinementCX2 = typicalPredictionGenericRefinementCXPtr2.NextPixel();
                        typicalPredictionGenericRefinementCX2 = (m_bitOperation.Bit32Shift(typicalPredictionGenericRefinementCX2, 1, BitOperation.LEFT_SHIFT)) | typicalPredictionGenericRefinementCXPtr2.NextPixel();
                        typicalPredictionGenericRefinementCX2 = (m_bitOperation.Bit32Shift(typicalPredictionGenericRefinementCX2, 1, BitOperation.LEFT_SHIFT)) | typicalPredictionGenericRefinementCXPtr2.NextPixel();
                    }

                    for (int col = 0; col < m_width; col++)
                    {

                        cx0 = ((m_bitOperation.Bit32Shift(cx0, 1, BitOperation.LEFT_SHIFT)) | cxPtr0.NextPixel()) & 3;
                        cx2 = ((m_bitOperation.Bit32Shift(cx2, 1, BitOperation.LEFT_SHIFT)) | cxPtr2.NextPixel()) & 3;
                        cx3 = ((m_bitOperation.Bit32Shift(cx3, 1, BitOperation.LEFT_SHIFT)) | cxPtr3.NextPixel()) & 7;
                        cx4 = ((m_bitOperation.Bit32Shift(cx4, 1, BitOperation.LEFT_SHIFT)) | cxPtr4.NextPixel()) & 7;

                        if (typicalPredictionGenericRefinementOn)
                        {
                            typicalPredictionGenericRefinementCX0 = ((m_bitOperation.Bit32Shift(typicalPredictionGenericRefinementCX0, 1, BitOperation.LEFT_SHIFT)) | typicalPredictionGenericRefinementCXPtr0.NextPixel()) & 7;
                            typicalPredictionGenericRefinementCX1 = ((m_bitOperation.Bit32Shift(typicalPredictionGenericRefinementCX1, 1, BitOperation.LEFT_SHIFT)) | typicalPredictionGenericRefinementCXPtr1.NextPixel()) & 7;
                            typicalPredictionGenericRefinementCX2 = ((m_bitOperation.Bit32Shift(typicalPredictionGenericRefinementCX2, 1, BitOperation.LEFT_SHIFT)) | typicalPredictionGenericRefinementCXPtr2.NextPixel()) & 7;

                            int decodeBit = m_arithmeticDecoder.DecodeBit(ltpCX, m_arithmeticDecoder.RefinementRegionStats);
                            if (decodeBit == 1)
                            {
                                ltp = !ltp;
                            }
                            if (typicalPredictionGenericRefinementCX0 == 0 && typicalPredictionGenericRefinementCX1 == 0 && typicalPredictionGenericRefinementCX2 == 0)
                            {
                                SetPixel(col, row, 0);
                                continue;
                            }
                            else if (typicalPredictionGenericRefinementCX0 == 7 && typicalPredictionGenericRefinementCX1 == 7 && typicalPredictionGenericRefinementCX2 == 7)
                            {
                                SetPixel(col, row, 1);
                                continue;
                            }
                        }

                        cx = (m_bitOperation.Bit32Shift(cx0, 11, BitOperation.LEFT_SHIFT)) | (cxPtr1.NextPixel() << 10) | (m_bitOperation.Bit32Shift(cx2, 8, BitOperation.LEFT_SHIFT)) | (m_bitOperation.Bit32Shift(cx3, 5, BitOperation.LEFT_SHIFT)) | (m_bitOperation.Bit32Shift(cx4, 2, BitOperation.LEFT_SHIFT)) | (cxPtr5.NextPixel() << 1) | cxPtr6.NextPixel();

                        int pixel = m_arithmeticDecoder.DecodeBit(cx, m_arithmeticDecoder.RefinementRegionStats);
                        if (pixel == 1)
                        {
                            SetPixel(col, row, 1);
                        }
                    }
                }
            }
        }

        internal void ReadTextRegion(bool huffman, bool symbolRefine, int noOfSymbolInstances, int logStrips, int noOfSymbols, int[][] symbolCodeTable, int symbolCodeLength, JBIG2Image[] symbols, int defaultPixel, int combinationOperator, bool transposed, int referenceCorner, int sOffset, int[][] huffmanFSTable, int[][] huffmanDSTable, int[][] huffmanDTTable, int[][] huffmanRDWTable, int[][] huffmanRDHTable, int[][] huffmanRDXTable, int[][] huffmanRDYTable, int[][] huffmanRSizeTable, int template, short[] symbolRegionAdaptiveTemplateX, short[] symbolRegionAdaptiveTemplateY, JBIG2StreamDecoder decoder)
        {

            JBIG2Image symbolBitmap;
            int strips = 1 << logStrips;
            Clear(defaultPixel);
            int t;
            if (huffman)
            {
                t = m_huffmanDecoder.DecodeInt(huffmanDTTable).IntResult;
            }
            else
            {
                t = m_arithmeticDecoder.DecodeInt(m_arithmeticDecoder.IadtStats).IntResult;
            }
            t *= -strips;

            int currentInstance = 0;
            int firstS = 0;
            int dt, tt, ds, s;
            while (currentInstance < noOfSymbolInstances)
            {

                if (huffman)
                {
                    dt = m_huffmanDecoder.DecodeInt(huffmanDTTable).IntResult;
                }
                else
                {
                    dt = m_arithmeticDecoder.DecodeInt(m_arithmeticDecoder.IadtStats).IntResult;
                }
                t += dt * strips;

                if (huffman)
                {
                    ds = m_huffmanDecoder.DecodeInt(huffmanFSTable).IntResult;
                }
                else
                {
                    ds = m_arithmeticDecoder.DecodeInt(m_arithmeticDecoder.IafsStats).IntResult;
                }
                firstS += ds;
                s = firstS;

                while (true)
                {

                    if (strips == 1)
                    {
                        dt = 0;
                    }
                    else if (huffman)
                    {
                        dt = decoder.ReadBits(logStrips);
                    }
                    else
                    {
                        dt = m_arithmeticDecoder.DecodeInt(m_arithmeticDecoder.IaitStats).IntResult;
                    }
                    tt = t + dt;

                    long symbolID;
                    if (huffman)
                    {
                        if (symbolCodeTable != null)
                        {
                            symbolID = m_huffmanDecoder.DecodeInt(symbolCodeTable).IntResult;
                        }
                        else
                        {
                            symbolID = decoder.ReadBits(symbolCodeLength);
                        }
                    }
                    else
                    {
                        symbolID = m_arithmeticDecoder.DecodeIAID(symbolCodeLength, m_arithmeticDecoder.IaidStats);
                    }

                    if (symbolID < noOfSymbols)
                    {
                        symbolBitmap = null;

                        int ri;
                        if (symbolRefine)
                        {
                            if (huffman)
                            {
                                ri = decoder.ReadBit();
                            }
                            else
                            {
                                ri = m_arithmeticDecoder.DecodeInt(m_arithmeticDecoder.IariStats).IntResult;
                            }
                        }
                        else
                        {
                            ri = 0;
                        }
                        if (ri != 0)
                        {

                            int refinementDeltaWidth, refinementDeltaHeight, refinementDeltaX, refinementDeltaY;

                            if (huffman)
                            {
                                refinementDeltaWidth = m_huffmanDecoder.DecodeInt(huffmanRDWTable).IntResult;
                                refinementDeltaHeight = m_huffmanDecoder.DecodeInt(huffmanRDHTable).IntResult;
                                refinementDeltaX = m_huffmanDecoder.DecodeInt(huffmanRDXTable).IntResult;
                                refinementDeltaY = m_huffmanDecoder.DecodeInt(huffmanRDYTable).IntResult;

                                decoder.ConsumeRemainingBits();
                                m_arithmeticDecoder.Start();
                            }
                            else
                            {
                                refinementDeltaWidth = m_arithmeticDecoder.DecodeInt(m_arithmeticDecoder.IardwStats).IntResult;
                                refinementDeltaHeight = m_arithmeticDecoder.DecodeInt(m_arithmeticDecoder.IardhStats).IntResult;
                                refinementDeltaX = m_arithmeticDecoder.DecodeInt(m_arithmeticDecoder.IardxStats).IntResult;
                                refinementDeltaY = m_arithmeticDecoder.DecodeInt(m_arithmeticDecoder.IardyStats).IntResult;
                            }
                            refinementDeltaX = ((refinementDeltaWidth >= 0) ? refinementDeltaWidth : refinementDeltaWidth - 1) / 2 + refinementDeltaX;
                            refinementDeltaY = ((refinementDeltaHeight >= 0) ? refinementDeltaHeight : refinementDeltaHeight - 1) / 2 + refinementDeltaY;

                            symbolBitmap = new JBIG2Image(refinementDeltaWidth + symbols[(int)symbolID].m_width, refinementDeltaHeight + symbols[(int)symbolID].m_height, m_arithmeticDecoder, m_huffmanDecoder, m_mmrDecoder);

                            symbolBitmap.ReadGenericRefinementRegion(template, false, symbols[(int)symbolID], refinementDeltaX, refinementDeltaY, symbolRegionAdaptiveTemplateX, symbolRegionAdaptiveTemplateY);

                        }
                        else
                        {
                            symbolBitmap = symbols[(int)symbolID];
                        }

                        int bitmapWidth = symbolBitmap.m_width - 1;
                        int bitmapHeight = symbolBitmap.m_height - 1;
                        if (transposed)
                        {
                            switch (referenceCorner)
                            {
                                case 0: // bottom left
                                    Combine(symbolBitmap, tt, s, combinationOperator);
                                    break;
                                case 1: // top left
                                    Combine(symbolBitmap, tt, s, combinationOperator);
                                    break;
                                case 2: // bottom right
                                    Combine(symbolBitmap, (tt - bitmapWidth), s, combinationOperator);
                                    break;
                                case 3: // top right
                                    Combine(symbolBitmap, (tt - bitmapWidth), s, combinationOperator);
                                    break;
                            }
                            s += bitmapHeight;
                        }
                        else
                        {
                            switch (referenceCorner)
                            {
                                case 0: // bottom left
                                    Combine(symbolBitmap, s, (tt - bitmapHeight), combinationOperator);
                                    break;
                                case 1: // top left
                                    Combine(symbolBitmap, s, tt, combinationOperator);
                                    break;
                                case 2: // bottom right
                                    Combine(symbolBitmap, s, (tt - bitmapHeight), combinationOperator);
                                    break;
                                case 3: // top right
                                    Combine(symbolBitmap, s, tt, combinationOperator);
                                    break;
                            }
                            s += bitmapWidth;
                        }
                    }

                    currentInstance++;

                    DecodeIntResult decodeIntResult;

                    if (huffman)
                    {
                        decodeIntResult = m_huffmanDecoder.DecodeInt(huffmanDSTable);
                    }
                    else
                    {
                        decodeIntResult = m_arithmeticDecoder.DecodeInt(m_arithmeticDecoder.IadsStats);
                    }

                    if (!decodeIntResult.BooleanResult)
                    {
                        break;
                    }

                    ds = decodeIntResult.IntResult;

                    s += sOffset + ds;
                }
            }
        }

        internal void Clear(int defPixel)
        {
            data.Set(0, defPixel == 1);
        }

        internal void Combine(JBIG2Image bitmap, int x, int y, long combOp)
        {
            int srcWidth = bitmap.m_width;
            int srcHeight = bitmap.m_height;
            int srcRow = 0, srcCol = 0;

            for (int row = y; row < y + srcHeight; row++)
            {
                for (int col = x; col < x + srcWidth; col++)
                {

                    int srcPixel = bitmap.GetPixel(srcCol, srcRow);

                    switch ((int)combOp)
                    {
                        case 0: // or
                            SetPixel(col, row, GetPixel(col, row) | srcPixel);
                            break;
                        case 1: // and
                            SetPixel(col, row, GetPixel(col, row) & srcPixel);
                            break;
                        case 2: // xor
                            SetPixel(col, row, GetPixel(col, row) ^ srcPixel);
                            break;
                        case 3: // xnor
                            if ((GetPixel(col, row) == 1 && srcPixel == 1) || (GetPixel(col, row) == 0 && srcPixel == 0))
                            {
                                SetPixel(col, row, 1);
                            }
                            else
                            {
                                SetPixel(col, row, 0);
                            }

                            break;
                        case 4: // replace
                            SetPixel(col, row, srcPixel);
                            break;
                    }
                    srcCol++;
                }

                srcCol = 0;
                srcRow++;
            }
        }
        private void DuplicateRow(int yDest, int ySrc)
        {
            for (int i = 0; i < m_width; i++)
            {
                SetPixel(i, yDest, GetPixel(i, ySrc));
            }
        }

        internal byte[] GetData(bool switchPixelColor)
        {
            byte[] bytes = new byte[m_height * m_line];

            int count = 0, offset = 0;
            for (int row = 0; row < m_height; row++)
            {
                for (int col = 0; col < m_width; col++)
                {
                    if (data.Get(count))
                    {
                        int bite = (count + offset) / 8;
                        int bit = (count + offset) % 8;

                        bytes[bite] |= (byte)(1 << (7 - bit));
                    }
                    count++;
                }

                offset = (m_line * 8 * (row + 1)) - count;
            }

            if (switchPixelColor)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] ^= 0xff;
                }
            }

            return bytes;
        }

        internal JBIG2Image GetSlice(int x, int y, int width, int height)
        {
            JBIG2Image slice = new JBIG2Image(width, height, m_arithmeticDecoder, m_huffmanDecoder, m_mmrDecoder);

            int sliceRow = 0, sliceCol = 0;
            for (int row = y; row < height; row++)
            {
                for (int col = x; col < x + width; col++)
                {
                    slice.SetPixel(sliceCol, sliceRow, GetPixel(col, row));
                    sliceCol++;
                }
                sliceCol = 0;
                sliceRow++;
            }
            return slice;
        }

        private void SetPixel(int col, int row, BitArray data, int value)
        {
            int index = (row * m_width) + col;

            data.Set(index, value == 1);
        }

        internal void SetPixel(int col, int row, int value)
        {
            SetPixel(col, row, data, value);
        }

        internal int GetPixel(int col, int row)
        {
            try
            {
                return data.Get((row * m_width) + col) ? 1 : 0;
            }
            catch
            {
                return 0;
            }
        }

        internal void Expand(int newHeight, int defaultPixel)
        {
            BitArray newData = new BitArray(newHeight * m_width);

            for (int row = 0; row < m_height; row++)
            {
                for (int col = 0; col < m_width; col++)
                {
                    SetPixel(col, row, newData, GetPixel(col, row));
                }
            }
            this.m_height = newHeight;
            this.data = newData;
        }        
    }
}


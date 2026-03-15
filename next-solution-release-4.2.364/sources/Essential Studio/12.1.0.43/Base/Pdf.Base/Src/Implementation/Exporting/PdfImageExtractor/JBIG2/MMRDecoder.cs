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

namespace Syncfusion.Pdf
{
    class MMRDecoder
    {
        private Jbig2StreamReader m_reader;
        private BitOperation m_bitOperation = new BitOperation();
        private long m_bufferLength = 0, m_buffer = 0, m_noOfBytesRead = 0;
        private int[][] m_twoDimensionalTable1 = { new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { 7, TwoDimensionalVerticalL3 }, new int[] { 7, TwoDimensionalVerticalR3 }, new int[] { 6, TwoDimensionalVerticalL2 }, new int[] { 6, TwoDimensionalVerticalL2 }, new int[] { 6, TwoDimensionalVerticalR2 }, new int[] { 6, TwoDimensionalVerticalR2 }, new int[] { 4, TwoDimensionalPass }, new int[] { 4, TwoDimensionalPass }, new int[] { 4, TwoDimensionalPass }, new int[] { 4, TwoDimensionalPass }, new int[] { 4, TwoDimensionalPass }, new int[] { 4, TwoDimensionalPass }, new int[] { 4, TwoDimensionalPass }, new int[] { 4, TwoDimensionalPass }, new int[] { 3, TwoDimensionalHorizontal }, new int[] { 3, TwoDimensionalHorizontal }, new int[] { 3, TwoDimensionalHorizontal }, new int[] { 3, TwoDimensionalHorizontal }, new int[] { 3, TwoDimensionalHorizontal }, new int[] { 3, TwoDimensionalHorizontal }, new int[] { 3, TwoDimensionalHorizontal }, new int[] { 3, TwoDimensionalHorizontal }, new int[] { 3, TwoDimensionalHorizontal }, new int[] { 3, TwoDimensionalHorizontal }, new int[] { 3, TwoDimensionalHorizontal }, new int[] { 3, TwoDimensionalHorizontal }, new int[] { 3, TwoDimensionalHorizontal }, new int[] { 3, TwoDimensionalHorizontal }, new int[] { 3, TwoDimensionalHorizontal }, new int[] { 3, TwoDimensionalHorizontal }, new int[] { 3, TwoDimensionalVerticalL1 }, new int[] { 3, TwoDimensionalVerticalL1 }, new int[] { 3, TwoDimensionalVerticalL1 }, new int[] { 3, TwoDimensionalVerticalL1 }, new int[] { 3, TwoDimensionalVerticalL1 }, new int[] { 3, TwoDimensionalVerticalL1 }, new int[] { 3, TwoDimensionalVerticalL1 }, new int[] { 3, TwoDimensionalVerticalL1 }, new int[] { 3, TwoDimensionalVerticalL1 }, new int[] { 3, TwoDimensionalVerticalL1 }, new int[] { 3, TwoDimensionalVerticalL1 }, new int[] { 3, TwoDimensionalVerticalL1 }, new int[] { 3, TwoDimensionalVerticalL1 }, new int[] { 3, TwoDimensionalVerticalL1 }, new int[] { 3, TwoDimensionalVerticalL1 }, new int[] { 3, TwoDimensionalVerticalL1 }, new int[] { 3, TwoDimensionalVerticalR1 }, new int[] { 3, TwoDimensionalVerticalR1 }, new int[] { 3, TwoDimensionalVerticalR1 }, new int[] { 3, TwoDimensionalVerticalR1 }, new int[] { 3, TwoDimensionalVerticalR1 }, new int[] { 3, TwoDimensionalVerticalR1 }, new int[] { 3, TwoDimensionalVerticalR1 }, new int[] { 3, TwoDimensionalVerticalR1 }, new int[] { 3, TwoDimensionalVerticalR1 }, new int[] { 3, TwoDimensionalVerticalR1 }, new int[] { 3, TwoDimensionalVerticalR1 }, new int[] { 3, TwoDimensionalVerticalR1 }, new int[] { 3, TwoDimensionalVerticalR1 }, new int[] { 3, TwoDimensionalVerticalR1 }, new int[] { 3, TwoDimensionalVerticalR1 }, new int[] { 3, TwoDimensionalVerticalR1 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 }, new int[] { 1, TwoDimensionalVertical0 } };
        /// <summary>
        /// white run lengths </summary>
        private int[][] m_whiteTable1 = { new int[] { -1, -1 }, new int[] { 12, CcittEndOfLine }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { 11, 1792 }, new int[] { 11, 1792 }, new int[] { 12, 1984 }, new int[] { 12, 2048 }, new int[] { 12, 2112 }, new int[] { 12, 2176 }, new int[] { 12, 2240 }, new int[] { 12, 2304 }, new int[] { 11, 1856 }, new int[] { 11, 1856 }, new int[] { 11, 1920 }, new int[] { 11, 1920 }, new int[] { 12, 2368 }, new int[] { 12, 2432 }, new int[] { 12, 2496 }, new int[] { 12, 2560 } };
        private int[][] m_whiteTable2 = { new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { 8, 29 }, new int[] { 8, 29 }, new int[] { 8, 30 }, new int[] { 8, 30 }, new int[] { 8, 45 }, new int[] { 8, 45 }, new int[] { 8, 46 }, new int[] { 8, 46 }, new int[] { 7, 22 }, new int[] { 7, 22 }, new int[] { 7, 22 }, new int[] { 7, 22 }, new int[] { 7, 23 }, new int[] { 7, 23 }, new int[] { 7, 23 }, new int[] { 7, 23 }, new int[] { 8, 47 }, new int[] { 8, 47 }, new int[] { 8, 48 }, new int[] { 8, 48 }, new int[] { 6, 13 }, new int[] { 6, 13 }, new int[] { 6, 13 }, new int[] { 6, 13 }, new int[] { 6, 13 }, new int[] { 6, 13 }, new int[] { 6, 13 }, new int[] { 6, 13 }, new int[] { 7, 20 }, new int[] { 7, 20 }, new int[] { 7, 20 }, new int[] { 7, 20 }, new int[] { 8, 33 }, new int[] { 8, 33 }, new int[] { 8, 34 }, new int[] { 8, 34 }, new int[] { 8, 35 }, new int[] { 8, 35 }, new int[] { 8, 36 }, new int[] { 8, 36 }, new int[] { 8, 37 }, new int[] { 8, 37 }, new int[] { 8, 38 }, new int[] { 8, 38 }, new int[] { 7, 19 }, new int[] { 7, 19 }, new int[] { 7, 19 }, new int[] { 7, 19 }, new int[] { 8, 31 }, new int[] { 8, 31 }, new int[] { 8, 32 }, new int[] { 8, 32 }, new int[] { 6, 1 }, new int[] { 6, 1 }, new int[] { 6, 1 }, new int[] { 6, 1 }, new int[] { 6, 1 }, new int[] { 6, 1 }, new int[] { 6, 1 }, new int[] { 6, 1 }, new int[] { 6, 12 }, new int[] { 6, 12 }, new int[] { 6, 12 }, new int[] { 6, 12 }, new int[] { 6, 12 }, new int[] { 6, 12 }, new int[] { 6, 12 }, new int[] { 6, 12 }, new int[] { 8, 53 }, new int[] { 8, 53 }, new int[] { 8, 54 }, new int[] { 8, 54 }, new int[] { 7, 26 }, new int[] { 7, 26 }, new int[] { 7, 26 }, new int[] { 7, 26 }, new int[] { 8, 39 }, new int[] { 8, 39 }, new int[] { 8, 40 }, new int[] { 8, 40 }, new int[] { 8, 41 }, new int[] { 8, 41 }, new int[] { 8, 42 }, new int[] { 8, 42 }, new int[] { 8, 43 }, new int[] { 8, 43 }, new int[] { 8, 44 }, new int[] { 8, 44 }, new int[] { 7, 21 }, new int[] { 7, 21 }, new int[] { 7, 21 }, new int[] { 7, 21 }, new int[] { 7, 28 }, new int[] { 7, 28 }, new int[] { 7, 28 }, new int[] { 7, 28 }, new int[] { 8, 61 }, new int[] { 8, 61 }, new int[] { 8, 62 }, new int[] { 8, 62 }, new int[] { 8, 63 }, new int[] { 8, 63 }, new int[] { 8, 0 }, new int[] { 8, 0 }, new int[] { 8, 320 }, new int[] { 8, 320 }, new int[] { 8, 384 }, new int[] { 8, 384 }, new int[] { 5, 10 }, new int[] { 5, 10 }, new int[] { 5, 10 }, new int[] { 5, 10 }, new int[] { 5, 10 }, new int[] { 5, 10 }, new int[] { 5, 10 }, new int[] { 5, 10 }, new int[] { 5, 10 }, new int[] { 5, 10 }, new int[] { 5, 10 }, new int[] { 5, 10 }, new int[] { 5, 10 }, new int[] { 5, 10 }, new int[] { 5, 10 }, new int[] { 5, 10 }, new int[] { 5, 11 }, new int[] { 5, 11 }, new int[] { 5, 11 }, new int[] { 5, 11 }, new int[] { 5, 11 }, new int[] { 5, 11 }, new int[] { 5, 11 }, new int[] { 5, 11 }, new int[] { 5, 11 }, new int[] { 5, 11 }, new int[] { 5, 11 }, new int[] { 5, 11 }, new int[] { 5, 11 }, new int[] { 5, 11 }, new int[] { 5, 11 }, new int[] { 5, 11 }, new int[] { 7, 27 }, new int[] { 7, 27 }, new int[] { 7, 27 }, new int[] { 7, 27 }, new int[] { 8, 59 }, new int[] { 8, 59 }, new int[] { 8, 60 }, new int[] { 8, 60 }, new int[] { 9, 1472 }, new int[] { 9, 1536 }, new int[] { 9, 1600 }, new int[] { 9, 1728 }, new int[] { 7, 18 }, new int[] { 7, 18 }, new int[] { 7, 18 }, new int[] { 7, 18 }, new int[] { 7, 24 }, new int[] { 7, 24 }, new int[] { 7, 24 }, new int[] { 7, 24 }, new int[] { 8, 49 }, new int[] { 8, 49 }, new int[] { 8, 50 }, new int[] { 8, 50 }, new int[] { 8, 51 }, new int[] { 8, 51 }, new int[] { 8, 52 }, new int[] { 8, 52 }, new int[] { 7, 25 }, new int[] { 7, 25 }, new int[] { 7, 25 }, new int[] { 7, 25 }, new int[] { 8, 55 }, new int[] { 8, 55 }, new int[] { 8, 56 }, new int[] { 8, 56 }, new int[] { 8, 57 }, new int[] { 8, 57 }, new int[] { 8, 58 }, new int[] { 8, 58 }, new int[] { 6, 192 }, new int[] { 6, 192 }, new int[] { 6, 192 }, new int[] { 6, 192 }, new int[] { 6, 192 }, new int[] { 6, 192 }, new int[] { 6, 192 }, new int[] { 6, 192 }, new int[] { 6, 1664 }, new int[] { 6, 1664 }, new int[] { 6, 1664 }, new int[] { 6, 1664 }, new int[] { 6, 1664 }, new int[] { 6, 1664 }, new int[] { 6, 1664 }, new int[] { 6, 1664 }, new int[] { 8, 448 }, new int[] { 8, 448 }, new int[] { 8, 512 }, new int[] { 8, 512 }, new int[] { 9, 704 }, new int[] { 9, 768 }, new int[] { 8, 640 }, new int[] { 8, 640 }, new int[] { 8, 576 }, new int[] { 8, 576 }, new int[] { 9, 832 }, new int[] { 9, 896 }, new int[] { 9, 960 }, new int[] { 9, 1024 }, new int[] { 9, 1088 }, new int[] { 9, 1152 }, new int[] { 9, 1216 }, new int[] { 9, 1280 }, new int[] { 9, 1344 }, new int[] { 9, 1408 }, new int[] { 7, 256 }, new int[] { 7, 256 }, new int[] { 7, 256 }, new int[] { 7, 256 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 2 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 4, 3 }, new int[] { 5, 128 }, new int[] { 5, 128 }, new int[] { 5, 128 }, new int[] { 5, 128 }, new int[] { 5, 128 }, new int[] { 5, 128 }, new int[] { 5, 128 }, new int[] { 5, 128 }, new int[] { 5, 128 }, new int[] { 5, 128 }, new int[] { 5, 128 }, new int[] { 5, 128 }, new int[] { 5, 128 }, new int[] { 5, 128 }, new int[] { 5, 128 }, new int[] { 5, 128 }, new int[] { 5, 8 }, new int[] { 5, 8 }, new int[] { 5, 8 }, new int[] { 5, 8 }, new int[] { 5, 8 }, new int[] { 5, 8 }, new int[] { 5, 8 }, new int[] { 5, 8 }, new int[] { 5, 8 }, new int[] { 5, 8 }, new int[] { 5, 8 }, new int[] { 5, 8 }, new int[] { 5, 8 }, new int[] { 5, 8 }, new int[] { 5, 8 }, new int[] { 5, 8 }, new int[] { 5, 9 }, new int[] { 5, 9 }, new int[] { 5, 9 }, new int[] { 5, 9 }, new int[] { 5, 9 }, new int[] { 5, 9 }, new int[] { 5, 9 }, new int[] { 5, 9 }, new int[] { 5, 9 }, new int[] { 5, 9 }, new int[] { 5, 9 }, new int[] { 5, 9 }, new int[] { 5, 9 }, new int[] { 5, 9 }, new int[] { 5, 9 }, new int[] { 5, 9 }, new int[] { 6, 16 }, new int[] { 6, 16 }, new int[] { 6, 16 }, new int[] { 6, 16 }, new int[] { 6, 16 }, new int[] { 6, 16 }, new int[] { 6, 16 }, new int[] { 6, 16 }, new int[] { 6, 17 }, new int[] { 6, 17 }, new int[] { 6, 17 }, new int[] { 6, 17 }, new int[] { 6, 17 }, new int[] { 6, 17 }, new int[] { 6, 17 }, new int[] { 6, 17 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 4 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 6, 14 }, new int[] { 6, 14 }, new int[] { 6, 14 }, new int[] { 6, 14 }, new int[] { 6, 14 }, new int[] { 6, 14 }, new int[] { 6, 14 }, new int[] { 6, 14 }, new int[] { 6, 15 }, new int[] { 6, 15 }, new int[] { 6, 15 }, new int[] { 6, 15 }, new int[] { 6, 15 }, new int[] { 6, 15 }, new int[] { 6, 15 }, new int[] { 6, 15 }, new int[] { 5, 64 }, new int[] { 5, 64 }, new int[] { 5, 64 }, new int[] { 5, 64 }, new int[] { 5, 64 }, new int[] { 5, 64 }, new int[] { 5, 64 }, new int[] { 5, 64 }, new int[] { 5, 64 }, new int[] { 5, 64 }, new int[] { 5, 64 }, new int[] { 5, 64 }, new int[] { 5, 64 }, new int[] { 5, 64 }, new int[] { 5, 64 }, new int[] { 5, 64 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 }, new int[] { 4, 7 } };
        /// <summary>
        /// black run lengths </summary>
        private int[][] m_blackTable1 = { new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { 12, CcittEndOfLine }, new int[] { 12, CcittEndOfLine }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { 11, 1792 }, new int[] { 11, 1792 }, new int[] { 11, 1792 }, new int[] { 11, 1792 }, new int[] { 12, 1984 }, new int[] { 12, 1984 }, new int[] { 12, 2048 }, new int[] { 12, 2048 }, new int[] { 12, 2112 }, new int[] { 12, 2112 }, new int[] { 12, 2176 }, new int[] { 12, 2176 }, new int[] { 12, 2240 }, new int[] { 12, 2240 }, new int[] { 12, 2304 }, new int[] { 12, 2304 }, new int[] { 11, 1856 }, new int[] { 11, 1856 }, new int[] { 11, 1856 }, new int[] { 11, 1856 }, new int[] { 11, 1920 }, new int[] { 11, 1920 }, new int[] { 11, 1920 }, new int[] { 11, 1920 }, new int[] { 12, 2368 }, new int[] { 12, 2368 }, new int[] { 12, 2432 }, new int[] { 12, 2432 }, new int[] { 12, 2496 }, new int[] { 12, 2496 }, new int[] { 12, 2560 }, new int[] { 12, 2560 }, new int[] { 10, 18 }, new int[] { 10, 18 }, new int[] { 10, 18 }, new int[] { 10, 18 }, new int[] { 10, 18 }, new int[] { 10, 18 }, new int[] { 10, 18 }, new int[] { 10, 18 }, new int[] { 12, 52 }, new int[] { 12, 52 }, new int[] { 13, 640 }, new int[] { 13, 704 }, new int[] { 13, 768 }, new int[] { 13, 832 }, new int[] { 12, 55 }, new int[] { 12, 55 }, new int[] { 12, 56 }, new int[] { 12, 56 }, new int[] { 13, 1280 }, new int[] { 13, 1344 }, new int[] { 13, 1408 }, new int[] { 13, 1472 }, new int[] { 12, 59 }, new int[] { 12, 59 }, new int[] { 12, 60 }, new int[] { 12, 60 }, new int[] { 13, 1536 }, new int[] { 13, 1600 }, new int[] { 11, 24 }, new int[] { 11, 24 }, new int[] { 11, 24 }, new int[] { 11, 24 }, new int[] { 11, 25 }, new int[] { 11, 25 }, new int[] { 11, 25 }, new int[] { 11, 25 }, new int[] { 13, 1664 }, new int[] { 13, 1728 }, new int[] { 12, 320 }, new int[] { 12, 320 }, new int[] { 12, 384 }, new int[] { 12, 384 }, new int[] { 12, 448 }, new int[] { 12, 448 }, new int[] { 13, 512 }, new int[] { 13, 576 }, new int[] { 12, 53 }, new int[] { 12, 53 }, new int[] { 12, 54 }, new int[] { 12, 54 }, new int[] { 13, 896 }, new int[] { 13, 960 }, new int[] { 13, 1024 }, new int[] { 13, 1088 }, new int[] { 13, 1152 }, new int[] { 13, 1216 }, new int[] { 10, 64 }, new int[] { 10, 64 }, new int[] { 10, 64 }, new int[] { 10, 64 }, new int[] { 10, 64 }, new int[] { 10, 64 }, new int[] { 10, 64 }, new int[] { 10, 64 } };
        private int[][] m_blackTable2 = { new int[] { 8, 13 }, new int[] { 8, 13 }, new int[] { 8, 13 }, new int[] { 8, 13 }, new int[] { 8, 13 }, new int[] { 8, 13 }, new int[] { 8, 13 }, new int[] { 8, 13 }, new int[] { 8, 13 }, new int[] { 8, 13 }, new int[] { 8, 13 }, new int[] { 8, 13 }, new int[] { 8, 13 }, new int[] { 8, 13 }, new int[] { 8, 13 }, new int[] { 8, 13 }, new int[] { 11, 23 }, new int[] { 11, 23 }, new int[] { 12, 50 }, new int[] { 12, 51 }, new int[] { 12, 44 }, new int[] { 12, 45 }, new int[] { 12, 46 }, new int[] { 12, 47 }, new int[] { 12, 57 }, new int[] { 12, 58 }, new int[] { 12, 61 }, new int[] { 12, 256 }, new int[] { 10, 16 }, new int[] { 10, 16 }, new int[] { 10, 16 }, new int[] { 10, 16 }, new int[] { 10, 17 }, new int[] { 10, 17 }, new int[] { 10, 17 }, new int[] { 10, 17 }, new int[] { 12, 48 }, new int[] { 12, 49 }, new int[] { 12, 62 }, new int[] { 12, 63 }, new int[] { 12, 30 }, new int[] { 12, 31 }, new int[] { 12, 32 }, new int[] { 12, 33 }, new int[] { 12, 40 }, new int[] { 12, 41 }, new int[] { 11, 22 }, new int[] { 11, 22 }, new int[] { 8, 14 }, new int[] { 8, 14 }, new int[] { 8, 14 }, new int[] { 8, 14 }, new int[] { 8, 14 }, new int[] { 8, 14 }, new int[] { 8, 14 }, new int[] { 8, 14 }, new int[] { 8, 14 }, new int[] { 8, 14 }, new int[] { 8, 14 }, new int[] { 8, 14 }, new int[] { 8, 14 }, new int[] { 8, 14 }, new int[] { 8, 14 }, new int[] { 8, 14 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 10 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 7, 11 }, new int[] { 9, 15 }, new int[] { 9, 15 }, new int[] { 9, 15 }, new int[] { 9, 15 }, new int[] { 9, 15 }, new int[] { 9, 15 }, new int[] { 9, 15 }, new int[] { 9, 15 }, new int[] { 12, 128 }, new int[] { 12, 192 }, new int[] { 12, 26 }, new int[] { 12, 27 }, new int[] { 12, 28 }, new int[] { 12, 29 }, new int[] { 11, 19 }, new int[] { 11, 19 }, new int[] { 11, 20 }, new int[] { 11, 20 }, new int[] { 12, 34 }, new int[] { 12, 35 }, new int[] { 12, 36 }, new int[] { 12, 37 }, new int[] { 12, 38 }, new int[] { 12, 39 }, new int[] { 11, 21 }, new int[] { 11, 21 }, new int[] { 12, 42 }, new int[] { 12, 43 }, new int[] { 10, 0 }, new int[] { 10, 0 }, new int[] { 10, 0 }, new int[] { 10, 0 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 }, new int[] { 7, 12 } };
        private int[][] m_blackTable3 = { new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { 6, 9 }, new int[] { 6, 8 }, new int[] { 5, 7 }, new int[] { 5, 7 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 6 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 4, 5 }, new int[] { 3, 1 }, new int[] { 3, 1 }, new int[] { 3, 1 }, new int[] { 3, 1 }, new int[] { 3, 1 }, new int[] { 3, 1 }, new int[] { 3, 1 }, new int[] { 3, 1 }, new int[] { 3, 4 }, new int[] { 3, 4 }, new int[] { 3, 4 }, new int[] { 3, 4 }, new int[] { 3, 4 }, new int[] { 3, 4 }, new int[] { 3, 4 }, new int[] { 3, 4 }, new int[] { 2, 3 }, new int[] { 2, 3 }, new int[] { 2, 3 }, new int[] { 2, 3 }, new int[] { 2, 3 }, new int[] { 2, 3 }, new int[] { 2, 3 }, new int[] { 2, 3 }, new int[] { 2, 3 }, new int[] { 2, 3 }, new int[] { 2, 3 }, new int[] { 2, 3 }, new int[] { 2, 3 }, new int[] { 2, 3 }, new int[] { 2, 3 }, new int[] { 2, 3 }, new int[] { 2, 2 }, new int[] { 2, 2 }, new int[] { 2, 2 }, new int[] { 2, 2 }, new int[] { 2, 2 }, new int[] { 2, 2 }, new int[] { 2, 2 }, new int[] { 2, 2 }, new int[] { 2, 2 }, new int[] { 2, 2 }, new int[] { 2, 2 }, new int[] { 2, 2 }, new int[] { 2, 2 }, new int[] { 2, 2 }, new int[] { 2, 2 }, new int[] { 2, 2 } };
        
        internal const int CcittEndOfLine = -2;
        internal const int TwoDimensionalPass = 0;
        internal const int TwoDimensionalHorizontal = 1;
        internal const int TwoDimensionalVertical0 = 2;
        internal const int TwoDimensionalVerticalR1 = 3;
        internal const int TwoDimensionalVerticalL1 = 4;
        internal const int TwoDimensionalVerticalR2 = 5;
        internal const int TwoDimensionalVerticalL2 = 6;
        internal const int TwoDimensionalVerticalR3 = 7;
        internal const int TwoDimensionalVerticalL3 = 8;

        internal MMRDecoder(Jbig2StreamReader reader)
        {
            this.m_reader = reader;
        }

        internal void Reset()
        {
            m_bufferLength = 0;
            m_noOfBytesRead = 0;
            m_buffer = 0;
        }

        internal void SkipTo(int length)
        {
            while (m_noOfBytesRead < length)
            {
                m_reader.ReadByte();
                m_noOfBytesRead++;
            }
        }

        internal long Get24Bits()
        {
            while (m_bufferLength < 24)
            {

                m_buffer = ((m_bitOperation.Bit32Shift(m_buffer, 8, BitOperation.LEFT_SHIFT)) | (m_reader.ReadByte() & 0xff));
                m_bufferLength += 8;
                m_noOfBytesRead++;
            }

            return (m_bitOperation.Bit32Shift(m_buffer, (int)(m_bufferLength - 24), BitOperation.RIGHT_SHIFT)) & 0xffffff;
        }

        internal int Get2DCode()
        {
            int[] tuple;

            if (m_bufferLength == 0)
            {
                m_buffer = (m_reader.ReadByte() & 0xff);
                if (m_reader.bytePointer == 297)
                {
                }
                m_bufferLength = 8;

                m_noOfBytesRead++;

                int lookup = (int)((m_bitOperation.Bit32Shift(m_buffer, 1, BitOperation.RIGHT_SHIFT)) & 0x7f);

                tuple = m_twoDimensionalTable1[lookup];
            }
            else if (m_bufferLength == 8)
            {
                int lookup = (int)((m_bitOperation.Bit32Shift(m_buffer, 1, BitOperation.RIGHT_SHIFT)) & 0x7f);
                tuple = m_twoDimensionalTable1[lookup];
            }
            else
            {
                int lookup = (int)((m_bitOperation.Bit32Shift(m_buffer, (int)(7 - m_bufferLength), BitOperation.LEFT_SHIFT)) & 0x7f);

                tuple = m_twoDimensionalTable1[lookup];
                if (tuple[0] < 0 || tuple[0] > (int)m_bufferLength)
                {
                    int right = (m_reader.ReadByte() & 0xff);

                    long left = (m_bitOperation.Bit32Shift(m_buffer, 8, BitOperation.LEFT_SHIFT));

                    m_buffer = left | right;
                    m_bufferLength += 8;
                    m_noOfBytesRead++;

                    int look = (int)(m_bitOperation.Bit32Shift(m_buffer, (int)(m_bufferLength - 7), BitOperation.RIGHT_SHIFT) & 0x7f);

                    tuple = m_twoDimensionalTable1[look];
                }
            }
            if (tuple[0] < 0)
            {
                return 0;
            }
            m_bufferLength -= tuple[0];

            return tuple[1];
        }
        internal int GetWhiteCode()
        {
            int[] tuple;
            long code;

            if (m_bufferLength == 0)
            {
                m_buffer = (m_reader.ReadByte() & 0xff);
                m_bufferLength = 8;
                m_noOfBytesRead++;
            }
            while (true)
            {
                if (m_bufferLength >= 7 && ((m_bitOperation.Bit32Shift(m_buffer, (int)(m_bufferLength - 7), BitOperation.RIGHT_SHIFT)) & 0x7f) == 0)
                {
                    if (m_bufferLength <= 12)
                    {
                        code = m_bitOperation.Bit32Shift(m_buffer, (int)(12 - m_bufferLength), BitOperation.LEFT_SHIFT);
                    }
                    else
                    {
                        code = m_bitOperation.Bit32Shift(m_buffer, (int)(m_bufferLength - 12), BitOperation.RIGHT_SHIFT);
                    }

                    tuple = m_whiteTable1[(int)(code & 0x1f)];
                }
                else
                {
                    if (m_bufferLength <= 9)
                    {
                        code = m_bitOperation.Bit32Shift(m_buffer, (int)(9 - m_bufferLength), BitOperation.LEFT_SHIFT);
                    }
                    else
                    {
                        code = m_bitOperation.Bit32Shift(m_buffer, (int)(m_bufferLength - 9), BitOperation.RIGHT_SHIFT);
                    }

                    int lookup = (int)(code & 0x1ff);
                    if (lookup >= 0)
                    {
                        tuple = m_whiteTable2[lookup];
                    }
                    else
                    {
                        tuple = m_whiteTable2[m_whiteTable2.Length + lookup];
                    }
                }
                if (tuple[0] > 0 && tuple[0] <= (int)m_bufferLength)
                {
                    m_bufferLength -= tuple[0];
                    return tuple[1];
                }
                if (m_bufferLength >= 12)
                {
                    break;
                }
                m_buffer = ((m_bitOperation.Bit32Shift(m_buffer, 8, BitOperation.LEFT_SHIFT)) | m_reader.ReadByte() & 0xff);
                m_bufferLength += 8;
                m_noOfBytesRead++;
            }
            m_bufferLength--;

            return 1;
        }

        internal int GetblackCode()
        {
            int[] tuple;
            long code;

            if (m_bufferLength == 0)
            {
                m_buffer = (m_reader.ReadByte() & 0xff);
                m_bufferLength = 8;
                m_noOfBytesRead++;
            }
            while (true)
            {
                if (m_bufferLength >= 6 && ((m_bitOperation.Bit32Shift(m_buffer, (int)(m_bufferLength - 6), BitOperation.RIGHT_SHIFT)) & 0x3f) == 0)
                {
                    if (m_bufferLength <= 13)
                    {
                        code = m_bitOperation.Bit32Shift(m_buffer, (int)(13 - m_bufferLength), BitOperation.LEFT_SHIFT);
                    }
                    else
                    {
                        code = m_bitOperation.Bit32Shift(m_buffer, (int)(m_bufferLength - 13), BitOperation.RIGHT_SHIFT);
                    }
                    tuple = m_blackTable1[(int)(code & 0x7f)];
                }
                else if (m_bufferLength >= 4 && (((int)m_buffer >> (int)(m_bufferLength - 4)) & 0x0f) == 0)
                {
                    if (m_bufferLength <= 12)
                    {
                        code = m_bitOperation.Bit32Shift(m_buffer, (int)(12 - m_bufferLength), BitOperation.LEFT_SHIFT);
                    }
                    else
                    {
                        code = m_bitOperation.Bit32Shift(m_buffer, (int)(m_bufferLength - 12), BitOperation.RIGHT_SHIFT);
                    }

                    int lookup = (int)((code & 0xff) - 64);
                    if (lookup >= 0)
                    {
                        tuple = m_blackTable2[lookup];
                    }
                    else
                    {
                        tuple = m_blackTable1[m_blackTable1.Length + lookup];
                    }
                }
                else
                {
                    if (m_bufferLength <= 6)
                    {
                        code = m_bitOperation.Bit32Shift(m_buffer, (int)(6 - m_bufferLength), BitOperation.LEFT_SHIFT);
                    }
                    else
                    {
                        code = m_bitOperation.Bit32Shift(m_buffer, (int)(m_bufferLength - 6), BitOperation.RIGHT_SHIFT);
                    }

                    int lookup = (int)(code & 0x3f);
                    if (lookup >= 0)
                    {
                        tuple = m_blackTable3[lookup];
                    }
                    else
                    {
                        tuple = m_blackTable2[m_blackTable2.Length + lookup];
                    }
                }
                if (tuple[0] > 0 && tuple[0] <= (int)m_bufferLength)
                {
                    m_bufferLength -= tuple[0];
                    return tuple[1];
                }
                if (m_bufferLength >= 13)
                {
                    break;
                }
                m_buffer = ((m_bitOperation.Bit32Shift(m_buffer, 8, BitOperation.LEFT_SHIFT)) | (m_reader.ReadByte() & 0xff));
                m_bufferLength += 8;
                m_noOfBytesRead++;
            }
            m_bufferLength--;
            return 1;
        }
        
        
    }
}

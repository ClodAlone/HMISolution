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

namespace Syncfusion.Pdf.Compression
{
    internal static class JBIG2Statics
    {
        internal static int[] MatchOffset = { 0, 0, 0, 1, -1, 0, 0, -1, 1, 0, -1, 1, 1, 1, -1, -1, 1, -1, 0, -2, 2, 0, 0, 2, -2, 0, -1, -2, 1, -2, 2, -1, 2, 1, 1, 2, -1, 2, -2, 1, -2, -1, -2, -2, 2, -2, 2, 2, -2, 2 };
        internal static int PixSrc = 0xc << 1;
        internal static int PixDst = 0xa << 1;
        internal static int PixClr = 0x0 << 1;
        internal static int PixSet = 0xf << 1;
        internal static int PixPaint = (PixSrc | PixDst);
        internal static int PixMask = (PixSrc & PixDst);
        internal static int PixSubtract = (PixDst & PixNot(PixSrc));
        internal static int PixXor = (PixSrc ^ PixDst);
        internal static int SelDontCare = 0;
        internal static int SelHit = 1;
        internal static int SelMiss = 2;
        internal static int Undef = -1;
        internal static int ShiftLeft = 0;
        internal static int ShiftRight = 1;
        internal static int SelectIfLt = 1, SelectIfGt = 2, SelectIfLte = 3, SelectIfGte = 4;
        internal const int SelectWidth = 1, SelectHeight = 2, SelectIfEither = 3, SelectIfBoth = 4;
        internal const int Unknown = 0,
    Bmp = 1,
    JfifJpeg = 2,
    Png = 3,
    Tiff = 4,
    TiffPackBits = 5,
    TiffRle = 6,
    TiffG3 = 7,
    TiffG4 = 8,
    TiffLzw = 9,
    TiffZip = 10,
    TiffPnm = 11,
    TiffPs = 12,
    Gif = 13,
    Jp2 = 14,
    Webp = 15,
    Lpdf = 16,
    Default = 17,
    Spix = 18;
        internal const int CompressionNone = 1,
CompressionCcittRle = 2,
CompressionCcittFax3 = 3, CompressionCcittT4 = 3,
CompressionCcittFax4 = 4, CompressionCcittT6 = 4,
CompressionLzw = 5,
CompressionOJpeg = 6,
CompressionJpeg = 7,
CompressionNext = 32766,
CompressionCcittRlew = 32771,
CompressionPackBits = 32773,
CompressionDeflate = 32946,
CompressionAdobeDeflate = 8;

        internal const float RedWeight = 0.3f;
        internal const float GreenWeight = 0.5f;
        internal const float BlueWeight = 0.2f;
        internal static uint[] RightMask = new uint[] {0x0, 0x00000001, 0x00000003, 0x00000007, 0x0000000f, 0x0000001f, 0x0000003f, 
            0x0000007f, 0x000000ff, 0x000001ff, 0x000003ff, 0x000007ff, 0x00000fff, 0x00001fff, 0x00003fff, 0x00007fff, 0x0000ffff, 
            0x0001ffff, 0x0003ffff, 0x0007ffff, 0x000fffff, 0x001fffff, 0x003fffff, 0x007fffff, 0x00ffffff, 0x01ffffff, 0x03ffffff, 
            0x07ffffff, 0x0fffffff, 0x1fffffff, 0x3fffffff, 0x7fffffff, 0xffffffff};

        internal static uint[] LeftMask = new uint[]{0x0, 0x80000000, 0xc0000000, 0xe0000000, 0xf0000000,
    0xf8000000, 0xfc000000, 0xfe000000, 0xff000000, 0xff800000, 0xffc00000, 0xffe00000, 0xfff00000,
    0xfff80000, 0xfffc0000, 0xfffe0000, 0xffff0000, 0xffff8000, 0xffffc000, 0xffffe000, 0xfffff000,
    0xfffff800, 0xfffffc00, 0xfffffe00, 0xffffff00, 0xffffff80, 0xffffffc0, 0xffffffe0, 0xfffffff0,
    0xfffffff8, 0xfffffffc, 0xfffffffe, 0xffffffff};

        internal static uint Htonl(object p)
        {
            byte[] array = null;

            if (p is int)
                array = BitConverter.GetBytes((int)p);
            if (p is uint)
                array = BitConverter.GetBytes((uint)p);
            if (p is char)
                array = BitConverter.GetBytes((char)p);
            if (p is float)
                array = BitConverter.GetBytes((float)p);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(array);
            return BitConverter.ToUInt32(array, 0);
        }

        internal static byte[] Htonl(uint p)
        {
            byte[] array = BitConverter.GetBytes((uint)p);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(array);
            return array;
        }

        internal static int PixNot(int op)
        {
            return ((op) ^ 0x1e);
        }

        internal static void ClearDataBit(ref uint[] line, int index, int n)
        {
            if (n > 31)
            {
                index = index + (n / 32);
                n = n % 32;
            }

            int quo = n / 32; // 8 bits per byte; 4 bytes per uint. n is bit
            string binary = Convert.ToString(line[index], 2);
            while (binary.Length < 32)
                binary = string.Concat("0", binary);
            char[] temp = binary.ToCharArray();
            temp[n] = '0'; // clear bit.
            binary = new string(temp);
            uint output = Convert.ToUInt32(binary, 2);
            line[index] = output;
        }

        internal static void ClearDataBit(ref uint line, int n)
        {
            if (n > 31)
                return;
            string binary = Convert.ToString(line, 2);
            while (binary.Length < 32)
                binary = string.Concat("0", binary);
            char[] temp = binary.ToCharArray();
            temp[n] = '0'; // clear bit.
            binary = new string(temp);
            uint output = Convert.ToUInt32(binary, 2);
            line = output;
        }

        internal static uint GetDataBit(uint[] line, int index, int n)
        {
            int rem = n % 32; //n is bit index
            int quo = index + (n / 32);

            string binary = Convert.ToString(line[quo], 2);
            while (binary.Length < 32)
                binary = string.Concat("0", binary);
            uint output = uint.Parse(binary.Substring(rem, 1));
            return output;
        }

        internal static uint GetDataDibit(uint line, int n)
        {
            return (line >> (2 * (15 - (n & 15)))) & 3;
        }

        internal static uint GetDataQbit(uint line, int n)
        {
            return (line >> (4 * (7 - (n & 7)))) & 0xf;
        }

        internal static void SetDataBit(ref uint line, int n)
        {
            line |= (uint)(0x80000000 >> (n & 31));
        }

        internal static void SetDataQbit(ref uint[] line, int n, uint val)
        {
            uint pword = line[n] >> 3;
            //pword &= ~(0xf0000000 >> (4 * (n & 7)));  /* clear */
            pword |= (val & 15) << (28 - 4 * (n & 7));   /* set */
            line[n] = pword;
        }

        internal static void SetDataDibit(ref uint[] line, int n, uint val)
        {
            uint pword = line[n] >> 4;
            //pword &= ~(0xc0000000 >> (2 * (n & 15)));  /* clear */
            pword |= (val & 3) << (30 - 2 * (n & 15));   /* set */

            line[n] = pword;
        }

        internal static uint GetDataByte(uint[] line, int n)
        {
            return line[n];
        }

        internal static uint GetDataByte(uint line, int n)
        {
            return BitConverter.GetBytes(line)[n];
        }

        internal static void SetDataByte(ref uint[] line, int n, uint val)
        {
            byte val0, val1, val2, val3;
            int rem = n % 4;
            int quo = n / 4;
            byte[] ori = BitConverter.GetBytes(line[quo]);
            val3 = ori[3]; val2 = ori[2]; val1 = ori[1]; val0 = ori[0];
            switch (rem)
            {
                case 0:
                    val3 = (byte)val;
                    break;
                case 1:
                    val2 = (byte)val;
                    break;
                case 2:
                    val1 = (byte)val;
                    break;
                case 3:
                    val0 = (byte)val;
                    break;
            }
            byte[] converted = new byte[] { val0, val1, val2, val3 };
            line[quo] = (uint)BitConverter.ToUInt32(converted, 0);
        }

        internal static void SetDataTwoBytes(ref uint[] line, int n, short val)
        {
            byte val0, val1, val2, val3;
            int rem = n % 4;
            int quo = n / 4;
            byte[] ori = BitConverter.GetBytes(line[quo]);
            val3 = ori[3]; val2 = ori[2]; val1 = ori[1]; val0 = ori[0];
            byte[] temp = BitConverter.GetBytes(val);
            switch (rem)
            {
                case 0:
                    val3 = (byte)val;
                    break;
                case 1:
                    val2 = (byte)val;
                    break;
                case 2:
                    val1 = (byte)val;
                    break;
                case 3:
                    val0 = (byte)val;
                    break;
            }
            byte[] converted = new byte[] { val0, val1, val2, val3 };
            line[quo] = (uint)BitConverter.ToUInt32(converted, 0);
        }

        internal static short GetDataTwoBytes(uint[] line, int n)
        {
            return (short)(line[n] ^ 2);
        }

        internal static Pixa CreatePixa(int n)
        {
            const int INITIAL_PTR_ARRAYSIZE = 20;

            if (n <= 0)
                n = INITIAL_PTR_ARRAYSIZE;

            Pixa pixa = new Pixa(n);
            pixa.Boxa = CreateBoxa(n);
            if (pixa.Boxa == null)
            {
                throw new Exception(); //return (PIXA*)ERROR_PTR("boxa not made", procName, null);
            }

            return pixa;
        }

        internal static Boxa CreateBoxa(int n)
        {
            const int INITIAL_PTR_ARRAYSIZE = 50;

            if (n <= 0)
                n = INITIAL_PTR_ARRAYSIZE;

            Boxa boxa = new Boxa(n);
            return boxa;
        }

        internal static Numa CreateNuma(int n)
        {
            Numa na;
            int INITIAL_PTR_ARRAYSIZE = 50;

            if (n <= 0)
                n = INITIAL_PTR_ARRAYSIZE;

            na = new Numa(n);

            return na;
        }

        internal static L_Stack CreateLStack(int nalloc)
        {
            int INITIAL_PTR_ARRAYSIZE = 20;

            if (nalloc <= 0)
                nalloc = INITIAL_PTR_ARRAYSIZE;

            L_Stack lstack = new L_Stack(nalloc);

            return lstack;
        }

        internal static Pta CreatePta(int n)
        {
            int INITIAL_PTR_ARRAYSIZE = 20;

            if (n <= 0)
                n = INITIAL_PTR_ARRAYSIZE;

            Pta pta = new Pta(n);
            pta.N = 0;
            pta.Nalloc = n;
            pta.RefCount += 1;  /* sets to 1 */

            return pta;
        }

        internal static PixColormap CreatePixCmap(int depth)
        {
            PixColormap cmap = new PixColormap();
            int count = 1 << depth;
            if (depth != 1 && depth != 2 && depth != 4 && depth != 8)
            { }// return (PIXCMAP*)ERROR_PTR("depth not in {1,2,4,8}", procName, NULL);

            cmap.Depth = depth;
            cmap.Nalloc = 1 << depth;
            cmap.Array = new RGBA_Quad[count];
            cmap.N = 0;

            return cmap;
        }

        internal static Sel CreateSel(int height, int weight, string name)
        {
            Sel sel = new Sel();
            sel.SY = height;
            sel.SX = weight;
            sel.Name = name;

            List<int[]> array = new List<int[]>(height);

            for (int i = 0; i < height; i++)
                array.Add(new int[weight]);

            sel.Data = array;

            return sel;
        }
    }
}
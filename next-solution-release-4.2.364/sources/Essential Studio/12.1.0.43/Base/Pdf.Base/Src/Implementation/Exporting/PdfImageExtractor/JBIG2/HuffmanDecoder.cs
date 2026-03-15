#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;

namespace Syncfusion.Pdf
{
    class HuffmanDecoder
    {
        private Jbig2StreamReader reader;

        internal int jbig2HuffmanLOW = int.Parse("fffffffd", System.Globalization.NumberStyles.HexNumber);
        internal int jbig2HuffmanOOB = int.Parse("fffffffe", System.Globalization.NumberStyles.HexNumber);
        internal int jbig2HuffmanEOT = int.Parse("ffffffff", System.Globalization.NumberStyles.HexNumber);
        internal int[][] huffmanTableA;
        internal int[][] huffmanTableB;
        internal int[][] huffmanTableC;
        internal int[][] huffmanTableD;
        internal int[][] huffmanTableE;
        internal int[][] huffmanTableF;
        internal int[][] huffmanTableG;
        internal int[][] huffmanTableH;
        internal int[][] huffmanTableI;
        internal int[][] huffmanTableJ;
        internal int[][] huffmanTableK;
        internal int[][] huffmanTableL;
        internal int[][] huffmanTableM;
        internal int[][] huffmanTableN;
        internal int[][] huffmanTableO;

        internal HuffmanDecoder(Jbig2StreamReader reader)
        {
            this.reader = reader;
            Initialize();
        }

        internal HuffmanDecoder()
        {
            Initialize();
        }

        internal void Initialize()
        {
            huffmanTableA = new int[][] { new int[] { 0, 1, 4, 0x000 }, new int[] { 16, 2, 8, 0x002 }, new int[] { 272, 3, 16, 0x006 }, new int[] { 65808, 3, 32, 0x007 }, new int[] { 0, 0, jbig2HuffmanEOT, 0 } };
            huffmanTableB = new int[][] { new int[] { 0, 1, 0, 0x000 }, new int[] { 1, 2, 0, 0x002 }, new int[] { 2, 3, 0, 0x006 }, new int[] { 3, 4, 3, 0x00e }, new int[] { 11, 5, 6, 0x01e }, new int[] { 75, 6, 32, 0x03e }, new int[] { 0, 6, jbig2HuffmanOOB, 0x03f }, new int[] { 0, 0, jbig2HuffmanEOT, 0 } };
            huffmanTableC = new int[][] { new int[] { 0, 1, 0, 0x000 }, new int[] { 1, 2, 0, 0x002 }, new int[] { 2, 3, 0, 0x006 }, new int[] { 3, 4, 3, 0x00e }, new int[] { 11, 5, 6, 0x01e }, new int[] { 0, 6, jbig2HuffmanOOB, 0x03e }, new int[] { 75, 7, 32, 0x0fe }, new int[] { -256, 8, 8, 0x0fe }, new int[] { -257, 8, jbig2HuffmanLOW, 0x0ff }, new int[] { 0, 0, jbig2HuffmanEOT, 0 } };
            huffmanTableD = new int[][] { new int[] { 1, 1, 0, 0x000 }, new int[] { 2, 2, 0, 0x002 }, new int[] { 3, 3, 0, 0x006 }, new int[] { 4, 4, 3, 0x00e }, new int[] { 12, 5, 6, 0x01e }, new int[] { 76, 5, 32, 0x01f }, new int[] { 0, 0, jbig2HuffmanEOT, 0 } };
            huffmanTableE = new int[][] { new int[] { 1, 1, 0, 0x000 }, new int[] { 2, 2, 0, 0x002 }, new int[] { 3, 3, 0, 0x006 }, new int[] { 4, 4, 3, 0x00e }, new int[] { 12, 5, 6, 0x01e }, new int[] { 76, 6, 32, 0x03e }, new int[] { -255, 7, 8, 0x07e }, new int[] { -256, 7, jbig2HuffmanLOW, 0x07f }, new int[] { 0, 0, jbig2HuffmanEOT, 0 } };
            huffmanTableF = new int[][] { new int[] { 0, 2, 7, 0x000 }, new int[] { 128, 3, 7, 0x002 }, new int[] { 256, 3, 8, 0x003 }, new int[] { -1024, 4, 9, 0x008 }, new int[] { -512, 4, 8, 0x009 }, new int[] { -256, 4, 7, 0x00a }, new int[] { -32, 4, 5, 0x00b }, new int[] { 512, 4, 9, 0x00c }, new int[] { 1024, 4, 10, 0x00d }, new int[] { -2048, 5, 10, 0x01c }, new int[] { -128, 5, 6, 0x01d }, new int[] { -64, 5, 5, 0x01e }, new int[] { -2049, 6, jbig2HuffmanLOW, 0x03e }, new int[] { 2048, 6, 32, 0x03f }, new int[] { 0, 0, jbig2HuffmanEOT, 0 } };
            huffmanTableG = new int[][] { new int[] { -512, 3, 8, 0x000 }, new int[] { 256, 3, 8, 0x001 }, new int[] { 512, 3, 9, 0x002 }, new int[] { 1024, 3, 10, 0x003 }, new int[] { -1024, 4, 9, 0x008 }, new int[] { -256, 4, 7, 0x009 }, new int[] { -32, 4, 5, 0x00a }, new int[] { 0, 4, 5, 0x00b }, new int[] { 128, 4, 7, 0x00c }, new int[] { -128, 5, 6, 0x01a }, new int[] { -64, 5, 5, 0x01b }, new int[] { 32, 5, 5, 0x01c }, new int[] { 64, 5, 6, 0x01d }, new int[] { -1025, 5, jbig2HuffmanLOW, 0x01e }, new int[] { 2048, 5, 32, 0x01f }, new int[] { 0, 0, jbig2HuffmanEOT, 0 } };
            huffmanTableH = new int[][] { new int[] { 0, 2, 1, 0x000 }, new int[] { 0, 2, jbig2HuffmanOOB, 0x001 }, new int[] { 4, 3, 4, 0x004 }, new int[] { -1, 4, 0, 0x00a }, new int[] { 22, 4, 4, 0x00b }, new int[] { 38, 4, 5, 0x00c }, new int[] { 2, 5, 0, 0x01a }, new int[] { 70, 5, 6, 0x01b }, new int[] { 134, 5, 7, 0x01c }, new int[] { 3, 6, 0, 0x03a }, new int[] { 20, 6, 1, 0x03b }, new int[] { 262, 6, 7, 0x03c }, new int[] { 646, 6, 10, 0x03d }, new int[] { -2, 7, 0, 0x07c }, new int[] { 390, 7, 8, 0x07d }, new int[] { -15, 8, 3, 0x0fc }, new int[] { -5, 8, 1, 0x0fd }, new int[] { -7, 9, 1, 0x1fc }, new int[] { -3, 9, 0, 0x1fd }, new int[] { -16, 9, jbig2HuffmanLOW, 0x1fe }, new int[] { 1670, 9, 32, 0x1ff }, new int[] { 0, 0, jbig2HuffmanEOT, 0 } };
            huffmanTableI = new int[][] { new int[] { 0, 2, jbig2HuffmanOOB, 0x000 }, new int[] { -1, 3, 1, 0x002 }, new int[] { 1, 3, 1, 0x003 }, new int[] { 7, 3, 5, 0x004 }, new int[] { -3, 4, 1, 0x00a }, new int[] { 43, 4, 5, 0x00b }, new int[] { 75, 4, 6, 0x00c }, new int[] { 3, 5, 1, 0x01a }, new int[] { 139, 5, 7, 0x01b }, new int[] { 267, 5, 8, 0x01c }, new int[] { 5, 6, 1, 0x03a }, new int[] { 39, 6, 2, 0x03b }, new int[] { 523, 6, 8, 0x03c }, new int[] { 1291, 6, 11, 0x03d }, new int[] { -5, 7, 1, 0x07c }, new int[] { 779, 7, 9, 0x07d }, new int[] { -31, 8, 4, 0x0fc }, new int[] { -11, 8, 2, 0x0fd }, new int[] { -15, 9, 2, 0x1fc }, new int[] { -7, 9, 1, 0x1fd }, new int[] { -32, 9, jbig2HuffmanLOW, 0x1fe }, new int[] { 3339, 9, 32, 0x1ff }, new int[] { 0, 0, jbig2HuffmanEOT, 0 } };
            huffmanTableJ = new int[][] { new int[] { -2, 2, 2, 0x000 }, new int[] { 6, 2, 6, 0x001 }, new int[] { 0, 2, jbig2HuffmanOOB, 0x002 }, new int[] { -3, 5, 0, 0x018 }, new int[] { 2, 5, 0, 0x019 }, new int[] { 70, 5, 5, 0x01a }, new int[] { 3, 6, 0, 0x036 }, new int[] { 102, 6, 5, 0x037 }, new int[] { 134, 6, 6, 0x038 }, new int[] { 198, 6, 7, 0x039 }, new int[] { 326, 6, 8, 0x03a }, new int[] { 582, 6, 9, 0x03b }, new int[] { 1094, 6, 10, 0x03c }, new int[] { -21, 7, 4, 0x07a }, new int[] { -4, 7, 0, 0x07b }, new int[] { 4, 7, 0, 0x07c }, new int[] { 2118, 7, 11, 0x07d }, new int[] { -5, 8, 0, 0x0fc }, new int[] { 5, 8, 0, 0x0fd }, new int[] { -22, 8, jbig2HuffmanLOW, 0x0fe }, new int[] { 4166, 8, 32, 0x0ff }, new int[] { 0, 0, jbig2HuffmanEOT, 0 } };
            huffmanTableK = new int[][] { new int[] { 1, 1, 0, 0x000 }, new int[] { 2, 2, 1, 0x002 }, new int[] { 4, 4, 0, 0x00c }, new int[] { 5, 4, 1, 0x00d }, new int[] { 7, 5, 1, 0x01c }, new int[] { 9, 5, 2, 0x01d }, new int[] { 13, 6, 2, 0x03c }, new int[] { 17, 7, 2, 0x07a }, new int[] { 21, 7, 3, 0x07b }, new int[] { 29, 7, 4, 0x07c }, new int[] { 45, 7, 5, 0x07d }, new int[] { 77, 7, 6, 0x07e }, new int[] { 141, 7, 32, 0x07f }, new int[] { 0, 0, jbig2HuffmanEOT, 0 } };
            huffmanTableL = new int[][] { new int[] { 1, 1, 0, 0x000 }, new int[] { 2, 2, 0, 0x002 }, new int[] { 3, 3, 1, 0x006 }, new int[] { 5, 5, 0, 0x01c }, new int[] { 6, 5, 1, 0x01d }, new int[] { 8, 6, 1, 0x03c }, new int[] { 10, 7, 0, 0x07a }, new int[] { 11, 7, 1, 0x07b }, new int[] { 13, 7, 2, 0x07c }, new int[] { 17, 7, 3, 0x07d }, new int[] { 25, 7, 4, 0x07e }, new int[] { 41, 8, 5, 0x0fe }, new int[] { 73, 8, 32, 0x0ff }, new int[] { 0, 0, jbig2HuffmanEOT, 0 } };
            huffmanTableM = new int[][] { new int[] { 1, 1, 0, 0x000 }, new int[] { 2, 3, 0, 0x004 }, new int[] { 7, 3, 3, 0x005 }, new int[] { 3, 4, 0, 0x00c }, new int[] { 5, 4, 1, 0x00d }, new int[] { 4, 5, 0, 0x01c }, new int[] { 15, 6, 1, 0x03a }, new int[] { 17, 6, 2, 0x03b }, new int[] { 21, 6, 3, 0x03c }, new int[] { 29, 6, 4, 0x03d }, new int[] { 45, 6, 5, 0x03e }, new int[] { 77, 7, 6, 0x07e }, new int[] { 141, 7, 32, 0x07f }, new int[] { 0, 0, jbig2HuffmanEOT, 0 } };
            huffmanTableN = new int[][] { new int[] { 0, 1, 0, 0x000 }, new int[] { -2, 3, 0, 0x004 }, new int[] { -1, 3, 0, 0x005 }, new int[] { 1, 3, 0, 0x006 }, new int[] { 2, 3, 0, 0x007 }, new int[] { 0, 0, jbig2HuffmanEOT, 0 } };
            huffmanTableO = new int[][] { new int[] { 0, 1, 0, 0x000 }, new int[] { -1, 3, 0, 0x004 }, new int[] { 1, 3, 0, 0x005 }, new int[] { -2, 4, 0, 0x00c }, new int[] { 2, 4, 0, 0x00d }, new int[] { -4, 5, 1, 0x01c }, new int[] { 3, 5, 1, 0x01d }, new int[] { -8, 6, 2, 0x03c }, new int[] { 5, 6, 2, 0x03d }, new int[] { -24, 7, 4, 0x07c }, new int[] { 9, 7, 4, 0x07d }, new int[] { -25, 7, jbig2HuffmanLOW, 0x07e }, new int[] { 25, 7, 32, 0x07f }, new int[] { 0, 0, jbig2HuffmanEOT, 0 } };
        }

        internal DecodeIntResult DecodeInt(int[][] table)
        {
            int length = 0, prefix = 0;

            for (int i = 0; table[i][2] != jbig2HuffmanEOT; i++)
            {
                for (; length < table[i][1]; length++)
                {
                    int bit = reader.ReadBit();
                    prefix = (prefix << 1) | bit;
                }

                if (prefix == table[i][3])
                {
                    if (table[i][2] == jbig2HuffmanOOB)
                    {
                        return new DecodeIntResult(-1, false);
                    }
                    int decodedInt;
                    if (table[i][2] == jbig2HuffmanLOW)
                    {
                        int readBits = reader.ReadBits(32);
                        decodedInt = table[i][0] - readBits;
                    }
                    else if (table[i][2] > 0)
                    {
                        int readBits = reader.ReadBits(table[i][2]);
                        decodedInt = table[i][0] + readBits;
                    }
                    else
                    {
                        decodedInt = table[i][0];
                    }
                    return new DecodeIntResult(decodedInt, true);
                }
            }
            return new DecodeIntResult(-1, false);
        }

        internal int[][] BuildTable(int[][] table, int length)
        {
            int i, j, k, prefix;
            int[] tab;

            for (i = 0; i < length; i++)
            {
                for (j = i; j < length && table[j][1] == 0; j++)
                {
                    ;
                }

                if (j == length)
                {
                    break;
                }
                for (k = j + 1; k < length; k++)
                {
                    if (table[k][1] > 0 && table[k][1] < table[j][1])
                    {
                        j = k;
                    }
                }
                if (j != i)
                {
                    tab = table[j];
                    for (k = j; k > i; k--)
                    {
                        table[k] = table[k - 1];
                    }
                    table[i] = tab;
                }
            }
            table[i] = table[length];

            i = 0;
            prefix = 0;
            table[i++][3] = prefix++;
            for (; table[i][2] != jbig2HuffmanEOT; i++)
            {
                prefix <<= table[i][1] - table[i - 1][1];
                table[i][3] = prefix++;
            }

            return table;
        }
    }
}

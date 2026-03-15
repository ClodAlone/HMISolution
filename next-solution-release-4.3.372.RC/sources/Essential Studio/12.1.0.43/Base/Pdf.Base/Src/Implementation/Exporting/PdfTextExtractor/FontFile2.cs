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
    internal class FontFile2
    {
        private const long serialVersionUID = -3097990864237320960L;
        private RectangularArrays m_rectangularArrays = new RectangularArrays();

        public const int HEAD = 0;
        public const int MAXP = 1;
        public const int CMAP = 2;
        public const int LOCA = 3;
        public const int GLYF = 4;
        public const int HHEA = 5;
        public const int HMTX = 6;
        public const int NAME = 7;
        public const int POST = 8;
        public const int CVT = 9;
        public const int FPGM = 10;
        public const int HDMX = 11;
        public const int KERN = 12;
        public const int OS2 = 13;
        public const int PREP = 14;
        public const int DSIG = 15;
        public const int CFF = 16;
        public const int GSUB = 17;
        public const int BASE = 18;
        public const int EBDT = 19;
        public const int EBLC = 20;
        public const int GASP = 21;
        public const int VHEA = 22;
        public const int VMTX = 23;
        public const int GDEF = 24;
        public const int JSTF = 25;
        public const int LTSH = 26;
        public const int PCLT = 27;
        public const int VDMX = 28;
        public const int BSLN = 29;
        public const int MORT = 30;
        public const int FDSC = 31;
        public const int FFTM = 32;
        public const int GPOS = 33;
        public const int FEAT = 34;
        public const int JUST = 35;
        public const int PROP = 36;

        protected internal int tableCount = 37;

        protected internal int[][] checksums;
        protected internal int[][] tables;
        protected internal int[][] tableLength;

        private byte[] fontDataAsArray = null;

        private bool useArray = true;

        protected internal List<string> tableList = new List<string>();
        internal List<TableEntry> tableEntries = new List<TableEntry>();
        internal int pointer = 0;

        public const int OPENTYPE = 1;
        public const int TRUETYPE = 2;
        public const int TTC = 3;

        protected internal int type = TRUETYPE;

        public int currentFontID = 0;
        internal int fontCount = 1;

        protected internal int numTables = 11, searchRange = 128, entrySelector = 3, rangeShift = 48;

        public FontFile2(byte[] data)
        {
            useArray = true;
            this.fontDataAsArray = data;
            readHeader();
        }
        
        /// <summary>
        /// Reads the header of the font
        /// </summary>
        private void readHeader()
        {
            /// <summary>
            ///code to read the data at start of file </summary>
            //scalertype
            int scalerType = getnextUint32();

            if (scalerType == 1330926671)
            {
                type = OPENTYPE;
            }
            else if (scalerType == 1953784678)
            {
                type = TTC;
            }

            if (type == TTC)
            {
                getnextUint32();
                fontCount = getnextUint32();

                checksums = m_rectangularArrays.ReturnRectangularIntArray(tableCount, fontCount);
                tables = m_rectangularArrays.ReturnRectangularIntArray(tableCount, fontCount);
                tableLength = m_rectangularArrays.ReturnRectangularIntArray(tableCount, fontCount);

                int[] fontOffsets = new int[fontCount];

                for (int currentFont = 0; currentFont < fontCount; currentFont++)
                {
                    currentFontID = currentFont;
                    int fontStart = getnextUint32();
                    fontOffsets[currentFont] = fontStart;
                }

                for (int currentFont = 0; currentFont < fontCount; currentFont++)
                {
                    currentFontID = currentFont; //choose this font
                    this.pointer = fontOffsets[currentFont];
                    scalerType = getnextUint32();
                    readTablesForFont();
                }
                currentFontID = 0;
            }
            else
            {
                checksums = m_rectangularArrays.ReturnRectangularIntArray(tableCount, 1);
                tables = m_rectangularArrays.ReturnRectangularIntArray(tableCount, 1);
                tableLength = m_rectangularArrays.ReturnRectangularIntArray(tableCount, 1);

                readTablesForFont();
            }
        }

        /// <summary>
        /// Reads the table part to get the details about the tables in the font
        /// </summary>
        private void readTablesForFont()
        {
            numTables = getnextUint16();
            searchRange = getnextUint16();
            entrySelector = getnextUint16();
            rangeShift = getnextUint16();

            int id;

            for (int l = 0; l < numTables; l++)
            {
                TableEntry entry = new TableEntry();
                entry.id = getnextUint32AsTag();
                entry.checkSum = getnextUint32();
                entry.offset = getnextUint32();
                entry.length = getnextUint32();

                tableList.Add(entry.id);
                tableEntries.Add(entry);
                id = getTableID(entry.id);

                if (id != -1)
                {
                    checksums[id][currentFontID] = entry.checkSum;
                    tables[id][currentFontID] = entry.offset;
                    tableLength[id][currentFontID] = entry.length;
                }
            }
        }

        /// <summary>
        /// Returns the id of the font table corresponding to the tag
        /// </summary>
        /// <param name="tag">Font tag</param>
        /// <returns>Corresponding table ID</returns>
        protected internal int getTableID(string tag)
        {
            int id = -1;

            if (tag.Equals("maxp"))
            {
                id = MAXP;
            }
            else if (tag.Equals("head"))
            {
                id = HEAD;
            }
            else if (tag.Equals("cmap"))
            {
                id = CMAP;
            }
            else if (tag.Equals("loca"))
            {
                id = LOCA;
            }
            else if (tag.Equals("glyf"))
            {
                id = GLYF;
            }
            else if (tag.Equals("hhea"))
            {
                id = HHEA;
            }
            else if (tag.Equals("hmtx"))
            {
                id = HMTX;
            }
            else if (tag.Equals("name"))
            {
                id = NAME;
            }
            else if (tag.Equals("post"))
            {
                id = POST;
            }
            else if (tag.Equals("cvt "))
            {
                id = CVT;
            }
            else if (tag.Equals("fpgm"))
            {
                id = FPGM;
            }
            else if (tag.Equals("hdmx"))
            {
                id = HDMX;
            }
            else if (tag.Equals("kern"))
            {
                id = KERN;
            }
            else if (tag.Equals("OS/2"))
            {
                id = OS2;
            }
            else if (tag.Equals("prep"))
            {
                id = PREP;
            }
            else if (tag.Equals("DSIG"))
            {
                id = DSIG;
            }
            else if (tag.Equals("BASE"))
            {
                id = BASE;
            }
            else if (tag.Equals("CFF "))
            {
                id = CFF;
            }
            else if (tag.Equals("GSUB"))
            {
                id = GSUB;
            }
            else if (tag.Equals("EBDT"))
            {
                id = EBDT;
            }
            else if (tag.Equals("EBLC"))
            {
                id = EBLC;
            }
            else if (tag.Equals("gasp"))
            {
                id = GASP;
            }
            else if (tag.Equals("vhea"))
            {
                id = VHEA;
            }
            else if (tag.Equals("vmtx"))
            {
                id = VMTX;
            }
            else if (tag.Equals("GDEF"))
            {
                id = GDEF;
            }
            else if (tag.Equals("JSTF"))
            {
                id = JSTF;
            }
            else if (tag.Equals("LTSH"))
            {
                id = LTSH;
            }
            else if (tag.Equals("PCLT"))
            {
                id = PCLT;
            }
            else if (tag.Equals("VDMX"))
            {
                id = VDMX;
            }
            else if (tag.Equals("mort"))
            {
                id = MORT;
            }
            else if (tag.Equals("bsln"))
            {
                id = BSLN;
            }
            else if (tag.Equals("fdsc"))
            {
                id = FDSC;
            }
            else if (tag.Equals("FFTM"))
            {
                id = FFTM;
            }
            else if (tag.Equals("GPOS"))
            {
                id = GPOS;
            }
            else if (tag.Equals("feat"))
            {
                id = FEAT;
            }
            else if (tag.Equals("just"))
            {
                id = JUST;
            }
            else if (tag.Equals("prop"))
            {
                id = PROP;
            }            
            return id;
        }

        /// <summary>
        /// Reads 4 bytes from the byte array
        /// </summary>
        /// <returns>Corresponding value</returns>
        public int getnextUint32()
        {
            int returnValue = 0, nextValue = 0;

            for (int i = 0; i < 4; i++)
            {
                if (pointer < fontDataAsArray.Length)
                {
                    nextValue = fontDataAsArray[pointer] & 255;
                }
                else
                {
                    nextValue = 0;
                }
                returnValue = returnValue + ((nextValue << (8 * (3 - i))));
                pointer++;
            }
            return returnValue;
        }

        /// <summary>
        /// Reads 8 bytes from the byte array
        /// </summary>
        /// <returns>Corresponding value</returns>
        public int getnextUint64()
        {
            int returnValue = 0, nextValue = 0;

            for (int i = 0; i < 8; i++)
            {
                nextValue = fontDataAsArray[pointer];

                if (nextValue < 0)
                {
                    nextValue = 256 + nextValue;
                }
                returnValue = returnValue + (nextValue << (8 * (7 - i)));
                pointer++;
            }
            return returnValue;
        }

        /// <summary>
        /// Read 4 bytes from the byte array
        /// </summary>
        /// <returns>Corresponding string value</returns>
        public string getnextUint32AsTag()
        {
            StringBuilder returnValue = new StringBuilder();

            char c;

            for (int i = 0; i < 4; i++)
            {
                c = (char)fontDataAsArray[pointer];
                returnValue.Append(c);
                pointer++;
            }
            return returnValue.ToString();
        }

        /// <summary>
        /// Read 2 bytes from the byte array
        /// </summary>
        /// <returns>Corresponding integer value</returns>
        public int getnextUint16()
        {
            int returnValue = 0, nextValue;
            for (int i = 0; i < 2; i++)
            {
                if (fontDataAsArray.Length > 0)
                {
                    nextValue = fontDataAsArray[pointer] & 255;
                    returnValue = returnValue + (nextValue << (8 * (1 - i)));
                }
                pointer++;
            }
            return returnValue;
        }

        /// <summary>
        /// Separated the byte array corresponding to the table entry from the font stream
        /// </summary>
        /// <param name="tableID">Table ID</param>
        /// <returns>Byte array of the table</returns>
        public byte[] getTableBytes(int tableID)
        {
            int startPointer = tables[tableID][currentFontID];
            int length = tableLength[tableID][currentFontID];
            byte[] block = new byte[length];
            Array.Copy(fontDataAsArray, startPointer, block, 0, length);
            return block;
        }
    }
    
    class RectangularArrays
    {
        internal  int[][] ReturnRectangularIntArray(int Size1, int Size2)
        {
            int[][] Array = new int[Size1][];
            for (int Array1 = 0; Array1 < Size1; Array1++)
            {
                Array[Array1] = new int[Size2];
            }
            return Array;
        }
        internal  string[][] ReturnRectangularStringArray(int Size1, int Size2)
        {
            string[][] Array = new string[Size1][];
            for (int Array1 = 0; Array1 < Size1; Array1++)
            {
                Array[Array1] = new string[Size2];
            }
            return Array;
        }
        internal  float[][] ReturnRectangularFloatArray(int Size1, int Size2)
        {
            float[][] Array = new float[Size1][];
            for (int Array1 = 0; Array1 < Size1; Array1++)
            {
                Array[Array1] = new float[Size2];
            }
            return Array;
        }
        internal  short[][] ReturnRectangularShortArray(int Size1, int Size2)
        {
            short[][] Array = new short[Size1][];
            for (int Array1 = 0; Array1 < Size1; Array1++)
            {
                Array[Array1] = new short[Size2];
            }
            return Array;
        }
        internal  byte[][] ReturnRectangularSbyteArray(int Size1, int Size2)
        {
            byte[][] Array = new byte[Size1][];
            for (int Array1 = 0; Array1 < Size1; Array1++)
            {
                Array[Array1] = new byte[Size2];
            }
            return Array;
        }
    }
}

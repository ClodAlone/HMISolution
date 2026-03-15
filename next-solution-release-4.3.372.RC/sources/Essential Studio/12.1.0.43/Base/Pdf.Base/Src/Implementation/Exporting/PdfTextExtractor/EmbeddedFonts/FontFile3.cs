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
using System.IO;

namespace Syncfusion.Pdf
{
    internal class FontFile3
    {
        int glyphCount = 0;
        char[] nybChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.', 'e', 'e', ' ', '-' };
        int ROS = -1, CIDFontVersion = 0, CIDFontRevision = 0, CIDFontType = 0, CIDcount = 0, UIDBase = -1, FDArray = -1, FDSelect = -1;
        //current location in file
        private int top = 0;
        private string copyright = null;
        String embeddedFontName = null;
        private int charset = 0;
        public double[] FontMatrix = { 0.001d, 0d, 0d, 0.001d, 0, 0 };
        private int enc = 0;
        private bool hasFontMatrix = false;
        private int italicAngle = 0;
        private int charstrings = 0;
       internal bool isCID = false;
        private int stringIdx;
        bool trackIndices = false;
        private int stringStart;
        private Dictionary<string, byte[]> glyphs = new Dictionary<string, byte[]>();
        private int stringOffSize;

        /// <summary>
        ///  Variable to hold the font matrix
        /// </summary>
        internal double[] m_fontMatrix = new double[6];

        /// <summary>
        ///  Variable to cff glyphs 
        /// </summary>
        internal CffGlyphs m_cffGlyphs = new CffGlyphs();

        //private Rectangle BBox = null;


        //private boolean hasFontMatrix = false;// hasFontBBox=false,;

        private int privateDict = -1, privateDictOffset = -1;

        private int defaultWidthX = 0, nominalWidthX = 0;
        //private int top = 0;
        //private int stringIdx;
        //private int charstrings = 0;
        //private int stringStart;
        public float[] FontBBox = { 0f, 0f, 1000f, 1000f };
        //private int stringOffSize;

        private static int[] ExpertSubCharset = { // 87
																					  // elements
		0,
			1,
			231,
			232,
			235,
			236,
			237,
			238,
			13,
			14,
			15,
			99,
			239,
			240,
			241,
			242,
			243,
			244,
			245,
			246,
			247,
			248,
			27,
			28,
			249,
			250,
			251,
			253,
			254,
			255,
			256,
			257,
			258,
			259,
			260,
			261,
			262,
			263,
			264,
			265,
			266,
			109,
			110,
			267,
			268,
			269,
			270,
			272,
			300,
			301,
			302,
			305,
			314,
			315,
			158,
			155,
			163,
			320,
			321,
			322,
			323,
			324,
			325,
			326,
			150,
			164,
			169,
			327,
			328,
			329,
			330,
			331,
			332,
			333,
			334,
			335,
			336,
			337,
			338,
			339,
			340,
			341,
			342,
			343,
			344,
			345,
			346 };


        /** lookup table for names for type 1C glyphs */
        private static String[] type1CStdStrings = { // 391
																					   // elements
		".notdef",
			"space",
			"exclam",
			"quotedbl",
			"numbersign",
			"dollar",
			"percent",
			"ampersand",
			"quoteright",
			"parenleft",
			"parenright",
			"asterisk",
			"plus",
			"comma",
			"hyphen",
			"period",
			"slash",
			"zero",
			"one",
			"two",
			"three",
			"four",
			"five",
			"six",
			"seven",
			"eight",
			"nine",
			"colon",
			"semicolon",
			"less",
			"equal",
			"greater",
			"question",
			"at",
			"A",
			"B",
			"C",
			"D",
			"E",
			"F",
			"G",
			"H",
			"I",
			"J",
			"K",
			"L",
			"M",
			"N",
			"O",
			"P",
			"Q",
			"R",
			"S",
			"T",
			"U",
			"V",
			"W",
			"X",
			"Y",
			"Z",
			"bracketleft",
			"backslash",
			"bracketright",
			"asciicircum",
			"underscore",
			"quoteleft",
			"a",
			"b",
			"c",
			"d",
			"e",
			"f",
			"g",
			"h",
			"i",
			"j",
			"k",
			"l",
			"m",
			"n",
			"o",
			"p",
			"q",
			"r",
			"s",
			"t",
			"u",
			"v",
			"w",
			"x",
			"y",
			"z",
			"braceleft",
			"bar",
			"braceright",
			"asciitilde",
			"exclamdown",
			"cent",
			"sterling",
			"fraction",
			"yen",
			"florin",
			"section",
			"currency",
			"quotesingle",
			"quotedblleft",
			"guillemotleft",
			"guilsinglleft",
			"guilsinglright",
			"fi",
			"fl",
			"endash",
			"dagger",
			"daggerdbl",
			"periodcentered",
			"paragraph",
			"bullet",
			"quotesinglbase",
			"quotedblbase",
			"quotedblright",
			"guillemotright",
			"ellipsis",
			"perthousand",
			"questiondown",
			"grave",
			"acute",
			"circumflex",
			"tilde",
			"macron",
			"breve",
			"dotaccent",
			"dieresis",
			"ring",
			"cedilla",
			"hungarumlaut",
			"ogonek",
			"caron",
			"emdash",
			"AE",
			"ordfeminine",
			"Lslash",
			"Oslash",
			"OE",
			"ordmasculine",
			"ae",
			"dotlessi",
			"lslash",
			"oslash",
			"oe",
			"germandbls",
			"onesuperior",
			"logicalnot",
			"mu",
			"trademark",
			"Eth",
			"onehalf",
			"plusminus",
			"Thorn",
			"onequarter",
			"divide",
			"brokenbar",
			"degree",
			"thorn",
			"threequarters",
			"twosuperior",
			"registered",
			"minus",
			"eth",
			"multiply",
			"threesuperior",
			"copyright",
			"Aacute",
			"Acircumflex",
			"Adieresis",
			"Agrave",
			"Aring",
			"Atilde",
			"Ccedilla",
			"Eacute",
			"Ecircumflex",
			"Edieresis",
			"Egrave",
			"Iacute",
			"Icircumflex",
			"Idieresis",
			"Igrave",
			"Ntilde",
			"Oacute",
			"Ocircumflex",
			"Odieresis",
			"Ograve",
			"Otilde",
			"Scaron",
			"Uacute",
			"Ucircumflex",
			"Udieresis",
			"Ugrave",
			"Yacute",
			"Ydieresis",
			"Zcaron",
			"aacute",
			"acircumflex",
			"adieresis",
			"agrave",
			"aring",
			"atilde",
			"ccedilla",
			"eacute",
			"ecircumflex",
			"edieresis",
			"egrave",
			"iacute",
			"icircumflex",
			"idieresis",
			"igrave",
			"ntilde",
			"oacute",
			"ocircumflex",
			"odieresis",
			"ograve",
			"otilde",
			"scaron",
			"uacute",
			"ucircumflex",
			"udieresis",
			"ugrave",
			"yacute",
			"ydieresis",
			"zcaron",
			"exclamsmall",
			"Hungarumlautsmall",
			"dollaroldstyle",
			"dollarsuperior",
			"ampersandsmall",
			"Acutesmall",
			"parenleftsuperior",
			"parenrightsuperior",
			"twodotenleader",
			"onedotenleader",
			"zerooldstyle",
			"oneoldstyle",
			"twooldstyle",
			"threeoldstyle",
			"fouroldstyle",
			"fiveoldstyle",
			"sixoldstyle",
			"sevenoldstyle",
			"eightoldstyle",
			"nineoldstyle",
			"commasuperior",
			"threequartersemdash",
			"periodsuperior",
			"questionsmall",
			"asuperior",
			"bsuperior",
			"centsuperior",
			"dsuperior",
			"esuperior",
			"isuperior",
			"lsuperior",
			"msuperior",
			"nsuperior",
			"osuperior",
			"rsuperior",
			"ssuperior",
			"tsuperior",
			"ff",
			"ffi",
			"ffl",
			"parenleftinferior",
			"parenrightinferior",
			"Circumflexsmall",
			"hyphensuperior",
			"Gravesmall",
			"Asmall",
			"Bsmall",
			"Csmall",
			"Dsmall",
			"Esmall",
			"Fsmall",
			"Gsmall",
			"Hsmall",
			"Ismall",
			"Jsmall",
			"Ksmall",
			"Lsmall",
			"Msmall",
			"Nsmall",
			"Osmall",
			"Psmall",
			"Qsmall",
			"Rsmall",
			"Ssmall",
			"Tsmall",
			"Usmall",
			"Vsmall",
			"Wsmall",
			"Xsmall",
			"Ysmall",
			"Zsmall",
			"colonmonetary",
			"onefitted",
			"rupiah",
			"Tildesmall",
			"exclamdownsmall",
			"centoldstyle",
			"Lslashsmall",
			"Scaronsmall",
			"Zcaronsmall",
			"Dieresissmall",
			"Brevesmall",
			"Caronsmall",
			"Dotaccentsmall",
			"Macronsmall",
			"figuredash",
			"hypheninferior",
			"Ogoneksmall",
			"Ringsmall",
			"Cedillasmall",
			"questiondownsmall",
			"oneeighth",
			"threeeighths",
			"fiveeighths",
			"seveneighths",
			"onethird",
			"twothirds",
			"zerosuperior",
			"foursuperior",
			"fivesuperior",
			"sixsuperior",
			"sevensuperior",
			"eightsuperior",
			"ninesuperior",
			"zeroinferior",
			"oneinferior",
			"twoinferior",
			"threeinferior",
			"fourinferior",
			"fiveinferior",
			"sixinferior",
			"seveninferior",
			"eightinferior",
			"nineinferior",
			"centinferior",
			"dollarinferior",
			"periodinferior",
			"commainferior",
			"Agravesmall",
			"Aacutesmall",
			"Acircumflexsmall",
			"Atildesmall",
			"Adieresissmall",
			"Aringsmall",
			"AEsmall",
			"Ccedillasmall",
			"Egravesmall",
			"Eacutesmall",
			"Ecircumflexsmall",
			"Edieresissmall",
			"Igravesmall",
			"Iacutesmall",
			"Icircumflexsmall",
			"Idieresissmall",
			"Ethsmall",
			"Ntildesmall",
			"Ogravesmall",
			"Oacutesmall",
			"Ocircumflexsmall",
			"Otildesmall",
			"Odieresissmall",
			"OEsmall",
			"Oslashsmall",
			"Ugravesmall",
			"Uacutesmall",
			"Ucircumflexsmall",
			"Udieresissmall",
			"Yacutesmall",
			"Thornsmall",
			"Ydieresissmall",
			"001.000",
			"001.001",
			"001.002",
			"001.003",
			"Black",
			"Bold",
			"Book",
			"Light",
			"Medium",
			"Regular",
			"Roman",
			"Semibold" };

        /** Lookup table to map values */
        private static int[] ISOAdobeCharset = { // 229
																					  // elements
		0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11,
			12,
			13,
			14,
			15,
			16,
			17,
			18,
			19,
			20,
			21,
			22,
			23,
			24,
			25,
			26,
			27,
			28,
			29,
			30,
			31,
			32,
			33,
			34,
			35,
			36,
			37,
			38,
			39,
			40,
			41,
			42,
			43,
			44,
			45,
			46,
			47,
			48,
			49,
			50,
			51,
			52,
			53,
			54,
			55,
			56,
			57,
			58,
			59,
			60,
			61,
			62,
			63,
			64,
			65,
			66,
			67,
			68,
			69,
			70,
			71,
			72,
			73,
			74,
			75,
			76,
			77,
			78,
			79,
			80,
			81,
			82,
			83,
			84,
			85,
			86,
			87,
			88,
			89,
			90,
			91,
			92,
			93,
			94,
			95,
			96,
			97,
			98,
			99,
			100,
			101,
			102,
			103,
			104,
			105,
			106,
			107,
			108,
			109,
			110,
			111,
			112,
			113,
			114,
			115,
			116,
			117,
			118,
			119,
			120,
			121,
			122,
			123,
			124,
			125,
			126,
			127,
			128,
			129,
			130,
			131,
			132,
			133,
			134,
			135,
			136,
			137,
			138,
			139,
			140,
			141,
			142,
			143,
			144,
			145,
			146,
			147,
			148,
			149,
			150,
			151,
			152,
			153,
			154,
			155,
			156,
			157,
			158,
			159,
			160,
			161,
			162,
			163,
			164,
			165,
			166,
			167,
			168,
			169,
			170,
			171,
			172,
			173,
			174,
			175,
			176,
			177,
			178,
			179,
			180,
			181,
			182,
			183,
			184,
			185,
			186,
			187,
			188,
			189,
			190,
			191,
			192,
			193,
			194,
			195,
			196,
			197,
			198,
			199,
			200,
			201,
			202,
			203,
			204,
			205,
			206,
			207,
			208,
			209,
			210,
			211,
			212,
			213,
			214,
			215,
			216,
			217,
			218,
			219,
			220,
			221,
			222,
			223,
			224,
			225,
			226,
			227,
			228 };
        /** lookup data to convert Expert values */
        private static int[] ExpertCharset = { // 166
																				// elements
		0,
			1,
			229,
			230,
			231,
			232,
			233,
			234,
			235,
			236,
			237,
			238,
			13,
			14,
			15,
			99,
			239,
			240,
			241,
			242,
			243,
			244,
			245,
			246,
			247,
			248,
			27,
			28,
			249,
			250,
			251,
			252,
			253,
			254,
			255,
			256,
			257,
			258,
			259,
			260,
			261,
			262,
			263,
			264,
			265,
			266,
			109,
			110,
			267,
			268,
			269,
			270,
			271,
			272,
			273,
			274,
			275,
			276,
			277,
			278,
			279,
			280,
			281,
			282,
			283,
			284,
			285,
			286,
			287,
			288,
			289,
			290,
			291,
			292,
			293,
			294,
			295,
			296,
			297,
			298,
			299,
			300,
			301,
			302,
			303,
			304,
			305,
			306,
			307,
			308,
			309,
			310,
			311,
			312,
			313,
			314,
			315,
			316,
			317,
			318,
			158,
			155,
			163,
			319,
			320,
			321,
			322,
			323,
			324,
			325,
			326,
			150,
			164,
			169,
			327,
			328,
			329,
			330,
			331,
			332,
			333,
			334,
			335,
			336,
			337,
			338,
			339,
			340,
			341,
			342,
			343,
			344,
			345,
			346,
			347,
			348,
			349,
			350,
			351,
			352,
			353,
			354,
			355,
			356,
			357,
			358,
			359,
			360,
			361,
			362,
			363,
			364,
			365,
			366,
			367,
			368,
			369,
			370,
			371,
			372,
			373,
			374,
			375,
			376,
			377,
			378 };


        internal CffGlyphs readType1CFontFile(byte[] fontDataAsArray)
        {
            StreamReader br = new StreamReader(new MemoryStream(fontDataAsArray));
            string line;

            while (true)
            {

                line = br.ReadLine();

                if (line == null)
                {
                    break;
                }

                if (line.IndexOf("/FontMatrix") != -1)
                {

                    int startP, endP;
                    string values = "";

                    startP = line.IndexOf('[');
                    if (startP != -1)
                    {
                        endP = line.IndexOf(']');
                        values = line.Substring(startP + 1, endP - (startP + 1));
                    }
                    else
                    {
                        startP = line.IndexOf('{');
                        if (startP != -1)
                        {
                            endP = line.IndexOf('}');
                            values = line.Substring(startP + 1, endP - (startP + 1));
                        }
                    }
                    string[] matrixValues = values.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < 6; i++)
                    {
                        m_fontMatrix[i] = Convert.ToDouble(matrixValues[i]);
                    }
                }
            }


            int start;
            int size = 2;

            int major, minor;

            major = fontDataAsArray[0];
            minor = fontDataAsArray[1];

            top = fontDataAsArray[2];


            int count = 0, offsize = 0;

            count = getWord(fontDataAsArray, top, size);
            offsize = fontDataAsArray[top + size];

            top += (size + 1);
            start = top + (count + 1) * offsize - 1;

            top = start + getWord(fontDataAsArray, top + count * offsize, offsize);

            count = getWord(fontDataAsArray, top, size);
            offsize = fontDataAsArray[top + size];


            top += (size + 1); //update pointer
            start = top + (count + 1) * offsize - 1;

            int dicStart = 0, dicEnd = 0;

            dicStart = start + getWord(fontDataAsArray, top, offsize);
            dicEnd = start + getWord(fontDataAsArray, top + offsize, offsize);
            string[] strings = readStringIndex(fontDataAsArray, start, offsize, count);
            readGlobalSubRoutines(fontDataAsArray);

            decodeDictionary(fontDataAsArray, dicStart, dicEnd, strings);

            if (FDSelect != -1)
            {
                int nextDic = FDArray;

                count = getWord(fontDataAsArray, nextDic, size);
                offsize = fontDataAsArray[nextDic + size];

                nextDic += (size + 1);
                start = nextDic + (count + 1) * offsize - 1;

                dicStart = start + getWord(fontDataAsArray, nextDic, offsize);
                dicEnd = start + getWord(fontDataAsArray, nextDic + offsize, offsize);

                decodeDictionary(fontDataAsArray, dicStart, dicEnd, strings);
            }
            top = charstrings;
            glyphCount = getWord(fontDataAsArray, top, size); //start of glyph index
            int[] names = readCharset(charset, glyphCount, charstrings, fontDataAsArray);
            top = charstrings;
            readGlyphs(fontDataAsArray, glyphCount, names);
            if (privateDict != -1)
            {

                decodeDictionary(fontDataAsArray, privateDict, privateDictOffset + privateDict, strings);

                top = privateDict + privateDictOffset;

                int len = 0, nSubrs = 0;


                len = fontDataAsArray.Length;

                if (top + 2 < len)
                {
                    nSubrs = getWord(fontDataAsArray, top, size);
                    if (nSubrs > 0)
                    {
                        readSubrs(fontDataAsArray, nSubrs);
                    }
                }

            }
            m_cffGlyphs.Glyphs = glyphs;
            m_cffGlyphs.FontMatrix = m_fontMatrix;

            return m_cffGlyphs;
        }

        private int getWord(byte[] fontDataAsArray, int index, int size)
        {
            int result = 0;
            for (int i = 0; i < size; i++)
            {
                result = (result << 8) + (fontDataAsArray[index + i] & 0xff);

            }
            return result;
        }

        private string[] readStringIndex(byte[] fontDataAsArray, int start, int offsize, int count)
        {

            int nStrings = 0;

            bool isByteArray = (fontDataAsArray != null);

            if (isByteArray)
            {
                top = start + getWord(fontDataAsArray, top + count * offsize, offsize);
                //start of string index
                nStrings = getWord(fontDataAsArray, top, 2);
                stringOffSize = fontDataAsArray[top + 2];
            }
            //else
            //{
            //    top = start + getWord(fontDataAsObject, top + count * offsize, offsize);
            //    //start of string index
            //    nStrings = getWord(fontDataAsObject, top, 2);
            //    stringOffSize = fontDataAsObject.getByte(top + 2);
            //}

            top += 3;
            stringIdx = top;
            stringStart = top + (nStrings + 1) * stringOffSize - 1;

            if (isByteArray)
            {
                top = stringStart + getWord(fontDataAsArray, top + nStrings * stringOffSize, stringOffSize);
            }
            //else
            //{
            //    top = stringStart + getWord(fontDataAsObject, top + nStrings * stringOffSize, stringOffSize);
            //}

            int[] offsets = new int[nStrings + 2];
            string[] strings = new string[nStrings + 2];

            int ii = stringIdx;
            //read the offsets
            for (int jj = 0; jj < nStrings + 1; jj++)
            {

                if (isByteArray)
                {
                    offsets[jj] = getWord(fontDataAsArray, ii, stringOffSize); //content[ii] & 0xff;
                }
                //else
                //{
                //    offsets[jj] = getWord(fontDataAsObject, ii, stringOffSize); //content[ii] & 0xff;
                //}
                //getWord(content,ii,stringOffSize);
                ii = ii + stringOffSize;

            }

            offsets[nStrings + 1] = top - stringStart;

            //read the strings
            int current = 0;
            StringBuilder nextString;
            for (int jj = 0; jj < nStrings + 1; jj++)
            {

                nextString = new StringBuilder(offsets[jj] - current);
                for (int c = current; c < offsets[jj]; c++)
                {
                    if (isByteArray)
                    {
                        nextString.Append((char)fontDataAsArray[stringStart + c]);
                    }
                    //else
                    //{
                    //    nextString.Append((char)fontDataAsObject.getByte(stringStart + c));
                    //}
                }

                //if (debugFont)
                //{
                //    Console.WriteLine("String " + jj + " =" + nextString);
                //}

                strings[jj] = nextString.ToString();
                current = offsets[jj];

            }
            return strings;
        }

        private void readGlobalSubRoutines(byte[] fontDataAsArray)
        {

            bool isByteArray = (fontDataAsArray != null);

            int subOffSize = 0, count = 0;

            if (isByteArray)
            {
                subOffSize = (fontDataAsArray[top + 2] & 0xff);
                count = getWord(fontDataAsArray, top, 2);
            }
            //else
            //{
            //    subOffSize = (fontDataAsObject.getByte(top + 2) & 0xff);
            //    count = getWord(fontDataAsObject, top, 2);
            //}

            top += 3;
            if (count > 0)
            {

                int idx = top;
                int start = top + (count + 1) * subOffSize - 1;
                if (isByteArray)
                {
                    top = start + getWord(fontDataAsArray, top + count * subOffSize, subOffSize);
                }
                //else
                //{
                //    top = start + getWord(fontDataAsObject, top + count * subOffSize, subOffSize);
                //}

                int[] offset = new int[count + 2];

                int ii = idx;

                //read the offsets
                for (int jj = 0; jj < count + 1; jj++)
                {

                    if (isByteArray)
                    {
                        offset[jj] = start + getWord(fontDataAsArray, ii, subOffSize);
                    }
                    //else
                    //{
                    //    offset[jj] = start + getWord(fontDataAsObject, ii, subOffSize);
                    //}

                    ii = ii + subOffSize;

                }

                offset[count + 1] = top;

                //glyphs.GlobalBias = calculateSubroutineBias(count);

                //read the subroutines and store
                int current = offset[0];
                for (int jj = 1; jj < count + 1; jj++)
                {

                    MemoryStream nextStream = new MemoryStream();
                    for (int c = current; c < offset[jj]; c++)
                    {
                        if (isByteArray)
                        {
                            nextStream.WriteByte(fontDataAsArray[c]);
                        }
                        //else
                        //{
                        //    nextStream.write(fontDataAsObject.getByte(c));
                        //}
                    }
                    nextStream.Close();

                    //store
                    glyphs.Add("global" + (jj - 1).ToString(), nextStream.ToArray());

                    //setGlobalSubroutine(new Integer(jj-1+bias),nextStream.toByteArray());
                    current = offset[jj];

                }
            }
        }

        private void decodeDictionary(byte[] fontDataAsArray, int dicStart, int dicEnd, string[] strings)
        {

            bool fdReset = false;

            //if (debugDictionary)
            //{
            //    Console.WriteLine("=============Read dictionary====================" + BaseFontName);
            //}

            bool isByteArray = fontDataAsArray != null;

            int p = dicStart, nextVal = 0, key;
            int i = 0;
            double[] op = new double[48]; //current operand in dictionary

            while (p < dicEnd)
            {

                if (isByteArray)
                {
                    nextVal = fontDataAsArray[p] & 0xFF;
                }
                //else
                //{
                //    nextVal = fontDataAsObject.getByte(p) & 0xFF;
                //}

                if (nextVal <= 27 || nextVal == 31) // operator
                {

                    key = nextVal;

                    p++;

                    //if (debugDictionary && key != 12)
                    //{
                    //    Console.WriteLine(key + " (1) " + OneByteCCFDict[key]);
                    //}

                    if (key == 0x0c) //handle escaped keys
                    {

                        if (isByteArray)
                        {
                            key = fontDataAsArray[p] & 0xFF;
                        }
                        //else
                        //{
                        //    key = fontDataAsObject.getByte(p) & 0xFF;
                        //}

                        //if (debugDictionary)
                        //{
                        //    Console.WriteLine(key + " (2) " + TwoByteCCFDict[key]);
                        //}

                        p++;

                        if (key != 36 && key != 37 && key != 7 && FDSelect != -1)
                        {
                            //if (debugDictionary)
                            //{
                            //    Console.WriteLine("Ignored as part of FDArray ");

                            //    for (int ii = 0; ii < 6; ii++)
                            //    {
                            //        Console.WriteLine(op[ii]);
                            //    }
                            //}
                        } //italic
                        else if (key == 2)
                        {

                            italicAngle = (int)op[0];
                            //if (debugDictionary)
                            //{
                            //    Console.WriteLine("Italic=" + op[0]);
                            //}

                        } //fontMatrix
                        else if (key == 7)
                        {
                            if (!hasFontMatrix)
                            {
                                Array.Copy(op, 0, FontMatrix, 0, 6);
                            }

                            //if (debugDictionary)
                            //{
                            //    for (int ii = 0; ii < 6; ii++)
                            //    {
                            //        Console.WriteLine(ii + "=" + op[ii] + " " + this);
                            //    }
                            //}

                            hasFontMatrix = true;
                        } //ROS
                        else if (key == 30)
                        {
                            ROS = (int)op[0];
                            isCID = true;
                            //if (debugDictionary)
                            //{
                            //    Console.WriteLine(op[0]);
                            //}
                        } //CIDFontVersion
                        else if (key == 31)
                        {
                            CIDFontVersion = (int)op[0];
                            //if (debugDictionary)
                            //{
                            //    Console.WriteLine(op[0]);
                            //}
                        } //CIDFontRevision
                        else if (key == 32)
                        {
                            CIDFontRevision = (int)op[0];
                            //if (debugDictionary)
                            //{
                            //    Console.WriteLine(op[0]);
                            //}
                        } //CIDFontType
                        else if (key == 33)
                        {
                            CIDFontType = (int)op[0];
                            //if (debugDictionary)
                            //{
                            //    Console.WriteLine(op[0]);
                            //}
                        } //CIDcount
                        else if (key == 34)
                        {
                            CIDcount = (int)op[0];
                            //if (debugDictionary)
                            //{
                            //    Console.WriteLine(op[0]);
                            //}
                        } //UIDBase
                        else if (key == 35)
                        {
                            UIDBase = (int)op[0];
                            //if (debugDictionary)
                            //{
                            //    Console.WriteLine(op[0]);
                            //}
                        } //FDArray
                        else if (key == 36)
                        {
                            FDArray = (int)op[0];
                            //if (debugDictionary)
                            //{
                            //    Console.WriteLine(op[0]);
                            //}

                        } //FDSelect
                        else if (key == 37)
                        {
                            FDSelect = (int)op[0];

                            fdReset = true;
                            //if (debugDictionary)
                            //{
                            //    Console.WriteLine(op[0]);
                            //}
                        } //copyright
                        else if (key == 0)
                        {

                            int id = (int)op[0];
                            if (id > 390)
                            {
                                id = id - 390;
                            }
                            copyright = strings[id];
                            //if (debugDictionary)
                            //{
                            //    Console.WriteLine("copyright= " + copyright);
                            //}
                        } //Postscript
                        else if (key == 21)
                        {


                            //postscriptFontName=strings[id];
                            //if (debugDictionary)
                            //{
                            //    int id = (int)op[0];
                            //    if (id > 390)
                            //    {
                            //        id = id - 390;
                            //    }

                            //    Console.WriteLine("Postscript= " + strings[id]);
                            //    Console.WriteLine(TwoByteCCFDict[key] + ' ' + op[0]);
                            //}
                        } //BaseFontname
                        else if (key == 22)
                        {

                            //baseFontName=strings[id];
                            //if (debugDictionary)
                            //{

                            //    int id = (int)op[0];
                            //    if (id > 390)
                            //    {
                            //        id = id - 390;
                            //    }

                            //    Console.WriteLine("BaseFontname= " + embeddedFontName);
                            //    Console.WriteLine(TwoByteCCFDict[key] + ' ' + op[0]);
                            //}
                        } //fullname
                        else if (key == 38)
                        {

                            //fullname=strings[id];
                            //if (debugDictionary)
                            //{

                            //    int id = (int)op[0];
                            //    if (id > 390)
                            //    {
                            //        id = id - 390;
                            //    }

                            //    Console.WriteLine("fullname= " + strings[id]);
                            //    Console.WriteLine(TwoByteCCFDict[key] + ' ' + op[0]);
                            //}

                        }
                        //else if (debugDictionary)
                        //{
                        //    Console.WriteLine(op[0]);
                        //}

                    }
                    else
                    {

                        if (key == 2) //fullname
                        {

                            int id = (int)op[0];
                            if (id > 390)
                            {
                                id = id - 390;
                            }
                            embeddedFontName = strings[id];
                            //if (debugDictionary)
                            //{
                            //    Console.WriteLine("name= " + embeddedFontName);
                            //    Console.WriteLine(OneByteCCFDict[key] + ' ' + op[0]);
                            //}

                        } //familyname
                        else if (key == 3)
                        {

                            //embeddedFamilyName=strings[id];
                            //if (debugDictionary)
                            //{

                            //    int id = (int)op[0];
                            //    if (id > 390)
                            //    {
                            //        id = id - 390;
                            //    }

                            //    Console.WriteLine("FamilyName= " + embeddedFamilyName);
                            //    Console.WriteLine(OneByteCCFDict[key] + ' ' + op[0]);
                            //}

                        } //fontBBox
                        else if (key == 5)
                        {
                            //if (debugDictionary)
                            //{
                            //    for (int ii = 0; ii < 4; ii++)
                            //    {
                            //        Console.WriteLine(op[ii]);
                            //    }
                            //}
                            for (int ii = 0; ii < 4; ii++)
                            {
                                //System.out.println(" "+ii+" "+op[ii]);
                                this.FontBBox[ii] = (float)op[ii];
                            }

                            //hasFontBBox=true;
                        } // charset
                        else if (key == 0x0f)
                        {
                            charset = (int)op[0];

                            //if (debugDictionary)
                            //{
                            //    Console.WriteLine(op[0]);
                            //}

                        } // encoding
                        else if (key == 0x10)
                        {
                            enc = (int)op[0];
                            //if (debugDictionary)
                            //{
                            //    Console.WriteLine(op[0]);
                            //}

                        } // charstrings
                        else if (key == 0x11)
                        {
                            charstrings = (int)op[0];

                            //if (debugDictionary)
                            //{
                            //    Console.WriteLine(op[0]);
                            //}

                            //System.out.println("charStrings="+charstrings);
                        } // readPrivate
                        //Suresh
                        else if (key == 18)// && glyphs.is1C())
                        {
                            privateDict = (int)op[1];
                            privateDictOffset = (int)op[0];

                            //if (debugDictionary)
                            //{
                            //    Console.WriteLine("privateDict=" + op[0] + " Offset=" + op[1]);
                            //}

                        } //defaultWidthX
                        else if (key == 20)
                        {
                            defaultWidthX = (int)op[0];
                            //Suresh
                            //if (glyphs is T1Glyphs)
                            //{
                            //    ((T1Glyphs)glyphs).setWidthValues(defaultWidthX, nominalWidthX);
                            //}

                            //if (debugDictionary)
                            //{
                            //    Console.WriteLine("defaultWidthX=" + op[0]);
                            //}

                        } //nominalWidthX
                        else if (key == 21)
                        {
                            nominalWidthX = (int)op[0];
                            //Suresh
                            //if (glyphs is T1Glyphs)
                            //{
                            //    ((T1Glyphs)glyphs).setWidthValues(defaultWidthX, nominalWidthX);
                            //}

                            //if (debugDictionary)
                            //{
                            //    Console.WriteLine("nominalWidthX=" + op[0]);
                            //}

                        }
                        //else if (debugDictionary)
                        //{

                        //    // System.out.println(p+" "+key+" "+T1CcharCodes1Byte[key]+" <<<"+op);

                        //    Console.WriteLine("Other value " + key);
                        //    /// <summary>
                        //    ///if(op <type1CStdStrings.length)
                        //    /// System.out.println(type1CStdStrings[(int)op]);
                        //    /// else if((op-390) <strings.length)
                        //    /// System.out.println("interesting key:"+key);
                        //    /// </summary>
                        //}
                        //System.out.println(p+" "+key+" "+raw1ByteValues[key]+" <<<"+op);
                    }

                    i = 0;

                }
                else
                {

                    if (isByteArray)
                    {
                        //p = 0;
                        p = getNumber(fontDataAsArray, p, op, i, false);
                    }
                    //else
                    //{
                    //    p = glyphs.getNumber(fontDataAsObject, p, op, i, false);
                    //}

                    i++;
                }
            }

            //if (debugDictionary)
            //{
            //    Console.WriteLine("=================================" + BaseFontName);
            //}

            //reset
            if (!fdReset)
            {
                FDSelect = -1;
            }

        }

        public int getNumber(byte[] fontDataAsArray, int pos, double[] values, int valuePointer, bool debug)
        {

            int b0, i;
            double x = 0;

            b0 = fontDataAsArray[pos] & 0xFF;

            if ((b0 < 28) | (b0 == 31)) //error!
            {
                Console.Error.WriteLine("!!!!Incorrect type1C operand");
            } //2 byte number in range -32768
            else if (b0 == 28)
            {
                // +32767
                x = (fontDataAsArray[pos + 1] << 8) + (fontDataAsArray[pos + 2] & 0xff);
                pos += 3;
            }
            else if (b0 == 255)
            {

                //if (is1C)
                if (true)
                {
                    int top = ((fontDataAsArray[pos + 1] & 0xFF) << 8) + (fontDataAsArray[pos + 2] & 0xFF);
                    if (top > 32768)
                    {
                        top = 65536 - top;
                    }
                    double numb = top;
                    double dec = ((fontDataAsArray[pos + 3] & 0xFF) << 8) + (fontDataAsArray[pos + 4] & 0xFF);
                    x = numb + (dec / 65536);
                    if (fontDataAsArray[pos + 1] < 0)
                    {
                        if (debug)
                        {
                            Console.WriteLine("Negative " + x);
                        }
                        x = -x;

                    }

                    if (debug)
                    {
                        Console.WriteLine("x=" + x);

                        for (int j = 0; j < 5; j++)
                        {
                            Console.WriteLine(j + " " + fontDataAsArray[pos + j] + ' ' + (fontDataAsArray[pos + j] & 0xff) + ' ' + (fontDataAsArray[pos + j] & 0x7f));
                        }
                    }
                }
                else
                {
                    //x=((content[pos + 1]& 127) << 24) + (content[pos + 2]<<16)+(content[pos + 3] << 8) + content[pos + 4];
                    x = ((fontDataAsArray[pos + 1] & 0xFF) << 24) + ((fontDataAsArray[pos + 2] & 0xFF) << 16) + ((fontDataAsArray[pos + 3] & 0xFF) << 8) + (fontDataAsArray[pos + 4] & 0xFF);

                }

                pos += 5;
            } //4 byte signed number
            else if (b0 == 29)
            {
                x = ((fontDataAsArray[pos + 1] & 0xFF) << 24) + ((fontDataAsArray[pos + 2] & 0xFF) << 16) + ((fontDataAsArray[pos + 3] & 0xFF) << 8) + (fontDataAsArray[pos + 4] & 0xFF);
                pos += 5;
            } //BCD values
            else if (b0 == 30)
            {

                char[] buf = new char[65];
                pos += 1;
                i = 0;
                while (i < 64)
                {
                    int b = fontDataAsArray[pos++] & 0xFF;

                    int nyb0 = (b >> 4) & 0x0f;
                    int nyb1 = b & 0x0f;

                    if (nyb0 == 0xf)
                    {
                        break;
                    }
                    buf[i++] = nybChars[nyb0];
                    if (i == 64)
                    {
                        break;
                    }
                    if (nyb0 == 0xc)
                    {
                        buf[i++] = '-';
                    }
                    if (i == 64)
                    {
                        break;
                    }
                    if (nyb1 == 0xf)
                    {
                        break;
                    }
                    buf[i++] = nybChars[nyb1];
                    if (i == 64)
                    {
                        break;
                    }
                    if (nyb1 == 0xc)
                    {
                        buf[i++] = '-';
                    }
                }
                x = (double)(Convert.ToDouble(new string(buf, 0, i)));

            } //-107 +107
            else if (b0 < 247)
            {
                x = b0 - 139;
                pos++;
            } //2 bytes +108 +1131
            else if (b0 < 251)
            {
                x = ((b0 - 247) << 8) + (fontDataAsArray[pos + 1] & 0xff) + 108;
                pos += 2;
            } //-1131 -108
            else
            {
                x = -((b0 - 251) << 8) - (fontDataAsArray[pos + 1] & 0xff) - 108;
                pos += 2;
            }

            //assign number
            values[valuePointer] = x;

            //if(debug)
            //	System.out.println("Number ="+x);
            return pos;
        }


        private int[] readCharset(int charset, int nGlyphs, int top, byte[] fontDataAsArray)
        {

            bool isByteArray = fontDataAsArray != null;

            int[] glyphNames;
            int i, j;

            //if (debugFont)
            //{
            //    Console.WriteLine("charset=" + charset);
            //}

            /// <summary>
            /// //handle CIDS first
            /// if(isCID){
            /// glyphNames = new int[nGlyphs];
            /// glyphNames[0] = 0;
            /// 
            /// for (i = 1; i < nGlyphs; ++i) {
            ///		glyphNames[i] = i;//getWord(fontData, top, 2);
            ///		//top += 2;
            ///		}
            /// 
            /// 
            /// // read appropriate non-CID charset
            /// }else 		/// </summary>
            if (charset == 0)
            {
                glyphNames = ISOAdobeCharset;
            }
            else if (charset == 1)
            {
                glyphNames = ExpertCharset;
            }
            else if (charset == 2)
            {
                glyphNames = ExpertSubCharset;
            }
            else
            {
                glyphNames = new int[nGlyphs + 1];
                glyphNames[0] = 0;
                top = charset;

                int charsetFormat = 0;

                if (isByteArray)
                {
                    charsetFormat = fontDataAsArray[top++] & 0xff;
                }
                //else
                //{
                //    charsetFormat = fontDataAsObject.getByte(top++) & 0xff;
                //}

                //if (debugFont)
                //{
                //    Console.WriteLine("charsetFormat=" + charsetFormat);
                //}

                if (charsetFormat == 0)
                {
                    for (i = 1; i < nGlyphs; ++i)
                    {
                        if (isByteArray)
                        {
                            glyphNames[i] = getWord(fontDataAsArray, top, 2);
                        }
                        //else
                        //{
                        //    glyphNames[i] = getWord(fontDataAsObject, top, 2);
                        //}

                        top += 2;
                    }

                }
                else if (charsetFormat == 1)
                {

                    i = 1;

                    int c = 0, nLeft = 0;
                    while (i < nGlyphs)
                    {

                        if (isByteArray)
                        {
                            c = getWord(fontDataAsArray, top, 2);
                        }
                        //else
                        //{
                        //    c = getWord(fontDataAsObject, top, 2);
                        //}
                        top += 2;
                        if (isByteArray)
                        {
                            nLeft = fontDataAsArray[top++] & 0xff;
                        }
                        //else
                        //{
                        //    nLeft = fontDataAsObject.getByte(top++) & 0xff;
                        //}

                        for (j = 0; j <= nLeft; ++j)
                        {
                            glyphNames[i++] = c++;
                        }

                    }
                }
                else if (charsetFormat == 2)
                {
                    i = 1;

                    int c = 0, nLeft = 0;

                    while (i < nGlyphs)
                    {
                        if (isByteArray)
                        {
                            c = getWord(fontDataAsArray, top, 2);
                        }
                        //else
                        //{
                        //    c = getWord(fontDataAsObject, top, 2);
                        //}

                        top += 2;

                        if (isByteArray)
                        {
                            nLeft = getWord(fontDataAsArray, top, 2);
                        }
                        //else
                        //{
                        //    nLeft = getWord(fontDataAsObject, top, 2);
                        //}

                        top += 2;
                        for (j = 0; j <= nLeft; ++j)
                        {
                            glyphNames[i++] = c++;
                        }
                    }
                }
            }

            return glyphNames;
        }

        private int calculateSubroutineBias(int subroutineCount)
        {
            int bias;
            if (subroutineCount < 1240)
            {
                bias = 107;
            }
            else if (subroutineCount < 33900)
            {
                bias = 1131;
            }
            else
            {
                bias = 32768;
            }
            return bias;
        }

        //internal virtual void setEncoding(PdfObject pdfObject, PdfObject pdfFontDescriptor)
        //{


        //    //handle to unicode mapping
        //    PdfObject ToUnicode = pdfObject.getDictionary(PdfDictionary.ToUnicode);


        //    if (ToUnicode != null)
        //    {
        //        readUnicode(currentPdfFile.readStream(ToUnicode, true, true, false, false, false, ToUnicode.getCacheName(currentPdfFile.ObjectReader)));
        //    }

        //    //handle encoding
        //    PdfObject Encoding = pdfObject.getDictionary(PdfDictionary.Encoding);

        //    if (Encoding != null)
        //    {
        //        handleFontEncoding(pdfObject, Encoding);
        //    }
        //    else
        //    {
        //        handleNoEncoding(0, pdfObject);
        //    }

        //    if (pdfFontDescriptor != null)
        //    {

        //        int fontFlag = 0;
        //        if (pdfFontDescriptor != null)
        //        {
        //            fontFlag = pdfFontDescriptor.getInt(PdfDictionary.Flags);
        //        }

        //        //reset to defaults
        //        glyphs.remapFont = false;

        //        int flag = fontFlag;
        //        if ((flag & 4) == 4)
        //        {
        //            glyphs.remapFont = true;
        //        }

        //        //set missingWidth
        //        missingWidth = pdfFontDescriptor.getInt(PdfDictionary.MissingWidth);

        //    }
        //}

        internal virtual void readGlyphs(byte[] fontDataAsArray, int nGlyphs, int[] names)
        {

            bool isByteArray = fontDataAsArray != null;

            int glyphOffSize = 0;

            if (isByteArray)
            {
                glyphOffSize = fontDataAsArray[top + 2];
            }
            //else
            //{
            //    glyphOffSize = fontDataAsObject.getByte(top + 2);
            //}

            top += 3;
            int glyphIdx = top;
            int glyphStart = top + (nGlyphs + 1) * glyphOffSize - 1;

            if (isByteArray)
            {
                top = glyphStart + getWord(fontDataAsArray, top + nGlyphs * glyphOffSize, glyphOffSize);
            }
            //else
            //{
            //    top = glyphStart + getWord(fontDataAsObject, top + nGlyphs * glyphOffSize, glyphOffSize);
            //}

            int[] glyphoffset = new int[nGlyphs + 2];

            int ii = glyphIdx;

            //read the offsets
            for (int jj = 0; jj < nGlyphs + 1; jj++)
            {

                if (isByteArray)
                {
                    glyphoffset[jj] = glyphStart + getWord(fontDataAsArray, ii, glyphOffSize);
                }
                //else
                //{
                //    glyphoffset[jj] = glyphStart + getWord(fontDataAsObject, ii, glyphOffSize);
                //}

                ii = ii + glyphOffSize;

            }

            glyphoffset[nGlyphs + 1] = top;

            //read the glyphs and store
            int current = glyphoffset[0];
            string glyphName = "";
            byte[] nextGlyph;
            for (int jj = 1; jj < nGlyphs + 1; jj++)
            {

                nextGlyph = new byte[glyphoffset[jj] - current]; //read name of glyph

                //get data for the glyph
                for (int c = current; c < glyphoffset[jj]; c++)
                {

                    if (isByteArray)
                    {
                        nextGlyph[c - current] = fontDataAsArray[c];
                    }
                    //else
                    //{
                    //    nextGlyph[c - current] = fontDataAsObject.getByte(c);
                    //}
                }

                if (isCID)
                {
                    glyphName = Convert.ToString(names[jj - 1]);
                }
                else
                {

                    if (isByteArray)
                    {
                        glyphName = getString(fontDataAsArray, names[jj - 1], stringIdx, stringStart, stringOffSize);
                    }
                    //else
                    //{
                    //    glyphName = getString(fontDataAsObject, names[jj - 1], stringIdx, stringStart, stringOffSize);
                    //}
                }
                //if (debugFont)
                //{
                //    Console.WriteLine("glyph= " + glyphName + " start=" + current + " length=" + glyphoffset[jj] + " isCID=" + isCID);
                //}

                glyphs.Add(glyphName, nextGlyph);

                current = glyphoffset[jj];

                if (trackIndices)
                {
                    //glyphs.setIndexForCharString(jj, glyphName);

                }
            }
        }

        private string getString(byte[] fontDataAsArray, int sid, int idx, int start, int offsize)
        {

            int len;
            string result;

            if (sid < 391)
            {
                result = type1CStdStrings[sid];
            }
            else
            {
                sid -= 391;
                int idx0 = start + getWord(fontDataAsArray, idx + sid * offsize, offsize);
                int idxPtr1 = start + getWord(fontDataAsArray, idx + (sid + 1) * offsize, offsize);
                //System.out.println(sid+" "+idx0+" "+idxPtr1);
                if ((len = idxPtr1 - idx0) > 255)
                {
                    len = 255;
                }
                result = System.Text.Encoding.UTF8.GetString(fontDataAsArray, idx0, len);

                //result = StringHelperClass.NewString(fontDataAsArray, idx0, len);

            }
            return result;
        }

        internal virtual void readSubrs(byte[] fontDataAsArray, int nSubrs)
        {

            bool isByteArray = fontDataAsArray != null;

            int subrOffSize = 0;

            if (isByteArray)
            {
                subrOffSize = fontDataAsArray[top + 2];
            }
            //else
            //{
            //    subrOffSize = fontDataAsObject.getByte(top + 2);
            //}

            top += 3;
            int subrIdx = top;
            int subrStart = top + (nSubrs + 1) * subrOffSize - 1;

            int nextTablePtr = top + nSubrs * subrOffSize;

            if (isByteArray)
            {
                if (nextTablePtr < fontDataAsArray.Length) //allow for table at end of file
                {
                    top = subrStart + getWord(fontDataAsArray, nextTablePtr, subrOffSize);
                }
                else
                {
                    top = fontDataAsArray.Length - 1;
                }
            }
            //else
            //{
            //    if (nextTablePtr < fontDataAsArray.Length) //allow for table at end of file
            //    {
            //        top = subrStart + getWord(fontDataAsObject, nextTablePtr, subrOffSize);
            //    }
            //    else
            //    {
            //        top = fontDataAsObject.length() - 1;
            //    }
            //}

            int[] subrOffset = new int[nSubrs + 2];
            int ii = subrIdx;
            for (int jj = 0; jj < nSubrs + 1; jj++)
            {

                if (isByteArray)
                {
                    if ((ii + subrOffSize) < fontDataAsArray.Length)
                    {
                        subrOffset[jj] = subrStart + getWord(fontDataAsArray, ii, subrOffSize);
                    }
                }
                //else
                //{
                //    if ((ii + subrOffSize) < fontDataAsObject.length())
                //    {
                //        subrOffset[jj] = subrStart + getWord(fontDataAsObject, ii, subrOffSize);
                //    }
                //}

                ii += subrOffSize;
            }
            subrOffset[nSubrs + 1] = top;

            localBias = calculateSubroutineBias(nSubrs);
            //glyphs.LocalBias = calculateSubroutineBias(nSubrs);

            //read the glyphs and store
            int current = subrOffset[0];

            for (int jj = 1; jj < nSubrs + 1; jj++)
            {


                //skip if out of bounds
                if (current == 0 || subrOffset[jj] > fontDataAsArray.Length || subrOffset[jj] < 0 || subrOffset[jj] == 0)
                {
                    continue;
                }

                //MemoryStream nextSubr = new MemoryStream();

                //for (int c = current; c < subrOffset[jj]; c++)
                //{
                //    if (!isByteArray && c < fontDataAsObject.length())
                //    {
                //        nextSubr.WriteByte(fontDataAsObject.getByte(c));
                //    }

                //}

                if (isByteArray)
                {

                    int length = subrOffset[jj] - current;

                    if (length > 0)
                    {
                        byte[] nextSub = new byte[length];

                        Array.Copy(fontDataAsArray, current, nextSub, 0, length);
                        glyphs.Add("subrs" + (jj - 1).ToString(), nextSub);
                        //glyphs.setCharString("subrs" + (jj - 1), nextSub);
                    }
                }
                //else
                //{
                //    nextSubr.Close();

                //    glyphs.setCharString("subrs" + (jj - 1), nextSubr.ToArray());
                //}
                current = subrOffset[jj];

            }
        }

        public int localBias;
    }
}

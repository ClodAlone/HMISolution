#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !SILVERLIGHT
using System;
# if WPF || BARCODE_SILVERLIGHT || BARCODE_WINRT
namespace Syncfusion.UI.Xaml.Controls.Barcode
#elif WINDOWS_PHONE 
namespace Syncfusion.WP.Controls.Barcode
#elif ASPNET
namespace Syncfusion.Web.UI.WebControls.Barcode
#elif WINFORMS
namespace Syncfusion.Windows.Forms.Barcode
#elif MVC
namespace Syncfusion.Mvc.Barcode
#else
namespace Syncfusion.Pdf.Barcode
#endif
{
    /// <summary>
    /// Struct to hold the value of each dots in Barcode.
    /// </summary>
    struct ModuleValue
    {
        /// <summary>
        /// Specifies if the Dot is black.
        /// </summary>
        public bool IsBlack;

        /// <summary>
        /// Specifies if the Dot is already filled.
        /// </summary>
        public bool IsFilled;

        /// <summary>
        /// Specifies if the Dot is PDP.
        /// </summary>
        public bool IsPDP;
    }

    class PdfQRBarcodeValues
    {
        #region Private Members

        /// <summary>
        /// Holds the Version Information.
        /// </summary>
#if !XAML && !GDI
        private QRCodeVersion m_version;
#else
        private QRBarcodeVersion m_version;
#endif

        /// <summary>
        /// Holds the Error Correction Level.
        /// </summary>
#if !XAML && !GDI
        private PdfErrorCorrectionLevel m_errorCorrectionLevel;
#else
        private ErrorCorrectionLevel m_errorCorrectionLevel;
#endif

        /// <summary>
        /// Holds the Number of Data code word.
        /// </summary>
        private int m_numberOfDataCodeWord;

        /// <summary>
        /// Holds the Number of Error correcting code words.
        /// </summary>
        private int m_numberOfErrorCorrectingCodeWords;

        /// <summary>
        /// Holds the Number of Error correction Blocks.
        /// </summary>
        private int[] m_numberOfErrorCorrectionBlocks;

        /// <summary>
        /// Holds the End value of the version.
        /// </summary>
        private int m_end;

        /// <summary>
        /// Holds the Data copacity of the version.
        /// </summary>
        private int m_dataCapacity;

        /// <summary>
        /// Holds the Format Information.
        /// </summary>
        private byte[] m_formatInformation;

        /// <summary>
        /// Holds the Version Information.
        /// </summary>
        private byte[] m_versionInformation;

        #region numberOfErrorCorrectingCodeWords
        /// <summary>
        /// Holds all the values of Error correcting code words.
        /// </summary>
        static int[] numberOfErrorCorrectingCodeWords = new int[] { 
            7,10,13,17,
            10,16,22,28,
            15,26,36,44,
            20,36,52,64,
            26,48,72,88,
            36,64,96,112,
            40,72,108,130,
            48,88,132,156,
            60,110,160,192,
            72,130,192,224,
            80,150,224,264,
            96,176,260,308,
            104,198,288,352,
            120,216,320,384,
            132,240,360,432,
            144,280,408,480,
            168,308,448,532,
            180,338,504,588,
            196,364,546,650,
            224,416,600,700,
            224,442,644,750,
            252,476,690,816,
            270,504,750,900,
            300,560,810,960,
            312,588,870,1050,
            336,644,952,1110,
            360,700,1020,1200,
            390,728,1050,1260,
            420,784,1140,1350,
            450,812,1200,1440,
            480,868,1290,1530,
            510,924,1350,1620,
            540,980,1440,1710,
            570,1036,1530,1800,
            570,1064,1590,1890,
            600,1120,1680,1980,
            630,1204,1770,2100,
            660,1260,1860,2220,
            720,1316,1950,2310,
            750,1372,2040,2430
        };
        #endregion

        #region Character Set

        //Hexadecimal values of CP437 characters
        string[] CP437CharSet = { "2591", "2592", "2593", "2502", "2524", "2561", "2562", "2556", "2555", "2563", "2551", "2557", "255D", "255C", "255B", "2510", "2514", "2534", "252C", "251C", "2500", "253C", "255E", "255F", "255A", "2554", "2569", "2566", "2560", "2550", "256C", "2567", "2568", "2564", "2565", "2559", "2558", "2552", "2553", "256B", "256A", "2518", "250C", "2588", "2584", "258C", "2590", "2580", "25A0" };

        //Hexadecimal values of ISO8859_2 characters
        private string[] ISO8859_2CharSet = { "104", "2D8", "141", "13D", "15A", "160", "15E", "164", "179", "17D", "17B", "105", "2DB", "142", "13E", "15B", "2C7", "161", "15F", "165", "17A", "2DD", "17E", "17C", "154", "102", "139", "106", "10C", "118", "11A", "10E", "110", "143", "147", "150", "158", "16E", "170", "162", "155", "103", "13A", "107", "10D", "119", "11B", "10F", "111", "144", "148", "151", "159", "16F", "171", "163", "2D9" };

        //Hexadecimal values of ISO8859_3 characters
        private string[] ISO8859_3CharSet = { "126", "124", "130", "15E", "11E", "134", "17B", "127", "125", "131", "15F", "11F", "135", "17C", "10A", "108", "120", "11C", "16C", "15C", "10B", "109", "121", "11D", "16D", "15D" };

        //Hexadecimal values of ISO8859_4 characters
        private string[] ISO8859_4CharSet = { "104", "138", "156", "128", "13B", "160", "112", "122", "166", "17D", "105", "2DB", "157", "129", "13C", "2C7", "161", "113", "123", "167", "14A", "17E", "14B", "100", "12E", "10C", "118", "116", "12A", "110", "145", "14C", "136", "172", "168", "16A", "101", "12F", "10D", "119", "117", "12B", "111", "146", "14D", "137", "173", "169", "16B" };

        //Hexadecimal values of Windows1250 characters
        private string[] Windows1250CharSet = { "141", "104", "15E", "17B", "142", "105", "15F", "13D", "13E", "17C" };

        //Hexadecimal values of Windows1251 characters
        private string[] Windows1251CharSet = { "402", "403", "453", "409", "40A", "40C", "40B", "40F", "452", "459", "45A", "45C", "45B", "45F", "40E", "45E", "408", "490", "401", "404", "407", "406", "456", "491", "451", "454", "458", "405", "455", "457" };

        //Hexadecimal values of Windows1252 characters
        private string[] Windows1252CharSet = { "20AC", "201A", "192", "201E", "2026", "2020", "2021", "2C6", "2030", "160", "2039", "152", "17D", "2018", "2019", "201C", "201D", "2022", "2013", "2014", "2DC", "2122", "161", "203A", "153", "17E", "178" };

        //Hexadecimal values of Windows1256 characters
        string[] Windows1256CharSet = { "67E", "679", "152", "686", "698", "688", "6AF", "6A9", "691", "153", "6BA", "6BE", "6C1", "644", "645", "646", "647", "648", "649", "64A", "6D2" };
        #endregion

        #region Replacing Number

        //Equivalent values of CP437 characters
        int[] CP437ReplaceNumber = { 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 254 };


        //Equivalent values of ISO8859_2 characters
        private int[] ISO8859_2ReplaceNumber = { 161, 162, 163, 165, 166, 169, 170, 171, 172, 174, 175, 177, 178, 179, 181, 182, 183, 185, 186, 187, 188, 189, 190, 191, 192, 195, 197, 198, 200, 202, 204, 207, 208, 209, 210, 213, 216, 217, 219, 222, 224, 227, 229, 230, 232, 234, 236, 239, 240, 241, 242, 245, 248, 249, 251, 254, 255 };

        //Equivalent values of ISO8859_3 characters
        private int[] ISO8859_3ReplaceNumber = { 161, 166, 169, 170, 171, 172, 175, 177, 182, 185, 186, 187, 188, 191, 197, 198, 213, 216, 221, 222, 229, 230, 245, 248, 253, 254 };

        //Equivalent values of ISO8859_4 characters
        private int[] ISO8859_4ReplaceNumber = { 161, 162, 163, 165, 166, 169, 170, 171, 172, 174, 177, 178, 179, 181, 182, 183, 185, 186, 187, 188, 189, 190, 191, 192, 199, 200, 202, 204, 207, 208, 209, 210, 211, 217, 221, 222, 224, 231, 232, 234, 236, 239, 240, 241, 242, 243, 249, 253, 254 };

        //Equivalent values of Windows1250 characters
        private int[] Windows1250ReplaceNumber = { 163, 165, 170, 175, 179, 185, 186, 188, 190, 191 };

        //Equivalent values of Windows1251 characters
        private int[] Windows1251ReplaceNumber = { 128, 129, 131, 138, 140, 141, 142, 143, 144, 154, 156, 157, 158, 159, 161, 162, 163, 165, 168, 170, 175, 178, 179, 180, 184, 186, 188, 189, 190, 191 };

        //Equivalent values of Windows1252 characters
        private int[] Windows1252ReplaceNumber = { 128, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 142, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 158, 159 };

        //Equivalent values of Windows1256 characters
        int[] Windows1256ReplaceNumber = { 129, 138, 140, 141, 142, 143, 144, 152, 154, 156, 159, 170, 192, 225, 227, 228, 229, 230, 236, 237, 255 };

        #endregion


        /// <summary>
        /// Holds all the end values.
        /// </summary>
        static int[] EndValues = new int[] { 208, 359, 567, 807, 1079, 1383, 1568, 1936, 2336, 2768, 3232, 3728, 4256, 4651, 5243, 5867, 6523, 7211, 7931, 8683, 9252, 10068, 10916, 11796, 12708, 13652, 14628, 15371, 16411, 17483, 18587, 19723, 20891, 22091, 23008, 24272, 25568, 26896, 28256, 29648 };

        /// <summary>
        /// Holds all the Data capacity values.
        /// </summary>
        static int[] DataCapacityValues = new int[] { 26, 44, 70, 100, 134, 172, 196, 242, 292, 346, 404, 466, 532, 581, 655, 733, 815, 901, 991, 1085, 1156, 1258, 1364, 1474, 1588, 1706, 1828, 1921, 2051, 2185, 2323, 2465, 2611, 2761, 2876, 3034, 3196, 3362, 3532, 3706 };

        #region NumericDataCapacity
        /// <summary>
        /// Holds all  the Numeric Data capacity of the Error correction level Low.
        /// </summary>
        internal static int[] NumericDataCapacityLow = new int[] { 41, 77, 127, 187, 255, 322, 370, 461, 552, 652, 772, 883, 1022, 1101, 1250, 1408, 1548, 1725, 1903, 2061, 2232, 2409, 2620, 2812, 3057, 3283, 3517, 3669, 3909, 4158, 4417, 4686, 4965, 5253, 5529, 5836, 6153, 6479, 6743, 7089, };

        /// <summary>
        /// Holds all  the Numeric Data capacity of the Error correction level Medium.
        /// </summary>
        internal static int[] NumericDataCapacityMedium = new int[] { 34, 63, 101, 149, 202, 255, 293, 365, 432, 513, 604, 691, 796, 871, 991, 1082, 1212, 1346, 1500, 1600, 1708, 1872, 2059, 2188, 2395, 2544, 2701, 2857, 3035, 3289, 3486, 3693, 3909, 4134, 4343, 4588, 4775, 5039, 5313, 5596, };

        /// <summary>
        /// Holds all  the Numeric Data capacity of the Error correction level Quartile.
        /// </summary>
        internal static int[] NumericDataCapacityQuartile = new int[] { 27, 48, 77, 111, 144, 178, 207, 259, 312, 364, 427, 489, 580, 621, 703, 775, 876, 948, 1063, 1159, 1224, 1358, 1468, 1588, 1718, 1804, 1933, 2085, 2181, 2358, 2473, 2670, 2805, 2949, 3081, 3244, 3417, 3599, 3791, 3993, };

        /// <summary>
        /// Holds all  the Numeric Data capacity of the Error correction level High.
        /// </summary>
        internal static int[] NumericDataCapacityHigh = new int[] { 17, 34, 58, 82, 106, 139, 154, 202, 235, 288, 331, 374, 427, 468, 530, 602, 674, 746, 813, 919, 969, 1056, 1108, 1228, 1286, 1425, 1501, 1581, 1677, 1782, 1897, 2022, 2157, 2301, 2361, 2524, 2625, 2735, 2927, 3057 };

        #endregion

        #region AlphanumericDataCapacity

        /// <summary>
        /// Holds all  the Alphaumeric Data capacity of the Error correction level Low.
        /// </summary>
        internal static int[] AlphanumericDataCapacityLow = new int[] { 25, 47, 77, 114, 154, 195, 224, 279, 335, 395, 468, 535, 619, 667, 758, 854, 938, 1046, 1153, 1249, 1352, 1460, 1588, 1704, 1853, 1990, 2132, 2223, 2369, 2520, 2677, 2840, 3009, 3183, 3351, 3537, 3729, 3927, 4087, 4296 };

        /// <summary>
        /// Holds all  the Alphaumeric Data capacity of the Error correction level Medium.
        /// </summary>
        internal static int[] AlphanumericDataCapacityMedium = new int[] { 20, 38, 61, 90, 122, 154, 178, 221, 262, 311, 366, 419, 483, 528, 600, 656, 734, 816, 909, 970, 1035, 1134, 1248, 1326, 1451, 1542, 1637, 1732, 1839, 1994, 2113, 2238, 2369, 2506, 2632, 2780, 2894, 3054, 3220, 3391 };

        /// <summary>
        /// Holds all  the Alphaumeric Data capacity of the Error correction level Quartile.
        /// </summary>
        internal static int[] AlphanumericDataCapacityQuartile = new int[] { 16, 29, 47, 67, 87, 108, 125, 157, 189, 221, 259, 296, 352, 376, 426, 470, 531, 574, 644, 702, 742, 823, 890, 963, 1041, 1094, 1172, 1263, 1322, 1429, 1499, 1618, 1700, 1787, 1867, 1966, 2071, 2181, 2298, 2420 };

        /// <summary>
        /// Holds all  the Alphaumeric Data capacity of the Error correction level High.
        /// </summary>
        internal static int[] AlphanumericDataCapacityHigh = new int[] { 10, 20, 35, 50, 64, 84, 93, 122, 143, 174, 200, 227, 259, 283, 321, 365, 408, 452, 493, 557, 587, 640, 672, 744, 779, 864, 910, 958, 1016, 1080, 1150, 1226, 1307, 1394, 1431, 1530, 1591, 1658, 1774, 1852 };

        #endregion

        #region BinaryDataCapacity
        /// <summary>
        /// Holds all  the Binary Data capacity of the Error correction level Low.
        /// </summary>
        internal static int[] BinaryDataCapacityLow = new int[] { 17, 32, 53, 78, 106, 134, 154, 192, 230, 271, 321, 367, 425, 458, 520, 586, 644, 718, 792, 858, 929, 1003, 1091, 1171, 1273, 1367, 1465, 1528, 1628, 1732, 1840, 1952, 2068, 2188, 2303, 2431, 2563, 2699, 2809, 2953 };

        /// <summary>
        /// Holds all  the Binary Data capacity of the Error correction level Medium.
        /// </summary>
        internal static int[] BinaryDataCapacityMedium = new int[] { 14, 26, 42, 62, 84, 106, 122, 152, 180, 213, 251, 287, 331, 362, 412, 450, 504, 560, 624, 666, 711, 779, 857, 911, 997, 1059, 1125, 1190, 1264, 1370, 1452, 1538, 1628, 1722, 1809, 1911, 1989, 2099, 2213, 2331 };

        /// <summary>
        /// Holds all  the Binary Data capacity of the Error correction level Quartile.
        /// </summary>
        internal static int[] BinaryDataCapacityQuartile = new int[] { 11, 20, 32, 46, 60, 74, 86, 108, 130, 151, 177, 203, 241, 258, 292, 322, 364, 394, 442, 482, 509, 565, 611, 661, 715, 751, 805, 868, 908, 982, 1030, 1112, 1168, 1228, 1283, 1351, 1423, 1499, 1579, 1663 };

        /// <summary>
        /// Holds all  the Binary Data capacity of the Error correction level High.
        /// </summary>
        internal static int[] BinaryDataCapacityHigh = new int[] { 7, 14, 24, 34, 44, 58, 64, 84, 98, 119, 137, 155, 177, 194, 220, 250, 280, 310, 338, 382, 403, 439, 461, 511, 535, 593, 625, 658, 698, 742, 790, 842, 898, 958, 983, 1051, 1093, 1139, 1219, 1273 };

        #endregion

        #endregion

        #region Properties
        /// <summary>
        /// Get or Private set the Number of Data code words.
        /// </summary>
        internal int NumberOfDataCodeWord
        {
            get
            {
                return m_numberOfDataCodeWord;
            }
            private set
            {
                m_numberOfDataCodeWord = value;
            }
        }

        /// <summary>
        /// Get or Private set the Number of Error correction code words.
        /// </summary>
        internal int NumberOfErrorCorrectingCodeWords
        {
            get
            {
                return m_numberOfErrorCorrectingCodeWords;
            }
            private set
            {
                m_numberOfErrorCorrectingCodeWords = value;
            }
        }

        /// <summary>
        /// Get or Private set the Number of Error correction Blocks.
        /// </summary>
        internal int[] NumberOfErrorCorrectionBlocks
        {
            get
            {
                return m_numberOfErrorCorrectionBlocks;
            }
            private set
            {
                m_numberOfErrorCorrectionBlocks = value;
            }
        }

        /// <summary>
        /// Get or Private set the End value of the Current Version.
        /// </summary>
        internal int End
        {
            get
            {
                return m_end;
            }
            private set
            {
                m_end = value;
            }
        }

        /// <summary>
        /// Get or Private set the Data capacity.
        /// </summary>
        internal int DataCapacity
        {
            get
            {
                return m_dataCapacity;
            }
            private set
            {
                m_dataCapacity = value;
            }
        }

        /// <summary>
        /// Get or Private set the Format Information.
        /// </summary>
        internal byte[] FormatInformation
        {
            get
            {
                return m_formatInformation;
            }
            private set
            {
                m_formatInformation = value;
            }
        }

        /// <summary>
        /// Get or Private set the Version Information.
        /// </summary>
        internal byte[] VersionInformation
        {
            get
            {
                return m_versionInformation;
            }
            private set
            {
                m_versionInformation = value;
            }
        }
        
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the Values.
        /// </summary>
#if !XAML && !GDI
        public PdfQRBarcodeValues(QRCodeVersion version, PdfErrorCorrectionLevel errorCorrectionLevel)
#else
        public PdfQRBarcodeValues(QRBarcodeVersion version, ErrorCorrectionLevel errorCorrectionLevel)
#endif
        {
            m_version = version;
            m_errorCorrectionLevel = errorCorrectionLevel;

            NumberOfDataCodeWord = GetNumberOfDataCodeWord();
            NumberOfErrorCorrectingCodeWords = GetNumberOfErrorCorrectingCodeWords();
            NumberOfErrorCorrectionBlocks = GetNumberOfErrorCorrectionBlocks();
            End = GetEnd();
            DataCapacity = GetDataCapacity();
            FormatInformation = GetFormatInformation();
            VersionInformation = GetVersionInformation();
           
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Gets the Alphanumeric values.
        /// </summary>
        internal int GetAlphanumericvalues(char Value)
        {
            int valueInInt = 0;
            switch (Value)
            {
                case '0':
                    valueInInt = 0;
                    break;
                case '1':
                    valueInInt = 1;
                    break;
                case '2':
                    valueInInt = 2;
                    break;
                case '3':
                    valueInInt = 3;
                    break;
                case '4':
                    valueInInt = 4;
                    break;
                case '5':
                    valueInInt = 5;
                    break;
                case '6':
                    valueInInt = 6;
                    break;
                case '7':
                    valueInInt = 7;
                    break;
                case '8':
                    valueInInt = 8;
                    break;
                case '9':
                    valueInInt = 9;
                    break;
                case 'A':
                    valueInInt = 10;
                    break;
                case 'B':
                    valueInInt = 11;
                    break;
                case 'C':
                    valueInInt = 12;
                    break;
                case 'D':
                    valueInInt = 13;
                    break;
                case 'E':
                    valueInInt = 14;
                    break;
                case 'F':
                    valueInInt = 15;
                    break;
                case 'G':
                    valueInInt = 16;
                    break;
                case 'H':
                    valueInInt = 17;
                    break;
                case 'I':
                    valueInInt = 18;
                    break;
                case 'J':
                    valueInInt = 19;
                    break;
                case 'K':
                    valueInInt = 20;
                    break;
                case 'L':
                    valueInInt = 21;
                    break;
                case 'M':
                    valueInInt = 22;
                    break;
                case 'N':
                    valueInInt = 23;
                    break;
                case 'O':
                    valueInInt = 24;
                    break;
                case 'P':
                    valueInInt = 25;
                    break;
                case 'Q':
                    valueInInt = 26;
                    break;
                case 'R':
                    valueInInt = 27;
                    break;
                case 'S':
                    valueInInt = 28;
                    break;
                case 'T':
                    valueInInt = 29;
                    break;
                case 'U':
                    valueInInt = 30;
                    break;
                case 'V':
                    valueInInt = 31;
                    break;
                case 'W':
                    valueInInt = 32;
                    break;
                case 'X':
                    valueInInt = 33;
                    break;
                case 'Y':
                    valueInInt = 34;
                    break;
                case 'Z':
                    valueInInt = 35;
                    break;
                case ' ':
                    valueInInt = 36;
                    break;
                case '$':
                    valueInInt = 37;
                    break;
                case '%':
                    valueInInt = 38;
                    break;
                case '*':
                    valueInInt = 39;
                    break;
                case '+':
                    valueInInt = 40;
                    break;
                case '-':
                    valueInInt = 41;
                    break;
                case '.':
                    valueInInt = 42;
                    break;
                case '/':
                    valueInInt = 43;
                    break;
                case ':':
                    valueInInt = 44;
                    break;
                default:
# if !XAML && !NETFX_CORE && !GDI && !WP
                    throw new PdfBarcodeException("Not a valid input");
#else
                    throw new BarcodeException("Not a valid input");
#endif
            }
            return valueInInt;
        }

        /// <summary>
        /// Gets number of Data code words.
        /// </summary>
        private int GetNumberOfDataCodeWord()
        {
            int countOfDataCodeWord = 0;

            switch ((int)m_version)
            {
                case 01:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 19;
                            break;
                        case 15:
                            countOfDataCodeWord = 16;
                            break;
                        case 25:
                            countOfDataCodeWord = 13;
                            break;
                        case 30:
                            countOfDataCodeWord = 9;
                            break;
                    }
                    break;
                case 02:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 34;
                            break;
                        case 15:
                            countOfDataCodeWord = 28;
                            break;
                        case 25:
                            countOfDataCodeWord = 22;
                            break;
                        case 30:
                            countOfDataCodeWord = 16;
                            break;
                    }
                    break;
                case 03:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 55;
                            break;
                        case 15:
                            countOfDataCodeWord = 44;
                            break;
                        case 25:
                            countOfDataCodeWord = 34;
                            break;
                        case 30:
                            countOfDataCodeWord = 26;
                            break;
                    }
                    break;
                case 04:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 80;
                            break;
                        case 15:
                            countOfDataCodeWord = 64;
                            break;
                        case 25:
                            countOfDataCodeWord = 48;
                            break;
                        case 30:
                            countOfDataCodeWord = 36;
                            break;
                    }
                    break;
                case 05:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 108;
                            break;
                        case 15:
                            countOfDataCodeWord = 86;
                            break;
                        case 25:
                            countOfDataCodeWord = 62;
                            break;
                        case 30:
                            countOfDataCodeWord = 46;
                            break;
                    }
                    break;
                case 06:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 136;
                            break;
                        case 15:
                            countOfDataCodeWord = 108;
                            break;
                        case 25:
                            countOfDataCodeWord = 76;
                            break;
                        case 30:
                            countOfDataCodeWord = 60;
                            break;
                    }
                    break;
                case 07:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 156;
                            break;
                        case 15:
                            countOfDataCodeWord = 124;
                            break;
                        case 25:
                            countOfDataCodeWord = 88;
                            break;
                        case 30:
                            countOfDataCodeWord = 66;
                            break;
                    }
                    break;
                case 08:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 194;
                            break;
                        case 15:
                            countOfDataCodeWord = 154;
                            break;
                        case 25:
                            countOfDataCodeWord = 110;
                            break;
                        case 30:
                            countOfDataCodeWord = 86;
                            break;
                    }
                    break;
                case 09:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 232;
                            break;
                        case 15:
                            countOfDataCodeWord = 182;
                            break;
                        case 25:
                            countOfDataCodeWord = 132;
                            break;
                        case 30:
                            countOfDataCodeWord = 100;
                            break;
                    }
                    break;
                case 10:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 274;
                            break;
                        case 15:
                            countOfDataCodeWord = 216;
                            break;
                        case 25:
                            countOfDataCodeWord = 154;
                            break;
                        case 30:
                            countOfDataCodeWord = 122;
                            break;
                    }
                    break;
                case 11:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 324;
                            break;
                        case 15:
                            countOfDataCodeWord = 254;
                            break;
                        case 25:
                            countOfDataCodeWord = 180;
                            break;
                        case 30:
                            countOfDataCodeWord = 140;
                            break;
                    }
                    break;
                case 12:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 370;
                            break;
                        case 15:
                            countOfDataCodeWord = 290;
                            break;
                        case 25:
                            countOfDataCodeWord = 206;
                            break;
                        case 30:
                            countOfDataCodeWord = 158;
                            break;
                    }
                    break;
                case 13:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 428;
                            break;
                        case 15:
                            countOfDataCodeWord = 334;
                            break;
                        case 25:
                            countOfDataCodeWord = 244;
                            break;
                        case 30:
                            countOfDataCodeWord = 180;
                            break;
                    }
                    break;
                case 14:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 461;
                            break;
                        case 15:
                            countOfDataCodeWord = 365;
                            break;
                        case 25:
                            countOfDataCodeWord = 261;
                            break;
                        case 30:
                            countOfDataCodeWord = 197;
                            break;
                    }
                    break;
                case 15:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 523;
                            break;
                        case 15:
                            countOfDataCodeWord = 415;
                            break;
                        case 25:
                            countOfDataCodeWord = 295;
                            break;
                        case 30:
                            countOfDataCodeWord = 223;
                            break;
                    }
                    break;
                case 16:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 589;
                            break;
                        case 15:
                            countOfDataCodeWord = 453;
                            break;
                        case 25:
                            countOfDataCodeWord = 325;
                            break;
                        case 30:
                            countOfDataCodeWord = 253;
                            break;
                    }
                    break;
                case 17:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 647;
                            break;
                        case 15:
                            countOfDataCodeWord = 507;
                            break;
                        case 25:
                            countOfDataCodeWord = 367;
                            break;
                        case 30:
                            countOfDataCodeWord = 283;
                            break;
                    }
                    break;
                case 18:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 721;
                            break;
                        case 15:
                            countOfDataCodeWord = 563;
                            break;
                        case 25:
                            countOfDataCodeWord = 397;
                            break;
                        case 30:
                            countOfDataCodeWord = 313;
                            break;
                    }
                    break;
                case 19:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 795;
                            break;
                        case 15:
                            countOfDataCodeWord = 627;
                            break;
                        case 25:
                            countOfDataCodeWord = 445;
                            break;
                        case 30:
                            countOfDataCodeWord = 341;
                            break;
                    }
                    break;
                case 20:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 861;
                            break;
                        case 15:
                            countOfDataCodeWord = 669;
                            break;
                        case 25:
                            countOfDataCodeWord = 485;
                            break;
                        case 30:
                            countOfDataCodeWord = 385;
                            break;
                    }
                    break;
                case 21:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 932;
                            break;
                        case 15:
                            countOfDataCodeWord = 714;
                            break;
                        case 25:
                            countOfDataCodeWord = 512;
                            break;
                        case 30:
                            countOfDataCodeWord = 406;
                            break;
                    }
                    break;
                case 22:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 1006;
                            break;
                        case 15:
                            countOfDataCodeWord = 782;
                            break;
                        case 25:
                            countOfDataCodeWord = 568;
                            break;
                        case 30:
                            countOfDataCodeWord = 442;
                            break;
                    }
                    break;
                case 23:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 1094;
                            break;
                        case 15:
                            countOfDataCodeWord = 860;
                            break;
                        case 25:
                            countOfDataCodeWord = 614;
                            break;
                        case 30:
                            countOfDataCodeWord = 464;
                            break;
                    }
                    break;
                case 24:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 1174;
                            break;
                        case 15:
                            countOfDataCodeWord = 914;
                            break;
                        case 25:
                            countOfDataCodeWord = 664;
                            break;
                        case 30:
                            countOfDataCodeWord = 514;
                            break;
                    }
                    break;
                case 25:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 1276;
                            break;
                        case 15:
                            countOfDataCodeWord = 1000;
                            break;
                        case 25:
                            countOfDataCodeWord = 718;
                            break;
                        case 30:
                            countOfDataCodeWord = 538;
                            break;
                    }
                    break;
                case 26:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 1370;
                            break;
                        case 15:
                            countOfDataCodeWord = 1062;
                            break;
                        case 25:
                            countOfDataCodeWord = 754;
                            break;
                        case 30:
                            countOfDataCodeWord = 596;
                            break;
                    }
                    break;
                case 27:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 1468;
                            break;
                        case 15:
                            countOfDataCodeWord = 1128;
                            break;
                        case 25:
                            countOfDataCodeWord = 808;
                            break;
                        case 30:
                            countOfDataCodeWord = 628;
                            break;
                    }
                    break;
                case 28:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 1531;
                            break;
                        case 15:
                            countOfDataCodeWord = 1193;
                            break;
                        case 25:
                            countOfDataCodeWord = 871;
                            break;
                        case 30:
                            countOfDataCodeWord = 661;
                            break;
                    }
                    break;
                case 29:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 1631;
                            break;
                        case 15:
                            countOfDataCodeWord = 1267;
                            break;
                        case 25:
                            countOfDataCodeWord = 911;
                            break;
                        case 30:
                            countOfDataCodeWord = 701;
                            break;
                    }
                    break;
                case 30:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 1735;
                            break;
                        case 15:
                            countOfDataCodeWord = 1373;
                            break;
                        case 25:
                            countOfDataCodeWord = 985;
                            break;
                        case 30:
                            countOfDataCodeWord = 745;
                            break;
                    }
                    break;
                case 31:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 1843;
                            break;
                        case 15:
                            countOfDataCodeWord = 1455;
                            break;
                        case 25:
                            countOfDataCodeWord = 1033;
                            break;
                        case 30:
                            countOfDataCodeWord = 793;
                            break;
                    }
                    break;
                case 32:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 1955;
                            break;
                        case 15:
                            countOfDataCodeWord = 1541;
                            break;
                        case 25:
                            countOfDataCodeWord = 1115;
                            break;
                        case 30:
                            countOfDataCodeWord = 845;
                            break;
                    }
                    break;
                case 33:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 2071;
                            break;
                        case 15:
                            countOfDataCodeWord = 1631;
                            break;
                        case 25:
                            countOfDataCodeWord = 1171;
                            break;
                        case 30:
                            countOfDataCodeWord = 901;
                            break;
                    }
                    break;
                case 34:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 2191;
                            break;
                        case 15:
                            countOfDataCodeWord = 1725;
                            break;
                        case 25:
                            countOfDataCodeWord = 1231;
                            break;
                        case 30:
                            countOfDataCodeWord = 961;
                            break;
                    }
                    break;
                case 35:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 2306;
                            break;
                        case 15:
                            countOfDataCodeWord = 1812;
                            break;
                        case 25:
                            countOfDataCodeWord = 1286;
                            break;
                        case 30:
                            countOfDataCodeWord = 986;
                            break;
                    }
                    break;
                case 36:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 2434;
                            break;
                        case 15:
                            countOfDataCodeWord = 1914;
                            break;
                        case 25:
                            countOfDataCodeWord = 1354;
                            break;
                        case 30:
                            countOfDataCodeWord = 1054;
                            break;
                    }
                    break;
                case 37:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 2566;
                            break;
                        case 15:
                            countOfDataCodeWord = 1992;
                            break;
                        case 25:
                            countOfDataCodeWord = 1426;
                            break;
                        case 30:
                            countOfDataCodeWord = 1096;
                            break;
                    }
                    break;
                case 38:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 2702;
                            break;
                        case 15:
                            countOfDataCodeWord = 2102;
                            break;
                        case 25:
                            countOfDataCodeWord = 1502;
                            break;
                        case 30:
                            countOfDataCodeWord = 1142;
                            break;
                    }
                    break;
                case 39:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 2812;
                            break;
                        case 15:
                            countOfDataCodeWord = 2216;
                            break;
                        case 25:
                            countOfDataCodeWord = 1582;
                            break;
                        case 30:
                            countOfDataCodeWord = 1222;
                            break;
                    }
                    break;
                case 40:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            countOfDataCodeWord = 2956;
                            break;
                        case 15:
                            countOfDataCodeWord = 2334;
                            break;
                        case 25:
                            countOfDataCodeWord = 1666;
                            break;
                        case 30:
                            countOfDataCodeWord = 1276;
                            break;
                    }
                    break;
            }

            return countOfDataCodeWord;
        }

        /// <summary>
        /// Get number of Error correction code words.
        /// </summary>
        private int GetNumberOfErrorCorrectingCodeWords()
        {
            int index = ((int)m_version - 1) * 4;
            switch ((int)m_errorCorrectionLevel)
            {
                case 7:
                    index += 0;
                    break;
                case 15:
                    index += 1;
                    break;
                case 25:
                    index += 2;
                    break;
                case 30:
                    index += 3;
                    break;
            }
            return numberOfErrorCorrectingCodeWords[index];
        }

        /// <summary>
        /// Gets number of Error correction Blocks.
        /// </summary>
        private int[] GetNumberOfErrorCorrectionBlocks()
        {
            int[] numberOfErrorCorrectionBlocks = null;

            switch ((int)m_version)
            {
                #region 1-10
                case 01:
                    numberOfErrorCorrectionBlocks = new int[] { 1 };
                    break;
                case 02:
                    numberOfErrorCorrectionBlocks = new int[] { 1 };
                    break;
                case 03:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 1 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 1 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 2 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 2 };
                            break;
                    }
                    break;
                case 04:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 1 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 2 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 2 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 4 };
                            break;
                    }
                    break;
                case 05:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 1 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 2 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 2, 33, 15, 2, 34, 16 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 2, 33, 11, 2, 34, 12 };
                            break;
                    }
                    break;
                case 06:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 2 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 4 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 4 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 4 };
                            break;
                    }
                    break;
                case 07:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 2 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 4 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 2, 32, 14, 4, 33, 15 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 4, 39, 13, 1, 40, 14 };
                            break;
                    }
                    break;
                case 08:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 2 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 2, 60, 38, 2, 61, 39 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 4, 40, 18, 2, 41, 19 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 4, 40, 14, 2, 41, 15 };
                            break;
                    }
                    break;
                case 09:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 2 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 3, 58, 36, 2, 59, 37 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 4, 36, 16, 4, 37, 17 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 4, 36, 12, 4, 37, 13 };
                            break;
                    }
                    break;
                case 10:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 2, 86, 68, 2, 87, 69 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 4, 69, 43, 1, 70, 44 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 6, 43, 19, 2, 44, 20 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 6, 43, 15, 2, 44, 16 };
                            break;
                    }
                    break;
                #endregion

                #region 11-20
                case 11:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 4 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 1, 80, 50, 4, 81, 51 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 4, 50, 22, 4, 51, 23 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 3, 36, 12, 8, 37, 13 };
                            break;
                    }
                    break;
                case 12:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 2, 116, 92, 2, 117, 93 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 6, 58, 36, 2, 59, 37 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 4, 46, 20, 6, 47, 21 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 7, 42, 14, 4, 43, 15 };
                            break;
                    }
                    break;
                case 13:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 4 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 8, 59, 37, 1, 60, 38 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 8, 44, 20, 4, 45, 21 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 12, 33, 11, 4, 34, 12 };
                            break;
                    }
                    break;
                case 14:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 3, 145, 115, 1, 146, 116 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 4, 64, 40, 5, 65, 41 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 11, 36, 16, 5, 37, 17 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 11, 36, 12, 5, 37, 13 };
                            break;
                    }
                    break;
                case 15:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 5, 109, 87, 1, 110, 88 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 5, 65, 41, 5, 66, 42 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 5, 54, 24, 7, 55, 25 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 11, 36, 12, 7, 37, 13 };
                            break;
                    }
                    break;
                case 16:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 5, 112, 98, 1, 123, 99 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 7, 73, 45, 3, 74, 46 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 15, 43, 19, 2, 44, 20 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 3, 45, 15, 13, 46, 16 };
                            break;
                    }
                    break;
                case 17:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 1, 135, 107, 5, 136, 108 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 10, 74, 46, 1, 75, 47 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 1, 50, 22, 15, 51, 23 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 2, 42, 14, 17, 43, 15 };
                            break;
                    }
                    break;
                case 18:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 5, 150, 120, 1, 151, 121 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 9, 69, 43, 4, 70, 44 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 17, 50, 22, 1, 51, 23 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 2, 42, 14, 19, 43, 15 };
                            break;
                    }
                    break;
                case 19:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 3, 141, 113, 4, 142, 114 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 3, 70, 44, 11, 71, 45 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 17, 47, 21, 4, 48, 22 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 9, 39, 13, 16, 40, 14 };
                            break;
                    }
                    break;
                case 20:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 3, 135, 107, 5, 136, 108 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 3, 67, 41, 13, 68, 42 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 15, 54, 24, 5, 55, 25 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 15, 43, 15, 10, 44, 16 };
                            break;
                    }
                    break;
                #endregion

                #region 21-30
                case 21:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 4, 144, 116, 4, 145, 117 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 17 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 17, 50, 22, 6, 51, 23 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 19, 46, 16, 6, 47, 17 };
                            break;
                    }
                    break;
                case 22:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 2, 139, 111, 7, 140, 112 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 17 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 7, 54, 24, 16, 55, 25 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 34 };
                            break;
                    }
                    break;
                case 23:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 4, 151, 121, 5, 152, 122 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 4, 75, 47, 14, 76, 48 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 11, 54, 24, 14, 55, 25 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 16, 45, 15, 14, 46, 16 };
                            break;
                    }
                    break;
                case 24:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 6, 147, 117, 4, 148, 118 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 6, 73, 45, 14, 74, 46 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 11, 54, 24, 16, 55, 25 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 30, 46, 16, 2, 47, 17 };
                            break;
                    }
                    break;
                case 25:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 8, 132, 106, 4, 133, 107 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 8, 75, 47, 13, 76, 48 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 7, 54, 24, 22, 55, 25 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 22, 45, 15, 13, 46, 16 };
                            break;
                    }
                    break;
                case 26:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 10, 142, 114, 2, 143, 115 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 19, 74, 46, 4, 75, 47 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 28, 50, 22, 6, 51, 23 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 33, 46, 16, 4, 47, 17 };
                            break;
                    }
                    break;
                case 27:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 8, 152, 122, 4, 153, 123 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 22, 73, 45, 3, 74, 46 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 8, 53, 23, 26, 54, 24 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 12, 45, 15, 28, 46, 16 };
                            break;
                    }
                    break;
                case 28:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 3, 147, 117, 10, 148, 118 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 3, 73, 45, 23, 74, 46 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 4, 54, 24, 31, 55, 25 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 11, 45, 15, 31, 46, 16 };
                            break;
                    }
                    break;
                case 29:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 7, 146, 116, 7, 147, 117 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 21, 73, 45, 7, 74, 46 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 1, 53, 23, 37, 54, 24 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 19, 45, 15, 26, 46, 16 };
                            break;
                    }
                    break;
                case 30:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 5, 145, 115, 10, 146, 116 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 19, 75, 47, 10, 76, 48 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 15, 54, 24, 25, 55, 25 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 23, 45, 15, 25, 46, 16 };
                            break;
                    }
                    break;
                #endregion

                #region 31-40
                case 31:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 13, 145, 115, 3, 146, 116 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 2, 74, 46, 29, 75, 47 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 42, 54, 24, 1, 55, 25 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 23, 45, 15, 28, 46, 16 };
                            break;
                    }
                    break;
                case 32:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 17 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 10, 74, 46, 23, 75, 47 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 10, 54, 24, 35, 55, 25 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 19, 45, 15, 35, 46, 16 };
                            break;
                    }
                    break;
                case 33:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 17, 145, 115, 1, 146, 116 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 14, 74, 46, 21, 75, 47 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 29, 54, 24, 19, 55, 25 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 11, 45, 15, 46, 46, 16 };
                            break;
                    }
                    break;
                case 34:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 13, 145, 115, 6, 146, 116 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 14, 74, 46, 23, 75, 47 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 44, 54, 24, 7, 55, 25 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 59, 46, 16, 1, 47, 17 };
                            break;
                    }
                    break;
                case 35:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 12, 151, 121, 7, 152, 122 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 12, 75, 47, 26, 76, 48 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 39, 54, 24, 14, 55, 25 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 22, 45, 15, 41, 46, 16 };
                            break;
                    }
                    break;
                case 36:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 6, 151, 121, 14, 152, 122 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 6, 75, 47, 34, 76, 48 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 46, 54, 24, 10, 55, 25 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 2, 45, 15, 64, 46, 16 };
                            break;
                    }
                    break;
                case 37:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 17, 152, 122, 4, 153, 123 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 29, 74, 46, 14, 75, 47 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 49, 54, 24, 10, 55, 25 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 24, 45, 15, 46, 46, 16 };
                            break;
                    }
                    break;
                case 38:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 4, 152, 122, 18, 153, 123 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 13, 74, 46, 32, 75, 47 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 48, 54, 24, 14, 55, 25 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 42, 45, 15, 32, 46, 16 };
                            break;
                    }
                    break;
                case 39:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 20, 147, 117, 4, 148, 118 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 40, 75, 47, 7, 76, 48 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 43, 54, 24, 22, 55, 25 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 10, 45, 15, 67, 46, 16 };
                            break;
                    }
                    break;
                case 40:
                    switch ((int)m_errorCorrectionLevel)
                    {
                        case 7:
                            numberOfErrorCorrectionBlocks = new int[] { 19, 148, 118, 6, 149, 119 };
                            break;
                        case 15:
                            numberOfErrorCorrectionBlocks = new int[] { 18, 75, 47, 31, 76, 48 };
                            break;
                        case 25:
                            numberOfErrorCorrectionBlocks = new int[] { 34, 54, 24, 34, 55, 25 };
                            break;
                        case 30:
                            numberOfErrorCorrectionBlocks = new int[] { 20, 45, 15, 61, 46, 16 };
                            break;
                    }
                    break;
                #endregion
            }

            return numberOfErrorCorrectionBlocks;

        }

        /// <summary>
        /// Gets the End of the version.
        /// </summary>
        private int GetEnd()
        {
            return EndValues[(int)m_version - 1];
        }

        /// <summary>
        /// Gets Data capacity.
        /// </summary>
        private int GetDataCapacity()
        {
            return DataCapacityValues[(int)m_version - 1];
        }

        /// <summary>
        /// Gets Format Information.
        /// </summary>
        private byte[] GetFormatInformation()
        {
            byte[] formatInformation = null;
            switch ((int)m_errorCorrectionLevel)
            {
                case 7:
                    formatInformation = new byte[] { 1, 0, 1, 1, 1, 0, 0, 1, 0, 0, 0, 1, 1, 1, 1 };
                    break;
                case 15:
                    formatInformation = new byte[] { 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 1, 0, 1 };
                    break;
                case 25:
                    formatInformation = new byte[] { 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0 };
                    break;
                case 30:
                    formatInformation = new byte[] { 0, 0, 0, 0, 1, 0, 1, 1, 1, 0, 0, 1, 1, 0, 0 };
                    break;
            }
            return formatInformation;
        }

        /// <summary>
        /// Gets Version Information.
        /// </summary>
        private byte[] GetVersionInformation()
        {
            byte[] versionInformation = null;

            switch ((int)m_version)
            {
                case 07:
                    versionInformation = new byte[] { 0, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0 };
                    break;
                case 08:
                    versionInformation = new byte[] { 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 1, 0, 0, 0, 0, 1, 0, 0 };
                    break;
                case 09:
                    versionInformation = new byte[] { 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0, 1, 1, 0, 0, 1, 0, 0 };
                    break;
                case 10:
                    versionInformation = new byte[] { 1, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0 };
                    break;
                case 11:
                    versionInformation = new byte[] { 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 0 };
                    break;
                case 12:
                    versionInformation = new byte[] { 0, 1, 0, 0, 0, 1, 1, 0, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0 };
                    break;
                case 13:
                    versionInformation = new byte[] { 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 0 };
                    break;
                case 14:
                    versionInformation = new byte[] { 1, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0 };
                    break;
                case 15:
                    versionInformation = new byte[] { 0, 0, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 1, 1, 1, 1, 0, 0 };
                    break;
                case 16:
                    versionInformation = new byte[] { 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 1, 0, 0, 0, 0, 1, 0 };
                    break;
                case 17:
                    versionInformation = new byte[] { 1, 0, 1, 1, 1, 0, 1, 0, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0 };
                    break;
                case 18:
                    versionInformation = new byte[] { 1, 1, 1, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0 };
                    break;
                case 19:
                    versionInformation = new byte[] { 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0, 1, 1, 0, 0, 1, 0 };
                    break;
                case 20:
                    versionInformation = new byte[] { 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0 };
                    break;
                case 21:
                    versionInformation = new byte[] { 1, 1, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0 };
                    break;
                case 22:
                    versionInformation = new byte[] { 1, 0, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0 };
                    break;
                case 23:
                    versionInformation = new byte[] { 0, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 0 };
                    break;
                case 24:
                    versionInformation = new byte[] { 0, 0, 1, 0, 0, 0, 1, 1, 0, 1, 1, 1, 0, 0, 0, 1, 1, 0 };
                    break;
                case 25:
                    versionInformation = new byte[] { 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0, 1, 1, 0 };
                    break;
                case 26:
                    versionInformation = new byte[] { 1, 1, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 0 };
                    break;
                case 27:
                    versionInformation = new byte[] { 0, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 0, 1, 1, 0 };
                    break;
                case 28:
                    versionInformation = new byte[] { 0, 1, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0 };
                    break;
                case 29:
                    versionInformation = new byte[] { 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 1, 0 };
                    break;
                case 30:
                    versionInformation = new byte[] { 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 0, 1, 1, 1, 1, 0 };
                    break;
                case 31:
                    versionInformation = new byte[] { 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 1, 1, 1, 1, 0 };
                    break;
                case 32:
                    versionInformation = new byte[] { 1, 0, 1, 0, 1, 0, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1 };
                    break;
                case 33:
                    versionInformation = new byte[] { 0, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 1, 0, 0, 0, 0, 1 };
                    break;
                case 34:
                    versionInformation = new byte[] { 0, 1, 0, 1, 1, 1, 0, 1, 0, 0, 0, 1, 0, 1, 0, 0, 0, 1 };
                    break;
                case 35:
                    versionInformation = new byte[] { 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 1 };
                    break;
                case 36:
                    versionInformation = new byte[] { 1, 1, 0, 1, 0, 0, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 0, 1 };
                    break;
                case 37:
                    versionInformation = new byte[] { 0, 1, 1, 1, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1 };
                    break;
                case 38:
                    versionInformation = new byte[] { 0, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0, 1, 1, 0, 0, 1 };
                    break;
                case 39:
                    versionInformation = new byte[] { 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1, 0, 0, 1 };
                    break;
                case 40:
                    versionInformation = new byte[] { 1, 0, 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1 };
                    break;
            }

            return versionInformation;
        }

        /// <summary>
        /// Gets equivalent Number of the Character.
        /// </summary>
        internal int GetNumberInEci(char inputChar)
        {
            string inputHex = ((int)inputChar).ToString("X");

            int index = Array.IndexOf(CP437CharSet, inputHex);
            if (index > -1)
                return CP437ReplaceNumber[index];

            index = Array.IndexOf(ISO8859_2CharSet, inputHex);
            if (index > -1)
                return ISO8859_2ReplaceNumber[index];

            index = Array.IndexOf(ISO8859_3CharSet, inputHex);
            if (index > -1)
                return ISO8859_3ReplaceNumber[index];

            index = Array.IndexOf(ISO8859_4CharSet, inputHex);
            if (index > -1)
                return ISO8859_4ReplaceNumber[index];

            if ((int)inputChar >= 1025 && (int)inputChar <= 1119 &&
                (int)inputChar != 1037 && (int)inputChar != 1104 && (int)inputChar != 1117)
                return ((int)inputChar - 864);

            if (((int)inputChar >= 1569 && (int)inputChar <= 1594) ||
                ((int)inputChar >= 1600 && (int)inputChar <= 1618) ||
                  (int)inputChar == 1567 || (int)inputChar == 1563 || (int)inputChar == 1548)
                return ((int)inputChar - 1376);

            if (((int)inputChar >= 900 && (int)inputChar <= 974) || (int)inputChar == 890)
                return ((int)inputChar - 720);

            if (((int)inputChar >= 1488 && (int)inputChar <= 1514))
                return ((int)inputChar - 1264);

            if (((int)inputChar >= 3585 && (int)inputChar <= 3675))
                return ((int)inputChar - 3424);


            index = Array.IndexOf(Windows1250CharSet, inputHex);
            if (index > -1)
                return Windows1250ReplaceNumber[index];

            index = Array.IndexOf(Windows1251CharSet, inputHex);
            if (index > -1)
                return Windows1251ReplaceNumber[index];

            index = Array.IndexOf(Windows1252CharSet, inputHex);
            if (index > -1)
                return Windows1252ReplaceNumber[index];

            index = Array.IndexOf(Windows1256CharSet, inputHex);
            if (index > -1)
                return Windows1256ReplaceNumber[index];


            return (int)inputChar;
        }

        /// <summary>
        /// Gets Numeric Data capacity.
        /// </summary>
#if !XAML && !GDI
        internal static int GetNumericDataCapacity(QRCodeVersion version, PdfErrorCorrectionLevel errorCorrectionLevel)
#else
        internal static int GetNumericDataCapacity(QRBarcodeVersion version, ErrorCorrectionLevel errorCorrectionLevel)
#endif
        {
            int[] capacity = null;

            switch ((int)errorCorrectionLevel)
            {
                case 7:
                    capacity = NumericDataCapacityLow;
                    break;
                case 15:
                    capacity = NumericDataCapacityMedium;
                    break;
                case 25:
                    capacity = NumericDataCapacityQuartile;
                    break;
                case 30:
                    capacity = NumericDataCapacityHigh;
                    break;

            }

            return capacity[(int)version - 1];
        }

        /// <summary>
        /// Gets Alphanumeric data capacity.
        /// </summary>
#if !XAML && !GDI
        internal static int GetAlphanumericDataCapacity(QRCodeVersion version, PdfErrorCorrectionLevel errorCorrectionLevel)
#else
        internal static int GetAlphanumericDataCapacity(QRBarcodeVersion version, ErrorCorrectionLevel errorCorrectionLevel)
#endif
        {
            int[] capacity = null;

            switch ((int)errorCorrectionLevel)
            {
                case 7:
                    capacity = AlphanumericDataCapacityLow;
                    break;
                case 15:
                    capacity = AlphanumericDataCapacityMedium;
                    break;
                case 25:
                    capacity = AlphanumericDataCapacityQuartile;
                    break;
                case 30:
                    capacity = AlphanumericDataCapacityHigh;
                    break;

            }

            return capacity[(int)version - 1];
        }

#if !XAML && !GDI
        internal static int GetBinaryDataCapacity(QRCodeVersion version, PdfErrorCorrectionLevel errorCorrectionLevel)
#else
        internal static int GetBinaryDataCapacity(QRBarcodeVersion version, ErrorCorrectionLevel errorCorrectionLevel)
#endif
        {
            int[] capacity = null;

            switch ((int)errorCorrectionLevel)
            {
                case 7:
                    capacity = BinaryDataCapacityLow;
                    break;
                case 15:
                    capacity = BinaryDataCapacityMedium;
                    break;
                case 25:
                    capacity = BinaryDataCapacityQuartile;
                    break;
                case 30:
                    capacity = BinaryDataCapacityHigh;
                    break;

            }

            return capacity[(int)version - 1];
        }

        #endregion
    }
}
#endif
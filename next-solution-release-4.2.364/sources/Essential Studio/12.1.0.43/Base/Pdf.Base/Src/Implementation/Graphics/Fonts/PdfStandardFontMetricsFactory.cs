#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Globalization;

namespace Syncfusion.Pdf.Graphics.Fonts
{
    /// <summary>
    /// Factory of the standard fonts metrics.
    /// </summary>
    internal class PdfStandardFontMetricsFactory
    {
        #region Constants
        /// <summary>
        /// Multiplier os subscript superscript.
        /// </summary>
        private const float c_subSuperScriptFactor = 1.52f;


        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_HelveticaAscent = 931f;

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_HelveticaDescent = -225f;

        /// <summary>
        /// Font type
        /// </summary>
        private const string c_HelveticaName = "Helvetica";

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_HelveticaBoldAscent = 962f;

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_HelveticaBoldDescent = -228f;

        /// <summary>
        /// Font type
        /// </summary>
        private const string c_HelveticaBoldName = "Helvetica-Bold";

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_HelveticaItalicAscent = 931f;

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_HelveticaItalicDescent = -225f;

        /// <summary>
        /// Font type
        /// </summary>
        private const string c_HelveticaItalicName = "Helvetica-Oblique";

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_HelveticaBoldItalicAscent = 962f;

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_HelveticaBoldItalicDescent = -228f;

        /// <summary>
        /// Font type
        /// </summary>
        private const string c_HelveticaBoldItalicName = "Helvetica-BoldOblique";

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_CourierAscent = 805f;

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_CourierDescent = -250f;

        /// <summary>
        /// Font type.
        /// </summary>
        private const string c_CourierName = "Courier";

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_CourierBoldAscent = 801f;

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_CourierBoldDescent = -250f;

        /// <summary>
        /// Font type.
        /// </summary>
        private const string c_CourierBoldName = "Courier-Bold";

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_CourierItalicAscent = 805f;

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_CourierItalicDescent = -250f;

        /// <summary>
        /// Font type.
        /// </summary>
        private const string c_CourierItalicName = "Courier-Oblique";

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_CourierBoldItalicAscent = 801f;

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_CourierBoldItalicDescent = -250f;

        /// <summary>
        /// Font type.
        /// </summary>
        private const string c_CourierBoldItalicName = "Courier-BoldOblique";

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_TimesAscent = 898f;

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_TimesDescent = -218f;

        /// <summary>
        /// Font type.
        /// </summary>
        private const string c_TimesName = "Times-Roman";

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_TimesBoldAscent = 935f;

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_TimesBoldDescent = -218f;

        /// <summary>
        /// Font type.
        /// </summary>
        private const string c_TimesBoldName = "Times-Bold";

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_TimesItalicAscent = 883f;

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_TimesItalicDescent = -217f;

        /// <summary>
        /// Font type.
        /// </summary>
        private const string c_TimesItalicName = "Times-Italic";

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_TimesBoldItalicAscent = 921f;

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_TimesBoldItalicDescent = -218f;

        /// <summary>
        /// Font type.
        /// </summary>
        private const string c_TimesBoldItalicName = "Times-BoldItalic";

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_symbolAscent = 1010f;

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_symbolDescent = -293f;

        /// <summary>
        /// Font type.
        /// </summary>
        private const string c_symbolName = "Symbol";

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_zapfDingbatsAscent = 820f;

        /// <summary>
        /// Ascender value for the font.
        /// </summary>
        private const float c_zapfDingbatsDescent = -143f;

        /// <summary>
        /// Font type.
        /// </summary>
        private const string c_zapfDingbatsName = "ZapfDingbats";

        /// <summary>
        /// Arial widths table.
        /// </summary>
        private static int[] c_arialWidth =
			{
				278, 278, 355, 556, 556, 889, 667, 191, 333, 333, 389, 584, 278, 333,
				278, 278, 556, 556, 556, 556, 556, 556, 556, 556, 556, 556, 278, 278, 584, 584,
				584, 556, 1015, 667, 667, 722, 722, 667, 611, 778, 722, 278, 500, 667, 556, 833,
				722, 778, 667, 778, 722, 667, 611, 722, 667, 944, 667, 667, 611, 278, 278, 278,
				469, 556, 333, 556, 556, 500, 556, 556, 278, 556, 556, 222, 222, 500, 222, 833,
				556, 556, 556, 556, 333, 500, 278, 556, 500, 722, 500, 500, 500, 334, 260, 334,
				584, 0, 556, 0, 222, 556, 333, 1000, 556, 556, 333, 1000, 667, 333, 1000, 0,
				611, 0, 0, 222, 222, 333, 333, 350, 556, 1000, 333, 1000, 500, 333, 944, 0,
				500, 667, 0, 333, 556, 556, 556, 556, 260, 556, 333, 737, 370, 556, 584, 0,
				737, 333, 400, 584, 333, 333, 333, 556, 537, 278, 333, 333, 365, 556, 834, 834,
				834, 611, 667, 667, 667, 667, 667, 667, 1000, 722, 667, 667, 667, 667, 278, 278,
				278, 278, 722, 722, 778, 778, 778, 778, 778, 584, 778, 722, 722, 722, 722, 667,
				667, 611, 556, 556, 556, 556, 556, 556, 889, 500, 556, 556, 556, 556, 278, 278,
				278, 278, 556, 556, 556, 556, 556, 556, 556, 584, 611, 556, 556, 556, 556, 500,
				556, 500
			};

        /// <summary>
        /// Arial bold widths table.
        /// </summary>
        private static int[] c_arialBoldWidth =
			{
				278, 333, 474, 556, 556, 889, 722, 238, 333, 333, 389, 584, 278, 333,
				278, 278, 556, 556, 556, 556, 556, 556, 556, 556, 556, 556, 333, 333, 584, 584,
				584, 611, 975, 722, 722, 722, 722, 667, 611, 778, 722, 278, 556, 722, 611, 833,
				722, 778, 667, 778, 722, 667, 611, 722, 667, 944, 667, 667, 611, 333, 278, 333,
				584, 556, 333, 556, 611, 556, 611, 556, 333, 611, 611, 278, 278, 556, 278, 889,
				611, 611, 611, 611, 389, 556, 333, 611, 556, 778, 556, 556, 500, 389, 280, 389,
				584, 0, 556, 0, 278, 556, 500, 1000, 556, 556, 333, 1000, 667, 333, 1000, 0,
				611, 0, 0, 278, 278, 500, 500, 350, 556, 1000, 333, 1000, 556, 333, 944, 0,
				500, 667, 0, 333, 556, 556, 556, 556, 280, 556, 333, 737, 370, 556, 584, 0,
				737, 333, 400, 584, 333, 333, 333, 611, 556, 278, 333, 333, 365, 556, 834, 834,
				834, 611, 722, 722, 722, 722, 722, 722, 1000, 722, 667, 667, 667, 667, 278, 278,
				278, 278, 722, 722, 778, 778, 778, 778, 778, 584, 778, 722, 722, 722, 722, 667,
				667, 611, 556, 556, 556, 556, 556, 556, 889, 556, 556, 556, 556, 556, 278, 278,
				278, 278, 611, 611, 611, 611, 611, 611, 611, 584, 611, 611, 611, 611, 611, 556,
				611, 556
			};

        /// <summary>
        /// Fixed widths table.
        /// </summary>
        private static int[] c_fixedWidth =
			{
				600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600,
				600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600,
				600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600,
				600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600,
				600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600,
				600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600,
				600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600,
				600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600,
				600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600,
				600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600,
				600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600,
				600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600,
				600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600,
				600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600
			};

        /// <summary>
        /// Times widths table.
        /// </summary>
        private static int[] c_timesRomanWidth =
			{
				250, 333, 408, 500, 500, 833, 778, 180, 333, 333, 500, 564, 250, 333,
				250, 278, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 278, 278, 564, 564,
				564, 444, 921, 722, 667, 667, 722, 611, 556, 722, 722, 333, 389, 722, 611, 889,
				722, 722, 556, 722, 667, 556, 611, 722, 722, 944, 722, 722, 611, 333, 278, 333,
				469, 500, 333, 444, 500, 444, 500, 444, 333, 500, 500, 278, 278, 500, 278, 778,
				500, 500, 500, 500, 333, 389, 278, 500, 500, 722, 500, 500, 444, 480, 200, 480,
				541, 000, 500, 000, 333, 500, 444, 1000, 500, 500, 333, 1000, 556, 333, 889, 0,
				611, 000, 000, 333, 333, 444, 444, 350, 500, 1000, 333, 980, 389, 333, 722, 0,
				444, 722, 000, 333, 500, 500, 500, 500, 200, 500, 333, 760, 276, 500, 564, 0,
				760, 333, 400, 564, 300, 300, 333, 500, 453, 250, 333, 300, 310, 500, 750, 750,
				750, 444, 722, 722, 722, 722, 722, 722, 889, 667, 611, 611, 611, 611, 333, 333,
				333, 333, 722, 722, 722, 722, 722, 722, 722, 564, 722, 722, 722, 722, 722, 722,
				556, 500, 444, 444, 444, 444, 444, 444, 667, 444, 444, 444, 444, 444, 278, 278,
				278, 278, 500, 500, 500, 500, 500, 500, 500, 564, 500, 500, 500, 500, 500, 500,
				500, 500
			};

        /// <summary>
        /// Times bold widths table.
        /// </summary>
        private static int[] c_timesRomanBoldWidth =
			{
				250, 333, 555, 500, 500, 1000, 833, 278, 333, 333, 500, 570, 250, 333,
				250, 278, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 333, 333, 570, 570,
				570, 500, 930, 722, 667, 722, 722, 667, 611, 778, 778, 389, 500, 778, 667, 944,
				722, 778, 611, 778, 722, 556, 667, 722, 722, 1000, 722, 722, 667, 333, 278, 333,
				581, 500, 333, 500, 556, 444, 556, 444, 333, 500, 556, 278, 333, 556, 278, 833,
				556, 500, 556, 556, 444, 389, 333, 556, 500, 722, 500, 500, 444, 394, 220, 394,
				520, 0, 500, 0, 333, 500, 500, 1000, 500, 500, 333, 1000, 556, 333, 1000, 0,
				667, 0, 0, 333, 333, 500, 500, 350, 500, 1000, 333, 1000, 389, 333, 722, 0,
				444, 722, 0, 333, 500, 500, 500, 500, 220, 500, 333, 747, 300, 500, 570, 0,
				747, 333, 400, 570, 300, 300, 333, 556, 540, 250, 333, 300, 330, 500, 750, 750,
				750, 500, 722, 722, 722, 722, 722, 722, 1000, 722, 667, 667, 667, 667, 389, 389,
				389, 389, 722, 722, 778, 778, 778, 778, 778, 570, 778, 722, 722, 722, 722, 722,
				611, 556, 500, 500, 500, 500, 500, 500, 722, 444, 444, 444, 444, 444, 278, 278,
				278, 278, 500, 556, 500, 500, 500, 500, 500, 570, 500, 556, 556, 556, 556, 500,
				556, 500
			};

        /// <summary>
        /// Times italic widths table.
        /// </summary>
        private static int[] c_timesRomanItalicWidth =
			{
				250, 333, 420, 500, 500, 833, 778, 214, 333, 333, 500, 675, 250, 333,
				250, 278, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 333, 333, 675, 675,
				675, 500, 920, 611, 611, 667, 722, 611, 611, 722, 722, 333, 444, 667, 556, 833,
				667, 722, 611, 722, 611, 500, 556, 722, 611, 833, 611, 556, 556, 389, 278, 389,
				422, 500, 333, 500, 500, 444, 500, 444, 278, 500, 500, 278, 278, 444, 278, 722,
				500, 500, 500, 500, 389, 389, 278, 500, 444, 667, 444, 444, 389, 400, 275, 400,
				541, 0, 500, 0, 333, 500, 556, 889, 500, 500, 333, 1000, 500, 333, 944, 0,
				556, 0, 0, 333, 333, 556, 556, 350, 500, 889, 333, 980, 389, 333, 667, 0,
				389, 556, 0, 389, 500, 500, 500, 500, 275, 500, 333, 760, 276, 500, 675, 0,
				760, 333, 400, 675, 300, 300, 333, 500, 523, 250, 333, 300, 310, 500, 750, 750,
				750, 500, 611, 611, 611, 611, 611, 611, 889, 667, 611, 611, 611, 611, 333, 333,
				333, 333, 722, 667, 722, 722, 722, 722, 722, 675, 722, 722, 722, 722, 722, 556,
				611, 500, 500, 500, 500, 500, 500, 500, 667, 444, 444, 444, 444, 444, 278, 278,
				278, 278, 500, 500, 500, 500, 500, 500, 500, 675, 500, 500, 500, 500, 500, 444,
				500, 444
			};

        /// <summary>
        /// Times bold italic widths table.
        /// </summary>
        public static int[] c_timesRomanBoldItalicWidth =
			{
				250, 389, 555, 500, 500, 833, 778, 278, 333, 333, 500, 570, 250, 333,
				250, 278, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 333, 333, 570, 570,
				570, 500, 832, 667, 667, 667, 722, 667, 667, 722, 778, 389, 500, 667, 611, 889,
				722, 722, 611, 722, 667, 556, 611, 722, 667, 889, 667, 611, 611, 333, 278, 333,
				570, 500, 333, 500, 500, 444, 500, 444, 333, 500, 556, 278, 278, 500, 278, 778,
				556, 500, 500, 500, 389, 389, 278, 556, 444, 667, 500, 444, 389, 348, 220, 348,
				570, 0, 500, 0, 333, 500, 500, 1000, 500, 500, 333, 1000, 556, 333, 944, 0,
				611, 0, 0, 333, 333, 500, 500, 350, 500, 1000, 333, 1000, 389, 333, 722, 0,
				389, 611, 0, 389, 500, 500, 500, 500, 220, 500, 333, 747, 266, 500, 606, 0,
				747, 333, 400, 570, 300, 300, 333, 576, 500, 250, 333, 300, 300, 500, 750, 750,
				750, 500, 667, 667, 667, 667, 667, 667, 944, 667, 667, 667, 667, 667, 389, 389,
				389, 389, 722, 722, 722, 722, 722, 722, 722, 570, 722, 722, 722, 722, 722, 611,
				611, 500, 500, 500, 500, 500, 500, 500, 722, 444, 444, 444, 444, 444, 278, 278,
				278, 278, 500, 556, 500, 500, 500, 500, 500, 570, 500, 556, 556, 556, 556, 444,
				500, 444
			};

        /// <summary>
        /// Symbol widths table.
        /// </summary>
        private static int[] c_symbolWidth =
			{
				250, 333, 713, 500, 549, 833, 778, 439, 333, 333, 500, 549, 250, 549,
				250, 278, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 278, 278,
				549, 549, 549, 444, 549, 722, 667, 722, 612, 611, 763, 603, 722, 333,
				631, 722, 686, 889, 722, 722, 768, 741, 556, 592, 611, 690, 439, 768,
				645, 795, 611, 333, 863, 333, 658, 500, 500, 631, 549, 549, 494, 439,
				521, 411, 603, 329, 603, 549, 549, 576, 521, 549, 549, 521, 549, 603,
				439, 576, 713, 686, 493, 686, 494, 480, 200, 480, 549, 750, 620, 247,
				549, 167, 713, 500, 753, 753, 753, 753, 1042, 987, 603, 987, 603, 400,
				549, 411, 549, 549, 713, 494, 460, 549, 549, 549, 549, 1000, 603, 1000,
				658, 823, 686, 795, 987, 768, 768, 823, 768, 768, 713, 713, 713, 713,
				713, 713, 713, 768, 713, 790, 790, 890, 823, 549, 250, 713, 603, 603,
				1042, 987, 603, 987, 603, 494, 329, 790, 790, 786, 713, 384, 384, 384,
				384, 384, 384, 494, 494, 494, 494, 329, 274, 686, 686, 686, 384, 384,
				384, 384, 384, 384, 494, 494, 494, -1
			};

        /// <summary>
        /// Zip dingbats widths table.
        /// </summary>
        private static int[] c_zapfDingbatsWidth =
			{
				278, 974, 961, 974, 980, 719, 789, 790, 791, 690, 960, 939, 549, 855,
				911, 933, 911, 945, 974, 755, 846, 762, 761, 571, 677, 763, 760, 759,
				754, 494, 552, 537, 577, 692, 786, 788, 788, 790, 793, 794, 816, 823,
				789, 841, 823, 833, 816, 831, 923, 744, 723, 749, 790, 792, 695, 776,
				768, 792, 759, 707, 708, 682, 701, 826, 815, 789, 789, 707, 687, 696,
				689, 786, 787, 713, 791, 785, 791, 873, 761, 762, 762, 759, 759, 892,
				892, 788, 784, 438, 138, 277, 415, 392, 392, 668, 668, 390, 390, 317,
				317, 276, 276, 509, 509, 410, 410, 234, 234, 334, 334, 732, 544, 544,
				910, 667, 760, 760, 776, 595, 694, 626, 788, 788, 788, 788, 788, 788,
				788, 788, 788, 788, 788, 788, 788, 788, 788, 788, 788, 788, 788, 788,
				788, 788, 788, 788, 788, 788, 788, 788, 788, 788, 788, 788, 788, 788,
				788, 788, 788, 788, 788, 788, 894, 838, 1016, 458, 748, 924, 748, 918,
				927, 928, 928, 834, 873, 828, 924, 924, 917, 930, 931, 463, 883, 836,
				836, 867, 867, 696, 696, 874, 874, 760, 946, 771, 865, 771, 888, 967,
				888, 831, 873, 927, 970, 918
			};

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfStandardFontMetricsFactory"/> class.
        /// </summary>
        private PdfStandardFontMetricsFactory()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Returns metrics of the font.
        /// </summary>
        /// <param name="fontFamily">Family of the font.</param>
        /// <param name="fontStyle">Style of the font.</param>
        /// <param name="size">Size of the font.</param>
        /// <returns>Returns metrics of the font.</returns>
        public static PdfFontMetrics GetMetrics(PdfFontFamily fontFamily, PdfFontStyle fontStyle, float size)
        {
            PdfFontMetrics metrics = null;

            switch (fontFamily)
            {
                case PdfFontFamily.Helvetica:
                    metrics = GetHelveticaMetrics(fontFamily, fontStyle, size);
                    break;

                case PdfFontFamily.Courier:
                    metrics = GetCourierMetrics(fontFamily, fontStyle, size);
                    break;

                case PdfFontFamily.TimesRoman:
                    metrics = GetTimesMetrics(fontFamily, fontStyle, size);
                    break;

                case PdfFontFamily.Symbol:
                    metrics = GetSymbolMetrics(fontFamily, fontStyle, size);
                    break;

                case PdfFontFamily.ZapfDingbats:
                    metrics = GetZapfDingbatsMetrics(fontFamily, fontStyle, size);
                    break;

                default:
                    metrics = GetHelveticaMetrics(PdfFontFamily.Helvetica, fontStyle, size);
                    break;
            }

            metrics.Name = fontFamily.ToString();
            metrics.SubScriptSizeFactor = c_subSuperScriptFactor;
            metrics.SuperscriptSizeFactor = c_subSuperScriptFactor;

            return metrics;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Creates Helvetica font metrics.
        /// </summary>
        /// <param name="fontFamily">FontFamily of the font.</param>
        /// <param name="fontStyle">Style of the font.</param>
        /// <param name="size">Size of the font.</param>
        /// <returns>Helvetica font metrics.</returns>
        private static PdfFontMetrics GetHelveticaMetrics(PdfFontFamily fontFamily, PdfFontStyle fontStyle, float size)
        {
            PdfFontMetrics metrics = new PdfFontMetrics();

            if ((fontStyle & PdfFontStyle.Bold) > 0 && (fontStyle & PdfFontStyle.Italic) > 0)
            {
                metrics.Ascent = c_HelveticaBoldItalicAscent;
                metrics.Descent = c_HelveticaBoldItalicDescent;
                metrics.PostScriptName = c_HelveticaBoldItalicName;
                metrics.Size = size;
                metrics.WidthTable = new StandardWidthTable(c_arialBoldWidth);
                metrics.Height = metrics.Ascent - metrics.Descent;
            }
            else if ((fontStyle & PdfFontStyle.Bold) > 0)
            {
                metrics.Ascent = c_HelveticaBoldAscent;
                metrics.Descent = c_HelveticaBoldDescent;
                metrics.PostScriptName = c_HelveticaBoldName;
                metrics.Size = size;
                metrics.WidthTable = new StandardWidthTable(c_arialBoldWidth);
                metrics.Height = metrics.Ascent - metrics.Descent;
            }
            else if ((fontStyle & PdfFontStyle.Italic) > 0)
            {
                metrics.Ascent = c_HelveticaItalicAscent;
                metrics.Descent = c_HelveticaItalicDescent;
                metrics.PostScriptName = c_HelveticaItalicName;
                metrics.Size = size;
                metrics.WidthTable = new StandardWidthTable(c_arialWidth);
                metrics.Height = metrics.Ascent - metrics.Descent;
            }
            else
            {
                metrics.Ascent = c_HelveticaAscent;
                metrics.Descent = c_HelveticaDescent;
                metrics.PostScriptName = c_HelveticaName;
                metrics.Size = size;
                metrics.WidthTable = new StandardWidthTable(c_arialWidth);
                metrics.Height = metrics.Ascent - metrics.Descent;
            }

            return metrics;
        }

        /// <summary>
        /// Creates Courier font metrics.
        /// </summary>
        /// <param name="fontFamily">FontFamily of the font.</param>
        /// <param name="fontStyle">Style of the font.</param>
        /// <param name="size">Size of the font.</param>
        /// <returns>Helvetica font metrics.</returns>
        private static PdfFontMetrics GetCourierMetrics(PdfFontFamily fontFamily, PdfFontStyle fontStyle, float size)
        {
            PdfFontMetrics metrics = new PdfFontMetrics();

            if ((fontStyle & PdfFontStyle.Bold) > 0 && (fontStyle & PdfFontStyle.Italic) > 0)
            {
                metrics.Ascent = c_CourierBoldItalicAscent;
                metrics.Descent = c_CourierBoldItalicDescent;
                metrics.PostScriptName = c_CourierBoldItalicName;
                metrics.Size = size;
                metrics.WidthTable = new StandardWidthTable(c_fixedWidth);
                metrics.Height = metrics.Ascent - metrics.Descent;
            }
            else if ((fontStyle & PdfFontStyle.Bold) > 0)
            {
                metrics.Ascent = c_CourierBoldAscent;
                metrics.Descent = c_CourierBoldDescent;
                metrics.PostScriptName = c_CourierBoldName;
                metrics.Size = size;
                metrics.WidthTable = new StandardWidthTable(c_fixedWidth);
                metrics.Height = metrics.Ascent - metrics.Descent;
            }
            else if ((fontStyle & PdfFontStyle.Italic) > 0)
            {
                metrics.Ascent = c_CourierItalicAscent;
                metrics.Descent = c_CourierItalicDescent;
                metrics.PostScriptName = c_CourierItalicName;
                metrics.Size = size;
                metrics.WidthTable = new StandardWidthTable(c_fixedWidth);
                metrics.Height = metrics.Ascent - metrics.Descent;
            }
            else
            {
                metrics.Ascent = c_CourierAscent;
                metrics.Descent = c_CourierDescent;
                metrics.PostScriptName = c_CourierName;
                metrics.Size = size;
                metrics.WidthTable = new StandardWidthTable(c_fixedWidth);
                metrics.Height = metrics.Ascent - metrics.Descent;
            }

            return metrics;
        }

        /// <summary>
        /// Creates Times font metrics.
        /// </summary>
        /// <param name="fontFamily">FontFamily of the font.</param>
        /// <param name="fontStyle">Style of the font.</param>
        /// <param name="size">Size of the font.</param>
        /// <returns>Helvetica font metrics.</returns>
        private static PdfFontMetrics GetTimesMetrics(PdfFontFamily fontFamily, PdfFontStyle fontStyle, float size)
        {
            PdfFontMetrics metrics = new PdfFontMetrics();

            if ((fontStyle & PdfFontStyle.Bold) > 0 && (fontStyle & PdfFontStyle.Italic) > 0)
            {
                metrics.Ascent = c_TimesBoldItalicAscent;
                metrics.Descent = c_TimesBoldItalicDescent;
                metrics.PostScriptName = c_TimesBoldItalicName;
                metrics.Size = size;
                metrics.WidthTable = new StandardWidthTable(c_timesRomanBoldItalicWidth);
                metrics.Height = metrics.Ascent - metrics.Descent;
            }
            else if ((fontStyle & PdfFontStyle.Bold) > 0)
            {
                metrics.Ascent = c_TimesBoldAscent;
                metrics.Descent = c_TimesBoldDescent;
                metrics.PostScriptName = c_TimesBoldName;
                metrics.Size = size;
                metrics.WidthTable = new StandardWidthTable(c_timesRomanBoldWidth);
                metrics.Height = metrics.Ascent - metrics.Descent;
            }
            else if ((fontStyle & PdfFontStyle.Italic) > 0)
            {
                metrics.Ascent = c_TimesItalicAscent;
                metrics.Descent = c_TimesItalicDescent;
                metrics.PostScriptName = c_TimesItalicName;
                metrics.Size = size;
                metrics.WidthTable = new StandardWidthTable(c_timesRomanItalicWidth);
                metrics.Height = metrics.Ascent - metrics.Descent;
            }
            else
            {
                metrics.Ascent = c_TimesAscent;
                metrics.Descent = c_TimesDescent;
                metrics.PostScriptName = c_TimesName;
                metrics.Size = size;
                metrics.WidthTable = new StandardWidthTable(c_timesRomanWidth);
                metrics.Height = metrics.Ascent - metrics.Descent;
            }

            return metrics;
        }

        /// <summary>
        /// Creates Symbol font metrics.
        /// </summary>
        /// <param name="fontFamily">FontFamily of the font.</param>
        /// <param name="fontStyle">Style of the font.</param>
        /// <param name="size">Size of the font.</param>
        /// <returns>Helvetica font metrics.</returns>
        private static PdfFontMetrics GetSymbolMetrics(PdfFontFamily fontFamily, PdfFontStyle fontStyle, float size)
        {
            PdfFontMetrics metrics = new PdfFontMetrics();

            metrics.Ascent = c_symbolAscent;
            metrics.Descent = c_symbolDescent;
            metrics.PostScriptName = c_symbolName;
            metrics.Size = size;
            metrics.WidthTable = new StandardWidthTable(c_symbolWidth);
            metrics.Height = metrics.Ascent - metrics.Descent;

            return metrics;
        }

        /// <summary>
        /// Creates ZapfDingbats font metrics.
        /// </summary>
        /// <param name="fontFamily">FontFamily of the font.</param>
        /// <param name="fontStyle">Style of the font.</param>
        /// <param name="size">Size of the font.</param>
        /// <returns>Helvetica font metrics.</returns>
        private static PdfFontMetrics GetZapfDingbatsMetrics(PdfFontFamily fontFamily, PdfFontStyle fontStyle, float size)
        {
            PdfFontMetrics metrics = new PdfFontMetrics();

            metrics.Ascent = c_zapfDingbatsAscent;
            metrics.Descent = c_zapfDingbatsDescent;
            metrics.PostScriptName = c_zapfDingbatsName;
            metrics.Size = size;
            metrics.WidthTable = new StandardWidthTable(c_zapfDingbatsWidth);
            metrics.Height = metrics.Ascent - metrics.Descent;

            return metrics;
        }
        #endregion
    }
}

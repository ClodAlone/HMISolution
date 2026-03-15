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

namespace Syncfusion.DocIO.DLS
{
    internal class AutoShapeHelper
    {
        internal static Dictionary<string, int> dictionary;

        internal static Dictionary<string, string> shapeTypes = new Dictionary<string, string>();
        internal static AutoShapeConstant GetAutoShapeConstant(string string_4)
        {
            string key = string_4;
            if (key != null)
            {
                int num;
                if (AutoShapeHelper.dictionary == null)
                {
                    Dictionary<string, int> dictionary1 = new Dictionary<string, int>(187);
                    dictionary1.Add("line", 0);
                    dictionary1.Add("lineInv", 1);
                    dictionary1.Add("triangle", 2);
                    dictionary1.Add("rtTriangle", 3);
                    dictionary1.Add("rect", 4);
                    dictionary1.Add("diamond", 5);
                    dictionary1.Add("parallelogram", 6);
                    dictionary1.Add("trapezoid", 7);
                    dictionary1.Add("nonIsoscelesTrapezoid", 8);
                    dictionary1.Add("pentagon", 9);
                    dictionary1.Add("hexagon", 10);
                    dictionary1.Add("heptagon", 11);
                    dictionary1.Add("octagon", 12);
                    dictionary1.Add("decagon", 13);
                    dictionary1.Add("dodecagon", 14);
                    dictionary1.Add("star4", 15);
                    dictionary1.Add("star5", 16);
                    dictionary1.Add("star6", 17);
                    dictionary1.Add("star7", 18);
                    dictionary1.Add("star8", 19);
                    dictionary1.Add("star10", 20);
                    dictionary1.Add("star12", 21);
                    dictionary1.Add("star16", 22);
                    dictionary1.Add("star24", 23);
                    dictionary1.Add("star32", 24);
                    dictionary1.Add("roundRect", 25);
                    dictionary1.Add("round1Rect", 26);
                    dictionary1.Add("round2SameRect", 27);
                    dictionary1.Add("round2DiagRect", 28);
                    dictionary1.Add("snipRoundRect", 29);
                    dictionary1.Add("snip1Rect", 30);
                    dictionary1.Add("snip2SameRect", 31);
                    dictionary1.Add("snip2DiagRect", 32);
                    dictionary1.Add("plaque", 33);
                    dictionary1.Add("ellipse", 34);
                    dictionary1.Add("teardrop", 35);
                    dictionary1.Add("homePlate", 36);
                    dictionary1.Add("chevron", 37);
                    dictionary1.Add("pieWedge", 38);
                    dictionary1.Add("pie", 39);
                    dictionary1.Add("blockArc", 40);
                    dictionary1.Add("donut", 41);
                    dictionary1.Add("noSmoking", 42);
                    dictionary1.Add("rightArrow", 43);
                    dictionary1.Add("leftArrow", 44);
                    dictionary1.Add("upArrow", 45);
                    dictionary1.Add("downArrow", 46);
                    dictionary1.Add("stripedRightArrow", 47);
                    dictionary1.Add("notchedRightArrow", 48);
                    dictionary1.Add("bentUpArrow", 49);
                    dictionary1.Add("leftRightArrow", 50);
                    dictionary1.Add("upDownArrow", 51);
                    dictionary1.Add("leftUpArrow", 52);
                    dictionary1.Add("leftRightUpArrow", 53);
                    dictionary1.Add("quadArrow", 54);
                    dictionary1.Add("leftArrowCallout", 55);
                    dictionary1.Add("rightArrowCallout", 56);
                    dictionary1.Add("upArrowCallout", 57);
                    dictionary1.Add("downArrowCallout", 58);
                    dictionary1.Add("leftRightArrowCallout", 59);
                    dictionary1.Add("upDownArrowCallout", 60);
                    dictionary1.Add("quadArrowCallout", 61);
                    dictionary1.Add("bentArrow", 62);
                    dictionary1.Add("uturnArrow", 63);
                    dictionary1.Add("circularArrow", 64);
                    dictionary1.Add("leftCircularArrow", 65);
                    dictionary1.Add("leftRightCircularArrow", 66);
                    dictionary1.Add("curvedRightArrow", 67);
                    dictionary1.Add("curvedLeftArrow", 68);
                    dictionary1.Add("curvedUpArrow", 69);
                    dictionary1.Add("curvedDownArrow", 70);
                    dictionary1.Add("swooshArrow", 71);
                    dictionary1.Add("cube", 72);
                    dictionary1.Add("can", 73);
                    dictionary1.Add("lightningBolt", 74);
                    dictionary1.Add("heart", 75);
                    dictionary1.Add("sun", 76);
                    dictionary1.Add("moon", 77);
                    dictionary1.Add("smileyFace", 78);
                    dictionary1.Add("irregularSeal1", 79);
                    dictionary1.Add("irregularSeal2", 80);
                    dictionary1.Add("foldedCorner", 81);
                    dictionary1.Add("bevel", 82);
                    dictionary1.Add("frame", 83);
                    dictionary1.Add("halfFrame", 84);
                    dictionary1.Add("corner", 85);
                    dictionary1.Add("diagStripe", 86);
                    dictionary1.Add("chord", 87);
                    dictionary1.Add("arc", 88);
                    dictionary1.Add("leftBracket", 89);
                    dictionary1.Add("rightBracket", 90);
                    dictionary1.Add("leftBrace", 91);
                    dictionary1.Add("rightBrace", 92);
                    dictionary1.Add("bracketPair", 93);
                    dictionary1.Add("bracePair", 94);
                    dictionary1.Add("straightConnector1", 95);
                    dictionary1.Add("bentConnector2", 96);
                    dictionary1.Add("bentConnector3", 97);
                    dictionary1.Add("bentConnector4", 98);
                    dictionary1.Add("bentConnector5", 99);
                    dictionary1.Add("curvedConnector2", 100);
                    dictionary1.Add("curvedConnector3", 101);
                    dictionary1.Add("curvedConnector4", 102);
                    dictionary1.Add("curvedConnector5", 103);
                    dictionary1.Add("callout1", 104);
                    dictionary1.Add("callout2", 105);
                    dictionary1.Add("callout3", 106);
                    dictionary1.Add("accentCallout1", 107);
                    dictionary1.Add("accentCallout2", 108);
                    dictionary1.Add("accentCallout3", 109);
                    dictionary1.Add("borderCallout1", 110);
                    dictionary1.Add("borderCallout2", 111);
                    dictionary1.Add("borderCallout3", 112);
                    dictionary1.Add("accentBorderCallout1", 113);
                    dictionary1.Add("accentBorderCallout2", 114);
                    dictionary1.Add("accentBorderCallout3", 115);
                    dictionary1.Add("wedgeRectCallout", 116);
                    dictionary1.Add("wedgeRoundRectCallout", 117);
                    dictionary1.Add("wedgeEllipseCallout", 118);
                    dictionary1.Add("cloudCallout", 119);
                    dictionary1.Add("cloud", 120);
                    dictionary1.Add("ribbon", 121);
                    dictionary1.Add("ribbon2", 122);
                    dictionary1.Add("ellipseRibbon", 123);
                    dictionary1.Add("ellipseRibbon2", 124);
                    dictionary1.Add("leftRightRibbon", 125);
                    dictionary1.Add("verticalScroll", 126);
                    dictionary1.Add("horizontalScroll", 127);
                    dictionary1.Add("wave", 128);
                    dictionary1.Add("doubleWave", 129);
                    dictionary1.Add("plus", 130);
                    dictionary1.Add("flowChartProcess", 131);
                    dictionary1.Add("flowChartDecision", 132);
                    dictionary1.Add("flowChartInputOutput", 133);
                    dictionary1.Add("flowChartPredefinedProcess", 134);
                    dictionary1.Add("flowChartInternalStorage", 135);
                    dictionary1.Add("flowChartDocument", 136);
                    dictionary1.Add("flowChartMultidocument", 137);
                    dictionary1.Add("flowChartTerminator", 138);
                    dictionary1.Add("flowChartPreparation", 139);
                    dictionary1.Add("flowChartManualInput", 140);
                    dictionary1.Add("flowChartManualOperation", 141);
                    dictionary1.Add("flowChartConnector", 142);
                    dictionary1.Add("flowChartPunchedCard", 143);
                    dictionary1.Add("flowChartPunchedTape", 144);
                    dictionary1.Add("flowChartSummingJunction", 145);
                    dictionary1.Add("flowChartOr", 146);
                    dictionary1.Add("flowChartCollate", 147);
                    dictionary1.Add("flowChartSort", 148);
                    dictionary1.Add("flowChartExtract", 149);
                    dictionary1.Add("flowChartMerge", 150);
                    dictionary1.Add("flowChartOfflineStorage", 151);
                    dictionary1.Add("flowChartOnlineStorage", 152);
                    dictionary1.Add("flowChartMagneticTape", 153);
                    dictionary1.Add("flowChartMagneticDisk", 154);
                    dictionary1.Add("flowChartMagneticDrum", 155);
                    dictionary1.Add("flowChartDisplay", 156);
                    dictionary1.Add("flowChartDelay", 157);
                    dictionary1.Add("flowChartAlternateProcess", 158);
                    dictionary1.Add("flowChartOffpageConnector", 159);
                    dictionary1.Add("actionButtonBlank", 160);
                    dictionary1.Add("actionButtonHome", 161);
                    dictionary1.Add("actionButtonHelp", 162);
                    dictionary1.Add("actionButtonInformation", 163);
                    dictionary1.Add("actionButtonForwardNext", 164);
                    dictionary1.Add("actionButtonBackPrevious", 165);
                    dictionary1.Add("actionButtonEnd", 166);
                    dictionary1.Add("actionButtonBeginning", 167);
                    dictionary1.Add("actionButtonReturn", 168);
                    dictionary1.Add("actionButtonDocument", 169);
                    dictionary1.Add("actionButtonSound", 170);
                    dictionary1.Add("actionButtonMovie", 171);
                    dictionary1.Add("gear6", 172);
                    dictionary1.Add("gear9", 173);
                    dictionary1.Add("funnel", 174);
                    dictionary1.Add("mathPlus", 175);
                    dictionary1.Add("mathMinus", 176);
                    dictionary1.Add("mathMultiply", 177);
                    dictionary1.Add("mathDivide", 178);
                    dictionary1.Add("mathEqual", 179);
                    dictionary1.Add("mathNotEqual", 180);
                    dictionary1.Add("cornerTabs", 181);
                    dictionary1.Add("squareTabs", 182);
                    dictionary1.Add("plaqueTabs", 183);
                    dictionary1.Add("chartX", 184);
                    dictionary1.Add("chartStar", 185);
                    dictionary1.Add("chartPlus", 186);
                    AutoShapeHelper.dictionary = dictionary1;
                }
                if (AutoShapeHelper.dictionary.TryGetValue(key, out num))
                {
                    switch (num)
                    {
                        case 0:
                            return AutoShapeConstant.Index_0;
                        case 1:
                            return AutoShapeConstant.Index_1;
                        case 2:
                            return AutoShapeConstant.Index_2;
                        case 3:
                            return AutoShapeConstant.Index_3;
                        case 4:
                            return AutoShapeConstant.Index_4;
                        case 5:
                            return AutoShapeConstant.Index_5;
                        case 6:
                            return AutoShapeConstant.Index_6;
                        case 7:
                            return AutoShapeConstant.Index_7;
                        case 8:
                            return AutoShapeConstant.Index_8;
                        case 9:
                            return AutoShapeConstant.Index_9;
                        case 10:
                            return AutoShapeConstant.Index_10;
                        case 11:
                            return AutoShapeConstant.Index_11;
                        case 12:
                            return AutoShapeConstant.Index_12;
                        case 13:
                            return AutoShapeConstant.Index_13;
                        case 14:
                            return AutoShapeConstant.Index_14;
                        case 15:
                            return AutoShapeConstant.Index_15;
                        case 16:
                            return AutoShapeConstant.Index_16;
                        case 17:
                            return AutoShapeConstant.Index_17;
                        case 18:
                            return AutoShapeConstant.Index_18;
                        case 19:
                            return AutoShapeConstant.Index_19;
                        case 20:
                            return AutoShapeConstant.Index_20;
                        case 21:
                            return AutoShapeConstant.Index_21;
                        case 22:
                            return AutoShapeConstant.Index_22;
                        case 23:
                            return AutoShapeConstant.Index_23;
                        case 24:
                            return AutoShapeConstant.Index_24;
                        case 25:
                            return AutoShapeConstant.Index_25;
                        case 26:
                            return AutoShapeConstant.Index_26;
                        case 27:
                            return AutoShapeConstant.Index_27;
                        case 28:
                            return AutoShapeConstant.Index_28;
                        case 29:
                            return AutoShapeConstant.Index_29;
                        case 30:
                            return AutoShapeConstant.Index_30;
                        case 31:
                            return AutoShapeConstant.Index_31;
                        case 32:
                            return AutoShapeConstant.Index_32;
                        case 33:
                            return AutoShapeConstant.Index_33;
                        case 34:
                            return AutoShapeConstant.Index_34;
                        case 35:
                            return AutoShapeConstant.Index_35;
                        case 36:
                            return AutoShapeConstant.Index_36;
                        case 37:
                            return AutoShapeConstant.Index_37;
                        case 38:
                            return AutoShapeConstant.Index_38;
                        case 39:
                            return AutoShapeConstant.Index_39;
                        case 40:
                            return AutoShapeConstant.Index_40;
                        case 41:
                            return AutoShapeConstant.Index_41;
                        case 42:
                            return AutoShapeConstant.Index_42;
                        case 43:
                            return AutoShapeConstant.Index_43;
                        case 44:
                            return AutoShapeConstant.Index_44;
                        case 45:
                            return AutoShapeConstant.Index_45;
                        case 46:
                            return AutoShapeConstant.Index_46;
                        case 47:
                            return AutoShapeConstant.Index_47;
                        case 48:
                            return AutoShapeConstant.Index_48;
                        case 49:
                            return AutoShapeConstant.Index_49;
                        case 50:
                            return AutoShapeConstant.Index_50;
                        case 51:
                            return AutoShapeConstant.Index_51;
                        case 52:
                            return AutoShapeConstant.Index_52;
                        case 53:
                            return AutoShapeConstant.Index_53;
                        case 54:
                            return AutoShapeConstant.Index_54;
                        case 55:
                            return AutoShapeConstant.Index_55;
                        case 56:
                            return AutoShapeConstant.Index_56;
                        case 57:
                            return AutoShapeConstant.Index_57;
                        case 58:
                            return AutoShapeConstant.Index_58;
                        case 59:
                            return AutoShapeConstant.Index_59;
                        case 60:
                            return AutoShapeConstant.Index_60;
                        case 61:
                            return AutoShapeConstant.Index_61;
                        case 62:
                            return AutoShapeConstant.Index_62;
                        case 63:
                            return AutoShapeConstant.Index_63;
                        case 64:
                            return AutoShapeConstant.Index_64;
                        case 65:
                            return AutoShapeConstant.Index_65;
                        case 66:
                            return AutoShapeConstant.Index_66;
                        case 67:
                            return AutoShapeConstant.Index_67;
                        case 68:
                            return AutoShapeConstant.Index_68;
                        case 69:
                            return AutoShapeConstant.Index_69;
                        case 70:
                            return AutoShapeConstant.Index_70;
                        case 71:
                            return AutoShapeConstant.Index_71;
                        case 72:
                            return AutoShapeConstant.Index_72;
                        case 73:
                            return AutoShapeConstant.Index_73;
                        case 74:
                            return AutoShapeConstant.Index_74;
                        case 75:
                            return AutoShapeConstant.Index_75;
                        case 76:
                            return AutoShapeConstant.Index_76;
                        case 77:
                            return AutoShapeConstant.Index_77;
                        case 78:
                            return AutoShapeConstant.Index_78;
                        case 79:
                            return AutoShapeConstant.Index_79;
                        case 80:
                            return AutoShapeConstant.Index_80;
                        case 81:
                            return AutoShapeConstant.Index_81;
                        case 82:
                            return AutoShapeConstant.Index_82;
                        case 83:
                            return AutoShapeConstant.Index_83;
                        case 84:
                            return AutoShapeConstant.Index_84;
                        case 85:
                            return AutoShapeConstant.Index_85;
                        case 86:
                            return AutoShapeConstant.Index_86;
                        case 87:
                            return AutoShapeConstant.Index_87;
                        case 88:
                            return AutoShapeConstant.Index_88;
                        case 89:
                            return AutoShapeConstant.Index_89;
                        case 90:
                            return AutoShapeConstant.Index_90;
                        case 91:
                            return AutoShapeConstant.Index_91;
                        case 92:
                            return AutoShapeConstant.Index_92;
                        case 93:
                            return AutoShapeConstant.Index_93;
                        case 94:
                            return AutoShapeConstant.Index_94;
                        case 95:
                            return AutoShapeConstant.Index_95;
                        case 96:
                            return AutoShapeConstant.Index_96;
                        case 97:
                            return AutoShapeConstant.Index_97;
                        case 98:
                            return AutoShapeConstant.Index_98;
                        case 99:
                            return AutoShapeConstant.Index_99;
                        case 100:
                            return AutoShapeConstant.Index_100;
                        case 101:
                            return AutoShapeConstant.Index_101;
                        case 102:
                            return AutoShapeConstant.Index_102;
                        case 103:
                            return AutoShapeConstant.Index_103;
                        case 104:
                            return AutoShapeConstant.Index_104;
                        case 105:
                            return AutoShapeConstant.Index_105;
                        case 106:
                            return AutoShapeConstant.Index_106;
                        case 107:
                            return AutoShapeConstant.Index_107;
                        case 108:
                            return AutoShapeConstant.Index_108;
                        case 109:
                            return AutoShapeConstant.Index_109;
                        case 110:
                            return AutoShapeConstant.Index_110;
                        case 111:
                            return AutoShapeConstant.Index_111;
                        case 112:
                            return AutoShapeConstant.Index_112;
                        case 113:
                            return AutoShapeConstant.Index_113;
                        case 114:
                            return AutoShapeConstant.Index_114;
                        case 115:
                            return AutoShapeConstant.Index_115;
                        case 116:
                            return AutoShapeConstant.Index_116;
                        case 117:
                            return AutoShapeConstant.Index_117;
                        case 118:
                            return AutoShapeConstant.Index_118;
                        case 119:
                            return AutoShapeConstant.Index_119;
                        case 120:
                            return AutoShapeConstant.Index_120;
                        case 121:
                            return AutoShapeConstant.Index_121;
                        case 122:
                            return AutoShapeConstant.Index_122;
                        case 123:
                            return AutoShapeConstant.Index_123;
                        case 124:
                            return AutoShapeConstant.Index_124;
                        case 125:
                            return AutoShapeConstant.Index_125;
                        case 126:
                            return AutoShapeConstant.Index_126;
                        case 127:
                            return AutoShapeConstant.Index_127;
                        case 128:
                            return AutoShapeConstant.Index_128;
                        case 129:
                            return AutoShapeConstant.Index_129;
                        case 130:
                            return AutoShapeConstant.Index_130;
                        case 131:
                            return AutoShapeConstant.Index_131;
                        case 132:
                            return AutoShapeConstant.Index_132;
                        case 133:
                            return AutoShapeConstant.Index_133;
                        case 134:
                            return AutoShapeConstant.Index_134;
                        case 135:
                            return AutoShapeConstant.Index_135;
                        case 136:
                            return AutoShapeConstant.Index_136;
                        case 137:
                            return AutoShapeConstant.Index_137;
                        case 138:
                            return AutoShapeConstant.Index_138;
                        case 139:
                            return AutoShapeConstant.Index_139;
                        case 140:
                            return AutoShapeConstant.Index_140;
                        case 141:
                            return AutoShapeConstant.Index_141;
                        case 142:
                            return AutoShapeConstant.Index_142;
                        case 143:
                            return AutoShapeConstant.Index_143;
                        case 144:
                            return AutoShapeConstant.Index_144;
                        case 145:
                            return AutoShapeConstant.Index_145;
                        case 146:
                            return AutoShapeConstant.Index_146;
                        case 147:
                            return AutoShapeConstant.Index_147;
                        case 148:
                            return AutoShapeConstant.Index_148;
                        case 149:
                            return AutoShapeConstant.Index_149;
                        case 150:
                            return AutoShapeConstant.Index_150;
                        case 151:
                            return AutoShapeConstant.Index_151;
                        case 152:
                            return AutoShapeConstant.Index_152;
                        case 153:
                            return AutoShapeConstant.Index_153;
                        case 154:
                            return AutoShapeConstant.Index_154;
                        case 155:
                            return AutoShapeConstant.Index_155;
                        case 156:
                            return AutoShapeConstant.Index_156;
                        case 157:
                            return AutoShapeConstant.Index_157;
                        case 158:
                            return AutoShapeConstant.Index_158;
                        case 159:
                            return AutoShapeConstant.Index_159;
                        case 160:
                            return AutoShapeConstant.Index_160;
                        case 161:
                            return AutoShapeConstant.Index_161;
                        case 162:
                            return AutoShapeConstant.Index_162;
                        case 163:
                            return AutoShapeConstant.Index_163;
                        case 164:
                            return AutoShapeConstant.Index_164;
                        case 165:
                            return AutoShapeConstant.Index_165;
                        case 166:
                            return AutoShapeConstant.Index_166;
                        case 167:
                            return AutoShapeConstant.Index_167;
                        case 168:
                            return AutoShapeConstant.Index_168;
                        case 169:
                            return AutoShapeConstant.Index_169;
                        case 170:
                            return AutoShapeConstant.Index_170;
                        case 171:
                            return AutoShapeConstant.Index_171;
                        case 172:
                            return AutoShapeConstant.Index_172;
                        case 173:
                            return AutoShapeConstant.Index_173;
                        case 174:
                            return AutoShapeConstant.Index_174;
                        case 175:
                            return AutoShapeConstant.Index_175;
                        case 176:
                            return AutoShapeConstant.Index_176;
                        case 177:
                            return AutoShapeConstant.Index_177;
                        case 178:
                            return AutoShapeConstant.Index_178;
                        case 179:
                            return AutoShapeConstant.Index_179;
                        case 180:
                            return AutoShapeConstant.Index_180;
                        case 181:
                            return AutoShapeConstant.Index_181;
                        case 182:
                            return AutoShapeConstant.Index_182;
                        case 183:
                            return AutoShapeConstant.Index_183;
                        case 184:
                            return AutoShapeConstant.Index_184;
                        case 185:
                            return AutoShapeConstant.Index_185;
                        case 186:
                            return AutoShapeConstant.Index_186;

                    }
                }
            }
            return AutoShapeConstant.Index_187;

        }

        internal static AutoShapeType GetAutoShapeType(AutoShapeConstant enum184_0)
        {
            switch (enum184_0)
            {
                case AutoShapeConstant.Index_0:
                    return AutoShapeType.Line;

                case AutoShapeConstant.Index_2:
                    return AutoShapeType.IsoscelesTriangle;

                case AutoShapeConstant.Index_3:
                    return AutoShapeType.RightTriangle;

                case AutoShapeConstant.Index_4:
                    return AutoShapeType.Rectangle;

                case AutoShapeConstant.Index_5:
                    return AutoShapeType.Diamond;

                case AutoShapeConstant.Index_6:
                    return AutoShapeType.Parallelogram;

                case AutoShapeConstant.Index_7:
                    return AutoShapeType.Trapezoid;

                case AutoShapeConstant.Index_9:
                    return AutoShapeType.RegularPentagon;

                case AutoShapeConstant.Index_10:
                    return AutoShapeType.Hexagon;

                case AutoShapeConstant.Index_11:
                    return AutoShapeType.Heptagon;

                case AutoShapeConstant.Index_12:
                    return AutoShapeType.Octagon;

                case AutoShapeConstant.Index_13:
                    return AutoShapeType.Decagon;

                case AutoShapeConstant.Index_14:
                    return AutoShapeType.Dodecagon;

                case AutoShapeConstant.Index_15:
                    return AutoShapeType.Star4Point;

                case AutoShapeConstant.Index_16:
                    return AutoShapeType.Star5Point;

                case AutoShapeConstant.Index_17:
                    return AutoShapeType.Star6Point;

                case AutoShapeConstant.Index_18:
                    return AutoShapeType.Star7Point;

                case AutoShapeConstant.Index_19:
                    return AutoShapeType.Star8Point;

                case AutoShapeConstant.Index_20:
                    return AutoShapeType.Star10Point;

                case AutoShapeConstant.Index_21:
                    return AutoShapeType.Star12Point;

                case AutoShapeConstant.Index_22:
                    return AutoShapeType.Star16Point;

                case AutoShapeConstant.Index_23:
                    return AutoShapeType.Star24Point;

                case AutoShapeConstant.Index_24:
                    return AutoShapeType.Star32Point;

                case AutoShapeConstant.Index_25:
                    return AutoShapeType.RoundedRectangle;

                case AutoShapeConstant.Index_26:
                    return AutoShapeType.RoundSingleCornerRectangle;

                case AutoShapeConstant.Index_27:
                    return AutoShapeType.RoundSameSideCornerRectangle;

                case AutoShapeConstant.Index_28:
                    return AutoShapeType.RoundDiagonalCornerRectangle;

                case AutoShapeConstant.Index_29:
                    return AutoShapeType.SnipAndRoundSingleCornerRectangle;

                case AutoShapeConstant.Index_30:
                    return AutoShapeType.SnipSingleCornerRectangle;

                case AutoShapeConstant.Index_31:
                    return AutoShapeType.SnipSameSideCornerRectangle;

                case AutoShapeConstant.Index_32:
                    return AutoShapeType.SnipDiagonalCornerRectangle;

                case AutoShapeConstant.Index_33:
                    return AutoShapeType.Plaque;

                case AutoShapeConstant.Index_34:
                    return AutoShapeType.Oval;

                case AutoShapeConstant.Index_35:
                    return AutoShapeType.Teardrop;

                case AutoShapeConstant.Index_36:
                    return AutoShapeType.Pentagon;

                case AutoShapeConstant.Index_37:
                    return AutoShapeType.Chevron;

                case AutoShapeConstant.Index_39:
                    return AutoShapeType.Pie;

                case AutoShapeConstant.Index_40:
                    return AutoShapeType.BlockArc;

                case AutoShapeConstant.Index_41:
                    return AutoShapeType.Donut;

                case AutoShapeConstant.Index_42:
                    return AutoShapeType.NoSymbol;

                case AutoShapeConstant.Index_43:
                    return AutoShapeType.RightArrow;

                case AutoShapeConstant.Index_44:
                    return AutoShapeType.LeftArrow;

                case AutoShapeConstant.Index_45:
                    return AutoShapeType.UpArrow;

                case AutoShapeConstant.Index_46:
                    return AutoShapeType.DownArrow;

                case AutoShapeConstant.Index_47:
                    return AutoShapeType.StripedRightArrow;

                case AutoShapeConstant.Index_48:
                    return AutoShapeType.NotchedRightArrow;

                case AutoShapeConstant.Index_49:
                    return AutoShapeType.BentUpArrow;

                case AutoShapeConstant.Index_50:
                    return AutoShapeType.LeftRightArrow;

                case AutoShapeConstant.Index_51:
                    return AutoShapeType.UpDownArrow;

                case AutoShapeConstant.Index_52:
                    return AutoShapeType.LeftUpArrow;

                case AutoShapeConstant.Index_53:
                    return AutoShapeType.LeftRightUpArrow;

                case AutoShapeConstant.Index_54:
                    return AutoShapeType.QuadArrow;

                case AutoShapeConstant.Index_55:
                    return AutoShapeType.LeftArrowCallout;

                case AutoShapeConstant.Index_56:
                    return AutoShapeType.RightArrowCallout;

                case AutoShapeConstant.Index_57:
                    return AutoShapeType.UpArrowCallout;

                case AutoShapeConstant.Index_58:
                    return AutoShapeType.DownArrowCallout;

                case AutoShapeConstant.Index_59:
                    return AutoShapeType.LeftRightArrowCallout;

                case AutoShapeConstant.Index_60:
                    return AutoShapeType.UpDownArrowCallout;

                case AutoShapeConstant.Index_61:
                    return AutoShapeType.QuadArrowCallout;

                case AutoShapeConstant.Index_62:
                    return AutoShapeType.BentArrow;

                case AutoShapeConstant.Index_63:
                    return AutoShapeType.UTurnArrow;

                case AutoShapeConstant.Index_64:
                    return AutoShapeType.CircularArrow;

                case AutoShapeConstant.Index_67:
                    return AutoShapeType.CurvedRightArrow;

                case AutoShapeConstant.Index_68:
                    return AutoShapeType.CurvedLeftArrow;

                case AutoShapeConstant.Index_69:
                    return AutoShapeType.CurvedUpArrow;

                case AutoShapeConstant.Index_70:
                    return AutoShapeType.CurvedDownArrow;

                case AutoShapeConstant.Index_72:
                    return AutoShapeType.Cube;

                case AutoShapeConstant.Index_73:
                    return AutoShapeType.Can;

                case AutoShapeConstant.Index_74:
                    return AutoShapeType.LightningBolt;

                case AutoShapeConstant.Index_75:
                    return AutoShapeType.Heart;

                case AutoShapeConstant.Index_76:
                    return AutoShapeType.Sun;

                case AutoShapeConstant.Index_77:
                    return AutoShapeType.Moon;

                case AutoShapeConstant.Index_78:
                    return AutoShapeType.SmileyFace;

                case AutoShapeConstant.Index_79:
                    return AutoShapeType.Explosion1;

                case AutoShapeConstant.Index_80:
                    return AutoShapeType.Explosion2;

                case AutoShapeConstant.Index_81:
                    return AutoShapeType.FoldedCorner;

                case AutoShapeConstant.Index_82:
                    return AutoShapeType.Bevel;

                case AutoShapeConstant.Index_83:
                    return AutoShapeType.Frame;

                case AutoShapeConstant.Index_84:
                    return AutoShapeType.HalfFrame;

                case AutoShapeConstant.Index_85:
                    return AutoShapeType.L_Shape;

                case AutoShapeConstant.Index_86:
                    return AutoShapeType.DiagonalStripe;

                case AutoShapeConstant.Index_87:
                    return AutoShapeType.Chord;

                case AutoShapeConstant.Index_88:
                    return AutoShapeType.Arc;

                case AutoShapeConstant.Index_89:
                    return AutoShapeType.LeftBracket;

                case AutoShapeConstant.Index_90:
                    return AutoShapeType.RightBracket;

                case AutoShapeConstant.Index_91:
                    return AutoShapeType.LeftBrace;

                case AutoShapeConstant.Index_92:
                    return AutoShapeType.RightBrace;

                case AutoShapeConstant.Index_93:
                    return AutoShapeType.DoubleBracket;

                case AutoShapeConstant.Index_94:
                    return AutoShapeType.DoubleBrace;

                case AutoShapeConstant.Index_95:
                    return AutoShapeType.StraightConnector;

                case AutoShapeConstant.Index_96:
                    return AutoShapeType.BentConnector2;

                case AutoShapeConstant.Index_97:
                    return AutoShapeType.ElbowConnector;

                case AutoShapeConstant.Index_98:
                    return AutoShapeType.BentConnector4;

                case AutoShapeConstant.Index_99:
                    return AutoShapeType.BentConnector5;

                case AutoShapeConstant.Index_100:
                    return AutoShapeType.CurvedConnector2;

                case AutoShapeConstant.Index_101:
                    return AutoShapeType.CurvedConnector;

                case AutoShapeConstant.Index_102:
                    return AutoShapeType.CurvedConnector4;

                case AutoShapeConstant.Index_103:
                    return AutoShapeType.CurvedConnector5;

                case AutoShapeConstant.Index_104:
                    return AutoShapeType.LineCallout1NoBorder;

                case AutoShapeConstant.Index_105:
                    return AutoShapeType.LineCallout2NoBorder;

                case AutoShapeConstant.Index_106:
                    return AutoShapeType.LineCallout3NoBorder;

                case AutoShapeConstant.Index_107:
                    return AutoShapeType.LineCallout1AccentBar;

                case AutoShapeConstant.Index_108:
                    return AutoShapeType.LineCallout2AccentBar;

                case AutoShapeConstant.Index_109:
                    return AutoShapeType.LineCallout3AccentBar;

                case AutoShapeConstant.Index_110:
                    return AutoShapeType.LineCallout1;

                case AutoShapeConstant.Index_111:
                    return AutoShapeType.LineCallout2;

                case AutoShapeConstant.Index_112:
                    return AutoShapeType.LineCallout3;

                case AutoShapeConstant.Index_113:
                    return AutoShapeType.LineCallout1BorderAndAccentBar;

                case AutoShapeConstant.Index_114:
                    return AutoShapeType.LineCallout2BorderAndAccentBar;

                case AutoShapeConstant.Index_115:
                    return AutoShapeType.LineCallout3BorderAndAccentBar;

                case AutoShapeConstant.Index_116:
                    return AutoShapeType.RectangularCallout;

                case AutoShapeConstant.Index_117:
                    return AutoShapeType.RoundedRectangularCallout;

                case AutoShapeConstant.Index_118:
                    return AutoShapeType.OvalCallout;

                case AutoShapeConstant.Index_119:
                    return AutoShapeType.CloudCallout;

                case AutoShapeConstant.Index_120:
                    return AutoShapeType.Cloud;

                case AutoShapeConstant.Index_121:
                    return AutoShapeType.DownRibbon;

                case AutoShapeConstant.Index_122:
                    return AutoShapeType.UpRibbon;

                case AutoShapeConstant.Index_123:
                    return AutoShapeType.CurvedDownRibbon;

                case AutoShapeConstant.Index_124:
                    return AutoShapeType.CurvedUpRibbon;

                case AutoShapeConstant.Index_126:
                    return AutoShapeType.VerticalScroll;

                case AutoShapeConstant.Index_127:
                    return AutoShapeType.HorizontalScroll;

                case AutoShapeConstant.Index_128:
                    return AutoShapeType.Wave;

                case AutoShapeConstant.Index_129:
                    return AutoShapeType.DoubleWave;

                case AutoShapeConstant.Index_130:
                    return AutoShapeType.Cross;

                case AutoShapeConstant.Index_131:
                    return AutoShapeType.FlowChartProcess;

                case AutoShapeConstant.Index_132:
                    return AutoShapeType.FlowChartDecision;

                case AutoShapeConstant.Index_133:
                    return AutoShapeType.FlowChartData;

                case AutoShapeConstant.Index_134:
                    return AutoShapeType.FlowChartPredefinedProcess;

                case AutoShapeConstant.Index_135:
                    return AutoShapeType.FlowChartInternalStorage;

                case AutoShapeConstant.Index_136:
                    return AutoShapeType.FlowChartDocument;

                case AutoShapeConstant.Index_137:
                    return AutoShapeType.FlowChartMultiDocument;

                case AutoShapeConstant.Index_138:
                    return AutoShapeType.FlowChartTerminator;

                case AutoShapeConstant.Index_139:
                    return AutoShapeType.FlowChartPreparation;

                case AutoShapeConstant.Index_140:
                    return AutoShapeType.FlowChartManualInput;

                case AutoShapeConstant.Index_141:
                    return AutoShapeType.FlowChartManualOperation;

                case AutoShapeConstant.Index_142:
                    return AutoShapeType.FlowChartConnector;

                case AutoShapeConstant.Index_143:
                    return AutoShapeType.FlowChartCard;

                case AutoShapeConstant.Index_144:
                    return AutoShapeType.FlowChartPunchedTape;

                case AutoShapeConstant.Index_145:
                    return AutoShapeType.FlowChartSummingJunction;

                case AutoShapeConstant.Index_146:
                    return AutoShapeType.FlowChartOr;

                case AutoShapeConstant.Index_147:
                    return AutoShapeType.FlowChartCollate;

                case AutoShapeConstant.Index_148:
                    return AutoShapeType.FlowChartSort;

                case AutoShapeConstant.Index_149:
                    return AutoShapeType.FlowChartExtract;

                case AutoShapeConstant.Index_150:
                    return AutoShapeType.FlowChartMerge;

                case AutoShapeConstant.Index_152:
                    return AutoShapeType.FlowChartStoredData;

                case AutoShapeConstant.Index_153:
                    return AutoShapeType.FlowChartSequentialAccessStorage;

                case AutoShapeConstant.Index_154:
                    return AutoShapeType.FlowChartMagneticDisk;

                case AutoShapeConstant.Index_155:
                    return AutoShapeType.FlowChartDirectAccessStorage;

                case AutoShapeConstant.Index_156:
                    return AutoShapeType.FlowChartDisplay;

                case AutoShapeConstant.Index_157:
                    return AutoShapeType.FlowChartDelay;

                case AutoShapeConstant.Index_158:
                    return AutoShapeType.FlowChartAlternateProcess;

                case AutoShapeConstant.Index_159:
                    return AutoShapeType.FlowChartOffPageConnector;

                case AutoShapeConstant.Index_175:
                    return AutoShapeType.MathPlus;

                case AutoShapeConstant.Index_176:
                    return AutoShapeType.MathMinus;

                case AutoShapeConstant.Index_177:
                    return AutoShapeType.MathMultiply;

                case AutoShapeConstant.Index_178:
                    return AutoShapeType.MathDivision;

                case AutoShapeConstant.Index_179:
                    return AutoShapeType.MathEqual;

                case AutoShapeConstant.Index_180:
                    return AutoShapeType.MathNotEqual;
            }
            return AutoShapeType.Unknown;
        }

        internal static AutoShapeConstant GetAutoShapeConstant(AutoShapeType autoShapeType_0)
        {
            switch (autoShapeType_0)
            {
                case AutoShapeType.Rectangle:
                    return AutoShapeConstant.Index_4;

                case AutoShapeType.RoundedRectangle:
                    return AutoShapeConstant.Index_25;

                case AutoShapeType.Oval:
                    return AutoShapeConstant.Index_34;

                case AutoShapeType.Diamond:
                    return AutoShapeConstant.Index_5;

                case AutoShapeType.IsoscelesTriangle:
                    return AutoShapeConstant.Index_2;

                case AutoShapeType.RightTriangle:
                    return AutoShapeConstant.Index_3;

                case AutoShapeType.Parallelogram:
                    return AutoShapeConstant.Index_6;

                case AutoShapeType.Trapezoid:
                    return AutoShapeConstant.Index_7;

                case AutoShapeType.Hexagon:
                    return AutoShapeConstant.Index_10;

                case AutoShapeType.Octagon:
                    return AutoShapeConstant.Index_12;

                case AutoShapeType.Cross:
                    return AutoShapeConstant.Index_130;

                case AutoShapeType.Star5Point:
                    return AutoShapeConstant.Index_16;

                case AutoShapeType.RightArrow:
                    return AutoShapeConstant.Index_43;

                case AutoShapeType.Pentagon:
                    return AutoShapeConstant.Index_36;

                case AutoShapeType.Cube:
                    return AutoShapeConstant.Index_72;

                case AutoShapeType.Arc:
                    return AutoShapeConstant.Index_88;

                case AutoShapeType.Line:
                    return AutoShapeConstant.Index_0;

                case AutoShapeType.Plaque:
                    return AutoShapeConstant.Index_33;

                case AutoShapeType.Can:
                    return AutoShapeConstant.Index_73;

                case AutoShapeType.Donut:
                    return AutoShapeConstant.Index_41;

                case AutoShapeType.StraightConnector:
                    return AutoShapeConstant.Index_95;

                case AutoShapeType.BentConnector2:
                    return AutoShapeConstant.Index_96;

                case AutoShapeType.ElbowConnector:
                    return AutoShapeConstant.Index_97;

                case AutoShapeType.BentConnector4:
                    return AutoShapeConstant.Index_98;

                case AutoShapeType.BentConnector5:
                    return AutoShapeConstant.Index_99;

                case AutoShapeType.CurvedConnector2:
                    return AutoShapeConstant.Index_100;

                case AutoShapeType.CurvedConnector:
                    return AutoShapeConstant.Index_101;

                case AutoShapeType.CurvedConnector4:
                    return AutoShapeConstant.Index_102;

                case AutoShapeType.CurvedConnector5:
                    return AutoShapeConstant.Index_103;

                case AutoShapeType.LineCallout1:
                    return AutoShapeConstant.Index_110;

                case AutoShapeType.LineCallout2:
                    return AutoShapeConstant.Index_111;

                case AutoShapeType.LineCallout3:
                    return AutoShapeConstant.Index_112;

                case AutoShapeType.LineCallout1AccentBar:
                    return AutoShapeConstant.Index_107;

                case AutoShapeType.LineCallout2AccentBar:
                    return AutoShapeConstant.Index_108;

                case AutoShapeType.LineCallout3AccentBar:
                    return AutoShapeConstant.Index_109;

                case AutoShapeType.LineCallout1NoBorder:
                    return AutoShapeConstant.Index_104;

                case AutoShapeType.LineCallout2NoBorder:
                    return AutoShapeConstant.Index_105;

                case AutoShapeType.LineCallout3NoBorder:
                    return AutoShapeConstant.Index_106;

                case AutoShapeType.LineCallout1BorderAndAccentBar:
                    return AutoShapeConstant.Index_113;

                case AutoShapeType.LineCallout2BorderAndAccentBar:
                    return AutoShapeConstant.Index_114;

                case AutoShapeType.LineCallout3BorderAndAccentBar:
                    return AutoShapeConstant.Index_115;

                case AutoShapeType.DownRibbon:
                    return AutoShapeConstant.Index_121;

                case AutoShapeType.UpRibbon:
                    return AutoShapeConstant.Index_122;

                case AutoShapeType.Chevron:
                    return AutoShapeConstant.Index_37;

                case AutoShapeType.RegularPentagon:
                    return AutoShapeConstant.Index_9;

                case AutoShapeType.NoSymbol:
                    return AutoShapeConstant.Index_42;

                case AutoShapeType.Star8Point:
                    return AutoShapeConstant.Index_19;

                case AutoShapeType.Star16Point:
                    return AutoShapeConstant.Index_22;

                case AutoShapeType.Star32Point:
                    return AutoShapeConstant.Index_24;

                case AutoShapeType.RectangularCallout:
                    return AutoShapeConstant.Index_116;

                case AutoShapeType.RoundedRectangularCallout:
                    return AutoShapeConstant.Index_117;

                case AutoShapeType.OvalCallout:
                    return AutoShapeConstant.Index_118;

                case AutoShapeType.Wave:
                    return AutoShapeConstant.Index_128;

                case AutoShapeType.FoldedCorner:
                    return AutoShapeConstant.Index_81;

                case AutoShapeType.LeftArrow:
                    return AutoShapeConstant.Index_44;

                case AutoShapeType.DownArrow:
                    return AutoShapeConstant.Index_46;

                case AutoShapeType.UpArrow:
                    return AutoShapeConstant.Index_45;

                case AutoShapeType.LeftRightArrow:
                    return AutoShapeConstant.Index_50;

                case AutoShapeType.UpDownArrow:
                    return AutoShapeConstant.Index_51;

                case AutoShapeType.Explosion1:
                    return AutoShapeConstant.Index_79;

                case AutoShapeType.Explosion2:
                    return AutoShapeConstant.Index_80;

                case AutoShapeType.LightningBolt:
                    return AutoShapeConstant.Index_74;

                case AutoShapeType.Heart:
                    return AutoShapeConstant.Index_75;

                case AutoShapeType.QuadArrow:
                    return AutoShapeConstant.Index_54;

                case AutoShapeType.LeftArrowCallout:
                    return AutoShapeConstant.Index_55;

                case AutoShapeType.RightArrowCallout:
                    return AutoShapeConstant.Index_56;

                case AutoShapeType.UpArrowCallout:
                    return AutoShapeConstant.Index_57;

                case AutoShapeType.DownArrowCallout:
                    return AutoShapeConstant.Index_58;

                case AutoShapeType.LeftRightArrowCallout:
                    return AutoShapeConstant.Index_59;

                case AutoShapeType.UpDownArrowCallout:
                    return AutoShapeConstant.Index_60;

                case AutoShapeType.QuadArrowCallout:
                    return AutoShapeConstant.Index_61;

                case AutoShapeType.Bevel:
                    return AutoShapeConstant.Index_82;

                case AutoShapeType.LeftBracket:
                    return AutoShapeConstant.Index_89;

                case AutoShapeType.RightBracket:
                    return AutoShapeConstant.Index_90;

                case AutoShapeType.LeftBrace:
                    return AutoShapeConstant.Index_91;

                case AutoShapeType.RightBrace:
                    return AutoShapeConstant.Index_92;

                case AutoShapeType.LeftUpArrow:
                    return AutoShapeConstant.Index_52;

                case AutoShapeType.BentUpArrow:
                    return AutoShapeConstant.Index_49;

                case AutoShapeType.BentArrow:
                    return AutoShapeConstant.Index_62;

                case AutoShapeType.Star24Point:
                    return AutoShapeConstant.Index_23;

                case AutoShapeType.StripedRightArrow:
                    return AutoShapeConstant.Index_47;

                case AutoShapeType.NotchedRightArrow:
                    return AutoShapeConstant.Index_48;

                case AutoShapeType.BlockArc:
                    return AutoShapeConstant.Index_40;

                case AutoShapeType.SmileyFace:
                    return AutoShapeConstant.Index_78;

                case AutoShapeType.VerticalScroll:
                    return AutoShapeConstant.Index_126;

                case AutoShapeType.HorizontalScroll:
                    return AutoShapeConstant.Index_127;

                case AutoShapeType.CircularArrow:
                    return AutoShapeConstant.Index_64;

                case AutoShapeType.UTurnArrow:
                    return AutoShapeConstant.Index_63;

                case AutoShapeType.CurvedRightArrow:
                    return AutoShapeConstant.Index_67;

                case AutoShapeType.CurvedLeftArrow:
                    return AutoShapeConstant.Index_68;

                case AutoShapeType.CurvedUpArrow:
                    return AutoShapeConstant.Index_69;

                case AutoShapeType.CurvedDownArrow:
                    return AutoShapeConstant.Index_70;

                case AutoShapeType.CloudCallout:
                    return AutoShapeConstant.Index_119;

                case AutoShapeType.CurvedDownRibbon:
                    return AutoShapeConstant.Index_123;

                case AutoShapeType.CurvedUpRibbon:
                    return AutoShapeConstant.Index_124;

                case AutoShapeType.FlowChartProcess:
                    return AutoShapeConstant.Index_131;

                case AutoShapeType.FlowChartDecision:
                    return AutoShapeConstant.Index_132;

                case AutoShapeType.FlowChartData:
                    return AutoShapeConstant.Index_133;

                case AutoShapeType.FlowChartPredefinedProcess:
                    return AutoShapeConstant.Index_134;

                case AutoShapeType.FlowChartInternalStorage:
                    return AutoShapeConstant.Index_135;

                case AutoShapeType.FlowChartDocument:
                    return AutoShapeConstant.Index_136;

                case AutoShapeType.FlowChartMultiDocument:
                    return AutoShapeConstant.Index_137;

                case AutoShapeType.FlowChartTerminator:
                    return AutoShapeConstant.Index_138;

                case AutoShapeType.FlowChartPreparation:
                    return AutoShapeConstant.Index_139;

                case AutoShapeType.FlowChartManualInput:
                    return AutoShapeConstant.Index_140;

                case AutoShapeType.FlowChartManualOperation:
                    return AutoShapeConstant.Index_141;

                case AutoShapeType.FlowChartConnector:
                    return AutoShapeConstant.Index_142;

                case AutoShapeType.FlowChartCard:
                    return AutoShapeConstant.Index_143;

                case AutoShapeType.FlowChartPunchedTape:
                    return AutoShapeConstant.Index_144;

                case AutoShapeType.FlowChartSummingJunction:
                    return AutoShapeConstant.Index_145;

                case AutoShapeType.FlowChartOr:
                    return AutoShapeConstant.Index_146;

                case AutoShapeType.FlowChartCollate:
                    return AutoShapeConstant.Index_147;

                case AutoShapeType.FlowChartSort:
                    return AutoShapeConstant.Index_148;

                case AutoShapeType.FlowChartExtract:
                    return AutoShapeConstant.Index_149;

                case AutoShapeType.FlowChartMerge:
                    return AutoShapeConstant.Index_150;

                case AutoShapeType.FlowChartStoredData:
                    return AutoShapeConstant.Index_152;

                case AutoShapeType.FlowChartSequentialAccessStorage:
                    return AutoShapeConstant.Index_153;

                case AutoShapeType.FlowChartMagneticDisk:
                    return AutoShapeConstant.Index_154;

                case AutoShapeType.FlowChartDirectAccessStorage:
                    return AutoShapeConstant.Index_155;

                case AutoShapeType.FlowChartDisplay:
                    return AutoShapeConstant.Index_156;

                case AutoShapeType.FlowChartDelay:
                    return AutoShapeConstant.Index_157;

                case AutoShapeType.FlowChartAlternateProcess:
                    return AutoShapeConstant.Index_158;

                case AutoShapeType.FlowChartOffPageConnector:
                    return AutoShapeConstant.Index_159;

                case AutoShapeType.LeftRightUpArrow:
                    return AutoShapeConstant.Index_53;

                case AutoShapeType.Sun:
                    return AutoShapeConstant.Index_76;

                case AutoShapeType.Moon:
                    return AutoShapeConstant.Index_77;

                case AutoShapeType.DoubleBracket:
                    return AutoShapeConstant.Index_93;

                case AutoShapeType.DoubleBrace:
                    return AutoShapeConstant.Index_94;

                case AutoShapeType.Star4Point:
                    return AutoShapeConstant.Index_15;

                case AutoShapeType.DoubleWave:
                    return AutoShapeConstant.Index_129;

                case AutoShapeType.Heptagon:
                    return AutoShapeConstant.Index_11;

                case AutoShapeType.Decagon:
                    return AutoShapeConstant.Index_13;

                case AutoShapeType.Dodecagon:
                    return AutoShapeConstant.Index_14;

                case AutoShapeType.Star6Point:
                    return AutoShapeConstant.Index_17;

                case AutoShapeType.Star7Point:
                    return AutoShapeConstant.Index_18;

                case AutoShapeType.Star10Point:
                    return AutoShapeConstant.Index_20;

                case AutoShapeType.Star12Point:
                    return AutoShapeConstant.Index_21;

                case AutoShapeType.RoundSingleCornerRectangle:
                    return AutoShapeConstant.Index_26;

                case AutoShapeType.RoundSameSideCornerRectangle:
                    return AutoShapeConstant.Index_27;

                case AutoShapeType.RoundDiagonalCornerRectangle:
                    return AutoShapeConstant.Index_28;

                case AutoShapeType.SnipAndRoundSingleCornerRectangle:
                    return AutoShapeConstant.Index_29;

                case AutoShapeType.SnipSingleCornerRectangle:
                    return AutoShapeConstant.Index_30;

                case AutoShapeType.SnipSameSideCornerRectangle:
                    return AutoShapeConstant.Index_31;

                case AutoShapeType.SnipDiagonalCornerRectangle:
                    return AutoShapeConstant.Index_32;

                case AutoShapeType.Teardrop:
                    return AutoShapeConstant.Index_35;

                case AutoShapeType.Pie:
                    return AutoShapeConstant.Index_39;

                case AutoShapeType.Frame:
                    return AutoShapeConstant.Index_83;

                case AutoShapeType.HalfFrame:
                    return AutoShapeConstant.Index_84;

                case AutoShapeType.L_Shape:
                    return AutoShapeConstant.Index_85;

                case AutoShapeType.DiagonalStripe:
                    return AutoShapeConstant.Index_86;

                case AutoShapeType.Chord:
                    return AutoShapeConstant.Index_87;

                case AutoShapeType.Cloud:
                    return AutoShapeConstant.Index_120;

                case AutoShapeType.MathPlus:
                    return AutoShapeConstant.Index_175;

                case AutoShapeType.MathMinus:
                    return AutoShapeConstant.Index_176;

                case AutoShapeType.MathMultiply:
                    return AutoShapeConstant.Index_177;

                case AutoShapeType.MathDivision:
                    return AutoShapeConstant.Index_178;

                case AutoShapeType.MathEqual:
                    return AutoShapeConstant.Index_179;

                case AutoShapeType.MathNotEqual:
                    return AutoShapeConstant.Index_180;
            }
            return AutoShapeConstant.Index_187;
        }

        internal static string GetAutoShapeString(AutoShapeConstant enum184_0)
        {
            switch (enum184_0)
            {
                case AutoShapeConstant.Index_0:
                    return "line";

                case AutoShapeConstant.Index_1:
                    return "lineInv";

                case AutoShapeConstant.Index_2:
                    return "triangle";

                case AutoShapeConstant.Index_3:
                    return "rtTriangle";

                case AutoShapeConstant.Index_4:
                    return "rect";

                case AutoShapeConstant.Index_5:
                    return "diamond";

                case AutoShapeConstant.Index_6:
                    return "parallelogram";

                case AutoShapeConstant.Index_7:
                    return "trapezoid";

                case AutoShapeConstant.Index_8:
                    return "nonIsoscelesTrapezoid";

                case AutoShapeConstant.Index_9:
                    return "pentagon";

                case AutoShapeConstant.Index_10:
                    return "hexagon";

                case AutoShapeConstant.Index_11:
                    return "heptagon";

                case AutoShapeConstant.Index_12:
                    return "octagon";

                case AutoShapeConstant.Index_13:
                    return "decagon";

                case AutoShapeConstant.Index_14:
                    return "dodecagon";

                case AutoShapeConstant.Index_15:
                    return "star4";

                case AutoShapeConstant.Index_16:
                    return "star5";

                case AutoShapeConstant.Index_17:
                    return "star6";

                case AutoShapeConstant.Index_18:
                    return "star7";

                case AutoShapeConstant.Index_19:
                    return "star8";

                case AutoShapeConstant.Index_20:
                    return "star10";

                case AutoShapeConstant.Index_21:
                    return "star12";

                case AutoShapeConstant.Index_22:
                    return "star16";

                case AutoShapeConstant.Index_23:
                    return "star24";

                case AutoShapeConstant.Index_24:
                    return "star32";

                case AutoShapeConstant.Index_25:
                    return "roundRect";

                case AutoShapeConstant.Index_26:
                    return "round1Rect";

                case AutoShapeConstant.Index_27:
                    return "round2SameRect";

                case AutoShapeConstant.Index_28:
                    return "round2DiagRect";

                case AutoShapeConstant.Index_29:
                    return "snipRoundRect";

                case AutoShapeConstant.Index_30:
                    return "snip1Rect";

                case AutoShapeConstant.Index_31:
                    return "snip2SameRect";

                case AutoShapeConstant.Index_32:
                    return "snip2DiagRect";

                case AutoShapeConstant.Index_33:
                    return "plaque";

                case AutoShapeConstant.Index_34:
                    return "ellipse";

                case AutoShapeConstant.Index_35:
                    return "teardrop";

                case AutoShapeConstant.Index_36:
                    return "homePlate";

                case AutoShapeConstant.Index_37:
                    return "chevron";

                case AutoShapeConstant.Index_38:
                    return "pieWedge";

                case AutoShapeConstant.Index_39:
                    return "pie";

                case AutoShapeConstant.Index_40:
                    return "blockArc";

                case AutoShapeConstant.Index_41:
                    return "donut";

                case AutoShapeConstant.Index_42:
                    return "noSmoking";

                case AutoShapeConstant.Index_43:
                    return "rightArrow";

                case AutoShapeConstant.Index_44:
                    return "leftArrow";

                case AutoShapeConstant.Index_45:
                    return "upArrow";

                case AutoShapeConstant.Index_46:
                    return "downArrow";

                case AutoShapeConstant.Index_47:
                    return "stripedRightArrow";

                case AutoShapeConstant.Index_48:
                    return "notchedRightArrow";

                case AutoShapeConstant.Index_49:
                    return "bentUpArrow";

                case AutoShapeConstant.Index_50:
                    return "leftRightArrow";

                case AutoShapeConstant.Index_51:
                    return "upDownArrow";

                case AutoShapeConstant.Index_52:
                    return "leftUpArrow";

                case AutoShapeConstant.Index_53:
                    return "leftRightUpArrow";

                case AutoShapeConstant.Index_54:
                    return "quadArrow";

                case AutoShapeConstant.Index_55:
                    return "leftArrowCallout";

                case AutoShapeConstant.Index_56:
                    return "rightArrowCallout";

                case AutoShapeConstant.Index_57:
                    return "upArrowCallout";

                case AutoShapeConstant.Index_58:
                    return "downArrowCallout";

                case AutoShapeConstant.Index_59:
                    return "leftRightArrowCallout";

                case AutoShapeConstant.Index_60:
                    return "upDownArrowCallout";

                case AutoShapeConstant.Index_61:
                    return "quadArrowCallout";

                case AutoShapeConstant.Index_62:
                    return "bentArrow";

                case AutoShapeConstant.Index_63:
                    return "uturnArrow";

                case AutoShapeConstant.Index_64:
                    return "circularArrow";

                case AutoShapeConstant.Index_65:
                    return "leftCircularArrow";

                case AutoShapeConstant.Index_66:
                    return "leftRightCircularArrow";

                case AutoShapeConstant.Index_67:
                    return "curvedRightArrow";

                case AutoShapeConstant.Index_68:
                    return "curvedLeftArrow";

                case AutoShapeConstant.Index_69:
                    return "curvedUpArrow";

                case AutoShapeConstant.Index_70:
                    return "curvedDownArrow";

                case AutoShapeConstant.Index_71:
                    return "swooshArrow";

                case AutoShapeConstant.Index_72:
                    return "cube";

                case AutoShapeConstant.Index_73:
                    return "can";

                case AutoShapeConstant.Index_74:
                    return "lightningBolt";

                case AutoShapeConstant.Index_75:
                    return "heart";

                case AutoShapeConstant.Index_76:
                    return "sun";

                case AutoShapeConstant.Index_77:
                    return "moon";

                case AutoShapeConstant.Index_78:
                    return "smileyFace";

                case AutoShapeConstant.Index_79:
                    return "irregularSeal1";

                case AutoShapeConstant.Index_80:
                    return "irregularSeal2";

                case AutoShapeConstant.Index_81:
                    return "foldedCorner";

                case AutoShapeConstant.Index_82:
                    return "bevel";

                case AutoShapeConstant.Index_83:
                    return "frame";

                case AutoShapeConstant.Index_84:
                    return "halfFrame";

                case AutoShapeConstant.Index_85:
                    return "corner";

                case AutoShapeConstant.Index_86:
                    return "diagStripe";

                case AutoShapeConstant.Index_87:
                    return "chord";

                case AutoShapeConstant.Index_88:
                    return "arc";

                case AutoShapeConstant.Index_89:
                    return "leftBracket";

                case AutoShapeConstant.Index_90:
                    return "rightBracket";

                case AutoShapeConstant.Index_91:
                    return "leftBrace";

                case AutoShapeConstant.Index_92:
                    return "rightBrace";

                case AutoShapeConstant.Index_93:
                    return "bracketPair";

                case AutoShapeConstant.Index_94:
                    return "bracePair";

                case AutoShapeConstant.Index_95:
                    return "straightConnector1";

                case AutoShapeConstant.Index_96:
                    return "bentConnector2";

                case AutoShapeConstant.Index_97:
                    return "bentConnector3";

                case AutoShapeConstant.Index_98:
                    return "bentConnector4";

                case AutoShapeConstant.Index_99:
                    return "bentConnector5";

                case AutoShapeConstant.Index_100:
                    return "curvedConnector2";

                case AutoShapeConstant.Index_101:
                    return "curvedConnector3";

                case AutoShapeConstant.Index_102:
                    return "curvedConnector4";

                case AutoShapeConstant.Index_103:
                    return "curvedConnector5";

                case AutoShapeConstant.Index_104:
                    return "callout1";

                case AutoShapeConstant.Index_105:
                    return "callout2";

                case AutoShapeConstant.Index_106:
                    return "callout3";

                case AutoShapeConstant.Index_107:
                    return "accentCallout1";

                case AutoShapeConstant.Index_108:
                    return "accentCallout2";

                case AutoShapeConstant.Index_109:
                    return "accentCallout3";

                case AutoShapeConstant.Index_110:
                    return "borderCallout1";

                case AutoShapeConstant.Index_111:
                    return "borderCallout2";

                case AutoShapeConstant.Index_112:
                    return "borderCallout3";

                case AutoShapeConstant.Index_113:
                    return "accentBorderCallout1";

                case AutoShapeConstant.Index_114:
                    return "accentBorderCallout2";

                case AutoShapeConstant.Index_115:
                    return "accentBorderCallout3";

                case AutoShapeConstant.Index_116:
                    return "wedgeRectCallout";

                case AutoShapeConstant.Index_117:
                    return "wedgeRoundRectCallout";

                case AutoShapeConstant.Index_118:
                    return "wedgeEllipseCallout";

                case AutoShapeConstant.Index_119:
                    return "cloudCallout";

                case AutoShapeConstant.Index_120:
                    return "cloud";

                case AutoShapeConstant.Index_121:
                    return "ribbon";

                case AutoShapeConstant.Index_122:
                    return "ribbon2";

                case AutoShapeConstant.Index_123:
                    return "ellipseRibbon";

                case AutoShapeConstant.Index_124:
                    return "ellipseRibbon2";

                case AutoShapeConstant.Index_125:
                    return "leftRightRibbon";

                case AutoShapeConstant.Index_126:
                    return "verticalScroll";

                case AutoShapeConstant.Index_127:
                    return "horizontalScroll";

                case AutoShapeConstant.Index_128:
                    return "wave";

                case AutoShapeConstant.Index_129:
                    return "doubleWave";

                case AutoShapeConstant.Index_130:
                    return "plus";

                case AutoShapeConstant.Index_131:
                    return "flowChartProcess";

                case AutoShapeConstant.Index_132:
                    return "flowChartDecision";

                case AutoShapeConstant.Index_133:
                    return "flowChartInputOutput";

                case AutoShapeConstant.Index_134:
                    return "flowChartPredefinedProcess";

                case AutoShapeConstant.Index_135:
                    return "flowChartInternalStorage";

                case AutoShapeConstant.Index_136:
                    return "flowChartDocument";

                case AutoShapeConstant.Index_137:
                    return "flowChartMultidocument";

                case AutoShapeConstant.Index_138:
                    return "flowChartTerminator";

                case AutoShapeConstant.Index_139:
                    return "flowChartPreparation";

                case AutoShapeConstant.Index_140:
                    return "flowChartManualInput";

                case AutoShapeConstant.Index_141:
                    return "flowChartManualOperation";

                case AutoShapeConstant.Index_142:
                    return "flowChartConnector";

                case AutoShapeConstant.Index_143:
                    return "flowChartPunchedCard";

                case AutoShapeConstant.Index_144:
                    return "flowChartPunchedTape";

                case AutoShapeConstant.Index_145:
                    return "flowChartSummingJunction";

                case AutoShapeConstant.Index_146:
                    return "flowChartOr";

                case AutoShapeConstant.Index_147:
                    return "flowChartCollate";

                case AutoShapeConstant.Index_148:
                    return "flowChartSort";

                case AutoShapeConstant.Index_149:
                    return "flowChartExtract";

                case AutoShapeConstant.Index_150:
                    return "flowChartMerge";

                case AutoShapeConstant.Index_151:
                    return "flowChartOfflineStorage";

                case AutoShapeConstant.Index_152:
                    return "flowChartOnlineStorage";

                case AutoShapeConstant.Index_153:
                    return "flowChartMagneticTape";

                case AutoShapeConstant.Index_154:
                    return "flowChartMagneticDisk";

                case AutoShapeConstant.Index_155:
                    return "flowChartMagneticDrum";

                case AutoShapeConstant.Index_156:
                    return "flowChartDisplay";

                case AutoShapeConstant.Index_157:
                    return "flowChartDelay";

                case AutoShapeConstant.Index_158:
                    return "flowChartAlternateProcess";

                case AutoShapeConstant.Index_159:
                    return "flowChartOffpageConnector";

                case AutoShapeConstant.Index_160:
                    return "actionButtonBlank";

                case AutoShapeConstant.Index_161:
                    return "actionButtonHome";

                case AutoShapeConstant.Index_162:
                    return "actionButtonHelp";

                case AutoShapeConstant.Index_163:
                    return "actionButtonInformation";

                case AutoShapeConstant.Index_164:
                    return "actionButtonForwardNext";

                case AutoShapeConstant.Index_165:
                    return "actionButtonBackPrevious";

                case AutoShapeConstant.Index_166:
                    return "actionButtonEnd";

                case AutoShapeConstant.Index_167:
                    return "actionButtonBeginning";

                case AutoShapeConstant.Index_168:
                    return "actionButtonReturn";

                case AutoShapeConstant.Index_169:
                    return "actionButtonDocument";

                case AutoShapeConstant.Index_170:
                    return "actionButtonSound";

                case AutoShapeConstant.Index_171:
                    return "actionButtonMovie";

                case AutoShapeConstant.Index_172:
                    return "gear6";

                case AutoShapeConstant.Index_173:
                    return "gear9";

                case AutoShapeConstant.Index_174:
                    return "funnel";

                case AutoShapeConstant.Index_175:
                    return "mathPlus";

                case AutoShapeConstant.Index_176:
                    return "mathMinus";

                case AutoShapeConstant.Index_177:
                    return "mathMultiply";

                case AutoShapeConstant.Index_178:
                    return "mathDivide";

                case AutoShapeConstant.Index_179:
                    return "mathEqual";

                case AutoShapeConstant.Index_180:
                    return "mathNotEqual";

                case AutoShapeConstant.Index_181:
                    return "cornerTabs";

                case AutoShapeConstant.Index_182:
                    return "squareTabs";

                case AutoShapeConstant.Index_183:
                    return "plaqueTabs";

                case AutoShapeConstant.Index_184:
                    return "chartX";

                case AutoShapeConstant.Index_185:
                    return "chartStar";

                case AutoShapeConstant.Index_186:
                    return "chartPlus";
            }
            return null;
        }
        internal static AutoShapeType GetAutoShapeType(string shapeTypeID)
        {
            CreateAutoShapeDictionary();

            string[] keys = new string[shapeTypes.Count];
            string[] values = new string[shapeTypes.Count];
            shapeTypes.Keys.CopyTo(keys, 0);
            shapeTypes.Values.CopyTo(values, 0);

            int shapeIndex = Array.IndexOf(values, shapeTypeID);
            if (shapeIndex == -1)
                return AutoShapeType.Unknown;
#if SILVERLIGHT || WP
            return (AutoShapeType)Enum.Parse(typeof(AutoShapeType), keys[shapeIndex],true);
#else
            return (AutoShapeType)Enum.Parse(typeof(AutoShapeType), keys[shapeIndex]);
#endif
        }

       
        internal static string GetAutoShapeTypeIndex(AutoShapeType autoShapeType)
        {
            CreateAutoShapeDictionary();
            return shapeTypes[autoShapeType.ToString()];
        }
        internal static string GetShapeTypeIDorAttributeToCheck(AutoShapeType autoShapeType)
        {
            string type = GetAutoShapeTypeIndex(autoShapeType);
            switch (type)
            {
                case "Cloud":
                case "Snip Single Corner Rectangle":
                case "Snip Same Side Corner Rectangle":
                case "Snip Diagonal Corner Rectangle":
                case "Snip and Round Single Corner Rectangle":
                case "Round Single Corner Rectangle":
                case "Round Same Side Corner Rectangle":
                case "Round Diagonal Corner Rectangle":
                case "Heptagon":
                case "Decagon":
                case "Dodecagon":
                case "Pie":
                case "Chord":
                case "Teardrop":
                case "Frame":
                case "Half Frame":
                case "L-Shape":
                case "Diagonal Stripe":
                case "Plus":
                case "Minus":
                case "Multiply":
                case "Division":
                case "Equal":
                case "Not Equal":
                case "6-Point Star":
                case "7-Point Star":
                case "10-Point Star":
                case "12-Point Star":
                    type = "id=\"" + type + "\"";
                    break;
                default:
                    //type = "o:spt=\""+type +"\"";
                    type = "id=\"_x0000_t" + type + "\"";
                    break;
            }
            return type;
        }
        
        private static void CreateAutoShapeDictionary()
        {
            if (shapeTypes.Count == 0)
            {
                shapeTypes.Add("Unknown", "-1");
                //Rectangles	
                shapeTypes.Add("Rectangle", "1");
                shapeTypes.Add("RoundedRectangle ", "2");
                shapeTypes.Add("SnipSingleCornerRectangle", "Snip Single Corner Rectangle");
                shapeTypes.Add("SnipSameSideCornerRectangle", "Snip Same Side Corner Rectangle");
                shapeTypes.Add("SnipDiagonalCornerRectangle", "Snip Diagonal Corner Rectangle");
                shapeTypes.Add("SnipAndRoundSingleCornerRectangle", "Snip and Round Single Corner Rectangle");
                shapeTypes.Add("RoundSingleCornerRectangle", "Round Single Corner Rectangle");
                shapeTypes.Add("RoundSameSideCornerRectangle", "Round Same Side Corner Rectangle");
                shapeTypes.Add("RoundDiagonalCornerRectangle", "Round Diagonal Corner Rectangle");

                //Basic Shapes	
                shapeTypes.Add("Oval", "3");
                shapeTypes.Add("IsoscelesTriangle", "5");
                shapeTypes.Add("RightTriangle", "6");
                shapeTypes.Add("Parallelogram", "7");
                shapeTypes.Add("Trapezoid", "8");
                shapeTypes.Add("Diamond", "4");
                shapeTypes.Add("RegularPentagon", "56");
                shapeTypes.Add("Hexagon", "9");
                shapeTypes.Add("Heptagon", "Heptagon");
                shapeTypes.Add("Octagon", "10");
                shapeTypes.Add("Decagon", "Decagon");
                shapeTypes.Add("Dodecagon", "Dodecagon");
                shapeTypes.Add("Pie", "Pie");
                shapeTypes.Add("Chord", "Chord");
                shapeTypes.Add("Teardrop", "Teardrop");
                shapeTypes.Add("Frame", "Frame");
                shapeTypes.Add("HalfFrame", "Half Frame");
                shapeTypes.Add("L_Shape", "L-Shape");
                shapeTypes.Add("DiagonalStripe", "Diagonal Stripe");
                shapeTypes.Add("Cross", "11");
                shapeTypes.Add("Plaque", "21");
                shapeTypes.Add("Can", "22");
                shapeTypes.Add("Cube", "16");
                shapeTypes.Add("Bevel", "84");
                shapeTypes.Add("Donut", "23");
                shapeTypes.Add("NoSymbol", "57");
                shapeTypes.Add("BlockArc", "95");
                shapeTypes.Add("FoldedCorner", "65");
                shapeTypes.Add("SmileyFace", "96");
                shapeTypes.Add("Heart", "74");
                shapeTypes.Add("LightningBolt", "73");
                shapeTypes.Add("Sun", "183");
                shapeTypes.Add("Moon", "184");
                shapeTypes.Add("Cloud", "Cloud");
                shapeTypes.Add("Arc", "19");
                shapeTypes.Add("DoubleBracket", "185");
                shapeTypes.Add("DoubleBrace", "186");
                shapeTypes.Add("LeftBracket", "85");
                shapeTypes.Add("RightBracket", "86");
                shapeTypes.Add("LeftBrace", "87");
                shapeTypes.Add("RightBrace", "88");


                //BlockArrows	
                shapeTypes.Add("RightArrow", "13");
                shapeTypes.Add("LeftArrow", "66");
                shapeTypes.Add("UpArrow", "68");
                shapeTypes.Add("DownArrow", "67");
                shapeTypes.Add("LeftRightArrow", "69");
                shapeTypes.Add("UpDownArrow", "70");
                shapeTypes.Add("QuadArrow", "76");
                shapeTypes.Add("LeftRightUpArrow", "182");
                shapeTypes.Add("BentArrow", "91");
                shapeTypes.Add("UTurnArrow", "101");
                shapeTypes.Add("LeftUpArrow", "89");
                shapeTypes.Add("BentUpArrow", "90");
                shapeTypes.Add("CurvedRightArrow", "102");
                shapeTypes.Add("CurvedLeftArrow", "103");
                shapeTypes.Add("CurvedUpArrow", "104");
                shapeTypes.Add("CurvedDownArrow", "105");
                shapeTypes.Add("StripedRightArrow", "93");
                shapeTypes.Add("NotchedRightArrow", "94");
                shapeTypes.Add("Pentagon", "15");
                shapeTypes.Add("Chevron", "55");
                shapeTypes.Add("RightArrowCallout", "78");
                shapeTypes.Add("DownArrowCallout", "80");
                shapeTypes.Add("LeftArrowCallout", "77");
                shapeTypes.Add("UpArrowCallout", "79");
                shapeTypes.Add("LeftRightArrowCallout", "81");
                shapeTypes.Add("UpDownArrowCallout", "82");
                shapeTypes.Add("QuadArrowCallout", "83");
                shapeTypes.Add("CircularArrow", "99");

                //Equations	
                shapeTypes.Add("MathPlus", "Plus");
                shapeTypes.Add("MathMinus", "Minus");
                shapeTypes.Add("MathMultiply", "Multiply");
                shapeTypes.Add("MathDivision", "Division");
                shapeTypes.Add("MathEqual", "Equal");
                shapeTypes.Add("MathNotEqual", "Not Equal");

                //FlowCharts	
                shapeTypes.Add("FlowChartProcess", "109");
                shapeTypes.Add("FlowChartAlternateProcess", "176");
                shapeTypes.Add("FlowChartDecision", "110");
                shapeTypes.Add("FlowChartData", "111");
                shapeTypes.Add("FlowChartPredefinedProcess", "112");
                shapeTypes.Add("FlowChartInternalStorage", "113");
                shapeTypes.Add("FlowChartDocument", "114");
                shapeTypes.Add("FlowChartMultiDocument", "115");
                shapeTypes.Add("FlowChartTerminator", "116");
                shapeTypes.Add("FlowChartPreparation", "117");
                shapeTypes.Add("FlowChartManualInput", "118");
                shapeTypes.Add("FlowChartManualOperation", "119");
                shapeTypes.Add("FlowChartConnector", "120");
                shapeTypes.Add("FlowChartOffPageConnector", "177");
                shapeTypes.Add("FlowChartCard", "121");
                shapeTypes.Add("FlowChartPunchedTape", "122");
                shapeTypes.Add("FlowChartSummingJunction", "123");
                shapeTypes.Add("FlowChartOr", "124");
                shapeTypes.Add("FlowChartCollate", "125");
                shapeTypes.Add("FlowChartSort", "126");
                shapeTypes.Add("FlowChartExtract", "127");
                shapeTypes.Add("FlowChartMerge", "128");
                shapeTypes.Add("FlowChartStoredData", "130");
                shapeTypes.Add("FlowChartDelay", "135");
                shapeTypes.Add("FlowChartSequentialAccessStorage", "131");
                shapeTypes.Add("FlowChartMagneticDisk", "132");
                shapeTypes.Add("FlowChartDirectAccessStorage", "133");
                shapeTypes.Add("FlowChartDisplay", "134");

                //StarsAndBanner	
                shapeTypes.Add("Explosion1", "71");
                shapeTypes.Add("Explosion2", "72");
                shapeTypes.Add("Star4Point", "187");
                shapeTypes.Add("Star5Point", "12");
                shapeTypes.Add("Star6Point", "6-Point Star");
                shapeTypes.Add("Star7Point", "7-Point Star");
                shapeTypes.Add("Star8Point", "58");
                shapeTypes.Add("Star10Point", "10-Point Star");
                shapeTypes.Add("Star12Point", "12-Point Star");
                shapeTypes.Add("Star16Point", "59");
                shapeTypes.Add("Star24Point", "92");
                shapeTypes.Add("Star32Point", "60");
                shapeTypes.Add("UpRibbon", "54");
                shapeTypes.Add("DownRibbon", "53");
                shapeTypes.Add("CurvedUpRibbon", "108");
                shapeTypes.Add("CurvedDownRibbon", "107");
                shapeTypes.Add("VerticalScroll", "97");
                shapeTypes.Add("HorizontalScroll", "98");
                shapeTypes.Add("Wave", "64");
                shapeTypes.Add("DoubleWave", "188");

                //CallOuts	
                shapeTypes.Add("RectangularCallout", "61");
                shapeTypes.Add("RoundedRectangularCallout", "17");
                shapeTypes.Add("OvalCallout", "63");
                shapeTypes.Add("CloudCallout", "106");
                shapeTypes.Add("LineCallout1", "47");
                shapeTypes.Add("LineCallout2", "48");
                shapeTypes.Add("LineCallout3", "49");
                shapeTypes.Add("LineCallout1AccentBar", "44");
                shapeTypes.Add("LineCallout2AccentBar", "45");
                shapeTypes.Add("LineCallout3AccentBar", "46");
                shapeTypes.Add("LineCallout1NoBorder", "41");
                shapeTypes.Add("LineCallout2NoBorder", "42");
                shapeTypes.Add("LineCallout3NoBorder", "43");
                shapeTypes.Add("LineCallout1BorderAndAccentBar", "50");
                shapeTypes.Add("LineCallout2BorderAndAccentBar", "51");
                shapeTypes.Add("LineCallout3BorderAndAccentBar", "52");
                //Connectors
                shapeTypes.Add("Line", "Line");
                shapeTypes.Add("StraightConnector", "32");
                shapeTypes.Add("CurvedConnector", "38");
                shapeTypes.Add("ElbowConnector", "34");
                shapeTypes.Add("CurvedConnector2", "37");
                shapeTypes.Add("CurvedConnector4", "39");
                shapeTypes.Add("CurvedConnector5", "40");
                shapeTypes.Add("BentConnector2", "33");
                shapeTypes.Add("BentConnector4", "35");
                shapeTypes.Add("BentConnector5", "36");
            }
        }
    }
    internal enum AutoShapeConstant
    {
        Index_0 = 1,
        Index_1 = 2,
        Index_2 = 3,
        Index_3 = 4,
        Index_4 = 5,
        Index_5 = 6,
        Index_6 = 7,
        Index_7 = 8,
        Index_8 = 9,
        Index_9 = 10,
        Index_10 = 11,
        Index_11 = 12,
        Index_12 = 13,
        Index_13 = 14,
        Index_14 = 15,
        Index_15 = 16,
        Index_16 = 17,
        Index_17 = 18,
        Index_18 = 19,
        Index_19 = 20,
        Index_20 = 21,
        Index_21 = 22,
        Index_22 = 23,
        Index_23 = 24,
        Index_24 = 25,
        Index_25 = 26,
        Index_26 = 27,
        Index_27 = 28,
        Index_28 = 29,
        Index_29 = 30,
        Index_30 = 31,
        Index_31 = 32,
        Index_32 = 33,
        Index_33 = 34,
        Index_34 = 35,
        Index_35 = 36,
        Index_36 = 37,
        Index_37 = 38,
        Index_38 = 39,
        Index_39 = 40,
        Index_40 = 41,
        Index_41 = 42,
        Index_42 = 43,
        Index_43 = 44,
        Index_44 = 45,
        Index_45 = 46,
        Index_46 = 47,
        Index_47 = 48,
        Index_48 = 49,
        Index_49 = 50,
        Index_50 = 51,
        Index_51 = 52,
        Index_52 = 53,
        Index_53 = 54,
        Index_54 = 55,
        Index_55 = 56,
        Index_56 = 57,
        Index_57 = 58,
        Index_58 = 59,
        Index_59 = 60,
        Index_60 = 61,
        Index_61 = 62,
        Index_62 = 63,
        Index_63 = 64,
        Index_64 = 65,
        Index_65 = 66,
        Index_66 = 67,
        Index_67 = 68,
        Index_68 = 69,
        Index_69 = 70,
        Index_70 = 71,
        Index_71 = 72,
        Index_72 = 73,
        Index_73 = 74,
        Index_74 = 75,
        Index_75 = 76,
        Index_76 = 77,
        Index_77 = 78,
        Index_78 = 79,
        Index_79 = 80,
        Index_80 = 81,
        Index_81 = 82,
        Index_82 = 83,
        Index_83 = 84,
        Index_84 = 85,
        Index_85 = 86,
        Index_86 = 87,
        Index_87 = 88,
        Index_88 = 89,
        Index_89 = 90,
        Index_90 = 91,
        Index_91 = 92,
        Index_92 = 93,
        Index_93 = 94,
        Index_94 = 95,
        Index_95 = 96,
        Index_96 = 97,
        Index_97 = 98,
        Index_98 = 99,
        Index_99 = 100,
        Index_100 = 101,
        Index_101 = 102,
        Index_102 = 103,
        Index_103 = 104,
        Index_104 = 105,
        Index_105 = 106,
        Index_106 = 107,
        Index_107 = 108,
        Index_108 = 109,
        Index_109 = 110,
        Index_110 = 111,
        Index_111 = 112,
        Index_112 = 113,
        Index_113 = 114,
        Index_114 = 115,
        Index_115 = 116,
        Index_116 = 117,
        Index_117 = 118,
        Index_118 = 119,
        Index_119 = 120,
        Index_120 = 121,
        Index_121 = 122,
        Index_122 = 123,
        Index_123 = 124,
        Index_124 = 125,
        Index_125 = 126,
        Index_126 = 127,
        Index_127 = 128,
        Index_128 = 129,
        Index_129 = 130,
        Index_130 = 131,
        Index_131 = 132,
        Index_132 = 133,
        Index_133 = 134,
        Index_134 = 135,
        Index_135 = 136,
        Index_136 = 137,
        Index_137 = 138,
        Index_138 = 139,
        Index_139 = 140,
        Index_140 = 141,
        Index_141 = 142,
        Index_142 = 143,
        Index_143 = 144,
        Index_144 = 145,
        Index_145 = 146,
        Index_146 = 147,
        Index_147 = 148,
        Index_148 = 149,
        Index_149 = 150,
        Index_150 = 151,
        Index_151 = 152,
        Index_152 = 153,
        Index_153 = 154,
        Index_154 = 155,
        Index_155 = 156,
        Index_156 = 157,
        Index_157 = 158,
        Index_158 = 159,
        Index_159 = 160,
        Index_160 = 161,
        Index_161 = 162,
        Index_162 = 163,
        Index_163 = 164,
        Index_164 = 165,
        Index_165 = 166,
        Index_166 = 167,
        Index_167 = 168,
        Index_168 = 169,
        Index_169 = 170,
        Index_170 = 171,
        Index_171 = 172,
        Index_172 = 173,
        Index_173 = 174,
        Index_174 = 175,
        Index_175 = 176,
        Index_176 = 177,
        Index_177 = 178,
        Index_178 = 179,
        Index_179 = 180,
        Index_180 = 181,
        Index_181 = 182,
        Index_182 = 183,
        Index_183 = 184,
        Index_184 = 185,
        Index_185 = 186,
        Index_186 = 187,
        Index_187 = 188,
    }
}

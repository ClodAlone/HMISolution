#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.Windows.Shared
{
    
    internal class PhoneticsProcessor
    {
        // Fields
        private static string frontv = "EIY";
        private static int maxCodeLen = 6;
        private static string varson = "CSPTG";
        private static string vowels = "AEIOU";

        // Methods
        public static bool HasSameMetaPhone(string txt, string metaphone)
        {
            int num2;
            int num = 0;
            bool flag = false;
            if ((txt == null) || (txt.Length == 0))
            {
                return false;
            }
            if ((txt.Length == 1) && txt.ToUpper().Equals(metaphone))
            {
                return true;
            }
            if (txt.Length == 1)
            {
                return false;
            }
            char[] chArray = txt.ToUpper().ToCharArray();
            StringBuilder builder = new StringBuilder(40);
            StringBuilder builder2 = new StringBuilder(10);
            char ch2 = chArray[0];
            if (ch2 <= 'G')
            {
                switch (ch2)
                {
                    case 'A':
                        if (chArray[1] == 'E')
                        {
                            builder.Append(chArray, 1, chArray.Length - 1);
                        }
                        else
                        {
                            builder.Append(chArray);
                        }
                        goto Label_0196;

                    case 'G':
                        goto Label_00C5;
                }
                goto Label_018B;
            }
            if ((ch2 != 'K') && (ch2 != 'P'))
            {
                switch (ch2)
                {
                    case 'W':
                        if (chArray[1] != 'R')
                        {
                            if (chArray[1] == 'H')
                            {
                                builder.Append(chArray, 1, chArray.Length - 1);
                                builder[0] = 'W';
                            }
                            else
                            {
                                builder.Append(chArray);
                            }
                        }
                        else
                        {
                            builder.Append(chArray, 1, chArray.Length - 1);
                        }
                        goto Label_0196;

                    case 'X':
                        chArray[0] = 'S';
                        builder.Append(chArray);
                        goto Label_0196;
                }
                goto Label_018B;
            }
        Label_00C5:
            if (chArray[1] == 'N')
            {
                builder.Append(chArray, 1, chArray.Length - 1);
            }
            else
            {
                builder.Append(chArray);
            }
            goto Label_0196;
        Label_018B:
            builder.Append(chArray);
        Label_0196:
            num2 = builder.Length;
            int startIndex = 0;
            while ((num < maxCodeLen) && (startIndex < num2))
            {
                string str;
                if (builder2.Length > metaphone.Length)
                {
                    return false;
                }
                if ((builder2.Length > 0) && (builder2[builder2.Length - 1] != metaphone[builder2.Length - 1]))
                {
                    return false;
                }
                char ch = builder[startIndex];
                if (((ch != 'C') && (startIndex > 0)) && (builder[startIndex - 1] == ch))
                {
                    startIndex++;
                    goto Label_08BA;
                }
                switch (ch)
                {
                    case 'A':
                    case 'E':
                    case 'I':
                    case 'O':
                    case 'U':
                        if (startIndex == 0)
                        {
                            builder2.Append(ch);
                            num++;
                        }
                        goto Label_08B3;

                    case 'B':
                        if (((startIndex <= 0) || ((startIndex + 1) == num2)) || (builder[startIndex - 1] != 'M'))
                        {
                            break;
                        }
                        builder2.Append(ch);
                        goto Label_032B;

                    case 'C':
                        if ((((startIndex <= 0) || (builder[startIndex - 1] != 'S')) || ((startIndex + 1) >= num2)) || (frontv.IndexOf(builder[startIndex + 1]) < 0))
                        {
                            goto Label_0377;
                        }
                        goto Label_08B3;

                    case 'D':
                        if ((((startIndex + 2) >= num2) || (builder[startIndex + 1] != 'G')) || (frontv.IndexOf(builder[startIndex + 2]) < 0))
                        {
                            goto Label_04EB;
                        }
                        builder2.Append('J');
                        startIndex += 2;
                        goto Label_04F7;

                    case 'F':
                    case 'J':
                    case 'L':
                    case 'M':
                    case 'N':
                    case 'R':
                        builder2.Append(ch);
                        num++;
                        goto Label_08B3;

                    case 'G':
                        if (((startIndex + 2) != num2) || (builder[startIndex + 1] != 'H'))
                        {
                            goto Label_0528;
                        }
                        goto Label_08B3;

                    case 'H':
                        if ((startIndex + 1) != num2)
                        {
                            goto Label_062D;
                        }
                        goto Label_08B3;

                    case 'K':
                        if (startIndex <= 0)
                        {
                            goto Label_06D1;
                        }
                        if (builder[startIndex - 1] != 'C')
                        {
                            builder2.Append(ch);
                        }
                        goto Label_06DB;

                    case 'P':
                        if (((startIndex + 1) >= num2) || (builder[startIndex + 1] != 'H'))
                        {
                            goto Label_0713;
                        }
                        builder2.Append('F');
                        goto Label_071D;

                    case 'Q':
                        builder2.Append('K');
                        num++;
                        goto Label_08B3;

                    case 'S':
                        str = builder.ToString();
                        if (((str.IndexOf("SH", startIndex) != startIndex) && (str.IndexOf("SIO", startIndex) != startIndex)) && (str.IndexOf("SIA", startIndex) != startIndex))
                        {
                            goto Label_078C;
                        }
                        builder2.Append('X');
                        goto Label_0796;

                    case 'T':
                        str = builder.ToString();
                        if ((str.IndexOf("TIA", startIndex) != startIndex) && (str.IndexOf("TIO", startIndex) != startIndex))
                        {
                            goto Label_07E9;
                        }
                        builder2.Append('X');
                        num++;
                        goto Label_08B3;

                    case 'V':
                        builder2.Append('F');
                        num++;
                        goto Label_08B3;

                    case 'W':
                    case 'Y':
                        if (((startIndex + 1) < num2) && (vowels.IndexOf(builder[startIndex + 1]) >= 0))
                        {
                            builder2.Append(ch);
                            num++;
                        }
                        goto Label_08B3;

                    case 'X':
                        builder2.Append('K');
                        builder2.Append('S');
                        num += 2;
                        goto Label_08B3;

                    case 'Z':
                        builder2.Append('S');
                        num++;
                        goto Label_08B3;

                    default:
                        goto Label_08B3;
                }
                builder2.Append(ch);
            Label_032B:
                num++;
                goto Label_08B3;
            Label_0377:
                str = builder.ToString();
                if (str.IndexOf("CIA", startIndex) == startIndex)
                {
                    builder2.Append('X');
                    num++;
                }
                else if (((startIndex + 1) < num2) && (frontv.IndexOf(builder[startIndex + 1]) >= 0))
                {
                    builder2.Append('S');
                    num++;
                }
                else if ((startIndex > 0) && (str.IndexOf("SCH", (int)(startIndex - 1)) == (startIndex - 1)))
                {
                    builder2.Append('K');
                    num++;
                }
                else if (str.IndexOf("CH", startIndex) == startIndex)
                {
                    if (((startIndex == 0) && (num2 >= 3)) && (vowels.IndexOf(builder[2]) < 0))
                    {
                        builder2.Append('K');
                    }
                    else
                    {
                        builder2.Append('X');
                    }
                    num++;
                }
                else
                {
                    builder2.Append('K');
                    num++;
                }
                goto Label_08B3;
            Label_04EB:
                builder2.Append('T');
            Label_04F7:
                num++;
                goto Label_08B3;
            Label_0528:
                if ((((startIndex + 2) >= num2) || (builder[startIndex + 1] != 'H')) || (vowels.IndexOf(builder[startIndex + 2]) >= 0))
                {
                    str = builder.ToString();
                    if (((startIndex <= 0) || (str.IndexOf("GN", startIndex) != startIndex)) && (str.IndexOf("GNED", startIndex) != startIndex))
                    {
                        if ((startIndex > 0) && (builder[startIndex - 1] == 'G'))
                        {
                            flag = true;
                        }
                        else
                        {
                            flag = false;
                        }
                        if (!((((startIndex + 1) >= num2) || (frontv.IndexOf(builder[startIndex + 1]) < 0)) || flag))
                        {
                            builder2.Append('J');
                        }
                        else
                        {
                            builder2.Append('K');
                        }
                        num++;
                    }
                }
                goto Label_08B3;
            Label_062D:
                if (((startIndex <= 0) || (varson.IndexOf(builder[startIndex - 1]) < 0)) && (vowels.IndexOf(builder[startIndex + 1]) >= 0))
                {
                    builder2.Append('H');
                    num++;
                }
                goto Label_08B3;
            Label_06D1:
                builder2.Append(ch);
            Label_06DB:
                num++;
                goto Label_08B3;
            Label_0713:
                builder2.Append(ch);
            Label_071D:
                num++;
                goto Label_08B3;
            Label_078C:
                builder2.Append('S');
            Label_0796:
                num++;
                goto Label_08B3;
            Label_07E9:
                if (str.IndexOf("TCH", startIndex) != startIndex)
                {
                    if (str.IndexOf("TH", startIndex) == startIndex)
                    {
                        builder2.Append('0');
                    }
                    else
                    {
                        builder2.Append('T');
                    }
                    num++;
                }
            Label_08B3:
                startIndex++;
            Label_08BA:
                if (num > maxCodeLen)
                {
                    builder2.Length = maxCodeLen;
                }
            }
            return builder2.ToString().Equals(metaphone);
        }

        public static string MetaPhone(string txt)
        {
            int num2;
            int num = 0;
            bool flag = false;
            if ((txt == null) || (txt.Length == 0))
            {
                return "";
            }
            if (txt.Length == 1)
            {
                return txt.ToUpper();
            }
            char[] chArray = txt.ToUpper().ToCharArray();
            StringBuilder builder = new StringBuilder(40);
            StringBuilder builder2 = new StringBuilder(10);
            char ch2 = chArray[0];
            if (ch2 <= 'G')
            {
                switch (ch2)
                {
                    case 'A':
                        if (chArray[1] == 'E')
                        {
                            builder.Append(chArray, 1, chArray.Length - 1);
                        }
                        else
                        {
                            builder.Append(chArray);
                        }
                        goto Label_0176;

                    case 'G':
                        goto Label_00A5;
                }
                goto Label_016B;
            }
            if ((ch2 != 'K') && (ch2 != 'P'))
            {
                switch (ch2)
                {
                    case 'W':
                        if (chArray[1] != 'R')
                        {
                            if (chArray[1] == 'H')
                            {
                                builder.Append(chArray, 1, chArray.Length - 1);
                                builder[0] = 'W';
                            }
                            else
                            {
                                builder.Append(chArray);
                            }
                        }
                        else
                        {
                            builder.Append(chArray, 1, chArray.Length - 1);
                        }
                        goto Label_0176;

                    case 'X':
                        chArray[0] = 'S';
                        builder.Append(chArray);
                        goto Label_0176;
                }
                goto Label_016B;
            }
        Label_00A5:
            if (chArray[1] == 'N')
            {
                builder.Append(chArray, 1, chArray.Length - 1);
            }
            else
            {
                builder.Append(chArray);
            }
            goto Label_0176;
        Label_016B:
            builder.Append(chArray);
        Label_0176:
            num2 = builder.Length;
            int startIndex = 0;
            while ((num < maxCodeLen) && (startIndex < num2))
            {
                string str;
                char ch = builder[startIndex];
                if (((ch != 'C') && (startIndex > 0)) && (builder[startIndex - 1] == ch))
                {
                    startIndex++;
                    goto Label_0832;
                }
                switch (ch)
                {
                    case 'A':
                    case 'E':
                    case 'I':
                    case 'O':
                    case 'U':
                        if (startIndex == 0)
                        {
                            builder2.Append(ch);
                            num++;
                        }
                        goto Label_082B;

                    case 'B':
                        if (((startIndex <= 0) || ((startIndex + 1) == num2)) || (builder[startIndex - 1] != 'M'))
                        {
                            break;
                        }
                        builder2.Append(ch);
                        goto Label_02A3;

                    case 'C':
                        if ((((startIndex <= 0) || (builder[startIndex - 1] != 'S')) || ((startIndex + 1) >= num2)) || (frontv.IndexOf(builder[startIndex + 1]) < 0))
                        {
                            goto Label_02EF;
                        }
                        goto Label_082B;

                    case 'D':
                        if ((((startIndex + 2) >= num2) || (builder[startIndex + 1] != 'G')) || (frontv.IndexOf(builder[startIndex + 2]) < 0))
                        {
                            goto Label_0463;
                        }
                        builder2.Append('J');
                        startIndex += 2;
                        goto Label_046F;

                    case 'F':
                    case 'J':
                    case 'L':
                    case 'M':
                    case 'N':
                    case 'R':
                        builder2.Append(ch);
                        num++;
                        goto Label_082B;

                    case 'G':
                        if (((startIndex + 2) != num2) || (builder[startIndex + 1] != 'H'))
                        {
                            goto Label_04A0;
                        }
                        goto Label_082B;

                    case 'H':
                        if ((startIndex + 1) != num2)
                        {
                            goto Label_05A5;
                        }
                        goto Label_082B;

                    case 'K':
                        if (startIndex <= 0)
                        {
                            goto Label_0649;
                        }
                        if (builder[startIndex - 1] != 'C')
                        {
                            builder2.Append(ch);
                        }
                        goto Label_0653;

                    case 'P':
                        if (((startIndex + 1) >= num2) || (builder[startIndex + 1] != 'H'))
                        {
                            goto Label_068B;
                        }
                        builder2.Append('F');
                        goto Label_0695;

                    case 'Q':
                        builder2.Append('K');
                        num++;
                        goto Label_082B;

                    case 'S':
                        str = builder.ToString();
                        if (((str.IndexOf("SH", startIndex) != startIndex) && (str.IndexOf("SIO", startIndex) != startIndex)) && (str.IndexOf("SIA", startIndex) != startIndex))
                        {
                            goto Label_0704;
                        }
                        builder2.Append('X');
                        goto Label_070E;

                    case 'T':
                        str = builder.ToString();
                        if ((str.IndexOf("TIA", startIndex) != startIndex) && (str.IndexOf("TIO", startIndex) != startIndex))
                        {
                            goto Label_0761;
                        }
                        builder2.Append('X');
                        num++;
                        goto Label_082B;

                    case 'V':
                        builder2.Append('F');
                        num++;
                        goto Label_082B;

                    case 'W':
                    case 'Y':
                        if (((startIndex + 1) < num2) && (vowels.IndexOf(builder[startIndex + 1]) >= 0))
                        {
                            builder2.Append(ch);
                            num++;
                        }
                        goto Label_082B;

                    case 'X':
                        builder2.Append('K');
                        builder2.Append('S');
                        num += 2;
                        goto Label_082B;

                    case 'Z':
                        builder2.Append('S');
                        num++;
                        goto Label_082B;

                    default:
                        goto Label_082B;
                }
                builder2.Append(ch);
            Label_02A3:
                num++;
                goto Label_082B;
            Label_02EF:
                str = builder.ToString();
                if (str.IndexOf("CIA", startIndex) == startIndex)
                {
                    builder2.Append('X');
                    num++;
                }
                else if (((startIndex + 1) < num2) && (frontv.IndexOf(builder[startIndex + 1]) >= 0))
                {
                    builder2.Append('S');
                    num++;
                }
                else if ((startIndex > 0) && (str.IndexOf("SCH", (int)(startIndex - 1)) == (startIndex - 1)))
                {
                    builder2.Append('K');
                    num++;
                }
                else if (str.IndexOf("CH", startIndex) == startIndex)
                {
                    if (((startIndex == 0) && (num2 >= 3)) && (vowels.IndexOf(builder[2]) < 0))
                    {
                        builder2.Append('K');
                    }
                    else
                    {
                        builder2.Append('X');
                    }
                    num++;
                }
                else
                {
                    builder2.Append('K');
                    num++;
                }
                goto Label_082B;
            Label_0463:
                builder2.Append('T');
            Label_046F:
                num++;
                goto Label_082B;
            Label_04A0:
                if ((((startIndex + 2) >= num2) || (builder[startIndex + 1] != 'H')) || (vowels.IndexOf(builder[startIndex + 2]) >= 0))
                {
                    str = builder.ToString();
                    if (((startIndex <= 0) || (str.IndexOf("GN", startIndex) != startIndex)) && (str.IndexOf("GNED", startIndex) != startIndex))
                    {
                        if ((startIndex > 0) && (builder[startIndex - 1] == 'G'))
                        {
                            flag = true;
                        }
                        else
                        {
                            flag = false;
                        }
                        if (!((((startIndex + 1) >= num2) || (frontv.IndexOf(builder[startIndex + 1]) < 0)) || flag))
                        {
                            builder2.Append('J');
                        }
                        else
                        {
                            builder2.Append('K');
                        }
                        num++;
                    }
                }
                goto Label_082B;
            Label_05A5:
                if (((startIndex <= 0) || (varson.IndexOf(builder[startIndex - 1]) < 0)) && (vowels.IndexOf(builder[startIndex + 1]) >= 0))
                {
                    builder2.Append('H');
                    num++;
                }
                goto Label_082B;
            Label_0649:
                builder2.Append(ch);
            Label_0653:
                num++;
                goto Label_082B;
            Label_068B:
                builder2.Append(ch);
            Label_0695:
                num++;
                goto Label_082B;
            Label_0704:
                builder2.Append('S');
            Label_070E:
                num++;
                goto Label_082B;
            Label_0761:
                if (str.IndexOf("TCH", startIndex) != startIndex)
                {
                    if (str.IndexOf("TH", startIndex) == startIndex)
                    {
                        builder2.Append('0');
                    }
                    else
                    {
                        builder2.Append('T');
                    }
                    num++;
                }
            Label_082B:
                startIndex++;
            Label_0832:
                if (num > maxCodeLen)
                {
                    builder2.Length = maxCodeLen;
                }
            }
            return builder2.ToString();
        }
    }
}

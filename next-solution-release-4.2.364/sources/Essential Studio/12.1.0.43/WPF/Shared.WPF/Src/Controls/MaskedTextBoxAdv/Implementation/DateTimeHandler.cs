#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Syncfusion.Windows.Tools.Controls
{
    public class DateTimeHandler
    {
        internal static char[] allStandardFormats = new char[] { 
            'd', 'D', 'f', 'F', 'g', 'G', 'm', 'M', 'o', 'O', 'r', 'R', 's', 't', 'T', 'u', 
            'U', 'y', 'Y'
         };

        public static DateTimeHandler dateTimeHandler = new DateTimeHandler();

        public ObservableCollection<DateTimeProperties> CreateDateTimePatteren(MaskedTextBoxAdv MaskedTextBoxAdv)
        {
            var Collection = new ObservableCollection<DateTimeProperties>();

            DateTimeFormatInfo formatinfo = MaskedTextBoxAdv.GetCulture().DateTimeFormat.Clone() as DateTimeFormatInfo;
            string format = MaskedTextBoxAdv.DateTimeFormat;
            format = GetSpecificFormat(format, formatinfo);

            while (format.Length > 0)
            {
                int elementLength = GetGroupLengthByMask(format);

                switch (format[0])
                {
                    case '"':
                    case '\'':
                        {
                            int closingQuotePosition = format.IndexOf(format[0], 1);
                            Collection.Add(new DateTimeProperties()
                            {
                                IsReadOnly = true,
                                Type = DateTimeType.others,
                                Lenghth = 1,
                                Content = format.Substring(1, Math.Max(1, closingQuotePosition - 1)).ToString(),
                            });
                            elementLength = Math.Max(1, closingQuotePosition + 1);
                            break;
                        }

                    case 'D':
                    case 'd':
                        string d = format.Substring(0, elementLength);
                        if (elementLength == 1)
                            d = "%" + d;

                        if (elementLength > 2)
                        {
                            Collection.Add(new DateTimeProperties()
                            {
                                IsReadOnly = true,
                                Type = DateTimeType.Dayname,
                                Pattern = d
                            });
                        }
                        else
                        {
                            Collection.Add(new DateTimeProperties()
                            {
                                IsReadOnly = false,
                                Type = DateTimeType.Day,
                                Pattern = d
                            });
                        }
                        break;

                    case 'F':
                    case 'f':
                        string f = format.Substring(0, elementLength);
                        if (elementLength == 1)
                            f = "%" + f;

                        Collection.Add(new DateTimeProperties()
                        {
                            IsReadOnly = false,
                            Type = DateTimeType.Fraction,
                            Pattern = f
                        });
                        break;

                    case 'h':
                        string h = format.Substring(0, elementLength);
                        if (elementLength == 1)
                            h = "%" + h;

                        Collection.Add(new DateTimeProperties()
                        {
                            IsReadOnly = false,
                            Type = DateTimeType.Hour12,
                            Pattern = h
                        });
                        break;

                    case 'H':
                        string H = format.Substring(0, elementLength);
                        if (elementLength == 1)
                            H = "%" + H;

                        Collection.Add(new DateTimeProperties()
                        {
                            IsReadOnly = false,
                            Type = DateTimeType.Hour24,
                            Pattern = H
                        });
                        break;

                    case 'M':
                        //if (elementLength > 4)
                        //{
                        //    elementLength = 4;
                        //}

                        string M = format.Substring(0, elementLength);
                        if (elementLength == 1)
                            M = "%" + M;

                        if (elementLength >= 3)
                        {
                            Collection.Add(new DateTimeProperties()
                            {
                                IsReadOnly = false,
                                Type = DateTimeType.monthname,
                                Pattern = M
                            });
                        }
                        else
                        {
                            Collection.Add(new DateTimeProperties()
                            {
                                IsReadOnly = false,
                                Type = DateTimeType.Month,
                                Pattern = M
                            });
                        }
                        break;

                    case 'S':
                    case 's':
                        string s = format.Substring(0, elementLength);
                        if (elementLength == 1)
                            s = "%" + s;
                        //if (elementLength > 2)
                        //{
                        //    elementLength = 2;
                        //}
                        Collection.Add(new DateTimeProperties()
                        {
                            IsReadOnly = false,
                            Type = DateTimeType.Second,
                            Pattern = s
                        });
                        break;

                    case 'T':
                    case 't':
                        string t = format.Substring(0, elementLength);
                        if (elementLength == 1)
                        {
                            t = "%" + t;
                        }
                        Collection.Add(new DateTimeProperties()
                        {
                            IsReadOnly = false,
                            Type = DateTimeType.designator,
                            Pattern = t
                        });
                        break;

                    case 'Y':
                    case 'y':
                        string y = format.Substring(0, elementLength);
                        if (elementLength == 1)
                        {
                            y = "%" + y;
                        }
                        Collection.Add(new DateTimeProperties()
                        {
                            IsReadOnly = false,
                            Type = DateTimeType.year,
                            Pattern = y
                        });
                        break;

                    case '\\':
                        //string y = format.Substring(0, elementLength);
                        if (format.Length >= 2)
                        {
                            Collection.Add(new DateTimeProperties()
                            {
                                IsReadOnly = true,
                                Content = format.Substring(1, 1),
                                Lenghth = 1,
                                Type = DateTimeType.others
                            });
                            elementLength = 2;
                        }
                        break;

                    case 'g':
                        string g = format.Substring(0, elementLength);
                        if (elementLength == 1)
                        {
                            g = "%" + g;
                        }
                        Collection.Add(new DateTimeProperties()
                        {
                            IsReadOnly = true,
                            Type = DateTimeType.period,
                            Pattern = format.Substring(0, elementLength)
                        });
                        break;

                    case 'm':
                        string m = format.Substring(0, elementLength);
                        if (elementLength == 1)
                        {
                            m = "%" + m;
                        }
                        Collection.Add(new DateTimeProperties()
                        {
                            IsReadOnly = false,
                            Type = DateTimeType.Minutes,
                            Pattern = m
                        });
                        break;

                    case 'z':
                        string z = format.Substring(0, elementLength);
                        if (elementLength == 1)
                        {
                            z = "%" + z;
                        }
                        Collection.Add(new DateTimeProperties()
                        {
                            IsReadOnly = true,
                            Type = DateTimeType.TimeZone,
                            Pattern = z
                        });
                        break;

                    default:
                        elementLength = 1;
                        Collection.Add(new DateTimeProperties()
                        {
                            IsReadOnly = true,
                            Lenghth = 1,
                            Content = format[0].ToString(),
                            Type = DateTimeType.others
                        });
                        break;
                    //Collection.Add(new DateTimeProperties()
                    //{
                    //    IsReadOnly = true,
                    //    Lenght = elementLength,
                    //    Type = DateTimeType.others,
                    //    Content = 
                    //});
                }
                format = format.Substring(elementLength);
            }
            return Collection;
        }

        private static int GetGroupLengthByMask(string mask)
        {
            for (int i = 1; i < mask.Length; i++)
            {
                if (mask[i] != mask[0])
                {
                    return i;
                }
            }
            return mask.Length;
        }

        private static string GetSpecificFormat(string format, DateTimeFormatInfo info)
        {
            if ((format == null) || (format.Length == 0))
            {
                format = "G";
            }
            if (format.Length == 1)
            {
                switch (format[0])
                {
                    case 'd':
                        return info.ShortDatePattern;

                    case 'D':
                        return info.LongDatePattern;

                    case 'f':
                        return (info.LongDatePattern + ' ' + info.ShortTimePattern);

                    case 'F':
                        return info.FullDateTimePattern;

                    case 'g':
                        return (info.ShortDatePattern + ' ' + info.ShortTimePattern);

                    case 'G':
                        return (info.ShortDatePattern + ' ' + info.LongTimePattern);

                    case 'M':
                    case 'm':
                        return info.MonthDayPattern;

                    case 'R':
                    case 'r':
                        return info.RFC1123Pattern;

                    case 's':
                        return info.SortableDateTimePattern;

                    case 't':
                        return info.ShortTimePattern;

                    case 'T':
                        return info.LongTimePattern;

                    case 'u':
                        return info.UniversalSortableDateTimePattern;

                    case 'y':
                    case 'Y':
                        return info.YearMonthPattern;
                }
            }
            if ((format.Length == 2) && (format[0] == '%'))
            {
                format = format.Substring(1);
            }
            return format;
        }

        public string CreateDisplayText(MaskedTextBoxAdv MaskedTextBoxAdv)
        {
            string displayText = string.Empty;
            DateTimeFormatInfo formatinfo = MaskedTextBoxAdv.GetCulture().DateTimeFormat as DateTimeFormatInfo;
            for (int i = 0; i < MaskedTextBoxAdv.DateTimeProperties.Count; i++)
            {
                var charProperty = MaskedTextBoxAdv.DateTimeProperties[i];

                if (charProperty.Pattern == null)
                {
                    MaskedTextBoxAdv.DateTimeProperties[i].StartPosition = displayText.Length;
                    MaskedTextBoxAdv.DateTimeProperties[i].Lenghth = charProperty.Content.Length;
                    displayText += charProperty.Content;
                }
                else
                {
                    DateTime preVal;
                    DateTime.TryParse(MaskedTextBoxAdv.mValue.ToString(), out preVal);

                    preVal = (DateTime)MaskedTextBoxAdv.mValue;
                    MaskedTextBoxAdv.DateTimeProperties[i].StartPosition = displayText.Length;

                    //charProperty.Content = MaskedTextBoxAdv.mValue.ToString(charProperty.Pattern, formatinfo);
                    charProperty.Content = preVal.ToString(charProperty.Pattern, formatinfo);
                    //charProperty.Content = preVal.ToString(charProperty.Pattern, formatinfo);
                    MaskedTextBoxAdv.DateTimeProperties[i].Lenghth = charProperty.Content.Length;
                    displayText += charProperty.Content;
                }
            }
            return displayText;
        }

        public void HandleSelection(MaskedTextBoxAdv MaskedTextBoxAdv)
        {
            for (int i = 0; i < MaskedTextBoxAdv.DateTimeProperties.Count; i++)
            {
                var charProperty = MaskedTextBoxAdv.DateTimeProperties[i];
                if ((charProperty.StartPosition <= MaskedTextBoxAdv.SelectionStart) && (MaskedTextBoxAdv.SelectionStart < (charProperty.StartPosition + charProperty.Lenghth)))
                {
                    if (charProperty.IsReadOnly == false)
                    {
                        MaskedTextBoxAdv.selectionChanged = false;
                        MaskedTextBoxAdv.Select(charProperty.StartPosition, charProperty.Lenghth);
#if WPF
                        MaskedTextBoxAdv.selectionChanged = true;
#endif
                        MaskedTextBoxAdv.mSelectedCollection = i;
                        return;
                    }
                    else
                    {
                        MaskedTextBoxAdv.selectionChanged = false;
                        MaskedTextBoxAdv.Select(charProperty.StartPosition, 0);
#if WPF
                        MaskedTextBoxAdv.selectionChanged = true;
#endif
                        MaskedTextBoxAdv.mSelectedCollection = i;
                        return;
                    }
                }
            }
            MaskedTextBoxAdv.selectionChanged = false;
            MaskedTextBoxAdv.Select(MaskedTextBoxAdv.SelectionStart, 0);
#if WPF
            MaskedTextBoxAdv.selectionChanged = true;
#endif
            MaskedTextBoxAdv.mSelectedCollection = -1;
            return;
        }

        //private static int ParseRepeatPattern(string format, int pos, char patternChar)
        //{
        //    int length = format.Length;
        //    int num2 = pos + 1;
        //    while ((num2 < length) && (format[num2] == patternChar))
        //    {
        //        num2++;
        //    }
        //    return (num2 - pos);
        //}

        //private static int ParseNextChar(string format, int pos)
        //{
        //    if (pos >= (format.Length - 1))
        //    {
        //        return -1;
        //    }
        //    return format[pos + 1];
        //}
    }
}

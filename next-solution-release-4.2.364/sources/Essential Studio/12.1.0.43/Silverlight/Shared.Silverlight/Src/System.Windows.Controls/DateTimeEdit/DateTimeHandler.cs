#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Input;

#if WPF

namespace Syncfusion.Windows.Shared
#endif

#if SILVERLIGHT
namespace Syncfusion.Windows.Tools.Controls
#endif
{
    internal class DateTimeHandler
    {
        internal static char[] allStandardFormats = new char[] {
            'd', 'D', 'f', 'F', 'g', 'G', 'm', 'M', 'o', 'O', 'r', 'R', 's', 't', 'T', 'u',
            'U', 'y', 'Y'
         };

        public static DateTimeHandler dateTimeHandler = new DateTimeHandler();
        public static bool selectedflag = false;

        public ObservableCollection<DateTimeProperties> CreateDateTimePatteren(DateTimeEdit MaskedTextBoxAdv)
        {
            var Collection = new ObservableCollection<DateTimeProperties>();

            DateTimeFormatInfo formatinfo = MaskedTextBoxAdv.GetCulture().DateTimeFormat.Clone() as DateTimeFormatInfo;
            string format = MaskedTextBoxAdv.GetStringPattern(formatinfo, MaskedTextBoxAdv.Pattern, MaskedTextBoxAdv.CustomPattern);
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

        public string CreateDisplayText(DateTimeEdit datetimeeditobj)
        {
            string displayText = string.Empty;

            DateTimeFormatInfo formatinfo = datetimeeditobj.GetCulture().DateTimeFormat as DateTimeFormatInfo;
            for (int i = 0; i < datetimeeditobj.DateTimeProperties.Count; i++)
            {
                var charProperty = datetimeeditobj.DateTimeProperties[i];

                if (charProperty.Pattern == null)
                {
                    datetimeeditobj.DateTimeProperties[i].StartPosition = displayText.Length;
                    datetimeeditobj.DateTimeProperties[i].Lenghth = charProperty.Content.Length;
                    displayText += charProperty.Content;
                }
                else
                {
                    DateTime preVal;
                    if (datetimeeditobj.mValue == null)
                        DateTime.TryParse(datetimeeditobj.DateTime.ToString(), out preVal);
                    else
                        DateTime.TryParse(datetimeeditobj.mValue.ToString(), out preVal);
                    if (datetimeeditobj.mValue != null)
                        preVal = (DateTime)datetimeeditobj.mValue;
                    datetimeeditobj.DateTimeProperties[i].StartPosition = displayText.Length;

                    charProperty.Content = preVal.ToString(charProperty.Pattern, formatinfo);
                    datetimeeditobj.DateTimeProperties[i].Lenghth = charProperty.Content.Length;
                    displayText += charProperty.Content;
                }
            }
            return displayText;
        }

        public void HandleSelection(DateTimeEdit datetimeeditobj)
        {
            if (datetimeeditobj.IsEditable == true && !datetimeeditobj.CanEdit)
            {
                if (datetimeeditobj.SelectWholeContent == true && datetimeeditobj.SelectionLength == datetimeeditobj.Text.Length)
                {
                    if ((ModifierKeys.Shift == Keyboard.Modifiers) || !selectedflag)
                    {
                        datetimeeditobj.selectionChanged = false;
                        datetimeeditobj.SelectAll();
                        selectedflag = true;
                        datetimeeditobj.selectionChanged = true;
                        return;
                    }
                }
                else
                {
                    for (int i = 0; i < datetimeeditobj.DateTimeProperties.Count; i++)
                    {
                        var charProperty = datetimeeditobj.DateTimeProperties[i];

                        if ((charProperty.StartPosition <= datetimeeditobj.SelectionStart) && (datetimeeditobj.SelectionStart < (charProperty.StartPosition + charProperty.Lenghth)))
                        {
                            if (charProperty.IsReadOnly == false)
                            {
                                datetimeeditobj.selectionChanged = false;

                                for (int j = 0; j < datetimeeditobj.DateTimeProperties.Count; j++)
                                {
#if WPF
                                    datetimeeditobj.DateTimeProperties[j].KeyPressCount = 0;
#endif
                                }

                                datetimeeditobj.Select(charProperty.StartPosition, charProperty.Lenghth);
                                if (i + 1 < datetimeeditobj.DateTimeProperties.Count && datetimeeditobj.SelectedText.Contains(datetimeeditobj.DateTimeProperties[i + 1].Content))
                                {
                                    datetimeeditobj.selectionChanged = false;
                                    datetimeeditobj.Select(charProperty.StartPosition, charProperty.Lenghth - datetimeeditobj.DateTimeProperties[i + 1].Content.Length);
                                }
#if WPF
                                datetimeeditobj.selectionChanged = true;
#endif
                                if (i >= 0 && datetimeeditobj.mTextInputpartended)
                                {
                                    datetimeeditobj.mSelectedCollection = i;
                                }
                                return;
                            }
                            else
                            {
                                datetimeeditobj.selectionChanged = false;
                                for (int j = 0; j < datetimeeditobj.DateTimeProperties.Count; j++)
                                {
                                    datetimeeditobj.DateTimeProperties[j].KeyPressCount = 0;
                                }
#if WPF
                                if (charProperty.Content == datetimeeditobj.GetCulture().DateTimeFormat.DateSeparator)
                                {
                                    datetimeeditobj.SelectionLength = 0;
                                }
                                else
#endif
                                {
                                    datetimeeditobj.Select(charProperty.StartPosition, charProperty.Lenghth);
                                }
#if WPF
                                datetimeeditobj.selectionChanged = true;
#endif
                                if (i >= 0 && datetimeeditobj.mTextInputpartended)
                                {
                                    datetimeeditobj.mSelectedCollection = i;
                                }
                                return;
                            }
                        }
                    }
                    datetimeeditobj.selectionChanged = false;
                    datetimeeditobj.Select(datetimeeditobj.SelectionStart, 0);
#if WPF
                    datetimeeditobj.selectionChanged = true;
#endif
                    datetimeeditobj.mSelectedCollection = 0;
                }
                return;
            }
        }

        public bool MatchWithMask(DateTimeEdit datetimeeditobj, string text)
        {
            if (!Regex.IsMatch(text, "[0-9]{1,}") || datetimeeditobj.IsReadOnly)
            {
                return true;
            }
            for (int i = 0; i < datetimeeditobj.DateTimeProperties.Count; i++)
            {
                DateTimeFormatInfo formatinfo = datetimeeditobj.GetCulture().DateTimeFormat.Clone() as DateTimeFormatInfo;
                var charProperty = datetimeeditobj.DateTimeProperties[i];
                if ((charProperty.StartPosition <= datetimeeditobj.SelectionStart) && (datetimeeditobj.SelectionStart <= (charProperty.StartPosition + charProperty.Lenghth - 1)))
                {
                    #region YEAR

                    if (charProperty.IsReadOnly == false && charProperty.Type == DateTimeType.year)
                    {
                        DateTime date;
                        date = (DateTime)datetimeeditobj.mValue;

                        datetimeeditobj.mTextInputpartended = false;
                        int year;
                        string oldtext = (date.Year.ToString());
                        if (datetimeeditobj.DateTimeProperties[i].Pattern == "yy" || datetimeeditobj.DateTimeProperties[i].Pattern == "YY")
                        {
                            if (datetimeeditobj.checkyear == true)
                            {
                                try
                                {
                                    oldtext = datetimeeditobj.checkyear1.Remove(datetimeeditobj.DateTimeProperties[i].KeyPressCount + 2, 1);
                                    text = oldtext.Insert(datetimeeditobj.DateTimeProperties[i].KeyPressCount + 2, text);
                                }
                                catch
                                {
                                }
                                datetimeeditobj.checkyear = false;
                            }
                            else
                            {
                                try
                                {
                                    oldtext = oldtext.Remove(datetimeeditobj.DateTimeProperties[i].KeyPressCount + 2, 1);
                                    text = oldtext.Insert(datetimeeditobj.DateTimeProperties[i].KeyPressCount + 2, text);
                                }
                                catch
                                {
                                }
                            }
                        }
                        else
                        {
                            if (datetimeeditobj.checkyear == true)
                            {
                                try
                                {
                                    oldtext = datetimeeditobj.checkyear1.Remove(datetimeeditobj.DateTimeProperties[i].KeyPressCount, 1);
                                    text = oldtext.Insert(datetimeeditobj.DateTimeProperties[i].KeyPressCount, text);
                                }
                                catch
                                {
                                }
                                datetimeeditobj.checkyear = false;
                            }
                            else
                            {
                                if (oldtext.Length <= 1)
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        oldtext = oldtext.Insert(j, "0");
                                    }
                                }
                                try
                                {
                                    oldtext = oldtext.Remove(datetimeeditobj.DateTimeProperties[i].KeyPressCount, 1);
                                    text = oldtext.Insert(datetimeeditobj.DateTimeProperties[i].KeyPressCount, text);
                                }
                                catch
                                {
                                }
                            }
                        }

                        int.TryParse(text, out year);
                        if (datetimeeditobj.CultureInfo.Name != "ar-SA")
                            year = formatinfo.Calendar.ToFourDigitYear(year);
                        if (year > 0)
                            year = year - date.Year;
                        date = date.AddYears(year);

                        int temp = datetimeeditobj.DateTimeProperties[i].KeyPressCount;

                        datetimeeditobj.mValueChanged = false;
                        if (int.Parse(datetimeeditobj.MaxDateTime.Year.ToString()) <= int.Parse(text) || int.Parse(datetimeeditobj.MinDateTime.Year.ToString()) >= int.Parse(text))
                        {
                            datetimeeditobj.checkyear1 = text;
                            datetimeeditobj.checkyear = true;
                            if (datetimeeditobj.DateTimeProperties[i].KeyPressCount == 3)
                            {
                                datetimeeditobj.DateTime = date;
                            }
                            else if (datetimeeditobj.DateTimeProperties[i].Pattern == "yy" || datetimeeditobj.DateTimeProperties[i].Pattern == "YY")
                            {
                                if (datetimeeditobj.DateTimeProperties[i].KeyPressCount == 1)
                                    datetimeeditobj.DateTime = date;
                            }
                        }
                        else
                        {
                            datetimeeditobj.DateTime = date;
                        }

                        datetimeeditobj.mValueChanged = true;

                        datetimeeditobj.DateTimeProperties[i].KeyPressCount = temp + 1;
                        if (datetimeeditobj.DateTimeProperties[i].Pattern == "yy" || datetimeeditobj.DateTimeProperties[i].Pattern == "YY")
                        {
                            if (datetimeeditobj.DateTimeProperties[i].KeyPressCount == 2)
                            {
                                datetimeeditobj.mTextInputpartended = true;
                                datetimeeditobj.DateTimeProperties[i].KeyPressCount = 0;
                                KeyHandler.keyHandler.HandleRightKey(datetimeeditobj);
                            }
                        }
                        else if (datetimeeditobj.DateTimeProperties[i].KeyPressCount == 4)
                        {
                            datetimeeditobj.mTextInputpartended = true;
                            datetimeeditobj.DateTimeProperties[i].KeyPressCount = 0;
                            KeyHandler.keyHandler.HandleRightKey(datetimeeditobj);
                        }

                        return true;
                    }

                    #endregion YEAR

                    #region Minutes

                    else if (charProperty.Type == DateTimeType.Minutes)
                    {
                        DateTime date;
                        date = (DateTime)datetimeeditobj.mValue;

                        datetimeeditobj.mTextInputpartended = false;
                        int min;
                        if ((text + date.Minute.ToString()).Length > 2)
                        {
                            text = (date.Minute.ToString() + text);
                            text = text.Substring(text.Length - 2, 2);
                        }
                        else
                            text = (date.Minute.ToString() + text);
                        int.TryParse(text, out min);

                        if (min > 59)
                            text = text.Substring(text.Length - 1, 1);
                        int.TryParse(text, out min);

                        min = min - date.Minute;
                        date = date.AddMinutes(min);
                        int temp = datetimeeditobj.DateTimeProperties[i].KeyPressCount;

                        datetimeeditobj.mValueChanged = false;
                        datetimeeditobj.DateTime = date;
                        datetimeeditobj.mValueChanged = true;
                        datetimeeditobj.DateTimeProperties[i].KeyPressCount = temp + 1;
                        if (datetimeeditobj.DateTimeProperties[i].KeyPressCount == 2)
                        {
                            datetimeeditobj.mTextInputpartended = true;
                            datetimeeditobj.DateTimeProperties[i].KeyPressCount = 0;
                            KeyHandler.keyHandler.HandleRightKey(datetimeeditobj);
                        }

                        return true;
                    }

                    #endregion Minutes

                    #region Seconds

                    else if (charProperty.Type == DateTimeType.Second)
                    {
                        DateTime date;
                        date = (DateTime)datetimeeditobj.mValue;

                        datetimeeditobj.mTextInputpartended = false;
                        int lenghth = charProperty.Lenghth;
                        if (lenghth < 2)
                            lenghth = 2;

                        if ((text + date.Second.ToString()).Length > lenghth)
                        {
                            text = (date.Second.ToString() + text);
                            text = text.Substring(text.Length - lenghth, lenghth);
                        }
                        else
                            text = (date.Second.ToString() + text);

                        int sec;
                        int.TryParse(text, out sec);
                        if (sec > 59)
                            text = text.Substring(text.Length - 1, 1);
                        int.TryParse(text, out sec);

                        sec = sec - date.Second;
                        date = date.AddSeconds(sec);

                        int temp = datetimeeditobj.DateTimeProperties[i].KeyPressCount;

                        datetimeeditobj.mValueChanged = false;
                        datetimeeditobj.DateTime = date;
                        datetimeeditobj.mValueChanged = true;

                        datetimeeditobj.DateTimeProperties[i].KeyPressCount = temp + 1;
                        if (datetimeeditobj.DateTimeProperties[i].KeyPressCount == lenghth)
                        {
                            datetimeeditobj.mTextInputpartended = true;
                            datetimeeditobj.DateTimeProperties[i].KeyPressCount = 0;
                            KeyHandler.keyHandler.HandleRightKey(datetimeeditobj);
                        }

                        return true;
                    }

                    #endregion Seconds

                    #region Hour

                    else if (charProperty.Type == DateTimeType.Hour24 || charProperty.Type == DateTimeType.Hour12)
                    {
                        DateTime date;
                        date = (DateTime)datetimeeditobj.mValue;

                        datetimeeditobj.mTextInputpartended = false;
                        int lenghth = charProperty.Lenghth;
                        if (lenghth < 2)
                            lenghth = 2;

                        if ((text + date.Hour.ToString()).Length > lenghth)
                        {
                            text = (date.Hour.ToString() + text);
                            text = text.Substring(text.Length - lenghth, lenghth);
                        }
                        else
                            text = (date.Hour.ToString() + text);

                        int hour;
                        int.TryParse(text, out hour);
                        if (hour > (charProperty.Type == DateTimeType.Hour12 ? 12 : 23))
                            text = text.Substring(text.Length - 1, 1);
                        int.TryParse(text, out hour);

                        if (charProperty.Type == DateTimeType.Hour12 && date.Hour > 12)
                        {
                            hour += 12;
                        }
                        hour = hour - date.Hour;

                        date = date.AddHours(hour);

                        int temp = datetimeeditobj.DateTimeProperties[i].KeyPressCount;

                        datetimeeditobj.mValueChanged = false;
                        datetimeeditobj.DateTime = date;
                        datetimeeditobj.mValueChanged = true;

                        datetimeeditobj.DateTimeProperties[i].KeyPressCount = temp + 1;
                        if (datetimeeditobj.DateTimeProperties[i].KeyPressCount == lenghth)
                        {
                            datetimeeditobj.mTextInputpartended = true;
                            datetimeeditobj.DateTimeProperties[i].KeyPressCount = 0;
                            KeyHandler.keyHandler.HandleRightKey(datetimeeditobj);
                        }
                        return true;
                    }

                    #endregion Hour

                    #region Fraction

                    else if (charProperty.Type == DateTimeType.Fraction)
                    {
                        DateTime date;
                        date = (DateTime)datetimeeditobj.mValue;

                        int fraction;
                        if ((text + date.Millisecond.ToString()).Length > 3)//charProperty.Lenghth
                        {
                            text = (date.Millisecond.ToString() + text);
                            text = text.Substring(text.Length - 3, 3);
                        }
                        else
                            text = (date.Millisecond.ToString() + text);

                        int.TryParse(text, out fraction);

                        fraction = fraction - date.Millisecond;
                        date = date.AddMilliseconds(fraction);

                        datetimeeditobj.mValueChanged = false;
                        datetimeeditobj.DateTime = date;
                        datetimeeditobj.mValueChanged = true;
                        return true;
                    }

                    #endregion Fraction

                    #region Month

                    else if (charProperty.Type == DateTimeType.Month || charProperty.Type == DateTimeType.monthname)
                    {
                        for (int j = 0; j < datetimeeditobj.DateTimeProperties.Count; j++)
                        {
                            if (j != i)
                            {
                                datetimeeditobj.DateTimeProperties[j].KeyPressCount = 0;
                            }
                        }
                        DateTime date;
                        DateTime.TryParse(datetimeeditobj.mValue.ToString(), out date);

                        datetimeeditobj.mTextInputpartended = false;
                        if ((text + date.Month.ToString()).Length > charProperty.Lenghth)
                        {
                            text = (date.Month.ToString() + text);
                            text = text.Substring(text.Length - charProperty.Lenghth, charProperty.Lenghth);
                        }
                        else
                            text = (date.Month.ToString() + text);
                        if ((text + date.Month.ToString()).Length > 2)
                        {
                            text = (date.Month.ToString() + text);
                            text = text.Substring(text.Length - 2, 2);
                        }
                        else
                            text = (date.Month.ToString() + text);
                        string oldtext = date.Month.ToString();
                        if (oldtext.Length > 1)
                        {
                            if (datetimeeditobj.checkmonth == true && datetimeeditobj.DateTimeProperties[i].KeyPressCount == 1)
                            {
                                if ((oldtext == "11") && (text == "1" || text == "2"))
                                {
                                    oldtext = oldtext.Remove(datetimeeditobj.DateTimeProperties[i].KeyPressCount - 1, 2);
                                    text = oldtext.Insert(datetimeeditobj.DateTimeProperties[i].KeyPressCount - 1, text);
                                    datetimeeditobj.checkmonth = false;
                                }
                                else
                                {
                                    oldtext = oldtext.Replace(oldtext[datetimeeditobj.DateTimeProperties[i].KeyPressCount - 1], oldtext[datetimeeditobj.DateTimeProperties[i].KeyPressCount]);
                                    oldtext = oldtext.Remove(datetimeeditobj.DateTimeProperties[i].KeyPressCount, 1);
                                    text = oldtext.Insert(datetimeeditobj.DateTimeProperties[i].KeyPressCount, text);
                                    datetimeeditobj.checkmonth = false;
                                }
                            }
                            else
                            {
                                if (((oldtext == "12" && text == "0") || (oldtext == "11" && text == "0") || (oldtext == "10" && text == "0")) && datetimeeditobj.DateTimeProperties[i].KeyPressCount == 0)
                                {
                                    datetimeeditobj.checkmonth = true;
                                }
                                else
                                {
                                    if (datetimeeditobj.DateTimeProperties[i].KeyPressCount == 0)
                                    {
                                        oldtext = oldtext.Remove(datetimeeditobj.DateTimeProperties[i].KeyPressCount, 2);
                                        text = oldtext.Insert(datetimeeditobj.DateTimeProperties[i].KeyPressCount, text);
                                    }
                                    else
                                    {
                                        oldtext = oldtext.Remove(datetimeeditobj.DateTimeProperties[i].KeyPressCount, 1);
                                        if (datetimeeditobj.checkmonth == true)
                                        {
                                        }
                                        if (int.Parse(oldtext) == 0 && int.Parse(text) == 0)
                                            datetimeeditobj.checkmonth = true;
                                        text = oldtext.Insert(datetimeeditobj.DateTimeProperties[i].KeyPressCount, text);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (datetimeeditobj.DateTimeProperties[i].KeyPressCount == 0)
                            {
                                oldtext.Replace(oldtext[datetimeeditobj.DateTimeProperties[i].KeyPressCount], text[datetimeeditobj.DateTimeProperties[i].KeyPressCount]);
                            }
                            if (datetimeeditobj.DateTimeProperties[i].KeyPressCount == 1)
                            {
                            }
                        }

                        ////new
                        int month;
                        int.TryParse(text, out month);

                        if (text.Length == 1)
                        {
                            int newmonth;
                            datetimeeditobj.checktext = datetimeeditobj.checktext + text;
                            int.TryParse(datetimeeditobj.checktext, out newmonth);
                            if (newmonth > month)
                                int.TryParse(datetimeeditobj.checktext, out month);
                            text = datetimeeditobj.checktext;
                        }
                        if (month > 12)
                        {
                            if (text.Length >= 3)
                                text = text.Substring(text.Length - (text.Length - 1), 2);
                            if (Int32.Parse(text) > 12)
                            {
                                text = text.Substring(text.Length - 1, 1);
                            }
                        }
                        if (datetimeeditobj.DateTimeProperties[i].KeyPressCount == 0)
                        {
                            if (month.ToString().Length == datetimeeditobj.DateTimeProperties[i].Lenghth)
                            {
                                text = text.Substring(text.Length - 1, 1);
                            }
                        }
                        ////

                        int.TryParse(text, out month);
                        if (month > 0)
                            month = month - date.Month;
                        date = date.AddMonths(month);

                        int temp = datetimeeditobj.DateTimeProperties[i].KeyPressCount;

                        datetimeeditobj.mValueChanged = false;
                        datetimeeditobj.DateTime = date;
                        datetimeeditobj.mValueChanged = true;

                        datetimeeditobj.Select(datetimeeditobj.SelectionStart, 0);
                        datetimeeditobj.selectionChanged = true;

                        datetimeeditobj.DateTimeProperties[i].KeyPressCount = temp + 1;
                        if (datetimeeditobj.DateTimeProperties[i].KeyPressCount == 2 || text.Length == 2)
                        {
                            datetimeeditobj.mTextInputpartended = true;
                            datetimeeditobj.DateTimeProperties[i].KeyPressCount = 0;
                            datetimeeditobj.checktext = "";
                            KeyHandler.keyHandler.HandleRightKey(datetimeeditobj);
                        }
                        return true;
                    }

                    #endregion Month

                    #region Day

                    else if (charProperty.Type == DateTimeType.Day)
                    {
                        DateTime date;
                        DateTime.TryParse(datetimeeditobj.mValue.ToString(), out date);
                        datetimeeditobj.mTextInputpartended = false;
                        int day;
                        if ((text + date.Day.ToString()).Length > charProperty.Lenghth)
                        {
                            text = (date.Day.ToString() + text);
                            text = text.Substring(text.Length - charProperty.Lenghth, charProperty.Lenghth);
                        }
                        else
                            text = (date.Day.ToString() + text);
                        if ((text + date.Day.ToString()).Length > 2)
                        {
                            text = (date.Day.ToString() + text);
                            text = text.Substring(text.Length - 2, 2);
                        }
                        else
                            text = (date.Day.ToString() + text);
                        string oldtext = date.Day.ToString();
                        if (oldtext.Length > 1)
                        {
                            if (int.Parse(oldtext) >= 12 && int.Parse(oldtext) <= 29 && int.Parse(text) >= 3 && datetimeeditobj.DateTimeProperties[i].KeyPressCount == 0)
                            {
                                oldtext = oldtext.Remove(datetimeeditobj.DateTimeProperties[i].KeyPressCount, 2);
                                text = oldtext.Insert(datetimeeditobj.DateTimeProperties[i].KeyPressCount, text);
                            }
                            else if (int.Parse(oldtext) >= 30 && datetimeeditobj.DateTimeProperties[i].KeyPressCount == 0 && int.Parse(text) == 3)
                            {
                                text = oldtext;
                                datetimeeditobj.checkday1 = true;
                            }
                            else
                            {
                                if (datetimeeditobj.checkday == true && datetimeeditobj.DateTimeProperties[i].KeyPressCount != 0)
                                {
                                    oldtext = oldtext.Replace(oldtext[datetimeeditobj.DateTimeProperties[i].KeyPressCount - 1], oldtext[datetimeeditobj.DateTimeProperties[i].KeyPressCount]);
                                    oldtext = oldtext.Remove(datetimeeditobj.DateTimeProperties[i].KeyPressCount, 1);
                                    text = oldtext.Insert(datetimeeditobj.DateTimeProperties[i].KeyPressCount, text);
                                    datetimeeditobj.checkday = false;
                                }
                                else if (datetimeeditobj.checkday1 == true && datetimeeditobj.DateTimeProperties[i].KeyPressCount != 0)
                                {
                                    oldtext = oldtext.Replace(oldtext[datetimeeditobj.DateTimeProperties[i].KeyPressCount], text[datetimeeditobj.DateTimeProperties[i].KeyPressCount - 1]);
                                    text = oldtext;
                                    datetimeeditobj.checkday1 = false;
                                }
                                else
                                {
                                    if (datetimeeditobj.checkday2 == true && datetimeeditobj.DateTimeProperties[i].KeyPressCount == 1)
                                    {
                                        oldtext = oldtext.Replace(oldtext[datetimeeditobj.DateTimeProperties[i].KeyPressCount - 1], oldtext[datetimeeditobj.DateTimeProperties[i].KeyPressCount]);
                                        oldtext = oldtext.Remove(datetimeeditobj.DateTimeProperties[i].KeyPressCount, 1);
                                        text = oldtext.Insert(datetimeeditobj.DateTimeProperties[i].KeyPressCount, text);
                                        datetimeeditobj.checkday2 = false;
                                    }
                                    else
                                    {
                                        if (datetimeeditobj.DateTimeProperties[i].KeyPressCount == 0)
                                        {
                                            oldtext = oldtext.Remove(datetimeeditobj.DateTimeProperties[i].KeyPressCount, 2);
                                            text = oldtext.Insert(datetimeeditobj.DateTimeProperties[i].KeyPressCount, text);
                                        }
                                        else
                                        {
                                            oldtext = oldtext.Remove(datetimeeditobj.DateTimeProperties[i].KeyPressCount - 1, 2);
                                            text = oldtext.Insert(datetimeeditobj.DateTimeProperties[i].KeyPressCount - 1, text);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (datetimeeditobj.DateTimeProperties[i].KeyPressCount == 0)
                            {
                                oldtext.Replace(oldtext[datetimeeditobj.DateTimeProperties[i].KeyPressCount], text[datetimeeditobj.DateTimeProperties[i].KeyPressCount]);
                            }
                            if (datetimeeditobj.DateTimeProperties[i].KeyPressCount == 1)
                            {
                            }
                        }
                        ////new
                        int month;
                        int.TryParse(text, out month);

                        if (text.Length == 1)
                        {
                            int newmonth;
                            datetimeeditobj.checktext = datetimeeditobj.checktext + text;
                            int.TryParse(datetimeeditobj.checktext, out newmonth);
                            if (newmonth > month)
                                int.TryParse(datetimeeditobj.checktext, out month);
                            text = datetimeeditobj.checktext;
                        }

                        if (month > DateTime.DaysInMonth(date.Year, date.Month))
                            text = text.Substring(text.Length - 1, 1);
                        ////

                        int.TryParse(text, out day);
                        if (day > 0)
                            day = day - date.Day;
                        date = date.AddDays(day);

                        int temp = datetimeeditobj.DateTimeProperties[i].KeyPressCount;

                        datetimeeditobj.mValueChanged = false;
                        datetimeeditobj.DateTime = date;
                        datetimeeditobj.mValueChanged = true;

                        datetimeeditobj.DateTimeProperties[i].KeyPressCount = temp + 1;
                        if (datetimeeditobj.DateTimeProperties[i].KeyPressCount == 2)
                        {
                            datetimeeditobj.mTextInputpartended = true;
                            datetimeeditobj.DateTimeProperties[i].KeyPressCount = 0;
                            datetimeeditobj.checktext = "";
                            KeyHandler.keyHandler.HandleRightKey(datetimeeditobj);
                        }
                        return true;
                    }

                    #endregion Day
                }
            }
            datetimeeditobj.Select(datetimeeditobj.SelectionStart, 0);
            datetimeeditobj.selectionChanged = true;
            return true;
        }
    }
}
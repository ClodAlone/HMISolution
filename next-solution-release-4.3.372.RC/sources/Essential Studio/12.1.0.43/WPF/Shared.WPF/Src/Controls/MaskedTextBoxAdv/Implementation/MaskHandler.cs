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
using System.Text.RegularExpressions;
using System.Globalization;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    internal class MaskHandler
    {
        /// <summary>
        /// 
        /// </summary>
        public static MaskHandler maskHandler = new MaskHandler();

        /// <summary>
        /// Matches the with mask.
        /// </summary>
        /// <param name="MaskedTextBoxAdv">The masked text box.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public bool MatchWithMask(MaskedTextBoxAdv MaskedTextBoxAdv, string text)
        {
            if (MaskedTextBoxAdv.MaskType == MaskType.Standard)
            {
                #region Standard
                int currentcaretPosition = MaskedTextBoxAdv.SelectionStart;
                if (currentcaretPosition == MaskedTextBoxAdv.Text.Length)
                    return true;
                while (currentcaretPosition < MaskedTextBoxAdv.Text.Length)
                {
                    if (MaskedTextBoxAdv.CharCollection[currentcaretPosition].IsPromptCharacter != null)
                    {
                        if (Regex.IsMatch(text, MaskedTextBoxAdv.CharCollection[currentcaretPosition].RegExpression))
                        {
                            string temp = MaskedTextBoxAdv.MaskedText;
                            temp = temp.Remove(currentcaretPosition, 1);
                            temp = temp.Insert(currentcaretPosition, text);
                            MaskedTextBoxAdv.MaskedText = temp;
                            MaskedTextBoxAdv.SelectionStart = currentcaretPosition + 1;
                            MaskedTextBoxAdv.CharCollection[currentcaretPosition].IsPromptCharacter = false;
                            currentcaretPosition++;
                            return true;
                        }
                        else
                        {
                            currentcaretPosition++;
                            return true;
                        }
                    }
                    else
                    {
                        currentcaretPosition++;
                    }
                }
                return true;
                #endregion
            }
            else if (MaskedTextBoxAdv.MaskType == MaskType.Integer)
            {
                #region Integer
                NumberFormatInfo numberFormat = MaskedTextBoxAdv.GetCulture().NumberFormat;
                if (text == "-" || text == "+")
                {
                    Int64 tempVal = (Int64)MaskedTextBoxAdv.mValue * -1;
                    if (tempVal > MaskedTextBoxAdv.IntegerMaxValue)
                    {
                        tempVal = MaskedTextBoxAdv.IntegerMaxValue;
                    }
                    else if (tempVal < MaskedTextBoxAdv.IntegerMinValue)
                    {
                        tempVal = MaskedTextBoxAdv.IntegerMinValue;
                    }
                    MaskedTextBoxAdv.MaskedText = tempVal.ToString("N", numberFormat);
                    return true;
                }
                int SelectionStart = 0;
                int SelectionEnd = 0;
                int SelectionLength = 0;
                int CaretPosition = 0;
                string unmaskedText = "";

                for (int i = 0; i <= MaskedTextBoxAdv.MaskedText.Length; i++)
                {
                    if (i == MaskedTextBoxAdv.SelectionStart)
                    {
                        SelectionStart = unmaskedText.Length;
                        CaretPosition = unmaskedText.Length;
                    }

                    if (i == (MaskedTextBoxAdv.SelectionStart + MaskedTextBoxAdv.SelectionLength))
                        SelectionEnd = unmaskedText.Length;

                    if (i < MaskedTextBoxAdv.MaskedText.Length)
                    {
                        if (char.IsDigit(MaskedTextBoxAdv.MaskedText[i]))
                            unmaskedText += MaskedTextBoxAdv.MaskedText[i];
                    }
                }
                SelectionLength = SelectionEnd - SelectionStart;
                unmaskedText = unmaskedText.Remove(SelectionStart, SelectionLength);
                unmaskedText = unmaskedText.Insert(SelectionStart, text);
                //MaskedTextBoxAdv.MaskedText = unmaskedText;

                Int64 preValue;
                if (Int64.TryParse(unmaskedText, NumberStyles.Number, numberFormat, out preValue))
                {
                    if ((preValue > MaskedTextBoxAdv.IntegerMaxValue) || (preValue < MaskedTextBoxAdv.IntegerMinValue))
                    {
                        return true;
                    }

                    numberFormat.NumberDecimalDigits = 0;
                    //int tempint = MaskedTextBoxAdv.SelectionStart;
                    MaskedTextBoxAdv.MaskedText = preValue.ToString("N", numberFormat);
                }

                int j = 0;
                for (int i = 0; i < SelectionStart; i++)
                {
                    if (char.IsDigit(MaskedTextBoxAdv.MaskedText[j]))
                        j++;
                    else
                    {
                        for (int k = j; k < MaskedTextBoxAdv.MaskedText.Length; k++)
                        {
                            if (char.IsDigit(MaskedTextBoxAdv.Text[k]))
                                break;
                            j++;
                        }
                        j++;
                    }
                }
                MaskedTextBoxAdv.SelectionStart = j + text.Length;
                SelectionStart = j + text.Length;
                MaskedTextBoxAdv.SelectionLength = 0;
                if (MaskedTextBoxAdv.IsNegative)
                {
                    Int64 tempVal = (Int64)MaskEditModel.maskEditorModelHelp.CreateValueFromText(MaskedTextBoxAdv);
                    tempVal = tempVal * -1;
                    MaskedTextBoxAdv.MaskedText = tempVal.ToString("N", numberFormat);
                    MaskedTextBoxAdv.SelectionStart = (SelectionStart + 1) > MaskedTextBoxAdv.Text.Length ? SelectionStart : (SelectionStart + 1);
                }
                
                return true;
                #endregion
            }
            else if (MaskedTextBoxAdv.MaskType == MaskType.Double||MaskedTextBoxAdv.MaskType == MaskType.Currency || MaskedTextBoxAdv.MaskType == MaskType.Percentage)
            {
                #region Number Masks
                NumberFormatInfo numberFormat = MaskedTextBoxAdv.GetCulture().NumberFormat;
                if (text == "-" || text == "+")
                {
                    double tempVal = (double)MaskedTextBoxAdv.mValue * -1;
                    if (tempVal > MaskedTextBoxAdv.MaxValue)
                    {
                        tempVal = MaskedTextBoxAdv.MaxValue;
                    }
                    else if (tempVal < MaskedTextBoxAdv.MinValue)
                    {
                        tempVal = MaskedTextBoxAdv.MinValue;
                    }
                    if (MaskedTextBoxAdv.MaskType == MaskType.Double)
                        MaskedTextBoxAdv.MaskedText = tempVal.ToString("N", numberFormat);
                    else if (MaskedTextBoxAdv.MaskType == MaskType.Currency)
                        MaskedTextBoxAdv.MaskedText = tempVal.ToString("C", numberFormat);
                    else if (MaskedTextBoxAdv.MaskType == MaskType.Percentage)
                    {
                        tempVal = tempVal / 100;
                        MaskedTextBoxAdv.MaskedText = tempVal.ToString("P", numberFormat);
                    }
                    return true;
                }

                string maskedText = MaskedTextBoxAdv.MaskedText;
                int SelectionStart = 0;//MaskedTextBoxAdv.SelectionStart;
                int SelectionEnd = 0;
                int SelectionLength = 0;
                //MaskedTextBoxAdv.SelectionLength;
                int SeparatorStart = MaskedTextBoxAdv.MaskedText.IndexOf(numberFormat.NumberDecimalSeparator);
                int SeparatorEnd = SeparatorStart + numberFormat.NumberDecimalSeparator.Length;

                int CaretPosition = MaskedTextBoxAdv.SelectionStart;
                string unmaskedText = "";

                int i;
                for (i = 0; i <= MaskedTextBoxAdv.MaskedText.Length; i++)
                {
                    if (i == MaskedTextBoxAdv.SelectionStart)
                    {
                        SelectionStart = unmaskedText.Length;
                        CaretPosition = SelectionStart;
                    }
                    if (i == (MaskedTextBoxAdv.SelectionStart + MaskedTextBoxAdv.SelectionLength))
                        SelectionEnd = unmaskedText.Length;//i;//SelectionLength = unmaskedText.Length - SelectionStart;

                    if (i == SeparatorEnd)
                        SeparatorEnd = unmaskedText.Length;

                    if (i == SeparatorStart)
                    {
                        SeparatorStart = unmaskedText.Length;
                        unmaskedText += ".";
                    }

                    if (i < MaskedTextBoxAdv.MaskedText.Length)
                    {
                        if (char.IsDigit(MaskedTextBoxAdv.MaskedText[i]))
                            unmaskedText += MaskedTextBoxAdv.MaskedText[i];
                    }
                }
                SelectionLength = SelectionEnd - SelectionStart;

                if (SelectionStart <= SeparatorStart && SelectionEnd >= SeparatorEnd)
                {
                    for (int decpos = SeparatorEnd; decpos < SelectionEnd; decpos++)
                    {
                        if (decpos != unmaskedText.Length)
                        {
                            unmaskedText = unmaskedText.Remove(decpos, 1);
                            unmaskedText = unmaskedText.Insert(decpos, "0");
                        }
                    }
                    unmaskedText = unmaskedText.Remove(SelectionStart, (SeparatorStart - SelectionStart));
                    CaretPosition = SelectionStart;

                    unmaskedText = unmaskedText.Insert(SelectionStart, text);
                    CaretPosition = CaretPosition + text.Length;
                }
                else if (SelectionStart <= SeparatorStart && SelectionEnd < SeparatorEnd)
                {
                    //unmaskedText = unmaskedText.Remove(SelectionStart, (SeparatorStart - SelectionStart));
                    unmaskedText = unmaskedText.Remove(SelectionStart, SelectionLength);
                    CaretPosition = SelectionStart;

                    unmaskedText = unmaskedText.Insert(SelectionStart, text);
                    CaretPosition = CaretPosition + text.Length;
                }
                else
                {
                    if (SelectionStart == SelectionEnd)
                    {
                        if (SelectionStart != unmaskedText.Length)
                            unmaskedText = unmaskedText.Insert(SelectionStart, text[0].ToString());
                        else
                            return true;
                    }
                    else
                    {
                        int textpos = 0;
                        for (int decpos = SelectionStart; decpos < SelectionEnd; decpos++)
                        {
                            unmaskedText = unmaskedText.Remove(decpos, 1);
                            if (textpos < text.Length)
                                unmaskedText = unmaskedText.Insert(decpos, text[textpos].ToString());
                            else
                                unmaskedText = unmaskedText.Insert(decpos, "0");
                            textpos++;
                        }
                        CaretPosition = SelectionStart + SelectionLength;
                    }
                }

                double preValue;
                if (double.TryParse(unmaskedText, out preValue))
                {
                    if ((preValue > MaskedTextBoxAdv.MaxValue) || (preValue < MaskedTextBoxAdv.MinValue))
                        return true;

                    if (MaskedTextBoxAdv.MaskType == MaskType.Double)
                        MaskedTextBoxAdv.MaskedText = preValue.ToString("N", numberFormat);
                    else if (MaskedTextBoxAdv.MaskType == MaskType.Currency)
                        MaskedTextBoxAdv.MaskedText = preValue.ToString("C", numberFormat);
                    else if (MaskedTextBoxAdv.MaskType == MaskType.Percentage)
                    {
                        preValue = preValue / 100;
                        MaskedTextBoxAdv.MaskedText = preValue.ToString("P", numberFormat);
                    }

                    int j = 0;
                    for (i = 0; i < unmaskedText.Length; i++)
                    {
                        if (i == CaretPosition)
                        {
                            break;
                        }
                        if (j == MaskedTextBoxAdv.MaskedText.Length)
                            break;
                        if (char.IsDigit(MaskedTextBoxAdv.MaskedText[j]))
                            j++;
                        else
                        {
                            for (int k = j; k < MaskedTextBoxAdv.MaskedText.Length; k++)
                            {
                                if (char.IsDigit(MaskedTextBoxAdv.MaskedText[k]))
                                    break;
                                j++;
                            }
                            i--;
                        }
                    }
                    MaskedTextBoxAdv.SelectionStart = j;
                    SelectionStart = j;
                    MaskedTextBoxAdv.SelectionLength = 0;
                }

                if (MaskedTextBoxAdv.IsNegative)
                {
                    double tempVal = (double)MaskEditModel.maskEditorModelHelp.CreateValueFromText(MaskedTextBoxAdv);
                    tempVal = tempVal * -1;
                    if (MaskedTextBoxAdv.MaskType == MaskType.Double)
                        MaskedTextBoxAdv.MaskedText = tempVal.ToString("N", numberFormat);
                    else if (MaskedTextBoxAdv.MaskType == MaskType.Currency)
                        MaskedTextBoxAdv.MaskedText = tempVal.ToString("C", numberFormat);
                    else if (MaskedTextBoxAdv.MaskType == MaskType.Percentage)
                    {
                        tempVal = tempVal / 100;
                        MaskedTextBoxAdv.MaskedText = tempVal.ToString("P", numberFormat);
                    }
                    MaskedTextBoxAdv.SelectionStart = (SelectionStart + 1) > MaskedTextBoxAdv.Text.Length ? SelectionStart : (SelectionStart + 1);
                }
                return true;
                #endregion
            }

            else if (MaskedTextBoxAdv.MaskType == MaskType.DateTime)
            {
                if (!Regex.IsMatch(text, "[0-9]{1,}"))
                {
                    return true;
                }
                for (int i = 0; i < MaskedTextBoxAdv.DateTimeProperties.Count; i++)
                {
                    DateTimeFormatInfo formatinfo = MaskedTextBoxAdv.GetCulture().DateTimeFormat.Clone() as DateTimeFormatInfo;
                    var charProperty = MaskedTextBoxAdv.DateTimeProperties[i];
                    if ((charProperty.StartPosition <= MaskedTextBoxAdv.SelectionStart) && (MaskedTextBoxAdv.SelectionStart <= (charProperty.StartPosition + charProperty.Lenghth - 1)))
                    {
                        #region YEAR
                        if (charProperty.IsReadOnly == false && charProperty.Type == DateTimeType.year)
                        {
                            DateTime date;
                            date = (DateTime)MaskedTextBoxAdv.mValue;
                            //DateTime.TryParse(MaskedTextBoxAdv.mValue.ToString(), out date);

                            int year;
                            if ((text + date.Year.ToString()).Length > 4)
                            {
                                text = (date.Year.ToString() + text);
                                text = text.Substring(text.Length - 4, 4);
                            }
                            else
                                text = (date.Year.ToString() + text);

                            int.TryParse(text, out year);

                            year = formatinfo.Calendar.ToFourDigitYear(year);
                            year = year - date.Year;
                            date = date.AddYears(year);

                            MaskedTextBoxAdv.mValueChanged = false;
                            MaskedTextBoxAdv.Value = date;
                            MaskedTextBoxAdv.mValueChanged = true;
                            return true;
                        }
                        #endregion

                        #region Minutes
                        else if (charProperty.Type == DateTimeType.Minutes)
                        {
                            DateTime date;
                            date = (DateTime)MaskedTextBoxAdv.mValue;
                            //DateTime.TryParse(MaskedTextBoxAdv.mValue.ToString(), out date);

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
                            MaskedTextBoxAdv.mValueChanged = false;
                            MaskedTextBoxAdv.Value = date;
                            MaskedTextBoxAdv.mValueChanged = true;
                            return true;
                        }
                        #endregion

                        #region Seconds
                        else if (charProperty.Type == DateTimeType.Second)
                        {
                            DateTime date;
                            date = (DateTime)MaskedTextBoxAdv.mValue;
                            //DateTime.TryParse(MaskedTextBoxAdv.mValue.ToString(), out date);

                            int sec;
                            if ((text + date.Second.ToString()).Length > charProperty.Lenghth)
                            {
                                text = (date.Second.ToString() + text);
                                text = text.Substring(text.Length - charProperty.Lenghth, charProperty.Lenghth);
                            }
                            else
                                text = (date.Second.ToString() + text);
                            int.TryParse(text, out sec);

                            if (sec > 59)
                                text = text.Substring(text.Length - 1, 1);
                            int.TryParse(text, out sec);

                            sec = sec - date.Second;
                            date = date.AddSeconds(sec);

                            MaskedTextBoxAdv.mValueChanged = false;
                            MaskedTextBoxAdv.Value = date;
                            MaskedTextBoxAdv.mValueChanged = true;
                            return true;
                        }
                        #endregion

                        #region Hour
                        else if (charProperty.Type == DateTimeType.Hour24 || charProperty.Type == DateTimeType.Hour12)
                        {
                            DateTime date;
                            date = (DateTime)MaskedTextBoxAdv.mValue;
                            //DateTime.TryParse(MaskedTextBoxAdv.mValue.ToString(), out date);

                            int hour;
                            if ((text + date.Hour.ToString()).Length > charProperty.Lenghth)
                            {
                                text = (date.Hour.ToString() + text);
                                text = text.Substring(text.Length - charProperty.Lenghth, charProperty.Lenghth);
                            }
                            else
                                text = (date.Hour.ToString() + text);

                            int.TryParse(text, out hour);
                            if (hour > (charProperty.Type == DateTimeType.Hour12 ? 12 : 23))
                                text = text.Substring(text.Length - 1, 1);
                            int.TryParse(text, out hour);

                            hour = hour - date.Hour;
                            date = date.AddHours(hour);

                            MaskedTextBoxAdv.mValueChanged = false;
                            MaskedTextBoxAdv.Value = date;
                            MaskedTextBoxAdv.mValueChanged = true;
                            return true;
                        }
                        #endregion

                        #region Fraction
                        else if (charProperty.Type == DateTimeType.Fraction)
                        {
                            DateTime date;
                            date = (DateTime)MaskedTextBoxAdv.mValue;
                            //DateTime.TryParse(MaskedTextBoxAdv.mValue.ToString(), out date);

                            int fraction;
                            if ((text + date.Millisecond.ToString()).Length > 3)//charProperty.Lenghth
                            {
                                text = (date.Millisecond.ToString() + text);
                                text = text.Substring(text.Length - 3, 3);
                                //text = text.Substring(text.Length - charProperty.Lenghth, charProperty.Lenghth);
                            }
                            else
                                text = (date.Millisecond.ToString() + text);

                            int.TryParse(text, out fraction);

                            fraction = fraction - date.Millisecond;
                            date = date.AddMilliseconds(fraction);

                            MaskedTextBoxAdv.mValueChanged = false;
                            MaskedTextBoxAdv.Value = date;
                            MaskedTextBoxAdv.mValueChanged = true;
                            return true;
                        }
                        #endregion

                        #region Month
                        else if (charProperty.Type == DateTimeType.Month || charProperty.Type == DateTimeType.monthname)
                        {
                            DateTime date;
                            DateTime.TryParse(MaskedTextBoxAdv.mValue.ToString(), out date);

                            int month;
                            if ((text + date.Month.ToString()).Length > charProperty.Lenghth)
                            {
                                text = (date.Month.ToString() + text);
                                text = text.Substring(text.Length - charProperty.Lenghth, charProperty.Lenghth);
                            }
                            else
                                text = (date.Month.ToString() + text);

                            int.TryParse(text, out month);
                            if (month > 12)
                                text = text.Substring(text.Length - 1, 1);

                            int.TryParse(text, out month);
                            month = month - date.Month;
                            date = date.AddMonths(month);

                            MaskedTextBoxAdv.mValueChanged = false;
                            MaskedTextBoxAdv.Value = date;
                            MaskedTextBoxAdv.mValueChanged = true;
                        }
                        #endregion

                        #region Day
                        else if (charProperty.Type == DateTimeType.Day)
                        {
                            DateTime date;
                            DateTime.TryParse(MaskedTextBoxAdv.mValue.ToString(), out date);

                            int day;
                            if ((text + date.Day.ToString()).Length > charProperty.Lenghth)
                            {
                                text = (date.Day.ToString() + text);
                                text = text.Substring(text.Length - charProperty.Lenghth, charProperty.Lenghth);
                            }
                            else
                                text = (date.Day.ToString() + text);

                            //int.TryParse(text, out month);
                            //if (month > 12)
                            //    text = text.Substring(text.Length - 1, 1);

                            int.TryParse(text, out day);
                            day = day - date.Day;
                            date = date.AddDays(day);

                            MaskedTextBoxAdv.mValueChanged = false;
                            MaskedTextBoxAdv.Value = date;
                            MaskedTextBoxAdv.mValueChanged = true;
                        }
                        #endregion
                    }
                }
                MaskedTextBoxAdv.Select(MaskedTextBoxAdv.SelectionStart, 0);
                MaskedTextBoxAdv.selectionChanged = true;
                return true;
            }
            else
                throw new ArgumentOutOfRangeException();
        }
    }
}

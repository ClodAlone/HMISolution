#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

#if WPF

using System.ComponentModel;

#endif

#if WPF

namespace Syncfusion.Windows.Shared
#endif

#if SILVERLIGHT
namespace Syncfusion.Windows.Tools.Controls
#endif
{
    internal class DoubleValueHandler
    {
        public static DoubleValueHandler doubleValueHandler = new DoubleValueHandler();

        internal string oldunmaskedText = "";

        internal bool CanUpdate = false;

        internal bool Allow = false;

        internal bool AllowSelectionStart = false;

        internal bool AllowChange = false;

        internal int count = 0;

        public bool MatchWithMask(DoubleTextBox doubleTextBox, string text)
        {
            if (doubleTextBox.IsReadOnly)
                return true;

            bool minusKeyValidationflag = false;
            doubleTextBox.negativeFlag = false;
            count = 0;
            if (doubleTextBox.mValue == null || doubleTextBox.mValue == 0 ||
                Double.Equals(doubleTextBox.mValue, double.NaN))
            {
                if (text == "-")
                {
                    doubleTextBox.minusPressed = true;
                    if (doubleTextBox.count % 2 == 0)
                    {
                        doubleTextBox.Foreground = doubleTextBox.PositiveForeground;
                        doubleTextBox.IsNegative = false;
                    }
                    else
                    {
                        if (doubleTextBox.Value != 0)
                        {
                            doubleTextBox.Foreground = doubleTextBox.NegativeForeground;
                            doubleTextBox.IsNegative = true;
                        }
                    }

                    doubleTextBox.count++;
                    doubleTextBox.MaskedText = "-";
                    doubleTextBox.Value = 0;
                    doubleTextBox.CaretIndex = 1;
                    return true;
                }
                else
                {
                    if (doubleTextBox.minusPressed == true)
                    {
                        doubleTextBox.minusPressed = false;
                        minusKeyValidationflag = true;
                    }
                }
            }

            NumberFormatInfo numberFormat = doubleTextBox.GetCulture().NumberFormat;
            int selectedLength = 0;

            if (!string.IsNullOrEmpty(doubleTextBox.SelectedText))
                selectedLength = doubleTextBox.SelectedText.Length;

            if (text == "-" || text == "+")
            {
                if (doubleTextBox.mValue != null)
                {
                    double tempVal = (double)doubleTextBox.mValue;
                    if (text == "+")
                    {
                        if (doubleTextBox.Value < 1)
                            tempVal = (double)doubleTextBox.mValue * -1;
                        else
                            tempVal = (double)doubleTextBox.mValue * 1;
                    }
                    else
                        tempVal = (double)doubleTextBox.mValue * -1;

                    if ((tempVal > doubleTextBox.MaxValue) && (doubleTextBox.MaxValidation == MaxValidation.OnKeyPress))
                    {
                        if (doubleTextBox.MaxValueOnExceedMaxDigit)
                            tempVal = doubleTextBox.MaxValue;
                        else return true;
                    }

                    if (selectedLength == doubleTextBox.MaskedText.Length)
                    {
                        selectedLength = 0;
                        if (text == "-")
                        {
                            doubleTextBox.minusPressed = true;
                            if (doubleTextBox.UseNullOption)
                            {
                                doubleTextBox.SetValue(false, null);
                            }
                            else
                                doubleTextBox.Value = 0;
                            doubleTextBox.MaskedText = "-";
                            doubleTextBox.CaretIndex = 1;
                            return true;
                        }
                    }
                    if ((doubleTextBox.MinValidation == MinValidation.OnKeyPress))
                    {
                        if (tempVal > doubleTextBox.MinValue)
                        {
                            if (doubleTextBox.MinValueOnExceedMinDigit &&
                                (tempVal.ToString()).Length > (doubleTextBox.MinValue.ToString()).Length)
                                tempVal = doubleTextBox.MinValue;
                            else if ((tempVal.ToString()).Length <= (doubleTextBox.MinValue.ToString()).Length)
                            {
                                doubleTextBox.MaskedText = tempVal.ToString();
                            }
                            else return true;
                        }
                        else
                            tempVal = doubleTextBox.MinValue;
                    }
                    doubleTextBox.MaskedText = tempVal.ToString("N", numberFormat);
                    if (doubleTextBox.MaskedText.Contains("-") == true)
                        doubleTextBox.SelectionStart++;

                    doubleTextBox.SetValue(false, tempVal);
                }
                return true;
            }

            int selectionStart = 0;
            int selectionEnd = 0;
            int selectionLength = 0;
            if (doubleTextBox.Value == null)
                doubleTextBox.MaskedText = "";
            string maskedText = doubleTextBox.MaskedText;
            int separatorStart = maskedText.IndexOf(numberFormat.NumberDecimalSeparator);
            int separatorEnd = separatorStart + numberFormat.NumberDecimalSeparator.Length;

            if (text == numberFormat.NumberDecimalSeparator || text == ".")
            {
                if (doubleTextBox.NumberDecimalDigits == 0)
                {
                    if (doubleTextBox.MaximumNumberDecimalDigits > 0 || doubleTextBox.numberDecimalDigits > 0)
                    {
                        CanUpdate = true;
                        doubleTextBox.NumberDecimalDigits = doubleTextBox.NumberDecimalDigits + 1;
                        CanUpdate = false;
                        doubleTextBox.SelectionStart = separatorEnd + maskedText.Length +
                                                       numberFormat.NumberDecimalSeparator.Length;
                    }
                }
                else
                    doubleTextBox.SelectionStart = separatorEnd;
                if ((doubleTextBox.Text == "" || selectedLength == doubleTextBox.MaskedText.Length) && (text == "." || text == numberFormat.NumberDecimalSeparator))
                {
                    doubleTextBox.MaskedText = "0" + numberFormat.NumberDecimalSeparator + "00";
                    doubleTextBox.Value = Convert.ToDouble(doubleTextBox.MaskedText);
                    doubleTextBox.Select(2, 0);

                    if (selectedLength == doubleTextBox.MaskedText.Length)
                    {
                        selectedLength = 0;
                    }
                }
                return true;
            }
            if (text == numberFormat.NumberGroupSeparator)
            {
                if ((doubleTextBox.SelectionStart < doubleTextBox.Text.Length))
                {
                    if (doubleTextBox.Text[doubleTextBox.SelectionStart].ToString() ==
                        numberFormat.NumberGroupSeparator.ToString())
                    {
                        doubleTextBox.SelectionStart += 1;
                        return true;
                    }
                }
                return true;
            }

            int caretPosition = doubleTextBox.SelectionStart;
            int offsetCaretPosition = 0;
            string unmaskedText = "";

            int i;
            for (i = 0; i <= maskedText.Length; i++)
            {
                if (i == doubleTextBox.SelectionStart)
                {
                    selectionStart = unmaskedText.Length;
                    caretPosition = selectionStart + offsetCaretPosition;
                }
                if (i == (doubleTextBox.SelectionStart + doubleTextBox.SelectionLength))
                    selectionEnd = unmaskedText.Length;

                if (i == separatorEnd)
                    separatorEnd = unmaskedText.Length;

                if (i == separatorStart)
                {
                    separatorStart = unmaskedText.Length;
                    unmaskedText += CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToString();
                }

                if (i < maskedText.Length)
                {
                    if (char.IsDigit(maskedText[i]))
                    {
                        if (unmaskedText.Length == 0)
                        {
                            if (maskedText[i] != '0')
                            {
                                unmaskedText += maskedText[i];
                            }
                            else
                                offsetCaretPosition++;
                        }
                        else
                        {
                            unmaskedText += maskedText[i];
                        }
                    }
                }
            }
            selectionLength = selectionEnd - selectionStart;
            if (separatorStart < 0)
            {
                separatorStart = unmaskedText.Length;
                separatorEnd = unmaskedText.Length;
            }

            #region TextChange

            if (text != string.Empty)
            {
                if (selectionStart <= separatorStart && selectionEnd >= separatorEnd && char.IsDigit(text[0]))
                {
                    for (int decpos = separatorEnd; decpos < selectionEnd; decpos++)
                    {
                        if (decpos != unmaskedText.Length)
                        {
                            unmaskedText = unmaskedText.Remove(decpos, 1);
                            unmaskedText = unmaskedText.Insert(decpos, "0");
                        }
                    }
                    unmaskedText = unmaskedText.Remove(selectionStart, (separatorStart - selectionStart));
                    caretPosition = selectionStart;

                    unmaskedText = unmaskedText.Insert(selectionStart, text);
                    caretPosition = caretPosition + text.Length;
                }

                else if (selectionStart <= separatorStart && selectionEnd < separatorEnd)
                {
                    unmaskedText = unmaskedText.Remove(selectionStart, selectionLength);
                    caretPosition = selectionStart;

                    unmaskedText = unmaskedText.Insert(selectionStart, text);
                    caretPosition = caretPosition + text.Length;
                }
                else
                {
                    if (selectionStart == selectionEnd)
                    {
                        var isLastDigit = true;
                        if (doubleTextBox.NumberDecimalDigits > 0)
                        {
                            int decimalDigitsCount = 0;
                            for (int len = unmaskedText.Length - 1; len >= 0; len--)
                            {
                                if (numberFormat != null)
                                {
                                    if (unmaskedText[len].ToString() == numberFormat.NumberDecimalSeparator || unmaskedText.Length.ToString() == doubleTextBox.NumberDecimalSeparator)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        decimalDigitsCount++;
                                    }
                                }
                            }

                            if ((doubleTextBox.MaximumNumberDecimalDigits >= 0 && decimalDigitsCount == doubleTextBox.MaximumNumberDecimalDigits) || (doubleTextBox.numberDecimalDigits >= 0 && doubleTextBox.MaximumNumberDecimalDigits < 0 && doubleTextBox.numberDecimalDigits == decimalDigitsCount))
                            {
                                isLastDigit = true;
                            }
                            else
                            {
                                isLastDigit = false;
                            }
                        }

                        if (selectionStart == unmaskedText.Length && isLastDigit && !string.IsNullOrEmpty(unmaskedText))
                        {
                            unmaskedText = unmaskedText.Insert(unmaskedText.Length - 1, text[0].ToString());
                        }
                        else if (!doubleTextBox.IsExceedDecimalDigits &&
                            selectionStart < unmaskedText.Length && numberFormat != null && 
                            selectionStart > unmaskedText.IndexOf((numberFormat.NumberDecimalSeparator)) && 
                            doubleTextBox.MaximumNumberDecimalDigits < 0)
                        {
                            unmaskedText = unmaskedText.Insert(selectionStart, text[0].ToString());
                        }
                        else if (selectionStart != unmaskedText.Length)
                        {
                            unmaskedText = unmaskedText.Insert(selectionStart, text[0].ToString());
                            if (doubleTextBox.IsExceedDecimalDigits && (AllowChange))
                            {
                                for (int len = unmaskedText.Length - 1; len >= 0; len--)
                                {
                                    if (numberFormat != null)
                                    {
                                        if (unmaskedText[len].ToString() == numberFormat.NumberDecimalSeparator ||
                                            unmaskedText.Length.ToString() == doubleTextBox.NumberDecimalSeparator)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            count++;
                                        }
                                    }
                                }
                                if (count >= doubleTextBox.MinimumNumberDecimalDigits &&
                                    count < doubleTextBox.MaximumNumberDecimalDigits)
                                {
                                    if (doubleTextBox.MaximumNumberDecimalDigits > 0)
                                    {
                                        if (doubleTextBox.MinimumNumberDecimalDigits > 0)
                                        {
                                            CanUpdate = true;
                                            doubleTextBox.NumberDecimalDigits = count;
                                            AllowChange = true;
                                            CanUpdate = false;
                                        }
                                        else if (count <= doubleTextBox.numberDecimalDigits)
                                        {
                                            doubleTextBox.NumberDecimalDigits = doubleTextBox.numberDecimalDigits;
                                        }
                                        else if (count <= doubleTextBox.MaximumNumberDecimalDigits)
                                        {
                                            CanUpdate = true;
                                            doubleTextBox.NumberDecimalDigits = count;
                                            AllowChange = true;
                                            CanUpdate = false;
                                        }
                                    }
                                }
                                else if (doubleTextBox.MaximumNumberDecimalDigits > 0)
                                {
                                    CanUpdate = true;
                                    doubleTextBox.NumberDecimalDigits = doubleTextBox.MaximumNumberDecimalDigits;
                                    AllowChange = false;
                                    CanUpdate = false;
                                }
                                numberFormat = doubleTextBox.GetCulture().NumberFormat;
                            }
                            if (doubleTextBox.Value > -1 && doubleTextBox.Value < 1)
                            {
                                caretPosition = selectionStart + 1;
                            }
                        }
                        else if (doubleTextBox.IsExceedDecimalDigits)
                        {
                            if (doubleTextBox.MaximumNumberDecimalDigits > 0 ||
                                doubleTextBox.numberDecimalDigits > doubleTextBox.MaximumNumberDecimalDigits)
                            {
                                if (unmaskedText.Length <= doubleTextBox.SelectionStart)
                                {
                                    unmaskedText = unmaskedText.Insert(unmaskedText.Length, "0");
                                    unmaskedText = unmaskedText.Remove(unmaskedText.Length - 1);
                                    unmaskedText = unmaskedText.Insert(unmaskedText.Length, text[0].ToString());
                                }
                                else
                                {
                                    unmaskedText = unmaskedText.Insert(doubleTextBox.SelectionStart - 1, "0");
                                    unmaskedText = unmaskedText.Remove(doubleTextBox.SelectionStart - 1);
                                    unmaskedText = unmaskedText.Insert(doubleTextBox.SelectionStart - 1,
                                                                       text[0].ToString());
                                }
                                if (doubleTextBox.Value > -1 && doubleTextBox.Value < 1)
                                {
                                    caretPosition = selectionStart + 1;
                                }
                                for (int len = unmaskedText.Length - 1; len >= 0; len--)
                                {
                                    if (numberFormat != null)
                                    {
                                        if (unmaskedText[len].ToString() == numberFormat.NumberDecimalSeparator ||
                                            unmaskedText.Length.ToString() == doubleTextBox.NumberDecimalSeparator)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            count++;
                                        }
                                    }
                                }
                                AllowChange = true;
                                if (numberFormat != null)
                                {
                                    if (numberFormat.NumberDecimalDigits != doubleTextBox.NumberDecimalDigits &&
                                        doubleTextBox.NumberDecimalDigits == -1)
                                    {
                                        doubleTextBox.NumberDecimalDigits = numberFormat.NumberDecimalDigits;
                                        CanUpdate = true;
                                        doubleTextBox.NumberDecimalDigits = numberFormat.NumberDecimalDigits + 1;
                                        CanUpdate = false;
                                    }
                                    else
                                    {
                                        CanUpdate = true;
                                        if (doubleTextBox.NumberDecimalDigits < doubleTextBox.MaximumNumberDecimalDigits)
                                        {
                                            doubleTextBox.NumberDecimalDigits = doubleTextBox.NumberDecimalDigits + 1;
                                        }
                                        else if (count <= doubleTextBox.numberDecimalDigits)
                                        {
                                            doubleTextBox.NumberDecimalDigits = count;
                                        }
                                        else
                                        {
                                            if (unmaskedText.Length > 0)
                                            {
                                                unmaskedText = unmaskedText.Remove(selectionStart);
                                            }
                                        }
                                        CanUpdate = false;
                                    }
                                    numberFormat = doubleTextBox.GetCulture().NumberFormat;
                                }
                            }
                        }
                        else
                            return true;
                    }
                    else if (char.IsDigit(text[0]))
                    {
                        int textpos = 0;
                        for (int decpos = selectionStart; decpos < selectionEnd; decpos++)
                        {
                            unmaskedText = unmaskedText.Remove(decpos, 1);
                            if (textpos < text.Length)
                                unmaskedText = unmaskedText.Insert(decpos, text[textpos].ToString());
                            else
                                unmaskedText = unmaskedText.Insert(decpos, "0");
                            textpos++;
                        }
                        caretPosition = selectionStart + text.Length;
                    }
                    else if (!doubleTextBox.IsExceedDecimalDigits)
                    {
                        doubleTextBox.negativeFlag = true;
                        unmaskedText = doubleTextBox.MaskedText;
                    }
                    else
                    {
                    }
                }
            }

            #endregion TextChange

            oldunmaskedText = unmaskedText;
            double preValue;
            if (double.TryParse(unmaskedText, out preValue))
            {
                if (doubleTextBox.MaskedText.Length >= 15 && doubleTextBox.MaxValidation == MaxValidation.OnLostFocus)
                {
                    if (doubleTextBox.IsNegative || minusKeyValidationflag)
                    {
                        if (!doubleTextBox.negativeFlag)
                            unmaskedText = "-" + unmaskedText;
                    }
                    try
                    {
                        decimal newvalue = decimal.Parse(unmaskedText);
                        int startlen = doubleTextBox.MaskedText.Length;
                        selectionStart = doubleTextBox.SelectionStart;
                        doubleTextBox.MaskedText = newvalue.ToString("N", numberFormat);
                        int endlen = doubleTextBox.MaskedText.Length;
                        int totlen = endlen - startlen;
                        if (totlen == 0 &&
                            doubleTextBox.MaskedText[selectionStart - 1].ToString() ==
                            numberFormat.CurrencyDecimalSeparator)
                            doubleTextBox.SelectionStart = selectionStart + 1;
                        else if (selectionStart + 1 == endlen)
                            doubleTextBox.SelectionStart = selectionStart + 1;
                        else if (totlen == 1)
                            doubleTextBox.SelectionStart = selectionStart + 1;
                        else if (totlen == 2)
                            doubleTextBox.SelectionStart = selectionStart + 2;
                    }
                    catch
                    {
                    }
                    return true;
                }
                if (doubleTextBox.IsNegative || minusKeyValidationflag)
                {
                    if (!(doubleTextBox.SelectedText == doubleTextBox.Text.ToString()) &&
                        !doubleTextBox.SelectedText.Contains("-"))
                    {
                        preValue = preValue * -1;
                    }
                    else
                    {
                        if ((doubleTextBox.Value == 0 || doubleTextBox.IsNull) && minusKeyValidationflag)
                        {
                            preValue = preValue * -1;
                        }
                    }
                }
                if ((preValue > doubleTextBox.MaxValue) && (doubleTextBox.MaxValidation == MaxValidation.OnKeyPress))
                {
                    if (doubleTextBox.MaxValueOnExceedMaxDigit)
                        preValue = doubleTextBox.MaxValue;
                    else return true;
                }

                if (preValue < doubleTextBox.MinValue && (doubleTextBox.MinValidation == MinValidation.OnKeyPress))
                {
                    if (preValue <= doubleTextBox.MinValue && doubleTextBox.MinValue >= 0)
                    {
                        if (numberFormat != null)
                        {
                            if (doubleTextBox.UseNullOption)
                            {
                                unmaskedText = preValue.ToString("N", numberFormat);
                            }
                            if (unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) >=
                                (doubleTextBox.MinValue.ToString("N", numberFormat)).Length)
                                preValue = doubleTextBox.MinValue;
                            else if (unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) <=
                                     (doubleTextBox.MinValue.ToString("N", numberFormat)).Length)
                            {
                                doubleTextBox.checktext = doubleTextBox.checktext + text;
                                if (double.Parse(doubleTextBox.checktext) >= doubleTextBox.MinValue)
                                {
                                    doubleTextBox.Value = double.Parse(doubleTextBox.checktext);
                                    doubleTextBox.CaretIndex = doubleTextBox.Value.ToString().Length;
                                    doubleTextBox.checktext = "";
                                }
                                return true;
                            }
                        }
                    }
                    else if (preValue > doubleTextBox.MinValue)
                    {
                        doubleTextBox.MaskedText = unmaskedText;
                    }
                    else if (preValue >= doubleTextBox.MinValue)
                    {
                        if (doubleTextBox.MinValueOnExceedMinDigit &&
                            unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) >
                            (doubleTextBox.MinValue.ToString()).Length)
                            preValue = doubleTextBox.MinValue;
                        else if (unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) <=
                                 (doubleTextBox.MinValue.ToString()).Length)
                        {
                            doubleTextBox.MaskedText = unmaskedText;
                        }
                        else return true;
                    }
                    else
                    {
                        if (doubleTextBox.MinValueOnExceedMinDigit)
                            preValue = doubleTextBox.MinValue;
                        else return true;
                    }
                }
                else
                {
                    if (preValue >= doubleTextBox.MinValue && (doubleTextBox.MinValidation == MinValidation.OnKeyPress) &&
                        doubleTextBox.checktext != "" && double.Parse(doubleTextBox.checktext) == 0.0)
                    {
                        doubleTextBox.checktext = "";
                    }
                }
                if (doubleTextBox.MaxLength != 0)
                {
                    if (unmaskedText.Length > doubleTextBox.MaxLength &&
                        doubleTextBox.NumberDecimalDigits <= doubleTextBox.MaxLength)
                    {
                        int len = doubleTextBox.NumberDecimalDigits;
                        if (len < 0 && doubleTextBox.MaxLength > 3)
                        {
                            preValue = double.Parse(unmaskedText.Remove((doubleTextBox.MaxLength) - 3));
                            caretPosition++;
                            doubleTextBox.CaretIndex = caretPosition;
                        }
                        else
                            preValue = double.Parse(unmaskedText.Remove((doubleTextBox.MaxLength) - 1 - len));
                        doubleTextBox.SetValue(false, preValue);
                        doubleTextBox.MaskedText = preValue.ToString("N", numberFormat);
                        if (caretPosition == doubleTextBox.CaretIndex)
                            caretPosition++;
                        doubleTextBox.CaretIndex = caretPosition;
                        return true;
                    }
                }
                if (doubleTextBox.checktext != "" && preValue >= doubleTextBox.MinValue)
                    caretPosition = caretPosition + 1;
                double lastValue = double.Parse(doubleTextBox.checktext + preValue.ToString());
                if (lastValue <= doubleTextBox.MaxValue)
                    preValue = lastValue;
                if (unmaskedText.Length > 1 && maskedText.Length - 1 == selectionStart &&
                    doubleTextBox.NumberDecimalDigits > 0 && !maskedText.Contains("-"))
                {
                    preValue = double.Parse(unmaskedText.Remove(unmaskedText.Length - 1));
                }
                doubleTextBox.MaskedText = preValue.ToString("N", numberFormat);
                maskedText = doubleTextBox.MaskedText;
                if (!string.IsNullOrEmpty(maskedText) && maskedText[0] != '0' && selectionStart != 0 &&
                    selectionLength != 0)
                    caretPosition--;
                doubleTextBox.SetValue(false, preValue);

                int j = 0;
                for (i = 0; i < maskedText.Length; i++)
                {
                    if (i == caretPosition)
                    {
                        break;
                    }
                    if (j == maskedText.Length)
                        break;
                    if (char.IsDigit(maskedText[j]))
                        j++;
                    else
                    {
                        for (int k = j; k < maskedText.Length; k++)
                        {
                            if (char.IsDigit(maskedText[k]))
                                break;
                            j++;
                        }
                        i--;
                    }
                }
                doubleTextBox.SelectionStart = j;
                doubleTextBox.SelectionLength = 0;
#if WPF
                if (!doubleTextBox.OnValidating(new CancelEventArgs(false)))
                {
                    if (doubleTextBox.ValueValidation == StringValidation.OnKeyPress)
                    {
                        string validationerror = "";
                        bool validationstatus = true;

                        if (doubleTextBox.ValidationValue == doubleTextBox.Value.ToString())
                            validationstatus = true;
                        else
                            validationstatus = false;

                        string message = validationstatus ? "String validation succeeded" : "String validation failed";

                        if (!validationstatus)
                        {
                            if (doubleTextBox.InvalidValueBehavior == InvalidInputBehavior.DisplayErrorMessage)
                            {
                                MessageBox.Show(message, "Invalid value", MessageBoxButton.OK);
                                doubleTextBox.OnValueValidationCompleted(new StringValidationEventArgs(
                                                                             validationstatus, validationerror,
                                                                             doubleTextBox.ValidationValue));
                                doubleTextBox.OnValidated(EventArgs.Empty);
                                return true;
                            }
                            else if (doubleTextBox.InvalidValueBehavior == InvalidInputBehavior.ResetValue)
                            {
                                doubleTextBox.OnValueValidationCompleted(new StringValidationEventArgs(
                                                                             validationstatus, validationerror,
                                                                             doubleTextBox.ValidationValue));
                                doubleTextBox.OnValidated(EventArgs.Empty);
                                return true;
                            }
                            else if (doubleTextBox.InvalidValueBehavior == InvalidInputBehavior.None)
                            {
                                doubleTextBox.OnValueValidationCompleted(new StringValidationEventArgs(
                                                                             validationstatus, validationerror,
                                                                             doubleTextBox.ValidationValue));
                                doubleTextBox.OnValidated(EventArgs.Empty);
                            }
                        }
                        else
                        {
                            doubleTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus,
                                                                                                   validationerror,
                                                                                                   doubleTextBox.
                                                                                                       ValidationValue));
                            doubleTextBox.OnValidated(EventArgs.Empty);
                        }
                        return true;
                    }
                }
#endif
            }
            return true;
        }

        public double? ValueFromText(DoubleTextBox doubleTextBox, string maskedText)
        {
            double preValue;
            if (double.TryParse(maskedText, NumberStyles.Number, doubleTextBox.GetCulture().NumberFormat, out preValue))
                return preValue;
            else
            {
                if (doubleTextBox.UseNullOption)
                    return null;
                else
                {
                    double temp = 0;
                    if (temp > doubleTextBox.MaxValue && doubleTextBox.MinValidation == MinValidation.OnKeyPress)
                    {
                        temp = doubleTextBox.MaxValue;
                    }
                    else if (temp < doubleTextBox.MinValue && doubleTextBox.MaxValidation == MaxValidation.OnKeyPress)
                    {
                        temp = doubleTextBox.MinValue;
                    }
                    return temp;
                }
            }
        }

        public bool HandleKeyDown(DoubleTextBox doubleTextBox, KeyEventArgs eventArgs)
        {
            if (eventArgs.Key == Key.Space)
                return true;
            if (eventArgs.Key == Key.Right || eventArgs.Key == Key.Left)
            {
                DoubleValueHandler.doubleValueHandler.AllowSelectionStart = false;
            }
            switch (eventArgs.Key)
            {
                case Key.None:
                    break;
#if WPF
                case Key.Cancel:
                    break;
#endif
                case Key.Back:
                    doubleTextBox.count = 1;
                    return HandleBackSpaceKey(doubleTextBox);
                case Key.Tab:
                    break;
#if WPF
                case Key.LineFeed:
                    break;

                case Key.Clear:
                    break;

                case Key.Pause:
                    break;

                case Key.Capital:
                    break;

                case Key.KanaMode:
                    break;

                case Key.JunjaMode:
                    break;

                case Key.FinalMode:
                    break;

                case Key.HanjaMode:
                    break;
#endif
#if SILVERLIGHT
                case Key.Enter:
                    if (doubleTextBox.EnterToMoveNext)
                    {
                        //FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;
                        //TraversalRequest request = new TraversalRequest(focusDirection);
                        //UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;
                        //if (elementWithFocus != null)
                        //{
                        //    elementWithFocus.MoveFocus(request);
                        //}
                    }
                    break;
#endif
#if SILVERLIGHT
                case Key.Shift:
                    break;

                case Key.Ctrl:
                    break;

                case Key.Alt:
                    break;
#endif
#if SILVERLIGHT
                case Key.CapsLock:
                    break;
#endif
#if WPF
                case Key.ImeConvert:
                    break;

                case Key.ImeNonConvert:
                    break;

                case Key.ImeAccept:
                    break;

                case Key.ImeModeChange:
                    break;

                case Key.Space:
                    break;

                case Key.Prior:
                    break;

                case Key.Next:
                    break;
#endif
                case Key.Escape:
                    break;
#if SILVERLIGHT
                case Key.Space:
                    break;

                case Key.PageUp:
                    break;

                case Key.PageDown:
                    break;
#endif
                case Key.End:
                    break;

                case Key.Home:
                    break;

                case Key.Left:
                    break;

                case Key.Up:
                    return HandleUpKey(doubleTextBox);
                case Key.Right:
                    break;

                case Key.Down:
                    return HandleDownKey(doubleTextBox);
#if WPF
                case Key.Select:
                    break;

                case Key.Print:
                    break;

                case Key.Execute:
                    break;

                case Key.Snapshot:
                    break;
#endif
                case Key.Insert:
                    break;

                case Key.Delete:
                    doubleTextBox.count = 1;
                    return HandleDeleteKey(doubleTextBox);
#if WPF
                case Key.Help:
                    break;
#endif
                case Key.D0:
                    break;

                case Key.D1:
                    break;

                case Key.D2:
                    break;

                case Key.D3:
                    break;

                case Key.D4:
                    break;

                case Key.D5:
                    break;

                case Key.D6:
                    break;

                case Key.D7:
                    break;

                case Key.D8:
                    break;

                case Key.D9:
                    break;

                case Key.A:
                    break;

                case Key.B:
                    break;

                case Key.C:
                    break;

                case Key.D:
                    break;

                case Key.E:
                    break;

                case Key.F:
                    break;

                case Key.G:
                    break;

                case Key.H:
                    break;

                case Key.I:
                    break;

                case Key.J:
                    break;

                case Key.K:
                    break;

                case Key.L:
                    break;

                case Key.M:
                    break;

                case Key.N:
                    break;

                case Key.O:
                    break;

                case Key.P:
                    break;

                case Key.Q:
                    break;

                case Key.R:
                    break;

                case Key.S:
                    break;

                case Key.T:
                    break;

                case Key.U:
                    break;

                case Key.V:
                    break;

                case Key.W:
                    break;

                case Key.X:
                    break;

                case Key.Y:
                    break;

                case Key.Z:
                    break;
#if WPF
                case Key.LWin:
                    break;

                case Key.RWin:
                    break;

                case Key.Apps:
                    break;

                case Key.Sleep:
                    break;
#endif

                case Key.NumPad0:
                    break;

                case Key.NumPad1:
                    break;

                case Key.NumPad2:
                    break;

                case Key.NumPad3:
                    break;

                case Key.NumPad4:
                    break;

                case Key.NumPad5:
                    break;

                case Key.NumPad6:
                    break;

                case Key.NumPad7:
                    break;

                case Key.NumPad8:
                    break;

                case Key.NumPad9:
                    break;

                case Key.Multiply:
                    break;

                case Key.Add:
                    break;
#if WPF
                case Key.Separator:
                    break;
#endif
                case Key.Subtract:
                    break;

                case Key.Decimal:
                    break;

                case Key.Divide:
                    break;

                case Key.F1:
                    break;

                case Key.F2:
                    break;

                case Key.F3:
                    break;

                case Key.F4:
                    break;

                case Key.F5:
                    break;

                case Key.F6:
                    break;

                case Key.F7:
                    break;

                case Key.F8:
                    break;

                case Key.F9:
                    break;

                case Key.F10:
                    break;

                case Key.F11:
                    break;

                case Key.F12:
                    break;
#if WPF
                case Key.F13:
                    break;

                case Key.F14:
                    break;

                case Key.F15:
                    break;

                case Key.F16:
                    break;

                case Key.F17:
                    break;

                case Key.F18:
                    break;

                case Key.F19:
                    break;

                case Key.F20:
                    break;

                case Key.F21:
                    break;

                case Key.F22:
                    break;

                case Key.F23:
                    break;

                case Key.F24:
                    break;

                case Key.NumLock:
                    break;

                case Key.Scroll:
                    break;

                case Key.LeftShift:
                    break;

                case Key.RightShift:
                    break;

                case Key.LeftCtrl:
                    break;

                case Key.RightCtrl:
                    break;

                case Key.LeftAlt:
                    break;

                case Key.RightAlt:
                    break;

                case Key.BrowserBack:
                    break;

                case Key.BrowserForward:
                    break;

                case Key.BrowserRefresh:
                    break;

                case Key.BrowserStop:
                    break;

                case Key.BrowserSearch:
                    break;

                case Key.BrowserFavorites:
                    break;

                case Key.BrowserHome:
                    break;

                case Key.VolumeMute:
                    break;

                case Key.VolumeDown:
                    break;

                case Key.VolumeUp:
                    break;

                case Key.MediaNextTrack:
                    break;

                case Key.MediaPreviousTrack:
                    break;

                case Key.MediaStop:
                    break;

                case Key.MediaPlayPause:
                    break;

                case Key.LaunchMail:
                    break;

                case Key.SelectMedia:
                    break;

                case Key.LaunchApplication1:
                    break;

                case Key.LaunchApplication2:
                    break;

                case Key.Oem1:
                    break;

                case Key.OemPlus:
                    break;

                case Key.OemComma:
                    break;

                case Key.OemMinus:
                    break;

                case Key.OemPeriod:
                    break;

                case Key.Oem2:
                    break;

                case Key.Oem3:
                    break;

                case Key.AbntC1:
                    break;

                case Key.AbntC2:
                    break;

                case Key.Oem4:
                    break;

                case Key.Oem5:
                    break;

                case Key.Oem6:
                    break;

                case Key.Oem7:
                    break;

                case Key.Oem8:
                    break;

                case Key.Oem102:
                    break;

                case Key.ImeProcessed:
                    break;

                case Key.System:
                    break;

                case Key.OemAttn:
                    break;

                case Key.OemFinish:
                    break;

                case Key.OemCopy:
                    break;

                case Key.OemAuto:
                    break;

                case Key.OemEnlw:
                    break;

                case Key.OemBackTab:
                    break;

                case Key.Attn:
                    break;

                case Key.CrSel:
                    break;

                case Key.ExSel:
                    break;

                case Key.EraseEof:
                    break;

                case Key.Play:
                    break;

                case Key.Zoom:
                    break;

                case Key.NoName:
                    break;

                case Key.Pa1:
                    break;

                case Key.OemClear:
                    break;

#endif
#if SILVERLIGHT
                case Key.Unknown:
                    break;
#endif
                default:
                    break;
            }
            return false;
        }

        public bool HandleBackSpaceKey(DoubleTextBox doubleTextBox)
        {
            if (doubleTextBox.IsReadOnly)
                return true;

            NumberFormatInfo numberFormat = doubleTextBox.GetCulture().NumberFormat;
            count = 0;
            string maskedText = doubleTextBox.MaskedText;
            int selectionStart = 0;
            int selectionEnd = 0;
            int selectionLength = 0;
            int separatorStart = maskedText.IndexOf(numberFormat.NumberDecimalSeparator);
            int separatorEnd = separatorStart + numberFormat.NumberDecimalSeparator.Length;
            int negflag = 0;
            int caretPosition = doubleTextBox.SelectionStart;
            string unmaskedText = "";

            if (doubleTextBox.SelectionLength == 1)
            {
                if (!char.IsDigit(maskedText[doubleTextBox.SelectionStart]))
                {
                    if (maskedText[doubleTextBox.SelectionStart] == '-')
                    {
                        doubleTextBox.Value = (doubleTextBox.Value * -1);
                    }
                    doubleTextBox.SelectionLength = 0;
                    return true;
                }
            }
            else if (doubleTextBox.SelectionLength == 0 && doubleTextBox.SelectionStart == 1 && doubleTextBox.SelectionStart != maskedText.Length - 2)
            {
                if (!char.IsDigit(maskedText[doubleTextBox.SelectionStart - 1]))
                {
                    if (maskedText[0] == '-')
                    {
                        doubleTextBox.Value = (doubleTextBox.Value * -1);
                        doubleTextBox.SelectionLength = 0;
                        return true;
                    }
                }
            }
            if (doubleTextBox.SelectionLength == 0 && doubleTextBox.SelectionStart != 0)
            {
                string numbergroup;
                if (numberFormat == null)
                    numbergroup = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
                else
                    numbergroup = numberFormat.NumberGroupSeparator;
                if ((doubleTextBox.NumberGroupSeparator == string.Empty && maskedText[doubleTextBox.SelectionStart - 1].ToString() == numbergroup) || (maskedText[doubleTextBox.SelectionStart - 1].ToString() == numbergroup))
                {
                    unmaskedText = "";
                    doubleTextBox.SelectionStart--;
                    return true;
                }
            }

            int i;
            for (i = 0; i <= maskedText.Length; i++)
            {
                if (i == doubleTextBox.SelectionStart)
                {
                    selectionStart = unmaskedText.Length;
                    caretPosition = selectionStart;
                }
                if (i == (doubleTextBox.SelectionStart + doubleTextBox.SelectionLength))
                    selectionEnd = unmaskedText.Length;

                if (i == separatorEnd)
                    separatorEnd = unmaskedText.Length;

                if (i == separatorStart)
                {
                    separatorStart = unmaskedText.Length;
                    unmaskedText += CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToString();
                }

                if (i < maskedText.Length)
                {
                    if (char.IsDigit(maskedText[i]))
                        unmaskedText += maskedText[i];
                }
            }
            selectionLength = selectionEnd - selectionStart;

            if (selectionStart <= separatorStart && selectionEnd >= separatorEnd && unmaskedText.Length > 0)
            {
                if (numberFormat != null && selectionLength < unmaskedText.Length)
                {
                    for (int decpos = 0; decpos < doubleTextBox.SelectionLength; decpos++)
                    {
                        if (unmaskedText.Length > 0)
                        {
                            if (numberFormat != null && unmaskedText != string.Empty && unmaskedText.Length > selectionStart)
                            {
                                if (unmaskedText[selectionStart].ToString() != numberFormat.NumberDecimalSeparator && unmaskedText[selectionStart].ToString() != doubleTextBox.NumberDecimalSeparator)
                                {
                                    unmaskedText = unmaskedText.Remove(selectionStart, 1);
                                }
                                else
                                    selectionStart++;
                            }
                        }
                    }
                    if (doubleTextBox.IsExceedDecimalDigits)
                    {
                        for (int len = unmaskedText.Length - 1; len >= 0; len--)
                        {
                            if (numberFormat != null)
                            {
                                if (unmaskedText[len].ToString() == numberFormat.NumberDecimalSeparator || unmaskedText.Length.ToString() == doubleTextBox.NumberDecimalSeparator)
                                {
                                    break;
                                }
                                else
                                {
                                    count++;
                                }
                            }
                        }
                        if (doubleTextBox.MinimumNumberDecimalDigits >= 0)
                        {
                            if (count >= doubleTextBox.MinimumNumberDecimalDigits)
                            {
                                CanUpdate = true;
                                doubleTextBox.NumberDecimalDigits = count;
                                AllowChange = true;
                                CanUpdate = false;
                            }
                            else
                            {
                                CanUpdate = true;
                                doubleTextBox.NumberDecimalDigits = doubleTextBox.MinimumNumberDecimalDigits;
                                CanUpdate = false;
                                AllowChange = false;
                            }
                        }
                        else if (count <= doubleTextBox.numberDecimalDigits)
                        {
                            doubleTextBox.NumberDecimalDigits = doubleTextBox.numberDecimalDigits;
                            AllowChange = false;
                        }
                        else
                        {
                            CanUpdate = true;
                            doubleTextBox.NumberDecimalDigits = count;
                            AllowChange = true;
                            CanUpdate = false;
                        }

                        caretPosition++;
                        Allow = true;
                    }
                }
                else if (selectionLength == unmaskedText.Length)
                {
                    unmaskedText = "";
                    AllowChange = false;
                }
            }
            else if (selectionStart <= separatorStart && selectionEnd < separatorEnd)
            {
                if (selectionLength == 0)
                {
                    if (selectionStart != 0)
                    {
                        selectionLength = 1;
                        if (doubleTextBox.MinValue == Double.Parse(unmaskedText))
                        {
                            if (doubleTextBox.UseNullOption && unmaskedText.Length > 0)
                                unmaskedText = unmaskedText.Remove(selectionStart - 1, selectionLength);
                            caretPosition = selectionStart - 1;
                        }
                        else if (unmaskedText.Length > 0)
                        {
                            unmaskedText = unmaskedText.Remove(selectionStart - 1, selectionLength);
                            caretPosition = selectionStart - 1;
                            if (doubleTextBox.SelectionStart == 2)
                                negflag = 1;
                        }
                    }
                    else
                        return true;
                }
                else if (unmaskedText.Length > 0)
                {
                    if (maskedText[doubleTextBox.SelectionStart] == '-')
                    {
                        doubleTextBox.Value = doubleTextBox.Value * -1;
                    }
                    unmaskedText = unmaskedText.Remove(selectionStart, selectionLength);
                    caretPosition = selectionStart;
                    if (doubleTextBox.SelectionStart > 0)
                    {
                        if (doubleTextBox.Text[doubleTextBox.SelectionStart - 1] == '-')
                        {
                            negflag = 1;
                        }
                    }
                }
            }
            else if (separatorStart < 0)
            {
                if (selectionStart >= 0)
                {
                    if (selectionStart == 0 && selectionLength >= 1)
                    {
                        if (maskedText[doubleTextBox.SelectionStart] == '-')
                        {
                            doubleTextBox.Value = doubleTextBox.Value * -1;
                        }
                    }
                    if (selectionLength >= 1 && unmaskedText.Length > 0)
                    {
                        if (selectionLength == unmaskedText.Length && doubleTextBox.MinValue.ToString() != "" && doubleTextBox.MinValue > 0)
                        {
                            unmaskedText = doubleTextBox.MinValue.ToString();
                        }
                        else
                        {
                            unmaskedText = unmaskedText.Remove(selectionStart, selectionLength);
                            caretPosition = selectionStart;
                            if (doubleTextBox.SelectionStart == 1)
                                negflag = 1;
                        }
                    }
                    else if (selectionLength == 0 && selectionStart == unmaskedText.Length && unmaskedText.Length > 0)
                    {
                        if (Double.Parse(unmaskedText) == doubleTextBox.MinValue)
                        {
                            caretPosition = selectionStart - 1;
                        }
                        else
                        {
                            unmaskedText = unmaskedText.Remove(selectionStart - 1, 1);
                            caretPosition = selectionStart;
                        }
                    }
                    else if (selectionLength == 0 && unmaskedText.Length > 0 && selectionStart != unmaskedText.Length && selectionStart != 0 && maskedText[doubleTextBox.SelectionStart - 1] != ',')
                    {
                        if (doubleTextBox.SelectionStart == 2)
                        {
                            unmaskedText = unmaskedText.Remove(selectionStart - 1, 1);
                            negflag = 1;
                        }
                        else
                        {
                            unmaskedText = unmaskedText.Remove(selectionStart - 1, 1);
                            caretPosition = selectionStart - 1;
                        }
                    }
                    else if (selectionLength == 0 && selectionStart != unmaskedText.Length && selectionStart != 0 && maskedText[doubleTextBox.SelectionStart - 1] == ',')
                    {
                        unmaskedText = "";
                        doubleTextBox.SelectionStart--;
                        return true;
                    }
                    else
                    {
                        if (selectionStart != 0 && unmaskedText.Length > 0)
                        {
                            unmaskedText = unmaskedText.Remove(selectionStart, 1);
                            caretPosition = selectionStart;
                        }
                    }
                }
            }
            else
            {
                if (selectionStart == selectionEnd && unmaskedText.Length > 0)
                {
                    if (selectionStart != separatorEnd)
                    {
                        unmaskedText = unmaskedText.Remove(selectionStart - 1, 1);

                        for (int len = unmaskedText.Length - 1; len >= 0; len--)
                        {
                            if (numberFormat != null && unmaskedText != string.Empty)
                            {
                                if (unmaskedText[len].ToString() == numberFormat.NumberDecimalSeparator || unmaskedText.Length.ToString() == doubleTextBox.NumberDecimalSeparator)
                                {
                                    break;
                                }
                                else
                                {
                                    count++;
                                }
                            }
                        }
                        if (doubleTextBox.IsExceedDecimalDigits)
                        {
                            if (count >= doubleTextBox.MinimumNumberDecimalDigits)
                            {
                                if (doubleTextBox.MinimumNumberDecimalDigits >= 0)
                                {
                                    CanUpdate = true;
                                    doubleTextBox.NumberDecimalDigits = count;
                                    AllowChange = true;
                                    CanUpdate = false;
                                }
                                else if (count >= doubleTextBox.numberDecimalDigits)
                                {
                                    CanUpdate = true;
                                    doubleTextBox.NumberDecimalDigits = count;
                                    AllowChange = false;
                                    CanUpdate = false;
                                }
                            }
                            else
                            {
                                doubleTextBox.NumberDecimalDigits = doubleTextBox.MinimumNumberDecimalDigits;
                                AllowChange = false;
                            }
                            Allow = true;
                        }
                        caretPosition = selectionStart - 1;
                    }
                    else
                    {
                        doubleTextBox.SelectionStart = doubleTextBox.SelectionStart - 1;
                        return true;
                    }
                }
                else if (unmaskedText.Length > 0)
                {
                    if (!doubleTextBox.IsExceedDecimalDigits)
                    {
                        for (int decpos = 0; decpos < doubleTextBox.SelectionLength; decpos++)
                        {
                            unmaskedText = unmaskedText.Remove(selectionStart, 1);
                        }
                        caretPosition--;
                    }
                    else
                    {
                        for (int decpos = 0; decpos < doubleTextBox.SelectionLength; decpos++)
                        {
                            unmaskedText = unmaskedText.Remove(selectionStart, 1);
                        }
                        for (int len = unmaskedText.Length - 1; len >= 0; len--)
                        {
                            if (numberFormat != null && unmaskedText != string.Empty)
                            {
                                if (unmaskedText[len].ToString() == numberFormat.NumberDecimalSeparator || unmaskedText.Length.ToString() == doubleTextBox.NumberDecimalSeparator)
                                {
                                    break;
                                }
                                else
                                {
                                    count++;
                                }
                            }
                        }
                        if (doubleTextBox.IsExceedDecimalDigits && doubleTextBox.MinimumNumberDecimalDigits >= 0)
                        {
                            if (count >= doubleTextBox.MinimumNumberDecimalDigits)
                            {
                                CanUpdate = true;
                                doubleTextBox.NumberDecimalDigits = count;
                                AllowChange = true;
                                CanUpdate = false;
                            }
                            else
                            {
                                CanUpdate = true;
                                doubleTextBox.NumberDecimalDigits = doubleTextBox.MinimumNumberDecimalDigits;
                                CanUpdate = true;
                                AllowChange = false;
                            }
                            Allow = true;
                        }
                        else if (count <= doubleTextBox.numberDecimalDigits)
                        {
                            doubleTextBox.NumberDecimalDigits = doubleTextBox.numberDecimalDigits;
                            AllowChange = false;
                        }
                        else
                        {
                            CanUpdate = true;
                            doubleTextBox.NumberDecimalDigits = count;
                            AllowChange = true;
                            CanUpdate = false;
                        }
                    }
                }
            }

            if (doubleTextBox.IsExceedDecimalDigits && !Allow && doubleTextBox.MinimumNumberDecimalDigits >= 0 && doubleTextBox.MaximumNumberDecimalDigits > 0)
            {
                double newValue;
                if (double.TryParse(unmaskedText, out newValue))
                    if (selectionLength > 0)
                    {
                        if (numberFormat != null)
                        {
                            CanUpdate = true;
                            doubleTextBox.NumberDecimalDigits = doubleTextBox.numberDecimalDigits;
                            AllowChange = false;
                            CanUpdate = false;
                        }
                    }
            }
            oldunmaskedText = unmaskedText;
            double preValue;
            bool separatorflag = false;
            if (double.TryParse(unmaskedText, out preValue))
            {
                if (doubleTextBox.MaskedText.Length >= 15 && doubleTextBox.MaxValidation == MaxValidation.OnLostFocus)
                {
                    if (doubleTextBox.IsNegative)
                    {
                        if (!doubleTextBox.negativeFlag)
                            unmaskedText = "-" + unmaskedText;
                    }
                    try
                    {
                        decimal newvalue = decimal.Parse(unmaskedText);
                        int startlen = doubleTextBox.MaskedText.Length;
                        selectionStart = doubleTextBox.SelectionStart;
                        doubleTextBox.MaskedText = newvalue.ToString("N", numberFormat);
                        int endlen = doubleTextBox.MaskedText.Length;
                        if (startlen != 0)
                        {
                            int totlen = startlen - endlen;
                            if (totlen == 0 && doubleTextBox.MaskedText[selectionStart - 2].ToString() == numberFormat.CurrencyDecimalSeparator)
                                doubleTextBox.SelectionStart = selectionStart - 1;
                            else if (selectionStart == endlen)
                                doubleTextBox.SelectionStart = selectionStart - 1;
                            else if (totlen == 1)
                                doubleTextBox.SelectionStart = selectionStart - 1;
                            else if (totlen == 2)
                                doubleTextBox.SelectionStart = selectionStart - 2;
                        }
                    }
                    catch { }
                    return true;
                }
                if (preValue == 0)
                {
                    if (doubleTextBox.IsExceedDecimalDigits && doubleTextBox.MinimumNumberDecimalDigits >= 0)
                    {
                        if (numberFormat != null)
                        {
                            CanUpdate = true;
                            doubleTextBox.NumberDecimalDigits = doubleTextBox.MinimumNumberDecimalDigits;
                            AllowChange = false;
                            CanUpdate = false;
                        }
                    }
                    else
                    {
                        if (doubleTextBox.NumberDecimalDigits < 0)
                            doubleTextBox.NumberDecimalDigits = numberFormat.NumberDecimalDigits;
                        else
                            doubleTextBox.NumberDecimalDigits = doubleTextBox.numberDecimalDigits;
                    }
                    if (doubleTextBox.UseNullOption)
                    {
                        doubleTextBox.SetValue(true, null);
                    }
                    else
                    {
                        if (doubleTextBox.MinValue.ToString() == "0" || doubleTextBox.MinValue < 0)
                        {
                            if (doubleTextBox.SelectionStart != 0)
                            {
                                doubleTextBox.SelectionStart--;
                            }
                            doubleTextBox.SetValue(true, 0.0);
                        }
                        else
                        {
                            doubleTextBox.SetValue(true, doubleTextBox.MinValue);
                        }
                    }
                    return true;
                }
                numberFormat = doubleTextBox.GetCulture().NumberFormat;
                if (doubleTextBox.IsNegative)
                {
                    preValue = preValue * -1;
                }

                if ((preValue > doubleTextBox.MaxValue) && (doubleTextBox.MaxValidation == MaxValidation.OnKeyPress))
                {
                    if (doubleTextBox.MaxValueOnExceedMaxDigit)
                        preValue = doubleTextBox.MaxValue;
                    else return true;
                }
                if (preValue <= doubleTextBox.MinValue && (doubleTextBox.MinValidation == MinValidation.OnKeyPress))
                {
                    if (preValue < doubleTextBox.MinValue && doubleTextBox.MinValue >= 0)
                    {
                        if (numberFormat != null)
                        {
                            if (unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) >= (doubleTextBox.MinValue.ToString()).Length)
                            {
                                if (doubleTextBox.MinValueOnExceedMinDigit)
                                    preValue = doubleTextBox.MinValue;
                                else
                                    return true;
                            }
                            else if (unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) <= (doubleTextBox.MinValue.ToString()).Length)
                            {
                                if (doubleTextBox.MinValueOnExceedMinDigit)
                                    preValue = doubleTextBox.MinValue;
                                else if (preValue >= doubleTextBox.MinValue)
                                    doubleTextBox.MaskedText = unmaskedText;
                                else
                                    return true;
                            }
                        }
                        else return true;
                    }
                    else if (preValue > doubleTextBox.MinValue)
                    {
                        doubleTextBox.MaskedText = unmaskedText;
                    }
                    else if (preValue >= doubleTextBox.MinValue)
                    {
                        if (doubleTextBox.MinValueOnExceedMinDigit && unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) > (doubleTextBox.MinValue.ToString()).Length)
                            preValue = doubleTextBox.MinValue;
                        else if (unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) <= (doubleTextBox.MinValue.ToString()).Length)
                        {
                            doubleTextBox.MaskedText = unmaskedText;
                        }
                        else return true;
                    }
                    else if (doubleTextBox.MinValueOnExceedMinDigit)
                        preValue = doubleTextBox.MinValue;
                    else
                        return true;
                }

                doubleTextBox.MaskedText = preValue.ToString("N", numberFormat);
                maskedText = doubleTextBox.MaskedText;
                doubleTextBox.SetValue(false, preValue);
                if (negflag == 0)
                {
                    int j = 0;
                    for (i = 0; i < unmaskedText.Length; i++)
                    {
                        if (i == caretPosition)
                            break;
                        if (j == maskedText.Length)
                            break;

                        if (char.IsDigit(maskedText[j]))
                            j++;

                        else
                        {
                            for (int k = j; k < maskedText.Length; k++)
                            {
                                if (j == maskedText.IndexOf(numberFormat.NumberDecimalSeparator))
                                    separatorflag = true;

                                if (char.IsDigit(maskedText[k]))
                                    break;
                                j++;
                            }
                            if (separatorflag == false)
                                i--;
                            separatorflag = false;
                        }
                    }

                    doubleTextBox.SelectionStart = j;
                }
                else
                    doubleTextBox.SelectionStart = 1;
                doubleTextBox.SelectionLength = 0;
                negflag = 0;
            }
            else
            {
                if (preValue == 0)
                {
                    if (doubleTextBox.IsExceedDecimalDigits && doubleTextBox.MinimumNumberDecimalDigits >= 0)
                    {
                        if (numberFormat != null)
                        {
                            CanUpdate = true;
                            doubleTextBox.NumberDecimalDigits = doubleTextBox.MinimumNumberDecimalDigits;
                            CanUpdate = false;
                            AllowChange = false;
                        }
                    }
                    else
                    {
                        {
                            if (doubleTextBox.NumberDecimalDigits < 0)
                                doubleTextBox.NumberDecimalDigits = numberFormat.NumberDecimalDigits;
                            else
                                doubleTextBox.NumberDecimalDigits = doubleTextBox.numberDecimalDigits;
                        }
                    }
                    if (doubleTextBox.UseNullOption)
                    {
                        doubleTextBox.SetValue(true, null);
                    }
                    else
                    {
                        if (doubleTextBox.MinValue.ToString() == "0" || doubleTextBox.MinValue < 0)
                        {
                            doubleTextBox.SetValue(true, 0.0);
                        }
                        else
                        {
                            doubleTextBox.SetValue(true, doubleTextBox.MinValue);
                        }
                    }
                    return true;
                }
                numberFormat = doubleTextBox.GetCulture().NumberFormat;
                doubleTextBox.MaskedText = preValue.ToString("N", numberFormat);
                maskedText = doubleTextBox.MaskedText;
                doubleTextBox.SetValue(false, preValue);
            }
#if WPF
            if (!doubleTextBox.OnValidating(new CancelEventArgs(false)))
            {
                if (doubleTextBox.ValueValidation == StringValidation.OnKeyPress)
                {
                    string validationerror = "";
                    bool validationstatus = true;

                    if (doubleTextBox.ValidationValue == doubleTextBox.Value.ToString())
                        validationstatus = true;
                    else
                        validationstatus = false;

                    string message = validationstatus ? "String validation succeeded" : "String validation failed";

                    if (!validationstatus)
                    {
                        if (doubleTextBox.InvalidValueBehavior == InvalidInputBehavior.DisplayErrorMessage)
                        {
                            MessageBox.Show(message, "Invalid value", MessageBoxButton.OK);
                            doubleTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, doubleTextBox.ValidationValue));
                            doubleTextBox.OnValidated(EventArgs.Empty);
                            return true;
                        }
                        else if (doubleTextBox.InvalidValueBehavior == InvalidInputBehavior.ResetValue)
                        {
                            doubleTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, doubleTextBox.ValidationValue));
                            doubleTextBox.OnValidated(EventArgs.Empty);
                            return true;
                        }
                        else if (doubleTextBox.InvalidValueBehavior == InvalidInputBehavior.None)
                        {
                            doubleTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, doubleTextBox.ValidationValue));
                            doubleTextBox.OnValidated(EventArgs.Empty);
                        }
                    }
                    else
                    {
                        doubleTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, doubleTextBox.ValidationValue));
                        doubleTextBox.OnValidated(EventArgs.Empty);
                    }
                    return false;
                }
            }
#endif
            return true;
        }

        public bool HandleDeleteKey(DoubleTextBox doubleTextBox)
        {
            if (doubleTextBox.IsReadOnly)
                return true;

            NumberFormatInfo numberFormat = doubleTextBox.GetCulture().NumberFormat;
            count = 0;
            string maskedText = doubleTextBox.MaskedText;
            int selectionStart = 0;
            int selectionEnd = 0;
            int selectionLength = 0;
            int separatorStart = maskedText.IndexOf(numberFormat.NumberDecimalSeparator);
            int separatorEnd = separatorStart + numberFormat.NumberDecimalSeparator.Length;
            int negflag = 0;
            int caretPosition = doubleTextBox.SelectionStart;
            string unmaskedText = "";
            if (doubleTextBox.SelectionLength <= 1 && doubleTextBox.SelectionStart != maskedText.Length)
            {
                if (!char.IsDigit(maskedText[doubleTextBox.SelectionStart]))
                {
                    if (maskedText[doubleTextBox.SelectionStart] == '-' && !(doubleTextBox.Value < 0 && doubleTextBox.MaxValue < 0))
                    {
                        doubleTextBox.Value = (doubleTextBox.Value * -1);
                        doubleTextBox.SelectionLength = 0;
                        return true;
                    }
                }
                if (doubleTextBox.NumberFormat != null)
                {
                    if (doubleTextBox.SelectionStart == maskedText.Length - (doubleTextBox.NumberFormat.NumberDecimalDigits + 2))
                    {
                        if (maskedText[doubleTextBox.SelectionStart] == '0' && doubleTextBox.SelectionStart == 0)
                        {
                            negflag = 1;
                        }
                    }
                }
                if (doubleTextBox.NumberFormat == null)
                {
                    if (maskedText[doubleTextBox.SelectionStart] == '0' && doubleTextBox.SelectionStart == 0)
                    {
                        negflag = 1;
                        unmaskedText = "";
                        doubleTextBox.SelectionStart++;
                        return true;
                    }
                    if (doubleTextBox.SelectionStart == 1)
                    {
                        negflag = 1;
                    }
                }
                string numbergroup;
                if (numberFormat == null)
                    numbergroup = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
                else
                    numbergroup = numberFormat.NumberGroupSeparator;
                if ((doubleTextBox.NumberGroupSeparator == string.Empty && maskedText[doubleTextBox.SelectionStart].ToString() == numbergroup) || (maskedText[doubleTextBox.SelectionStart].ToString() == numbergroup))
                {
                    unmaskedText = "";
                    doubleTextBox.SelectionStart++;
                    doubleTextBox.SelectionLength = 0;
                    return true;
                }
            }

            int i;
            for (i = 0; i <= maskedText.Length; i++)
            {
                if (i == doubleTextBox.SelectionStart)
                {
                    selectionStart = unmaskedText.Length;
                    caretPosition = selectionStart;
                }
                if (i == (doubleTextBox.SelectionStart + doubleTextBox.SelectionLength))
                    selectionEnd = unmaskedText.Length;

                if (i == separatorEnd)
                    separatorEnd = unmaskedText.Length;

                if (i == separatorStart)
                {
                    separatorStart = unmaskedText.Length;
                    unmaskedText += CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToString();
                }

                if (i < maskedText.Length)
                {
                    if (char.IsDigit(maskedText[i]))
                        unmaskedText += maskedText[i];
                }
            }
            selectionLength = selectionEnd - selectionStart;

            if (separatorStart < 0)
            {
                separatorStart = unmaskedText.Length;
                separatorEnd = unmaskedText.Length;
            }
            if (selectionStart <= separatorStart && selectionEnd >= separatorEnd && unmaskedText.Length > 0)
            {
                if (numberFormat != null && selectionLength < unmaskedText.Length)
                {
                    for (int decpos = 0; decpos < doubleTextBox.SelectionLength; decpos++)
                    {
                        if (unmaskedText.Length > 0)
                        {
                            if (numberFormat != null && unmaskedText != string.Empty && unmaskedText.Length != selectionStart)
                            {
                                if (unmaskedText[selectionStart].ToString() != numberFormat.NumberDecimalSeparator && unmaskedText[selectionStart].ToString() != doubleTextBox.NumberDecimalSeparator)
                                {
                                    unmaskedText = unmaskedText.Remove(selectionStart, 1);
                                }
                                else
                                    selectionStart++;
                            }
                        }
                    }
                    if (doubleTextBox.IsExceedDecimalDigits && unmaskedText.Contains(numberFormat.NumberDecimalSeparator))
                    {
                        for (int len = unmaskedText.Length - 1; len >= 0; len--)
                        {
                            if (numberFormat != null)
                            {
                                if (unmaskedText[len].ToString() == numberFormat.NumberDecimalSeparator || unmaskedText.Length.ToString() == doubleTextBox.NumberDecimalSeparator)
                                {
                                    break;
                                }
                                else
                                {
                                    count++;
                                }
                            }
                        }
                        if (doubleTextBox.MinimumNumberDecimalDigits >= 0)
                        {
                            if (count >= doubleTextBox.MinimumNumberDecimalDigits)
                            {
                                CanUpdate = true;
                                doubleTextBox.NumberDecimalDigits = count;
                                AllowChange = true;
                                CanUpdate = false;
                            }
                            else
                            {
                                doubleTextBox.NumberDecimalDigits = doubleTextBox.MinimumNumberDecimalDigits;
                                AllowChange = false;
                            }
                        }
                        else if (count <= doubleTextBox.numberDecimalDigits)
                        {
                            doubleTextBox.NumberDecimalDigits = doubleTextBox.numberDecimalDigits;
                            AllowChange = false;
                        }
                        else
                        {
                            CanUpdate = true;
                            doubleTextBox.NumberDecimalDigits = count;
                            CanUpdate = true;
                            AllowChange = false;
                        }
                        Allow = true;
                    }
                }
                else if (selectionLength == unmaskedText.Length)
                {
                    unmaskedText = "";
                    AllowChange = false;
                }
                if (selectionLength == unmaskedText.Length && doubleTextBox.MinValue.ToString() != "" && doubleTextBox.MinValue > 0)
                {
                    unmaskedText = doubleTextBox.MinValue.ToString();
                }
                else
                {
                }
            }
            else if (selectionStart <= separatorStart && selectionEnd < separatorEnd && unmaskedText.Length > 0)
            {
                if (selectionLength == 0)
                {
                    if (selectionStart != separatorStart)
                    {
                        selectionLength = 1;
                        if (doubleTextBox.MinValue == Double.Parse(unmaskedText))
                        {
                            if (doubleTextBox.UseNullOption && unmaskedText.Length > 0)
                                unmaskedText = unmaskedText.Remove(selectionStart, selectionLength);
                            caretPosition = selectionStart + 1;
                        }

                        else
                        {
                            unmaskedText = unmaskedText.Remove(selectionStart, selectionLength);
                            if (negflag == 1)
                            {
                                caretPosition = selectionStart + 1;
                                negflag = 0;
                            }
                            Allow = true;
                        }
                        if (doubleTextBox.SelectionStart == 1)
                        {
                            negflag = 1;
                        }
                    }

                    else
                    {
                        doubleTextBox.SelectionStart = doubleTextBox.SelectionStart + 1;
                        AllowSelectionStart = true;
                        return true;
                    }
                }
                else
                {
                    if (selectionStart == 0)
                    {
                        if (maskedText[doubleTextBox.SelectionStart] == '-')
                        {
                            doubleTextBox.Value = doubleTextBox.Value * -1;
                        }
                    }

                    unmaskedText = unmaskedText.Remove(selectionStart, selectionLength);
                    caretPosition = selectionStart;
                    if (doubleTextBox.SelectionStart == 1)
                    {
                        negflag = 1;
                    }
                }
            }
            else if (unmaskedText.Length > 0)
            {
                if (selectionStart == selectionEnd)
                {
                    if (selectionStart != unmaskedText.Length)
                    {
                        if (numberFormat != null && unmaskedText != string.Empty)
                        {
                            if (unmaskedText.Length >= selectionStart && (unmaskedText[selectionStart - 1].ToString() == numberFormat.NumberDecimalSeparator || unmaskedText[selectionStart - 1].ToString() == doubleTextBox.NumberDecimalSeparator))
                            {
                                AllowSelectionStart = true;
                            }
                            else
                                AllowSelectionStart = false;
                            if (unmaskedText[selectionStart].ToString() != numberFormat.NumberDecimalSeparator && unmaskedText[selectionStart].ToString() != doubleTextBox.NumberDecimalSeparator)
                            {
                                unmaskedText = unmaskedText.Remove(selectionStart, 1);
                            }
                        }
                        if (doubleTextBox.IsExceedDecimalDigits)
                        {
                            for (int len = unmaskedText.Length - 1; len >= 0; len--)
                            {
                                if (numberFormat != null)
                                {
                                    if (unmaskedText[len].ToString() == numberFormat.NumberDecimalSeparator || unmaskedText.Length.ToString() == doubleTextBox.NumberDecimalSeparator)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        count++;
                                    }
                                }
                            }
                            if (doubleTextBox.MinimumNumberDecimalDigits >= 0)
                            {
                                if (count >= doubleTextBox.MinimumNumberDecimalDigits)
                                {
                                    CanUpdate = true;
                                    doubleTextBox.NumberDecimalDigits = count;
                                    AllowChange = true;
                                    CanUpdate = false;
                                }
                                else
                                {
                                    CanUpdate = true;
                                    doubleTextBox.NumberDecimalDigits = doubleTextBox.MinimumNumberDecimalDigits;
                                    CanUpdate = false;
                                    AllowChange = false;
                                }
                            }
                            else if (count <= doubleTextBox.numberDecimalDigits)
                            {
                                doubleTextBox.NumberDecimalDigits = doubleTextBox.numberDecimalDigits;
                                AllowChange = false;
                            }
                            else
                            {
                                CanUpdate = true;
                                doubleTextBox.NumberDecimalDigits = count;
                                CanUpdate = true;
                                AllowChange = false;
                            }

                            Allow = true;
                        }
                        caretPosition = selectionStart - 1;
                    }
                    else
                        return true;
                }
                else
                {
                    if (!doubleTextBox.IsExceedDecimalDigits)
                    {
                        for (int decpos = 0; decpos < doubleTextBox.SelectionLength; decpos++)
                        {
                            if (unmaskedText.Length > 0)
                            {
                                if (numberFormat != null && unmaskedText != string.Empty)
                                {
                                    if (unmaskedText[selectionStart].ToString() != numberFormat.NumberDecimalSeparator)
                                    {
                                        unmaskedText = unmaskedText.Remove(selectionStart, 1);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int decpos = 0; decpos < doubleTextBox.SelectionLength; decpos++)
                        {
                            if (unmaskedText.Length > 0)
                            {
                                if (numberFormat != null)
                                {
                                    if (unmaskedText[selectionStart].ToString() != numberFormat.NumberDecimalSeparator && unmaskedText[selectionStart - 1].ToString() != doubleTextBox.NumberDecimalSeparator)
                                    {
                                        unmaskedText = unmaskedText.Remove(selectionStart, 1);
                                    }
                                }
                            }
                        }
                        for (int len = unmaskedText.Length - 1; len >= 0; len--)
                        {
                            if (numberFormat != null)
                            {
                                if (unmaskedText[len].ToString() == numberFormat.NumberDecimalSeparator || unmaskedText.Length.ToString() == doubleTextBox.NumberDecimalSeparator)
                                {
                                    break;
                                }
                                else
                                {
                                    count++;
                                }
                            }
                        }
                        if (doubleTextBox.MinimumNumberDecimalDigits >= 0)
                        {
                            if (count >= doubleTextBox.MinimumNumberDecimalDigits)
                            {
                                CanUpdate = true;
                                doubleTextBox.NumberDecimalDigits = count;
                                AllowChange = true;
                                CanUpdate = false;
                            }
                            else
                            {
                                CanUpdate = true;
                                doubleTextBox.NumberDecimalDigits = doubleTextBox.MinimumNumberDecimalDigits;
                                CanUpdate = false;
                                AllowChange = false;
                            }
                        }
                        else if (count <= doubleTextBox.numberDecimalDigits)
                        {
                            doubleTextBox.NumberDecimalDigits = doubleTextBox.numberDecimalDigits;
                            AllowChange = false;
                        }
                        else
                        {
                            CanUpdate = true;
                            doubleTextBox.NumberDecimalDigits = count;
                            AllowChange = true;
                            CanUpdate = false;
                        }
                        caretPosition--;
                        Allow = true;
                    }
                }
            }
            if (doubleTextBox.IsExceedDecimalDigits && !Allow && doubleTextBox.MinimumNumberDecimalDigits >= 0 && doubleTextBox.MaximumNumberDecimalDigits > 0)
            {
                double newValue;
                if (double.TryParse(unmaskedText, out newValue))
                    if (selectionLength > 0)
                    {
                        if (numberFormat != null)
                        {
                            doubleTextBox.NumberDecimalDigits = doubleTextBox.numberDecimalDigits;
                            AllowChange = false;
                        }
                    }
            }
            oldunmaskedText = unmaskedText;
            double preValue;
            if (double.TryParse(unmaskedText, out preValue))
            {
                if (doubleTextBox.MaskedText.Length >= 15 && doubleTextBox.MaxValidation == MaxValidation.OnLostFocus)
                {
                    if (doubleTextBox.IsNegative)
                    {
                        if (!doubleTextBox.negativeFlag)
                            unmaskedText = "-" + unmaskedText;
                    }
                    try
                    {
                        decimal newvalue = decimal.Parse(unmaskedText);
                        int startlen = doubleTextBox.MaskedText.Length;
                        selectionStart = doubleTextBox.SelectionStart;
                        doubleTextBox.MaskedText = newvalue.ToString("N", numberFormat);
                        int endlen = doubleTextBox.MaskedText.Length;
                        if (startlen != 0)
                        {
                            int totlen = startlen - endlen;
                            if (totlen == 0 && doubleTextBox.MaskedText[selectionStart - 1].ToString() == numberFormat.CurrencyDecimalSeparator)
                                doubleTextBox.SelectionStart = selectionStart;
                            else if (selectionStart + 1 == endlen)
                                doubleTextBox.SelectionStart = selectionStart + 1;
                            else if (totlen == 1)
                                doubleTextBox.SelectionStart = selectionStart;
                            else if (totlen == 2)
                                doubleTextBox.SelectionStart = selectionStart - 1;
                        }
                    }
                    catch { }
                    return true;
                }
                if (preValue == 0)
                {
                    if (doubleTextBox.IsExceedDecimalDigits && doubleTextBox.MinimumNumberDecimalDigits >= 0)
                    {
                        if (numberFormat != null)
                        {
                            CanUpdate = true;
                            doubleTextBox.NumberDecimalDigits = doubleTextBox.MinimumNumberDecimalDigits;
                            CanUpdate = false;
                        }
                    }
                    else
                    {
                        if (doubleTextBox.NumberDecimalDigits < 0)
                            doubleTextBox.NumberDecimalDigits = numberFormat.NumberDecimalDigits;
                        else
                            doubleTextBox.NumberDecimalDigits = doubleTextBox.numberDecimalDigits;
                    }
                    if (doubleTextBox.UseNullOption)
                        doubleTextBox.SetValue(true, null);
                    else
                    {
                        doubleTextBox.SelectionStart++;
                        doubleTextBox.SetValue(true, 0.0);
                    }
                    return true;
                }
                if (doubleTextBox.IsNegative)
                {
                    preValue = preValue * -1;
                }
                numberFormat = doubleTextBox.GetCulture().NumberFormat;
                if ((preValue > doubleTextBox.MaxValue) && (doubleTextBox.MaxValidation == MaxValidation.OnKeyPress))
                {
                    if (doubleTextBox.MaxValueOnExceedMaxDigit)
                        preValue = doubleTextBox.MaxValue;
                    else return true;
                }

                if ((preValue < doubleTextBox.MinValue) && (doubleTextBox.MinValidation == MinValidation.OnKeyPress))
                {
                    if (preValue <= doubleTextBox.MinValue && doubleTextBox.MinValue >= 0)
                    {
                        if (numberFormat != null)
                            if (unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) >= (doubleTextBox.MinValue.ToString()).Length)
                            {
                                if (doubleTextBox.MinValueOnExceedMinDigit)
                                    preValue = doubleTextBox.MinValue;
                                else
                                    return true;
                            }
                            else if (unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) <= (doubleTextBox.MinValue.ToString()).Length)
                            {
                                if (doubleTextBox.MinValueOnExceedMinDigit)
                                    preValue = doubleTextBox.MinValue;
                                else if (preValue >= doubleTextBox.MinValue)
                                    doubleTextBox.MaskedText = unmaskedText;
                                else
                                    return true;
                            }
                            else return true;
                    }
                    else if (preValue > doubleTextBox.MinValue)
                    {
                        doubleTextBox.MaskedText = unmaskedText;
                    }
                    else if (preValue >= doubleTextBox.MinValue)
                    {
                        if (doubleTextBox.MinValueOnExceedMinDigit && unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) > (doubleTextBox.MinValue.ToString()).Length)
                            preValue = doubleTextBox.MinValue;
                        else if (unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) <= (doubleTextBox.MinValue.ToString()).Length)
                        {
                            doubleTextBox.MaskedText = unmaskedText;
                        }
                        else return true;
                    }
                    else if (doubleTextBox.MinValueOnExceedMinDigit)
                        preValue = doubleTextBox.MinValue;
                    else
                        return true;
                }

                doubleTextBox.MaskedText = preValue.ToString("N", numberFormat);
                maskedText = doubleTextBox.MaskedText;
                doubleTextBox.SetValue(false, preValue);
                if (negflag == 0)
                {
                    int j = 0;
                    for (i = 0; i < unmaskedText.Length; i++)
                    {
                        if (i == caretPosition)
                        {
                            break;
                        }
                        if (j == maskedText.Length)
                            break;
                        if (char.IsDigit(maskedText[j]))
                            j++;
                        else
                        {
                            for (int k = j; k < maskedText.Length; k++)
                            {
                                if (char.IsDigit(maskedText[k]))
                                    break;
                                j++;
                            }
                            i--;
                        }
                    }
                    if (!AllowSelectionStart)
                    {
                        doubleTextBox.SelectionStart = j;
                        selectionStart = j;
                    }
                    else
                    {
                        if (j > 0)
                            doubleTextBox.SelectionStart = j + 1;
                    }
                }
                else
                {
                    if (doubleTextBox.Value < 0)
                    {
                        doubleTextBox.SelectionStart++;
                        selectionStart++;
                    }
                    else
                    {
                        doubleTextBox.SelectionStart = 1;
                        selectionStart = 1;
                    }
                }
                doubleTextBox.SelectionLength = 0;
                negflag = 0;
            }
            else
            {
                if (preValue == 0)
                {
                    if (doubleTextBox.IsExceedDecimalDigits && doubleTextBox.MinimumNumberDecimalDigits >= 0)
                    {
                        if (numberFormat != null)
                        {
                            CanUpdate = true;
                            doubleTextBox.NumberDecimalDigits = doubleTextBox.MinimumNumberDecimalDigits;
                            AllowChange = false;
                            CanUpdate = false;
                        }
                    }
                    else
                    {
                        if (doubleTextBox.NumberDecimalDigits < 0)
                            doubleTextBox.NumberDecimalDigits = numberFormat.NumberDecimalDigits;
                        else
                            doubleTextBox.NumberDecimalDigits = doubleTextBox.numberDecimalDigits;
                    }
                    numberFormat = doubleTextBox.GetCulture().NumberFormat;
                    if (doubleTextBox.UseNullOption)
                    {
                        doubleTextBox.SetValue(true, null);
                    }
                    else
                    {
                        if (doubleTextBox.MinValue.ToString() == "0" || doubleTextBox.MinValue < 0)
                        {
                            doubleTextBox.SetValue(true, 0.0);
                        }
                        else
                        {
                            doubleTextBox.SetValue(true, doubleTextBox.MinValue);
                        }
                    }

                    return true;
                }
                doubleTextBox.MaskedText = preValue.ToString("N", numberFormat);
                maskedText = doubleTextBox.MaskedText;
                doubleTextBox.SetValue(false, preValue);
            }
#if WPF
            if (!doubleTextBox.OnValidating(new CancelEventArgs(false)))
            {
                if (doubleTextBox.ValueValidation == StringValidation.OnKeyPress)
                {
                    string validationerror = "";
                    bool validationstatus = true;

                    if (doubleTextBox.ValidationValue == doubleTextBox.Value.ToString())
                        validationstatus = true;
                    else
                        validationstatus = false;

                    string message = validationstatus ? "String validation succeeded" : "String validation failed";

                    if (!validationstatus)
                    {
                        if (doubleTextBox.InvalidValueBehavior == InvalidInputBehavior.DisplayErrorMessage)
                        {
                            MessageBox.Show(message, "Invalid value", MessageBoxButton.OK);
                            doubleTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, doubleTextBox.ValidationValue));
                            doubleTextBox.OnValidated(EventArgs.Empty);
                            return true;
                        }
                        else if (doubleTextBox.InvalidValueBehavior == InvalidInputBehavior.ResetValue)
                        {
                            doubleTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, doubleTextBox.ValidationValue));
                            doubleTextBox.OnValidated(EventArgs.Empty);
                            return true;
                        }
                        else if (doubleTextBox.InvalidValueBehavior == InvalidInputBehavior.None)
                        {
                            doubleTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, doubleTextBox.ValidationValue));
                            doubleTextBox.OnValidated(EventArgs.Empty);
                        }
                    }
                    else
                    {
                        doubleTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, doubleTextBox.ValidationValue));
                        doubleTextBox.OnValidated(EventArgs.Empty);
                    }
                    return true;
                }
            }
#endif
            return true;
        }

        public bool HandleDownKey(DoubleTextBox doubleTextBox)
        {
            if (doubleTextBox.IsReadOnly)
                return true;

            if (doubleTextBox.mValue != null)
            {
                if (((doubleTextBox.mValue - doubleTextBox.ScrollInterval) < doubleTextBox.MinValue) && (doubleTextBox.MinValidation == MinValidation.OnKeyPress))
                {
                    return true;
                }
                else
                {
                    doubleTextBox.SetValue(true, doubleTextBox.mValue - doubleTextBox.ScrollInterval);
                }
            }
#if WPF
            if (!doubleTextBox.OnValidating(new CancelEventArgs(false)))
            {
                if (doubleTextBox.ValueValidation == StringValidation.OnKeyPress)
                {
                    string validationerror = "";
                    bool validationstatus = true;
                    if (doubleTextBox.ValidationValue == doubleTextBox.Value.ToString())
                        validationstatus = true;
                    else
                        validationstatus = false;

                    string message = validationstatus ? "String validation succeeded" : "String validation failed";

                    if (!validationstatus)
                    {
                        if (doubleTextBox.InvalidValueBehavior == InvalidInputBehavior.DisplayErrorMessage)
                        {
                            MessageBox.Show(message, "Invalid value", MessageBoxButton.OK);
                            doubleTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, doubleTextBox.ValidationValue));
                            doubleTextBox.OnValidated(EventArgs.Empty);
                            return true;
                        }
                        else if (doubleTextBox.InvalidValueBehavior == InvalidInputBehavior.ResetValue)
                        {
                            doubleTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, doubleTextBox.ValidationValue));
                            doubleTextBox.OnValidated(EventArgs.Empty);
                            return true;
                        }
                        else if (doubleTextBox.InvalidValueBehavior == InvalidInputBehavior.None)
                        {
                            doubleTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, doubleTextBox.ValidationValue));
                            doubleTextBox.OnValidated(EventArgs.Empty);
                        }
                    }
                    else
                    {
                        doubleTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, doubleTextBox.ValidationValue));
                        doubleTextBox.OnValidated(EventArgs.Empty);
                    }
                    return true;
                }
            }
#endif

            return true;
        }

        public bool HandleUpKey(DoubleTextBox doubleTextBox)
        {
            if (doubleTextBox.IsReadOnly)
                return true;

            if (doubleTextBox.mValue != null)
            {
                if (((doubleTextBox.mValue + doubleTextBox.ScrollInterval) > doubleTextBox.MaxValue) && (doubleTextBox.MaxValidation == MaxValidation.OnKeyPress))
                {
                    return true;
                }
                else if (((doubleTextBox.mValue + doubleTextBox.ScrollInterval) < doubleTextBox.MinValue) && (doubleTextBox.MinValidation == MinValidation.OnKeyPress))
                {
                    return true;
                }
                else
                {
                    doubleTextBox.SetValue(true, doubleTextBox.mValue + doubleTextBox.ScrollInterval);
                }
            }
#if WPF
            if (!doubleTextBox.OnValidating(new CancelEventArgs(false)))
            {
                if (doubleTextBox.ValueValidation == StringValidation.OnKeyPress)
                {
                    string validationerror = "";
                    bool validationstatus = true;

                    if (doubleTextBox.ValidationValue == doubleTextBox.Value.ToString())
                        validationstatus = true;
                    else
                        validationstatus = false;

                    string message = validationstatus ? "String validation succeeded" : "String validation failed";

                    if (!validationstatus)
                    {
                        if (doubleTextBox.InvalidValueBehavior == InvalidInputBehavior.DisplayErrorMessage)
                        {
                            MessageBox.Show(message, "Invalid value", MessageBoxButton.OK);
                            doubleTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, doubleTextBox.ValidationValue));
                            doubleTextBox.OnValidated(EventArgs.Empty);
                            return true;
                        }
                        else if (doubleTextBox.InvalidValueBehavior == InvalidInputBehavior.ResetValue)
                        {
                            doubleTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, doubleTextBox.ValidationValue));
                            doubleTextBox.OnValidated(EventArgs.Empty);
                            return true;
                        }
                        else if (doubleTextBox.InvalidValueBehavior == InvalidInputBehavior.None)
                        {
                            doubleTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, doubleTextBox.ValidationValue));
                            doubleTextBox.OnValidated(EventArgs.Empty);
                        }
                    }
                    else
                    {
                        doubleTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, doubleTextBox.ValidationValue));
                        doubleTextBox.OnValidated(EventArgs.Empty);
                    }
                    return true;
                }
            }
#endif
            return true;
        }
    }
}
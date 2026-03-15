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
    internal class PercentValueHandler
    {
        public static PercentValueHandler percentValueHandler = new PercentValueHandler();

        public bool MatchWithMask(PercentTextBox percentTextBox, string text)
        {
            if (percentTextBox.IsReadOnly)
                return true;

            bool minusKeyValidationflag = false;
            if (percentTextBox.mValue == null || percentTextBox.mValue == 0)
            {
                if (text == "-")
                {
                    percentTextBox.minusPressed = true;
                    percentTextBox.MaskedText = "-";
                    percentTextBox.PercentValue = 0;
                    percentTextBox.CaretIndex = 1;
                    return true;
                }
                else
                {
                    if (percentTextBox.minusPressed == true)
                    {
                        percentTextBox.minusPressed = false;
                        minusKeyValidationflag = true;
                    }
                }
            }

            NumberFormatInfo numberFormat = percentTextBox.GetCulture().NumberFormat;

            if (text == "-" || text == "+")
            {
                double tempVal;
                tempVal = (double)percentTextBox.mValue;

                if (percentTextBox.mValue != null)
                {
                    if (text == "+")
                    {
                        if (percentTextBox.PercentValue < 1)
                            tempVal = (double)percentTextBox.mValue * -1;
                        else
                            tempVal = (double)percentTextBox.mValue * 1;
                    }
                    else
                        tempVal = (double)percentTextBox.mValue * -1;

                    if ((tempVal > percentTextBox.MaxValue) && (percentTextBox.MaxValidation == MaxValidation.OnKeyPress) && !percentTextBox.ValidationOnLostFocus)
                    {
                        if (percentTextBox.MaxValueOnExceedMaxDigit)
                            tempVal = percentTextBox.MaxValue;
                        else return true;
                    }

                    if ((tempVal < percentTextBox.MinValue) && (percentTextBox.MinValidation == MinValidation.OnKeyPress) && !percentTextBox.ValidationOnLostFocus)
                    {
                        if (percentTextBox.MinValueOnExceedMinDigit)
                            tempVal = percentTextBox.MinValue;
                        else return true;
                    }
                    bool select = false;
                    int selectedLength = percentTextBox.SelectedText.Length;
                    if (selectedLength == percentTextBox.MaskedText.Length)
                    {
                        select = true;
                        selectedLength = 0;
                        if (text == "-")
                        {
                            percentTextBox.minusPressed = true;
                            if (percentTextBox.UseNullOption)
                            {
                                percentTextBox.SetValue(false, null);
                            }
                            else
                                percentTextBox.PercentValue = 0;
                            percentTextBox.MaskedText = "-";
                            percentTextBox.CaretIndex = 1;
                            return true;
                        }
                    }
                    if (percentTextBox.PercentEditMode == PercentEditMode.DoubleMode)
                        percentTextBox.MaskedText = (tempVal / 100).ToString("P", numberFormat);
                    else
                        percentTextBox.MaskedText = (tempVal).ToString("P", numberFormat);
                    percentTextBox.SetValue(false, tempVal);
                    if (percentTextBox.MaskedText.Contains("-") == true)
                        percentTextBox.SelectionStart++;
                    if (select)
                        percentTextBox.SelectAll();
                }
                return true;
            }

            int selectionStart = 0;
            int selectionEnd = 0;
            int selectionLength = 0;
            string maskedText = percentTextBox.MaskedText;
            int separatorStart = maskedText.IndexOf(numberFormat.PercentDecimalSeparator);
            int separatorEnd = separatorStart + numberFormat.PercentDecimalSeparator.Length;

            if (text == numberFormat.NumberDecimalSeparator || text == ".")
            {
                percentTextBox.SelectionStart = separatorEnd;
                if (percentTextBox.Text == "" && (text == "." || text == numberFormat.PercentDecimalSeparator))
                {
                    percentTextBox.MaskedText = "0" + numberFormat.PercentDecimalSeparator + "00";
                    percentTextBox.PercentValue = Convert.ToDouble(percentTextBox.MaskedText);
                    percentTextBox.Select(2, 0);
                }
                return true;
            }
            if (percentTextBox.GroupSeperatorEnabled == true)
            {
                if (text == numberFormat.NumberGroupSeparator)
                {
                    if (percentTextBox.SelectionStart < percentTextBox.Text.Length)
                    {
                        if (percentTextBox.Text[percentTextBox.SelectionStart].ToString() == numberFormat.NumberGroupSeparator.ToString())
                        {
                            percentTextBox.SelectionStart += 1;
                            return true;
                        }
                    }
                    return true;
                }
            }

            int caretPosition = percentTextBox.SelectionStart;
            string unmaskedText = "";

            int i;
            for (i = 0; i <= maskedText.Length; i++)
            {
                if (i == percentTextBox.SelectionStart)
                {
                    selectionStart = unmaskedText.Length;
                    caretPosition = selectionStart;
                }
                if (i == (percentTextBox.SelectionStart + percentTextBox.SelectionLength))
                    selectionEnd = unmaskedText.Length;

                if (i == separatorEnd)
                    separatorEnd = unmaskedText.Length;

                if (i == separatorStart)
                {
                    separatorStart = unmaskedText.Length;
                    unmaskedText += CultureInfo.CurrentCulture.NumberFormat.PercentDecimalSeparator.ToString();
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

            if (selectionStart <= separatorStart && selectionEnd >= separatorEnd)
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
                    if (selectionStart == unmaskedText.Length)
                    {
                        unmaskedText = unmaskedText.Insert(unmaskedText.Length - 1, text[0].ToString());
                        unmaskedText = unmaskedText.Remove(unmaskedText.Length - 1, 1);
                    }
                    else if ( selectionStart < unmaskedText.Length && numberFormat != null && selectionStart > unmaskedText.IndexOf((numberFormat.NumberDecimalSeparator)))
                    {
                        unmaskedText = unmaskedText.Insert(selectionStart, text[0].ToString());
                        unmaskedText = unmaskedText.Remove(percentTextBox.CaretIndex +1,1);
                    }
                    else if (selectionStart != unmaskedText.Length)
                    {
                        if (text != "")
                            unmaskedText = unmaskedText.Insert(selectionStart, text[0].ToString());
                        if (percentTextBox.PercentValue > -1 && percentTextBox.PercentValue < 1 )
                        {
                            caretPosition = selectionStart + 1;
                        }
                    }
                    else
                        return true;
                }
                else
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
            }

            #endregion TextChange

            double preValue;
            if (double.TryParse(unmaskedText, out preValue))
            {
                double tempValue = preValue;

                if (percentTextBox.IsNegative || minusKeyValidationflag)
                {
                    if (!(percentTextBox.SelectedText == percentTextBox.Text.ToString()) && !percentTextBox.SelectedText.Contains("-"))
                    {
                        preValue = preValue * -1;
                        tempValue = preValue;
                    }
                    else
                    {
                        if ((percentTextBox.PercentValue == 0 || percentTextBox.IsNull) && minusKeyValidationflag)
                        {
                            preValue = preValue * -1;
                            tempValue = preValue;
                        }
                    }
                }

                if (percentTextBox.PercentEditMode == PercentEditMode.PercentMode)
                    tempValue = preValue / 100;

                if ((tempValue > percentTextBox.MaxValue) && (percentTextBox.MaxValidation == MaxValidation.OnKeyPress) && !percentTextBox.ValidationOnLostFocus)
                {
                    if (percentTextBox.MaxValueOnExceedMaxDigit)
                        tempValue = percentTextBox.MaxValue;
                    else return true;
                }

                if ((tempValue < percentTextBox.MinValue) && (percentTextBox.MinValidation == MinValidation.OnKeyPress) && !percentTextBox.ValidationOnLostFocus)
                {
                    if (tempValue <= percentTextBox.MinValue && percentTextBox.MinValue >= 0)
                    {
                        if (numberFormat != null)
                        {
                            if (percentTextBox.UseNullOption)
                            {
                                unmaskedText = tempValue.ToString("N", numberFormat);
                            }
                            if (unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) >= (percentTextBox.MinValue.ToString()).Length)
                                tempValue = percentTextBox.MinValue;
                            else if (unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) <= (percentTextBox.MinValue.ToString()).Length)
                            {
                                percentTextBox.checktext = percentTextBox.checktext + text;
                                if (double.Parse(percentTextBox.checktext) > percentTextBox.MinValue)
                                {
                                    percentTextBox.PercentValue = double.Parse(percentTextBox.checktext);
                                    percentTextBox.CaretIndex = percentTextBox.PercentValue.ToString().Length;
                                }
                                return true;
                            }
                        }
                    }
                    else if (tempValue > percentTextBox.MinValue)
                    {
                        percentTextBox.MaskedText = unmaskedText;
                    }
                    else if (tempValue >= percentTextBox.MinValue)
                    {
                        if (percentTextBox.MinValueOnExceedMinDigit && unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) > (percentTextBox.MinValue.ToString()).Length)
                            tempValue = percentTextBox.MinValue;
                        else if (unmaskedText.Length - (numberFormat.NumberDecimalDigits + 1) <= (percentTextBox.MinValue.ToString()).Length)
                        {
                            percentTextBox.MaskedText = unmaskedText;
                        }
                        else return true;
                    }
                    else
                    {
                        if (percentTextBox.MinValueOnExceedMinDigit)
                            tempValue = percentTextBox.MinValue;
                        else return true;
                    }
                }

                if (percentTextBox.PercentEditMode == PercentEditMode.PercentMode)
                    percentTextBox.MaskedText = tempValue.ToString("P", numberFormat);
                else
                    percentTextBox.MaskedText = (tempValue / 100).ToString("P", numberFormat);

                maskedText = percentTextBox.MaskedText;
                percentTextBox.SetValue(false, tempValue);

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
                percentTextBox.SelectionStart = j;
                percentTextBox.SelectionLength = 0;
            }
#if WPF
            if (!percentTextBox.OnValidating(new CancelEventArgs(false)))
            {
                if (percentTextBox.ValueValidation == StringValidation.OnKeyPress)
                {
                    string validationerror = "";
                    bool validationstatus = true;

                    if (percentTextBox.ValidationValue == percentTextBox.PercentValue.ToString())
                        validationstatus = true;
                    else
                        validationstatus = false;

                    string message = validationstatus ? "String validation succeeded" : "String validation failed";
                    if (!validationstatus)
                    {
                        if (percentTextBox.InvalidValueBehavior == InvalidInputBehavior.DisplayErrorMessage)
                        {
                            MessageBox.Show(message, "Invalid value", MessageBoxButton.OK);
                            percentTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, percentTextBox.ValidationValue));
                            percentTextBox.OnValidated(EventArgs.Empty);
                            return true;
                        }
                        else if (percentTextBox.InvalidValueBehavior == InvalidInputBehavior.ResetValue)
                        {
                            percentTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, percentTextBox.ValidationValue));
                            percentTextBox.OnValidated(EventArgs.Empty);
                            return true;
                        }
                        else if (percentTextBox.InvalidValueBehavior == InvalidInputBehavior.None)
                        {
                            percentTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, percentTextBox.ValidationValue));
                            percentTextBox.OnValidated(EventArgs.Empty);
                        }
                    }
                    else
                    {
                        percentTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, percentTextBox.ValidationValue));
                        percentTextBox.OnValidated(EventArgs.Empty);
                    }
                    return true;
                }
            }
#endif
            return true;
        }

        public double? ValueFromText(PercentTextBox percentTextBox, string maskedText)
        {
            double preValue;
            if (double.TryParse(maskedText, NumberStyles.Number, percentTextBox.GetCulture().NumberFormat, out preValue))
                return preValue;
            else
            {
                if (percentTextBox.UseNullOption)
                    return null;
                else
                {
                    double temp = 0;
                    if (temp > percentTextBox.MaxValue && percentTextBox.MinValidation == MinValidation.OnKeyPress && !percentTextBox.ValidationOnLostFocus)
                    {
                        temp = percentTextBox.MaxValue;
                    }
                    else if (temp < percentTextBox.MinValue && percentTextBox.MaxValidation == MaxValidation.OnKeyPress && !percentTextBox.ValidationOnLostFocus)
                    {
                        temp = percentTextBox.MinValue;
                    }
                    return temp;
                }
            }
        }

        public bool HandleKeyDown(PercentTextBox percentTextBox, KeyEventArgs eventArgs)
        {
            if (eventArgs.Key == Key.Space)
                return true;
            switch (eventArgs.Key)
            {
                case Key.None:
                    break;
#if WPF
                case Key.Cancel:
                    break;
#endif
                case Key.Back:
                    return HandleBackSpaceKey(percentTextBox);
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
                    return HandleUpKey(percentTextBox);
                case Key.Right:
                    break;

                case Key.Down:
                    return HandleDownKey(percentTextBox);
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
                    return HandleDeleteKey(percentTextBox);
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
                //case Key.DeadCharProcessed:
                //    break;
#endif
#if SILVERLIGHT
                case Key.Unknown:
                    break;
#endif
                default:
                    break;
                //throw new ArgumentOutOfRangeException();
            }
            return false;
        }

        public bool HandleBackSpaceKey(PercentTextBox percentTextBox)
        {
            if (percentTextBox.IsReadOnly)
                return true;

            NumberFormatInfo numberFormat = percentTextBox.GetCulture().NumberFormat;

            string maskedText = percentTextBox.MaskedText;
            int selectionStart = 0;
            int selectionEnd = 0;
            int selectionLength = 0;
            int separatorStart = maskedText.IndexOf(numberFormat.PercentDecimalSeparator);
            int separatorEnd = separatorStart + numberFormat.PercentDecimalSeparator.Length;

            int caretPosition = percentTextBox.SelectionStart;
            string unmaskedText = "";

            if (percentTextBox.SelectionLength == 1)
            {
                if (!char.IsDigit(maskedText[percentTextBox.SelectionStart]))
                {
                    percentTextBox.SelectionLength = 0;
                    return true;
                }
            }
            else if (percentTextBox.SelectionLength == 0 && percentTextBox.SelectionStart != 0)
            {
                if (!char.IsDigit(maskedText[percentTextBox.SelectionStart - 1]))
                {
                    if (maskedText[percentTextBox.SelectionStart - 1] == '-')
                    {
                        unmaskedText = percentTextBox.MaskedText;
                        unmaskedText = maskedText.Remove(selectionStart, 1);
                        percentTextBox.MaskedText = unmaskedText;
                        maskedText = unmaskedText;
                        int len = maskedText.Length;
                        if (!string.IsNullOrEmpty(maskedText))
                            unmaskedText = maskedText.Remove(len - 1, 1);
                        double value;
                        if (double.TryParse(unmaskedText, out value))
                        {
                            percentTextBox.SetValue(false, value);
                        }
                        //percentTextBox.SelectionStart--;
                        percentTextBox.SelectionLength = 0;
                        return true;
                    }
                    if (maskedText[percentTextBox.SelectionStart - 1].ToString() == "," || maskedText[percentTextBox.SelectionStart - 1].ToString() == "%")
                    {
                        unmaskedText = "";
                        percentTextBox.SelectionStart--;
                        percentTextBox.SelectionLength = 0;
                        return true;
                    }
                }
            }

            int i;
            for (i = 0; i <= maskedText.Length; i++)
            {
                if (i == percentTextBox.SelectionStart)
                {
                    selectionStart = unmaskedText.Length;
                    caretPosition = selectionStart;
                }
                if (i == (percentTextBox.SelectionStart + percentTextBox.SelectionLength))
                    selectionEnd = unmaskedText.Length;

                if (i == separatorEnd)
                    separatorEnd = unmaskedText.Length;

                if (i == separatorStart)
                {
                    separatorStart = unmaskedText.Length;
                    unmaskedText += CultureInfo.CurrentCulture.NumberFormat.PercentDecimalSeparator.ToString();
                }

                if (i < maskedText.Length)
                {
                    if (char.IsDigit(maskedText[i]) || maskedText[i] == '-')
                        unmaskedText += maskedText[i];
                }
            }
            selectionLength = selectionEnd - selectionStart;

            if (separatorStart < 0)
            {
                separatorStart = unmaskedText.Length;
                separatorEnd = unmaskedText.Length;
            }

            if (unmaskedText.Length == selectionLength)
            {
                //percentTextBox.SetValue(true, null);
                //return true;
                if (percentTextBox.UseNullOption)
                {
                    if (percentTextBox.MinValue.ToString() == "0" || percentTextBox.MinValue < 0)
                    {
                        percentTextBox.SetValue(true, null);
                    }
                    else
                    {
                        percentTextBox.SetValue(true, percentTextBox.MinValue);
                        percentTextBox.SelectionStart = 0;
                    }
                }
                else
                {
                    if (percentTextBox.MinValue.ToString() == "0" || percentTextBox.MinValue < 0)
                    {
                        percentTextBox.SetValue(true, 0.0);
                        percentTextBox.SelectionStart = 0;
                    }
                    else
                    {
                        percentTextBox.SetValue(true, percentTextBox.MinValue);
                        percentTextBox.SelectionStart = 0;
                    }
                }
                return true;
            }

            if (selectionStart <= separatorStart && selectionEnd >= separatorEnd)
            {
                for (int decpos = separatorEnd; decpos < selectionEnd; decpos++)
                {
                    if (decpos != unmaskedText.Length)
                    {
                        unmaskedText = unmaskedText.Remove(decpos, 1);
                        unmaskedText = unmaskedText.Insert(decpos, "0");
                    }
                }
                if (separatorStart == separatorEnd)
                {
                    unmaskedText = unmaskedText.Remove(selectionStart - 1, (separatorStart - (selectionStart - 1)));
                }
                else
                {
                    unmaskedText = unmaskedText.Remove(selectionStart, (separatorStart - selectionStart));
                }
                caretPosition = selectionStart;
            }
            else if (selectionStart <= separatorStart && selectionEnd < separatorEnd)
            {
                if (selectionLength == 0)
                {
                    if (selectionStart != 0)
                    {
                        selectionLength = 1;
                        unmaskedText = unmaskedText.Remove(selectionStart - 1, selectionLength);
                        caretPosition = selectionStart - 1;
                    }
                    else
                        return true;
                }
                else
                {
                    unmaskedText = unmaskedText.Remove(selectionStart, selectionLength);
                    caretPosition = selectionStart;
                }
            }
            else
            {
                if (selectionStart == selectionEnd)
                {
                    if (selectionStart != separatorEnd)
                    {
                        unmaskedText = unmaskedText.Remove(selectionStart - 1, 1);
                        unmaskedText = unmaskedText.Insert(selectionStart - 1, "0");
                        caretPosition = selectionStart - 1;
                    }
                    else
                    {
                        percentTextBox.SelectionStart = percentTextBox.SelectionStart - 1;
                        return true;
                    }
                }
                else
                {
                    for (int decpos = selectionStart; decpos < selectionEnd; decpos++)
                    {
                        unmaskedText = unmaskedText.Remove(decpos, 1);
                        unmaskedText = unmaskedText.Insert(decpos, "0");
                    }
                    caretPosition = selectionStart;
                }
            }

            double preValue;
            bool separatorflag = false;
            if (double.TryParse(unmaskedText, out preValue))
            {
                if (percentTextBox.IsNegative)
                {
                    preValue = preValue * 1;
                }

                if (percentTextBox.PercentEditMode == PercentEditMode.PercentMode)
                {
                    preValue = preValue / 100;
                }
                if ((preValue > percentTextBox.MaxValue) && (percentTextBox.MaxValidation == MaxValidation.OnKeyPress) && !percentTextBox.ValidationOnLostFocus)
                {
                    if (percentTextBox.MaxValueOnExceedMaxDigit)
                        preValue = percentTextBox.MaxValue;
                    else return true;
                }

                if (percentTextBox.UseNullOption && preValue == 0)
                {
                    percentTextBox.SetValue(true, null);
                    return true;
                }
                if ((preValue < percentTextBox.MinValue) && (percentTextBox.MinValidation == MinValidation.OnKeyPress) && !percentTextBox.ValidationOnLostFocus)
                {
                    if (percentTextBox.MinValueOnExceedMinDigit)
                        preValue = percentTextBox.MinValue;
                    else return true;
                }

                if (percentTextBox.PercentEditMode == PercentEditMode.DoubleMode)
                    percentTextBox.MaskedText = (preValue / 100).ToString("P", numberFormat);
                else
                    percentTextBox.MaskedText = preValue.ToString("P", numberFormat);

                maskedText = percentTextBox.MaskedText;
                percentTextBox.SetValue(false, preValue);

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
                            if (j == maskedText.IndexOf(numberFormat.PercentDecimalSeparator))
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
                percentTextBox.SelectionStart = j;
                percentTextBox.SelectionLength = 0;
            }
            else if ((unmaskedText.Length == 0) && percentTextBox.PercentDecimalDigits == 0)
            {
                percentTextBox.MaskedText = preValue.ToString("P", numberFormat);
            }
#if WPF
            if (!percentTextBox.OnValidating(new CancelEventArgs(false)))
            {
                if (percentTextBox.ValueValidation == StringValidation.OnKeyPress)
                {
                    string validationerror = "";
                    bool validationstatus = true;

                    if (percentTextBox.ValidationValue == percentTextBox.PercentValue.ToString())
                        validationstatus = true;
                    else
                        validationstatus = false;

                    string message = validationstatus ? "String validation succeeded" : "String validation failed";

                    if (!validationstatus)
                    {
                        if (percentTextBox.InvalidValueBehavior == InvalidInputBehavior.DisplayErrorMessage)
                        {
                            MessageBox.Show(message, "Invalid value", MessageBoxButton.OK);
                            percentTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, percentTextBox.ValidationValue));
                            percentTextBox.OnValidated(EventArgs.Empty);
                            return true;
                        }
                        else if (percentTextBox.InvalidValueBehavior == InvalidInputBehavior.ResetValue)
                        {
                            percentTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, percentTextBox.ValidationValue));
                            percentTextBox.OnValidated(EventArgs.Empty);
                            return true;
                        }
                        else if (percentTextBox.InvalidValueBehavior == InvalidInputBehavior.None)
                        {
                            percentTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, percentTextBox.ValidationValue));
                            percentTextBox.OnValidated(EventArgs.Empty);
                        }
                    }
                    else
                    {
                        percentTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, percentTextBox.ValidationValue));
                        percentTextBox.OnValidated(EventArgs.Empty);
                    }
                    return false;
                }
            }
#endif
            return true;
        }

        public bool HandleDeleteKey(PercentTextBox percentTextBox)
        {
            if (percentTextBox.IsReadOnly)
                return true;

            NumberFormatInfo numberFormat = percentTextBox.GetCulture().NumberFormat;

            string maskedText = percentTextBox.MaskedText;
            int selectionStart = 0;
            int selectionEnd = 0;
            int selectionLength = 0;
            int separatorStart = maskedText.IndexOf(numberFormat.PercentDecimalSeparator);
            int separatorEnd = separatorStart + numberFormat.PercentDecimalSeparator.Length;

            int caretPosition = percentTextBox.SelectionStart;
            string unmaskedText = "";

            if (percentTextBox.SelectionLength <= 1 && percentTextBox.SelectionStart != maskedText.Length)
            {
                if (!char.IsDigit(maskedText[percentTextBox.SelectionStart]))
                {
                    if (maskedText[percentTextBox.SelectionStart] == '-')
                    {
                        unmaskedText = percentTextBox.MaskedText;
                        unmaskedText = maskedText.Remove(selectionStart, 1);
                        maskedText = unmaskedText;
                        int len = maskedText.Length;
                        percentTextBox.MaskedText = unmaskedText;
                        unmaskedText = maskedText.Remove(len - 1, 1);
                        double value;
                        if (double.TryParse(unmaskedText, out value))
                        {
                            percentTextBox.SetValue(false, value);
                        }
                        //percentTextBox.SelectionStart++;
                        percentTextBox.SelectionLength = 0;
                        return true;
                    }
                    if (maskedText[percentTextBox.SelectionStart].ToString() == "," || maskedText[percentTextBox.SelectionStart].ToString() == "%")
                    {
                        unmaskedText = "";
                        percentTextBox.SelectionStart++;
                        percentTextBox.SelectionLength = 0;
                        return true;
                    }
                }
            }

            int i;
            for (i = 0; i <= maskedText.Length; i++)
            {
                if (i == percentTextBox.SelectionStart)
                {
                    selectionStart = unmaskedText.Length;
                    caretPosition = selectionStart;
                }
                if (i == (percentTextBox.SelectionStart + percentTextBox.SelectionLength))
                    selectionEnd = unmaskedText.Length;

                if (i == separatorEnd)
                    separatorEnd = unmaskedText.Length;

                if (i == separatorStart)
                {
                    separatorStart = unmaskedText.Length;
                    unmaskedText += CultureInfo.CurrentCulture.NumberFormat.PercentDecimalSeparator.ToString();
                }

                if (i < maskedText.Length)
                {
                    if (char.IsDigit(maskedText[i]) || maskedText[i] == '-')
                        unmaskedText += maskedText[i];
                }
            }
            selectionLength = selectionEnd - selectionStart;

            if (separatorStart < 0)
            {
                separatorStart = unmaskedText.Length;
                separatorEnd = unmaskedText.Length;
            }

            if (unmaskedText.Length == selectionLength)
            {
                if (percentTextBox.UseNullOption)
                {
                    if (percentTextBox.MinValue.ToString() == "0" || percentTextBox.MinValue < 0)
                    {
                        percentTextBox.SetValue(true, null);
                    }
                    else
                    {
                        percentTextBox.SetValue(true, percentTextBox.MinValue);
                        percentTextBox.SelectionStart = 0;
                    }
                }
                else
                {
                    if ((percentTextBox.MinValue.ToString() == "0" || percentTextBox.MinValue < 0) && percentTextBox.MinValue != percentTextBox.MaxValue)
                    {
                        percentTextBox.SetValue(true, 0.0);
                        percentTextBox.SelectionStart = 0;
                    }
                    else
                    {
                        percentTextBox.SetValue(true, percentTextBox.MinValue);
                        percentTextBox.SelectionStart = 0;
                    }
                }
                return true;
            }

            if (selectionStart <= separatorStart && selectionEnd >= separatorEnd)
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
            }
            else if (selectionStart <= separatorStart && selectionEnd < separatorEnd)
            {
                if (selectionLength == 0)
                {
                    if (selectionStart != separatorStart)
                    {
                        selectionLength = 1;
                        unmaskedText = unmaskedText.Remove(selectionStart, selectionLength);
                        if (numberFormat != null)
                        {
                            if (unmaskedText[0].ToString() == numberFormat.NumberDecimalSeparator)
                            {
                                caretPosition = 1;
                            }
                            else
                            {
                                caretPosition = selectionStart;
                            }
                        }
                    }
                    else
                    {
                        percentTextBox.SelectionStart = percentTextBox.SelectionStart + 1;
                        return true;
                    }
                }
                else
                {
                    unmaskedText = unmaskedText.Remove(selectionStart, selectionLength);
                    caretPosition = selectionStart;
                }
            }
            else
            {
                if (selectionStart == selectionEnd)
                {
                    if (selectionStart != unmaskedText.Length)
                    {
                        unmaskedText = unmaskedText.Remove(selectionStart, 1);
                        unmaskedText = unmaskedText.Insert(selectionStart, "0");
                    }
                    else
                        return true;
                }
                else
                {
                    for (int decpos = selectionStart; decpos < selectionEnd; decpos++)
                    {
                        unmaskedText = unmaskedText.Remove(decpos, 1);
                        unmaskedText = unmaskedText.Insert(decpos, "0");
                    }
                    caretPosition = selectionStart + selectionLength;
                }
            }

            double preValue;
            if (double.TryParse(unmaskedText, out preValue))
            {
                if (percentTextBox.UseNullOption && preValue == 0)
                {
                    percentTextBox.SetValue(true, null);
                    return true;
                }
                if (percentTextBox.IsNegative)
                {
                    preValue = preValue * 1;
                }

                if (percentTextBox.PercentEditMode == PercentEditMode.PercentMode)
                    preValue = preValue / 100;

                if ((preValue > percentTextBox.MaxValue) && (percentTextBox.MaxValidation == MaxValidation.OnKeyPress) && !percentTextBox.ValidationOnLostFocus)
                {
                    if (percentTextBox.MaxValueOnExceedMaxDigit)
                        preValue = percentTextBox.MaxValue;
                    else return true;
                }

                if ((preValue < percentTextBox.MinValue) && (percentTextBox.MinValidation == MinValidation.OnKeyPress) && !percentTextBox.ValidationOnLostFocus)
                {
                    if (percentTextBox.MinValueOnExceedMinDigit)
                        preValue = percentTextBox.MinValue;
                    else return true;
                }

                if (percentTextBox.PercentEditMode == PercentEditMode.DoubleMode)
                    percentTextBox.MaskedText = (preValue / 100).ToString("P", numberFormat);
                else
                    percentTextBox.MaskedText = preValue.ToString("P", numberFormat);

                maskedText = percentTextBox.MaskedText;
                percentTextBox.SetValue(false, preValue);

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
                percentTextBox.SelectionStart = j;
                selectionStart = j;
                percentTextBox.SelectionLength = 0;
            }
            return true;
        }

        public bool HandleDownKey(PercentTextBox percentTextBox)
        {
            if (percentTextBox.IsReadOnly || !percentTextBox.IsScrollingOnCircle)
                return true;

            if (percentTextBox.mValue != null)
            {
                if (((percentTextBox.mValue - percentTextBox.ScrollInterval) < percentTextBox.MinValue) && (percentTextBox.MinValidation == MinValidation.OnKeyPress) && !percentTextBox.ValidationOnLostFocus)
                {
                    if (percentTextBox.ScrollInterval < 0)
                    {
                        percentTextBox.SetValue(true, percentTextBox.mValue - percentTextBox.ScrollInterval);
                    }
                    //if (integerTextBox.MinValueOnExceedMinDigit)
                    //    integerTextBox.mValue = integerTextBox.MinValue;
                    //else
                    return true;
                }
                else
                {
                    percentTextBox.SetValue(true, percentTextBox.mValue - percentTextBox.ScrollInterval);
                }
            }
#if WPF
            if (!percentTextBox.OnValidating(new CancelEventArgs(false)))
            {
                if (percentTextBox.ValueValidation == StringValidation.OnKeyPress)
                {
                    string validationerror = "";
                    bool validationstatus = true;

                    if (percentTextBox.ValidationValue == percentTextBox.PercentValue.ToString())
                        validationstatus = true;
                    else
                        validationstatus = false;

                    string message = validationstatus ? "String validation succeeded" : "String validation failed";

                    if (!validationstatus)
                    {
                        if (percentTextBox.InvalidValueBehavior == InvalidInputBehavior.DisplayErrorMessage)
                        {
                            MessageBox.Show(message, "Invalid value", MessageBoxButton.OK);
                            percentTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, percentTextBox.ValidationValue));
                            percentTextBox.OnValidated(EventArgs.Empty);
                            return true;
                        }
                        else if (percentTextBox.InvalidValueBehavior == InvalidInputBehavior.ResetValue)
                        {
                            percentTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, percentTextBox.ValidationValue));
                            percentTextBox.OnValidated(EventArgs.Empty);
                            return true;
                        }
                        else if (percentTextBox.InvalidValueBehavior == InvalidInputBehavior.None)
                        {
                            percentTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, percentTextBox.ValidationValue));
                            percentTextBox.OnValidated(EventArgs.Empty);
                        }
                    }
                    else
                    {
                        percentTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, percentTextBox.ValidationValue));
                        percentTextBox.OnValidated(EventArgs.Empty);
                    }
                    return true;
                }
            }
#endif
            return true;
        }

        public bool HandleUpKey(PercentTextBox percentTextBox)
        {
            if (percentTextBox.IsReadOnly || !percentTextBox.IsScrollingOnCircle)
                return true;

            if (percentTextBox.mValue != null)
            {
                if (((percentTextBox.mValue + 1) > percentTextBox.MaxValue) && (percentTextBox.MaxValidation == MaxValidation.OnKeyPress) && !percentTextBox.ValidationOnLostFocus)
                {
                    if (percentTextBox.ScrollInterval < 0)
                    {
                        percentTextBox.SetValue(true, percentTextBox.mValue + percentTextBox.ScrollInterval);
                    }
                    //if (integerTextBox.MaxValueOnExceedMaxDigit)
                    //    integerTextBox.mValue = integerTextBox.MaxValue;
                    //else
                    return true;
                }
                else
                {
                    if (percentTextBox.mValue + percentTextBox.ScrollInterval >= percentTextBox.MinValue)
                        percentTextBox.SetValue(true, percentTextBox.mValue + percentTextBox.ScrollInterval);
                }
            }
#if WPF
            if (!percentTextBox.OnValidating(new CancelEventArgs(false)))
            {
                if (percentTextBox.ValueValidation == StringValidation.OnKeyPress)
                {
                    string validationerror = "";
                    bool validationstatus = true;

                    if (percentTextBox.ValidationValue == percentTextBox.PercentValue.ToString())
                        validationstatus = true;
                    else
                        validationstatus = false;

                    string message = validationstatus ? "String validation succeeded" : "String validation failed";

                    if (!validationstatus)
                    {
                        if (percentTextBox.InvalidValueBehavior == InvalidInputBehavior.DisplayErrorMessage)
                        {
                            MessageBox.Show(message, "Invalid value", MessageBoxButton.OK);
                            percentTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, percentTextBox.ValidationValue));
                            percentTextBox.OnValidated(EventArgs.Empty);
                            return true;
                        }
                        else if (percentTextBox.InvalidValueBehavior == InvalidInputBehavior.ResetValue)
                        {
                            percentTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, percentTextBox.ValidationValue));
                            percentTextBox.OnValidated(EventArgs.Empty);
                            return true;
                        }
                        else if (percentTextBox.InvalidValueBehavior == InvalidInputBehavior.None)
                        {
                            percentTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, percentTextBox.ValidationValue));
                            percentTextBox.OnValidated(EventArgs.Empty);
                        }
                    }
                    else
                    {
                        percentTextBox.OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, percentTextBox.ValidationValue));
                        percentTextBox.OnValidated(EventArgs.Empty);
                    }
                    return true;
                }
            }
#endif
            return true;
        }
    }
}
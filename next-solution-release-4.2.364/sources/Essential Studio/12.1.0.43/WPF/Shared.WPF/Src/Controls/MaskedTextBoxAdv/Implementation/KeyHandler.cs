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
using System.Globalization;

namespace Syncfusion.Windows.Tools.Controls
{
    internal class KeyHandler
    {
        public static KeyHandler keyHandler = new KeyHandler();

        /// <summary>
        /// Handles the key down.
        /// </summary>
        /// <param name="MaskedTextBoxAdv">The masked text box.</param>
        /// <param name="eventArgs">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public bool HandleKeyDown(MaskedTextBoxAdv MaskedTextBoxAdv, KeyEventArgs eventArgs)
        {
            switch (eventArgs.Key)
            {
                case Key.None:
                    break;
#if WPF
                case Key.Cancel:
                    break;
#endif
                case Key.Back:
                    return HandleBackSpaceKey(MaskedTextBoxAdv, eventArgs);
                case Key.Tab:
                    break;
#if WPF
                case Key.LineFeed:
                    break;
                case Key.Clear:
                    break;
                case Key.Return:
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
                    return HandleKeyUp(MaskedTextBoxAdv);
                    break;
                case Key.Right:
                    break;
                case Key.Down:
                    return HandleKeyDown(MaskedTextBoxAdv);
                    break;
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
                    return HandleDeleteKey(MaskedTextBoxAdv, eventArgs);
                    break;
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
                case Key.DeadCharProcessed:
                    break;
#endif
#if SILVERLIGHT
                case Key.Unknown:
                    break;
#endif
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return false;
        }

        /// <summary>
        /// Handles the delete key.
        /// </summary>
        /// <param name="MaskedTextBoxAdv">The masked text box.</param>
        /// <param name="eventArgs">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public bool HandleDeleteKey(MaskedTextBoxAdv MaskedTextBoxAdv, KeyEventArgs eventArgs)
        {
            switch (MaskedTextBoxAdv.MaskType)
            {
                case MaskType.Standard:
                    {
                        #region Standard
                        int currentcaretPosition = MaskedTextBoxAdv.SelectionStart;
                        string text = MaskedTextBoxAdv.MaskedText;

                        int len = MaskedTextBoxAdv.SelectionStart + (MaskedTextBoxAdv.SelectionLength == 0
                                                                      ? 1
                                                                      : MaskedTextBoxAdv.SelectionLength);

                        while (currentcaretPosition <= len - 1 && currentcaretPosition < MaskedTextBoxAdv.Text.Length)
                        {
                            if (MaskedTextBoxAdv.CharCollection[currentcaretPosition].IsPromptCharacter != null &&
                                MaskedTextBoxAdv.CharCollection[currentcaretPosition].IsPromptCharacter != true)
                            {
                                MaskedTextBoxAdv.CharCollection[currentcaretPosition].IsPromptCharacter = true;
                                text = text.Remove(currentcaretPosition, 1);
                                text = text.Insert(currentcaretPosition, MaskedTextBoxAdv.PromptChar.ToString());
                            }
                            currentcaretPosition++;
                        }
                        //int selectionstart = MaskedTextBoxAdv.SelectionStart;
                        MaskedTextBoxAdv.MaskedText = text;
                        //MaskedTextBoxAdv.SelectionStart = selectionstart;
                        MaskedTextBoxAdv.SelectionStart = currentcaretPosition;
                        MaskedTextBoxAdv.SelectionLength = 0;
                        return true;
                        #endregion
                    }
                case MaskType.Integer:
                    {
                        #region Integer
                        NumberFormatInfo numberFormat = MaskedTextBoxAdv.GetCulture().NumberFormat;
                        int selectionStart = 0;
                        int selectionEnd = 0;
                        int selectionLength = 0;
                        int CaretPosition = 0;
                        string unmaskedText = "";

                        for (int i = 0; i <= MaskedTextBoxAdv.MaskedText.Length; i++)
                        {
                            if (i == MaskedTextBoxAdv.SelectionStart)
                            {
                                selectionStart = unmaskedText.Length;
                                CaretPosition = unmaskedText.Length;
                            }

                            if (i == (MaskedTextBoxAdv.SelectionStart + MaskedTextBoxAdv.SelectionLength))
                                selectionEnd = unmaskedText.Length;

                            if (i < MaskedTextBoxAdv.MaskedText.Length)
                            {
                                if (char.IsDigit(MaskedTextBoxAdv.MaskedText[i]))
                                    unmaskedText += MaskedTextBoxAdv.MaskedText[i];
                            }
                        }

                        selectionLength = selectionEnd - selectionStart;

                        if (selectionLength != 0)
                            unmaskedText = unmaskedText.Remove(selectionStart, selectionLength);
                        else
                        {
                            if (selectionStart != unmaskedText.Length)
                            {
                                selectionLength = 1;
                                unmaskedText = unmaskedText.Remove(selectionStart, selectionLength);
                            }
                        }
                        //unmaskedText = unmaskedText.Remove(SelectionStart, SelectionLength);
                        if (unmaskedText.Length == 0)
                        {
                            unmaskedText = "0";
                        }
                        Int64 preValue;
                        if (Int64.TryParse(unmaskedText, NumberStyles.Number, numberFormat, out preValue))
                        {
                            if ((preValue > MaskedTextBoxAdv.IntegerMaxValue) || (preValue < MaskedTextBoxAdv.IntegerMinValue))
                            {
                                return true;
                            }
                            numberFormat.NumberDecimalDigits = 0;
                            MaskedTextBoxAdv.MaskedText = preValue.ToString("N", numberFormat);
                        }

                        int j = 0;
                        for (int i = 0; i < selectionStart; i++)
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
                        MaskedTextBoxAdv.SelectionStart = j;
                        selectionStart = j;
                        MaskedTextBoxAdv.SelectionLength = 0;
                        if (MaskedTextBoxAdv.IsNegative)
                        {
                            Int64 tempVal = (Int64)MaskEditModel.maskEditorModelHelp.CreateValueFromText(MaskedTextBoxAdv);
                            tempVal = tempVal * -1;
                            MaskedTextBoxAdv.MaskedText = tempVal.ToString("N", numberFormat);
                            MaskedTextBoxAdv.SelectionStart = (selectionStart + 1) > MaskedTextBoxAdv.Text.Length ? selectionStart : (selectionStart + 1);
                            return true;
                        }
                        return true;
                        #endregion
                    }
                case MaskType.Double:
                case MaskType.Currency:
                case MaskType.Percentage:
                    {
                        #region Number Masks
                        NumberFormatInfo numberFormat = MaskedTextBoxAdv.GetCulture().NumberFormat;

                        string maskedText = MaskedTextBoxAdv.MaskedText;
                        int selectionStart = 0;//MaskedTextBoxAdv.SelectionStart;
                        int selectionEnd = 0;
                        int selectionLength = 0;
                        //MaskedTextBoxAdv.SelectionLength;
                        int separatorStart = MaskedTextBoxAdv.MaskedText.IndexOf(numberFormat.NumberDecimalSeparator);
                        int separatorEnd = separatorStart + numberFormat.NumberDecimalSeparator.Length;

                        int caretPosition = MaskedTextBoxAdv.SelectionStart;
                        string unmaskedText = "";

                        int i;
                        for (i = 0; i <= MaskedTextBoxAdv.MaskedText.Length; i++)
                        {
                            if (i == MaskedTextBoxAdv.SelectionStart)
                            {
                                selectionStart = unmaskedText.Length;
                                caretPosition = selectionStart;
                            }
                            if (i == (MaskedTextBoxAdv.SelectionStart + MaskedTextBoxAdv.SelectionLength))
                                selectionEnd = unmaskedText.Length;//i;//SelectionLength = unmaskedText.Length - SelectionStart;

                            if (i == separatorEnd)
                                separatorEnd = unmaskedText.Length;

                            if (i == separatorStart)
                            {
                                separatorStart = unmaskedText.Length;
                                unmaskedText += ".";
                            }

                            if (i < MaskedTextBoxAdv.MaskedText.Length)
                            {
                                if (char.IsDigit(MaskedTextBoxAdv.MaskedText[i]))
                                    unmaskedText += MaskedTextBoxAdv.MaskedText[i];
                            }
                        }
                        selectionLength = selectionEnd - selectionStart;

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
                                    caretPosition = selectionStart;
                                }
                                else
                                {
                                    MaskedTextBoxAdv.SelectionStart = MaskedTextBoxAdv.SelectionStart + 1;
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
                                if (i == caretPosition)
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
                            selectionStart = j;
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

                            MaskedTextBoxAdv.SelectionStart = (selectionStart + 1) > MaskedTextBoxAdv.Text.Length ? selectionStart : (selectionStart + 1);
                        }
                        return true;
                        #endregion
                    }
                case MaskType.DateTime:
                    return true;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Handles the back space key.
        /// </summary>
        /// <param name="MaskedTextBoxAdv">The masked text box.</param>
        /// <param name="eventArgs">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public bool HandleBackSpaceKey(MaskedTextBoxAdv MaskedTextBoxAdv, KeyEventArgs eventArgs)
        {
            if (MaskedTextBoxAdv.MaskType == MaskType.Standard)
            {
                #region Standard
                int currentcaretPosition = MaskedTextBoxAdv.SelectionStart + (MaskedTextBoxAdv.SelectionLength > 0 ? 1 : 0);
                string text = MaskedTextBoxAdv.MaskedText;

                int len = MaskedTextBoxAdv.SelectionStart + MaskedTextBoxAdv.SelectionLength;

                while (currentcaretPosition <= len && currentcaretPosition > 0)
                {
                    if (MaskedTextBoxAdv.CharCollection[currentcaretPosition - 1].IsPromptCharacter != null &&
                        MaskedTextBoxAdv.CharCollection[currentcaretPosition - 1].IsPromptCharacter != true)
                    {
                        MaskedTextBoxAdv.CharCollection[currentcaretPosition - 1].IsPromptCharacter = true;
                        text = text.Remove(currentcaretPosition - 1, 1);
                        text = text.Insert(currentcaretPosition - 1, MaskedTextBoxAdv.PromptChar.ToString());
                    }
                    currentcaretPosition++;
                }

                int selectionstart = MaskedTextBoxAdv.SelectionStart;
                MaskedTextBoxAdv.MaskedText = text;
                MaskedTextBoxAdv.SelectionStart = selectionstart == 0 ? 0 : selectionstart - 1;
                MaskedTextBoxAdv.SelectionLength = 0;
                return true;
                #endregion
            }
            else if (MaskedTextBoxAdv.MaskType == MaskType.Integer)
            {
                #region Integer
                NumberFormatInfo numberFormat = MaskedTextBoxAdv.GetCulture().NumberFormat;
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
                if (SelectionLength != 0)
                    unmaskedText = unmaskedText.Remove(SelectionStart, SelectionLength);
                else
                {
                    if (SelectionStart != 0)
                    {
                        SelectionLength = 1;
                        unmaskedText = unmaskedText.Remove(SelectionStart - 1, SelectionLength);
                    }
                }
                SelectionStart = SelectionStart - SelectionLength;
                if (unmaskedText.Length == 0)
                    unmaskedText = "0";

                Int64 preValue;
                if (Int64.TryParse(unmaskedText, NumberStyles.Number, numberFormat, out preValue))
                {
                    if ((preValue > MaskedTextBoxAdv.IntegerMaxValue) || (preValue < MaskedTextBoxAdv.IntegerMinValue))
                    {
                        return true;
                    }
                    numberFormat.NumberDecimalDigits = 0;
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
                MaskedTextBoxAdv.SelectionStart = j;
                SelectionStart = j;
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
            else if (MaskedTextBoxAdv.MaskType == MaskType.Double || MaskedTextBoxAdv.MaskType == MaskType.Currency || MaskedTextBoxAdv.MaskType == MaskType.Percentage)
            {
                #region Number Masks
                NumberFormatInfo numberFormat = MaskedTextBoxAdv.GetCulture().NumberFormat;

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
                }
                else if (SelectionStart <= SeparatorStart && SelectionEnd < SeparatorEnd)
                {
                    if (SelectionLength == 0)
                    {
                        if (SelectionStart != 0)
                        {
                            SelectionLength = 1;
                            unmaskedText = unmaskedText.Remove(SelectionStart - 1, SelectionLength);
                            CaretPosition = SelectionStart - 1;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        unmaskedText = unmaskedText.Remove(SelectionStart, SelectionLength);
                        CaretPosition = SelectionStart;
                    }
                }
                else
                {
                    if (SelectionStart == SelectionEnd)
                    {
                        if (SelectionStart != SeparatorEnd)
                        {
                            unmaskedText = unmaskedText.Remove(SelectionStart - 1, 1);
                            unmaskedText = unmaskedText.Insert(SelectionStart - 1, "0");
                            CaretPosition = SelectionStart - 1;
                        }
                        else
                        {
                            MaskedTextBoxAdv.SelectionStart = MaskedTextBoxAdv.SelectionStart - 1;
                            return true;
                        }
                    }
                    else
                    {
                        for (int decpos = SelectionStart; decpos < SelectionEnd; decpos++)
                        {
                            unmaskedText = unmaskedText.Remove(decpos, 1);
                            unmaskedText = unmaskedText.Insert(decpos, "0");
                        }
                        CaretPosition = SelectionStart;// +SelectionLength;
                    }
                }

                double preValue;
                bool separatorflag = false;
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
                            break;

                        if (j == MaskedTextBoxAdv.MaskedText.Length)
                            break;

                        if (char.IsDigit(MaskedTextBoxAdv.MaskedText[j]))
                            j++;

                        else
                        {
                            for (int k = j; k < MaskedTextBoxAdv.MaskedText.Length; k++)
                            {
                                if (j == MaskedTextBoxAdv.MaskedText.IndexOf(numberFormat.NumberDecimalSeparator))
                                    separatorflag = true;

                                if (char.IsDigit(MaskedTextBoxAdv.MaskedText[k]))
                                    break;
                                j++;
                            }
                            if (separatorflag == false)
                                i--;
                            separatorflag = false;
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
                return true;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Handles the key up.
        /// </summary>
        /// <param name="MaskedTextBoxAdv">The masked text box.</param>
        /// <returns></returns>
        public bool HandleKeyUp(MaskedTextBoxAdv MaskedTextBoxAdv)
        {
            switch (MaskedTextBoxAdv.MaskType)
            {
                case MaskType.Standard:
                    return false;
                case MaskType.Double:
                    double d = 0.0;
                    double.TryParse(MaskedTextBoxAdv.mValue.ToString(), out d);
                    if ((d + 1) <= MaskedTextBoxAdv.MaxValue)
                        MaskedTextBoxAdv.Value = (d + 1);
                    return true;
                case MaskType.Integer:
                    Int64 i;
                    Int64.TryParse(MaskedTextBoxAdv.mValue.ToString(), out i);
                    if ((i + 1) <= MaskedTextBoxAdv.IntegerMaxValue)
                        MaskedTextBoxAdv.Value = (i + 1);
                    return true;
                case MaskType.Currency:
                    double currency = 0.0;
                    double.TryParse(MaskedTextBoxAdv.mValue.ToString(), out currency);
                    if ((currency + 1) <= MaskedTextBoxAdv.MaxValue)
                        MaskedTextBoxAdv.Value = (currency + 1);
                    return true;
                case MaskType.Percentage:
                    double percent = 0.0;
                    double.TryParse(MaskedTextBoxAdv.mValue.ToString(), out percent);
                    //if ((percent + 1) <= MaskedTextBoxAdv.MaxValue)
                    //{
                    if (percent < 1 && percent > 0)
                    {
                        percent = percent * 100;
                        percent = percent + 1;
                    }
                    else if (percent < 0 && percent > -1)
                    {
                        percent = percent * 100;
                        percent = percent + 1;
                    }
                    else
                        percent = percent + 1;
                    if (percent <= MaskedTextBoxAdv.MaxValue)
                        MaskedTextBoxAdv.Value = percent;
                    //}
                    //if ((percent + 1) <= MaskedTextBoxAdv.MaxValue)
                    //{
                    //    //percent = (percent * 100) + 1;
                    //    //percent = percent / 100;
                    //    MaskedTextBoxAdv.Value = (percent + 1);
                    //}
                    return true;
                case MaskType.DateTime:
                    DateTimeFormatInfo formatinfo = MaskedTextBoxAdv.GetCulture().DateTimeFormat.Clone() as DateTimeFormatInfo;
                    if (MaskedTextBoxAdv.mSelectedCollection < 0 || MaskedTextBoxAdv.mSelectedCollection > MaskedTextBoxAdv.DateTimeProperties.Count)
                    {
                        return true;
                    }
                    var charProperty = MaskedTextBoxAdv.DateTimeProperties[MaskedTextBoxAdv.mSelectedCollection];
                    if (charProperty.IsReadOnly == true || charProperty.IsReadOnly == false)
                    {
                        #region YEAR
                        if (charProperty.IsReadOnly == false && charProperty.Type == DateTimeType.year)
                        {
                            DateTime date;
                            date = (DateTime)MaskedTextBoxAdv.mValue;
                            date = date.AddYears(1);

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

                            date = date.AddMinutes(1);
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
                            date = date.AddSeconds(1);

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
                            date = date.AddHours(1);

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
                            date = date.AddMilliseconds(1);

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
                            date = (DateTime)MaskedTextBoxAdv.mValue;
                            //DateTime.TryParse(MaskedTextBoxAdv.mValue.ToString(), out date);
                            date = date.AddMonths(1);

                            MaskedTextBoxAdv.mValueChanged = false;
                            MaskedTextBoxAdv.Value = date;
                            MaskedTextBoxAdv.mValueChanged = true;
                            return true;
                        }
                        #endregion

                        #region Day
                        else if (charProperty.Type == DateTimeType.Day)
                        {
                            DateTime date;
                            date = (DateTime)MaskedTextBoxAdv.mValue;
                            //DateTime.TryParse(MaskedTextBoxAdv.mValue.ToString(), out date);
                            date = date = date.AddDays(1);

                            MaskedTextBoxAdv.mValueChanged = false;
                            MaskedTextBoxAdv.Value = date;
                            MaskedTextBoxAdv.mValueChanged = true;
                            return true;
                        }
                        #endregion

                        else if (charProperty.Type == DateTimeType.designator)
                        {
                            DateTime date;
                            date = (DateTime)MaskedTextBoxAdv.mValue;
                            date = date.AddHours(12);

                            MaskedTextBoxAdv.mValueChanged = false;
                            MaskedTextBoxAdv.Value = date;
                            MaskedTextBoxAdv.mValueChanged = true;
                            return true;
                        }
                    }


                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return true;
        }

        /// <summary>
        /// Handles the key down.
        /// </summary>
        /// <param name="MaskedTextBoxAdv">The masked text box.</param>
        /// <returns></returns>
        public bool HandleKeyDown(MaskedTextBoxAdv MaskedTextBoxAdv)
        {
            switch (MaskedTextBoxAdv.MaskType)
            {
                case MaskType.Standard:
                    return false;
                case MaskType.Double:
                    double d = 0.0;
                    double.TryParse(MaskedTextBoxAdv.mValue.ToString(), out d);
                    if ((d - 1) >= MaskedTextBoxAdv.MinValue)
                        MaskedTextBoxAdv.Value = (d - 1);
                    return true;
                case MaskType.Integer:
                    Int64 i;
                    Int64.TryParse(MaskedTextBoxAdv.mValue.ToString(), out i);
                    if ((i - 1) >= MaskedTextBoxAdv.IntegerMinValue)
                        MaskedTextBoxAdv.Value = (i - 1);
                    break;
                case MaskType.Currency:
                    double currency = 0.0;
                    double.TryParse(MaskedTextBoxAdv.mValue.ToString(), out currency);
                    if ((currency - 1) >= MaskedTextBoxAdv.MinValue)
                        MaskedTextBoxAdv.Value = (currency - 1);
                    break;
                case MaskType.Percentage:
                    double percent = 0.0;
                    double.TryParse(MaskedTextBoxAdv.mValue.ToString(), out percent);
                    
                    
                        if (percent < 1 && percent > 0)
                        {
                            percent = percent * 100;
                            percent = percent - 1;
                        }
                        else if (percent < 0 && percent > -1)
                        {
                            percent = percent * 100;
                            percent = percent - 1;
                        }
                        else
                            percent = percent - 1;
                        if (percent >= MaskedTextBoxAdv.MinValue)
                            MaskedTextBoxAdv.Value = percent;

                        break;
                case MaskType.DateTime:
                    DateTimeFormatInfo formatinfo = MaskedTextBoxAdv.GetCulture().DateTimeFormat.Clone() as DateTimeFormatInfo;
                    if (MaskedTextBoxAdv.mSelectedCollection < 0 || MaskedTextBoxAdv.mSelectedCollection > MaskedTextBoxAdv.DateTimeProperties.Count)
                    {
                        return true;
                    }
                        var charProperty = MaskedTextBoxAdv.DateTimeProperties[MaskedTextBoxAdv.mSelectedCollection];
                        if (charProperty.IsReadOnly == true || charProperty.IsReadOnly == false)
                        {
                            #region YEAR
                            if (charProperty.IsReadOnly == false && charProperty.Type == DateTimeType.year)
                            {
                                DateTime date;
                                date = (DateTime)MaskedTextBoxAdv.mValue;
                                date = date.AddYears(-1);

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
                                date = date.AddMinutes(-1);

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
                                date = date.AddSeconds(-1);

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
                                date = date.AddHours(-1);

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
                                date = date.AddMilliseconds(-1);

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
                                date = date.AddMonths(-1);

                                MaskedTextBoxAdv.mValueChanged = false;
                                MaskedTextBoxAdv.Value = date;
                                MaskedTextBoxAdv.mValueChanged = true;
                                return true;
                            }
                            #endregion

                            #region Day
                            else if (charProperty.Type == DateTimeType.Day)
                            {
                                DateTime date;
                                DateTime.TryParse(MaskedTextBoxAdv.mValue.ToString(), out date);
                                date = date.AddDays(-1);

                                MaskedTextBoxAdv.mValueChanged = false;
                                MaskedTextBoxAdv.Value = date;
                                MaskedTextBoxAdv.mValueChanged = true;
                                return true;
                            }
                            #endregion

                            else if (charProperty.Type == DateTimeType.designator)
                            {
                                DateTime date;
                                date = (DateTime)MaskedTextBoxAdv.mValue;
                                //DateTime.TryParse(MaskedTextBoxAdv.mValue.ToString(), out date);
                                date = date.AddHours(-12);

                                MaskedTextBoxAdv.mValueChanged = false;
                                MaskedTextBoxAdv.Value = date;
                                MaskedTextBoxAdv.mValueChanged = true;
                                return true;
                            }
                        }
                    return true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return true;
        }

        /// <summary>
        /// Handles the selection.
        /// </summary>
        /// <returns></returns>
        public bool HandleSelection()
        {
            return true;
        }
    }
}

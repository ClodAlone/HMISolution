#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Globalization;
using System.Windows.Input;

#if WPF

using System.Collections;

namespace Syncfusion.Windows.Shared

#endif

#if SILVERLIGHT

using System.Collections.Generic;

namespace Syncfusion.Windows.Tools.Controls

#endif
{
    internal class KeyHandler
    {
        public static KeyHandler keyHandler = new KeyHandler();
#if WPF
        private Hashtable monthTable = new Hashtable();
#endif

#if SILVERLIGHT
        private Dictionary<int, string> monthTable = new Dictionary<int, string>();
#endif

        private KeyHandler()
        {
            monthTable.Add(1, "january");
            monthTable.Add(2, "february");
            monthTable.Add(3, "march");
            monthTable.Add(4, "april");
            monthTable.Add(5, "may");
            monthTable.Add(6, "june");
            monthTable.Add(7, "july");
            monthTable.Add(8, "august");
            monthTable.Add(9, "september");
            monthTable.Add(10, "october");
            monthTable.Add(11, "november");
            monthTable.Add(12, "december");
        }

        public bool HandleKeyDown(DateTimeEdit dateTimeEdit, KeyEventArgs eventArgs)
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
                    if (dateTimeEdit.EnableBackspaceKey)
                    {
                        for (int i = 0; i < dateTimeEdit.DateTimeProperties.Count; i++)
                        {
                            var charProperty = dateTimeEdit.DateTimeProperties[i];
                            if ((charProperty.StartPosition <= dateTimeEdit.SelectionStart) && (dateTimeEdit.SelectionStart <= (charProperty.StartPosition + charProperty.Lenghth - 1)))
                            {
                                if (charProperty.Type == DateTimeType.Hour24 || charProperty.Type == DateTimeType.Hour12)
                                {
                                    DateTime date;
                                    date = (DateTime)dateTimeEdit.mValue;
                                    dateTimeEdit.mTextInputpartended = false;
                                    int hour;
                                    int.TryParse("00", out hour);
                                    hour = hour - date.Hour;
                                    date = date.AddHours(hour);
                                    dateTimeEdit.mValueChanged = false;
                                    dateTimeEdit.DateTime = date;
                                    dateTimeEdit.mValueChanged = true;
                                    return true;
                                }
                                else if (charProperty.Type == DateTimeType.Minutes)
                                {
                                    DateTime date;
                                    date = (DateTime)dateTimeEdit.mValue;
                                    dateTimeEdit.mTextInputpartended = false;
                                    int min;
                                    int.TryParse("00", out min);

                                    min = min - date.Minute;
                                    date = date.AddMinutes(min);
                                    dateTimeEdit.mValueChanged = false;
                                    dateTimeEdit.DateTime = date;
                                    dateTimeEdit.mValueChanged = true;
                                    return true;
                                }
                                else if (charProperty.IsReadOnly == false && charProperty.Type == DateTimeType.year)
                                {
                                    return true;
                                }
                                else if (charProperty.Type == DateTimeType.Month || charProperty.Type == DateTimeType.monthname)
                                {
                                    return true;
                                }
                                else if (charProperty.Type == DateTimeType.Day)
                                {
                                    return true;
                                }
                            }
                        }
                        if (dateTimeEdit.SelectWholeContent && dateTimeEdit.SelectionLength == dateTimeEdit.Text.Length && !dateTimeEdit.IsReadOnly)
                        {
                            dateTimeEdit.Text = dateTimeEdit.NoneDateText;
                            dateTimeEdit.DateTime = null;
                        }
                    }
                    else
                        return true;
                    break;

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
                    dateTimeEdit.IsDropDownOpen = false;
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
                    return HandleLeftKey(dateTimeEdit);

                case Key.Up:
                    return HandleUpKey(dateTimeEdit);

                case Key.Right:
                    return HandleRightKey(dateTimeEdit);

                case Key.Down:
                    return HandleDownKey(dateTimeEdit);

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
                    if (dateTimeEdit.EnableDeleteKey)
                    {
                        for (int i = 0; i < dateTimeEdit.DateTimeProperties.Count; i++)
                        {
                            var charProperty = dateTimeEdit.DateTimeProperties[i];
                            if ((charProperty.StartPosition <= dateTimeEdit.SelectionStart) && (dateTimeEdit.SelectionStart <= (charProperty.StartPosition + charProperty.Lenghth - 1)))
                            {
                                if (charProperty.Type == DateTimeType.Hour24 || charProperty.Type == DateTimeType.Hour12)
                                {
                                    DateTime date;
                                    date = (DateTime)dateTimeEdit.mValue;
                                    dateTimeEdit.mTextInputpartended = false;
                                    int hour;
                                    int.TryParse("00", out hour);
                                    hour = hour - date.Hour;
                                    date = date.AddHours(hour);
                                    dateTimeEdit.mValueChanged = false;
                                    dateTimeEdit.DateTime = date;
                                    dateTimeEdit.mValueChanged = true;
                                    return true;
                                }
                                else if (charProperty.Type == DateTimeType.Minutes)
                                {
                                    DateTime date;
                                    date = (DateTime)dateTimeEdit.mValue;
                                    dateTimeEdit.mTextInputpartended = false;
                                    int min;
                                    int.TryParse("00", out min);

                                    min = min - date.Minute;
                                    date = date.AddMinutes(min);
                                    dateTimeEdit.mValueChanged = false;
                                    dateTimeEdit.DateTime = date;
                                    dateTimeEdit.mValueChanged = true;
                                    return true;
                                }
                                else if (charProperty.IsReadOnly == false && charProperty.Type == DateTimeType.year)
                                {
                                    return true;
                                }
                                else if (charProperty.Type == DateTimeType.Month || charProperty.Type == DateTimeType.monthname)
                                {
                                    return true;
                                }
                                else if (charProperty.Type == DateTimeType.Day)
                                {
                                    return true;
                                }
                            }
                        }
                        if (dateTimeEdit.SelectWholeContent && dateTimeEdit.SelectionLength == dateTimeEdit.Text.Length && !dateTimeEdit.IsReadOnly)
                        {
                            dateTimeEdit.Text = dateTimeEdit.NoneDateText;
                            dateTimeEdit.DateTime = null;
                        }
                    }
                    else
                        return true;
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
                    return HandleAlphaKeys(dateTimeEdit, Key.A);

                case Key.B:
                    return HandleAlphaKeys(dateTimeEdit, Key.B);

                case Key.C:
                    return HandleAlphaKeys(dateTimeEdit, Key.C);

                case Key.D:
                    return HandleAlphaKeys(dateTimeEdit, Key.D);

                case Key.E:
                    return HandleAlphaKeys(dateTimeEdit, Key.E);

                case Key.F:
                    return HandleAlphaKeys(dateTimeEdit, Key.F);

                case Key.G:
                    return HandleAlphaKeys(dateTimeEdit, Key.G);

                case Key.H:
                    return HandleAlphaKeys(dateTimeEdit, Key.H);

                case Key.I:
                    return HandleAlphaKeys(dateTimeEdit, Key.I);

                case Key.J:
                    return HandleAlphaKeys(dateTimeEdit, Key.J);

                case Key.K:
                    return HandleAlphaKeys(dateTimeEdit, Key.K);

                case Key.L:
                    return HandleAlphaKeys(dateTimeEdit, Key.L);

                case Key.M:
                    return HandleAlphaKeys(dateTimeEdit, Key.M);

                case Key.N:
                    return HandleAlphaKeys(dateTimeEdit, Key.N);

                case Key.O:
                    return HandleAlphaKeys(dateTimeEdit, Key.O);

                case Key.P:
                    return HandleAlphaKeys(dateTimeEdit, Key.P);

                case Key.Q:
                    return HandleAlphaKeys(dateTimeEdit, Key.Q);

                case Key.R:
                    return HandleAlphaKeys(dateTimeEdit, Key.R);

                case Key.S:
                    return HandleAlphaKeys(dateTimeEdit, Key.S);

                case Key.T:
                    return HandleAlphaKeys(dateTimeEdit, Key.T);

                case Key.U:
                    return HandleAlphaKeys(dateTimeEdit, Key.U);

                case Key.V:
                    return HandleAlphaKeys(dateTimeEdit, Key.V);

                case Key.W:
                    return HandleAlphaKeys(dateTimeEdit, Key.W);

                case Key.X:
                    return HandleAlphaKeys(dateTimeEdit, Key.X);

                case Key.Y:
                    return HandleAlphaKeys(dateTimeEdit, Key.Y);

                case Key.Z:
                    return HandleAlphaKeys(dateTimeEdit, Key.Z);

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

        public bool HandleAlphaKeys(DateTimeEdit dateTimeEdit, Key k)
        {
            if (!dateTimeEdit.EnableAlphaKeyNavigation)
                return true;
            if (dateTimeEdit.IsReadOnly)
                return true;
            if (dateTimeEdit.IsNull != true)
            {
                DateTimeFormatInfo formatinfo = dateTimeEdit.GetCulture().DateTimeFormat.Clone() as DateTimeFormatInfo;
                var charProperty = dateTimeEdit.DateTimeProperties[dateTimeEdit.mSelectedCollection];

                if (charProperty.Type == DateTimeType.Month || charProperty.Type == DateTimeType.monthname)
                {
                    dateTimeEdit.DateTimeProperties[dateTimeEdit.mSelectedCollection].MonthName += k.ToString();
                    string mname = dateTimeEdit.DateTimeProperties[dateTimeEdit.mSelectedCollection].MonthName;
                    if (mname.Length > 1)
                        if (mname.Substring(mname.Length - 2, 1).Equals(mname.Substring(mname.Length - 1, 1), StringComparison.CurrentCultureIgnoreCase))
                        {
                            mname = mname.Substring(mname.Length - 1, 1);
                            mname = dateTimeEdit.DateTimeProperties[dateTimeEdit.mSelectedCollection].MonthName = mname;
                        }

                    DateTime date;
                    date = (DateTime)dateTimeEdit.mValue;
                    string temp;
                    int monthno = date.Month;
                    mname = mname.ToLower();
                    int monthcount = 1;
                    int tempmonthno = monthno;

                    while (monthcount <= 12)
                    {
                        temp = monthTable[monthcount].ToString();

                        if (tempmonthno == 6 && mname.Equals("j", StringComparison.CurrentCultureIgnoreCase))
                        {
                            monthno = 7;
                            break;
                        }
                        else
                            if (temp.StartsWith(mname))
                            {
                                monthno = monthcount;
                                if (monthno == tempmonthno && mname.Length <= 1)
                                {
                                    switch (tempmonthno)
                                    {
                                        case 1:
                                            monthno = 6;
                                            break;

                                        case 6:
                                            monthno = 7;
                                            break;

                                        case 3:
                                            monthno = 5;
                                            break;

                                        case 5:
                                            monthno = 3;
                                            break;

                                        case 7:
                                            monthno = 1;
                                            break;

                                        case 4:
                                            monthno = 8;
                                            break;

                                        case 8:
                                            monthno = 4;
                                            break;

                                        default:
                                            break;
                                    }
                                }

                                break;
                            }
                        monthcount++;
                    }

                    if (monthcount > 12)
                    {
                        switch (k)
                        {
                            case Key.A:
                                monthno = 4;
                                dateTimeEdit.DateTimeProperties[dateTimeEdit.mSelectedCollection].MonthName = k.ToString();
                                break;

                            case Key.D:
                                monthno = 12;
                                dateTimeEdit.DateTimeProperties[dateTimeEdit.mSelectedCollection].MonthName = k.ToString();
                                break;

                            case Key.F:
                                monthno = 2;
                                dateTimeEdit.DateTimeProperties[dateTimeEdit.mSelectedCollection].MonthName = k.ToString();
                                break;

                            case Key.J:
                                monthno = 1;
                                dateTimeEdit.DateTimeProperties[dateTimeEdit.mSelectedCollection].MonthName = k.ToString();
                                break;

                            case Key.M:
                                monthno = 3;
                                dateTimeEdit.DateTimeProperties[dateTimeEdit.mSelectedCollection].MonthName = k.ToString();
                                break;

                            case Key.N:
                                monthno = 11;
                                dateTimeEdit.DateTimeProperties[dateTimeEdit.mSelectedCollection].MonthName = k.ToString();
                                break;

                            case Key.O:
                                monthno = 10;
                                dateTimeEdit.DateTimeProperties[dateTimeEdit.mSelectedCollection].MonthName = k.ToString();
                                break;

                            case Key.S:
                                monthno = 9;
                                dateTimeEdit.DateTimeProperties[dateTimeEdit.mSelectedCollection].MonthName = k.ToString();
                                break;

                            default:
                                dateTimeEdit.DateTimeProperties[dateTimeEdit.mSelectedCollection].MonthName = "";
                                break;
                        }
                    }

                    DateTime newdate = new DateTime(date.Year, monthno, 1);
                    dateTimeEdit.mValueChanged = false;
                    dateTimeEdit.DateTime = newdate;
                    dateTimeEdit.mValueChanged = true;
                }
            }
            return true;
        }

        public bool HandleLeftKey(DateTimeEdit dateTimeEdit)
        {
            int tempIndex;
            if (!dateTimeEdit.CanEdit)
                dateTimeEdit.checktext = "";
            if (dateTimeEdit.IsEditable == true)
            {
                if (dateTimeEdit.Text.Equals(dateTimeEdit.NoneDateText))
                {
                    return true;
                }
                if (dateTimeEdit.mSelectedCollection == -1)
                {
                    tempIndex = dateTimeEdit.DateTimeProperties.Count - 1;
                }
                else if (dateTimeEdit.mSelectedCollection > dateTimeEdit.DateTimeProperties.Count - 1)
                {
                    tempIndex = dateTimeEdit.DateTimeProperties.Count - 1;
                }
                else
                {
                    tempIndex = dateTimeEdit.mSelectedCollection - 1;
                }
                while (tempIndex - 1 >= 0)
                {
                    if (dateTimeEdit.DateTimeProperties[tempIndex].IsReadOnly == false)
                    {
                        dateTimeEdit.mSelectedCollection = tempIndex;
                        dateTimeEdit.DateTimeProperties[tempIndex].KeyPressCount = 0;
                        dateTimeEdit.Select(dateTimeEdit.DateTimeProperties[tempIndex].StartPosition, dateTimeEdit.DateTimeProperties[tempIndex].Lenghth);
                        return true;
                    }
                    tempIndex--;
                }

                if ((tempIndex < dateTimeEdit.DateTimeProperties.Count) && (tempIndex >= 0))
                {
                    dateTimeEdit.mSelectedCollection = tempIndex;
                    dateTimeEdit.DateTimeProperties[tempIndex].KeyPressCount = 0;
                    dateTimeEdit.Select(dateTimeEdit.DateTimeProperties[tempIndex].StartPosition, dateTimeEdit.DateTimeProperties[tempIndex].Lenghth);
                }
            }
            return true;
        }

        public bool HandleRightKey(DateTimeEdit dateTimeEdit)
        {
            if (!dateTimeEdit.CanEdit)
                dateTimeEdit.checktext = "";
            if (dateTimeEdit.IsEditable == true)
            {
                if (dateTimeEdit.Text.Equals(dateTimeEdit.NoneDateText))
                    return true;
                int tempIndex;
                if (dateTimeEdit.mSelectedCollection == -1)
                {
                    tempIndex = 0;
                }
                else if (dateTimeEdit.mSelectedCollection > dateTimeEdit.DateTimeProperties.Count - 1)
                {
                    tempIndex = 0;
                }
                else
                {
                    tempIndex = dateTimeEdit.mSelectedCollection + 1;
                }
                while (tempIndex < dateTimeEdit.DateTimeProperties.Count)
                {
                    if (dateTimeEdit.DateTimeProperties[tempIndex].IsReadOnly == false)
                    {
                        dateTimeEdit.mSelectedCollection = tempIndex;
                        dateTimeEdit.DateTimeProperties[tempIndex].KeyPressCount = 0;
                        dateTimeEdit.Select(dateTimeEdit.DateTimeProperties[tempIndex].StartPosition, dateTimeEdit.DateTimeProperties[tempIndex].Lenghth);
                        return true;
                    }
                    tempIndex++;
                }

                if ((dateTimeEdit.mSelectedCollection + 1 < dateTimeEdit.DateTimeProperties.Count) && (dateTimeEdit.mSelectedCollection + 1 >= 0))
                {
                    dateTimeEdit.mSelectedCollection++;
                    dateTimeEdit.Select(dateTimeEdit.DateTimeProperties[dateTimeEdit.mSelectedCollection].StartPosition, dateTimeEdit.DateTimeProperties[dateTimeEdit.mSelectedCollection].Lenghth);
                }
            }
            return true;
        }

        private DateTimeProperties GetDateTimeEditProperty(DateTimeEdit dateTimeEdit)
        {
            DateTimeProperties charProperty = null;
            for (int i = 0; i < dateTimeEdit.DateTimeProperties.Count; i++)
            {
                charProperty = dateTimeEdit.DateTimeProperties[i];

                if ((charProperty.StartPosition <= dateTimeEdit.SelectionStart) && (dateTimeEdit.SelectionStart <= (charProperty.StartPosition + charProperty.Lenghth)))
                {
                    if (charProperty.IsReadOnly == false)
                    {
                        dateTimeEdit.selectionChanged = false;

                        dateTimeEdit.Select(charProperty.StartPosition, charProperty.Lenghth);

                        if (i >= 0)
                        {
                            dateTimeEdit.mSelectedCollection = i;
                        }
                    }
                    else
                    {
                        dateTimeEdit.selectionChanged = false;
                        for (int j = 0; j < dateTimeEdit.DateTimeProperties.Count; j++)
                        {
                            dateTimeEdit.DateTimeProperties[j].KeyPressCount = 0;
                        }
                        dateTimeEdit.Select(charProperty.StartPosition, charProperty.Lenghth);
                    }

                    charProperty = dateTimeEdit.DateTimeProperties[dateTimeEdit.mSelectedCollection];
                    return charProperty;
                }
            }
            return null;
        }

        public bool HandleUpKey(DateTimeEdit dateTimeEdit)
        {
            if (dateTimeEdit.IsReadOnly)
                return true;
            if (dateTimeEdit.Text.Equals(dateTimeEdit.NoneDateText))
                return true;
            if (!dateTimeEdit.IsScrollingOnCircle || dateTimeEdit.IsNull)
            {
                return true;
            }
            DateTimeFormatInfo formatinfo = dateTimeEdit.GetCulture().DateTimeFormat.Clone() as DateTimeFormatInfo;
            if (dateTimeEdit.mSelectedCollection < 0 || dateTimeEdit.mSelectedCollection > dateTimeEdit.DateTimeProperties.Count)
            {
                return true;
            }
            if (dateTimeEdit.CanEdit && dateTimeEdit.Text.ToString()!="")
            {
                DateTime date;
                DateTime.TryParse(dateTimeEdit.Text.ToString(), out date);
                dateTimeEdit.DateTime = date;
            }
            var charProperty = GetDateTimeEditProperty(dateTimeEdit);

            if (dateTimeEdit.DefaultDatePart != DateParts.None)
            {
                if (!dateTimeEdit.mtextboxclicked)
                {
                    for (int i = 0; i < dateTimeEdit.DateTimeProperties.Count; i++)
                    {
                        switch (dateTimeEdit.DateTimeProperties[i].Type)
                        {
                            case DateTimeType.Day:
                                if (dateTimeEdit.DefaultDatePart == DateParts.Day)
                                {
                                    charProperty = dateTimeEdit.DateTimeProperties[i];
                                }
                                break;

                            case DateTimeType.Month:
                                if (dateTimeEdit.DefaultDatePart == DateParts.Month)
                                {
                                    charProperty = dateTimeEdit.DateTimeProperties[i];
                                }
                                break;

                            case DateTimeType.year:
                                if (dateTimeEdit.DefaultDatePart == DateParts.Year)
                                {
                                    charProperty = dateTimeEdit.DateTimeProperties[i];
                                }
                                break;
                        }
                    }
                }
                else
                {
                    switch (charProperty.Type)
                    {
                        case DateTimeType.Day:
                            dateTimeEdit.DefaultDatePart = DateParts.Day;
                            break;

                        case DateTimeType.Month:
                            dateTimeEdit.DefaultDatePart = DateParts.Month;
                            break;

                        case DateTimeType.year:
                            dateTimeEdit.DefaultDatePart = DateParts.Year;
                            break;
                    }
                }
            }
            if (charProperty == null)
            {
                if (dateTimeEdit.mSelectedCollection >= 0 && dateTimeEdit.mSelectedCollection <= dateTimeEdit.DateTimeProperties.Count)
                {
                    if (dateTimeEdit.IsEditable == true)
                    {
                        dateTimeEdit.SelectionStart = dateTimeEdit.DateTimeProperties[dateTimeEdit.mSelectedCollection].StartPosition;
                        dateTimeEdit.SelectionLength = dateTimeEdit.DateTimeProperties[dateTimeEdit.mSelectedCollection].Lenghth;
                    }
                }
            }
            else
                if (charProperty.IsReadOnly == true || charProperty.IsReadOnly == false)
                {
                    #region YEAR

                    if (charProperty.IsReadOnly == false && charProperty.Type == DateTimeType.year)
                    {
                        DateTime date;
                        date = (DateTime)dateTimeEdit.mValue;
                        try
                        {
                            date = date.AddYears(1);
                        }
                        catch { }
                        dateTimeEdit.mTextInputpartended = false;
                        dateTimeEdit.mValueChanged = false;
                        dateTimeEdit.DateTime = date;
                        dateTimeEdit.mValueChanged = true;
                        return true;
                    }

                    #endregion YEAR

                    #region Minutes

                    else if (charProperty.Type == DateTimeType.Minutes)
                    {
                        DateTime date;
                        date = (DateTime)dateTimeEdit.mValue;
                        try
                        {
                            date = date.AddMinutes(1);
                        }
                        catch { }
                        dateTimeEdit.mTextInputpartended = false;
                        dateTimeEdit.mValueChanged = false;
                        dateTimeEdit.DateTime = date;
                        dateTimeEdit.mValueChanged = true;
                        return true;
                    }

                    #endregion Minutes

                    #region Seconds

                    else if (charProperty.Type == DateTimeType.Second)
                    {
                        DateTime date;
                        date = (DateTime)dateTimeEdit.mValue;
                        try
                        {
                            date = date.AddSeconds(1);
                        }
                        catch { }
                        dateTimeEdit.mTextInputpartended = false;

                        dateTimeEdit.mValueChanged = false;
                        dateTimeEdit.DateTime = date;
                        dateTimeEdit.mValueChanged = true;
                        return true;
                    }

                    #endregion Seconds

                    #region Hour

                    else if (charProperty.Type == DateTimeType.Hour24 || charProperty.Type == DateTimeType.Hour12)
                    {
                        DateTime date;
                        date = (DateTime)dateTimeEdit.mValue;
                        try
                        {
                            date = date.AddHours(1);
                        }
                        catch { }
                        dateTimeEdit.mTextInputpartended = false;

                        dateTimeEdit.mValueChanged = false;
                        dateTimeEdit.DateTime = date;
                        dateTimeEdit.mValueChanged = true;
                        return true;
                    }

                    #endregion Hour

                    #region Fraction

                    else if (charProperty.Type == DateTimeType.Fraction)
                    {
                        DateTime date;
                        date = (DateTime)dateTimeEdit.mValue;
                        try
                        {
                            date = date.AddMilliseconds(1);
                        }
                        catch { }
                        dateTimeEdit.mTextInputpartended = false;

                        dateTimeEdit.mValueChanged = false;
                        dateTimeEdit.DateTime = date;
                        dateTimeEdit.mValueChanged = true;
                        return true;
                    }

                    #endregion Fraction

                    #region Month

                    else if (charProperty.Type == DateTimeType.Month || charProperty.Type == DateTimeType.monthname)
                    {
                        DateTime date;
                        date = (DateTime)dateTimeEdit.mValue;
                        try
                        {
                            date = date.AddMonths(1);
                        }
                        catch { }
                        dateTimeEdit.mTextInputpartended = false;
                        dateTimeEdit.mValueChanged = false;
                        dateTimeEdit.DateTime = date;
                        dateTimeEdit.mValueChanged = true;
                        return true;
                    }

                    #endregion Month

                    #region Day

                    else if (charProperty.Type == DateTimeType.Day || charProperty.Type == DateTimeType.Dayname)
                    {
                        DateTime date;
                        DateTime.TryParse(dateTimeEdit.mValue.ToString(), out date);                        
                            try
                            {
                                if (dateTimeEdit.mValue != null)                                    
                                date = date = date.AddDays(1);
                            }
                            catch { }
                        dateTimeEdit.mTextInputpartended = false;

                        dateTimeEdit.mValueChanged = false;
                        dateTimeEdit.DateTime = date;
                        dateTimeEdit.mValueChanged = true;
                        return true;
                    }

                    #endregion Day

                    else if (charProperty.Type == DateTimeType.designator)
                    {
                        DateTime date;
                        date = (DateTime)dateTimeEdit.mValue;
                        try
                        {
                            date = date.AddHours(12);
                        }
                        catch { }
                        dateTimeEdit.mTextInputpartended = false;

                        dateTimeEdit.mValueChanged = false;
                        dateTimeEdit.DateTime = date;
                        dateTimeEdit.mValueChanged = true;
                        return true;
                    }
                }
            return true;
        }

        public bool HandleDownKey(DateTimeEdit dateTimeEdit)
        {
            if (dateTimeEdit.IsReadOnly)
                return true;
            if (dateTimeEdit.Text.Equals(dateTimeEdit.NoneDateText))
                return true;

            if (!dateTimeEdit.IsScrollingOnCircle || dateTimeEdit.IsNull)
            {
                return true;
            }
            DateTimeFormatInfo formatinfo = dateTimeEdit.GetCulture().DateTimeFormat.Clone() as DateTimeFormatInfo;
            if (dateTimeEdit.mSelectedCollection < 0 || dateTimeEdit.mSelectedCollection > dateTimeEdit.DateTimeProperties.Count)
            {
                return true;
            }
            if(dateTimeEdit.CanEdit)
            {
                DateTime date;
                DateTime.TryParse(dateTimeEdit.Text.ToString(), out date);
                dateTimeEdit.DateTime = date;
            }

            var charProperty = GetDateTimeEditProperty(dateTimeEdit);

            if (dateTimeEdit.DefaultDatePart != DateParts.None)
            {
                if (!dateTimeEdit.mtextboxclicked)
                {
                    for (int i = 0; i < dateTimeEdit.DateTimeProperties.Count; i++)
                    {
                        switch (dateTimeEdit.DateTimeProperties[i].Type)
                        {
                            case DateTimeType.Day:
                                if (dateTimeEdit.DefaultDatePart == DateParts.Day)
                                {
                                    charProperty = dateTimeEdit.DateTimeProperties[i];
                                }
                                break;

                            case DateTimeType.Month:
                                if (dateTimeEdit.DefaultDatePart == DateParts.Month)
                                {
                                    charProperty = dateTimeEdit.DateTimeProperties[i];
                                }
                                break;

                            case DateTimeType.year:
                                if (dateTimeEdit.DefaultDatePart == DateParts.Year)
                                {
                                    charProperty = dateTimeEdit.DateTimeProperties[i];
                                }
                                break;
                        }
                    }
                }
                else
                {
                    switch (charProperty.Type)
                    {
                        case DateTimeType.Day:
                            dateTimeEdit.DefaultDatePart = DateParts.Day;
                            break;

                        case DateTimeType.Month:
                            dateTimeEdit.DefaultDatePart = DateParts.Month;
                            break;

                        case DateTimeType.year:
                            dateTimeEdit.DefaultDatePart = DateParts.Year;
                            break;
                    }
                }
            }
            if (charProperty == null)
            {
                if (dateTimeEdit.mSelectedCollection >= 0 && dateTimeEdit.mSelectedCollection <= dateTimeEdit.DateTimeProperties.Count)
                {
                    if (dateTimeEdit.IsEditable == true)
                    {
                        dateTimeEdit.SelectionStart = dateTimeEdit.DateTimeProperties[dateTimeEdit.mSelectedCollection].StartPosition;
                        dateTimeEdit.SelectionLength = dateTimeEdit.DateTimeProperties[dateTimeEdit.mSelectedCollection].Lenghth;
                    }
                }
            }
            else
                if (charProperty.IsReadOnly == true || charProperty.IsReadOnly == false)
                {
                    #region YEAR

                    if (charProperty.IsReadOnly == false && charProperty.Type == DateTimeType.year)
                    {
                        DateTime date;
                        date = (DateTime)dateTimeEdit.mValue;
                        try
                        {
                            date = date.AddYears(-1);
                        }
                        catch
                        {
                        }
                        dateTimeEdit.mTextInputpartended = false;

                        dateTimeEdit.mValueChanged = false;
                        dateTimeEdit.DateTime = date;
                        dateTimeEdit.mValueChanged = true;
                        return true;
                    }

                    #endregion YEAR

                    #region Minutes

                    else if (charProperty.Type == DateTimeType.Minutes)
                    {
                        DateTime date;
                        date = (DateTime)dateTimeEdit.mValue;
                        try
                        {
                            date = date.AddMinutes(-1);
                        }
                        catch
                        {
                        }
                        dateTimeEdit.mTextInputpartended = false;
                        dateTimeEdit.mValueChanged = false;
                        dateTimeEdit.DateTime = date;
                        dateTimeEdit.mValueChanged = true;
                        return true;
                    }

                    #endregion Minutes

                    #region Seconds

                    else if (charProperty.Type == DateTimeType.Second)
                    {
                        DateTime date;
                        date = (DateTime)dateTimeEdit.mValue;
                        try
                        {
                            date = date.AddSeconds(-1);
                        }
                        catch { }
                        dateTimeEdit.mTextInputpartended = false;

                        dateTimeEdit.mValueChanged = false;
                        dateTimeEdit.DateTime = date;
                        dateTimeEdit.mValueChanged = true;
                        return true;
                    }

                    #endregion Seconds

                    #region Hour

                    else if (charProperty.Type == DateTimeType.Hour24 || charProperty.Type == DateTimeType.Hour12)
                    {
                        DateTime date;
                        date = (DateTime)dateTimeEdit.mValue;
                        try
                        {
                            date = date.AddHours(-1);
                        }
                        catch
                        {
                        }
                        dateTimeEdit.mTextInputpartended = false;

                        dateTimeEdit.mValueChanged = false;
                        dateTimeEdit.DateTime = date;
                        dateTimeEdit.mValueChanged = true;
                        return true;
                    }

                    #endregion Hour

                    #region Fraction

                    else if (charProperty.Type == DateTimeType.Fraction)
                    {
                        DateTime date;
                        date = (DateTime)dateTimeEdit.mValue;
                        try
                        {
                            date = date.AddMilliseconds(-1);
                        }
                        catch
                        {
                        }
                        dateTimeEdit.mTextInputpartended = false;
                        dateTimeEdit.mValueChanged = false;
                        dateTimeEdit.DateTime = date;
                        dateTimeEdit.mValueChanged = true;
                        return true;
                    }

                    #endregion Fraction

                    #region Month

                    else if (charProperty.Type == DateTimeType.Month || charProperty.Type == DateTimeType.monthname)
                    {
                        DateTime date;
                        DateTime.TryParse(dateTimeEdit.mValue.ToString(), out date);
                        try
                        {
                            date = date.AddMonths(-1);
                        }
                        catch
                        {
                        }
                        dateTimeEdit.mTextInputpartended = false;
                        dateTimeEdit.mValueChanged = false;
                        dateTimeEdit.DateTime = date;
                        dateTimeEdit.mValueChanged = true;
                        return true;
                    }

                    #endregion Month

                    #region Day

                    else if (charProperty.Type == DateTimeType.Day || charProperty.Type == DateTimeType.Dayname)
                    {
                        DateTime date;
                        DateTime.TryParse(dateTimeEdit.mValue.ToString(), out date);
                        try
                        {
                            date = date.AddDays(-1);
                        }
                        catch
                        {
                        }
                        dateTimeEdit.mTextInputpartended = false;

                        dateTimeEdit.mValueChanged = false;
                        dateTimeEdit.DateTime = date;
                        dateTimeEdit.mValueChanged = true;
                        return true;
                    }

                    #endregion Day

                    else if (charProperty.Type == DateTimeType.designator)
                    {
                        DateTime date;
                        date = (DateTime)dateTimeEdit.mValue;
                        try
                        {
                            date = date.AddHours(-12);
                        }
                        catch
                        {
                        }
                        dateTimeEdit.mTextInputpartended = false;
                        dateTimeEdit.mValueChanged = false;
                        dateTimeEdit.DateTime = date;
                        dateTimeEdit.mValueChanged = true;
                        return true;
                    }
                }
            return true;
        }
    }
}
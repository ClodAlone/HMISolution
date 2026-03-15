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
using System.Windows.Input;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Key Code Class.
    /// </summary>
    public class KeyCode
    {

        /// <summary>
        /// Keycodes to char.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="isShift">if set to <c>true</c> [is shift].</param>
        /// <returns></returns>
        public static String KeycodeToChar(Key key,bool isShift)
        {
            switch (key)
            {
                case Key.Add:
                    return "+";                
                case Key.Decimal:
                    return ".";
                case Key.Divide:
                    return "/";
                case Key.Multiply:
                    return "*";
                //case Key.OemPipe:
                //    {
                //        if (!isShift)
                //            return "\\";
                //        else
                //            return "|";
                //    }
                //case Key.OemCloseBrackets:
                //    if (!isShift)
                //        return "]";
                //    else
                //        return "}";
                //case Key.OemMinus:
                //    if (!isShift)
                //        return "-";
                //    else
                //        return "_";
                //case Key.OemOpenBrackets:
                //    if (!isShift)
                //        return "[";
                //    else
                //        return "{";
                //case Key.OemPeriod:
                //    if (!isShift)
                //        return ".";
                //    else
                //        return ">";
               
                //case Key.OemQuestion:
                //    if (!isShift)
                //        return "/";
                //    else
                //        return "?";
                //case Key.OemQuotes:
                //    if (!isShift)
                //        return "'";
                //    else
                //        return "\"";
                //case Key.OemSemicolon:
                //    if (!isShift)
                //        return ";";
                //    else
                //        return ":";
                //case Key.OemComma:
                //    if (!isShift)
                //        return ",";
                //    else
                //        return "<";
                //case Key.OemPlus:
                //    if (!isShift)
                //        return "=";
                //    else
                //        return "+";
                
                //case Key.OemTilde:
                //    if (!isShift)
                //        return "`";
                //    else
                //        return "~";
                //case Key.Separator:
                //    return "-";
                case Key.Subtract:
                    return "-";
                case Key.D0:
                    if (!isShift)
                        return "0";
                    else
                        return ")";
                case Key.D1:
                    if (!isShift)
                        return "1";
                    else
                        return "!";
                case Key.D2:
                    if (!isShift)
                        return "2";
                    else
                        return "@";
                case Key.D3:
                    if (!isShift)
                        return "3";
                    else
                        return "#";
                case Key.D4:
                    if (!isShift)
                        return "4";
                    else
                        return "$";
                case Key.D5:
                    if (!isShift)
                        return "5";
                    else
                        return "%";
                case Key.D6:
                    if (!isShift)
                        return "6";
                    else
                        return "^";
                case Key.D7:
                    if (!isShift)
                        return "7";
                    else
                        return "&";
                case Key.D8:
                    if (!isShift)
                        return "8";
                    else
                        return "*";
                case Key.D9:
                    if (!isShift)
                        return "9";
                    else
                        return "(";
                case Key.Space:
                    return " ";
                    
                case Key.NumPad1:
                    return "1";

                case Key.NumPad2:
                    return "2";

                case Key.NumPad3:
                    return "3";

                case Key.NumPad4:
                    return "4";

                case Key.NumPad5:
                    return "5";

                case Key.NumPad6:
                    return "6";

                case Key.NumPad7:
                    return "7";

                case Key.NumPad8:
                    return "8";

                case Key.NumPad9:
                    return "9";

                case Key.NumPad0:
                    return "0";
                default:
                    if (!isShift)
                        return key.ToString().ToLower();
                    else
                        return key.ToString();
            }
        }

        /// <summary>
        /// Platforms the key code to char.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="isShift">if set to <c>true</c> [is shift].</param>
        /// <returns></returns>
        public static string PlatformKeyCodeToChar(int key, bool isShift)
        {
            switch (key)
            {
                case 220:
                    if (!isShift)
                        return "\\";
                    else
                        return "|";

                case 221:
                    if (!isShift)
                        return "]";
                    else
                        return "}";
                case 189:
                    if (!isShift)
                        return "-";
                    else
                        return "_";
                case 219:
                    if (!isShift)
                        return "[";
                    else
                        return "{";
                case 190:
                    if (!isShift)
                        return ".";
                    else
                        return ">";

                case 191:
                    if (!isShift)
                        return "/";
                    else
                        return "?";
                case 222:
                    if (!isShift)
                        return "'";
                    else
                        return "\"";
                case 186:
                    if (!isShift)
                        return ";";
                    else
                        return ":";
                case 188:
                    if (!isShift)
                        return ",";
                    else
                        return "<";
                case 187:
                    if (!isShift)
                        return "=";
                    else
                        return "+";

                case 192:
                    if (!isShift)
                        return "`";
                    else
                        return "~";                
                

                default:
                    return "";
            }
        }

        /// <summary>
        /// Determines whether [is modifier key] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if [is modifier key] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsModifierKey(Key key)
        {
            switch (key)
            {
                case Key.Shift:
                    return true;

                case Key.Alt:
                    return true;

                case Key.Tab:
                    return true;

                case Key.CapsLock:
                    return true;
                case Key.Ctrl:
                    return true;
                case Key.Up:
                    return true;
                case Key.Down:
                    return true;
                case Key.Insert:
                    return true;
                case Key.PageUp:
                    return true;
                case Key.PageDown:
                    return true;
                case Key.F1:
                    return true;
                case Key.F2:
                    return true;
                case Key.F3:
                    return true;

                case Key.F4:
                    return true;

                case Key.F5:
                    return true;

                case Key.F6:
                    return true;

                case Key.F7:
                    return true;

                case Key.F8:
                    return true;

                case Key.F9:
                    return true;

                case Key.F10:
                    return true;

                case Key.F11:
                    return true;

                case Key.F12:
                    return true;

                default:
                    return false;
            }
        }


    }
}

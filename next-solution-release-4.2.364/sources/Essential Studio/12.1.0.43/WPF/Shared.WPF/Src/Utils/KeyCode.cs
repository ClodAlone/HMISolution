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

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents the KeyCode for keydown
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
                case Key.OemPipe:
                    {
                        if (!isShift)
                            return "\\";
                        else
                            return "|";
                    }
                case Key.OemCloseBrackets:
                    if (!isShift)
                        return "]";
                    else
                        return "}";
                case Key.OemMinus:
                    if (!isShift)
                        return "-";
                    else
                        return "_";
                case Key.OemOpenBrackets:
                    if (!isShift)
                        return "[";
                    else
                        return "{";
                case Key.OemPeriod:
                    if (!isShift)
                        return ".";
                    else
                        return ">";
               
                case Key.OemQuestion:
                    if (!isShift)
                        return "/";
                    else
                        return "?";
                case Key.OemQuotes:
                    if (!isShift)
                        return "'";
                    else
                        return "\"";
                case Key.OemSemicolon:
                    if (!isShift)
                        return ";";
                    else
                        return ":";
                case Key.OemComma:
                    if (!isShift)
                        return ",";
                    else
                        return "<";
                case Key.OemPlus:
                    if (!isShift)
                        return "=";
                    else
                        return "+";
                
                case Key.OemTilde:
                    if (!isShift)
                        return "`";
                    else
                        return "~";
                case Key.Separator:
                    return "-";
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
                    
                default:
                    return key.ToString();
            }
        }
    }
}

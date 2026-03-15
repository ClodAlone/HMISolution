// <copyright file="AutomaticKey.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Automatic key tip calculation class.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class AutomaticKeys
    {
        /// <summary>
        /// Define the key.
        /// </summary>
        internal static Stack<string> m_keys = new Stack<string>();

        /// <summary>
        /// Get next key tip value for QAT items.
        /// </summary>
        /// <returns>Return the key.</returns>
        internal static string Next()
        {
            if (m_keys.Count > 0)
            {
                string previous = m_keys.Peek();
                if (previous.Length == 1)
                {
                    int temp = 0;
                    if (int.TryParse(previous, out temp))
                    {
                        if (temp < 9)
                        {
                            temp++;
                            m_keys.Push(temp.ToString());
                            return m_keys.Peek();
                        }
                        else if (temp == 9)
                        {
                            m_keys.Push("09");
                            return m_keys.Peek();
                        }
                    }
                }
                else
                {
                    char[] chars = previous.ToCharArray();
                    char secondDigit = chars[1];
                    if (Char.IsNumber(secondDigit) && int.Parse(secondDigit.ToString()) <= 9 && int.Parse(secondDigit.ToString()) > 1)
                    {
                        int temp = int.Parse(secondDigit.ToString());
                        m_keys.Push("0" + --temp);
                        return m_keys.Peek();
                    }
                    else
                    {
                        if (Char.IsNumber(secondDigit) && int.Parse(secondDigit.ToString()) == 1)
                        {
                            m_keys.Push("0" + "A");
                            return m_keys.Peek();
                        }
                        else if (secondDigit >= 65 && secondDigit <= 91)
                        {
                            m_keys.Push("0" + (char)++secondDigit);
                            return m_keys.Peek();
                        }
                    }
                }
            }
            else
            {
                m_keys.Push("1");
                return m_keys.Peek();
            }

            return string.Empty;
        }

        /// <summary>
        /// Pop the last item from Stack.
        /// </summary>
        internal static void Pop()
        {
            if(m_keys.Count>0)
                m_keys.Pop();
        }
    }
}

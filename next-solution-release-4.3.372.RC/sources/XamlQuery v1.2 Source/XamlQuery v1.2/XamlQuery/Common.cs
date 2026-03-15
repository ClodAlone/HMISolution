//Common.cs

/*
 * XamlQuery Silverlight Library v1.2
 * http://XamlQuery.codeplex.com/
 * 
 * Author: cincoutprabu (cincoutprabu@gmail.com)
 * Copyright 2010 cincoutprabu. All rights reserved.
 * 
 * Released under Microsoft Public License.
 * http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
 *
 * Date: 2010-Sep-10 06:00 IST
 * Revision: 1
 */

using System;
using System.Collections.Generic;
using System.Windows;

namespace XamlQuery
{
    internal class Common
    {
        #region Fields

        internal static List<string> Log = new List<string>();
        internal static bool TraceLog = false;

        #endregion

        #region Methods

        internal static void ClearLog()
        {
            Log.Clear();
        }

        internal static void AddToLog(string entry)
        {
            if (TraceLog)
            {
                Log.Add(entry);
            }
        }

        internal static void Show(string entry)
        {
            MessageBox.Show(entry);
        }

        internal static void Error(string message)
        {
            MessageBox.Show(message, "XamlQuery", MessageBoxButton.OK);
        }

        #endregion
    }
}

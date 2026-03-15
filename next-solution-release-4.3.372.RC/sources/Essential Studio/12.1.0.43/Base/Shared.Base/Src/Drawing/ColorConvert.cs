#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;

namespace Syncfusion.Drawing
{
    /// <summary>
    /// Conversion methods for a <see cref="Color"/> to and from a string.
    /// </summary>
    public sealed class ColorConvert
    {
		/// <summary>
		///		ColorFromString parses a string previously generated with ColorToString and returns a color.
		/// </summary>
		/// <param name="parseStr">String generated with ColorToString.</param>
		/// <returns>
		///		Color value that was encoded in parseStr.
		///	</returns>
        /// <internalonly/>
		public static Color ColorFromString(string parseStr)
		{
            TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
            object o = tc.ConvertFrom(parseStr);
            if (o != null && o is Color)
                return (Color) o;

            return Color.Empty;
        }

		/// <summary>
		///		ColorToString creates a string from a color. All information such as 
		///		knownColor and name in the color structure will be preserved. 
		/// </summary>
		/// <param name="color"> </param>
		/// <param name="writeName"> </param>
		/// <returns>
		///		A string that can be passed as parameter to ColorFromString.
		///	</returns>
        /// <internalonly/>
		public static string ColorToString(Color color, bool writeName)
		{
            TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
            object o = tc.ConvertTo(color, typeof(string));
            if (o != null && o is string)
                return (string) o;

            return "";
        }
    }
}

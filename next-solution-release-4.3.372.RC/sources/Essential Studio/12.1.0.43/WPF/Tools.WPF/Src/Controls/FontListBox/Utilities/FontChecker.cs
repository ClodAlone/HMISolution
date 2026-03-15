// <copyright file="FontChecker.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Used for checking whether font is symbolic.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public static class FontChecker
    {
        /// <summary>
        /// Checks whether font is symbolic.
        /// </summary>
        /// <param name="fontFamily">Font to be checked.</param>
        /// <returns>True if font is symbolic, otherwise false.</returns>
        public static bool IsFontSymbol(FontFamily fontFamily)
        {
            foreach (Typeface typeface in fontFamily.GetTypefaces())
            {
                GlyphTypeface face;
                if (typeface.TryGetGlyphTypeface(out face))
                {
                    return face.Symbol;
                }
            }

            return false;
        }
    }
}
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

#region file using directives
using System;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents information about page size.
    /// </summary>
    public sealed class PageSize
    {
        #region Class constants
        /// <summary>
        /// A3 format.
        /// </summary>
        public static readonly SizeF A3 = new SizeF(842, 1190);
        /// <summary>
        /// A4 format.
        /// </summary>
        public static readonly SizeF A4 = new SizeF(595, 842);
        /// <summary>
        /// A5 format.
        /// </summary>
        public static readonly SizeF A5 = new SizeF(421, 595);
        /// <summary>
        /// A6 format.
        /// </summary>
        public static readonly SizeF A6 = new SizeF(297, 421);
        /// <summary>
        /// B4 format.
        /// </summary>
        public static readonly SizeF B4 = new SizeF(709, 1002);
        /// <summary>
        /// B5 format.
        /// </summary>
        public static readonly SizeF B5 = new SizeF(501, 709);
        /// <summary>
        /// B5 format.
        /// </summary>
        public static readonly SizeF B6 = new SizeF(501, 354);
        /// <summary>
        /// Letter format.
        /// </summary>
        public static readonly SizeF Letter = new SizeF(612, 792);
        /// <summary>
        /// HalfLetter format.
        /// </summary>
        public static readonly SizeF HalfLetter = new SizeF(396, 612);
        /// <summary>
        /// 11x17 format.
        /// </summary>
        public static readonly SizeF Letter11x17 = new SizeF(792, 1224);
        /// <summary>
        /// EnvelopeDL format.
        /// </summary>
        public static readonly SizeF EnvelopeDL = new SizeF(312, 624);
        /// <summary>
        /// Quarto format;.
        /// </summary>
        public static readonly SizeF Quarto = new SizeF(610, 780);
        /// <summary>
        /// Statement format.
        /// </summary>
        public static readonly SizeF Statement = new SizeF(396, 612);
        /// <summary>
        /// Ledger format.
        /// </summary>
        public static readonly SizeF Ledger = new SizeF(1224, 792);
        /// <summary>
        /// Tabloid format.
        /// </summary>
        public static readonly SizeF Tabloid = new SizeF(792, 1224);
        /// <summary>
        /// Note format.
        /// </summary>
        public static readonly SizeF Note = new SizeF(540, 720);
        /// <summary>
        /// Legal format.
        /// </summary>
        public static readonly SizeF Legal = new SizeF(612, 1008);
        /// <summary>
        /// Flsa format.
        /// </summary>
        public static readonly SizeF Flsa = new SizeF(612, 936);
        /// <summary>
        /// Executive format.
        /// </summary>
        public static readonly SizeF Executive = new SizeF(522, 756);
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Private constructor.
        /// </summary>
        private PageSize()
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}

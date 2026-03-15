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
using System.Diagnostics;
using System.Drawing;
#endregion

namespace Syncfusion.Pdf
{
    /// <property name="flag" value="Finished" />
    ///
    /// <summary>
    /// Represents information about page size.
    /// </summary>
    public sealed class PdfPageSize
    {
        #region Constants
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Letter format.
        /// </summary>
        public static readonly SizeF Letter = new SizeF(612, 792);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Note format.
        /// </summary>
        public static readonly SizeF Note = new SizeF(540, 720);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Legal format.
        /// </summary>
        public static readonly SizeF Legal = new SizeF(612, 1008);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// A0 format.
        /// </summary>
        public static readonly SizeF A0 = new SizeF(2380, 3368);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// A1 format.
        /// </summary>
        public static readonly SizeF A1 = new SizeF(1684, 2380);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// A2 format.
        /// </summary>
        public static readonly SizeF A2 = new SizeF(1190, 1684);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// A3 format.
        /// </summary>
        public static readonly SizeF A3 = new SizeF(842, 1190);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// A4 format.
        /// </summary>
        public static readonly SizeF A4 = new SizeF(595, 842);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// A5 format.
        /// </summary>
        public static readonly SizeF A5 = new SizeF(421, 595);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// A6 format.
        /// </summary>
        public static readonly SizeF A6 = new SizeF(297, 421);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// A7 format.
        /// </summary>
        public static readonly SizeF A7 = new SizeF(210, 297);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// A8 format.
        /// </summary>
        public static readonly SizeF A8 = new SizeF(148, 210);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// A9 format.
        /// </summary>
        public static readonly SizeF A9 = new SizeF(105, 148);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// A10 format.
        /// </summary>
        public static readonly SizeF A10 = new SizeF(74, 105);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// B0 format.
        /// </summary>
        public static readonly SizeF B0 = new SizeF(2836, 4008);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// B1 format.
        /// </summary>
        public static readonly SizeF B1 = new SizeF(2004, 2836);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// B2 format.
        /// </summary>
        public static readonly SizeF B2 = new SizeF(1418, 2004);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// B3 format.
        /// </summary>
        public static readonly SizeF B3 = new SizeF(1002, 1418);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// B4 format.
        /// </summary>
        public static readonly SizeF B4 = new SizeF(709, 1002);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// B5 format.
        /// </summary>
        public static readonly SizeF B5 = new SizeF(501, 709);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// ArchE format.
        /// </summary>
        public static readonly SizeF ArchE = new SizeF(2592, 3456);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// ArchD format.
        /// </summary>
        public static readonly SizeF ArchD = new SizeF(1728, 2592);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// ArchC format.
        /// </summary>
        public static readonly SizeF ArchC = new SizeF(1296, 1728);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// ArchB format.
        /// </summary>
        public static readonly SizeF ArchB = new SizeF(864, 1296);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// ArchA format.
        /// </summary>
        public static readonly SizeF ArchA = new SizeF(648, 864);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// The American Foolscap format.
        /// </summary>
        public static readonly SizeF Flsa = new SizeF(612, 936);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// HalfLetter format.
        /// </summary>
        public static readonly SizeF HalfLetter = new SizeF(396, 612);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// 11x17 format.
        /// </summary>
        public static readonly SizeF Letter11x17 = new SizeF(792, 1224);
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Ledger format.
        /// </summary>
        public static readonly SizeF Ledger = new SizeF(1224, 792);
        #endregion

        #region Constructors
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Private constructor.
        /// </summary>
        private PdfPageSize()
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}

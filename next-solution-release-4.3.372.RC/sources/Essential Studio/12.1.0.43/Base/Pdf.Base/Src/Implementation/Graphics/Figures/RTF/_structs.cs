#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Syncfusion.Pdf.Graphics
{
    // It makes no difference if we use PARAFORMAT or
    // PARAFORMAT2 here, so I have opted for PARAFORMAT2.
    [StructLayout(LayoutKind.Sequential)]
    internal struct PARAFORMAT
    {
        /// <summary>
        /// internal variable to store Size.
        /// </summary>
        public int cbSize;

        /// <summary>
        /// internal variable to store Mask.
        /// </summary>
        public uint dwMask;

        /// <summary>
        /// internal variable to store Numbering.
        /// </summary>
        public short wNumbering;

        /// <summary>
        /// internal variable to store Reserved.
        /// </summary>
        public short wReserved;

        /// <summary>
        /// internal variable to store Start Indent.
        /// </summary>
        public int dxStartIndent;

        /// <summary>
        /// internal variable to store Right Indent.
        /// </summary>
        public int dxRightIndent;

        /// <summary>
        /// internal variable to store Offset.
        /// </summary>
        public int dxOffset;

        /// <summary>
        /// internal variable to store Alignment.
        /// </summary>
        public short wAlignment;

        /// <summary>
        /// internal variable to store Tab Count.
        /// </summary>
        public short cTabCount;

        /// <summary>
        /// internal variable to store rgxTabs.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public int[] rgxTabs;

        // PARAFORMAT2 from here onwards.

        /// <summary>
        /// internal variable to store Space Before.
        /// </summary>
        public int dySpaceBefore;

        /// <summary>
        /// internal variable to store Space After.
        /// </summary>
        public int dySpaceAfter;

        /// <summary>
        /// internal variable to store Line Spacing.
        /// </summary>
        public int dyLineSpacing;

        /// <summary>
        /// internal variable to store Style.
        /// </summary>
        public short sStyle;

        /// <summary>
        /// internal variable to store Line Spacing Rule.
        /// </summary>
        public byte bLineSpacingRule;

        /// <summary>
        /// internal variable to store Out line Level.
        /// </summary>
        public byte bOutlineLevel;

        /// <summary>
        /// internal variable to store Shading Weight.
        /// </summary>
        public short wShadingWeight;

        /// <summary>
        /// internal variable to store Shading Style.
        /// </summary>

        public short wShadingStyle;
        /// <summary>
        /// internal variable to store Numbering Start.
        /// </summary>
        public short wNumberingStart;

        /// <summary>
        /// internal variable to store Numbering Style.
        /// </summary>
        public short wNumberingStyle;

        /// <summary>
        /// internal variable to store Numbering Tab.
        /// </summary>
        public short wNumberingTab;
        /// <summary>
        /// internal variable to store Border Space.
        /// </summary>
        public short wBorderSpace;

        /// <summary>
        /// internal variable to store Border Width.
        /// </summary>
        public short wBorderWidth;

        /// <summary>
        /// internal variable to store Borders.
        /// </summary>
        public short wBorders;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CHARFORMAT
    {
        /// <summary>
        /// internal variable to store size.
        /// </summary>
        public int cbSize;

        /// <summary>
        /// internal variable to store Mask.
        /// </summary>
        public UInt32 dwMask;

        /// <summary>
        /// internal variable to store Effects.
        /// </summary>
        public UInt32 dwEffects;

        /// <summary>
        /// internal variable to store Height.
        /// </summary>
        public Int32 yHeight;

        /// <summary>
        /// internal variable to store Offset.
        /// </summary>
        public Int32 yOffset;

        /// <summary>
        /// internal variable to store Text Color.
        /// </summary>
        public Int32 crTextColor;

        /// <summary>
        /// internal variable to store CharSet.
        /// </summary>
        public byte bCharSet;

        /// <summary>
        /// internal variable to store Pitch And Family.
        /// </summary>
        public byte bPitchAndFamily;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] szFaceName;

        // CHARFORMAT2 from here onwards.

        /// <summary>
        /// internal variable to store Weight.
        /// </summary>
        public short wWeight;

        /// <summary>
        /// internal variable to store Spacing.
        /// </summary>
        public short sSpacing;

        /// <summary>
        /// internal variable to store BackColor.
        /// </summary>
        public Int32 crBackColor;

        /// <summary>
        /// internal variable to store lcid.
        /// </summary>
        public uint lcid;

        /// <summary>
        /// internal variable to store Reserved.
        /// </summary>
        public uint dwReserved;

        /// <summary>
        /// internal variable to store Style.
        /// </summary>
        public short sStyle;

        /// <summary>
        /// internal variable to store Kerning.
        /// </summary>
        public short wKerning;

        /// <summary>
        /// internal variable to store Under line Type.
        /// </summary>
        public byte bUnderlineType;

        /// <summary>
        /// internal variable to store Animation.
        /// </summary>
        public byte bAnimation;

        /// <summary>
        /// internal variable to store RevAuthor.
        /// </summary>
        public byte bRevAuthor;

        /// <summary>
        /// internal variable to store Reserved.
        /// </summary>
        public byte bReserved1;
    }
}

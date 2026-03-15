//-------------------------------------------------------------------------------------------------
// <copyright file="DynamicFilterBitmaps.cs" company="Syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    using System;
    using System.Drawing;
    using Syncfusion.Drawing;

    /// <summary>
    /// A helper class for drawing and caching bitmaps from the DataBound.Resources folder of the Grid assembly
    /// </summary>
    public class FilterBitmaps
    {
        static IconPaint iconPainter;

        /// <summary>
        /// Initializes bitmaps for dynamic filter.
        /// </summary>
        public FilterBitmaps()
            : base()
        {
        }

        internal static IconPaint IconPainter
        {
            get
            {
                if (iconPainter == null)
                {
                    iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Resources.", AssemblyInfo.Assembly);
                }

                return iconPainter;
            }
        }

        internal static Bitmap GetBitmap(string name)
        {
            return IconPainter.GetBitmap(name.ToLower() + ".png");
        }

        /// <summary>
        /// Bitmap for moving columns.
        /// </summary>
        public static Bitmap RedDownBitmap
        {
            get
            {
                return IconPainter.GetBitmap("RedDownArrow.bmp");
            }
        }

        /// <summary>
        /// Bitmap for moving columns.
        /// </summary>
        public static Bitmap RedUpBitmap
        {
            get
            {
                return IconPainter.GetBitmap("RedUpArrow.bmp");
            }
        }
        /// <summary>
        /// Bitmap for moving columns.
        /// </summary>
        public static Bitmap RedLeftBitmap
        {
            get
            {
                return IconPainter.GetBitmap("RedLeftArrow.bmp");
            }
        }

        /// <summary>
        /// Bitmap for moving columns.
        /// </summary>
        public static Bitmap RedRightBitmap
        {
            get
            {
                return IconPainter.GetBitmap("RedRightArrow.bmp");
            }
        }
    }
}

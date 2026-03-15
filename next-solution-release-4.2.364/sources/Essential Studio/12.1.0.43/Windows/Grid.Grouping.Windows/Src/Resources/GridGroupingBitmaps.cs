//-------------------------------------------------------------------------------------------------
// <copyright file="GridGroupingBitmaps.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Grid.Grouping
{
    /// <summary>
    /// GridGroupingIconPaint is a helper class for drawing and caching bitmaps from the DataBound.Resources folder of the Grid assembly
    /// </summary>
    /// <remarks>
    /// The bitmaps are loaded from the manifest and cached. The PaintIcon routine
    /// will substitute black pixels of the original bitmap and draw them with a 
    /// specified forecolor.<para/>
    /// Example:<para/>
    ///    GridGroupingBitmaps.IconPainter.PaintIcon(g, rect, Point.Empty, bitmapName, Color.Black);
    /// </remarks>
    public class GridGroupingBitmaps 
    {
        [ThreadStaticAttribute] static IconPaint iconPainter;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridGroupingBitmaps()
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
    }
}

//-------------------------------------------------------------------------------------------------
// <copyright file="GridDataBoundIconPaint.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// GridDataBoundIconPaint is a helper class for drawing and caching bitmaps from the DataBound.Resources folder of the Grid assembly
    /// </summary>
    /// <remarks>
    /// The bitmaps are loaded from the manifest and cached. The PaintIcon routine
    /// will substitute black pixels of the original bitmap and draw them with a 
    /// specified forecolor.<para/>
    /// Example:<para/>
    ///    GridDataBoundIconPaint.Paint.PaintIcon(g, rect, Point.Empty, bitmapName, Color.Black);
    /// </remarks>
    public class GridDataBoundIconPaint
    {
        [ThreadStaticAttribute]
        static IconPaint gridPainter;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridDataBoundIconPaint()
            : base()
        {
        }

        internal static IconPaint Paint
        {
            get
            {
                if (gridPainter == null)
                {
                    gridPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Extensions.DataBound.Resources.", AssemblyInfo.Assembly);
                }

                return gridPainter;
            }
        }
    }
}

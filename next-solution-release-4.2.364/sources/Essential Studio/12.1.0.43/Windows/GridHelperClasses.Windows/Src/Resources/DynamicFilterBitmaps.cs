//-------------------------------------------------------------------------------------------------
// <copyright file="DynamicFilterBitmaps.cs" company="Syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    using System;
    using System.Drawing;
    using Syncfusion.Drawing;

    /// <summary>
    /// A helper class for drawing and caching bitmaps from the DataBound.Resources folder of the Grid assembly
    /// </summary>
    public class DynamicFilterBitmaps
    {
        static IconPaint iconPainter;

        /// <summary>
        /// Initializes bitmaps for dynamic filter.
        /// </summary>
        public DynamicFilterBitmaps()
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
    }
}

//-------------------------------------------------------------------------------------------------
// <copyright file="GridIconPaint.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

using Syncfusion.Diagnostics;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// GridIconPaint is a helper class for drawing and caching bitmaps from a resource manifest with a given forecolor.
    /// </summary>
    /// <remarks>
    /// The bitmaps are loaded from the manifest and cached. The PaintIcon routine
    /// will substitute black pixels of the original bitmap and draw them with a 
    /// specified forecolor.
    /// </remarks>
    public sealed class GridIconPaint : IconPaint
    {
        [ThreadStaticAttribute]
        static GridIconPaint gridPainter;

        internal static GridIconPaint GridPainter
        {
            get
            {
                if (gridPainter == null)
                {
                    gridPainter = new GridIconPaint(AssemblyInfo.RootNamespace + @".Resources.", AssemblyInfo.Assembly);
                }

                return gridPainter;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="GridIconPaint"/> object with manifestPrefix and a reference to the assembly
        /// to load bitmaps from. You should save this object in a static variable.
        /// </summary>
        /// <param name="manifestPrefix"> The manifest to load from. The bitmaps should be saved in the Resources
        /// tree in the Visual Studio project with the build action set to "Embedded Resource".</param>
        /// <param name="ass">The assembly to load from. The bitmaps should be saved in the Resources
        /// tree in Visual Studio project with the build action set to "Embedded Resource".</param>
        public GridIconPaint(string manifestPrefix, Assembly ass)
            : base(manifestPrefix, ass)
        {
        }
    }
}

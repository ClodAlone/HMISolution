//-------------------------------------------------------------------------------------------------
// <copyright file="GridBorderWeightEditor.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.ComponentModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    ///    Provides a <see cref="UITypeEditor"/> for the <see cref="GridBorderWeight"/> enumeration.
    /// </summary>
    internal class GridBorderWeightEditor : UITypeEditor
    {
        /// <summary>
        /// Indicates whether the specified context supports painting a representation of an object's value within the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <returns>
        /// true if <see cref="M:System.Drawing.Design.UITypeEditor.PaintValue(System.Object,System.Drawing.Graphics,System.Drawing.Rectangle)"/> is implemented; otherwise, false.
        /// </returns>
        /// <override/>
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <override/>
        public override void PaintValue(PaintValueEventArgs e)
        {
            if (e.Value is GridBorderWeight)
            {
                GridBorderWeight borderWeight = (GridBorderWeight)e.Value;
                GridBorder border = new GridBorder(GridBorderStyle.Solid, SystemColors.WindowText, borderWeight);

                Rectangle r = e.Bounds;
                r.Inflate(-2, -(r.Height / 2) + 2);
                GridBorderPaint.DrawRectangle(e.Graphics, border, r, SystemColors.Window, GridBorderSide.Top);
            }
        }
    }
}


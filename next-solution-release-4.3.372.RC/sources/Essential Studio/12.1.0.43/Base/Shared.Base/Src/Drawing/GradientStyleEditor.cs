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

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms;
using System.ComponentModel;

namespace Syncfusion.Drawing
{
    /// <summary>
	///    Provides a <see cref="UITypeEditor"/> for the <see cref="GradientStyle"/> enumeration.
	/// </summary>
    public sealed class GradientStyleEditor : UITypeEditor 
    {
		/// <override/>
		public override bool GetPaintValueSupported(ITypeDescriptorContext context)  
        {
            return true;
        }

		/// <override/>
		public override void PaintValue(PaintValueEventArgs e)  
        {
            if (e.Value is GradientStyle)
            {
                GradientStyle gradientStyle = (GradientStyle) e.Value;
                BrushPaint.FillRectangle(e.Graphics, e.Bounds, gradientStyle, SystemColors.WindowText, SystemColors.Window);
            }
        }

    }
}


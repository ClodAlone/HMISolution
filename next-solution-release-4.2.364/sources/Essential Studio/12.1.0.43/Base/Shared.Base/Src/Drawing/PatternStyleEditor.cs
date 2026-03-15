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
using System.Drawing.Design;//using Syncfusion.Windows.Forms.Shared.Drawing;

using System.Windows.Forms;
using System.ComponentModel;


namespace Syncfusion.Drawing
{
	/// <summary>
	///    Provides a <see cref="UITypeEditor"/> for the <see cref="PatternStyle"/> enumeration.
	/// </summary>
	public sealed class PatternStyleEditor : UITypeEditor 
    {
		/// <override/>
		public override bool GetPaintValueSupported(ITypeDescriptorContext context)  
        {
            return true;
        }

		/// <override/>
		public override void PaintValue(PaintValueEventArgs e)  
        {
            if (e.Value is PatternStyle)
            {
                PatternStyle hatch = (PatternStyle) e.Value;
                if (hatch != PatternStyle.None)
                {
                    Brush br = new HatchBrush((HatchStyle) (hatch-1), SystemColors.WindowText, SystemColors.Window);
                    e.Graphics.FillRectangle(br, e.Bounds);
                    br.Dispose();
                }
            }
        }

    }
}


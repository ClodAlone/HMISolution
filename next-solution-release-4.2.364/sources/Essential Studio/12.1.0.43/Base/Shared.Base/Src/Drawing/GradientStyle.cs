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
using System.ComponentModel;
using System.Drawing.Design;
using Syncfusion.Drawing;

namespace Syncfusion.Drawing
{
	/// <summary>
	/// Specifies the Gradient style used by the <see cref="BrushInfo.GradientStyle"/>.
	/// </summary>
	[
	Editor(typeof(Syncfusion.Drawing.GradientStyleEditor),
		typeof(System.Drawing.Design.UITypeEditor)),
	]
    public enum GradientStyle 
    {
		/// <summary>
		/// None.
		/// </summary>
        None,
		
		/// <summary>
		/// ForwardDiagonal Gradient.
		/// </summary>
        ForwardDiagonal,

		/// <summary>
		/// BackwardDiagonal Gradient.
		/// </summary>
		BackwardDiagonal,
        
		/// <summary>
		/// Horizontal Gradient.
		/// </summary>
		Horizontal,
        
		/// <summary>
		/// Vertical Gradient.
		/// </summary>
		Vertical,
        
		/// <summary>
		/// PathRectangle Gradient.
		/// </summary>
		PathRectangle,
        
		/// <summary>
		/// PathEllipse Gradient.
		/// </summary>
		PathEllipse,
        //PathPie
    }
}

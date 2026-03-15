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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Tools
{

	/// <summary>
	/// Specifies the foreground styles of the ProgressBarAdv.
	/// </summary>
	public enum ProgressBarStyles
	{
		/// <summary>
		/// The foreground of the ProgressBar will be drawn with a constant color.
		/// </summary>
		Constant,
		/// <summary>
		/// The foreground of the ProgressBar will be drawn with a gradient brush.
		/// </summary>
		Gradient,
		/// <summary>
		/// The foreground of the ProgressBar will be drawn with a multiple gradient brush.
		/// </summary>
		MultipleGradient,
		/// <summary>
		/// The foreground of the ProgressBar will be drawn with a vertical tube-like gradient brush.
		/// </summary>
		Tube,
		/// <summary>
		/// The foreground of the ProgressBar will be drawn with an image.
		/// </summary>
		Image,
		/// <summary>
		/// The foreground of the ProgressBar will be drawn by the system.
		/// </summary>
		System,
		/// <summary>
		/// The foreground of the ProgressBar will be drawn with a moving gradient line.
		/// </summary>
		WaitingGradient,
        /// <summary>
        /// The foreground of the ProgressBar will be drawn by Metro.
        /// </summary>
		Metro
	}
	/// <summary>
	/// Specifies the text styles.
	/// </summary>
	public enum ProgressBarTextStyles
	{
		/// <summary>
		/// The text of the ProgressBar will be a percentage value. Ex: 75%
		/// </summary>
		Percentage,
		/// <summary>
		/// The text of the ProgressBar will be the value of the format: ProgressBar / the maximum value. Ex 75/200
		/// </summary>
		Value,
		/// <summary>
		/// The text of the ProgressBar will be asked through the ValueChanged event.
		/// </summary>
		Custom

	}	
	/// <summary>
	/// Specifies the background styles that the ProgressBar can draw.
	/// </summary>
	public enum ProgressBarBackgroundStyles
	{
		/// <summary>
		/// The background is drawn with an image.
		/// </summary>
		Image,
		/// <summary>
		/// The background is drawn with a gradient brush.
		/// </summary>
		Gradient,
		/// <summary>
		/// The background is drawn with a vertical gradient brush.
		/// </summary>
		VerticalGradient,
		/// <summary>
		/// The background is drawn with a vertical tube-like gradient brush.
		/// </summary>
		Tube,
		/// <summary>
		/// The background is drawn with a multiple gradient brush.
		/// </summary>
		MultipleGradient,
		/// <summary>
		/// The background is drawn by the system.
		/// </summary>
		System,
		/// <summary>
		/// The background is drawn with the Backcolor.
		/// </summary>
		None
	}
}

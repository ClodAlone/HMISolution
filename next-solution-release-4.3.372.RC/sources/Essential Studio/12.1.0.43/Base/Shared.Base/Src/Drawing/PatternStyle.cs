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

namespace Syncfusion.Drawing
{

	/// <summary>
	/// Specifies the pattern style used in <see cref="BrushInfo.PatternStyle"/>
	/// </summary>
	[
	Editor(typeof(Syncfusion.Drawing.PatternStyleEditor), 
		typeof(System.Drawing.Design.UITypeEditor)),
	]
    public enum PatternStyle 
    {
		/// <summary>
		/// None.
		/// </summary>
        None,

        /// <summary>
        ///    <para>
        ///       A pattern of horizontal lines.
        ///    </para>
        /// </summary>
        Horizontal,

        /// <summary>
        ///    <para>
        ///       A pattern of vertical lines.
        ///    </para>
        /// </summary>
        Vertical,

        /// <summary>
        ///    <para>
        ///       A pattern of lines on a diagonal from top-left to bottom-right.
        ///    </para>
        /// </summary>
        ForwardDiagonal, 

        /// <summary>
        ///    A pattern of lines on a diagonal from
        ///    top-right to bottom-left.
        /// </summary>
        BackwardDiagonal, 

        /// <summary>
        ///    <para>
        ///       A pattern of criss-cross horizontal and vertical lines.
        ///    </para>
        /// </summary>
        Cross, 

        /// <summary>
        ///    <para>
        ///       A pattern of criss-cross diagonal lines.
        ///    </para>
        /// </summary>
        DiagonalCross,


		/// <summary>
		/// Specifies a 5-percent hatch. The ratio of foreground color to background color is 5:100.
		/// </summary>
		Percent05, 

		/// <summary>
		/// Specifies a 10-percent hatch. The ratio of foreground color to background color is 10:100.
		/// </summary>
		Percent10, 

		/// <summary>
		/// Specifies a 20-percent hatch. The ratio of foreground color to background color is 20:100.
		/// </summary>
		Percent20, 

		/// <summary>
		/// Specifies a 25-percent hatch. The ratio of foreground color to background color is 25:100.
		/// </summary>
		Percent25, 

		/// <summary>
		/// Specifies a 30-percent hatch. The ratio of foreground color to background color is 30:100.
		/// </summary>
		Percent30, 

		
		/// <summary>
		/// Specifies a 40-percent hatch. The ratio of foreground color to background color is 40:100.
		/// </summary>
		Percent40, 
		
		/// <summary>
		/// Specifies a 50-percent hatch. The ratio of foreground color to background color is 50:100.
		/// </summary>
		Percent50, 
		
		/// <summary>
		/// Specifies a 60-percent hatch. The ratio of foreground color to background color is 60:100.
		/// </summary>
		Percent60, 
		
		/// <summary>
		/// Specifies a 70-percent hatch. The ratio of foreground color to background color is 70:100.
		/// </summary>
		Percent70, 
		
		/// <summary>
		/// Specifies a 75-percent hatch. The ratio of foreground color to background color is 75:100.
		/// </summary>
		Percent75, 
		
		/// <summary>
		/// Specifies a 80-percent hatch. The ratio of foreground color to background color is 80:100.
		/// </summary>
		Percent80, 
		
		/// <summary>
		/// Specifies a 90-percent hatch. The ratio of foreground color to background color is 90:100.
		/// </summary>
		Percent90, 

		/// <summary>
		/// Specifies diagonal lines that slant to the right from top points to bottom points and are spaced 50 percent closer together than BackwardDiagonal, but they are not antialiased.
		/// </summary>
		LightDownwardDiagonal,
 
		/// <summary>
		/// Specifies diagonal lines that slant to the left from top points to bottom points and are spaced 50 percent closer together than BackwardDiagonal, but they are not antialiased.
		/// </summary>
		LightUpwardDiagonal, 

		/// <summary>
		/// Specifies diagonal lines that slant to the right from top points to bottom points, are spaced 50 percent closer together than, and are twice the width of ForwardDiagonal. This hatch pattern is not antialiased.
		/// </summary>
		DarkDownwardDiagonal, 

		/// <summary>
		/// Specifies diagonal lines that slant to the left from top points to bottom points, are spaced 50 percent closer together than BackwardDiagonal and are twice its width, but the lines are not antialiased.
		/// </summary>
		DarkUpwardDiagonal, 
		
		/// <summary>
		/// Specifies diagonal lines that slant to the right from top points to bottom points, have the same spacing as hatch style ForwardDiagonal and are triple its width, but are not antialiased.
		/// </summary>
		WideDownwardDiagonal, 

		/// <summary>
		/// Specifies diagonal lines that slant to the left from top points to bottom points, have the same spacing as hatch style BackwardDiagonal and are triple its width, but are not antialiased.
		/// </summary>
		WideUpwardDiagonal, 

		/// <summary>
		/// Specifies light vertical lines. 
		/// </summary>
		LightVertical, 

		/// <summary>
		/// Specifies light horizontal lines. 
		/// </summary>
		LightHorizontal, 

		/// <summary>
		/// Specifies narrow vertical lines .
		/// </summary>
		NarrowVertical, 

		/// <summary>
		/// Specifies narrow horizontal lines 
		/// </summary>
		NarrowHorizontal, 

		/// <summary>
        /// Specifies vertical lines that are spaced 50 percent closer together than Vertical and are twice its width.
		/// </summary>
		DarkVertical, 
		
		/// <summary>
		/// Specifies horizontal lines that are spaced 50 percent closer together than Horizontal and are twice the width of HatchStyleHorizontal.
		/// </summary>
		DarkHorizontal, 
		
		/// <summary>
		/// Specifies dashed diagonal lines, that slant to the right from top points to bottom points.
		/// </summary>
		DashedDownwardDiagonal,
 
		/// <summary>
		/// Specifies dashed diagonal lines, that slant to the left from top points to bottom points.
		/// </summary>
		DashedUpwardDiagonal, 

		/// <summary>
		/// Specifies dashed horizontal lines.
		/// </summary>
		DashedHorizontal, 

		/// <summary>
		/// Specifies dashed vertical lines.
		/// </summary>
		DashedVertical, 

		/// <summary>
		/// Specifies a hatch that has the appearance of confetti.
		/// </summary>
		SmallConfetti, 

		/// <summary>
		/// Specifies a hatch that has the appearance of confetti and is composed of larger pieces than SmallConfetti.
		/// </summary>
		LargeConfetti, 
		
		/// <summary>
		/// Specifies horizontal lines that are composed of zigzags.
		/// </summary>
		ZigZag, 
		
		/// <summary>
		/// Specifies horizontal lines that are composed of tildes.
		/// </summary>
		Wave, 

		/// <summary>
		/// Specifies a hatch that has the appearance of layered bricks that slant to the left from top points to bottom points.
		/// </summary>
		DiagonalBrick, 

		/// <summary>
		/// Specifies a hatch that has the appearance of horizontally layered bricks.
		/// </summary>
		HorizontalBrick, 
		
		/// <summary>
		/// Specifies a hatch that has the appearance of a woven material.
		/// </summary>
		Weave, 

		/// <summary>
		/// Specifies a hatch that has the appearance of a plaid material.
		/// </summary>
		Plaid, 

		/// <summary>
		/// Specifies a hatch that has the appearance of divots.
		/// </summary>
		Divot, 

		/// <summary>
		/// Specifies horizontal and vertical lines, each of which is composed of dots, that cross.
		/// </summary>
		DottedGrid, 

		/// <summary>
		/// Specifies forward diagonal and backward diagonal lines, each of which is composed of dots, that cross.
		/// </summary>
		DottedDiamond, 

		/// <summary>
		/// Specifies a hatch that has the appearance of diagonally-layered shingles that slant to the right from top points to bottom points.
		/// </summary>
		Shingle, 
		
		/// <summary>
		/// Specifies a hatch that has the appearance of a trellis.
		/// </summary>
		Trellis, 
		
		/// <summary>
		/// Specifies a hatch that has the appearance of spheres laid adjacent to one another.
		/// </summary>
		Sphere, 
		
		/// <summary>
		/// Specifies horizontal and vertical lines that cross and are spaced 50 percent closer together than hatch style Cross.
		/// </summary>
		SmallGrid, 

		/// <summary>
		/// Specifies a hatch that has the appearance of a checkerboard.
		/// </summary>
		SmallCheckerBoard, 

		/// <summary>
		/// Specifies a hatch that has the appearance of a checkerboard with squares that are twice the size of SmallCheckerBoard.
		/// </summary>
		LargeCheckerBoard, 

		/// <summary>
		/// Specifies forward diagonal and backward diagonal lines that cross but are not antialiased.
		/// </summary>
		OutlinedDiamond, 
		
		/// <summary>
		/// Specifies a hatch that has the appearance of a checkerboard placed diagonally.
		/// </summary>
		SolidDiamond, 
	}


}

#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
//  Author: Jeff Boenig
//
#endregion

#region File using derectives

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Windows.Forms.Diagram;

#endregion

namespace Syncfusion.Windows.Forms.Diagram
{
	/// <summary>
	/// Markup in Points
	/// </summary>
	public class PointsMarkupRenderer
		: RulerMarkupRenderer
	{

		#region Class properties
		/// <summary>
		/// Gets minimum particle of the ruler.
		/// </summary>
		public override float Precision
		{
			get
			{
				return 5;
			}
		}

		#endregion

		#region Class initialize/finalize methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ruler"></param>
		public PointsMarkupRenderer( Ruler ruler )
			: base( ruler )
		{
		}
		#endregion
	}
}

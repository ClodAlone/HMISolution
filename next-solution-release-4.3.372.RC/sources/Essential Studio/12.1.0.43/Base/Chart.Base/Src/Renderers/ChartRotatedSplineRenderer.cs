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

using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Drawing;
using System.Collections;
using System;

namespace Syncfusion.Windows.Forms.Chart.Renderers
{
  /// <summary>
  /// 
  /// </summary>
	internal class RotatedSplineRenderer : SplineRenderer
	{
		#region Properties
		/// <summary>
		/// Gets description of regions.
		/// </summary>
		/// <value></value>
		protected override string RegionDescription
		{
		get
		{
				return "RotatedSpline Chart	Region";
			}
		}
		#endregion

		#region Consturctor
		/// <summary>
		/// Initializes a new instance of the <see cref="RotatedSplineRenderer"/> class.
		/// </summary>
		/// <param name="series"></param>
		public RotatedSplineRenderer(ChartSeries series)
			: base(series)
		{
		}
		#endregion
	}
}
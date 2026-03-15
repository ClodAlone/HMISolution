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

namespace Syncfusion.Windows.Forms.Diagram
{
	/// <summary>
	/// Summary description for FeetMarkupRenderer.
	/// </summary>
	public class FeetMarkupRenderer
		: RulerMarkupRenderer
	{
		#region Class properies
		/// <summary>
		/// Gets the precision value for the scale
		/// </summary>
		public override float Precision
		{
			get
			{
				return 1f / 12f / 83;
			}
		}
		/// <summary>
		/// Gets specified precision for round mark.
		/// </summary>
		public override int RoundPrecision
		{
			get
			{
				return 3;
			}
		}

		#endregion

		#region Class initialize methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ruler"></param>
		public FeetMarkupRenderer( Ruler ruler )
			: base( ruler )
		{
		}
		#endregion
	}
}

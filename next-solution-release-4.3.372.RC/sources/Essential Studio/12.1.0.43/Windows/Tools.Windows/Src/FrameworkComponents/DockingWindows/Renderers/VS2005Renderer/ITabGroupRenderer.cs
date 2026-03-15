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

#region Class using directives
using System.Collections;
using System.Drawing;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	public interface ITabGroupRenderer
	{
		/// <summary>
		/// Gets/sets group tab data.
		/// </summary>
		ITabData TabData { get; set; }
		/// <summary>
		/// Gets group item bounds form number of items.
		/// </summary>
		/// <param name="i">number of items.</param>
		/// <returns>Group item bounds.</returns>
		RectangleF GetGroupItemBounds(int i);
		/// <summary>
		/// Gets overlap size for current size.
		/// </summary>
		/// <param name="tabSize">Starting size.</param>
		/// <returns>Overlap size.</returns>
		SizeF GetOverlapSize(SizeF tabSize);
		/// <summary>
		/// Gets/sets bounds.
		/// </summary>
		RectangleF Bounds { get; set; }
		/// <summary>
		/// Gets preferred size for item.
		/// </summary>
		/// <param name="g">Graphics in which to measure strings.</param>
		/// <returns>Preferred size.</returns>
		SizeF GetPreferredSize(Graphics g);
	}
}
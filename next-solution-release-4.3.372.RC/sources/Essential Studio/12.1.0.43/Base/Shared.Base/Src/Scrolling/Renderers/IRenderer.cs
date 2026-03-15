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

#region file using directives
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.Renderers
{
	/// <summary></summary>
	public interface IRenderer
	{
		/// <summary></summary>
		/// <param name="g"></param>
		/// <param name="bounds"></param>
		/// <param name="state"></param>
		void DrawBackground( Graphics g, Rectangle bounds, ButtonState state );

		/// <summary></summary>
		/// <param name="g"></param>
		/// <param name="bounds"></param>
		/// <param name="type"></param>
		/// <param name="state"></param>
		void DrawArrowButton( Graphics g, Rectangle bounds, ScrollButton type, ButtonState state );

		/// <summary></summary>
		/// <param name="g"></param>
		/// <param name="bounds"></param>
		/// <param name="state"></param>
		void DrawThumb( Graphics g, Rectangle bounds, ButtonState state );
	}
}
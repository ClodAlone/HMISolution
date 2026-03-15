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
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
	/// <summary>
	/// TabUIOffice2003Properties describes properties for painting tab groups
	///  in AH mode Offise2003 style.
	/// </summary>
	public class TabUIOffice2003Properties
		: OneNoteStyleRendererProperty
	{
		#region Class properties
		/// <summary>
		///Indicates whether to draw from left to right.
		/// </summary>
		public override bool DrawLeftToRight
		{
			get
			{
				return false;
			}
		}
		#endregion

		#region Class constants
		/// <summary>
		/// Space between top border and panel.
		/// </summary>
		private const int DEF_OVERLAP_HEIGHT = 1;
		#endregion

		#region Class overrides
		/// <summary>
		/// Returns the size by which the selected tab overlaps the inactive tabs.
		/// </summary>
		public override SizeF GetOverlapSize( SizeF tabSize )
		{			
			SizeF overlapSize = base.GetOverlapSize( tabSize );
			overlapSize.Height = DEF_OVERLAP_HEIGHT;
			return overlapSize;
		}
		/// <summary>
		/// Paints panel back ground.
		/// </summary>
		/// <param name="tabControl">Panel owner.</param>
		/// <param name="g">Graphics to use.</param>
		/// <param name="bgColor">Background color.</param>
		/// <param name="bounds">Bounds of panel to paint.</param>
		public override void OnPaintPanelBackground(ITabControl tabControl, Graphics g, Color bgColor, Rectangle bounds)
		{
			bool vertical = true;
			Brush brush;

			if( bounds.Width > bounds.Height )
				vertical = false;

			if( bounds != Rectangle.Empty )
			{
				if( bounds.Width > 0 && bounds.Height > 0 )
				{
					if( vertical )
					{
						brush = new LinearGradientBrush( bounds, Color.White, bgColor, LinearGradientMode.Horizontal );
					}
					else
					{
						brush = new LinearGradientBrush( bounds, bgColor, Color.White, LinearGradientMode.Vertical );
					}
					g.FillRectangle( brush, bounds );
				}
			}
		}
		#endregion
	}
}

#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region Class using directives
using System;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
#endregion

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
	/// <summary>
	/// TabUIOffice2007Properties describes properties for painting tab groups
	///  in AH mode Offise2007 style.
	/// </summary>
	class TabUIOffice2007Properties
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
		private const int DEF_BORDER_WIDTH = 1;
		#endregion

		#region Class overrides		
		/// <summary>
		/// Returns the size by which the selected tab overlaps the inactive tabs.
		/// </summary>
		public override SizeF GetOverlapSize( SizeF tabSize )
		{			
			SizeF overlapSize = base.GetOverlapSize(tabSize);
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
		public override void OnPaintPanelBackground( ITabControl tabControl, Graphics g, Color bgColor, Rectangle bounds )
		{
			Brush brush; 
			Pen borderPen; 
			TabGroupRendererOffice2007 tgrOffice2007 = null;
			if (tabControl.Renderer.Renderers.Count > 0)
			{
				tgrOffice2007 = tabControl.Renderer.Renderers[0] as TabGroupRendererOffice2007;
			}
			if (tgrOffice2007 != null)
			{
				brush = new SolidBrush(tgrOffice2007.ThemeColors.TabPanelColor);
				borderPen = new Pen(tgrOffice2007.ThemeColors.TabPanelBorderColor, DEF_BORDER_WIDTH);
			}
			else
			{
				brush = new SolidBrush(Office2007Colors.Default.TabPanelColor);
				borderPen = new Pen(Office2007Colors.Default.TabPanelBorderColor, DEF_BORDER_WIDTH);
			}

			TabAlignment align = tabControl.Renderer.TabPanelData.Alignment;
			Point p1 = Point.Empty;
			Point p2 = Point.Empty;

			switch( align )
			{ 
				case TabAlignment.Bottom:
					p1 = new Point(bounds.Left, bounds.Bottom-1);					
					p2 = new Point(bounds.Right, bounds.Bottom-1);
					break;

				case TabAlignment.Top:
					p1 = new Point( bounds.Left, bounds.Top );
					p2 = new Point( bounds.Right, bounds.Top );
					break;

				case TabAlignment.Right:
					p1 = new Point(bounds.Right-1, bounds.Top);
					p2 = new Point(bounds.Right-1, bounds.Bottom);
					break;

				case TabAlignment.Left:
					p1 = bounds.Location;
					p2 = new Point( bounds.Left, bounds.Bottom );
					break;
			}

			g.FillRectangle(brush, bounds);
			g.DrawLine(borderPen, p1, p2);
		}
		#endregion
	}
}

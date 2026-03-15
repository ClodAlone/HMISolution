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
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Tools.Renderers;
#endregion

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
	/// <summary>
	/// TabGroupRendererOffice2003 paints tabs in group mode.
	/// </summary>
	public class TabGroupRendererOffice2003
		: TabGroupRenderer
	{
		#region Class members
		/// <summary>
		/// Properties for panel painting for current style.
		/// </summary>
		static TabUIOffice2003Properties m_tabPropertyExtender;
		#endregion

		#region Class constants
		// Theese colors used to draw shadow for selected item.
		private static readonly Color DEF_SHADOW_COLOR_DARK = Color.FromArgb( 70, Color.Black );
		private static readonly Color DEF_SHADOW_COLOR_LIGHT = Color.FromArgb( 20, Color.Black );
		/// <summary>
		/// Item border width.
		/// </summary>
		public const int DEF_BORDER_WIDTH = 1;
		/// <summary>
		/// Width of shadow rectangle.
		/// </summary>
		private const int DEF_SHADOW_WIDTH = 2;
		#endregion		

		#region Class properties
		/// <summary>
		/// Gets tab style name.
		/// </summary>
		public static new string TabStyleName 
		{
			get
			{
				return "DockingTabsOffice2003";
			}
		}
		/// <summary>
		/// Returns the <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties"/>
		/// instance that provides default properties for this renderer.
		/// </summary>
		public static new TabUIOffice2003Properties TabPanelPropertyExtender
		{
			get
			{
				return m_tabPropertyExtender;
			}
		}
		#endregion

		#region Class Initialise/Finalize methods
		/// <summary>
		/// Registers class types.
		/// </summary>
		static TabGroupRendererOffice2003()
		{
			m_tabPropertyExtender = new TabUIOffice2003Properties();
			TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabGroupRendererOffice2003), TabPanelPropertyExtender);
		}
		/// <summary>
		/// Creates an instance of the <see cref="Syncfusion.Windows.Forms.Tools.Renderers.TabGroupRendererOffice2003"/>.
		/// </summary>
		/// <param name="parent">The <see cref="Syncfusion.Windows.Forms.Tools.ITabControl"/> instance.</param>
		/// <param name="panelRenderer">The parent <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> instance.</param>
		public TabGroupRendererOffice2003(ITabControl parent, ITabPanelRenderer panelRenderer)
			: base(parent, panelRenderer)
		{
		}
		#endregion

		#region Class private methods
		/// <summary>
		/// Returns the rectangle to draw shadow in from item's bounds.
		/// </summary>
		/// <param name="bounds"> Item bounds to draw shadow for. </param>
		private RectangleF GetShadowBounds( RectangleF bounds )
		{
			switch( this.TabAlignment )
			{
				case System.Windows.Forms.TabAlignment.Bottom:
				case System.Windows.Forms.TabAlignment.Top:
					bounds.Location = new PointF( ++bounds.X+bounds.Width, bounds.Y );
					bounds.Width = DEF_SHADOW_WIDTH;
					break;

				case System.Windows.Forms.TabAlignment.Left:
				case System.Windows.Forms.TabAlignment.Right:
					bounds.Location = new PointF( bounds.X, ++bounds.Height + bounds.Y );
					bounds.Height = DEF_SHADOW_WIDTH;
					break;
			}

			return bounds;
		}
		#endregion

		#region Class overrides
		protected override RectangleF CorrectBounds(RectangleF bounds)
		{
			return base.CorrectBounds (bounds);
		}
		/// <summary>
		/// Draws shadow near the tab.
		/// </summary>
		/// <param name="g">Graphics to use.</param>
		/// <param name="shadowRectangle">Rectangle to shadow near.</param>
		protected virtual void DrawShadow( Graphics g, RectangleF shadowRectangle )
		{
			if( g == null )
				throw new ArgumentNullException( "g" );
			Brush brush;

			switch( this.TabAlignment )
			{
				case System.Windows.Forms.TabAlignment.Bottom:
				case System.Windows.Forms.TabAlignment.Top:
					brush = new LinearGradientBrush( shadowRectangle, 
						DEF_SHADOW_COLOR_DARK, DEF_SHADOW_COLOR_LIGHT,
						LinearGradientMode.Horizontal );
					break;

				case System.Windows.Forms.TabAlignment.Left:
				case System.Windows.Forms.TabAlignment.Right:
					brush = new LinearGradientBrush( shadowRectangle, 
						DEF_SHADOW_COLOR_DARK, DEF_SHADOW_COLOR_LIGHT,
						LinearGradientMode.Vertical );
					break;

				default: 
					throw new ArgumentException("Unknown alignment.");
			}

			g.FillRectangle( brush, shadowRectangle );
		}
		/// <summary>
		/// Draws borders of tab.
		/// </summary>
		/// <param name="drawItemInfo">Drawing arguments.</param>
		protected override void DrawBorders( DrawTabEventArgs drawItemInfo )
		{
            Graphics g = drawItemInfo.Graphics;

            AHTabControl tabctrl = this.TabControl.GetControl() as AHTabControl;
            int width = drawItemInfo.Bounds.Width;
            if (tabctrl != null)
                width = (tabctrl.Edge == DockingStyle.Bottom || tabctrl.Edge == DockingStyle.Top) ? drawItemInfo.Bounds.Width + 5 : drawItemInfo.Bounds.Width;
            Rectangle borderRect = new Rectangle(drawItemInfo.Bounds.X, drawItemInfo.Bounds.Y, width, drawItemInfo.Bounds.Height);
			Pen borderPen = new Pen( Office2003Colors.SelBorderColor, DEF_BORDER_WIDTH );

			switch( this.TabAlignment )
			{
				case System.Windows.Forms.TabAlignment.Bottom:
					borderRect.Y--;
					break;

                case System.Windows.Forms.TabAlignment.Right:
					borderRect.X--;
					break;
			}
			RectangleF shadowBounds = GetShadowBounds( borderRect );

			g.DrawRectangle( borderPen, borderRect );
			DrawShadow( g, shadowBounds );
		}
		/// <summary>
		/// Draws background of tab.
		/// </summary>
		/// <param name="drawItemInfo">Drawing arguments.</param>
		protected override void DrawBackground(DrawTabEventArgs drawItemInfo)
		{
			// Do nothing! AH panel tabs are inactive always.
		}
		#endregion
	}
}

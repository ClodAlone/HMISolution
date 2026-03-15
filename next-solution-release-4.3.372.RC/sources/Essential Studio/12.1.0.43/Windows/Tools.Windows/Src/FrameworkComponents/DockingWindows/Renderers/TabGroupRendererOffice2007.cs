#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region Class using directives
using System;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
	/// <summary>
	/// Paints group tabs in office 2007 style.
	/// </summary>
	class TabGroupRendererOffice2007
		: TabGroupRenderer
	{
		#region Class members
		/// <summary>
		/// Properties for panel painting for current style.
		/// </summary>
		static TabUIOffice2007Properties m_tabPropertyExtender;
		private GraphicsState m_savedState = null;
		private Office2007Theme m_theme;
		private Office2007Colors m_themeColors;
		#endregion

		#region Class Constants
		/// <summary>
		/// Border width.
		/// </summary>
		private const int c_borderWidth = 1;
		#endregion

		#region Class properties
		/// <summary>
		/// Gets tab style name.
		/// </summary>
		public static new string TabStyleName
		{
			get
			{				
				return "DockingTabsOffice2007";
			}
		}
		/// <summary>
		/// Returns the <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties"/>
		/// instance that provides default properties for this renderer.
		/// </summary>
		public static new TabUIOffice2007Properties TabPanelPropertyExtender
		{
			get
			{
				return m_tabPropertyExtender;
			}
		}

		public Office2007Theme Theme
		{
			get { return m_theme; }
			set
			{
				if (m_theme != value)
				{
					m_theme = value;
					m_themeColors = Office2007Colors.GetColorTable(value);
				}
			}
		}

		public Office2007Colors ThemeColors
		{
			get { return m_themeColors; }
		}
		#endregion

		#region Class Initialise/Finalize methods
		/// <summary>
		/// Registers class types.
		/// </summary>
		static TabGroupRendererOffice2007()
		{
			m_tabPropertyExtender = new TabUIOffice2007Properties();
			TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabGroupRendererOffice2007), TabPanelPropertyExtender);
		}
		/// <summary>
		/// Creates an instance of the <see cref="Syncfusion.Windows.Forms.Tools.Renderers.TabGroupRendererOffice2007"/>.
		/// </summary>
		/// <param name="parent">The <see cref="Syncfusion.Windows.Forms.Tools.ITabControl"/> instance.</param>
		/// <param name="panelRenderer">The parent <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> instance.</param>
		public TabGroupRendererOffice2007(ITabControl parent, ITabPanelRenderer panelRenderer)
			: base(parent, panelRenderer)
		{
			m_theme = Office2007Theme.Blue;
			m_themeColors = Office2007Colors.GetColorTable(m_theme);
		}
		#endregion

		#region Class overrides
		/// <summary>
		/// Corrects bounds of tab item.
		/// </summary>
		/// <param name="bounds">default bounds.</param>
		/// <returns>corrected bounds.</returns>
		protected override RectangleF CorrectBounds( RectangleF bounds )
		{
            bounds.Width += 5;
			return base.CorrectBounds(bounds);
		}
		/// <summary>
		/// Correctes bounds for interior.
		/// </summary>
		/// <param name="bounds">default bounds.</param>
		protected override void CorrectTabBounds( ref RectangleF bounds )
		{
			bounds.Y += 2;
		}
		/// <summary>
		/// Gets text color
		/// </summary>
		/// <returns>Color of text in tab item</returns>
		protected override Color GetForeColor()
		{
			return ThemeColors.TabItemTextColor;
		}

		private Rectangle GetItemInnerBounds(DrawTabEventArgs drawItemInfo)
		{
			Rectangle itemBounds = Rectangle.Round(
				TabUtils.ApplyTransform(
				drawItemInfo.Graphics, this.TabAlignment, drawItemInfo.Bounds, true));
			return new Rectangle(
				itemBounds.Left + 1,
				itemBounds.Top + 1,
				itemBounds.Width - 1,
				itemBounds.Height - 2);
		}
				
		/// <summary>
		/// Draws borders of tab.
		/// </summary>
		/// <param name="drawItemInfo">Drawing arguments.</param>
		protected override void DrawBorders( DrawTabEventArgs drawItemInfo )
		{			
			if( drawItemInfo.Bounds.Width == 0 || drawItemInfo.Bounds.Height == 0 )
				return;			

			Pen outerPen = new Pen(this.ThemeColors.TabItemOuterBorderColor, c_borderWidth);
			Pen innerPen = new Pen(this.ThemeColors.TabItemInnerBorderColor, c_borderWidth);
			Pen borderPen = new Pen(this.ThemeColors.TabItemBorderColor, c_borderWidth);			

			Graphics gph = drawItemInfo.Graphics;
			RectangleF curBounds = TabUtils.ApplyTransform(gph, this.TabAlignment, drawItemInfo.Bounds, true);

			// Make g horizontal
			ApplyTransform(gph);
			SaveGraphicsState(gph, ref curBounds);
			Rectangle itemBounds = Rectangle.Round(curBounds);

			Point leftBottom = new Point(itemBounds.Left, itemBounds.Bottom);
			Point leftTop = new Point(itemBounds.Left, itemBounds.Top + 2);
			Point topLeft = new Point(itemBounds.Left + 3, itemBounds.Top );
			Point topRight = new Point(itemBounds.Right - 3, itemBounds.Top );
			Point rightTop = new Point(itemBounds.Right, itemBounds.Top + 2);
			Point rightBottom = new Point(itemBounds.Right, itemBounds.Bottom);

			gph.DrawLine(outerPen, leftBottom, leftTop);
			gph.DrawLine(outerPen, rightBottom, rightTop);
			gph.DrawLine(borderPen, topLeft, topRight);
			leftBottom.X++;
			leftTop.X++;
			rightBottom.X--;
			rightTop.X--;
			gph.DrawLine(borderPen, leftBottom, leftTop);
			gph.DrawLine(borderPen, rightBottom, rightTop);
			
			Point sLeftTop = leftTop;
			Point sTopLeft = topLeft;
			Point sRightTop = rightTop;
			Point sTopRight = topRight;			
			
			leftBottom.X++;
			leftTop.X++;
			rightBottom.X--;
			rightTop.X--;
			topLeft.Y++;
			topRight.Y++;
			
			GraphicsPath path = new GraphicsPath();
			path.AddLine(leftBottom, leftTop);
			path.AddLine(leftTop, topLeft);
			path.AddLine(topLeft, topRight);
			path.AddLine(topRight, rightTop);
			path.AddLine(rightTop, rightBottom);
			gph.DrawPath(innerPen, path);

			SmoothingMode saved = gph.SmoothingMode;
			gph.SmoothingMode = SmoothingMode.AntiAlias;
			gph.DrawLine(borderPen, sLeftTop, sTopLeft);
			gph.DrawLine(borderPen, sRightTop, sTopRight);
			gph.SmoothingMode = saved;

			Rectangle innerBounds = new Rectangle(leftTop.X+1, leftTop.Y-1,
				rightTop.X - leftTop.X-1, leftBottom.Y - leftTop.Y);

			m_itemInnerBounds = innerBounds;
			RestoreGraphicsState(gph);

			gph.ResetTransform();
		}
		Rectangle m_itemInnerBounds = Rectangle.Empty;
		/// <summary>
		/// Draws background of tab.
		/// </summary>
		/// <param name="drawItemInfo">Drawing arguments.</param>
		protected override void DrawBackground( DrawTabEventArgs drawItemInfo )
		{	
			Point mouse = Cursor.Position;
			Control ctrl = ( this.TabControl as Control );
			mouse = ctrl.PointToClient(mouse);
			bool isSelected = false;
			isSelected = drawItemInfo.Bounds.Contains(mouse);

			Rectangle innerBounds = GetItemInnerBounds( drawItemInfo );
			if( innerBounds != Rectangle.Empty )
			{				
				Graphics g = drawItemInfo.Graphics;
				ApplyTransform(g);

				LinearGradientBrush inactive = new LinearGradientBrush(innerBounds
				, this.ThemeColors.TabItemTopGradientColor
				, this.ThemeColors.TabItemInActiveBottomColor
				, LinearGradientMode.Vertical);

				g.FillRectangle(inactive, innerBounds);

				if( isSelected )
				{
					LinearGradientBrush activeBrush = new LinearGradientBrush(innerBounds
						, this.ThemeColors.TabItemTopGradientColor
						, this.ThemeColors.TabItemActiveBottomColor
						, LinearGradientMode.Vertical);

					g.FillRectangle(activeBrush, innerBounds);
				}

				RestoreGraphicsState(g);
				g.ResetTransform();
			}
		}
		#endregion

		#region Class helper methods		
		protected override void SaveGraphicsState( Graphics g, ref RectangleF curBounds )
		{
			m_savedState = g.Save();			
		}

		protected override void RestoreGraphicsState( Graphics g )
		{
			if( m_savedState != null )
			{
				g.Restore(m_savedState);
				m_savedState = null;
			}
		}
		#endregion
	}
}

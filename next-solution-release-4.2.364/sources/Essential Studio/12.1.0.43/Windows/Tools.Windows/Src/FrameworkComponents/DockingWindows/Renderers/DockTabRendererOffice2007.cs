#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
	public class DockRendererPropertyOffice2007
		:TabPanelProperty2D
	{
		#region Class Constants
		/// <summary>
		/// Space between top border and panel.
		/// </summary>
		private const int DEF_OVERLAP_HEIGHT = 2;
		#endregion

		#region Class Overrides
		public override Color DefaultTabForeColor( ITabPanelData panelData, ITabControl tabControl )
		{
			DockTabRendererOffice2007 trOffice2007 = null;
			if (tabControl.Renderer.Renderers.Count > 0)
			{
				trOffice2007 = tabControl.Renderer.Renderers[0] as DockTabRendererOffice2007;
			}
			if (trOffice2007 != null)
			{
				return trOffice2007.ThemeColors.DockTabForeColor;
			}
			else
			{
				return Office2007Colors.Default.DockTabForeColor;
			}
		}
		public override void OnPaintPanelBackground( ITabControl tabControl, Graphics g, Color bgColor, Rectangle bounds )
		{
			DockTabRendererOffice2007 dtrOffice2007 = null;
			if (tabControl.Renderer.Renderers.Count > 0)
			{
				dtrOffice2007 = tabControl.Renderer.Renderers[0] as DockTabRendererOffice2007;
			}
			Pen borderPen = null;
			if (dtrOffice2007 != null)
			{
				g.FillRectangle(new SolidBrush(dtrOffice2007.ThemeColors.DockTabBackgroundColor), bounds);
				borderPen = new Pen(dtrOffice2007.ThemeColors.TabDefaultBorderColor);
			}
			else
			{
				g.FillRectangle(new SolidBrush(Office2007Colors.Default.DockTabBackgroundColor), bounds);
				borderPen = new Pen(Office2007Colors.Default.TabDefaultBorderColor);
			}
			g.DrawLine(borderPen, bounds.Left, bounds.Bottom, bounds.Right, bounds.Bottom);
		}
		/// <summary>
		/// Returns the size by which the selected tab overlaps the inactive tabs.
		/// </summary>
		public override SizeF GetOverlapSize( SizeF tabSize )
		{
			Size size = Size.Empty;
			size.Height = DEF_OVERLAP_HEIGHT;
			return size;
		}

		#endregion
	}

	public class DockTabRendererOffice2007
		:TabRendererOffice2007
	{
		/// <summary>
		/// Returns the unique name of this tab renderer.
		/// </summary>
		public static new string TabStyleName
		{
			get
			{
				return "DockTabRendererOffice2007";
			}
		}
		/// <summary>
		/// Use TabPanelPropertyExtender property as my default properties provider.
		/// </summary>
		private static DockRendererPropertyOffice2007 m_tabPropertyExtender;

		/// <summary>
		/// Returns the <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties"/>
		/// instance that provides default properties for this renderer.
		/// </summary>
		public static new DockRendererPropertyOffice2007 TabPanelPropertyExtender
		{
			get
			{
				return m_tabPropertyExtender;
			}
		}

		public override SizeF GetOverlapSize( SizeF tabSize )
		{
			return Size.Empty;
		}

		static DockTabRendererOffice2007()
		{
			m_tabPropertyExtender = new DockRendererPropertyOffice2007();
			TabRendererFactory.RegisterTabType(TabStyleName, typeof(DockTabRendererOffice2007), TabPanelPropertyExtender);
		}

		/// <summary>
		/// Creates a new instance of the DockTabRenderer Office2007 class.
		/// </summary>
		/// <param name="parent">The tab control parent.</param>
		/// <param name="panelRenderer">The <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> parent.</param>
		public DockTabRendererOffice2007( ITabControl parent, ITabPanelRenderer panelRenderer )
			: base( parent, panelRenderer )
		{
		}
	}
}

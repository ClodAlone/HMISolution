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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;
using Syncfusion.Windows.Forms.Tools.Win32API;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	#region MenuDropDownDesigner
	class MenuDropDownDesigner : ControlDesigner
	{
		#region *** MenuDropDownGlyph
		class MenuDropDownGlyph : ControlBodyGlyph
		{
			#region Constructors
			public MenuDropDownGlyph(MenuDropDownDesigner designer)
				: base(Rectangle.Empty, Cursors.Default, designer.Component, designer)
			{
				m_designer = designer;
			}
			#endregion

			#region Properties
			/// <summary>
			/// 
			/// </summary>
			public override Rectangle Bounds
			{
				get
				{
					Control c = this.RelatedComponent as Control;

					if (c != null && c.Visible)
					{
						return c.Bounds;
					}
					return Rectangle.Empty;
				}
			}
			#endregion

			#region Overrides
			public override Cursor GetHitTest(Point p)
			{
				if (this.Bounds.Contains(p))
				{
					return Cursors.Default;
				}
				return null;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="pe"></param>
			public override void Paint(PaintEventArgs pe)
			{
				ISelectionService selSvc = m_designer.GetService(typeof(ISelectionService)) as ISelectionService;

				if (selSvc != null && selSvc.GetComponentSelected(this.RelatedComponent))
				{
					Rectangle rc = this.Bounds;

					if (rc.Width > 0 && rc.Height > 0)
					{
						pe.Graphics.DrawRectangle(Pens.Black, rc.X - 1, rc.Y - 1, rc.Width + 1, rc.Height + 1);
					}
				}
			}
			#endregion

			#region Fields
			MenuDropDownDesigner m_designer;
			#endregion
		}
		#endregion

		#region *** MenuDropDownOpeningHandler
		class MenuDropDownOpeningHandler : IDisposable
		{
			#region Constructor
			public MenuDropDownOpeningHandler(IComponent component)
			{
				m_dropDown = component as MenuDropDown;
				if(m_dropDown!=null)
				{
					m_dropDown.Opening+=new CancelEventHandler(OnDropDownOpening);
				}
			}
			#endregion

			#region IDisposable Members
			public void  Dispose()
			{
				if(m_dropDown!=null)
				{
					m_dropDown.Opening-=new CancelEventHandler(OnDropDownOpening);
				}
			}
			#endregion
			
			#region Event handlers
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			void  OnDropDownOpening(object sender, CancelEventArgs e)
			{
 				e.Cancel = true;
			}
			#endregion

			#region Fields
			MenuDropDown m_dropDown;
			#endregion
		}
		#endregion

		#region Constructors
		public MenuDropDownDesigner()
		{
			m_dropDownGlyphs = new GlyphCollection();
		}
		#endregion

		#region Properties
		public override DesignerActionListCollection ActionLists
		{
			get
			{
				if (m_actionLists == null)
				{
					m_actionLists = new DesignerActionListCollection();
				}
				return m_actionLists;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override SelectionRules SelectionRules
		{
			get { return SelectionRules.None; }
		}
		/// <summary>
		/// 
		/// </summary>
		internal ControlBodyGlyph BodyGlyph
		{
			get
			{
				if(m_bodyGlyph==null)
				{
					m_bodyGlyph = new MenuDropDownGlyph(this);
				}
				return m_bodyGlyph;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private ISelectionService SelectionService
		{
			get { return this.GetService(typeof(ISelectionService)) as ISelectionService; }
		}
		/// <summary>
		/// 
		/// </summary>
		private GlyphSelectionType SelectionType
		{
			get
			{
				if (this.SelectionService.GetComponentSelected(m_dropDown))
				{
					return GlyphSelectionType.Selected;
				}
				return GlyphSelectionType.NotSelected;
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		public override void Initialize(IComponent component)
		{
			using(MenuDropDownOpeningHandler openingHandler = new MenuDropDownOpeningHandler(component))
			{
				base.Initialize(component);
			}

			m_dropDown = component as MenuDropDown;
			if(m_dropDown!=null)
			{
				m_ribbonAdornerSvc = RibbonAdornerService.Get(m_dropDown.Site);

				m_dropDown.VisibleChanged += new EventHandler(OnVisibleChanged);
				m_dropDown.SizeChanged += new EventHandler(OnDropDownSizeChanged);

				UpdatePanel(m_dropDown.MainPanel);
				UpdatePanel(m_dropDown.AuxPanel);
				UpdatePanel(m_dropDown.SystemPanel);

				DesignerUtils.UpdateDropDownParent(m_dropDown);
			}

			this.SelectionService.SelectionChanged += new EventHandler(OnSelectionChanged);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if (m_dropDown != null)
			{
				m_dropDown.VisibleChanged -= new EventHandler(OnVisibleChanged);
				m_dropDown.SizeChanged -= new EventHandler(OnDropDownSizeChanged);
			}

			ISelectionService selSvc = this.SelectionService;
			if(selSvc!=null)
			{
				selSvc.SelectionChanged -= new EventHandler(OnSelectionChanged);
			}

			base.Dispose(disposing);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		protected override bool GetHitTest( Point point )
		{
			Point pt = m_dropDown.PointToClient( point );

			bool bResult =
				m_dropDown.MainPanel.UpScrollBounds.Contains(pt) ||
				m_dropDown.MainPanel.DownScrollBounds.Contains(pt) ||
				m_dropDown.AuxPanel.UpScrollBounds.Contains(pt) ||
				m_dropDown.AuxPanel.DownScrollBounds.Contains(pt) ;

			return bResult;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
			switch ((Msg)m.Msg)
			{
				case Msg.WM_SETCURSOR:
					WindowsAPI.SetCursor(Cursors.Default.Handle);
					m.Result = (IntPtr)1;
					return;
				case Msg.WM_LBUTTONDOWN:
				case Msg.WM_LBUTTONUP:
				case Msg.WM_RBUTTONDOWN:
				case Msg.WM_RBUTTONUP:
					ToolStrip ts = this.Control as ToolStrip;
					if(ts!=null)
					{
						Point pt = WindowsAPI.GetPointFromLPARAM((int)m.LParam);
						if (ts.GetItemAt(pt)!=null)
						{
							m.Result = IntPtr.Zero;
							return;
						}
					}
					break;
			}
			base.WndProc(ref m);
		}
		#endregion

		#region Event handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnSelectionChanged(object sender, EventArgs e)
		{
			if (m_dropDown.Visible)
			{
				UpdateGlyphs();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnVisibleChanged(object sender, EventArgs e)
		{
			UpdateGlyphs();

			if (m_dropDown.Visible)
			{
				m_dropDown.ItemAdded += new ToolStripItemEventHandler(OnDropDownItemAdded);
				m_dropDown.ItemRemoved += new ToolStripItemEventHandler(OnDropDownItemRemoved);
				m_dropDown.LayoutCompleted += new EventHandler(OnLayoutCompleted);

				// Place dropdown between the root control and ToolStripAdornerWindow
				Control root = DesignerUtils.GetRootControl(m_dropDown);
				if (root != null)
				{
					IntPtr hPrev = WindowsAPI.GetWindow(root.Handle, GetWindowCmd.GW_HWNDPREV);

					if (hPrev != IntPtr.Zero && hPrev != m_dropDown.Handle)
					{
						WindowsAPI.SetWindowPos(m_dropDown.Handle, hPrev, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE);
					}
				}
				SelectMenuDropDownItem(null/*m_dropDown.GetNextItem(null, ArrowDirection.Down)*/);
			}
			else
			{
				m_dropDown.ItemAdded -= new ToolStripItemEventHandler(OnDropDownItemAdded);
				m_dropDown.ItemRemoved -= new ToolStripItemEventHandler(OnDropDownItemRemoved);
				m_dropDown.LayoutCompleted -= new EventHandler(OnLayoutCompleted);
			}
			
			this.BehaviorService.Invalidate();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnDropDownSizeChanged(object sender, EventArgs e)
		{
			BehaviorService behaviorSvc = this.GetService(typeof(BehaviorService)) as BehaviorService;
			if (behaviorSvc != null)
			{
				behaviorSvc.Invalidate();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnDropDownItemAdded(object sender, ToolStripItemEventArgs e)
		{
			SelectMenuDropDownItem(e.Item);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnDropDownItemRemoved(object sender, ToolStripItemEventArgs e)
		{
			SelectMenuDropDownItem(m_dropDown.GetNextItem(null, ArrowDirection.Down));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnLayoutCompleted(object sender, EventArgs e)
		{
			UpdateGlyphs();
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="panel"></param>
		private void UpdatePanel(MenuDropDown.Panel panel)
		{
			ToolStripDropDown dropDown = panel.ToolStrip as ToolStripDropDown;
			if (dropDown != null)
			{
				ToolStripDropDownButton ownerItem = new ToolStripDropDownButton();
				
				ownerItem.Available = false;
				ownerItem.Owner = m_dropDown.ParentToolStrip;
				
				dropDown.OwnerItem = ownerItem;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private void UpdateGlyphs()
		{
			if (m_ribbonAdornerSvc != null)
			{
				GlyphCollection glyphs = m_ribbonAdornerSvc.Adorner.Glyphs;

				foreach (Glyph g in m_dropDownGlyphs)
				{
					glyphs.Remove(g);
				}

				m_dropDownGlyphs.Clear();

				if (m_dropDown != null && m_dropDown.Visible)
				{
					GlyphSelectionType selectionType = this.SelectionType;

					m_dropDownGlyphs.Add(GetControlGlyph(selectionType));

					m_dropDownGlyphs.AddRange(this.GetGlyphs(selectionType));

					UpdateGlyphs(m_dropDownGlyphs, m_dropDown.MainItems, m_dropDown.MainItemsBounds);
					UpdateGlyphs(m_dropDownGlyphs, m_dropDown.AuxItems, m_dropDown.AuxItemsBounds);
					UpdateGlyphs(m_dropDownGlyphs, m_dropDown.SystemItems, m_dropDown.SystemItemsBounds);
				}

				foreach (Glyph g in m_dropDownGlyphs)
				{
					glyphs.Insert(0, g);
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="items"></param>
		private void UpdateGlyphs(GlyphCollection glyphs, ToolStripItemCollection items, Rectangle bounds)
		{
			foreach (ToolStripItem item in items)
			{
				if (bounds.Contains(item.Bounds))
				{
					Glyph glyph = DesignerUtils.GetToolStripItemGlyph(item);

					if (glyph != null)
					{
						glyphs.Add(glyph);
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		private void SelectMenuDropDownItem(Component component)
		{
			ISelectionService selSvc = this.GetService(typeof(ISelectionService)) as ISelectionService;
			if (component != null)
			{
				this.SelectionService.SetSelectedComponents(new IComponent[] { component }, SelectionTypes.Replace);
			}
			else
			{
				this.SelectionService.SetSelectedComponents(new IComponent[] { m_dropDown }, SelectionTypes.Replace);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private ToolStripItem GetItemAt(int x, int y)
		{
			Point ptClient = m_dropDown.PointToClient(new Point(x, y));
			
			return m_dropDown.GetItemAt(ptClient);
		}
		#endregion

		#region Fields
		/// <summary>
		/// 
		/// </summary>
		MenuDropDown m_dropDown;
		/// <summary>
		/// 
		/// </summary>
		ControlBodyGlyph m_bodyGlyph;
		/// <summary>
		/// 
		/// </summary>
		DesignerActionListCollection m_actionLists;
		/// <summary>
		/// 
		/// </summary>
		RibbonAdornerService m_ribbonAdornerSvc;
		/// <summary>
		/// 
		/// </summary>
		GlyphCollection m_dropDownGlyphs;
		#endregion
	}
	#endregion
}
#endif

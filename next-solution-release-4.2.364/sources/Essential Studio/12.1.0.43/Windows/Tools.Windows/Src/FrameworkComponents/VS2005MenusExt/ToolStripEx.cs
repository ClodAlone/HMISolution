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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using System.ComponentModel;
using System.Collections;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Text;
//Syncfusion
using Syncfusion.Windows.Forms.Tools.Win32API;
using System.Collections.Generic;

namespace Syncfusion.Windows.Forms.Tools
{
	#region BoolEx
	public enum BoolEx
	{
		Default,
		True,
		False,
	}
	#endregion
    /// <summary>
    /// StatusStripEx Style
    /// </summary>
    public enum ToolStripExStyle
    {
        /// <summary>
        /// Classic appearance.
        /// </summary>
        Default,
        /// <summary>
        /// Metro-like appearance.
        /// </summary>
        Metro
    }
	#region IOffice12Porperties
	/// <summary>
	/// Office12 related properties.
	/// </summary>
	public interface IOffice12Settings
	{
		#region Properties
		/// <summary>
		/// Style of launcher.
		/// </summary>
		LauncherStyle LauncherStyle { get; set; }
		/// <summary>
		/// 
		/// </summary>
		bool ShowCaption { get; set; }
		/// <summary>
		/// 
		/// </summary>
		bool ShowLauncher { get; set; }
		/// <summary>
		/// 
		/// </summary>
		CaptionStyle CaptionStyle { get; set; }
		/// <summary>
		/// 
		/// </summary>
		CaptionTextStyle CaptionTextStyle { get; set; }
		/// <summary>
		/// 
		/// </summary>
		CaptionAlignment CaptionAlignment { get; set; }
		/// <summary>
		/// 
		/// </summary>
		Font CaptionFont { get; set; }
		/// <summary>
		/// 
		/// </summary>
		int CaptionMinHeight { get; set; }
		/// <summary>
		/// 
		/// </summary>
		ToolStripBorderStyle BorderStyle { get; set; }
		/// <summary>
		/// 
		/// </summary>
		ToolStripEx.ColorScheme OfficeColorScheme { get; set; }
        /// <summary>
        /// 
        /// </summary>
        RibbonStyle RibbonStyle { get; } 
        /// <summary>
        /// 
        /// </summary>
        Office2013ColorScheme Office2013ColorScheme { get; }
		#endregion
	}
	#endregion

	#region ToolStripEx
	[Designer(typeof(Syncfusion.Windows.Forms.Tools.Design.ToolStripExDesigner))]
	[ToolboxBitmap(typeof(Syncfusion.Windows.Forms.Tools.ToolStripEx), "ToolboxIcons.ToolStripEx.bmp")]
	[Description("Represents Office 2007 Style ToolStripEx")]
	public partial class ToolStripEx : System.Windows.Forms.ToolStrip, IToolStripExSupport2, INativeMessageFilter,IVisualStyle 
	{
		#region Constants
		public enum ColorScheme
		{
			[Browsable(false)]
			Default,
			Managed,
			Silver,
			Blue,
			Black,
		}
		/// <summary>
		/// State of ToolStripEx.
		/// </summary>
		public enum ToolStripExState
		{
			/// <summary>
			/// All the items are visible.
			/// </summary>
			Expanded,
			/// <summary>
			/// All the items are hidden to the dropdown.
			/// </summary>
			Collapsed
		}
		/// <summary>
		/// Height of space between image and text and between text and down arrow in collapsed state dropdown button.
		/// </summary>
		protected internal const int DEF_PIXELS_BETWEEN_ELEMENTS_COLLAPSED = 2;
		protected internal const int DEF_IMAGE_BORDER_OFFSET = 5;
		private const int DEF_MINIMAL_COLLAPSED_WIDTH = 15;

		const string TOOLTIP_CLASSNAME = "tooltips_class32";
		const int CLASSNAME_BUFLEN = 256;
		#endregion

		#region *** LayoutEngineCollapsed
		internal class LayoutEngineCollapsed : LayoutEngine
		{
			public LayoutEngineCollapsed() { }
			public override void InitLayout(object child, BoundsSpecified specified) { }
			public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
			{
				ToolStripEx ts = container as ToolStripEx;
				if (ts != null)
				{
					Point pt = ts.DisplayRectangle.Location;
					ToolStripItem item = ts.DropDownButton;

					pt.X += item.Margin.Left;
					pt.Y += item.Margin.Top;

					ts.SetItemLocation(ts.DropDownButton, pt);
				}
				return false;
			}
		}
		#endregion

		#region Constructors
		static ToolStripEx()
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(ToolStripEx));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
			m_DefaultForeColor = Color.MidnightBlue;
			m_OfficeRenderers = new Hashtable();
			office2010Renderers = new Hashtable();
            office2013Renderers = new Hashtable();
			m_LayoutEngineStub = new LayoutEngineStub();
			m_LayoutEngineCollapsed = new LayoutEngineCollapsed();
		}
		/// <summary>
		/// 
		/// </summary>
		public ToolStripEx()
			: this(GetRenderer(ColorScheme.Managed))
		{
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="renderer"></param>
		public ToolStripEx(ToolStripRenderer renderer)
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(ToolStripEx));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
			base.ShowItemToolTips = false;
            this.LauncherMouseEnter += delegate { };
            this.LauncherMouseLeave += delegate { };
			this.Renderer = renderer;
			this.ForeColor = m_DefaultForeColor;

			m_toolTips = new Hashtable();
			m_image = new Bitmap(typeof(ToolStripButton), "blank.bmp");
		}
		#endregion

		#region Methods
		/// <summary>
		/// Returns collection of toolstrip items. Pays caution to the state of toolstrip.
		/// </summary>
		/// <returns>Collection of toolstrip items.</returns>
		public ToolStripItemCollection GetItems()
		{
			return (this.State == ToolStripExState.Expanded) ? (this.Items) : (m_collapsedDropDown.Panel.ToolStrip.Items);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="szProposed"></param>
		/// <returns></returns>
		internal Size GetPreferredSizeCore(Size szProposed)
		{
			if (this.State == ToolStripExState.Collapsed)
			{
				return this.DropDownButton.Panel.ToolStrip.GetPreferredSizeCore(szProposed);
			}
			return base.GetPreferredSize(szProposed);
		}
		#endregion

		#region Public overrides
		/// <summary>
		/// Retrieves the size of a rectangular area into which a control can be fitted.
		/// </summary>
		/// <param name="szProposed"> The custom-sized area for a control.</param>
		/// <returns> An ordered pair of type System.Drawing.Size representing the width and height
		/// of a rectangle.</returns>
		public override Size GetPreferredSize(Size szProposed)
		{
			Size szResult = GetPreferredSizeCore(szProposed);

			if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
			{
				szResult.Width += this.CaptionHeightInternal;
			}
			else
			{
				szResult.Height += this.CaptionHeightInternal;
			}

			int nBorderWidth = this.BorderWidth;

			szResult.Width += 2 * nBorderWidth;
			szResult.Height += 2 * nBorderWidth;

			return szResult;
		}
		#endregion

		#region Protected overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.State == ToolStripExState.Collapsed)
				{
					if (this.DropDownButton != null)
					{
						ToolStripPanelItem panel = this.DropDownButton.Panel;
                        if (panel != null && panel.ToolStrip != null)
                        {
                            panel.ToolStrip.LauncherClick -= new EventHandler(OnDropDownLauncherClick);
                            panel.ToolStrip.LauncherMouseEnter -= delegate { };
                            panel.ToolStrip.LauncherMouseLeave -= delegate { };
                        }
					}
				}

				if (m_collapsedDropDown != null)
				{
					m_collapsedDropDown.DropDown.Closing -= new ToolStripDropDownClosingEventHandler(OnCollapsedDropDownClosing);
                    m_collapsedDropDown.Dispose();
					m_collapsedDropDown = null;
				}

                if (m_CaptionFont != null)
                {
                    m_CaptionFont.Dispose();
                    m_CaptionFont = null;
                }
			}
			if(!this.DesignMode)
			base.Dispose(disposing);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);

			if (this.ShowCaption)
			{
				RefreshCaption();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFontChanged(EventArgs e)
		{
			m_nCaptionHeight = -1;
			base.OnFontChanged(e);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRendererChanged(EventArgs e)
		{
			m_nCaptionHeight = -1;

			base.OnRendererChanged(e);

			if (m_foreColor.IsEmpty)
			{
				base.ForeColor = this.DefForeColor;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

            if ((this.Parent is IOffice12Settings) && (this.Parent as IOffice12Settings).RibbonStyle == RibbonStyle.Office2010)
                UpdateRegion();

			RefreshCaption();
			SetCollapsedSize(this.Height);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaddingChanged(EventArgs e)
		{
			base.OnPaddingChanged(e);
			
			if (this.State == ToolStripExState.Collapsed)
			{
				this.DropDownButton.Panel.Padding = this.Padding;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			Office12ToolStripRenderer renderer = this.Renderer as Office12ToolStripRenderer;
            Rectangle rcLauncher = Rectangle.Empty;

            if (renderer != null)
                rcLauncher = ToolStripRendererUtils.GetLauncherBounds(this, renderer.RenderType);
            else
                rcLauncher = ToolStripRendererUtils.GetLauncherBounds(this, Office12ToolStripRenderer.ERENDERTYPE.Normal);
            
            bool bLauncherSelected = rcLauncher.Contains(PointToScreen(e.Location));

            if (bLauncherSelected != m_bLauncherSelected)
            {
                if (bLauncherSelected)
                    LauncherMouseEnter(this, e);
                else
                {
                    LauncherMouseLeave(this, e);
                }
                m_bLauncherSelected = bLauncherSelected;
                RefreshCaption();
            }
		}
        public delegate void LauncherEnter(object o, MouseEventArgs e);
        public event LauncherEnter LauncherMouseEnter;
        public delegate void LauncherLeave(object o, EventArgs e);
        public event LauncherLeave LauncherMouseLeave;
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
            if (this.Parent.Parent is RibbonControlAdv)
            {
                touch = (this.Parent.Parent as RibbonControlAdv).RibbonTouchModeEnabled;
            }
			if (this.Parent.Parent is RibbonControlAdv)
			{
				if ((this.Parent.Parent as RibbonControlAdv).RibbonStyle == RibbonStyle.Office2010)
				{
					if ((this.Parent.Parent as RibbonControlAdv).OfficeColorScheme == ColorScheme.Black)
					{
						foreach (ToolStripItem item in this.Items)
						{
							if (item is ToolStripSeparator)
							{
								ToolStripSeparator Seperator = item as ToolStripSeparator;
								e.Graphics.DrawLine(Pens.Gray, new Point(Seperator.Bounds.X + 3, Seperator.Bounds.Y + 5), new Point(Seperator.Bounds.X + 3, Seperator.Bounds.Y + Seperator.Bounds.Height - 5));
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			if (m_bLauncherSelected)
			{
				m_bLauncherSelected = false;
                LauncherMouseLeave(this, e);
				RefreshCaption();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseClick(MouseEventArgs e)
		{
			bool bProcessed = false;

			Office12ToolStripRenderer renderer = this.Renderer as Office12ToolStripRenderer;
            Office2010ToolStripRenderer renderer2010 = this.Renderer as Office2010ToolStripRenderer;
            Office2013ToolStripRenderer renderer2013 = this.Renderer as Office2013ToolStripRenderer;
			if (renderer !=null||renderer2010!=null ||renderer2013!=null )
            {
                Rectangle rcLauncher;
               if( renderer != null)
                   rcLauncher = ToolStripRendererUtils.GetLauncherBounds(this, renderer.RenderType);
               else if (renderer2010 != null)
                rcLauncher = ToolStripRendererUtils.GetLauncherBounds(this, renderer2010.RenderType);
               else
                   rcLauncher = ToolStripRendererUtils.GetLauncherBounds(this, renderer2013.RenderType);
				if (rcLauncher.Contains(PointToScreen(e.Location)))
				{
					OnLauncherClick();
					bProcessed = true;
				}
			}

			if (!bProcessed)
			{
				base.OnMouseClick(e);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnLauncherClick()
		{
			if (LauncherClick != null)
			{
				LauncherClick(this, EventArgs.Empty);
			}
		}
        internal bool touch = false;
        private void setTouchFont(ToolStripItem ctrl)
        {
            foreach (ToolStripItem TComboBox in (ctrl as ToolStripPanelItem).Items)
            {
                if (TComboBox is ToolStripComboBox)
                {
                    (TComboBox as ToolStripComboBox).Font = new System.Drawing.Font(TComboBox.Font.FontFamily, TComboBox.Font.Size + 1.25F, TComboBox.Font.Style);
                    (TComboBox as ToolStripComboBox).SelectionLength = 0;
                }
                if (TComboBox is ToolStripPanelItem)
                    setTouchFont(TComboBox);
            }
        }
        private void resetTouchFont(ToolStripItem ctrl)
        {
            foreach (ToolStripItem TComboBox in (ctrl as ToolStripPanelItem).Items)
            {
                if (TComboBox is ToolStripComboBox)
                {
                    (TComboBox as ToolStripComboBox).Font = new System.Drawing.Font(TComboBox.Font.FontFamily, TComboBox.Font.Size - 1.25F, TComboBox.Font.Style);
                }
                if (TComboBox is ToolStripPanelItem)
                    resetTouchFont(TComboBox);
            }
        }
        private bool touchValueAssigned = false;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLayoutCompleted(EventArgs e)
		{
			base.OnLayoutCompleted(e);
            if (this.Parent != null && this.Parent.Parent != null && this.Parent.Parent is RibbonControlAdv)
            {
                if ((this.Parent.Parent as RibbonControlAdv).RibbonTouchModeEnabled)
                {
                    if (!touchValueAssigned)
                    {
                        foreach (ToolStripItem ctrl in this.Items)
                        {
                            if (ctrl is ToolStripPanelItem)
                            {
                                setTouchFont(ctrl);
                            }
                            else if (ctrl is ToolStripComboBox)
                            {
                                (ctrl as ToolStripComboBox).Font = new System.Drawing.Font(ctrl.Font.FontFamily, ctrl.Font.Size + 1.25F, ctrl.Font.Style);
                            }
                        }
                    }
                    touchValueAssigned = true;
                }
                else
                {
                    if (touchValueAssigned)
                    {
                        foreach (ToolStripItem ctrl in this.Items)
                        {
                            if (ctrl is ToolStripPanelItem)
                            {
                                resetTouchFont(ctrl);
                            }
                            else if (ctrl is ToolStripComboBox)
                            {
                                (ctrl as ToolStripComboBox).Font = new System.Drawing.Font(ctrl.Font.FontFamily, ctrl.Font.Size - 1.25F, ctrl.Font.Style);
                            }
                        }
                        touchValueAssigned = false;
                    }
                }
               
            }
     
            if (this.Parent != null && this.Parent.Parent is RibbonControlAdv.RibbonControlPopup && (this.Parent.Parent as RibbonControlAdv.RibbonControlPopup).RibbonStyle == RibbonStyle.Office2013)
            {
                if (this.Renderer is Office2013ToolStripRenderer)
                {
                    (this.Renderer as Office2013ToolStripRenderer).ToolStipOffice2013ColorScheme = (this.Parent.Parent as RibbonControlAdv.RibbonControlPopup).Office2013ColorScheme;
                    (this.Renderer as Office2013ToolStripRenderer).MenuColor = ControlPaint.LightLight((this.Parent.Parent as RibbonControlAdv.RibbonControlPopup).MenuColor);
                    (this.Renderer as Office2013ToolStripRenderer).UseDefaultHighlightColor = (this.Parent.Parent as RibbonControlAdv.RibbonControlPopup).UseDefaultHighlightColor;
                }
            }
			if (this.ShowCaption)
			{
				m_bSetOverflowBounds = true;
				try
				{
					OverflowButton.Size = Rectangle.Intersect(OverflowButton.Bounds, this.ClientRectangle).Size;
				}
				finally
				{
					m_bSetOverflowBounds = false;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnHandleDestroyed(EventArgs e)
		{
			foreach (DictionaryEntry entry in m_toolTips)
			{
				NativeMessageHandler handler = entry.Value as NativeMessageHandler;
				if (handler != null)
				{
					handler.MessageFilter = null;
				}
			}
			m_toolTips.Clear();

			base.OnHandleDestroyed(e);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns>Returns true to allow changing of State property</returns>
		protected virtual bool OnStateChanging()
		{
			bool bResult = true;

			if (StateChanging != null)
			{
				CancelEventArgs eventArgs = new CancelEventArgs(false);

				StateChanging(this, eventArgs);
				bResult = !eventArgs.Cancel;
			}

			return bResult;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnItemAdded(ToolStripItemEventArgs e)
		{
			base.OnItemAdded(e);

			ToolStripControlHost hostItem = e.Item as ToolStripControlHost;
			if (hostItem != null)
			{
				Control c = hostItem.Control;
				if (c != null && c.Parent != this)
				{
					c.Parent = null;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
			Msg msg = (Msg)m.Msg;
			switch (msg)
			{
				case Msg.WM_NOTIFY:
					{
						NMHDR hdr = (NMHDR)Marshal.PtrToStructure(m.LParam, typeof(NMHDR));

						if (hdr.code == (int)TTN.TTN_SHOW)
						{
							IntPtr hWnd = hdr.hwndFrom;

							if (!m_toolTips.ContainsKey(hWnd))
							{
								NativeMessageHandler handler = new NativeMessageHandler();

								handler.Assign(hWnd);
								handler.MessageFilter = this;

								m_toolTips[hWnd] = handler;
							}
						}
					}
					break;
				case Msg.WM_MOUSEACTIVATE:
					for (Control parent = this.Parent; parent != null; parent = parent.Parent)
					{
						if (parent is RibbonControlAdv.RibbonControlPopup)
						{
							m.Result = (IntPtr)MouseActivateFlags.MA_NOACTIVATE;
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
		void OnDropDownLauncherClick(object sender, EventArgs e)
		{
			OnLauncherClick();
		}
		/// <summary>
		/// Raises CollapsedDropDownClosing event if ToolStripEx is in collapsed state;
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnCollapsedDropDownClosing(object sender, ToolStripDropDownClosingEventArgs e)
		{
			if (this.State == ToolStripExState.Collapsed)
			{
				if (this.CollapsedDropdownClosing != null)
				{
					this.CollapsedDropdownClosing(this, e);
				}
			}
		}

		#endregion

		#region INativeMessageFilter implementation
		bool INativeMessageFilter.ProcessMessage(ref Message m)
		{
			bool bResult = false;

			if (m.HWnd != IntPtr.Zero)
			{
				if (ToolStripEx.GetIsToolTip(m.HWnd))
				{
					ToolStripGallery tsGallery = GetItemAt(this.PointToClient(Control.MousePosition)) as ToolStripGallery;
					if (tsGallery != null && tsGallery.ShowToolTip)
						NativeWindow.FromHandle(m.HWnd).DestroyHandle();
				}
			}

			switch ((Msg)m.Msg)
			{
				case Msg.WM_WINDOWPOSCHANGING:
					bResult = OnWindowPosChanging(ref m);
					break;
			}

			return bResult;
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		internal void RefreshCaption()
		{
			if (this.ShowCaption && this.IsHandleCreated)
			{
				RedrawWindowFlags flags = RedrawWindowFlags.RDW_FRAME | RedrawWindowFlags.RDW_INVALIDATE;
				WindowsAPI.RedrawWindow(this.Handle, IntPtr.Zero, IntPtr.Zero, flags);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal void UpdateCaption()
		{
            if ((this.Renderer is Office12ToolStripRenderer || this.Renderer is Office2010ToolStripRenderer || this.Renderer is Office2013ToolStripRenderer) && this.IsHandleCreated)
			{
				WindowsAPI.SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, 0, 0, SWP_ON_FRAMECHANGED);
				WindowsAPI.RedrawWindow(this.Handle, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.RDW_INVALIDATE);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal void UpdateRenderer()
		{
            IOffice12Settings officeSettings = this.Parent as IOffice12Settings;

            if (officeSettings != null && officeSettings.RibbonStyle == RibbonStyle.Office2010)
            {
                this.Renderer = GetOffice2010Renderer(this.OfficeColorScheme);

                UpdateRegion();
            }
            if (officeSettings != null && officeSettings.RibbonStyle == RibbonStyle.Office2013)
            {
                this.Renderer = GetOffice2013Renderer(this.OfficeColorScheme);
                if (this.Parent.Parent is RibbonControlAdv && this.Renderer is Office2013ToolStripRenderer)
                {
                    (this.Renderer as Office2013ToolStripRenderer).ToolStipOffice2013ColorScheme = (this.Parent.Parent as RibbonControlAdv).Office2013ColorScheme;
                    (this.Renderer as Office2013ToolStripRenderer).MenuColor = ControlPaint.LightLight((this.Parent.Parent as RibbonControlAdv).MenuColor);
                    (this.Renderer as Office2013ToolStripRenderer).UseDefaultHighlightColor = (this.Parent.Parent as RibbonControlAdv).UseDefaultHighlightColor;
                }
                UpdateRegion();
            }
			if (this.Office12Mode)
			{
				this.Renderer = GetRenderer(this.OfficeColorScheme);
			}
            if (this.VisualStyle == ToolStripExStyle.Metro)
            {
                this.Renderer = new Office2013ToolStripRenderer();
            }
		}

        private void UpdateRegion()
        {
            Rectangle rect = new Rectangle(1, 0, this.Width - 1, this.Height - 1);
            this.Region = new Region(rect);
        }

        private ToolStripRenderer GetOffice2013Renderer(ColorScheme colorScheme)
        {
            ToolStripRenderer renderer = null;

            renderer = office2013Renderers[colorScheme] as ToolStripRenderer;
                renderer = new Office2013ToolStripRenderer(new Office2010ColorTable(Office2010ColorScheme.Blue), Syncfusion.Windows.Forms.Tools.Office2013ToolStripRenderer.ERENDERTYPE.Normal);
                office2013Renderers[colorScheme] = renderer;
            return renderer;
        }
        private ToolStripRenderer GetOffice2010Renderer(ColorScheme colorScheme)
        {
            ToolStripRenderer renderer = null;

            renderer = office2010Renderers[colorScheme] as ToolStripRenderer;

            if (renderer == null)
            {
                switch (colorScheme)
                {
                    case ColorScheme.Silver:
                        renderer = new Office2010ToolStripRenderer(new Office2010ColorTable(Office2010ColorScheme.Silver), Syncfusion.Windows.Forms.Tools.Office2010ToolStripRenderer.ERENDERTYPE.Normal);
                        break;
                    case ColorScheme.Blue:
                        renderer = new Office2010ToolStripRenderer(new Office2010ColorTable(Office2010ColorScheme.Blue), Syncfusion.Windows.Forms.Tools.Office2010ToolStripRenderer.ERENDERTYPE.Normal);
                        break;
                    case ColorScheme.Black:
                        renderer = new Office2010ToolStripRenderer(new Office2010ColorTable(Office2010ColorScheme.Black), Syncfusion.Windows.Forms.Tools.Office2010ToolStripRenderer.ERENDERTYPE.Normal);
                        break;
                    default:
                        renderer = new Office2010ToolStripRenderer(new Office2010ColorTable(Office2010ColorScheme.Managed), Syncfusion.Windows.Forms.Tools.Office2010ToolStripRenderer.ERENDERTYPE.Normal);
                        break;
                }

                office2010Renderers[colorScheme] = renderer;
            }

            return renderer;
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		bool OnWindowPosChanging(ref Message m)
		{
			WINDOWPOS winPos = (WINDOWPOS)Marshal.PtrToStructure(m.LParam, typeof(WINDOWPOS));

			IntPtr hCursor = WindowsAPI.GetCursor();
			if (hCursor != IntPtr.Zero)
			{
				Cursor cur = new Cursor(hCursor);

				Point pt = Cursor.Position;

				winPos.x = pt.X;
				winPos.y = pt.Y + cur.Size.Height - cur.HotSpot.Y;
			}

			Marshal.StructureToPtr(winPos, m.LParam, false);

			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="colorScheme"></param>
		/// <returns></returns>
		static ToolStripRenderer GetRenderer(ColorScheme colorScheme)
		{
			ToolStripRenderer renderer = null;
            
			renderer = m_OfficeRenderers[colorScheme] as ToolStripRenderer;

			if (renderer == null)
			{
				switch (colorScheme)
				{
					case ColorScheme.Silver:
						renderer = new Office12ToolStripRenderer(new Office12ColorTable());
						break;
					case ColorScheme.Blue:
						renderer = new Office12ToolStripRenderer(new OfficeBlue());
						break;
					case ColorScheme.Black:
						renderer = new Office12ToolStripRenderer(new OfficeBlack());
						break;
					default:
						renderer = new Office12ToolStripRenderer(Office12ColorTable.ManagedColors);
						break;
				}

				m_OfficeRenderers[colorScheme] = renderer;
			}

			return renderer;
		}
		/// <summary>
		/// Updates state of the control.
		/// </summary>
		private void UpdateState()
		{
			this.SuspendLayout();
			this.DropDownButton.Panel.Control.SuspendLayout();

			switch (this.State)
			{
				case ToolStripExState.Collapsed:
					{
						ToolStripPanelItem panel = this.DropDownButton.Panel;

						m_expandedSize = this.Size;

						ToolStripItem[] items = new ToolStripItem[this.Items.Count];
						this.Items.CopyTo(items, 0);

						panel.Items.AddRange(items);
						panel.Padding = this.Padding;

						panel.ToolStrip.LauncherClick += new EventHandler(OnDropDownLauncherClick);

						this.Items.Add(this.DropDownButton);

						this.Width = this.CollapsedWidth;
					}
					break;

				case ToolStripExState.Expanded:
					{
						if (m_collapsedDropDown != null)
						{
							m_collapsedDropDown.DropDown.Close();
						}

						ToolStripItem[] items = new ToolStripItem[this.DropDownButton.Panel.Items.Count];
						this.DropDownButton.Panel.Items.CopyTo(items, 0);

						this.Items.Remove(this.DropDownButton);
						this.DropDownButton.Panel.ToolStrip.LauncherClick -= new EventHandler(OnDropDownLauncherClick);

						this.Items.AddRange(items);

						this.Size = m_expandedSize;
					}
					break;
			}

			this.DropDownButton.Panel.Control.ResumeLayout(false);
			this.ResumeLayout();

			UpdateCaption();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="toolStripHeight"></param>
		private void SetCollapsedSize(int toolStripHeight)
		{
			if (m_collapsedDropDown != null) // Don't need to create dropdown button if it wasn't requested from the outside yet.
			{
				ToolStripDropDownButton button = this.DropDownButton;

				int height = toolStripHeight - (this.Padding.Vertical + button.Margin.Vertical + button.Padding.Vertical);
                int width = TextRenderer.MeasureText(button.Text, button.Font).Width;

                int arrowHeight = 0;
                if (this.Renderer is Office12ToolStripRenderer)
                {
                    arrowHeight = (this.Renderer as Office12ToolStripRenderer).GetDownArrowSize().Height;
                }
                else if (this.Renderer is Office2013ToolStripRenderer)
                {
                    arrowHeight = (this.Renderer as Office2013ToolStripRenderer).GetDownArrowSize().Height;
                }
                else if (this.Renderer is Office2010ToolStripRenderer)
                {
                    arrowHeight = (this.Renderer as Office2010ToolStripRenderer).GetDownArrowSize().Height;
                }

                Size textSize = TextRenderer.MeasureText(button.Text, button.Font);
                int h = +textSize.Height + DEF_PIXELS_BETWEEN_ELEMENTS_COLLAPSED;

					if (h < height)
					{
						if (textSize.Width > 0)
						{
							width = textSize.Width;
						}

						Size imageSize = this.ImageScalingSize;
						imageSize.Height += 2 * DEF_IMAGE_BORDER_OFFSET;
						imageSize.Width += 2 * DEF_IMAGE_BORDER_OFFSET;

						h += imageSize.Height;

						if (h < height)
						{
							if (width < imageSize.Width)
							{
								width = imageSize.Width;
							}
							button.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
						}
						else button.DisplayStyle = ToolStripItemDisplayStyle.Text;
					}
					else button.DisplayStyle = ToolStripItemDisplayStyle.None;

				button.Size = new Size(width + button.Padding.Horizontal, height + button.Padding.Vertical);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private void OnImageChanged()
		{
			if (ImageChanged != null)
			{
				ImageChanged(this, EventArgs.Empty);
			}
		}
		/// <summary>
		/// Converts BoolEx to bool.
		/// </summary>
		/// <param name="boolEx">Input BoolEx.</param>
		/// <returns>Output bool.</returns>
		public static bool BoolExToBool(BoolEx boolEx)
		{
			if (boolEx == BoolEx.True) return true;
			if (boolEx == BoolEx.False) return false;

			throw new Exception();
		}
		/// <summary>
		/// Converts bool to BoolEx.
		/// </summary>
		/// <param name="bBool">Input bool.</param>
		/// <returns>Output BoolEx.</returns>
		public static BoolEx BoolToBoolEx(bool bBool)
		{
			return bBool ? BoolEx.True : BoolEx.False;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hWnd"></param>
		/// <returns></returns>
		static bool GetIsToolTip(IntPtr hWnd)
		{
			bool bResult = false;

			if (hWnd != IntPtr.Zero)
			{
				StringBuilder sName = new StringBuilder(CLASSNAME_BUFLEN);
				if (GetClassName(hWnd, sName, CLASSNAME_BUFLEN) > 0)
				{
					bResult = sName.ToString().IndexOf(TOOLTIP_CLASSNAME) >= 0;
				}
			}
			return bResult;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="sName"></param>
		/// <param name="nMaxCount"></param>
		/// <returns></returns>
		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		static extern int GetClassName(IntPtr hWnd, StringBuilder sName, int nMaxCount);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hWndParent"></param>
		/// <param name="hWnd"></param>
		/// <returns></returns>
		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		static extern bool IsChild(IntPtr hWndParent, IntPtr hWnd);
		/// <summary>
		/// Imitates launcher clicking.
		/// </summary>
		public void PerformLauncherClick()
		{
			OnLauncherClick();
		}
		#endregion

		#region Properties
		#region IToolStripExSupport2 implementation
		/// <summary>
		/// Gets or Sets whether the caption should be shown or not.
		/// </summary>
		/// <value>true, if caption should be shown, false otherwise</value>
		[Category("Office mode")]
		[Description("Gets or Sets whether the caption should be shown or not.")]
		public virtual bool ShowCaption
		{
			get
			{
				if (this.State == ToolStripExState.Collapsed) return false;

				return this.ShowCaptionInternal;
			}
			set
			{
				this.ShowCaptionInternal = value;
			}
		}
		/// <summary>
		/// Gets or Sets whether the Launcher should be shown or not.
		/// </summary>
		/// <value>true, if Launcher should be shown, false otherwise</value>
		[Category("Office mode")]
		[Description("Gets or Sets whether the Launcher should be shown or not.")]
		public virtual bool ShowLauncher
		{
			get
			{
				if (m_bShowLauncher == BoolEx.Default)
				{
					IOffice12Settings office12settings = this.Parent as IOffice12Settings;
					return office12settings != null ? office12settings.ShowLauncher : true;
				}

				return BoolExToBool(m_bShowLauncher);
			}
			set
			{
				BoolEx bValue = BoolToBoolEx(value);

				if (m_bShowLauncher != bValue)
				{
					m_bShowLauncher = bValue;

					UpdateCaption();
				}
			}
		}
		/// <summary>
		/// Gets or sets whether the caption should be aligned to top or bottom.
		/// </summary>
		[Category("Office mode")]
		[Description("Gets or sets whether the caption should be aligned to top or bottom..")]
		public virtual CaptionStyle CaptionStyle
		{
			get
			{
				if (m_CaptionStyle == CaptionStyle.Default)
				{
					IOffice12Settings office12settings = this.Parent as IOffice12Settings;
					return office12settings != null ? office12settings.CaptionStyle : CaptionStyle.Top;
				}
				return m_CaptionStyle;
			}
			set
			{
				if (m_CaptionStyle != value)
				{
					m_CaptionStyle = value;

					UpdateCaption();
				}
			}
		}
		/// <summary>
		/// Gets or sets whether the caption text should be drawn etched, plain or with shadow.
		/// </summary>
		[Category("Office mode")]
		[Description("Gets or sets whether the caption text should be drawn etched, plain or with shadow.")]
		public virtual CaptionTextStyle CaptionTextStyle
		{
			get
			{
				if (m_CaptionTextStyle == CaptionTextStyle.Default)
				{
					IOffice12Settings office12settings = this.Parent as IOffice12Settings;
					return office12settings != null ? office12settings.CaptionTextStyle : CaptionTextStyle.Shadow;
				}
				return m_CaptionTextStyle;
			}
			set
			{
				if (m_CaptionTextStyle != value)
				{
					m_CaptionTextStyle = value;
					RefreshCaption();
				}
			}
		}
		/// <summary>
		/// Gets or sets the alignment of caption.
		/// </summary>
		[Category("Office mode")]
		[Description("Gets or sets the alignment of caption.")]
		public virtual CaptionAlignment CaptionAlignment
		{
			get
			{
				if (m_CaptionAlignment == CaptionAlignment.Default)
				{
					IOffice12Settings office12settings = this.Parent as IOffice12Settings;
					return office12settings != null ? office12settings.CaptionAlignment : CaptionAlignment.Near;
				}
				return m_CaptionAlignment;
			}
			set
			{
				if (m_CaptionAlignment != value)
				{
					m_CaptionAlignment = value;
					RefreshCaption();
				}
			}
		}
		/// <summary>
		/// Gets or sets the caption font.
		/// </summary>
		[Category("Office mode")]
		[Description("Gets or sets the caption font.")]
		public virtual Font CaptionFont
		{
			get
			{
				if (m_CaptionFont == null)
				{
					IOffice12Settings office12settings = this.Parent as IOffice12Settings;
					return office12settings != null ? office12settings.CaptionFont : this.Font;
				}
				return m_CaptionFont;
			}
			set
			{
				if (m_CaptionFont != value)
				{
					m_CaptionFont = value;
					m_nCaptionHeight = -1;

					UpdateCaption();
				}
			}
		}
		/// <summary>
		/// Gets or sets the minimum height of the caption.
		/// </summary>
		[Category("Office mode")]
		[Description("Gets or sets the minimum height of caption.")]
		public virtual int CaptionMinHeight
		{
			get
			{
				if (m_nCaptionMinHeight == -1)
				{
					IOffice12Settings office12settings = this.Parent as IOffice12Settings;
					return office12settings != null ? office12settings.CaptionMinHeight : 0;
				}
				return m_nCaptionMinHeight;
			}
			set
			{
				if (m_nCaptionMinHeight != value)
				{
					m_nCaptionMinHeight = value;
					m_nCaptionHeight = -1;

					UpdateCaption();
				}
			}
		}
		/// <summary>
		/// Gets or sets the border style for the control.
		/// </summary>
		[Category("Office mode")]
		[Description("Gets or sets the border style for the control.")]
		public virtual ToolStripBorderStyle BorderStyle
		{
			get
			{
				if (m_BorderStyle == ToolStripBorderStyle.Default)
				{
					IOffice12Settings office12settings = this.Parent as IOffice12Settings;
					return office12settings != null ? office12settings.BorderStyle : ToolStripBorderStyle.None;
				}
				return m_BorderStyle;
			}
			set
			{
				if (m_BorderStyle != value)
				{
					m_BorderStyle = value;

					UpdateCaption();
				}
			}
		}
		/// <summary>
		/// Gets or sets whether the launcher style should be Office12 or Office2007.
		/// </summary>
		[Category("Office mode")]
		[Description("Gets or sets whether the launcher style should be Office12 or Office2007.")]
		public virtual LauncherStyle LauncherStyle
		{
			get
			{
				if (m_LauncherStyle == LauncherStyle.Default)
				{
					IOffice12Settings office12settings = this.Parent as IOffice12Settings;
					return office12settings != null ? office12settings.LauncherStyle : LauncherStyle.Office12;
				}
				return m_LauncherStyle;
			}
			set
			{
				if (m_LauncherStyle != value)
				{
					m_LauncherStyle = value;

					if (this.ShowLauncher)
					{
						RefreshCaption();
					}
				}
			}
		}
		/// <summary>
		/// Gets or Sets whether the buttons should be grouped.
		/// </summary>
		[
		Category("Office mode"), DefaultValue(false),
		Description("Gets or Sets whether the buttons should be grouped.")
		]
		public virtual bool GroupedButtons
		{
			get
			{
				return m_bGroupedButtons;
			}
			set
			{
				if (m_bGroupedButtons != value)
				{
					m_bGroupedButtons = value;
					Invalidate();
				}
			}
		}
		/// <summary>
		/// Gets a value indicating whether the launcher is selected.
		/// </summary>
		[Browsable(false)]
		public virtual bool LauncherSelected
		{
			get { return m_bLauncherSelected; }
		}
		/// <summary>
		/// Gets the height of caption.
		/// </summary>
		[Browsable(false)]
		public virtual int CaptionHeight
		{
			get
			{
				if (this.ShowCaption)
				{
					return this.CaptionHeightInternal;
				}
				return 0;
			}
		}
		/// <summary>
		/// Gets the width of border.
		/// </summary>
		protected int BorderWidth
		{
			get
			{
				Office12ToolStripRenderer renderer = this.Renderer as Office12ToolStripRenderer;
                Office2010ToolStripRenderer renderer2010 = this.Renderer as Office2010ToolStripRenderer;
                Office2013ToolStripRenderer renderer2013 = this.Renderer as Office2013ToolStripRenderer;
                if (renderer != null ||renderer2010 != null || renderer2013 != null)
				{
					return ToolStripRendererUtils.GetBorderWidth(this);
				}

				return 0;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal bool ShowCaptionInternal
		{
			get
			{
				if (m_bShowCaption == BoolEx.Default)
				{
					IOffice12Settings office12settings = this.Parent as IOffice12Settings;
					return office12settings != null ? office12settings.ShowCaption : true;
				}
				return BoolExToBool(m_bShowCaption);
			}
			set
			{
				BoolEx bValue = BoolToBoolEx(value);

				if (m_bShowCaption != bValue)
				{
					m_bShowCaption = bValue;

					UpdateCaption();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal int CaptionHeightInternal
		{
			get
			{
				if (this.ShowCaptionInternal)
				{
					if (m_nCaptionHeight < 0)
					{
						if (this.Renderer is Office12ToolStripRenderer)
							m_nCaptionHeight = ToolStripRendererUtils.GetCaptionHeight(this, (this.Renderer as Office12ToolStripRenderer).RenderType);
                        else if (this.Renderer is Office2013ToolStripRenderer || this.Renderer is Office2010ToolStripRenderer)
							m_nCaptionHeight = ToolStripRendererUtils.GetCaptionHeight(this, Office12ToolStripRenderer.ERENDERTYPE.Normal);
						else
							m_nCaptionHeight = 0;
					}
					return m_nCaptionHeight;
				}
				return 0;
			}
		}
		#endregion

		/// <summary>
		/// Gets or sets a value indicating whether ToolTips are to be displayed on the Toolstrip
		/// items.
		/// </summary>
		[DefaultValue(false)]
		public new bool ShowItemToolTips
		{
			get { return base.ShowItemToolTips; }
			set { base.ShowItemToolTips = value; }
		}

        ToolStripTabItem m_tabItem;
        /// <summary>
        /// Gets or sets corresponding ToolStripTabItem.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToolStripTabItem TabItem
        {
            get
            {
                return m_tabItem;
            }
            set
            {
                if (m_tabItem != value)
                {
                    m_tabItem = value;
                }
            }
        }

		/// <summary>
		/// Gets or sets a value indicating whether Office12 mode should be turned on.
		/// </summary>
		/// <value><c>true</c> if [office12 mode]; otherwise, <c>false</c>.</value>
		[Category("Office mode"), DefaultValue(true)]
		[Description("Gets or sets a value indicating whether Office12 or Office2007 mode should be turned on.")]
		public virtual bool Office12Mode
		{
			get
			{
				return this.Renderer is Office12ToolStripRenderer;
			}
			set
			{
				if (value != this.Office12Mode)
				{
					if (value)
					{
						Renderer = null; // Workaround for Microsfot's bug
						Renderer = GetRenderer(this.OfficeColorScheme);
					}
					else
					{
						RenderMode = ToolStripRenderMode.ManagerRenderMode;
					}
				}
			}
		}

        /// <summary>
        /// Gets or sets an advanced appearance for the ToolStripEx.
        /// </summary>
        [Description("Gets or sets an advanced appearance for the ToolStripEx.")]
        [Category("Appearance")]
        [DefaultValue(ToolStripExStyle.Default)]
        public ToolStripExStyle VisualStyle
        {
            get
            {
                return this.stripExstyle;
            }
            set
            {
                if (this.stripExstyle != value)
                {
                    this.stripExstyle = value;
                    this.UpdateRenderer();
                    if (this.stripExstyle == ToolStripExStyle.Default)
                    {
                        this.Office12Mode = true;
                    }
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string style;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return style;
            }
            set
            {
                style = value;

                if (value == "Office2007Blue")
                    OfficeColorScheme = ColorScheme.Blue;
                else if (value == "Office2007Silver")
                    OfficeColorScheme = ColorScheme.Silver;
                else if (value == "Office2007Black")
                    OfficeColorScheme = ColorScheme.Black;
                else if (value == "Managed")
                    OfficeColorScheme = ColorScheme.Managed;
            }
        }
		/// <summary>
		/// Gets or sets whether the Office color scheme should be Silver or Blue.
		/// </summary>
		[Category("Office mode")]
		[Description("Gets or sets whether the Office color scheme should be Silver or Blue.")]
		public virtual ColorScheme OfficeColorScheme
		{
			get
			{
				if (m_ColorScheme == ColorScheme.Default)
				{
					IOffice12Settings office12settings = this.Parent as IOffice12Settings;
					return office12settings != null ? office12settings.OfficeColorScheme : ColorScheme.Managed;
				}
				return m_ColorScheme;
			}
			set
			{
				if (m_ColorScheme != ColorScheme.Default)
					this.ForeColor = Color.MidnightBlue;
				m_ColorScheme = value;

				if (this.Office12Mode)
				{
					this.Renderer = GetRenderer(this.OfficeColorScheme);
				}
			}
		}
		/// <summary>
		/// Gets a cached instance of the control's layout engine. (overridden property)
		/// </summary>
		public override LayoutEngine LayoutEngine
		{
			get
			{
				if (m_bSetOverflowBounds)
				{
					return m_LayoutEngineStub;
				}
				if (this.State == ToolStripExState.Collapsed)
				{
					return m_LayoutEngineCollapsed;
				}
				return base.LayoutEngine;
			}
		}
		/// <summary>
		/// Gets or sets state of ToolStripEx.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ToolStripExState State
		{
			get
			{
				return m_state;
			}
			set
			{
				if (m_state != value && OnStateChanging())
				{
					m_state = value;
					UpdateState();
				}
			}
		}
		/// <summary>
		/// Gets or sets the image of collapsed state dropdown button.
		/// </summary>
		[Obsolete("Use Image instead.")]
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Image CollapsedDropDownButtonImage
		{
			get { return this.Image; }
			set { this.Image = value; }
		}
		/// <summary>
		/// Gets or sets the text of collapsed state dropdown button.
		/// </summary>
		[Category("Office mode")]
		[Localizable(true),Description("Gets or sets the text of collapsed state dropdown button.")]
		public string CollapsedDropDownButtonText
		{
			get
			{
				return this.DropDownButton.Text;
			}
			set
			{
				this.DropDownButton.Text = value;
			}
		}
		/// <summary>
		/// Width of toolstrip in collapsed state.
		/// </summary>
		[Browsable(false)]
		public int CollapsedWidth
		{
			get
			{
				return this.DropDownButton.Width + this.DropDownButton.Margin.Horizontal + this.Padding.Horizontal + (this.Width - this.ClientSize.Width);
			}
		}
		/// <summary>
		/// Size of toolstrip in expanded state.
		/// </summary>
		[Browsable(false)]
		public Size ExpandedSize
		{
			get
			{
				if (this.State == ToolStripExState.Collapsed)
				{
					return m_expandedSize;
				}
				return this.Size;
			}
		}
		/// <summary>
		/// Gets or sets the image of ToolStripEx (shown in quick items panel).
		/// </summary>
		[Category("Office mode")]
		[Description("Gets or sets the image of ToolStripEx (shown in collapsed state and quick items panel ).")]
		public Image Image
		{
			get
			{
				return this.DropDownButton.Image;
			}
			set
			{
				this.DropDownButton.Image = value;

				OnImageChanged();
			}
		}
		/// <summary>
		/// Gets or sets control fore color.
		/// </summary>
		[Browsable(true)]
		[Category("Appearance")]
		[Description("Gets or sets control fore color.")]
		public new virtual Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				if (m_foreColor != value)
				{
					m_foreColor = value;
					base.ForeColor = m_foreColor.IsEmpty ? this.DefForeColor : m_foreColor;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private Color DefForeColor
		{
			get
			{
				ToolStripProfessionalRenderer renderer = this.Renderer as ToolStripProfessionalRenderer;

				if (renderer != null)
				{
					Office12ColorTable officeColorTable = renderer.ColorTable as Office12ColorTable;

					if (officeColorTable != null)
					{
						return officeColorTable.RibbonText;
					}
				}
				return Color.Empty;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected override Padding DefaultMargin
		{
			get
			{
				return new Padding(1, 0, 1, 0);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal bool IsVisible
		{
			get
			{
				if (this.IsHandleCreated)
				{
					int style = WindowsAPI.GetWindowLong(this.Handle, (int)SetWindowLongOffsets.GWL_STYLE);
					return (style & (int)WindowStyles.WS_VISIBLE) != 0;
				}
				return this.Visible;
			}
		}
		#endregion

		#region Private Properties
		/// <summary>
		/// Dropdown button.
		/// </summary>
		internal CollapsedDropDownButton DropDownButton
		{
			get
			{
				if (m_collapsedDropDown == null)
				{
					m_collapsedDropDown = new CollapsedDropDownButton(this);
					m_collapsedDropDown.DropDown.Closing += new ToolStripDropDownClosingEventHandler(OnCollapsedDropDownClosing);
				}

				return m_collapsedDropDown;
			}
		}
		#endregion

		#region ShouldSerialize/Reset methods
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
        private bool ShouldSerializeFont()
        {
            return base.Font == Control.DefaultFont;
        }
		/// <summary>
		/// Occurs when the ToolStrip State is Changing.
		/// </summary>
		public override void ResetFont()
		{
			base.Font = Control.DefaultFont;
		}
		#endregion

		#region Events
		[Category("Action"), Description("Occurs when the launcher button is clicked")]
		public event EventHandler LauncherClick;
		[Browsable(false)]
		public event CancelEventHandler StateChanging;
		/// <summary>
		/// Occurs when the image is changed
		/// </summary>
		[Category("Action"), Description("Occurs when the image is changed")]
		public event EventHandler ImageChanged;
		[Category("Action"), Description("Occurs in collapsed state when the DropDown control is about to close.")]
		public event ToolStripDropDownClosingEventHandler CollapsedDropdownClosing;

		#endregion

		#region Fields
		private int m_nCaptionHeight = -1;
		private bool m_bLauncherSelected = false;
		private BoolEx m_bShowCaption = BoolEx.Default;
		private BoolEx m_bShowLauncher = BoolEx.Default;
		private bool m_bGroupedButtons = false;
		private bool m_bSetOverflowBounds = false;
		/// <summary>
		/// State of the ToolStripEx.
		/// </summary>
		private ToolStripExState m_state = ToolStripExState.Expanded;
		private Image m_image;

		private Font m_CaptionFont = null;
		private int m_nCaptionMinHeight = -1;

		private ColorScheme m_ColorScheme = ColorScheme.Default;
		private ToolStripBorderStyle m_BorderStyle = ToolStripBorderStyle.Default;
		private CaptionStyle m_CaptionStyle = CaptionStyle.Default;
		private CaptionTextStyle m_CaptionTextStyle = CaptionTextStyle.Default;
		private CaptionAlignment m_CaptionAlignment = CaptionAlignment.Default;
		private LauncherStyle m_LauncherStyle = LauncherStyle.Default;
		/// <summary>
		/// Specifies an advanced appearance this control.
		/// </summary>
		private ToolStripExStyle stripExstyle = ToolStripExStyle.Default;
		private Color m_foreColor = Color.Empty;
		private Hashtable m_toolTips;
		/// <summary>
		/// DropDown button shown in collapsed state.
		/// </summary>
		private CollapsedDropDownButton m_collapsedDropDown;

		private Size m_expandedSize = Size.Empty;

		static Color m_DefaultForeColor;

		static Hashtable m_OfficeRenderers;

        static Hashtable office2010Renderers;
        static Hashtable office2013Renderers;
		static LayoutEngineStub m_LayoutEngineStub;
		static LayoutEngineCollapsed m_LayoutEngineCollapsed;

		internal static SetWindowPosFlags SWP_ON_FRAMECHANGED =
				SetWindowPosFlags.SWP_NOZORDER |
				SetWindowPosFlags.SWP_NOSIZE |
				SetWindowPosFlags.SWP_NOMOVE |
				SetWindowPosFlags.SWP_NOACTIVATE |
				SetWindowPosFlags.SWP_FRAMECHANGED;

		#endregion

		#region ShouldSerialize & Reset Methods
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeLauncherStyle()
		{
			return (m_LauncherStyle != LauncherStyle.Default);
		}
		/// <summary>
		/// 
		/// </summary>
		void ResetLauncherStyle()
		{
			this.LauncherStyle = LauncherStyle.Default;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeShowCaption()
		{
			return (m_bShowCaption != BoolEx.Default);
		}
		/// <summary>
		/// 
		/// </summary>
		void ResetShowCaption()
		{
			m_bShowCaption = BoolEx.Default;
			UpdateCaption();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeShowLauncher()
		{
			return (m_bShowLauncher != BoolEx.Default);
		}
		/// <summary>
		/// 
		/// </summary>
		void ResetShowLauncher()
		{
			m_bShowLauncher = BoolEx.Default;
			UpdateCaption();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeCaptionStyle()
		{
			return (m_CaptionStyle != CaptionStyle.Default);
		}
		/// <summary>
		/// 
		/// </summary>
		void ResetCaptionStyle()
		{
			this.CaptionStyle = CaptionStyle.Default;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeCaptionTextStyle()
		{
			return (m_CaptionTextStyle != CaptionTextStyle.Default);
		}
		/// <summary>
		/// 
		/// </summary>
		void ResetCaptionTextStyle()
		{
			this.CaptionTextStyle = CaptionTextStyle.Default;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeCaptionAlignment()
		{
			return (m_CaptionAlignment != CaptionAlignment.Default);
		}
		/// <summary>
		/// 
		/// </summary>
		void ResetCaptionAlignment()
		{
			this.CaptionAlignment = CaptionAlignment.Default;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeCaptionFont()
		{
			return (m_CaptionFont != null);
		}
		/// <summary>
		/// 
		/// </summary>
		void ResetCaptionFont()
		{
			this.CaptionFont = null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeCaptionMinHeight()
		{
			return (m_nCaptionMinHeight != -1);
		}
		/// <summary>
		/// 
		/// </summary>
		void ResetCaptionMinHeight()
		{
			this.CaptionMinHeight = -1;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeBorderStyle()
		{
			return (m_BorderStyle != ToolStripBorderStyle.Default);
		}
		/// <summary>
		/// 
		/// </summary>
		void ResetBorderStyle()
		{
			this.BorderStyle = ToolStripBorderStyle.Default;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeOfficeColorScheme()
		{
			return (m_ColorScheme != ColorScheme.Default);
		}
		/// <summary>
		/// 
		/// </summary>
		void ResetOfficeColorScheme()
		{
			this.OfficeColorScheme = ColorScheme.Default;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeImage()
		{
			return (m_image != null);
		}
		/// <summary>
		/// 
		/// </summary>
		void ResetImage()
		{
			this.Image = null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeForeColor()
		{
			return !m_foreColor.IsEmpty;
		}
		/// <summary>
		/// 
		/// </summary>
		new void ResetForeColor()
		{
			this.ForeColor = Color.Empty;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeCollapsedDropDownButtonText()
		{
			return this.CollapsedDropDownButtonText != string.Empty;
		}
		/// <summary>
		/// 
		/// </summary>
		void ResetCollapsedDropDownButtonText()
		{
			this.CollapsedDropDownButtonText = string.Empty;
		}
		#endregion
	}
	#endregion

	#region LayoutEngineStub
	/// <summary>
	/// Workaround to fix overflow button bounds
	/// </summary>
	internal class LayoutEngineStub : LayoutEngine
	{
		public LayoutEngineStub() { }
		public override void InitLayout(object child, BoundsSpecified specified) { }
		public override bool Layout(object container, LayoutEventArgs layoutEventArgs) { return false; }
	}
	#endregion

	internal static class ColorSchemeConverter
	{
		public static Office2007Theme ToOffice2007Theme(ToolStripEx.ColorScheme colorScheme)
		{
			Office2007Theme theme;

			switch (colorScheme)
			{
				case ToolStripEx.ColorScheme.Silver:
					theme = Office2007Theme.Silver;
					break;

				case ToolStripEx.ColorScheme.Blue:
					theme = Office2007Theme.Blue;
					break;

				case ToolStripEx.ColorScheme.Black:
					theme = Office2007Theme.Black;
					break;

				case ToolStripEx.ColorScheme.Managed:
					theme = Office2007Theme.Managed;
					break;

				default:
					throw new NotSupportedException("Converting from " + colorScheme.ToString() + "to Office2007Theme isn't supported.");
			}

			return theme;
		}
	}
}
#endif
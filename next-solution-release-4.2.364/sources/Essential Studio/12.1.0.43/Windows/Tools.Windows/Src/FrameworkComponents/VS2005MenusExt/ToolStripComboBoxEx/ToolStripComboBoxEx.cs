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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Documentation;
using Syncfusion.Runtime.InteropServices;

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System.Windows.Forms.VisualStyles;
#endif


namespace Syncfusion.Windows.Forms.Tools
{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
	[Description("Represents Office 2007 Style combobox item.")]
	[ToolboxBitmap(typeof(ToolStripComboBoxEx), "ToolboxIcons.ToolStripComboBoxEx.bmp")]
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ContextMenuStrip | ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ToolStrip)]
	[DefaultProperty("Items")]
	public class ToolStripComboBoxEx : ToolStripControlHost
	{
		#region Constructor
		/// <summary>
		/// 
		/// </summary>
		public ToolStripComboBoxEx() : base(new ComboBoxEx(true, true))
		{
			ComboBox cb = this.ComboBox;
			if (cb != null)
			{
                cb.Font = SystemFonts.MenuFont; 
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets a <see cref="T:System.Windows.Forms.ComboBox"></see> in which the user can enter text, along with a list from which the user can select.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Windows.Forms.ComboBox"></see>.
		/// </returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public ComboBox ComboBox
		{
			get
			{
				return (base.Control as ComboBox);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Category("Data"), Description("Gets a collection of the items contained in this ToolStripComboBoxEx.")]
		[Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design", typeof(UITypeEditor))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ComboBox.ObjectCollection Items
		{
			get { return this.ComboBox.Items; }
		}

		/// <summary>
		/// Gets or sets a value specifying the style of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripComboBoxEx"></see>.
		/// </summary>
		/// <returns>
		/// One of the <see cref="T:System.Windows.Forms.ComboBoxStyle"></see> values. The default is <see cref="F:System.Windows.Forms.ComboBoxStyle.DropDown"></see>.
		/// </returns>
		[Category("Appearance"), Description("Gets or sets a value specifying the style of the ToolStripComboBoxEx. ")]
		[DefaultValue(typeof(ComboBoxStyle), "DropDown"), RefreshProperties(RefreshProperties.Repaint)]
		public ComboBoxStyle DropDownStyle
		{
			get
			{
				return this.ComboBox.DropDownStyle;
			}
			set
			{
				this.ComboBox.DropDownStyle = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripComboBoxEx"></see> currently displays its drop-down portion.
		/// </summary>
		/// <returns>
		/// true if the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripComboBoxEx"></see> currently displays its drop-down portion; otherwise, false.
		/// </returns>
		[Description("Gets or sets a value indicating whether the ToolStripComboBoxEx currently displays its drop-down portion.")]
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool DroppedDown
		{
			get
			{
				return this.ComboBox.DroppedDown;
			}
			set
			{
				this.ComboBox.DroppedDown = value;
			}
		}

		/// <summary>
		/// Gets or sets the maximum number of items to be shown in the drop-down portion of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripComboBoxEx"></see>.
		/// </summary>
		/// <returns>The maximum number of items in the drop-down portion. The minimum for this property is 1 and the maximum is 100.</returns>
		[Category("Behavior"), Description("Gets or sets the maximum number of items to be shown in the drop-down portion of the ToolStripComboBoxEx.")]
		[DefaultValue(8), Localizable(true)]
		public int MaxDropDownItems
		{
			get
			{
				return this.ComboBox.MaxDropDownItems;
			}
			set
			{
				this.ComboBox.MaxDropDownItems = value;
			}
		}

		/// <summary>
		/// Gets or sets the maximum number of characters allowed in the editable portion of a combo box.
		/// </summary>
		/// <returns>
		/// The maximum number of characters the user can enter. Values of less than zero are reset to zero, which is the default value.
		/// </returns>
		[Category("Behavior"), Description("Gets or sets the maximum number of characters allowed in the editable portion of a combo box.")]
		[DefaultValue(0), Localizable(true)]
		public int MaxLength
		{
			get
			{
				return this.ComboBox.MaxLength;
			}
			set
			{
				this.ComboBox.MaxLength = value;
			}
		}

		/// <summary>
		/// Gets or sets the index specifying the currently selected item.
		/// </summary>
		/// <returns>
		/// A zero-based index of the currently selected item. A value of negative one (-1) is returned if no item is selected.
		/// </returns>
		[Description("Gets or sets the index specifying the currently selected item.")]
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectedIndex
		{
			get
			{
				return this.ComboBox.SelectedIndex;
			}
			set
			{
				this.ComboBox.SelectedIndex = value;
			}
		}

        /// <summary>
		/// Gets or sets a value indicating whether the items in the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripComboBoxEx"></see> are sorted.
		/// </summary>
		/// <returns>
		/// true if the combo box is sorted; otherwise, false. The default is false.
		/// </returns>
		[Category("Behavior"), Description("Gets or sets a value indicating whether the items in the ToolStripComboBox are sorted.")]
		[DefaultValue(false)]
		public bool Sorted
		{
			get
			{
				return this.ComboBox.Sorted;
			}
			set
			{
				this.ComboBox.Sorted = value;
			}
		}

        /// <summary>
		/// Gets the default size of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripComboBoxEx"></see>.
		/// </summary>
		/// <returns></returns>
		protected override Size DefaultSize
		{
			get
			{
				return new Size(100, 22);
			}
		}
		/// <summary>
		/// Gets the default spacing, in pixels, between the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripComboBoxEx"></see> and an adjacent item.
		/// </summary>
		protected override Padding DefaultMargin
		{
			get
			{
				if (base.IsOnDropDown)
				{
					return new Padding(2);
				}
				return new Padding(1, 0, 1, 0);
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		protected override void OnSubscribeControlEvents(Control control)
		{
			ComboBox cb = control as ComboBox;
			if (cb != null)
			{
				cb.DropDown += new EventHandler(HandleDropDown);
				cb.DropDownClosed += new EventHandler(HandleDropDownClosed);
				cb.SelectedIndexChanged += new EventHandler(HandleSelectedIndexChanged);
				cb.TextUpdate += new EventHandler(HandleTextUpdate);
			}
			base.OnSubscribeControlEvents(control);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		protected override void OnUnsubscribeControlEvents(Control control)
		{
			ComboBox cb = control as ComboBox;
			if (cb != null)
			{
				cb.DropDown -= new EventHandler(HandleDropDown);
				cb.DropDownClosed -= new EventHandler(HandleDropDownClosed);
				cb.SelectedIndexChanged -= new EventHandler(HandleSelectedIndexChanged);
				cb.TextUpdate -= new EventHandler(HandleTextUpdate);
			}
			base.OnUnsubscribeControlEvents(control);
		}

		/// <summary>
		/// Raises the DropDown event.
		/// </summary>
		protected virtual void OnDropDown(EventArgs ea)
		{
			ToolStripDropDown dropDown = GetParentDropDown();
			if (dropDown != null)
			{
				dropDown.Closing += new ToolStripDropDownClosingEventHandler(OnParentClosing);
				dropDown.Closed += new ToolStripDropDownClosedEventHandler(OnParentClosed);
			}

			if (this.DropDown != null)
			{
				this.DropDown(this, ea);
			}
		}
		/// <summary>
		/// Raises the DropDownClosed event.
		/// </summary>
		protected virtual void OnDropDownClosed(EventArgs ea)
		{
			if (this.DropDownClosed != null)
			{
				this.DropDownClosed(this, ea);
			}
		}

		/// <summary>
		/// Raises the SelectedIndexChanged event.
		/// </summary>
		protected virtual void OnSelectedIndexChanged(EventArgs ea)
		{
			if (this.SelectedIndexChanged != null)
			{
				this.SelectedIndexChanged(this, ea);
			}
		}
		/// <summary>
		/// Raises the TextUpdate event.
		/// </summary>
		protected virtual void OnTextUpdate(EventArgs ea)
		{
			if (this.TextUpdate != null)
			{
				this.TextUpdate(this, ea);
			}
		}
		#endregion

		#region Event handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleDropDown(object sender, EventArgs e)
		{
			this.OnDropDown(e);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleDropDownClosed(object sender, EventArgs e)
		{
			this.OnDropDownClosed(e);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleSelectedIndexChanged(object sender, EventArgs e)
		{
			this.OnSelectedIndexChanged(e);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleTextUpdate(object sender, EventArgs e)
		{
			this.OnTextUpdate(e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnParentClosing(object sender, ToolStripDropDownClosingEventArgs e)
		{
			ComboBox cb = this.ComboBox;
			
			if (cb != null && cb.DroppedDown)
			{
				e.Cancel = true;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnParentClosed(object sender, ToolStripDropDownClosedEventArgs e)
		{
			ToolStripDropDown dropDown = sender as ToolStripDropDown;
			if (dropDown != null)
			{
				dropDown.Closing -= new ToolStripDropDownClosingEventHandler(OnParentClosing);
				dropDown.Closed -= new ToolStripDropDownClosedEventHandler(OnParentClosed);
			}
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ts"></param>
		/// <returns></returns>
		private ToolStripDropDown GetParentDropDown()
		{
			Control parent = this.GetCurrentParent();

			while (parent != null && !(parent is ToolStripDropDown))
			{
				parent = parent.Parent;
			}
			return parent as ToolStripDropDown;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool ShouldSerializeFont()
		{
			return !object.Equals(this.Font,SystemFonts.MenuFont);
		}
		#endregion

		#region Events
		/// <summary>Occurs when the drop-down portion of a <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripComboBoxEx"></see> is shown.</summary>
		[Description("Occurs when the drop-down portion of a ToolStripComboBoxEx is shown."), Category("CatBehavior")]
		public event EventHandler DropDown;
		/// <summary>Occurs when the drop-down portion of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripComboBoxEx"></see> has closed.</summary>
		[Description("Occurs when the drop-down portion of the ToolStripComboBoxEx has closed."), Category("CatBehavior")]
		public event EventHandler DropDownClosed;
		/// <summary>Occurs when the value of the <see cref="P:Syncfusion.Windows.Forms.Tools.ToolStripComboBoxEx.SelectedIndex"></see> property has changed.</summary>
		[Description("Occurs when the value of the SelectedIndex property has changed."), Category("CatBehavior")]
		public event EventHandler SelectedIndexChanged;
		/// <summary>Occurs when the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripComboBoxEx"></see> text has changed.</summary>
		[Description("Occurs when the ToolStripComboBoxEx text has changed."), Category("CatBehavior")]
		public event EventHandler TextUpdate;
		#endregion
	}

#endif

	[ToolboxItem(false)]
	[DocumentationExclude]
	public class ComboBoxEx :
		ComboBox
	{
		#region Constants
		const int ARROW_WIDTH = 5;
		const int ARROW_HEIGHT = 3;
		const int ARROW_PADDING = 4;

		const int CBS_STYLED = CBS_NOINTEGRALHEIGHT | CBS_OWNERDRAWFIXED;
		#endregion

		#region Constructors
		static ComboBoxEx()
		{
			m_blSelection = new Blend();
			m_blSelection.Positions = new float[] { 0.0f, 0.5f, 0.5f, 1.0f };
			m_blSelection.Factors = new float[] { 0.0f, 0.2f, 0.5f, 1.0f };

			m_measuringBitmap = new Bitmap(1, 1);
		}
		/// <summary>
		/// 
		/// </summary>
		internal ComboBoxEx() : this(false, false)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		internal ComboBoxEx(bool bIsStyled, bool bShowGripper)
		{
			m_bIsStyled = bIsStyled;
			m_bShowGripper = bShowGripper;
		}
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;

				if (this.IsStyled)
				{
					cp.Style |= CBS_STYLED;
				}

				return cp;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected virtual Office12ColorTable ColorTable
		{
			get
			{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				ToolStrip ts = this.Parent as ToolStrip;

				if (ts != null)
				{
					ToolStripProfessionalRenderer renderer = ts.Renderer as ToolStripProfessionalRenderer;
					if (renderer != null)
					{
						Office12ColorTable colorTable = renderer.ColorTable as Office12ColorTable;
						if (colorTable != null)
						{
							return colorTable;
						}
					}
				}
#endif
				return Office12ColorTable.ManagedColors;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private Color BorderColor
		{
			get
			{
				return this.ColorTable.GroupBorder;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private Color BackgroundColor
		{
			get
			{
				return this.ColorTable.ComboBoxBackgroundColor;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private Color HighlightColor
		{
			get
			{
				return Office12ColorTable.GetAlphaBlendedColor(Color.Transparent, Color.White, 128);
			}
		}

		protected bool IsStyled
		{
			get
			{
				return m_bIsStyled;
			}
			set
			{
				if (m_bIsStyled != value)
				{
					m_bIsStyled = value;

					this.DroppedDown = false;

					if (!this.IsHandleCreated)
					{
						RecreateHandle();
					}

					OnIsStyledChanged();
				}
			}
		}

		protected bool ShowGripper
		{
			get
			{
				return m_bShowGripper;
			}
			set
			{
				m_bShowGripper = value;
			}
		}

		private int ItemHeightInternal
		{
			get
			{
				int height = 0;
				using (Graphics g = Graphics.FromImage(m_measuringBitmap))
				{
					IntPtr hdc = g.GetHdc();

					IntPtr hFont = this.Font.ToHfont();
                    try
                    {
                        IntPtr oldFont = SelectObject(hdc, hFont);

                        RECT rc = new RECT(0, 0, 1, 1);
                        if (0 != DrawText(hdc, "X", 1, ref rc, DT_CALCRECT))
                        {
                            height = (rc.Height + 2);
                        }

                        SelectObject(hdc, oldFont);
                    }
                    finally
                    {
                        DeleteObject(hFont);
                        g.ReleaseHdc(hdc);
                    }
				}
				
				return height;
			}
		}

        
		#endregion

		#region Events

		/// <summary>
		/// Raised when <see cref="IsStyled"/> property is changed.
		/// </summary>
		internal event EventHandler IsStyledChanged;

		#endregion

        #region Methods
        /// <summary>
        /// Create the ListBoxWindow's Handle.
        /// </summary>
        protected virtual void CreateListBoxWindowHandle()
        {
            if (m_list != null && m_list.Handle == IntPtr.Zero)
            {
                COMBOBOXINFO cbInfo = new COMBOBOXINFO();
                cbInfo.cbSize = (uint)Marshal.SizeOf(cbInfo);
                GetComboBoxInfo(this.Handle, ref cbInfo);
                m_list.AssignHandle(cbInfo.hwndList);
            }
        }

        /// <summary>
        /// Releases the ListBoxWindow's Handle.
        /// </summary>
        protected virtual void ReleaseListBoxWindowHandle()
        {
            if (m_list != null)
                m_list.ReleaseHandle();
        }
        #endregion

        #region Overrides

        /// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnHandleCreated(EventArgs e)
		{
			m_list = new ListBoxWindow(this);
			base.OnHandleCreated(e);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnHandleDestroyed(EventArgs e)
		{
            base.OnHandleDestroyed(e);
            if (m_list != null)
            {
                m_list.DestroyHandle();
                m_list = null;
            }
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter(e);
			Invalidate(true);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLeave(EventArgs e)
		{
			base.OnLeave(e);
			Invalidate(true);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if (this.DroppedDown)
			{
				DrawItemBackground(e);
				DrawItemText(e);
			}
			else
			{
                DrawForeground(e.Graphics, e.Bounds);
			}
			base.OnDrawItem(e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);

			if(this.IsHandleCreated)
			{
				int itemHeight = this.ItemHeightInternal;

				SendMessage(this.Handle, CB_SETITEMHEIGHT, -1, itemHeight);
				SendMessage(this.Handle, CB_SETITEMHEIGHT, 0, itemHeight);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
            if (this.IsStyled)
            {
				switch (m.Msg)
				{
					case WM_CTLCOLOREDIT:
					case WM_CTLCOLORSTATIC:
						if (OnCtlColorEdit(ref m)) return;
						break;
					case WM_PAINT:
						OnPaint(ref m);
						return;
					case WM_MOUSEMOVE:
						OnMouseMove(ref m);
						break;
					case WM_MOUSELEAVE:
						OnMouseLeave(ref m);
						break;
					case CB_GETITEMHEIGHT:
						OnGetItemHeight(ref m);
						return;
					case WM_REFLECTEDCOMMAND:
						OnReflectedCommand(ref m);
						break;
				}
			}

			base.WndProc(ref m);
		}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_list != null)
                {
                    m_list.DestroyHandle();
                    m_list = null;
                }
            }
            base.Dispose(disposing);
        }
		#endregion

		#region Mesage handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnCtlColorEdit(ref Message m)
		{
			if (!GetIsHighlighted())
			{
				SetBkMode(m.WParam, TRANSPARENT);

				m.Result = this.ColorTable.ComboBoxBackgroundBrush;

				return true;
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		private void OnPaint(ref Message m)
		{
			COMBOBOXINFO cbi = new COMBOBOXINFO();
			cbi.cbSize = (uint)Marshal.SizeOf(cbi);

			if (GetComboBoxInfo(m.HWnd, ref cbi))
			{
				PAINTSTRUCT stPaint = new PAINTSTRUCT();
				IntPtr hdc = BeginPaint(m.HWnd, ref stPaint);

				if (hdc != IntPtr.Zero)
				{
					Rectangle rcClient = this.ClientRectangle;

					if (rcClient.Width > 0 && rcClient.Height > 0)
					{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
						using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(hdc, rcClient))
						{
							Graphics g = bg.Graphics;
#else
						using( Graphics g = Graphics.FromHdc( hdc ) )
						{
#endif
							DrawBackground(g, rcClient);

							if (this.DropDownStyle == ComboBoxStyle.DropDownList)
							{
								DrawForeground(g, (Rectangle)cbi.rcItem);
							}

							if (cbi.stateButton != STATE_SYSTEM_INVISIBLE)
							{
								DrawButton(g, ref cbi);
							}
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
							bg.Render();
#endif
						}
					}
				}

				EndPaint(m.HWnd, ref stPaint);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		private void OnMouseMove(ref Message m)
		{
			if (m_hoverWindow != m.HWnd)
			{
				Invalidate(true);
				m_hoverWindow = m.HWnd;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		private void OnMouseLeave(ref Message m)
		{
			Invalidate(true);
			m_hoverWindow = IntPtr.Zero;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		private void OnGetItemHeight(ref Message m)
		{
			m.Result = (IntPtr)this.ItemHeightInternal;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		private void OnReflectedCommand(ref Message m)
		{
			switch(HIWORD(m.WParam))
			{
				case CBN_CLOSEUP:
					Invalidate();
					break;
			}
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		private void DrawBackground(Graphics g, Rectangle rc)
		{
			Color bkColor = GetIsHighlighted() ? SystemColors.Window : this.BackgroundColor;

			using (Brush brush = new SolidBrush(bkColor))
			{
				g.FillRectangle(brush, rc);
			}

			using (Pen pen = new Pen(this.BorderColor))
			{
				g.DrawRectangle(pen, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="cbi"></param>
		private void DrawForeground(Graphics g, Rectangle rc)
		{
            string sText = null;
            if (SelectedIndex != -1)
            {
                sText = GetItemText(this.Items[SelectedIndex]);
            }
            if (sText != null)
			{
                Rectangle rcText = Rectangle.Inflate(rc, -1, -1);
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				TextFormatFlags tf = this.RightToLeft == RightToLeft.Yes ? TextFormatFlags.NoPadding | TextFormatFlags.Right : TextFormatFlags.NoPadding;
#else
				StringFormat sf = (StringFormat)StringFormat.GenericTypographic.Clone();

				if( this.RightToLeft == RightToLeft.Yes )
				{
					sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
				}
#endif
				if (this.Focused && !this.DroppedDown)
				{
					g.FillRectangle(SystemBrushes.Highlight, rcText);
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
					TextRenderer.DrawText(g, sText, this.Font, rcText, SystemColors.HighlightText, tf);
#else
					g.DrawString( sText, this.Font, SystemBrushes.HighlightText, rcText, sf );
#endif
					ControlPaint.DrawFocusRectangle(g, rc);
				}
				else
				{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
					TextRenderer.DrawText(g, sText, this.Font, rcText, this.ForeColor, tf);
#else
					g.DrawString( sText, this.Font, new SolidBrush(this.ForeColor), rcText, sf );
#endif
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="cbi"></param>
		private void DrawButton(Graphics g, ref COMBOBOXINFO cbi)
		{
			Rectangle rcButton = (Rectangle)cbi.rcButton;

			if (!DrawButtonPressed(g, ref rcButton))
			{
				if (!DrawButtonSelected(g, ref rcButton))
				{
					DrawButtonNormal(g, ref rcButton);
				}
			}
			DrawArrow(g, ref rcButton);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pi"></param>
		/// <param name="rcButton"></param>
		/// <returns></returns>
		private bool DrawButtonPressed(Graphics g, ref Rectangle rc)
		{
			bool bResult = false;

			if (GetIsPressed())
			{
				if (rc.Width > 0 && rc.Height > 0)
				{
					Color cl1 = this.ColorTable.ButtonPressedGradientBegin;
					Color cl2 = this.ColorTable.ButtonPressedGradientEnd;

					DrawButtonBackground(g, rc, cl1, cl2);

					using (Pen p = new Pen(this.ColorTable.ButtonPressedBorder))
					{
						g.DrawRectangle(p, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
					}
					using (Pen p = new Pen(this.HighlightColor))
					{
						g.DrawRectangle(p, rc.X + 1, rc.Y + 1, rc.Width - 3, rc.Height - 3);
					}
				}
				bResult = true;
			}

			return bResult;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pi"></param>
		/// <param name="rcButton"></param>
		/// <returns></returns>
		private bool DrawButtonSelected(Graphics g, ref Rectangle rc)
		{
			bool bResult = false;

			if (GetIsSelected())
			{
				if (rc.Width > 0 && rc.Height > 0)
				{
					Color cl1 = this.ColorTable.ButtonSelectedGradientBegin;
					Color cl2 = this.ColorTable.ButtonSelectedGradientEnd;

					DrawButtonBackground(g, rc, cl1, cl2);

					using (Pen p = new Pen(this.ColorTable.ButtonSelectedBorder))
					{
						g.DrawRectangle(p, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
					}
					using (Pen p = new Pen(this.HighlightColor))
					{
						g.DrawRectangle(p, rc.X + 1, rc.Y + 1, rc.Width - 3, rc.Height - 3);
					}
				}

				bResult = true;
			}

			return bResult;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pi"></param>
		/// <param name="rcButton"></param>
		private void DrawButtonNormal(Graphics g, ref Rectangle rc)
		{
			if (rc.Width > 0 && rc.Height > 0)
			{
				Color cl1 = this.ColorTable.GroupGradientBegin;
				Color cl2 = this.ColorTable.GroupGradientEnd;

				DrawButtonBackground(g, rc, cl1, cl2);

				using (Pen p = new Pen(this.BorderColor))
				{
					g.DrawRectangle(p, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pi"></param>
		/// <param name="rcButton"></param>
		private void DrawArrow(Graphics g, ref Rectangle rc)
		{
			int x = rc.X + rc.Width / 2;
			int y = rc.Y + rc.Height / 2;

			Point[] points = new Point[]
				{
					new Point(x - 2, y - 1), 
					new Point(x + 3, y - 1),
					new Point(x, y + 2)
				};

			Brush brush = this.Enabled ? SystemBrushes.ControlText : SystemBrushes.ControlDark;
			g.FillPolygon(brush, points);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		/// <param name="clBegin"></param>
		/// <param name="clEnd"></param>
		private void DrawButtonBackground(Graphics g, Rectangle rc, Color clBegin, Color clEnd)
		{
			if (rc.Width > 0 && rc.Height > 0)
			{
				Color clMedium = Office12ColorTable.GetAlphaBlendedColor(clBegin, clEnd, 128);

				using (LinearGradientBrush brush = GetVerticalBrush(ref rc, Color.Black, Color.White))
				{
					ColorBlend clb = new ColorBlend();

					clb.Colors = new Color[] { clBegin, clMedium, clEnd, clMedium };
					clb.Positions = new float[] { 0.0F, 0.38F, 0.38F, 1.0F };

					brush.InterpolationColors = clb;

					brush.WrapMode = WrapMode.TileFlipY;
					g.FillRectangle(brush, rc);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		private void DrawItemBackground(DrawItemEventArgs e)
		{
			if ((e.State & DrawItemState.Selected) != 0 && this.IsStyled)
			{
				Rectangle rcItem = e.Bounds;

				Point[] ptBounds = GetRoundedPolygon(rcItem, 1);

				Color c1 = this.ColorTable.ButtonSelectedGradientBegin;
				Color c2 = this.ColorTable.ButtonSelectedGradientEnd;

				using (LinearGradientBrush brush = GetVerticalBrush(ref rcItem, c1, c2))
				{
					brush.Blend = m_blSelection;
					e.Graphics.FillPolygon(brush, ptBounds);
				}
				using (Pen p = new Pen(c2))
				{
					e.Graphics.DrawPolygon(p, ptBounds);
				}
			}
			else
			{
				e.DrawBackground();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected virtual void DrawItemText(DrawItemEventArgs e)
		{
			if (e.Index >= 0 && e.Index < this.Items.Count && this.IsStyled)
			{
				RectangleF rc = e.Bounds;

				string sText = GetItemText(this.Items[e.Index]);
				using (StringFormat sf = new StringFormat(StringFormat.GenericDefault))
				{
					if (this.RightToLeft == RightToLeft.Yes)
					{
						sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
					}
					e.Graphics.DrawString(sText, e.Font, Brushes.Black, rc, sf);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cb"></param>
		/// <returns></returns>
		bool GetIsHighlighted()
		{
			if (this.Enabled)
			{
				return GetIsSelected() || GetIsPressed();
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cb"></param>
		/// <returns></returns>
		private bool GetIsSelected()
		{
			bool bResult = this.Focused;

			if (!bResult)
			{
				Rectangle rc = RectangleToScreen(this.ClientRectangle);
				bResult = rc.Contains(Cursor.Position);
			}

			return bResult;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cb"></param>
		/// <returns></returns>
		private bool GetIsPressed()
		{
			return this.DroppedDown;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rc"></param>
		/// <param name="cl1"></param>
		/// <param name="cl2"></param>
		/// <returns></returns>
		private static LinearGradientBrush GetHorizontalBrush(ref Rectangle rc, Color cl1, Color cl2)
		{
			Rectangle rcBrush = new Rectangle(rc.Left, rc.Top, Math.Max(rc.Width, 1), 1);

			LinearGradientBrush brush = new LinearGradientBrush(rcBrush, cl1, cl2, 0F);
			brush.WrapMode = WrapMode.TileFlipXY;

			return brush;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="rc"></param>
		/// <param name="cl1"></param>
		/// <param name="cl2"></param>
		/// <returns></returns>
		private static LinearGradientBrush GetVerticalBrush(ref Rectangle rc, Color cl1, Color cl2)
		{
			Rectangle rcBrush = new Rectangle(0, rc.Top, 1, Math.Max(rc.Height, 1));

			LinearGradientBrush brush = new LinearGradientBrush(rcBrush, cl1, cl2, 90);
			brush.WrapMode = WrapMode.TileFlipXY;

			return brush;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rc"></param>
		/// <param name="iRadius"></param>
		/// <returns></returns>
		private static Point[] GetRoundedPolygon(Rectangle rc, int iRadius)
		{
			int iLeft = rc.X;
			int iTop = rc.Y;
			int iRight = rc.Right - 1;
			int iBottom = rc.Bottom - 1;

			Point[] points = new Point[]
				{
					new Point(iLeft, iTop+iRadius),
					new Point(iLeft+iRadius, iTop),
					new Point(iRight-iRadius, iTop),
					new Point(iRight, iTop+iRadius),
					new Point(iRight, iBottom-iRadius),
					new Point(iRight-iRadius, iBottom),
					new Point(iLeft+iRadius, iBottom),
					new Point(iLeft, iBottom-iRadius),
				};

			return points;
		}

		/// <summary>
		/// Raises <see cref="IsStyledChanged"/> event.
		/// </summary>
		protected virtual void OnIsStyledChanged()
		{
			if( this.IsStyledChanged != null )
			{
				this.IsStyledChanged( this, EventArgs.Empty );
			}
		}

		#endregion

		#region Fields
		ListBoxWindow m_list;
		IntPtr m_hoverWindow;
		private bool m_bIsStyled = false;
		private bool m_bShowGripper = false;

		static Blend m_blSelection;

		static Bitmap m_measuringBitmap;
		#endregion

		#region Win32

		#region Messages
		const int WM_PAINT = 0x000F;
		const int WM_ERASEBKGND = 0x0014;

		const int WM_WINDOWPOSCHANGING = 0x0046;
		const int WM_WINDOWPOSCHANGED = 0x0047;

		const int WM_NCCALCSIZE = 0x0083;
		const int WM_NCPAINT = 0x0085;

		const int WM_KEYDOWN = 0x0100;

		const int WM_CTLCOLOREDIT = 0x0133;
		const int WM_CTLCOLORSTATIC = 0x0138;

		const int WM_MOUSEMOVE = 0x0200;
		const int WM_LBUTTONDOWN = 0x0201;
		const int WM_LBUTTONUP = 0x0202;
		const int WM_LBUTTONDBLCLK = 0x0203;
		const int WM_MOUSEWHEEL = 0x020A;
		const int WM_MOUSELEAVE = 0x02A3;

		const int WM_CAPTURECHANGED = 0x0215;

		const int WM_PRINT = 0x0317;


		#region ComboBox messages
		const int CB_SETITEMHEIGHT = 0x0153;
		const int CB_GETITEMHEIGHT = 0x0154;
		const int CB_GETEXTENDEDUI = 0x0156;
		#endregion

		#region Listbox messages
		const int LB_GETTOPINDEX = 0x018E;
		const int LB_SETTOPINDEX = 0x0197;
		#endregion

		const int WM_USER = 0x0400;

		const int WM_REFLECTEDCOMMAND = 0x2111;
		#endregion

		#region Constants

		#region GetWindowLong offsets
		const int GWL_STYLE = (-16);
		#endregion

		#region Common window styles
		const int WS_VSCROLL = 0x00200000;
		#endregion

		#region ComboBox styles
		const int CBS_OWNERDRAWFIXED = 0x0010;
		const int CBS_NOINTEGRALHEIGHT = 0x0400;
		#endregion

		#region Scrollbar IDs
		const int SB_VERT = 1;
		#endregion

		#region SCROLLINFO flags
		private const int SIF_RANGE = 0x0001;
		private const int SIF_PAGE = 0x0002;
		private const int SIF_POS = 0x0004;
		private const int SIF_TRACKPOS = 0x0010;
		private const int SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS);
		#endregion

		#region WM_PRINT flags
		private const int PRF_NONCLIENT = 0x00000002;
		#endregion

		#region Background Mode
		private const int TRANSPARENT = 1;
		private const int OPAQUE = 2;
		#endregion

		#region RedrawWindow Flags
		private const int RDW_INVALIDATE = 0x0001;
		private const int RDW_INTERNALPAINT = 0x0002;
		private const int RDW_ERASE = 0x0004;
		private const int RDW_VALIDATE = 0x0008;
		private const int RDW_NOINTERNALPAINT = 0x0010;
		private const int RDW_NOERASE = 0x0020;
		private const int RDW_NOCHILDREN = 0x0040;
		private const int RDW_ALLCHILDREN = 0x0080;
		private const int RDW_UPDATENOW = 0x0100;
		private const int RDW_ERASENOW = 0x0200;
		private const int RDW_FRAME = 0x0400;
		private const int RDW_NOFRAME = 0x0800;
		#endregion

		#region State flags
		const int STATE_SYSTEM_INVISIBLE = 0x00008000;
		#endregion

		#region SetWindowPos Flags
		const int SWP_NOSIZE = 0x0001;
		const int SWP_NOMOVE = 0x0002;
		const int SWP_NOZORDER = 0x0004;
		const int SWP_NOREDRAW = 0x0008;
		const int SWP_NOACTIVATE = 0x0010;
		const int SWP_FRAMECHANGED = 0x0020;
		const int SWP_SHOWWINDOW = 0x0040;
		const int SWP_HIDEWINDOW = 0x0080;
		const int SWP_NOCOPYBITS = 0x0100;
		const int SWP_NOOWNERZORDER = 0x0200;
		const int SWP_NOSENDCHANGING = 0x0400;
		#endregion

		#region Window styles
		private const int WS_DISABLED = 0x08000000;
		private const int WS_POPUP = unchecked((int)0x80000000);

		private const int WS_EX_TOOLWINDOW = 0x00000080;
		private const int WS_EX_LAYERED = 0x00080000;
		#endregion

		#region ComboBox notifications
		const int CBN_CLOSEUP = 8;
		#endregion

		#region DrawText flags
		const int DT_CALCRECT = 0x00000400;
		#endregion
		#endregion

		#region Structs

		#region POINT
		[StructLayout(LayoutKind.Sequential)]
		struct POINT
		{
			public int x;
			public int y;
		}
		#endregion

		#region RECT
		[StructLayout(LayoutKind.Sequential)]
		struct RECT
		{
			public RECT(int l, int t, int r, int b)
			{
				left = l;
				top = t;
				right = r;
				bottom = b;
			}

			public int Width
			{
				get { return this.right - this.left; }
			}
			public int Height
			{
				get { return this.bottom - this.top; }
			}

			public static implicit operator Rectangle(RECT rect)
			{
				return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
			}

			public int left;
			public int top;
			public int right;
			public int bottom;
		}
		#endregion

		#region COMBOBOXINFO
		[StructLayout(LayoutKind.Sequential)]
		struct COMBOBOXINFO
		{
			public uint cbSize;
			public RECT rcItem;
			public RECT rcButton;
			public uint stateButton;
			public IntPtr hwndCombo;
			public IntPtr hwndItem;
			public IntPtr hwndList;
		}
		#endregion

		#region SCROLLINFO
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		struct SCROLLINFO
		{
			public int cbSize;
			public int fMask;
			public int nMin;
			public int nMax;
			public uint nPage;
			public int nPos;
			public int nTrackPos;
		}
		#endregion

		#region PAINTSTRUCT
		[StructLayout(LayoutKind.Sequential)]
		struct PAINTSTRUCT
		{
			public IntPtr hdc;
			public int fErase;
			public Rectangle rcPaint;
			public int fRestore;
			public int fIncUpdate;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public byte[] reserved;
		}
		#endregion

		#region WINDOWPOS
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct WINDOWPOS
		{
			public IntPtr hwnd;
			public IntPtr hwndInsertAfter;
			public int x;
			public int y;
			public int cx;
			public int cy;
			public int flags;
		}
		#endregion

		#endregion

		#region Functions

		private static int LOWORD(IntPtr lParam)
		{
			return (Int16)(lParam.ToInt64() & 0xffff);
		}

		private static int HIWORD(IntPtr lParam)
		{
			return (Int16)(lParam.ToInt64() >> 16);
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int value);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private extern static bool GetWindowRect(IntPtr hWnd, ref RECT rc);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static private extern bool GetComboBoxInfo(IntPtr hWnd, ref COMBOBOXINFO cbi);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr GetWindowDC(IntPtr hWnd);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static private extern bool RedrawWindow(IntPtr hWnd, IntPtr rcUpdate, IntPtr hrgnUpdate, int flags);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		private static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, ref POINT lpPoints, uint cPoints);

		[DllImport("user32", CharSet = CharSet.Auto)]
		private static extern bool GetScrollInfo(IntPtr hWnd, int fnBar, ref SCROLLINFO si);

		[DllImport("uxtheme.dll", EntryPoint = "SetWindowTheme", CharSet = CharSet.Unicode)]
		private static extern int SetWindowTheme(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]string pszSubAppName, [MarshalAs(UnmanagedType.LPWStr)]string pszSubIdList);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr BeginPaint(IntPtr hWnd, ref PAINTSTRUCT ps);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		private static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT ps);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int Width, int Height, int flags);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr SetCursor(IntPtr hCursor);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int DrawText(IntPtr hdc, string lpString, int nCount, ref RECT lpRect, int uFormat);

		[DllImport("gdi32")]
		private static extern int SetBkMode(IntPtr hDC, int mode);

		[DllImport("gdi32.dll")]
		private static extern bool DeleteObject(IntPtr hObject);

		[DllImport("gdi32.dll")]
		private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

		#endregion

		#endregion

		#region *** ListBoxWindow
		/// <summary>
		/// 
		/// </summary>
		class ListBoxWindow : NativeWindow
		{
			#region Constants
			/// <summary>
			/// 
			/// </summary>
			private const int BORDER_WIDTH = 1;
			private const int BORDER_HEIGHT = 1;
			/// <summary>
			/// 
			/// </summary>
			private const int GRIPPER_HEIGHT = 10;
			/// <summary>
			/// 
			/// </summary>
			private const int RDW_UPDATEFRAME = RDW_INVALIDATE | RDW_FRAME;

			/// <summary>
			/// Scroll regions identifiers
			/// </summary>
			private const int SCROLL_NONE = 0;
			private const int SCROLL_UP = 1;
			private const int SCROLL_DOWN = 2;
			private const int SCROLL_THUMB = 3;
			private const int SCROLL_PGUP = 4;
			private const int SCROLL_PGDN = 5;
			private const int SCROLL_GRIP = 6;

			private const int THUMB_MIN_HEIGHT = 13;

			// User messages
			public const int WMU_SHOWSHADOW = WM_USER + 0x1000;

			#endregion

			#region Constructors
			static ListBoxWindow()
			{
				m_blScrollerBackground = new Blend();
				m_blScrollerBackground.Positions = new float[] { 0.0F, 0.35F, 1.0F };
				m_blScrollerBackground.Factors = new float[] { 0.3F, 0.6F, 0.4F };

				m_blScrollButton = new Blend();
				m_blScrollButton.Positions = new float[] { 0.0F, 0.5F, 0.5F, 1.0F };
				m_blScrollButton.Factors = new float[] { 0.0F, 0.2F, 1.0F, 0.4F };

				m_blThumbNormal = new Blend();
				m_blThumbNormal.Positions = new float[] { 0.0F, 0.35F, 0.5F, 0.7F, 1.0F };
				m_blThumbNormal.Factors = new float[] { 0.1F, 0.0F, 0.5F, 1.0F, 0.5F };

				m_blThumbSelected = new Blend();
				m_blThumbSelected.Positions = new float[] { 0.0F, 0.4F, 0.5F, 0.7F, 1.0F };
				m_blThumbSelected.Factors = new float[] { 0.4F, 0.0F, 1.0F, 1.0F, 0.0F };
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="comboBox"></param>
			public ListBoxWindow(ComboBoxEx comboBox)
			{
				bool bDropShadowEnabled = false;

				NativeMethods.SystemParametersInfo(NativeMethods.SPI_GETDROPSHADOW, 0, ref bDropShadowEnabled, 0);

				if (bDropShadowEnabled)
				{
					m_shadowWindow = new ShadowWindow();
				}

				m_comboBox = comboBox;

				COMBOBOXINFO cbInfo = new COMBOBOXINFO();
				cbInfo.cbSize = (uint)Marshal.SizeOf(cbInfo);

				GetComboBoxInfo(comboBox.Handle, ref cbInfo);
				AssignHandle(cbInfo.hwndList);

				m_timer = new Timer();
				m_timer.Tick += new EventHandler(OnTimerTick);

				m_comboBox.IsStyledChanged += new EventHandler( ComboBoxIsStyledChanged );

				OnComboBoxIsStyledChanged();
			}

			#endregion

			#region Properties
			/// <summary>
			/// 
			/// </summary>
			private int TopIndex
			{
				get
				{
					return SendMessage(this.Handle, LB_GETTOPINDEX, 0, 0);
				}
				set
				{
					if (value < 0)
					{
						value = 0;
					}
					SendMessage(this.Handle, LB_SETTOPINDEX, value, 0);
				}
			}
			/// <summary>
			/// 
			/// </summary>
			private int PageSize
			{
				get
				{
					SCROLLINFO si = new SCROLLINFO();

					si.cbSize = Marshal.SizeOf(si);
					si.fMask = SIF_PAGE;

					if (GetScrollInfo(this.Handle, SB_VERT, ref si))
					{
						return (int)si.nPage;
					}
					return 0;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			private int TrackPos
			{
				get
				{
					SCROLLINFO si = new SCROLLINFO();

					si.cbSize = Marshal.SizeOf(si);
					si.fMask = SIF_TRACKPOS;

					if (GetScrollInfo(this.Handle, SB_VERT, ref si))
					{
						return si.nTrackPos;
					}
					return 0;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			private int ScrollWidth
			{
				get
				{
					return SystemInformation.VerticalScrollBarWidth;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			private int ButtonHeight
			{
				get
				{
					return this.ScrollWidth;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			private Color GalleryScrollBarBackground
			{
				get
				{
					Office12ColorTable colorTable = m_comboBox.ColorTable;
					return colorTable.GalleryScrollBarBackground;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			protected int GripperHeight
			{
				get
				{
					return m_comboBox.ShowGripper ? GRIPPER_HEIGHT : BORDER_HEIGHT;
				}
			}

			#endregion

			#region Overrides
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			protected override void WndProc(ref Message m)
			{
				if (m_comboBox.IsStyled)
				{
					switch (m.Msg)
					{
						case WM_WINDOWPOSCHANGING:
							OnWmWindowPosChanging(ref m);
							break;
						case WM_WINDOWPOSCHANGED:
							OnWmWindowPosChanged(ref m);
							break;
						case WM_NCCALCSIZE:
							OnNcCalcSize(ref m);
							return;
						case WM_NCPAINT:
							OnWmNcPaint(ref m);
							return;
						case WM_PRINT:
							OnWmPrint(ref m);
							return;
						case WM_MOUSEMOVE:
							OnWmMouseMove(ref m);
							return;
						case WM_LBUTTONDOWN:
						case WM_LBUTTONDBLCLK:
							OnWmLButtonDown(ref m);
							return;
						case WM_LBUTTONUP:
						case WM_CAPTURECHANGED:
							OnWmLButtonUp(ref m);
							break;
						case WM_MOUSEWHEEL:
							OnWmMouseWheel(ref m);
							return;
						case WM_KEYDOWN:
							OnWmKeyDown(ref m);
							return;
						case WMU_SHOWSHADOW:
							OnWmuShowShadow(ref m);
							return;
					}
				}

				base.WndProc(ref m);
			}

			/// <summary>
			/// 
			/// </summary>
			public override void DestroyHandle()
			{
				base.DestroyHandle();

                if (m_shadowWindow != null)
                {
                    m_shadowWindow.Dispose();
                    m_shadowWindow = null;
                }

                if( m_timer != null )
                {
                    m_timer.Stop();
                    m_timer.Tick -= new EventHandler(OnTimerTick);
                    m_timer.Dispose();
                    m_timer = null;
                }
				m_comboBox.IsStyledChanged -= new EventHandler( ComboBoxIsStyledChanged );
               // m_comboBox = null;
			}

			#endregion

			#region Message handlers
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			private void OnWmWindowPosChanging(ref Message m)
			{
				WINDOWPOS wp = (WINDOWPOS)m.GetLParam(typeof(WINDOWPOS));
				if ((wp.flags & SWP_SHOWWINDOW) == SWP_SHOWWINDOW)
				{
					// Reserving place for gripper, if necessary.
					int totalItems = Math.Max(m_comboBox.Items.Count, (m_comboBox.ShowGripper ? 1 : 0));
					int displayItems = Math.Min(m_comboBox.MaxDropDownItems, totalItems);

					wp.cy = displayItems * m_comboBox.ItemHeight + BORDER_HEIGHT + this.GripperHeight;

					if ((wp.flags & SWP_NOSIZE) == SWP_NOSIZE)
					{
						wp.flags &= ~SWP_NOSIZE;
						wp.cx = m_comboBox.DropDownWidth;
					}

					Marshal.StructureToPtr(wp, m.LParam, false);
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			private void OnWmWindowPosChanged(ref Message m)
			{
				if (m_shadowWindow != null && !m_shadowWindow.IsDisposed)
				{
					WINDOWPOS wp = (WINDOWPOS)m.GetLParam(typeof(WINDOWPOS));

					int flags = wp.flags & (SWP_HIDEWINDOW | SWP_NOSIZE | SWP_NOMOVE);

					if (flags != (SWP_NOSIZE | SWP_NOMOVE))
					{
						int x = 0;
						int y = 0;
						int cx = 0;
						int cy = 0;

						if ((flags & SWP_NOMOVE) == 0)
						{
							x = wp.x;
							y = wp.y;
						}
						if ((flags & SWP_NOSIZE) == 0)
						{
							cx = wp.cx;
							cy = wp.cy;
						}
						SetWindowPos(m_shadowWindow.Handle, this.Handle, x, y, cx, cy, flags | SWP_NOACTIVATE);
					}
					if ((wp.flags & SWP_SHOWWINDOW) != 0 && m_comboBox.IsStyled)
					{
						PostMessage(this.Handle, WMU_SHOWSHADOW, 0, 0);
					}
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			private void OnNcCalcSize(ref Message m)
			{
				Layout((RECT)m.GetLParam(typeof(RECT)));

				Marshal.StructureToPtr(m_rcClient, m.LParam, false);

				m.Result = IntPtr.Zero;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			private void OnWmNcPaint(ref Message m)
			{
				DrawFrame();
				m.Result = IntPtr.Zero;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			private void OnWmPrint(ref Message m)
			{
				base.WndProc(ref m);

				if (((int)m.LParam & PRF_NONCLIENT) != 0)
				{
					DrawFrame(m.WParam);
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			private void OnWmMouseMove(ref Message m)
			{
				m_mousePos = GetMousePos(ref m);

				this.SetSelectedRegion(GetScrollRegion(m_mousePos));
				this.SetScrollSelected(m_rcScroll.Contains(m_mousePos));

				switch (m_regionPressed)
				{
					case SCROLL_THUMB:
						Scroll();
						break;
					case SCROLL_GRIP:
						Resize();
						break;
					default:
						BaseWndProc(ref m);
						UpdateCursor();
						break;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			private void OnWmLButtonDown(ref Message m)
			{
				base.WndProc(ref m);

				SetPressedRegion(GetScrollRegion(m_mousePos));

				m_mousePressedPos = m_mousePos;

				switch (m_regionPressed)
				{
					case SCROLL_THUMB:
						m_startTopIndex = this.TopIndex;
						return;
					case SCROLL_UP:
						this.TopIndex -= 1;
						break;
					case SCROLL_DOWN:
						this.TopIndex += 1;
						break;
					case SCROLL_PGUP:
						this.TopIndex -= this.PageSize;
						break;
					case SCROLL_PGDN:
						this.TopIndex += this.PageSize;
						break;
					case SCROLL_GRIP:
						GetWindowRect(m.HWnd, ref m_startRect);
						return;
					default:
						return;
				}

				m_timer.Interval = 500;
				m_timer.Start();
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			private void OnWmLButtonUp(ref Message m)
			{
				base.WndProc(ref m);

				SetPressedRegion(SCROLL_NONE);
				UpdateCursor();
				m_timer.Stop();
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			private void OnWmMouseWheel(ref Message m)
			{
				BaseWndProc(ref m);
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			private void OnWmKeyDown(ref Message m)
			{
				BaseWndProc(ref m);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			private void OnWmuShowShadow(ref Message m)
			{
				if (m_shadowWindow != null && !m_shadowWindow.IsDisposed)
				{
					SetWindowPos(m_shadowWindow.Handle, this.Handle, 0, 0, 0, 0, SWP_SHOWWINDOW | SWP_NOACTIVATE | SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER);
				}
			}
			#endregion

			#region Event handlers
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			void OnTimerTick(object sender, EventArgs e)
			{
				SetSelectedRegion(GetScrollRegion(m_mousePos));

				if (m_regionPressed == m_regionSelected)
				{
					switch (m_regionPressed)
					{
						case SCROLL_UP:
							this.TopIndex -= 1;
							break;
						case SCROLL_DOWN:
							this.TopIndex += 1;
							break;
						case SCROLL_PGUP:
							this.TopIndex -= this.PageSize;
							break;
						case SCROLL_PGDN:
							this.TopIndex += this.PageSize;
							break;
					}
					DrawFrame();
				}
				m_timer.Interval = 50;
			}

			private void ComboBoxIsStyledChanged( object sender, EventArgs e )
			{
				OnComboBoxIsStyledChanged();
			}

			private void OnComboBoxIsStyledChanged()
			{
				if( OSFeature.Feature.IsPresent( OSFeature.Themes ) && NativeMethods.IsAppThemed() )
				{
					if (m_comboBox.IsStyled)
					{
						SetWindowTheme(this.Handle, " ", " ");
					}
					else
					{
						SetWindowTheme(this.Handle, null, null);
					}
				}
			}
			#endregion

			#region Implementation
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			private void BaseWndProc(ref Message m)
			{
				int trackPos = this.TrackPos;

				base.WndProc(ref m);

				if (trackPos != this.TrackPos)
				{
					UpdateFrame();
				}
			}
			/// <summary>
			/// 
			/// </summary>
			private void Scroll()
			{
				SCROLLINFO si = new SCROLLINFO();

				si.cbSize = Marshal.SizeOf(si);
				si.fMask = SIF_ALL;

				if (GetScrollInfo(this.Handle, SB_VERT, ref si))
				{
					int offset = ((si.nMax - si.nMin) * (m_mousePos.Y - m_mousePressedPos.Y)) / (m_rcDown.Top - m_rcUp.Bottom);

					this.TopIndex = m_startTopIndex + offset;

					DrawScrollBar();
				}
			}

			/// <summary>
			/// 
			/// </summary>
			private void Resize()
			{
				RECT rc = new RECT();
				if (GetWindowRect(this.Handle, ref rc))
				{
					int scrollHeight = this.ButtonHeight * 2 + THUMB_MIN_HEIGHT;
					int itemsHeight = Math.Max(m_comboBox.Items.Count, 1) * m_comboBox.ItemHeight;
					int gripperHeight = this.GripperHeight;
					int maxHeight = itemsHeight + BORDER_HEIGHT + gripperHeight;
					int minHeight = Math.Min(scrollHeight, itemsHeight) + BORDER_HEIGHT + gripperHeight;

					int height = m_startRect.Height + (m_mousePos.Y - m_mousePressedPos.Y);

					if (height > maxHeight)
					{
						height = maxHeight;
					}
					if (height < minHeight)
					{
						height = minHeight;
					}
					if (height != rc.Height)
					{
						SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, m_startRect.Width, height, SWP_NOMOVE | SWP_NOACTIVATE | SWP_NOZORDER);
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="value"></param>
			private void SetScrollSelected(bool value)
			{
				if (m_scrollbarSelected != value)
				{
					m_scrollbarSelected = value;
					UpdateFrame();
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="buttonId"></param>
			private void SetSelectedRegion(int regionId)
			{
				if (m_regionSelected != regionId)
				{
					m_regionSelected = regionId;
					UpdateFrame();
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="buttonId"></param>
			private void SetPressedRegion(int regionId)
			{
				if (m_regionPressed != regionId)
				{
					m_regionPressed = regionId;
					UpdateFrame();
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="rc"></param>
			private void Layout(RECT rc)
			{
				m_rcScroll = Rectangle.Empty;
				m_rcUp = Rectangle.Empty;
				m_rcDown = Rectangle.Empty;

				m_rcClient = new RECT(rc.left + BORDER_WIDTH, rc.top + BORDER_WIDTH, rc.right - BORDER_WIDTH, rc.bottom);
				m_rcBorder = new RECT(0, 0, rc.Width, rc.Height);

				int gripperHeight = this.GripperHeight;

				m_rcClient.bottom -= gripperHeight;
				m_rcGrip = new Rectangle(0, rc.Height - GRIPPER_HEIGHT, rc.Width, gripperHeight - BORDER_HEIGHT);

				int style = GetWindowLong(this.Handle, GWL_STYLE);
				if ((style & WS_VSCROLL) != 0)
				{
					int sbWidth = this.ScrollWidth;
					int buttonHeight = this.ButtonHeight;

					if (m_comboBox.RightToLeft == RightToLeft.Yes)
					{
						m_rcScroll = new Rectangle(BORDER_WIDTH, BORDER_WIDTH, sbWidth, m_rcClient.Height);
						m_rcClient.left += sbWidth;
					}
					else
					{
						m_rcScroll = new Rectangle(rc.Width - sbWidth - BORDER_WIDTH, BORDER_WIDTH, sbWidth, m_rcClient.Height);
						m_rcClient.right -= sbWidth;
					}

					m_rcUp = new Rectangle(m_rcScroll.X, m_rcScroll.Y, sbWidth, buttonHeight);
					m_rcDown = new Rectangle(m_rcScroll.X, m_rcScroll.Bottom - sbWidth, sbWidth, buttonHeight);
				}
			}
			/// <summary>
			/// 
			/// </summary>
			private void UpdateFrame()
			{
				RedrawWindow(this.Handle, IntPtr.Zero, IntPtr.Zero, RDW_UPDATEFRAME);
			}
			/// <summary>
			/// 
			/// </summary>
			private void DrawFrame()
			{
				IntPtr hDc = GetWindowDC(this.Handle);
                try
                {
                    if (hDc != IntPtr.Zero)
                    {
                        DrawFrame(hDc);                        
                    }
                }
                finally 
                {
                    ReleaseDC(this.Handle, hDc);
                }
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="hDc"></param>
			private void DrawFrame(IntPtr hDc)
			{
				if (m_rcBorder.Width > 0 && m_rcBorder.Height > 0)
				{
					using (Graphics g = Graphics.FromHdc(hDc))
					{
						DrawScrollBar(g);
						DrawGrip(g);
						DrawBorder(g);
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			private void DrawScrollBar()
			{
				if (m_rcScroll.Width > 0 && m_rcScroll.Height > 0)
				{
					IntPtr hDc = GetWindowDC(this.Handle);
                    try
                    {
                        if (hDc != IntPtr.Zero)
                        {
                            using (Graphics g = Graphics.FromHdc(hDc))
                            {
                                DrawScrollBar(g);
                            }                            
                        }
                    }
                    finally
                    {
                        ReleaseDC(this.Handle, hDc);
                    }
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="rc"></param>
			private void DrawScrollBar(Graphics g)
			{
				if (m_rcScroll.Width > 0 && m_rcScroll.Height > 0)
				{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
					using (BufferedGraphics buffer = BufferedGraphicsManager.Current.Allocate(g, m_rcScroll))
					{
						Graphics bg = buffer.Graphics;
#else
						Graphics bg = g;
#endif
						DrawScrollBarBackground(bg);

						DrawScrollButton(bg, m_rcUp, SCROLL_UP);
						DrawScrollButton(bg, m_rcDown, SCROLL_DOWN);

						DrawThumb(bg);
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
						buffer.Render();
					}
#endif
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			private void DrawScrollBarBackground(Graphics g)
			{
				Rectangle scrollRect = m_rcScroll;
				using (LinearGradientBrush b = GetHorizontalBrush(ref m_rcScroll, Color.White, this.GalleryScrollBarBackground))
				{
					b.Blend = m_blScrollerBackground;
					g.FillRectangle(b, m_rcScroll);
				}

			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="rc"></param>
			/// <param name="buttonID"></param>
			private void DrawScrollButton(Graphics g, Rectangle rc, int buttonID)
			{
				DrawScrollButtonBackground(g, rc, buttonID);
				DrawScrollButtonImage(g, rc, buttonID);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="rc"></param>
			/// <param name="buttonID"></param>
			private void DrawScrollButtonBackground(Graphics g, Rectangle rc, int buttonID)
			{
				if (m_scrollbarSelected)
				{
					Color c1 = Color.Empty;
					Color c2 = Color.Empty;

					Office12ColorTable colorTable = m_comboBox.ColorTable;
					if (buttonID == m_regionSelected)
					{
						if (buttonID == m_regionPressed)
						{
							c1 = colorTable.StandardScrollButtonPressedGradientBegin;
							c2 = colorTable.StandardScrollButtonPressedGradientEnd;
						}
						else
						{
							c1 = colorTable.StandardScrollButtonSelectedGradientBegin;
							c2 = colorTable.StandardScrollButtonSelectedGradientEnd;
						}
					}
					else
					{
						c1 = colorTable.StandardScrollButtonHighlightedGradientBegin;
						c2 = colorTable.StandardScrollButtonHighlightedGradientEnd;
					}

					using (LinearGradientBrush brush = GetVerticalBrush(ref rc, c1, c2))
					{
						brush.Blend = m_blScrollButton;
						g.FillRectangle(brush, rc);
					}
					using (Pen p = new Pen(Color.FromArgb(100, Color.Black)))
					{
						g.DrawRectangle(p, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="rc"></param>
			/// <param name="buttonID"></param>
			private void DrawScrollButtonImage(Graphics g, Rectangle rc, int buttonID)
			{
				using (Brush b = new SolidBrush(m_comboBox.ColorTable.ScrollButtonLargeArrow))
				{
					using (GraphicsPath path = new GraphicsPath())
					{
						Rectangle rcImage = new Rectangle(rc.Left + rc.Width / 2 - 4, rc.Top + rc.Height / 2 - 1, 9, 5);

						switch (buttonID)
						{
							case SCROLL_UP:
								path.AddLine(rcImage.Left - 1, rcImage.Bottom, rcImage.Left + 4, rcImage.Bottom - 6);
								path.AddLine(rcImage.Left + 4, rcImage.Bottom - 6, rcImage.Left + 9, rcImage.Bottom);
								break;
							case SCROLL_DOWN:
								path.AddLine(rcImage.Left, rcImage.Top, rcImage.Left + 9, rcImage.Top);
								path.AddLine(rcImage.Left + 9, rcImage.Top, rcImage.Left + 4, rcImage.Top + 5);
								break;
						}

						path.CloseFigure();
						g.FillPath(b, path);
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			private void DrawThumb(Graphics g)
			{
				Rectangle rc = GetThumbRect();

				if (rc.Width > 0 && rc.Height > 0)
				{
					SmoothingMode smoothing = g.SmoothingMode;
					g.SmoothingMode = SmoothingMode.AntiAlias;

					DrawThumbBackground(g, rc);
					DrawThumbImage(g, rc);

					g.SmoothingMode = smoothing;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="rc"></param>
			private void DrawThumbBackground(Graphics g, Rectangle rc)
			{
				Color c1 = Color.Empty;
				Color c2 = Color.Empty;
				Color borderColor = Color.Empty;

				Office12ColorTable colorTable = m_comboBox.ColorTable;

				if (m_regionPressed == SCROLL_THUMB)
				{
					c1 = colorTable.ScrollerPressedGradientBegin;
					c2 = colorTable.ScrollerPressedGradientEnd;
					borderColor = colorTable.ScrollerPressedBorder;
				}
				else if (m_regionSelected == SCROLL_THUMB)
				{
					c1 = colorTable.ScrollerSelectedGradientBegin;
					c2 = colorTable.ScrollerSelectedGradientEnd;
					borderColor = colorTable.ScrollerSelectedBorder;
				}
				else
				{
					c1 = colorTable.ScrollerNormalGradientBegin;
					c2 = colorTable.ScrollerNormalGradientEnd;
					borderColor = colorTable.ScrollerNormalBorder;
				}

				using (LinearGradientBrush brush = GetHorizontalBrush(ref rc, c1, c2))
				{
					brush.Blend = (m_regionSelected == SCROLL_THUMB || m_regionPressed == SCROLL_THUMB) ? m_blThumbSelected : m_blThumbNormal;
					g.FillRectangle(brush, rc);
				}

				using (Pen p = new Pen(borderColor))
				{
					g.DrawPolygon(p, GetRoundedPolygon(rc, 1));
				}
				using (Pen p = new Pen(Color.FromArgb(128, Color.White)))
				{
					g.DrawRectangle(p, rc.X + 1, rc.Y + 1, rc.Width - 3, rc.Height - 3);
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="rc"></param>
			private void DrawThumbImage(Graphics g, Rectangle rc)
			{
				Rectangle rcImage = new Rectangle(rc.X + (rc.Width - 8) / 2, rc.Y + (rc.Height - 7) / 2, 8, 8);

				using (Pen pShadow = new Pen(Color.FromArgb(100, Color.Black)))
				{
					using (Pen pHighlight = new Pen(Color.FromArgb(100, Color.White)))
					{
						for (int i = 0, y = rcImage.Top; i < 4; i++)
						{
							g.DrawLine(pShadow, rcImage.Left, y, rcImage.Right, y);
							y++;
							g.DrawLine(pHighlight, rcImage.Left + 2, y, rcImage.Right, y);
							y++;
						}
					}
				}

			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			private void DrawGrip(Graphics g)
			{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				using (BufferedGraphics buffer = BufferedGraphicsManager.Current.Allocate(g, m_rcGrip))
				{
					Graphics bg = buffer.Graphics;
#else
					Graphics bg = g;
#endif
					bg.SmoothingMode = SmoothingMode.AntiAlias;

					using (LinearGradientBrush brush = GetVerticalBrush(ref m_rcGrip, Color.White, this.GalleryScrollBarBackground))
					{
						bg.FillRectangle(brush, m_rcGrip);
					}
					using (Brush brush = new SolidBrush(m_comboBox.ColorTable.ScrollButtonLargeArrow))
					{
						int x = m_rcGrip.Left + (m_rcGrip.Width - 4 * 5) / 2;
						int y = m_rcGrip.Top + (m_rcGrip.Height - 2) / 2;

						for (int i = 0; i < 4; i++)
						{
							bg.FillRectangle(brush, x + i * 5, y, 2, 2);
						}
					}
					using (Pen p = new Pen(m_comboBox.BorderColor))
					{
						bg.DrawLine(p, m_rcGrip.X, m_rcGrip.Y, m_rcGrip.Right, m_rcGrip.Y);
					}
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
					buffer.Render();
				}
#endif
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			private void DrawBorder(Graphics g)
			{
				using (Pen p = new Pen(m_comboBox.BorderColor))
				{
					g.DrawRectangle(p, m_rcBorder.left, m_rcBorder.top, m_rcBorder.Width - 1, m_rcBorder.Height - 1);
				}

			}

			/// <summary>
			/// 
			/// </summary>
			private void UpdateCursor()
			{
				if (m_regionPressed == SCROLL_NONE)
				{
					Cursor cur = m_regionSelected == SCROLL_GRIP ? Cursors.SizeNS : Cursors.Default;
					SetCursor(cur.Handle);
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="pt"></param>
			/// <returns></returns>
			private int GetScrollRegion(Point pt)
			{
				if (m_rcScroll.Width > 0 && m_rcScroll.Height > 0)
				{
					if (m_rcUp.Contains(pt))
					{
						return SCROLL_UP;
					}

					if (m_rcDown.Contains(pt))
					{
						return SCROLL_DOWN;
					}

					if (m_rcScroll.Contains(pt))
					{
						Rectangle thumb = GetThumbRect();

						if (pt.Y < thumb.Top)
						{
							return SCROLL_PGUP;
						}
						if (pt.Y > thumb.Bottom)
						{
							return SCROLL_PGDN;
						}
						return SCROLL_THUMB;
					}
				}

				if (m_rcGrip.Contains(pt))
				{
					return SCROLL_GRIP;
				}

				return SCROLL_NONE;
			}

			/// <summary>
			/// Returns mouse coordinates relative to upper-left corner of the listbox window
			/// </summary>
			/// <param name="?"></param>
			/// <returns></returns>
			private Point GetMousePos(ref Message m)
			{
				RECT rc = new RECT();
				GetWindowRect(m.HWnd, ref rc);

				POINT pt = new POINT();
				pt.x = LOWORD(m.LParam);
				pt.y = HIWORD(m.LParam);

				MapWindowPoints(m.HWnd, IntPtr.Zero, ref pt, 1);

				return new Point(pt.x - rc.left, pt.y - rc.top);
			}

			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			private Rectangle GetThumbRect()
			{
				Rectangle rcThumb = Rectangle.Empty;

				SCROLLINFO si = new SCROLLINFO();

				si.cbSize = Marshal.SizeOf(si);
				si.fMask = SIF_ALL;

				if (GetScrollInfo(this.Handle, SB_VERT, ref si))
				{
					if (si.nMax > si.nMin)
					{
						Rectangle rc = Rectangle.Inflate(m_rcScroll, 0, -this.ButtonHeight);

						int height = Math.Max((int)(rc.Height * si.nPage) / (si.nMax - si.nMin), THUMB_MIN_HEIGHT);

						int topMax = rc.Height - height;
						int posMax = (int)(si.nMax - si.nPage + 1);

						int pos = Math.Min(posMax, si.nTrackPos);
						int top = rc.Top + (pos * topMax) / posMax;

						rcThumb = new Rectangle(rc.X, top, rc.Width, height);
					}
				}

				return rcThumb;
			}

			#endregion

			#region Fields
			ComboBoxEx m_comboBox;
			ShadowWindow m_shadowWindow = null;

			bool m_scrollbarSelected = false;

			int m_regionSelected = SCROLL_NONE;
			int m_regionPressed = SCROLL_NONE;

			//Layout dada
			RECT m_rcClient;
			RECT m_rcBorder;

			Rectangle m_rcUp;
			Rectangle m_rcDown;
			Rectangle m_rcScroll;
			Rectangle m_rcGrip;

			Timer m_timer;

			// Last mouse position
			Point m_mousePos;
			// Last pressed mouse position
			Point m_mousePressedPos;
			/// Sizing initial rectangle
			RECT m_startRect = new RECT();
			/// Scrolling initial top index
			int m_startTopIndex;

			protected static Blend m_blScrollerBackground;
			protected static Blend m_blScrollButton;
			protected static Blend m_blThumbNormal;
			protected static Blend m_blThumbSelected;

			#endregion
		}
		#endregion

		#region *** ShadowWindow
		class ShadowWindow : Control
		{
			#region Constructors
			public ShadowWindow()
			{
				SetStyle(ControlStyles.AllPaintingInWmPaint, false);
			}
			#endregion

			#region Properties
			/// <summary>
			/// 
			/// </summary>
			protected override CreateParams CreateParams
			{
				get
				{
					CreateParams cp = base.CreateParams;

					cp.Style = WS_POPUP | WS_DISABLED;
					cp.ExStyle = WS_EX_TOOLWINDOW;
					cp.ClassStyle = NativeMethods.CS_DROPSHADOW;

					return cp;
				}
			}
			#endregion

			#region Overrides
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			protected override void WndProc(ref Message m)
			{
				switch (m.Msg)
				{
					case WM_ERASEBKGND:
						m.Result = (IntPtr)1;
						return;
				}
				base.WndProc(ref m);
			}
			#endregion
		}
		#endregion
	}
}

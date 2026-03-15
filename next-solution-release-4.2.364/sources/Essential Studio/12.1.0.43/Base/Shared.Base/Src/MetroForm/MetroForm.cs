#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Syncfusion.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Syncfusion.Windows.Forms
{

	public partial class MetroForm : Form
	{
		#region Constants
		const int BORDER_WIDTH = 6;
		const int FRAME_PADDING = 2;
		const int CURVE_WIDTH = 12;
		const int SEPARATOR_WIDTH = 2;
		const int SRCCOPY = 0x00CC0020;

		const int SB_CLOSE = 0x0;
		const int SB_MAXIMIZE = 0x1;
		const int SB_MINIMIZE = 0x2;
		const int SB_RESTORE = 0x3;
		const int SB__MAX = 0x4;

		const int SB_MDICLOSE = 0x10;
		const int SB_MDIMINIMIZE = 0x12;
		const int SB_MDIRESTORE = 0x13;

		const int SB_HELPBUTTON = 0x14;
		const int SB_MDIHELPBUTTON = 0x15;

		const int SWP_NORELOCATE =
			NativeMethods.SWP_NOSIZE |
			NativeMethods.SWP_NOMOVE;

		const int SWP_FRAME =
			NativeMethods.SWP_FRAMECHANGED |
			NativeMethods.SWP_NOACTIVATE |
			NativeMethods.SWP_NOSIZE |
			NativeMethods.SWP_NOMOVE |
			NativeMethods.SWP_NOZORDER;

		const int WMSZ_LEFT = 1;
		const int WMSZ_RIGHT = 2;
		const int WMSZ_TOP = 3;
		const int WMSZ_TOPLEFT = 4;
		const int WMSZ_TOPRIGHT = 5;
		const int WMSZ_BOTTOM = 6;
		const int WMSZ_BOTTOMLEFT = 7;
		const int WMSZ_BOTTOMRIGHT = 8;

		const int DT_CENTER = 0x00000001;
		const int DT_RIGHT = 0x00000002;
		const int DT_VCENTER = 0x00000004;
        const int DT_BOTTOM = 0x00000008;
        const int DT_TOP = 0x00000000;
		const int DT_SINGLELINE = 0x00000020;
		const int DT_END_ELLIPSIS = 0x00008000;
		const int DT_NOPREFIX = 0x00000800;

		const int CS_NOCLOSE = 0x200;
		const int GCL_STYLE = (-26);

		/// <summary>
		///  Normal font weight
		/// </summary>
		const int FW_NORMAL = 400;

		const int TEXT_MARGIN = 4;

		const int WM_DWMCOMPOSITIONCHANGED = 0x031E;

		#endregion

		#region Constructors
		static MetroForm()
		{
			Bitmap systemButtons = new Bitmap(typeof(MetroForm).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Office2007Form.SystemButtons.bmp"));

			m_systemButtons = new ImageList();
			m_systemButtons.ImageSize = new Size(9, 9);
			m_systemButtons.Images.AddStrip(systemButtons);
			m_systemButtons.TransparentColor = Color.Magenta;

			m_bmpHelpButton = new Bitmap(typeof(MetroForm).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.MetroForm.help.png"));
			m_bmpHelpButton.MakeTransparent(Color.Magenta);

			m_systemCommands = new int[]
			{
				NativeMethods.SC_CLOSE,
				NativeMethods.SC_MAXIMIZE,
				NativeMethods.SC_MINIMIZE,
				NativeMethods.SC_RESTORE
			};

			m_iSystemHelpCommand = NativeMethods.SC_CONTEXTHELP;

			m_blTitle = new Blend();
			m_blTitle.Positions = new float[] { 0.0F, 0.27F, 0.27F, 1.0F };
			m_blTitle.Factors = new float[] { 0.0F, 0.2F, 1.0F, 0.0F };

			m_blFrameButton = new Blend();
			m_blFrameButton.Positions = new float[] { 0.0F, 0.5F, 0.5F, 1.0F };
			m_blFrameButton.Factors = new float[] { 0.2F, 0.0F, 1.0F, 0.5F };

			m_blFrameButtonBorder = new Blend();
			m_blFrameButtonBorder.Positions = new float[] { 0.0f, 0.5f, 1.0f };
			m_blFrameButtonBorder.Factors = new float[] { 0.5f, 1.0f, 0.5f };
		}
		/// <summary>
		/// 
		/// </summary>
		public MetroForm()
        {
            captionLabels = new CaptionLabelCollection(this);
            captionImages = new CaptionImageCollection(this);
			m_pBorder = new Pen(Color.FromArgb(64, Color.Black));
			m_pSeparatorDark = new Pen(Color.FromArgb(32, Color.Black));
			m_pSeparatorLight = new Pen(Color.FromArgb(64, Color.White));
			this.BackColor = Color.White;
			this.CaptionAlign = HorizontalAlignment.Center;
            this.IconAlign = HorizontalAlignment.Left;
            this.IconTextRelation = LeftRightAlignment.Left;// default icon align
			m_bCompositionEnabled = GetIsCompositionEnabled();
			this.CaptionForeColor = ColorTranslator.FromHtml("#343434");

			base.AutoScroll = false;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets if to disable Office2007 look and feel.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)
		, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal virtual bool DisableOffice2007Style
		{
			get
			{
				return m_bDisableOffice2007Style || (m_bCompositionEnabled && this.applyAeroTheme);
			}
			set
			{
				if (m_bDisableOffice2007Style != value)
				{
					m_bDisableOffice2007Style = value;

					if (this.IsHandleCreated)
					{
						this.RecreateHandle();
					}
				}
			}
		}

		public override Color BackColor
		{
			get
			{
				 return base.BackColor;
			}
			set
			{
				base.BackColor = value;
				this.Invalidate();
			}
		}

		private bool showMouseOver = false;
        /// <summary>
        /// Gets or Sets Mouse over color for CaptionButtons
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Appearance"),
        System.ComponentModel.Description("Gets or Sets Mouse over color for CaptionButtons.")]
        public bool ShowMouseOver
		{
			get { return showMouseOver; }
			set { showMouseOver = value; }
		}


  


        private Color metroColor = ColorTranslator.FromHtml("#119EDA");
        /// <summary>
        /// Gets or Sets the value for CaptionBarColor BorderColor MetroColor
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Appearance"),
        System.ComponentModel.Description("Gets or Sets the value for CaptionBarColor BorderColor MetroColor.")]
        public Color MetroColor
		{
			get { return metroColor; }
			set {
                if (value != Color.White)
                    metroColor = value;
			this.UpdateFrame();
			}
		}



		private Color captionBarColor = Color.White;
        /// <summary>
        /// Gets or Sets the value for CaptionBarColor
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Appearance"), 
        System.ComponentModel.Description("Gets or Sets the value for CaptionBarColor.")]
        public Color CaptionBarColor
		{
			get { return captionBarColor; }
			set { captionBarColor = value;
			this.UpdateFrame();

			}

		}
        private VerticalAlignment captionVerticalAlignment = VerticalAlignment.Center;
        /// <summary>
        /// Gets/Sets the value for CaptionVerticalAlignment
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Design"),
        System.ComponentModel.Description("Gets/Sets the value for CaptionVerticalAlignment.")]
        public VerticalAlignment CaptionVerticalAlignment
        {
            get
            {
                return captionVerticalAlignment;
            }
            set
            {
                captionVerticalAlignment = value;
                this.UpdateFrame();
            }
        }
		private Color borderColor = ColorTranslator.FromHtml ("#737373");
        /// <summary>
        /// Gets or Sets the value for CaptionBarColor BorderColor
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Appearance"), 
        System.ComponentModel.Description("Gets or Sets the value for CaptionBarColor BorderColor.")]
        public Color BorderColor
		{
			get { return borderColor; }
			set { borderColor = value;
			this.UpdateFrame();

			}

		}

		private Color buttonColor = Color.DarkGray;
		private Color captionButtonColor = Color.DarkGray;
        /// <summary>
        /// Gets/Sets the value for CaptionButtonColor
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Appearance"),
        System.ComponentModel.Description("Gets/Sets the value for CaptionButtonColor.")]
		public Color CaptionButtonColor
		{
			get { return captionButtonColor; }
			set { 
				captionButtonColor = value;
				buttonColor = value;
				this.UpdateFrame();
				}
		}
        
        /// <summary>
        /// Hover Color for CaptionButton
        /// </summary>
        private Color captionButtonHoverColor = Color.DarkGray;
        /// <summary>
        /// Gets/Sets the value for CaptionButtonHoverColor
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Appearance"),
        System.ComponentModel.Description("Gets/Sets the value for CaptionButtonHoverColor.")]
        public Color CaptionButtonHoverColor
        {
            get { return captionButtonHoverColor; }
            set
            {
                captionButtonHoverColor = value;
                this.UpdateFrame();
            }
        }
        private int captionBarHeight = 26;
        /// <summary>
        /// Gets or Sets value for CaptionBarHeight
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Appearance"),
        System.ComponentModel.Description("Gets or Sets value for CaptionBarHeight.")]
        public int CaptionBarHeight
        {
            get { return captionBarHeight; }
            set
            {
                captionBarHeight = value;
                this.UpdateFrame();
            }
        }
        /// <summary>
        /// Gets/Sets touch enabled
        /// </summary>
        private bool touchMode = false;
        /// <summary>
        /// Gets/Sets  touch enabled
        /// </summary>
        [DefaultValue(false)]
        public bool EnableTouchMode
        {
            get
            {
                return touchMode;
            }
            set
            {
                if (value != touchMode)
                {
                    touchMode = value;
                    this.UpdateFrame();
                    if (touchMode)
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }
		private int borderThickness = 1;
		/// <summary>
		/// Gets or Set the valur for BorderThickness
		/// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Appearance"),
        System.ComponentModel.Description("Gets or Set the valur for BorderThickness.")]
        public int BorderThickness
		{
			get { return borderThickness; }
			set { borderThickness = value;
			this.Invalidate();
			this.UpdateFrame();
			}
		}

		/// <summary>
		/// Gets/sets if to Force the OS Aero theme look and feel when Office2007Visual style is enabled.
		/// </summary>
		/// <remarks>
		/// If DisableOffice2007Style is set to true then the ordinary form drawn will still have the aero theme applied
		/// as the default frame is drawn in the base.
		/// </remarks>
		[
		Browsable(false),
		Description("Gets/sets if to Force the Default OS Aero theme look and feel when Office2007Visual style is enabled. This will not work for XP and lower OS versions"),
		DefaultValue(true)
		]
		internal virtual bool ApplyAeroTheme
		{
			get
			{
				return this.applyAeroTheme;
			}
			set
			{
				if (this.applyAeroTheme != value)
				{
					this.applyAeroTheme = value;

					if (this.IsHandleCreated)
					{
						this.RecreateHandle();
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AutoScroll
		{
			get { return base.AutoScroll; }
			set { base.AutoScroll = value; }
		}
		/// <summary>
		/// 
		/// </summary>
		[TypeConverter(typeof(MetroForm.ColorSchemeTypeConverter))]
		internal Office2007Theme ColorScheme
		{
			get
			{
				return m_theme;
			}
			set
			{
				if (m_theme != value)
				{
					m_theme = value;

					OnColorSchemeChanged();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Obsolete("Please, use Office2007Form.ColorScheme = Office2007Theme.Managed")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal bool ColorSchemeIsManaged
		{
			get
			{
				return (m_theme == Office2007Theme.Managed);
			}
			set
			{
				if (!this.ColorSchemeIsManaged)
				{
					this.ColorScheme = Office2007Theme.Managed;
				}
			}
		}

		/// <summary>
		/// Gets or sets the font of the form's title.
		/// </summary>
		/// Don't rename this property. Name is important for CodeDomSerialization order.
		[Category("Appearance"), Description("Gets or sets the font of the form's title.")]
		public Font CaptionFont
		{
			get
			{
				if (m_captionFont == null)
				{
					Font captionFont;
					IntPtr hSysFont = this.SystemCaptionFont;

					if (hSysFont != IntPtr.Zero)
					{
						captionFont = Font.FromHfont(hSysFont);

						NativeMethods.DeleteObject(hSysFont);
					}
					else
					{
						captionFont = Form.DefaultFont;
					}

					return captionFont;
				}

				return m_captionFont;
			}
			set
			{
				if (m_captionFont != value)
				{
					m_captionFont = value;

					UpdateFrame();
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for caption in titlebar
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the color for caption in titlebar.")]
		[DefaultValue(typeof(Color), "Empty")]
		public Color CaptionForeColor
		{
			get
			{
				if (captionForeColor == Color.Empty)
				{
					return this.ColorTable.FormTextColor;
				}
				return captionForeColor;
			}
			set
			{
				if (captionForeColor != value)
				{
					captionForeColor = value;
					UpdateFrame();
				}
			}
		}

		/// <summary>
		/// Gets or sets the alignment of of the form's title. 
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the alignment of of the form's title.")]
		[DefaultValue(typeof(HorizontalAlignment), "Left")]
		public HorizontalAlignment CaptionAlign
		{
			get
			{
				return m_captionAlign;
			}
			set
			{
				if (m_captionAlign != value)
				{
					m_captionAlign = value;
					
					UpdateFrame();
				}
			}
		}
        [Category("Appearance"), Description("Gets or sets the alignment of of the form's icon.")]
        [DefaultValue(typeof(HorizontalAlignment), "Left")]
        public HorizontalAlignment IconAlign// seter and geter for iconalign
        {
            get
            {
                return m_iconAlign;
            }
            set
            {
                if (m_iconAlign != value)
                {
                    m_iconAlign = value;

                    UpdateFrame();
                }
            }
        }
        [Category("Appearance"), Description("Gets or sets the alignment between form's icon and form's text.")]
        [DefaultValue(typeof(LeftRightAlignment), "Left")]
        public LeftRightAlignment IconTextRelation// seter and geter for iconalign
        {
            get
            {
                return m_iconTextRelation;
            }
            set
            {
                if (m_iconTextRelation != value)
                {
                    m_iconTextRelation = value;

                    UpdateFrame();
                }
            }
        }
		private bool dropShawdow = false;
		/// <summary>
		/// Gets or Set Value to Drop Shadow to the form
		/// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Appearance"),
        System.ComponentModel.Description("Gets or Set Value to Drop Shadow to the form.")]
        public bool DropShadow
		{
			get { return dropShawdow; }
			set { dropShawdow = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		protected Office2007Colors ColorTable
		{
			get
			{
				return Office2007Colors.GetColorTable(this.ColorScheme);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected bool IsActive
		{
			get
			{
				if (this.IsMdiChild)
				{
					Form f = this.MdiParent;

					if (f != null)
					{
						return f.ActiveMdiChild == this;
					}
				}
				return m_bActive;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected bool IsRightToLeft
		{
			get
			{
				bool bResult = false;

				if (this.IsHandleCreated)
				{
					int styleEx = NativeMethods.GetWindowLong(this.Handle, NativeMethods.GWL_EXSTYLE);
					bResult = (styleEx & NativeMethods.WS_EX_LAYOUTRTL) != 0;
				}

				return bResult;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private int SelectedButton
		{
			get
			{
				return m_selectedButton;
			}
			set
			{
				if (m_selectedButton != value)
				{
					m_selectedButton = value;

					if (m_pressedButton != SB__MAX && m_pressedButton != value)
					{
						value = SB__MAX;
					}

					if (m_highlightedButton != value)
					{
						m_highlightedButton = value;
						InvalidateFrame();
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private int PressedButton
		{
			get
			{
				return m_pressedButton;
			}
			set
			{
				if (m_pressedButton != value)
				{
					m_pressedButton = value;

					if (value == SB__MAX)
					{
						value = m_selectedButton;
					}

					m_highlightedButton = value;
					InvalidateFrame();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private int HighlightedButton
		{
			get
			{
				return m_highlightedButton;
			}
			set
			{
				if (m_highlightedButton != value)
				{
					m_highlightedButton = value;
					InvalidateFrame();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private int MaximizeButton
		{
			get { return this.WindowState == FormWindowState.Maximized ? SB_RESTORE : SB_MAXIMIZE; }
		}
		/// <summary>
		/// 
		/// </summary>
		private int MinimizeButton
		{
			get { return this.WindowState == FormWindowState.Minimized ? SB_RESTORE : SB_MINIMIZE; }
		}
		/// <summary>
		/// 
		/// </summary>
		private FrameLayoutInfo FrameLayout
		{
			get
			{
				if (m_frameLayout == null)
				{
					m_frameLayout = new FrameLayoutInfo(this);
					m_frameLayout.PerformLayout(this.Width, this.Height);
				}
				return m_frameLayout;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private int CaptionHeight
		{
			get { return this.FrameLayout.CaptionHeight; }
		}
		/// <summary>
		/// 
		/// </summary>
		private int TitleHeight
		{
			get { return this.FrameLayout.TitleHeight; }
		}
		/// <summary>
		/// 
		/// </summary>
		private bool IsMinimized
		{
			get
			{
				if (this.IsHandleCreated)
				{
					int style = NativeMethods.GetWindowLong(this.Handle, NativeMethods.GWL_STYLE);
					return (style & NativeMethods.WS_MINIMIZE) != 0;
				}
				return this.WindowState == FormWindowState.Minimized;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private bool IsMaximized
		{
			get
			{
				if (this.IsHandleCreated)
				{
					int style = NativeMethods.GetWindowLong(this.Handle, NativeMethods.GWL_STYLE);
					return (style & NativeMethods.WS_MAXIMIZE) != 0;
				}
				return this.WindowState == FormWindowState.Maximized;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool IsSizeable
		{
			get
			{
				bool bResult = false;
				if (this.WindowState == FormWindowState.Normal)
				{
					int ws = NativeMethods.GetWindowLong(this.Handle, NativeMethods.GWL_STYLE);

					bResult = ((ws & NativeMethods.WS_THICKFRAME) == NativeMethods.WS_THICKFRAME);
				}
				return bResult;
			}
		}
   
		/// <summary>
		/// 
		/// </summary>
		private Rectangle DesktopRectangle
		{
			get
			{
				Screen scr = Screen.FromHandle(this.Handle);
				if (scr != null)
				{
					return new Rectangle(Screen.PrimaryScreen.WorkingArea.Location, scr.WorkingArea.Size);
				}
				return Rectangle.Empty;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private Rectangle ParentClientRectangle
		{
			get
			{
				Control parent = this.Parent;
				if (parent != null && parent.IsHandleCreated)
				{
					NativeMethods.RECT rc = new NativeMethods.RECT();
					NativeMethods.GetClientRect(parent.Handle, ref rc);

					return new Rectangle(0, 0, rc.Width, rc.Height);
				}
				return Rectangle.Empty;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private bool IsVisible
		{
			get
			{
				bool bResult = false;

				if (this.IsHandleCreated)
				{
					int style = NativeMethods.GetWindowLong(this.Handle, (int)NativeMethods.GWL_STYLE);
					bResult = (style & (int)NativeMethods.WS_VISIBLE) != 0;
				}

				return bResult;
			}
		}

		/// <summary>
		/// Specifies whether current selected Office2007 scheme background color is used to fill form's backround is used.
		/// If false <see cref="Form.Background"/> is used.
		/// </summary>
		[Description("Specifies whether current selected Office2007 scheme background color.")]
		[DefaultValue(false)]
		[Category("Appearance")]
		internal bool UseOffice2007SchemeBackColor
		{
			get
			{
				return m_bUseOffice2007ThemeBackground;
			}
			set
			{
				if (value != m_bUseOffice2007ThemeBackground)
				{
					m_bUseOffice2007ThemeBackground = value;

					Invalidate();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private IntPtr CaptionFontInternal
		{
			get
			{
				if (m_captionFont == null)
				{
					return this.SystemCaptionFont;
				}
				return m_captionFont.ToHfont();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private IntPtr SystemCaptionFont
		{
			get
			{
				IntPtr font = IntPtr.Zero;

				NativeMethods.NONCLIENTMETRICS ncm = new NativeMethods.NONCLIENTMETRICS();
				ncm.cbSize = Marshal.SizeOf(ncm);

				if (NativeMethods.SystemParametersInfo(0x0029/*SPI_GETNONCLIENTMETRICS*/, 0, ref ncm, 0) != 0)
				{
					ncm.lfCaptionFont.lfWeight = FW_NORMAL;
					font = CreateFontIndirect(ref ncm.lfCaptionFont);
				}

				return font;
			}
		}

		private bool CloseBox
		{
			get
			{
				return (0 == (this.CreateParams.ClassStyle & CS_NOCLOSE));
			}
		}

		private bool GetCloseBox(Control control)
		{
			bool bResult = true;

			if (control.IsHandleCreated)
			{
				int style = (int)GetClassLongPtr(control.Handle, GCL_STYLE);

				bResult = (0 == (style & CS_NOCLOSE));
			}

			return bResult;
		}

        /// <summary>
        /// applies the scaling
        /// </summary>
        /// <param name="scaleFactor"></param>
        public void ApplyScaleToControl(float sf)
        {
            this.SuspendLayout();
            foreach (Control ctrl in this.Controls)
            {
                    PropertyInfo fi = ctrl.GetType().GetProperty("EnableTouchMode");
                    if(fi!= null)
                        fi.SetValue(ctrl, this.touchMode, null);
                    Touch(ctrl);
            }
            this.ResumeLayout();
            this.Invalidate();
        }
        /// <summary>
        /// applies the scaling
        /// </summary>
        /// <param name="ctr"></param>
        private void Touch(Control ctr)
        {
            foreach (Control ctrl in ctr.Controls)
            {
                PropertyInfo fi = ctrl.GetType().GetProperty("EnableTouchMode");
                if (fi != null)
                {
                    if(!(ctrl is ButtonAdv))
                        fi.SetValue(ctrl, this.touchMode, null);
                    else
                        if (ctrl.Parent == null || ctrl.Parent is Form || ctrl.Parent is UserControl || ctrl.Parent is Panel)
                            fi.SetValue(ctrl, this.touchMode, null);
                }
                Touch(ctrl);
            }
        }
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				if (DropShadow)
				cp.ClassStyle |= 0x00020000;
				return cp;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="specified"></param>
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			if (!m_bSuppressSizing || this.DisableOffice2007Style)
			{
				base.SetBoundsCore(x, y, width, height, specified);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		protected override void SetClientSizeCore(int x, int y)
		{
			if (this.DisableOffice2007Style)
			{
				base.SetClientSizeCore(x, y);
			}
			else
			{
				int iBorderWidth = FrameLayout.BorderWidth;
				this.Size = new Size(x + 2 * iBorderWidth, y + this.CaptionHeight + iBorderWidth);
				UpdateBounds(this.Left, this.Top, x + 2 * iBorderWidth, y + this.CaptionHeight + iBorderWidth, x, y);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			if (!this.DisableOffice2007Style)
			{
				UpdateRegion();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnStyleChanged(EventArgs e)
		{
			if (!this.DisableOffice2007Style)
			{
				using (CaptionManager cm = new CaptionManager(this, true))
				{
					base.OnStyleChanged(e);
				}
			}
			else base.OnStyleChanged(e);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
			if (!this.DisableOffice2007Style)
			{
				switch (m.Msg)
				{
					case NativeMethods.WM_NCCALCSIZE:
						if (OnWmNcCalcSize(ref m)) return;
						break;
					case NativeMethods.WM_NCPAINT:
						if (OnWmNcPaint(ref m)) return;
						break;
					case NativeMethods.WM_NCHITTEST:
						if (OnWmNcHitTest(ref m)) return;
						break;
					case NativeMethods.WM_NCACTIVATE:
						if (OnWmNcActivate(ref m)) return;
						break;
					case NativeMethods.WM_NCMOUSEMOVE:
						if (OnWmNcMouseMove(ref m)) return;
						break;
					case NativeMethods.WM_NCMOUSELEAVE:
						if (OnWmNcMouseLeave(ref m)) return;
						break;
					case NativeMethods.WM_NCLBUTTONDOWN:
						if (OnWmNcLButtonDown(ref m)) return;
						break;
					case NativeMethods.WM_MOUSEMOVE:
						if (OnWmMouseMove(ref m)) return;
						break;
                    case NativeMethods.WM_NCLBUTTONUP:
                        {
                            Point pt1 = PointToClient(Cursor.Position);

                            foreach (CaptionLabel ctrl in CaptionLabels)
                            {
                                if (new Rectangle(ctrl.Location.X, ctrl.Location.Y, ctrl.Size.Width, ctrl.Size.Height).Contains(pt1.X, this.CaptionBarHeight - (-pt1.Y)))
                                {
                                    ctrl.Mouseup(new Point(pt1.X - ctrl.Location.X , this.CaptionBarHeight - (-pt1.Y) - ctrl.Location.Y));
                                }
                            }
                            foreach (CaptionImage ctrl in CaptionImages)
                            {
                                if (new Rectangle(ctrl.Location.X, ctrl.Location.Y, ctrl.Size.Width, ctrl.Size.Height).Contains(pt1.X, this.CaptionBarHeight - (-pt1.Y)))
                                {
                                    ctrl.Mouseup();
                                }
                            }
                        }
                        break;
					case NativeMethods.WM_LBUTTONUP:
						if (OnWmLButtonUp(ref m)) return;
						break;
					case NativeMethods.WM_CAPTURECHANGED:
						if (OnWmCaptureChanged(ref m)) return;
						break;
					case NativeMethods.WM_SETICON:
						if (OnWmSetIcon(ref m)) return;
						break;
					case NativeMethods.WM_SETTEXT:
						if (OnWmSetText(ref m)) return;
						break;
					case NativeMethods.WM_GETMINMAXINFO:
						if (OnWmGetMinMaxInfo(ref m)) return;
						break;
					case NativeMethods.WM_WINDOWPOSCHANGING:
						if (OnWmWindowPosChanging(ref m)) return;
						break;
					case NativeMethods.WM_WINDOWPOSCHANGED:
						if (OnWmWindowPosChanged(ref m)) return;
						break;
					case NativeMethods.WM_SETCURSOR:
						if (OnWmSetCursor(ref m)) return;
						break;
					case NativeMethods.WM_SYSCOMMAND:
						if (OnWmSysCommand(ref m)) return;
						break;
					case NativeMethods.WM_CONTEXTMENU:
						if (OnWmCotextMenu(ref m)) return;
						break;
					case WM_DWMCOMPOSITIONCHANGED:
						OnCompositionChanged(ref m);
						break;
                    case NativeMethods.WM_NCLBUTTONDBLCLK:
                        OnWmMouseDoubleClick(ref m);
                        break;
				}
			}

			base.WndProc(ref m);
		}
		Point mousePoint = new Point();

		private bool OnWmMouseDoubleClick(ref Message m)
		{
			Point point = new Point(mousePoint.X - this.Bounds.Left, mousePoint.Y - this.Bounds.Top);
			if (IconBounds.Contains(point)) 
			this.Dispose();
			return false;
			
		}

	 
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaintBackground(PaintEventArgs e)
		{
            base.OnPaintBackground(e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged(e);

			InvalidateOnDemand();
		}
#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRightToLeftLayoutChanged(EventArgs e)
		{
			base.OnRightToLeftLayoutChanged(e);

			InvalidateOnDemand();
		}
#endif
		#endregion

		#region Message handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmNcCalcSize(ref Message m)
		{
			NativeMethods.RECT rc = (NativeMethods.RECT)m.GetLParam(typeof(NativeMethods.RECT));

			using (CaptionManager cm = new CaptionManager(this, true))
			{
				base.WndProc(ref m);
			}

			FrameLayoutInfo fl = this.FrameLayout;
			fl.PerformLayout(rc.Width, rc.Height);

			int iBorderWidth = fl.BorderWidth;

			rc.top += fl.CaptionHeight;
			rc.left += iBorderWidth;
			rc.right -= iBorderWidth;
			rc.bottom -= iBorderWidth;

			Marshal.StructureToPtr(rc, m.LParam, true);

			m.Result = IntPtr.Zero;
			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmNcPaint(ref Message m)
		{
			IntPtr hdc = NativeMethods.GetWindowDC(this.Handle);
			if (hdc != IntPtr.Zero)
			{
				NativeMethods.RECT rc = new NativeMethods.RECT();
				NativeMethods.GetWindowRect((int)this.Handle, ref rc);
				NativeMethods.GetActiveWindow();

				if (m.WParam.ToInt32() != 1)
				{
					int regionDataSize = NativeMethods.GetRegionData(m.WParam, 0, IntPtr.Zero);
					if (regionDataSize > 0)
					{
						IntPtr regionData = Marshal.AllocHGlobal(regionDataSize);

						if (NativeMethods.GetRegionData(m.WParam, regionDataSize, regionData) > 0)
						{
							Matrix matrix = new Matrix();

							matrix.Translate(-rc.left, -rc.top, MatrixOrder.Append);

							if (this.IsRightToLeft)
							{
								matrix.Scale(-1F, 1f, MatrixOrder.Append);
								matrix.Translate(rc.Width, 0F, MatrixOrder.Append);
							}

							NativeMethods.XFORM xf = new NativeMethods.XFORM();
							xf.eM11 = matrix.Elements[0];
							xf.eM12 = matrix.Elements[1];
							xf.eM21 = matrix.Elements[2];
							xf.eM22 = matrix.Elements[3];
							xf.eDx = matrix.Elements[4];
							xf.eDy = matrix.Elements[5];

							IntPtr hRgn = NativeMethods.ExtCreateRegion(ref xf, regionDataSize, regionData);
							if (hRgn != IntPtr.Zero)
							{
								NativeMethods.SelectClipRgn(hdc, hRgn);
								NativeMethods.DeleteObject(hRgn);
							}
						}

						Marshal.FreeHGlobal(regionData);
					}
				}

				IntPtr bufferDC = NativeMethods.CreateCompatibleDC(hdc);
				if (bufferDC != IntPtr.Zero)
				{
					IntPtr hBmp = NativeMethods.CreateCompatibleBitmap(hdc, rc.Width, rc.Height);
					if (hBmp != IntPtr.Zero)
					{
						IntPtr oldBmp = NativeMethods.SelectObject(bufferDC, hBmp);

						using (Graphics bufferedGraphics = Graphics.FromHdc(bufferDC))
						{
							DrawFrame(bufferedGraphics, new Rectangle(0, 0, rc.Width, rc.Height));

							int iBorderWidth = FrameLayout.BorderWidth;
							int right = rc.Width - iBorderWidth;
							if (right - iBorderWidth > 0)
							{
								int captionHeight = this.CaptionHeight;
								int bottom = rc.Height - iBorderWidth;
                                if (bottom - captionHeight > 0)
                                {
                                    NativeMethods.ExcludeClipRect(hdc, iBorderWidth, captionHeight, right, bottom);
                                }
							}
							NativeMethods.BitBlt(hdc, 0, 0, rc.Width, rc.Height, bufferDC, 0, 0, SRCCOPY);
						}
						NativeMethods.DeleteObject(hBmp);
					}
					NativeMethods.DeleteDC(bufferDC);
				}
				NativeMethods.ReleaseDC(this.Handle, hdc);
			}

			m.Result = IntPtr.Zero;
			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmNcHitTest(ref Message m)
		{
			bool bResult = false;

			int x = NativeMethods.LOWORD(m.LParam);
			int y = NativeMethods.HIWORD(m.LParam);
			mousePoint = new Point(x, y);
			int hitTest = GetHitTest(x, y);
			if (hitTest != NativeMethods.HTERROR)
			{
				m.Result = (IntPtr)hitTest;
				bResult = true;
			}

			return bResult;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmNcActivate(ref Message m)
		{
			bool bResult = false;

			m_bActive = (m.WParam != IntPtr.Zero);

			if (!m_bActive)
			{
				m_highlightedButton = SB__MAX;
			}

			if (this.IsVisible)
			{
                if (!this.IsMdiContainer)
                    NativeMethods.LockWindowUpdate(this.Handle);
                base.WndProc(ref m);
                NativeMethods.LockWindowUpdate(IntPtr.Zero);
				Message msg = new Message();

				msg.Msg = NativeMethods.WM_NCPAINT;
				msg.HWnd = m.HWnd;
				msg.WParam = (IntPtr)1;
				msg.LParam = (IntPtr)0;

				OnWmNcPaint(ref msg);

				bResult = true;
			}

			return bResult;
		}
        bool isLabelMouseEnter = true;
        bool isImageMouseEnter = true;
        string labelText = string.Empty;
        string imageName = string.Empty;
		/// <summary>
		/// 
		/// </summary>
        /// <param name="m"></param> 
		/// <returns></returns>
		private bool OnWmNcMouseMove(ref Message m)
		{
			this.SelectedButton = GetButtonId(m.LParam);
            Point p = PointToClient(Cursor.Position);
            foreach (CaptionLabel ctrl in CaptionLabels)
            {
                if (new Rectangle(ctrl.Location.X, ctrl.Location.Y, ctrl.Size.Width, ctrl.Size.Height).Contains(p.X, this.CaptionBarHeight - (-p.Y)))
                {
                    ctrl.Mousemove(new Point(p.X - ctrl.Location.X, (this.CaptionBarHeight - (-p.Y)) - ctrl.Location.Y));
                    if (isLabelMouseEnter)
                    {
                        ctrl.Mouseenter(new Point(p.X - ctrl.Location.X, (this.CaptionBarHeight - (-p.Y)) - ctrl.Location.Y));
                        isLabelMouseEnter = false;
                        labelText = ctrl.Text;
                    }
                }
                else
                {
                    if (!isLabelMouseEnter && ctrl.Text == labelText )
                    {
                        ctrl.Mouseleave(new Point(p.X - ctrl.Location.X, (this.CaptionBarHeight - (-p.Y)) - ctrl.Location.Y));
                        isLabelMouseEnter = true;
                    }
                }
            }
            foreach (CaptionImage ctrl in CaptionImages)
            {
                if (new Rectangle(ctrl.Location.X, ctrl.Location.Y, ctrl.Size.Width, ctrl.Size.Height).Contains(p.X, this.CaptionBarHeight - (-p.Y)))
                {
                    ctrl.Mousemove(new Point(p.X - ctrl.Location.X, (this.CaptionBarHeight - (-p.Y)) - ctrl.Location.Y));
                    if (isImageMouseEnter)
                    {
                        ctrl.Mouseenter(new Point(p.X - ctrl.Location.X, (this.CaptionBarHeight - (-p.Y)) - ctrl.Location.Y));
                        isImageMouseEnter = false;
                        imageName = ctrl.Name;
                        this.UpdateFrame();
                    }
                }
                else
                {
                    if (!isImageMouseEnter && ctrl.Name == imageName)
                    {
                        ctrl.Mouseleave(new Point(p.X - ctrl.Location.X, (this.CaptionBarHeight - (-p.Y)) - ctrl.Location.Y));
                        isImageMouseEnter = true;
                        this.UpdateFrame();
                    }
                }
            }
			if (!m_bMouseIsTracked)
			{
				NativeMethods.TRACKMOUSEEVENT tme = new NativeMethods.TRACKMOUSEEVENT();

				tme.cbSize = Marshal.SizeOf(tme);

				tme.hwndTrack = this.Handle;
				tme.dwFlags = NativeMethods.TME_LEAVE | NativeMethods.TME_NONCLIENT;

				NativeMethods.TrackMouseEvent(ref tme);

				m_bMouseIsTracked = true;
			}

			m.Result = IntPtr.Zero;
			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmNcMouseLeave(ref Message m)
		{
			this.SelectedButton = SB__MAX;

			m_bMouseIsTracked = false;

			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
        private bool OnWmNcLButtonDown(ref Message m)
        {
            Point p = PointToClient(Cursor.Position);
            foreach (CaptionLabel ctrl in CaptionLabels)
            {
                if (new Rectangle(ctrl.Location.X, ctrl.Location.Y, ctrl.Size.Width, ctrl.Size.Height).Contains(p.X, this.CaptionBarHeight - (-p.Y)))
                {
                    ctrl.Mousedown( new Point( p.X - ctrl.Location.X , this.CaptionBarHeight - (-p.Y) - ctrl.Location.Y));
                }
            }
            foreach (CaptionImage ctrl in CaptionImages)
            {
                if (new Rectangle(ctrl.Location.X, ctrl.Location.Y, ctrl.Size.Width, ctrl.Size.Height).Contains(p.X, this.CaptionBarHeight - (-p.Y)))
                {
                    ctrl.Mousedown();
                }
            }
          
            int button = GetButtonId(m.LParam);
           
            bool statusCheck = false;
            if (button != SB__MAX)
            {
                if (IsButtonEnabled(button))
                {
                    this.PressedButton = button;
                    this.Capture = true;
                }
                m.Result = IntPtr.Zero;
                statusCheck = true;
            }
            else
            {
                using (CaptionManager cm = new CaptionManager(this, true))
                {
                    statusCheck = false;
                }
            }
            return statusCheck;
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmMouseMove(ref Message m)
		{
			if (this.Capture)
			{
				Point pt = new Point(NativeMethods.LOWORD(m.LParam), NativeMethods.HIWORD(m.LParam));
				this.SelectedButton = GetButtonId(this.PointToScreen(pt));
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmLButtonUp(ref Message m)
		{
			if (this.PressedButton != SB__MAX)
			{
				Point pt = new Point(NativeMethods.LOWORD(m.LParam), NativeMethods.HIWORD(m.LParam));
				int button = GetButtonId(this.PointToScreen(pt));

				if (button == this.PressedButton)
				{
					Form f = (button < SB__MAX || button == SB_HELPBUTTON) ? this : this.ActiveMdiChild;

					if (f != null && f.IsHandleCreated)
					{
						f.BeginInvoke(new SendMessageDelegate(NativeMethods.SendMessage), new object[] { f.Handle, NativeMethods.WM_SYSCOMMAND, GetButtonCommand(button), 0 });
					}
				}
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmCaptureChanged(ref Message m)
		{
			this.PressedButton = SB__MAX;
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmSetIcon(ref Message m)
		{
			BaseWndProc(ref m);

			this.FrameLayout.PerformLayout(this.Width, this.Height);

			InvalidateFrame();

			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmSetText(ref Message m)
		{
			BaseWndProc(ref m);

			InvalidateFrame();

			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmGetMinMaxInfo(ref Message m)
		{
			NativeMethods.MINMAXINFO mmi = (NativeMethods.MINMAXINFO)m.GetLParam(typeof(NativeMethods.MINMAXINFO));

			Rectangle rcMax = GetMaxRectangle();

			int customMaxWidth = this.MaximumSize.Width;
			int customMaxHeight = this.MaximumSize.Height;
			int customMinWidth = this.MinimumSize.Width;
			int customMinHeight = this.MinimumSize.Height;

			mmi.ptMaxPosition.X = rcMax.X;
			mmi.ptMaxPosition.Y = rcMax.Y;

			mmi.ptMaxSize.X = rcMax.Width;
			mmi.ptMaxSize.Y = rcMax.Height;

			if (customMaxWidth > 0)
			{
				if (mmi.ptMaxSize.X > customMaxWidth)
					mmi.ptMaxSize.X = customMaxWidth;
				if (mmi.ptMaxTrackSize.X > customMaxWidth)
					mmi.ptMaxTrackSize.X = customMaxWidth;
			}
			if (customMaxHeight > 0)
			{
				if (mmi.ptMaxSize.Y > customMaxHeight)
					mmi.ptMaxSize.Y = customMaxHeight;
				if (mmi.ptMaxTrackSize.Y > customMaxHeight)
					mmi.ptMaxTrackSize.Y = customMaxHeight;
			}

			FrameLayoutInfo fl = this.FrameLayout;
			int iBorderWidth = fl.BorderWidth;

			int minWidth = fl.CaptionMinWidth + iBorderWidth * 2;
			int minHeight = fl.CaptionHeight + iBorderWidth;

			mmi.ptMinTrackSize.X = (customMinWidth > 0 && customMinWidth > minWidth) ? customMinWidth : minWidth;
			mmi.ptMinTrackSize.Y = (customMinHeight > 0 && customMinHeight > minHeight) ? customMinHeight : minHeight;

			Marshal.StructureToPtr(mmi, m.LParam, false);
			m.Result = IntPtr.Zero;

			return true;
		}
		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmWindowPosChanging(ref Message m)
		{
			base.WndProc(ref m);

			if (!this.TopLevel)
			{
				if (this.IsMaximized)
				{
					NativeMethods.WINDOWPOS wPos = (NativeMethods.WINDOWPOS)m.GetLParam(typeof(NativeMethods.WINDOWPOS));
					if ((wPos.flags & SWP_NORELOCATE) != SWP_NORELOCATE)
					{
						Rectangle rcMax = GetMaxRectangle();

						wPos.x = rcMax.X;
						wPos.y = rcMax.Y;
						wPos.cx = rcMax.Width;
						wPos.cy = rcMax.Height;

						wPos.flags &= ~SWP_NORELOCATE;

						Marshal.StructureToPtr(wPos, m.LParam, false);
					}
				}
			}
			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmWindowPosChanged(ref Message m)
		{
			bool bResult = false;

			NativeMethods.WINDOWPOS wpos = (NativeMethods.WINDOWPOS)m.GetLParam(typeof(NativeMethods.WINDOWPOS));
			if ((wpos.flags & NativeMethods.SWP_NOSIZE) == 0)
			{
				this.SelectedButton = SB__MAX;

				m_bSuppressSizing = true;
				UpdateRegion();
				base.WndProc(ref m);
				Invalidate();

				m_bSuppressSizing = false;

				UpdateRegion();

				bResult = true;
			}

			return bResult;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmSetCursor(ref Message m)
		{
			Cursor c;
			int htArea = NativeMethods.LOWORD(m.LParam);

			switch (htArea)
			{
				case NativeMethods.HTTOP:
				case NativeMethods.HTBOTTOM:
					c = Cursors.SizeNS;
					break;
				case NativeMethods.HTLEFT:
				case NativeMethods.HTRIGHT:
					c = Cursors.SizeWE;
					break;
				case NativeMethods.HTTOPLEFT:
				case NativeMethods.HTBOTTOMRIGHT:
					c = Cursors.SizeNWSE;
					break;
				case NativeMethods.HTTOPRIGHT:
				case NativeMethods.HTBOTTOMLEFT:
					c = Cursors.SizeNESW;
					break;
				default:
					return false;
			}

			NativeMethods.SetCursor(c.Handle);

			m.Result = (IntPtr)1;
			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmSysCommand(ref Message m)
		{
           // Message mine = 0x007b;
         
            switch ((int)m.WParam & 0xfff0)//61458
			{
				case NativeMethods.SC_KEYMENU:
				case NativeMethods.SC_MOUSEMENU:
				case NativeMethods.SC_CONTEXTHELP:
					BaseWndProc(ref m);
					return true;
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmCotextMenu(ref Message m)
		{
			if (m.WParam == m.HWnd)
			{
				Point p = new Point(NativeMethods.LOWORD(m.LParam), NativeMethods.HIWORD(m.LParam));

				if (PointToClient(p).Y < 0)
				{
					IntPtr hMenu = NativeMethods.GetSystemMenu(this.Handle, false);
					if (hMenu != IntPtr.Zero)
					{
						int cmd = (int)TrackPopupMenu(hMenu, 0x0100, p.X, p.Y, 0, m.HWnd, IntPtr.Zero);
						if (cmd != 0)
						{
							NativeMethods.SendMessage(m.HWnd, NativeMethods.WM_SYSCOMMAND, cmd, m.LParam);
						}
					}
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		private void OnCompositionChanged(ref Message m)
		{
			m_bCompositionEnabled = GetIsCompositionEnabled();

			this.Region = null;

			RecreateHandle();
		}

		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private void BaseWndProc(ref Message m)
		{
			using (CaptionManager cm = new CaptionManager(this, true))
			{
				base.WndProc(ref m);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private void UpdateRegion()
		{
			if (this.IsHandleCreated)
			{
				IntPtr hWnd = this.Handle;

				if (this.FormBorderStyle != FormBorderStyle.None)
				{
					NativeMethods.RECT rc = new NativeMethods.RECT();
					NativeMethods.GetWindowRect((int)hWnd, ref rc);

					if (!this.IsMaximized || (this.IsMaximized && this.IsMdiChild))
					{
						IntPtr hRgn = NativeMethods.CreateRectRgn(0, 0, rc.Width + 1, rc.Height + 1);
						if (hRgn != IntPtr.Zero)
						{
							IntPtr hRoundRgn = NativeMethods.CreateRectRgn (0, 0, rc.Width + 1, rc.Height + BORDER_WIDTH + 1);
							if (hRoundRgn != IntPtr.Zero)
							{
								if (NativeMethods.CombineRgn(hRgn, hRgn, hRoundRgn, NativeMethods.RGN_AND) != 0)
								{
									NativeMethods.SetWindowRgn(hWnd, hRgn, true);
									hRgn = IntPtr.Zero;
								}
									NativeMethods.DeleteObject(hRoundRgn);
							}
							NativeMethods.DeleteObject(hRgn);
						}
					}
					else
					{
						Screen scr = Screen.FromHandle(hWnd);
						if (scr != null)
						{
							Rectangle rcWnd = new Rectangle(rc.left, rc.top, rc.Width, rc.Height);
							Rectangle rcScr = Rectangle.Intersect(rcWnd, scr.WorkingArea);

							rcScr.Offset(-rcWnd.X, -rcWnd.Y);

							IntPtr hRgn = NativeMethods.CreateRectRgn(rcScr.X, rcScr.Y, rcScr.Right, rcScr.Bottom);

							if (hRgn != IntPtr.Zero)
							{
								NativeMethods.SetWindowRgn(hWnd, hRgn, true);
							}
						}
					}
				}
				else NativeMethods.SetWindowRgn(hWnd, IntPtr.Zero, true);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private void UpdateFrame()
		{
			m_frameLayout = null;

			if (this.IsHandleCreated)
			{
				NativeMethods.SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, 0, 0, SWP_FRAME);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private GraphicsPath GetFormPath()
		{
			GraphicsPath path = null;

			if (this.WindowState == FormWindowState.Normal)
			{
				NativeMethods.RECT rc = new NativeMethods.RECT();
				NativeMethods.GetWindowRect((int)this.Handle, ref rc);

				path = GetFormPath(rc.Width, rc.Height);
			}
			return path;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private GraphicsPath GetFormPath(int width, int height)
		{
			GraphicsPath path = new GraphicsPath();

			int r = CURVE_WIDTH / 2;
			int d = r * 2;

			path.AddArc(0, 0, d, d, 180f, 90f);
			path.AddLine(r, 0, width - r, 0);
			path.AddArc(width - d - 1, 0, d, d, -89f, 89f);
			path.AddLine(width, r, width, height);
			path.AddLine(width, height, 0, height);

			path.CloseFigure();

			return path;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="rc"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		private Point[] GetRoundedPolygon(Rectangle rc, int radius)
		{
			int left = rc.X;
			int top = rc.Y;
			int right = rc.Right - 1;
			int bottom = rc.Bottom - 1;

			Point[] points = new Point[]
				{
					new Point(left, top+radius),
					new Point(left + radius, top),
					new Point(right - radius, top),
					new Point(right, top + radius),
					new Point(right, bottom - radius),
					new Point(right - radius, bottom),
					new Point(left + radius, bottom),
					new Point(left, bottom - radius),
				};

			return points;
		}
		/// <summary>
		/// 
		/// </summary>
		private void InvalidateFrame()
		{
			if (this.IsHandleCreated)
			{
				NativeMethods.RedrawWindow(this.Handle, IntPtr.Zero, IntPtr.Zero, NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private int GetHitTest(int x, int y)
		{
			NativeMethods.RECT rc = new NativeMethods.RECT();
			NativeMethods.GetWindowRect((int)this.Handle, ref rc);

			if (NativeMethods.PtInRect(ref rc, new NativeMethods.POINT(x, y)))
			{
				// Offset from top/left
				int x1 = x - rc.left;
				int y1 = y - rc.top;

				// Offset from bottom/right
				int x2 = rc.Width - x1;
				int y2 = rc.Height - y1;

				FrameLayoutInfo fl = this.FrameLayout;
				int captionHeight = fl.CaptionHeight;
                int iBorderWidth = 6; //Resize recognizing size

				if (x1 < iBorderWidth || x2 < iBorderWidth || y1 < captionHeight || y2 < iBorderWidth)
				{
					if (this.IsSizeable)
					{
						if (x1 < iBorderWidth)
						{
							if (y1 < captionHeight)
								return NativeMethods.HTTOPLEFT;
							if (y2 < iBorderWidth)
								return NativeMethods.HTBOTTOMLEFT;
							return NativeMethods.HTLEFT;
						}
						if (x2 < iBorderWidth)
						{
							if (y1 < captionHeight)
								return NativeMethods.HTTOPRIGHT;
							if (y2 < iBorderWidth)
								return NativeMethods.HTBOTTOMRIGHT;
							return NativeMethods.HTRIGHT;
						}
						if (y1 < iBorderWidth)
							return NativeMethods.HTTOP;
						if (y2 < iBorderWidth)
							return NativeMethods.HTBOTTOM;
					}
					if (y1 < this.TitleHeight)
					{
						if (fl.IconBox.Contains(x1, y1))
							return NativeMethods.HTSYSMENU;
						if (fl.CloseBox.Contains(x1, y1))
							return NativeMethods.HTCLOSE;
						if (fl.MaximizeBox.Contains(x1, y1))
							return NativeMethods.HTMAXBUTTON;
						if (fl.MinimizeBox.Contains(x1, y1))
							return NativeMethods.HTMINBUTTON;
						if (fl.HelpButton.Contains(x1, y1))
							return NativeMethods.HTHELP;
                        foreach (CaptionLabel ctrl in CaptionLabels)
                        {
                            if (new Rectangle(ctrl.Location.X, ctrl.Location.Y, ctrl.Size.Width, ctrl.Size.Height).Contains(x1, y1))
                            {
                                return 111;
                            }
                        }
                        foreach (CaptionImage ctrl in CaptionImages)
                        {
                            if (new Rectangle(ctrl.Location.X, ctrl.Location.Y, ctrl.Size.Width, ctrl.Size.Height).Contains(x1, y1))
                            {
                                return 121;
                            }
                        }
						return NativeMethods.HTCAPTION;
					}
					return NativeMethods.HTMENU;
				}
			}
			return NativeMethods.HTERROR;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="points"></param>
		/// <returns></returns>
		private int GetButtonId(IntPtr points)
		{
			int x = NativeMethods.LOWORD(points);
			int y = NativeMethods.HIWORD(points);

			return GetButtonId(new Point(x, y));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pt">pt - coordinates of the cursor. The coordinates are relative to the upper-left corner of the screen.</param>
		/// <returns></returns>
		private int GetButtonId(Point pt)
		{
			NativeMethods.RECT rc = new NativeMethods.RECT();
			NativeMethods.GetWindowRect((int)this.Handle, ref rc);

			int x = pt.X - rc.left;
			int y = pt.Y - rc.top;

			FrameLayoutInfo fl = this.FrameLayout;

			if (fl.CloseBox.Contains(x, y))
				return SB_CLOSE;
			if (fl.MaximizeBox.Contains(x, y))
				return this.MaximizeButton;
			if (fl.MinimizeBox.Contains(x, y))
				return this.MinimizeButton;
			if (fl.HelpButton.Contains(x, y))
				return SB_HELPBUTTON;
			if (fl.MdiMaximizeBox.Contains(x, y))
				return SB_MDIRESTORE;
			if (fl.MdiMinimizeBox.Contains(x, y))
				return SB_MDIMINIMIZE;
			if (fl.MdiCloseBox.Contains(x, y))
				return SB_MDICLOSE;
			if (fl.MdiHelpButton.Contains(x, y))
				return SB_MDIHELPBUTTON;

			return SB__MAX;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="button"></param>
		/// <returns></returns>
		private bool IsButtonEnabled(int button)
		{
			bool bResult = true;

			switch (button)
			{
				case SB_MAXIMIZE:
					{
						bResult = this.MaximizeBox;
					}
					break;
				case SB_MINIMIZE:
					{
						bResult = this.MinimizeBox;
					}
					break;
				case SB_CLOSE:
					{
						bResult = this.CloseBox;
					}
					break;
				case SB_MDIMINIMIZE:
					{
						if (this.IsMdiContainer)
						{
							Form f = this.ActiveMdiChild;
							if (f != null)
							{
								bResult = f.MinimizeBox;
							}
						}
					}
					break;
			}
			return bResult;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		private void DrawFrame(Graphics g, Rectangle rc)
		{
			DrawFrameBackground(g, rc);

			DrawFrameCaption(g, rc);
            DrawPicture(g, rc);
            DrawLabel(g, rc);
			if (this.FormBorderStyle != FormBorderStyle.None)
			{
				DrawFrameBorders(g, rc);
			}
		}
        /// <summary>
        /// Gets/Sets the label for form caption
        /// </summary>
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        System.ComponentModel.Description("Gets/Sets the label for form caption.")]
        public CaptionLabelCollection CaptionLabels
        {
            get
            {
                return captionLabels;
            }
        }

        /// <summary>
        /// Gets/Sets the CaptionImage for form caption
        /// </summary>
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        System.ComponentModel.Description("Gets/Sets the CaptionImage for form caption.")]
        public CaptionImageCollection CaptionImages
        {
            get
            {
                return captionImages;
            }
        }
       

        private void DrawPicture(Graphics g, Rectangle rc)
        {
            foreach (CaptionImage fCaptionImage in CaptionImages)
            {
                g.FillRectangle(new SolidBrush(fCaptionImage.BackColor), new Rectangle(fCaptionImage.Location.X, fCaptionImage.Location.Y, fCaptionImage.Size.Width, fCaptionImage.Size.Height));

                if (fCaptionImage.Image != null)
                    g.DrawImage(fCaptionImage.Image, new Rectangle(fCaptionImage.Location.X, fCaptionImage.Location.Y, fCaptionImage.Size.Width, fCaptionImage.Size.Height));

            }
        }
        private void DrawLabel(Graphics g, Rectangle rc)
        {

            foreach (CaptionLabel clabel in CaptionLabels)
            {
                switch (this.RightToLeft)
                {
                    case System.Windows.Forms.RightToLeft.No:
                        labelLocation = new Point(clabel.Location.X, clabel.Location.Y);
                        break;
                    case System.Windows.Forms.RightToLeft.Yes:
                        labelLocation = new Point(this.Width - (clabel.Location.X + clabel.Size.Width), clabel.Location.Y);
                        break;
                }
                g.FillRectangle(new SolidBrush(clabel.BackColor), new Rectangle(labelLocation.X, labelLocation.Y, clabel.Size.Width, clabel.Size.Height));

                if (clabel.Text != string.Empty)
                {
                    using (StringFormat sf = new StringFormat(StringFormat.GenericTypographic))
                    {
                        sf.Trimming = StringTrimming.EllipsisWord;
                        g.DrawString(clabel.Text, clabel.Font, new SolidBrush(clabel.ForeColor), new Rectangle(labelLocation.X, (clabel.Location.Y + ((clabel.Size.Height) / 2)) - (TextRenderer.MeasureText(clabel.Text, clabel.Font).Height / 2), clabel.Size.Width, clabel.Size.Height), sf);
                    }
                }
            }
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		private void DrawFrameBackground(Graphics g, Rectangle rc)
		{
			Office2007Colors colorTbl = this.ColorTable;

			// Paint Title background depending on form state.
			Color clBegin = this.IsActive ? colorTbl.ActiveTitleGradientBegin : colorTbl.InactiveTitleGradientBegin;
			Color clEnd = this.IsActive ? colorTbl.ActiveTitleGradientEnd : colorTbl.InactiveTitleGradientEnd;
			if (this.Text == string.Empty && !this.ControlBox)
				clBegin = clEnd = this.IsActive ? colorTbl.ActiveFormBorderColor : colorTbl.InactiveFormBorderColor;

			// Fix of RightToLeft painting problem
			int x = rc.Left - 1;
			int width = rc.Width + 1;

			int titleHeight = this.TitleHeight;
			if (titleHeight > 0)
			{
				SolidBrush brush = new SolidBrush(CaptionBarColor);
				g.FillRectangle(brush, new Rectangle(x, rc.Top, width, titleHeight));
				brush.Dispose();
			   
				
			}

			// Paint border depending on form state.
			Brush MetroBrush = new SolidBrush(this.BackColor);
			g.FillRectangle(MetroBrush, new Rectangle(x, rc.Top + titleHeight, width, rc.Height - titleHeight));
            MetroBrush.Dispose();
		
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		private void DrawFrameCaption(Graphics g, Rectangle rc)
		{
			FrameLayoutInfo fl = this.FrameLayout;

			

			DrawFrameButton(g, fl.CloseBox, SB_CLOSE, this.CloseBox);
			DrawFrameButton(g, fl.MaximizeBox, this.MaximizeButton, this.MaximizeBox);
			DrawFrameButton(g, fl.MinimizeBox, this.MinimizeButton, this.MinimizeBox);
			DrawFrameButton(g, fl.HelpButton, SB_HELPBUTTON, this.HelpButton);

			if (this.IsMdiContainer)
			{
				Form f = this.ActiveMdiChild;
				if (f != null && f.WindowState == FormWindowState.Maximized)
				{
					DrawFrameIcon(g, fl.MdiIconBox);

					DrawFrameButton(g, fl.MdiCloseBox, SB_MDICLOSE, GetCloseBox(f));
					DrawFrameButton(g, fl.MdiMaximizeBox, SB_MDIRESTORE, f.MaximizeBox);
					DrawFrameButton(g, fl.MdiMinimizeBox, SB_MDIMINIMIZE, f.MinimizeBox);
					DrawFrameButton(g, fl.MdiHelpButton, SB_MDIHELPBUTTON, f.HelpButton);
				}
			}

			DrawFrameText(g, fl.TextBox);
			DrawFrameIcon(g, fl.IconBox, fl.TextBox.Right);
		}
		private Rectangle iconBounds = new Rectangle();
		internal Rectangle IconBounds
		{
			get { return iconBounds; }
			set { iconBounds = value; }
		}
		private void DrawFrameIcon(Graphics g, Rectangle rc)
		{
			DrawFrameIcon(g, rc, 0);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		private void DrawFrameIcon(Graphics g, Rectangle rc, int left)
		{
            Size IconSize = new System.Drawing.Size(16, 16);
            int adjsut = 0;
            if (g.DpiX > 120)
            {
                IconSize = new System.Drawing.Size(20, 20);
                adjsut = 4;
            }
            else if (g.DpiX > 96)
            {
                IconSize = new System.Drawing.Size(18, 18);
                adjsut = 2;
            }
            FrameLayoutInfo fl = this.FrameLayout;
            if (rc.Width > 0 && rc.Height > 0)
			{
				if (this.Icon != null)
				{
					Icon smallIcon = new Icon(this.Icon, rc.Size);
                    Rectangle rect = rc;
                    if (smallIcon != null)
                    {
                        SolidBrush brush = new SolidBrush(MetroColor);
                        int x = (int)g.MeasureString(this.Text, this.Font).Width;
                        if (this.IsRightToLeft)
                        {
                            rc.X = this.Width - rc.Width;
                        }
                        if (IconTextRelation == LeftRightAlignment.Left && IconAlign == CaptionAlign)
                        {
                            if (IconAlign == HorizontalAlignment.Right)
                            {
                                if (fl.TextBox.Right - (x + 45) <= 0)
                                {
                                    rect = new Rectangle(0, 0, 24, this.TitleHeight);
                                    g.FillRectangle(brush, rect);
                                    switch (this.CaptionVerticalAlignment)
                                    {
                                        case VerticalAlignment.Top:
                                            switch (this.WindowState)
                                            {
                                                case FormWindowState.Normal:
                                                    rect = new Rectangle(5, 3, IconSize.Width, IconSize.Height);
                                                    break;
                                                case FormWindowState.Maximized:
                                                    rect = new Rectangle(5, 9, IconSize.Width, IconSize.Height);
                                                    break;
                                            }
                                            break;
                                        case VerticalAlignment.Center:
                                            rect = new Rectangle(5, rc.Y, IconSize.Width, IconSize.Height);
                                            break;
                                        case VerticalAlignment.Bottom:
                                            rect = new Rectangle(5, (this.CaptionBarHeight - this.IconBounds.Height) - 3, IconSize.Width, IconSize.Height);
                                            break;
                                    }
                                }
                                else
                                {
                                    rect = new Rectangle(fl.TextBox.Right - (x + 45), 0, 24, this.TitleHeight);
                                    if (RightToLeftLayout && RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                                        rect = new Rectangle(this.Width - (fl.TextBox.Right - (x + 45)) - 26, 0, 24, this.TitleHeight);
                                    g.FillRectangle(brush, rect);
                                    switch (this.CaptionVerticalAlignment)
                                    {
                                        case VerticalAlignment.Top:
                                            switch (this.WindowState)
                                            {
                                                case FormWindowState.Normal:
                                                    rect = new Rectangle(fl.TextBox.Right - (x + 40), 3, IconSize.Width, IconSize.Height);
                                                    break;
                                                case FormWindowState.Maximized:
                                                    rect = new Rectangle(fl.TextBox.Right - (x + 40), 9, IconSize.Width, IconSize.Height);
                                                    break;
                                            }
                                            break;
                                        case VerticalAlignment.Center:
                                            rect = new Rectangle(fl.TextBox.Right - (x + 40), rc.Y, IconSize.Width, IconSize.Height);
                                            break;
                                        case VerticalAlignment.Bottom:
                                            rect = new Rectangle(fl.TextBox.Right - (x + 40), (this.CaptionBarHeight - this.IconBounds.Height) - 3, IconSize.Width, IconSize.Height);
                                            break;
                                    }
                                }
                            }
                            else if (IconAlign == HorizontalAlignment.Center)
                            {

                                if ((this.Width / 2) - (x - 10) <= 0)
                                {
                                    rect = new Rectangle(0, 0, 24, this.TitleHeight);
                                    g.FillRectangle(brush, rect);
                                    switch (this.CaptionVerticalAlignment)
                                    {
                                        case VerticalAlignment.Top:
                                            switch (this.WindowState)
                                            {
                                                case FormWindowState.Normal:
                                                    rect = new Rectangle(5, 3, IconSize.Width, IconSize.Height);
                                                    break;
                                                case FormWindowState.Maximized:
                                                    rect = new Rectangle(5, 9, IconSize.Width, IconSize.Height);
                                                    break;
                                            }
                                            break;
                                        case VerticalAlignment.Center:
                                            rect = new Rectangle(5, rc.Y, IconSize.Width, IconSize.Height);
                                            break;
                                        case VerticalAlignment.Bottom:
                                            rect = new Rectangle(5, (this.CaptionBarHeight - this.IconBounds.Height) - 3, IconSize.Width, IconSize.Height);
                                            break;
                                    }
                                }
                                else
                                {
                                    rect = new Rectangle((this.Width / 2) - (x - 10), 0, 24, this.TitleHeight);
                                    if (this.RightToLeftLayout && this.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                                    {
                                        rect = new Rectangle(this.Width - (((this.Width / 2) - (x - 10)) + 26), 0, 24, this.TitleHeight);
                                    }
                                    g.FillRectangle(brush, rect);
                                    switch (this.CaptionVerticalAlignment)
                                    {
                                        case VerticalAlignment.Top:
                                            switch (this.WindowState)
                                            {
                                                case FormWindowState.Normal:
                                                    rect = new Rectangle((this.Width / 2) - (x - 15), 3, IconSize.Width, IconSize.Height);
                                                    break;
                                                case FormWindowState.Maximized:
                                                    rect = new Rectangle((this.Width / 2) - (x - 15), 9, IconSize.Width, IconSize.Height);
                                                    break;
                                            }
                                            break;
                                        case VerticalAlignment.Center:
                                            rect = new Rectangle((this.Width / 2) - (x - 15), rc.Y, IconSize.Width, IconSize.Height);
                                            break;
                                        case VerticalAlignment.Bottom:
                                            rect = new Rectangle((this.Width / 2) - (x - 15), (this.CaptionBarHeight - this.IconBounds.Height) - 3, IconSize.Width, IconSize.Height);
                                            break;
                                    }
                                }
                            }
                            else if (IconAlign == HorizontalAlignment.Left)
                            {
                                if (this.WindowState == FormWindowState.Maximized)
                                {
                                    rect = new Rectangle(5, 0, 24, this.TitleHeight);
                                    g.FillRectangle(brush, rect);
                                    switch (this.CaptionVerticalAlignment)
                                    {
                                        case VerticalAlignment.Top:
                                            switch (this.WindowState)
                                            {
                                                case FormWindowState.Normal:
                                                    rect = new Rectangle(12, 3, IconSize.Width, IconSize.Height);
                                                    break;
                                                case FormWindowState.Maximized:
                                                    rect = new Rectangle(12, 9, IconSize.Width, IconSize.Height);
                                                    break;
                                            }
                                            break;
                                        case VerticalAlignment.Center:
                                            rect = new Rectangle(12, rc.Y, IconSize.Width, IconSize.Height);
                                            break;
                                        case VerticalAlignment.Bottom:
                                            rect = new Rectangle(12, (this.CaptionBarHeight - this.IconBounds.Height) - 3, IconSize.Width, IconSize.Height);
                                            break;
                                    }
                                }
                                else
                                {
                                    rect = new Rectangle(0, 0, 24, this.TitleHeight);
                                    switch (this.RightToLeftLayout)
                                    {
                                        case true:
                                            rect = new Rectangle(this.Width - 24, 0, 24, this.TitleHeight);
                                            break;
                                        case false:
                                            rect = new Rectangle(0, 0, 24, this.TitleHeight);
                                            break;
                                    }
                                    g.FillRectangle(brush, rect);
                                    switch (this.CaptionVerticalAlignment)
                                    {
                                        case VerticalAlignment.Top:
                                            switch (this.WindowState)
                                            {
                                                case FormWindowState.Normal:
                                                    rect = new Rectangle(5, 3, IconSize.Width, IconSize.Height);
                                                    break;
                                                case FormWindowState.Maximized:
                                                    rect = new Rectangle(5, 9, IconSize.Width, IconSize.Height);
                                                    break;
                                            }
                                            break;
                                        case VerticalAlignment.Center:
                                            rect = new Rectangle(5, rc.Y, IconSize.Width, 16);
                                            break;
                                        case VerticalAlignment.Bottom:
                                            rect = new Rectangle(5, (this.CaptionBarHeight - this.IconBounds.Height) - 3, IconSize.Width, IconSize.Height);
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (IconAlign == HorizontalAlignment.Right)
                            {
                                if (this.RightToLeftLayout && this.RightToLeft == RightToLeft.Yes)
                                {
                                    rect = new Rectangle(70, 0, 24, this.TitleHeight);
                                    g.FillRectangle(brush, rect);
                                    switch (this.CaptionVerticalAlignment)
                                    {
                                        case VerticalAlignment.Top:
                                            switch (this.WindowState)
                                            {
                                                case FormWindowState.Normal:
                                                    rect = new Rectangle(left - 20, 3, IconSize.Width, IconSize.Height);
                                                    break;
                                                case FormWindowState.Maximized:
                                                    rect = new Rectangle(left - 20, 9, IconSize.Width, IconSize.Height);
                                                    break;
                                            }
                                            break;
                                        case VerticalAlignment.Center:
                                            rect = new Rectangle(left - 20, rc.Y, IconSize.Width, IconSize.Height);
                                            break;
                                        case VerticalAlignment.Bottom:
                                            rect = new Rectangle(left - 20, (this.CaptionBarHeight - this.IconBounds.Height) - 3, IconSize.Width, IconSize.Height);
                                            break;
                                    }
                                }
                                else
                                {
                                    rect = new Rectangle(left - 24, 0, 24, this.TitleHeight);
                                    g.FillRectangle(brush, rect);
                                    switch (this.CaptionVerticalAlignment)
                                    {
                                        case VerticalAlignment.Top:
                                            switch (this.WindowState)
                                            {
                                                case FormWindowState.Normal:
                                                    rect = new Rectangle(left - 20, 3, IconSize.Width, IconSize.Height);
                                                    break;
                                                case FormWindowState.Maximized:
                                                    rect = new Rectangle(left - 20, 9, IconSize.Width, IconSize.Height);
                                                    break;
                                            }
                                            break;
                                        case VerticalAlignment.Center:
                                            rect = new Rectangle(left - 20, rc.Y, IconSize.Width, IconSize.Height);
                                            break;
                                        case VerticalAlignment.Bottom:
                                            rect = new Rectangle(left - 20, (this.CaptionBarHeight - this.IconBounds.Height) - 3, IconSize.Width, IconSize.Height);
                                            break;
                                    }
                                }

                            }
                            else if (IconAlign == HorizontalAlignment.Center)
                            {
                                if ((this.Width / 2) + (x - 45) >= left - 24)
                                {
                                    rect = new Rectangle(left - 24, 0, 24, this.TitleHeight);
                                    g.FillRectangle(brush, rect);
                                    switch (this.CaptionVerticalAlignment)
                                    {
                                        case VerticalAlignment.Top:
                                            switch (this.WindowState)
                                            {
                                                case FormWindowState.Normal:
                                                    rect = new Rectangle(left - 20, 3, IconSize.Width, IconSize.Height);
                                                    break;
                                                case FormWindowState.Maximized:
                                                    rect = new Rectangle(left - 20, 9, IconSize.Width, IconSize.Height);
                                                    break;
                                            }
                                            break;
                                        case VerticalAlignment.Center:
                                            rect = new Rectangle(left - 20, rc.Y, IconSize.Width, IconSize.Height);
                                            break;
                                        case VerticalAlignment.Bottom:
                                            rect = new Rectangle(left - 20, (this.CaptionBarHeight - this.IconBounds.Height) - 3, IconSize.Width, IconSize.Height);
                                            break;
                                    }
                                }
                                else
                                {
                                    rect = new Rectangle((this.Width / 2) + (x - 25), 0, 24, this.TitleHeight);
                                    if (RightToLeft == System.Windows.Forms.RightToLeft.Yes && RightToLeftLayout)
                                        rect = new Rectangle(this.Width - ((this.Width / 2) + (x - 25) + 26), 0, 24, this.TitleHeight);
                                    g.FillRectangle(brush, rect);
                                    switch (this.CaptionVerticalAlignment)
                                    {
                                        case VerticalAlignment.Top:
                                            switch (this.WindowState)
                                            {
                                                case FormWindowState.Normal:
                                                    rect = new Rectangle((this.Width / 2) + (x - 20), 3, IconSize.Width, IconSize.Height);
                                                    break;
                                                case FormWindowState.Maximized:
                                                    rect = new Rectangle((this.Width / 2) + (x - 20), 9, IconSize.Width, IconSize.Height);
                                                    break;
                                            }
                                            break;
                                        case VerticalAlignment.Center:
                                            rect = new Rectangle((this.Width / 2) + (x - 20), rc.Y, IconSize.Width, IconSize.Height);
                                            break;
                                        case VerticalAlignment.Bottom:
                                            rect = new Rectangle((this.Width / 2) + (x - 20), (this.CaptionBarHeight - this.IconBounds.Height) - 3, IconSize.Width, IconSize.Height);
                                            break;
                                    }
                                }
                            }
                            else if (IconAlign == HorizontalAlignment.Left && CaptionAlign == IconAlign)
                            {
                                if (fl.TextBox.Left + (x) >= left - 24)
                                {
                                    rect = new Rectangle(left - 24, 0, 24, this.TitleHeight);
                                    g.FillRectangle(brush, rect);
                                    rect = new Rectangle(left - 20, rc.Y, IconSize.Width, IconSize.Height);

                                }
                                else
                                {
                                    rect = new Rectangle(fl.TextBox.Left + (x), 0, 24, this.TitleHeight);
                                    if (RightToLeft == System.Windows.Forms.RightToLeft.Yes && RightToLeftLayout)
                                        rect = new Rectangle((this.Width - (fl.TextBox.Left + (x)) - 45), 0, 24, this.TitleHeight);
                                    g.FillRectangle(brush, rect);
                                    switch (this.CaptionVerticalAlignment)
                                    {
                                        case VerticalAlignment.Top:
                                            switch (this.WindowState)
                                            {
                                                case FormWindowState.Normal:
                                                    rect = new Rectangle(fl.TextBox.Left + (x + 5), 3, IconSize.Width, IconSize.Height);
                                                    break;
                                                case FormWindowState.Maximized:
                                                    rect = new Rectangle(fl.TextBox.Left + (x + 5), 9, IconSize.Width, IconSize.Height);
                                                    break;
                                            }
                                            break;
                                        case VerticalAlignment.Center:
                                            rect = new Rectangle(fl.TextBox.Left + (x + 5), rc.Y, IconSize.Width, IconSize.Height);
                                            break;
                                        case VerticalAlignment.Bottom:
                                            rect = new Rectangle(fl.TextBox.Left + (x + 5), (this.CaptionBarHeight - this.IconBounds.Height) - 3, IconSize.Width, IconSize.Height);
                                            break;
                                    }
                                    switch (this.RightToLeftLayout)
                                    {
                                        case true:
                                            rect = new Rectangle(fl.TextBox.Left + (x + 24), (this.CaptionBarHeight - this.IconBounds.Height) - 3, IconSize.Width, IconSize.Height);
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                if (this.WindowState == FormWindowState.Maximized)
                                {
                                    rect = new Rectangle(5, 0, 24, this.TitleHeight);
                                    switch (this.RightToLeftLayout && this.RightToLeft == RightToLeft.Yes)
                                    {
                                        case true:
                                            rect = new Rectangle(this.Width - 31, 0, 24, this.TitleHeight);
                                            break;
                                        case false:
                                            rect = new Rectangle(5, 0, 24, this.TitleHeight);
                                            break;
                                    }
                                    g.FillRectangle(brush, rect);
                                    switch (this.CaptionVerticalAlignment)
                                    {
                                        case VerticalAlignment.Top:
                                            switch (this.WindowState)
                                            {
                                                case FormWindowState.Normal:
                                                    rect = new Rectangle(12, 3, IconSize.Width, IconSize.Height);
                                                    break;
                                                case FormWindowState.Maximized:
                                                    rect = new Rectangle(12, 9, IconSize.Width, IconSize.Height);
                                                    break;
                                            }
                                            break;
                                        case VerticalAlignment.Center:
                                            rect = new Rectangle(12, rc.Y, IconSize.Width, IconSize.Height);
                                            break;
                                        case VerticalAlignment.Bottom:
                                            rect = new Rectangle(12, (this.CaptionBarHeight - this.IconBounds.Height) - 3, IconSize.Width, IconSize.Height);
                                            break;
                                    }
                                }
                                else
                                {
                                    rect = new Rectangle(0, 0, 24, this.TitleHeight);
                                    switch (this.RightToLeftLayout && this.RightToLeft == RightToLeft.Yes)
                                    {
                                        case true:
                                            rect = new Rectangle(this.Width - 24, 0, 24, this.TitleHeight);
                                            break;
                                        case false:
                                            rect = new Rectangle(0, 0, 24, this.TitleHeight);
                                            break;
                                    }
                                    g.FillRectangle(brush, rect);
                                    switch (this.CaptionVerticalAlignment)
                                    {
                                        case VerticalAlignment.Top:
                                            switch (this.WindowState)
                                            {
                                                case FormWindowState.Normal:
                                                    rect = new Rectangle(5, 3, IconSize.Width, IconSize.Height);
                                                    break;
                                                case FormWindowState.Maximized:
                                                    rect = new Rectangle(5, 9, IconSize.Width, IconSize.Height);
                                                    break;
                                            }
                                            break;
                                        case VerticalAlignment.Center:
                                            rect = new Rectangle(5, rc.Y, IconSize.Width, IconSize.Height);
                                            break;
                                        case VerticalAlignment.Bottom:
                                            rect = new Rectangle(5, (this.CaptionBarHeight - this.IconBounds.Height) - 3, IconSize.Width, IconSize.Height);
                                            break;
                                    }
                                }
                            }
                        }
                        if (g.DpiX > 120)
                        {
                            this.IconBounds = new Rectangle(rect.X - adjsut + 1, rc.Y - adjsut, IconSize.Width + adjsut, IconSize.Height + adjsut);
                            g.DrawIcon(smallIcon, new Rectangle(rect.X - adjsut + 1, rc.Y - adjsut, IconSize.Width + adjsut, IconSize.Height + adjsut));
                        }
                        else if (g.DpiX > 96)
                        {
                            this.IconBounds = new Rectangle(rect.X - adjsut - 1, rc.Y - adjsut, IconSize.Width + adjsut, IconSize.Height + adjsut);
                            g.DrawIcon(smallIcon, new Rectangle(rect.X - adjsut - 1, rc.Y - adjsut, IconSize.Width + adjsut, IconSize.Height + adjsut));
                        }
                        else
                        {
                            g.DrawIcon(smallIcon, new Rectangle(rect.X, rc.Y, IconSize.Width, IconSize.Height));
                            this.IconBounds = new Rectangle(rect.X, rc.Y, IconSize.Width, IconSize.Height);
                        }
                        brush.Dispose();
                    }
				}
			}
		}
        /// <summary>
        /// Disabling the controlbox highlights
        /// </summary>
        public void HideControlboxHighlights()
        {
            isDisablingHighlights = true;
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="img"></param>
		/// <param name="rc"></param>
		/// <param name="bEnabled"></param>
        private int adjustvalueForDPI = 4;
		private void DrawFrameButton(Graphics g, Rectangle rc, int buttonId, bool bEnabled)
		{
			if (rc.Width > 0 && rc.Height > 0)
			{
				Image img = GetButtonImage(buttonId);

				if (img != null)
				{
					Size szImage = img.Size;

					int x = rc.X + (rc.Width - szImage.Width) / 2 - (this.IsRightToLeft ? 1 : 0);
					int y = rc.Y + (rc.Height - szImage.Height) / 2 +
						((buttonId != SB_HELPBUTTON && buttonId != SB_MDIHELPBUTTON) ? 1 : 0);

					if (bEnabled  )
					{
						Color penColor = CaptionButtonColor;
                        Border = Color.Transparent;
						if (this.HighlightedButton == buttonId)
						{
							if (this.PressedButton == buttonId)
							{
								DrawFrameButtonBackgroundPressed(g, ref rc);
							}
							else
							{
								DrawFrameButtonBackgroundSelected(g, ref rc);
								if (ShowMouseOver)
									penColor = Color.White;
								else
                                    penColor = CaptionButtonHoverColor;
                                if (!isDisablingHighlights)
                                    Border = CaptionButtonColor;
							}
						}

					  Pen pen = new Pen (penColor,2.0f);
						if (buttonId == 0)
						{
        
                            if (g.DpiX > 120)
                            {
                                g.DrawLine(pen, new Point(rc.X+1 , rc.Y + 3), new Point(rc.X + 17, rc.Y + 12 + 7));
                                g.DrawLine(pen, new Point(rc.X +1, rc.Y + 12 + 7), new Point(rc.X + 17, rc.Y + 3));
                            }
                            else if (g.DpiX > 96)
                            {
                                g.DrawLine(pen, new Point(rc.X + 4, rc.Y + 5), new Point(rc.X + 14, rc.Y+12 + 5));
                                g.DrawLine(pen, new Point(rc.X + 4 ,rc.Y + 12 + 5), new Point(rc.X + 14, rc.Y +5));
                            }
                            else
                            {
                                g.DrawLine(pen, new Point(x + 1, y), new Point(x + 7, y + 7));
                                g.DrawLine(pen, new Point(x + 1, y + 7), new Point(x + 7, y));
                            }
						}
                        else if (buttonId == 1)
                        {
                            if (g.DpiX > 120)
                            {
                                pen = new Pen(penColor);
                                Rectangle rectangle = new Rectangle(rc.X + 2, rc.Y + 6 - adjustvalueForDPI + 5, 8 + adjustvalueForDPI + 3, 6 + adjustvalueForDPI + 2);
                                g.DrawRectangle(pen, rectangle);
                                g.DrawLine(pen, new Point(rc.X + 2, rc.Y + 7 - adjustvalueForDPI + 5), new Point(rc.X + 12 + adjustvalueForDPI, rc.Y + 7 - adjustvalueForDPI + 5));
                                g.DrawLine(pen, new Point(rc.X + 2, rc.Y + 7 - adjustvalueForDPI + 6), new Point(rc.X + 12 + adjustvalueForDPI, rc.Y + 7 - adjustvalueForDPI + 6));
                            }
                            else if (g.DpiX > 96)
                            {
                                pen = new Pen(penColor);
                                Rectangle rectangle = new Rectangle(rc.X + 5, rc.Y + 6 - adjustvalueForDPI + 5, 8 + adjustvalueForDPI, 6 + adjustvalueForDPI);
                                g.DrawRectangle(pen, rectangle);
                                g.DrawLine(pen, new Point(rc.X + 5, rc.Y + 7 - adjustvalueForDPI + 5), new Point(rc.X + 12 + adjustvalueForDPI, rc.Y + 7 - adjustvalueForDPI + 5));
                            }
                            else
                            {            
                                pen = new Pen(penColor);
                                Rectangle rectangle = new Rectangle(rc.X + 5, rc.Y + 6, 8, 6);
                                g.DrawRectangle(pen, rectangle);
                                g.DrawLine(pen, new Point(rc.X + 5, rc.Y + 7), new Point(rc.X + 12, rc.Y + 7));
                            }
                        }
                        else if (buttonId == 2)
                        {
                            if (g.DpiX > 120)
                            {
                                g.DrawLine(pen, new Point(rc.X + 3, rc.Y + 11 + 7), new Point(rc.X + 16 + adjustvalueForDPI, rc.Y + 11 + 7));
                            }
                            else if (g.DpiX > 96)
                            {
                                g.DrawLine(pen, new Point(rc.X + 6, rc.Y + 11 + 5), new Point(rc.X + 13 + adjustvalueForDPI, rc.Y + 11 + 5));
                            }
                            else
                            g.DrawLine(pen, new Point(rc.X + 6, rc.Y + 12), new Point(rc.X + 13, rc.Y + 12));


                        }
                        else if (buttonId == 3)
                        {
                            if (g.DpiX > 120)
                            {
                                pen = new Pen(penColor);
                                Rectangle rectangle = new Rectangle(rc.X + 1, rc.Y + 6 - adjustvalueForDPI + 5, 6 + adjustvalueForDPI + 2, 5 + adjustvalueForDPI + 2);
                                g.DrawRectangle(pen, rectangle);
                                g.DrawLine(pen, new Point(rc.X + 1, rc.Y + 6 - adjustvalueForDPI + 5 + 1), new Point(rectangle.X + 12, rc.Y + 6 - adjustvalueForDPI + 5 + 1));
                                g.DrawLine(pen, new Point(rc.X + 8, rc.Y + 3), new Point(rc.X + 20, rc.Y + 3));
                                g.DrawLine(pen, new Point(rc.X + 8, rc.Y + 4), new Point(rc.X + 20, rc.Y + 4));
                                g.DrawLine(pen, new Point(rc.X + 8, rc.Y + 4), new Point(rc.X + 8, rc.Y + 6));
                                g.DrawLine(pen, new Point(rc.X + 20, rc.Y + 4), new Point(rc.X + 20, rc.Y + 14));
                                g.DrawLine(pen, new Point(rc.X + 20, rc.Y + 14), new Point(rc.X + 13, rc.Y + 14));

                            }
                            else if (g.DpiX > 96)
                            {
                                pen = new Pen(penColor);
                                Rectangle rectangle = new Rectangle(rc.X + 3, rc.Y + 6 - adjustvalueForDPI + 5, 6 + adjustvalueForDPI, 5 + adjustvalueForDPI);
                                g.DrawRectangle(pen, rectangle);
                                g.DrawLine(pen, new Point(rc.X + 3, rc.Y + 6 - adjustvalueForDPI + 5 + 1), new Point(rectangle.X + 10, rc.Y + 6 - adjustvalueForDPI + 5 + 1));
                                g.DrawLine(pen, new Point(rc.X + 7, rc.Y + 3), new Point(rc.X + 17, rc.Y + 3));
                                g.DrawLine(pen, new Point(rc.X + 7, rc.Y + 4), new Point(rc.X + 17, rc.Y + 4));
                                g.DrawLine(pen, new Point(rc.X + 7, rc.Y + 4), new Point(rc.X + 7, rc.Y + 6));
                                g.DrawLine(pen , new Point(rc.X + 17 , rc.Y +4) , new Point (rc.X + 17 , rc.Y + 12));
                                g.DrawLine(pen, new Point(rc.X + 17, rc.Y + 12), new Point(rc.X + 13, rc.Y + 12));
                               
                            }
                            else
                            {
                                pen = new Pen(penColor);
                                Rectangle rectangle = new Rectangle(rc.X + 7, rc.Y + 6, 6, 5);
                                //g.DrawRectangle(pen, rectangle);
                                g.DrawLine(pen, new Point(rc.X + 7, rc.Y + 5), new Point(rc.X + 12, rc.Y + 5));
                                g.DrawLine(pen, new Point(rc.X + 7, rc.Y + 6), new Point(rc.X + 12, rc.Y + 6));
                                g.DrawLine(pen, new Point(rc.X + 13, rc.Y + 5), new Point(rc.X + 13, rc.Y + 10));
                                g.DrawLine(pen, new Point(rc.X + 11, rc.Y + 10), new Point(rc.X + 13, rc.Y + 10));
                                rectangle = new Rectangle(rc.X + 5, rc.Y + 6, 6, 5);
                                g.DrawRectangle(pen, rectangle);
                                g.DrawLine(pen, new Point(rc.X + 5, rc.Y + 7), new Point(rc.X + 10, rc.Y + 7));
                            }
                        }
                        else if (buttonId == 20)
                        {
                            g.DrawImage(img, this.FrameLayout.HelpButton);
                        }
                        if (g.DpiX > 120)
                        {
                            rc.Width -= 5;
                        }
                        else if (g.DpiX > 96)
                        {
                            rc.Width -= 2;
                            if (buttonId == 0)
                                rc.X -= 1;
                        }
                        if (EnableTouchMode)
                        {
                            if (buttonId != 0)
                                rc.X -= rc.Width/4;
                        }
                        g.DrawRectangle(new Pen(Border), rc);
                        pen.Dispose();
					   
					}
					else
					{
                        Color penColor = Color.LightGray;
                        Pen pen = new Pen(penColor, 2.0f);
                        if (buttonId == 2)
                        {
                            if (g.DpiX > 120)
                            {
                                g.DrawLine(pen, new Point(rc.X + 3, rc.Y + 11 + 7), new Point(rc.X + 16 + adjustvalueForDPI, rc.Y + 11 + 7));
                            }
                            else if (g.DpiX > 96)
                            {
                                g.DrawLine(pen, new Point(rc.X + 6, rc.Y + 11 + 5), new Point(rc.X + 13 + adjustvalueForDPI, rc.Y + 11 + 5));
                            }
                            else
                            {
                                g.DrawLine(pen, new Point(rc.X + 6, rc.Y + 12), new Point(rc.X + 13, rc.Y + 12));
                            }
                        }
                        else if (buttonId == 1)
                        {
                            if (g.DpiX > 120)
                            {
                                pen = new Pen(penColor);
                                Rectangle rectangle = new Rectangle(rc.X + 2, rc.Y + 6 - adjustvalueForDPI + 5, 8 + adjustvalueForDPI + 3, 6 + adjustvalueForDPI + 2);
                                g.DrawRectangle(pen, rectangle);
                                g.DrawLine(pen, new Point(rc.X + 2, rc.Y + 7 - adjustvalueForDPI + 5), new Point(rc.X + 12 + adjustvalueForDPI, rc.Y + 7 - adjustvalueForDPI + 5));
                                g.DrawLine(pen, new Point(rc.X + 2, rc.Y + 7 - adjustvalueForDPI + 6), new Point(rc.X + 12 + adjustvalueForDPI, rc.Y + 7 - adjustvalueForDPI + 6));
                            }
                            else if (g.DpiX > 96)
                            {
                                pen = new Pen(penColor);
                                Rectangle rectangle = new Rectangle(rc.X + 5, rc.Y + 6 - adjustvalueForDPI + 5, 8 + adjustvalueForDPI, 6 + adjustvalueForDPI);
                                g.DrawRectangle(pen, rectangle);
                                g.DrawLine(pen, new Point(rc.X + 5, rc.Y + 7 - adjustvalueForDPI + 5), new Point(rc.X + 12 + adjustvalueForDPI, rc.Y + 7 - adjustvalueForDPI + 5));
                            }
                            else
                            {
                                pen = new Pen(penColor);
                                Rectangle rectangle = new Rectangle(rc.X + 5, rc.Y + 6, 8, 6);
                                g.DrawRectangle(pen, rectangle);
                                g.DrawLine(pen, new Point(rc.X + 5, rc.Y + 7), new Point(rc.X + 12, rc.Y + 7));
                            }
                        }
                        else
                            ControlPaint.DrawImageDisabled(g, img, x, y, Color.White);
                        pen.Dispose();
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		private void DrawFrameButtonBackgroundSelected(Graphics g, ref Rectangle rc)
		{
			if (ShowMouseOver)
			{
				SolidBrush brush = new SolidBrush(MetroColor);
				rc.Y = 0;
				rc.Height = this.TitleHeight;
				g.FillRectangle(brush, rc);
				brush.Dispose();
			}
			ToolTip t = new ToolTip();

		   
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		private void DrawFrameButtonBackgroundPressed(Graphics g, ref Rectangle rc)
		{

			if (ShowMouseOver)
			{
				SolidBrush brush = new SolidBrush(this.BackColor);
				rc.Y = 0;
				rc.Height = this.TitleHeight;
				g.FillRectangle(brush, rc);
				brush.Dispose();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		/// <param name="color"></param>
		/// <param name="color_4"></param>
		private void DrawFrameButtonGradient(Graphics g, ref Rectangle rc, Color clBegin, Color clEnd)
		{
			IntPtr hRgn = NativeMethods.CreateRoundRectRgn(rc.X, rc.Y, rc.Right, rc.Bottom, 2, 2);
			if (hRgn != IntPtr.Zero)
			{
				using (Region region = Region.FromHrgn(hRgn))
				{
					using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(rc.X, rc.Y, 1, rc.Height), clBegin, clEnd, 90f))
					{
						brush.Blend = m_blFrameButton;
						brush.WrapMode = WrapMode.TileFlipY;

						g.FillRegion(brush, region);
					}
				}
				NativeMethods.DeleteObject(hRgn);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		private void DrawFrameButtonFlash(Graphics g, ref Rectangle rc)
		{
			Rectangle rcBrush = new Rectangle(rc.X, rc.Y + rc.Height * 3 / 5, rc.Width, rc.Height);
			using (GraphicsPath path = new GraphicsPath())
			{
				path.AddEllipse(rcBrush);
				using (PathGradientBrush brush = new PathGradientBrush(path))
				{
					brush.CenterColor = Color.White;
					brush.SurroundColors = new Color[] { Color.Transparent };

					g.FillRectangle(brush, rc);
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		/// <param name="color"></param>
		private void DrawFrameButtonBorder(Graphics g, ref Rectangle rc, Color color)
		{
			Rectangle rcBrush = new Rectangle(rc.X, rc.Y, 1, rc.Height);
			using (LinearGradientBrush brush = new LinearGradientBrush(rcBrush, Color.White, Color.Transparent, 90f))
			{
				brush.Blend = m_blFrameButtonBorder;

				using (Pen pen = new Pen(brush))
				{
					g.DrawRectangle(pen, rc.X + 1, rc.Y + 1, rc.Width - 3, rc.Height - 3);
				}
			}
			using (Pen pen = new Pen(color))
			{
				g.DrawPolygon(pen, GetRoundedPolygon(rc, 1));
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		private void DrawFrameText(Graphics g, Rectangle rc)
		{
			if (rc.Width > 0 && rc.Height > 0)
			{
				string sText = this.Text;

				try
				{
					int cultureID = InputLanguage.CurrentInputLanguage.Culture.LCID;

					if (this.RightToLeft == RightToLeft.Yes && (cultureID == (int)HebrewCulture.HebrewIsrael || cultureID == (int)HebrewCulture.Hebrew))
					{
						string newstr = string.Empty;
						if (sText.EndsWith("."))
						{
							string[] str = sText.Split('.');
							newstr = str[0];
							sText = "." + newstr;
						}
						else if (sText.EndsWith(".]"))
						{
							int index = sText.LastIndexOf('.');
							newstr = sText.Remove(index, 2);
							sText = "[." + newstr;
						}
					}
				}
				catch (Exception ex)
				{
					// Forwards exceptions caught here to the Application class, therefore Application.ThreadException listener can handle it.
					Application.OnThreadException(ex);
				}

				if (sText != string.Empty)
				{
					IntPtr hdc = g.GetHdc();

					IntPtr hFont = this.CaptionFontInternal;
					if (hFont != IntPtr.Zero)
					{
						int bkMode = NativeMethods.SetBkMode(hdc, 1);
						int color = 0;
					   // if (this.CaptionForeColor != Color.Empty && this.CaptionForeColor != this.ColorTable.FormTextColor)
							color = NativeMethods.SetTextColor(hdc, ColorTranslator.ToWin32(this.CaptionForeColor));
						//else
						//    color = NativeMethods.SetTextColor(hdc, ColorTranslator.ToWin32(this.ColorTable.FormTextColor));
						IntPtr hObj = NativeMethods.SelectObject(hdc, hFont);
					   // color = this.ForeColor;
						int format = DT_SINGLELINE | DT_END_ELLIPSIS | DT_NOPREFIX;

						HorizontalAlignment align = this.CaptionAlign;
                        switch (this.CaptionVerticalAlignment)
                        {
                            case VerticalAlignment.Top:
                                format = DT_SINGLELINE | DT_END_ELLIPSIS | DT_NOPREFIX | DT_TOP;
                                break;
                            case VerticalAlignment.Center:
                                format = DT_SINGLELINE | DT_END_ELLIPSIS | DT_NOPREFIX | DT_VCENTER;
                                break;
                            case VerticalAlignment.Bottom:
                                format = DT_SINGLELINE | DT_END_ELLIPSIS | DT_NOPREFIX | DT_BOTTOM;
                                break;
                        }
                        switch (align)
                        {
                            case HorizontalAlignment.Center:
                                {
                                    format |= DT_CENTER;
                                }
                                break;
                            default:
                                if ((this.RightToLeft == RightToLeft.Yes && !this.IsRightToLeft) ^ (align == HorizontalAlignment.Right))
                                {
                                    format |= DT_RIGHT;
                                }
                                break;
                        }
             
            
						NativeMethods.RECT rect = new NativeMethods.RECT(rc);
                       if (IconTextRelation == LeftRightAlignment.Left && IconAlign == CaptionAlign)
						{
                           
                            if (align == HorizontalAlignment.Right)
                            {
                                rect.right = rect.right - 0;
                            }
                            else if (align == HorizontalAlignment.Center)
                            {
                                if (!MaximizeBox && !MinimizeBox)
                                    rect.left = rect.left + 82;
                                else
                                    rect.left = rect.left + 120;
                            }
                            if (this.WindowState == FormWindowState.Maximized)
                            {
                                rect.top = rect.top + 2;
                                rect.left = rect.left + 5;
                            }
                            NativeMethods.DrawText(hdc, sText, sText.Length, ref rect, format);
                        }
                        else
                        {
                            if (align == HorizontalAlignment.Right&& IconAlign==CaptionAlign)
                            {
                                rect.right = rect.right - 40;
                            }
                            else if(align==HorizontalAlignment.Right)
                            {
                                rect.right = rect.right - 0;
                            }
                            else
                            {
                                if (this.RightToLeft == RightToLeft.No)
                                {
                                    switch (this.WindowState)
                                    {
                                        case FormWindowState.Normal:
                                            rect.left = rect.left - 25;
                                            break;
                                        case FormWindowState.Maximized:
                                            rect.left = rect.left - 15;
                                            rect.top += 3;
                                            break;
                                    }
                                    if (rect.left < 5)
                                        rect.left = 5;
                                }
                                else
                                {
                                    switch (this.RightToLeft)
                                    {
                                        case System.Windows.Forms.RightToLeft.No:
                                            rect.right = rect.right - 25;
                                            break;
                                        case System.Windows.Forms.RightToLeft.Yes:
                                            rect.right = rect.right+25;
                                            break;
                                    }
                                }
                            }
                            NativeMethods.DrawText(hdc, sText, sText.Length, ref rect, format);//637603982
                        }
                       
						NativeMethods.SelectObject(hdc, hObj);
						NativeMethods.SetTextColor(hdc, color);
						NativeMethods.SetBkMode(hdc, bkMode);

						NativeMethods.DeleteObject(hFont);
					}

					g.ReleaseHdc(hdc);
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		private void DrawFrameBorders(Graphics g, Rectangle rc)
		{
			SmoothingMode smoothingMode = g.SmoothingMode;
			g.SmoothingMode = SmoothingMode.AntiAlias;

			if (this.WindowState != FormWindowState.Maximized)
			{
				Pen pen = new Pen (BorderColor,(float)this.BorderThickness );
				Rectangle rect = new Rectangle(rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
					g.DrawRectangle(pen, rect);
					pen.Dispose();
			   
			}

			int x1 = rc.Left + 1;
			int x2 = rc.Right - 1;
			int y = rc.Top + this.TitleHeight - 2;

			g.SmoothingMode = smoothingMode;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private Rectangle GetMaxRectangle()
		{
			Rectangle rc = this.TopLevel ? this.DesktopRectangle : this.ParentClientRectangle;

			int iBorderWidth = FrameLayout.BorderWidth;

			if (this.IsMdiChild)
			{
				rc.X -= iBorderWidth;
				rc.Y -= this.CaptionHeight;
				rc.Width += 2 * iBorderWidth;
				rc.Height += this.CaptionHeight + iBorderWidth;
			}
			else
			{
				rc = new Rectangle(-iBorderWidth, -iBorderWidth, rc.Width + 2 * iBorderWidth, rc.Height + 2 * iBorderWidth);
			}

			return rc;
		}
		/// <summary>
		/// Gets Image for a button by specific ID.
		/// </summary>
		/// <param name="buttonID"> Button ID that indicates Image. </param>
		/// <returns></returns>
		private Image GetButtonImage(int buttonID)
		{
			Image imgButton = null;

			switch (buttonID)
			{
				case SB_HELPBUTTON:
				case SB_MDIHELPBUTTON:
					imgButton = m_bmpHelpButton;
					break;
				default:
					int idx = m_theme < 0 ? 0 : (int)m_theme;
					try
					{
						imgButton = m_systemButtons.Images[idx * SB__MAX + (buttonID & 0xf)];
					}
					catch (ArgumentOutOfRangeException)
					{
						UpdateSystemButtonsImages();
						imgButton = m_systemButtons.Images[idx * SB__MAX + (buttonID & 0xf)];
					}
					catch { }
					break;
			}

			return imgButton;
		}

		private void UpdateSystemButtonsImages()
		{
			m_systemButtons.Images.Clear();
			m_systemButtons = null;

			Bitmap systemButtons = new Bitmap(typeof(MetroForm).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Office2007Form.SystemButtons.bmp"));

			m_systemButtons = new ImageList();
			m_systemButtons.ImageSize = new Size(9, 9);
			m_systemButtons.Images.AddStrip(systemButtons);
			m_systemButtons.TransparentColor = Color.Magenta;
		}

		/// <summary>
		/// Gets system command by specific buttonID.
		/// </summary>
		/// <param name="buttonID"> Button ID that indicates system command. </param>
		/// <returns> </returns>
		private int GetButtonCommand(int buttonID)
		{
			int iSystemCommand = 0;

			switch (buttonID)
			{
				case SB_HELPBUTTON:
				case SB_MDIHELPBUTTON:
					iSystemCommand = m_iSystemHelpCommand;
					break;

				default:
					iSystemCommand = m_systemCommands[buttonID & 0x3];
					break;
			}

			return iSystemCommand;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		private void MoveWindow(ref Message m)
		{
			if (this.WindowState != FormWindowState.Maximized)
			{
				NativeMethods.SendMessage(m.HWnd, NativeMethods.WM_SYSCOMMAND, NativeMethods.SC_MOVE | 2, m.LParam);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <param name="command"></param>
		private void SizeWindow(ref Message m, int direction)
		{
			NativeMethods.SendMessage(m.HWnd, NativeMethods.WM_SYSCOMMAND, NativeMethods.SC_SIZE | direction, m.LParam);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		internal bool GetIsCompositionEnabled()
		{
			bool bResult = false;

			if (Environment.OSVersion.Version.Major >= 6)
			{
				DwmIsCompositionEnabled(ref bResult);
			}

			return bResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hMenu"></param>
		/// <param name="uFlags"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="nReserved"></param>
		/// <param name="hWnd"></param>
		/// <param name="prcRect"></param>
		/// <returns></returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
		private static extern int TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
		private static extern bool AdjustWindowRectEx(ref NativeMethods.RECT lpRect, int dwStyle, bool bMenu, int dwExStyle);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
		private static extern IntPtr CreateFontIndirect(ref NativeMethods.LOGFONT lplf);

		[DllImport("dwmapi.dll")]
		private static extern Int32 DwmIsCompositionEnabled(ref bool pfEnabled);

		[DllImport("user32.dll", EntryPoint = "GetClassLong")]
		private static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
		private static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

		private static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
		{
			if (IntPtr.Size > 4)
			{
				return GetClassLongPtr64(hWnd, nIndex);
			}
			else
			{
				return new IntPtr(GetClassLongPtr32(hWnd, nIndex));
			}
		}

		/// <summary>
		/// Invalidates client area if MetroScheme back color is used.
		/// </summary>
		private void InvalidateOnDemand()
		{
			if (this.UseOffice2007SchemeBackColor)
			{
				Invalidate();
			}
		}

		/// <summary>
		/// Occurs when <see cref="Office2007Form.ColorScheme"/> property is changed.
		/// </summary>
		protected virtual void OnColorSchemeChanged()
		{
			InvalidateFrame();
			InvalidateOnDemand();
		}

		#endregion

		#region ShouldSerialize/Reset members

		bool ShouldSerializeColorScheme()
		{
			return m_theme >= 0;
		}
		void ResetColorScheme()
		{
			m_theme = (Office2007Theme)(-1);
		}

		bool ShouldSerializeCaptionFont()
		{
			return m_captionFont != null;
		}
        bool ShouldSerializeCaptionButtonHoverColor()
        {
            return this.CaptionButtonHoverColor != Color.DarkGray;
        }
		void ResetCaptionFont()
		{
			this.CaptionFont = null;
		}
        void ResetCaptionHoverColor()
        {
            this.CaptionButtonHoverColor = Color.DarkGray;
        }
        void ResetVerticalAlignment()
        {
            this.CaptionVerticalAlignment = VerticalAlignment.Center;
        }
        void ResetCaptionBarHeight()
        {
            this.CaptionBarHeight = 31;
        }
        private bool ShouldSerializeShowMouseOver()
        {
            return ShowMouseOver != false;
        }
        private bool ShouldSerializeVerticalAlignmment()
        {
            return CaptionVerticalAlignment != VerticalAlignment.Center;
        }
        private bool ShouldSerialzeCaptionHeight()
        {
            return CaptionBarHeight != 31;
        }
        private bool ShouldSerializeMetroColor()
        {
            return MetroColor != Color.White;
        }

        private bool ShouldSerializeCaptionBarColor()
        {
            return CaptionBarColor != Color.White;
        }
        private bool ShouldSerializeBorderColor()
        {
            return BorderColor != ColorTranslator.FromHtml("#737373");
        }

        private bool ShouldSerializeCaptionButtonColor()
        {
            return CaptionButtonColor != Color.DarkGray;
        }

        private bool ShouldSerializeBorderThickness()
        {
            return BorderThickness != 1;
        }



		#endregion

		#region Fields
		/// <summary>
		/// 
		/// </summary>
		private Office2007Theme m_theme = Office2007Theme.Managed;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bActive = false;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bMouseIsTracked = false;
		/// <summary>
		/// 
		/// </summary>
		private FrameLayoutInfo m_frameLayout;
		/// <summary>
		/// Selected system button. (SB__MAX - no button is selected)
		/// </summary>
		private int m_selectedButton = SB__MAX;
		/// <summary>
		/// Pressed system button. (SB__MAX - no button is pressed)
		/// </summary>
		private int m_pressedButton = SB__MAX;
		/// <summary>
		/// 
		/// </summary>
		private int m_highlightedButton = SB__MAX;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bSuppressSizing = false;
		/// <summary>
		/// 
		/// </summary>
		private Pen m_pBorder;
		/// <summary>
		/// 
		/// </summary>
		private Pen m_pSeparatorDark;
		/// <summary>
		/// 
		/// </summary>
		private Pen m_pSeparatorLight;
		/// <summary>
		/// 
		/// </summary>
		static ImageList m_systemButtons;
		/// <summary>
		/// 
		/// </summary>
		static Bitmap m_bmpHelpButton;
		/// <summary>
		/// 
		/// </summary>
		static int[] m_systemCommands;
		/// <summary>
		/// 
		/// </summary>
		static int m_iSystemHelpCommand;
		/// <summary>
		/// 
		/// </summary>
		static Blend m_blTitle;
		/// <summary>
		/// 
		/// </summary>
		static Blend m_blFrameButton;
		/// <summary>
		/// 
		/// </summary>
		static Blend m_blFrameButtonBorder;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bUseOffice2007ThemeBackground = false;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bDisableOffice2007Style = false;
		/// <summary>
		/// 
		/// </summary>
		private bool applyAeroTheme = false;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bCompositionEnabled = false;

		/// <summary>
		/// 
		/// </summary>
		private Font m_captionFont = null;
		/// <summary>
		/// 
		/// </summary>
		private HorizontalAlignment m_captionAlign = HorizontalAlignment.Left;
        private HorizontalAlignment m_iconAlign = HorizontalAlignment.Left;// icon align
        private LeftRightAlignment m_iconTextRelation = LeftRightAlignment.Left;// icon text relation
		/// <summary>
		/// CaptionText Color
		/// </summary>
		private Color captionForeColor = Color.Empty;
        /// <summary>
        /// location for label in caption
        /// </summary>
        private Point labelLocation;
        /// <summary>
        /// CaptionImage Collection
        /// </summary>
        private CaptionImageCollection captionImages;
        /// <summary>
        /// LabelCollection
        /// </summary>
        private CaptionLabelCollection captionLabels;
        /// <summary>
        /// BorderColor to highlights the ControlBox
        /// </summary>
        private Color Border;
        /// <summary>
        /// Value for show/hide the highlights 
        /// </summary>
        private bool isDisablingHighlights = false;
		#endregion

		#region Delegates
		private delegate IntPtr SendMessageDelegate(IntPtr hWnd, int msg, int wParam, int lParam);
		#endregion

		#region Nested classes

		#region *** FrameLayoutInfo

		class FrameLayoutInfo
		{
			#region Constructors
			public FrameLayoutInfo(MetroForm form)
			{
				m_form = form;
			}
			#endregion

			#region Methods
			public void PerformLayout(int width, int height)
			{
				m_titleHeight = 0;
				m_captionHeight = 0;
				m_captionMinWidth = 0;

				m_rcIcon = Rectangle.Empty;
				m_rcMin = Rectangle.Empty;
				m_rcMax = Rectangle.Empty;
				m_rcClose = Rectangle.Empty;
				m_rcHelpButton = Rectangle.Empty;
				m_rcMdiIcon = Rectangle.Empty;
				m_rcMdiMin = Rectangle.Empty;
				m_rcMdiMax = Rectangle.Empty;
				m_rcMdiClose = Rectangle.Empty;
				m_rcMdiHelpButton = Rectangle.Empty;
				m_rcText = Rectangle.Empty;

				bool bIsFormBorderStyleNone = m_form.FormBorderStyle == FormBorderStyle.None;

                m_iBorderWidth = bIsFormBorderStyleNone ? 0 : BORDER_WIDTH;

				Size szIcon = SystemInformation.SmallIconSize;
                if (m_form.EnableTouchMode)
                {
                    szIcon = new Size(szIcon.Width + 15, szIcon.Height);
                }
				bool bRightToLeft = m_form.IsRightToLeft;

				int buttonWidth = (szIcon.Width & ~1) + 3;
				int buttonHeight = szIcon.Height + 2;

				int sysBorderWidth = this.SysBorderWidth;
				int captionPadding = sysBorderWidth + FRAME_PADDING;

				if (!bIsFormBorderStyleNone)
				{
					Font captionFont = m_form.CaptionFont;
					int contentHeight = Math.Max(captionFont.Height, buttonHeight);

					if (!m_form.ShouldSerializeCaptionFont())	// It means m_captionForn == null and CaptionFont property returns default dynamic font.
					{
						captionFont.Dispose();
					}
					if (!m_form.ControlBox && m_form.Text == string.Empty)
					{
						m_titleHeight = this.BorderWidth;
					}
					else
					{
                        m_titleHeight = Math.Max(captionPadding + contentHeight + SEPARATOR_WIDTH ,m_form.CaptionBarHeight);
					}
					m_captionHeight = m_titleHeight;

					int top = m_form.IsMinimized ? (height - buttonHeight) / 2 : sysBorderWidth + (m_titleHeight - sysBorderWidth - SEPARATOR_WIDTH - buttonHeight) / 2;
					int left = captionPadding;
					int right = width - captionPadding;

					if (m_form.ControlBox)
					{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
						if (m_form.ShowIcon)
#endif
						{
							m_rcIcon.Size = szIcon;
							m_rcIcon.Y = top;

							if (bRightToLeft)
							{
								right -= szIcon.Width;
								m_rcIcon.X = right;
							}
							else
							{
								m_rcIcon.X = left;
								left = m_rcIcon.Right;
							}
							m_captionMinWidth += szIcon.Width;
						}

						m_rcClose.Y = top;
						m_rcClose.Width = buttonWidth;
						m_rcClose.Height = buttonHeight;

						if (bRightToLeft)
						{
							m_rcClose.X = left;
							left = m_rcClose.Right;
						}
						else
						{
							right -= buttonWidth;
                            if (m_form.EnableTouchMode)
                            {
                                m_rcClose.X = right - 5;
                                
                            }
                            else
							m_rcClose.X = right;
						}

						m_captionMinWidth += buttonWidth;

						if (m_form.MaximizeBox || m_form.MinimizeBox)
						{
							m_rcMax = m_rcClose;
							m_rcMin = m_rcClose;

							if (bRightToLeft)
							{
								m_rcMax.X = left;
								m_rcMin.X = m_rcMax.Right;
								left = m_rcMin.Right;
							}
							else
							{
								m_rcMax.X = right - buttonWidth;
								m_rcMin.X = m_rcMax.X - buttonWidth;
								right = m_rcMin.X;
							}

							m_captionMinWidth += 2 * buttonWidth;
						}
						else if (m_form.HelpButton)
						{
							m_rcHelpButton = m_rcClose;

							if (bRightToLeft)
							{
								m_rcHelpButton.X = left;
								left = m_rcHelpButton.Right;
							}
							else
							{
								right -= buttonWidth;
								m_rcHelpButton.X = right;
							}

							m_captionMinWidth += buttonWidth;
						}
					}

					m_rcText = new Rectangle(left + TEXT_MARGIN, 5, right - left - 2 * TEXT_MARGIN, m_titleHeight - sysBorderWidth);

					if (m_form.IsRightToLeft)
					{
						m_rcText.X = width - m_rcText.Right;
					}
				}

				if (m_form.IsMdiContainer)
				{
					Form fChild = m_form.ActiveMdiChild;

					if (fChild != null && IsMaximized(fChild))
					{
						Control mdiClient = fChild.Parent;
						if (mdiClient != null && NativeMethods.SendMessage(mdiClient.Handle, NativeMethods.WM_MDIREFRESHMENU, 0, 0) != IntPtr.Zero)
						{
							if (fChild.ControlBox)
							{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
								if (fChild.ShowIcon)
#endif
								{
									m_rcMdiIcon.X = bRightToLeft ? width - captionPadding - szIcon.Width : captionPadding;
									m_rcMdiIcon.Y = m_captionHeight + 1;
									m_rcMdiIcon.Size = szIcon;
								}

								m_rcMdiClose.X = bRightToLeft ? captionPadding : width - captionPadding - buttonWidth;
								m_rcMdiClose.Y = m_captionHeight;
								m_rcMdiClose.Width = buttonWidth;
								m_rcMdiClose.Height = buttonHeight;

								if (fChild.MaximizeBox || fChild.MinimizeBox)
								{
									m_rcMdiMax = m_rcMdiClose;
									m_rcMdiMin = m_rcMdiClose;

									if (bRightToLeft)
									{
										m_rcMdiMax.X = m_rcMdiClose.Right;
										m_rcMdiMin.X = m_rcMdiMax.Right;
									}
									else
									{
										m_rcMdiMax.X = m_rcMdiClose.X - buttonWidth;
										m_rcMdiMin.X = m_rcMdiMax.X - buttonWidth;
									}
								}
								else if (fChild.HelpButton)
								{
									m_rcMdiHelpButton = m_rcMdiClose;

									if (bRightToLeft)
									{
										m_rcMdiHelpButton.X = m_rcMdiClose.Right;
									}
									else
									{
										m_rcMdiHelpButton.X = m_rcMdiClose.X - buttonWidth;
									}
								}
							}

							m_captionHeight += buttonHeight + 2;
						}
					}
				}
			}
			#endregion

			#region Properties
			/// <summary>
			/// 
			/// </summary>
			public Rectangle TextBox
			{
				get { return m_rcText; }
			}
			/// <summary>
			/// 
			/// </summary>
			public Rectangle IconBox
			{
				get { return m_rcIcon; }
			}
			/// <summary>
			/// 
			/// </summary>
			public Rectangle MinimizeBox
			{
				get { return m_rcMin; }
			}
			/// <summary>
			/// 
			/// </summary>
			public Rectangle MaximizeBox
			{
				get { return m_rcMax; }
			}
			/// <summary>
			/// 
			/// </summary>
			public Rectangle CloseBox
			{
				get { return m_rcClose; }
			}
			/// <summary>
			/// 
			/// </summary>
			public Rectangle HelpButton
			{
				get { return m_rcHelpButton; }
			}
			/// <summary>
			/// 
			/// </summary>
			public Rectangle MdiIconBox
			{
				get { return m_rcMdiIcon; }
			}
			/// <summary>
			/// 
			/// </summary>
			public Rectangle MdiMinimizeBox
			{
				get { return m_rcMdiMin; }
			}
			/// <summary>
			/// 
			/// </summary>
			public Rectangle MdiMaximizeBox
			{
				get { return m_rcMdiMax; }
			}
			/// <summary>
			/// 
			/// </summary>
			public Rectangle MdiCloseBox
			{
				get { return m_rcMdiClose; }
			}
			/// <summary>
			/// 
			/// </summary>
			public Rectangle MdiHelpButton
			{
				get { return m_rcMdiHelpButton; }
			}
			/// <summary>
			/// 
			/// </summary>
			public int TitleHeight
			{
				get { return m_titleHeight; }
			}
			/// <summary>
			/// 
			/// </summary>
			public int CaptionHeight
			{
				get { return m_captionHeight; }
			}
			/// <summary>
			/// 
			/// </summary>
			public int CaptionMinWidth
			{
				get { return m_captionMinWidth; }
			}
			/// <summary>
			/// Gets border width of the Office2007Form instance.
			/// </summary>
			public int BorderWidth
			{
				get { return m_iBorderWidth; }
			}

			/// <summary>
			/// 
			/// </summary>
			private int SysCaptionHeight
			{
				get
				{
					CreateParams cp = m_form.CreateParams;

					NativeMethods.RECT rc = new NativeMethods.RECT();

					if (AdjustWindowRectEx(ref rc, cp.Style, false, cp.ExStyle))
					{
						return -rc.top;
					}

					return 0;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			private int SysBorderWidth
			{
				get
				{
					CreateParams cp = m_form.CreateParams;

					NativeMethods.RECT rc = new NativeMethods.RECT();

					if (AdjustWindowRectEx(ref rc, cp.Style, false, cp.ExStyle))
					{
						return rc.Width / 2;
					}

					return 0;
				}
			}
			#endregion

			#region Implementation
			/// <summary>
			/// 
			/// </summary>
			/// <param name="f"></param>
			/// <returns></returns>
			static bool IsMaximized(Form f)
			{
				if (f.IsHandleCreated)
				{
					int style = NativeMethods.GetWindowLong(f.Handle, NativeMethods.GWL_STYLE);
					return (style & NativeMethods.WS_MAXIMIZE) != 0;
				}
				return f.WindowState == FormWindowState.Maximized;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="rc"></param>
			/// <param name="dwStyle"></param>
			/// <param name="bMenu"></param>
			/// <param name="dwExStyle"></param>
			/// <returns></returns>
			[DllImport("User32.dll", CharSet = CharSet.Auto)]
			static extern bool AdjustWindowRectEx(ref NativeMethods.RECT rc, Int32 dwStyle, bool bMenu, Int32 dwExStyle);
			#endregion

			#region Fields
			/// <summary>
			/// 
			/// </summary>
			private MetroForm m_form;
			/// <summary>
			/// 
			/// </summary>
			private Rectangle m_rcText = Rectangle.Empty;
			/// <summary>
			/// 
			/// </summary>
			public Rectangle m_rcIcon = Rectangle.Empty;
			/// <summary>
			/// 
			/// </summary>
			public Rectangle m_rcMin = Rectangle.Empty;
			/// <summary>
			/// 
			/// </summary>
			public Rectangle m_rcMax = Rectangle.Empty;
			/// <summary>
			/// 
			/// </summary>
			public Rectangle m_rcClose = Rectangle.Empty;
			/// <summary>
			/// 
			/// </summary>
			public Rectangle m_rcHelpButton = Rectangle.Empty;
			/// <summary>
			/// 
			/// </summary>
			public Rectangle m_rcMdiIcon = Rectangle.Empty;
			/// <summary>
			/// 
			/// </summary>
			public Rectangle m_rcMdiMin = Rectangle.Empty;
			/// <summary>
			/// 
			/// </summary>
			public Rectangle m_rcMdiMax = Rectangle.Empty;
			/// <summary>
			/// 
			/// </summary>
			public Rectangle m_rcMdiClose = Rectangle.Empty;
			/// <summary>
			/// 
			/// </summary>
			public Rectangle m_rcMdiHelpButton = Rectangle.Empty;
			/// <summary>
			/// 
			/// </summary>
			private int m_titleHeight = 0;
			/// <summary>
			/// 
			/// </summary>
			private int m_captionHeight = 0;
			/// <summary>
			/// 
			/// </summary>
			private int m_captionMinWidth = 0;
			/// <summary>
			/// Border width of the Office2007Form instance.
			/// </summary>
			private int m_iBorderWidth = BORDER_WIDTH;
			#endregion
		}

		#endregion

		#region *** ColorSchemeTypeConverter

		class ColorSchemeTypeConverter : EnumConverter
		{
			public ColorSchemeTypeConverter(Type type) : base(type) { }

			public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
			{
				if (destinationType == typeof(string) && (int)value < 0)
				{
					return "Managed";
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}

		#endregion

		#region *** CaptionManager

		class CaptionManager : IDisposable
		{
			const int MASK = NativeMethods.WS_CAPTION;

			#region Constructors
			/// <summary>
			/// 
			/// </summary>
			/// <param name="c"></param>
			public CaptionManager(MetroForm f, bool bHideCaption)
			{
				if (f != null && f.IsHandleCreated)
				{
					IntPtr hWnd = f.Handle;

					m_style = NativeMethods.GetWindowLong(hWnd, NativeMethods.GWL_STYLE);

					if (bHideCaption)
					{
						if ((m_style & MASK) != 0)
						{
							NativeMethods.SetWindowLong(hWnd, NativeMethods.GWL_STYLE, (IntPtr)(m_style & ~MASK));
							m_form = f;
						}
					}
					else
					{
						int mask = f.CreateParams.Style & MASK;
						if (mask != 0)
						{
							if ((m_style & mask) == 0 && (m_style & NativeMethods.WS_THICKFRAME) != 0)
							{
								NativeMethods.SetWindowLong(hWnd, NativeMethods.GWL_STYLE, (IntPtr)(m_style | mask));
								m_form = f;
							}
						}
					}
				}
			}
			#endregion

			#region IDisposable Members

			void IDisposable.Dispose()
			{
				if (m_form != null && m_form.IsHandleCreated)
				{
					NativeMethods.SetWindowLong(m_form.Handle, NativeMethods.GWL_STYLE, (IntPtr)(m_style));
				}
			}

			#endregion

			#region Fields

			private MetroForm m_form = null;

			private int m_style;

			#endregion

		}

		#endregion

		#endregion
	}
     
    /// <summary>
    /// Vertical Alignment
    /// </summary>
    public enum VerticalAlignment
    {
        /// <summary>
        /// Top 
        /// </summary>
        Top,
        /// <summary>
        /// Center
        /// </summary>
        Center,
        /// <summary>
        /// 
        /// </summary>
        Bottom
    }
}
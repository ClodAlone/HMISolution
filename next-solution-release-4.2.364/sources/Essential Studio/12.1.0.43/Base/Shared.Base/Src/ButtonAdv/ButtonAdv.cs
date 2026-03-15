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
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.ComponentModel;
using Syncfusion.Windows.Forms.Tools;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Data;
using Syncfusion.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using ComponentModel_RefreshProperties = System.ComponentModel.RefreshProperties;
#endregion

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Button with advanced rendering features.
	/// </summary>
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
	[Designer( typeof( ButtonAdvDesigner ), typeof( IDesigner ) )]
#endif
	[
	ToolboxItem( true ),
	ToolboxBitmap( typeof( Syncfusion.Windows.Forms.ButtonAdv ), "ToolboxIcons.ButtonAdv.bmp" )
	]
	public class ButtonAdv:
		Button,
		ISupportOffice2007Theme,
		ISupportThemeChanged, IMessageFilter,IVisualStyle 
	{
		#region Class constants
		/// <summary>
		/// Specifies ButtonAdv default border width.
		/// </summary>
		private const int c_nDEFAULT_BORDER_WIDTH = 1;
		/// <summary>
		/// Draw the window only if it is visible.
		/// </summary>
		private const int PRF_CHECKVISIBLE = 0x00000001;
		/// <summary>
		/// Draw the non-client area of the window.
		/// </summary>
		private const int PRF_NONCLIENT = 0x00000002;
		/// <summary>
		/// Draw the client area of the window.
		/// </summary>
		private const int PRF_CLIENT = 0x00000004;
		/// <summary>
		/// Erase the background before drawing the window.
		/// </summary>
		private const int PRF_ERASEBKGND = 0x00000008;
		/// <summary>
		/// Draw all visible child windows.
		/// </summary>
		private const int PRF_CHILDREN = 0x00000010;
		/// <summary>
		/// Draw all owned windows.
		/// </summary>
		private const int PRF_OWNED = 0x00000020;
		/// <summary>
		/// </summary>
		private const int PRF_ALL = PRF_CHILDREN | PRF_ERASEBKGND | PRF_CLIENT | PRF_NONCLIENT;
		#endregion

		#region Class members
		/// <summary>
		/// Indicates whether renderer could  draw the background for half of the control differently.
		/// Used in XP style when used as a combo button.
		/// </summary>
		/// <remarks>Used only for by <see cref="WindowsXPButtonRenderer"/> class.</remarks>
		[EditorBrowsable( EditorBrowsableState.Never )]
		public bool isLastLeftButton = false;
		/// <summary>
		/// Indicates whether renderer could draw the background for half of the control differently.
		/// Used in XP style when used as a combo button.
		/// </summary>
		/// <remarks>Used only for by <see cref="WindowsXPButtonRenderer"/> class.</remarks>
		[EditorBrowsable( EditorBrowsableState.Never )]
		public bool isFirstRightButton = false;
		/// <summary></summary>
		private bool m_bIgnoreMouse = false;
		/// <summary>
		/// Specifies border style of ButtonAdv.
		/// </summary>
		private ButtonAdvBorderStyle m_bsBorderStyle = ButtonAdvBorderStyle.Default;
		/// <summary>
		/// Internal usage field ( Border drawing ).
		/// </summary>
		static private Hashtable s_hashDelegates;
		/// <summary>
		/// Reference counter.
		/// </summary>
		static private int s_nHashDelegates = 0;
        /// <summary></summary>
        internal bool altPressed = false;
		/// <summary>
		/// Indicates whether ButtonAdv will show focus rectenagle.
		/// </summary>
		private bool m_bKeepFocusRectangle = true;
		/// <summary></summary>
		private ButtonAppearance m_appearance = ButtonAppearance.Classic;
		/// <summary></summary>
		private ButtonAppearance m_cachedAppearance;
		/// <summary></summary>
		private ButtonRenderer m_renderer;
		/// <summary></summary>
		private ButtonAdvState m_state = ButtonAdvState.Default;
		/// <summary></summary>
		private ButtonTypes m_buttonImageType = ButtonTypes.Normal;
		/// <summary></summary>
		private Color m_comboEditBackColor = Color.Empty;
		/// <summary></summary>
		private bool m_isComboButton = false;
		/// <summary></summary>
		private bool m_bPushable = false;
		/// <summary></summary>
		private bool m_pushed = false;
		/// <summary></summary>
		private bool m_bIsDown = false;
		/// <summary></summary>
		private int m_animateState;
		/// <summary></summary>
		private Office2007Theme m_colorScheme = Office2007Theme.Blue;
        private Office2010Theme m_color2010Scheme = Office2010Theme.Blue;
        /// <summary></summary>
        private Color m_metroColor=Color.Empty;
		/// <summary></summary>
		private Color m_foreColor = Color.Empty;
		/// <summary></summary>
		private Color m_backColor = Color.Empty;
        /// <summary></summary>
        private Color m_customColor = Color.Empty;
        /// <summary></summary>
        private bool m_overrideManagedColor = false;
		/// <summary></summary>
		private UseStyle m_useStyle = UseStyle.Inherited;
		/// <summary></summary>
		private XPThemesThemeChangedWeakContainer m_weakThemeChanged;

        private bool resetStateOnLostFocus = true;

        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

		#endregion

		#region Class static methods
		/// <summary>
		/// Drawing ButtonAdv's surface on custom Graphics
		/// </summary>
		/// <param name="g">Graphics to draw on</param>
		/// <param name="control">buttonAdv control to draw</param>
		/// <param name="p">Point that represents the location of the upper-left
		///  corner of the drawn image.</param>
		///  <example>
		///         private void pictureBox1_Paint(object sender, PaintEventArgs e)
		///         {
		///             e.Graphics.RotateTransform(20); 
		///             ButtonAdv.PaintButton(e.Graphics, this.buttonAdv1, new Point(75, 25));
		///         }
		///  </example>
		public static void PaintButton( Graphics g, ButtonAdv control, Point p )
		{
			if( control != null && control.IsHandleCreated && g != null )
			{
				Rectangle bounds = control.Bounds;

				using( Bitmap b = new Bitmap( bounds.Width, bounds.Height ) )
				{
					b.SetResolution( g.DpiX, g.DpiY );

					using( Graphics gr = Graphics.FromImage( b ) )
					{
						IntPtr hdc = gr.GetHdc();
                        try
                        {
                            NativeMethods.SendMessage(control.Handle, NativeMethods.WM_PRINT, hdc, PRF_ALL);
                        }
                        finally
                        {
                            gr.ReleaseHdc(hdc);
                        }
					}

					g.DrawImage( b, p );
				}
			}
		}
		#endregion

		#region Class properties

        /// <summary>
        /// Indicates whether button have DesignMode
        /// </summary>                
        internal bool IsDesignMode
        {
            get
            {
                return base.DesignMode;
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
                if (style != null)
                   UseVisualStyle = true;
                else
                    UseVisualStyle = false;
                if (value == "Office2007Blue")
                {
                    Appearance = ButtonAppearance.Office2007;
                    Office2007ColorScheme = Office2007Theme.Blue;
                }
                else if (value == "Office2007Silver")
                {
                    Appearance = ButtonAppearance.Office2007;
                    Office2007ColorScheme = Office2007Theme.Silver;
                }
                else if (value == "Office2007Black")
                {
                    Appearance = ButtonAppearance.Office2007;
                    Office2007ColorScheme = Office2007Theme.Black;
                }
                else if (value == "Office2010Blue")
                {
                    Appearance = ButtonAppearance.Office2010;
                    Office2010ColorScheme = Office2010Theme.Blue;
                }
                else if (value == "Office2010Silver")
                {
                    Appearance = ButtonAppearance.Office2010;
                    Office2010ColorScheme = Office2010Theme.Silver;
                }
                else if (value == "Office2010Black")
                {
                    Appearance = ButtonAppearance.Office2010;
                    Office2010ColorScheme = Office2010Theme.Black;
                } 
                else if (value == "Managed")
                {
                     Office2007ColorScheme = Office2007Theme.Managed;
                }
                else if (value == "Metro")
                {
                    Appearance = ButtonAppearance.Metro;                  
                }
                else if (value == "Office2000")
                    Appearance = ButtonAppearance.Office2000;
                else if (value == "Classic")
                    Appearance = ButtonAppearance.Classic;
                else if (value == "Office2003")
                    Appearance = ButtonAppearance.Office2003;
                else if (value == "OfficeXP")
                    Appearance = ButtonAppearance.OfficeXP;
                else if (value == "WindowsXP")
                    Appearance = ButtonAppearance.WindowsXP;
                else if (value == "None")
                    Appearance = ButtonAppearance.None;
               
            }
        }
		/// <summary>
        /// Gets or sets office 2007 color scheme.
		/// </summary>
		[
		Browsable( true ),
		Category( "Appearance - Styles" ),
		Description( "Specifies color scheme for the control." ),
		RefreshProperties( RefreshProperties.Repaint ),
		DefaultValue( Office2007Theme.Blue )
		]
		public Office2007Theme Office2007ColorScheme
		{
			get
			{
				return m_colorScheme;
			}
			set
			{
				if( m_colorScheme != value )
				{
					m_colorScheme = value;
					if( m_renderer != null && m_renderer is Office2007ButtonRenderer )
						m_renderer.SetColorScheme( m_colorScheme );
					this.Invalidate();
				}
			}
		}
        /// <summary>
        /// Gets or sets office 2010 color scheme.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance - Styles"),
        Description("Specifies color scheme for the control."),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(Office2010Theme.Blue)
        ]
        public Office2010Theme Office2010ColorScheme
        {
            get
            {
                return m_color2010Scheme;
            }
            set
            {
                if (m_color2010Scheme != value)
                {
                    m_color2010Scheme = value;
                    if (m_renderer != null && m_renderer is Office2010ButtonRenderer)
                        m_renderer.Set2010ColorScheme(m_color2010Scheme);
                    this.Invalidate();
                }
            }
        }
		/// <summary>
		/// Gets or sets ButtonAdv border style.	Borders styles supported only in 
		/// appearance styles: <see cref="ButtonAppearance.Office2003"/>, 
		/// <see cref="ButtonAppearance.OfficeXP"/> and <see cref="ButtonAppearance.WindowsXP"/>.
		/// </summary>
		[
		Browsable( true ),
		Category( "Appearance - Styles" ),
		Description( "Specifies ButtonAdv border style. Has influence only on Office2003, OfficeXp and WindowsXP styles." ),
		RefreshProperties( RefreshProperties.Repaint ),
		DefaultValue( ButtonAdvBorderStyle.Default )
		]
		public ButtonAdvBorderStyle BorderStyleAdv
		{
			get
			{
				return m_bsBorderStyle;
			}
			set
			{
				if( m_bsBorderStyle != value )
				{
					m_bsBorderStyle = value;
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or Sets value specifying whether ButtonAdv will show focus rectangle receiveng focus. 
		/// </summary>
		[
		Browsable( true ),
		Category( "Appearance - Styles" ),
		Description( "Indicates whether ButtonAdv will show focus rectangle receiving focus." ),
		DefaultValue( true )
		]
		public bool KeepFocusRectangle
		{
			get
			{
				return m_bKeepFocusRectangle;
			}
			set
			{
				m_bKeepFocusRectangle = value;
			}
		}

		/// <summary>
		/// Gets or sets the look and feel of the ButtonAdv. Set <see cref="UseVisualStyle"/> 
		/// to <c>True</c> if you want to apply style on button, otherwise <c>False</c>.
		/// </summary>
		[
		Browsable( true ),
		Category( "Appearance - Styles" ),
		RefreshProperties( RefreshProperties.Repaint ),
		DefaultValue( ButtonAppearance.Classic ),
		Description( "Indicates the look and feel of the ButtonAdv. Set UseVisualStyle property " +
			"to True if you want to apply style on button." ),
		AmbientValue( ButtonAppearance.None )
		]
		public ButtonAppearance Appearance
		{
			get
			{
				if( m_appearance != ButtonAppearance.None )
				{
					return m_appearance;
				}

				object value;
				if( AmbientHelper.GetAmbientValue( this.Parent, "Appearance", typeof( ButtonAppearance ), out value ) )
				{
					ButtonAppearance tempAppearance = (ButtonAppearance)value;
					return tempAppearance;
				}

				return GetButtonAppearance();
			}
			set
			{
				if( m_appearance != value || m_renderer == null )
				{
					m_appearance = value;

					SetRenderer();

                    if (m_renderer is Office2007ButtonRenderer)
                        m_renderer.SetColorScheme(m_colorScheme);
                    else if (m_renderer is MetroButtonRenderer)
                    {
                        m_renderer.SetMetroColor(m_metroColor);                       

						if (this is ColorPickerButton)
						{
							(this as ColorPickerButton).ColorUI.VisualStyle = ColorUIStyle.Metro;
							(this as ColorPickerButton).ColorUI.MetroColor = this.BackColor;
							(this as ColorPickerButton).ColorUI.MetroForeColor = this.ForeColor;
						}
					}
					this.Refresh();
					this.OnButtonChanged( EventArgs.Empty );
				}
			}
		}

		/// <summary>
		/// Gets or sets the foreground color of the ButtonAdv
		/// </summary>
		[
		Browsable( true ),
		Category( "Appearance" ),
		RefreshProperties( RefreshProperties.Repaint ),
		Description( "Gets or sets the foreground color of the control." )
		]
		public override Color ForeColor
		{
			get
			{
				if( m_foreColor != Color.Empty )
				{
					return m_foreColor;
				}

				return base.ForeColor;
			}
			set
			{
				if( value != m_foreColor )
				{
					m_foreColor = value;
					base.OnForeColorChanged( EventArgs.Empty );
					this.Invalidate();
					if (this is ColorPickerButton)
					{
						(this as ColorPickerButton).ColorUI.MetroForeColor = this.ForeColor;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the background color of the ButtonAdv
		/// </summary>
		[
		Browsable( true ),
		Category( "Appearance" ),
		RefreshProperties( RefreshProperties.Repaint ),
		Description( "Gets or sets the background color of the control." )
		]
		public override Color BackColor
		{
			get
			{
				if( m_backColor != Color.Empty )
				{
					return m_backColor;
				}

				return base.BackColor;
			}
			set
			{
				if( value != m_backColor )
				{
					m_backColor = value;
					base.OnBackColorChanged( EventArgs.Empty );
					this.Invalidate();
					if (this is ColorPickerButton)
					{
						(this as ColorPickerButton).ColorUI.MetroColor = this.BackColor;
					}
				}
			}
		}
        /// <summary>
        /// Gets or sets the ImageListAdv of the ButtonAdv
        /// </summary>
        [
        Browsable(true), DefaultValue(typeof(Color), ""),
        Category("Appearance"),
        Description("Gets or sets the ImageListAdv of the ButtonAdv.")
        ]
        private ImageListAdv imageListAdv = new ImageListAdv();
        public ImageListAdv ImageListAdv
        {
            get {
                     return imageListAdv;
                }
            set { 

                     if(value !=null)
                     this.ImageList = value.ToImageList();
                     imageListAdv = value;
                }
        }

        /// <summary>
        /// Gets or sets the background color of the ButtonAdv
        /// </summary>
        [
        Browsable(true), DefaultValue(typeof(Color), ""),
        Category("Appearance"), RefreshProperties(RefreshProperties.Repaint), 
        Description("Gets or sets the custom managed color of the control.")
        ]
        public Color CustomManagedColor
        {
            get
            {
                return m_customColor;
            }
            set
            {
                if (value != m_customColor)
                {
                    m_customColor = value;
                  
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the custom managed color is to be applied by overiding the Form's managed color
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        RefreshProperties(RefreshProperties.Repaint),
        Description("Indicates whether the custom managed color is to be applied by overiding the Form's managed color"),
        DefaultValue(false)
        ]
        public bool OverrideFormManagedColor
        {
            get
            {
                return m_overrideManagedColor;
            }
            set
            {
                if(value != m_overrideManagedColor)
                {
                     m_overrideManagedColor = value;
                }
               

            }
        }
		/// <summary>
		/// Indicates whether Visual Styles must be enabled for the button.
		/// Set value to <c>True</c> if you want to apply settings of properties:
		/// <see cref="Appearance"/>, <see cref="BorderStyleAdv"/>, <see cref="ButtonType"/>
		/// and etc. on current button.
		/// </summary>
		[
		Browsable( true ),
		Category( "Appearance - Styles" ),
		RefreshProperties( RefreshProperties.Repaint ),
		Description( "Indicates whether Visual Styles must be enabled for the button. Set " +
			"value to True if you want to apply settings of properties: Appearance, " +
			"BorderStyleAdv, ButtonType and etc. on current button." )
		]
		public virtual bool UseVisualStyle
		{
			get
			{
				if( m_useStyle == UseStyle.Inherited )
				{
					object value;
					if( AmbientHelper.GetAmbientValue( this.Parent, "UseVisualStyle", typeof( bool ), out value ) )
					{
						bool tempUseStyle = (bool)value;
						if( m_renderer == null )
						{
							SetRenderer();
						}
						return tempUseStyle;
					}
				}

				return (m_useStyle == UseStyle.True && this.FlatStyle != FlatStyle.System);
			}
			set
			{
				if( this.FlatStyle != FlatStyle.System )
				{
					m_useStyle = (value) ? UseStyle.True : UseStyle.False;

					if( m_renderer == null )
					{
						SetRenderer();
					}


					SetRegion();

					this.Refresh();
				}

			}
		}

		/// <summary>
		/// Gets or sets the type of button to be used.
		/// </summary>
		[
		Category( "Appearance - Styles" ),
		Browsable( true ),
		RefreshProperties( RefreshProperties.Repaint ),
		DefaultValue( ButtonTypes.Normal ),
		Description( "Indicates the type of button to be used." )
		]
		public virtual ButtonTypes ButtonType
		{
			get
			{
				return m_buttonImageType;
			}
			set
			{
				if( m_buttonImageType != value )
				{
					m_buttonImageType = value;

					if( value != ButtonTypes.Normal )
					{
						this.ImageAlign = ContentAlignment.MiddleCenter;
					}

					this.Refresh();
				}
			}
		}
		/// <summary>
		/// Gets or sets the text to be displayed on the button.
		/// </summary>
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
				this.Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the different states the button can hold: Default, MouseOver, Pressed.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public ButtonAdvState State
		{
			get
			{
				return m_state;
			}
			set
			{
				if( m_state != value )
				{
					m_state = value;

					if( this.PushButton )
					{
						if( m_state == ButtonAdvState.Pressed )
						{
							m_pushed = true;
						}
						else if( m_state == ButtonAdvState.Default )
						{
							m_pushed = false;
						}
					}

					Invalidate();
				}
			}
		}

		/// <summary>
		/// Indicates the state of control.
		/// </summary>
		[
		Category( "Appearance - Styles" ),
		Browsable( true ),
		RefreshProperties( RefreshProperties.Repaint ),
		DefaultValue( false ),
		Description( "Indicates the state of control." )
		]
		public bool PushButton
		{
			get
			{
				return m_bPushable;
			}
			set
			{
				m_bPushable = value;
				this.State = ButtonAdvState.Default;
				this.Invalidate();
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether state should be reset on focus lost.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if reset state on lost focus; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(true), Category( "Appearance - Styles" ),
        Description("Indicates whether state should be reset on focus lost.")]
        public bool ResetStateOnLostFocus
        {
            get 
            {
                return resetStateOnLostFocus;
            }
            set
            {
                resetStateOnLostFocus = value;
            }
        }

		/// <summary>
		/// Indicates whether the mouse is currently pressed.
		/// </summary>
		[Browsable( false ),
		 DefaultValue( false )]
		public bool IsMouseDown
		{
			get
			{
				return m_bIsDown;
			}
			set
			{
				m_bIsDown = value;
			}
		}

		/// <summary>
		/// Indicates whether the button is of ComboButton type.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public bool IsComboButton
		{
			get
			{
				return m_isComboButton;
			}
		}
		/// <summary>
		/// Gets or sets the combo edit backcolor.
		/// </summary>
		[
		Category( "Appearance - Styles" ),
		Browsable( true ),
		RefreshProperties( RefreshProperties.Repaint ),
		Description( "Indicates the combo edit backcolor." )
		]
		public Color ComboEditBackColor
		{
			get
			{
				return m_comboEditBackColor;
			}

			set
			{
				m_comboEditBackColor = value;
			}
		}
		#endregion

		#region Class events
		/// <summary>
		/// Indicates whether Appearance of the ButtonAdv has changed.
		/// </summary>
		[
		Category( "Property Changed" ),
		Description( "Indicates whether Appearance of the ButtonAdv has changed." )
		]
		public event EventHandler ButtonChanged;
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary></summary>
		public ButtonAdv()
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(ButtonAdv));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			InitializeProperties();
			Application.AddMessageFilter(this);
			DrawBorderDelegateRoutine();

		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="isComboButton">Indicates whether button is of combo button type.</param>
		public ButtonAdv( bool isComboButton )
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(MaskedEditBox));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

			InitializeComponent();

			InitializeProperties();

			m_isComboButton = isComboButton;
		}		

		/// <summary></summary>
		private void InitializeProperties()
		{
			this.SetStyle( ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
				WhidbeyCompatibleControlStyles.DoubleBuffer, true );

			base.TextAlign = ContentAlignment.MiddleCenter;
			base.ImageAlign = ContentAlignment.MiddleCenter;
            CTRLSIZE = this.Size;
		}

		/// <summary> 
		/// Cleans up any resources being used.
		/// </summary>
		/// <param name="disposing"/>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( m_renderer != null )
				{
					m_renderer.Dispose();
				}

				--s_nHashDelegates;

				if( s_nHashDelegates == 0 )
				{
					s_hashDelegates.Clear();
					s_hashDelegates = null;
				}
			}
			Application.RemoveMessageFilter(this);
			base.Dispose( disposing );
		}
		#endregion

        #region For Touch

        bool isScaling = false;

        bool _touchMode = false;
        /// <summary>
        /// gets or sets the touchmode
        /// </summary>
        [DefaultValue(false)]
        public virtual bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }

        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }
        /// <summary>
        /// applies the scaling
        /// </summary>
        /// <param name="scaleFactor"></param>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;

            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }
        /// <summary>
        /// Font changed event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
        #endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		#region Control Paint logic
		/// <summary>
		/// Indicates whether to animate the image.
		/// </summary>
		/// <param name="flag"></param>
		/// <returns></returns>
		private bool GetFlag( int flag )
		{
			return ((m_animateState & flag) == flag);
		}

		/// <summary>
		/// Sets the value of animateState member.
		/// </summary>
		/// <param name="flag"></param>
		/// <param name="value"></param>
		private void SetFlag( int flag, bool value )
		{
			bool flag1 = (m_animateState & flag) != 0;

			if( value )
			{
				m_animateState |= flag;
			}
			else
			{
				m_animateState &= ~flag;
			}
		}

		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFrameChanged( object sender, EventArgs e )
		{
			if( base.InvokeRequired )
			{
				base.BeginInvoke( new EventHandler( this.OnFrameChanged ), new object[] { sender, e } );
			}
			else
			{
				base.Invalidate();
			}
		}

		/// <summary>
		/// If animate is true - animates the image, else doesn`t animate.
		/// </summary>
		/// <param name="animate"></param>
		private void Animate( bool animate )
		{
			if( animate != this.GetFlag( 0x10 ) )
			{
				if( animate )
				{
					if( this.Image != null )
					{
						ImageAnimator.Animate( this.Image, new EventHandler( this.OnFrameChanged ) );
						SetFlag( 0x10, animate );
					}
				}
				else if( this.Image != null )
				{
					ImageAnimator.StopAnimate( this.Image, new EventHandler( this.OnFrameChanged ) );
					SetFlag( 0x10, animate );
				}
			}
		}

		/// <summary></summary>
		private void Animate()
		{
			this.Animate( !base.DesignMode && base.Visible && base.Enabled );
		}

		/// <summary></summary>
		private void StopAnimate()
		{
			this.Animate( false );
		}
        /// <summary>
        /// finding the button is BackStageButton or not.
        /// </summary>
        private bool isBackStageButton = false;
        /// <summary>
        /// Gets/Sets the value for IsBackStageButton
        /// </summary>
        [Browsable(false)]
        public bool IsBackStageButton
        {
            get
            {
                return isBackStageButton;
            }
            set
            {
                isBackStageButton = value;
            }
        }
		/// <summary></summary>
		/// <param name="e"></param>
		protected override void OnPaint( PaintEventArgs e )
		{
			if( this.UseVisualStyle )
			{
				if( m_appearance == ButtonAppearance.None )
				{
					SetRenderer();
				}

				Animate();

				ImageAnimator.UpdateFrames();

				// Draw border
				int nBorderWidth = DrawBorder( e );

				// Inflate drawing rect and show button.
				Rectangle rectClient = this.ClientRectangle;
                if (this.Appearance != ButtonAppearance.Metro)
                m_renderer.SetBounds(rectClient);
                else
                    m_renderer.SetBounds(Rectangle.Inflate(rectClient, 0, +1));
				m_renderer.Render( e.Graphics );

                if(this.Appearance != ButtonAppearance.Metro )
				DrawFocus( e.Graphics );

				// don't forget to call user custom drawing event
				RaisePaintEvent( e );

			}
			else
			{
				base.OnPaint( e );
			}
		}

		/// <summary></summary>
		/// <param name="g"></param>
		private void DrawFocus( Graphics g )
		{
			if( this.KeepFocusRectangle && this.Focused )
			{
				Rectangle focusRect = this.ClientRectangle;
				focusRect.Inflate( -4, -4 );
				ControlPaint.DrawFocusRectangle( g, focusRect );
			}
		}

		/// <summary>
		/// Creates delegates for border drawing methods.
		/// </summary>
		private static void DrawBorderDelegateRoutine()
		{
			++s_nHashDelegates;

			if( s_hashDelegates == null )
			{
				s_hashDelegates = new Hashtable();

				s_hashDelegates.Add( ButtonAdvBorderStyle.None, new DrawBorderEventHandler( DrawNoBorder ) );
				s_hashDelegates.Add( ButtonAdvBorderStyle.Default, new DrawBorderEventHandler( DrawNoBorder ) );
				s_hashDelegates.Add( ButtonAdvBorderStyle.Dashed, new DrawBorderEventHandler( DrawDashedBorder ) );
				s_hashDelegates.Add( ButtonAdvBorderStyle.Dotted, new DrawBorderEventHandler( DrawDottedBorder ) );
				s_hashDelegates.Add( ButtonAdvBorderStyle.Solid, new DrawBorderEventHandler( DrawSolidBorder ) );
				s_hashDelegates.Add( ButtonAdvBorderStyle.Inset, new DrawBorderEventHandler( DrawInsetBorder ) );
				s_hashDelegates.Add( ButtonAdvBorderStyle.Outset, new DrawBorderEventHandler( DrawOutsetBorder ) );
				s_hashDelegates.Add( ButtonAdvBorderStyle.Bump, new DrawBorderEventHandler( DrawBumpBorder ) );
				s_hashDelegates.Add( ButtonAdvBorderStyle.Etched, new DrawBorderEventHandler( DrawEtchedBorder ) );
				s_hashDelegates.Add( ButtonAdvBorderStyle.Raised, new DrawBorderEventHandler( DrawRaisedBorder ) );
				s_hashDelegates.Add( ButtonAdvBorderStyle.RaisedInner, new DrawBorderEventHandler( DrawRaisedInnerBorder ) );
				s_hashDelegates.Add( ButtonAdvBorderStyle.RaisedOuter, new DrawBorderEventHandler( DrawRaisedOuterBorder ) );
				s_hashDelegates.Add( ButtonAdvBorderStyle.Sunken, new DrawBorderEventHandler( DrawSunkenBorder ) );
				s_hashDelegates.Add( ButtonAdvBorderStyle.SunkenInner, new DrawBorderEventHandler( DrawSunkenInnerBorder ) );
				s_hashDelegates.Add( ButtonAdvBorderStyle.SunkenOuter, new DrawBorderEventHandler( DrawSunkenOuterBorder ) );
				s_hashDelegates.Add( ButtonAdvBorderStyle.Flat, new DrawBorderEventHandler( DrawFlatBorder ) );
			}
		}

		/// <summary>
		/// Performs border drawing.
		/// </summary>
		/// <param name="e">PaintEventArgs from OnPaint method</param>
		/// <returns>border width</returns>
		private int DrawBorder( PaintEventArgs e )
		{
			int nBorderWidth = 0;

			// Border drawing is supported only with Office2003ButtonRenderer
			// and OfficeXPButtonRenderer renderers
			if( (m_renderer is Office2003ButtonRenderer) ||
				(m_renderer is OfficeXPButtonRenderer) ||
				(m_renderer is WindowsXPButtonRenderer) )
			{
				using( Pen pen = new Pen( SystemColors.Control ) )
				{
					Rectangle rect = new Rectangle( ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1 );
					e.Graphics.DrawRectangle( pen, rect );
					rect.Inflate( -1, -1 );
					e.Graphics.DrawRectangle( pen, rect );
				}

				DrawBorderDelegateRoutine();

				DrawBorderEventHandler evtHandler = s_hashDelegates[BorderStyleAdv] as DrawBorderEventHandler;

				if( evtHandler != null )
				{
					nBorderWidth = evtHandler( e.Graphics, e.ClipRectangle, GetBorderColor() );
				}
			}

			return nBorderWidth;
		}

		/// <summary>
		/// Draws border with ButtonAdvBorderStyle.Flat style.
		/// </summary>
		/// <param name="g">Graphics to draw on</param>
		/// <param name="rectClient">rectangle to draw border in</param>
		/// <returns>border width</returns>
		private static int DrawFlatBorder( Graphics g, Rectangle rectClient, Color clrBorder )
		{
			ControlPaint.DrawBorder3D( g, rectClient, Border3DStyle.Flat );

			return SystemInformation.Border3DSize.Width;
		}

		/// <summary>
		/// Draws border with ButtonAdvBorderStyle.SunkenOuter style.
		/// </summary>
		/// <param name="g">Graphics to draw on</param>
		/// <param name="rectClient">rectangle to draw border in</param>
		/// <returns>border width</returns>
		private static int DrawSunkenOuterBorder( Graphics g, Rectangle rectClient, Color clrBorder )
		{
			ControlPaint.DrawBorder3D( g, rectClient, Border3DStyle.RaisedOuter );

			return 2;
		}

		/// <summary>
		/// Draws border with ButtonAdvBorderStyle.SunkenInner style.
		/// </summary>
		/// <param name="g">Graphics to draw on</param>
		/// <param name="rectClient">rectangle to draw border in</param>
		/// <returns>border width</returns>
		private static int DrawSunkenInnerBorder( Graphics g, Rectangle rectClient, Color clrBorder )
		{
			ControlPaint.DrawBorder3D( g, rectClient, Border3DStyle.RaisedInner );

			return SystemInformation.Border3DSize.Width;
		}

		/// <summary>
		/// Draws border with ButtonAdvBorderStyle.Sunken style.
		/// </summary>
		/// <param name="g">Graphics to draw on</param>
		/// <param name="rectClient">rectangle to draw border in</param>
		/// <returns>border width</returns>
		private static int DrawSunkenBorder( Graphics g, Rectangle rectClient, Color clrBorder )
		{
			ControlPaint.DrawBorder3D( g, rectClient, Border3DStyle.Raised );

			return SystemInformation.Border3DSize.Width;
		}

		/// <summary>
		/// Draws border with ButtonAdvBorderStyle.RaisedOuter style.
		/// </summary>
		/// <param name="g">Graphics to draw on</param>
		/// <param name="rectClient">rectangle to draw border in</param>
		/// <returns>border width</returns>
		private static int DrawRaisedOuterBorder( Graphics g, Rectangle rectClient, Color clrBorder )
		{
			ControlPaint.DrawBorder3D( g, rectClient, Border3DStyle.RaisedOuter );

			return SystemInformation.Border3DSize.Width;
		}

		/// <summary>
		/// Draws border with ButtonAdvBorderStyle.RaisedInner style.
		/// </summary>
		/// <param name="g">Graphics to draw on</param>
		/// <param name="rectClient">rectangle to draw border in</param>
		/// <returns>border width</returns>
		private static int DrawRaisedInnerBorder( Graphics g, Rectangle rectClient, Color clrBorder )
		{
			ControlPaint.DrawBorder3D( g, rectClient, Border3DStyle.RaisedInner );

			return SystemInformation.Border3DSize.Width;
		}

		/// <summary>
		/// Draws border with ButtonAdvBorderStyle.Raised style.
		/// </summary>
		/// <param name="g">Graphics to draw on</param>
		/// <param name="rectClient">rectangle to draw border in</param>
		/// <returns>border width</returns>
		private static int DrawRaisedBorder( Graphics g, Rectangle rectClient, Color clrBorder )
		{
			ControlPaint.DrawBorder3D( g, rectClient, Border3DStyle.Raised );

			return SystemInformation.Border3DSize.Width;
		}

		/// <summary>
		/// Draws border with ButtonAdvBorderStyle.Etched style.
		/// </summary>
		/// <param name="g">Graphics to draw on</param>
		/// <param name="rectClient">rectangle to draw border in</param>
		/// <returns>border width</returns>
		private static int DrawEtchedBorder( Graphics g, Rectangle rectClient, Color clrBorder )
		{
			ControlPaint.DrawBorder3D( g, rectClient, Border3DStyle.Etched );

			return SystemInformation.Border3DSize.Width;
		}

		/// <summary>
		/// Draws border with ButtonAdvBorderStyle.Bump style.
		/// </summary>
		/// <param name="g">Graphics to draw on</param>
		/// <param name="rectClient">rectangle to draw border in</param>
		/// <returns>border width</returns>
		private static int DrawBumpBorder( Graphics g, Rectangle rectClient, Color clrBorder )
		{
			ControlPaint.DrawBorder3D( g, rectClient, Border3DStyle.Bump );

			return SystemInformation.Border3DSize.Width;
		}

		/// <summary>
		/// Draws border with ButtonAdvBorderStyle.Outset style.
		/// </summary>
		/// <param name="g">Graphics to draw on</param>
		/// <param name="rectClient">rectangle to draw border in</param>
		/// <returns>border width</returns>
		private static int DrawOutsetBorder( Graphics g, Rectangle rectClient, Color clrBorder )
		{
			ControlPaint.DrawBorder( g, rectClient, clrBorder, ButtonBorderStyle.Outset );

			return c_nDEFAULT_BORDER_WIDTH;
		}

		/// <summary>
		/// Draws border with ButtonAdvBorderStyle.Inset style.
		/// </summary>
		/// <param name="g">Graphics to draw on</param>
		/// <param name="rectClient">rectangle to draw border in</param>
		/// <returns>border width</returns>
		private static int DrawInsetBorder( Graphics g, Rectangle rectClient, Color clrBorder )
		{
			ControlPaint.DrawBorder( g, rectClient, clrBorder, ButtonBorderStyle.Inset );

			return c_nDEFAULT_BORDER_WIDTH;
		}

		/// <summary>
		/// Draws border with ButtonAdvBorderStyle.Solid style.
		/// </summary>
		/// <param name="g">Graphics to draw on</param>
		/// <param name="rectClient">rectangle to draw border in</param>
		/// <returns>border width</returns>
		private static int DrawSolidBorder( Graphics g, Rectangle rectClient, Color clrBorder )
		{
			ControlPaint.DrawBorder( g, rectClient, clrBorder, ButtonBorderStyle.Solid );

			return c_nDEFAULT_BORDER_WIDTH;
		}

		/// <summary>
		/// Draws border with ButtonAdvBorderStyle.Dotted style.
		/// </summary>
		/// <param name="g">Graphics to draw on</param>
		/// <param name="rectClient">rectangle to draw border in</param>
		/// <returns>border width</returns>
		private static int DrawDottedBorder( Graphics g, Rectangle rectClient, Color clrBorder )
		{
			ControlPaint.DrawBorder( g, rectClient, clrBorder, ButtonBorderStyle.Dotted );

			return c_nDEFAULT_BORDER_WIDTH;
		}

		/// <summary>
		/// Draws border with ButtonAdvBorderStyle.Dashed style.
		/// </summary>
		/// <param name="g">Graphics to draw on</param>
		/// <param name="rectClient">rectangle to draw border in</param>
		/// <returns>border width</returns>
		private static int DrawDashedBorder( Graphics g, Rectangle rectClient, Color clrBorder )
		{
			ControlPaint.DrawBorder( g, rectClient, clrBorder, ButtonBorderStyle.Dashed );

			return c_nDEFAULT_BORDER_WIDTH;
		}

		/// <summary>
		/// This method draws nothing. Used with ButtonAdvBorderStyle.Default and ButtonAdvBorderStyle.None
		/// </summary>
		/// <param name="g">Graphics to draw on</param>
		/// <param name="rectClient">rectangle to draw border in</param>
		/// <returns>border width</returns>
		private static int DrawNoBorder( Graphics g, Rectangle rectClient, Color clrBorder )
		{
			return 0;
		}

		/// <summary>
		/// Gets color to draw border with.
		/// </summary>
		/// <returns>border color</returns>
		private Color GetBorderColor()
		{
			Color clrBorder;

			if( m_renderer is Office2003ButtonRenderer )
			{
				clrBorder = ButtonOffice2003Colors.FocusBorderColor;
			}
			else
			{
				clrBorder = ButtonOfficeXPColors.BorderColor;
			}

			return clrBorder;
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Recover or suspends mouse handling logic for property <see cref="State"/>. 
		/// </summary>
		/// <param name="ignoreMouse">
		/// True to suspend, otherwise to resume handling logic.
		/// </param>
		[Obsolete( "Do not use this method. Instead use methods pair SuspendMouseState, ResumeMouseState." )]
		[EditorBrowsable( EditorBrowsableState.Never )]
		public void SetIgnoreMouse( bool ignoreMouse )
		{
			if( m_bIgnoreMouse != ignoreMouse )
			{
				m_bIgnoreMouse = ignoreMouse;
			}
		}

		/// <summary>
		/// Suspend <see cref="State"/> property updating till the <see cref="ResumeMouseState"/> method call.
		/// </summary>
		public void SuspendMouseState()
		{
			m_bIgnoreMouse = true;
		}

		/// <summary>
		/// Recover mouse handling logic for property <see cref="State"/>. To suspend call <see cref="SuspendMouseState"/> method.
		/// </summary>
		public void ResumeMouseState()
		{
			m_bIgnoreMouse = false;
		}

		/// <summary>
		/// If the isLastleftButton is set to True, the button's renderer could 
		/// draw the background for half of the control differently. Used in XP style 
		/// when used as a combo button.
		/// </summary>
		/// <remarks>Used only for by <see cref="WindowsXPButtonRenderer"/> class.</remarks>
		/// <param name="val">The value that is to be set.</param>
		public void SetIsLastLeftButton( bool val )
		{
			this.isLastLeftButton = val;
		}

		/// <summary>
		/// If the IsFirstRightButton is set to True, the button's renderer could 
		/// draw the background for half of the control differently. Used in XP style 
		/// when used as a combo button.
		/// </summary>
		/// <remarks>Used only for by <see cref="WindowsXPButtonRenderer"/> class.</remarks>
		/// <param name="val">The value that is to be set.</param>
		public void SetIsFirstRightButton( bool val )
		{
			this.isFirstRightButton = val;
		}

		/// <summary>
		/// If the isLastleftButton is set to True, the button's renderer could 
		/// draw the background for half of the control differently. Used in XP style 
		/// when used as a combo button.
		/// </summary>
		/// <remarks>Used only for by <see cref="WindowsXPButtonRenderer"/> class.</remarks>
		/// <returns></returns>
		protected internal bool GetIsLastLeftButton()
		{
			return this.isLastLeftButton;
		}

		/// <summary>
		/// If the IsFirstRightButton is set to True, the button's renderer could 
		/// draw the background for half of the control differently. Used in XP style 
		/// when used as a combo button.
		/// </summary>
		/// <remarks>Used only for by <see cref="WindowsXPButtonRenderer"/> class.</remarks>
		/// <returns></returns>
		protected internal bool GetIsFirstRightButton()
		{
			return this.isFirstRightButton;
		}

		#endregion

		#region Class overrides
		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnSystemColorsChanged( EventArgs e )
		{
			switch( Appearance )
			{
				case ButtonAppearance.WindowsXP:
				m_renderer = new WindowsXPButtonRenderer( this );
				(m_renderer as WindowsXPButtonRenderer).SetColorScheme();
				break;

			}
			base.OnSystemColorsChanged( e );
		}
        /// <summary>
        /// Gets/Sets Control size before touch enabled
        /// </summary>
        [Browsable(false)]
        public Size BeforeTouchSize
        {
            get
            {
                return CTRLSIZE;
            }
            set
            {
                CTRLSIZE = value;
            }
        }
		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnSizeChanged( EventArgs e )
		{
			this.Invalidate();
			base.OnSizeChanged( e );
            if (!EnableTouchMode && this.DesignMode)
            {
                CTRLSIZE = this.Size;
            }
			SetRegion();
		}

		/// <summary></summary>
		/// <param name="e"/>
		protected virtual void OnButtonChanged( EventArgs e )
		{
			if( this.ButtonChanged != null )
			{
				this.ButtonChanged( this, e );
			}
		}

		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnClick( EventArgs e )
		{
			if( this.PushButton )
			{
				m_pushed = !m_pushed;

				if( m_pushed )
				{
					this.State &= ~ButtonAdvState.Default;
					this.State |= ButtonAdvState.Pressed;
				}
				else
				{
					this.State &= ~ButtonAdvState.Pressed;
					this.State |= ButtonAdvState.Default;
				}

				this.State &= ~ButtonAdvState.MouseOver;
			}

			this.IsMouseDown = true;
			this.Invalidate();
			base.OnClick( e );
			this.IsMouseDown = false;
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnMouseEnter( EventArgs e )
		{
			if( m_bIgnoreMouse == false )
			{
                if (this.Enabled && this.Appearance == ButtonAppearance.Metro)
                {
                    this.State &= ~ButtonAdvState.Inactive;
                }
                    this.State &= ~ButtonAdvState.Default;
                    this.State |= ButtonAdvState.MouseOver;
                
			}

			this.Refresh();
			base.OnMouseEnter( e );
		}

		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnMouseLeave( EventArgs e )
		{
            if (m_bIgnoreMouse == false)
            {
                this.State &= ~ButtonAdvState.MouseOver;
                this.State |= ButtonAdvState.Default;

            }

			this.Invalidate();
			base.OnMouseLeave( e );
		}

        protected override void OnLostFocus(EventArgs e)
        {
            if (resetStateOnLostFocus)
            {
                this.State = ButtonAdvState.All;
                this.Invalidate();
            }
            base.OnLostFocus(e);
        }

		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnMouseDown( MouseEventArgs e )
		{
			bool mouseInside = MousePointInside();

			if( mouseInside == false )
			{
				this.State &= ~ButtonAdvState.MouseOver;
				this.State |= ButtonAdvState.Default;
				this.IsMouseDown = false;
			}
			else
			{
				if( m_bIgnoreMouse == false )
				{
					if( (e.Button & MouseButtons.Left) == MouseButtons.Left )
					{
						if( !m_pushed )
						{
							State |= ButtonAdvState.Pressed;
						}
                        else
                            this.State &= ~ButtonAdvState.Pressed;
					}
				}

				this.IsMouseDown = true;
			}

			this.Refresh();
			base.OnMouseDown( e );
		}

		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnMouseMove( MouseEventArgs e )
		{
			bool mouseInside = MousePointInside();

			if( mouseInside == false )
			{
				this.State &= ~ButtonAdvState.MouseOver;
				this.State |= ButtonAdvState.Default;
			}

			base.OnMouseMove( e );
		}

		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnMouseUp( MouseEventArgs e )
		{
			bool mouseInside = MousePointInside();

			if( mouseInside == false )
			{
				this.State &= ~ButtonAdvState.MouseOver;
				this.State |= ButtonAdvState.Default;
			}
			else
			{
				if( m_bIgnoreMouse == false )
				{
					if( (e.Button & MouseButtons.Left) == MouseButtons.Left )
					{
						if( m_pushed || !this.PushButton )
						{
							State &= ~ButtonAdvState.Pressed;
						}

						State |= ButtonAdvState.MouseOver;

						this.IsMouseDown = false;
					}
				}
			}

			this.Refresh();
			base.OnMouseUp( e );
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.HandleCreated"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnHandleCreated( EventArgs e )
		{
			base.OnHandleCreated( e );

			if( m_weakThemeChanged == null )
			{
				m_weakThemeChanged = new XPThemesThemeChangedWeakContainer( this );
			}
		}

		protected override void OnHandleDestroyed( EventArgs e )
		{
			m_weakThemeChanged.Target = null;
			m_weakThemeChanged = null;

			base.OnHandleDestroyed( e );
		}

		/// <summary>
		/// Raises the <see cref="M:System.Windows.Forms.ButtonBase.OnKeyUp(System.Windows.Forms.KeyEventArgs)"/> event.
		/// </summary>
		/// <param name="kevent">A <see cref="T:System.Windows.Forms.KeyEventArgs"/> that contains the event data.</param>
		protected override void OnKeyDown( KeyEventArgs kevent )
		{
			base.OnKeyDown( kevent );

			if( kevent.KeyData == Keys.Space )
			{
				this.State |= ButtonAdvState.Pressed;
				this.Invalidate();
			}
		}

		/// <summary>
		/// Raises the <see cref="M:System.Windows.Forms.ButtonBase.OnKeyUp(System.Windows.Forms.KeyEventArgs)"/> event.
		/// </summary>
		/// <param name="kevent">A <see cref="T:System.Windows.Forms.KeyEventArgs"/> that contains the event data.</param>
		protected override void OnKeyUp( KeyEventArgs kevent )
		{
			if( (kevent.KeyCode == Keys.Return) || (kevent.KeyCode == Keys.Space) )
			{
				if( (this.State & ButtonAdvState.Pressed) != 0 )
				{
					this.State &= ~ButtonAdvState.Pressed;
				}
			}

			base.OnKeyUp( kevent );
		}

		#endregion

		#region Class utility methods
		/// <summary>
		/// Resets the UseVisualStyle to default.
		/// </summary>
		protected virtual void ResetUseVisualStyle()
		{
			m_useStyle = UseStyle.Inherited;
		}

		/// <summary>
		/// </summary>
		protected virtual bool ShouldSerializeUseVisualStyle()
		{
			return m_useStyle != UseStyle.Inherited;
		}

		/// <summary>
		/// Resets the ComboEditBackColor to default.
		/// </summary>
		protected virtual void ResetComboEditBackColor()
		{
			ComboEditBackColor = Color.Empty;
		}

		/// <summary>
		/// </summary>
		protected virtual bool ShouldSerializeComboEditBackColor()
		{
			return ComboEditBackColor != Color.Empty;
		}
        private bool ShouldSerailizeIsBackStageButton()
        {
            return isBackStageButton != false;
        }
        private void ResetIsBackStageButton()
        {
            this.isBackStageButton = false;
        }
		/// <summary>
		/// Resets the System.Windows.Forms.Control.ForeColor property to its default value.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		public override void ResetForeColor()
		{
			ForeColor = Color.Empty;
		}

		/// <summary></summary>
		protected internal virtual bool ShouldSerializeForeColor()
		{
			return m_foreColor != Color.Empty;
		}

		/// <summary>
		/// Resets the System.Windows.Forms.Control.BackColor property to its default value.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		public override void ResetBackColor()
		{
			BackColor = Color.Empty;
		}
        /// <summary>
        /// Should serialize for IsBackStageButton.
        /// </summary>
        bool ShouldSerialize()
        {
            return IsBackStageButton != false;
        }
		/// <summary>
		/// Indicates whether the <see cref="BackColor"/> property should be persisted.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		public virtual bool ShouldSerializeBackColor()
		{
			return m_backColor != Color.Empty;
		}

		/// <summary>Method allow to raise <see cref="Control.Paint"/> event.</summary>
		/// <param name="e">Paint event arguments.</param>
		protected void RaisePaintEvent( PaintEventArgs e )
		{
			PaintEventHandler handler = this.Events[ControlEventsHelper.EventPaint] as PaintEventHandler;

			if( handler != null )
			{
				handler( this, e );
			}
		}

		/// <summary>
		/// </summary>
		private void SetRenderer()
		{
			// don't forget to dispose old renderer first
			if( m_renderer != null )
				m_renderer.Dispose();

			if( m_cachedAppearance != this.Appearance )
			{
                this.ForeColor = Color.Empty;
				switch( this.Appearance )
				{
					case ButtonAppearance.Classic:
					m_renderer = new ClassicButtonRenderer( this );
					break;
					case ButtonAppearance.Office2000:
					m_renderer = new Office2000ButtonRenderer( this );
					break;
					case ButtonAppearance.OfficeXP:
					m_renderer = new OfficeXPButtonRenderer( this );
					break;
					case ButtonAppearance.WindowsXP:
					m_renderer = new WindowsXPButtonRenderer( this );
					break;
					case ButtonAppearance.Office2003:
					m_renderer = new Office2003ButtonRenderer( this );
					break;
					case ButtonAppearance.Office2007:
					m_renderer = new Office2007ButtonRenderer( this );
					break;
                    case ButtonAppearance.Office2010:
                    m_renderer = new Office2010ButtonRenderer(this);
                    break;
                    case ButtonAppearance.Metro :
                    m_renderer = new MetroButtonRenderer(this);
                    break;
				}

				SetRegion();
			}

			m_cachedAppearance = this.Appearance;
		}

		/// <summary>
		/// Sets region of the control
		/// </summary>
		private void SetRegion()
		{
			if( UseVisualStyle && m_renderer != null )
				this.Region = m_renderer.GetRegion( this.ClientRectangle );
			else
				this.Region = null;
		}

		/// <summary>
		/// Gets ButtonAppearance depending on current operating system
		/// </summary>
		private ButtonAppearance GetButtonAppearance()
		{
			OperatingSystem osInfo = Environment.OSVersion;

			if( osInfo.Platform == PlatformID.Win32NT && osInfo.Version.Major == 5 )
			{
				if( osInfo.Version.Minor != 0 ) //Windows XP
				{
					if( NativeMethods.IsThemeActive() )
						return ButtonAppearance.WindowsXP;
				}
			}

			return ButtonAppearance.Classic;
		}

		/// <summary></summary>
		/// <returns></returns>
		private bool MousePointInside()
		{
			if( !this.IsDisposed )
			{
				Rectangle screenBounds = this.RectangleToScreen( this.ClientRectangle );
				Point mousePt = Control.MousePosition;

				return (this.Visible || screenBounds.Contains( mousePt ));
			}

			return false;
		}
		#endregion

		#region Class internal declarations
		/// <summary>
		/// Delegate used for border drawing.
		/// </summary>
		/// <returns></returns>
		/// <param name="g"/>
		/// <param name="rectClient"/>
		private delegate int DrawBorderEventHandler( Graphics g, Rectangle rectClient, Color clrBorder );
		#endregion

		#region ISupportOffice2007Theme Members

		Office2007Theme ISupportOffice2007Theme.Office2007ColorTheme
		{
			get
			{
				return this.Office2007ColorScheme;
			}
			set
			{
				this.Office2007ColorScheme = value;
			}
		}

		void ISupportOffice2007Theme.EnableOffice2007Style()
		{
			this.Appearance = ButtonAppearance.Office2007;
		}
		#endregion

		#region ISupportThemeChanged Members

		void ISupportThemeChanged.ThemeChanged( object sender, EventArgs e )
		{
			SetRenderer();
			Invalidate();
		}

		#endregion

        #region IMessageFilter overrides
        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x0104:
                    this.altPressed = true;
                    break;
            }
            return false;
        }
        #endregion
    }
}
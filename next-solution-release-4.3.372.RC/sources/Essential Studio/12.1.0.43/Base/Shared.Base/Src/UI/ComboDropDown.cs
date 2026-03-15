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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Design;


namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Specifies the appearance of the <see cref="ComboBoxBase"/> control.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This enumeration is used by the <see cref="ComboDropDown.FlatStyle"/> property.
	/// </para>
	/// <para>
	/// Use <see cref="Syncfusion.Windows.Forms.XPThemes.IsThemedOS"/> and 
	/// <see cref="Syncfusion.Windows.Forms.XPThemes.IsThemeActive"/> to determine
	/// if Themes are on during run-time.
	/// </para>
	/// </remarks>
	public enum ComboFlatStyle
	{
		/// <summary>
		/// The control and the button appear flat.
		/// </summary>
		Flat,
		/// <summary>
		/// The control and the button appear three-dimensional.
		/// </summary>
		Standard,
		/// <summary>
		/// The appearance is determined by the user's operating system. 
		/// </summary>
		/// <remarks>
		/// If XP, then themes will be used to draw this control. For other operating systems,
		/// the behavior will be that of the <see cref="ComboFlatStyle.Standard"/> style. This
		/// setting will also use XP Themes to draw the combo when an XP Theme is loaded in the OS.
		/// </remarks>
		System
	}

	/// <summary>
	/// This class used for additional handling messages, that are sent
	/// to DropDown's child controls.
	/// </summary>
	public class DropDownNativeWindow: NativeWindow
	{
		#region Class members
		/// <summary>
		/// ComboDropDown control, listen to messages for.
		/// </summary>
		private ComboDropDown m_cmb;

		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Default Constructor.
		/// </summary>
		/// <param name="value"></param>
		public DropDownNativeWindow( ComboDropDown value )
		{
			if( value == null )
				throw new ArgumentNullException( "value" );

			m_cmb = value;
			AssignHandle( m_cmb.TextBox.Handle );
		}
		#endregion

		#region Class overrides
		protected override void WndProc( ref Message m )
		{
			if( m.Msg == NativeMethods.WM_IME_COMPOSITION || 
				m.Msg == NativeMethods.WM_IME_CHAR )
			{
				if( ImeMessageReceived != null )
				{
					ImeMessageReceived( this, EventArgs.Empty );
				}
			}

			if( m.Msg == NativeMethods.WM_KEYDOWN ||
				m.Msg == NativeMethods.WM_CHAR || 
				m.Msg == NativeMethods.WM_PASTE || 
				m.Msg == NativeMethods.WM_CUT )
			{
				if( m_cmb.PerformAllowNewText( m ) )
					return;
			}

			base.WndProc( ref m );
		}
		#endregion

		#region Class events
		/// <summary>
		/// Fired when IME messages are received
		/// </summary>
		public event EventHandler ImeMessageReceived;
		#endregion
	}
    [Designer(typeof(ComboDropDownDesigner), typeof(System.ComponentModel.Design.IDesigner))]
	[
	Designer( typeof( ComboBoxBaseDesigner ), typeof( System.ComponentModel.Design.IDesigner ) ),
	System.Drawing.ToolboxBitmap( typeof( Syncfusion.Windows.Forms.PopupControlContainer ), "ToolboxIcons.ComboDropDown.bmp" ),
	Description( "Represents advanced combobox control with container control support in this dropdown." )
	]
	public class ComboDropDown:
		Control,
		IThemedControl,
		ISupportInitialize,
		INonClientPaintingSupport,
		ISupportOffice2007Theme,
        IVisualStyle
	//This cannot be a ContainerControl derived, because when databound (in a ComboBoxAdv, you cannot set the 
	// data-bindings in the design-time. Otherwise, there is a crash within InitializeComponent 
	// when ValueMember is changed. This is basically because the ContainerControl.BindingContext
	// override returns a empty binding context (assuming the container control will never be data bound).
	// Also made sure that the control will not get created too early.
	{
		#region Constants
		// Border width
		private const int c_iDefaultBorderWidthVSStyle = 2;
		private const int c_iDefaultBorderWidth = 1;
		private const int c_scrollerWidth = 16;
		private const int c_adjustScrollerPosition = 1;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);
        /// <summary>
        /// User Size changed
        /// </summary>
        private Size USERSIZE = default(Size);
        /// <summary>
        /// Default font style of the control
        /// </summary>
        private Font FONTSTYLE = default(Font);

        /// <summary>
        /// Font which stored after changed in design
        /// </summary>
        private Font USERFONTSTYLE = default(Font);
		#endregion

		#region FIELDS

		/// <summary>
		/// Indicates whether to perform case sensitive search.
		/// </summary>
		private bool m_bCaseSensitive = false;

		private bool m_bIsInImeMode = false;
		private DropDownNativeWindow m_nativeWindow = null;
		protected int m_lastFoundIndex = -1;
		private string m_strTextToSearch = string.Empty;
		protected bool initializing = false;
		/// <summary>
		/// The width of the drop-down button.
		/// </summary>
		//		protected readonly int DropDownButtonWidth = 16;
		internal static int ComboBoxOfficeStyleDropDownWidth = 12;
		private int dropDownButtonHeight = 17;
		private bool preventHeightChange = true;
		internal int editPortionHeight = 0;
		private int dropDownWidth = -1;
		private bool numberOnly = false;
		private bool readOnly = false;
		private ContextMenu cachedContextMenu = null;
		private bool mouseWasUnderChild = false;
		private bool quickSelectOn = false;
		private bool allowQuickSelection = true;
		private bool grayOnReadOnly = true;
		private bool useBackColor = false;
		private Timer quickSelectTimer = null;
		private ComboFlatStyle comboFlatStyle = ComboFlatStyle.Standard;
		private VisualStyle editorStyle = VisualStyle.Default;
		private static Color DefaultComboBackColor = SystemColors.Window;
		internal bool disposing = false;
		private bool comboInFocus = false;
		private string cachedTextBeforePopup = String.Empty;
		private string latestCommittedString = String.Empty;
		internal bool ignoreNextPopupControlMouseMove = false;
		internal bool ignorePopupValueChange = false;
		private Timer selectionTimer = null;
		private bool selectInAFewMilliSecs = false;

		private Color flatBorderColor = Color.Empty;
		private Border3DSide borderSides = Border3DSide.All;
		private Border3DStyle border3DStyle = Border3DStyle.Sunken;
		private ControlDrawing cd;

		private Control popupControl = null;
		private TextBoxExt textBox = null;
		private PopupControlContainer popupContainer = null;

		private DropDownButton ddButton;
		private bool needLayout = false;
		private ThemedEditDrawing themedDrawing = null;
		private ComboBoxStyle comboStyle = ComboBoxStyle.DropDown;
		private bool isActive = false;
		private int inputTextIndex = 0;
		private bool matchFirstCharacterOnly = true;
        private bool textBoxEventsHooked = false;
		/// <summary>
		/// Indicates whether theme BackColor is painted or ignored and BackColor of control is painted.
		/// </summary>
		private bool ignoreThemeBackground = true;
		/// <summary>
		/// Cached BackColor of ComboBox.
		/// </summary>
		private Color cachedBackColor = DefaultComboBackColor;

		internal ComboDropDownWeakContainer comboDropDownWeakContainer = null;
		/// <summary>
        /// Indicates whether to suppress the DropDown Event
		/// </summary>
		private bool suppressdropdownevent = false;
		/// <summary>
		/// Used to draw office 2007 scroller.
		/// </summary>
        private bool useMnemonic = false;
        /// <summary>
        /// Used to draw office 2007 backcolors in ActiveMode when DropDownList style set.
        /// </summary>
        private bool useOffice2007ColorsInActiveMode = false;
        /// <summary>
        /// Used to draw metro backcolors in ActiveMode when DropDownList style set.
        /// </summary>
        private bool useMetroColorsInActiveMode = false;
        /// <summary>
        /// <summary>
        /// Indicates whether to enable or disable the ampersand(&) in the Text.
        /// </summary>
		private ScrollersFrame m_scrollersFrame = null;
		/// <summary>
		/// Office 2007 color scheme.
		/// </summary>
		private Office2007Theme m_office2007ColorTheme = Office2007Theme.Blue;
        /// <summary>
        /// Office 2010 color scheme.
        /// </summary>
        private Office2010Theme m_office2010ColorTheme = Office2010Theme.Blue;
        /// <summary>
        /// Metro color scheme.
        /// </summary>
        private bool allowMouseWheelSelection = true;
        private Point prevMouseLocation = Point.Empty;
        private Color BaseBackColor = DefaultComboBackColor;
        /// <summary>
        /// Default size of the control
        /// </summary>
      




		#endregion FIELDS

		#region INIT
		/// <summary>
		/// Creates a new instance of the ComboDropDown class.
		/// </summary>
		public ComboDropDown()
			: base()
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(ComboDropDown));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
			// Enable double-buffering.		
			this.SetStyle( ControlStyles.Selectable | 
				WhidbeyCompatibleControlStyles.DoubleBuffer |
				ControlStyles.UserPaint |
				ControlStyles.AllPaintingInWmPaint, true );

			this.textBox = new TextBoxExt();
			this.ddButton = this.CreateDropDownButton();
            CTRLSIZE = this.Size;
            USERSIZE = this.Size;
            FONTSTYLE = this.Font;
            USERFONTSTYLE = FONTSTYLE;
			comboDropDownWeakContainer = new ComboDropDownWeakContainer( this );
			MenuColors.MenuColorsChanged += new EventHandler( comboDropDownWeakContainer.MenuColorsChangedWeakEventHandler );
			Office2003Colors.MenuColorsChanged += new EventHandler( comboDropDownWeakContainer.Office2003ColorsChangedWeakEventHandler );
		}

		private void Init()
		{
			this.textBox.BorderStyle = BorderStyle.None;
			this.textBox.Visible = false;
			this.textBox.TabIndex = 0;
			this.Controls.Add( this.textBox );

			// Call this after creating the text box.
			RefreshBackColor( ignoreThemeBackground );

			this.cd = new ControlDrawing();

			// Init Text Box.
			this.InitTextBox();
			// Init DD Button.
			this.InitDropDownButton( this.ddButton );


			m_nativeWindow = new DropDownNativeWindow( this );
			m_nativeWindow.ImeMessageReceived += new EventHandler( m_nativeWindow_ImeMessageReceived );

			// Init PopupControlContainer.
			if( popupContainer == null )
			{
				this.popupContainer = this.CreatePopupContainer();
				this.InitPopupContainer();
			}

			if(!this.DesignMode && m_scrollersFrame == null)
			{
				m_scrollersFrame = CreateScrollersFrame();
			}

			this.quickSelectTimer = new Timer();
			this.quickSelectTimer.Interval = 50;
			this.quickSelectTimer.Tick += new EventHandler( QuickSelectTimer_Tick );

			if( XPThemes.IsThemedOS )
				this.themedDrawing = new ThemedEditDrawing();

			//MenuColors.UpdateMenuColors();
			//Office2003Colors.UpdateMenuColors();
		}

		private void Uninit()
		{
			if( null != ddButton )
			{
				ddButton.MouseDown -= new EventHandler( DDButtonPressed );
			}
            if(this.textBoxEventsHooked)
                this.UnInitTextBox();
			m_nativeWindow.ImeMessageReceived -= new EventHandler( m_nativeWindow_ImeMessageReceived );
			m_nativeWindow = null;
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnSystemColorsChanged"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSystemColorsChanged( EventArgs e )
		{
			base.OnSystemColorsChanged( e );
			MenuColors.SysColorsChanged( false );
			Office2003Colors.SysColorsChanged( false );
			this.Refresh();
		}

		void ISupportInitialize.BeginInit()
		{
			this.initializing = true;

			this.OnBeginInit();
		}

		protected virtual void OnBeginInit()
		{

		}

		void ISupportInitialize.EndInit()
		{
			this.initializing = false;

			this.OnEndInit();
		}

		protected virtual void OnEndInit()
		{
			// Validate the text property, if in DropDownList mode, against the attached list.
			if( this.DropDownStyle == ComboBoxStyle.DropDownList )
			{
				if( this.IsTextValid( this.Text ) )
					this.UpdatePopupControl();
				else
					this.UpdateText( true );
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is initializing.
		/// </summary>
		[Browsable( false )]
		[Description( "Gets a value indicating whether this instance is initializing." )]
		public bool IsInitializing
		{
			get { return this.initializing; }
		}

		/// <summary>
		/// Called once to create a <see cref="Syncfusion.Windows.Forms.PopupControlContainer"/>
		/// that will host the drop-down list.
		/// </summary>
		/// <returns>A new instance of a <see cref="Syncfusion.Windows.Forms.PopupControlContainer"/>.</returns>
		/// <remarks>
		/// Override this method to provide a custom implementation of the <b>PopupControlContainer</b>.
		/// <seealso cref="InitPopupContainer"/>
		/// </remarks>
		protected virtual PopupControlContainer CreatePopupContainer()
		{
			return new ComboDropDownPopupContainer();
		}

		/// <summary>
		/// Called to initialize the drop-down container.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method is called once to initialize the drop-down container that hosts the
		/// drop-down list. Use the <see cref="PopupContainer"/> property to get a reference to 
		/// the <see cref="PopupControlContainer"/> from inside an override of this method.
		/// </para>
		/// <para>
		/// Make sure to call the base class when you override this method for default initialization.
		/// </para>
		/// <seealso cref="CreatePopupContainer"/>
		/// </remarks>
		protected virtual void InitPopupContainer()
		{
			this.popupContainer.BorderStyle = BorderStyle.None;
			this.popupContainer.ParentControl = this;
			this.popupContainer.CloseUp += new PopupClosedEventHandler( this.Popup_Closed );
			this.popupContainer.BeforePopup += new CancelEventHandler( this.Popup_BeforePopup );
			this.popupContainer.BackColor = SystemColors.Window;
			this.popupContainer.HandleCreated += new EventHandler( this.Popup_HandleCreated );

			this.popupContainer.EnsurePopupHost();
			this.popupContainer.PopupHost.IgnoreWorkingArea = true;
			WinFormsUtils.ChangeStyle( this.popupContainer, ControlStyles.Selectable, false );
		}

		/// <summary>
		/// Called once to create a <see cref="System.Windows.Forms.TextBox"/>
		/// for the editable text portion of the combo.
		/// </summary>
		/// <returns>A new instance of a <see cref="System.Windows.Forms.TextBox"/>.</returns>
		/// <remarks>
		/// Override this method to provide a custom implementation of a <b>text box</b>.
		/// <seealso cref="InitTextBox"/>
		/// </remarks>
		protected virtual TextBox CreateTextBox()
		{
			return new TextBox();
		}


		/// <summary>
		/// Called once to create a <see cref="DropDownButton"/>
		/// for the drop-down button portion of the combo.
		/// </summary>
		/// <returns>A new instance of a <see cref="DropDownButton"/>.</returns>
		/// <remarks>
		/// Override this method to provide a custom implementation of a <b>DropDownButton</b>.
		/// <seealso cref="InitDropDownButton"/>
		/// </remarks>
		protected virtual DropDownButton CreateDropDownButton()
		{
			return new DropDownButton( this );
		}


		/// <summary>
		/// Called to initialize the <see cref="DropDownButton"/> used in the drop-down button portion of the combo.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method is called once to initialize the <b>DropDownButton</b> used to draw the 
		/// drop-down portion of the combo.
		/// </para>
		/// <para>
		/// Make sure to call the base class when you override this method for default initialization.
		/// </para>
		/// <seealso cref="CreateDropDownButton"/>
		/// </remarks>
		protected virtual void InitDropDownButton( DropDownButton ddButton )
		{
			ddButton.MouseDown += new EventHandler( DDButtonPressed );
			ddButton.FlatStyle = this.FlatStyle;
		}

		/// <summary>
		/// Called to initialize the <see cref="System.Windows.Forms.TextBox"/> used in the editable text portion.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method is called once to initialize the <b>text box</b> used to draw the 
		/// editable portion of the combo. Use the <see cref="TextBox"/> property to get a reference to 
		/// the <b>text box</b> from inside a override of this method.
		/// </para>
		/// <para>
		/// Make sure to call the base class when you override this method for default initialization.
		/// </para>
		/// <seealso cref="CreateTextBox"/>
		/// </remarks>
		protected virtual void InitTextBox()
		{
			this.textBox.KeyDown += new KeyEventHandler( this.TextBox_KeyDown );
			this.textBox.KeyPress += new KeyPressEventHandler( this.TextBox_KeyPress );
			this.textBox.KeyUp += new KeyEventHandler( this.TextBox_KeyUp );
			this.textBox.Click += new EventHandler( this.TextBox_Click );
			this.textBox.DoubleClick += new EventHandler( this.TextBox_DoubleClick );
			this.textBox.GotFocus += new EventHandler( this.TextBox_GotFocus );
			this.textBox.LostFocus += new EventHandler( this.TextBox_LostFocus );
			this.textBox.HelpRequested += new HelpEventHandler( this.TextBox_HelpRequested );
			this.textBox.ImeModeChanged += new EventHandler( this.TextBox_ImeModeChanged );
			this.textBox.QueryAccessibilityHelp += new QueryAccessibilityHelpEventHandler( this.TextBox_QueryAccessibilityHelp );
			this.textBox.MouseHover += new EventHandler( this.TextBox_MouseHover );
			this.textBox.MouseMove += new MouseEventHandler( this.TextBox_MouseMove );
			this.textBox.MouseLeave += new EventHandler( this.TextBox_MouseLeave );
			this.textBox.MouseEnter += new EventHandler( this.TextBox_MouseEnter );
			this.textBox.TextChanged += new EventHandler( this.TextBox_TextChanged );
            textBoxEventsHooked = true;
		}

        protected virtual void UnInitTextBox()
        {
            this.textBox.KeyDown -= new KeyEventHandler(this.TextBox_KeyDown);
            this.textBox.KeyPress -= new KeyPressEventHandler(this.TextBox_KeyPress);
            this.textBox.KeyUp -= new KeyEventHandler(this.TextBox_KeyUp);
            this.textBox.Click -= new EventHandler(this.TextBox_Click);
            this.textBox.DoubleClick -= new EventHandler(this.TextBox_DoubleClick);
            this.textBox.GotFocus -= new EventHandler(this.TextBox_GotFocus);
            this.textBox.LostFocus -= new EventHandler(this.TextBox_LostFocus);
            this.textBox.HelpRequested -= new HelpEventHandler(this.TextBox_HelpRequested);
            this.textBox.ImeModeChanged -= new EventHandler(this.TextBox_ImeModeChanged);
            this.textBox.QueryAccessibilityHelp -= new QueryAccessibilityHelpEventHandler(this.TextBox_QueryAccessibilityHelp);
            this.textBox.MouseHover -= new EventHandler(this.TextBox_MouseHover);
            this.textBox.MouseMove -= new MouseEventHandler(this.TextBox_MouseMove);
            this.textBox.MouseLeave -= new EventHandler(this.TextBox_MouseLeave);
            this.textBox.MouseEnter -= new EventHandler(this.TextBox_MouseEnter);
            this.textBox.TextChanged -= new EventHandler(this.TextBox_TextChanged);
            textBoxEventsHooked = false;
        }

		/// <overload>
		/// Releases all resources used by the control.
		/// </overload>
		/// <override/>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				this.SelectInAFewMilliSecs = false;
				//MenuColors.MenuColorsChanged -= new EventHandler(this.MenuColorsChanged);
				//Office2003Colors.MenuColorsChanged -= new EventHandler(this.MenuColorsChanged);

				this.disposing = disposing;
				if( this.popupContainer != null )
				{
                    this.popupContainer.ParentControl = null;
					this.popupContainer.BeforePopup -= new CancelEventHandler( this.Popup_BeforePopup );
					this.popupContainer.CloseUp -= new PopupClosedEventHandler( this.Popup_Closed );
					this.popupContainer.HandleCreated -= new EventHandler( this.Popup_HandleCreated );
					this.popupContainer.Dispose();
					this.popupContainer = null;
				}

                UnInitTextBox();

				if( null != this.quickSelectTimer )
				{
					this.quickSelectTimer.Stop();
					this.quickSelectTimer.Tick -= new EventHandler( QuickSelectTimer_Tick );
					this.quickSelectTimer.Dispose();
					this.quickSelectTimer = null;
				}

				if( m_scrollersFrame != null )
				{
					DetachScrollersFrame();
					m_scrollersFrame.Dispose();
					m_scrollersFrame = null;
				}


                this.cd = null;
			}

            if (comboDropDownWeakContainer != null)
            {
                MenuColors.MenuColorsChanged -= new EventHandler(comboDropDownWeakContainer.MenuColorsChangedWeakEventHandler);
                Office2003Colors.MenuColorsChanged -= new EventHandler(comboDropDownWeakContainer.Office2003ColorsChangedWeakEventHandler);
                comboDropDownWeakContainer = null;
            }
			if( this.ddButton != null )
			{
				this.ddButton.MouseDown -= new EventHandler( this.DDButtonPressed );
				( (IDisposable)this.ddButton ).Dispose();
				this.ddButton = null;
			}

			if( null != this.themedDrawing )
			{
				this.themedDrawing.Dispose();
				this.themedDrawing = null;
			}

			base.Dispose( disposing );

			if (disposing)
			{
				this.textBox.Dispose();
				this.textBox = null;
			}
		}

		#endregion INIT
		#region PROPERTIES

		/// <summary>
		/// Indicates whether to suppress the DropDown Event
		/// </summary>
		[DefaultValue( false )]
		[Description( "Indicates whether to suppress the DropDown Event" )]
		public bool SuppressDropDownEvent
		{
			get
			{
				return suppressdropdownevent;
			}

			set
			{
				if( value != suppressdropdownevent )
				{
					suppressdropdownevent = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether [use office2007 colors in active mode].
        /// </summary>
        /// <value>
        /// <c>true</c> if [use office2007 colors in active mode]; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        [Description("Indicates whether to enable or disable the Office2007Color BackColor in the TextBox with DropDownList style.")]
        public bool UseOffice2007ColorsInActiveMode
        {
            get
            {
                return this.useOffice2007ColorsInActiveMode;
            }
            set
            {
                if (this.useOffice2007ColorsInActiveMode != value)
                {
                    this.useOffice2007ColorsInActiveMode = value;
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [use metro colors in active mode].
        /// </summary>
        /// <value>
        /// <c>true</c> if [use metro colors in active mode]; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        [Description("Indicates whether to enable or disable the Metro BackColor in the TextBox with DropDownList style.")]
        public bool UseMetroColorsInActiveMode
        {
            get
            {
                return this.useMetroColorsInActiveMode;
            }
            set
            {
                if (this.useMetroColorsInActiveMode != value)
                {
                    this.useMetroColorsInActiveMode = value;
                    this.Invalidate();
                }
            }
        }
        /// <summary>   
        /// <summary>
        /// Indicates whether to enable or disable the ampersand(&&) in the Text.
        /// </summary>
        /// <remarks>
        /// This property is used when the DropDownStyle is set to DropDownList only.
        /// </remarks>
        [DefaultValue(false)]
        [Description("Indicates whether to enable or disable the ampersand(&&)in the Text.")]
        public bool UseMnemonic
        {
            get
            {
                return useMnemonic;
            }
            set
            {
                if (useMnemonic != value)
                {
                    useMnemonic = value;
                    this.Invalidate();
                }
            }
        } 

		/// <summary>
		/// Indicates whether the search in autocomplete is case-sensitive.
		/// </summary>
		[DefaultValue( false ), Description( "Indicates whether the search in autocomplete is case-sensitive." )]
		public bool CaseSensitiveAutocomplete
		{
			get
			{
				return m_bCaseSensitive;
			}
			set
			{
				if( value != m_bCaseSensitive )
				{
					m_bCaseSensitive = value;
				}
			}
		}


		/// <summary>
		/// Specifies whether the control is focused. (overridden property)
		/// </summary>
		public override bool Focused
		{
			get
			{
				if( this.textBox.Visible && this.textBox.Focused )
				{
					return true;
				}

				return false;
			}

		}


		/// <summary>
		/// Gets or sets a value indicating whether control's elements are aligned to
		///     support locales using right-to-left fonts.
		/// </summary>
		public override RightToLeft RightToLeft
		{
			get
			{
				return base.RightToLeft;
			}
			set
			{
				base.RightToLeft = value;

				// update controls layout
				PerformLayout();
			}
		}

		private bool SelectInAFewMilliSecs
		{
			get { return this.selectInAFewMilliSecs; }
			set
			{
				if( this.selectInAFewMilliSecs != value )
				{
					this.selectInAFewMilliSecs = value;
					if( value )
					{
						this.selectionTimer = new Timer();
						selectionTimer.Interval = 50;
						selectionTimer.Tick += new EventHandler( this.Timer_Tick );
						selectionTimer.Enabled = true;
					}
					else
					{
						selectionTimer.Tick -= new EventHandler( this.Timer_Tick );
						selectionTimer.Enabled = false;
						this.selectionTimer.Dispose();
						this.selectionTimer = null;
					}
				}

			}
		}
		private void Timer_Tick( object sender, EventArgs e )
		{
			this.TextBox.SelectAll();
			this.SelectInAFewMilliSecs = false;
		}
		[Documentation.DocumentationExclude(), Browsable( false )]
		protected bool IgnorePopupValueChange
		{
			get { return this.ignorePopupValueChange; }
			set { this.ignorePopupValueChange = value; }
		}

		/// <summary>
		/// This property defines the autocomplete behavior in DropDownList mode.
		/// </summary>
		/// <remarks>This property is used when the DropDownStyle is set to DropDownList only.</remarks>
		[
		DefaultValue( true ),
		Category( "Behavior" ), Description( "Defines the autocomplete behavior in DropDownList mode." )
		]
		public bool MatchFirstCharacterOnly
		{
			get { return matchFirstCharacterOnly; }
			set { matchFirstCharacterOnly = value; }
		}

		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> that will be used in the drop-down portion.
		/// </summary>
		/// <value>A <see cref="System.Windows.Forms.Control"/> derived instance.</value>
		[
		DefaultValue( null ),
		Category( "Behavior" ),
		Description( "The control that will be used in the drop-down portion." )
		]
		public virtual Control PopupControl
		{
			get
			{
				return this.popupControl;
			}
			set
			{
				if( this.popupControl != value )
				{
					if( this.popupControl != null )
						this.DetachPopupControl( false );

					this.popupControl = value;

					if( this.popupControl != null )
						this.AttachPopupControl();

					this.SetNeedLayout( true );
				}
			}
		}
		protected virtual void AttachPopupControl()
		{
			if( DesignMode )
			{
				return;
			}

			if( DropDownStyle == ComboBoxStyle.DropDownList )
			{
				if( !IsTextValid( Text ) )
				{
					UpdateText( true );
				}
			}

			PopupControl.SizeChanged += new EventHandler( PopupControl_SizeChanged );

			UpdatePopupControlRelationship();
			PopupControl.Visible = false;
			PopupControl.BackColor = BackColor;

            if ((this.Style == VisualStyle.Office2007 || this.Style == VisualStyle.Office2010 || this.Style == VisualStyle.Metro) && this.popupControl.IsHandleCreated && this.popupContainer.IsHandleCreated)
			{
				AttachScrollersFrame( this.popupControl );
				InitScrollersFrame();
			}

			WinFormsUtils.ChangeStyle( PopupControl, ControlStyles.Selectable, false );

			// Also listen to mouse up of all the listcontrol's immediate children.
			foreach( Control childControl in this.PopupControl.Controls )
			{
				//				SH - This is not necessary any more because with latest GridListControl
				//				version the childControl forward messages to the parent.
				//				childControl.MouseUp += new MouseEventHandler(this.List_MouseUp);
				//				childControl.MouseMove += new MouseEventHandler(this.List_MouseMove);
				//				childControl.MouseDown += new MouseEventHandler(this.List_MouseDown);
				WinFormsUtils.ChangeStyle( childControl, ControlStyles.Selectable, false );
			}
		}

		protected virtual void DetachPopupControl( bool disposing )
		{
			if( this.DesignMode )
				return;

			if( this.PopupControl != null )
			{
				this.PopupControl.SizeChanged -= new EventHandler( this.PopupControl_SizeChanged );
                if (!disposing && this.PopupControl.Parent != null)
					this.PopupControl.Parent.Controls.Remove( this.PopupControl );
			}
		}
		private void PopupControl_SizeChanged( object sender, EventArgs e )
		{
			if( this.DroppedDown )
			{
				// Then adjust the popupcontainer to the popupcontrol's size.
				this.UpdatePopupBounds();
			}
		}
		/// <summary>
		/// Updates the attached <see cref="PopupControl"/>'s parent-child relationship.
		/// </summary>
		/// <remarks>
		/// If in ComboBoxStyle.Simple mode, the base class implementation 
		/// parents the Popup Control to the combo itself. Otherwise, it parents the <see cref="PopupContainer"/>
		/// to the Popup Control.
		/// </remarks>
		protected virtual void UpdatePopupControlRelationship()
		{
			if( this.PopupControl == null || this.DesignMode )
				return;

			this.SuspendLayout();

			if( this.DropDownStyle != ComboBoxStyle.Simple )
			{
				if( this.PopupContainer != null )
				{
					this.PopupContainer.Controls.Add( this.PopupControl );
					this.UpdatePopupBounds();
				}
			}
			else
			{
				this.Controls.Add( this.PopupControl );
			}

			this.ResumeLayout( false );
		}
		protected virtual void UpdatePopupBounds()
		{
			this.PopupContainer.Height = this.PopupControl.Height;
			this.PopupControl.Bounds = this.PopupContainer.ClientRectangle;
		}

		/// <summary>
		/// Gets or sets the back color. (overridden property)
		/// </summary>
		public override Color BackColor
		{
			get
			{
				Color backColor;

				if (!this.UseBackColor)
				{
					if (this.Style == VisualStyle.Office2007)
					{
						if (this.IsActive && this.Enabled && !this.ReadOnly)
						{
							backColor = this.Office2007ColorTable.ComboBoxAdvNormalBackColor;
						}
						else if (this.GrayOnReadOnly)
						{
							if (this.Enabled && !this.ReadOnly)
							{
								backColor = this.Office2007ColorTable.ComboBoxAdvHotBackColor;
							}
							else
							{
								backColor = Color.LightGray;
							}
						}
						else
						{
							backColor = this.Office2007ColorTable.ComboBoxAdvHotBackColor;
						}
						if (this.TextBox != null && this.TextBox.BackColor != backColor)
						{
							this.TextBox.BackColor = backColor;
						}
					}
                    else if (this.Style == VisualStyle.Office2010)
                    {
                        if (this.IsActive && this.Enabled && !this.ReadOnly)
                        {
                            backColor = this.Office2010ColorTable.ComboBoxAdvNormalBackColor;
                        }
                        else if (this.GrayOnReadOnly)
                        {
                            if (this.Enabled && !this.ReadOnly)
                            {
                                backColor = this.Office2010ColorTable.ComboBoxAdvHotBackColor;
                            }
                            else
                            {
                                backColor = Color.LightGray;
                            }
                        }
                        else
                        {
                            backColor = this.Office2010ColorTable.ComboBoxAdvHotBackColor;
                        }
                        if (this.TextBox != null && this.TextBox.BackColor != backColor)
                        {
                            this.TextBox.BackColor = backColor;
                        }
                    }
                    else if (this.Style == VisualStyle.Metro)
                    {
                        if (this.IsActive && this.Enabled && !this.ReadOnly)
                        {
                            backColor = Color.White;
                            this.TextBox.BackColor = backColor;

                        }
                        else if (this.GrayOnReadOnly)
                        {
                            if (this.Enabled && !this.ReadOnly)
                            {
                                backColor = Color.White;
                            }
                            else
                            {
                                backColor = Color.LightGray;
                            }
                            this.TextBox.BackColor = backColor;
                        }
                        else
                        {
                            backColor = Color.White;
                            this.TextBox.BackColor = backColor;
                        }
                        if (this.TextBox != null && this.TextBox.BackColor != backColor)
                        {
                            this.TextBox.BackColor = backColor;
                        }
                    }
					else
					{
						backColor = BaseBackColor;
					}
                    if (this.Style != VisualStyle.Office2007 && this.Style != VisualStyle.Office2010 && this.Style != VisualStyle.Metro && this.grayOnReadOnly)
					{
						if (!this.Enabled)
						{
							backColor = Color.LightGray;
							this.TextBox.BackColor = Color.LightGray;
						}
						else
						{
							backColor = BaseBackColor;
                            if (this.textBox.BackColor == Color.LightGray)
                            this.TextBox.BackColor = BaseBackColor;
						}
					}
				}
                else
                {
                    backColor = BaseBackColor;
                }
				return backColor;
			}
			set
			{
                if (useBackColor || ignoreThemeBackground)
                {
                    BaseBackColor = value;

                    this.TextBox.BackColor = value;


                    if (this.popupControl != null)
                        this.popupControl.BackColor = value;

                    if (this.BackColor == DefaultComboBackColor)
                    {
                        this.TextBox.ResetBackColor();
                        if (this.popupControl != null)
                            this.popupControl.ResetBackColor();
                    }

                    this.SetNeedLayout(true);
                    if (this.IsHandleCreated)
                        this.InvalidateWindow();

                    if ((this.Style != VisualStyle.Office2007 && this.Style != VisualStyle.Office2010 && this.Style != VisualStyle.Office2010 && this.Style != VisualStyle.Metro)
                        || (base.BackColor != this.Office2007ColorTable.ComboBoxAdvHotBackColor
                             && base.BackColor != this.Office2007ColorTable.ComboBoxAdvNormalBackColor))
                    {
                        cachedBackColor = value;
                    }
                }
			}
		}

		/// <summary>
		/// Indicates whether users should be forced to enter numbers.
		/// </summary>
		/// <value>True to force numbers; False otherwise.</value>
		[Category( "Behavior" ), DefaultValue( false ),
		Description( "Specifies whether or not users should be forced to enter numbers." )]
		public virtual bool NumberOnly
		{
			get
			{
				return this.numberOnly;
			}
			set
			{
				this.numberOnly = value;
			}
		}

		/// <summary>
		/// Indicates whether the text in the edit portion can be changed.
		/// </summary>
		/// <value>If True, the edit portion is not editable by the user; False otherwise.</value>
		[Category( "Behavior" ), DefaultValue( false ),
		Description( "Specifies whether the text in the edit portion can be changed or not." )]
		public virtual bool ReadOnly
		{
			get { return this.readOnly; }
			set
			{
				if( this.readOnly != value )
				{
					this.readOnly = value;
					this.OnReadOnlyChanged( EventArgs.Empty );
					//this.Invalidate(true);
					if( this.IsHandleCreated )
						this.InvalidateWindow();
				}
			}
		}

		/// <summary>
		/// Specifies the ComboBoxBase control modifies the case of characters as they are typed.
		/// </summary>
		/// <value>
		/// <para>One of the <see cref="System.Windows.Forms.CharacterCasing"/> enumeration values that specifies whether the ComboBoxBase control modifies the case of characters. The default is CharacterCasing.Normal.</para>
		/// </value>
		[Category( "Behavior" ), DefaultValue( CharacterCasing.Normal ),
		Description( "Specifies whether the ComboBoxBase control modifies the case of characters as they are typed." )]
		public virtual CharacterCasing CharacterCasing
		{
			get
			{
				return this.TextBox.CharacterCasing;
			}
			set
			{
				if( this.TextBox.CharacterCasing != value )
				{
					this.TextBox.CharacterCasing = value;
					this.SetNeedLayout( true );
				}
			}
		}

		/// <summary>
		/// Indicates whether the control will ignore the theme`s background color and draw the BackColor instead.
		/// </summary>
		[Description( "Indicates if the control will ignore the theme's background color and draw the BackColor instead." )]
		[Category( "Appearance" )]
		[DefaultValue( true )]
		public virtual bool IgnoreThemeBackground
		{
			get
			{
				return ignoreThemeBackground;
			}
			set
			{
				if( ignoreThemeBackground != value )
				{
					ignoreThemeBackground = value;

					RefreshBackColor( value );
				}
			}
		}

		/// <summary>
		/// Gets or sets the alignment of text in this control.
		/// </summary>
		/// <value>
		/// One of the <see cref="System.Windows.Forms.HorizontalAlignment"/> enumeration 
		/// values that specifies how text is aligned in the control. 
		/// The default is <b>HorizontalAlignment.Left</b>.
		/// </value>
		/// <remarks>
		/// You can use this property to align the text within a ComboBoxBase 
		/// to match the layout of text on your form. For example, if your controls 
		/// are all located on the right side of the form, you can set the TextAlign 
		/// property to <b>HorizontalAlignment.Right</b> and the text will be aligned 
		/// along the right side of the control instead of the default left alignment.
		/// </remarks>
		[Category( "Appearance" ), DefaultValue( HorizontalAlignment.Left ),
		Description( "Specifies how the text is aligned in this control." )]
		public virtual HorizontalAlignment TextAlign
		{
			get
			{
				return this.TextBox.TextAlign;
			}
			set
			{
				if( this.TextBox.TextAlign != value )
				{
					this.TextBox.TextAlign = value;
					this.SetNeedLayout( true );
				}
			}
		}
		/// <summary>
		/// Gets or sets the ContextMenu. (overridden property)
		/// </summary>
		public override ContextMenu ContextMenu
		{
			get
			{
				if( this.TextBox.ContextMenu != null )
					return this.TextBox.ContextMenu;
				else
					return this.cachedContextMenu;
			}
			set
			{
				this.TextBox.ContextMenu = value;
				base.ContextMenu = this.TextBox.ContextMenu;
				if( this.popupControl != null
					&& this.popupControl.Parent == this )
					this.popupControl.ContextMenu = this.TextBox.ContextMenu;
			}
		}


		/// <summary>
		/// Gets or sets the ForeColor. (overridden property)
		/// </summary>
		public override Color ForeColor
		{
			get
			{
				return this.TextBox != null ? this.TextBox.ForeColor : Color.Empty;
			}
			set
			{
				this.TextBox.ForeColor = value;
				base.ForeColor = this.TextBox.ForeColor;
				if( this.popupControl != null )
					this.popupControl.ForeColor = this.TextBox.ForeColor;
				this.SetNeedLayout( true );
			}
		}
		/// <summary>
		/// Resets the <see cref="P:System.Windows.Forms.Control.BackColor"/> property to its default value.
		/// </summary>
		/// <override/>
		public override void ResetBackColor()
		{
			this.BackColor = DefaultComboBackColor;
			this.SetNeedLayout( true );
		}
		protected bool ShouldSerializeBackColor()
		{
			if( this.BackColor == DefaultComboBackColor )
				return false;
			else
				return true;
		}

		/// <override/>
		protected override void OnEnabledChanged( EventArgs e )
		{
			base.OnEnabledChanged( e );
			// Make sure to call this after calling the base class imp.
			this.ddButton.Enabled = this.Enabled && !this.ReadOnly;
			this.TextBox.Enabled = this.Enabled;
			if( this.popupControl != null && this.DroppedDown)
				this.popupControl.Enabled = this.Enabled && !this.ReadOnly;
		}

		/// <summary>
		/// Gets or sets the 3D border style for the control.
		/// </summary>
		/// <remarks>
		/// This property is used only when BorderStyle is Fixed3D.
		/// </remarks>
		[Description( "Indicates the style of the 3D border." )]
		[Category( "Appearance" )]
		[DefaultValue( Border3DStyle.Sunken )]
		public Border3DStyle Border3DStyle
		{
			get { return border3DStyle; }
			set
			{
				if( border3DStyle !=value )
				{
					border3DStyle = value;
					this.OnBorder3DStyleChanged( EventArgs.Empty );
					if( this.IsHandleCreated )
						this.InvalidateWindow();
				}
			}
		}
		/// <summary>
		/// Gets or sets the border sides for which you want the 3D border style applied.
		/// </summary>
		/// <remarks>
		/// This property is used only when BorderStyle is Fixed3D.
		/// </remarks>
		[Description( "Indicates the border sides for which to use 3D borders in the control." )]
		[Category( "Appearance" )]
		[DefaultValue( Border3DSide.All )]
		public Border3DSide BorderSides
		{
			get { return borderSides; }
			set
			{
				if( borderSides!=value )
				{
					borderSides = value;
					this.OnBorderSidesChanged( EventArgs.Empty );
					if( this.IsHandleCreated )
						InvalidateWindow();
				}
			}
		}
		protected string LastCommittedText
		{
			get { return this.latestCommittedString; }
			set
			{
				this.latestCommittedString = value;
			}
		}

		/// <summary>
		/// Gets or sets the flat style appearance of the combo box control.
		/// </summary>
		/// <value>
		/// One of the <see cref="ComboFlatStyle"/> values. The default value is <b>Standard</b>.
		/// </value>
		/// <remarks>
		/// <para>
		/// When you specify <see cref="ComboFlatStyle.System"/> and the application is run
		/// in Windows XP, themes will be used to draw this control.
		/// </para>
		/// <para>
		/// Use <see cref="Syncfusion.Windows.Forms.XPThemes.IsThemedOS"/> and 
		/// <see cref="Syncfusion.Windows.Forms.XPThemes.IsThemeActive"/> to determine
		/// if themes are on during run-time.
		/// </para>
		/// </remarks>
		[
		Category( "Appearance" ),
		DefaultValue( ComboFlatStyle.Standard ),
		Description( "Specifies the flat style appearance of the combo box control." )
		]
		public virtual ComboFlatStyle FlatStyle
		{
			get
			{
				return this.comboFlatStyle;
			}
			set
			{
				if( this.comboFlatStyle != value )
				{
					if( this.Style != VisualStyle.Default && value == ComboFlatStyle.System )
					{
						if( this.DesignMode )
							MessageBox.Show( "Cannot set FlatStyle to System when the Style property is non-default.", "ComboDropDown design-time information:", MessageBoxButtons.OK, MessageBoxIcon.Warning );
						return;
					}
					this.comboFlatStyle = value;
					this.ddButton.FlatStyle = value;
					this.UpdateStyles();
					if( this.IsHandleCreated )
						this.InvalidateWindow();
					this.SetNeedLayout( true );
				}
			}
		}
		/// <summary>
		/// Gets or sets an advanced appearance and behavior for this control.
		/// </summary>
		/// <value>One of the <see cref="Syncfusion.Windows.Forms.VisualStyle"/> values.
		/// Default is <b>VisualStyle.Default</b>.</value>
		/// <remarks>
		/// <para>When a non-default style is specified here, it will override the
		/// other settings of the control such as <see cref="BorderSides"/> and <see cref="FlatStyle"/>.
		/// The <see cref="FlatBorderColor"/> is used to draw the borders for the office styles, when
		/// the control is not active.</para>
		/// <para>
		/// The <b>Office2003</b> style behaves the same as <b>OfficeXP</b> in non-XP systems.
		/// </para>
		/// </remarks>
		[
		Category( "Appearance" ),
		Description( "Specifies an advanced appearance and behavior." ),
		DefaultValue( VisualStyle.Default ),
		TypeConverter( typeof( DefaultVisualStyleEnumFilter ) )
		]
		public VisualStyle Style
		{
			get { return this.editorStyle; }
			set
			{
				if( this.editorStyle != value )
				{
					if( value != VisualStyle.Default && this.FlatStyle == ComboFlatStyle.System )
					{
						MessageBox.Show( "Changing FlatStyle to Standard", "ComboDropDown design-time information:", MessageBoxButtons.OK, MessageBoxIcon.Information );
						this.FlatStyle = ComboFlatStyle.Standard;
					}

					this.editorStyle = value;
					this.ddButton.Style = value;
					this.UpdateStyles();
					this.SetNeedLayout( true );

					this.OnStyleChanged();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		internal void OnStyleChanged()
		{
			if( this.Style == VisualStyle.Office2007 || this.Style == VisualStyle.Office2010)
			{
				if( this.popupControl != null && this.popupControl.IsHandleCreated && 
                    this.popupContainer != null && this.popupContainer.IsHandleCreated )
				{
					AttachScrollersFrame( this.popupControl );
					InitScrollersFrame();
				}
			}
                 else if (this.Style == VisualStyle.Metro)
            {
                if (this.popupControl != null && this.popupControl.IsHandleCreated &&
                    this.popupContainer != null && this.popupContainer.IsHandleCreated)
                {                   
                    AttachScrollersFrame(this.popupControl);
                    InitScrollersFrame();
                }
            }
			else
			{
				this.BackColor = cachedBackColor;
				DetachScrollersFrame();
			}

			if( this.IsHandleCreated )
			{
				this.InvalidateWindow();
			}
		}

		/// <summary>
		/// Indicates whether the combo box is displaying its drop-down portion.
		/// </summary>
		/// <value>True if the drop-down portion is displayed; False otherwise. The default is False.</value>
		[Browsable( false ), EditorBrowsable( EditorBrowsableState.Always ),
		Description( "Specifies a value indicating whether the combo box is displaying its drop-down portion." ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool DroppedDown
		{
			get
			{
				if( this.PopupContainer != null )
					return this.PopupContainer.IsShowing();
				return false;
			}
			set
			{
				if( value == true )
					this.ShowPopup();
				else
					this.HidePopup();
			}
		}

		/// <summary>
		/// Returns the <see cref="Syncfusion.Windows.Forms.PopupControlContainer"/> used to host the drop-down list.
		/// </summary>
		[Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Description( "Gets the PopupControlContainer used to host the drop-down list." )]
		public PopupControlContainer PopupContainer
		{
			get
			{
				if( this.popupContainer == null )
				{
					this.popupContainer = this.CreatePopupContainer();
					this.InitPopupContainer();
				}

				return this.popupContainer;
			}
		}

		///<override/>
		protected override /*Control*/ Size DefaultSize
		{
			get
			{
				if( this.IsHandleCreated )
				{
					Graphics g = this.CreateGraphics();
					this.Layout( g );
					g.Dispose();
					return new Size( 121, this.Height );
				}
				else
					return new Size( 121, 21 );
			}
		}

		/// <summary>
		/// Returns the <see cref="System.Windows.Forms.TextBox"/> used to draw the editable text portion of the combo.
		/// </summary>
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false ), EditorBrowsable( EditorBrowsableState.Always ),
		Description( "Gets the text box used to draw the editable text portion of the combo." )]
		public TextBox TextBox
		{
			get { return this.textBox; }
		}

		/// <summary>
		/// Returns the <see cref="DropDownButton"/> used to draw the drop-down portion of the combo.
		/// </summary>
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false ), EditorBrowsable( EditorBrowsableState.Always ),
		Description( "Gets the DropDownButton used to draw the drop-down portion of the combo." )]
		public DropDownButton DropDownButton
		{
			get { return this.ddButton; }
		}

		/// <summary>
		/// Gets or sets the height of the drop-down button.
		/// </summary>
		protected int DropDownButtonHeight
		{
			get { return this.dropDownButtonHeight; }
			set { this.dropDownButtonHeight = value; }
		}
		/// <summary>
		/// Gets or sets the height of the edit portion.
		/// </summary>
		protected int EditPortionHeight
		{
			get { return this.editPortionHeight; }
			set { this.editPortionHeight = value; }
		}
		/// <summary>
		/// Indicates whether the Height property of the control can be changed.
		/// </summary>
		/// <value>True to prevent height change; False otherwise .</value>
		/// <remarks>
		/// Note that this property will be frequently set and reset within the control layout.
		/// You can use this temporarily to force a particular height on the control.
		/// </remarks>
		protected bool PreventHeightChange
		{
			get { return this.preventHeightChange; }
			set { this.preventHeightChange = value; }
		}
		/// <summary>
		/// Gets or sets the text associated with this control.
		/// </summary>
		/// <remarks>
		/// <para>When in <b>DropDownList</b> mode, setting this property will also validate the
		/// new value against the attached popup control.</para>
		/// </remarks>
		public override string Text
		{
			get { return this.textBox.Text; }
			set
            {
                this.SetText(value, true);
			}
		}
		/// <summary>
		/// Gets or sets the maximum number of characters allowed in the editable portion of a combo box.
		/// </summary>
		/// <value>The maximum number of characters the user can enter.
		/// Default value is 32767.</value>
		[
		Category( "Behavior" ),
		DefaultValue( 32767 /*0x7FFF*/),
		Description( "Gets or sets the maximum number of characters allowed in the editable portion of a combo box." )
		]
		public int MaxLength
		{
			get { return this.textBox.MaxLength; }
			set { this.textBox.MaxLength = value; }
		}
		/// <summary>
		/// Gets or sets the drop-down's width.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The value provided here will be used to determine the width of the drop-down portion.
		/// If you do not set a value explicitly, the width of this control will be used as the preferred width.
		/// </para>
		/// <para>To reset your settings on this property, call the <see cref="ResetDropDownWidth"/> method.</para>
		/// </remarks>
		[
		Category( "Behavior" ),
		Description( "Specifies the drop-down's width." )
		]
		public int DropDownWidth
		{
			get
			{
				if( this.dropDownWidth != -1 )
					return this.dropDownWidth;
				else
					return this.Width;
			}
			set
			{
				if( this.dropDownWidth != value )
				{
					this.dropDownWidth = value;
					// Update the popupContainer's width with the new width:
					if( this.popupContainer != null  && !this.popupContainer.IsShowing() )
					{
						this.popupContainer.Width = value;
					}
				}
			}
		}
		/// <summary>
		/// Resets the <see cref="DropDownWidth"/> property's value to its default.
		/// </summary>
		public void ResetDropDownWidth()
		{
			this.dropDownWidth = -1;
		}
		/// <summary>
		/// Indicates whether the <see cref="DropDownWidth"/> property's value is the default.
		/// </summary>
		/// <returns>True if the value is not the default; false otherwise.</returns>
		public bool ShouldSerializeDropDownWidth()
		{
			return this.dropDownWidth != -1;
		}

		/// <summary>
		/// Gets or sets the style of the combo box.
		/// </summary>
		/// <value>
		/// One of the <see cref="System.Windows.Forms.ComboBoxStyle"/> values. 
		/// The default is <see cref="System.Windows.Forms.ComboBoxStyle.DropDown"/>.
		/// </value>
		/// <remarks>
		/// The <b>DropDownStyle</b> property controls the interface that is presented 
		/// to the user. You can enter a value that allows for a simple drop-down list box, 
		/// where the list always displays a drop-down list box, where the text portion is 
		/// not editable and you must select an arrow to view the drop-down, 
		/// or the default drop-down list box where the text portion is editable and the 
		/// user must press the arrow key to view the list. 
		/// </remarks>
		[
		Category( "Appearance" ),
		DefaultValue( typeof( System.Windows.Forms.ComboBoxStyle ), "DropDown" ),
		Description( "Specifies a value specifying the style of the combo box." )
		]
		public ComboBoxStyle DropDownStyle
		{
			get { return this.comboStyle; }
			set
			{
				if( this.comboStyle != value )
				{
					this.comboStyle = value;

					this.SetNeedLayout( true );

				}
				this.OnDropDownStyleChanged(EventArgs.Empty);
			}
		}
		/// <summary>
		/// Gets or sets the color with which the flat border should be drawn.
		/// </summary>
		/// <value>
		/// A color value. Default is SystemColors.ControlDark.
		/// </value>
		/// <remarks>
		/// This is also the color used to draw the border when the <see cref="Style"/>
		/// property is set to <b>OfficeXP</b> or <b>Office2003</b>.
		/// </remarks>
		[Category( "Appearance" ),
		Description( "Specifies the color with which the flat border should be drawn." )]
		public virtual Color FlatBorderColor
		{
			get
			{
				if( this.flatBorderColor != Color.Empty )
					return this.flatBorderColor;
				else
					return SystemColors.ControlDark;
			}
			set
			{
				if( this.flatBorderColor != value )
				{
					this.flatBorderColor = value;
					if( this.IsHandleCreated )
						this.InvalidateWindow();
				}
			}
		}
		protected void ResetFlatBorderColor()
		{
			this.flatBorderColor = Color.Empty;
			this.Invalidate();
		}

		protected bool ShouldSerializeFlatBorderColor()
		{
			if( this.flatBorderColor != Color.Empty )
				return true;
			else
				return false;
		}

		/// <summary>
		/// Specifies the BackgroundImage.(overridden property)
		/// </summary>
		[Browsable( false ),
		DefaultValue( null )]
		public override Image BackgroundImage
		{
			get { return null; }
			set { }
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// Gets or sets background image layout. 
		/// </summary>
		[Browsable( false ),
		DefaultValue( ImageLayout.None )]
		public new ImageLayout BackgroundImageLayout
		{
			get
			{ return ImageLayout.None; }
			set
			{ }
		}
#endif

		// Keeps track of the overall focus state of the combo.
		private bool ComboInFocus
		{
			get { return this.comboInFocus; }
			set
			{
				if( this.comboInFocus != value )
				{
					if( value )
					{
						this.comboInFocus = value;
						this.LastCommittedText = this.Text;
					}
					else //if(!this.ContainsFocus)
					{
						this.comboInFocus = value;
						this.CommitChanges();
                        if(!this.LastCommittedText.Equals(this.Text))
						    this.LastCommittedText = this.Text;
					}

                    if (value)
                        this.IsActive = true;
                    else
                        this.UpdateIsActiveState();

				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating if quick selction on mouse move is on.
		/// </summary>
		[Description("Gets or sets a value indicating if quick selction on mouse move is on."),DefaultValue(true)]
		public bool AllowQuickSelection
		{
			get { return allowQuickSelection; }
			set { allowQuickSelection = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating to show the Gary BackColor on ReadOnly is set.
		/// </summary>
		[Description("Gets or sets a value indicating to show the Gary BackColor on ReadOnly is set."), DefaultValue(true)]
		public bool GrayOnReadOnly
		{
			get { return grayOnReadOnly; }
			set { grayOnReadOnly = value; }
		}

		/// <summary>
		/// Gets or sets a value to have effect of BackColor
		/// </summary>
        [Description("Gets or sets a value to have a effect of BackColor."), DefaultValue(false), Category("Appearance")]
		public bool UseBackColor
		{
			get { return useBackColor; }
			set { useBackColor = value; }
		}
		
		internal bool QuickSelectOn
		{
			get { return this.quickSelectOn; }
			set
			{
				if( this.quickSelectOn != value )
				{
					if( this.quickSelectOn )
						this.quickSelectTimer.Stop();

					this.quickSelectOn = value;

					if( this.quickSelectOn )
					{
						this.quickSelectTimer.Start();

						this.ddButton.CancelMouseTrack();
					}
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
                {
                    Style = VisualStyle.Office2007;
                    Office2007ColorTheme = Office2007Theme.Blue;
                }
                else if (value == "Office2007Silver")
                {
                    Style = VisualStyle.Office2007;
                    Office2007ColorTheme = Office2007Theme.Silver;
                }
                else if (value == "Office2007Black")
                {
                    Style = VisualStyle.Office2007;
                    Office2007ColorTheme = Office2007Theme.Black;
                }
                else if (value == "Managed")
                {
                    Style = VisualStyle.Office2007;
                    Office2007ColorTheme = Office2007Theme.Managed;
                }
                if (value == "Office2010Blue")
                {
                    Style = VisualStyle.Office2010;
                    Office2010ColorTheme = Office2010Theme.Blue;
                }
                else if (value == "Office2010Silver")
                {
                    Style = VisualStyle.Office2010;
                    Office2010ColorTheme = Office2010Theme.Silver;
                }
                else if (value == "Office2010Black")
                {
                    Style = VisualStyle.Office2010;
                    Office2010ColorTheme = Office2010Theme.Black;
                }
                else if (value == "Managed")
                {
                    Style = VisualStyle.Office2010;
                    Office2010ColorTheme = Office2010Theme.Managed;
                }
                else if (value == "Metro")
                {
                    Style = VisualStyle.Metro;
                }
                else if (value == "Default")
                    Style = VisualStyle.Default;
                else if (value == "Office2003")
                    Style = VisualStyle.Office2003;
                else if (value == "OfficeXP")
                    Style = VisualStyle.OfficeXP;
                else if (value == "Office2007Outlook")
                    Style = VisualStyle.Office2007Outlook;
                else if (value == "VS2005")
                    Style = VisualStyle.VS2005;
                else if (value == "VS2010")
                    Style = VisualStyle.VS2010;
            }
        }
		/// <summary>
		/// Gets or sets office 2007 color theme. 
		/// </summary>
		[Category( "Appearance" ),
		Description( "Office 2007 color scheme." ),
		DefaultValue( Office2007Theme.Blue )]
		public Office2007Theme Office2007ColorTheme
		{
			get
			{
				return m_office2007ColorTheme;
			}

			set
			{
				if( m_office2007ColorTheme != value )
				{
					m_office2007ColorTheme = value;
					this.ddButton.Office2007ColorTheme = value;

					this.OnStyleChanged();
				}
			}
		}
        /// <summary>
        /// Gets or sets office 2010 color theme. 
        /// </summary>
        [Category("Appearance"),
        Description("Office 2010 color scheme."),
        DefaultValue(Office2010Theme.Blue)]
        public Office2010Theme Office2010ColorTheme
        {
            get
            {
                return m_office2010ColorTheme;
            }

            set
            {
                if (m_office2010ColorTheme != value)
                {
                    m_office2010ColorTheme = value;
                    this.ddButton.Office2010ColorTheme = value;

                    this.OnStyleChanged();
                }
            }
        }
		/// <summary>
		/// Metrocolor.
		/// </summary>
        private Color metroColor = ColorTranslator.FromHtml("#16A5DC");
		/// <summary>
		/// Gets or sets the metrocolor.
		/// </summary>
        public Color MetroColor
        {
            get { return metroColor; }
            set { metroColor = value; }
        }
		/// <summary>
		///Serialize metrocolor.
		/// </summary>
        private bool ShouldSerializeMetroColor()
        {
            if (this.MetroColor == ColorTranslator.FromHtml("#16A5DC"))
                return false;
            else
                return true;
        }
		/// <summary>
		/// Resets the ,metrocolor.
		/// </summary>
        private void ResetMetroColor()
        {
            this.MetroColor = ColorTranslator.FromHtml("#16A5DC");
        }
		/// <summary>
		/// Gets color table for Office2007 visual style.
		/// </summary>
		internal Office2007Colors Office2007ColorTable
		{
			get
			{
				Office2007Colors colorTable = Office2007Colors.GetColorTable( this.Office2007ColorTheme );

				return colorTable;
			}
		}
        /// <summary>
        /// Gets color table for Office2010 visual style.
        /// </summary>
        internal Office2010Colors Office2010ColorTable
        {
            get
            {
                Office2010Colors colorTable = Office2010Colors.GetColorTable(this.Office2010ColorTheme);

                return colorTable;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether selection changing can be done using mouse wheel rotation.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if seelction can be changed using mouse wheel; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Instead of handling mouse wheel event and handling it, user can set this bool property as per the needs.
        /// This property is used to work when the dropdown is not shown and changing selection through mouse wheel.
        /// </remarks>
        [Description("Indicates whether item selection changing can be done using mouse wheel rotation."), DefaultValue(true)]
        public bool AllowMouseWheelSelection
        {
            get { return allowMouseWheelSelection; }
            set { allowMouseWheelSelection = value; }
        }

		#endregion PROPERTIES
		#region OVERRIDES
		private bool m_bIsEntering = false;

		/// <override/>
		protected override void OnEnter( EventArgs e )
		{
			if( m_bIsEntering )
			{
				return;
			}

			m_bIsEntering = true;
			this.ComboInFocus = true;
			base.OnEnter( e );

			if( this.textBox.Visible )
				this.textBox.Focus();

			if( this.NeedLayout )
			{
				Graphics g = this.CreateGraphics();
				this.Layout( g );
				g.Dispose();
			}

			//this.ActiveControl = this.textBox;
			if( this.DropDownStyle == ComboBoxStyle.DropDownList )
				this.SetNeedLayout( true );

			m_bIsEntering = false;
		}

		// The next two methods are not necessary in Everette.
		/// <override/>
		protected override void OnGotFocus( EventArgs e )
		{
			base.OnGotFocus( e );

			if( this.DropDownStyle == ComboBoxStyle.DropDownList )
				this.SetNeedLayout( true );

			if( this.FlatStyle != ComboFlatStyle.System )
			{
				this.InvalidateWindow();
			}
		}
		/// <override/>
		protected override void OnLostFocus( EventArgs e )
		{
			m_strTextToSearch = string.Empty;
			base.OnLostFocus( e );
			if( this.DropDownStyle == ComboBoxStyle.DropDownList )
				this.SetNeedLayout( true );
			//this.ComboInFocus = false;

			if( this.FlatStyle != ComboFlatStyle.System )
			{
				this.InvalidateWindow();
			}
			m_lastFoundIndex = -1;
		}
		protected override void OnTabStopChanged( EventArgs e )
		{
			base.OnTabStopChanged( e );

			this.TextBox.TabStop = this.TabStop;
		}

		protected override void OnValidated( EventArgs e )
		{
			base.OnValidated( e );
			inputTextIndex = 0;
			this.ComboInFocus = false;
		}
		protected virtual void SetText( string newValue, bool fireChangeEvent )
		{
            if (this.Text != newValue)
                {
                    bool changed = true;

                    // If Valid text, then throw the SelectionChangeCommitted event.
                    if (this.IsTextValid(newValue))
                    {
                        this.ignorePopupValueChange = true;
                        this.SetPopupText(newValue);
                        this.ignorePopupValueChange = false;
                        this.UpdateText(fireChangeEvent);
                    }
                    else
                    {
                        // Update only if not in list mode or initializing.
                        if (this.DropDownStyle != ComboBoxStyle.DropDownList
                            || this.initializing)
                        {
                            this.TextBox.Text = newValue;
                            if (!this.initializing)
                                this.UpdatePopupControl();
                        }
                        else
                        {
                            changed = false;
                        }
                    }
                    if (changed)
                    {
                        this.SetNeedLayout(true);
                    }
                }
		}
		protected virtual bool IsTextValid( string text )
		{
			return true;
		}
		protected virtual void SetPopupText( string value )
		{
			if( this.PopupControl != null )
				this.PopupControl.Text = value;
		}
		protected virtual string GetPopupText()
		{
			return this.PopupControl.Text;
		}

		/// <summary>
		/// Updates the <see cref="Text"/> property based on the PopupControl's selected value.
		/// </summary>
		/// <param name="fireEvent">Indicates whether the <see cref="SelectionChangeCommitted"/> event should be fired if the text is changed.</param>
		/// <returns>True if the <see cref="SelectionChangeCommitted"/> event was fired; False otherwise.</returns>
		/// <remarks>You normally do not have to call this method. However when you 
		/// programmatically update the SelectedValue of a plug in the list control, 
		/// you might have to call this method to update the combo's text based on that new value.</remarks>
		public virtual bool UpdateText( bool fireEvent )
		{
			string oldText = this.Text;
			if( this.PopupControl != null )
			{
				string newText = this.GetPopupText();

				if( this.CharacterCasing == CharacterCasing.Upper )
					newText = newText.ToUpper();
				else if( this.CharacterCasing == CharacterCasing.Lower )
					newText = newText.ToLower();

                if (this.IsTextValid(newText) || newText == String.Empty)
                    this.TextBox.Text = newText;

             

				this.SetNeedLayout( true );
			}
			else if( this.DropDownStyle == ComboBoxStyle.DropDownList )
			{
				this.TextBox.Text = String.Empty;
			}
			// Fire the selection change committed event.
			if( fireEvent && oldText != this.Text && !disposing )
			{
				this.OnSelectionChangeCommitted( EventArgs.Empty );
				return true;
			}

			return false;
		}

		/// <summary>
		/// Indicates whether another change was made except text changing
		/// in control.
		/// </summary>
		/// <returns>True if changes were detected; false otherwise.</returns>
		protected virtual bool OtherChangesMade()
		{
			return false;
		}

		protected override void OnHandleCreated( EventArgs e )
		{
			base.OnHandleCreated( e );

			Init();
		}

		protected override void OnHandleDestroyed( EventArgs e )
		{
			base.OnHandleDestroyed( e );

            if( this.popupControl == null)
            {
                DetachPopupControl(false);
                if(this.popupControl!=null)
                this.popupControl.Dispose();
                this.popupControl = null;
            }

			Uninit();
		}

		#endregion OVERRIDES
		#region EVENTS
		/// <summary>
		/// Fired when BorderSides are changed.
		/// </summary>
		[Category( "Property Changed" ), Description( "Fired when BorderSides are changed." )]
		public event EventHandler BorderSidesChanged;
		/// <summary>
		/// Occurs when border's 3D style is changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "Occurs when border's 3D style is changed." )]
		public event EventHandler Border3DStyleChanged;

		/// <summary>
		/// Raises the BorderSidesChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event. <para>The OnBorderSidesChanged method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OmBorderSidesChanged 
		/// in a derived class, be sure to call the base class's 
		/// OnBorderSidesChanged method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnBorderSidesChanged( EventArgs e )
		{
			if( BorderSidesChanged!=null ) { BorderSidesChanged( this, e ); }
		}
		/// <summary>
		/// Raises the Border3DStyleChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event. <para>The OnBorder3DStyleChanged method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnBorder3DStyleChanged 
		/// in a derived class, be sure to call the base class's 
		/// OnBorder3DStyleChanged method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnBorder3DStyleChanged( EventArgs e )
		{
			if( Border3DStyleChanged!=null ) { Border3DStyleChanged( this, e ); }
		}
		protected virtual void CommitChanges()
		{
			if( this.LastCommittedText != this.Text )
				this.OnSelectionChangeCommitted( EventArgs.Empty );
			this.LastCommittedText = this.Text;
		}

		protected virtual void ResetToLatestCommittedText()
		{
			this.SetText( this.LastCommittedText, false );
		}
		/// <summary>
		/// Occurs before the drop-down portion is shown.
		/// </summary>
		[
		Category( "Action" ),
		Description( @"Occurs before the drop-down portion is shown." )
		]
		public event EventHandler DropDown;

		/// <summary>
		/// Raises the Drop-Down event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks><para>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event.</para> <para>The OnDropDown method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnDropDown 
		/// in a derived class, be sure to call the base class's 
		/// OnDropDown method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnDropDown( EventArgs e )
		{
			if( this.suppressdropdownevent )
			{
				return;
			}

			m_strTextToSearch = string.Empty;

			if( this.DropDown != null )
				this.DropDown( this, e );
		}

		/// <summary>
		/// Occurs when the user selects a new text for the combo in one of many possible ways.
		/// </summary>
		/// <remarks>
		/// <para>This event will be fired for the following cases:</para>
		/// <list type="bullet">
		/// <item>
		/// <term>
		/// When the user selects a new item in the list box by clicking or pressing Enter.
		/// </term>
		/// </item>
		/// <item>
		/// <term>
		/// When the user Tabs out of the combo after changing the current text and Validation was successful.
		/// </term>
		/// </item>
		/// </list>
		/// </remarks>
		[Category( "Behavior" ), Description( "Occurs when the user selects a new text for the combo." )]
		public event EventHandler SelectionChangeCommitted;


		/// <summary>
		/// Raises the SelectionChangeCommitted event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks><para>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event.</para> <para>The OnSelectionChangeCommitted method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnSelectionChangeCommitted 
		/// in a derived class, be sure to call the base class's 
		/// OnSelectionChangeCommitted method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnSelectionChangeCommitted( EventArgs e )
		{
			this.LastCommittedText = this.Text;

            if (this.SelectionChangeCommitted != null)
            {
                this.SelectionChangeCommitted(this, e);
            }
		}

		/// <summary>
		/// Raises the <see cref="SelectionChangeCommitted"/> event.
		/// </summary>
		/// <remarks>
		/// The event will be fired usually when
		/// the user commits selection change. You could use this method to raise an event manually for a 
		/// custom scenario.
		/// </remarks>
		public void RaiseSelectionChangeCommitted()
		{
			this.OnSelectionChangeCommitted( EventArgs.Empty );
		}

		/// <summary>
		/// Occurs when the <see cref="DropDownStyle"/> of the combo changes.
		/// </summary>
		[Category( "Behavior" ), Description( "Occurs when the DropDownStyle of the combo changes." )]
		public event EventHandler DropDownStyleChanged;

		/// <summary>
		/// Raises the DropDownStyleChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks><para>Raising an event invokes the event handler 
		/// through a delegate. For more information, see "Raising 
		/// an Event".</para> <para>The DropDownStyleChanged method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnDropDownStyleChanged 
		/// in a derived class, be sure to call the base class's 
		/// OnDropDownStyleChanged method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnDropDownStyleChanged( EventArgs e )
		{
			if( DropDownStyle == ComboBoxStyle.Simple )
			{
				this.preventHeightChange = false;
				if( this.Parent != null )
				{
					this.Parent.PerformLayout( this, "Bounds" );
				}
			}
			else
            {
                using (Graphics g = this.CreateGraphics())
                {
                    if (g.DpiX > 96)
                    {
                        this.preventHeightChange = false;
                        if (this.Parent != null)
                        {
                            this.Parent.PerformLayout(this, "Bounds");
                        }
                    }
                    else
                        this.preventHeightChange = true;
                }
            }

			if( this.DropDownStyle == ComboBoxStyle.DropDownList )
				this.UpdateText( false );

			if( this.DropDownStyleChanged != null )
				this.DropDownStyleChanged( this, e );
		}

		/// <summary>
		/// Occurs when the <see cref="ReadOnly"/> property of the combo changes.
		/// </summary>
		[Category( "Behavior" ), Description( "Occurs when the Read-only property of the combo changes." )]
		public event EventHandler ReadOnlyChanged;

		/// <summary>
		/// Raises the ReadOnlyChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks><para>Raising an event invokes the event handler 
		/// through a delegate. For more information, see "Raising 
		/// an Event".</para> <para>The ReadOnlyChanged method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnReadOnlyChanged 
		/// in a derived class, be sure to call the base class's 
		/// OnReadOnlyChanged method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnReadOnlyChanged( EventArgs e )
		{
			this.ddButton.Enabled = this.Enabled && !this.ReadOnly;
			this.TextBox.ReadOnly = this.ReadOnly;
			if( this.popupControl != null )
				this.popupControl.Enabled = this.Enabled && !this.ReadOnly;

			// Fire event.
			if( this.ReadOnlyChanged != null )
				this.ReadOnlyChanged( this, e );
		}

		private void QuickSelectTimer_Tick( object sender, EventArgs e )
		{
			Point pt = Control.MousePosition;
			pt = this.PointToClient( pt );
			this.ProcessPopupControlMouseMove( this, new MouseEventArgs( Control.MouseButtons, 1, pt.X, pt.Y, 0 ) );
		}
		#endregion EVENTS
		#region BORDER_DRAWING_STUFF
		private void InvalidateWindow()
		{
			NativeMethods.RedrawWindow( this.Handle, IntPtr.Zero, IntPtr.Zero,
				NativeMethods.RDW_FRAME
				| NativeMethods.RDW_INVALIDATE | NativeMethods.RDW_ALLCHILDREN );
		}
		/// <override/>
		protected override CreateParams CreateParams
		{
			[SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode=true )]
			get
			{
				CreateParams cparams;

				cparams = base.CreateParams;
				cparams.ExStyle &= ~512;
				cparams.Style &= ~8388608;
				if( this.Style != VisualStyle.Default )
				{
					// Use a single width nc border for office styles.
					cparams.Style |= 0x800000;
				}
				else
				{
					switch( this.comboFlatStyle )
					{
						// It's easier to use the WS_CLIENTEDGE for both flat and 3d borders and draw differently.
						case ComboFlatStyle.Standard:
						case ComboFlatStyle.Flat:
						cparams.ExStyle = cparams.ExStyle | 512;
						break;
					}
				}
				return cparams;
			}
		}
		private IntPtr cachedRgn = IntPtr.Zero;

		IntPtr INonClientPaintingSupport.NonClientPaint( PaintEventArgs e, Rectangle displayRect, Rectangle windowRectInScreen )
		{
			Graphics g = e.Graphics;
			Rectangle bounds = displayRect;

			// This is not good for the following reasons:
			// 1) When dragging a hidden tree into the visible desktop range, clipping
			// is not proper and as a result the BG is drawn over the whole control and stays there!
			// 2) Even in other scenarios, we can see the BG color being drawn first
			// followed by the tree. Causing a flicker effect.
			//
			// So, instead fill the bg only in the border area.
			// 
			// Without this some 3D styles (SunkenOuter) will leave a 1 pixel transparent area.
			// g.FillRectangle(new SolidBrush(this.BackColor),bounds);

			int w = c_iDefaultBorderWidthVSStyle;// 2 for either of the border style because we allocate 2 pixels for either border style

			BorderStyle borderStyle = BorderStyle.Fixed3D;
			if( this.Style != VisualStyle.Default )
			{
				borderStyle = BorderStyle.FixedSingle;

				w = c_iDefaultBorderWidth;
			}
			else
			{
				switch( this.FlatStyle )
				{
					case ComboFlatStyle.Flat:
					borderStyle = BorderStyle.FixedSingle;
					break;
					case ComboFlatStyle.Standard:
					borderStyle = BorderStyle.Fixed3D;
					break;
				}
			}

			// The borders as 4 rectangles.
			Rectangle[] clipRects = new Rectangle[]
						{
							new Rectangle(bounds.Location,new Size(w,bounds.Height)),
							new Rectangle(bounds.Location,new Size(bounds.Width,w)),
							new Rectangle(bounds.Width-w,bounds.Y,w,bounds.Height),
							new Rectangle(bounds.X,bounds.Height-w,bounds.Width,w)
						};


			// Fill the border-rectangles with the bg brush, since some of the 
			// 3d border types are only 1 pixel wide.
			using( Brush brush = new SolidBrush( this.BackColor ) )
			{
				for( int i = 0; i < 4; i++ )
				{
					g.FillRectangle( brush, clipRects[i] );
				}
			}

			Color borderColor = this.flatBorderColor;

			if( this.Style == VisualStyle.Office2007 )
			{
				borderColor = this.Office2007ColorTable.ComboBoxAdvNormalBorderColor;
			}
            else if (this.Style == VisualStyle.Office2010)
            {
                borderColor = this.Office2010ColorTable.ComboBoxAdvNormalBorderColor;
            }
            else if (this.Style == VisualStyle.Metro)
            {
                borderColor = ColorTranslator.FromHtml("#FFCCCCCC");
            }
			else if( this.Style == VisualStyle.OfficeXP )
			{
				if( this.IsActive )
					borderColor = MenuColors.SelBorderColor;
				else
					borderColor = this.FlatBorderColor;
			}
			else if( this.Style == VisualStyle.Office2003 )
			{
				if( this.IsActive )
					borderColor = Office2003Colors.SelBorderColor;
				else
					borderColor = this.FlatBorderColor;
			}

			if( this.BorderSides!= Border3DSide.All )
			{
				if( this.BorderSides != Border3DSide.Middle )
					cd.DrawBorder( g, bounds, borderStyle, this.border3DStyle, ButtonBorderStyle.Solid, borderColor, this.borderSides );
			}
			else
				cd.DrawBorder( g, bounds, borderStyle, this.border3DStyle, ButtonBorderStyle.Solid, borderColor );

			if( this.Style == VisualStyle.Office2007 && this.IsActive )
			{
				Rectangle rect = this.DropDownButton.Bounds;
				rect.Inflate( -1, 0 );
				Color penColor = this.Office2007ColorTable.ComboBoxAdvNormalBorderColor;

				if( this.DropDownButton.Pushed || this.DroppedDown )
				{
					penColor = this.Office2007ColorTable.ComboBoxAdvPushedBorderColor;
				}
				else if( this.DropDownButton.Hot || this.Focused )
				{
					penColor = this.Office2007ColorTable.ComboBoxAdvHotBorderColor;
				}

				using( Pen pen = new Pen( penColor ) )
				{
					g.DrawLine( pen, rect.X, rect.Y, displayRect.Right, rect.Y );
					g.DrawLine( pen, displayRect.Right - 1, rect.Y, displayRect.Right - 1, rect.Bottom );
					g.DrawLine( pen, rect.X, displayRect.Bottom - 1, displayRect.Right - 1, displayRect.Bottom - 1 );
				}
			}
            else if (this.Style == VisualStyle.Office2010 && this.IsActive)
            {
                Rectangle rect = this.DropDownButton.Bounds;
                rect.Inflate(-1, 0);
                Color penColor = this.Office2010ColorTable.ComboBoxAdvNormalBorderColor;

                if (this.DropDownButton.Pushed || this.DroppedDown)
                {
                    penColor = this.Office2010ColorTable.ComboBoxAdvPushedBorderColor;
                }
                else if (this.DropDownButton.Hot || this.Focused)
                {
                    penColor = this.Office2010ColorTable.ComboBoxAdvHotBorderColor;
                }

                using (Pen pen = new Pen(penColor))
                {
                    g.DrawLine(pen, rect.X, rect.Y, displayRect.Right, rect.Y);
                    g.DrawLine(pen, displayRect.Right - 1, rect.Y, displayRect.Right - 1, rect.Bottom);
                    g.DrawLine(pen, rect.X, displayRect.Bottom - 1, displayRect.Right - 1, displayRect.Bottom - 1);
                }
            }
            else if (this.Style == VisualStyle.Metro && this.IsActive)
            {
                Rectangle rect = this.DropDownButton.Bounds;
                rect.Inflate(-1, 0);
                Color penColor = ColorTranslator.FromHtml("#FFCCCCCC");

                if (this.DropDownButton.Pushed || this.DroppedDown)
                {
                    penColor = ColorTranslator.FromHtml("#FFCCCCCC");
                }
                else if (this.DropDownButton.Hot || this.Focused)
                {
                    penColor = ColorTranslator.FromHtml("#FFCCCCCC");
                }

                using (Pen pen = new Pen(penColor))
                {
                    g.DrawLine(pen, rect.X, rect.Y, displayRect.Right, rect.Y);
                    g.DrawLine(pen, displayRect.Right - 1, rect.Y, displayRect.Right - 1, rect.Bottom);
                    g.DrawLine(pen, rect.X, displayRect.Bottom - 1, displayRect.Right - 1, displayRect.Bottom - 1);
                }
            }
			// Return a region excluding where you just drew.
			return NativeMethods.CreateRectRgn( windowRectInScreen.Left+w, windowRectInScreen.Top+w, windowRectInScreen.Right-w, windowRectInScreen.Bottom-w );
		}

		#endregion BORDER_DRAWING_STUFF

		#region TEXTBOX
		/// <summary>
		/// Performs auto complete in the text area.
		/// </summary>
		/// <param name="e">The arguments of the KeyPress event.</param>
		protected virtual void PerformTextAutoComplete( KeyPressEventArgs e )
		{
			if( !this.HasListInterface() )
			{
				e.Handled = false;
				return;
			}

			char charCode = (char)( e.KeyChar & 255 );
			int indexFound = -1;
			int selStart = 0;
			int selLength = 0;

			if( this.Text.Length >= this.MaxLength &&
				this.DropDownStyle != ComboBoxStyle.DropDownList )
			{
				return;
			}

			selStart = this.TextBox.SelectionStart;
			selLength = this.TextBox.SelectionLength;

			//	This code is used in DropDownList mode to correct 
			//  selStart and selLength values when MatchFirstCharacterOnly
			//	is set to True. This is not .NET combo box behavior.
			//	
			if( ( DropDownStyle == ComboBoxStyle.DropDownList ) &&
				( !matchFirstCharacterOnly ) )
			{
				if( inputTextIndex > this.TextBox.Text.Length )
					inputTextIndex = this.TextBox.Text.Length;
				selStart = inputTextIndex;

				selLength = 1;
			}


			string findString = null;

			// Pass the e.KeyChar to the IsControl method, not the charCode.
			if( !Char.IsControl( e.KeyChar ) || charCode == (int)Keys.Back )
			{
				if( selStart + selLength == this.TextBox.Text.Length
					|| this.DropDownStyle == ComboBoxStyle.DropDownList )
				{
                    if (selStart == this.TextBox.Text.Length)
                    {
                        m_strTextToSearch = this.TextBox.Text;
                    }

                    if (selStart == 0 && selLength == this.TextBox.Text.Length)
                    {
                        m_strTextToSearch = string.Empty;
                    }

					findString = ( matchFirstCharacterOnly && this.DropDownStyle == ComboBoxStyle.DropDownList ) ?
						TextBox.Text.Substring( 0, selStart ) : m_strTextToSearch;

					if( charCode != (int)Keys.Back )
					{
						string beginningText = this.Text;
						findString = findString + e.KeyChar;
						m_strTextToSearch += e.KeyChar;
						string TextBoxText = "";

						int start = -1;

						m_lastFoundIndex = ( m_lastFoundIndex == -1 ) ?
							FindItem( this.TextBox.Text, false, -1, false ) : m_lastFoundIndex;
						if( this.TextBox.Text.Length > 0 && m_lastFoundIndex != -1 )
							start = m_lastFoundIndex;

						start = ( matchFirstCharacterOnly && this.DropDownStyle == ComboBoxStyle.DropDownList ) ? start : -1;

						if( beginningText.StartsWith( findString ) )
						{
                            indexFound = FindItem(findString, false, start, !CaseSensitiveAutocomplete);
							m_lastFoundIndex = indexFound;
							TextBoxText = ( indexFound < 0 ) ? beginningText : this.GetPopupControlItemText( indexFound );
							selLength = Math.Max( 0, TextBoxText.Length - findString.Length );
						}
						else
						{
							start = ( start < -1 ) ? -1 : start;
							indexFound = this.FindItem( findString, false, start, !CaseSensitiveAutocomplete );

							if( indexFound == -1 )
							{
								indexFound = this.FindItem( m_strTextToSearch, false, -1, !CaseSensitiveAutocomplete );
                                if (indexFound == -1 && this.DropDownStyle == ComboBoxStyle.DropDownList && !this.MatchFirstCharacterOnly)
                                {
                                    inputTextIndex = 0;
                                    m_strTextToSearch = e.KeyChar.ToString();
                                    indexFound = this.FindItem(m_strTextToSearch, false, -1, !CaseSensitiveAutocomplete);
                                }
							}

							if( indexFound > -1 )
							{
								TextBoxText = this.GetPopupControlItemText( indexFound );
								selLength = Math.Max( 0, TextBoxText.Length - findString.Length );
							}

							string text = ( this.CaseSensitiveAutocomplete ) ? TextBoxText : TextBoxText.ToLower();
							string foundText = ( this.CaseSensitiveAutocomplete ) ? findString : findString.ToLower();
							bool bFound = text.StartsWith( foundText );

							if( ( this.DropDownStyle != ComboBoxStyle.DropDownList ) && ( !bFound ) )
							{
								TextBoxText = findString;
							}

							m_lastFoundIndex = indexFound;
						}
						if( this.DropDownStyle == ComboBoxStyle.DropDownList )
						{
							if( indexFound > -1 )
							{
								// Allow listening to popup value change which might happen in the call to OnNewListItemKeyedIn.
								bool cached = this.ignorePopupValueChange;
								this.ignorePopupValueChange = false;
								this.TextBox.Text = TextBoxText;
								if( !this.OnNewListItemKeyedIn( indexFound ) )
								{
									this.ignorePopupValueChange = cached;
									this.SetNeedLayout( true );
								}

								this.ignorePopupValueChange = cached;
							}
						}
						else
						{
							string text = TextBoxText.Substring( findString.Length-1 );
							SetSelectedText( text, indexFound );
							selStart = findString.Length;
						}

						if( ( this.DropDownStyle == ComboBoxStyle.DropDownList )
							&& indexFound > -1 && beginningText != this.Text 
							&& !this.popupContainer.IsShowing() )
						{
							this.OnSelectionChangeCommitted( EventArgs.Empty );
						}
						e.Handled = true;
					}
					else
					{
						int backSpacePos = this.textBox.SelectionStart;
						int deleteLength = this.textBox.SelectionLength;

						if( this.Text.Length > 0 && backSpacePos > 0 )
						{
							if( this.textBox.SelectionLength == 0 )
							{
								backSpacePos--;
								deleteLength = 1;
							}

							this.m_strTextToSearch = this.Text.Remove( backSpacePos, deleteLength );
						}
					}
				}

				if( this.DropDownStyle != ComboBoxStyle.DropDownList )
				{
					this.TextBox.SelectionStart = selStart;
					this.TextBox.SelectionLength = selLength;
    			}
			}

			inputTextIndex++;
            if (charCode == (int)Keys.Enter && this.DropDownStyle == ComboBoxStyle.DropDown && !this.DroppedDown)
            {
                indexFound = m_lastFoundIndex;
            }
			this.ignorePopupValueChange = false;
            if (indexFound == -1)
            {
                this.autoCompleteSuccess = false;
            }
            else
            {
                this.autoCompleteSuccess = true;
            }
		}

        protected bool autoCompleteSuccess = false;

		protected virtual bool OnNewListItemKeyedIn( int indexFound )
		{
			return false;
		}

		protected virtual int FindItem( string prefix, bool select, int start, bool ignoreCase )
		{
			return -1;
		}
		protected virtual void SetSelectedText( string text, int foundIndex )
		{
			if( text == null )
				throw new ArgumentNullException( "foundIndex" );

			if( this.IsValidIndex( foundIndex ) )
			{
    			this.TextBox.Text = this.GetPopupControlItemText( foundIndex );
			}
			else
			{
				this.TextBox.SelectedText = text;
			}
		}
		protected virtual bool IsValidIndex( int index )
		{
			return true;
		}
		protected virtual bool HasListInterface()
		{
			return false;
		}
		protected virtual string GetPopupControlItemText( int index )
		{
			return this.Text;
		}

		private void TextBox_Click( object sender, EventArgs e )
		{
			this.OnClick( e );
		}
		private void TextBox_GotFocus( object sender, EventArgs e )
		{
			// Better to do this in Enter, but for some reason Enter is not fired!
			if( Control.MouseButtons == MouseButtons.Left )
				this.SelectInAFewMilliSecs = true;

            
			// To force the text to be selected in the text box.
			// Doesn't seem to be necessary:
			//this.textBox.SelectAll();

			// This is not good because the combo will then fire two gotfocus on tab enter.
			//this.OnGotFocus(e);

			if( this.FlatStyle != ComboFlatStyle.System )
			{
				this.InvalidateWindow();
			}
		}

		private void TextBox_LostFocus( object sender, EventArgs e )
		{
			// This is not good because the combo will then fire two lostfocus on tab enter.
			//this.OnLostFocus(e);

			if( this.FlatStyle != ComboFlatStyle.System )
			{
				this.InvalidateWindow();
			}
		}
		private void TextBox_DoubleClick( object sender, EventArgs e )
		{
			this.OnDoubleClick( e );
		}
		private void TextBox_HelpRequested( object sender, HelpEventArgs e )
		{
			this.OnHelpRequested( e );
		}
		private void TextBox_ImeModeChanged( object sender, EventArgs e )
		{
			this.OnImeModeChanged( e );
		}
		private void TextBox_MouseHover( object sender, EventArgs e )
		{
			this.OnMouseHover( e );
		}
		private void TextBox_MouseMove( object sender, MouseEventArgs e )
		{
			this.ProcessMouseMove( e, true );
		}
		private void TextBox_MouseEnter( object sender, EventArgs e )
		{
			this.mouseWasUnderChild = true;

			if( this.FlatStyle != ComboFlatStyle.System )
			{
				this.InvalidateWindow();
			}
		}
		private void TextBox_MouseLeave( object sender, EventArgs e )
		{
			this.OnMouseLeave( e );

			if( this.FlatStyle != ComboFlatStyle.System )
			{
				this.InvalidateWindow();
			}
		}
		private void TextBox_TextChanged( object sender, EventArgs e )
		{
            if (!disposing)
            {
                this.Invalidate();
                this.OnTextChanged(e);
            }
		}

		private void TextBox_QueryAccessibilityHelp( object sender, QueryAccessibilityHelpEventArgs e )
		{
			//TODO: This doesn't work?
			//this.QueryAccessibilityHelp(this, e);
		}
		private void TextBox_KeyUp( object sender, KeyEventArgs e )
		{
			this.OnTextBoxKeyUp( e );

			this.OnKeyUp( e );
		}
		protected virtual void OnTextBoxKeyUp( KeyEventArgs e )
		{
			// If dropped down or list mode, update the drop-down list.
			if( this.DroppedDown || this.DropDownStyle == ComboBoxStyle.DropDownList || 
				this.DropDownStyle == ComboBoxStyle.Simple )
			{
				this.UpdatePopupControl();
			}
		}


		/// <override/>
		protected override void OnKeyPress( KeyPressEventArgs e )
		{
			base.OnKeyPress( e );

			if( !ReadOnly && !e.Handled )
			{
				// Force NumberOnly.
				// Pass the e.KeyChar to the IsControl method, not the charCode.
				if( !( Char.IsNumber( e.KeyChar ) || e.KeyChar == (int)Keys.Back ) && NumberOnly )
				{
					e.Handled = true;
				}
				else
				{
					PerformTextAutoComplete( e );
				}
				// If dropped down or list mode, update the drop-down list.
				if( DroppedDown || DropDownStyle == ComboBoxStyle.DropDownList )
				{
					UpdatePopupControl();
				}
			}
		}
        private bool firstKeyDown = true;
		protected override void OnKeyDown( KeyEventArgs e )
		{
            if(firstKeyDown) 
			base.OnKeyDown( e );
            firstKeyDown = true;

			if( !e.Handled )
			{
				bool bShouldProcess = ( e.Alt && e.KeyCode == Keys.Down );
				bShouldProcess = bShouldProcess || ( e.Alt && e.KeyCode == Keys.Up );
				bShouldProcess = bShouldProcess || ( e.KeyCode == Keys.F4 && !e.Alt );

				if( bShouldProcess )
				{
					if( this.popupContainer.IsShowing() )
					{
						this.HidePopup();
					}
					else
					{
						this.ShowPopup();
					}
					
					e.Handled = true;
				}
			}
		}


		private void TextBox_KeyPress( object sender, KeyPressEventArgs e )
		{
			if( m_bIsInImeMode )
			{
				m_bIsInImeMode = false;
				return;
			}

			this.OnKeyPress( e );
		}

		/// <summary>
		/// Verifies whether new text is allowed to be entered from native message.
		/// </summary>
		/// <param name="m">The message.</param>
		/// <returns>True if the message is handled.</returns>
		public virtual bool PerformAllowNewText( Message m )
		{
			return false;
		}

		private void TextBox_KeyDown( object sender, KeyEventArgs e )
		{
            firstKeyDown = false;
			this.OnKeyDown( e );
			if( !e.Handled )
			{
				this.ProcessKey( e.KeyCode );
			}
		}

		private bool ProcessKey( Keys key )
		{
			if( this.ReadOnly )
				return false;

			bool popupShowing = this.popupContainer.IsShowing();

			// Hide popup if showing.
			if( popupShowing 
				&& ( key == Keys.Tab || key == ( Keys.Shift | Keys.Tab ) )
				)
			{
				this.HidePopup();
				//return True;
			}

			if( key == ( Keys.Down | Keys.Alt )
				|| key == ( Keys.Up | Keys.Alt )
				)
			{
				return true;
			}

			switch( key )
			{
				case Keys.Left:
				if( this.DropDownStyle != ComboBoxStyle.DropDownList )
					break;
				else
					goto case Keys.Up;
				case Keys.Up:
				this.MoveSelelctionInPopupControl( true );
				return true;
				case Keys.Right:
				if( this.DropDownStyle != ComboBoxStyle.DropDownList )
					break;
				else
					goto case Keys.Down;
				case Keys.Down:
				this.MoveSelelctionInPopupControl( false );
				return true;
				case Keys.Escape:
				m_strTextToSearch = string.Empty;
				if( popupShowing )
				{
					this.popupContainer.HidePopup( PopupCloseType.Canceled );
					return true;
				}
				break;
				case Keys.Return:
				m_strTextToSearch = string.Empty;
				if( popupShowing )
				{
					this.popupContainer.HidePopup( PopupCloseType.Done );
					return true;
				}
				break;
			}

			return false;
		}

		/// <override/>
		protected override bool IsInputKey( Keys keyData )
		{
			Keys keys = ( keyData & ( Keys.Alt | Keys.KeyCode ) );
			if( this.PopupContainer.IsShowing() )
			{
				if( keys == Keys.Return || keys == Keys.Escape )
					return true;
			}

			if( keys == Keys.Alt )
				return false;

			return base.IsInputKey( keyData );
		}

		/// <override/>
		protected override bool IsInputChar( char charCode )
		{
			// Want all chars.
			return true;
		}

		/// <override/>
		protected override bool ProcessDialogKey( Keys keyData )
		{
			// Remove TabStop temporarily, if losing focus.
			bool losingFocus = false;
			bool oldTabStop = this.TabStop;
			bool processed = false;
			if( keyData == ( Keys.Shift | Keys.Tab ) )
			{
				losingFocus = true;
				oldTabStop = this.TabStop;
				this.TabStop = false;
			}
			else if( keyData == Keys.Down || keyData == Keys.Up )
			{
				if( !base.ProcessDialogKey( keyData ) )
				{
					processed = this.ProcessKey( keyData );
				}
			}

			if( !processed )
				processed = base.ProcessDialogKey( keyData );
			if( losingFocus )
				this.TabStop = oldTabStop;

			return processed;
		}
		/// <override/>
		protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
		{
			if( !this.ReadOnly )
			{
				KeyEventArgs e = new KeyEventArgs(keyData);
				this.OnKeyDown(e);

				if (!e.Handled)
				{
					if (this.ProcessKey(keyData))
					{
						if (keyData == Keys.Down || keyData == Keys.Up)
							this.TextBox.SelectAll();

						return true;
					}
					else
					{
						switch (keyData)
						{
							case Keys.PageDown:
							case Keys.PageUp:
								if (this.popupContainer.IsShowing())
								{
									this.ForwardKeyMessageToPopupControl(msg.Msg, msg.WParam, msg.LParam);
									this.UpdateText(false);
									return true;
								}
								break;
						};
					}
				}
				else return true;
			}
			
			return base.ProcessCmdKey( ref msg, keyData );
		}

		/// <summary>
		/// Forwards the key message to the attached <see cref="PopupControl"/>.
		/// </summary>
		/// <param name="msg">The message id.</param>
		/// <param name="wparam">The first message parameter.</param>
		/// <param name="lparam">The second message parameter.</param>
		protected virtual void ForwardKeyMessageToPopupControl( int msg, IntPtr wparam, IntPtr lparam )
		{
			if( this.PopupControl == null )
				return;
			NativeMethods.SendMessage( this.PopupControl.Handle, msg, wparam, lparam );
		}

		#endregion TEXTBOX
		#region DROPDOWN
		private void Popup_Closed( object sender, PopupClosedEventArgs e )
		{
			this.ContextMenu = this.cachedContextMenu;
			this.cachedContextMenu = null;
			this.QuickSelectOn = false;

			this.ddButton.IsDroppedDown = false;
			this.UpdateIsActiveState();

			if( e.PopupCloseType == PopupCloseType.Done )
			{
				if( !this.UpdateText( true ) && !OtherChangesMade() )
				{
					if( this.cachedTextBeforePopup != this.Text )
						this.OnSelectionChangeCommitted( EventArgs.Empty );
					else
					{
						this.OnNoChangeDetectedOnPopupClosed();
					}
				}
				else
					this.OnSelectionChangedOnPopupClose();
			}
			else if( this.cachedTextBeforePopup != this.Text )
			{
				this.UpdatePopupControl();
			}

			this.cachedTextBeforePopup = String.Empty;

			if( this.DropDownStyle == ComboBoxStyle.DropDownList )
				this.Invalidate();

			this.OnPopupClosed(e);
		}
		[Documentation.DocumentationExclude(), Browsable( false )]
		protected virtual void OnSelectionChangedOnPopupClose()
		{
		}
		/// <summary>
		/// Called when the popup is closed.
		/// </summary>
        protected virtual void OnPopupClosed(PopupClosedEventArgs e)
		{

		}

		/// <summary>
		/// Updates the attached popup control based on the current <see cref="Text"/> property.
		/// </summary>
		/// <remarks>
		/// Call this method to update the popup list box, for example, with the latest text
		/// value that the user might have entered. This is useful when you enable auto completion
		/// in the text area and force the list box's SelectedValue to be updated to the
		/// latest text value in this control's Validated event.
		/// </remarks>
		public virtual void UpdatePopupControl()
		{
			if( this.PopupControl != null )
			{
				bool oldValue = this.ignorePopupValueChange;
				this.ignorePopupValueChange = true;
				this.SetPopupText( this.Text );
				this.ignorePopupValueChange = oldValue;
			}
		}

		protected virtual void OnNoChangeDetectedOnPopupClosed()
		{
		}
		private void Popup_BeforePopup( object sender, CancelEventArgs e )
		{
			this.OnBeforePopup();

			inputTextIndex = 0;
			this.cachedTextBeforePopup = this.Text;
			this.cachedContextMenu = this.ContextMenu;
			this.ContextMenu = null;
			this.ddButton.IsDroppedDown = true;
			this.OnDropDown( EventArgs.Empty );
		}
		/// <summary>
		/// Called before the popup gets dropped down.
		/// </summary>
		protected virtual void OnBeforePopup()
		{

		}

		private void Popup_HandleCreated( object sender, EventArgs e )
		{
            if (this.Style == VisualStyle.Office2007 || this.Style == VisualStyle.Office2010)
			{
				AttachScrollersFrame( this.PopupControl );
				InitScrollersFrame();
			}
		}
		private void DDButtonPressed( object sender, EventArgs e )
		{
			this.ProcessDDMouseDown();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void ProcessDDMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				this.ProcessDDMouseDown();
		}

		protected virtual void ProcessDDMouseDown()
		{
			if( !this.ContainsFocus )
			{
				if( this.textBox.Visible )
					this.textBox.Focus();
				else
					this.Focus();
			}

			if( !this.ReadOnly )
			{
				if( this.popupContainer.IsShowing() )
					this.HidePopup();
				else if( this.ContainsFocus )
				{
					this.ShowPopup();
				}
			}
		}
		/// <summary>
		/// Shows the drop-down.
		/// </summary>
		protected virtual void ShowPopup()
		{
            if(this.DropDownStyle == ComboBoxStyle.Simple || this.ReadOnly)
				return;

			this.SetNeedLayout( true );

			this.QuickSelectOn = false;

			// Required since calling Visible = True could reset the popup control's value (list control's 
			// selected index, for example) with a call to BindingContextChanged.
			this.ignorePopupValueChange = true;

			this.ignorePopupValueChange = false;

			this.popupContainer.Width = this.DropDownWidth;

			this.popupContainer.ShowPopup( Point.Empty );

			if( this.PopupControl != null )
				this.PopupControl.Visible = true;
		}
		/// <summary>
		/// Hides the drop-down list box.
		/// </summary>
		protected virtual void HidePopup()
		{
			this.popupContainer.HidePopup();
		}
		#endregion DROPDOWN
		#region UI
		/// <override/>
		protected override void SetBoundsCore(
			int x,
			int y,
			int width,
			int height,
			BoundsSpecified specified
			)
		{
			if( height <= 0 )
				// Height cannot be 0. We need to check this rather than in OnPaint, since OnPaint will not be called if set to 0!
				height = this.Height;
            using (Graphics g = this.CreateGraphics())
            {
                if (g.DpiX > 96)
                    this.preventHeightChange = false;
                else
                    this.preventHeightChange = true;
            }
			if( this.preventHeightChange )
			{
				specified &= ~BoundsSpecified.Height;

				Rectangle curBounds = this.Bounds;

				int ncwidth = ( this.Style == VisualStyle.Default ) ? c_iDefaultBorderWidthVSStyle : c_iDefaultBorderWidth;
				this.editPortionHeight = this.DropDownButtonHeight + ncwidth * 2;
				height = this.editPortionHeight;

				if( curBounds.X != x || curBounds.Y != y || curBounds.Width != width || curBounds.Height != height )
					base.SetBoundsCore( x, y, width, height, specified );
			}
            else
            {
                int ncwidth = (this.Style == VisualStyle.Default) ? c_iDefaultBorderWidthVSStyle : c_iDefaultBorderWidth;
                this.editPortionHeight = this.DropDownButtonHeight + ncwidth * 2;
                height = this.editPortionHeight;
                base.SetBoundsCore(x, y, width, height, specified);
            }
		}

		/// <override/>
		protected override void OnLayout( LayoutEventArgs levent )
		{
			SetNeedLayout( true );
			base.OnLayout( levent );
		}

		internal void MenuColorsChanged( object sender, EventArgs e )
		{
            // This helps if MenuColors were changed programmatically.
			this.Invalidate();
		}
		/// <summary>
		/// Forces laying out of the combo box elements.
		/// </summary>
		/// <param name="g">The Graphics object using which element sizes and positions are calculated.</param>
		/// <remarks>
		/// Advanced method. You do not have to call this directly.
		/// </remarks>
		public new virtual void Layout( Graphics g )
		{
			this.SetNeedLayout( false );
			int textAreaHeight = 0;

			this.UpdatePopupControlRelationship();
			this.DetermineHeightsBasedOnFont( g, ref textAreaHeight );
			this.UpdateEditPortionBounds( textAreaHeight );
			this.UpdateDropDownButtonBounds();
			this.UpdatePopupControlBounds();
		}
		/// <summary>
		/// Forces laying out of the combo elements within the next Paint Message handler.
		/// </summary>
		/// <param name="needLayout">True to force; False to prevent layout.</param>
		protected virtual internal void SetNeedLayout( bool needLayout )
		{
			this.needLayout = needLayout;
			if( this.needLayout )
				this.Invalidate( true );
		}
		/// <summary>
		/// Indicates whether the layout method needs to be called to layout the combo
		/// elements.
		/// </summary>
		/// <remarks>
		/// Internal method. You will not have to call this property explicitly.
		/// </remarks>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public bool NeedLayout
		{
			get { return needLayout; }
		}

		/// <summary>
		/// Determines the heights of certain portions of this control.
		/// </summary>
		/// <param name="g">A <see cref="System.Drawing.Graphics"/> object.</param>
		/// <param name="textAreaHeight">A reference variable through which to return the height for the text area.</param>
		/// <remarks>
		/// <para>
		/// Make sure to call the base class when you override this method.
		/// </para>
		/// <para>
		/// This method expects you to return a height for the text area through the reference variable,
		/// set the height of this control (if not in ComboBoxStyle.Simple mode) and the height
		/// of the drop-down button (<see cref="DropDownButtonHeight"/>).
		/// </para>
		/// </remarks>
		protected virtual void DetermineHeightsBasedOnFont( Graphics g, ref int textAreaHeight )
		{
			// Use the text box to determine the preferred height.
			int ncwidth = ( this.Style == VisualStyle.Default ) ? c_iDefaultBorderWidthVSStyle : c_iDefaultBorderWidth;
			string text = this.Text;
			Size textAreaSize;

			if( text == String.Empty )
			{
				text = this.ToString();
			}

			textAreaSize = ControlDrawing.MeasureDisplayStringSize( g, text, this.Font, ( this.RightToLeft == RightToLeft.Yes ) );

			// Button Size.
			this.DropDownButtonHeight = textAreaSize.Height + 4;
			if( this.Style != VisualStyle.Default )
				this.DropDownButtonHeight += 2; // because ncheight is only 1

			textAreaHeight = textAreaSize.Height;

			// Determine control height.
			this.editPortionHeight = this.DropDownButtonHeight + ncwidth * 2;

			if( this.DropDownStyle != ComboBoxStyle.Simple
				|| this.Height < this.editPortionHeight )
			{
				this.Height = this.editPortionHeight;
			}
		}

		/// <summary>
		/// Updates the internal textbox's bounds and visibility based on the <see cref="DropDownStyle"/>.
		/// </summary>
		/// <param name="textAreaHeight">The height of the text area.</param>
		protected virtual void UpdateEditPortionBounds( int textAreaHeight )
		{
			this.SuspendLayout();

			int ddWidth = this.GetComboBoxDropDownWidth();
			if( this.DropDownStyle == ComboBoxStyle.Simple )
				ddWidth = 0;
			else if( this.Style != VisualStyle.Default )
			{
				ddWidth = this.GetOfficeStyleComboBoxDropDownWidth();
			}

			int ncwidth = ( this.Style == VisualStyle.Default ) ? c_iDefaultBorderWidthVSStyle : c_iDefaultBorderWidth;

			// Same logic used in UpdateDropDownButtonBounds.
			int textBoxWidth = this.Bounds.Width - 2/*1 pixel space after borders*/ - ( ncwidth * 2 ) /*borders*/
				- ddWidth;

			// Center the text box based on its height.
			int textBoxTop = 0/*Not ClientRectangle.Top*/ + 
				( ( this.editPortionHeight - textAreaHeight ) / 2 );
			if( this.FlatStyle != ComboFlatStyle.System )
				textBoxTop -= ncwidth;
			int textBoxLeft = 0/*Not ClientRectangle.Left*/ + ncwidth + 1;
			if( this.FlatStyle != ComboFlatStyle.System )
				textBoxLeft -= ncwidth;
			// Set the bounds in all modes.

			textBoxLeft = ( RightToLeft == RightToLeft.Yes ) ? textBoxLeft + ddWidth : textBoxLeft;

			Rectangle textRect = new Rectangle( textBoxLeft, textBoxTop,
				textBoxWidth, textAreaHeight );
            using (Graphics g = this.CreateGraphics())
            {
                if (g.DpiX > 96)
                {
                    if (this.textBox.BorderStyle == BorderStyle.None)
                        this.textBox.BorderStyle = BorderStyle.FixedSingle;
                    this.textBox.BorderColor = this.BackColor;
                    if (this.DesignMode)
                        textRect = new Rectangle(textBoxLeft, 0, textBoxWidth, textAreaHeight);
                    else
                        textRect = new Rectangle(textBoxLeft, -2, textBoxWidth, textAreaHeight);
                }
            }
			this.SetTextBoxBounds( textRect );

			// Make it visible if not DropDownList.
			if( this.DropDownStyle != ComboBoxStyle.DropDownList && !this.DesignMode )
				this.textBox.Visible = true;
			else
				this.textBox.Visible = false;

			this.ResumeLayout( false );
		}

		protected virtual void SetTextBoxBounds( Rectangle rect )
		{
			this.textBox.Bounds = rect;
		}

		/// <summary>
		/// Returns the width for the combo box drop-down button.
		/// </summary>
		/// <returns>The button width.</returns>
		protected virtual int GetComboBoxDropDownWidth()
		{
			return SystemInformation.HorizontalScrollBarThumbWidth;
			//			return (int)(3 + (ComboBoxOfficeStyleDropDownWidth - 3) * 
			//				(SystemInformation.MenuCheckSize.Width / 13f/*13 is the standard size of the check boxes.*/)
			//				);
		}
		/// <summary>
		/// Returns the width for the combo box drop-down button when office style is on.
		/// </summary>
		/// <returns>The button width.</returns>
		protected virtual int GetOfficeStyleComboBoxDropDownWidth()
		{
			return this.GetComboBoxDropDownWidth() - 2;
		}
		/// <summary>
		/// Updates the bounds of the drop-down button bounds.
		/// </summary>
		/// <remarks>
		/// Sets the bounds based on the value returned by the 
		/// <see cref="GetComboBoxDropDownWidth"/> method.
		/// </remarks>
		protected virtual void UpdateDropDownButtonBounds()
		{
			int ddWidth = this.GetComboBoxDropDownWidth();
			if( this.DropDownStyle == ComboBoxStyle.Simple )
				ddWidth = 0;
			else if( this.Style != VisualStyle.Default )
			{
				ddWidth = this.GetOfficeStyleComboBoxDropDownWidth();
			}
			int ncwidth = ( this.Style == VisualStyle.Default ) ? c_iDefaultBorderWidthVSStyle : c_iDefaultBorderWidth;
			// Same logic used in UpdateEditPortionBounds.
			int left = this.Bounds.Width - ddWidth - ncwidth;
			int top = 0/*Not ClientRectangle.Top*/ + ncwidth;
			if( this.FlatStyle != ComboFlatStyle.System )
			{
				left -= ncwidth;
				top -= ncwidth;
			}

			left = ( RightToLeft == RightToLeft.Yes ) ? 0 : left;
			Rectangle ddBounds = new Rectangle( left,
				top, ddWidth, DropDownButtonHeight );
			this.ddButton.Bounds = ddBounds;
		}

		/// <summary>
		/// Updates the attached <see cref="PopupControl"/>'s bounds.
		/// </summary>
		/// <remarks>
		/// The base class updates the PopupControl's bounds and visibility based on the 
		/// <see cref="DropDownStyle"/> settings.
		/// </remarks>
		protected virtual void UpdatePopupControlBounds()
		{
			if( PopupControl == null || DesignMode )
			{
				return;
			}

			SuspendLayout();

			if( DropDownStyle == ComboBoxStyle.Simple )
			{
				PopupControl.Visible = false;
			}
			else
			{
				if( !PopupContainer.IsShowing() )
				{
					PopupControl.Visible = false;
				}
			}

			ResumeLayout( false );
		}

		/// <summary>
		/// Overloaded. The bounds for the control that is associated with this popup when in DropDownStyle.Simple mode.
		/// </summary>
		/// <param name="comboHeight">The height of the combo control.</param>
		/// <returns>The bounds for the embedded control.</returns>
		protected virtual Rectangle GetEmbeddedChildBounds( int comboHeight )
		{
			int left = 0;
			int top = 0;
			int width = 0;
			int height = 0;

			if( comboHeight - this.editPortionHeight <= 3 )
			{
				left = 0/*Not ClientRectangle.Left*/;
				top = this.editPortionHeight + 1;
				width = this.Bounds.Width;
				height = comboHeight - this.editPortionHeight;
			}
			else
			{
				left = 0/*Not ClientRectangle.Left*/ + 2;
				top = this.editPortionHeight + 2;
				if( FlatStyle != ComboFlatStyle.System )
				{
					left -= 2;
					top -= 2;
				}
				width = this.Bounds.Width - 4;
				height = comboHeight - this.editPortionHeight - 4;
				//if(FlatStyle != ComboFlatStyle.System)
				//	height -= 1;
			}
			return new Rectangle( left, top, width, height );
		}
		/// <summary>
		/// The bounds for the control that is associated with this popup when in DropDownStyle.Simple mode.
		/// </summary>
		/// <returns>The bounds for the embedded control.</returns>
		protected virtual Rectangle GetEmbeddedChildBounds()
		{
			return this.GetEmbeddedChildBounds( this.Height );
		}

		/// <override/>
		protected override void OnPaint( PaintEventArgs e )
		{
			MenuColors.UpdateMenuColors();
			Office2003Colors.UpdateMenuColors();

			if( NeedLayout )
				this.Layout( e.Graphics );

			this.DrawEditPortionBorderAndBackground( e );
			this.DrawEditPortion( e );
			this.DrawDropDownPortion( e );
			this.DrawListPortion( e );

			base.OnPaint( e );
		}
		/// <summary>
		/// Called from the <b>Paint</b> event handler to draw the text portion.
		/// </summary>
		/// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> from the Paint event.</param>
		/// <remarks>
		/// This method calls the <see cref="DrawListModeEditPortion"/> method
		/// to draw the edit portion only when in <b>ComboBoxStyle.DropDownList</b> mode
		/// or when in design-mode.
		/// </remarks>
		protected virtual void DrawEditPortion( PaintEventArgs e )
		{
			if( this.DropDownStyle == ComboBoxStyle.DropDownList
				|| this.DesignMode )
			{
				this.DrawListModeEditPortion( e, SystemColors.Highlight, SystemColors.ActiveCaptionText, true );
			}
		}
		/// <summary>
		/// Called from <see cref="DrawEditPortion"/> to draw the text area when in ComboBoxStyle.DropDownList mode.
		/// </summary>
		/// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> from the Paint event.</param>
		/// <param name="highlightBG">The background color for the highlight.</param>
		/// <param name="highlightText">The text color for the highlight.</param>
		/// <param name="drawFocusRect">Indicates whether to draw a focus rectangle.</param>
		protected virtual void DrawListModeEditPortion( PaintEventArgs e, Color highlightBG, Color highlightText, bool drawFocusRect )
		{
			Graphics g = e.Graphics;
			Color foreColor = this.ForeColor;
			if( this.ContainsFocus && !this.popupContainer.IsShowing()
                && this.Style != VisualStyle.Office2007 && this.Style != VisualStyle.Office2010 && this.Style != VisualStyle.Metro)
			{
				using( Brush brush = new SolidBrush( highlightBG ) )
				{
					g.FillRectangle( brush, this.textBox.Bounds );
				}

				Rectangle focusRect = this.textBox.Bounds;
				focusRect.Inflate( 1, 1 );

				foreColor = highlightText;

				if( drawFocusRect )
					ControlPaint.DrawFocusRectangle( g, focusRect, foreColor, this.BackColor );
			}

			// Draw the text myself.
			TextFormatFlags tf = TextFormatFlags.SingleLine | TextFormatFlags.TextBoxControl;
            if (!this.useMnemonic)
            {
                tf |= TextFormatFlags.NoPrefix;
            }
			HorizontalAlignment hAlign = this.TextAlign;

			switch( hAlign )
			{
				case HorizontalAlignment.Left:
				tf |=  TextFormatFlags.Left;
				break;
				case HorizontalAlignment.Center:
				tf |= TextFormatFlags.HorizontalCenter;
				break;
				case HorizontalAlignment.Right:
				tf |= TextFormatFlags.Right;
				break;
			}

			if( RightToLeft.Yes == this.RightToLeft )
			{
				tf |= TextFormatFlags.RightToLeft;

				if( hAlign != HorizontalAlignment.Center )
				{
					tf ^= TextFormatFlags.Left | TextFormatFlags.Right;
				}
			}

			if( !this.Enabled )
			{
				foreColor = SystemColors.GrayText;
			}

			TextRenderer.DrawText( g, this.textBox.Text, this.Font, this.textBox.Bounds, foreColor, tf );
		}

		/// <summary>
		/// Called from the <b>Paint</b> event handler to draw the drop-down button.
		/// </summary>
		/// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> from the Paint event.</param>
		protected virtual void DrawDropDownPortion( PaintEventArgs e )
		{
			this.ddButton.OnPaint( e.Graphics );
            if (this.Style != VisualStyle.Default && this.Style != VisualStyle.Office2007 && this.Style != VisualStyle.Office2010 && this.Style != VisualStyle.Metro)
			{
				Color borderColor = Color.Empty;
				if( this.IsActive )
				{
					if( this.Style == VisualStyle.OfficeXP )
						borderColor = MenuColors.SelBorderColor;
					else
						borderColor = Office2003Colors.SelBorderColor;
				}
				else
					borderColor = this.FlatBorderColor;

				using( Pen pen = new Pen( borderColor ) )
				{
					e.Graphics.DrawLine( pen, new Point( this.ddButton.Bounds.X - 1, 0 ),
						new Point( this.ddButton.Bounds.X - 1, this.ddButton.Bounds.Bottom - 1 ) );
				}
			}
		}
		/// <summary>
		/// Called from the <b>Paint</b> event handler to draw the edit portion's border and background.
		/// </summary>
		/// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> from the Paint event.</param>
		/// <remarks>
		/// This method calls the <see cref="DrawBorderAndBackground"/> method with the appropriate
		/// dimension to draw the border around the text portion.
		/// </remarks>
		protected virtual void DrawEditPortionBorderAndBackground( PaintEventArgs e )
		{
			int left = 0;
			int top = 0;
			if( this.FlatStyle != ComboFlatStyle.System )
			{
				left -= 2;
				top -= 2;
			}

			this.DrawBorderAndBackground( e.Graphics, new Rectangle( left, top,
				this.Width, this.editPortionHeight ) );
		}
		/// <summary>
		/// Draws the border and background of the control.
		/// </summary>
		/// <param name="g">The <see cref="System.Drawing.Graphics"/> context.</param>
		/// <param name="rect">The <see cref="System.Drawing.Rectangle"/> within which to draw.</param>
		/// <remarks>
		/// <para>
		/// This method is used to draw the border around the text area (when called from 
		/// <see cref="DrawEditPortionBorderAndBackground"/> method) and around the list box area (when in 
		/// ComboBoxStyle.Simple mode and called from the <see cref="DrawListPortion"/> method).
		/// </para>
		/// <para>This method uses themes to draw if necessary or calls <see cref="DrawUnThemedBackground"/> and 
		/// <see cref="DrawUnThemedBorder"/> to draw the background and border.</para>
		/// </remarks>
		protected virtual void DrawBorderAndBackground( Graphics g, Rectangle rect )
		{
			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled )
			{
				this.themedDrawing.DrawEditBoxBackground( g, rect, this.Enabled && !this.ReadOnly );

				// Draw Background.
				Size borderWidth = this.themedDrawing.GetThemeBorderSize();
				Rectangle bgRect =  rect;

				bgRect.Inflate( -borderWidth.Width, -borderWidth.Height );
				this.DrawUnThemedBackground( g, bgRect );
			}
			else
			{
				this.DrawUnThemedBackground( g, rect );

				if( this.Style == VisualStyle.Default )
					this.DrawUnThemedBorder( g, rect );
			}
		}
		/// <summary>
		/// Draws the unthemed border of this control.
		/// </summary>
		/// <param name="g">The <see cref="System.Drawing.Graphics"/> context.</param>
		/// <param name="rect">The <see cref="System.Drawing.Rectangle"/> within which to draw.</param>
		/// <remarks>
		/// <para>Called by <see cref="DrawBorderAndBackground"/> to draw the border when
		/// not in themes mode.</para>
		/// </remarks>
		protected virtual void DrawUnThemedBorder( Graphics g, Rectangle rect )
		{
			if( this.FlatStyle != ComboFlatStyle.Flat )
				ControlPaint.DrawBorder3D( g, rect, this.border3DStyle );
			else
				ControlPaint.DrawBorder( g, rect, this.FlatBorderColor, ButtonBorderStyle.Solid );
		}

		/// <summary>
		/// Draws the unthemed background of this control.
		/// </summary>
		/// <param name="g">The <see cref="System.Drawing.Graphics"/> context.</param>
		/// <param name="rect">The <see cref="System.Drawing.Rectangle"/> within which to draw.</param>
		/// <remarks>
		/// <para>Called by <see cref="DrawBorderAndBackground"/> to draw the background when
		/// not in themes mode.</para>
		/// </remarks>
		protected virtual void DrawUnThemedBackground( Graphics g, Rectangle rect )
		{
			Color color = this.BackColor;
			if( ( !this.Enabled || this.ReadOnly ) && this.BackColor == DefaultComboBackColor )
			{
				color = SystemColors.Control;
			}

			using( Brush brush = new SolidBrush( color ) )
			{
				g.FillRectangle( brush, rect );
			}
		}

		/// <summary>
		/// Called from the <b>Paint</b> event handler to draw the list portion.
		/// </summary>
		/// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> from the Paint event.</param>
		/// <remarks>
		/// This method draws the list portion when in ComboBoxStyle.Simple mode.
		/// </remarks>
		protected virtual void DrawListPortion( PaintEventArgs e )
		{
			if( this.DropDownStyle != ComboBoxStyle.Simple )
				return;

			int listTop = this.editPortionHeight + 1;
			if( this.Height - listTop > 3 )
			{
				int left = 0;
				int top = listTop - 1;
				if( this.FlatStyle != ComboFlatStyle.System )
				{
					left -= 1;
					top -= 2;
				}

				// Draw list border.
				this.DrawBorderAndBackground( e.Graphics, new Rectangle( left, top,
					this.Bounds.Width, this.Height - listTop + 1 ) );
			}
		}
		/// <summary>
		/// Refreshes the BackColor of the control after changing 'IgnoreThemeBackground' property.
		/// </summary>
		/// <param name="ignoreThemeBackColor">Value of 'IgnoreThemeBackground' property.</param>
		private void RefreshBackColor( bool ignoreThemeBackColor )
		{
			// Restore BackColor of control.
            if (this.Style != VisualStyle.Office2007 && this.Style != VisualStyle.Office2010 && this.Style != VisualStyle.Metro)
			{
				if( ignoreThemeBackColor )
				{
					this.BackColor = cachedBackColor;
				}
				else
				{
					// set default BackColor to control.
					Color oldColor  = cachedBackColor;
					this.BackColor  = DefaultComboBackColor;
					cachedBackColor = oldColor;
				}
			}
		}

		/// <summary>
		/// Creates new instance of the scrollersFrame.
		/// </summary>
		protected ScrollersFrame CreateScrollersFrame()
		{
			return new ScrollersFrame();
		}

		/// <summary>
		/// Attaches scrollersFrame to the PopupControl.
		/// </summary>
		protected void AttachScrollersFrame( Control control )
		{
			if( m_scrollersFrame == null )
			{
				m_scrollersFrame = this.CreateScrollersFrame();
			}

			if( m_scrollersFrame.AttachedTo != control )
			{
				m_scrollersFrame.AttachedTo = control;
			}
		}

		/// <summary>
		/// Detaches scrollersFrame from the PopupControl.
		/// </summary>
		protected void DetachScrollersFrame()
		{
			if( m_scrollersFrame != null )
			{
				m_scrollersFrame.DetachFrame();
			}
		}

		/// <summary>
		/// Inits the scrollers frame.
		/// </summary>
		protected void InitScrollersFrame()
		{
            if (m_scrollersFrame != null)
            {
                if (this.Style == VisualStyle.Metro)
                {
                    m_scrollersFrame.VerticallSmallChange = 1;
                    m_scrollersFrame.VisualStyle = ScrollBarCustomDrawStyles.Metro;
                    m_scrollersFrame.VerticalScroller.Width = c_scrollerWidth;
                    m_scrollersFrame.VerticalScroller.Left += c_adjustScrollerPosition;
                 }
                else
                {
                    m_scrollersFrame.VerticallSmallChange = 1;
                    m_scrollersFrame.VisualStyle = ScrollBarCustomDrawStyles.Office2007Generic;
                    m_scrollersFrame.VerticalScroller.Width = c_scrollerWidth;
                    m_scrollersFrame.VerticalScroller.Left += c_adjustScrollerPosition;
                    m_scrollersFrame.OfficeColorScheme = (Office2007ColorScheme)this.Office2007ColorTheme;
                }
            }
		}
		#endregion UI
		#region MOUSE_MESSAGES
		/// <override/>
		protected override void OnMouseMove( MouseEventArgs e )
		{
            // Turn off quick select if on.
            if (prevMouseLocation != Point.Empty && e.Location != prevMouseLocation)
            {
                prevMouseLocation = Point.Empty;
            }
			this.ignoreNextPopupControlMouseMove = false;

			this.ProcessMouseMove( e, false );

			base.OnMouseMove( e );
		}
		/// <summary>
		/// Processes mouse moves on the combo-box and textbox area.
		/// </summary>
		/// <param name="e">The event args of the MouseMove event.</param>
		/// <param name="fromTextBox">Indicates whether this was called due to mouse move in the text area or combo.</param>
		protected virtual void ProcessMouseMove( MouseEventArgs e, bool fromTextBox )
		{
			this.ignoreNextPopupControlMouseMove = false;

			if( !fromTextBox && !this.popupContainer.IsShowing() )
			{
				this.ddButton.OnMouseMove( e, this.DropDownStyle == ComboBoxStyle.DropDownList );
			}
			this.IsActive = true;

			if( this.popupContainer.IsShowing() )
			{
				this.ProcessPopupControlMouseMove( this, e );
			}
		}
		protected virtual void ProcessPopupControlMouseMove( object sender, MouseEventArgs e )
		{

		}


		private bool IsActive
		{
			get { return this.isActive; }
			set
			{
				if( this.isActive != value )
				{
					this.ddButton.IsActive = value;
					this.isActive = value;

					if( this.Style == VisualStyle.Office2007 && !this.UseBackColor )
					{
						if( value )
						{
							this.BackColor = this.Office2007ColorTable.ComboBoxAdvNormalBackColor;
						}
						else
						{
							this.BackColor = this.Office2007ColorTable.ComboBoxAdvHotBackColor;
						}
					}
                    if (this.Style == VisualStyle.Office2010 && !this.UseBackColor)
                    {
                        if (value)
                        {
                            this.BackColor = this.Office2010ColorTable.ComboBoxAdvNormalBackColor;
                        }
                        else
                        {
                            this.BackColor = this.Office2010ColorTable.ComboBoxAdvHotBackColor;
                        }
                    }
					else if (this.Style == VisualStyle.Metro && !this.UseBackColor)
					{
						this.BackColor =Color.White; 
					}
					//this.InvalidateWindow();
				}
			}
		}
		/// <summary>
		///Gets the Isactive state
		/// </summary>
		private void UpdateIsActiveState()
		{
            if ((this.ComboInFocus && !( (this.useOffice2007ColorsInActiveMode || this.useMetroColorsInActiveMode) && this.DropDownStyle == ComboBoxStyle.DropDownList)) || this.ClientRectangle.Contains(this.PointToClient(Control.MousePosition))
				|| this.DroppedDown )
				this.IsActive = true;
			else
				this.IsActive = false;
		}
		/// <override/>
		protected override void OnMouseLeave( EventArgs e )
		{
			this.ddButton.OnMouseLeave( e );

			Point pt = Control.MousePosition;
			pt = this.PointToClient( pt );
			if( this.textBox.Visible && this.textBox.Bounds.Contains( pt ) )
				return;
			else
			{
				this.UpdateIsActiveState();
				base.OnMouseLeave( e );
			}
			if( this.FlatStyle != ComboFlatStyle.System )
			{
                this.InvalidateWindow();
			}
		}
		/// <override/>
		protected override void OnMouseEnter( EventArgs e )
		{
			if( this.mouseWasUnderChild )
				this.mouseWasUnderChild = false;
			else
				base.OnMouseEnter( e );

			if( this.FlatStyle != ComboFlatStyle.System )
			{
                this.InvalidateWindow();
			}
		}
		/// <override/>
		protected override void OnMouseDown( MouseEventArgs e )
		{
			bool listShowing = this.popupContainer.IsShowing();
            prevMouseLocation = e.Location;
			this.ddButton.OnMouseDown( e, this.DropDownStyle == ComboBoxStyle.DropDownList );
			if( this.DropDownStyle == ComboBoxStyle.DropDownList && this.Enabled && !this.ReadOnly )
			{
				// If in list mode and clicked on the text area, then act as if clicked on the drop-down.
				if( listShowing == this.popupContainer.IsShowing() && !this.popupContainer.args.Cancel)
					this.ProcessDDMouseDown(e);
			}

			base.OnMouseDown( e );
		}
		/// <override/>
		protected override void OnMouseUp( MouseEventArgs e )
		{
			if( DropDownStyle == ComboBoxStyle.DropDownList )
				ddButton.Pushed = false;

			if( DropDownStyle == ComboBoxStyle.DropDown )
				ddButton.OnMouseUp( e );

			if( this.popupContainer.IsShowing()
				|| this.DropDownStyle != ComboBoxStyle.DropDown )
			{
                bool allowQuickMouseSeletion = true;
                if (prevMouseLocation != Point.Empty && e.Location == prevMouseLocation)
                {
                    allowQuickMouseSeletion = false;
                }
                if (this.QuickSelectOn && allowQuickMouseSeletion)
				{
					this.OnMouseUpOnQuickSelect();
				}
			}
			// Turn off quick select if on.
			this.QuickSelectOn = false;

			base.OnMouseUp( e );
		}
		protected virtual void OnMouseUpOnQuickSelect()
		{
		}
		protected override void OnMouseWheel( MouseEventArgs e )
		{
			if( !this.ReadOnly )
			{
				if( e.Delta != 0 )
				{
                    if (!this.PopupContainer.IsShowing())
                    {
                        if(allowMouseWheelSelection)
                            this.MoveSelelctionInPopupControl(e.Delta > 0);
                    }
                    else
                    {
                        // Send the mouse wheel message to the hosted list.
                        int wlow = NativeMethods.MK_MBUTTON;
                        if ((Control.ModifierKeys & Keys.Shift) > 0)
                            wlow &= NativeMethods.MK_SHIFT;
                        if ((Control.ModifierKeys & Keys.Control) > 0)
                            wlow &= NativeMethods.MK_CONTROL;

                        int whigh = e.Delta;

                        int llow = Control.MousePosition.X;
                        int lhigh = Control.MousePosition.Y;
                        IntPtr wParam = (IntPtr)NativeMethods.MAKELONG(wlow, whigh);

                        if (this.PopupControl != null && this.PopupControl.IsHandleCreated)
                        {
                            NativeMethods.SendMessage(this.PopupControl.Handle,
                                NativeMethods.WM_MOUSEWHEEL,
                                wParam,
                                NativeMethods.MAKELPARAM(llow, lhigh)
                                );
                        }
                    }
				}
			}

			if( e is HandledMouseEventArgs )
			{
				( (HandledMouseEventArgs)e ).Handled = true;
			}

			base.OnMouseWheel( e );
		}
		/// <summary>
		/// Moves the current selection in the attached <see cref="PopupControl"/>.
		/// </summary>
		/// <param name="up">Indicates whether to move up.</param>
		protected virtual void MoveSelelctionInPopupControl( bool up )
		{
			m_lastFoundIndex = ( up ) ? --m_lastFoundIndex : ++m_lastFoundIndex;
			m_strTextToSearch = string.Empty;
		}
		#endregion MOUSE_MESSAGES

		#region THEMES
		/// <summary>
		/// Fired when the ThemesEnabled property changes.
		/// </summary>
		[Description( "This event will be fired when the ThemesEnabled property changes." ),
		Category( "Appearance" )]
		public event EventHandler ThemeChanged;
		/// <override/>
		[SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode=true )]
		protected override void WndProc( ref Message m )
		{
			if( this.cachedRgn != IntPtr.Zero )
			{
				NativeMethods.DeleteObject( this.cachedRgn );
				this.cachedRgn = IntPtr.Zero;
			}
			if( m.Msg ==	Syncfusion.Runtime.InteropServices.NativeMethods.WM_NCPAINT )
			{
				if( this.FlatStyle != ComboFlatStyle.System )
					this.cachedRgn = DrawingUtils.NCPaintHelper( this, this, ref m );
			}
			//			if(m.Msg == NativeMethods.TTM_SETTOOLINFO
			//				|| m.Msg == NativeMethods.TTM_ADDTOOL)
			//			{
			//				NativeMethods.TOOLINFO ti = (NativeMethods.TOOLINFO)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.TOOLINFO));
			//				NativeMethods.TOOLINFO newTi = new NativeMethods.TOOLINFO();
			//				newTi.hinst = ti.hinst;
			//				newTi.hwnd = ti.hwnd;
			//				newTi.lParam = ti.lParam;
			//				newTi.lpszText = ti.lpszText;
			//				newTi.rect = ti.rect;
			//				// Assuming that the incoming uFlags has TTF_IDISHWND set.
			//				newTi.uId = this.TextBox.Handle;
			//				newTi.uFlags = ti.uFlags;
			//				NativeMethods.SendMessage(this.TextBox.Handle, m.Msg,
			//					0, newTi);
			//			}
			base.WndProc( ref m );
			// When the drop-down closes, SetFocus is called on this control which is already active, so
			// we will have to transfer focus to the text box at that point.
			if( m.Msg == NativeMethods.WM_SETFOCUS )
			{

				if( textBox.Visible && !textBox.Focused )
				{
					this.textBox.Focus();
				}
			}
		}

		/// <summary>
		/// Raises the ThemeChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnThemeChanged method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnThemeChanged in a derived
		/// class, be sure to call the base class's OnThemeChanged method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnThemeChanged( EventArgs e )
		{
			if( this.ThemeChanged != null )
			{
				try
				{
					this.ThemeChanged( this, e );
				}
				catch { }
			}
		}
		private bool ThemesEnabled
		{
			get { return ( (IThemedControl)this ).ThemesEnabled; }
		}
		/// <summary>
		/// Indicates whether themes are enabled for this control.
		/// </summary>
		bool IThemedControl.ThemesEnabled
		{
			get { return this.FlatStyle == ComboFlatStyle.System; }
			set
			{
				if( this.ThemesEnabled != value )
				{
					if( value && !(SkinManager.ContainsSkinManager))
						this.FlatStyle = ComboFlatStyle.System;
					else
						this.FlatStyle = ComboFlatStyle.Standard;
					this.OnThemeChanged( EventArgs.Empty );
				}
			}
		}
		#endregion THEMES

        #region For Touch

        bool isScaling = false;
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
        bool _touchMode = false;
        /// <summary>
        /// Gets or sets value to enable or disable the Touchmode to the controls.
        /// </summary>
        /// <remarks>Scale factor will be updated automatically if scalefactor is equal to 1</remarks>
        [Browsable(true), DefaultValue(false),
        Category("Layout"), Description("Gets or sets value to enable or disable the Touchmode to the controls."),
        ]
        public bool EnableTouchMode
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
                    ddButton._touchmode = value;
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
        /// Scale the control based on the scale factor passed in the argument.
        /// </summary>
        /// <param name="scaleFactor">value to scale the factor based upon.</param>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            this.PreventHeightChange = false;
            if (FONTSTYLE == USERFONTSTYLE)
                this.Font = new Font(FONTSTYLE.FontFamily, FONTSTYLE.Size * scaleFactor, this.Font.Style, this.Font.Unit, this.Font.GdiCharSet, this.Font.GdiVerticalFont);
            else
                this.Font = new Font(USERFONTSTYLE.FontFamily, FONTSTYLE.Size * scaleFactor, this.Font.Style, this.Font.Unit, this.Font.GdiCharSet, this.Font.GdiVerticalFont);
            this.DropDownButtonHeight = (int)(dropDownButtonHeight * scaleFactor);
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            isScaling = false;
            this.ResumeLayout();
            this.Refresh();
        }
        /// <summary>
        ///Font chnaged
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (!isScaling)
            {
                if (USERFONTSTYLE != this.Font)
                    USERFONTSTYLE = this.Font;
            }
        }
        /// <summary></summary>
        /// <param name="e"/>
        protected override void OnSizeChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnSizeChanged(e);
            if (!EnableTouchMode && this.DesignMode)
            {
                CTRLSIZE = this.Size;
            }
        }
        #endregion


		internal class ComboDropDownPopupContainer: PopupControlContainer
		{
			public override bool IsRelatedControl( Control control, bool askPopupParent )
			{
				if( !this.IsShowing() )
				{
					return false;
				}

				if( control == this || this.Contains( control ) || control == this.Parent 
                    || ( ( control is VScrollBarCustomDraw || control is HScrollBarCustomDraw || control is ScrollersFrame.SizeGripperAdv ) 
                    && this.Bounds.Contains( control.Bounds ) ) )
				{
					return true;
				}
				else if( askPopupParent )
				{
					if( this.PopupParent != null )
					{
						return this.PopupParent.IsRelatedControl( control, askPopupParent );
					}
					else if( control != null && this.ParentControl != null &&
					         ( control == this.ParentControl || this.ParentControl.Contains( control ) ) )
					{
						return true;
					}
				}

				return false;
			}

			protected override void OnSizeChanged(EventArgs e)
			{
				base.OnSizeChanged(e);

				Control parent = this.Parent;
				
				if (parent != null)
				{
					parent.Size = parent.GetPreferredSize(Size.Empty);
				}
			}
		}

		[ToolboxItem( false ), Documentation.DocumentationExclude()]
		public class ComboTextBox: TextBox
		{
			private ComboDropDown parent;
			private bool doubleClickFired;

			public ComboTextBox( ComboDropDown parentCombo )
			{
				this.doubleClickFired = false;
				this.parent = parentCombo;
				base.SetStyle( ControlStyles.Selectable, false );
			}

			protected override /*Control*/ void OnLostFocus( EventArgs e )
			{
				this.parent.OnLostFocus( e );
				return;
			}

			protected override void OnMouseUp( MouseEventArgs e )
			{
				Point pt;

				pt = new Point( e.X, e.Y );
				pt = this.PointToScreen( pt );
				if( e.Button == MouseButtons.Left )
				{
					if( this.Focused && NativeMethods.WindowFromPoint( pt.X, pt.Y ) == base.Handle )
					{
						if( !( this.doubleClickFired ) )
							this.parent.OnClick( EventArgs.Empty );
						else
						{
							this.doubleClickFired = false;
							this.parent.OnDoubleClick( EventArgs.Empty );
						}
					}
					this.doubleClickFired = false;
				}
				this.parent.OnMouseUp( e );
			}


			protected override void OnMouseDown( MouseEventArgs e )
			{
				if( e.Clicks == 2 )
					this.doubleClickFired = true;

				this.parent.OnMouseDown( e );
			}

			protected override /*Control*/ void OnKeyUp( KeyEventArgs e )
			{
				this.parent.TextBox_KeyUp( this, e );
				return;
			}

			protected override /*TextBox*/ void OnGotFocus( EventArgs e )
			{
				// Select all as soon as got focus and lose the mouse down, if that is what caused it to get focus.
				this.SelectAll();
				this.parent.OnGotFocus( e );
				return;
			}

			[SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode=true )]
			protected override void WndProc( ref Message m )
			{

				if( m.Msg == 0x0201 /*WM_LBUTTONDOWN*/
					|| m.Msg == 0x0204 /*WM_RBUTTONDOWN*/
					|| m.Msg == 0x0207 /*WM_MBUTTONDOWN*/)
				{


					bool focused = this.Focused;



					// This will activate all the parents before making itself active.
					// (Cannot use this any more ever since we made the ComboDropDown derive from control instead of ContainerControl.
					//if(!WinFormsUtils.ActivateAllParents(this.parent))
					//	return;
					// Lose the first left mouse down if that is what caused it to get focus.
					if( m.Msg == 0x0201 && focused != this.Focused )
						return;
				}
				base.WndProc( ref m );
			}
		}

		/// <summary>
		/// Fired when an IMe message is received.
		/// </summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event data.</param>
		private void m_nativeWindow_ImeMessageReceived( object sender, EventArgs e )
		{
			m_bIsInImeMode = true;
		}

		#region ISupportOffice2007Theme implementation

		void ISupportOffice2007Theme.EnableOffice2007Style()
		{
			this.Style = VisualStyle.Office2007;
		}

		#endregion
	}
	/// <summary>
	/// Class that represents the drop-down portion of a <see cref="ComboBoxBase"/>.
	/// </summary>
	public class DropDownButton:
		IDisposable
	{
		#region Constants
		private readonly float[] c_backgroundColorsPositionsOffice2007 = new float[] { 0f, 0.49f, 0.51f, 1f };
        private readonly float[] c_backgroundColorsPositionsMetro = new float[] { 0f, 0.09f, 0.01f, 0f };
		#endregion

		private ThemedDropDownButtonDrawing themedDrawing = null;
		protected Control control;
		private Rectangle bounds;
		private bool hot = false;
		private bool enabled = true;
		private bool pushed = false;
		private ComboFlatStyle flatStyle = ComboFlatStyle.Standard;
		private Office2007Theme m_office2007ColorTheme = Office2007Theme.Blue;
        private Office2010Theme m_office2010ColorTheme = Office2010Theme.Blue;
        private MetroTheme m_metroColorTheme = MetroTheme.Managed;
		private VisualStyle editorStyle = VisualStyle.Default;
		private IThemedControl themedControl = null;
		private bool isActive = false;
		private bool isDroppedDown = false;
		private int _suspendInvalidates = 0;
        private Color metroColor = ColorTranslator.FromHtml("#119EDA");
		/// <summary>
		/// Creates an instance of the DropDownButton class.
		/// </summary>
		/// <param name="control">The control that is using this class to draw the drop-down portion.</param>
		public DropDownButton( Control control )
		{
			Trace.Assert( control != null, "DropDownButton constructor cannot take a NULL parameter." );

			this.control = control;

			if( this.control is IThemedControl )
				this.themedControl = this.control as IThemedControl;

			control.HandleCreated += new EventHandler( control_HandleCreated );
			control.HandleDestroyed += new EventHandler( control_HandleDestroyed );

			if( control.IsHandleCreated )
			{
				CreateThemedDrawing();
			}
		}

		[Category( "Appearance" ),
		Description( "Office 2007 color scheme." ),
		DefaultValue( Office2007Theme.Blue )]
		public Office2007Theme Office2007ColorTheme
		{
			get
			{
				return m_office2007ColorTheme;
			}

			set
			{
				if( m_office2007ColorTheme != value )
				{
					m_office2007ColorTheme = value;
					this.OnStyleChanged();
				}
			}
		}
		/// <summary>
		///Gets the touchmode value.
		/// </summary>
        public bool _touchmode = false;
        [Category("Appearance"),
        Description("Office 2010 color scheme."),
        DefaultValue(Office2010Theme.Blue)]
        public Office2010Theme Office2010ColorTheme
        {
            get
            {
                return m_office2010ColorTheme;
            }

            set
            {
                if (m_office2010ColorTheme != value)
                {
                    m_office2010ColorTheme = value;
                    this.OnStyleChanged();
                }
            }
        }
        [Category("Appearance"),
       Description("Metro color scheme."),
       DefaultValue(MetroTheme.Blue)]
        public MetroTheme MetroColorTheme
        {
            get
            {
                return m_metroColorTheme;
            }

            set
            {
                if (m_metroColorTheme != value)
                {
                    m_metroColorTheme = value;
                    this.OnStyleChanged();
                }
            }
        }
		/// <summary>
		/// Fired when the user performs a mouse down in the drop-down area.
		/// </summary>
		public event EventHandler MouseDown;


		/// <summary>
		/// Raises the MouseDown event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks><para>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event.</para> <para>The MouseDown method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnMouseDown 
		/// in a derived class, be sure to call the base class's 
		/// OnMouseDown method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnMouseDown( EventArgs e )
		{
			if( this.MouseDown != null )
			{
				try
				{
					this.MouseDown( this, e );
				}
				catch { }
			}
		}

		#region DRAWING
		/// <summary>
		/// Indicates whether themes is currently enabled for this control.
		/// </summary>
		/// <remarks>This property doesn't check whether themes are currently enabled in the OS.</remarks>
		protected bool ThemesEnabled
		{
			get
			{
				if( this.themedControl == null )
					return false;
				else
					return this.themedControl.ThemesEnabled && this.FlatStyle == ComboFlatStyle.System;
			}
		}
		/// <summary>
		/// Invalidates the specified portion in the underlying control.
		/// </summary>
		/// <param name="bounds">The area to invalidate.</param>
		protected void InvalidateBounds( Rectangle bounds )
		{
			if( !this.SuspendInvalidates && this.control.IsHandleCreated )
			{
				if( Environment.OSVersion.Platform == PlatformID.Win32NT
					|| this.control.ContainsFocus )
					this.control.Invalidate( bounds );
			}
		}


		private void DrawHotBackgroundOffice2007Style( Graphics g, Rectangle bounds )
		{
			Rectangle rect = bounds;

			using( LinearGradientBrush br = new LinearGradientBrush( rect, Color.Empty, Color.Empty, LinearGradientMode.Vertical ) )
			{
				ColorBlend colorBlend = new ColorBlend();
				colorBlend.Colors = new Color[]
				{
					this.Office2007ColorTable.ComboBoxAdvHotBackgroundButtonColor1,
					this.Office2007ColorTable.ComboBoxAdvHotBackgroundButtonColor2,
					this.Office2007ColorTable.ComboBoxAdvHotBackgroundButtonColor3,
					this.Office2007ColorTable.ComboBoxAdvHotBackgroundButtonColor4
				};
				colorBlend.Positions = c_backgroundColorsPositionsOffice2007;

				br.InterpolationColors = colorBlend;

				g.FillRectangle( br, rect );
			}
		}
        private void DrawHotBackgroundOffice2010Style(Graphics g, Rectangle bounds)
        {
            Rectangle rect = bounds;

            using (LinearGradientBrush br = new LinearGradientBrush(rect, Color.Empty, Color.Empty, LinearGradientMode.Vertical))
            {
                ColorBlend colorBlend = new ColorBlend();
                colorBlend.Colors = new Color[]
				{
					this.Office2010ColorTable.ComboBoxAdvHotBackgroundButtonColor1,
					this.Office2010ColorTable.ComboBoxAdvHotBackgroundButtonColor2,
					this.Office2010ColorTable.ComboBoxAdvHotBackgroundButtonColor3,
					this.Office2010ColorTable.ComboBoxAdvHotBackgroundButtonColor4
				};
                colorBlend.Positions = c_backgroundColorsPositionsOffice2007;

                br.InterpolationColors = colorBlend;

                g.FillRectangle(br, rect);
            }
        }

		private void DrawOffice2007Style( Graphics g, ButtonState btState )
		{
			if( this.IsDroppedDown )
			{
				btState = ButtonState.Pushed;
			}
			else if( this.IsControlActive )
			{
				btState = ButtonState.Checked;
			}

			bool bRTL = this.control.RightToLeft == RightToLeft.Yes;
			Rectangle rect = this.Bounds;
			int x = rect.Left;

			if( bRTL )
			{
				x = rect.Right - 1;
			}
			else
			{
				rect.X++;
			}

			if( btState == ButtonState.Normal && this.IsActive )
			{
				using( Pen pen = new Pen( this.Office2007ColorTable.ComboBoxAdvNormalBorderColor ) )
				{
					g.DrawLine( pen, x, rect.Y, x, rect.Bottom - 1 );
				}

				rect.Width--;

				using( LinearGradientBrush br = new LinearGradientBrush( rect, Color.Empty, Color.Empty, LinearGradientMode.Vertical ) )
				{
					ColorBlend colorBlend = new ColorBlend();
					colorBlend.Colors = new Color[]
					{
						this.Office2007ColorTable.ComboBoxAdvNormalBackgroundButtonColor1,
						this.Office2007ColorTable.ComboBoxAdvNormalBackgroundButtonColor2,
						this.Office2007ColorTable.ComboBoxAdvNormalBackgroundButtonColor3,
						this.Office2007ColorTable.ComboBoxAdvNormalBackgroundButtonColor4
					};
					colorBlend.Positions = c_backgroundColorsPositionsOffice2007;

					br.InterpolationColors = colorBlend;

					g.FillRectangle( br, rect );
				}
			}
			else if( btState == ButtonState.Checked )
			{
				g.FillRectangle( Brushes.White, rect );

				using( Pen pen = new Pen( this.Office2007ColorTable.ComboBoxAdvHotBorderColor ) )
				{
					g.DrawLine( pen, x, rect.Y, x, rect.Bottom );
				}

				rect.Width--;
				rect.Inflate( -1, -1 );

				this.DrawHotBackgroundOffice2007Style( g, rect );
			}
			else if( btState == ButtonState.Pushed )
			{
				using( Pen pen = new Pen( this.Office2007ColorTable.ComboBoxAdvPushedBorderColor ) )
				{
					g.DrawLine( pen, x, rect.Y, x, rect.Bottom - 1 );
				}

				rect.Width--;

				this.DrawHotBackgroundOffice2007Style( g, rect );

				using( Pen pen = new Pen( this.Office2007ColorTable.ComboBoxAdvButtonUpperLineColor ) )
				{
					g.DrawLine( pen, rect.Left, rect.Y, rect.Right - 1, rect.Y );
				}

				rect.Inflate( -1, -1 );

				using( LinearGradientBrush br = new LinearGradientBrush( rect, Color.Empty, Color.Empty, LinearGradientMode.Vertical ) )
				{
					ColorBlend colorBlend = new ColorBlend();
					colorBlend.Colors = new Color[]
					{
						this.Office2007ColorTable.ComboBoxAdvPushedBackgroundButtonColor1, 
						this.Office2007ColorTable.ComboBoxAdvPushedBackgroundButtonColor2,
						this.Office2007ColorTable.ComboBoxAdvPushedBackgroundButtonColor3,
						this.Office2007ColorTable.ComboBoxAdvPushedBackgroundButtonColor4
					};
					colorBlend.Positions = c_backgroundColorsPositionsOffice2007;

					br.InterpolationColors = colorBlend;

					g.FillRectangle( br, rect );
				}
			}

			// Darw arrow
            Point[] dropDownArrowBounds;
            if (!_touchmode)
            {
                dropDownArrowBounds = DropDownButton.GetComboDropDownBorderBounds(this.Bounds);
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddLines(dropDownArrowBounds);
                    if (this.IsActive)
                    {
                        GraphicsState stateForActive = g.Save();
                        g.TranslateTransform(0, 1);
                        using (Brush brush = new SolidBrush(this.Office2007ColorTable.ComboBoxAdvLowerArrowLineColor))
                        {
                            g.FillPath(brush, path);
                        }
                        g.Restore(stateForActive);
                    }
                    using (Brush brush = new SolidBrush(this.Office2007ColorTable.ComboBoxAdvArrowColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
            else
            {
                dropDownArrowBounds = new Point[] { 
											   new Point(this.Bounds.Left +3, this.Bounds.Top +8),
											   new Point(this.Bounds.Right -2, this.Bounds.Top +8),
											   new Point(this.Bounds.Left+8, this.Bounds.Top +16),
											   new Point(this.Bounds.Left +3, this.Bounds.Top+8) };
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddLines(dropDownArrowBounds);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    if (this.IsActive)
                    {
                        GraphicsState stateForActive = g.Save();

                        g.TranslateTransform(0, 1);

                        using (Brush brush = new SolidBrush(this.Office2007ColorTable.ComboBoxAdvLowerArrowLineColor))
                        {
                            g.FillPath(brush, path);
                        }

                        g.Restore(stateForActive);
                    }

                    using (Brush brush = new SolidBrush(this.Office2007ColorTable.ComboBoxAdvArrowColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }


		}
        private void DrawOffice2010Style(Graphics g, ButtonState btState)
        {
            if (this.IsDroppedDown)
            {
                btState = ButtonState.Pushed;
            }
            else if (this.IsControlActive)
            {
                btState = ButtonState.Checked;
            }

            bool bRTL = this.control.RightToLeft == RightToLeft.Yes;
            Rectangle rect = this.Bounds;
            int x = rect.Left;

            if (bRTL)
            {
                x = rect.Right - 1;
            }
            else
            {
                rect.X++;
            }

            if (btState == ButtonState.Normal && this.IsActive)
            {
                using (Pen pen = new Pen(this.Office2010ColorTable.ComboBoxAdvNormalBorderColor))
                {
                    g.DrawLine(pen, x, rect.Y, x, rect.Bottom - 1);
                }

                rect.Width--;

                using (LinearGradientBrush br = new LinearGradientBrush(rect, Color.Empty, Color.Empty, LinearGradientMode.Vertical))
                {
                    ColorBlend colorBlend = new ColorBlend();
                    colorBlend.Colors = new Color[]
					{
						this.Office2010ColorTable.ComboBoxAdvNormalBackgroundButtonColor1,
						this.Office2010ColorTable.ComboBoxAdvNormalBackgroundButtonColor2,
						this.Office2010ColorTable.ComboBoxAdvNormalBackgroundButtonColor3,
						this.Office2010ColorTable.ComboBoxAdvNormalBackgroundButtonColor4
					};
                    colorBlend.Positions = c_backgroundColorsPositionsOffice2007;

                    br.InterpolationColors = colorBlend;

                    g.FillRectangle(br, rect);
                }
            }
            else if (btState == ButtonState.Checked)
            {
                g.FillRectangle(Brushes.White, rect);

                using (Pen pen = new Pen(this.Office2010ColorTable.ComboBoxAdvHotBorderColor))
                {
                    g.DrawLine(pen, x, rect.Y, x, rect.Bottom);
                }

                rect.Width--;
                rect.Inflate(-1, -1);

                this.DrawHotBackgroundOffice2010Style(g, rect);
            }
            else if (btState == ButtonState.Pushed)
            {
                using (Pen pen = new Pen(this.Office2010ColorTable.ComboBoxAdvPushedBorderColor))
                {
                    g.DrawLine(pen, x, rect.Y, x, rect.Bottom - 1);
                }

                rect.Width--;

                this.DrawHotBackgroundOffice2010Style(g, rect);

                using (Pen pen = new Pen(this.Office2010ColorTable.ComboBoxAdvButtonUpperLineColor))
                {
                    g.DrawLine(pen, rect.Left, rect.Y, rect.Right - 1, rect.Y);
                }

                rect.Inflate(-1, -1);

                using (LinearGradientBrush br = new LinearGradientBrush(rect, Color.Empty, Color.Empty, LinearGradientMode.Vertical))
                {
                    ColorBlend colorBlend = new ColorBlend();
                    colorBlend.Colors = new Color[]
					{
						this.Office2010ColorTable.ComboBoxAdvPushedBackgroundButtonColor1, 
						this.Office2010ColorTable.ComboBoxAdvPushedBackgroundButtonColor2,
						this.Office2010ColorTable.ComboBoxAdvPushedBackgroundButtonColor3,
						this.Office2010ColorTable.ComboBoxAdvPushedBackgroundButtonColor4
					};
                    colorBlend.Positions = c_backgroundColorsPositionsOffice2007;

                    br.InterpolationColors = colorBlend;

                    g.FillRectangle(br, rect);
                }
            }

            // Darw arrow
            Point[] dropDownArrowBounds;
            if (!_touchmode)
            {
                dropDownArrowBounds = DropDownButton.GetComboDropDownBorderBounds(this.Bounds);
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddLines(dropDownArrowBounds);
                    if (this.IsActive)
                    {
                        GraphicsState stateForActive = g.Save();
                        g.TranslateTransform(0, 1);
                        using (Brush brush = new SolidBrush(this.Office2010ColorTable.ComboBoxAdvLowerArrowLineColor))
                        {
                            g.FillPath(brush, path);
                        }
                        g.Restore(stateForActive);
                    }
                    using (Brush brush = new SolidBrush(this.Office2010ColorTable.ComboBoxAdvArrowColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
            else
            {
                dropDownArrowBounds = new Point[] { 
											   new Point(this.Bounds.Left +3, this.Bounds.Top +8),
											   new Point(this.Bounds.Right -2, this.Bounds.Top +8),
											   new Point(this.Bounds.Left+8, this.Bounds.Top +16),
											   new Point(this.Bounds.Left +3, this.Bounds.Top+8) };
                using (GraphicsPath path = new GraphicsPath())

                {
                    path.AddLines(dropDownArrowBounds);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    if (this.IsActive)
                    {
                        GraphicsState stateForActive = g.Save();

                        g.TranslateTransform(0, 1);

                        using (Brush brush = new SolidBrush(this.Office2010ColorTable.ComboBoxAdvLowerArrowLineColor))
                        {
                            g.FillPath(brush, path);
                        }
                        g.Restore(stateForActive);
                    }
                    using (Brush brush = new SolidBrush(this.Office2010ColorTable.ComboBoxAdvArrowColor))
                    {
                        g.FillPath(brush, path);
                    }
                }

            }
        }
        private void DrawMetroStyle(Graphics g, ButtonState btState)
        {


            Color bgColor = (this.control as ComboDropDown).MetroColor;
            Color color = Color.Gray;

            bool bRTL = this.control.RightToLeft == RightToLeft.Yes;
            Rectangle rect = this.Bounds;
            int x = rect.Left;

            if (bRTL)
            {
                x = rect.Right - 1;
            }
            else
            {
                rect.X++;
            }

            if (btState == ButtonState.Checked)
            {
                bgColor = ControlPaint.LightLight(Color.LightGray);
               color = Color.Gray;
            }
            else if (btState == ButtonState.Pushed)
            {
                bgColor = ControlPaint.Light(bgColor);
                color = Color.White;
              
            }
            else
            {
                if (!this.IsDroppedDown)
                {
                    bgColor = Color.White;
                }
                else
                {
                    color = Color.White;
                }
               
            }

            Pen pen = new Pen(bgColor == Color.White ? Color.LightGray :Color.LightGray);

          
                    g.DrawLine(pen, x, rect.Y, x, rect.Bottom - 1);
                    pen.Dispose();
                    SolidBrush Solidbrush = new SolidBrush(bgColor);
                    g.FillRectangle(Solidbrush, rect);
                    Solidbrush.Dispose();

            // Darw arrow
            Point[] dropDownArrowBounds;
            if (!_touchmode)
            {
                dropDownArrowBounds = DropDownButton.GetComboDropDownBorderBounds(this.Bounds);
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddLines(dropDownArrowBounds);
                    SolidBrush brush = new SolidBrush(color);
                    g.FillPath(brush, path);
                    brush.Dispose();
                }
            }
            else
            {
                dropDownArrowBounds = new Point[] { 
											   new Point(this.Bounds.Left +2, this.Bounds.Top +8),
											   new Point(this.Bounds.Right -3, this.Bounds.Top +8),
											   new Point(this.Bounds.Left+7, this.Bounds.Top +16),
											   new Point(this.Bounds.Left +2, this.Bounds.Top+8) };
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddLines(dropDownArrowBounds);
                    SolidBrush brush = new SolidBrush(color);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.FillPath(brush, path);
                    brush.Dispose ();
                }
            }

        }
		/// <summary>
		/// Indicates whether owner control is active.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is control active; otherwise, <c>false</c>.
		/// </value>
		protected virtual bool IsControlActive
		{
			get
			{
				return this.control.Focused 
					|| ( this.IsActive && control is ComboDropDown && ( (ComboDropDown)control ).DropDownStyle == ComboBoxStyle.DropDownList );
			}
		}

		/// <summary>
		/// Call this method from the control's OnPaint method.
		/// </summary>
		/// <param name="g">The Graphics context using which to draw the button.</param>
		public virtual void OnPaint( Graphics g )
		{
			ButtonState btnState = ButtonState.Normal;

			if( this.Bounds.Width <= 0 || this.Bounds.Height <= 0 )
				return;

			if( !this.Enabled )
				btnState = ButtonState.Inactive;
			else if( this.Pushed )
				btnState = ButtonState.Pushed;
			else if( this.Hot )
				btnState = ButtonState.Checked;
			else
				btnState = ButtonState.Normal;

			if( this.Style == VisualStyle.Office2007 )
			{
				DrawOffice2007Style( g, btnState );
			}
            else if (this.Style == VisualStyle.Office2010)
            {
                DrawOffice2010Style(g, btnState);
            }
            else if (this.Style == VisualStyle.Metro)
            {
                DrawMetroStyle(g, btnState);
            }
			else if( this.Style != VisualStyle.Default )
			{
				Color btnColor = Color.Empty;
				if( this.IsDroppedDown )
				{
					if( this.Style == VisualStyle.OfficeXP )
						btnColor = MenuColors.PressedSelColor;
					else
						btnColor = Office2003Colors.PressedSelColor;
				}
				else if( this.IsActive )
				{
					if( this.Style == VisualStyle.OfficeXP )
						btnColor = MenuColors.SelColor;
					else
						btnColor = Office2003Colors.SelColor;
				}
				else
					btnColor = this.control.Parent.BackColor;

				using( Brush brush = new SolidBrush( btnColor ) )
				{
					g.FillRectangle( brush, this.Bounds );
				}

                Color arrowColor = SystemColors.ControlText;
				if( this.IsDroppedDown )
					arrowColor = SystemColors.Window;
				if( !this.Enabled )
					arrowColor = SystemColors.GrayText;
                // The arrow:
                Point[] dropDownArrowBounds;
                if (!_touchmode)
                {
                    dropDownArrowBounds = DropDownButton.GetComboDropDownBorderBounds(this.Bounds);
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.AddLines(dropDownArrowBounds);
                        using (Brush brush = new SolidBrush(arrowColor))
                        {
                            using (Region region = new Region(path))
                                g.FillRegion(brush, region);
                        }
                    }
                }
                else
                {
                    dropDownArrowBounds = new Point[] { 
											   new Point(this.Bounds.Left +2, this.Bounds.Top +9),
											   new Point(this.Bounds.Right -2, this.Bounds.Top +9),
											   new Point(this.Bounds.Left+7, this.Bounds.Top +15),
											   new Point(this.Bounds.Left +2, this.Bounds.Top+9) };
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.AddLines(dropDownArrowBounds);
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        using (Brush brush = new SolidBrush(arrowColor))
                        {
                            using (Region region = new Region(path))
                                g.FillRegion(brush, region);
                        }
                    }
                }



			}
			else if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled )
			{
				// Adjust the bounds a bit so that it looks the same as the .NET combo.
				Rectangle bounds = this.Bounds;
				bounds.Inflate( 0, 1 );
				bounds.X++;
				this.themedDrawing.DrawDropDownButton( g, bounds, btnState );
			}
			else
			{
				if( btnState == ButtonState.Pushed )
					btnState = ButtonState.Pushed | ButtonState.Flat;

				// If Flat
				if( this.FlatStyle == ComboFlatStyle.Flat )
				{
					if( btnState == ButtonState.Normal )
						btnState = ButtonState.Flat;
					else if( btnState == ButtonState.Checked )
						btnState = ButtonState.Normal;
					else if( btnState == ButtonState.Inactive )
						btnState |= ButtonState.Flat;
				}
				// Standard 
				else
				{
					if( btnState == ButtonState.Checked )
						btnState = ButtonState.Normal;
				}

				if( !isDroppedDown )
					btnState = ButtonState.Normal;

				ControlPaint.DrawComboButton( g, this.Bounds, btnState );
			}
		}
		public static float GetComboDropDownArrowWidth()
		{
			//float ddwidth = 7f * 
			//	(SystemInformation.MenuCheckSize.Width / 13f/*13 is the standard size of the checkboxes.*/);
			// Need to determine the exact transformation logic for higher font sizes.
			float ddwidth = 7f;

			// ddwidth cannot be an even number.
			int iddwidth = (int)ddwidth;
			if( ( iddwidth % 2 ) == 0 )
				ddwidth++;

			return ddwidth;
		}
		public static float GetComboDropDownArrowHeight()
		{
			return ( (int)DropDownButton.GetComboDropDownArrowWidth() )/2 + 1;
		}
		public static Point[] GetComboDropDownBorderBounds( Rectangle btnBounds )
		{
			int arWidth = (int)DropDownButton.GetComboDropDownArrowWidth();
			int arHeight = (int)DropDownButton.GetComboDropDownArrowHeight();

			return DropDownButton.GetComboDropDownBorderBounds( btnBounds, arWidth, arHeight );
		}
		public static Point[] GetComboDropDownBorderBounds( Rectangle btnBounds,
			int arWidth, int arHeight )
		{
			int left = btnBounds.Left + ( btnBounds.Width-arWidth )/2;
			int top = btnBounds.Top + ( btnBounds.Height - arHeight )/2;

			Rectangle rcddbtn = new Rectangle( left, top, arWidth, arHeight );

			Point[] ptsscrll = new Point[] { 
											   new Point(rcddbtn.Left, rcddbtn.Top),
											   new Point(rcddbtn.Right, rcddbtn.Top),
											   new Point(left + arWidth/2, rcddbtn.Bottom),
											   new Point(rcddbtn.Left, rcddbtn.Top) };

			return ptsscrll;
		}
		#endregion DRAWING

		#region MOUSE_PROCESSING
		/// <summary>
		/// Cancels any mouse tracking.
		/// </summary>
		public void CancelMouseTrack()
		{
			this.Pushed = false;
		}
		/// <summary>
		/// Call this method from the control's OnMouseMove to inform this class of mouse move events.
		/// </summary>
		/// <param name="e">The MouseEventArsg in the OnMouseMove method.</param>
		/// <param name="useFullControlBounds">Indicates whether the full control bounds should be considered 
		/// part of the button. Useful in a combo in list mode.</param>
		public void OnMouseMove( MouseEventArgs e, bool useFullControlBounds )
		{
			if( this.Bounds.Contains( e.X, e.Y )
				|| useFullControlBounds )
			{
				this.Hot = true;
				if( Control.MouseButtons == MouseButtons.Left && this.Enabled)
					this.Pushed = true;
			}
			else
			{
				this.Hot = false;
				this.Pushed = false;
			}
		}
		/// <summary>
		/// Call this method from the control's OnMouseLeave to inform this class of mouse leave events.
		/// </summary>
		/// <param name="e">The EventArsg in the OnMouseLeave method.</param>
		public void OnMouseLeave( EventArgs e )
		{
			this.Hot = false;
			this.Pushed = false;
		}
		/// <summary>
		/// Call this method from the control's OnMouseDown to inform this class of mouse down events.
		/// </summary>
		/// <param name="e">The MouseEventArsg in the OnMouseDown method.</param>
		/// <param name="useFullControlBounds">Indicates whether the full control bounds should be considered 
		/// part of the button. Useful in a combo in list mode.</param>
		public void OnMouseDown( MouseEventArgs e, bool useFullControlBounds )
		{
			if( this.Enabled && e.Button == MouseButtons.Left )
			{
				if( this.Bounds.Contains( e.X, e.Y ) || useFullControlBounds )
					this.Pushed = true;
				else
					this.Pushed = false;
			}
		}
		/// <summary>
		/// Call this method from the control's OnMouseUp to inform this class of mouse up events.
		/// </summary>
		/// <param name="e">The MouseEventArsg in the OnMouseUp method.</param>
		public void OnMouseUp( MouseEventArgs e )
		{
			if( this.Enabled )
				this.Pushed = false;
		}
		#endregion MOUSE_PROCESSING

		#region PROPERTIES
		/// <summary>
		/// Set the bounds for the dropdown button. You typically should do this from the control's Layout event / method.
		/// </summary>
		public Rectangle Bounds
		{
			get { return this.bounds; }
			set
			{
				if( this.bounds != value )
				{
					Rectangle oldBounds = this.bounds;
					this.bounds = value;

					this.InvalidateBounds( Rectangle.Union( oldBounds, this.bounds ) );
				}
			}
		}
		/// <summary>
		/// Indicates whether the button is active. Will be referred to when drawn hot with office style.
		/// </summary>
		public bool IsActive
		{
			get { return this.isActive; }
			set
			{
				if( this.isActive != value )
				{
					this.isActive = value;
					this.InvalidateBounds( this.Bounds );
				}
			}
		}
		/// <summary>
		/// Indicates whether the drop-down is showing. Will be referred to when drawn hot with office style.
		/// </summary>
		public bool IsDroppedDown
		{
			get { return this.isDroppedDown; }
			set
			{
				if( this.isDroppedDown != value )
				{
					this.isDroppedDown = value;
                    if (this.Style == VisualStyle.Office2007 || this.Style == VisualStyle.Office2010 || this.Style == VisualStyle.Metro)
					{
						this.InvalidateParentNCArea();
					}

					this.InvalidateBounds( this.Bounds );
				}
			}
		}
		/// <summary>
		/// Indicates whether the button should be drawn hot.
		/// </summary>
		public bool Hot
		{
			get { return this.hot; }
			set
			{
				if( this.hot != value )
				{
					this.hot = value;

                    if (this.Style == VisualStyle.Office2007 || this.Style == VisualStyle.Office2010)
					{
						this.InvalidateParentNCArea();
					}

					if( Environment.OSVersion.Platform == PlatformID.Win32NT )
					{
						// Invalidate if not Standard or if System and themes disabled.
						if( this.FlatStyle == ComboFlatStyle.Flat
							|| ( this.FlatStyle == ComboFlatStyle.System && XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled ) )
							this.InvalidateBounds( this.Bounds );
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets the ComboFlatStyle with which to draw.
		/// </summary>
		public ComboFlatStyle FlatStyle
		{
			get { return this.flatStyle; }
			set
			{
				if( this.flatStyle != value )
				{
					this.flatStyle = value;
					this.InvalidateBounds( this.Bounds );
				}
			}
		}
		/// <summary>
		/// Gets or sets the VisualStyle with which to draw.
		/// </summary>
		public VisualStyle Style
		{
			get { return this.editorStyle; }
			set
			{
				if( this.editorStyle != value )
				{
					this.editorStyle = value;
					this.OnStyleChanged();
				}
			}
		}

		private void OnStyleChanged()
		{
			this.InvalidateBounds( this.Bounds );
		}

		/// <summary>
		/// Gets color table for Office2007 visual style.
		/// </summary>
		private Office2007Colors Office2007ColorTable
		{
			get
			{
				Office2007Colors colorTable = Office2007Colors.GetColorTable( this.Office2007ColorTheme );

				return colorTable;
			}
		}

        /// <summary>
        /// Gets color table for Office2007 visual style.
        /// </summary>
        private Office2010Colors Office2010ColorTable
        {
            get
            {
                Office2010Colors colorTable = Office2010Colors.GetColorTable(this.Office2010ColorTheme);

                return colorTable;
            }
        }
        /// <summary>
        /// Gets color table for Metro visual style.
        /// </summary>
        private MetroColors MetroColorTable
        {
            get
            {
                MetroColors colorTable = MetroColors.GetColorTable(this.MetroColorTheme);
                return colorTable;
            }
        }
		/// <summary>
		/// Indicates whether the buttons should be drawn enabled.
		/// </summary>
		public bool Enabled
		{
			get { return this.enabled; }
			set
			{
				if( this.enabled != value )
				{
					this.enabled = value;
					this.InvalidateBounds( this.Bounds );
				}
			}
		}

		private void InvalidateParentNCArea()
		{
			if( !this.SuspendInvalidates )
			{
				NativeMethodsHelper.RedrawWindow( this.control.Handle, NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE );	
			}			
		}

		/// <summary>
		/// Indicates whether the button is currently pushed.
		/// </summary>
		public bool Pushed
		{
			get { return this.pushed; }
			set
			{
				if( this.pushed != value )
				{
					this.pushed = value;

                    if (this.Style == VisualStyle.Office2007 || this.Style == VisualStyle.Office2007|| this.Style == VisualStyle.Metro )
					{
						this.InvalidateParentNCArea();
					}
					if( Environment.OSVersion.Platform == PlatformID.Win32NT
						|| !this.IsDroppedDown )
						this.InvalidateBounds( this.Bounds );

					if( this.pushed )
					{
						this.OnMouseDown( EventArgs.Empty );
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether invalidating of owner control is suspended.
		/// </summary>
		public bool SuspendInvalidates
		{
			get
			{
				return _suspendInvalidates > 0;
			}
			set
			{
				if( value )
				{
					++_suspendInvalidates;
				}
				else if( this.SuspendInvalidates )
				{
					--_suspendInvalidates;
				}
			}
		}

		#endregion PROPERTIES

		private void CreateThemedDrawing()
		{
			if( XPThemes.IsThemedOS && null == this.themedDrawing )
			{
				this.themedDrawing = new ThemedDropDownButtonDrawing();
			}
		}

		private void control_HandleCreated( object sender, EventArgs e )
		{
			CreateThemedDrawing();
		}

		private void control_HandleDestroyed( object sender, EventArgs e )
		{
			if( null != this.themedDrawing )
			{
				this.themedDrawing.Dispose();
				this.themedDrawing = null;
			}
		}

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			if( null != this.control )
			{
				control.HandleCreated -= new EventHandler( control_HandleCreated );
				control.HandleDestroyed -= new EventHandler( control_HandleDestroyed );

				this.control = null;
			}

			if( this.themedDrawing != null )
			{
				this.themedDrawing.Dispose();
				this.themedDrawing = null;
			}
		}

		#endregion
	}
	internal class ComboBoxBaseDesigner: ParentControlDesigner
	{
		protected override bool DrawGrid
		{
			get { return false; }
			set { base.DrawGrid = value; }
		}
	}
     /// <summary>
    /// ComboDropDown Designer
    /// </summary>
    public class ComboDropDownDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        /// <summary>
        /// Designer ActionList collection
        /// </summary>
        private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        /// <summary>
        ///  Initializes a new instance of the CheckBoxAdvDesigner class
        /// </summary>
        public ComboDropDownDesigner()
            : base()
        {
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

        /// <summary>
        /// Gets a value indication the designer action
        /// </summary>
        public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == this.actionLists)
                {
                    this.actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
                    this.actionLists.Add(new ComboDropDownActionList(this.Component));
                }

                return this.actionLists;
            }
        }

#endif
    }
}

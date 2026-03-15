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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;

using Syncfusion.Core.Licensing;
using Syncfusion.Windows.Forms.Design;
using Syncfusion.Runtime.InteropServices;

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using Syncfusion.Windows.Forms.Utils;
using System.Windows.Forms.Design.Behavior;
#endif
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary></summary>
	internal class TextBoxExtSubClass:
		NativeWindowSubclass
	{
		#region Class members

		private TextBoxExt m_textBoxExt = null;
		private bool m_bIsListening = true;

		#endregion

		#region Class Initialize/Finalize methods

		/// <summary>
		/// Initializes a new instance of the <see cref="TextBoxExtSubClass"/> class.
		/// </summary>
		/// <param name="textBoxExt">The instance subclasses <see cref="TextBoxExt"/>.</param>
		public TextBoxExtSubClass( TextBoxExt textBoxExt ) :
			base()
		{
			if( null == textBoxExt )
				throw new ArgumentNullException( "textBoxExt" );

			m_textBoxExt = textBoxExt;
		}

		#endregion

		#region Class Public Methods
		/// <summary></summary>
		public void StopListening()
		{
			m_bIsListening = false;
			m_textBoxExt = null;
		}
		#endregion

		#region Class overrides

		/// <summary>
		/// Invokes the default window procedure associated with this window.
		/// </summary>
		/// <param name="m">A <see cref="T:System.Windows.Forms.Message"/> that is associated with the current Windows message.</param>
		protected override void WndProc( ref Message m )
		{
			if( m_bIsListening )
			{
				if( m_textBoxExt != null &&					
					( m.Msg == NativeMethods.WM_CTLCOLOREDIT || m.Msg == NativeMethods.WM_CTLCOLORSTATIC ) 
                    /*&& m.LParam == m_textBoxExt.Handle */)
				{
					if( m_textBoxExt.ShowOverflowIndicator &&
						m_textBoxExt.GetOverflowIndicatorState() != m_textBoxExt.OverFlowIndicatorLastState )
					{
						m_textBoxExt.UpdateNCArea();
					}
				}
			}

			base.WndProc( ref m );
		}

		#endregion
	}

	/// <summary>
	/// An extended textbox that provides advanced border styles.
	/// </summary>
	/// <remarks>
	/// The <see cref="Border3DStyle"/> property provides you advanced 3D border options. The
	/// <see cref="BorderColor"/> property lets you specify custom single border colors.
	/// </remarks>
	[
	Designer( typeof( TextBoxExtDesigner ), typeof( IDesigner ) ),
	ToolboxBitmap( typeof( PopupControlContainer ), "ToolboxIcons.TextBoxExt.bmp" ),
	Description( "An extended textbox that provides advanced border styles." )
	]
	public class TextBoxExt:
		TextBox,
		ISupportInitialize,
        IVisualStyle 
	{
		#region Class constants
		/// <summary>
		/// Overflow indicator size.
		/// </summary>
		private static readonly Size c_sOverflowIndicator = new Size( 13, 13 );
		/// <summary>
		/// Overflow indicator element size.
		/// </summary>
		private static readonly Size c_overflowIndicatorElementSize = new Size( 4, 8 );
		/// <summary>
		/// Overflow indicator border indent.
		/// </summary>
		private const int c_ioverflowIndicatorBorderIndent = 2;
		/// <summary>
		/// Overflow indicator border color.
		/// </summary>
		private static readonly Color c_overflowIndicatorBorderColor = Color.Gray;
		/// <summary>
		/// Overflow indicator foreground element color.
		/// </summary>
		private static readonly Color c_overflowIndicatorForegroundElementColor = Color.DarkCyan;
		/// <summary>
		/// Overflow indicator ToolTip offset.
		/// </summary>
		private static readonly Point c_overflowIndicatorToolTipOffset = new Point( 13, 15 );
        /// <summary>
        /// Default size of the control
        /// </summary>
        private static Size CTRLSIZE = default(Size);

        /// <summary>

        ///IsScaling value
        /// </summary>
        private bool isScaling = false;
		#endregion

		#region Class members

		/// <summary></summary>
		private string m_strOVerflowToolTipText = string.Empty;
		/// <summary></summary>
		        private ThemedEditDrawing themedEditDrawing;
		/// <summary></summary>
		private Border3DSide borderSides = Border3DSide.All;
		/// <summary></summary>
		private Border3DStyle border3DStyle = Border3DStyle.Sunken;
		/// <summary></summary>
		/// <summary></summary>
		private Color borderColor = Color.Black;
		/// <summary></summary>
		private ControlDrawing cd;
		/// <summary></summary>
		private bool themesEnabled = true;
		/// <summary></summary>
		private bool drawActiveWhenDisabled = false;
		/// <summary>
		/// Parent sub class.
		/// </summary>
		private TextBoxExtSubClass m_subClass = null;
		/// <summary>
		/// overflow indicator visibility.
		/// </summary>
		private bool m_bShowOverflowIndicator = false;
		/// <summary>
		/// Last overflow indicator state.
		/// </summary>
		private OverflowIndicatorState m_bOverflowIndicatorLastState = OverflowIndicatorState.None;        
		/// <summary>
		/// Overflow indicator ToolTip.
		/// </summary>
		private ToolTipAdv m_overflowIndicatorToolTip = null;
		/// <summary>
		/// Left last painted overflow indicator rectcangle.
		/// </summary>
		private Rectangle m_leftIndicatorLastPaintedRect = Rectangle.Empty;
		/// <summary>
		/// Right last painted overflow indicator rectcangle.
		/// </summary>
		private Rectangle m_rightIndicatorLastPaintedRect = Rectangle.Empty;
		/// <summary>
		/// 
		/// </summary>
		private Rectangle m_rcClient = Rectangle.Empty;
		/// <summary>
		/// Show overflow indicator ToolTip.
		/// </summary>
		
		private bool m_bShowOverflowIndicatorToolTip = false;
		private Utils.CornerRadiusHelper m_cornerRadiusHelper;
		private Image m_imgNear;
		private Image m_imgFar;
		private int m_nCornerRadius = -1;

		#endregion

		#region Class properties
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
        [Browsable(true),DefaultValue(false),
        Category("Layout"), Description("Gets or sets value to enable or disable the Touchmode to the controls.")]
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
                    if (_touchMode)
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode!= false;
        }
        /// <summary>
        /// 
        /// </summary>
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
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }

		/// <override/>
		/// <summary></summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cparams = base.CreateParams;
				BorderStyle border = this.BorderStyle;

				if( !( border == BorderStyle.Fixed3D && this.ThemesEnabled ) )
				{
					cparams.ExStyle &= ~512;
					cparams.Style &= ~8388608;

					switch( border )
					{
						// Unlike other controls, text box doesn't draw its single border in the NC area!
						case BorderStyle.Fixed3D:
						case BorderStyle.FixedSingle:
						cparams.ExStyle = cparams.ExStyle | 512;
						break;
					}
				}

				return cparams;
			}
		}
		/// <summary>
		/// MetroColor.
		/// </summary>
        private Color metrocolor = ColorTranslator.FromHtml("#D1D3D4");
		/// <summary>
		/// Gets or sets the metrocolor.
		/// </summary>
        public Color Metrocolor
        {
            get
            {
                return metrocolor;
            }
            set
            {
                metrocolor = value;
                if (style == theme.Metro)
                {

                    this.borderColor = metrocolor;
                }
            }
        }
        protected override void OnGotFocus(EventArgs e)
        {
            if (style == theme.Metro)
            {
                this.borderColor = ColorTranslator.FromHtml("#16A5DC"); 
            }
            else if (style == theme.Office2007)
            {
                this.BorderStyle = BorderStyle.FixedSingle;
                this.BorderColor = this.Office2007ColorTable.ActiveTextBoxBorderColor;// ColorTranslator.FromHtml("#eb8900"); 
                this.BackColor = this.Office2007ColorTable.ActiveTextBoxBackColor;
            }
            else if (style == theme.Office2010)
            {
                this.BorderStyle = BorderStyle.FixedSingle;
                this.BorderColor = this.Office2010ColorTable.ActiveTextBoxBorderColor;// ColorTranslator.FromHtml("#eb8900"); 
                this.BackColor = this.Office2010ColorTable.ActiveTextBoxBackColor;
            }
            base.OnGotFocus(e);
        }

		/// <summary>
		/// Gets or sets can show overflow indicator ToolTip.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Appearance" )]
		[Description( "Gets or sets can show overflow indicator ToolTip." )]
		public bool ShowOverflowIndicatorToolTip
		{
			get
			{
				return m_bShowOverflowIndicatorToolTip;
			}
			set
			{
				m_bShowOverflowIndicatorToolTip = value;
			}
		}

		/// <summary>
		/// Gets or sets overflow indicator ToolTip text.
		/// </summary>
		[DefaultValue( "" )]
		[Category( "Appearance" )]
		[Description( "Gets or sets overflow indicator ToolTip text." )]
		public string OverflowIndicatorToolTipText
		{
			get
			{
				return m_strOVerflowToolTipText;
			}
			set
			{
				if( value != m_strOVerflowToolTipText )
				{
					m_strOVerflowToolTipText = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets overflow indicator visibility.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Appearance" )]
		[Description( "Gets or sets overflow indicator visibility." )]
		public bool ShowOverflowIndicator
		{
			get
			{
				return m_bShowOverflowIndicator;
			}
			set
			{
				if( m_bShowOverflowIndicator != value )
				{
					m_bShowOverflowIndicator = value;
					this.UpdateOverflowIndicator();
				}
			}
		}

		/// <summary>
		/// Indicates whether the text should be drawn active even when the control is disabled.
		/// </summary>
		/// <value>False for default textbox behavior; True to draw the text enabled even when the control is
		/// disabled.</value>
		[DefaultValue( false )]
		[Category( "Appearance" )]
		[Description( "Specifies if the text should be drawn active even when disabled." )]

		public bool DrawActiveWhenDisabled
		{
			get
			{
				return this.drawActiveWhenDisabled;
			}
			set
			{
				if( this.drawActiveWhenDisabled != value )
				{
					this.drawActiveWhenDisabled = value;
					this.UpdatePaintingStyle();
				}
			}
		}

		/// <summary>
		/// Gets or sets the border sides for which you want the 3D border style applied.
		/// </summary>
		/// <remarks>
		/// This property is used only when BorderStyle is Fixed3D.
		/// </remarks>
		[Description( "Indicates the border sides of the panel." )]
		[Category( "Appearance" )]
		[DefaultValue( Border3DSide.All )]
		public virtual Border3DSide BorderSides
		{
			get { return borderSides; }
			set
			{
				if( borderSides != value )
				{
					borderSides = value;
					this.OnBorderSidesChanged( EventArgs.Empty );
					InvalidateWindow();
				}
			}
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
		public virtual Border3DStyle Border3DStyle
		{
			get { return border3DStyle; }
			set
			{
				if( border3DStyle != value )
				{
					border3DStyle = value;
					this.OnBorder3DStyleChanged( EventArgs.Empty );
					this.InvalidateWindow();
				}
			}
		}
		/// <summary>
		/// Gets or sets the single border color for the control.
		/// </summary>
		/// <remarks>
		/// This property is used only when BorderStyle is FixedSingle.
		/// </remarks>
		[Description( "Indicates the color of the 2D border." )]
		[Category( "Appearance" )]
		public virtual Color BorderColor
		{
			get { return borderColor; }
			set
			{               
				if( borderColor != value &&style!=theme.Metro )
				{
					borderColor = value;
					this.OnBorderColorChanged( EventArgs.Empty );
					InvalidateWindow();
				}
                if (style == theme.Metro)
                    borderColor = metrocolor;

              }
		}


        /// <summary>
        /// Last overflow indicator state.
        /// </summary>
        internal OverflowIndicatorState OverFlowIndicatorLastState
        {
            get
            {
                return this.m_bOverflowIndicatorLastState;
            }
            set
            {
                if (this.m_bOverflowIndicatorLastState != value)
                    this.m_bOverflowIndicatorLastState = value;
            }
        }

		/// <summary>
		/// Overflow indicator ToolTip.
		/// </summary>
		protected internal ToolTipAdv OverflowIndicatorToolTip
		{
			get
			{
				if( null == m_overflowIndicatorToolTip )
				{
					m_overflowIndicatorToolTip = new ToolTipAdv( this );
					m_overflowIndicatorToolTip.BackColor = SystemColors.Info;
					m_overflowIndicatorToolTip.BorderStyle = BorderStyle.FixedSingle;
				}

				return m_overflowIndicatorToolTip;
			}
			set
			{
				m_overflowIndicatorToolTip = value;
			}
		}
		/// <summary>
		/// Indicates whether XPThemes should be used when BorderStyle is set to Fixed3D.
		/// </summary>
		/// <value>True to use XPThemes; False otherwise. Default is True.</value>
		/// <remarks>
		/// This property is used only when BorderStyle is Fixed3D.
		/// </remarks>
		[Description( "Specifies whether or not use XP Themes when BorderStyle = Fixed3D." )]
		[Category( "Appearance" )]
		[DefaultValue( true )]
		public virtual bool ThemesEnabled
		{
			get { return this.themesEnabled; }
			set
			{
				if( this.themesEnabled != value )
				{
					this.themesEnabled = value;
					this.UpdateStyles();
					InvalidateWindow();
				}
			}
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// Gets or sets the maximum size for the control.
		/// </summary>
		[Description( "Gets or sets the maximum size for the control." )]
		[Category( "Layout" )]
		public override Size MaximumSize
		{
			get { return base.MaximumSize; }
			set
			{
				if( base.MaximumSize != value )
				{
					if( value.Height == 0 )
					{
						value.Height = this.Height;
					}
					else
						if( value.Width == 0 )
						{
							value.Width = this.Width;
						}

					base.MaximumSize = value;
					this.OnMaximumSizeChanged( EventArgs.Empty );
				}
			}
		}

		/// <summary>
		/// Gets or sets the minimum size for the control.
		/// </summary>
		[Description( "Gets or sets the minimum size for the control." )]
		[Category( "Layout" )]
		public override Size MinimumSize
		{
			get { return base.MinimumSize; }
			set
			{
				if( !this.Initializing )
				{
					if( base.MinimumSize != value )
					{
						if( value.Height <= 0 )
						{
							value.Height = base.MinimumSize.Height;
						}
						if( value.Width <= 0 )
						{
							value.Width = base.MinimumSize.Width;
						}

						base.MinimumSize = GetMinSize( value );

						OnMinimumSizeChanged( EventArgs.Empty );
					}
				}
				else
				{
					Size szMin = base.MinimumSize;

					szMin.Height = Math.Max( szMin.Height, value.Height );
					szMin.Width = Math.Max( szMin.Width, value.Width );

					base.MinimumSize = szMin;
				}
			}
		}

		private Size GetMinSize( Size szCurrentMin )
		{
			Size szMin = m_cornerRadiusHelper.GetMinimalSize( szCurrentMin );

            if (this.NearImage != null)
            {
                szMin.Width += this.NearImage.Width + 1;
            }

            if (this.FarImage != null)
            {
                szMin.Width += this.FarImage.Width + 1;
            }
           
                 szMin = m_cornerRadiusHelper.GetMinimalSize(szCurrentMin);  

			return szMin;
		}
#endif
		/// <summary>
		/// Gets or sets the case of characters as they are typed.
		/// </summary>
		[
		  Category( "Behavior" ),
		  Browsable( true ),
		  DefaultValue( CharacterCasing.Normal ),
		  Description( "Gets or sets the case of characters as they are typed." )
		]
		public new CharacterCasing CharacterCasing
		{
			get
			{
				return base.CharacterCasing;
			}
			set
			{
				if( value != base.CharacterCasing )
				{
					base.CharacterCasing = value;
					OnCharacterCasingChanged( EventArgs.Empty );
				}
			}
		}

		/// <summary>
		/// Gets or sets corner radius of control.
		/// </summary>
		/// <remarks>Radius has to be not less than zero or half of minimum dimension (width or height) of control.
		/// If radius is zero, control has standard rectangular appearance.
		/// </remarks>
		[DefaultValue( 0 )]
		[Category( "Appearance" )]
		[Description( "Corner radius of control" )]
		public int CornerRadius
		{
			get
			{
				return m_cornerRadiusHelper.CornerRadius;
			}
			set
			{
				if( this.Initializing )
				{
					m_nCornerRadius = value;
				}
				else
				{
					if( this.BorderStyle != BorderStyle.Fixed3D )
					{
						m_cornerRadiusHelper.CornerRadius = value;
						UpdateNCArea();
					}
					else
					{
						throw new ArgumentException( "Curved corners are not supported for fixed 3D border style." );
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the near image.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Appearance" )]
		[Description( "Near image" )]
		public Image NearImage
		{
			get
			{
				return m_imgNear;
			}
			set
			{
				if( m_imgNear != value )
				{
					m_imgNear = value;
					this.MinimumSize = Size.Empty;
					UpdateNCArea();
				}
			}
		}

		/// <summary>
		/// Gets or sets the far image.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Appearance" )]
		[Description( "Far image" )]
		public Image FarImage
		{
			get
			{
				return m_imgFar;
			}
			set
			{
				if( m_imgFar != value )
				{
					m_imgFar = value;
					this.MinimumSize = Size.Empty;
					UpdateNCArea();
				}
			}
		}

		#endregion

		#region Class Events
		/// <summary>
		/// This event is raised if the BorderSides property is changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "This event is raised if the BorderSides property is changed." )]
		public event EventHandler BorderSidesChanged;
		/// <summary>
		/// This event is raised if the Border3DStyle property is changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "This event is raised if the Border3DStyle property is changed." )]
		public event EventHandler Border3DStyleChanged;
		/// <summary>
		/// This event is raised when the value of the BorderColor property is changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "This event is raised when the value of the BorderColor property is changed." )]
		public event EventHandler BorderColorChanged;
		/// <summary>
		/// This event is raised if the ThemesEnabled property is changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "This event is raised if the ThemesEnabled property is changed." )]
		public event EventHandler ThemesEnabledChanged;

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// This event is raised if the MaximumSize property is changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "This event is raised if the MaximumSize property is changed." )]
		public event EventHandler MaximumSizeChanged;
		/// <summary>
		/// This event is raised if the MinimumSize property is changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "This event is raised if the MinimumSize property is changed." )]
		public event EventHandler MinimumSizeChanged;
#endif

		/// <summary>
		/// This event is raised if the CharacterCasing property is changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "This event is raised if the CharacterCasing property is changed." )]
		public event EventHandler CharacterCasingChanged;
		#endregion

		#region Class Initialize/Finilize logic
		/// <summary>
		/// Creates a new instance of the TextBoxExt class.
		/// </summary>
		public TextBoxExt()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
				new LicensedComponent( typeof( TextBoxExt ) );
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
			}

			m_cornerRadiusHelper = new Utils.CornerRadiusHelper( this );
            CTRLSIZE = this.Size;

		}

		/// <summary></summary>
		private void Init()
		{
			if( null == this.cd )
			{
				this.cd = new ControlDrawing();
			}

			if( null == themedEditDrawing )
			{
				themedEditDrawing = new ThemedEditDrawing( this );
			}

			this.BorderStyle = base.BorderStyle;

			if( this.ShowOverflowIndicator )
			{
				UpdateOverflowIndicator();
			}

			this.BackColorChanged += new EventHandler( My_BackColorChanged );
		}

		/// <summary></summary>
		private void Uninit()
		{
			this.BackColorChanged -= new EventHandler( My_BackColorChanged );
		}

		/// <summary>Dispose all created internal object</summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( m_subClass != null )
				{
					m_subClass.StopListening();
					m_subClass = null;
				}

				this.cd = null;

				if( null != themedEditDrawing )
				{
					this.themedEditDrawing.Dispose();
					this.themedEditDrawing = null;
				}

				if( null != m_overflowIndicatorToolTip )
				{
					m_overflowIndicatorToolTip.Dispose();
					m_overflowIndicatorToolTip = null;
				}

				if( m_cornerRadiusHelper != null )
				{
					m_cornerRadiusHelper.Dispose();
					m_cornerRadiusHelper = null;
				}
			}

			base.Dispose( disposing );
		}
		#endregion

		#region Class codedom serialization
		/// <summary></summary>
		/// <returns></returns>
		private bool ShouldSerializeBorderColor()
		{
			return !( this.BorderColor == Color.Black );
		}

		/// <summary></summary>
		private void ResetBorderColor()
		{
			this.BorderColor = Color.Black;
		}

		#endregion

		#region Class overrides
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// Raises the MaximumSizeChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event. <para>The OnMaximumSizeChanged method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnMaximumSizeChanged 
		/// in a derived class, be sure to call the base class's 
		/// OnMaximumSizeChanged method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnMaximumSizeChanged( EventArgs e )
		{
			if( MaximumSizeChanged != null )
			{
				MaximumSizeChanged( this, e );
			}
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }

		/// <summary>
		/// Raises the MinimumSizeChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event. <para>The OnMinimumSizeChanged method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnMinimumSizeChanged 
		/// in a derived class, be sure to call the base class's 
		/// OnMinimumSizeChanged method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnMinimumSizeChanged( EventArgs e )
		{
			if( MinimumSizeChanged != null )
			{
				MinimumSizeChanged( this, e );
			}
		}
#endif


		/// <summary>
		/// Raises the CharacterCasingChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event. <para>The OnCharacterCasingChanged method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnCharacterCasingChanged 
		/// in a derived class, be sure to call the base class's 
		/// OnCharacterCasingChanged method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		public virtual void OnCharacterCasingChanged( EventArgs e )
		{
			if( CharacterCasingChanged != null )
			{
				CharacterCasingChanged( this, e );
			}
		}

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
			if( BorderSidesChanged != null )
			{
				BorderSidesChanged( this, e );
			}
		}

		/// <summary>
		/// Raises the BorderColorChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event. <para>The OnBorderColorChanged method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnBorderColorChanged 
		/// in a derived class, be sure to call the base class's 
		/// OnBorderColorChanged method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnBorderColorChanged( EventArgs e )
		{
			if( BorderColorChanged != null )
			{
				BorderColorChanged( this, e );
			}
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
			if( Border3DStyleChanged != null )
			{
				Border3DStyleChanged( this, e );
			}
		}

		/// <summary>
		/// Raises the ThemesEnabledChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event. <para>The OnThemesEnabledChanged method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnThemesEnabledChanged 
		/// in a derived class, be sure to call the base class's 
		/// OnThemesEnabledChanged method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnThemesEnabledChanged( EventArgs e )
		{
			if( ThemesEnabledChanged != null )
			{
				ThemesEnabledChanged( this, e );
			}
		}

		/// <summary></summary>
		/// <param name="e"></param>
		protected override void OnParentChanged( EventArgs e )
		{
			if( this.Parent != null )
			{

				if( m_subClass == null )
				{
					m_subClass = new TextBoxExtSubClass(this);
				}
				else
				{
					m_subClass.StopListening();
					m_subClass = new TextBoxExtSubClass(this);
					m_subClass.AssignHandleCustom(this.Parent);
				}
			}

			base.OnParentChanged( e );
		}

		/// <summary></summary>
		/// <param name="e"></param>
		protected override void OnEnabledChanged( EventArgs e )
		{
			this.UpdatePaintingStyle();
			base.OnEnabledChanged( e );
		}

		/// <summary></summary>
		/// <param name="e"></param>
		protected override void OnPaint( PaintEventArgs e )
		{
			// Handle TextAlign in OnPaint so that it does not get affected by ControlStyles.UserPaint set to true for DrawActiveWhenDisabled property.
			if( this.DrawActiveWhenDisabled && this.Enabled == false )
			{
				Brush b = new SolidBrush( this.ForeColor );
				SizeF txtSize = e.Graphics.MeasureString( this.Text, this.Font );

				if( this.TextAlign == HorizontalAlignment.Left )
				{
					e.Graphics.DrawString( this.Text, this.Font, b, this.ClientRectangle );
				}
				else if( this.TextAlign == HorizontalAlignment.Right )
				{
					e.Graphics.DrawString( this.Text, this.Font, b, this.ClientRectangle.Width - txtSize.Width, 0f );
				}
				else if( this.TextAlign == HorizontalAlignment.Center )
				{
					e.Graphics.DrawString( this.Text, this.Font, b,
						(int)( ( e.ClipRectangle.Width - txtSize.Width ) / 2 ),
						(int)( e.ClipRectangle.Height - txtSize.Height ) / 2 );
				}

				b.Dispose();
			}

			base.OnPaint( e );
		}

		/// <summary></summary>
		/// <param name="e"></param>
		protected override void OnHandleCreated( EventArgs e )
		{
			if( !this.Disposing )
			{
				base.OnHandleCreated( e );

				Init();
			}
		}

		/// <summary></summary>
		/// <param name="e"></param>
		protected override void OnHandleDestroyed( EventArgs e )
		{
			base.OnHandleDestroyed( e );

			Uninit();
		}

		/// <summary></summary>
		/// <param name="e"></param>
		protected override void OnMultilineChanged( EventArgs e )
		{
			base.OnMultilineChanged( e );

			this.UpdateOverflowIndicator();
		}

		/// <summary></summary>
		/// <param name="e"></param>
		protected override void OnTextChanged( EventArgs e )
		{
			base.OnTextChanged( e );

			this.UpdateOverflowIndicator();
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="m"></param>
		protected override void WndProc( ref Message m )
		{
			switch( m.Msg )
			{
				case NativeMethods.WM_LBUTTONDBLCLK:
				{
					if(Cursor==Cursors.Arrow)
						return;
					break;
				}
				case NativeMethods.WM_NCPAINT:
				{
					OnWmNcPaint( ref m );
					return;
				}
				case NativeMethods.WM_NCCALCSIZE:
				{
					OnWmNcCalcSize( ref m );
					return;
				}
				case NativeMethods.WM_NCHITTEST:
				{
					TrackMouseEvent( false );
					UpdateOverflowIndicatorToolTipVisible( new Point( (int)m.LParam & 0xffff, (int)m.LParam >> 16 ), true );

					base.WndProc( ref m );

					if( m.Result == (IntPtr)NativeMethods.HTNOWHERE )
					{
						m.Result = (IntPtr)NativeMethods.HTCLIENT;
						Cursor = Cursors.Arrow;
					}
					else
					{
						Cursor = Cursors.IBeam;
					}

					return;
				}
				case NativeMethods.WM_NCMOUSEHOVER:
				{
					TrackMouseEvent( true );
					UpdateOverflowIndicatorToolTipVisible( new Point( (int)m.LParam & 0xffff, (int)m.LParam >> 16 ), false );
					break;
				}
				case NativeMethods.WM_NCMOUSELEAVE:
				{
					if( OverflowIndicatorToolTip != null )
					{
						OverflowIndicatorToolTip.HidePopup();
					}
					break;
				}
				case NativeMethods.WM_CONTEXTMENU:
				{
					if( !this.ShortcutsEnabled )
					{
						return;
					}
					break;
				}
				case NativeMethods.WM_PRINT:
				{
					OnWmPrint( ref m );
					return;
				}
			}

			base.WndProc( ref m );
		}

		private void OnWmPrint( ref Message m )
		{
			IntPtr hDC = m.WParam;

			if( hDC != IntPtr.Zero )
			{
				IntPtr hOrigRgn = NativeMethods.CreateRectRgn( 0, 0, 0, 0 );

				using( Graphics g = Graphics.FromHdc( hDC ) )
				{
					g.FillRectangle( SystemBrushes.Control, new Rectangle( 0, 0, this.Width, this.Height ) );

					if( this.Region != null )
					{
						g.Clip = this.Region;
					}

					RenderNCArea( g, this.Width, this.Height, !false );
				}

				NativeMethods.GetClipRgn( hDC, hOrigRgn );
				NativeMethods.IntersectClipRect( hDC, m_rcClient.Left, m_rcClient.Top, m_rcClient.Right, m_rcClient.Bottom );

				base.WndProc( ref m );	// Print client area.

				NativeMethods.SelectClipRgn( hDC, hOrigRgn );
				NativeMethods.DeleteObject( hOrigRgn );
			}
		}

		/// <summary>
		/// Overflow indicators drawing.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="left"></param>
		/// <param name="right"></param>
		protected virtual void NCOverflowIndicatorPaint( Graphics g, Rectangle left, Rectangle right )
		{
			OverflowIndicatorState state = this.OverFlowIndicatorLastState;

			if( state == OverflowIndicatorState.Left ||
				state == OverflowIndicatorState.LeftAndRight )
			{
				DrawOverflowIndicator( g, left, true );
			}

			if( state == OverflowIndicatorState.Right ||
				state == OverflowIndicatorState.LeftAndRight )
			{
				DrawOverflowIndicator( g, right, false );
			}
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.SizeChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnSizeChanged( EventArgs e )
		{
			m_cornerRadiusHelper.UpdateRegion();

			base.OnSizeChanged( e );
            if (!EnableTouchMode && this.DesignMode)
            {
                CTRLSIZE = this.Size;
            }
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.TextBoxBase.BorderStyleChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnBorderStyleChanged( EventArgs e )
		{
			if( this.CornerRadius > 0 && this.BorderStyle == BorderStyle.Fixed3D )
			{
				this.BorderStyle = BorderStyle.FixedSingle;

				throw new ArgumentException( "Fixed 3D border style is not supported with curved corners." );
			}

			base.OnBorderStyleChanged( e );
		}

		#endregion

		#region Borders drawing
		/// <summary></summary>
		private void UpdatePaintingStyle()
		{
			if( this.DrawActiveWhenDisabled && this.Enabled == false )
			{
				this.SetStyle( ControlStyles.UserPaint, true );
			}
			else
			{
				this.SetStyle( ControlStyles.UserPaint, false );
			}

			this.Invalidate();
		}

		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void My_BackColorChanged( object sender, EventArgs e )
		{
			this.InvalidateWindow();
		}

		/// <summary></summary>
		private void InvalidateWindow()
		{
			if( this.IsHandleCreated )
			{
				const int flags = NativeMethods.RDW_FRAME | NativeMethods.RDW_UPDATENOW | NativeMethods.RDW_INVALIDATE;

				NativeMethods.RedrawWindow( this.Handle, IntPtr.Zero, IntPtr.Zero, flags );
			}
		}

		/// <summary></summary>
		/// <param name="pt"></param>
		/// <param name="hitTest"></param>
		private void UpdateOverflowIndicatorToolTipVisible( Point pt, bool hitTest )
		{
			if( ShowOverflowIndicatorToolTip )
			{
				Point clientPoint = pt;

				clientPoint.X -= this.Parent.PointToScreen( this.Location ).X;
				clientPoint.Y -= this.Parent.PointToScreen( this.Location ).Y;

				if( ( ( this.OverFlowIndicatorLastState == OverflowIndicatorState.Left ||
					this.OverFlowIndicatorLastState  == OverflowIndicatorState.LeftAndRight ) &&
					m_leftIndicatorLastPaintedRect.Contains( clientPoint ) ) ||
					( ( this.OverFlowIndicatorLastState == OverflowIndicatorState.Right ||
					this.OverFlowIndicatorLastState == OverflowIndicatorState.LeftAndRight ) &&
					m_rightIndicatorLastPaintedRect.Contains( clientPoint ) ) )
				{
					if( !OverflowIndicatorToolTip.IsShowing() && hitTest )
					{
						string toolTipText = ( OverflowIndicatorToolTipText == null ) ? Text :
							OverflowIndicatorToolTipText;

						OverflowIndicatorToolTip.Text = toolTipText;

						if( toolTipText != null && toolTipText.Length > 0 )
						{
							OverflowIndicatorToolTip.ShowPopup( new Point( pt.X + c_overflowIndicatorToolTipOffset.X,
																			 pt.Y + c_overflowIndicatorToolTipOffset.Y ) );
						}
					}
				}
				else
				{
					if( OverflowIndicatorToolTip.IsShowing() )
					{
						OverflowIndicatorToolTip.HidePopup();
					}
				}
			}
		}

		/// <summary></summary>
		/// <param name="useLeaveFlags"></param>
		private void TrackMouseEvent( bool useLeaveFlags )
		{
			NativeMethods.TRACKMOUSEEVENT tme = new NativeMethods.TRACKMOUSEEVENT();
			tme.cbSize = Marshal.SizeOf( tme );
			tme.hwndTrack = this.Handle;
			tme.dwFlags = NativeMethods.TME_NONCLIENT;

			if( useLeaveFlags )
			{
				tme.dwFlags |= NativeMethods.TME_LEAVE;
			}
			else
			{
				tme.dwFlags |= NativeMethods.TME_HOVER;
			}

			tme.dwHoverTime = 0;

			NativeMethods.TrackMouseEvent( ref tme );
		}

		/// <summary>
		/// Return GDI text size.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		private Size GetTextSize( string text )
		{
			Graphics g = this.CreateGraphics();

			IntPtr hdc = g.GetHdc();
			IntPtr hFont = this.Font.ToHfont();
			IntPtr prevFont = NativeMethods.SelectObject( hdc, hFont );

			NativeMethods.SIZE size = new NativeMethods.SIZE( 0, 0 );

			NativeMethods.GetTextExtentPoint32( hdc, text, text.Length, ref size );

			prevFont = NativeMethods.SelectObject( hdc, prevFont );
			NativeMethods.DeleteObject( hFont );

			g.ReleaseHdc( hdc );
			g.Dispose();

			return new Size( size.CX, size.CY );
		}

		/// <summary>
		///  Send WM_NCCALCSIZE message.
		/// </summary>
		internal void UpdateNCArea()
		{
			if( this.IsHandleCreated )
			{
				const int flags = NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE |
					NativeMethods.SWP_NOZORDER | NativeMethods.SWP_FRAMECHANGED;

				NativeMethods.SetWindowPos( this.Handle, IntPtr.Zero, 0, 0, 0, 0, flags );
			}
		}
		/// <summary>
		/// Send WM_NCCALCSIZE message and invalidate NC area.
		/// </summary>
		private void UpdateOverflowIndicator()
		{
			if( this.IsHandleCreated )
			{
				UpdateNCArea();

				NativeMethodsHelper.RedrawWindow( this.Handle, NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE );
			}
		}

		/// <summary>
		/// Return overflow indicator state.
		/// </summary>
		/// <returns></returns>
		internal OverflowIndicatorState GetOverflowIndicatorState()
		{
			OverflowIndicatorState state = OverflowIndicatorState.None;

			if( !this.Multiline &&
				this.ShowOverflowIndicator &&
				this.Text != string.Empty &&
				this.IsHandleCreated )
			{
				NativeMethods.RECT rect = new NativeMethods.RECT( 0, 0, 0, 0 );
				NativeMethods.SendMessage( this.Handle, NativeMethods.EM_GETRECT, IntPtr.Zero, ref rect );

				int charIndex = (int)NativeMethods.SendMessage( this.Handle,
				   NativeMethods.EM_CHARFROMPOS, 0, NativeMethods.MAKELPARAM( rect.left, rect.top ) );

				string text = this.Text;

                rect.left = rect.top = c_ioverflowIndicatorBorderIndent;

                int currentTextWidth = rect.Width;
                
				if( (charIndex > 0 || charIndex == -1 ) && this.GetTextSize( text ).Width > currentTextWidth )
				{
                    if (charIndex == text.Length)
                    {
                        state = OverflowIndicatorState.None;
                    }
                    else if( charIndex != -1 )
					{
						state = OverflowIndicatorState.Left;
						text = text.Remove( 0, charIndex + 1 );
					}
				}

                if (this.OverFlowIndicatorLastState == OverflowIndicatorState.Right
                    || this.OverFlowIndicatorLastState == OverflowIndicatorState.Left)
                {
                    currentTextWidth += c_sOverflowIndicator.Width;
                }
                else if (this.OverFlowIndicatorLastState == OverflowIndicatorState.LeftAndRight)
                {
                    currentTextWidth += (2 * c_sOverflowIndicator.Width);
                }

				if( this.GetTextSize( text ).Width > currentTextWidth )
				{
					if( state == OverflowIndicatorState.Left )
					{
						state = OverflowIndicatorState.LeftAndRight;
					}
                    else if (this.Text.Length == this.SelectionStart && this.SelectionLength == 0)
                    {
                        state = OverflowIndicatorState.Left;
                    }
                    else
                    {
                        state = OverflowIndicatorState.Right;
                    }
				}
			}

			return state;
		}
		/// <summary></summary>
		/// <param name="m"></param>
		private void OnWmNcCalcSize( ref Message m )
		{
			NativeMethods.RECT rcWindow = (NativeMethods.RECT)Marshal.PtrToStructure( m.LParam, typeof( NativeMethods.RECT ) );
			NativeMethods.RECT rcClient = new NativeMethods.RECT( 0, 0, rcWindow.Width, rcWindow.Height );

			if( m_cornerRadiusHelper != null )
			{
				int radius = this.CornerRadius;

				if( radius > 0 )
				{
					rcClient.left += radius; rcClient.right -= radius;
					rcWindow.left += radius; rcWindow.right -= radius;
				}
			}

			bool bLTR = ( this.RightToLeft != RightToLeft.Yes );

			if( this.NearImage != null )
			{
				Rectangle rcImg = GetImageBounds( this.NearImage, rcClient, true );

				ModifyRect( bLTR, rcImg.Width, ref rcClient, ref rcWindow );
			}

			if( this.FarImage != null )
			{
				Rectangle rcImg = GetImageBounds( this.FarImage, rcClient, false );

				ModifyRect( !bLTR, rcImg.Width, ref rcClient, ref rcWindow );
			}

			OverflowIndicatorState state = GetOverflowIndicatorState();

			if( state != OverflowIndicatorState.None )
			{
				int indicatorWidth = c_sOverflowIndicator.Width;

				if( state == OverflowIndicatorState.Left ||
					state == OverflowIndicatorState.LeftAndRight )
				{
					rcClient.left += indicatorWidth;
					rcWindow.left += indicatorWidth;
				}

				if( state == OverflowIndicatorState.Right ||
					state == OverflowIndicatorState.LeftAndRight )
				{
					rcClient.right -= indicatorWidth;
					rcWindow.right -= indicatorWidth;
				}
			}

			Marshal.StructureToPtr( rcWindow, m.LParam, false );

			this.OverFlowIndicatorLastState = state;

			int style = NativeMethods.GetWindowLong( m.HWnd, NativeMethods.GWL_STYLE );
			int styleEx = NativeMethods.GetWindowLong( m.HWnd, NativeMethods.GWL_EXSTYLE );

			NativeMethods.RECT rc = new NativeMethods.RECT( 0, 0, 0, 0 );
			AdjustWindowRectEx( ref rc, style, false, styleEx );

			m_rcClient = new Rectangle( rcClient.left - rc.left, rcClient.top - rc.top, rcClient.Width - rc.Width, rcClient.Height - rc.Height );

			base.WndProc( ref m );
		}

		private Rectangle GetImageBounds( Image image, NativeMethods.RECT rcClient, bool nearImage )
		{
			Size szImage = image.Size;
			int borderDim = this.BorderWidth;
			int clientHeight = rcClient.Height;
			int maxDim = clientHeight - 2*borderDim + clientHeight%2;

			if( szImage.Height > szImage.Width )
			{
				if( szImage.Height > maxDim )
				{
					szImage.Width = szImage.Width * maxDim / szImage.Height;
					szImage.Height = maxDim;
				}
			}
			else if( szImage.Width > maxDim )
			{
				szImage.Height = szImage.Height * maxDim / szImage.Width;
				szImage.Width = maxDim;
			}

			Point ptLoc = new Point( 0, rcClient.top + borderDim + ( maxDim - szImage.Height )/2 );

			borderDim += this.CornerRadius;

			if( nearImage == ( this.RightToLeft != RightToLeft.Yes ) )
			{
				ptLoc.X = rcClient.left + borderDim;
			}
			else
			{
				ptLoc.X = rcClient.right - szImage.Width - borderDim;
			}

			return new Rectangle( ptLoc, szImage );
		}

		private void ModifyRect( bool bLTR, int width, ref NativeMethods.RECT rcClient, ref NativeMethods.RECT rcWindow )
		{
			width += this.BorderWidth;

			if( bLTR )
			{
				rcClient.left += width;
				rcWindow.left += width;
			}
			else
			{
				rcClient.right -= width;
				rcWindow.right -= width;
			}
		}

		private int BorderWidth
		{
			get
			{
				int width = 0;

				switch( this.BorderStyle )
				{
					case BorderStyle.Fixed3D:
					width = 2;
					break;
					case BorderStyle.FixedSingle:
					width = 1;
					break;
				}

				return width;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		private void OnWmNcPaint( ref Message m )
		{
			NativeMethods.RECT rcWnd = new NativeMethods.RECT();
			NativeMethods.GetWindowRect( m.HWnd, ref rcWnd );

			int width = rcWnd.Width;
			int height = rcWnd.Height;

			IntPtr hRgn = NativeMethods.CreateRectRgn( 0, 0, width, height );
			if( hRgn != IntPtr.Zero )
			{
				IntPtr hDc = NativeMethods.GetWindowDC( m.HWnd );
				if( hDc != IntPtr.Zero )
				{
					IntPtr memDc = NativeMethods.CreateCompatibleDC( hDc );
					if( memDc != IntPtr.Zero )
					{
						IntPtr hBmp = NativeMethods.CreateCompatibleBitmap( hDc, width, height );
						if( hBmp != IntPtr.Zero )
						{
							IntPtr hObj = NativeMethods.SelectObject( memDc, hBmp );

							if( m.WParam.ToInt32() != 1 )
							{
								IntPtr tmpRgn = NativeMethods.CreateRectRgn( 0, 0, 0, 0 );

								NativeMethods.CombineRgn( tmpRgn, m.WParam, IntPtr.Zero, NativeMethods.RGN_COPY );
								NativeMethods.OffsetRgn( tmpRgn, -rcWnd.left, -rcWnd.top );
								NativeMethods.CombineRgn( hRgn, hRgn, tmpRgn, NativeMethods.RGN_AND );

								NativeMethods.DeleteObject( tmpRgn );
							}

							using( Graphics g = Graphics.FromHdc( memDc ) )
							{
								RenderNCArea( g, width, height, true );
							}

							NativeMethods.SelectClipRgn( hDc, hRgn );
							NativeMethods.ExcludeClipRect( hDc, m_rcClient.X, m_rcClient.Y, m_rcClient.Right, m_rcClient.Bottom );

							NativeMethods.BitBlt( hDc, 0, 0, rcWnd.Width, rcWnd.Height, memDc, 0, 0, 0x00CC0020/*SRCCOPY*/ );

							NativeMethods.SelectObject( memDc, hObj );
							NativeMethods.DeleteObject( hBmp );
						}

						NativeMethods.DeleteDC( memDc );
					}
					NativeMethods.DeleteDC( hDc );
				}

				IntPtr hTmp = NativeMethods.CreateRectRgn( m_rcClient.Left, m_rcClient.Top, m_rcClient.Right, m_rcClient.Bottom );
				if( hTmp != IntPtr.Zero )
				{
					NativeMethods.CombineRgn( hRgn, hRgn, hTmp, NativeMethods.RGN_AND );
					NativeMethods.OffsetRgn( hRgn, rcWnd.left, rcWnd.top );
					NativeMethods.DeleteObject( hTmp );

					Message msg = new Message();

					msg.Msg = m.Msg;
					msg.HWnd = m.HWnd;
					msg.WParam = hRgn;

					base.WndProc( ref msg );
				}
				NativeMethods.DeleteObject( hRgn );
			}

			m.Result = IntPtr.Zero;
		}

		private void RenderNCArea( Graphics g, int width, int height, bool bDrawBorder )
		{
			Rectangle rc = new Rectangle( 0, 0, width, height );
			int radius = this.CornerRadius;

			using( Brush brush = new SolidBrush( this.BackColor ) )
            {
				g.FillRectangle( brush, 0, 0, width, height );
			}

			DrawImages( g, rc );

			if( bDrawBorder )
			{
				DrawBorders( rc, g, radius );
			}

			DrawOverflowIndicators( g );
		}

		private void DrawImages( Graphics g, Rectangle rc )
		{
			DrawImage( g, this.NearImage, rc, true );
			DrawImage( g, this.FarImage, rc, false );
		}

		private void DrawImage( Graphics g, Image image, Rectangle rc, bool nearImage )
		{
			if( image != null )
			{
				Rectangle bounds = GetImageBounds( image, new NativeMethods.RECT( rc ), nearImage );

				g.DrawImage( image, bounds );
			}
		}
        public enum theme
        {
            Default,
            Office2007,
            Office2010,
            Metro
        }
        private theme style;
        [Category("Appearance")]
        public theme Style
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
                if (style == theme.Metro)
                {
                    this.BackColor = Color.White;
                    this.BorderStyle = BorderStyle.FixedSingle;
                    this.BorderColor = metrocolor;
                }
                else if (style == theme.Office2007)
                {
                    this.BorderStyle = BorderStyle.FixedSingle;
                    OnOffice2007ColorSchemeChanged();
                }
                else if (style == theme.Office2010)
                {
                    this.BorderStyle = BorderStyle.FixedSingle;
                    OnOffice2010ColorSchemeChanged();
                }
            }
        }
        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string skinstyle;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return skinstyle;
            }
            set
            {
                skinstyle = value;

                if (value == "Office2007Blue")
                {
                    Style = theme.Office2007;
                    Office2007ColorScheme = Office2007Theme.Blue;
                }
                else if (value == "Office2007Silver")
                {
                    Style = theme.Office2007;
                    Office2007ColorScheme = Office2007Theme.Silver;
                }
                else if (value == "Office2007Black")
                {
                    Style = theme.Office2007;
                    Office2007ColorScheme = Office2007Theme.Black;
                }
                else if (value == "Office2010Blue")
                {
                    Style = theme.Office2010;
                    Office2010ColorScheme = Office2010Theme.Blue;
                }
                else if (value == "Office2010Silver")
                {
                    Style = theme.Office2010;
                    Office2010ColorScheme = Office2010Theme.Silver;
                }
                else if (value == "Office2010Black")
                {
                    Style = theme.Office2010;
                    Office2010ColorScheme = Office2010Theme.Black;
                }
                else if (value == "Managed")
                {
                    Style = theme.Office2007;
                    Office2007ColorScheme = Office2007Theme.Managed;
                }                
                else if (value == "Metro")
                    Style = theme.Metro;
                else
                    Style = theme.Default;
            }
        }
        private Office2010Theme m_Office2010ColorScheme = Office2010Theme.Blue;
        /// <summary>
        /// Office2010 color scheme.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        Description("Specifies color scheme for the control."),
        DefaultValue(Office2010Theme.Blue)
        ]
        public Office2010Theme Office2010ColorScheme
        {
            get
            {
                return m_Office2010ColorScheme;
            }
            set
            {
                if (m_Office2010ColorScheme != value)
                {
                    m_Office2010ColorScheme = value;
                    OnOffice2010ColorSchemeChanged();
                    this.Invalidate();
                }
            }
        }
        private void OnOffice2010ColorSchemeChanged()
        {
            if (style == theme.Office2010)
            {
                this.BorderColor = this.Office2010ColorTable.InactiveTextBoxBorderColor;
                this.BackColor = this.Office2010ColorTable.InactiveTextBoxBackColor;
                this.Invalidate();
            }
        }
        /// <summary>
        /// Gets color table for Office2007 visual style.
        /// </summary>
        protected Office2010Colors Office2010ColorTable
        {
            get
            {
                Office2010Colors colorTable = Office2010Colors.GetColorTable(this.Office2010ColorScheme);

                return colorTable;
            }
        }

        private Office2007Theme m_Office2007ColorScheme = Office2007Theme.Blue;
        /// <summary>
        /// Office2007 color scheme.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        Description("Specifies color scheme for the control."),
        DefaultValue(Office2007Theme.Blue)
        ]
        public Office2007Theme Office2007ColorScheme
        {
            get
            {
                return m_Office2007ColorScheme;
            }
            set
            {
                if (m_Office2007ColorScheme != value)
                {
                    m_Office2007ColorScheme = value;
                    OnOffice2007ColorSchemeChanged();
                    this.Invalidate();
                }
            }
        }
        private void OnOffice2007ColorSchemeChanged()
        {
            if (style == theme.Office2007)
            {
                this.BorderColor = this.Office2007ColorTable.InactiveTextBoxBorderColor;
                this.BackColor = this.Office2007ColorTable.InactiveTextBoxBackColor;
            }
        }
        /// <summary>
        /// Gets color table for Office2007 visual style.
        /// </summary>
        protected Office2007Colors Office2007ColorTable
        {
            get
            {
                Office2007Colors colorTable = Office2007Colors.GetColorTable(this.Office2007ColorScheme);

                return colorTable;
            }
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (style == theme.Metro)
            {

                this.borderColor = metrocolor;
            }
            else if (style == theme.Office2007)
            {
                this.BorderColor = this.Office2007ColorTable.InactiveTextBoxBorderColor;// Color.FromArgb(185, 200, 220);
                this.BackColor = this.Office2007ColorTable.InactiveTextBoxBackColor;
            }
            else if (style == theme.Office2010)
            {
                this.BorderColor = this.Office2010ColorTable.InactiveTextBoxBorderColor;// Color.FromArgb(185, 200, 220);
                this.BackColor = this.Office2010ColorTable.InactiveTextBoxBackColor;
            }
        }

		private void DrawBorders( Rectangle rc, Graphics g, int radius )
		{
			if( this.BorderStyle != BorderStyle.None )
			{

				bool bThemesEnabled = ( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled );

				if( bThemesEnabled && this.BorderStyle == BorderStyle.Fixed3D )
				{
					themedEditDrawing.DrawEditBoxBackground( g, rc, this.Enabled );
				}
				else
				{
					if( radius > 0 && this.BorderStyle == BorderStyle.FixedSingle && this.BorderSides == Border3DSide.All )
					{
						GraphicsState state = g.Save();

						g.SmoothingMode = SmoothingMode.AntiAlias;

						using( Pen pen = new Pen( Color.FromArgb( 128, this.BorderColor ), this.BorderWidth ) )
						{
							RectangleF rcPath = new RectangleF( 1, 1, rc.Width - 2, rc.Height - 2 );

							using( GraphicsPath path = m_cornerRadiusHelper.GetPath( rcPath ) )
							{
								g.DrawPath( pen, path );
								path.Reverse();
								g.DrawPath( pen, path );
							}
						}

						g.Restore( state );
					}
					else if( this.BorderSides != Border3DSide.All )
					{
						if( this.BorderSides != Border3DSide.Middle )
						{
							cd.DrawBorder( g, rc, this.BorderStyle, this.border3DStyle,
								ButtonBorderStyle.Solid, this.borderColor, this.borderSides );
						}
					}
					else
					{
						cd.DrawBorder( g, rc, this.BorderStyle, this.border3DStyle, ButtonBorderStyle.Solid, this.borderColor );
					}
				}
			}
		}

		private void DrawOverflowIndicators( Graphics g )
		{
			int indicatorWidth = c_sOverflowIndicator.Width;
			Rectangle rcLeft = new Rectangle( m_rcClient.Left - indicatorWidth, m_rcClient.Top, indicatorWidth, m_rcClient.Height );
			Rectangle rcRight = new Rectangle( m_rcClient.Right, m_rcClient.Top, indicatorWidth, m_rcClient.Height );

			NCOverflowIndicatorPaint( g, rcLeft, rcRight );
		}

		/// <summary>
		/// Draw one overflow indicator.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rect"></param>
		/// <param name="mirrored"></param>
		private void DrawOverflowIndicator( Graphics g, Rectangle rect, bool mirrored )
		{
			// Background
			using( Brush brush = new SolidBrush( this.BackColor ) )
			{
				g.FillRectangle( brush, rect );
			}

			Rectangle indicatorRect = new Rectangle(
				rect.X + ( rect.Width - c_sOverflowIndicator.Width ) / 2,
				rect.Y + ( rect.Height - c_sOverflowIndicator.Height ) / 2,
				c_sOverflowIndicator.Width, c_sOverflowIndicator.Height );

			// Save last painted indicator Rect
			if( mirrored )
			{
				m_leftIndicatorLastPaintedRect = indicatorRect;
			}
			else
			{
				m_rightIndicatorLastPaintedRect = indicatorRect;
			}

			// Foreground
			using( Pen pen = new Pen( c_overflowIndicatorBorderColor ) )
			{
				// Draw border
				g.DrawRectangle( pen,
					indicatorRect.X, indicatorRect.Y,
					indicatorRect.Width - 1, indicatorRect.Height - 1 );
			}

			indicatorRect.Inflate( -c_ioverflowIndicatorBorderIndent, -c_ioverflowIndicatorBorderIndent ); // Border indent

			DrawOverflowIndicatorForegroundElement( g, indicatorRect, mirrored );

			indicatorRect.X += c_overflowIndicatorElementSize.Width + 1; // Correct rect for draw next element

			DrawOverflowIndicatorForegroundElement( g, indicatorRect, mirrored );
		}

		/// <summary>
		/// Draw overflow indicator foregraund element.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rect"></param>
		/// <param name="mirrored"></param>
		private void DrawOverflowIndicatorForegroundElement( Graphics g, Rectangle rect, bool mirrored )
		{
			using( GraphicsPath gp = new GraphicsPath() )
			{
				if( mirrored )
				{
					gp.AddLine( rect.X + c_overflowIndicatorElementSize.Width, rect.Y,
								rect.X + c_overflowIndicatorElementSize.Width, rect.Y + c_overflowIndicatorElementSize.Height );
					gp.AddLine( rect.X + c_overflowIndicatorElementSize.Width, rect.Y + c_overflowIndicatorElementSize.Height,
								rect.X, rect.Y + c_overflowIndicatorElementSize.Height / 2 );
					gp.AddLine( rect.X, rect.Y + c_overflowIndicatorElementSize.Height / 2,
								rect.X + c_overflowIndicatorElementSize.Width, rect.Y );
				}
				else
				{
					gp.AddLine( rect.X, rect.Y, rect.X, rect.Y + c_overflowIndicatorElementSize.Height );
					gp.AddLine( rect.X, rect.Y + c_overflowIndicatorElementSize.Height,
								rect.X + c_overflowIndicatorElementSize.Width, rect.Y + c_overflowIndicatorElementSize.Height / 2 );
					gp.AddLine( rect.X + c_overflowIndicatorElementSize.Width, rect.Y + c_overflowIndicatorElementSize.Height / 2,
								rect.X, rect.Y );
				}

				gp.CloseFigure();

				using( Brush brush = new SolidBrush( c_overflowIndicatorForegroundElementColor ) )
				{
					g.FillPath( brush, gp );
				}
			}
		}

		#endregion

		#region WinAPI
		[DllImport( "user32.dll", CharSet=CharSet.Auto )]
		static extern int AdjustWindowRectEx( [In, Out]ref NativeMethods.RECT lpRect, int dwStyle, bool bMenu, int dwExStyle );
		#endregion

		#region ISupportInitialize implementation

		private int m_nInitializing = 0;

		/// <summary>
		/// Signals the object that initialization is starting.
		/// </summary>
		public virtual void BeginInit()
		{
			++m_nInitializing;
		}

		/// <summary>
		/// Signals the object that initialization is complete.
		/// </summary>
		public virtual void EndInit()
		{
			if( m_nInitializing > 0 )
			{
				--m_nInitializing;
			}
			else
			{
				throw new InvalidOperationException( "EndInit() called without appropriate BeginInit() method call." );
			}

			if( !this.Initializing )
			{
				if( m_nCornerRadius > 0 )
				{
					this.CornerRadius = m_nCornerRadius;	
				}

				this.MinimumSize = base.MinimumSize;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="TextBoxExt"/> is initializing.
		/// </summary>
		/// <value><c>true</c> if initializing; otherwise, <c>false</c>.</value>
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		[Browsable( false )]
		public bool Initializing
		{
			get
			{
				return ( m_nInitializing > 0 );
			}
		}

		#endregion
	}

	/// <summary></summary>
	internal enum OverflowIndicatorState
	{
		/// <summary></summary>
		None,
		/// <summary></summary>
		Left,
		/// <summary></summary>
		Right,
		/// <summary></summary>
		LeftAndRight
	}

	#region TextBoxExt Designer class
	/// <summary></summary>
	public class TextBoxExtDesigner: System.Windows.Forms.Design.ControlDesigner
	{
		/// <summary>
		/// Instance of TextBoxExt
		/// </summary>
		private TextBoxExt m_txtBox = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="TextBoxExtDesigner"/> class.
		/// </summary>
		public TextBoxExtDesigner()
			: base()
		{

		}

		/// <summary>
		/// Glyph for <see cref="TextBoxExt"/> representation.
		/// </summary>
		class TextBoxExtGlyph:
			ControlBodyGlyph
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="TextBoxExtGlyph"/> class.
			/// </summary>
			/// <param name="bounds">The bounds.</param>
			/// <param name="cursor">The cursor.</param>
			/// <param name="relatedComponent">The related component.</param>
			/// <param name="designer">The designer.</param>
			public TextBoxExtGlyph( Rectangle bounds, Cursor cursor, IComponent relatedComponent, ControlDesigner designer ) :
				base( bounds, cursor, relatedComponent, designer )
			{
			}

			/// <summary>
			/// Indicates whether a mouse click at the specified point should be handled by the <see cref="T:System.Windows.Forms.Design.Behavior.ControlBodyGlyph"/>.
			/// </summary>
			/// <param name="p">A point to hit test.</param>
			/// <returns>
			/// A <see cref="T:System.Windows.Forms.Cursor"/> if the <see cref="T:System.Windows.Forms.Design.Behavior.Glyph"/> is associated with <paramref name="p"/>; otherwise, null.
			/// </returns>
			public override Cursor GetHitTest( Point p )
			{
				Cursor cursor = base.GetHitTest( p );
				Rectangle bounds = this.Bounds;

				if( cursor != null && !HitTest( this.RelatedComponent as TextBoxExt, ref p, ref bounds ) )
				{
					cursor = null;
				}

				return cursor;
			}

			public static bool HitTest( TextBoxExt textBox, ref Point p, ref Rectangle bounds )
			{
				bool result = false;

				if( textBox != null && textBox.Region != null )
				{
					p.Offset( -bounds.Location.X, -bounds.Location.Y );
					result = textBox.Region.IsVisible( p );
				}

				return result;
			}
		}

		#region Overrides

		/// <summary>
		/// Initializes the designer with the specified component.
		/// </summary>
		/// <param name="component">The <see cref="T:System.ComponentModel.IComponent"/> to associate the designer with. This component must always be an instance of, or derive from, <see cref="T:System.Windows.Forms.Control"/>.</param>
		public override void Initialize( IComponent component )
		{
			base.Initialize( component );

			m_txtBox = component as TextBoxExt;

			if( m_txtBox != null )
			{
				m_txtBox.TextAlignChanged += new EventHandler( OnTextAlignChanged );
				m_txtBox.TextChanged += new EventHandler( OnTextChanged );
				m_txtBox.ForeColorChanged += new EventHandler( OnForeColorChanged );
				m_txtBox.BackColorChanged += new EventHandler( OnBackColorChanged );
				m_txtBox.CharacterCasingChanged += new EventHandler( OnCharacterCasingChanged );
			}
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		System.ComponentModel.Design.DesignerActionListCollection actionLists;

		/// <summary>
		/// Gets the design-time action lists supported by the component associated with the designer.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The design-time action lists supported by the component associated with the designer.
		/// </returns>
		public override DesignerActionListCollection ActionLists
		{
			get
			{
				if( null == actionLists )
				{
					actionLists = new DesignerActionListCollection();
					actionLists.Add(
						new TextBoxExtActionList( this.Component ) );
				}
				return actionLists;
			}
		}
#endif
		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Design.ControlDesigner"/> and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose( bool disposing )
		{
			if( m_txtBox != null )
			{
				m_txtBox.TextAlignChanged -= new EventHandler( OnTextAlignChanged );
				m_txtBox.TextChanged -= new EventHandler( OnTextChanged );
				m_txtBox.ForeColorChanged -= new EventHandler( OnForeColorChanged );
				m_txtBox.BackColorChanged -= new EventHandler( OnBackColorChanged );
				m_txtBox.CharacterCasingChanged -= new EventHandler( OnCharacterCasingChanged );
				m_txtBox = null;
			}

			base.Dispose( disposing );
		}

		/// <summary>
		/// Gets the selection rules that indicate the movement capabilities of a component.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// A bitwise combination of <see cref="T:System.Windows.Forms.Design.SelectionRules"/> values.
		/// </returns>
		public override SelectionRules SelectionRules
		{
			get
			{

				if (m_txtBox.Parent!=null 
					&& m_txtBox.Parent.GetType().ToString() == "Syncfusion.Windows.Forms.Tools.ButtonEdit")
				{
					return SelectionRules.None;
				}

				SelectionRules rules = base.SelectionRules;

				if( m_txtBox != null )
				{
					if( !m_txtBox.Multiline )
					{
						rules &= ~( SelectionRules.TopSizeable | SelectionRules.BottomSizeable );
					}
				}

				return rules;
			}
		}

		/// <summary>
		/// Returns a <see cref="T:System.Windows.Forms.Design.Behavior.ControlBodyGlyph"/> representing the bounds of this control.
		/// </summary>
		/// <param name="selectionType">A <see cref="T:System.Windows.Forms.Design.Behavior.GlyphSelectionType"/> value that specifies the selection state.</param>
		/// <returns>
		/// A <see cref="T:System.Windows.Forms.Design.Behavior.ControlBodyGlyph"/>.
		/// </returns>
		protected override ControlBodyGlyph GetControlGlyph( GlyphSelectionType selectionType )
		{
			ControlBodyGlyph baseGlyph = base.GetControlGlyph( selectionType );
			Cursor cursor = Cursor.Current;
			ISelectionService service = (ISelectionService)this.GetService( typeof( ISelectionService ) );

			if( ( service == null || !service.GetComponentSelected( this.Control ) ) && ( cursor == Cursors.SizeAll ) )
			{
				cursor = Cursors.Default;
			}

			ControlBodyGlyph glyph = new TextBoxExtGlyph( baseGlyph.Bounds, cursor, m_txtBox, this );

			return glyph;
		}

		#endregion

		#region Implementation

		/// <summary>
		///  Raises the TextAlignChanged event.
		/// </summary>
		/// <param name="sender">The TextBox control that sends the event.</param>
		/// <param name="e">The event data.</param>
		private void OnTextAlignChanged( object sender, EventArgs e )
		{
			TextBoxExt tbe = sender as TextBoxExt;
			if( tbe != null )
			{
				PropertyDescriptor pd = TypeDescriptor.GetProperties( tbe )["TextAlign"];
				if( pd != null )
				{
					RaiseComponentChanged( pd, tbe.TextAlign, tbe.TextAlign );
				}
			}
		}

		/// <summary>
		///  Raises the TextChanged event.
		/// </summary>
		/// <param name="sender">The TextBox control that sends the event.</param>
		/// <param name="e">The event data.</param>
		private void OnTextChanged( object sender, EventArgs e )
		{
			TextBoxExt tbe = sender as TextBoxExt;
			if( tbe != null )
			{
				PropertyDescriptor pd = TypeDescriptor.GetProperties( tbe )["Text"];
				if( pd != null )
				{
					RaiseComponentChanged( pd, tbe.Text, tbe.Text );
				}
			}
		}

		/// <summary>
		///  Raises the ForeColorChanged event.
		/// </summary>
		/// <param name="sender">The TextBox control that sends the event.</param>
		/// <param name="e">The event data.</param>
		private void OnForeColorChanged( object sender, EventArgs e )
		{
			TextBoxExt tbe = sender as TextBoxExt;
			if( tbe != null )
			{
				PropertyDescriptor pd = TypeDescriptor.GetProperties( tbe )["ForeColor"];
				if( pd != null )
				{
					RaiseComponentChanged( pd, tbe.ForeColor, tbe.ForeColor );
				}
			}
		}

		/// <summary>
		///  Raises the CharacterCasingChanged event.
		/// </summary>
		/// <param name="sender">The TextBox control that sends the event.</param>
		/// <param name="e">The event data.</param>
		private void OnCharacterCasingChanged( object sender, EventArgs e )
		{
			TextBoxExt tbe = sender as TextBoxExt;
			if( tbe != null )
			{
				PropertyDescriptor pd = TypeDescriptor.GetProperties( tbe )["CharacterCasing"];
				if( pd != null )
				{
					RaiseComponentChanged( pd, tbe.CharacterCasing, tbe.CharacterCasing );
				}
			}
		}

		/// <summary>
		///  Raises the BackColorChanged event.
		/// </summary>
		/// <param name="sender">The TextBox control that sends the event.</param>
		/// <param name="e">The event data.</param>
		private void OnBackColorChanged( object sender, EventArgs e )
		{
			TextBoxExt tbe = sender as TextBoxExt;
			if( tbe != null )
			{
				PropertyDescriptor pd = TypeDescriptor.GetProperties( tbe )["BackColor"];
				if( pd != null )
				{
					RaiseComponentChanged( pd, tbe.BackColor, tbe.BackColor );
				}
			}
		}
		#endregion
	}

	#endregion

	#region TextBoxExt Action List class
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
	public class TextBoxExtActionList: SyncActionListBase<TextBoxExt>
	{

		public TextBoxExtActionList( IComponent component )
			: base( component )
		{
		}

		protected override void InitializeActionList()
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties( this.Component );

			this.AddDesignerActionHeaderItem( "Essential Tools - TextBoxExt" );

			this.AddDesignerActionPropertyItem( "Name", "Name", "Design", "Indicates the Name used in code to identify the Object." );

			//Appearance category.
			this.AddDesignerActionHeaderItem( "Appearance" );

			System.ComponentModel.PropertyDescriptor pdText = properties.Find( "Text", false );
			if( pdText != null && pdText.IsBrowsable )
			{
				this.AddDesignerActionPropertyItem( "Text", "Text", "Appearance", "The text associated with the control." );
			}

			this.AddDesignerActionPropertyItem( "TextAlign", "Text Alignment", "Appearance", "Gets or sets how text is aligned in a TextBoxExt control." );
			this.AddDesignerActionPropertyItem( "BackColor", "BackColor", "Appearance", "Specifies BackColor" );


			System.ComponentModel.PropertyDescriptor pdWordWrap = properties.Find( "WordWrap", false );
			bool bShowWordWrap = ( pdWordWrap != null && pdWordWrap.IsBrowsable );

			System.ComponentModel.PropertyDescriptor pdMultiline = properties.Find( "Multiline", false );
			bool bShowMultiline = ( pdMultiline != null && pdMultiline.IsBrowsable );

			if( bShowWordWrap || bShowMultiline )
			{
				//Behavior Category
				this.AddDesignerActionHeaderItem( "Behavior" );
				if( bShowWordWrap )
				{
					this.AddDesignerActionPropertyItem( "WordWrap", "Word Wrap", "Behavior", "Indicates whether a multiline text box control automatically wraps words to the beginning of the next line when necessary." );
				}
				if( bShowMultiline )
				{
					this.AddDesignerActionPropertyItem( "Multiline", "Multiline", "Behavior", "Gets or sets a value indicating whether this is a multiline TextBoxExt control." );
				}
			}
		}


		public string Name
		{

			get
			{
				string name = " ";
				if( this.Control != null )
				{
					TextBoxExt control = this.Control as TextBoxExt;
					name = control.Name;
				}
				return name;
			}
			set
			{
				SetValue( "Name", value );
			}
		}

		public string Text
		{

			get
			{
				string text = " ";
				if( this.Control != null )
				{
					TextBoxExt control = this.Control as TextBoxExt;
					text = control.Text;
				}
				return text;
			}
			set
			{
				SetValue( "Text", value );
			}
		}

		public HorizontalAlignment TextAlign
		{
			get
			{
				HorizontalAlignment bTextAlign = HorizontalAlignment.Left;
				if( this.Control != null )
				{
					TextBoxExt control = this.Control as TextBoxExt;
					bTextAlign = control.TextAlign;
				}
				return bTextAlign;
			}
			set
			{
				SetValue( "TextAlign", value );
			}
		}

		public bool WordWrap
		{
			get
			{
				bool wordWrap = true;
				if( this.Control != null )
				{
					TextBoxExt control = this.Control as TextBoxExt;
					wordWrap = control.WordWrap;
				}
				return wordWrap;
			}
			set
			{
				SetValue( "WordWrap", value );
			}
		}
		public virtual Color BackColor
		{
			get
			{
				Color backColor = Color.White;
				if( this.Control != null )
				{
					TextBoxExt control = this.Control as TextBoxExt;
					backColor = control.BackColor;
				}
				return backColor;
			}
			set
			{
				SetValue( "BackColor", value );
			}
		}


		public bool Multiline
		{
			get
			{
				bool multiLine = true;
				if( this.Control != null )
				{
					TextBoxExt control = this.Control as TextBoxExt;
					multiLine = control.Multiline;
				}
				return multiLine;
			}
			set
			{
				SetValue( "Multiline", value );
			}
		}
	}
#endif
	#endregion
}

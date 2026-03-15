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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Syncfusion.Windows.Forms;
using System.Drawing.Drawing2D;
using Syncfusion.Runtime.InteropServices;

using Syncfusion.Drawing;
using System.Reflection;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// The GradientPanel is a <see cref="Panel"/>-derived class that has numerous border styles and has a background composed of a multiple gradient brush.
	/// 
	/// </summary>
	[Designer( typeof( ScrollableControlDesigner ) )]
	[
	System.Drawing.ToolboxBitmap( typeof( Syncfusion.Windows.Forms.PopupControlContainer ), "ToolboxIcons.GradientPanel.bmp" )
	]
	[Description( "Panel derived control that has numerous border styles and has a background composed of a multiple gradient brush." )]
	public class GradientPanel: System.Windows.Forms.Panel, IThemedControl, INonClientPaintingSupport, ISupportInitialize
	{
		private Border3DSide borderSides = Border3DSide.All;
		private Border3DStyle border3DStyle = Border3DStyle.Sunken;
		private BorderStyle borderStyle = BorderStyle.Fixed3D;
		private ButtonBorderStyle borderSingle = ButtonBorderStyle.Solid;
		private Color m_borderColor = Color.Black;
		private BrushInfo bgBrush;
		private Brush backBrush;
		private ControlDrawing cd;
		private bool themesEnabled;
		private ThemedControlDrawing tcd;
		private bool themedBorder = true;
		private bool ignoreThemeBackground = false;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);


		/// <summary>
		/// Occurs when border style is changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "Occurs when border style is changed." )]
		public event EventHandler BorderStyleChanged;

		/// <summary>
		/// Occurs when border's 3D style is changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "Occurs when border's 3D style is changed." )]
		public event EventHandler Border3DStyleChanged;

		/// <summary>
		/// Occurs when border's 2D style is changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "Occurs when border's 2D style is changed." )]
		public event EventHandler BorderSingleChanged;

		/// <summary>
		/// Occurs when border color is changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "Occurs when border color is changed." )]
		public event EventHandler BorderColorChanged;

		/// <summary>
		/// Occurs when gradient background is changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "Occurs when gradient background is changed." )]
		public event EventHandler GradientBackgroundChanged;

		/// <summary>
		/// Occurs when vertical gradient is changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "Occurs when vertical gradient is changed." )]
		public event EventHandler VerticalGradientChanged;

		/// <summary>
		/// Occurs when gradient colors are changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "Occurs when gradient colors are changed." )]
		public event EventHandler GradientColorsChanged;

		/// <summary>
		/// Occurs when border sides are changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "Occurs when border sides are changed." )]
		public event EventHandler BorderSidesChanged;

		/// <summary>
		/// Occurs when <see cref="ThemesEnabled"/> property is changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "Occurs when ThemesEnabled property is changed." )]
		public event EventHandler ThemeChanged;

		private IntPtr cachedRgn = IntPtr.Zero;
		protected override void WndProc( ref Message m )
		{
			if( this.cachedRgn != IntPtr.Zero )
			{
				NativeMethods.DeleteObject( this.cachedRgn );
				this.cachedRgn = IntPtr.Zero;
			}
			if( m.Msg == 0x031A/*WM_THEMECHANGED*/)
			{
				this.InvalidateWindow();
			}
			if( m.Msg ==	Syncfusion.Runtime.InteropServices.NativeMethods.WM_NCPAINT )
			{
				this.cachedRgn = DrawingUtils.NCPaintHelper( this, this, ref m );
			}

			if( m.Msg == NativeMethods.WM_HSCROLL || m.Msg == NativeMethods.WM_VSCROLL )
			{
				Invalidate();
			}

			base.WndProc( ref m );
		}

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

			int w = this.borderStyle==BorderStyle.Fixed3D?2:1;
			if( ThemesEnabled && XPThemes.IsThemedOS &&  XPThemes.IsThemeActive &&XPThemes.IsAppThemed && this.themedBorder ) w = 1;
			if( borderStyle==BorderStyle.None ) w=0;

			// The borders as 4 rectangles.
			Rectangle[] clipRects = new Rectangle[]
						{
							new Rectangle(bounds.Location,new Size(w,bounds.Height)),
							new Rectangle(bounds.Location,new Size(bounds.Width,w)),
							new Rectangle(bounds.Width-w,bounds.Y,w,bounds.Height),
							new Rectangle(bounds.X,bounds.Height-w,bounds.Width,w)
						};

			if( ThemesEnabled && XPThemes.IsThemedOS &&  XPThemes.IsThemeActive &&XPThemes.IsAppThemed && this.themedBorder )
			{
				for( int i=0; i<4; i++ )
				{
					tcd.DrawThemeBackground( g, 1, 1, bounds, clipRects[i] );
				}
			}
			else
			{
				// Fill the border-rectangles with the bg brush, since some of the 
				// 3d border types are only 1 pixel wide.
				for( int i=0; i<4; i++ )
				{
					g.FillRectangle( backBrush, clipRects[i] );
				}

				if( this.BorderSides != Border3DSide.All )
				{
					if( this.BorderSides != Border3DSide.Middle )
					{
						cd.DrawBorder( g, bounds, this.borderStyle, this.border3DStyle, this.borderSingle, this.BorderColorInternal, this.borderSides );
					}
				}
				else
				{
					if( !DrawSingleBorder( g, bounds ) )
					{
						cd.DrawBorder( g, bounds, this.borderStyle, this.border3DStyle, this.borderSingle, this.BorderColorInternal );
					}
				}
			}

			// return a region excluding where you just drew.
			return NativeMethods.CreateRectRgn( windowRectInScreen.Left+w, windowRectInScreen.Top+w, windowRectInScreen.Right-w, windowRectInScreen.Bottom-w );
		}
        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            this.Update();
        }
		bool DrawSingleBorder( Graphics g, Rectangle bounds )
		{
			if( this.borderStyle == BorderStyle.FixedSingle )
			{
				switch( this.borderSingle )
				{

					case ButtonBorderStyle.Inset:
					{
						using( Pen pDark = new Pen( ControlPaint.Dark( this.BorderColorInternal, 1.0f ) ) )
						{
							g.DrawLine( pDark, bounds.X, bounds.Y, bounds.Right - 1, bounds.Y );
							g.DrawLine( pDark, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1 );
						}
						using( Pen pLight = new Pen( ControlPaint.Light( this.BorderColorInternal, 1.0f ) ) )
						{
							g.DrawLine( pLight, bounds.X, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1 );
							g.DrawLine( pLight, bounds.Right - 1, bounds.Y, bounds.Right - 1, bounds.Bottom - 1 );
						}
					}
					return true;
					case ButtonBorderStyle.Outset:
					{
						using( Pen pLight = new Pen( ControlPaint.Light( this.BorderColorInternal, 1.0f ) ) )
						{
							g.DrawLine( pLight, bounds.X, bounds.Y, bounds.Right - 1, bounds.Y );
							g.DrawLine( pLight, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1 );
						}
						using( Pen pDark = new Pen( ControlPaint.Dark( this.BorderColorInternal, 1.0f ) ) )
						{
							g.DrawLine( pDark, bounds.X, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1 );
							g.DrawLine( pDark, bounds.Right - 1, bounds.Y, bounds.Right - 1, bounds.Bottom - 1 );
						}
					}
					return true;
				}
			}
			return false;
		}

		protected virtual void OnThemeChanged( EventArgs e )
		{
			if( this.ThemeChanged != null )
			{
				this.ThemeChanged( this, e );
			}
		}
		protected virtual void OnBorderStyleChanged()
		{
			if( BorderStyleChanged!=null ) { BorderStyleChanged( this, EventArgs.Empty ); }
		}
		protected virtual void OnBorder3DStyleChanged()
		{
			if( Border3DStyleChanged!=null ) { Border3DStyleChanged( this, EventArgs.Empty ); }
		}
		protected virtual void OnBorderSingleChanged()
		{
			if( BorderSingleChanged!=null ) { BorderSingleChanged( this, EventArgs.Empty ); }
		}
		protected virtual void OnBorderColorChanged()
		{
			if( BorderColorChanged!=null ) { BorderColorChanged( this, EventArgs.Empty ); }
		}
		protected virtual void OnGradientBackgroundChanged()
		{
			if( GradientBackgroundChanged!=null ) { GradientBackgroundChanged( this, EventArgs.Empty ); }
		}
		protected virtual void OnVerticalGradientChanged()
		{
			if( VerticalGradientChanged!=null ) { VerticalGradientChanged( this, EventArgs.Empty ); }
		}
		protected virtual void OnGradientColorsChanged()
		{
			if( GradientColorsChanged!=null ) { GradientColorsChanged( this, EventArgs.Empty ); }
		}
		protected virtual void OnBorderSidesChanged()
		{
			if( BorderSidesChanged!=null ) { BorderSidesChanged( this, EventArgs.Empty ); }
		}

		/// <summary>
		/// Indicates whether the control will ignore the theme's background color and draw the backcolor instead.
		/// </summary>
		[Description( "Indicates if the control will ignore the theme's background color and draw the backcolor instead." )]
		[Category( "Appearance" )]
		[DefaultValue( false )]
		public bool IgnoreThemeBackground
		{
			get { return ignoreThemeBackground; }
			set
			{
				if( ignoreThemeBackground!=value )
				{
					ignoreThemeBackground = value;
					Invalidate();
				}
			}
		}

		[Documentation.DocumentationExclude()]
		protected bool ThemedBorder
		{
			get { return themedBorder; }
			set { this.themedBorder = value; }
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cparams;
				BorderStyle border;

				cparams = base.CreateParams;
				cparams.ExStyle = cparams.ExStyle | 65536;
				cparams.ExStyle = cparams.ExStyle & -513;
				cparams.Style = cparams.Style & -8388609;
				border = this.borderStyle;
				if( border == BorderStyle.Fixed3D )
					if( ThemesEnabled && XPThemes.IsThemedOS &&  XPThemes.IsThemeActive &&XPThemes.IsAppThemed && this.themedBorder )
						border = BorderStyle.FixedSingle;
				switch( border )
				{
					case BorderStyle.Fixed3D:
					cparams.ExStyle = cparams.ExStyle | 512;
					break;
					case BorderStyle.FixedSingle:
					cparams.Style = cparams.Style | 8388608;
					break;
				}
				cparams.Style = cparams.Style |(int)ControlStyles.AllPaintingInWmPaint | 	(int)ControlStyles.UserPaint| (int)WhidbeyCompatibleControlStyles.DoubleBuffer;
				return cparams;
			}

		}


		/// <summary>
		/// Indicates whether the control is themed.
		/// </summary>
		[Description( "Indicates if the control is themed." )]
		[Category( "Appearance" )]
		[DefaultValue( false )]
		public bool ThemesEnabled
		{
			get { return themesEnabled; }
			set
			{
				if( themesEnabled!=value )
				{
					themesEnabled = value;
					this.UpdateStyles();
					InvalidateWindow();
					this.OnThemeChanged( EventArgs.Empty );
				}
			}
		}
		//
		//		public override Rectangle DisplayRectangle
		//		{
		//			get
		//			{
		//				int borderWidth = 0;
		//				
		//				switch(borderStyle)
		//				{
		//					case BorderStyle.FixedSingle: borderWidth = 1;break;
		//					case BorderStyle.Fixed3D: borderWidth = 2;break;
		//				}
		//
		//				if(XPThemes.IsThemedOS &&  XPThemes.IsThemeActive &&XPThemes.IsAppThemed && ThemesEnabled)
		//					borderWidth++;
		//				
		//				Rectangle rc = new Rectangle(0,0,Width,Height);
		//				if((borderSides&Border3DSide.Left) == Border3DSide.Left)
		//				{
		//					rc.X+=borderWidth;
		//					rc.Width-=borderWidth;
		//				}
		//				if((borderSides&Border3DSide.Top) == Border3DSide.Top)
		//				{
		//					rc.Y+=borderWidth;
		//					rc.Height-=borderWidth;
		//				}
		//				if((borderSides&Border3DSide.Right) == Border3DSide.Right) rc.Width-=borderWidth;
		//				if((borderSides&Border3DSide.Bottom) == Border3DSide.Bottom) rc.Height-=borderWidth;
		//
		//                return rc;
		//			}
		//		}

		/// <summary>
		/// Gets or sets the border sides of the panel.
		/// </summary>
		[Description( "Indicates the border sides of the panel." )]
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
					this.OnBorderSidesChanged();
					RedrawBorders();
					InvalidateWindow();
				}
			}
		}
		/// <summary>
		/// The background color, gradient and other styles can be set through 
		/// this property.
		/// </summary>
		/// <remarks>
		/// The GradientPanel control provides this property to enable specialized
		/// custom gradient backgrounds.
		/// </remarks>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Category( "Appearance" ),
		Description( "Lets you set the background color, gradient, etc." )
		]
		public BrushInfo BackgroundColor
		{
			get
			{
				return this.bgBrush;
			}

			set
			{
				if( this.bgBrush != value )
				{
					this.bgBrush = value;
					this.Invalidate();
				}
			}
		}
		private void ResetBackgroundColor()
		{
			this.BackgroundColor = BrushInfo.Empty;
		}
		private bool ShouldSerializeBackgroundColor()
		{
			if( this.bgBrush == BrushInfo.Empty )
				return false;
			else
				return true;
		}
		/// <summary>
		/// Gets or sets the color array that defines the gradient.
		/// </summary>
		/// <remarks>
		/// This property will be removed in the future. Please use the BackgroundColor property instead.
		/// </remarks>
		[Browsable( false ),
		Obsolete( "This property will be removed in the future. Please use the BackgroundColor property instead." ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public Color[] GradientColors
		{
			get { return (Color[])( this.BackgroundColor.GradientColors.ToArray( typeof( Color ) ) ); }
			set
			{
				BrushInfo bi = this.BackgroundColor;

				if( bi.Style == BrushStyle.Gradient )
				{
					this.BackgroundColor = new BrushInfo( bi.GradientStyle, new BrushInfoColorArrayList( value ) );
				}
				else if( bi.Style == BrushStyle.Pattern )
				{
					this.BackgroundColor = new BrushInfo( bi.PatternStyle, new BrushInfoColorArrayList( value ) );
				}
				else if( value.Length > 0 )
				{
					this.BackgroundColor = new BrushInfo( GradientStyle.Vertical, new BrushInfoColorArrayList( value ) );
				}
			}
		}
		/// <summary>
		/// Indicates whether the gradient is vertical.
		/// </summary>
		/// <remarks>
		/// This property will be removed in the future. Please use the BackgroundColor property instead.
		/// </remarks>
		[Browsable( false ),
		Obsolete( "This property will be removed in the future. Please use the BackgroundColor property instead." ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool VerticalGradient
		{
			get { return this.IsVerticalGradient; }
			set
			{
				if( value && this.IsVerticalGradient
					|| !value && !this.IsVerticalGradient )
					return;

				BrushInfo bi = this.BackgroundColor;
				this.BackgroundColor = new BrushInfo( GradientStyle.Vertical, bi.GradientColors );
			}
		}
		/// <summary>
		/// Indicates whether the background will be drawn with the gradient.
		/// </summary>
		/// <remarks>
		/// This property will be removed in the future. Please use the BackgroundColor property instead.
		/// </remarks>
		[Browsable( false ),
		Obsolete( "This property will be removed in the future. Please use the BackgroundColor property instead." ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool GradientBackground
		{
			get { return !this.BackgroundColor.IsEmpty; }
			set
			{
				if( !value )
					this.BackgroundColor = BrushInfo.Empty;

				// if true then the other settings will update the BackgroundColor.
			}
		}
		/// <summary>
		/// Gets or sets the color of the 2D border.
		/// </summary>
		[Description( "Indicates the color of the 2D border." )]
		[Category( "Appearance" )]
        [DefaultValue(typeof(Color), "Black")]
		public virtual Color BorderColor
		{
			get 
            {
                return m_borderColor; 
            }
			set
			{
                if (m_borderColor != value)
				{
                    m_borderColor = value;
					
                    this.OnBorderColorChanged();
					
                    RedrawBorders();
				}
			}
		}
        /// <summary>
        /// 
        /// </summary>
        protected virtual Color BorderColorInternal
        {
            get
            {
                return m_borderColor;
            }
        }

		protected virtual void RedrawBorders()
		{
			if( this.IsHandleCreated )
			{
				NativeMethods.SendMessage( this.Handle, NativeMethods.WM_NCPAINT, new IntPtr( 1 ), IntPtr.Zero );
			}
		}

		/// <summary>
		/// Gets or sets the 2D border style.
		/// </summary>
		[Description( "Indicates the 2D border style." )]
		[Category( "Appearance" )]
		[DefaultValue( ButtonBorderStyle.Solid )]
		public ButtonBorderStyle BorderSingle
		{
			get { return borderSingle; }
			set
			{
				if( borderSingle!=value )
				{
					borderSingle = value;
					this.OnBorderSingleChanged();
					RedrawBorders();
					InvalidateWindow();
				}
			}
		}

		/// <summary>
		/// Gets or sets the border style of the panel.
		/// </summary>
		[DefaultValue( BorderStyle.Fixed3D )]
		public new BorderStyle BorderStyle
		{
			get { return borderStyle; }
			set
			{
				base.BorderStyle = BorderStyle.None;
				if( borderStyle !=value )
				{
					BorderStyle baseValue = value;
					// The text box doesn't give you a 1 pixel border for FixedSingle,
					// so specifying 3D border and then drawing a single border manually.
					if( value == BorderStyle.FixedSingle )
						baseValue = BorderStyle.Fixed3D;
					base.BorderStyle = baseValue;

					borderStyle = value;
					this.UpdateStyles();
					InvalidateWindow();
					this.OnBorderStyleChanged();
				}
			}
		}
		/// <summary>
		/// Gets or sets the style of the 3D border.
		/// </summary>
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
					this.OnBorder3DStyleChanged();
					RedrawBorders();
					InvalidateWindow();
				}
			}
		}
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GradientPanel()
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(GradientPanel));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
			SetStyle( ControlStyles.AllPaintingInWmPaint | 	ControlStyles.UserPaint|WhidbeyCompatibleControlStyles.DoubleBuffer, true );
			InitializeComponent();
			cd = new ControlDrawing();
			backBrush = new SolidBrush( BackColor );
			this.bgBrush = BrushInfo.Empty;
            CTRLSIZE = this.Size;
		}

		/// <summary> 
		/// Cleans up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
				{
					components.Dispose();
				}

				backBrush.Dispose();
               // backBrush = null;
				bgBrush = null;
				cd = null;

				this.BackColorChanged -= new EventHandler( GradientPanel_BackColorChanged );
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// GradientPanel
			// 
			this.Size = new System.Drawing.Size( 224, 80 );

			this.BackColorChanged += new EventHandler( GradientPanel_BackColorChanged );

		}
		#endregion
		//		private void RefreshBrush()
		//		{
		//			if(Height<=0 || Width<=0) return;
		//			if(gradientColors.Count==0)
		//			{
		//				this.gradientBrush = new SolidBrush(BackColor);
		//				return;
		//			}
		//			if(verticalGradient)
		//			{
		//					
		//				gradientBrush= new LinearGradientBrush(
		//					new Point(0, 0),
		//					new Point(0, Height),
		//					Color.Black,
		//					Color.Black);
		//			}
		//			else
		//			{
		//				gradientBrush= new LinearGradientBrush(
		//					new Point(0, 0),
		//					new Point(Width,0),
		//					Color.Black,
		//					Color.Black);
		//			}
		//			ColorBlend cb = new ColorBlend(gradientColors.Count);
		//			for(int i=0;i<gradientColors.Count;i++)
		//			{
		//				cb.Colors.SetValue(gradientColors[i],i);
		//				cb.Positions.SetValue((float)i/(gradientColors.Count-1),i);
		//			}
		//
		//			((LinearGradientBrush)gradientBrush).InterpolationColors = cb;
		//		}

		protected override void OnPaintBackground( PaintEventArgs pevent )
		{
			bool bgPainted = false;
			if( this.ThemesEnabled && XPThemes.IsThemedOS && XPThemes.IsThemeActive && !this.ignoreThemeBackground )
			{
				bgPainted = this.PaintChildrenBackground( pevent.Graphics, this, pevent.ClipRectangle );
			}

			if( !bgPainted )
				base.OnPaintBackground( pevent );
		}
		protected virtual void ThemedPaintBackground( System.Drawing.Graphics graphics, Rectangle rect, Rectangle clip )
		{
			tcd.DrawThemeBackground( graphics, 1, Enabled?1:4, rect, clip );
		}
		private bool PaintChildrenBackground( System.Drawing.Graphics graphics, System.Windows.Forms.Control control, Rectangle clipRect )
		{
			foreach( System.Windows.Forms.Control child in control.Controls )
			{
				System.Drawing.Rectangle childBounds = new System.Drawing.Rectangle( child.Location, child.Size );
				childBounds = child.Parent.RectangleToScreen( childBounds );
				childBounds = this.RectangleToClient( childBounds );

				if( childBounds.Contains( clipRect ) )
				{
					if( this.PaintChildrenBackground( graphics, child, clipRect ) )
					{
						return true;
					}

					Rectangle client = this.ClientRectangle;
					client = this.RectangleToScreen( client );
					client = child.RectangleToClient( client );

					clipRect = this.RectangleToScreen( clipRect );
					clipRect = child.RectangleToClient( clipRect );
					this.ThemedPaintBackground( graphics, client, clipRect );
					return true;
				}
			}

			return false;
		}
		private bool IsVerticalGradient
		{
			get
			{
				if( this.bgBrush != null
					&& this.bgBrush.Style == BrushStyle.Gradient
					&& this.bgBrush.GradientStyle == GradientStyle.Vertical )
					return true;
				else
					return false;
			}
		}
		private bool IsHorizontalGradient
		{
			get
			{
				if( this.bgBrush != null
					&& this.bgBrush.Style == BrushStyle.Gradient
					&& this.bgBrush.GradientStyle == GradientStyle.Horizontal )
					return true;
				else
					return false;
			}
		}
		protected override void OnPaint( System.Windows.Forms.PaintEventArgs e )
		{
			base.OnPaint( e );
			if( Height<=0 || Width<=0 ) return;

			if( !( ThemesEnabled && XPThemes.IsThemedOS &&  XPThemes.IsThemeActive &&XPThemes.IsAppThemed ) || IgnoreThemeBackground )
			{
				if( this.BackgroundColor != null )
				{
					if( this.BackgroundColor != BrushInfo.Empty )
					{
						BrushPaint.FillRectangle( e.Graphics, this.ClientRectangle, this.BackgroundColor );
					}
				}
			}
			else
			{
				if( Enabled )
				{
					//if(tcd != null)
					//{						
					//    tcd.DrawThemeBackground(e.Graphics,1,1,new Rectangle(-2,-2,Width+4,Height+4)); 
					//}

					using( Brush br = new SolidBrush( SystemColors.Control ) )
					{
						e.Graphics.FillRectangle( br, ClientRectangle );
					}
				}
				else
				{
					using( Brush br = new SolidBrush( ( this.BackColor == SystemColors.Window ) ? SystemColors.Control : BackColor ) )
					{
						e.Graphics.FillRectangle( br, ClientRectangle );
					}
				}
			}
			this.RaisePaintEvent( this, e );
		}

		private void GradientPanel_BackColorChanged( object sender, System.EventArgs e )
		{
			backBrush = new SolidBrush( BackColor );
			if (BackColor == Color.Transparent)
				DoubleBuffered = false;
			else
				DoubleBuffered = true;
			InvalidateWindow();
		}
		private void InvalidateWindow()
		{
			Invalidate();
		}

		protected override void OnSizeChanged( EventArgs e )
		{
			base.OnSizeChanged( e );
            if (!EnableTouchMode && CTRLSIZE != this.Size)
            {
                CTRLSIZE = this.Size;
            }
			this.InvalidateWindow();
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed)
			{
				tcd = new ThemedControlDrawing(ThemedControls.EDIT, this);
			}
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (this.tcd != null)
			{
				this.tcd.Dispose();
				this.tcd = null;
			}

			 base.OnHandleDestroyed(e);
		}
        #region For Touch

        bool isScaling = false;

        bool _touchMode = false;
        /// <summary>
        ///Gets or Sets the touch mode
        /// </summary>
        [DefaultValue(false)]
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
        ///Applies the scaling for controls
        /// </summary>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            foreach (Control ctrl in this.Controls)
            {
                PropertyInfo fi = ctrl.GetType().GetProperty("EnableTouchMode");//,
                if(fi != null)
                    fi.SetValue(ctrl, this.EnableTouchMode, null);
            }
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }
        /// <summary>
        ///Font changed
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
        #endregion
		#region ISupportInitialize Members

		/// <summary>
		/// Signals the object that initialization is starting.
		/// </summary>
		[Description( "Signals the object that initialization is starting." )]
		public virtual void BeginInit()
		{
		}

		/// <summary>
		/// Signals the object that initialization is complete.
		/// </summary>
		[Description( "Signals the object that initialization is complete." )]
		public virtual void EndInit()
		{
		}

		#endregion
	}
}

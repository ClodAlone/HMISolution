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
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms
{

	/// <summary>
	/// An interface for hosting <see cref="InternalButton"/> objects and
	/// receiving clicks from these buttons.
	/// </summary>
	interface IInternalButtonParent: IThemedControl
	{
		/// <summary>
		/// Occurs when the specified button is clicked or the mouse is pressed down on the button.
		/// </summary>
		/// <param name="button">The source of the event.</param>
		void OnClickedButton( InternalButton button );
	}

	/// <summary>
	/// Specifies the current state of the button.
	/// </summary>
	[
	Flags,
	]
	public enum InternalButtonState
	{
		/// <summary>
		/// Button is in normal state.
		/// </summary>
		Normal=0x00,
		/// <summary>
		/// Button is disabled.
		/// </summary>
		Disabled=0x01,
		/// <summary>
		/// Mouse is hovering over button.
		/// </summary>
		Hovered=0x02,
		/// <summary>
		/// Button is pushed.
		/// </summary>
		Pushed=0x04,
		/// <summary>
		/// Button is checked.
		/// </summary>
		Checked=0x08,
		/// <summary>
		/// Button is considered a drop target of a drag-and-drop operation.
		/// </summary>
		DragTarget=0x10,
		/// <summary>
		/// A mask for the drawing state of the button without behavioral options (without Checked, DropTarget).
		/// </summary>
		[Browsable( false )]
		Options=Checked|DragTarget
	}

	/// <internalonly/>
	[Syncfusion.Documentation.DocumentationExclude]
	public enum SpinButtonType
	{
		/// <internalonly/>
		None,
		/// <internalonly/>
		Up,
		/// <internalonly/>
		Down
	};

	/// <summary>
	/// InternalButton are buttons that are displayed inside a <see cref="InternalButtonBar"/>.
	/// </summary>
	[
	ToolboxItem( false ),
	]
	public class InternalButton: Component
	{
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		internal InternalButtonState state;

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		internal bool dirty;

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		internal object cookie;

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		internal object owner;

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		internal string tooltip = "";

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		internal int tooltipID = -1;

		private Size size;
		private Rectangle bounds = Rectangle.Empty;
		private bool drawButton = true;
		private bool ignorePaint = false;
		private bool repeatClick = true;
		private SpinButtonType spinButtonType = SpinButtonType.None;
		private ThemedPushButtonDrawing themedDrawing = null;
		/// <summary>
		/// Style of the control.
		/// </summary>
		private TabBarSplitterStyle m_style = TabBarSplitterStyle.Default;
		/// <summary>
		/// Color table for Office2007 visual style.
		/// </summary>
		private Office2007Colors m_office2007ColorTable = null;
		/// <summary>
		/// Specifies office 2007 color scheme.
		/// </summary>
		private Office2007Theme m_colorScheme = Office2007Theme.Blue;

		/// <overload>
		/// Initializes a new <see cref="InternalButton"/>.
		/// </overload>
		/// <summary>
		/// Initializes a new <see cref="InternalButton"/>.
		/// </summary>
		public InternalButton()
		{
		}

		/// <summary>
		/// Initializes a new <see cref="InternalButton"/> with a specified size.
		/// </summary>
		/// <param name="size">The initial size of the button.</param>
		public InternalButton( Size size )
		{
			this.size = size;
		}

		/// <summary>
		/// Initializes a new <see cref="InternalButton"/> with a cookie.
		/// </summary>
		/// <param name="cookie">The cookie for the button.</param>
		public InternalButton( object cookie )
			: this( null, cookie, "", new Size(), true )
		{
		}

		/// <summary>
		/// Initializes a new <see cref="InternalButton"/> with an owner and cookie.
		/// </summary>
		/// <param name="owner">The owner of the button.</param>
		/// <param name="cookie">The cookie for the button.</param>
		public InternalButton( object owner, object cookie )
			: this( owner, cookie, "", new Size(), true )
		{
		}

		/// <summary>
		/// Initializes a new <see cref="InternalButton"/> with a owner, cookie and ToolTip text.
		/// </summary>
		/// <param name="owner">The owner of the button.</param>
		/// <param name="cookie">The cookie for the button.</param>
		/// <param name="tooltip">The ToolTip text.</param>
		public InternalButton( object owner, object cookie, string tooltip )
			: this( owner, cookie, tooltip, new Size(), true )
		{
		}

		/// <summary>
		/// Initializes a new <see cref="InternalButton"/> with a owner, cookie, ToolTip text and size.
		/// </summary>
		/// <param name="owner">The owner of the button.</param>
		/// <param name="cookie">The cookie for the button.</param>
		/// <param name="tooltip">The ToolTip text.</param>
		/// <param name="size">The initial size of the button.</param>
		public InternalButton( object owner, object cookie, string tooltip, Size size )
			: this( owner, cookie, tooltip, new Size(), true )
		{
		}

		/// <summary>
		/// Initializes a new <see cref="InternalButton"/> with a owner, cookie, ToolTip text, size and a value that
		/// indicates whether this control should look like a button or if just a centered text should be drawn.
		/// </summary>
		/// <param name="owner">The owner of the button.</param>
		/// <param name="cookie">The cookie for the button.</param>
		/// <param name="tooltip">The ToolTip text.</param>
		/// <param name="size">The initial size of the button</param>
		/// <param name="drawButton">A value indicating if this should look like a button or if just a centered text should be drawn.</param>
		public InternalButton( object owner, object cookie, string tooltip, Size size, bool drawButton )
		{
			this.dirty = true;
			this.cookie = cookie;
			this.owner = owner;
			this.tooltip = tooltip;
			this.size = size;
			this.drawButton = drawButton;
			this.state = drawButton ? InternalButtonState.Normal : InternalButtonState.Disabled;

			if( XPThemes.IsThemedOS && this.Owner is Control )
			{
				this.themedDrawing = new ThemedPushButtonDrawing( (IComponent)this.Owner );
			}
		}

		/// <override/>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				ResetToolTip();

				this.cookie = null;
				this.owner = null;

				if( this.themedDrawing != null )
				{
					this.themedDrawing.Dispose();
					this.themedDrawing = null;
				}

				themedDrawing = null;
			}
			base.Dispose( disposing );
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ThemedPushButtonDrawing ThemedDrawing
		{
			get { return this.themedDrawing; }
		}

		/// <summary>
		/// Returns the preferred size of button.
		/// </summary>
		/// <param name="maxSize">The maximum allowed size.</param>
		/// <returns>The preferred size based on text, button type.</returns>
		public virtual Size GetPreferredSize( Size maxSize )
		{
			return maxSize;
		}

		/// <summary>
		/// Recalculates the best size for the button and resizes it.
		/// </summary>
		public virtual void AdjustSize()
		{
			if( this.ignorePaint )
				Size = new Size();
			else
			{
				Control parent = this.Owner as Control;
				if( parent != null && this.cookie is String )
				{
					String text = (String)this.cookie + "g";
					if( text.Length > 1 )
					{
						Graphics g = parent.CreateGraphics();
						Font font = parent.Font;
						Size textSize = g.MeasureString( text, font ).ToSize();
						g.Dispose();

						Size = textSize;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the visual style of the control.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Specifies the style with which control will appear." )]
		[DefaultValue( TabBarSplitterStyle.Default )]
		public TabBarSplitterStyle Style
		{
			get
			{
				return m_style;
			}
			set
			{
				if( m_style != value )
				{
					m_style = value;
					this.OnStyleChanged();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnStyleChanged()
		{
			m_office2007ColorTable = Office2007Colors.GetColorTable( m_colorScheme );
			if( this.Owner is Control )
			{
				( this.Owner as Control ).Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets office 2007 color scheme.
		/// </summary>
		[Description( "Gets or sets office 2007 color scheme." )]
		[Category( "Appearance" )]
		[DefaultValue( Office2007Theme.Blue )]
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
					this.OnStyleChanged();
				}
			}
		}

		/// <summary>
		/// Gets color table for Office2007 visual style.
		/// </summary>
		internal Office2007Colors Office2007ColorTable
		{
			get
			{
				Office2007Colors colorTable = ( m_office2007ColorTable == null ) ? 
					Office2007Colors.Default :	m_office2007ColorTable;

				return colorTable;
			}
		}

		/// <summary>
		/// Gets / sets the owner of this button.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public object Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				if( this.owner != value )
				{
					this.owner = value;
					OnOwnerChanged();
				}
			}
		}

		/// <summary>
		/// Some external state (e.g. button type or command id).
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public object Cookie
		{
			get
			{
				return this.cookie;
			}
			set
			{
				if( this.cookie != value )
				{
					this.dirty = true;
					this.cookie = value;
					OnCookieChanged();
				}
			}
		}

		/// <summary>
		/// Gets / sets the ToolTip text for this button.
		/// </summary>
		public string ToolTip
		{
			get
			{
				return this.tooltip;
			}
			set
			{
				this.tooltip = value;
				if( this.tooltip != value )
				{
					this.dirty = true;
					this.tooltip = value;
					OnToolTipChanged();
				}
			}
		}

		/// <summary>
		/// Gets / sets the bounds of this button.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public Rectangle Bounds
		{
			get
			{
				if( this.ignorePaint )
					return Rectangle.Empty;
				else
					return this.bounds;
			}
			set
			{
				Rectangle r = value;
				if( this.spinButtonType != SpinButtonType.None )
				{
					value.Height = value.Height/2;
					if( this.spinButtonType == SpinButtonType.Down )
						value.Y = value.Bottom;
				}

				if( this.bounds != value )
				{
					this.bounds = value;
					this.dirty = true;
				}
			}
		}

		/// <summary>
		/// Gets / sets the size of this button.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public Size Size
		{
			get
			{
				return this.size;
			}
			set
			{
				if( this.size != value )
				{
					this.size = value;
					this.Dirty = true;
				}
			}
		}

		/// <internalonly/>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false ),
		EditorBrowsable( EditorBrowsableState.Never ),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public SpinButtonType SpinButton
		{
			get
			{
				return this.spinButtonType;
			}
			set
			{
				this.spinButtonType = value;
			}
		}

		/// <summary>
		/// Indicates whether button is hidden.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public bool HideButton
		{
			get
			{
				return this.ignorePaint;
			}
			set
			{
				this.ignorePaint = value;
			}
		}

		/// <summary>
		/// Indicates whether this button supports repeated clicking when the user holds down the mouse button.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public bool RepeatClick
		{
			get
			{
				return this.repeatClick;
			}
			set
			{
				this.repeatClick = value;
			}
		}

		/// <summary>
		/// Indicates whether it is a Dirty flag.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public bool Dirty
		{
			get
			{
				return this.dirty;
			}
			set
			{
				this.dirty = value;
			}
		}

		/// <summary>
		/// Indicates the Enabled state.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public bool Enabled
		{
			get
			{
				return ( ( this.state & InternalButtonState.Disabled ) == 0 );
			}
			set
			{
				// Note that setting the button enabled will negate the Disabled flag.
				if( value != this.Enabled )
				{
					if( value )
						this.state &= ~InternalButtonState.Disabled;
					else
					{
						// Checked is the only other valid flag
						this.state = InternalButtonState.Disabled
							| ( this.state & InternalButtonState.Options );
					}
					this.dirty = true;
				}
			}
		}

		/// <summary>
		/// Indicates the Hovered state.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public bool Hovered
		{
			get
			{
				return ( ( this.state & InternalButtonState.Hovered ) != 0 );
			}
			set
			{
				if( this.Enabled && value != this.Hovered )
				{
					if( value )
						// Checked is the only other valid flag.
						this.state = InternalButtonState.Hovered
							| ( this.state & InternalButtonState.Options );
					else
						this.state &= ~InternalButtonState.Hovered;
					this.dirty = true;
					OnStateChanged();
				}
			}
		}

		// Pushed state.

		/// <summary>
		/// Indicates the Pushed state.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public bool Pushed
		{
			get
			{
				return ( ( this.state & InternalButtonState.Pushed ) != 0 );
			}
			set
			{
				if( this.Enabled && value != this.Pushed )
				{
					if( value )
						// Checked is the only other valid flag.
						this.state = InternalButtonState.Pushed
							| ( this.state & InternalButtonState.Options );
					else
						this.state &= ~InternalButtonState.Pushed;
					this.dirty = true;
					OnStateChanged();
				}
			}
		}

		/// <summary>
		/// Indicates the Checked state.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public bool Checked
		{
			get
			{
				return ( ( this.state & InternalButtonState.Checked ) != 0 );
			}
			set
			{
				if( this.Enabled && value != this.Checked )
				{
					if( value )
						this.state |= InternalButtonState.Checked;
					else
						this.state &= ~InternalButtonState.Checked;
					this.dirty = true;
					OnStateChanged();
				}
			}
		}

		/// <summary>
		/// Indicates the DragTarget state.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public bool DragTarget
		{
			get
			{
				return ( ( this.state & InternalButtonState.DragTarget ) != 0 );
			}
			set
			{
				if( this.Enabled && value != this.DragTarget )
				{
					if( value )
						this.state |= InternalButtonState.DragTarget;
					else
						this.state &= ~InternalButtonState.DragTarget;
					this.dirty = true;
					OnStateChanged();
				}
			}
		}

		/// <summary>
		/// The <see cref="T:System.Windows.Forms.ButtonState"/> for this button.
		/// </summary>
		/// <param name="flatLook">True if flat button; False if normal button.</param>
		/// <returns>The <see cref="T:System.Windows.Forms.ButtonState"/>.</returns>
		public System.Windows.Forms.ButtonState GetWinFormButtonState( bool flatLook )
		{
			System.Windows.Forms.ButtonState state = System.Windows.Forms.ButtonState.Normal;
			if( !this.Enabled )
				state |= System.Windows.Forms.ButtonState.Inactive;
			if( this.Pushed )
				state |= System.Windows.Forms.ButtonState.Pushed;
			if( this.Checked )
				state |= System.Windows.Forms.ButtonState.Checked;

			if( flatLook )
			{
				if( !this.Hovered )
					state |= System.Windows.Forms.ButtonState.Flat;
			}

			return state;
		}

		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		internal ControlToolTip TooltipProvider
		{
			get
			{
				IControlToolTipProvider target = this.Owner as IControlToolTipProvider;
				if( target != null )
					return target.GetControlToolTip();
				return null;
			}
		}

		/// <summary>
		/// Reinitializes and hides the ToolTip.
		/// </summary>
		public void ResetToolTip()
		{
			ControlToolTip toolTipProvider = this.TooltipProvider;
			if( toolTipProvider != null )
				toolTipProvider.InitToolTip( ref this.tooltipID, "", Rectangle.Empty );
		}

		/// <summary>
		/// Initializes ToolTip area at the specified bounds.
		/// </summary>
		/// <param name="bounds"></param>
		public void InitToolTip( Rectangle bounds )
		{
			ControlToolTip toolTipProvider = this.TooltipProvider;
			if( toolTipProvider != null )
				toolTipProvider.InitToolTip( ref this.tooltipID, this.ToolTip, bounds );
		}


		/// <summary>
		/// Calculates coordinates for a centered rectangle.
		/// </summary>
		/// <param name="rect">The existing bounds.</param>
		/// <param name="size">The size of the rectangle to be centered.</param>
		/// <returns>A rectangle inside the specified bounds.</returns>
		static public Rectangle CenterInRect( Rectangle rect, Size size )
		{
			int dx = 0;
			if( size.Width < rect.Width )
				dx = rect.Width-size.Width;

			int dy = 0;
			if( size.Height < rect.Height )
				dy = rect.Height-size.Height;

			return new Rectangle( rect.Left+dx/2, rect.Top+dy/2,
				Math.Min( size.Width, rect.Width ), Math.Min( size.Height, rect.Height ) );
		}

		/// <summary>
		/// Paints the button.
		/// </summary>
		/// <param name="g">A Graphics object used to draw the bitmap.</param>
		/// <param name="bounds">A Rectangle which contains the boundary data of the rectangle.</param>
		/// <param name="flatLook">True if flat looking button; False if normal.</param>
		/// <param name="barArea">A Rectangle which contains the boundary data of the parent bar rectangle.</param>
		public virtual void Paint( System.Drawing.Graphics g, System.Drawing.Rectangle bounds, bool flatLook, System.Drawing.Rectangle barArea )
		{
			Debug.Assert( g != null, "Must pass valid graphics" );

			if( this.ignorePaint )
				return;

			try
			{
				InitToolTip( Rectangle.Intersect( bounds, barArea ) );

				// Draw a blank button.
				if( g.ClipBounds.IntersectsWith( bounds ) )
				{
					if( this.drawButton )
					{
						ButtonState state = GetWinFormButtonState( flatLook );
						IInternalButtonParent parent = this.Owner as IInternalButtonParent;
						if( parent != null && XPThemes.IsThemedOS && XPThemes.IsThemeActive && parent.ThemesEnabled )
						{
							this.themedDrawing.DrawPushButton( g, bounds, state );
						}
						else
						{
							System.Windows.Forms.ControlPaint.DrawButton( g, bounds, state );
						}
					}
					else
					{
						Rectangle r = Rectangle.Intersect( bounds, barArea );
						Control parent = this.Owner as Control;
						if( parent != null )
						{
							Brush fillBrush = new SolidBrush( parent.BackColor );
							g.FillRectangle( fillBrush, r );
							fillBrush.Dispose();

							if( this.cookie is String )
							{
								String text = (String)this.cookie;
								Brush textBrush = new SolidBrush( parent.Enabled ? parent.ForeColor : SystemColors.GrayText );
								Font font = parent.Font;
								Size textSize = g.MeasureString( text, font ).ToSize();
								Rectangle textArea = CenterInRect( r, textSize );
								g.DrawString( text, font, textBrush, new Point( textArea.Left, textArea.Top ) );
								textBrush.Dispose();
							}
						}

					}
				}
			}
			finally
			{
				this.dirty = false;
			}
		}

		/// <summary>
		/// Called when <see cref="Owner"/> is changed.
		/// </summary>
		public virtual void OnOwnerChanged()
		{
		}

		/// <summary>
		/// Called when <see cref="Cookie"/> is changed.
		/// </summary>
		public virtual void OnCookieChanged()
		{
		}

		/// <summary>
		/// Called when state is changed.
		/// </summary>
		public virtual void OnStateChanged()
		{
		}

		/// <summary>
		/// Called when <see cref="ToolTip"/> is changed.
		/// </summary>
		public virtual void OnToolTipChanged()
		{
		}
	}
}

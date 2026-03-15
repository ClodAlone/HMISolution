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
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;

using Syncfusion.ComponentModel;
using Syncfusion.Documentation;
using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
#endregion

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Implement this interface in a <see cref="Control"/> derived class to start supporting themes.
	/// </summary>
	public interface IThemedControl
	{
		/// <summary>
		/// Indicates whether themes are enabled.
		/// </summary>
		bool ThemesEnabled { get; set; }
	}

	/// <summary>
	/// A <see cref="Control"/> implementing the <see cref="IThemedControl"/> interface.
	/// </summary>
	[ToolboxItem( false )]
	public class ThemedControl: Control, IThemedControl
	{
		private bool themesEnabled = false;
		/// <summary>
		/// Fired when the ThemesEnabled property changes.
		/// </summary>
		[Description( "Fired when the ThemesEnabled property changes." )]
		public event EventHandler ThemeChanged;

		/// <override/>
		[SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode=true )]
		protected override void WndProc( ref Message m )
		{
			if( m.Msg == 0x031A /*WM_THEMECHANGED*/ )
			{
				this.Invalidate();
			}
			base.WndProc( ref m );
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
				this.ThemeChanged( this, e );
			}
		}

		/// <summary>
		/// Indicates whether themes are enabled for this control.
		/// </summary>
		[Description( "Indicates whether themes are enabled for this control." )]
		public virtual bool ThemesEnabled
		{
			get
			{
				return this.themesEnabled;
			}
			set
			{
				if( this.themesEnabled != value )
				{
					this.themesEnabled = value;
					this.OnThemeChanged( EventArgs.Empty );
				}
			}
		}
	}

	/// <summary>
	/// Manages the theme handle given a control and exposes some basic themed Drawing methods.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="Control"/> bound to this class can either implement the 
	/// <see cref="IThemedControl"/> interface or pass on the WM_THEMECHANGED message 
	/// to this class with a call to the <see cref="RefreshThemeHandle"/> method.</para>
	/// </remarks>
	public class ThemedControlDrawing: Disposable
	{
		private IntPtr htheme = IntPtr.Zero;
		private string classList = String.Empty;
		private static int dpiYScreen = 96;
		private IComponent m_owner;
		private bool m_bIsMirrored = false;

		/// <summary>
		/// Creates a new instance of the <see cref="ThemedControlDrawing"/> class.
		/// </summary>
		/// <param name="classList">Pointer to a string that contains a semicolon-separated list of classes, as expected
		/// in the OpenThemeData API.</param>
		/// <remarks>
		/// <para>
		/// The <see cref="Control"/> bound to this class can either implement the 
		/// <see cref="IThemedControl"/> interface or pass on the WM_THEMECHANGED message 
		/// to this class with a call to the <see cref="RefreshThemeHandle"/> method.
		/// This is necessary in order that this class can refresh its handles when themes
		/// settings are updated.
		/// </para>
		/// <para>You can get the part and state ids required for the DrawXXX methods from the tmschema.h file (that comes with
		/// Platform SDK) or refer to the undocumented and incomplete ThemeParts and ThemeStates classes 
		/// in our shared library (in the ThemeDefines.cs file).</para>
		/// </remarks>
		public ThemedControlDrawing( string classList )
		{
			this.classList = classList;

			this.OpenThemeData();
			XPThemes.RegisterControlDrawing( this );

			// Get screen dpi information.
			IntPtr hdc = NativeMethods.GetDC( IntPtr.Zero );
			dpiYScreen = NativeMethods.GetDeviceCaps( hdc, 88 );
			NativeMethods.ReleaseDC( IntPtr.Zero, hdc );
		}

		/// <summary>
		/// Creates a new instance of the <see cref="ThemedControlDrawing"/> class.
		/// </summary>
		/// <param name="classList">Pointer to a string that contains a semicolon-separated list of classes, as expected
		/// in the OpenThemeData API.</param>
		/// <param name="owner">Owning component.</param>
		/// <remarks>
		/// <para>
		/// The <see cref="Control"/> bound to this class can either implement the 
		/// <see cref="IThemedControl"/> interface or pass on the WM_THEMECHANGED message 
		/// to this class with a call to the <see cref="RefreshThemeHandle"/> method.
		/// This is necessary in order that this class can refresh its handles when themes
		/// settings are updated.
		/// </para>
		/// <para>You can get the part and state ids required for the DrawXXX methods from the tmschema.h file (that comes with
		/// Platform SDK) or refer to the undocumented and incomplete ThemeParts and ThemeStates classes 
		/// in our shared library (in the ThemeDefines.cs file).</para>
		/// <para>If owning component is not null, <see cref="ThemedControlDrawing"/> automatically unregisters itself from <see cref="XPThemes"/>
		/// on component disposing.</para>
		/// </remarks>
		public ThemedControlDrawing( string classList, IComponent owner ) :
			this( classList )
		{
			if( owner != null )
			{
				AdwiseOwner( owner );
			}
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.Dispose"/>.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			UnadwiseOwner();

			if( htheme != IntPtr.Zero )
			{
				NativeMethods.CloseThemeData( htheme );
				htheme = IntPtr.Zero;
			}

			base.Dispose( disposing );
		}

		/// <summary>
		/// Returns the current theme handle used to render the parts.
		/// </summary>
		public IntPtr HTheme
		{
			get
			{
				return this.htheme;
			}
		}

		public Color GetThemeSysColor( int colorId )
		{
			int color = (int)NativeMethods.GetThemeSysColor( this.HTheme, colorId );
			return Color.FromArgb(
			  NativeMethods.GetRValue( color ),
			  NativeMethods.GetGValue( color ),
			  NativeMethods.GetBValue( color ) );
		}

		public int GetThemeSysInt( int id )
		{
			int value = 0;
			NativeMethods.GetThemeSysInt( this.HTheme, id, ref value );
			return value;
		}

		public string GetThemeSysString( int id )
		{
			string value = new string( ' ', 256 );
			NativeMethods.GetThemeSysString( this.HTheme, id, ref value, 256 );
			return value;
		}

		public int GetThemeSysSize( int id )
		{
			int value = NativeMethods.GetThemeSysSize( this.HTheme, id );
			return value;
		}

		public bool GetThemeSysBool( int id )
		{
			bool value = NativeMethods.GetThemeSysBool( this.HTheme, id );
			return value;
		}

		#region THEME_HANDLE_LIFECYCLE
		/// <summary>
		/// Called to create a theme handle, given the specified control and classList.
		/// </summary>
		protected virtual void OpenThemeData()
		{
			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive )
			{
				//				Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo();
				this.htheme = NativeMethods.OpenThemeData( IntPtr.Zero, this.classList );
			}
		}

		/// <summary>
		/// Closes the currently open theme handle.
		/// </summary>
		protected virtual void CloseThemeData()
		{
			if( this.htheme != IntPtr.Zero )
			{
				NativeMethods.CloseThemeData( this.htheme );
				this.htheme = IntPtr.Zero;
			}
		}

		/// <summary>
		/// Closes the current theme handle.
		/// </summary>
		public void ResetThemeHandle()
		{
			this.CloseThemeData();
		}

		/// <summary>
		/// Closes the current theme handle and tries to open a new one.
		/// </summary>
		public void RefreshThemeHandle()
		{
			this.CloseThemeData();
			this.OpenThemeData();
		}
		#endregion THEME_HANDLE_LIFECYCLE

		#region DRAWING
		/// <summary>
		/// Overloaded. Draws the specified theme background.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> object.</param>
		/// <param name="partID">An integer specifying the part.</param>
		/// <param name="stateID">An integer specifying the state.</param>
		/// <param name="rectangle">The background <see cref="Rectangle"/>.</param>
		/// <remarks>
		/// <para>See <see cref="ThemedControlDrawing"/> for information on how to get the part and state IDs.</para>
		/// <para>This method uses the VisibleClipRegion in the Graphics object to obtain the clip rect.</para>
		/// </remarks>
		public virtual void DrawThemeBackground( Graphics g, int partID, int stateID, Rectangle rectangle )
		{
			this.DrawThemeBackground( g, partID, stateID, rectangle, Rectangle.Ceiling( g.VisibleClipBounds ) );
		}

		/// <summary>
		/// Draws the specified theme background.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> object.</param>
		/// <param name="partID">An integer specifying the part.</param>
		/// <param name="stateID">An integer specifying the state.</param>
		/// <param name="rectangle">The background <see cref="Rectangle"/>.</param>
		/// <param name="clipRect">The clip rect to be used.</param>
		/// <remarks>
		/// <para>See <see cref="ThemedControlDrawing"/> for information on how to get the part and state IDs.</para>
		/// <para>Use this function to provide custom clip bounds.</para>
		/// </remarks>
		public virtual void DrawThemeBackground( Graphics g, int partID, int stateID, Rectangle rectangle, Rectangle clipRect )
		{
			bool bDrawMirrored = DrawMirrored;
			if( g.DpiY != dpiYScreen || bDrawMirrored )
			{
				using( CMirroredDrawer mdDrawer = new CMirroredDrawer( g, rectangle, bDrawMirrored ) )
				{
					Graphics gfxVirt = mdDrawer.VirtualGfx;
					Rectangle rectCanvas = mdDrawer.VirtualBounds;

					IntPtr hdc = gfxVirt.GetHdc();

					NativeMethods.RECT rect = new NativeMethods.RECT( rectCanvas );
					NativeMethods.RECT rectClip = new NativeMethods.RECT( rectCanvas );

					DrawThemeBackground( partID, stateID, hdc, ref rect, ref rectClip );

					gfxVirt.ReleaseHdc( hdc );
				}
			}
			else
			{
				IntPtr hdc = g.GetHdc();
				NativeMethods.RECT rect = new NativeMethods.RECT( rectangle );
				NativeMethods.RECT rectClip = new NativeMethods.RECT( clipRect );

				DrawThemeBackground( partID, stateID, hdc, ref rect, ref rectClip );

				g.ReleaseHdc( hdc );
			}
		}

		private void DrawThemeBackground( int partID, int stateID, IntPtr hdc, ref NativeMethods.RECT rect, ref NativeMethods.RECT rectClip )
		{
			if( partID == ThemeParts.TABP_BODY && XPThemes.IsSilverThemeOn )
			{
				DrawSilverTabBody( partID, stateID, hdc, ref rect, ref rectClip );
			}
			else
			{
				NativeMethods.DrawThemeBackground( this.HTheme, hdc, partID, stateID, ref rect, ref rectClip );
			}
		}

		private void DrawSilverTabBody( int partID, int stateID, IntPtr hdc, ref NativeMethods.RECT rect, ref NativeMethods.RECT rectClip )
		{
			Rectangle drawRect = rect;
			drawRect.Intersect( rectClip );

			if( drawRect.Width > 0 && drawRect.Height > 0 )
			{
				const int patternWidth = 10;	// Width of silver bitmap pattern.
				NativeMethods.RECT part = new NativeMethods.RECT( drawRect );

				part.right = part.left + patternWidth;

				while( part.left <= drawRect.Right )
				{
					NativeMethods.DrawThemeBackground( this.HTheme, hdc, partID, stateID, ref part, ref rectClip );

					part.left += patternWidth;
					part.right += patternWidth;
				}
			}
		}

		/// <summary>
		/// Overloaded. Draws the specified theme text.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> object.</param>
		/// <param name="partID">An integer specifying the part.</param>
		/// <param name="stateID">An integer specifying the state.</param>
		/// <param name="text">The text to be drawn.</param>
		/// <param name="bounds">The layout bounds within which to draw.</param>
		/// <param name="formatFlags1">Refers to the DrawThemeText function in the Windows API.</param>
		/// <param name="formatFlags2">Refers to the DrawThemeText function in the Windows API.</param>
		[CLSCompliant( false )]
		public virtual void DrawThemeText( Graphics g, int partID, int stateID, String text, Rectangle bounds,
		  uint formatFlags1, uint formatFlags2 )
		{
			IntPtr dc = g.GetHdc();
            try
            {
                NativeMethods.RECT rect = new NativeMethods.RECT(bounds);

                NativeMethods.DrawThemeText(this.HTheme, dc, partID, stateID, text, text.Length,
                  formatFlags1, formatFlags2, ref rect);
            }
            finally
            {
                g.ReleaseHdc(dc);
            }
		}

		/// <summary>
		/// Draws the specified theme text.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> object.</param>
		/// <param name="partID">An integer specifying the part.</param>
		/// <param name="stateID">An integer specifying the state.</param>
		/// <param name="text">The text to be drawn.</param>
		/// <param name="bounds">The layout bounds within which to draw.</param>
		/// <param name="formatFlags1">Refers to the DrawThemeText function in the Windows API.</param>
		/// <param name="formatFlags2">Refers to the DrawThemeText function in the Windows API.</param>
		public void DrawThemeText( Graphics g, int partID, int stateID, String text, Rectangle bounds,
		  int formatFlags1, int formatFlags2 )
		{
			DrawThemeText( g, partID, stateID, text, bounds, (uint)formatFlags1, (uint)formatFlags2 );
		}

		/// <summary>
		/// Returns the size for the specified part.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> object.</param>
		/// <param name="partId">An integer specifying the part.</param>
		/// <param name="stateId">An integer specifying the state.</param>
		/// <param name="sizeType">The size type.</param>
		/// <returns>The requested size.</returns>
		public virtual Size GetPartSize( Graphics g, int partId, int stateId, int sizeType )
		{
			IntPtr hdc = g.GetHdc();
            NativeMethods.SIZE sz;
            try
            {                
                uint hr = NativeMethods.GetThemePartSize(this.HTheme, hdc, partId,
                  stateId, IntPtr.Zero, (int)sizeType, out sz);                
            }
            finally
            {
                g.ReleaseHdc(hdc);
            }
			return new Size( sz.CX, sz.CY );
		}

		internal Size GetPartSize( Graphics g, int partId, int stateId, THEMESIZE sizeType )
		{
			return GetPartSize( g, partId, stateId, (int)sizeType );
		}

		/// <summary>
		/// Calculates the size and location of the specified text when rendered in the theme font.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> object.</param>
		/// <param name="partId">An integer specifying the part.</param>
		/// <param name="stateId">An integer specifying the state.</param>
		/// <param name="text">The text to draw.</param>
		/// <param name="bounds">The layout bounds.</param>
		/// <param name="format">See GetThemeTextExtent method documentation in Windows API.</param>
		/// <returns>The rectangle representing the extent.</returns>
		[CLSCompliant( false )]
		public virtual Rectangle GetTextExtent( Graphics g, int partId, int stateId, String text, Rectangle bounds, uint format )
		{
			IntPtr hdc = g.GetHdc();

			NativeMethods.RECT boundingRect = new NativeMethods.RECT( bounds );
			NativeMethods.RECT rect;
			uint hr = NativeMethods.GetThemeTextExtent( this.HTheme, hdc, partId, stateId, text, text.Length,
			  format, ref boundingRect, out rect );

			g.ReleaseHdc( hdc );

			return new Rectangle( rect.left, rect.top, rect.Width, rect.Height );
		}
		#endregion DRAWING

		[DocumentationExclude()]
		public bool DrawMirrored
		{
			get
			{
				return m_bIsMirrored;
			}
			set
			{
				m_bIsMirrored = value;
			}
		}

		private void OnOwnerDisposed( object sender, EventArgs e )
		{
			if( m_owner != null )
			{
				UnadwiseOwner();
			}
		}

		private void AdwiseOwner( IComponent owner )
		{
			m_owner = owner;
			m_owner.Disposed += new EventHandler( OnOwnerDisposed );
		}

		private void UnadwiseOwner()
		{
			if( m_owner != null )
			{
				m_owner.Disposed -= new EventHandler( OnOwnerDisposed );
				m_owner = null;
			}

			XPThemes.UnregisterControlDrawing( this );
		}
	}

	[DocumentationExclude()]
	public class ThemedStatusBarDrawing: ThemedControlDrawing
	{
		public ThemedStatusBarDrawing()
			: base( ThemedControls.STATUS )
		{
		}

		public void DrawStatusGripper( Graphics g, Rectangle rect )
		{
			this.DrawThemeBackground( g, ThemeParts.SP_GRIPPER, 0, rect );
		}
	}

	[DocumentationExclude()]
	public class ThemedComboBoxDrawing: ThemedControlDrawing
	{
		// Reflects the order in the theme defines.
		public enum DropDownState
		{
			Normal=1,
			Hot,
			Pressed,
			Disabled
		}

		public ThemedComboBoxDrawing( string classNames )
			: base( classNames )
		{
		}

		public ThemedComboBoxDrawing( string classNames, IComponent component )
			: base( classNames, component )
		{
		}

		public void DrawDropDownButton( Graphics g, DropDownState state, Rectangle rect )
		{
			this.DrawThemeBackground( g, ThemeParts.CP_DROPDOWNBUTTON, (int)state, rect );
		}
	}

	[DocumentationExclude()]
	public class ThemedPushButtonDrawing: ThemedControlDrawing
	{
		public ThemedPushButtonDrawing()
			: base( ThemedControls.BUTTON )
		{
		}

		public ThemedPushButtonDrawing( IComponent owner )
			: base( ThemedControls.BUTTON, owner )
		{
		}

		// Use ButtonState.Flat for "normal" and ButtonState.Normal for "hot".
		public void DrawPushButton( Graphics g, Rectangle rect, ButtonState btnState )
		{
			int part = ThemeParts.BP_PUSHBUTTON;
			int state = 0;

			if( ( btnState & ButtonState.Inactive ) > 0 )
			{
				state = ThemeStates.PBS_DISABLED;
			}
			else if( ( btnState & ButtonState.Pushed ) > 0 )
			{
				state = ThemeStates.PBS_PRESSED;
			}
			else if( ( btnState & ButtonState.Flat ) > 0 )
			{
				state = ThemeStates.PBS_NORMAL;
			}
			else
			{
				state = ThemeStates.PBS_HOT;
			}

			this.DrawThemeBackground( g, part, state, rect );
		}
	}

	[DocumentationExclude()]
	public class ThemedHeaderDrawing: ThemedControlDrawing
	{
		public enum HeaderState
		{
			Normal=1,
			Hot,
			Pressed
		}

		public ThemedHeaderDrawing()
			: base( ThemedControls.HEADER )
		{
		}

		public void DrawHeader( Graphics g, Rectangle rect, HeaderState state )
		{
			this.DrawThemeBackground( g, ThemeParts.HP_HEADERITEM, (int)state, rect );
		}
	}

	[DocumentationExclude()]
	public class ThemedScrollBarDrawing: ThemedControlDrawing
	{
		public ThemedScrollBarDrawing()
			: base( ThemedControls.SCROLLBAR )
		{
		}

		public ThemedScrollBarDrawing( IComponent owner )
			: base( ThemedControls.SCROLLBAR, owner )
		{
		}

		// Using ButtonState.Flat to represent "hot" state.
		public void DrawThumb( Graphics g, Rectangle rect, bool horizontal, ButtonState btnState )
		{
			int state = 0;

			if( ( btnState & ButtonState.Inactive ) > 0 )
			{
				state = ThemeStates.SCRBS_DISABLED;
			}
			else if( ( btnState & ButtonState.Pushed ) > 0 )
			{
				state = ThemeStates.SCRBS_PRESSED;
			}
			else if( ( btnState & ButtonState.Flat ) > 0 )
			{
				state = ThemeStates.SCRBS_HOT;
			}
			else
			{
				state = ThemeStates.SCRBS_NORMAL;
			}

			this.DrawThemeBackground( g,
			  horizontal ? ThemeParts.SBP_THUMBBTNHORZ : ThemeParts.SBP_THUMBBTNVERT,
			  state, rect );
		}

		public void DrawSizeBox( Graphics g, Rectangle rect )
		{
			this.DrawThemeBackground( g, ThemeParts.SBP_SIZEBOX, ThemeStates.SCRBS_NORMAL, rect );
		}
	}

	[DocumentationExclude()]
	public class ThemedCheckBoxDrawing: ThemedControlDrawing
	{
		public ThemedCheckBoxDrawing()
			: base( ThemedControls.BUTTON )
		{
		}

		// Using ButtonState.Flat to represent "hot" state.
		public void DrawCheckBox( Graphics g, Rectangle rect, ButtonState btnState, bool mixedStateSet )
		{
			int part = ThemeParts.BP_CHECKBOX;
			int state = 0;

			// Relying on the order of the state defines.
			if( ( btnState & ButtonState.Inactive ) > 0 )
			{
				state = ThemeStates.CBS_UNCHECKEDDISABLED;
			}
			else if( ( btnState & ButtonState.Pushed ) > 0 )
			{
				state = ThemeStates.CBS_UNCHECKEDPRESSED;
			}
			else if( ( btnState & ButtonState.Flat ) > 0 )
			{
				state = ThemeStates.CBS_UNCHECKEDHOT;
			}
			else
			{
				state = ThemeStates.CBS_UNCHECKEDNORMAL;
			}

			if( ( btnState & ButtonState.Checked ) > 0 )
			{
				state += 4;
			}
			else if( mixedStateSet )
			{
				state += 8;
			}

			this.DrawThemeBackground( g, part, state, rect );
		}
	}

	[DocumentationExclude()]
	public class ThemedRadioButtonDrawing: ThemedControlDrawing
	{
		public ThemedRadioButtonDrawing()
			: base( ThemedControls.BUTTON )
		{
		}

		// Using ButtonState.Flat to represent "hot" state.
		public void DrawRadio( Graphics g, Rectangle rect, ButtonState btnState, bool mixedStateSet )
		{
			int part = ThemeParts.BP_RADIOBUTTON;
			int state = 0;

			// Relying on the order of the state defines.
			if( ( btnState & ButtonState.Inactive ) > 0 )
			{
				state = ThemeStates.RBS_UNCHECKEDDISABLED;
			}
			else if( ( btnState & ButtonState.Pushed ) > 0 )
			{
				state = ThemeStates.RBS_UNCHECKEDPRESSED;
			}
			else if( ( btnState & ButtonState.Flat ) > 0 )
			{
				state = ThemeStates.RBS_UNCHECKEDHOT;
			}
			else
			{
				state = ThemeStates.RBS_UNCHECKEDNORMAL;
			}

			if( ( btnState & ButtonState.Checked ) > 0 )
			{
				state += 4;
			}
			else if( mixedStateSet )
			{
				state += 8;
			}

			this.DrawThemeBackground( g, part, state, rect );
		}
	}

	[DocumentationExclude()]
	public class ThemedSpinButtonDrawing: ThemedControlDrawing
	{
		public ThemedSpinButtonDrawing()
			: base( ThemedControls.SPIN )
		{
		}

		public ThemedSpinButtonDrawing( IComponent owner )
			: base( ThemedControls.SPIN, owner )
		{
		}

		// ButtonState.Normal will be drawn as "hot" button and ButtonState.Flat will be drawn as "normal".
		public void DrawScrollButton( Graphics g, Rectangle rect, ButtonID buttonID,
		  ScrollButtonAppearance appearance, ButtonState btnState )
		{
			// Get part id.
			int part = 0;
			int state = 0;
			if( buttonID == ButtonID.Up )
			{
				if( appearance == ScrollButtonAppearance.Horizontal )
				{
					part = ThemeParts.SPNP_UPHORZ;
					if( ( btnState & ButtonState.Inactive ) > 0 )
					{
						state = ThemeStates.UPHZS_DISABLED;
					}
					else if( ( btnState & ButtonState.Pushed ) > 0 )
					{
						state = ThemeStates.UPHZS_PRESSED;
					}
					else if( ( btnState & ButtonState.Flat ) > 0 )
					{
						state = ThemeStates.UPHZS_NORMAL;
					}
					else
					{
						state = ThemeStates.UPHZS_HOT;
					}
				}
				else
				{
					part = ThemeParts.SPNP_UP;
					if( ( btnState & ButtonState.Inactive ) > 0 )
					{
						state = ThemeStates.UPS_DISABLED;
					}
					else if( ( btnState & ButtonState.Pushed ) > 0 )
					{
						state = ThemeStates.UPS_PRESSED;
					}
					else if( ( btnState & ButtonState.Flat ) > 0 )
					{
						state = ThemeStates.UPS_NORMAL;
					}
					else
					{
						state = ThemeStates.UPS_HOT;
					}
				}
			}
			else
			{
				if( appearance == ScrollButtonAppearance.Horizontal )
				{
					part = ThemeParts.SPNP_DOWNHORZ;
					if( ( btnState & ButtonState.Inactive ) > 0 )
					{
						state = ThemeStates.DNHZS_DISABLED;
					}
					else if( ( btnState & ButtonState.Pushed ) > 0 )
					{
						state = ThemeStates.DNHZS_PRESSED;
					}
					else if( ( btnState & ButtonState.Flat ) > 0 )
					{
						state = ThemeStates.DNHZS_NORMAL;
					}
					else
					{
						state = ThemeStates.DNHZS_HOT;
					}
				}
				else
				{
					part = ThemeParts.SPNP_DOWN;
					if( ( btnState & ButtonState.Inactive ) > 0 )
					{
						state = ThemeStates.DNS_DISABLED;
					}
					else if( ( btnState & ButtonState.Pushed ) > 0 )
					{
						state = ThemeStates.DNS_PRESSED;
					}
					else if( ( btnState & ButtonState.Flat ) > 0 )
					{
						state = ThemeStates.DNS_NORMAL;
					}
					else
					{
						state = ThemeStates.DNS_HOT;
					}
				}
			}

			this.DrawThemeBackground( g, part, state, rect );
		}
	}

	[DocumentationExclude()]
	public class ThemedGlobalsDrawing: ThemedControlDrawing
	{
		public ThemedGlobalsDrawing()
			: base( ThemedControls.GLOBALS )
		{
		}
	}

	[DocumentationExclude()]
	public class ThemedWindowDrawing: ThemedControlDrawing, INonClientPaintingSupport
	{
		public ThemedWindowDrawing()
			: base( ThemedControls.WINDOW )
		{
		}

		public ThemedWindowDrawing( IComponent owner )
			: base( ThemedControls.WINDOW, owner )
		{
		}

		protected override void Dispose( bool disposing )
		{
			if( cachedRgn != IntPtr.Zero )
			{
				NativeMethods.DeleteObject( cachedRgn );
				this.cachedRgn = IntPtr.Zero;
			}
			base.Dispose( disposing );
		}

		private IntPtr cachedRgn = IntPtr.Zero;

		public void DrawThemedBorderColor( Control control, ref Message msg )
		{
			if( cachedRgn != IntPtr.Zero )
			{
				NativeMethods.DeleteObject( cachedRgn );
				this.cachedRgn = IntPtr.Zero;
			}

			int style = NativeMethods.GetWindowLong( msg.HWnd, NativeMethods.GWL_STYLE );
			if( ( style & NativeMethods.WS_BORDER ) != 0 )
			{
				if( XPThemes.IsThemedOS && XPThemes.IsThemeActive )
				{
					this.cachedRgn = DrawingUtils.NCPaintHelper( control, this, ref msg );
				}
			}
		}

		IntPtr INonClientPaintingSupport.NonClientPaint( PaintEventArgs e, Rectangle displayRect, Rectangle windowRectInScreen )
		{
			Graphics g = e.Graphics;
			Rectangle bounds = displayRect;

			ThemedWindowDrawing themedDrawing = this;

			Color borderColor = themedDrawing.GetThemeSysColor( (int)ThemeColors.BORDERCOLOR );

			// Draw border with themed border color.
			// Draw lines, don't fill a rectangle.
			using( Pen pen = new Pen( borderColor ) )
			{
				g.DrawLine( pen, bounds.Location, new Point( bounds.Right, bounds.Y ) );
				g.DrawLine( pen, bounds.Location, new Point( bounds.X, bounds.Height ) );
				g.DrawLine( pen, new Point( bounds.X, bounds.Height - 1 ), new Point( bounds.Right - 1, bounds.Height - 1 ) );
				g.DrawLine( pen, new Point( bounds.Right - 1, bounds.Y ), new Point( bounds.Right - 1, bounds.Height - 1 ) );
			}

			return NativeMethods.CreateRectRgn( windowRectInScreen.Left + 1, windowRectInScreen.Top + 1, windowRectInScreen.Right - 1, windowRectInScreen.Bottom - 1 );
		}
	}

	[DocumentationExclude()]
	public class ThemedEditDrawing: ThemedControlDrawing
	{
		public ThemedEditDrawing() :
			base( ThemedControls.EDIT )
		{
		}

		public ThemedEditDrawing( IComponent owner ) :
			base( ThemedControls.EDIT, owner )
		{
		}

		public void DrawEditBoxBackground( Graphics g, Rectangle rect, bool enabled )
		{
			int part = ThemeParts.EP_EDITTEXT;
			int state = 0;
			if( !enabled )
			{
				state = ThemeStates.ETS_DISABLED;
			}
			else
			{
				state = ThemeStates.ETS_NORMAL;
			}

			this.DrawThemeBackground( g, part, state, rect );
		}

		public Size GetThemeBorderSize()
		{
			int hBorderWidth = NativeMethods.GetSystemMetrics( NativeMethods.SM_CXBORDER );
			int vBorderWidth = NativeMethods.GetSystemMetrics( NativeMethods.SM_CYBORDER );

			return new Size( hBorderWidth, vBorderWidth );
		}
	}

	[DocumentationExclude()]
	public class ThemedDropDownButtonDrawing: ThemedControlDrawing
	{
		public ThemedDropDownButtonDrawing()
			: base( ThemedControls.COMBOBOX )
		{
		}

		// ButtonState.Checked will be drawn as "HOT".
		public void DrawDropDownButton( Graphics g, Rectangle rect,
		  ButtonState btnState )
		{
			int part = ThemeParts.CP_DROPDOWNBUTTON;
			int state = 0;

			if( ( btnState & ButtonState.Inactive ) > 0 )
			{
				state = ThemeStates.CBXS_DISABLED;
			}
			else if( ( btnState & ButtonState.Pushed ) > 0 )
			{
				state = ThemeStates.CBXS_PRESSED;
			}
			else if( ( btnState & ButtonState.Checked ) > 0 )
			{
				state = ThemeStates.CBXS_HOT;
			}
			else
			{
				state = ThemeStates.CBXS_NORMAL;
			}

			this.DrawThemeBackground( g, part, state, rect );
		}
	}

	public class ThemedButtonDrawing: ThemedControlDrawing
	{
		public ThemedButtonDrawing()
			: base( ThemedControls.BUTTON )
		{
		}
	}

	public class ThemedTreeViewDrawing: ThemedControlDrawing
	{
		public ThemedTreeViewDrawing()
			: base( ThemedControls.TREEVIEW )
		{
		}
	}

	/// <summary>Cached version of themed controls instances. Class will 
	/// return NULL instead of instance if OS does not support XP Themes.</summary>
	public sealed class ThemedDrawing
	{
		#region Class static members
		/// <summary>Thread synchronization object. Used for instance clear and create 
		/// operations locks.</summary>
		private static readonly object s_sync = new object();
		/// <summary>Edit control.</summary>
		private static ThemedEditDrawing s_edit;
		/// <summary>Button control.</summary>
		private static ThemedButtonDrawing s_button;
		/// <summary>Tree control.</summary>
		private static ThemedTreeViewDrawing s_tree;
		#endregion

		#region Clas static properties
		/// <summary></summary>
		public static ThemedEditDrawing Edit
		{
			get
			{
				lock( s_sync )
				{
					if( s_edit == null && XPThemes.IsThemedOS )
						s_edit = new ThemedEditDrawing();

					return s_edit;
				}
			}
		}
		/// <summary></summary>
		public static ThemedButtonDrawing Button
		{
			get
			{
				lock( s_sync )
				{
					if( s_button == null && XPThemes.IsThemedOS )
						s_button = new ThemedButtonDrawing();

					return s_button;
				}
			}
		}
		/// <summary></summary>
		public static ThemedTreeViewDrawing TreeView
		{
			get
			{
				lock( s_sync )
				{
					if( s_tree == null && XPThemes.IsThemedOS )
						s_tree = new ThemedTreeViewDrawing();

					return s_tree;
				}
			}
		}

		#endregion

		#region Class utility methods
		/// <summary>Reset static class cache. All internal static resources will be released. 
		/// Operation is thread safe.</summary>
		public static void ResetCache()
		{
			lock( s_sync )
			{
				if( s_edit != null )
				{
					s_edit.Dispose();
					s_edit = null;
				}

				if( s_button != null )
				{
					s_button.Dispose();
					s_button = null;
				}

				if( s_tree != null )
				{
					s_tree.Dispose();
					s_tree = null;
				}
			}
		}
		#endregion
	}
}
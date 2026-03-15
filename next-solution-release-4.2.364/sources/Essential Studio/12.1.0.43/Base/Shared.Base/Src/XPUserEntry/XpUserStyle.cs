#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;

#endregion

namespace Syncfusion.Windows.Forms.Tools
{
/// <summary>
/// Class component style.
/// </summary>
[ TypeConverter( typeof( ExpandableObjectConverter ) )
, Serializable ]
public class XpUserStyle
{
	#region Class constants
	#endregion

	#region Class members

	/// <summary>
	/// Skip events for quiet mode.
	/// </summary>
	private bool m_bSkipEvents;

	/// <summary>
	/// User name text font for active mode.
	/// </summary>
	private Font m_ActiveFont = Control.DefaultFont;

	/// <summary>
	/// User name text color for default mode.
	/// </summary>
	private Color m_clrUserNameDefault = Color.Black;

	/// <summary>
	/// User name text color for active mode.
	/// </summary>
	private Color m_clrUserNameActive = Color.Black;

	/// <summary>
	///	User name text font for default mode.
	/// </summary>
	private Font m_font = Control.DefaultFont;

	/// <summary>
	/// Background gradient style.
	/// </summary>
	private StyleGradientObject m_background = new StyleGradientObject();

	/// <summary>
	/// Gradient style rectangle in user icon for default mode.
	/// </summary>
	private StyleGradientObject m_normalRect = new StyleGradientObject();

	/// <summary>
	/// Gradient style rectangle in user icon for active mode.
	/// </summary>
	private StyleGradientObject m_activeRect = new StyleGradientObject();

	/// <summary>
	///	User name text font for active mode.
	/// </summary>
	private Font m_fontActiveUserHelp = Control.DefaultFont;

	/// <summary>
	///	User help text font for default mode.
	/// </summary>
	private Font m_fontDefaultUserHelp = Control.DefaultFont;

	/// <summary>
	///	User name text color for active mode.
	/// </summary>
	private Color m_clrUserHelpActive = Color.Black;

	/// <summary>
	///	User name text color for default mode.
	/// </summary>
	private Color m_clrUserHelpDefault = Color.Black;

	/// <summary>
	///	Radius rounded rectangle in icon.
	/// </summary>
	private int m_rectRadius = 8;

	/// <summary>
	///	Radius rounded icons.
	/// </summary>
	private int m_iconRadius = 8;


	/// <summary>
	///	Draw shadow rectangle in icon.
	/// </summary>
	private bool m_drawShadow = true;

	/// <summary>
	///	Shadow size at pixel.
	/// </summary>
	private int m_shadowSize = 3;
	#endregion

	#region Class properties

	/// <summary>
	/// Get or set user name text font for default mode.
	/// </summary>
	[ Description( "Get or set user name text font for default mode." ) ]
	public Font Font
	{
		get
		{
			return m_font;
		}
		set
		{
			if( value != m_font )
			{
				ValueChangedEventArgs args = new ValueChangedEventArgs( m_font, value );
				m_font = value;
				OnFontChanged( args );
			}
		}
	}

	/// <summary>
	/// Get or set user name text font for active mode.
	/// </summary>
	[ 
		Category( "Appearance" )
		, Browsable( true )
		, Description( "Get or set user name text font for active mode." ) 
	]
	public Font ActiveFont
	{
		get
		{
			return m_ActiveFont;
		}
		set
		{
			if( value != m_ActiveFont )
			{
				ValueChangedEventArgs args = new ValueChangedEventArgs( m_ActiveFont, value );
				m_ActiveFont = value;
				OnActiveFontChanged( args );
			}
		}
	}

	/// <summary>
	///	Get or set help name text color for active mode.
	/// </summary>
	[ Description( "Get or set help name text color for active mode." ) ]
	public Font ActiveUserHelpFont
	{
		get
		{
			return m_fontActiveUserHelp ;
		}
		set
		{
			if( value != m_fontActiveUserHelp  )
			{
				ValueChangedEventArgs args = new ValueChangedEventArgs( m_fontActiveUserHelp , value );
				m_fontActiveUserHelp  = value;
				OnActiveUserHelpFontChanged( args );
			}
		}
	}

	/// <summary>
	///	Get or set help name text color for default mode.
	/// </summary>
	[ Description( "Get or set help name text color for default mode." ) ]
	public Font DefaultUserHelpFont
	{
		get
		{
			return m_fontDefaultUserHelp;
		}
		set
		{
			if( value != m_fontDefaultUserHelp )
			{
				ValueChangedEventArgs args = new ValueChangedEventArgs( m_fontDefaultUserHelp, value );
				m_fontDefaultUserHelp = value;
				OnDefaultUserHelpFontChanged( args );
			}
		}
	}

	/// <summary>
	/// Get or set background gradient style.
	/// </summary>
	[ 
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ) 
		, Description( "Get or set background gradient style." )
	]
	public StyleGradientObject Background
	{
		get
		{
			return m_background;
		}
		set
		{
			if( value != m_background )
			{
				if( m_background != null )
				{
					m_background.OnChanged -= new EventHandler( gradients_OnChanged );
				}
				
				m_background = ( value == null ) ? new StyleGradientObject() : value;
				
				m_background.OnChanged += new EventHandler( gradients_OnChanged );
			}
		}
	}

	/// <summary>
	/// Get or set gradient style rectangle in user icon for default mode.
	/// </summary>
	[ 
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ) 
		, Description( "Get or set gradient style rectangle in user icon for default mode." )
	]
	public StyleGradientObject NormalRectangle
	{
		get
		{
			return m_normalRect;
		}
		set
		{
			if( value != m_normalRect )
			{
				if( m_normalRect != null )
				{
					m_normalRect.OnChanged -= new EventHandler( gradients_OnChanged );
				}
				
				m_normalRect = ( value == null ) ? new StyleGradientObject() : value;
				
				m_normalRect.OnChanged += new EventHandler( gradients_OnChanged );
			}
		}
	}

	/// <summary>
	/// Get or set gradient style rectangle in user icon for active mode.
	/// </summary>
	[ 
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ) 
		, Description( "Get or set gradient style rectangle in user icon for active mode." )
	]
	public StyleGradientObject ActiveRectangle
	{
		get
		{
			return m_activeRect; 
		}
		set
		{
			if( value != m_activeRect )
			{
				if( m_activeRect != null )
				{
					m_activeRect.OnChanged -= new EventHandler( gradients_OnChanged );
				}
				
				m_activeRect = ( value == null ) ? new StyleGradientObject() : value;
				
				m_activeRect.OnChanged += new EventHandler( gradients_OnChanged );
			}
		}
	}

	/// <summary>
	/// User name text font for default mode.
	/// </summary>
	[ Description( "User name text font for default mode." ) ]
	public Color UserNameColorDefault
	{
		get
		{
			return m_clrUserNameDefault;
		}
		set
		{
			if( value != m_clrUserNameDefault )
			{
				ValueChangedEventArgs args = new ValueChangedEventArgs( m_clrUserNameDefault, value );
				m_clrUserNameDefault = value;
				OnUserNameColorDefaultChanged( args );
			}
		}
	}

	/// <summary>
	/// Get or set User name text font for active mode.
	/// </summary>
	[ Description( "Get or set User name text font for active mode." ) ]
	public Color UserNameColorActive
	{
		get
		{
			return m_clrUserNameActive;
		}
		set
		{
			if( value != m_clrUserNameActive )
			{
				ValueChangedEventArgs args = new ValueChangedEventArgs( m_clrUserNameActive, value );
				m_clrUserNameActive = value;
				OnUserNameColorActiveChanged( args );
			}
		}
	}

	/// <summary>
	///	Get or set user name text color for active mode.
	/// </summary>
	[ Description( "Get or set user name text color for active mode." ) ]
	public Color UserHelpColorActive
	{
		get
		{
			return m_clrUserHelpActive;
		}
		set
		{
			if( value != m_clrUserHelpActive )
			{
				ValueChangedEventArgs args = new ValueChangedEventArgs( m_clrUserHelpActive, value );
				m_clrUserHelpActive = value;
				OnUserHelpColorActiveChanged( args );
			}
		}
	}

	/// <summary>
	///	Get or set user name text color for default mode.
	/// </summary>
	[ Description( "Get or set user name text color for default mode." ) ]
	public Color UserHelpColorDefault
	{
		get
		{
			return m_clrUserHelpDefault;
		}
		set
		{
			if( value != m_clrUserHelpDefault )
			{
				ValueChangedEventArgs args = new ValueChangedEventArgs( m_clrUserHelpDefault, value );
				m_clrUserHelpDefault = value;
				OnUserHelpColorDefaultChanged( args );
			}
		}
	}

	/// <summary>
	///	Get or set radius rounded rectangle in icon.
	/// </summary>
  [ 
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		DefaultValue( 8 )
		, Description( "Get or set radius rounded rectangle in icon." )
	]
	public int RectRadius
	{
		get
		{
			return m_rectRadius;
		}
		set
		{
			if( value != m_rectRadius )
			{
				ValueChangedEventArgs args = new ValueChangedEventArgs( m_rectRadius, value );
				m_rectRadius = value;
				OnRectRadiusChanged( args );
			}
		}
	}
	
	/// <summary>
	///	Get or set radius rounded icons.
	/// </summary>
	[ 
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ), 
		DefaultValue( 8 )
		, Description( "Get or set radius rounded icons." )
	]
	public int IconRadius
	{
		get
		{
			return m_iconRadius;
		}
		set
		{
			if( value != m_iconRadius )
			{
				ValueChangedEventArgs args = new ValueChangedEventArgs( m_iconRadius, value );
				m_iconRadius = value;
				OnIconRadiusChanged( args );
			}
		}
	}

	/// <summary>
	///	Get or set draw shadow rectangle in icon.
	/// </summary>
	[ 
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ), 
		DefaultValue( true )
		, Description( "Get or set draw shadow rectangle in icon." )
	]
	public bool DrawShadow
	{
		get
		{
			return m_drawShadow;
		}
		set
		{
			if( value != m_drawShadow )
			{
				ValueChangedEventArgs args = new ValueChangedEventArgs( m_drawShadow, value );
				m_drawShadow = value;
				OnDrawShadowChanged( args );
			}
		}
	}

	/// <summary>
	///	Get or set shadow size at pixel.
	/// </summary>
	[ 
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ), 
		DefaultValue( 3 )
		, Description( "Get orset shadow size at pixel." )
	]

	public int ShadowSize
	{
		get
		{
			return m_shadowSize;
		}
		set
		{
			if( value != m_shadowSize )
			{
				ValueChangedEventArgs args = new ValueChangedEventArgs( m_shadowSize, value );
				m_shadowSize = value;
				OnShadowSizeChanged( args );
			}
		}
	}


	/// <summary>
	/// True - do not raise any events, otherwise False.
	/// </summary>
	[ Description( "True - do not raise any events, otherwise False." ) ]
	internal protected bool QuietMode
	{
		get
		{
			return m_bSkipEvents;
		}
		set
		{
			if( value != m_bSkipEvents )
			{
				m_bSkipEvents = value;
				OnQuietModeChanged();
			}
		}
	}

	#endregion

	#region	Serialization helper methods
	
	protected bool ShouldSerializeFont()
	{
		return ( this.Font != Control.DefaultFont );
	}
	
	protected bool ShouldSerializeActiveFont()
	{
		return ( this.ActiveFont != Control.DefaultFont );
	}

	protected bool ShouldSerializeActiveUserHelpFont()
	{
		return ( this.ActiveUserHelpFont != Control.DefaultFont );
	}
	
	protected bool ShouldSerializeDefaultUserHelpFont()
	{
		return ( this.DefaultUserHelpFont != Control.DefaultFont );
	}

	protected bool ShouldSerializeUserNameColorDefault()
	{
		return ( this.UserNameColorDefault != Color.Black );
	}

	protected bool ShouldSerializeUserNameColorActive()
	{
		return ( this.UserNameColorActive != Color.Black );
	}

	protected bool ShouldSerializeUserHelpColorActive()
	{
		return ( this.UserHelpColorActive != Color.Black );
	}

	protected bool ShouldSerializeUserHelpColorDefault()
	{
		return ( this.UserHelpColorDefault != Color.Black );
	}


	#endregion

	#region Class events
	/// <summary>
	/// Occurs when quiet mode changed.
	/// </summary>
	public event EventHandler QuietModeChanged;
	/// <summary>
	/// Occurs when StyleGradientObject changed.
	/// </summary>
	[ Category( "Property Changed" ) ]
	public event EventHandler OnChanged;
	/// <summary>
	/// Occurs when	active font changed.
	/// </summary>
	[ Category( "Property Changed" ) ]
	public event ValueChangedEventHandler ActiveFontChanged;
	/// <summary>
	/// Occurs when RectColorDefault �hanged.
	/// </summary>
	[ Category( "Property Changed" ) ]
	public event ValueChangedEventHandler RectColorDefaultChanged;
	/// <summary>
	/// Occurs when RectColorActive �hanged.
	/// </summary>
	[ Category( "Property Changed" ) ]
	public event ValueChangedEventHandler RectColorActiveChanged;
	/// <summary>
	/// Occurs when user name color default changed.
	/// </summary>
	[ Category( "Property Changed" ) ]
	public event ValueChangedEventHandler UserNameColorDefaultChanged;
	/// <summary>
	/// Occurs when user name color active changed.
	/// </summary>
	[ Category( "Property Changed" ) ]
	public event ValueChangedEventHandler UserNameColorActiveChanged;

	/// <summary>
	///	Occurs when font changed.
	/// </summary>
	[ Category( "Property Changed" ) ]
	public event ValueChangedEventHandler FontChanged;
	/// <summary>
	///	Occurs when background start color changed.
	/// </summary>
	[ Category( "Property Changed" ) ]
	public event ValueChangedEventHandler BackgroundStartColorChanged;

	/// <summary>
	///	Occurs when background end color changed.
	/// </summary>
	[ Category( "Property Changed" ) ]
	public event ValueChangedEventHandler BackgroundEndColorChanged;
	/// <summary>
	///	Occurs when	background gradient changed.
	/// </summary>
	[ Category( "Property Changed" ) ]
	public event ValueChangedEventHandler BackgroundGradientChanged;
	/// <summary>
	///	Occurs when	active user help font changed.
	/// </summary>
	[ Category( "Property Changed" ) ]
	public event ValueChangedEventHandler ActiveUserHelpFontChanged;
	/// <summary>
	///	Occurs when	default user help font changed.
	/// </summary>
	[ Category( "Property Changed" ) ]
	public event ValueChangedEventHandler DefaultUserHelpFontChanged;

	/// <summary>
	///	Occurs when	user help color active changed.
	/// </summary>
	[ Category( "Property Changed" ) ]
	public event ValueChangedEventHandler UserHelpColorActiveChanged;

	/// <summary>
	///	Occurs when	user help color default changed.
	/// </summary>
	[ Category( "Property Changed" ) ]
	public event ValueChangedEventHandler UserHelpColorDefaultChanged;

	/// <summary>
	///	Occurs when	RectRadius changed.
	/// </summary>
	[ Category( "Property Changed" ) ]
	public event ValueChangedEventHandler RectRadiusChanged;

	/// <summary>
	///	Occurs when	icon radius changed.
	/// </summary>
	[ Category( "Property Changed" ) ]
	public event ValueChangedEventHandler IconRadiusChanged;

	/// <summary>
	///	Occurs when	draw shadow changed.
	/// </summary>
	[ Category( "Property Changed" ) ]
	public event ValueChangedEventHandler DrawShadowChanged;

	/// <summary>
	///	Occurs when	shadow size changed
	/// </summary>
	[ Category( "Property Changed" ) ]
	public event ValueChangedEventHandler ShadowSizeChanged;
	#endregion

	#region Class initialize/finalize methods
	/// <summary>
	/// Constructor this class.
	/// </summary>
	public XpUserStyle()
	{
		m_background.OnChanged += new EventHandler( gradients_OnChanged );
		m_normalRect.OnChanged += new EventHandler( gradients_OnChanged );
		m_activeRect.OnChanged += new EventHandler( gradients_OnChanged );
	}
	#endregion

	#region Class Public Methods
	/// <summary>
	/// Draw image rounded rectangle.
	/// </summary>
	/// <param name="g">Graphics for paint.</param>
	/// <param name="rc">GraphicsPath rounded rectangle.</param>
	/// <param name="bActive">Active mode.</param>
	public void DrawImageRect( Graphics g, GraphicsPath rc, bool bActive )
	{
		if( bActive )
		{
			m_activeRect.FillRectangle( g, rc );
		}
		else
		{
			m_normalRect.FillRectangle( g, rc );
		}
	}
	/// <summary>
	/// Draw control background.
	/// </summary>
	/// <param name="g">Graphics for paint.</param>
	/// <param name="bounds">Rectangle it sketch.</param>
	public void DrawControlBackground( Graphics g, Rectangle bounds )
	{
		m_background.FillRectangle( g, bounds );
	}
	#endregion

	#region Class event raisers
	
	protected void RaiseOnChanged( ValueChangedEventArgs args )
	{
		if( OnChanged != null && !this.QuietMode )
		{
			OnChanged( this, args );
		}
	}
	
	protected void RaiseQuietModeChangedEvent()
	{
		if( QuietModeChanged != null )
		{
			QuietModeChanged( this, EventArgs.Empty );
		}
	}
	
	protected void RaiseActiveFontChanged( ValueChangedEventArgs args )
	{
		if( ActiveFontChanged != null && !this.QuietMode )
		{
			ActiveFontChanged( this, args );
		}

		RaiseOnChanged( args );
	}
	
	protected void RaiseRectColorDefaultChanged( ValueChangedEventArgs args )
	{
		if( RectColorDefaultChanged != null && !this.QuietMode )
		{
			RectColorDefaultChanged( this, args );
		}

		RaiseOnChanged( args );
	}
	
	protected void RaiseRectColorActiveChanged( ValueChangedEventArgs args )
	{
		if( RectColorActiveChanged != null && !this.QuietMode )
		{
			RectColorActiveChanged( this, args );
		}

		RaiseOnChanged( args );
	}

	protected void RaiseUserNameColorDefaultChanged( ValueChangedEventArgs args )
	{
		if( UserNameColorDefaultChanged != null && !this.QuietMode )
		{
			UserNameColorDefaultChanged( this, args );
		}

		RaiseOnChanged( args );
	}

	protected void RaiseUserNameColorActiveChanged( ValueChangedEventArgs args )
	{
		if( UserNameColorActiveChanged != null && !this.QuietMode )
		{
			UserNameColorActiveChanged( this, args );
		}

		RaiseOnChanged( args );
	}

	protected void RaiseFontChanged( ValueChangedEventArgs args )
	{
		if( FontChanged != null )
		{
			FontChanged( this, args );
		}

		RaiseOnChanged( args );
	}

	protected void RaiseBackgroundStartColorChanged( ValueChangedEventArgs args )
	{
		if( BackgroundStartColorChanged != null )
		{
			BackgroundStartColorChanged( this, args );
		}
	}

	protected void RaiseBackgroundEndColorChanged( ValueChangedEventArgs args )
	{
		if( BackgroundEndColorChanged != null )
		{
			BackgroundEndColorChanged( this, args );
		}
	}

	protected void RaiseBackgroundGradientChanged( ValueChangedEventArgs args )
	{
		if( BackgroundGradientChanged != null )
		{
			BackgroundGradientChanged( this, args );
		}
	}

	protected void RaiseActiveUserHelpFontChanged( ValueChangedEventArgs args )
	{
		if( ActiveUserHelpFontChanged != null && !this.QuietMode )
		{
			ActiveUserHelpFontChanged( this, args );
		}
		RaiseOnChanged( args );
	}

	protected void RaiseDefaultUserHelpFontChanged( ValueChangedEventArgs args )
	{
		if( DefaultUserHelpFontChanged != null && !this.QuietMode )
		{
			DefaultUserHelpFontChanged( this, args );
		}
		RaiseOnChanged( args );
	}

	protected void RaiseUserHelpColorActiveChanged( ValueChangedEventArgs args )
	{
		if( UserHelpColorActiveChanged != null && !this.QuietMode)
		{
			UserHelpColorActiveChanged( this, args );
		}
		RaiseOnChanged( args );
	}

	protected void RaiseUserHelpColorDefaultChanged( ValueChangedEventArgs args )
	{
		if( UserHelpColorDefaultChanged != null && !this.QuietMode)
		{
			UserHelpColorDefaultChanged( this, args );
		}
		RaiseOnChanged( args );
	}

	protected void RaiseRectRadiusChanged( ValueChangedEventArgs args )
	{
		if( RectRadiusChanged != null && !this.QuietMode)
		{
			RectRadiusChanged( this, args );
		}
		RaiseOnChanged( args );
	}

	protected void RaiseIconRadiusChanged( ValueChangedEventArgs args )
	{
		if( IconRadiusChanged != null )
		{
			IconRadiusChanged( this, args );
		}
		RaiseOnChanged( args );
	}

	
	protected void RaiseDrawShadowChanged( ValueChangedEventArgs args )
	{
		if( DrawShadowChanged != null )
		{
			DrawShadowChanged( this, args );
		}
		RaiseOnChanged( args );
	}

	protected void RaiseShadowSizeChanged( ValueChangedEventArgs args )
	{
		if( ShadowSizeChanged != null )
		{
			ShadowSizeChanged( this, args );
		}
		RaiseOnChanged( args );
	}
	#endregion

	#region Class overrides

	/// <summary>
	/// Occurs when quiet mode changed.
	/// </summary>
	protected virtual void OnQuietModeChanged()
	{
		RaiseQuietModeChangedEvent();
	}

	/// <summary>
	/// Occurs when	active font changed.
	/// </summary>
	/// <param name="args"/>
	protected virtual void OnActiveFontChanged( ValueChangedEventArgs args )
	{
		RaiseActiveFontChanged( args );
	}
	/// <summary>
	/// Occurs when RectColorDefault �hanged.
	/// </summary>
	/// <param name="args"/>
	protected virtual void OnRectColorDefaultChanged( ValueChangedEventArgs args )
	{
		RaiseRectColorDefaultChanged( args );
	}

	/// <summary>
	/// Occurs when RectColorActive �hanged.
	/// </summary>
	/// <param name="args"/>
	protected virtual void OnRectColorActiveChanged( ValueChangedEventArgs args )
	{
		RaiseRectColorActiveChanged( args );
	}

	/// <summary>
	/// Occurs when user name color default changed.
	/// </summary>
	/// <param name="args"/>
	protected virtual void OnUserNameColorDefaultChanged( ValueChangedEventArgs args )
	{
		RaiseUserNameColorDefaultChanged( args );
	}

	/// <summary>
	/// Occurs when user name color active changed.
	/// </summary>
	/// <param name="args"/>
	protected virtual void OnUserNameColorActiveChanged( ValueChangedEventArgs args )
	{
		RaiseUserNameColorActiveChanged( args );
	}
	
	/// <summary>
	///	Occurs when font changed.
	/// </summary>
	protected virtual void OnFontChanged( ValueChangedEventArgs args )
	{
		RaiseFontChanged( args );
	}

	/// <summary>
	///	Occurs when background start color changed.
	/// </summary>
	protected virtual void OnBackgroundStartColorChanged( ValueChangedEventArgs args )
	{
		RaiseBackgroundStartColorChanged( args );
	}

	/// <summary>
	///	Occurs when background end color changed.
	/// </summary>
	protected virtual void OnBackgroundEndColorChanged( ValueChangedEventArgs args )
	{
		RaiseBackgroundEndColorChanged( args );
	}

	/// <summary>
	///	Occurs when	background gradient changed.
	/// </summary>
	protected virtual void OnBackgroundGradientChanged( ValueChangedEventArgs args )
	{
		RaiseBackgroundGradientChanged( args );
	}


	/// <summary>
	///	Occurs when	active user help font changed.
	/// </summary>
	protected virtual void OnActiveUserHelpFontChanged( ValueChangedEventArgs args )
	{
		RaiseActiveUserHelpFontChanged( args );
	}

	/// <summary>
	///	Occurs when	default user help font changed.
	/// </summary>
	protected virtual void OnDefaultUserHelpFontChanged( ValueChangedEventArgs args )
	{
		RaiseDefaultUserHelpFontChanged( args );
	}

	/// <summary>
	///	Occurs when	user help color active changed.
	/// </summary>
	protected virtual void OnUserHelpColorActiveChanged( ValueChangedEventArgs args )
	{
		RaiseUserHelpColorActiveChanged( args );
	}

	/// <summary>
	///	Occurs when	user help color default changed.
	/// </summary>
	protected virtual void OnUserHelpColorDefaultChanged( ValueChangedEventArgs args )
	{
		RaiseUserHelpColorDefaultChanged( args );
	}

	/// <summary>
	///	Occurs when	RectRadius changed.
	/// </summary>
	protected virtual void OnRectRadiusChanged( ValueChangedEventArgs args )
	{
		RaiseRectRadiusChanged( args );
	}

	/// <summary>
	///	Occurs when	icon radius changed.
	/// </summary>
	protected virtual void OnIconRadiusChanged( ValueChangedEventArgs args )
	{
		RaiseIconRadiusChanged( args );
	}

	/// <summary>
	///	Occurs when draw shadow changed.
	/// </summary>
	protected virtual void OnDrawShadowChanged( ValueChangedEventArgs args )
	{
		RaiseDrawShadowChanged( args );
	}

	/// <summary>
	///	Occurs when	shadow size changed
	/// </summary>
	protected virtual void OnShadowSizeChanged( ValueChangedEventArgs args )
	{
		RaiseShadowSizeChanged( args );
	}
	#endregion

	#region Class utility methods
	/// <summary>
	/// Occurs when StyleGradientObject changed.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void gradients_OnChanged( object sender, EventArgs e )
	{
		RaiseOnChanged( ( ValueChangedEventArgs )e );
	}
	#endregion
}
}
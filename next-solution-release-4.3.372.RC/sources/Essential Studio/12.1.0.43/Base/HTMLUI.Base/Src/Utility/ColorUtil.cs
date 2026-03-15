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
using System.Drawing;
#endregion

namespace Syncfusion.HTMLUI.Base.Utility
{
  /// <summary>
  /// Summary description for ColorConvert.
  /// </summary>
  public sealed class ColorUtil
  {
    #region Class members
    /// <summary>
    ///
    /// </summary>
    private static Color  m_clrBackground = Color.Empty;
    /// <summary>
    ///
    /// </summary>
    private static Color  m_clrSelection  = Color.Empty;
    /// <summary>
    ///
    /// </summary>
    private static Color  m_clrControl    = Color.Empty;
    /// <summary>
    ///
    /// </summary>
    private static Color  m_clrPressed    = Color.Empty;
    /// <summary>
    ///
    /// </summary>
    private static Color  m_clrChecked    = Color.Empty;
    /// <summary>
    ///
    /// </summary>
    private static Color  m_clrBorder     = Color.Empty;
    /// <summary>
    ///
    /// </summary>
    private static SolidBrush  m_brushBack;
    /// <summary>
    ///
    /// </summary>
    private static SolidBrush  m_brushSelect;
    /// <summary>
    ///
    /// </summary>
    private static SolidBrush  m_brushCtrl;
    /// <summary>
    ///
    /// </summary>
    private static SolidBrush  m_brushPress;
    /// <summary>
    ///
    /// </summary>
    private static SolidBrush  m_brushCheck;
    /// <summary>
    ///
    /// </summary>
    private static SolidBrush  m_brushBorder;
    /// <summary>
    ///
    /// </summary>
    private static SolidBrush m_brushContrast1;
    /// <summary>
    ///
    /// </summary>
    private static SolidBrush m_brushContrast2;
    /// <summary>
    ///
    /// </summary>
    private static SolidBrush m_brushContrast3;
    /// <summary>
    ///
    /// </summary>
    private static Pen    m_penBack;
    /// <summary>
    ///
    /// </summary>
    private static Pen    m_penSelect;
    /// <summary>
    ///
    /// </summary>
    private static Pen    m_penCtrl;
    /// <summary>
    ///
    /// </summary>
    private static Pen    m_penPress;
    /// <summary>
    ///
    /// </summary>
    private static Pen    m_penCheck;
    /// <summary>
    ///
    /// </summary>
    private static Pen    m_penBorder;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets the color of the VS net background.
    /// </summary>
    /// <value>The color of the VS net background.</value>
    static public Color VSNetBackgroundColor
    {
      get
      {
        return m_clrBackground;
      }
    }

    /// <summary>
    /// Gets the VS net background brush.
    /// </summary>
    /// <value>The VS net background brush.</value>
    static public Brush VSNetBackgroundBrush
    {
      get
      {
        // If system colors are changed by user, then re-create brush.
        if( m_brushBack == null )
          m_brushBack = new SolidBrush( VSNetBackgroundColor );
        else
          m_brushBack.Color = VSNetBackgroundColor;

        return m_brushBack;
      }
    }

    /// <summary>
    /// Gets the VS net background pen.
    /// </summary>
    /// <value>The VS net background pen.</value>
    static public Pen   VSNetBackgroundPen
    {
      get
      {
        if( m_penBack == null )
          m_penBack = new Pen( VSNetBackgroundBrush );
        else
          m_penBack.Color = VSNetBackgroundColor;

        return m_penBack;
      }
    }

    /// <summary>
    /// Gets the color of the VS net selection.
    /// </summary>
    /// <value>The color of the VS net selection.</value>
    static public Color VSNetSelectionColor
    {
      get
      {
        return m_clrSelection;
      }
    }

    /// <summary>
    /// Gets the VS net selection brush.
    /// </summary>
    /// <value>The VS net selection brush.</value>
    static public Brush VSNetSelectionBrush
    {
      get
      {
        if( m_brushSelect == null )
          m_brushSelect = new SolidBrush( VSNetSelectionColor );
        else
          m_brushSelect.Color = VSNetSelectionColor;

        return m_brushSelect;
      }
    }
    /// <summary>
    /// Gets the VS net selection pen.
    /// </summary>
    /// <value>The VS net selection pen.</value>
    static public Pen   VSNetSelectionPen
    {
      get
      {
        if( m_penSelect == null )
          m_penSelect = new Pen( VSNetSelectionBrush );
        else
          m_penSelect.Color = VSNetSelectionColor;

        return m_penSelect;
      }
    }

    /// <summary>
    /// Gets the color of the VS net control.
    /// </summary>
    /// <value>The color of the VS net control.</value>
    static public Color VSNetControlColor
    {
      get
      {
        return m_clrControl;
      }
    }

    /// <summary>
    /// Gets the VS net control brush.
    /// </summary>
    /// <value>The VS net control brush.</value>
    static public Brush VSNetControlBrush
    {
      get
      {
        if( m_brushCtrl == null )
          m_brushCtrl = new SolidBrush( VSNetControlColor );
        else
          m_brushCtrl.Color = VSNetControlColor;

        return m_brushCtrl;
      }
    }

    /// <summary>
    /// Gets the VS net control pen.
    /// </summary>
    /// <value>The VS net control pen.</value>
    static public Pen   VSNetControlPen
    {
      get
      {
        if( m_penCtrl == null )
          m_penCtrl = new Pen( VSNetControlBrush );
        else
          m_penCtrl.Color = VSNetControlColor;

        return m_penCtrl;
      }
    }

    /// <summary>
    /// Gets the color of the VS net pressed.
    /// </summary>
    /// <value>The color of the VS net pressed.</value>
    static public Color VSNetPressedColor
    {
      get
      {
        return m_clrPressed;
      }
    }

    /// <summary>
    /// Gets the VS net pressed brush.
    /// </summary>
    /// <value>The VS net pressed brush.</value>
    static public Brush VSNetPressedBrush
    {
      get
      {
        if( m_brushPress == null )
          m_brushPress = new SolidBrush( VSNetPressedColor );
        else
          m_brushPress.Color = VSNetPressedColor;

        return m_brushPress;
      }
    }

    /// <summary>
    /// Gets the VS net pressed pen.
    /// </summary>
    /// <value>The VS net pressed pen.</value>
    static public Pen   VSNetPressedPen
    {
      get
      {
        if( m_penPress == null )
          m_penPress = new Pen( VSNetPressedBrush );
        else
          m_penPress.Color = VSNetPressedColor;

        return m_penPress;
      }
    }

    /// <summary>
    /// Gets the color of the VS net checked.
    /// </summary>
    /// <value>The color of the VS net checked.</value>
    static public Color VSNetCheckedColor
    {
      get
      {
        return m_clrChecked;
      }
    }

    /// <summary>
    /// Gets the VS net checked brush.
    /// </summary>
    /// <value>The VS net checked brush.</value>
    static public Brush VSNetCheckedBrush
    {
      get
      {
        if( m_brushCheck == null )
          m_brushCheck = new SolidBrush( VSNetCheckedColor );
        else
          m_brushCheck.Color = VSNetCheckedColor;

        return m_brushCheck;
      }
    }

    /// <summary>
    /// Gets the VS net checked pen.
    /// </summary>
    /// <value>The VS net checked pen.</value>
    static public Pen   VSNetCheckedPen
    {
      get
      {
        if( m_penCheck == null )
          m_penCheck = new Pen( VSNetCheckedBrush );
        else
          m_penCheck.Color = VSNetCheckedColor;

        return m_penCheck;
      }
    }

    /// <summary>
    /// Gets the color of the VS net border.
    /// </summary>
    /// <value>The color of the VS net border.</value>
    static public Color VSNetBorderColor
    {
      get
      {
        return SystemColors.Highlight;
      }
    }

    /// <summary>
    /// Gets the VS net border brush.
    /// </summary>
    /// <value>The VS net border brush.</value>
    static public Brush VSNetBorderBrush
    {
      get
      {
        if( m_brushBorder == null )
          m_brushBorder = new SolidBrush( VSNetBorderColor );
        else
          m_brushBorder.Color = VSNetBorderColor;

        return m_brushBorder;
      }
    }

    /// <summary>
    /// Gets the VS net border pen.
    /// </summary>
    /// <value>The VS net border pen.</value>
    static public Pen   VSNetBorderPen
    {
      get
      {
        if( m_penBorder == null )
          m_penBorder = new Pen( VSNetBorderBrush );
        else
          m_penBorder.Color = VSNetBorderColor;

        return m_penBorder;
      }
    }

    /// <summary>
    /// Gets the VS net contrast1.
    /// </summary>
    /// <value>The VS net contrast1.</value>
    static public Brush VSNetContrast1
    {
      get
      {
        if( m_brushContrast1 == null )
          m_brushContrast1 = new SolidBrush( Color.FromArgb( 120, SystemColors.MenuText ) );
        else
          m_brushContrast1.Color = Color.FromArgb( 120, SystemColors.MenuText );

        return m_brushContrast1;
      }
    }
    /// <summary>
    /// Gets the VS net contrast2.
    /// </summary>
    /// <value>The VS net contrast2.</value>
    static public Brush VSNetContrast2
    {
      get
      {
        if( m_brushContrast2 == null )
          m_brushContrast2 = new SolidBrush( Color.FromArgb( 255, SystemColors.MenuText ) );
        else
          m_brushContrast2.Color = Color.FromArgb( 255, SystemColors.MenuText );

        return m_brushContrast2;
      }
    }
    /// <summary>
    /// Gets the VS net contrast3.
    /// </summary>
    /// <value>The VS net contrast3.</value>
    static public Brush VSNetContrast3
    {
      get
      {
        if( m_brushContrast3 == null )
          m_brushContrast3 = new SolidBrush( Color.FromArgb( 255, Color.White ) );

        return m_brushContrast3;
      }
    }
    #endregion

    #region Class events
    /// <summary>
    /// Event that is to be raised when colors are changed and all controls must be repainted.
    /// </summary>
    public static event EventHandler RepaintNeeded;
    #endregion

    #region Initialization/Constructor
    /// <summary>
    ///
    /// </summary>
    static ColorUtil()
    {
      Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler( SystemEvents_DisplaySettingsChanged );
      Microsoft.Win32.SystemEvents.PaletteChanged += new EventHandler( SystemEvents_PaletteChanged );
      Microsoft.Win32.SystemEvents.UserPreferenceChanged += new Microsoft.Win32.UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);

      CalculateMainColors();
    }
    // No need to construct this object.
    private ColorUtil()
    {
      throw new NotImplementedException();
    }
    /// <summary>
    ///
    /// </summary>
    private static void CalculateMainColors()
    {
      m_clrBackground = CalculateColor( SystemColors.Window, SystemColors.Control, 220 );
      m_clrSelection = CalculateColor( SystemColors.Highlight, SystemColors.Window, 70 );
      m_clrControl = CalculateColor( SystemColors.Control, m_clrBackground, 195 );
      m_clrPressed = CalculateColor( SystemColors.Highlight, m_clrSelection, 70 );
      m_clrChecked = CalculateColor( SystemColors.Highlight,  SystemColors.Window, 30 );

      if( RepaintNeeded != null )
      {
        RepaintNeeded( null, EventArgs.Empty );
      }
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private static void SystemEvents_DisplaySettingsChanged( object sender, EventArgs e )
    {
      CalculateMainColors();
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private static void SystemEvents_PaletteChanged( object sender, EventArgs e )
    {
      CalculateMainColors();
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private static void SystemEvents_UserPreferenceChanged( object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e )
    {
      CalculateMainColors();
    }
    #endregion

    #region Conversion between RGB and Hue, Saturation and Luminosity function helpers
    /// <summary>
    /// HSLs to RGB.
    /// </summary>
    /// <param name="h">The h.</param>
    /// <param name="s">The s.</param>
    /// <param name="l">The l.</param>
    /// <param name="r">The r.</param>
    /// <param name="g">The g.</param>
    /// <param name="b">The b.</param>
    static public void HSLToRGB( float h, float s, float l, ref float r, ref float g, ref float b )
    {
      // given h,s,l,[240 and r,g,b [0-255]
      // convert h [0-360], s,l,r,g,b [0-1]
      h=( h/240 )*360;
      s /= 240;
      l /= 240;
      r /= 255;
      g /= 255;
      b /= 255;

      // Begin Foley
      float m1,m2;

      // Calc m2
      if( l<=0.5f )
      {
        //m2=( l*( l+s ) ); seems to be typo in Foley??, replace l for 1
        m2=( l*( 1+s ) );
      }
      else
      {
        m2=( l+s-l*s );
      }

      //calc m1
      m1=2.0f*l-m2;

      //calc r,g,b in [0-1]
      if( s==0.0f )
      { // Achromatic: There is no hue
        // leave out the UNDEFINED part, h will always have value
        r=g=b=l;
      }
      else
      { // Chromatic: There is a hue
        r= getRGBValue( m1,m2,h+120.0f );
        g= getRGBValue( m1,m2,h );
        b= getRGBValue( m1,m2,h-120.0f );
      }

      // End Foley
      // convert to 0-255 ranges
      r*=255;
      g*=255;
      b*=255;

    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    /// <param name="hue"></param>
    /// <returns></returns>
    static private float getRGBValue( float n1, float n2, float hue )
    {
      // Helper function for the HSLToRGB function above
      if( hue>360.0f )
      {
        hue-=360.0f;
      }
      else if( hue<0.0f )
      {
        hue+=360.0f;
      }

      if( hue<60.0 )
      {
        return n1+( n2-n1 )*hue/60.0f;
      }
      else if( hue<180.0f )
      {
        return n2;
      }
      else if( hue<240.0f )
      {
        return n1+( n2-n1 )*( 240.0f-hue )/60.0f;
      }
      else
      {
        return n1;
      }
    }

    /// <summary>
    /// RGBs to HSL.
    /// </summary>
    /// <param name="r">The r.</param>
    /// <param name="g">The g.</param>
    /// <param name="b">The b.</param>
    /// <param name="h">The h.</param>
    /// <param name="s">The s.</param>
    /// <param name="l">The l.</param>
    static public void RGBToHSL( int r, int g, int b, ref float h, ref float s, ref float l )
    {

      //Computer Graphics - Foley p.595
      float delta;
      float fr = ( float )r/255;
      float fg = ( float )g/255;
      float fb = ( float )b/255;
      float max = Math.Max( fr,Math.Max( fg,fb ) );
      float min = Math.Min( fr,Math.Min( fg,fb ) );

      //calc the lightness
      l = ( max+min )/2;

      if( max==min )
      {
        //should be undefined but this works for what we need
        s = 0;
        h = 240.0f;
      }
      else
      {

        delta = max-min;

        //calc the saturation
        if( l < 0.5 )
        {
          s = delta/( max+min );
        }
        else
        {
          s = delta/( 2.0f-( max+min ) );
        }

        //calc the hue
        if( fr==max )
        {
          h = ( fg-fb )/delta;
        }
        else if( fg==max )
        {
          h = 2.0f + ( fb-fr )/delta;
        }
        else if( fb==max )
        {
          h = 4.0f + ( fr-fg )/delta;
        }

        //convert hue to degrees
        h*=60.0f;
        if( h<0.0f )
        {
          h+=360.0f;
        }
      }
      //end foley

      //convert to 0-255 ranges
      //h [0-360], h,l [0-1]
      l*=240;
      s*=240;
      h=( h/360 )*240;

    }
    #endregion

    #region Visual Studio .NET colors calculation helpers
    /// <summary>
    ///
    /// </summary>
    /// <param name="front"></param>
    /// <param name="back"></param>
    /// <param name="alpha"></param>
    /// <returns></returns>
    private static Color CalculateColor( Color front, Color back, int alpha )
    {
      // Use alpha blending to brighten the colors but don't use it
      // directly. Instead derive an opaque color that we can use.
      // If we use a color with alpha blending directly, we won't be able
      // to paint over whatever color was in the background and there
      // would be shadows of that color showing through
      Color frontColor = Color.FromArgb( 255, front );
      Color backColor = Color.FromArgb( 255, back );

      float frontRed = frontColor.R;
      float frontGreen = frontColor.G;
      float frontBlue = frontColor.B;
      float backRed = backColor.R;
      float backGreen = backColor.G;
      float backBlue = backColor.B;

      float fRed = frontRed*alpha/255 + backRed*(( float )( 255-alpha )/255 );
      byte newRed = ( byte )fRed;
      float fGreen = frontGreen*alpha/255 + backGreen*(( float )( 255-alpha )/255 );
      byte newGreen = ( byte )fGreen;
      float fBlue = frontBlue*alpha/255 + backBlue*(( float )( 255-alpha )/255 );
      byte newBlue = ( byte )fBlue;

      return  Color.FromArgb( 255, newRed, newGreen, newBlue );

    }
    #endregion

    #region General functions
    /// <summary>
    /// Determines whether [is known color] [the specified color].
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="knownColor">Color of the known.</param>
    /// <param name="useTransparent">if set to <c>true</c> [use transparent].</param>
    /// <returns>
    /// 	<c>true</c> if [is known color] [the specified color]; otherwise, <c>false</c>.
    /// </returns>
    static public bool IsKnownColor( Color color, ref Color knownColor, bool useTransparent )
    {

      // Using the color structrure "FromKnownColor" does not work if
      // we did not create the color as a known color to begin with.
      // We need to compare the RGBs of both colors.
      Color currentColor = Color.Empty;
      bool badColor = false;
      for( KnownColor enumValue = 0; enumValue <= KnownColor.YellowGreen; enumValue++ )
      {
        currentColor = Color.FromKnownColor( enumValue );
        string colorName = currentColor.Name;
        if( !useTransparent )
          badColor = ( colorName == "Transparent" );
        if( color.A == currentColor.A && color.R == currentColor.R && color.G == currentColor.G
          && color.B == currentColor.B && !currentColor.IsSystemColor
          && !badColor )
        {
          knownColor = currentColor;
          return true;
        }

      }
      return false;

    }

    /// <summary>
    /// Determines whether [is system color] [the specified color].
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="knownColor">Color of the known.</param>
    /// <returns>
    /// 	<c>true</c> if [is system color] [the specified color]; otherwise, <c>false</c>.
    /// </returns>
    static public bool IsSystemColor( Color color, ref Color knownColor )
    {

      // Using the color structrure "FromKnownColor" does not work if
      // we did not create the color as a known color to begin with.
      // We need to compare the RGBs of both colors.
      Color currentColor = Color.Empty;
      for( KnownColor enumValue = 0; enumValue <= KnownColor.YellowGreen; enumValue++ )
      {
        currentColor = Color.FromKnownColor( enumValue );
        string colorName = currentColor.Name;
        if( color.R == currentColor.R && color.G == currentColor.G
          && color.B == currentColor.B && currentColor.IsSystemColor )
        {
          knownColor = currentColor;
          return true;
        }

      }
      return false;
    }

    /// <summary>
    /// Colors from RGB string.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    static public Color ColorFromRGBString( string text )
    {
      Color rgbColor = Color.Empty;
      string[] RGBs = text.Split( ',' );
      if( RGBs.Length != 3 )
      {
        // If we don't have three pieces of information, then the
        // string is not properly formatted, inform the user.
        throw new Exception( "RGB color string is not well formed." );
      }

      string stringR = RGBs[0];
      string stringG = RGBs[1];
      string stringB = RGBs[2];
      int R, G, B;

      try
      {
        R = Convert.ToInt32( stringR );
        G = Convert.ToInt32( stringG );
        B = Convert.ToInt32( stringB );
        if( ( R < 0 || R > 255 ) || ( G < 0 || G > 255 ) || ( B < 0 || B > 255 ) )
        {
          throw new Exception( "Out of bounds RGB value." );
        }
        else
        {
          // Convert to color.
          rgbColor = Color.FromArgb( R, G, B );
          // See if we have either a web color or a system color.
          Color knownColor = Color.Empty;
          bool isKnown = ColorUtil.IsKnownColor( rgbColor, ref knownColor, true );
          if( !isKnown )
            isKnown = ColorUtil.IsSystemColor( rgbColor, ref knownColor );
          if( isKnown )
            rgbColor = knownColor;
        }
      }
      catch( InvalidCastException )
      {
        throw new Exception( "Invalid RGB value" );
      }

      return rgbColor;
    }
    #endregion

    #region Windows RGB related macros
    /// <summary>
    /// Returns the red value of one channel level.
    /// </summary>
    /// <param name="color">Integer type color value.</param>
    /// <returns>Red channel of color.</returns>
    static public byte GetRValue( int color )
    {
      return ( byte )color;
    }
    /// <summary>
    /// Returns green channel level of color.
    /// </summary>
    /// <param name="color">Integer type color value.</param>
    /// <returns>Green channel level of color.</returns>
    static public byte GetGValue( int color )
    {
      return (( byte )((( short )( color ) ) >> 8 ) );
    }
    /// <summary>
    /// Returns the blue channel level of color.
    /// </summary>
    /// <param name="color">Integer type color value.</param>
    /// <returns>Blue channel level of color.</returns>
    static public byte GetBValue( int color )
    {
      return (( byte )(( color )>>16 ) );
    }
    /// <summary>
    /// RGBs the specified r.
    /// </summary>
    /// <param name="r">The r.</param>
    /// <param name="g">The g.</param>
    /// <param name="b">The b.</param>
    /// <returns>RGB color in integer</returns>
    static public int RGB( int r, int g, int b )
    {
      return (( int )((( byte )( r ) |
        (( short )(( byte )( g ) )<<8 ) ) |
        ((( short )( byte )( b ) )<<16 ) ) );
    }
    /// <summary>
    /// RGBs the specified CLR.
    /// </summary>
    /// <param name="clr">The CLR.</param>
    /// <returns>RGB color in integer</returns>
    static public int RGB( Color clr )
    {
      return (( int )((( byte )( clr.R ) |
        (( short )(( byte )( clr.G ) )<<8 ) ) |
        ((( short )( byte )( clr.B ) )<<16 ) ) );
    }
    #endregion

  }
}
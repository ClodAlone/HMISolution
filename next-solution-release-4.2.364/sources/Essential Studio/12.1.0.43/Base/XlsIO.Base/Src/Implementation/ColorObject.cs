#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

using Syncfusion.XlsIO.Implementation.XmlReaders;

#if ( WINRT )
using Windows.UI;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;

#endif

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// This object used to store, convert colors.
  /// </summary>
  public class ColorObject : IDisposable
  {
    #region Members
    /// <summary>
    /// Type of the stored color.
    /// </summary>
    private ColorType m_colorType;
    /// <summary>
    /// Color value.
    /// </summary>
    private int m_color;
    private double m_tintAndShade;
    /// <summary>
    /// Color Saturation Modulation value
    /// </summary>
    private double m_satMod;
    /// <summary>
    /// Color Luminence value
    /// </summary>
    private double m_lumOff;
    /// <summary>
    /// Color Saturation
    /// </summary>
    private double m_sat;
    /// <summary>
    /// Color Luminence Modulation
    /// </summary>
    private double m_lumMod;
    /// <summary>
    /// Indicate Scheme color
    /// </summary>
    private bool m_bIsSchemeColor;
    /// <summary>
    /// Indicate Schema Name
    /// </summary>
    private string m_schemaName;
    #endregion

    #region Properties
    /// <summary>
    /// Event called after color change.
    /// </summary>
    public event AfterChangeHandler AfterChange;
    /// <summary>
    /// Returns color value (it can be index, rgb color, etc.)
    /// </summary>
    public int Value
    {
      get
      {
        return m_color;
      }
    }
    /// <summary>
    /// Gets or sets Tint.
    /// </summary>
    public double Tint
    {
      get
      {
        return m_tintAndShade;
      }
      set
      {
        m_tintAndShade = value;
      }
    }
    internal double Saturation
    {
        get
        {
            return m_satMod;
        }
        set
        {
            m_satMod = value;
        }
    }
    internal double Luminance
    {
        get
        {
            return m_lumMod;
        }
        set
        {
            m_lumMod = value;
        }
    }
    internal double LuminanceOffSet
    {
        get
        {
            return m_lumOff;
        }
        set
        {
            m_lumOff = value;
        }
    }
    internal bool IsSchemeColor
    {
        get
        {
            return m_bIsSchemeColor;
        }
        set
        {
            m_bIsSchemeColor = value;
        }
    }
    internal string SchemaName
    {
        get
        {
            return m_schemaName;
        }
        set
        {
            m_schemaName = value;
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the color object.
    /// </summary>
    /// <param name="color">Color value to initialize.</param>
    public ColorObject( Color color )
      : this( ColorType.RGB, color.ToArgb() )
    {
    }
    /// <summary>
    /// Initializes new instance of the color object.
    /// </summary>
    /// <param name="color">Color value to initialize.</param>
    public ColorObject( ExcelKnownColors color )
      : this( ColorType.Indexed, ( int )color )
    {
    }
    /// <summary>
    /// Initializes a new instance of the ColorObject class.
    /// </summary>
    /// <param name="colorType">Represents type of Color.</param>
    /// <param name="colorValue">Represents color value.</param>
    public ColorObject( ColorType colorType, int colorValue )
      : this( colorType, colorValue, 0 )
    {
    }
    /// <summary>
    /// Initializes a new instance of the ColorObject class.
    /// </summary>
    /// <param name="colorType">Represents color type.</param>
    /// <param name="colorValue">Represents color value.</param>
    /// <param name="tint">Represents tint value.</param>
    public ColorObject( ColorType colorType, int colorValue, double tint )
    {
      m_colorType = colorType;
      m_color = colorValue;
      m_tintAndShade = tint;
    }
    /// <summary>
    /// Returns type of the stored color.
    /// </summary>
		public ColorType ColorType
    {
      get
      {
        return m_colorType;
      }
        set
        {
            m_colorType = value;
        }
    }
    /// <summary>
    /// Returns index for indexed color or the closest color for any other color type.
    /// </summary>
    /// <param name="book">Parent workbook</param>
    /// <returns>Index for indexed color or the closest color for any other color type.</returns>
    public ExcelKnownColors GetIndexed( IWorkbook book )
    {
      ExcelKnownColors result;

      switch( m_colorType )
      {
        case ColorType.Indexed:
          result = ( ExcelKnownColors )m_color;
          break;

        default:            
          result = ( book as WorkbookImpl ).GetNearestColor( GetRGB( book ), WorkbookImpl.DEF_FIRST_USER_COLOR );    
           
          break;
      }

      return result;
    }
    /// <summary>
    /// Sets indexed color.
    /// </summary>
    /// <param name="value">Color index to set.</param>
    public void SetIndexed( ExcelKnownColors value)
    {
      SetIndexed( value, true );
    }
    /// <summary>
    /// Sets indexed color.
    /// </summary>
    /// <param name="value">Color index to set.</param>
    public void SetIndexed( ExcelKnownColors value, bool raiseEvent  )
    {
      if( m_colorType != ColorType.Indexed ||
        m_color != ( int )value )
      {
        m_colorType = ColorType.Indexed;
        m_color = ( int )value;
        Normalize( false );

        m_tintAndShade = 0;

        if( raiseEvent && AfterChange != null )
          AfterChange();
      }
    }
    /// <summary>
    /// Sets the indexed.
    /// </summary>
    /// <param name="value">Color index to set.</param>
    /// <param name="raiseEvent">if set to <c>true</c> [raise event].</param>
    /// <param name="book">The book.</param>
    public void SetIndexed(ExcelKnownColors value, bool raiseEvent,WorkbookImpl book)
    {
        if (m_colorType != ColorType.Indexed ||
          m_color != (int)value)
        {
            m_colorType = ColorType.Indexed;
            m_color = (int)value;
            if(!book.IsEqualColor)
            Normalize(false,book);

            m_tintAndShade = 0;

            if (raiseEvent && AfterChange != null)
                AfterChange();
        }
    }
    /// <summary>
    /// Returns RGB Color object that corresponds to this color.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    /// <returns>RGB Color object that corresponds to this color.</returns>
    public Color GetRGB( IWorkbook book )
    {
      Color result;

      switch( m_colorType )
      {
        case ColorType.RGB:
          result = ColorExtension.FromArgb( m_color );
          break;

        case ColorType.Indexed:
          result = book.GetPaletteColor( ( ExcelKnownColors )m_color );
          break;

        case ColorType.Theme:
          result = ( book as WorkbookImpl ).GetThemeColor( m_color );
#if  (SILVERLIGHT) || (WINRT) || (WP)
          if(m_tintAndShade==0)
            result=Color.FromArgb(255, result.R, result.G, result.B);
              
#endif
          break;

        default:
          throw new InvalidOperationException();
      }

      if (m_tintAndShade != 0)
      {
          int column = 0;
          int row = 0;
          int themeColorCount = WorkbookImpl.ThemeColorPalette.Length;
          int tintCount = WorkbookImpl.DefaultTints.Length;
          foreach (double tint in WorkbookImpl.DefaultTints)
          {
              if (tint == m_tintAndShade)
                  break;
              column++;
          }
          foreach (Color color in WorkbookImpl.DefaultThemeColors)
          {
              if (color.Equals(result))
                  break;
              row++;
          }
          if (row < themeColorCount && column < tintCount && WorkbookImpl.ThemeColorPalette[row].Length > column)
            result = WorkbookImpl.ThemeColorPalette[row][column];
          else
            result = Excel2007Parser.ConvertColorByTint(result, m_tintAndShade);
      }


      return result;
    }
    /// <summary>
    /// Sets RGB color value.
    /// </summary>
    /// <param name="value">Value to set.</param>
    /// <param name="book">Parent workbook.</param>
    public void SetRGB( Color value, IWorkbook book )
    {
      SetRGB( value, book, 0 );
    }
    /// <summary>
    /// Sets RGB color value.
    /// </summary>
    /// <param name="value">Value to set.</param>
    internal void SetRGB( Color value )
    {
      int iRgb = value.ToArgb();
      int defaultColor;
#if  (SILVERLIGHT) || ( WINRT ) || (WP)
      defaultColor = Color.FromArgb(0, 0, 0, 0).ToArgb();
#else 
        defaultColor=Color.Black.ToArgb();
#endif

      if ( m_colorType != ColorType.RGB ||
          m_color != iRgb || m_color == defaultColor)
      {
        m_colorType = ColorType.RGB;
        m_color = iRgb;
        m_tintAndShade = 0;

        if( AfterChange != null )
          AfterChange();
      }
    }
    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="color">Value to convert.</param>
    /// <returns>Converted item.</returns>
    public static implicit operator ColorObject( Color color )
    {
      return new ColorObject( color );
    }
    /// <summary>
    /// Checks whether two instance have the same data.
    /// </summary>
    /// <param name="first">First color object to check.</param>
    /// <param name="second">Second color object to check.</param>
    /// <returns>True if they have the same data.</returns>
    public static bool operator ==( ColorObject first, ColorObject second )
    {
      object objFirst = ( object )first;
      object objSecond = ( object )second;

      if( objFirst == null && objSecond == null )
        return true;

      if( objFirst == null && objSecond != null ||
        objFirst != null && objSecond == null)
        return false;

      return( first.m_colorType == second.m_colorType &&
        first.m_color == second.m_color &&
        first.m_tintAndShade == second.m_tintAndShade );
    }
    /// <summary>
    /// Checks whether two instances have different data.
    /// </summary>
    /// <param name="first">First color object to check.</param>
    /// <param name="second">Second color object to check.</param>
    /// <returns>True if they have different data.</returns>
    public static bool operator !=( ColorObject first, ColorObject second )
    {
      object objFirst = ( object )first;
      object objSecond = ( object )second;

      if( objFirst == null && objSecond == null )
        return false;

      if( objFirst == null && objSecond != null ||
        objFirst != null && objSecond == null)
        return true;

      return( first.m_colorType != second.m_colorType ||
        first.m_color != second.m_color ||
        first.m_tintAndShade != second.m_tintAndShade );
    }
    /// <summary>
    /// Copies data from another color object.
    /// </summary>
    /// <param name="colorObject">Color object to copy data from.</param>
    /// <param name="callEvent">Indicates whether we should call AfterChange method.</param>
    internal void CopyFrom( ColorObject colorObject, bool callEvent )
    {
      if( colorObject == null )
        throw new ArgumentNullException( "colorObject" );

      m_color = colorObject.m_color;
      m_colorType = colorObject.m_colorType;
      m_tintAndShade = colorObject.m_tintAndShade;

      if( callEvent && AfterChange != null )
        AfterChange();
    }
    /// <summary>
    /// Converts current color to closest indexed.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    internal void ConvertToIndexed( IWorkbook book )
    {
      if( m_colorType != ColorType.Indexed )
      {
        SetIndexed( GetIndexed( book ) );
      }
    }
    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
      return m_color.GetHashCode() ^ m_colorType.GetHashCode();
    }
    /// <summary>
    /// Sets indexed color without calling AfterChange event.
    /// </summary>
    /// <param name="value">Color index to set.</param>
    public void SetIndexedNoEvent( ExcelKnownColors value )
    {
      m_colorType = ColorType.Indexed;
      m_color = ( int )value;
    }
    /// <summary>
    /// Creates copy of the current object.
    /// </summary>
    /// <returns>A copy of the current object.</returns>
    internal ColorObject Clone()
    {
      return ( ColorObject )MemberwiseClone();
    }
    /// <summary>
    /// Normalizes indexed color if necessary.
    /// </summary>
    internal void Normalize()
    {
      Normalize( true );
    }
    /// <summary>
    /// Normalizes indexed color if necessary.
    /// </summary>
    internal void Normalize( bool raiseEvent )
    {
      if( m_colorType == ColorType.Indexed )
      {
        int value = m_color;
        //if( m_color < DEF_MAXBADCOLOR )
        if( m_color == 0 )
        {
          m_color += BorderImpl.DEF_BADCOLOR_INCREMENT;
        }
#if !SILVERLIGHT && !WINRT && !WP
        else if ((m_color < BorderImpl.DEF_MAXBADCOLOR) && 
                    this.AfterChange != null && 
                    this.AfterChange.Method != null &&    
                    (this.AfterChange.Method.Name == "UpdateForeColor" ||
                     this.AfterChange.Method.Name == "ColorChangeEventHandler"))
        {
            m_color += BorderImpl.DEF_MAXBADCOLOR;
        }
#else
        else if (m_color < BorderImpl.DEF_MAXBADCOLOR)
            m_color += BorderImpl.DEF_MAXBADCOLOR;
#endif

        if ( value != m_color )
          SetIndexed( ( ExcelKnownColors )m_color, raiseEvent );
      }
    }
    /// <summary>
    /// Normalizes indexed color if necessary.
    /// </summary>
    internal void Normalize(bool raiseEvent,WorkbookImpl book)
    {
        if (m_colorType == ColorType.Indexed)
        {
            int value = m_color;
            //if( m_color < DEF_MAXBADCOLOR )
            if (m_color == 0)
            {
                m_color += BorderImpl.DEF_BADCOLOR_INCREMENT;
            }
            //else if (m_color < BorderImpl.DEF_MAXBADCOLOR && !book.IsLoaded)
            //{
            //    m_color += BorderImpl.DEF_MAXBADCOLOR;
            //}

            if (value != m_color)
                SetIndexed((ExcelKnownColors)m_color, raiseEvent);
        }
    }
    /// <summary>
    /// Determines whether the specified Object is equal to the current Object.
    /// </summary>
    /// <param name="obj">The Object to compare with the current Object.</param>
    /// <returns>true if the specified Object is equal to the current Object; otherwise, false.</returns>
    public override bool Equals( object obj )
    {
      ColorObject toCheck = obj as ColorObject;

      return ( obj != null ) ? ( toCheck == this ) : false;
    }
    /// <summary>
    /// Sets theme color.
    /// </summary>
    /// <param name="themeIndex">Theme color index.</param>
    /// <param name="book">Parent workbook.</param>
    public void SetTheme( int themeIndex, IWorkbook book )
    {
      SetTheme( themeIndex, book, 0 );
    }
    /// <summary>
    /// Sets theme color
    /// </summary>
    /// <param name="themeIndex">Theme color index.</param>
    /// <param name="book">Parent workbook.</param>
    /// <param name="dTintValue">Tint value.</param>
    public void SetTheme( int themeIndex, IWorkbook book, double dTintValue )
    {
      if( m_colorType != ColorType.Theme ||
        m_color != themeIndex )
      {
        m_colorType = ColorType.Theme;
        m_color = themeIndex;
        m_tintAndShade = dTintValue;

        if( AfterChange != null )
           AfterChange();        
      }
    }
    /// <summary>
    /// Sets rgb color.
    /// </summary>
    /// <param name="rgb">Rgb color to set.</param>
    /// <param name="book">Parent workbook.</param>
    /// <param name="dTintValue">Tint value.</param>
    public void SetRGB( Color rgb, IWorkbook book, double dTintValue )
    {
      int iRgb = rgb.ToArgb();
      int defaultColor;
#if  (SILVERLIGHT) || ( WINRT ) || (WP)
      defaultColor = Color.FromArgb(0, 0, 0, 0).ToArgb();
#else 
        defaultColor=Color.Black.ToArgb();
#endif
      if( m_colorType != ColorType.RGB ||
        m_color != iRgb || m_color == defaultColor )
      {
        m_colorType = ColorType.RGB;
        m_color = iRgb;
        m_tintAndShade = 0;
        
        if( AfterChange != null )
        AfterChange();        
      }
    }
    #endregion

    #region Types
    /// <summary>
    /// Delegate used for after change event.
    /// </summary>
    public delegate void AfterChangeHandler();
    #endregion

    #region IDisposable Members

    public void Dispose()
    {
       AfterChange = null; 
       GC.SuppressFinalize(this);
    }

    #endregion
  }
  /// <summary>
  /// This enumeration contains possible color types.
  /// </summary>
  public enum ColorType
  {
    /// <summary>
    /// Automatic color.
    /// </summary>
    Automatic = 0x0,
    /// <summary>
    /// Indexed color.
    /// </summary>
    Indexed = 0x1,
    /// <summary>
    /// RGB color.
    /// </summary>
    RGB = 0x2,
    /// <summary>
    /// Theme color.
    /// </summary>
    Theme = 0x3,
    /// <summary>
    /// Color not set
    /// </summary>
    None=0x4,
  }
}

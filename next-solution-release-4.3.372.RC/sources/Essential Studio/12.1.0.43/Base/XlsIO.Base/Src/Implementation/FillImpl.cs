#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

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

using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Implementation
{
	/// <summary>
	/// Summary description for FillImpl.
	/// </summary>
	public class FillImpl
	{
    #region Members
    /// <summary>
    /// 
    /// </summary>
    private ColorObject m_color = new ColorObject( ( ExcelKnownColors )64 );
    /// <summary>
    /// 
    /// </summary>
    private ColorObject m_patternColor = new ColorObject( ( ExcelKnownColors )65 );
    /// <summary>
    /// 
    /// </summary>
    private ExcelPattern m_pattern;
    /// <summary>
    /// Gradient fill style.
    /// </summary>
    private ExcelGradientStyle m_gradientStyle;
    /// <summary>
    /// Gradient fill variant.
    /// </summary>
    private ExcelGradientVariants m_gradientVariant;
    /// <summary>
    /// Gradient fill type.
    /// </summary>
    private ExcelFillType m_fillType;
    #endregion

    #region Constructors
    /// <summary>
    /// Prevents user from creating such items without arguments.
    /// </summary>
    public FillImpl()
    {
    }
    /// <summary>
    /// Initializes new instance of the fill.
    /// </summary>
    /// <param name="format">Parent extended format.</param>
    public FillImpl( ExtendedFormatImpl format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      IGradient gradient = format.Gradient;

      if( gradient != null )
      {
    //    m_fillType = gradient.FillType;
        m_gradientStyle = gradient.GradientStyle;
        m_gradientVariant = gradient.GradientVariant;
        m_color = gradient.BackColorObject;
        m_patternColor = gradient.ForeColorObject;
      }
      else
      {
        m_color = format.ColorObject;
        m_patternColor = format.PatternColorObject;

      }

      m_pattern = format.FillPattern;
    }
    /// <summary>
    /// Initializes new instance of the fill.
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="color"></param>
    /// <param name="patternColor"></param>
    public FillImpl( ExcelPattern pattern, Color color, Color patternColor )
    {
      m_pattern = pattern;

      if( pattern != ExcelPattern.None )
        m_color.SetRGB( color );

      if( pattern != ExcelPattern.Solid )
        m_patternColor.SetRGB( patternColor );

      m_fillType = ( pattern == ExcelPattern.Solid ) ? ExcelFillType.SolidColor : ExcelFillType.Pattern;
    }
    /// <summary>
    /// Initializes new instance of the fill.
    /// </summary>
    /// <param name="pattern">Represents pattern.</param>
    /// <param name="color">Represents color.</param>
    /// <param name="patternColor">Represents pattern color.</param>
    public FillImpl( ExcelPattern pattern, ColorObject color, ColorObject patternColor )
    {
      m_pattern = pattern;

      if( pattern != ExcelPattern.None )
        m_color = color;
        //m_color.SetRGB( color );

      if( pattern != ExcelPattern.Solid )
        m_patternColor = patternColor;
        //m_patternColor.SetRGB( patternColor );

      m_fillType = ( pattern == ExcelPattern.Solid ) ? ExcelFillType.SolidColor : ExcelFillType.Pattern;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Returns the color of the interior. The color is specified as
    /// an index value into the current color palette. Read-only.
    /// </summary>
    public ColorObject ColorObject
    {
      get
      {
        return m_color;
      }
    }
    /// <summary>
    /// Returns the color of the interior pattern as an index into the current
    /// color palette. Read-only.
    /// </summary>
    public ColorObject PatternColorObject
    {
      get
      {
        return m_patternColor;
      }
    }
    /// <summary>
    /// Returns fill pattern. Read-only.
    /// </summary>
    public ExcelPattern Pattern
    {
      get
      {
        //return m_format.FillPattern;
        return m_pattern;
      }
      set
      {
        m_pattern = value;
      }
    }
    /// <summary>
    /// Gets / sets gradient style.
    /// </summary>
    public ExcelGradientStyle GradientStyle
    {
      get
      {
        return m_gradientStyle;
      }
      set
      {
        m_gradientStyle = value;
      }
    }
    /// <summary>
    /// Gets / sets gradient variant.
    /// </summary>
    public ExcelGradientVariants GradientVariant
    {
      get
      {
        return m_gradientVariant;
      }
      set
      {
        m_gradientVariant = value;
      }
    }
    /// <summary>
    /// Gets / sets fill type.
    /// </summary>
    public ExcelFillType FillType
    {
      get
      {
        return m_fillType;
      }
      set
      {
        m_fillType = value;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Determines whether the specified Object is equal to the current Object.
    /// </summary>
    /// <param name="obj">The Object to compare with the current Object.</param>
    /// <returns>true if the specified Object is equal to the current Object; otherwise, false.</returns>
    public override bool Equals( object obj )
    {
      FillImpl toCompare = obj as FillImpl;

      if( toCompare == null ) return false;

      return ColorObject == toCompare.ColorObject &&
        PatternColorObject == toCompare.PatternColorObject &&
        Pattern == toCompare.Pattern &&
        GradientStyle == toCompare.GradientStyle &&
        GradientVariant == toCompare.GradientVariant &&
        FillType == toCompare.FillType;
    }
    /// <summary>
    /// Serves as a hash function for a particular type, suitable for use
    /// in hashing algorithms and data structures like a hash table.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
      return ColorObject.GetHashCode() ^
        PatternColorObject.GetHashCode() ^
        Pattern.GetHashCode() ^
        GradientStyle.GetHashCode() ^
        GradientVariant.GetHashCode() ^
        FillType.GetHashCode();
    }
    /// <summary>
    /// Creates copy of the current object.
    /// </summary>
    /// <returns>A copy of the current object.</returns>
    public FillImpl Clone()
    {
      FillImpl result = ( FillImpl )MemberwiseClone();

      return result;
    }
    #endregion

    internal void Dispose()
    {
        this.m_color.Dispose();
        this.m_patternColor.Dispose();

        this.m_color = null;
        this.m_patternColor = null;
    }
    }
}

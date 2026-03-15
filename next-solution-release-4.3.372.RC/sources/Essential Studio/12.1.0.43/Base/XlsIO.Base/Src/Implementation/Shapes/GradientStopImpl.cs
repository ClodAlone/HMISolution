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

namespace Syncfusion.XlsIO.Implementation.Shapes
{
  /// <summary>
  /// This class represents single gradient stop of the gradient fill.
  /// </summary>
  public class GradientStopImpl
  {
    #region Constants
    /// <summary>
    /// Size of the binary data.
    /// </summary>
    internal const int Size = 12;
    #endregion

    #region Members
    /// <summary>
    /// Gradient stop color.
    /// </summary>
    private ColorObject m_color;
    /// <summary>
    /// Stop position.
    /// </summary>
    private int m_iPosiiton;
    /// <summary>
    /// Transparency.
    /// </summary>
    private int m_iTransparency;
    /// <summary>
    /// Tint of the color for this gradient stop.
    /// </summary>
    private int m_iTint = -1;
    /// <summary>
    /// Shade of the color for this gradient stop.
    /// </summary>
    private int m_iShade = -1;
    #endregion

    #region Properties
    /// <summary>
    /// Gets / sets color of this gradient stop.
    /// </summary>
    public ColorObject ColorObject
    {
      get
      {
        return m_color;
      }
    }
    /// <summary>
    /// Gets / sets position of this gradient stop.
    /// </summary>
    public int Position
    {
      get
      {
        return m_iPosiiton;
      }
      set
      {
        m_iPosiiton = value;
      }
    }
    /// <summary>
    /// Gets / sets transparency of this gradient stop.
    /// </summary>
    public int Transparency
    {
      get
      {
        return m_iTransparency;
      }
      set
      {
        m_iTransparency = value;
      }
    }
    /// <summary>
    /// Gets / sets tint of the color for this gradient stop.
    /// </summary>
    public int Tint
    {
      get
      {
        return m_iTint;
      }
      set
      {
        m_iTint = value;
      }
    }
    /// <summary>
    /// Gets / sets shade of the color for this gradient stop.
    /// </summary>
    public int Shade
    {
      get
      {
        return m_iShade;
      }
      set
      {
        m_iShade = value;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Creates new instance of the GradientStop.
    /// </summary>
    /// <param name="color">Gradient stop color.</param>
    /// <param name="position">Gradient stop position.</param>
    /// <param name="transparency">Gradient stop transparecy.</param>
    public GradientStopImpl( ColorObject color, int position, int transparency )
      :this( color, position, transparency, -1, -1 )
    {
    }
    /// <summary>
    /// Creates new instance of the GradientStop.
    /// </summary>
    /// <param name="color">Gradient stop color.</param>
    /// <param name="position">Gradient stop position.</param>
    /// <param name="transparency">Gradient stop transparecy.</param>
    /// <param name="tint">Color tint value.</param>
    /// <param name="shade">Color shade value.</param>
    public GradientStopImpl( ColorObject color, int position, int transparency, int tint, int shade )
    {
      m_color = color;
      m_iPosiiton = position;
      m_iTransparency = transparency;
      m_iTint = tint;
      m_iShade = shade;
    }
    /// <summary>
    /// Initializes new instance of the gradient stop and extracts settings from specified data array..
    /// </summary>
    /// <param name="data">Data to parse.</param>
    /// <param name="offset">Offset to start data parsing from.</param>
    public GradientStopImpl( byte[] data, int offset )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      m_iPosiiton = BitConverter.ToInt32( data, offset );
      offset += ExcelConstants.IntSize;

      int iColor = BitConverter.ToInt32( data, offset );
      m_color = new ColorObject( ColorExtension.FromArgb( iColor ) );
      offset += ExcelConstants.IntSize;

      m_iTransparency = BitConverter.ToInt32( data, offset );
      //offset += ExcelConstants.IntSize;
    }
    /// <summary>
    /// Serializes gradient stop into specified stream.
    /// </summary>
    /// <param name="stream">Stream to serialize data into.</param>
    internal void Serialize( System.IO.Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      byte[] arrBuffer = BitConverter.GetBytes( m_iPosiiton );
      stream.Write( arrBuffer, 0, arrBuffer.Length );

      arrBuffer = BitConverter.GetBytes( m_color.Value );
      stream.Write( arrBuffer, 0, arrBuffer.Length );

      arrBuffer = BitConverter.GetBytes( m_iTransparency );
      stream.Write( arrBuffer, 0, arrBuffer.Length );
    }
    /// <summary>
    /// Creates copy of the current instance.
    /// </summary>
    /// <returns>A copy of the current instance.</returns>
    internal GradientStopImpl Clone()
    {
      GradientStopImpl result = ( GradientStopImpl )MemberwiseClone();
      result.m_color = new ColorObject( ExcelKnownColors.None );
      result.m_color.CopyFrom( m_color, false );
      return result;
    }
    #endregion
    /// <summary>
    /// Checks whether the specified gradient stop  is equal to the current
    /// gradient stop without taking transparency into consideration.
    /// </summary>
    /// <param name="stop">Represents gradient stop.</param>
    /// <returns>Value indicating transparency</returns>
    internal bool EqualsWithoutTransparency( GradientStopImpl stop )
    {
      if( stop == null )
        return false;

      return ( stop.m_color == m_color &&
        stop.m_iPosiiton == m_iPosiiton &&
        stop.m_iShade == m_iShade &&
        stop.m_iTint == m_iTint );
    }

    internal void Dispose()
    {
        m_color.Dispose();
        m_color = null;
    }
  }
}

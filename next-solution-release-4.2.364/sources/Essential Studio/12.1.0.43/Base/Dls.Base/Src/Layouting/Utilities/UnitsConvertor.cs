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
using System.Drawing.Drawing2D;
#endregion

namespace Syncfusion.Layouting
{
  /// <summary>
  /// Known to us measure units.
  /// </summary>
  public enum PrintUnits
  {
    /// <summary>
    /// Specifies 1/75 inch as the unit of measure.
    /// </summary>
    Display,
    /// <summary>
    /// Specifies the document unit (1/300 inch) as the unit of measure.
    /// </summary>
    Document,
    /// <summary>
    /// Specifies the inch as the unit of measure.
    /// </summary>
    Inch,
    /// <summary>
    /// Specifies the millimeter as the unit of measure.
    /// </summary>
    Millimeter,
    /// <summary>
    /// Specifies the centimeter as the unit of measure.
    /// </summary>
    Centimeter,
    /// <summary>
    /// Specifies a device pixel as the unit of measure.
    /// </summary>
    Pixel,
    /// <summary>
    /// Specifies a printers point (1/72 inch) as the unit of measure.
    /// </summary>
    Point,
  }

  /// <summary>
  /// Class allow to convert differ metrics units. Convert is 
  /// based on Graphics object DPI settings that is why for differ
  /// graphics settings must be created new instance. For example:
  /// printers often has 300 and greater dpi resolution, for compare
  /// default display screen dpi is 96.
  /// </summary>
  public class UnitsConvertor
  {
    #region Class static members
    private static Bitmap   _bmp = new Bitmap( 1, 1 );
    private static Graphics _graph = Graphics.FromImage( _bmp );
    #endregion

    #region Class members
    /// <summary>
    /// Matrix for conversations between different numeric systems
    /// </summary>
    private double[]    m_Proportions = null;
    #endregion

    #region Class static properties
    /// <summary>
    /// Get Empty graphics created on Bitmap
    /// </summary>
    public static Graphics EmptyGraphics
    {
      get
      {
        return _graph;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Prevent class creation
    /// </summary>
    public UnitsConvertor()
      : this( UnitsConvertor.EmptyGraphics ) 
    {
    }
    /// <summary>
    /// Create Units convert class based on specified Graphics units
    /// </summary>
    /// <param name="g">Graphics for measuring</param>
    public UnitsConvertor( Graphics g )
    {
      UpdateProportions( g ); 
    }
    /// <summary>
    /// Update proportions matrix according to Graphics settings
    /// </summary>
    /// <param name="g">reference to graphics</param>
    private void  UpdateProportions( Graphics g )
    {
      if( g == null )
        throw new ArgumentNullException( "g" );

      PointF[] points = new PointF[]{ new PointF( 1, 1 ) };

      GraphicsContainer cont = g.BeginContainer(
        new Rectangle( 0, 0, 1, 1 ), new Rectangle( 0, 0, 1, 1 ),
        GraphicsUnit.Pixel );

      g.PageUnit = GraphicsUnit.Inch;
      g.TransformPoints( CoordinateSpace.Device, CoordinateSpace.Page, points );
      g.EndContainer( cont );

      double DEF_INCH_PIXEL = points[ 0 ].X;

      m_Proportions = new double[]
      {
        DEF_INCH_PIXEL / 75,     // Display
        DEF_INCH_PIXEL / 300,    // Document
        DEF_INCH_PIXEL,          // Inch
        DEF_INCH_PIXEL / 25.4f,  // Millimeter
        DEF_INCH_PIXEL / 2.54f,  // Centimeter
        1,                       // Pixel
        DEF_INCH_PIXEL / 72,     // Point
      };
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Converts value, stored in "from" units, to value in "to" units
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="from">Indicates units to convert from</param>
    /// <param name="to">Indicates units to convert to</param>
    /// <returns>Value stored in "to" units</returns>
    public double ConvertUnits( double value, PrintUnits from, PrintUnits to )
    {
      if( from == to )
        return value;
      
      return ConvertFromPixels( ConvertToPixels( value, from ), to );
    }
    /// <summary>
    /// Converts value, stored in "from" units, to pixels
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="from">Indicates units to convert from</param>
    /// <returns>Value stored in pixels</returns>
    public float ConvertToPixels( float value, PrintUnits from )
    {
      if( from == PrintUnits.Pixel )
        return value;
      
      return (float)(value * m_Proportions[ ( int )from ]); 
    }
    /// <summary>
    /// Converts value, stored in "from" units, to pixels
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="from">Indicates units to convert from</param>
    /// <returns>Value stored in pixels</returns>
    public double ConvertToPixels( double value, PrintUnits from )
    {
      if( from == PrintUnits.Pixel )
        return value;

      return value * m_Proportions[ ( int )from ];
    }
    /// <summary>
    /// Convert rectangle location and size to Pixels from specified 
    /// measure units
    /// </summary>
    /// <param name="rect">source rectangle</param>
    /// <param name="from">source rectangle measure units</param>
    /// <returns>Rectangle with Pixels</returns>
    public RectangleF ConvertToPixels( RectangleF rect, PrintUnits from )
    {
      float x = ( float )ConvertToPixels( rect.X, from );
      float y = ( float )ConvertToPixels( rect.Y, from );
      float w = ( float )ConvertToPixels( rect.Width, from );
      float h = ( float )ConvertToPixels( rect.Height, from );

      return new RectangleF( x, y, w, h );
    }
    /// <summary>
    /// Convert point from specified measure units to pixels
    /// </summary>
    /// <param name="point">source point for convert</param>
    /// <param name="from">measure units</param>
    /// <returns>point in pixels coordinates</returns>
    public PointF ConvertToPixels( PointF point, PrintUnits from )
    {
      float x = ( float )ConvertToPixels( point.X, from );
      float y = ( float )ConvertToPixels( point.Y, from );

      return new PointF( x, y );
    }
    /// <summary>
    /// Convert size from specified measure units to pixels
    /// </summary>
    /// <param name="size">source size</param>
    /// <param name="from">measure units</param>
    /// <returns>size in pixels</returns>
    public SizeF ConvertToPixels( SizeF size, PrintUnits from )
    {
      float w = ( float )ConvertToPixels( size.Width, from );
      float h = ( float )ConvertToPixels( size.Height, from );

      return new SizeF( w, h );
    }
    /// <summary>
    /// Converts value, stored in pixels, to value in "to" units
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="to">Indicates units to convert to</param>
    /// <returns>Value stored in "to" units</returns>
    public float ConvertFromPixels( float value, PrintUnits to )
    {
      if( to == PrintUnits.Pixel )
        return value;
      
      return (float)(value / m_Proportions[ ( int )to ]);
    }
    /// <summary>
    /// Converts value, stored in pixels, to value in "to" units
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="to">Indicates units to convert to</param>
    /// <returns>Value stored in "to" units</returns>
    public double ConvertFromPixels( double value, PrintUnits to )
    {
      if( to == PrintUnits.Pixel )
        return value;

      return value / m_Proportions[ ( int )to ];
    }
    /// <summary>
    /// Convert rectangle in Pixels into rectangle with specified 
    /// measure units
    /// </summary>
    /// <param name="rect">source rectangle in pixels units</param>
    /// <param name="to">convert to units</param>
    /// <returns>output Rectangle in specified units</returns>
    public RectangleF ConvertFromPixels( RectangleF rect, PrintUnits to )
    {
      float x = ( float )ConvertFromPixels( rect.X, to );
      float y = ( float )ConvertFromPixels( rect.Y, to );
      float w = ( float )ConvertFromPixels( rect.Width, to );
      float h = ( float )ConvertFromPixels( rect.Height, to );

      return new RectangleF( x, y, w, h );
    }
    /// <summary>
    /// Convert rectangle from pixels to specified units
    /// </summary>
    /// <param name="point">point in pixels units</param>
    /// <param name="to">convert to units</param>
    /// <returns>output Point in specified units</returns>
    public PointF ConvertFromPixels( PointF point, PrintUnits to )
    {
      float x = ( float )ConvertFromPixels( point.X, to );
      float y = ( float )ConvertFromPixels( point.Y, to );

      return new PointF( x, y );
    }
    /// <summary>
    /// Convert Size in pixels to size in specified measure units
    /// </summary>
    /// <param name="size">source size</param>
    /// <param name="to">convert to units</param>
    /// <returns>output size in specified measure units</returns>
    public SizeF ConvertFromPixels( SizeF size, PrintUnits to )
    {
      float w = ( float )ConvertFromPixels( size.Width, to );
      float h = ( float )ConvertFromPixels( size.Height, to );

      return new SizeF( w, h );
    }
    #endregion
  }
}
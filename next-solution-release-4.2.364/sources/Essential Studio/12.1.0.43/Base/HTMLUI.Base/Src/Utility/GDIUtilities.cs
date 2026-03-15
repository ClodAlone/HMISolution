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
using System.Collections;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.ComponentModel;
#endregion

namespace Syncfusion.HTMLUI.Base.Utility
{
  #region 3D Styles Enums
  /// <summary>
  /// Styles of canvas which emulate 3D effect on screen.
  /// </summary>
  public enum Canvas3DStyle
  {
    /// <summary>
    /// 3D canvas must be drawn in one line.
    /// </summary>
    Single,
    /// <summary>
    /// Raised canvas 3D style. Canvas will take two pixels from all sides.
    /// </summary>
    Raised,
    /// <summary>
    /// Raised canvas 3D style. Canvas will take two pixels from all sides.
    /// </summary>
    Upped,
    /// <summary>
    /// Used when two or more items must look like one 3D area. Such items
    /// are split by special 3D vertical line. Mostly used for headers drawing.
    /// </summary>
    Title,
    /// <summary>
    /// Flat style of 3D canvas. Single line border.
    /// </summary>
    Flat,
    /// <summary>
    /// Raised canvas style, but more simple.
    /// </summary>
    RaisedSimple
  }
  /// <summary>
  /// Specifies the styles which item can have for highlighting.
  /// </summary>
  public enum HightlightStyle
  {
    /// <summary>
    /// Mouse cursor to enter item area.
    /// </summary>
    Active,
    /// <summary>
    /// Item is selected, contains focus or is checked.
    /// </summary>
    Selected
  }
  #endregion

  /// <summary>
  /// Class representing different GDI capabilities.
  /// </summary>
  public sealed class GDIUtils : IDisposable
  {
    #region Class Members
    /// <summary>
    ///
    /// </summary>
    private SolidBrush m_brushDark = null;
    /// <summary>
    ///
    /// </summary>
    private SolidBrush m_brushDarkDark = null;
    /// <summary>
    ///
    /// </summary>
    private SolidBrush m_brushLight = null;
    /// <summary>
    ///
    /// </summary>
    private SolidBrush m_brushLightLight = null;
    /// <summary>
    ///
    /// </summary>
    private Color m_clrDark;
    /// <summary>
    ///
    /// </summary>
    private Color m_clrDarkDark;
    /// <summary>
    ///
    /// </summary>
    private Color m_clrLight;
    /// <summary>
    ///
    /// </summary>
    private Color m_clrLightLight;
    /// <summary>
    ///
    /// </summary>
    private Pen m_penDark;
    /// <summary>
    ///
    /// </summary>
    private Pen m_penDarkDark;
    /// <summary>
    ///
    /// </summary>
    private Pen m_penLight;
    /// <summary>
    ///
    /// </summary>
    private Pen m_penLightLight;
    #endregion

    #region Class Properties
    /// <summary>
    /// Gets or sets the dark.
    /// </summary>
    /// <value>The dark.</value>
    public Color  Dark
    {
      get
      {
        return m_clrDark;
      }
      set
      {
        if( value != m_clrDark || m_penDark == null )
        {
          m_clrDark = value;

          // Destroy old values.
          DisposeBrush( ref m_brushDark );
          DisposePen( ref m_penDark );

          m_brushDark = new SolidBrush( m_clrDark );
          m_penDark = new Pen( m_brushDark );
        }
      }
    }
    /// <summary>
    /// Gets or sets the dark dark.
    /// </summary>
    /// <value>The dark dark.</value>
    public Color  DarkDark
    {
      get
      {
        return m_clrDarkDark;
      }
      set
      {
        if( value != m_clrDarkDark || m_penDarkDark == null )
        {
          m_clrDarkDark = value;

          // Destroy old values.
          DisposeBrush( ref m_brushDarkDark );
          DisposePen( ref m_penDarkDark );

          m_brushDarkDark = new SolidBrush( m_clrDarkDark );
          m_penDarkDark = new Pen( m_brushDarkDark );
        }
      }
    }
    /// <summary>
    /// Gets or sets the light.
    /// </summary>
    /// <value>The light.</value>
    public Color  Light
    {
      get
      {
        return m_clrLight;
      }
      set
      {
        if( value != m_clrLight || m_penLight == null )
        {
          m_clrLight = value;

          // Destroy old values.
          DisposeBrush( ref m_brushLight );
          DisposePen( ref m_penLight );

          m_brushLight = new SolidBrush( m_clrLight );
          m_penLight = new Pen( m_brushLight );
        }
      }
    }
    /// <summary>
    /// Gets or sets the light light.
    /// </summary>
    /// <value>The light light.</value>
    public Color  LightLight
    {
      get
      {
        return m_clrLightLight;
      }
      set
      {
        if( value == m_clrLightLight || m_penLightLight == null )
        {
          m_clrLightLight = value;

          // Destroy old values.
          DisposeBrush( ref m_brushLightLight );
          DisposePen( ref m_penLightLight );

          m_brushLightLight = new SolidBrush( m_clrLightLight );
          m_penLightLight = new Pen( m_brushLightLight );
        }
      }
    }
    /// <summary>
    /// Gets the dark brush.
    /// </summary>
    /// <value>The dark brush.</value>
    public Brush  DarkBrush
    {
      get
      {
        if( m_brushDark == null )
          throw new ArgumentNullException( "m_brushDark" );

        return m_brushDark;
      }
    }
    /// <summary>
    /// Gets the dark dark brush.
    /// </summary>
    /// <value>The dark dark brush.</value>
    public Brush  DarkDarkBrush
    {
      get
      {
        if( m_brushDarkDark == null )
          throw new ArgumentNullException( "m_brushDarkDark" );

        return m_brushDarkDark;
      }
    }
    /// <summary>
    /// Gets the light brush.
    /// </summary>
    /// <value>The light brush.</value>
    public Brush  LightBrush
    {
      get
      {
        if( m_brushLight == null )
          throw new ArgumentNullException( "m_brushLight" );

        return m_brushLight;
      }
    }
    /// <summary>
    /// Gets the light light brush.
    /// </summary>
    /// <value>The light light brush.</value>
    public Brush  LightLightBrush
    {
      get
      {
        if( m_brushLightLight == null )
          throw new ArgumentNullException( "m_brushLightLight" );

        return m_brushLightLight;
      }
    }
    /// <summary>
    /// Gets the dark pen.
    /// </summary>
    /// <value>The dark pen.</value>
    public Pen    DarkPen
    {
      get
      {
        if( m_penDark == null )
          throw new ArgumentNullException( "m_penDark" );

        return m_penDark;
      }
    }
    /// <summary>
    /// Gets the dark dark pen.
    /// </summary>
    /// <value>The dark dark pen.</value>
    public Pen    DarkDarkPen
    {
      get
      {
        if( m_penDarkDark == null )
          throw new ArgumentNullException( "m_penDarkDark" );

        return m_penDarkDark;
      }
    }
    /// <summary>
    /// Gets the light pen.
    /// </summary>
    /// <value>The light pen.</value>
    public Pen    LightPen
    {
      get
      {
        if( m_penLight == null )
          throw new ArgumentNullException( "m_penLight" );

        return m_penLight;
      }
    }
    /// <summary>
    /// Gets the light light pen.
    /// </summary>
    /// <value>The light light pen.</value>
    public Pen    LightLightPen
    {
      get
      {
        if( m_penLightLight == null )
          throw new ArgumentNullException( "m_penLightLight" );

        return m_penLightLight;
      }
    }
    /// <summary>
    /// Gets the one line format.
    /// </summary>
    /// <value>The one line format.</value>
    static public StringFormat OneLineFormat
    {
      get
      {
        StringFormat format = new StringFormat();

        format.Alignment = StringAlignment.Center;
        format.LineAlignment = StringAlignment.Center;
        format.Trimming = StringTrimming.EllipsisCharacter;
        format.FormatFlags = StringFormatFlags.LineLimit;
        format.HotkeyPrefix = HotkeyPrefix.Show;

        return format;
      }
    }
    /// <summary>
    /// Gets the one line no trimming.
    /// </summary>
    /// <value>The one line no trimming.</value>
    static public StringFormat OneLineNoTrimming
    {
      get
      {
        StringFormat format = new StringFormat();

        format.Alignment = StringAlignment.Center;
        format.LineAlignment = StringAlignment.Center;
        format.Trimming = StringTrimming.None;
        format.FormatFlags = StringFormatFlags.LineLimit;
        format.HotkeyPrefix = HotkeyPrefix.Show;

        return format;
      }
    }
    #endregion

    #region Initialize/Finilize functions
    /// <summary>
    /// Default constructor.
    /// </summary>
    public GDIUtils()
    {
      this.Dark       = SystemColors.ControlDark;
      this.DarkDark   = SystemColors.ControlDarkDark;
      this.Light      = SystemColors.ControlLight;
      this.LightLight = SystemColors.ControlLightLight;
    }
    /// <summary>
    /// Destructor.
    /// </summary>
    ~GDIUtils()
    {
      this.Dispose();
    }
    /// <summary>
    /// Destroys all pens and brushes used by the class.
    /// </summary>
    public void Dispose()
    {
      DisposeBrush( ref m_brushDark );
      DisposePen( ref m_penDark );

      DisposeBrush( ref m_brushDarkDark );
      DisposePen( ref m_penDarkDark );

      DisposeBrush( ref m_brushLight );
      DisposePen( ref m_penLight );

      DisposeBrush( ref m_brushLightLight );
      DisposePen( ref m_penLightLight );
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="brush"></param>
    private void DisposeBrush( ref SolidBrush brush )
    {
      if( brush != null )
      {
        brush.Dispose();
        brush = null;
      }
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="pen"></param>
    private void DisposePen( ref Pen pen )
    {
      if( pen != null )
      {
        pen.Dispose();
        pen = null;
      }
    }
    #endregion

    #region Custom Drawing functions
    /// <summary>
    /// Draws a 3D Line. 3D Line is a simple line which contains one dark and one light line.
    /// Using dark and light line, we create an optical 3D effect.
    /// </summary>
    /// <param name="graph">Graphics object which is used by function to draw.</param>
    /// <param name="pnt1">Start point.</param>
    /// <param name="pnt2">End point.</param>
    public void Draw3DLine( Graphics graph, Point pnt1, Point pnt2 )
    {
      if( graph == null )
        throw new ArgumentNullException( "graph" );

      Pen penDark = this.DarkPen;
      Pen penLight = this.LightLightPen;

      Point[] arrPoint = { pnt1, pnt2 }; // Create copy of point input params.
      graph.DrawLine( m_penLight, pnt1, pnt2 ); // Draw first line.

      if( pnt1.X == pnt2.X )
      {
        arrPoint[0].X--;
        arrPoint[1].X--;
      }
      else if( pnt1.Y == pnt2.Y )
      {
        arrPoint[0].Y--;
        arrPoint[1].Y--;
      }
      else
      {
        arrPoint[0].X--; arrPoint[0].Y--;
        arrPoint[1].X--; arrPoint[1].Y--;
      }

      graph.DrawLine( penDark, arrPoint[0], arrPoint[1] );
    }

    /// <summary>
    /// Draws a 3D box according to style specification. There are four styles 
    /// available to draw.
    /// </summary>
    /// <param name="graph">Graphics object used for drawing.</param>
    /// <param name="rect">Box rectangle.</param>
    /// <param name="style">Style of box.</param>
    public void Draw3DBox( Graphics graph, Rectangle rect, Canvas3DStyle style )
    {
      if( graph == null )
        throw new ArgumentNullException( "graph" );

      Point pnt1 = Point.Empty
        , pnt2 = Point.Empty
        , pnt4 = Point.Empty;

      Point[] arrPoints = new Point[4];

      switch( style )
      {
        case Canvas3DStyle.Flat:
          graph.DrawRectangle( this.DarkPen, rect );
          break;

        case Canvas3DStyle.Title:
          #region Canvas 3DStyle - Title
          graph.DrawRectangle( this.DarkPen, rect );

          pnt1.X = rect.X+1; pnt1.Y = rect.Y+1;
          pnt2.X = rect.X+1; pnt2.Y = rect.Height-1;
          pnt4.X = rect.Width-1; pnt4.Y = rect.Y+1;

          // Set new values to array of pointers.
          arrPoints[0] = arrPoints[2] = pnt1;
          arrPoints[1] = pnt2; arrPoints[3] = pnt4;

          graph.DrawLines( LightLightPen, arrPoints );
          #endregion
          break;

        case Canvas3DStyle.Raised:
          #region Canvas 3DStyle Raised
          // Draw left upper corner.
          pnt1.X = rect.X; pnt1.Y = rect.Y;
          pnt2.X = rect.X + rect.Width; pnt2.Y = rect.Y;
          pnt4.X = rect.X; pnt4.Y = rect.Y + rect.Height;

          // Set new values to array of pointers.
          arrPoints[0] = arrPoints[2] = pnt1;
          arrPoints[1] = pnt2; arrPoints[3] = pnt4;

          graph.DrawLines( this.DarkPen, arrPoints );

          pnt1.X++; pnt1.Y++;
          pnt2.X-=2; pnt2.Y++;
          pnt4.X++; pnt4.Y-=2;

          // Set new values to array of pointers.
          arrPoints[0] = arrPoints[2] = pnt1;
          arrPoints[1] = pnt2; arrPoints[3] = pnt4;

          graph.DrawLines( this.DarkDarkPen, arrPoints );

          pnt1.X = rect.X + rect.Width; pnt1.Y = rect.Y + rect.Height;
          pnt2.X = rect.X; pnt2.Y = rect.Y + rect.Height;
          pnt4.X = rect.X + rect.Width; pnt4.Y = rect.Y;

          // Set new values to array of pointers.
          arrPoints[0] = arrPoints[2] = pnt1;
          arrPoints[1] = pnt2; arrPoints[3] = pnt4;

          graph.DrawLines( this.LightLightPen, arrPoints );

          pnt1.X--; pnt1.Y--;
          pnt2.X++; pnt2.Y--;
          pnt4.X--; pnt4.Y++;

          // Set new values to array of pointers.
          arrPoints[0] = arrPoints[2] = pnt1;
          arrPoints[1] = pnt2; arrPoints[3] = pnt4;

          graph.DrawLines( this.LightPen, arrPoints );
          #endregion
          break;
        case Canvas3DStyle.RaisedSimple :
          #region Canvas 3DStyle Raised
          arrPoints = new Point[ 3 ];
          
          pnt1.X = rect.X; 
          pnt1.Y = rect.Y;
          
          pnt2.X = rect.X + rect.Width; 
          pnt2.Y = rect.Y;
          pnt2.X--;
          
          pnt4.X = rect.X; 
          pnt4.Y = rect.Y + rect.Height;
          pnt4.Y--;
          
          arrPoints[ 0 ] = pnt2;
          arrPoints[ 1 ] = pnt1;
          arrPoints[ 2 ] = pnt4;
          graph.DrawLines( this.DarkPen, arrPoints );
          
          pnt1.X = rect.X + rect.Width;
          pnt1.Y = rect.Y + rect.Height;
          pnt1.X--;
          pnt1.Y--;
          
          pnt2.X = rect.X;
          pnt2.Y = rect.Y + rect.Height;
          pnt2.Y--;

          pnt4.X = rect.X + rect.Width; pnt4.Y = rect.Y;
          pnt4.X--;
          
          arrPoints[ 0 ] = pnt4;
          arrPoints[ 1 ] = pnt1;
          arrPoints[ 2 ] = pnt2;
          graph.DrawLines( this.LightPen, arrPoints );
          #endregion
          break;

        case Canvas3DStyle.Upped:
          #region Canvas 3D Style Upped
          // Draw left upper corner.
          pnt1.X = rect.X; pnt1.Y = rect.Y;
          pnt2.X = rect.X + rect.Width; pnt2.Y = rect.Y;
          pnt4.X = rect.X; pnt4.Y = rect.Y + rect.Height;

          // Set new values to array of pointers.
          arrPoints[0] = arrPoints[2] = pnt1;
          arrPoints[1] = pnt2; arrPoints[3] = pnt4;

          graph.DrawLines( this.LightLightPen, arrPoints );

          pnt1.X++; pnt1.Y++;
          pnt2.X-=2; pnt2.Y++;
          pnt4.X++; pnt4.Y-=2;

          // Set new values to array of pointers.
          arrPoints[0] = arrPoints[2] = pnt1;
          arrPoints[1] = pnt2; arrPoints[3] = pnt4;

          graph.DrawLines( this.LightPen, arrPoints );

          pnt1.X = rect.X + rect.Width; pnt1.Y = rect.Y + rect.Height;
          pnt2.X = rect.X; pnt2.Y = rect.Y + rect.Height;
          pnt4.X = rect.X + rect.Width; pnt4.Y = rect.Y;

          // Set new values to array of pointers.
          arrPoints[0] = arrPoints[2] = pnt1;
          arrPoints[1] = pnt2; arrPoints[3] = pnt4;

          graph.DrawLines( this.DarkDarkPen, arrPoints );

          pnt1.X--; pnt1.Y--;
          pnt2.X++; pnt2.Y--;
          pnt4.X--; pnt4.Y++;

          // Set new values to array of pointers.
          arrPoints[0] = arrPoints[2] = pnt1;
          arrPoints[1] = pnt2; arrPoints[3] = pnt4;

          graph.DrawLines( this.DarkPen, arrPoints );
          #endregion
          break;

        case Canvas3DStyle.Single:
          #region Canvas 3D Style Single
          // Draw left upper corner.
          pnt1.X = rect.X; pnt1.Y = rect.Y;
          pnt2.X = rect.Width; pnt2.Y = rect.Y;
          pnt4.X = rect.X; pnt4.Y = rect.Height;

          // Set new values to array of pointers.
          arrPoints[0] = arrPoints[2] = pnt1;
          arrPoints[1] = pnt2; arrPoints[3] = pnt4;

          graph.DrawLines( this.DarkPen, arrPoints );

          // Draw right lower corner.
          pnt1.X = rect.Width; pnt1.Y = rect.Height;
          pnt2.X = rect.X; pnt2.Y = rect.Height;
          pnt4.X = rect.Width; pnt4.Y = rect.Y;

          // Set new values to array of pointers.
          arrPoints[0] = arrPoints[2] = pnt1;
          arrPoints[1] = pnt2; arrPoints[3] = pnt4;

          graph.DrawLines( this.LightLightPen, arrPoints );
          #endregion
          break;
      }
    }

    /// <summary>
    /// Draws an active rectangle by blue colors.
    /// </summary>
    /// <param name="graph">Graphic context where rectangle must be drawn.</param>
    /// <param name="rect">Destination rectangle.</param>
    /// <param name="state">State of rectangle. Influence on colors by which rectangle
    /// will be drawn.</param>
    public void DrawActiveRectangle( Graphics graph, Rectangle rect, HightlightStyle state )
    {
      if( graph == null )
        throw new ArgumentNullException( "graph" );

      Color highlight = ( state == HightlightStyle.Active ) ?
        ColorUtil.VSNetBackgroundColor : ColorUtil.VSNetSelectionColor;

      Color highBorder = SystemColors.Highlight;

      SolidBrush high = new SolidBrush( highlight );
      SolidBrush bord = new SolidBrush( highBorder );
      Pen penBord = new Pen( bord );

      graph.FillRectangle( high, rect );
      graph.DrawRectangle( penBord, rect );

      penBord.Dispose();
      bord.Dispose();
      high.Dispose();
    }

    /// <summary>
    /// Draws an active rectangle.
    /// </summary>
    /// <param name="graph">Graphic context where rectangle must be drawn.</param>
    /// <param name="rect">Destination rectangle.</param>
    /// <param name="state">State of rectangle. Influence on colors by which rectangle
    /// will be drawn.</param>
    /// <param name="bSubRect">Indicates whether we need rectangle width and height fix.</param>
    public void DrawActiveRectangle( Graphics graph, Rectangle rect, HightlightStyle state, bool bSubRect )
    {
      if( graph == null )
        throw new ArgumentNullException( "graph" );

      Rectangle rc = ( bSubRect ) ? FixRectangleHeightWidth( rect ) : rect;
      DrawActiveRectangle( graph, rc, state );
    }

    /// <summary>
    /// Makes rectangle's width and height less on one pixel. This is useful because
    /// in some cases, a rectangle's last pixels does not show.
    /// </summary>
    /// <param name="rect">Rectangle whose contexts must be fixed.</param>
    /// <returns>
    /// A new rectangle object which contains fixed values.
    /// </returns>
    public static Rectangle FixRectangleHeightWidth( Rectangle rect )
    {
      return new Rectangle( rect.X, rect.Y, rect.Width - 1, rect.Height - 1 );
    }
    /// <summary>
    /// Fixes the width of the rectangle height.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <returns>A new rectangle object which contains fixed values.</returns>
    public static Rectangle FixRectangleHeightWidth( int x, int y, int width, int height )
    {
      return new Rectangle( x, y, width - 1, height - 1 );
    }
    #endregion

    #region Class Helper Functions
    /// <summary>
    /// Calculates X and Y coordinates to place object at the center of the rectangle.
    /// </summary>
    /// <param name="rect">Destination rectangle.</param>
    /// <param name="sz">Object size.</param>
    /// <returns>Point class with X and Y coordinates of center.</returns>
    static public Point CalculateCenter( Rectangle rect, Size sz )
    {
      Point pnt1 = new Point( 0 );

      pnt1.X = rect.X + ( rect.Width - sz.Width ) / 2;
      pnt1.Y = rect.Y + ( rect.Height - sz.Height ) / 2;

      return pnt1;
    }
    #endregion

    #region Static Public Methods
    /// <summary>
    /// Overloaded. Draws 3D-style rectangle.
    /// </summary>
    /// <param name="g">Graphics canvas where rectangle must be drawn.</param>
    /// <param name="rc">Rectangle coordinates.</param>
    /// <param name="clrTL">Color of top left corner of rectangle.</param>
    /// <param name="clrBR">Color of bottom right corner of rectangle.</param>
    static public void Draw3DRect( Graphics g, Rectangle rc, Color clrTL, Color clrBR )
    {
      Draw3DRect( g, rc.Left, rc.Top, rc.Width, rc.Height, clrTL, clrBR );
    }
    /// <summary>
    /// Draws 3D-style rectangle.
    /// </summary>
    /// <param name="g">Graphics object.</param>
    /// <param name="x">X coordinate of top left corner of the rectangle.</param>
    /// <param name="y">Y coordinate of top left corner of the rectangle.</param>
    /// <param name="width">Width of the rectangle.</param>
    /// <param name="height">Height of the rectangle.</param>
    /// <param name="clrTL">Color that is to be used for top left corner drawing.</param>
    /// <param name="clrBR">Color that is to be used for bottom right corner drawing.</param>
    static public void Draw3DRect( Graphics g, int x, int y, int width, int height, Color clrTL, Color clrBR )
    {
      if( g == null )
        throw new ArgumentNullException( "g" );

      using( Brush brushTL = new SolidBrush( clrTL ) )
      {
        using( Brush brushBR = new SolidBrush( clrBR ) )
        {
          g.FillRectangle( brushTL, x, y, width - 1, 1 );
          g.FillRectangle( brushTL, x, y, 1, height - 1 );
          g.FillRectangle( brushBR, x + width, y, -1, height );
          g.FillRectangle( brushBR, x, y + height, width, -1 );
        }
      }
    }
    /// <summary>
    /// Overloaded. Creates bitmap with specified size and infill. Bitmap sent by parameter
    /// will be used to fill destination area. If bitmap is less than destination
    /// area, it will be tiled.
    /// </summary>
    /// <param name="sz"></param>
    /// <param name="bitmap">Infill of output bitmap.</param>
    /// <returns>Returns bitmap filled by input bitmap.</returns>
    static public Bitmap GetTileBitmap( Size sz, Bitmap bitmap )
    {
      if( sz == Size.Empty )
        throw new ArgumentOutOfRangeException( "sz", "Destination area size cannot be zero." );

      return GetTileBitmap( new Rectangle( Point.Empty, sz ), bitmap );
    }
    /// <summary>
    /// Creates bitmap with specified size and infill. Bitmap sent by parameter
    /// will be used to fill destination area. If bitmap is less then destination
    /// area, it will be tiled.
    /// </summary>
    /// <param name="rcDest">Destination size.</param>
    /// <param name="bitmap">Infill of output bitmap.</param>
    /// <returns>Returns bitmap filled by input bitmap.</returns>
    static public Bitmap GetTileBitmap( Rectangle rcDest, Bitmap bitmap )
    {
      if( bitmap == null )
        throw new ArgumentNullException( "bitmap" );

      if( rcDest == Rectangle.Empty )
        throw new ArgumentOutOfRangeException( "rcDest", "Destination rectangle size cannot be zero" );

      Bitmap tiledBitmap = new Bitmap( rcDest.Width, rcDest.Height );

      using( Graphics g = Graphics.FromImage( tiledBitmap ) )
      {
        for( int i = 0; i < tiledBitmap.Width; i += bitmap.Width )
        {
          for( int j = 0; j < tiledBitmap.Height; j += bitmap.Height )
          {
            g.DrawImage( bitmap, new Point( i, j ) );
          }
        }
      }

      return tiledBitmap;
    }
    /// <summary>
    /// Overloaded. Draws arrow glyph at the center of the rectangle. Width and height of arrow will be 5 and 3.
    /// For arrow drawing, SystemColors.Highlight color will be used.
    /// TIP: Use an odd number for the arrowWidth and arrowWidth/2+1 for the arrowHeight
    /// so that the arrow gets the same pixel number on the left and on the right and
    /// get symmetrically painted.
    /// </summary>
    /// <param name="g">Graphics object.</param>
    /// <param name="rc">Destination rectangle.</param>
    /// <param name="up">Direction of arrow.</param>
    static public void DrawArrowGlyph( Graphics g, Rectangle rc, bool up )
    {
      DrawArrowGlyph( g, rc, up, SystemColors.Highlight );
    }
    /// <summary>
    /// Draws arrow glyph at the center of rectangle. Width and height of arrow will be 5 and 3.
    /// TIP: Use an odd number for the arrowWidth and arrowWidth/2+1 for the arrowHeight
    /// so that the arrow gets the same pixel number on the left and on the right and
    /// get symmetrically painted.
    /// </summary>
    /// <param name="g">Graphics object.</param>
    /// <param name="rc">Destination rectangle.</param>
    /// <param name="up">Direction of arrow.</param>
    /// <param name="clr">Color which must be used for drawing.</param>
    static public void DrawArrowGlyph( Graphics g, Rectangle rc, bool up, Color clr )
    {
      DrawArrowGlyph( g, rc, up, new SolidBrush( clr ) );
    }
    /// <summary>
    /// Draws arrow glyph at the center of the rectangle. Width and height of arrow will be 5 and 3.
    /// TIP: Use an odd number for the arrowWidth and arrowWidth/2+1 for the arrowHeight
    /// so that the arrow gets the same pixel number on the left and on the right and
    /// get symmetrically painted.
    /// </summary>
    /// <param name="g">Graphics object.</param>
    /// <param name="rc">Destination rectangle.</param>
    /// <param name="up">Direction of arrow.</param>
    /// <param name="brush">Brush which must be used for drawing.</param>
    static public void DrawArrowGlyph( Graphics g, Rectangle rc, bool up, Brush brush )
    {
      // Draw arrow glyph with the default size of 5 pixel wide and 3 pixel high.
      DrawArrowGlyph( g, rc, 5, 3, up, brush );
    }
    /// <summary>
    /// Draws arrow glyph at the center of rectangle.
    /// TIP: Use an odd number for the arrowWidth and arrowWidth/2+1 for the arrowHeight
    /// so that the arrow gets the same pixel number on the left and on the right and
    /// get symmetrically painted.
    /// </summary>
    /// <param name="g">Graphics object.</param>
    /// <param name="rc">Destination Rectangle. Arrow glyph will be placed at the center
    /// of the destination rectangle.</param>
    /// <param name="width">Width of arrow.</param>
    /// <param name="height">Height of arrow.</param>
    /// <param name="isUp">Direction arrow.</param>
    /// <param name="clr">Color which must be used by arrow draw function.</param>
    static public void DrawArrowGlyph( Graphics g, Rectangle rc, int width, int height, bool isUp, Color clr )
    {
      DrawArrowGlyph( g, rc, width, height, isUp, new SolidBrush( clr ) );
    }
    /// <summary>
    /// Draws arrow glyph at the center of the rectangle.
    /// TIP: Use an odd number for the arrowWidth and arrowWidth/2+1 for the arrowHeight
    /// so that the arrow gets the same pixel number on the left and on the right and
    /// get symmetrically painted.
    /// </summary>
    /// <param name="g">Graphics object</param>
    /// <param name="rc">Destination rectangle. Arrow Glyph will be placed at the center
    /// of the destination rectangle.</param>
    /// <param name="width">Width of arrow.</param>
    /// <param name="height">Height of arrow.</param>
    /// <param name="isUp">Direction arrow.</param>
    /// <param name="brush">Brush which must be used for drawing.</param>
    static public void DrawArrowGlyph( Graphics g, Rectangle rc, int width, int height, bool isUp, Brush brush )
    {
      if( g == null )
        throw new ArgumentNullException( "g" );

      if( rc == Rectangle.Empty )
        throw new ArgumentException( "Destination rectangle cannot be empty" );

      if( rc.Width < width )
        throw new ArgumentOutOfRangeException( "width", "Must be less then destination rectangle width." );

      if( rc.Height < height )
        throw new ArgumentOutOfRangeException( "height", "Must be less then destination rectangle height." );

      if( brush == null )
        throw new ArgumentNullException( "brush" );

      Point[] pts = new Point[3];
      int yMiddle = rc.Top + rc.Height/2 - height/2+1;
      int xMiddle = rc.Left + rc.Width/2;
      int yArrowHeight  = yMiddle + height;
      int xArrowWidthR  = xMiddle + width/2;
      int xArrowWidthL  = xMiddle - width/2;

      if( isUp )
      {
        pts[0] = new Point( xMiddle, yMiddle-2 );
        pts[1] = new Point( xArrowWidthL - 1, yArrowHeight - 1 );
        pts[2] = new Point( xArrowWidthR + 1, yArrowHeight - 1 );

      }
      else
      {
        pts[0] = new Point( xArrowWidthL, yMiddle );
        pts[1] = new Point( xArrowWidthR + 1,  yMiddle );
        pts[2] = new Point( xMiddle, yArrowHeight );
      }

      g.FillPolygon( brush, pts );
    }

    #endregion
  }
}
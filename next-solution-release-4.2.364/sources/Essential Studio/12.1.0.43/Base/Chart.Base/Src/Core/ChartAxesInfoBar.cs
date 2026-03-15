#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
  /// <summary>
	/// The ChartAxesInfoBar display the labels between the rectangular axes.
  /// </summary>
	/// <example>
	/// ChartArea.AxesInfoBar.Visible = true;
	/// ChartArea.AxesInfoBar.Text = "";
	/// ChartArea.AxesInfoBar.ShowBorder = true;
	/// </example>
  public sealed class ChartAxesInfoBar
  {
    #region Members
    private bool m_visible = false;
    private string m_text = "";
    private Font m_font = new Font( "Verdana", 10 );
    private Color m_textColor = Color.Black;
    private bool m_showBorder = true;
    private LineInfo m_border = new LineInfo();
    private Hashtable m_groupingCells = new Hashtable();
    private StringFormat m_stringFormat = (StringFormat)(StringFormat.GenericDefault.Clone());
    #endregion

    #region Events
    /// <summary>
    /// Occurs when <see cref="Syncfusion.Windows.Forms.Chart.ChartAxesInfoBar"/> properties was changed.
    /// </summary>
    public event EventHandler Changed;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets value indicates that bar is visible or not.
    /// </summary>
    public bool Visible
    {
      get
      {
        return m_visible;
      }
      set
      {
        if( m_visible != value )
        {
          m_visible = value;
          OnChanged( EventArgs.Empty );
        }
      }
    }
    /// <summary>
    /// Gets or sets text.
    /// </summary>
    public string Text
    {
      get
      {
        return m_text;
      }
      set
      {
        if( m_text != value )
        {
          m_text = value;
          OnChanged( EventArgs.Empty );
        }
      }
    }
    /// <summary>
    /// Gets or sets <see cref="System.Drawing.Font"/>.
    /// </summary>
    public Font Font
    {
      get
      {
        return m_font;
      }
      set
      {
        if( m_font != value )
        {
          m_font = value;
          OnChanged( EventArgs.Empty );
        }
      }
    }
    /// <summary>
    /// Gets or sets text color.
    /// </summary>
    public Color TextColor
    {
      get
      {
        return m_textColor;
      }
      set
      {
        if( m_textColor != value )
        {
          m_textColor = value;
          OnChanged( EventArgs.Empty );
        }
      }
    }
    /// <summary>
    /// Gets or sets value indicates that need to render border.
    /// </summary>
    public bool ShowBorder
    {
      get
      {
        return m_showBorder;
      }
      set
      {
        if( m_showBorder != value )
        {
          m_showBorder = value;
          OnChanged( EventArgs.Empty );
        }
      }
    }
    /// <summary>
    /// Gets or sets <see cref="Syncfusion.Windows.Forms.Chart.LineInfo"/> of the border.
    /// </summary>
    public LineInfo Border
    {
      get
      {
        return m_border;
      }
      set
      {
        if( m_border != value )
        {
          m_border = value;
          OnChanged( EventArgs.Empty );
        }
      }
    }
    /// <summary>
    /// Gets or sets <see cref="System.Drawing.StringFormat"/> of the text to render.
    /// </summary>
    public StringFormat TextFormat
    {
      get
      {
        return m_stringFormat;
      }
      set
      {
        if( m_stringFormat != value )
        {
          m_stringFormat = value;
          OnChanged( EventArgs.Empty );
        }
      }
    }
    /// <summary>
    /// Gets or sets grouping cell's text by its column and row index.
    /// </summary>
    public string this[ int col, int row ]
    {
      get
      {
        return (string)m_groupingCells[ GetCellKey( col, row ) ];
      }
      set
      {
        int key = GetCellKey( col, row );

        if( !String.Equals( m_groupingCells[ key ], value ) )
        {
          if( value == String.Empty )
          {
            m_groupingCells.Remove( key );
          }
          else
          {
            m_groupingCells[ key ] = value;
          }

          OnChanged( EventArgs.Empty );
        }
      }
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Creates instance of the ChartAxesInfoBar.
    /// </summary>
    internal ChartAxesInfoBar()
    {
      m_stringFormat.Alignment = StringAlignment.Center;
      m_stringFormat.LineAlignment = StringAlignment.Center;
    }
    #endregion

    #region Public methods
		/// <summary>
		/// Draws to the specified graphics.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="xAxis">The x axis.</param>
		/// <param name="yAxis">The y axis.</param>
    internal void Draw( Graphics g, ChartAxis xAxis, ChartAxis yAxis )
    {
      float coefX = yAxis.OpposedPosition ? 1 : -1;
      float coefY = xAxis.OpposedPosition ? -1 : 1;

      RectangleF rect = CorrectRect(new RectangleF( yAxis.Location.X, xAxis.Location.Y,
        coefX*yAxis.TickAndLabelsDimension, coefY*xAxis.TickAndLabelsDimension ));

      using( SolidBrush textBrush = new SolidBrush( m_textColor ))
      {
        if( m_text != null && m_text != "" )
        {
          g.DrawString( m_text, m_font, textBrush, rect, m_stringFormat );
      
          if( m_showBorder )
          {
            g.DrawRectangle( m_border.Pen, rect.X, rect.Y, rect.Width, rect.Height );
          }
        }

        float currY = xAxis.TickAndLabelsDimension;

        for( int i = 0; i < xAxis.GroupingLabelsRowsDimensions.Length; i ++ )
        {
          float yDim = xAxis.GroupingLabelsRowsDimensions[ i ];

          if( yDim != currY )
          {
            float currX = yAxis.TickAndLabelsDimension;

            for( int j = 0; j < yAxis.GroupingLabelsRowsDimensions.Length; j ++ )
            {
              float xDim = yAxis.GroupingLabelsRowsDimensions[ j ];

              if( xDim != currX )
              {
                RectangleF cellRect = CalcCellRect( CorrectRect( new RectangleF( yAxis.Location.X, xAxis.Location.Y, coefX*currX, coefY*currY )),
                  new SizeF( coefX*(xDim - currX), coefY*(yDim - currY) ));

                string text = (string)m_groupingCells[ GetCellKey( i , j ) ];
                if( text != null )
                {

                  if( m_showBorder )
                  {
                    g.DrawRectangle( m_border.Pen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height );
                  }

                  g.DrawString( text, m_font, textBrush, cellRect, m_stringFormat );
                }

                currX = xDim;
              }
            }

            currY = yDim;
          }
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="g"></param>
    /// <param name="xAxis"></param>
    /// <param name="yAxis"></param>
    internal void Draw( Graphics3D g, ChartAxis xAxis, ChartAxis yAxis )
    {
      float coefX = yAxis.OpposedPosition ? 1 : -1;
      float coefY = xAxis.OpposedPosition ? -1 : 1;

      RectangleF rect = CorrectRect(new RectangleF( yAxis.Location.X, xAxis.Location.Y,
        coefX*yAxis.TickAndLabelsDimension, coefY*xAxis.TickAndLabelsDimension ));

      ArrayList resultPolygons = new ArrayList();

      BrushInfo textBrush = new BrushInfo( m_textColor );

      if( m_text != null && m_text != "" )
      {
        GraphicsPath gp = new GraphicsPath();
        gp.AddString( m_text, m_font.FontFamily, (int)m_font.Style, 
          RenderingHelper.GetFontSizeInPixels( m_font ), rect, m_stringFormat );

        resultPolygons.Add( Path3D.FromGraphicsPath( gp, 0, textBrush ));
      
        if( m_showBorder )
        {
          GraphicsPath gpr = new GraphicsPath();
          gpr.AddRectangle( rect );
          resultPolygons.Add( Path3D.FromGraphicsPath( gpr, 0, (BrushInfo)null, m_border.Pen ));
        }
      }

      float currY = xAxis.TickAndLabelsDimension;

      for( int i = 0; i < xAxis.GroupingLabelsRowsDimensions.Length; i ++ )
      {
        float yDim = xAxis.GroupingLabelsRowsDimensions[ i ];

        if( yDim != currY )
        {
          float currX = yAxis.TickAndLabelsDimension;

          for( int j = 0; j < yAxis.GroupingLabelsRowsDimensions.Length; j ++ )
          {
            float xDim = yAxis.GroupingLabelsRowsDimensions[ j ];

            if( xDim != currX )
            {
              RectangleF cellRect = CalcCellRect( CorrectRect( new RectangleF( yAxis.Location.X, xAxis.Location.Y, coefX*currX, coefY*currY )),
                new SizeF( coefX*(xDim - currX), coefY*(yDim - currY) ));

              string text = (string)m_groupingCells[ GetCellKey( i , j ) ];

              if( text != null )
              {
                GraphicsPath gp = new GraphicsPath();
                gp.AddString( text, m_font.FontFamily, (int)m_font.Style, 
                  RenderingHelper.GetFontSizeInPixels( m_font ), cellRect, m_stringFormat );

                resultPolygons.Add( Path3D.FromGraphicsPath( gp, 0, textBrush ));
      
                if( m_showBorder )
                {
                  GraphicsPath gpr = new GraphicsPath();
                  gpr.AddRectangle( cellRect );
                  resultPolygons.Add( Path3D.FromGraphicsPath( gpr, 0, (BrushInfo)null, m_border.Pen ));
                }
              }

              currX = xDim;
            }
          }

          currY = yDim;
        }
      }

      g.AddPolygon( new Path3DCollect( (Polygon[])resultPolygons.ToArray( typeof( Polygon ))));
    }
    #endregion

    #region Helper methdos
    /// <summary>
    /// 
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    private static int GetCellKey( int col, int row )
    {
      return (col << 0x10) | row;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    private static RectangleF CorrectRect( RectangleF rect )
    {
      return new RectangleF( Math.Min( rect.Left, rect.Right ), Math.Min( rect.Top, rect.Bottom ),
        Math.Abs( rect.Width ), Math.Abs( rect.Height ));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mainRect"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private static RectangleF CalcCellRect( RectangleF mainRect, SizeF size )
    {
      RectangleF result = RectangleF.Empty;

      if( size.Width > 0 )
      {
        result.X = mainRect.Right;
        result.Width = size.Width;
      }
      else
      {
        result.X = mainRect.Left + size.Width;
        result.Width = -size.Width;
      }

      if( size.Height > 0 )
      {
        result.Y = mainRect.Bottom;
        result.Height = size.Height;
      }
      else
      {
        result.Y = mainRect.Top + size.Height;
        result.Height = -size.Height;
      }

      return result;
    }
    /// <summary>
    /// Raise changed event with event arguments.
    /// </summary>
    /// <param name="e">Event arguments to raise event.</param>
    private void OnChanged( EventArgs e )
    {
      if( Changed != null )
      {
        Changed( this, e );
      }
    }
    #endregion
  }
}

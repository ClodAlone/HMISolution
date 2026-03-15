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
using System.ComponentModel;
using System.Drawing;
using Syncfusion.Drawing;
using Syncfusion.Documentation;
using System.Drawing.Drawing2D;
using System.Collections;

namespace Syncfusion.Windows.Forms.Chart
{
    public enum ChartStripLineZorder
    {
        /// <summary>
        /// The StripLine will be rendered over chart.
        /// </summary>
        Over,

        /// <summary>
        /// The StripLine will be rendered behind chart.
        /// </summary>
        Behind
    }
  /// <summary>
  /// This class specifies information on rendering a strip line. A strip line is a horizontal or vertical band rendered on the background of a chart 
  /// to indicate some areas of interest.
  /// </summary>
  [ Serializable ]
  public sealed class ChartStripLine
  {
    #region Members
    private double m_start;
    private double m_end;
    private double m_width = 1;
    private double m_fixedWidth = 0;
    private double m_period = 2;
    private string m_text = "StripLine";
    private Font m_font = new Font( "Verdana", 10 );
    private Color m_textColor = SystemColors.ControlText;
    private ContentAlignment m_textAlign = ContentAlignment.MiddleCenter;
    private bool m_vertical = false;
    private bool m_startAtAxisPosition = false;
    private double m_offset = 0;
    private BrushInfo m_interior = new BrushInfo(Color.White);
    private Image m_backImage = null;
    private bool m_enabled = false;
    private ChartStripLineZorder m_zOrder = ChartStripLineZorder.Behind;

    #endregion

    #region Events
		/// <summary>
		/// Occurs when the properties was changed.
		/// </summary>
		/// <internalonly/>
    [DocumentationExclude()]
    public event EventHandler Changed;
    #endregion

    #region Properties
    [DefaultValue(ChartStripLineZorder.Behind), Description("Indicates the depth order of StripLine"), NotifyParentProperty(true)]
    public ChartStripLineZorder ZOrder
    {
        get
        {
            return m_zOrder;
        }

        set
        {
            if (m_zOrder != value)
            {
                m_zOrder = value;
                RaiseChanged(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Indicates whether the strip line will start at the start of the axis range.
    /// </summary>
    public bool StartAtAxisPosition
    {
      get
      {
        return m_startAtAxisPosition;
      }
      set
      {
        if( m_startAtAxisPosition != value )
        {
          m_startAtAxisPosition = value;
          RaiseChanged( this, EventArgs.Empty );
        }
      }
    }
    /// <summary>
    /// Indicates whether the text of strip line will be drawn vertical.
    /// </summary>
    public bool Vertical
    {
      get
      {
        return m_vertical;
      }
      set
      {
        if( m_vertical != value )
        {
          m_vertical = value;
          RaiseChanged( this, EventArgs.Empty );
        }
      }
    }
    /// <summary>
    /// Gets or sets the alignment of text that is to be rendered within a strip line.
    /// </summary>
    public ContentAlignment TextAlignment
    {
      get
      {
        return m_textAlign;
      }
      set
      {
        if( m_textAlign != value )
        {
          m_textAlign = value;
          RaiseChanged( this, EventArgs.Empty );
        }
      }
    }
    /// <summary>
    /// Gets or sets the font with which text associated with this strip line is to be rendered.
    /// </summary>
    public Font Font
    {
      get
      {
        return m_font;
      }
      set
      {
        if(m_font != value)
        {
          m_font = value;
          RaiseChanged( this, EventArgs.Empty );
        }
      }
    }
    /// <summary>
    /// Gets or sets the color of the text rendered with this strip line.
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
          RaiseChanged( this, EventArgs.Empty );
        }
      }
    }
    /// <summary>
    /// Gets or sets the text associated with this strip line.
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
          RaiseChanged( this, EventArgs.Empty );
        }
      }
    }
    /// <summary>
    /// Gets or sets the background image associated with this strip line.
    /// </summary>
    [ DefaultValue( null ) ]
    public Image BackImage
    {
      get
      {
        return m_backImage;
      }
      set
      {
        if( m_backImage != value )
        {
          m_backImage = value;
          RaiseChanged( this, EventArgs.Empty );
        }
      }
    }
    /// <summary>
    /// Gets or sets the interior brush information for this strip line.
    /// </summary>
    public BrushInfo Interior
    {
      get
      {
        return m_interior;
      }
      set
      {
        if( m_interior != value )
        {
          m_interior = new BrushInfo( value );
          RaiseChanged( this, EventArgs.Empty );
        }
      }
    }

		/// <summary>
		/// Indicates whether the strip line is enabled.
		/// </summary>
		public bool Enabled
		{
			get
			{
				return m_enabled;
			}
			set
			{
				if (m_enabled != value)
				{
					m_enabled = value;
					RaiseChanged(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the offset of the strip line if the chart's Primary X axis is of type DateTime and StartAtAxisPosition is True.
		/// </summary>
		public TimeSpan DateOffset
		{
			get
			{
				return DateTime.FromOADate(m_offset) - DateTime.FromOADate(0);
			}
			set
			{
				if (Offset != DateTime.FromOADate(0).Add(value).ToOADate())
				{
					Offset = DateTime.FromOADate(0).Add(value).ToOADate();
					RaiseChanged(this, EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets or sets the offset of the strip line if the chart's Primary X axis is of type double and StartAtAxisPosition is True.
		/// </summary>
		public double Offset
		{
			get
			{
				return m_offset;
			}
			set
			{
				if (m_offset != value)
				{
					m_offset = value;
					RaiseChanged(this, EventArgs.Empty);
				}
			}
		}

    /// <summary>
    /// Gets or sets the period over which this strip line appears when the value is DateTime.
    /// </summary>
    public TimeSpan PeriodDate
    {
      get
      {
        return DateTime.FromOADate( m_period ) - DateTime.FromOADate( 0 );
      }
      set
      {
        if( m_period != DateTime.FromOADate( 0 ).Add( value ).ToOADate() )
        {
          m_period = DateTime.FromOADate( 0 ).Add( value ).ToOADate();
          RaiseChanged( this, EventArgs.Empty );
        }
      }
    }
		/// <summary>
		/// Gets or sets the period over which this strip line appears.
		/// </summary>
		public double Period
		{
			get
			{
				return m_period;
			}
			set
			{
				if (m_period != value)
				{
					m_period = value;
					RaiseChanged(this, EventArgs.Empty);
				}
			}
		}

    /// <summary>
    /// Gets or sets the width of each strip line as a TimeSpan.
    /// </summary>
    public TimeSpan WidthDate
    {
      get
      {
        return DateTime.FromOADate( m_width ) - DateTime.FromOADate( 0 );
      }

      set
      {
        if( m_width != DateTime.FromOADate( 0 ).Add( value ).ToOADate() )
        {
          m_width = DateTime.FromOADate( 0 ).Add( value ).ToOADate();
          RaiseChanged( this, EventArgs.Empty );
        }
      }
    }
    /// <summary>
    /// Gets or sets the width of each strip line.
    /// </summary>
    public double Width
    {
      get
      {
        return m_width;
      }
      set
      {
        if( m_width != value )
        {
          m_width = value;
          RaiseChanged( this, EventArgs.Empty );
        }
      }
    }

    /// <summary>
    /// Gets or sets the fixed width of each strip line. This property value represents real value not range between two Chart Points.
    /// </summary>
    [DefaultValue(0), Description("Gets or sets the fixed width of each strip line. This property value represents real value not range between two Chart Points.")]
    public double FixedWidth
    {
        get
        {
            return m_fixedWidth;
        }
        set
        {
            if (m_fixedWidth != value)
            {
                m_fixedWidth = value;
                RaiseChanged(this, EventArgs.Empty);
            }
        }
    }    

    /// <summary>
    /// Gets or sets the date from which the strip line is to start.
    /// </summary>
    public DateTime StartDate
    {
      get
      {
        return DateTime.FromOADate( m_start );
      }
      set
      {
        if( m_start != value.ToOADate())
        {
          m_start = value.ToOADate();
          RaiseChanged( this, EventArgs.Empty );
        }
      }
    }
    /// <summary>
    /// Gets or sets the date after which the strip line should not be displayed.
    /// </summary>
    public DateTime EndDate
    {
      get
      {
        return DateTime.FromOADate( m_end );
      }
      set
      {
        if( m_end != value.ToOADate())
        {
          m_end = value.ToOADate();
          RaiseChanged( this, EventArgs.Empty );
        }
      }
    }
    /// <summary>
    /// Gets or sets the start of this strip line.
    /// </summary>
    public double Start
    {
      get
      {
        return m_start;
      }
      set
      {
        if( m_start != value )
        {
          m_start = value;
          RaiseChanged( this, EventArgs.Empty );
        }
      }
    }
    /// <summary>
    /// Gets or sets the end range of this strip line.
    /// </summary>
    public double End
    {
      get
      {
        return m_end;
      }
      set
      {
        if( m_end != value )
        {
          m_end = value;
          RaiseChanged( this, EventArgs.Empty );
        }
      }
    }
    #endregion

    #region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartStripLine"/> class.
		/// </summary>
    public ChartStripLine()
    {
    }
    #endregion

    #region Implementation
		/// <summary>
		/// Draws the striplines to the specified <see cref="Graphics"/>.
		/// </summary>
		/// <param name="graph">The ChartGraph<see cref="Graphics"/>.</param>
		/// <param name="bounds">The array of bounds.</param>
		internal void Draw(ChartGraph graph, RectangleF[] bounds)
		{
			using (StringFormat strFormat = this.CreateStringFormat())
			{
				using (SolidBrush sb = new SolidBrush(m_textColor))
				{
					foreach (RectangleF rect in bounds)
					{
						graph.PushTranfsorm();

						if (m_backImage != null)
						{
							graph.DrawImage(m_backImage, rect);
						}
						else
						{
							graph.DrawRect(m_interior, null, rect);
						}

						if (m_vertical)
                        {
                            PostScript.PostScriptGraphics.StripLine = true;
                            graph.MultiplyTransform(this.CreateVerticalTransform(rect));
                            graph.DrawString(m_text, m_font, sb, new RectangleF(0, 0, rect.Height, rect.Width), strFormat);
						}
						else
						{
							graph.DrawString(m_text, m_font, sb, rect, strFormat);
						}

						graph.PopTransform();
					}
				}
			}
		}
		/// <summary>
		/// Draws the specified <see cref="Graphics3D"/>.
		/// </summary>
		/// <param name="g3d">The <see cref="Graphics3D"/>.</param>
		/// <param name="bounds">The bounds.</param>
		/// <param name="z">The Z coordinate of <see cref="ChartStripLine"/>.</param>
		/// <returns></returns>
		internal Polygon Draw(Graphics3D g3d, RectangleF[] bounds, float z)
		{
			ArrayList polygons = new ArrayList(2 * bounds.Length);

			using (StringFormat strFormat = this.CreateStringFormat())
			{
				foreach (RectangleF rect in bounds)
				{
					if (m_backImage != null)
					{
						polygons.Add(Image3D.FromImage(m_backImage, Rectangle.Round(rect), z));
					}
					else
					{
						polygons.Add(new Polygon(new Vector3D[]{ new Vector3D( rect.Left, rect.Top, z ),
						                                         new Vector3D( rect.Right, rect.Top, z ),
						                                         new Vector3D( rect.Right, rect.Bottom, z ),
						                                         new Vector3D( rect.Left, rect.Bottom, z ) }, m_interior));
					}

					if (m_text != null)
					{
						GraphicsPath textGp = new GraphicsPath();

						if (m_vertical)
						{
							RenderingHelper.AddTextPath(textGp, g3d.Graphics, m_text, m_font,
								new RectangleF(0, 0, rect.Height, rect.Width), strFormat);
							textGp.Transform(this.CreateVerticalTransform(rect));
						}
						else
						{
							RenderingHelper.AddTextPath(textGp, g3d.Graphics, m_text, m_font, rect, strFormat);
						}

						polygons.Add(Path3D.FromGraphicsPath(textGp, z, new SolidBrush(m_textColor)));
					}
				}
			}

			return polygons.Count == 0 ? null : new Path3DCollect(polygons.ToArray(typeof(Polygon)) as Polygon[]);
		}
		/// <summary>
		/// Creates the string format.
		/// </summary>
		/// <returns></returns>
		private StringFormat CreateStringFormat()
		{
			StringFormat result = new StringFormat();

			result.FormatFlags = StringFormatFlags.NoClip;
			result.Trimming = StringTrimming.None;

			StringAlignment verticalAlignment = StringAlignment.Center;
			StringAlignment horizontalAlignment = StringAlignment.Center;

			#region Get Alignment
			switch (m_textAlign)
			{
				case ContentAlignment.BottomCenter:
					verticalAlignment = StringAlignment.Far;
					horizontalAlignment = StringAlignment.Center;
					break;
				case ContentAlignment.BottomLeft:
					verticalAlignment = StringAlignment.Far;
					horizontalAlignment = StringAlignment.Near;
					break;
				case ContentAlignment.BottomRight:
					verticalAlignment = StringAlignment.Far;
					horizontalAlignment = StringAlignment.Far;
					break;
				case ContentAlignment.MiddleCenter:
					verticalAlignment = StringAlignment.Center;
					horizontalAlignment = StringAlignment.Center;
					break;
				case ContentAlignment.MiddleLeft:
					verticalAlignment = StringAlignment.Center;
					horizontalAlignment = StringAlignment.Near;
					break;
				case ContentAlignment.MiddleRight:
					verticalAlignment = StringAlignment.Center;
					horizontalAlignment = StringAlignment.Far;
					break;
				case ContentAlignment.TopCenter:
					verticalAlignment = StringAlignment.Near;
					horizontalAlignment = StringAlignment.Center;
					break;
				case ContentAlignment.TopLeft:
					verticalAlignment = StringAlignment.Near;
					horizontalAlignment = StringAlignment.Near;
					break;
				case ContentAlignment.TopRight:
					verticalAlignment = StringAlignment.Near;
					horizontalAlignment = StringAlignment.Far;
					break;
			}
			#endregion

			result.LineAlignment = verticalAlignment;
			result.Alignment = horizontalAlignment;

			return result;
		}
		/// <summary>
		/// Creats the mirrow transform.
		/// </summary>
		/// <param name="rect">The rect.</param>
		/// <returns></returns>
		private Matrix CreateVerticalTransform(RectangleF rect)
		{
			Matrix matrix = new Matrix();

			matrix.Translate(rect.Left, rect.Bottom);
			matrix.Rotate(-90);

			return matrix;

			//return new Matrix(rect, new PointF[]{
			//  new PointF( rect.Bottom, rect.Left ), 
			//  new PointF( rect.Bottom, rect.Right ),
			//  new PointF( rect.Top, rect.Left )});
		}
		/// <summary>
		/// Raises the changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void RaiseChanged(object sender, EventArgs e)
    {
      if( Changed != null )
      {
        Changed( sender, e );
      }
    }
    #endregion
  }
}
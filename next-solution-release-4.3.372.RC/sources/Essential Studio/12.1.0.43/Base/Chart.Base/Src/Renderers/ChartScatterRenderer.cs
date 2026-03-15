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
using System.Drawing;
using System.Collections;
using Syncfusion.Drawing;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Chart.Renderers
{
	/// <internalonly/>
	[Syncfusion.Documentation.DocumentationExclude()]
  internal class ScatterRenderer : ChartSeriesRenderer
	{
		#region Properties
		/// <summary>
		/// Gets count of require Y values of the points.
		/// </summary>
		/// <value></value>
		protected override int RequireYValuesCount
		{
			get
			{
				return 1;
			}
		}
		#endregion

		#region Constructor
		/// <internalonly/>
    public ScatterRenderer( ChartSeries series )
      : base( series )
    {
    }
    #endregion

    #region Public methods
		/// <summary>
		/// Renders the specified args.
		/// </summary>
		/// <param name="args">The args.</param>
		public override void Render(ChartRenderArgs2D args)
		{
             #region Draw connectors
			if (m_series.ScatterConnectType != ScatterConnectType.None)
			{
                IndexRange[] ranges = this.ScatterCalculateUnEmptyRanges(new IndexRange(0, m_series.Points.Count - 1));
				Graphics g = (args.Graph as ChartGDIGraph).Graphics;

				for (int i = 0; i < ranges.Length; i++)
				{
					if ((ranges[i].To - ranges[i].From) > 0)
					{
						Render(g, ranges[i].From, ranges[i].To - ranges[i].From + 1);
					}
				}
			}
      #endregion

			#region Symbol rendering
			IndexRange visibleRange = this.CalculateVisibleRange();
			ChartStyledPoint[] styledPoints = this.PrepearePoints();

			for (int i = visibleRange.From; i <= visibleRange.To; i++)
			{
				ChartStyledPoint point = styledPoints[i];

				if (point.IsVisible && point.Style.Symbol.Shape == ChartSymbolShape.None)
				{
					GraphicsPath gp = new GraphicsPath();
					PointF ptF = args.GetPoint(point.X, point.YValues[0]);
					Size sz = point.Style.Symbol.Size;

					gp.AddEllipse(ptF.X - sz.Width / 2, ptF.Y - sz.Height / 2, sz.Width, sz.Height);

					args.Graph.DrawPath(this.GetBrush(point.Index), point.Style.Symbol.Border.GdipPen, gp);

					if (this.Chart.NeedRegionUpdate)
					{
						Size sblSize = point.Style.Symbol.Size;
						RectangleF rect = new RectangleF(ptF.X - 0.5f * sblSize.Width,
							ptF.Y - 0.5f * sblSize.Height, sblSize.Width, sblSize.Height);
						this.Chart.ChartRegions.Add(new ChartRegion(new Region(rect),
							this.Chart.Series.IndexOf(m_series), point.Index, point.ToolTip, "Symbol"));
					}
				}
			}
			#endregion
		}

        protected IndexRange[] ScatterCalculateUnEmptyRanges(IndexRange vrange)
        {
            ArrayList ranges = new ArrayList();
            int lastIndex = 0;

            for (int i = vrange.From, ci = vrange.To + 1; i < ci; i++)
            {
                if (this.IsVisiblePoint(m_series.Points[i]))
                {

                    if (lastIndex == -1)
                    {
                        lastIndex = i;
                    }
                    else if (m_series.Points.Count == i + 1 && this.IsVisiblePoint(m_series.Points[i]))
                    {
                        ranges.Add(new IndexRange(lastIndex, i));
                    }
                }

                else
                {
                    ranges.Add(new IndexRange(lastIndex, i - 1));
                    lastIndex = -1;
                }
            }

            if (ranges.Count == 0)
            {
                ranges.Add(vrange);
            }

            return (IndexRange[])ranges.ToArray(typeof(IndexRange));
        }
    /// <internalonly/>
    private void Render(Graphics g, int from, int count)
    {
      if( m_series.ScatterConnectType == ScatterConnectType.None ) return;
      int serIndex = Chart.Series.IndexOf( m_series );
      ChartStyleInfo serStyle = SeriesStyle;
      Pen pen = serStyle.GdipPen.Clone() as Pen;
      pen.Color = serStyle.Interior.BackColor;
      Pen borderpen = serStyle.GdipPen as Pen;      
      BrushInfo brush = this.GetBrush();
      bool dropPoints = Chart.DropSeriesPoints;
      double tension = m_series.ScatterSplineTension;
      if( m_series.ScatterConnectType == ScatterConnectType.Line ) tension = 0.0d;

      ChartPointWithIndex[] cpwiA = new ChartPointWithIndex[count];
      ChartPointWithIndex[] bsplA, bsplAE;

      for( int i = 0; i < count; i ++ )
      {
        cpwiA[i] = new ChartPointWithIndex( m_series.Points[ from + i ], from + i );
      }

      canonicalSpline( cpwiA, tension, Chart.Series3D, out bsplA, out bsplAE);
      
      ///////////////////////////////////////////////////////////////////////////
			GraphicsPath m_path = new GraphicsPath();
      ///////////////////////////////////////////////////////////////////////////

      ArrayList pointsF = new ArrayList(m_series.Points.Count);
      ArrayList pointsEF = new ArrayList(m_series.Points.Count);

      for( int i = 0, end = bsplA.Length; i < end; i += 4 )
      {
        ChartPoint p0 = bsplA[i].Point;
        ChartPoint p1 = bsplA[i+1].Point;
        ChartPoint p2 = bsplA[i+2].Point;
        ChartPoint p3 = bsplA[i+3].Point;

        PointF pf0 = new PointF( GetXFromValue( p0, 0 ), GetYFromValue( p0, 0 ) );
        PointF pf1 = new PointF( GetXFromValue( p1, 0 ), GetYFromValue( p1, 0 ) );
        PointF pf2 = new PointF( GetXFromValue( p2, 0 ), GetYFromValue( p2, 0 ) );
        PointF pf3 = new PointF( GetXFromValue( p3, 0 ), GetYFromValue( p3, 0 ) );

        pointsF.Add( pf0 );
        pointsF.Add( pf1 );
        pointsF.Add( pf2 );
        pointsF.Add( pf3 );
        m_path.AddBezier( pf0, pf1, pf2, pf3 );
        //m_path.AddPolygon( new PointF[]{ pf0, pf1, pf2, pf3 });
      }

      for( int i = 0, end = bsplAE.Length; i < end; i += 4 )
      {
        ChartPoint p0 = bsplAE[i].Point;
        ChartPoint p1 = bsplAE[i+1].Point;
        ChartPoint p2 = bsplAE[i+2].Point;
        ChartPoint p3 = bsplAE[i+3].Point;

        PointF pf0 = new PointF( GetXFromValue( p0, 0 ), GetYFromValue( p0, 0 ) );
        PointF pf1 = new PointF( GetXFromValue( p1, 0 ), GetYFromValue( p1, 0 ) );
        PointF pf2 = new PointF( GetXFromValue( p2, 0 ), GetYFromValue( p2, 0 ) );
        PointF pf3 = new PointF( GetXFromValue( p3, 0 ), GetYFromValue( p3, 0 ) );

        pointsEF.Add( pf0 );
        pointsEF.Add( pf1 );
        pointsEF.Add( pf2 );
        pointsEF.Add( pf3 );
      }

      PointF[] points = (PointF[]) pointsF.ToArray( typeof(PointF) );
      PointF[] pointsE = (PointF[]) pointsEF.ToArray( typeof(PointF) );
      //m_path.AddBeziers( points );  //microsoft bug


      #region Draw Shadow

      if( serStyle.DisplayShadow && !Chart.Series3D)
      {
        pen.Color = serStyle.ShadowInterior.ForeColor;
        GraphicsPath shadPth = (GraphicsPath)m_path.Clone();
        shadPth.Transform( new Matrix( 1f, 0f, 0f, 1f, serStyle.ShadowOffset.Width, serStyle.ShadowOffset.Height ) );
        g.DrawPath( pen, shadPth );
        // Reset the pen color
        pen.Color = serStyle.Interior.BackColor;
      }

      #endregion

      #region Draw lines
      if( Chart.Series3D )
      {
        CalculateStepPointsForSeries3D( ref points );
        CalculateStepPointsForSeries3D( ref pointsE );

        Region topRgn = Draw3DBeziers( g, points, pointsE, GetSeriesOffset(), brush, borderpen );
        
        if( this.Chart.NeedRegionUpdate )
        {
          this.ChartArea.ChartRegions.Add(new ChartRegion(topRgn, serIndex, 
            this.GetToolTip(), "Line Chart Region") );
        }
      }
      else
      {
        g.DrawPath( pen, m_path );
      }
      #endregion

      #region Sets region
      if( this.Chart.NeedRegionUpdate )
      {
        for( int i = 0; i <= points.Length; i+=4 )
        {
		  i = (points.Length == i) ? i - 1 : i;	
          int cpIndex = bsplA[i].Index;
          ChartStyleInfo style =  GetStyleAt( cpIndex ); 
          string s = GetToolTip( cpIndex );
          this.ChartArea.ChartRegions.Add( new ChartRegion( this.GetRegionFromCircle( points[i], style.HitTestRadius ),
            serIndex, cpIndex, s,"Line Chart Renderer") );
            
          if( (i - 1) >= 0 )
          {
            GraphicsPath pieceOfLine = new GraphicsPath();
            pieceOfLine.AddPolygon( new PointF[]
                {
                  new PointF( (points[i-1].X + points[i].X)/2, (points[i-1].Y + points[i].Y)/2 + style.HitTestRadius ),
                  new PointF( points[i].X, points[i].Y + style.HitTestRadius ),
                  new PointF( points[i].X, points[i].Y - style.HitTestRadius ),
                  new PointF( (points[i-1].X + points[i].X)/2, (points[i-1].Y + points[i].Y)/2 - style.HitTestRadius )
                } );
            this.ChartArea.ChartRegions.Add( new ChartRegion( new Region( pieceOfLine ), serIndex, cpIndex, s ,"Line Chart Renderer") );              
          }
          if( (i + 1) < cpwiA.Length )
          {
            GraphicsPath pieceOfLine = new GraphicsPath();
            pieceOfLine.AddPolygon( new PointF[]
                {
                  new PointF( (points[i+1].X + points[i].X)/2, (points[i+1].Y + points[i].Y)/2 + style.HitTestRadius ),
                  new PointF( points[i].X, points[i].Y + style.HitTestRadius ),
                  new PointF( points[i].X, points[i].Y - style.HitTestRadius ),
                  new PointF( (points[i+1].X + points[i].X)/2, (points[i+1].Y + points[i].Y)/2 - style.HitTestRadius )
                } );
            this.ChartArea.ChartRegions.Add( new ChartRegion( new Region( pieceOfLine ), serIndex, cpIndex, s ,"Line Chart Renderer") );              
          }
        }
      }
      #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="g"></param>
    public override void Render( Graphics3D g )
    {
      IndexRange[] ranges = this.UnEmptyRanges;

      #region Draw connectors
      for (int i = 0; i < ranges.Length; i++)
      {
        if( (ranges[i].To - ranges[i].From)  > 0 )
        {
          Render( g, ranges[i].From, ranges[i].To - ranges[i].From + 1 );
        }
      }
      #endregion

      #region Draw default symbols
			IndexRange visibleRange = this.CalculateVisibleRange();
			ChartStyledPoint[] styledPoints = this.PrepearePoints();

			for (int i = visibleRange.From; i <= visibleRange.To; i++)
          {
				ChartStyledPoint styledPoint = styledPoints[i];
				ChartStyleInfo style = styledPoint.Style;

                if (styledPoint.IsVisible && styledPoint.Style.Symbol.Shape == ChartSymbolShape.None)
            {
              GraphicsPath gp = new GraphicsPath();
					Vector3D pt = GetSymbolVector(styledPoint);
              Size sz = style.Symbol.Size;

              gp.AddEllipse((float)(pt.X - sz.Width / 2), (float)(pt.Y - sz.Height / 2), sz.Width, sz.Height);
                                 
              Path3D p3d = Path3D.FromGraphicsPath(gp, pt.Z, this.GetBrush(styledPoint.Index), style.Symbol.Border.GdipPen);
			  p3d.RegionData = new ChartRegionData(Chart.Series.IndexOf(m_series), styledPoint.Index, styledPoint.ToolTip, "Symbol");
              g.AddPolygon(p3d);
            }
          }
      #endregion
    }

		/// <summary>
		/// Renders the adornment.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="point">The point.</param>
		protected override void RenderAdornment(Graphics g, ChartStyledPoint point)
		{
			base.RenderAdornment(g, point);
		}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="g"></param>
    /// <param name="from"></param>
    /// <param name="count"></param>
    private void Render( Graphics3D g, int from, int count )
    {
      if( m_series.ScatterConnectType == ScatterConnectType.None ) return;
      int serIndex = Chart.Series.IndexOf( m_series );
      ChartStyleInfo serStyle = SeriesStyle;
      Pen pen = serStyle.GdipPen.Clone() as Pen;
      pen.Color = serStyle.Interior.BackColor;
      Pen borderpen = serStyle.GdipPen as Pen;      
      BrushInfo brush = this.GetBrush();
      int dsc = SPLINE_DIGITIZATION;
      float fd = GetPlaceDepth();
      float dpth = GetSeriesDepth();
      float bd = fd + dpth;
      bool dropPoints = Chart.DropSeriesPoints;
      double tension = m_series.ScatterSplineTension;
      if( m_series.ScatterConnectType == ScatterConnectType.Line ) tension = 0.0d;

      ChartPointWithIndex[] cpwiA = new ChartPointWithIndex[count];
      ChartPointWithIndex[] bsplA, bsplAE;
      for( int i = 0; i < count; i ++ )
      {
        cpwiA[i] = new ChartPointWithIndex( m_series.Points[ from + i ], from + i );
      }
      canonicalSpline( cpwiA, tension, Chart.Series3D, out bsplA, out bsplAE);

      for( int i = 0; i < bsplAE.Length - 3; i += 4 )
      {
        ChartPoint p0 = bsplAE[i].Point;
        ChartPoint p1 = bsplAE[i+1].Point;
        ChartPoint p2 = bsplAE[i+2].Point;
        ChartPoint p3 = bsplAE[i+3].Point;

        PointF pf0 = new PointF( GetXFromValue( p0, 0 ), GetYFromValue( p0, 0 ) );
        PointF pf1 = new PointF( GetXFromValue( p1, 0 ), GetYFromValue( p1, 0 ) );
        PointF pf2 = new PointF( GetXFromValue( p2, 0 ), GetYFromValue( p2, 0 ) );
        PointF pf3 = new PointF( GetXFromValue( p3, 0 ), GetYFromValue( p3, 0 ) );

				PointF[] ps = ChartMath.InterpolateBezier(pf0, pf1, pf2, pf3, dsc);

        Vector3D v1 = new Vector3D( pf0.X, pf0.Y, fd );
        Vector3D v2 = new Vector3D( pf0.X, pf0.Y, bd );

        Pen pnInter = new Pen( SeriesStyle.Interior.BackColor );

        ChartRegionData crd = null;
        if(	Chart.NeedRegionUpdate )
        {
          string s = GetToolTip();
          crd = new ChartRegionData( Chart.Series.IndexOf( m_series ), s,"Scatter Chart	Region" );
        }

        for( int j = 0; j < ps.Length; j ++ )
        {
          Vector3D v3 = new Vector3D( ps[ j ].X, ps[ j ].Y, bd );
          Vector3D v4 = new Vector3D( ps[ j ].X, ps[ j ].Y, fd );
                     
          Polygon plg = new Polygon(new Vector3D[] { v1, v2, v3, v4 }, this.GetBrush(), pnInter);
          plg.RegionData = crd;
          g.AddPolygon( plg );

          v1 = v4;
          v2 = v3;
        }
      }
    }

		/// <summary>
		/// Brush information is retrieved from the style associated with the index of the point to be rendered.
		/// It is then changed for special cases such as when automatic highlighting is enabled.
		/// </summary>
		/// <returns>
		/// Brush information that is to be used for filling elements displayed at this index.
		/// </returns>
    protected override BrushInfo GetBrush()
    {
      BrushInfo brushInfo = base.GetBrush();
      ChartColumnConfigItem config = m_series.ConfigItems.ColumnItem;

      if (Chart.Model.ColorModel.AllowGradient)
      {
        if (config.ShadingMode == ChartColumnShadingMode.PhongCylinder)
        {
          brushInfo = this.GetPhongInterior(brushInfo, config.LightColor, config.LightAngle, config.PhongAlpha);
        }
      }

      return brushInfo;
    }
		/// <summary>
		/// Brush information is retrieved from the style associated with the index of the point to be rendered.
		/// It is then changed for special cases such as when automatic highlighting is enabled.
		/// </summary>
		/// <param name="index">Index value of the point for which the brush information is required.</param>
		/// <returns>
		/// Brush information that is to be used for filling elements displayed at this index.
		/// </returns>
    protected override BrushInfo GetBrush(int index)
    {
      BrushInfo brushInfo = base.GetBrush(index);
      ChartColumnConfigItem config = m_series.ConfigItems.ColumnItem;

      if (Chart.Model.ColorModel.AllowGradient)
      {
        if (config.ShadingMode == ChartColumnShadingMode.PhongCylinder)
        {
          brushInfo = this.GetPhongInterior(brushInfo, config.LightColor, config.LightAngle, config.PhongAlpha);
        }
      }

      return brushInfo;
    }

		/// <summary>
		/// Draws the icon on the legend.
		/// </summary>
		/// <param name="g">Instance of <see cref="Graphics"/>.</param>
		/// <param name="bounds">Bounds of icon.</param>
		/// <param name="isShadow">If is true method draws the shadow.</param>
		/// <param name="shadowColor"><see cref="Color"/> of shadow.</param>
    public override void DrawIcon(Graphics g, Rectangle bounds, bool isShadow, Color shadowColor)
    {
      if (isShadow)
      {
        using (SolidBrush br = new SolidBrush(shadowColor))
        {
          g.FillRectangle(br, bounds);
        }
      }
      else
      {
          BrushInfo brushInfo = this.SeriesStyle.Interior;          
          ChartColumnConfigItem config = m_series.ConfigItems.ColumnItem;

        if (Chart.Model.ColorModel.AllowGradient)
        {
          if (config.ShadingMode == ChartColumnShadingMode.PhongCylinder)
          {
            brushInfo = this.GetPhongInterior(brushInfo, config.LightColor, config.LightAngle, config.PhongAlpha);
          }
        }

        BrushPaint.FillRectangle(g, bounds, brushInfo);
        g.DrawRectangle(SeriesStyle.GdipPen, bounds);
      }
    }    
    #endregion
  }
}
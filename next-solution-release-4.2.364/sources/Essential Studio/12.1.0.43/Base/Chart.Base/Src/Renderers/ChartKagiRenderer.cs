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
using Syncfusion.Drawing;
using System.Collections;
namespace Syncfusion.Windows.Forms.Chart.Renderers
{
  /// <summary>
  /// 
  /// </summary>
	internal class KagiRenderer : ChartSeriesRenderer
	{
		#region Properties
		/// <summary>
		/// Gets count of required Y values of the points.
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
		/// <summary>
		/// Initializes a new instance of the <see cref="KagiRenderer"/> class.
		/// </summary>
		/// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
    public KagiRenderer( ChartSeries series )
      : base( series )
    {
    }
    #endregion
    
    #region Public methods
		/// <summary>
		/// In the base <see cref="ChartSeriesRenderer"/> it does not do anything. In derived classes this function does
		/// the rendering.
		/// </summary>
		/// <param name="g">The graphics object that is to be used for rendering.</param>
    public override void Render( Graphics g )
    {
      int serIndex = Chart.Series.IndexOf( m_series );
      bool series3D = Chart.Series3D;
      bool inverted = m_series.ConfigItems.StepItem.Inverted;
      Pen pen = SeriesStyle.GdipPen.Clone() as Pen;
      BrushInfo brush = SeriesStyle.Interior;
      Pen borderpen = m_series.GetOfflineStyle().GdipPen as Pen;
                                 
      Color priceUpColor = m_series.ConfigItems.FinancialItem.PriceUpColor;
			Color priceDownColor = m_series.ConfigItems.FinancialItem.PriceDownColor;
      double reversalAmount = m_series.ReversalAmount;
      bool reversalIsPercent = m_series.ReversalIsPercent;
      if ( reversalIsPercent )
        reversalAmount /= 100.0;
      

    // FORMING KAGI POINTS
      // minimums and maximums variables of previous vertical line          
      double prevMaxY = (float)m_series.Points[0].YValues[0];
      double prevMinY = (float)m_series.Points[0].YValues[0];
      // minimums and maximums variables of current vertical line          
      double curMaxY = (float)m_series.Points[0].YValues[0];
      double curMinY = (float)m_series.Points[0].YValues[0];
      // lists which will hold Kagi step points 
      ArrayList listChartPoints = new ArrayList( );
      ArrayList listPoints = new ArrayList( );
      ArrayList listColors = new ArrayList( );
      double tempX;
      double prevYVal = m_series.Points[0].YValues[0];
      double yVal;
      //variable which indicates direction of current vertical line
      bool goingUp = ( m_series.Points[0].YValues[0] <= m_series.Points[1].YValues[0] );
      //float verticalX = this.GetPointFromIndex( 0 ).X;
      double verticalX = m_series.Points[0].X;
      Color curColor = goingUp ? priceUpColor : priceDownColor;
      
      listPoints.Add( this.GetPointFromIndex( 0 ) );
      listColors.Add( curColor );
      for( int i = 1; i < m_series.Points.Count ; i++ )
      {
          yVal = m_series.Points[i].YValues[0];
          
          if (reversalIsPercent)
          {
            // REVERSING LINE
            if ((yVal - curMaxY) / Math.Abs(curMaxY) <= -reversalAmount)
            {
              if( goingUp)
              { // STARTING NEXT VERITCAL LINE
                ChartPoint cpt = new ChartPoint( verticalX, curMaxY );
                listChartPoints.Add( cpt );
                listPoints.Add( new PointF( GetXFromValue( cpt, 0 ) , GetYFromValue( cpt, 0 ) ) );
                listColors.Add( curColor );

                tempX = m_series.Points[i].X;
                cpt = new ChartPoint( tempX , curMaxY );
                listChartPoints.Add( cpt );
                listPoints.Add( new PointF( GetXFromValue( cpt, 0 ) , GetYFromValue( cpt, 0 ) ) );
                listColors.Add( curColor );
                goingUp = false;
                verticalX = tempX;
                
                //initializing previous vert. line maximum and minimums 
                prevMaxY = curMaxY;
                prevMinY = curMinY;
                
                //initializinf current vert. line maximum and minimums
                curMaxY = prevMaxY;
                curMinY = yVal;
              }
            }

            if ((yVal - curMinY) / Math.Abs(curMinY) >= reversalAmount)
            {
              if( !goingUp)
              { // STARTING NEXT VERITCAL LINE
                ChartPoint cpt = new ChartPoint( verticalX, curMinY );
                listChartPoints.Add( cpt );
                listPoints.Add( new PointF( GetXFromValue( cpt, 0 ) , GetYFromValue( cpt, 0 ) ) );
                listColors.Add( curColor );
                tempX = m_series.Points[i].X;
                cpt = new ChartPoint( tempX, curMinY );
                listChartPoints.Add( cpt );
                listPoints.Add( new PointF( GetXFromValue( cpt, 0 ) , GetYFromValue( cpt, 0 ) ) );
                listColors.Add( curColor );
                goingUp = true;
                verticalX = tempX;

                //initializing previous vert. line maximum and minimums 
                prevMaxY = curMaxY;
                prevMinY = curMinY;
                
                //initializinf current vert. line maximum and minimums
                curMaxY = yVal;
                curMinY = prevMinY;
              }
            }
          }
          else
          {
            // REVERSING LINE
            if ( ( yVal - curMaxY ) <= - reversalAmount )
            {
              if( goingUp)
              { // STARTING NEXT VERITCAL LINE
                ChartPoint cpt = new ChartPoint( verticalX, curMaxY );
                listChartPoints.Add( cpt );
                listPoints.Add( new PointF( GetXFromValue( cpt, 0 ) , GetYFromValue( cpt, 0 ) ) );
                listColors.Add( curColor );

                tempX = m_series.Points[i].X;
                cpt = new ChartPoint( tempX, curMaxY );
                listChartPoints.Add( cpt );
                listPoints.Add( new PointF( GetXFromValue( cpt, 0 ) , GetYFromValue( cpt, 0 ) ) );
                listColors.Add( curColor );

                goingUp = false;
                verticalX = tempX;

                //initializing previous vert. line maximum and minimums 
                prevMaxY = curMaxY;
                prevMinY = curMinY;
                
                //initializinf current vert. line maximum and minimums
                curMaxY = prevMaxY;
                curMinY = yVal;
              }
            }
            
            if ( ( yVal - curMinY ) >=  reversalAmount )
            {
              if( !goingUp)
              { // STARTING NEXT VERITCAL LINE
                ChartPoint cpt = new ChartPoint( verticalX, curMinY );
                listChartPoints.Add( cpt );
                listPoints.Add( new PointF( GetXFromValue( cpt, 0 ) , GetYFromValue( cpt, 0 ) ) );
                listColors.Add( curColor );
                tempX = m_series.Points[i].X;

                cpt = new ChartPoint( tempX, curMinY );
                listChartPoints.Add( cpt );
                listPoints.Add( new PointF( GetXFromValue( cpt, 0 ) , GetYFromValue( cpt, 0 ) ) );
                listColors.Add( curColor );
                goingUp = true;
                verticalX = tempX;

                //initializing previous vert. line maximum and minimums 
                prevMaxY = curMaxY;
                prevMinY = curMinY;
                
                //initializinf current vert. line maximum and minimums
                curMaxY = yVal;
                curMinY = prevMinY;
              }
            }
          }

          
          if( goingUp )
          {// CHANGING LINE COLOR, IF WE PASS THE PREVIOUS VERTICAL LINE MAXIMUM          
            if( (yVal > prevMaxY) && (curMinY < prevMaxY) && ( curColor != priceUpColor ) )
            {
              ChartPoint cpt = new ChartPoint( verticalX, prevMaxY );
              listChartPoints.Add( cpt );
              listPoints.Add( new PointF( GetXFromValue( cpt, 0 ) , GetYFromValue( cpt, 0 ) ) );
              curColor = priceUpColor;
              listColors.Add( curColor );
            }
          }
          else
          {// CHANGING LINE COLOR, IF WE PASS THE PREVIOUS VERTICAL LINE MINIMUM
            if( (yVal < prevMinY) && (curMaxY > prevMinY) && ( curColor != priceDownColor ) )
            {
              ChartPoint cpt = new ChartPoint( verticalX, prevMinY );
              listChartPoints.Add( cpt );
              listPoints.Add( new PointF( GetXFromValue( cpt, 0 ) , GetYFromValue( cpt, 0 ) ) );
              curColor = priceDownColor;
              listColors.Add( curColor );
            }
          }

          // MINIMUMS AND MAXIMUMS OF CURRENT VERTICAL LINE          
          if( yVal < curMinY )
          {
            curMinY = yVal;
          }
          if( yVal > curMaxY )
          {
            curMaxY = yVal;
          }
          
          prevYVal = yVal;
      }
      // Adding Points which wasnt processed
      ChartPoint cpL = m_series.Points[ m_series.Points.Count - 1 ];
      ChartPoint cpT = new ChartPoint( verticalX, cpL.YValues );
      listChartPoints.Add( cpT );
      listPoints.Add( new PointF( GetXFromValue( cpT, 0 ), GetYFromValue( cpT, 0 ) ) );                
      listColors.Add( curColor );
    //END FORMING KAGI POINTS
      
      
      PointF[] stepPoints = new PointF[listPoints.Count];
      Color[] pointColors = new Color[listColors.Count];
      for( int i = 0 ; i < listPoints.Count ; i++ )
      {
        stepPoints[i] = (PointF)listPoints[i];
        pointColors[i] = (Color)listColors[i];
      }

      #region Draw Shadow

      if( m_series.GetOfflineStyle().DisplayShadow && !series3D )
      {
        PointF[] shadowPts = new PointF[stepPoints.Length];

        for( int i = 0 ; i < stepPoints.Length ; i++ )
        {
          ChartStyleInfo style = m_series.GetOfflineStyle();
          shadowPts[i] = new PointF( stepPoints[i].X + style.ShadowOffset.Width, stepPoints[i].Y + style.ShadowOffset.Height );
        }
        pen.Color = m_series.GetOfflineStyle().ShadowInterior.ForeColor;
        g.DrawLines( pen, shadowPts );
      }

      #endregion
        
      if( series3D )
      {
        SizeF offset = GetSeriesOffset();
        CalculateStepPointsForSeries3D(ref stepPoints);
          
        Region topRgn = Draw3DLines( g, stepPoints, offset, brush, borderpen, pointColors);

        if( this.Chart.NeedRegionUpdate )
        {
          this.ChartArea.ChartRegions.Add( new ChartRegion( topRgn , serIndex, GetToolTip(), 
            "Kagi Chart Region") );
        }
      }

      else
      {
        for( int i = 0 ; i < stepPoints.Length - 1; i++ )
        {
          pen.Color = pointColors[i];
          g.DrawLine( pen, stepPoints[i], stepPoints[i+1] );          
        }
      }
    }
		/// <summary>
		/// In the base <see cref="ChartSeriesRenderer"/> it does not do anything. In derived classes this function does
		/// the rendering.
		/// </summary>
		/// <param name="g">The graphics object that is to be used for rendering.</param>
    public override void Render( Graphics3D g )
    {
      int serIndex = Chart.Series.IndexOf( m_series );
      bool series3D = Chart.Series3D;
      bool inverted = m_series.ConfigItems.StepItem.Inverted;
      Pen pen = m_series.GetOfflineStyle().GdipPen as Pen;
      BrushInfo brush = m_series.GetOfflineStyle().Interior;
      Pen borderpen = m_series.GetOfflineStyle().GdipPen as Pen;
      float fd = GetPlaceDepth();
      float dpth = GetSeriesDepth();
      float serDpth = fd;
      
      g.AddPolygon( CreateBoundsPolygon( serDpth ));

			Color priceUpColor = m_series.ConfigItems.FinancialItem.PriceUpColor;
			Color priceDownColor = m_series.ConfigItems.FinancialItem.PriceDownColor;
      double reversalAmount = m_series.ReversalAmount;
      bool reversalIsPercent = m_series.ReversalIsPercent;
      if ( reversalIsPercent )
      {
        reversalAmount /= 100.0;
      }
      

      // FORMING KAGI POINTS
      // minimums and maximums variables of previous vertical line          
      double prevMaxY = (float)m_series.Points[0].YValues[0];
      double prevMinY = (float)m_series.Points[0].YValues[0];
      // minimums and maximums variables of current vertical line          
      double curMaxY = (float)m_series.Points[0].YValues[0];
      double curMinY = (float)m_series.Points[0].YValues[0];
      // lists which will hold Kagi step points 
      ArrayList listChartPoints = new ArrayList( );
      ArrayList listPoints = new ArrayList( );
      ArrayList listColors = new ArrayList( );
      double tempX;
      double prevYVal = m_series.Points[0].YValues[0];
      double yVal;
      //variable which indicates direction of current vertical line
      bool goingUp = (m_series.Points[0].YValues[0] <= m_series.Points[1].YValues[0]);
      //float verticalX = this.GetPointFromIndex( 0 ).X;
      double verticalX = m_series.Points[0].X;
      Color curColor = goingUp ? priceUpColor : priceDownColor;
      
      listPoints.Add( this.GetPointFromIndex( 0 ) );
      listColors.Add( curColor );
      listChartPoints.Add( m_series.Points[0] );
      for( int i = 1; i < m_series.Points.Count ; i++ )
      {
        yVal = m_series.Points[i].YValues[0];
          
        if (reversalIsPercent)
        {
          // REVERSING LINE
          if ((yVal - curMaxY) / Math.Abs(curMaxY) <= -reversalAmount)
          {
            if( goingUp)
            { // STARTING NEXT VERITCAL LINE
              ChartPoint cpt = new ChartPoint( verticalX, curMaxY );
              listChartPoints.Add( cpt );
              listPoints.Add( new PointF( GetXFromValue( cpt, 0 ) , GetYFromValue( cpt, 0 ) ) );
              listColors.Add( curColor );

              tempX = m_series.Points[i].X;
              cpt = new ChartPoint( tempX , curMaxY );
              listChartPoints.Add( cpt );
              listPoints.Add( new PointF( GetXFromValue( cpt, 0 ) , GetYFromValue( cpt, 0 ) ) );
              listColors.Add( curColor );
              goingUp = false;
              verticalX = tempX;
                
              //initializing previous vert. line maximum and minimums 
              prevMaxY = curMaxY;
              prevMinY = curMinY;
                
              //initializinf current vert. line maximum and minimums
              curMaxY = prevMaxY;
              curMinY = yVal;
            }
          }

          if ((yVal - curMinY) / Math.Abs(curMinY) >= reversalAmount)
          {
            if( !goingUp)
            { // STARTING NEXT VERITCAL LINE
              ChartPoint cpt = new ChartPoint( verticalX, curMinY );
              listChartPoints.Add( cpt );
              listPoints.Add( new PointF( GetXFromValue( cpt, 0 ) , GetYFromValue( cpt, 0 ) ) );
              listColors.Add( curColor );

              tempX = m_series.Points[i].X;
              cpt = new ChartPoint( tempX, curMinY );
              listChartPoints.Add( cpt );
              listPoints.Add( new PointF( GetXFromValue( cpt, 0 ) , GetYFromValue( cpt, 0 ) ) );
              listColors.Add( curColor );
              goingUp = true;
              verticalX = tempX;

              //initializing previous vert. line maximum and minimums 
              prevMaxY = curMaxY;
              prevMinY = curMinY;
                
              //initializinf current vert. line maximum and minimums
              curMaxY = yVal;
              curMinY = prevMinY;
            }
          }
        }
        else
        {
          // REVERSING LINE
          if ((yVal - curMaxY) <= -reversalAmount)
          {
            if( goingUp)
            { // STARTING NEXT VERITCAL LINE
              ChartPoint cpt = new ChartPoint( verticalX, curMaxY );
              listChartPoints.Add( cpt );
              listPoints.Add( new PointF( GetXFromValue( cpt, 0 ) , GetYFromValue( cpt, 0 ) ) );
              listColors.Add( curColor );

              tempX = m_series.Points[i].X;
              cpt = new ChartPoint( tempX, curMaxY );
              listChartPoints.Add( cpt );
              listPoints.Add( new PointF( GetXFromValue( cpt, 0 ) , GetYFromValue( cpt, 0 ) ) );
              listColors.Add( curColor );

              goingUp = false;
              verticalX = tempX;

              //initializing previous vert. line maximum and minimums 
              prevMaxY = curMaxY;
              prevMinY = curMinY;
                
              //initializinf current vert. line maximum and minimums
              curMaxY = prevMaxY;
              curMinY = yVal;
            }
          }

          if ((yVal - curMinY) >= reversalAmount)
          {
            if( !goingUp)
            { // STARTING NEXT VERITCAL LINE
              ChartPoint cpt = new ChartPoint( verticalX, curMinY );
              listChartPoints.Add( cpt );
              listPoints.Add( new PointF( GetXFromValue( cpt, 0 ) , GetYFromValue( cpt, 0 ) ) );
              listColors.Add( curColor );
              tempX = m_series.Points[i].X;

              cpt = new ChartPoint( tempX, curMinY );
              listChartPoints.Add( cpt );
              listPoints.Add( new PointF( GetXFromValue( cpt, 0 ) , GetYFromValue( cpt, 0 ) ) );
              listColors.Add( curColor );
              goingUp = true;
              verticalX = tempX;

              //initializing previous vert. line maximum and minimums 
              prevMaxY = curMaxY;
              prevMinY = curMinY;
                
              //initializinf current vert. line maximum and minimums
              curMaxY = yVal;
              curMinY = prevMinY;
            }
          }
        }

          
        if( goingUp )
        {// CHANGING LINE COLOR, IF WE PASS THE PREVIOUS VERTICAL LINE MAXIMUM          
          if( (yVal > prevMaxY) && (curMinY < prevMaxY) && ( curColor != priceUpColor ) )
          {
            ChartPoint cpt = new ChartPoint( verticalX, prevMaxY );
            listChartPoints.Add( cpt );
            listPoints.Add( new PointF( GetXFromValue( cpt, 0 ) , GetYFromValue( cpt, 0 ) ) );
            curColor = priceUpColor;
            listColors.Add( curColor );
          }
        }
        else
        {// CHANGING LINE COLOR, IF WE PASS THE PREVIOUS VERTICAL LINE MINIMUM
          if( (yVal < prevMinY) && (curMaxY > prevMinY) && ( curColor != priceDownColor ) )
          {
            ChartPoint cpt = new ChartPoint( verticalX, prevMinY );
            listChartPoints.Add( cpt );
            listPoints.Add( new PointF( GetXFromValue( cpt, 0 ) , GetYFromValue( cpt, 0 ) ) );
            curColor = priceDownColor;
            listColors.Add( curColor );
          }
        }

        // MINIMUMS AND MAXIMUMS OF CURRENT VERTICAL LINE          
        if( yVal < curMinY )
        {
          curMinY = yVal;
        }
        if( yVal > curMaxY )
        {
          curMaxY = yVal;
        }
          
        prevYVal = yVal;
      }
      // Adding Points which wasnt processed
      ChartPoint cpL = m_series.Points[ m_series.Points.Count - 1 ];
      ChartPoint cpT = new ChartPoint( verticalX, cpL.YValues );
      listChartPoints.Add( cpT );
      listPoints.Add( new PointF( GetXFromValue( cpT, 0 ), GetYFromValue( cpT, 0 ) ) );                
      listColors.Add( curColor );
      //END FORMING KAGI POINTS
      
      
      PointF[] stepPoints = new PointF[listPoints.Count];
      Color[] pointColors = new Color[listColors.Count];
      ChartPoint[] chartPoints = new ChartPoint[ listChartPoints.Count ];
      for( int i = 0 ; i < listPoints.Count ; i++ )
      {
        stepPoints[i] = (PointF)listPoints[i];
        pointColors[i] = (Color)listColors[i];
        chartPoints[i] = (ChartPoint)listChartPoints[i];
      }

      ChartRegionData crd = null;

      if(	Chart.NeedRegionUpdate )
      {
        string s = GetToolTip();
        crd = new ChartRegionData( serIndex, s,"Kagi Chart	Region" );
      }

      int count = listColors.Count;

      #region adding front plane
      float maxY = float.MinValue; 
      float maxX = float.MinValue; 
      float minY = float.MaxValue; 
      float minX = float.MaxValue; 
      for( int i = 0; i < count; i++ )
      {
        PointF p1 = stepPoints[ i ];

        if( p1.X > maxX ) maxX = p1.X;
        if( p1.Y > maxY ) maxY = p1.Y;
        if( p1.X < minX ) minX = p1.X;
        if( p1.Y > minY ) minY = p1.Y;
      }
      float frontDepth = serDpth;
      Vector3D[] front = new Vector3D[]{
                                        new Vector3D( minX, minY, frontDepth ),
                                        new Vector3D( maxX, minY, frontDepth ),
                                        new Vector3D( maxX, maxY, frontDepth),
                                        new Vector3D( minX, maxY, frontDepth),
      };
      Polygon frontPoly = new Polygon( front, (BrushInfo)null, (Pen) null /*new Pen( Color.White )*/ );
      g.AddPolygon( frontPoly );
      #endregion 

      for( int i = 0; i < count-1; i++ )
      {
        PointF p1 = stepPoints[ i ];
        PointF p2 = stepPoints[ i + 1 ];
        ChartPoint cp1 = chartPoints[ i ];
        ChartPoint cp2 = chartPoints[ i+1 ];
        if( cp1.X == cp2.X )
        {
          Vector3D v1 = new Vector3D( p1.X, p1.Y, serDpth );
          Vector3D v2 = new Vector3D( p2.X, p2.Y, serDpth );
          Vector3D v3 = new Vector3D( p2.X, p2.Y, serDpth+dpth );
          Vector3D v4 = new Vector3D( p1.X, p1.Y, serDpth+dpth );

          Polygon p = new Polygon( new Vector3D[]{ v1, v2, v3, v4 }, new BrushInfo( pointColors[ i ] ) , SeriesStyle.GdipPen );
          p.RegionData = crd;
          g.AddPolygon( p );
        }
      }

      for( int i = 0; i < count-1; i++ )
      {
        PointF p1 = stepPoints[ i ];
        PointF p2 = stepPoints[ i + 1 ];
        ChartPoint cp1 = chartPoints[ i ];
        ChartPoint cp2 = chartPoints[ i+1 ];

        if( cp1.X == cp2.X )
          continue;
        
        Vector3D v1 = new Vector3D( p1.X, p1.Y, serDpth );
        Vector3D v2 = new Vector3D( p2.X, p2.Y, serDpth );
        Vector3D v3 = new Vector3D( p2.X, p2.Y, serDpth+dpth );
        Vector3D v4 = new Vector3D( p1.X, p1.Y, serDpth+dpth );

        Polygon p = new Polygon( new Vector3D[]{ v1, v2, v3, v4 }, new BrushInfo( pointColors[ i ] ) , SeriesStyle.GdipPen );
        p.RegionData = crd;
        g.AddPolygon( p );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="g"></param>
    /// <param name="bounds"></param>
    /// <param name="isShadow"></param>
    /// <param name="shadowColor"></param>
    public override void DrawIcon(Graphics g, Rectangle bounds, bool isShadow, Color shadowColor)
    {
      base.DrawIcon (g, bounds, isShadow, shadowColor);

      if( !isShadow )
      {
        int w4 = bounds.Width/4;
        int h4 = bounds.Height/4;

				Pen gp = new Pen(m_series.ConfigItems.FinancialItem.PriceUpColor);
				Pen rp = new Pen(m_series.ConfigItems.FinancialItem.PriceDownColor);

        g.DrawLines( rp, new Point[]{ 
                                      new Point( bounds.Left + w4, bounds.Top ),
                                      new Point( bounds.Left + w4, bounds.Top + 3*h4 ),
                                      new Point( bounds.Left + 2*w4, bounds.Top + 3*h4 ),
                                      new Point( bounds.Left + 2*w4, bounds.Top + 2*h4 )
                                    });
        g.DrawLines( gp, new Point[]{ 
                                      new Point( bounds.Left + 2*w4, bounds.Top + 2*h4 ),
                                      new Point( bounds.Left + 2*w4, bounds.Top + h4 ),
                                      new Point( bounds.Left + 3*w4, bounds.Top + h4 ),
                                      new Point( bounds.Left + 3*w4, bounds.Top )
                                    });

        gp.Dispose();
        rp.Dispose();
      }
    }
    #endregion
  }
}

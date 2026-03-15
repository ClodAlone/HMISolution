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
  /// Summary description for ChartRenkoRenderer.
  /// </summary>
  internal class RenkoRenderer : ChartSeriesRenderer
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
		/// <summary>
		/// Initializes a new instance of the <see cref="RenkoRenderer"/> class.
		/// </summary>
		/// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
    public RenkoRenderer( ChartSeries series )
      : base( series )
    {
    }
    #endregion

    #region Public methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="g"></param>
    public override void Render( Graphics g )
    {
      int serIndex = Chart.Series.IndexOf( m_series );
      bool series3D = Chart.Series3D;
      bool inverted = m_series.ConfigItems.StepItem.Inverted;
      Pen pen = SeriesStyle.GdipPen as Pen;
      BrushInfo brush = SeriesStyle.Interior;
      Pen borderpen = SeriesStyle.GdipPen as Pen;
      SizeF offset = GetSeriesOffset();
      SizeF serOffset = this.GetThisOffset();
      bool ReverseOrderOfPointsDrawing = false;

      BrushInfo upPriceInterior = GetUpPriceInterior(SeriesStyle.Interior);
      BrushInfo downPriceInterior = GetDownPriceInterior(SeriesStyle.Interior);

      float offsetWidth = serOffset.Width;
      float offsetHeight = serOffset.Height;

      if ( m_series.XAxis.Inversed )
      {
        // reverse order of points drawing is needed for preventing of overlapping of columns.
        ReverseOrderOfPointsDrawing = true;
      }

      double reversalAmount = m_series.ReversalAmount;
      if( m_series.ReversalAmount <= 0.0 )
      {
        reversalAmount = 1.0;
      }

      // FORMING RENKO RECTS
      // minimums and maximums variables of previous vertical line          
      ChartPoint prevMaxYCP = m_series.Points[0];
      ChartPoint prevMinYCP = m_series.Points[0];
      double prevMaxY = m_series.Points[0].YValues[0];
      double prevMinY = m_series.Points[0].YValues[0];
      // lists which will hold Renko rects, and colors of this rects
      ArrayList listRects = new ArrayList( );
      ArrayList listColors = new ArrayList( );
      double prevYVal = m_series.Points[0].YValues[0];
      double yVal, dYMin, dYMax;
      PointF curP = this.GetPointFromIndex( 0 );
      ChartPoint curCP = m_series.Points[0];
      //variable which indicates direction of current vertical line
      bool goingUp = ( this.GetPointFromIndex( 0 ).Y >= this.GetPointFromIndex( 1 ).Y );
      float verticalX = this.GetPointFromIndex( 0 ).X;
      
      for( int i = 1; i < m_series.Points.Count ; i++ )
      {
        curCP = m_series.Points[i];
        yVal = curCP.YValues[0];
        curP = this.GetPointFromIndex( i );
        
        dYMin = yVal - prevMinY;
        if ( dYMin <= - reversalAmount )
        {// we are going down
            
          int rectDownCount = (int)Math.Floor( Math.Abs( dYMin/reversalAmount ) );
            
          double rectHeight = reversalAmount;
          double rectWidth = (curCP.X - prevMinYCP.X)/rectDownCount;// should be > 0

            
          for( int j = 0; j < rectDownCount; j++ )
          {
            ChartPoint cp1 = new ChartPoint( prevMinYCP.X + j*rectWidth, prevMinYCP.YValues[0] - j*rectHeight );
            ChartPoint cp2 = new ChartPoint( cp1.X + rectWidth, cp1.YValues[0] - rectHeight );
            listRects.Add( GetRectangle( cp1, cp2 ) );
            listColors.Add( downPriceInterior );
          }
            
          prevMinY -= rectDownCount*reversalAmount;
          prevMaxY = prevMinY + reversalAmount;
          prevMinYCP = new ChartPoint( prevMinYCP.X + rectDownCount*rectWidth, prevMinYCP.YValues[0] - (rectDownCount)*rectHeight);
          prevMaxYCP = new ChartPoint( prevMinYCP.X, prevMinYCP.YValues[0] + rectHeight );
        }
        
        
        dYMax = yVal - prevMaxY;  
        if ( dYMax >=  reversalAmount )
        {// we are going up
            
          int rectUpCount = (int)Math.Floor( Math.Abs( dYMax/reversalAmount ) );

          double rectHeight = reversalAmount;// should be < 0
          double rectWidth = (curCP.X - prevMinYCP.X)/rectUpCount;// should be > 0
            
          for( int j = 0; j < rectUpCount; j++ )
          {
            ChartPoint cp1 = new ChartPoint( prevMaxYCP.X + j*rectWidth, prevMaxYCP.YValues[0] + j*rectHeight );
            ChartPoint cp2 = new ChartPoint( cp1.X + rectWidth, cp1.YValues[0] + rectHeight );
            listRects.Add( GetRectangle( cp1, cp2 ) );
            listColors.Add( upPriceInterior );
          }
            
          prevMaxY += rectUpCount*reversalAmount;
          prevMinY = prevMaxY - reversalAmount;
          prevMaxYCP = new ChartPoint( prevMaxYCP.X + rectUpCount*rectWidth, prevMaxYCP.YValues[0] + rectUpCount*rectHeight );
          prevMinYCP = new ChartPoint( prevMaxYCP.X, prevMaxYCP.YValues[0] - rectHeight );
        }

          
        prevYVal = yVal;
      }
      //END FORMING RENKO RECTS
      
      //adding offsets
      for( int i=0; i<listRects.Count; i++)
      {
        RectangleF tr=(RectangleF)listRects[i];
        tr.X += offsetWidth;
        tr.Y += offsetHeight;
        listRects[i] = tr;
      }

      
      #region Draw Shadow

      if( SeriesStyle.DisplayShadow && !series3D )
      {
        ChartStyleInfo style = SeriesStyle;

        int i = 0, di = 1, mi = listRects.Count;
        if (ReverseOrderOfPointsDrawing)
        {
          i = mi - 1;
          di = -1;
          mi = -1;
        }
        for(  ; i != mi ; i += di )
        {
          RectangleF shadowRC = new RectangleF( ((RectangleF)listRects[i]).Location, ((RectangleF)listRects[i]).Size );
          shadowRC.Offset( style.ShadowOffset.Width, style.ShadowOffset.Height );
          BrushPaint.FillRectangle( g, shadowRC, style.ShadowInterior );
        }
      }

      #endregion
        
      if( series3D )
      {
        int i = 0, di = 1, mi = listRects.Count;
        if (ReverseOrderOfPointsDrawing)
        {
          i = mi - 1;
          di = -1;
          mi = -1;
        }
        for(  ; i != mi ; i += di )
        {
          BrushInfo sBrush = (BrushInfo)listColors[i];
          Region rectReg = Draw3DRectangle( g, (RectangleF)listRects[i], offset, sBrush, borderpen );
        }
      }

      else
      {
        int i = 0, di = 1, mi = listRects.Count;
        if (ReverseOrderOfPointsDrawing)
        {
          i = mi - 1;
          di = -1;
          mi = -1;
        }
        for(  ; i != mi ; i += di )
        {
          BrushInfo sBrush = (BrushInfo)listColors[i];
          BrushPaint.FillRectangle(g, (RectangleF)listRects[i], sBrush);

          if( EnableStyles )
          {
            g.DrawRectangle( borderpen, ((RectangleF)listRects[i]).X, ((RectangleF)listRects[i]).Y, ((RectangleF)listRects[i]).Width, ((RectangleF)listRects[i]).Height );
          }
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="g"></param>
    public override void Render( Graphics3D g )
    {
      int serIndex = Chart.Series.IndexOf( m_series );
      bool series3D = Chart.Series3D;
      bool inverted = m_series.ConfigItems.StepItem.Inverted;
      Pen pen = SeriesStyle.GdipPen as Pen;
      BrushInfo brush = SeriesStyle.Interior;
      Pen borderpen = SeriesStyle.GdipPen as Pen;
      float fd = GetPlaceDepth();
      float sd = GetSeriesDepth();

      bool ReverseOrderOfPointsDrawing = false;

      if ( m_series.XAxis.Inversed )
      {
        // reverse order of points drawing is needed for preventing of overlapping of columns.
        ReverseOrderOfPointsDrawing = true;
      }

      BrushInfo upPriceInterior = GetUpPriceInterior(SeriesStyle.Interior);
      BrushInfo downPriceInterior = GetDownPriceInterior(SeriesStyle.Interior);
      double reversalAmount = m_series.ReversalAmount;
      if( m_series.ReversalAmount <= 0.0 )
      {
        reversalAmount = 1.0;
      }

      // FORMING RENKO RECTS
      // minimums and maximums variables of previous vertical line          
      ChartPoint prevMaxYCP = m_series.Points[0];
      ChartPoint prevMinYCP = m_series.Points[0];
      double prevMaxY = m_series.Points[0].YValues[0];
      double prevMinY = m_series.Points[0].YValues[0];
      // lists which will hold Renko rects, and colors of this rects
      ArrayList listRects = new ArrayList( );
      ArrayList listColors = new ArrayList( );
      double prevYVal = m_series.Points[0].YValues[0];
      double yVal, dYMin, dYMax;
      PointF curP = this.GetPointFromIndex( 0 );
      ChartPoint curCP = m_series.Points[0];
      //variable which indicates direction of current vertical line
      bool goingUp = ( this.GetPointFromIndex( 0 ).Y >= this.GetPointFromIndex( 1 ).Y );
      float verticalX = this.GetPointFromIndex( 0 ).X;
      
      for( int i = 1; i < m_series.Points.Count ; i++ )
      {
        curCP = m_series.Points[i];
        yVal = curCP.YValues[0];
        curP = this.GetPointFromIndex( i );
        
        dYMin = yVal - prevMinY;
        if ( dYMin <= - reversalAmount )
        {// we are going down
            
          int rectDownCount = (int)Math.Floor( Math.Abs( dYMin/reversalAmount ) );
            
          double rectHeight = reversalAmount;
          double rectWidth = (curCP.X - prevMinYCP.X)/rectDownCount;// should be > 0

            
          for( int j = 0; j < rectDownCount; j++ )
          {
            ChartPoint cp1 = new ChartPoint( prevMinYCP.X + j*rectWidth, prevMinYCP.YValues[0] - j*rectHeight );
            ChartPoint cp2 = new ChartPoint( cp1.X + rectWidth, cp1.YValues[0] - rectHeight );
            listRects.Add( GetRectangle( cp1, cp2 ) );
            listColors.Add( downPriceInterior );
          }
            
          prevMinY -= rectDownCount*reversalAmount;
          prevMaxY = prevMinY + reversalAmount;
          prevMinYCP = new ChartPoint( prevMinYCP.X + rectDownCount*rectWidth, prevMinYCP.YValues[0] - (rectDownCount)*rectHeight);
          prevMaxYCP = new ChartPoint( prevMinYCP.X, prevMinYCP.YValues[0] + rectHeight );
        }
        
        
        dYMax = yVal - prevMaxY;  
        if ( dYMax >=  reversalAmount )
        {// we are going up
            
          int rectUpCount = (int)Math.Floor( Math.Abs( dYMax/reversalAmount ) );

          double rectHeight = reversalAmount;// should be < 0
          double rectWidth = (curCP.X - prevMinYCP.X)/rectUpCount;// should be > 0
            
          for( int j = 0; j < rectUpCount; j++ )
          {
            ChartPoint cp1 = new ChartPoint( prevMaxYCP.X + j*rectWidth, prevMaxYCP.YValues[0] + j*rectHeight );
            ChartPoint cp2 = new ChartPoint( cp1.X + rectWidth, cp1.YValues[0] + rectHeight );
            listRects.Add( GetRectangle( cp1, cp2 ) );
            listColors.Add( upPriceInterior );
          }
            
          prevMaxY += rectUpCount*reversalAmount;
          prevMinY = prevMaxY - reversalAmount;
          prevMaxYCP = new ChartPoint( prevMaxYCP.X + rectUpCount*rectWidth, prevMaxYCP.YValues[0] + rectUpCount*rectHeight );
          prevMinYCP = new ChartPoint( prevMaxYCP.X, prevMaxYCP.YValues[0] - rectHeight );
        }

          
        prevYVal = yVal;
      }
      //END FORMING RENKO RECTS
      
      //adding offsets
      for( int i=0; i<listRects.Count; i++)
      {
        RectangleF tr=(RectangleF)listRects[i];
        listRects[i] = tr;
      }          

      int q = 0, dq = 1, mq = listRects.Count;
      if (ReverseOrderOfPointsDrawing)
      {
        q = mq - 1;
        dq = -1;
        mq = -1;
      }
      for(  ; q != mq ; q += dq )
      {
        BrushInfo sBrush = (BrushInfo)listColors[q];
        RectangleF rc = (RectangleF)listRects[q];
        g.CreateBox( new Vector3D( rc.Left, rc.Top, fd ), new Vector3D( rc.Right, rc.Bottom, fd+sd ), borderpen, sBrush );
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

				SolidBrush gsb = new SolidBrush(m_series.ConfigItems.FinancialItem.PriceUpColor);
				SolidBrush rsb = new SolidBrush(m_series.ConfigItems.FinancialItem.PriceDownColor);

        g.FillRectangle( rsb, bounds.Left, bounds.Top + h4, w4, h4 );
        g.FillRectangle( rsb, bounds.Left + w4, bounds.Top + 2*h4, w4, h4 );
        g.FillRectangle( gsb, bounds.Left + 2*w4, bounds.Top + h4, w4, h4 );
        g.FillRectangle( gsb, bounds.Left + 3*w4, bounds.Top, w4, h4 );

        gsb.Dispose();
        rsb.Dispose();
      }
    }
    #endregion
  }
}

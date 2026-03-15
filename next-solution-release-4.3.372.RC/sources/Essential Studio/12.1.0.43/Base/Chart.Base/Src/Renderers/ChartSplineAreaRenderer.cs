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
using System.Drawing.Drawing2D;
using Syncfusion.Drawing;
using System.Collections;

namespace Syncfusion.Windows.Forms.Chart.Renderers
{
	/// <summary>
	/// 
	/// </summary>
	internal class SplineAreaRenderer : ChartSeriesRenderer
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
		/// 
		/// </summary>
		/// <param name="series"></param>
		public SplineAreaRenderer(ChartSeries series)
			: base(series)
		{
		}
		#endregion

		#region Public methods
		/// <summary>
		/// In the base <see cref="ChartSeriesRenderer"/> it does not do anything. In derived classes this function does
		/// the rendering.
		/// </summary>
		/// <param name="g">The graphics object that is to be used for rendering.</param>
		public override void Render(Graphics g)
		{
			IndexRange vRange = this.CalculateVisibleRange();
			IndexRange[] ranges = this.CalculateUnEmptyRanges(vRange);

			for (int i = 0; i < ranges.Length; i++)
			{
				if ((ranges[i].To - ranges[i].From) > 1)
				{
					this.Render(g, ranges[i].From, ranges[i].To - ranges[i].From + 1);
				}
			}
		}
		/// <summary>
		/// In the base <see cref="ChartSeriesRenderer"/> it does not do anything. In derived classes this function does
		/// the rendering.
		/// </summary>
		/// <param name="g">The graphics object that is to be used for rendering.</param>
		public override void Render(Graphics3D g)
		{
			IndexRange vRange = this.CalculateVisibleRange();
			IndexRange[] ranges = this.CalculateUnEmptyRanges(vRange);

			for (int i = 0; i < ranges.Length; i++)
			{
				if ((ranges[i].To - ranges[i].From) > 1)
				{
					this.Render(g, ranges[i].From, ranges[i].To - ranges[i].From + 1);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="from"></param>
		/// <param name="count"></param>
		public void Render(Graphics g, int from, int count)
		{
			bool series3D = Chart.Series3D;
			int serIndex = Chart.Series.IndexOf(m_series);
			BrushInfo brush = this.GetBrush();
			Pen borderpen = SeriesStyle.GdipPen as Pen;
			ArrayList pointsWithXNotCoinside = new ArrayList(2);
			double origin0 = m_series.ActualYAxis.Origin;
			double[] y2;
			ChartPointWithIndex p0, p1, p2, p3;
            bool areaToolTip = (m_series.EnableAreaToolTip && Chart.CalcRegions);
            ChartRenderArgs2D args = new ChartRenderArgs2D(Chart, m_series);
            args.Graph = new ChartGDIGraph(g);

            IList toolTipRegions = this.ChartArea.ChartRegions;
            ArrayList toolTipFrontRegions = new ArrayList();
            ArrayList toolTipTopRegions = new ArrayList();
            ArrayList toolTipBottomRegions = new ArrayList();                                  
            int leftIndex=0,rightIndex=0;

			if (this.Chart.NeedRegionUpdate)
			{               
				for (int i = 0; i < count; i++)
				{
					ChartStyleInfo style = GetStyleAt(from + i);
					this.ChartArea.ChartRegions.Add(new ChartRegion(this.GetRegionFromCircle(GetPointFromIndex(i), style.HitTestRadius), serIndex, from + i, style.ToolTip, "Spline Area Chart Region"));                  
                }                
			}
			//coefficient that fixes problem with axes inversing
			SizeF offset = GetSeriesOffset();
			SizeF serOffset = GetThisOffset();
			float originY = CustomOriginY + serOffset.Height;

			ChartPointWithIndex[] cpwiArr = new ChartPointWithIndex[count];
			Array.Copy(this.PrepearePoints(), from, cpwiArr, 0, count);

			// dividing points into arrays where each point has defferent x ( the x's in one array can't coinside )
			for (int j = 0, start = 0; j < count - 1; j++)
			{
				if (cpwiArr[j].Point.X == cpwiArr[j + 1].Point.X)
				{
					if (j + 1 - start > 1)
					{
						ChartPointWithIndex[] tPA = new ChartPointWithIndex[j + 1 - start];
						//Array.Copy( series.Points, start, tPA, 0, j + 1 - start );
						Array.Copy(cpwiArr, start, tPA, 0, j + 1 - start);
						pointsWithXNotCoinside.Add(tPA);
					}

					for (int k = j + 2; k < count; k++)
					{
						if (cpwiArr[j + 1].Point.X != cpwiArr[k].Point.X)
						{
							j = k - 2;
							start = k - 1;
							break;
						}
					}
				}
				else
				{
					if (j == count - 2)
					{
						ChartPointWithIndex[] tPA = new ChartPointWithIndex[count - start];
						//Array.Copy( series.Points, start, tPA, 0, count - start );
						Array.Copy(cpwiArr, start, tPA, 0, count - start);
						pointsWithXNotCoinside.Add(tPA);
					}
				}
			}// END {dividing points into arrays where each point has defferent x ( the x's in one array can't coinside )}

			//Front
			GraphicsPath gp = new GraphicsPath();           

			for (int j = 0; j < pointsWithXNotCoinside.Count; j++)
			{
				//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
				ChartPointWithIndex[] points = (ChartPointWithIndex[])pointsWithXNotCoinside[j];
				//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

				NaturalSpline(points, out y2);

				PointF pf0, pf1, pf2, pf3;
                PointF ttpf0, ttpf1, ttpf2, ttpf3;
                PointF toolTipCurrentTopPt, toolTipNextTopPt, toolTipMidPt1, toolTipMidPt2;
                             
				for (int i = 0; i < points.Length - 1; i++)
				{
					BezierPointsFromSpline(points[i], points[i + 1], y2[i], y2[i + 1], out p0, out p1, out p2, out p3);
					pf0 = new PointF(GetXFromValue(p0.Point, 0) + serOffset.Width, GetYFromValue(p0.Point, 0) + serOffset.Height);
					pf1 = new PointF(GetXFromValue(p1.Point, 0) + serOffset.Width, GetYFromValue(p1.Point, 0) + serOffset.Height);
					pf2 = new PointF(GetXFromValue(p2.Point, 0) + serOffset.Width, GetYFromValue(p2.Point, 0) + serOffset.Height);
					pf3 = new PointF(GetXFromValue(p3.Point, 0) + serOffset.Width, GetYFromValue(p3.Point, 0) + serOffset.Height);
					gp.AddBezier(pf0, pf1, pf2, pf3);                  

                    if (areaToolTip && Chart.NeedRegionUpdate)
                    {
                        #region toolTipFrontRegions 

                        toolTipCurrentTopPt = pf0;
                        toolTipNextTopPt = pf3;

                        ChartPoint toolTipOriginCp1 = new ChartPoint(points[i].Point.X, origin0);
                        PointF toolTipCurrentBtmPt = new PointF(GetXFromValue(toolTipOriginCp1, 0) + serOffset.Width, GetYFromValue(toolTipOriginCp1, 0) + serOffset.Height);
                        ChartPoint toolTipOriginCp2 = new ChartPoint(points[i + 1].Point.X, origin0);
                        PointF toolTipNextBtmPt = new PointF(GetXFromValue(toolTipOriginCp2, 0) + serOffset.Width, GetYFromValue(toolTipOriginCp2, 0) + serOffset.Height);

                        float topX=0, topY=0, bottomX=0, bottomY = 0;

                        topX= (toolTipCurrentTopPt.X + toolTipNextTopPt.X) / 2;
                        topY = (toolTipCurrentTopPt.Y + toolTipNextTopPt.Y) / 2;
                        bottomX = (toolTipCurrentBtmPt.X + toolTipNextBtmPt.X) / 2;
                        bottomY = (toolTipCurrentBtmPt.Y + toolTipNextBtmPt.Y) / 2;

                        toolTipMidPt1 = new PointF(topX, topY);
                        toolTipMidPt2 = new PointF(bottomX, bottomY);

                        GraphicsPath left = new GraphicsPath();
                        GraphicsPath right = new GraphicsPath();

                        left.AddLine(toolTipCurrentBtmPt, toolTipCurrentTopPt);
                        if(i==0)
                          left.AddBezier(pf0, pf1, pf2, pf3);                   
                        else
                          left.AddBezier(pf0, pf1, toolTipMidPt1, toolTipMidPt1);
                     
                        left.AddLine(toolTipMidPt1, toolTipMidPt2);
                        left.AddLine(toolTipMidPt2, toolTipCurrentBtmPt);

                        right.AddLine(toolTipMidPt2, toolTipMidPt1);
                        if (i == 0)
                          right.AddBezier(pf0, pf1, pf2, pf3);
                        else
                          right.AddBezier(toolTipMidPt1, toolTipMidPt1, pf2, pf3);

                        right.AddLine(toolTipNextTopPt, toolTipNextBtmPt);
                        right.AddLine(toolTipNextBtmPt, toolTipMidPt2);

                        left.CloseFigure();
                        right.CloseFigure();                  

                        toolTipFrontRegions.Add(new ChartRegion(new Region(left), serIndex, points[i].Index, GetToolTip(points[i].Index), "Spline Area Chart Region"));
                        toolTipFrontRegions.Add(new ChartRegion(new Region(right), serIndex, points[i + 1].Index, GetToolTip(points[i + 1].Index), "Spline Area Chart Region"));
                                                
                        #endregion
                    }                    
				}

				ChartPoint cpo1 = new ChartPoint(points[points.Length - 1].Point.X, origin0);
				PointF originPoint1 = new PointF(GetXFromValue(cpo1, 0) + serOffset.Width, GetYFromValue(cpo1, 0) + serOffset.Height);
				ChartPoint cpo2 = new ChartPoint(points[0].Point.X, origin0);
				PointF originPoint2 = new PointF(GetXFromValue(cpo2, 0) + serOffset.Width, GetYFromValue(cpo2, 0) + serOffset.Height);

				PointF pointEnd = new PointF(GetXFromValue(points[points.Length - 1].Point, 0) + serOffset.Width, GetYFromValue(points[points.Length - 1].Point, 0) + serOffset.Height);
				PointF pointStart = new PointF(GetXFromValue(points[0].Point, 0) + serOffset.Width, GetYFromValue(points[0].Point, 0) + serOffset.Height);

				gp.AddLine(pointEnd, originPoint1);
				gp.AddLine(originPoint1, originPoint2);               

				gp.CloseFigure();

                Region ttLeftSide = new Region();
                Region ttRightSide = new Region();
				if (series3D)
				{
					Region rgn;

					//Back
					GraphicsPath back = new GraphicsPath();
					//PointF[] offsetPoints = GetOffsetPoints( points, offset );
                    
					for (int i = 0; i < points.Length - 1; i++)
					{
						BezierPointsFromSpline(points[i], points[i + 1], y2[i], y2[i + 1], out p0, out p1, out p2, out p3);

						pf0 = new PointF(GetXFromValue(p0.Point, 0) + serOffset.Width + offset.Width, GetYFromValue(p0.Point, 0) + serOffset.Height + offset.Height);
						pf1 = new PointF(GetXFromValue(p1.Point, 0) + serOffset.Width + offset.Width, GetYFromValue(p1.Point, 0) + serOffset.Height + offset.Height);
						pf2 = new PointF(GetXFromValue(p2.Point, 0) + serOffset.Width + offset.Width, GetYFromValue(p2.Point, 0) + serOffset.Height + offset.Height);
						pf3 = new PointF(GetXFromValue(p3.Point, 0) + serOffset.Width + offset.Width, GetYFromValue(p3.Point, 0) + serOffset.Height + offset.Height);

						back.AddBezier(pf0, pf1, pf2, pf3);

                        ttpf0 = new PointF(GetXFromValue(p0.Point, 0) + serOffset.Width , GetYFromValue(p0.Point, 0) + serOffset.Height );
                        ttpf1 = new PointF(GetXFromValue(p1.Point, 0) + serOffset.Width, GetYFromValue(p1.Point, 0) + serOffset.Height);
                        ttpf2 = new PointF(GetXFromValue(p2.Point, 0) + serOffset.Width, GetYFromValue(p2.Point, 0) + serOffset.Height);
                        ttpf3 = new PointF(GetXFromValue(p3.Point, 0) + serOffset.Width, GetYFromValue(p3.Point, 0) + serOffset.Height);                     
                
                        if (areaToolTip )
                        {
                            if (i == 0)
                                leftIndex = points[i].Index;

                            rightIndex = points[i + 1].Index;

                            #region toolTipTopRegions 

                            toolTipCurrentTopPt = ttpf0;
                            toolTipNextTopPt = ttpf3;                                                             

                            PointF toolTipCurrentBackPt = pf0;
                            PointF toolTipNextBackPt = pf3;

                            float frontX=0, frontY=0, backX=0, backY=0;
                            frontX=(toolTipCurrentTopPt.X+ toolTipNextTopPt.X)/2;
                            frontY = (toolTipCurrentTopPt.Y + toolTipNextTopPt.Y) / 2;
                            backX=(toolTipCurrentBackPt.X+toolTipNextBackPt.X)/2;
                            backY = (toolTipCurrentBackPt.Y + toolTipNextBackPt.Y) / 2;

                            toolTipMidPt1 = new PointF(frontX,frontY);
                            toolTipMidPt2 = new PointF(backX, backY);

                            GraphicsPath left = new GraphicsPath();
                            left.AddLine(toolTipCurrentTopPt, toolTipCurrentBackPt);                           
                           // left.AddBezier(pf0, pf1, pf2, pf3);
                            left.AddBezier(pf0, pf1, toolTipMidPt2, toolTipMidPt2);
                            left.AddLine(toolTipMidPt2, toolTipMidPt1);                            
                            //left.AddBezier(ttpf3, ttpf2, ttpf1, ttpf0);
                            left.AddBezier(toolTipMidPt1, toolTipMidPt1, ttpf1, ttpf0); 
                            left.CloseFigure();
                            
                            GraphicsPath right = new GraphicsPath();
                            right.AddLine(toolTipMidPt1, toolTipMidPt2);                          
                           // right.AddBezier(pf0, pf1, pf2, pf3);
                            right.AddBezier(toolTipMidPt2, toolTipMidPt2, pf2, pf3);
                            right.AddLine(toolTipNextBackPt, toolTipNextTopPt);                          
                           // right.AddBezier(ttpf3, ttpf2, ttpf1, ttpf0);
                            right.AddBezier(ttpf3, ttpf2, toolTipMidPt1, toolTipMidPt1);
                            right.CloseFigure();

                            toolTipTopRegions.Add(new ChartRegion(new Region(left), serIndex, points[i].Index, GetToolTip(points[i].Index), "Spline Area Chart Region"));
                            toolTipTopRegions.Add(new ChartRegion(new Region(right), serIndex, points[i + 1].Index, GetToolTip(points[i + 1].Index), "Spline Area Chart Region"));                          
                            #endregion

                            #region toolTipBottomRegions 

                            ChartPoint toolTipOriginCp1 = new ChartPoint(points[i].Point.X, origin0);
                            PointF toolTipCurrentBtmPt = new PointF(GetXFromValue(toolTipOriginCp1, 0) + serOffset.Width, GetYFromValue(toolTipOriginCp1, 0) + serOffset.Height);
                            ChartPoint toolTipOriginCp2 = new ChartPoint(points[i + 1].Point.X, origin0);
                            PointF toolTipNextBtmPt = new PointF(GetXFromValue(toolTipOriginCp2, 0) + serOffset.Width, GetYFromValue(toolTipOriginCp2, 0) + serOffset.Height);

                            PointF toolTipCurrentBtmBackPt = new PointF(GetXFromValue(toolTipOriginCp1, 0) + serOffset.Width+offset.Width, GetYFromValue(toolTipOriginCp1, 0) + serOffset.Height+offset.Height);                          
                            PointF toolTipNextBtmBackPt = new PointF(GetXFromValue(toolTipOriginCp2, 0) + serOffset.Width + offset.Width, GetYFromValue(toolTipOriginCp2, 0) + serOffset.Height + offset.Height);

                            float btmFrontX = 0, btmFrontY = 0, btmBackX = 0, btmBackY = 0;

                            btmFrontX = (toolTipCurrentBtmPt.X + toolTipNextBtmPt.X) / 2;
                            btmFrontY = (toolTipCurrentBtmPt.Y + toolTipNextBtmPt.Y) / 2;
                            btmBackX = (toolTipCurrentBtmBackPt.X + toolTipNextBtmBackPt.X) / 2;
                            btmBackY = (toolTipCurrentBtmBackPt.Y + toolTipNextBtmBackPt.Y) / 2;

                            toolTipMidPt1 = new PointF(btmFrontX, btmFrontY);
                            toolTipMidPt2 = new PointF(btmBackX, btmBackY);

                            GraphicsPath btmLeft = new GraphicsPath();
                            GraphicsPath btmRight = new GraphicsPath();

                            btmLeft.AddPolygon(new PointF[] { toolTipCurrentBtmPt, toolTipCurrentBtmBackPt, toolTipMidPt2, toolTipMidPt1 });
                            Region btmLft = new Region(btmLeft);
                            toolTipBottomRegions.Add(new ChartRegion(btmLft, serIndex, points[i].Index, GetToolTip(points[i].Index), "Spline Area Chart Region"));

                            btmRight.AddPolygon(new PointF[] { toolTipMidPt1, toolTipMidPt2, toolTipNextBtmBackPt, toolTipNextBtmPt });
                            Region btmRht = new Region(btmRight);
                            toolTipBottomRegions.Add(new ChartRegion(btmRht, serIndex, points[i + 1].Index, GetToolTip(points[i + 1].Index), "Spline Area Chart Region"));
#endregion
                        }
					}

					ChartPoint backCpo1 = new ChartPoint(points[points.Length - 1].Point.X, 0);
					PointF backOriginPoint1 = new PointF(GetXFromValue(cpo1, 0) + serOffset.Width + offset.Width, GetYFromValue(cpo1, 0) + serOffset.Height + offset.Height);
					ChartPoint backCpo2 = new ChartPoint(points[0].Point.X, 0);
					PointF backOriginPoint2 = new PointF(GetXFromValue(cpo2, 0) + serOffset.Width + offset.Width, GetYFromValue(cpo2, 0) + serOffset.Height + offset.Height);

					PointF backPointEnd = new PointF(GetXFromValue(points[points.Length - 1].Point, 0) + serOffset.Width + offset.Width, GetYFromValue(points[points.Length - 1].Point, 0) + serOffset.Height + offset.Height);
					PointF backPointStart = new PointF(GetXFromValue(points[0].Point, 0) + serOffset.Width + offset.Width, GetYFromValue(points[0].Point, 0) + serOffset.Height + offset.Height);

					back.AddLine(backOriginPoint1, backOriginPoint2);
					back.CloseFigure();

                    if (!args.Chart.Style3D)
                    {
                        BrushPaint.FillPath(g, back, brush);
                        g.DrawPath(borderpen, back);
                    }
                    else
                        this.Draw(args.Graph, back, brush, borderpen);

					//Left Side
					GraphicsPath leftside = new GraphicsPath();
					leftside.AddPolygon(new PointF[]
          {
            pointStart,
            originPoint2,
            backOriginPoint2,
            backPointStart 
          });

					//Right Side
					GraphicsPath rightside = new GraphicsPath();
					rightside.AddPolygon(new PointF[]
          {
            pointEnd,
            originPoint1,
            backOriginPoint1,
            backPointEnd
          });


                    
					//Lower side
					GraphicsPath lowerside = new GraphicsPath();
					lowerside.AddPolygon(new PointF[]
          {
            originPoint2,
            backOriginPoint2,
            backOriginPoint1,
            originPoint1
          });
					if (!args.Chart.Style3D)
                    {
                        BrushPaint.FillPath(g, rightside, brush);
                        g.DrawPath(borderpen, rightside);
                    }
                    else
                        this.Draw(args.Graph, rightside, brush, borderpen);

					if (YAxis.Inversed)
					{
						//Top
						rgn = Draw3DSpline(g, points, y2, offset, brush, borderpen);
						//Left
                        if (!args.Chart.Style3D)
                        {
                            BrushPaint.FillPath(g, leftside, brush);
                            g.DrawPath(borderpen, leftside);
                        }
                        else
                            this.Draw(args.Graph, leftside, brush, borderpen);
						//Right
                        if (!args.Chart.Style3D)
                        {
                            BrushPaint.FillPath(g, rightside, brush);
                            g.DrawPath(borderpen, rightside);
                        }
                        else
                            this.Draw(args.Graph, rightside, brush, borderpen);
						//Lower
                        if (!args.Chart.Style3D)
                        {
                            BrushPaint.FillPath(g, lowerside, brush);
                            g.DrawPath(borderpen, lowerside);
                        }
                        else
                            this.Draw(args.Graph, lowerside, brush, borderpen);
					}

					else
					{
						//Lower
                        if (!args.Chart.Style3D)
                        {
                            BrushPaint.FillPath(g, lowerside, brush);
                            g.DrawPath(borderpen, lowerside);
                        }
                        else
                            this.Draw(args.Graph, lowerside, brush, borderpen);
                        //Left
                        if (!args.Chart.Style3D)
                        {
                            BrushPaint.FillPath(g, leftside, brush);
                            g.DrawPath(borderpen, leftside);
                        }
                        else
                            this.Draw(args.Graph, leftside, brush, borderpen);

                        //Top
                        rgn = Draw3DSpline(g, points, y2, offset, brush, borderpen);
                        //Right
                        if (!args.Chart.Style3D)
                        {
                            BrushPaint.FillPath(g, rightside, brush);
                            g.DrawPath(borderpen, rightside);
                        }
                        else

                            this.Draw(args.Graph, rightside, brush, borderpen);
					}

					if (this.Chart.NeedRegionUpdate)
					{
						Region rgn1 = new Region(gp);
						rgn1.Union(leftside);
						rgn1.Union(rightside);
						//rgn1.Union( rgn );
                        this.ChartArea.ChartRegions.Add(new ChartRegion(rgn1,
                            serIndex, GetToolTip(), "Spline Area Chart Region"));

                        ttLeftSide = new Region(leftside);
                        ttRightSide = new Region(rightside);                   
					}                				                                     
                                             
				}                    
				else
				{
					if (this.Chart.NeedRegionUpdate)
					{
                        this.ChartArea.ChartRegions.Add(new ChartRegion(new Region(gp),
                         serIndex, GetToolTip(), "Spline Area Chart Region"));                      
					}                                      
				}
                BrushPaint.FillPath(g, gp, brush);
                g.DrawPath(borderpen, gp);


                #region toolTipRegionsAdd 

                if (this.Chart.NeedRegionUpdate && areaToolTip && series3D)
                {
                    this.ChartArea.ChartRegions.Add(new ChartRegion(ttLeftSide, serIndex,
                           leftIndex, GetToolTip(leftIndex), "Spline Area Chart Region"));
                    this.ChartArea.ChartRegions.Add(new ChartRegion(ttRightSide, serIndex,
                       rightIndex, GetToolTip(rightIndex), "Spline Area Chart Region"));
                    
                    for (int i = 0; i < toolTipTopRegions.Count; i++)
                    {                      
                        ChartRegion cre = (ChartRegion)toolTipTopRegions[i];
                        this.ChartArea.ChartRegions.Add(cre);                    
                    }

                    for (int i = 0; i < toolTipBottomRegions.Count; i++)
                    {                     
                        ChartRegion cre = (ChartRegion)toolTipBottomRegions[i];
                       this.ChartArea.ChartRegions.Add(cre);                        
                    }                                                                      
                }

                if (this.Chart.NeedRegionUpdate && areaToolTip)
                {                 
                    for (int i = 0; i < toolTipFrontRegions.Count; i++)
                    {                      
                        ChartRegion cre = (ChartRegion)toolTipFrontRegions[i];
                        this.ChartArea.ChartRegions.Add(cre);                    
                    }
                }
                #endregion              
            }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="from"></param>
		/// <param name="count"></param>
		public void Render(Graphics3D g, int from, int count)
		{
			int serIndex = Chart.Series.IndexOf(m_series);
			float orY = CustomOriginY;
			int dsc = SPLINE_DIGITIZATION;
			PointF[] points = new PointF[count];
			float fd = GetPlaceDepth();
			float dpth = GetSeriesDepth();
			float bd = fd + dpth;
			double origin0 = m_series.ActualYAxis.Origin;
            bool areaToolTip = (m_series.EnableAreaToolTip && Chart.CalcRegions);
                     
            Graphics3D gg = g;
            int toolTipEndCpNo = from + count - 1;  
            int leftIndex = 0;
            int rightIndex = 0;

			for (int i = 0; i < count; i++)
			{
				points[i] = this.GetPointFromIndex(from + i);
			}

			g.AddPolygon(CreateBoundsPolygon(fd));

			ChartRegionData crd = null;
                      
            if (Chart.NeedRegionUpdate)
            {
				string s = GetToolTip();
				crd = new ChartRegionData(Chart.Series.IndexOf(m_series),s, "SplineArea Chart	Region");
            }
            
			ChartPoint cpo1 = new ChartPoint(m_series.Points[from + count - 1].X, origin0);            
			PointF originPoint1 = new PointF(GetXFromValue(cpo1, 0), GetYFromValue(cpo1, 0));
			ChartPoint cpo2 = new ChartPoint(m_series.Points[0].X, origin0);
			PointF originPoint2 = new PointF(GetXFromValue(cpo2, 0), GetYFromValue(cpo2, 0));           

                    

			GraphicsPath gp = new GraphicsPath();
			gp.AddCurve(points);
			gp.AddLine(points[count - 1], new PointF(points[count - 1].X, orY));
			gp.AddLine(gp.GetLastPoint(), new PointF(points[0].X, orY));
			gp.CloseFigure();

			PointF[] pts = gp.PathData.Points;
                                    
            Polygon pll = new Polygon(new Vector3D[]{ new Vector3D( points[0].X, points[0].Y, fd ),
                                                 new Vector3D( points[0].X, points[0].Y, bd ), 
                                                 new Vector3D( originPoint2.X, originPoint2.Y, bd ), 
                                                 new Vector3D( originPoint2.X, originPoint2.Y, fd )},
                this.GetBrush(), SeriesStyle.GdipPen);
            Polygon plr = new Polygon(new Vector3D[]{ new Vector3D( points[count-1].X, points[count-1].Y, fd ),
                                                 new Vector3D( points[count-1].X, points[count-1].Y, bd ), 
                                                 new Vector3D( originPoint1.X, originPoint1.Y, bd ), 
                                                 new Vector3D( originPoint1.X, originPoint1.Y, fd )},
                this.GetBrush(), SeriesStyle.GdipPen);
            Polygon plb = new Polygon(new Vector3D[]{ new Vector3D( originPoint2.X, originPoint2.Y, fd ),
                                                 new Vector3D( originPoint2.X, originPoint2.Y, bd ), 
                                                 new Vector3D( originPoint1.X, originPoint1.Y, bd ), 
                                                 new Vector3D( originPoint1.X, originPoint1.Y, fd )},
            this.GetBrush(), SeriesStyle.GdipPen);            

			Vector3D[] fvs = new Vector3D[2 + dsc * (count - 1)];
			Vector3D[] bvs = new Vector3D[2 + dsc * (count - 1)];
                                                
			for (int i = 0; i < count - 1; i++)
			{
				PointF p1 = pts[i * 3];
				PointF p2 = pts[i * 3 + 1];
				PointF p3 = pts[i * 3 + 2];
				PointF p4 = pts[i * 3 + 3];

				PointF[] ps = ChartMath.InterpolateBezier(p1, p2, p3, p4, dsc);
               
				for (int j = 0; j < ps.Length; j++)
				{
					fvs[i * dsc + j] = new Vector3D(ps[j].X, ps[j].Y, fd);
					bvs[i * dsc + j] = new Vector3D(ps[j].X, ps[j].Y, bd);
				}
            }

            #region AreaToolTip 

            Vector3D[] toolTipfvs = new Vector3D[2 + dsc * (count - 1)];
            Vector3D[] toolTipbvs = new Vector3D[2 + dsc * (count - 1)];
            int midNo = 0;
            PointF toolTipMid1 = PointF.Empty;
            PointF toolTipMid2 = PointF.Empty;

            for (int i = 0; i < count - 1; i++)
            {
                PointF p1 = pts[i * 3];
                PointF p2 = pts[i * 3 + 1];
                PointF p3 = pts[i * 3 + 2];
                PointF p4 = pts[i * 3 + 3];

                PointF[] ps = ChartMath.InterpolateBezier(p1, p2, p3, p4, dsc);
                midNo = ps.Length / 2;

                if (i == 0)
                    leftIndex = from + i;
                rightIndex = from + i + 1;

                Vector3D[] toolTipFrontVts = new Vector3D[ps.Length + 3];
                Vector3D[] toolTipBackVts = new Vector3D[ps.Length + 3];
                Vector3D[] toolTipBottomVts = new Vector3D[ps.Length + 3];
                int leftCnt = ps.Length / 2;
                int rightCnt = ps.Length - (ps.Length / 2);
                Vector3D[] ttLeftFrontVts = new Vector3D[leftCnt + 2];
                Vector3D[] ttRightFrontVts = new Vector3D[rightCnt + 2];
                Vector3D[] ttLeftBackVts = new Vector3D[leftCnt + 2];
                Vector3D[] ttRightBackVts = new Vector3D[rightCnt + 2];

                for (int j = 0; j < ps.Length; j++)
                {
                    toolTipfvs[i * dsc + j] = new Vector3D(ps[j].X, ps[j].Y, fd);
                    toolTipbvs[i * dsc + j] = new Vector3D(ps[j].X, ps[j].Y, bd);
                }

                if ((i < count - 1) && areaToolTip)                             
                {
                    ChartPoint currentCp = new ChartPoint(m_series.Points[from + i].X, origin0);
                    PointF currentBottomPt = new PointF(GetXFromValue(currentCp, 0), GetYFromValue(currentCp, 0));

                    ChartPoint nextCp = new ChartPoint(m_series.Points[from + i + 1].X, origin0);
                    PointF nextBottomPt = new PointF(GetXFromValue(nextCp, 0), GetYFromValue(nextCp, 0));

                    PointF currentBtmBackPt = currentBottomPt;
                    PointF nextBtmBackPt = nextBottomPt;

                    float btmX, btmY;
                    btmX = (currentBottomPt.X + nextBottomPt.X) / 2;
                    btmY = (currentBottomPt.Y + nextBottomPt.Y) / 2;
                    toolTipMid2 = new PointF(btmX, btmY);

                    int j = 0;
                    for (j = 0; j < ps.Length; j++)
                    {
                        toolTipFrontVts[j] = toolTipfvs[j + (i * dsc)];
                        toolTipBackVts[j] = toolTipbvs[j + (i * dsc)];
                    }
                    BrushInfo bi = new BrushInfo(Color.Transparent);

                    #region BottomToolTip

                    Vector3D lfb = new Vector3D(currentBottomPt.X, currentBottomPt.Y, fd);
                    Vector3D lbb = new Vector3D(currentBottomPt.X, currentBottomPt.Y, bd);
                    Vector3D rbb = new Vector3D(nextBtmBackPt.X, nextBtmBackPt.Y, bd);
                    Vector3D rfb = new Vector3D(nextBtmBackPt.X, nextBtmBackPt.Y, fd);
                    Vector3D midFront = new Vector3D(toolTipMid2.X, toolTipMid2.Y, fd);
                    Vector3D midBack = new Vector3D(toolTipMid2.X, toolTipMid2.Y, bd);

                    Polygon leftBtm = new Polygon(new Vector3D[] { lfb, lbb, midBack, midFront });
                    leftBtm.RegionData = new ChartRegionData(serIndex, (from + i), GetToolTip(from + i), "SplineArea Chart	Region");
                    g.AddPolygon(leftBtm);

                    Polygon rightBtm = new Polygon(new Vector3D[] { midFront, midBack, rbb, rfb });
                    rightBtm.RegionData = new ChartRegionData(serIndex, (from + i + 1), GetToolTip(from + i + 1), "SplineArea Chart	Region");
                    g.AddPolygon(rightBtm);


                    #endregion

                    int l = 0;
                    int m = 0;
                    for (int k = 0; k < ps.Length; k++)
                    {
                        if (k < midNo)
                        {
                            ttLeftFrontVts[l] = toolTipFrontVts[k];
                            ttLeftBackVts[l] = toolTipBackVts[k];
                            l++;
                        }
                        else
                        {
                            ttRightFrontVts[m] = toolTipFrontVts[k];
                            ttRightBackVts[m] = toolTipBackVts[k];
                            if (m == 0)
                            {
                                ttRightFrontVts[m] = toolTipFrontVts[l - 1];
                                ttRightBackVts[m] = toolTipBackVts[l - 1];
                            }
                            m++;
                        }
                    }
                    ttLeftFrontVts[l] = new Vector3D(toolTipMid2.X, toolTipMid2.Y, ttLeftFrontVts[l - 1].Z);
                    ttLeftFrontVts[l + 1] = new Vector3D(currentBottomPt.X, currentBottomPt.Y, ttLeftFrontVts[0].Z);
                    ttLeftBackVts[l] = new Vector3D(toolTipMid2.X, toolTipMid2.Y, ttLeftBackVts[l - 1].Z);
                    ttLeftBackVts[l + 1] = new Vector3D(currentBottomPt.X, currentBottomPt.Y, ttLeftBackVts[0].Z);

                    ttRightFrontVts[m] = new Vector3D(nextBottomPt.X, nextBottomPt.Y, ttRightFrontVts[m - 1].Z);
                    ttRightFrontVts[m + 1] = new Vector3D(toolTipMid2.X, toolTipMid2.Y, ttRightFrontVts[0].Z);
                    ttRightBackVts[m] = new Vector3D(nextBottomPt.X, nextBottomPt.Y, ttRightBackVts[m - 1].Z);
                    ttRightBackVts[m + 1] = new Vector3D(toolTipMid2.X, toolTipMid2.Y, ttRightBackVts[0].Z);

                    Polygon leftPg = new Polygon(ttLeftFrontVts);
                    leftPg.RegionData = new ChartRegionData(serIndex, (from + i), GetToolTip(from + i), "SplineArea Chart	Region");
                    gg.AddPolygon(leftPg);
                    Polygon leftBackPg = new Polygon(ttLeftBackVts);
                    leftBackPg.RegionData = new ChartRegionData(serIndex, (from + i), GetToolTip(from + i), "SplineArea Chart	Region");
                    gg.AddPolygon(leftBackPg);

                    Polygon rightPg = new Polygon(ttRightFrontVts);
                    rightPg.RegionData = new ChartRegionData(serIndex, (from + i + 1), GetToolTip(from + i + 1), "SplineArea Chart	Region");
                    gg.AddPolygon(rightPg);
                    Polygon rightBackPg = new Polygon(ttRightBackVts);
                    rightBackPg.RegionData = new ChartRegionData(serIndex, (from + i + 1), GetToolTip(from + i + 1), "SplineArea Chart	Region");
                    gg.AddPolygon(rightBackPg);              
                }
            }

            #endregion

            fvs[fvs.Length - 2] = new Vector3D(originPoint1.X, originPoint1.Y, fvs[fvs.Length - 3].Z);
			fvs[fvs.Length - 1] = new Vector3D(originPoint2.X, originPoint2.Y, fvs[0].Z);
			bvs[bvs.Length - 2] = new Vector3D(originPoint1.X, originPoint1.Y, bvs[bvs.Length - 3].Z);
			bvs[bvs.Length - 1] = new Vector3D(originPoint2.X, originPoint2.Y, bvs[0].Z);           
                      
            Polygon pf = new Polygon(fvs, this.GetBrush(), SeriesStyle.GdipPen);
            Polygon pb = new Polygon(bvs, this.GetBrush(), SeriesStyle.GdipPen);


            if (!areaToolTip)
            {
                pf.RegionData = crd;
                pb.RegionData = crd;
                pll.RegionData = crd;
                plr.RegionData = crd;
                plb.RegionData = crd;
            }           

            if (areaToolTip)
            {
                ChartRegionData lftEnd = new ChartRegionData(serIndex, leftIndex, GetToolTip(leftIndex), "SplineArea Chart	Region");
                ChartRegionData rhtEnd = new ChartRegionData(serIndex, rightIndex, GetToolTip(rightIndex), "SplineArea Chart	Region");
                pll.RegionData = lftEnd;
                plr.RegionData = rhtEnd;
            }
           
           
            g.AddPolygon(pf);
            g.AddPolygon(pb);
            g.AddPolygon(pll);
            g.AddPolygon(plr);
            g.AddPolygon(plb);           
                    
			for (int i = 0; i < count - 1; i++)
			{
                int indx = from;
				PointF p1 = pts[i * 3];
				PointF p2 = pts[i * 3 + 1];
				PointF p3 = pts[i * 3 + 2];
				PointF p4 = pts[i * 3 + 3];

				PointF[] ps = ChartMath.InterpolateBezier(p1, p2, p3, p4, dsc);
				//Array.Sort( ps, new ComparerPointFByX() );

                int toolTipMid = ps.Length / 2;

				Pen penInter = new Pen(GetBrush(i).BackColor);
				for (int j = 1; j < ps.Length; j++)
				{
					Vector3D v1 = new Vector3D(ps[j - 1].X, ps[j - 1].Y, fd);
					Vector3D v2 = new Vector3D(ps[j].X, ps[j].Y, fd);
					Vector3D v3 = new Vector3D(ps[j].X, ps[j].Y, bd);
					Vector3D v4 = new Vector3D(ps[j - 1].X, ps[j - 1].Y, bd);
					ChartRegionData crd2 = null;

                    #region toolTipTopRegions  

                    ChartRegionData toolTipcrd = null;
					if (Chart.NeedRegionUpdate)
					{
						crd2 = new ChartRegionData(serIndex, i, GetToolTip(i), "Spline Renderer");
                        if (areaToolTip)
                        {
                            if (j < toolTipMid)
                            {
                                toolTipcrd = new ChartRegionData(serIndex, from + i, GetToolTip(from + i), "Spline Renderer");
                            }
                            else
                            {
                                toolTipcrd = new ChartRegionData(serIndex, from + i + 1, GetToolTip(from + i + 1), "Spline Renderer");
                            }
                        }
                    }
                    #endregion
                                        
                    Polygon plg = new Polygon(new Vector3D[] { v1, v2, v3, v4 }, GetBrush(), penInter);
                    plg.RegionData = crd2;

                        if (areaToolTip) 
                        {
                            plg.RegionData = toolTipcrd;
                        }
                        g.AddPolygon(plg);                  
				}
			}         
            
		}

		/// <summary>
		/// Draws icon.
		/// </summary>
		/// <param name="g">The <see cref="System.Drawing.Graphics"/> to render icon.</param>
		/// <param name="bounds">The bounds of the icon.</param>
		/// <param name="isShadow">The value indicates that need draw shadow.</param>
		/// <param name="shadowColor">The <see cref="System.Drawing.Color"/> to render shadow.</param>
		public override void DrawIcon(Graphics g, Rectangle bounds, bool isShadow, Color shadowColor)
		{
			int x1 = bounds.X + bounds.Width / 3;
			int x2 = bounds.X + 2 * bounds.Width / 3;
			GraphicsPath gp = new GraphicsPath();

			gp.AddCurve(new Point[5]{
                                     new Point( bounds.X, bounds.Bottom ),
                                     new Point( x1, bounds.Top ),
                                     new Point( x2, bounds.Top + bounds.Height / 2 ),
                                     new Point( bounds.Right, bounds.Top ),
                                     new Point( bounds.Right, bounds.Bottom )
                                   });
			gp.CloseFigure();

			if (isShadow)
			{
				using (SolidBrush sb = new SolidBrush(shadowColor))
				{
					g.FillPath(sb, gp);
				}
			}
			else
			{
                BrushPaint.FillPath(g, gp, SeriesStyle.Interior);                
				g.DrawPath(SeriesStyle.GdipPen, gp);
			}
		}
		#endregion
	}
}

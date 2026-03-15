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
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart.Renderers
{
	/// <summary>
	/// 
	/// </summary>
	internal class StepAreaRenderer : ChartSeriesRenderer
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
		/// <summary>
		/// Get description of regions.
		/// </summary>
		/// <value></value>
		protected override string RegionDescription
		{
			get
			{
				return "StepArea renderer region";
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// 
		/// </summary>
		/// <param name="series"></param>
		public StepAreaRenderer( ChartSeries series )
			: base(series)
		{
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Renders chart by the specified args.
		/// </summary>
		/// <param name="args">The args.</param>
		public override void Render( ChartRenderArgs2D args )
		{
			bool stepInverted = m_series.ConfigItems.StepItem.Inverted;
			bool rightToLeft = args.IsInvertedAxes ? YAxis.Inversed : XAxis.Inversed;
			bool upToDown = args.IsInvertedAxes ? XAxis.Inversed : YAxis.Inversed;
			bool needRegionUpdate = this.Chart.NeedRegionUpdate;
			bool dropPoints = Chart.DropSeriesPoints;
            bool areaToolTip = (m_series.EnableAreaToolTip && Chart.CalcRegions);         

			double origin0 = m_series.ActualYAxis.Origin;
			int yIndex = m_series.PointFormats[ChartYValueUsage.YValue];

			ChartStyleInfo seriesStyle = this.SeriesStyle;
			BrushInfo interior = this.GetBrush();
			Pen borderPen = seriesStyle.GdipPen;

			IList regions = this.ChartArea.ChartRegions;

			ChartStyledPoint[] styledPoints = PrepearePoints();
			IndexRange indexedRange = CalculateVisibleRange();
			ArrayList visiblePointsList = new ArrayList();

			ChartStyledPoint startPoint = null;
			ChartStyledPoint endPoint = null;                                             
           
            PointF previousPoint = PointF.Empty;
            PointF previousBottomPoint = PointF.Empty;
            PointF tempMidPointOne = PointF.Empty;
            PointF tempMidPointTwo = PointF.Empty;
            PointF tempCurrentPoint = PointF.Empty; 
            PointF tempCurrentBackPoint = PointF.Empty; 
            ArrayList toolTipSeriesIndex = new ArrayList();           

            int leftIndex = 0;
            int rightIndex = 0;          

			Region baseRegion = null;

			#region Add base region
			if (needRegionUpdate)
			{
				baseRegion = new Region(RectangleF.Empty);
				regions.Add(new ChartRegion(baseRegion, args.SeriesIndex, this.GetToolTip(), this.RegionDescription));
			}
			#endregion

			#region Calculate visible poitns
			ChartStyledPoint previosStyledPoint = null;
			PointF previosPoint = PointF.Empty;         

			for (int i = indexedRange.From, ci = indexedRange.To + 1; i < ci; i++)
			{
				ChartStyledPoint styledPoint = styledPoints[i];

				if (styledPoint.IsVisible)
				{
					PointF currentPoint = args.GetPoint(styledPoint.X, styledPoint.YValues[yIndex]);
                    PointF currentBottomPoint = args.GetPoint(styledPoint.X, origin0);                 

					if ((dropPoints && !previosPoint.IsEmpty)
						&& ((!args.IsInvertedAxes && Math.Abs(currentPoint.X - previosPoint.X) < 1f)
							|| (args.IsInvertedAxes && Math.Abs(currentPoint.Y - previosPoint.Y) < 1f)))
					{
						continue;
					}

					if (startPoint == null)
					{
						startPoint = styledPoint;
                        leftIndex = styledPoint.Index;
					}

					if (previosStyledPoint != null)
					{
						if (stepInverted)
						{
							visiblePointsList.Add(args.GetPoint(previosStyledPoint.X, styledPoint.YValues[yIndex]));
						}
						else
						{
							visiblePointsList.Add(args.GetPoint(styledPoint.X, previosStyledPoint.YValues[yIndex]));
						}
					}

					endPoint = styledPoint;

					#region Adds the region of point
					if (needRegionUpdate)
					{
						Region rgn = this.GetRegionFromCircle(currentPoint, styledPoint.Style.HitTestRadius);

						regions.Add(new ChartRegion(rgn, args.SeriesIndex, styledPoint.Index,
							styledPoint.ToolTip, this.RegionDescription));

                        #region AreaToolTip  FrontRegions 
                        if (areaToolTip && !previosPoint.IsEmpty)
                        {
                            //FrontRegions

                            GraphicsPath leftPath2D = new GraphicsPath();
                            GraphicsPath rightPath2D = new GraphicsPath();
                            tempCurrentPoint.X = currentPoint.X;
                            tempCurrentPoint.Y = previosPoint.Y;
                            tempCurrentBackPoint = ChartMath.AddPoint(tempCurrentPoint, args.DepthOffset);
                            tempMidPointOne.X = (previosPoint.X + tempCurrentPoint.X) / 2;
                            tempMidPointOne.Y = (previosPoint.Y + tempCurrentPoint.Y) / 2;
                            tempMidPointTwo.X = (previousBottomPoint.X + currentBottomPoint.X) / 2 ;
                            tempMidPointTwo.Y = (previousBottomPoint.Y + currentBottomPoint.Y) / 2;                    
                            leftPath2D.AddPolygon(new PointF[] { previosPoint, tempMidPointOne, tempMidPointTwo, previousBottomPoint });
                            Region lft = new Region(leftPath2D);
                            regions.Add(new ChartRegion(lft, args.SeriesIndex, previosStyledPoint.Index,
                                       previosStyledPoint.ToolTip, this.RegionDescription));                           
                            rightPath2D.AddPolygon(new PointF[] { tempMidPointOne, tempCurrentPoint, currentBottomPoint, tempMidPointTwo });
                            Region rht = new Region(rightPath2D);
                            regions.Add(new ChartRegion(rht, args.SeriesIndex, styledPoint.Index,
                                       styledPoint.ToolTip, this.RegionDescription));                        

                        }
                        #endregion

						if (args.Is3D && !previosPoint.IsEmpty)
						{
							GraphicsPath gp = new GraphicsPath();
							PointF offBStart = ChartMath.AddPoint(previosPoint, args.DepthOffset);
							PointF offBEnd = ChartMath.AddPoint(currentPoint, args.DepthOffset);
							gp.AddPolygon(new PointF[] { previosPoint, currentPoint, offBEnd, offBStart });
							baseRegion.Union(gp);


                            #region AreaToolTip TopRegions 
                            if (areaToolTip)
                            {
                                //TopRegions

                                PointF topBackLeft = ChartMath.AddPoint(previosPoint, args.DepthOffset);
                                PointF topBackRight = ChartMath.AddPoint(tempCurrentPoint, args.DepthOffset);
                                GraphicsPath leftpath = new GraphicsPath();
                                GraphicsPath rightpath = new GraphicsPath();
                                tempMidPointOne.X = (previosPoint.X + tempCurrentPoint.X) / 2;
                                tempMidPointOne.Y = (previosPoint.Y + tempCurrentPoint.Y) / 2;
                                tempMidPointTwo.X = (topBackRight.X + topBackLeft.X) / 2;
                                tempMidPointTwo.Y = (topBackRight.Y + topBackLeft.Y) / 2;
                                leftpath.AddPolygon(new PointF[] { previosPoint, tempMidPointOne, tempMidPointTwo, topBackLeft });
                                Region lft = new Region(leftpath);
                                regions.Add(new ChartRegion(lft, args.SeriesIndex, previosStyledPoint.Index,
                                           previosStyledPoint.ToolTip, this.RegionDescription));                           
                                rightpath.AddPolygon(new PointF[] { tempMidPointOne, tempCurrentPoint, topBackRight, tempMidPointTwo });
                                Region rht = new Region(rightpath);
                                regions.Add(new ChartRegion(rht, args.SeriesIndex, styledPoint.Index,
                                           styledPoint.ToolTip, this.RegionDescription));                              

                            }
                            #endregion

                            #region  AreaToolTip SideRegions 
                            if (areaToolTip)
                            {
                                PointF currentBackPoint = ChartMath.AddPoint(currentPoint, args.DepthOffset);
                                GraphicsPath leftpath = new GraphicsPath();
                                leftpath.AddPolygon(new PointF[] { tempCurrentPoint, currentPoint, currentBackPoint, tempCurrentBackPoint });
                                Region lft = new Region(leftpath);
                                regions.Add(new ChartRegion(lft, args.SeriesIndex, styledPoints[i].Index,
                                           styledPoints[i].ToolTip, this.RegionDescription));
                            }
                            #endregion                           

                            #region AreaToolTip  BottomRegions 

                            PointF bottomBackLeft = ChartMath.AddPoint(previousBottomPoint, args.DepthOffset);
                            PointF bottomBackRight = ChartMath.AddPoint(currentBottomPoint, args.DepthOffset);

                            if (areaToolTip)
                            {
                                //BottomRegions

                                GraphicsPath leftpath = new GraphicsPath();
                                GraphicsPath rightpath = new GraphicsPath();
                                tempMidPointOne.X = (previousBottomPoint.X + currentBottomPoint.X) / 2;
                                tempMidPointOne.Y = (previousBottomPoint.Y + currentBottomPoint.Y) / 2;
                                tempMidPointTwo.X = (bottomBackLeft.X + bottomBackRight.X) / 2;
                                tempMidPointTwo.Y = (bottomBackLeft.Y + bottomBackRight.Y) / 2;
                                leftpath.AddPolygon(new PointF[] { previousBottomPoint, bottomBackLeft, tempMidPointTwo, tempMidPointOne });
                                Region lft = new Region(leftpath);
                                regions.Add(new ChartRegion(lft, args.SeriesIndex, previosStyledPoint.Index,
                                           previosStyledPoint.ToolTip, this.RegionDescription));                          
                                rightpath.AddPolygon(new PointF[] { tempMidPointOne, tempMidPointTwo, bottomBackRight, currentBottomPoint });
                                Region rht = new Region(rightpath);
                                regions.Add(new ChartRegion(rht, args.SeriesIndex, styledPoint.Index,
                                           styledPoint.ToolTip, this.RegionDescription));

                            }

                            #endregion
                        }
					}
					#endregion

					visiblePointsList.Add(currentPoint);
					previosStyledPoint = styledPoint;
					previosPoint = currentPoint;

                    previousBottomPoint = currentBottomPoint;
                    rightIndex = styledPoint.Index;                
                    toolTipSeriesIndex.Add(styledPoint.Index);
				}
			}
			#endregion

			if (startPoint != null)
			{
				PointF startOrigin = args.GetPoint(startPoint.X, origin0);
				PointF endOrigin = args.GetPoint(endPoint.X, origin0);
				PointF[] points = (PointF[])visiblePointsList.ToArray(typeof(PointF));

				#region Generates the front graphics path
				GraphicsPath frontGp = new GraphicsPath();

				frontGp.AddLines(points);
				frontGp.AddLine(endOrigin, startOrigin);
				frontGp.CloseFigure();
				#endregion

				if (args.Is3D)
				{
					PointF topLeft = points[0];
					PointF topRight = points[points.Length - 1];
					PointF bottomLeft = startOrigin;
					PointF bottomRight = endOrigin;

					#region Generates graphics paths of other sides
					GraphicsPath leftGp = new GraphicsPath();
					GraphicsPath rightGp = new GraphicsPath();
					GraphicsPath bottomGp = new GraphicsPath();

					leftGp.AddPolygon(new PointF[]{ bottomLeft, topLeft,
            ChartMath.AddPoint(topLeft, args.DepthOffset), ChartMath.AddPoint(bottomLeft, args.DepthOffset)});
					rightGp.AddPolygon(new PointF[]{ bottomRight, topRight,
            ChartMath.AddPoint(topRight, args.DepthOffset), ChartMath.AddPoint(bottomRight, args.DepthOffset)});
					bottomGp.AddPolygon(new PointF[]{ bottomLeft, bottomRight, 
						ChartMath.AddPoint( bottomRight, args.DepthOffset ), ChartMath.AddPoint( bottomLeft, args.DepthOffset ) });

                    if (areaToolTip)
                    {
                        Region ttLftReg = new Region(leftGp);
                        regions.Add(new ChartRegion(ttLftReg, args.SeriesIndex, styledPoints[leftIndex].Index, styledPoints[leftIndex].ToolTip, this.RegionDescription));
                        Region ttRhtReg = new Region(rightGp);
                        regions.Add(new ChartRegion(ttRhtReg, args.SeriesIndex, styledPoints[rightIndex].Index, styledPoints[rightIndex].ToolTip, this.RegionDescription));
                    }

					#endregion
                   
					PointF preFPoint = PointF.Empty;
					PointF preBPoint = PointF.Empty;

					#region Draw the sides before top side
					args.Graph.DrawPath( interior, borderPen, rightToLeft ? rightGp : leftGp);
                    if (args.Chart.Style3D)
                        this.Draw(args.Graph, rightToLeft ? rightGp : leftGp, interior, borderPen); 

					if (!upToDown)
					{
						args.Graph.DrawPath(interior, borderPen, bottomGp);
                        if (args.Chart.Style3D)
                            this.Draw(args.Graph, bottomGp, interior, borderPen); 
					}
					#endregion

					#region Draw 3d lines
					int j = rightToLeft ? points.Length - 1 : 0;
					int cj = rightToLeft ? -1 : points.Length;
					int dj = rightToLeft ? -1 : 1;                                  
                   
                    for (; j != cj; j += dj)
					{
						PointF currFPoint = points[j];
						PointF currBPoint = ChartMath.AddPoint(currFPoint, args.DepthOffset);

    					if (!preFPoint.IsEmpty)
						{
							GraphicsPath segment = new GraphicsPath();
							segment.AddPolygon(new PointF[] { currFPoint, currBPoint, preBPoint, preFPoint });
                            args.Graph.DrawPath(interior, borderPen, segment);                                                
                            if (args.Chart.Style3D)
                                this.Draw(args.Graph, segment, interior, borderPen);               
                        }      
                                       
                            preFPoint = currFPoint;
						    preBPoint = currBPoint;                                               
                    }                                        

					#endregion

					#region Draw the sides after top side
					if (upToDown)
					{
						args.Graph.DrawPath(interior, borderPen, bottomGp);                       
                         if (args.Chart.Style3D)
                             this.Draw(args.Graph, bottomGp, interior, borderPen);
					}

					args.Graph.DrawPath(interior, borderPen, rightToLeft ? leftGp : rightGp);
                    if (args.Chart.Style3D)
                        this.Draw(args.Graph, rightToLeft ? leftGp : rightGp, interior, borderPen);
                     
					#endregion

					#region Adds the regions of left, right and bottom sides
					if (baseRegion != null)
					{
						baseRegion.Union(leftGp);
						baseRegion.Union(rightGp);
						baseRegion.Union(bottomGp);
					}
					#endregion
				}
				else
				{
					#region Draw shadow
					if (seriesStyle.DisplayShadow)
					{
						GraphicsPath shadowGP = (GraphicsPath)frontGp.Clone();
						shadowGP.Transform(new Matrix(1.0f, 0.0f, 0.0f, 1.0f,
							seriesStyle.ShadowOffset.Width, seriesStyle.ShadowOffset.Height));
						args.Graph.DrawPath(seriesStyle.ShadowInterior, null, shadowGP);
					}
					#endregion
				}

				args.Graph.DrawPath(interior, borderPen, frontGp);
                
				if (baseRegion != null)
				{
					baseRegion.Union(frontGp);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		public override void Render( Graphics3D g )
		{
			IndexRange[] ranges = this.UnEmptyRanges;

			for (int i = 0; i < ranges.Length; i++)
			{
				if ((ranges[i].To - ranges[i].From) > 1)
				{
					Render(g, ranges[i].From, ranges[i].To - ranges[i].From + 1);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="from"></param>
		/// <param name="count"></param>
		public void Render( Graphics3D g, int from, int count )
		{
			if (count > 1)
			{
				int yIndex = m_series.PointFormats[ChartYValueUsage.YValue];
				int serCount = Chart.Series.VisibleCount;
				float serDpth = GetPlaceDepth();
				float dpth = GetSeriesDepth();

				bool series3D = Chart.Series3D;
				int serIndex = Chart.Series.IndexOf(m_series);
				bool yAxisInversed = m_series.ActualYAxis.Inversed;
				bool xAxisInversed = m_series.ActualXAxis.Inversed;
				double origin0 = m_series.ActualYAxis.Origin;

                bool areaToolTip =(m_series.EnableAreaToolTip && Chart.CalcRegions);
                ArrayList toolTipTopPolygonRegions = new ArrayList();
                ArrayList toolTipFrontPolygonRegions = new ArrayList();
                ArrayList toolTipBackPolygonRegions = new ArrayList();
                ArrayList toolTipBottomPolygonRegions = new ArrayList();               
                Polygon toolTipPolyFront = null;
                Polygon toolTipPolyBack = null;
                Polygon toolTipPolyBottom = null;                                  
                int firstStyledpointNo = 0;
                int endStyledPointNo = 0;
                ArrayList seriesIndexArray = new ArrayList();                
                
                ChartPointWithIndex[] cpwiA = new ChartPointWithIndex[count];
				for (int i = 0; i < count; i++)
				{                    
					cpwiA[i] = new ChartPointWithIndex(m_series.Points[from + i], from + i);
				}
				Array.Sort(cpwiA, new ComparerPointWithIndexByX());

				ArrayList pointsWI = new ArrayList(m_series.Points.Count);
				ArrayList pointsF = new ArrayList(m_series.Points.Count);
				for (int i = 0, end = cpwiA.Length; i < end; i++)
				{
					ChartPoint cp = cpwiA[i].Point;
					int cpIndex = cpwiA[i].Index;
                    if (cp.IsEmpty)
                    {
                        continue;
                    }

					pointsWI.Add(cpwiA[i]);
					pointsF.Add(this.GetPointFromIndex(cpIndex));
				}

				PointF[] points = (PointF[])pointsF.ToArray(typeof(PointF));
				cpwiA = (ChartPointWithIndex[])pointsWI.ToArray(typeof(ChartPointWithIndex));

				PointF[] stepPoints = new PointF[points.Length * 2 - 1];
				Vector3D[] vsf = new Vector3D[stepPoints.Length + 2];
				Vector3D[] vsb = new Vector3D[stepPoints.Length + 2];

				bool inverted = m_series.ConfigItems.StepItem.Inverted;

				for (int i = 0; i < points.Length; i++)
				{
					stepPoints[2 * i] = points[i];

					if (i < points.Length - 1)
					{

						if (inverted)
						{
							ChartPoint cp = new ChartPoint(((ChartPointWithIndex)pointsWI[i]).Point.X, ((ChartPointWithIndex)pointsWI[i + 1]).Point.YValues);
							stepPoints[2 * i + 1] = new PointF(GetXFromValue(cp, yIndex), GetYFromValue(cp, yIndex));
						}

						else
						{
							ChartPoint cp = new ChartPoint(((ChartPointWithIndex)pointsWI[i + 1]).Point.X, ((ChartPointWithIndex)pointsWI[i]).Point.YValues);
							stepPoints[2 * i + 1] = new PointF(GetXFromValue(cp, yIndex), GetYFromValue(cp, yIndex));
						}
					}
				}

				GraphicsPath gp = new GraphicsPath();
				ChartPoint cpo1 = new ChartPoint(((ChartPointWithIndex)pointsWI[points.Length - 1]).Point.X, origin0);
				PointF originPoint1 = new PointF(GetXFromValue(cpo1, 0), GetYFromValue(cpo1, 0));
				ChartPoint cpo2 = new ChartPoint(((ChartPointWithIndex)pointsWI[0]).Point.X, origin0);
				PointF originPoint2 = new PointF(GetXFromValue(cpo2, 0), GetYFromValue(cpo2, 0));

				for (int i = 0; i < stepPoints.Length; i++)
				{
					vsf[i] = new Vector3D(stepPoints[i].X, stepPoints[i].Y, serDpth);
					vsb[i] = new Vector3D(stepPoints[i].X, stepPoints[i].Y, serDpth + dpth);
				}
				vsf[vsf.Length - 2] = new Vector3D(originPoint1.X, originPoint1.Y, vsf[vsf.Length - 3].Z);
				vsf[vsf.Length - 1] = new Vector3D(originPoint2.X, originPoint2.Y, vsf[0].Z);
				vsb[vsf.Length - 2] = new Vector3D(originPoint1.X, originPoint1.Y, vsb[vsb.Length - 3].Z);
				vsb[vsf.Length - 1] = new Vector3D(originPoint2.X, originPoint2.Y, vsb[0].Z);                            

                Polygon plf = new Polygon(vsf, this.GetBrush(), null);
                Polygon plbk = new Polygon(vsb, this.GetBrush(), null);
               
				Polygon pll = new Polygon(new Vector3D[]{ vsf[ 0 ], 
                                                   vsf[ vsf.Length-1 ],
                                                   vsb[ vsb.Length-1 ],
                                                   vsb[ 0 ] }, GetBrush(), SeriesStyle.GdipPen);

				Polygon plr = new Polygon(new Vector3D[]{ vsf[ vsf.Length-2 ], 
                                                   vsf[ vsf.Length-3 ],
                                                   vsb[ vsb.Length-3 ],
                                                   vsb[ vsb.Length-2 ] }, GetBrush(), SeriesStyle.GdipPen);

				Polygon plb = new Polygon(new Vector3D[]{ vsf[ vsf.Length-1 ], 
                                                   vsf[ vsf.Length-2 ],
                                                   vsb[ vsb.Length-2 ],
                                                   vsb[ vsb.Length-1 ]}, GetBrush(), SeriesStyle.GdipPen);

				ChartRegionData crd = null;

				if (Chart.NeedRegionUpdate)
				{
					string s = GetToolTip();
					crd = new ChartRegionData(serIndex, s, "StepArea Chart	Region");
				}

                if (!areaToolTip)
                {
                    plf.RegionData = crd;
                    plbk.RegionData = crd;
                    pll.RegionData = (ChartRegionData)null;
                    plr.RegionData = (ChartRegionData)null;
                    plb.RegionData = crd;
                }

				g.AddPolygon(plf);
				g.AddPolygon(plbk);
				g.AddPolygon(pll);
				g.AddPolygon(plr);
				g.AddPolygon(plb);

				for (int i = 0; i < pointsWI.Count - 1; i++)
				{
                    int cpIndex = ((ChartPointWithIndex)pointsWI[i + 1]).Index;
					Vector3D v1 = new Vector3D(stepPoints[2 * i + 1].X, stepPoints[2 * i + 1].Y, serDpth);
					Vector3D v2 = new Vector3D(stepPoints[2 * (i + 1)].X, stepPoints[2 * (i + 1)].Y, serDpth);
					Vector3D v3 = new Vector3D(stepPoints[2 * (i + 1)].X, stepPoints[2 * (i + 1)].Y, serDpth + dpth);
					Vector3D v4 = new Vector3D(stepPoints[2 * i + 1].X, stepPoints[2 * i + 1].Y, serDpth + dpth);
					ChartRegionData crd2 = null;
					if (Chart.NeedRegionUpdate)
					{
					    crd2 = new ChartRegionData(serIndex, cpIndex, GetToolTip(cpIndex), "Step Area Renderer");                        
					}
                                        
                    Polygon p = new Polygon(new Vector3D[] { v1, v2, v3, v4 }, GetBrush(), SeriesStyle.GdipPen);
                    p.RegionData = crd2;                    
					g.AddPolygon(p);
				}                             

				for (int i = 0; i < pointsWI.Count - 1; i++)
				{
					int cpIndex = ((ChartPointWithIndex)pointsWI[i]).Index;
					Vector3D v1 = new Vector3D(stepPoints[2 * i].X, stepPoints[2 * i].Y, serDpth);
					Vector3D v2 = new Vector3D(stepPoints[2 * i + 1].X, stepPoints[2 * i + 1].Y, serDpth);
					Vector3D v3 = new Vector3D(stepPoints[2 * i + 1].X, stepPoints[2 * i + 1].Y, serDpth + dpth);
					Vector3D v4 = new Vector3D(stepPoints[2 * i].X, stepPoints[2 * i].Y, serDpth + dpth);
					ChartRegionData crd2 = null;
					if (Chart.NeedRegionUpdate)
					{
						crd2 = new ChartRegionData(serIndex, cpIndex, GetToolTip(cpIndex), "Step Area Renderer");
					}
                                        
                    Polygon p = new Polygon(new Vector3D[] { v1, v2, v3, v4 }, GetBrush(), SeriesStyle.GdipPen);
                    if (!areaToolTip)
                    {
                        p.RegionData = crd2;
                    }
					g.AddPolygon(p);

                    #region AreaToolTip
                    if (areaToolTip)
                    {                                              
                        ChartPoint ttcpo1 = new ChartPoint(((ChartPointWithIndex)pointsWI[i]).Point.X,origin0);                        
                        ChartPoint ttcpo2 = new ChartPoint(((ChartPointWithIndex)pointsWI[i + 1]).Point.X,origin0);
                        PointF bottomCurrent = new PointF(GetXFromValue(ttcpo1, 0), GetYFromValue(ttcpo1, 0));
                        PointF bottomNext = new PointF(GetXFromValue(ttcpo2, 0), GetYFromValue(ttcpo2, 0));

                        Vector3D toolTipFrontLT = v1;
                        Vector3D toolTipFrontLB = new Vector3D(bottomCurrent.X, bottomCurrent.Y, serDpth);
                        Vector3D toolTipFrontRT = v2;
                        Vector3D toolTipFrontRB = new Vector3D(bottomNext.X, bottomNext.Y, serDpth); 

                        Vector3D toolTipBackLT = new Vector3D(v1.X,v1.Y,serDpth+dpth);
                        Vector3D toolTipBackLB = new Vector3D(bottomCurrent.X, bottomCurrent.Y, serDpth+dpth);
                        Vector3D toolTipBackRT = new Vector3D(v2.X,v2.Y,serDpth+dpth);
                        Vector3D toolTipBackRB = new Vector3D(bottomNext.X, bottomNext.Y, serDpth+dpth); 

                        Vector3D toolTipBottomLB = new Vector3D(bottomCurrent.X, bottomCurrent.Y, serDpth);
                        Vector3D toolTipBottomBackLB = new Vector3D(bottomCurrent.X, bottomCurrent.Y, serDpth+dpth);
                        Vector3D toolTipBottomRB = new Vector3D(bottomNext.X, bottomNext.Y, serDpth);
                        Vector3D toolTipBottomBackRB = new Vector3D(bottomNext.X, bottomNext.Y, serDpth+dpth);

                        toolTipPolyFront = new Polygon(new Vector3D[] { toolTipFrontLT, toolTipFrontRT, toolTipFrontRB, toolTipFrontLB });                    
                        toolTipPolyBack = new Polygon(new Vector3D[] { toolTipBackLT, toolTipBackLB, toolTipBackRB, toolTipBackRT });
                        toolTipPolyBottom = new Polygon(new Vector3D[] { toolTipBottomLB, toolTipBottomBackLB, toolTipBottomBackRB, toolTipBottomRB });                      
                                                                   
                        toolTipFrontPolygonRegions.Add(toolTipPolyFront);
                        toolTipBackPolygonRegions.Add(toolTipPolyBack);
                        toolTipBottomPolygonRegions.Add(toolTipPolyBottom);
                        toolTipTopPolygonRegions.Add(new Polygon(new Vector3D[] {v1,v4,v3,v2}));

                        seriesIndexArray.Add(cpIndex);
                    }
                    #endregion
                }

                seriesIndexArray.Add(((ChartPointWithIndex)pointsWI[pointsWI.Count - 1]).Index);

                #region  toolTip Left/Right PolygonRegions 
                if (areaToolTip)
                {
                    int k = seriesIndexArray.Count - 1;
                    firstStyledpointNo = (int)seriesIndexArray[0]; 
                    endStyledPointNo = (int)seriesIndexArray[k]; 
                    ChartRegionData toolTipLeftSideCrd = new ChartRegionData(serIndex, firstStyledpointNo, GetToolTip(firstStyledpointNo), "Step Area Renderer");
                    ChartRegionData toolTipRightSideCrd = new ChartRegionData(serIndex, endStyledPointNo, GetToolTip(endStyledPointNo), "Step Area Renderer");
                    pll.RegionData = toolTipLeftSideCrd;
                    plr.RegionData = toolTipRightSideCrd;
                }
                #endregion

                #region  toolTipTopPolygonRegions 

                if (areaToolTip && Chart.NeedRegionUpdate)
                {
                    for (int i = 0; (i < toolTipTopPolygonRegions.Count);i++ )                            
                    {
                        int Idx = (int)seriesIndexArray[i];
                        int IdxNxt = (int)seriesIndexArray[i + 1];
                        Polygon pg =(Polygon)toolTipTopPolygonRegions[i];
                        Vector3D topLeftV = pg.Points[0];
                        Vector3D topLeftBackV = pg.Points[1];
                        Vector3D topRightBackV = pg.Points[2];
                        Vector3D topRightV = pg.Points[3];
                        double x, y, z;
                        x = (topLeftV.X + topRightV.X) / 2;
                        y = (topLeftV.Y + topRightV.Y) / 2;
                        z = (topLeftV.Z + topRightV.Z) / 2;
                        Vector3D midPointOne = new Vector3D(x, y, z);
                        x = (topLeftBackV.X + topRightBackV.X) / 2;
                        y = (topLeftBackV.Y + topRightBackV.Y) / 2;
                        z = (topLeftBackV.Z + topRightBackV.Z) / 2;
                        Vector3D midPointTwo = new Vector3D(x, y, z);
                        Polygon leftPolygon = new Polygon(new Vector3D[] { topLeftV, topLeftBackV, midPointTwo, midPointOne });
                        Polygon rightPolygon = new Polygon(new Vector3D[] { midPointOne, midPointTwo, topRightBackV, topRightV });
                       
                        leftPolygon.RegionData = new ChartRegionData(serIndex, Idx, GetToolTip(Idx), "Step Area Renderer");                    
                        rightPolygon.RegionData = new ChartRegionData(serIndex, IdxNxt, GetToolTip(IdxNxt), "Step Area Renderer");

                        g.AddPolygon(leftPolygon);
                        g.AddPolygon(rightPolygon);
                    }
                }
                #endregion

                #region  toolTipFrontPolygonRegions

                if (areaToolTip && Chart.NeedRegionUpdate)
                {
                    for (int i = 0; (i < toolTipFrontPolygonRegions.Count);i++ )                                
                    {
                        int Idx = (int)seriesIndexArray[i];
                        int IdxNxt = (int)seriesIndexArray[i + 1];
                        Polygon pg = (Polygon)toolTipFrontPolygonRegions[i];
                        Vector3D topLeftV = pg.Points[0];
                        Vector3D topRightV = pg.Points[1];
                        Vector3D BottomRightV = pg.Points[2];
                        Vector3D BottomLeftV = pg.Points[3];
                        double x, y, z;
                        x = (topLeftV.X + topRightV.X) / 2;
                        y = (topLeftV.Y + topRightV.Y) / 2;
                        z = (topLeftV.Z + topRightV.Z) / 2;
                        Vector3D midPointOne = new Vector3D(x, y, z);
                        x = (BottomLeftV.X + BottomRightV.X) / 2;
                        y = (BottomLeftV.Y + BottomRightV.Y) / 2;
                        z = (BottomLeftV.Z + BottomRightV.Z) / 2;
                        Vector3D midPointTwo = new Vector3D(x, y, z);
                        Polygon leftPolygon = new Polygon(new Vector3D[] { topLeftV, midPointOne, midPointTwo, BottomLeftV });
                        Polygon rightPolygon = new Polygon(new Vector3D[] { midPointOne, topRightV, BottomRightV, midPointTwo });
                       
                        leftPolygon.RegionData = new ChartRegionData(serIndex, Idx, GetToolTip(Idx), "Step Area Renderer");                    
                        rightPolygon.RegionData = new ChartRegionData(serIndex, IdxNxt, GetToolTip(IdxNxt), "Step Area Renderer");  

                        g.AddPolygon(leftPolygon);
                        g.AddPolygon(rightPolygon);
                    }
                }
                #endregion

                #region  toolTipBackPolygonRegions 

                if (areaToolTip && Chart.NeedRegionUpdate)
                {
                    for (int i = 0; (i < toolTipBackPolygonRegions.Count) ; i++ )                               
                    {
                        int Idx = (int)seriesIndexArray[i];
                        int IdxNxt = (int)seriesIndexArray[i + 1];
                        Polygon pg = (Polygon)toolTipBackPolygonRegions[i];
                        Vector3D topLeftV = pg.Points[0];
                        Vector3D topRightV = pg.Points[1];
                        Vector3D BottomRightV = pg.Points[2];
                        Vector3D BottomLeftV = pg.Points[3];
                        double x, y, z;
                        x = (topLeftV.X + topRightV.X) / 2;
                        y = (topLeftV.Y + topRightV.Y) / 2;
                        z = (topLeftV.Z + topRightV.Z) / 2;
                        Vector3D midPointOne = new Vector3D(x, y, z);
                        x = (BottomLeftV.X + BottomRightV.X) / 2;
                        y = (BottomLeftV.Y + BottomRightV.Y) / 2;
                        z = (BottomLeftV.Z + BottomRightV.Z) / 2;
                        Vector3D midPointTwo = new Vector3D(x, y, z);
                        Polygon leftPolygon = new Polygon(new Vector3D[] { topLeftV, midPointOne, midPointTwo, BottomLeftV });
                        Polygon rightPolygon = new Polygon(new Vector3D[] { midPointOne, topRightV, BottomRightV, midPointTwo });
                       
                        leftPolygon.RegionData = new ChartRegionData(serIndex, Idx, GetToolTip(Idx), "Step Area Renderer");                  
                        rightPolygon.RegionData = new ChartRegionData(serIndex, IdxNxt, GetToolTip(IdxNxt), "Step Area Renderer");

                        g.AddPolygon(leftPolygon);
                        g.AddPolygon(rightPolygon);
                    }
                }
                #endregion

                #region toolTipBottomPolygonRegions 
                if (areaToolTip && Chart.NeedRegionUpdate)
                {
                    for (int i = 0; (i < toolTipBottomPolygonRegions.Count) ; i++)                              
                    {
                        int Idx = (int)seriesIndexArray[i];
                        int IdxNxt = (int)seriesIndexArray[i + 1];
                        Polygon pg = (Polygon)toolTipBottomPolygonRegions[i];
                        Vector3D bottomLeftV = pg.Points[0];
                        Vector3D bottomBackLeftV = pg.Points[1];
                        Vector3D bottomBackRightV = pg.Points[2];
                        Vector3D bottomRightV = pg.Points[3];
                        double x, y, z;
                        x = (bottomLeftV.X + bottomRightV.X) / 2;
                        y = (bottomLeftV.Y + bottomRightV.Y) / 2;
                        z = (bottomLeftV.Z + bottomRightV.Z) / 2;
                        Vector3D midPointOne = new Vector3D(x, y, z);
                        x = (bottomBackLeftV.X + bottomBackRightV.X) / 2;
                        y = (bottomBackLeftV.Y + bottomBackRightV.Y) / 2;
                        z = (bottomBackLeftV.Z + bottomBackRightV.Z) / 2;
                        Vector3D midPointTwo = new Vector3D(x, y, z);
                        Polygon leftPolygon = new Polygon(new Vector3D[] { bottomLeftV, bottomBackLeftV, midPointTwo, midPointOne });
                        Polygon rightPolygon = new Polygon(new Vector3D[] { midPointOne, midPointTwo, bottomBackRightV, bottomRightV });                       

                        leftPolygon.RegionData = new ChartRegionData(serIndex, Idx, GetToolTip(Idx), "Step Area Renderer");                        
                        rightPolygon.RegionData = new ChartRegionData(serIndex, IdxNxt, GetToolTip(IdxNxt), "Step Area Renderer");              

                        g.AddPolygon(leftPolygon);
                        g.AddPolygon(rightPolygon);
                    }
                }
                #endregion

			}
		}
		#endregion
	}
}
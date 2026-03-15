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
  /// The StackingArea chart renderering class.
  /// </summary>
  internal class StackingAreaRenderer : ChartSeriesRenderer
  {
    #region Properties
		/// <summary>
		/// </summary>
		/// <value></value>
    public override ChartUsedSpaceType FillSpaceType
    {
      get
      {
        return ChartUsedSpaceType.OneForAll;
      }
    }
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
				return "Stacking Area Chart Region";
			}
		}
    #endregion

    #region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="StackingAreaRenderer"/> class.
		/// </summary>
		/// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
    public StackingAreaRenderer( ChartSeries series )
      : base( series )
    {
    }
    #endregion

    #region Public methods
		/// <summary>
		/// Renders chart by the specified args.
		/// </summary>
		/// <param name="args">The args.</param>
		public override void Render(ChartRenderArgs2D args)
		{
			bool rightToLeft = args.IsInvertedAxes ? YAxis.Inversed : XAxis.Inversed;
			bool upToDown = args.IsInvertedAxes ? XAxis.Inversed : YAxis.Inversed;
			bool needRegionUpdate = this.Chart.NeedRegionUpdate;
			bool dropPoints = Chart.DropSeriesPoints;
            bool areaToolTip = (m_series.EnableAreaToolTip && Chart.CalcRegions);

			ChartStyleInfo seriesStyle = this.SeriesStyle;
			BrushInfo interior = this.GetBrush();
			Pen borderPen = seriesStyle.GdipPen;                     
            
            int leftIndex = 0;
            int rightIndex = 0;
          
            PointF tempMidPt1 = PointF.Empty;
            PointF tempMidPt2 = PointF.Empty;

			IList regions = this.ChartArea.ChartRegions;

			ChartStyledPoint[] styledPoints = this.PrepearePoints();
			IndexRange indexedRange = CalculateVisibleRange();
			ArrayList visibleTopPointsList = new ArrayList();
			ArrayList visibleBottomPointsList = new ArrayList();

			bool isTopSeries = false;
			bool isBottomSeries = false;

			IList visibleList = m_series.ChartModel.Series.VisibleList;

			for (int i = 0; i < visibleList.Count; i++)
			{
				ChartSeries series = visibleList[i] as ChartSeries;

				if( series.Type == m_series.Type )
				{
					isBottomSeries = series == m_series;
					break;
				}
			}

			for (int i = visibleList.Count - 1; i > -1; i--)
			{
				ChartSeries series = visibleList[i] as ChartSeries;

				if (series.Type == m_series.Type)
				{
					isTopSeries = series == m_series;
					break;
				}
			}

			Region baseRegion = null;

			#region Add base region
			if (needRegionUpdate)
			{
				baseRegion = new Region(RectangleF.Empty);
				regions.Add(new ChartRegion(baseRegion, args.SeriesIndex, this.GetToolTip(), this.RegionDescription));
			}
			#endregion

			#region Calculate visible poitns
			PointF previosPoint1 = PointF.Empty;
			PointF previosPoint2 = PointF.Empty;
			ChartStyledPoint previosChartPoint = null;

			for (int i = indexedRange.From, ci = indexedRange.To + 1; i < ci; i++)
			{
				ChartStyledPoint styledPoint = styledPoints[i];

				if (styledPoint.IsVisible)
				{
					double y1 = this.GetStackInfoValue(styledPoint.Index, false);
					double y2 = this.GetStackInfoValue(styledPoint.Index, true);

					PointF currentPoint1 = args.GetPoint(styledPoint.X, y1);
					PointF currentPoint2 = args.GetPoint(styledPoint.X, y2);

					if (dropPoints && previosChartPoint != null)
					{
						float delta = args.IsInvertedAxes ? currentPoint1.Y - previosPoint1.Y : currentPoint1.X - previosPoint1.X;

						if (Math.Abs(delta) < 1f)
						{
							continue;
						}
					}

					bool reverse = (y1 > y2) ^ upToDown;

                    if (previosChartPoint != null && reverse)
                    {
                        PointF crossPoint = ChartMath.LineSegmentIntersectionPoint(previosPoint1, currentPoint1, previosPoint2, currentPoint2);

                        //if (!crossPoint.IsEmpty)//Hide this code. If not, StackingArea and StackingArea100 charts are not working properly when Axis's Inversed property set to true.
                        //{
                        //    visibleTopPointsList.Add(crossPoint);
                        //    visibleBottomPointsList.Add(crossPoint);
                        //}
                    }

					if (reverse)
					{
						PointF swapPoint = currentPoint1;
						currentPoint1 = currentPoint2;
						currentPoint2 = swapPoint;
					}

					visibleTopPointsList.Add(currentPoint2);
					visibleBottomPointsList.Add(currentPoint1);

                    #region AreaToolTip

                    if (previosChartPoint==null)
                        leftIndex = i;

                    rightIndex = i;

                    if (areaToolTip && (previosChartPoint != null))
                    {         
                        GraphicsPath left = new GraphicsPath();
                        GraphicsPath right = new GraphicsPath();                       

                        PointF ttFrontTopLeft = previosPoint2;
                        PointF ttFrontTopRight = currentPoint2;
                        PointF ttFrontBtmLeft = previosPoint1;
                        PointF ttFrontBtmRight = currentPoint1;

                        if (reverse)
                        {
                            ttFrontTopLeft = previosPoint1;
                            ttFrontTopRight = currentPoint1;
                            ttFrontBtmLeft = previosPoint2;
                            ttFrontBtmRight = currentPoint2;

                        }

                        tempMidPt1.X = (ttFrontTopLeft.X + ttFrontTopRight.X) / 2;
                        tempMidPt1.Y = (ttFrontTopLeft.Y + ttFrontTopRight.Y) / 2;
                        tempMidPt2.X = (ttFrontBtmLeft.X + ttFrontBtmRight.X) / 2;
                        tempMidPt2.Y = (ttFrontBtmLeft.Y + ttFrontBtmRight.Y) / 2;

                        if (reverse)
                        {
                            left.AddPolygon(new PointF[] { previosPoint2, previosPoint1, tempMidPt1, tempMidPt2 });
                            right.AddPolygon(new PointF[] { tempMidPt1, currentPoint1, currentPoint2, tempMidPt2 });
                        }
                        else
                        {
                            left.AddPolygon(new PointF[] { previosPoint2, tempMidPt1, tempMidPt2, previosPoint1 });
                            right.AddPolygon(new PointF[] { tempMidPt1, currentPoint2, currentPoint1, tempMidPt2 });
                        }                       

                        Region lft = new Region(left);
                        Region rht = new Region(right);                                              

                        regions.Add(new ChartRegion(lft, args.SeriesIndex, previosChartPoint.Index, GetToolTip(previosChartPoint.Index), this.RegionDescription));
                        regions.Add(new ChartRegion(rht, args.SeriesIndex, styledPoint.Index, GetToolTip(styledPoint.Index), this.RegionDescription));

                        if (args.Is3D)
                        {
                            PointF ttBtmLeft = ttFrontBtmLeft;
                            PointF ttBtmRight = ttFrontBtmRight;
                            PointF ttBtmbackLeft = ChartMath.AddPoint(ttBtmLeft, args.DepthOffset);
                            PointF ttBtmbackRight = ChartMath.AddPoint(ttBtmRight, args.DepthOffset);

                            PointF ttTopLeft = ttFrontTopLeft;
                            PointF ttTopRight = ttFrontTopRight;
                            PointF ttTopbackLeft = ChartMath.AddPoint(ttTopLeft, args.DepthOffset);
                            PointF ttTopbackRight = ChartMath.AddPoint(ttTopRight, args.DepthOffset);

                            if (isTopSeries)
                            {
                                GraphicsPath leftTopGp = new GraphicsPath();
                                GraphicsPath rightTopGp = new GraphicsPath();

                                tempMidPt1.X = (ttTopLeft.X + ttTopRight.X) / 2;
                                tempMidPt1.Y = (ttTopLeft.Y + ttTopRight.Y) / 2;
                                tempMidPt2.X = (ttTopbackLeft.X + ttTopbackRight.X) / 2;
                                tempMidPt2.Y = (ttTopbackLeft.Y + ttTopbackRight.Y) / 2;

                                leftTopGp.AddPolygon(new PointF[] { ttTopLeft, ttTopbackLeft, tempMidPt2, tempMidPt1 });                                
                                Region topLftReg = new Region(leftTopGp);
                                regions.Add(new ChartRegion(topLftReg, args.SeriesIndex, previosChartPoint.Index, GetToolTip(previosChartPoint.Index), this.RegionDescription));

                                rightTopGp.AddPolygon(new PointF[] { tempMidPt1, tempMidPt2, ttTopbackRight, ttTopRight });                               
                                Region topRhtReg = new Region(rightTopGp);
                                regions.Add(new ChartRegion(topRhtReg, args.SeriesIndex, styledPoint.Index, GetToolTip(styledPoint.Index), this.RegionDescription));
                            }

                            if (isBottomSeries)
                            {
                                GraphicsPath leftBtmGp = new GraphicsPath();
                                GraphicsPath rightBtmGp = new GraphicsPath();

                                tempMidPt1.X = (ttBtmLeft.X + ttBtmRight.X) / 2;
                                tempMidPt1.Y = (ttBtmLeft.Y + ttBtmRight.Y) / 2;
                                tempMidPt2.X = (ttBtmbackLeft.X + ttBtmbackRight.X) / 2;
                                tempMidPt2.Y = (ttBtmbackLeft.Y + ttBtmbackRight.Y) / 2;

                                leftBtmGp.AddPolygon(new PointF[] { ttBtmLeft, ttBtmbackLeft, tempMidPt2, tempMidPt1 });                                
                                Region btmLftReg = new Region(leftBtmGp);
                                regions.Add(new ChartRegion(btmLftReg, args.SeriesIndex, previosChartPoint.Index, GetToolTip(previosChartPoint.Index), this.RegionDescription));

                                rightBtmGp.AddPolygon(new PointF[] { tempMidPt1, tempMidPt2, ttBtmbackRight, ttBtmRight });                                
                                Region btmRhtReg = new Region(rightBtmGp);
                                regions.Add(new ChartRegion(btmRhtReg, args.SeriesIndex, styledPoint.Index, GetToolTip(styledPoint.Index), this.RegionDescription));
                            }

                        }
                    }
                    #endregion

                    previosChartPoint = styledPoint;
					previosPoint1 = currentPoint1;
					previosPoint2 = currentPoint2;

               
				}
			}
			#endregion

			if (visibleTopPointsList.Count > 0)
			{
				PointF[] points1 = (PointF[])visibleTopPointsList.ToArray(typeof(PointF));
				PointF[] points2 = (PointF[])visibleBottomPointsList.ToArray(typeof(PointF));

				Array.Reverse(points2);

				#region Generates the front graphics path
				GraphicsPath frontGp = new GraphicsPath();

				frontGp.AddLines(points1);
				frontGp.AddLines(points2);

				frontGp.CloseFigure();
				#endregion

				if (args.Is3D)
				{
					PointF topLeft = points1[0];
					PointF bottomLeft = points2[points2.Length - 1];
					PointF topRight = points1[points1.Length - 1];
					PointF bottomRight = points2[0];

					#region Generates graphics paths of other sides

                    GraphicsPath leftGp = new GraphicsPath();
					GraphicsPath rightGp = new GraphicsPath();

					leftGp.AddPolygon(new PointF[]{ bottomLeft, topLeft,
            ChartMath.AddPoint(topLeft, args.DepthOffset), ChartMath.AddPoint(bottomLeft, args.DepthOffset)});
					rightGp.AddPolygon(new PointF[]{ bottomRight, topRight,
            ChartMath.AddPoint(topRight, args.DepthOffset), ChartMath.AddPoint(bottomRight, args.DepthOffset)});
					#endregion

					//args.Graph.DrawPath(interior, borderPen, rightToLeft ? rightGp : leftGp);

					#region Draw 3d lines
					bool down = Math.Abs(args.DepthOffset.Width) > Math.Abs(args.DepthOffset.Height);

					PointF[] vPoints1 = upToDown ? points2 : points1;
					PointF[] vPoints2 = upToDown ? points1 : points2;

					PointF preFPoint1 = PointF.Empty;
					PointF preBPoint1 = PointF.Empty;

					PointF preFPoint2 = PointF.Empty;
					PointF preBPoint2 = PointF.Empty;

					int sj = rightToLeft ? vPoints1.Length - 1 : 0;
					int cj = rightToLeft ? -1 : vPoints1.Length;
					int dj = rightToLeft ? -1 : 1;

					for (int j = sj; j != cj; j += dj)
					{
						PointF currFPoint1 = vPoints1[j];
						PointF currBPoint1 = ChartMath.AddPoint(currFPoint1, args.DepthOffset);

						PointF currFPoint2 = vPoints2[vPoints2.Length - j - 1];
						PointF currBPoint2 = ChartMath.AddPoint(currFPoint2, args.DepthOffset);

						if (j != sj)
						{
							bool orde12 = false;
							GraphicsPath segment1 = new GraphicsPath();
							GraphicsPath segment2 = new GraphicsPath();

							segment1.AddPolygon(new PointF[] { currFPoint1, currBPoint1, preBPoint1, preFPoint1 });
							segment2.AddPolygon(new PointF[] { currFPoint2, currBPoint2, preBPoint2, preFPoint2 });

                            if (args.IsInvertedAxes)
                            {
                                orde12 = currFPoint2.X >= preFPoint2.X;
                                //orde12 &= Math.Abs(args.DepthOffset.Width / args.DepthOffset.Height) < Math.Abs((currFPoint2.X - preFPoint2.X) / (currFPoint2.Y - preFPoint2.Y));
                            }
                            else
                            {
                                orde12 = currFPoint2.Y <= preFPoint2.Y;
                                //orde12 &= Math.Abs(args.DepthOffset.Width / args.DepthOffset.Height) > Math.Abs((currFPoint2.X - preFPoint2.X) / (currFPoint2.Y - preFPoint2.Y));
                            }

                            if (isTopSeries ^ args.ActualYAxis.Inversed)
                            {
                                args.Graph.DrawPath(interior, borderPen, segment1);
                            }

                            if (!orde12 && (isBottomSeries ^ args.ActualYAxis.Inversed))
                            {
                                args.Graph.DrawPath(interior, borderPen, segment2);
                            }
						}

						preFPoint1 = currFPoint1;
						preBPoint1 = currBPoint1;

						preFPoint2 = currFPoint2;
						preBPoint2 = currBPoint2;
					}
					#endregion

					args.Graph.DrawPath(interior, borderPen, rightToLeft ? leftGp : rightGp);

					#region Adds the regions of left, right and bottom sides
					if (baseRegion != null)
					{
						baseRegion.Union(leftGp);
						baseRegion.Union(rightGp);
					}
					#endregion

                    if (areaToolTip && (baseRegion != null))
                    {
                        regions.Add(new ChartRegion(new Region(leftGp), args.SeriesIndex, leftIndex, GetToolTip(leftIndex), this.RegionDescription));
                        regions.Add(new ChartRegion(new Region(rightGp), args.SeriesIndex, rightIndex, GetToolTip(rightIndex), this.RegionDescription));
                    }
				}
				else
				{
					#region Draw shadow
					if (seriesStyle.DisplayShadow)
					{
						GraphicsPath shadowGP = (GraphicsPath)frontGp.Clone();
						shadowGP.Transform(new Matrix(1.0f, 0.0f, 0.0f, 1.0f,
							seriesStyle.ShadowOffset.Width, seriesStyle.ShadowOffset.Height));
						args.Graph.DrawPath(interior, null, shadowGP);
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
		public override void Render(Graphics3D g)
		{
			PointF orig = OriginLocation;
			Pen borderpen = SeriesStyle.GdipPen as Pen;
			BrushInfo brush = GetBrush();
			int serIndex = Chart.Series.IndexOf(m_series);
			string toolTipText = GetToolTip();
			float offset = GetPlaceDepth();
			float dpth = GetSeriesDepth();
			bool yAxisInversed = m_series.YAxis.Inversed;
			int iMost = Chart.Series.Count - 1;

            bool areaToolTip = (m_series.EnableAreaToolTip && Chart.CalcRegions);                   

            ArrayList toolTipTopPolygonRegions = new ArrayList();
            ArrayList toolTipFrontPolygonRegions = new ArrayList();
            ArrayList toolTipBackPolygonRegions = new ArrayList();
            ArrayList toolTipBottomPolygonRegions = new ArrayList();
            Polygon toolTipPolyTop = null;
            Polygon toolTipPolyFront = null;
            Polygon toolTipPolyBack = null;
            Polygon toolTipPolyBottom = null;                                 
            int firstStyledpointNo = 0;
            int endStyledPointNo = 0;
            ArrayList seriesIndexArray = new ArrayList();

			g.AddPolygon(CreateBoundsPolygon(offset));

			ChartRegionData crd = null;

			if (Chart.NeedRegionUpdate)
			{
				string s = GetToolTip();
				crd = new ChartRegionData(Chart.Series.IndexOf(m_series), s, this.RegionDescription);
			}

			#region getting stacking series
			for (; iMost > 0; iMost--)
			{
				if ((Chart.Series[iMost].Type == ChartSeriesType.StackingArea)
					&& (Chart.Series[iMost].Visible))
				{
					break;
				}
			}
			bool isMostTopSeries = (serIndex == iMost);

			int iLower = 0;

			for (; iLower <= iMost; iLower++)
			{

				if ((Chart.Series[iLower].Type == ChartSeriesType.StackingArea)
					&& (Chart.Series[iLower].Visible))
				{
					break;
				}
			}
			bool isMostLowerSeries = (serIndex == iLower);
			#endregion

			ChartPointWithIndex[] cpwiA = new ChartPointWithIndex[m_series.Points.Count];
			for (int i = 0; i < cpwiA.Length; i++)
			{
				cpwiA[i] = new ChartPointWithIndex(m_series.Points[i], i);
			}
			if (Chart.EnableXZooming)
				Array.Sort(cpwiA, new ComparerPointWithIndexByX());

			ArrayList pointsWI = new ArrayList(m_series.Points.Count);
			ArrayList pointsF = new ArrayList(m_series.Points.Count);
			ArrayList pointsF2 = new ArrayList(m_series.Points.Count);

			for (int i = 0, end = cpwiA.Length; i < end; i++)
			{
				ChartPoint cp = cpwiA[i].Point;
				int cpIndex = cpwiA[i].Index;
				if (cp.IsEmpty)
				{
					continue;
				}

				pointsWI.Add(cpwiA[i]);

				double y = this.GetStackInfoValue(cpIndex);
				ChartPoint tcp1 = new ChartPoint(cp.X, this.GetStackInfoValue(cpIndex, true));
				PointF ptF = new PointF(this.GetXFromValue(tcp1, 0), this.GetYFromValue(tcp1, 0));
				pointsF.Add(ptF);

				if (Chart.NeedRegionUpdate)
				{
					ChartStyleInfo style = GetStyleAt(cpIndex);
					Path3D gp3d = GetPath3DFromCircle(new Vector3D(ptF.X, ptF.Y, offset), style.HitTestRadius);
					//ChartArea.ChartRegions.Add( new ChartRegion( this.GetRegionFromCircle( ptF, style.HitTestRadius ), GetToolTip( i ),"Stacking Area Chart Region") );
					gp3d.RegionData = new ChartRegionData(serIndex, i, GetToolTip(i), this.RegionDescription);
					g.AddPolygon(gp3d);
				}


                if(areaToolTip)
                    seriesIndexArray.Add(i);

				tcp1 = new ChartPoint(cp.X, y);
				pointsF2.Add(new PointF(this.GetXFromValue(tcp1, 0), this.GetYFromValue(tcp1, 0)));
			}
          

			// sorting pointsF2 in back order
			for (int i = 0, end = pointsF2.Count, end2 = pointsF2.Count / 2; i < end2; i++)
			{
				PointF tp = (PointF)pointsF2[i];
				int tind = end - i - 1;
				pointsF2[i] = pointsF2[tind];
				pointsF2[tind] = tp;
			}

			PointF[] points = (PointF[])pointsF.ToArray(typeof(PointF));
			PointF[] points2 = (PointF[])pointsF2.ToArray(typeof(PointF));
			cpwiA = (ChartPointWithIndex[])pointsWI.ToArray(typeof(ChartPointWithIndex));

			Vector3D[] vsfr = new Vector3D[points.Length + points2.Length];
			Vector3D[] vsbk = new Vector3D[points.Length + points2.Length];

			int count = m_series.Points.Count;
			int count1 = points.Length;
			int count2 = points2.Length;

			for (int i = 0; i < count1; i++)
			{
				vsfr[i] = new Vector3D(points[i].X, points[i].Y, offset);
				vsbk[i] = new Vector3D(points[i].X, points[i].Y, offset + dpth);
			}

			for (int i = 0; i < count2; i++)
			{
				vsfr[i + count1] = new Vector3D(points2[i].X, points2[i].Y, offset);
				vsbk[i + count1] = new Vector3D(points2[i].X, points2[i].Y, offset + dpth);
			}            

            Polygon plfr = new Polygon(vsfr, brush);
            Polygon plbk = new Polygon(vsbk, brush);

            Polygon pllf = new Polygon(new Vector3D[]{ vsfr[ 0 ], vsbk[ 0 ], 
                                                  vsbk[ vsbk.Length-1 ], 
                                                  vsfr[ vsfr.Length-1 ] },
                brush, SeriesStyle.GdipPen);

            Polygon plrh = new Polygon(new Vector3D[]{ vsfr[ count1-1 ], 
                                                  vsbk[ count1-1 ], 
                                                  vsbk[ count1 ],
                                                  vsfr[ count1 ] },
                brush, SeriesStyle.GdipPen);
                    
			plfr.RegionData = (ChartRegionData)null;//crd;
			plbk.RegionData = crd;
			pllf.RegionData = (ChartRegionData)null;//crd;
			plrh.RegionData = (ChartRegionData)null;//crd;

			g.AddPolygon(plfr);
			g.AddPolygon(plbk);
			g.AddPolygon(pllf);
			g.AddPolygon(plrh);

			for (int i = 0; i < count1 - 1; i++)
			{
				Vector3D v1 = new Vector3D(points[i].X, points[i].Y, offset);
				Vector3D v2 = new Vector3D(points[i].X, points[i].Y, offset + dpth);
				Vector3D v3 = new Vector3D(points[i + 1].X, points[i + 1].Y, offset + dpth);
				Vector3D v4 = new Vector3D(points[i + 1].X, points[i + 1].Y, offset);

				ChartRegionData crd2 = null;
				if (Chart.NeedRegionUpdate)
				{
					crd2 = new ChartRegionData(serIndex, i, GetToolTip(i), this.RegionDescription);
				}
                				
                Polygon pl = new Polygon(new Vector3D[] { v1, v2, v3, v4 }, GetBrush(), SeriesStyle.GdipPen);
				pl.RegionData = crd2;
				g.AddPolygon(pl);
			}

			for (int i = 0; i < count2 - 1; i++)
			{
				Vector3D v1 = new Vector3D(points2[i].X, points2[i].Y, offset);
				Vector3D v2 = new Vector3D(points2[i].X, points2[i].Y, offset + dpth);
				Vector3D v3 = new Vector3D(points2[i + 1].X, points2[i + 1].Y, offset + dpth);
				Vector3D v4 = new Vector3D(points2[i + 1].X, points2[i + 1].Y, offset);
                				
                Polygon pl = new Polygon(new Vector3D[] { v1, v2, v3, v4 }, brush, null);
               	pl.RegionData = (ChartRegionData)null;
				g.AddPolygon(pl);
            }

            #region AreaToolTip 
            if (areaToolTip)
            {
                for (int i = 0, j = count2 - 1; (i < count1 - 1) && (j>0) ; i++,j--)
                {
                    Vector3D FTopL = new Vector3D(points[i].X, points[i].Y, offset);
                    Vector3D BTopL = new Vector3D(points[i].X, points[i].Y, offset + dpth);
                    Vector3D BTopR = new Vector3D(points[i + 1].X, points[i + 1].Y, offset + dpth);
                    Vector3D FTopR = new Vector3D(points[i + 1].X, points[i + 1].Y, offset);               

                    Vector3D FBtmL = new Vector3D(points2[j].X, points2[j].Y, offset);
                    Vector3D BBtmL = new Vector3D(points2[j].X, points2[j].Y, offset + dpth);
                    Vector3D BBtmR = new Vector3D(points2[j - 1].X, points2[j-1].Y, offset + dpth);
                    Vector3D FBtmR = new Vector3D(points2[j - 1].X, points2[j-1].Y, offset);
               
                    toolTipPolyTop = new Polygon(new Vector3D[] { FTopL,BTopL,BTopR,FTopR });
                    toolTipPolyFront = new Polygon(new Vector3D[] { FTopL, FTopR, FBtmR, FBtmL });
                    toolTipPolyBack = new Polygon(new Vector3D[] { BTopL, BBtmL, BBtmR, BTopR });
                    toolTipPolyBottom = new Polygon(new Vector3D[] { FBtmL, BBtmL, BBtmR, FBtmR });

                    toolTipTopPolygonRegions.Add(toolTipPolyTop);
                    toolTipFrontPolygonRegions.Add(toolTipPolyFront);
                    toolTipBackPolygonRegions.Add(toolTipPolyBack);
                    toolTipBottomPolygonRegions.Add(toolTipPolyBottom);                    
                }
            }                                  

            #region  toolTip Left/Right PolygonRegions
            if (areaToolTip &&Chart.NeedRegionUpdate)
            {
                int i = seriesIndexArray.Count - 1;
                firstStyledpointNo = (int)seriesIndexArray[0];
                endStyledPointNo = (int)seriesIndexArray[i];

                ChartRegionData toolTipLeftSideCrd = new ChartRegionData(serIndex, firstStyledpointNo, GetToolTip(firstStyledpointNo), this.RegionDescription);
                ChartRegionData toolTipRightSideCrd = new ChartRegionData(serIndex, endStyledPointNo, GetToolTip(endStyledPointNo), this.RegionDescription);
                pllf.RegionData = toolTipLeftSideCrd; 
                plrh.RegionData = toolTipRightSideCrd;
                g.AddPolygon(pllf);
                g.AddPolygon(plrh);
            }


            #endregion

            #region  toolTipTopPolygonRegions 

            if (areaToolTip && Chart.NeedRegionUpdate)
            {
                for (int i = 0; (i < toolTipTopPolygonRegions.Count); i++)                               
                {
                    int Idx = (int)seriesIndexArray[i];
                    int IdxNxt = (int)seriesIndexArray[i+1];
                    Polygon pg = (Polygon)toolTipTopPolygonRegions[i];
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
                    leftPolygon.RegionData = new ChartRegionData(serIndex,
                                  Idx, GetToolTip(Idx), this.RegionDescription);
                    rightPolygon.RegionData = new ChartRegionData(serIndex,
                                   IdxNxt, GetToolTip(IdxNxt), this.RegionDescription);
                    g.AddPolygon(leftPolygon);
                    g.AddPolygon(rightPolygon);
                } 
            }
            #endregion

            #region  toolTipFrontPolygonRegions

            if (areaToolTip && Chart.NeedRegionUpdate)
            {
                for (int i = 0; (i < toolTipFrontPolygonRegions.Count); i++)                                
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
                    leftPolygon.RegionData = new ChartRegionData(serIndex,
                                       Idx, GetToolTip(Idx), this.RegionDescription);
                    rightPolygon.RegionData = new ChartRegionData(serIndex,
                                   IdxNxt, GetToolTip(IdxNxt), this.RegionDescription);
                    g.AddPolygon(leftPolygon);
                    g.AddPolygon(rightPolygon);
                }
            }
            #endregion

            #region  toolTipBackPolygonRegions

            if (areaToolTip && Chart.NeedRegionUpdate)
            {
                for (int i = 0; (i < toolTipBackPolygonRegions.Count); i++)                            
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
                    leftPolygon.RegionData = new ChartRegionData(serIndex,
                                  Idx, GetToolTip(Idx), this.RegionDescription);
                    rightPolygon.RegionData = new ChartRegionData(serIndex,
                                   IdxNxt, GetToolTip(IdxNxt), this.RegionDescription);
                    g.AddPolygon(leftPolygon);
                    g.AddPolygon(rightPolygon);
                }
            }
            #endregion

            #region toolTipBottomPolygonRegions
            if (areaToolTip && Chart.NeedRegionUpdate)
            {
                for (int i = 0; (i < toolTipBottomPolygonRegions.Count); i++)                               
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
                    leftPolygon.RegionData = new ChartRegionData(serIndex,
                                 Idx, GetToolTip(Idx), this.RegionDescription);
                    rightPolygon.RegionData = new ChartRegionData(serIndex,
                                   IdxNxt, GetToolTip(IdxNxt), this.RegionDescription);          
                    g.AddPolygon(leftPolygon);
                    g.AddPolygon(rightPolygon);
                }
            }

            #endregion

            #endregion


        }
		/// <summary>
		/// Draws the icon on the legend.
		/// </summary>
		/// <param name="g">Instance of <see cref="Graphics"/>.</param>
		/// <param name="bounds">Bounds of icon.</param>
		/// <param name="isShadow">If is true method draws the shadow.</param>
		/// <param name="shadowColor"><see cref="Color"/> of shadow.</param>
    public override void DrawIcon( Graphics g, Rectangle bounds, bool isShadow, Color shadowColor )
    {
      int x1 = bounds.X + bounds.Width / 3;
      int x2 = bounds.X + 2 * bounds.Width / 3;
      GraphicsPath gp = new GraphicsPath();

      gp.AddPolygon( new Point[ 5 ]{
                                     new Point( bounds.X, bounds.Bottom ),
                                     new Point( x1, bounds.Top ),
                                     new Point( x2, bounds.Top + bounds.Height / 2 ),
                                     new Point( bounds.Right, bounds.Top ),
                                     new Point( bounds.Right, bounds.Bottom )
                                   });

      if( isShadow )
      {
        using( SolidBrush sb = new SolidBrush( shadowColor ))
        {
          g.FillPath( sb, gp );
        }
      }
      else
      {
          BrushPaint.FillPath(g, gp, SeriesStyle.Interior);          
          g.DrawPath( SeriesStyle.GdipPen, gp ); 
      }
    }
		/// <summary>
		/// Measures the X range.
		/// </summary>
		/// <returns></returns>
		public override DoubleRange GetYDataMeasure()
		{           
            double max = 0;
            double min = 0;

			for (int i = 0; i < m_series.Points.Count; i++)
			{
				double val = this.GetStackInfoValue(i, true);

				if (val > max)
				{
					max = val;
				}

				if (val < min)
				{
					min = val;
				}
			}

			DoubleRange range = new DoubleRange(min, max);

			if (m_series.OriginDependent)
			{
				if (m_series.ActualYAxis.CustomOrigin)
				{
					range += m_series.ActualYAxis.Origin;
				}
				else
				{
					range += 0d;
				}
			}

			return range;

		}
    #endregion
  }
}

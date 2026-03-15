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
	internal class AreaRenderer : ChartSeriesRenderer
	{
		#region Constants
		private const string c_areaRegionDescription = "Area Chart Renderer";
		#endregion

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
		/// Initializes a new instance of the <see cref="AreaRenderer"/> class.
		/// </summary>
		/// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
		public AreaRenderer(ChartSeries series)
			: base(series)
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

			int yIndex = args.Series.PointFormats[ChartYValueUsage.YValue];
			double origin0 = m_series.ActualYAxis.Origin;

			ChartStyleInfo seriesStyle = this.SeriesStyle;
			BrushInfo interior = this.GetBrush();
			Pen borderPen = seriesStyle.GdipPen;          

			IList regions = this.ChartArea.ChartRegions;

			ChartStyledPoint[] styledPoints = PrepearePoints();
			IndexRange indexedRange = CalculateVisibleRange();
			ArrayList visiblePointsList = new ArrayList();

			ChartStyledPoint startPoint = null;
			ChartStyledPoint endPoint = null;                   

			Region baseRegion = null;

			#region Add base region
			if (needRegionUpdate)
			{
				baseRegion = new Region(RectangleF.Empty);
				regions.Add(new ChartRegion(baseRegion, args.SeriesIndex, this.GetToolTip(), c_areaRegionDescription));               
			}
			#endregion

			#region Calculate visible poitns
			PointF previosPoint = PointF.Empty;
            PointF previousBottomPoint = PointF.Empty;               
            PointF tempMidPointOne = PointF.Empty;
            PointF tempMidPointTwo = PointF.Empty;
            ChartStyledPoint previousStyledPoint = null;
            int firstStyledPoint = 0;
            int lastStyledPoint = 0;

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
                            firstStyledPoint = i;
                        }

                        endPoint = styledPoint;

                        #region Adds the region of point
                        if (needRegionUpdate)
                        {
                            Region rgn = this.GetRegionFromCircle(currentPoint, styledPoint.Style.HitTestRadius);                                 
                            regions.Add(new ChartRegion(rgn, args.SeriesIndex, styledPoint.Index, styledPoint.ToolTip, c_areaRegionDescription));                                 

                            #region AreaToolTip 

                            if (areaToolTip && !previosPoint.IsEmpty)
                            {
                                //FrontRegions

                                GraphicsPath leftPath2D = new GraphicsPath();
                                GraphicsPath rightPath2D = new GraphicsPath();
                                tempMidPointOne.X = (previosPoint.X + currentPoint.X) / 2;
                                tempMidPointOne.Y = (previosPoint.Y + currentPoint.Y) / 2;
                                tempMidPointTwo.X = (previousBottomPoint.X + currentBottomPoint.X) / 2;
                                tempMidPointTwo.Y = (previousBottomPoint.Y + currentBottomPoint.Y) / 2;
                                leftPath2D.AddPolygon(new PointF[] { previosPoint, tempMidPointOne, tempMidPointTwo, previousBottomPoint });
                                Region lft = new Region(leftPath2D);                        
                                regions.Add(new ChartRegion(lft, args.SeriesIndex, previousStyledPoint.Index,
                                          previousStyledPoint.ToolTip, c_areaRegionDescription)); 
                                rightPath2D.AddPolygon(new PointF[] { tempMidPointOne, currentPoint, currentBottomPoint, tempMidPointTwo });
                                Region rht = new Region(rightPath2D);                            
                                regions.Add(new ChartRegion(rht, args.SeriesIndex, styledPoint.Index,
                                         styledPoint.ToolTip, c_areaRegionDescription));                           
                                
                            }
                            #endregion

                            if (args.Is3D && !previosPoint.IsEmpty)
                            {
                                GraphicsPath gp = new GraphicsPath();                              
                                PointF offBStart = ChartMath.AddPoint(previosPoint, args.DepthOffset);
                                PointF offBEnd = ChartMath.AddPoint(currentPoint, args.DepthOffset);
                                gp.AddPolygon(new PointF[] { previosPoint, currentPoint, offBEnd, offBStart });
                                baseRegion.Union(gp);

                                PointF bottomBackLeftn = ChartMath.AddPoint(previousBottomPoint, args.DepthOffset); 
                                PointF bottomBackRightn = ChartMath.AddPoint(currentBottomPoint, args.DepthOffset);

                                #region AreaToolTip 

                                if (areaToolTip)
                                {
                                    //TopRegions

                                    GraphicsPath leftpath = new GraphicsPath();
                                    GraphicsPath rightpath = new GraphicsPath();
                                    tempMidPointOne.X = (previosPoint.X + currentPoint.X) / 2;
                                    tempMidPointOne.Y = (previosPoint.Y + currentPoint.Y) / 2;
                                    tempMidPointTwo.X = (offBEnd.X + offBStart.X) / 2;
                                    tempMidPointTwo.Y = (offBEnd.Y + offBStart.Y) / 2;
                                    leftpath.AddPolygon(new PointF[] { previosPoint, tempMidPointOne, tempMidPointTwo, offBStart });
                                    Region lft = new Region(leftpath);
                                    regions.Add(new ChartRegion(lft, args.SeriesIndex, previousStyledPoint.Index,
                                               previousStyledPoint.ToolTip, c_areaRegionDescription));                        
                                    rightpath.AddPolygon(new PointF[] { tempMidPointOne, currentPoint, offBEnd, tempMidPointTwo });
                                    Region rht = new Region(rightpath);
                                    regions.Add(new ChartRegion(rht, args.SeriesIndex, styledPoint.Index,
                                               styledPoint.ToolTip, c_areaRegionDescription));
                                   
                                }

                                if (areaToolTip)
                                {
                                    //BottomRegions

                                    GraphicsPath leftpath = new GraphicsPath();
                                    GraphicsPath rightpath = new GraphicsPath();
                                    tempMidPointOne.X = (previousBottomPoint.X + currentBottomPoint.X) / 2;
                                    tempMidPointOne.Y = (previousBottomPoint.Y + currentBottomPoint.Y) / 2;
                                    tempMidPointTwo.X = (bottomBackLeftn.X + bottomBackRightn.X) / 2;
                                    tempMidPointTwo.Y = (bottomBackLeftn.Y + bottomBackRightn.Y) / 2;
                                    leftpath.AddPolygon(new PointF[] {previousBottomPoint,bottomBackLeftn,tempMidPointTwo,tempMidPointOne  });
                                    Region lft = new Region(leftpath);
                                    regions.Add(new ChartRegion(lft, args.SeriesIndex, previousStyledPoint.Index,
                                               previousStyledPoint.ToolTip, c_areaRegionDescription));                        
                                    rightpath.AddPolygon(new PointF[] { tempMidPointOne,tempMidPointTwo,bottomBackRightn,currentBottomPoint });
                                    Region rht = new Region(rightpath);
                                    regions.Add(new ChartRegion(rht, args.SeriesIndex, styledPoint.Index,
                                               styledPoint.ToolTip, c_areaRegionDescription));
                                    
                                }
                             #endregion
                            }
                        }
                        #endregion
                        
                        visiblePointsList.Add(currentPoint);
                        previosPoint = currentPoint;
                        previousBottomPoint = currentBottomPoint;
                        lastStyledPoint = i;
                        previousStyledPoint = styledPoint;
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
					#endregion                   

					PointF preFPoint = PointF.Empty;
					PointF preBPoint = PointF.Empty;

					#region Draw the sides before top side
					args.Graph.DrawPath(interior, borderPen, rightToLeft ? rightGp : leftGp);
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


                    #region AreaToolTip                    
                    if (areaToolTip && needRegionUpdate)
                    {
                        Region leftSide = new Region(leftGp);                                    
                        Region rightSide = new Region(rightGp);                                                                        
                        regions.Add(new ChartRegion(leftSide, args.SeriesIndex, styledPoints[firstStyledPoint].Index,
                           styledPoints[firstStyledPoint].ToolTip, c_areaRegionDescription));                         
                        regions.Add(new ChartRegion(rightSide, args.SeriesIndex, styledPoints[lastStyledPoint].Index,
                           styledPoints[lastStyledPoint].ToolTip, c_areaRegionDescription));                   
                                      
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
		/// Renders chart by the specified args.
		/// </summary>
		/// <param name="args">The args.</param>
		public override void Render(ChartRenderArgs3D args)
		{
			bool needRegionUpdate = this.Chart.NeedRegionUpdate;
			bool dropPoints = Chart.DropSeriesPoints;
            
            bool areaToolTip = (m_series.EnableAreaToolTip && Chart.CalcRegions); 

			int yIndex = args.Series.PointFormats[ChartYValueUsage.YValue];
			double origin0 = m_series.ActualYAxis.Origin;

			BrushInfo interior = this.GetBrush();
			Pen borderPen = this.SeriesStyle.GdipPen;

			ChartStyledPoint[] styledPoints = PrepearePoints();
			IndexRange indexedRange = CalculateVisibleRange();
			ArrayList areaPoints = new ArrayList();
			ArrayList topPolygons = new ArrayList();          
             
            ArrayList toolTipTopPolygonRegions = new ArrayList();
            ArrayList toolTipFrontPolygonRegions = new ArrayList();
            ArrayList toolTipBackPolygonRegions = new ArrayList();
            ArrayList toolTipBottomPolygonRegions = new ArrayList();
            Polygon toolTipPolyFront=null ;
            Polygon toolTipPolyBack = null;
            Polygon toolTipPolyBottom=null;                                    
            int firstStyledpointNo = 0;
            int endStyledPointNo = 0;
            ArrayList seriesIndexArray = new ArrayList();           
           
            ChartRegionData crd = needRegionUpdate ?
                new ChartRegionData(args.SeriesIndex, GetToolTip(), c_areaRegionDescription) : null;
                       
			ChartStyledPoint startPoint = null;
			ChartStyledPoint endPoint = null;
            
            PointF bottomCurrent = PointF.Empty;
            PointF bottomPrevious=PointF.Empty;

			args.Graph.AddPolygon(CreateBoundsPolygon((float)args.Z));

			#region Calculate visible poitns and top polygons
			PointF previosPoint = PointF.Empty;
			ChartStyledPoint previos = null;                     

			for (int i = indexedRange.From, ci = indexedRange.To + 1; i < ci; i++)
			{
				ChartStyledPoint styledPoint = styledPoints[i];

				if (styledPoint.IsVisible)
				{
					PointF currentPoint = args.GetPoint(styledPoint.X, styledPoint.YValues[yIndex]);
                    bottomCurrent = this.GetPointFromValue(styledPoint.X,origin0);

                    #region Drops points
					if ((dropPoints && !previosPoint.IsEmpty)
						&& ((!args.IsInvertedAxes && Math.Abs(currentPoint.X - previosPoint.X) < 1f)
							|| (args.IsInvertedAxes && Math.Abs(currentPoint.Y - previosPoint.Y) < 1f)))
					{
						continue;
					}
					#endregion

					#region Get start and end points
					if (startPoint == null)
					{
						startPoint = styledPoint;
                        firstStyledpointNo = i;                   
					}

					endPoint = styledPoint;
                    endStyledPointNo = i;
					#endregion

					#region Create top polygon
					if (previos != null)
					{
                        Vector3D v1 = new Vector3D(previosPoint.X, previosPoint.Y, args.Z);
                        Vector3D v2 = new Vector3D(previosPoint.X, previosPoint.Y, args.Z + args.Depth);
                        Vector3D v3 = new Vector3D(currentPoint.X, currentPoint.Y, args.Z);
                        Vector3D v4 = new Vector3D(currentPoint.X, currentPoint.Y, args.Z + args.Depth);

                        Polygon poly = new Polygon(new Vector3D[] { v1, v2, v4, v3 }, interior, borderPen);
                        
                        if (areaToolTip)
                        {
                            Vector3D toolTipFrontLT = new Vector3D(previosPoint.X, previosPoint.Y, args.Z);
                            Vector3D toolTipFrontLB = new Vector3D(previosPoint.X, bottomPrevious.Y, args.Z);
                            Vector3D toolTipFrontRT = new Vector3D(currentPoint.X, currentPoint.Y, args.Z);
                            Vector3D toolTipFrontRB = new Vector3D(currentPoint.X, bottomCurrent.Y, args.Z); 

                            Vector3D toolTipBackLT = new Vector3D(previosPoint.X, previosPoint.Y, args.Z + args.Depth);
                            Vector3D toolTipBackLB = new Vector3D(previosPoint.X, bottomPrevious.Y, args.Z + args.Depth);
                            Vector3D toolTipBackRT = new Vector3D(currentPoint.X, currentPoint.Y, args.Z + args.Depth);
                            Vector3D toolTipBackRB = new Vector3D(currentPoint.X, bottomCurrent.Y, args.Z + args.Depth);

                            Vector3D toolTipBottomLB = new Vector3D(bottomPrevious.X, bottomPrevious.Y, args.Z);
                            Vector3D toolTipBottomBackLB = new Vector3D(bottomPrevious.X, bottomPrevious.Y, args.Z + args.Depth);
                            Vector3D toolTipBottomRB = new Vector3D(bottomCurrent.X, bottomCurrent.Y, args.Z);
                            Vector3D toolTipBottomBackRB = new Vector3D(bottomCurrent.X, bottomCurrent.Y, args.Z + args.Depth);

                            toolTipPolyFront = new Polygon(new Vector3D[] { toolTipFrontLT, toolTipFrontRT, toolTipFrontRB, toolTipFrontLB });
                            toolTipPolyBack = new Polygon(new Vector3D[] { toolTipBackLT, toolTipBackRT, toolTipBackRB, toolTipBackLB });
                            toolTipPolyBottom = new Polygon(new Vector3D[] { toolTipBottomLB, toolTipBottomBackLB, toolTipBottomBackRB, toolTipBottomRB });                            
                        }

						if (needRegionUpdate)
						{
                            if (!areaToolTip)
                            {
                                poly.RegionData = new ChartRegionData(args.SeriesIndex,
                                    previos.Index, previos.ToolTip, c_areaRegionDescription);
                            }                                                                                 
						}                    
						topPolygons.Add(new PolygonWithTangent(poly, (v1.Y - v3.Y) / (v1.X - v3.X)));

                        if (areaToolTip)
                        {
                            toolTipTopPolygonRegions.Add(new PolygonWithTangent(poly, (v1.Y - v3.Y) / (v1.X - v3.X)));
                            toolTipFrontPolygonRegions.Add(toolTipPolyFront);
                            toolTipBackPolygonRegions.Add(toolTipPolyBack);
                            toolTipBottomPolygonRegions.Add(toolTipPolyBottom);
                        }
					}
					#endregion
					areaPoints.Add(currentPoint);
					previosPoint = currentPoint;
					previos = styledPoint;
                    bottomPrevious = this.GetPointFromValue(styledPoint.X,origin0);
                    seriesIndexArray.Add(styledPoint.Index);
				}
			}

			PointF topLeft = args.GetPoint(startPoint.X, startPoint.YValues[yIndex]);
			PointF topRight = this.GetPointFromValue(endPoint.X, endPoint.YValues[yIndex]);
			PointF bottomLeft = this.GetPointFromValue(startPoint.X, origin0);
			PointF bottomRight = this.GetPointFromValue(endPoint.X, origin0);           

			Vector3D tlfVector = new Vector3D(topLeft.X, topLeft.Y, args.Z);
			Vector3D tldVector = new Vector3D(topLeft.X, topLeft.Y, args.Z + args.Depth);
			Vector3D trfVector = new Vector3D(topRight.X, topRight.Y, args.Z);
			Vector3D trdVector = new Vector3D(topRight.X, topRight.Y, args.Z + args.Depth);
			Vector3D blfVector = new Vector3D(bottomLeft.X, bottomLeft.Y, args.Z);
			Vector3D bldVector = new Vector3D(bottomLeft.X, bottomLeft.Y, args.Z + args.Depth);
			Vector3D brfVector = new Vector3D(bottomRight.X, bottomRight.Y, args.Z);
			Vector3D brdVector = new Vector3D(bottomRight.X, bottomRight.Y, args.Z + args.Depth);

			areaPoints.Add(bottomRight);
			areaPoints.Add(bottomLeft);
			#endregion

			Vector3D[] frontVectors = new Vector3D[areaPoints.Count];           

			Vector3D[] backVectors = new Vector3D[areaPoints.Count];

			Vector3D[] leftVectors = new Vector3D[4] { tlfVector, tldVector, bldVector, blfVector };
			Vector3D[] rightVectors = new Vector3D[4] { trfVector, trdVector, brdVector, brfVector };
			Vector3D[] bottomVectors = new Vector3D[4] { blfVector, bldVector, brdVector, brfVector };

			for (int i = 0; i < areaPoints.Count; i++)                
			{
				PointF ptf = (PointF)areaPoints[i];                            
                 frontVectors[i] = new Vector3D(ptf.X, ptf.Y, args.Z);              
				 backVectors[i] = new Vector3D(ptf.X, ptf.Y, args.Z + args.Depth);
			}                       

			Polygon frontPoly = new Polygon(frontVectors, interior);                     
			Polygon backPoly = new Polygon(backVectors, interior);
			Polygon leftPoly = new Polygon(leftVectors, interior, borderPen);
			Polygon rightPoly = new Polygon(rightVectors, interior, borderPen);
			Polygon bottomPoly = new Polygon(bottomVectors, interior, borderPen);

            if (!areaToolTip )
            {
                frontPoly.RegionData = crd; 
                backPoly.RegionData = crd; 
                leftPoly.RegionData = crd; 
                rightPoly.RegionData = crd; 
                bottomPoly.RegionData = crd; 
            }

            args.Graph.AddPolygon(frontPoly);                                     
			args.Graph.AddPolygon(backPoly);
			args.Graph.AddPolygon(leftPoly);
			args.Graph.AddPolygon(rightPoly);
			args.Graph.AddPolygon(bottomPoly);

			topPolygons.Sort(new PolygonWithTangentComparer());
           
			for (int i = 0; i < topPolygons.Count; i++)
			{
				args.Graph.AddPolygon(((PolygonWithTangent)topPolygons[i]).Polygon);
            }

            #region  toolTip Left/Right PolygonRegions 
            if (areaToolTip && needRegionUpdate)
            {
                ChartRegionData toolTipLeftSideCrd = new ChartRegionData(args.SeriesIndex, styledPoints[firstStyledpointNo].Index, GetToolTip(firstStyledpointNo), c_areaRegionDescription);
                ChartRegionData toolTipRightSideCrd = new ChartRegionData(args.SeriesIndex, styledPoints[endStyledPointNo].Index, GetToolTip(endStyledPointNo), c_areaRegionDescription);
                leftPoly.RegionData = toolTipLeftSideCrd;
                rightPoly.RegionData = toolTipRightSideCrd;
            }
            #endregion

            #region  toolTipTopPolygonRegions 

            if (areaToolTip && needRegionUpdate)
            {
                for (int i = 0;(i < toolTipTopPolygonRegions.Count); i++)                            
                {
                    int Idx = (int)seriesIndexArray[i];
                    int IdxNxt = (int)seriesIndexArray[i + 1];
                    Polygon pg = ((PolygonWithTangent)toolTipTopPolygonRegions[i]).Polygon;
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
                    leftPolygon.RegionData = new ChartRegionData(args.SeriesIndex,
                                  styledPoints[Idx].Index, styledPoints[Idx].ToolTip, c_areaRegionDescription);
                    rightPolygon.RegionData = new ChartRegionData(args.SeriesIndex,
                                   styledPoints[IdxNxt].Index, styledPoints[IdxNxt].ToolTip, c_areaRegionDescription);
                    args.Graph.AddPolygon(leftPolygon);
                    args.Graph.AddPolygon(rightPolygon);
                }
            }
            #endregion

            #region  toolTipFrontPolygonRegions 

            if (areaToolTip && needRegionUpdate)
            {
                for (int i = 0; (i < toolTipFrontPolygonRegions.Count) ; i++)                              
                {
                    int Idx = (int)seriesIndexArray[i];
                    int IdxNxt = (int)seriesIndexArray[i+1];
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
                    leftPolygon.RegionData = new ChartRegionData(args.SeriesIndex,
                                   styledPoints[Idx].Index, styledPoints[Idx].ToolTip, c_areaRegionDescription);
                    Polygon rightPolygon = new Polygon(new Vector3D[] { midPointOne, topRightV, BottomRightV, midPointTwo });
                    rightPolygon.RegionData = new ChartRegionData(args.SeriesIndex,
                                   styledPoints[IdxNxt].Index, styledPoints[IdxNxt].ToolTip, c_areaRegionDescription);
                    args.Graph.AddPolygon(leftPolygon);
                    args.Graph.AddPolygon(rightPolygon);
                }
            }
            #endregion

            #region  toolTipBackPolygonRegions 
                       
            if (areaToolTip && needRegionUpdate)
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
                    leftPolygon.RegionData = new ChartRegionData(args.SeriesIndex,
                                 styledPoints[Idx].Index, styledPoints[Idx].ToolTip, c_areaRegionDescription);
                    Polygon rightPolygon = new Polygon(new Vector3D[] { midPointOne, topRightV, BottomRightV, midPointTwo });
                    rightPolygon.RegionData = new ChartRegionData(args.SeriesIndex,
                                   styledPoints[IdxNxt].Index, styledPoints[IdxNxt].ToolTip, c_areaRegionDescription);
                    args.Graph.AddPolygon(leftPolygon);
                    args.Graph.AddPolygon(rightPolygon);
                }
            }
            #endregion

            #region toolTipBottomPolygonRegions 
            if (areaToolTip && needRegionUpdate)
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
                        leftPolygon.RegionData = new ChartRegionData(args.SeriesIndex,
                                       styledPoints[Idx].Index, styledPoints[Idx].ToolTip, c_areaRegionDescription);                  
                    Polygon rightPolygon = new Polygon(new Vector3D[] { midPointOne, midPointTwo, bottomBackRightV, bottomRightV });                
                        rightPolygon.RegionData = new ChartRegionData(args.SeriesIndex,
                                       styledPoints[IdxNxt].Index, styledPoints[IdxNxt].ToolTip, c_areaRegionDescription);                 
                    args.Graph.AddPolygon(leftPolygon);
                    args.Graph.AddPolygon(rightPolygon);
                }
            }
            #endregion

        }

		/// <summary>
		/// Draws icon.
		/// </summary>
		/// <param name="g">The <see cref="System.Drawing.Graphics"/> to render icon.</param>
		/// <param name="bounds">The bounds of icon.</param>
		/// <param name="isShadow">The value indicates that draw shadow.</param>
		/// <param name="shadowColor">The shadow <see cref="System.Drawing.Color"/>.</param>
		public override void DrawIcon(Graphics g, Rectangle bounds, bool isShadow, Color shadowColor)
		{
			int x1 = bounds.X + bounds.Width / 3;
			int x2 = bounds.X + 2 * bounds.Width / 3;
			GraphicsPath gp = new GraphicsPath();

			gp.AddPolygon(new Point[]{
                                  new Point( bounds.X, bounds.Bottom ),
                                  new Point( x1, bounds.Top ),
                                  new Point( x2, bounds.Top + bounds.Height / 2 ),
                                  new Point( bounds.Right, bounds.Top ),
                                  new Point( bounds.Right, bounds.Bottom )
                                });

			if (isShadow)
			{
				using (SolidBrush br = new SolidBrush(shadowColor))
				{
					g.FillPath(br, gp);
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


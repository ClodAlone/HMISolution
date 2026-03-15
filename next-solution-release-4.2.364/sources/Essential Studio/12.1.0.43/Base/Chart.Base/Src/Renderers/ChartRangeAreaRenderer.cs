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
  internal class RangeAreaRenderer : ChartSeriesRenderer
  {
    #region Properties
    /// <summary>
    /// 
    /// </summary>
    protected override int RequireYValuesCount
    {
      get
      {
        return 2;
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
				return "RangeArea Chart Renderer";
			}
		}
    #endregion

    #region Constructor
    /// <summary>
    /// 
    /// </summary>
    /// <param name="series"></param>
		public RangeAreaRenderer(ChartSeries series)
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
            bool isAdded = false;
            bool allowGapToRangeArea = false;

            ArrayList toolTipSeriesIndxArr = new ArrayList();
            int leftIndex = 0;
            int rightIndex = 0;

			int lowIndex = args.Series.PointFormats[ChartYValueUsage.LowValue];
			int highIndex = args.Series.PointFormats[ChartYValueUsage.HighValue];

			ChartStyleInfo seriesStyle = this.SeriesStyle;
			BrushInfo interior = this.GetBrush();
			Pen borderPen = seriesStyle.GdipPen;

			IList regions = this.ChartArea.ChartRegions;

			ChartStyledPoint[] styledPoints = this.PrepearePoints();
			IndexRange indexedRange = CalculateVisibleRange();
			ArrayList visibleTopPointsList = new ArrayList();
			ArrayList visibleBottomPointsList = new ArrayList();

            ArrayList[] topList = new ArrayList[this.StyledPoints.Length];
            ArrayList[] bottomList = new ArrayList[this.StyledPoints.Length];
            topList[0] = new ArrayList();
            bottomList[0] = new ArrayList(); 

            int emptyCount = 0;

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

                //Create the below code for creating empty space for rangearea chart when using double.NaN for Y-Axis and show the DisplayText in chartarea.

                if (!styledPoint.IsVisible && this.Chart.AllowGapForEmptyPoints)
                {
                    styledPoint.Point.YValues[0] = double.NaN;
                    styledPoint.Point.YValues[1] = double.NaN;
                }
                else if (!styledPoint.IsVisible && !this.Chart.AllowGapForEmptyPoints)
                {
                    continue;
                }
                //if (styledPoint.IsVisible)
                //{
					PointF currentPoint1 = args.GetPoint(styledPoint.X, styledPoint.YValues[lowIndex]);
					PointF currentPoint2 = args.GetPoint(styledPoint.X, styledPoint.YValues[highIndex]);

					if (dropPoints && previosChartPoint != null)
					{
						float delta = args.IsInvertedAxes ? currentPoint1.Y - previosPoint1.Y : currentPoint1.X - previosPoint1.X;

						if (Math.Abs(delta) < 1f)
						{
                            continue;
						}
					}					

                    bool reverse = args.ActualYAxis.Inversed ^ (styledPoint.YValues[lowIndex] > styledPoint.YValues[highIndex]) ^ upToDown;

                    //if (previosChartPoint != null && reverse) //Hide this code. If not, Issue will occur when the YAxis is Inversed and the Min, Max points order is changed in Chart.Points.Add method.
                    //{
                    //    PointF crossPoint = ChartMath.LineSegmentIntersectionPoint(previosPoint1, currentPoint1, previosPoint2, currentPoint2);

                    //    if (!crossPoint.IsEmpty)
                    //    {
                    //        visibleTopPointsList.Add(crossPoint);
                    //        visibleBottomPointsList.Add(crossPoint);
                    //    }
                    //}

					if (reverse)
					{
						PointF swapPoint = currentPoint1;
						currentPoint1 = currentPoint2;
						currentPoint2 = swapPoint;
					}
                #region Draw separate path to RangeArea when using Empty Points
                    if (!double.IsNaN(currentPoint1.Y) && (!double.IsNaN(currentPoint2.Y)))
                    {
                        topList[emptyCount].Add(currentPoint1);
                        bottomList[emptyCount].Add(currentPoint2);
                        isAdded = false;
                    }
                    else if (!isAdded)
                    {
                        if (!allowGapToRangeArea)
                        {
                            allowGapToRangeArea = true;
                        }

                        emptyCount++;
                        topList[emptyCount] = new ArrayList();
                        bottomList[emptyCount] = new ArrayList();                        
                        isAdded = true;                        
                    }
                    #endregion
					visibleTopPointsList.Add(currentPoint2);
					visibleBottomPointsList.Add(currentPoint1);

					previosChartPoint = styledPoint;
					previosPoint1 = currentPoint1;
					previosPoint2 = currentPoint2;

                    toolTipSeriesIndxArr.Add(styledPoint.Index);
                //} //End styledPoint.IsVisible
			}
			#endregion

			if (visibleTopPointsList.Count > 0)
			{

				PointF[] points1 = (PointF[])visibleTopPointsList.ToArray(typeof(PointF));
				PointF[] points2 = (PointF[])visibleBottomPointsList.ToArray(typeof(PointF));

				Array.Reverse(points2);
                #region Generates the front graphics path when using Empty Points and double.NaN
                if (allowGapToRangeArea)
                {

                    for (int l = 0; l <= emptyCount; l++)
                    {
                        if (topList[l].Count != 0)
                        {
                            PointF[] topPoint = (PointF[])topList[l].ToArray(typeof(PointF));
                            PointF[] bottomPoint = (PointF[])bottomList[l].ToArray(typeof(PointF));

                            Array.Reverse(bottomPoint);

                            GraphicsPath GP = new GraphicsPath();
                            GP.AddLines(topPoint);
                            GP.AddLines(bottomPoint);
                            GP.CloseFigure();
                            args.Graph.DrawPath(interior, borderPen, GP);
                        }
                    }
                }
                #endregion
				#region Generates the front graphics path
				GraphicsPath frontGp = new GraphicsPath();

				frontGp.AddLines(points1);
				frontGp.AddLines(points2);

				frontGp.CloseFigure();
				#endregion


                #region AreaToolTip 

                if (areaToolTip && Chart.NeedRegionUpdate)
                {                    
                    int topLeftIdx = 0,topRightIdx=0,btmRightIdx=0,btmLeftIdx=0;

                    PointF ttFrontTopLeft = PointF.Empty;
                    PointF ttFrontTopRight = PointF.Empty;
                    PointF ttFrontBtmLeft = PointF.Empty;
                    PointF ttFrontBtmRight = PointF.Empty;

                    PointF ttBtmLeft = PointF.Empty;
                    PointF ttBtmRight = PointF.Empty;
                    PointF ttBtmbackLeft = PointF.Empty;
                    PointF ttBtmbackRight = PointF.Empty;

                    PointF ttTopLeft = PointF.Empty;
                    PointF ttTopRight = PointF.Empty;
                    PointF ttTopbackLeft = PointF.Empty;
                    PointF ttTopbackRight = PointF.Empty;

                    PointF tempMidPt1 = PointF.Empty;
                    PointF tempMidPt2 = PointF.Empty;

                    int indx = 0;
                    int indxNxt = 0;
                    for (int i = 0; (i < points1.Length - 1) && (i < points2.Length - 1); i++)
                    {           
                        //FrontRegions

                        GraphicsPath leftFrntGp = new GraphicsPath();
                        GraphicsPath rightFrntGp= new GraphicsPath();

                        indx = (int)toolTipSeriesIndxArr[i];
                        indxNxt = (int)toolTipSeriesIndxArr[i+1];

                        if (i == 0)
                            leftIndex = indx;

                        rightIndex = indxNxt;

                        topLeftIdx = i;
                        topRightIdx = i + 1;
                        btmRightIdx = points2.Length - i - 2;
                        btmLeftIdx = points2.Length - i - 1;

                        ttFrontTopLeft = points1[topLeftIdx];
                        ttFrontTopRight = points1[topRightIdx];
                        ttFrontBtmLeft = points2[btmLeftIdx];
                        ttFrontBtmRight = points2[btmRightIdx];

                        tempMidPt1.X = (ttFrontTopLeft.X + ttFrontTopRight.X) / 2;
                        tempMidPt1.Y = (ttFrontTopLeft.Y + ttFrontTopRight.Y) / 2;
                        tempMidPt2.X=(ttFrontBtmLeft.X+ttFrontBtmRight.X)/2;
                        tempMidPt2.Y=(ttFrontBtmLeft.Y+ttFrontBtmRight.Y)/2;                                               

                        leftFrntGp.AddPolygon(new PointF[] { ttFrontTopLeft, tempMidPt1, tempMidPt2, ttFrontBtmLeft });                        
                        Region frntLftReg = new Region(leftFrntGp);
                        regions.Add(new ChartRegion(frntLftReg, args.SeriesIndex, styledPoints[indx].Index, GetToolTip(indx), this.RegionDescription));

                        rightFrntGp.AddPolygon(new PointF[] { tempMidPt1, ttFrontTopRight, ttFrontBtmRight, tempMidPt2 });                        
                        Region frntRhtReg = new Region(rightFrntGp);
                        regions.Add(new ChartRegion(frntRhtReg, args.SeriesIndex, styledPoints[indxNxt].Index, GetToolTip(indxNxt), this.RegionDescription));

                        #region AreatoolTip 3D 

                        if (args.Is3D)
                        {
                            //TopRegions

                            GraphicsPath leftTopGp = new GraphicsPath();
                            GraphicsPath rightTopGp = new GraphicsPath();

                            ttTopLeft = ttFrontTopLeft;
                            ttTopRight = ttFrontTopRight;
                            ttTopbackLeft = ChartMath.AddPoint(ttTopLeft, args.DepthOffset);
                            ttTopbackRight = ChartMath.AddPoint(ttTopRight,args.DepthOffset);

                            tempMidPt1.X = (ttTopLeft.X + ttTopRight.X) / 2;
                            tempMidPt1.Y = (ttTopLeft.Y + ttTopRight.Y) / 2;
                            tempMidPt2.X = (ttTopbackLeft.X + ttTopbackRight.X) / 2;
                            tempMidPt2.Y = (ttTopbackLeft.Y + ttTopbackRight.Y) / 2;

                            leftTopGp.AddPolygon(new PointF[] { ttTopLeft, ttTopbackLeft, tempMidPt2, tempMidPt1 });                           
                            Region topLftReg = new Region(leftTopGp);
                            regions.Add(new ChartRegion(topLftReg, args.SeriesIndex, styledPoints[indx].Index, GetToolTip(indx), this.RegionDescription));

                            rightTopGp.AddPolygon(new PointF[] { tempMidPt1, tempMidPt2, ttTopbackRight, ttTopRight });                          
                            Region topRhtReg = new Region(rightTopGp);
                            regions.Add(new ChartRegion(topRhtReg, args.SeriesIndex, styledPoints[indxNxt].Index, GetToolTip(indxNxt), this.RegionDescription));

                            //BottomRegions

                            GraphicsPath leftBtmGp = new GraphicsPath();
                            GraphicsPath rightBtmGp = new GraphicsPath();

                            ttBtmLeft = ttFrontBtmLeft;
                            ttBtmRight = ttFrontBtmRight;
                            ttBtmbackLeft = ChartMath.AddPoint(ttBtmLeft, args.DepthOffset);
                            ttBtmbackRight = ChartMath.AddPoint(ttBtmRight, args.DepthOffset);

                            tempMidPt1.X = (ttBtmLeft.X + ttBtmRight.X) / 2;
                            tempMidPt1.Y = (ttBtmLeft.Y + ttBtmRight.Y) / 2;
                            tempMidPt2.X = (ttBtmbackLeft.X + ttBtmbackRight.X) / 2;
                            tempMidPt2.Y = (ttBtmbackLeft.Y + ttBtmbackRight.Y) / 2;                           

                            leftBtmGp.AddPolygon(new PointF[] { ttBtmLeft, ttBtmbackLeft, tempMidPt2, tempMidPt1 });                          
                            Region btmLftReg = new Region(leftBtmGp);
                            regions.Add(new ChartRegion(btmLftReg, args.SeriesIndex, styledPoints[indx].Index, GetToolTip(indx), this.RegionDescription));

                            rightBtmGp.AddPolygon(new PointF[] { tempMidPt1, tempMidPt2, ttBtmbackRight, ttBtmRight });                        
                            Region btmRhtReg = new Region(rightBtmGp);
                            regions.Add(new ChartRegion(btmRhtReg, args.SeriesIndex, styledPoints[indxNxt].Index, GetToolTip(indxNxt), this.RegionDescription));
                        }

                        #endregion
                    }
                }

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

					args.Graph.DrawPath(interior, borderPen, rightToLeft ? rightGp : leftGp);

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
								orde12 = currFPoint2.X > preFPoint2.X;
								orde12 &= Math.Abs(args.DepthOffset.Width / args.DepthOffset.Height) < Math.Abs((currFPoint2.X - preFPoint2.X) / (currFPoint2.Y - preFPoint2.Y));
							}
							else
							{
								orde12 = currFPoint2.Y < preFPoint2.Y;
								orde12 &= Math.Abs(args.DepthOffset.Width / args.DepthOffset.Height) > Math.Abs((currFPoint2.X - preFPoint2.X) / (currFPoint2.Y - preFPoint2.Y));
							}

							if (orde12)
							{
								args.Graph.DrawPath(interior, borderPen, segment1);
								args.Graph.DrawPath(interior, borderPen, segment2);
							}
							else
							{
								args.Graph.DrawPath(interior, borderPen, segment2);
								args.Graph.DrawPath(interior, borderPen, segment1);
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

                    #region AreaToolTip left/right

                    if (areaToolTip && (baseRegion != null))
                    {
                        regions.Add(new ChartRegion(new Region(leftGp), args.SeriesIndex, leftIndex, GetToolTip(leftIndex), this.RegionDescription));
                        regions.Add(new ChartRegion(new Region(rightGp), args.SeriesIndex, rightIndex, GetToolTip(rightIndex), this.RegionDescription));
                    }

                   #endregion

					#endregion
				}
				else
				{
					#region Draw shadow
                    if (seriesStyle.DisplayShadow && !allowGapToRangeArea)
                    {
						GraphicsPath shadowGP = (GraphicsPath)frontGp.Clone();
						shadowGP.Transform(new Matrix(1.0f, 0.0f, 0.0f, 1.0f,
							seriesStyle.ShadowOffset.Width, seriesStyle.ShadowOffset.Height));
						args.Graph.DrawPath(interior, null, shadowGP);
					}
					#endregion
				}

                if (!allowGapToRangeArea)
                {
                    args.Graph.DrawPath(interior, borderPen, frontGp);
                }

                if (baseRegion != null && !allowGapToRangeArea)
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
      bool isInvertedAxes = IsInvertedAxes;
      bool needRegionUpdate = this.Chart.NeedRegionUpdate;
      bool dropPoints = Chart.DropSeriesPoints;
      bool areaToolTip = (m_series.EnableAreaToolTip && Chart.CalcRegions);

     

      int seriesIndex = this.Chart.Series.IndexOf(m_series);
			int lowIndex = m_series.PointFormats[ChartYValueUsage.LowValue];
			int highIndex = m_series.PointFormats[ChartYValueUsage.HighValue];

			float fd = GetPlaceDepth();
      float bd = fd + GetSeriesDepth();

      BrushInfo interior = this.GetBrush();
      Pen borderPen = this.SeriesStyle.GdipPen;

      ChartStyledPoint[] styledPoints = PrepearePoints();
      IndexRange indexedRange = CalculateVisibleRange();
      ArrayList areaPoints1 = new ArrayList();
			ArrayList areaPoints2 = new ArrayList();
			ArrayList topPolygons = new ArrayList();


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
     

      ChartRegionData crd = needRegionUpdate ? 
        new ChartRegionData( seriesIndex, GetToolTip(), this.RegionDescription ) : null;

      g.AddPolygon(CreateBoundsPolygon(fd));

      #region Calculate visible poitns and top polygons
      PointF previosPoint1 = PointF.Empty;
			PointF previosPoint2 = PointF.Empty;

      for (int i = indexedRange.From, ci = indexedRange.To + 1; i < ci; i++)
      {
        ChartStyledPoint styledPoint = styledPoints[i];

        if (!styledPoint.Point.IsEmpty)
        {
          PointF currentPoint1 = this.GetPointFromValue(styledPoint.Point, lowIndex);
					PointF currentPoint2 = this.GetPointFromValue(styledPoint.Point, highIndex);

          #region Drops points
          if ((dropPoints && !previosPoint1.IsEmpty)
            && ((!isInvertedAxes && Math.Abs(currentPoint1.X - previosPoint1.X) < 1f)
              || (isInvertedAxes && Math.Abs(currentPoint1.Y - previosPoint1.Y) < 1f)))
          {
            continue;
          }
        	#endregion

          areaPoints1.Add(currentPoint1);
					areaPoints2.Add(currentPoint2);

          previosPoint1 = currentPoint1;
					previosPoint2 = currentPoint2;

          seriesIndexArray.Add(styledPoint.Index);

        }
        
      }

			PointF[] points1 = areaPoints1.ToArray(typeof(PointF)) as PointF[];
			PointF[] points2 = areaPoints2.ToArray(typeof(PointF)) as PointF[];

			Array.Reverse(points2);

			PointF topLeft = points1[0];
			PointF topRight = points1[points1.Length - 1];
			PointF bottomLeft = points2[points2.Length - 1];
			PointF bottomRight = points2[0];

      Vector3D tlfVector = new Vector3D(topLeft.X, topLeft.Y, fd);
      Vector3D tldVector = new Vector3D(topLeft.X, topLeft.Y, bd);
      Vector3D trfVector = new Vector3D(topRight.X, topRight.Y, fd);
      Vector3D trdVector = new Vector3D(topRight.X, topRight.Y, bd);
      Vector3D blfVector = new Vector3D(bottomLeft.X, bottomLeft.Y, fd);
      Vector3D bldVector = new Vector3D(bottomLeft.X, bottomLeft.Y, bd);
      Vector3D brfVector = new Vector3D(bottomRight.X, bottomRight.Y, fd);
      Vector3D brdVector = new Vector3D(bottomRight.X, bottomRight.Y, bd);
      #endregion

      Vector3D[] frontVectors = new Vector3D[areaPoints1.Count + areaPoints2.Count];
			Vector3D[] backVectors = new Vector3D[areaPoints1.Count + areaPoints2.Count];

      Vector3D[] leftVectors = new Vector3D[4] { tlfVector, tldVector, bldVector, blfVector };
      Vector3D[] rightVectors = new Vector3D[4] { trfVector, trdVector, brdVector, brfVector };

			int pi = 0;

			for (; pi < points1.Length; pi++)
      {
				frontVectors[pi] = new Vector3D(points1[pi].X, points1[pi].Y, fd);
				backVectors[pi] = new Vector3D(points1[pi].X, points1[pi].Y, bd);
      }

			for (int i = 0; i < points2.Length; i++)
			{
				frontVectors[pi + i] = new Vector3D(points2[i].X, points2[i].Y, fd);
				backVectors[pi + i] = new Vector3D(points2[i].X, points2[i].Y, bd);
			}

      Polygon frontPoly = new Polygon(frontVectors, interior);
      Polygon backPoly = new Polygon(backVectors, interior);      
      Polygon leftPoly = new Polygon(leftVectors, interior, borderPen);
      Polygon rightPoly = new Polygon(rightVectors, interior, borderPen);

      if (!areaToolTip)
      {
          frontPoly.RegionData = crd;
          backPoly.RegionData = crd;
          leftPoly.RegionData = crd;
          rightPoly.RegionData = crd;
      }

      g.AddPolygon(frontPoly);
      g.AddPolygon(backPoly);
      g.AddPolygon(leftPoly);
      g.AddPolygon(rightPoly);

			ArrayList polys = new ArrayList();

			for (int i = 1, cl = frontVectors.Length, ci = cl / 2; i < ci; i++)
			{
			  Vector3D v11 = frontVectors[i - 1];
			  Vector3D v12 = backVectors[i - 1];
			  Vector3D v13 = frontVectors[i];
			  Vector3D v14 = backVectors[i];

			  Vector3D v21 = frontVectors[cl - i - 1];
			  Vector3D v22 = backVectors[cl - i - 1];
			  Vector3D v23 = frontVectors[cl - i - 0];
			  Vector3D v24 = backVectors[cl - i - 0];

			  Polygon poly1 = new Polygon(new Vector3D[] { v11, v12, v14, v13 }, interior, borderPen);
              Polygon poly2 = new Polygon(new Vector3D[] { v21, v22, v24, v23 }, interior, borderPen);              

                if (areaToolTip)
                {
                    toolTipPolyTop = new Polygon(new Vector3D[] { v23, v24, v22, v21 });
                    toolTipPolyFront = new Polygon(new Vector3D[] { v23, v21, v13, v11 });
                    toolTipPolyBack = new Polygon(new Vector3D[] { v24, v12, v14, v22 });
                    toolTipPolyBottom = new Polygon(new Vector3D[] { v11, v12, v14, v13 });

                    toolTipTopPolygonRegions.Add(toolTipPolyTop);
                    toolTipFrontPolygonRegions.Add(toolTipPolyFront);
                    toolTipBackPolygonRegions.Add(toolTipPolyBack);
                    toolTipBottomPolygonRegions.Add(toolTipPolyBottom);
                }
                 

			  if (needRegionUpdate)
			  {
					ChartStyledPoint styledPoint = styledPoints[i - 1];

                    if (!areaToolTip)
                    {
                        poly1.RegionData = poly2.RegionData = new ChartRegionData(seriesIndex,
                            styledPoint.Index, styledPoint.ToolTip, this.RegionDescription);
                    }                   
			  }
				polys.Add(poly1);
				polys.Add(poly2);              
				g.AddPolygon(new Polygon(new Vector3D[] { v13, v14, v22, v21 }, interior));                
			}

			foreach(Polygon poly in polys)
			{
				g.AddPolygon(poly);
			}


            #region  toolTip Left/Right PolygonRegions 
            if (areaToolTip && needRegionUpdate)
            {
                int i = seriesIndexArray.Count-1;
                firstStyledpointNo = (int)seriesIndexArray[0];
                endStyledPointNo = (int)seriesIndexArray[i];
                ChartRegionData toolTipLeftSideCrd = new ChartRegionData(seriesIndex, styledPoints[firstStyledpointNo].Index, styledPoints[firstStyledpointNo].ToolTip, this.RegionDescription);
                ChartRegionData toolTipRightSideCrd = new ChartRegionData(seriesIndex, styledPoints[endStyledPointNo].Index, styledPoints[endStyledPointNo].ToolTip, this.RegionDescription);
                leftPoly.RegionData = toolTipLeftSideCrd;
                rightPoly.RegionData = toolTipRightSideCrd;
                g.AddPolygon(leftPoly);
                g.AddPolygon(rightPoly);
            }


            #endregion

            #region  toolTipTopPolygonRegions

            if (areaToolTip && needRegionUpdate)
            {
                for (int i = 0;i < toolTipTopPolygonRegions.Count ;i++)                            
                {
                    int Idx = (int)seriesIndexArray[i];
                    int IdxNxt = (int)seriesIndexArray[i + 1];                   
                    Polygon pg =(Polygon) toolTipTopPolygonRegions[i];
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
                    leftPolygon.RegionData = new ChartRegionData(seriesIndex,
                                  styledPoints[Idx].Index, styledPoints[Idx].ToolTip, this.RegionDescription);
                    rightPolygon.RegionData = new ChartRegionData(seriesIndex,
                                   styledPoints[IdxNxt].Index, styledPoints[IdxNxt].ToolTip, this.RegionDescription);
                    g.AddPolygon(leftPolygon);
                    g.AddPolygon(rightPolygon);
                }
            }
            #endregion

            #region  toolTipFrontPolygonRegions 

            if (areaToolTip && needRegionUpdate)
            {
                for (int i = 0;i < toolTipFrontPolygonRegions.Count; i++)                           
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
                    leftPolygon.RegionData = new ChartRegionData(seriesIndex,
                                 styledPoints[Idx].Index, styledPoints[Idx].ToolTip, this.RegionDescription);
                    rightPolygon.RegionData = new ChartRegionData(seriesIndex,
                                   styledPoints[IdxNxt].Index, styledPoints[IdxNxt].ToolTip, this.RegionDescription);
                    g.AddPolygon(leftPolygon);
                    g.AddPolygon(rightPolygon);
                }
            }
            #endregion

            #region  toolTipBackPolygonRegions 

            if (areaToolTip && needRegionUpdate)
            {
                for (int i = 0;i < toolTipBackPolygonRegions.Count;i++)                          
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
                    leftPolygon.RegionData = new ChartRegionData(seriesIndex,
                                 styledPoints[Idx].Index, styledPoints[Idx].ToolTip, this.RegionDescription);
                    rightPolygon.RegionData = new ChartRegionData(seriesIndex,
                                   styledPoints[IdxNxt].Index, styledPoints[IdxNxt].ToolTip, this.RegionDescription);
                    g.AddPolygon(leftPolygon);
                    g.AddPolygon(rightPolygon);
                }
            }
            #endregion

            #region toolTipBottomPolygonRegions
            if (areaToolTip && needRegionUpdate)
            {
                for (int i = 0, j = 0; (i < toolTipBottomPolygonRegions.Count) && (j < seriesIndexArray.Count); i++, j++)                            
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
                    leftPolygon.RegionData = new ChartRegionData(seriesIndex,
                                 styledPoints[Idx].Index, styledPoints[Idx].ToolTip, this.RegionDescription);
                    rightPolygon.RegionData = new ChartRegionData(seriesIndex,
                                   styledPoints[IdxNxt].Index, styledPoints[IdxNxt].ToolTip, this.RegionDescription);            
                    g.AddPolygon(leftPolygon);
                    g.AddPolygon(rightPolygon);
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
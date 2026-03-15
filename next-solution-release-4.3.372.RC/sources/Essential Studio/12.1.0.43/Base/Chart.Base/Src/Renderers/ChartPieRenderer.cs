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
using System.Collections.Generic;

namespace Syncfusion.Windows.Forms.Chart.Renderers
{
	/// <summary>
	/// 
	/// </summary>
	internal class PieRenderer : ChartSeriesRenderer
	{
		#region Constants
		private const double D_TO_R = Math.PI / 180d;
		private const double R_TO_D = 180d / Math.PI;
		private const float MIN_RADIUS = 50;
		private const float MAX_TEXT_WIDTH = 300;
		private const float MIN_FOV_SECTOR = 8;
		private const float MARGINS_RATIO = 0.03f;
		private const float SPACE_RATIO = 0.02f;
		private const float TICK_RATIO = 0.05f;
		private const float PERCENT_COEF = 0.01f;
        private const float INSIDE_LABELS_COEF = 0.8f;        

		private const string c_pieRegionDescription = "Pie Chart Region";

		private const int c_drawSides180Order = 1;
		private const int c_drawSides360Order = 0;
		private const int c_drawInnerOrder = 2;
		private const int c_drawOuterOrder = 3;
		private const int c_drawTopOrder = 4;
		private const int c_drawOrdersCount = 5;
		private readonly static ColorBlend c_cylinderPhong;
		private const float c_orderEpsilon = 0.0001f;
		#endregion

		#region Members
		private float m_startAngle = 0f;

		private readonly static ColorBlend s_insideGradient;
		private readonly static ColorBlend s_outsideGradient;
		private readonly static ColorBlend s_roundGradient;
		private readonly static ColorBlend s_bevelGradient;
		#endregion

		#region Properties

        /// <summary>
        /// Indicates how much space this type will use.
        /// </summary>
        /// <value></value>
		public override ChartUsedSpaceType FillSpaceType
		{
			get
			{
                if (ChartArea.MultiplePies)
                {
                    return ChartUsedSpaceType.OneForOne;
                }

                return ChartUsedSpaceType.All;
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
		#endregion

		#region Helper classes
		/// <summary>
		/// 
		/// </summary>
		public enum PieSectorCorner
		{
			TopLeft,
			TopRight,
			BottomLeft,
			BottomRight
		}
		/// <summary>
		/// 
		/// </summary>
		private class PieSector
		{
			#region Members
			private float m_depth = 0;
			private RectangleF m_inSideRect;
			private RectangleF m_outSideRect;
			private RectangleF m_inUpSideRect;
			private RectangleF m_outUpSideRect;
			private float m_startAngle;
			private float m_endAngle;
			private ChartStyledPoint m_point;

			private GraphicsPath m_geometry = null;            

			private readonly static DoubleRange c_1PISector = new DoubleRange(0, 180);
			private readonly static DoubleRange c_2PISector = new DoubleRange(180, 360);
			#endregion

			#region Properties
			/// <summary>
			/// 
			/// </summary>
			public ChartStyledPoint StyledPoint
			{
				get
				{
					return m_point;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			public float StartAngle
			{
				get
				{
					return m_startAngle;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			public float EndAngle
			{
				get
				{
					return m_endAngle;
				}
			}
			/// <summary>
			/// Gets the inner bounds.
			/// </summary>
			/// <value>The inner bounds.</value>
			public RectangleF InnerBounds
			{
				get
				{
					return m_inSideRect;
				}
			}
			/// <summary>
			/// Gets the outer bounds.
			/// </summary>
			/// <value>The outer bounds.</value>
			public RectangleF OuterBounds
			{
				get
				{
					return m_outSideRect;
				}
			}
			#endregion

			#region Constructor
			/// <summary>
			/// Initializes a new instance of the <see cref="PieSector"/> class.
			/// </summary>
			/// <param name="point">The point.</param>
			/// <param name="outSideRect">The out side rect.</param>
			/// <param name="inSideRect">The inner bounds.</param>
			/// <param name="startAngle">The start angle.</param>
			/// <param name="endAngle">The end angle.</param>
			/// <param name="depth">The depth.</param>
			public PieSector(ChartStyledPoint point, RectangleF outSideRect, RectangleF inSideRect,
				float startAngle, float endAngle, float depth)
			{
				m_point = point;
				m_depth = depth;

				m_inSideRect = inSideRect;
				m_outSideRect = outSideRect;

				m_inUpSideRect = inSideRect;
				m_outUpSideRect = outSideRect;

				m_startAngle = startAngle;
				m_endAngle = endAngle;

				m_inSideRect.Offset(0, depth);
				m_outSideRect.Offset(0, depth);
			}
			#endregion

			#region Public methods
			/// <summary>
			/// Renders this instance.
			/// </summary>
			/// <returns></returns>
			public IEnumerable<Pie3DSegment> Render()
			{
				IList<Pie3DSegment> result = null;

				if (((int)m_outSideRect.Width > 0) && ((int)m_outSideRect.Height > 0))
				{
					if (m_depth != 0)
					{
						result = new List<Pie3DSegment>();

						float startAngle = m_startAngle % 360;
						float endAngle = m_endAngle % 360;
						float sweepAngle = m_endAngle - m_startAngle;

						if (sweepAngle == 360)
						{
							result.Add(this.CreateSegment(0, 180, false, false));
							result.Add(this.CreateSegment(180, 360, false, false));
						}
						else if ((sweepAngle <= 180)
							&& ((startAngle < 180 && endAngle <= 180) || (startAngle >= 180 && endAngle >= 180)))
						{
							result.Add(this.CreateSegment(startAngle, endAngle, true, true));
						}
						else if (startAngle > endAngle)
						{
							if (startAngle < 180)
							{
								result.Add(this.CreateSegment(startAngle, 180, true, false));

								if (endAngle < 180)
								{
									result.Add(this.CreateSegment(180, 360, true, false));
								}
							}
							else
							{
								result.Add(this.CreateSegment(startAngle, 360, true, false));
							}

							if (endAngle < 180)
							{
								result.Add(this.CreateSegment(0, endAngle, false, true));
							}
							else
							{
								result.Add(this.CreateSegment(0, 180, false, false));
								result.Add(this.CreateSegment(180, endAngle, false, true));
							}

						}
						else if (startAngle < 180 && endAngle > 180)
						{
							result.Add(this.CreateSegment(startAngle, 180, true, false));
							result.Add(this.CreateSegment(180, endAngle, false, true));
						}
					}

					#region Upside GraphicsPath
					m_geometry = new GraphicsPath();

					if (m_inSideRect.IsEmpty)
					{
						AddClosedSector(m_geometry, Rectangle.Round(m_outUpSideRect), m_startAngle, m_endAngle - m_startAngle);
					}
					else
					{                        
                        AddSector(m_geometry, m_outUpSideRect, m_startAngle, m_endAngle - m_startAngle);
                        AddSector(m_geometry, m_inUpSideRect, m_endAngle, m_startAngle - m_endAngle);
                        m_geometry.CloseFigure();                     
					}
					#endregion
				}

				return result;
			}
			/// <summary>
			/// Draws sector.
			/// </summary>
			/// <param name="graph">The graph.</param>
			/// <param name="brInfo">The br info.</param>
			/// <param name="pen">The pen.</param>
			/// <param name="type">The type.</param>
			/// <param name="gradient">The gradient.</param>
			public void Draw(ChartGraph graph, BrushInfo brInfo, Pen pen,
				ChartPieFillMode type, ColorBlend gradient)
			{
				if (m_geometry != null)
				{
					graph.DrawPath(brInfo, null, m_geometry);
				
					if (gradient != null)
					{
					float m_geometryHeight = m_geometry.GetBounds().Height;
					
					if (m_geometryHeight != 0)
					{
						using (PathGradientBrush pgb = new PathGradientBrush(m_geometry))
						{
							pgb.InterpolationColors = gradient;

							if (type == ChartPieFillMode.AllPie)
							{
								pgb.CenterPoint = new PointF(m_outUpSideRect.X + m_outUpSideRect.Width / 2,
									m_outUpSideRect.Y + m_outUpSideRect.Height / 2);
							}

							graph.DrawPath(pgb, null, m_geometry);
						}
					}
					}

					graph.DrawPath(pen, m_geometry);
				}
			}
			/// <summary>
			/// Gets the lower region.
			/// </summary>
			/// <param name="regionData">The region data.</param>
			/// <returns></returns>
			public ChartRegion GetRegion(ChartRegionData regionData)
			{
				Region rgn = new Region(Rectangle.Empty);

				if (m_geometry != null)
				{
					rgn.Union(m_geometry);
				}

				return new ChartRegion(rgn, regionData);
			}

			/// <summary>
			/// Creates the segment.
			/// </summary>
			/// <param name="startAngle">The start angle.</param>
			/// <param name="endAngle">The end angle.</param>
			/// <param name="left">if set to <c>true</c> left side will be created.</param>
			/// <param name="right">if set to <c>true</c> right side will be created.</param>
			/// <returns></returns>
			private Pie3DSegment CreateSegment(float startAngle, float endAngle, bool left, bool right)
			{
				Pie3DSegment result = new Pie3DSegment();

				result.StartAngle = startAngle;
				result.EndAngle = endAngle;
				result.Sector = this;

				if (left)
				{
					PointF sOutPt = ChartMath.GetPointByAngle(m_outSideRect, startAngle * ChartMath.ToRadians, true);
					PointF sInPt = ChartMath.GetPointByAngle(m_inSideRect, startAngle * ChartMath.ToRadians, true);

					result.LeftSide = new GraphicsPath();
					result.LeftSide.AddPolygon(new PointF[]{ sInPt, new PointF( sInPt.X, sInPt.Y - m_depth ),
					new PointF( sOutPt.X, sOutPt.Y - m_depth ), sOutPt });
				}

				if (right)
				{
					PointF eOutPt = ChartMath.GetPointByAngle(m_outSideRect, endAngle * ChartMath.ToRadians, true);
					PointF eInPt = ChartMath.GetPointByAngle(m_inSideRect, endAngle * ChartMath.ToRadians, true);

					result.RightSide = new GraphicsPath();
					result.RightSide.AddPolygon(new PointF[]{ eInPt, new PointF( eInPt.X, eInPt.Y - m_depth ),
					new PointF( eOutPt.X, eOutPt.Y - m_depth ), eOutPt });
				}

				if (!m_inSideRect.IsEmpty)
				{                    
                    result.InnerSide = new GraphicsPath();                    
                    this.AddSector(result.InnerSide, m_inSideRect, startAngle, (endAngle - startAngle));
                    this.AddSector(result.InnerSide, m_inUpSideRect, endAngle, (startAngle - endAngle));
                    result.InnerSide.CloseFigure();                    
				}

				result.OuterSide = new GraphicsPath();
				this.AddSector(result.OuterSide, m_outSideRect, startAngle, (endAngle - startAngle));
				this.AddSector(result.OuterSide, m_outUpSideRect, endAngle, (startAngle - endAngle));
				result.OuterSide.CloseFigure();

				return result;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="gp"></param>
			/// <param name="rect"></param>
			/// <param name="start"></param>
			/// <param name="angle"></param>
			private void AddSector(GraphicsPath gp, RectangleF rect, float start, float angle)
			{
				GraphicsPath arc = new GraphicsPath();
				PointF[] mtrPts = new PointF[]{ rect.Location,
          new PointF(rect.Right, rect.Top), new PointF(rect.Left, rect.Bottom)};

				if (angle < 0)
				{
					angle = -Math.Max(-angle, 0.0001f);
				}
				else
				{
					angle = Math.Max(angle, 0.0001f);
				}

				arc.AddArc(0, 0, rect.Width, rect.Width, start, angle);
				arc.Transform(new Matrix(new RectangleF(0, 0, rect.Width, rect.Width), mtrPts));

				gp.AddPath(arc, true);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="gp"></param>
			/// <param name="rect"></param>
			/// <param name="start"></param>
			/// <param name="angle"></param>
			private void AddClosedSector(GraphicsPath gp, RectangleF rect, float start, float angle)
			{
				GraphicsPath pie = new GraphicsPath();
				PointF[] mtrPts = new PointF[]{ rect.Location,
          new PointF(rect.Right, rect.Top), new PointF(rect.Left, rect.Bottom)};

				if (angle < 0)
				{
					angle = -Math.Max(-angle, 0.0001f);
				}
				else
				{
					angle = Math.Max(angle, 0.0001f);
				}

				if (angle == 360)
				{
					pie.AddArc(0, 0, rect.Width, rect.Width, start, angle);
				}
				else
				{
					pie.AddPie(0, 0, rect.Width, rect.Width, start, angle);
				}

				pie.Transform(new Matrix(new RectangleF(0, 0, rect.Width, rect.Width), mtrPts));

				gp.AddPath(pie, true);
			}
			#endregion
		}
		/// <summary>
		/// 
		/// </summary>
		private class PieSectorComparer : IComparer
		{
			#region IComparer Members
			/// <summary>
			/// 
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public int Compare(object x, object y)
			{
				int res = 0;
				PieSector ps1 = x as PieSector;
				PieSector ps2 = y as PieSector;

				if (ps1 != ps2 && ps1 != null && ps2 != null)
				{
					float ps1StartAngle = ps1.StartAngle % 360;
					float ps2StartAngle = ps2.StartAngle % 360;

					if (ps1StartAngle > 90 && ps1StartAngle < 270)
					{
						if (ps2StartAngle > 90 && ps2StartAngle < 270)
						{
							res = ps1StartAngle > ps2StartAngle ? -1 : 1;
						}
						else
						{
							res = -1;
						}
					}
					else
					{
						if ((ps2StartAngle > 90 && ps2StartAngle < 270)
							|| (ps1StartAngle < 90) && (ps2StartAngle > 270))
						{
							res = 1;
						}
						else if ((ps1StartAngle > 270) && (ps2StartAngle < 90))
						{
							res = -1;
						}
						else
						{
							res = ps1StartAngle < ps2StartAngle ? -1 : 1;
						}
					}
				}

				return res;
			}
			#endregion
		}
		/// <summary>
		/// 
		/// </summary>
		private class PieLabel
		{
			#region Members
			private ChartStyledPoint m_styledPoint = null;
			private RectangleF m_rect;
			private PointF m_connectPoint;
			private PointF m_notCorrectPoint;
			private double m_value;
			private float m_angle;
			private PieSectorCorner m_corner;
            private ChartSeries m_series;
            private int m_labelIndex;
			#endregion

			#region Properties
			/// <summary>
			/// Gets the styled point.
			/// </summary>
			/// <value>The styled point.</value>
			public ChartStyledPoint StyledPoint
			{
				get
				{
					return m_styledPoint;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			public RectangleF Rectangle
			{
				get
				{
					return m_rect;
				}
				set
				{
					m_rect = value;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			public PointF ConnectPoint
			{
				get
				{
					return m_connectPoint;
				}
				set
				{
					m_connectPoint = value;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			public PointF NotCorrectPoint
			{
				get
				{
					return m_notCorrectPoint;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			public double Value
			{
				get
				{
					return m_value;
				}
				set
				{
					m_value = value;
				}
			}
			/// <summary>
			/// 
			/// </summary>
            public ChartSeries Series
            {
                get
                {
                    return m_series;
                }
                set
                {
                    m_series = value;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public int LabelIndex
            {
                get
                {
                    return m_labelIndex;
                }
                set
                {
                    m_labelIndex = value;
                }
            }
			/// <summary>
			/// 
			/// </summary>
			public float Angle
			{
				get
				{
					return m_angle;
				}
				set
				{
					m_angle = value;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			public PieSectorCorner Corner
			{
				get
				{
					return m_corner;
				}
				set
				{
					m_corner = value;
				}
			}
			#endregion

			#region Constructor
			/// <summary>
			/// 
			/// </summary>
			public PieLabel(ChartStyledPoint styledPoint, float angle)
			{
				m_styledPoint = styledPoint;
				m_rect = RectangleF.Empty;
				m_angle = angle;
                m_series = null;
                m_labelIndex = 0;
			}

            /// <summary>
            /// 
            /// </summary>
            public PieLabel(ChartStyledPoint styledPoint, float angle,ChartSeries series,int LblIndex)
            {
                m_styledPoint = styledPoint;
                m_rect = RectangleF.Empty;
                m_angle = angle;
                m_series = series;
                m_labelIndex = LblIndex;
			}
			#endregion

			#region Public methods
			/// <summary>
			/// Measures the specified g.
			/// </summary>
			/// <param name="g">The g.</param>
			/// <param name="maxWidth">Width of the max.</param>
			/// <returns></returns>
			public SizeF Measure(ChartGraph g, float maxWidth)
			{
				if (m_styledPoint.Style.DisplayText)
				{
                    if (this.Series.ConfigItems.PieItem.ShowDataBindLabels && this.Series.XAxis.LabelsImpl != null)
                    {
                        String str = m_series.XAxis.LabelsImpl.GetLabelAt(this.StyledPoint.Index).Text;
                        Font fnt = m_series.XAxis.LabelsImpl.GetLabelAt(this.StyledPoint.Index).Font;
                        return m_rect.Size = g.MeasureString(str, fnt, maxWidth);
                    }
                    else
                    {
					return m_rect.Size = g.MeasureString(m_styledPoint.Style.Text,
						m_styledPoint.Style.GdipFont, maxWidth);
				}
				}

				return m_rect.Size = SizeF.Empty;
			}
			/// <summary>
			/// Sets the connect point.
			/// </summary>
			/// <param name="pt">The point to connect.</param>
			public void SetConnectPoint(PointF pt)
			{
				m_connectPoint = pt;
				m_notCorrectPoint = pt;
				m_rect.Location = pt;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="value"></param>
			public void CorrectTopLeft(float value)
			{
				m_rect.Location = new PointF(m_connectPoint.X - m_rect.Width,
					m_connectPoint.Y - m_rect.Height / 2);
                
                if (m_series.ConfigItems.PieItem.LabelStyle != ChartAccumulationLabelStyle.OutsideInArea)
                {
                    float offset = m_rect.Bottom - value;

                    if (offset > 0)
                    {
                        m_rect.Location = new PointF(m_rect.X, m_rect.Y - offset);
                        m_connectPoint = new PointF(m_connectPoint.X, m_connectPoint.Y - offset);
                    }
                }
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="value"></param>
			public void CorrectTopRight(float value)
			{
				m_rect.Location = new PointF(m_connectPoint.X,
					m_connectPoint.Y - m_rect.Height / 2);
                if (m_series.ConfigItems.PieItem.LabelStyle != ChartAccumulationLabelStyle.OutsideInArea)
                {
                    float offset = m_rect.Bottom - value;

                    if (offset > 0)
                    {
                        m_rect.Location = new PointF(m_rect.X, m_rect.Y - offset);
                        m_connectPoint = new PointF(m_connectPoint.X, m_connectPoint.Y - offset);
                    }
                }
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="value"></param>
			public void CorrectBottomLeft(float value)
			{
				m_rect.Location = new PointF(m_connectPoint.X - m_rect.Width,
					m_connectPoint.Y - m_rect.Height / 2);
                if (m_series.ConfigItems.PieItem.LabelStyle != ChartAccumulationLabelStyle.OutsideInArea)
                {
                    float offset = m_rect.Top - value;

                    if (offset < 0)
                    {
                        m_rect.Location = new PointF(m_rect.X, m_rect.Y - offset);
                        m_connectPoint = new PointF(m_connectPoint.X, m_connectPoint.Y - offset);
                    }
                }
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="value"></param>
			public void CorrectBottomRight(float value)
			{
				m_rect.Location = new PointF(m_connectPoint.X,
					m_connectPoint.Y - m_rect.Height / 2);
                if (m_series.ConfigItems.PieItem.LabelStyle != ChartAccumulationLabelStyle.OutsideInArea)
                {
                    float offset = m_rect.Top - value;

                    if (offset < 0)
                    {
                        m_rect.Location = new PointF(m_rect.X, m_rect.Y - offset);
                        m_connectPoint = new PointF(m_connectPoint.X, m_connectPoint.Y - offset);
                    }
                }
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="value"></param>
			public void AlignRightSide(float value)
			{
				float offset = m_rect.Right - value;

				m_rect.Location = new PointF(m_rect.X - offset, m_rect.Y);
				m_connectPoint = new PointF(m_connectPoint.X - offset, m_connectPoint.Y);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="value"></param>
			public void AlignLeftSide(float value)
			{
				m_rect.Location = new PointF(value, m_rect.Y);
				m_connectPoint = new PointF(value, m_connectPoint.Y);
			}
			#endregion
		}
		/// <summary>
		/// 
		/// </summary>
		private class PieLabelComparer : IComparer
		{
			#region IComparer Members
			/// <summary>
			/// 
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public int Compare(object x, object y)
			{
				int res = 0;
				PieLabel pl1 = x as PieLabel;
				PieLabel pl2 = y as PieLabel;

				if (pl1 != null && pl2 != null)
				{
					if (pl1.Value != pl2.Value)
					{
						res = pl1.Value > pl2.Value ? 1 : -1;
					}
				}

				return res;
			}
			#endregion
		}

		/// <summary>
		/// 
		/// </summary>
		private class Pie3DSegment
		{
			public float StartAngle = 0;
			public float EndAngle = 0;
			public GraphicsPath LeftSide;
			public GraphicsPath RightSide;

			public GraphicsPath OuterSide;
			public GraphicsPath InnerSide;
			public PieSector Sector;

			/// <summary>
			/// Draws the specified graph.
			/// </summary>
			/// <param name="graph">The graph.</param>
			/// <param name="interior">The interior.</param>
			/// <param name="pen">The pen.</param>
            /// <param name="series">The ChartSeries.</param>
			public void Draw(ChartGraph graph, BrushInfo interior, Pen pen, ChartSeries series)
			{
                
				if (this.StartAngle < 90 || this.StartAngle > 270)
				{
					this.DrawLeft(graph, interior, pen,series);
				}

				if (this.EndAngle > 90 && this.EndAngle < 270)
				{
					this.DrawRight(graph, interior, pen, series);
				}

				if (this.EndAngle != this.StartAngle)
				{
					if (this.StartAngle >= 180)
					{
						this.DrawOuter(graph, interior, pen);
						this.DrawInner(graph, interior, pen);
					}
					else
					{
						this.DrawInner(graph, interior, pen);
						this.DrawOuter(graph, interior, pen);
					}
				}

				if (this.StartAngle > 90 && this.StartAngle < 270)
				{
					this.DrawLeft(graph, interior, pen,series);
				}

				if (this.EndAngle < 90 || this.EndAngle > 270)
				{
					this.DrawRight(graph, interior, pen,series);
				}
			}

			/// <summary>
			/// Gets the region.
			/// </summary>
			/// <param name="regionData">The region data.</param>
			/// <returns></returns>
			public ChartRegion GetRegion(ChartRegionData regionData)
			{
				Region rgn = new Region(Rectangle.Empty);

				if (LeftSide != null)
				{
					rgn.Union(LeftSide);
				}

				if (RightSide != null)
				{
					rgn.Union(RightSide);
				}

				if (OuterSide != null)
				{
					rgn.Union(OuterSide);
				}

				if (InnerSide != null)
				{
					rgn.Union(InnerSide);
				}

				return new ChartRegion(rgn, regionData);
			}

			private void DrawInner(ChartGraph graph, BrushInfo interior, Pen pen)
			{
				if (this.InnerSide != null)
				{
					graph.DrawPath(interior, null, InnerSide);

					using (LinearGradientBrush lgb = new LinearGradientBrush(this.Sector.InnerBounds,
						Color.White, Color.Black, LinearGradientMode.Horizontal))
					{
						lgb.InterpolationColors = c_cylinderPhong;
						graph.DrawPath(lgb, pen, InnerSide);
					}
				}
			}

			private void DrawOuter(ChartGraph graph, BrushInfo interior, Pen pen)
			{
				if (this.OuterSide != null)
				{
					graph.DrawPath(interior, null, OuterSide);

					using (LinearGradientBrush lgb = new LinearGradientBrush(this.Sector.OuterBounds,
						Color.White, Color.Black, LinearGradientMode.Horizontal))
					{
						lgb.InterpolationColors = c_cylinderPhong;
						graph.DrawPath(lgb, pen, OuterSide);
					}
				}
			}
			
			private void DrawLeft(ChartGraph graph, BrushInfo interior, Pen pen, ChartSeries series)
			{
				if (this.LeftSide != null)
				{
					graph.DrawPath(interior, pen, this.LeftSide);

                    if (series.ExplodedAll || series.ExplodedIndex >= 0)
                    {
                        using (LinearGradientBrush lgb = new LinearGradientBrush(this.Sector.OuterBounds,
                            Color.White, Color.Black, LinearGradientMode.Horizontal))
                        {
                            lgb.InterpolationColors = c_cylinderPhong;
                            graph.DrawPath(lgb, pen, this.LeftSide);
                        }
                    }
                    
				}
			}

			private void DrawRight(ChartGraph graph, BrushInfo interior, Pen pen, ChartSeries series)
			{
				if (this.RightSide != null)
				{
					graph.DrawPath(interior, pen, this.RightSide);

                    if (series.ExplodedAll || series.ExplodedIndex >= 0)
                    {
                        using (LinearGradientBrush lgb = new LinearGradientBrush(this.Sector.OuterBounds,
                            Color.White, Color.Black, LinearGradientMode.Horizontal))
                        {
                            lgb.InterpolationColors = c_cylinderPhong;
                            graph.DrawPath(lgb, pen, this.RightSide);
                        }
                    }
				}
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// 
		/// </summary>
		static PieRenderer()
		{
			#region Inside
			ColorBlend inside = new ColorBlend();

			inside.Positions = new float[] { 0f, 0.05f, 1f };
			inside.Colors = new Color[] { Color.Transparent, Color.Transparent, Color.FromArgb(0x64000000) };

			s_insideGradient = inside;
			#endregion

			#region Outside
			ColorBlend outside = new ColorBlend();

			outside.Positions = new float[] { 0f, 0.05f, 1f };
			outside.Colors = new Color[] { Color.FromArgb(0x32000000), Color.Transparent, Color.Transparent };

			s_outsideGradient = outside;
			#endregion

			#region Round
			ColorBlend round = new ColorBlend();

			round.Positions = new float[] { 0f, 0.4f, 0.6f, 1f };
			round.Colors = new Color[] { Color.FromArgb(0x48000000), Color.Transparent, Color.Transparent, Color.FromArgb(0x48000000) };

			s_roundGradient = round;
			#endregion

			#region Bevel
			ColorBlend bevel = new ColorBlend();

			bevel.Positions = new float[] { 0f, 0.04f, 0.05f, 0.3f, 0.4f, 1f };
			bevel.Colors = new Color[]{ Color.FromArgb( 0x32000000 ), Color.FromArgb( 0x64000000 ),
        Color.FromArgb( 0x32000000 ), Color.FromArgb( 0x48000000 ), Color.FromArgb( 0x24000000 ), Color.Transparent };

			s_bevelGradient = bevel;
			#endregion

			Color[] colors;
			float[] positions;
			ColorBlend colorBlend = new ColorBlend();

			ChartSeriesRenderer.PhongShadingColors(Color.FromArgb(0x90, Color.Black), Color.FromArgb(0x90, Color.Black),
				Color.FromArgb(100, Color.White), Math.PI / 4, 30, out colors, out positions);

			colorBlend.Positions = positions;
			colorBlend.Colors = colors;
			c_cylinderPhong = colorBlend;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="series"></param>
		public PieRenderer(ChartSeries series)
			: base(series)
		{
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Renders chart by the specified args.
		/// </summary>
		/// <param name="args">The args.</param>
		public override void Render(ChartRenderArgs2D args)
		{
			ChartPieConfigItem pieConfig = m_series.ConfigItems.PieItem;

			m_startAngle = (float)ChartMath.ModAngle( pieConfig.AngleOffset, 360);

			int serIndex = Chart.Series.IndexOf(m_series);
			int yIndex = m_series.PointFormats[ChartYValueUsage.YValue];
			double all = GetAllValue();
			float maxTitleHeight = 0;

			foreach (ChartSeries cs in Chart.Series)
			{
				if ((cs.Type == ChartSeriesType.Pie) && (cs.ConfigItems.PieItem.PieWithSameRadius == true))
				{
					ChartStyleInfo csi = cs.GetOfflineStyle();
					SizeF textSize = args.Graph.MeasureString(cs.Text, csi.GdipFont);

					if (maxTitleHeight < textSize.Height)
					{
						maxTitleHeight = textSize.Height;
					}
				}
			}

            if (all == 0)
            {
                return;
            }
                            			
			double coef = 360d / all;
			bool series3D = Chart.Series3D;
			bool optimizePoints = m_series.OptimizePiePointPositions;
			bool outerLabels = pieConfig.LabelStyle == ChartAccumulationLabelStyle.OutsideInColumn ||
				pieConfig.LabelStyle == ChartAccumulationLabelStyle.Outside || pieConfig.LabelStyle == ChartAccumulationLabelStyle.OutsideInArea;
			bool showTicks = m_series.ShowTicks && outerLabels;

            RectangleF bounds = args.Bounds;          
            SizeF inflateSize = SizeF.Empty;
            inflateSize = pieConfig.PieSize;            

            if (ChartArea.MultiplePies && pieConfig.PieRadius <= 0)
            {
                bounds = RenderingHelper.GetPieBounds(serIndex, this.Chart.Series.VisibleList.Count, bounds);
                if (serIndex != 0)
                {
                    RectangleF innerBounds = RenderingHelper.GetPieBounds(serIndex - 1, this.Chart.Series.VisibleList.Count, bounds);                    
                }
            }
                       
			if (pieConfig.ShowSeriesTitle)
			{
				SizeF titleSize = args.Graph.MeasureString(args.Series.Text, this.SeriesStyle.GdipFont);

                if (pieConfig.PieWithSameRadius == true && (pieConfig.LabelStyle == ChartAccumulationLabelStyle.Outside || pieConfig.LabelStyle == ChartAccumulationLabelStyle.OutsideInColumn))
                {
			        titleSize.Height = maxTitleHeight;
                }              

				RectangleF titleRect = LayoutHelper.AlignRectangle(bounds, titleSize, ContentAlignment.BottomCenter);

				using (SolidBrush sb = new SolidBrush(this.SeriesStyle.TextColor))
				{
					args.Graph.DrawString(args.Series.Text, this.SeriesStyle.GdipFont, sb, titleRect);
				}

				bounds.Height -= titleSize.Height;
			}

			int count = m_series.Points.Count;
			float radius = Math.Min(bounds.Width / 2, bounds.Height / 2);
            float storeRadius = radius;           

			float pieHeight = 0;
			PointF center = ChartMath.GetCenter(bounds);
			SizeF radiusXY = SizeF.Empty;
			SizeF outRadiusXY = SizeF.Empty;

			ChartStyledPoint[] points = this.PrepearePoints().Clone() as ChartStyledPoint[];

			#region Optimize points
			if (optimizePoints)
			{
				int len4 = points.Length / 4;
				for (int i = 0; i < len4; i++)
				{
					int ind = 2 * i + 1;
					ChartStyledPoint tp = points[ind];
					points[ind] = points[points.Length - ind - 1];
					points[points.Length - ind - 1] = tp;
				}
			}
			#endregion

			PieLabel[] labels = null;

			if (outerLabels)
			{
				labels = this.CreateLabels(points);
				SizeF resultRadiusXY = this.MeasureLabels(labels, args.Graph, new SizeF(bounds.Width / 2, bounds.Height / 2));
				radius = Math.Min(resultRadiusXY.Width, resultRadiusXY.Height);
			}

            if (pieConfig.PieWithSameRadius == true && (pieConfig.LabelStyle == ChartAccumulationLabelStyle.Outside || pieConfig.LabelStyle == ChartAccumulationLabelStyle.OutsideInColumn))
            {
                radius = storeRadius;
            }

            if ((pieConfig.PieRadius > 0)&&(!pieConfig.PieWithSameRadius))
            {
                radius = pieConfig.PieRadius;
            }

			if (args.Is3D)
			{
				pieHeight = pieConfig.HeightByAreaDepth ? ChartArea.Depth : pieConfig.HeightCoeficient * radius;
				pieHeight += pieHeight * ChartArea.Tilt / 180;
                radiusXY = new SizeF(radius - MARGINS_RATIO * radius, radius - MARGINS_RATIO * radius - pieHeight * ChartArea.Tilt / 90);

                if (ChartArea.MultiplePies)
                {                     	
                    pieHeight = pieConfig.HeightByAreaDepth ? (ChartArea.Depth * (ChartArea.Tilt / 180)) : pieConfig.PieHeight;
                    radiusXY = new SizeF(radius - MARGINS_RATIO * radius, radius - MARGINS_RATIO * radius - pieHeight * pieConfig.PieTilt);
                }         
       
                center = new PointF(center.X, center.Y - pieHeight / 2);
			}
			else
			{
				radiusXY = new SizeF(radius - MARGINS_RATIO * radius, radius - MARGINS_RATIO * radius);
			}

			if (outerLabels)
			{
				this.ArrangeLabels(labels, radiusXY, center, ((pieConfig.LabelStyle == ChartAccumulationLabelStyle.OutsideInColumn)||(pieConfig.LabelStyle == ChartAccumulationLabelStyle.OutsideInArea)));
			}

			float spacingCoef = 1 - SPACE_RATIO;
			radiusXY = new SizeF(spacingCoef * radiusXY.Width, spacingCoef * radiusXY.Height);

			if (showTicks)
			{
				radiusXY.Width -= this.SeriesStyle.TextOffset;
				radiusXY.Height -= this.SeriesStyle.TextOffset;
			}

			outRadiusXY = radiusXY;

			if (m_series.ExplodedAll || m_series.ExplodedIndex >= 0)
			{
				float explosionCoef = (1 - PERCENT_COEF * m_series.ExplosionOffset);
				radiusXY = new SizeF(radiusXY.Width * explosionCoef, radiusXY.Height * explosionCoef);
			}

			List<PieSector> sectors = new List<PieSector>();
			RectangleF boundsRect = new RectangleF(center.X - radiusXY.Width, center.Y - radiusXY.Height,
				2 * radiusXY.Width, 2 * radiusXY.Height);
			double curr = all * m_startAngle / 90f;

			#region Fill sectors
			for (int i = 0; i < count; i++)
			{
				if (points[i].IsVisible)
				{
					RectangleF rect = boundsRect;
					RectangleF rect2 = boundsRect;
                    
                    rect2.Inflate(-((1 - pieConfig.DoughnutCoeficient) * (rect.Width - inflateSize.Width) / 2),
                        -((1 - pieConfig.DoughnutCoeficient) * (rect.Height - inflateSize.Height) / 2));

					double val = this.GetMaxZero(points[i].YValues[yIndex]);
					ChartStyleInfo style = points[i].Style;

					if (m_series.ExplodedIndex == points[i].Index || m_series.ExplodedAll)
					{
						PointF offset = new PointF(
							(float)(Math.Cos(2 * Math.PI * (curr + val / 2) / all)),
							(float)(Math.Sin(2 * Math.PI * (curr + val / 2) / all)));

						rect.Offset(0.01f * outRadiusXY.Width * offset.X * m_series.ExplosionOffset,
							0.01f * outRadiusXY.Height * offset.Y * m_series.ExplosionOffset);

						rect2.Offset(0.01f * outRadiusXY.Width * offset.X * m_series.ExplosionOffset,
							 0.01f * outRadiusXY.Height * offset.Y * m_series.ExplosionOffset);
					}

					sectors.Add(new PieSector(points[i], rect, rect2, (float)(coef * curr), (float)(coef * (curr + val)), pieHeight));

					if (!series3D && style.DisplayShadow)
					{
						Matrix mtr = new Matrix();
						GraphicsPath shp = new GraphicsPath();
						shp.AddPie(Rectangle.Round(rect), (float)(coef * curr), (float)(coef * val));
						mtr.Translate(style.ShadowOffset.Width, style.ShadowOffset.Height);
						shp.Transform(mtr);
						args.Graph.DrawPath(style.ShadowInterior, null, shp);
					}

					curr = (curr + val) % all;
				}
			}
			#endregion

			ColorBlend gradient = pieConfig.PieType == ChartPieType.Custom ?
				pieConfig.Gradient : SelectKnow(pieConfig.PieType);
			List<Pie3DSegment> pie3DSegments = new List<Pie3DSegment>();

			foreach (PieSector sector in sectors)
			{
				IEnumerable<Pie3DSegment> segments = sector.Render();

				if( segments != null )
					pie3DSegments.AddRange(segments);
			}

			#region Add regions
			if (args.UpdateRegions)
			{
				if (args.Is3D)
				{
					foreach (Pie3DSegment segment in pie3DSegments)
					{
						ChartRegionData crgd = new ChartRegionData(serIndex, segment.Sector.StyledPoint.Index,
							segment.Sector.StyledPoint.ToolTip, c_pieRegionDescription);

						args.Chart.ChartRegions.Add(segment.GetRegion(crgd));
					}
				}

				foreach (PieSector sector in sectors)
				{
					ChartRegionData crgd = new ChartRegionData(serIndex, sector.StyledPoint.Index,
						sector.StyledPoint.ToolTip, c_pieRegionDescription);

					args.Chart.ChartRegions.Add(sector.GetRegion(crgd));
				}
			}
			#endregion

			pie3DSegments.Sort(new Comparison<Pie3DSegment>(CompareByStartAngle));

			if (args.Is3D)
			{
				foreach (Pie3DSegment segment in pie3DSegments)
				{
                        segment.Draw(args.Graph, this.GetBrush(segment.Sector.StyledPoint.Index),
                            segment.Sector.StyledPoint.Style.GdipPen, m_series);
                    
				}
			}

			foreach (PieSector sector in sectors)
			{
                sector.Draw(args.Graph, this.GetBrush(sector.StyledPoint.Index),
                    sector.StyledPoint.Style.GdipPen, pieConfig.FillMode, gradient);              
			}

			#region Draw labels
			if (pieConfig.LabelStyle != ChartAccumulationLabelStyle.Disabled)
			{
				if (outerLabels)
				{
					foreach (PieLabel pieLabel in labels)
					{
						RectangleF rect = boundsRect;
						ChartStyleInfo style = pieLabel.StyledPoint.Style;
                       
                        if( style.DisplayText )
						{
							if (m_series.ShowTicks)
							{
								if (m_series.ExplodedIndex == pieLabel.StyledPoint.Index || m_series.ExplodedAll)
								{
									PointF offset = new PointF((float)Math.Cos(pieLabel.Angle), (float)Math.Sin(pieLabel.Angle));

									rect.Offset(0.01f * outRadiusXY.Width * offset.X * m_series.ExplosionOffset,
										0.01f * outRadiusXY.Height * offset.Y * m_series.ExplosionOffset);
								}
								args.Graph.DrawLine(style.GdipPen, ChartMath.GetPointByAngle(rect, pieLabel.Angle, true), pieLabel.NotCorrectPoint);
								args.Graph.DrawLine(style.GdipPen, pieLabel.ConnectPoint, pieLabel.NotCorrectPoint);
							}

							using (SolidBrush sb = new SolidBrush(style.TextColor))
							{
                                if (m_series.ConfigItems.PieItem.ShowDataBindLabels && m_series.XAxis.LabelsImpl != null)
                                {
                                    String str = m_series.XAxis.LabelsImpl.GetLabelAt(pieLabel.LabelIndex).Text;
                                    Font fnt = m_series.XAxis.LabelsImpl.GetLabelAt(pieLabel.LabelIndex).Font;
                                    args.Graph.DrawString(str, fnt, sb, pieLabel.Rectangle);
                                }
                                else if (style.Text != "")
                                {
								    args.Graph.DrawString(style.Text, style.GdipFont, sb, pieLabel.Rectangle);
							    }
						}
					}
				}
				}
				else
				{
					StringFormat strFormat = new StringFormat();
					strFormat.Alignment = StringAlignment.Center;
                    strFormat.LineAlignment = StringAlignment.Center;
					curr = all * m_startAngle / 90f;
					foreach (ChartStyledPoint styledPoint in points)
					{
                        if (!styledPoint.Point.IsEmpty)
                        {
                            ChartStyleInfo style = styledPoint.Style;
                            double val = this.GetMaxZero(styledPoint.YValues[yIndex]);
                            if (style.DisplayText)
                            {
                                using (Brush sb = new SolidBrush(style.TextColor))
                                {
                                    PointF medPoint = new PointF(center.X + (float)(INSIDE_LABELS_COEF * radiusXY.Width * Math.Cos(2 * Math.PI * (curr + val / 2) / all)),
                                        center.Y + (float)(INSIDE_LABELS_COEF * radiusXY.Height * Math.Sin(2 * Math.PI * (curr + val / 2) / all)));

                                    if (m_series.ConfigItems.PieItem.ShowDataBindLabels && m_series.XAxis.LabelsImpl != null)
                                    {
                                        String str = m_series.XAxis.LabelsImpl.GetLabelAt(styledPoint.Index).Text;
                                        Font fnt = m_series.XAxis.LabelsImpl.GetLabelAt(styledPoint.Index).Font;
                                        args.Graph.DrawString(str, fnt, sb, medPoint, strFormat);
                                    }
                                    else if (style.Text != "")
                                    {
                                        args.Graph.DrawString(style.Text, style.GdipFont, sb, medPoint, strFormat);
                                    }
                                }
                            }
                            curr += val;
                        }
					}
				}
                
			}
			#endregion
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		public override void Render(Graphics3D g)
		{
			ChartPieConfigItem pieConfig = m_series.ConfigItems.PieItem;
            int serIndex = Chart.Series.IndexOf(m_series);

			m_startAngle = (float)ChartMath.ModAngle(pieConfig.AngleOffset, 360);

			double all = GetAllValue();

			if (all == 0)
			{
				return;
			}

			double coef = 360d / all;
			bool optimizePoints = m_series.OptimizePiePointPositions;
			bool outerLabels = pieConfig.LabelStyle == ChartAccumulationLabelStyle.Outside
				|| pieConfig.LabelStyle == ChartAccumulationLabelStyle.OutsideInColumn;

			RectangleF bounds = this.Bounds;          
            SizeF inflateSize = SizeF.Empty;
            inflateSize = pieConfig.PieSize;  

            if (ChartArea.MultiplePies && pieConfig.PieRadius <= 0)
            {
                bounds = RenderingHelper.GetPieBounds(serIndex, this.Chart.Series.VisibleList.Count, bounds);
                if (serIndex != 0)
                {
                    RectangleF innerBounds = RenderingHelper.GetPieBounds(serIndex - 1, this.Chart.Series.VisibleList.Count, bounds);
                }
            }

            float maxTitleHeight = 0;
            
            foreach (ChartSeries cs in Chart.Series)
            {
                if ( (cs.Type == ChartSeriesType.Pie) && (cs.ConfigItems.PieItem.PieWithSameRadius==true ))
                {
                    ChartStyleInfo csi = cs.GetOfflineStyle();
                    SizeF textSize = g.Graphics.MeasureString(cs.Text, csi.GdipFont);
                    if (maxTitleHeight < textSize.Height)
                    {
                        maxTitleHeight = textSize.Height;
                    }
                }
            }
       
			if (pieConfig.ShowSeriesTitle)
			{
				GraphicsPath gp = new GraphicsPath();
				SizeF titleSize = g.Graphics.MeasureString(m_series.Text, this.SeriesStyle.GdipFont);
                if (pieConfig.PieWithSameRadius == true && (pieConfig.LabelStyle == ChartAccumulationLabelStyle.Outside || pieConfig.LabelStyle == ChartAccumulationLabelStyle.OutsideInColumn))
                {
                  titleSize.Height = maxTitleHeight;
                }
				RectangleF titleRect = LayoutHelper.AlignRectangle(bounds, titleSize, ContentAlignment.BottomCenter);
				RenderingHelper.AddTextPath(gp, g.Graphics, m_series.Text, this.SeriesStyle.GdipFont, titleRect);

				g.AddPolygon(Path3D.FromGraphicsPath(gp, 0, new SolidBrush(this.SeriesStyle.TextColor)));
				bounds.Height -= titleSize.Height;
			}

			int index = Chart.Series.IndexOf(m_series);
			int count = m_series.Points.Count;
			float radius = (float)((1d - MARGINS_RATIO) * Math.Min(bounds.Width / 2, bounds.Height / 2));

            float storeRadius = radius;            

			float outRadius = 0f;

			ChartStyledPoint[] points = PrepearePoints().Clone() as ChartStyledPoint[];

			#region Optimize points
			if (optimizePoints)
			{
				int len4 = points.Length / 4;
				for (int i = 0; i < len4; i++)
				{
					int ind = 2 * i + 1;
					ChartStyledPoint tp = points[ind];
					points[ind] = points[points.Length - ind - 1];
					points[points.Length - ind - 1] = tp;
				}
			}
			#endregion

			PieLabel[] labels = null;

			if (outerLabels)
			{
				labels = CreateLabels(points);
				SizeF resultRadiusXY = this.MeasureLabels(labels, new ChartGDIGraph(g.Graphics), new SizeF(bounds.Width / 2, bounds.Height / 2));
				radius = Math.Min(resultRadiusXY.Width, resultRadiusXY.Height);
				ArrangeLabels(labels, new SizeF(radius, radius), Center, pieConfig.LabelStyle == ChartAccumulationLabelStyle.OutsideInColumn);
			}

            if (pieConfig.PieWithSameRadius == true && (pieConfig.LabelStyle == ChartAccumulationLabelStyle.Outside || pieConfig.LabelStyle == ChartAccumulationLabelStyle.OutsideInColumn))
            {
                radius = storeRadius;
                ArrangeLabels(labels, new SizeF(radius, radius), Center, false);
            }

            if (pieConfig.PieRadius > 0)
            {
                radius = pieConfig.PieRadius;

                if (pieConfig.PieWithSameRadius == true && (pieConfig.LabelStyle == ChartAccumulationLabelStyle.Outside || pieConfig.LabelStyle == ChartAccumulationLabelStyle.OutsideInColumn))
                {                    
                    ArrangeLabels(labels, new SizeF(radius, radius), Center, false);
                }
            }

            float pieHeight = pieConfig.HeightByAreaDepth ? ChartArea.Depth : pieConfig.HeightCoeficient * radius;            

            if (ChartArea.MultiplePies)
            {
                pieHeight = pieConfig.HeightByAreaDepth ? ChartArea.Depth: pieConfig.PieHeight;             
            }            

			radius *= (1 - SPACE_RATIO);

			if (m_series.ShowTicks)
			{
				radius -= this.SeriesStyle.TextOffset;
			}

			outRadius = radius;

			if (m_series.ExplodedAll || m_series.ExplodedIndex >= 0)
			{
				radius = (1 - PERCENT_COEF * m_series.ExplosionOffset) * radius;
			}

            g.CreateBox(new Vector3D(Bounds.Left, Bounds.Top, 0), new Vector3D(Bounds.Right, Bounds.Bottom, pieHeight), null, (BrushInfo)null);            
			double curr = 0;
			RectangleF boundsRect = new RectangleF(Center.X - radius, Center.Y - radius, 2 * radius, 2 * radius);
			ArrayList[] poligons = new ArrayList[]{ new ArrayList(), new ArrayList(),
                                              new ArrayList(), new ArrayList() };
			for (int i = 0; i < count; i++)
			{
				if (points[i].Point.IsEmpty)
				{
					continue;
				}

				RectangleF rect = boundsRect;
				double val = GetMaxZero(points[i].Point.YValues[0]);
				ChartStyleInfo style = GetStyleAt(points[i].Index);
				BrushInfo seriesInterior = GetBrush(points[i].Index);
				GraphicsPath gp = new GraphicsPath();

				if (m_series.ExplodedIndex == points[i].Index || m_series.ExplodedAll)
				{
					PointF offset = new PointF(
						(float)(Math.Cos(2 * Math.PI * (curr + val / 2) / all)),
						(float)(Math.Sin(2 * Math.PI * (curr + val / 2) / all)));

					rect.Offset(0.01f * outRadius * offset.X * m_series.ExplosionOffset,
						0.01f * outRadius * offset.Y * m_series.ExplosionOffset);
				}

                Vector3D cnt = new Vector3D(rect.X + rect.Width / 2, rect.Y + rect.Height / 2, 0);                
          
                Polygon[][] plgs = CreateSector(cnt, pieConfig.DoughnutCoeficient * radius + ((Math.Min(inflateSize.Width, inflateSize.Height) / 2)), radius, (float)(coef * curr), (float)(coef * val),
                   pieHeight, style.GdipPen, seriesInterior);

                ChartRegionData crd = new ChartRegionData(index, points[i].Index, GetToolTip(points[i].Index), "Pie Chart Region");

				for (int ai = 0; ai < plgs.Length; ai++)
				{
					if (plgs[ai] != null)
					{
						for (int pi = 0; pi < plgs[ai].Length; pi++)
						{
							poligons[ai].Add(plgs[ai][pi]);

							if (Chart.NeedRegionUpdate)
							{
								plgs[ai][pi].RegionData = crd;
							}
						}
					}
				}
				curr += val;
			}

			for (int ai = 0; ai < poligons.Length; ai++)
			{
				foreach (Polygon plg in poligons[ai])
				{
					g.AddPolygon(plg);
				}
			}

			if (pieConfig.LabelStyle != ChartAccumulationLabelStyle.Disabled)
			{
				curr = 0;
				GraphicsPath lines = new GraphicsPath();
				ArrayList arr = new ArrayList();

                int idx = 0;
				for (int i = 0; i < count; i++)
				{
					if (points[i].Point.IsEmpty)
					{
						continue;
					}

					double val = GetMaxZero(points[i].Point.YValues[0]);
					ChartStyleInfo style = GetStyleAt(points[i].Index);
					Brush brsh = new SolidBrush(style.TextColor);
					StringFormat strFormat = new StringFormat();
                  

                    if (style.DisplayText && style.Text != "" && (!style.IsEmpty))
					{
						if (outerLabels)
						{
                            PieLabel lb = labels[idx];

                            idx++;

							if (m_series.ShowTicks)
							{
								RectangleF rect = boundsRect;

								if (m_series.ExplodedIndex == lb.StyledPoint.Index || m_series.ExplodedAll)
								{
									PointF offset = new PointF((float)Math.Cos(lb.Angle), (float)Math.Sin(lb.Angle));

									rect.Offset(0.01f * outRadius * offset.X * m_series.ExplosionOffset,
										0.01f * outRadius * offset.Y * m_series.ExplosionOffset);
								}

								lines.AddLine(ChartMath.GetPointByAngle(rect, lb.Angle, true), lb.NotCorrectPoint);
								lines.AddLine(lb.NotCorrectPoint, lb.ConnectPoint);
								lines.AddLine(lb.ConnectPoint, lb.NotCorrectPoint);
								lines.CloseFigure();
							}

                            if (m_series.ConfigItems.PieItem.ShowDataBindLabels && m_series.XAxis.LabelsImpl != null)
                            {
                                String str = m_series.XAxis.LabelsImpl.GetLabelAt(points[i].Index).Text;
                                Font fnt = m_series.XAxis.LabelsImpl.GetLabelAt(points[i].Index).Font;

                                Pseudo3DText p3dt = new Pseudo3DText(str, fnt, brsh,
                                new Vector3D(lb.Rectangle.X + lb.Rectangle.Width / 2, lb.Rectangle.Y + lb.Rectangle.Height / 2, 0));
                                p3dt.Alignment = ContentAlignment.MiddleCenter;
                                arr.Add(p3dt);
                            }
                            else if (style.Text != "")
							{
								Pseudo3DText p3dt = new Pseudo3DText(style.Text, style.GdipFont, brsh,
									new Vector3D(lb.Rectangle.X + lb.Rectangle.Width / 2, lb.Rectangle.Y + lb.Rectangle.Height / 2, 0));
								p3dt.Alignment = ContentAlignment.MiddleCenter;
								arr.Add(p3dt);
							}
						}
						else
						{
                            PointF medPoint = new PointF(Center.X + (float)(INSIDE_LABELS_COEF * radius * Math.Cos(2 * Math.PI * (curr + val / 2) / all)),
                                Center.Y + (float)(INSIDE_LABELS_COEF * radius * Math.Sin(2 * Math.PI * (curr + val / 2) / all)));
                           
                            if (m_series.ConfigItems.PieItem.ShowDataBindLabels && m_series.XAxis.LabelsImpl != null)
                            {
                                String str = m_series.XAxis.LabelsImpl.GetLabelAt(points[i].Index).Text;
                                Font fnt = m_series.XAxis.LabelsImpl.GetLabelAt(points[i].Index).Font;

                                Pseudo3DText p3dt = new Pseudo3DText(str, fnt,
                                     brsh, new Vector3D(medPoint.X, medPoint.Y, 0));
                                p3dt.Alignment = ContentAlignment.MiddleCenter;
                                arr.Add(p3dt);
                            }
                            else if (style.Text != null)
							{
								Pseudo3DText p3dt = new Pseudo3DText(style.Text, style.GdipFont,
									brsh, new Vector3D(medPoint.X, medPoint.Y, 0));
								p3dt.Alignment = ContentAlignment.MiddleCenter;

								arr.Add(p3dt);
							}
						}
					}

					curr += val;
				}

				if (arr.Count > 0)
				{
					Path3DCollect coll = new Path3DCollect((Polygon[])arr.ToArray(typeof(Polygon)));

					if (lines.PointCount > 0)
					{
                        coll.Add(Path3D.FromGraphicsPath(lines, 0, SeriesStyle.GdipPen));                       
					}

					g.AddPolygon(coll);
				}
			}
		}
		/// <summary>
		/// Overloaded. Renders elements such as Text and Point Symbols.
		/// </summary>
		/// <param name="g">The graphics object that is to be used.</param>
		protected internal override void RenderAdornments(Graphics g)
		{
			//base.RenderAdornments(g);
		}
		/// <summary>
		/// Renders elements such as Text and Point Symbols.
		/// </summary>
		/// <param name="g">The graphics object that is to be used.</param>
		protected internal override void RenderAdornments(Graphics3D g)
		{
			//base.RenderAdornments(g);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private int CompareByStartAngle(Pie3DSegment x, Pie3DSegment y)
		{
			if (x != y)
			{
				float c1 = this.GetCost(x);
				float c2 = this.GetCost(y);

				return c1.CompareTo(c2);
			}

			return 0;
		}

		/// <summary>
		/// Gets the cost.
		/// </summary>
		/// <param name="segment">The segment.</param>
		/// <returns></returns>
		private float GetCost(Pie3DSegment segment)
		{ 
			if( segment.StartAngle < 90 && segment.EndAngle > 90)
			{
				return this.GetCost(90);
			}
			else if( segment.StartAngle < 270 && segment.EndAngle > 270)
			{
				return this.GetCost(270);
			}

			if (segment.StartAngle == segment.EndAngle)
			{
				return Math.Max(this.GetCost(segment.StartAngle),
					this.GetCost(segment.EndAngle + c_orderEpsilon));
			}

			return Math.Max(this.GetCost(segment.StartAngle), 
				this.GetCost(segment.EndAngle)); 
		}
		/// <summary>
		/// Gets the cost.
		/// </summary>
		/// <param name="angle">The angle.</param>
		/// <returns></returns>
		private float GetCost(float angle)
		{
			if (angle < 90) return 90 + angle;
			if (angle > 270) return angle - 270;

			return 270 - angle;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private int CompareByEndAngle(Pie3DSegment x, Pie3DSegment y)
		{
			int res = 0;
			float ps1StartAngle = x.EndAngle;
			float ps2StartAngle = y.EndAngle;

			if (ps1StartAngle > 90 && ps1StartAngle < 270)
			{
				if (ps2StartAngle > 90 && ps2StartAngle < 270)
				{
					res = ps1StartAngle > ps2StartAngle ? -1 : 1;
				}
				else
				{
					res = -1;
				}
			}
			else
			{
				if ((ps2StartAngle > 90 && ps2StartAngle < 270)
					|| (ps1StartAngle < 90) && (ps2StartAngle > 270))
				{
					res = 1;
				}
				else if ((ps1StartAngle > 270) && (ps2StartAngle < 90))
				{
					res = -1;
				}
				else
				{
					res = ps1StartAngle < ps2StartAngle ? -1 : 1;
				}
			}

			return res;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <returns></returns>
		public override SizeF GetMinSize(Graphics g)
		{
			float right = 0;
			float bottom = 0;
			float left = float.MaxValue;
			float top = float.MaxValue;
			int count = m_series.Points.Count;

			ChartStyledPoint[] points = PrepearePoints().Clone() as ChartStyledPoint[];

			if (m_series.OptimizePiePointPositions)
			{
				int len4 = points.Length / 4;
				for (int i = 0; i < len4; i++)
				{
					int ind = 2 * i + 1;
					ChartStyledPoint tp = points[ind];
					points[ind] = points[points.Length - ind - 1];
					points[points.Length - ind - 1] = tp;
				}
			}

			float rad = MIN_RADIUS;
			PieLabel[] lbls = CreateLabels(points);
			MeasureLabels(lbls, new ChartGDIGraph(g), new SizeF(rad, rad));

			for (int i = 0; i < lbls.Length; i++)
			{
				top = Math.Min(top, lbls[i].Rectangle.Top);
				left = Math.Min(left, lbls[i].Rectangle.Left);
				right = Math.Max(right, lbls[i].Rectangle.Right);
				bottom = Math.Max(bottom, lbls[i].Rectangle.Bottom);
			}

			float width = (Chart != null) ? Math.Max(right - left, ChartArea.MinSize.Width) : right - left;
			float height = (Chart != null) ? Math.Max(bottom - top, ChartArea.MinSize.Height) : bottom - top;

			return new SizeF(width, height);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="center"></param>
		/// <param name="inSideRadius"></param>
		/// <param name="outSideRadius"></param>
		/// <param name="start"></param>
		/// <param name="fov"></param>
		/// <param name="depth"></param>
		/// <param name="pen"></param>
		/// <param name="brInfo"></param>
		/// <returns></returns>
		private Polygon[][] CreateSector(Vector3D center, float inSideRadius, float outSideRadius
			, float start, float fov, float depth, Pen pen, BrushInfo brInfo)
		{
			int count = fov >= MIN_FOV_SECTOR ? (int)(fov / MIN_FOV_SECTOR) : 1;
			float f = fov / count;
			Polygon[][] res = new Polygon[4][];

			PointF[] oPts = new PointF[count + 1];
			PointF[] iPts = new PointF[count + 1];

			#region Calc all points
			for (int i = 0; i < count + 1; i++)
			{
                float ox = (float)(center.X + outSideRadius * Math.Cos((start + i * f) * D_TO_R));
                float oy = (float)(center.Y + outSideRadius * Math.Sin((start + i * f) * D_TO_R));

                oPts[i] = new PointF(ox, oy);

                float ix = (float)(center.X + inSideRadius * Math.Cos((start + i * f) * D_TO_R));
                float iy = (float)(center.Y + inSideRadius * Math.Sin((start + i * f) * D_TO_R));

                iPts[i] = new PointF(ix, iy);                                
			}
			#endregion

			#region Draw inside and outside poligons
			Polygon[] oPlgs = new Polygon[count];

			for (int i = 0; i < count; i++)
			{
				Vector3D[] vts = new Vector3D[]{ new Vector3D( oPts[ i ].X, oPts[ i ].Y, 0 ),
                                         new Vector3D( oPts[ i ].X, oPts[ i ].Y, depth ),
                                         new Vector3D( oPts[ i+1 ].X, oPts[ i+1 ].Y, depth ),
                                         new Vector3D( oPts[ i+1 ].X, oPts[ i+1 ].Y, 0 ) };

				oPlgs[i] = new Polygon(vts, brInfo, null);
			}

			res[1] = oPlgs;

			if (inSideRadius > 0)
			{
				Polygon[] iPlgs = new Polygon[count];

				for (int i = 0; i < count; i++)
				{
					Vector3D[] vts = new Vector3D[]{ new Vector3D( iPts[ i ].X, iPts[ i ].Y, 0 ),
                                           new Vector3D( iPts[ i ].X, iPts[ i ].Y, depth ),
                                           new Vector3D( iPts[ i+1 ].X, iPts[ i+1 ].Y, depth ),
                                           new Vector3D( iPts[ i+1 ].X, iPts[ i+1 ].Y, 0 ) };

                    iPlgs[i] = new Polygon(vts, brInfo, null);                   
				}

				res[3] = iPlgs;
			}
			#endregion

			#region Draw top and bottom poligons
			ArrayList tVtxs = new ArrayList();
			ArrayList bVtxs = new ArrayList();

			for (int i = 0; i < count + 1; i++)
			{
				tVtxs.Add(new Vector3D(oPts[i].X, oPts[i].Y, 0));
                bVtxs.Add(new Vector3D(oPts[i].X, oPts[i].Y, depth));                
			}

			if (inSideRadius > 0)
			{
				for (int i = count; i > -1; i--)
				{
                    tVtxs.Add(new Vector3D(iPts[i].X, iPts[i].Y, 0));
                    bVtxs.Add(new Vector3D(iPts[i].X, iPts[i].Y, depth));                                       
				}
			}
			else
			{
				tVtxs.Add(center);
                bVtxs.Add(new Vector3D(center.X, center.Y, depth));
			}

            if (ChartArea.MultiplePies)
            {                                   
                Pen pen2 = new Pen(Color.Transparent, pen.Width);
                res[0] = new Polygon[]{ new Polygon( (Vector3D[])tVtxs.ToArray( typeof( Vector3D )), brInfo, pen ),
                            new Polygon( (Vector3D[])bVtxs.ToArray( typeof( Vector3D )), brInfo, pen2 ) };
            }
            else
            {
                res[0] = new Polygon[]{ new Polygon( (Vector3D[])tVtxs.ToArray( typeof( Vector3D )), brInfo, pen ),
                                    new Polygon( (Vector3D[])bVtxs.ToArray( typeof( Vector3D )), brInfo, pen ) };
            }
			#endregion

			#region Draw left and right poligons
			if (inSideRadius > 0)
			{
				Vector3D[] rvts = new Vector3D[]{ new Vector3D( oPts[ 0 ].X, oPts[ 0 ].Y, 0 ),
                                          new Vector3D( oPts[ 0 ].X, oPts[ 0 ].Y, depth ),
                                          new Vector3D( iPts[ 0 ].X, iPts[ 0 ].Y, depth ),
                                          new Vector3D( iPts[ 0 ].X, iPts[ 0 ].Y, 0 ) };

				Vector3D[] lvts = new Vector3D[]{ new Vector3D( oPts[ count ].X, oPts[ count ].Y, 0 ),
                                          new Vector3D( oPts[ count ].X, oPts[ count ].Y, depth ),
                                          new Vector3D( iPts[ count ].X, iPts[ count ].Y, depth ),
                                          new Vector3D( iPts[ count ].X, iPts[ count ].Y, 0 ) };

                res[2] = new Polygon[]{ new Polygon( rvts, brInfo, pen ),
                                  new Polygon( lvts, brInfo, pen ) };
                
			}
			else
			{
				Vector3D[] rvts = new Vector3D[]{ new Vector3D( oPts[ 0 ].X, oPts[ 0 ].Y, 0 ),
                                          new Vector3D( oPts[ 0 ].X, oPts[ 0 ].Y, depth ),
                                          new Vector3D( center.X, center.Y, depth ),
                                          new Vector3D( center.X, center.Y, 0 ) };

				Vector3D[] lvts = new Vector3D[]{ new Vector3D( oPts[ count ].X, oPts[ count ].Y, 0 ),
                                          new Vector3D( oPts[ count ].X, oPts[ count ].Y, depth ),
                                          new Vector3D( center.X, center.Y, depth ),
                                          new Vector3D( center.X, center.Y, 0 ) };

                res[2] = new Polygon[]{ new Polygon( rvts, brInfo, pen ),
                                  new Polygon( lvts, brInfo, pen ) };
              
			}
			#endregion

			return res;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="points"></param>
		/// <returns></returns>
		private PieLabel[] CreateLabels(ChartStyledPoint[] points)
		{
			double all = GetAllValue();
			double coef = ChartMath.DblPI / all;
			double curr = m_startAngle * all / 90f;
			ArrayList labels = new ArrayList();

			for (int i = 0; i < points.Length; i++)
			{
                if (points[i].Point.IsEmpty)
                {
                    continue;
                }
				double val = GetMaxZero(points[i].Point.YValues[0]);
				curr = (curr + val / 2) % all;
        PieLabel lbl = new PieLabel(points[i], (float)(coef * curr), m_series,points[i].Index);

				if (curr <= 0.25 * all)
				{
					lbl.Corner = PieSectorCorner.BottomRight;
				}
				else if (curr <= 0.50 * all)
				{
					lbl.Corner = PieSectorCorner.BottomLeft;
				}
				else if (curr <= 0.75 * all)
				{
					lbl.Corner = PieSectorCorner.TopLeft;
				}
				else
				{
					lbl.Corner = PieSectorCorner.TopRight;
				}

				lbl.Value = curr;
				labels.Add(lbl);

				curr = (curr + val / 2) % all;
			}

			return (PieLabel[])labels.ToArray(typeof(PieLabel));
		}
		/// <summary>
		/// Measures the labels.
		/// </summary>
		/// <param name="labels">The labels.</param>
		/// <param name="g">The g.</param>
		/// <param name="radius">The radius.</param>
		/// <returns></returns>
		private SizeF MeasureLabels(PieLabel[] labels, ChartGraph g, SizeF radius)
		{
			float left = 0;
			float right = 0;

			foreach (PieLabel lbl in labels)
			{
				lbl.Measure(g, radius.Width);

				if (lbl.Corner == PieSectorCorner.TopLeft || lbl.Corner == PieSectorCorner.BottomLeft)
				{
					left = Math.Max(left, lbl.Rectangle.Width);
				}
				else
				{
					right = Math.Max(right, lbl.Rectangle.Width);
				}
			}

			return new SizeF(radius.Width - Math.Max(left, right), radius.Height);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="labels"></param>
		/// <param name="radius"></param>
		/// <param name="center"></param>
		/// <param name="inColumn"></param>
		private void ArrangeLabels(PieLabel[] labels, SizeF radius, PointF center, bool inColumn)
		{
			ArrayList topLeft = new ArrayList();
			ArrayList topRight = new ArrayList();
			ArrayList bottomLeft = new ArrayList();
			ArrayList bottomRight = new ArrayList();
			PieLabelComparer pieLabelComparer = new PieLabelComparer();

			#region Arrange and sort the labels
			foreach (PieLabel lbl in labels)
			{
				float cos = (float)Math.Cos(lbl.Angle);
				float sin = (float)Math.Sin(lbl.Angle);
				lbl.SetConnectPoint(new PointF(center.X + radius.Width * cos,
					center.Y + radius.Height * sin));

				if (lbl.Corner == PieSectorCorner.TopLeft) topLeft.Add(lbl);
				if (lbl.Corner == PieSectorCorner.TopRight) topRight.Add(lbl);
				if (lbl.Corner == PieSectorCorner.BottomLeft) bottomLeft.Add(lbl);
				if (lbl.Corner == PieSectorCorner.BottomRight) bottomRight.Add(lbl);
			}

			topLeft.Sort(pieLabelComparer);
			topRight.Sort(pieLabelComparer);
			bottomLeft.Sort(pieLabelComparer);
			bottomRight.Sort(pieLabelComparer);
			#endregion
            
			#region Correct the labels
			////////////////////////////////////////////////////////////////////
			float corr = center.Y;

			for (int i = 0; i < topLeft.Count; i++)
			{
				PieLabel lb = (topLeft[i] as PieLabel);
				lb.CorrectTopLeft(corr);
				corr = lb.Rectangle.Top;
			}
			////////////////////////////////////////////////////////////////////
			corr = center.Y;

			for (int i = bottomLeft.Count - 1; i > -1; i--)
			{
				PieLabel lb = (bottomLeft[i] as PieLabel);
				lb.CorrectBottomLeft(corr);
				corr = lb.Rectangle.Bottom;
			}
			////////////////////////////////////////////////////////////////////
			corr = center.Y;

			for (int i = topRight.Count - 1; i > -1; i--)
			{
				PieLabel lb = (topRight[i] as PieLabel);
				lb.CorrectTopRight(corr);
				corr = lb.Rectangle.Top;
			}
			////////////////////////////////////////////////////////////////////
			corr = center.Y;

			for (int i = 0, c = bottomRight.Count; i < c; i++)
			{
				PieLabel lb = (bottomRight[i] as PieLabel);
				lb.CorrectBottomRight(corr);
				corr = lb.Rectangle.Bottom;
			}
			#endregion
      
			#region Aling the labels
			if (inColumn)
			{
				foreach (PieLabel lbl in labels)
				{
					if (lbl.Corner == PieSectorCorner.TopLeft || lbl.Corner == PieSectorCorner.BottomLeft)
					{
						lbl.AlignRightSide(center.X - radius.Width);
					}
					else
					{
						lbl.AlignLeftSide(center.X + radius.Width);
					}
				}
			}
			#endregion
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private double GetAllValue()
		{
			double res = 0;

			for (int i = 0; i < m_series.Points.Count; i++)
			{
				ChartPoint pt = m_series.Points[i];

				if (pt.IsEmpty)
				{
					continue;
				}

				res += GetMaxZero(pt.YValues[0]);
			}

			return res;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private double GetMaxZero(double value)
		{
			return Math.Abs( value );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private ColorBlend SelectKnow(ChartPieType type)
		{
			ColorBlend result = null;

			switch (type)
			{
				case ChartPieType.InSide:
					result = s_insideGradient;
					break;

				case ChartPieType.OutSide:
					result = s_outsideGradient;
					break;

				case ChartPieType.Round:
					result = s_roundGradient;
					break;

				case ChartPieType.Bevel:
					result = s_bevelGradient;
					break;
			}

			return result;
		}
		/// <summary>
		/// Draws the icon of pie chart on the legend.
		/// </summary>
		/// <param name="index">Index of point.</param>
		/// <param name="g">Instance of <see cref="Graphics"/>.</param>
		/// <param name="bounds">Bounds of icon.</param>
		/// <param name="isShadow">If is true method draws the shadow.</param>
		/// <param name="shadowColor"><see cref="Color"/> of shadow.</param>
		public override void DrawIcon(int index, Graphics g, Rectangle bounds, bool isShadow, Color shadowColor)
		{
			GraphicsPath gp = new GraphicsPath();

			gp.AddPie(bounds.X, bounds.Y, 2 * bounds.Width, 2 * bounds.Height, -180, 90);

			if (isShadow)
			{
				using (SolidBrush br = new SolidBrush(shadowColor))
				{
					g.FillPath(br, gp);
				}
			}
			else
			{
				BrushPaint.FillPath(g, gp, this.GetBrush(index));
				g.DrawPath(SeriesStyle.GdipPen, gp);
			}
		}
		#endregion
	}
}
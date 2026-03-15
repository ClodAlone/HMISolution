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
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart.Renderers
{
	/// <summary>
	/// Summary description for PointAndFigureRenderer.
	/// </summary>
	internal class PointAndFigureRenderer : ChartSeriesRenderer
	{
		#region Class PNFColumn
		/// <summary>
		/// 
		/// </summary>
		private class PNFColumn
		{
			public double X = 0;
			public double Width = 0;
			public double Low = 0;
			public double High = 0;
			public bool IsPoint = false;
			public int indexX = 0;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Get description of regions.
		/// </summary>
		/// <value></value>
		protected override string RegionDescription
		{
			get
			{
				return "PointAndFigure Chart Region";
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
				return 2;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="series"></param>
		public PointAndFigureRenderer(ChartSeries series)
			: base(series)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Renders the specified args.
		/// </summary>
		/// <param name="args">The args.</param>
		public override void Render(ChartRenderArgs2D args)
		{
			PNFColumn[] columns = this.ComputeRectangles(m_series);

			if (columns != null)
			{
				ChartStyleInfo style = this.SeriesStyle;

				Pen penUp = style.GdipPen.Clone() as Pen;
				Pen penDown = style.GdipPen.Clone() as Pen;
				Pen penShadow = style.GdipPen.Clone() as Pen;

				penUp.Color = m_series.ConfigItems.FinancialItem.PriceUpColor;
				penDown.Color = m_series.ConfigItems.FinancialItem.PriceDownColor;
				penShadow.Color = style.ShadowInterior.BackColor;

				int ir = 0, mi = columns.Length, di = 1;

				if (args.ActualXAxis.Inversed)
				{
					ir = mi - di;
					mi = -1;
					di = -1;
				}

				for (; ir != mi; ir += di)
				{
					PNFColumn column = columns[ir];
                    if (column.X>args.ActualXAxis.Range.Min && column.X < args.ActualXAxis.Range.Max)
                    {
                        column.X = Math.Min(Math.Max(column.X, args.ActualXAxis.Range.Min), args.ActualXAxis.Range.Max);
                        column.Width = column.X + column.Width < args.ActualXAxis.Range.Max ? column.Width : args.ActualXAxis.Range.Max - column.X;
                        column.Low = Math.Min(Math.Max(column.Low, args.ActualYAxis.Range.Min), args.ActualYAxis.Range.Max);
                        column.High = Math.Min(Math.Max(column.High, args.ActualYAxis.Range.Min), args.ActualYAxis.Range.Max);
                    }
					PointF pt1 = args.GetPoint(column.X, column.Low);
					PointF pt2 = args.GetPoint(column.X + column.Width, column.High);

					RectangleF rect = ChartMath.CorrectRect(pt1.X, pt1.Y, pt2.X, pt2.Y);

					if (args.Is3D)
					{
						GraphicsPath gp = this.CreateBox(rect, true);

						args.Graph.DrawPath(this.GetBrush(), style.GdipPen, gp);

						if (args.UpdateRegions)
						{
							this.Chart.ChartRegions.Add(new ChartRegion(new Region(gp), args.SeriesIndex,
								GetToolTip(), this.RegionDescription));
						}
					}

					int count = (int)((column.High - column.Low) / m_series.HeightBox + 0.5);

					for (int j = 0; j < count; j++)
					{
						PointF xpt1 = args.GetPoint(column.X, column.Low + j * m_series.HeightBox);
						PointF xpt2 = args.GetPoint(column.X + column.Width, column.Low + (j + 1) * m_series.HeightBox);

						RectangleF xRect = ChartMath.CorrectRect(xpt1.X, xpt1.Y, xpt2.X, xpt2.Y);

						if (style.DisplayShadow && !Chart.Series3D)
						{
							RectangleF sRect = xRect;
							sRect.Offset(style.ShadowOffset.Width, style.ShadowOffset.Height);
							if (!column.IsPoint)
							{
								DrawFigureX(args.Graph, penShadow, sRect);
							}
							else
							{
								DrawPointO(args.Graph, penShadow, sRect);
							}
						}
						if (!column.IsPoint)
						{
							DrawFigureX(args.Graph, penUp, xRect);
						}
						else
						{
							DrawPointO(args.Graph, penDown, xRect);
						}
					}

					if (!args.Is3D && args.UpdateRegions)
					{
						this.Chart.ChartRegions.Add(new ChartRegion(new Region(rect), args.SeriesIndex,
							GetToolTip(), this.RegionDescription));
					}
				}

				penUp.Dispose();
				penDown.Dispose();
				penShadow.Dispose();
			}
		}
		/// <summary>
		/// Renders the specified args.
		/// </summary>
		/// <param name="args">The args.</param>
		public override void Render(ChartRenderArgs3D args)
		{
			PNFColumn[] columns = this.ComputeRectangles(m_series);

			if (columns != null)
			{
				ChartStyleInfo style = this.SeriesStyle;

				Pen penUp = style.GdipPen.Clone() as Pen;
				Pen penDown = style.GdipPen.Clone() as Pen;

				penUp.Color = m_series.ConfigItems.FinancialItem.PriceUpColor;
				penDown.Color = m_series.ConfigItems.FinancialItem.PriceDownColor;

				ChartRegionData crd = null;

				args.Graph.AddPolygon(this.CreateBoundsPolygon((float)args.Z));

				if (args.UpdateRegions)
				{
					crd = new ChartRegionData(args.SeriesIndex, this.GetToolTip(), this.RegionDescription);
				}

				for (int i = 0; i < columns.Length; i ++)
				{
					PNFColumn column = columns[i];
					PointF pt1 = args.GetPoint(column.X, column.Low);
					PointF pt2 = args.GetPoint(column.X + column.Width, column.High);

					RectangleF rect = ChartMath.CorrectRect(pt1.X, pt1.Y, pt2.X, pt2.Y);
                    //=====================================
                    Polygon[] plgs = args.Graph.CreateBoxV(new Vector3D(rect.Left, rect.Top, args.Z),
                        new Vector3D(rect.Right, rect.Bottom, args.Z + args.Depth), style.GdipPen, style.Interior);
                    foreach (Polygon plg in plgs)
                    {
                        plg.RegionData = crd;
                    }
                    int count = (int)((column.High - column.Low) / m_series.HeightBox + 0.5);
                    float top = rect.Top;
                    for (int j = 0; j < count; j++)
                    {
                        if (column.IsPoint)
                            args.Graph.CreateEllipse(new Vector3D(rect.Left, top, args.Z), new SizeF(rect.Width, rect.Height / count), 25, penDown, null);
                        else
                            args.Graph.CreateRectangle(new Vector3D(rect.Left, top, args.Z), new SizeF(rect.Width, (rect.Height) / count), penUp, null, true);
                        top += rect.Height / count;
                    }
                }
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="pen"></param>
		/// <param name="rect"></param>
		private void DrawPointO(ChartGraph g, Pen pen, RectangleF rect)
		{
			PointF ltPoint = new PointF(rect.X + pen.Width,
				rect.Y + pen.Width);
			SizeF sizeEllipse = new SizeF(rect.Width - 2 * pen.Width,
				rect.Height - 2 * pen.Width);
			g.DrawEllipse((Brush)null, pen, ltPoint.X, ltPoint.Y, sizeEllipse.Width,sizeEllipse.Height);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="pen"></param>
		/// <param name="rect"></param>
		private void DrawFigureX(ChartGraph g, Pen pen, RectangleF rect)
		{
			PointF ltPoint = new PointF(rect.X + pen.Width,
				rect.Y + pen.Width);
			PointF lbPoint = new PointF(rect.X + pen.Width,
				rect.Y + rect.Height - pen.Width);

			PointF rtPoint = new PointF(rect.X + rect.Width - pen.Width,
				rect.Y + pen.Width / 2);
			PointF rbPoint = new PointF(rect.X + rect.Width - pen.Width,
				rect.Y + rect.Height - pen.Width);

			g.DrawLine(pen, ltPoint.X, ltPoint.Y, rbPoint.X, rbPoint.Y);
			g.DrawLine(pen, lbPoint.X, lbPoint.Y, rtPoint.X, rtPoint.Y);
		}

		/// <summary>
		/// Computes the rectangles.
		/// </summary>
		/// <param name="series">The series.</param>
		/// <returns></returns>
		private PNFColumn[] ComputeRectangles(ChartSeries series)
		{
			ArrayList columns = new ArrayList();
			int pointCount = series.Points.Count;

			for (int i = 0; i < pointCount; i++)
			{
				if (m_series.Points[i].YValues.Length < 2)
					return null;
				double x = m_series.Points[i].X;
				double yh = Math.Max(m_series.Points[i].YValues[0], m_series.Points[i].YValues[1]);
				double yl = Math.Min(m_series.Points[i].YValues[0], m_series.Points[i].YValues[1]);

				yh = Math.Floor(yh / m_series.HeightBox) * m_series.HeightBox + m_series.HeightBox;
				yl = Math.Floor(yl / m_series.HeightBox) * m_series.HeightBox;

				if (columns.Count == 0)
				{
					PNFColumn column = new PNFColumn();
					column.X = m_series.Points[i].X;
					column.High = yh;
					column.Low = yl;
					column.Width = 1;
					column.IsPoint = true;
					columns.Add(column);
				}
				else
				{
					PNFColumn currColumn = columns[columns.Count - 1] as PNFColumn;
					currColumn.Width = m_series.Points[i].X - currColumn.X;

					if (currColumn.IsPoint)
					{
						if (yl < currColumn.Low)
						{
							currColumn.Low = yl;
						}
						if (yh > currColumn.Low + series.ReversalAmount)
						{

							PNFColumn column = new PNFColumn();
							column.indexX = i;
							column.X = m_series.Points[i].X;
							column.High = yh;
							column.Low = currColumn.Low + series.HeightBox;
							column.IsPoint = false;
							column.Width = 1;
							columns.Add(column);
						}
					}
					else
					{
						if (yh > currColumn.High)
						{
							currColumn.High = yh;
						}
						if (yl < currColumn.High - series.ReversalAmount)
						{
							PNFColumn column = new PNFColumn();
							column.indexX = i;
							column.X = m_series.Points[i].X;
							column.Low = yl;
							column.High = currColumn.High - series.HeightBox;
							column.IsPoint = true;
							column.Width = 1;
							columns.Add(column);
						}
					}
				}
			}

			return columns.ToArray(typeof(PNFColumn)) as PNFColumn[];
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
			base.DrawIcon(g, bounds, isShadow, shadowColor);

			if (!isShadow)
			{
				int w4 = bounds.Width / 4;
				int h4 = bounds.Height / 4;

				Pen gsb = new Pen(m_series.ConfigItems.FinancialItem.PriceUpColor);
				Pen rsb = new Pen(m_series.ConfigItems.FinancialItem.PriceDownColor);

				g.DrawEllipse(rsb, bounds);
				g.DrawLine(gsb, bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
				g.DrawLine(gsb, bounds.Left, bounds.Bottom, bounds.Right, bounds.Top);

				gsb.Dispose();
				rsb.Dispose();
			}
		}
		#endregion
	}
}

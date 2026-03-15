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
using System.ComponentModel;
using System.Diagnostics;
using Syncfusion.Documentation;


namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	/// This class is the wrapper for <see cref="ChartSeries"/>. 
	/// Implements the <see cref="IEditableChartSeriesModel"/> and <see cref="IChartSeriesModel"/> interfaces.
	/// </summary>
	public class ChartSeriesModelAdapter : IChartSeriesModel, IEditableChartSeriesModel
	{
		#region Members
		private ChartSeries m_series;
		#endregion

		#region Events
		/// <summary>
		/// Please refer to <see cref="IChartSeriesModel.Changed"/>.
		/// </summary>
		public event ListChangedEventHandler Changed
		{ 
			add {}
			remove {}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Please refer to <see cref="IChartSeriesModel.Count"/>.
		/// </summary>
		public virtual int Count
		{
			get
			{
				return m_series.SeriesModel.Count;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="series">The series.</param>
		public ChartSeriesModelAdapter(ChartSeries series)
		{
			m_series = series;
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Please refer to <see cref="IChartSeriesModel.GetX"/>.
		/// </summary>
		public virtual double GetX(int xIndex)
		{
			return m_series.SeriesModel.GetX(xIndex);
		}
		/// <summary>
		/// Please refer to <see cref="IChartSeriesModel.GetY"/>.
		/// </summary>
		public virtual double[] GetY(int xIndex)
		{
			return m_series.SeriesModel.GetY(xIndex);
		}
		/// <summary>
		/// Please refer to <see cref="IChartSeriesModel.GetEmpty"/>.
		/// </summary>
		public virtual bool GetEmpty(int xIndex)
		{
			return m_series.SeriesModel.GetEmpty(xIndex);
		}
		/// <summary>
		/// Adds data to the end of the data representation.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="yValues">The y values.</param>
		public void Add(double x, double[] yValues)
		{
			if (m_series.IsEditableData())
			{
				m_series.GetEditableData().Add(x, yValues);
			}
		}
		/// <summary>
		/// Adds data to the end of the data representation.
		/// </summary>
		/// <param name="x">X value.</param>
		/// <param name="yValues">The y values.</param>
		/// <param name="isEmpty">if set to <c>true</c> point is empty.</param>
		public void Add(double x, double[] yValues, bool isEmpty)
		{
			if (m_series.IsEditableData())
			{
				m_series.GetEditableData().Add(x, yValues, isEmpty);
			}
		}
		/// <summary>
		/// Please refer to <see cref="IEditableChartSeriesModel.Insert"/>.
		/// </summary>
		/// <param name="xIndex">Index value where the insertion is to be made.</param>
		/// <param name="x">The X value.</param>
		/// <param name="yValues">The associated Y values.</param>
		public void Insert(int xIndex, double x, double[] yValues)
		{
			if (m_series.IsEditableData())
			{
				m_series.GetEditableData().Insert(xIndex, x, yValues);
			}
		}
		/// <summary>
		/// Please refer to <see cref="IEditableChartSeriesModel.SetX"/>.
		/// </summary>
		/// <param name="xIndex">Index value where the data is to be changed.</param>
		/// <param name="value">New X value.</param>
		public void SetX(int xIndex, double value)
		{
			if (m_series.IsEditableData())
			{
				m_series.GetEditableData().SetX(xIndex, value);
			}
		}
		/// <summary>
		/// Please refer to <see cref="IEditableChartSeriesModel.SetY"/>.
		/// </summary>
		/// <param name="xIndex">Index value where data is to be changed.</param>
		/// <param name="yValues">New Y values.</param>
		public void SetY(int xIndex, double[] yValues)
		{
			if (m_series.IsEditableData())
			{
				m_series.GetEditableData().SetY(xIndex, yValues);
			}
		}
		/// <summary>
		/// Please refer to <see cref="IEditableChartSeriesModel.SetEmpty"/>.
		/// </summary>
		/// <param name="xIndex">Index value where the empty state indicator is to be stored.</param>
		/// <param name="isEmpty">Empty state indicator.</param>
		public void SetEmpty(int xIndex, bool isEmpty)
		{
			if (m_series.IsEditableData())
			{
				m_series.GetEditableData().SetEmpty(xIndex, isEmpty);
			}
            else
            {
                if (m_series.SeriesModel is ChartDataBindModel)
                {
                    ChartDataBindModel model = (ChartDataBindModel)this.m_series.SeriesModel;
                    model.SetEmpty(xIndex, isEmpty);
                }
            }
		}
		/// <summary>
		/// Please refer to <see cref="IEditableChartSeriesModel.Remove"/>.
		/// </summary>
		/// <param name="xIndex">Index value where data is to be removed.</param>
		public void Remove(int xIndex)
		{
			if (m_series.IsEditableData())
			{
				m_series.GetEditableData().Remove(xIndex);
			}
		}
		/// <summary>
		/// Please refer to <see cref="IEditableChartSeriesModel.Clear"/>.
		/// </summary>
		public void Clear()
		{
			if (m_series.IsEditableData())
			{
				m_series.GetEditableData().Clear();
			}
		}
		#endregion
	}

	/// <summary>
	/// Contains predefined random values.
	/// </summary>
	[DocumentationExclude]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class ChartPredefinedValues
	{
		#region Implementation
		/// <summary>
		/// Gets the points.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public static ChartPoint[] GetPoints(ChartSeriesType type, int index)
		{
			ChartPoint[] result = null;

			switch (type)
			{
				case ChartSeriesType.Line:
				case ChartSeriesType.Spline:
				case ChartSeriesType.RotatedSpline:
				case ChartSeriesType.StepLine:
					{
						if (index % 3 == 0)
						{
							result = ConvertYtoPoints(new double[] { 15, 20, 30, 45, 65, 70, 75 });
						}
						else if (index % 3 == 1)
						{
							result = ConvertYtoPoints(new double[] { 45, 52, 62, 55, 50, 45, 30 });
						}
						else
						{
							result = ConvertYtoPoints(new double[] { 90, 80, 75, 85, 65, 55, 50 });
						}
					}
					break;

				case ChartSeriesType.Scatter:
				case ChartSeriesType.Column:
				case ChartSeriesType.Bar:
					{
						if (index % 2 == 0)
						{
							result = ConvertYtoPoints(new double[] { 55, 70, 80, 65, 75 });
						}
						else
						{
							result = ConvertYtoPoints(new double[] { 70, 35, 65, 25, 50 });
						}
					}
					break;

				case ChartSeriesType.Gantt:
					{
						if (index % 2 == 0)
						{
							result = ConvertYtoPoints(new double[] { 10, 12, 8, 15, 13 }, new double[] { 20, 22, 20, 24, 21 });
						}
						else
						{
							result = ConvertYtoPoints(new double[] { 15, 10, 13, 14, 17 }, new double[] { 25, 18, 21, 19, 26 });
						}
					}
					break;

				case ChartSeriesType.StackingBar:
				case ChartSeriesType.StackingBar100:
					{
						if (index % 3 == 0)
						{
							result = ConvertYtoPoints(new double[] { 55, 70, 80, 65, 75 });
						}
						else if (index % 3 == 1)
						{
							result = ConvertYtoPoints(new double[] { 70, 35, 65, 25, 50 });
						}
						else
						{
							result = ConvertYtoPoints(new double[] { 100, 55, 35, 45, 65 });
						}
					}

					break;

				case ChartSeriesType.Area:
				case ChartSeriesType.SplineArea:
				case ChartSeriesType.StepArea:
					{
						if (index % 2 == 0)
						{
							result = ConvertYtoPoints(new double[] { 55, 60, 75, 45, 50, 40, 30 });
						}
						else
						{
							result = ConvertYtoPoints(new double[] { 55, 70, 80, 65, 75, 70, 50 });
						}
					}
					break;

				case ChartSeriesType.RangeArea:
					{
						if (index % 2 == 0)
						{
							result = ConvertYtoPoints(new double[] { 20, 25, 25, 30, 15, 20, 30 }, new double[] { 70, 60, 75, 70, 70, 65, 55 });
						}
						else
						{
							result = ConvertYtoPoints(new double[] { 30, 35, 40, 35, 30, 35, 35 }, new double[] { 80, 75, 85, 75, 80, 75, 75 });
						}
					}
					break;

				case ChartSeriesType.StackingArea:
				case ChartSeriesType.StackingArea100:
					{
						if (index % 3 == 0)
						{
							result = ConvertYtoPoints(new double[] { 55, 60, 75, 45, 50, 40, 60 });
						}
						else if (index % 3 == 1)
						{
							result = ConvertYtoPoints(new double[] { 55, 70, 80, 65, 75, 70, 50 });
						}
						else
						{
							result = ConvertYtoPoints(new double[] { 100, 55, 35, 45, 65, 55, 50 });
						}
					}
					break;

				case ChartSeriesType.StackingColumn:
				case ChartSeriesType.StackingColumn100:
					{
						if (index % 3 == 0)
						{
							result = ConvertYtoPoints(new double[] { 55, 70, 80, 65, 75 });
						}
						else if (index % 3 == 1)
						{
							result = ConvertYtoPoints(new double[] { 70, 35, 65, 25, 50 });
						}
						else
						{
							result = ConvertYtoPoints(new double[] { 90, 80, 75, 85, 65 });
						}
					}
					break;

				case ChartSeriesType.Pie:
				case ChartSeriesType.Funnel:
				case ChartSeriesType.Pyramid:
					{
						result = ConvertYtoPoints(new double[] { 70, 35, 65, 25, 50 });
					}
					break;

				case ChartSeriesType.HiLo:
					{
						if (index % 2 == 0)
						{
							result = ConvertYtoPoints(new double[] { 65, 70, 68, 75, 102 }, new double[] { 30, 30, 15, 30, 40 });
						}
						else
						{
							result = ConvertYtoPoints(new double[] { 56, 55, 50, 65, 90 }, new double[] { 30, 15, 25, 45, 60 });
						}
					}
					break;

				case ChartSeriesType.HiLoOpenClose:
				case ChartSeriesType.Candle:
					{
						result = ConvertYtoPoints(new double[] { 65, 70, 68, 75, 102 }, new double[] { 30, 30, 15, 30, 40 },
							new double[] { 56, 55, 50, 65, 90 }, new double[] { 40, 40, 30, 45, 60 });
					}
					break;

				case ChartSeriesType.Bubble:
					{
						if (index % 2 == 0)
						{
							result = ConvertYtoPoints(new double[] { 55, 70, 80, 65, 75 }, new double[] { 1, 2, 3, 1, 7 });
						}
						else
						{
							result = ConvertYtoPoints(new double[] { 70, 35, 65, 25, 50 }, new double[] { 2, 3, 7, 2, 1 });
						}
					}
					break;

				case ChartSeriesType.Kagi:
				case ChartSeriesType.Renko:
				case ChartSeriesType.ThreeLineBreak:
					{
						result = ConvertYtoPoints(new double[] { 27, 25, 16, 24, 19, 18, 10, 15, 12, 19, 15, 12, 10, 22, 13, 15, 12, 10, 22, 13 });
					}
					break;

				case ChartSeriesType.Radar:
				case ChartSeriesType.Polar:
					{
						if (index % 2 == 0)
						{
							result = ConvertYtoPoints(new double[] { 70, 35, 65, 25, 50 });
						}
						else
						{
							result = ConvertYtoPoints(new double[] { 56, 55, 50, 65, 90 });
						}
					}
					break;

				case ChartSeriesType.ColumnRange:
					{
						if (index % 2 == 0)
						{
							result = ConvertYtoPoints(new double[] { 20, 25, 45, 50, 15 }, new double[] { 70, 60, 75, 70, 75 });
						}
						else
						{
							result = ConvertYtoPoints(new double[] { 30, 35, 20, 35, 30 }, new double[] { 80, 75, 55, 85, 80 });
						}
					}
					break;

				case ChartSeriesType.PointAndFigure:
					{
						result = ConvertYtoPoints(new double[] { 27, 25, 16, 24, 19, 18, 10, 15, 12, 19, 15, 12, 10, 22, 13, 15, 12, 10, 22, 13 },
							new double[] { 10, 12, 13, 15, 14, 15, 04, 08, 05, 14, 10, 06, 08, 18, 08, 12, 08, 09, 19, 10 });
					}
					break;

				case ChartSeriesType.BoxAndWhisker:
					{
						result = ConvertYtoPoints(new double[] { 10, 12, 8, 15, 13 }, new double[] { 20, 22, 20, 24, 21 },
							new double[] { 30, 33, 31, 34, 32 }, new double[] { 40, 40, 42, 45, 43 }, new double[] { 50, 51, 51, 52, 53 });
					}
					break;

				case ChartSeriesType.Histogram:
					{
						result = ConvertXtoPoints(new double[] { 100, 200, 350, 450, 500 });
					}
					break;

				case ChartSeriesType.HeatMap:
					{
						result = ConvertYtoPoints(new double[] { 27, 25, 16, 24, 19}, new double[] { 10, 12, 13, 15, 14 });
					}
					break;

				case ChartSeriesType.Tornado:
					{
						if (index % 2 == 0)
						{
							result = ConvertYtoPoints(new double[] { 0, 0, 0, 0, 0 }, new double[] { 20, 22, 20, 24, 21 });
						}
						else
						{
							result = ConvertYtoPoints(new double[] { 0, 0, 0, 0, 0 }, new double[] { -25, -18, -21, -19, -26 });
						}
					}
					break;

				case ChartSeriesType.Custom:
				default:
					result = new ChartPoint[0];
					break;
			}

			return result;
		}
		/// <summary>
		/// Gets the series count.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static int GetSeriesCount(ChartSeriesType type)
		{
			int result = 0;

			switch (type)
			{
				case ChartSeriesType.Line:
				case ChartSeriesType.Spline:
				case ChartSeriesType.RotatedSpline:
				case ChartSeriesType.StepLine:
				case ChartSeriesType.StackingBar:
				case ChartSeriesType.StackingArea:
				case ChartSeriesType.StackingColumn:
				case ChartSeriesType.StackingArea100:
				case ChartSeriesType.StackingBar100:
				case ChartSeriesType.StackingColumn100:
					result = 3;
					break;

				case ChartSeriesType.Scatter:
				case ChartSeriesType.Column:
				case ChartSeriesType.Bar:
				case ChartSeriesType.Area:
				case ChartSeriesType.RangeArea:
				case ChartSeriesType.SplineArea:
				case ChartSeriesType.StepArea:
				case ChartSeriesType.Radar:
				case ChartSeriesType.Gantt:
				case ChartSeriesType.HiLo:
				case ChartSeriesType.Bubble:
				case ChartSeriesType.Polar:
				case ChartSeriesType.ColumnRange:
				case ChartSeriesType.Tornado:
					result = 2;
					break;

				case ChartSeriesType.Pie:
				case ChartSeriesType.Funnel:
				case ChartSeriesType.Pyramid:
				case ChartSeriesType.HiLoOpenClose:
				case ChartSeriesType.Candle:
				case ChartSeriesType.ThreeLineBreak:
				case ChartSeriesType.PointAndFigure:
				case ChartSeriesType.Kagi:
				case ChartSeriesType.Renko:
				case ChartSeriesType.BoxAndWhisker:
				case ChartSeriesType.Histogram:
				case ChartSeriesType.HeatMap:
				case ChartSeriesType.Custom:
				default:
					result = 1;
					break;
			}

			return result;
		}
		/// <summary>
		/// Converts the 1D array to 2D.
		/// </summary>
		/// <param name="array">The array.</param>
		/// <returns></returns>
		private static ChartPoint[] ConvertYtoPoints(params double[][] array)
		{
			ChartPoint[] result = new ChartPoint[array[0].Length];

			for (int i = 0; i < result.Length; i++)
			{
				double[] ys = new double[array.Length];

				for (int j = 0; j < ys.Length; j++)
				{
					ys[j] = array[j][i];
				}

				result[i] = new ChartPoint(i + 1, ys);
			}

			return result;
		}
		/// <summary>
		/// Converts the xto points.
		/// </summary>
		/// <param name="xvalues">The xvalues.</param>
		/// <returns></returns>
		private static ChartPoint[] ConvertXtoPoints(double[] xvalues)
		{
			ChartPoint[] points = new ChartPoint[xvalues.Length];

			for (int i = 0; i < xvalues.Length; i++)
			{
				points[i] = new ChartPoint(xvalues[i], 0);
			}

			return points;
		}
		#endregion
	}

	/// <summary>
	/// Implements the <see cref="IChartSeriesModel"/> interfaces. 
	/// If values is empty, it's return "dummy" values.
	/// </summary>
	class ChartDummyPointsAdapter : IChartSeriesModel
	{
		#region Constants
		private const int c_yMaxValue = 400;
		private const int c_yMinValue = 100;
		private const int c_pointsCount = 5;

		private readonly static Random c_random = new Random();
		#endregion

		#region Members
		private ChartSeries m_series = null;
		#endregion

		#region Properties
		/// <summary>
		/// Returns the number of points in this series.
		/// </summary>
		/// <value></value>
		public int Count
		{
			get 
			{
				return m_series.SeriesModel.Count > 0 ? m_series.SeriesModel.Count : this.RandomPoints.Length;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private ChartPoint[] RandomPoints
		{
			get
			{
				if (m_series.ChartModel != null)
				{
					return ChartPredefinedValues.GetPoints(m_series.Type, m_series.ChartModel.Series.IndexOf(m_series));
				}

				return new ChartPoint[0];
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Occurs when Model is changed.
		/// </summary>
		public event ListChangedEventHandler Changed
		{
			add { }
			remove { }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartDummyPointsAdapter"/> class.
		/// </summary>
		/// <param name="series">The series.</param>
		public ChartDummyPointsAdapter(ChartSeries series)
		{
			m_series = series;
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Returns the X value of the series at the specified point index.
		/// </summary>
		/// <param name="xIndex">The index value of the point.</param>
		/// <returns>X value.</returns>
		public double GetX(int xIndex)
		{
			if (m_series.SeriesModel.Count > 0)
			{
				return m_series.SeriesModel.GetX(xIndex);
			}

			return this.RandomPoints[xIndex].X;
		}
		/// <summary>
		/// Returns the Y value of the series at the specified point index.
		/// </summary>
		/// <param name="xIndex">The index value of the point.</param>
		/// <returns>Y value.</returns>
		public double[] GetY(int xIndex)
		{
			if (m_series.SeriesModel.Count > 0)
			{
				return m_series.SeriesModel.GetY(xIndex);
			}

			return this.RandomPoints[xIndex].YValues;
		}
		/// <summary>
		/// Indicates whether a plottable value is present at the specified point index.
		/// </summary>
		/// <param name="xIndex">The index value of the point.</param>
		/// <returns>
		/// True, if there is a value present at this point index; false otherwise.
		/// </returns>
		public bool GetEmpty(int xIndex)
		{
			if (m_series.SeriesModel.Count > 0)
			{
				return m_series.SeriesModel.GetEmpty(xIndex);
			}

			return xIndex >= this.RandomPoints.Length || xIndex < 0;
		}
		#endregion
	}
}

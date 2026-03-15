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
using System.Collections.Generic;

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	/// 
	/// </summary>
	internal class ChartSeriesSummary : IChartSeriesSummary
	{
		#region Internal types
		/// <summary>
		/// 
		/// </summary>
		enum Coordinate
		{
			/// <summary>
			/// 
			/// </summary>
			X,
			/// <summary>
			/// 
			/// </summary>
			Y
		}
		#endregion

		#region Constants
		private const string c_textFormat = "MinX: {0}, MaxX: {1}, MinY: {2}, MaxY: {3}";
		#endregion

		#region Members
		private bool m_needUpdate = false;
		private IChartSeriesModel m_model = null;

		private double m_minX = 0;
		private double m_minY = 0;
		private double m_maxX = 0;
		private double m_maxY = 0;
		private double[] m_sums = null;
		#endregion

		#region Properties
		/// <summary>
		/// Returns the maximum X value.
		/// </summary>
		/// <value></value>
		public double MaxX
		{
			get
			{
				this.EnsureRefreshed();

				return m_maxX;
			}
		}
		/// <summary>
		/// Returns the maximum Y value.
		/// </summary>
		/// <value></value>
		public double MaxY
		{
			get
			{
				this.EnsureRefreshed();

				return m_maxY;
			}
		}
		/// <summary>
		/// Returns the minimum X value.
		/// </summary>
		/// <value></value>
		public double MinX
		{
			get
			{
				this.EnsureRefreshed();

				return m_minX;
			}
		}
		/// <summary>
		/// Returns the minimum Y value.
		/// </summary>
		/// <value></value>
		public double MinY
		{
			get
			{
				this.EnsureRefreshed();

				return m_minY;
			}
		}
		/// <summary>
		/// </summary>
		/// <value></value>
		/// <internalonly/>
		public IChartSeriesModel ModelImpl
		{
			get
			{
				return m_model;
			}
			set
			{
				if (m_model != value)
				{
					if (m_model != null)
					{
						m_model.Changed -= new ListChangedEventHandler(OnModelChanged);
					}

					m_model = value;

					if (m_model != null)
					{
						m_model.Changed += new ListChangedEventHandler(OnModelChanged);
					}

					m_needUpdate = true;
				}
			}
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Gets the Y percentage.
		/// </summary>
		/// <param name="pointIndex">Index of the point.</param>
		/// <returns></returns>
		/// <remarks>Percentages computes for positive values only.</remarks>
		public double GetYPercentage(int pointIndex)
		{
			return this.GetYPercentage(pointIndex, 0);
		}
		/// <summary>
		/// Gets the Y percentage.
		/// </summary>
		/// <param name="pointIndex">Index of the point.</param>
		/// <param name="yIndex">Index of the y.</param>
		/// <returns></returns>
		/// <remarks>Percentages computes for positive values only.</remarks>
		public double GetYPercentage(int pointIndex, int yIndex)
		{
			this.EnsureRefreshed();

			if (m_model != null)
			{
				return 100 * Math.Max(0, m_model.GetY(pointIndex)[yIndex]) / m_sums[yIndex];
			}

			return 0d;
		}

		/// <summary>
		/// Finds point by specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>Found point or null.</returns>
		public ChartPoint FindValue(double value)
		{
			int index = this.FindYValue(value, 0, 0, m_model.Count - 1);

			if (index > -1)
			{
				return new ChartPoint(m_model, index);
			}

			return null;
		}
		/// <summary>
		/// Finds point by specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="useValue">The use value.</param>
		/// <returns>Found point or null.</returns>
		public ChartPoint FindValue(double value, string useValue)
		{
			int index = 0;
			return this.FindValue(value, useValue, ref index);
		}
		/// <summary>
		/// Finds point by specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="useValue">Which point value to use (X, Y1, Y2,...).</param>
		/// <param name="index">Index to start looking from. Returns index of found point or -1.</param>
		/// <returns>Found point or null.</returns>
		public ChartPoint FindValue(double value, string useValue, ref int index)
		{
			return FindValue(value, useValue, ref index, m_model.Count - 1);
		}
		/// <summary>
		/// Finds point by specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="useValue">Which point value to use (X, Y1, Y2,...).</param>
		/// <param name="index">Index to start looking from. Returns index of found point or -1.</param>
        /// <param name="endIndex">The end index.</param>
		/// <returns>Found point or null.</returns>
		public ChartPoint FindValue(double value, string useValue, ref int index, int endIndex)
		{
			if (endIndex < index)
				throw new ArgumentOutOfRangeException("index must be less than endIndex.");

			if (index > m_model.Count - 1)
				throw new ArgumentOutOfRangeException("index must be less than points count.");

			if (endIndex > m_model.Count - 1)
				throw new ArgumentOutOfRangeException("endIndex must be less than points count.");

			int yIndex = 0;
			Coordinate coordinate = this.ProcessUsageString(useValue, out yIndex);

			if (coordinate == Coordinate.X)
			{
				index = this.FindXValue(value, index, endIndex);
			}
			else
			{
				index = this.FindYValue(value, yIndex, index, endIndex);
			}

			if (index > -1)
				{
					return new ChartPoint(m_model, index);
				}

			return null;
		}

		/// <summary>
		/// Finds point with minimal value.
		/// </summary>
		/// <returns>Found point or null.</returns>
		public ChartPoint FindMinValue()
		{
			int index = this.FindMinYValue(0, 0, m_model.Count - 1);

			if (index > -1)
			{
				return new ChartPoint(m_model, index);
			}

			return null;
		}
		/// <summary>
		/// Finds point with minimal value.
		/// </summary>
		/// <param name="useValue">The use value.</param>
		/// <returns>Found point or null.</returns>
		public ChartPoint FindMinValue(string useValue)
		{
			int index = 0;
			return this.FindMinValue(useValue, ref index);
		}
		/// <summary>
		/// Finds point with minimal value.
		/// </summary>
		/// <param name="useValue">Which point value to use (X, Y1, Y2,...).</param>
		/// <param name="index">Index to start looking from. Returns index of found point or -1.</param>
		/// <returns>Found point or null.</returns>
		public ChartPoint FindMinValue(string useValue, ref int index)
		{
			return FindMinValue(useValue, ref index, m_model.Count - 1);
		}
		/// <summary>
		/// Finds point with minimal value.
		/// </summary>
		/// <param name="useValue">Which point value to use (X, Y1, Y2,...).</param>
		/// <param name="index">Index to start looking from. Returns index of found point or -1.</param>
        /// <param name="endIndex">The index where the search is end.</param>
		/// <returns>Found point or null.</returns>
		public ChartPoint FindMinValue(string useValue, ref int index, int endIndex)
		{
			if (endIndex < index)
				throw new ArgumentOutOfRangeException("index must be less than endIndex.");

			if (index > m_model.Count - 1)
				throw new ArgumentOutOfRangeException("index must be less than points count.");

			if (endIndex > m_model.Count - 1)
				throw new ArgumentOutOfRangeException("endIndex must be less than points count.");

			int yIndex = 0;
			Coordinate coordinate = this.ProcessUsageString(useValue, out yIndex);

			if (coordinate == Coordinate.X)
			{
				index = this.FindMinXValue(index, endIndex);
			}
			else
			{
				index = this.FindMinYValue(yIndex, index, endIndex);
			}

			if (index > -1)
				{
					return new ChartPoint(m_model, index);
				}

			return null;
		}

		/// <summary>
		/// Finds point with maximal value.
		/// </summary>
		/// <returns>Found point or null.</returns>
		public ChartPoint FindMaxValue()
		{
			int index = this.FindMaxYValue(0, 0, m_model.Count - 1);

			if (index > -1)
			{
				return new ChartPoint(m_model, index);
			}

			return null;
		}
		/// <summary>
		/// Finds point with maximal value.
		/// </summary>
		/// <param name="useValue">The use value.</param>
		/// <returns>Found point or null.</returns>
		public ChartPoint FindMaxValue(string useValue)
		{
			int index = 0;
			return this.FindMaxValue(useValue, ref index);
		}
		/// <summary>
		/// Finds point with maximal value.
		/// </summary>
		/// <param name="useValue">Which point value to use (X, Y1, Y2,...).</param>
		/// <param name="index">Index to start looking from. Returns index of found point or -1.</param>
		/// <returns>Found point or null.</returns>
		public ChartPoint FindMaxValue(string useValue, ref int index)
		{
			return FindMaxValue(useValue, ref index, m_model.Count - 1);
		}
		/// <summary>
		/// Finds point with maximal value.
		/// </summary>
		/// <param name="useValue">Which point value to use (X, Y1, Y2,...).</param>
		/// <param name="index">Index to start looking from. Returns index of found point or -1.</param>
        /// <param name="endIndex">The end Index.</param>
		/// <returns>Found point or null.</returns>
		public ChartPoint FindMaxValue(string useValue, ref int index, int endIndex)
		{
			if (endIndex < index)
				throw new ArgumentOutOfRangeException("index must be less than endIndex.");

			if (index > m_model.Count - 1)
				throw new ArgumentOutOfRangeException("index must be less than points count.");

			if (endIndex > m_model.Count - 1)
				throw new ArgumentOutOfRangeException("endIndex must be less than points count."); 
			
			int yIndex = 0;
			Coordinate coordinate = this.ProcessUsageString(useValue, out yIndex);

			if (coordinate == Coordinate.X)
			{
				index = this.FindMaxXValue(index, endIndex);
			}
			else
			{
				index = this.FindMaxYValue(yIndex, index, endIndex);
			}

			if (index > -1)
				{
					return new ChartPoint(m_model, index);
				}

			return null;
		}

		/// <summary>
		/// Refreshes summary information.
		/// </summary>
		public void Refresh()
		{
			if (m_model != null)
			{
				m_minX = double.MaxValue;
				m_minY = double.MaxValue;
				m_maxX = double.MinValue;
				m_maxY = double.MinValue;

				List<double> sumValues = new List<double>();

				for (int i = 0; i < m_model.Count; i++)
				{
					double x = m_model.GetX(i);
					double[] ys = m_model.GetY(i);

					while (sumValues.Count < ys.Length)
					{
						sumValues.Add(0d);
					}

					for (int j = 0; j < ys.Length; j++)
					{
						sumValues[j] += Math.Max(0, ys[j]);
					}

					m_minX = Math.Min(m_minX, x);
					m_minY = Math.Min(m_minY, ChartMath.Min(ys));
					m_maxX = Math.Max(m_maxX, x);
					m_maxY = Math.Max(m_maxY, ChartMath.Max(ys));
				}

				m_sums = sumValues.ToArray();
			}
			else
			{
				m_minX = 0;
				m_minY = 0;
				m_maxX = 0;
				m_maxY = 0;
				m_sums = null;
			}
		}
		/// <summary>
		/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override string ToString()
		{
			return string.Format(c_textFormat, this.MinX, this.MaxX, this.MinY, this.MaxY);
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Finds the X value.
		/// </summary>
		/// <param name="value">The value.</param>
        ///<param name="from">The searching start at this index value</param> 
        ///<param name="to">The searching end at this index value</param>
		private int FindXValue(double value, int from, int to)
		{
			for (int i = from; i <= to; i++)
			{
				if (m_model.GetX(i) == value)
				{
					return i;
				}
			}

			return -1;
		}
		/// <summary>
		/// Finds the X value.
		/// </summary>
		/// <param name="value">The value.</param>
        /// <param name="yIndex">The yIndex</param> 
        ///<param name="from">The searching start at this index value</param> 
        ///<param name="to">The searching end at this index value</param>              
		private int FindYValue(double value, int yIndex, int from, int to)
		{
			for (int i = from; i <= to; i++)
			{
				double[] yValues = m_model.GetY(i);

				if (yValues.Length > yIndex && yValues[yIndex] == value)
				{
					return i;
				}
			}

			return -1;
		}
		/// <summary>
		/// Finds the X value.
		/// </summary>
        /// <param name="from">The searching start at this index value</param>
        /// <param name="to">The searching end at this index value</param>
		private int FindMinXValue(int from, int to)
		{
			int index = -1;
			double min = Double.MaxValue;

			for (int i = from; i <= to; i++)
			{
				double x = m_model.GetX(i);

				if (x < min)
				{
					min = x;
					index = i;
				}
			}

			return index;
		}
		/// <summary>
		/// Finds the X value.
		/// </summary>
        /// <param name="from">The searching start at this index value</param>
        /// <param name="to">The searching end at this index value</param>
		private int FindMaxXValue(int from, int to)
		{
			int index = -1;
			double max = Double.MinValue;

			for (int i = from; i <= to; i++)
			{
				double x = m_model.GetX(i);

				if (x > max)
				{
					max = x;
					index = i;
				}
			}

			return index;
		}
		/// <summary>
		/// Finds the X value.
		/// </summary>
		/// <param name="yIndex">The yIndex value.</param>
        /// <param name="from">The searching start at this index value</param>
        /// <param name="to">The searching end at this index value</param>
		private int FindMinYValue(int yIndex, int from, int to)
		{
			int index = -1;
			double min = Double.MaxValue;

			for (int i = from; i <= to; i++)
			{
				double[] yValues = m_model.GetY(i);

				if (yValues.Length > yIndex && yValues[yIndex] < min)
				{
					min = yValues[yIndex];
					index = i;
				}
			}

			return index;
		}
		/// <summary>
		/// Finds the X value.
		/// </summary>
		/// <param name="yIndex">The yIndex value.</param>
        /// <param name="from">The searching start at this index value</param>
        /// <param name="to">The searching end at this index value</param>
		private int FindMaxYValue(int yIndex, int from, int to)
		{
			int index = -1;
			double max = Double.MinValue;

			for (int i = from; i <= to; i++)
			{
				double[] yValues = m_model.GetY(i);
				
				if (yValues.Length > yIndex && yValues[yIndex] > max)
				{
					max = yValues[yIndex];
					index = i;
				}
			}

			return index;
		}

		/// <summary>
		/// Called when model is changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.ComponentModel.ListChangedEventArgs"/> instance containing the event data.</param>
		private void OnModelChanged(object sender, ListChangedEventArgs e)
		{
			m_needUpdate = true;
		}
		/// <summary>
		/// Ensures the refreshed.
		/// </summary>
		private void EnsureRefreshed()
		{
			if (m_needUpdate)
			{
				this.Refresh();
				m_needUpdate = false;
			}
		}
		/// <summary>
		/// Processes the useValue string.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		private Coordinate ProcessUsageString(string request, out int index)
		{
			if (string.Compare(request, "x", true) == 0)
			{
				index = -1;
				return Coordinate.X;
			}
			else if (request.StartsWith("y", StringComparison.InvariantCultureIgnoreCase))
			{
				if (request.Length == 1)
				{
					index = 0;
				}
				else
				{
					if(!int.TryParse(request.Substring(1), out index))
					{
						throw new ArgumentException("Incorrect request");
					}

					//index--;
				}

				return Coordinate.Y;
			}

			throw new ArgumentException("Incorrect request");
		}
		#endregion
	}
}
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

using System.ComponentModel;
using System.Diagnostics;

using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	/// Provides the wrapper for <see cref="IChartSeriesIndexedModel"/> that implements the <see cref="IChartSeriesModel"/>.
	/// </summary>
	/// <internalonly/>
	[ DocumentationExclude() ]
	public class ChartSeriesIndexedModelAdapter : IChartSeriesModel
	{
		#region Members
		private IChartSeriesIndexedModel m_indexedModel = null;
		#endregion

		#region Events
		/// <summary>
		/// Event that should be raised by any implementation of this interface if data that it holds changes. This will cause the
		/// chart to be updated accordingly.
		/// </summary>
		/// <internalonly/>
		public event ListChangedEventHandler Changed
		{
			add
			{
				m_indexedModel.Changed += value;
			}
			remove
			{
				m_indexedModel.Changed -= value;
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the <see cref="IChartSeriesIndexedModel"/>.
		/// </summary>
		/// <value>The <see cref="IChartSeriesIndexedModel"/>.</value>
		/// <internalonly/>
		public IChartSeriesIndexedModel Inner
		{
			get
			{
				return m_indexedModel;
			}
		}
		/// <summary>
		/// Returns the number of points in this series.
		/// </summary>
		/// <value></value>
		/// <internalonly/>
		public int Count
		{
			get
			{
				return m_indexedModel.Count;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartSeriesIndexedModelAdapter"/> class.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <internalonly/>
		public ChartSeriesIndexedModelAdapter( IChartSeriesIndexedModel model )
		{
			m_indexedModel = model;
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Returns the X value of the series at the specified point index.
		/// </summary>
		/// <param name="xIndex">The index value of the point.</param>
		/// <returns>X value.</returns>
		/// <internalonly/>
		public double GetX( int xIndex )
		{
			return xIndex;
		}
		/// <summary>
		/// Returns the Y value of the series at the specified point index.
		/// </summary>
		/// <param name="xIndex">The index value of the point.</param>
		/// <returns>Y value.</returns>
		/// <internalonly/>
		public double[] GetY( int xIndex )
		{
			return m_indexedModel.GetY(xIndex);
		}
		/// <summary>
		/// Indicates whether a plottable value is present at the specified point index.
		/// </summary>
		/// <param name="xIndex">The index value of the point.</param>
		/// <returns>
		/// True, if there is a value present at this point index; false otherwise.
		/// </returns>
		/// <internalonly/>
		public bool GetEmpty( int xIndex )
		{
			return m_indexedModel.GetEmpty(xIndex);
		}
		#endregion
	}
}
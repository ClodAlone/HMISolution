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
using Syncfusion.Documentation;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Chart
{

	/// <summary>
	/// Delegate that is to be used with the <see cref="ChartSeriesCollection.Changed"/> event.
	/// </summary>
	/// <param name="sender" type="object">
	///     <para>
	///     Sender.
	///     </para>
	/// </param>
	/// <param name="e" type="Syncfusion.Windows.Forms.Chart.ChartSeriesCollectionChangedEventArgs">
	///     <para>
	///     Argument.
	///     </para>
	/// </param>
	/// <remarks>
	///
	/// </remarks>
	public delegate void ChartSeriesCollectionChangedEventHandler(object sender, ChartSeriesCollectionChangedEventArgs e);

	/// <summary>
	///     The type of change that had occurred to the Chart series collection.
	/// </summary>
	public enum ChartSeriesCollectionChangeType
	{
		/// <summary>
		/// Series has been added to the collection.
		/// </summary>
		Added,
		/// <summary>
		/// Series has been inserted into the collection.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		Inserted = Added,
		/// <summary>
		/// Series has been removed from the collection.
		/// </summary>
		Removed,
		/// <summary>
		/// Series in the collection has been changed.
		/// </summary>
		Changed,
		/// <summary>
		/// The collection has been reset.
		/// </summary>
		Reset
	}

	/// <summary>
	///    Argument that is to be used with the <see cref="ChartSeriesCollection.Changed"/> event.
	/// </summary>
	public class ChartSeriesCollectionChangedEventArgs : EventArgs
	{
		#region Members
		private readonly ChartSeriesCollectionChangeType m_changeType;
		private readonly ChartSeries m_series = null;
		#endregion

		#region Properties
		/// <summary>
		///    Returns the type of change that had occurred in the collection.
		/// </summary>
		public ChartSeriesCollectionChangeType ChangeType
		{
			get
			{
				return m_changeType;
			}
		}
		/// <summary>
		/// Gets the series.
		/// </summary>
		/// <value>The series.</value>
		public ChartSeries Series
		{
			get { return m_series; }
		}
		#endregion

		#region Constructor
		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="changeType" type="Syncfusion.Windows.Forms.Chart.ChartSeriesCollectionChangeType">
		///     <para>
		///     The type of change that had occurred in the collection.
		///     </para>
		/// </param>
		public ChartSeriesCollectionChangedEventArgs(ChartSeriesCollectionChangeType changeType)
		{
			m_changeType = changeType;
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="changeType">The type of change that had occurred in the collection.</param>
		/// <param name="series">The series.</param>
		public ChartSeriesCollectionChangedEventArgs(ChartSeriesCollectionChangeType changeType, ChartSeries series)
		{
			m_changeType = changeType;
			m_series = series;
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Creates the Added event arguments.
		/// </summary>
		/// <param name="series">The series.</param>
		/// <returns></returns>
		/// <internalonly/>
		[DocumentationExclude()]
		public static ChartSeriesCollectionChangedEventArgs CreateAddedEvent(ChartSeries series)
		{
			return new ChartSeriesCollectionChangedEventArgs(ChartSeriesCollectionChangeType.Added, series);
		}
		/// <summary>
		/// Creates the Changed event arguments.
		/// </summary>
		/// <param name="series">The series.</param>
		/// <returns></returns>
		/// <internalonly/>
		[DocumentationExclude()]
		public static ChartSeriesCollectionChangedEventArgs CreateChangedEvent(ChartSeries series)
		{
			return new ChartSeriesCollectionChangedEventArgs(ChartSeriesCollectionChangeType.Changed);
		}
		/// <summary>
		/// Creates the Removed event arguments.
		/// </summary>
		/// <param name="series">The series.</param>
		/// <returns></returns>		
		/// <internalonly/>
		[DocumentationExclude()]
		public static ChartSeriesCollectionChangedEventArgs CreateRemovedEvent(ChartSeries series)
		{
			return new ChartSeriesCollectionChangedEventArgs(ChartSeriesCollectionChangeType.Removed, series);
		}
		/// <summary>
		/// Creates the Reset event arguments.
		/// </summary>		
        /// <returns>The new ChartSeriesCollectionChangedEventArgs instance</returns>
		/// <internalonly/>
		[DocumentationExclude()]
		public static ChartSeriesCollectionChangedEventArgs CreateResetEvent()
		{
			return new ChartSeriesCollectionChangedEventArgs(ChartSeriesCollectionChangeType.Reset);
		}
		#endregion
	}

	/// <summary>
	/// Exposes a method that compares two <see cref="ChartSeries"/> by Y values.
	/// </summary>
	/// <internalonly/>
	[Syncfusion.Documentation.DocumentationExclude()]
	public class ComparerByZandY : IComparer
	{
		#region IComparer Members
		/// <summary>
		/// Compares the two <see cref="ChartSeries"/>.
		/// </summary>
		/// <param name="x">The first <see cref="ChartSeries"/> to compare.</param>
		/// <param name="y">The second <see cref="ChartSeries"/> to compare.</param>
		/// <returns></returns>
		int System.Collections.IComparer.Compare(object x, object y)
		{
			ChartSeries s1 = (ChartSeries)x;
			ChartSeries s2 = (ChartSeries)y;

			if ((s1.Points.Count == 0) && (s2.Points.Count == 0))
			{
				return 0;
			}
			else if (s1.Points.Count == 0)
			{
				return -1;
			}
			else if (s2.Points.Count == 0)
			{
				return 1;
			}

			double[] tmpYs;

			double s1AbsMax = 0;
			double s2AbsMax = 0;

			for (int i = 0; i < s1.Points.Count; i++)
			{
				tmpYs = s1.Points[i].YValues;

				for (int j = 0, lj = tmpYs.Length; j < lj; j++)
				{
					s1AbsMax = Math.Max(s1AbsMax, Math.Abs(tmpYs[j]));
				}
			}

			for (int i = 0; i < s2.Points.Count; i++)
			{
				tmpYs = s2.Points[i].YValues;

				for (int j = 0, lj = tmpYs.Length; j < lj; j++)
				{
					s2AbsMax = Math.Max(s2AbsMax, Math.Abs(tmpYs[j]));
				}
			}

			if (s1AbsMax < s2AbsMax)
			{
				return -1;
			}
			else if (s1AbsMax > s2AbsMax)
			{
				return 1;
			}

			return 0;
		}
		#endregion
	}

	/// <summary>
	/// Exposes a method that compares two <see cref="ChartSeries"/> by <see cref="ChartSeries.ZOrder"/>.
	/// </summary>
	sealed class ChartSeriesComparerByZOrder : IComparer
	{
		#region IComparer Members
		/// <summary>
		/// Compares two objects.
		/// </summary>
		/// <param name="x">The first <see cref="ChartSeries"/> to compare.</param>
		/// <param name="y">The second <see cref="ChartSeries"/> to compare.</param>
		/// <returns></returns>
		public int Compare(object x, object y)
		{
			ChartSeries s1 = (ChartSeries)x;
			ChartSeries s2 = (ChartSeries)y;

			if (s1.ZOrder > s2.ZOrder)
			{
				return 1;
			}
			else if (s1.ZOrder < s2.ZOrder)
			{
				return -1;
			}

			return 0;
		}
		#endregion
	}

	/// <summary>
	/// <see cref="CollectionBase"/> derived class that holds instances of <see cref="ChartSeries"/>.
	/// </summary>
	[TypeConverter(typeof(CollectionConverter))]
	public sealed class ChartSeriesCollection : CollectionBase
	{
		#region Constants
		private const string c_seriesNameFormat = "Series{0}";
		#endregion

		#region Members
		private ArrayList m_visibleList = new ArrayList();
		private ChartPlace[] t_chartPlace = null;
		private ChartModel m_chartModel;

		private bool m_shouldSort = false;
		private bool m_sorted = false;
		private int m_updateCount = 0;
		private bool m_needUpdateVisibleList = false;
        private bool m_disableStyles = false;
		#endregion

		#region Events
		/// <summary>
		/// Event that will be raised when this collection is changed.
		/// </summary>
		public event ChartSeriesCollectionChangedEventHandler Changed;
		#endregion

		#region Properties
		/// <summary>
		/// Indicates whether the series in this collection should be sorted.
		/// </summary>
		public bool ShouldSort
		{
			get
			{
				return m_shouldSort;
			}
			set
			{
				m_shouldSort = value;
				m_sorted = false;
			}
		}
        /// <summary>
        /// Indicates whether the Series's EnableStyles Enable or Not.
        /// </summary>
        public bool DisableStyles 
        {
            get
            {
                return m_disableStyles;
            }
            set
            {
                m_disableStyles = value;
            }
        }
		/// <summary>
		/// Indicates whether this <see cref="ChartSeriesCollection"/> is sorted.
		/// </summary>
		public bool Sorted
		{
			get { return m_sorted; }
		}

		/// <summary>
		/// Overloaded. Returns the <see cref="ChartSeries"/> object stored at the specified index.
		/// </summary>
		public ChartSeries this[int index]
		{
			get
			{
				return this.List[index] as ChartSeries;
			}
		}
		/// <summary>
		/// Returns the <see cref="ChartSeries"/> object stored with the specified name.
		/// </summary>
		public ChartSeries this[string name]
		{
			get
			{
				foreach (ChartSeries series in this.List)
				{
					if (series.Name == name)
					{
						return series;
					}
				}

				return null;
			}
		}

		/// <summary>
		/// Returns the number of visible series in the collection
		/// </summary>
		public int VisibleCount
		{
			get
			{
				return VisibleList.Count;
			}
		}
		/// <summary>
		/// Returns <see cref="IList"/> sorted/unsorted collection <see cref="ChartSeries"/> objects.
		/// </summary>
		internal IList VisibleList
		{
			get
			{
				if (m_needUpdateVisibleList)
				{
					RefreshVisibleList();
					m_needUpdateVisibleList = false;
				}
				return m_visibleList;
			}
		}
		/// <summary>
		/// Gets a value indicating whether should update collecation.
		/// </summary>
		/// <value><c>true</c> if should update; otherwise, <c>false</c>.</value>
		private bool ShouldUpdate
		{
			get
			{
				return m_updateCount == 0;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="chartModel" type="Syncfusion.Windows.Forms.Chart.ChartModel">
		///     <para>
		///      Chart model associated with this collection.
		///     </para>
		/// </param>
		public ChartSeriesCollection(ChartModel chartModel)
		{
			m_chartModel = chartModel;
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Call this method if you perform multiple changes in quick succession.
		/// </summary>
		/// <seealso cref="ChartSeriesCollection.EndUpdate"/>
		/// <internalonly/>
		[DocumentationExclude()]
		public void BeginUpdate()
		{
			m_updateCount++;
		}
		/// <summary>
		/// Call this method if you called <see cref="ChartSeriesCollection.BeginUpdate"/> earlier and you are done with your changes.
		/// </summary>
		/// <seealso cref="ChartSeriesCollection.BeginUpdate"/>
		/// <internalonly/>
		[DocumentationExclude()]
		public void EndUpdate()
		{
			m_updateCount--;

			if (m_updateCount == 0)
			{
				this.RaiseResetEvent();
			}
		}

		/// <summary>
		/// Adds the specified <see cref="ChartSeries"/> into this collection.
		/// </summary>
		/// <param name="series" type="Syncfusion.Windows.Forms.Chart.ChartSeries">
		///     <para>
		/// <see cref="ChartSeries"/> An instance of the Chartseries that is to be added to this collection.
		///     </para>
		/// </param>
		public void Add(ChartSeries series)
		{
			if (!this.List.Contains(series))
			{
				this.List.Add(series);
			}
		}
		/// <summary>
		/// Call this method to retrieve the index value of the specified <see cref="ChartSeries"/>.
		/// </summary>
		/// <param name="series" type="Syncfusion.Windows.Forms.Chart.ChartSeries">
		///     <para>
		///		An instance of the <see cref="ChartSeries"/> that is to be located.
		///     </para>
		/// </param>
		/// <returns>
		///     The index value of specified <see cref="ChartSeries"/>.
		/// </returns>
		public int IndexOf(ChartSeries series)
		{
			return this.List.IndexOf(series);
		}
		/// <summary>
		/// Determines whether the collection contains a specific value.
		/// </summary>
		/// <param name="series">The series.</param>
		/// <returns></returns>
		public bool Contains(ChartSeries series)
		{
			return this.List.Contains(series);
		}
		/// <summary>
		/// Inserts the specified <see cref="ChartSeries"/> to this collection at the specified index.
		/// </summary>
		/// <param name="index" type="int">
		///     <para>
		/// Index value where the insert is to be made.
		///     </para>
		/// </param>
		/// <param name="series" type="Syncfusion.Windows.Forms.Chart.ChartSeries">
		///     <para>
		/// An instance of the <see cref="ChartSeries"/> that is to be inserted into this collection.
		///     </para>
		/// </param>
		public void Insert(int index, ChartSeries series)
		{
			if (!this.List.Contains(series))
			{
				this.List.Insert(index, series);
			}
		}
		/// <summary>
		/// Removes the specified <see cref="ChartSeries"/> from this collection.
		/// </summary>
		/// <param name="series" type="Syncfusion.Windows.Forms.Chart.ChartSeries">
		///     <para>
		/// <see cref="ChartSeries"/> that is to be removed from this collection.
		///     </para>
		/// </param>
		public void Remove(ChartSeries series)
		{
			int index = this.List.IndexOf(series);

			if (index >= 0)
			{
				this.List.RemoveAt(index);
			}
		}
		/// <summary>
		/// Call this method to remove any temporarily cached style instances. You do not normally have to call this method.
		/// </summary>
		public void ResetCache()
		{
			foreach (ChartSeries series in this)
			{
				ResetCache(series);
			}
		}
		/// <summary>
		/// Call this method to remove any temporarily cached style instances. You do not normally have to call this method.
		/// </summary>
		public void ResetCache(ChartSeries series)
		{
            if (series.ResetStyles)
            {
                series.StylesImpl.ComposedStyles.ResetCache();  //issuefixed line
            }

            //series.StylesImpl.ComposedStyles.ResetCache(); 
			series.ResetLegend();
			series.UpdateRenderer(ChartUpdateFlags.All);
		}
		/// <summary>
		/// Sorts by the specified comparer.
		/// </summary>
		/// <param name="comparer">The <see cref="IComparer"/>. Only <see cref="ComparerByZandY"/> is supported.</param>
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void Sort(IComparer comparer)
		{
			m_sorted = false;

			if (comparer is ComparerByZandY)
			{
				try
				{
					for (int i = 0; i < List.Count - 1; i++)
					{
						for (int j = i + 1; j < List.Count; j++)
						{
							if (0 > comparer.Compare(List[j], List[i]))
							{
								object aux = List[j];
								object aux2 = List[i];
								((ChartSeries)List[i]).ZOrder = j;
								List[j] = List[i];
								((ChartSeries)aux).ZOrder = i;
								List[i] = aux;
								List[j] = aux2;
							}
						}
					}
				}
				catch (NotSupportedException)
				{
				}

				m_sorted = true;
			}
			else
			{
				throw new NotSupportedException("Only ComparerByZandY is supported.");
			}
		}
		#endregion

		#region Implementation

		#region Collection implementation
		/// <summary>
		/// Performs additional custom processes when clearing the contents of the <see cref="T:System.Collections.CollectionBase"></see> instance.
		/// </summary>
		/// <internalonly/>
		[DocumentationExclude()]
		protected override void OnClear()
		{
			for (int i = this.List.Count - 1; i >= 0; i--)
			{
				this.UnwireSeries(this.List[i] as ChartSeries);
			}
		}
		/// <summary>
		/// Performs additional custom processes after clearing the contents of the <see cref="T:System.Collections.CollectionBase"></see> instance.
		/// </summary>
		/// <internalonly/>
		[DocumentationExclude()]
		protected override void OnClearComplete()
		{
			this.RaiseCollectionChangedEventHandler(ChartSeriesCollectionChangedEventArgs.CreateResetEvent());
		}
		/// <summary>
		/// Performs additional custom processes before inserting a new element into the <see cref="T:System.Collections.CollectionBase"></see> instance.
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsert(int index, object value)
		{
			ChartSeries series = value as ChartSeries;

			// Name check
			//if (series.Name == "")
			//{
			//  int lastIntex = this.Count - 1;
			//  string newName = string.Format(c_seriesNameFormat, lastIntex);

			//  while (this[newName] != null)
			//  {
			//    lastIntex++;
			//    newName = string.Format(c_seriesNameFormat, lastIntex);
			//  }

			//  series.Name = newName;
			//}

			//if (this[series.Name] != null)
			//  throw new ArgumentException(string.Format("A series with {0} name is already added.", series.Name));

			if( series.ZOrder == -1 )
				series.ZOrder = this.Count > 0 ? this[this.Count - 1].ZOrder + 1 : 0;

			base.OnInsert(index, value);
		}
		/// <summary>
		/// Performs additional custom processes after inserting a new element into the <see cref="T:System.Collections.CollectionBase"></see> instance.
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		/// <internalonly/>
		[DocumentationExclude()]
		protected override void OnInsertComplete(int index, object value)
		{
			m_sorted = false;

			ChartSeries series = value as ChartSeries;

			this.WireSeries(series);
			this.RaiseCollectionChangedEventHandler(ChartSeriesCollectionChangedEventArgs.CreateAddedEvent(series));
		}
		/// <summary>
		/// Performs additional custom processes after removing an element from the <see cref="T:System.Collections.CollectionBase"></see> instance.
		/// </summary>
		/// <param name="index">The zero-based index at which value can be found.</param>
		/// <param name="value">The value of the element to remove from index.</param>
		/// <internalonly/>
		[DocumentationExclude()]
		protected override void OnRemoveComplete(int index, object value)
		{
			this.ResetCache();
			this.UnwireSeries(value as ChartSeries);
			this.RaiseCollectionChangedEventHandler(ChartSeriesCollectionChangedEventArgs.CreateRemovedEvent(value as ChartSeries));
		}
		/// <summary>
		/// Called when [set complete].
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="newValue">The new value.</param>
		/// <param name="oldValue">The old value.</param>
		/// <internalonly/>
		[DocumentationExclude()]
		protected override void OnSetComplete(int index, object newValue, object oldValue)
		{
			m_sorted = false;

			this.UnwireSeries(oldValue as ChartSeries);
			this.RaiseCollectionChangedEventHandler(ChartSeriesCollectionChangedEventArgs.CreateRemovedEvent(oldValue as ChartSeries));

			this.WireSeries(newValue as ChartSeries);
			this.RaiseCollectionChangedEventHandler(ChartSeriesCollectionChangedEventArgs.CreateAddedEvent(newValue as ChartSeries));
		}
		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		/// <internalonly/>
		[DocumentationExclude()]
		protected override void OnValidate(object value)
		{
			if (!(value is ChartSeries))
				throw new ArgumentException("value should be ChartSeries");
		}
		#endregion

		/// <summary>
		/// Draws series on input <see cref=" System.Drawing.Graphics"/> object
		/// </summary>
		/// <param name="g">Graphics for drawing on.</param>
		/// <param name="chart"><see cref="Syncfusion.Windows.Forms.Chart.IChartAreaHost"/> interface reference</param>
		internal void DrawSeries(Graphics g, IChartAreaHost chart)
		{
			if (!Sorted && this.ShouldSort)
			{
				m_visibleList.Sort(new ComparerByZandY());
			}

			m_needUpdateVisibleList = true;

			RectangleF clip = chart.GetChartArea().RenderBounds;
			foreach (ChartSeries cs in this.List)
			{
				ChartAxis Yaxis = chart.GetChartArea().GetYAxis(cs);
				ChartAxis Xaxis = chart.GetChartArea().GetXAxis(cs);
				cs.Renderer.SetBoundsAndRange(chart, clip, Xaxis, Yaxis);
			}

			RenderViewer viewer = new RenderViewer();
			ChartPlace[] res = ChartPlace.CalculateSpace(this, chart.GetChartArea().DivideArea, chart);

			viewer.AxisInverted = chart.RequireInvertedAxes;
			viewer.NeedRegionUpdate = chart.NeedRegionUpdate;
			viewer.Regions = chart.ChartRegions;

			for (int i = res.Length - 1; i >= 0; i--)
			{
				ChartPlace plc = res[i];

				for (int si = plc.Series.Count - 1; si > -1; si--)
				{
					ChartSeries cs = plc.Series[si] as ChartSeries;

					if (cs.Compatible)
					{
						RectangleF bounds = clip;

						if (chart.GetChartArea().DivideArea && cs.BaseType == ChartSeriesBaseType.Single)
						{
							bounds = RenderingHelper.GetBounds(this.VisibleList.IndexOf(cs), plc.Series.Count, clip);
						}

						ChartAxis Yaxis = chart.GetChartArea().GetYAxis(cs);
						ChartAxis Xaxis = chart.GetChartArea().GetXAxis(cs);
						cs.Renderer.SetBoundsAndRange(chart, bounds, Xaxis, Yaxis);

						GraphicsState gs = g.Save();
						// here some property check may be added, to check whether clip or not
						//g.IntersectClip( new Region(clip2) );
						cs.Renderer.Render(g);
						g.Restore(gs);

						cs.Renderer.RenderSeriesNameInDepth(g);
						viewer.Add(cs.Renderer);
					}
				}

				viewer.View(g);
				viewer.Clear();
			}

			for (int i = res.Length - 1; i >= 0; i--)
			{
				ChartPlace plc = res[i];

				foreach (ChartSeries cs in plc.Series)
				{
					if (cs.Compatible)
					{
						//if( cs.EnableStyles )
						{
							cs.Renderer.RenderAdornments(g);
						}
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="chart"></param>
		internal void DrawSeries(Graphics3D g, IChartAreaHost chart)
		{
			try
			{
				if (t_chartPlace == null)
					t_chartPlace = SetClip(g, chart);

				for (int i = t_chartPlace.Length - 1; i >= 0; i--)
				{
					ChartPlace plc = t_chartPlace[i];

					for (int si = 0; si < plc.Series.Count; si++)
					{
						ChartSeries cs = plc.Series[si] as ChartSeries;

						if (cs.Compatible)
						{
							cs.Renderer.Render(g);
							cs.Renderer.RenderAdornments(g);
						}
					}
				}
			}
			finally
			{
				t_chartPlace = null;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="chart"></param>
		internal void DrawSeriesNamesInDepth(Graphics3D g, IChartAreaHost chart)
		{
			t_chartPlace = SetClip(g, chart);

			for (int i = t_chartPlace.Length - 1; i >= 0; i--)
			{
				ChartPlace plc = t_chartPlace[i];

				for (int si = 0; si < plc.Series.Count; si++)
				{
					ChartSeries cs = plc.Series[si] as ChartSeries;

					if (cs.Compatible)
					{
						cs.Renderer.RenderSeriesNameInDepth(g);
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="chart"></param>
		internal ChartPlace[] SetClip(Graphics3D g, IChartAreaHost chart)
		{
			if (!Sorted && this.ShouldSort)
			{
				Sort(new ComparerByZandY());
			}

			ChartPlace[] res = ChartPlace.CalculateSpace(this, chart.GetChartArea().DivideArea, chart);

			for (int i = res.Length - 1; i >= 0; i--)
			{
				ChartPlace plc = res[i];

				for (int si = 0; si < plc.Series.Count; si++)
				{
					ChartSeries cs = plc.Series[si] as ChartSeries;

					if (cs.Compatible)
					{
						RectangleF clip = chart.GetChartArea().RenderBounds;
						ChartAxis Yaxis = chart.GetChartArea().GetYAxis(cs);
						ChartAxis Xaxis = chart.GetChartArea().GetXAxis(cs);

						if (chart.GetChartArea().DivideArea && cs.BaseType == ChartSeriesBaseType.Single)
						{
							clip = RenderingHelper.GetBounds(this.VisibleList.IndexOf(cs), plc.Series.Count, clip);
						}

						cs.Renderer.SetBoundsAndRange(chart, clip, Xaxis, Yaxis);
					}
				}
			}

			return res;
		}

		/// <summary>
		/// Cals methods, assigned on collectionChangedEventHandler delegate. 
		/// </summary>
		/// <param name="e">Event arguments.</param>
		private void RaiseCollectionChangedEventHandler(ChartSeriesCollectionChangedEventArgs e)
		{
			m_needUpdateVisibleList = true;

			if (this.ShouldUpdate)
			{
				if (this.Changed != null)
				{
					this.Changed(this, e);
				}
			}
		}

		/// <internalonly/>
		[DocumentationExclude()]
		private void OnSeriesChanged(object sender, EventArgs args)
		{
			m_needUpdateVisibleList = true;
			this.RaiseCollectionChangedEventHandler(ChartSeriesCollectionChangedEventArgs.CreateChangedEvent( sender as ChartSeries ));
		}
		/// <internalonly/>
		[DocumentationExclude()]
		private void OnSeriesDataChanged(object sender, ListChangedEventArgs args)
		{
			if (args.ListChangedType != ListChangedType.ItemAdded)
			{
				this.ResetCache(sender as ChartSeries);
			}

			this.RaiseCollectionChangedEventHandler(ChartSeriesCollectionChangedEventArgs.CreateChangedEvent(sender as ChartSeries));
		}

		/// <internalonly/>
		[DocumentationExclude()]
		internal void RaiseResetEvent()
		{
			RaiseCollectionChangedEventHandler(ChartSeriesCollectionChangedEventArgs.CreateResetEvent());
		}

		/// <internalonly/>
		[DocumentationExclude()]
		private void UnwireSeries(ChartSeries series)
		{
			series.ChartModel = null;
			series.DataChanged -= new ListChangedEventHandler(this.OnSeriesDataChanged);
			series.SeriesChanged -= new EventHandler(this.OnSeriesChanged);
		}
		/// <internalonly/>
		[DocumentationExclude()]
		private void WireSeries(ChartSeries series)
		{
			series.ChartModel = m_chartModel;
			series.DataChanged += new ListChangedEventHandler(this.OnSeriesDataChanged);
			series.SeriesChanged += new EventHandler(this.OnSeriesChanged);

			this.ResetCache(series);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pointIndex"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		[Obsolete("Internal method"), EditorBrowsable(EditorBrowsableState.Never)]
		public int GetIndexMostSeries(int pointIndex, ChartSeriesType type)
		{
			int result = -1;
			for (int i = Count - 1; i >= 0; i--)
			{
				if ((this[i].Type == type)
					&& (this[i].Visible)
					&& (pointIndex < this[i].Points.Count))
				{
					result = i;
					break;
				}
			}
			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pointIndex"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		[Obsolete("Internal method"), EditorBrowsable(EditorBrowsableState.Never)]
		public int GetIndexLowerSeries(int pointIndex, ChartSeriesType type)
		{
			int result = -1;
			for (int i = 0; i < Count; i++)
			{
				if ((this[i].Type == type)
					&& (this[i].Visible)
					&& (pointIndex < this[i].Points.Count))
				{
					result = i;
					break;
				}
			}
			return result;
		}
		/// <summary>
		/// Returns the maximum number of points in the series collection
		/// </summary>
		/// <returns></returns>
		[Obsolete("Internal method"), EditorBrowsable(EditorBrowsableState.Never)]
		public int GetMaxCountPoint()
		{
			int result = 0;

			for (int i = 0; i < Count; i++)
			{
				result = Math.Max(result, this[i].Points.Count);
			}

			return result;
		}
		/// <summary>
		/// Returns the index of the visible series
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		[Obsolete("Internal method"), EditorBrowsable(EditorBrowsableState.Never)]
		public int VisibleByIndex(int index)
		{
			return VisibleSeriesIndexOf(this[index]);
		}
		/// <summary>
		/// Returns the visible series with the specified index 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ChartSeries GetSeriesByVisible(int index)
		{
			return VisibleList[index] as ChartSeries;
		}
		/// <summary>
		/// Returns the index of the series if visible otherwise returns -1
		/// </summary>
		/// <param name="series"></param>
		/// <returns></returns>
		[Obsolete("Internal method"), EditorBrowsable(EditorBrowsableState.Never)]
		public int VisibleSeriesIndexOf(ChartSeries series)
		{
			return VisibleList.IndexOf(series);
		}
		/// <summary>
		/// Recalculates list of visible series, when some of the series are changed.
		/// </summary>
		private void RefreshVisibleList()
		{
			m_visibleList.Clear();

			for (int i = 0; i < Count; i++)
			{
				if (this[i].Visible && this[i].Compatible)
				{
					m_visibleList.Add(this[i]);
				}
			}

			if (!m_shouldSort)
			{
				m_visibleList.Sort(new ChartSeriesComparerByZOrder());
			}
		}
		#endregion
	}
}

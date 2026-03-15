#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.CodeDom;
using System.Drawing.Design;
using System.Collections.Generic;

namespace Syncfusion.Windows.Forms.Chart
{
	#region class ChartPointIndexerEditor
	/// <summary>
	/// Provides the GUI editor of <see cref="ChartPointIndexer"/> instance.
	/// </summary>
	class ChartPointIndexerEditor : CollectionEditor
	{
		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartPointIndexerEditor"/> class.
		/// </summary>
		public ChartPointIndexerEditor()
			: base(typeof(ChartPointIndexer))
		{
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Gets an array of objects containing the specified collection.
		/// </summary>
		/// <param name="editValue">The collection to edit.</param>
		/// <returns>
		/// An array containing the collection objects, or an empty object array if the specified collection does not inherit from <see cref="T:System.Collections.ICollection"></see>.
		/// </returns>
		protected override object[] GetItems(object editValue)
		{
			ChartPointIndexer pointIndexer = editValue as ChartPointIndexer;
			ChartPoint[] points = new ChartPoint[pointIndexer.Count];

			for (int i = 0; i < pointIndexer.Count; i++)
			{
				ChartPoint pt = pointIndexer[i];
				points[i] = new ChartPoint(pt.X, pt.YValues);
			}

			return points;
		}
		/// <summary>
		/// Sets the specified array as the items of the collection.
		/// </summary>
		/// <param name="editValue">The collection to edit.</param>
		/// <param name="value">An array of objects to set as the collection items.</param>
		/// <returns>
		/// The newly created collection object or, otherwise, the collection indicated by the editValue parameter.
		/// </returns>
		protected override object SetItems(object editValue, object[] value)
		{
			ChartPointIndexer pointIndexer = editValue as ChartPointIndexer;

			pointIndexer.Clear();

			foreach (ChartPoint point in value)
			{
				pointIndexer.Add(point);
			}

			return pointIndexer;
		}
		/// <summary>
		/// Gets the data types that this collection editor can contain.
		/// </summary>
		/// <returns>
		/// An array of data types that this collection can contain.
		/// </returns>
		protected override Type[] CreateNewItemTypes()
		{
			return new Type[] { typeof(ChartPoint) };
		}
		#endregion
	}
	#endregion

	#region class ChartPointIndexerCodeDomSerializer
	/// <summary>
	/// Serializes the <see cref="ChartPointIndexer"/>.
	/// </summary>
	class ChartPointIndexerCodeDomSerializer : CodeDomSerializer
	{
		#region Impelmentation
		/// <summary>
		/// Serializes the specified object into a CodeDOM object.
		/// </summary>
		/// <param name="manager">The serialization manager to use during serialization.</param>
		/// <param name="value">The object to serialize.</param>
		/// <returns>
		/// A CodeDOM object representing the object that has been serialized.
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">manager or value is null.</exception>
		public override object Serialize(IDesignerSerializationManager manager, object value)
		{
			ChartPointIndexer pointIndexer = value as ChartPointIndexer;
			CodeStatementCollection statements = new CodeStatementCollection();

#if SyncfusionFramework2_0
			CodeExpression targetExpression = this.GetExpression(manager, value);
#else
			CodeExpression targetExpression = this.SerializeToReferenceExpression(manager, value);
#endif

			foreach (ChartPoint point in pointIndexer)
			{
				CodeMethodInvokeExpression methodInvokeExpression = new CodeMethodInvokeExpression();

				methodInvokeExpression.Method = new CodeMethodReferenceExpression(targetExpression, "Add");
				methodInvokeExpression.Parameters.Add(this.SerializeToExpression(manager, point.X));
				//methodInvokeExpression.Parameters.Add(this.SerializeToExpression(manager, point.YVlues));

				foreach (double yValue in point.YValues)
				{
					CodeExpression valueExpression = this.SerializeToExpression(manager, yValue);
					methodInvokeExpression.Parameters.Add(new CodeCastExpression(typeof(double), valueExpression));
				}

				statements.Add(methodInvokeExpression);
			}

			return statements;
		}
		/// <summary>
		/// Deserializes the specified serialized CodeDOM object into an object.
		/// </summary>
		/// <param name="manager">A serialization manager interface that is used during the deserialization process.</param>
		/// <param name="codeObject">A serialized CodeDOM object to deserialize.</param>
		/// <returns>The deserialized CodeDOM object.</returns>
		/// <exception cref="T:System.ArgumentNullException">manager or codeObject is null.</exception>
		/// <exception cref="T:System.ArgumentException">codeObject is an unsupported code element.</exception>
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
		{
			return null;
		}
		#endregion
	}
	#endregion

	#region class ChartPointIndexer
	/// <summary>
	/// Represents the wrapper for <see cref="IChartSeriesModel"/> that implements the <see cref="IList"/>.
	/// </summary>
	/// <internalonly/>
	[Syncfusion.Documentation.DocumentationExclude()]
	[DesignerSerializer(typeof(ChartPointIndexerCodeDomSerializer), typeof(CodeDomSerializer))]
	public class ChartPointIndexer : IList
	{
		#region class ChartPointEnumerator
		/// <summary>
		/// Represents the enumerator for <see cref="ChartPointIndexer"/>.
		/// </summary>
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		class ChartPointEnumerator : IEnumerator
		{
			#region Members
			private int m_currIndex = -1;
			private IChartSeriesModel m_seriesModel = null;
			#endregion

			#region Properties
			/// <summary>
			/// Gets the current element in the collection.
			/// </summary>
			public object Current
			{
				get
				{
					return new ChartPoint(m_seriesModel, m_currIndex);
				}
			}
			#endregion

			#region Constructor
			/// <summary>
			/// Initializes a new instance of the ChartPointEnumerator class. 
			/// </summary>
			/// <param name="chartSeriesModel">Instance of IChartSeriesModel.</param>
			public ChartPointEnumerator(IChartSeriesModel chartSeriesModel)
			{
				m_seriesModel = chartSeriesModel;
			}
			#endregion

			#region Public methods
			/// <summary>
			/// Advances the enumerator to the next element of the collection.  
			/// </summary>
			/// <returns></returns>
			public bool MoveNext()
			{
				m_currIndex++;

				return m_currIndex < m_seriesModel.Count;
			}
			/// <summary>
			/// Sets the enumerator to its initial position, which is before the first element in the collection. 
			/// </summary>
			public void Reset()
			{
				m_currIndex = -1;
			}
			#endregion
		}
		#endregion

		#region Members
		private IChartSeriesModel m_model = null;
		private IEditableChartSeriesModel m_editableModel = null;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the series model.
		/// </summary>
		/// <value>The series model.</value>
		internal IChartSeriesModel SeriesModel
		{
			get
			{
				return m_model;
			}
			set
			{
				m_model = value;
				m_editableModel = value as IEditableChartSeriesModel;
			}
		}
		/// <summary>
		/// Gets the <see cref="Syncfusion.Windows.Forms.Chart.ChartPoint"/> with the specified x index.
		/// </summary>
		/// <value></value>
		/// <internalonly/>
		public ChartPoint this[int xIndex]
		{
			get
			{
				return new ChartPoint(m_model, xIndex);
			}
		}
		/// <summary>
		/// Gets count of points int the series.
		/// </summary>
		public int Count
		{
			get
			{
				return m_model.Count;
			}
		}
		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IList"></see> is read-only.
		/// </summary>
		/// <value></value>
		/// <returns>true if the <see cref="T:System.Collections.IList"></see> is read-only; otherwise, false.</returns>
		public bool IsReadOnly
		{
			get
			{
				return m_editableModel == null;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartPointIndexer"/> class.
		/// </summary>
		/// <param name="series">The series.</param>
		/// <internalonly/>
		public ChartPointIndexer(ChartSeries series)
		{
			this.SeriesModel = series.SeriesModelAdapter;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartPointIndexer"/> class.
		/// </summary>
		/// <param name="model">The model.</param>
		public ChartPointIndexer(IChartSeriesModel model)
		{
			this.SeriesModel = model;
		}
		#endregion

		#region Collection interface
		/// <summary>
		/// Adds a point to the series
		/// </summary>
		/// <param name="x">X value of point</param>
		/// <param name="yValues">Y values of point</param>
		/// <param name="isEmpty">if set to <c>true</c> points is empty.</param>
		/// <returns></returns>
		private int Add(double x, double[] yValues, bool isEmpty)
		{
			if (m_editableModel != null)
			{
				int index = m_editableModel.Count;
				m_editableModel.Add(x, yValues, isEmpty);
				return index;
			}

			return -1;
		}
		/// <summary>
		/// Adds a point to the series
		/// </summary>
		/// <param name="x">X value of point</param>
		/// <param name="yValues">Y values of point</param>
		public int Add(double x, params double[] yValues)
		{
			if (m_editableModel != null)
			{
				int index = m_editableModel.Count;
				m_editableModel.Add(x, yValues, false);
				return index;
			}

			return -1;
		}
		/// <summary>
		/// Adds a point to the series
		/// </summary>
		/// <param name="x">X value of point</param>
		/// <param name="y">Y value of point</param>
		public int Add(double x, double y)
		{
			return this.Add(x, new double[] { y });
		}
		/// <summary>
		/// Adds a point to the series
		/// </summary>
		/// <param name="x">X value of point</param>
		/// <param name="dates">Y dates of point</param>
		public int Add(double x, params DateTime[] dates)
		{
			double[] yValues = new double[dates.Length];

			for (int i = 0; i < yValues.Length; i++)
			{
				yValues[i] = dates[i].ToOADate();
			}

			return this.Add(x, yValues);
		}
		/// <summary>
		/// Adds a point to the series
		/// </summary>
		/// <param name="x">X value of point</param>
		/// <param name="date">Y date of point</param>
		public int Add(double x, DateTime date)
		{
			return this.Add(x, date.ToOADate());
		}
		/// <summary>
		/// Adds a point to the series
		/// </summary>
		/// <param name="cp">Instance of ChartPoint</param>
		public int Add(ChartPoint cp)
		{
			return this.Add(cp.X, cp.YValues, cp.IsEmpty);
		}
		/// <summary>
		/// Adds a point to the series
		/// </summary>
		/// <param name="date">X date of point</param>
		/// <param name="yValues">Y values of point</param>
		public int Add(DateTime date, params double[] yValues)
		{
			return this.Add(date.ToOADate(), yValues);
		}
		/// <summary>
		/// Adds a point to the series
		/// </summary>
		/// <param name="date">X date of point</param>
		/// <param name="y">Y value of point</param>
		public int Add(DateTime date, double y)
		{
			return this.Add(date.ToOADate(), y);
		}
		/// <summary>
		/// Removes all points from the series.
		/// </summary>
		public void Clear()
		{
			if (m_editableModel != null)
			{
				m_editableModel.Clear();
			}
		}
		/// <summary>
		/// Inserts a point to the series at the specified index. 
		/// </summary>
		/// <param name="xIndex">Index of a point</param>
		/// <param name="cp">Instance of ChartPoint</param>
		public void Insert(int xIndex, ChartPoint cp)
		{
			if (m_editableModel != null)
			{
				m_editableModel.Insert(xIndex, cp.X, cp.YValues);
			}
		}
		/// <summary>
		/// Removes the specified <see cref="ChartPoint"/>.
		/// </summary>
		/// <param name="cp">The cp.</param>
		public void Remove(ChartPoint cp)
		{
			this.RemoveAt(this.IndexOf(cp));
		}
		/// <summary>
		/// Removes a point from the series at the specified index. 
		/// </summary>
		/// <param name="xIndex">Index of a point</param>
		[Obsolete("Use RemoveAt method.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Remove(int xIndex)
		{
			this.RemoveAt(xIndex);
		}
		/// <summary>
		/// Removes the <see cref="T:System.Collections.IList"></see> item at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.IList"></see>. </exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
		public void RemoveAt(int index)
		{
			if (m_editableModel != null)
			{
				m_editableModel.Remove(index);
			}
		}
		/// <summary>
		/// Returns index of a point.
		/// </summary>
		/// <param name="cp">Instance of ChartPoint</param>
		/// <returns>The specified index of point.</returns>
		public int IndexOf(ChartPoint cp)
		{
			int res = -1;

			for (int i = 0, end = Count; i < end; i++)
			{				
				if (m_model.GetX(i) == cp.X)
				{
					double[] yvalues1 = m_model.GetY(i);
					double[] yvalues2 = cp.YValues;
					if (yvalues1[0].Equals(yvalues2[0]))
					{
						res = i;
						break;
					}
					else if( yvalues1 != null && yvalues2 != null
						&& yvalues1.Length == yvalues2.Length)
					{
						bool equal = true;

						for (int j = 0; j < yvalues1.Length; j++)
						{
							if (yvalues1[j] == yvalues2[j])
							{
								equal = false;
								break;
							}
						}

						if (equal)
						{
							res = i;
							break;
						}
					}
				}
			}

			return res;
		}
		/// <summary>
		/// Returns an enumerator that iterates through a collection.  
		/// </summary>
		/// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
		public IEnumerator GetEnumerator()
		{
			return new ChartPointEnumerator(m_model);
		}
		#endregion

		#region IList Members
		/// <summary>
		/// Adds an item to the <see cref="T:System.Collections.IList"></see>.
		/// </summary>
		/// <param name="value">The <see cref="T:System.Object"></see> to add to the <see cref="T:System.Collections.IList"></see>.</param>
		/// <returns>
		/// The position into which the new element was inserted.
		/// </returns>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
		int IList.Add(object value)
		{
			this.Add(value as ChartPoint);

			return this.Count - 1;
		}
		/// <summary>
		/// Determines whether the <see cref="T:System.Collections.IList"></see> contains a specific value.
		/// </summary>
		/// <param name="value">The <see cref="T:System.Object"></see> to locate in the <see cref="T:System.Collections.IList"></see>.</param>
		/// <returns>
		/// true if the <see cref="T:System.Object"></see> is found in the <see cref="T:System.Collections.IList"></see>; otherwise, false.
		/// </returns>
		bool IList.Contains(object value)
		{
			return this.IndexOf(value as ChartPoint) > -1;
		}

		/// <summary>
		/// Determines the index of a specific item in the <see cref="T:System.Collections.IList"></see>.
		/// </summary>
		/// <param name="value">The <see cref="T:System.Object"></see> to locate in the <see cref="T:System.Collections.IList"></see>.</param>
		/// <returns>
		/// The index of value if found in the list; otherwise, -1.
		/// </returns>
		int IList.IndexOf(object value)
		{
			return this.IndexOf(value as ChartPoint);
		}
		/// <summary>
		/// Inserts an item to the <see cref="T:System.Collections.IList"></see> at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value">The <see cref="T:System.Object"></see> to insert into the <see cref="T:System.Collections.IList"></see>.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.IList"></see>. </exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
		/// <exception cref="T:System.NullReferenceException">value is null reference in the <see cref="T:System.Collections.IList"></see>.</exception>
		void IList.Insert(int index, object value)
		{
			this.Insert(index, value as ChartPoint);
		}
		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IList"></see> has a fixed size.
		/// </summary>
		/// <value></value>
		/// <returns>true if the <see cref="T:System.Collections.IList"></see> has a fixed size; otherwise, false.</returns>
		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"></see>.
		/// </summary>
		/// <param name="value">The <see cref="T:System.Object"></see> to remove from the <see cref="T:System.Collections.IList"></see>.</param>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
		void IList.Remove(object value)
		{
			this.Remove(value as ChartPoint);
		}
		/// <summary>
		/// Gets or sets the <see cref="System.Object"/> at the specified index.
		/// </summary>
		/// <value></value>
		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
			}
		}
		#endregion

		#region ICollection Members
		/// <summary>
		/// Copies the elements of the <see cref="T:System.Collections.ICollection"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">array is null. </exception>
		/// <exception cref="T:System.ArgumentException">The type of the source <see cref="T:System.Collections.ICollection"></see> cannot be cast automatically to the type of the destination array. </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">index is less than zero. </exception>
		/// <exception cref="T:System.ArgumentException">array is multidimensional.-or- index is equal to or greater than the length of array.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"></see> is greater than the available space from index to the end of the destination array. </exception>
		void ICollection.CopyTo(Array array, int index)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		/// <summary>
		/// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe).
		/// </summary>
		/// <value></value>
		/// <returns>true if access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe); otherwise, false.</returns>
		bool ICollection.IsSynchronized
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}
		/// <summary>
		/// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.
		/// </summary>
		/// <value></value>
		/// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.</returns>
		object ICollection.SyncRoot
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}
		#endregion
	}
	#endregion
}

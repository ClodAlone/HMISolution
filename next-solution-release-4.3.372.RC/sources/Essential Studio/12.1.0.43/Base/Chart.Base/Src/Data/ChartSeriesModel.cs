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

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{

	/// <summary>
	/// This is the core data container implementation for a chart. This is a very simple model that stores data in the list inherited
	/// from the CollectionBase. It relies on the events raised by the CollectionBase base class to inform users of the changes that had occurred to the series.
	/// </summary>
	public class ChartSeriesModel : CollectionBase, IEditableChartSeriesModel
	{
		#region Internal types
		/// <summary>
		/// Represents the data item of <see cref="IChartSeriesModel"/>.
		/// </summary>
		/// <internalonly/>
		[DocumentationExclude()]
		protected class SeriesEntity
		{
			#region Members
			internal double[] m_yValues;
			internal double m_x;
			internal bool m_isEmpty;
			#endregion

			#region Properties
			/// <summary>
			/// Gets or sets the X value.
			/// </summary>
			/// <value>The X.</value>
			/// <internalonly/>
			public double X
			{
				get
				{
					return m_x;
				}
				set
				{
					m_x = value;
				}
			}
			/// <summary>
			/// Gets or sets the Y values.
			/// </summary>
			/// <value>The Y.</value>
			/// <internalonly/>
			public double[] Y
			{
				get
				{
					return m_yValues;
				}
				set
				{
					m_yValues = value;
				}
			}
			/// <summary>
			/// Gets or sets a value indicating whether this instance is empty.
			/// </summary>
			/// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
			/// <internalonly/>
			public bool IsEmpty
			{
				get
				{
					return m_isEmpty;
				}
				set
				{
					m_isEmpty = value;
				}
			}
			#endregion

			#region Constructor
			/// <internalonly/>
			public SeriesEntity(double x, double[] yValues)
			{
				m_x = x;
				m_yValues = yValues;
			}
			/// <internalonly/>
			public SeriesEntity(double x, double[] yValues, bool isEmpty)
			{
				m_x = x;
				m_yValues = yValues;
				m_isEmpty = isEmpty;
			}
			/// <internalonly/>
			public SeriesEntity(double x, double y)
				: this(x, new double[] { y })
			{
			}
			#endregion
		}
		#endregion

		#region Events
		/// <summary>
		/// Please refer to <see cref="IChartSeriesModel.Changed"/>.
		/// </summary>
		public event ListChangedEventHandler Changed;
		#endregion

		#region Public methods
		/// <summary>
		/// Please refer to <see cref="IChartSeriesModel.GetX"/>.
		/// </summary>
		public double GetX( int xIndex )
		{
			return (this.List[xIndex] as SeriesEntity).m_x;
		}
		/// <summary>
		/// Please refer to <see cref="IChartSeriesModel.GetY"/>.
		/// </summary>
		public double[] GetY( int xIndex )
		{
			return (this.List[xIndex] as SeriesEntity).m_yValues;
		}
		/// <summary>
		/// Please refer to <see cref="IChartSeriesModel.GetEmpty"/>.
		/// </summary>
		public bool GetEmpty( int xIndex )
		{
			return (this.List[xIndex] as SeriesEntity).m_isEmpty;
		}

		/// <summary>
		/// Adds data to the end of the data representation.
		/// </summary>
		public void Add( double x, double[] yValues )
		{
			this.List.Add( new SeriesEntity( x, yValues ) );
		}
		/// <summary>
		/// Adds data to the end of the data representation.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="yValues">The y values.</param>
		/// <param name="isEmpty">if set to <c>true</c> is empty.</param>
		public void Add(double x, double[] yValues, bool isEmpty)
		{
			this.List.Add(new SeriesEntity(x, yValues, isEmpty));
		}
		/// <summary>
		/// Please refer to <see cref="IEditableChartSeriesModel.Insert"/>.
		/// </summary>
		public void Insert( int xIndex, double x, double[] yValues )
		{
			this.List.Insert( xIndex, new SeriesEntity( x, yValues ) );
		}
		/// <summary>
		/// Please refer to <see cref="IEditableChartSeriesModel.SetX"/>.
		/// </summary>
		public void SetX( int xIndex, double value )
		{
			(this.List[xIndex] as SeriesEntity).m_x = value;
			this.RaiseChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, xIndex));
		}
		/// <summary>
		/// Please refer to <see cref="IEditableChartSeriesModel.SetY"/>.
		/// </summary>
		public void SetY( int xIndex, double[] yValues )
		{
			(this.List[xIndex] as SeriesEntity).m_yValues = yValues;
			this.RaiseChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, xIndex));
		}
		/// <summary>
		/// Please refer to <see cref="IEditableChartSeriesModel.SetEmpty"/>.
		/// </summary>
		public void SetEmpty( int xIndex, bool isEmpty )
		{
			(this.List[xIndex] as SeriesEntity).m_isEmpty = isEmpty;
			this.RaiseChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, xIndex));
		}
		/// <summary>
		/// Please refer to <see cref="IEditableChartSeriesModel.Remove"/>.
		/// </summary>
		public void Remove( int xIndex )
		{
			this.List.RemoveAt( xIndex );
		}
		#endregion

		#region Implementation
		/// <internalonly/>
		[ DocumentationExclude() ]
		protected override void OnClear()
		{
		}
		/// <internalonly/>
		[ DocumentationExclude() ]
		protected override void OnClearComplete()
		{
			this.RaiseChanged( new ListChangedEventArgs( ListChangedType.Reset, -1) );
		}
		/// <internalonly/>
		[ DocumentationExclude() ]
		protected override void OnInsert( int index, object value )
		{
		}
		/// <internalonly/>
		[ DocumentationExclude() ]
		protected override void OnInsertComplete( int index, object value )
		{
			this.RaiseChanged(new ListChangedEventArgs( ListChangedType.ItemAdded, index ));
		}
		/// <internalonly/>
		[ DocumentationExclude() ]
		protected override void OnRemove( int index, object value )
		{
		}
		/// <internalonly/>
		[ DocumentationExclude() ]
		protected override void OnRemoveComplete( int index, object value )
		{
			this.RaiseChanged(new ListChangedEventArgs( ListChangedType.ItemDeleted, index ));
		}
		/// <internalonly/>
		[ DocumentationExclude() ]
		protected override void OnSet( int index, object value, object newValue )
		{
		}
		/// <internalonly/>
		[ DocumentationExclude() ]
		protected override void OnSetComplete( int index, object newValue, object oldValue )
		{
			this.RaiseChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index, index));
		}
		/// <summary>
		/// Raises the Changed event.
		/// </summary>
		/// <param name="args">The <see cref="System.ComponentModel.ListChangedEventArgs"/> instance containing the event data.</param>
		protected void RaiseChanged(ListChangedEventArgs args)
		{
			if (this.Changed != null)
			{
				this.Changed(this, args);
			}
		}
		#endregion
	}
}
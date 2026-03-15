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
using System.Diagnostics;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Collections;

using Syncfusion.Windows.Forms.Chart;
using System.Drawing.Design;

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	/// The default implementation of the IChartSeriesModel.
	/// </summary>
	[TypeConverter(typeof(ChartInstanceConverter))]
	public class ChartDataBindModel : ChartBaseDataBindList, 
		IChartSeriesModel, IChartSeriesIndexedModel
	{
		#region Internal class
		class PointData
		{
			public double X;
			public double[] YValues;
			public bool IsEmpty;
		}
		#endregion

		#region Constants
		private const int m_emptyIndex = -1;
		#endregion

		#region Members
		private PointData[] m_cache = null;

		private string m_xName = null;
		private string[] m_yNames = null;

		private PropertyDescriptor m_xProperty = null;
		private PropertyDescriptor[] m_yProperties = null;

		private DataBindTimeSpanUnit m_timeSpanUnit = DataBindTimeSpanUnit.TotalMinutes;
		#endregion

		#region Properties
		/// <summary>
		/// Binds this field to the X Axis
		/// </summary>
		/// <value>The index of the X.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public int XIndex
		{
			get
			{
				return m_properties != null ? m_properties.IndexOf(m_xProperty) : m_emptyIndex;
			}
			set
			{
				if (m_properties != null)
				{
					this.XProperty = m_properties[value];
					m_xName = this.XProperty.Name;
				}
			}
		}
		/// <summary>
		/// Binds this field to the X Axis.
		/// </summary>
		[DefaultValue( null )]
		public string XName
		{
			get
			{
				return m_xName;
			}
			set
			{
				if (m_xName != value)
				{
					m_xName = value;
					this.UpdateProperites();
				}
			}
		}

		/// <summary>
		/// Binds this fields to the Y Axis
		/// </summary>
		/// <value>The Y indexes.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public int[] YIndexes
		{
			get
			{
				if (m_properties != null && m_yProperties != null)
				{
					int[] indices = new int[m_yProperties.Length];

					for (int i = 0; i < indices.Length; i++)
					{
						indices[i] = m_properties.IndexOf(m_yProperties[i]);
					}

					return indices;
				}

				return null;
			}
			set
			{
				if (m_properties != null && value != null)
				{
					PropertyDescriptor[] properties = new PropertyDescriptor[value.Length];
					m_yNames = new string[value.Length];

					for (int i = 0; i < value.Length; i++)
					{
						properties[i] = m_properties[value[i]];
						m_yNames[i] = properties[i].Name;
					}

					this.YProperties = properties;
				}
			}
		}
		/// <summary>
		/// Binds this fields to the Y Axis
		/// </summary>
		/// <value>The Y names.</value>
		[DefaultValue( null ) ]
		public string[] YNames
		{
			get
			{
				return m_yNames;
			}
			set
			{
				if (m_yNames != value)
				{
					m_yNames = value;
					this.UpdateProperites();
				}
			}
		}

		/// <summary>
		/// Gets or sets the X property.
		/// </summary>
		/// <value>The X property.</value>
		protected PropertyDescriptor XProperty
		{
			get
			{
				return m_xProperty;
			}
			set
			{
				if (m_xProperty != value)
				{
					m_xProperty = value;
					m_cache = null;
				}
			}
		}
		/// <summary>
		/// Gets or sets the X property.
		/// </summary>
		/// <value>The X property.</value>
		protected PropertyDescriptor[] YProperties
		{
			get
			{
				return m_yProperties;
			}
			set
			{
				if (m_yProperties != value)
				{
					m_yProperties = value;
					m_cache = null;
				}
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartDataBindModel"/> class.
		/// </summary>
		public ChartDataBindModel()
			: this(null, "")
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartDataBindModel"/> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		public ChartDataBindModel(object dataSource)
			: this(dataSource, "")
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartDataBindModel"/> class.
		/// </summary>
		/// <param name="dataSource">The data source that is to be used.</param>
		/// <param name="dataMember">The data member that holds the label.</param>
		public ChartDataBindModel(object dataSource, string dataMember)
			: this(dataSource, dataMember, null)
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartDataBindModel"/> class.
		/// </summary>
		/// <param name="dataSource">The data source that is to be used.</param>
		/// <param name="dataMember">The data member that holds the label.</param>
		/// <param name="bindingContext">The BindingContext. Set this to be the Form's (hosting the ChartControl) BindingContext.</param>
		public ChartDataBindModel(object dataSource, string dataMember, BindingContext bindingContext)
			: base(dataSource, dataMember, bindingContext)
		{
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Raise the Changed event.
		/// </summary>
		/// <param name="ea">The ItemChanged event data</param>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void RaiseChangedEvent(ListChangedEventArgs ea)
		{
			this.RaiseChanged(ea);
		}
		/// <summary>
		/// Implements IChartSeriesModel.GetX
		/// </summary>
		public double GetX(int index)
		{
			PointData data = this.GetPointData(index);
			return data == null ? 0d : data.X;            
		}
		/// <summary>
		/// Implements IChartSeriesModel.GetY
		/// </summary>
		public double[] GetY(int index)
		{
			PointData data = this.GetPointData(index);
			return (data == null || data.YValues == null) ? new double[] { 0d } : data.YValues;
		}
		/// <summary>
		/// Implements IChartSeriesModel.GetEmpty
		/// </summary>
		public bool GetEmpty(int index)
		{
			PointData data = this.GetPointData(index);
			return data == null ? true : data.IsEmpty;
		}
		#endregion

		#region Implementation
        /// <summary>
        /// Implements SetEmpty
        /// </summary>
        public void SetEmpty(int xIndex, bool isEmpty)
        {
            m_cache[xIndex].IsEmpty = isEmpty;
        }
		/// <summary>
		/// Gets the appropriate value.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns></returns>
		private double GetAppropriateValue(object obj)
		{
			double result = 0;

			try
			{
				if (obj != DBNull.Value && obj != null)
				{
					if (obj is TimeSpan)
					{
						result = this.GetTimeSpanValue((TimeSpan)obj);
					}
					else if (obj is DateTime)
					{
						result = ((DateTime)obj).ToOADate();
					}
					else if (obj is IConvertible)
					{
						result = Convert.ToDouble(obj);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

			return result;
		}
		/// <summary>
		/// Gets the time span value.
		/// </summary>
		/// <param name="ts">The ts.</param>
		/// <returns></returns>
		private double GetTimeSpanValue(TimeSpan ts)
		{
			switch (m_timeSpanUnit)
			{
				case DataBindTimeSpanUnit.TotalDays:
					return ts.TotalDays;

				case DataBindTimeSpanUnit.TotalHours:
					return ts.TotalHours;

				case DataBindTimeSpanUnit.TotalMinutes:
					return ts.TotalMinutes;

				case DataBindTimeSpanUnit.TotalSeconds:
					return ts.TotalSeconds;

				case DataBindTimeSpanUnit.TotalMilliseconds:
					return ts.TotalMilliseconds;
			}

			return ts.TotalSeconds;
		}
		/// <summary>
		/// Gets the point data.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		private PointData GetPointData(int index)
		{
			PointData result = null;

			#region Ensure cache is created
            if (m_cache == null || m_cache.Length != m_dataManager.Count)
			{
				if (m_dataManager != null)
				{
					m_cache = new PointData[m_dataManager.Count];
				}
			}
			#endregion

			if (m_cache != null)
			{
				if (m_cache[index] == null)
				{
					if (m_dataManager != null)
					{
						#region Create new point
						object obj = m_dataManager.List[index];

						m_cache[index] = result = new PointData();

						if (obj == null || obj == DBNull.Value)
						{
							result.IsEmpty = true;
						}

						if (m_properties != null)
						{
							#region Get X value
							if (m_xProperty != null)
							{
								object value = m_xProperty.GetValue(obj);

								if (value == null || value == DBNull.Value)
								{
									result.IsEmpty = true;
								}
								else
								{
									result.X = this.GetAppropriateValue(value);
								}
							}
							else
							{
								result.X = index;
							}
							#endregion

							#region Get Y values
							if (m_yProperties != null)
							{
								result.YValues = new double[m_yProperties.Length];

								for (int i = 0; i < m_yProperties.Length; i++)
								{
									if (m_yProperties[i] != null)
									{
										object value = m_yProperties[i].GetValue(obj);

										if (value == null || value == DBNull.Value)
										{
											result.IsEmpty = true;
										}
										else
										{
											result.YValues[i] = this.GetAppropriateValue(value);
										}
									}
								}
							}
							else
							{
								result.YValues = new double[] { this.GetAppropriateValue(obj) };
							}
							#endregion
						}
						#endregion
					}
				}
				else
				{
					result = m_cache[index];
				}
			}

			return result;
		}
		/// <summary>
		/// Resets this instance.
		/// </summary>
		protected override void Reset()
		{
			base.Reset();
			this.UpdateProperites();

			m_cache = null;
		}
		/// <summary>
		/// Called when list is changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.ComponentModel.ListChangedEventArgs"/> instance containing the event data.</param>
		protected override void OnListChanged(object sender, ListChangedEventArgs args)
		{
			m_cache = null;
			base.OnListChanged(sender, args);
		}
		/// <summary>
		/// Updates the properites.
		/// </summary>
		private void UpdateProperites()
		{
			m_xProperty = null;
			m_yProperties = null;

			if (m_properties != null)
			{
				if (m_xName != null && m_xName != string.Empty)
				{
					m_xProperty = m_properties[m_xName];
				}

				if (m_yNames != null)
				{
					m_yProperties = new PropertyDescriptor[m_yNames.Length];

					for (int i = 0; i < m_yNames.Length; i++)
					{
						m_yProperties[i] = m_properties[m_yNames[i]];
					}
				}
			}
		}
		#endregion
	}

	/// <summary>
	/// Specifies the unit that is to be used by the ChartDataBindModel for handling TimeSpan values.
	/// </summary>
	public enum DataBindTimeSpanUnit
	{
		/// <summary>
		/// 
		/// </summary>
		TotalDays,
		/// <summary>
		/// 
		/// </summary>
		TotalHours,
		/// <summary>
		/// 
		/// </summary>
		TotalMinutes,
		/// <summary>
		/// 
		/// </summary>
		TotalSeconds,
		/// <summary>
		/// 
		/// </summary>
		TotalMilliseconds
	}
}

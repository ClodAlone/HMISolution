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
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using Syncfusion.Documentation;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Chart.Renderers;
using System.CodeDom;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Text;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	/// Delegate used by the <see cref="ChartSeries.PrepareStyle"/> and <see cref="ChartSeries.PrepareSeriesStyle"/> events.
	/// </summary>
	/// <param name="sender" type="object">
	///     <para>
	///     Sender.
	///     </para>
	/// </param>
	/// <param name="args" type="Syncfusion.Windows.Forms.Chart.ChartPrepareStyleInfoEventArgs">
	///     <para>
	///		Argument.
	///     </para>
	/// </param>
	public delegate void ChartPrepareStyleInfoHandler(object sender, ChartPrepareStyleInfoEventArgs args);

	#region class ChartPrepareStyleInfoEventArgs
	/// <summary>
	/// This class is used as the argument by the <see cref="ChartSeries.PrepareStyle"/> and <see cref="ChartSeries.PrepareSeriesStyle"/> events.
	/// These events are raised when chart style information is about to be used for rendering. They provide a just-in-time hook for changing any
	/// attributes of the style object(<see cref="ChartStyleInfo"/>) before it is used by the chart.
	/// </summary>
	public class ChartPrepareStyleInfoEventArgs : EventArgs
	{
		#region Members
		private bool m_handled = false;
		private int m_index;
		private ChartStyleInfo m_styleInfo;
		#endregion

		#region Properties
		/// <summary>
		/// If the event raised has been completely handled by user code and no further processing is required, this flag should be set to
		/// True.
		/// </summary>
		public bool Handled
		{
			get
			{
				return m_handled;
			}
			set
			{
				m_handled = value;
			}
		}
		/// <summary>
		/// Returns the position of the contained style in the series.
		/// </summary>
		public int Index
		{
			get
			{
				return m_index;
			}
		}
		/// <summary>
		/// Returns the style object that is to be used by the chart.
		/// </summary>
		public ChartStyleInfo Style
		{
			get
			{
				return m_styleInfo;
			}
		}
		#endregion

		#region Constructors
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		internal ChartPrepareStyleInfoEventArgs(ChartStyleInfo styleInfo, int index)
		{
			m_styleInfo = styleInfo;
			m_index = index;
		}
		#endregion
	}
	#endregion

	/// <summary>
	/// ChartSeries acts as a wrapper around data that is to be displayed and styles that are associated with the data.
	/// The data that is to be displayed is contained in either <see cref="IChartSeriesModel"/> or <see cref="IEditableChartSeriesModel"/>
	/// implementation. The style to be used to display the points is stored in a contained implementation of <see cref="IChartSeriesStylesModel"/>.
	/// </summary>
	[TypeConverter(typeof(ChartInstanceConverter))]
	public class ChartSeries : IChartSeriesStylesHost
	{
		#region Constants
		private const string c_seriesToolTipFormat = "{0}";
		private const string c_pointsToolTipFormat = "{4}";
		#endregion

		#region Members

		#region Base Series Members
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ChartModel m_model;
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ChartSeriesIndexedModelAdapter m_indexedModelAdapter;
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected string m_name;
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ChartPointIndexer m_pointsIndexer;
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ChartPrepareStyleInfoHandler m_prepareSeriesStyleInfoHandler;
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ChartPrepareStyleInfoHandler m_prepareStyleInfoHandler;
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ChartSeriesConfigCollection m_seriesConfigCollection;
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected IChartSeriesModel m_seriesModelAdapter;
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected IChartSeriesModel m_seriesModel;
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected IChartSeriesStylesModel m_seriesStylesModel;
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ChartStyleInfoIndexer m_stylesIndexer;
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected string m_text;
		#endregion

		#region Members
		private bool m_enableStyles = true;
		private int m_explodedIndex = -1;
		private object m_tag;
		private bool m_explodedAll = false;
		private float m_explosionOffset = 20;
		private ChartSeriesRenderer m_renderer;
		private IChartSeriesSummary m_seriesSummary;
		private ChartSeriesType m_type = ChartSeriesType.Column;
		private bool m_visible = true;
		private bool m_compatible = true;
		private ChartAxis m_xAxis = null;
		private ChartAxis m_yAxis = null;
		private int m_zOrder = -1;
        private string m_stackingGroup = "Default Group";
		private double m_heightBox = 1.0;
		private double m_reversalAmount = 1.0;
		private bool m_reversalIsPercent = true;
		private ChartFancyToolTipInfo m_toolTip = new ChartFancyToolTipInfo();
		private bool m_optimizePiePointPositions = true;
		private string m_seriesToolTipFormat = c_seriesToolTipFormat;
		private string m_pointsToolTipFormat = c_pointsToolTipFormat;
		private bool m_rotate = false;
		private bool m_drawColumnSeparatingLines = false;
		private bool m_showTicks = true;
		private ScatterConnectType m_scatterConnect;
		private double m_scatterSplineTension = 0.5d;
		private bool m_drawSeriesNameInDepth;
		private float m_seriesNameOXAngle = 90f;
		private ChartSeriesLegendItem m_legendItem = null;
		private bool m_legendItemUseSeriesStyle = true;
		private bool m_needUpdateLegend = true;
		private ChartLegendItemStyle m_baseLegendStyle = new ChartLegendItemStyle();
		private bool m_smartLabels = false;
		private string m_legendName = "";
		private bool m_sortPoints = true;
		private ChartPointFormatsRegistry m_pointFormats;
        private bool m_areaToolTip = false;

        private float m_smartLabelsBorderWidth = 1f;
        private Color m_smartLabelsBorderColor = Color.Red;
        public bool m_resetStyles = true;
        private double m_seriesResolution;

		#endregion

		#endregion

		#region Events
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		public event EventHandler AppearanceChanged;
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		internal event ListChangedEventHandler DataChanged;
		/// <summary>
		/// When a series point is about to be rendered by the chart, it will raise this event and allow event subscribers to change the
		/// Series style used. You can handle this event to easily change style attributes based on external rules (for example).
		/// </summary>
		public event ChartPrepareStyleInfoHandler PrepareSeriesStyle
		{
			add
			{
				m_prepareSeriesStyleInfoHandler += value;
				RaiseSeriesStylesImplChanged();
			}
			remove
			{
				m_prepareSeriesStyleInfoHandler -= value;
				RaiseSeriesStylesImplChanged();
			}
		}
		/// <summary>
		/// When a series point is about to be rendered by the chart, it will raise this event and allow event subscribers to change the
		/// style used. You can handle this event to easily change style attributes based on external rules (for example).
		/// </summary>
		public event ChartPrepareStyleInfoHandler PrepareStyle
		{
			add
			{
				m_prepareStyleInfoHandler += value;
				RaiseSeriesStylesImplChanged();
			}
			remove
			{
				m_prepareStyleInfoHandler -= value;
				RaiseSeriesStylesImplChanged();
			}
		}
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		internal event EventHandler SeriesChanged;
		#endregion

		#region Properties

		#region Data
		/// <summary>
		/// Collection of Data points. These data points only serve as a thin wrapper around the actual data contained in the 
		/// <see cref="IChartSeriesModel"/> or <see cref="IEditableChartSeriesModel"/>. You can add, remove and edit points
		/// in this collection.
		/// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), ChartTemplate(ChartTemplateSet.SimpleAndCollection,typeof(ChartPoint))
		, TypeConverter(typeof(CollectionConverter))
		, Editor(typeof(ChartPointIndexerEditor), typeof(UITypeEditor))]
		public ChartPointIndexer Points
		{
			get
			{
				if (m_pointsIndexer == null)
				{
					m_pointsIndexer = new ChartPointIndexer(this.SeriesModelAdapter);
				}

				return m_pointsIndexer;
			}
		}
		/// <summary>
		/// Gets the formats.
		/// </summary>
		/// <value>The formats.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		internal ChartPointFormatsRegistry PointFormats
		{
			get { return m_pointFormats; }
		}
		/// <summary>
		/// Gets or sets the name of this series object. You can retrieve a series by its name from the <see cref="ChartSeriesCollection"/> object in the
		/// <see cref="ChartModel"/> where it is stored.
		/// </summary>
		/// <remarks>
		/// The <see cref="ChartModel"/> can't contains several series with the same name.
		/// </remarks>
		[ChartTemplate(ChartTemplateSet.Simple),DefaultValue(""), Category("Data")]
		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				if (m_name != value)
				{
					// Name check
					//if (string.IsNullOrEmpty(value))
					//  throw new ArgumentNullException("value");

					//if (m_model != null && m_model.Series[value] != null)
					//  throw new ArgumentException( string.Format("A series with {0} name is already added.", value ));

					m_name = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the object that contains data about the series. 
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public object Tag
		{
			get
			{
				return m_tag;
			}
			set
			{
				m_tag = value;
			}
		}
		/// <summary>
		/// Gets /sets the text that is to be associated with this series. This is the text that will be displayed by default by the legend item associated with
		/// this series.
		/// </summary>
		[DefaultValue(""), ChartTemplate(ChartTemplateSet.Simple)]
		public string Text
		{
			get
			{
				return m_text;
			}
			set
			{
				if (m_text != value)
				{
					m_text = value;

					this.UpdateLegendItem();
					this.OnAppearanceChanged(EventArgs.Empty);
				}
			}
		}
		#endregion

		#region Object model
		/// <summary>
		/// Gets or sets the Chart's model.
		/// <seealso cref="ChartModel"/>
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public ChartModel ChartModel
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
						m_model.ColorModel.Changed -= new EventHandler(OnColorModelChanged);
						m_model.Series.Remove(this);
					}

					m_model = value;

					if (m_model != null)
					{
						m_model.ColorModel.Changed += new EventHandler(OnColorModelChanged);
						m_model.Series.Add(this);

						if (m_model.Chart != null && m_model.Chart.IsDesignTime)
						{
							this.SetSeriesModelAdapter(new ChartDummyPointsAdapter(this));
						}
					}

					this.StylesImpl.ComposedStyles.ResetCache();
				}
			}
		}
		/// <summary>
		/// The <see cref="IChartSeriesIndexedModel"/> interface is a special interface that serves as a degraded special case of <see cref="IChartSeriesModel"/>.
		/// The special case being situations where the X value is not needed. When you implement <see cref="IChartSeriesIndexedModel"/> and set it to this property,
		/// the chart will internally create an adapter that implements <see cref="IChartSeriesModel"/> and treat it as any other model.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public IChartSeriesIndexedModel SeriesIndexedModelImpl
		{
			get
			{
				return m_indexedModelAdapter.Inner;
			}
			set
			{
				m_indexedModelAdapter = new ChartSeriesIndexedModelAdapter(value);
				this.SetSeriesModel(m_indexedModelAdapter);
			}
		}
		/// <summary>
		/// Returns an instance of the <see cref="IChartSeriesModel"/> underlying this series.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(false)]
		public IChartSeriesModel SeriesModel
		{
			get
			{
				if (m_seriesModel == null)
				{
					this.SetSeriesModel(this.OnCreateSeriesModel());
				}

				return m_seriesModel;
			}
			set
			{
				this.SetSeriesModel(value);
			}
		}
		/// <summary>
		/// Returns an instance of the <see cref="IChartSeriesModel"/> underlying this series.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		internal IChartSeriesModel SeriesModelAdapter
		{
			get
			{
				if (m_seriesModelAdapter == null)
				{
					this.SetSeriesModelAdapter(new ChartSeriesModelAdapter(this));
				}

				return m_seriesModelAdapter;
			}
		}
		/// <summary>
		/// Provides access to summary information such as minimum / maximum values contained in this series at any given
		/// moment.    
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public IChartSeriesSummary Summary
		{
			get
			{
				if (m_seriesSummary == null)
				{
					this.SetSummaryImpl(this.OnCreateSeriesSummaryImpl());
				}

				return m_seriesSummary;
			}
		}
		/// <summary>
		/// Gets or sets the X Axis instance against which this series will be plotted.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public ChartAxis XAxis
		{
			get
			{
				return m_xAxis;
			}
			set
			{

				if (m_xAxis != value)
				{
					m_xAxis = value;
					this.OnSeriesChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Returns actual X axis, that values of series.Points[i].X are plotted on it.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public ChartAxis ActualXAxis
		{
			get
			{
				return RequireInvertedAxes ? YAxis : XAxis;
			}
		}
		/// <summary>
		/// Returns actual Y axis, that values of series.Points[i].Yvalues are plotted on it.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public ChartAxis ActualYAxis
		{
			get
			{
				return RequireInvertedAxes ? XAxis : YAxis;
			}
		}
		/// <summary>
		/// Returns the X value type that is being rendered. Please refer to <see cref="ChartValueType"/> for details on supported value types.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ChartValueType XType
		{
			get
			{
				return ActualXAxis.ValueType;
			}
		}
		/// <summary>
		/// Gets or sets the Y Axis instance against which this series will be plotted.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public ChartAxis YAxis
		{
			get
			{
				return m_yAxis;
			}
			set
			{

				if (m_yAxis != value)
				{
					m_yAxis = value;
					this.OnSeriesChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Returns the Y value type that is being rendered. Please refer to <see cref="ChartValueType"/> for details on supported value types.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ChartValueType YType
		{
			get
			{
				return ActualYAxis.ValueType;
			}
		}
		#endregion

		#region Styles

        /// <summary>     
        /// Based on the resolution the number of points drawn will be reduced for improving the performance.
        /// </summary>
        public double Resolution
        {
            get
            {
               return m_seriesResolution;
            }
            set
            {
                m_seriesResolution = value;
            }

        }
		/// <summary>
		/// If set to False, the rendering is faster with the following remarks:
		/// The points style is disabled, all points use series style.
		/// </summary>
		[DefaultValue(true),ChartTemplate(ChartTemplateSet.Simple)]
		public bool EnableStyles
		{
			get
			{
				return m_enableStyles;
			}
			set
			{
				m_enableStyles = value;
			}
		}
		/// <summary>
		/// Returns the style object associated with the series. Attributes that are applied to this style will change the appearance of the complete
		/// series.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), ChartTemplate(ChartTemplateSet.Content),Browsable(false)]
		public ChartStyleInfo Style
		{
			get
			{
				if (m_seriesStylesModel == null)
				{
					this.SetStylesImpl(this.OnCreateSeriesStylesModelImpl());
				}

				return m_seriesStylesModel.Style;
			}
		}
		/// <summary>
		/// Returns the styles that represent rendering information for the individual points in the series. Each of these style objects can be manipulated to
		/// affect the formatting and display of individual points. Styles set to individual points take precedence over the style of the Series (<see cref="Style"/>.
		/// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),ChartTemplate(ChartTemplateSet.SimpleAndCollection,typeof(ChartStyleInfo)), Browsable(false)]
		public ChartStyleInfoIndexer Styles
		{
			get
			{
				if (m_seriesStylesModel == null)
				{
					this.SetStylesImpl(this.OnCreateSeriesStylesModelImpl());
				}

				return m_stylesIndexer;
			}
		}
		/// <summary>
		/// Gets or sets the object that implements <see cref="IChartSeriesStylesModel"/>. This object stores styles in an optimized manner and
		/// provides them on demand. You can replace this object with your own implementation of this interface to meet specific performance
		/// needs. In most cases, you should just use the default styles model that is provided.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public IChartSeriesStylesModel StylesImpl
		{
			get
			{
				if (m_seriesStylesModel == null)
				{
					this.SetStylesImpl(this.OnCreateSeriesStylesModelImpl());
				}

				return m_seriesStylesModel;
			}
			set
			{
				this.SetStylesImpl(value);
			}
		}
		#endregion

		#region Appearance
		/// <summary>
		/// Gets the base type of the ChartSeries. The BaseType is used by the rendering code to check which of the pre-defined display patterns this series fits.
		/// <seealso cref="ChartSeriesBaseType"/>
		/// </summary>
		[Browsable(false)]
		public ChartSeriesBaseType BaseType
		{
			get
			{
				if (m_type == ChartSeriesType.Bar ||
					m_type == ChartSeriesType.Candle ||
					m_type == ChartSeriesType.Column ||
					m_type == ChartSeriesType.ColumnRange ||
					m_type == ChartSeriesType.HiLo ||
					m_type == ChartSeriesType.HiLoOpenClose ||
					m_type == ChartSeriesType.Gantt ||
					m_type == ChartSeriesType.BoxAndWhisker ||
					m_type == ChartSeriesType.StackingColumn ||
					m_type == ChartSeriesType.StackingBar ||
					m_type == ChartSeriesType.StackingArea100 ||
					m_type == ChartSeriesType.StackingBar100 ||
					m_type == ChartSeriesType.StackingColumn100)
				{
					return ChartSeriesBaseType.SideBySide;
				}

				if (m_type == ChartSeriesType.Area ||
					m_type == ChartSeriesType.Line ||
					m_type == ChartSeriesType.SplineArea ||
					m_type == ChartSeriesType.RotatedSpline ||
					m_type == ChartSeriesType.Spline ||
					m_type == ChartSeriesType.StepArea ||
					m_type == ChartSeriesType.Tornado ||
					m_type == ChartSeriesType.StackingArea)
				{
					return ChartSeriesBaseType.Independent;
				}

				if (m_type == ChartSeriesType.Radar ||
					m_type == ChartSeriesType.Polar)
				{
					return ChartSeriesBaseType.Circular;
				}

				if (m_type == ChartSeriesType.Pie ||
					m_type == ChartSeriesType.Funnel ||
					m_type == ChartSeriesType.Pyramid ||
					m_type == ChartSeriesType.HeatMap)
				{
					return ChartSeriesBaseType.Single;
				}

				return ChartSeriesBaseType.Other;
			}
		}
		/// <summary>
		/// Gets the base type of the ChartSeries. The BaseType is used by the rendering code to check which of the pre-defined display patterns this series fits.
		/// <seealso cref="ChartSeriesBaseType"/>
		/// </summary>
		[Browsable(false)]
		public ChartSeriesBaseStackingType BaseStackingType
		{
			get
			{

				if (m_type == ChartSeriesType.StackingBar ||
					m_type == ChartSeriesType.StackingColumn ||
					m_type == ChartSeriesType.StackingArea)
				{
					return ChartSeriesBaseStackingType.Stacked;
				}

				if (m_type == ChartSeriesType.StackingArea100 ||
					m_type == ChartSeriesType.StackingBar100 ||
					m_type == ChartSeriesType.StackingColumn100)
				{
					return ChartSeriesBaseStackingType.FullStacked;
				}

				return ChartSeriesBaseStackingType.NotStacked;
			}
		}
		/// <summary>
		///    Returns the Chart series configuration items.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public ChartSeriesConfigCollection ConfigItems
		{
			get
			{
				return m_seriesConfigCollection;
			}
		}
		/// <summary>
		/// Gets or sets the <see cref="ChartSeriesRenderer"/>.
		/// </summary>
		/// <value>The renderer.</value>
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public ChartSeriesRenderer Renderer
		{
			get
			{
				return m_renderer;
			}
			set
			{
				m_renderer = value;
			}
		}
		/// <summary>
		/// Indicates whether the currently set series type requires axes to be rendered. Currently set to False only for Pie charts.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),ChartTemplate(ChartTemplateSet.Simple), Browsable(false)]
		public bool RequireAxes
		{
			get
			{
				return (this.BaseType != ChartSeriesBaseType.Circular)
					&& (this.BaseType != ChartSeriesBaseType.Single);
			}
		}
		/// <summary>
		/// Indicates whether the currently set series type requires axes to be inverted.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),ChartTemplate(ChartTemplateSet.Simple), Browsable(false)]
		public bool RequireInvertedAxes
		{
			get
			{
				return m_type == ChartSeriesType.Bar || m_type == ChartSeriesType.Gantt
					|| m_type == ChartSeriesType.StackingBar || m_type == ChartSeriesType.StackingBar100
					|| m_type == ChartSeriesType.RotatedSpline || m_type == ChartSeriesType.Tornado
					|| this.Rotate;
			}
		}
		/// <summary>
		/// Gets a value indicating whether a series is dependent by <see cref="ChartAxis.Origin"/>.
		/// </summary>
		/// <value><c>true</c> if a series is dependent by origin; otherwise, <c>false</c>.</value>
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		[Browsable(false),ChartTemplate(ChartTemplateSet.Simple)]
		public bool OriginDependent
		{
			get
			{
				return (m_type == ChartSeriesType.Area ||
					m_type == ChartSeriesType.Bar ||
					m_type == ChartSeriesType.Column ||
					m_type == ChartSeriesType.SplineArea ||
					m_type == ChartSeriesType.StackingArea ||
					m_type == ChartSeriesType.StackingBar ||
					m_type == ChartSeriesType.StackingColumn ||
					m_type == ChartSeriesType.StepArea ||
					m_type == ChartSeriesType.StackingArea100 ||
					m_type == ChartSeriesType.StackingBar100 ||
					m_type == ChartSeriesType.Histogram ||
					m_type == ChartSeriesType.StackingColumn100);
			}
		}
		/// <summary>
		/// Gets or sets the chart type that is to be rendered using this series. Please refer to <see cref="ChartSeriesType"/> for a complete list and
		/// explanation of chart types.
		/// </summary>
		[ChartTemplate(ChartTemplateSet.Simple)
	 , DefaultValue(ChartSeriesType.Column), Category("Chart")]
		public ChartSeriesType Type
		{
			get
			{
				return m_type;
			}
			set
			{
				if (m_type != value || m_renderer == null)
				{
					m_type = value;
					m_pointFormats.OnSeriesTypeChanged(value);

					switch (m_type)
					{
						case ChartSeriesType.RangeArea:
							m_renderer = new RangeAreaRenderer(this);
							break;

						case ChartSeriesType.Line:
							m_renderer = new LineRenderer(this);
							break;

						case ChartSeriesType.Column:
							m_renderer = new ColumnRenderer(this);
							break;

						case ChartSeriesType.Pie:
							m_renderer = new PieRenderer(this);
							break;

						case ChartSeriesType.Area:
							m_renderer = new AreaRenderer(this);
							break;

						case ChartSeriesType.Bar:
							m_renderer = new BarRenderer(this);
							break;

						case ChartSeriesType.SplineArea:
							m_renderer = new SplineAreaRenderer(this);
							break;

						case ChartSeriesType.Gantt:
							m_renderer = new GanttRenderer(this);
							break;

						case ChartSeriesType.StackingArea:
							m_renderer = new StackingAreaRenderer(this);
							break;

						case ChartSeriesType.HiLo:
							m_renderer = new HiLoRenderer(this);
							break;

						case ChartSeriesType.HiLoOpenClose:
							m_renderer = new HiLoOpenCloseRenderer(this);
							break;

						case ChartSeriesType.Candle:
							m_renderer = new CandleRenderer(this);
							break;

						case ChartSeriesType.Scatter:
							m_renderer = new ScatterRenderer(this);
							break;

						case ChartSeriesType.StackingColumn:
							m_renderer = new StackingColumnRenderer(this);
							break;

						case ChartSeriesType.StackingBar:
							m_renderer = new StackingBarRenderer(this);
							break;

						case ChartSeriesType.Spline:
							m_renderer = new SplineRenderer(this);
							break;

						case ChartSeriesType.Bubble:
							m_renderer = new BubbleRenderer(this);
							break;

						case ChartSeriesType.Custom:
							m_renderer = new ChartSeriesRenderer(this);
							break;

						case ChartSeriesType.StepLine:
							m_renderer = new StepLineRenderer(this);
							break;

						case ChartSeriesType.StepArea:
							m_renderer = new StepAreaRenderer(this);
							break;

						case ChartSeriesType.Radar:
							m_renderer = new RadarRenderer(this);
							break;

						case ChartSeriesType.Kagi:
							m_renderer = new KagiRenderer(this);
							break;

						case ChartSeriesType.Renko:
							m_renderer = new RenkoRenderer(this);
							break;

						case ChartSeriesType.Polar:
							m_renderer = new RadarRenderer(this);
							break;

						case ChartSeriesType.ThreeLineBreak:
							m_renderer = new ThreeLineBreakRenderer(this);
							break;

						case ChartSeriesType.PointAndFigure:
							m_renderer = new PointAndFigureRenderer(this);
							break;

						case ChartSeriesType.RotatedSpline:
							m_renderer = new RotatedSplineRenderer(this);
							break;

						case ChartSeriesType.ColumnRange:
							m_renderer = new ColumnRangeRenderer(this);
							break;

						case ChartSeriesType.BoxAndWhisker:
							m_renderer = new BoxWhiskerRenderer(this);
							break;

						case ChartSeriesType.Histogram:
							m_renderer = new HistogramRenderer(this);
							break;

						case ChartSeriesType.Tornado:
							m_renderer = new TornadoRenderer(this);
							break;

						case ChartSeriesType.StackingArea100:
							m_renderer = new FullStackingAreaRenderer(this);
							break;

						case ChartSeriesType.StackingBar100:
							m_renderer = new FullStackingBarRenderer(this);
							break;

						case ChartSeriesType.StackingColumn100:
							m_renderer = new FullStackedColumnRenderer(this);
							break;

						case ChartSeriesType.Funnel:
							m_renderer = new FunnelRenderer(this);
							break;

						case ChartSeriesType.Pyramid:
							m_renderer = new PyramidRenderer(this);
							break;

						case ChartSeriesType.HeatMap:
							m_renderer = new ChartHeatMapRenderer(this);
							break;
					}

					this.OnSeriesChanged(EventArgs.Empty);
					this.InvalidateStyles();
				}
			}
		}
        public string StackingGroup
        {
            get
            {
                return m_stackingGroup;
            }
            set
            {
                if (m_stackingGroup != value)
                {
                    m_stackingGroup = value;
                }

            }
        }
		/// <summary>
		/// Gets or sets the ZOrder of the series. You can use this setting to control which series gets plotted first. The chart will sort
		/// by ZOrder before rendering.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),ChartTemplate(ChartTemplateSet.Simple),Browsable(false)]
		public int ZOrder
		{
			get
			{
				return m_zOrder;
			}
			set
			{

				if (m_zOrder != value)
				{
					m_zOrder = value;
					//this.OnAppearanceChanged( EventArgs.Empty );
				}
			}
		}
		/// <summary>
		/// Gets or sets the format for tooltip
		/// "{0}" - series name
		/// "{1}" - series style tooltip
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public string SeriesToolTipFormat
		{
			get
			{
				return m_seriesToolTipFormat;
			}
			set
			{
				if (m_seriesToolTipFormat != value)
				{
					m_seriesToolTipFormat = value;
					this.UpdateRenderer(ChartUpdateFlags.Styles);
					this.OnAppearanceChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets or sets the format for tooltip
		/// "{0}" - series name
		/// "{1}" - series style tooltip
		/// "{2}" - tooltip of point
		/// "{3}" - X value of point
		/// "{4}" and other - Y value of point
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),ChartTemplate(ChartTemplateSet.Simple), Browsable(false)]
		public string PointsToolTipFormat
		{
			get
			{
				return m_pointsToolTipFormat;
			}
			set
			{
				if (m_pointsToolTipFormat != value)
				{
					m_pointsToolTipFormat = value;
					this.UpdateRenderer(ChartUpdateFlags.Styles);
					this.OnAppearanceChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Indicates whether the series is to be plotted.
		/// </summary>
		[DefaultValue(true),ChartTemplate(ChartTemplateSet.Simple)]
		public bool Visible
		{
			get
			{
				return m_visible;
			}
			set
			{
				if (m_visible != value)
				{
					m_visible = value;

					this.OnAppearanceChanged(EventArgs.Empty);
					this.OnSeriesChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Refer <see cref="IChartSeriesStylesHost.BackColor"/>
		/// </summary>
		[Browsable(false)]
		public Color BackColor
		{
			get
			{
				int seriesIndex = m_model.Series.IndexOf(this);

				return m_model.ColorModel.GetColor(seriesIndex);
			}
		}
		/// <summary>
		/// Indicates whether the series is compatible with other series added to the series collection.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),ChartTemplate(ChartTemplateSet.Simple),Browsable(false)]
		public bool Compatible
		{
			get
			{
				return m_compatible;
			}
			set
			{

				if (m_compatible != value)
				{
					m_compatible = value;
					this.OnAppearanceChanged(EventArgs.Empty);
					this.OnSeriesChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Returns the Fancy tooltip
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),ChartTemplate(ChartTemplateSet.Content), Browsable(false)]
		public ChartFancyToolTipInfo FancyToolTip
		{
			get
			{
				return m_toolTip;
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether points will be sorted.
		/// </summary>
		/// <value><c>true</c> if points will be sorted; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool SortPoints
		{
			get { return m_sortPoints; }
			set
			{
				m_sortPoints = value;
				this.OnSeriesChanged(EventArgs.Empty);
			}
		}
		#endregion

		#region Legend
		/// <summary>
		/// Returns the private instance of LegendItem class.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public ChartSeriesLegendItem LegendItem
		{
			get
			{
				if (m_legendItem == null)
				{
					m_legendItem = new ChartSeriesLegendItem(this);
				}

				if (m_needUpdateLegend)
				{
					this.UpdateLegendItem();
					m_needUpdateLegend = false;
				}

				return m_legendItem;
			}
		}
		/// <summary>
		/// Gets or sets value indicates the legend for representation of series.
		/// </summary>
		[DefaultValue(""),ChartTemplate(ChartTemplateSet.Simple)]
		public string LegendName
		{
			get
			{
				return m_legendName;
			}
			set
			{
				m_legendName = value;
			}
		}
		/// <summary>
		/// Indicates whether the legend item should use the series style.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), ChartTemplate(ChartTemplateSet.Simple),Browsable(false)]
		public bool LegendItemUseSeriesStyle
		{
			get
			{
				return m_legendItemUseSeriesStyle;
			}
			set
			{
				if (m_legendItemUseSeriesStyle != value)
				{
					m_legendItemUseSeriesStyle = value;

					if (!m_legendItemUseSeriesStyle)
					{
						m_legendItem.ItemStyle.Clear();
					}

					this.OnAppearanceChanged(EventArgs.Empty);
				}
			}
		}
		#endregion

		#region User customized properties
		/// <summary>
		/// Gets or sets the index of the point that is to be exploded from the main display. In the current implementation, this property is used
		/// only by the pie chart.
		/// </summary>
		[ChartTemplate(ChartTemplateSet.Simple), DefaultValue(-1)]
		public int ExplodedIndex
		{
			get
			{
				return m_explodedIndex;
			}
			set
			{

				if (m_explodedIndex != value)
				{
					m_explodedIndex = value;
					this.OnAppearanceChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Explode all points . In the current implementation, this property is used
		/// only by the pie chart.
		/// </summary>
		[ChartTemplate(ChartTemplateSet.Simple), DefaultValue(false)]
		public bool ExplodedAll
		{
			get
			{
				return m_explodedAll;
			}
			set
			{

				if (m_explodedAll != value)
				{
					m_explodedAll = value;
					this.OnAppearanceChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets or sets the offset value that is to be used when a point is to be exploded from the main display. Currently applies
		/// only to the Pie chart. Offset is taken in percentage terms.
		/// </summary>
		[ChartTemplate(ChartTemplateSet.Simple), DefaultValue(20f)]
		public float ExplosionOffset
		{
			get
			{
				return m_explosionOffset;
			}
			set
			{
				value = ChartMath.MinMax(value, 0f, 100f);

				if (m_explosionOffset != value)
				{
					m_explosionOffset = value;
					this.OnAppearanceChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets or sets the reversal amount (Useful for Kagi chart,PointAndFigure chart and Renko chart)
		/// </summary>
		[DefaultValue(1d),ChartTemplate(ChartTemplateSet.Simple)]
		public double ReversalAmount
		{
			get
			{
				return m_reversalAmount;
			}
			set
			{
				if (m_reversalAmount != value)
				{
					m_reversalAmount = value;
					this.OnAppearanceChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Indicates if the Reversal amount is taken in percentage
		/// </summary>
		[DefaultValue(true)]
		public bool ReversalIsPercent
		{
			get
			{
				return m_reversalIsPercent;
			}
			set
			{
				if (m_reversalIsPercent != value)
				{
					m_reversalIsPercent = value;
					this.OnAppearanceChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets or sets height of the boxes in the financial chart types.
		/// </summary>
		[DefaultValue(1d)]
		public double HeightBox
		{
			get
			{
				return m_heightBox;
			}
			set
			{
				m_heightBox = value;
			}
		}

		/// <summary>
		/// Indicates if the pie points are optimized for position
		/// </summary>
		[DefaultValue(true),ChartTemplate(ChartTemplateSet.Simple)]
		public bool OptimizePiePointPositions
		{
			get
			{
				return m_optimizePiePointPositions;
			}
			set
			{
				if (m_optimizePiePointPositions != value)
				{
					m_optimizePiePointPositions = value;
					this.OnAppearanceChanged(EventArgs.Empty);
				}
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether reset all the styles while modifying the ChartPoint properties.
        /// </summary>
        [Description("Gets or sets a value indicating whether reset all the styles while modifying the ChartPoint properties."), ChartTemplate(ChartTemplateSet.Simple), DefaultValue(true)]
        public bool ResetStyles //addedline
        {
            get
            {
                return m_resetStyles;
            }
            set
            {
                if (value != m_resetStyles)
                {
                    m_resetStyles = value;
                }
            }
        }

		/// <summary>
		/// Indicates whether the ChartArea is to be rotated and rendered. Default value is false.    
		/// </summary>
		[ChartTemplate(ChartTemplateSet.Simple)
		, DefaultValue(false), Category("Apearance")]
		public bool Rotate
		{
			get
			{
				return m_rotate;
			}
			set
			{
				if (m_rotate != value)
				{
					m_rotate = value;
					this.OnAppearanceChanged(EventArgs.Empty);
					this.OnSeriesChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// The drawing of separating line between columns is controlled by this property.     
		/// </summary>
		[DefaultValue(false),ChartTemplate(ChartTemplateSet.Simple)]
		public bool DrawColumnSeparatingLines
		{
			get
			{
				return m_drawColumnSeparatingLines;
			}
			set
			{
				if (m_drawColumnSeparatingLines != value)
				{
					m_drawColumnSeparatingLines = value;
					this.OnAppearanceChanged(EventArgs.Empty);
					this.OnSeriesChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Indicates if the Ticks should be shown (only for Pie charts)
		/// </summary>
		[DefaultValue(true),ChartTemplate(ChartTemplateSet.Simple),Category("Apearance")]
		public bool ShowTicks
		{
			get
			{
				return m_showTicks;
			}
			set
			{
				if (m_showTicks != value)
				{
					m_showTicks = value;
					this.OnAppearanceChanged(EventArgs.Empty);
					this.OnSeriesChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Specifies connect type of scatter chart
		/// </summary>
		[DefaultValue(ScatterConnectType.None),ChartTemplate(ChartTemplateSet.Simple)]
		public ScatterConnectType ScatterConnectType
		{
			get
			{
				return m_scatterConnect;
			}
			set
			{
				if (m_scatterConnect != value)
				{
					m_scatterConnect = value;
					this.OnAppearanceChanged(EventArgs.Empty);
					this.OnSeriesChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets or sets the tension required for the scatter spline chart.
		/// </summary>
		[DefaultValue((double)0.5d),ChartTemplate(ChartTemplateSet.Simple)]
		public double ScatterSplineTension
		{
			get
			{
				return m_scatterSplineTension;
			}
			set
			{
				if (m_scatterSplineTension != value)
				{
					m_scatterSplineTension = value;
					this.OnAppearanceChanged(EventArgs.Empty);
					this.OnSeriesChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Indicates whether to draw series name at opposed position to origin, along x axis.
		/// </summary>
		[DefaultValue(false), ChartTemplate(ChartTemplateSet.Simple),Category("Apearance")]
		public bool DrawSeriesNameInDepth
		{
			get
			{
				return m_drawSeriesNameInDepth;
			}
			set
			{
				if (m_drawSeriesNameInDepth != value)
				{
					m_drawSeriesNameInDepth = value;
					this.OnAppearanceChanged(EventArgs.Empty);
					this.OnSeriesChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Indicates rotation angle around x axis of series name string.
		/// </summary>
		[DefaultValue(90f)]
		public float SeriesNameOXAngle
		{
			get
			{
				return m_seriesNameOXAngle;
			}
			set
			{
				if (m_seriesNameOXAngle != value)
				{
					m_seriesNameOXAngle = value;
					this.OnAppearanceChanged(EventArgs.Empty);
					this.OnSeriesChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Specifies the behavior of the labels.
		/// </summary>
		[ChartTemplate(ChartTemplateSet.Simple)
		, DefaultValue(false), Category("Apearance")]
		public bool SmartLabels
		{
			get
			{
				return m_smartLabels;
			}
			set
			{
				if (m_smartLabels != value)
				{
					m_smartLabels = value;
					this.OnAppearanceChanged(EventArgs.Empty);
					this.OnSeriesChanged(EventArgs.Empty);
				}
			}
		}

        /// <summary>
        /// Gets or sets the BorderWidth of the Smartlabels.
        /// </summary>

        [ChartTemplate(ChartTemplateSet.Simple),Description("Specifies the BorderWidth of the Smartlabels"), DefaultValue(1f)]
        public float SmartLabelsBorderWidth
        {
            get
            {
                return m_smartLabelsBorderWidth;
            }
            set
            {
                if (m_smartLabelsBorderWidth != value)
                {
                    m_smartLabelsBorderWidth = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                    this.OnSeriesChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the BorderColor of the Smartlabels.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple),Description("Specifies the BorderColor of the Smartlabels"), DefaultValue(typeof(Color),"Red"), Category("Appearance")]
        public Color SmartLabelsBorderColor
        {
            get
            {
                return m_smartLabelsBorderColor;
            }
            set
            {
                if (m_smartLabelsBorderColor != value)
                {
                    m_smartLabelsBorderColor = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                    this.OnSeriesChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get or set to enable or disable the ToolTip for Full Area of ChartInterior. This is only for AreaCharts.
        /// </summary>
        [DefaultValue(false), ChartTemplate(ChartTemplateSet.Simple), Description("Get or set to enable or disable the ToolTip for Full Area of ChartInterior. This is only for AreaCharts.")]
        public bool EnableAreaToolTip 
        {
            get
            {
                return m_areaToolTip;
            }
            set
            {
                if (m_areaToolTip != value)
                {
                    m_areaToolTip = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                    this.OnSeriesChanged(EventArgs.Empty);
                }
            }
        }        

		#endregion

		#region Old properties
		/// <summary>
		/// Error Bars are used to indicate a degree of uncertainity in the plotted data through a bar indicating an "error range". 
		/// The 2nd Y value of a <see cref="ChartPoint"/> is used to indicate the error range. This is supported with Line, Bar and Column charts. 
		/// Also see <see cref="ErrorBarsSymbolShape"/>
		/// </summary>
		[DefaultValue(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use ConfigItems.ErrorBars.Enabled property")]
		public bool DrawErrorBars
		{
			get
			{
				return m_seriesConfigCollection.ErrorBars.Enabled;
			}
			set
			{
				m_seriesConfigCollection.ErrorBars.Enabled = value;
			}
		}
		/// <summary>
		/// Specifies the symbol that should be used in error bars. Also see <see cref="DrawErrorBars"/> 
		/// </summary>
		[DefaultValue(ChartSymbolShape.Diamond)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use ConfigItems.ErrorBars.SymbolShape property")]
		public ChartSymbolShape ErrorBarsSymbolShape
		{
			get
			{
				return m_seriesConfigCollection.ErrorBars.SymbolShape;
			}
			set
			{
				m_seriesConfigCollection.ErrorBars.SymbolShape = value;
			}
		}
		/// <summary>
		/// Gets or sets the number of Histogram intervals
		/// </summary>
		[DefaultValue(10)]
		[Obsolete("Use ConfigItems.HistogramItem.NumberOfIntervals")
		, EditorBrowsable(EditorBrowsableState.Never)]
		public int NumberOfHistogramIntervals
		{
			get
			{
				return m_seriesConfigCollection.HistogramItem.NumberOfIntervals;
			}
			set
			{
				m_seriesConfigCollection.HistogramItem.NumberOfIntervals = value;
			}
		}
		/// <summary>
		/// Indicates if the histogram data points should be shown
		/// </summary>
		[DefaultValue(true)]
		[Obsolete("Use ConfigItems.HistogramItem.ShowDataPoints")
		, EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShowHistogramDataPoints
		{
			get
			{
				return m_seriesConfigCollection.HistogramItem.ShowDataPoints;
			}
			set
			{
				m_seriesConfigCollection.HistogramItem.ShowDataPoints = value;
			}
		}
		/// <summary>
		/// Gets or sets an instance of the <see cref="IChartSeriesModel"/> underlying this series. Use this property to replace this instance with your own
		/// implementation. Use <see cref="ChartSeries.SeriesModel"/> to access the model if you do not intend to replace it.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		, EditorBrowsable(EditorBrowsableState.Never)
		, Browsable(false), Obsolete("Use SeriesModel property. This property just duplicate it.")]
		public IChartSeriesModel SeriesModelImpl
		{
			get
			{
				return this.SeriesModel;
			}
			set
			{
				this.SetSeriesModel(value);
			}
		}
		/// <summary>
		/// Indicates if the Histogram normal distribution should be drawn
		/// </summary>
		[DefaultValue(false)]
		[Obsolete("Use ConfigItems.HistogramItem.ShowNormalDistribution")
		, EditorBrowsable(EditorBrowsableState.Never)]
		public bool DrawHistogramNormalDistribution
		{
			get
			{
				return m_seriesConfigCollection.HistogramItem.ShowNormalDistribution;
			}
			set
			{
				m_seriesConfigCollection.HistogramItem.ShowNormalDistribution = value;
			}
		}
		/// <summary>
		/// Specifies the drawing mode of Gantt chart
		/// </summary>
		[Obsolete("Use ConfigItems.GanttItem.DrawMode")]
		[DefaultValue(ChartGanttDrawMode.CustomPointWidthMode)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ChartGanttDrawMode GanttDrawMode
		{
			get
			{
				return m_seriesConfigCollection.GanttItem.DrawMode;
			}
			set
			{
				m_seriesConfigCollection.GanttItem.DrawMode = value;
			}
		}
		/// <summary>
		/// Indicates rotation angle around x axis of series name string.
		/// </summary>
		[DefaultValue(ChartOpenCloseDrawMode.Both)]
		[Obsolete("Use ConfigItems.HiLoOpenCloseItem.DrawMode")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ChartOpenCloseDrawMode OpenCloseDrawMode
		{
			get
			{
				return this.ConfigItems.HiLoOpenCloseItem.DrawMode;
			}
			set
			{
				this.ConfigItems.HiLoOpenCloseItem.DrawMode = value;
			}
		}
		/// <summary>
		/// Sets / Gets the doughnut coefficient of pie chart 
		/// </summary>
		[Obsolete("Use ConfigItems.PieItem.DoughnutCoeficient")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		, Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public float InSideRadius
		{
			get
			{
				return ConfigItems.PieItem.DoughnutCoeficient;
			}
			set
			{
				ConfigItems.PieItem.DoughnutCoeficient = value;
			}
		}
		/// <summary>
		/// Gets or sets the price down color
		/// </summary>
		[Obsolete("Use ConfigItems.FinancialItem.PriceUpColor")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		, Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Color PriceUpColor
		{
			get
			{
				return ConfigItems.FinancialItem.PriceUpColor;
			}
			set
			{
				ConfigItems.FinancialItem.PriceUpColor = value;
			}
		}
		/// <summary>
		/// Gets or sets the price up color
		/// </summary>
		[Obsolete("Use CConfigItems.FinancialItem.PriceDownColor")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		, Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Color PriceDownColor
		{
			get
			{
				return ConfigItems.FinancialItem.PriceDownColor;
			}
			set
			{
				ConfigItems.FinancialItem.PriceDownColor = value;
			}
		}
		#endregion

		#region ShouldSerialize and Reset methods
		/// <summary>
		/// Should the serialize points.
		/// </summary>
		/// <returns></returns>
		private bool ShouldSerializePoints()
		{
			return (m_seriesModel is ChartSeriesModel) && (m_seriesModel.Count > 0);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool ShouldSerializeSeriesModel()
		{
			return !(m_seriesModel is ChartSeriesModel);
		}
		/// <summary>
		/// 
		/// </summary>
		public void ResetSeriesModel()
		{
			this.SeriesModel = this.OnCreateSeriesModel();
		}
		#endregion

		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the ChartSeries class.
		/// </summary>
		public ChartSeries()
			: this("")
		{
		}
		/// <summary>
		/// Initializes a new instance of the ChartSeries class.
		/// </summary>
		/// <param name="name">An name of series. This value will be set to <see cref="ChartSeries.Text"/> property too.</param>
		/// <param name="type">An type of series.</param>
		public ChartSeries(string name, ChartSeriesType type)
		{
			m_name = name;

			m_seriesConfigCollection = new ChartSeriesConfigCollection();
			m_seriesConfigCollection.Changed += new EventHandler(this.OnConfigItemsChanged);
			m_pointFormats = new ChartPointFormatsRegistry(this);
			m_toolTip = new ChartFancyToolTipInfo();

			this.Type = type;
			this.Text = name;
		}
        protected void Dispose()
        {
            if (m_name != null)
                m_name = null;
            if (m_seriesConfigCollection != null)
            {
                m_seriesConfigCollection.Changed -= new EventHandler(this.OnConfigItemsChanged);
                m_seriesConfigCollection = null;
            }
            if (m_pointFormats != null)
                m_pointFormats = null;
            if (m_toolTip != null)
                m_toolTip = null;
        }
		/// <summary>
		/// Initializes a new instance of the ChartSeries class.
		/// </summary>
		/// <param name="name">An name of series. This value will be set to <see cref="ChartSeries.Text"/> property too.</param>
		public ChartSeries(string name)
			: this(name, ChartSeriesType.Column)
		{
		}
		#endregion

		#region Public methods
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		public ChartStyleInfo GetOfflineStyle()
		{
			ChartStyleInfo style = this.StylesImpl.ComposedStyles.GetOfflineStyle();

			if (m_model != null)
			{
				int seriesIndex = m_model.Series.IndexOf(this);
				this.OnPrepareSeriesStyleInfo(style, seriesIndex);
			}

			return style;
		}
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		public ChartStyleInfo GetOfflineStyle(int index)
		{
			ChartStyleInfo style = this.StylesImpl.ComposedStyles.GetOfflineStyle(index);
			// Give users a chance to change the style.
			this.OnPrepareStyleInfo(style, index);
			return style;
		}
		/// <summary>
		/// Refer <see cref="IChartSeriesStylesHost.GetStylesMap"/>
		/// </summary>
		/// <returns></returns>
		public ChartBaseStylesMap GetStylesMap()
		{
			return m_model == null ? null : m_model.GetStylesMap();
		}
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		internal void ResetLegend()
		{
			this.UpdateLegendItem();
		}
		/// <summary>
		/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override string ToString()
		{
			return string.Format("{0} - [ {1} ]", base.ToString(), this.Name);
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Factory method that gets called to create an instance of an implementation of IChartSeriesSummary
		/// </summary>
		/// <returns>
		///     A Syncfusion.Windows.Forms.Chart.IChartSeriesSummary value.
		/// </returns>
		private IChartSeriesSummary OnCreateSeriesSummaryImpl()
		{
			return new ChartSeriesSummary();
		}
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal IEditableChartSeriesModel GetEditableData()
		{
			return this.SeriesModel as IEditableChartSeriesModel;
		}
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool IsEditableData()
		{
			return this.SeriesModel is IEditableChartSeriesModel;
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		internal protected virtual void OnAppearanceChanged(EventArgs e)
		{
			m_needUpdateLegend = true;
			this.UpdateRenderer(ChartUpdateFlags.All);

			if (this.AppearanceChanged != null)
			{
				this.AppearanceChanged(this, e);
			}
		}
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void OnChartStyleChanged(object sender, ChartStyleChangedEventArgs args)
		{
			this.InvalidateStyles();
		}

		/// <summary>
		/// Factory method that gets called to create an instance of an implementation of <see cref="IChartSeriesModel"/>. The default instance
		/// created is of type <see cref="ChartSeriesModel"/>.
		/// </summary>
		///
		/// <returns>
		/// An instance that implements IChartSeriesModel.
		/// </returns>
		protected virtual IChartSeriesModel OnCreateSeriesModel()
		{
			return new ChartSeriesModel();
		}
		/// <summary>
		/// Factory method that gets called to create an instance of an implementation of <see cref="IChartSeriesStylesModel"/>. The default instance
		/// created is of type <see cref="ChartSeriesStylesModel"/>.
		/// </summary>
		/// <returns>
		/// An instance that implements IChartSeriesStylesModel.
		/// </returns>
		protected virtual IChartSeriesStylesModel OnCreateSeriesStylesModelImpl()
		{
			IChartSeriesStylesModel res = new ChartSeriesStylesModel(this);

			res.Changed += new ChartStyleChangedEventHandler(OnChartStyleChanged);

			return res;
		}
		/// <summary>
		/// After composing the style of each series, the chart's style system will call this method before the style is used for display.
		/// Override this method if you wish to change the contents of the series ChartStyleInfo object that is passed in. For instance, if
		/// you wish to change the back color of the series being rendered based on external criteria, you could check this criteria when this
		/// method gets called and change the color of the series based on such. Note that any changes made to the style object in this method
		/// are not permanent but are temporary and lasts only for the current rendering cycle. This makes this method
		/// a convenient place to set transient data based attributes.
		/// </summary>
		/// <param name="styleInfo" type="Syncfusion.Windows.Forms.Chart.ChartStyleInfo">
		///     <para>
		///     ChartStyleInfo object that can be changed.
		///     </para>
		/// </param>
		/// <param name="seriesIndex" type="int">
		///     <para>
		///     The index value of this series in the chart's <see cref="ChartSeriesCollection"/>.
		///     </para>
		/// </param>
		protected virtual void OnPrepareSeriesStyleInfo(ChartStyleInfo styleInfo, int seriesIndex)
		{
			if (seriesIndex > -1)
			{
				if (m_prepareSeriesStyleInfoHandler != null)
				{
					m_prepareSeriesStyleInfoHandler(this, new ChartPrepareStyleInfoEventArgs(styleInfo, seriesIndex));
				}
			}

			#region Prepare interior
			if (styleInfo.Interior == null)
			{
				styleInfo.Interior = new BrushInfo(m_model.ColorModel.GetColor(seriesIndex));
			}
			#endregion
		}
		/// <summary>
		/// After composing the style of each point in a series, the chart's style system will call this method before the style is used for display.
		/// Override this method if you wish to change the contents of the ChartStyleInfo object that is passed in. For instance, if
		/// you wish to change the back color of the series point being rendered based on external criteria, you could check this criteria when this
		/// method gets called and change the color of the point based on such. Note that any changes made to the style object in this method
		/// are not permanent but are temporary and lasts only for the current rendering cycle. This makes this method
		/// a convenient place to set transient data based attributes.
		/// </summary>
		/// <param name="styleInfo" type="Syncfusion.Windows.Forms.Chart.ChartStyleInfo">
		///     <para>
		///      ChartStyleInfo object that can be changed.
		///     </para>
		/// </param>
		/// <param name="index" type="int">
		///     <para>
		///     The index value of the point (in the current series) associated with the style information passed in.
		///     </para>
		/// </param>
		protected virtual void OnPrepareStyleInfo(ChartStyleInfo styleInfo, int index)
		{
			if (m_prepareStyleInfoHandler != null)
			{
				m_prepareStyleInfoHandler(this, new ChartPrepareStyleInfoEventArgs(styleInfo, index));
			}

			#region Prepare interior
			if (this.BaseType != ChartSeriesBaseType.Single)
			{
				int seriesIndex = m_model.Series.IndexOf(this);

				if (seriesIndex != -1)
				{
					if (styleInfo.Interior == null)
					{
						styleInfo.Interior = new BrushInfo(m_model.ColorModel.GetColor(seriesIndex));
					}
				}
			}
			else
			{
				if (styleInfo.Interior == null)
				{
					styleInfo.Interior = new BrushInfo(m_model.ColorModel.GetColor(index));
				}
			}
			#endregion

			#region Prepare text
			if (styleInfo.Text == string.Empty)
			{
				if (styleInfo.TextFormat != string.Empty)
				{
					styleInfo.Text = string.Format(styleInfo.TextFormat, this.Points[index].YValues[0]);
				}
				else
				{
					styleInfo.Text = this.Points[index].YValues[0].ToString();
				}
			}
			#endregion

			#region Prepare tooltips
			if (styleInfo.ToolTip == string.Empty)
			{
				if (styleInfo.ToolTipFormat != string.Empty)
				{
					styleInfo.ToolTip = string.Format(styleInfo.ToolTipFormat, this.Points[index].YValues[0]);
				}
				else
				{
					styleInfo.ToolTip = this.Points[index].YValues[0].ToString();
				}
			}
			#endregion
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void OnSeriesChanged(EventArgs e)
		{
			m_needUpdateLegend = true;
			this.UpdateRenderer(ChartUpdateFlags.Data);

			if (this.SeriesChanged != null)
			{
				this.SeriesChanged(this, e);
			}
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void OnSeriesModelImplChanged()
		{
			// Change the summary's model information.
			this.Summary.ModelImpl = m_seriesModelAdapter;
			this.OnSeriesModelChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
			this.RaiseModelImplChanged();
		}
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void OnSeriesModelImplChanging()
		{
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void OnSeriesStylesImplChanged()
		{
			m_stylesIndexer = new ChartStyleInfoIndexer(m_seriesStylesModel);

			this.WireStylesEvents();
			this.RaiseSeriesStylesImplChanged();
		}
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void OnSeriesStylesImplChanging()
		{
			this.UnwireStylesEvents();
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void OnSeriesSummaryImplChanged()
		{
		}
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void OnSeriesSummaryImplChanging()
		{
		}

		/// <summary>
		/// Called when color model is changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnColorModelChanged(object sender, EventArgs e)
		{
			this.StylesImpl.ComposedStyles.ResetCache();
			this.UpdateRenderer(ChartUpdateFlags.Styles);
			this.UpdateLegendItem();
		}

		#region Raise methods
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void RaiseModelImplChanged()
		{
			if (this.DataChanged != null)
			{
				this.DataChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
			}
		}
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void RaiseSeriesStylesImplChanged()
		{
			this.InvalidateStyles();
		}
		#endregion

		#region Set methods
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		private void SetSummaryImpl(IChartSeriesSummary summaryImpl)
		{
			if (m_seriesSummary != summaryImpl)
			{
				this.OnSeriesSummaryImplChanging();
				m_seriesSummary = summaryImpl;
				m_seriesSummary.ModelImpl = m_seriesModelAdapter;
				this.OnSeriesSummaryImplChanged();
			}
		}
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void SetSeriesModel(IChartSeriesModel seriesModel)
		{
			if (m_seriesModel != seriesModel)
			{
				this.OnSeriesModelImplChanging();

				if (m_seriesModel != null)
				{
					m_seriesModel.Changed -= new ListChangedEventHandler(OnSeriesModelChanged);
				}

				m_seriesModel = seriesModel;

				if (m_seriesModel != null)
				{
					m_seriesModel.Changed += new ListChangedEventHandler(OnSeriesModelChanged);
				}

				this.OnSeriesModelImplChanged();
			}
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		private void SetSeriesModelAdapter(IChartSeriesModel seriesModelAdapter)
		{

			if (m_seriesModelAdapter != seriesModelAdapter)
			{
				this.OnSeriesModelImplChanging();

				if (m_seriesModelAdapter != null)
				{
					m_seriesModelAdapter.Changed -= new ListChangedEventHandler(OnSeriesModelChanged);
				}

				m_seriesModelAdapter = seriesModelAdapter;
				m_seriesModelAdapter = seriesModelAdapter;

				if (m_pointsIndexer != null)
				{
					m_pointsIndexer.SeriesModel = seriesModelAdapter;
				}


				if (m_seriesModelAdapter != null)
				{
					m_seriesModelAdapter.Changed += new ListChangedEventHandler(OnSeriesModelChanged);
				}

				this.OnSeriesModelImplChanged();
			}
		}
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void SetStylesImpl(IChartSeriesStylesModel seriesStylesModel)
		{
			if (m_seriesStylesModel != seriesStylesModel)
			{
				this.OnSeriesStylesImplChanging();

				if (m_seriesStylesModel != null && m_seriesStylesModel.Style != null)
				{
					m_seriesStylesModel.Style.Changed -= new Syncfusion.Styles.StyleChangedEventHandler(Style_Changed);
				}

				m_seriesStylesModel = seriesStylesModel;

				if (m_seriesStylesModel != null && m_seriesStylesModel.Style != null)
				{
					m_seriesStylesModel.Style.Changed += new Syncfusion.Styles.StyleChangedEventHandler(Style_Changed);
				}

				this.OnSeriesStylesImplChanged();
			}
		}
		#endregion

		#region Wire/Unwire methods
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void UnwireStylesEvents()
		{
			if (m_seriesStylesModel != null)
			{
				m_seriesStylesModel.Changed -= new ChartStyleChangedEventHandler(this.OnChartStyleChanged);
			}
		}
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void WireStylesEvents()
		{
			m_seriesStylesModel.Changed += new ChartStyleChangedEventHandler(this.OnChartStyleChanged);
		}
		#endregion

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		private void OnConfigItemsChanged(object sender, EventArgs args)
		{
			this.UpdateRenderer(ChartUpdateFlags.Config);
			this.OnAppearanceChanged(EventArgs.Empty);
		}
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		private void Style_Changed(object sender, Syncfusion.Styles.StyleChangedEventArgs e)
		{
			this.InvalidateStyles();
			this.OnAppearanceChanged(EventArgs.Empty);
		}
		/// <summary>
		/// Called when series model is changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.ComponentModel.ListChangedEventArgs"/> instance containing the event data.</param>
		private void OnSeriesModelChanged(object sender, ListChangedEventArgs args)
		{
			m_renderer.Update(ChartUpdateFlags.Data);
			m_renderer.DataUpdate(args);

			if (this.DataChanged != null)
			{
				this.DataChanged(this, args);
			}
		}
		/// <summary>
		/// Resets the settings of the legend item.
		/// </summary>
		private void UpdateLegendItem()
		{
			if (m_legendItem != null)
			{
				m_legendItem.Refresh(m_legendItemUseSeriesStyle);

				if (this.BaseType == ChartSeriesBaseType.Single && m_type != ChartSeriesType.HeatMap)
				{
					m_legendItem.Children.Clear();

					for (int i = 0; i < Points.Count; i++)
					{
						ChartSeriesLegendItem legendItem = new ChartSeriesLegendItem(this, i);
						legendItem.Refresh(m_legendItemUseSeriesStyle);
						m_legendItem.Children.Add(legendItem);
					}
				}
				else
				{
					m_legendItem.Children.Clear();
				}
			}
		}
		/// <summary>
		/// Updates the renderer.
		/// </summary>
		/// <param name="flags">The flags.</param>
		internal void UpdateRenderer(ChartUpdateFlags flags)
		{
			if (m_renderer != null)
			{
				m_renderer.Update(flags);
			}
		}
		/// <summary>
		/// Invalidates the styles.
		/// </summary>
		private void InvalidateStyles()
		{
			this.StylesImpl.ComposedStyles.ResetCache();
			this.UpdateRenderer(ChartUpdateFlags.Styles);
			this.UpdateLegendItem();
		}
		#endregion
	}
}